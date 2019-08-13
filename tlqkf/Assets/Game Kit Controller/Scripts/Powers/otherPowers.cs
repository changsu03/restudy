using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

[System.Serializable]
public class otherPowers : MonoBehaviour
{
	public int choosedPower;
	public bool carryingObjects;
	public bool wallWalk;
	public bool wallWalkEnabled;

	public bool changeCameraSideActive = true;

	public float wallWalkRotationSpeed = 10;

	public bool running;
	public bool activatedShield;
	public armorSurfaceSystem armorSurfaceManager;
	public bool laserActive;

	public bool laserAbilityEnabled = true;

	public LayerMask layerToDamage;

	public powersSettings settings = new powersSettings ();
	public aimSettings aimsettings = new aimSettings ();
	public shootSettings shootsettings = new shootSettings ();
	public bool usingPowers = false;
	public GameObject playerLaserGameObject;
	public Rect touchZoneRect;
	public float auxPowerAmount;
	public bool showSettings;
	public bool showAimSettings;
	public bool showShootSettings;
	public GameObject playerCameraGameObject;
	public bool usedByAI;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool shooting;

	public bool infinitePower;
	public bool regeneratePower;
	public bool constantRegenerate;
	public float regenerateSpeed = 0;
	public float regenerateTime;
	public float regenerateAmount;

	public Transform carryObjectsTransform;
	public List<Transform> carryObjectsTransformList = new List<Transform> ();

	public bool useAimAssistInThirdPerson;
	public bool useMaxDistanceToCameraCenterAimAssist;
	public float maxDistanceToCameraCenterAimAssist;

	public bool useAimAssistInLockedCamera = true;

	public LayerMask targetForScorchLayer;

	public bool grabbedObjectCanBeBroken;

	public bool canGrabObjectsUsingWeapons;

	public bool stopGravityAdherenceWhenStopRun = true;

	public bool powersModeActive;

	//Teleport ability variables
	public bool teleportingEnabled;
	public LayerMask teleportLayerMask;
	public bool useBulletTimeOnTeleport;
	public float bulletTimeScaleOnTeleport = 0.5f;
	public float maxDistanceToTeleport = 100;
	public bool useTeleporIfSurfaceNotFound;
	public float maxDistanceToTeleportAir = 10;
	public float teleportSpeed = 10;
	public bool useTeleportMark;
	public GameObject teleportMark;
	public float holdButtonTimeToActivateTeleport = 0.4f;
	public bool stopTeleportIfMoving;
	public bool addForceIfTeleportStops;
	public float forceIfTeleportStops;
	public bool changeCameraFovOnTeleport;
	public float cameraFovOnTeleport;
	public float cameraFovOnTeleportSpeed;

	public bool canTeleportOnZeroGravity;

	public bool canFirePowersWithoutAiming;
	public bool useAimCameraOnFreeFireMode;
	bool aimingPowerFromShooting;
	public bool usingFreeFireMode;

	public bool checkToKeepPowersAfterAimingPowerFromShooting;

	public float timeToStopAimAfterStopFiring = 0.85f;

	public bool aimModeInputPressed;
	public bool weaponAimedFromFiringActive;

	bool checkToKeepPowersAfterAimingPowerFromShooting2_5d;

	public bool headLookWhenAiming;
	public float headLookSpeed;
	public Transform headLookTarget;

	public menuPause pauseManager;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public weaponRotationPointInfo rotationPointInfo;
	public bool usePowerRotationPoint;
	public Transform powerRotationPoint;

	public AudioSource shootZoneAudioSource;
	public Transform mainCameraTransform;
	public Transform pivotCameraTransform;
	public Camera mainCamera;
	public upperBodyRotationSystem upperBodyRotationManager;
	public IKSystem IKSystemManager;

	public playerCamera playerCameraManager;
	public scannerSystem scannerSystemManager;
	public headBob headBobManager;
	public playerWeaponsManager weaponsManager;
	public gravitySystem gravityManager;
	public playerController playerControllerManager;
	public grabObjects grabObjectsManager;
	public playerScreenObjectivesSystem playerScreenObjectivesManager;
	public headTrack headTrackManager;
	public decalManager impactDecalManager;
	public laserPlayer laserPlayerManager;
	public timeBullet timeBulletManager;
	public inputManager input;
	public powersListManager powersManager;
	public Collider playerCollider;
	public RectTransform cursorRectTransform;
	public Animation carryObjectsTransformAnimation;
	public launchTrayectory parable;
	public Renderer playerRenderer;

	public bool changePowersWithNumberKeysActive = true;
	public bool changePowersWithMouseWheelActive = true;
	public bool changePowersWithKeysActive = true;

	public bool useLowerRotationSpeedAimedThirdPerson;
	public float verticalRotationSpeedAimedInThirdPerson = 4;
	public float horizontalRotationSpeedAimedInThirdPerson = 4;

	public bool runWhenAimingPowerInThirdPerson;
	public bool stopRunIfPreviouslyNotRunning;
	bool runningPreviouslyAiming;

	bool teleportCanBeExecuted;
	bool searchingForTeleport;
	float lastTimeTeleportButtonPressed;
	Coroutine teleportCoroutine;
	Vector3 currentTeleportPosition;
	Vector3 teleportMarkDirection;
	Vector3 currentTeleportPositionNormal;
	bool teleportSurfaceFound;
	bool teleportInProcess;

	public Powers currentPower;
	Transform closestCarryObjecsTransform;

	TrailRenderer[] trails;
	GameObject currentProjectile;
	GameObject currentLaser;

	public int amountPowersEnabled;
	Vector3 normalOrig;
	Vector3 laserPosition;

	Vector3 swipeStartPos;
	Material[] mats;
	Material[] auxMats;
	List<grabbedObject> grabbedObjectList = new List<grabbedObject> ();
	List<GameObject> locatedEnemies = new List<GameObject> ();

	RaycastHit hit;
	float currentForceToLaunchObjects = 0;
	float trailTimer = -1;
	float lastTimeRun = 0;
	float buttonTimer = 0;
	float powerSelectionTimer;

	float lastTimeUsed;
	float lastTimeFired;

	float maxPowerAmount;

	bool selection;

	bool playerIsDead;
	bool checkRunGravity;
	bool homingProjectiles;
	bool touchPlatform;

	Touch currentTouch;
	laserDevice.laserType lasertype;

	GameObject closestEnemy;

	public string[] impactDecalList;

	GameObject parableGameObject;
	bool objectiveFound;
	Vector3 aimedZone;
	Vector3 forceDirection;

	Coroutine muzzleFlahsCoroutine;

	bool playerCurrentlyBusy;

	bool firstPersonActive;

	bool startInitialized;

	bool canMove;

	bool usingScreenSpaceCamera;
	bool targetOnScreen;
	Vector3 screenPoint;

	void Start ()
	{
		//get the input manager
		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}

		//get the trail renderers in the player's model
		trails = GetComponentsInChildren<TrailRenderer> ();

		canMove = !playerIsDead && playerControllerManager.canPlayerMove ();

		//get the materials of the mesh
		if (playerRenderer != null) {
			mats = playerRenderer.materials;
			auxMats = mats;
		}

		//get the slider used when the player launchs objects
		if (settings.slider) {
			settings.slider.gameObject.SetActive (false);
		}

		currentPower = shootsettings.powersList [choosedPower];

		//set the texture of the current selected power
		if (shootsettings.selectedPowerHud) {
			shootsettings.selectedPowerHud.texture = currentPower.texture;
		}

		//by default the aim mode stays in the right side of the player, but it is checked in the start
		setAimModeSide (false);

		//set a touch zone in the upper left corner of the screen, to change betweeen powers by swiping
		setHudZone ();

		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		//set the amount of current powers enabled
		updateAmountPowersEnabled ();

		//check if the platform is a touch device or not
		touchPlatform = touchJoystick.checkTouchPlatform ();

		//set the value of energy avaliable at the beginning of the game
		if (settings.powerBar) {
			settings.powerBar.maxValue = shootsettings.powerAmount;
			settings.powerBar.value = shootsettings.powerAmount;
		}

		//store the max amount of energy in axiliar variables, used for the pick ups to check that the player doesn't use more pickups that the neccessary
		maxPowerAmount = shootsettings.powerAmount;
		auxPowerAmount = shootsettings.powerAmount;
		mainCamera = playerCameraManager.getMainCamera ();

		//get the parable launcher in case the weapons has it
		if (parable) {
			parableGameObject = parable.gameObject;
		}

