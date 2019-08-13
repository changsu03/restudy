using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class headTrack : MonoBehaviour
{
	public Animator animator;
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;
	public IKSystem IKManager;
	public otherPowers powersManager;

	public bool useHeadRangeRotation = true;
	public Vector2 rangeAngleX = new Vector2 (-90, 90);
	public Vector2 rangeAngleY = new Vector2 (-90, 90);

	public Transform head;

	[Range (0, 1)] public float headWeight = 1;
	[Range (0, 1)] public float bodyWeight = 0.4f;
	public float rotationSpeed = 3;
	public float weightChangeSpeed = 2;
	public bool useTimeToChangeTarget;
	public float timeToChangeTarget;
	public LayerMask obstacleLayer;

	public bool showGizmo;
	public float gizmoRadius = 0.2f;
	public float arcGizmoRadius;

	public bool headTrackEnabled;

	public bool lookInCameraDirection;
	public Transform cameraTargetToLook;
	public Vector2 cameraRangeAngleX = new Vector2 (-90, 90);
	public Vector2 cameraRangeAngleY = new Vector2 (-90, 90);
	[Range (0, 1)] public float cameraHeadWeight = 1;
	[Range (0, 1)] public float cameraBodyWeight = 0.4f;

	public bool lookInOppositeDirectionOutOfRange;
	public Transform oppositeCameraTargetToLook;
	public Transform oppositeCameraTargetToLookParent;

	public float oppositeCameraParentRotationSpeed;
	public bool lookBehindIfMoving;
	public float lookBehindRotationSpeed;
	float currentParentRotationSpeed;

	public bool useDeadZone;
	public float deadZoneLookBehind = 10;

	public bool playerCanLookState;

	public bool useHeadTrackTarget;
	public Transform headTrackTargeTransform;
	Transform headTrackTargetParent;
	Vector3 originalHeadTrackTargetTransformPosition;

	Vector2 currentRangeAngleX;
	Vector2 currentRangeAngleY;
	float currentHeadWeightValue;
	float currentBodyWeightValue;

	List<headTrackTarget> lookTargetList = new List<headTrackTarget> ();

	public bool useTargetsToIgnoreList;
	public List<GameObject> targetsToIgnoreList = new List<GameObject> ();

	float currentHeadWeight;
	float currentbodyWeight;
	float headHeight;
	float lastTimeTargetChanged;

	headTrackTarget currentLookTarget;

	Vector3 currentPositionToLook;
	Vector3 currentDirectionToLook;
	public Vector3 temporalDirectionToLook;

	bool headTrackPaused;

	bool positionToLookFound;
	Vector3 positionToLook;
	Vector3 headPosition;
	bool lookingInCameraDirection;
	bool currentTargetVisible;
	bool targetOnHeadRange;
	float lookTargetDistance;
	bool lookingAtRight;

	Vector3 currentOppositeCameraParentRotation;

	float originalCameraBodyWeight;
	float cameraBodyWeightTarget;

	float cameraBodyWeightSpeed = 2;

	void Start ()
	{
		head = animator.GetBoneTransform (HumanBodyBones.Head);

		if (head) {
			headHeight = GKC_Utils.distance (transform.position, head.position);
		}

		originalCameraBodyWeight = cameraBodyWeight;
		cameraBodyWeightTarget = cameraBodyWeight;

		if (useHeadTrackTarget) {
			originalHeadTrackTargetTransformPosition = headTrackTargeTransform.localPosition;
			headTrackTargetParent = headTrackTargeTransform.parent;
		}
	}

	void Update ()
	{
		if (lookInOppositeDirectionOutOfRange) {
			float horizontalAngle = Vector3.SignedAngle (transform.forward, playerCameraManager.getCameraTransform ().forward, transform.up);

			if (horizontalAngle > 0) {
				horizontalAngle = 180 - horizontalAngle;

				lookingAtRight = true;
			} else {
				horizontalAngle = -(180 + horizontalAngle);

				lookingAtRight = false;
			}

			if (lookBehindIfMoving && (playerControllerManager.getVerticalInput () < -0.5f) && Math.Abs (horizontalAngle) < 30) {

				if (!useDeadZone || Math.Abs (horizontalAngle) > deadZoneLookBehind) {
					if (lookingAtRight) {
						horizontalAngle = cameraRangeAngleY.y;
					} else {
						horizontalAngle = cameraRangeAngleY.x;
					}
				}

				currentParentRotationSpeed = lookBehindRotationSpeed;
			} else {
				currentParentRotationSpeed = oppositeCameraParentRotationSpeed;
			}

			currentOppositeCameraParentRotation = new Vector3 (playerCameraManager.getPivotCameraTransform ().localEulerAngles.x, horizontalAngle, 0);
				
			oppositeCameraTargetToLookParent.localRotation = 
				Quaternion.Lerp (oppositeCameraTargetToLookParent.localRotation, Quaternion.Euler (currentOppositeCameraParentRotation), Time.deltaTime * currentParentRotationSpeed);
		}
	}

	void OnAnimatorIK ()
	{
		playerCanLookState = playerCanLook ();
		if (!playerControllerManager.isPlayerOnFirstPerson () && playerCanLookState && !headTrackPaused) {
			animator.SetLookAtWeight (currentHeadWeight, currentbodyWeight);
			currentPositionToLook = getLookPosition ();
			animator.SetLookAtPosition (currentPositionToLook);

			if (lookTargetList.Count == 0 && currentDirectionToLook == Vector3.zero && !lookInCameraDirection) {
				//print ("pause");
				headTrackPaused = true;
			}
		} else {
			if (currentHeadWeight != 0 && currentbodyWeight != 0) {
				lerpWeights (0, 0);
			}
		}
	}

	public bool playerCanLook ()
	{ 
		if ((playerControllerManager.canMove || powersManager.isSearchingForTeleport ()) && !playerControllerManager.driving && !playerControllerManager.isUsingDevice ()) {

			if ((!playerControllerManager.isPlayerAiming () && IKManager.getHeadWeight () == 0) ||
			    (!playerControllerManager.hasToLookInCameraDirectionOnFreeFire () && !playerControllerManager.isCheckToKeepWeaponAfterAimingWeaponFromShooting ()) ||
			
			    (playerControllerManager.isPlayerAiming () && IKManager.getHeadWeight () == 0 && playerControllerManager.isCheckToKeepWeaponAfterAimingWeaponFromShooting ())) {
				return true;
			}
			return false;
		}
		return false;
	}

	public Vector3 getLookPosition ()
	{
		//get the head position
		headPosition = transform.position + (transform.up * headHeight);

		positionToLookFound = false;
		currentTargetVisible = false;

		//if the player is inside a head track target trigger, check if he can look at it
		if (currentLookTarget) {
			//check if the target is visible according to its configuration
			if (currentLookTarget.lookTargetVisible (headPosition, obstacleLayer)) {
				//get the look position
				positionToLook = currentLookTarget.getLookPositon ();

				//assign the range values
				currentRangeAngleX = rangeAngleX;
				currentRangeAngleY = rangeAngleY;
				currentHeadWeightValue = headWeight;
				currentBodyWeightValue = bodyWeight;

				//check if it the look direction is inside the range
				targetOnHeadRange = isTargetOnHeadRange (positionToLook - headPosition);

				//in that case, set the found position as the one to look
				if (targetOnHeadRange) {
					positionToLookFound = true;

					lookingInCameraDirection = false;
				}
				currentTargetVisible = true;
			} 
		}

		//if there is no target to look or it can be seen by the player or is out of the range of vision, check if the player can look in the camera direction
		if (!positionToLookFound) {
			if (lookInCameraDirection && !playerControllerManager.isLockedCameraStateActive () && !playerCameraManager.isMoveAwayCameraActive ()) {
				//get the look position
				positionToLook = cameraTargetToLook.position;

				//assign the range values
				currentRangeAngleX = cameraRangeAngleX;
				currentRangeAngleY = cameraRangeAngleY;
				currentHeadWeightValue = cameraHeadWeight;
				currentBodyWeightValue = cameraBodyWeight;

				if (cameraBodyWeight != cameraBodyWeightTarget) {
					cameraBodyWeight = Mathf.Lerp (cameraBodyWeight, cameraBodyWeightTarget, cameraBodyWeightSpeed * Time.deltaTime);
				}

				//check if it the look direction is inside the range
				targetOnHeadRange = isTargetOnHeadRange (positionToLook - headPosition);

				//in that case, set the found position as the one to look
				if (targetOnHeadRange) {
					positionToLookFound = true;

					lookingInCameraDirection = true;
				} else {
					if (lookInOppositeDirectionOutOfRange) {
						positionToLook = oppositeCameraTargetToLook.position;

						targetOnHeadRange = true;
						positionToLookFound = true;

						lookingInCameraDirection = true;
					}
				}
			} 
		}

		if (positionToLookFound) {
			temporalDirectionToLook = headPosition + transform.forward - headPosition;

			if ((useHeadRangeRotation && targetOnHeadRange) || !useHeadRangeRotation) {
				if (currentTargetVisible || (lookInCameraDirection && lookingInCameraDirection)) {
					temporalDirectionToLook = positionToLook - headPosition;
				}
			}

			if (showGizmo) {
				lookTargetDistance = GKC_Utils.distance (headPosition, positionToLook);
				Debug.DrawRay (headPosition, temporalDirectionToLook.normalized * lookTargetDistance, Color.black);
			}

			if (useHeadRangeRotation) {
				if (targetOnHeadRange) {
					lerpWeights (currentHeadWeightValue, currentBodyWeightValue);
				} else
					lerpWeights (0, 0);
			} else {
				lerpWeights (currentHeadWeightValue, currentBodyWeightValue);
			}
			getClosestTarget ();
		} else {
			lerpWeights (0, 0);
			getClosestTarget ();
			if (lookInCameraDirection) {
				temporalDirectionToLook = transform.forward;
			} else {
				temporalDirectionToLook = Vector3.zero;
			}
		}

		currentDirectionToLook = Vector3.Lerp (currentDirectionToLook, temporalDirectionToLook, Time.deltaTime * rotationSpeed);
		//Debug.DrawLine (headPosition, currentDirectionToLook, Color.white);
		return headPosition + currentDirectionToLook;
	}

	public bool isTargetOnHeadRange (Vector3 direction)
	{
		float horizontalAngle = Vector3.SignedAngle (transform.forward, direction, transform.up);
		float verticalAngle = Vector3.SignedAngle (transform.forward, direction, transform.right);

		if (Math.Abs (verticalAngle) <= currentRangeAngleX.y && verticalAngle >= currentRangeAngleX.x &&
		    Math.Abs (horizontalAngle) <= currentRangeAngleY.y && horizontalAngle >= currentRangeAngleY.x) {
			return true;
		}

		return false;
	}

	public void lerpWeights (float headWeightTarget, float bodyWeightTarget)
	{
		currentHeadWeight = Mathf.Lerp (currentHeadWeight, headWeightTarget, weightChangeSpeed * Time.deltaTime);
		currentbodyWeight = Mathf.Lerp (currentbodyWeight, bodyWeightTarget, weightChangeSpeed * Time.deltaTime);
	}

	public void getClosestTarget ()
	{
		if (lookTargetList.Count > 0) {
			
			if (!useTimeToChangeTarget || Time.time > lastTimeTargetChanged + timeToChangeTarget) {
				lastTimeTargetChanged = Time.time;

				for (int i = lookTargetList.Count - 1; i >= 0; i--) {
					if (lookTargetList [i] == null) {
						lookTargetList.RemoveAt (i);
					}
				}

				if (lookTargetList.Count == 1) {
					currentLookTarget = lookTargetList [0];
					return;
				}

				float maxDistance = Mathf.Infinity;
				for (int i = 0; i < lookTargetList.Count; i++) {
					float currentDistance = GKC_Utils.distance (transform.position, lookTargetList [i].getLookPositon ());
					if (currentDistance < maxDistance) {
						maxDistance = currentDistance;
						currentLookTarget = lookTargetList [i];
					}
				}
			} 
		} else {
			currentLookTarget = null;
		}
	}

	public void checkHeadTrackTarget (headTrackTarget target)
	{
		if (useTargetsToIgnoreList && target != null && targetsToIgnoreList.Contains (target.gameObject)) {
			return;
		}

		if (!lookTargetList.Contains (target)) {
			lookTargetList.Add (target);
			headTrackPaused = false;
		}
	}

	public void removeHeadTrackTarget (headTrackTarget target)
	{
		if (lookTargetList.Contains (target)) {
			lookTargetList.Remove (target);
		}
	}

	public void setHeadTransform (Transform headTransform)
	{
		head = headTransform;
	}

	public void searchHead ()
	{
		head = animator.GetBoneTransform (HumanBodyBones.Head);

		updateComponent ();
	}

	public void setCameraBodyWeightValue (float newValue)
	{
		cameraBodyWeightTarget = newValue;
	}

	public void setOriginalCameraBodyWeightValue ()
	{
		cameraBodyWeightTarget = originalCameraBodyWeight;
	}

	public Transform getHeadTrackTargetTransform ()
	{
		return headTrackTargeTransform;
	}

	public Transform getHeadTrackTargetParent ()
	{
		return headTrackTargetParent;
	}

	public Vector3 getOriginalHeadTrackTargetPosition ()
	{
		return originalHeadTrackTargetTransformPosition;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<headTrack> ());
		#endif
	}

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

	void DrawGizmos ()
	{
		if (showGizmo && Application.isPlaying) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (currentPositionToLook, gizmoRadius);
		}
	}
}