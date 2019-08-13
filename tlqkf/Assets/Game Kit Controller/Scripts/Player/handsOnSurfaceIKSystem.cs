using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class handsOnSurfaceIKSystem : MonoBehaviour
{
	public bool adjustHandsToSurfaces;
	public LayerMask layerToAdjustHandsToSurfaces;

	public List<handsToSurfaceInfo> handsToSurfaceInfoList = new List<handsToSurfaceInfo> ();

	public float movementSpeedLerpSpeed = 2;

	public float waitTimeAfterGroundToCheckSurface = 1.5f;
	public float waitTimeAfterCrouchToCheckSurface = 1.5f;

	public float weightLerpSpeed = 6;

	public bool usingHands;

	public float maxHandsDistance = 3;

	public float minMovingInputToUseWalkSpeed = 0.5f;
	public float minIKWeightToUseWalkSpeed = 0.4f;

	public Animator animator;
	public IKSystem IKManager;
	public playerController playerControllerManager;
	public closeCombatSystem closeCombatManager;

	public bool showGizmo;
	public Color gizmoColor = Color.white;
	public float gizmoRadius;

	handsToSurfaceInfo currentHandInfo;
	Vector3 rayDirection;

	RaycastHit hit;
	float movementSpeed;
	float currentMovementSpeed;

	bool isThirdPersonView;
	bool isBusy;

	bool onGround;

	bool dead;

	bool groundChecked;
	float lastTimeOnGround;

	bool crouchingChecked;
	float lastTimeCrouch;

	bool combatModeActive;

	Vector3 currentPositionCenter;

	float currentRotationSpeed;

	float originalWeightLerpSpeed;

	bool crouching;

	Transform rightHandTransform;
	Transform leftHandTransform;

	void Start ()
	{
		originalWeightLerpSpeed = weightLerpSpeed;

		rightHandTransform = animator.GetBoneTransform (HumanBodyBones.RightHand);

		leftHandTransform = animator.GetBoneTransform (HumanBodyBones.LeftHand);
	}

	void FixedUpdate ()
	{
		if (!adjustHandsToSurfaces) {
			return;
		}

		combatModeActive = closeCombatManager.isCurrentPlayerMode ();

		isThirdPersonView = !playerControllerManager.isPlayerOnFirstPerson ();

		usingHands = IKManager.getUsingHands ();

		onGround = playerControllerManager.isPlayerOnGround ();

		isBusy = playerIsBusy ();

		crouching = playerControllerManager.isCrouching ();

		dead = playerControllerManager.isPlayerDead ();

		if (groundChecked != onGround) {
			groundChecked = onGround;

			if (onGround) {
				lastTimeOnGround = Time.time;
			}
		}

		if (crouchingChecked != crouching) {
			crouchingChecked = crouching;

			if (!crouching) {
				lastTimeCrouch = Time.time;
			}
		}

		if (adjustHandsToSurfaces) {
			if (!isBusy) {
				for (int j = 0; j < handsToSurfaceInfoList.Count; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					currentHandInfo.surfaceFound = false;
					currentHandInfo.multipleRaycastHitNormal = Vector3.zero;
					currentHandInfo.multipleRaycastHitPoint = Vector3.zero;
					currentHandInfo.numberOfSurfacesFound = 0;
					currentHandInfo.multipleRaycastDirection = Vector3.zero;

					for (int k = 0; k < currentHandInfo.raycastTransformList.Count; k++) {
						rayDirection = currentHandInfo.raycastTransformList [k].raycastTransform.forward;
						if (currentHandInfo.raycastTransformList [k].raycastEnabled &&
						    Physics.Raycast (currentHandInfo.raycastTransformList [k].raycastTransform.position, rayDirection, out hit, 
							    currentHandInfo.raycastTransformList [k].rayCastDistance, layerToAdjustHandsToSurfaces)) {

							currentHandInfo.raycastTransformList [k].hitPoint = hit.point;
							currentHandInfo.raycastTransformList [k].hitNormal = hit.normal;
							currentHandInfo.surfaceFound = true;
							currentHandInfo.multipleRaycastHitNormal += hit.normal;
							currentHandInfo.multipleRaycastHitPoint += hit.point;

							currentHandInfo.numberOfSurfacesFound++;
							currentHandInfo.multipleRaycastDirection += rayDirection;

							Debug.DrawRay (currentHandInfo.raycastTransformList [k].raycastTransform.position, rayDirection * hit.distance, Color.green);
						} else {
							Debug.DrawRay (currentHandInfo.raycastTransformList [k].raycastTransform.position, 
								rayDirection * currentHandInfo.raycastTransformList [k].rayCastDistance, Color.red);
						}
					}

					if (currentHandInfo.surfaceFound) {
						currentHandInfo.handPosition = 
							(currentHandInfo.multipleRaycastHitPoint / currentHandInfo.numberOfSurfacesFound) +
						(currentHandInfo.handSurfaceOffset * currentHandInfo.multipleRaycastDirection / currentHandInfo.numberOfSurfacesFound);      

						Quaternion rot = Quaternion.LookRotation (currentHandInfo.multipleRaycastDirection / currentHandInfo.numberOfSurfacesFound);           
						currentHandInfo.handRotation = 
							Quaternion.FromToRotation (transform.up, currentHandInfo.multipleRaycastHitNormal / currentHandInfo.numberOfSurfacesFound) * rot;   

						currentHandInfo.elbowPosition =
							((currentHandInfo.multipleRaycastHitPoint / currentHandInfo.numberOfSurfacesFound) + currentHandInfo.raycastTransform.position) / 2 - transform.up * 0.1f;

						currentHandInfo.handWeight = 1;

						if (currentHandInfo.useMinDistance) {
							currentHandInfo.currentDistance = GKC_Utils.distance (currentHandInfo.raycastTransform.position, currentHandInfo.handPosition);
							if (currentHandInfo.currentDistance < currentHandInfo.minDistance) {
								currentHandInfo.surfaceFound = false;
							}
						}
					} 

					if (!currentHandInfo.surfaceFound || !onGround || Time.time < waitTimeAfterGroundToCheckSurface + lastTimeOnGround ||
					    crouching || Time.time < waitTimeAfterCrouchToCheckSurface + lastTimeCrouch || dead) {
						currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
						currentHandInfo.handRotation = Quaternion.LookRotation (transform.forward);

						currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

						currentHandInfo.handWeight = 0;
					}

					if (playerControllerManager.isPlayerMoving (minMovingInputToUseWalkSpeed) && currentHandInfo.surfaceFound && currentHandInfo.currentHandWeight > minIKWeightToUseWalkSpeed) {
						movementSpeed = currentHandInfo.walkingMovementSpeed;
					} else {
						movementSpeed = currentHandInfo.movementSpeed;
					}

					currentMovementSpeed = Mathf.Lerp (currentMovementSpeed, movementSpeed, Time.deltaTime * movementSpeedLerpSpeed);

					currentHandInfo.currentHandPosition = Vector3.Lerp (currentHandInfo.currentHandPosition, currentHandInfo.handPosition, Time.deltaTime * currentMovementSpeed);            

					currentRotationSpeed = Time.deltaTime * currentHandInfo.rotationSpeed;
					if (currentRotationSpeed > 0) {
						currentHandInfo.currentHandRotation = Quaternion.Lerp (currentHandInfo.currentHandRotation, currentHandInfo.handRotation, currentRotationSpeed);
					}

					currentHandInfo.currentElbowPosition = Vector3.Lerp (currentHandInfo.currentElbowPosition, currentHandInfo.elbowPosition, Time.deltaTime * currentMovementSpeed);            

					currentHandInfo.currentHandWeight = Mathf.Lerp (currentHandInfo.currentHandWeight, currentHandInfo.handWeight, Time.deltaTime * weightLerpSpeed);

					if (currentHandInfo.currentHandWeight < 0.01f) {
						currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
						currentHandInfo.handRotation = Quaternion.LookRotation (transform.forward);

						currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

						currentHandInfo.currentHandPosition = currentHandInfo.handPosition;            

						currentHandInfo.currentHandRotation = currentHandInfo.handRotation;

						currentHandInfo.currentElbowPosition = currentHandInfo.elbowPosition;
					}
				}
			} else {
				for (int j = 0; j < handsToSurfaceInfoList.Count; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					currentHandInfo.surfaceFound = false;

					currentHandInfo.handWeight = 0;

					currentHandInfo.currentHandWeight = 0;
					currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
					currentHandInfo.handRotation = Quaternion.LookRotation (transform.forward);

					currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

					currentHandInfo.currentHandPosition = currentHandInfo.handPosition;            

					currentHandInfo.currentHandRotation = currentHandInfo.handRotation;

					currentHandInfo.currentElbowPosition = currentHandInfo.elbowPosition;
				}

			}
		}
	}

	bool isRightHand;

	void OnAnimatorIK ()
	{
		if (adjustHandsToSurfaces) {

			if (!isBusy) {

				for (int j = 0; j < handsToSurfaceInfoList.Count; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					currentPositionCenter = transform.position + transform.up;

					isRightHand = currentHandInfo.isRightHand;

					if (float.IsNaN (currentHandInfo.currentHandPosition.x)) {
						if (isRightHand) {
							currentHandInfo.currentHandPosition.x = rightHandTransform.position.x;
						} else {
							currentHandInfo.currentHandPosition.x = leftHandTransform.position.x;
						}
					}

					if (float.IsNaN (currentHandInfo.currentHandPosition.y)) {
						if (isRightHand) {
							currentHandInfo.currentHandPosition.y = rightHandTransform.position.y;
						} else {
							currentHandInfo.currentHandPosition.y = leftHandTransform.position.y;
						}
					}

					if (float.IsNaN (currentHandInfo.currentHandPosition.z)) {
						if (isRightHand) {
							currentHandInfo.currentHandPosition.z = rightHandTransform.position.z;
						} else {
							currentHandInfo.currentHandPosition.z = leftHandTransform.position.z;
						}
					}

					currentHandInfo.currentHandPosition.x = 
						Mathf.Clamp (currentHandInfo.currentHandPosition.x, currentPositionCenter.x - maxHandsDistance, currentPositionCenter.x + maxHandsDistance);
					currentHandInfo.currentHandPosition.y =
						Mathf.Clamp (currentHandInfo.currentHandPosition.y, currentPositionCenter.y - maxHandsDistance, currentPositionCenter.y + maxHandsDistance);
					currentHandInfo.currentHandPosition.z = 
						Mathf.Clamp (currentHandInfo.currentHandPosition.z, currentPositionCenter.z - maxHandsDistance, currentPositionCenter.z + maxHandsDistance);

					animator.SetIKRotationWeight (currentHandInfo.IKGoal, currentHandInfo.currentHandWeight);
					animator.SetIKPositionWeight (currentHandInfo.IKGoal, currentHandInfo.currentHandWeight);
					animator.SetIKPosition (currentHandInfo.IKGoal, currentHandInfo.currentHandPosition);
					animator.SetIKRotation (currentHandInfo.IKGoal, currentHandInfo.currentHandRotation);

					animator.SetIKHintPositionWeight (currentHandInfo.IKHint, currentHandInfo.currentHandWeight);
					animator.SetIKHintPosition (currentHandInfo.IKHint, currentHandInfo.currentElbowPosition);
				}
			} 
		}
	}

	public bool playerIsBusy ()
	{
		return usingHands || !isThirdPersonView || playerControllerManager.isPlayerOnFFOrZeroGravityModeOn () || combatModeActive;
	}

	public void setWeightLerpSpeedValue (float newValue)
	{
		weightLerpSpeed = newValue;
	}

	public void setOriginalWeightLerpSpeed ()
	{
		setWeightLerpSpeedValue (originalWeightLerpSpeed);
	}

	//draw every floor position and a line between floors
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int j = 0; j < handsToSurfaceInfoList.Count; j++) {
				Gizmos.color = Color.green;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].handPosition, gizmoRadius);

				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].elbowPosition, gizmoRadius);

				Gizmos.color = gizmoColor;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].currentHandPosition, gizmoRadius);
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].currentElbowPosition, gizmoRadius);
			}
		}
	}

	[System.Serializable]
	public class handsToSurfaceInfo
	{
		public string Name;
		public Transform raycastTransform;

		public bool useMultipleRaycast;
		public List<multipleRayInfo> raycastTransformList = new List<multipleRayInfo> ();

		public AvatarIKHint IKHint;
		[HideInInspector] public Vector3 elbowPosition;
		[HideInInspector] public Vector3 currentElbowPosition;

		public bool isRightHand;
		public AvatarIKGoal IKGoal;
		public float handSurfaceOffset;
		public float rayCastDistance;

		public float currentHandWeight;

		public float movementSpeed;
		public float rotationSpeed;

		public float walkingMovementSpeed;

		[HideInInspector] public bool surfaceFound;

		public bool useMinDistance;
		public float minDistance;

		[HideInInspector]public float currentDistance;

		[HideInInspector] public Vector3 multipleRaycastDirection;

		[HideInInspector] public int numberOfSurfacesFound;
		[HideInInspector] public Vector3 multipleRaycastHitNormal;
		[HideInInspector] public Vector3 multipleRaycastHitPoint;

		[HideInInspector] public float handWeight;
		[HideInInspector] public Vector3 handPosition;
		[HideInInspector] public Quaternion handRotation;

		[HideInInspector] public Vector3 currentHandPosition;
		[HideInInspector] public Quaternion currentHandRotation;

		public Transform noSurfaceHandPosition;
		public Transform noSurfaceElbowPosition;
	}

	[System.Serializable]
	public class multipleRayInfo
	{
		public Transform raycastTransform;
		public bool surfaceFound;
		[HideInInspector] public Vector3 hitPoint;
		[HideInInspector] public Vector3 hitNormal;
		public float rayCastDistance;
		public bool raycastEnabled = true;
	}
}