		if (!usedByAI) {
			setFirstPowerAvailable ();

			setSelectedPowerIconState (false);
		}

		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();
	}

	void Update ()
	{
		if (!startInitialized) {
			startInitialized = true;
		}

		canMove = !playerIsDead && playerControllerManager.canPlayerMove ();

		if (!usedByAI) {
			//the power is regenerated if the player is not using it
			if (regeneratePower && canMove && !infinitePower) {
				if (constantRegenerate) {
					if (regenerateSpeed > 0 && shootsettings.powerAmount < maxPowerAmount) {
						if (Time.time > lastTimeUsed + regenerateTime) {
							getEnergy (regenerateSpeed * Time.deltaTime);
						}
					}
				} else {
					if (shootsettings.powerAmount < maxPowerAmount) {
						if (Time.time > lastTimeUsed + regenerateTime) {
							getEnergy (regenerateAmount);
							lastTimeUsed = Time.time;
						}
					}
				}
			}
		}

		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		playerCurrentlyBusy = playerIsBusy ();

		//check that the player is not using a device, so all the key input can be checked
		if (!playerCurrentlyBusy) {
			//enable shield when the player touch a laser
			if (settings.shield.activeSelf && currentLaser) {
				Vector3 targetDir = currentLaser.transform.position - settings.shield.transform.position;
				Quaternion qTo = Quaternion.LookRotation (targetDir);
				settings.shield.transform.rotation = Quaternion.Slerp (settings.shield.transform.rotation, qTo, 10 * Time.deltaTime);
			}
				
			//if the shield is enabled, the power decreases
			if (settings.shield.activeSelf && activatedShield && !laserActive) {
				//also, rotates the shield towards the camera direction
				if (pivotCameraTransform.localRotation.x < 0) {
					settings.shield.transform.rotation = pivotCameraTransform.rotation;
				} else {
					settings.shield.transform.rotation = Quaternion.Euler (playerCameraGameObject.transform.eulerAngles);
				}
				if (isThereEnergy ()) {
					usePowerBar (Time.deltaTime * shootsettings.powerUsedByShield);
				} else {
					settings.shield.SetActive (false);
					activatedShield = false;
				}
			}

			if (!usedByAI) {
				if (powersModeActive) {
					if (changePowersWithNumberKeysActive) {
						//check if any keyboard number is preseed, and in that case, check which of it and if a power has that number associated
						for (int i = 0; i < shootsettings.powersSlotsAmount; i++) {
							if (Input.GetKeyDown ("" + (i + 1))) {
								for (int k = 0; k < shootsettings.powersList.Count; k++) {
									if (shootsettings.powersList [k].numberKey == (i + 1) && choosedPower != k) {
										if (shootsettings.powersList [k].powerEnabled) {
											choosedPower = k;
											powerChanged ();
										}
									}
								}
							}
						}
					}

					if (isAimingPower ()) {
						checkAutoShootOnTag ();
					}

					if (shootingBurst) {
						if (Time.time > lastShoot + currentPower.fireRate) {
							currentBurstAmount--;
							if (currentBurstAmount == 0) {
								powerShoot (false);
								shootingBurst = false;
							} else {
								powerShoot (true);
							}
						}
					}
				}
			}

			//if the wheel of the mouse rotates, the selected power is showed in the center of the screen a few seconds, and also changed in the hud
			if (selection) {
				powerSelectionTimer -= Time.deltaTime;
				if (powerSelectionTimer < 0) {
					powerSelectionTimer = 0.5f;
					selection = false;
					setSelectedPowerIconState (false);
				}
			}

			//if the touch controls are enabled, activate the swipe option
			if (input.isUsingTouchControls ()) {
				//select the power by swiping the finger in the left corner of the screen, above the selected power icon
				int touchCount = Input.touchCount;
				if (!touchPlatform) {
					touchCount++;
				}
				for (int i = 0; i < touchCount; i++) {
					if (!touchPlatform) {
						currentTouch = touchJoystick.convertMouseIntoFinger ();
					} else {
						currentTouch = Input.GetTouch (i);
					}
					//get the start position of the swipe
					if (currentTouch.phase == TouchPhase.Began) {
						if (touchZoneRect.Contains (currentTouch.position) && !shootsettings.touching) {
							swipeStartPos = currentTouch.position;
							shootsettings.touching = true;
						}
					}
					//and the final position, and get the direction, to change to the previous or the next power
					if (currentTouch.phase == TouchPhase.Ended && shootsettings.touching) {
						float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;
						if (swipeDistHorizontal > shootsettings.minSwipeDist) {
							float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);
							if (swipeValue > 0) {
								//right swipe
								choosePreviousPower ();
							} else if (swipeValue < 0) {
								//left swipe
								chooseNextPower ();
							}
						}
						shootsettings.touching = false;
					}
				}
			} 

			//if the player is editing the power list using the power manager, disable the swipe checking
			else if (powersManager && powersManager.editingPowers) {
				shootsettings.touching = false;
				return;
			}

			//if the homing projectiles are being using, then
			if (homingProjectiles) {
				//while the number of located enemies is lowers that the max enemies amount, then
				if (locatedEnemies.Count < currentPower.homingProjectilesMaxAmount) {
					//uses a ray to detect enemies, to locked them
					bool surfaceFound = false;
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
						if (hit.collider != playerCollider) {
							surfaceFound = true;
						} else {
							if (Physics.Raycast (hit.point + mainCameraTransform.forward * 0.2f, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
								surfaceFound = true;
							}
						}
					}

					if (surfaceFound) {
						GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);
						if (target != null && target != gameObject) {
							if (currentPower.tagToLocate.Contains (target.tag)) {

								//print (target);

								GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);
								if (!locatedEnemies.Contains (placeToShoot)) {
									//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
									locatedEnemies.Add (placeToShoot);

									playerScreenObjectivesManager.addElementToPlayerList (placeToShoot, false, false, 0, true, false, 
										false, currentPower.locatedEnemyIconName, false, Color.white, true, -1, 0, false);
								}
							}
						}
					}
				}
			}
				

			if (!usedByAI && teleportingEnabled && !gravityManager.isSearchingSurface () && (!playerControllerManager.isPlayerOnZeroGravityMode () || canTeleportOnZeroGravity)) {
				if (searchingForTeleport) {
					if ((Time.time > lastTimeTeleportButtonPressed + holdButtonTimeToActivateTeleport || teleportInProcess) && !teleportCanBeExecuted) {
						teleportCanBeExecuted = true;

						if (useTeleportMark) {
							teleportMark.SetActive (true);
						}

						stopTeleporting ();

						if (useBulletTimeOnTeleport) {
							timeBulletManager.setBulletTimeState (true, bulletTimeScaleOnTeleport); 
						}

						setPlayerControlState (false);
					}

					if (teleportCanBeExecuted && useTeleportMark) {
						if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceToTeleport, teleportLayerMask)) {
							currentTeleportPosition = hit.point + hit.normal * 0.4f;
							teleportMark.transform.position = hit.point + hit.normal * 0.1f;
							currentTeleportPositionNormal = hit.normal;
							teleportSurfaceFound = true;
						} else {
							teleportSurfaceFound = false;
							currentTeleportPosition = mainCameraTransform.position + mainCameraTransform.forward * maxDistanceToTeleportAir;
							teleportMark.transform.position = currentTeleportPosition;
							currentTeleportPositionNormal = Vector3.up;
						}
							
						if (useTeleporIfSurfaceNotFound || !useTeleporIfSurfaceNotFound && teleportSurfaceFound) {
							Quaternion teleportMarkTargetRotation = Quaternion.LookRotation (currentTeleportPositionNormal);
							teleportMark.transform.rotation = teleportMarkTargetRotation;
							if (!teleportMark.activeSelf) {
								teleportMark.SetActive (true);
							}
						} else {
							if (teleportMark.activeSelf) {
								teleportMark.SetActive (false);
							}
						}
					}

					if (pauseManager.gamePaused || (stopTeleportIfMoving && playerControllerManager.isPlayerUsingInput ())) {
						teleportCanBeExecuted = false;
						if (useBulletTimeOnTeleport) {
							timeBulletManager.disableTimeBulletTotally (); 
						}
						searchingForTeleport = false;
						if (useTeleportMark) {
							teleportMark.SetActive (false);
						}
						stopTeleporting ();
					}
				}

				if (teleportInProcess && stopTeleportIfMoving && playerControllerManager.isPlayerUsingInput ()) {
					stopTeleporting ();
					teleportInProcess = false;
					if (addForceIfTeleportStops) {
						Vector3 targetPositionDirection = currentTeleportPosition - transform.position;
						targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;
						playerControllerManager.addExternalForce (targetPositionDirection * forceIfTeleportStops);
					}
					if (changeCameraFovOnTeleport) {
						playerCameraManager.setOriginalCameraFov ();
					}
				}
			}

			//when the player touchs a new surface, he is rotated to it while he stills running
			if (wallWalk) {
				//check a surface in front of the player, to rotate to it
				if (Physics.Raycast (transform.position + transform.up, transform.forward, out hit, 2, settings.layer)) {
					if (!hit.collider.isTrigger) {
						gravityManager.checkRotateToSurfaceWithoutParent (hit.normal, wallWalkRotationSpeed);
						playerControllerManager.setCurrentNormalCharacter (hit.normal);
					}
				}

				//check if the player is too far from his current ground, to rotate to his previous normal
				if (!Physics.Raycast (transform.position + transform.up, -transform.up, out hit, 5, settings.layer)) {
					if (gravityManager.getCurrentRotatingNormal () != normalOrig && !checkRunGravity) {
						checkRunGravity = true;
						if (gravityManager.isCharacterRotatingToSurface ()) {
							gravityManager.stopRotateToSurfaceWithOutParentCoroutine ();
						}

						gravityManager.checkRotateToSurface (normalOrig, 2);
						playerControllerManager.setCurrentNormalCharacter (normalOrig);
					}

					if (checkRunGravity && gravityManager.getCurrentRotatingNormal () == normalOrig) {
						checkRunGravity = false;
					}
				}
			}

			if (carryingObjects) {
				if (grabbedObjectList.Count > 0) {
					//when all the objects are stored, then set their position close to the player
					for (int k = 0; k < grabbedObjectList.Count; k++) {
						if (grabbedObjectList [k].objectToMove) {

							float distance = GKC_Utils.distance (grabbedObjectList [k].objectToMove.transform.localPosition, Vector3.zero);
							if (distance > 0.8f) {
								Vector3 nextPos = grabbedObjectList [k].objectToFollow.position;
								Vector3 currPos = grabbedObjectList [k].objectToMove.transform.position;
								grabbedObjectList [k].mainRigidbody.velocity = (nextPos - currPos) * 5;
							} else {
								grabbedObjectList [k].mainRigidbody.velocity = Vector3.zero;
							}
						} else {
							grabbedObjectList.RemoveAt (k);
						}
					}
				} else {
					carryingObjects = false;
				}
			}

			//stop the running action if the player is not moving
			if (playerControllerManager.moveInput.magnitude == 0) {
				if (running) {
					stopRun ();
				}
			}
		}

		//just a configuration to the trails in the player
		if (settings.trailsActive) {
			if (trailTimer > 0) {
				trailTimer -= Time.deltaTime;
				for (int j = 0; j < trails.Length; j++) {
					trails [j].time -= Time.deltaTime;
				}
			}
			if (trailTimer <= 0 && trailTimer > -1) {
				for (int j = 0; j < trails.Length; j++) {
					trails [j].enabled = false;
				}
				trailTimer = -1;
			}
		}

		if (!firstPersonActive) {

			if (aimingInThirdPerson && (checkToKeepPowersAfterAimingPowerFromShooting || checkToKeepPowersAfterAimingPowerFromShooting2_5d)) {
				if (Time.time > lastTimeFired + timeToStopAimAfterStopFiring) {

					disableFreeFireModeAfterStopFiring ();
				}
			}
		}
	}

	public void disableFreeFireModeAfterStopFiring ()
	{
		headTrackManager.setOriginalCameraBodyWeightValue ();

		checkToKeepPowersAfterAimingPowerFromShooting = false;

		checkToKeepPowersAfterAimingPowerFromShooting2_5d = false;

		playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		aimModeInputPressed = false;

		useAimMode ();
	}

	public void resetPowerFiringAndAimingIfPlayerDisabled ()
	{
		if (powersModeActive) {
			disableFreeFireModeAfterStopFiring ();

			aimOrKeepPowerInThirdPerson (false);

			disableFreeFireModeState ();

			shootPower (false);

			if (!firstPersonActive) {
				deactivateAimMode ();
			}

			IKSystemManager.disableArmsState ();
		}
	}

	public bool getteleportCanBeExecutedState ()
	{
		return teleportCanBeExecuted;
	}

	public void teleportPlayer (Transform objectToMove, Vector3 teleportPosition, bool checkPositionAngle, bool changeFov, float teleportSpeed)
	{
		stopTeleporting ();
		teleportCoroutine = StartCoroutine (teleportPlayerCoroutine (objectToMove, teleportPosition, checkPositionAngle, changeFov, teleportSpeed));
	}

	public void stopTeleporting ()
	{
		if (teleportCoroutine != null) {
			StopCoroutine (teleportCoroutine);
		}
		setPlayerControlState (true);
	}

	public void setPlayerControlState (bool state)
	{
		playerControllerManager.changeScriptState (state);
		playerControllerManager.setGravityForcePuase (!state);
		playerControllerManager.setRigidbodyVelocityToZero ();
		playerControllerManager.setPhysicMaterialAssigmentPausedState (!state);

		if (!state) {
			playerControllerManager.setZeroFrictionMaterial ();
		}
	}

	IEnumerator teleportPlayerCoroutine (Transform objectToMove, Vector3 targetPosition, bool checkPositionAngle, bool changeFov, float currentTeleportSpeed)
	{
		teleportInProcess = true;

		setPlayerControlState (false);

		if (changeCameraFovOnTeleport && changeFov) {
			playerCameraManager.setMainCameraFov (cameraFovOnTeleport, cameraFovOnTeleportSpeed);
		}

		float dist = GKC_Utils.distance (objectToMove.position, targetPosition);
		float duration = dist / currentTeleportSpeed;
		float t = 0;

		Vector3 targetPositionDirection = targetPosition - objectToMove.position;
		targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;

		if (checkPositionAngle) {
			float targetNormalAngle = Vector3.Angle (objectToMove.up, currentTeleportPositionNormal);
			if (targetNormalAngle > 150) {
				targetPosition += currentTeleportPositionNormal * 2;
			}
		}
	
		bool targetReached = false;
		while (!targetReached && t < 1 && objectToMove.position != targetPosition) {
			t += Time.deltaTime / duration;
			objectToMove.position = Vector3.Lerp (objectToMove.position, targetPosition, t);
			if (GKC_Utils.distance (objectToMove.position, targetPosition) < 0.2f) {
				targetReached = true;
			}
			yield return null;
		}

		setPlayerControlState (true);

		teleportInProcess = false;

		if (changeCameraFovOnTeleport && changeFov) {
			playerCameraManager.setOriginalCameraFov ();
		}
	}

	public bool isTeleportInProcess ()
	{
		return teleportInProcess;
	}

	public bool isSearchingForTeleport ()
	{
		return searchingForTeleport;
	}

	//check if the player is using a device or using a game submen
	public bool playerIsBusy ()
	{
		if (!playerControllerManager.usingDevice && ((pauseManager && !pauseManager.usingSubMenu && !pauseManager.playerMenuActive) || !pauseManager)) {
			return false;
		}
		return true;
	}

	public void useAimMode ()
	{
		if (canMove && settings.aimModeEnabled && !playerControllerManager.isGravityPowerActive () && !firstPersonActive && !playerControllerManager.sphereModeActive) {
			
			//check if the player is crouched, to prevent that the player enables the aim mode in a place where he can not get up
			if (playerControllerManager.isCrouching ()) {
				playerControllerManager.crouch ();
			}

			//if the player can get up, or was not crouched, allow to enable or disable the aim mode
			if (!playerControllerManager.isCrouching ()) {
				if ((!aimingPowerFromShooting && !checkToKeepPowersAfterAimingPowerFromShooting) || !aimingInThirdPerson) {
					//print ("aimorkeep");
					aimOrKeepPower ();
				}

				//print ("aimModeInputPressed " + aimModeInputPressed);
				if (aimModeInputPressed && (checkToKeepPowersAfterAimingPowerFromShooting || usingFreeFireMode)) {
					//print ("disable");
					disableFreeFireModeState ();
				}

				if (isAimingPower ()) {
					//print ("activateaimmode");
					activateAimMode ();
				} else {
					deactivateAimMode ();
				}


				if (runWhenAimingPowerInThirdPerson) {
					if (!aimingPowerFromShooting) {
						if (aimingInThirdPerson) {
							runningPreviouslyAiming = running;
							if (!runningPreviouslyAiming) {
								checkIfCanRun ();
							}
						} else {
							if (stopRunIfPreviouslyNotRunning) {
								if (!runningPreviouslyAiming) {
									stopRun ();
								}
							}
						}
					}
				}
			}
		}
	}

	public void aimOrKeepPower ()
	{
		if (firstPersonActive) {
			aimOrKeepPowerInFirstPerson (!aimingInFirstPerson);
		} else {
			aimOrKeepPowerInThirdPerson (!aimingInThirdPerson);
		}
	}

	public void aimOrKeepPowerInThirdPerson (bool state)
	{
		aimingInThirdPerson = state;

		if (!state) {
			weaponAimedFromFiringActive = false;
		}
	}

	public void aimOrKeepPowerInFirstPerson (bool state)
	{
		aimingInFirstPerson = state;
	}

	public bool isAimingPower ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool isAimingPowerInFirstPerson ()
	{
		return aimingInFirstPerson;
	}

	public bool isAimingPowerInThirdPerson ()
	{
		return aimingInThirdPerson;
	}

	public void keepPower ()
	{
		aimOrKeepPowerInFirstPerson (false);
		aimOrKeepPowerInThirdPerson (false);
	}

	public void setAimOrKeepPowerState (bool firstPersonState, bool thirdPersonState)
	{
		aimOrKeepPowerInFirstPerson (firstPersonState);
		aimOrKeepPowerInThirdPerson (thirdPersonState);
	}

	public projectileInfo setPowerProjectileInfo (Powers selectedPower)
	{
		projectileInfo newProjectile = new projectileInfo ();

		newProjectile.isHommingProjectile = selectedPower.isHommingProjectile;
		newProjectile.isSeeker = selectedPower.isSeeker;
		newProjectile.waitTimeToSearchTarget = selectedPower.waitTimeToSearchTarget;
		newProjectile.useRayCastShoot = selectedPower.useRayCastShoot;
		newProjectile.useRaycastCheckingOnRigidbody = selectedPower.useRaycastCheckingOnRigidbody;

		newProjectile.projectileDamage = selectedPower.projectileDamage;
		newProjectile.projectileSpeed = selectedPower.projectileSpeed;

		newProjectile.impactForceApplied = selectedPower.impactForceApplied;
		newProjectile.forceMode = selectedPower.forceMode;
		newProjectile.applyImpactForceToVehicles = selectedPower.applyImpactForceToVehicles;
		newProjectile.impactForceToVehiclesMultiplier = selectedPower.impactForceToVehiclesMultiplier;

		newProjectile.projectileWithAbility = selectedPower.projectileWithAbility;

		newProjectile.impactSoundEffect = selectedPower.impactSoundEffect;

		newProjectile.scorch = selectedPower.scorch;
		newProjectile.scorchRayCastDistance = selectedPower.scorchRayCastDistance;

		newProjectile.owner = gameObject;

		newProjectile.projectileParticles = selectedPower.projectileParticles;
		newProjectile.impactParticles = selectedPower.impactParticles;

		newProjectile.isExplosive = selectedPower.isExplosive;
		newProjectile.isImplosive = selectedPower.isImplosive;
		newProjectile.useExplosionDelay = selectedPower.useExplosionDelay;
		newProjectile.explosionDelay = selectedPower.explosionDelay;
		newProjectile.explosionForce = selectedPower.explosionForce;
		newProjectile.explosionRadius = selectedPower.explosionRadius;
		newProjectile.explosionDamage = selectedPower.explosionDamage;
		newProjectile.pushCharacters = selectedPower.pushCharacters;
		newProjectile.canDamageProjectileOwner = selectedPower.canDamageProjectileOwner;
		newProjectile.applyExplosionForceToVehicles = selectedPower.applyExplosionForceToVehicles;
		newProjectile.explosionForceToVehiclesMultiplier = selectedPower.explosionForceToVehiclesMultiplier;

		newProjectile.killInOneShot = selectedPower.killInOneShot;

		newProjectile.useDisableTimer = selectedPower.useDisableTimer;
		newProjectile.noImpactDisableTimer = selectedPower.noImpactDisableTimer;
		newProjectile.impactDisableTimer = selectedPower.impactDisableTimer;

		newProjectile.targetToDamageLayer = shootsettings.targetToDamageLayer;
		newProjectile.targetForScorchLayer = targetForScorchLayer;

		newProjectile.impactDecalIndex = selectedPower.impactDecalIndex;

		newProjectile.launchProjectile = selectedPower.launchProjectile;

		newProjectile.adhereToSurface = selectedPower.adhereToSurface;
		newProjectile.adhereToLimbs = selectedPower.adhereToLimbs;

		newProjectile.useGravityOnLaunch = selectedPower.useGravityOnLaunch;
		newProjectile.useGraivtyOnImpact = selectedPower.useGraivtyOnImpact;

		newProjectile.breakThroughObjects = selectedPower.breakThroughObjects;
		newProjectile.infiniteNumberOfImpacts = selectedPower.infiniteNumberOfImpacts;
		newProjectile.numberOfImpacts = selectedPower.numberOfImpacts;
		newProjectile.canDamageSameObjectMultipleTimes = selectedPower.canDamageSameObjectMultipleTimes;
		newProjectile.forwardDirection = mainCameraTransform.forward;

		newProjectile.damageTargetOverTime = selectedPower.damageTargetOverTime;
		newProjectile.damageOverTimeDelay = selectedPower.damageOverTimeDelay;
		newProjectile.damageOverTimeDuration = selectedPower.damageOverTimeDuration;
		newProjectile.damageOverTimeAmount = selectedPower.damageOverTimeAmount;
		newProjectile.damageOverTimeRate = selectedPower.damageOverTimeRate;
		newProjectile.damageOverTimeToDeath = selectedPower.damageOverTimeToDeath;
		newProjectile.removeDamageOverTimeState = selectedPower.removeDamageOverTimeState;

		newProjectile.sedateCharacters = selectedPower.sedateCharacters;
		newProjectile.sedateDelay = selectedPower.sedateDelay;
		newProjectile.useWeakSpotToReduceDelay = selectedPower.useWeakSpotToReduceDelay;
		newProjectile.sedateDuration = selectedPower.sedateDuration;
		newProjectile.sedateUntilReceiveDamage = selectedPower.sedateUntilReceiveDamage;

		newProjectile.pushCharacter = selectedPower.pushCharacter;
		newProjectile.pushCharacterForce = selectedPower.pushCharacterForce;
		newProjectile.pushCharacterRagdollForce = selectedPower.pushCharacterRagdollForce;
	
		return newProjectile;
	}

	public void enableMuzzleFlashLight ()
	{
		if (!currentPower.useMuzzleFlash) {
			return;
		}

		if (muzzleFlahsCoroutine != null) {
			StopCoroutine (muzzleFlahsCoroutine);
		}
		muzzleFlahsCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		currentPower.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (currentPower.muzzleFlahsDuration);

		currentPower.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	//use the remaining power of the player, to use any of his powers
	void usePowerBar (float amount)
	{
		if (infinitePower) {
			return;
		}
		shootsettings.powerAmount -= amount;
		auxPowerAmount = shootsettings.powerAmount;
		lastTimeUsed = Time.time;
		updateSlider (shootsettings.powerAmount);
	}

	public bool isThereEnergy ()
	{
		if (shootsettings.powerAmount > 0 || infinitePower) {
			return true;
		}
		return false;
	}

	public void updateSlider (float value)
	{
		if (settings.powerBar) {
			settings.powerBar.value = value;
		}
	}

	//if the player pick a enegy object, increase his energy value
	public void getEnergy (float amount)
	{
		if (canMove) {
			shootsettings.powerAmount += amount;
			//check that the energy amount is not higher that the energy max value of the slider
			if (shootsettings.powerAmount >= maxPowerAmount) {
				shootsettings.powerAmount = maxPowerAmount;
			}
			updateSlider (shootsettings.powerAmount);
		}

		auxPowerAmount = shootsettings.powerAmount;
	}

	public void removeEnergy (float amount)
	{
		shootsettings.powerAmount -= amount;
		//check that the energy amount is not higher that the energy max value of the slider
		if (shootsettings.powerAmount < 0) {
			shootsettings.powerAmount = 0;
		}
		updateSlider (shootsettings.powerAmount);
		auxPowerAmount = shootsettings.powerAmount;
	}

	//energy management
	public float getCurrentEnergyAmount ()
	{
		return shootsettings.powerAmount;
	}

	public float getMaxEnergyAmount ()
	{
		return maxPowerAmount;
	}

	public float getAuxEnergyAmount ()
	{
		return auxPowerAmount;
	}

	public void addAuxEnergyAmount (float amount)
	{
		auxPowerAmount += amount;
	}

	public float getEnergyAmountToLimit ()
	{
		return maxPowerAmount - auxPowerAmount;
	}

	//remove the localte enemies icons
	void removeLocatedEnemiesIcons ()
	{
		for (int i = 0; i < locatedEnemies.Count; i++) {
			playerScreenObjectivesManager.removeElementFromListByPlayer (locatedEnemies [i]);
		}
		locatedEnemies.Clear ();
	}

	//set the choosed power value in the next, changing the type of shoot action
	public void chooseNextPower ()
	{
		//if the wheel mouse or the change power button have been used and the powers can be changed, then
		if (amountPowersEnabled > 1 && settings.changePowersEnabled) {
			//increase the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex++;

			//if the index is higher than the current powers slots, reset the index
			if (currentPowerIndex > shootsettings.powersSlotsAmount) {
				currentPowerIndex = 1;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = 0; k < shootsettings.powersList.Count; k++) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}
				max++;
				if (max > 100) {
					//print ("forward error in index");
					return;
				}
				//set the current power
				currentPowerIndex++;
				if (currentPowerIndex > shootsettings.powersSlotsAmount) {
					currentPowerIndex = 1;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}
	}

	//set the choosed power value in the previous, changing the type of shoot action
	public void choosePreviousPower ()
	{
		//if the wheel mouse or the change power button have been used and the powers can be changed, then
		if (amountPowersEnabled > 1 && settings.changePowersEnabled) {
			//decrease the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex--;

			//if the index is lower than 0, reset the index
			if (currentPowerIndex < 1) {
				currentPowerIndex = shootsettings.powersSlotsAmount;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = shootsettings.powersList.Count - 1; k >= 0; k--) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}
				max++;
				if (max > 100) {
					//print ("backward error in index");
					return;
				}
				//set the current power
				currentPowerIndex--;
				if (currentPowerIndex < 1) {
					currentPowerIndex = shootsettings.powersSlotsAmount;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}
	}

	//every time that a power is selected, the icon of the power is showed in the center of the screen
	//and changed if the upper left corner of the screen
	void powerChanged ()
	{
		if (settings.changePowersEnabled) {
			selection = true;
			powerSelectionTimer = 0.5f;

			if (grabObjectsManager) {
				grabObjectsManager.checkIfDropObjectIfNotPhysical (false);
			}

			currentPower = shootsettings.powersList [choosedPower];
			shootsettings.selectedPowerHud.texture = currentPower.texture;
			shootsettings.selectedPowerIcon.texture = currentPower.texture;
			setSelectedPowerIconState (true);

			removeLocatedEnemiesIcons ();
			checkParableTrayectory (true);
		}
	}

	public void checkParableTrayectory (bool parableState)
	{
		if (currentPower == null) {
			return;
		}
		//enable or disable the parable linerenderer
		if (((currentPower.activateLaunchParableThirdPerson && aimingInThirdPerson) ||
		    (currentPower.activateLaunchParableFirstPerson && aimingInFirstPerson)) && parableState && currentPower.launchProjectile) {
			if (parable) {
				parableGameObject.transform.position = shootsettings.shootZone.position;
				parable.shootPosition = shootsettings.shootZone;
				parable.changeParableState (true);
			}
		} else {
			if (parable) {
				parable.changeParableState (false);
			}
		}
	}

	public void grabCloseObjects ()
	{
		//if the player has not grabbedObjects, store them
		if (grabbedObjectList.Count == 0) {
			//check in a radius, the close objects which can be grabbed
			Collider[] objects = Physics.OverlapSphere (carryObjectsTransform.position + transform.up, settings.grabRadius);
			foreach (Collider hits in objects) {
				Collider currentCollider = hits.GetComponent<Collider> ();
				Rigidbody currentRigidbody = hits.GetComponent<Rigidbody> ();
				if (settings.ableToGrabTags.Contains (currentCollider.tag.ToString ()) && currentRigidbody) {
					if (currentRigidbody.isKinematic) {
						currentRigidbody.isKinematic = false;
					}

					grabbedObject newGrabbedObject = new grabbedObject ();
					//removed tag and layer after store them, so the camera can still use raycast properly
					GameObject currentObject = hits.gameObject;
					newGrabbedObject.objectToMove = currentObject;
					newGrabbedObject.objectTag = currentObject.tag;
					newGrabbedObject.objectLayer = currentObject.layer;
					newGrabbedObject.mainRigidbody = currentRigidbody;

					currentObject.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);
					currentObject.tag = "Untagged";
					currentObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
					currentRigidbody.useGravity = false;

					//get the distance from every object to left and right side of the player, to set every side as parent of every object
					//disable collisions between the player and the objects, to avoid issues
					Physics.IgnoreCollision (currentCollider, playerControllerManager.getMainCollider (), true);

					float distance = Mathf.Infinity;
					for (int k = 0; k < carryObjectsTransformList.Count; k++) {
						float currentDistance = GKC_Utils.distance (currentObject.transform.position, carryObjectsTransformList [k].position);
						if (currentDistance < distance) {
							distance = currentDistance;
							closestCarryObjecsTransform = carryObjectsTransformList [k];
						}
					}

					if (closestCarryObjecsTransform != null) {
						currentObject.transform.SetParent (closestCarryObjecsTransform);
						newGrabbedObject.objectToFollow = closestCarryObjecsTransform;
						closestCarryObjecsTransform = null;
					}

					//if any object grabbed has its own gravity, paused the script to move the object properly
					artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();
					if (currentArtificialObjectGravity) {
						currentArtificialObjectGravity.active = false;
					}

					explosiveBarrel currentExplosiveBarrel = currentObject.GetComponent<explosiveBarrel> ();
					if (currentExplosiveBarrel) {
						currentExplosiveBarrel.setExplosiveBarrelOwner (gameObject);
					}

					if (!grabbedObjectCanBeBroken) {
						if (currentExplosiveBarrel) {
							currentExplosiveBarrel.canExplodeState (false);
						}

						crate currentCrate = currentObject.GetComponent<crate> ();
						if (currentCrate) {
							currentCrate.crateCanBeBrokenState (false);
						}
					}

					grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();
					if (!currentGrabbedObjectState) {
						currentGrabbedObjectState = currentObject.AddComponent<grabbedObjectState> ();
					}

					objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();
					if (currentObjectToPlaceSystem) {
						currentObjectToPlaceSystem.setObjectInGrabbedState (true);
					}
						
					//if any object is pickable and is inside an opened chest, activate its trigger or if it has been grabbed by the player, remove of the list
					pickUpObject currentPickUpObject = currentObject.GetComponent<pickUpObject> ();
					if (currentPickUpObject) {
						currentPickUpObject.activateObjectTrigger ();
					}

					deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();
					if (currentDeviceStringAction) {
						currentDeviceStringAction.setIconEnabledState (false);
					}

					if (currentGrabbedObjectState) {
						currentGrabbedObjectState.setCurrentHolder (gameObject);
						currentGrabbedObjectState.setGrabbedState (true);
					}

					grabbedObjectList.Add (newGrabbedObject);
				}
			}
			//if there are not any object close to the player, cancel 
			if (grabbedObjectList.Count > 0) {
				carryingObjects = true;
			}
		} 
	}

	//drop or throw the current grabbed objects
	public void dropObjects ()
	{
		if (!carryObjectsTransformAnimation.IsPlaying ("grabObjects")) {
			//get the point at which the camera is looking, to throw the objects in that direction
			Vector3 hitDirection = Vector3.zero;
			bool surfaceFound = false;
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
				if (hit.collider != playerCollider) {
					surfaceFound = true;
				} else {
					if (Physics.Raycast (hit.point + mainCameraTransform.forward, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
						surfaceFound = true;
					}
				}
			}

			if (surfaceFound) {
				hitDirection = hit.point;
			}

			for (int j = 0; j < grabbedObjectList.Count; j++) {
				dropObject (grabbedObjectList [j], hitDirection);
			}

			carryingObjects = false;
			grabbedObjectList.Clear ();
			usingPowers = false;
			enableOrDisablePowerCursor (false);
			if (settings.slider) {
				settings.slider.gameObject.SetActive (false);
				settings.slider.value = 0;
			}
		}
	}

	public void addForceToLaunchObjects ()
	{
		if (!carryObjectsTransformAnimation.IsPlaying ("grabObjects")) {
			if (currentForceToLaunchObjects < 3500) {
				//enable the power slider in the center of the screen
				currentForceToLaunchObjects += Time.deltaTime * 1200;
				if (settings.slider && currentForceToLaunchObjects > 300) {
					settings.slider.value = currentForceToLaunchObjects;
					if (!settings.slider.gameObject.activeSelf) {
						settings.slider.gameObject.SetActive (true);
					}
					usingPowers = true;
					enableOrDisablePowerCursor (true);
				}
			}
		}
	}

	public void dropObject (grabbedObject currentGrabbedObject, Vector3 launchDirection)
	{
		GameObject currentObject = currentGrabbedObject.objectToMove;

		Rigidbody currentRigidbody = currentGrabbedObject.mainRigidbody;

		currentObject.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);
		currentObject.transform.SetParent (null);

		currentObject.tag = currentGrabbedObject.objectTag.ToString ();
		currentObject.layer = currentGrabbedObject.objectLayer;

		//drop the objects, because the grab objects button has been pressed quickly
		if (currentForceToLaunchObjects < 300) {
			Physics.IgnoreCollision (currentObject.GetComponent<Collider> (), playerControllerManager.getMainCollider (), false);
		}

		//launch the objects according to the amount of time that the player has held the buttton
		if (currentForceToLaunchObjects > 300) {
			//if the objects are launched, add the script launchedObject, to damage any enemy that the object would touch
			currentObject.AddComponent<launchedObjects> ().setCurrentPlayerAndCollider (gameObject, playerControllerManager.getMainCollider ());
			//if there are any collider in from of the camera, use the hit point, else, use the camera direciton
			if (launchDirection != Vector3.zero) {
				Vector3 throwDirection = launchDirection - currentObject.transform.position;
				throwDirection = throwDirection / throwDirection.magnitude;
				currentRigidbody.AddForce (throwDirection * currentForceToLaunchObjects * currentRigidbody.mass);
			} else {
				currentRigidbody.AddForce (mainCameraTransform.TransformDirection (Vector3.forward) * currentForceToLaunchObjects * currentRigidbody.mass);
			}
		}

		//set again the custom gravity of the object
		artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();
		if (currentArtificialObjectGravity) {
			currentArtificialObjectGravity.active = true;
		}

		explosiveBarrel currentExplosiveBarrel = currentObject.GetComponent<explosiveBarrel> ();

		if (!grabbedObjectCanBeBroken) {
			if (currentExplosiveBarrel) {
				currentExplosiveBarrel.canExplodeState (true);
			}

			crate currentCrate = currentObject.GetComponent<crate> ();
			if (currentCrate) {
				currentCrate.crateCanBeBrokenState (true);
			}
		}

		deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();
		if (currentDeviceStringAction) {
			currentDeviceStringAction.setIconEnabledState (true);
		}

		objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();
		if (currentObjectToPlaceSystem) {
			currentObjectToPlaceSystem.setObjectInGrabbedState (false);
		}

		grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();
		if (currentGrabbedObjectState) {
			bool currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();
			currentGrabbedObjectState.checkGravityRoomState ();
			currentGrabbedObjectState.setGrabbedState (false);
			if (!currentObjectWasInsideGravityRoom) {
				Destroy (currentGrabbedObjectState);
				currentRigidbody.useGravity = true;
			}
		}
	}

	//if the player edits the current powers in the wheel, when a power is changed of place, removed, or added, change its key number to change
	//and the order in the power list
	public void changePowerState (Powers power, int numberKey, bool value, int index)
	{
		print (power.Name + " " + value);
		//change the state of the power sent
		for (int k = 0; k < shootsettings.powersList.Count; k++) {
			if (shootsettings.powersList [k].Name == power.Name) {
				shootsettings.powersList [k].numberKey = numberKey;
				shootsettings.powersList [k].powerEnabled = value;
			}
		}

		//increase or decrease the amount of powers enabled
		amountPowersEnabled += index;
		//if the current power is removed, select the previous
		if (amountPowersEnabled > 0 && !value && currentPower.Name == power.Name) {
			//decrease the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex--;

			//if the index is lower than 0, reset the index
			if (currentPowerIndex < 1) {
				currentPowerIndex = shootsettings.powersSlotsAmount;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = shootsettings.powersList.Count - 1; k >= 0; k--) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}
				max++;
				if (max > 100) {
					//print ("backward error in index");
					return;
				}
				//set the current power
				currentPowerIndex--;
				if (currentPowerIndex < 1) {
					currentPowerIndex = shootsettings.powersSlotsAmount;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}

		//if all the powers are disabled, disable the icon in the upper left corner of the screen
		if (amountPowersEnabled == 0) {
			shootsettings.selectedPowerHud.texture = null;
			shootsettings.selectedPowerHud.gameObject.SetActive (false);
			shootsettings.selectedPowerIcon.texture = null;
		} 

		//if only a power still enabled and the power is not selected, search and set it.
		else if (amountPowersEnabled == 1) {
			for (int k = 0; k < shootsettings.powersList.Count; k++) {
				if (shootsettings.powersList [k].powerEnabled) {
					choosedPower = k;
					shootsettings.selectedPowerHud.gameObject.SetActive (true);
					shootsettings.selectedPowerHud.texture = currentPower.texture;
					currentPower = shootsettings.powersList [choosedPower];
				}
			}
		}
	}

	//if the player selects a power using the wheel and the mouse, set the power closed to the mouse
	public void setPower (Powers power)
	{
		for (int k = 0; k < shootsettings.powersList.Count; k++) {
			if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].Name == power.Name) {
				choosedPower = k;
				currentPower = shootsettings.powersList [choosedPower];
				shootsettings.selectedPowerHud.gameObject.SetActive (true);
				shootsettings.selectedPowerHud.texture = currentPower.texture;
			}
		}
	}

	float lastShoot;

	public void shootPower (bool state)
	{
		if (state) {
			if (!shootingBurst) {
				if (shootsettings.powerAmount > 0) {
					shooting = true;
				} else {
					shooting = false;
				}
				powerShoot (state);
				setLastTimeFired ();
			}
		} else {
			shooting = false;
			powerShoot (state);
		}
	}

	float minimumFireRate = 0.2f;

	//when the player is in aim mode, and press shoot, it is checked which power is selected, to create a bullet, push objects, etc...
	public void powerShoot (bool shootAtKeyDown)
	{
		if (((currentPower.useFireRate || currentPower.automatic) && Time.time < lastShoot + currentPower.fireRate) ||
		    (!currentPower.useFireRate && !currentPower.automatic && Time.time < lastShoot + minimumFireRate)) {
			return;
		} else {
			lastShoot = Time.time;
		}

		if ((isAimingPower () || firstPersonActive) && !playerControllerManager.isGravityPowerActive () && !laserActive && settings.shootEnabled && canMove) {
			if (shootsettings.powerAmount >= currentPower.amountPowerNeeded && amountPowersEnabled > 0 && !grabObjectsManager.isGrabbedObject ()) {

				checkWeaponAbility (shootAtKeyDown);

				//If the current projectile is homming type, check when the shoot button is pressed and release
				if ((currentPower.isHommingProjectile && shootAtKeyDown)) {
					homingProjectiles = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}
					
				if ((currentPower.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!currentPower.isHommingProjectile && !shootAtKeyDown)) {
					homingProjectiles = false;
					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if (currentPower.automatic && currentPower.useBurst) {
					if (!shootingBurst && currentPower.burstAmount > 0) {
						shootingBurst = true;
						currentBurstAmount = currentPower.burstAmount;
					}
				}

				shootZoneAudioSource.PlayOneShot (currentPower.shootSoundEffect);
				//every power uses a certain amount of the power bar	
				usePowerBar (currentPower.amountPowerNeeded);

				checkPowerShake ();

				if (currentPower.useEventToCall) {
					currentPower.eventToCall.Invoke ();
					return;
				}

				bool isLaunchingHomingProjectiles = (currentPower.isHommingProjectile && !shootAtKeyDown);
					
				if (!isLaunchingHomingProjectiles && !currentPower.powerWithAbility) {
					
					//if the player shoots, instantate the bullet and set its direction, velocity, etc...
					createShootParticles ();

					//use a raycast to check if there is any collider in the forward of the camera
					//if hit exits, then rotate the bullet in that direction, else launch the bullet in the camera direction
					currentProjectile = (GameObject)Instantiate (currentPower.projectile, shootsettings.shootZone.position, mainCameraTransform.rotation);

					Vector3 cameraDirection = mainCameraTransform.TransformDirection (Vector3.forward);

					bool armCrossingSurface = false;
					if (aimingInFirstPerson) {
						RaycastHit hitCamera;
						RaycastHit hitPower;
						if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hitCamera, Mathf.Infinity, layerToDamage)
						    && Physics.Raycast (shootsettings.shootZone.position, cameraDirection, out hitPower, Mathf.Infinity, layerToDamage)) {
							if (hitCamera.collider != hitPower.collider) {
								armCrossingSurface = true;
								//print ("crossing surface");
							} 
						}
					}

					if (!currentPower.launchProjectile) {
						if (!armCrossingSurface) {
							bool surfaceFound = false;
							if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
								if (hit.collider != playerCollider) {
									surfaceFound = true;
								} else {
									if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
										surfaceFound = true;
									}
								}
							}

							if (surfaceFound) {
								//Debug.DrawRay (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward) * hit.distance, Color.red);
								currentProjectile.transform.LookAt (hit.point);
							}
						}
					}

					if (currentPower.launchProjectile) {
						currentProjectile.GetComponent<Rigidbody> ().isKinematic = true;
						//if the vehicle has a gravity control component, and the current gravity is not the regular one, add an artifical gravity component to the projectile
						//like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle

						if (gravityManager.getCurrentNormal () != Vector3.up) {
							currentProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-gravityManager.getCurrentNormal ());
						}

						if (currentPower.useParableSpeed) {
							//get the ray hit point where the projectile will fall
							bool surfaceFound = false;
							if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
								if (hit.collider != playerCollider) {
									surfaceFound = true;
								} else {
									if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
										surfaceFound = true;
									}
								}
							}

							if (surfaceFound) {
								aimedZone = hit.point;
								objectiveFound = true;
							} else {
								objectiveFound = false;
							}
						}

						launchCurrentProjectile ();
					}
						
					//add spread to the projectile
					Vector3 spreadAmount = Vector3.zero;
					if (currentPower.useProjectileSpread) {
						spreadAmount = setProjectileSpread ();
						currentProjectile.transform.Rotate (spreadAmount);
					}

					projectileSystem currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

					if (currentProjectileSystem) {
						currentProjectileSystem.setProjectileInfo (setPowerProjectileInfo (currentPower));
					}

					if (currentPower.useRayCastShoot || armCrossingSurface) {
						Vector3 forwardDirection = cameraDirection;
						if (spreadAmount.magnitude != 0) {
							forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
						}

						bool surfaceFound = false;
						if (Physics.Raycast (mainCameraTransform.position, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
							if (hit.collider != playerCollider) {
								surfaceFound = true;
							} else {
								if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
									surfaceFound = true;
								}
							}
						}

						if (surfaceFound) {
							currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);
						} else {
							currentProjectileSystem.destroyProjectile ();
						}
					}

					if (currentPower.isSeeker) {
						setSeekerProjectileInfo (currentProjectile);
					}

					if (currentPower.applyForceAtShoot) {
						forceDirection = (mainCameraTransform.right * currentPower.forceDirection.x +
						mainCameraTransform.up * currentPower.forceDirection.y +
						mainCameraTransform.forward * currentPower.forceDirection.z) * currentPower.forceAmount;
						playerControllerManager.externalForce (forceDirection);
					}
				}

				activatePowerHandRecoil ();
			}

			// if the player is holding an object in the aim mode (not many in the normal mode) and press left button of the mouse
			//the gravity of this object is changed, sending the object in the camera direction, and the normal of the first surface that it touchs
			//will be its new gravity
			//to enable previous gravity of that object, grab again and change its gravity again, but this time aim to the actual ground with normal (0,1,0)
			if (grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.checkGrabbedObjectAction ();
			}
		}
	}

	public void checkWeaponAbility (bool keyDown)
	{
		if (currentPower.powerWithAbility) {
			if (keyDown) {
				if (currentPower.useDownButton) {
					if (currentPower.downButtonAction.GetPersistentEventCount () > 0) {
						currentPower.downButtonAction.Invoke ();
					}
				}
			} else {
				if (currentPower.useUpButton) {
					if (currentPower.upButtonAction.GetPersistentEventCount () > 0) {
						currentPower.upButtonAction.Invoke ();
					}
				}
			}
		}
	}

	public void activateSecondaryAction ()
	{
		if (currentPower.useSecondaryAction) {
			if (currentPower.secondaryAction.GetPersistentEventCount () > 0) {
				currentPower.secondaryAction.Invoke ();
			}
		}
	}

	bool shootingBurst;
	int currentBurstAmount;

	public void launchCurrentProjectile ()
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		Rigidbody currentProjectileRigidbody = currentProjectile.GetComponent<Rigidbody> ();
		currentProjectileRigidbody.isKinematic = false;
		if (currentPower.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone);
			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}
			currentProjectileRigidbody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			currentProjectileRigidbody.AddForce (currentPower.parableDirectionTransform.forward * currentPower.projectileSpeed, ForceMode.Impulse);
		}
	}

	//calculate the speed applied to the launched projectile to make a parable according to a hit point
	Vector3 getParableSpeed (Vector3 origin, Vector3 target)
	{
		//if a hit point is not found, return
		if (!objectiveFound) {
			return -Vector3.one;
		}

		//get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		//remove the Y axis value
		toTargetXZ -= transform.InverseTransformDirection (toTargetXZ).y * transform.up;
		float y = transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		//get the velocity accoring to distance ang gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		//create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;   

		//get direction of xz but with magnitude 1
		result *= v0xz;                     

		// set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= transform.InverseTransformDirection (result).y * transform.up;
		result += transform.up * v0y;
		return result;
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = currentPower.spreadAmount;
		Vector3 randomSpread = Vector3.zero;
		randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
		randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
		randomSpread.z = Random.Range (-spreadAmount, spreadAmount);
		return randomSpread;
	}

	//shoot to any object with the tag configured in the inspector, in case the option is enabled
	public void checkAutoShootOnTag ()
	{
		if (currentPower.autoShootOnTag) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, 
				    currentPower.maxDistanceToRaycast, currentPower.layerToAutoShoot)) {
				GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);
				if (target != null && target != gameObject) {
					if (currentPower.autoShootTagList.Contains (target.tag)) {
						shootPower (true);
					}
				} else {
					if (currentPower.autoShootTagList.Contains (hit.collider.gameObject.tag) ||
					    (currentPower.shootAtLayerToo &&
					    (1 << hit.collider.gameObject.layer & currentPower.layerToAutoShoot.value) == 1 << hit.collider.gameObject.layer)) {
						shootPower (true);
					}
				}
			}
		}
	}

	public void setSeekerProjectileInfo (GameObject projectile)
	{
		//get all the enemies in the scene
		List<GameObject> enemiesInFront = new List<GameObject> ();
		List<GameObject> fullEnemyList = new List<GameObject> ();

		for (int i = 0; i < currentPower.tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (currentPower.tagToLocate [i]);
			fullEnemyList.AddRange (enemiesList);
		}

		for (int i = 0; i < fullEnemyList.Count; i++) {
			//get those enemies which are not dead and in front of the camera
			if (!applyDamage.checkIfDead (fullEnemyList [i])) {

				if (usingScreenSpaceCamera) {
					screenPoint = mainCamera.WorldToViewportPoint (fullEnemyList [i].transform.position);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
				} else {
					screenPoint = mainCamera.WorldToScreenPoint (fullEnemyList [i].transform.position);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
				}

				//the target is visible in the screen
				if (targetOnScreen) {
					enemiesInFront.Add (fullEnemyList [i]);
				}
			}
		}

		for (int i = 0; i < enemiesInFront.Count; i++) {
			//for every enemy in front of the camera, use a raycast, if it finds an obstacle between the enemy and the camera, the enemy is removed from the list
			Vector3 direction = enemiesInFront [i].transform.position - shootsettings.shootZone.position;
			direction = direction / direction.magnitude;
			float distance = GKC_Utils.distance (enemiesInFront [i].transform.position, shootsettings.shootZone.position);

			bool surfaceFound = false;
			if (Physics.Raycast (shootsettings.shootZone.position, direction, out hit, distance, layerToDamage)) {
				if (hit.collider != playerCollider) {
					surfaceFound = true;
				} else {
					if (Physics.Raycast (hit.point + direction * 0.2f, direction, out hit, Mathf.Infinity, layerToDamage)) {
						surfaceFound = true;
					}
				}
			}

			if (surfaceFound) {
				if (!hit.transform.IsChildOf (enemiesInFront [i].transform) && hit.collider.gameObject != enemiesInFront [i]) {
					enemiesInFront.RemoveAt (i);
					i = i - 1;
				}
			}
		}

		//finally, get the enemy closest to the player
		float minDistance = Mathf.Infinity;
		for (int i = 0; i < enemiesInFront.Count; i++) {
			float currentDistance = GKC_Utils.distance (enemiesInFront [i].transform.position, transform.position);
			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				closestEnemy = enemiesInFront [i];
			}
		}

		if (closestEnemy) {
			Transform placeToShoot = applyDamage.getPlaceToShoot (closestEnemy);
			if (placeToShoot != null) {
				projectile.GetComponent<projectileSystem> ().setEnemy (placeToShoot.gameObject);
			} else {
				projectile.GetComponent<projectileSystem> ().setEnemy (closestEnemy);
			}
		}
		closestEnemy = null;
	}

	public void createShootParticles ()
	{
		if (currentPower.shootParticles) {
			GameObject shootParticles = (GameObject)Instantiate (currentPower.shootParticles, shootsettings.shootZone.position, Quaternion.LookRotation (mainCameraTransform.forward));
			shootParticles.transform.position = shootsettings.shootZone.position;
			//shootParticles.transform.SetParent (shootsettings.shootZone);
		}
	}

	public void setLastTimeFired ()
	{
		lastTimeFired = Time.time;
	}

	public float getLastTimeFired ()
	{
		return lastTimeFired;
	}

	//enable and disable the shield when the player want to stop attacks or when he touchs a laser
	public void activateShield ()
	{
		if (shootsettings.powerAmount > 0 && !laserActive && canMove && settings.shieldEnabled) {
			settings.shield.SetActive (!settings.shield.activeSelf);
			activatedShield = !activatedShield;
		}
	}

	//shoot the bullets and missiles catched by the shield
	public void shootEnemyProjectiles ()
	{
		if (usingPowers) {
			//check if a raycast hits a surface from the center of the screen to forward
			//to set the direction of the projectiles in the shield
			Vector3 direction = mainCameraTransform.TransformDirection (Vector3.forward);

			bool surfaceFound = false;
			if (Physics.Raycast (mainCameraTransform.position, direction, out hit, Mathf.Infinity, layerToDamage)) {
				if (hit.collider != playerCollider) {
					surfaceFound = true;
				} else {
					if (Physics.Raycast (hit.point + direction * 0.2f, direction, out hit, Mathf.Infinity, layerToDamage)) {
						surfaceFound = true;
					}
				}
			}

			if (surfaceFound) {
				direction = hit.point;
			}

			armorSurfaceManager.throwProjectilesStored (direction);
		}
	}

	//if the player dies, check if the player was aiming, grabbing and object, etc... and disable any necessary parameter
	public void death (bool state)
	{
		playerIsDead = state;

		if (state) {
			if (playerCameraManager.moveAwayActive) {
				playerCameraManager.moveAwayCamera ();
			} else {
				//check that the player is not in first person view to disable the aim mode
				if (!firstPersonActive) {
					deactivateAimMode ();
				}
			}

			if (playerControllerManager.isCrouching ()) {
				playerControllerManager.crouch ();
			}

			deactivateLaserForceField ();

			stopRun ();
		}
	}

	//functions to enable or disable the aim mode
	public void activateAimMode ()
	{
		if (!canFirePowersWithoutAiming || !aimingPowerFromShooting || (canFirePowersWithoutAiming && useAimCameraOnFreeFireMode)) {
			playerCameraManager.activateAiming (aimsettings.aimSide); 	
		}

		if (headLookWhenAiming) {
			IKSystemManager.setHeadLookState (true, headLookSpeed, headLookTarget);
		}

		playerControllerManager.enableOrDisableAiminig (true);				
		usingPowers = true;
		enableOrDisablePowerCursor (true);

		//enable the grab objects mode in aim mode
		if (grabObjectsManager) {
			grabObjectsManager.setAimingState (true);
		}

		//if the player is touching by a laser device, enable the laser in the player
		if (laserActive && !playerLaserGameObject.activeSelf) {
			playerLaserGameObject.SetActive (true);
		}

		//else disable the laser
		if (!laserActive && playerLaserGameObject.activeSelf) {
			playerLaserGameObject.SetActive (false);
		}

		IKSystemManager.changeArmState (1);

		if (!firstPersonActive && usePowerRotationPoint) {
			upperBodyRotationManager.setCurrentWeaponRotationPoint (powerRotationPoint, rotationPointInfo, -1);
			upperBodyRotationManager.setUsingWeaponRotationPointState (true);
		}

		upperBodyRotationManager.enableOrDisableIKUpperBody (true);

		if (useAimAssistInThirdPerson) {
			playerCameraManager.setCurrentLockedCameraCursor (cursorRectTransform);
			playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);

			if (!playerCameraManager.isCameraTypeFree ()) {
				if (useAimAssistInLockedCamera) {
					playerCameraManager.setLookAtTargetOnLockedCameraState ();
				}
			}

			playerCameraManager.setLookAtTargetState (true, null);
		}

		//enable the parable linerenderer
		checkParableTrayectory (true);

		if (!usingFreeFireMode) {
			if (useLowerRotationSpeedAimedThirdPerson) {
				playerCameraManager.changeRotationSpeedValue (verticalRotationSpeedAimedInThirdPerson, horizontalRotationSpeedAimedInThirdPerson);
			}
		}
	}

	public void deactivateAimMode ()
	{
		if (headLookWhenAiming) {
			IKSystemManager.setHeadLookState (false, headLookSpeed, headLookTarget);
		}

		playerCameraManager.deactivateAiming ();
		playerControllerManager.enableOrDisableAiminig (false);
		usingPowers = false;
		enableOrDisablePowerCursor (false);

		//disable the grab objects mode in aim mode, and drop any object that the player has grabbed
		if (grabObjectsManager) {
			grabObjectsManager.checkIfDropObjectIfNotPhysical (true);
		}

		playerLaserGameObject.SetActive (false);

		IKSystemManager.changeArmState (0);

		upperBodyRotationManager.enableOrDisableIKUpperBody (false);

		keepPower ();

		if (useAimAssistInThirdPerson) {
			playerCameraManager.setCurrentLockedCameraCursor (null);
			playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);
			playerCameraManager.setLookAtTargetState (false, null);
		}

		//disable the parable linerenderer
		checkParableTrayectory (false);

		if (locatedEnemies.Count > 0) {
			homingProjectiles = false;
			//remove the icons in the screen
			removeLocatedEnemiesIcons ();
		}

		if (useLowerRotationSpeedAimedThirdPerson) {
			playerCameraManager.setOriginalRotationSpeed ();
		}
	}

	public void fireHomingProjectiles ()
	{
		//check that the located enemies are higher that 0
		if (locatedEnemies.Count > 0) {
			
			createShootParticles ();

			print (locatedEnemies.Count);

			//shoot the missiles
			for (int i = 0; i < locatedEnemies.Count; i++) {
				Quaternion rot = mainCameraTransform.rotation;
				currentProjectile = (GameObject)Instantiate (currentPower.projectile, shootsettings.shootZone.position, rot);
				projectileSystem currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();
				currentProjectileSystem.setProjectileInfo (setPowerProjectileInfo (currentPower));
				currentProjectileSystem.setEnemy (locatedEnemies [i]);
				if (currentPower.useRayCastShoot) {
					Vector3 forwardDirection = mainCameraTransform.TransformDirection (Vector3.forward);
					Vector3 forwardPositon = mainCameraTransform.position;

					bool surfaceFound = false;
					if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
						if (hit.collider != playerCollider) {
							surfaceFound = true;
						} else {
							if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
								surfaceFound = true;
							}
						}
					}

					if (surfaceFound) {
						currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);
					}
				}
			}

			//remove the icons in the screen
			removeLocatedEnemiesIcons ();

			//decrease the value of the power bar
			shootZoneAudioSource.PlayOneShot (currentPower.shootSoundEffect);
			usePowerBar (currentPower.amountPowerNeeded);

			activatePowerHandRecoil ();
		}
		homingProjectiles = false;
	}

	public void activatePowerHandRecoil ()
	{
		if (currentPower.useRecoil) {
			IKSystemManager.startRecoil (currentPower.recoilSpeed, currentPower.recoilAmount);
		}
	}

	public void enableOrDisablePowerCursor (bool state)
	{
		//show a cursor in the center of the screen to aim when the player is going to launch some objects
		if (settings.cursor) {
			settings.cursor.SetActive (state);
			if (!state) {
				if (cursorRectTransform) {
					cursorRectTransform.localPosition = Vector3.zero;
				}
			}
		}
	}

	public void changePlayerMode (bool state)
	{
		enableOrDisablePowerCursor (state);
		checkParableTrayectory (state);
	}

	//change the camera view according to the situation
	public void changeTypeView ()
	{
		//in the aim mode, the player can choose which side to aim, left or right
		if (changeCameraSideActive && playerControllerManager.isPlayerAimingInThirdPerson () && !checkToKeepPowersAfterAimingPowerFromShooting && !aimingPowerFromShooting && !firstPersonActive) {
			setAimModeSide (true);
		}

		changeCameraToThirdOrFirstView ();
	}

	public void changeCameraToThirdOrFirstView ()
	{
		//in the normal mode, change camera from third to first and viceversa
		if (!playerControllerManager.isPlayerAimingInThirdPerson () && !scannerSystemManager.isScannerActivated ()) {

			deactivateAimMode ();

			gravityManager.changeCameraView ();

			if (playerCameraManager.isFirstPersonActive ()) {
				//change the place where the projectiles are instantiated to a place below the camera
				shootsettings.shootZone.SetParent (mainCameraTransform.transform);
				shootsettings.shootZone.localPosition = shootsettings.firstPersonShootPosition.localPosition;
			} else {
				if (scannerSystemManager.isScannerActivated ()) {
					scannerSystemManager.disableScanner ();
				}
				//change the place where the projectiles are instantiated back to the hand of the player
				shootsettings.shootZone.SetParent (aimsettings.handActive.transform);
				shootsettings.shootZone.localPosition = Vector3.zero;
			}

			shootsettings.shootZone.localRotation = Quaternion.identity;
			setAimOrKeepPowerState (playerCameraManager.isFirstPersonActive (), false);

			if (powersModeActive) {
				checkParableTrayectory (true);
				if (playerCameraManager.isFirstPersonActive ()) {
					enableOrDisablePowerCursor (true);
				}
			}
		}
	}

	//in the aim mode, the player can change the side to aim, left or right, moving the camera and changing the arm,
	//to configure the gameplay with the style of the player
	public void setAimModeSide (bool state)
	{
		int value;
		if (state) {
			value = (int)aimsettings.aimSide * (-1);
		} else {
			value = (int)aimsettings.aimSide;
		}

		//change to the right side, enabling the right arm
		if (value == 1) {
			aimsettings.handActive = aimsettings.rightHand;
			aimsettings.aimSide = sideToAim.Right;
			IKSystemManager.changeArmSide (true);
		}
		//change to the left side, enabling the left arm
		else {
			aimsettings.handActive = aimsettings.leftHand;
			aimsettings.aimSide = sideToAim.Left;
			IKSystemManager.changeArmSide (false);
		}

		//change the place, in this case, the hand, where the projectiles are instantiated
		if (state) {
			playerCameraManager.changeAimSide (value);
		}

		if (!playerCameraManager.isFirstPersonActive ()) {
			shootsettings.shootZone.SetParent (aimsettings.handActive.transform);
			shootsettings.shootZone.localPosition = Vector3.zero;
			shootsettings.shootZone.localRotation = Quaternion.identity;
		}
	}

	//enable disable the laser in the hand of the player, when he is in the range of one
	public void activateLaserForceField (Vector3 pos)
	{
		if (!laserAbilityEnabled) {
			return;
		}

		//print ("enable laser force field");
		activatedShield = false;
		laserActive = true;

		if (laserActive) {
			if (!settings.shield.activeSelf) {
				settings.shield.SetActive (true);
			}
			laserPosition = pos;
			if (usingPowers) {
				if (!playerLaserGameObject.activeSelf) {
					if (powersModeActive) {
						playerLaserGameObject.SetActive (true);
					}
				}
			}
		}

		if (playerLaserGameObject.activeSelf) {
			laserPlayerManager.setLaserInfo (lasertype, currentLaser, laserPosition);
		}
	}

	public void deactivateLaserForceField ()
	{
		//print ("disable laser force field");
		laserActive = false;
		if (settings.shield.activeSelf) {
			settings.shield.SetActive (false);
		}

		if (playerLaserGameObject.activeSelf) {
			playerLaserGameObject.SetActive (false);
		}

		laserPlayerManager.removeLaserInfo ();
	}

	//get the laser device that touch the player, not enemy lasers, and if the laser reflects in other surfaces or not
	public void setLaser (GameObject l, laserDevice.laserType type)
	{
		currentLaser = l;
		lasertype = type;
	}

	//set the number of refractions in the laser in another function
	public void setValue (int value)
	{
		laserPlayerManager.reflactionLimit = value + 1;
	}

	public bool isUsingPowers ()
	{
		return usingPowers;
	}

	public void checkIfCanRun ()
	{
		if ((playerControllerManager.moveInput.magnitude > 0 && !playerControllerManager.isGravityPowerActive () && playerControllerManager.isPlayerOnGround ()
		    && !gravityManager.isSearchingSurface ()) || running) {
			//avoid to run backwards
			if (playerControllerManager.playerCanRunNow ()) {
				//check if he can run while crouching
				if (!playerControllerManager.isCrouching () || (settings.runOnCrouchEnabled && playerControllerManager.isCrouching ())) {
					if (!wallWalk) {
						//check the amount of time that the button is being pressed
						buttonTimer += Time.deltaTime;
						if (!running) {
							run ();
							lastTimeRun = buttonTimer;
							normalOrig = gravityManager.getCurrentRotatingNormal ();
						}

						if (wallWalkEnabled && buttonTimer > 0.5f) {
							if (!gravityManager.isCurcumnavigating ()) {
								wallWalk = true;	

								if (gravityManager.isCharacterRotatingToSurface ()) {
									gravityManager.stopRotateToSurfaceCoroutine ();
								}
							}
							return;
						}
					}
				}
			}
		}
	}

	public void checkIfStopRun ()
	{
		if (running && (buttonTimer > 0.5f || buttonTimer - lastTimeRun > 0.12f)) {
			
			stopRun ();

			buttonTimer = 0;
			lastTimeRun = 0;
		}
	}

	//if the player runs, a set of parameters are changed, like the speed of movement, animation, jumppower....
	public void run ()
	{
		if (wallWalkEnabled) {
			if (settings.trailsActive && !firstPersonActive) {
				for (int j = 0; j < trails.Length; j++) {
					trails [j].enabled = true;
					trails [j].time = 1;
				}
				trailTimer = -1;
			}

			if (playerRenderer != null && settings.runMat) {
				Material[] allMats = playerRenderer.materials;
				for (int m = 0; m < mats.Length; m++) {
					allMats [m] = settings.runMat;
				}
				playerRenderer.materials = allMats;
			}
		}

		running = true;

		playerControllerManager.setRunnningState (true);

		weaponsManager.setRunningState (true);
	}

	//when the player stops running, those parameters back to their normal values
	public void stopRun ()
	{
		if (settings.trailsActive) {
			trailTimer = 2;
		}
			
		if (playerRenderer != null) {
			playerRenderer.materials = auxMats;
		}
			
		if (wallWalk) {
			if (stopGravityAdherenceWhenStopRun) {
				if (gravityManager.getCurrentRotatingNormal () != normalOrig) {
					if (gravityManager.isCharacterRotatingToSurface ()) {
						gravityManager.stopRotateToSurfaceWithOutParentCoroutine ();
					}

					gravityManager.checkRotateToSurface (normalOrig, 2);
				}
				playerControllerManager.setCurrentNormalCharacter (normalOrig);
			} else {
				if (gravityManager.isUsingRegularGravity ()) {
					gravityManager.changeColor (false);
				} else {
					gravityManager.changeColor (true);
				}
			}
			wallWalk = false;
		}

		running = false;

		playerControllerManager.setRunnningState (false);

		weaponsManager.setRunningState (false);
	}

	public void checkIfDropObject (GameObject objectToCheck)
	{
		for (int j = 0; j < grabbedObjectList.Count; j++) {
			if (grabbedObjectList [j].objectToMove == objectToCheck) {
				dropObject (grabbedObjectList [j], Vector3.zero);
				grabbedObjectList [j].objectToMove = null;
				return;
			}
		}
	}

	public void checkPowerShake ()
	{
		if (!currentPower.useShake) {
			return;
		}
		if (currentPower.sameValueBothViews) {
			headBobManager.setShotShakeState (currentPower.thirdPersonShakeInfo);
		} else {
			if (firstPersonActive && currentPower.useShakeInFirstPerson) {
				headBobManager.setShotShakeState (currentPower.firstPersonShakeInfo);
			}

			if (!firstPersonActive && currentPower.useShakeInThirdPerson) {
				headBobManager.setShotShakeState (currentPower.thirdPersonShakeInfo);
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<otherPowers> ());
		#endif
	}

	public void getImpactListInfo ()
	{
		if (!impactDecalManager) {
			impactDecalManager = FindObjectOfType<decalManager> ();
		} 
		if (impactDecalManager) {
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];
			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}
			updateComponent ();
		}
	}

	public void setPowersModeState (bool state)
	{
		powersModeActive = state;

		if (!powersModeActive && startInitialized) {
			disableFreeFireModeState ();
		}

		checkEventsOnStateChange (powersModeActive);
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}

	public void checkIfChangePlayerMode ()
	{
		if (isUsingPowers ()) {
			changePlayerMode (true);
		}
	}

	public void checkIfChangePlayerModeAccordingToViewType ()
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			changePlayerMode (true);
		} else {
			changePlayerMode (false);
		}
	}

	public bool isPowersModeActive ()
	{
		return powersModeActive;
	}

	public void disableFreeFireModeState ()
	{
		aimingPowerFromShooting = false;

		checkToKeepPowersAfterAimingPowerFromShooting = false;

		checkToKeepPowersAfterAimingPowerFromShooting2_5d = false;

		playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerControllerManager.setUsingFreeFireModeState (false);

		usingFreeFireMode = false;

		weaponAimedFromFiringActive = false;
	}


	//CALL INPUT FUNCTIONS
	public void inputReturnProjectiles ()
	{
		//the bullets and missiles from the enemies are stored in the shield, so if the player press the right button of the mouse
		//the shoots are sent to its owners if they still alive, else, the shoots are launched in the camera direction
		if (!playerCurrentlyBusy && settings.shield.activeSelf && activatedShield && !laserActive) {
			shootEnemyProjectiles ();
		}
	}

	public void inputActivateShield ()
	{
		//enable or disable the shield
		if (!playerCurrentlyBusy && powersModeActive) {
			activateShield ();
		}
	}

	public void inputNextOrPreviousPowerByButton (bool setNextPower)
	{
		if (!changePowersWithKeysActive) {
			return;
		}

		//select the power using the mouse wheel or the change power buttons
		if (!playerCurrentlyBusy && powersModeActive) {
			if (setNextPower) {
				chooseNextPower ();
			} else {
				choosePreviousPower ();
			}
		}
	}

	public void inputNextOrPreviousPowerByMouse (bool setNextPower)
	{
		if (!changePowersWithMouseWheelActive) {
			return;
		}

		if (!playerCurrentlyBusy && powersModeActive) {
			if ((!grabObjectsManager.canUseZoomWhileGrabbed && grabObjectsManager.isGrabbedObject ()) || !grabObjectsManager.isGrabbedObject ()) {
				if (setNextPower) {
					chooseNextPower ();
				} else {
					choosePreviousPower ();
				}
			}
		}
	}

	public void inputHoldOrReleaseShootPower (bool holdingButton)
	{
		//according to the selected power, when the left button of the mouse is pressed, that power is activated
		if (!playerCurrentlyBusy && powersModeActive && !playerIsDead && playerControllerManager.canPlayerMove ()) {
			if (holdingButton) {

				if (canFirePowersWithoutAiming && !firstPersonActive) {
					if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
						if (!aimingInThirdPerson || checkToKeepPowersAfterAimingPowerFromShooting) {
							playerControllerManager.setUsingFreeFireModeState (true);

							usingFreeFireMode = true;

							aimingPowerFromShooting = true;

							weaponAimedFromFiringActive = true;

							checkToKeepPowersAfterAimingPowerFromShooting = false;
						}

						if (!aimingInThirdPerson) {
							playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

							aimModeInputPressed = false;

							useAimMode ();
						}
					} else {
						if (!aimingInThirdPerson) {
							checkToKeepPowersAfterAimingPowerFromShooting2_5d = true;
							useAimMode ();
						}
					}
				}

				if (currentPower.automatic) {
					if (currentPower.useBurst) {
						shootPower (true);
					}
				} else {
					shootPower (true);
				}
			} else {
				shootPower (false);

				if (homingProjectiles) {
					//if the button to shoot is released, shoot a homing projectile for every located enemy
					fireHomingProjectiles ();
				}

				if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
					if (aimingInThirdPerson && canFirePowersWithoutAiming && aimingPowerFromShooting && !firstPersonActive) {
						checkToKeepPowersAfterAimingPowerFromShooting = true;
						playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (true);
					}
				}

				aimingPowerFromShooting = false;

				playerControllerManager.setUsingFreeFireModeState (false);

				usingFreeFireMode = false;
			}
		}
	}

	public void inputHoldShootPower ()
	{
		if (!playerCurrentlyBusy && powersModeActive && !playerIsDead && playerControllerManager.canPlayerMove ()) {

			if (canFirePowersWithoutAiming && !firstPersonActive) {
				if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
					if (!aimingInThirdPerson || checkToKeepPowersAfterAimingPowerFromShooting) {
						playerControllerManager.setUsingFreeFireModeState (true);

						usingFreeFireMode = true;

						aimingPowerFromShooting = true;

						weaponAimedFromFiringActive = true;

						checkToKeepPowersAfterAimingPowerFromShooting = false;
					}

					if (!aimingInThirdPerson) {

						playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

						aimModeInputPressed = false;

						useAimMode ();
					}
				} else {
					if (!aimingInThirdPerson) {
						checkToKeepPowersAfterAimingPowerFromShooting2_5d = true;
						useAimMode ();
					}
				}
			}

			if (currentPower.automatic) {
				if (!currentPower.useBurst) {
					shootPower (true);
				}
			}
		}
	}

	public void inputAimPower ()
	{
		//activate or deactivate the aim mode, checking that the gravity power is active and nither the first person mode
		if (!playerCurrentlyBusy && powersModeActive) {
			aimModeInputPressed = !aimModeInputPressed;

			useAimMode ();
		}
	}

	public void inputChangeCameraView ()
	{
		if (!playerCurrentlyBusy && canMove && playerCameraManager.isCameraTypeFree () && playerCameraManager.isChangeCameraViewEnabled ()) {
			if (weaponsManager.isEditinWeaponAttachments ()) {
				return;
			}
			changeTypeView ();
		}
	}

	public void inputStartToRun ()
	{
		//check if the player is moving and he is not using the gravity power
		//in that case according to the duration of the press key, the player will only run or run and change his gravity
		//also in the new version of the asset also check if the touch control is being used
		if (!playerCurrentlyBusy && canMove && playerControllerManager.isSprintEnabled()) {
			checkIfCanRun ();
		}
	}

	public void inputStopToRun ()
	{
		//if the run button is released, stop the run power, if the power was holding the run button to adhere to other surfaces, else the 
		//run button has to be pressed again to stop the run power
		if (!playerCurrentlyBusy && canMove) {
			checkIfStopRun ();
		}
	}

	public void inputHoldTeleport ()
	{
		if (!playerCurrentlyBusy && teleportingEnabled && !playerControllerManager.isPlayerUsingInput () && !gravityManager.isSearchingSurface () &&
		    (!playerControllerManager.isPlayerOnZeroGravityMode () || canTeleportOnZeroGravity)) {
			searchingForTeleport = true;
			lastTimeTeleportButtonPressed = Time.time;
		}
	}

	public void inputReleaseTeleport ()
	{
		if (!playerCurrentlyBusy && teleportingEnabled) {
			if (teleportCanBeExecuted) {
				if (useTeleporIfSurfaceNotFound || !useTeleporIfSurfaceNotFound && teleportSurfaceFound) {
					teleportPlayer (transform, currentTeleportPosition, true, true, teleportSpeed);
				} else {
					stopTeleporting ();
				}
				if (useBulletTimeOnTeleport) {
					timeBulletManager.setBulletTimeState (false, 1); 
				}
			}
			teleportCanBeExecuted = false;

			searchingForTeleport = false;
			if (useTeleportMark) {
				teleportMark.SetActive (false);
			}
		}
	}

	public void inputGrabObjects ()
	{
		//grab and carry objets in both sides of the player, every objetc will translate to the closest side of the player, left or right
		//this mode is only when the player press E in the normal mode, in the aim mode, the player only will grab one object at the same time
		if (!playerCurrentlyBusy && (powersModeActive || (!playerControllerManager.isPlayerAiming () && canGrabObjectsUsingWeapons))) {
			if (!grabObjectsManager.isCarryingPhysicalObject () && !carryingObjects && !isAimingPower () && canMove && settings.grabObjectsEnabled && !usingPowers
			    && !grabObjectsManager.physicalObjectToGrabFound ()) {

				print (grabObjectsManager.physicalObjectToGrabFound ());

				carryObjectsTransformAnimation.Play ("grabObjects");
				grabCloseObjects ();
				currentForceToLaunchObjects = 0;
			}
		}
	}

	public void inputHoldToLaunchObjects ()
	{
		//the objects can be dropped or launched, according to the duration of the key press, in the camera direction
		if (!playerCurrentlyBusy && carryingObjects && canMove) {
			addForceToLaunchObjects ();
		}
	}

	public void inputReleaseToLaunchObjects ()
	{
		//drop or thrown the objects
		if (!playerCurrentlyBusy && carryingObjects && canMove) {
			dropObjects ();
		}
	}

	public void inputActivateSecondaryAction ()
	{
		if (!playerCurrentlyBusy && powersModeActive) {
			activateSecondaryAction ();
		}
	}

	//Set abilities and powers enabled and disable state, so they can be unlocked if the player activates them or disable if the situation needs it
	public void setRunEnabledState (bool state)
	{
		playerControllerManager.setSprintEnabledState (state);
	}

	public void setWalkWallEnabledState (bool state)
	{
		wallWalkEnabled = state;
	}

	public void setShieldEnabledState (bool state)
	{
		settings.shieldEnabled = state;
	}

	public void setGrabObjectsEnabledState (bool state)
	{
		settings.grabObjectsEnabled = state;
	}

	public void setShootEnabledState (bool state)
	{
		settings.shootEnabled = state;
	}

	public void setTeleportingEnabledState (bool state)
	{
		teleportingEnabled = state;
	}

	public void setLaserAbilityEnabledState (bool state)
	{
		laserAbilityEnabled = state;
	}

	public void enableRegularPowerListElement (string powerName)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].Name == powerName) {
				if (!shootsettings.powersList [i].powerEnabled) {
					shootsettings.powersList [i].powerEnabled = true;
					powersManager.enableOrDisablePowerSlot (powerName, true);

					updateAmountPowersEnabled ();

					choosedPower = i;

					powerChanged ();

					shootsettings.selectedPowerHud.gameObject.SetActive (true);

					return;
				}
			}
		}
	}

	public void disableRegularPowerListElement (string powerName)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].Name == powerName) {
				if (shootsettings.powersList [i].powerEnabled) {
					shootsettings.powersList [i].powerEnabled = false;
					powersManager.enableOrDisablePowerSlot (powerName, false);

					updateAmountPowersEnabled ();

					return;
				}
			}
		}
	}

	public void updateAmountPowersEnabled ()
	{
		amountPowersEnabled = 0;
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled) {
				if (amountPowersEnabled + 1 <= shootsettings.powersSlotsAmount) {
					amountPowersEnabled++;
				}
			}
		}
	}

	public int getNumberOfPowersAvailable ()
	{
		int numberOfPowersAvailable = 0;
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled) {
				numberOfPowersAvailable++;
			}
		}
		return numberOfPowersAvailable;
	}

	public void setFirstPowerAvailable ()
	{
		if (amountPowersEnabled > 0) {
			for (int i = 0; i < shootsettings.powersList.Count; i++) {
				if (shootsettings.powersList [i].powerEnabled) {
					choosedPower = i;
		
					powerChanged ();
					return;
				}
			}
		} else {
			shootsettings.selectedPowerHud.texture = null;
			shootsettings.selectedPowerHud.gameObject.SetActive (false);
			shootsettings.selectedPowerIcon.texture = null;
		}
	}

	public void setSelectedPowerIconState (bool state)
	{
		if (shootsettings.selectedPowerIcon) {
			shootsettings.selectedPowerIcon.gameObject.SetActive (state);
		}
	}

	public Powers getCurrentPower ()
	{
		return currentPower;
	}

	public void enableOrDisableAllPowers (bool state)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			shootsettings.powersList [i].powerEnabled = state;
		}
		updateComponent ();
	}

	public void setMeshCharacter (SkinnedMeshRenderer currentMeshCharacter)
	{
		playerRenderer = currentMeshCharacter.GetComponent<Renderer> ();

		updateComponent ();
	}

	//draw the touch zone of the panel that allow to change the choosed power, located in the upper left corner
	//you can see it in the left upper corner of hudAndMenus object in the hierachyby selecting the player controller and set the scene window
	//also you can check its size
	#if UNITY_EDITOR
	//draw the lines of the pivot camera in the editor
	void OnDrawGizmos ()
	{
		if (!shootsettings.showGizmo) {
			return;
		}

		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (shootsettings.showGizmo) {
			if (!EditorApplication.isPlaying) {
				setHudZone ();
			}
			Gizmos.color = shootsettings.gizmoColor;
			Vector3 touchZone = new Vector3 (touchZoneRect.x + touchZoneRect.width / 2f, touchZoneRect.y + touchZoneRect.height / 2f, shootsettings.hudZone.transform.position.z);
			Gizmos.DrawWireCube (touchZone, new Vector3 (shootsettings.touchZoneSize.x, shootsettings.touchZoneSize.y, 0f));
		}
	}
	#endif
	//get the correct size of the rect
	void setHudZone ()
	{
		if (shootsettings.hudZone) {
			touchZoneRect = new Rect (shootsettings.hudZone.transform.position.x - shootsettings.touchZoneSize.x / 2f, 
				shootsettings.hudZone.transform.position.y - shootsettings.touchZoneSize.y / 2f, shootsettings.touchZoneSize.x, shootsettings.touchZoneSize.y);
		}
	}

	[System.Serializable]
	public class powersSettings
	{
		public bool runOnCrouchEnabled;
		public bool aimModeEnabled;
		public bool shieldEnabled;
		public bool grabObjectsEnabled;
		public bool shootEnabled;
		public bool changePowersEnabled;
		//if runmat and body are not set, the player will not change his materials, but everything still working properly
		//also if trailsactive is false, the trails will not be activated
		public GameObject cursor;
		public Material runMat;
		public LayerMask layer;
		public bool trailsActive = true;
		public Slider slider;
		public Slider powerBar;

		public float grabRadius = 10;
		public List< string> ableToGrabTags = new List< string> ();
		public PhysicMaterial highFrictionMaterial;
		public GameObject shield;
	}

	[System.Serializable]
	public class aimSettings
	{
		public sideToAim aimSide;
		public GameObject leftHand;
		public GameObject rightHand;
		public bool aimSideLeft;
		public GameObject handActive;
	}

	public enum sideToAim
	{
		Left = -1,
		Right = 1
	}

	[System.Serializable]
	public class shootSettings
	{
		public List<Powers> powersList = new List<Powers> ();

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public LayerMask targetToDamageLayer;
		public float powerAmount;
		public int powersSlotsAmount;
		public float powerUsedByShield;
		public int homingProjectilesMaxAmount;

		public RawImage selectedPowerIcon;
		public RawImage selectedPowerHud;
		public Transform shootZone;
		public Transform firstPersonShootPosition;
		public Vector2 touchZoneSize = new Vector2 (3, 3);
		public float minSwipeDist = 20;
		public bool touching;
		public GameObject hudZone;
		public bool showGizmo;
		public Color gizmoColor;
	}

	[System.Serializable]
	public class Powers
	{
		public string Name;
		public GameObject projectile;
		public int numberKey;
		public bool useRayCastShoot;

		public bool useRaycastCheckingOnRigidbody;

		public Texture texture;
		public bool powerEnabled = true;
		public bool powerAssigned;
		public float amountPowerNeeded;

		public bool useRecoil = true;
		public float recoilSpeed = 10;
		public float recoilAmount = 0.1f;

		public bool shootAProjectile;
		public bool launchProjectile;

		public bool projectileWithAbility;

		public bool powerWithAbility;
		public bool useDownButton;
		public UnityEvent downButtonAction;
		public bool useHoldButton;
		public UnityEvent holdButtonAction;
		public bool useUpButton;
		public UnityEvent upButtonAction;

		public bool useSecondaryAction;
		public UnityEvent secondaryAction;

		public bool automatic;

		public bool useBurst;
		public int burstAmount;

		public bool useFireRate;
		public float fireRate;
		public float projectileDamage;
		public float projectileSpeed;

		public bool useProjectileSpread;
		public float spreadAmount;

		public bool isExplosive;
		public bool isImplosive;
		public float explosionForce;
		public float explosionRadius;
		public bool useExplosionDelay;
		public float explosionDelay;
		public float explosionDamage;
		public bool pushCharacters;
		public bool canDamageProjectileOwner;
		public bool applyExplosionForceToVehicles = true;
		public float explosionForceToVehiclesMultiplier = 0.2f;

		public GameObject scorch;
		public float scorchRayCastDistance;

		public bool useEventToCall;
		public UnityEvent eventToCall;

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public bool applyForceAtShoot;
		public Vector3 forceDirection;
		public float forceAmount;

		public bool isHommingProjectile;
		public bool isSeeker;
		public float waitTimeToSearchTarget;
		public int homingProjectilesMaxAmount;

		public float impactForceApplied;
		public ForceMode forceMode;
		public bool applyImpactForceToVehicles;
		public float impactForceToVehiclesMultiplier = 1;

		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;

		public GameObject shootParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		public bool killInOneShot;

		public bool useDisableTimer;
		public float noImpactDisableTimer;
		public float impactDisableTimer;

		public List<string> tagToLocate = new List<string> ();
		public string locatedEnemyIconName = "Homing Located Enemy";

		public bool activateLaunchParableThirdPerson;
		public bool activateLaunchParableFirstPerson;
		public bool useParableSpeed;
		public Transform parableDirectionTransform;

		public bool adhereToSurface;
		public bool adhereToLimbs;

		public bool useGravityOnLaunch;
		public bool useGraivtyOnImpact;

		public bool breakThroughObjects;
		public bool infiniteNumberOfImpacts;
		public int numberOfImpacts;
		public bool canDamageSameObjectMultipleTimes;

		public int impactDecalIndex;
		public string impactDecalName;

		public bool useShake;
		public bool useShakeInFirstPerson;
		public bool useShakeInThirdPerson;
		public bool sameValueBothViews;
		public IKWeaponSystem.weaponShotShakeInfo thirdPersonShakeInfo;
		public IKWeaponSystem.weaponShotShakeInfo firstPersonShakeInfo;
		public bool showShakeSettings;

		public bool useMuzzleFlash;
		public Light muzzleFlahsLight;
		public float muzzleFlahsDuration;

		public bool damageTargetOverTime;
		public float damageOverTimeDelay;
		public float damageOverTimeDuration;
		public float damageOverTimeAmount;
		public float damageOverTimeRate;
		public bool damageOverTimeToDeath;
		public bool removeDamageOverTimeState;

		public bool sedateCharacters;
		public float sedateDelay;
		public bool useWeakSpotToReduceDelay;
		public bool sedateUntilReceiveDamage;
		public float sedateDuration;

		public bool pushCharacter;
		public float pushCharacterForce;
		public float pushCharacterRagdollForce;
	}

	[System.Serializable]
	public class grabbedObject
	{
		public GameObject objectToMove;
		public Transform objectToFollow;
		public string objectTag;
		public int objectLayer;
		public Rigidbody mainRigidbody;
	}

	[System.Serializable]
	public class powerInfo
	{
		public string Name;
		public UnityEvent eventToEnable;
		public UnityEvent eventToDisable;
	}
}