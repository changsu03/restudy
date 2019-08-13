using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerWeaponsManager : MonoBehaviour
{
	public bool carryingWeaponInThirdPerson;
	public bool carryingWeaponInFirstPerson;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool shootingSingleWeapon;

	public bool shootingRightWeapon;
	public bool shootingLeftWeapon;

	public int weaponsSlotsAmount;

	public bool dropCurrentWeaponWhenDie;
	public bool dropAllWeaponsWhenDie;
	public bool drawWeaponWhenResurrect;

	public bool dropWeaponsOnlyIfUsing;
	public GameObject weaponsHUD;

	public GameObject singleWeaponHUD;

	public GameObject dualWeaponHUD;

	public string currentWeaponName;

	public string currentRighWeaponName;
	public string currentLeftWeaponName;

	public Text currentWeaponNameText;
	public Text currentWeaponAmmoText;
	public Slider ammoSlider;
	public RawImage currentWeaponIcon;

	public GameObject attachmentPanel;
	public Text attachmentAmmoText;
	public RawImage currentAttachmentIcon;

	public Text currentRightWeaponAmmoText;
	public GameObject rightAttachmentPanel;
	public Text rigthAttachmentAmmoText;
	public RawImage currentRightAttachmentIcon;

	public Text currentLeftWeaponAmmoText;
	public GameObject leftAttachmentPanel;
	public Text leftAttachmentAmmoText;
	public RawImage currentLeftAttachmentIcon;

	public Transform weaponsParent;
	public Transform weaponsTransformInFirstPerson;
	public Transform weaponsTransformInThirdPerson;
	public Transform thirdPersonParent;
	public Transform firstPersonParent;
	public Transform cameraController;
	public Camera weaponsCamera;
	public string weaponsLayer;
	public Vector2 touchZoneSize;
	public float minSwipeDist;
	public bool touching;
	public bool showGizmo;
	public Color gizmoColor;

	public bool anyWeaponAvailable;

	public List<IKWeaponSystem> weaponsList = new List<IKWeaponSystem> ();
	public IKWeaponSystem currentIKWeapon;
	public playerWeaponSystem currentWeaponSystem;

	public LayerMask targetToDamageLayer;
	public LayerMask targetForScorchLayer;
	public bool changeToNextWeaponWhenDrop;
	public bool canDropWeapons;
	public float dropWeaponForceThirdPerson;
	public float dropWeaponForceFirstPerson;

	public bool holdDropButtonToIncreaseForce;
	public float dropIncreaseForceSpeed;
	public float maxDropForce;
	bool holdingDropButtonToIncreaseForce;
	float currentDropForce;
	float lastTimeHoldDropButton;

	public Transform rightHandTransform;
	public Transform leftHandTransform;
	public bool startGameWithCurrentWeapon;
	public bool drawInitialWeaponSelected = true;

	public bool startGameWithDualWeapons;

	public bool usedByAI;

	public GameObject weaponCursor;
	public RectTransform cursorRectTransform;
	public GameObject weaponCursorRegular;
	public GameObject weaponCursorAimingInFirstPerson;
	public GameObject weaponCursorUnableToShoot;
	public GameObject weaponCustomReticle;
	public GameObject swipeCenterPosition;

	public menuPause pauseManager;

	public bool startWithFirstWeaponAvailable;

	public string weaponToStartName;
	public string[] avaliableWeaponList;
	public int weaponToStartIndex;

	public string rightWeaponToStartName;
	public string leftWeaponToStartName;
	public int rightWeaponToStartIndex;
	public int leftWeaponToStartIndex;

	public float extraRotation;
	public float targetRotation;

	public bool useAimAssistInThirdPerson;
	public bool useAimAssistInFirstPerson;
	public bool useMaxDistanceToCameraCenterAimAssist;
	public float maxDistanceToCameraCenterAimAssist;

	public bool useAimAssistInLockedCamera = true;

	public float aimAssistLookAtTargetSpeed = 4;

	public bool setWeaponWhenPicked;

	public bool canGrabObjectsCarryingWeapons;

	public bool changeToNextWeaponIfAmmoEmpty;

	public List<weaponPocket> weaponPocketList = new List<weaponPocket> ();

	public bool weaponsModeActive;
	public bool drawKeepWeaponWhenModeChanged;

	public bool storePickedWeaponsOnInventory;
	public bool drawWeaponWhenPicked;
	public bool changeToNextWeaponWhenUnequipped;
	public bool changeToNextWeaponWhenEquipped;
	public bool notActivateWeaponsAtStart;

	public int choosedWeapon = 0;
	public int chooseDualWeaponIndex = -1;

	//Inspector variables
	public bool showSettings;
	public bool showElementSettings;
	public bool showWeaponsList;
	public bool showDebugSettings;

	public bool changingWeapon;
	public bool keepingWeapon;

	public bool changingDualWeapon;
	public bool changingSingleWeapon;

	public bool canFireWeaponsWithoutAiming;
	bool aimingWeaponFromShooting;
	public bool useAimCameraOnFreeFireMode;

	public bool drawWeaponIfFireButtonPressed;

	public bool drawAndAimWeaponIfFireButtonPressed;

	public bool keepWeaponAfterDelayThirdPerson;
	public bool keepWeaponAfterDelayFirstPerson;
	public float keepWeaponDelay;
	float lastTimeWeaponUsed;

	public bool useQuickDrawWeapon;

	public bool usingFreeFireMode;
	public float timeToStopAimAfterStopFiring = 0.85f;

	public bool aimModeInputPressed;

	public bool canJumpWhileAimingThirdPerson = true;
	public bool canAimOnAirThirdPerson = true;
	public bool stopAimingOnAirThirdPerson;

	public GameObject weaponsMessageWindow;
	[TextArea (1, 5)] public string cantDropCurrentWeaponMessage;
	[TextArea (1, 5)] public string cantPickWeaponMessage;
	[TextArea (1, 5)] public string cantPickAttachmentMessage;
	public float weaponMessageDuration;

	public bool loadCurrentPlayerWeaponsFromSaveFile;
	public bool saveCurrentPlayerWeaponsToSaveFile;

	public bool canMarkTargets;
	public playerScreenObjectivesSystem playerScreenObjectivesManager;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public playerCamera playerCameraManager;
	public headBob headBobManager;
	public inputManager input;
	public IKSystem IKManager;
	public playerController playerManager;
	public otherPowers powersManager;
	public upperBodyRotationSystem upperBodyRotationManager;
	public headTrack headTrackManager;
	public grabObjects grabObjectsManager;
	public gameManager gameSystemManager;
	public inventoryManager playerInventoryManager;
	public inventoryListManager mainInventoryListManager;
	public usingDevicesSystem usingDevicesManager;
	public Collider mainCollider;
	public ragdollActivator ragdollManager;
	public Transform mainCameraTransform;
	public Camera mainCamera;
	public weaponListManager mainWeaponListManager;

	public bool pivotPointRotationActive;
	public float pivotPointRotationSpeed = 6;

	public bool runWhenAimingWeaponInThirdPerson;
	public bool stopRunIfPreviouslyNotRunning;
	bool runningPreviouslyAiming;

	List<persistanceWeaponInfo> persistanceWeaponInfoList = new List<persistanceWeaponInfo> ();

	Vector3 swipeStartPos;
	Quaternion originalWeaponsParentRotation;
	float originalFov;
	float originalWeaponsCameraFov;
	float lastTimeFired;

	bool isThirdPersonView;
	bool touchPlatform;

	Coroutine cameraFovCoroutine;
	Touch currentTouch;
	Rect touchZoneRect;

	RaycastHit hit;
	Vector3 originalWeaponsPositionThirdPerson;
	Quaternion originalWeaponsRotationThirdPerson;
	Vector3 originalWeaponsPositionFirstPerson;
	Quaternion originalWeaponsRotationFirstPerson;
	bool playerIsDead;

	GameObject currentWeaponGameObject;
	Coroutine changeExtraRotation;

	bool aimWhenItIsReady;
	bool weaponCursorRegularPreviouslyEnabled;
	bool weaponCursorAimingInFirstPersonPreviouslyEnabled;

	bool usingDevice;
	bool carryingPhysicalObject;

	//Attachments variables
	public bool openWeaponAttachmentsMenuEnabled = true;
	public bool setFirstPersonForAttachmentEditor;
	public bool useUniversalAttachments;
	public bool editingWeaponAttachments;

	weaponAttachmentSystem currentWeaponAttachmentSystem;
	weaponAttachmentSystem currentRightWeaponAttachmentSystem;
	weaponAttachmentSystem currentLeftWeaponAttachmentSystem;

	bool usingSight;
	bool gamePaused;
	playerWeaponSystem currentWeaponSystemWithAttachment;

	public bool changeWeaponsWithNumberKeysActive = true;
	public bool changeWeaponsWithMouseWheelActive = true;
	public bool changeWeaponsWithKeysActive = true;

	bool cursorLocked = true;

	bool playerCurrentlyBusy;

	IKWeaponSystem IKWeaponToDrop;
	IKWeaponSystem currentIKWeaponBeforeCheckPockets;

	bool startInitialized;

	bool running;

	bool canMove;

	bool checkToKeepWeaponAfterAimingWeaponFromShooting;

	bool checkToKeepWeaponAfterAimingWeaponFromShooting2_5d;

	bool carryWeaponInLowerPositionActive;

	bool initialWeaponChecked;

	bool playerOnGround;

	public bool usingDualWeapon;
	public bool usingDualWeaponsPreviously;
	public IKWeaponSystem currentRightIKWeapon;
	public IKWeaponSystem currentLeftIkWeapon;

	public playerWeaponSystem currentRightWeaponSystem;
	public playerWeaponSystem currentLeftWeaponSystem;

	bool playerRunningWithNewFov;

	bool weaponsHUDActive;

	bool carryingSingleWeaponPreviously;
	bool carryingDualWeaponsPreviously;

	bool equippingDualWeaponsFromInventoryMenu;

	IKWeaponSystem previousSingleIKWeapon;

	IKWeaponSystem previousRightIKWeapon;
	IKWeaponSystem previousLeftIKWeapon;

	IKWeaponSystem currentIKWeaponToCheck;

	bool settingSingleWeaponFromNumberKeys;
	string singleWeaponNameToChangeFromNumberkeys;

	float movementHorizontalInput;
	float movementVerticalInput;
	float cameraHorizontalInput;
	float cameraVerticalInput;

	void Awake ()
	{
		getComponents ();

		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}

		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		canMove = !playerIsDead && playerManager.canPlayerMove ();

		if (storePickedWeaponsOnInventory) {
			for (int k = 0; k < weaponsList.Count; k++) {
				weaponsList [k].setWeaponEnabledState (false);
			}
			return;
		}

		if (loadCurrentPlayerWeaponsFromSaveFile && gameSystemManager.loadEnabled) {
			persistanceWeaponInfoList = gameSystemManager.loadPlayerWeaponList (playerManager.getPlayerID ());

			if (persistanceWeaponInfoList != null) {
				bool thereIsCurrentWeapon = false;
				if (persistanceWeaponInfoList.Count > 0) {
					for (int i = 0; i < persistanceWeaponInfoList.Count; i++) {
						for (int k = 0; k < weaponsList.Count; k++) {
							currentIKWeaponToCheck = weaponsList [k];

							if (persistanceWeaponInfoList [i].Name == currentIKWeaponToCheck.getWeaponSystemName ()) {
								currentIKWeaponToCheck.setWeaponEnabledState (persistanceWeaponInfoList [i].isWeaponEnabled);
								currentIKWeaponToCheck.setCurrentWeaponState (persistanceWeaponInfoList [i].isCurrentWeapon);
								currentIKWeaponToCheck.weaponSystemManager.weaponSettings.remainAmmo = persistanceWeaponInfoList [i].remainingAmmo
								- currentIKWeaponToCheck.weaponSystemManager.getWeaponClipSize ();
								
								if (currentIKWeaponToCheck.isCurrentWeapon ()) {
									thereIsCurrentWeapon = true;
								}
							}
						}
					}
				}

				if (!thereIsCurrentWeapon) {
					for (int k = 0; k < weaponsList.Count; k++) {
						currentIKWeaponToCheck = weaponsList [k];

						if (!thereIsCurrentWeapon && currentIKWeaponToCheck.isWeaponEnabled ()) {
							currentIKWeaponToCheck.setCurrentWeaponState (true);
							thereIsCurrentWeapon = true;
						}
					}
				}
			}
		}
	}

	void Start ()
	{
		bool anyWeaponEnabled = checkIfWeaponsAvailable ();

		//print (anyWeaponEnabled);

		if (anyWeaponEnabled) {
			anyWeaponAvailable = true;
			setFirstWeaponWithLowerKeyNumber ();
		} else {
			anyWeaponAvailable = checkAndSetWeaponsAvailable ();
		}

		if (anyWeaponAvailable) {
			getCurrentWeapon ();
			getCurrentWeaponRotation (currentIKWeapon);
		}

		originalFov = mainCamera.fieldOfView;

		touchPlatform = touchJoystick.checkTouchPlatform ();

		setHudZone ();

		originalWeaponsCameraFov = weaponsCamera.fieldOfView;

		if (weaponsSlotsAmount < 10) {
			weaponsSlotsAmount++;
		}

		mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (storePickedWeaponsOnInventory) {
			if (!notActivateWeaponsAtStart) {
				for (int k = 0; k < weaponsList.Count; k++) {

					currentIKWeaponToCheck = weaponsList [k];

					if (currentIKWeaponToCheck.isWeaponEnabled ()) {
						string weaponName = currentIKWeaponToCheck.getWeaponSystemName ();
						inventoryInfo newWeaponInventoryInfo = mainInventoryListManager.getInventoryInfoFromName (weaponName);
						newWeaponInventoryInfo.amount = 1;
						playerInventoryManager.tryToPickUpObject (newWeaponInventoryInfo);
					}
				}
			}
		}
	}

	void Update ()
	{
		if (!startInitialized) {
			startInitialized = true;
		}

		canMove = !playerIsDead && playerManager.canPlayerMove ();

		playerOnGround = playerManager.isPlayerOnGround ();

		if (canMove && weaponsModeActive) {

			playerCurrentlyBusy = playerIsBusy ();
			if (!playerCurrentlyBusy) {
		
				checkTypeView ();

				if (anyWeaponAvailable) {
					if ((!usingDualWeapon && currentIKWeapon.isCurrentWeapon ()) || (usingDualWeapon && (currentLeftIkWeapon.isCurrentWeapon () || currentRightIKWeapon.isCurrentWeapon ()))) {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {

							checkIfCanMarkTargets ();

							if ((keepWeaponAfterDelayThirdPerson && isThirdPersonView) || (keepWeaponAfterDelayFirstPerson && !isThirdPersonView)) {
								if (!isAimingWeapons () && !shootingSingleWeapon) {
									if (Time.time > keepWeaponDelay + lastTimeWeaponUsed) {
										drawOrKeepWeaponInput ();
									}
								}
							}

							if (stopAimingOnAirThirdPerson && aimingInThirdPerson && !canAimOnAirThirdPerson && !playerOnGround) {
								aimCurrentWeaponInput ();
							}

							if (usingDualWeapon) {
								if (isThirdPersonView) {
									checkForDisableFreeFireModeAfterStopFiring ();
								}
							} else {
								if (isThirdPersonView) {
									checkForDisableFreeFireModeAfterStopFiring ();
								}

								if (playerCameraManager.isCameraTypeFree () && pivotPointRotationActive && carryingWeaponInThirdPerson && !checkToKeepWeaponAfterAimingWeaponFromShooting) {
									if (currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint) {
										float currentAngle = 0;

										if ((usingFreeFireMode || checkToKeepWeaponAfterAimingWeaponFromShooting)) {
											currentAngle = Vector3.SignedAngle (-currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.forward, transform.forward, transform.up);
										}

										currentAngle = -180 + currentAngle;

										currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.localRotation =
									Quaternion.Lerp (currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.localRotation, 
											Quaternion.Euler (new Vector3 (0, currentAngle, 0)), Time.deltaTime * pivotPointRotationSpeed);
									}
								}
							}
						}

						if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
							checkAutoShootOnTag ();
						} 

						if (!usedByAI) {
					
							if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {

								//if the touch controls are enabled, activate the swipe option
								if (input.isUsingTouchControls ()) {
									//select the weapon by swiping the finger in the right corner of the screen, above the weapon info
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
											if (touchZoneRect.Contains (currentTouch.position) && !touching) {
												swipeStartPos = currentTouch.position;
												touching = true;
											}
										}
										//and the final position, and get the direction, to change to the previous or the next power
										if (currentTouch.phase == TouchPhase.Ended && touching) {
											float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;
											if (swipeDistHorizontal > minSwipeDist) {
												float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);
												if (swipeValue > 0) {
													//right swipe
													if (usingDualWeapon) {
														keepDualWeaponAndDrawSingle (true);
													} else {
														choosePreviousWeapon (false, true);
													}
												} else if (swipeValue < 0) {
													//left swipe
													if (usingDualWeapon) {
														keepDualWeaponAndDrawSingle (false);
													} else {
														chooseNextWeapon (false, true);
													}
												}
											}
											touching = false;
										}
									}
								}
							}
						}
					}

					if (!usedByAI) {
						if (changeWeaponsWithNumberKeysActive && !currentIKWeapon.isWeaponMoving () && !gameSystemManager.isGamePaused ()) {
							for (int i = 0; i < weaponsSlotsAmount; i++) {
								if (Input.GetKeyDown ("" + (i))) {
									for (int k = 0; k < weaponsList.Count; k++) {
										int keyNumberToCheck = weaponsList [k].getWeaponSystemKeyNumber ();

										if ((keyNumberToCheck == i) && weaponsList [k].isWeaponEnabled ()) {

											if (checkWeaponToChangeByIndex (weaponsList [k], keyNumberToCheck, i, k)) {
												return;
											}
										}
									}
								}
							}
						}
					} else {
						if (aimWhenItIsReady) {
							aimCurrentWeaponInput ();
							if (aimingInFirstPerson || aimingInThirdPerson) {
								aimWhenItIsReady = false;
							}
						}
					}

					if (!initialWeaponChecked) {
						if (startGameWithCurrentWeapon) {
							if (startGameWithDualWeapons) {
								if (!startWithFirstWeaponAvailable) {
									if (avaliableWeaponList.Length > 1) {
										if (drawInitialWeaponSelected) {
											changeDualWeapons (rightWeaponToStartName, leftWeaponToStartName);
										
											getCurrentWeaponRotation (currentRightIKWeapon);

											updateWeaponSlotInfo ();
										}
									}
								}
							} else {
								if (!startWithFirstWeaponAvailable) {
									if (avaliableWeaponList.Length > 0) {
										setWeaponToStartGame (weaponToStartName);
									}
								}

								getCurrentWeaponRotation (currentIKWeapon);

								if (drawInitialWeaponSelected) {
									drawOrKeepWeaponInput ();

									checkShowWeaponSlotsParentWhenWeaponSelected (currentIKWeapon.getWeaponSystemKeyNumber ());
								} else {

									updateCurrentChoosedDualWeaponIndex ();

									playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
								}
							}
								
						} else {
							if (!initialWeaponChecked) {
								for (int k = 0; k < weaponsList.Count; k++) {
									if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemManager ().getWeaponNumberKey () == 1) {
										setWeaponByName (weaponsList [k].getWeaponSystemManager ().getWeaponSystemName ());

										getCurrentWeaponRotation (currentIKWeapon);

										updateCurrentChoosedDualWeaponIndex ();

										playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
									}
								}
							}
						}
					}
				}
			} 

			initialWeaponChecked = true;
		}

		if (changingWeapon) {
			if (usingDualWeapon || changingDualWeapon) {
				if (!keepingWeapon) {
					keepDualWeapons ();
					keepingWeapon = true;
				}

				if (!currentRightIKWeapon.isWeaponMoving () && !currentLeftIkWeapon.isWeaponMoving () && (!carryingSingleWeaponPreviously || !currentIKWeapon.isWeaponMoving ()) &&
				    (!isThirdPersonView || !IKManager.usingWeapons) &&
				    (!carryingDualWeaponsPreviously || (!previousRightIKWeapon.isWeaponMoving () && !previousLeftIKWeapon.isWeaponMoving ()))) {

					if (currentRightIKWeapon) {
						currentRightIKWeapon.setDisablingDualWeaponState (false);
					}

					if (currentLeftIkWeapon) {
						currentLeftIkWeapon.setDisablingDualWeaponState (false);
					}

					if (currentIKWeapon) {
						currentIKWeapon.setDisablingDualWeaponState (false);
					}

					if (previousRightIKWeapon) {
						previousRightIKWeapon.setDisablingDualWeaponState (false);
					}

					weaponChanged ();

					print ("draw dual weapons aqui " + currentRightIKWeapon.getWeaponSystemName () + " " + currentLeftIkWeapon.getWeaponSystemName ());
					drawDualWeapons ();

					keepingWeapon = false;
					changingWeapon = false;

					changingDualWeapon = false;

					carryingSingleWeaponPreviously = false;

					carryingDualWeaponsPreviously = false;
				}
			} else {
				if (!keepingWeapon) {
					drawOrKeepWeapon (false);
					keepingWeapon = true;
				}

				if (!currentIKWeapon.isWeaponMoving () && (!carryingDualWeaponsPreviously || (!currentRightIKWeapon.isWeaponMoving () && !currentLeftIkWeapon.isWeaponMoving ())) &&
				    (!isThirdPersonView || !IKManager.usingWeapons)) {

					if (currentRightIKWeapon) {
						currentRightIKWeapon.setDisablingDualWeaponState (false);
					}

					if (currentLeftIkWeapon) {
						currentLeftIkWeapon.setDisablingDualWeaponState (false);
					}

					if (currentIKWeapon) {
						currentIKWeapon.setDisablingDualWeaponState (false);
					}

					weaponChanged ();

					drawOrKeepWeapon (true);
					keepingWeapon = false;
					changingWeapon = false;

					if (changingSingleWeapon) {
						currentRightIKWeapon = null;
						currentLeftIkWeapon = null;
						currentRightWeaponSystem = null;
						currentLeftWeaponSystem = null;

						changingSingleWeapon = false;

						carryingDualWeaponsPreviously = false;
					}
				}
			}
		}
	}

	public bool checkWeaponToChangeByIndex (IKWeaponSystem currentWeaponToCheck, int keyNumberToCheck, int weaponSlotIndex, int weaponIndex)
	{
		checkShowWeaponSlotsParentWhenWeaponSelected (weaponSlotIndex);

		print (currentWeaponToCheck.getWeaponSystemName () + " " + currentWeaponToCheck.isWeaponConfiguredAsDualWepaon ());

		if (currentWeaponToCheck.isWeaponConfiguredAsDualWepaon ()) {
			if (chooseDualWeaponIndex != keyNumberToCheck) {
				chooseDualWeaponIndex = keyNumberToCheck;

				if (isPlayerCarringWeapon ()) {
					print ("configured with dual weapon");
					if (currentWeaponToCheck.usingRightHandDualWeapon) {
						changeDualWeapons (currentWeaponToCheck.getWeaponSystemName (), currentWeaponToCheck.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentWeaponToCheck.getLinkedDualWeaponName (), currentWeaponToCheck.getWeaponSystemName ());
					}
				}

				return true;
			} else {
				if (!isPlayerCarringWeapon ()) {
					print ("configured with dual weapon");
					if (currentWeaponToCheck.usingRightHandDualWeapon) {
						changeDualWeapons (currentWeaponToCheck.getWeaponSystemName (), currentWeaponToCheck.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentWeaponToCheck.getLinkedDualWeaponName (), currentWeaponToCheck.getWeaponSystemName ());
					}
				}
			}
		} else {
			print ("configured as single weapon");

			if (choosedWeapon != weaponIndex || usingDualWeapon) {
				if (usingDualWeapon) {
					chooseDualWeaponIndex = keyNumberToCheck;

					print ("previously using dual weapons");

					settingSingleWeaponFromNumberKeys = true;

					singleWeaponNameToChangeFromNumberkeys = currentWeaponToCheck.getWeaponSystemName ();
					changeSingleWeapon (currentWeaponToCheck.getWeaponSystemName ());

					return true;
				} else {
					if (choosedWeapon != weaponIndex) {
						chooseDualWeaponIndex = keyNumberToCheck;

						print ("previously using single weapon");

						choosedWeapon = weaponIndex;
						currentIKWeapon.setCurrentWeaponState (false);

						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if (useQuickDrawWeapon && carryingWeaponInThirdPerson) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								changingWeapon = true;
							}
						} else {
							weaponChanged ();
						}

						return true;
					}
				}
			} else {
				if (chooseDualWeaponIndex == keyNumberToCheck) {
					if (!isPlayerCarringWeapon ()) {
						drawOrKeepWeaponInput ();
					}
				} else {
					chooseDualWeaponIndex = keyNumberToCheck;
				}
			}
		}

		return false;
	}

	public void checkShowWeaponSlotsParentWhenWeaponSelected (int weaponSlotIndex)
	{
		if (playerInventoryManager) {
			playerInventoryManager.showWeaponSlotsParentWhenWeaponSelected (weaponSlotIndex);
		}
	}

	public void checkForDisableFreeFireModeAfterStopFiring ()
	{
		if (carryingWeaponInThirdPerson && aimingInThirdPerson && (checkToKeepWeaponAfterAimingWeaponFromShooting || checkToKeepWeaponAfterAimingWeaponFromShooting2_5d)) {
			if (Time.time > lastTimeFired + timeToStopAimAfterStopFiring) {

				disableFreeFireModeAfterStopFiring ();
			}
		}
	}

	public void disableFreeFireModeAfterStopFiring ()
	{
		headTrackManager.setOriginalCameraBodyWeightValue ();

		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		aimModeInputPressed = false;

		aimCurrentWeaponInput ();
	}

	public void resetWeaponFiringAndAimingIfPlayerDisabled ()
	{
		if (weaponsModeActive) {
			disableFreeFireModeState ();

			if (usingDualWeapon) {
				shootDualWeapon (false, false, false);
			} else {
				shootWeapon (false);
			}
		}
	}

	public void setWeaponsJumpStartPositionState (bool state)
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon && currentRightIKWeapon.weaponUseJumpPositions ()) {
				currentRightIKWeapon.setPlayerOnJumpStartState (state);
			}

			if (currentLeftIkWeapon && currentLeftIkWeapon.weaponUseJumpPositions ()) {
				currentLeftIkWeapon.setPlayerOnJumpStartState (state);
			}
		} else {
			if (currentIKWeapon && currentIKWeapon.weaponUseJumpPositions ()) {
				currentIKWeapon.setPlayerOnJumpStartState (state);
			}
		}
	}

	public void setWeaponsJumpEndPositionState (bool state)
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon && currentRightIKWeapon.weaponUseJumpPositions ()) {
				currentRightIKWeapon.setPlayerOnJumpEndState (state);
			}

			if (currentLeftIkWeapon && currentLeftIkWeapon.weaponUseJumpPositions ()) {
				currentLeftIkWeapon.setPlayerOnJumpEndState (state);
			}
		} else {
			if (currentIKWeapon && currentIKWeapon.weaponUseJumpPositions ()) {
				currentIKWeapon.setPlayerOnJumpEndState (state);
			}
		}
	}

	public void disableWeaponJumpState ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.setPlayerOnJumpStartState (false);
			currentRightIKWeapon.setPlayerOnJumpEndState (false);

			currentLeftIkWeapon.setPlayerOnJumpStartState (false);
			currentLeftIkWeapon.setPlayerOnJumpEndState (false);
		} else {
			if (currentIKWeapon) {
				currentIKWeapon.setPlayerOnJumpStartState (false);
				currentIKWeapon.setPlayerOnJumpEndState (false);
			}
		}
	}

	public void setRunningState (bool state)
	{
		running = state;
	}

	public bool isPlayerRunning ()
	{
		return running;
	}

	public bool isPlayerCrouching ()
	{
		return playerManager.isCrouching ();
	}

	//if the option to change camera fov is active, set the fov value according to if the player is running or not
	public void setPlayerRunningState (bool state, IKWeaponSystem IKWeaponToUse)
	{
		IKWeaponToUse.setPlayerRunningState (state);

		playerRunningWithNewFov = state;

		if (usingDualWeapon) {
			IKWeaponToUse = currentRightIKWeapon;
		}

		if (state) {
			playerCameraManager.setMainCameraFov (IKWeaponToUse.firstPersonWeaponInfo.newFovOnRun, IKWeaponToUse.firstPersonWeaponInfo.changeFovSpeed);

			if (playerCameraManager.isUsingZoom ()) {
				changeWeaponsCameraFov (true, originalWeaponsCameraFov, IKWeaponToUse.aimFovSpeed);
				playerCameraManager.disableZoom ();
			}
		} else {
			playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), IKWeaponToUse.firstPersonWeaponInfo.changeFovSpeed);
		}
	}

	public void disablePlayerRunningState ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon.isPlayerRunning ()) {
				setPlayerRunningState (false, currentRightIKWeapon);
			}
		} else {
			if (currentIKWeapon.isPlayerRunning ()) {
				setPlayerRunningState (false, currentIKWeapon);
			}
		}
	}

	public void resetPlayerRunningState ()
	{
		playerRunningWithNewFov = false;
		playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), playerCameraManager.zoomSpeed);
	}

	public bool isPlayerRunningWithNewFov ()
	{
		return playerRunningWithNewFov;
	}

	public void useMeleeAttack ()
	{
		if (usingDualWeapon) {
			if (!shootingRightWeapon) {
				currentRightIKWeapon.walkOrMeleeAttackWeaponPosition ();
			}

			if (!shootingLeftWeapon) {
				currentLeftIkWeapon.walkOrMeleeAttackWeaponPosition ();
			}
		} else {
			if (!shootingSingleWeapon && currentIKWeapon.isWeaponHandsOnPositionToAim ()) {
				currentIKWeapon.walkOrMeleeAttackWeaponPosition ();
			}
		}
	}

	public bool isPlayerCarringWeapon ()
	{
		if (usingDualWeapon) {
			return currentRightIKWeapon.isPlayerCarringWeapon () && currentLeftIkWeapon.isPlayerCarringWeapon ();
		} else {
			if (currentIKWeapon) {
				return currentIKWeapon.isPlayerCarringWeapon ();
			} else {
				return false;
			}
		}
	}

	public bool currentWeaponIsMoving ()
	{
		if (usingDualWeapon) {
			return currentRightIKWeapon.isWeaponMoving () || currentLeftIkWeapon.isWeaponMoving ();
		} else {
			if (currentIKWeapon) {
				return currentIKWeapon.isWeaponMoving ();
			} else {
				return false;
			}
		}
	}

	public bool currentWeaponWithHandsInPosition ()
	{
		return currentIKWeapon.thirdPersonWeaponInfo.handsInPosition;
	}

	public bool currentDualWeaponWithHandsInPosition (bool isRightWeapon)
	{
		if (isRightWeapon) {
			return currentRightIKWeapon.thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInPosition;
		} else {
			return currentLeftIkWeapon.thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInPosition;
		}
	}

	public bool weaponsAreMoving ()
	{
		if (changingWeapon || keepingWeapon) {
			return true;
		}
		return false;
	}

	public void checkTypeView ()
	{
		isThirdPersonView = !playerCameraManager.isFirstPersonActive ();
	}

	public bool isFirstPersonActive ()
	{
		return playerCameraManager.isFirstPersonActive ();
	}

	//shoot to any object with the tag configured in the inspector, in case the option is enabled
	public void checkAutoShootOnTag ()
	{
		if (currentWeaponSystem.weaponSettings.autoShootOnTag) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, 
				    currentWeaponSystem.weaponSettings.maxDistanceToRaycast, currentWeaponSystem.weaponSettings.layerToAutoShoot)) {

				GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

				if (target != null && target != gameObject) {
					if (currentWeaponSystem.weaponSettings.autoShootTagList.Contains (target.tag)) {
						if (usingDualWeapon) {
							shootDualWeapon (aimingInThirdPerson, false, true);
						} else {
							shootWeapon (aimingInThirdPerson);
						}
					}
				} else {
					if (currentWeaponSystem.weaponSettings.autoShootTagList.Contains (hit.collider.gameObject.tag) ||
					    (currentWeaponSystem.weaponSettings.shootAtLayerToo &&
					    (1 << hit.collider.gameObject.layer & currentWeaponSystem.weaponSettings.layerToAutoShoot.value) == 1 << hit.collider.gameObject.layer)) {

						if (usingDualWeapon) {
							shootDualWeapon (aimingInThirdPerson, false, true);
						} else {
							shootWeapon (aimingInThirdPerson);
						}
					}
				}
			}
		}
	}

	public void checkIfCanMarkTargets ()
	{
		if (!canMarkTargets) {
			return;
		}

		if (currentWeaponSystem.canMarkTargets) {
			if ((isThirdPersonView && currentWeaponSystem.canMarkTargetsOnThirdPerson) || (!isThirdPersonView && currentWeaponSystem.canMarkTargetsOnFirstPerson)) {
				if ((isThirdPersonView && aimingInThirdPerson) || !currentWeaponSystem.aimOnFirstPersonToMarkTarget || aimingInFirstPerson) {
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, 
						    currentWeaponSystem.maxDistanceToMarkTargets, currentWeaponSystem.markTargetsLayer)) {
						if (currentWeaponSystem.tagListToMarkTargets.Contains (hit.collider.tag)) {

							GameObject currentCharacter = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

							float iconOffset = applyDamage.getCharacterHeight (currentCharacter);

							if (iconOffset < 0) {
								iconOffset = 2;
							}

							if (!playerScreenObjectivesManager.objectAlreadyOnList (currentCharacter)) {

								playerScreenObjectivesManager.addElementToPlayerList (currentCharacter, false, false, 0, true, false, 
									true, currentWeaponSystem.markTargetName, false, Color.white, true, -1, iconOffset, false);

								if (currentWeaponSystem.useMarkTargetSound) {
									currentWeaponSystem.playSound (currentWeaponSystem.markTargetSound);
								}
							}
						}
					}
				}
			}
		}
	}

	//shoot the current weapon
	public void shootWeapon (bool state)
	{
		if (!playerCurrentlyBusy) {
			if (state) {
				if (!currentWeaponSystem.isShootingBurst ()) {

					if (currentIKWeapon.isCursorHidden ()) {
						enableOrDisableGeneralWeaponCursor (true);
						currentIKWeapon.setCursorHiddenState (false);
					}

					disablePlayerRunningState ();

					if (!currentWeaponSystem.reloading && currentWeaponSystem.getWeaponClipSize () > 0) {
						shootingSingleWeapon = true;
					} else {
						shootingSingleWeapon = false;
					}
					currentWeaponSystem.shootWeapon (isThirdPersonView, state);

					setLastTimeFired ();

					setLastTimeUsed ();

					if (storePickedWeaponsOnInventory && currentWeaponSystem.weaponSettings.weaponUsesAmmo) {
						playerInventoryManager.updateWeaponSlotAmmo (currentWeaponSystem.getWeaponNumberKey () - 1);
					}

					currentWeaponSystem.checkWeaponAbilityHoldButton ();
				}
			} else {
				shootingSingleWeapon = false;
				currentWeaponSystem.shootWeapon (isThirdPersonView, state);
			}
		}
	}

	public void shootDualWeapon (bool state, bool rightWeapon, bool shootBothWeapons)
	{
		if (!playerCurrentlyBusy) {
			if (rightWeapon || shootBothWeapons) {
				if (state) {
					if (!currentRightWeaponSystem.isShootingBurst ()) {

						if (currentRightIKWeapon.isCursorHidden ()) {
							enableOrDisableGeneralWeaponCursor (true);
							currentRightIKWeapon.setCursorHiddenState (false);
						}

						disablePlayerRunningState ();

						if (!currentRightWeaponSystem.reloading && currentRightWeaponSystem.getWeaponClipSize () > 0) {
							shootingRightWeapon = true;
						} else {
							shootingRightWeapon = false;
						}

						currentRightWeaponSystem.shootWeapon (isThirdPersonView, state);

						setLastTimeFired ();

						setLastTimeUsed ();

						currentRightWeaponSystem.checkWeaponAbilityHoldButton ();
					}
				} else {
					shootingRightWeapon = false;
					currentRightWeaponSystem.shootWeapon (isThirdPersonView, state);
				}
			} 

			if (!rightWeapon || shootBothWeapons) {
				if (state) {
					if (!currentLeftWeaponSystem.isShootingBurst ()) {

						if (currentLeftIkWeapon.isCursorHidden ()) {
							enableOrDisableGeneralWeaponCursor (true);
							currentLeftIkWeapon.setCursorHiddenState (false);
						}

						disablePlayerRunningState ();

						if (!currentLeftWeaponSystem.reloading && currentLeftWeaponSystem.getWeaponClipSize () > 0) {
							shootingLeftWeapon = true;
						} else {
							shootingLeftWeapon = false;
						}

						currentLeftWeaponSystem.shootWeapon (isThirdPersonView, state);

						setLastTimeFired ();

						setLastTimeUsed ();

						currentLeftWeaponSystem.checkWeaponAbilityHoldButton ();
					}
				} else {
					shootingLeftWeapon = false;
					currentLeftWeaponSystem.shootWeapon (isThirdPersonView, state);
				}
			}
		}
	}

	public void setShootingState (bool state)
	{
		if (usingDualWeapon) {
			shootingRightWeapon = state;
			shootingLeftWeapon = state;
		} else {
			shootingSingleWeapon = state;
		}
	}

	//check if the player is using a device or using a game submen
	public bool playerIsBusy ()
	{
		if (!playerManager.usingDevice && !carryingPhysicalObject && !editingWeaponAttachments && ((pauseManager && !pauseManager.usingSubMenu && !pauseManager.playerMenuActive) || !pauseManager)) {
			return false;
		}
		return true;
	}

	//get and set the last time when player fired a weapon
	public void setLastTimeFired ()
	{
		lastTimeFired = Time.time;
	}

	public void setLastTimeUsed ()
	{
		lastTimeWeaponUsed = Time.time;
	}

	public float getLastTimeFired ()
	{
		return lastTimeFired;
	}

	public void setLastTimeMoved ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.setLastTimeMoved ();
			currentLeftIkWeapon.setLastTimeMoved ();
		} else {
			if (currentIKWeapon) {
				currentIKWeapon.setLastTimeMoved ();
			}
		}
	}

	public void setGamePausedState (bool state)
	{
		gamePaused = state;

		setLastTimeFired ();

		setLastTimeMoved ();
	}

	public void setUsingDeviceState (bool state)
	{
		usingDevice = state;

		setLastTimeFired ();

		setLastTimeMoved ();
	}

	//get the head bod manager
	public headBob getHeadBobManager ()
	{
		return headBobManager;
	}

	//set the weapon sway values in first person
	void FixedUpdate ()
	{
		if (weaponsModeActive && anyWeaponAvailable) {
			if ((currentIKWeapon.isCurrentWeapon () || usingDualWeapon) &&
			    isPlayerCarringWeapon () && !editingWeaponAttachments && cursorLocked &&
			    (!isThirdPersonView || !aimingInThirdPerson)) {

				currentWeaponSway ();
			}
		}
	}

	public void currentWeaponSway ()
	{
		movementHorizontalInput = playerManager.getHorizontalInput ();
		movementVerticalInput = playerManager.getVerticalInput ();
		cameraHorizontalInput = playerCameraManager.getHorizontalInput ();
		cameraVerticalInput = playerCameraManager.getVerticalInput ();

		if (usingDualWeapon) {
			currentRightIKWeapon.currentWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingRightWeapon, playerOnGround, 
				headBobManager.externalShakingActive, playerManager.usingDevice, isThirdPersonView);
			
			currentLeftIkWeapon.currentWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingLeftWeapon, playerOnGround, 
				headBobManager.externalShakingActive, playerManager.usingDevice, isThirdPersonView);
		} else {
			currentIKWeapon.currentWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingSingleWeapon, playerOnGround, 
				headBobManager.externalShakingActive, playerManager.usingDevice, isThirdPersonView);
		}
	}

	public bool isPlayerOnGround ()
	{
		return playerOnGround;
	}

	public bool isPlayerMoving ()
	{
		return playerManager.isPlayerMoving (0);
	}

	//in any view, draw or keep the weapon
	public void drawOrKeepWeaponInput ()
	{
		if (isThirdPersonView) {
			drawOrKeepWeapon (!carryingWeaponInThirdPerson);
		} else {
			drawOrKeepWeapon (!carryingWeaponInFirstPerson);
		}
	}

	public void drawWeaponAIInput ()
	{
		if (isThirdPersonView) {
			if (!carryingWeaponInThirdPerson) {
				drawOrKeepWeapon (true);
			}
		} else {
			if (!carryingWeaponInFirstPerson) {
				drawOrKeepWeapon (true);
			}
		}
	}

	public void keepWeaponAIInput ()
	{
		if (isThirdPersonView) {
			if (carryingWeaponInThirdPerson) {
				drawOrKeepWeapon (false);
			}
		} else {
			if (carryingWeaponInFirstPerson) {
				drawOrKeepWeapon (false);
			}
		}
	}

	public void drawOrKeepWeapon (bool state)
	{
		if (isThirdPersonView) {
			drawOrKeepWeaponThirdPerson (state);
		} else {
			drawOrKeepWeaponFirstPerson (state);

			currentWeaponSystem.setWeaponCarryState (false, state);
		}

		if (state) {
			if (usingDualWeapon) {
				checkShowWeaponSlotsParentWhenWeaponSelected (currentRightWeaponSystem.getWeaponNumberKey ());
			} else {
				checkShowWeaponSlotsParentWhenWeaponSelected (currentWeaponSystem.getWeaponNumberKey ());
			}

			updateCurrentChoosedDualWeaponIndex ();
		}

		setLastTimeFired ();

		setLastTimeUsed ();

		lastTimeDrawWeapon = Time.time;
	}

	public void aimCurrentWeaponWhenItIsReady (bool state)
	{
		aimWhenItIsReady = state;
	}

	//in any view, aim or draw the current weapon
	public void aimCurrentWeaponInput ()
	{
		if (isThirdPersonView) {
			aimCurrentWeapon (!aimingInThirdPerson);
		} else {
			aimCurrentWeapon (!aimingInFirstPerson);
		}
	}

	public void aimCurrentWeapon (bool state)
	{
		if (isThirdPersonView) {
			aimOrDrawWeaponThirdPerson (state);
		} else {
			aimOrDrawWeaponFirstPerson (state);
		}

		setLastTimeFired ();

		setLastTimeUsed ();
	}

	//draw or keep the weapon in third person
	public void drawOrKeepWeaponThirdPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			currentWeaponSystem.setWeaponCarryState (false, false);
			return;
		}

		lockCursorAgain ();

		if (playerManager.isCrouching ()) {
			playerManager.crouch ();
		}

		if (playerManager.isCrouching ()) {
			return;
		}

		if (useQuickDrawWeapon || (usingDualWeapon && currentIKWeapon.isQuickDrawKeepDualWeaponActive ())) {
			if (state) {
				quickDrawWeaponThirdPersonAction ();
			} else {
				quickKeepWeaponThirdPersonAction ();
			}

		} else {
			currentWeaponSystem.setWeaponCarryState (state, false);

			carryingWeaponInThirdPerson = state;

			enableOrDisableWeaponsHUD (carryingWeaponInThirdPerson);

			if (!usingDualWeapon) {
				getCurrentWeapon ();
			}

			if (carryingWeaponInThirdPerson) {
				updateWeaponHUDInfo ();

				updateAmmo ();

				currentIKWeapon.checkWeaponSidePosition ();

				IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, true);
			} else {
				currentWeaponSystem.enableHUD (false);

				if (aimingInThirdPerson) {
					activateOrDeactivateAimMode (false);
					aimingInThirdPerson = state;

					checkPlayerCanJumpWhenAimingState ();

					enableOrDisableGrabObjectsManager (aimingInThirdPerson);
				} 

				currentWeaponSystem.setWeaponAimState (false, false);

				IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, false);

				if (currentIKWeapon.carrying) {
					if (currentIKWeapon.thirdPersonWeaponInfo.useQuickDrawKeepWeapon) {
						weaponReadyToMoveDirectlyOnDrawHand ();
					} else {
						weaponReadyToMove ();
					}
				}

				enableOrDisableWeaponCursor (false);

				setAimAssistInThirdPersonState ();
			}
		}
	}

	public void weaponReadyToMove ()
	{
		currentIKWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
	}

	public void dualWeaponReadyToMove (bool isRightWeapon)
	{
		if (isRightWeapon) {
			if (currentRightIKWeapon) {
				currentRightIKWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
			}
		} else {
			if (currentLeftIkWeapon) {
				currentLeftIkWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
			}
		}
	}

	public void grabWeaponWithNoDrawHand ()
	{
		IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, false);
	}

	public void weaponReadyToMoveDirectlyOnDrawHand ()
	{
		currentIKWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
	}

	public void dualWeaponReadyToMoveDirectlyOnDrawHand (bool isRightWeapon)
	{
		if (isRightWeapon) {
			if (currentRightIKWeapon) {
				currentRightIKWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
			}
		} else {
			if (currentLeftIkWeapon) {
				currentLeftIkWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
			}
		}
	}

	//aim or draw the weapon in third person
	public void aimOrDrawWeaponThirdPerson (bool state)
	{
		if (state != aimingInThirdPerson || (state && aimModeInputPressed && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode))) {
			if (!canUseWeapons () && state) {
				return;
			}

			if (playerManager.isCrouching ()) {
				playerManager.crouch ();
			}

			if (playerManager.isCrouching ()) {
				return;
			}

			if (aimModeInputPressed && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode)) {
				disableFreeFireModeState ();
			}
				
			if (usingDualWeapon) {
				if (!currentRightIKWeapon.isRightWeaponHandOnPositionToAim () || !currentLeftIkWeapon.isLeftWeaponHandOnPositionToAim ()) {
					return;
				}
			} else {
				if (!currentIKWeapon.isWeaponHandsOnPositionToAim ()) {
					return;
				}
			}

			enableOrDisableWeaponCursor (state);
			aimingInThirdPerson = state;

			checkPlayerCanJumpWhenAimingState ();

			enableOrDisableGrabObjectsManager (aimingInThirdPerson);

			if (usingDualWeapon) {
				currentRightIKWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentRightWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentRightWeaponSystem.enableHUD (aimingInThirdPerson);

				currentLeftIkWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentLeftWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentLeftWeaponSystem.enableHUD (aimingInThirdPerson);
			} else {
				currentIKWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentWeaponSystem.enableHUD (aimingInThirdPerson);
			}

			if (aimingInThirdPerson) {
				activateOrDeactivateAimMode (true);
			} else {
				activateOrDeactivateAimMode (false);
			}

			setAimAssistInThirdPersonState ();

			if (!usingDualWeapon) {
				if (currentIKWeapon.useLowerRotationSpeedAimedThirdPerson) {
					if (aimingInThirdPerson) {
						playerCameraManager.changeRotationSpeedValue (currentIKWeapon.verticalRotationSpeedAimedInThirdPerson, currentIKWeapon.horizontalRotationSpeedAimedInThirdPerson);
					} else {
						playerCameraManager.setOriginalRotationSpeed ();
					}
				}
			}

			if (runWhenAimingWeaponInThirdPerson) {
				if (!aimingWeaponFromShooting) {
					if (aimingInThirdPerson) {
						runningPreviouslyAiming = running;
						if (!runningPreviouslyAiming) {
							powersManager.checkIfCanRun ();
						}
					} else {
						if (stopRunIfPreviouslyNotRunning) {
							if (!runningPreviouslyAiming) {
								powersManager.stopRun ();
							}
						}
					}
				}
			}
		}
	}

	public void setAimAssistInThirdPersonState ()
	{
		if (useAimAssistInThirdPerson) {
			if (playerCameraManager.isCameraTypeFree ()) {
				playerCameraManager.setLookAtTargetSpeedValue (aimAssistLookAtTargetSpeed);

				playerCameraManager.setLookAtTargetEnabledStateDuration (true, playerCameraManager.timeToStopAimAssist, true);
			}

			if (aimingInThirdPerson) {
				playerCameraManager.setCurrentLockedCameraCursor (cursorRectTransform);
			} else {
				playerCameraManager.setCurrentLockedCameraCursor (null);
			}

			playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);

			if (!playerCameraManager.isCameraTypeFree ()) {
				if (useAimAssistInLockedCamera) {
					playerCameraManager.setLookAtTargetOnLockedCameraState ();
				}
			}

			playerCameraManager.setLookAtTargetState (aimingInThirdPerson, null);
		}
	}

	//draw or keep the weapon in first person
	public void drawOrKeepWeaponFirstPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			return;
		}

		weaponsCamera.enabled = false;
		weaponsCamera.enabled = true;

		lockCursorAgain ();

		carryingWeaponInFirstPerson = state;

		enableOrDisableWeaponsHUD (carryingWeaponInFirstPerson);

		if (!usingDualWeapon) {
			getCurrentWeapon ();
		}

		if (carryingWeaponInFirstPerson) {
			updateWeaponHUDInfo ();

			updateAmmo ();

			if (playerCameraManager.isUsingZoom ()) {
				changeWeaponsCameraFov (true, playerCameraManager.getMainCameraCurrentFov (), currentIKWeapon.aimFovSpeed);
			}
		} else {
			if (aimingInFirstPerson) {
				aimingInFirstPerson = state;

				currentWeaponSystem.setWeaponAimState (false, false);

				changeCameraFov (aimingInFirstPerson);
			}

			IKManager.setUsingWeaponsState (false);
		}

		currentIKWeapon.drawOrKeepWeaponFirstPerson (carryingWeaponInFirstPerson);

		enableOrDisableWeaponCursor (state);

		currentWeaponSystem.enableHUD (carryingWeaponInFirstPerson);
	}

	//aim or draw the weapon in first person
	public void aimOrDrawWeaponFirstPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			return;
		}

		if (usingDualWeapon) {
			return;
		}

		if (currentIKWeapon.canAimInFirstPerson) {
			aimingInFirstPerson = state;

			//if the weapon was detecting an obstacle which has disabled the weapon cursor, enable it again when the weapon enters in aim mode
			if (aimingInFirstPerson) {
				if (weaponCursor && !weaponCursor.activeSelf) {
					enableOrDisableGeneralWeaponCursor (true);
				}
			}

			enableOrDisableWeaponCursor (aimingInFirstPerson);

			currentIKWeapon.aimOrDrawWeaponFirstPerson (aimingInFirstPerson);

			if (currentWeaponSystem.weaponSettings.disableHUDInFirstPersonAim) {
				currentWeaponSystem.enableHUD (!aimingInFirstPerson);
			}

			changeCameraFov (aimingInFirstPerson);

			if (currentIKWeapon.useLowerRotationSpeedAimed) {
				if (aimingInFirstPerson) {
					playerCameraManager.changeRotationSpeedValue (currentIKWeapon.verticalRotationSpeedAimedInFirstPerson, currentIKWeapon.horizontalRotationSpeedAimedInFirstPerson);
				} else {
					playerCameraManager.setOriginalRotationSpeed ();
				}
			}

			currentWeaponSystem.setWeaponAimState (false, aimingInFirstPerson);
			playerManager.enableOrDisableAiminig (aimingInFirstPerson);

			if (useAimAssistInFirstPerson) {

				if (playerCameraManager.isCameraTypeFree ()) {
					playerCameraManager.setLookAtTargetSpeedValue (aimAssistLookAtTargetSpeed);

					playerCameraManager.setLookAtTargetEnabledStateDuration (true, playerCameraManager.timeToStopAimAssist, true);
				}

				playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);
				playerCameraManager.setLookAtTargetState (aimingInFirstPerson, null);
			}
		}
	}

	//change the camera fov when the player aims in first person
	public void changeCameraFov (bool increaseFov)
	{
		disablePlayerRunningState ();

		playerCameraManager.disableZoom ();

		if (increaseFov) {
			if (usingDualWeapon) {
				playerCameraManager.setMainCameraFov (currentRightIKWeapon.aimFovValue, currentRightIKWeapon.aimFovSpeed);
			} else {
				playerCameraManager.setMainCameraFov (currentIKWeapon.aimFovValue, currentIKWeapon.aimFovSpeed);
			}
		} else {
			if (usingDualWeapon) {
				playerCameraManager.setMainCameraFov (originalFov, currentRightIKWeapon.aimFovSpeed);
			} else {
				playerCameraManager.setMainCameraFov (originalFov, currentIKWeapon.aimFovSpeed);
			}
		}

		if (weaponsCamera.fieldOfView != originalWeaponsCameraFov) {
			if (usingDualWeapon) {
				changeWeaponsCameraFov (false, originalWeaponsCameraFov, currentRightIKWeapon.aimFovSpeed);
			} else {
				changeWeaponsCameraFov (false, originalWeaponsCameraFov, currentIKWeapon.aimFovSpeed);
			}
		}
	}

	public void changeWeaponsCameraFov (bool increaseFov, float targetFov, float speed)
	{
		if (cameraFovCoroutine != null) {
			StopCoroutine (cameraFovCoroutine);
		}
		cameraFovCoroutine = StartCoroutine (changeWeaponsCameraFovCoroutine (increaseFov, targetFov, speed));
	}

	IEnumerator changeWeaponsCameraFovCoroutine (bool increaseFov, float targetFov, float speed)
	{
		float targetValue = originalWeaponsCameraFov;
		if (increaseFov) {
			targetValue = targetFov;
		}
		while (weaponsCamera.fieldOfView != targetValue) {
			weaponsCamera.fieldOfView = Mathf.MoveTowards (weaponsCamera.fieldOfView, targetValue, Time.deltaTime * speed);
			yield return null;
		}
	}

	//used to change the parent of all the objects used for weapons in the place for first or third person
	public void setCurrentWeaponsParent (bool isFirstPerson)
	{
		bool quickDrawWeaponThirdPerson = false;
		bool quickDrawWeaponFirstPerson = false;

		checkTypeView ();

		//if the player is activating the change to first person view, check if the player was carring a weapon in third person previously
		if (isFirstPerson) {
			//then, keep that weapon quickly, without transition
			if (carryingWeaponInThirdPerson) {
				//print ("from third to first");
				carryingWeaponInThirdPerson = false;

				if (usingDualWeapon) {
					currentRightWeaponSystem.enableHUD (false);
					currentLeftWeaponSystem.enableHUD (false);

					IKManager.setIKWeaponState (false, currentRightIKWeapon.thirdPersonWeaponInfo, false);
					IKManager.setIKWeaponState (false, currentLeftIkWeapon.thirdPersonWeaponInfo, false);

					currentRightIKWeapon.quickKeepWeaponThirdPerson ();
					currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentRightWeaponSystem.setWeaponCarryState (false, true);
					currentLeftWeaponSystem.setWeaponCarryState (false, true);
				} else {
					currentWeaponSystem.enableHUD (false);

					IKManager.setIKWeaponState (false, currentIKWeapon.thirdPersonWeaponInfo, false);
			
					currentIKWeapon.quickKeepWeaponThirdPerson ();

					currentWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentWeaponSystem.setWeaponCarryState (false, true);
				}

				quickDrawWeaponFirstPerson = true;


			} else {
				if (usingDualWeapon) {
					if (currentRightIKWeapon && currentRightIKWeapon.isWeaponMoving ()) {
						currentRightIKWeapon.quickKeepWeaponThirdPerson ();
					}

					if (currentLeftIkWeapon && currentLeftIkWeapon.isWeaponMoving ()) {
						currentLeftIkWeapon.quickKeepWeaponThirdPerson ();
					}
				} else {
					//if the player was keeping his weapon while keeping his weapon, make a quick weapon keep
					if (currentIKWeapon && currentIKWeapon.isWeaponMoving ()) {
						currentIKWeapon.quickKeepWeaponThirdPerson ();
					}
				}
			}
		} else {

			//if the player is activating the change to third person view, check if the player was carrying a weapon in first person previously
			if (carryingWeaponInFirstPerson) {
				//print ("from first to third");
				carryingWeaponInFirstPerson = false;
				enableOrDisableWeaponsHUD (false);

				changingWeapon = false;
				keepingWeapon = false;

				if (usingDualWeapon) {
					currentRightIKWeapon.quickKeepWeaponFirstPerson ();
					currentLeftIkWeapon.quickKeepWeaponFirstPerson ();

					currentRightWeaponSystem.enableHUD (false);
					currentLeftWeaponSystem.enableHUD (false);
				} else {
					currentIKWeapon.quickKeepWeaponFirstPerson ();

					currentWeaponSystem.enableHUD (false);
				}

				enableOrDisableWeaponCursor (false);

				if (usingDualWeapon) {
					currentRightWeaponSystem.setWeaponAimState (false, false);
					currentLeftWeaponSystem.setWeaponAimState (false, false);
				} else {
					currentWeaponSystem.setWeaponAimState (false, false);
				}

				if (aimingInFirstPerson) {
					aimingInFirstPerson = false;
					changeCameraFov (false);
				}

				quickDrawWeaponThirdPerson = true;

				if (usingDualWeapon) {
					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentRightWeaponSystem.setWeaponCarryState (true, false);
					currentLeftWeaponSystem.setWeaponCarryState (true, false);
				} else {

					currentWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentWeaponSystem.setWeaponCarryState (true, false);
				}
			} else {
				//if the player was keeping his weapon while keeping his weapon, make a quick weapon keep
				if (usingDualWeapon) {
					if (currentRightIKWeapon && currentRightIKWeapon.isWeaponMoving ()) {
						currentRightIKWeapon.quickKeepWeaponFirstPerson ();
					}

					if (currentLeftIkWeapon && currentLeftIkWeapon.isWeaponMoving ()) {
						currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
					}
				} else {
					if (currentIKWeapon && currentIKWeapon.isWeaponMoving ()) {
						currentIKWeapon.quickKeepWeaponFirstPerson ();
					}
				}
			}
		}

		playerCameraManager.setOriginalRotationSpeed ();

		setWeaponsParent (isFirstPerson, false);

		if (quickDrawWeaponThirdPerson) {
			carryingWeaponInThirdPerson = true;

			enableOrDisableWeaponsHUD (true);

			updateWeaponHUDInfo ();

			updateAmmo ();

			if (usingDualWeapon) {
				currentRightIKWeapon.quickDrawWeaponThirdPerson ();
				currentLeftIkWeapon.quickDrawWeaponThirdPerson ();

				IKManager.setUsingDualWeaponState (true);

				IKManager.quickDrawWeaponState (currentRightIKWeapon.thirdPersonWeaponInfo);
				IKManager.quickDrawWeaponState (currentLeftIkWeapon.thirdPersonWeaponInfo);
			} else {
				currentIKWeapon.quickDrawWeaponThirdPerson ();

				IKManager.quickDrawWeaponState (currentIKWeapon.thirdPersonWeaponInfo);
			}

			enableOrDisableWeaponCursor (false);
		}

		if (usingDualWeapon) {
			if (quickDrawWeaponFirstPerson) {
				currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
				currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

				currentRightWeaponSystem.setWeaponCarryState (false, false);
				currentLeftWeaponSystem.setWeaponCarryState (false, false);

				if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable) {
					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					drawRightWeapon ();
					drawLeftWeapon ();
				}
			}
		} else {
			if (quickDrawWeaponFirstPerson) {

				currentWeaponSystem.setPauseDrawKeepWeaponSound ();

				keepingWeapon = true;
				changingWeapon = true;
			}
		}

		if (usingDualWeapon) {
			if (currentRightIKWeapon) {
				currentRightIKWeapon.checkHandsPosition ();
			}

			if (currentLeftIkWeapon) {
				currentLeftIkWeapon.checkHandsPosition ();
			}
		} else {
			if (currentIKWeapon) {
				currentIKWeapon.checkHandsPosition ();
			}
		}
	}

	//enable or disable a weapon mesh, when he drops it or pick it
	public void enableOrDisableWeaponsMesh (bool state)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			weaponsList [k].weaponGameObject.SetActive (state);
		}
	}

	public void enableOrDisableEnabledWeaponsMesh (bool state)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			if (weaponsList [k].isWeaponEnabled ()) {
				weaponsList [k].weaponGameObject.SetActive (state);
			}
		}
	}

	//check if the player can draw a weapon right now
	public bool canUseWeapons ()
	{
		bool value = false;
		if (!playerManager.isGravityPowerActive () && weaponsModeActive) {
			value = true;
		}
		return value;
	}

	//select next or previous weapon
	public void chooseNextWeapon (bool isDroppingWeapon, bool checkIfMoreThanOneWeaponAvailable)
	{
		if (!moreThanOneWeaponAvailable () && checkIfMoreThanOneWeaponAvailable) {
			return;
		}

		lockCursorAgain ();

		//check the index and get the correctly weapon 
		int max = 0;
		currentIKWeapon.setCurrentWeaponState (false);
		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();
		currentWeaponIndex++;
		if (currentWeaponIndex > weaponsSlotsAmount) {
			currentWeaponIndex = 1;
		}

		bool exit = false;
		while (!exit) {
			for (int k = 0; k < weaponsList.Count; k++) {
				if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemKeyNumber () == currentWeaponIndex) {
					choosedWeapon = k;
					exit = true;
				}
			}
			max++;
			if (max > 100) {
				return;
			}
			//get the current weapon index
			currentWeaponIndex++;
			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}
		}

		checkIfChangeWeapon (isDroppingWeapon);
	}

	public void choosePreviousWeapon (bool isDroppingWeapon, bool checkIfMoreThanOneWeaponAvailable)
	{
		if (!moreThanOneWeaponAvailable () && checkIfMoreThanOneWeaponAvailable) {
			return;
		}

		lockCursorAgain ();

		int max = 0;
		currentIKWeapon.setCurrentWeaponState (false);
		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();
		currentWeaponIndex--;
		if (currentWeaponIndex < 1) {
			currentWeaponIndex = weaponsSlotsAmount;
		}

		bool exit = false;
		while (!exit) {
			for (int k = weaponsList.Count - 1; k >= 0; k--) {
				if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemKeyNumber () == currentWeaponIndex) {
					choosedWeapon = k;
					exit = true;
				}
			}
			max++;
			if (max > 100) {
				return;
			}
			currentWeaponIndex--;
			if (currentWeaponIndex < 1) {
				currentWeaponIndex = weaponsSlotsAmount;
			}
		}

		checkIfChangeWeapon (isDroppingWeapon);
	}

	bool usingQuickDrawWeapon;

	public void checkIfChangeWeapon (bool isDroppingWeapon)
	{
		usingQuickDrawWeapon = false;

		//set the current weapon 
		if ((carryingWeaponInThirdPerson || carryingWeaponInFirstPerson || (changeToNextWeaponWhenDrop && isDroppingWeapon)) && canMove
		    && ((changeToNextWeaponWhenDrop && isDroppingWeapon) || (!changeToNextWeaponWhenDrop && !isDroppingWeapon) || (changeToNextWeaponWhenDrop && !isDroppingWeapon))) {

			if (weaponsList [choosedWeapon].isWeaponConfiguredAsDualWepaon ()) {
				checkWeaponToChangeByIndex (weaponsList [choosedWeapon], weaponsList [choosedWeapon].getWeaponSystemKeyNumber (), weaponsList [choosedWeapon].getWeaponSystemKeyNumber (), choosedWeapon);
			} else {

				checkShowWeaponSlotsParentWhenWeaponSelected (weaponsList [choosedWeapon].getWeaponSystemKeyNumber ());

				if (useQuickDrawWeapon && isThirdPersonView) {
					usingQuickDrawWeapon = true;
				} else {
					changingWeapon = true;
				}
			}
			//print ("changing weapon");
		} else {

			checkShowWeaponSlotsParentWhenWeaponSelected (weaponsList [choosedWeapon].getWeaponSystemKeyNumber ());

			weaponChanged ();

			//print ("weapon changed while not carrying any of them");
		}

		if (usingQuickDrawWeapon) {
			quicChangeWeaponThirdPersonAction ();
		}
	}

	//set the info of the selected weapon in the hud
	void weaponChanged ()
	{
		if (!usingDualWeapon && !changingDualWeapon) {
			weaponsList [choosedWeapon].setCurrentWeaponState (true);
		}

		getCurrentWeapon ();

		updateWeaponHUDInfo ();

		updateAmmo ();

		getCurrentWeaponRotation (weaponsList [choosedWeapon]);
	}

	public void updateWeaponHUDInfo ()
	{
		if (currentWeaponNameText) {
			if (usingDualWeapon || changingDualWeapon) {

				if (currentRightWeaponSystem) {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getCurrentAmmoText ();
				}

				if (currentLeftWeaponSystem) {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getCurrentAmmoText ();
				}
			} else {
				if (currentWeaponSystem) {
					currentWeaponNameText.text = currentWeaponSystem.getWeaponSystemName ();
					currentWeaponAmmoText.text = currentWeaponSystem.getCurrentAmmoText ();
					ammoSlider.maxValue = currentWeaponSystem.weaponSettings.ammoPerClip;
					ammoSlider.value = currentWeaponSystem.getWeaponClipSize ();
			
					updateElementsEnabledOnHUD ();
				}
			}
		}
	}

	public void updateElementsEnabledOnHUD ()
	{
		if (currentWeaponSystem.weaponSettings.showWeaponNameInHUD) {
			currentWeaponNameText.gameObject.SetActive (true);
		} else {
			currentWeaponNameText.gameObject.SetActive (false);
		}

		if (currentWeaponSystem.weaponSettings.showWeaponIconInHUD) {
			currentWeaponIcon.gameObject.SetActive (true);
			currentWeaponIcon.texture = currentWeaponSystem.weaponSettings.weaponIConHUD;
		} else {
			currentWeaponIcon.gameObject.SetActive (false);
		}

		if (currentWeaponSystem.weaponSettings.showWeaponAmmoSliderInHUD) {
			ammoSlider.gameObject.SetActive (true);
		} else {
			ammoSlider.gameObject.SetActive (false);
		}

		if (currentWeaponSystem.weaponSettings.showWeaponAmmoTextInHUD) {
			currentWeaponAmmoText.gameObject.SetActive (true);
		} else {
			currentWeaponAmmoText.gameObject.SetActive (false);
		}
	}

	public void updateAmmo ()
	{
		if (currentWeaponAmmoText) {
			if (usingDualWeapon || changingDualWeapon) {
				if (!currentRightWeaponSystem.weaponSettings.infiniteAmmo) {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getWeaponClipSize ().ToString () + "/" + currentRightWeaponSystem.weaponSettings.remainAmmo;
				} else {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getWeaponClipSize ().ToString () + "/" + "Inf";
				}

				if (!currentLeftWeaponSystem.weaponSettings.infiniteAmmo) {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getWeaponClipSize ().ToString () + "/" + currentLeftWeaponSystem.weaponSettings.remainAmmo;
				} else {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getWeaponClipSize ().ToString () + "/" + "Inf";
				}
			} else {
				if (currentWeaponSystem) {
					if (!currentWeaponSystem.weaponSettings.infiniteAmmo) {
						currentWeaponAmmoText.text = currentWeaponSystem.getWeaponClipSize ().ToString () + "/" + currentWeaponSystem.weaponSettings.remainAmmo;
					} else {
						currentWeaponAmmoText.text = currentWeaponSystem.getWeaponClipSize ().ToString () + "/" + "Inf";
					}

					ammoSlider.value = currentWeaponSystem.getWeaponClipSize ();
				}
			}
		}
	}

	public void enableOrDisableWeaponsHUD (bool state)
	{
		if (weaponsHUD) {
			if (currentWeaponSystem && currentWeaponSystem.weaponSettings.useCanvasHUD) {
				if (usingDualWeapon) {
					if (dualWeaponHUD) {
						dualWeaponHUD.SetActive (state);
					}

					if (singleWeaponHUD) {
						singleWeaponHUD.SetActive (false);
					}
				} else {
					if (dualWeaponHUD) {
						dualWeaponHUD.SetActive (false);
					}

					if (singleWeaponHUD) {
						singleWeaponHUD.SetActive (state);
					}
				}

				weaponsHUD.SetActive (state);

				weaponsHUDActive = state;
			}
		}
	}

	public bool isWeaponsHUDActive ()
	{
		return weaponsHUDActive;
	}

	//get the current weapon which is being used by the player right now
	public void getCurrentWeapon ()
	{
		for (int i = 0; i < weaponsList.Count; i++) {

			if (usingDualWeapon || changingDualWeapon) {
				if (weaponsList [i].getWeaponSystemName () == currentRighWeaponName) {
					print ("setting right weapon " + currentRighWeaponName);
					currentRightWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentRightIKWeapon = weaponsList [i];

					if (!currentRightIKWeapon.checkAttachmentsHUD ()) {
						setRightWeaponAttachmentPanelState (false);
					}
				} 

				if (weaponsList [i].getWeaponSystemName () == currentLeftWeaponName) {
					print ("setting left weapon " + currentLeftWeaponName);
					currentLeftWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentLeftIkWeapon = weaponsList [i];

					if (!currentLeftIkWeapon.checkAttachmentsHUD ()) {
						setLeftWeaponAttachmentPanelState (false);
					}
				} 
			} else {
				if (weaponsList [i].isCurrentWeapon ()) {
					currentWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentIKWeapon = weaponsList [i];
					currentWeaponName = currentWeaponSystem.getWeaponSystemName ();

					print ("setting current weapon to " + currentWeaponName);

					if (!currentIKWeapon.checkAttachmentsHUD ()) {
						setAttachmentPanelState (false);
					}
				} 
			}
		}
	}

	public void setWeaponByElement (IKWeaponSystem weaponToConfigure)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i] == weaponToConfigure) {
				weaponsList [i].setCurrentWeaponState (true);
				choosedWeapon = i;
			} else {
				weaponsList [i].setCurrentWeaponState (false);
			}
		}

		getCurrentWeapon ();
	}

	//check if there is any weapon which can be used by the player and set as current
	public bool checkAndSetWeaponsAvailable ()
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if ((weaponsList [i].isWeaponEnabled () && (!loadCurrentPlayerWeaponsFromSaveFile || !gameSystemManager.loadEnabled))
			    || (loadCurrentPlayerWeaponsFromSaveFile && weaponsList [i].isCurrentWeapon ())) {
				weaponsList [i].setCurrentWeaponState (true);
				choosedWeapon = i;
				return true;
			}
		}
		return false;
	}

	public bool setNextWeaponAvailableToIndex (int currentIndex)
	{
		if (currentIndex < weaponsList.Count) {

			int currentWeaponIndex = weaponsList [currentIndex].getWeaponSystemKeyNumber ();

			currentWeaponIndex++;
			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}

			bool nextWeaponFound = false;

			int max = 0;
			bool exit = false;
			while (!exit) {
				for (int k = 0; k < weaponsList.Count; k++) {
					if (!nextWeaponFound) {
						IKWeaponSystem currentWeaponToCheck = weaponsList [k];

						if (currentWeaponToCheck.isWeaponEnabled () && currentWeaponToCheck.getWeaponSystemKeyNumber () == currentWeaponIndex) {
							choosedWeapon = k;

							nextWeaponFound = true;

							currentWeaponToCheck.setCurrentWeaponState (true);

							print ("The next weapon available is " + currentWeaponToCheck.getWeaponSystemName ());
						}

						if (nextWeaponFound) {
							exit = true;
						}
					}
				}

				max++;
				if (max > 100) {
					return false;
				}

				//get the current weapon index
				currentWeaponIndex++;
				if (currentWeaponIndex > weaponsSlotsAmount) {
					currentWeaponIndex = 1;
				}
			}

			if (nextWeaponFound) {
				return true;
			}

//			for (int i = currentIndex; i < weaponsList.Count; i++) {
//				if (weaponsList [i].isWeaponEnabled ()) {
//					weaponsList [i].setCurrentWeaponState (true);
//					choosedWeapon = i;
//					return true;
//				}
//			}
		}



		return false;
	}

	public void setFirstWeaponAvailable ()
	{
		bool weaponConfigured = false;
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled () && !weaponConfigured) {
				weaponsList [i].setCurrentWeaponState (true);
				choosedWeapon = i;
				weaponConfigured = true;
			} else {
				weaponsList [i].setCurrentWeaponState (false);
			}
		}
	}

	public void setFirstWeaponWithLowerKeyNumber ()
	{
		bool anyWeaponFound = false;
		int lowerKeyNumber = 10000;
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemKeyNumber () < lowerKeyNumber) {
				lowerKeyNumber = weaponsList [i].getWeaponSystemKeyNumber ();
				anyWeaponFound = true;
			}
		}

		//print (lowerKeyNumber);

		if (anyWeaponFound) {
			for (int i = 0; i < weaponsList.Count; i++) {
				if (weaponsList [i].isWeaponEnabled () && lowerKeyNumber == weaponsList [i].getWeaponSystemKeyNumber ()) {
					weaponsList [i].setCurrentWeaponState (true);
					choosedWeapon = i;
				} else {
					weaponsList [i].setCurrentWeaponState (false);
				}
			}
		}
	}

	//check if there is any weapon which can be used by the player
	public bool checkIfWeaponsAvailable ()
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}
		return false;
	}

	//check if there is any more that one weapon which can be used, so the player doesn't change between the same weapon
	public bool moreThanOneWeaponAvailable ()
	{
		int number = 0;
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				number++;
			}
		}
		if (number > 1) {
			return true;
		} else {
			return false;
		}
	}

	//check if a weapon can be picked or is already Available to be used by the player
	public bool checkIfWeaponCanBePicked (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName && !weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}
		return false;
	}

	public bool checkIfWeaponIsAvailable (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName && weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}
		return false;
	}

	public bool hasAmmoLimit (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName && weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemManager ().hasAmmoLimit ()) {
				return true;
			}
		}
		return false;
	}

	public bool hasMaximumAmmoAmount (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName && weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemManager ().hasMaximumAmmoAmount ()) {
				return true;
			}
		}
		return false;
	}

	public int getAmmoAmountToMaximumLimit (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName && weaponsList [i].isWeaponEnabled ()) {
				return weaponsList [i].getWeaponSystemManager ().ammoAmountToMaximumLimit ();
			}
		}
		return -1;
	}

	//add ammo to a certain weapon
	public void AddAmmo (int amount, string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				weaponsList [i].getWeaponSystemManager ().getAmmo (amount);
				updateAmmo ();
				return;
			}
		}
	}

	public void addAmmoToCurrentWeapon (float amount)
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.getWeaponSystemManager ().getAmmo ((int)amount);
			currentLeftIkWeapon.getWeaponSystemManager ().getAmmo ((int)amount);

			updateAmmo ();
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.getWeaponSystemManager ().getAmmo ((int)amount);
				updateAmmo ();
			}
		}
	}

	public void addAmmoForAllWeapons (float amount)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].weaponEnabled) {
				weaponsList [i].getWeaponSystemManager ().getAmmo ((int)amount);
			}
		}
		updateAmmo ();
	}

	public void setKillInOneShootToAllWeaponsState (bool state)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			weaponsList [i].getWeaponSystemManager ().setKillOneShotState (state);
		}
	}

	public void enableAllAttachmentsInCurrentWeapon ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.enableAllAttachments ();
			currentLeftIkWeapon.enableAllAttachments ();
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.enableAllAttachments ();
			}
		}
	}

	public void enableWeaponByName (string weaponName)
	{
		//print (weaponName);
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.tryToPickUpObjectByName (weaponName);
		} else {
			pickWeapon (weaponName);
		}
	}

	public void setThirdPersonParent (Transform newParent)
	{
		thirdPersonParent = newParent;

		updateComponent ();
	}

	public void setRightHandTransform (Transform handTransform)
	{
		rightHandTransform = handTransform;

		updateComponent ();
	}

	public void setLeftHandTransform (Transform handTransform)
	{
		leftHandTransform = handTransform;

		updateComponent ();
	}

	public void setVisibleToAIState (bool state)
	{
		playerManager.setVisibleToAIState (state);
	}

	public void externalForce (Vector3 direction)
	{
		playerManager.externalForce (direction);
	}

	public void checkPlayerCanJumpWhenAimingState ()
	{
		if (!canJumpWhileAimingThirdPerson) {
			playerManager.setcanJumpActiveState (!aimingInThirdPerson);
		}
	}

	//get all the weapons configured inside the player's body
	public void setWeaponList ()
	{
		Animator anim = transform.GetChild (0).GetComponentInChildren<Animator> ();

		Transform chest = anim.GetBoneTransform (HumanBodyBones.Chest);
		Transform spine = anim.GetBoneTransform (HumanBodyBones.Spine);

		Transform weaponsParent = chest;

		if (weaponsParent == null) {
			weaponsParent = anim.GetBoneTransform (HumanBodyBones.Head).parent.parent;

			if (weaponsParent.parent != spine) {
				weaponsParent = weaponsParent.parent;
			}
		}
			
		GameObject playerCameraGameObject = playerCameraManager.gameObject;
		weaponsList.Clear ();

		Component[] components = GetComponentsInChildren (typeof(IKWeaponSystem));
		foreach (Component c in components) {
			IKWeaponSystem currentWeapon = c.GetComponent<IKWeaponSystem> ();
			currentWeapon.player = gameObject;
			currentWeapon.getWeaponSystemManager ().setCharacter (gameObject, playerCameraGameObject);
			currentWeapon.setHandTransform (rightHandTransform, leftHandTransform);
			currentWeapon.getWeaponSystemManager ().setWeaponParent (weaponsParent, anim);

			currentWeapon.setWeaponSystemManager ();

			print ("Warning: Assign weapon parent into PlayerWeaponSystem inspector of the weapon: " + currentWeapon.getWeaponSystemName ());
			playerWeaponSystem currentPlayerWeaponSystem = currentWeapon.getWeaponSystemManager ();
			if (currentPlayerWeaponSystem) {
				if (currentPlayerWeaponSystem.getWeaponsParent ()) {
					print (currentPlayerWeaponSystem.getWeaponsParent ().name + " Assigned by default, change it in the player weapon system inspector");
				} else {
					print ("Parent not assigned or assigned by using Human Bones configuration. Check it in the player weapon system inspector if there is any issue");
				}
			}

			launchTrayectory currentLaunchTrajectory = currentWeapon.weaponGameObject.GetComponentInChildren<launchTrayectory> ();
			if (currentLaunchTrajectory) {
				currentLaunchTrajectory.character = gameObject;
				currentLaunchTrajectory.characterCamera = cameraController.gameObject;
			}

			currentWeapon.getWeaponSystemManager ().getWeaponComponents ();

			weaponsList.Add (currentWeapon);
		}

		getAvailableWeaponListString ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<playerWeaponsManager> ());
		#endif
	}

	//get the list of Available weapons in the list, in case the atribute weaponEnabled is true
	public void getAvailableWeaponListString ()
	{
		int numberOfWeaponsAvailable = 0;
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				numberOfWeaponsAvailable++;
			}
		}
		if (numberOfWeaponsAvailable > 0) {
			avaliableWeaponList = new string[numberOfWeaponsAvailable];
			int currentWeaponIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++) {
				if (weaponsList [i].isWeaponEnabled ()) {
					string name = weaponsList [i].getWeaponSystemName ();
					avaliableWeaponList [currentWeaponIndex] = name;
					currentWeaponIndex++;
				}
			}
		} else {
			avaliableWeaponList = new string[0];
		}
		weaponToStartIndex = 0;

		updateComponent ();
	}

	public void getWeaponListString ()
	{
		if (weaponsList.Count > 0) {
			avaliableWeaponList = new string[weaponsList.Count];
			int currentWeaponIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++) {
				string name = weaponsList [i].getWeaponSystemName ();
				avaliableWeaponList [currentWeaponIndex] = name;
				currentWeaponIndex++;
			}
		} else {
			avaliableWeaponList = new string[0];
		}
		weaponToStartIndex = 0;

		updateComponent ();
	}

	public void changeToNextWeaponWithAmmo ()
	{
		if (usingDualWeapon) {
			return;
		}

		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

		currentWeaponIndex++;
		if (currentWeaponIndex > weaponsSlotsAmount) {
			currentWeaponIndex = 1;
		}

		bool nextWeaponFound = false;

		int max = 0;
		bool exit = false;
		while (!exit) {
			for (int k = 0; k < weaponsList.Count; k++) {
				if (!nextWeaponFound) {
					IKWeaponSystem currentWeaponToCheck = weaponsList [k];

					if (currentWeaponToCheck.isWeaponEnabled () && currentWeaponToCheck.getWeaponSystemKeyNumber () == currentWeaponIndex) {
						if (currentWeaponToCheck.isWeaponConfiguredAsDualWepaon ()) {
							if (currentWeaponToCheck.weaponSystemManager.hasAnyAmmo ()) { 
								choosedWeapon = k;

								nextWeaponFound = true;

								print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to dual weapon " +
								currentWeaponToCheck.getWeaponSystemName () + " and " + currentWeaponToCheck.getLinkedDualWeaponName ());
							} else if (getWeaponSystemByName (currentWeaponToCheck.getLinkedDualWeaponName ()).hasAnyAmmo ()) {

								choosedWeapon = getWeaponIndexByName (currentWeaponToCheck.getLinkedDualWeaponName ());

								nextWeaponFound = true;

								print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to dual weapon " +
								currentWeaponToCheck.getWeaponSystemName () + " and " + currentWeaponToCheck.getLinkedDualWeaponName ());
							}
						} else {
							if (currentWeaponToCheck.weaponSystemManager.hasAnyAmmo ()) {
								choosedWeapon = k;

								nextWeaponFound = true;

								print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to weapon " +
								currentWeaponToCheck.getWeaponSystemName ());

							}
						}
					}

					if (nextWeaponFound) {
						exit = true;
					}
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			//get the current weapon index
			currentWeaponIndex++;
			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}
		}

		if (nextWeaponFound) {
			if (weaponsList [choosedWeapon].isWeaponConfiguredAsDualWepaon ()) {
				checkIfChangeWeapon (false);
			} else {
				currentIKWeapon.setCurrentWeaponState (false);

				if (useQuickDrawWeapon && isThirdPersonView) {
					quicChangeWeaponThirdPersonAction ();
				} else {
					changingWeapon = true;
				}
			}
		}
	}

	public void removeWeaponFromList (int index)
	{
		for (int j = 0; j < weaponPocketList.Count; j++) {
			for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
				for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
					if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponsList [index].gameObject) {
						weaponPocketList [j].weaponOnPocketList [k].weaponList.RemoveAt (h);
						weaponsList.RemoveAt (index);

						updateComponent ();
						return;
					}
				}
			}
		}
	}

	//clear the weapon list of the player in the inspector
	public void clearWeaponList ()
	{
		weaponsList.Clear ();
		getAvailableWeaponListString ();

		updateComponent ();
	}

	//set to usable or non usable all the current weapon list in the player, used in the custom editor
	public void enableOrDisableWeaponsList (bool value)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			weaponsList [i].setWeaponEnabledState (value);
			#if UNITY_EDITOR
			EditorUtility.SetDirty (weaponsList [i]);
			#endif
		}
		getAvailableWeaponListString ();
	}

	//if the player is in aim mode, enable the upper body to rotate with the camera movement
	public void checkSetExtraRotationCoroutine (bool state)
	{
		if (changeExtraRotation != null) {
			StopCoroutine (changeExtraRotation);
		}
		changeExtraRotation = StartCoroutine (setExtraRotation (state));
	}

	IEnumerator setExtraRotation (bool state)
	{
		if (targetRotation != 0) {
			for (float t = 0; t < 1;) {
				t += Time.deltaTime;
				if (state) {
					extraRotation = Mathf.Lerp (extraRotation, targetRotation, t);
				} else {
					extraRotation = Mathf.Lerp (extraRotation, 0, t);
				}
				currentWeaponGameObject.transform.localEulerAngles = new Vector3 (0, -extraRotation, 0);
				upperBodyRotationManager.setCurrentBodyRotation (extraRotation);
				yield return null;
			}
		}
	}

	public void checkDualWeaponShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue, bool isRightWeapon)
	{
		extraAngleValue = Math.Abs (extraAngleValue);

		if (!isRightWeapon) {
			extraAngleValue = -extraAngleValue;
		}

		upperBodyRotationManager.checkShakeUpperBodyRotationCoroutine (extraAngleValue, speedValue);
	}

	public void checkShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue)
	{
		upperBodyRotationManager.checkShakeUpperBodyRotationCoroutine (extraAngleValue, speedValue);
	}

	//get the extra rotation in the upper body of the player for the current weapon
	public void getCurrentWeaponRotation (IKWeaponSystem weapon)
	{
		currentWeaponGameObject = weapon.gameObject;

		if (usingDualWeapon || changingDualWeapon || weapon.getUsingWeaponAsOneHandWieldState ()) {
			targetRotation = 0;
		} else {
			targetRotation = weapon.extraRotation;
		}
	}

	public void resetCurrentWeaponRotation (IKWeaponSystem weapon)
	{
		weapon.gameObject.transform.localEulerAngles = new Vector3 (0, 0, 0);
	}

	//used in third person to enable or disable the player's spine rotation
	public void activateOrDeactivateAimMode (bool state)
	{
		if (state) {
			if (!canFireWeaponsWithoutAiming || !aimingWeaponFromShooting || (canFireWeaponsWithoutAiming && useAimCameraOnFreeFireMode)) {
				playerCameraManager.activateAiming (powersManager.aimsettings.aimSide); 	
			}

			playerManager.enableOrDisableAiminig (true);	
		} else {
			playerCameraManager.deactivateAiming ();
			playerManager.enableOrDisableAiminig (false);
		}

		if (!isFirstPersonActive ()) {
			if (usingDualWeapon) {
				if (currentRightIKWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentRightWeaponRotationPoint (currentRightIKWeapon.thirdPersonWeaponInfo.weaponRotationPoint,
						currentRightIKWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				if (currentLeftIkWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentLeftWeaponRotationPoint (currentLeftIkWeapon.thirdPersonWeaponInfo.weaponRotationPoint,
						currentLeftIkWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				upperBodyRotationManager.setUsingDualWeaponState (true);
			} else {
				if (currentIKWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentWeaponRotationPoint (currentIKWeapon.thirdPersonWeaponInfo.weaponRotationPoint, currentIKWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				upperBodyRotationManager.setUsingDualWeaponState (false);
			}

			upperBodyRotationManager.setUsingWeaponRotationPointState (true);
		}
			
		checkSetExtraRotationCoroutine (state);
		upperBodyRotationManager.enableOrDisableIKUpperBody (state);
		upperBodyRotationManager.setCurrentBodyRotation (extraRotation);
	}

	//enable or disable the UI cursor used in weapons
	public void enableOrDisableWeaponCursor (bool value)
	{
		if (weaponCursorRegular) {
			if (carryingWeaponInThirdPerson) {
				weaponCursorRegular.SetActive (value);
				weaponCursorAimingInFirstPerson.SetActive (false);

				checkRegularCustomWeaponReticle (weaponCursorRegular, value);
			} else if (carryingWeaponInFirstPerson) {
				if (!aimingInFirstPerson) {
					weaponCursorAimingInFirstPerson.SetActive (false);
					weaponCursorRegular.SetActive (true);

					checkRegularCustomWeaponReticle (weaponCursorRegular, true);
				} else {
					if (usingSight) {
						weaponCursorAimingInFirstPerson.SetActive (false);
						weaponCursorRegular.SetActive (false);
					} else {

						if (currentIKWeapon.isHideCursorOnAimingEnabled ()) {
							checkRegularCustomWeaponReticle (weaponCursorAimingInFirstPerson, false);
						} else {
							weaponCursorAimingInFirstPerson.SetActive (value);

							checkRegularCustomWeaponReticle (weaponCursorAimingInFirstPerson, value);
						}
						weaponCursorRegular.SetActive (false);
					}
				}
			} else {
				weaponCursorRegular.SetActive (value);
				weaponCursorAimingInFirstPerson.SetActive (value);

				checkRegularCustomWeaponReticle (weaponCursorRegular, value);
			}
			if (!weaponCursorRegular.activeSelf && !weaponCursorAimingInFirstPerson.activeSelf) {
				weaponCursorUnableToShoot.SetActive (false);
			}
		}
	}

	public void enableOrDisableWeaponCursorUnableToShoot (bool state)
	{
		if (weaponCursorRegular) {

			if (usingDualWeapon) {
				if (state) {
					if (!currentRightIKWeapon.isWeaponSurfaceDetected () || !currentLeftIkWeapon.isWeaponSurfaceDetected ()) {
						return;
					}
				}
			}

			if (state) {
				weaponCursorRegularPreviouslyEnabled = weaponCursorRegular.activeSelf;
				weaponCursorAimingInFirstPersonPreviouslyEnabled = weaponCursorAimingInFirstPerson.activeSelf;

				weaponCursorRegular.SetActive (false);

				weaponCursorAimingInFirstPerson.SetActive (false);

				weaponCustomReticle.SetActive (false);
			} else {
				weaponCursorRegular.SetActive (weaponCursorRegularPreviouslyEnabled);

				weaponCursorAimingInFirstPerson.SetActive (weaponCursorAimingInFirstPersonPreviouslyEnabled);

				checkRegularCustomWeaponReticle (weaponCursorRegular, true);
			}

			weaponCursorUnableToShoot.SetActive (state);
		}
	}

	public void enableOrDisableGeneralWeaponCursor (bool state)
	{
		if (weaponCursor) {
			weaponCursor.SetActive (state);
		}
	}

	public void checkRegularCustomWeaponReticle (GameObject cursorToCheck, bool state)
	{
		if (usingDualWeapon || currentWeaponSystem) {
			if ((!usingDualWeapon && currentWeaponSystem.useCustomReticleEnabled ()) || (usingDualWeapon && currentRightWeaponSystem.useCustomReticleEnabled ())) {
				if (state) {
					weaponCustomReticle.SetActive (true);
					cursorToCheck.SetActive (false);

					if (usingDualWeapon) {
						if (currentRightWeaponSystem.useAimCustomReticleEnabled ()) {
							if (isAimingWeapons ()) {
								weaponCustomReticle.GetComponent<RawImage> ().texture = currentRightWeaponSystem.getAimCustomReticle ();
							} else {
								weaponCustomReticle.GetComponent<RawImage> ().texture = currentRightWeaponSystem.getRegularCustomReticle ();
							}
						} else {
							weaponCustomReticle.GetComponent<RawImage> ().texture = currentRightWeaponSystem.getRegularCustomReticle ();
						}
					} else {
						if (currentWeaponSystem.useAimCustomReticleEnabled ()) {
							if (isAimingWeapons ()) {
								weaponCustomReticle.GetComponent<RawImage> ().texture = currentWeaponSystem.getAimCustomReticle ();
							} else {
								weaponCustomReticle.GetComponent<RawImage> ().texture = currentWeaponSystem.getRegularCustomReticle ();
							}
						} else {
							weaponCustomReticle.GetComponent<RawImage> ().texture = currentWeaponSystem.getRegularCustomReticle ();
						}
					}
				} else {
					weaponCustomReticle.SetActive (false);
				}
			} else {
				weaponCustomReticle.SetActive (false);
				cursorToCheck.SetActive (state);
			}
		}
	}
		
	//pick a weapon unable to be used before
	public bool pickWeapon (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName ().ToLower () == weaponName.ToLower ()) {
				if (!weaponsList [i].isWeaponEnabled ()) {
					//check if the pocket of the weapon to pick already contains another active weapon
					bool weaponInPocketFound = false;
					GameObject weaponGameObject = weaponsList [i].gameObject;

					int weaponPocketIndex = -1;
					int weaponSubpocketIndex = -1;
					for (int j = 0; j < weaponPocketList.Count; j++) {
						if (!weaponInPocketFound) {
							for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
								for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
									if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponGameObject) {
										weaponPocketIndex = j;
										weaponSubpocketIndex = k;
										weaponInPocketFound = true;
									}
								}
							}
						}
					}

					bool carryingWeaponPreviously = false;
					if (isUsingWeapons ()) {
						carryingWeaponPreviously = true;
					}
	
					IKWeaponSystem currentIKWeaponSystem = currentIKWeapon;
					bool weaponToDropFound = false;
					bool canBeDropped = true;

					//when the player picks a weapon and another is going to be dropped, these are the cases that can happen:
					//-The player was using the weapon to drop, so this can happen:
					//--The option to change to weapon picked is not active, so the player just drops his weapon and the new one is enabled
					//--The option to change to weapon picked is active, so the player needs to drop his weapon and draw the new one
					//-The player was not using the weapon to drop, so it is just dropped and this can happen
					//--The weapon to drop was the current weapon
					//--The weapon to drop was not the current weapon, and this can happen:
					//---The current weapon, which doesn't changes, is being used by the player
					if (weaponPocketIndex > -1 && weaponSubpocketIndex > -1) {
						for (int j = 0; j < weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList.Count; j++) {
							if (!weaponToDropFound) {
								GameObject weaponGameObjectToDrop = weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList [j];
								IKWeaponToDrop = weaponGameObjectToDrop.GetComponent<IKWeaponSystem> ();

								if (IKWeaponToDrop.isWeaponEnabled ()) {
									weaponToDropFound = true;
									//print (IKWeaponToDrop.name + " enabled in that pocket found will be dropped");
									//-The player was using the weapon to drop, so this can happen:
									if (IKWeaponToDrop.carrying) {
										currentIKWeaponBeforeCheckPockets = currentIKWeapon;
										//print (IKWeaponToDrop.name + " was being used by the player");

										canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, setWeaponWhenPicked);
										if (!canBeDropped) {
											//print (IKWeaponToDrop.name + " can't be dropped");
											showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
											return false;
										}

										setWeaponByElement (currentIKWeaponBeforeCheckPockets);
									} 
									//-The player was not using the weapon to drop, so it is just dropped and this can happen
									else {
										//print (IKWeaponToDrop.name + " wasn't being used by the player");
										if (IKWeaponToDrop.isCurrentWeapon ()) {
											//print (IKWeaponToDrop.name + " is current weapon, so it needs to be changed");

											canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, false);
											if (!canBeDropped) {
												//print (IKWeaponToDrop.name + " can't be dropped");
												showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
												return false;
											}

											currentIKWeapon.setCurrentWeaponState (false);
										} else {
											//-The player was not using the weapon to drop, so it is just dropped
											//---The current weapon, which doesn't changes, is being used by the player
											currentIKWeaponBeforeCheckPockets = currentIKWeapon;
											//print (IKWeaponToDrop.name + " just dropped");
											if (isUsingWeapons ()) {
												//print ("previous weapon was " + currentIKWeaponBeforeCheckPockets.name);

												canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, false);
												if (!canBeDropped) {
													//print (IKWeaponToDrop.name + " can't be dropped");
													showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
													return false;
												}

												//in case the current weapon has been changed due to the function dropCurrentWeapon, select again the previous current weapon before
												//the player picked the new weapon
												setWeaponByElement (currentIKWeaponBeforeCheckPockets);
											} else {
												//before drop the weapon, it is neccessary to store the weapon that the player is using
												canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, true);
												if (!canBeDropped) {
													//print (IKWeaponToDrop.name + " can't be dropped");
													showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
													return false;
												}

												setWeaponByElement (currentIKWeaponBeforeCheckPockets);
											}
										}
									}
								}
							}
						}
					}

					//print ("taking " + weaponsList [i].name);
					//check if this picked weapon is the first one that the player has
					bool anyWeaponAvailablePreviosly = checkIfWeaponsAvailable ();

					//reset IK values
					weaponsList [i].setHandsIKTargetValue (0, 0);
					weaponsList [i].setIKWeight (0, 0);

					weaponsList [i].setWeaponEnabledState (true);

					//enable the picked weapon model in the player
					if (isThirdPersonView) {
						weaponsList [i].enableOrDisableWeaponMesh (true);
					}
						
					//get the upper body rotation of the current weapon picked
					getCurrentWeaponRotation (weaponsList [i]);	

					//set the state of weapons availables again
					anyWeaponAvailable = checkIfWeaponsAvailable ();
				
					//draw or not the picked weapon according to the main settings
					if (!setWeaponWhenPicked) {
						if (!anyWeaponAvailablePreviosly || !carryingWeaponPreviously) {
							//set the new picked weapon as the current one
							setWeaponByElement (weaponsList [i]);
						} else {
							if (weaponToDropFound) {
								//print (currentIKWeaponBeforeCheckPockets.name + " " + IKWeaponToDrop.name);
								if (currentIKWeaponBeforeCheckPockets == IKWeaponToDrop) {
									//print ("weapon to drop is the same that is being used by the player");
									setWeaponByElement (weaponsList [i]);
									carryingWeaponInFirstPerson = false;
									carryingWeaponInThirdPerson = false;
								}
							}
						}
						return true;
					}

					//the player was carring a weapon, set it to not the current weapon
					if (anyWeaponAvailablePreviosly) {
						//if (carryingWeaponPreviously) {
						setWeaponByElement (weaponsList [i]);
						//print ("disable " + currentIKWeaponSystem.name);
						//}
					} else {
						// else, the player hadn't any weapon previously, so se the current picked weapon as the current weapon
						//print ("first weapon picked");
						setWeaponByElement (weaponsList [i]);
					}

					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (carryingWeaponPreviously) {
							setWeaponByElement (currentIKWeaponSystem);
							currentIKWeaponSystem.setCurrentWeaponState (false);
							choosedWeapon = i;
							//print ("Change weapon");
							keepingWeapon = false;

							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								changingWeapon = true;
							}
						} else {
							if (anyWeaponAvailablePreviosly) {
								weaponsList [i].setCurrentWeaponState (true);
								if (!weaponToDropFound) {
									setWeaponByElement (weaponsList [i]);
								}
							}
							//print ("draw weapon");
							drawOrKeepWeapon (true);
						}
					} else {
						setWeaponByElement (weaponsList [i]);
					}
				}
				return true;
			}
		}
		return false;
	}

	public IKWeaponSystem equipWeapon (string weaponName, bool initiatingInventory, bool equippingDualWeapon, string rightWeaponName, string lefWeaponName)
	{
		checkTypeView ();

		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				if (!weaponsList [i].isWeaponEnabled () || equippingDualWeapon) {
					//check if the pocket of the weapon to pick already contains another active weapon
					bool weaponInPocketFound = false;
					GameObject weaponGameObject = weaponsList [i].gameObject;

					if (equippingDualWeapon) {
						print ("equip dual weapons: " + rightWeaponName + " and " + lefWeaponName);
					} else {
						print ("equip single weapon: " + weaponName);
					}

					int weaponPocketIndex = -1;
					int weaponSubpocketIndex = -1;

					if (!equippingDualWeapon) {
						for (int j = 0; j < weaponPocketList.Count; j++) {
							if (!weaponInPocketFound) {
								for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
									for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
										if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponGameObject) {
											weaponPocketIndex = j;
											weaponSubpocketIndex = k;
											weaponInPocketFound = true;
										}
									}
								}
							}
						}
					}

					bool carryingWeaponPreviously = false;
					if (isUsingWeapons ()) {
						carryingWeaponPreviously = true;
					}

					bool carryingDualWeaponToRemove = false;

					IKWeaponSystem currentIKWeaponSystem = currentIKWeapon;
					IKWeaponSystem IKWeaponToEquip = weaponsList [i];
					bool weaponToDropFound = false;

					if (!equippingDualWeapon) {
						if (weaponPocketIndex > -1 && weaponSubpocketIndex > -1) {
							for (int j = 0; j < weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList.Count; j++) {
								if (!weaponToDropFound) {
									GameObject weaponGameObjectToDrop = weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList [j];
									IKWeaponToDrop = weaponGameObjectToDrop.GetComponent<IKWeaponSystem> ();

									if (IKWeaponToDrop.isWeaponEnabled ()) {
										weaponToDropFound = true;
										//print (IKWeaponToDrop.name + " enabled in that pocket found will be dropped");

										if (usingDualWeapon) {
											if (currentRightIKWeapon == IKWeaponToDrop) {
												currentIKWeapon = currentRightIKWeapon;
												currentWeaponSystem = currentRightWeaponSystem;

												disableCurrentWeapon ();
											}

											if (currentLeftIkWeapon == IKWeaponToDrop) {
												currentIKWeapon = currentLeftIkWeapon;
												currentWeaponSystem = currentLeftWeaponSystem;

												disableCurrentWeapon ();
											}
										} else {
											if (currentIKWeaponSystem == IKWeaponToDrop) {
												disableCurrentWeapon ();
											}
										}

										IKWeaponToDrop.setWeaponEnabledState (false);

										IKWeaponToDrop.setCurrentWeaponState (false);

										IKWeaponToDrop.enableOrDisableWeaponMesh (false);

										playerInventoryManager.unEquipObjectByName (IKWeaponToDrop.getWeaponSystemName ());

										print (IKWeaponToDrop.name + " is on the same pocket as " + weaponName + " so it will be unequipped");

										bool weaponToRemoveIsConfiguredAsDual = IKWeaponToDrop.isWeaponConfiguredAsDualWepaon ();

										if (weaponToRemoveIsConfiguredAsDual) {
											print (IKWeaponToDrop.name + " was configured as dual weapon with " + IKWeaponToDrop.getLinkedDualWeaponName ());

											IKWeaponSystem secondaryWeaponToSetAsSingle = getWeaponSystemByName (IKWeaponToDrop.getLinkedDualWeaponName ()).getIKWeaponSystem ();

											IKWeaponToDrop.setWeaponConfiguredAsDualWepaonState (false, "");
											IKWeaponToDrop.setUsingDualWeaponState (false);

											secondaryWeaponToSetAsSingle.setWeaponConfiguredAsDualWepaonState (false, "");
											secondaryWeaponToSetAsSingle.setUsingDualWeaponState (false);
											
											if (storePickedWeaponsOnInventory) {
												playerInventoryManager.updateSingleWeaponSlotInfoWithoutAddingAnotherSlot (secondaryWeaponToSetAsSingle.getWeaponSystemName ());

												updateCurrentChoosedDualWeaponIndex ();
											}

											if (usingDualWeapon) {
												if (currentRighWeaponName == IKWeaponToDrop.name || currentLeftWeaponName == IKWeaponToDrop.name) {
													print ("carrying the weapon to remove " + IKWeaponToDrop.name + " as dual weapon");
													carryingDualWeaponToRemove = true;
												}
											}
										}
									}
								}
							}
						}
					}

					//print ("taking " + IKWeaponToEquip.name);
					//check if this picked weapon is the first one that the player has
					bool anyWeaponAvailablePreviosly = checkIfWeaponsAvailable ();

					//reset IK values
					IKWeaponToEquip.setHandsIKTargetValue (0, 0);
					IKWeaponToEquip.setIKWeight (0, 0);

					IKWeaponToEquip.setWeaponEnabledState (true);

					//enable the picked weapon model in the player

					if (initiatingInventory) {
						checkTypeViewBeforeStart ();
					}

					if (isThirdPersonView && drawWeaponWhenPicked && !IKWeaponToEquip.hideWeaponIfKeptInThirdPerson) {
						IKWeaponToEquip.enableOrDisableWeaponMesh (true);
					}

					if (!isThirdPersonView && initiatingInventory) {
						IKWeaponToEquip.enableOrDisableWeaponMesh (false);
					}

					//get the upper body rotation of the current weapon picked
					getCurrentWeaponRotation (IKWeaponToEquip);	

					//set the state of weapons availables again
					anyWeaponAvailable = checkIfWeaponsAvailable ();

					//draw or not the picked weapon according to the main settings
					if ((!changeToNextWeaponWhenEquipped && !drawWeaponWhenPicked) || initiatingInventory) {
						if (!anyWeaponAvailablePreviosly || !carryingWeaponPreviously) {

							if (equippingDualWeapon) {
								if (currentRightIKWeapon) {
									currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								}

								if (currentLeftIkWeapon) {
									currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								}

								currentRighWeaponName = rightWeaponName;
								currentLeftWeaponName = lefWeaponName;

								setCurrentRightIkWeaponByName (currentRighWeaponName);
								setCurrentLeftIKWeaponByName (currentLeftWeaponName);

								currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (true, currentLeftIkWeapon.getWeaponSystemName ());
								currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (true, currentRightIKWeapon.getWeaponSystemName ());
							} else {
								//set the new picked weapon as the current one
								IKWeaponToEquip.getWeaponSystemName ();
								setWeaponByElement (IKWeaponToEquip);
							}
						}

						return IKWeaponToEquip;
					}

					if (!equippingDualWeapon && !carryingDualWeaponToRemove) {
						//the player was carring a weapon, set it to not the current weapon
						if (anyWeaponAvailablePreviosly) {
							setWeaponByElement (IKWeaponToEquip);
							//print ("disable " + currentIKWeaponSystem.name);
						} else {
							// else, the player hadn't any weapon previously, so se the current picked weapon as the current weapon
							//print ("first weapon picked");
							setWeaponByElement (IKWeaponToEquip);
						}
					}

					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (equippingDualWeapon) {
							equippingDualWeaponsFromInventoryMenu = true;

							if (currentIKWeaponSystem && currentIKWeaponSystem != currentRightIKWeapon && currentIKWeaponSystem != currentLeftIkWeapon) {
								currentIKWeaponSystem.setCurrentWeaponState (false);
							}

							changeDualWeapons (rightWeaponName, lefWeaponName);
						} else {
							if (carryingWeaponPreviously) {

								if (usingDualWeapon) {
									if (carryingDualWeaponToRemove) {
										IKWeaponToDrop.setHandsIKTargetValue (0, 0);
										IKWeaponToDrop.setIKWeight (0, 0);

										currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
										currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
									}

									print ("previously using dual weapons");

									settingSingleWeaponFromNumberKeys = true;

									singleWeaponNameToChangeFromNumberkeys = IKWeaponToEquip.getWeaponSystemName ();
									changeSingleWeapon (IKWeaponToEquip.getWeaponSystemName ());
								} else {
									setWeaponByElement (currentIKWeaponSystem);
									currentIKWeaponSystem.setCurrentWeaponState (false);
									choosedWeapon = i;

									//print ("Change weapon");
									keepingWeapon = false;
						
									if (useQuickDrawWeapon && isThirdPersonView) {
										quicChangeWeaponThirdPersonAction ();
									} else {
										changingWeapon = true;
									}
								}
							} else {
								IKWeaponToEquip.setCurrentWeaponState (true);

								if (anyWeaponAvailablePreviosly) {
								
									if (!weaponToDropFound) {
										setWeaponByElement (IKWeaponToEquip);
									}
								}

								//print ("draw weapon");
								drawOrKeepWeapon (true);
							}
						}
					} else {
						if (!equippingDualWeapon) {
							setWeaponByElement (IKWeaponToEquip);
						}
					}
				}

				return weaponsList [i];
		
			}
		}

		return null;
	}

	public void unequipWeapon (string weaponName, bool unequippingDualWeapon)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				if (weaponsList [i].isWeaponEnabled ()) {

					if (unequippingDualWeapon) {
						print ("unequipping Dual Weapon: " + weaponName + " to leave equipped " + currentRighWeaponName);
					} else {
						print ("unequipping single Weapon: " + weaponName);
					}

					//print ("unequip " + weaponName);
					IKWeaponSystem currentIKWeaponSystem = weaponsList [i];

					bool carryinWeaponToUnequip = false;

					if (usingDualWeapon) {
						if (currentRightIKWeapon == currentIKWeaponSystem) {
							currentIKWeapon = currentRightIKWeapon;
							currentWeaponSystem = currentRightWeaponSystem;

							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}

						if (currentLeftIkWeapon == currentIKWeaponSystem) {
							currentIKWeapon = currentLeftIkWeapon;
							currentWeaponSystem = currentLeftWeaponSystem;

							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}

						if (!carryinWeaponToUnequip) {
							currentIKWeapon = currentIKWeaponSystem;
							currentWeaponSystem = currentIKWeaponSystem.getWeaponSystemManager ();

							disableCurrentWeapon ();
						}
					} else {
						if (currentIKWeapon == currentIKWeaponSystem) {
							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}
					}

					currentIKWeaponSystem.setWeaponEnabledState (false);
					currentIKWeaponSystem.setCurrentWeaponState (false);

					//enable the picked weapon model in the player
					currentIKWeaponSystem.enableOrDisableWeaponMesh (false);

					playerInventoryManager.unEquipObjectByName (currentIKWeaponSystem.getWeaponSystemName ());

					//reset IK values
					currentIKWeaponSystem.setHandsIKTargetValue (0, 0);
					currentIKWeaponSystem.setIKWeight (0, 0);
				
					//print ("disabling " + currentIKWeaponSystem.name);
					if (!unequippingDualWeapon) {
						bool anyWeaponAvailableCurrently = checkIfWeaponsAvailable ();

						if (carryinWeaponToUnequip && anyWeaponAvailableCurrently) {
						
							if (setNextWeaponAvailableToIndex (i)) {
								weaponChanged ();

								chooseDualWeaponIndex = currentWeaponSystem.getWeaponNumberKey ();
							}
						}

						if (!anyWeaponAvailableCurrently) {
							playerInventoryManager.disableCurrentlySelectedIcon ();
						}

						//draw or not the picked weapon according to the main settings
						if (!changeToNextWeaponWhenUnequipped || !anyWeaponAvailableCurrently) {
							return;
						}
					}
						
					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (unequippingDualWeapon) {
							if (carryinWeaponToUnequip) {
								print ("carrying current dual weapons to unequip " + currentIKWeaponSystem.getWeaponSystemName ());
								currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

								changeSingleWeapon (currentRighWeaponName);
							} else {
								print ("not carrying current dual weapons to unequip " + currentIKWeaponSystem.getWeaponSystemName ());
								IKWeaponSystem secondaryWeaponToSetAsSingle = getWeaponSystemByName (currentIKWeaponSystem.getLinkedDualWeaponName ()).getIKWeaponSystem ();

								secondaryWeaponToSetAsSingle.setWeaponConfiguredAsDualWepaonState (false, "");
								currentIKWeaponSystem.setWeaponConfiguredAsDualWepaonState (false, "");
							}
						} else {
							if (carryinWeaponToUnequip && !isPlayerCarringWeapon ()) {
								//print ("draw next weapon");
								drawOrKeepWeapon (true);
							}
						}
					}
				} else {
					playerInventoryManager.unEquipObjectByName (weaponsList [i].getWeaponSystemName ());
				}

				return;
			}
		}

		return;
	}

	public void checkTypeViewBeforeStart ()
	{
		checkTypeView ();
	}

	//the player has dead, so set his state in the weapons
	public void setDeadState (bool state)
	{
		playerIsDead = state;
		if (state) {
			lockCursorAgain ();
			dropWeaponWhenPlayerDies ();
		} else {
			if (anyWeaponAvailable) {
				getCurrentWeaponRotation (currentIKWeapon);
				extraRotation = 0;
			}
		}
	}

	public bool isDrawWeaponWhenResurrectActive ()
	{
		return drawWeaponWhenResurrect;
	}

	//drop a weapon, so it is disabled and the player can't use it until that weapon is picked again
	public void dropWeapon ()
	{
		//&& (!aimingInFirstPerson && !aimingInThirdPerson) 
		if (currentWeaponSystem && canDropWeapons) {

			bool canBeDropped = false;
			if (usingDualWeapon) {
				canBeDropped = dropCurrentDualWeapon (currentIKWeapon, false, true);
			} else {
				canBeDropped = dropCurrentWeapon (currentIKWeapon, false, true);
			}

			if (!canBeDropped) {
				showObjectMessage (currentIKWeapon.getWeaponSystemName () + " " + cantDropCurrentWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
			}
		}
	}

	public void dropWeaponByBebugButton ()
	{
		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
				dropWeapon ();
			}
		}
	}

	//drop the weapons when the player dies, according to the configuration in the inspector
	public void dropWeaponWhenPlayerDies ()
	{
		if (dropCurrentWeaponWhenDie || dropAllWeaponsWhenDie) {
			if (!weaponsModeActive || (dropWeaponsOnlyIfUsing && !carryingWeaponInThirdPerson && !carryingWeaponInFirstPerson)) {
				return;
			}
			if (dropAllWeaponsWhenDie) {
				for (int k = 0; k < weaponsList.Count; k++) {
					if (weaponsList [k].isWeaponEnabled ()) {
						dropCurrentWeapon (weaponsList [k], true, true);
					}
				}
			} else {
				if (dropCurrentWeaponWhenDie) {
					if (currentIKWeapon.weaponEnabled) {
						dropCurrentWeapon (currentIKWeapon, true, true);
					}
				}
			}
		}
	}

	//drop the current weapon that the player is carrying, in third or first person
	public bool dropCurrentWeapon (IKWeaponSystem weaponToDrop, bool ignoreRagdollCollision, bool checkChangeToNextWeapon)
	{
		if (weaponToDrop == null || !weaponToDrop.canBeDropped) {
			return false;
		}

		lockCursorAgain ();

		launchWeaponRigidbodyWhenDropWeapon (weaponToDrop, ignoreRagdollCollision);

		if (checkChangeToNextWeapon) {
			//if player dies and he is aiming, disable that state
			if (isAimingWeapons ()) {
				disableCurrentWeapon ();
			}
				
			if (!useQuickDrawWeapon) {
				//set the states in the weapons manager to search the next weapon to use
				if (moreThanOneWeaponAvailable ()) {
					chooseNextWeapon (true, true);
				} else {
					weaponToDrop.setCurrentWeaponState (false);
				}
			}
		} else {
			if (isAimingWeapons ()) {
				changeCameraFov (false);
			}
		}

		if (isThirdPersonView) {
			weaponToDrop.enableOrDisableWeaponMesh (false);
			if (checkChangeToNextWeapon) {
				carryingWeaponInThirdPerson = false;
			}
			weaponToDrop.quickKeepWeaponThirdPerson ();
			if (checkChangeToNextWeapon) {
				IKManager.quickKeepWeaponState ();
			}
		} else {
			if (checkChangeToNextWeapon) {
				carryingWeaponInFirstPerson = false;
			}
			weaponToDrop.quickKeepWeaponFirstPerson ();
			weaponToDrop.enableOrDisableFirstPersonArms (false);
		}

		playerCameraManager.setOriginalRotationSpeed ();

		weaponToDrop.setWeaponEnabledState (false);
		currentWeaponSystem.setWeaponCarryState (false, false);
		enableOrDisableWeaponsHUD (false);
		enableOrDisableWeaponCursor (false);
		currentWeaponSystem.enableHUD (false);

		aimingInFirstPerson = false;
		aimingInThirdPerson = false;

		checkPlayerCanJumpWhenAimingState ();

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.dropEquipByName (weaponToDrop.getWeaponSystemName (), 1);
		}

		resetCurrentWeaponRotation (weaponToDrop);
	
		if (!canMove) {
			getCurrentWeapon ();
		} else {
			if (checkChangeToNextWeapon && useQuickDrawWeapon) {
				//set the states in the weapons manager to search the next weapon to use

				if (checkIfWeaponsAvailable ()) {
					chooseNextWeapon (true, false);
				} else {
					weaponToDrop.setCurrentWeaponState (false);
				}
			}
		}
			
		return true;
	}

	public bool dropCurrentDualWeapon (IKWeaponSystem weaponToDrop, bool ignoreRagdollCollision, bool checkChangeToNextWeapon)
	{
		if (weaponToDrop == null || !weaponToDrop.canBeDropped) {
			return false;
		}

		lockCursorAgain ();

		launchWeaponRigidbodyWhenDropWeapon (weaponToDrop, ignoreRagdollCollision);

		if (weaponToDrop == currentRightIKWeapon) {
			launchWeaponRigidbodyWhenDropWeapon (currentLeftIkWeapon, ignoreRagdollCollision);
		} else {
			launchWeaponRigidbodyWhenDropWeapon (currentRightIKWeapon, ignoreRagdollCollision);
		}
			
		//if player dies and he is aiming, disable that state
		disableCurrentDualWeapon ();

		currentRightIKWeapon.setCurrentWeaponState (false);
		currentLeftIkWeapon.setCurrentWeaponState (false);

		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

		usingDualWeapon = false;

		disableDualWeaponStateOnWeapons ();

		if (isAimingWeapons ()) {
			changeCameraFov (false);
		}

		if (isThirdPersonView) {
			currentRightIKWeapon.enableOrDisableWeaponMesh (false);
			currentLeftIkWeapon.enableOrDisableWeaponMesh (false);

			if (checkChangeToNextWeapon) {
				carryingWeaponInThirdPerson = false;
			}

			currentRightIKWeapon.quickKeepWeaponThirdPerson ();
			currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

			if (checkChangeToNextWeapon) {
				IKManager.quickKeepWeaponState ();
			}
		} else {
			if (checkChangeToNextWeapon) {
				carryingWeaponInFirstPerson = false;
			}

			currentRightIKWeapon.quickKeepWeaponFirstPerson ();
			currentRightIKWeapon.enableOrDisableFirstPersonArms (false);

			currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
			currentLeftIkWeapon.enableOrDisableFirstPersonArms (false);
		}

		playerCameraManager.setOriginalRotationSpeed ();

		currentRightIKWeapon.setWeaponEnabledState (false);
		currentRightWeaponSystem.setWeaponCarryState (false, false);

		currentLeftIkWeapon.setWeaponEnabledState (false);
		currentLeftWeaponSystem.setWeaponCarryState (false, false);

		enableOrDisableWeaponsHUD (false);
		enableOrDisableWeaponCursor (false);

		currentRightWeaponSystem.enableHUD (false);
		currentLeftWeaponSystem.enableHUD (false);

		aimingInFirstPerson = false;
		aimingInThirdPerson = false;

		checkPlayerCanJumpWhenAimingState ();

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.dropEquipByName (currentRightIKWeapon.getWeaponSystemName (), 1);
			playerInventoryManager.dropEquipByName (currentLeftIkWeapon.getWeaponSystemName (), 1);
		}

		resetCurrentWeaponRotation (weaponToDrop);

		if (!canMove) {
			getCurrentWeapon ();
		} else {
			if (checkChangeToNextWeapon) {
				//set the states in the weapons manager to search the next weapon to use
				if (checkIfWeaponsAvailable ()) {
					chooseNextWeapon (true, false);
				}
			}
		}

		return true;
	}

	public void launchWeaponRigidbodyWhenDropWeapon (IKWeaponSystem weaponToDrop, bool ignoreRagdollCollision)
	{
		Vector3 position = weaponToDrop.weaponTransform.position;
		Quaternion rotation = weaponToDrop.weaponTransform.rotation;

		GameObject weaponPickupToInstantiate = weaponToDrop.weaponPrefabModel;

		if (storePickedWeaponsOnInventory) {
			weaponPickupToInstantiate = weaponToDrop.inventoryWeaponPrefabObject;
		}

		//instantiate and drag the weapon object
		GameObject weaponClone = (GameObject)Instantiate (weaponPickupToInstantiate, position, rotation);
		Collider weaponToDropCollider = weaponClone.GetComponent<Collider> ();

		Physics.IgnoreCollision (weaponToDropCollider, mainCollider, true);

		Rigidbody weaponRigidbody = weaponClone.GetComponent<Rigidbody> ();
		weaponRigidbody.isKinematic = true;

		weaponClone.transform.position = position;
		weaponClone.transform.rotation = rotation;

		weaponRigidbody.isKinematic = false;

		Vector3 forceDirection = transform.forward * dropWeaponForceThirdPerson;
		if (holdingDropButtonToIncreaseForce) {
			forceDirection = transform.forward * currentDropForce;
		}

		if (ignoreRagdollCollision) {
			forceDirection = weaponToDrop.weaponTransform.position - transform.position;
			float distance = forceDirection.magnitude;
			forceDirection = forceDirection / distance;
		}

		if (!isThirdPersonView) {
			forceDirection = playerCameraManager.mainCameraTransform.forward * dropWeaponForceFirstPerson;

			if (holdingDropButtonToIncreaseForce) {
				forceDirection = playerCameraManager.mainCameraTransform.forward * currentDropForce;
			}
		}
		weaponRigidbody.AddForce (forceDirection);

		if (holdingDropButtonToIncreaseForce) {
			weaponClone.AddComponent<launchedObjects> ().setCurrentPlayer (gameObject);
		}

		//if the player dies, ignore collision between the current dropped weapon and the player's ragdoll
		if (ignoreRagdollCollision) {
			List <Collider> ragdollColliders = ragdollManager.getBodyColliderList ();

			for (int k = 0; k < ragdollColliders.Count; k++) {
				Physics.IgnoreCollision (weaponToDropCollider, ragdollColliders [k]);
			}
		}
	}

	public void activateSecondaryAction ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryAction ();
			currentLeftWeaponSystem.activateSecondaryAction ();
		} else {
			currentWeaponSystem.activateSecondaryAction ();
		}
	}

	public void activateSecondaryActionOnDownPress ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnDownPress ();
			currentLeftWeaponSystem.activateSecondaryActionOnDownPress ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnDownPress ();
		}
	}

	public void activateSecondaryActionOnUpPress ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnUpPress ();
			currentLeftWeaponSystem.activateSecondaryActionOnUpPress ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnUpPress ();
		}
	}

	public void activateSecondaryActionOnUpHold ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnUpHold ();
			currentLeftWeaponSystem.activateSecondaryActionOnUpHold ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnUpHold ();
		}
	}

	//a quick function to keep the current weapon that the player is using (if he is using one), for example, when he enters in a vehicle. It is called from playerStatesManager
	public void disableCurrentWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			currentIKWeapon.stopWeaponMovement ();

			if (aimingInThirdPerson) {
				//print ("deactivate");
				activateOrDeactivateAimMode (false);
				IKManager.disableIKWeight ();
			}

			if (aimingInFirstPerson) {
				playerManager.enableOrDisableAiminig (false);
			}

			changeCameraFov (false);

			currentWeaponSystem.setPauseDrawKeepWeaponSound ();

			currentWeaponSystem.setWeaponAimState (false, false);
			currentWeaponSystem.setWeaponCarryState (false, false);

			aimingInFirstPerson = false;
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			if (carryingWeaponInThirdPerson) {
				carryingWeaponInThirdPerson = false;
				currentIKWeapon.quickKeepWeaponThirdPerson ();

				if (!usingDualWeapon) {
					IKManager.quickKeepWeaponState ();
					IKManager.setUsingWeaponsState (false);
					IKManager.setDisableWeaponsState (false);
				}
			} 

			if (carryingWeaponInFirstPerson) {
				carryingWeaponInFirstPerson = false;
				currentIKWeapon.quickKeepWeaponFirstPerson ();
				currentIKWeapon.enableOrDisableFirstPersonArms (false);
			}

			enableOrDisableWeaponsHUD (false);
			currentWeaponSystem.enableHUD (false);
			enableOrDisableWeaponCursor (false);

			playerCameraManager.setOriginalRotationSpeed ();

			if (usingDualWeapon) {
				if (currentRightIKWeapon.isCurrentWeapon () && currentLeftIkWeapon.isCurrentWeapon ()) {
					if (isFirstPersonActive ()) {
						carryingWeaponInFirstPerson = true;
					} else {
						carryingWeaponInThirdPerson = true;
					}
				}
			}
		}
	}

	public void disableCurrentDualWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			currentRightIKWeapon.stopWeaponMovement ();
			currentLeftIkWeapon.stopWeaponMovement ();

			if (aimingInThirdPerson) {
				//print ("deactivate");
				activateOrDeactivateAimMode (false);
				IKManager.disableIKWeight ();
			}

			if (aimingInFirstPerson) {
				playerManager.enableOrDisableAiminig (false);
			}

			changeCameraFov (false);

			currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
			currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

			currentRightWeaponSystem.setWeaponAimState (false, false);
			currentRightWeaponSystem.setWeaponCarryState (false, false);

			currentLeftWeaponSystem.setWeaponAimState (false, false);
			currentLeftWeaponSystem.setWeaponCarryState (false, false);

			aimingInFirstPerson = false;
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			if (carryingWeaponInThirdPerson) {
				carryingWeaponInThirdPerson = false;

				currentRightIKWeapon.quickKeepWeaponThirdPerson ();
				currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

				if (!usingDualWeapon) {
					IKManager.quickKeepWeaponState ();
					IKManager.setUsingWeaponsState (false);
					IKManager.setDisableWeaponsState (false);
				}
			} 

			if (carryingWeaponInFirstPerson) {
				carryingWeaponInFirstPerson = false;

				currentRightIKWeapon.quickKeepWeaponFirstPerson ();
				currentRightIKWeapon.enableOrDisableFirstPersonArms (false);

				currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
				currentLeftIkWeapon.enableOrDisableFirstPersonArms (false);
			}

			enableOrDisableWeaponsHUD (false);
			currentWeaponSystem.enableHUD (false);
			enableOrDisableWeaponCursor (false);

			playerCameraManager.setOriginalRotationSpeed ();

			usingDualWeapon = false;

			disableDualWeaponStateOnWeapons ();
		}
	}

	public void checkIfDisableCurrentWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			if (usingDualWeapon) {

				usingDualWeaponsPreviously = true;
				disableCurrentDualWeapon ();
			} else {
				disableCurrentWeapon ();
			}
		}
	}

	//set the weapons mode in third or first person from the player camera component, only in editor mode
	public void getPlayerWeaponsManagerComponents (bool isFirstPerson)
	{
		getComponents ();

		anyWeaponAvailable = checkAndSetWeaponsAvailable ();
		if (anyWeaponAvailable) {
			getCurrentWeapon ();
			getCurrentWeaponRotation (currentIKWeapon);
		}

		originalFov = mainCamera.fieldOfView;
		touchPlatform = touchJoystick.checkTouchPlatform ();
		setHudZone ();
		originalWeaponsCameraFov = weaponsCamera.fieldOfView;
	}

	//get the components in player used in the script
	public void getComponents ()
	{
		originalWeaponsPositionThirdPerson = weaponsTransformInThirdPerson.localPosition;
		originalWeaponsRotationThirdPerson = weaponsTransformInThirdPerson.localRotation;

		originalWeaponsPositionFirstPerson = weaponsTransformInFirstPerson.localPosition;
		originalWeaponsRotationFirstPerson = weaponsTransformInFirstPerson.localRotation;
	}

	public Transform getMainCameraTransform ()
	{
		return mainCameraTransform;
	}

	//change the weapons parent between third and first person
	public void setWeaponsParent (bool isFirstPerson, bool settingInEditor)
	{
		weaponsCamera.enabled = isFirstPerson;

		string newLayer = "Default";

		if (isFirstPerson) {
			weaponsParent.SetParent (firstPersonParent);
			weaponsParent.localRotation = originalWeaponsRotationFirstPerson;
			weaponsParent.localPosition = originalWeaponsPositionFirstPerson;
			newLayer = weaponsLayer;
		} else {
			weaponsParent.SetParent (thirdPersonParent);
			weaponsParent.localPosition = originalWeaponsPositionThirdPerson;
			weaponsParent.localRotation = originalWeaponsRotationThirdPerson;
			//print ("third person");
		}

		for (int k = 0; k < weaponsList.Count; k++) {
			weaponsList [k].getWeaponSystemManager ().enableHUDTemporarily (true);

			Component[] components = weaponsList [k].getWeaponSystemManager ().weaponSettings.weaponMesh.GetComponentsInChildren (typeof(Transform));
			foreach (Component c in components) {
				c.gameObject.layer = LayerMask.NameToLayer (newLayer);
			}

			weaponsList [k].getWeaponSystemManager ().enableHUDTemporarily (false);

			if (weaponsList [k].isWeaponEnabled ()) {
				if (isFirstPerson || !weaponsList [k].hideWeaponIfKeptInThirdPerson) {
					weaponsList [k].enableOrDisableWeaponMesh (!isFirstPerson);
				}
			}

			Transform weaponTransform = weaponsList [k].weaponTransform;
			if (weaponTransform == null) {
				weaponTransform = weaponsList [k].weaponGameObject.transform;
			}

			if (!isFirstPerson) {
				if (settingInEditor) {
					weaponTransform.SetParent (weaponsList [k].transform);
				} else {
					weaponTransform.SetParent (weaponsList [k].thirdPersonWeaponInfo.keepPosition.parent);
				}
				weaponTransform.localPosition = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localRotation;
				weaponsList [k].enableOrDisableFirstPersonArms (false);
				weaponsList [k].getWeaponSystemManager ().weaponSettings.weaponMesh.transform.localPosition = Vector3.zero;
				weaponsList [k].getWeaponSystemManager ().weaponSettings.weaponMesh.transform.localRotation = Quaternion.identity;
			} else {
				weaponTransform.SetParent (weaponsList [k].transform);
				weaponTransform.localPosition = weaponsList [k].firstPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = weaponsList [k].firstPersonWeaponInfo.keepPosition.localRotation;
			}
			weaponsList [k].getWeaponSystemManager ().changeHUDPosition (!isFirstPerson);

			weaponsList [k].setCurrentSwayInfo (true);
		}

		IKManager.stopIKWeaponsActions ();
	}

	public void setWeaponPartLayer (GameObject weaponPart)
	{
		string newLayer = "Default";

		if (isFirstPersonActive ()) {
			newLayer = weaponsLayer;
		} 
		Component[] components = weaponPart.GetComponentsInChildren (typeof(Transform));
		foreach (Component c in components) {
			c.gameObject.layer = LayerMask.NameToLayer (newLayer);
		}
	}

	//check if the player is carrying weapons in third or first person
	public bool isUsingWeapons ()
	{
		return carryingWeaponInFirstPerson || carryingWeaponInThirdPerson;
	}

	public bool isAimingWeapons ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public void setWeaponToStartGame (string weaponName)
	{
		setWeaponByName (weaponName);
	}

	public void setWeaponByName (string weaponName)
	{
		currentIKWeapon.setCurrentWeaponState (false);

		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {

				weaponsList [i].setCurrentWeaponState (true);

				anyWeaponAvailable = true;
				choosedWeapon = i;

				currentWeaponSystem = weaponsList [i].getWeaponSystemManager ();

				currentIKWeapon = weaponsList [i];

				currentWeaponName = currentWeaponSystem.getWeaponSystemName ();

				print ("current IK Weapon " + currentIKWeapon.getWeaponSystemName ());
			} 
		}
	}

	public playerWeaponSystem getWeaponSystemByName (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				return weaponsList [i].getWeaponSystemManager ();
			}
		}

		return null;
	}

	public int getWeaponIndexByName (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				return i;
			}
		}

		return -1;
	}

	public void changeCurrentWeaponByName (string weaponName)
	{
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].getWeaponSystemName () == weaponName) {
				checkWeaponToChangeByIndex (weaponsList [i], weaponsList [i].getWeaponSystemKeyNumber (), weaponsList [i].getWeaponSystemKeyNumber (), i);
			}
		}
	}

	public void enableOrDisableGrabObjectsManager (bool state)
	{
		if (canGrabObjectsCarryingWeapons) {
			if (grabObjectsManager) {
				grabObjectsManager.setAimingState (state);
				if (!state) {
					grabObjectsManager.dropObject ();
				}
			}
		}
	}

	public void setCarryingPhysicalObjectState (bool state)
	{
		carryingPhysicalObject = state;
	}

	public void setShotCameraNoise (Vector2 noiseAmount)
	{
		playerCameraManager.setShotCameraNoise (noiseAmount);
	}

	public void addPocket ()
	{
		weaponPocket newPocket = new weaponPocket ();
		newPocket.Name = "New Pocket";
		weaponPocketList.Add (newPocket);

		updateComponent ();
	}

	public void addSubPocket (int index)
	{
		weaponListOnPocket newSubPocket = new weaponListOnPocket ();
		newSubPocket.Name = "New Subpocket";
		weaponPocketList [index].weaponOnPocketList.Add (newSubPocket);

		updateComponent ();
	}

	public void showCantPickAttacmentMessage (string attachmentName)
	{
		showObjectMessage (attachmentName + " " + cantPickAttachmentMessage, weaponMessageDuration, weaponsMessageWindow);
	}

	Coroutine weaponMessageCoroutine;

	public void showObjectMessage (string message, float messageDuration, GameObject messagePanel)
	{
		if (weaponMessageCoroutine != null) {
			StopCoroutine (weaponMessageCoroutine);
		}
		weaponMessageCoroutine = StartCoroutine (showObjectMessageCoroutine (message, messageDuration, messagePanel));
	}

	IEnumerator showObjectMessageCoroutine (string info, float messageDuration, GameObject messagePanel)
	{
		usingDevicesManager.checkDeviceName ();
		messagePanel.SetActive (true);
		messagePanel.GetComponentInChildren<Text> ().text = info;

		yield return new WaitForSeconds (messageDuration);

		messagePanel.SetActive (false);
	}

	public List<IKWeaponSystem> getPlayerWeaponList ()
	{
		return weaponsList;
	}

	public IKWeaponSystem getIKWeaponSystem (string weaponName)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			if (weaponsList [k].getWeaponSystemName () == weaponName) {
				return weaponsList [k];
			}
		}
		return null;
	}

	public persistanceWeaponListByPlayerInfo getPersistanceWeaponInfoList ()
	{
		persistanceWeaponListByPlayerInfo newPersistanceWeaponListByPlayerInfo = new persistanceWeaponListByPlayerInfo ();
		newPersistanceWeaponListByPlayerInfo.playerID = playerManager.getPlayerID ();

		List<persistanceWeaponInfo> newPersistanceWeaponInfoList = new List<persistanceWeaponInfo> ();

		for (int k = 0; k < weaponsList.Count; k++) {
			persistanceWeaponInfo newPersistanceWeaponInfo = new persistanceWeaponInfo ();
			newPersistanceWeaponInfo.Name = weaponsList [k].getWeaponSystemName ();
			newPersistanceWeaponInfo.index = k;
			newPersistanceWeaponInfo.isWeaponEnabled = weaponsList [k].isWeaponEnabled ();
			newPersistanceWeaponInfo.isCurrentWeapon = weaponsList [k].isCurrentWeapon ();
			newPersistanceWeaponInfo.remainingAmmo = weaponsList [k].getWeaponSystemManager ().weaponSettings.remainAmmo + weaponsList [k].getWeaponSystemManager ().getWeaponClipSize ();

			newPersistanceWeaponInfoList.Add (newPersistanceWeaponInfo);
		}

		newPersistanceWeaponListByPlayerInfo.weaponList = newPersistanceWeaponInfoList;

		return newPersistanceWeaponListByPlayerInfo;
	}

	public void saveCurrentWeaponListToFile ()
	{
		if (gameSystemManager.saveNumberToLoad > 0) {
			gameSystemManager.savePlayerWeaponList (getPersistanceWeaponInfoList (), gameSystemManager.saveNumberToLoad);
		} else {
			print ("Configure a valid save number in the Game Manager inspector, between 1 and infinite");
			return;
		}
		print ("Inventory bank list saved");

		updateComponent ();
	}

	public void setWeaponsModeActive (bool state)
	{
		weaponsModeActive = state;

		if (drawKeepWeaponWhenModeChanged) {
			if (canMove && !playerIsBusy () && checkIfWeaponsAvailable () && (startInitialized || !notActivateWeaponsAtStart)) {
				
				checkTypeView ();

				if (weaponsModeActive) {
					checkIfDrawSingleOrDualWeapon ();
				} else {
					checkIfKeepSingleOrDualWeapon ();

					disableFreeFireModeState ();
				}
			}
		}

		checkEventsOnStateChange (weaponsModeActive);

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.checkToEnableOrDisableWeaponSlotsParentOutOfInventory (weaponsModeActive);
		}
	}

	public void checkIfDrawSingleOrDualWeapon ()
	{
		if (usingDualWeaponsPreviously) {
			usingDualWeaponsPreviously = false;

			drawDualWeapons ();
		} else {
			if (currentIKWeapon.isCurrentWeapon ()) {
				drawOrKeepWeaponInput ();
			}
		}
	}

	public void checkIfKeepSingleOrDualWeapon ()
	{
		if (isPlayerCarringWeapon ()) {

			if (usingDualWeapon) {
				keepDualWeapons ();

				usingDualWeaponsPreviously = true;
			} else {
				drawOrKeepWeaponInput ();
			}
		} else {
			if (isUsingWeapons ()) {
				if (usingDualWeapon) {
					disableCurrentDualWeapon ();

					usingDualWeaponsPreviously = true;
				} else {
					disableCurrentWeapon ();
				}
			}
		}
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

	public bool isWeaponsModeActive ()
	{
		return weaponsModeActive;
	}

	public void selectWeaponByName (string weaponName, bool drawSelectedWeapon)
	{
		if (!currentIKWeapon.isWeaponMoving () && !weaponsAreMoving ()) {
			for (int k = 0; k < weaponsList.Count; k++) {
				if (weaponsList [k].getWeaponSystemName () == weaponName && (choosedWeapon != k || drawSelectedWeapon) && weaponsList [k].isWeaponEnabled ()) {

					if (choosedWeapon == k && (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson)) {
						return;
					}

					choosedWeapon = k;

					currentIKWeapon.setCurrentWeaponState (false);

					if (drawSelectedWeapon) {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								changingWeapon = true;
							}
						} else {
							if (isWeaponsModeActive ()) {
								weaponChanged ();
								drawOrKeepWeaponInput ();
							}
						}
					} else {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								changingWeapon = true;
							}
						} else {
							weaponChanged ();
						}
					}
				}
			}
		}
	}

	public void setCurrentRightIkWeaponByName (string weaponName)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			if (weaponsList [k].getWeaponSystemName () == weaponName && weaponsList [k].isWeaponEnabled ()) {

				if (currentRightIKWeapon != null) {
					currentRightIKWeapon.setCurrentWeaponState (false);
				}

				currentRightIKWeapon = weaponsList [k];
				currentRightWeaponSystem = weaponsList [k].getWeaponSystemManager ();
			}
		}
	}

	public void setCurrentLeftIKWeaponByName (string weaponName)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			if (weaponsList [k].getWeaponSystemName () == weaponName && weaponsList [k].isWeaponEnabled ()) {

				if (currentLeftIkWeapon) {
					currentLeftIkWeapon.setCurrentWeaponState (false);
				}

				currentLeftIkWeapon = weaponsList [k];
				currentLeftWeaponSystem = weaponsList [k].getWeaponSystemManager ();
			}
		}
	}

	public void keepDualWeapons ()
	{
		currentRightWeaponSystem.setWeaponCarryStateAtOnce (true);
		currentLeftWeaponSystem.setWeaponCarryStateAtOnce (true);

		drawRightWeapon ();
		drawLeftWeapon ();
	}

	public void drawDualWeapons ()
	{
		currentRightWeaponSystem.setWeaponCarryStateAtOnce (false);
		currentLeftWeaponSystem.setWeaponCarryStateAtOnce (false);

		if (equippingDualWeaponsFromInventoryMenu) {
			drawRightWeapon ();
			drawLeftWeapon ();

			equippingDualWeaponsFromInventoryMenu = false;
		} else {
			
			updateCanMoveValue ();

			if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable) {
				drawRightWeapon ();
				drawLeftWeapon ();
			}
		}
	}

	public void drawDualWeaponsIfNotCarryingWeapons ()
	{
		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !usingDualWeapon) {
			string rightWeaponToUseName = currentIKWeapon.getWeaponSystemName ();
			string leftWeaponToUseName = "";

			if (currentIKWeapon.isWeaponEnabled ()) {
				if (storePickedWeaponsOnInventory) {
					leftWeaponToUseName = playerInventoryManager.getFirstSingleWeaponSlot (rightWeaponToUseName);

					if (leftWeaponToUseName != "") {
						changeDualWeapons (rightWeaponToUseName, leftWeaponToUseName);

						updateWeaponSlotInfo ();
					}
				} else {
					for (int k = 0; k < weaponsList.Count; k++) {
						leftWeaponToUseName = weaponsList [k].getWeaponSystemName ();

						if (!weaponsList [k].isWeaponConfiguredAsDualWepaon () && leftWeaponToUseName != rightWeaponToUseName) {
						
							changeDualWeapons (rightWeaponToUseName, leftWeaponToUseName);

							updateWeaponSlotInfo ();

							return;
						}
					}
				}
			}
		}
	}

	public void removeWeaponConfiguredAsDualWeaponState (int keyNumberToSearch)
	{
		for (int k = 0; k < weaponsList.Count; k++) {
			if (weaponsList [k].isWeaponEnabled ()) {
				if (weaponsList [k].getWeaponSystemKeyNumber () == keyNumberToSearch) {
					weaponsList [k].setWeaponConfiguredAsDualWepaonState (false, "");
				}
			}
		}
	}

	public void changeDualWeapons (string rightWeaponName, string lefWeaponName)
	{
		carryingSingleWeaponPreviously = false;

		if (isUsingWeapons ()) {
			if (!usingDualWeapon) {
				previousSingleIKWeapon = currentIKWeapon;

				carryingSingleWeaponPreviously = true;
				drawOrKeepWeaponInput ();

				usingDualWeapon = true;

			} else {

				if (currentRightIKWeapon && currentRightIKWeapon.getWeaponSystemName () == rightWeaponName) {
					currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
				}

				if (currentLeftIkWeapon && currentLeftIkWeapon.getWeaponSystemName () == lefWeaponName) {
					currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
				}

				carryingDualWeaponsPreviously = true;

				previousRightIKWeapon = currentRightIKWeapon;
				previousLeftIKWeapon = currentLeftIkWeapon;

				keepDualWeapons ();
			}
		} else {
			if (!equippingDualWeaponsFromInventoryMenu) {
				previousSingleIKWeapon = currentIKWeapon;
			}
		}

		currentRighWeaponName = rightWeaponName;
		currentLeftWeaponName = lefWeaponName;

		setCurrentRightIkWeaponByName (rightWeaponName);
		setCurrentLeftIKWeaponByName (lefWeaponName);
	
		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (true, currentLeftIkWeapon.getWeaponSystemName ());
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (true, currentRightIKWeapon.getWeaponSystemName ());

		changingDualWeapon = true;
		keepingWeapon = true;
		changingWeapon = true;
	}

	public void changeSingleWeapon (string singleWeaponName)
	{
		print ("set single weapon " + singleWeaponName);

		carryingDualWeaponsPreviously = false;

		if (isUsingWeapons ()) {
			if (usingDualWeapon) {

				carryingDualWeaponsPreviously = true;

				keepDualWeapons ();
			}
		}

		usingDualWeapon = false;

		disableDualWeaponStateOnWeapons ();

//		setWeaponByName (singleWeaponName);

		changingSingleWeapon = true;
		changingDualWeapon = false;
		keepingWeapon = true;
		changingWeapon = true;
	}

	public void separateDualWeapons ()
	{
		carryingDualWeaponsPreviously = false;

		if (isUsingWeapons ()) {
			if (usingDualWeapon) {

				carryingDualWeaponsPreviously = true;

				keepDualWeapons ();
			}
		}

		usingDualWeapon = false;

		disableDualWeaponStateOnWeapons ();

		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

		changingSingleWeapon = true;
		changingDualWeapon = false;
		keepingWeapon = true;
		changingWeapon = true;

		updateSingleWeaponSlotInfo ();
	}

	public void disableDualWeaponStateOnWeapons ()
	{
		mainWeaponListManager.setSelectingWeaponState (false);

		currentRightIKWeapon.setUsingDualWeaponState (false);
		currentLeftIkWeapon.setUsingDualWeaponState (false);

		currentRightIKWeapon.disableUsingDualWeaponState ();
		currentLeftIkWeapon.disableUsingDualWeaponState ();

		IKManager.setUsingDualWeaponState (false);
	}

	public void updateWeaponSlotInfo ()
	{
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.updateDualWeaponSlotInfo (currentRighWeaponName, currentLeftWeaponName);

			updateCurrentChoosedDualWeaponIndex ();

			playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
		}
	}

	public void updateSingleWeaponSlotInfo ()
	{
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.updateSingleWeaponSlotInfo (currentRighWeaponName, currentLeftWeaponName);

			updateCurrentChoosedDualWeaponIndex ();

			playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
		}
	}

	public void updateCurrentChoosedDualWeaponIndex ()
	{
		if (isUsingWeapons ()) {
			if (usingDualWeapon) {
				chooseDualWeaponIndex = currentRightWeaponSystem.getWeaponNumberKey ();
			} else {
				chooseDualWeaponIndex = currentWeaponSystem.getWeaponNumberKey ();
			}
		}
	}

	public int getNumberOfWeaponsAvailable ()
	{
		int numberOfWeaponsAvailable = 0;
		for (int i = 0; i < weaponsList.Count; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				numberOfWeaponsAvailable++;
			}
		}
		return numberOfWeaponsAvailable;
	}

	public playerWeaponSystem getCurrentWeaponSystem ()
	{
		return currentWeaponSystem;
	}

	public void editWeaponAttachments ()
	{
		if ((!pauseManager.playerMenuActive || editingWeaponAttachments) && !playerManager.usingDevice && !gameSystemManager.isGamePaused () && !usingDevicesManager.hasDeviceToUse ()) {
			if (usingDualWeapon || currentIKWeapon) {

				if (usingDualWeapon) {
					currentRightWeaponAttachmentSystem = currentRightIKWeapon.getWeaponAttachmentSystem ();
					currentLeftWeaponAttachmentSystem = currentLeftIkWeapon.getWeaponAttachmentSystem ();
					 
					if (currentRightWeaponAttachmentSystem || currentLeftWeaponAttachmentSystem) {

						bool canOpenRightWeaponAttachments = false;
						bool canOpenLeftWeaponAttachments = false;
						if (currentRightWeaponAttachmentSystem && currentRightWeaponAttachmentSystem.canBeOpened ()) {
							canOpenRightWeaponAttachments = true;
						}

						if (currentLeftWeaponAttachmentSystem && currentLeftWeaponAttachmentSystem.canBeOpened ()) {
							canOpenLeftWeaponAttachments = true;
						}

						if (canOpenRightWeaponAttachments || canOpenLeftWeaponAttachments) {
							editingWeaponAttachments = !editingWeaponAttachments;

							pauseManager.setIngameMenuOpenedState ("Player Weapons Manager", editingWeaponAttachments);
						}

						if (canOpenRightWeaponAttachments) {
							if (currentLeftWeaponAttachmentSystem) {
								currentRightWeaponAttachmentSystem.setSecondaryWeaponAttachmentSystem (currentLeftWeaponAttachmentSystem);
							}
							currentRightWeaponAttachmentSystem.setUsingDualWeaponState (true);
							currentRightWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}

						if (canOpenLeftWeaponAttachments) {
							currentLeftWeaponAttachmentSystem.setAdjustThirdPersonCameraActiveState (false);
							currentLeftWeaponAttachmentSystem.setUsingDualWeaponState (true);
							currentLeftWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}
					}
				} else {
					currentWeaponAttachmentSystem = currentIKWeapon.getWeaponAttachmentSystem ();

					if (currentWeaponAttachmentSystem) {
						if (currentWeaponAttachmentSystem.canBeOpened ()) {
							editingWeaponAttachments = !editingWeaponAttachments;

							pauseManager.setIngameMenuOpenedState ("Player Weapons Manager", editingWeaponAttachments);

							if (currentLeftWeaponAttachmentSystem) {
								currentLeftWeaponAttachmentSystem.setAdjustThirdPersonCameraActiveState (true);
							}
							
							currentWeaponAttachmentSystem.setUsingDualWeaponState (false);

							currentWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}
					}
				}
			}
		}
	}

	public bool isEditinWeaponAttachments ()
	{
		return editingWeaponAttachments;
	}

	public void checkPressedAttachmentButton (Button pressedButton)
	{
		if (usingDualWeapon) {
			if (currentRightWeaponAttachmentSystem) {
				currentRightWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
			}

			if (currentLeftWeaponAttachmentSystem) {
				currentLeftWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
			}
		} else {
			if (currentIKWeapon) {
				currentWeaponAttachmentSystem = currentIKWeapon.getWeaponAttachmentSystem ();
				if (currentWeaponAttachmentSystem) {
					currentWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
				}
			}
		}
	}

	public void setAttachmentPanelState (bool state)
	{
		if (attachmentPanel) {
			attachmentPanel.SetActive (state);
		}
	}

	public void setRightWeaponAttachmentPanelState (bool state)
	{
		if (rightAttachmentPanel) {
			rightAttachmentPanel.SetActive (state);
		}
	}

	public void setLeftWeaponAttachmentPanelState (bool state)
	{
		if (leftAttachmentPanel) {
			leftAttachmentPanel.SetActive (state);
		}
	}

	public void setAttachmentIcon (Texture attachmentIcon)
	{
		currentAttachmentIcon.texture = attachmentIcon;
	}

	public void setAttachmentPanelAmmoText (string ammoText)
	{
		attachmentAmmoText.text = ammoText;
	}

	//functions to change current weapon stats
	public void setCurrentWeaponSystemWithAttachment (playerWeaponSystem weaponToConfigure)
	{
		currentWeaponSystemWithAttachment = weaponToConfigure;
		//print ("current weapon with attachments: " + currentWeaponSystemWithAttachment.getWeaponSystemName ());
	}

	public void setNumberKey (int newNumberKey)
	{
		currentWeaponSystemWithAttachment.setNumberKey (newNumberKey);
	}

	public void setMagazineSize (int magazineSize)
	{
		currentWeaponSystemWithAttachment.setMagazineSize (magazineSize);
	}

	public void setOriginalMagazineSize ()
	{
		currentWeaponSystemWithAttachment.setOriginalMagazineSize ();
	}

	public void setSilencerState (bool state)
	{
		currentWeaponSystemWithAttachment.setSilencerState (state);
	}

	public void setAutomaticFireMode (bool state)
	{
		currentWeaponSystemWithAttachment.setAutomaticFireMode (state);
	}

	public void setOriginalAutomaticFireMode ()
	{
		currentWeaponSystemWithAttachment.setOriginalAutomaticFireMode ();
	}

	public void setFireRate (float newFireRate)
	{
		currentWeaponSystemWithAttachment.setFireRate (newFireRate);
	}

	public void setOriginalFireRate ()
	{
		currentWeaponSystemWithAttachment.setOriginalFireRate ();
	}

	public void setProjectileDamage (float newDamage)
	{
		currentWeaponSystemWithAttachment.setProjectileDamage (newDamage);
	}

	public void setOriginalProjectileDamage ()
	{
		currentWeaponSystemWithAttachment.setOriginalProjectileDamage ();
	}

	public void setBurstModeState (bool state)
	{
		currentWeaponSystemWithAttachment.setBurstModeState (state);
	}

	public void setOriginalBurstMode ()
	{
		currentWeaponSystemWithAttachment.setOriginalBurstMode ();
	}

	public void setUsingSightState (bool state)
	{
		currentWeaponSystemWithAttachment.setUsingSightState (state);
		usingSight = state;
	}

	public void setProjectileDamageMultiplier (float multiplier)
	{
		currentWeaponSystemWithAttachment.setProjectileDamageMultiplier (multiplier);
	}

	public void setSpreadState (bool state)
	{
		currentWeaponSystemWithAttachment.setSpreadState (state);
	}

	public void setOriginalSpreadState ()
	{
		currentWeaponSystemWithAttachment.setOriginalSpreadState ();
	}

	public void setExplosiveAmmoState (bool state)
	{
		currentWeaponSystemWithAttachment.setExplosiveAmmoState (state);
	}

	public void setOriginalExplosiveAmmoState ()
	{
		currentWeaponSystemWithAttachment.setOriginalExplosiveAmmoState ();
	}

	public void setDamageOverTimeAmmoState (bool state)
	{
		currentWeaponSystemWithAttachment.setDamageOverTimeAmmoState (state);
	}

	public void setOriginalDamageOverTimeAmmoState ()
	{
		currentWeaponSystemWithAttachment.setOriginalDamageOverTimeAmmoState ();
	}

	public void setRemoveDamageOverTimeAmmoState (bool state)
	{
		currentWeaponSystemWithAttachment.setRemoveDamageOverTimeAmmoState (state);
	}

	public void setOriginalRemoveDamageOverTimeAmmoState ()
	{
		currentWeaponSystemWithAttachment.setOriginalRemoveDamageOverTimeAmmoState ();
	}

	public void setSedateAmmoState (bool state)
	{
		currentWeaponSystemWithAttachment.setSedateAmmoState (state);
	}

	public void setOriginalSedateAmmoState ()
	{
		currentWeaponSystemWithAttachment.setOriginalSedateAmmoState ();
	}

	public void setPushCharacterState (bool state)
	{
		currentWeaponSystemWithAttachment.setPushCharacterState (state);
	}

	public void setOriginalPushCharacterState ()
	{
		currentWeaponSystemWithAttachment.setOriginalPushCharacterState ();
	}

	public void setNewWeaponProjectile (GameObject newProjectile)
	{
		currentWeaponSystemWithAttachment.setNewWeaponProjectile (newProjectile);
	}

	public void setOriginalWeaponProjectile ()
	{
		currentWeaponSystemWithAttachment.setOriginalWeaponProjectile ();
	}

	public void setProjectileWithAbilityState (bool newValue)
	{
		currentWeaponSystemWithAttachment.setProjectileWithAbilityState (newValue);
	}

	public void setOriginalProjectileWithAbilityValue ()
	{
		currentWeaponSystemWithAttachment.setOriginalProjectileWithAbilityValue ();
	}

	public bool pickupAttachment (string weaponName, string attachmentName)
	{
		if (useUniversalAttachments) {
			if (usingDualWeapon) {
				return currentRightIKWeapon.pickupAttachment (attachmentName) || currentLeftIkWeapon.pickupAttachment (attachmentName);
			} else {
				if (currentIKWeapon != null) {
					return currentIKWeapon.pickupAttachment (attachmentName);
				} else {
					return false;
				}
			}
		} else {
			for (int k = 0; k < weaponsList.Count; k++) {
				if (weaponsList [k].getWeaponSystemName () == weaponName) {
					return weaponsList [k].pickupAttachment (attachmentName);
				}
			}
		}

		return false;
	}

	public IKSystem getIKSystem ()
	{
		return IKManager;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		usedByAI = !state;
	}

	public void lockOrUnlockCursor (bool state)
	{
		cursorLocked = state;
		pauseManager.showOrHideCursor (!cursorLocked);
		pauseManager.changeCameraState (cursorLocked);
		pauseManager.setHeadBobPausedState (!cursorLocked);
		pauseManager.openOrClosePlayerMenu (!cursorLocked, null, false);
		pauseManager.usingDeviceState (!cursorLocked);

		setLastTimeFired ();

		setLastTimeMoved ();

		enableOrDisableGeneralWeaponCursor (cursorLocked);

		grabObjectsManager.enableOrDisableGeneralCursor (cursorLocked);

		pauseManager.enableOrDisableDynamicElementsOnScreen (cursorLocked);
	}

	public void lockCursorAgain ()
	{
		if (!cursorLocked) {
			lockOrUnlockCursor (true);
		}
	}

	public bool isCursorLocked ()
	{
		return cursorLocked;
	}

	public void setPlayerControllerMovementValues (bool useNewCarrySpeed, float newCarrySpeed)
	{
		if (useNewCarrySpeed) {
			if (isFirstPersonActive ()) {
				playerManager.setNoAnimatorGeneralMovementSpeed (newCarrySpeed, false);
			} else {
				playerManager.setAnimatorGeneralMovementSpeed (newCarrySpeed, false);
			}
		}
	}

	public void setPlayerControllerCanRunValue (bool canRun)
	{
		if (isFirstPersonActive ()) {
			playerManager.setNoAnimatorCanRunState (canRun, false);
		} else {
			playerManager.setAnimatorCanRunState (canRun, false);
		}
	}

	public void setPlayerControllerMovementOriginalValues ()
	{
		if (isFirstPersonActive ()) {
			playerManager.setNoAnimatorGeneralMovementSpeed (0, true);
			playerManager.setNoAnimatorCanRunState (false, true);
		} else {
			playerManager.setAnimatorGeneralMovementSpeed (0, true);
			playerManager.setAnimatorCanRunState (false, true);
		}
	}

	public bool canUseCarriedWeaponsInput ()
	{
		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && (currentIKWeapon.isCurrentWeapon () || usingDualWeapon)) {
			if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
				return true;
			}
		}

		return false;
	}

	public bool canUseWeaponsInput ()
	{
		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && (currentIKWeapon.isCurrentWeapon () || usingDualWeapon)) {
			return true;
		}

		return false;
	}

	public void quickDrawWeaponThirdPersonAction ()
	{
		if (playerManager.isCrouching ()) {
			playerManager.crouch ();
		}

		if (playerManager.isCrouching ()) {
			return;
		}

		playerCameraManager.setOriginalRotationSpeed ();

		carryingWeaponInThirdPerson = true;

		getCurrentWeapon ();

		enableOrDisableWeaponsHUD (true);
		updateWeaponHUDInfo ();
		updateAmmo ();

		if (usingDualWeapon) {
			IKManager.setUsingDualWeaponState (true);
		}

		currentIKWeapon.quickDrawWeaponThirdPersonAction ();

		currentWeaponSystem.setPauseDrawKeepWeaponSound ();

		currentWeaponSystem.setWeaponCarryState (true, false);

		IKManager.quickDrawWeaponState (currentIKWeapon.thirdPersonWeaponInfo);

		enableOrDisableWeaponCursor (false);

		if (!usingDualWeapon) {
			if (currentIKWeapon) {
				currentIKWeapon.checkHandsPosition ();
			}
		}
	}

	public void quickKeepWeaponThirdPersonAction ()
	{
		carryingWeaponInThirdPerson = false;
		enableOrDisableWeaponsHUD (false);

		currentWeaponSystem.enableHUD (false);

		IKManager.setIKWeaponState (false, currentIKWeapon.thirdPersonWeaponInfo, false);

		currentIKWeapon.quickKeepWeaponThirdPersonAction ();

		IKManager.stopIKWeaponsActions ();

		currentWeaponSystem.setPauseDrawKeepWeaponSound ();
	
		currentWeaponSystem.setWeaponCarryState (false, false);

		enableOrDisableWeaponCursor (false);

		playerCameraManager.setOriginalRotationSpeed ();

		if (aimingInThirdPerson) {
			activateOrDeactivateAimMode (false);
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			enableOrDisableGrabObjectsManager (aimingInThirdPerson);

			currentWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);

			if (currentIKWeapon.useLowerRotationSpeedAimedThirdPerson) {
				playerCameraManager.setOriginalRotationSpeed ();
			}
		}
			
		setAimAssistInThirdPersonState ();
	}

	public void quicChangeWeaponThirdPersonAction ()
	{
		drawOrKeepWeapon (false);

		weaponChanged ();

		drawOrKeepWeapon (true);
		keepingWeapon = false;
		changingWeapon = false;
	}

	public void enableFreeFireModeState ()
	{
		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerManager.setUsingFreeFireModeState (true);

		usingFreeFireMode = true;
	}

	public void disableFreeFireModeState ()
	{
		aimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerManager.setUsingFreeFireModeState (false);

		usingFreeFireMode = false;

		if (headTrackManager) {
			headTrackManager.setOriginalCameraBodyWeightValue ();
		}

		checkSetExtraRotationCoroutine (false);
		upperBodyRotationManager.enableOrDisableIKUpperBody (false);
		upperBodyRotationManager.setCurrentBodyRotation (extraRotation);
	}

	public playerScreenObjectivesSystem getPlayerScreenObjectivesManager ()
	{
		return playerScreenObjectivesManager;
	}

	public void updateCanMoveValue ()
	{
		canMove = !playerIsDead && playerManager.canPlayerMove ();
	}

	public bool isCheckToKeepWeaponAfterAimingWeaponFromShooting ()
	{
		return checkToKeepWeaponAfterAimingWeaponFromShooting;
	}

	public void setCarryWeaponInLowerPositionActiveState (bool state)
	{
		carryWeaponInLowerPositionActive = state;
	}

	public bool isCarryWeaponInLowerPositionActive ()
	{
		return carryWeaponInLowerPositionActive;
	}

	public bool currentWeaponUsesHeadLookWhenAiming ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon) {
				return currentRightIKWeapon.headLookWhenAiming;
			} else {
				return false;
			}
		} else {
			if (currentIKWeapon) {
				return currentIKWeapon.headLookWhenAiming;
			} else {
				return false;
			}
		}
	}

	public float getCurrentWeaponHeadLookSpeed ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon) {
				return currentRightIKWeapon.headLookSpeed;
			} else {
				return 0;
			}
		} else {
			if (currentIKWeapon) {
				return currentIKWeapon.headLookSpeed;
			} else {
				return 0;
			}
		}
	}

	public Vector3 getCurrentHeadLookTargetPosition ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon) {
				return currentRightIKWeapon.currentHeadLookTarget.position;
			} else {
				return Vector3.zero;
			}
		} else {
			if (currentIKWeapon) {
				return currentIKWeapon.currentHeadLookTarget.position;
			} else {
				return Vector3.zero;
			}
		}
	}

	public bool playerCanAIm ()
	{
		if (currentIKWeapon && currentIKWeapon.isReloadingWeapon ()) {
			return false;
		}

		if (canAimOnAirThirdPerson || isFirstPersonActive () || (!canAimOnAirThirdPerson && !isFirstPersonActive () && playerManager.isPlayerOnGround ()) || aimingInThirdPerson) {
			return true;
		}

		return false;
	}

	public void disableAimModeInputPressedState ()
	{
		aimModeInputPressed = false;
	}

	public void setCurrentWeaponAsOneHandWield ()
	{
		currentIKWeapon.setUsingDualWeaponState (true);
		
		currentIKWeapon.setCurrentWeaponAsOneHandWield ();

		getCurrentWeaponRotation (currentIKWeapon);
	}

	public void setCurrentWeaponAsTwoHandsWield ()
	{
		currentIKWeapon.setUsingDualWeaponState (false);

		currentIKWeapon.setCurrentWeaponAsTwoHandsWield ();

		getCurrentWeaponRotation (currentIKWeapon);
	}

	public void inputSetCurrentWeaponAsOneOrTwoHandsWield ()
	{ 
		if (canUseCarriedWeaponsInput () && !usingDualWeapon && !isFirstPersonActive () && !currentIKWeapon.isReloadingWeapon () && !currentIKWeapon.isWeaponMoving ()) {
			if (isAimingWeapons ()) {
				return;
			}

			print ("change hand");
			bool usingWeaponAsOneHandWield = currentIKWeapon.getUsingWeaponAsOneHandWieldState ();

			usingWeaponAsOneHandWield = !usingWeaponAsOneHandWield;

			if (usingWeaponAsOneHandWield) {
				setCurrentWeaponAsOneHandWield ();
			} else {
				setCurrentWeaponAsTwoHandsWield ();
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputAimWeapon ()
	{
		if (!playerCanAIm ()) {
			return;
		}

		if (canUseWeaponsInput ()) {
			aimModeInputPressed = !aimModeInputPressed;

			if (isUsingWeapons ()) {
				if (aimingInThirdPerson && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode)) {
					aimCurrentWeapon (true);
				} else {
					aimCurrentWeaponInput ();
				}
			} else {
				if (drawAndAimWeaponIfFireButtonPressed && isThirdPersonView) {
					quickDrawWeaponThirdPersonAction ();
					aimCurrentWeaponInput ();
				}
			}
		}
	}

	public void inputStartOrStopAimWeapon (bool aimingWeapon)
	{
		if (usingDualWeapon) {
			return;
		}

		if (!playerCanAIm ()) {
			return;
		}

		if (canUseWeaponsInput ()) {
			aimModeInputPressed = aimingWeapon;

			if (isUsingWeapons ()) {
				if (usingFreeFireMode) {
					aimCurrentWeapon (true);
				} else {
					if (aimingWeapon) {
						aimCurrentWeapon (true);
					} else {
						aimCurrentWeapon (false);
					}
				}
			} else {
				if (drawAndAimWeaponIfFireButtonPressed && isThirdPersonView) {
					quickDrawWeaponThirdPersonAction ();
					aimCurrentWeaponInput ();
				}
			}
		}
	}

	public void inputPressDownOrReleaseDropWeapon (bool holdingButton)
	{
		if (canUseCarriedWeaponsInput ()) {
			if (holdingButton) {
				holdingDropButtonToIncreaseForce = true;
				lastTimeHoldDropButton = Time.time;
				if (holdDropButtonToIncreaseForce) {
					currentDropForce = 0;
				} else {
					dropWeapon ();
				}
			} else {
				if (holdDropButtonToIncreaseForce) {
					if (holdingDropButtonToIncreaseForce && Time.time < lastTimeHoldDropButton + 0.2f) {
						holdingDropButtonToIncreaseForce = false;
					}

					dropWeapon ();
				}
				holdingDropButtonToIncreaseForce = false;
			}
		}
	}

	public void inputHoldDropWeapon ()
	{
		if (canUseCarriedWeaponsInput ()) {
			if (holdDropButtonToIncreaseForce) {
				currentDropForce += dropIncreaseForceSpeed * Time.deltaTime;

				currentDropForce = Mathf.Clamp (currentDropForce, 0, maxDropForce);		
			}
		}
	}

	public void inputActivateSecondaryAction ()
	{
		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryAction ();
		}
	}

	public void inputActivateSecondaryActionOnPressDown ()
	{
		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnDownPress ();
		}
	}

	public void inputActivateSecondaryActionOnPressUp ()
	{
		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnUpPress ();
		}
	}

	public void inputActivateSecondaryActionOnPressHold ()
	{
		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnUpHold ();
		}
	}

	public void inputWeaponMeleeAttack ()
	{
		if (canUseCarriedWeaponsInput ()) {
			useMeleeAttack ();
		}
	}

	public void inputLockOrUnLockCursor ()
	{
		if (canMove && weaponsModeActive && anyWeaponAvailable && (currentIKWeapon.isCurrentWeapon () || usingDualWeapon) && carryingWeaponInFirstPerson &&
		    ((!usingDualWeapon && currentIKWeapon.canUnlockCursor) || (usingDualWeapon && (currentRightIKWeapon.canUnlockCursor || currentLeftIkWeapon.canUnlockCursor))) &&
		    !playerManager.usingDevice && !pauseManager.usingSubMenu && !editingWeaponAttachments) {

			lockOrUnlockCursor (!cursorLocked);
		}
	}

	float lastTimeDrawWeapon;

	public void inputHoldOrReleaseShootWeapon (bool holdingButton)
	{
		//used on press down and press up button actions
		if (usingDualWeapon) {
			return;
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (canUseWeaponsInput ()) {
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if ((aimingInThirdPerson || canFireWeaponsWithoutAiming) || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is holding the fire button
						if (holdingButton) {
							//if the player is in third person
							if (isThirdPersonView) {
								//if the player is carrying a weapon
								if (carryingWeaponInThirdPerson) {
									//if the player is not aiming and the option to fire weapons without aim is active
									if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
										if (playerManager.isPlayerMovingOn3dWorld ()) {
											//set the aim state activate from firing to true and activate the aim mode

											enableFreeFireModeState ();

											aimingWeaponFromShooting = true;

											aimModeInputPressed = false;

											if (useQuickDrawWeapon) {
												if (Time.time > lastTimeDrawWeapon + 0.2f) {
													aimCurrentWeaponInput ();
												}
											} else {
												aimCurrentWeaponInput ();
											}
										} else {
											checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
											aimCurrentWeaponInput ();
										}
									}
								}
							}

							//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
							if (!isUsingWeapons ()) {
								if (drawWeaponIfFireButtonPressed) {
									drawOrKeepWeaponInput ();

									//avoid to keep checking the function, so the weapon is not fired before draw it
									return;
								}
							}
								
							if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentWeaponWithHandsInPosition ())) {
								//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
								if (currentWeaponSystem.weaponSettings.automatic) {
									if (currentWeaponSystem.weaponSettings.useBurst) {
										shootWeapon (true);
									}
								} else {
									shootWeapon (true);
								}
							}
							
						} else {
							//if the fire button is released, stop shoot the weapon
							shootWeapon (false);

							if (playerManager.isPlayerMovingOn3dWorld ()) {
								//if the player is aiming in third person and the option to fire weapons without need to aim is active and the player activated the aiming weapon from shooting
								if (aimingInThirdPerson && canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									//deactivate the aim mode
									checkToKeepWeaponAfterAimingWeaponFromShooting = true;
									playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (true);
								} else if (canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									if (Time.time < lastTimeDrawWeapon + 0.2f) {
										disableFreeFireModeState ();
									}
								}
							}
								
							aimingWeaponFromShooting = false;

							playerManager.setUsingFreeFireModeState (false);

							usingFreeFireMode = false;
						}
					}
				}
			}
		}
	}

	public void inputHoldShootWeapon ()
	{
		//used on press button action
		if (usingDualWeapon) {
			return;
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (canUseWeaponsInput ()) {
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if (aimingInThirdPerson || canFireWeaponsWithoutAiming || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is in third person
						if (isThirdPersonView) {
							//if the player is carrying a weapon
							if (carryingWeaponInThirdPerson) {
								//if the player is not aiming and the option to fire weapons without aim is active
								if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
									if (playerManager.isPlayerMovingOn3dWorld ()) {
										//set the aim state activate from firing to true and activate the aim mode

										enableFreeFireModeState ();

										aimingWeaponFromShooting = true;

										aimModeInputPressed = false;

										if (useQuickDrawWeapon) {
											if (Time.time > lastTimeDrawWeapon + 0.2f) {
												aimCurrentWeaponInput ();
											}
										} else {
											aimCurrentWeaponInput ();
										}
									} else {
										checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
										aimCurrentWeaponInput ();
									}
								}
							}
						} 
							
						//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
						if (!isUsingWeapons ()) {
							if (drawWeaponIfFireButtonPressed) {

								drawOrKeepWeaponInput ();

								//avoid to keep checking the function, so the weapon is not fired before draw it
								return;
							}
						}

						if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentWeaponWithHandsInPosition ())) {
							//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
							if (currentWeaponSystem.weaponSettings.automatic) {
								if (!currentWeaponSystem.weaponSettings.useBurst) {
									shootWeapon (true);
								}
							} else {
								currentWeaponSystem.checkWeaponAbilityHoldButton ();
							}
						}
					}
				}
			}
		}
	}

	public void inputDrawWeapon ()
	{
		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (!usingDualWeapon && !currentIKWeapon.isWeaponMoving ()) {
				if (currentIKWeapon.isWeaponConfiguredAsDualWepaon ()) {
					
					chooseDualWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

					print ("weapon to draw " + currentIKWeapon.getWeaponSystemName () + "configured as dual weapon with " + currentIKWeapon.getLinkedDualWeaponName ());
					if (currentIKWeapon.usingRightHandDualWeapon) {
						changeDualWeapons (currentIKWeapon.getWeaponSystemName (), currentIKWeapon.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentIKWeapon.getLinkedDualWeaponName (), currentIKWeapon.getWeaponSystemName ());
					}
				} else {
					drawOrKeepWeaponInput ();
				}

				disableFreeFireModeState ();
			}
		}
	}

	public void inputReloadWeapon ()
	{
		if (canUseCarriedWeaponsInput ()) {		
			if (usingDualWeapon) {
				currentRightWeaponSystem.manualReload ();
				currentLeftWeaponSystem.manualReload ();
			} else {
				currentWeaponSystem.manualReload ();
			}
		}
	}

	public void inputNextOrPreviousWeaponByButton (bool setNextWeapon)
	{
		if (!changeWeaponsWithKeysActive) {
			return;
		}

		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (setNextWeapon) {
				chooseNextWeapon (false, true);
			} else {
				choosePreviousWeapon (false, true);
			}
		}
	}

	public void inputNextOrPreviousWeapnByMouse (bool setNextWeapon)
	{
		if (!changeWeaponsWithMouseWheelActive) {
			return;
		}

		//select the power using the mouse wheel or the change power buttons
		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (setNextWeapon) {
				chooseNextWeapon (false, true);
			} else {
				choosePreviousWeapon (false, true);
			}
		}
	}

	public void inputEditWeaponAttachments ()
	{
		if (!openWeaponAttachmentsMenuEnabled) {
			return;
		}
			
		if ((canMove || editingWeaponAttachments) && weaponsModeActive && anyWeaponAvailable && isPlayerCarringWeapon () && !isAimingWeapons ()) {
			if (!weaponsAreMoving () && !currentIKWeapon.isWeaponMoving ()) {
				if (currentIKWeapon.isCurrentWeapon () || usingDualWeapon) {
					editWeaponAttachments ();
				}
			}
		}
	}

	public void inputDrawRightWeapon ()
	{
		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && usingDualWeapon && !currentWeaponIsMoving () && !changingDualWeapon) {
			keepDualWeaponAndDrawSingle (true);
		}
	}

	public void inputDrawLeftWeapon ()
	{
		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && usingDualWeapon && !currentWeaponIsMoving () && !changingDualWeapon) {
			keepDualWeaponAndDrawSingle (false);
		}
	}

	public void drawRightWeapon ()
	{
		if (currentRightIKWeapon == null || currentLeftIkWeapon == null) {
			return;
		}

		drawCurrentDualWeaponSelected (true);
	}

	public void drawLeftWeapon ()
	{
		if (currentLeftIkWeapon == null || currentRightIKWeapon == null) {
			return;
		}

		drawCurrentDualWeaponSelected (false);
	}

	public void drawCurrentDualWeaponSelected (bool isRightWeapon)
	{
		if (previousSingleIKWeapon && previousSingleIKWeapon.isCurrentWeapon ()) {
			if (previousSingleIKWeapon != currentRightIKWeapon && previousSingleIKWeapon != currentLeftIkWeapon) {
				previousSingleIKWeapon.setCurrentWeaponState (false);

				previousSingleIKWeapon = null;
			}
		}

		if (isRightWeapon) {
			currentIKWeapon = currentRightIKWeapon;
			currentWeaponSystem = currentRightWeaponSystem;
		} else {
			currentIKWeapon = currentLeftIkWeapon;
			currentWeaponSystem = currentLeftWeaponSystem;
		}

		currentIKWeapon.setCurrentWeaponState (true);

		print ("drawing right weapon " + isRightWeapon + " " + currentIKWeapon.getWeaponSystemName ());

		if (currentIKWeapon.isCurrentWeapon ()) {
			if (!currentIKWeapon.isWeaponMoving ()) {

				currentIKWeapon.thirdPersonWeaponInfo.usedOnRightHand = isRightWeapon;
				currentIKWeapon.thirdPersonWeaponInfo.dualWeaponActive = true;

				currentIKWeapon.firstPersonWeaponInfo.usedOnRightHand = isRightWeapon;
				currentIKWeapon.firstPersonWeaponInfo.dualWeaponActive = true;

				if (!currentWeaponSystem.carryingWeapon ()) {
					usingDualWeapon = true;

					mainWeaponListManager.setSelectingWeaponState (true);

					getCurrentWeaponRotation (currentRightIKWeapon);
				}

				if (isRightWeapon) {
					IKManager.setIKWeaponsRightHandSettings (currentIKWeapon.thirdPersonWeaponInfo);
				} else {
					IKManager.setIKWeaponsLeftHandSettings (currentIKWeapon.thirdPersonWeaponInfo);
				}

				if (isThirdPersonView) {
					IKManager.setUsingDualWeaponState (true);
				}

				currentIKWeapon.setUsingRightHandDualWeaponState (isRightWeapon);

				currentIKWeapon.setUsingDualWeaponState (true);

				drawOrKeepWeapon (!currentWeaponSystem.carryingWeapon ());

				if (currentRightWeaponSystem.carryingWeapon () || currentLeftWeaponSystem.carryingWeapon ()) {

					if (isThirdPersonView) {
						carryingWeaponInThirdPerson = true;
					} else {
						carryingWeaponInFirstPerson = true;
					}

					enableOrDisableWeaponsHUD (true);
				} else {
					usingDualWeapon = false;

					disableDualWeaponStateOnWeapons ();

					if (settingSingleWeaponFromNumberKeys) {

						currentRightIKWeapon.setCurrentWeaponState (false);
						currentLeftIkWeapon.setCurrentWeaponState (false);

						setWeaponByName (singleWeaponNameToChangeFromNumberkeys);
						settingSingleWeaponFromNumberKeys = false;
					} else {
						if (currentRightIKWeapon && currentRightIKWeapon.isWeaponEnabled ()) {
							setWeaponByName (currentRighWeaponName);
						} else if (currentLeftIkWeapon && currentLeftIkWeapon.isWeaponEnabled ()) {
							setWeaponByName (currentLeftWeaponName);
						}
					}

					weaponChanged ();
				}

				disableFreeFireModeState ();
			}
		}
	}

	public void inputKeepDualWeapons ()
	{
		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !currentWeaponIsMoving ()) {
			if (usingDualWeapon) {
				keepDualWeaponAndDrawSingle (true);
			}
		}
	}

	public void keepDualWeaponAndDrawSingle (bool drawRightWeaponActive)
	{
		settingSingleWeaponFromNumberKeys = true;

		if (drawRightWeaponActive) {
			chooseDualWeaponIndex = currentRightIKWeapon.getWeaponSystemKeyNumber ();

			singleWeaponNameToChangeFromNumberkeys = currentRightIKWeapon.getWeaponSystemName ();
		} else {
			chooseDualWeaponIndex = currentLeftIkWeapon.getWeaponSystemKeyNumber ();

			singleWeaponNameToChangeFromNumberkeys = currentLeftIkWeapon.getWeaponSystemName ();
		}

		separateDualWeapons ();
	}

	public void setSelectingWeaponState (bool state)
	{
		usingDualWeapon = state;
	}

	public void inputHoldOrReleaseShootRightWeapon (bool holdingButton)
	{
		//used on press down and press up button actions
		holdOrReleaseShootRightOrLeftDualWeapon (holdingButton, true);
	}

	public void inputHoldShootRightWeapon ()
	{
		//used on press button action
		holdShootRightOrLeftDualWeapon (true);
	}

	public void inputHoldOrReleaseShootLeftWeapon (bool holdingButton)
	{
		//used on press down and press up button actions
		holdOrReleaseShootRightOrLeftDualWeapon (holdingButton, false);
	}

	public void inputHoldShootLeftWeapon ()
	{
		//used on press button action
		holdShootRightOrLeftDualWeapon (false);
	}

	public void holdOrReleaseShootRightOrLeftDualWeapon (bool holdingButton, bool isRightWeapon)
	{
		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (usingDualWeapon && canUseWeaponsInput ()) {
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if ((aimingInThirdPerson || canFireWeaponsWithoutAiming) || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is holding the fire button
						if (holdingButton) {
							//if the player is in third person
							if (isThirdPersonView) {
								//if the player is carrying a weapon
								if (carryingWeaponInThirdPerson) {
									//if the player is not aiming and the option to fire weapons without aim is active
									if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
										if (playerManager.isPlayerMovingOn3dWorld ()) {
											//set the aim state activate from firing to true and activate the aim mode

											enableFreeFireModeState ();

											aimingWeaponFromShooting = true;

											aimModeInputPressed = false;

											if (useQuickDrawWeapon) {
												if (Time.time > lastTimeDrawWeapon + 0.2f) {
													aimCurrentWeaponInput ();
												}
											} else {
												aimCurrentWeaponInput ();
											}
										} else {
											checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
											aimCurrentWeaponInput ();
										}
									}
								}
							}

							//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
							if (!isUsingWeapons ()) {
								if (drawWeaponIfFireButtonPressed) {
									drawOrKeepWeaponInput ();

									//avoid to keep checking the function, so the weapon is not fired before draw it
									return;
								}
							}

							if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentDualWeaponWithHandsInPosition (isRightWeapon))) {
								//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
								if (isRightWeapon) {
									if (currentRightWeaponSystem.weaponSettings.automatic) {
										if (currentRightWeaponSystem.weaponSettings.useBurst) {
											shootDualWeapon (true, isRightWeapon, false);
										}
									} else {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									if (currentLeftWeaponSystem.weaponSettings.automatic) {
										if (currentLeftWeaponSystem.weaponSettings.useBurst) {
											shootDualWeapon (true, isRightWeapon, false);
										}
									} else {
										shootDualWeapon (true, isRightWeapon, false);
									}
								}
							}

						} else {
							//if the fire button is released, stop shoot the weapon
							shootDualWeapon (false, isRightWeapon, false);

							if (playerManager.isPlayerMovingOn3dWorld ()) {
								//if the player is aiming in third person and the option to fire weapons without need to aim is active and the player activated the aiming weapon from shooting
								if (aimingInThirdPerson && canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									//deactivate the aim mode
									checkToKeepWeaponAfterAimingWeaponFromShooting = true;
								} else if (canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									if (Time.time < lastTimeDrawWeapon + 0.2f) {
										disableFreeFireModeState ();
									}
								}
							}

							aimingWeaponFromShooting = false;

							usingFreeFireMode = false;
						}
					}
				}
			}
		}
	}

	public void holdShootRightOrLeftDualWeapon (bool isRightWeapon)
	{
		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (usingDualWeapon && canUseWeaponsInput ()) {
	
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
	
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if (aimingInThirdPerson || canFireWeaponsWithoutAiming || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is in third person
						if (isThirdPersonView) {
							//if the player is carrying a weapon
							if (carryingWeaponInThirdPerson) {
								//if the player is not aiming and the option to fire weapons without aim is active
								if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
									if (playerManager.isPlayerMovingOn3dWorld ()) {
										//set the aim state activate from firing to true and activate the aim mode
										aimingWeaponFromShooting = true;

										enableFreeFireModeState ();

										aimModeInputPressed = false;

										if (useQuickDrawWeapon) {
											if (Time.time > lastTimeDrawWeapon + 0.2f) {
												aimCurrentWeaponInput ();
											}
										} else {
											aimCurrentWeaponInput ();
										}
									} else {
										checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
										aimCurrentWeaponInput ();
									}
								}
							}
						} 
							
						//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
						if (!isUsingWeapons ()) {
							if (drawWeaponIfFireButtonPressed) {

								drawOrKeepWeaponInput ();

								//avoid to keep checking the function, so the weapon is not fired before draw it
								return;
							}
						}
							
						if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentDualWeaponWithHandsInPosition (isRightWeapon))) {
							//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
							if (isRightWeapon) {
								if (currentRightWeaponSystem.weaponSettings.automatic) {
									if (!currentRightWeaponSystem.weaponSettings.useBurst) {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									currentRightWeaponSystem.checkWeaponAbilityHoldButton ();
								}
							} else {
								if (currentLeftWeaponSystem.weaponSettings.automatic) {
									if (!currentLeftWeaponSystem.weaponSettings.useBurst) {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									if (playerCurrentlyBusy) {
										currentLeftWeaponSystem.checkWeaponAbilityHoldButton ();
									}
								}
							}
						}
					}
				}
			}
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
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
		if (showGizmo) {
			//set the change weapon touch zone in the right upper corner of the scren, visile as gizmo
			if (!EditorApplication.isPlaying) {
				setHudZone ();
			}
			if (swipeCenterPosition) {
				Gizmos.color = gizmoColor;
				Vector3 touchZone = new Vector3 (touchZoneRect.x + touchZoneRect.width / 2f, touchZoneRect.y + touchZoneRect.height / 2f, swipeCenterPosition.transform.position.z);
				Gizmos.DrawWireCube (touchZone, new Vector3 (touchZoneSize.x, touchZoneSize.y, 0f));
			}

		}
	}
	#endif

	//get the correct size of the rect
	void setHudZone ()
	{
		if (swipeCenterPosition) {
			touchZoneRect = new Rect (swipeCenterPosition.transform.position.x - touchZoneSize.x / 2f, swipeCenterPosition.transform.position.y - touchZoneSize.y / 2f, touchZoneSize.x, touchZoneSize.y);
		}
	}

	[System.Serializable]
	public class weaponPocket
	{
		public string Name;
		public Transform pocketTransform;
		public List<weaponListOnPocket> weaponOnPocketList = new List<weaponListOnPocket> ();
	}

	[System.Serializable]
	public class weaponListOnPocket
	{
		public string Name;
		public List<GameObject> weaponList = new List<GameObject> ();
	}
}