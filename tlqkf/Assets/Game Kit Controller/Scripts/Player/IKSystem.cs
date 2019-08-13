using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IKSystem : MonoBehaviour
{
	public aimMode currentAimMode;
	public Transform rightHandIKPos;
	public Transform leftHandIKPos;
	public Transform currentHandIKPos;

	public Transform rightElbowIKPos;
	public Transform leftElbowIKPos;
	public Transform currentElbowIKPos;
	public Transform IKBodyCOM;

	public LayerMask layer;
	[Range (1, 10)] public float IKSpeed;

	public List<IKBodyInfoState> IKBodyInfoStateList = new List<IKBodyInfoState> ();

	public bool usingHands;

	public bool usingWeapons;
	public bool usingDualWeapon;
	public bool disablingDualWeapons;

	public bool usingArms;
	public bool driving;
	public bool usingZipline;
	public bool usingJetpack;
	public bool usingFlyingMode;

	public bool disableWeapons;

	List<IKBodyInfo> currentIKBodyInfoList = new List<IKBodyInfo> ();

	IKBodyInfoState currentIKBodyInfoState;

	public bool IKBodyInfoActive;

	public bool disablingIKBodyInfoActive;

	IKBodyInfo currentIKBodyInfo;
	Vector2 playerInputAxis;

	public bool objectGrabbed;
	public bool IKSystemEnabledToGrab;

	public Animator animator;
	public playerController playerControllerManager;
	public playerWeaponsManager weaponsManager;
	public otherPowers powersManager;

	public bool showGizmo;
	public IKSettings settings;

	public enum aimMode
	{
		hands,
		weapons
	}

	IKDrivingSystem.IKDrivingInformation IKDrivingSettings;
	zipline.IKZiplineInfo IKZiplineSettings;
	jetpackSystem.IKJetpackInfo IKJetpackSettings;
	flySystem.IKFlyInfo IKFlyingModeSettings;

	IKWeaponInfo IKWeaponsSettings;

	IKWeaponInfo IKWeaponsRightHandSettings;
	IKWeaponInfo IKWeaponsLeftHandSettings;

	AvatarIKGoal currentHand;

	float IKWeight;
	float IKWeightTargetValue;
	float originalDist;
	float hitDist;
	float currentDist;
	AvatarIKHint currenElbow;

	Ray ray;
	RaycastHit hit;

	Coroutine powerHandRecoil;
	float originalCurrentDist;
	int handsDisabled;
	float currentHeadWeight;
	float headWeightTarget;
	Transform playerHead;

	bool isThirdPersonView;


	List<grabObjects.handInfo> handInfoList = new List<grabObjects.handInfo> ();
	grabObjects.handInfo currentHandInfo;

	float currentWeightLerpSpeed;

	bool resetIKCOMRotationChecked;

	Vector3 targetRotation;

	bool headLookStateEnabled;
	bool disableHeadLookState;
	float headLookSpeed;
	Transform headLookTarget;

	Vector3 drivingBodyRotation;

	IKWeaponsPosition currentIKWeaponsPosition;

	void Start ()
	{
		for (int i = 0; i < IKBodyInfoStateList.Count; i++) {
			for (int j = 0; j < IKBodyInfoStateList [i].IKBodyInfoList.Count; j++) {
				IKBodyInfoStateList [i].IKBodyInfoList [j].originalPosition = IKBodyInfoStateList [i].IKBodyInfoList [j].targetToFollow.localPosition;
			}
		}
	}

	void Update ()
	{
		isThirdPersonView = !playerControllerManager.isPlayerOnFirstPerson ();

		if (usingWeapons || usingArms || driving || usingZipline || usingJetpack || usingFlyingMode || objectGrabbed) {
			usingHands = true;
		} else {
			usingHands = false;
		}

		if (!driving && usingArms) {
			//change the current weight of the ik 
			if (IKWeight != IKWeightTargetValue) {
				IKWeight = Mathf.MoveTowards (IKWeight, IKWeightTargetValue, Time.deltaTime * IKSpeed);
			}

			if (IKWeight > 0) {
				//if the raycast detects a surface, get the distance to it
				if (Physics.Raycast (currentHandIKPos.position, transform.forward, out hit, 3, layer)) {
					if (!hit.collider.isTrigger) {
						if (hit.distance < originalDist) {
							hitDist = hit.distance;
						} else {
							hitDist = originalDist;
						}
					}
				}
				//else, set the original distance
				else {
					hitDist = originalDist;
				}
				hitDist = Mathf.Clamp (hitDist, 0.1f, originalDist);
				//set the correct position of the current hand to avoid cross any collider with it
				currentDist = Mathf.Lerp (currentDist, hitDist, Time.deltaTime * IKSpeed);
				currentHandIKPos.localPosition = new Vector3 (currentHandIKPos.localPosition.x, currentDist, currentHandIKPos.localPosition.z);
			}

			if (IKWeight == 0) {
				usingArms = false;
			}
		}

		if (usingWeapons) {
			if (disableWeapons || disablingDualWeapons) {
				handsDisabled = 0;
			}

			if (usingDualWeapon) {
				if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue) {
						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disablingDualWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}

				if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue) {
						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disablingDualWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}
			} else {
				for (int j = 0; j < IKWeaponsSettings.handsInfo.Count; j++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [j];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue &&
					    (!currentIKWeaponsPosition.ignoreIKWeight || currentIKWeaponsPosition.targetValue == 0)) {

						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue &&
					    (!currentIKWeaponsPosition.ignoreIKWeight || currentIKWeaponsPosition.elbowInfo.targetValue == 0)) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disableWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}
			}

			if (disableWeapons || disablingDualWeapons) {					
				if (handsDisabled == 2) {
					disablingDualWeapons = false;
					disableWeapons = false;
					handsDisabled = 0;

					usingDualWeapon = false;

					setUsingWeaponsState (false);
					checkHandsInPosition (false);
				}
			}
		}

		if (usingZipline || usingJetpack || usingFlyingMode) {
			if (IKWeight != IKWeightTargetValue) {
				IKWeight = Mathf.MoveTowards (IKWeight, IKWeightTargetValue, Time.deltaTime * IKSpeed);
			}

			if (IKWeight == 0) {
				usingJetpack = false;
				usingFlyingMode = false;
				IKJetpackSettings = null;
				IKFlyingModeSettings = null;
			}
		}

		if (IKBodyInfoActive && !isThirdPersonView && resetIKCOMRotationChecked) {
			IKBodyCOM.localRotation = Quaternion.identity;
			resetIKCOMRotationChecked = false;
		}
	}

	void LateUpdate ()
	{
		if (driving) {
			if (IKDrivingSettings.useHeadLookDirection) {
				if (IKDrivingSettings.headLookDirection) {
					if (playerHead == null) {
						playerHead = animator.GetBoneTransform (HumanBodyBones.Head);
					} else {
						Quaternion headRotation = Quaternion.FromToRotation (playerHead.transform.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward), 
							                          IKDrivingSettings.headLookDirection.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward));
						Vector3 headDirection = headRotation.eulerAngles;
//				Quaternion headRotationY = Quaternion.FromToRotation (playerHead.transform.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward), 
						//IKDrivingSettings.headLookDirection.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward));
//				Vector3 headDirectionY = headRotationY.eulerAngles;
						playerHead.transform.localEulerAngles = -headDirection;
					}
				}
			}
		}
	}

	void OnAnimatorIK ()
	{
		if (!driving && !usingWeapons && IKWeight > 0 && usingArms) {
			//set the current hand target position and rotation
			animator.SetIKPositionWeight (currentHand, IKWeight);
			animator.SetIKRotationWeight (currentHand, IKWeight);  
			animator.SetIKPosition (currentHand, currentHandIKPos.position);
			animator.SetIKRotation (currentHand, currentHandIKPos.rotation);     
			animator.SetIKHintPositionWeight (currenElbow, IKWeight);
			animator.SetIKHintPosition (currenElbow, currentElbowIKPos.position);
		}

		//if the player is driving, set all the position and rotations of every player's limb
		if (driving) {
			for (int i = 0; i < IKDrivingSettings.IKDrivingPos.Count; i++) {
				//hands and foots
				animator.SetIKPositionWeight (IKDrivingSettings.IKDrivingPos [i].limb, 1);
				animator.SetIKRotationWeight (IKDrivingSettings.IKDrivingPos [i].limb, 1);  
				animator.SetIKPosition (IKDrivingSettings.IKDrivingPos [i].limb, IKDrivingSettings.IKDrivingPos [i].position.position);
				animator.SetIKRotation (IKDrivingSettings.IKDrivingPos [i].limb, IKDrivingSettings.IKDrivingPos [i].position.rotation);   
			}

			//knees and elbows
			for (int i = 0; i < IKDrivingSettings.IKDrivingKneePos.Count; i++) {
				animator.SetIKHintPositionWeight (IKDrivingSettings.IKDrivingKneePos [i].knee, 1);
				animator.SetIKHintPosition (IKDrivingSettings.IKDrivingKneePos [i].knee, IKDrivingSettings.IKDrivingKneePos [i].position.position);
			}

			//comment/discomment these two lines to edit correctly the body position of the player ingame.
			transform.position = IKDrivingSettings.vehicleSeatInfo.seatTransform.position;
			transform.rotation = IKDrivingSettings.vehicleSeatInfo.seatTransform.rotation;
			//set the rotation of the upper body of the player according to the steering direction
			if (IKDrivingSettings.useSteerDirection) {
				if (IKDrivingSettings.steerDirecion) {
					Vector3 lookDirection = IKDrivingSettings.steerDirecion.transform.forward + IKDrivingSettings.steerDirecion.transform.position;
					animator.SetLookAtPosition (lookDirection);
					animator.SetLookAtWeight (settings.weight, settings.bodyWeight, settings.headWeight, settings.eyesWeight, settings.clampWeight);
				}
			}

			if (IKDrivingSettings.shakePlayerBodyOnCollision) {
				IKDrivingSettings.currentSeatShake = Vector3.Lerp (IKDrivingSettings.currentSeatShake, Vector3.zero, Time.deltaTime * IKDrivingSettings.shakeFadeSpeed);

				drivingBodyRotation = IKDrivingSettings.currentAngularDirection;

				drivingBodyRotation = Vector3.Scale (drivingBodyRotation, IKDrivingSettings.forceDirection);

				drivingBodyRotation.x += ((Mathf.Cos (Time.time * IKDrivingSettings.shakeSpeed)) / 2) * IKDrivingSettings.currentSeatShake.x;
				drivingBodyRotation.y -= (Mathf.Cos (Time.time * IKDrivingSettings.shakeSpeed)) * IKDrivingSettings.currentSeatShake.y;
				drivingBodyRotation.z -= ((Mathf.Sin (Time.time * IKDrivingSettings.shakeSpeed)) / 2) * IKDrivingSettings.currentSeatShake.z;

				drivingBodyRotation.x = Mathf.Clamp (drivingBodyRotation.x, IKDrivingSettings.forceDirectionMinClamp.z, IKDrivingSettings.forceDirectionMaxClamp.z);
				drivingBodyRotation.y = Mathf.Clamp (drivingBodyRotation.y, IKDrivingSettings.forceDirectionMinClamp.y, IKDrivingSettings.forceDirectionMaxClamp.y);
				drivingBodyRotation.z = Mathf.Clamp (drivingBodyRotation.z, IKDrivingSettings.forceDirectionMinClamp.x, IKDrivingSettings.forceDirectionMaxClamp.x);

				IKDrivingSettings.currentSeatUpRotation = drivingBodyRotation;

				IKDrivingSettings.playerBodyParent.localRotation =
					Quaternion.Lerp (IKDrivingSettings.playerBodyParent.localRotation, Quaternion.Euler (IKDrivingSettings.currentSeatUpRotation), Time.deltaTime * IKDrivingSettings.stabilitySpeed);
			}

			if (IKDrivingSettings.useHeadLookPosition) {
				if (IKDrivingSettings.headLookPosition) {
					animator.SetLookAtWeight (1, 0, 1);
					animator.SetLookAtPosition (IKDrivingSettings.headLookPosition.position);
				}
			}
		}

		if (!driving && usingWeapons) {
			if (usingDualWeapon) {
				if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

					animator.SetIKPositionWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);
					animator.SetIKRotationWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);

					if (currentIKWeaponsPosition.transformFollowByHand) {
						animator.SetIKPosition (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.position);
						animator.SetIKRotation (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.rotation); 
					}

					animator.SetIKHintPositionWeight (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);
					animator.SetIKHintPosition (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.position.position);
				}

				if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

					animator.SetIKPositionWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);
					animator.SetIKRotationWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);

					if (currentIKWeaponsPosition.transformFollowByHand) {
						animator.SetIKPosition (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.position);
						animator.SetIKRotation (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.rotation); 
					}

					animator.SetIKHintPositionWeight (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);
					animator.SetIKHintPosition (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.position.position);
				}
			} else {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					if (currentIKWeaponsPosition.handUsedInWeapon) {
						animator.SetIKPositionWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);
						animator.SetIKRotationWeight (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.HandIKWeight);  

						if (currentIKWeaponsPosition.transformFollowByHand) {
							animator.SetIKPosition (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.position);
							animator.SetIKRotation (currentIKWeaponsPosition.limb, currentIKWeaponsPosition.transformFollowByHand.rotation); 
						}

						animator.SetIKHintPositionWeight (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);
						animator.SetIKHintPosition (currentIKWeaponsPosition.elbowInfo.elbow, currentIKWeaponsPosition.elbowInfo.position.position);
					}
				}
			}

			if (weaponsManager.currentWeaponUsesHeadLookWhenAiming ()) {
				if (weaponsManager.aimingInThirdPerson) {
					headWeightTarget = 1;
				} else {
					headWeightTarget = 0;
				}

				if (currentHeadWeight != headWeightTarget) {
					currentHeadWeight = Mathf.MoveTowards (currentHeadWeight, headWeightTarget, Time.deltaTime * weaponsManager.getCurrentWeaponHeadLookSpeed ());
				}

				animator.SetLookAtWeight (1, 0, currentHeadWeight);
				animator.SetLookAtPosition (weaponsManager.getCurrentHeadLookTargetPosition ());
			}
		}

		if (!driving) {
			if (headLookStateEnabled) {

				if (disableHeadLookState) {
					if (headWeightTarget == 0 && currentHeadWeight == 0) {
						headLookStateEnabled = false;
					}
				} else {
					if (powersManager.aimingInThirdPerson) {
						headWeightTarget = 1;
					} else {
						headWeightTarget = 0;
					}
				}

				if (currentHeadWeight != headWeightTarget) {
					currentHeadWeight = Mathf.MoveTowards (currentHeadWeight, headWeightTarget, Time.deltaTime * headLookSpeed);
				}

				animator.SetLookAtWeight (1, 0, currentHeadWeight);
				animator.SetLookAtPosition (headLookTarget.position);
			}
		}

		if (usingZipline) {
			for (int i = 0; i < IKZiplineSettings.IKGoals.Count; i++) {
				animator.SetIKPositionWeight (IKZiplineSettings.IKGoals [i].limb, IKWeight);
				animator.SetIKRotationWeight (IKZiplineSettings.IKGoals [i].limb, IKWeight);  
				animator.SetIKPosition (IKZiplineSettings.IKGoals [i].limb, IKZiplineSettings.IKGoals [i].position.position);
				animator.SetIKRotation (IKZiplineSettings.IKGoals [i].limb, IKZiplineSettings.IKGoals [i].position.rotation); 
			}

			for (int i = 0; i < IKZiplineSettings.IKHints.Count; i++) {
				animator.SetIKHintPositionWeight (IKZiplineSettings.IKHints [i].limb, IKWeight);
				animator.SetIKHintPosition (IKZiplineSettings.IKHints [i].limb, IKZiplineSettings.IKHints [i].position.position);
			}

			transform.position = IKZiplineSettings.bodyPosition.position;
			transform.rotation = IKZiplineSettings.bodyPosition.rotation;
		}

		if (usingJetpack) {
			for (int i = 0; i < IKJetpackSettings.IKGoals.Count; i++) {
				animator.SetIKPositionWeight (IKJetpackSettings.IKGoals [i].limb, IKWeight);
				animator.SetIKRotationWeight (IKJetpackSettings.IKGoals [i].limb, IKWeight);  
				animator.SetIKPosition (IKJetpackSettings.IKGoals [i].limb, IKJetpackSettings.IKGoals [i].position.position);
				animator.SetIKRotation (IKJetpackSettings.IKGoals [i].limb, IKJetpackSettings.IKGoals [i].position.rotation); 
			}

			for (int i = 0; i < IKJetpackSettings.IKHints.Count; i++) {
				animator.SetIKHintPositionWeight (IKJetpackSettings.IKHints [i].limb, IKWeight);
				animator.SetIKHintPosition (IKJetpackSettings.IKHints [i].limb, IKJetpackSettings.IKHints [i].position.position);
			}
		}

		if (usingFlyingMode) {
			for (int i = 0; i < IKFlyingModeSettings.IKGoals.Count; i++) {
				animator.SetIKPositionWeight (IKFlyingModeSettings.IKGoals [i].limb, IKWeight);
				animator.SetIKRotationWeight (IKFlyingModeSettings.IKGoals [i].limb, IKWeight);  
				animator.SetIKPosition (IKFlyingModeSettings.IKGoals [i].limb, IKFlyingModeSettings.IKGoals [i].position.position);
				animator.SetIKRotation (IKFlyingModeSettings.IKGoals [i].limb, IKFlyingModeSettings.IKGoals [i].position.rotation); 
			}

			for (int i = 0; i < IKFlyingModeSettings.IKHints.Count; i++) {
				animator.SetIKHintPositionWeight (IKFlyingModeSettings.IKHints [i].limb, IKWeight);
				animator.SetIKHintPosition (IKFlyingModeSettings.IKHints [i].limb, IKFlyingModeSettings.IKHints [i].position.position);
			}
		}

		if (isThirdPersonView && objectGrabbed) {

			if (IKSystemEnabledToGrab) {
				for (int j = 0; j < handInfoList.Count; j++) {

					currentHandInfo = handInfoList [j];

					if (currentHandInfo.useHand) {

						currentHandInfo.handPosition = currentHandInfo.handTransform.position;          
						currentHandInfo.handRotation = currentHandInfo.handTransform.rotation;

						currentHandInfo.currentHandWeight = 1;

						animator.SetIKRotationWeight (currentHandInfo.IKGoal, currentHandInfo.currentHandWeight);
						animator.SetIKPositionWeight (currentHandInfo.IKGoal, currentHandInfo.currentHandWeight);

						animator.SetIKPosition (currentHandInfo.IKGoal, currentHandInfo.handPosition);
						animator.SetIKRotation (currentHandInfo.IKGoal, currentHandInfo.handRotation);

						if (currentHandInfo.useElbow) {

							currentHandInfo.elbowPosition = currentHandInfo.elbowTransform.position;          

							animator.SetIKHintPositionWeight (currentHandInfo.IKHint, currentHandInfo.currentHandWeight);

							animator.SetIKHintPosition (currentHandInfo.IKHint, currentHandInfo.elbowPosition);
						}
					}
				}
			}
		} 

		if (isThirdPersonView && IKBodyInfoActive) {
			if (!playerControllerManager.isPlayerOnGround () || disablingIKBodyInfoActive) {
				resetIKCOMRotationChecked = true;

				playerInputAxis = playerControllerManager.getAxisValues ();

				int numberOfLimbsActive = 0;

				for (int j = 0; j < currentIKBodyInfoList.Count; j++) {

					currentIKBodyInfo = currentIKBodyInfoList [j];

					if (currentIKBodyInfo.IKBodyPartEnabled) {
			
						if (currentIKBodyInfo.isHand) {
							if (!usingHands) {
								if (currentIKBodyInfo.bodyPartBusy) {
									currentIKBodyInfo.bodyPartBusy = false;
									currentIKBodyInfo.IKBodyWeigthTarget = 1;
								}
							} else {
								if (!currentIKBodyInfo.bodyPartBusy) {
									currentIKBodyInfo.bodyPartBusy = true;
									currentIKBodyInfo.IKBodyWeigthTarget = 0;
								}
							}
						}

						if (currentIKBodyInfo.currentIKWeight != currentIKBodyInfo.IKBodyWeigthTarget) {

							if (currentIKBodyInfo.bodyPartBusy) {
								currentWeightLerpSpeed = currentIKBodyInfoState.busyBodyPartWeightLerpSpeed;
							} else {
								currentWeightLerpSpeed = currentIKBodyInfo.weightLerpSpeed;
							}

							currentIKBodyInfo.currentIKWeight = Mathf.MoveTowards (currentIKBodyInfo.currentIKWeight,
								currentIKBodyInfo.IKBodyWeigthTarget, Time.deltaTime * currentWeightLerpSpeed);
						} else {

							if (currentIKBodyInfo.currentIKWeight == 0 && currentIKBodyInfo.IKBodyWeigthTarget == 0) {
								numberOfLimbsActive++;
							}

							if (numberOfLimbsActive == currentIKBodyInfoList.Count && disablingIKBodyInfoActive) {
								IKBodyInfoActive = false;
								disablingIKBodyInfoActive = false;
								return;
							}
						}

						bool applyIKToBodyPart = false;

						if (!currentIKBodyInfo.bodyPartBusy) {
							if (playerInputAxis == Vector2.zero) {
								currentIKBodyInfo.currentLimbVerticalMovementSpeed = currentIKBodyInfo.limbVerticalMovementSpeed;
							} else {
								currentIKBodyInfo.currentLimbVerticalMovementSpeed = currentIKBodyInfo.slowLimbVerticalMovementSpeed;
							}

							if (currentIKBodyInfo.useSin) {
								currentIKBodyInfo.posTargetY = Mathf.Sin (Time.time * currentIKBodyInfo.currentLimbVerticalMovementSpeed) * currentIKBodyInfo.limbMovementAmount;
							} else {
								currentIKBodyInfo.posTargetY = Mathf.Cos (Time.time * currentIKBodyInfo.currentLimbVerticalMovementSpeed) * currentIKBodyInfo.limbMovementAmount;
							}

							if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
								currentIKBodyInfo.newPosition = new Vector3 (-playerInputAxis.x, currentIKBodyInfo.posTargetY, -playerInputAxis.y);
							} else {
								currentIKBodyInfo.newPosition = new Vector3 (0, currentIKBodyInfo.posTargetY - (playerInputAxis.y / 8), -Mathf.Abs (playerInputAxis.x));
							}

							currentIKBodyInfo.newPosition.x = Mathf.Clamp (currentIKBodyInfo.newPosition.x, currentIKBodyInfo.minClampPosition.x, currentIKBodyInfo.maxClampPosition.x);
							//currentIKBodyInfo.newPosition.y = Mathf.Clamp (currentIKBodyInfo.newPosition.y, currentIKBodyInfo.minClampPosition.y, currentIKBodyInfo.maxClampPosition.y);
							currentIKBodyInfo.newPosition.z = Mathf.Clamp (currentIKBodyInfo.newPosition.z, currentIKBodyInfo.minClampPosition.y, currentIKBodyInfo.maxClampPosition.y);

							currentIKBodyInfo.newPosition += currentIKBodyInfo.originalPosition;

							currentIKBodyInfo.targetToFollow.localPosition = 
					Vector3.Slerp (currentIKBodyInfo.targetToFollow.localPosition, currentIKBodyInfo.newPosition, Time.deltaTime * currentIKBodyInfo.limbsMovementSpeed);

							currentIKBodyInfo.IKGoalPosition = currentIKBodyInfo.targetToFollow.position;          
							currentIKBodyInfo.IKGoalRotation = currentIKBodyInfo.targetToFollow.rotation;
							applyIKToBodyPart = true;

						} else if (currentIKBodyInfo.currentIKWeight != currentIKBodyInfo.IKBodyWeigthTarget) {
							applyIKToBodyPart = true;
						}

						if (applyIKToBodyPart) {
							animator.SetIKRotationWeight (currentIKBodyInfo.IKGoal, currentIKBodyInfo.currentIKWeight);
							animator.SetIKPositionWeight (currentIKBodyInfo.IKGoal, currentIKBodyInfo.currentIKWeight);
							animator.SetIKPosition (currentIKBodyInfo.IKGoal, currentIKBodyInfo.IKGoalPosition);
							animator.SetIKRotation (currentIKBodyInfo.IKGoal, currentIKBodyInfo.IKGoalRotation);
						}
					}
				}

				if (!disablingIKBodyInfoActive) {
					if (weaponsManager.isUsingWeapons ()) {
						targetRotation = Vector3.zero;
					} else {

						if (currentIKBodyInfoState.increaseRotationAmountWhenHigherSpeed && playerControllerManager.isMovementSpeedIncreased ()) {
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountX = currentIKBodyInfoState.increasedIKBodyCOMRotationAmountX;
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountY = currentIKBodyInfoState.increasedIKBodyComRotationAmountY;

							currentIKBodyInfoState.currentIKBodyCOMRotationSpeed = currentIKBodyInfoState.increasedIKBodyCOMRotationSpeed;

						} else {
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountX = currentIKBodyInfoState.IKBodyCOMRotationAmountX;
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountY = currentIKBodyInfoState.IKBodyComRotationAmountY;

							currentIKBodyInfoState.currentIKBodyCOMRotationSpeed = currentIKBodyInfoState.IKBodyCOMRotationSpeed;
						}

						if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
							targetRotation = new Vector3 (playerInputAxis.y * currentIKBodyInfoState.currentIKBodyCOMRotationAmountY, 
								0, playerInputAxis.x * currentIKBodyInfoState.currentIKBodyCOMRotationAmountX);
						} else {
							targetRotation = new Vector3 (-(Math.Abs (playerInputAxis.x) - playerInputAxis.y) * currentIKBodyInfoState.currentIKBodyCOMRotationAmountX, 0, 0);
						}
					}
					
					IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.Euler (targetRotation), Time.deltaTime * currentIKBodyInfoState.currentIKBodyCOMRotationSpeed);
				}
			} else if (playerControllerManager.isPlayerOnGround ()) {
				IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.identity, Time.deltaTime * currentIKBodyInfoState.currentIKBodyCOMRotationSpeed);
			}
		}
	}

	public void setHeadLookState (bool state, float newHeadLookSpeed, Transform newHeadLookTarget)
	{
		if (state) {
			headLookStateEnabled = state;
			headWeightTarget = 1;
		} else {
			headWeightTarget = 0;
		}

		disableHeadLookState = !state;

		headLookSpeed = newHeadLookSpeed;
		headLookTarget = newHeadLookTarget;
	}


	public bool getUsingHands ()
	{
		return usingHands;
	}

	public float getHeadWeight ()
	{
		return currentHeadWeight;
	}

	//change the ik weight in the current arm
	public void changeArmState (float value)
	{
		if (currentAimMode == aimMode.weapons) {
			usingArms = false;
		} else {
			usingArms = true;
		}
			
		IKWeightTargetValue = value;

		if (usingZipline || usingJetpack || usingFlyingMode) {
			IKWeightTargetValue = 1;
		}
	}

	public void disableArmsState ()
	{
		usingArms = false;
		IKWeightTargetValue = 0;
		IKWeight = 0;
	}

	//change current arm to aim
	public void changeArmSide (bool value)
	{
		if (value) {
			//set the right arm as the current ik position
			currentHandIKPos.position = rightHandIKPos.position;
			currentHandIKPos.rotation = rightHandIKPos.rotation;
			currentHand = AvatarIKGoal.RightHand;

			currentElbowIKPos.position = rightElbowIKPos.position;
			currenElbow = AvatarIKHint.RightElbow;
		} else {
			//set the left arm as the current ik position
			currentHandIKPos.position = leftHandIKPos.position;
			currentHandIKPos.rotation = leftHandIKPos.rotation;
			currentHand = AvatarIKGoal.LeftHand;

			currentElbowIKPos.position = leftElbowIKPos.position;
			currenElbow = AvatarIKHint.LeftElbow;
		}

		originalDist = currentHandIKPos.localPosition.y;
		currentDist = originalDist;
		hitDist = originalDist;
	}

	//set if the player is driving or not, getting the current positions to every player's limb
	public void setDrivingState (bool state, IKDrivingSystem.IKDrivingInformation IKPositions)
	{
		driving = state;
		if (driving) {
			IKDrivingSettings = IKPositions;
		} else {
			IKDrivingSettings = null;
		}
	}

	public void setIKWeaponState (bool state, IKWeaponInfo IKPositions, bool useHandToDrawWeapon)
	{
		//move hands through the waypoints to grab the weapon
		IKWeaponsSettings = IKPositions;

		//the player is drawing a weapon
		if (state) {
			setUsingWeaponsState (state);

			if (IKPositions.deactivateIKIfNotAiming) {
				disableWeapons = false;
			}

			if (usingDualWeapon) {
				if (IKWeaponsSettings.usedOnRightHand) {
					currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
				}
					
				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

				currentIKWeaponsPosition.targetValue = 1;

				currentIKWeaponsPosition.handMovementCoroutine = 
					StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, false, IKPositions.deactivateIKIfNotAiming, 
					IKWeaponsSettings.usedOnRightHand, IKWeaponsSettings.useQuickDrawKeepWeapon));

			} else {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					if ((currentIKWeaponsPosition.usedToDrawWeapon && useHandToDrawWeapon) || (!currentIKWeaponsPosition.usedToDrawWeapon && !useHandToDrawWeapon)) {
						stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

						currentIKWeaponsPosition.targetValue = 1;

						currentIKWeaponsPosition.handMovementCoroutine = 
							StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, false, IKPositions.deactivateIKIfNotAiming,
							IKWeaponsSettings.usedOnRightHand, IKWeaponsSettings.useQuickDrawKeepWeapon));
					}
				}
			}
		} 

		//the player is keeping a weapon
		else {
			if (!usingDualWeapon) {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					if (!currentIKWeaponsPosition.usedToDrawWeapon) {
						stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

						currentIKWeaponsPosition.handMovementCoroutine = 
							StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, true, IKPositions.deactivateIKIfNotAiming, 
							IKWeaponsSettings.usedOnRightHand, IKWeaponsSettings.useQuickDrawKeepWeapon));
					} else {
						if (!currentIKWeaponsPosition.handInPositionToAim) {
							stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

							currentIKWeaponsPosition.targetValue = 0;
				
							disableWeapons = true;
						}
					}
				}

				IKWeaponsSettings.handsInPosition = false;
			}
		}
	}

	IEnumerator moveThroughWaypoints (IKWeaponsPosition IKWeapon, bool keepingWeapon, bool deactivateIKIfNotAiming, bool isUsedOnDualRightHand, bool useQuickDrawKeepWeapon)
	{
		if (IKWeapon.handUsedInWeapon) {
			Transform follower = IKWeapon.waypointFollower;
			List<Transform> wayPoints = new List<Transform> (IKWeapon.wayPoints);

			if (keepingWeapon) {
				wayPoints.Reverse ();
				wayPoints.RemoveAt (0);
			}

			follower.position = IKWeapon.handTransform.position;
			follower.rotation = IKWeapon.handTransform.rotation;
			IKWeapon.transformFollowByHand = follower;

			if (!useQuickDrawKeepWeapon || !IKWeapon.usedToDrawWeapon) {
				if (!deactivateIKIfNotAiming || IKWeapon.usedToDrawWeapon) {

					bool targetReached = false;

					float angleDifference = 0;

					float movementTimer = 0;

					foreach (Transform transformPath in wayPoints) {
						// find the distance to travel
						float dist = GKC_Utils.distance (follower.position, transformPath.position); 

						// calculate the movement duration
						float duration = dist / IKWeapon.handMovementSpeed; 
						float t = 0;

						targetReached = false;

						angleDifference = 0;

						movementTimer = 0;

						float handDistance = 0;

						while (!targetReached) {
							t += Time.deltaTime / duration;
							follower.position = Vector3.Lerp (follower.position, transformPath.position, t);
							follower.rotation = Quaternion.Slerp (follower.rotation, transformPath.rotation, t);

							angleDifference = Quaternion.Angle (follower.rotation, transformPath.rotation);

							movementTimer += Time.deltaTime;

							dist = GKC_Utils.distance (transformPath.position, IKWeapon.handTransform.position); 

							handDistance = GKC_Utils.distance (transformPath.position, IKWeapon.handTransform.position);

							if ((dist < 0.025f && angleDifference < 0.025f && handDistance < 0.025f) || movementTimer > (duration + 0.5f)) {
//								print ("reached");
								targetReached = true;
							} else {
//								print ("moving " + handDistance + " " + dist + " " + angleDifference + " " + movementTimer);
							}

							yield return null;
						}
					}
				}
			}

			if (keepingWeapon) {
				IKWeapon.handInPositionToAim = false;
				IKWeapon.targetValue = 0;

				disableWeapons = true;
				IKWeapon.elbowInfo.targetValue = 0;

			} else {
				IKWeapon.handInPositionToAim = true;
				IKWeapon.transformFollowByHand = IKWeapon.position;

				if (IKWeapon.usedToDrawWeapon) {
					if (usingDualWeapon) {
						if (useQuickDrawKeepWeapon) {
							weaponsManager.dualWeaponReadyToMoveDirectlyOnDrawHand (isUsedOnDualRightHand);
						} else {
							weaponsManager.dualWeaponReadyToMove (isUsedOnDualRightHand);
						}
					} else {
						if (useQuickDrawKeepWeapon) {
							weaponsManager.weaponReadyToMoveDirectlyOnDrawHand ();
						} else {
							weaponsManager.weaponReadyToMove ();
						}
					}
				} else {
					IKWeapon.elbowInfo.targetValue = 1;
				}

				if (deactivateIKIfNotAiming && !IKWeapon.usedToDrawWeapon) {
					IKWeapon.targetValue = 0;
					IKWeapon.elbowInfo.targetValue = 0;
				}

				checkHandsInPosition (true);
			}
		} else {
			if (keepingWeapon) {
				IKWeapon.handInPositionToAim = false;
			
				disableWeapons = true;
			} else {
				IKWeapon.handInPositionToAim = true;
				if (IKWeapon.usedToDrawWeapon) {
					
					if (usingDualWeapon) {
						weaponsManager.dualWeaponReadyToMove (isUsedOnDualRightHand);
					} else {
						weaponsManager.weaponReadyToMove ();
					}
				} 

				checkHandsInPosition (true);
			}
		}
	}

	public void checkHandsInPosition (bool drawingWeapon)
	{
		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {

				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];
				if (currentIKWeaponsPosition.handInPositionToAim == drawingWeapon) {
					IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInPosition = drawingWeapon;
				}
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {

				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];
				if (currentIKWeaponsPosition.handInPositionToAim == drawingWeapon) {
					IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInPosition = drawingWeapon;
				}
			}
		} else {
			int numberOfHandsInPosition = 0;

			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
				if (IKWeaponsSettings.handsInfo [i].handInPositionToAim == drawingWeapon) {
					numberOfHandsInPosition++;
				}
			}

			if (numberOfHandsInPosition == 2) {
				IKWeaponsSettings.handsInPosition = drawingWeapon;
			}
		}
	}

	public void stopIKWeaponsActions ()
	{
		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {
			if (IKWeaponsSettings != null) {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
				}
			}
		}

		disableWeapons = false;
		usingWeapons = false;

		usingDualWeapon = false;
		disablingDualWeapons = false;
	}

	public void quickDrawWeaponState (IKWeaponInfo IKPositions)
	{
		IKWeaponsSettings = IKPositions;

		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {
			if (IKWeaponsSettings != null) {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
				}
			}
		}

		setUsingWeaponsState (true);

		if (usingDualWeapon) {
			if (IKWeaponsSettings.usedOnRightHand) {
				currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
			} else {
				currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
			}

			float targetValue = 1;
			if (IKWeaponsSettings.deactivateIKIfNotAiming) {
				targetValue = 0;
			}

			currentIKWeaponsPosition.targetValue = targetValue;
			currentIKWeaponsPosition.HandIKWeight = targetValue;
			currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
			currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

			currentIKWeaponsPosition.handInPositionToAim = true;
			currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
		} else {
			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {

				currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

				float targetValue = 1;
				if (IKWeaponsSettings.deactivateIKIfNotAiming) {
					targetValue = 0;
				}

				if (IKWeaponsSettings.usingWeaponAsOneHandWield && !currentIKWeaponsPosition.usedToDrawWeapon) {
					targetValue = 0;
				}

				currentIKWeaponsPosition.targetValue = targetValue;
				currentIKWeaponsPosition.HandIKWeight = targetValue;
				currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
				currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

				currentIKWeaponsPosition.handInPositionToAim = true;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
			}
		}

		checkHandsInPosition (true);
	}

	public void quickDrawWeaponStateDualWeapon (IKWeaponInfo IKPositions, bool isRigthWeapon)
	{
		IKWeaponsSettings = IKPositions;

		if (isRigthWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		}

		setUsingWeaponsState (true);

		if (IKWeaponsSettings.usedOnRightHand) {
			currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
		} else {
			currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
		}

		float targetValue = 1;
		if (IKWeaponsSettings.deactivateIKIfNotAiming) {
			targetValue = 0;
		}

		currentIKWeaponsPosition.targetValue = targetValue;
		currentIKWeaponsPosition.HandIKWeight = targetValue;
		currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
		currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

		currentIKWeaponsPosition.handInPositionToAim = true;
		currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;

		checkHandsInPosition (true);
	}

	public void stopIKWeaponHandMovementCoroutine (IKWeaponsPosition IKWeaponPositionToCheck)
	{
		if (IKWeaponPositionToCheck.handMovementCoroutine != null) {
			StopCoroutine (IKWeaponPositionToCheck.handMovementCoroutine);
		}
	}

	public void quickKeepWeaponState ()
	{
		disableWeapons = true;

		disablingDualWeapons = true;
	}

	public void setDisableWeaponsState (bool state)
	{
		disableWeapons = state;

		disablingDualWeapons = state;
	}

	public void disableIKWeight ()
	{
		headWeightTarget = 0;
		currentHeadWeight = 0;
	}

	public void setUsingWeaponsState (bool state)
	{
		usingWeapons = state;
	}

	public void setUsingDualWeaponState (bool state)
	{
		if (state) {
			usingDualWeapon = state;
		} else {
			disablingDualWeapons = true;
		}
	}

	public void disableUsingDualWeaponState ()
	{
		usingDualWeapon = false;
		disablingDualWeapons = false;
	}

	public void setIKWeaponsRightHandSettings (IKWeaponInfo IKPositions)
	{
		IKWeaponsRightHandSettings = IKPositions;
	}

	public void setIKWeaponsLeftHandSettings (IKWeaponInfo IKPositions)
	{
		IKWeaponsLeftHandSettings = IKPositions;
	}

	public void startRecoil (float recoilSpeed, float recoilAmount)
	{
		if (powerHandRecoil != null) {
			StopCoroutine (powerHandRecoil);
		}

		powerHandRecoil = StartCoroutine (recoilMovementBack (recoilSpeed, recoilAmount));
	}

	IEnumerator recoilMovementBack (float recoilSpeed, float recoilAmount)
	{
		originalCurrentDist = currentDist;
		float newDist = currentDist - recoilAmount;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * recoilSpeed;
			currentDist = Mathf.Lerp (currentDist, newDist, t);
			yield return null;
		}

		powerHandRecoil = StartCoroutine (recoilMovementForward (recoilSpeed));
	}

	IEnumerator recoilMovementForward (float recoilSpeed)
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * recoilSpeed;
			currentDist = Mathf.Lerp (currentDist, originalCurrentDist, t);
			yield return null;
		}
	}

	public void setElbowsIKTargetValue (float leftValue, float rightValue)
	{
		for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
			currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
				currentIKWeaponsPosition.elbowInfo.targetValue = leftValue;
			}

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
				currentIKWeaponsPosition.elbowInfo.targetValue = rightValue;
			}
		}
	}

	public void ziplineState (bool state, zipline.IKZiplineInfo IKPositions)
	{
		usingZipline = state;

		if (usingZipline) {
			IKWeightTargetValue = 1;
			IKZiplineSettings = IKPositions;
		} else {
			IKWeightTargetValue = 0;
			IKZiplineSettings = null;
		}
	}

	public void jetpackState (bool state, jetpackSystem.IKJetpackInfo IKPositions)
	{
		if (state) {
			usingJetpack = true;
			IKWeightTargetValue = 1;
			IKJetpackSettings = IKPositions;
		} else {
			IKWeightTargetValue = 0;
		}
	}

	public void flyingModeState (bool state, flySystem.IKFlyInfo IKPositions)
	{
		if (state) {
			usingFlyingMode = true;
			IKWeightTargetValue = 1;
			IKFlyingModeSettings = IKPositions;
		} else {
			IKWeightTargetValue = 0;
		}
	}

	public void setGrabedObjectState (bool state, bool IKSystemEnabledToGrabState, List<grabObjects.handInfo> handInfo)
	{
		objectGrabbed = state;
		IKSystemEnabledToGrab = IKSystemEnabledToGrabState;

		if (objectGrabbed) {
			handInfoList = handInfo;
		}
	}

	public void setIKBodyState (bool state, string IKBodyStateName)
	{
		for (int j = 0; j < IKBodyInfoStateList.Count; j++) {
			if (IKBodyInfoStateList [j].Name == IKBodyStateName) {
				currentIKBodyInfoState = IKBodyInfoStateList [j];
				currentIKBodyInfoList = currentIKBodyInfoState.IKBodyInfoList;
			}
		}

		if (state) {
			for (int j = 0; j < currentIKBodyInfoList.Count; j++) {
				currentIKBodyInfoList [j].targetToFollow.position = animator.GetIKPosition (currentIKBodyInfoList [j].IKGoal);
			}
		}

		if (state) {
			IKBodyInfoActive = true;
			disablingIKBodyInfoActive = false;
		} else {
			disablingIKBodyInfoActive = true;
		}

		for (int j = 0; j < currentIKBodyInfoList.Count; j++) {
			if (state) {
				currentIKBodyInfoList [j].IKBodyWeigthTarget = 1;
				if (currentIKBodyInfoList [j].isHand) {
					if (usingHands) {
						if (!currentIKBodyInfoList [j].bodyPartBusy) {
							currentIKBodyInfoList [j].bodyPartBusy = true;
							currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;
						}
					}
				}
			} else {
				currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;
				if (currentIKBodyInfoList [j].isHand) {
					if (usingHands) {
						if (!currentIKBodyInfoList [j].bodyPartBusy) {
							currentIKBodyInfoList [j].bodyPartBusy = true;
							currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;
						}
					}
				}
			}
		}

		if (!state) {
			resetBodyRotation ();
		}
	}

	Coroutine bodyRotationCoroutine;

	public void resetBodyRotation ()
	{
		if (bodyRotationCoroutine != null) {
			StopCoroutine (bodyRotationCoroutine);
		}
		bodyRotationCoroutine = StartCoroutine (resetBodyRotationCoroutine ());
	}

	IEnumerator resetBodyRotationCoroutine ()
	{
		targetRotation = Vector3.zero;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * currentIKBodyInfoState.IKBodyCOMRotationSpeed;
			IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.Euler (targetRotation), t);
			yield return null;
		}
	}

	public Transform getIKBodyCOM ()
	{
		return IKBodyCOM;
	}

	public void setCurrentAimModeType (string newAimModeTypeName)
	{
		if (newAimModeTypeName == "Weapons") {
			currentAimMode = aimMode.weapons;
		}

		if (newAimModeTypeName == "Powers") {
			currentAimMode = aimMode.hands;
		}
	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<IKSystem> ());
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo && !Application.isPlaying) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube (leftHandIKPos.transform.position, Vector3.one / 10);
			Gizmos.DrawCube (rightHandIKPos.transform.position, Vector3.one / 10);
		}
	}

	[System.Serializable]
	public class IKSettings
	{
		public float weight;
		public float bodyWeight;
		public float headWeight;
		public float eyesWeight;
		public float clampWeight;
	}

	[System.Serializable]
	public class IKBodyInfoState
	{
		public string Name;
		public List<IKBodyInfo> IKBodyInfoList = new List<IKBodyInfo> ();
		public float IKBodyCOMRotationAmountX = 20;
		public float IKBodyComRotationAmountY = 20;
		public float IKBodyCOMRotationSpeed = 2;

		public bool increaseRotationAmountWhenHigherSpeed;
		public float increasedIKBodyCOMRotationAmountX = 20;
		public float increasedIKBodyComRotationAmountY = 20;
		public float increasedIKBodyCOMRotationSpeed = 2;

		public float busyBodyPartWeightLerpSpeed = 3;

		[HideInInspector] public float currentIKBodyCOMRotationAmountX;
		[HideInInspector] public float currentIKBodyCOMRotationAmountY;

		[HideInInspector] public float currentIKBodyCOMRotationSpeed = 2;
	}

	[System.Serializable]
	public class IKBodyInfo
	{
		public string Name;
		public bool IKBodyPartEnabled = true;

		public Transform targetToFollow;
		public AvatarIKGoal IKGoal;
		public bool isHand;

		public bool bodyPartBusy;
		public float currentIKWeight;
		public float weightLerpSpeed;
		public float IKBodyWeigthTarget;

		[HideInInspector] public Vector3 newPosition;

		public bool useSin = true;

		[HideInInspector] public Vector3 IKGoalPosition;
		[HideInInspector] public Quaternion IKGoalRotation;

		public float limbsMovementSpeed;
		public float limbMovementAmount;
		public float limbVerticalMovementSpeed;

		public float slowLimbVerticalMovementSpeed = 0.2f;

		[HideInInspector] public float currentLimbVerticalMovementSpeed;

		public Vector2 minClampPosition;
		public Vector2 maxClampPosition;
		[HideInInspector] public Vector3 originalPosition;

		[HideInInspector] public float posTargetY;
	}
}