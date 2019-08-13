using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class examineObjectSystem : MonoBehaviour
{
	public Transform objectTransform;
	public bool objectCanBeRotated;
	public float rotationSpeed;

	public bool horizontalRotationEnabled = true;
	public bool verticalRotationEnabled = true;

	public bool zoomCanBeUsed;
	public bool rotationEnabled = true;

	public bool activateActionScreen = true;
	public string actionScreenName = "Examine Object";

	public bool useSecundaryCancelExamineFunction;
	public UnityEvent secundaryCancelExamineFunction = new UnityEvent ();

	public bool useExamineMessage;
	[TextArea (1, 10)] public string examineMessage;

	public bool pressPlacesInOrder;
	public int currentPlacePressedIndex;
	public bool useIncorrectPlacePressedMessage;
	[TextArea (1, 10)] public string incorrectPlacePressedMessage;
	public float incorrectPlacePressedMessageDuration;

	public bool objectUsesCanvas;
	public Canvas mainCanvas;
	public bool useTriggerOnTopOfCanvas;
	public GameObject triggerOnTopOfCanvas;

	public bool usingDevice;

	public List<examinePlaceInfo> examinePlaceList = new List<examinePlaceInfo> ();

	playerInputManager playerInput;
	bool touchPlatform;
	Touch currentTouch;
	bool touching;
	moveDeviceToCamera moveDeviceToCameraManager;
	public GameObject currentPlayer;
	usingDevicesSystem usingDevicesManager;
	examineObjectSystemPlayerManagement examineObjectSystemPlayerManager;
	Camera deviceCamera;

	bool showingMessage;
	electronicDevice electronicDeviceManager;
	Collider mainCollider;
	AudioSource mainAudioSource;
	Ray ray;
	RaycastHit hit;

	void Start ()
	{
		touchPlatform = touchJoystick.checkTouchPlatform ();
		moveDeviceToCameraManager = GetComponent<moveDeviceToCamera> ();
		if (objectTransform == null) {
			objectTransform = transform;
		}
		electronicDeviceManager = GetComponent<electronicDevice> ();
		mainCollider = GetComponent<Collider> ();
		mainAudioSource = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (usingDevice) {
			if (objectCanBeRotated && rotationEnabled) {
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

					if (currentTouch.phase == TouchPhase.Began) {
						touching = true;

						if (objectUsesCanvas && useTriggerOnTopOfCanvas) {
							ray = deviceCamera.ScreenPointToRay (currentTouch.position);
							if (Physics.Raycast (ray, out hit, 20)) 
							{
								if (hit.collider.gameObject == triggerOnTopOfCanvas) {
									touching = false;
								}
							}
						}
					}

					if (currentTouch.phase == TouchPhase.Ended) {
						touching = false;
					}

					if (touching && (currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary)) {
						if (horizontalRotationEnabled) {
							objectTransform.RotateAroundLocal (Vector3.up, -Mathf.Deg2Rad * rotationSpeed * playerInput.getPlayerMovementAxis ("mouse").x);
						}

						if (verticalRotationEnabled) {
							objectTransform.RotateAroundLocal (Vector3.right, Mathf.Deg2Rad * rotationSpeed * playerInput.getPlayerMovementAxis ("mouse").y);
						}
					}
				}
			}
		}
	}

	public void showExamineMessage (bool state)
	{
		showingMessage = state;
		getPlayerComponents ();
		if (showingMessage) {
			usingDevicesManager.checkShowObjectMessage (examineMessage, 0);
		} else {
			usingDevicesManager.stopShowObjectMessage ();
		}
	}

	public void stopExamineDevice ()
	{
		if (usingDevicesManager) {
			usingDevicesManager.useDevice ();
		}
	}

	public void disableAndRemoveExamineDevice ()
	{
		if (usingDevicesManager) {
			moveDeviceToCameraManager.setIgnoreDeviceTriggerEnabledState (true);
			mainCollider.enabled = false;
			usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
		}
	}

	public void cancelExamine ()
	{
		if (secundaryCancelExamineFunction.GetPersistentEventCount () > 0) {
			secundaryCancelExamineFunction.Invoke ();
		}
	}

	public void pauseOrResumePlayerInteractionButton (bool state)
	{
		if (usingDevicesManager) {
			usingDevicesManager.setUseDeviceButtonEnabledState (!state);
		}
	}

	//enable or disable the device
	public void examineDevice ()
	{
		usingDevice = !usingDevice;

		if (usingDevice) {
			getPlayerComponents ();
		} else {
			showExamineMessage (false);
		}

		if (activateActionScreen) {
			playerInput.enableOrDisableActionScreen (actionScreenName, usingDevice);
		}

		if (examineObjectSystemPlayerManager) {
			examineObjectSystemPlayerManager.setExaminingObjectState (usingDevice);
		}
	}

	public void setExamineDeviceState (bool state)
	{
		usingDevice = state;
		if (!usingDevice) {
			touching = false;
		}

		if (examineObjectSystemPlayerManager) {
			examineObjectSystemPlayerManager.setExaminingObjectState (usingDevice);
		}
	}

	public void setRotationState (bool state)
	{
		rotationEnabled = state;
	}

	public void getPlayerComponents ()
	{
		currentPlayer = electronicDeviceManager.getCurrentPlayer ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
		playerInput = currentPlayer.GetComponent<playerInputManager> ();
		examineObjectSystemPlayerManager = currentPlayer.GetComponentInChildren<examineObjectSystemPlayerManagement> ();
		examineObjectSystemPlayerManager.setcurrentExanimeObject (GetComponent<examineObjectSystem> ());

		if (objectUsesCanvas) {
			deviceCamera = usingDevicesManager.getExaminateDevicesCamera ();
			mainCanvas.worldCamera = deviceCamera;
		}
	}

	public void checkExaminePlaceInfo (Transform examinePlaceToCheck)
	{
		for (int i = 0; i < examinePlaceList.Count; i++) {
			if (!examinePlaceList [i].elementPlaceDisabled && examinePlaceList [i].examinePlaceTransform == examinePlaceToCheck) {

				if (pressPlacesInOrder) {
					if (i == currentPlacePressedIndex) {
						currentPlacePressedIndex++;
					} else {
						currentPlacePressedIndex = 0;

						if (useIncorrectPlacePressedMessage) {
							usingDevicesManager.checkShowObjectMessage (incorrectPlacePressedMessage, incorrectPlacePressedMessageDuration);
						}
						return;
					}
				}

				if (examinePlaceList [i].showMessageOnPress) {
					usingDevicesManager.checkShowObjectMessage (examinePlaceList [i].messageOnPress, examinePlaceList [i].messageDuration);
				}

				if (examinePlaceList [i].stopUseObjectOnPress) {
					usingDevicesManager.useDevice ();
				}

				if (examinePlaceList [i].disableObjectInteractionOnPress) {
					moveDeviceToCameraManager.setIgnoreDeviceTriggerEnabledState (true);
					mainCollider.enabled = false;
					if (examinePlaceList [i].removeObjectFromDevicesList) {
						usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
					}
				}

				if (examinePlaceList [i].useEventOnPress) {
					if (examinePlaceList [i].sendPlayerOnEvent) {
						examinePlaceList [i].eventToSendPlayer.Invoke (currentPlayer);
					}
					examinePlaceList [i].eventOnPress.Invoke ();
				}

				if (examinePlaceList [i].resumePlayerInteractionButtonOnPress) {
					usingDevicesManager.setUseDeviceButtonEnabledState (true);
				}

				if (examinePlaceList [i].pausePlayerInteractionButtonOnPress) {
					usingDevicesManager.setUseDeviceButtonEnabledState (false);
				}

				if (examinePlaceList [i].disableElementPlaceAfterPress) {
					examinePlaceList [i].elementPlaceDisabled = true;
				}

				if (examinePlaceList [i].useSoundOnPress) {
					if (mainAudioSource) {
						mainAudioSource.PlayOneShot (examinePlaceList [i].soundOnPress);
					}
				}

				return;
			}
		}
	}

	public void setExaminePlaceEnabledState (Transform examinePlaceToCheck)
	{
		for (int i = 0; i < examinePlaceList.Count; i++) {
			if (examinePlaceList [i].examinePlaceTransform == examinePlaceToCheck) {
				examinePlaceList [i].elementPlaceDisabled = true;
				return;
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputSetZoomValue (bool state)
	{
		if (usingDevice && objectCanBeRotated && rotationEnabled) {
				
			if (zoomCanBeUsed) {
				if (state) {
					moveDeviceToCameraManager.changeDeviceZoom (true);
				} else {

					moveDeviceToCameraManager.changeDeviceZoom (false);
				}
			}
		}
	}

	public void inputResetRotation ()
	{
		if (usingDevice && objectCanBeRotated && rotationEnabled) {

			if (zoomCanBeUsed) {

				moveDeviceToCameraManager.resetRotation ();
			}
		}
	}

	public void inputResetRotationAndPosition ()
	{
		if (usingDevice && objectCanBeRotated && rotationEnabled) {

			if (zoomCanBeUsed) {
				
				moveDeviceToCameraManager.resetRotationAndPosition ();
			}
		}
	}

	public void inputCancelExamine ()
	{
		if (usingDevice && objectCanBeRotated && rotationEnabled) {
			if (useSecundaryCancelExamineFunction) {
				cancelExamine ();
			}
		}
	}

	public void inputCheckIfMessage ()
	{
		if (usingDevice && objectCanBeRotated && rotationEnabled) {
			if (useExamineMessage) {
				showExamineMessage (!showingMessage);
			}
		}
	}

	[System.Serializable]
	public class examinePlaceInfo
	{
		public string Name;

		public Transform examinePlaceTransform;

		public bool showMessageOnPress;
		[TextArea (1, 10)] public string messageOnPress;
		public float messageDuration;

		public bool useEventOnPress;
		public UnityEvent eventOnPress;

		public bool sendPlayerOnEvent;
		public eventParameters.eventToCallWithGameObject eventToSendPlayer;

		public bool stopUseObjectOnPress;
		public bool disableObjectInteractionOnPress;
		public bool removeObjectFromDevicesList;

		public bool resumePlayerInteractionButtonOnPress;
		public bool pausePlayerInteractionButtonOnPress;

		public bool disableElementPlaceAfterPress;

		public bool elementPlaceDisabled;

		public bool useSoundOnPress;
		public AudioClip soundOnPress;
	}
}