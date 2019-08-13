using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerCamera : MonoBehaviour
{
	public bool cameraCanBeUsed;
	public Camera mainCamera;
	public Transform mainCameraTransform;
	public Transform pivotCameraTransform;

	public string currentStateName;
	public string defaultStateName;
	public string defaultThirdPersonStateName = "Third Person";
	public string defaultFirstPersonStateName = "First Person";

	public string defaultThirdPersonCrouchStateName = "Third Person Crouch";
	public string defaultFirstPersonCrouchStateName = "First Person Crouch";

	public string defaultThirdPersonAimRightStateName = "Aim Right";
	public string defaultThirdPersonAimLeftStateName = "Aim Left";

	public string defaultMoveCameraAwayStateName = "Move Away";

	public string defaultLockedCameraStateName = "Locked Camera";

	string previousFreeCameraStateName = "";

	public float firstPersonVerticalRotationSpeed = 5;
	public float firstPersonHorizontalRotationSpeed = 5;
	public float thirdPersonVerticalRotationSpeed = 5;
	public float thirdPersonHorizontalRotationSpeed = 5;

	public float currentVerticalRotationSpeed;
	public float currentHorizontalRotationSpeed;

	float originalFirstPersonVerticalRotationSpeed;
	float originalFirstPersonHorizontalRotationSpeed;
	float originalThirdPersonVerticalRotationSpeed;
	float originalThirdPersonHorizontalRotationSpeed;

	public float smoothBetweenState;
	public float maxCheckDist = 0.1f;
	public float movementLerpSpeed = 5;
	public float zoomSpeed = 120;
	public float fovChangeSpeed;

	public float firstPersonVerticalRotationSpeedZoomIn = 2.5f;
	public float firstPersonHorizontalRotationSpeedZoomIn = 2.5f;
	public float thirdPersonVerticalRotationSpeedZoomIn = 2.5f;
	public float thirdPersonHorizontalRotationSpeedZoomIn = 2.5f;

	public List<cameraStateInfo> playerCameraStates = new List<cameraStateInfo> ();
	public cameraSettings settings = new cameraSettings ();
	public bool onGround;
	public bool aimingInThirdPerson;
	public bool moveAwayActive;
	public bool crouching;
	public bool firstPersonActive;
	public bool usingZoomOn;
	public bool usingZoomOff;
	public bool cameraCanRotate = true;
	public GameObject playerControllerGameObject;
	public typeOfCamera cameraType;

	public bool cameraCurrentlyLocked;

	public bool changeCameraViewEnabled = true;

	public enum typeOfCamera
	{
		free,
		locked
	}

	Vector2 lookAngle;
	public cameraStateInfo currentState;
	public cameraStateInfo lerpState;
	public bool usedByAI;

	public float horizontalInput;
	public float verticalInput;

	public bool playerNavMeshEnabled;
	bool touchPlatform;
	Touch currentTouch;
	Vector2 playerLookAngle;

	//Look at target variables
	public Transform lookAtTargetTransform;
	public bool lookAtTargetEnabled;
	public bool canActivateLookAtTargetEnabled = true;
	public Transform targetToLook;
	public bool lookingAtTarget;

	public float lookAtTargetSpeed;
	public float lookCloserAtTargetSpeed = 3;
	public float lookAtTargetSpeed2_5dView = 5;

	public float maxDistanceToFindTarget;
	public bool useLookTargetIcon;
	public GameObject lookAtTargetIcon;
	public RectTransform lookAtTargetIconRectTransform;

	public List<string> tagToLookList = new List<string> ();
	public LayerMask layerToLook;
	public bool lookOnlyIfTargetOnScreen;
	public bool checkObstaclesToTarget;
	public bool getClosestToCameraCenter;
	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;
	public bool useTimeToStopAimAssist;
	public float timeToStopAimAssist;

	public bool useTimeToStopAimAssistLockedCamera;
	public float timeToStopAimAssistLockedCamera;

	public bool lookingAtPoint;
	public Vector3 pointTargetPosition;
	float originalLookAtTargetSpeed;
	float originalMaxDistanceToFindTarget;

	public bool searchPointToLookComponents;
	public LayerMask pointToLookComponentsLayer;

	public bool lookAtBodyPartsOnCharacters;
	public List<string> bodyPartsToLook = new List<string> ();
	Vector3 lookTargetPosition;

	public bool useObjectToGrabFoundShader;
	public Shader objectToGrabFoundShader;
	public float shaderOutlineWidth;
	public Color shaderOutlineColor;

	outlineObjectSystem currentOutlineObjectSystem;

	public bool pauseOutlineShaderChecking;

	public List<Camera> cameraList = new List<Camera> ();

	bool searchingToTargetOnLockedCamera;

	public bool canActiveLookAtTargetOnLockedCamera;
	bool activeLookAtTargetOnLockedCamera;

	public bool canChangeTargetToLookWithCameraAxis;
	public float minimumCameraDragToChangeTargetToLook = 1;
	public float waitTimeToNextChangeTargetToLook = 0.5f;
	public bool useOnlyHorizontalCameraDragValue;

	//Aim assist variables
	float lastTimeAimAsisst;
	bool usingAutoCameraRotation;
	bool lookintAtTargetByInput;

	Vector3 targetPosition;
	Vector3 screenPoint;
	Transform placeToShoot;
	bool targetOnScreen;

	//Custom editor variables
	public bool showSettings;
	public bool showLookTargetSettings;

	//2.5d variables
	public bool using2_5ViewActive;

	Vector2 moveCameraLimitsX2_5d;
	Vector2 moveCameraLimitsY2_5d;

	bool moveInXAxisOn2_5d;
	Vector3 originalLockedCameraPivotPosition;

	public bool clampAimDirections;
	public int numberOfAimDirections = 8;
	public float minDistanceToCenterClampAim = 1.2f;

	//Top down variables
	public bool useTopDownView;
	Vector2 moveCameraLimitsXTopDown;
	Vector2 moveCameraLimitsYTopDown;

	bool setLookDirection;
	Vector3 currentCameraMovementPosition;

	public bool regularMovementOnBulletTime = true;

	bool fieldOfViewChanging;
	float AccelerometerUpdateInterval = 0.01f;
	float LowPassKernelWidthInSeconds = 0.001f;
	float lastTimeMoved;
	bool adjustPivotAngle;

	bool horizontalCameraLimitActiveOnGround = true;
	bool horizontalCameraLimitActiveOnAir = true;

	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;
	Transform currentTargetToAim;

	bool isMoving;
	RaycastHit hit;
	Vector3 lowPassValue = Vector3.zero;
	Vector2 acelerationAxis;

	Matrix4x4 calibrationMatrix;

	public playerInputManager playerInput;
	public playerController playerControllerManager;
	public otherPowers powersManager;
	public gravitySystem gravityManager;
	public headBob headBobManager;
	public grabObjects grabObjectsManager;
	public scannerSystem scannerManager;
	public playerWeaponsManager weaponsManager;
	public playerNavMeshSystem playerNavMeshManager;
	public characterStateIconSystem characterStateIconManager;
	public gameManager mainGameManager;
	public menuPause pauseManager;
	public Collider mainCollider;
	public Animator mainAnimator;

	Transform hips;
	Coroutine changeFovCoroutine;
	Transform targetToFollow;


	bool smoothFollow;
	bool smoothReturn;
	bool smoothGo;
	bool dead;

	//AI variables
	Vector3 navMeshCurrentLookPos;

	//Locked camera variables

	bool lockedCameraChanged;
	bool lockedCameraCanFollow;
	Transform previousLockedCameraAxisTransform;
	lockedCameraSystem.cameraAxis currentLockedCameraAxisInfo;
	lockedCameraSystem.cameraAxis previousLockedCameraAxisInfo;
	Coroutine lockedCameraCoroutine;
	Vector3 lockedCameraFollowPlayerPositionVelocity = Vector3.zero;
	bool lockedCameraMoving;

	bool inputNotPressed;

	Vector2 currentLockedLoonAngle;
	Quaternion currentLockedCameraRotation;
	Quaternion currentLockedPivotRotation;
	bool usingLockedZoomOn;
	float lastTimeLockedSpringRotation;
	float lastTimeLockedSpringMovement;
	Vector3 currentLockedCameraMovementPosition;
	Vector3 currentLockedMoveCameraPosition;
	Vector2 currentLockedLimitLookAngle;

	public Transform lockedCameraElementsParent;
	public Transform lockedMainCameraTransform;
	public Transform lockedCameraAxis;
	public Transform lockedCameraPosition;
	public Transform lockedCameraPivot;

	public Transform lookCameraParent;
	public Transform lookCameraPivot;
	public Transform lookCameraDirection;

	public Transform clampAimDirectionTransform;

	public Transform lookDirectionTransform;

	public Transform auxLockedCameraAxis;

	public RectTransform currentLockedCameraCursor;
	public Vector2 currentLockedCameraCursorSize;

	public bool useLayerToSearchTargets;

	float horizontaLimit;
	float verticalLimit;

	float newVerticalPosition;
	float newVerticalPositionVelocity;

	float newHorizontalPosition;
	float newHorizontalPositionVelocity;

	//Locked Camera Limits Variables
	public bool useCameraLimit;
	public Vector3 currentCameraLimitPosition;

	public bool useWidthLimit;
	public float widthLimitRight;
	public float widthLimitLeft;
	public bool useHeightLimit;
	public float heightLimitUpper;
	public float heightLimitLower;

	public bool useDepthLimit;
	public float depthLimitFront;
	public float depthLimitBackward;

	public setTransparentSurfaces setTransparentSurfacesManager;

	//Camera noise variables
	Vector2 shotCameraNoise;
	bool addNoiseToCamera;

	public bool useSmoothCameraRotation;
	public bool useSmoothCameraRotationThirdPerson;
	public float smoothCameraRotationSpeedVerticalThirdPerson = 10;
	public float smoothCameraRotationSpeedHorizontalThirdPerson = 10;
	public bool useSmoothCameraRotationFirstPerson;
	public float smoothCameraRotationSpeedVerticalFirstPerson = 10;
	public float smoothCameraRotationSpeedHorizontalFirstPerson = 10;

	float currentSmoothCameraRotationSpeedVertical;
	float currentSmoothCameraRotationSpeedHorizontal;
	Quaternion currentPivotRotation;

	Quaternion currentCameraRotation;

	float currentCameraUpRotation;

	public cameraRotationType cameraRotationMode;

	public enum cameraRotationType
	{
		vertical,
		horizontal,
		free
	}

	float currentDeltaTime;
	float currentUpdateDeltaTime;
	float currentFixedUpdateDeltaTime;
	float currentLateUpdateDeltaTime;

	Vector2 axisValues;

	bool usingPlayerNavMeshPreviously;

	Vector3 cameraInitialPosition;
	Vector3 offsetInitialPosition;

	public bool zeroGravityModeOn;
	float forwardRotationAngle;
	float targetForwardRotationAngle;
	Quaternion currentForwardRotation;
	public bool canRotateForwardOnZeroGravityModeOn;
	public float rotateForwardOnZeroGravitySpeed;

	public bool freeFloatingModeOn;

	Coroutine lookAtTargetEnabledCoroutine;
	Coroutine fixPlayerZPositionCoroutine;
	Coroutine cameraFovStartAndEndCoroutine;
	Coroutine zoomStateDurationCoroutine;

	List<Transform> targetsListToLookTransform = new List<Transform> ();
	int currentTargetToLookIndex;

	bool driving;

	public bool useEventOnMovingLockedCamera;
	public UnityEvent eventOnStartMovingLockedCamera;
	public UnityEvent eventOnKeepMovingLockedCamera;
	public UnityEvent eventOnStopMovingLockedCamera;
	public bool useEventOnFreeCamereToo;

	bool movingCameraState;
	bool movingCameraPrevioslyState;

	//Camera Bound variables
	Vector3 horizontalOffsetOnSide;
	Vector3 horizontalOffsetOnFaceSideSpeed;

	Vector3 horizontalOffsetOnSideOnMoving;
	Vector3 horizontalOffsetOnFaceSideOnMovingSpeed;

	Vector3 verticalOffsetOnMove;
	Vector3 verticalOffsetOnMoveSpeed;

	public FocusArea focusArea;

	public Vector3 focusTargetPosition;

	bool playerAiming;
	bool playerAimingPreviously;
	float lastTimeAiming;

	float originalMaxDistanceToCameraCenterValue;
	bool originalUsemaxDistanceToCameraCenterValue;

	public bool resetCameraRotationAfterTime;
	public float timeToResetCameraRotation;
	public float resetCameraRotationSpeed;
	float lastTimeCameraRotated;
	bool resetingCameraActive;

	bool usingSetTransparentSurfacesPreviously;

	bool rotatingLockedCameraToRight;
	bool rotatingLockedCameraToLeft;

	float lockedCameraRotationDirection;
	bool selectingTargetToLookWithMouseActive;

	float currentAxisValuesMagnitude;
	float lastTimeChangeCurrentTargetToLook;
	float currentLockedCameraAngle;

	public UnityEvent setThirdPersonInEditorEvent;
	public UnityEvent setFirstPersonInEditorEvent;

	public bool useEventOnThirdFirstPersonViewChange;
	public UnityEvent setThirdPersonEvent;
	public UnityEvent setFirstPersonEvent;

	public bool moveCameraPositionWithMouseWheelActive;
	public float moveCameraPositionBackwardWithMouseWheelSpeed = 0.3f;
	public float moveCameraPositionForwardWithMouseWheelSpeed = 0.3f;
	public float minDistanceToChangeToFirstPerson = 0.1f;
	public float maxExtraDistanceOnThirdPerson = 1;

	public bool useCameraMouseWheelStates;

	public List<cameraMouseWheelStates> cameraMouseWheelStatesList = new List<cameraMouseWheelStates> ();

	public GameObject lockedCameraSystemPrefab;
	public GameObject lockedCameraLimitSystemPrefab;

	public List<lockedCameraPrefabsTypes> lockedCameraPrefabsTypesList = new List<lockedCameraPrefabsTypes> ();

	string currentCameraShakeStateName;

	void Awake ()
	{
		for (int i = 0; i < playerCameraStates.Count; i++) {
			playerCameraStates [i].originalCamPositionOffset = playerCameraStates [i].camPositionOffset;
		}

		//get the player's hips, so the camera can follow the ragdoll
		hips = mainAnimator.GetBoneTransform (HumanBodyBones.Hips).transform;

		//if the game doesn't starts with the first person view, get the original camera position and other parameters for the camera collision system and 
		//movement ranges
		if (!firstPersonActive) {
			//check if the player uses animator in first person
			playerControllerManager.checkAnimatorIsEnabled (false);		
		} else {
			//check if the player uses animator in first person
			playerControllerManager.checkAnimatorIsEnabled (true);			
		}

		originalThirdPersonVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
		originalThirdPersonHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		originalFirstPersonVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
		originalFirstPersonHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;

		//get the input manager to get every key or touch press

		cameraInitialPosition = mainCameraTransform.localPosition;
		defaultStateName = currentStateName;

		//set the camera state when the game starts
		setCameraState (currentStateName);
		currentState = new cameraStateInfo (lerpState);

		offsetInitialPosition = currentState.camPositionOffset;

		mainCameraTransform.localPosition = currentState.camPositionOffset;

		targetToFollow = playerControllerGameObject.transform;

		if (lookAtTargetTransform) {
			lookAtTargetTransform.SetParent (null);
		}

		if (mainCamera.enabled && mainGameManager) {
			mainGameManager.setMainCamera (mainCamera);
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		originalLookAtTargetSpeed = lookAtTargetSpeed;

		originalMaxDistanceToFindTarget = maxDistanceToFindTarget;

		if (lockedCameraElementsParent) {
			lockedCameraElementsParent.SetParent (null);
			lockedCameraElementsParent.position = Vector3.zero;

			lockedCameraElementsParent.rotation = Quaternion.identity;
		}

		originalMaxDistanceToCameraCenterValue = maxDistanceToCameraCenter;
		originalUsemaxDistanceToCameraCenterValue = useMaxDistanceToCameraCenter;

		checkCurrentRotationSpeed ();

		if (!usedByAI) {
			mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
			halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;

			usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
		}

		if (moveCameraPositionWithMouseWheelActive) {
			for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
				if (cameraMouseWheelStatesList [i].isCurrentCameraState) {
					currentMouseWheelStateIndex = i;
				}
			}
		}

		if (lookAtTargetIcon) {
			lookAtTargetIconRectTransform = lookAtTargetIcon.GetComponent<RectTransform> ();
		}
	}

	void Update ()
	{
		if (usedByAI) {
			transform.position = targetToFollow.position;			
			return;
		}

		checkCurrentRotationSpeed ();

		currentUpdateDeltaTime = getCurrentDeltaTime ();

		playerAiming = playerControllerManager.isPlayerAiming ();

		if (playerAiming != playerAimingPreviously) {
			playerAimingPreviously = playerAiming;
			lastTimeAiming = Time.time;
		}

		if (cameraType == typeOfCamera.free) {
			if (cameraCanBeUsed) {
				if (!dead) {
					//checkCameraPosition ();
					pivotCameraTransform.localPosition = Vector3.Lerp (pivotCameraTransform.localPosition, currentState.pivotPositionOffset, currentUpdateDeltaTime * movementLerpSpeed);

					//shake the camera if the player is moving in the air or accelerating on it
					if (settings.enableShakeCamera && settings.shake) {
						if (!settings.accelerateShaking) {
							headBobManager.setState (currentCameraShakeStateName);
						}

						if (settings.accelerateShaking) {
							headBobManager.setState ("High Shaking");
						}
					}
				}
			}

			//the camera follows the player position
			//if smoothfollow is false, it means that the player is alive
			if (!smoothFollow) {
				//smoothreturn is used to move the camera from the hips to the player controller position smoothly, to avoid change their positions quickly
				if (smoothReturn) {
					float speed = 1;
					float distance = GKC_Utils.distance (transform.position, targetToFollow.position);
					if (distance > 1) {
						speed = distance;
					}

					transform.position = Vector3.MoveTowards (transform.position, targetToFollow.position, currentUpdateDeltaTime * speed);

					if (transform.position == targetToFollow.position) {
						smoothReturn = false;
					}
				} else {
					//in this state the player is playing normally
					transform.position = targetToFollow.position +
					transform.right * currentState.pivotParentPossitionOffset.x +
					transform.up * currentState.pivotParentPossitionOffset.y +
					transform.forward * currentState.pivotParentPossitionOffset.z;
				}
			} else {
				//else follow the ragdoll
				//in this state the player has dead, he cannot move, and the camera follows the skeleton, until the player chooses play again
				if (smoothGo) {
					float speed = 1;
					float distance = GKC_Utils.distance (transform.position, targetToFollow.position);
					if (distance > 1) {
						speed = distance;
					}

					transform.position = Vector3.MoveTowards (transform.position, targetToFollow.position - Vector3.up / 1.5f, currentUpdateDeltaTime * speed);

					if (transform.position == targetToFollow.position - Vector3.up / 1.5f) {
						smoothGo = false;
					}
				} else {
					transform.position = targetToFollow.position - Vector3.up / 1.5f;
				}
			}
		}

		//if current camera mode is locked, set its values according to every fixed camera trigger configuration
		if (cameraType == typeOfCamera.locked) {
			
			checkCameraPosition ();

			transform.position = targetToFollow.position;

			if (lockedCameraChanged) {
				if (currentLockedCameraAxisInfo.axis != previousLockedCameraAxisTransform) {
					if (!playerControllerManager.isPlayerMoving (0.6f) && !playerControllerManager.isPlayerUsingInput ()) {
						inputNotPressed = true;
					}

					if (inputNotPressed && (playerControllerManager.isPlayerUsingInput () || !playerControllerManager.isPlayerMoving (0))) {
						setCurrentAxisTransformValues (lockedCameraAxis);

						previousLockedCameraAxisTransform = currentLockedCameraAxisInfo.axis;

						lockedCameraChanged = false;
						lockedCameraCanFollow = true;

						inputNotPressed = false;
					}
				} else {
					lockedCameraChanged = false;
				}
			}

			//look at player position
			if (!lockedCameraMoving && currentLockedCameraAxisInfo.lookAtPlayerPosition) {

				calculateLockedCameraLookAtPlayerPosition ();
					
				lockedCameraPosition.localRotation = Quaternion.Slerp (lockedCameraPosition.localRotation, 
					Quaternion.Euler (new Vector3 (currentLockedLimitLookAngle.x, 0, 0)), currentUpdateDeltaTime * currentLockedCameraAxisInfo.lookAtPlayerPositionSpeed);

				lockedCameraAxis.localRotation = Quaternion.Slerp (lockedCameraAxis.localRotation, 
					Quaternion.Euler (new Vector3 (0, currentLockedLimitLookAngle.y, 0)),	currentUpdateDeltaTime * currentLockedCameraAxisInfo.lookAtPlayerPositionSpeed);

				if (lockedCameraCanFollow) {
					setCurrentAxisTransformValues (lockedCameraAxis);
				}
			}

			//rotate camera with mouse using a range
			if (currentLockedCameraAxisInfo.cameraCanRotate && cameraCanBeUsed && !playerAiming) {
				Vector2 currentAxisValues = playerInput.getPlayerMovementAxis ("mouse");
				float horizontalMouse = currentAxisValues.x;
				float verticalMouse = currentAxisValues.y;

				currentLockedLoonAngle.x += horizontalMouse * currentHorizontalRotationSpeed;
				currentLockedLoonAngle.y -= verticalMouse * currentVerticalRotationSpeed;

				currentLockedLoonAngle.x = Mathf.Clamp (currentLockedLoonAngle.x, currentLockedCameraAxisInfo.rangeAngleY.x, currentLockedCameraAxisInfo.rangeAngleY.y);
				
				currentLockedLoonAngle.y = Mathf.Clamp (currentLockedLoonAngle.y, currentLockedCameraAxisInfo.rangeAngleX.x, currentLockedCameraAxisInfo.rangeAngleX.y);

				if (currentLockedCameraAxisInfo.smoothRotation) {

					currentLockedCameraRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX + currentLockedLoonAngle.y, 0, 0);

					currentLockedPivotRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY + currentLockedLoonAngle.x, 0);

					if (currentLockedCameraAxisInfo.useSpringRotation) {
						if (horizontalMouse != 0 || verticalMouse != 0) {
							lastTimeLockedSpringRotation = Time.time;

						}
						if (Time.time > lastTimeLockedSpringRotation + currentLockedCameraAxisInfo.springRotationDelay) {
							currentLockedCameraRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX, 0, 0);
							currentLockedPivotRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY, 0);
							currentLockedLoonAngle = Vector2.zero;
						}
					}

					lockedCameraPosition.localRotation = Quaternion.Slerp (lockedCameraPosition.localRotation, currentLockedCameraRotation, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.rotationSpeed);
					
					lockedCameraAxis.localRotation = Quaternion.Slerp (lockedCameraAxis.localRotation, currentLockedPivotRotation, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.rotationSpeed);

				} else {
					lockedCameraPosition.localRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX + currentLockedLoonAngle.y, 0, 0);
			
					lockedCameraAxis.localRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY + currentLockedLoonAngle.x, 0);
				}
			}
				
			//move camera up, down, right and left
			if (currentLockedCameraAxisInfo.canMoveCamera && cameraCanBeUsed && !playerAiming) {
				Vector2 currentAxisValues = playerInput.getPlayerMovementAxis ("mouse");
				float horizontalMouse = currentAxisValues.x;
				float verticalMouse = currentAxisValues.y;

				currentLockedMoveCameraPosition.x += horizontalMouse * currentLockedCameraAxisInfo.moveCameraSpeed;
				currentLockedMoveCameraPosition.y += verticalMouse * currentLockedCameraAxisInfo.moveCameraSpeed;

				currentLockedMoveCameraPosition.x = Mathf.Clamp (currentLockedMoveCameraPosition.x, currentLockedCameraAxisInfo.moveCameraLimitsX.x, currentLockedCameraAxisInfo.moveCameraLimitsX.y);

				currentLockedMoveCameraPosition.y = Mathf.Clamp (currentLockedMoveCameraPosition.y, currentLockedCameraAxisInfo.moveCameraLimitsY.x, currentLockedCameraAxisInfo.moveCameraLimitsY.y);

				Vector3 moveInput = currentLockedMoveCameraPosition.x * Vector3.right +	currentLockedMoveCameraPosition.y * Vector3.up;	

				if (currentLockedCameraAxisInfo.smoothCameraMovement) {
					
					currentLockedCameraMovementPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition + moveInput;

					if (currentLockedCameraAxisInfo.useSpringMovement) {
						if (horizontalMouse != 0 || verticalMouse != 0) {
							lastTimeLockedSpringMovement = Time.time;

						}
						if (Time.time > lastTimeLockedSpringMovement + currentLockedCameraAxisInfo.springMovementDelay) {
							currentLockedCameraMovementPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;
							currentLockedMoveCameraPosition = Vector3.zero;
						}
					}

					lockedCameraAxis.localPosition = Vector3.MoveTowards (lockedCameraAxis.localPosition, currentLockedCameraMovementPosition, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.smoothCameraSpeed);
				} else {
					lockedCameraAxis.localPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition + moveInput;
				}
			}

			if (useEventOnMovingLockedCamera) {
				checkEventOnMoveCamera (playerInput.getPlayerMovementAxis ("mouse"));
			}

			if (cameraCanBeUsed && currentLockedCameraAxisInfo.canRotateCameraHorizontally) {
				if (rotatingLockedCameraToLeft || rotatingLockedCameraToRight) {
					lockedMainCameraTransform.Rotate (0, Time.deltaTime * currentLockedCameraAxisInfo.horizontalCameraRotationSpeed * lockedCameraRotationDirection, 0);
					lockedCameraPivot.Rotate (0, Time.deltaTime * currentLockedCameraAxisInfo.horizontalCameraRotationSpeed * lockedCameraRotationDirection, 0);
				}
			}

			//aim weapon
			if (playerAiming) {
				if (currentLockedCameraCursor != null) {
					float horizontalMouse = 0;
					float verticalMouse = 0;

					if (cameraCanBeUsed && !lookingAtFixedTarget) {
						Vector2 currentAxisValues = playerInput.getPlayerMovementAxis ("mouse");
						horizontalMouse = currentAxisValues.x;
						verticalMouse = currentAxisValues.y;
					}

					//if the player is on 2.5d view, set the cursor position on screen where the player will aim
					if (using2_5ViewActive) {
						if (Time.time < lastTimeAiming + 0.01f) {
							return;
						}

						if (!setLookDirection) {

							moveCameraLimitsX2_5d = currentLockedCameraAxisInfo.moveCameraLimitsX2_5d;
							moveCameraLimitsY2_5d = currentLockedCameraAxisInfo.moveCameraLimitsY2_5d;

							if (targetToLook) {
								Vector3 worldPosition = targetToLook.position;
								if (moveInXAxisOn2_5d) {
									lookCameraDirection.position = new Vector3 (worldPosition.x, worldPosition.y, lookCameraDirection.position.z);
								} else {
									lookCameraDirection.position = new Vector3 (lookCameraDirection.position.x, worldPosition.y, worldPosition.z);
								}

							} else {

								//Check the rotation of the player in his local Y axis to check the closest direction to look
								bool lookingAtRight = false;
								float lookingDirectionAngle = 0;

								if (moveInXAxisOn2_5d) {
									lookingDirectionAngle = Vector3.Dot (targetToFollow.forward, lookCameraPivot.right); 
								} else {
									lookingDirectionAngle = Vector3.Dot (targetToFollow.forward, lookCameraPivot.forward); 
								}

								if (lookingDirectionAngle > 0) {
									lookingAtRight = true;
								}

								//The player will look in the right direction of the screen
								if (lookingAtRight) {
									if (moveInXAxisOn2_5d) {
										lookCameraDirection.localPosition = new Vector3 (moveCameraLimitsX2_5d.y, 0, 0);
									} else {
										lookCameraDirection.localPosition = new Vector3 (0, 0, moveCameraLimitsX2_5d.y);
									}
									currentCameraMovementPosition.x = moveCameraLimitsX2_5d.y;
								} 

								//else the player will look in the left direction
								else {
									if (moveInXAxisOn2_5d) {
										lookCameraDirection.localPosition = new Vector3 (moveCameraLimitsX2_5d.x, 0, 0);
									} else {
										lookCameraDirection.localPosition = new Vector3 (0, 0, moveCameraLimitsX2_5d.x);
									}
									currentCameraMovementPosition.x = moveCameraLimitsX2_5d.x;
								}

								if (moveInXAxisOn2_5d) {
									lookCameraDirection.localPosition = new Vector3 (currentCameraMovementPosition.x, 0, lookCameraDirection.localPosition.z);
								} else {
									lookCameraDirection.localPosition = new Vector3 (lookCameraDirection.localPosition.x, 0, currentCameraMovementPosition.x);
								}
							}

							setLookDirection = true;

							pivotCameraTransform.localRotation = Quaternion.identity;
							transform.rotation = targetToFollow.rotation;

							//set the transform position and rotation to follow the lookCameraDirection direction only in the local Y axis, to get the correct direction to look to right or left
							lookDirectionTransform.localRotation = Quaternion.Euler (getLookDirectionTransformRotationValue (transform.forward));

							lookAngle = Vector2.zero;

							clampAimDirectionTransform.localPosition = lookCameraDirection.localPosition;
						}


						//if the camera is following a target, set that direction to the camera to aim directly at that object
						if (targetToLook) {
							Vector3 worldPosition = targetToLook.position;
							if (moveInXAxisOn2_5d) {
								lookCameraDirection.position = new Vector3 (worldPosition.x, worldPosition.y, lookCameraDirection.position.z);
							} else {
								lookCameraDirection.position = new Vector3 (lookCameraDirection.position.x, worldPosition.y, worldPosition.z);
							}
						} else {
							//else, the player is aiming freely on the screen
							if (moveInXAxisOn2_5d) {
								currentCameraMovementPosition = horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.right;
							} else {
								currentCameraMovementPosition = horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.forward;
							}
							currentCameraMovementPosition += verticalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.up;

							lookCameraDirection.Translate (lookCameraDirection.InverseTransformDirection (currentCameraMovementPosition));

							//clamp aim direction in 8, 4 or 2 directions
							if (clampAimDirections) {
								bool lookingAtRight = false;
								float lookingDirectionAngle = 0;

								if (moveInXAxisOn2_5d) {
									lookingDirectionAngle = Vector3.Dot (lookDirectionTransform.forward, lookCameraPivot.right); 
								} else {
									lookingDirectionAngle = Vector3.Dot (lookDirectionTransform.forward, lookCameraPivot.forward); 
								}

								if (lookingDirectionAngle > 0) {
									lookingAtRight = true;
								}

								float targetHorizontalValue = 0;

								Vector3 currentDirectionToLook = lookCameraDirection.position - lookDirectionTransform.position;

								if (lookingAtRight) {
									if (moveInXAxisOn2_5d) {
										targetHorizontalValue = Vector3.SignedAngle (currentDirectionToLook, lookCameraPivot.right, lookCameraPivot.forward);
									} else {
										targetHorizontalValue = -Vector3.SignedAngle (currentDirectionToLook, lookCameraPivot.forward, lookCameraPivot.right);
									}
								} else {
									if (moveInXAxisOn2_5d) {
										targetHorizontalValue = Vector3.SignedAngle (currentDirectionToLook, -lookCameraPivot.right, -lookCameraPivot.forward);
									} else {
										targetHorizontalValue = -Vector3.SignedAngle (currentDirectionToLook, -lookCameraPivot.forward, -lookCameraPivot.right);
									}
								}
									
								Vector2 new2DPosition = Vector2.zero;

								float distanceToCenter = GKC_Utils.distance (lookCameraDirection.localPosition, Vector3.zero);

								//print (lookingAtRight + " " + targetHorizontalValue + " " + distanceToCenter);

								if (numberOfAimDirections == 8) {
									if (targetHorizontalValue > 0) {
										if (targetHorizontalValue < 30 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else if (targetHorizontalValue > 30 && targetHorizontalValue < 60) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, 0);
										}
									} else {
										if (targetHorizontalValue > -30 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else if (targetHorizontalValue < -30 && targetHorizontalValue > -60) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, 0);
										}
									}
								} else if (numberOfAimDirections == 4) {
									if (targetHorizontalValue > 0) {
										if (targetHorizontalValue < 45 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, 0);
										}
									} else {
										if (targetHorizontalValue > -45 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, 0);
										}
									}
								} else if (numberOfAimDirections == 2) {
									if (lookingAtRight) {
										new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
									} else {
										new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
									}
								}

								if (moveInXAxisOn2_5d) {
									clampAimDirectionTransform.localPosition = new Vector3 (new2DPosition.y, new2DPosition.x, lookCameraDirection.localPosition.z);
								} else {
									clampAimDirectionTransform.localPosition = new Vector3 (lookCameraDirection.localPosition.x, new2DPosition.x, new2DPosition.y);
								}
							}
						}
							
						//clamp the aim position to the limits of the current camera
						Vector3 newCameraPosition = lookCameraDirection.localPosition;
						float posX, posY, posZ;
						posY = Mathf.Clamp (newCameraPosition.y, moveCameraLimitsY2_5d.x, moveCameraLimitsY2_5d.y);

						if (moveInXAxisOn2_5d) {
							posX = Mathf.Clamp (newCameraPosition.x, moveCameraLimitsX2_5d.x, moveCameraLimitsX2_5d.y);
							lookCameraDirection.localPosition = new Vector3 (posX, posY, newCameraPosition.z);
						} else {
							posZ = Mathf.Clamp (newCameraPosition.z, moveCameraLimitsX2_5d.x, moveCameraLimitsX2_5d.y);
							lookCameraDirection.localPosition = new Vector3 (newCameraPosition.x, posY, posZ);
						}
				
						if (clampAimDirections && targetToLook == null) {
							pointTargetPosition = clampAimDirectionTransform.position;
						} else {
							pointTargetPosition = lookCameraDirection.position;
						}

						//set the position to the UI icon showing the position where teh player aims
						if (usingScreenSpaceCamera) {
							currentLockedCameraCursor.anchoredPosition = getIconPosition (pointTargetPosition);
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (pointTargetPosition);
							currentLockedCameraCursor.transform.position = screenPoint;
						}


						//set the transform position and rotation to follow the lookCameraDirection direction only in the local Y axis, to get the correct direction to look to right or left
						Vector3 newDirectionToLook = lookCameraDirection.position - lookDirectionTransform.position;

						Vector3 newLookDirectionTransformRotation = getLookDirectionTransformRotationValue (newDirectionToLook);

						lookDirectionTransform.localRotation = Quaternion.Lerp (lookDirectionTransform.localRotation, Quaternion.Euler (newLookDirectionTransformRotation), Time.deltaTime * 5);
					} 

					//else, the player is on top down view or in point and click mode, so check the cursor position to aim
					else {
						//the current view is top down
						if (useTopDownView) {
							if (Time.time < lastTimeAiming + 0.01f) {
								return;
							}

							if (!setLookDirection) {
								moveCameraLimitsXTopDown = currentLockedCameraAxisInfo.moveCameraLimitsXTopDown;
								moveCameraLimitsYTopDown = currentLockedCameraAxisInfo.moveCameraLimitsYTopDown;

								if (targetToLook) {
									Vector3 worldPosition = mainCamera.ScreenToWorldPoint (currentLockedCameraCursor.position);
									lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);
								} else {
									if (currentLockedCameraAxisInfo.use8DiagonalAim) {
										float currentPlayerRotationY = targetToFollow.eulerAngles.y + currentLockedCameraAxisInfo.extraTopDownYRotation;
										if (currentPlayerRotationY > 180) {
											currentPlayerRotationY -= 360;
										}

										float currentPlayerRotationYABS = Mathf.Abs (currentPlayerRotationY);

										//check the current forward direction in Y axis to aim to the closes direction in an angle diviced in 8 directions, so every angles is 360/8=45
										if (currentPlayerRotationYABS < 45) {
											if (currentPlayerRotationYABS > 22.5f) {
												if (currentPlayerRotationY > 0) {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
												} else {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
												}
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.y;
											} else {
												currentCameraMovementPosition.x = 0;
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.y;
											}
										} else if (currentPlayerRotationYABS > 45 && currentPlayerRotationYABS < 135) {
											if (currentPlayerRotationY > 0) {
												currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
											} else {
												currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
											}
											currentCameraMovementPosition.y = 0;
										} else if (currentPlayerRotationYABS > 135) {
											if (currentPlayerRotationYABS < 157.5f) {
												if (currentPlayerRotationY > 0) {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
												} else {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
												}
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.x;
											} else {
												currentCameraMovementPosition.x = 0;
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.x;
											}
										}

										lookCameraDirection.localPosition = 
											new Vector3 (currentCameraMovementPosition.x / 2, lookCameraDirection.localPosition.y, currentCameraMovementPosition.y / 2);
									} else {
										currentCameraMovementPosition = targetToFollow.position + targetToFollow.forward * 4;
										lookCameraDirection.position = new Vector3 (currentCameraMovementPosition.x, lookCameraDirection.position.y, currentCameraMovementPosition.z);
									}
								}

								setLookDirection = true;
								transform.rotation = targetToFollow.rotation;
							}

							lookCameraParent.localRotation = lockedCameraAxis.localRotation;

							if (targetToLook) {
								Vector3 worldPosition = targetToLook.position;
								lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);
							} else {
								currentCameraMovementPosition = 
									horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.right +
								verticalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.forward;	

								lookCameraDirection.Translate (currentCameraMovementPosition);
							}

							Vector3 newCameraPosition = lookCameraDirection.localPosition;
							float posX, posZ;
							posX = Mathf.Clamp (newCameraPosition.x, moveCameraLimitsXTopDown.x, moveCameraLimitsXTopDown.y);
							posZ = Mathf.Clamp (newCameraPosition.z, moveCameraLimitsYTopDown.x, moveCameraLimitsYTopDown.y);
							lookCameraDirection.localPosition = new Vector3 (posX, newCameraPosition.y, posZ);

							pointTargetPosition = lookCameraDirection.position;

							if (usingScreenSpaceCamera) {
								currentLockedCameraCursor.anchoredPosition = getIconPosition (pointTargetPosition);
							} else {
								screenPoint = mainCamera.WorldToScreenPoint (pointTargetPosition);
								currentLockedCameraCursor.transform.position = screenPoint;
							}

							if (currentLockedCameraAxisInfo.checkPossibleTargetsBelowCursor) {
								//check objects below the current camera cursor on the screen to check possible targets to aim, getting their proper place to shoot position
								Ray newRay = mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);

								if (Physics.Raycast (newRay, out hit, Mathf.Infinity, layerToLook)) {
									currentDetectedSurface = hit.collider.gameObject;
								} else {
									currentDetectedSurface = null;
								}

								if (currentTargetDetected != currentDetectedSurface) {
									currentTargetDetected = currentDetectedSurface;
									if (currentTargetDetected) {
										currentTargetToAim = applyDamage.getPlaceToShoot (currentTargetDetected);
//										if (currentTargetToAim) {
//											print (currentTargetToAim.name);
//										}
									} else {
										currentTargetToAim = null;
									}
								}

								if (currentTargetToAim) {
									pointTargetPosition = currentTargetToAim.position;
								}
							}
						} 

						//the player is on point and click camera type
						else {
							if (playerNavMeshEnabled) {
								if (cameraCanBeUsed) {
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

										if (touchPlatform) {
											if (currentTouch.phase == TouchPhase.Began) {
												currentLockedCameraCursor.position = currentTouch.position;
											}
										} else {
											currentLockedCameraCursor.position = currentTouch.position;
										}
									}
								}
							} else {
								if (!setLookDirection) {
									setLookDirection = true;
									transform.rotation = targetToFollow.rotation;

									if (!targetToLook) {
										bool surfaceFound = false;
										bool surfaceFoundOnScreen = false;
										Vector2 screenPos = Vector2.zero;
										if (Physics.Raycast (targetToFollow.position + targetToFollow.up, targetToFollow.forward, out hit, Mathf.Infinity, layerToLook)) {
											surfaceFound = true;

											screenPos = mainCamera.WorldToScreenPoint (hit.point);
											if (screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height) {
												surfaceFoundOnScreen = true;
											}
										}

										if (!surfaceFound || !surfaceFoundOnScreen) {
											screenPos = mainCamera.WorldToScreenPoint (targetToFollow.position + targetToFollow.forward * 3);
											Debug.DrawLine (targetToFollow.position, targetToFollow.position + targetToFollow.forward * 3, Color.white, 5);
										}

										if (currentLockedCameraCursor) {
											currentLockedCameraCursor.position = screenPos;
										}

										lookAngle = Vector2.zero;

										pivotCameraTransform.localRotation = Quaternion.identity;
										transform.rotation = targetToFollow.rotation;
									}
								}

								currentLockedCameraCursor.Translate (new Vector3 (horizontalMouse, verticalMouse, 0) * currentLockedCameraAxisInfo.moveCameraCursorSpeed);
							}

							Vector3 newCameraPosition = currentLockedCameraCursor.position;
							newCameraPosition.x = Mathf.Clamp (newCameraPosition.x, currentLockedCameraCursorSize.x, horizontaLimit);
							newCameraPosition.y = Mathf.Clamp (newCameraPosition.y, currentLockedCameraCursorSize.y, verticalLimit);
							currentLockedCameraCursor.position = new Vector3 (newCameraPosition.x, newCameraPosition.y, 0);

							Ray newRay = mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);
							if (Physics.Raycast (newRay, out hit, Mathf.Infinity, layerToLook)) {
								pointTargetPosition = hit.point;
							} else {
								print ("look at screen point in work position");
							}
						}
					}
				}
			} else {
				if (setLookDirection) {
					if (using2_5ViewActive) {
						lookCameraDirection.localPosition = Vector3.zero;

						if (using2_5ViewActive) {
							transform.rotation = targetToFollow.rotation;
						}
					}

					if (useTopDownView) {
						lookCameraDirection.localPosition = new Vector3 (0, lookCameraDirection.localPosition.y, 0);
					}
				}

				currentCameraMovementPosition = Vector3.zero;
				setLookDirection = false;

				if (using2_5ViewActive && freeFloatingModeOn) {
					transform.rotation = targetToFollow.rotation;
				}
			}
		}
	}

	public Vector3 getLookDirectionTransformRotationValue (Vector3 forwardDirection)
	{
		float lookDirectionTransformRotationAngle = 0;
		if (moveInXAxisOn2_5d) {
			lookDirectionTransformRotationAngle = Vector3.Dot (forwardDirection, lookCameraDirection.right); 
		} else {
			lookDirectionTransformRotationAngle = Vector3.Dot (forwardDirection, lookCameraDirection.forward); 
		}

		Vector3 lookDirectionTransformRotation = Vector3.zero;
		if (lookDirectionTransformRotationAngle < 0) {
			if (moveInXAxisOn2_5d) {
				lookDirectionTransformRotation = Vector3.up * (-90);
			} else {
				lookDirectionTransformRotation = Vector3.up * (-180);
			}
		} else {
			if (moveInXAxisOn2_5d) {
				lookDirectionTransformRotation = Vector3.up * (90);
			}
		}

		return lookDirectionTransformRotation;
	}

	public void checkEventOnMoveCamera (Vector2 currentAxisValues)
	{
		if (currentAxisValues.magnitude != 0) {
			movingCameraState = true;
			eventOnKeepMovingLockedCamera.Invoke ();
		} else {
			movingCameraState = false;
		}

		if (movingCameraState != movingCameraPrevioslyState) {
			movingCameraPrevioslyState = movingCameraState;

			if (movingCameraPrevioslyState) {
				eventOnStartMovingLockedCamera.Invoke ();
			} else {
				eventOnStopMovingLockedCamera.Invoke ();
			}
		}
	}

	public Transform getCurrentLookDirection2_5d ()
	{
		return lookCameraDirection;
	}

	public Transform getCurrentLookDirectionTopDown ()
	{
		return lookCameraDirection;
	}

	public void setCurrentLockedCameraCursor (RectTransform currentCursor)
	{
		if (currentCursor != null) {
			currentLockedCameraCursor = currentCursor;
			currentLockedCameraCursorSize = currentLockedCameraCursor.sizeDelta;

			horizontaLimit = Screen.currentResolution.width - currentLockedCameraCursorSize.x;
			verticalLimit = Screen.currentResolution.height - currentLockedCameraCursorSize.y;
		} else {
			if (currentLockedCameraCursor) {
				currentLockedCameraCursor.anchoredPosition = Vector2.zero;
				currentLockedCameraCursor = null;
			}
		}
	}

	public int sortTargetsListToLookTransformByDistance (Transform a, Transform b)
	{
		Vector2 centerScreen = new Vector2 (Screen.width / 2, Screen.height / 2);

		Vector3	newScreenPoint = Vector3.zero;

		if (usingScreenSpaceCamera) {
			newScreenPoint = mainCamera.WorldToViewportPoint (a.position);
		} else {
			newScreenPoint = mainCamera.WorldToScreenPoint (a.position);
		}

		float newDistanceX = GKC_Utils.distance (new Vector2 (newScreenPoint.x, newScreenPoint.y), centerScreen);

		if (usingScreenSpaceCamera) {
			newScreenPoint = mainCamera.WorldToViewportPoint (b.position);
		} else {
			newScreenPoint = mainCamera.WorldToScreenPoint (b.position);
		}

		float newDistanceY = GKC_Utils.distance (new Vector2 (newScreenPoint.x, newScreenPoint.y), centerScreen);

		return newDistanceX.CompareTo (newDistanceY);
	}

	public void checkChangeCurrentTargetToLook ()
	{
		if (canChangeTargetToLookWithCameraAxis && cameraCanBeUsed) {
			
			axisValues = playerInput.getPlayerMovementAxis ("mouse");

			currentAxisValuesMagnitude = Math.Abs (axisValues.magnitude);
			if (currentAxisValuesMagnitude > minimumCameraDragToChangeTargetToLook) {
				if (!selectingTargetToLookWithMouseActive) {

					float closestAngle = 360;

					float closestDistance = Mathf.Infinity;

					int targetToLookIndex = -1;

					//print (axisValues + "\n\n\n");

					List<Transform> newTargetsListToLookTransform = new List<Transform> ();

					List<Transform> placeToShootParent = new List<Transform> ();

					for (int i = 0; i < targetsListToLookTransform.Count; i++) {
						if (targetsListToLookTransform [i] != null) {
							Transform newPlaceToShoot = applyDamage.getPlaceToShoot (targetsListToLookTransform [i].gameObject);

							if (targetToLook != newPlaceToShoot) {
								if (newPlaceToShoot == null) {
									newPlaceToShoot = targetsListToLookTransform [i];
								}

								placeToShootParent.Add (targetsListToLookTransform [i]);

								newTargetsListToLookTransform.Add (newPlaceToShoot);
							}
						}
					}

					newTargetsListToLookTransform.Sort (sortTargetsListToLookTransformByDistance);

					placeToShootParent.Sort (sortTargetsListToLookTransformByDistance);
						
					for (int i = 0; i < newTargetsListToLookTransform.Count; i++) {

						Transform newPlaceToShoot = newTargetsListToLookTransform [i];
						Vector2 centerScreen = new Vector2 (Screen.width / 2, Screen.height / 2);

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (newPlaceToShoot.position);
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (newPlaceToShoot.position);
						}
							
						if (useOnlyHorizontalCameraDragValue) {
							Vector2 targeDirection = new Vector2 (screenPoint.x, screenPoint.y) - new Vector2 (centerScreen.x, centerScreen.y);

							float newAngle = Vector2.Dot (Vector2.right, targeDirection);
						
							if ((newAngle > 0 && axisValues.x > 0) || (newAngle < 0 && axisValues.x < 0)) {
									
								float newDistance = GKC_Utils.distance (new Vector2 (screenPoint.x, 0), new Vector2 (centerScreen.x, 0));

								if (newDistance < closestDistance) {
									closestDistance = newDistance;
									targetToLookIndex = i;
								}
							}
						} else {
							float newAngle = Vector2.Angle (axisValues, (new Vector2 (screenPoint.x, screenPoint.y) - centerScreen));
							//print (screenPoint + " " + newPlaceToShoot.name + " " + newPlaceToShoot.name + " " + newAngle);
						
							if (newAngle < closestAngle) {
								closestAngle = newAngle;

								targetToLookIndex = i;
							}
						}
					}

					if (newTargetsListToLookTransform.Count > 0 && targetToLookIndex > -1) {
						currentTargetToLookIndex = targetToLookIndex;

						//print ("new " + newTargetsListToLookTransform [currentTargetToLookIndex].name);

						checkTargetToLookShader (placeToShootParent [currentTargetToLookIndex]);
						pauseOutlineShaderChecking = true;

						setLookAtTargetState (true, newTargetsListToLookTransform [currentTargetToLookIndex]);

						lastTimeChangeCurrentTargetToLook = Time.time;

						pauseOutlineShaderChecking = false;
					} else {
						setLookAtTargetState (true, null);
					}

					selectingTargetToLookWithMouseActive = true;
				}
			} else {
				if (selectingTargetToLookWithMouseActive) {
					if (currentAxisValuesMagnitude < 0.09f && Time.time > lastTimeChangeCurrentTargetToLook + waitTimeToNextChangeTargetToLook) {
						selectingTargetToLookWithMouseActive = false;
					}
				}
			}
		}
	}

	void LateUpdate ()
	{
		currentLateUpdateDeltaTime = getCurrentDeltaTime ();

		//convert the mouse input in the tilt angle for the camera or the input from the touch screen depending of the settings
		if (!usedByAI) {
			if (cameraType == typeOfCamera.free) {
				if (cameraCanBeUsed) {
					if (!dead) {
						checkCameraPosition ();
					}
				}
			}

			if (lookingAtTarget) {
				usingAutoCameraRotation = true;

				if (targetToLook != null && cameraType == typeOfCamera.free) {
					checkChangeCurrentTargetToLook ();
				}
			} else { 
				usingAutoCameraRotation = false;
				if (cameraCanRotate && !gravityManager.isCharacterRotatingToSurface ()) {
//					axisValues = playerInput.getPlayerMovementAxis ("mouse");
					horizontalInput = axisValues.x;
					verticalInput = axisValues.y;

					if (horizontalInput != 0 || verticalInput != 0) {
						lastTimeCameraRotated = Time.time;
						resetingCameraActive = false;
					}

					if (playerControllerManager.isPlayerMoving (0)) {
						lastTimeCameraRotated = Time.time;
					}

					if (useEventOnFreeCamereToo) {
						checkEventOnMoveCamera (playerInput.getPlayerMovementAxis ("mouse"));
					}
				} 
			}
		} else {
			usingAutoCameraRotation = true;
		}

		if (usingAutoCameraRotation) {

			lastTimeCameraRotated = Time.time;

			if (lookingAtTarget) {
				
				lookAtTarget ();

				if (!lookintAtTargetByInput) {
					if (useTimeToStopAimAssist) {
						if (Time.time > lastTimeAimAsisst + timeToStopAimAssist) {
							setLookAtTargetState (false, null);
						}
					}
				} else {
					if (cameraType == typeOfCamera.locked) {
						if (useTimeToStopAimAssistLockedCamera && targetToLook != null) {
							if (Time.time > lastTimeAimAsisst + timeToStopAimAssistLockedCamera) {
								setTargetToLook (null);

								checkTargetToLookShader (null);
							}
						}
					}
				}
			}

			//get horizontal input direction for the camera when the lock on mode is active
			Vector3 forward = navMeshCurrentLookPos;
			float targetHorizontalValue = 0;

			float newAngle = Vector3.Angle (forward, transform.forward);

			if (newAngle >= Mathf.Abs (1)) {

				if (using2_5ViewActive) {

					targetHorizontalValue = Vector3.SignedAngle (lookDirectionTransform.forward, transform.forward, transform.up);

					targetHorizontalValue = -targetHorizontalValue * Mathf.Deg2Rad;

					targetHorizontalValue = Mathf.Clamp (targetHorizontalValue, -1, 1); 
				} else {
					Vector3 lookDelta = Vector3.zero;
					forward = forward.normalized;

					lookDelta = transform.InverseTransformDirection (forward);

					if (lookDelta.magnitude > 1) {
						lookDelta.Normalize ();
					}

					targetHorizontalValue = lookDelta.x;
				}
			}

			//get vertical input direction for the camera when the lock on mode is active
			Vector3 forwardY = navMeshCurrentLookPos;
			Vector3 lookDeltaY = Vector3.zero;
			forwardY = forwardY.normalized;

			lookDeltaY = pivotCameraTransform.InverseTransformDirection (forwardY);

			if (lookDeltaY.magnitude > 1) {
				lookDeltaY.Normalize ();
			}

			float targetVerticalValue = lookDeltaY.y;
				
			horizontalInput = targetHorizontalValue;
			verticalInput = targetVerticalValue;

			if (lookingAtTarget) {

				currentLockedCameraAngle = Vector3.Angle (mainCameraTransform.forward, navMeshCurrentLookPos);

				if (using2_5ViewActive) {
					verticalInput *= lookAtTargetSpeed2_5dView;
					horizontalInput *= lookAtTargetSpeed2_5dView;
				} else {
					if (currentLockedCameraAngle > 0.5f) {
						verticalInput *= lookAtTargetSpeed;
						horizontalInput *= lookAtTargetSpeed;
					} else {
						verticalInput *= lookCloserAtTargetSpeed;
						horizontalInput *= lookCloserAtTargetSpeed;
					}
				}
			}
		}

		if (!cameraCanRotate || !cameraCanBeUsed) {
			horizontalInput = 0;
			verticalInput = 0;
		}

		isMoving = Mathf.Abs (horizontalInput) > 0.1f || Mathf.Abs (verticalInput) > 0.1f;
		if (isMoving) {
			setLastTimeMoved ();
		}

		//if the use of the accelerometer is enabled, check the rotation of the device, to add its rotation to the x and y values, to roate the camera
		if (playerInput.isUsingTouchControls () && settings.useAcelerometer && (playerAiming || playerControllerManager.isGravityPowerActive ())) {
			//x rotates y camera axis
			acelerationAxis.x = Input.acceleration.x;

			horizontalInput += acelerationAxis.x * playerInput.rightTouchSensitivity;

			//y rotates x camera axis
			acelerationAxis.y = lowpass ().z;
			verticalInput += acelerationAxis.y * playerInput.rightTouchSensitivity;

			//accelerometer axis in left landscape
			//z righ phone
			//y up phone
			//x out phone
		}

		clampLookAngle (currentLateUpdateDeltaTime);

		if (cameraType == typeOfCamera.locked) {
			if (driving) {
				followPlayerPositionOnLockedCamera (currentLateUpdateDeltaTime);
			}
		}
	}

	public void clampLookAngle (float deltaTimeToUse)
	{
		if (cameraCanRotate && Time.deltaTime != 0 && cameraCanBeUsed) {
			//add the values from the input to the angle applied to the camera

			lookAngle.x = horizontalInput * currentHorizontalRotationSpeed;
			lookAngle.y -= verticalInput * currentVerticalRotationSpeed;

			if (zeroGravityModeOn) {
				playerLookAngle.x = horizontalInput * currentHorizontalRotationSpeed;
				playerLookAngle.y = verticalInput * currentVerticalRotationSpeed;
				if (!onGround) {
					lookAngle.y = 0;
				}

				forwardRotationAngle = Mathf.Lerp (forwardRotationAngle, targetForwardRotationAngle, (currentVerticalRotationSpeed + currentHorizontalRotationSpeed) / 2);
			}
		} else {
			lookAngle.x = 0;
		}

		//when the player is in ground after a jump or a fall, if the camera rotation is higher than the limits, it is returned to a valid rotation
		if (onGround) {
			if (adjustPivotAngle) {
				if (lookAngle.y < currentState.yLimits.x) {
					lookAngle.y += deltaTimeToUse * 250;
				}

				if (lookAngle.y > currentState.yLimits.y) {
					lookAngle.y -= deltaTimeToUse * 250;
				} else if (lookAngle.y > currentState.yLimits.x && lookAngle.y < currentState.yLimits.y) {
					adjustPivotAngle = false;
				}
			} else {
				if (horizontalCameraLimitActiveOnGround) {
					lookAngle.y = Mathf.Clamp (lookAngle.y, currentState.yLimits.x, currentState.yLimits.y);
				} else {
					lookAngle.y = Mathf.Clamp (lookAngle.y, -85, 85);
				}

				if (lookAngle.y > 360 || lookAngle.y < -360) {
					lookAngle.y = 0;
				}
			}
		}

		//restart the rotation to avoid acumulate a high value in the x axis
		else {
			if (horizontalCameraLimitActiveOnAir && !zeroGravityModeOn) {
				lookAngle.y = Mathf.Clamp (lookAngle.y, currentState.yLimits.x, currentState.yLimits.y);
			} else {
				if (lookAngle.y > 360 || lookAngle.y < -360) {
					lookAngle.y = 0;
				}
			}
		}
	}

	public void checkCurrentRotationSpeed ()
	{
		if (firstPersonActive) {
			currentVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;
		} else {
			currentVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		}
	}

	public void sethorizontalCameraLimitActiveOnAirState (bool state)
	{
		horizontalCameraLimitActiveOnAir = state;
	}

	public float getCurrentDeltaTime ()
	{
		currentDeltaTime = Time.deltaTime;

		if (regularMovementOnBulletTime) {
			currentDeltaTime *= GKC_Utils.getCurrentDeltaTime ();
		}
		return currentDeltaTime;
	}

	public void setShotCameraNoise (Vector2 noiseAmount)
	{
		shotCameraNoise = noiseAmount;
		addNoiseToCamera = true;
	}

	void FixedUpdate ()
	{
		if (!usedByAI) {
			if (!lookingAtTarget) {
				if (cameraCanRotate && !gravityManager.isCharacterRotatingToSurface ()) {
					axisValues = playerInput.getPlayerMovementAxis ("mouse");
				}
			}
		}

		currentFixedUpdateDeltaTime = getCurrentDeltaTime ();
	
		if (cameraCanBeUsed) {

			if (addNoiseToCamera) {
				horizontalInput = shotCameraNoise.x;
				verticalInput = shotCameraNoise.y;

				addNoiseToCamera = false;
				clampLookAngle (currentFixedUpdateDeltaTime);
			}

			//apply rotation to camera according to input and the state on the player
			if (useSmoothCameraRotation && ((firstPersonActive && useSmoothCameraRotationFirstPerson) || (!firstPersonActive && useSmoothCameraRotationThirdPerson))) {
				if (firstPersonActive) {
					if (useSmoothCameraRotationFirstPerson) {
						currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalFirstPerson;
						currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalFirstPerson;
					}
				} else {
					if (useSmoothCameraRotationThirdPerson) {
						currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalThirdPerson;
						currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalThirdPerson;
					}
				}

				if (resetCameraRotationAfterTime && !firstPersonActive && cameraType == typeOfCamera.free) {
					if (!resetingCameraActive && Time.time > timeToResetCameraRotation + lastTimeCameraRotated) {
						resetingCameraActive = true;
					}
				}

				if (resetingCameraActive) {
					pivotCameraTransform.localRotation = Quaternion.Lerp (pivotCameraTransform.localRotation, Quaternion.identity, resetCameraRotationSpeed * currentFixedUpdateDeltaTime);
					transform.rotation = Quaternion.Lerp (transform.rotation, playerControllerGameObject.transform.rotation, resetCameraRotationSpeed * currentFixedUpdateDeltaTime);

					float pivotCameraTransformAngle = Vector3.SignedAngle (pivotCameraTransform.forward, transform.forward, transform.right);

					float cameraTransformAngle = Vector3.SignedAngle (transform.forward, playerControllerGameObject.transform.forward, transform.up);

					if (Math.Abs (pivotCameraTransformAngle) < 1 && Math.Abs (cameraTransformAngle) < 1) {
						resetingCameraActive = false;
						lastTimeCameraRotated = Time.time;
					}

					lookAngle.y = pivotCameraTransform.localRotation.x;
				} else {
					currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

					pivotCameraTransform.localRotation = Quaternion.Lerp (pivotCameraTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * currentFixedUpdateDeltaTime);

					if (!zeroGravityModeOn || onGround) {
						currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, currentSmoothCameraRotationSpeedHorizontal * currentFixedUpdateDeltaTime);

						transform.Rotate (0, currentCameraUpRotation, 0);

					} else {
						currentCameraRotation = 
						Quaternion.Lerp (currentCameraRotation, Quaternion.Euler (-playerLookAngle.y, lookAngle.x, 0), currentSmoothCameraRotationSpeedHorizontal * currentFixedUpdateDeltaTime);

						if (canRotateForwardOnZeroGravityModeOn) {
							currentForwardRotation = Quaternion.Lerp (currentForwardRotation, Quaternion.Euler (0, 0, forwardRotationAngle), rotateForwardOnZeroGravitySpeed * currentFixedUpdateDeltaTime);

							currentCameraRotation *= currentForwardRotation;
						}

						transform.Rotate (currentCameraRotation.eulerAngles);
					}
				}
					
			} else {
				//apply the rotation to the X axis of the pivot
				pivotCameraTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
				//apply the rotation to the Y axis of the camera
				transform.Rotate (0, lookAngle.x, 0);
			}
				
			slerpCameraState (currentState, lerpState, smoothBetweenState);
		}

		if (cameraType == typeOfCamera.locked) {
			if (!driving) {
				followPlayerPositionOnLockedCamera (currentFixedUpdateDeltaTime);
			}
		}
	}

	public void followPlayerPositionOnLockedCamera (float deltaTimeToUse)
	{
		//follow player position
		if (currentLockedCameraAxisInfo.followPlayerPosition) {
			Vector3 targetPosition = Vector3.zero;

			if (currentLockedCameraAxisInfo.useWaypoints) {
				//get the closest waypoint to the player
				//when the player is close enought to that point, check the forward direction of the player, to configure the direction of the pivot
				if (!checkClosestWaypooints) {
					checkClosestWaypooints = true;
					calcultateClosestWaypoint ();
					originWaypoint = waypointBackward;
					targetWaypoint = waypointForward;
				}

				//check if player is moving forward or backward
				playerClosestWaypointForwardAngle = Vector3.Dot (targetToFollow.forward, (closestWaypoint.position - waypointForward.position));
				if (playerClosestWaypointForwardAngle < 0) {
					if (!movingForwardClosestWaypoint) {
						calcultateClosestWaypoint ();
					}
					nextWaypoint = waypointForward;
					movingForwardClosestWaypoint = true;
					print ("moving forward");
				} else {
					if (movingForwardClosestWaypoint) {
						calcultateClosestWaypoint ();
					}
					nextWaypoint = waypointBackward;
					movingForwardClosestWaypoint = false;
					print ("moving backward");
				}

				//if he is moving forward, get the distance to the closest waypoint, if he is close enough, the next couple of waypoints are obtained
				float currentDistanceToNextWaypoint = GKC_Utils.distance (lockedCameraPivot.position, nextWaypoint.position);
				if (currentDistanceToNextWaypoint < 5) {
					playerClosestWaypointForwardAngle = Vector3.Dot (targetToFollow.forward, (targetToFollow.position - nextWaypoint.position));
					//if waypoint behind player
					if (playerClosestWaypointForwardAngle > 0) {
						calcultateClosestWaypoint ();		

						assignOriginAndTarget ();
					}
				} 

				float currentDistanceToClosestWaypoint = GKC_Utils.distance (lockedCameraPivot.position, closestWaypoint.position);
				if (currentDistanceToClosestWaypoint < 1) {
					playerClosestWaypointForwardAngle = Vector3.Dot (targetToFollow.forward, (targetToFollow.position - originWaypoint.position));

					if (playerClosestWaypointForwardAngle > 0) {
						print ("target behind, recalculate");
						assignOriginAndTarget ();
					} else {
						print ("in front");
					}
				} 

				//get the current direction of both waypooint, forward or backward
				Vector3 waypointDirection = targetWaypoint.forward;

				if (currentLockedCameraAxisInfo.useNextWaypointDirection) {
					waypointDirection = targetWaypoint.position - originWaypoint.position;
					waypointDirection = waypointDirection / waypointDirection.magnitude;
				}

				//rotate the pivot toward the new waypoint
				pivotRotation = Quaternion.LookRotation (waypointDirection);

				lockedCameraPivot.rotation = Quaternion.Slerp (lockedCameraPivot.rotation, pivotRotation, deltaTimeToUse * currentLockedCameraAxisInfo.waypointCameraRotationSpeed);

				lockedMainCameraTransform.rotation = lockedCameraPivot.rotation;

				//calculate the position of the pivot according to player distance to next waypoint
				//					Vector3 relativePosition = targetWaypoint.InverseTransformPoint(targetToFollow.position);
				//					float angle = Mathf.Atan2 (relativePosition.z, relativePosition.x);
				//					float distanceToTarget = Mathf.Abs (Mathf.Cos (angle) * relativePosition.magnitude);

				//float distanceToTarget = Mathf.Abs( (targetWaypoint.position - targetToFollow.position).z);

				Vector3 waypointPosition = (Vector3.up * targetWaypoint.position.y - targetWaypoint.position);
				Vector3 playerPosition = (Vector3.up * targetToFollow.position.y - targetToFollow.position);
				playerPosition += lockedCameraPivot.right * transform.InverseTransformPoint (targetToFollow.position).x;
				float distanceToTarget = GKC_Utils.distance (waypointPosition, playerPosition);

				//float distanceToTarget = Mathf.Abs (transform.InverseTransformPoint (targetWaypoint.position - targetToFollow.position).z);
				//float distanceToTarget = Mathf.Abs(transform.InverseTransformPoint(targetToFollow.position).z - transform.InverseTransformPoint(targetWaypoint.position).z);

				print (distanceToTarget);
				targetPosition = targetWaypoint.position - waypointDirection * distanceToTarget;


				if (targetWaypoint == originWaypoint) {
					targetPosition = targetToFollow.position;
				}
			} else {
				//check if the current camera has a limit in the width, height or depth axis
				if (useCameraLimit) {
					lookCameraParent.position = targetToFollow.position;
				} else {
					lookCameraParent.localPosition = Vector3.zero;
				}

				targetPosition = getPositionToLimit (true);
			}

			//set locked pivot position smoothly
			if (currentLockedCameraAxisInfo.followPlayerPositionSmoothly) {
				if (currentLockedCameraAxisInfo.useLerpToFollowPlayerPosition) {
					lockedCameraPivot.position = Vector3.MoveTowards (lockedCameraPivot.position, targetPosition, deltaTimeToUse * currentLockedCameraAxisInfo.followPlayerPositionSpeed);
				} else {
					if (currentLockedCameraAxisInfo.useSeparatedVerticalHorizontalSpeed) {
						newVerticalPosition = Mathf.SmoothDamp (newVerticalPosition, targetPosition.y, ref newVerticalPositionVelocity, currentLockedCameraAxisInfo.verticalFollowPlayerPositionSpeed);

						if (moveInXAxisOn2_5d) {
							newHorizontalPosition = 
									Mathf.SmoothDamp (newHorizontalPosition, targetPosition.x, ref newHorizontalPositionVelocity, currentLockedCameraAxisInfo.horizontalFollowPlayerPositionSpeed);

							lockedCameraPivot.position = new Vector3 (newHorizontalPosition, newVerticalPosition, lockedCameraPivot.position.z);
						} else {
							newHorizontalPosition = 
									Mathf.SmoothDamp (newHorizontalPosition, targetPosition.z, ref newHorizontalPositionVelocity, currentLockedCameraAxisInfo.horizontalFollowPlayerPositionSpeed);

							lockedCameraPivot.position = new Vector3 (lockedCameraPivot.position.x, newVerticalPosition, newHorizontalPosition);
						}
					} else {
						lockedCameraPivot.position = Vector3.SmoothDamp (lockedCameraPivot.position, 
							targetPosition, ref lockedCameraFollowPlayerPositionVelocity, currentLockedCameraAxisInfo.followPlayerPositionSpeed);
					}
				}
				lockedMainCameraTransform.position = lockedCameraPivot.position;
			} else {
				lockedCameraPivot.position = targetPosition;
				lockedMainCameraTransform.position = targetPosition;
			}
		}
	}

	public float getHorizontalInput ()
	{
		return horizontalInput;
	}

	public float getVerticalInput ()
	{
		return verticalInput;
	}

	public Vector3 getPositionToLimit (bool calculateOnRunTime)
	{
		Vector3 newPosition = transform.position;

		//if the calculation of the position is made on update, check if the camera has an offset, that can be applied only when the player moves or also, when he moves
		if (calculateOnRunTime) {

			if (currentLockedCameraAxisInfo.useBoundToFollowPlayer) {
				newPosition = calculateBoundPosition (true);
			}

			//if the player is on 2.5d view
			if (using2_5ViewActive) {
				if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide || currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
					//check if the player is using the input
					bool playerIsUsingInput = playerControllerManager.isPlayerMovingHorizontal (currentLockedCameraAxisInfo.inputToleranceOnFaceSide);
					bool playerIsMoving = playerControllerManager.isPlayerMovingHorizontal (currentLockedCameraAxisInfo.inputToleranceOnFaceSideOnMoving);

					bool lookToRight = false;

					//Check the rotation of the player in his local Y axis to check the closest direction to look
					float currentPlayerRotationY = 0;

					Vector3 newOffset = Vector3.zero;
					Vector3 newOffsetOnMoving = Vector3.zero;

					//check the axis where he is moving, on XY or YZ
					if (moveInXAxisOn2_5d) {
						currentPlayerRotationY = Vector3.SignedAngle (targetToFollow.forward, lockedCameraPivot.right, transform.up);
					} else {
						currentPlayerRotationY = Vector3.SignedAngle (targetToFollow.forward, lockedCameraPivot.forward, transform.up);
					}

					//check if the player is moving to the left or to the right
					if (moveInXAxisOn2_5d) {
						if (Math.Abs (currentPlayerRotationY) < 90) {
							lookToRight = true;
						}
					} else {
						if (Math.Abs (currentPlayerRotationY) <= 90) {
							lookToRight = true;
						}
					}


					//add the offset to left and right according to the direction where the player is moving
					if (moveInXAxisOn2_5d) {
						if (playerIsMoving) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
								if (lookToRight) {
									newOffsetOnMoving = Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								} else {
									newOffsetOnMoving = -Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								}
							}
						} 

						if (!playerIsUsingInput) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
								if (lookToRight) {
									newOffset = Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								} else {
									newOffset = -Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								}
							}
						}
					} else {
						if (playerIsMoving) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
								if (lookToRight) {
									newOffsetOnMoving = Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								} else {
									newOffsetOnMoving = -Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								}
							}
						} 

						if (!playerIsUsingInput) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
								if (lookToRight) {
									newOffset = Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								} else {
									newOffset = -Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								}
							}
						}
					}

					//add this offset to the current camera position
					horizontalOffsetOnSide = Vector3.SmoothDamp (horizontalOffsetOnSide, newOffset, ref horizontalOffsetOnFaceSideSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideSpeed);

					horizontalOffsetOnSideOnMoving = Vector3.SmoothDamp (horizontalOffsetOnSideOnMoving, newOffsetOnMoving,
						ref horizontalOffsetOnFaceSideOnMovingSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMovingSpeed);

					newPosition += horizontalOffsetOnSide + horizontalOffsetOnSideOnMoving;
				}

				//check to add vertical offset to the camera according to vertical input on 2.5d
				if (currentLockedCameraAxisInfo.use2_5dVerticalOffsetOnMove) {
					float newVerticalInput = playerControllerManager.getRawAxisValues ().y;

					Vector3 newOffsetOnMoving = Vector3.zero;

					if (newVerticalInput > 0) {
						newOffsetOnMoving = Vector3.up * currentLockedCameraAxisInfo.verticalTopOffsetOnMove;
					} else if (newVerticalInput < 0) {
						newOffsetOnMoving = -Vector3.up * currentLockedCameraAxisInfo.verticalBottomOffsetOnMove;
					}

					verticalOffsetOnMove = Vector3.SmoothDamp (verticalOffsetOnMove, newOffsetOnMoving, ref verticalOffsetOnMoveSpeed, currentLockedCameraAxisInfo.verticalOffsetOnMoveSpeed);

					newPosition += verticalOffsetOnMove;
				}
			}

			//the player is on a top down view or similar, like isometric
			if (useTopDownView) {
				if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide || currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
					//check if the player is using the input
					bool playerIsUsingInput = playerControllerManager.isPlayerMoving (currentLockedCameraAxisInfo.inputToleranceOnFaceSide);
					bool playerIsMoving = playerControllerManager.isPlayerMoving (currentLockedCameraAxisInfo.inputToleranceOnFaceSideOnMoving);

					Vector3 newOffset = Vector3.zero;
					Vector3 newOffsetOnMoving = Vector3.zero;

					//add the offset to the camera, setting the direction of this offset as the forward direction of the player
					if (playerIsMoving) {
						if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
							newOffsetOnMoving = targetToFollow.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
						}
					} 

					if (!playerIsUsingInput) {
						if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
							newOffset = targetToFollow.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
						}
					}

					//add the offset to the camera position
					horizontalOffsetOnSide = Vector3.SmoothDamp (horizontalOffsetOnSide, newOffset, ref horizontalOffsetOnFaceSideSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideSpeed);

					horizontalOffsetOnSideOnMoving = Vector3.SmoothDamp (horizontalOffsetOnSideOnMoving, newOffsetOnMoving, 
						ref horizontalOffsetOnFaceSideOnMovingSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMovingSpeed);

					newPosition += horizontalOffsetOnSide + horizontalOffsetOnSideOnMoving;
				}
			}
		} else {
			calculateBoundPosition (false);
		}

		if (useCameraLimit) {
			if (useHeightLimit) {
				newPosition.y = Mathf.Clamp (newPosition.y, currentCameraLimitPosition.y - heightLimitLower - lockedCameraAxis.localPosition.y,
					currentCameraLimitPosition.y + heightLimitUpper - lockedCameraAxis.localPosition.y);
			}

			if (currentLockedCameraAxisInfo.moveInXAxisOn2_5d) {
				if (useWidthLimit) {
					newPosition.x = Mathf.Clamp (newPosition.x, currentCameraLimitPosition.x - widthLimitLeft - lockedCameraAxis.localPosition.x, 
						currentCameraLimitPosition.x + widthLimitRight - lockedCameraAxis.localPosition.x);
				}

				if (useDepthLimit) {
					newPosition.z = Mathf.Clamp (newPosition.z, currentCameraLimitPosition.z - depthLimitBackward, currentCameraLimitPosition.z + depthLimitFront);
				}
			} else {
				if (useWidthLimit) {
					newPosition.z = Mathf.Clamp (newPosition.z, currentCameraLimitPosition.z - widthLimitLeft, currentCameraLimitPosition.z + widthLimitRight);
				}

				if (useDepthLimit) {
					newPosition.x = Mathf.Clamp (newPosition.x, currentCameraLimitPosition.x - depthLimitBackward, currentCameraLimitPosition.x + depthLimitFront);
				}
			}
		}

		return newPosition;
	}

	public Vector3 calculateBoundPosition (bool calculateOnRunTime)
	{
		if (!calculateOnRunTime) {
			focusArea = new FocusArea (mainCollider.bounds, currentLockedCameraAxisInfo.heightBoundTop,
				currentLockedCameraAxisInfo.widthBoundRight, currentLockedCameraAxisInfo.widthBoundLeft,
				currentLockedCameraAxisInfo.depthBoundFront, currentLockedCameraAxisInfo.depthBoundBackward);
		}

		focusArea.Update (mainCollider.bounds);

		focusTargetPosition = focusArea.centre +
		Vector3.right * currentLockedCameraAxisInfo.boundOffset.x +
		Vector3.up * currentLockedCameraAxisInfo.boundOffset.y +
		Vector3.forward * currentLockedCameraAxisInfo.boundOffset.z;

		return focusTargetPosition;
	}

	public void assignOriginAndTarget ()
	{
		if (movingForwardClosestWaypoint) {
			originWaypoint = closestWaypoint;
			targetWaypoint = waypointForward;
		} else {
			originWaypoint = waypointBackward;
			targetWaypoint = closestWaypoint;
		}
	}

	public void calcultateClosestWaypoint ()
	{
		float distance = Mathf.Infinity;
		closestWaypointIndex = -1;

		//var nClosest = myTransforms.OrderBy(t=>(t.position - referencePos).sqrMagnitude).Take(3).ToArray();
		for (int i = 0; i < currentLockedCameraAxisInfo.waypointList.Count; i++) {
			float currentDistance = GKC_Utils.distance (currentLockedCameraAxisInfo.waypointList [i].position, transform.position);
			if (distance > currentDistance) {
				distance = currentDistance;
				closestWaypointIndex = i;
			}
		}

		closestWaypoint = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex];

		if (closestWaypointIndex + 1 < currentLockedCameraAxisInfo.waypointList.Count) {
			waypointForward = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex + 1];
		} else {
			waypointForward = closestWaypoint;
		}

		if (closestWaypointIndex - 1 >= 0) {
			waypointBackward = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex - 1];
		} else {
			waypointBackward = closestWaypoint;
		}
	}

	bool movingForwardClosestWaypoint;

	bool checkClosestWaypooints;

	float playerClosestWaypointForwardAngle;

	Quaternion pivotRotation;
	bool checkForwardWaypoint;
	int closestWaypointIndex;

	Transform previousClosestWaypoint;
	Transform closestWaypoint;
	Transform waypointForward;
	Transform waypointBackward;

	Transform nextWaypoint;

	Transform originWaypoint;
	Transform targetWaypoint;

	public float extraCameraCollisionDistance;

	public void checkCameraPosition ()
	{
		Vector3 dir = mainCameraTransform.position - pivotCameraTransform.position;
		float dist = GKC_Utils.distance (currentState.camPositionOffset, Vector3.zero);
		if (Physics.SphereCast (pivotCameraTransform.position, maxCheckDist, dir, out hit, dist + extraCameraCollisionDistance, settings.layer)) {
			Debug.DrawLine (pivotCameraTransform.position, pivotCameraTransform.position + (dir.normalized * hit.distance), Color.green);
			Vector3 targetCameraPosition = pivotCameraTransform.position + (dir.normalized * hit.distance);

//			targetCameraPosition = targetCameraPosition - transform.up * transform.InverseTransformDirection (targetCameraPosition).y;
//
//			targetCameraPosition += (currentState.pivotPositionOffset.y + currentState.camPositionOffset.y) * transform.up;

			//mainCameraTransform.position = targetCameraPosition;

			Vector3 newPosition = pivotCameraTransform.InverseTransformPoint (targetCameraPosition);

			mainCameraTransform.localPosition = new Vector3 (newPosition.x, currentState.camPositionOffset.y, newPosition.z);

			//print (mainCameraTransform.localPosition);

		} else {
			Debug.DrawLine (pivotCameraTransform.position, pivotCameraTransform.position + (dir.normalized * dist), Color.red);
			Vector3 mainCamPos = mainCameraTransform.localPosition;
			Vector3 newPos = Vector3.Lerp (mainCamPos, currentState.camPositionOffset, getCurrentDeltaTime () * movementLerpSpeed);
			mainCameraTransform.localPosition = newPos;
		}
	}

	bool previouslyInFirstPerson;

	public void setCameraToFreeOrLocked (typeOfCamera state, lockedCameraSystem.cameraAxis lockedCameraInfo)
	{
		if (state == typeOfCamera.free) {
			if (cameraType != state) {

				if (driving) {
					cameraType = state;

					cameraCurrentlyLocked = false;

					previousLockedCameraAxisTransform = null;
					playerControllerManager.setLockedCameraState (false, false, false);

					if (usingPlayerNavMeshPreviously) {
						playerNavMeshManager.setPlayerNavMeshEnabledState (false);
					}
					usingPlayerNavMeshPreviously = false;

					//check if the player is driving and set the locked camera properly
					vehicleHUDManager currentVehicleHUDManager = playerControllerManager.getCurrentVehicle ().GetComponent<vehicleHUDManager> ();
					if (currentVehicleHUDManager) {
						currentVehicleHUDManager.setPlayerCameraParentAndPosition (mainCamera.transform, this);
					}
				} else {
					
					currentLockedCameraAxisInfo = lockedCameraInfo;

					if (previousFreeCameraStateName != "") {

						setCameraState (previousFreeCameraStateName);
						resetCurrentCameraStateAtOnce ();

						previousFreeCameraStateName = "";
					}

					transform.eulerAngles = new Vector3 (transform.eulerAngles.x, lockedCameraInfo.axis.eulerAngles.y, transform.eulerAngles.z);
					pivotCameraTransform.eulerAngles = new Vector3 (lockedCameraInfo.axis.localEulerAngles.x, pivotCameraTransform.eulerAngles.y, pivotCameraTransform.eulerAngles.z);
					mainCamera.transform.SetParent (mainCameraTransform);

					if (currentLockedCameraAxisInfo.smoothCameraTransition) {
						lockedCameraMovement (false);
						if (previousLockedCameraAxisInfo.useDifferentCameraFov || usingLockedZoomOn) {
							setMainCameraFov (currentState.initialFovValue, zoomSpeed);
						}
					} else {
						cameraType = state;

						cameraCurrentlyLocked = false;

						mainCamera.transform.localPosition = Vector3.zero;
						mainCamera.transform.localRotation = Quaternion.identity;
						if (previousLockedCameraAxisInfo.useDifferentCameraFov || usingLockedZoomOn) {
							mainCamera.fieldOfView = currentState.initialFovValue;
						}
					}
			
					lookAngle = Vector2.zero;
					previousLockedCameraAxisTransform = null;
					playerControllerManager.setLockedCameraState (false, false, false);

					if (usingPlayerNavMeshPreviously) {
						playerNavMeshManager.setPlayerNavMeshEnabledState (false);
					}
					usingPlayerNavMeshPreviously = false;

					if (previouslyInFirstPerson) {
						changeCameraToThirdOrFirstView (false);
					} else {
						if (weaponsManager.isAimingWeapons ()) {
							activateAiming (powersManager.aimsettings.aimSide); 
						}
					}
				}

				setLookAtTargetState (false, null);

				//check the unity events on enter and exit
				callLockedCameraEventOnEnter (lockedCameraInfo);

				if (previousLockedCameraAxisInfo != null) {
					callLockedCameraEventOnExit (previousLockedCameraAxisInfo);
				}

				if (previousLockedCameraAxisInfo.changeRootMotionActive) {
					playerControllerManager.setOriginalUseRootMotionActiveState ();
				}

				previousLockedCameraAxisInfo = null;

				useCameraLimit = false;

				if (usingSetTransparentSurfacesPreviously) {
					setTransparentSurfacesManager.setCheckSurfaceEnabled (false);
				}
			}
		} 

		if (state == typeOfCamera.locked) {
			//assign the new locked camera info

			currentLockedCameraAxisInfo = lockedCameraInfo;

			bool newCameraFound = false;

			if (previousLockedCameraAxisInfo == null || previousLockedCameraAxisInfo != currentLockedCameraAxisInfo) {
				newCameraFound = true;
			}

			print ("New locked camera found: " + newCameraFound + " " + currentLockedCameraAxisInfo.name);

			//if a new camera is found, adjust the position of the locked camera on the player
			if (!newCameraFound) {
				return;
			}

			mainCamera.transform.SetParent (null);

			//set the position and rotations of the new locked camera transform to the previous locked transform elements of the player
			lockedCameraPosition.localPosition = currentLockedCameraAxisInfo.cameraPosition.localPosition;
			lockedCameraPosition.localRotation = currentLockedCameraAxisInfo.cameraPosition.localRotation;

			lockedCameraAxis.localPosition = currentLockedCameraAxisInfo.axis.localPosition;
			lockedCameraAxis.localRotation = currentLockedCameraAxisInfo.axis.localRotation;

			//if the is a locked camera pivot, it means the camera follows the player position on this locked view
			if (currentLockedCameraAxisInfo.lockedCameraPivot) {
				lockedCameraPivot.rotation = currentLockedCameraAxisInfo.lockedCameraPivot.rotation;

				//place the new camera found in the current position of the player to have a smoother transition between cameras that are following the player positition
				if (currentLockedCameraAxisInfo.useZeroCameraTransition) {
					lockedCameraPivot.position = targetToFollow.position;

					lockedCameraPivot.position = getPositionToLimit (false);
				} else {
					lockedCameraPivot.position = currentLockedCameraAxisInfo.lockedCameraPivot.position;
				}
			} else {
				//else, the camera will stay in a fixed position

				lockedCameraAxis.position = currentLockedCameraAxisInfo.axis.position;
				lockedCameraAxis.rotation = currentLockedCameraAxisInfo.axis.rotation;

				lockedCameraPosition.position = currentLockedCameraAxisInfo.cameraPosition.position;
				lockedCameraPosition.rotation = currentLockedCameraAxisInfo.cameraPosition.rotation;
			}

			lookCameraParent.localPosition = Vector3.zero;
			lookCameraParent.localRotation = Quaternion.identity;

			if (!cameraCurrentlyLocked) {
				previousLockedCameraAxisTransform = getCameraTransform ();
				previouslyInFirstPerson = isFirstPersonActive ();
				if (previouslyInFirstPerson) {
					changeCameraToThirdOrFirstView (true);
				}

				cameraCurrentlyLocked = true;
			}

			if (previousLockedCameraAxisTransform != null) {
				setCurrentAxisTransformValues (previousLockedCameraAxisTransform);
			}

			lockedCameraCanFollow = false;

			//2.5d camera setting
			if (currentLockedCameraAxisInfo.use2_5dView) {

				lookCameraPivot.localPosition = currentLockedCameraAxisInfo.pivot2_5d.localPosition;
				lookCameraPivot.localRotation = currentLockedCameraAxisInfo.pivot2_5d.localRotation;

				lookCameraDirection.localPosition = currentLockedCameraAxisInfo.lookDirection2_5d.localPosition;
				lookCameraDirection.localRotation = currentLockedCameraAxisInfo.lookDirection2_5d.localRotation;

				playerControllerManager.set3dOr2_5dWorldType (false);
				using2_5ViewActive = true;

				originalLockedCameraPivotPosition = currentLockedCameraAxisInfo.originalLockedCameraPivotPosition;
				moveInXAxisOn2_5d = currentLockedCameraAxisInfo.moveInXAxisOn2_5d;

				if (currentLockedCameraAxisInfo.useDefaultZValue2_5d) {
					movePlayerToDefaultHorizontalValue2_5d ();
				}

				clampAimDirections = currentLockedCameraAxisInfo.clampAimDirections;
				numberOfAimDirections = (int)currentLockedCameraAxisInfo.numberOfAimDirections;
			} else {
				playerControllerManager.set3dOr2_5dWorldType (true);
				using2_5ViewActive = false;
			}

			horizontalCameraLimitActiveOnGround = !using2_5ViewActive;

			//point and click setting
			if (currentLockedCameraAxisInfo.usePointAndClickSystem) {
				if (!usingPlayerNavMeshPreviously) {
					playerNavMeshManager.setPlayerNavMeshEnabledState (true);
				}
				usingPlayerNavMeshPreviously = true;
			} else {
				if (usingPlayerNavMeshPreviously) {
					playerNavMeshManager.setPlayerNavMeshEnabledState (true);
				}
				usingPlayerNavMeshPreviously = false;
			}


			//top down setting
			if (currentLockedCameraAxisInfo.useTopDownView) {

				lookCameraPivot.localPosition = currentLockedCameraAxisInfo.topDownPivot.localPosition;
				lookCameraPivot.localRotation = currentLockedCameraAxisInfo.topDownPivot.localRotation;

				lookCameraDirection.localPosition = currentLockedCameraAxisInfo.topDownLookDirection.localPosition;
				lookCameraDirection.localRotation = currentLockedCameraAxisInfo.topDownLookDirection.localRotation;
			
				useTopDownView = true;
			} else {
				useTopDownView = false;
			}
		
			cameraType = state;

			mainCamera.transform.SetParent (lockedCameraPosition);

			if (currentLockedCameraAxisInfo.smoothCameraTransition) {
				lockedCameraMovement (true);
				if (currentLockedCameraAxisInfo.useDifferentCameraFov) {
					setMainCameraFov (currentLockedCameraAxisInfo.fovValue, zoomSpeed);
				}

				if (previousLockedCameraAxisInfo != null) {
					if (usingLockedZoomOn) {
						setMainCameraFov (currentState.initialFovValue, zoomSpeed);
					}
				}
			} else {
				mainCamera.transform.localPosition = Vector3.zero;
				mainCamera.transform.localRotation = Quaternion.identity;
				lockedCameraChanged = true;
				if (currentLockedCameraAxisInfo.useDifferentCameraFov) {
					mainCamera.fieldOfView = currentLockedCameraAxisInfo.fovValue;
				}

				if (previousLockedCameraAxisInfo != null) {
					if (usingLockedZoomOn) {
						mainCamera.fieldOfView = currentState.initialFovValue;
					}
				}
			}
				
			playerControllerManager.setLockedCameraState (true, currentLockedCameraAxisInfo.useTankControls, currentLockedCameraAxisInfo.useRelativeMovementToLockedCamera);

			if (!previouslyInFirstPerson && weaponsManager.isUsingWeapons ()) {
				activateAiming (powersManager.aimsettings.aimSide); 
			}

			if (previousLockedCameraAxisInfo != null) {
				if (usingLockedZoomOn || previousLockedCameraAxisInfo.cameraCanRotate || previousLockedCameraAxisInfo.canMoveCamera) {

					//Reset locked camera values on this player camera
					currentLockedLoonAngle = Vector2.zero;
					currentLockedCameraRotation = Quaternion.identity;
					currentLockedPivotRotation = Quaternion.identity;
					lastTimeLockedSpringRotation = 0;
					lastTimeLockedSpringMovement = 0;
					currentLockedCameraMovementPosition = Vector3.zero;
					currentLockedMoveCameraPosition = Vector3.zero;

					usingLockedZoomOn = false;
				}
			}

			//Assign the previous locked camera and call the event on exit
			if (previousLockedCameraAxisInfo != currentLockedCameraAxisInfo) {
				previousLockedCameraAxisInfo = currentLockedCameraAxisInfo;

				//check the unity events on exit
				callLockedCameraEventOnExit (previousLockedCameraAxisInfo);
			}

			//check the unity events on enter
			callLockedCameraEventOnEnter (currentLockedCameraAxisInfo);

			//set the current vertical and horizontal position of the camera in case the following speed is separated into these two values, so the position is calculated starting at that point
			if (currentLockedCameraAxisInfo.lockedCameraPivot) {
				newVerticalPosition = lockedCameraPivot.position.y;

				if (moveInXAxisOn2_5d) {
					newHorizontalPosition = lockedCameraPivot.position.x;
				} else {
					newHorizontalPosition = lockedCameraPivot.position.z;
				}
			}

			if (currentLockedCameraAxisInfo.lookAtPlayerPosition) {
				calculateLockedCameraLookAtPlayerPosition ();

				lockedCameraPosition.localRotation = Quaternion.Euler (new Vector3 (currentLockedLimitLookAngle.x, 0, 0));

				lockedCameraAxis.localRotation = Quaternion.Euler (new Vector3 (0, currentLockedLimitLookAngle.y, 0));
			}

			horizontalOffsetOnSide = Vector3.zero;

			horizontalOffsetOnSideOnMoving = Vector3.zero;

			verticalOffsetOnMove = Vector3.zero;

			if (currentLockedCameraAxisInfo.useTransparetSurfaceSystem) {
				setTransparentSurfacesManager.setCheckSurfaceEnabled (true);
			} else {
				setTransparentSurfacesManager.setCheckSurfaceEnabled (false);
			}

			usingSetTransparentSurfacesPreviously = currentLockedCameraAxisInfo.useTransparetSurfaceSystem;

			if (previousFreeCameraStateName == "") {

				if (isFirstPersonActive ()) {
					previousFreeCameraStateName = defaultFirstPersonStateName;
				} else {
					previousFreeCameraStateName = defaultThirdPersonStateName;
				}

				setCameraState (defaultLockedCameraStateName);
				resetCurrentCameraStateAtOnce ();
			}

			if (currentLockedCameraAxisInfo.changeRootMotionActive) {
				playerControllerManager.setUseRootMotionActiveState (currentLockedCameraAxisInfo.useRootMotionActive);
			}
		}
	}

	public void setClampAimDirectionsState (bool state)
	{
		clampAimDirections = state;
	}

	public void calculateLockedCameraLookAtPlayerPosition ()
	{
		Vector3 cameraAxisPosition = lockedCameraPosition.position;
		if (currentLockedCameraAxisInfo.usePositionOffset) {
			cameraAxisPosition += lockedCameraAxis.transform.right * currentLockedCameraAxisInfo.positionOffset.x;
			cameraAxisPosition += lockedCameraAxis.transform.up * currentLockedCameraAxisInfo.positionOffset.y;
			cameraAxisPosition += lockedCameraAxis.transform.forward * currentLockedCameraAxisInfo.positionOffset.z;
		}

		Vector3 lookPos = targetToFollow.position - cameraAxisPosition;
		Quaternion rotation = Quaternion.LookRotation (lookPos);
		Vector3 rotatioEuler = rotation.eulerAngles;
		float lockedCameraPivotY = lockedCameraPivot.localEulerAngles.y;
		currentLockedLimitLookAngle.x = rotatioEuler.x;
		currentLockedLimitLookAngle.y = rotatioEuler.y - lockedCameraPivotY;

		if (currentLockedCameraAxisInfo.useRotationLimits) {
			if (currentLockedLimitLookAngle.x > 180) {
				currentLockedLimitLookAngle.x -= 360;				
				currentLockedLimitLookAngle.x = Mathf.Clamp (currentLockedLimitLookAngle.x, currentLockedCameraAxisInfo.rotationLimitsX.x, 0);
			} else {
				currentLockedLimitLookAngle.x = Mathf.Clamp (currentLockedLimitLookAngle.x, currentLockedCameraAxisInfo.rotationLimitsX.x, currentLockedCameraAxisInfo.rotationLimitsX.y);
			}

			currentLockedLimitLookAngle.y = Mathf.Clamp (currentLockedLimitLookAngle.y, currentLockedCameraAxisInfo.rotationLimitsY.x, currentLockedCameraAxisInfo.rotationLimitsY.y);
		} 
	}

	public void setCameraLimit (bool useCameraLimitValue, bool useWidthLimitValue, float newWidthLimitRight, float newWidthLimitLeft, bool useHeightLimitValue, float newHeightLimitUpper,
	                            float newHeightLimitLower, Vector3 newCameraLimitPosition, bool depthLimitEnabled, float newDepthLimitFront, float newDepthLimitBackward)
	{
		useCameraLimit = useCameraLimitValue;

		currentCameraLimitPosition = newCameraLimitPosition;

		useWidthLimit = useWidthLimitValue;
		widthLimitRight = newWidthLimitRight;
		widthLimitLeft = newWidthLimitLeft;

		useHeightLimit = useHeightLimitValue;
		heightLimitUpper = newHeightLimitUpper;
		heightLimitLower = newHeightLimitLower;
			
		useDepthLimit = depthLimitEnabled;
		depthLimitFront = newDepthLimitFront;
		depthLimitBackward = newDepthLimitBackward;
	}

	public void setNewCameraForwardPosition (float newCameraForwardPosition)
	{
		mainCamera.transform.SetParent (null);

		Vector3 originalCameraAxisLocalPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;
		if (moveInXAxisOn2_5d) {
			lockedCameraAxis.localPosition = new Vector3 (lockedCameraAxis.localPosition.x, lockedCameraAxis.localPosition.y, newCameraForwardPosition);
			currentLockedCameraAxisInfo.originalCameraAxisLocalPosition = new Vector3 (originalCameraAxisLocalPosition.x, originalCameraAxisLocalPosition.y, newCameraForwardPosition);
		} else {
			lockedCameraAxis.localPosition = new Vector3 (newCameraForwardPosition, lockedCameraAxis.localPosition.y, lockedCameraAxis.localPosition.z);
			currentLockedCameraAxisInfo.originalCameraAxisLocalPosition = new Vector3 (newCameraForwardPosition, originalCameraAxisLocalPosition.y, originalCameraAxisLocalPosition.z);
		}

		mainCamera.transform.SetParent (lockedCameraPosition);

		lockedCameraMovement (true);
	}

	public bool isTopdownViewEnabled ()
	{
		return useTopDownView;
	}

	public bool is2_5ViewActive ()
	{
		return using2_5ViewActive;
	}

	public void callLockedCameraEventOnEnter (lockedCameraSystem.cameraAxis cameraAxisToCheck)
	{
		if (cameraAxisToCheck.useUnityEvent && cameraAxisToCheck.useUnityEventOnEnter) {
			if (cameraAxisToCheck.unityEventOnEnter.GetPersistentEventCount () > 0) {
				cameraAxisToCheck.unityEventOnEnter.Invoke ();
			}
		}
	}

	public void callLockedCameraEventOnExit (lockedCameraSystem.cameraAxis cameraAxisToCheck)
	{
		if (cameraAxisToCheck.useUnityEvent && cameraAxisToCheck.useUnityEventOnExit) {
			if (cameraAxisToCheck.unityEventOnExit.GetPersistentEventCount () > 0) {
				cameraAxisToCheck.unityEventOnExit.Invoke ();
			}
		}
	}

	public bool isMoveInXAxisOn2_5d ()
	{
		return moveInXAxisOn2_5d;
	}

	public Vector3 getOriginalLockedCameraPivotPosition ()
	{
		return originalLockedCameraPivotPosition;
	}
		
	//Adjust the player to a fixed axis position for the 2.5d view
	public void movePlayerToDefaultHorizontalValue2_5d ()
	{
		if (moveInXAxisOn2_5d) {
			if (targetToFollow.position.z == originalLockedCameraPivotPosition.z) {
				return;
			}
		} else {
			if (targetToFollow.position.x == originalLockedCameraPivotPosition.x) {
				return;
			}
		}

		if (fixPlayerZPositionCoroutine != null) {
			StopCoroutine (fixPlayerZPositionCoroutine);
		}
		fixPlayerZPositionCoroutine = StartCoroutine (movePlayerToDefaultHorizontalValue2_5dCoroutine ());
	}

	IEnumerator movePlayerToDefaultHorizontalValue2_5dCoroutine ()
	{
		playerControllerManager.changeScriptState (false);

		Vector3 targetPosition = Vector3.zero;

		Vector3 currentPosition = targetToFollow.position;
		if (moveInXAxisOn2_5d) {
			targetPosition = new Vector3 (currentPosition.x, currentPosition.y, originalLockedCameraPivotPosition.z);
		} else {
			targetPosition = new Vector3 (originalLockedCameraPivotPosition.x, currentPosition.y, currentPosition.z);
		}
			
		float i = 0;

		while (i < 1) {
			i += getCurrentDeltaTime () * 5;
			targetToFollow.position = Vector3.Lerp (currentPosition, targetPosition, i);
			yield return null;
		}

		playerControllerManager.changeScriptState (true);
	}

	public void lockedCameraMovement (bool isBeingSetToLocked)
	{
		if (lockedCameraCoroutine != null) {
			StopCoroutine (lockedCameraCoroutine);
		}
		lockedCameraCoroutine = StartCoroutine (lockedCameraMovementCoroutine (isBeingSetToLocked));
	}

	//move the camera from its position in player camera to a fix position
	IEnumerator lockedCameraMovementCoroutine (bool isBeingSetToLocked)
	{
		lockedCameraMoving = true;
		float i = 0;
		//store the current rotation of the camera
		Quaternion currentQ = mainCamera.transform.localRotation;
		//store the current position of the camera
		Vector3 currentPos = mainCamera.transform.localPosition;
		//translate position and rotation camera
		while (i < 1) {
			i += getCurrentDeltaTime () * currentLockedCameraAxisInfo.cameraTransitionSpeed;
			mainCamera.transform.localRotation = Quaternion.Lerp (currentQ, Quaternion.identity, i);
			mainCamera.transform.localPosition = Vector3.Lerp (currentPos, Vector3.zero, i);
			yield return null;
		}

		if (isBeingSetToLocked) {
			lockedCameraChanged = true;
		} else {
			cameraType = typeOfCamera.free;
			cameraCurrentlyLocked = false;
		}
		lockedCameraMoving = false;
	}

	public void setCurrentAxisTransformValues (Transform newValues)
	{
		lockedMainCameraTransform.position = newValues.position;
		lockedMainCameraTransform.eulerAngles = new Vector3 (0, newValues.eulerAngles.y, 0);
	}

	public Transform getLockedCameraTransform ()
	{
		return lockedMainCameraTransform;
	}

	public void setLockedMainCameraTransformRotation (Vector3 normal)
	{
		if (!cameraCurrentlyLocked) {
			return;
		}

		//print (normal);

		Vector3 previousLockedCameraAxisPosition = Vector3.zero;
		Vector3 lockedCameraAxisLocalPosition = Vector3.zero;
		Quaternion previousLockedCameraPivotRotation = lockedCameraPivot.rotation;

		if (moveInXAxisOn2_5d) {
			Quaternion targetRotation = Quaternion.LookRotation (Vector3.forward, normal);
			float rotationAmount = targetRotation.eulerAngles.z;
		
			lockedCameraPivot.eulerAngles = new Vector3 (0, 0, rotationAmount);
		} else {
			Quaternion targetRotation = Quaternion.LookRotation (Vector3.right, normal);
			float rotationAmount = targetRotation.eulerAngles.x;

			lockedCameraPivot.eulerAngles = new Vector3 (rotationAmount, 0, 0);
		}

		float lookCameraParentTargetAngle = 0;
		float lockedMainCameraTransformTargetAngle = 0;
		if (normal != Vector3.up) {
			float currentUpAngle = 0;
			float currentRightAngle = 0;

			if (moveInXAxisOn2_5d) {
				currentUpAngle = Vector3.SignedAngle (normal, Vector3.up, Vector3.forward);
				currentRightAngle = Vector3.SignedAngle (normal, Vector3.right, Vector3.forward);
			} else {
				currentUpAngle = Vector3.SignedAngle (normal, Vector3.up, Vector3.right);
				currentRightAngle = Vector3.SignedAngle (normal, Vector3.right, Vector3.right);
			}

			//print (currentUpAngle + " " + currentRightAngle);

			lockedMainCameraTransformTargetAngle = currentUpAngle;
			lookCameraParentTargetAngle = currentUpAngle;

			if (currentUpAngle < 0) {
				lockedMainCameraTransformTargetAngle = -currentUpAngle;
				lookCameraParentTargetAngle = -currentUpAngle;
			}

			if (currentUpAngle < -90) {
				lockedMainCameraTransformTargetAngle = currentUpAngle + 90;
			}

			if (currentUpAngle > 175 && currentUpAngle < 185) {
				lockedMainCameraTransformTargetAngle = 0;
			}

			if (currentUpAngle > 90 && currentUpAngle < 175) {
				lockedMainCameraTransformTargetAngle = 90 - currentRightAngle;
				lookCameraParentTargetAngle = 360 - currentUpAngle;
			}

			if (currentUpAngle > 85 && currentUpAngle < 95) {
				lookCameraParentTargetAngle = -currentUpAngle;
			}

			if (currentUpAngle > 0 && currentUpAngle < 85) {
				lockedMainCameraTransformTargetAngle = -currentUpAngle;
				lookCameraParentTargetAngle = -currentUpAngle;
			}

		}

		lockedMainCameraTransform.eulerAngles = new Vector3 (lockedMainCameraTransform.eulerAngles.x, lockedMainCameraTransform.eulerAngles.y, lockedMainCameraTransformTargetAngle);

		lookCameraParent.localRotation = Quaternion.Euler (new Vector3 (0, 0, lookCameraParentTargetAngle));



		auxLockedCameraAxis.localPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;

		previousLockedCameraAxisPosition = auxLockedCameraAxis.position;

		lockedCameraPivot.rotation = previousLockedCameraPivotRotation;

		auxLockedCameraAxis.position = previousLockedCameraAxisPosition;

		lockedCameraAxisLocalPosition = auxLockedCameraAxis.localPosition;

		if (moveInXAxisOn2_5d) {
			setLockedCameraAxisPositionOnGravityChange (
				new Vector3 (lockedCameraAxisLocalPosition.x, lockedCameraAxisLocalPosition.y, currentLockedCameraAxisInfo.originalCameraAxisLocalPosition.z));
		} else {
			setLockedCameraAxisPositionOnGravityChange (
				new Vector3 (currentLockedCameraAxisInfo.originalCameraAxisLocalPosition.x, lockedCameraAxisLocalPosition.y, lockedCameraAxisLocalPosition.z));
		}
	}

	Coroutine adjustLockedCameraAxisPositionOnGravityChangeCoroutine;

	public void setLockedCameraAxisPositionOnGravityChange (Vector3 newPosition)
	{
		if (adjustLockedCameraAxisPositionOnGravityChangeCoroutine != null) {
			StopCoroutine (adjustLockedCameraAxisPositionOnGravityChangeCoroutine);
		}
		adjustLockedCameraAxisPositionOnGravityChangeCoroutine = StartCoroutine (setLockedCameraAxisPositionOnGravityChangeCoroutine (newPosition));
	}

	//move the camera from its position in player camera to a fix position
	IEnumerator setLockedCameraAxisPositionOnGravityChangeCoroutine (Vector3 newPosition)
	{
		float i = 0;
		Vector3 currentPos = lockedCameraAxis.localPosition;
		while (i < 1) {
			i += getCurrentDeltaTime () * currentLockedCameraAxisInfo.cameraTransitionSpeed;
			lockedCameraAxis.localPosition = Vector3.Lerp (lockedCameraAxis.localPosition, newPosition, i);
			yield return null;
		}
	}

	public void setPlayerAndCameraParent (Transform newParent)
	{
		playerControllerGameObject.transform.SetParent (newParent);
		transform.SetParent (newParent);

		if (newParent == null && gravityManager.getCurrentNormal () == Vector3.up) {
			playerControllerGameObject.transform.rotation = Quaternion.Euler (Vector3.Scale (gravityManager.getCurrentNormal (), playerControllerGameObject.transform.eulerAngles));
			transform.rotation = Quaternion.Euler (Vector3.Scale (gravityManager.getCurrentNormal (), transform.eulerAngles));
		}

		if (cameraType == typeOfCamera.locked && currentLockedCameraAxisInfo.followPlayerPosition) {
			if (newParent != null) {
				lockedCameraPivot.SetParent (newParent);
			} else {
				lockedCameraPivot.SetParent (lockedCameraElementsParent);
			}
		}
	}

	public Transform getCurrentLockedCameraTransform ()
	{
		return lockedCameraPosition;
	}

	public Transform getCameraTransform ()
	{
		return mainCameraTransform;
	}

	public Camera getMainCamera ()
	{
		return mainCamera;
	}

	public Transform getPivotCameraTransform ()
	{
		return pivotCameraTransform;
	}

	//Aim assist functions, and get the closest object to look
	public bool getClosestTargetToLook ()
	{
		bool targetFound = false;

		targetsListToLookTransform.Clear ();
		List<Collider> targetsListCollider = new List<Collider> ();

		List<GameObject> targetist = new List<GameObject> ();
		List<GameObject> fullTargetList = new List<GameObject> ();
	
		if (useLayerToSearchTargets) {
			targetsListCollider.AddRange (Physics.OverlapSphere (transform.position, maxDistanceToFindTarget, layerToLook));
			for (int i = 0; i < targetsListCollider.Count; i++) {
				fullTargetList.Add (targetsListCollider [i].gameObject);
			}
		} else {
			for (int i = 0; i < tagToLookList.Count; i++) {
				GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLookList [i]);
				targetist.AddRange (enemiesList);
			}

			for (int i = 0; i < targetist.Count; i++) {	
				float distance = GKC_Utils.distance (targetist [i].transform.position, transform.position);
				if (distance < maxDistanceToFindTarget) {
					fullTargetList.Add (targetist [i]);
				}
			}
		}

		List<GameObject> pointToLookComponentList = new List<GameObject> ();

		if (searchPointToLookComponents) {
			targetsListCollider.Clear ();
			targetsListCollider.AddRange (Physics.OverlapSphere (transform.position, maxDistanceToFindTarget, pointToLookComponentsLayer));
			for (int i = 0; i < targetsListCollider.Count; i++) {
				if (targetsListCollider [i].isTrigger) {
					pointToLook currentPointToLook = targetsListCollider [i].GetComponent<pointToLook> ();
					if (currentPointToLook) {
						GameObject currenTargetToLook = currentPointToLook.getPointToLookTransform ().gameObject;
						fullTargetList.Add (currenTargetToLook);
						pointToLookComponentList.Add (currenTargetToLook);
					}
				}
			}
		}

		for (int i = 0; i < fullTargetList.Count; i++) {
			if (fullTargetList [i] != null) {
				GameObject currentTarget = fullTargetList [i];
				if (tagToLookList.Contains (currentTarget.tag) || pointToLookComponentList.Contains (currentTarget)) {
					bool objectVisible = false;
					bool obstacleDetected = false;

					Vector3 targetPosition = currentTarget.transform.position;
					if (lookOnlyIfTargetOnScreen) {
						Transform currentTargetPlaceToShoot = applyDamage.getPlaceToShoot (currentTarget);
						if (currentTargetPlaceToShoot != null) {
							targetPosition = currentTargetPlaceToShoot.position;
						}

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (targetPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (targetPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
						}

						//the target is visible in the screen
						if (targetOnScreen) {
							objectVisible = true;
						}
					} else {
						objectVisible = true;
					}

					if (objectVisible && checkObstaclesToTarget) {
						//for every target in front of the camera, use a raycast, if it finds an obstacle between the target and the camera, the target is removed from the list
						Vector3 direction = targetPosition - lookAtTargetTransform.position;
						direction = direction / direction.magnitude;
						float distance = GKC_Utils.distance (targetPosition, lookAtTargetTransform.position);
						if (Physics.Raycast (targetPosition, -direction, out hit, distance, layerToLook)) {
							obstacleDetected = true;
						}
					} 

					if (objectVisible && !obstacleDetected) {
						targetsListToLookTransform.Add (currentTarget.transform);
					}
				} 
			}
		}
	
		//finally, get the target closest to the player
		float minDistance = Mathf.Infinity;
		Vector3 centerScreen = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		for (int i = 0; i < targetsListToLookTransform.Count; i++) {
			//find closes element to center screen
			if (getClosestToCameraCenter) {
				targetPosition = targetsListToLookTransform [i].position;
				placeToShoot = applyDamage.getPlaceToShoot (targetsListToLookTransform [i].gameObject);
				if (placeToShoot != null) {
					targetPosition = placeToShoot.position;
				}

				screenPoint = mainCamera.WorldToScreenPoint (targetPosition);

				float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);
				bool canBeChecked = false;
				if (useMaxDistanceToCameraCenter && !lookAtBodyPartsOnCharacters) {
					if (currentDistance < maxDistanceToCameraCenter) {
						canBeChecked = true;
					}
				} else {
					canBeChecked = true;
				}

				//print (currentDistance +" "+ canBeChecked);

				if (canBeChecked) {
					//print (currentDistance + " " + minDistance);
					if (currentDistance < minDistance) {
						minDistance = currentDistance;
						setTargetToLook (targetsListToLookTransform [i]);
						targetFound = true;
					}
				}
			} else {
				float currentDistance = GKC_Utils.distance (targetsListToLookTransform [i].position, transform.position);
				if (currentDistance < minDistance) {
					minDistance = currentDistance;
					setTargetToLook (targetsListToLookTransform [i]);
					targetFound = true;
				}
			}
		}
	
		if (targetFound) {

			checkTargetToLookShader (targetToLook);

			//print (targetToLook.name);

			bool bodyPartFound = false;
			if (lookAtBodyPartsOnCharacters) {

				Transform bodyPartToLook = targetToLook;

				List<health.weakSpot> characterWeakSpotList = applyDamage.getCharacterWeakSpotList (targetToLook.gameObject);

				if (characterWeakSpotList != null) {
					minDistance = Mathf.Infinity;
					for (int i = 0; i < characterWeakSpotList.Count; i++) {
						if (bodyPartsToLook.Contains (characterWeakSpotList [i].name)) {

							screenPoint = mainCamera.WorldToScreenPoint (characterWeakSpotList [i].spotTransform.position);
							float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);

							//print ("distance to body part " + currentDistance + " "+useMaxDistanceToCameraCenter);
							bool canBeChecked = false;
							if (useMaxDistanceToCameraCenter) {
								if (currentDistance < maxDistanceToCameraCenter) {
									canBeChecked = true;
								}
							} else {
								canBeChecked = true;
							}

							if (canBeChecked) {
								if (currentDistance < minDistance) {
									minDistance = currentDistance;
									bodyPartToLook = characterWeakSpotList [i].spotTransform;

									bodyPartFound = true;
								}
							}
						}
					}

					//print (bodyPartToLook.name);
					setTargetToLook (bodyPartToLook);
				}
			} 

			if (!bodyPartFound) {
				placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);
				if (placeToShoot != null) {
					setTargetToLook (placeToShoot);
				}

				//check if the object to check is too far from screen center in case the llok at body parts on characters is active and no body part is found or is close enough to screen center
				if (lookAtBodyPartsOnCharacters && useMaxDistanceToCameraCenter && getClosestToCameraCenter) {
					screenPoint = mainCamera.WorldToScreenPoint (targetToLook.position);
					float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);
					if (currentDistance > maxDistanceToCameraCenter) {
						setTargetToLook (null);
						targetFound = false;
						//print ("cancel look at target");
					}
				}
			}
		}
	
		return targetFound;
	}

	//update look at target position
	void lookAtTarget ()
	{
		bool lookingTarget = false;
		lookTargetPosition = Vector3.zero;
		//looking at a target in the scene, like an enemy, a vehicle, any element inside the tags to look list
		if (targetToLook != null) {
			lookingTarget = true;
			lookTargetPosition = targetToLook.position;

			lookAtTargetTransform.position = pivotCameraTransform.position
			+ pivotCameraTransform.transform.right * currentState.camPositionOffset.x
			+ pivotCameraTransform.transform.up * currentState.camPositionOffset.y;
		} 

		//look to a position created in the locked camera, like the position where the player is aiming right now
		if (lookingAtPoint) {
			lookingTarget = true;
			lookTargetPosition = pointTargetPosition;

			lookAtTargetTransform.position = pivotCameraTransform.position;
		}

		if (lookingTarget) {
			Debug.DrawLine (mainCameraTransform.position, lookTargetPosition, Color.green);
		}

		Vector3 targetDir = lookTargetPosition - lookAtTargetTransform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetDir, transform.up);

		lookAtTargetTransform.rotation = targetRotation;

		navMeshCurrentLookPos = lookAtTargetTransform.forward;

		if (useLookTargetIcon) {

			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (lookTargetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (lookTargetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
			}

			if (targetOnScreen) {
	
				if (usingScreenSpaceCamera) {
					lookAtTargetIconRectTransform.anchoredPosition = 
						new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
				} else {
					lookAtTargetIcon.transform.position = screenPoint;
				}
			} 
		}

		if (!lookingTarget) {
			setLookAtTargetState (false, null);
		}
	}

	public void setLookAtTargetStateInput (bool state)
	{
		//the player is in locked camera mode, so search if there is a target list stored to aim to the next target
		if (searchingToTargetOnLockedCamera) {
			currentTargetToLookIndex++;

			for (int i = 0; i < targetsListToLookTransform.Count; i++) {
				if (targetsListToLookTransform [i] == null) {
					targetsListToLookTransform.RemoveAt (i);
					i = 0;
				}
			}

			if (currentTargetToLookIndex >= targetsListToLookTransform.Count) {
				currentTargetToLookIndex = 0;
			}

			if (targetsListToLookTransform.Count > 0) {
				print ("new " + targetsListToLookTransform [currentTargetToLookIndex].name);
				setLookAtTargetState (true, targetsListToLookTransform [currentTargetToLookIndex]);
			} else {
				setLookAtTargetState (true, null);
			}
		} 
		//else, set the regular look at target mode
		else {
			setLookAtTargetState (state, null);
			lookintAtTargetByInput = state;
		}
	}

	public void setLookAtTargetStateManual (bool state, Transform objectToLook)
	{
		setLookAtTargetState (state, objectToLook);
		lookintAtTargetByInput = state;
	}

	//look at target
	public void setLookAtTargetState (bool state, Transform objectToLook)
	{
		if (!lookAtTargetEnabled) {
			return;
		}

		Quaternion targetRotation = Quaternion.LookRotation (mainCameraTransform.forward, transform.up);
		lookAtTargetTransform.rotation = targetRotation;
		lookingAtTarget = state;
		lastTimeAimAsisst = Time.time;

		if (!lookingAtTarget) {
			searchingToTargetOnLockedCamera = false;
		}

		//print (isCameraTypeFree ());

		if (isCameraTypeFree ()) {
			lookingAtPoint = false;
		} else {
			lookingAtPoint = state;

			lookintAtTargetByInput = state;
		}
			
		//check if the player is searching to look at a possible target
		if (lookingAtTarget && !lookingAtPoint) {
			if (objectToLook != null) {
				setTargetToLook (objectToLook);

				checkTargetToLookShader (targetToLook);
			} else if (!getClosestTargetToLook ()) {
				setLookAtTargetState (false, null);
				setTargetToLook (null);

				checkTargetToLookShader (null);
			}
		} else {
			//if the player is currently on locked mode, check to aim to the closest enemy
			if (searchingToTargetOnLockedCamera || activeLookAtTargetOnLockedCamera) {
				if (activeLookAtTargetOnLockedCamera) {
					lookingAtPoint = false;
					lookingAtFixedTarget = true;
				} else {
					lookingAtFixedTarget = false;
				}

				if (objectToLook != null) {
					setTargetToLook (objectToLook);

					checkTargetToLookShader (targetToLook);

					placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);
					if (placeToShoot != null) {
						setTargetToLook (placeToShoot);
					}
				} else {
					checkTargetToLookShader (null);
				}

				if (currentLockedCameraAxisInfo.useAimAssist) {
					//if no target is found, then set the state to just aim on the screen by moving the mouse
					if (objectToLook == null && !getClosestTargetToLookOnLockedCamera ()) {
						if (activeLookAtTargetOnLockedCamera) {
							lookingAtTarget = false;
							lookingAtPoint = false;

							setTargetToLook (null);

							checkTargetToLookShader (null);

							lookingAtFixedTarget = false;
							lookintAtTargetByInput = false;
							activeLookAtTargetOnLockedCamera = false;

							return;
						} else {
							lookingAtFixedTarget = false;
							lookingAtPoint = true;

							setTargetToLook (null);

							checkTargetToLookShader (null);
						}
					} else {
						//else, it means a target to aim was found

						if (usingScreenSpaceCamera) {
							currentLockedCameraCursor.anchoredPosition = getIconPosition (targetToLook.position);
						} else {
							Vector3 screenPos = mainCamera.WorldToScreenPoint (targetToLook.position);
							if (currentLockedCameraCursor) {
								currentLockedCameraCursor.transform.position = screenPos;
							}
						}
					}
				} else {
					lookingAtFixedTarget = false;
					lookingAtPoint = true;
					setTargetToLook (null);

					checkTargetToLookShader (null);
				}
			} else {
				lookingAtFixedTarget = false;

				checkTargetToLookShader (null);
			}
		}

		if (!lookingAtTarget) {
			setMaxDistanceToCameraCenter (originalUsemaxDistanceToCameraCenterValue, originalMaxDistanceToCameraCenterValue);
		}

		if (useLookTargetIcon) {
			lookAtTargetIcon.SetActive (lookingAtTarget);
		}
	}

	public bool lookingAtFixedTarget;

	public bool isPlayerLookingAtTarget ()
	{
		return lookingAtTarget;
	}

	public void setLookAtTargetOnLockedCameraState ()
	{
		searchingToTargetOnLockedCamera = true;
	}

	public bool getClosestTargetToLookOnLockedCamera ()
	{
		bool targetFound = false;

		currentTargetToLookIndex = 0;
		targetsListToLookTransform.Clear ();
		List<Collider> targetsListCollider = new List<Collider> ();

		List<GameObject> targetist = new List<GameObject> ();
		List<GameObject> fullTargetList = new List<GameObject> ();

		if (useLayerToSearchTargets) {
			targetsListCollider.AddRange (Physics.OverlapSphere (transform.position, maxDistanceToFindTarget, layerToLook));
			for (int i = 0; i < targetsListCollider.Count; i++) {
				fullTargetList.Add (targetsListCollider [i].gameObject);
			}
		} else {
			for (int i = 0; i < tagToLookList.Count; i++) {
				GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLookList [i]);
				targetist.AddRange (enemiesList);
			}

			for (int i = 0; i < targetist.Count; i++) {	
				float distance = GKC_Utils.distance (targetist [i].transform.position, transform.position);
				if (distance < maxDistanceToFindTarget) {
					fullTargetList.Add (targetist [i]);
				}
			}
		}

		List<GameObject> pointToLookComponentList = new List<GameObject> ();

		if (searchPointToLookComponents) {
			targetsListCollider.Clear ();
			targetsListCollider.AddRange (Physics.OverlapSphere (transform.position, maxDistanceToFindTarget, pointToLookComponentsLayer));
			for (int i = 0; i < targetsListCollider.Count; i++) {
				if (targetsListCollider [i].isTrigger) {
					pointToLook currentPointToLook = targetsListCollider [i].GetComponent<pointToLook> ();
					if (currentPointToLook) {
						GameObject currenTargetToLook = currentPointToLook.getPointToLookTransform ().gameObject;
						fullTargetList.Add (currenTargetToLook);
						pointToLookComponentList.Add (currenTargetToLook);
					}
				}
			}
		}

		for (int i = 0; i < fullTargetList.Count; i++) {
			if (fullTargetList [i] != null) {
				GameObject currentTarget = fullTargetList [i];
				if (tagToLookList.Contains (currentTarget.tag) || pointToLookComponentList.Contains (currentTarget)) {

					bool obstacleDetected = false;

					//for every target in front of the camera, use a raycast, if it finds an obstacle between the target and the camera, the target is removed from the list
					Vector3 originPosition = transform.position + transform.up * 0.5f;
					Vector3 targetPosition = currentTarget.transform.position + currentTarget.transform.up * 0.5f;

					Vector3 direction = targetPosition - originPosition;
					direction = direction / direction.magnitude;

					float distance = GKC_Utils.distance (targetPosition, originPosition);
					Debug.DrawLine (originPosition, originPosition + direction * distance, Color.black, 4);
					if (Physics.Raycast (originPosition, direction, out hit, distance, layerToLook)) {
						Debug.DrawLine (targetPosition, hit.point, Color.cyan, 4);
						if (hit.collider.gameObject != currentTarget) {
							obstacleDetected = true;
						}
					}

					bool objectVisible = false;
					if (currentLockedCameraAxisInfo.lookOnlyIfTargetOnScreen) {

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (currentTarget.transform.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (currentTarget.transform.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
						}

						//the target is visible in the screen
						if (targetOnScreen) {
							//check if the target is visible on the current locked camera
							if (currentLockedCameraAxisInfo.lookOnlyIfTargetVisible) {
								originPosition = mainCamera.transform.position;
								targetPosition = currentTarget.transform.position + currentTarget.transform.up * 0.5f;

								direction = targetPosition - originPosition;
								direction = direction / direction.magnitude;

								distance = GKC_Utils.distance (targetPosition, originPosition);
								if (Physics.Raycast (originPosition, direction, out hit, distance, layerToLook)) {
									Debug.DrawLine (targetPosition, hit.point, Color.cyan, 4);
									if (hit.collider.gameObject == currentTarget) {
										objectVisible = true;
									}
								}
							} else {
								objectVisible = true;
							}
						}
					} else {
						objectVisible = true;
					}

					if (objectVisible && !obstacleDetected) {
						targetsListToLookTransform.Add (currentTarget.transform);
					}
				}
			}
		}

		//finally, get the target closest to the player
		float minDistance = Mathf.Infinity;
		//Vector3 centerScreen = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		for (int i = 0; i < targetsListToLookTransform.Count; i++) {
			float currentDistance = GKC_Utils.distance (targetsListToLookTransform [i].position, transform.position);
			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				setTargetToLook (targetsListToLookTransform [i]);

				targetFound = true;
			}
		}

		targetsListToLookTransform.Sort (delegate(Transform a, Transform b) {
			return Vector3.Distance (transform.position, a.position)
						.CompareTo (
				Vector3.Distance (transform.position, b.position));
		});
			
		if (targetFound) {
//			bool bodyPartFound = false;
//			if (lookAtBodyPartsOnCharacters) {
//
//				Transform bodyPartToLook = targetToLook;
//
//				List<health.weakSpot> characterWeakSpotList = applyDamage.getCharacterWeakSpotList (targetToLook.gameObject);
//
//				if (characterWeakSpotList != null) {
//					minDistance = Mathf.Infinity;
//					for (int i = 0; i < characterWeakSpotList.Count; i++) {
//						if (bodyPartsToLook.Contains (characterWeakSpotList [i].name)) {
//
//							screenPoint = mainCamera.WorldToScreenPoint (characterWeakSpotList [i].spotTransform.position);
//							float currentDistance = Utils.distance (screenPoint, centerScreen);
//							if (currentDistance < minDistance) {
//								minDistance = currentDistance;
//								bodyPartToLook = characterWeakSpotList [i].spotTransform;
//							}
//						}
//					}
//
//					bodyPartFound = true;
//
//					targetToLook = bodyPartToLook;
//				}
//			} 
//
//			if (!bodyPartFound) {

			checkTargetToLookShader (targetToLook);
			placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);
			if (placeToShoot != null) {
				setTargetToLook (placeToShoot);
			}
