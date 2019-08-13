using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class usingDevicesSystem : MonoBehaviour
{
	public bool canUseDevices;
	public GameObject touchButton;
	public GameObject iconButton;
	public RectTransform iconButtonRectTransform;
	public Text actionText;
	public Text keyText;
	public Text objectNameText;
	public string useDeviceFunctionName = "activateDevice";

	public string setCurrentUserOnDeviceFunctionName = "setCurrentUser";

	public List<string> tagsForDevices = new List<string> ();
	public Camera examinateDevicesCamera;
	public bool usePickUpAmountIfEqualToOne;
	public bool usedByAI;

	public bool driving;
	public bool iconButtonCanBeShown = true;
	public GameObject objectToUse;

	public GameObject interactionMessageGameObject;
	public Text interactionMessageText;

	public GameObject currentVehicle;
	public List<deviceInfo> deviceList = new List<deviceInfo> ();

	public List<GameObject> deviceGameObjectList = new List<GameObject> ();

	public deviceInfo currentDeviceInfo;

	public LayerMask layer;
	public float raycastDistance;

	public bool searchingDevicesWithRaycast;
	public bool secondaryDeviceFound;

	public bool showUseDeviceIconEnabled = true;
	public bool useDeviceButtonEnabled = true;
	public bool useFixedDeviceIconPosition;
	public bool deviceOnScreenIfUseFixedIconPosition;

	public bool getClosestDeviceToCameraCenter;

	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter = 20;

	public bool currentDeviceIsPickup;

	public bool holdButtonToTakePickupsAround;
	public float holdButtonTime = 0.5f;

	public bool useDeviceFoundShader;
	public Shader deviceFoundShader;
	public float shaderOutlineWidth;
	public Color shaderOutlineColor;

	public bool examiningObject;

	public playerController playerControllerManager;
	public grabObjects grabObjectsManager;
	public menuPause pauseManager;
	public ragdollActivator ragdollManager;
	public playerInputManager playerInput;
	public Camera mainCamera;
	public playerCamera playerCameraManager;
	public Transform mainCameraTransform;

	float lastTimePressedButton;
	bool holdingButton;

	Touch currentTouch;

	deviceStringAction deviceStringManager;

	Vector3 currentIconPosition;

	Coroutine showObjectMessageCoroutine;

	RaycastHit hit;
	deviceStringAction secondaryDeviceStringManager;
	GameObject currentRaycastFoundObject;
	deviceInfo secundaryDeviceInfo;

	Vector3 currentPosition;

	string currentDeviceActionText;
	public string currenDeviceActionName;

	Vector3 devicePosition;
	Vector3 screenPoint;
	Vector3 centerScreen;
	float currentDistanceToTarget;
	float minDistanceToTarget;
	int currentTargetIndex;
	bool deviceCloseEnoughToScreenCenter;

	GameObject objectToRemoveAferStopUse;

	GameObject currentDeviceToUseFound;

	Vector3 originalDeviceIconPosition;

	bool firstPersonActive;
	bool cameraViewChecked;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	void Start ()
	{
		centerScreen = new Vector3 (Screen.width / 2, Screen.height / 2, 0);

		if (iconButton) {
			originalDeviceIconPosition = iconButton.transform.position;
		}

		if (!usedByAI) {
			mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
			halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
			usingScreenSpaceCamera = pauseManager.getMainCanvas ().renderMode == RenderMode.ScreenSpaceCamera;
		}
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}
			
		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		//set the icon button above the device to use, just to indicate to the player that he can activate a device by pressing T
		if (deviceList.Count > 0 && !examiningObject) {
			int index = getclosestDevice ();
			if (iconButtonCanBeShown && index != -1) {
				currentDeviceInfo = deviceList [index];

				//show a secondary device string action inside the current object to use
				if (searchingDevicesWithRaycast) {
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, raycastDistance, layer)) {
						if (currentRaycastFoundObject != hit.collider.gameObject) {
							currentRaycastFoundObject = hit.collider.gameObject;
							secondaryDeviceStringManager = currentRaycastFoundObject.GetComponent<deviceStringAction> ();
							if (secondaryDeviceStringManager) {
								secundaryDeviceInfo = new deviceInfo ();
								secundaryDeviceInfo.deviceGameObject = currentRaycastFoundObject;

								if (secondaryDeviceStringManager.useSeparatedTransformForEveryViewEnabled ()) {
									if (firstPersonActive) {
										secundaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getTransformForIconFirstPerson ();
									} else {
										secundaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getTransformForIconThirdPerson ();
									}
								} else {
									secundaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getRegularTransformForIcon ();
								}

								secundaryDeviceInfo.useTransformForStringAction = secondaryDeviceStringManager.useTransformForStringAction;
								secundaryDeviceInfo.actionOffset = secondaryDeviceStringManager.actionOffset;
								secundaryDeviceInfo.useLocalOffset = secondaryDeviceStringManager.useLocalOffset;

								string deviceAction = secondaryDeviceStringManager.getDeviceAction ();
								setKeyText (true);

								currentDeviceActionText = deviceAction;
								actionText.text = deviceAction;
								objectNameText.text = secondaryDeviceStringManager.deviceName;
								secondaryDeviceFound = true;
							} else {
								secondaryDeviceFound = false;

								string deviceAction = deviceStringManager.getDeviceAction ();
								currentDeviceActionText = deviceAction;
								actionText.text = deviceAction;
								objectNameText.text = deviceStringManager.deviceName;
							}
						} 
					} else {
						removeSecondaryDeviceInfo ();
					}

					if (secondaryDeviceFound) {
						iconButtonCanBeShown = true;
						currentDeviceInfo = secundaryDeviceInfo;
					} else {
						if (!deviceStringManager.showIcon) {
							currentDeviceInfo = null;
						}
						removeSecondaryDeviceInfo ();
					}
				}

				if (showUseDeviceIconEnabled) {
					if (currentDeviceInfo != null) {
						if (useFixedDeviceIconPosition && !deviceOnScreenIfUseFixedIconPosition) {
							enableOrDisableIconButton (true);
							iconButton.transform.position = originalDeviceIconPosition;
						} else {
							//check if the current device to use has two separated positions to show the device icon on third person and firs person
							if (!useFixedDeviceIconPosition && currentDeviceInfo.useSeparatedTransformForEveryView) {
								if (firstPersonActive) {
									if (!cameraViewChecked) {
										setCurrentDeviceStringManagerInfo (currentDeviceInfo);
										cameraViewChecked = true;
									}
								} else {
									if (cameraViewChecked) {
										setCurrentDeviceStringManagerInfo (currentDeviceInfo);
										cameraViewChecked = false;
									}
								}
							}

							if (currentDeviceInfo.positionToIcon) {
								currentIconPosition = currentDeviceInfo.positionToIcon.position;
								if (!currentDeviceInfo.useTransformForStringAction) {
									if (currentDeviceInfo.useLocalOffset) {
										currentIconPosition += currentDeviceInfo.positionToIcon.up * currentDeviceInfo.actionOffset;
									} else {
										currentIconPosition += Vector3.up * currentDeviceInfo.actionOffset;
									}
								}

								if (usingScreenSpaceCamera) {
									screenPoint = mainCamera.WorldToViewportPoint (currentIconPosition);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
								} else {
									screenPoint = mainCamera.WorldToScreenPoint (currentIconPosition);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
								}

								if (targetOnScreen) {
									if (useFixedDeviceIconPosition) {
										iconButton.transform.position = originalDeviceIconPosition;
									} else {

										if (usingScreenSpaceCamera) {
											iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, 
												(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
										
											iconButtonRectTransform.anchoredPosition = iconPosition2d;
										} else {
											iconButton.transform.position = screenPoint;
										}
									}
									enableOrDisableIconButton (true);
								} else {
									enableOrDisableIconButton (false);
								}
							} else {
								enableOrDisableIconButton (false);
							}
						}
					} else {
						enableOrDisableIconButton (false);
					}
				}
			} else {
				enableOrDisableIconButton (false);
			}
		} else {
			enableOrDisableIconButton (false);
		}
	}

	public void removeSecondaryDeviceInfo ()
	{
		currentRaycastFoundObject = null;
		secondaryDeviceStringManager = null;
		secundaryDeviceInfo = null;
	}

	public void enableOrDisableIconButton (bool state)
	{
		if (iconButton.activeSelf != state) {
			setIconButtonState (state);
		}
	}

	public int getclosestDevice ()
	{
		currentTargetIndex = -1;
		minDistanceToTarget = Mathf.Infinity;
		currentPosition = transform.position;

		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject) {
				devicePosition = deviceList [i].deviceGameObject.transform.position;
				if (getClosestDeviceToCameraCenter) {
					screenPoint = mainCamera.WorldToScreenPoint (devicePosition);

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
							currentTargetIndex = i;
						}
					}
				} else {
					currentDistanceToTarget = GKC_Utils.distance (devicePosition, currentPosition);
					if (currentDistanceToTarget < minDistanceToTarget) {
						minDistanceToTarget = currentDistanceToTarget;
						currentTargetIndex = i;
					}
				}
			} else {
				deviceList.RemoveAt (i);
				deviceGameObjectList.RemoveAt (i);
				i = 0;
			}
		}

		if (getClosestDeviceToCameraCenter && useMaxDistanceToCameraCenter) {
			if (currentTargetIndex == -1 && objectToUse != null) {

				checkIfSetOriginalShaderToPreviousDeviceFound (objectToUse);

				objectToUse = null;
				return -1;
			}
		}

		if (currentTargetIndex != -1) {
			if (objectToUse != deviceList [currentTargetIndex].deviceGameObject) {

				if (objectToUse != null) {
					checkIfSetOriginalShaderToPreviousDeviceFound (objectToUse);
				}
					
				objectToUse = deviceList [currentTargetIndex].deviceGameObject;

				setCurrentDeviceStringManagerInfo (deviceList [currentTargetIndex]);

				removeSecondaryDeviceInfo ();

				checkIfSetNewShaderToDeviceFound (objectToUse);
			}
		}
		return currentTargetIndex;
	}

	public void setCurrentDeviceStringManagerInfo (deviceInfo deviceToUseInfo)
	{
		//get the action made by the current device
		string deviceAction = "";
		deviceStringManager = objectToUse.GetComponent<deviceStringAction> ();
		deviceToUseInfo.useSeparatedTransformForEveryView = deviceStringManager.useSeparatedTransformForEveryViewEnabled ();
		if (deviceToUseInfo.useSeparatedTransformForEveryView) {
			if (firstPersonActive) {
				deviceToUseInfo.positionToIcon = deviceStringManager.getTransformForIconFirstPerson ();
			} else {
				deviceToUseInfo.positionToIcon = deviceStringManager.getTransformForIconThirdPerson ();
			}
		} else {
			deviceToUseInfo.positionToIcon = deviceStringManager.getRegularTransformForIcon ();
		}

		deviceToUseInfo.useTransformForStringAction = deviceStringManager.useTransformForStringAction;
		deviceToUseInfo.actionOffset = deviceStringManager.actionOffset;
		deviceToUseInfo.useLocalOffset = deviceStringManager.useLocalOffset;

		deviceAction = deviceStringManager.getDeviceAction ();

		//show the icon in the hud of the screen according to the deviceStringAction component
		if ((deviceStringManager.showIcon && deviceAction.Length > 0 && deviceStringManager.iconEnabled) || searchingDevicesWithRaycast) {
			iconButtonCanBeShown = true;
		} else {
			iconButtonCanBeShown = false;
		}

		//enable the interection button in the touch screen
		if (deviceStringManager.showTouchIconButton) {
			enableOrDisableTouchButton (true);
		} else {
			enableOrDisableTouchButton (false);
		}

		setKeyText (true);

		if (actionText) {
			currentDeviceActionText = deviceAction;
			actionText.text = deviceAction;
			currenDeviceActionName = deviceStringManager.deviceName;

			setInteractionButtonName ();
		}
	}

	public void checkIfSetNewShaderToDeviceFound (GameObject objectToCheck)
	{
		if (useDeviceFoundShader) {
			//print ("new on device" + currenDeviceActionName);
	
			currentDeviceToUseFound = objectToCheck;

			outlineObjectSystem currentOutlineObjectSystem = objectToCheck.GetComponent<outlineObjectSystem> ();
			if (currentOutlineObjectSystem) {
				currentOutlineObjectSystem.setOutlineState (true, deviceFoundShader, shaderOutlineWidth, shaderOutlineColor, playerControllerManager);
			}
		}
	}

	public void checkIfSetOriginalShaderToPreviousDeviceFound (GameObject objectToCheck)
	{
		if (useDeviceFoundShader) {
			print ("original on device" + objectToCheck.name);

			outlineObjectSystem currentOutlineObjectSystem = objectToCheck.GetComponent<outlineObjectSystem> ();
			if (currentOutlineObjectSystem) {
				if (!grabObjectsManager.useObjectToGrabFoundShader || !grabObjectsManager.isCurrentObjectToGrabFound (currentOutlineObjectSystem.getMeshParent ())) {
					currentOutlineObjectSystem.setOutlineState (false, null, 0, Color.white, playerControllerManager);
				}
			}
			currentDeviceToUseFound = null;
		}
	}

	public bool isCurrentDeviceToUseFound (GameObject objectToCheck)
	{
		if (currentDeviceToUseFound == objectToCheck) {
			return true;
		}
		return false;
	}

	public void setInteractionButtonName ()
	{
		if (objectToUse == null) {
			return;
		}

		if (objectNameText) {
			objectNameText.text = currenDeviceActionName;
			pickUpObject currentPickUpObject = objectToUse.GetComponentInParent<pickUpObject> ();
			if (currentPickUpObject) {
				int currentAmount = currentPickUpObject.amount;
				if (currentAmount > 1 || (usePickUpAmountIfEqualToOne && currentAmount == 1)) {
					objectNameText.text += " x " + currentAmount.ToString ();
				}
				currentDeviceIsPickup = true;
			} else {
				currentDeviceIsPickup = false;
			}
		}
	}

	//check if the player enters or exits the trigger of a device

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!isInTagsForDevicesList (col.gameObject)) {
			return;
		}

		if (driving) {
			return;
		}
			
		if (isEnter) {
			//if the player is driving, he can't use any other device
			if (!canUseDevices || driving) {
				return;
			}

			if (usedByAI) {
				return;
			}

			GameObject usableObjectFound = col.gameObject;

			if (!existInDeviceList (usableObjectFound)) {
				addDeviceToList (usableObjectFound);

				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();
				if (deviceStringManager.useRaycastToDetectDeviceParts) {
					searchingDevicesWithRaycast = true;
				} 
			}
		} else {
			//when the player exits from the trigger of a device, if he is not driving, set the device to null
			//else the player is driving, so the current device is that vehicle, so the device can't be changed
			GameObject usableObjectFound = col.gameObject;

			if (existInDeviceList (usableObjectFound)) {

				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();
				if (deviceStringManager.useRaycastToDetectDeviceParts) {
					searchingDevicesWithRaycast = false;
				} 

				if (driving) {
					if (usableObjectFound != currentVehicle) {
						removeDeviceFromList (usableObjectFound);
					}
				} else {
					removeDeviceFromList (usableObjectFound);
				}

				checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);
			}

			if (!driving) {
				enableOrDisableTouchButton (false);
			}

			if (deviceList.Count == 0) {
				objectToUse = null;
				iconButtonCanBeShown = false;
				currentDeviceInfo = null;
			}
		}
	}

	public void addDeviceToList (GameObject deviceToAdd)
	{
		deviceInfo newDeviceInfo = new deviceInfo ();
		newDeviceInfo.deviceGameObject = deviceToAdd;
		deviceList.Add (newDeviceInfo);

		deviceGameObjectList.Add (deviceToAdd);
	}

	//call the device action
	public void useDevice ()
	{
		if (!ragdollManager.canMove) {
			return;
		}
			
		GameObject usableObjectFound = objectToUse;

		if (secundaryDeviceInfo != null) {
			usableObjectFound = secundaryDeviceInfo.deviceGameObject;
		}

		if (usableObjectFound != null && canUseDevices) {
				
			vehicleHUDManager currentVehicleHUDManager = usableObjectFound.GetComponent<vehicleHUDManager> ();
			if (currentVehicleHUDManager) {
				currentVehicle = usableObjectFound;
				currentVehicleHUDManager.setCurrentPassenger (gameObject);
			}

			usableObjectFound.SendMessage (setCurrentUserOnDeviceFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);

			usableObjectFound.SendMessage (useDeviceFunctionName, SendMessageOptions.DontRequireReceiver);

			if (playerControllerManager.isUsingDevice ()) {
				checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);

			} else {
				if (existInDeviceList (usableObjectFound)) {
					//print (usableObjectFound.name + " in device list");
					checkIfSetNewShaderToDeviceFound (usableObjectFound);
				} else {
					checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);
					//print (usableObjectFound.name + " not in device list");
				}
			}

			if (deviceStringManager) {
				if (deviceStringManager.setUsingDeviceState) {
					deviceStringManager.checkSetUsingDeviceState ();
				}

				if (deviceStringManager.usingDevice) {
					if (deviceStringManager.hideIconOnUseDevice) {
						iconButtonCanBeShown = false;
					}
				} else {
					if (deviceStringManager.showIconOnStopUseDevice) {
						iconButtonCanBeShown = true;
					}
				}
			}

			checkIfRemoveDeviceFromList ();

			if (!usableObjectFound) {
				return;
			}
		
			if (usableObjectFound.GetComponent<useInventoryObject> ()) {
				return;
			}

			if (secundaryDeviceInfo == null) {
				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();
			}

			//if the device is a turret or a chest, disable the icon
			if (deviceStringManager) {
				if (deviceStringManager.disableIconOnPress) {
					OnTriggerExit (usableObjectFound.GetComponent<Collider> ());
					return;
				}

				if (deviceStringManager.hideIconOnPress) {
					iconButtonCanBeShown = false;
				}

				if (deviceStringManager.reloadDeviceActionOnPress) {
					checkDeviceName ();
				}
			}

			setInteractionButtonName ();
		}
	}

	public void useCurrentDevice (GameObject currentObjectToUse)
	{
		//print ("Device To Use " + currentObjectToUse.name);
		objectToUse = currentObjectToUse;
		useDevice ();
	}

	public void setObjectToRemoveAferStopUse (GameObject objectToRemove)
	{
		objectToRemoveAferStopUse = objectToRemove;
	}

	public void checkIfRemoveDeviceFromList ()
	{
		if (!playerControllerManager.isUsingDevice ()) {
			if (objectToUse && objectToRemoveAferStopUse && objectToUse == objectToRemoveAferStopUse) {
				removeDeviceFromList (objectToRemoveAferStopUse);
				objectToRemoveAferStopUse = null;
				getclosestDevice ();
			}
		}
	}

	public void takePickupsAround ()
	{
		if (!ragdollManager.canMove || !canUseDevices) {
			return;
		}

		for (int i = 0; i < deviceList.Count; i++) {
			GameObject usableObjectFound = deviceList [i].deviceGameObject;

			if (usableObjectFound) {
				
				pickUpObject currentPickUpObject = usableObjectFound.GetComponentInParent<pickUpObject> ();
				if (currentPickUpObject) {
					usableObjectFound.SendMessage (useDeviceFunctionName, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		getclosestDevice ();
	}

	public void enableOrDisableTouchButton (bool state)
	{
		if (touchButton) {
			touchButton.SetActive (state);
		}
	}

	//disable the icon showed when the player is inside a device's trigger
	public void disableIcon ()
	{
		enableOrDisableTouchButton (false);
		iconButtonCanBeShown = true;
		driving = false;
		objectToUse = null;
	}

	public void setIconButtonCanBeShownState (bool state)
	{
		iconButtonCanBeShown = state;
	}

	public void setIconButtonState (bool state)
	{
		iconButton.SetActive (state);
	}

	public bool getCurrentIconButtonState ()
	{
		return iconButton.activeSelf;
	}

	public void removeVehicleFromList ()
	{
		if (currentVehicle) {
			removeDeviceFromList (currentVehicle);
		}
	}

	public void setDrivingState (bool state)
	{
		//if the player is driving, and he is inside the trigger of other vehicle, disable the icon to use the other vehicle
		driving = state;
		iconButtonCanBeShown = !state;
	}

	public void setCurrentVehicle (GameObject vehicle)
	{
		currentVehicle = vehicle;
	}

	public void removeCurrentVehicle (GameObject vehicle)
	{
		if (driving) {
			return;
		}

		if (vehicle == currentVehicle) {
			currentVehicle = null;
		}
	}

	public void clearDeviceList ()
	{
		deviceList.Clear ();

		deviceGameObjectList.Clear ();
	}

	public void clearDeviceListButOne (GameObject objectToKeep)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject != objectToKeep) {
				deviceList.RemoveAt (i);
				deviceGameObjectList.RemoveAt (i);
				i = 0;
			}
		}
	}

	public void checkDeviceName ()
	{
		GameObject usableObjectFound = objectToUse;

		if (secundaryDeviceInfo != null) {
			usableObjectFound = secundaryDeviceInfo.deviceGameObject;
		}

		if (usableObjectFound) {
			deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();
			if (deviceStringManager) {
				string deviceAction = "";
				deviceAction = deviceStringManager.getDeviceAction ();
				setKeyText (true);
				currentDeviceActionText = deviceAction;
				actionText.text = deviceAction;
				objectNameText.text = deviceStringManager.deviceName;
			}
		}
	}

	public bool existInDeviceList (GameObject objecToCheck)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject == objecToCheck) {
				return true;
			}
		}
		return false;
	}

	public void removeDeviceFromList (GameObject objectToRemove)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject == objectToRemove) {
				deviceList.RemoveAt (i);
				deviceGameObjectList.RemoveAt (i);
			}
		}

		if (deviceList.Count == 0) {
			objectToUse = null;
			currentDeviceInfo = null;
		}
	}

	Coroutine removeDeviceCoroutine;

	public void removeDeviceFromListExternalCall (GameObject objectToRemove)
	{
		stopRemoveDeviceFromListExternalCallCoroutine ();
		removeDeviceCoroutine = StartCoroutine (removeDeviceFromListExternalCallCoroutine (objectToRemove));
	}

	public void stopRemoveDeviceFromListExternalCallCoroutine ()
	{
		if (removeDeviceCoroutine != null) {
			StopCoroutine (removeDeviceCoroutine);
		}
	}

	IEnumerator removeDeviceFromListExternalCallCoroutine (GameObject objectToRemove)
	{
		yield return new WaitForSeconds (0.01f);

		removeDeviceFromList (objectToRemove);
		checkIfSetOriginalShaderToPreviousDeviceFound (objectToRemove);
	}

	public void removeDeviceFromListUsingParent (GameObject objectToRemove)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject.transform.IsChildOf (objectToRemove.transform)) {
				removeDeviceFromList (deviceList [i].deviceGameObject);
			}
		}
	}

	public bool isInTagsForDevicesList (GameObject objectToCheck)
	{
		if (tagsForDevices.Contains (objectToCheck.tag)) {
			return true;
		}
		return false;
	}

	public void setExamineteDevicesCameraState (bool state)
	{
		examinateDevicesCamera.enabled = state;

		examiningObject = state;
	}

	public Camera getExaminateDevicesCamera ()
	{
		return examinateDevicesCamera;
	}

	public bool isExaminingObject ()
	{
		return examiningObject;
	}

	public void setKeyText (bool state)
	{
		//set the key text in the icon with the current action
		if (keyText) {
			if (state) {
				keyText.text = "[" + playerInput.getButtonKey ("Activate Device") + "]";
			} else {
				keyText.text = "";
			}
		}
	}

	public void checkShowObjectMessage (string message, float waitTime)
	{
		if (showObjectMessageCoroutine != null) {
			StopCoroutine (showObjectMessageCoroutine);
		}
		showObjectMessageCoroutine = StartCoroutine (showObjectMessage (message, waitTime));
	}

	IEnumerator showObjectMessage (string message, float waitTime)
	{
		interactionMessageText.text = message;
		interactionMessageGameObject.SetActive (true);
		yield return new WaitForSeconds (waitTime);
		if (waitTime > 0) {
			interactionMessageGameObject.SetActive (false);
		}
	}

	public void stopShowObjectMessage ()
	{
		if (showObjectMessageCoroutine != null) {
			StopCoroutine (showObjectMessageCoroutine);
		}
		interactionMessageGameObject.SetActive (false);
	}

	public string getCurrentDeviceActionText ()
	{
		return currentDeviceActionText;
	}

	public bool hasDeviceToUse ()
	{
		return objectToUse != null;
	}

	public void setUseDeviceButtonEnabledState (bool state)
	{
		useDeviceButtonEnabled = state;
		holdingButton = false;
	}

	public void checkIfPickObjectsAround ()
	{
		if (holdingButton) {
			if (Time.time > lastTimePressedButton + holdButtonTime) {
				//print ("take all");
				takePickupsAround ();
				holdingButton = false;
			} else {
				//print ("only use device");
				useDevice ();
			}
		}

		holdingButton = false;
	}
		
	//CALL INPUT FUNCTIONS
	public void inputActivateDevice ()
	{
		if (useDeviceButtonEnabled) {
			if (!currentDeviceIsPickup || !holdButtonToTakePickupsAround) {
				useDevice ();
			}
		}
	}

	public void inputHoldToPickObjectsAround ()
	{
		if (useDeviceButtonEnabled && currentDeviceIsPickup && holdButtonToTakePickupsAround) {
			holdingButton = true;
			lastTimePressedButton = Time.time;
		}
	}

	public void inputReleaseToPickObjectsAround ()
	{
		if (useDeviceButtonEnabled && currentDeviceIsPickup && holdButtonToTakePickupsAround) {
			checkIfPickObjectsAround ();
		}
	}

	[System.Serializable]
	public class deviceInfo
	{
		public GameObject deviceGameObject;
		public Transform positionToIcon;
		public bool useTransformForStringAction;
		public bool useSeparatedTransformForEveryView;
		public float actionOffset;
		public bool useLocalOffset;
	}
}