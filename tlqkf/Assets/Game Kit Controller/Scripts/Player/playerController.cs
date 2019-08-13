using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerController : MonoBehaviour
{
	//Main variables
	[Range (0, 1)]public float walkSpeed = 1;
	public float stationaryTurnSpeed = 180;
	public float movingTurnSpeed = 200;
	public float autoTurnSpeed = 2;
	public float aimTurnSpeed = 10;

	public bool canUseSphereMode;
	public bool canGetOnVehicles;
	public bool canDrive;
	float originalWalkSpeedValue;

	public bool increaseWalkSpeedEnabled;
	public float increaseWalkSpeedValue = 1;
	public bool holdButtonToKeepIncreasedWalkSpeed;
	bool increaseWalkSpeedActive;

	public bool canRunThirdPersonActive = true;
	public bool sprintEnabled = true;
	public bool changeCameraFovOnSprint = true;
	public bool shakeCameraOnSprintThirdPerson = true;
	public string sprintThirdPersonCameraShakeName = "Sprint Shake";

	public bool useSecondarySprintValues = true;
	public float sprintVelocity = 1;
	public float sprintJumpPower = 15;
	public float sprintAirSpeed = 20;
	public float sprintAirControl = 4;

	//Input variables
	public float thresholdAngleDifference = 5;
	public Vector3 moveInput;
	public float horizontalInput;
	public float verticalInput;
	Vector2 axisValues;
	Vector2 rawAxisValues;
	Vector3 secondaryMoveInput;
	Vector3 secondaryMove;
	Vector3 stairsMoveInput;
	Vector3 airMove;

	Vector3 currentLockedInput;
	Vector3 lockedMove;

	Vector3 currentAdherenceInput;

	public float inputLerpSpeed = 0.4f;

	bool overrideOnGroundAnimatorValueActive;
	float overrideOnGrounDuration;
	float lastTimeOverrideOnGroundAnimator;

	public float defaultStrafeWalkSpeed = 1;
	public float defaultStrafeRunSpeed = 2;
	public bool rotateDirectlyTowardCameraOnStrafe;
	public float strafeLerpSpeed = 0.1f;

	//Fall damage variables
	public bool damageFallEnabled;

	public bool callEventOnFallDamage;
	public UnityEvent eventOnFallDamage;
	public float minDamageToCallFunction;

	public float fallindDamageMultiplier;
	public float lastTimeFalling;

	//Stairs variables
	public bool checkForStairAdherenceSystem;
	public float stairsMaxValue = 0.25f;
	public float stairsMinValue = 0.2f;

	public float currentStairAdherenceSystemMaxValue;
	public float currentStairAdherenceSystemMinValue;
	public float currentStairAdherenceSystemAdherenceValue;

	float currentStairMaxValue;
	float currentStairMinValue;
	float currentStairAdherenceValue;

	public bool stairAdherenceSystemDetected;
	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;

	public bool checkStairsWithInclination;
	public float minStairInclination = -10;
	public float maxStairInclination = 10;

	bool stairInclinationDetected;

	//Animator variables
	public int baseLayerIndex = 0;
	public float moveSpeedMultiplier = 1;
	public float animSpeedMultiplier = 1;
	public bool usingAnimator;
	public bool usingAnimatorInFirstMode;
	float originalAnimationSpeed;

	public bool lookAlwaysInCameraDirection;
	public bool lookInCameraDirectionIfLookingAtTarget;
	public bool lookOnlyIfMoving;
	public bool lookInCameraDirectionActive;

	public bool lookInCameraDirectionOnFreeFire = true;
	public bool usingFreeFireMode;
	public bool lookInCameraDirectionOnFreeFireActive;

	public bool useRootMotionActive = true;
	public float noRootLerpSpeed;
	public float noRootWalkMovementSpeed;
	public float noRootRunMovementSpeed;
	public float noRootSprintMovementSpeed;
	public float noRootCrouchMovementSpeed;
	public float noRootWalkStrafeMovementSpeed;
	public float noRootRunStrafeMovementSpeed;
	public float currentNoRootMovementSpeed;
	public float lerpNoRootMovementSpeed;

	public bool originalSetUseRootMotionActiveState;

	public Vector3 characterVelocity;

	float lastTimeInput;
	float lastTimePlayerUserInput;

	bool tankModeActive;
	bool scrollModeActive;
	float currentMovementID;
	float currentPlayerModeID;
	float currentMovementSpeed;
	float currentHorizontalValue;
	float currentVerticalValue;

	//States variables
	public bool playerIsAiming;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool slowingFall;
	public bool canMove = true;
	public bool sphereModeActive;
	public bool isMoving;
	public bool gravityPowerActive;
	public bool usingDevice;
	public bool headBobPaused;
	public bool isDead;
	public bool gamePaused;

	//Physics variables
	public PhysicMaterial zeroFrictionMaterial;
	public PhysicMaterial highFrictionMaterial;
	public LayerMask layer;
	public float rayDistance;
	public Vector3 currentVelocity;
	RaycastHit hit;
	bool physicMaterialAssigmentPaused;

	public float noAnimatorSpeed;
	public float noAnimatorWalkMovementSpeed;
	public float noAnimatorRunMovementSpeed;
	public float noAnimatorCrouchMovementSpeed;
	public float noAnimatorRunCrouchMovementSpeed;
	public float noAnimatorStrafeMovementSpeed;
	public float noAnimatorWalkBackwardMovementSpeed;
	public float noAnimatorRunBackwardMovementSpeed;
	public float noAnimatorCrouchBackwardMovementSpeed;
	public float noAnimatorRunCrouchBackwardMovementSpeed;
	public float noAnimatorStrafeBackwardMovementSpeed;
	public bool noAnimatorCanRun;
	public bool noAnimatorCanRunBackwards;
	public float noAnimatorAirSpeed = 11;

	public float noAnimatorCurrentMovementSpeed;

	Vector3 noAnimatorCurrentForce;
	Vector3 velocityChange;
	public float currentVelocityChangeMagnitude;
	public float maxVelocityChange;
	float originalNoAnimWalkMovementSpeed;
	float originalNoAnimRunMovementSpeed;
	float originalNoAnimCrouchMovementSpeed;
	float originalNoAnimStrafeMovementSpeed;
	float originalNoAnimWalkBackwardMovementSpeed;
	float originalNoAnimRunBackwardMovementSpeed;
	float originalNoAnimCrouchBackwardMovementSpeed;
	float originalNoAnimStrafeBackwardMovementSpeed;
	bool originalNoAnimCanRun;

	//Ground variables
	public bool playerOnGround;
	public float regularGroundAdherence = 5;
	public float slopesGroundAdherence = 1;
	public float stairsGroundAdherence = 8;
	public float noAnimatorSlopesGroundAdherence = 2;
	public float noAnimatorStairsGroundAdherence = 2;
	public bool useMaxWalkSurfaceAngle;
	public float maxWalkSurfaceAngle;
	bool onGroundChecked;
	float currentGroundAdherence;
	GameObject currentSurfaceBelowPlayer;
	public bool checkOnGroundStatePausedFFOrZG;
	public bool checkOnGroundStatePaused;
	[Range (0, 1)] public float maxRayDistanceRange = 0.5f;
	[Range (0, 1)] public float maxSlopeRayDistance = 0.5f;
	[Range (0, 1)] public float maxStairsRayDistance = 0.5f;

	Vector3 hitPoint;
	Vector3 rayPos;
	bool canSetGroundState;
	float lastTimeGround;

	//Collider variables
	public float originalHeight;
	public float originalRadius;

	//Air variables
	public float airSpeed = 6;
	public float airControl = 2;
	public float maxTimeInAirDamage;
	float lastTimeAir;

	//Gravity variables
	public float gravityMultiplier = 2;
	public float gravityForce = -9.8f;
	public float slowDownGravityMultiplier;
	float originalGravityMultiplier;
	public Vector3 currentNormal = new Vector3 (0, 1, 0);
	Vector3 extraGravityForce;
	float currentGravityMultiplier;
	float originalGravityForce;

	public bool gravityForcePaused;
	float lastTimeGravityForcePaused;
	bool unpauseGravityForceActive;

	//Zero Grvity Mode Variables
	public bool zeroGravityModeOn;
	public float zeroGravityMovementSpeed;
	public float zeroGravityControlSpeed;
	public float zeroGravityLookCameraSpeed;
	public bool useGravityDirectionLandMark;
	public Transform forwardSurfaceRayPosition;
	public float maxDistanceToAdjust;
	public bool pauseCheckOnGroundStateZG;
	public bool pushPlayerWhenZeroGravityModeIsEnabled;
	public float pushZeroGravityEnabledAmount;
	public bool canMoveVerticallyOnZeroGravity;
	public bool canMoveVerticallyAndHorizontalZG;
	public float zeroGravitySpeedMultiplier = 1.4f;
	float currentZeroGravitySpeedMultiplier = 1;

	//Free Floating Variables
	public bool freeFloatingModeOn;
	public float freeFloatingMovementSpeed;
	public float freeFloatingControlSpeed;
	public bool pauseCheckOnGroundStateFF;
	public bool canMoveVerticallyOnFreeFloating;
	public bool canMoveVerticallyAndHorizontalFF;
	public float pushFreeFloatingModeEnabledAmount;
	public bool canMoveVertically;
	public bool canMoveVerticallyAndHorizontal;
	public bool movingVertically;
	public float freeFloatingSpeedMultiplier = 1.4f;
	float currentFreeFloatingSpeedMultiplier = 1;

	public bool useMaxAngleToCheckOnGroundStateZGFF;
	public float maxAngleToChekOnGroundStateZGFF;

	bool movementSpeedIncreased;

	//Jump variables
	public bool enabledRegularJump;
	public bool enabledDoubleJump;
	public int maxNumberJumpsInAir;
	public float jumpPower = 12;
	public bool jump;
	public bool jumpInput;
	public bool doubleJump;
	public bool holdJumpSlowDownFall;
	float readyToJumpTime = 0.25f;
	float readyToDoubleJumpTime = 0.2f;
	float lastJumpTime;
	float lastDoubleJumpTime;
	int jumpsAmount;
	float currentJumpLeg;
	bool canJumpActive = true;


	//Land mark variables
	public bool useLandMark;
	public float maxLandDistance;
	public float minDistanceShowLandMark;
	public GameObject landMark;
	public Transform landMark1;
	public Transform landMark2;

	Transform head;
	Vector3 landMarkRayPosition;
	Vector3 landMarkRayDirection;
	float currentMaxLandDistance;
	Vector3 landMarkForwardDirection;
	Quaternion landMarkTargetRotation;

	//Locked camera variables
	public bool lockedCameraActive;
	[Range (0, 1)] public float tankModeRotationSpeed;
	public bool useTankControls;
	public lockedPlayerMovementMode lockedPlayerMovement;
	public bool canMoveWhileAimFirstPerson = true;
	public bool canMoveWhileAimThirdPerson = true;
	public bool canMoveWhileAimLockedCamera = true;

	public bool useRelativeMovementToLockedCamera;

	public enum lockedPlayerMovementMode
	{
		world3d,
		world2_5d
	}

	public bool crouchVerticalInput2_5dEnabled;

	public bool checkCameraDirectionFromLockedToFree;
	bool tankModeCurrentlyEnabled;

	bool inputNotPressed;

	//Air dash ability variables
	public bool airDashEnabled;
	public float airDashForce = 20;
	public float airDashColdDown = 0.5f;
	public bool pauseGravityForce;
	public float gravityForcePausedTime = 1;
	public bool resetGravityForceOnDash;
	public bool useDashLimit;
	public int dashLimit;
	public bool changeCameraFovOnDash;
	public float cameraFovOnDash;
	public float cameraFovOnDashSpeed;
	int currentNumberOfDash;
	float lastTimeDash;

	public bool visibleToAI = true;

	public int playerID;

	public bool regularMovementOnBulletTime;
	float currentDeltaTime;

	float currentUpdateDeltaTime;
	float currentFixedUpdateDeltaTime;

	//Crouch variables
	public bool crouching = false;
	public bool useAutoCrouch;
	public float raycastDistanceToAutoCrouch;
	public Transform autoCrouchRayPosition;
	public Vector3 secondRaycastOffset;
	public LayerMask layerToCrouch;
	public float capsuleHeightOnCrouch = 1;

	//Run variables
	public bool running;

	//AI variables
	public bool usedByAI;
	Vector3 navMeshCurrentLookPos;
	Vector3 navMeshMoveInput;
	bool lookInCameraDirection;
	public bool playerNavMeshEnabled;
	bool characterControlOverrideActive;

	//Fly mode variables
	public bool flyModeActive;
	public float flyModeForce;
	public float flyModeAirControl;
	public float flyModeAirSpeed;
	public float flyModeTurboSpeed;
	public bool flyModeTurboActive;
	Vector3 lastDirection;

	//Jetpack variables
	public bool jetPackEquiped;
	public bool usingJetpack;
	float jetpackForce;
	float jetpackAirControl;
	float jetpackAirSpeed;

	//Gizmo variables
	public bool showGizmo;
	public Color gizmoColor;
	public Color gizmoLabelColor;
	public float gizmoRadius;

	public bool showAdvancedSettings;
	public bool showPlayerStates;

	//Vehicle variables
	GameObject currentVehicle;
	public bool driving;
	public bool drivingRemotely;
	public bool overridingElement;

	//Controller variables
	float turnAmount;
	float forwardAmount;
	float runCycleLegOffset = 0.2f;
	float lastTimeMoved;
	bool slowingFallInput;

	//External force camera variables
	bool externalForceActive;
	Vector3 externalForceValue;
	public headBob headBobManager;

	//Player components
	public playerInputManager playerInput;
	public otherPowers powersManager;
	public playerWeaponsManager weaponsManager;
	public health healthManager;
	public IKSystem IKSystemManager;

	//Camera variables
	public GameObject playerCameraGameObject;
	Transform playerCameraTransform;
	public Transform mainCameraTransform;
	public playerCamera playerCameraManager;
	public Animator animator;
	public footStepManager stepManager;
	public Rigidbody mainRigidbody;
	public gravitySystem gravityManager;
	public characterStateIconSystem characterStateIconManager;
	public CapsuleCollider capsule;
	public Collider mainCollider;

	float originalMoveSpeedMultiplier;
	float originalJumpPower;
	float originalAirSpeed;
	float originalAirControl;

	float originalNoAnimatorAirSpeed;

	GameObject playerManagersParentGameObject;

	public GameObject characterMeshGameObject;

	public List<GameObject> extraCharacterMeshGameObject = new List<GameObject> ();

	bool firstPersonActive;

	Transform currentLockedCameraTransform;

	bool inputCanBeUsed;
	bool characterRotatingToSurface;

	float raycastDistance;
	Vector3 rayPosition;
	Vector3 rayDirection;

	float playerVerticalPosition;
	float hitPointVerticalPosition;
	float upDistance;
	float hitAngle;
	bool slopeFound;
	bool stairsFound;

	public bool playerSetAsChildOfParent;
	float waitTimeToSlowDown;

	bool previouslyOnGround;

	public bool checkOnGroungPaused;

	bool rotatingPlayerIn2_5dToRight;
	bool rotatingPlayerIn2_5dToRLeft;

	bool canRotateOn3dWorld;
	bool canRotateOn2_5dWorld;

	bool applyRootMotionAlwaysActive;

	public bool actionActive;

	Coroutine resetInputCoroutine;

	bool playerUsingInput;

	bool strafeModeActive;

	public Vector3 currentForwardDirection;
	public Vector3 currentRightDirection;

	bool originalLookAlwaysInCameraDirection;

	Vector3 currentPlayerPosition;
	Vector3 currentPlayerUp;

	bool stairStepDetected;

	bool setHeadbodStatesPaused;

	void Start ()
	{
		//set the collider center in the correct place
		originalHeight = capsule.height;
		originalRadius = capsule.radius;
		capsule.center = Vector3.up * capsule.height * 0.5f;

		//get the player camera
		playerCameraTransform = playerCameraGameObject.transform;

		head = animator.GetBoneTransform (HumanBodyBones.Head);

		originalWalkSpeedValue = walkSpeed;
		originalGravityMultiplier = gravityMultiplier;
		currentGravityMultiplier = gravityMultiplier;
		originalGravityForce = gravityForce;

		if (useLandMark) {
			landMark.SetActive (false);
		}
			
		originalAnimationSpeed = animSpeedMultiplier;

		//get all the important parameters of player controller
		originalMoveSpeedMultiplier = moveSpeedMultiplier;
		originalJumpPower = jumpPower;
		originalAirSpeed = airSpeed;
		originalAirControl = airControl;

		originalNoAnimatorAirSpeed = noAnimatorAirSpeed;

		if (transform.parent) {
			playerManagersParentGameObject = transform.parent.gameObject;
		}

		//no animator original movement values assignment
		originalNoAnimWalkMovementSpeed = noAnimatorWalkMovementSpeed;
		originalNoAnimRunMovementSpeed = noAnimatorRunMovementSpeed;
		originalNoAnimCrouchMovementSpeed = noAnimatorCrouchMovementSpeed;
		originalNoAnimStrafeMovementSpeed = noAnimatorStrafeMovementSpeed;
		originalNoAnimWalkBackwardMovementSpeed = noAnimatorWalkBackwardMovementSpeed;
		originalNoAnimRunBackwardMovementSpeed = noAnimatorRunBackwardMovementSpeed;
		originalNoAnimCrouchBackwardMovementSpeed = noAnimatorCrouchBackwardMovementSpeed;
		originalNoAnimStrafeBackwardMovementSpeed = noAnimatorStrafeBackwardMovementSpeed;
		originalNoAnimCanRun = noAnimatorCanRun;

		originalSetUseRootMotionActiveState = useRootMotionActive;

		originalLookAlwaysInCameraDirection = lookAlwaysInCameraDirection;
	}

	void Update ()
	{
		currentUpdateDeltaTime = getCurrentDeltaTime ();

		characterRotatingToSurface = gravityManager.isCharacterRotatingToSurface ();

		inputCanBeUsed = canUseInput ();

		playerIsAiming = isPlayerAiming ();

		lookInCameraDirectionOnFreeFireActive = hasToLookInCameraDirectionOnFreeFire ();

		playerUsingInput = isPlayerUsingInput ();

		if (inputCanBeUsed && !playerOnGround && !gravityPowerActive && holdJumpSlowDownFall) {
			waitTimeToSlowDown = lastJumpTime + 0.2f;
			if (enabledDoubleJump && doubleJump) {
				waitTimeToSlowDown += lastDoubleJumpTime + 1;
			}
		}

		if (useLandMark) {
			landMarkRayPosition = transform.position;
			landMarkRayDirection = -transform.up;

			if (gravityManager.isPlayerSearchingGravitySurface ()) {
				landMarkRayPosition = mainCameraTransform.position;
				landMarkRayDirection = mainCameraTransform.forward;
			}

			if (zeroGravityModeOn && useGravityDirectionLandMark) {
				if (firstPersonActive) {
					landMarkRayPosition = mainCameraTransform.position;
					landMarkRayDirection = mainCameraTransform.forward;
				} else {
					landMarkRayPosition = forwardSurfaceRayPosition.position;
					landMarkRayDirection = forwardSurfaceRayPosition.forward;
				}
				currentMaxLandDistance = maxDistanceToAdjust; 
			} else {
				currentMaxLandDistance = maxLandDistance;
			}

			if (Physics.Raycast (landMarkRayPosition, landMarkRayDirection, out hit, currentMaxLandDistance, layer) && !playerOnGround && canMove) {
				if (useLandMark) {
					if (hit.distance >= minDistanceShowLandMark) {
						if (!landMark.activeSelf) {
							landMark.SetActive (true);
						}

						landMark.transform.position = hit.point + hit.normal * 0.02f;
						landMarkForwardDirection = Vector3.Cross (landMark.transform.right, hit.normal);
						landMarkTargetRotation = Quaternion.LookRotation (landMarkForwardDirection, hit.normal);
						landMark.transform.rotation = landMarkTargetRotation;
						landMark1.transform.Rotate (0, 100 * currentUpdateDeltaTime, 0);
						landMark2.transform.Rotate (0, -100 * currentUpdateDeltaTime, 0);
					} else {
						if (useLandMark) {
							if (landMark.activeSelf) {
								landMark.SetActive (false);
							}
						}
					}
				}
			} else {
				if (useLandMark) {
					if (landMark.activeSelf) {
						landMark.SetActive (false);
					}
				}
			}
		}

		if (lockedCameraActive) {
			if (lockedPlayerMovement == lockedPlayerMovementMode.world2_5d) {
				if (playerCameraManager.isMoveInXAxisOn2_5d ()) {
					transform.position = new Vector3 (transform.position.x, transform.position.y, playerCameraManager.getOriginalLockedCameraPivotPosition ().z);
				} else {
					transform.position = new Vector3 (playerCameraManager.getOriginalLockedCameraPivotPosition ().x, transform.position.y, transform.position.z);
				}
			}
		}

		if (gravityForcePaused) {
			if (unpauseGravityForceActive && Time.time > lastTimeGravityForcePaused + gravityForcePausedTime) {
				gravityForcePaused = false;
				unpauseGravityForceActive = false;
			}
		}

		lookInCameraDirectionActive = hasToLookInCameraDirection () && !crouching;

		strafeModeActive = ((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirectionActive) && playerOnGround;
	}

	void FixedUpdate ()
	{
		currentFixedUpdateDeltaTime = getCurrentDeltaTime ();

		//convert the input from keyboard or a touch screen into values to move the player, given the camera direction
		if (canMove && !usedByAI && !playerNavMeshEnabled) {
			axisValues = playerInput.getPlayerMovementAxis ("keys");

			horizontalInput = axisValues.x;
			verticalInput = axisValues.y;
		} else {
			axisValues = Vector3.zero;
		}

		if (canMove) {
			if (playerInput) {
				rawAxisValues = playerInput.getPlayerRawMovementAxis ();
			}
		} else {
			rawAxisValues = Vector2.zero;
		}
			
		if (!usedByAI && !playerNavMeshEnabled) {
			//get the axis of the player camera, to move him properly
			if (lockedCameraActive) {

				currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();
				if (lockedPlayerMovement == lockedPlayerMovementMode.world3d) {

					//player can move while aiming
					if (!canMoveWhileAimLockedCamera && playerIsAiming) {
						verticalInput = 0;
						horizontalInput = 0;
					}

					//if the player is on tank mode, use his forward and right direction as the input
					if (useTankControls) {
						currentForwardDirection = transform.forward;
						currentRightDirection = transform.right;
					} else {
						//else, the player will follow the direction of the current locked camera 

						//if the player is looking at a target, the input direction used will be the player camera
						if (playerCameraManager.isPlayerLookingAtTarget ()) {
							currentForwardDirection = playerCameraTransform.forward;
							currentRightDirection = mainCameraTransform.right;
						} else {
							//else, he will use the locked camera direction
							currentForwardDirection = currentLockedCameraTransform.forward;
							currentRightDirection = currentLockedCameraTransform.right;
						}
					}
				} else {
					//else, the player is moving in 2.5d camera, so use only the horizontal input

					currentForwardDirection = Vector3.zero;
					currentRightDirection = currentLockedCameraTransform.right;
				}
			} else {
				//the player is on free camera mode

				//check if the player can move while aiming on first or third person according to settings
				if (playerIsAiming && !usingFreeFireMode) {
					if ((firstPersonActive && !canMoveWhileAimFirstPerson) || (!firstPersonActive && !canMoveWhileAimThirdPerson)) {
						verticalInput = 0;
						horizontalInput = 0;
					}
				}

				//in other case, the player uses the player camera direction as input
				if (checkCameraDirectionFromLockedToFree) {
					currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();
					//keep the latest locked camera direction until the player stops to move

					currentForwardDirection = currentLockedCameraTransform.forward;
					currentRightDirection = currentLockedCameraTransform.right;

					if (!isPlayerMoving (0.6f) && !playerUsingInput) {
						inputNotPressed = true;
					}

					if (inputNotPressed && (playerUsingInput || !isPlayerMoving (0))) {
						checkCameraDirectionFromLockedToFree = false;

						inputNotPressed = false;
					}
				} else {
					currentForwardDirection = playerCameraTransform.forward;
					currentRightDirection = mainCameraTransform.right;
				}
			}

			//the camera direccion and input is override by some external function, to make the playe to move toward some direction or position
			if (overrideMainCameraTransformActive) {
				currentForwardDirection = overrideMainCameraTransform.forward;
				currentRightDirection = overrideMainCameraTransform.right;
			}

			moveInput = (verticalInput * currentForwardDirection + horizontalInput * currentRightDirection) * walkSpeed;	

		} else {
			if (playerNavMeshEnabled) {
				currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();
			}

			axisValues = new Vector2 (navMeshMoveInput.x, navMeshMoveInput.z);

			rawAxisValues = axisValues;
			rawAxisValues.Normalize ();

			if (playerIsAiming) {
				moveInput = navMeshMoveInput * walkSpeed;
				horizontalInput = 0;
				verticalInput = moveInput.magnitude;
			} else {
				moveInput = navMeshMoveInput * walkSpeed;
				horizontalInput = moveInput.x;
				verticalInput = moveInput.z;
			}
		}

		//isMoving is true if the player is moving, else is false
		isMoving = Mathf.Abs (horizontalInput) > 0.1f || Mathf.Abs (verticalInput) > 0.1f;
		if (moveInput.magnitude > 1) {
			moveInput.Normalize ();
		}
			
		//get the velocity of the rigidbody
		if (!gravityPowerActive && (usingAnimator || (!usingAnimator && !playerOnGround))) {
			currentVelocity = mainRigidbody.velocity;
		}

		//convert the global movement in local
		getMoveInput ();

		//look in camera direction when the player is aiming
		lookCameraDirection ();

		//add an extra rotation to the player to get a better control of him
		addExtraRotation ();

		//if the animator is used, then
		if (usingAnimator) {

			//check when the player is on ground
			checkOnGround (); 

			//update mecanim
			updateAnimator ();
		} else {
			//else, apply force to the player's rigidbody
			if (playerOnGround) {
				if (!isMoving) {
					moveInput = Vector3.zero;
				}

				if (running) {
					if (noAnimatorCanRun) {
						if (verticalInput < 0) {
							if (noAnimatorCanRunBackwards) {
								if (crouching) {
									noAnimatorCurrentMovementSpeed = noAnimatorRunCrouchBackwardMovementSpeed;
								} else {
									noAnimatorCurrentMovementSpeed = noAnimatorRunBackwardMovementSpeed;
								}
							} else {
								powersManager.stopRun ();
							}
						} else {
							if (crouching) {
								noAnimatorCurrentMovementSpeed = noAnimatorRunCrouchMovementSpeed;
							} else {
								noAnimatorCurrentMovementSpeed = noAnimatorRunMovementSpeed;
							}
						}
					} else {
						powersManager.stopRun ();
					}
				} else if (crouching) {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorCrouchMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorCrouchBackwardMovementSpeed;
					}
				} else if (Mathf.Abs (verticalInput) > 0.5f && Mathf.Abs (horizontalInput) > 0.5f) {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorStrafeMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorStrafeBackwardMovementSpeed;
					}
				} else {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorWalkMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorWalkBackwardMovementSpeed;
					}
				}

				noAnimatorCurrentForce = moveInput * noAnimatorSpeed * noAnimatorCurrentMovementSpeed;
				//substract the local Y axis velocity of the rigidbody
				noAnimatorCurrentForce = noAnimatorCurrentForce - transform.up * transform.InverseTransformDirection (noAnimatorCurrentForce).y;

				velocityChange = noAnimatorCurrentForce - mainRigidbody.velocity;

				velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChange);

				mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
			} else {
				noAnimatorCurrentForce = moveInput * noAnimatorAirSpeed + transform.InverseTransformDirection (currentVelocity).y * transform.up;

				velocityChange = noAnimatorCurrentForce - mainRigidbody.velocity;

				velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChange);

				mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
			}

			currentVelocity = mainRigidbody.velocity;

			currentVelocityChangeMagnitude = mainRigidbody.velocity.magnitude;

			checkOnGround (); 
		}

		//check if the player is on ground or in air
		//also set the friction of the character if he is on the ground or in the air
		if (playerOnGround) {
			
			onGroundVelocity ();

			if (!onGroundChecked) {
				gravityManager.onGroundOrOnAir (true);
				onGroundChecked = true;

				//send a message to the headbob in the camera, when the player lands from a jump
				if (headBobManager.headBobEnabled) {
					headBobManager.setState ("Jump End");
				}

				stepManager.setFootStepState ("Air Landing");

				//set the number of jumps made by the player since this moment
				jumpsAmount = 0;

				weaponsManager.setWeaponsJumpEndPositionState (true);

				if (!healthManager.isDead ()) {
					//check the last time since the player is in the air, falling in its gravity direction
					//if the player has been in the air more time than maxTimeInAirDamage and his velocity is higher than 15, then apply damage
					if (damageFallEnabled && Time.time > lastTimeFalling + maxTimeInAirDamage && !gravityPowerActive && mainRigidbody.velocity.magnitude > 15) {
						//get the last time since the player is in the air and his velocity, and call the health damage function
						float damageValue = Mathf.Abs (Time.time - lastTimeFalling) * mainRigidbody.velocity.magnitude;
						if (fallindDamageMultiplier != 1) {
							damageValue *= fallindDamageMultiplier;
						}
						if (damageValue > healthManager.getCurrentHealthAmount ()) {
							damageValue = healthManager.getCurrentHealthAmount ();
						}
	
						healthManager.setDamage (damageValue, -mainRigidbody.velocity.normalized, transform.position + transform.up, gameObject, gameObject, false, false);

						//call another function when the player receives damage from a long fall
						if (callEventOnFallDamage) {
							if (damageValue >= minDamageToCallFunction) {
								eventOnFallDamage.Invoke ();
							}
						}
					}
				}

				if (freeFloatingModeOn || zeroGravityModeOn) {
					setFootStepManagerState (false);
				}

				lastTimeGround = Time.time;
			}

			if (!stepManager.isDelayToNewStateActive () && playerOnGround) {
				if (running) {
					if (crouching) {
						stepManager.setFootStepState ("Run Crouching");
					} else {
						stepManager.setFootStepState ("Running");
					}
				} else {
					if (crouching) {
						stepManager.setFootStepState ("Crouching");							
					} else {
						stepManager.setFootStepState ("Walking");
					}
				}
			}

			//change the collider material when the player moves and when the player is not moving
			if (!physicMaterialAssigmentPaused) {
				if (moveInput.magnitude == 0) {
					capsule.material = highFrictionMaterial;
				} else {
					capsule.material = zeroFrictionMaterial;
				}
			}

			//check the headbob state
			if (headBobManager.headBobEnabled && !setHeadbodStatesPaused) {
				if (isMoving) {
					if (running) {
						if (crouching) {
							headBobManager.setState ("Run Crouching");
						} else {
							headBobManager.setState ("Running");
						}
					} else {
						if (crouching) {
							headBobManager.setState ("Crouching");
						} else {
							headBobManager.setState ("Walking");
						}
					}
					if (headBobManager.useDynamicIdle) {
						setLastTimeMoved ();
					}
				} else {
					if (headBobManager.useDynamicIdle && canMove && !usingDevice && firstPersonActive && !headBobManager.externalShakingActive) {
						if (Time.time > lastTimeMoved + headBobManager.timeToActiveDynamicIdle &&
						    Time.time > playerCameraManager.getLastTimeMoved () + headBobManager.timeToActiveDynamicIdle &&
						    Time.time > weaponsManager.getLastTimeFired () + headBobManager.timeToActiveDynamicIdle &&
						    Time.time > powersManager.getLastTimeFired () + headBobManager.timeToActiveDynamicIdle &&
						    !headBobPaused) {
							headBobManager.setState ("Dynamic Idle");
						} else {
							headBobManager.setState ("Static Idle");
						}
					} else {
						headBobManager.setState ("Static Idle");
					}
				}
			}
		}

		//the player is in the air, so
		else {
			//call the air velocity function
			onAirVelocity ();

			if (onGroundChecked) {
				//set in other script this state
				gravityManager.onGroundOrOnAir (false);

				onGroundChecked = false;

				setLastTimeFalling ();

				weaponsManager.setWeaponsJumpStartPositionState (true);

				currentJumpLeg = 0;

				if (zeroGravityModeOn || freeFloatingModeOn) {
					setCheckOnGrundStatePausedFFOrZGState (true);
				
					setFootStepManagerState (true);
				}
			}
			capsule.material = zeroFrictionMaterial;

			if (headBobManager.headBobEnabled) {
				headBobManager.setState ("Air");
				if (headBobManager.useDynamicIdle) {
					setLastTimeMoved ();
				}
			}
		}

		//if the player is using the jetpack
		if (usingJetpack) {
			airMove = moveInput * jetpackAirSpeed + transform.InverseTransformDirection (currentVelocity).y * transform.up;
			currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * jetpackAirControl);	
			mainRigidbody.AddForce (-gravityForce * mainRigidbody.mass * transform.up * jetpackForce);
		}

		//if the player is flying
		if (flyModeActive) {
			Vector3 cameraForward = mainCameraTransform.TransformDirection (Vector3.forward);
			cameraForward = cameraForward.normalized;
			Vector3 targetDirection = verticalInput * mainCameraTransform.forward + horizontalInput * mainCameraTransform.right;

			if (isMoving && targetDirection != Vector3.zero) {
				Quaternion targetRotation = Quaternion.LookRotation (targetDirection, playerCameraTransform.up);
				targetRotation *= Quaternion.Euler (90, 0, 0);
				Quaternion newRotation = Quaternion.Slerp (mainRigidbody.rotation, targetRotation, flyModeAirControl * currentFixedUpdateDeltaTime);
				mainRigidbody.MoveRotation (newRotation);
				lastDirection = targetDirection;
			}

			if (!(Mathf.Abs (horizontalInput) > 0.9f || Mathf.Abs (verticalInput) > 0.9f)) {
				Vector3 repositioning = lastDirection;
				if (repositioning != Vector3.zero) {
					repositioning.y = 0;
					Quaternion targetRotation = Quaternion.LookRotation (repositioning, playerCameraTransform.up);
					Quaternion newRotation = Quaternion.Slerp (mainRigidbody.rotation, targetRotation, flyModeAirControl * currentFixedUpdateDeltaTime);
					mainRigidbody.MoveRotation (newRotation);
				}
			}

			if (flyModeTurboActive) {
				mainRigidbody.AddForce (targetDirection * flyModeForce * flyModeTurboSpeed);
			} else {
				mainRigidbody.AddForce (targetDirection * flyModeForce);
			}
		}

		//in case the player is using the gravity power, the update of the rigidbody velocity stops
		if (!gravityPowerActive) {
			if (usingAnimator || (!usingAnimator && !playerOnGround)) {
				mainRigidbody.velocity = currentVelocity;
			}

			if (externalForceActive) {
				mainRigidbody.velocity += externalForceValue;
				externalForceActive = false;
			}
		}
	}

	public Vector3 getCurrentForwardDirection ()
	{
		return currentForwardDirection;
	}

	public Vector3 getCurrentRightDirection ()
	{
		return currentRightDirection;
	}

	//convert the global movement into local movement
	void getMoveInput ()
	{
		Vector3 localMove = transform.InverseTransformDirection (moveInput);

		//get the amount of rotation added to the character mecanim
		if (moveInput.magnitude > 0) {
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
		} else {
			turnAmount = Mathf.Atan2 (0, 0);
		}

		//adjust player orientation to lef or right according to the input direction pressed just once
		if (lockedPlayerMovement == lockedPlayerMovementMode.world2_5d) {
			if (horizontalInput > 0) {
				rotatingPlayerIn2_5dToRight = true;
				rotatingPlayerIn2_5dToRLeft = false;
			}

			if (horizontalInput < 0) {
				rotatingPlayerIn2_5dToRLeft = true;
				rotatingPlayerIn2_5dToRight = false;
			}

			if (Mathf.Abs (horizontalInput) == 1) {
				rotatingPlayerIn2_5dToRLeft = false;
				rotatingPlayerIn2_5dToRight = false;
			}

			if (rotatingPlayerIn2_5dToRight) {
				float angle = Vector3.SignedAngle (transform.forward, currentLockedCameraTransform.right, transform.up);
				if (Mathf.Abs (angle) > 5) { 
					turnAmount = angle * Mathf.Deg2Rad;
				} else {
					turnAmount = 0;
					rotatingPlayerIn2_5dToRight = false;
				}
			}

			if (rotatingPlayerIn2_5dToRLeft) {
				float angle = Vector3.SignedAngle (transform.forward, -currentLockedCameraTransform.right, transform.up);
				if (Mathf.Abs (angle) > 5) {
					turnAmount = angle * Mathf.Deg2Rad;
				} else {
					turnAmount = 0;
					rotatingPlayerIn2_5dToRLeft = false;
				}
			}

			if (playerIsAiming) {
				if (horizontalInput != 0) {
					turnAmount = 0;

					rotatingPlayerIn2_5dToRLeft = false;
					rotatingPlayerIn2_5dToRight = false;
				}
			}

			//crouch using vertical input
			if (crouchVerticalInput2_5dEnabled) {
				if ((!crouching && verticalInput < 0) || (crouching && verticalInput > 0)) {
					crouch ();
				}
			}

			//move vertically on free floating mode or zero gravity mode
			if (freeFloatingModeOn || zeroGravityModeOn) {
				if (rawAxisValues.y != 0) {
					movingVertically = true;
				} else {
					movingVertically = false;
				}
			}
		}

		//get the amount of movement in forward direction

		forwardAmount = localMove.z;

		if (!lockedCameraActive) {
			forwardAmount = Mathf.Clamp (forwardAmount, -walkSpeed, walkSpeed);
		}
	}

	//function used when the player is aim mode, so the character will rotate in the camera direction
	void lookCameraDirection ()
	{
		if (usingAnimator) {
			if ((((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirection || lookInCameraDirectionActive) && playerOnGround) ||

			    (playerIsAiming && !usingFreeFireMode && !playerOnGround)) {

				bool ZGFFModeOnAir = ((zeroGravityModeOn || freeFloatingModeOn) && !playerOnGround);

				if (!ZGFFModeOnAir) {
					//get the camera direction, getting the local direction in any surface
					Vector3 forward = playerCameraTransform.TransformDirection (Vector3.forward);
					float newAngle = Vector3.Angle (forward, transform.forward);
					Vector3 targetDirection = Vector3.zero;
					Quaternion targetRotation = Quaternion.identity;

					forward = forward - transform.up * transform.InverseTransformDirection (forward).y;
					forward = forward.normalized;
					targetDirection = forward;
					Quaternion currentRotation = Quaternion.LookRotation (targetDirection, transform.up);
					targetRotation = Quaternion.Lerp (mainRigidbody.rotation, currentRotation, aimTurnSpeed * currentFixedUpdateDeltaTime);

					bool canRotatePlayer = (!lookAlwaysInCameraDirection ||
					                       (lookAlwaysInCameraDirection && !lookOnlyIfMoving) ||
					                       (lookAlwaysInCameraDirection && lookOnlyIfMoving && playerUsingInput) ||
					                       (lookAlwaysInCameraDirection && lookOnlyIfMoving && !playerUsingInput && playerIsAiming));

					if (newAngle >= Mathf.Abs (thresholdAngleDifference)) {
						//if the player is not moving, set the turnamount to rotate him around, setting its turn animation properly
						if (canRotatePlayer) {
							targetRotation = Quaternion.Lerp (mainRigidbody.rotation, currentRotation, autoTurnSpeed * currentFixedUpdateDeltaTime);
							mainRigidbody.MoveRotation (targetRotation);

							Vector3 lookDelta = transform.InverseTransformDirection (targetDirection * 100);
							float lookAngle = Mathf.Atan2 (lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;
							turnAmount += lookAngle * .01f * 6;
						}
					} else {
						turnAmount = Mathf.MoveTowards (turnAmount, 0, currentFixedUpdateDeltaTime * 2);
					}

					if (rotateDirectlyTowardCameraOnStrafe) {
						turnAmount = 0;
					}

					if (canRotatePlayer) {
						mainRigidbody.MoveRotation (targetRotation);
					}
				}
			}

			if ((zeroGravityModeOn || freeFloatingModeOn) && !firstPersonActive && !playerOnGround && !characterRotatingToSurface) {
				if (lockedPlayerMovement == lockedPlayerMovementMode.world3d || playerIsAiming) {
					if (playerIsAiming) {
						transform.rotation = playerCameraGameObject.transform.rotation;
					} else {
						Quaternion targetRotation = Quaternion.Lerp (transform.rotation, playerCameraGameObject.transform.rotation, zeroGravityLookCameraSpeed * currentFixedUpdateDeltaTime);
						transform.rotation = targetRotation;
					}
				}
			}
		} else {
			if (!characterRotatingToSurface) {
				transform.rotation = playerCameraGameObject.transform.rotation;
			}
		}
	}

	void addExtraRotation ()
	{
		if (usingAnimator) {
			if ((!playerIsAiming || !lookInCameraDirectionOnFreeFireActive) && !lookInCameraDirectionActive) {

				canRotateOn3dWorld = lockedPlayerMovement == lockedPlayerMovementMode.world3d && ((!freeFloatingModeOn && !zeroGravityModeOn) || playerOnGround);
				canRotateOn2_5dWorld = lockedPlayerMovement == lockedPlayerMovementMode.world2_5d;

				if (canRotateOn3dWorld || canRotateOn2_5dWorld) {
					
					//add an extra rotation to the player to get a smooth movement
					if (lockedCameraActive) {
						float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);

						if (useTankControls) {
							turnSpeed *= tankModeRotationSpeed;
							transform.Rotate (0, horizontalInput * turnSpeed * currentFixedUpdateDeltaTime * 1.5f, 0);
						} else {
							transform.Rotate (0, turnAmount * turnSpeed * currentFixedUpdateDeltaTime * 1.5f, 0);
						}
					} else {
						float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
						transform.Rotate (0, turnAmount * turnSpeed * currentFixedUpdateDeltaTime * 1.5f, 0);
					}
				}
			}
		}
	}

	//set the normal of the player every time it is changed in the other script
	public void setCurrentNormalCharacter (Vector3 newNormal)
	{
		currentNormal = newNormal;
	}

	//check if the player jumps
	void onGroundVelocity ()
	{
		if (moveInput.magnitude == 0) {
			currentVelocity = Vector3.zero;
		}

		//check when the player is able to jump, according to the timer and the animator state
		bool animationGrounded = false;
		if (usingAnimator) {
			animationGrounded = animator.GetCurrentAnimatorStateInfo (baseLayerIndex).IsName ("Grounded");
			
			if (!animationGrounded) {
				if (playerOnGround && Time.time > lastTimeGround + 0.5f) {
					animationGrounded = true;
				}
			}
		} else {
			animationGrounded = playerOnGround;
		}

		//if the player jumps, apply velocity to its rigidbody
		if (jumpInput && Time.time > lastTimeAir + readyToJumpTime && animationGrounded) {			
			if (playerOnGround) {
				stepManager.setFootStepState ("Jumping");
			}

			setOnGroundState (false);

			if (usingAnimator) {
				currentVelocity = moveInput * airSpeed;
			} else {
				currentVelocity = moveInput * noAnimatorAirSpeed;
			}

			currentVelocity = currentVelocity + transform.up * jumpPower;
			jump = true;
			lastJumpTime = Time.time;

			if (headBobManager.headBobEnabled) {
				//this is used for the headbod, to shake the camera when the player jumps
				headBobManager.setState ("Jump Start");
			}

			weaponsManager.setWeaponsJumpStartPositionState (true);
		}

		jumpInput = false;
	}

	//check if the player is in the air falling, applying the gravity force in his local up negative
	void onAirVelocity ()
	{
		if (!gravityPowerActive && !usingJetpack && !flyModeActive && !characterRotatingToSurface) {
			//when the player falls, allow him to move to his right, left, forward and backward with WASD
		
			if (zeroGravityModeOn || freeFloatingModeOn) {
				
				secondaryMoveInput = moveInput;

				canMoveVertically = (zeroGravityModeOn && canMoveVerticallyOnZeroGravity) || (freeFloatingModeOn && canMoveVerticallyOnFreeFloating);

				canMoveVerticallyAndHorizontal = (zeroGravityModeOn && canMoveVerticallyAndHorizontalZG) || (freeFloatingModeOn && canMoveVerticallyAndHorizontalFF);
				
				if (movingVertically) {
					if (canMoveVerticallyAndHorizontal) {
						secondaryMoveInput += verticalInput * playerCameraTransform.up * walkSpeed;	
					} else {
						secondaryMoveInput = (verticalInput * playerCameraTransform.up + horizontalInput * mainCameraTransform.right) * walkSpeed;	
					}
				}

				if (zeroGravityModeOn) {
					secondaryMove = secondaryMoveInput * zeroGravityMovementSpeed * currentZeroGravitySpeedMultiplier;
					currentVelocity = Vector3.Lerp (currentVelocity, secondaryMove, currentFixedUpdateDeltaTime * zeroGravityControlSpeed);
				}

				if (freeFloatingModeOn) {
					secondaryMove = secondaryMoveInput * freeFloatingMovementSpeed * currentFreeFloatingSpeedMultiplier;
					currentVelocity = Vector3.Lerp (currentVelocity, secondaryMove, currentFixedUpdateDeltaTime * freeFloatingControlSpeed);
				}
			} else {
				
				if (usingAnimator) {
					airMove = moveInput * airSpeed + transform.InverseTransformDirection (currentVelocity).y * transform.up;
					currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * airControl);
				}
			}

			//also, apply force in his local negative Y Axis
			if (!playerOnGround) {
				if (slowingFallInput) {
					slowingFall = true;
					slowingFallInput = false;
				}

				if (!gravityForcePaused && !zeroGravityModeOn && !freeFloatingModeOn) {
					mainRigidbody.AddForce (gravityForce * mainRigidbody.mass * transform.up);
					extraGravityForce = (transform.up * gravityForce * gravityMultiplier) + transform.up * (-gravityForce);
					mainRigidbody.AddForce (extraGravityForce);
				}
			}

			//also apply force if the player jumps again
			if (doubleJump) {
				if (usingAnimator) {
					currentVelocity += moveInput * airSpeed;
				} else {
					currentVelocity += moveInput * noAnimatorAirSpeed;
				}

				currentVelocity = currentVelocity + transform.up * jumpPower;
				jumpsAmount++;
				lastDoubleJumpTime = Time.time;
			}
			doubleJump = false;
	
		} else {
			if (characterRotatingToSurface) {
				mainRigidbody.velocity = Vector3.zero;
				currentVelocity = Vector3.zero;
			}
		}
	}

	public void setApplyRootMotionAlwaysActiveState (bool state)
	{
		applyRootMotionAlwaysActive = state;
	}

	//update the animator values
	void updateAnimator ()
	{
		//set the rootMotion according to the player state
		if (!applyRootMotionAlwaysActive) {
			if (useRootMotionActive) {
				animator.applyRootMotion = playerOnGround;
			} else {
				animator.applyRootMotion = false;
			}
		} else {
			animator.applyRootMotion = true;
		}

		if (running && sprintEnabled) {
			forwardAmount *= 2;
		}

		if (!lockedCameraActive || lockedPlayerMovement != lockedPlayerMovementMode.world2_5d) {
			//if the player is not aiming, set the forward direction
			if (!playerIsAiming || (!lookInCameraDirectionActive && (usingFreeFireMode || (checkToKeepWeaponAfterAimingWeaponFromShooting || Time.time < lastTimeCheckToKeepWeapon + 0.2f)))) {
				animator.SetFloat ("Forward", forwardAmount, 0.1f, currentFixedUpdateDeltaTime);
			} else {
				//else, set its forward to 0, to prevent any issue
				//this value is set to 0, because the player uses another layer of the mecanim to move while he is aiming
				animator.SetFloat ("Forward", 0);
			}
		} else {
			animator.SetFloat ("Forward", forwardAmount, 0.1f, currentFixedUpdateDeltaTime);
		}

		if (isTankModeActive ()) {
			if (!playerIsAiming) {
				animator.SetFloat ("Turn", 0);

				if (tankModeCurrentlyEnabled) {
					tankModeActive = true;
				} else {
					tankModeActive = false;
				}
			} else {
				if (isPlayerMoving (0)) {
					turnAmount = 0;
				}

				animator.SetFloat ("Turn", turnAmount, 0.1f, currentFixedUpdateDeltaTime);

				tankModeActive = false;
			}
		} else {
			if (strafeModeActive) {
				if (lookAlwaysInCameraDirection && lookOnlyIfMoving) {
					if (!playerIsAiming) {
						turnAmount = 0;
						lastTimePlayerUserInput = Time.time;
					}
				}

				if (playerUsingInput) {
					turnAmount = 0;
					lastTimePlayerUserInput = Time.time;
				}
			} 

			if (lastTimePlayerUserInput > 0) {
				if (isPlayerMoving (0)) {
					turnAmount = 0;
				} else {
					lastTimePlayerUserInput = 0;
				}
			}

			animator.SetFloat ("Turn", turnAmount, 0.1f, currentFixedUpdateDeltaTime);

			tankModeActive = false;
		}

		if (Mathf.Abs (horizontalInput) > 0 || Mathf.Abs (verticalInput) > 0) {
			lastTimeInput = Time.time;
		}

		animator.SetBool ("Last Time Input Pressed", Time.time > lastTimeInput + 0.5f);

		if (usingJetpack || flyModeActive) {
			animator.SetBool ("OnGround", true);
		} else {
			if (overrideOnGroundAnimatorValueActive) {
				animator.SetBool ("OnGround", true);

				if (Time.time > lastTimeOverrideOnGroundAnimator + overrideOnGrounDuration) {
					overrideOnGroundAnimatorValueActive = false;
				}
			} else {
				animator.SetBool ("OnGround", playerOnGround);
			}
		}

		animator.SetBool ("Crouch", crouching);
		animator.SetBool ("Strafe Mode Active", strafeModeActive);

		if (lockedCameraActive) {
			if (lockedPlayerMovement == lockedPlayerMovementMode.world2_5d) {

				scrollModeActive = true;
				animator.SetBool ("Strafe Mode Active", false);

			} else {
				scrollModeActive = false;
			}
		}

		animator.SetBool ("Moving", isMoving);

		animator.SetBool ("Movement Input Active", playerUsingInput);

		animator.SetBool ("Movement Relative To Camera", lockedCameraActive && useRelativeMovementToLockedCamera);

		if (playerIsAiming) {
			currentMovementID = 1;
		} else {
			currentMovementID = 0;
		}

		animator.SetFloat ("Movement ID", currentMovementID, 0.1f, currentFixedUpdateDeltaTime);

		if (!tankModeActive && !scrollModeActive) {
			currentPlayerModeID = 0;
		} else {
			if (tankModeActive) {
				currentPlayerModeID = 1;
			}

			if (scrollModeActive) {
				currentPlayerModeID = 2;
			}
		}

		animator.SetFloat ("Player Mode ID", currentPlayerModeID, 0.1f, currentFixedUpdateDeltaTime);

		animator.SetBool ("Carrying Weapon", weaponsManager.isUsingWeapons ());

		if (lockedCameraActive && useRelativeMovementToLockedCamera) {
			currentLockedInput = (verticalInput * currentLockedCameraTransform.forward + horizontalInput * currentLockedCameraTransform.right) * walkSpeed;	
			lockedMove = transform.InverseTransformDirection (currentLockedInput);
		
			currentHorizontalValue = lockedMove.x;
			currentVerticalValue = lockedMove.z;
		} else {

			if (tankModeActive) {
				if (running && sprintEnabled) {
					verticalInput *= 2;
				} else {
					if (verticalInput > 0) {
						verticalInput = Mathf.Clamp (verticalInput, 0, walkSpeed);
					}
				}
			}

			currentHorizontalValue = horizontalInput;
			currentVerticalValue = verticalInput;
		}

		animator.SetFloat ("Horizontal", currentHorizontalValue, inputLerpSpeed, currentFixedUpdateDeltaTime);
		animator.SetFloat ("Vertical", currentVerticalValue, inputLerpSpeed, currentFixedUpdateDeltaTime);

		if (strafeModeActive) {
			if (running) {
				currentMovementSpeed = defaultStrafeRunSpeed;
			} else {
				currentMovementSpeed = defaultStrafeWalkSpeed;
			}
		} else {
			currentMovementSpeed = defaultStrafeWalkSpeed;
		}

		animator.SetFloat ("Movement Speed", currentMovementSpeed, strafeLerpSpeed, currentFixedUpdateDeltaTime);

		if (!playerOnGround) {
			//when the player enables the power gravity and he is floating in the aire, set this value to 0 to set 
			//the look like floating animation
			if (gravityPowerActive) {
				animator.SetFloat ("Jump", 0);
			}
			//else set his jump value as his current rigidbody velocity
			else {
				if (zeroGravityModeOn || freeFloatingModeOn) {
					animator.SetFloat ("Jump", 0);
				} else {
					animator.SetFloat ("Jump", transform.InverseTransformDirection (mainRigidbody.velocity).y, 0.1f, currentFixedUpdateDeltaTime / 2);
				}
			}
		}

		if (usingJetpack || flyModeActive) {
			animator.SetFloat ("Forward", 0);
			animator.SetFloat ("Turn", 0);
		}

		//this value is used to know in which leg the player has to jump, left of right
		float runCycle = Mathf.Repeat (animator.GetCurrentAnimatorStateInfo (baseLayerIndex).normalizedTime + runCycleLegOffset, 1);
		float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forwardAmount;

		if (playerOnGround) {
			animator.SetFloat ("JumpLeg", jumpLeg);
		} else {
			if (zeroGravityModeOn || freeFloatingModeOn) {
				if (currentJumpLeg == 0) {
					currentJumpLeg = jumpLeg;
				}
				currentJumpLeg = Mathf.Lerp (currentJumpLeg, 1 * Mathf.Sign (currentJumpLeg), currentFixedUpdateDeltaTime / 2);
				animator.SetFloat ("JumpLeg", currentJumpLeg);
			}

			if (gravityPowerActive) {
				animator.SetFloat ("JumpLeg", currentJumpLeg, 0.1f, currentFixedUpdateDeltaTime / 2);
			}
		}

		if (playerIsAiming && !usingFreeFireMode && !checkToKeepWeaponAfterAimingWeaponFromShooting) {
			animator.SetBool ("Aiming Mode Active", true);
		} else {
			animator.SetBool ("Aiming Mode Active", false);
		}

		//if the player is on ground and moving set the speed of his animator to the properly value
		if (playerOnGround && moveInput.magnitude > 0) {
			animator.speed = animSpeedMultiplier;
		} else {
			animator.speed = 1;
		}

		if (regularMovementOnBulletTime) {
			animator.speed *= currentFixedUpdateDeltaTime;
		}
	}

	public void overrideOnGroundAnimatorValue (float newOverrideOnGrounDuration)
	{
		overrideOnGrounDuration = newOverrideOnGrounDuration;
		overrideOnGroundAnimatorValueActive = true;
		lastTimeOverrideOnGroundAnimator = Time.time;
	}

	//update the velocity of the player rigidbody
	public void OnAnimatorMove ()
	{
		if (!gravityPowerActive && usingAnimator) {
			mainRigidbody.rotation = animator.rootRotation;
			if ((playerOnGround || actionActive) && Time.deltaTime > 0) {
				characterVelocity = Vector3.zero;

				if (useRootMotionActive || actionActive) {
					characterVelocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
				} else {
					currentNoRootMovementSpeed = 1;

					if (running && !crouching) {
						if (strafeModeActive) {
							currentNoRootMovementSpeed = noRootRunStrafeMovementSpeed;
						} else {
							currentNoRootMovementSpeed = noRootSprintMovementSpeed;
						}
					} else {
						if (crouching) {
							currentNoRootMovementSpeed = noRootCrouchMovementSpeed;
						} else {
							if (strafeModeActive) {
								currentNoRootMovementSpeed = noRootWalkStrafeMovementSpeed;
							} else {
								if (walkSpeed >= 0.5f) {
									if (forwardAmount < 0.5f) {
										currentNoRootMovementSpeed = noRootWalkMovementSpeed / walkSpeed;
									} else {
										currentNoRootMovementSpeed = noRootRunMovementSpeed * walkSpeed;
									}
								} else {
									currentNoRootMovementSpeed = noRootWalkMovementSpeed;
								}
							}
						}
					}

					if (!strafeModeActive) {
						currentNoRootMovementSpeed -= currentMovementSpeed * (Mathf.Abs (turnAmount));
					}

					if (Time.time < lastTimeGround + 0.2f) {
						currentNoRootMovementSpeed = 0; 
					}
						
					lerpNoRootMovementSpeed = Mathf.Lerp (lerpNoRootMovementSpeed, currentNoRootMovementSpeed, Time.deltaTime * noRootRunMovementSpeed);

					if (lockedCameraActive && useRelativeMovementToLockedCamera) {

						characterVelocity = currentLockedInput * lerpNoRootMovementSpeed;

						characterVelocity = Vector3.ClampMagnitude (characterVelocity, currentNoRootMovementSpeed);
					} else {
						characterVelocity = moveInput * lerpNoRootMovementSpeed;
					}
				}

				mainRigidbody.velocity = characterVelocity;

				if (actionActive) {

					if (mainRigidbody.velocity.y < 0) {
						mainRigidbody.velocity = new Vector3 (mainRigidbody.velocity.x, 0, mainRigidbody.velocity.z);
					}
				}

				currentVelocityChangeMagnitude = mainRigidbody.velocity.magnitude;
			}
		}
	}

	public void setUseRootMotionActiveState (bool state)
	{
		useRootMotionActive = state;
	}

	public void setOriginalUseRootMotionActiveState ()
	{
		setUseRootMotionActiveState (originalSetUseRootMotionActiveState);
	}

	public void setPlayerControllerMovementValues (float moveSpeedMultiplierValue, float animSpeedMultiplierValue, float jumpPowerValue, float airSpeedValue, float airControlValue)
	{
		moveSpeedMultiplier = moveSpeedMultiplierValue;
		animSpeedMultiplier = animSpeedMultiplierValue;
		jumpPower = jumpPowerValue;
		airSpeed = airSpeedValue;
		airControl = airControlValue;

		noAnimatorAirSpeed = airSpeedValue;
	}

	public void resetPlayerControllerMovementValues ()
	{
		moveSpeedMultiplier = originalMoveSpeedMultiplier;
		animSpeedMultiplier = originalAnimationSpeed;
		jumpPower = originalJumpPower;
		airSpeed = originalAirSpeed;
		airControl = originalAirControl;

		noAnimatorAirSpeed = originalNoAnimatorAirSpeed;
	}

	public GameObject getCurrentSurfaceBelowPlayer ()
	{
		return currentSurfaceBelowPlayer;
	}

	//check if the player is in the ground with a raycast
	void checkOnGround ()
	{
		if (checkOnGroungPaused) {
			currentSurfaceBelowPlayer = null;
			return;
		}

		if (!gravityPowerActive && !usingJetpack && !flyModeActive) {
			if (transform.InverseTransformDirection (mainRigidbody.velocity).y < jumpPower * .5f) {
				previouslyOnGround = playerOnGround;

				setOnGroundState (false);

				if (jump || slowingFall) {
					if (!gravityForcePaused && !zeroGravityModeOn && !freeFloatingModeOn) {
						mainRigidbody.AddForce (gravityForce * mainRigidbody.mass * currentNormal);
					}
				}

				currentPlayerPosition = transform.position;
				currentPlayerUp = transform.up;

				//check what it is under the player
				rayPos = currentPlayerPosition + currentPlayerUp;
				hitAngle = 0;
				hitPoint = Vector3.zero;

				if (Physics.Raycast (rayPos, -currentPlayerUp, out hit, rayDistance, layer)) {
					//get the angle of the current surface
					hitAngle = Vector3.Angle (currentNormal, hit.normal);

					canSetGroundState = false;
					//check max angle between the ground and the player if he is on zero gravity or free floating mode
					if ((freeFloatingModeOn || zeroGravityModeOn) && !checkOnGroundStatePausedFFOrZG && !previouslyOnGround) {
						if (useMaxAngleToCheckOnGroundStateZGFF) {
							if (hitAngle < maxAngleToChekOnGroundStateZGFF) {
								canSetGroundState = true;
							}
						} else {
							canSetGroundState = true;
						}
					} else {
						canSetGroundState = true;
					}

					if (canSetGroundState) {
						setOnGroundState (true);
					}
				
					jump = false;
					slowingFall = false;
					hitPoint = hit.point;
					currentSurfaceBelowPlayer = hit.collider.gameObject;
					currentNumberOfDash = 0;
				} else {
					currentSurfaceBelowPlayer = null;
				}

				if (useMaxWalkSurfaceAngle && !stairStepDetected) {
					if (playerOnGround && hitAngle > maxWalkSurfaceAngle) {
						setOnGroundState (false);
					}
				}
					
				//check if the player has to adhere to the surface or not
				bool adhereToGround = false;
				if (playerOnGround) {
					//if the player is moving
					if (isMoving) {
						//use a raycast to check the distance to the ground in front of the player, 
						//if the ray doesn't find a collider, it means that the player is going down in an inclinated surface, so adhere to that surface
						bool hitInFront = false;
						slopeFound = false;
						stairsFound = false;

						//assign the current input direction, taking into account if the player is moving in free mode or in locked camera with movement relative to the camera direction
						if (lockedCameraActive && useRelativeMovementToLockedCamera) {
							currentAdherenceInput = currentLockedInput;
						} else {
							currentAdherenceInput = moveInput;
						}
							
						//rayPosition = rayPos + verticalInput * (transform.forward * 0.5f) + horizontalInput * (transform.right * 0.5f);
						rayPosition = rayPos + currentAdherenceInput * maxRayDistanceRange;

						Debug.DrawRay (rayPosition - currentPlayerUp, currentPlayerUp * 10, Color.yellow);

						raycastDistance = rayDistance - 0.2f;

						if (Physics.Raycast (rayPosition, -currentPlayerUp, out hit, raycastDistance, layer)) {
							Debug.DrawRay (rayPosition, -currentPlayerUp * raycastDistance, Color.red);
							hitInFront = true;
						}
							
						//check for slopes while aiming in third person or moving in first person
						if (hitAngle != 0 &&
						    ((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirectionActive || firstPersonActive) &&
						    !playerCameraManager.is2_5ViewActive ()) {

							rayPosition = rayPos + currentAdherenceInput * maxSlopeRayDistance;
							if (Physics.Raycast (rayPosition, -currentPlayerUp, out hit, (rayDistance + 0.5f), layer)) {

								playerVerticalPosition = Mathf.Abs (transform.InverseTransformPoint (currentPlayerPosition).y);
								hitPointVerticalPosition = Mathf.Abs (transform.InverseTransformPoint (hit.point).y);

								upDistance = Mathf.Abs (playerVerticalPosition - hitPointVerticalPosition);

								if (upDistance > 0) {
									//print ("slope");
									adhereToGround = true;
									hitPoint = hit.point;
									if (usingAnimator) {
										currentGroundAdherence = slopesGroundAdherence;
									} else {
										currentGroundAdherence = noAnimatorSlopesGroundAdherence;
									}

									slopeFound = true;
								}
								Debug.DrawRay (rayPosition, -currentPlayerUp * (rayDistance + 0.5f), Color.green);
							} else {
								Debug.DrawRay (rayPosition, -currentPlayerUp * (rayDistance + 0.5f), Color.yellow);
							}
						}

						//check for stairs below and in front of the player
						stairsMoveInput = currentAdherenceInput * (1 / walkSpeed);

						rayPosition = rayPos + stairsMoveInput;
						Debug.DrawRay (rayPosition, -currentPlayerUp * raycastDistance, Color.yellow);

						if (Physics.Raycast (rayPosition, -currentPlayerUp, out hit, raycastDistance, layer)) {
					
							hitAngle = Vector3.Angle (currentNormal, hit.normal);  

							//print (hitAngle);

							if (checkStairsWithInclination) {
								if (!stairStepDetected) {
									stairInclinationDetected = false;
								}
									
								if (hitAngle >= minStairInclination && hitAngle <= maxStairInclination) {
									stairInclinationDetected = true;
								}
							}
						}

						//walk on stairs
						if (hitInFront && (hitAngle == 0 || stairInclinationDetected)) {

							if (checkForStairAdherenceSystem) {
								rayPosition = rayPos + stairsMoveInput;

								Debug.DrawRay (rayPosition, -currentPlayerUp * raycastDistance, Color.black);

								if (Physics.Raycast (rayPosition, -currentPlayerUp, out hit, raycastDistance, layer)) {
									currentDetectedSurface = hit.collider.gameObject;
								} else {
									currentDetectedSurface = null;
								}

								if (currentTargetDetected != currentDetectedSurface) {

									stairAdherenceSystemDetected = false;

									currentTargetDetected = currentDetectedSurface;
									if (currentTargetDetected) {
										stairAdherenceSystem currentStairAdherenceSystem = currentTargetDetected.GetComponent<stairAdherenceSystem> ();

										if (currentStairAdherenceSystem) {
											if (currentStairAdherenceSystem.modifyStairsValuesOnPlayer) {
												currentStairAdherenceSystem.setStairsValuesOnPlayer (this);
											}
											stairAdherenceSystemDetected = true;
										} else {
											currentStairAdherenceSystem = currentSurfaceBelowPlayer.GetComponent<stairAdherenceSystem> ();
											if (currentStairAdherenceSystem) {
												if (currentStairAdherenceSystem.modifyStairsValuesOnPlayer) {
													currentStairAdherenceSystem.setStairsValuesOnPlayer (this);
												}
												stairAdherenceSystemDetected = true;
											}
										}
									}
								}
							}

							if (stairAdherenceSystemDetected) {
								currentStairMaxValue = currentStairAdherenceSystemMaxValue;
								currentStairMinValue = currentStairAdherenceSystemMinValue;
								currentStairAdherenceValue = currentStairAdherenceSystemAdherenceValue;
							} else {
								currentStairMaxValue = stairsMaxValue;
								currentStairMinValue = stairsMinValue;
								currentStairAdherenceValue = stairsGroundAdherence;
							}

							rayPosition = rayPos + stairsMoveInput * maxStairsRayDistance;

							Vector3 clampRayPosition = rayPosition - currentPlayerPosition;

							clampRayPosition = Vector3.ClampMagnitude (clampRayPosition, 1.1f);

							clampRayPosition = currentPlayerPosition + clampRayPosition;

							if (Physics.Raycast (clampRayPosition, -currentPlayerUp, out hit, raycastDistance, layer)) {
								if (!stairStepDetected) {
									float stepPosition = transform.InverseTransformPoint (hit.point).y;

									if (stepPosition > 0) {
										upDistance = Mathf.Abs (Mathf.Abs (transform.InverseTransformPoint (currentPlayerPosition).y) - Mathf.Abs (stepPosition));

										if (upDistance >= currentStairMinValue && upDistance <= currentStairMaxValue) {
											stairStepDetected = true;	
											//print ("stair step detected");
										}
									}
								}

								if (stairStepDetected) {
									//print ("climbing step");

									adhereToGround = true;
									hitPoint = hit.point;
									Debug.DrawLine (currentPlayerPosition + currentPlayerUp * 2, hitPoint, Color.black, 2);

									if (usingAnimator) {
										currentGroundAdherence = currentStairAdherenceValue;
										if (walkSpeed < 1) {
											float extraValue = walkSpeed;
											extraValue += 0.3f;
											extraValue = Mathf.Clamp (extraValue, 0, 1);
											currentGroundAdherence *= extraValue;
										}
									} else {
										currentGroundAdherence = noAnimatorStairsGroundAdherence;
									}

									stairsFound = true;
										
									//print (transform.InverseTransformPoint (hit.point).y);

									if (Mathf.Abs (transform.InverseTransformPoint (hit.point).y) < 0.02f) {
										stairStepDetected = false;

										//print ("new step reached");
									}
								} 
							} else {
								stairStepDetected = false;
							}
						}

						if (!slopeFound && !stairsFound) {
							adhereToGround = true;
							currentGroundAdherence = regularGroundAdherence;
						}
					}

					if (useAutoCrouch) {
						if (!crouching) {
							if (!Physics.Raycast (autoCrouchRayPosition.position, transform.forward, out hit, raycastDistanceToAutoCrouch, layerToCrouch)) {
								Debug.DrawRay (autoCrouchRayPosition.position, transform.forward * raycastDistanceToAutoCrouch, Color.white);
								if (Physics.Raycast (autoCrouchRayPosition.position + secondRaycastOffset, transform.forward, out hit, raycastDistanceToAutoCrouch, layerToCrouch)) {
									Debug.DrawRay (autoCrouchRayPosition.position + secondRaycastOffset, transform.forward * raycastDistanceToAutoCrouch, Color.red);
									crouch ();
								} 
							} 
						}
					}

					//if the player is not moving and the angle of the surface is 0, adhere to it, so if the player jumps for example, the player is correctly
					//placed in the ground, with out a gap between the player's collider and the ground
					if (!isMoving && hitAngle == 0) {
						adhereToGround = true;
						currentGroundAdherence = regularGroundAdherence;
					}
				}

				//the player has to ahdere to the current surface, so
				if (adhereToGround) {
					//move towards the surface the player's rigidbody 
					//print(currentGroundAdherence + " " +hitAngle);
					mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, hitPoint, currentFixedUpdateDeltaTime * currentGroundAdherence);
				}
			}

			if (!playerOnGround) {
				lastTimeAir = Time.time;
			}
		} else {
			setOnGroundState (false);
		}
	}

	public bool isPlayerOnGround ()
	{
		return playerOnGround;
	}

	public void setPlayerOnGroundState (bool state)
	{
		playerOnGround = state;
	}

	public void setOnGroundState (bool state)
	{
		playerOnGround = state;

//		if (!playerOnGround) {
//			print ("AIRE");
//		} else {
//			print ("NO AIRE");
//		}

		if (checkOnGroundStatePaused || checkOnGroundStatePausedFFOrZG) {
			playerOnGround = false;
		}
	}

	public void setAirDashEnabledState (bool state)
	{
		airDashEnabled = state;
	}

	public void doAirDash ()
	{
		if (characterRotatingToSurface) {
			return;
		}

		if (useDashLimit) {
			if (currentNumberOfDash >= dashLimit) {
				return;
			}
		}

		if (airDashEnabled && Time.time > lastTimeDash + airDashColdDown) {
			
			Vector3 dashDirection = moveInput;
			if (dashDirection == Vector3.zero || dashDirection.magnitude < 0.1f) {
				if (firstPersonActive) {
					dashDirection = playerCameraGameObject.transform.forward;
				} else {
					dashDirection = transform.forward;
				}
			}

			if (resetGravityForceOnDash) {
				mainRigidbody.velocity = Vector3.zero;
			}

			mainRigidbody.AddForce (dashDirection * mainRigidbody.mass * airDashForce, ForceMode.Impulse);
			lastTimeDash = Time.time;
			if (pauseGravityForce) {
				gravityForcePaused = true;
				lastTimeGravityForcePaused = Time.time;
				unpauseGravityForceActive = true;
			}

			if (useDashLimit) {
				currentNumberOfDash++;
			}

			if (changeCameraFovOnDash) {
				playerCameraManager.setMainCameraFovStartAndEnd (cameraFovOnDash, playerCameraManager.getOriginalCameraFov (), cameraFovOnDashSpeed);
			}
		}
	}

	public void addExternalForce (Vector3 forceDireciton)
	{
		mainRigidbody.AddForce (forceDireciton * mainRigidbody.mass, ForceMode.Impulse);
	}

	public void setGravityForcePuase (bool state)
	{
		gravityForcePaused = state;
	}

	public void setRigidbodyVelocityToZero ()
	{
		mainRigidbody.velocity = Vector3.zero;
	}

	public float getCurrentDeltaTime ()
	{
		currentDeltaTime = Time.deltaTime;

		if (regularMovementOnBulletTime) {
			currentDeltaTime *= GKC_Utils.getCurrentDeltaTime ();
		}
		return currentDeltaTime;
	}

	public float getCurrentScaleTime ()
	{
		return GKC_Utils.getCurrentDeltaTime ();
	}

	//set the scale of the capsule if the player is crouched
	void scaleCapsule (bool state)
	{
		if (state) {
			capsule.height = capsuleHeightOnCrouch;
			capsule.center = Vector3.up * capsuleHeightOnCrouch / 2;
		} else {
			capsule.height = originalHeight;
			capsule.center = Vector3.up * originalHeight * 0.5f;
		}
	}

	public Collider getMainCollider ()
	{
		return mainCollider;
	}

	public void setMainColliderState (bool state)
	{
		mainCollider.enabled = state;
	}

	public void setGravityMultiplierValue (bool setOriginal, float newValue)
	{
		if (setOriginal) {
			currentGravityMultiplier = originalGravityMultiplier;
			gravityMultiplier = originalGravityMultiplier;
		} else {
			currentGravityMultiplier = newValue;
			gravityMultiplier = newValue;
		}
	}

	public void setGravityForceValue (bool setOriginal, float newValue)
	{
		if (setOriginal) {
			gravityForce = originalGravityForce;
		} else {
			gravityForce = newValue;
		}
	}

	public float getGravityForce ()
	{
		return gravityForce;
	}

	//CALL INPUT FUNCTIONS
	public bool canUseInput ()
	{
		return (!jetPackEquiped && !flyModeActive && canMove && !usedByAI && !isTankModeActive () && !playerNavMeshEnabled && !driving);
	}

	public void inputCrouch ()
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		//check if the crouch button has been pressed
		crouch ();
	}

	public void inputJump ()
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (!canJumpActive) {
			return;
		}
			
		//check if the jump button has been pressed
		if (enabledRegularJump) {
			jumpInput = true;
		}

		if (!playerOnGround && !gravityPowerActive) {
			if (enabledDoubleJump && !zeroGravityModeOn && !freeFloatingModeOn) {
				//then check the last time the jump button has been pressed, so the player can jump again  
				//jump again
				if (Time.time > lastJumpTime + readyToDoubleJumpTime && jumpsAmount < maxNumberJumpsInAir) {
					doubleJump = true;
				}
			}
		}
	}

	public void inputStartSlowDownFall ()
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (holdJumpSlowDownFall) {
			if (Time.time > waitTimeToSlowDown) {
				gravityMultiplier = slowDownGravityMultiplier;
				if (!slowingFall) {
					slowingFallInput = true;
				}
			}
		}
	}

	public void inputStopSlowDownFall ()
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (holdJumpSlowDownFall) {
			gravityMultiplier = currentGravityMultiplier;
		}
	}

	public void inputAirDash ()
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (airDashEnabled && !gravityPowerActive && !playerOnGround && !gravityManager.isSearchingSurface () && !powersManager.getteleportCanBeExecutedState ()) {
			doAirDash ();
		}
	}

	public void inputSetMoveVerticallyState (bool state)
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (zeroGravityModeOn || freeFloatingModeOn) {
			if (canMoveVertically && lockedPlayerMovement == lockedPlayerMovementMode.world3d) {
				movingVertically = state;
			}
		}
	}

	public void inputSetChangeZeroGravityMovementSpeeState (bool state)
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (zeroGravityModeOn) {
			changeZeroGravityMovementVelocity (state);
		}
	}

	public void inputSetChangeFreeFloatingMovementSpeeState (bool state)
	{
		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (freeFloatingModeOn) {
			changeFreeFloatingMovementVelocity (state);
		}
	}

	public void inputSetIncreaseMovementSpeedState (bool state)
	{
		if (jetPackEquiped || flyModeActive || !canMove || usedByAI || playerNavMeshEnabled || driving) {
			return;
		}

		if (!enabled) {
			return;
		}

		if (increaseWalkSpeedEnabled) {
			if (holdButtonToKeepIncreasedWalkSpeed) {
				setIncreaseMovementSpeedState (state);
			} else {
				if (state) {
					increaseWalkSpeedActive = !increaseWalkSpeedActive;
					setIncreaseMovementSpeedState (increaseWalkSpeedActive);
				}
			}
		}
	}

	//check with a sphere cast if the there are any surface too close
	public void crouch ()
	{
		if (!gravityPowerActive && checkWeaponsState () && playerOnGround) {
			crouching = !crouching;
			if (!crouching) {
				//check if there is anything above the player when he is crouched, to prevent he stand up
				//stop the player to get up
				if (!playerCanGetUpFromCrouch ()) {
					crouching = true;
				}
			}

			//set the pivot position
			playerCameraManager.crouch (crouching);

			healthManager.changePlaceToShootPosition (crouching);

			scaleCapsule (crouching);

			setLastTimeMoved ();
		}
	}

	public bool playerCanGetUpFromCrouch ()
	{
		Ray crouchRay = new Ray (mainRigidbody.position + transform.up * originalRadius * 0.5f, transform.up);
		float crouchRayLength = originalHeight - originalRadius * 0.5f;
		if (Physics.SphereCast (crouchRay, originalRadius * 0.5f, crouchRayLength, layer)) {
			return false;
		}

		return true;
	}

	public bool isCrouching ()
	{
		return crouching;
	}

	public void setGravityPowerActiveState (bool state)
	{
		gravityPowerActive = state;
	}

	public bool isGravityPowerActive ()
	{
		return gravityPowerActive;
	}

	public void setRunnningState (bool state)
	{
		running = state;

		if (useSecondarySprintValues) {
			if (running) {
				setPlayerControllerMovementValues (sprintVelocity, sprintVelocity, sprintJumpPower, sprintAirSpeed, sprintAirControl);
			} else {
				resetPlayerControllerMovementValues ();
			}
		}

		if (!lockedCameraActive) {
			if (sprintEnabled && changeCameraFovOnSprint && !isPlayerOnFirstPerson ()) {
				changeSprintFovThirdPerson (running);

				if (shakeCameraOnSprintThirdPerson) {
					setHeadbodStatesPaused = running;

					playerCameraManager.setShakeCameraState (running, sprintThirdPersonCameraShakeName);
				}
			}
		}
	}

	public bool isPlayerRunning ()
	{
		return running;
	}

	public void setSprintEnabledState (bool state)
	{
		sprintEnabled = state;
	}

	public bool isSprintEnabled ()
	{
		return sprintEnabled;
	}

	public void enableOrDisableSphereMode (bool state)
	{
		if (canUseSphereMode) {
			sphereModeActive = state;
		}
	}

	public void setLastTimeMoved ()
	{
		lastTimeMoved = Time.time;
	}

	public void setLastTimeFalling ()
	{
		lastTimeFalling = Time.time;
	}

	public bool checkWeaponsState ()
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			return true;
		} else if (!powersManager.isAimingPower () && !weaponsManager.isUsingWeapons () && !IKSystemManager.usingWeapons) {
			return true;
		} else if (crouching) {
			return true;
		}
		return false;
	}

	public void setUsingFreeFireModeState (bool state)
	{
		usingFreeFireMode = state;
	}

	//set if the animator is enabled or not according to if the usingAnimatorInFirstMode is true or false
	public void checkAnimatorIsEnabled (bool state)
	{
		//if the animator in first person is disabled, then
		if (!usingAnimatorInFirstMode) {
			//the first person mode is enabled, so disable the animator
			if (state) {
				animator.enabled = false;
			}
			//the third person mode is enabled, so enable the animator
			else {
				animator.enabled = true;
			}
		}

		//if the animator is enabled, 
		if (animator.enabled) {
			//check the state of the animator to set the values in the mecanim
			usingAnimator = true;
		} else {
			//disable the functions that set the values in the mecanim, and apply force to the player rigidbody directly, instead of using the animation motion
			usingAnimator = false;
		}

		//change the type of footsteps, with the triggers in the feet of the player or the raycast checking the surface under the player
		stepManager.changeFootStespType (usingAnimator);
	}

	public void setFootStepManagerState (bool state)
	{
		if (state) {
			stepManager.enableOrDisableFootStepsComponents (false);
		} else {
			stepManager.enableOrDisableFootStepsWithDelay (true, 0);
		}
	}
	
	//if the vehicle driven by the player ejects him, add force to his rigidbody
	public void ejectPlayerFromVehicle (float force)
	{
		jumpInput = true;
		mainRigidbody.AddForce (currentNormal * force, ForceMode.Impulse);
	}

	//use a jump platform
	public void useJumpPlatform (Vector3 direction, ForceMode forceMode)
	{
		jumpInput = true;
		if (forceMode == ForceMode.VelocityChange) {
			mainRigidbody.velocity = Vector3.zero;
			currentVelocity = Vector3.zero;
		}
		mainRigidbody.AddForce (direction, forceMode);
	}

	public void useJumpPlatformWithKeyButton (bool state, float newJumpPower)
	{
		if (state) {
			jumpPower = newJumpPower;
		} else {
			jumpPower = originalJumpPower;
		}
	}

	public void externalForce (Vector3 direction)
	{
		externalForceActive = true;
		externalForceValue = direction;
	}

	public void changeHeadScale (bool state)
	{
		if (drivingRemotely) {
			return;
		}

		if (state) {
			head.localScale = Vector3.zero;
		} else {
			head.localScale = Vector3.one;
		}
	}

	//if the player is driving, set to 0 his movement values and disable the player controller component
	public void setDrivingState (bool state, GameObject vehicle)
	{
		driving = state;
		if (driving) {
			currentVehicle = vehicle;
		} else {
			currentVehicle = null;
		}

		if (usingAnimator) {
			animator.SetFloat ("Forward", 0);
			animator.SetFloat ("Turn", 0);
		}

		playerCameraManager.setDrivingState (driving);
	}

	public bool isPlayerRotatingToSurface ()
	{
		return characterRotatingToSurface;
	}

	public bool isPlayerDriving ()
	{
		return driving;
	}

	public bool isDrivingRemotely ()
	{
		return drivingRemotely;
	}

	public void setDrivingRemotelyState (bool state)
	{
		drivingRemotely = state;
	}

	public bool isOverridingElement ()
	{
		return overridingElement;
	}

	public void setOverridingElementState (bool state)
	{
		overridingElement = state;
	}

	public void setUsingDeviceState (bool state)
	{
		usingDevice = state;
		setLastTimeMoved ();
	}

	public bool isUsingDevice ()
	{
		return usingDevice;
	}

	public void setHeadBobPausedState (bool state)
	{
		headBobPaused = state;
	}

	public GameObject getCurrentVehicle ()
	{
		return currentVehicle;
	}

	//if it is neccessary, stop any movement from the keyboard or the touch controls in the player controller
	public void changeScriptState (bool state)
	{
		if (usingAnimator) {
			animator.SetFloat ("Forward", 0);
			animator.SetFloat ("Turn", 0);
		}

		isMoving = false;
		canMove = state;

		jump = false;

		jumpInput = false;

		doubleJump = false;

		slowingFall = false;

		slowingFallInput = false;

		setHeadbodStatesPaused = false;

		gravityMultiplier = currentGravityMultiplier;

		resetPlayerControllerInput ();
	}

	public void setPlayerVelocityToZero ()
	{
		mainRigidbody.velocity = Vector3.zero;
		currentVelocity = Vector3.zero;
		velocityChange = Vector3.zero;
	}

	public void smoothChangeScriptState (bool state)
	{
		canMove = state;
		if (!state) {
			smoothResetPlayerControllerInput ();
		} else {
			if (resetInputCoroutine != null) {
				StopCoroutine (resetInputCoroutine);
			}
		}
	}

	public bool canPlayerMove ()
	{
		return canMove;
	}

	public void smoothResetPlayerControllerInput ()
	{
		if (resetInputCoroutine != null) {
			StopCoroutine (resetInputCoroutine);
		}
		resetInputCoroutine = StartCoroutine (smoothResetPlayerControllerInputCoroutine ());
	}

	IEnumerator smoothResetPlayerControllerInputCoroutine ()
	{
		float currentTime = 0;
		while (verticalInput != 0 || horizontalInput != 0) {
			currentTime = getCurrentDeltaTime ();
			verticalInput = Mathf.MoveTowards (verticalInput, 0, currentTime * 2);
			horizontalInput = Mathf.MoveTowards (horizontalInput, 0, currentTime * 2);
			yield return null;
		}
	}

	public void enableOrDisablePlayerControllerScript (bool state)
	{
		enabled = state;
		if (!enabled) {
			animator.SetFloat ("Forward", 0);
			animator.SetFloat ("Turn", 0);
			animator.SetFloat ("Jump", 0);
			animator.SetFloat ("JumpLeg", 0);
			animator.SetFloat ("Horizontal", 0);
			animator.SetFloat ("Vertical", 0);
			animator.SetBool ("Moving", false);
		}
	}

	public void setGamePausedState (bool state)
	{
		gamePaused = state;
		setLastTimeMoved ();
	}

	public void equipJetpack (bool state, float force, float newAirControl, float newAirSpeed)
	{
		jetPackEquiped = state;
		jetpackForce = force;
		jetpackAirControl = newAirControl;
		jetpackAirSpeed = newAirSpeed;
	}

	public void enableOrDisableFlyingMode (bool state, float force, float newAirControl, float newAirSpeed, float turboSpeed)
	{
		flyModeActive = state;
		flyModeForce = force;
		flyModeAirControl = newAirControl;
		flyModeAirSpeed = newAirSpeed;
		flyModeTurboSpeed = turboSpeed;
	}

	public void enableOrDisableFlyModeTurbo (bool state)
	{
		flyModeTurboActive = state;
		playerCameraManager.changeCameraFov (flyModeTurboActive);
		//when the player accelerates his movement in the air, the camera shakes
		if (flyModeTurboActive) {
			playerCameraManager.shakeCamera ();
			playerCameraManager.accelerateShake (true);
		} else {
			playerCameraManager.accelerateShake (false);
			playerCameraManager.stopShakeCamera ();
		}
	}

	public void enableOrDisableAiminig (bool state)
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			enableOrDisableAimingInFirstPerson (state);
			enableOrDisableAimingInThirdPerson (false);
		} else {
			enableOrDisableAimingInThirdPerson (state);
			enableOrDisableAimingInFirstPerson (false);
		}

		if (!state) {
			lastTimePlayerUserInput = 0;
		}
	}

	public void enableOrDisableAimingInFirstPerson (bool state)
	{
		aimingInFirstPerson = state;
	}

	public void enableOrDisableAimingInThirdPerson (bool state)
	{
		aimingInThirdPerson = state;
	}

	public bool isPlayerAiming ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool isPlayerAimingInThirdPerson ()
	{
		return aimingInThirdPerson;
	}

	public bool isPlayerAimingInFirstPerson ()
	{
		return aimingInFirstPerson;
	}

	public bool isLookingInCameraDirection ()
	{
		return ((isPlayerAiming () && hasToLookInCameraDirectionOnFreeFire ()) || lookInCameraDirectionActive || firstPersonActive);
	}

	public playerCamera getPlayerCameraManager ()
	{
		return playerCameraManager;
	}

	public GameObject getPlayerCameraGameObject ()
	{
		return playerCameraGameObject;
	}

	public bool isPlayerOnFirstPerson ()
	{
		return firstPersonActive;
	}

	public void setFirstPersonViewActiveState (bool state)
	{
		firstPersonActive = state;
	}

	public bool isPlayerMoving (float movingTolerance)
	{
		if (Mathf.Abs (horizontalInput) > movingTolerance || Mathf.Abs (verticalInput) > movingTolerance) {
			return true;
		}
		return false;
	}

	public bool isPlayerMovingHorizontal (float movingTolerance)
	{
		if (Mathf.Abs (horizontalInput) > movingTolerance) {
			return true;
		}
		return false;
	}

	public bool isPlayerMovingVertical (float movingTolerance)
	{
		if (Mathf.Abs (verticalInput) > movingTolerance) {
			return true;
		}
		return false;
	}

	public bool isPlayerUsingInput ()
	{
		if (rawAxisValues.x != 0 || rawAxisValues.y != 0) {
			return true;
		}
		return false;
	}

	public bool isPlayerUsingVerticalInput ()
	{
		if (rawAxisValues.y != 0) {
			return true;
		}
		return false;
	}

	public bool isPlayerUsingHorizontalInput ()
	{
		if (rawAxisValues.x != 0) {
			return true;
		}
		return false;
	}

	public Vector2 getAxisValues ()
	{
		return axisValues;
	}

	public Vector2 getRawAxisValues ()
	{
		return rawAxisValues;
	}

	public void resetPlayerControllerInput ()
	{
		horizontalInput = 0;
		verticalInput = 0;
	}

	bool overrideMainCameraTransformActive;
	Transform overrideMainCameraTransform;

	public void overrideMainCameraTransformDirection (Transform newCameraDirection, bool overrideState)
	{
		overrideMainCameraTransform = newCameraDirection;
		overrideMainCameraTransformActive = overrideState;
	}

	public void setLockedCameraState (bool state, bool useTankControlsValue, bool useRelativeMovementToLockedCameraValue)
	{
		lockedCameraActive = state;

		useTankControls = useTankControlsValue;
	
		tankModeCurrentlyEnabled = lockedCameraActive && useTankControls;

		useRelativeMovementToLockedCamera = useRelativeMovementToLockedCameraValue;

		if (!lockedCameraActive) {
			checkCameraDirectionFromLockedToFree = true;
		}
	}

	public bool isLockedCameraStateActive ()
	{
		return lockedCameraActive;
	}

	public bool isTankModeActive ()
	{
		return useTankControls && lockedCameraActive;
	}

	public bool canCharacterGetOnVehicles ()
	{
		return canGetOnVehicles;
	}

	public bool canCharacterDrive ()
	{
		return canDrive;
	}

	public GameObject getPlayerManagersParentGameObject ()
	{
		return playerManagersParentGameObject;
	}

	public void setPlayerAndCameraParent (Transform newParent)
	{
		playerCameraManager.setPlayerAndCameraParent (newParent);

		if (newParent != null) {
			setPlayerSetAsChildOfParentState (true);
		} else {
			setPlayerSetAsChildOfParentState (false);
		}
	}

	//AI input and functions to use this controller with an AI system
	public void setReducedVelocity (float newValue)
	{
		animSpeedMultiplier = newValue;
	}

	public void setNormalVelocity ()
	{
		animSpeedMultiplier = originalAnimationSpeed;
	}

	public void changeControlInputType (bool value)
	{
		usedByAI = value;
		if (usedByAI) {
			resetPlayerControllerInput ();
		}
	}

	public void startOverride ()
	{
		overrideCharacterControlState (true);
	}

	public void stopOverride ()
	{
		overrideCharacterControlState (false);
	}

	public void overrideCharacterControlState (bool state)
	{
		usedByAI = !state;
		characterControlOverrideActive = state;
	}

	public bool isCharacterUsedByAI ()
	{
		return usedByAI;
	}

	public bool isCharacterControlOverrideActive ()
	{
		return characterControlOverrideActive;
	}

	public float getHorizontalInput ()
	{
		return horizontalInput;
	}

	public float getVerticalInput ()
	{
		return verticalInput;
	}

	public Vector3 getMoveInputDirection ()
	{
		return moveInput;
	}

	public void setLookInCameraDirectionState (bool state)
	{
		lookInCameraDirection = state;
	}

	// The Move function is designed to be called from a separate component
	// based on User input, or an AI control script
	public void Move (AINavMeshMoveInfo inputInfo)
	{
		if (inputInfo.moveInput.magnitude > 1) {
			inputInfo.moveInput.Normalize ();
		}
		navMeshMoveInput = inputInfo.moveInput;
//		crouchInput = inputInfo.crouchInput;
		jumpInput = inputInfo.jumpInput;
		lookInCameraDirection = inputInfo.lookAtTarget;
	}

	public void setVisibleToAIState (bool state)
	{
		visibleToAI = state;
	}

	public bool isCharacterVisibleToAI ()
	{
		return visibleToAI;
	}

	public void setCharacterStateIcon (string stateName)
	{
		if (characterStateIconManager) {
			characterStateIconManager.setCharacterStateIcon (stateName);
		}
	}

	public void disableCharacterStateIcon ()
	{
		if (characterStateIconManager) {
			characterStateIconManager.disableCharacterStateIcon ();
		}
	}

	public int getPlayerID ()
	{
		return playerID;
	}

	public void setPlayerID (int newID)
	{
		playerID = newID;
	}

	public void setPlayerDeadState (bool state)
	{
		isDead = state;
	}

	public bool isPlayerDead ()
	{
		return isDead;
	}

	public void setPlayerNavMeshEnabledState (bool state)
	{
		playerNavMeshEnabled = state;
		resetPlayerControllerInput ();
	}

	public void enableOrDisableDoubleJump (bool state)
	{
		enabledDoubleJump = state;
	}

	public void setMaxNumberJumpsInAir (float amount)
	{
		maxNumberJumpsInAir = (int)amount;
	}

	public void enableOrDisableJump (bool state)
	{
		enabledRegularJump = state;
	}

	public void setcanJumpActiveState (bool state)
	{
		canJumpActive = state;
	}

	public void setPhysicMaterialAssigmentPausedState (bool state)
	{
		physicMaterialAssigmentPaused = state;
	}

	public void setHighFrictionMaterial ()
	{
		capsule.material = highFrictionMaterial;
	}

	public void setZeroFrictionMaterial ()
	{
		capsule.material = zeroFrictionMaterial;
	}

	public bool playerCanRunNow ()
	{
		return (!playerCameraManager.isFirstPersonActive () && canRunThirdPersonActive) ||
		(playerCameraManager.isFirstPersonActive () && (noAnimatorCanRun && (getVerticalInput () > 0 || noAnimatorCanRunBackwards)));
	}

	public void setNoAnimatorWalkMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorWalkMovementSpeed = originalNoAnimWalkMovementSpeed;
		} else {
			noAnimatorWalkMovementSpeed = originalNoAnimWalkMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorRunMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorRunMovementSpeed = originalNoAnimRunMovementSpeed;
		} else {
			noAnimatorRunMovementSpeed = originalNoAnimRunMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCrouchMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCrouchMovementSpeed = originalNoAnimCrouchMovementSpeed;
		} else {
			noAnimatorCrouchMovementSpeed = originalNoAnimCrouchMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorStrafeMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorStrafeMovementSpeed = originalNoAnimStrafeMovementSpeed;
		} else {
			noAnimatorStrafeMovementSpeed = originalNoAnimStrafeMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorWalkBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorWalkBackwardMovementSpeed = originalNoAnimWalkBackwardMovementSpeed;
		} else {
			noAnimatorWalkBackwardMovementSpeed = originalNoAnimWalkBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorRunBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorRunBackwardMovementSpeed = originalNoAnimRunBackwardMovementSpeed;
		} else {
			noAnimatorRunBackwardMovementSpeed = originalNoAnimRunBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCrouchBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCrouchBackwardMovementSpeed = originalNoAnimCrouchBackwardMovementSpeed;
		} else {
			noAnimatorCrouchBackwardMovementSpeed = originalNoAnimCrouchBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorStrafeBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorStrafeBackwardMovementSpeed = originalNoAnimStrafeBackwardMovementSpeed;
		} else {
			noAnimatorStrafeBackwardMovementSpeed = originalNoAnimStrafeBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCanRunState (bool newState, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCanRun = originalNoAnimCanRun;
		} else {
			noAnimatorCanRun = newState;
		}
	}

	public void setNoAnimatorGeneralMovementSpeed (float newValue, bool setOriginalValue)
	{
		setNoAnimatorWalkMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorRunMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorCrouchMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorStrafeMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorWalkBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorRunBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorCrouchBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorStrafeBackwardMovementSpeed (newValue, setOriginalValue);
	}


	public void setAnimatorCanRunState (bool newState, bool setOriginalValue)
	{
		if (setOriginalValue) {
			canRunThirdPersonActive = true;
		} else {
			canRunThirdPersonActive = newState;
		}
	}

	public void setAnimatorGeneralMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			setOriginalWalkSpeed ();
		} else {
			setWalkSpeedValue (newValue);
		}
	}

	public bool hasToLookInCameraDirection ()
	{
		//if the player has active the option to look in the camera direction, or to look on that direction when the camera is looking to a target, return true
		//also, check if the player only follows the camera direction while moving or not, and also, check if he is currently on ground state
		return (lookAlwaysInCameraDirection || (lookInCameraDirectionIfLookingAtTarget && playerCameraManager.isPlayerLookingAtTarget ()))
		&& lookInCameraDirectionOnFreeFireActive
		&& playerOnGround;
	}

	public bool hasToLookInCameraDirectionOnFreeFire ()
	{
		return (!usingFreeFireMode && !checkToKeepWeaponAfterAimingWeaponFromShooting) || lookInCameraDirectionOnFreeFire || useRelativeMovementToLockedCamera;
	}

	public bool checkToKeepWeaponAfterAimingWeaponFromShooting;

	float lastTimeCheckToKeepWeapon;

	public void setCheckToKeepWeaponAfterAimingWeaponFromShootingState (bool state)
	{
		checkToKeepWeaponAfterAimingWeaponFromShooting = state;

		if (!checkToKeepWeaponAfterAimingWeaponFromShooting) {
			lastTimeCheckToKeepWeapon = Time.time;
		}
	}

	public bool isCheckToKeepWeaponAfterAimingWeaponFromShooting ()
	{
		return checkToKeepWeaponAfterAimingWeaponFromShooting;
	}

	public void setLookAlwaysInCameraDirectionState (bool state)
	{
		lookAlwaysInCameraDirection = state;
	}

	public void setOriginalLookAlwaysInCameraDirectionState ()
	{
		setLookAlwaysInCameraDirectionState (originalLookAlwaysInCameraDirection);
	}

	public void setLookInCameraDirectionIfLookingAtTargetState (bool state)
	{
		lookInCameraDirectionIfLookingAtTarget = state;
	}

	public void setLookOnlyIfMovingState (bool state)
	{
		lookOnlyIfMoving = state;
	}

	public void setZeroGravityModeOnState (bool state)
	{
		zeroGravityModeOn = state;

		if (zeroGravityModeOn) {
			if (pushPlayerWhenZeroGravityModeIsEnabled) {
				useJumpPlatform (transform.up * pushZeroGravityEnabledAmount, ForceMode.Impulse);
			} 

			if (!playerOnGround) {
				setCheckOnGrundStatePausedFFOrZGState (true);
			}
		} else {
			setCheckOnGrundStatePausedFFOrZGState (false);
		}

		movingVertically = false;

		setLastTimeFalling ();

		IKSystemManager.setIKBodyState (zeroGravityModeOn, "Zero Gravity Mode");

		setFootStepManagerState (zeroGravityModeOn);
	}

	public void setPushPlayerWhenZeroGravityModeIsEnabledState (bool state)
	{
		pushPlayerWhenZeroGravityModeIsEnabled = state;
	}

	public bool isPlayerOnZeroGravityMode ()
	{
		return zeroGravityModeOn;
	}

	public void changeZeroGravityMovementVelocity (bool value)
	{
		if (value) {
			currentZeroGravitySpeedMultiplier = zeroGravitySpeedMultiplier;
			playerCameraManager.changeCameraFov (true);
		} else {
			currentZeroGravitySpeedMultiplier = 1;
			playerCameraManager.changeCameraFov (false);
		}
		movementSpeedIncreased = value;
	}

	public void setCheckOnGrundStatePausedFFOrZGState (bool state)
	{
		if (state) {
			if ((zeroGravityModeOn && pauseCheckOnGroundStateZG) || (freeFloatingModeOn && pauseCheckOnGroundStateFF)) {
				checkOnGroundStatePausedFFOrZG = state;
			}
		} else {
			checkOnGroundStatePausedFFOrZG = state;
		}
	}

	public void setcheckOnGroundStatePausedState (bool state)
	{
		checkOnGroundStatePaused = state;
	}

	public bool isPauseCheckOnGroundStateZGActive ()
	{
		return pauseCheckOnGroundStateZG;
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		freeFloatingModeOn = state;

		if (freeFloatingModeOn) {
			useJumpPlatform (transform.up * pushFreeFloatingModeEnabledAmount, ForceMode.Force);
			if (!playerOnGround) {
				setCheckOnGrundStatePausedFFOrZGState (true);
			}
		} else {
			setCheckOnGrundStatePausedFFOrZGState (false);
		}

		setLastTimeFalling ();

		movingVertically = false;

		playerCameraManager.changeCameraFov (false);

		IKSystemManager.setIKBodyState (freeFloatingModeOn, "Free Floating Mode");

		setFootStepManagerState (freeFloatingModeOn);
	}

	public bool isPlayerOnFreeFloatingMode ()
	{
		return freeFloatingModeOn;
	}

	public void changeFreeFloatingMovementVelocity (bool value)
	{
		if (value) {
			currentFreeFloatingSpeedMultiplier = freeFloatingSpeedMultiplier;
			playerCameraManager.changeCameraFov (true);
		} else {
			currentFreeFloatingSpeedMultiplier = 1;
			playerCameraManager.changeCameraFov (false);
		}
		movementSpeedIncreased = value;
	}

	public void changeSprintFovThirdPerson (bool value)
	{
		if (value) {
			playerCameraManager.changeCameraFov (true);
		} else {
			playerCameraManager.changeCameraFov (false);
		}
	}

	public bool isPlayerOnFFOrZeroGravityModeOn ()
	{
		return zeroGravityModeOn || freeFloatingModeOn;
	}

	public bool isMovementSpeedIncreased ()
	{
		return movementSpeedIncreased;
	}

	public float getCharacterHeight ()
	{
		return originalHeight;
	}

	public void setWalkSpeedValue (float newValue)
	{
		walkSpeed = Mathf.Clamp (newValue, 0, 1);
	}

	public void setOriginalWalkSpeed ()
	{
		setWalkSpeedValue (originalWalkSpeedValue);
	}

	public void setWalkByDefaultState (bool state)
	{
		if (state) {
			setWalkSpeedValue (0.5f);
		} else {
			setWalkSpeedValue (1);
		}
	}

	public void setIncreaseMovementSpeedState (bool state)
	{
		if (state) {
			setWalkSpeedValue (increaseWalkSpeedValue);
		} else {
			setWalkSpeedValue (originalWalkSpeedValue);
		}
	}

	public void setStairsValues (float stairsMin, float stairsMax, float stairsAhderence)
	{
		currentStairAdherenceSystemMinValue = stairsMin;
		currentStairAdherenceSystemMaxValue = stairsMax;
		currentStairAdherenceSystemAdherenceValue = stairsAhderence;
	}

	public void setCharacterMeshGameObject (GameObject characterMesh)
	{
		characterMeshGameObject = characterMesh;

		updateComponent ();
	}

	public void setCharacterMeshGameObjectState (bool state)
	{
		characterMeshGameObject.SetActive (state);

		for (int i = 0; i < extraCharacterMeshGameObject.Count; i++) {
			if (extraCharacterMeshGameObject [i]) {
				extraCharacterMeshGameObject [i].SetActive (state);
			}
		}
	}

	public GameObject getCharacterMeshGameObject ()
	{
		return characterMeshGameObject;
	}

	public Transform getCharacterHumanBone (HumanBodyBones boneToFind)
	{
		return animator.GetBoneTransform (boneToFind);
	}

	public void setAnimatorState (bool state)
	{
		animator.enabled = state;
	}

	public playerInputManager getPlayerInput ()
	{
		return playerInput;
	}

	public void set3dOr2_5dWorldType (bool movingOn3dWorld)
	{
		if (movingOn3dWorld) {
			lockedPlayerMovement = lockedPlayerMovementMode.world3d;
		} else {
			lockedPlayerMovement = lockedPlayerMovementMode.world2_5d;
		}
	}

	public bool isPlayerMovingOn3dWorld ()
	{
		return lockedPlayerMovement == lockedPlayerMovementMode.world3d;
	}

	public bool isPlayerSetAsChildOfParent ()
	{
		return playerSetAsChildOfParent;
	}

	public void setPlayerSetAsChildOfParentState (bool state)
	{
		playerSetAsChildOfParent = state;
	}

	public void destroyCharacterAtOnce ()
	{
		Destroy (playerManagersParentGameObject);
		Destroy (playerCameraGameObject);
		Destroy (gameObject);
	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<playerController> ());
		#endif
	}

	//Draw gizmos
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
		if (showGizmo) {

			if (useAutoCrouch) {
				Gizmos.color = gizmoColor;

				Vector3 position = autoCrouchRayPosition.position;
				Gizmos.DrawSphere (position, gizmoRadius);
				Gizmos.color = Color.white;
				Gizmos.DrawLine (position, position + transform.forward * raycastDistanceToAutoCrouch);

				Gizmos.DrawSphere (position + secondRaycastOffset, gizmoRadius);

				Gizmos.DrawLine (position + secondRaycastOffset, position + secondRaycastOffset + transform.forward * raycastDistanceToAutoCrouch);
			}

			Debug.DrawRay (transform.position + transform.up, -transform.up * rayDistance, Color.white);
		}
	}
}