//			}
		}

		return targetFound;
	}

	public void setTargetToLook (Transform newTarget)
	{
		targetToLook = newTarget;
	}

	public void checkTargetToLookShader (Transform newObjectFound)
	{
		if (pauseOutlineShaderChecking) {
			return;
		}
			
		if (useObjectToGrabFoundShader) {
			checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

			if (newObjectFound) {
				checkIfSetNewShaderToObjectToGrabFound (newObjectFound.gameObject);
			}
		}
	}

	public void checkIfSetNewShaderToObjectToGrabFound (GameObject objectToCheck)
	{
		if (useObjectToGrabFoundShader) {
			currentOutlineObjectSystem = objectToCheck.GetComponentInChildren<outlineObjectSystem> ();
			if (currentOutlineObjectSystem) {
				currentOutlineObjectSystem.setOutlineState (true, objectToGrabFoundShader, shaderOutlineWidth, shaderOutlineColor, playerControllerManager);
			}
		}
	}

	public void checkIfSetOriginalShaderToPreviousObjectToGrabFound ()
	{
		if (useObjectToGrabFoundShader && currentOutlineObjectSystem != null) {

			currentOutlineObjectSystem.setOutlineState (false, null, 0, Color.white, playerControllerManager);

			currentOutlineObjectSystem = null;
		}
	}


	public void setLookAtTargetEnabledState (bool state)
	{
		lookAtTargetEnabled = state;
	}

	public void setLookAtTargetEnabledStateDuration (bool currentState, float duration, bool nextState)
	{
		if (lookAtTargetEnabledCoroutine != null) {
			StopCoroutine (lookAtTargetEnabledCoroutine);
		}
		lookAtTargetEnabledCoroutine = StartCoroutine (setLookAtTargetEnabledStateDurationCoroutine (currentState, duration, nextState));
	}

	IEnumerator setLookAtTargetEnabledStateDurationCoroutine (bool currentState, float duration, bool nextState)
	{
		lookAtTargetEnabled = currentState;
		yield return new WaitForSeconds (duration);
		lookAtTargetEnabled = true;

		setLookAtTargetStateInput (false);

		lookAtTargetEnabled = nextState;

		if (lookAtTargetEnabled) {
			setOriginallookAtTargetSpeedValue ();
		}
	}

	public void setLookAtTargetSpeedValue (float newValue)
	{
		lookAtTargetSpeed = newValue;
	}

	public void setOriginallookAtTargetSpeedValue ()
	{
		lookAtTargetSpeed = originalLookAtTargetSpeed;
	}

	public void setMaxDistanceToFindTargetValue (float newValue)
	{
		maxDistanceToFindTarget = newValue;
	}

	public void setOriginalmaxDistanceToFindTargetValue ()
	{
		maxDistanceToFindTarget = originalMaxDistanceToFindTarget;
	}

	public void setMaxDistanceToCameraCenter (bool useMaxDistance, float maxDistance)
	{
		useMaxDistanceToCameraCenter = useMaxDistance;
		maxDistanceToCameraCenter = maxDistance;
	}

	public void death (bool state, bool followHips)
	{
		dead = state;
		headBobManager.playerAliveOrDead (dead);
		if (!firstPersonActive) {
			if (state) {
				if (followHips) {
					smoothFollow = true;
					smoothReturn = false;
					smoothGo = true;
					//this is for the ragdoll, it gets the hips of the player, which is the hips and the parent of the ragdoll
					//the hips is the object that the camera will follow when the player dies, because when this happens, the body of the player is out of the player controller
					//because, while the skeleton of the model will move by the gravity, player controller will not move of its position, due to player has dead
					targetToFollow = hips;
				}
			} else {
				smoothFollow = false;
				smoothReturn = true;
				smoothGo = false;
				targetToFollow = playerControllerGameObject.transform;
			}
		}
	}

	public void slerpCameraState (cameraStateInfo to, cameraStateInfo from, float time)
	{
		to.Name = from.Name;
		to.camPositionOffset = Vector3.Lerp (to.camPositionOffset, from.camPositionOffset, time);  
		to.pivotPositionOffset = Vector3.Lerp (to.pivotPositionOffset, from.pivotPositionOffset, time);
		to.pivotParentPossitionOffset = Vector3.Lerp (to.pivotParentPossitionOffset, from.pivotParentPossitionOffset, time);
		to.yLimits.x = Mathf.Lerp (to.yLimits.x, from.yLimits.x, time);
		to.yLimits.y = Mathf.Lerp (to.yLimits.y, from.yLimits.y, time);

		to.initialFovValue = from.initialFovValue;
		to.fovTransitionSpeed = from.fovTransitionSpeed;
		to.maxFovValue = from.maxFovValue;
		to.minFovValue = from.minFovValue;

		to.originalCamPositionOffset = from.originalCamPositionOffset;
	}

	public void slerpCameraStateAtOnce (cameraStateInfo to, cameraStateInfo from)
	{
		to.Name = from.Name;
		to.camPositionOffset = from.camPositionOffset;  
		to.pivotPositionOffset = from.pivotPositionOffset;
		to.pivotParentPossitionOffset = from.pivotParentPossitionOffset;
		to.yLimits.x = from.yLimits.x;
		to.yLimits.y = from.yLimits.y;

		to.initialFovValue = from.initialFovValue;
		to.fovTransitionSpeed = from.fovTransitionSpeed;
		to.maxFovValue = from.maxFovValue;
		to.minFovValue = from.minFovValue;

		to.originalCamPositionOffset = from.originalCamPositionOffset;
	}

	public void setTargetToFollow (Transform newTargetToFollow)
	{
		targetToFollow = newTargetToFollow;
		transform.position = targetToFollow.position;
	}

	public void setOriginalTargetToFollow ()
	{
		setTargetToFollow (playerControllerGameObject.transform);
	}

	public void setCameraState (string stateName)
	{
		for (int i = 0; i < playerCameraStates.Count; i++) {
			if (playerCameraStates [i].Name == stateName) {
				cameraStateInfo newState = new cameraStateInfo (playerCameraStates [i]);
				lerpState = newState;
				currentStateName = stateName;

				if (moveCameraPositionWithMouseWheelActive && useCameraMouseWheelStates) {
					updateCurrentMouseWheelCameraStateByDistance ();
				}
			}
		}

		if (mainCamera.fieldOfView != lerpState.initialFovValue) {
			if (usingZoomOn) {
				setZoom (false);
			} else {
				setMainCameraFov (lerpState.initialFovValue, lerpState.fovTransitionSpeed);
			}
		}
	}

	//if the player crouchs, move down the pivot
	public void crouch (bool isCrouching)
	{
		//check if the camera has been moved away from the player, then the camera moves from its position to the crouch position
		//else the pivot also is moved, but with other parameters
		if (isCrouching) {
			if (isCameraTypeFree ()) {
				if (firstPersonActive) {
					setCameraState (defaultFirstPersonCrouchStateName);
				} else {
					setCameraState (defaultThirdPersonCrouchStateName);
				}
			}

			crouching = true;
			moveAwayActive = false;
		} else {
			if (isCameraTypeFree ()) {
				if (firstPersonActive) {
					setCameraState (defaultFirstPersonStateName);
				} else {
					setCameraState (defaultThirdPersonStateName);
				}
			}

			crouching = false;
		}
	}

	//move away the camera
	public void moveAwayCamera ()
	{
		//check that the player is not in the first person mode or the aim mode
		if (isCameraTypeFree () && !aimingInThirdPerson && !firstPersonActive && !playerControllerManager.isPlayerOnZeroGravityMode ()) {
			bool canMoveAway = false;
			//if the player is crouched, the pivot is also moved, the player get up, but with other parameters
			if (playerControllerManager.isCrouching ()) {
				playerControllerManager.crouch ();
			}

			//if the player can not get up due the place where he is, stops the move away action of the pivot
			if (!playerControllerManager.isCrouching ()) {
				canMoveAway = true;
			}

			if (canMoveAway) {
				if (moveAwayActive) {
					setCameraState (defaultThirdPersonStateName);
					moveAwayActive = false;
				} else {
					setCameraState (defaultMoveCameraAwayStateName);
					moveAwayActive = true;
				}
			}
		}
	}

	public bool isMoveAwayCameraActive ()
	{
		return moveAwayActive;
	}

	public void setMainCameraFov (float targetValue, float speed)
	{
		if (cameraFovStartAndEndCoroutine != null) {
			StopCoroutine (cameraFovStartAndEndCoroutine);
		}
		stopFovChangeCoroutine ();
		changeFovCoroutine = StartCoroutine (changeFovValue (targetValue, speed));
	}

	public void stopFovChangeCoroutine ()
	{
		if (changeFovCoroutine != null) {
			StopCoroutine (changeFovCoroutine);
		}
	}

	public IEnumerator changeFovValue (float targetValue, float speed)
	{
		fieldOfViewChanging = true;
		while (mainCamera.fieldOfView != targetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, targetValue, getCurrentDeltaTime () * speed);
			yield return null;
		}
		fieldOfViewChanging = false;
	}

	public void setMainCameraFovStartAndEnd (float startTargetValue, float endTargetValue, float speed)
	{
		if (cameraFovStartAndEndCoroutine != null) {
			StopCoroutine (cameraFovStartAndEndCoroutine);
		}
		cameraFovStartAndEndCoroutine = StartCoroutine (changeFovValueStartAndEnd (startTargetValue, endTargetValue, speed));
	}

	public IEnumerator changeFovValueStartAndEnd (float startTargetValue, float endTargetValue, float speed)
	{
		while (mainCamera.fieldOfView != startTargetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, startTargetValue, getCurrentDeltaTime () * speed);
			yield return null;
		}
		while (mainCamera.fieldOfView != endTargetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, endTargetValue, getCurrentDeltaTime () * speed);
			yield return null;
		}
	}

	public bool isFOVChanging ()
	{
		return fieldOfViewChanging;
	}

	public void quickChangeFovValue (float targetValue)
	{
		mainCamera.fieldOfView = targetValue;
	}

	public void setOriginalCameraFov ()
	{
		setMainCameraFov (currentState.initialFovValue, zoomSpeed);
	}

	public float getMainCameraCurrentFov ()
	{
		return mainCamera.fieldOfView;
	}

	//set the zoom state
	public void setZoom (bool state)
	{
		if (!playerControllerManager.aimingInFirstPerson) {
			//to the fieldofview of the camera, it is added of substracted the zoomvalue
			usingZoomOn = state;
			float targetFov = currentState.minFovValue;
			float verticalRotationSpeedTarget = thirdPersonVerticalRotationSpeedZoomIn;
			float horizontalRotationSpeedTarget = thirdPersonHorizontalRotationSpeedZoomIn;

			if (firstPersonActive) {
				verticalRotationSpeedTarget = firstPersonVerticalRotationSpeedZoomIn;
				horizontalRotationSpeedTarget = firstPersonHorizontalRotationSpeedZoomIn;
			}
				
			if (!usingZoomOn) {
				if (firstPersonActive) {
					verticalRotationSpeedTarget = originalFirstPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalFirstPersonHorizontalRotationSpeed;
				} else {
					verticalRotationSpeedTarget = originalThirdPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalThirdPersonHorizontalRotationSpeed;
				}

				targetFov = currentState.initialFovValue;
			}

			setMainCameraFov (targetFov, zoomSpeed);
			//also, change the sensibility of the camera when the zoom is on or off, to control the camera properly
			changeRotationSpeedValue (verticalRotationSpeedTarget, horizontalRotationSpeedTarget);

			setScannerManagerState (state);

			if (weaponsManager.carryingWeaponInFirstPerson) {
				weaponsManager.changeWeaponsCameraFov (usingZoomOn, targetFov, zoomSpeed);
			}
		}
	}

	public void setZoomStateDuration (bool currentState, float duration, bool nextState)
	{
		if (zoomStateDurationCoroutine != null) {
			StopCoroutine (zoomStateDurationCoroutine);
		}
		zoomStateDurationCoroutine = StartCoroutine (setZoomStateDurationCoroutine (currentState, duration, nextState));
	}

	IEnumerator setZoomStateDurationCoroutine (bool currentState, float duration, bool nextState)
	{
		settings.zoomEnabled = currentState;
		yield return new WaitForSeconds (duration);

		setZoom (false);

		settings.zoomEnabled = nextState;
	}

	public void disableZoom ()
	{
		if (usingZoomOn) {
			usingZoomOn = false;

			setOriginalRotationSpeed ();

			setScannerManagerState (false);
		}
	}

	public bool isUsingZoom ()
	{
		return usingZoomOn;
	}

	public void setUsingZoomOnValue (bool value)
	{
		usingZoomOn = value;
		setScannerManagerState (value);
	}

	public void setScannerManagerState (bool state)
	{
		if (scannerManager) {
			scannerManager.enableOrDisableScannerZoom (state);
		}
	}

	public void changeRotationSpeedValue (float newVerticalRotationValue, float newHorizontalRotationValue)
	{
		if (firstPersonActive) {
			firstPersonVerticalRotationSpeed = newVerticalRotationValue;
			firstPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		} else {
			thirdPersonVerticalRotationSpeed = newVerticalRotationValue;
			thirdPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		}
	}

	public void setOriginalRotationSpeed ()
	{
		firstPersonVerticalRotationSpeed = originalFirstPersonVerticalRotationSpeed;
		firstPersonHorizontalRotationSpeed = originalFirstPersonHorizontalRotationSpeed;
		thirdPersonVerticalRotationSpeed = originalThirdPersonVerticalRotationSpeed;
		thirdPersonHorizontalRotationSpeed = originalThirdPersonHorizontalRotationSpeed;
	}

	//move away the camera when the player accelerates his movement velocity in the air, if the power of gravity is activated
	//once the player release shift, find a surface or stop in the air, the camera backs to its position
	//it is just to give the feeling of velocity
	public void changeCameraFov (bool state)
	{
		if (settings.enableMoveAwayInAir) {
			if (playerControllerManager.aimingInFirstPerson || weaponsManager.isPlayerRunningWithNewFov ()) {
				return;
			}
			//print ("disable zoom when land on ground");
			usingZoomOff = state;
			float targetFov = currentState.maxFovValue;
			float targetSpeed = fovChangeSpeed;
			if (!usingZoomOff) {
				targetFov = currentState.initialFovValue;
			}
			if (usingZoomOn) {
				targetSpeed = zoomSpeed;
				if (weaponsManager.carryingWeaponInFirstPerson) {
					weaponsManager.changeWeaponsCameraFov (false, targetFov, targetSpeed);
				}
			}
			setMainCameraFov (targetFov, targetSpeed);
			disableZoom ();
		}
	}

	//enable or disable the aim mode
	public void activateAiming (otherPowers.sideToAim side)
	{
		aimingInThirdPerson = true;
		if (isCameraTypeFree ()) {
			if (side == otherPowers.sideToAim.Right) {
				setCameraState (defaultThirdPersonAimRightStateName);
			} else {
				setCameraState (defaultThirdPersonAimLeftStateName);
			}
		} else {
			setCameraState (defaultLockedCameraStateName);
		}

		calibrateAccelerometer ();
		moveAwayActive = false;
	}

	public void deactivateAiming ()
	{
		aimingInThirdPerson = false;

		if (isCameraTypeFree ()) {
			setCameraState (defaultThirdPersonStateName);
		} else {
			setCameraState (defaultLockedCameraStateName);
		}
	}

	//change the aim side to left or right
	public void changeAimSide (int value)
	{
		if (value == 1) {
			setCameraState (defaultThirdPersonAimRightStateName);
		} else {
			setCameraState (defaultThirdPersonAimLeftStateName);
		}
	}

	//if the player is in the air, the camera can rotate 360 degrees, unlike when the player is in the ground where the rotation in x and y is limited
	public void onGroundOrOnAir (bool state)
	{
		onGround = state;
		if (!onGround) {
			adjustPivotAngle = true;
		}
	}

	public void setShakeCameraState (bool state, string stateName)
	{
		settings.shake = state;
		currentCameraShakeStateName = stateName;
	}

	//set the shake of the camera when the player moves in the air
	public void shakeCamera ()
	{
		settings.shake = true;

		currentCameraShakeStateName = "Shaking";
	}

	public void stopShakeCamera ()
	{
		settings.shake = false;
		settings.accelerateShaking = false;
		headBobManager.setDefaultState ();
	}

	public bool isCameraShaking ()
	{
		return settings.shake;
	}

	//now this funcion is here so it can be called by keyboard or touch button
	public void accelerateShake (bool value)
	{
		settings.accelerateShaking = value;
	}

	public void changeCameraToThirdOrFirstView (bool state)
	{
		if (state) {
			if (!firstPersonActive) {
				powersManager.changeCameraToThirdOrFirstView ();
			}
		} else {
			if (firstPersonActive) {
				powersManager.changeCameraToThirdOrFirstView ();
			}
		}
	}

	public void changeCameraToThirdOrFirstView ()
	{
		powersManager.changeCameraToThirdOrFirstView ();
	}

	//set first and third person camera position
	public void activateFirstPersonCamera ()
	{
		firstPersonActive = true;
		if (crouching) {
			setCameraState (defaultFirstPersonCrouchStateName);
		} else {
			setCameraState (defaultFirstPersonStateName);
		}
		moveAwayActive = false;

		updatePlayerStatesToView ();
	}

	public void deactivateFirstPersonCamera ()
	{
		firstPersonActive = false;
		if (crouching) {
			setCameraState (defaultThirdPersonCrouchStateName);
		} else {
			setCameraState (defaultThirdPersonStateName);
		}

		updatePlayerStatesToView ();
	}

	public void updatePlayerStatesToView ()
	{

		//check if in first person the animator is used, else the third person is enabled, so enable again the animator if it was disabled 
		playerControllerManager.checkAnimatorIsEnabled (firstPersonActive);	

		checkCharacterStateIconForViewChange ();

		playerControllerManager.setFootStepManagerState (firstPersonActive);

		if (grabObjectsManager) {
			grabObjectsManager.checkPhysicalObjectGrabbedPosition (firstPersonActive);
		}

		playerControllerManager.setFirstPersonViewActiveState (firstPersonActive);

		playerControllerManager.setCharacterMeshGameObjectState (!firstPersonActive);

		if (useEventOnThirdFirstPersonViewChange) {
			if (firstPersonActive) {
				setFirstPersonEvent.Invoke ();
			} else {
				setThirdPersonEvent.Invoke ();	
			}
		}
	}

	public bool isChangeCameraViewEnabled ()
	{
		return changeCameraViewEnabled;
	}

	public void enableOrDisableChangeCameraView (bool state)
	{
		changeCameraViewEnabled = state;
	}

	public void checkCharacterStateIconForViewChange ()
	{
		if (characterStateIconManager) {
			characterStateIconManager.checkCharacterStateIconForViewChange ();
		}
	}

	//stop the camera rotation or the camera collision detection
	public void changeCameraRotationState (bool state)
	{
		cameraCanRotate = state;
	}

	public void pauseOrPlayCamera (bool state)
	{
		cameraCanBeUsed = state;
	}

	//calibrate the initial accelerometer input according to how the player is holding the touch device
	public void calibrateAccelerometer ()
	{
		if (settings.useAcelerometer) {
			Vector3 wantedDeadZone = Input.acceleration;
			Quaternion rotateQuaternion = Quaternion.FromToRotation (new Vector3 (1, 0, 0), wantedDeadZone);
			//create identity matrix
			Matrix4x4 matrix = Matrix4x4.TRS (Vector3.zero, rotateQuaternion, Vector3.one);
			//get the inverse of the matrix
			calibrationMatrix = matrix.inverse;
		}
	}

	//get the accelerometer value, taking in account that the device is holing in left scape mode, with the home button in the right side
	Vector3 getAccelerometer (Vector3 accelerator)
	{
		Vector3 accel = calibrationMatrix.MultiplyVector (accelerator);
		return accel;
	}

	//get the accelerometer value more smoothly
	Vector3 lowpass ()
	{
		float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable
		lowPassValue = Vector3.Lerp (lowPassValue, getAccelerometer (Input.acceleration), LowPassFilterFactor);
		return lowPassValue;
	}

	public void setUseAcelerometerState (bool state)
	{
		settings.useAcelerometer = state;
	}

	public bool isCameraRotating ()
	{
		return isMoving;
	}

	public void setLastTimeMoved ()
	{
		lastTimeMoved = Time.time;
	}

	public float getLastTimeMoved ()
	{
		return lastTimeMoved;
	}

	public void playOrPauseHeadBob (bool state)
	{
		headBobManager.playOrPauseHeadBob (state);
	}

	public headBob getHeadBobManager ()
	{
		return headBobManager;
	}

	public void setFirstOrThirdHeadBobView (bool state)
	{
		headBobManager.setFirstOrThirdHeadBobView (state);
	}

	public float getOriginalCameraFov ()
	{
		return currentState.initialFovValue;
	}

	public bool isCameraPlacedInFirstPerson ()
	{
		float currentCameraDistance = Vector3.Distance (mainCameraTransform.position, pivotCameraTransform.position);
		if (currentCameraDistance < 0.01f) {
			return true;
		} else {
			return false;
		}
	}

	public bool isFirstPersonActive ()
	{
		return firstPersonActive;
	}

	public bool isCameraTypeFree ()
	{
		return cameraType == typeOfCamera.free;
	}

	public void setLookAngleValue (Vector2 newValue)
	{
		lookAngle = newValue;
	}

	public void resetMainCameraTransformLocalPosition ()
	{
		mainCameraTransform.localPosition = lerpState.camPositionOffset;
	}

	public void resetPivotCameraTransformLocalPosition ()
	{
		pivotCameraTransform.localPosition = lerpState.pivotPositionOffset;
	}

	public void resetCurrentCameraStateAtOnce ()
	{
		slerpCameraStateAtOnce (currentState, lerpState);
	}

	public void configureCurrentLerpState (Vector3 pivotOffset, Vector3 cameraOffset)
	{
		lerpState.camPositionOffset = cameraOffset;
		lerpState.pivotPositionOffset = pivotOffset;
	}

	public void configureCameraAndPivotPositionAtOnce ()
	{
		pivotCameraTransform.localPosition = currentState.pivotPositionOffset;
		mainCameraTransform.localPosition = currentState.camPositionOffset;
	}

	public void setCurrentCameraUpRotationValue (float newRotation)
	{
		currentCameraUpRotation = newRotation;
	}

	//AI Input
	public void Rotate (Vector3 currentLookPost)
	{
		navMeshCurrentLookPos = currentLookPost;
	}

	public void setPlayerNavMeshEnabledState (bool state)
	{
		playerNavMeshEnabled = state;
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

		if (usedByAI) {
			setOverrideCameraPosition (usedByAI);
		}
	}

	public void setOverrideCameraPosition (bool state)
	{
		int defaultStateIndex = -1;
		for (int i = 0; i < playerCameraStates.Count; i++) {
			if (playerCameraStates [i].Name == defaultStateName) {
				defaultStateIndex = i;
			}
		}

		if (state) {
			playerCameraStates [defaultStateIndex].camPositionOffset = offsetInitialPosition; 
		} else {
			playerCameraStates [defaultStateIndex].camPositionOffset = cameraInitialPosition;
		}

		setCameraState (defaultStateName);

		mainCameraTransform.localPosition = playerCameraStates [defaultStateIndex].camPositionOffset;
	}

	public void setZeroGravityModeOnState (bool state)
	{
		zeroGravityModeOn = state;
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		freeFloatingModeOn = state;
	}

	public void setForwardRotationValue (float value)
	{
		print (value);
		targetForwardRotationAngle = value;
	}

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	RectTransform iconRectTransform;
	Vector3 iconPositionViewPoint;
	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	public Vector2 getIconPosition (Vector3 worldObjectPosition)
	{
		iconPositionViewPoint = mainCamera.WorldToViewportPoint (worldObjectPosition);
		iconPosition2d = new Vector2 ((iconPositionViewPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (iconPositionViewPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

		return iconPosition2d;
	}

	public void setLockedCameraRotationActiveToLeft (bool holdingButton)
	{
		rotatingLockedCameraToLeft = holdingButton;
		lockedCameraRotationDirection = -1;

		if (!rotatingLockedCameraToLeft && rotatingLockedCameraToRight) {
			lockedCameraRotationDirection = 1;
		}
	}

	public void setLockedCameraRotationActiveToRight (bool holdingButton)
	{
		rotatingLockedCameraToRight = holdingButton;
		lockedCameraRotationDirection = 1;

		if (!rotatingLockedCameraToRight && rotatingLockedCameraToLeft) {
			lockedCameraRotationDirection = -1;
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputMoveCamerAway ()
	{
		if (cameraType == typeOfCamera.free) {
			if (cameraCanBeUsed) {
				if (settings.moveAwayCameraEnabled) {
					moveAwayCamera ();
				}
			}
		}
	}

	public void inputLookAtTarget ()
	{
		if (lookAtTargetEnabled && canActivateLookAtTargetEnabled && cameraCanBeUsed) {
			if (cameraType == typeOfCamera.free) {
				setLookAtTargetStateInput (!lookingAtTarget);
			} else {
				if (searchingToTargetOnLockedCamera) {
					setLookAtTargetStateInput (true);
				} 

//				else if (canActiveLookAtTargetOnLockedCamera && !using2_5ViewActive) {
//					activeLookAtTargetOnLockedCamera = !activeLookAtTargetOnLockedCamera;
//					setLookAtTargetStateInput (!lookingAtTarget);
//				}
			}
		}
	}

	public void inputZoom ()
	{
		if (cameraType == typeOfCamera.free) {
			if (cameraCanBeUsed) {
				if (settings.zoomEnabled) {
					setZoom (!usingZoomOn);
				}
			}
		} else if (cameraType == typeOfCamera.locked) {
			if (currentLockedCameraAxisInfo.canUseZoom) {
				usingLockedZoomOn = !usingLockedZoomOn;
				float targetFov = currentLockedCameraAxisInfo.zoomValue;
				if (!usingLockedZoomOn) {
					targetFov = currentState.initialFovValue;
				}
				setMainCameraFov (targetFov, zoomSpeed);
			}
		}
	}

	public void inputMoveSetRotateToLeftState (bool keyDown)
	{
		if (cameraType == typeOfCamera.free) {
			if (cameraCanBeUsed) {
				if (zeroGravityModeOn && canRotateForwardOnZeroGravityModeOn) {
					if (keyDown) {
						setForwardRotationValue (1);
					} else {
						setForwardRotationValue (0);
					}
				}
			}
		}
	}

	public void inputMoveSetRotateToRightState (bool keyDown)
	{
		if (cameraType == typeOfCamera.free) {
			if (cameraCanBeUsed) {
				if (zeroGravityModeOn && canRotateForwardOnZeroGravityModeOn) {
					if (keyDown) {
						setForwardRotationValue (-1);
					} else {
						setForwardRotationValue (0);
					}
				}
			}
		}
	}

	public void inputResetCameraRotation ()
	{
		resetCameraRotation ();
	}

	public void inputRotateLockedCameraToRight (bool holdingButton)
	{
		if (cameraType == typeOfCamera.locked) {
			setLockedCameraRotationActiveToRight (holdingButton);
		}
	}

	public void inputRotateLockedCameraToLeft (bool holdingButton)
	{
		if (cameraType == typeOfCamera.locked) {
			setLockedCameraRotationActiveToLeft (holdingButton);
		}
	}

	int currentMouseWheelStateIndex;

	public void inputMoveCameraCloserOrFartherFromPlayer (bool movingDirection)
	{
		if (moveCameraPositionWithMouseWheelActive) {
			if (cameraType == typeOfCamera.free) {
				if (cameraCanBeUsed) {

					if (grabObjectsManager) {
						if (grabObjectsManager.isPauseCameraMouseWheelWhileObjectGrabbedActive () && grabObjectsManager.isGrabbedObject ()) {
							return;
						} else {
							grabObjectsManager.dropObject ();
						}
					}

					if (useCameraMouseWheelStates) {
						if (movingDirection) {
							lerpState.camPositionOffset.z -= moveCameraPositionBackwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);

						} else {
							lerpState.camPositionOffset.z += moveCameraPositionForwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);
						}

						float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);
					
						int currentWheelMouseStateIndex = -1;
						for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
							if (currentWheelMouseStateIndex == -1) {
								if (currentCameraDistance >= cameraMouseWheelStatesList [i].cameraDistanceRange.x && currentCameraDistance <= cameraMouseWheelStatesList [i].cameraDistanceRange.y) {
									if (!cameraMouseWheelStatesList [i].isCurrentCameraState) {
										updateCurrentMousWheelCameraState (i);

										currentWheelMouseStateIndex = i;
									} else {
										if (cameraMouseWheelStatesList [i].changeCameraIfDistanceChanged) {
											if (currentCameraDistance >= cameraMouseWheelStatesList [i].minCameraDistanceToChange) {
												if (cameraMouseWheelStatesList [i].changeToAboveCameraState) {
													updateCurrentMousWheelCameraState (i - 1);

													currentWheelMouseStateIndex = (i - 1);
												} else {
													updateCurrentMousWheelCameraState (i + 1);

													currentWheelMouseStateIndex = (i + 1);
												}

												cameraMouseWheelStatesList [i].isCurrentCameraState = false;
											}
										}
									}
								} else {
									cameraMouseWheelStatesList [i].isCurrentCameraState = false;
								}
							}
						}

						if (currentWheelMouseStateIndex != -1) {
							cameraMouseWheelStatesList [currentWheelMouseStateIndex].isCurrentCameraState = true;
						}
					} else {
						if (movingDirection) {
							lerpState.camPositionOffset.z -= moveCameraPositionBackwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);

							if (firstPersonActive) {
								changeCameraToThirdOrFirstView ();
							}
						} else {
							lerpState.camPositionOffset.z += moveCameraPositionForwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z, 0);

							float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);

							if (!firstPersonActive) {
								if (currentCameraDistance <= minDistanceToChangeToFirstPerson) {
									changeCameraToThirdOrFirstView ();
								}
							}
						}
					}
				}
			}
		}
	}

	public void updateCurrentMousWheelCameraState (int newIndex)
	{
		cameraMouseWheelStates currentCameraMouseWheelStates = cameraMouseWheelStatesList [newIndex];

		if (newIndex < currentMouseWheelStateIndex) {
			currentCameraMouseWheelStates.eventFromBelowCameraState.Invoke ();
		} else {
			currentCameraMouseWheelStates.eventFromAboveCameraState.Invoke ();
		}

		if (newIndex < currentMouseWheelStateIndex) {
			if (currentCameraMouseWheelStates.changeCameraFromBelowStateWithName) {
				setCameraState (currentCameraMouseWheelStates.Name);
			}
		} else {
			if (currentCameraMouseWheelStates.changeCameraFromAboveStateWithName) {
				setCameraState (currentCameraMouseWheelStates.Name);
			}
		}

		currentCameraMouseWheelStates.isCurrentCameraState = true;
		currentMouseWheelStateIndex = newIndex;
	}

	public void updateCurrentMouseWheelCameraStateByDistance ()
	{
		float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);

		for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
			if (currentCameraDistance >= cameraMouseWheelStatesList [i].cameraDistanceRange.x && currentCameraDistance <= cameraMouseWheelStatesList [i].cameraDistanceRange.y) {
				cameraMouseWheelStatesList [i].isCurrentCameraState = true;
				currentMouseWheelStateIndex = i;
			} else {
				cameraMouseWheelStatesList [i].isCurrentCameraState = false;
			}
		}
	}

	public void resetCameraRotation ()
	{
		if (cameraType == typeOfCamera.free && !firstPersonActive && cameraCanBeUsed) {
			resetingCameraActive = true;
		}
	}

	public void setDrivingState (bool state)
	{
		driving = state;
	}

	Coroutine vehicleCameraMovementCoroutine;

	public void setVehicleCameraMovementCoroutine (Coroutine cameraMovement)
	{
		vehicleCameraMovementCoroutine = cameraMovement;
	}

	public Coroutine getVehicleCameraMovementCoroutine ()
	{
		return vehicleCameraMovementCoroutine;
	}

	public List<Camera> getCameraList ()
	{
		return cameraList;
	}

	public bool isUsingScreenSpaceCamera ()
	{
		return usingScreenSpaceCamera;
	}

	public void setThirdPersonInEditor ()
	{
		setThirdPersonInEditorEvent.Invoke ();
	}

	public void setFirstPersonInEditor ()
	{
		setFirstPersonInEditorEvent.Invoke ();
	}

	public void addNewLockedCameraSystemToLevel ()
	{
		GameObject newLockedCameraSystem = (GameObject)Instantiate (lockedCameraSystemPrefab, Vector3.zero, Quaternion.identity);

		#if UNITY_EDITOR
		Selection.activeGameObject = newLockedCameraSystem;
		#endif
	}

	public void addNewLockedCameraLimitSystemToLevel ()
	{
		GameObject newLockedCameraLimitSystem = (GameObject)Instantiate (lockedCameraLimitSystemPrefab, Vector3.zero, Quaternion.identity);

		#if UNITY_EDITOR
		Selection.activeGameObject = newLockedCameraLimitSystem;
		#endif
	}

	public void addNewLockedCameraPrefabTypeLevel (int cameraIndex)
	{
		if (cameraIndex <= lockedCameraPrefabsTypesList.Count && lockedCameraPrefabsTypesList [cameraIndex].lockedCameraPrefab != null) {
			GameObject newLockedCameraSystem = (GameObject)Instantiate (lockedCameraPrefabsTypesList [cameraIndex].lockedCameraPrefab, Vector3.zero, Quaternion.identity);

			#if UNITY_EDITOR
			Selection.activeGameObject = newLockedCameraSystem;
			#endif
		} else {
			print ("WARNING: prefab of the selected camera doesn't exist or is not configured on this list, make sure a prefab is assigned");
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}

	//draw the lines of the pivot camera in the editor
	void OnDrawGizmos ()
	{
		if (!settings.showCameraGizmo) {
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
		if (settings.showCameraGizmo) {
			for (int i = 0; i < playerCameraStates.Count; i++) {
				if (playerCameraStates [i].showGizmo) {
					Gizmos.color = playerCameraStates [i].gizmoColor;
					Vector3 currentPivotOffset = playerCameraStates [i].pivotPositionOffset;

					Vector3 pivotPosition = transform.position
					                        + transform.right * currentPivotOffset.x
					                        + transform.up * currentPivotOffset.y
					                        + transform.forward * currentPivotOffset.z;
					
					Vector3 currentCameraOffset = playerCameraStates [i].camPositionOffset;
					Vector3 cameraPosition = pivotPosition
					                         + pivotCameraTransform.right * currentCameraOffset.x
					                         + pivotCameraTransform.up * currentCameraOffset.y
					                         + pivotCameraTransform.forward * currentCameraOffset.z;
					
					Gizmos.DrawSphere (cameraPosition, settings.gizmoRadius);
					Gizmos.color = playerCameraStates [i].gizmoColor;
					Gizmos.DrawSphere (pivotPosition, settings.gizmoRadius);

					Gizmos.color = settings.linesColor;
					Gizmos.DrawLine (cameraPosition, pivotPosition);
					Gizmos.DrawLine (pivotPosition, transform.position);
				}
			}
		}

		if (waypointForward && waypointBackward) {
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere (closestWaypoint.position, 1.1f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (waypointForward.position, 1.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (waypointBackward.position, 1);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (nextWaypoint.position, 3);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (originWaypoint.position + Vector3.up, targetWaypoint.position + Vector3.up);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (originWaypoint.position, originWaypoint.position + Vector3.up * 3);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (targetWaypoint.position, targetWaypoint.position + Vector3.up * 4);

		}

		if (useCameraLimit) {
			
			Gizmos.color = Color.red;

			Gizmos.DrawWireSphere (currentCameraLimitPosition, 1);
		}
			
		if (cameraType == typeOfCamera.locked) {
			if (currentLockedCameraAxisInfo != null) {
				if (currentLockedCameraAxisInfo.lockedCameraPivot) {
					Gizmos.color = Color.green;
					Vector3 pivotPosition = lockedCameraPivot.position + lockedCameraPivot.up * lockedCameraAxis.localPosition.y;

					Gizmos.DrawLine (lockedCameraAxis.position, pivotPosition);

					Gizmos.DrawLine (pivotPosition, lockedCameraPivot.position);

					GKC_Utils.drawGizmoArrow (lockedCameraPosition.position, lockedCameraPosition.forward * 1, Color.green, 0.5f, 10);
				
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine (lookCameraParent.position, lookCameraPivot.position);
					Gizmos.DrawLine (lookCameraPivot.position, lookCameraDirection.position);
				}

				if (currentLockedCameraAxisInfo.useBoundToFollowPlayer) {
					//lockedCameraSystem.drawBoundGizmo (focusArea.centre, currentLockedCameraAxisInfo, new Color (1, 0, 0, .5f));

					Gizmos.color = new Color (1, 0, 0, .5f);

					float height = (currentLockedCameraAxisInfo.heightBoundTop);
					float width = (currentLockedCameraAxisInfo.widthBoundRight + currentLockedCameraAxisInfo.widthBoundLeft);
					float depth = (currentLockedCameraAxisInfo.depthBoundFront + currentLockedCameraAxisInfo.depthBoundBackward);

					Gizmos.DrawCube (focusArea.centre, new Vector3 (width, height, depth));
				}
			}
		}
	}

	//a group of parameters to configure the shake of the camera
	[System.Serializable]
	public class cameraSettings
	{
		public LayerMask layer;
		public bool useAcelerometer;
		public bool zoomEnabled;
		public bool moveAwayCameraEnabled;
		public bool enableMoveAwayInAir = true;
		public bool enableShakeCamera = true;
		public bool showCameraGizmo = true;
		public float gizmoRadius;
		public Color gizmoLabelColor;
		public Color linesColor;
		[HideInInspector] public bool shake = false;
		[HideInInspector] public bool accelerateShaking = false;
	}

	[System.Serializable]
	public struct FocusArea
	{
		public Vector3 centre;
		public Vector3 velocity;
		public float left, right;
		public float top, bottom;
		public float front, backward;

		public FocusArea (Bounds targetBounds, float heightBoundTop, float widthBoundRight, float widthBoundLeft, float depthBoundFront, float depthBoundBackward)
		{
			left = targetBounds.center.x - widthBoundLeft;
			right = targetBounds.center.x + widthBoundRight;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + heightBoundTop;

			front = targetBounds.center.z + depthBoundFront;
			backward = targetBounds.center.z - depthBoundBackward;

			velocity = Vector3.zero;
			centre = new Vector3 ((left + right) / 2, (top + bottom) / 2, (front + backward) / 2);
		}

		public void Update (Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;

			float shiftZ = 0;
			if (targetBounds.min.z < backward) {
				shiftZ = targetBounds.min.z - backward;
			} else if (targetBounds.max.z > front) {
				shiftZ = targetBounds.max.z - front;
			}
			front += shiftZ;
			backward += shiftZ;

			centre = new Vector3 ((left + right) / 2, (top + bottom) / 2, (front + backward) / 2);
			velocity = new Vector3 (shiftX, shiftY, shiftZ);
		}
	}

	[System.Serializable]
	public class cameraMouseWheelStates
	{
		public string Name;
		public Vector2 cameraDistanceRange;

		public bool changeCameraIfDistanceChanged;
		public float minCameraDistanceToChange;
		public bool changeToAboveCameraState;

		public bool changeCameraFromAboveStateWithName;
		public bool changeCameraFromBelowStateWithName;

		public bool isCurrentCameraState;
		public UnityEvent eventFromAboveCameraState;
		public UnityEvent eventFromBelowCameraState;
	}

	[System.Serializable]
	public class lockedCameraPrefabsTypes
	{
		public string Name;
		public GameObject lockedCameraPrefab;
	}
}