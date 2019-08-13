using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IKFootSystem : MonoBehaviour
{
	public bool IKFootSystemEnabled = true;

	public playerController playerControllerManager;
	public Animator mainAnimator;
	public LayerMask layerMask;

	public float IKWeightEnabledSpeed = 10;
	public float IKWeightDisabledSpeed = 2;
	public float hipsMovementSpeed = 1.2f;
	public float extraFootOffset = 0.005f;

	public float hipsPositionUpClampAmount;
	public float hipsPositionDownClampAmount;

	public Transform hips;
	public List<legInfo> legInfoList = new List<legInfo> ();

	float newHipsOffset;

	public bool canUseIkFoot;

	bool usingIKFootPreviously;

	Vector3 hipsPosition;
	float currentHipsOffset;

	bool playerOnGround;

	bool currentLegIsRight;

	Transform mainTransform;
	RaycastHit hit;

	legInfo currentLegInfo;

	float currentRaycastDistance;
	float currentAdjustmentSpeed;
	Vector3 currentRaycastPosition;
	float newRaycastDistance;

	float newOffset;

	Vector3 newFootPosition;
	Quaternion newFootRotation;
	float targetWeight;
	Vector3 localFootPosition;

	bool initialPositionAssigned;

	Vector3 newRaycastPosition;
	Vector3 newLocalHipsPosition;

	bool playerIsMoving;

	void Start ()
	{
		mainTransform = transform;	

		for (int i = 0; i < legInfoList.Count; i++) {
			legInfoList [i].offset = mainTransform.InverseTransformPoint (legInfoList [i].foot.position).y;
			legInfoList [i].maxLegLength = mainTransform.InverseTransformPoint (legInfoList [i].lowerLeg.position).y;
		}

		hipsPosition = hips.position;
	}

	void LateUpdate ()
	{
		if (!IKFootSystemEnabled) {
			return;
		}

		canUseIkFoot = (!playerControllerManager.isPlayerDead () && !playerControllerManager.isPlayerDriving () && !playerControllerManager.isPlayerOnFirstPerson () &&
		(!playerControllerManager.isPlayerOnFFOrZeroGravityModeOn () || playerControllerManager.isPlayerOnGround ()));

		if (usingIKFootPreviously != canUseIkFoot) {
			if (!usingIKFootPreviously) {
				hipsPosition = mainTransform.InverseTransformPoint (hips.position);
			}
			usingIKFootPreviously = canUseIkFoot;
		}

		if (!canUseIkFoot) {
			return;
		}

		hips.position = mainTransform.TransformPoint (hipsPosition);

		if (!initialPositionAssigned) {
			initialPositionAssigned = true;
		}
	}

	public void OnAnimatorIK ()
	{
		if (!IKFootSystemEnabled) {
			return;
		}

		if (!canUseIkFoot) {
			return;
		}

		playerOnGround = playerControllerManager.isPlayerOnGround ();

		newHipsOffset = 0f;
		if (playerOnGround) {
			for (int i = 0; i < legInfoList.Count; i++) {
				currentLegInfo = legInfoList [i];
				//raycast from the foot

				currentRaycastPosition = getNewRaycastPosition (currentLegInfo.foot, currentLegInfo.lowerLeg, out currentRaycastDistance);
				newRaycastDistance = currentRaycastDistance + currentLegInfo.offset + currentLegInfo.maxLegLength - (extraFootOffset * 2);

				if (Physics.Raycast (currentRaycastPosition, -mainTransform.up, out hit, newRaycastDistance, layerMask)) {
					currentLegInfo.raycastDistance = currentRaycastDistance;
					currentLegInfo.surfaceDistance = hit.distance;
					currentLegInfo.surfacePoint = hit.point;
					currentLegInfo.surfaceNormal = hit.normal;
				} else {
					currentLegInfo.surfaceDistance = float.MaxValue;
				}

				//raycast from the toe, if a closer object is found, the raycast used is the one in the toe

				currentRaycastPosition = getNewRaycastPosition (currentLegInfo.toes, currentLegInfo.lowerLeg, out currentRaycastDistance);
				newRaycastDistance = currentRaycastDistance + currentLegInfo.offset + currentLegInfo.maxLegLength - (extraFootOffset * 2);

				if (Physics.Raycast (currentRaycastPosition, -mainTransform.up, out hit, newRaycastDistance, layerMask)) {
					if (hit.distance < currentLegInfo.surfaceDistance && hit.normal == mainTransform.up) {
						currentLegInfo.raycastDistance = currentRaycastDistance;
						currentLegInfo.surfaceDistance = hit.distance;
						currentLegInfo.surfacePoint = hit.point;
						currentLegInfo.surfaceNormal = hit.normal;
					}
				}

				if (currentLegInfo.surfaceDistance != float.MaxValue) {
					newOffset = currentLegInfo.surfaceDistance - currentLegInfo.raycastDistance - mainTransform.InverseTransformPoint (currentLegInfo.foot.position).y;
					if (newOffset > newHipsOffset) {
						newHipsOffset = newOffset;
					}
				}
			}
		}

		playerIsMoving = playerControllerManager.isPlayerMoving (0);
		if (playerIsMoving) {
			newHipsOffset = Mathf.Clamp (newHipsOffset, hipsPositionDownClampAmount, hipsPositionUpClampAmount);
		}

		//set hips position
		if (initialPositionAssigned) {
			currentHipsOffset = Mathf.Lerp (currentHipsOffset, newHipsOffset, hipsMovementSpeed * Time.fixedDeltaTime);
		} else {
			currentHipsOffset = newHipsOffset;
		}

		hipsPosition = mainTransform.InverseTransformPoint (hips.position);
		hipsPosition.y -= currentHipsOffset;

		//set position and rotation on player's feet
		for (int i = 0; i < legInfoList.Count; i++) {
			currentLegInfo = legInfoList [i];

			newFootPosition = mainAnimator.GetIKPosition (currentLegInfo.IKGoal);
			newFootRotation = mainAnimator.GetIKRotation (currentLegInfo.IKGoal);
			targetWeight = currentLegInfo.IKWeight - 1;

			currentAdjustmentSpeed = IKWeightDisabledSpeed;

			if (playerOnGround) {
				if (currentLegInfo.surfaceDistance != float.MaxValue) {
					if (mainTransform.InverseTransformDirection (newFootPosition - currentLegInfo.surfacePoint).y - currentLegInfo.offset - extraFootOffset - currentHipsOffset < 0) {
						localFootPosition = mainTransform.InverseTransformPoint (newFootPosition);
						localFootPosition.y = mainTransform.InverseTransformPoint (currentLegInfo.surfacePoint).y;

						newFootPosition = mainTransform.TransformPoint (localFootPosition) + mainTransform.up * (currentLegInfo.offset + currentHipsOffset - extraFootOffset);
						newFootRotation = Quaternion.LookRotation (Vector3.Cross (currentLegInfo.surfaceNormal, newFootRotation * -Vector3.right), mainTransform.up);

						targetWeight = currentLegInfo.IKWeight + 1;
						currentAdjustmentSpeed = IKWeightEnabledSpeed;
					}
				}
			}

			if (initialPositionAssigned) {
				currentLegInfo.IKWeight = Mathf.Clamp01 (Mathf.Lerp (currentLegInfo.IKWeight, targetWeight, currentAdjustmentSpeed * Time.fixedDeltaTime));
			} else {
				currentLegInfo.IKWeight = Mathf.Clamp01 (targetWeight);
			}
				
			mainAnimator.SetIKPosition (currentLegInfo.IKGoal, newFootPosition);
			mainAnimator.SetIKRotation (currentLegInfo.IKGoal, newFootRotation);
			mainAnimator.SetIKPositionWeight (currentLegInfo.IKGoal, currentLegInfo.IKWeight);
			mainAnimator.SetIKRotationWeight (currentLegInfo.IKGoal, currentLegInfo.IKWeight);
		}
	}

	public Vector3 getNewRaycastPosition (Transform targetTransform, Transform lowerLeg, out float newDistance)
	{
		newRaycastPosition = mainTransform.InverseTransformPoint (targetTransform.position);
		newLocalHipsPosition = mainTransform.InverseTransformPoint (lowerLeg.position);

		newDistance = (newLocalHipsPosition.y - newRaycastPosition.y);
		newRaycastPosition.y = newLocalHipsPosition.y;

		return mainTransform.TransformPoint (newRaycastPosition);
	}

	public void setLegsInfo (Transform newHips, Transform rightLowerLeg, Transform leftLowerLeg, Transform rightFoot, Transform leftFoot, Transform rightToes, Transform leftToes)
	{
		hips = newHips;
		for (int i = 0; i < legInfoList.Count; i++) {
			if (legInfoList [i].IKGoal == AvatarIKGoal.RightFoot) {
				legInfoList [i].lowerLeg = rightLowerLeg;
				legInfoList [i].foot = rightFoot;
				legInfoList [i].toes = rightToes;
			} else {
				legInfoList [i].lowerLeg = leftLowerLeg;
				legInfoList [i].foot = leftFoot;
				legInfoList [i].toes = leftToes;
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<IKFootSystem> ());
		#endif
	}

	[System.Serializable]
	public class legInfo
	{
		public string Name;
		public AvatarIKGoal IKGoal;
		public Transform lowerLeg;
		public Transform foot;
		public Transform toes;
		public float offset;
		public float IKWeight;
		public float maxLegLength;
		public float raycastDistance;
		public float surfaceDistance;
		public Vector3 surfacePoint;
		public Vector3 surfaceNormal;
	}
}