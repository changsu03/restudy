using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class IKWeaponSystem : MonoBehaviour
{
	public IKWeaponInfo thirdPersonWeaponInfo;
	public IKWeaponInfo firstPersonWeaponInfo;

	public weaponSwayInfo firstPersonSwayInfo;
	public weaponSwayInfo runFirstPersonSwayInfo;
	public weaponSwayInfo thirdPersonSwayInfo;
	public weaponSwayInfo runThirdPersonSwayInfo;

	public weaponSwayInfo currentSwayInfo;

	public bool useShotShakeInFirstPerson;
	public bool useShotShakeInThirdPerson;
	public bool sameValueBothViews;
	public weaponShotShakeInfo thirdPersonshotShakeInfo;
	public weaponShotShakeInfo firstPersonshotShakeInfo;

	public weaponShotShakeInfo thirdPersonMeleeAttackShakeInfo;
	public weaponShotShakeInfo firstPersonMeleeAttackShakeInfo;

	public GameObject weaponPrefabModel;
	public GameObject inventoryWeaponPrefabObject;

	public GameObject weaponGameObject;
	public GameObject firstPersonArms;
	public bool headLookWhenAiming;
	public float headLookSpeed;
	public Transform headLookTarget;

	public bool canAimInFirstPerson;
	public bool currentWeapon;
	public bool aiming;
	public bool carrying;
	public float extraRotation;
	public float aimFovValue;
	public float aimFovSpeed;
	public bool weaponEnabled;

	public bool useWeaponIdle;
	public float timeToActiveWeaponIdle = 3;
	public bool playerMoving;
	public Vector3 idlePositionAmount;
	public Vector3 idleRotationAmount;
	public Vector3 idleSpeed;
	public bool idleActive;
	public bool showIdleSettings;

	public bool useLowerRotationSpeedAimed;
	public float verticalRotationSpeedAimedInFirstPerson = 4;
	public float horizontalRotationSpeedAimedInFirstPerson = 4;

	public bool useLowerRotationSpeedAimedThirdPerson;
	public float verticalRotationSpeedAimedInThirdPerson = 4;
	public float horizontalRotationSpeedAimedInThirdPerson = 4;

	public GameObject player;
	public bool moving;
	public playerWeaponSystem weaponSystemManager;
	public playerWeaponsManager weaponsManager;
	public Transform weaponTransform;

	public string relativePathInventory = "Assets/Game Kit Controller/Prefabs/Player Weapons/Weapons";
	public GameObject emtpyWeaponPrefab;

	public LayerMask layer;
	public bool surfaceDetected;

	public bool useShotCameraNoise;
	public Vector2 verticalShotCameraNoiseAmount;
	public Vector2 horizontalShotCameraNoiseAmount;

	public bool hideWeaponIfKeptInThirdPerson;

	public bool canBeDropped = true;

	public bool canUnlockCursor;

	public bool weaponInRunPosition;
	bool useRunPositionThirdPerson;
	bool useRunPositionFirstPerson;

	public bool playerRunning;
	bool useNewFovOnRunThirdPerson;
	bool useNewFovOnRunFirstPerson;

	public bool weaponConfiguredAsDualWepaon;
	public string linkedDualWeaponName;
	bool weaponConfiguredAsDualWeaponPreviously;

	bool cursorHidden;

	Coroutine weaponMovement;

	Vector3 weaponPositionTarget;
	Quaternion weaponRotationTarget;
	List<Transform> inverseKeepPath = new List<Transform> ();
	List<Transform> currentKeepPath = new List<Transform> ();

	Transform weaponSwayTransform;
	Vector3 swayPosition;
	Vector3 swayExtraPosition;
	Quaternion swayTargetRotation;
	Vector3 mainSwayTargetPosition;
	Vector3 currentSwayPosition;

	Vector3 swayRotation;
	Vector3 swayTilt;
	float swayPositionRunningMultiplier = 1;
	float swayRotationRunningMultiplier = 1;
	float bobPositionRunningMultiplier = 1;
	float bobRotationRunningMultiplier = 1;

	float currentSwayRotationSmooth;
	float currentSwayPositionSmooth;

	bool usingPositionSway;
	bool usingRotationSway;
	Coroutine swayValueCoroutine;
	bool resetingWeaponSwayValue;

	float lastTimeMoved = 0;
	RaycastHit hit;

	bool carryingWeapon;
	float currentRecoilSpeed;

	bool editingAttachments;

	bool weaponOnRecoil;
	weaponAttachmentSystem mainWeaponAttachmentSystem;

	public bool jumpingOnProcess;
	bool weaponInJumpStart;
	bool weaponInJumpEnd;

	bool canRunWhileCarrying;
	bool canRunWhileAiming;

	bool useNewCarrySpeed;
	float newCarrySpeed;
	bool useNewAimSpeed;
	float newAimSpeed;

	float timeToCheckSurfaceCollision = 0.4f;

	Transform currentAimPositionTransform;
	Transform currentDrawPositionTransform;
	Transform currentCollisionPositionTransform;
	Transform currentCollisionRayPositionTransform;
	Transform currentWalkOrRunPositionTransform;
	Transform currentJumpPositionTransform;
	Transform currentMeleeAttackPositionTransform;
	Transform currentEditAttachmentPositionTransform;
	Transform currentReloadPosition;

	bool currentlyUsingSway;

	bool meleeAtacking;

	Transform currentAimRecoilPositionTransform;

	Vector3 recoilExtraPosition;
	Vector3 recoilRandomPosition;
	Vector3 recoilExtraRotatation;
	Vector3 recoilRandomRotation;

	Coroutine handAttachmentCoroutine;

	//Gizmo variables
	public bool showThirdPersonGizmo;
	public bool showFirstPersonGizmo;
	public Color gizmoLabelColor;

	public bool showHandsWaypointGizmo = true;
	public bool showWeaponWaypointGizmo = true;
	public bool showPositionGizmo = true;

	public bool showThirdPersonSettings;
	public bool showFirstPersonSettings;
	public bool showShotShakeettings;
	public bool showSettings;
	public bool showElementSettings;

	public bool showSwaySettings;
	public bool showWalkSwaySettings;
	public bool showRunSwaySettings;

	public bool usingWeaponRotationPoint;
	public Transform currentHeadLookTarget;

	bool startInitialized;

	float timeToCheckRunPosition = 0.4f;

	public bool usingDualWeapon;

	public bool disablingDualWeapon;

	public bool usingRightHandDualWeapon;

	public bool playerOnJumpStart;
	public bool playerOnJumpEnd;

	IKWeaponsPosition currentIKWeaponsPosition;

	IKWeaponsPosition currentGizmoIKWeaponPosition;

	Transform mainWeaponMeshTransform;

	float currentCollisionRayDistance;

	public bool crouchingActive;

	bool useBezierCurve;
	BezierSpline spline;
	float bezierDuration;
	float lookDirectionSpeed;

	bool aimingWeaponInProcess;

	public bool reloadingWeapon;

	void Start ()
	{
		mainWeaponMeshTransform = weaponSystemManager.weaponSettings.weaponMesh.transform;

		setCurrentSwayInfo (true);

		weaponTransform = weaponGameObject.transform;
		thirdPersonWeaponInfo.weapon = weaponGameObject;
		firstPersonWeaponInfo.weapon = weaponGameObject;

		useRunPositionThirdPerson = thirdPersonWeaponInfo.useRunPosition;
		useRunPositionFirstPerson = firstPersonWeaponInfo.useRunPosition;

		useNewFovOnRunThirdPerson = thirdPersonWeaponInfo.useNewFovOnRun;
		useNewFovOnRunFirstPerson = firstPersonWeaponInfo.useNewFovOnRun;

		usingWeaponRotationPoint = thirdPersonWeaponInfo.useWeaponRotationPoint;
		if (usingWeaponRotationPoint) {
			currentHeadLookTarget = thirdPersonWeaponInfo.weaponRotationPointHeadLookTarget;
		} else {
			currentHeadLookTarget = headLookTarget;
		}
	}

	public void setInitialWeaponState ()
	{
		if (!weaponEnabled) {
			enableOrDisableWeaponMesh (false);
		}
		if (hideWeaponIfKeptInThirdPerson) {
			enableOrDisableWeaponMesh (false);
		}
	}

	void FixedUpdate ()
	{
		if (!startInitialized) {
			setInitialWeaponState ();
			startInitialized = true;
		}

		if (currentWeapon) {

			if (weaponsManager.weaponsAreMoving ()) {
				return;
			}

			//check collision on third person
			if (weaponSystemManager.aimingInThirdPerson && thirdPersonWeaponInfo.checkSurfaceCollision && !meleeAtacking && !aimingWeaponInProcess) {
				//if the raycast detects a surface, get the distance to it

				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentCollisionRayPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.surfaceCollisionRayPosition;

						currentCollisionRayDistance = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.collisionRayDistance;
					} else {
						currentCollisionRayPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.surfaceCollisionRayPosition;

						currentCollisionRayDistance = thirdPersonWeaponInfo.leftHandDualWeaponInfo.collisionRayDistance;
					}
				} else {
					currentCollisionRayPositionTransform = thirdPersonWeaponInfo.surfaceCollisionRayPosition;

					currentCollisionRayDistance = thirdPersonWeaponInfo.collisionRayDistance;
				}

				if (Physics.Raycast (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward, out hit, currentCollisionRayDistance, layer)) {
					if (!hit.collider.isTrigger) {
						Debug.DrawRay (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward * currentCollisionRayDistance, Color.red);
						if (!surfaceDetected) {
							walkOrSurfaceCollision (!surfaceDetected);
						}
					}
				} else {
					Debug.DrawRay (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward * currentCollisionRayDistance, Color.green);
					if (surfaceDetected) {
						walkOrSurfaceCollision (!surfaceDetected);
					}
				}
			}

			if (weaponSystemManager.carryingWeaponInThirdPerson && !moving && !aiming) {
				//manage the weapon position when the player is moving in third person
				if (!thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
					if (useRunPositionOnThirdPerson ()) {
						if (Time.time > weaponsManager.getLastTimeFired () + timeToCheckRunPosition && !meleeAtacking) {
							if (weaponsManager.isPlayerMoving ()) {
								if (!weaponInRunPosition) {
									walkOrRunWeaponPosition (true);
								}
							} else {
								if (weaponInRunPosition) {
									walkOrRunWeaponPosition (false);
								}
							}
						}
					}
				}
			}

			//check collision on first person
			if (weaponSystemManager.carryingWeaponInFirstPerson && !moving && !meleeAtacking && !aiming) {
				if (!weaponInRunPosition && firstPersonWeaponInfo.checkSurfaceCollision) {
					if (Time.time > weaponsManager.getLastTimeFired () + timeToCheckSurfaceCollision) {
						if (weaponsManager.isCarryWeaponInLowerPositionActive ()) {
							if (!surfaceDetected) {
								walkOrSurfaceCollision (!surfaceDetected);
							}
						} else {
							//if the raycast detects a surface, get the distance to it

							if (isUsingDualWeapon ()) {
								if (usingRightHandDualWeapon) {
									currentCollisionRayPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.surfaceCollisionRayPosition;

									currentCollisionRayDistance = firstPersonWeaponInfo.rightHandDualWeaopnInfo.collisionRayDistance;
								} else {
									currentCollisionRayPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.surfaceCollisionRayPosition;

									currentCollisionRayDistance = firstPersonWeaponInfo.leftHandDualWeaponInfo.collisionRayDistance;
								}
							} else {
								currentCollisionRayPositionTransform = firstPersonWeaponInfo.surfaceCollisionRayPosition;

								currentCollisionRayDistance = firstPersonWeaponInfo.collisionRayDistance;
							}

							if (Physics.Raycast (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward, out hit, currentCollisionRayDistance, layer)) {
								if (!hit.collider.isTrigger) {
									Debug.DrawRay (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward * currentCollisionRayDistance, Color.red);
									if (!surfaceDetected) {
										walkOrSurfaceCollision (!surfaceDetected);
									}
								}
							} else {
								Debug.DrawRay (currentCollisionRayPositionTransform.position, currentCollisionRayPositionTransform.forward * currentCollisionRayDistance, Color.green);
								if (surfaceDetected) {
									walkOrSurfaceCollision (!surfaceDetected);
								}
							}
						}
					}
				}

				if (!weaponsManager.weaponsAreMoving ()) {
					//manage the weapon position when the player is running in first person
					if (useRunPositionFirstPerson) {
						if (Time.time > weaponsManager.getLastTimeFired () + timeToCheckRunPosition && !jumpingOnProcess && !meleeAtacking) {
							if (weaponsManager.isPlayerMoving () && weaponsManager.isPlayerRunning () && weaponsManager.isPlayerOnGround ()) {
								if (!weaponInRunPosition) {
									walkOrRunWeaponPosition (true);
								}
							} else {
								if (weaponInRunPosition) {
									walkOrRunWeaponPosition (false);
								}
							}
						}
					}

					//manage if the camera fov is modified when the player is running on first person
					if (useNewFovOnRunFirstPerson) {
						if (Time.time > weaponsManager.getLastTimeFired () + timeToCheckRunPosition) {
							if (weaponsManager.isPlayerMoving () && weaponsManager.isPlayerRunning ()) {
								if (!playerRunning) {
									weaponsManager.setPlayerRunningState (true, this);
								}
							} else {
								if (playerRunning) {
									weaponsManager.setPlayerRunningState (false, this);
								}
							}
						}
					}

					if (firstPersonWeaponInfo.useJumpPositions) {
						if (playerOnJumpStart && !weaponsManager.isPlayerOnGround ()) {
							if (!weaponInJumpStart) {
								setJumpStatetWeaponPosition (true, false);
								playerOnJumpStart = false;
							}
						} 

						if (playerOnJumpEnd && weaponsManager.isPlayerOnGround ()) {
							if (!weaponInJumpEnd) {
								setJumpStatetWeaponPosition (false, true);
								playerOnJumpEnd = false;
							}
						}
					}

					if (firstPersonWeaponInfo.useCrouchPosition) {
						if (!crouchingActive && weaponsManager.isPlayerCrouching ()) {
							setCrouchStatetWeaponPosition (true);
						}

						if (crouchingActive && !weaponsManager.isPlayerCrouching ()) {
							setCrouchStatetWeaponPosition (false);
						}
					}
				}
			}
		}

		if (disablingDualWeapon && !moving) {
			disablingDualWeapon = false;
		}
	}

	public void setPlayerOnJumpStartState (bool state)
	{
		playerOnJumpStart = state;
	}

	public void setPlayerOnJumpEndState (bool state)
	{
		playerOnJumpEnd = state;
	}

	public bool isWeaponMoving ()
	{
		return moving;
	}

	public bool weaponCanFire ()
	{
		return !surfaceDetected || (!weaponSystemManager.aimingInThirdPerson && surfaceDetected);
	}

	public bool isWeaponSurfaceDetected ()
	{
		return surfaceDetected;
	}

	public void setWeaponSystemManager ()
	{
		weaponSystemManager = weaponGameObject.GetComponent<playerWeaponSystem> ();

		weaponsManager = weaponSystemManager.playerControllerGameObject.GetComponent<playerWeaponsManager> ();

		updateComponent ();
	}

	public playerWeaponSystem getWeaponSystemManager ()
	{
		return weaponSystemManager;
	}

	public int getWeaponSystemKeyNumber ()
	{
		return weaponSystemManager.getWeaponNumberKey ();
	}

	public bool isWeaponEnabled ()
	{
		return weaponEnabled;
	}

	public void setWeaponEnabledState (bool state)
	{
		//print ("set enabled "+gameObject.name + " " + state);
		weaponEnabled = state;
	}

	public string getWeaponSystemName ()
	{
		return weaponSystemManager.getWeaponSystemName ();
	}

	public void checkWeaponSidePosition ()
	{
		bool changeWeaponPositionSide = false;

		if (usingDualWeapon) {
			if (usingRightHandDualWeapon) {
				if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.placeWeaponOnKeepPositionSideBeforeDraw) {
					currentDrawPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition;

					changeWeaponPositionSide = true;
				}
			} else {
				if (thirdPersonWeaponInfo.leftHandDualWeaponInfo.placeWeaponOnKeepPositionSideBeforeDraw) {
					currentDrawPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition;

					changeWeaponPositionSide = true;
				}
			}
		} else {
			if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.placeWeaponOnKeepPositionSideBeforeDraw || thirdPersonWeaponInfo.leftHandDualWeaponInfo.placeWeaponOnKeepPositionSideBeforeDraw) {
				currentDrawPositionTransform = thirdPersonWeaponInfo.keepPosition;

				changeWeaponPositionSide = true;
			}
		}

		if (changeWeaponPositionSide) {
			weaponTransform.position = currentDrawPositionTransform.position;
			weaponTransform.rotation = currentDrawPositionTransform.rotation;
		}
	}

	//third person
	public void drawOrKeepWeaponThirdPerson (bool state)
	{
		bool moveWeapon = false;
		carrying = state;

		if (carrying) {
			//draw the weapon
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentKeepPath = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPath;
				} else {
					currentKeepPath = thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPath;
				}
			} else {
				currentKeepPath = thirdPersonWeaponInfo.keepPath;
			}

			moveWeapon = true;
		} else {
			//if the weapon is in its carrying position or if the weapon was moving towards its carrying position, stop it, reverse the path and start the movement
			if (carryingWeapon || moving) {
				inverseKeepPath.Clear ();

				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPath);
					} else {
						inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPath);
					}
				} else {
					inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.keepPath);
				}

				inverseKeepPath.Reverse ();

				currentKeepPath = inverseKeepPath;

				aiming = false;
				moveWeapon = true;
			}
		}

		//stop the coroutine to translate the camera and call it again
		stopWeaponMovement ();

		if (moveWeapon) {
			weaponMovement = StartCoroutine (drawOrKeepWeaponThirdPersonCoroutine ());
		}

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	IEnumerator drawOrKeepWeaponThirdPersonCoroutine ()
	{
		//print ("drawOrKeepWeaponThirdPersonCoroutine "+ carrying);
		moving = true;

		if (carrying) {
			weaponTransform.SetParent (weaponSystemManager.getWeaponParent ());
			if (hideWeaponIfKeptInThirdPerson) {
				enableOrDisableWeaponMesh (true);
			}

			weaponSystemManager.changeHUDPosition (true);
		}

		if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (carrying) {
				//print ("carrying");
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentKeepPath = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.deactivateIKDrawPath;
					} else {
						currentKeepPath = thirdPersonWeaponInfo.leftHandDualWeaponInfo.deactivateIKDrawPath;
					}
				} else {
					currentKeepPath = thirdPersonWeaponInfo.deactivateIKDrawPath;
				}
			} else {
				//print ("not carrying");
				inverseKeepPath.Clear ();

				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.deactivateIKDrawPath);
					} else {
						inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.leftHandDualWeaponInfo.deactivateIKDrawPath);
					}
				} else {
					inverseKeepPath = new List<Transform> (thirdPersonWeaponInfo.deactivateIKDrawPath);
				}

				inverseKeepPath.Reverse ();

				currentKeepPath = inverseKeepPath;

				if (isUsingDualWeapon () && !usingWeaponAsOneHandWield) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					setHandDeactivateIKStateToDisable ();
						
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							setHandDeactivateIKStateToDisable ();
						}
					}
				}

				weaponTransform.SetParent (transform);
			}
		}

		if (carrying) {
			resetElbowIKPositions ();
		} else {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				setElbowTargetValue (currentIKWeaponsPosition, 0);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];
				
					setElbowTargetValue (currentIKWeaponsPosition, 0);
				
					if (!currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.elbowInfo.position.SetParent (transform);
					}
				}
			}
		}

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				useBezierCurve = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.useBezierCurve;

				if (useBezierCurve) {
					spline = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.spline;
					bezierDuration = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.bezierDuration;
					lookDirectionSpeed = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.lookDirectionSpeed;
				}

			} else {
				useBezierCurve = thirdPersonWeaponInfo.leftHandDualWeaponInfo.useBezierCurve;

				if (useBezierCurve) {
					spline = thirdPersonWeaponInfo.leftHandDualWeaponInfo.spline;
					bezierDuration = thirdPersonWeaponInfo.leftHandDualWeaponInfo.bezierDuration;
					lookDirectionSpeed = thirdPersonWeaponInfo.leftHandDualWeaponInfo.lookDirectionSpeed;
				}
			}
		} else {
			useBezierCurve = thirdPersonWeaponInfo.useBezierCurve;

			if (useBezierCurve) {
				spline = thirdPersonWeaponInfo.spline;
				bezierDuration = thirdPersonWeaponInfo.bezierDuration;
				lookDirectionSpeed = thirdPersonWeaponInfo.lookDirectionSpeed;
			}
		}

		if (useBezierCurve) {

			bool targetReached = false;

			float angleDifference = 0;

			if (!carrying) {
				Transform targetTransform = spline.GetLookTransform (1);

				float dist = GKC_Utils.distance (weaponTransform.position, targetTransform.position);

				float duration = dist / thirdPersonWeaponInfo.drawWeaponMovementSpeed;

				float t = 0;

				Vector3 pos = targetTransform.localPosition;
				Quaternion rot = targetTransform.localRotation;

				float movementTimer = 0;

				while (!targetReached) {
					t += Time.deltaTime / duration; 
					weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, pos, t);
					weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);

					angleDifference = Quaternion.Angle (weaponTransform.localRotation, rot);

					movementTimer += Time.deltaTime;

					if ((GKC_Utils.distance (weaponTransform.localPosition, pos) < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
						targetReached = true;
					}
					yield return null;
				}
			}

			float progress = 0;
			float progressTarget = 1;

			if (!carrying) {
				progress = 1;
				progressTarget = 0;
			}

			targetReached = false;

			angleDifference = 0;
				
			while (!targetReached) {
				if (carrying) {
					progress += Time.deltaTime / bezierDuration;
				} else {
					progress -= Time.deltaTime / bezierDuration;
				}

				Vector3 position = spline.GetPoint (progress);
				weaponTransform.position = position;

				Quaternion targetRotation = Quaternion.Euler (spline.GetLookDirection (progress));
				weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, targetRotation, Time.deltaTime * lookDirectionSpeed);

				if ((carrying && progress > progressTarget) || (!carrying && progress < progressTarget)) {
					if (carrying) {
						angleDifference = Quaternion.Angle (weaponTransform.localRotation, targetRotation);

						if (angleDifference < 0.2f) {
							targetReached = true;
						}
					} else {
						targetReached = true;
					}
				}

				yield return null;
			}

		} else {
			bool targetReached = false;

			float angleDifference = 0;

			float movementTimer = 0;

			foreach (Transform transformPath in currentKeepPath) {
				float dist = GKC_Utils.distance (weaponTransform.position, transformPath.position);

				float duration = dist / thirdPersonWeaponInfo.drawWeaponMovementSpeed;

				float t = 0;

				Vector3 pos = transformPath.localPosition;
				Quaternion rot = transformPath.localRotation;

				targetReached = false;

				angleDifference = 0;

				movementTimer = 0;


				while (!targetReached) {
					t += Time.deltaTime / duration; 
					weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, pos, t);
					weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);

					angleDifference = Quaternion.Angle (weaponTransform.localRotation, rot);

					movementTimer += Time.deltaTime;

					if ((GKC_Utils.distance (weaponTransform.localPosition, pos) < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
						targetReached = true;
					}
					yield return null;
				}
			}
		}

		if (carrying) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				setElbowTargetValue (currentIKWeaponsPosition, 1);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						setElbowTargetValue (currentIKWeaponsPosition, 1);
					}
				}
			}
		}

		if (!aiming && !carrying) {
			weaponTransform.SetParent (weaponSystemManager.weaponSettings.weaponParent);

			if (isUsingDualWeapon () || weaponConfiguredAsDualWeaponPreviously) {
				if (usingRightHandDualWeapon) {
					currentDrawPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition;
				} else {
					currentDrawPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition;
				}

			} else {
				currentDrawPositionTransform = thirdPersonWeaponInfo.keepPosition;
			}

			float dist = GKC_Utils.distance (weaponTransform.localPosition, currentDrawPositionTransform.localPosition);

			float duration = dist / thirdPersonWeaponInfo.drawWeaponMovementSpeed;

			Vector3 pos = currentDrawPositionTransform.localPosition;
			Quaternion rot = currentDrawPositionTransform.localRotation;

			float t = 0;
			while (t < 1) {
				t += Time.deltaTime / duration;
				weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, pos, t);
				weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);
				yield return null;
			}

			setHandsIKTargetValue (0, 0);
			setElbowsIKTargetValue (0, 0);

			if (isUsingDualWeapon () || weaponConfiguredAsDualWeaponPreviously) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				currentIKWeaponsPosition.handInPositionToAim = false;

				currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
				currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

				weaponConfiguredAsDualWeaponPreviously = false;
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.handInPositionToAim = false;

						currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
						currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
						currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;
					}
				}
			}

			checkWeaponSidePosition ();
		}

		moving = false;
		carryingWeapon = carrying;

		if (carryingWeapon && !isUsingDualWeapon ()) {
			weaponsManager.grabWeaponWithNoDrawHand ();
		}

		if (carryingWeapon && usingWeaponAsOneHandWield) {
			if (usingWeaponAsOneHandWield) {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					currentIKWeaponsPosition.handInPositionToAim = true;
				}

				weaponsManager.getIKSystem ().checkHandsInPosition (true);
			} 
		}

		if (!carrying) {
			if (hideWeaponIfKeptInThirdPerson) {
				enableOrDisableWeaponMesh (false);
			}
		}

		if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (carryingWeapon) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
					currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
					currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

					weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
							currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
							currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

							weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
						}
					}
				}

				setHandsIKTargetValue (0, 0);
				setElbowsIKTargetValue (0, 0);
			}
		}
	}

	public void setHandDeactivateIKStateToDisable ()
	{
		currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
		currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
		currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;

		currentIKWeaponsPosition.HandIKWeight = 1;

		setHandTargetValue (currentIKWeaponsPosition, 1);
	}

	public void setHandTargetValue (IKWeaponsPosition handInfo, float newValue)
	{
		handInfo.targetValue = newValue;
	}

	public void setElbowTargetValue (IKWeaponsPosition handInfo, float newValue)
	{
		handInfo.elbowInfo.targetValue = newValue;
	}

	public void placeWeaponDirectlyOnDrawHand (bool state)
	{
		bool moveWeapon = false;
		carrying = state;

		if (carrying) {
			//draw the weapon
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentDrawPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
				} else {
					currentDrawPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
				}
			} else {
				currentDrawPositionTransform = thirdPersonWeaponInfo.walkPosition;
			}

			moveWeapon = true;
		} else {
			//if the weapon is in its carrying position or if the weapon was moving towards its carrying position, stop it, reverse the path and start the movement
			if (carryingWeapon || moving) {

				aiming = false;
				moveWeapon = true;
			}
		}

		//stop the coroutine to translate the camera and call it again
		stopWeaponMovement ();

		if (moveWeapon) {
			weaponMovement = StartCoroutine (placeWeaponDirectlyOnDrawHandCoroutine ());
		}

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	IEnumerator placeWeaponDirectlyOnDrawHandCoroutine ()
	{
		//print ("drawOrKeepWeaponThirdPersonCoroutine "+ carrying);
		moving = true;

		bool originallyDeactivateIKIfNotAiming = false;

		if (carrying) {
			weaponTransform.SetParent (weaponSystemManager.getWeaponParent ());
			if (hideWeaponIfKeptInThirdPerson) {
				enableOrDisableWeaponMesh (true);
			}
		
			if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
				if (carrying) {
					originallyDeactivateIKIfNotAiming = true;

					quickDrawWeaponThirdPerson ();

					if (usingDualWeapon) {
						if (usingRightHandDualWeapon) {
							weaponsManager.getIKSystem ().quickDrawWeaponStateDualWeapon (thirdPersonWeaponInfo, true);
						} else {
							weaponsManager.getIKSystem ().quickDrawWeaponStateDualWeapon (thirdPersonWeaponInfo, false);
						}
					} else {
						weaponsManager.getIKSystem ().quickDrawWeaponState (thirdPersonWeaponInfo);
					}
				}
			} else {
				if (carrying) {
					thirdPersonWeaponInfo.deactivateIKIfNotAiming = true;

					quickDrawWeaponThirdPerson ();

					thirdPersonWeaponInfo.deactivateIKIfNotAiming = false;
				}
			}

			if (!originallyDeactivateIKIfNotAiming) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}
			
					setHandDeactivateIKStateToDisable ();
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							setHandDeactivateIKStateToDisable ();
						}
					}
				}
				
				if (thirdPersonWeaponInfo.useWeaponRotationPoint) {
					weaponTransform.SetParent (thirdPersonWeaponInfo.weaponRotationPointHolder);
				} else {
					weaponTransform.SetParent (transform);
				}

				resetElbowIKPositions ();

				bool targetReached = false;

				float angleDifference = 0;

				float movementTimer = 0;

				float dist = GKC_Utils.distance (weaponTransform.position, currentDrawPositionTransform.position);

				float duration = dist / thirdPersonWeaponInfo.drawWeaponMovementSpeed;

				float t = 0;

				Vector3 pos = currentDrawPositionTransform.localPosition;
				Quaternion rot = currentDrawPositionTransform.localRotation;

				while (!targetReached) {
					t += Time.deltaTime / duration; 
					weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, pos, t);
					weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);

					angleDifference = Quaternion.Angle (weaponTransform.localRotation, rot);

					movementTimer += Time.deltaTime;

					if ((GKC_Utils.distance (weaponTransform.localPosition, pos) < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
						targetReached = true;
					}
					yield return null;
				}

				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					setElbowTargetValue (currentIKWeaponsPosition, 1);
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							setElbowTargetValue (currentIKWeaponsPosition, 1);
						}
					}
				}

				if (!isUsingDualWeapon ()) {
					weaponsManager.grabWeaponWithNoDrawHand ();
				}
			}
		} else {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
				currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

				weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
						currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
						currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

						weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
					}
				}
			}

			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				setElbowTargetValue (currentIKWeaponsPosition, 0);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					setElbowTargetValue (currentIKWeaponsPosition, 0);

					if (!currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.elbowInfo.position.SetParent (transform);
					}
				}
			}

			setHandsIKTargetValue (0, 0);
			setElbowsIKTargetValue (0, 0);

			bool targetReached = false;

			while (!targetReached) {
				if (disablingDualWeapon) {
					if (usingRightHandDualWeapon) {
						if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0].HandIKWeight <= 0) {
							targetReached = true;
						}
					} else {
						if (thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0].HandIKWeight <= 0) {
							targetReached = true;
						}
					}
				} else {
					int c = 0;
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						if (thirdPersonWeaponInfo.handsInfo [i].HandIKWeight == 0) {
							c++;
						}
					}

					if (c == 2) {
						targetReached = true;
					}
				}

				yield return null;
			}

			quickKeepWeaponThirdPerson ();
		}

		moving = false;
		carryingWeapon = carrying;
	}

	public void aimOrDrawWeaponThirdPerson (bool state)
	{
		if (currentWeapon) {
			aiming = state;

			aimingWeaponInProcess = true;

			if (surfaceDetected) {
				surfaceDetected = false;
				bool firstPersonActive = weaponsManager.isFirstPersonActive ();

				setWeaponRegularCursorState (firstPersonActive);
			}

			if (aiming) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentAimPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.aimPosition;
					} else {
						currentAimPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.aimPosition;
					}
				} else {
					currentAimPositionTransform = thirdPersonWeaponInfo.aimPosition;
				}
			} else {

				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentAimPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
					} else {
						currentAimPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
					}
				} else {
					currentAimPositionTransform = thirdPersonWeaponInfo.walkPosition;
				}
			}

			weaponPositionTarget = currentAimPositionTransform.localPosition;
			weaponRotationTarget = currentAimPositionTransform.localRotation;
		
			//stop the coroutine to translate the camera and call it again
			stopWeaponMovement ();

			weaponMovement = StartCoroutine (aimOrDrawWeaponThirdPersonCoroutine ());

			disableOtherWeaponsStates ();

			setPlayerControllerMovementValues ();

			if (thirdPersonWeaponInfo.useSwayInfo) {
				setCurrentSwayInfo (!weaponInRunPosition);
				if (aiming) {
					resetSwayValue ();
				}
			}
		}
	}

	IEnumerator aimOrDrawWeaponThirdPersonCoroutine ()
	{
		if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {

			bool rightHandUsedToDrawWeapon = false;

			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}
					
				if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
					rightHandUsedToDrawWeapon = true;
				}

				if (aiming) {
					currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
					currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
					currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;

					currentIKWeaponsPosition.HandIKWeight = 1;
				}
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
							rightHandUsedToDrawWeapon = true;
						}

						if (aiming) {
							currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
							currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
							currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;

							currentIKWeaponsPosition.HandIKWeight = 1;
						}
					}
				}
			}

			if (aiming) {
				if (thirdPersonWeaponInfo.useWeaponRotationPoint) {
					weaponTransform.SetParent (thirdPersonWeaponInfo.weaponRotationPointHolder);
				} else {
					weaponTransform.SetParent (transform);
				}
			}

			if (rightHandUsedToDrawWeapon) {
				setHandsIKTargetValue (0, 1);
				setElbowsIKTargetValue (0, 1);
			} else {
				setHandsIKTargetValue (1, 0);
				setElbowsIKTargetValue (1, 0);
			}
		} else {
			if (thirdPersonWeaponInfo.useWeaponRotationPoint) {
				if (aiming) {
					weaponTransform.SetParent (thirdPersonWeaponInfo.weaponRotationPointHolder);
				} else {
					weaponTransform.SetParent (transform);
				}
			}
		}

		bool placeWeaponOnWalkPositionBeforeDeactivateIK = false;

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				placeWeaponOnWalkPositionBeforeDeactivateIK = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.placeWeaponOnWalkPositionBeforeDeactivateIK;
			} else {
				placeWeaponOnWalkPositionBeforeDeactivateIK = thirdPersonWeaponInfo.leftHandDualWeaponInfo.placeWeaponOnWalkPositionBeforeDeactivateIK;
			}
		} else {
			placeWeaponOnWalkPositionBeforeDeactivateIK = thirdPersonWeaponInfo.placeWeaponOnWalkPositionBeforeDeactivateIK;
		}

		if (aiming || !thirdPersonWeaponInfo.deactivateIKIfNotAiming || placeWeaponOnWalkPositionBeforeDeactivateIK) {
			moving = true;
			//print ("aimOrDrawWeaponThirdPersonCoroutine "+ aiming);
			Vector3 currentWeaponPosition = weaponTransform.localPosition;
			Quaternion currentWeaponRotation = weaponTransform.localRotation;
			for (float t = 0; t < 1;) {
				t += Time.deltaTime * thirdPersonWeaponInfo.aimMovementSpeed;
				weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
				weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
				yield return null;
			}
			moving = false;
		}

		if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (aiming) {
				setHandsIKTargetValue (1, 1);
				setElbowsIKTargetValue (1, 1);
			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
					currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
					currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

					weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
							currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
							currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

							weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
						}
					}
				}
				
				setHandsIKTargetValue (0, 0);
				setElbowsIKTargetValue (0, 0);
			}
		}

		aimingWeaponInProcess = false;
	}

	public void walkOrSurfaceCollision (bool state)
	{
		if (currentWeapon) {
			surfaceDetected = state;
			bool firstPersonActive = weaponsManager.isFirstPersonActive ();

			float movementSpeed = thirdPersonWeaponInfo.collisionMovementSpeed;
			if (firstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.collisionMovementSpeed;
			}

			if (surfaceDetected) {
				if (firstPersonActive) {
					if (firstPersonWeaponInfo.lowerWeaponPosition && weaponsManager.isCarryWeaponInLowerPositionActive ()) {
						if (isUsingDualWeapon ()) {
							if (usingRightHandDualWeapon) {
								currentCollisionPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.lowerWeaponPosition;
							} else {
								currentCollisionPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.lowerWeaponPosition;
							}
						} else {
							currentCollisionPositionTransform = firstPersonWeaponInfo.lowerWeaponPosition;
						}

					} else {
						if (isUsingDualWeapon ()) {
							if (usingRightHandDualWeapon) {
								currentCollisionPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.surfaceCollisionPosition;
							} else {
								currentCollisionPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.surfaceCollisionPosition;
							}
						} else {
							currentCollisionPositionTransform = firstPersonWeaponInfo.surfaceCollisionPosition;
						}

					}
				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentCollisionPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.surfaceCollisionPosition;
						} else {
							currentCollisionPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.surfaceCollisionPosition;
						}
					} else {
						currentCollisionPositionTransform = thirdPersonWeaponInfo.surfaceCollisionPosition;
					}

				}
			} else {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							if (crouchingActive) {
								currentCollisionPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
							} else {
								currentCollisionPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
							}
						} else {
							if (crouchingActive) {
								currentCollisionPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
							} else {
								currentCollisionPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
							}
						}
					} else {
						if (crouchingActive) {
							currentCollisionPositionTransform = firstPersonWeaponInfo.crouchPosition;
						} else {
							currentCollisionPositionTransform = firstPersonWeaponInfo.walkPosition;
						}
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentCollisionPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.aimPosition;
						} else {
							currentCollisionPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.aimPosition;
						}
					} else {
						currentCollisionPositionTransform = thirdPersonWeaponInfo.aimPosition;
					}
				}
			}

			weaponPositionTarget = currentCollisionPositionTransform.localPosition;
			weaponRotationTarget = currentCollisionPositionTransform.localRotation;

			setWeaponRegularCursorState (firstPersonActive);

			if (!firstPersonActive) {
				if (!isUsingDualWeapon ()) {
					weaponsManager.checkSetExtraRotationCoroutine (!state);
				}
			}

			stopWeaponMovement ();
			weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (movementSpeed));
		}
	}

	public void setWeaponRegularCursorState (bool firstPersonActive)
	{
		if (firstPersonActive) {
			if (isUsingDualWeapon ()) {
				if (surfaceDetected) {
					if (!weaponsManager.currentRightIKWeapon.isWeaponSurfaceDetected () || !weaponsManager.currentLeftIkWeapon.isWeaponSurfaceDetected ()) {
						return;
					}
				}
			}

			if (firstPersonWeaponInfo.hideCursorOnCollision) {
				weaponsManager.enableOrDisableGeneralWeaponCursor (!surfaceDetected);
				cursorHidden = surfaceDetected;
			} else {
				weaponsManager.enableOrDisableGeneralWeaponCursor (true);
			}
		} else {
			if (thirdPersonWeaponInfo.hideCursorOnCollision) {
				weaponsManager.enableOrDisableGeneralWeaponCursor (!surfaceDetected);
				cursorHidden = surfaceDetected;
			} else {
				weaponsManager.enableOrDisableGeneralWeaponCursor (true);
			}
		}

		if (!firstPersonActive) {
			weaponsManager.enableOrDisableWeaponCursorUnableToShoot (surfaceDetected);
		}
	}

	public void walkOrRunWeaponPosition (bool state)
	{
		if (currentWeapon && weaponInRunPosition != state) {
			weaponInRunPosition = state;

			surfaceDetected = false;
			meleeAtacking = false;

			bool firstPersonActive = weaponsManager.isFirstPersonActive ();

			float movementSpeed = thirdPersonWeaponInfo.runMovementSpeed;
			if (firstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.runMovementSpeed;
			}

			if (weaponInRunPosition) {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentWalkOrRunPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.runPosition;
						} else {
							currentWalkOrRunPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.runPosition;
						}
					} else {
						currentWalkOrRunPositionTransform = firstPersonWeaponInfo.runPosition;
					}
				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.runPosition;
						} else {
							currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.runPosition;
						}
					} else {
						currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.runPosition;
					}
				}
			} else {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							if (crouchingActive) {
								currentWalkOrRunPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
							} else {
								currentWalkOrRunPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
							}
						} else {
							if (crouchingActive) {
								currentWalkOrRunPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
							} else {
								currentWalkOrRunPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
							}
						}
					} else {
						if (crouchingActive) {
							currentWalkOrRunPositionTransform = firstPersonWeaponInfo.crouchPosition;
						} else {
							currentWalkOrRunPositionTransform = firstPersonWeaponInfo.walkPosition;
						}
					}
				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						} else {
							currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					} else {
						currentWalkOrRunPositionTransform = thirdPersonWeaponInfo.walkPosition;
					}
				}
			}

			weaponPositionTarget = currentWalkOrRunPositionTransform.localPosition;
			weaponRotationTarget = currentWalkOrRunPositionTransform.localRotation;

			checkCursorState (firstPersonActive);

			stopWeaponMovement ();

			weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (movementSpeed));
		}
	}

	public void checkCursorState (bool firstPersonActive)
	{
		if (firstPersonActive) {
			if (firstPersonWeaponInfo.hideCursorOnRun) {
				weaponsManager.enableOrDisableGeneralWeaponCursor (!weaponInRunPosition);
				cursorHidden = weaponInRunPosition;
			}

			if (firstPersonWeaponInfo.useSwayInfo || thirdPersonWeaponInfo.useSwayInfo) {
				setCurrentSwayInfo (!weaponInRunPosition);
			}
		} else {
			if (thirdPersonWeaponInfo.hideCursorOnRun) {
				weaponsManager.enableOrDisableGeneralWeaponCursor (!weaponInRunPosition);
				cursorHidden = weaponInRunPosition;
			}

			if (firstPersonWeaponInfo.useSwayInfo || thirdPersonWeaponInfo.useSwayInfo) {
				setCurrentSwayInfo (!weaponInRunPosition);
			}
		}
	}

	public void setJumpStatetWeaponPosition (bool isJumpStart, bool isJumpEnd)
	{
		if (currentWeapon) {
			bool firstPersonActive = weaponsManager.isFirstPersonActive ();

			meleeAtacking = false;

			weaponInRunPosition = false;

			float movementSpeed = 0;
			if (firstPersonActive) {
				if (isJumpStart) {
					movementSpeed = firstPersonWeaponInfo.jumpStartMovementSpeed;
				} else {
					movementSpeed = firstPersonWeaponInfo.jumpEndtMovementSpeed;
				}
			} else {
				if (isJumpStart) {
					movementSpeed = thirdPersonWeaponInfo.jumpStartMovementSpeed;
				} else {
					movementSpeed = thirdPersonWeaponInfo.jumpEndtMovementSpeed;
				}
			}

			float delayAtJumpEnd = thirdPersonWeaponInfo.delayAtJumpEnd;
			if (firstPersonActive) {
				delayAtJumpEnd = firstPersonWeaponInfo.delayAtJumpEnd;
			}

			weaponInJumpStart = isJumpStart;
			weaponInJumpEnd = isJumpEnd;

			if (!jumpingOnProcess) {
				jumpingOnProcess = true;
			}

			if (weaponInJumpStart) {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.jumpStartPosition;
						} else {
							currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.jumpStartPosition;
						}
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.jumpStartPosition;
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.jumpStartPosition;
						} else {
							currentJumpPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.jumpStartPosition;
						}
					} else {
						currentJumpPositionTransform = thirdPersonWeaponInfo.jumpStartPosition;
					}

				}
			} else {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.jumpEndPosition;
						} else {
							currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.jumpEndPosition;
						}
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.jumpEndPosition;
					}
				
				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.jumpEndPosition;
						} else {
							currentJumpPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.jumpEndPosition;
						}
					} else {
						currentJumpPositionTransform = thirdPersonWeaponInfo.jumpEndPosition;
					}
				
				}
			}

			weaponPositionTarget = currentJumpPositionTransform.localPosition;
			weaponRotationTarget = currentJumpPositionTransform.localRotation;
				
			stopWeaponMovement ();

			if (isJumpStart) {
				weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (movementSpeed));
			} else {
				weaponMovement = StartCoroutine (weaponEndJumpCoroutine (movementSpeed, delayAtJumpEnd));
			}
		}
	}

	IEnumerator moveWeaponToPositionCoroutine (float movementSpeed)
	{
		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * movementSpeed;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}
	}

	IEnumerator weaponEndJumpCoroutine (float movementSpeed, float delayAtJumpEnd)
	{
		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * movementSpeed;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}

		yield return new WaitForSeconds (delayAtJumpEnd);

		bool firstPersonActive = weaponsManager.isFirstPersonActive ();
		if (firstPersonActive) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					if (crouchingActive) {
						currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
					}
				} else {
					if (crouchingActive) {
						currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
					}
				}
			} else {
				if (crouchingActive) {
					currentJumpPositionTransform = firstPersonWeaponInfo.crouchPosition;
				} else {
					currentJumpPositionTransform = firstPersonWeaponInfo.walkPosition;
				}
			}
		} else {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentJumpPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
				} else {
					currentJumpPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
				}
			} else {
				currentJumpPositionTransform = thirdPersonWeaponInfo.walkPosition;
			}
		}

		weaponPositionTarget = currentJumpPositionTransform.localPosition;
		weaponRotationTarget = currentJumpPositionTransform.localRotation;

		if (firstPersonActive) {
			movementSpeed = firstPersonWeaponInfo.resetJumpMovementSped;
		} else {
			movementSpeed = thirdPersonWeaponInfo.resetJumpMovementSped;
		}

		stopWeaponMovement ();
		weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (movementSpeed));

		weaponInJumpEnd = false;
		jumpingOnProcess = false;
	}

	public void setCrouchStatetWeaponPosition (bool state)
	{
		if (currentWeapon) {
			bool firstPersonActive = weaponsManager.isFirstPersonActive ();

			setLastTimeMoved ();

			crouchingActive = state;

			meleeAtacking = false;

			weaponInRunPosition = false;

			float movementSpeed = 0;
			if (firstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.crouchMovementSpeed;
			} else {
				movementSpeed = thirdPersonWeaponInfo.crouchMovementSpeed;
			}
				
			if (crouchingActive) {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
						} else {
							currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
						}
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.crouchPosition;
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
						} else {
							currentJumpPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
						}
					} else {
						currentJumpPositionTransform = thirdPersonWeaponInfo.crouchPosition;
					}

				}
			} else {
				if (firstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						} else {
							currentJumpPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					} else {
						currentJumpPositionTransform = firstPersonWeaponInfo.walkPosition;
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentJumpPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						} else {
							currentJumpPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					} else {
						currentJumpPositionTransform = thirdPersonWeaponInfo.walkPosition;
					}

				}
			}

			weaponPositionTarget = currentJumpPositionTransform.localPosition;
			weaponRotationTarget = currentJumpPositionTransform.localRotation;

			stopWeaponMovement ();

			weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (movementSpeed));
		}
	}

	public bool isWeaponInRunPosition ()
	{
		return weaponInRunPosition;
	}

	public bool useRunPositionOnFirstPerson ()
	{
		return useRunPositionFirstPerson;
	}

	public bool useRunPositionOnThirdPerson ()
	{
		return useRunPositionThirdPerson;
	}

	public bool isPlayerRunning ()
	{
		return playerRunning;
	}

	public void setPlayerRunningState (bool state)
	{
		playerRunning = state;
	}

	public bool isWeaponItJumpStart ()
	{
		return weaponInJumpStart;
	}

	public bool isWeaponItJumpEnd ()
	{
		return weaponInJumpEnd;
	}

	public bool isjumpingOnProcess ()
	{
		return jumpingOnProcess;
	}

	public bool weaponUseJumpPositions ()
	{
		return (firstPersonWeaponInfo.useJumpPositions && weaponsManager.isFirstPersonActive ()) || (thirdPersonWeaponInfo.useJumpPositions && !weaponsManager.isFirstPersonActive ());
	}

	public bool useNewFovOnRunOnThirdPerson ()
	{
		return useNewFovOnRunThirdPerson;
	}

	public bool useNewFovOnRunOnFirstPerson ()
	{
		return useNewFovOnRunFirstPerson;
	}

	public bool isCursorHidden ()
	{
		return cursorHidden;
	}

	public void setCursorHiddenState (bool state)
	{
		cursorHidden = state;
	}

	public void setCurrentSwayInfo (bool state)
	{
		if (weaponsManager && weaponsManager.isFirstPersonActive ()) {
			if (state) {
				//print ("regular first person");
				currentlyUsingSway = firstPersonSwayInfo.useSway;
				if (currentlyUsingSway) {
					currentSwayInfo = firstPersonSwayInfo;
				}
			} else {
				//print ("run first person");
				currentlyUsingSway = runFirstPersonSwayInfo.useSway;
				if (currentlyUsingSway) {
					currentSwayInfo = runFirstPersonSwayInfo;
				}
			}
		} else {
			if (state) {
				//print ("regular third person");
				currentlyUsingSway = thirdPersonSwayInfo.useSway;
				if (currentlyUsingSway) {
					currentSwayInfo = thirdPersonSwayInfo;
				}
			} else {
				//print ("run third person");
				currentlyUsingSway = runThirdPersonSwayInfo.useSway;
				if (currentlyUsingSway) {
					currentSwayInfo = runThirdPersonSwayInfo;
				}
			}
		}
	}

	public bool isHideCursorOnAimingEnabled ()
	{
		return (firstPersonWeaponInfo.hideCursorOnAming && weaponsManager.isFirstPersonActive ()) ||
		(thirdPersonWeaponInfo.hideCursorOnAming && !weaponsManager.isFirstPersonActive ());
	}

	public void walkOrMeleeAttackWeaponPosition ()
	{
		if (meleeAtacking) {
			return;
		}

		bool firstPersonActive = weaponsManager.isFirstPersonActive ();

		if (currentWeapon && (thirdPersonWeaponInfo.useMeleeAttack || firstPersonWeaponInfo.useMeleeAttack) &&
		    ((!weaponsManager.isAimingWeapons () && firstPersonActive) || (!firstPersonActive && !surfaceDetected))) {

			if ((firstPersonActive && !firstPersonWeaponInfo.useMeleeAttack) || (!firstPersonActive && !thirdPersonWeaponInfo.useMeleeAttack)) {
				return;
			}

			if (!firstPersonActive && thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					setHandDeactivateIKStateToDisable ();

					weaponTransform.SetParent (transform);
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							setHandDeactivateIKStateToDisable ();

							weaponTransform.SetParent (transform);
						}
					}
				}
			}

			meleeAtacking = true;

			surfaceDetected = false;

			checkCursorState (firstPersonActive);

			weaponInRunPosition = false;

			float movementSpeed = thirdPersonWeaponInfo.meleeAttackStartMovementSpeed;
			if (firstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.meleeAttackStartMovementSpeed;
			}
				
			if (firstPersonActive) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentMeleeAttackPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.meleeAttackPosition;
					} else {
						currentMeleeAttackPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.meleeAttackPosition;
					}
				} else {
					currentMeleeAttackPositionTransform = firstPersonWeaponInfo.meleeAttackPosition;
				}

			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.meleeAttackPosition;
					} else {
						currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.meleeAttackPosition;
					}
				} else {
					currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.meleeAttackPosition;
				}
			}
				
			weaponPositionTarget = currentMeleeAttackPositionTransform.localPosition;
			weaponRotationTarget = currentMeleeAttackPositionTransform.localRotation;

			float delayEndMeleeAttack = thirdPersonWeaponInfo.meleeAttackEndDelay;
			if (firstPersonActive) {
				delayEndMeleeAttack = firstPersonWeaponInfo.meleeAttackEndDelay;
			}

			weaponSystemManager.checkMeleeAttackShakeInfo ();

			stopWeaponMovement ();
			weaponMovement = StartCoroutine (weaponStartMeleeAttack (movementSpeed, delayEndMeleeAttack));
		}
	}

	IEnumerator weaponStartMeleeAttack (float movementSpeed, float delayEndMeleeAttack)
	{
		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * movementSpeed;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}

		checkMeleeAttackDamage ();

		yield return new WaitForSeconds (delayEndMeleeAttack);

		bool firstPersonActive = weaponsManager.isFirstPersonActive ();

		if (firstPersonActive || !thirdPersonWeaponInfo.deactivateIKIfNotAiming || (thirdPersonWeaponInfo.deactivateIKIfNotAiming && aiming)) {

			if (firstPersonActive) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						if (crouchingActive) {
							currentMeleeAttackPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
						} else {
							currentMeleeAttackPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						}
					} else {
						if (crouchingActive) {
							currentMeleeAttackPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
						} else {
							currentMeleeAttackPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					}
				} else {
					if (crouchingActive) {
						currentMeleeAttackPositionTransform = firstPersonWeaponInfo.crouchPosition;
					} else {
						currentMeleeAttackPositionTransform = firstPersonWeaponInfo.walkPosition;
					}
				}
			} else {
				if (weaponsManager.aimingInThirdPerson) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.aimPosition;
						} else {
							currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.aimPosition;
						}
					} else {
						currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.aimPosition;
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						} else {
							currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					} else {
						currentMeleeAttackPositionTransform = thirdPersonWeaponInfo.walkPosition;
					}
				
				}
			}

			weaponPositionTarget = currentMeleeAttackPositionTransform.localPosition;
			weaponRotationTarget = currentMeleeAttackPositionTransform.localRotation;

			if (firstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.meleeAttackEndMovementSpeed;
			} else {
				movementSpeed = thirdPersonWeaponInfo.meleeAttackEndMovementSpeed;
			}
			
			currentWeaponPosition = weaponTransform.localPosition;
			currentWeaponRotation = weaponTransform.localRotation;

			for (float t = 0; t < 1;) {
				t += Time.deltaTime * movementSpeed;
				weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
				weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
				yield return null;
			}
		} else {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
				currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

				weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
						currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
						currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

						weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
					}
				}
			}

			setHandsIKTargetValue (0, 0);
			setElbowsIKTargetValue (0, 0);
		}

		meleeAtacking = false;
	}

	public void checkWeaponCameraShake ()
	{
		weaponSystemManager.checkWeaponCameraShake ();
	}

	public void checkMeleeAttackShakeInfo ()
	{
		weaponSystemManager.checkMeleeAttackShakeInfo ();
	}

	public void checkMeleeAttackDamage ()
	{
		Transform raycastPosition = thirdPersonWeaponInfo.meleeAttackRaycastPosition;
		float raycastDistance = thirdPersonWeaponInfo.meleeAttackRaycastDistance;
		float raycastRadius = thirdPersonWeaponInfo.meleeAttackRaycastRadius;
		float damageAmount = thirdPersonWeaponInfo.meleeAttackDamageAmount;
		bool applyMeleeAtackForce = thirdPersonWeaponInfo.applyMeleeAtackForce;
		float meleeAttackForceAmount = thirdPersonWeaponInfo.meleeAttackForceAmount;
		ForceMode meleeAttackForceMode = thirdPersonWeaponInfo.meleeAttackForceMode;
		bool useMeleeAttackSound = thirdPersonWeaponInfo.useMeleeAttackSound;
		AudioClip meleeAttackSurfaceSound = thirdPersonWeaponInfo.meleeAttackSurfaceSound;
		AudioClip meleeAttackAirSound = thirdPersonWeaponInfo.meleeAttackAirSound;
		bool useMeleeAttackParticles = thirdPersonWeaponInfo.useMeleeAttackParticles;
		GameObject meleeAttackParticles = thirdPersonWeaponInfo.meleeAttackParticles;
		bool meleeAttackApplyForceToVehicles = thirdPersonWeaponInfo.meleeAttackApplyForceToVehicles;
		float meleAttackForceToVehicles = thirdPersonWeaponInfo.meleAttackForceToVehicles;


		bool firstPersonActive = weaponsManager.isFirstPersonActive ();
		if (firstPersonActive) {
			raycastPosition = weaponsManager.getMainCameraTransform ();
			raycastDistance = firstPersonWeaponInfo.meleeAttackRaycastDistance;
			raycastRadius = firstPersonWeaponInfo.meleeAttackRaycastRadius;
			damageAmount = firstPersonWeaponInfo.meleeAttackDamageAmount;
			applyMeleeAtackForce = firstPersonWeaponInfo.applyMeleeAtackForce;
			meleeAttackForceAmount = firstPersonWeaponInfo.meleeAttackForceAmount;
			meleeAttackForceMode = firstPersonWeaponInfo.meleeAttackForceMode;
			useMeleeAttackSound = firstPersonWeaponInfo.useMeleeAttackSound;
			meleeAttackSurfaceSound = firstPersonWeaponInfo.meleeAttackSurfaceSound;
			meleeAttackAirSound = firstPersonWeaponInfo.meleeAttackAirSound;
			useMeleeAttackParticles = firstPersonWeaponInfo.useMeleeAttackParticles;
			meleeAttackParticles = firstPersonWeaponInfo.meleeAttackParticles;
			meleeAttackApplyForceToVehicles = firstPersonWeaponInfo.meleeAttackApplyForceToVehicles;
			meleAttackForceToVehicles = firstPersonWeaponInfo.meleAttackForceToVehicles;
		} 

		//	if (Physics.SphereCast (raycastPosition.position, raycastRadius, raycastPosition.forward, out hit, raycastDistance, layer)) {
		//print (hit.collider.name);
		if (Physics.Raycast (raycastPosition.position, raycastPosition.forward, out hit, raycastDistance, layer)) {
			List<Collider> colliders = new List<Collider> ();
			colliders.AddRange (Physics.OverlapSphere (hit.point, raycastRadius, layer));
			foreach (Collider col in colliders) {
				GameObject objectToDamage = col.gameObject;
				applyDamage.checkHealth (gameObject, objectToDamage, damageAmount, raycastPosition.forward, hit.point, player, false, true);

				if (applyMeleeAtackForce) {
					Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);
					if (objectToDamageMainRigidbody) {
						bool isVehicle = false;
						bool canApplyForce = false;
						if (applyDamage.isVehicle (objectToDamage)) {
							isVehicle = true;
						}

						if (isVehicle) {
							if (meleeAttackApplyForceToVehicles) {
								canApplyForce = true;
							}
						} else {
							canApplyForce = true;
						}

						if (canApplyForce) {
							Vector3 force = raycastPosition.forward * meleeAttackForceAmount * objectToDamageMainRigidbody.mass;
							if (isVehicle) {
								force = raycastPosition.forward * meleAttackForceToVehicles * (objectToDamageMainRigidbody.mass / 4);
							}
							objectToDamageMainRigidbody.AddForce (force, meleeAttackForceMode);
						}
					}
				}
			}

			if (useMeleeAttackSound) {
				weaponSystemManager.playSound (meleeAttackSurfaceSound);
			}

			if (useMeleeAttackParticles) {
				GameObject newMeleeAttackParticles = (GameObject)Instantiate (meleeAttackParticles, hit.point + hit.normal * 0.1f, Quaternion.LookRotation (hit.normal));
				newMeleeAttackParticles.transform.position = hit.point + hit.normal * 0.1f;
			}
		} else {
			if (useMeleeAttackSound) {
				weaponSystemManager.playSound (meleeAttackAirSound);
			}
		}
	}

	public bool isMeleeAtacking ()
	{
		return meleeAtacking;
	}

	public void setHandsIKTargetValue (float leftValue, float rightValue)
	{
		if (usingRightHandDualWeapon) {
			if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];

				setHandTargetValue (currentIKWeaponsPosition, rightValue);
			}
		} else {
			if (thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];

				setHandTargetValue (currentIKWeaponsPosition, leftValue);
			}
		}

		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
				setHandTargetValue (currentIKWeaponsPosition, leftValue);
			}

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
				setHandTargetValue (currentIKWeaponsPosition, rightValue);
			}
		}
	}

	public void setElbowsIKTargetValue (float leftValue, float rightValue)
	{
		if (usingRightHandDualWeapon) {
			if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];

				setElbowTargetValue (currentIKWeaponsPosition, rightValue);
			}
		} else {
			if (thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];

				setElbowTargetValue (currentIKWeaponsPosition, leftValue);
			}
		}

		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
				setElbowTargetValue (currentIKWeaponsPosition, leftValue);
			}

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
				setElbowTargetValue (currentIKWeaponsPosition, rightValue);
			}
		}
	}

	public void setIKWeight (float leftValue, float rightValue)
	{
		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				currentIKWeaponsPosition.HandIKWeight = rightValue;
			} else {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				currentIKWeaponsPosition.HandIKWeight = leftValue;
			}
		} else {
			for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

				if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
					currentIKWeaponsPosition.HandIKWeight = leftValue;
				}

				if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
					currentIKWeaponsPosition.HandIKWeight = rightValue;
				}
			}
		}
	}

	public void resetElbowIKPositions ()
	{
		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			currentIKWeaponsPosition.elbowInfo.position.SetParent (currentIKWeaponsPosition.elbowInfo.elbowOriginalPosition);
			currentIKWeaponsPosition.elbowInfo.position.localPosition = Vector3.zero;
			currentIKWeaponsPosition.elbowInfo.position.localRotation = Quaternion.identity;
		}
	}

	public void quickDrawWeaponThirdPerson ()
	{
		carrying = true;
		currentKeepPath = thirdPersonWeaponInfo.keepPath;

		weaponTransform.SetParent (weaponSystemManager.getWeaponParent ());

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				currentDrawPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
			} else {
				currentDrawPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
			}
		} else {
			currentDrawPositionTransform = thirdPersonWeaponInfo.walkPosition;
		}

		weaponTransform.localPosition = currentDrawPositionTransform.localPosition;
		weaponTransform.localRotation = currentDrawPositionTransform.localRotation;
		carryingWeapon = carrying;

		if (hideWeaponIfKeptInThirdPerson) {
			enableOrDisableWeaponMesh (true);
		}

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();

		checkDeactivateIKIfNotAiming ();

		resetElbowIKPositions ();
	}

	public void checkDeactivateIKIfNotAiming ()
	{
		if (thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				currentIKWeaponsPosition.HandIKWeight = 0;

				setHandTargetValue (currentIKWeaponsPosition, 0);

				setElbowTargetValue (currentIKWeaponsPosition, 0);

				currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 0;

				if (currentIKWeaponsPosition.usedToDrawWeapon) {
					weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);

					if (usingRightHandDualWeapon) {
						weaponTransform.localPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.weaponPositionInHandForDeactivateIK.localPosition;
						weaponTransform.localRotation = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.weaponPositionInHandForDeactivateIK.localRotation;
					} else {
						weaponTransform.localPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.weaponPositionInHandForDeactivateIK.localPosition;
						weaponTransform.localRotation = thirdPersonWeaponInfo.leftHandDualWeaponInfo.weaponPositionInHandForDeactivateIK.localRotation;
					}
				}
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					currentIKWeaponsPosition.HandIKWeight = 0;

					setHandTargetValue (currentIKWeaponsPosition, 0);

					setElbowTargetValue (currentIKWeaponsPosition, 0);

					currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 0;

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
						weaponTransform.localPosition = thirdPersonWeaponInfo.weaponPositionInHandForDeactivateIK.localPosition;
						weaponTransform.localRotation = thirdPersonWeaponInfo.weaponPositionInHandForDeactivateIK.localRotation;
					}
				}
			}
		}
	}

	public void quickKeepWeaponThirdPerson ()
	{
		stopWeaponMovement ();

		aiming = false;
		carrying = false;
		moving = false;

		if (reloadingWeapon) {
			weaponSystemManager.stopReloadCoroutine ();
			weaponSystemManager.stopReloadAction ();

			reloadingWeapon = false;
		}	

		aimingWeaponInProcess = false;

		weaponTransform.SetParent (weaponSystemManager.weaponSettings.weaponParent);

		weaponTransform.localPosition = thirdPersonWeaponInfo.keepPosition.localPosition;
		weaponTransform.localRotation = thirdPersonWeaponInfo.keepPosition.localRotation;

		setHandsIKTargetValue (0, 0);
		setElbowsIKTargetValue (0, 0);

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
			} else {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
			}

			currentIKWeaponsPosition.handInPositionToAim = false;
			currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;
			currentIKWeaponsPosition.transformFollowByHand.position = currentIKWeaponsPosition.handTransform.position;
		} else {
			for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

				currentIKWeaponsPosition.handInPositionToAim = false;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;
				currentIKWeaponsPosition.transformFollowByHand.position = currentIKWeaponsPosition.handTransform.position;
			}
		}

		carryingWeapon = carrying;

		if (!carrying) {
			if (hideWeaponIfKeptInThirdPerson) {
				enableOrDisableWeaponMesh (false);
			}
		}

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	public void quickDrawWeaponThirdPersonAction ()
	{
		quickDrawWeaponThirdPerson ();
	}

	public void quickKeepWeaponThirdPersonAction ()
	{
		stopWeaponMovement ();

		aiming = false;
		carrying = false;
		moving = false;

		if (reloadingWeapon) {
			weaponSystemManager.stopReloadCoroutine ();
			weaponSystemManager.stopReloadAction ();

			reloadingWeapon = false;
		}	

		aimingWeaponInProcess = false;

		weaponTransform.SetParent (weaponSystemManager.weaponSettings.weaponParent);

		weaponTransform.localPosition = thirdPersonWeaponInfo.keepPosition.localPosition;
		weaponTransform.localRotation = thirdPersonWeaponInfo.keepPosition.localRotation;

		setHandsIKTargetValue (0, 0);
		setElbowsIKTargetValue (0, 0);

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
			} else {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
			}

			currentIKWeaponsPosition.handInPositionToAim = false;
			currentIKWeaponsPosition.HandIKWeight = 0;
		} else {
			for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {

				currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

				currentIKWeaponsPosition.handInPositionToAim = false;
				currentIKWeaponsPosition.HandIKWeight = 0;
			}
		}

		carryingWeapon = carrying;

		if (!carrying) {
			if (hideWeaponIfKeptInThirdPerson) {
				enableOrDisableWeaponMesh (false);
			}
		}

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	public bool isWeaponHandsOnPositionToAim ()
	{
		int handInPositionToAimAmount = 0;
		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			if (thirdPersonWeaponInfo.handsInfo [i].handInPositionToAim) {
				handInPositionToAimAmount++;
			}
		}

		return handInPositionToAimAmount == 2;
	}

	public bool isRightWeaponHandOnPositionToAim ()
	{
		return thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0].handInPositionToAim;
	}

	public bool isLeftWeaponHandOnPositionToAim ()
	{
		return thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0].handInPositionToAim;
	}

	//first person
	public void aimOrDrawWeaponFirstPerson (bool state)
	{
		if (currentWeapon) {
			aiming = state;

			if (aiming) {
				moving = false;
				carryingWeapon = true;
			}

			if (aiming) {
				if (weaponSystemManager.isUsingSight () && firstPersonWeaponInfo.useSightPosition) {
					currentAimPositionTransform = firstPersonWeaponInfo.sightPosition;
				} else {
					currentAimPositionTransform = firstPersonWeaponInfo.aimPosition;
				}
			} else {
				if (crouchingActive) {
					currentAimPositionTransform = firstPersonWeaponInfo.crouchPosition;
				} else {
					currentAimPositionTransform = firstPersonWeaponInfo.walkPosition;
				}
			}

			weaponPositionTarget = currentAimPositionTransform.localPosition;
			weaponRotationTarget = currentAimPositionTransform.localRotation;

			//stop the coroutine to translate the camera and call it again
			stopWeaponMovement ();

			weaponMovement = StartCoroutine (aimOrDrawWeaponFirstPersonCoroutine ());
			setLastTimeMoved ();

			disableOtherWeaponsStates ();

			setPlayerControllerMovementValues ();

			if (aiming) {
				resetSwayValue ();
			}
		}
	}

	IEnumerator aimOrDrawWeaponFirstPersonCoroutine ()
	{
		//print ("aimOrDrawWeaponFirstPersonCoroutine "+aiming);
		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * firstPersonWeaponInfo.aimMovementSpeed;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}
	}

	public void drawOrKeepWeaponFirstPerson (bool state)
	{
		if (!carrying && state && !moving) {
			resetWeaponPositionInFirstPerson ();
		}

		carrying = state;
		if (!carrying) {
			aiming = false;
		}

		if (carrying) {
			crouchingActive = weaponsManager.isPlayerCrouching ();
		}

		//stop the coroutine to translate the camera and call it again
		stopWeaponMovement ();

		weaponMovement = StartCoroutine (drawOrKeepWeaponFirstPersonCoroutine ());
		setLastTimeMoved ();

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	IEnumerator drawOrKeepWeaponFirstPersonCoroutine ()
	{
		//print ("drawOrKeepWeaponFirstPersonCoroutine "+carrying);
		Vector3 targetPosition = Vector3.zero;
		Quaternion targetRotation = Quaternion.identity;
		Vector3 worldTargetPosition = Vector3.zero;

		moving = true;

		if (carrying) {
			enableOrDisableWeaponMesh (true);

			enableOrDisableFirstPersonArms (true);

			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					if (crouchingActive) {
						currentDrawPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
					} else {
						currentDrawPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
					}
				} else {
					if (crouchingActive) {
						currentDrawPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
					} else {
						currentDrawPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
					}
				}
			} else {
				if (crouchingActive) {
					currentDrawPositionTransform = firstPersonWeaponInfo.crouchPosition;
				} else {
					currentDrawPositionTransform = firstPersonWeaponInfo.walkPosition;
				}
			}

			weaponSystemManager.changeHUDPosition (false);
	
		} else {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentDrawPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition;
				} else {
					currentDrawPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition;
				}
			} else {
				currentDrawPositionTransform = firstPersonWeaponInfo.keepPosition;
			}
		}

		targetPosition = currentDrawPositionTransform.localPosition;
		targetRotation = currentDrawPositionTransform.localRotation;
		worldTargetPosition = currentDrawPositionTransform.position;

		float dist = GKC_Utils.distance (weaponTransform.position, worldTargetPosition);
		float duration = dist / firstPersonWeaponInfo.drawWeaponMovementSpeed;
		float t = 0;

		while (t < 1 && (weaponTransform.localPosition != targetPosition && weaponTransform.localRotation != targetRotation)) {
			t += Time.deltaTime / duration;
			weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, targetPosition, t);
			weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, targetRotation, t);
			yield return null;
		}
	
		if (!aiming && !carrying) {
			weaponTransform.SetParent (weaponSystemManager.getWeaponParent ());

			resetWeaponPositionInFirstPerson ();

			enableOrDisableWeaponMesh (false);

			enableOrDisableFirstPersonArms (false);
		}

		moving = false;
		carryingWeapon = carrying;
	}

	public void reloadWeaponFirstPerson ()
	{
		if (!firstPersonWeaponInfo.useReloadMovement) {
			return;
		}

		if (aiming) {
			weaponsManager.aimCurrentWeapon (false);
			weaponsManager.disableAimModeInputPressedState ();
		}

		//stop the coroutine to translate the camera and call it again
		stopWeaponMovement ();

		weaponMovement = StartCoroutine (reloadWeaponFirstPersonCoroutine ());

		resetOtherWeaponsStates ();

		disableOtherWeaponsStates ();

		setPlayerControllerMovementValues ();
	}

	IEnumerator reloadWeaponFirstPersonCoroutine ()
	{
		moving = true;

		reloadingWeapon = true;

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				spline = firstPersonWeaponInfo.rightHandDualWeaopnInfo.reloadSpline;
				bezierDuration = firstPersonWeaponInfo.rightHandDualWeaopnInfo.reloadDuration;
				lookDirectionSpeed = firstPersonWeaponInfo.rightHandDualWeaopnInfo.reloadLookDirectionSpeed;
			} else {
				spline = firstPersonWeaponInfo.leftHandDualWeaponInfo.reloadSpline;
				bezierDuration = firstPersonWeaponInfo.leftHandDualWeaponInfo.reloadDuration;
				lookDirectionSpeed = firstPersonWeaponInfo.leftHandDualWeaponInfo.reloadLookDirectionSpeed;
			}
		} else {
			spline = firstPersonWeaponInfo.reloadSpline;
			bezierDuration = firstPersonWeaponInfo.reloadDuration;
			lookDirectionSpeed = firstPersonWeaponInfo.reloadLookDirectionSpeed;
		}

		bool targetReached = false;

		float angleDifference = 0;

		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				currentReloadPosition = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
			} else {
				currentReloadPosition = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
			}
		} else {
			currentReloadPosition = firstPersonWeaponInfo.walkPosition;
		}

		float dist = GKC_Utils.distance (weaponTransform.position, currentReloadPosition.position);

		float duration = dist;

		float t = 0;

		Vector3 pos = currentReloadPosition.localPosition;
		Quaternion rot = currentReloadPosition.localRotation;

		float movementTimer = 0;

		if (dist > 0.01f) {
			while (!targetReached) {
				t += Time.deltaTime / duration; 
				weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, pos, t);
				weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);

				angleDifference = Quaternion.Angle (weaponTransform.localRotation, rot);

				movementTimer += Time.deltaTime;

				if ((GKC_Utils.distance (weaponTransform.localPosition, pos) < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}
				yield return null;
			}
		}

		float progress = 0;
		float progressTarget = 1;

		targetReached = false;

		while (!targetReached) {
			progress += Time.deltaTime / bezierDuration;

			Vector3 position = spline.GetPoint (progress);
			weaponTransform.position = position;

			Quaternion targetRotation = Quaternion.Euler (spline.GetLookDirection (progress));
			weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, targetRotation, Time.deltaTime * lookDirectionSpeed);

			if (progress > progressTarget) {
				targetReached = true;
			}

			yield return null;
		}

		movementTimer = 0;

		duration = 0.2f;

		while (!targetReached) {
			t += Time.deltaTime / duration; 
			weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, rot, t);

			angleDifference = Quaternion.Angle (weaponTransform.localRotation, rot);

			movementTimer += Time.deltaTime;

			if (angleDifference < 0.2f || movementTimer > 1) {
				targetReached = true;
			}
			yield return null;
		}

		moving = false;

		reloadingWeapon = false;
	}

	public bool isReloadingWeapon ()
	{
		return reloadingWeapon;
	}

	public void quickKeepWeaponFirstPerson ()
	{
		carrying = false;
		aiming = false;
		moving = false;

		if (reloadingWeapon) {
			weaponSystemManager.stopReloadCoroutine ();
			weaponSystemManager.stopReloadAction ();

			reloadingWeapon = false;
		}	

		aimingWeaponInProcess = false;

		resetWeaponPositionInFirstPerson ();

		enableOrDisableWeaponMesh (false);
		carryingWeapon = carrying;

		stopWeaponMovement ();

		setPlayerControllerMovementValues ();
	}

	public void resetWeaponPositionInFirstPerson ()
	{
		if (isUsingDualWeapon () || weaponConfiguredAsDualWepaon) {
			if (usingRightHandDualWeapon) {
				currentDrawPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition;
			} else {
				currentDrawPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition;
			}
		} else {
			currentDrawPositionTransform = firstPersonWeaponInfo.keepPosition;
		}

		weaponTransform.localPosition = currentDrawPositionTransform.localPosition;
		weaponTransform.localRotation = currentDrawPositionTransform.localRotation;
	}

	public void setWeaponHasRecoilState (bool state)
	{
		thirdPersonWeaponInfo.weaponHasRecoil = state;
		firstPersonWeaponInfo.weaponHasRecoil = state;
	}

	public void startRecoilFromExternalFunction ()
	{
		startRecoil (!weaponsManager.isFirstPersonActive ());
	}

	//Recoil functions
	public void startRecoil (bool isThirdPersonView)
	{
		if ((isThirdPersonView && thirdPersonWeaponInfo.weaponHasRecoil) || (!isThirdPersonView && firstPersonWeaponInfo.weaponHasRecoil)) {

			disableOtherWeaponsStates ();

			stopWeaponMovement ();

			weaponMovement = StartCoroutine (recoilMovementBack (isThirdPersonView));
		}
	}

	IEnumerator recoilMovementBack (bool isThirdPersonView)
	{
		weaponOnRecoil = true;

		recoilExtraPosition = Vector3.zero;
		recoilRandomPosition = Vector3.zero;
		recoilExtraRotatation = Vector3.zero;
		recoilRandomRotation = Vector3.zero;

		if (isThirdPersonView) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentAimRecoilPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.aimRecoilPosition;
				} else {
					currentAimRecoilPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.aimRecoilPosition;
				}
			} else {
				currentAimRecoilPositionTransform = thirdPersonWeaponInfo.aimRecoilPosition;
			}

			weaponPositionTarget = currentAimRecoilPositionTransform.localPosition;
			weaponRotationTarget = currentAimRecoilPositionTransform.localRotation;

			if (thirdPersonWeaponInfo.useExtraRandomRecoil) {
				if (thirdPersonWeaponInfo.useExtraRandomRecoilPosition) {
					recoilExtraPosition = thirdPersonWeaponInfo.extraRandomRecoilPosition;
					recoilRandomPosition = new Vector3 (Random.Range (-recoilExtraPosition.x, recoilExtraPosition.x), 
						Random.Range (0, recoilExtraPosition.y), Random.Range (-recoilExtraPosition.z, 0));
					weaponPositionTarget += recoilRandomPosition;
				}

				if (thirdPersonWeaponInfo.useExtraRandomRecoilRotation) {
					recoilExtraRotatation = thirdPersonWeaponInfo.extraRandomRecoilRotation;
					recoilRandomRotation = new Vector3 (Random.Range (-recoilExtraRotatation.x, 0), 
						Random.Range (-recoilExtraRotatation.y, recoilExtraRotatation.y), Random.Range (-recoilExtraRotatation.z, recoilExtraRotatation.z));
					weaponRotationTarget = Quaternion.Euler (weaponRotationTarget.eulerAngles + recoilRandomRotation);
				}
			}
			currentRecoilSpeed = thirdPersonWeaponInfo.recoilSpeed;
		} else {
			if (aiming) {
				if (weaponSystemManager.isUsingSight () && firstPersonWeaponInfo.useSightPosition) {
					currentAimRecoilPositionTransform = firstPersonWeaponInfo.sightRecoilPosition;
				} else {
					currentAimRecoilPositionTransform = firstPersonWeaponInfo.aimRecoilPosition;
				}
			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						if (crouchingActive) {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchRecoilPosition;
						} else {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkRecoilPosition;
						}
					} else {
						if (crouchingActive) {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchRecoilPosition;
						} else {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkRecoilPosition;
						}
					}
				} else {
					if (crouchingActive) {
						currentAimRecoilPositionTransform = firstPersonWeaponInfo.crouchRecoilPosition;
					} else {
						currentAimRecoilPositionTransform = firstPersonWeaponInfo.walkRecoilPosition;
					}
				}
			}

			weaponPositionTarget = currentAimRecoilPositionTransform.localPosition;
			weaponRotationTarget = currentAimRecoilPositionTransform.localRotation;

			if (firstPersonWeaponInfo.useExtraRandomRecoil) {
				if (firstPersonWeaponInfo.useExtraRandomRecoilPosition) {
					recoilExtraPosition = firstPersonWeaponInfo.extraRandomRecoilPosition;
					recoilRandomPosition = new Vector3 (Random.Range (-recoilExtraPosition.x, recoilExtraPosition.x), 
						Random.Range (0, recoilExtraPosition.y), Random.Range (-recoilExtraPosition.z, 0));

					if (aiming) {
						recoilRandomPosition *= currentSwayInfo.swayPositionPercentageAiming;
					}

					weaponPositionTarget += recoilRandomPosition;
				}

				if (firstPersonWeaponInfo.useExtraRandomRecoilRotation) {
					recoilExtraRotatation = firstPersonWeaponInfo.extraRandomRecoilRotation;
					recoilRandomRotation = new Vector3 (Random.Range (-recoilExtraRotatation.x, 0), 
						Random.Range (-recoilExtraRotatation.y, recoilExtraRotatation.y), Random.Range (-recoilExtraRotatation.z, recoilExtraRotatation.z));
				}

				if (aiming) {
					recoilRandomRotation *= currentSwayInfo.swayRotationPercentageAiming;
				}

				weaponRotationTarget = Quaternion.Euler (weaponRotationTarget.eulerAngles + recoilRandomRotation);
			}
			currentRecoilSpeed = firstPersonWeaponInfo.recoilSpeed;
		}

		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * currentRecoilSpeed * 2;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}
		endRecoil (isThirdPersonView);
	}

	public void endRecoil (bool isThirdPersonView)
	{
		stopWeaponMovement ();

		weaponMovement = StartCoroutine (recoilMovementForward (isThirdPersonView));
	}

	IEnumerator recoilMovementForward (bool isThirdPersonView)
	{
		weaponOnRecoil = true;
		if (isThirdPersonView) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentAimRecoilPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.aimPosition;
				} else {
					currentAimRecoilPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.aimPosition;
				}
			} else {
				currentAimRecoilPositionTransform = thirdPersonWeaponInfo.aimPosition;
			}

			currentRecoilSpeed = thirdPersonWeaponInfo.endRecoilSpeed;
		} else {
			if (aiming) {
				if (weaponSystemManager.isUsingSight () && firstPersonWeaponInfo.useSightPosition) {
					currentAimRecoilPositionTransform = firstPersonWeaponInfo.sightPosition;
				} else {
					currentAimRecoilPositionTransform = firstPersonWeaponInfo.aimPosition;
				}
			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						if (crouchingActive) {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
						} else {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						}
					} else {
						if (crouchingActive) {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
						} else {
							currentAimRecoilPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					}
				} else {
					if (crouchingActive) {
						currentAimRecoilPositionTransform = firstPersonWeaponInfo.crouchPosition;
					} else {
						currentAimRecoilPositionTransform = firstPersonWeaponInfo.walkPosition;
					}
				}
			}

			currentRecoilSpeed = firstPersonWeaponInfo.endRecoilSpeed;
		}

		weaponPositionTarget = currentAimRecoilPositionTransform.localPosition;
		weaponRotationTarget = currentAimRecoilPositionTransform.localRotation;

		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * currentRecoilSpeed * 2;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}

		weaponOnRecoil = false;
	}

	public void disableOtherWeaponsStates ()
	{
		surfaceDetected = false;
		weaponInRunPosition = false;
		playerRunning = false;
		setCurrentSwayInfo (true);
		cursorHidden = false;
		meleeAtacking = false;
	}

	public void resetOtherWeaponsStates ()
	{
		if (jumpingOnProcess) {
			weaponsManager.disableWeaponJumpState ();
		}

		if (playerRunning) {
			weaponsManager.resetPlayerRunningState ();
		}

		jumpingOnProcess = false;

		if (cursorHidden) {
			weaponsManager.enableOrDisableGeneralWeaponCursor (true);
		}
	}

	public void stopWeaponMovement ()
	{
		if (weaponMovement != null) {
			StopCoroutine (weaponMovement);
		}

		if (resetingWeaponSwayValue) {
			stopResetSwayValue ();
		}

		weaponOnRecoil = false;
	}

	public void currentWeaponSway (float mouseX, float mouseY, float vertical, float horizontal, bool running, bool shooting, 
	                               bool onGround, bool externalShakingActive, bool usingDevice, bool isThirdPersonView)
	{
		if (currentlyUsingSway && currentSwayInfo.useSway && !resetingWeaponSwayValue && (!isThirdPersonView || !thirdPersonWeaponInfo.deactivateIKIfNotAiming) && carrying) {

			if (!isUsingDualWeapon () || !isThirdPersonView) {

				if (useWeaponIdle) {
					if (horizontal == 0 && vertical == 0 && mouseX == 0 && mouseY == 0 && onGround) {
						playerMoving = false;
					} else {
						playerMoving = true;
						idleActive = false;
						setLastTimeMoved ();
					}

					if (!playerMoving) {
						if (externalShakingActive) {
							setLastTimeMoved ();
						}

						if (Time.time > lastTimeMoved + timeToActiveWeaponIdle && !moving && !usingDevice) { 
							idleActive = true;
						} else {
							idleActive = false;
						}
					}
				}

				if (isThirdPersonView) {
					mouseX = 0;
					mouseY = 0;
					horizontal = 0;
				}

				//assign values for walk or running multiplier

				if (running && !aiming) {
					swayPositionRunningMultiplier = currentSwayInfo.swayPositionRunningMultiplier;
					swayRotationRunningMultiplier = currentSwayInfo.swayRotationRunningMultiplier;
					bobPositionRunningMultiplier = currentSwayInfo.bobPositionRunningMultiplier;
					bobRotationRunningMultiplier = currentSwayInfo.bobRotationRunningMultiplier;
				} else {
					swayPositionRunningMultiplier = 1;
					swayRotationRunningMultiplier = 1;
					bobPositionRunningMultiplier = 1;
					bobRotationRunningMultiplier = 1;
				}

				usingPositionSway = false;
				usingRotationSway = false;

				//set the current rotation and positioon smooth value
				if (playerMoving) {
					currentSwayRotationSmooth = currentSwayInfo.swayRotationSmooth;
					currentSwayPositionSmooth = currentSwayInfo.swayPositionSmooth;
				} else {
					currentSwayRotationSmooth = currentSwayInfo.resetSwayRotationSmooth;
					currentSwayPositionSmooth = currentSwayInfo.resetSwayPositionSmooth;
				}

				swayPosition = Vector3.zero;
				swayRotation = Vector3.zero;

				//set the position sway
				if (currentSwayInfo.usePositionSway) {

					usingPositionSway = true;

					if (Mathf.Abs (mouseX) > currentSwayInfo.minMouseAmountForSway) {
						swayPosition.x = -mouseX * currentSwayInfo.swayPositionVertical;
					} else {
						swayPosition.x = 0;
					}

					if (Mathf.Abs (mouseY) > currentSwayInfo.minMouseAmountForSway) {
						swayPosition.y = -mouseY * currentSwayInfo.swayPositionHorizontal;
					} else {
						swayPosition.y = 0;
					}

					swayPosition *= swayPositionRunningMultiplier;

					if (swayPosition.x > currentSwayInfo.swayPositionMaxAmount) {
						swayPosition.x = currentSwayInfo.swayPositionMaxAmount;
					}

					if (swayPosition.x < -currentSwayInfo.swayPositionMaxAmount) {
						swayPosition.x = -currentSwayInfo.swayPositionMaxAmount;
					}

					if (swayPosition.y > currentSwayInfo.swayPositionMaxAmount) {
						swayPosition.y = currentSwayInfo.swayPositionMaxAmount;
					}

					if (swayPosition.y < -currentSwayInfo.swayPositionMaxAmount) {
						swayPosition.y = -currentSwayInfo.swayPositionMaxAmount;
					}
				}

				//set the position bob
				if (currentSwayInfo.useBobPosition) {
					usingPositionSway = true;
					if ((Mathf.Abs (horizontal) > 0 || Mathf.Abs (vertical) > 0) && onGround) {
						mainSwayTargetPosition = getSwayPosition (currentSwayInfo.bobPositionSpeed, currentSwayInfo.bobPositionAmount, 1);
			
						if (aiming) {
							mainSwayTargetPosition *= currentSwayInfo.bobPositionPercentageAiming;
						}

						mainSwayTargetPosition *= bobPositionRunningMultiplier;
					} else {
						mainSwayTargetPosition = Vector3.zero;
					}
				}

				//apply total position sway
				if (usingPositionSway) {
					swayPosition += mainSwayTargetPosition;

					if (aiming) {
						swayPosition *= currentSwayInfo.swayPositionPercentageAiming;
					} else {
						swayExtraPosition = currentSwayInfo.movingExtraPosition;

						if (vertical > 0) {
							swayPosition += Vector3.forward * swayExtraPosition.z;
						} 

						if (vertical < 0) {
							swayPosition -= Vector3.forward * swayExtraPosition.z;
						}

						if (horizontal > 0) {
							swayPosition += Vector3.right * swayExtraPosition.x;
						} 

						if (horizontal < 0) {
							swayPosition -= Vector3.right * swayExtraPosition.x;
						}
					}

					if (!moving && idleActive) {
						swayPosition = getSwayPosition (idleSpeed, idlePositionAmount, 1);
					}

					if (currentSwayInfo.useSwayPositionClamp) {
						swayPosition.x = Mathf.Clamp (swayPosition.x, currentSwayInfo.swayPositionHorizontalClamp.x, currentSwayInfo.swayPositionHorizontalClamp.y);
						swayPosition.y = Mathf.Clamp (swayPosition.y, currentSwayInfo.swayPositionVerticalClamp.x, currentSwayInfo.swayPositionVerticalClamp.y);
					}

					mainWeaponMeshTransform.localPosition = Vector3.Lerp (mainWeaponMeshTransform.localPosition, swayPosition, Time.deltaTime * currentSwayPositionSmooth);
				}

				//set the rotation sway
				if (currentSwayInfo.useRotationSway) {
				
					usingRotationSway = true;

					if (Mathf.Abs (mouseX) > currentSwayInfo.minMouseAmountForSway) {
						swayRotation.z = mouseX * currentSwayInfo.swayRotationHorizontal;
					} else {
						swayRotation.z = 0;
					}

					if (Mathf.Abs (mouseY) > currentSwayInfo.minMouseAmountForSway) {
						swayRotation.x = mouseY * currentSwayInfo.swayRotationVertical;
					} else {
						swayRotation.x = 0;
					}

					swayRotation *= swayRotationRunningMultiplier;
				}

				//set the rotation bob
				if (currentSwayInfo.useBobRotation) {
					usingRotationSway = true;
					if (!shooting) {
						swayTilt.x = vertical * currentSwayInfo.bobRotationVertical;
						swayTilt.z = horizontal * currentSwayInfo.bobRotationHorizontal;
						swayTilt *= bobRotationRunningMultiplier;

						if (aiming) {
							swayTilt *= currentSwayInfo.bobRotationPercentageAiming;
						}
					} else {
						swayTilt = Vector3.zero;
					}
				}

				//apply total rotation sway
				if (usingRotationSway) {
					swayRotation += swayTilt;
					if (aiming) {
						swayRotation *= currentSwayInfo.swayRotationPercentageAiming;
					}

					if (!moving && idleActive) {
						swayRotation = getSwayPosition (idleSpeed, idleRotationAmount, 1);
					}

					if (currentSwayInfo.useSwayRotationClamp) {
						swayRotation.x = Mathf.Clamp (swayRotation.x, currentSwayInfo.swayRotationClampX.x, currentSwayInfo.swayRotationClampX.y);
						swayRotation.z = Mathf.Clamp (swayRotation.z, currentSwayInfo.swayRotationClampZ.x, currentSwayInfo.swayRotationClampZ.y);
					}

					swayTargetRotation = Quaternion.Euler (swayRotation);
					mainWeaponMeshTransform.localRotation = Quaternion.Slerp (mainWeaponMeshTransform.localRotation, swayTargetRotation, Time.deltaTime * currentSwayRotationSmooth);
				}
			} else {
				mainWeaponMeshTransform.localPosition = 
					Vector3.Lerp (mainWeaponMeshTransform.localPosition, Vector3.zero, Time.deltaTime * currentSwayInfo.resetSwayRotationSmooth);
				mainWeaponMeshTransform.localRotation = 
					Quaternion.Slerp (mainWeaponMeshTransform.localRotation, Quaternion.identity, Time.deltaTime * currentSwayInfo.resetSwayRotationSmooth);
			}
		}
	}

	public Vector3 getSwayPosition (Vector3 speed, Vector3 amount, float multiplier)
	{
		currentSwayPosition = Vector3.zero;
		currentSwayPosition.x = Mathf.Sin (Time.time * speed.x) * amount.x * multiplier;
		currentSwayPosition.y = Mathf.Sin (Time.time * speed.y) * amount.y * multiplier;
		currentSwayPosition.z = Mathf.Sin (Time.time * speed.z) * amount.z * multiplier;
		return currentSwayPosition;
	}

	public void resetSwayValue ()
	{
		stopResetSwayValue ();

		swayValueCoroutine = StartCoroutine (resetSwayValueCoroutine ());
	}

	public void stopResetSwayValue ()
	{
		if (swayValueCoroutine != null) {
			StopCoroutine (swayValueCoroutine);
		}

		resetingWeaponSwayValue = false;
	}

	IEnumerator resetSwayValueCoroutine ()
	{
		weaponSwayTransform = weaponSystemManager.weaponSettings.weaponMesh.transform;

		resetingWeaponSwayValue = true;

		Vector3 targetPosition = Vector3.zero;
		Vector3 worldTargetPosition = weaponGameObject.transform.position;

		Quaternion targetRotation = Quaternion.identity;

		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		float movementSpeed = 0;

		if (isFirstPersonActive) {
			movementSpeed = firstPersonWeaponInfo.aimMovementSpeed;
		} else {
			movementSpeed = thirdPersonWeaponInfo.aimMovementSpeed;
		}

		float dist = GKC_Utils.distance (weaponSwayTransform.position, worldTargetPosition);
		float duration = dist / movementSpeed;
		float t = 0;

		while (t < 1 && weaponSwayTransform.localPosition != targetPosition && weaponSwayTransform.localRotation != targetRotation) {
			t += Time.deltaTime / duration;
			weaponSwayTransform.localPosition = Vector3.Slerp (weaponSwayTransform.localPosition, targetPosition, t);
			weaponSwayTransform.localRotation = Quaternion.Slerp (weaponSwayTransform.localRotation, targetRotation, t);
			yield return null;
		}

		//print (weaponSwayTransform.localPosition + " " + weaponSwayTransform.localRotation);

		resetingWeaponSwayValue = false;
	}

	public void setLastTimeMoved ()
	{
		lastTimeMoved = Time.time;
	}

	public void enableOrDisableFirstPersonArms (bool state)
	{
		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				if (firstPersonWeaponInfo.rightHandDualWeaopnInfo.firstPersonHandMesh) {
					firstPersonWeaponInfo.rightHandDualWeaopnInfo.firstPersonHandMesh.SetActive (state);
				}
			} else {
				if (firstPersonWeaponInfo.leftHandDualWeaponInfo.firstPersonHandMesh) {
					firstPersonWeaponInfo.leftHandDualWeaponInfo.firstPersonHandMesh.SetActive (state);
				}
			}

			firstPersonArms.SetActive (false);
		} else {
			if (firstPersonArms) {
				firstPersonArms.SetActive (state);
			}

			if (firstPersonWeaponInfo.rightHandDualWeaopnInfo.firstPersonHandMesh) {
				firstPersonWeaponInfo.rightHandDualWeaopnInfo.firstPersonHandMesh.SetActive (false);
			}

			if (firstPersonWeaponInfo.leftHandDualWeaponInfo.firstPersonHandMesh) {
				firstPersonWeaponInfo.leftHandDualWeaponInfo.firstPersonHandMesh.SetActive (false);
			}
		}
	}

	public void enableOrDisableWeaponMesh (bool state)
	{
		//print ("mesh state "+gameObject.name + " " + state);
		weaponSystemManager.weaponSettings.weaponMesh.SetActive (state);
	}

	public void setHandTransform (Transform rightHand, Transform leftHand)
	{
		for (int j = 0; j < thirdPersonWeaponInfo.handsInfo.Count; j++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [j];

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
				currentIKWeaponsPosition.handTransform = rightHand;

				if (thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
					thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0].handTransform = rightHand;
				}
			}

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
				currentIKWeaponsPosition.handTransform = leftHand;

				if (thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo.Count > 0) {
					thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0].handTransform = leftHand;
				}
			}
		}

		updateComponent ();
	}

	public bool isPlayerCarringWeapon ()
	{
		return carryingWeapon;
	}

	public void setShotCameraNoise ()
	{
		if (useShotCameraNoise) {
			float verticalAmount = Random.Range (verticalShotCameraNoiseAmount.x, verticalShotCameraNoiseAmount.y);
			float horizontalAmount = Random.Range (horizontalShotCameraNoiseAmount.x, horizontalShotCameraNoiseAmount.y);

			weaponsManager.setShotCameraNoise (new Vector2 (horizontalAmount, verticalAmount));
		}
	}

	public bool checkIfIKHandsIsActive ()
	{
		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		if (isFirstPersonActive || (!isFirstPersonActive && !thirdPersonWeaponInfo.deactivateIKIfNotAiming) ||
		    (!isFirstPersonActive && thirdPersonWeaponInfo.deactivateIKIfNotAiming && editingAttachments)) {
			return true;
		}

		return false;
	}

	public void enableOrDisableEditAttachment (bool state)
	{
		//stop the coroutines and start them again
		stopWeaponMovement ();

		weaponMovement = StartCoroutine (enableOrDisableEditAttachmentCoroutine (state));

		if (checkIfIKHandsIsActive () && (weaponsManager.isFirstPersonActive () || !thirdPersonWeaponInfo.deactivateIKIfNotAiming)) {
			moveHandForAttachment ();
		}

		setLastTimeMoved ();
	}

	IEnumerator enableOrDisableEditAttachmentCoroutine (bool state)
	{
		editingAttachments = state;

		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		if (!isFirstPersonActive && thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (editingAttachments) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
					}

					setHandDeactivateIKStateToDisable ();

					weaponTransform.SetParent (transform);
				} else {
					for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
						currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

						if (currentIKWeaponsPosition.usedToDrawWeapon) {
							setHandDeactivateIKStateToDisable ();

							weaponTransform.SetParent (transform);
						}
					}
				}
			}
		}

		if (checkIfIKHandsIsActive ()) {
			Vector3 targetPosition = Vector3.zero;
			Quaternion targetRotation = Quaternion.identity;
			Vector3 worldTargetPosition = Vector3.zero;
			moving = true;

			float movementSpeed = 0;

			if (isFirstPersonActive) {
				movementSpeed = firstPersonWeaponInfo.editAttachmentMovementSpeed;
			} else {
				movementSpeed = thirdPersonWeaponInfo.editAttachmentMovementSpeed;
			}

			if (editingAttachments) {
				if (isFirstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.editAttachmentPosition;
						} else {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.editAttachmentPosition;
						}
					} else {
						currentEditAttachmentPositionTransform = firstPersonWeaponInfo.editAttachmentPosition;
					}
				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.editAttachmentPosition;
						} else {
							currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.editAttachmentPosition;
						}
					} else {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.editAttachmentPosition;
					}
				}
			} else {
				if (isFirstPersonActive) {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							if (crouchingActive) {
								currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
							} else {
								currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
							}
						} else {
							if (crouchingActive) {
								currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
							} else {
								currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
							}
						}
					} else {
						if (crouchingActive) {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.crouchPosition;
						} else {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.walkPosition;
						}
					}

				} else {
					if (isUsingDualWeapon ()) {
						if (usingRightHandDualWeapon) {
							currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						} else {
							currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					} else {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.walkPosition;
					}
				}
			}

			targetPosition = currentEditAttachmentPositionTransform.localPosition;
			targetRotation = currentEditAttachmentPositionTransform.localRotation;
			worldTargetPosition = currentEditAttachmentPositionTransform.position;

			float dist = GKC_Utils.distance (weaponTransform.position, worldTargetPosition);
			float duration = dist / movementSpeed;
			float t = 0;

			while (t < 1 && weaponTransform.localPosition != targetPosition && weaponTransform.localRotation != targetRotation) {
				t += Time.deltaTime / duration;
				weaponTransform.localPosition = Vector3.Slerp (weaponTransform.localPosition, targetPosition, t);
				weaponTransform.localRotation = Quaternion.Slerp (weaponTransform.localRotation, targetRotation, t);
				yield return null;
			}

			Transform weaponMeshTransform = weaponSystemManager.weaponSettings.weaponMesh.transform;
			Vector3 weaponMeshTargetPosition = Vector3.zero;
			Quaternion weaponMeshTargetRotation = Quaternion.identity;

			dist = GKC_Utils.distance (weaponMeshTransform.position, worldTargetPosition);
			duration = dist / movementSpeed;
			t = 0;
			while (t < 1 && weaponMeshTransform.localPosition != weaponMeshTargetPosition && weaponMeshTransform.localRotation != weaponMeshTargetRotation) {
				t += Time.deltaTime / duration;
				weaponMeshTransform.localPosition = Vector3.Slerp (weaponMeshTransform.localPosition, weaponMeshTargetPosition, t);
				weaponMeshTransform.localRotation = Quaternion.Slerp (weaponMeshTransform.localRotation, weaponMeshTargetRotation, t);
				yield return null;
			}
		
			moving = false;
		}

		if (!isFirstPersonActive && !editingAttachments && thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			if (isUsingDualWeapon ()) {
				if (usingRightHandDualWeapon) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInfo [0];
				} else {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInfo [0];
				}

				currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
				currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

				weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
			} else {
				for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
					currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

					if (currentIKWeaponsPosition.usedToDrawWeapon) {
						currentIKWeaponsPosition.waypointFollower.position = currentIKWeaponsPosition.position.position;
						currentIKWeaponsPosition.waypointFollower.rotation = currentIKWeaponsPosition.position.rotation;
						currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.waypointFollower;

						weaponTransform.SetParent (currentIKWeaponsPosition.handTransform);
					}
				}
			}

			setHandsIKTargetValue (0, 0);
			setElbowsIKTargetValue (0, 0);
		}
	}

	public void moveHandForAttachment ()
	{
		if (!firstPersonWeaponInfo.leftHandMesh || !thirdPersonWeaponInfo.leftHandMesh) {
			return;
		}

		if (handAttachmentCoroutine != null) {
			StopCoroutine (handAttachmentCoroutine);
		}
		handAttachmentCoroutine = StartCoroutine (moveHandForAttachnmentCoroutine ());
	}

	IEnumerator moveHandForAttachnmentCoroutine ()
	{
		Vector3 handTargetPosition = Vector3.zero;
		Vector3 handWorldTargetPosition = Vector3.zero;
		Transform handToMove = thirdPersonWeaponInfo.leftHandMesh.transform;

		Quaternion handTargetRotation = Quaternion.identity;

		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		float movementSpeed = 0;

		if (isFirstPersonActive) {
			handToMove = firstPersonWeaponInfo.leftHandMesh.transform;
			movementSpeed = firstPersonWeaponInfo.editAttachmentHandSpeed;
		} else {
			movementSpeed = thirdPersonWeaponInfo.editAttachmentHandSpeed;
		}

		if (editingAttachments) {
			if (isFirstPersonActive) {
				handTargetPosition = firstPersonWeaponInfo.leftHandEditPosition.localPosition;
				handWorldTargetPosition = firstPersonWeaponInfo.leftHandEditPosition.position;
			} else {
				handWorldTargetPosition = thirdPersonWeaponInfo.leftHandEditPosition.position;
				setElbowsIKTargetValue (0, 0);
				weaponsManager.getIKSystem ().setElbowsIKTargetValue (0, 0);
				handToMove.SetParent (thirdPersonWeaponInfo.leftHandEditPosition);
			}
		} else {
			if (isFirstPersonActive) {
				if (firstPersonWeaponInfo.usingSecondPositionForHand) {
					handTargetPosition = firstPersonWeaponInfo.secondPositionForHand.localPosition;
					handWorldTargetPosition = firstPersonWeaponInfo.secondPositionForHand.position;
				} else {
					handWorldTargetPosition = firstPersonWeaponInfo.leftHandEditPosition.parent.position;
				}
			} else {
				if (thirdPersonWeaponInfo.usingSecondPositionForHand) {
					handTargetPosition = thirdPersonWeaponInfo.secondPositionForHand.localPosition;
					handWorldTargetPosition = thirdPersonWeaponInfo.secondPositionForHand.position;
				} else {
					handWorldTargetPosition = thirdPersonWeaponInfo.leftHandParent.position;
				}

				setElbowsIKTargetValue (1, 1);

				weaponsManager.getIKSystem ().setElbowsIKTargetValue (1, 1);

				handToMove.SetParent (thirdPersonWeaponInfo.leftHandParent);
			}
		}

		float dist = GKC_Utils.distance (handToMove.position, handWorldTargetPosition);
		float duration = dist / movementSpeed;
		float t = 0;

		if (isFirstPersonActive) {
			while (t < 1 && handToMove.localPosition != handTargetPosition) {
				t += Time.deltaTime / duration;
				handToMove.localPosition = Vector3.Slerp (handToMove.localPosition, handTargetPosition, t);
				yield return null;
			}
		} else {
			while (t < 1 && handToMove.localPosition != handTargetPosition && handToMove.localRotation != handTargetRotation) {
				t += Time.deltaTime / duration;
				handToMove.localPosition = Vector3.Slerp (handToMove.localPosition, handTargetPosition, t);
				handToMove.localRotation = Quaternion.Slerp (handToMove.localRotation, handTargetRotation, t);
				yield return null;
			}
		}
	}

	public void quickEnableOrDisableEditAttachment (bool state)
	{
		editingAttachments = state;

		Vector3 targetPosition = Vector3.zero;
		Quaternion targetRotation = Quaternion.identity;

		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		if (editingAttachments) {
			if (isFirstPersonActive) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.editAttachmentPosition;
					} else {
						currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.editAttachmentPosition;
					}
				} else {
					currentEditAttachmentPositionTransform = firstPersonWeaponInfo.editAttachmentPosition;
				}

			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.editAttachmentPosition;
					} else {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.editAttachmentPosition;
					}
				} else {
					currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.editAttachmentPosition;
				}
			}
		} else {
			if (isFirstPersonActive) {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						if (crouchingActive) {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.crouchPosition;
						} else {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
						}
					} else {
						if (crouchingActive) {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.crouchPosition;
						} else {
							currentEditAttachmentPositionTransform = firstPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
						}
					}
				} else {
					if (crouchingActive) {
						currentEditAttachmentPositionTransform = firstPersonWeaponInfo.crouchPosition;
					} else {
						currentEditAttachmentPositionTransform = firstPersonWeaponInfo.walkPosition;
					}
				}

			} else {
				if (isUsingDualWeapon ()) {
					if (usingRightHandDualWeapon) {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
					} else {
						currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
					}
				} else {
					currentEditAttachmentPositionTransform = thirdPersonWeaponInfo.walkPosition;
				}
			}
		}
			
		weaponTransform.localPosition = currentEditAttachmentPositionTransform.localPosition;
		weaponTransform.localRotation = currentEditAttachmentPositionTransform.localRotation;

		Transform weaponMeshTransform = weaponSystemManager.weaponSettings.weaponMesh.transform;
		Vector3 weaponMeshTargetPosition = Vector3.zero;
		Quaternion weaponMeshTargetRotation = Quaternion.identity;
	
		weaponMeshTransform.localPosition = weaponMeshTargetPosition;
		weaponMeshTransform.localRotation = weaponMeshTargetRotation;

		Vector3 handTargetPosition = Vector3.zero;
		Transform handToMove = thirdPersonWeaponInfo.leftHandMesh.transform;

		Quaternion handTargetRotation = Quaternion.identity;	

		if (isFirstPersonActive) {
			handToMove = firstPersonWeaponInfo.leftHandMesh.transform;
		} 

		if (editingAttachments) {
			if (isFirstPersonActive) {
				handTargetPosition = firstPersonWeaponInfo.leftHandEditPosition.localPosition;
			} else {
				setElbowsIKTargetValue (0, 0);
				weaponsManager.getIKSystem ().setElbowsIKTargetValue (0, 0);
				handToMove.SetParent (thirdPersonWeaponInfo.leftHandEditPosition);
			}
		} else {
			if (!isFirstPersonActive) {
				setElbowsIKTargetValue (1, 1);
				weaponsManager.getIKSystem ().setElbowsIKTargetValue (1, 1);
				handToMove.SetParent (thirdPersonWeaponInfo.leftHandParent);
			}

			if (isFirstPersonActive) {
				if (firstPersonWeaponInfo.usingSecondPositionForHand) {
					handTargetPosition = firstPersonWeaponInfo.secondPositionForHand.localPosition;
				}
			} else {
				if (thirdPersonWeaponInfo.usingSecondPositionForHand) {
					handTargetPosition = thirdPersonWeaponInfo.secondPositionForHand.localPosition;
				}
			}
		}

		if (isFirstPersonActive) {
			handToMove.localPosition = handTargetPosition;
		} else {
			handToMove.localPosition = handTargetPosition;
			handToMove.localRotation = handTargetRotation;
		}
	}

	public void setUsingSecondPositionHandState (bool state)
	{
		firstPersonWeaponInfo.usingSecondPositionForHand = state;
		thirdPersonWeaponInfo.usingSecondPositionForHand = state;
	}

	public void setNewSecondPositionHandTransformFirstPerson (Transform newTransform)
	{
		firstPersonWeaponInfo.secondPositionForHand = newTransform;

		if (!editingAttachments) {
			checkHandsPosition ();
		}
	}

	public void setNewSecondPositionHandTransformThirdPerson (Transform newTransform)
	{
		thirdPersonWeaponInfo.secondPositionForHand = newTransform;

		if (!editingAttachments) {
			checkHandsPosition ();
		}
	}

	public void checkHandsPosition ()
	{
		if (!thirdPersonWeaponInfo.leftHandMesh || !firstPersonWeaponInfo.leftHandMesh) {
			return;
		}

		bool isFirstPersonActive = weaponsManager.isFirstPersonActive ();

		Vector3 handTargetPosition = Vector3.zero;
		Transform handToMove = thirdPersonWeaponInfo.leftHandMesh.transform;

		Quaternion handTargetRotation = Quaternion.identity;

		if (isFirstPersonActive) {
			handToMove = firstPersonWeaponInfo.leftHandMesh.transform;
		} 

		if (isFirstPersonActive) {
			if (firstPersonWeaponInfo.usingSecondPositionForHand) {
				if (firstPersonWeaponInfo.secondPositionForHand) {
					handTargetPosition = firstPersonWeaponInfo.secondPositionForHand.localPosition;
				}
			}
		} else {
			if (thirdPersonWeaponInfo.usingSecondPositionForHand) {
				if (thirdPersonWeaponInfo.secondPositionForHand) {
					handTargetPosition = thirdPersonWeaponInfo.secondPositionForHand.localPosition;
				}
			}
		}

		if (isFirstPersonActive) {
			handToMove.localPosition = handTargetPosition;
		} else {
			handToMove.localPosition = handTargetPosition;
			handToMove.localRotation = handTargetRotation;
		}
	}

	public void stopEditAttachment ()
	{
		if (editingAttachments) {
			//stop the coroutines
			stopWeaponMovement ();

			if (handAttachmentCoroutine != null) {
				StopCoroutine (handAttachmentCoroutine);
			}

			moving = false;

			setLastTimeMoved ();

			editingAttachments = false;

			if (firstPersonWeaponInfo.leftHandMesh) {
				firstPersonWeaponInfo.leftHandMesh.transform.localPosition = Vector3.zero;
			}

			if (thirdPersonWeaponInfo.leftHandMesh) {
				thirdPersonWeaponInfo.leftHandMesh.transform.localPosition = Vector3.zero;
			}
		}
	}

	public Transform getThirdPersonAttachmentCameraPosition ()
	{
		if (isUsingDualWeapon ()) {
			if (usingRightHandDualWeapon) {
				return thirdPersonWeaponInfo.rightHandDualWeaopnInfo.attachmentCameraPosition;
			} else {
				return thirdPersonWeaponInfo.leftHandDualWeaponInfo.attachmentCameraPosition;
			}
		} else {
			return thirdPersonWeaponInfo.attachmentCameraPosition;
		}
	}

	public bool isWeaponOnRecoil ()
	{
		return weaponOnRecoil;
	}

	public void setCurrentWeaponState (bool state)
	{
		currentWeapon = state;
	}

	public bool isCurrentWeapon ()
	{
		return currentWeapon;
	}

	public bool pickupAttachment (string attachmentName)
	{
		if (mainWeaponAttachmentSystem) {
			return mainWeaponAttachmentSystem.pickupAttachment (attachmentName);
		}
		return false;
	}

	public void setWeaponAttachmentSystem (weaponAttachmentSystem attachmentSystem)
	{
		mainWeaponAttachmentSystem = attachmentSystem;
	}

	public weaponAttachmentSystem getWeaponAttachmentSystem ()
	{
		return mainWeaponAttachmentSystem;
	}

	public bool checkAttachmentsHUD ()
	{
		if (mainWeaponAttachmentSystem) {
			mainWeaponAttachmentSystem.checkAttachmentsHUD ();

			return true;
		}

		return false;
	}

	public void enableAllAttachments ()
	{
		if (mainWeaponAttachmentSystem) {
			mainWeaponAttachmentSystem.enableAllAttachments ();
		}
	}

	public void setPlayerControllerMovementValues ()
	{
		if (aiming || carrying) {
			bool firstPersonActive = weaponsManager.isFirstPersonActive ();

			if (firstPersonActive) {
				useNewCarrySpeed = firstPersonWeaponInfo.useNewCarrySpeed;
				newCarrySpeed = firstPersonWeaponInfo.newCarrySpeed;
				useNewAimSpeed = firstPersonWeaponInfo.useNewAimSpeed;
				newAimSpeed = firstPersonWeaponInfo.newAimSpeed;
				canRunWhileCarrying = firstPersonWeaponInfo.canRunWhileCarrying;
				canRunWhileAiming = firstPersonWeaponInfo.canRunWhileAiming;
			} else {
				useNewCarrySpeed = thirdPersonWeaponInfo.useNewCarrySpeed;
				newCarrySpeed = thirdPersonWeaponInfo.newCarrySpeed;
				useNewAimSpeed = thirdPersonWeaponInfo.useNewAimSpeed;
				newAimSpeed = thirdPersonWeaponInfo.newAimSpeed;
				canRunWhileCarrying = thirdPersonWeaponInfo.canRunWhileCarrying;
				canRunWhileAiming = thirdPersonWeaponInfo.canRunWhileAiming;
			}
				
			if (aiming) {
				weaponsManager.setPlayerControllerMovementValues (useNewAimSpeed, newAimSpeed);
				weaponsManager.setPlayerControllerCanRunValue (canRunWhileAiming);
			} else {
				weaponsManager.setPlayerControllerMovementValues (useNewCarrySpeed, newCarrySpeed);
				weaponsManager.setPlayerControllerCanRunValue (canRunWhileCarrying);
			}
		} else {
			setPlayerControllerMovementOriginalValues ();
		}
	}

	public void setPlayerControllerMovementOriginalValues ()
	{
		weaponsManager.setPlayerControllerMovementOriginalValues ();
	}

	public void setUsingDualWeaponState (bool state)
	{
		usingDualWeapon = state;

		weaponSystemManager.setUsingDualWeaponState (state);
	}

	public void disableUsingDualWeaponState ()
	{
		disablingDualWeapon = true;
	}

	public void setDisablingDualWeaponState (bool state)
	{
		disablingDualWeapon = state;
	}

	public bool isUsingDualWeapon ()
	{
		return usingDualWeapon || disablingDualWeapon;
	}

	public void setUsingRightHandDualWeaponState (bool state)
	{
		usingRightHandDualWeapon = state;

		weaponSystemManager.setUsingRightHandDualWeaponState (state);
	}

	public bool isQuickDrawKeepDualWeaponActive ()
	{
		return (usingRightHandDualWeapon && thirdPersonWeaponInfo.rightHandDualWeaopnInfo.useQuickDrawKeepWeapon) ||
		(!usingRightHandDualWeapon && thirdPersonWeaponInfo.leftHandDualWeaponInfo.useQuickDrawKeepWeapon);
	}

	public void setWeaponConfiguredAsDualWepaonState (bool state, string newWeaponName)
	{
		if (weaponConfiguredAsDualWeaponPreviously != weaponConfiguredAsDualWepaon) {
			weaponConfiguredAsDualWeaponPreviously = weaponConfiguredAsDualWepaon;
		}

		weaponConfiguredAsDualWepaon = state;
		linkedDualWeaponName = newWeaponName;
	}

	public bool isWeaponConfiguredAsDualWepaon ()
	{
		return weaponConfiguredAsDualWepaon;
	}

	public string getLinkedDualWeaponName ()
	{
		return linkedDualWeaponName;
	}

	public bool usingWeaponAsOneHandWield;

	public void setCurrentWeaponAsOneHandWield ()
	{
		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			if (currentIKWeaponsPosition.usedToDrawWeapon) {
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
			} else {
				currentIKWeaponsPosition.ignoreIKWeight = true;
				currentIKWeaponsPosition.handInPositionToAim = true;
			}
		}

		usingWeaponAsOneHandWield = true;

		thirdPersonWeaponInfo.usingWeaponAsOneHandWield = true;

		bool rightHandUsedToDrawWeapon = false;

		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			if (currentIKWeaponsPosition.usedToDrawWeapon) {
				if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
					rightHandUsedToDrawWeapon = true;
				}
			}
		}

		setUsingRightHandDualWeaponState (rightHandUsedToDrawWeapon);

		if (!thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			
			thirdPersonWeaponInfo.usedOnRightHand = rightHandUsedToDrawWeapon;
			thirdPersonWeaponInfo.dualWeaponActive = true;

			stopWeaponMovement ();

			weaponMovement = StartCoroutine (setCurrentWeaponAsOneHandWieldCoroutine (rightHandUsedToDrawWeapon));
		}
	}

	IEnumerator setCurrentWeaponAsOneHandWieldCoroutine (bool rightHandUsedToDrawWeapon)
	{
		moving = true;

		if (rightHandUsedToDrawWeapon) {
			setHandsIKTargetValue (0, 1);
			setElbowsIKTargetValue (0, 1);
		} else {
			setHandsIKTargetValue (1, 0);
			setElbowsIKTargetValue (1, 0);
		}

		yield return new WaitForSeconds (0.2f);

		if (rightHandUsedToDrawWeapon) {
			currentDrawPositionTransform = thirdPersonWeaponInfo.rightHandDualWeaopnInfo.walkPosition;
		} else {
			currentDrawPositionTransform = thirdPersonWeaponInfo.leftHandDualWeaponInfo.walkPosition;
		}

		weaponPositionTarget = currentDrawPositionTransform.localPosition;
		weaponRotationTarget = currentDrawPositionTransform.localRotation;

		stopWeaponMovement ();

		weaponMovement = StartCoroutine (moveWeaponToPositionCoroutine (thirdPersonWeaponInfo.changeOneOrTwoHandWieldSpeed));

		moving = false;
	}

	public void setCurrentWeaponAsTwoHandsWield ()
	{
		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			if (!currentIKWeaponsPosition.usedToDrawWeapon) {
				currentIKWeaponsPosition.ignoreIKWeight = false;

				if (!thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
					currentIKWeaponsPosition.handInPositionToAim = false;
				}

				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
			}
		}

		setUsingRightHandDualWeaponState (false);

		usingWeaponAsOneHandWield = false;

		thirdPersonWeaponInfo.usingWeaponAsOneHandWield = false;

		thirdPersonWeaponInfo.usedOnRightHand = false;
		thirdPersonWeaponInfo.dualWeaponActive = false;

		if (!thirdPersonWeaponInfo.deactivateIKIfNotAiming) {
			bool rightHandUsedToDrawWeapon = false;

			for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
				currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

				if (currentIKWeaponsPosition.usedToDrawWeapon) {
					if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
						rightHandUsedToDrawWeapon = true;
					}
				}
			}

			stopWeaponMovement ();

			weaponMovement = StartCoroutine (setCurrentWeaponAsTwoHandWieldCoroutine (rightHandUsedToDrawWeapon));
		}
	}

	IEnumerator setCurrentWeaponAsTwoHandWieldCoroutine (bool rightHandUsedToDrawWeapon)
	{
		moving = true;
	
		if (rightHandUsedToDrawWeapon) {
			currentDrawPositionTransform = thirdPersonWeaponInfo.walkPosition;
		} else {
			currentDrawPositionTransform = thirdPersonWeaponInfo.walkPosition;
		}

		weaponPositionTarget = currentDrawPositionTransform.localPosition;
		weaponRotationTarget = currentDrawPositionTransform.localRotation;

		Vector3 currentWeaponPosition = weaponTransform.localPosition;
		Quaternion currentWeaponRotation = weaponTransform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * thirdPersonWeaponInfo.changeOneOrTwoHandWieldSpeed;
			weaponTransform.localPosition = Vector3.Lerp (currentWeaponPosition, weaponPositionTarget, t);
			weaponTransform.localRotation = Quaternion.Slerp (currentWeaponRotation, weaponRotationTarget, t);
			yield return null;
		}

		setHandsIKTargetValue (1, 1);
		setElbowsIKTargetValue (1, 1);

		for (int i = 0; i < thirdPersonWeaponInfo.handsInfo.Count; i++) {
			currentIKWeaponsPosition = thirdPersonWeaponInfo.handsInfo [i];

			currentIKWeaponsPosition.handInPositionToAim = true;
		}

		moving = false;
	}

	public bool getUsingWeaponAsOneHandWieldState ()
	{
		return usingWeaponAsOneHandWield;
	}

	public void createWeaponPrefab ()
	{
		#if UNITY_EDITOR
		GameObject newEmtpyWeaporefab = Instantiate (emtpyWeaponPrefab);

		playerWeaponSystem currentPlayerWeaponSystem = weaponGameObject.GetComponent<playerWeaponSystem> ();

		if (currentPlayerWeaponSystem) {
			
			GameObject weaponMeshParts = currentPlayerWeaponSystem.weaponSettings.weaponMesh;

			if (weaponMeshParts) {
				GameObject newWeaponMesh = Instantiate (weaponMeshParts);
				newWeaponMesh.transform.SetParent (newEmtpyWeaporefab.transform);
				newWeaponMesh.transform.localPosition = Vector3.zero;
				newWeaponMesh.transform.localRotation = Quaternion.identity;

				pickUpObject weaponPickupObject = newEmtpyWeaporefab.GetComponent<pickUpObject> ();

				string weaponName = currentPlayerWeaponSystem.getWeaponSystemName ();

				weaponPickupObject.secondaryString = weaponName;

				deviceStringAction currentDeviceStringAction = newEmtpyWeaporefab.GetComponentInChildren<deviceStringAction> ();

				currentDeviceStringAction.deviceName = weaponName;

				string prefabFilePath = relativePathInventory + "/" + weaponName + " Model " + ".prefab";

				UnityEngine.Object prefab = EditorUtility.CreateEmptyPrefab (prefabFilePath);
				EditorUtility.ReplacePrefab (newEmtpyWeaporefab, prefab, ReplacePrefabOptions.ConnectToPrefab);

				weaponPrefabModel = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));
			} else {
				print ("WARNING: No mesh parts configured in the player weapon system inspector");
			}
			
		} else {
			print ("WARNING: No player weapon system component found on the weapon");
		}

		DestroyImmediate (newEmtpyWeaporefab);

		updateComponent ();

		#endif
	}

	public void copyTransformValuesToBuffer (Transform transformToCopyValues)
	{
		string newPosition = transformToCopyValues.localPosition.ToString ("F7");
		string newRotation = transformToCopyValues.localEulerAngles.ToString ("F7");

		GUIUtility.systemCopyBuffer = newPosition + "_" + newRotation;

		print ("Copied transform values of " + transformToCopyValues.name + ": " + newPosition + "_" + newRotation);
	}

	public void pasteTransformValuesToBuffer (Transform transformToPasteValues)
	{
		string bufferContent = GUIUtility.systemCopyBuffer.ToString ();

		string positionValue = bufferContent;

		int separationIndex = bufferContent.IndexOf ("_");
	
		positionValue = positionValue.Substring (0, separationIndex);

		if (positionValue.StartsWith ("(") && positionValue.EndsWith (")")) {
			positionValue = positionValue.Substring (1, positionValue.Length - 2);
		}
			
		string[] sArray = positionValue.Split (',');

		Vector3 newPosition = new Vector3 (float.Parse (sArray [0]), float.Parse (sArray [1]), float.Parse (sArray [2]));

		transformToPasteValues.localPosition = newPosition;

		string rotationValue = bufferContent;

		rotationValue = rotationValue.Substring (separationIndex + 1, rotationValue.Length - separationIndex - 1);

		if (rotationValue.StartsWith ("(") && rotationValue.EndsWith (")")) {
			rotationValue = rotationValue.Substring (1, rotationValue.Length - 2);
		}

		sArray = rotationValue.Split (',');

		Vector3 newRotation = new Vector3 (float.Parse (sArray [0]), float.Parse (sArray [1]), float.Parse (sArray [2]));

		transformToPasteValues.localRotation = Quaternion.Euler (newRotation);

		print ("Pasted transform values to " + transformToPasteValues.name + ": " + newPosition.ToString ("F7") + "_" + newRotation.ToString ("F7"));
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<IKWeaponSystem> ());
		#endif
	}

	void OnDrawGizmos ()
	{
		if (!showThirdPersonGizmo && !showFirstPersonGizmo) {
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
		if (showThirdPersonGizmo) {
			drawWeaponInfoPositions (thirdPersonWeaponInfo);
		}
		if (showFirstPersonGizmo) {
			drawWeaponInfoPositions (firstPersonWeaponInfo);
		}
	}

	void drawWeaponInfoPositions (IKWeaponInfo info)
	{
		if (showPositionGizmo) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (info.aimPosition.position, 0.03f);
			Gizmos.color = Color.white;
			Gizmos.DrawLine (info.aimPosition.position, info.walkPosition.position);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (info.walkPosition.position, 0.03f);

			if (info.hasAttachments) {
				if (info.editAttachmentPosition) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (info.editAttachmentPosition.position, info.walkPosition.position);
					Gizmos.color = Color.green;
					Gizmos.DrawSphere (info.editAttachmentPosition.position, 0.03f);
				}

				if (info.useSightPosition) {
					if (info.sightPosition) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (info.sightPosition.position, info.walkPosition.position);
						Gizmos.color = Color.green;
						Gizmos.DrawSphere (info.sightPosition.position, 0.03f);
					}

					if (info.sightRecoilPosition) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (info.sightRecoilPosition.position, info.sightPosition.position);
						Gizmos.color = Color.green;
						Gizmos.DrawSphere (info.sightRecoilPosition.position, 0.03f);
					}
				}
			}

			if (info.surfaceCollisionPosition) {
				Gizmos.DrawLine (info.surfaceCollisionPosition.position, info.aimPosition.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (info.surfaceCollisionPosition.position, 0.03f);
			}

			if (info.surfaceCollisionRayPosition) {
				Gizmos.DrawLine (info.surfaceCollisionRayPosition.position, info.surfaceCollisionPosition.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (info.surfaceCollisionRayPosition.position, 0.03f);
			}
		}

		if (showWeaponWaypointGizmo) {
			for (int i = 0; i < info.keepPath.Count; i++) {
				if (i + 1 < info.keepPath.Count) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine (info.keepPath [i].position, info.keepPath [i + 1].position);
				}
				if (i != info.keepPath.Count - 1) {
					Gizmos.color = Color.red;
					Gizmos.DrawSphere (info.keepPath [i].position, 0.03f);
				}
			}

			if (info.keepPath.Count > 0) {
				Gizmos.color = Color.white;
				Gizmos.DrawLine (info.keepPosition.position, info.keepPath [0].position);
			} else {
				Gizmos.DrawLine (info.keepPosition.position, info.walkPosition.position);
			}
		}

		if (showPositionGizmo) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (info.keepPosition.position, 0.03f);
			Gizmos.color = Color.white;
			Gizmos.DrawLine (info.aimPosition.position, info.aimRecoilPosition.position);
			if (info.walkRecoilPosition) {
				Gizmos.DrawLine (info.walkPosition.position, info.walkRecoilPosition.position);
			}

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere (info.aimRecoilPosition.position, 0.03f);
			if (info.walkRecoilPosition) {
				Gizmos.DrawSphere (info.walkRecoilPosition.position, 0.03f);
			}
		}

		if (showHandsWaypointGizmo) {
			for (int i = 0; i < info.handsInfo.Count; i++) {
				Gizmos.color = Color.blue;

				currentGizmoIKWeaponPosition = info.handsInfo [i];

				if (currentGizmoIKWeaponPosition.position) {
					Gizmos.DrawSphere (currentGizmoIKWeaponPosition.position.position, 0.02f);
				}

				if (currentGizmoIKWeaponPosition.waypointFollower) {
					Gizmos.color = Color.cyan;
					Gizmos.DrawSphere (currentGizmoIKWeaponPosition.waypointFollower.position, 0.01f);
				}

				for (int j = 0; j < currentGizmoIKWeaponPosition.wayPoints.Count; j++) {
					if (j == 0) {
						if (currentGizmoIKWeaponPosition.handTransform) {
							Gizmos.color = Color.black;
							Gizmos.DrawLine (currentGizmoIKWeaponPosition.wayPoints [j].position, currentGizmoIKWeaponPosition.handTransform.position);
						}
					}

					Gizmos.color = Color.gray;
					Gizmos.DrawSphere (currentGizmoIKWeaponPosition.wayPoints [j].position, 0.01f);
					if (j + 1 < currentGizmoIKWeaponPosition.wayPoints.Count) {
						Gizmos.color = Color.yellow;
						Gizmos.DrawLine (currentGizmoIKWeaponPosition.wayPoints [j].position, currentGizmoIKWeaponPosition.wayPoints [j + 1].position);
					}
				}

				Gizmos.color = Color.blue;
				if (currentGizmoIKWeaponPosition.elbowInfo.position) {
					Gizmos.DrawSphere (currentGizmoIKWeaponPosition.elbowInfo.position.position, 0.02f);
				}
			}
		}

		if (showPositionGizmo) {
			if (info.checkSurfaceCollision) {
				if (info.surfaceCollisionRayPosition) {
					if (weaponTransform) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (info.surfaceCollisionRayPosition.position, info.surfaceCollisionRayPosition.position + info.surfaceCollisionRayPosition.forward *
						info.collisionRayDistance);
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (info.surfaceCollisionRayPosition.position + info.surfaceCollisionRayPosition.forward * info.collisionRayDistance, 0.01f);
					} else {
						weaponTransform = weaponGameObject.transform;
					}
				}
			}

			if (info.runPosition) {
				if (weaponTransform) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (info.walkPosition.position, info.runPosition.position);
					Gizmos.color = Color.red;
					Gizmos.DrawSphere (info.runPosition.position, 0.01f);
				} else {
					weaponTransform = weaponGameObject.transform;
				}
			}
		}
	}

	[System.Serializable]
	public class weaponShotShakeInfo
	{
		public float shotForce;
		public float shakeSmooth;
		public float shakeDuration;
		public Vector3 shakePosition;
		public Vector3 shakeRotation;
	}
}