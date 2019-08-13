using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class grabObjects : MonoBehaviour
{
	public float holdDistance = 3;
	public float maxDistanceHeld = 4;
	public float maxDistanceGrab = 10;
	public float holdSpeed = 10;
	public float alphaTransparency = 0.5f;
	public bool objectCanBeRotated;
	public float rotationSpeed;
	public float rotateSpeed;
	public float minTimeToIncreaseThrowForce = 300;
	public float increaseThrowForceSpeed = 1500;
	public float extraThorwForce = 10;
	public float maxThrowForce = 3500;
	public grabMode currentGrabMode;

	public bool grabInFixedPosition;
	public bool rotateToCameraInFixedPosition;
	public bool rotateToCameraInFreePosition;

	public float closestHoldDistanceInFixedPosition;
	public string grabbedObjectTag;
	public string grabbedObjectLayer;
	public Transform grabZoneTransform;

	public bool useCursor = true;
	public bool onlyEnableCursorIfLocatedObject;
	public GameObject cursor;
	public RectTransform cursorRectTransform;
	public GameObject grabObjectCursor;
	public GameObject grabbedObjectCursor;
	public GameObject playerCameraTransform;
	public float grabCursorScale;

	public bool grabbedObjectCanBeBroken;
	public bool usedByAI;

	public bool grabObjectsEnabled;
	public Shader pickableShader;
	public Slider powerSlider;

	public bool useLoadThrowParticles;

	public GameObject[] particles;
	public LayerMask layer;
	public bool enableTransparency = true;
	public bool useGrabbedParticles;
	public bool canUseZoomWhileGrabbed;
	public float zoomSpeed;
	public float maxZoomDistance;
	public float minZoomDistance;
	public ForceMode powerForceMode;

	public bool useThrowObjectsLayer = true;
	public LayerMask throwObjectsLayerToCheck;

	public float throwPower;
	public ForceMode realisticForceMode;

	public LayerMask gravityObjectsLayer;
	public string layerForCustomGravityObject;

	public bool changeGravityObjectsEnabled = true;

	public bool launchedObjectsCanMakeNoise;
	public float minObjectSpeedToActivateNoise;

	public enum grabMode
	{
		powers,
		realistic
	}

	public bool grabbed;
	public bool gear;
	public bool rail;
	public bool regularObject;

	public List<string> ableToGrabTags = new List<string> ();

	public GameObject currentObjectToGrabFound;

	public bool grabObjectsPhysicallyEnabled = true;
	public List<handInfo> handInfoList = new List<handInfo> ();
	public Transform placeToCarryPhysicalObjectsThirdPerson;
	public Transform placeToCarryPhysicalObjectsFirstPerson;
	public bool carryingPhysicalObject;
	public float translatePhysicalObjectSpeed = 5;

	public GameObject currentPhysicalObjectGrabbed;
	public GameObject currentPhysicalObjectToGrabFound;

	public GameObject currentPhysicsObjectElement;
	Coroutine droppingCoroutine;
	Coroutine translateGrabbedPhysicalObject;
	Coroutine grabbingCoroutine;

	public List<GameObject> physicalObjectToGrabFoundList = new List<GameObject> ();

	public bool useForceWhenObjectDropped;
	public bool useForceWhenObjectDroppedOnFirstPerson;
	public bool useForceWhenObjectDroppedOnThirdPerson;
	public float forceWhenObjectDroppedOnFirstPerson;
	public float forceWhenObjectDroppedOnThirdPerson;

	public Text keyText;

	Vector3 currentIconPosition;
	public bool showGrabObjectIconEnabled;
	public GameObject grabObjectIcon;
	public RectTransform iconRectTransform;

	public bool getClosestDeviceToCameraCenter;
	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;

	public bool useObjectToGrabFoundShader;
	public Shader objectToGrabFoundShader;
	public float shaderOutlineWidth;
	public Color shaderOutlineColor;

	public GameObject objectHeld;

	public bool pauseCameraMouseWheelWhileObjectGrabbed;

	public string grabObjectActionName = "Grab Object";

	int currentPhysicalObjectIndex;
	float minDistanceToTarget;
	Vector3 currentPosition;
	Vector3 objectPosition;
	float currentDistanceToTarget;
	Vector3 screenPoint;
	bool deviceCloseEnoughToScreenCenter;
	Vector3 centerScreen;

	public bool objectFocus;

	Rigidbody objectHeldRigidbody;
	GameObject smoke;
	float holdTimer = 0;
	float railAngle = 0;
	float timer = 0;
	RaycastHit hit;
	Shader dropShader;

	railMechanism currentRailMechanism;

	bool grabbedObjectTagLayerStored;
	string originalGrabbedObjectTag;
	int originalGrabbedObjectLayer;

	float orignalHoldDistance;

	public playerController playerControllerManager;
	public otherPowers powersManager;
	public playerInputManager playerInput;
	public playerCamera playerCameraManager;
	public usingDevicesSystem usingDevicesManager;
	public playerWeaponsManager weaponsManager;
	public gravitySystem gravityManager;
	public IKSystem IKManager;
	public Collider mainCollider;
	public menuPause pauseManager;
	public Transform mainCameraTransform;
	public Camera mainCamera;

	Transform fixedGrabedTransform;
	bool rotatingObject;
	bool usingDoor;
	RigidbodyConstraints objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
	Transform objectHeldFollowTransform;

	Vector3 nextObjectHeldPosition;
	Vector3 currentObjectHeldPosition;
	float currentMaxDistanceHeld;
	public bool aiming = false;

	GameObject currentObjectToThrow;
	Transform currentHoldTransform;
	artificialObjectGravity currentArtificialObjectGravity;
	vehicleGravityControl currentVehicleGravityControl;

	grabPhysicalObjectSystem currentgrabPhysicalObjectSystem;

	Vector2 axisValues;
	float currentDistanceToGrabbedObject;

	bool currentObjectWasInsideGravityRoom;

	outlineObjectSystem currentOutlineObjectSystem;

	List<Collider> grabeObjectColliderList = new List<Collider> ();

	public Transform grabbedObjectClonnedColliderTransform;
	public BoxCollider grabbedObjectClonnedCollider;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	void Start ()
	{
		orignalHoldDistance = holdDistance;

		powerSlider.maxValue = maxThrowForce;
		powerSlider.value = maxThrowForce;

		centerScreen = new Vector3 (Screen.width / 2, Screen.height / 2, 0);

		if (!useCursor && cursor) {
			cursor.SetActive (false);
		}

		if (!usedByAI) {
			mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
			halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
			usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
		}

		if (grabbedObjectClonnedCollider) {
			Physics.IgnoreCollision (mainCollider, grabbedObjectClonnedCollider, true);
		}
	}

	void FixedUpdate ()
	{
		if (usedByAI) {
			return;
		}

		if (physicalObjectToGrabFoundList.Count > 0) {
			int index = getClosesPhysicalObjectToGrab ();
			if (index != -1) {
				currentPhysicsObjectElement = physicalObjectToGrabFoundList [index];
				if (showGrabObjectIconEnabled) {
					if (currentPhysicsObjectElement != null) {
						currentIconPosition = currentPhysicsObjectElement.transform.position;

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (currentIconPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (currentIconPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
						}

						if (targetOnScreen) {

							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

								iconRectTransform.anchoredPosition = iconPosition2d;
							} else {
								grabObjectIcon.transform.position = screenPoint;
							}

							enableOrDisableIconButton (true);
						} else {
							enableOrDisableIconButton (false);
						}
					} else {
						enableOrDisableIconButton (false);
					}
				} else {
					enableOrDisableIconButton (false);
				}
			} else {
				enableOrDisableIconButton (false);
			}
		} else {
			enableOrDisableIconButton (false);
		}
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}

		if (rotatingObject) {
			axisValues = playerInput.getPlayerMovementAxis ("mouse");
			objectHeld.transform.RotateAroundLocal (mainCameraTransform.up, -Mathf.Deg2Rad * rotateSpeed * axisValues.x);
			objectHeld.transform.RotateAroundLocal (mainCameraTransform.right, Mathf.Deg2Rad * rotateSpeed * axisValues.y);
		}
			
		// if an object is grabbed, then move it from its original position, to the other in front of the camera
		if (objectHeld && !carryingPhysicalObject) {

			//get the transform for the grabbed object to follow
			currentHoldTransform = mainCameraTransform;
			if (playerCameraManager.is2_5ViewActive ()) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirection2_5d ();
				holdDistance = 0;
			}

			if (playerCameraManager.useTopDownView) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirectionTopDown ();
				holdDistance = 0;
			}

			currentDistanceToGrabbedObject = GKC_Utils.distance (objectHeld.transform.position, currentHoldTransform.position);
			if (!grabbed) {
				timer += Time.deltaTime;

				if ((currentDistanceToGrabbedObject <= currentMaxDistanceHeld || rail || gear || usingDoor || grabInFixedPosition) && timer > 0.5f) {
					grabbed = true;
					timer = 0;
				}
			}

			//if the object is not capable to move in front of the camera, because for example is being blocked for a wall, drop it
			if (currentDistanceToGrabbedObject > currentMaxDistanceHeld && grabbed && regularObject) {
				dropObject ();
			} else {
				//if the object is a cube, a turret, or anything that can move freely, set its position in front of the camera
				if (regularObject) {
					if (grabInFixedPosition) {
						nextObjectHeldPosition = fixedGrabedTransform.position + fixedGrabedTransform.forward * holdDistance;
						currentObjectHeldPosition = objectHeld.transform.position;
					} else {
						if (playerCameraManager.is2_5ViewActive ()) {
							nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * holdDistance;
						} else {
							nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * (holdDistance + objectHeld.transform.localScale.x);
						}
						currentObjectHeldPosition = objectHeld.transform.position;
					}

					objectHeldRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * holdSpeed;

					if (!rotatingObject && ((rotateToCameraInFixedPosition && grabInFixedPosition) || (!grabInFixedPosition && rotateToCameraInFreePosition))) {
						objectHeld.transform.rotation = Quaternion.Slerp (objectHeld.transform.rotation, mainCameraTransform.rotation, Time.deltaTime * rotationSpeed);
					}
				} 

				//else if the object is on a rail get the angle between the forward of the camera and the object forward
				if (rail) {
					int dir = 0;
					float newAngle = Vector3.Angle (objectHeld.transform.forward, mainCameraTransform.forward);
					if (newAngle >= railAngle + 5) {
						dir = -1;
					}
					if (newAngle <= railAngle - 5) {
						dir = 1;
					}
					//if the camera aims to the object, dont move it, else move in the direction the camera is looking in the local forward and back of the object
					if (Physics.Raycast (currentHoldTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer)) {
						if (hit.transform.gameObject == objectHeld) {
							dir = 0;
							railAngle = Vector3.Angle (objectHeld.transform.forward, mainCameraTransform.forward);
						}
					}
					if (Mathf.Abs (newAngle - railAngle) < 10) {
						dir = 0;
					}
					objectHeld.transform.Translate (Vector3.forward * dir * Time.deltaTime * currentRailMechanism.getDisplaceRailSpeed ());
				}

				if (gear) {
					//else, the object is a gear, so rotate it
					objectHeld.transform.Rotate (0, 0, 150 * Time.deltaTime);
				}

				if (usingDoor) {
					movableDoor currentMovableDoor = objectHeld.GetComponent<movableDoor> ();
					float yAxis = objectHeld.GetComponent<ConfigurableJoint> ().axis.y * playerInput.getPlayerMovementAxis ("mouse").y;
					Vector3 extraYRotation = objectHeld.transform.localEulerAngles + objectHeld.transform.up * yAxis;
					float angleY = extraYRotation.y;
					if (angleY > 180) {
						angleY = Mathf.Clamp (angleY, currentMovableDoor.limitXAxis.y, 360);
					} else if (angleY > 0) {
						angleY = Mathf.Clamp (angleY, 0, currentMovableDoor.limitXAxis.x);
					}
					extraYRotation = new Vector3 (extraYRotation.x, angleY, extraYRotation.z);
					//extraYRotation += objectHeld.transform.up*yAxis;
					Quaternion rot = Quaternion.Euler (extraYRotation);
					objectHeld.transform.localRotation = Quaternion.Slerp (objectHeld.transform.localRotation, rot, Time.deltaTime * currentMovableDoor.rotationSpeed);
				}

				if (currentGrabMode == grabMode.powers) {
					if (smoke) {
						//activate the particles while the player is moving an object
						smoke.transform.transform.LookAt (grabZoneTransform.position);
						smoke.GetComponent<ParticleSystem> ().startSpeed = GKC_Utils.distance (smoke.transform.position, grabZoneTransform.position) / 2;
					}
				}
			}
		}

		if (objectHeld && carryingPhysicalObject) {
			if (!grabbed) {
				timer += Time.deltaTime;
				if (timer > 0.2f) {
					grabbed = true;
					timer = 0;
				}
			}

			if (grabbedObjectClonnedColliderTransform && grabbedObjectClonnedColliderTransform.gameObject.activeSelf) {
				grabbedObjectClonnedColliderTransform.position = currentPhysicalObjectGrabbed.transform.position;
				grabbedObjectClonnedColliderTransform.rotation = currentPhysicalObjectGrabbed.transform.rotation;
			}
		}

		//change cursor size to show that the player is aiming a grabbable object and set to its normal scale and get the object to hold in case the player could grab it
		if (aiming && !objectHeld && !currentPhysicalObjectToGrabFound) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer)) {

				if (currentObjectToGrabFound != hit.collider.gameObject) {
					currentObjectToGrabFound = hit.collider.gameObject;

					if (checkTypeObject (currentObjectToGrabFound)) {
						GameObject mainObjectFound = applyDamage.getCharacterOrVehicle (currentObjectToGrabFound);
						if (mainObjectFound == null) {
							grabObjectParent currentGrabObjectParent = currentObjectToGrabFound.GetComponent<grabObjectParent> ();
							if (currentGrabObjectParent) {
								mainObjectFound = currentGrabObjectParent.getObjectToGrab ();
							} else {
								mainObjectFound = currentObjectToGrabFound;
							}
						}

						if (!physicalObjectToGrabFoundList.Contains (mainObjectFound)) {

							checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

							checkIfSetNewShaderToObjectToGrabFound (mainObjectFound);
						}
					}
				}

				if (checkTypeObject (hit.collider.gameObject) && !objectFocus) {

					if (onlyEnableCursorIfLocatedObject) {
						enableOrDisableGeneralCursor (true);
					}

					setGrabObjectCursorScale (grabCursorScale);
					objectFocus = true;
				}

				if (!checkTypeObject (hit.collider.gameObject) && objectFocus) {
					if (onlyEnableCursorIfLocatedObject) {
						enableOrDisableGeneralCursor (false);
					}

					setGrabObjectCursorScale (1);
					objectFocus = false;

					checkIfSetOriginalShaderToPreviousObjectToGrabFound ();
				}
			} else {
				if (objectFocus) {
					if (onlyEnableCursorIfLocatedObject) {
						enableOrDisableGeneralCursor (false);
					}

					setGrabObjectCursorScale (1);
					objectFocus = false;

					checkIfSetOriginalShaderToPreviousObjectToGrabFound ();
				}
			}
		}

		if (aiming && !playerCameraManager.isCameraTypeFree ()) {
			if (playerCameraManager.currentLockedCameraCursor) {
				cursorRectTransform.position = playerCameraManager.currentLockedCameraCursor.position;
			}
		}

		if (grabbed && objectHeld == null) {
			dropObject ();
		}
	}

	public void enableOrDisableIconButton (bool state)
	{
		if (grabObjectIcon.activeSelf != state) {
			grabObjectIcon.SetActive (state);
		}
	}

	public void grabObject ()
	{
		if (currentPhysicalObjectToGrabFound) {
			grabCurrenObject (currentPhysicalObjectToGrabFound);
		} else {
			//if the object which the player is looking, grab it
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer) && objectFocus) {
				grabCurrenObject (hit.collider.gameObject);
			} 
		}
	}

	public void grabCurrenObject (GameObject objectToGrab)
	{
		if (checkTypeObject (objectToGrab)) {
			//reset the hold distance
			holdDistance = orignalHoldDistance;

			//if the located object is part of a vehicle, get the main vehicle object to grab it
			objectHeld = applyDamage.getVehicle (objectToGrab);
			characterDamageReceiver currentCharacterDamageReceiver = objectToGrab.GetComponent<characterDamageReceiver> ();
			if (objectHeld != null) {
				if (!objectHeld.GetComponent<Rigidbody> ().isKinematic) {
					//get the extra grab distance configurable for every vehicle
					holdDistance += objectHeld.GetComponent<vehicleHUDManager> ().extraGrabDistance;
					currentVehicleGravityControl = objectHeld.GetComponent<vehicleGravityControl> ();
					if (currentVehicleGravityControl) {
						currentVehicleGravityControl.pauseDownForce (true);
					}
				} else {
					objectHeld = null;
					return;
				}
			} 

			//if the located object is part of a character, get the main character object to grab it
			else if (currentCharacterDamageReceiver && !currentCharacterDamageReceiver.isCharacterDead ()) {
				objectHeld = currentCharacterDamageReceiver.character;
			} 

			//else it is an object from the able to grab list
			else {
				grabObjectParent currentGrabObjectParent = objectToGrab.GetComponent<grabObjectParent> ();
				if (currentGrabObjectParent) {
					objectHeld = currentGrabObjectParent.getObjectToGrab ();
				} else {
					objectHeld = objectToGrab;
				}
			}

			currentgrabPhysicalObjectSystem = objectHeld.GetComponentInChildren <grabPhysicalObjectSystem> ();

			//get its tag, to set it again to the object, when it is dropped
			if (!objectHeld.GetComponent<vehicleHUDManager> ()) {
				grabbedObjectTagLayerStored = true;
				originalGrabbedObjectTag = objectHeld.tag.ToString ();
				originalGrabbedObjectLayer = objectHeld.layer;
				objectHeld.tag = grabbedObjectTag;
				objectHeld.layer = LayerMask.NameToLayer (grabbedObjectLayer);
			}

			objectHeldRigidbody = objectHeld.GetComponent<Rigidbody> ();
			if (objectHeldRigidbody) {
				objectHeldRigidbody.isKinematic = false;
				objectHeldRigidbody.useGravity = false;
				objectHeldRigidbody.velocity = Vector3.zero;
			}

			//if the object has its gravity modified, pause that script
			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();
			if (currentArtificialObjectGravity) {
				currentArtificialObjectGravity.active = false;
			}

			explosiveBarrel currentExplosiveBarrel = objectHeld.GetComponent<explosiveBarrel> ();
			if (currentExplosiveBarrel) {
				currentExplosiveBarrel.setExplosiveBarrelOwner (gameObject);
			}

			if (!grabbedObjectCanBeBroken) {
				if (currentExplosiveBarrel) {
					currentExplosiveBarrel.canExplodeState (false);
				}
				crate currentCrate = objectHeld.GetComponent<crate> ();
				if (currentCrate) {
					currentCrate.crateCanBeBrokenState (false);
				}
			}

			pickUpObject currentPickUpObject = objectHeld.GetComponent<pickUpObject> ();
			if (currentPickUpObject) {
				currentPickUpObject.activateObjectTrigger ();
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent<grabObjectEventSystem> ();
			if (currentGrabObjectEventSystem) {
				currentGrabObjectEventSystem.callEventOnGrab ();
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();
			if (currentObjectToPlaceSystem) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (true);
			}

			if (currentGrabMode == grabMode.powers) {
				//if the objects is a mechanism, the object is above a rail, so the player could move it only in two directions
				currentRailMechanism = objectHeld.GetComponent<railMechanism> ();
				if (currentRailMechanism) {
					railAngle = Vector3.Angle (objectHeld.transform.forward, mainCameraTransform.forward);
					rail = true;
					currentRailMechanism.setUsingRailState (true);
					objectHeld.layer = originalGrabbedObjectLayer;
				}

				//if the object is a gear, it only can be rotated
				else if (objectHeld.name == "rotatoryGear") {
					gear = true;
				} 

				//else, if it is a door
				else if (!objectHeld.GetComponent<ConfigurableJoint> () && objectHeldRigidbody) {
					regularObject = true;
					objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
					objectHeldRigidbody.freezeRotation = true;
				}

				grabbedObjectState currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();
				if (!currentGrabbedObjectState) {
					currentGrabbedObjectState = objectHeld.AddComponent<grabbedObjectState> ();
				}
				if (currentGrabbedObjectState) {
					currentGrabbedObjectState.setCurrentHolder (gameObject);
					currentGrabbedObjectState.setGrabbedState (true);
				}
			} else {
				if (!objectHeld.GetComponent<ConfigurableJoint> () && objectHeldRigidbody) {
					regularObject = true;
					if (!objectHeld.GetComponent<vehicleHUDManager> ()) {
						objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
						objectHeldRigidbody.freezeRotation = true;
					}
				}
			}

			//if the object grabbed is an AI, pause it
			objectHeld.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);

			if (objectHeld.GetComponent<ConfigurableJoint> ()) {
				usingDoor = true;
				objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
				objectHeldRigidbody.freezeRotation = true;
				playerCameraManager.changeCameraRotationState (false);
			}

			if (objectHeld.GetComponent<pickUpObject> ()) {
				objectHeld.transform.SetParent (null);
			}

			if (!currentgrabPhysicalObjectSystem || !currentgrabPhysicalObjectSystem.isGrabObjectPhysicallyEnabled ()) {
				//if the transparency is enabled, change all the color of all the materials of the object
				if (enableTransparency) {
					outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();
					if (currentOutlineObjectSystem) {
						currentOutlineObjectSystem.setTransparencyState (true, pickableShader, alphaTransparency);
					}
				}
			}

			powerSlider.value = 0;
			holdTimer = 0;
			enableOrDisableGrabObjectCursor (false);
			grabbedObjectCursor.SetActive (true);

			if (currentGrabMode == grabMode.powers) {
				if (useGrabbedParticles) {
					//enable particles and reset some powers values
					smoke = (GameObject)Instantiate (particles [3], objectHeld.transform.position, objectHeld.transform.rotation);
					smoke.transform.SetParent (objectHeld.transform);
					smoke.SetActive (true);
					particles [0].SetActive (true);
				}
			}

			if (regularObject) {
				if (!fixedGrabedTransform) {
					GameObject fixedPositionObject = new GameObject ();
					fixedGrabedTransform = fixedPositionObject.transform;
					fixedGrabedTransform.name = "Fixed Grabed Tansform";
				}

				if (grabInFixedPosition) {
					fixedGrabedTransform.SetParent (mainCameraTransform);
					fixedGrabedTransform.transform.position = objectHeld.transform.position;
					fixedGrabedTransform.transform.rotation = mainCameraTransform.rotation;
					currentMaxDistanceHeld = GKC_Utils.distance (objectHeld.transform.position, mainCameraTransform.position) + holdDistance;
					holdDistance = 0;
				} else {
					currentMaxDistanceHeld = maxDistanceHeld;
				}
			}

			currentObjectWasInsideGravityRoom = false;

			if (grabObjectsPhysicallyEnabled && currentgrabPhysicalObjectSystem) {
				if (currentgrabPhysicalObjectSystem.isGrabObjectPhysicallyEnabled ()) {
					currentgrabPhysicalObjectSystem.grabObject (gameObject);
				}
			}
		}
	}

	//check if the object detected by the raycast is in the able to grab list or is a vehicle
	public bool checkTypeObject (GameObject objectToCheck)
	{
		if (ableToGrabTags.Contains (objectToCheck.tag.ToString ())) {
			return true;
		}

		if (objectToCheck.GetComponent<vehicleDamageReceiver> () || objectToCheck.GetComponent<vehicleHUDManager> ()) {
			return true;
		}

		characterDamageReceiver currentCharacterDamageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (currentCharacterDamageReceiver) {
			if (ableToGrabTags.Contains (currentCharacterDamageReceiver.character.tag.ToString ())) {
				return true;
			}
		}

		grabObjectParent currentGrabObjectParent = objectToCheck.GetComponent<grabObjectParent> ();
		if (currentGrabObjectParent) {
			return true;
		}

		return false;
	}

	Vector3 throwObjectDirection;

	//drop the object
	public void dropObject ()
	{
		if (carryingPhysicalObject && !grabbed) {
			return;
		}

		if (useThrowObjectsLayer) {
			throwObjectDirection = Vector3.zero;

			if (objectHeld) {
				if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, throwObjectsLayerToCheck)) {
					Vector3 heading = hit.point - objectHeld.transform.position;
					float distance = heading.magnitude;
					throwObjectDirection = heading / distance;
					throwObjectDirection.Normalize ();
				}
			}
		}
			
		grabbedObjectCursor.SetActive (false);
		setGrabObjectCursorScale (1);
		if (aiming) {
			enableOrDisableGrabObjectCursor (true);
		}

		if (onlyEnableCursorIfLocatedObject) {
			enableOrDisableGeneralCursor (false);
		}

		if (playerCameraManager) {
			playerCameraManager.changeCameraRotationState (true);
		}

		rotatingObject = false;
		usingDoor = false;

		if (objectHeld) {
			//set the tag of the object that had before grab it, and if the object has its own gravity, enable again
			if (grabbedObjectTagLayerStored) {
				objectHeld.tag = originalGrabbedObjectTag;
				objectHeld.layer = originalGrabbedObjectLayer;
				grabbedObjectTagLayerStored = false;
			}

			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();
			if (currentArtificialObjectGravity) {
				currentArtificialObjectGravity.active = true;
			}

			explosiveBarrel currentExplosiveBarrel = objectHeld.GetComponent<explosiveBarrel> ();

			if (!grabbedObjectCanBeBroken) {
				if (currentExplosiveBarrel) {
					currentExplosiveBarrel.canExplodeState (true);
				}

				crate currentCrate = objectHeld.GetComponent<crate> ();
				if (currentCrate) {
					currentCrate.crateCanBeBrokenState (true);
				}
			}

			currentVehicleGravityControl = objectHeld.GetComponent<vehicleGravityControl> ();
			if (objectHeldRigidbody) {
				if (!currentVehicleGravityControl) {
					objectHeldRigidbody.useGravity = true;
				}
				if (!objectHeld.GetComponent<ConfigurableJoint> ()) {
					objectHeldRigidbody.freezeRotation = false;
					if (objectHeldRigidbodyConstraints != RigidbodyConstraints.None) {
						objectHeldRigidbody.constraints = objectHeldRigidbodyConstraints;
						objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
					}
				}
			}

			if (currentVehicleGravityControl) {
				currentVehicleGravityControl.pauseDownForce (false);
			}

			if (enableTransparency) {
				//set the normal shader of the object 
				outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();
				if (currentOutlineObjectSystem) {
					currentOutlineObjectSystem.setTransparencyState (false, null, 0);
				}
			}

			//if the grabbed object is a turret, call a function to make it kinematic again when it will touch a surface
			if (objectHeld.tag == "friend") {
				objectHeld.SendMessage ("dropCharacter", true, SendMessageOptions.DontRequireReceiver);
			}
			//if the object grabbed is an AI, enable it
			objectHeld.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);


			grabbedObjectState currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();
			if (currentGrabbedObjectState) {

				if (currentgrabPhysicalObjectSystem) {
					if (gravityManager.isPlayerInsiderGravityRoom ()) {
						currentGrabbedObjectState.setInsideZeroGravityRoomState (true);
						currentGrabbedObjectState.setCurrentZeroGravityRoom (gravityManager.getCurrentZeroGravityRoom ());
					}
				}

				currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();
				currentGrabbedObjectState.checkGravityRoomState ();
				currentGrabbedObjectState.setGrabbedState (false);
				if (!currentObjectWasInsideGravityRoom) {
					Destroy (currentGrabbedObjectState);
				}
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();
			if (currentObjectToPlaceSystem) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (false);
			}

			railMechanism newRailMechanism = objectHeld.GetComponent<railMechanism> ();
			if (newRailMechanism) {
				newRailMechanism.setUsingRailState (false);
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent <grabObjectEventSystem> ();
			if (currentGrabObjectEventSystem) {
				currentGrabObjectEventSystem.callEventOnDrop ();
			}
				
			if (currentgrabPhysicalObjectSystem) {
				if (currentgrabPhysicalObjectSystem.isGrabObjectPhysicallyEnabled ()) {
					currentgrabPhysicalObjectSystem.dropObject ();
				}
			}
		}

		grabbed = false;
		objectHeld = null;
		objectHeldRigidbody = null;
		currentgrabPhysicalObjectSystem = null;

		rail = false;

		gear = false;
		regularObject = false;
		particles [0].SetActive (false);
		particles [1].SetActive (false);

		objectFocus = false;
		Destroy (smoke);
	}

	public void checkJointsInObject (GameObject objectToThrow, float force)
	{
		CharacterJoint currentCharacterJoint = objectToThrow.GetComponent<CharacterJoint> ();
		if (currentCharacterJoint) {
			checkJointsInObject (currentCharacterJoint.connectedBody.gameObject, force);
		} else {
			addForceToThrownRigidbody (objectToThrow, force);
		}
	}

	public void addForceToThrownRigidbody (GameObject objectToThrow, float force)
	{
		Component[] components = objectToThrow.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Component c in components) {
			Rigidbody currentRigid = c as Rigidbody;
			if (!currentRigid.isKinematic && currentRigid.GetComponent<Collider> ()) {
				Vector3 forceDirection = mainCameraTransform.forward;

				if (useThrowObjectsLayer) {
					//print (throwObjectDirection + " " + forceDirection);
					if (throwObjectDirection != Vector3.zero) {
						forceDirection = throwObjectDirection;
					}
				}

				forceDirection *= force * currentRigid.mass;

				currentRigid.AddForce (forceDirection, powerForceMode);

				checkIfEnableNoiseOnCollision (objectToThrow, forceDirection.magnitude);
			}
		}
	}

	public void checkGrabbedObjectAction ()
	{
		if (currentGrabMode == grabObjects.grabMode.powers) {
			if (changeGravityObjectsEnabled) {
				GameObject grabbedObject = getGrabbedObject ();
				if (grabbedObject.GetComponent<Rigidbody> ()) {
					dropObject ();
					//if the current object grabbed is a vehicle, enable its own gravity control component
					currentVehicleGravityControl = grabbedObject.GetComponent<vehicleGravityControl> ();
					if (currentVehicleGravityControl) {
						currentVehicleGravityControl.activateGravityPower (mainCameraTransform.TransformDirection (Vector3.forward), 
							mainCameraTransform.TransformDirection (Vector3.right));
					} 

					//else, it is a regular object
					else {
						//change the layer, because the object will use a raycast to check the new normal when a collision happens
						grabbedObject.layer = LayerMask.NameToLayer (layerForCustomGravityObject);
						//if the object has a regular gravity, attach the scrip and set its values
						currentArtificialObjectGravity = grabbedObject.GetComponent<artificialObjectGravity> ();
						if (!currentArtificialObjectGravity) {
							currentArtificialObjectGravity = grabbedObject.AddComponent<artificialObjectGravity> ();
						} 
						currentArtificialObjectGravity.enableGravity (gravityObjectsLayer, powersManager.settings.highFrictionMaterial, mainCameraTransform.forward);
					}
				}
			}
		}

		if (currentGrabMode == grabObjects.grabMode.realistic) {
			throwRealisticGrabbedObject ();
		}
	}

	public void throwRealisticGrabbedObject ()
	{
		powerSlider.gameObject.SetActive (false);
		Rigidbody currentRigidbody = objectHeld.GetComponent<Rigidbody> ();
		bool wasRegularObject = regularObject;
		dropObject ();
		if (currentRigidbody && wasRegularObject) {
			Vector3 forceDirection = mainCameraTransform.forward * throwPower * currentRigidbody.mass;
			currentRigidbody.AddForce (forceDirection, realisticForceMode);

			checkIfEnableNoiseOnCollision (objectHeld, forceDirection.magnitude);
		}
	}

	public void changeGrabbedZoom (int zoomType)
	{
		if (zoomType > 0) {
			holdDistance += Time.deltaTime * zoomSpeed;
		} else {
			holdDistance -= Time.deltaTime * zoomSpeed;
		}
		if (!grabInFixedPosition) {
			if (holdDistance > maxZoomDistance) {
				holdDistance = maxZoomDistance;
			}
			if (holdDistance < minZoomDistance) {
				holdDistance = minZoomDistance;
			}
		} else {
			if (holdDistance > currentMaxDistanceHeld) {
				holdDistance = currentMaxDistanceHeld;
			}
			if (holdDistance < 0) {
				if ((holdDistance + currentMaxDistanceHeld - orignalHoldDistance) <= closestHoldDistanceInFixedPosition) {
					holdDistance = -currentMaxDistanceHeld + orignalHoldDistance + closestHoldDistanceInFixedPosition;
				}
			}
		}
	}

	Coroutine setFixedGrabbedTransformCoroutine;

	public void resetFixedGrabedTransformPosition ()
	{
		stopResetFixedGrabedTransformPositionCoroutine ();
		setFixedGrabbedTransformCoroutine = StartCoroutine (resetFixedGrabedTransformPositionCoroutine ());
	}

	public void stopResetFixedGrabedTransformPositionCoroutine ()
	{
		if (setFixedGrabbedTransformCoroutine != null) {
			StopCoroutine (setFixedGrabbedTransformCoroutine);
		}
	}

	IEnumerator resetFixedGrabedTransformPositionCoroutine ()
	{
		Vector3 targetPosition = Vector3.forward * orignalHoldDistance;

		float dist = GKC_Utils.distance (fixedGrabedTransform.position, mainCameraTransform.position + targetPosition);
		float duration = dist / zoomSpeed;
		float t = 0;
		bool targetReached = false;

		float timer = 0;

		while (!targetReached && t < 1 && fixedGrabedTransform.localPosition != targetPosition) {
			t += Time.deltaTime / duration;
			fixedGrabedTransform.localPosition = Vector3.Lerp (fixedGrabedTransform.localPosition, targetPosition, t);

			timer += Time.deltaTime;
			if (GKC_Utils.distance (fixedGrabedTransform.localPosition, targetPosition) < 0.01f || timer > duration) {
				targetReached = true;
			}

			yield return null;
		}
	}

	public void enableOrDisableGrabObjectCursor (bool value)
	{
		grabObjectCursor.SetActive (value);
		if (!value) {

			cursorRectTransform.localPosition = Vector3.zero;
		}
	}

	public void enableOrDisableGeneralCursor (bool state)
	{
		if (useCursor) {
			cursor.SetActive (state);
		}
	}

	public bool grabObjectCursorState ()
	{
		return grabObjectCursor.activeSelf;
	}

	public void setGrabObjectCursorScale (float scale)
	{
		grabObjectCursor.transform.localScale = Vector3.one * scale;
	}

	public void checkIfDropObject (GameObject objectToCheck)
	{
		if (objectHeld == objectToCheck) {
			dropObject ();
		}
	}

	public void setAimingState (bool state)
	{
		aiming = state;
		enableOrDisableGrabObjectCursor (state);

		if (aiming) {
			if (carryingPhysicalObject) {
				enableOrDisableGrabObjectCursor (false);
			}
		} else {
			if (carryingPhysicalObject) {
				grabbedObjectCursor.SetActive (false);
			}
			checkIfSetOriginalShaderToPreviousObjectToGrabFound ();
		}
	}

	public GameObject getGrabbedObject ()
	{
		return objectHeld;
	}

	public bool isGrabbedObject ()
	{
		return objectHeld != null;
	}

	public void checkIfEnableNoiseOnCollision (GameObject objectToCheck, float launchSpeed)
	{
		if (launchedObjectsCanMakeNoise && launchSpeed >= minObjectSpeedToActivateNoise) {
			noiseSystem currentNoiseSystem = objectToCheck.GetComponent<noiseSystem> ();
			if (currentNoiseSystem) {
				currentNoiseSystem.setUseNoiseState (true);
			}
		}
	}

	public void addForceToLaunchObject ()
	{
		if (regularObject && currentGrabMode == grabMode.powers) {
			if (holdTimer > minTimeToIncreaseThrowForce) {
				//if the button is not released immediately, active the power slider
				if (!powerSlider.gameObject.activeSelf) {
					if (useLoadThrowParticles) {
						particles [1].SetActive (true);
					}

					powerSlider.gameObject.SetActive (true);
				}
			}
			if (holdTimer < maxThrowForce) {
				holdTimer += Time.deltaTime * increaseThrowForceSpeed;
				powerSlider.value += Time.deltaTime * increaseThrowForceSpeed;
			}
		}
	}

	public void launchObject ()
	{
		powerSlider.gameObject.SetActive (false);
		currentObjectToThrow = objectHeld;
		Rigidbody currentRigidbody = currentObjectToThrow.GetComponent<Rigidbody> ();
		dropObject ();
		bool canAddForceToObjectDropped = false;

		if (currentRigidbody) {
			//if the button has been pressed and released quickly, drop the object, else addforce to its rigidbody
			if (!currentObjectToThrow.GetComponent<vehicleGravityControl> () && !currentObjectWasInsideGravityRoom) {
				currentRigidbody.useGravity = true;
			}

			if (currentGrabMode == grabMode.powers) {
				if (holdTimer > minTimeToIncreaseThrowForce) {
					currentObjectToThrow.AddComponent<launchedObjects> ().setCurrentPlayer (gameObject);

					if (useLoadThrowParticles) {
						GameObject launchParticles = (GameObject)Instantiate (particles [2], grabZoneTransform.position, mainCameraTransform.rotation);
						launchParticles.transform.SetParent (null);
						launchParticles.SetActive (true);
					}

					if (currentObjectToThrow.GetComponent<CharacterJoint> ()) {
						checkJointsInObject (currentObjectToThrow, holdTimer);
					} else {
						holdTimer += extraThorwForce;
						addForceToThrownRigidbody (currentObjectToThrow, holdTimer);
					}
				} else {
					canAddForceToObjectDropped = true;
				}
			} else {
				canAddForceToObjectDropped = true;
			}
		}

		if (canAddForceToObjectDropped) {
			if (useForceWhenObjectDropped) {
				if (useForceWhenObjectDroppedOnFirstPerson && playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnFirstPerson);
				} else if (useForceWhenObjectDroppedOnThirdPerson && !playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnThirdPerson);
				}
			}
		}
	}

	public bool isCurrentObjectToGrabFound (GameObject objectToCheck)
	{
		if (currentObjectToGrabFound == objectToCheck || currentPhysicalObjectToGrabFound == objectToCheck) {
			return true;
		}
		return false;
	}

	public bool objectInphysicalObjectToGrabFoundList (GameObject objectToCheck)
	{
		if (physicalObjectToGrabFoundList.Contains (objectToCheck)) {
			return true;
		} 
		return false;
	}

	public void addCurrentPhysicalObjectToGrabFound (GameObject objectToGrab)
	{
		if (!physicalObjectToGrabFoundList.Contains (objectToGrab)) {
			physicalObjectToGrabFoundList.Add (objectToGrab);
		}
	}

	public void removeCurrentPhysicalObjectToGrabFound (GameObject objectToGrab)
	{
		if (physicalObjectToGrabFoundList.Contains (objectToGrab)) {
			physicalObjectToGrabFoundList.Remove (objectToGrab);
			if (currentObjectToGrabFound == objectToGrab) {
				currentObjectToGrabFound = null;
			}
		}

		if (physicalObjectToGrabFoundList.Count == 0) {
			checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

			currentPhysicalObjectToGrabFound = null;
		}
	}

	public bool physicalObjectToGrabFound ()
	{
		return currentPhysicalObjectToGrabFound != null;
	}

	public int getClosesPhysicalObjectToGrab ()
	{
		if (currentPhysicalObjectGrabbed) {
			return -1;
		}

		currentPhysicalObjectIndex = -1;
		minDistanceToTarget = Mathf.Infinity;
		currentPosition = transform.position;

		for (int i = 0; i < physicalObjectToGrabFoundList.Count; i++) {
			if (physicalObjectToGrabFoundList [i]) {
				objectPosition = physicalObjectToGrabFoundList [i].transform.position;
				if (getClosestDeviceToCameraCenter && playerCameraManager.isCameraTypeFree ()) {
					screenPoint = mainCamera.WorldToScreenPoint (objectPosition);

					currentDistanceToTarget = GKC_Utils.distance (screenPoint, centerScreen);
					deviceCloseEnoughToScreenCenter = false;
					if (useMaxDistanceToCameraCenter) {
						if (currentDistanceToTarget < maxDistanceToCameraCenter) {
							deviceCloseEnoughToScreenCenter = true;
						}
					} else {
						deviceCloseEnoughToScreenCenter = true;
					}

					if (deviceCloseEnoughToScreenCenter) {
						if (currentDistanceToTarget < minDistanceToTarget) {
							minDistanceToTarget = currentDistanceToTarget;
							currentPhysicalObjectIndex = i;
						}
					}
				} else {
					currentDistanceToTarget = GKC_Utils.distance (objectPosition, currentPosition);
					if (currentDistanceToTarget < minDistanceToTarget) {
						minDistanceToTarget = currentDistanceToTarget;
						currentPhysicalObjectIndex = i;
					}
				}
			} else {
				physicalObjectToGrabFoundList.RemoveAt (i);
				i = 0;
			}
		}

		if (getClosestDeviceToCameraCenter && useMaxDistanceToCameraCenter) {
			if (currentPhysicalObjectIndex == -1 && currentPhysicalObjectToGrabFound != null) {
				checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

				currentPhysicalObjectToGrabFound = null;
				return -1;
			}
		}

		if (currentPhysicalObjectIndex != -1) {
			if (currentPhysicalObjectToGrabFound != physicalObjectToGrabFoundList [currentPhysicalObjectIndex]) {

				if (currentPhysicalObjectToGrabFound != null) {
					checkIfSetOriginalShaderToPreviousObjectToGrabFound ();
				}

				currentPhysicalObjectToGrabFound = physicalObjectToGrabFoundList [currentPhysicalObjectIndex];

				setKeyText (true);

				checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

				checkIfSetNewShaderToObjectToGrabFound (currentPhysicalObjectToGrabFound);
			}
		}
		return currentPhysicalObjectIndex;
	}

	public void checkIfSetNewShaderToObjectToGrabFound (GameObject objectToCheck)
	{
		if (useObjectToGrabFoundShader) {
			//print ("new on " + objectToCheck.name);

			currentOutlineObjectSystem = objectToCheck.GetComponentInChildren<outlineObjectSystem> ();
			if (currentOutlineObjectSystem) {
				currentOutlineObjectSystem.setOutlineState (true, objectToGrabFoundShader, shaderOutlineWidth, shaderOutlineColor, playerControllerManager);
			}
		}
	}

	public void checkIfSetOriginalShaderToPreviousObjectToGrabFound ()
	{
		if (useObjectToGrabFoundShader && currentOutlineObjectSystem != null) {
			if (!usingDevicesManager.isCurrentDeviceToUseFound (currentOutlineObjectSystem.getMeshParent ())) {
				//print ("original");

				currentOutlineObjectSystem.setOutlineState (false, null, 0, Color.white, playerControllerManager);
			}
		
			currentOutlineObjectSystem = null;
		}
	}

	public void setKeyText (bool state)
	{
		//set the key text in the icon with the current action
		if (keyText) {
			if (state) {
				keyText.text = "[" + playerInput.getButtonKey (grabObjectActionName) + "]";
			} else {
				keyText.text = "";
			}
		}
	}

	Transform currentReferencePosition;

	public void grabPhysicalObject (bool IKSystemEnabled, GameObject objectToGrab, float animationSpeed, Transform rightHandPosition, 
	                                Transform lefHandPosition, Transform referencePosition, bool useRightHand, bool useLeftHand,
	                                bool useRightElbow, bool useLeftElbow, Transform rightElbowPosition, Transform lefElbowPosition)
	{
		if (grabbingCoroutine != null) {
			StopCoroutine (grabbingCoroutine);
		}

		grabbingCoroutine = StartCoroutine (grabPhysicalObjectCoroutine (IKSystemEnabled, objectToGrab, animationSpeed, rightHandPosition, lefHandPosition, 
			referencePosition, useRightHand, useLeftHand, useRightElbow, useLeftElbow, rightElbowPosition, lefElbowPosition));
	}

	IEnumerator grabPhysicalObjectCoroutine (bool IKSystemEnabled, GameObject objectToGrab, float animationSpeed, Transform rightHandPosition, 
	                                         Transform lefHandPosition, Transform referencePosition, bool useRightHand, bool useLeftHand, 
	                                         bool useRightElbow, bool useLeftElbow, Transform rightElbowPosition, Transform lefElbowPosition)
	{
		carryingPhysicalObject = true;

		weaponsManager.setCarryingPhysicalObjectState (carryingPhysicalObject);

		weaponsManager.checkIfDisableCurrentWeapon ();

		currentPhysicalObjectGrabbed = objectToGrab;

		checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

		removeCurrentPhysicalObjectToGrabFound (currentPhysicalObjectGrabbed);

		bool firstPersonActive = playerCameraManager.isFirstPersonActive ();
		if (!firstPersonActive) {
			if (powersManager.isAimingPower ()) {
				powersManager.useAimMode ();
			}

			grabbedObjectCursor.SetActive (false);
		}

		Component[] components = objectToGrab.GetComponentsInChildren (typeof(Collider));
		foreach (Collider child in components) {
			grabeObjectColliderList.Add (child as Collider);
			Physics.IgnoreCollision (mainCollider, child, true);
			Physics.IgnoreCollision (child, grabbedObjectClonnedCollider, true);
		}

		if (!firstPersonActive) {
			yield return new WaitForSeconds (0.22f);
		}

		grabbedObjectClonnedColliderTransform.gameObject.SetActive (true);
		grabbedObjectClonnedCollider.size = currentgrabPhysicalObjectSystem.colliderScale;
		grabbedObjectClonnedCollider.center = currentgrabPhysicalObjectSystem.colliderOffset;

		if (referencePosition != null) {
			if (currentgrabPhysicalObjectSystem.isUseReferencePositionForEveryViewActive ()) {
				if (firstPersonActive) {
					currentReferencePosition = currentgrabPhysicalObjectSystem.getReferencePositionFirstPerson ();
				} else {
					currentReferencePosition = currentgrabPhysicalObjectSystem.getReferencePositionThirdPerson ();
				}
			} else {
				currentReferencePosition = referencePosition;
			}
		}

		Transform currentPlaceToCarryPhysicalObjects = placeToCarryPhysicalObjectsThirdPerson;

		if (firstPersonActive) {
			currentPlaceToCarryPhysicalObjects = placeToCarryPhysicalObjectsFirstPerson;
		}

		if (IKSystemEnabled) {
			for (int j = 0; j < handInfoList.Count; j++) {
				if (handInfoList [j].IKGoal == AvatarIKGoal.RightHand) {
					handInfoList [j].handTransform = rightHandPosition;
					handInfoList [j].useHand = useRightHand;
					handInfoList [j].elbowTransform = rightElbowPosition;
					handInfoList [j].useElbow = useRightElbow;
				} else {
					handInfoList [j].handTransform = lefHandPosition;
					handInfoList [j].useHand = useLeftHand;
					handInfoList [j].elbowTransform = lefElbowPosition;
					handInfoList [j].useElbow = useLeftElbow;
				}
			}
		} else {
			if (!firstPersonActive) {
				if (rightHandPosition) {
					currentPlaceToCarryPhysicalObjects = playerControllerManager.getCharacterHumanBone (HumanBodyBones.RightHand);
				} else {
					currentPlaceToCarryPhysicalObjects = playerControllerManager.getCharacterHumanBone (HumanBodyBones.LeftHand);
				}
			}
		}

		IKManager.setGrabedObjectState (true, IKSystemEnabled, handInfoList);

		if (firstPersonActive) {
			objectToGrab.transform.SetParent (currentPlaceToCarryPhysicalObjects);
		} else {
			objectToGrab.transform.SetParent (currentPlaceToCarryPhysicalObjects);
			if (currentReferencePosition) {
				Vector3 localPosition = currentReferencePosition.localPosition;
				Quaternion localRotation = currentReferencePosition.localRotation;

				objectToGrab.transform.localPosition = localPosition;
				objectToGrab.transform.localRotation = localRotation;
			} else {
				objectToGrab.transform.localPosition = Vector3.zero;
				objectToGrab.transform.localRotation = Quaternion.identity;
			}
		}

		playerControllerManager.setWalkSpeedValue (animationSpeed);
		Rigidbody grabedObjectRigidbody = objectToGrab.GetComponent<Rigidbody> ();
		grabedObjectRigidbody.isKinematic = true;

		if (firstPersonActive) {
			translatePhysicalObject (currentPlaceToCarryPhysicalObjects.position);
		}

		if (gravityManager.isPlayerInsiderGravityRoom ()) {
			gravityManager.getCurrentZeroGravityRoom ().removeObjectFromRoom (objectToGrab);
		}
	}

	public void dropPhysicalObject ()
	{
		if (grabbingCoroutine != null) {
			StopCoroutine (grabbingCoroutine);
		}

		if (droppingCoroutine != null) {
			StopCoroutine (droppingCoroutine);
		}

		droppingCoroutine = StartCoroutine (dropObjectCoroutine ());
	}

	IEnumerator dropObjectCoroutine ()
	{
		if (currentPhysicalObjectGrabbed) {
			playerControllerManager.setOriginalWalkSpeed ();

			stopTranslatePhysicalObject ();

			Rigidbody grabedObjectRigidbody = currentPhysicalObjectGrabbed.GetComponent<Rigidbody> ();
			grabedObjectRigidbody.isKinematic = false;
			currentPhysicalObjectGrabbed.transform.SetParent (null);

			carryingPhysicalObject = false;

			weaponsManager.setCarryingPhysicalObjectState (carryingPhysicalObject);

			weaponsManager.checkIfDisableCurrentWeapon ();

			IKManager.setGrabedObjectState (false, false, null);

			currentPhysicalObjectGrabbed = null;

			currentReferencePosition = null;

			grabbedObjectClonnedColliderTransform.gameObject.SetActive (false);

			yield return new WaitForSeconds (0.5f);

			for (int j = 0; j < grabeObjectColliderList.Count; j++) {
				if (grabeObjectColliderList [j]) {
					Physics.IgnoreCollision (mainCollider, grabeObjectColliderList [j], false);
				}
			}
		}
	}

	public bool isCarryingPhysicalObject ()
	{
		return carryingPhysicalObject;
	}

	public void translatePhysicalObject (Vector3 worldPosition)
	{
		stopTranslatePhysicalObject ();
		translateGrabbedPhysicalObject = StartCoroutine (translatePhysicalObjectCoroutine (worldPosition));
	}

	public void stopTranslatePhysicalObject ()
	{
		if (translateGrabbedPhysicalObject != null) {
			StopCoroutine (translateGrabbedPhysicalObject);
		}
	}

	IEnumerator translatePhysicalObjectCoroutine (Vector3 worldPosition)
	{
		float dist = GKC_Utils.distance (currentPhysicalObjectGrabbed.transform.position, worldPosition);

		Vector3 targetPosition = Vector3.zero;
		Quaternion targetRotation = Quaternion.identity;

		if (currentReferencePosition != null) {
			targetPosition = currentReferencePosition.localPosition;
			targetRotation = currentReferencePosition.localRotation;
		}

		float duration = dist / translatePhysicalObjectSpeed;
		float t = 0;

		while (t < 1 && (currentPhysicalObjectGrabbed.transform.localPosition != targetPosition || currentPhysicalObjectGrabbed.transform.localRotation != targetRotation)) {
			t += Time.deltaTime / duration;
			currentPhysicalObjectGrabbed.transform.localPosition = Vector3.Lerp (currentPhysicalObjectGrabbed.transform.localPosition, targetPosition, t);
			currentPhysicalObjectGrabbed.transform.localRotation = Quaternion.Lerp (currentPhysicalObjectGrabbed.transform.localRotation, targetRotation, t);
			yield return null;
		}
	}

	public void checkPhysicalObjectGrabbedPosition (bool firstPersonActive)
	{
		if (currentPhysicalObjectGrabbed) {
			Transform currentPlaceToCarryPhysicalObjects = placeToCarryPhysicalObjectsFirstPerson;

			if (!firstPersonActive) {
				if (currentgrabPhysicalObjectSystem.IKSystemEnabled) {
					currentPlaceToCarryPhysicalObjects = placeToCarryPhysicalObjectsThirdPerson;
				} else {
					if (currentgrabPhysicalObjectSystem.rightHandPosition) {
						currentPlaceToCarryPhysicalObjects = playerControllerManager.getCharacterHumanBone (HumanBodyBones.RightHand);
					} else {
						currentPlaceToCarryPhysicalObjects = playerControllerManager.getCharacterHumanBone (HumanBodyBones.LeftHand);
					}
				}
			}

			if (firstPersonActive) {
				currentPhysicalObjectGrabbed.transform.SetParent (currentPlaceToCarryPhysicalObjects);
			} else {
				currentPhysicalObjectGrabbed.transform.SetParent (currentPlaceToCarryPhysicalObjects);
			}

			if (currentReferencePosition != null) {
				if (currentgrabPhysicalObjectSystem.isUseReferencePositionForEveryViewActive ()) {
					if (firstPersonActive) {
						currentReferencePosition = currentgrabPhysicalObjectSystem.getReferencePositionFirstPerson ();
					} else {
						currentReferencePosition = currentgrabPhysicalObjectSystem.getReferencePositionThirdPerson ();
					}
				} 

				Vector3 localPosition = currentReferencePosition.localPosition;
				Quaternion localRotation = currentReferencePosition.localRotation;

				currentPhysicalObjectGrabbed.transform.localPosition = localPosition;
				currentPhysicalObjectGrabbed.transform.localRotation = localRotation;
			} else {
				currentPhysicalObjectGrabbed.transform.localPosition = Vector3.zero;
				currentPhysicalObjectGrabbed.transform.localRotation = Quaternion.identity;
			}
		}
	}

	public void checkIfDropObjectIfNotPhysical (bool disableAimState)
	{
		if (disableAimState) {
			setAimingState (false);
		}
		if (!isCarryingPhysicalObject ()) {
			dropObject ();
		}
	}

	public bool isPauseCameraMouseWheelWhileObjectGrabbedActive ()
	{
		return pauseCameraMouseWheelWhileObjectGrabbed;
	}

	//CALL INPUT FUNCTIONS
	public void inputGrabObject ()
	{
		//if the player is in aim mode, grab an object
		if (!playerControllerManager.usingDevice && (aiming || currentPhysicalObjectToGrabFound) && !objectHeld && grabObjectsEnabled) {
			grabObject ();
		}
	}

	public void inputHoldToLaunchObject ()
	{
		//if the drop button is being holding, add force to the final velocity of the drooped object
		if (grabbed) {
			addForceToLaunchObject ();
		}
	}

	public void inputReleaseToLaunchObject ()
	{
		//when the button is released, check the amount of strength accumulated
		if (grabbed) {
			launchObject ();
		}
	}

	public void inputSetRotateObjectState (bool rotationEnabled)
	{
		if (objectCanBeRotated && objectHeld && !usingDoor && !carryingPhysicalObject) {
			if (rotationEnabled) {
				playerCameraManager.changeCameraRotationState (false);
				rotatingObject = true;
			} else {

				playerCameraManager.changeCameraRotationState (true);
				rotatingObject = false;
			}
		}
	}

	public void inputZoomObject (bool zoomIn)
	{
		if (objectHeld && regularObject && canUseZoomWhileGrabbed) {
			if (zoomIn) {
				changeGrabbedZoom (1);
			} else {
				changeGrabbedZoom (-1);
			}
		}
	}

	public void inputResetFixedGrabedTransformPosition ()
	{
		if (objectHeld && regularObject && canUseZoomWhileGrabbed) {
			resetFixedGrabedTransformPosition ();
		}
	}


	[System.Serializable]
	public class handInfo
	{
		public string Name;
		public AvatarIKGoal IKGoal;
		public AvatarIKHint IKHint;

		public float currentHandWeight;
		public float weightLerpSpeed;

		public bool useHand;

		public Transform handTransform;

		public Vector3 handPosition;
		public Quaternion handRotation;

		public bool useElbow;

		public Transform elbowTransform;

		public Vector3 elbowPosition;
	}
}