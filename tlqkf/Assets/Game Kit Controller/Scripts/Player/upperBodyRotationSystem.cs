using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class upperBodyRotationSystem : MonoBehaviour
{
	public bool shakeUpperBodyEnabled = true;

	public Vector3 headLookVector = Vector3.forward;
	public Vector3 headUpVector = Vector3.up;
	public Transform spineTransform;
	public Transform chestTransform;

	public float horizontalThresholdAngleDifference = 5;
	public float verticalThresholdAngleDifference = 5;
	public float horizontalBendingMultiplier = 1;
	public float verticalBendingMultiplier = 1;
	public float horizontalMaxAngleDifference = 90;
	public float verticalMaxAngleDifference = 90;
	public float maxBendingAngle = 90;
	public float horizontalResponsiveness = 10;
	public float verticalResponsiveness = 10;

	public float upperBodyRotationSpeed = 5;
	public float rotationSpeed = 5;

	public Vector3 chestUpVector = new Vector3 (0, 0, 1);

	public Transform objectToFollow;
	public Transform weaponRotationPointToFollow;
	public playerController playerControllerManager;

	bool usingDualWeapon;

	bool IKUpperBodyEnabled;

	Transform currentWeaponRotationPoint;

	float currentRotation;
	float rotationXValue;

	bool usingWeaponRotationPoint;

	weaponRotationPointInfo rotationPointInfo;

	int currentRotationDirection = 1;

	Quaternion chestRotation;
	Quaternion spineRotation;

	float angleH;
	float angleV;
	Vector3 dirUp;
	Vector3 referenceLookDir;
	Vector3 referenceUpDir;
	float originalAngleDifference;
	Coroutine changeExtraRotation;

	Vector3 extraRotation;
	float currentExtraRotation;
	float auxCurrentExtraRotation;

	Coroutine shakeUpperBodyRotationCoroutine;

	Quaternion parentRot;
	Quaternion parentRotInv;
	Vector3 lookDirWorld;
	Vector3 lookDirGoal;
	float hAngle;
	Vector3 rightOfTarget;
	Vector3 lookDirGoalinHPlane;
	float vAngle;
	float hAngleThr;
	float vAngleThr;
	Vector3 referenceRightDir;
	Vector3 upDirGoal;
	Vector3 lookDir;
	Quaternion lookRot;
	Quaternion dividedRotation;

	Quaternion finalLookRotation;

	bool isPlayerDead;

	Transform currentRightWeaponRotationPoint;
	Transform currentLeftWeaponRotationPoint;
	weaponRotationPointInfo rightRotationPointInfo;
	weaponRotationPointInfo leftRotationPointInfo;
	int currentRightRotationDirection = 1;
	int currentLeftRotationDirection = 1;

	void Start ()
	{
		parentRot = spineTransform.parent.rotation;
		parentRotInv = Quaternion.Inverse (parentRot);

		referenceLookDir = parentRotInv * transform.rotation * headLookVector.normalized;
		referenceUpDir = parentRotInv * transform.rotation * headUpVector.normalized;
		dirUp = referenceUpDir;
		originalAngleDifference = maxBendingAngle;
	}

	void Update ()
	{
		if (IKUpperBodyEnabled) {

			isPlayerDead = playerControllerManager.isPlayerDead ();

			parentRot = spineTransform.parent.rotation;
			parentRotInv = Quaternion.Inverse (parentRot);

			// Desired look direction in world space
			lookDirWorld = (objectToFollow.position - chestTransform.position).normalized;

			// Desired look directions in neck parent space
			lookDirGoal = (parentRotInv * lookDirWorld);

			// Get the horizontal and vertical rotation angle to look at the target
			hAngle = AngleAroundAxis (referenceLookDir, lookDirGoal, referenceUpDir);
			rightOfTarget = Vector3.Cross (referenceUpDir, lookDirGoal);
			lookDirGoalinHPlane = lookDirGoal - Vector3.Project (lookDirGoal, referenceUpDir);
			vAngle = AngleAroundAxis (lookDirGoalinHPlane, lookDirGoal, rightOfTarget);

			// Handle threshold angle difference, bending multiplier, and max angle difference here
			hAngleThr = Mathf.Max (0, Mathf.Abs (hAngle) - horizontalThresholdAngleDifference) * Mathf.Sign (hAngle);			
			vAngleThr = Mathf.Max (0, Mathf.Abs (vAngle) - verticalThresholdAngleDifference) * Mathf.Sign (vAngle);

			hAngle = Mathf.Max (Mathf.Abs (hAngleThr) * Mathf.Abs (horizontalBendingMultiplier), Mathf.Abs (hAngle)
			- horizontalMaxAngleDifference) * Mathf.Sign (hAngle) * Mathf.Sign (horizontalBendingMultiplier);
			
			vAngle = Mathf.Max (Mathf.Abs (vAngleThr) * Mathf.Abs (verticalBendingMultiplier), Mathf.Abs (vAngle)
			- verticalMaxAngleDifference) * Mathf.Sign (vAngle) * Mathf.Sign (verticalBendingMultiplier);

			// Handle max bending angle here
			hAngle = Mathf.Clamp (hAngle, -maxBendingAngle, maxBendingAngle);
			vAngle = Mathf.Clamp (vAngle, -maxBendingAngle, maxBendingAngle);

			referenceRightDir = Vector3.Cross (referenceUpDir, referenceLookDir);

			// Lerp angles
			angleH = Mathf.Lerp (angleH, hAngle, Time.deltaTime * horizontalResponsiveness);
			angleV = Mathf.Lerp (angleV, vAngle, Time.deltaTime * verticalResponsiveness);

			// Get direction
			lookDirGoal = Quaternion.AngleAxis (angleH, referenceUpDir) * Quaternion.AngleAxis (angleV, referenceRightDir) * referenceLookDir;

			// Make look and up perpendicular
			upDirGoal = referenceUpDir;
			Vector3.OrthoNormalize (ref lookDirGoal, ref upDirGoal);

			// Interpolated look and up directions in neck parent space
			lookDir = lookDirGoal;
			dirUp = Vector3.Slerp (dirUp, upDirGoal, Time.deltaTime * 5);
			Vector3.OrthoNormalize (ref lookDir, ref dirUp);

			// Look rotation in world space
			lookRot = ((parentRot * Quaternion.LookRotation (lookDir, dirUp)) * Quaternion.Inverse (parentRot * Quaternion.LookRotation (referenceLookDir, referenceUpDir)));

			if (upperBodyRotationSpeed > 0) {
				finalLookRotation = Quaternion.Slerp (finalLookRotation, lookRot, Time.deltaTime * upperBodyRotationSpeed);
			} else {
				finalLookRotation = lookRot;
			}

			// Distribute rotation over all joints in segment
			dividedRotation = Quaternion.Slerp (Quaternion.identity, finalLookRotation, 0.5f);

			chestRotation = dividedRotation;
			spineRotation = dividedRotation;
		}

		if (usingWeaponRotationPoint) {
			if (IKUpperBodyEnabled && weaponRotationPointToFollow) {
				currentRotation = weaponRotationPointToFollow.localEulerAngles.x;

				if (usingDualWeapon) {
					if (currentRightWeaponRotationPoint) {

						if (currentRotation > 180) {
							rotationXValue = (360 - currentRotation) / rightRotationPointInfo.rotationUpPointAmountMultiplier;

							if (rightRotationPointInfo.useRotationUpClamp) {
								rotationXValue = Mathf.Clamp (rotationXValue, 0, rightRotationPointInfo.rotationUpClampAmount);
							}

							rotationXValue *= (-1) * currentRightRotationDirection;

						} else {
							rotationXValue = currentRotation / rightRotationPointInfo.rotationDownPointAmountMultiplier;

							if (rightRotationPointInfo.useRotationDownClamp) {
								rotationXValue = Mathf.Clamp (rotationXValue, 0, rightRotationPointInfo.rotationDownClamp);
							}

							rotationXValue *= currentRightRotationDirection;
						}

						Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));

						currentRightWeaponRotationPoint.localRotation = 
							Quaternion.Lerp (currentRightWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * rightRotationPointInfo.rotationPointSpeed);
					}

					if (currentLeftWeaponRotationPoint) {
						if (currentRotation > 180) {
							rotationXValue = (360 - currentRotation) / leftRotationPointInfo.rotationUpPointAmountMultiplier;

							if (leftRotationPointInfo.useRotationUpClamp) {
								rotationXValue = Mathf.Clamp (rotationXValue, 0, leftRotationPointInfo.rotationUpClampAmount);
							}

							rotationXValue *= (-1) * currentLeftRotationDirection;

						} else {
							rotationXValue = currentRotation / leftRotationPointInfo.rotationDownPointAmountMultiplier;

							if (leftRotationPointInfo.useRotationDownClamp) {
								rotationXValue = Mathf.Clamp (rotationXValue, 0, leftRotationPointInfo.rotationDownClamp);
							}

							rotationXValue *= currentLeftRotationDirection;
						}

						Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));

						currentLeftWeaponRotationPoint.localRotation = 
							Quaternion.Lerp (currentLeftWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * leftRotationPointInfo.rotationPointSpeed);
					}
				} else {
					if (currentRotation > 180) {
						rotationXValue = (360 - currentRotation) / rotationPointInfo.rotationUpPointAmountMultiplier;

						if (rotationPointInfo.useRotationUpClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, rotationPointInfo.rotationUpClampAmount);
						}

						rotationXValue *= (-1) * currentRotationDirection;

					} else {
						rotationXValue = currentRotation / rotationPointInfo.rotationDownPointAmountMultiplier;

						if (rotationPointInfo.useRotationDownClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, rotationPointInfo.rotationDownClamp);
						}

						rotationXValue *= currentRotationDirection;
					}

					Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));
					currentWeaponRotationPoint.localRotation = 
						Quaternion.Lerp (currentWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * rotationPointInfo.rotationPointSpeed);
				}
			} else {
				if (usingDualWeapon) {

					bool rightWeaponIsReset = false;
					bool leftWeaponIsReset = false;
					if (currentRightWeaponRotationPoint) {
						if (currentRightWeaponRotationPoint.localRotation != Quaternion.identity) {
							currentRightWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentRightWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * rightRotationPointInfo.rotationPointSpeed);
						} else {
							rightWeaponIsReset = true;
						}
					} else {
						rightWeaponIsReset = true;
					}

					if (currentLeftWeaponRotationPoint) {
						if (currentLeftWeaponRotationPoint.localRotation != Quaternion.identity) {
							currentLeftWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentLeftWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * leftRotationPointInfo.rotationPointSpeed);
						} else {
							leftWeaponIsReset = true;
						}
					} else {
						leftWeaponIsReset = true;
					}

					if (rightWeaponIsReset && leftWeaponIsReset) {
						usingWeaponRotationPoint = false;
					}
				} else {
					if (currentWeaponRotationPoint) {
						if (currentWeaponRotationPoint.localRotation != Quaternion.identity) {

							currentWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * rotationPointInfo.rotationPointSpeed);
						} else {
							usingWeaponRotationPoint = false;
						}
					}
				}
			}
		}
	}

	void LateUpdate ()
	{
		if (IKUpperBodyEnabled) {

			if (!isPlayerDead) {
				chestTransform.rotation = chestRotation * chestTransform.rotation;
				spineTransform.rotation = spineRotation * spineTransform.rotation;

				extraRotation = chestUpVector * currentExtraRotation;

				chestTransform.localEulerAngles = new Vector3 (chestTransform.localEulerAngles.x, chestTransform.localEulerAngles.y, chestTransform.localEulerAngles.z) + extraRotation;
			}
		}	
	}

	// The angle between dirA and dirB around axis
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		// Project A and B onto the plane orthogonal target axis
		dirA = dirA - Vector3.Project (dirA, axis);
		dirB = dirB - Vector3.Project (dirB, axis);

		// Find (positive) angle between A and B
		float angle = Vector3.Angle (dirA, dirB);
		return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}

	public void setCurrentBodyRotation (float bodyRotation)
	{
		currentExtraRotation = bodyRotation;
		auxCurrentExtraRotation = currentExtraRotation;
	}

	public void enableOrDisableIKUpperBody (bool value)
	{
		if (value) {
			IKUpperBodyEnabled = value;
		}
		checkSetExtraRotationCoroutine (value);
	}

	void checkSetExtraRotationCoroutine (bool state)
	{
		if (changeExtraRotation != null) {
			StopCoroutine (changeExtraRotation);
		}
		changeExtraRotation = StartCoroutine (setExtraRotation (state));
	}

	IEnumerator setExtraRotation (bool state)
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotationSpeed;
			if (state) {
				maxBendingAngle = Mathf.Lerp (maxBendingAngle, originalAngleDifference, t);
			} else {
				maxBendingAngle = Mathf.Lerp (maxBendingAngle, 0, t);
			}
			yield return null;
		}
		if (!state) {
			IKUpperBodyEnabled = false;
		}
	}

	public bool isIKUpperBodyEnabled ()
	{
		return IKUpperBodyEnabled;
	}

	public void checkShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue)
	{
		if (!shakeUpperBodyEnabled || !IKUpperBodyEnabled) {
			return;
		}

		stopShakeUpperBodyRotation ();
		shakeUpperBodyRotationCoroutine = StartCoroutine (shakeUpperBodyRotation (extraAngleValue, speedValue));
	}

	public void stopShakeUpperBodyRotation ()
	{
		if (shakeUpperBodyRotationCoroutine != null) {
			StopCoroutine (shakeUpperBodyRotationCoroutine);
		}
	}

	IEnumerator shakeUpperBodyRotation (float extraAngleValue, float speedValue)
	{
		float angleTarget = auxCurrentExtraRotation + extraAngleValue;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * speedValue;
			currentExtraRotation = Mathf.Lerp (currentExtraRotation, angleTarget, t);
			yield return null;
		}

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * speedValue;
			currentExtraRotation = Mathf.Lerp (currentExtraRotation, auxCurrentExtraRotation, t);
			yield return null;
		}
	}

	public void setCurrentWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentWeaponRotationPoint = newWeaponRotationPoint;

		rotationPointInfo = newRotationPointInfo;

		currentRotationDirection = newRotationDirection;
	}

	public void setUsingWeaponRotationPointState (bool state)
	{
		usingWeaponRotationPoint = state;
	}

	public void setUsingDualWeaponState (bool state)
	{
		usingDualWeapon = state;
	}

	public void setCurrentRightWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentRightWeaponRotationPoint = newWeaponRotationPoint;

		rightRotationPointInfo = newRotationPointInfo;

		currentRightRotationDirection = newRotationDirection;
	}

	public void setCurrentLeftWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentLeftWeaponRotationPoint = newWeaponRotationPoint;

		leftRotationPointInfo = newRotationPointInfo;

		currentLeftRotationDirection = newRotationDirection;
	}

	public void setNewChestUpVectorValue (Vector3 newValue)
	{
		chestUpVector = newValue;

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
}