using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class inventoryBankManager : MonoBehaviour
{
	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryInfo> playerInventoryList = new List<inventoryInfo> ();
	public List<inventoryInfo> bankInventoryList = new List<inventoryInfo> ();

	public List<inventoryInfo> currentBankInventoryList = new List<inventoryInfo> ();

	public List<inventoryMenuIconElement> playerIconElementList = new List<inventoryMenuIconElement> ();
	public List<inventoryMenuIconElement> bankIconElementList = new List<inventoryMenuIconElement> ();

	public string[] inventoryManagerListString;

	public GameObject inventoryMenu;

	public GameObject playerInventorySlots;
	public GameObject playerInventorySlotsContent;
	public GameObject playerInventorySlot;
	public Scrollbar playerInventoryScrollbar;

	public GameObject bankInventorySlots;
	public GameObject bankInventorySlotsContent;
	public GameObject bankInventorySlot;
	public Scrollbar bankInventoryScrollbar;

	public GameObject numberOfElementsWindow;
	public Text numberOfObjectsToUseText;

	public Camera inventoryCamera;
	public float zoomSpeed;
	public float maxZoomValue;
	public float minZoomValue;
	public float rotationSpeed;

	public RawImage inventoryMenuRenderTexture;

	public Transform lookObjectsPosition;

	public GameObject slotToMove;
	public GameObject menuBackground;
	public GameObject inventoryObjectRenderPanel;

	public bool useBlurUIPanel = true;

	public bool useNumberOfElementsOnPressUp;
	public bool dropFullObjectsAmount = true;
	public bool dragAndDropIcons = true;
	public float timeToDrag = 0.5f;

	public float configureNumberObjectsToUseRate = 0.4f;

	public bool menuOpened;

	public bool loadCurrentBankInventoryFromSaveFile;
	public bool saveCurrentBankInventoryToSaveFile;
	List<inventoryListElement> persistanceInventoryObjectList = new List<inventoryListElement> ();

	bool canDropObjectsToInventoryBank = true;

	inventoryInfo currentInventoryObject;
	Coroutine resetCameraFov;

	menuPause pauseManager;
	float originalFov;
	GameObject objectInCamera;
	int numberOfObjectsToUse;

	playerController playerControllerManager;
	inventoryManager playerInventoryManager;
	GameObject currentPlayer;
	usingDevicesSystem usingDevicesManager;

	public inventoryListManager mainInventoryManager;

	bool touchPlatform;
	bool touching;
	Touch currentTouch;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	bool zoomingIn;
	bool zoomingOut;
	bool slotToMoveFound;
	GameObject slotFound;
	bool draggedFromPlayerInventoryList;
	bool draggedFromBankInventoryList;
	bool droppedInPlayerInventoryList;
	bool droppedInBankInventoryList;
	inventoryInfo currentSlotToMoveInventoryObject;
	inventoryInfo currentSlotFoundInventoryObject;

	bool enableRotation;
	playerInputManager playerInput;
	inventoryBankSystem currentInventoryBankSystem;
	public gameManager gameSystemManager;

	bool useInventoryFromThisBank;

	int objectToMoveIndex;
	int objectFoundIndex;
	//RectTransform numberOfObjectsToUseMenuRectTransform;
	int currentAmountToMove;
	int currentBankInventoryListIndex;
	bool currentObjectAlreadyInBankList;
	bool settingAmountObjectsToMove;
	bool resetDragDropElementsEnabled = true;
	float lastTimeTouched;

	bool addingObjectToUse;
	bool removinvObjectToUse;
	float lastTimeAddObjectToUse;
	float lastTimeRemoveObjectToUse;

	inventoryObject currentInventoryObjectManager;
	float currentMaxZoomValue;
	float currentMinZoomValue;

	Vector2 axisValues;

	void Awake ()
	{
		if (loadCurrentBankInventoryFromSaveFile) {

			if (gameSystemManager.loadEnabled) {

				persistanceInventoryObjectList = gameSystemManager.loadBankInventoryList ();

				if (persistanceInventoryObjectList != null && gameSystemManager.getLastSaveNumber () > -1) {
					inventoryListManagerList.Clear ();
					inventoryListManagerList = persistanceInventoryObjectList;
				}
			}
		}
	}

	void Start ()
	{
		originalFov = inventoryCamera.fieldOfView;
		inventoryMenu.SetActive (false);
		playerInventorySlot.SetActive (false);
		bankInventorySlot.SetActive (false);

		setInventoryFromInventoryListManager ();
		touchPlatform = touchJoystick.checkTouchPlatform ();
		//numberOfObjectsToUseMenuRectTransform = numberOfElementsWindow.GetComponent<RectTransform> ();
	}

	void Update ()
	{
		if (menuOpened) {
			
			if (enableRotation) {
				axisValues = playerInput.getPlayerMovementAxis ("mouse");
				objectInCamera.transform.RotateAroundLocal (inventoryCamera.transform.up, -Mathf.Deg2Rad * rotationSpeed * axisValues.x);
				objectInCamera.transform.RotateAroundLocal (inventoryCamera.transform.right, Mathf.Deg2Rad * rotationSpeed * axisValues.y);
			}

			if (currentInventoryObjectManager && currentInventoryObjectManager.useZoomRange) {
				currentMaxZoomValue = currentInventoryObjectManager.maxZoom;
				currentMinZoomValue = currentInventoryObjectManager.minZoom;	
			} else {
				currentMaxZoomValue = maxZoomValue;
				currentMinZoomValue = minZoomValue;
			}

			if (zoomingIn) {
				if (inventoryCamera.fieldOfView > currentMaxZoomValue) {
					inventoryCamera.fieldOfView -= Time.deltaTime * zoomSpeed;
				} else {
					inventoryCamera.fieldOfView = currentMaxZoomValue;
				}
			}

			if (zoomingOut) {
				if (inventoryCamera.fieldOfView < currentMinZoomValue) {
					inventoryCamera.fieldOfView += Time.deltaTime * zoomSpeed;
				} else {
					inventoryCamera.fieldOfView = currentMinZoomValue;
				}
			}

			if (addingObjectToUse) {
				if (Time.time > configureNumberObjectsToUseRate + lastTimeAddObjectToUse) {
					lastTimeAddObjectToUse = Time.time;
					addObjectToUse ();
				}
			}

			if (removinvObjectToUse) {
				if (Time.time > configureNumberObjectsToUseRate + lastTimeRemoveObjectToUse) {
					lastTimeRemoveObjectToUse = Time.time;
					removeObjectToUse ();
				}
			}

			if (settingAmountObjectsToMove) {
				return;
			}

			//check the mouse position in the screen if we are in the editor, or the finger position in a touch device
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
					lastTimeTouched = Time.time;
					captureRaycastResults.Clear ();
					PointerEventData p = new PointerEventData (EventSystem.current);
					p.position = currentTouch.position;
					p.clickCount = i;
					p.dragging = false;
					EventSystem.current.RaycastAll (p, captureRaycastResults);
					foreach (RaycastResult r in captureRaycastResults) {
						if (!slotToMoveFound) {
							inventoryMenuIconElement currentInventoryMenuIconElement = r.gameObject.GetComponent<inventoryMenuIconElement> ();
							if (currentInventoryMenuIconElement) {
			
								for (int j = 0; j < playerInventoryList.Count; j++) {
									if (playerInventoryList [j].button == currentInventoryMenuIconElement.button) {

										if (!canDropObjectsToInventoryBank) {
											return;
										}

										if (playerInventoryList [j].amount > 0) {
											currentSlotToMoveInventoryObject = playerInventoryList [j];
											draggedFromPlayerInventoryList = true;
											slotToMoveFound = true;
											objectToMoveIndex = j;
											print ("dragged from player inventory list " + j.ToString ());
											getPressedButton (currentInventoryMenuIconElement.button);
										}
									}
								}

								if (!draggedFromPlayerInventoryList) {
									for (int j = 0; j < currentBankInventoryList.Count; j++) {
										if (currentBankInventoryList [j].button == currentInventoryMenuIconElement.button) {
											if (currentBankInventoryList [j].amount > 0) {
												currentSlotToMoveInventoryObject = currentBankInventoryList [j];
												draggedFromBankInventoryList = true;
												slotToMoveFound = true;
												objectToMoveIndex = j;
												print ("dragged from bank inventory list " + j.ToString ());
												getPressedButton (currentInventoryMenuIconElement.button);
											}
										}
									}
								}

								if (slotToMoveFound && dragAndDropIcons) {
									slotToMove.GetComponentInChildren<RawImage> ().texture = currentInventoryMenuIconElement.icon.texture;
								}
							}
						}
					}
				}

				if (touching && !dragAndDropIcons && slotToMoveFound) {
					touching = false;
					checkDroppedObject ();
					return;
				}
					
				if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
					if (slotToMoveFound && dragAndDropIcons) {
						if (touching && Time.time > lastTimeTouched + timeToDrag) {
							if (!slotToMove.activeSelf) {
								slotToMove.SetActive (true);
							}

							slotToMove.GetComponent<RectTransform> ().position = new Vector2 (currentTouch.position.x, currentTouch.position.y);
						}
					}
				}

				//if the mouse/finger press is released, then
				if (currentTouch.phase == TouchPhase.Ended && touching) {
					touching = false;
					if (slotToMoveFound) {
						//get the elements in the position where the player released the power element
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouch.position;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);
						foreach (RaycastResult r in captureRaycastResults) {
							if (r.gameObject != slotToMove) {
								if (r.gameObject.GetComponent<inventoryMenuIconElement> ()) {
									slotFound = r.gameObject;
								} else if (r.gameObject == playerInventorySlots) {
									droppedInPlayerInventoryList = true;
								} else if (r.gameObject == bankInventorySlots) {
									droppedInBankInventoryList = true;
								}
							}
						}
						checkDroppedObject ();
					}
				}
			}
		}
	}

	public void setCurrentInventoryBankSystem (GameObject inventoryBankGameObject)
	{
		currentInventoryBankSystem = inventoryBankGameObject.GetComponent<inventoryBankSystem> ();

		useInventoryFromThisBank = currentInventoryBankSystem.useInventoryFromThisBank;
		if (useInventoryFromThisBank) {
			currentBankInventoryList = currentInventoryBankSystem.getBankInventoryList ();
			canDropObjectsToInventoryBank = false;
		} else {
			currentBankInventoryList = bankInventoryList;
			canDropObjectsToInventoryBank = true;
		}

		menuBackground.SetActive (!useInventoryFromThisBank);
		inventoryObjectRenderPanel.SetActive (!useInventoryFromThisBank);
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		playerControllerManager = currentPlayer.GetComponent<playerController> ();
		playerInventoryManager = currentPlayer.GetComponent<inventoryManager> ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
		playerInput = currentPlayer.GetComponent<playerInputManager> ();
		pauseManager = playerInput.getPauseManager ();
	}

	public void openOrCloseInventoryBankMenuByButton ()
	{
		if (usingDevicesManager) {
			usingDevicesManager.useDevice ();
		}
		//openOrCloseInventoryBankMenu (!menuOpened);
		if (currentInventoryBankSystem) {
			currentInventoryBankSystem.setUsingInventoryBankState (menuOpened);
		}
	}

	public void openOrCloseInventoryBankMenu (bool state)
	{
		menuOpened = state;

		setMenuIconElementPressedState (false);
		pauseManager.openOrClosePlayerMenu (menuOpened, inventoryMenu.transform, useBlurUIPanel);

		pauseManager.setIngameMenuOpenedState ("Inventory Bank Manager", menuOpened);

		inventoryMenu.SetActive (menuOpened);

		bankIconElementList.Clear ();
		playerIconElementList.Clear ();
		if (menuOpened) {
			getCurrentPlayerInventoryList ();

			createInventoryIcons (playerInventoryList, playerInventorySlot, playerInventorySlotsContent, playerIconElementList);
			createInventoryIcons (currentBankInventoryList, bankInventorySlot, bankInventorySlotsContent, bankIconElementList);
			playerInventorySlotsContent.GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 0.5f;
			bankInventorySlotsContent.GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 0.5f;
			bankInventoryScrollbar.value = 1;
			playerInventoryScrollbar.value = 1;
		} else {
			for (int i = 0; i < playerInventoryList.Count; i++) {
				if (playerInventoryList [i].button != null) {
					Destroy (playerInventoryList [i].button.gameObject);
				}
			}

			for (int i = 0; i < currentBankInventoryList.Count; i++) {
				if (currentBankInventoryList [i].button != null) {
					Destroy (currentBankInventoryList [i].button.gameObject);
				}
			}
		}

		//set to visible the cursor
		pauseManager.showOrHideCursor (menuOpened);

		//disable the touch controls
		pauseManager.checkTouchControls (!menuOpened);

		//disable the camera rotation
		pauseManager.changeCameraState (!menuOpened);

		playerControllerManager.changeScriptState (!menuOpened);
	
		pauseManager.usingSubMenuState (menuOpened);

		pauseManager.enableOrDisableDynamicElementsOnScreen (!menuOpened);

		destroyObjectInCamera ();

		resetAndDisableNumberOfObjectsToUseMenu ();
		inventoryCamera.fieldOfView = originalFov;
		currentInventoryObject = null;
		inventoryCamera.enabled = menuOpened;

		currentInventoryObjectManager = null;

		if (!menuOpened) {
			resetDragDropElements ();
		}
	}

	public void checkDroppedObject ()
	{
		//an icon has been dropped on top of another icon from both inventory list
		if (slotFound) {
			bool droppedCorreclty = true;
			bool currentSlotDroppedFoundOnList = false;

			inventoryMenuIconElement slotFoundInventoryMenuIconElement = slotFound.GetComponent<inventoryMenuIconElement> ();

			for (int j = 0; j < playerInventoryList.Count; j++) {
				if (playerInventoryList [j].button == slotFoundInventoryMenuIconElement.button) {
					currentSlotFoundInventoryObject = playerInventoryList [j];
					currentSlotDroppedFoundOnList = true;
					objectFoundIndex = j;
				}
			}

			if (!currentSlotDroppedFoundOnList) {
				for (int j = 0; j < currentBankInventoryList.Count; j++) {
					if (currentBankInventoryList [j].button == slotFoundInventoryMenuIconElement.button) {
						currentSlotFoundInventoryObject = currentBankInventoryList [j];
						currentSlotDroppedFoundOnList = true;
						objectFoundIndex = j;
					}
				}
			}

			if (currentSlotDroppedFoundOnList) {
				if (((draggedFromBankInventoryList && droppedInPlayerInventoryList) || (draggedFromPlayerInventoryList && droppedInBankInventoryList))) {
					print (currentSlotToMoveInventoryObject.Name + " dropped on top of " + currentSlotFoundInventoryObject.Name);

					if (droppedInPlayerInventoryList) {
						print ("dropped correctly to player inventory");

						resetDragDropElementsEnabled = false;
						inventoryInfo auxInventoryInfo = currentSlotToMoveInventoryObject;
						int auxObjectToMoveIndex = objectToMoveIndex;

						//move the object dropped to players's inventory
						currentSlotToMoveInventoryObject = currentSlotFoundInventoryObject;
						objectToMoveIndex = objectFoundIndex;

						if (canDropObjectsToInventoryBank) {
							dragToBankInventory ();
						}
					
						objectToMoveIndex = auxObjectToMoveIndex;
						currentSlotToMoveInventoryObject = auxInventoryInfo;
						print (currentSlotToMoveInventoryObject.Name + " found on player's inventory");
						resetDragDropElementsEnabled = true;
						dragToPlayerInventory ();

					} else if (droppedInBankInventoryList) {
						print ("dropped correctly to bank inventory");

						resetDragDropElementsEnabled = false;
						//move the object dropped to bank inventory
						dragToBankInventory ();

						objectToMoveIndex = objectFoundIndex;
						currentSlotToMoveInventoryObject = currentBankInventoryList [objectToMoveIndex];
						print (currentSlotToMoveInventoryObject.Name + " found on bank");
						resetDragDropElementsEnabled = true;
						dragToPlayerInventory ();
					}
				} else {
					print ("dropped into the same dragged list");
					droppedCorreclty = false;
					if (useNumberOfElementsOnPressUp) {
						enableNumberOfElementsOnPressUp ();
						droppedCorreclty = true;
					}
				}
			} else {
				print ("dropped outside the list");
				droppedCorreclty = false;
			}

			slotToMove.SetActive (false);

			if (!droppedCorreclty) {
				resetDragDropElements ();
			}
		} 

		//the icon is dropped on top or bottom of any of the inventory list
		else {
			bool droppedCorreclty = true;
			//if the object to move has been dragged and dropped from one list to another correctly, then
			if (((draggedFromBankInventoryList && droppedInPlayerInventoryList) || (draggedFromPlayerInventoryList && droppedInBankInventoryList))) {
				//dropping object from bank to player's inventory
				if (droppedInPlayerInventoryList) {
					dragToPlayerInventory ();
				}
				//dropping object from player's inventory to bank
				else if (droppedInBankInventoryList) {
					dragToBankInventory ();
				} else {
					print ("not dropped correctly");
					droppedCorreclty = false;
				}
			} else {
				if (dragAndDropIcons) {
					print ("dropped into the same dragged list or outside of the list");
					droppedCorreclty = false;
					if (useNumberOfElementsOnPressUp) {
						enableNumberOfElementsOnPressUp ();
						droppedCorreclty = true;
					}
				} else {
					//if the drag and drop function is not being used, check if the pressed icon is in player or bank inventory to move the object
					if (draggedFromBankInventoryList) {
						dragToPlayerInventory ();
					} else if (draggedFromPlayerInventoryList) {
						dragToBankInventory ();
					}
				}
			}

			slotToMove.SetActive (false);

			if (!droppedCorreclty) {
				resetDragDropElements ();
			}
		}
	}

	public void enableNumberOfElementsOnPressUp ()
	{
		if (dragAndDropIcons && !dropFullObjectsAmount) {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());
			if (draggedFromPlayerInventoryList) {
				droppedInBankInventoryList = true;
				droppedInPlayerInventoryList = false;
			}
			if (draggedFromBankInventoryList) {
				droppedInPlayerInventoryList = true;
				droppedInBankInventoryList = false;
			}
			draggedFromPlayerInventoryList = true;
			draggedFromBankInventoryList = true;
		}
	}

	public void dragToPlayerInventory ()
	{
		print ("dropped correctly to player inventory");

		print (currentSlotToMoveInventoryObject.Name + " dropped correctly to player inventory");

		currentAmountToMove = currentSlotToMoveInventoryObject.amount;

		if (dropFullObjectsAmount) {
			moveObjectsToPlayerInventory ();
		} else {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());
		}
	}

	public void dragToBankInventory ()
	{
		print ("dropped correctly to bank inventory");
		currentObjectAlreadyInBankList = false;
		currentBankInventoryListIndex = -1;
		for (int j = 0; j < currentBankInventoryList.Count; j++) {
			if (!currentObjectAlreadyInBankList && currentBankInventoryList [j].Name == currentSlotToMoveInventoryObject.Name) {
				currentObjectAlreadyInBankList = true;
				currentBankInventoryListIndex = j;
			}
		}

		currentAmountToMove = currentSlotToMoveInventoryObject.amount;

		if (dropFullObjectsAmount) {
			moveObjectsToBankInventory ();
		} else {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());
		}
	}

	public void resetDragDropElements ()
	{
		if (!resetDragDropElementsEnabled) {
			return;
		}

		slotFound = null;
		draggedFromPlayerInventoryList = false;
		draggedFromBankInventoryList = false;
		droppedInBankInventoryList = false;
		droppedInPlayerInventoryList = false;
		slotToMoveFound = false;
		currentSlotFoundInventoryObject = null;
		currentSlotToMoveInventoryObject = null;
		settingAmountObjectsToMove = false;
		resetAndDisableNumberOfObjectsToUseMenu ();
	}

	public void confirmMoveObjects ()
	{
		if (draggedFromPlayerInventoryList) {

			if ((dragAndDropIcons && droppedInBankInventoryList) || !dragAndDropIcons) {
				moveObjectsToBankInventory ();
				return;
			}
		}

		if (draggedFromBankInventoryList) {

			if ((dragAndDropIcons && droppedInPlayerInventoryList) || !dragAndDropIcons) {
				moveObjectsToPlayerInventory ();
				return;
			}
		}
	}

	public void confirmMoveAllObjects ()
	{
		currentAmountToMove = currentInventoryObject.amount;
		confirmMoveObjects ();
	}

	public void cancelMoveObjects ()
	{
		resetDragDropElements ();
		resetAndDisableNumberOfObjectsToUseMenu ();
	}

	public void moveObjectsToBankInventory ()
	{
		if (currentObjectAlreadyInBankList) {
			currentBankInventoryList [currentBankInventoryListIndex].amount += currentAmountToMove;

			print ("combine objects amount + " + currentAmountToMove);
		} else {
			currentSlotToMoveInventoryObject.amount = currentAmountToMove;
			currentBankInventoryList.Add (new inventoryInfo (currentSlotToMoveInventoryObject));
			int lastAddedObjectIndex = currentBankInventoryList.Count;

			createInventoryIcon (currentBankInventoryList [lastAddedObjectIndex - 1], lastAddedObjectIndex - 1, bankInventorySlot, 
				bankInventorySlotsContent, bankIconElementList);

			print ("new object added +" + currentAmountToMove);
		}

		playerInventoryManager.moveObjectToBank (objectToMoveIndex, currentAmountToMove);

		getCurrentPlayerInventoryList ();

		updateFullInventorySlots (playerInventoryList, playerIconElementList);
		updateFullInventorySlots (currentBankInventoryList, bankIconElementList);

		resetDragDropElements ();
	}

	public void moveObjectsToPlayerInventory ()
	{
		int currentSlotToMoveAmount = currentSlotToMoveInventoryObject.amount;
		currentSlotToMoveInventoryObject.amount = currentAmountToMove;
		if (!playerInventoryManager.isInventoryFull ()) {
			int inventoryAmountPicked = playerInventoryManager.tryToPickUpObject (currentSlotToMoveInventoryObject);
			print ("amount taken " + inventoryAmountPicked);
			if (inventoryAmountPicked > 0) {
				currentSlotToMoveInventoryObject.amount = currentSlotToMoveAmount - inventoryAmountPicked;
			} else {
				playerInventoryManager.showInventoryFullMessage ();
			}
		} else {
			playerInventoryManager.showInventoryFullMessage ();
		}
			
		getCurrentPlayerInventoryList ();

		updateFullInventorySlots (playerInventoryList, playerIconElementList);
		updateFullInventorySlots (currentBankInventoryList, bankIconElementList);

		removeButton (currentBankInventoryList, bankIconElementList);

		LayoutRebuilder.ForceRebuildLayoutImmediate (bankInventorySlotsContent.GetComponent<RectTransform> ());

		resetDragDropElements ();
	}

	public void updateFullInventorySlots (List<inventoryInfo> currentInventoryList, List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryList.Count; i++) {
			inventoryInfo currentInventoryInfo = currentInventoryList [i];
			currentInventoryInfo.menuIconElement = currentIconElementList [i];
			currentInventoryInfo.button = currentIconElementList [i].button;
			currentInventoryInfo.menuIconElement.iconName.text = currentInventoryInfo.Name;
			currentInventoryInfo.menuIconElement.icon.texture = currentInventoryInfo.icon;
			currentInventoryInfo.menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();
		}
	}

	public void createInventoryIcons (List<inventoryInfo> currentInventoryListToCreate, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                  List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			if (currentInventoryListToCreate [i].button != null) {
				Destroy (currentInventoryListToCreate [i].button.gameObject);
			}
		}
		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			createInventoryIcon (currentInventoryListToCreate [i], i, currentInventorySlot, currentInventorySlotsContent, currentIconElementList);
		}
	}

	public void createInventoryIcon (inventoryInfo currentInventoryInfo, int index, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                 List<inventoryMenuIconElement> currentIconElementList)
	{
		GameObject newIconButton = (GameObject)Instantiate (currentInventorySlot, Vector3.zero, Quaternion.identity);
		newIconButton.SetActive (true);
		newIconButton.transform.SetParent (currentInventorySlotsContent.transform);
		newIconButton.transform.localScale = Vector3.one;
		newIconButton.transform.localPosition = Vector3.zero;
		inventoryMenuIconElement menuIconElement = newIconButton.GetComponent<inventoryMenuIconElement> ();
		menuIconElement.iconName.text = currentInventoryInfo.Name;
		if (currentInventoryInfo.inventoryGameObject != null) {
			menuIconElement.icon.texture = currentInventoryInfo.icon;
		} else {
			menuIconElement.icon.texture = null;
		}
		menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();
		menuIconElement.pressedIcon.SetActive (false);
		newIconButton.name = "Inventory Object-" + (index + 1).ToString ();
		Button button = menuIconElement.button;
		currentInventoryInfo.button = button;
		currentInventoryInfo.menuIconElement = menuIconElement;
		currentIconElementList.Add (menuIconElement);
	}

	public void getPressedButton (Button buttonObj)
	{
		int inventoryObjectIndex = -1;
		if (draggedFromPlayerInventoryList) {
			for (int i = 0; i < playerInventoryList.Count; i++) {
				if (playerInventoryList [i].button == buttonObj) {
					inventoryObjectIndex = i;
				}
			}
		} else {
			for (int i = 0; i < currentBankInventoryList.Count; i++) {
				if (currentBankInventoryList [i].button == buttonObj) {
					inventoryObjectIndex = i;
				}
			}
		}
			
		if (currentInventoryObject != null) {
			if (((draggedFromPlayerInventoryList && currentInventoryObject == playerInventoryList [inventoryObjectIndex]) ||
				(!draggedFromPlayerInventoryList && currentInventoryObject == currentBankInventoryList [inventoryObjectIndex])) &&
			    currentInventoryObject.menuIconElement.pressedIcon.activeSelf) {
				return;
			}
		}

		setObjectInfo (inventoryObjectIndex, draggedFromPlayerInventoryList);

		float currentCameraFov = originalFov;
		if (currentInventoryObjectManager) {
			if (currentInventoryObjectManager.useZoomRange) {
				currentCameraFov = currentInventoryObjectManager.initialZoom;
			}
		}
		if (inventoryCamera.fieldOfView != currentCameraFov) {
			checkResetCameraFov (currentCameraFov);
		}
	}

	public void setObjectInfo (int index, bool isPlayerInventoryList)
	{
		resetAndDisableNumberOfObjectsToUseMenu ();
		setMenuIconElementPressedState (false);
	
		if (isPlayerInventoryList) {
			currentInventoryObject = playerInventoryList [index];
		} else {
			currentInventoryObject = currentBankInventoryList [index];
		}
		setMenuIconElementPressedState (true);

		GameObject currentInventoryObjectPrefab = currentInventoryObject.inventoryGameObject;

		for (int i = 0; i < mainInventoryManager.inventoryList.Count; i++) {
			if (mainInventoryManager.inventoryList [i].inventoryGameObject == currentInventoryObject.inventoryGameObject) {
				currentInventoryObjectPrefab = mainInventoryManager.inventoryList [i].inventoryObjectPrefab;
			}
		}

		currentInventoryObjectManager = currentInventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();

		inventoryMenuRenderTexture.enabled = true;
		destroyObjectInCamera ();
		if (currentInventoryObject.inventoryGameObject) {
	
			objectInCamera = (GameObject)Instantiate (currentInventoryObject.inventoryGameObject, lookObjectsPosition.position, Quaternion.identity);
			objectInCamera.transform.SetParent (lookObjectsPosition);
		}
	}

	public void setMenuIconElementPressedState (bool state)
	{
		if (currentInventoryObject != null) {
			if (currentInventoryObject.menuIconElement != null) {
				currentInventoryObject.menuIconElement.pressedIcon.SetActive (state);
			}
		}
	}

	public void enableObjectRotation ()
	{
		if (objectInCamera) {
			enableRotation = true;
		}
	}

	public void disableObjectRotation ()
	{
		enableRotation = false;
	}

	public void destroyObjectInCamera ()
	{
		if (objectInCamera) {
			Destroy (objectInCamera);
		}
	}

	public void zoomInEnabled ()
	{
		zoomingIn = true;
	}

	public void zoomInDisabled ()
	{
		zoomingIn = false;
	}

	public void zoomOutEnabled ()
	{
		zoomingOut = true;
	}

	public void zoomOutDisabled ()
	{
		zoomingOut = false;
	}

	public void checkResetCameraFov (float targetValue)
	{
		if (resetCameraFov != null) {
			StopCoroutine (resetCameraFov);
		}
		resetCameraFov = StartCoroutine (resetCameraFovCorutine (targetValue));
	}

	IEnumerator resetCameraFovCorutine (float targetValue)
	{
		while (inventoryCamera.fieldOfView != targetValue) {
			inventoryCamera.fieldOfView = Mathf.MoveTowards (inventoryCamera.fieldOfView, targetValue, Time.deltaTime * zoomSpeed);
			yield return null;
		}
	}

	public void setInventoryFromInventoryListManager ()
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			inventoryInfo currentInventoryInfo = mainInventoryManager.inventoryList [inventoryListManagerList [i].elementIndex];
			if (currentInventoryInfo != null) {
				inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
				newInventoryInfo.Name = currentInventoryInfo.Name;
				newInventoryInfo.amount = inventoryListManagerList [i].amount;
				bankInventoryList.Add (newInventoryInfo);
			}
		}
		updateComponent ();
	}

	public void getInventoryListManagerList ()
	{
		inventoryManagerListString = new string[mainInventoryManager.inventoryList.Count];
		for (int i = 0; i < inventoryManagerListString.Length; i++) {
			inventoryManagerListString [i] = mainInventoryManager.inventoryList [i].Name;
		}

		updateComponent ();
	}

	public void addNewInventoryObjectToInventoryListManagerList ()
	{
		inventoryListElement newInventoryListElement = new inventoryListElement ();
		newInventoryListElement.name = "New Object";
		inventoryListManagerList.Add (newInventoryListElement);
		updateComponent ();
	}

	public void removeButton (List<inventoryInfo> currentInventoryList, List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryList.Count; i++) {
			if (currentInventoryList [i].amount == 0) {
				Destroy (currentInventoryList [i].button.gameObject);
				currentIconElementList.Remove (currentInventoryList [i].menuIconElement);
				print ("index to remove " + i);
				currentInventoryList.RemoveAt (i);
				i = 0;
			}
		}
		enableRotation = false;
		destroyObjectInCamera ();
		setMenuIconElementPressedState (false);
		currentInventoryObject = null;

		currentInventoryObjectManager = null;
	}

	public void getCurrentPlayerInventoryList ()
	{
		playerInventoryList.Clear ();
		for (int i = 0; i < playerInventoryManager.inventoryList.Count; i++) {
			inventoryInfo newPlayerInventoryObjectInfo = new inventoryInfo (playerInventoryManager.inventoryList [i]);
			newPlayerInventoryObjectInfo.button = null;
			playerInventoryList.Add (newPlayerInventoryObjectInfo);
		}
	}

	public void addObjectToUse ()
	{
		numberOfObjectsToUse++;
		if (numberOfObjectsToUse > currentInventoryObject.amount) {
			numberOfObjectsToUse = currentInventoryObject.amount;
		}
		currentAmountToMove = numberOfObjectsToUse;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void removeObjectToUse ()
	{
		numberOfObjectsToUse--;
		if (numberOfObjectsToUse < 1) {
			numberOfObjectsToUse = 1;
		}
		currentAmountToMove = numberOfObjectsToUse;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void startToAddObjectToUse ()
	{
		addingObjectToUse = true;
	}

	public void stopToAddObjectToUse ()
	{
		addingObjectToUse = false;
	}

	public void startToRemoveObjectToUse ()
	{
		removinvObjectToUse = true;
	}

	public void stopToRemoveObjectToUse ()
	{
		removinvObjectToUse = false;
	}

	public void enableNumberOfObjectsToUseMenu (RectTransform menuPosition)
	{
		numberOfElementsWindow.SetActive (true);
		//numberOfObjectsToUseMenuRectTransform.anchoredPosition = menuPosition.anchoredPosition;
		numberOfObjectsToUse = 1;
		currentAmountToMove = numberOfObjectsToUse;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
		settingAmountObjectsToMove = true;
	}

	public void resetAndDisableNumberOfObjectsToUseMenu ()
	{
		disableNumberOfObjectsToUseMenu ();
		resetNumberOfObjectsToUse ();
	}

	public void disableNumberOfObjectsToUseMenu ()
	{
		numberOfObjectsToUse = 1;
		numberOfElementsWindow.SetActive (false);
	}

	public void resetNumberOfObjectsToUse ()
	{
		numberOfObjectsToUse = 1;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void setNumberOfObjectsToUseText (int amount)
	{
		numberOfObjectsToUseText.text = amount.ToString ();
	}

	public persistanceInventoryListByPlayerInfo getPersistanceInventoryList ()
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();

		newPersistanceInventoryListByPlayerInfo.playerID = playerControllerManager.getPlayerID ();

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		for (int k = 0; k < bankInventoryList.Count; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();
			int inventoryListIndex = mainInventoryManager.getInventoryIndexFromInventoryGameObject (bankInventoryList [k].inventoryGameObject);
			if (inventoryListIndex > -1) {
				newPersistanceInventoryObjectInfo.name = bankInventoryList [k].Name;
				newPersistanceInventoryObjectInfo.amount = bankInventoryList [k].amount;
				newPersistanceInventoryObjectInfo.infiniteAmount = bankInventoryList [k].infiniteAmount;
				newPersistanceInventoryObjectInfo.inventoryObjectName = mainInventoryManager.inventoryList [k].Name;
				newPersistanceInventoryObjectInfo.elementIndex = inventoryListIndex;

				//print (newPersistanceInventoryObjectInfo.name + " " + newPersistanceInventoryObjectInfo.amount);
				newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
			}
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}

	public void saveCurrentInventoryListToFile ()
	{

		if (gameSystemManager.saveNumberToLoad > 0) {
			if (playerControllerManager == null) {
				playerControllerManager = GetComponentInChildren<playerController> ();
			}
			gameSystemManager.saveBankInventoryList (getPersistanceInventoryListElement (), gameSystemManager.saveNumberToLoad);
		} else {
			print ("Configure a valid save number in the Game Manager inspector, between 1 and infinite");
			return;
		}

		print ("Inventory bank list saved");
		updateComponent ();
	}

	public persistanceInventoryListByPlayerInfo getPersistanceInventoryListElement ()
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();

		newPersistanceInventoryListByPlayerInfo.playerID = playerControllerManager.getPlayerID ();

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		for (int k = 0; k < inventoryListManagerList.Count; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();
			newPersistanceInventoryObjectInfo.name = inventoryListManagerList [k].name;
			newPersistanceInventoryObjectInfo.amount = inventoryListManagerList [k].amount;
			newPersistanceInventoryObjectInfo.infiniteAmount = inventoryListManagerList [k].infiniteAmount;
			newPersistanceInventoryObjectInfo.inventoryObjectName = inventoryListManagerList [k].inventoryObjectName;
			newPersistanceInventoryObjectInfo.elementIndex = inventoryListManagerList [k].elementIndex;

			print (newPersistanceInventoryObjectInfo.name + " " + newPersistanceInventoryObjectInfo.amount);
			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<inventoryBankManager> ());
		#endif
	}
}