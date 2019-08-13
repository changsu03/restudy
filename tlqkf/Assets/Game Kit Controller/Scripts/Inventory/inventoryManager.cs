using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using System.Reflection;

using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class inventoryManager : MonoBehaviour
{
	public bool inventoryEnabled;
	public List<inventoryInfo> inventoryList = new List<inventoryInfo> ();
	public GameObject inventoryPanel;
	public GameObject inventoryListContent;
	public GameObject objectIcon;

	public GameObject equipButton;
	public GameObject unequipButton;

	public GameObject examineObjectPanel;

	public Text examineObjectName;
	public Text examineObjectDescription;
	public GameObject takeObjectInExaminePanelButton;

	public bool examiningObject;

	public Text currentObjectName;
	public Text currentObjectInfo;
	public RawImage objectImage;
	public Color buttonUsable;
	public Color buttonNotUsable;
	public bool infiniteSlots;
	public int inventorySlotAmount;
	public bool infiniteAmountPerSlot;
	public int amountPerSlot;
	public Camera inventoryCamera;
	public Transform lookObjectsPosition;
	public float rotationSpeed;
	public bool inventoryOpened;
	public GameObject emptyInventoryPrefab;

	public GameObject usedObjectMessage;
	public float usedObjectMessageTime;
	[TextArea (1, 10)]
	public string unableToUseObjectMessage;
	[TextArea (1, 10)]
	public string nonNeededAmountAvaliable;
	[TextArea (1, 10)]
	public string objectNotFoundMessage;
	[TextArea (1, 10)]
	public string cantUseThisObjectHereMessage;

	public GameObject fullInventoryMessage;
	public float fullInventoryMessageTime = 2;

	public GameObject combinedObjectMessage;
	public float combineObjectMessageTime;
	[TextArea (1, 10)]
	public string unableToCombineMessage;
	[TextArea (1, 10)]
	public string notEnoughSpaceToCombineMessage;

	public Scrollbar inventorySlotsScrollbar;
	public Scrollbar inventoryObjectInforScrollbar;

	public bool combineElementsAtDrop;
	public float zoomSpeed;
	public float maxZoomValue;
	public float minZoomValue;
	public bool showElementSettings;
	public inventoryInfo currentInventoryObject;

	public inventoryInfo firstObjectToCombine;
	public inventoryInfo secondObjectToCombine;
	public bool usedByAI;

	public inventoryBankManager bankManager;

	public gameManager gameSystemManager;
	public playerWeaponsManager weaponsManager;
	public playerController playerControllerManager;
	public menuPause pauseManager;
	public gameManager mainGameManager;
	public playerInputManager playerInput;
	public inventoryListManager mainInventoryListManager;
	public usingDevicesSystem usingDevicesManager;

	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryMenuIconElement> menuIconElementList = new List<inventoryMenuIconElement> ();
	public string[] inventoryManagerListString;

	public bool loadCurrentPlayerInventoryFromSaveFile;
	public bool saveCurrentPlayerInventoryToSaveFile;
	List<inventoryListElement> persistanceInventoryObjectList = new List<inventoryListElement> ();

	public float configureNumberObjectsToUseRate = 0.4f;

	public List<secondaryWeaponSlotInfo> secondaryWeaponSlotInfoList = new List<secondaryWeaponSlotInfo> ();

	public bool storePickedWeaponsOnInventory;
	public bool useDragDropWeaponSlots;
	public int numberWeaponsSlots;
	public GameObject inventoryWeaponsSlots;
	public float timeToDrag = 0.5f;
	public GameObject weaponSlotToMove;
	public GameObject weaponSlotPrefab;

	public bool equipWeaponsWhenPicked;

	public bool useBlurUIPanel = true;

	public bool examineObjectBeforeStoreEnabled;

	inventoryInfo duplicateObject;
	GameObject objectInCamera;
	int objectsAmount;
	int bucle = 0;
	bool enableRotation;
	bool zoomingIn;
	bool zoomingOut;
	float originalFov;
	GameObject currentUseInventoryGameObject;
	Coroutine resetCameraFov;
	Coroutine inventoryFullCoroutine;

	inventoryInfo currentPickUpObjectInfo;
	int inventoryAmountNotTaken = 0;

	public Image useButtonImage;
	public Image equipButtonImage;
	public Image unequipButtonImage;
	public Image dropButtonImage;
	public Image combineButtonImage;
	public Image examineButtonImage;
	public Image discardButtonImage;
	public Image dropAllUnitsObjectButtonImage;

	bool combiningObjects;

	bool addingObjectToUse;
	bool removinvObjectToUse;
	float lastTimeAddObjectToUse;
	float lastTimeRemoveObjectToUse;

	inventoryObject currentInventoryObjectManager;
	float currentMaxZoomValue;
	float currentMinZoomValue;

	bool touchPlatform;
	bool touching;
	Touch currentTouch;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();
	float lastTimeTouched;
	bool weaponSlotToMoveFound;
	GameObject weaponSlotFound;
	inventoryInfo currentWeaponSlotToMoveInventoryObject;
	secondaryWeaponSlotInfo currentWeaponSlotToUnequip;
	bool draggedFromInventoryList;
	bool draggedFromWeaponSlots;
	bool initiatingInventory;
	Vector2 axisValues;

	IKWeaponSystem currentIKWeaponSystem;

	float maxRadiusToInstantiate = 1;

	public bool setTotalAmountWhenDropObject;

	int numberOfObjectsToUse = 1;

	public bool useOnlyWhenNeededAmountToUseObject;
	public bool activeNumberOfObjectsToUseMenu;

	public GameObject numberOfObjectsToUseMenu;
	public RectTransform numberOfObjectsToUseMenuRectTransform;
	public RectTransform numberOfObjectsToUseMenuPosition;
	public RectTransform numberOfObjectsToDropMenuPosition;

	public Text numberOfObjectsToUseText;

	useInventoryObject currentUseInventoryObject;

	Coroutine objectMessageCoroutine;

	GameObject previousMessagePanel;
	pickUpObject currentPickupObject;

	public float distanceToPlaceObjectInCamera = 10;
	public float placeObjectInCameraSpeed = 10;
	public int numberOfRotationsObjectInCamera = 3;
	public float placeObjectInCameraRotationSpeed = 0.02f;
	public float extraCameraFovOnExamineObjects = 20;

	Coroutine objectInCameraPositionCoroutine;
	Coroutine objectInCameraRotationCoroutine;

	bool activatingDualWeaponSlot;
	string currentRighWeaponName;
	string currentLeftWeaponName;

	public bool showWeaponSlotsWhenChangingWepaons = true;
	public Transform weaponSlotsParentOnInventory;
	public Transform weaponSlotsParentOutOfInventory;
	public float showWepaonSlotsParentDuration = 1;
	public float weaponSlotsParentScale = 0.7f;
	public GameObject slotWeaponSelectedIcon;

	public bool showWeaponSlotsAlways;

	public bool setWeaponSlotsAlphaValueOutOfInventory;
	public float weaponSlotsAlphaValueOutOfInventory;

	Coroutine weaponSlotsParentCouroutine;

	void Awake ()
	{
		if (!inventoryEnabled) {
			return;
		}

		initiatingInventory = true;
		if (loadCurrentPlayerInventoryFromSaveFile) {
			
			if (gameSystemManager.loadEnabled) {

				persistanceInventoryObjectList = gameSystemManager.loadPlayerInventoryList (playerControllerManager.getPlayerID (), this);

				if (persistanceInventoryObjectList != null && gameSystemManager.getLastSaveNumber () > -1) {
					inventoryListManagerList.Clear ();
					inventoryListManagerList = persistanceInventoryObjectList;
				}
			}
		}
	}

	void Start ()
	{
		if (!inventoryEnabled) {
			return;
		}

		if (usedByAI) {
			return;
		}

		objectIcon.SetActive (false);

		setInventoryFromInventoryListManager ();

		setInventory (true);
		inventoryPanel.SetActive (false);

		disableCurrentObjectInfo ();
		originalFov = inventoryCamera.fieldOfView;

		inventoryCamera.enabled = false;

		if (storePickedWeaponsOnInventory) {
			for (int i = 0; i < numberWeaponsSlots; i++) {
				GameObject newWeaponSlot = (GameObject)Instantiate (weaponSlotPrefab, Vector3.zero, Quaternion.identity);
				newWeaponSlot.name = "Weapon Slot " + (i + 1).ToString ();
				newWeaponSlot.transform.SetParent (weaponSlotPrefab.transform.parent);
				newWeaponSlot.transform.localScale = Vector3.one;
				newWeaponSlot.transform.localPosition = Vector3.zero;
				secondaryWeaponSlotInfo currentsecondaryWeaponSlotInfo = newWeaponSlot.GetComponent<inventoryWeaponSlotElement> ().secondarySlotInfo;

				currentsecondaryWeaponSlotInfo.Name = "";
				currentsecondaryWeaponSlotInfo.slotActive = false;
				currentsecondaryWeaponSlotInfo.weaponSystem = null;
				currentsecondaryWeaponSlotInfo.weaponAmmoText.text = "";
				currentsecondaryWeaponSlotInfo.weaponIcon.texture = null;

				currentsecondaryWeaponSlotInfo.slotMainSingleContent.SetActive (false);

				int index = i;
				if (index == 9) {
					index = -1;
				}
				currentsecondaryWeaponSlotInfo.iconNumberKeyText.text = "[" + (index + 1).ToString () + "]";
				secondaryWeaponSlotInfoList.Add (currentsecondaryWeaponSlotInfo);
			}
			touchPlatform = touchJoystick.checkTouchPlatform ();
			weaponSlotPrefab.SetActive (false);

			for (int i = 0; i < inventoryList.Count; i++) {
				if (inventoryList [i].canBeEquiped && inventoryList [i].isEquiped) {
					if (inventoryList [i].isWeapon) {
						currentInventoryObject = inventoryList [i];
						equipCurrentObject ();
					}
				}
			}
		} else {
			inventoryWeaponsSlots.SetActive (false);
		}
			
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].isWeapon) {
				IKWeaponSystem newIKWeaponSystem = weaponsManager.getIKWeaponSystem (inventoryList [i].Name);
				inventoryList [i].IKWeaponManager = newIKWeaponSystem;
			}
		}

		initiatingInventory = false;
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}

		if (inventoryEnabled) {

			if (inventoryOpened) {
				if (enableRotation) {
					axisValues = playerInput.getPlayerMovementAxis ("mouse");
				} else if (examiningObject) {
					axisValues = playerInput.getPlayerMovementAxis ("keys");
				}

				if (enableRotation || examiningObject) {
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

				if (useDragDropWeaponSlots && !examiningObject) {
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

						if (currentTouch.phase == TouchPhase.Began && !touching) {
							touching = true;

							lastTimeTouched = Time.time;
							captureRaycastResults.Clear ();
							PointerEventData p = new PointerEventData (EventSystem.current);
							p.position = currentTouch.position;
							p.clickCount = i;
							p.dragging = false;
							EventSystem.current.RaycastAll (p, captureRaycastResults);

							foreach (RaycastResult r in captureRaycastResults) {
								if (!weaponSlotToMoveFound) {
									inventoryMenuIconElement currentInventoryMenuIconElement = r.gameObject.GetComponent<inventoryMenuIconElement> ();
									inventoryWeaponSlotElement currentInventoryWeaponSlotElement = r.gameObject.GetComponent<inventoryWeaponSlotElement> ();

									if (currentInventoryMenuIconElement) {
										for (int j = 0; j < inventoryList.Count; j++) {
											if (inventoryList [j].button == currentInventoryMenuIconElement.button) {
												if (inventoryList [j].canBeEquiped) {
													currentWeaponSlotToMoveInventoryObject = inventoryList [j];
													weaponSlotToMoveFound = true;
													draggedFromInventoryList = true;
												}
											}
										}
									} else {
										if (currentInventoryWeaponSlotElement) {
											for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
												if (secondaryWeaponSlotInfoList [j].slot == currentInventoryWeaponSlotElement.secondarySlotInfo.slot) {
													if (secondaryWeaponSlotInfoList [j].slotActive) {
														currentWeaponSlotToUnequip = secondaryWeaponSlotInfoList [j];
														weaponSlotToMoveFound = true;
														draggedFromWeaponSlots = true;
													}
												}
											}
										}
									}

									if (weaponSlotToMoveFound) {
										if (draggedFromInventoryList) {
											weaponSlotToMove.GetComponentInChildren<RawImage> ().texture = currentInventoryMenuIconElement.icon.texture;
										}

										if (draggedFromWeaponSlots) {
											if (currentInventoryWeaponSlotElement.secondarySlotInfo.dualWeaponSlotActive) {
												weaponSlotToMove.GetComponentInChildren<RawImage> ().texture = currentInventoryWeaponSlotElement.secondarySlotInfo.dualLeftWeaponIcon.texture;
											} else {
												weaponSlotToMove.GetComponentInChildren<RawImage> ().texture = currentInventoryWeaponSlotElement.secondarySlotInfo.weaponIcon.texture;
											}
										}
									}
								}
							}
						}

						if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) && touching) {
							if (weaponSlotToMoveFound) {
								if (touching && Time.time > lastTimeTouched + timeToDrag) {
									if (!weaponSlotToMove.activeSelf) {
										weaponSlotToMove.SetActive (true);
									}

									weaponSlotToMove.GetComponent<RectTransform> ().position = new Vector2 (currentTouch.position.x, currentTouch.position.y);
								}
							}
						}

						//if the mouse/finger press is released, then
						if (currentTouch.phase == TouchPhase.Ended && touching) {
							touching = false;

							if (weaponSlotToMoveFound) {
								//get the elements in the position where the player released the power element
								captureRaycastResults.Clear ();
								PointerEventData p = new PointerEventData (EventSystem.current);
								p.position = currentTouch.position;
								p.clickCount = i;
								p.dragging = false;
								EventSystem.current.RaycastAll (p, captureRaycastResults);

								foreach (RaycastResult r in captureRaycastResults) {
									if (r.gameObject != weaponSlotToMove) {
										if (r.gameObject.GetComponent<inventoryWeaponSlotElement> ()) {
											weaponSlotFound = r.gameObject;
										} else if (r.gameObject.GetComponent<inventoryMenuIconElement> ()) {
											weaponSlotFound = r.gameObject;
										}
									}
								}

								checkDroppedWeaponSlot ();
							}
						}
					}
				}
			} 
		}
	}

	public void checkDroppedWeaponSlot ()
	{
		if (weaponSlotFound && (currentWeaponSlotToUnequip == null || !currentWeaponSlotToUnequip.dualWeaponSlotActive)) {
			bool currentSlotDroppedFoundOnList = false;
			int weaponSlotFoundIndex = -1;

			activatingDualWeaponSlot = false;

			if (draggedFromInventoryList || draggedFromWeaponSlots) {
				if (draggedFromInventoryList) {
					print ("dragged from Inventory List to equip weapon");
				}

				if (draggedFromWeaponSlots) {
					print ("dragged from weapon slot to unequip or change weapon");
				}

				inventoryWeaponSlotElement slotFoundInventoryWeaponSlotElement = weaponSlotFound.GetComponent<inventoryWeaponSlotElement> ();

				if (slotFoundInventoryWeaponSlotElement) {

					if (draggedFromInventoryList) {
						if (slotFoundInventoryWeaponSlotElement.secondarySlotInfo.dualWeaponSlotActive) {
							print ("The weapon slot where you are trying to combine the weapon " + currentWeaponSlotToMoveInventoryObject.IKWeaponManager.getWeaponSystemName () +
							" is already active as dual weapon slot");

							resetDragAndDropWeaponSlotState ();

							return;
						} else {

							string weaponNameToEquip = currentWeaponSlotToMoveInventoryObject.IKWeaponManager.getWeaponSystemName ();
							for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
								if (secondaryWeaponSlotInfoList [j].dualWeaponSlotActive) {
									if (secondaryWeaponSlotInfoList [j].rightWeaponName == weaponNameToEquip || secondaryWeaponSlotInfoList [j].leftWeaponName == weaponNameToEquip) {
										print ("The weapon slot where you are trying to combine the weapon " +
										currentWeaponSlotToMoveInventoryObject.IKWeaponManager.getWeaponSystemName () +
										" is already active as dual weapon slot in another slot");

										resetDragAndDropWeaponSlotState ();

										return;
									}
								}
							}
						}
					}

					for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
						if (secondaryWeaponSlotInfoList [j].slot == slotFoundInventoryWeaponSlotElement.secondarySlotInfo.slot) {
							currentSlotDroppedFoundOnList = true;
							weaponSlotFoundIndex = j;
						}
					}
						
					//check that the current weapon slot added is not already present in the list, in that case, reset that slot to update with the new one
					if (draggedFromInventoryList) {
						for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
							if (secondaryWeaponSlotInfoList [i].slotActive && secondaryWeaponSlotInfoList [i].Name == currentWeaponSlotToMoveInventoryObject.Name) {
								updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [i], null);
							}
						}
					}

					if (draggedFromWeaponSlots) {
						if (currentSlotDroppedFoundOnList) {
							if (currentWeaponSlotToUnequip == slotFoundInventoryWeaponSlotElement.secondarySlotInfo) {
								print ("moving weapon slot to the same place, nothing to do");
							} else {
								print ("weaponn slot " + currentWeaponSlotToUnequip.weaponSystem.getWeaponSystemName () + " changed to " +
								"replace " + slotFoundInventoryWeaponSlotElement.secondarySlotInfo.Name);

								weaponsManager.unequipWeapon (slotFoundInventoryWeaponSlotElement.secondarySlotInfo.Name, false);

								weaponsManager.selectWeaponByName (currentWeaponSlotToUnequip.weaponSystem.getWeaponSystemName (), true);

								updateWeaponSlotInfo (weaponSlotFoundIndex, currentWeaponSlotToUnequip.weaponInventoryInfo, null, null);
							}
						}

						for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
							if (weaponSlotFoundIndex != i && secondaryWeaponSlotInfoList [i].slotActive && secondaryWeaponSlotInfoList [i].Name == currentWeaponSlotToUnequip.Name) {
								updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [i], null);
							}
						}
					}

					if (draggedFromInventoryList && slotFoundInventoryWeaponSlotElement.secondarySlotInfo.slotActive) {
						print ("equipping dual weapon");
						activatingDualWeaponSlot = true;

						currentRighWeaponName = currentWeaponSlotToMoveInventoryObject.IKWeaponManager.getWeaponSystemName ();
						currentLeftWeaponName = slotFoundInventoryWeaponSlotElement.secondarySlotInfo.weaponSystem.getWeaponSystemName ();

						updateWeaponSlotInfo (weaponSlotFoundIndex, currentWeaponSlotToMoveInventoryObject, null, slotFoundInventoryWeaponSlotElement.secondarySlotInfo.weaponSystem);

						currentInventoryObject = currentWeaponSlotToMoveInventoryObject;
						equipCurrentObject ();
					}
				}
			}

			if (currentSlotDroppedFoundOnList) {
				if (draggedFromInventoryList && !activatingDualWeaponSlot) {
					
					updateWeaponSlotInfo (weaponSlotFoundIndex, currentWeaponSlotToMoveInventoryObject, null, null);

					currentInventoryObject = currentWeaponSlotToMoveInventoryObject;
					equipCurrentObject ();
				}
	
				print ("dropped correctly");
			} else {
				print ("dropped outside the list");

				if (currentWeaponSlotToUnequip != null) {
					if (currentWeaponSlotToUnequip.dualWeaponSlotActive) {
						print ("dual weaponn " + currentWeaponSlotToUnequip.weaponSystem.getWeaponSystemName () + " removed");

						activatingDualWeaponSlot = true;

						weaponsManager.unequipWeapon (currentWeaponSlotToUnequip.secondaryWeaponSystem.getWeaponSystemName (), activatingDualWeaponSlot);
					} else {
						print ("dragged from Weapon List to unequip weapon");

						weaponsManager.unequipWeapon (currentWeaponSlotToUnequip.Name, false);
					}

					updateWeaponSlotInfo (-1, null, currentWeaponSlotToUnequip, null);
				}
			}
		} else {
			if (draggedFromWeaponSlots) {
				if (currentWeaponSlotToUnequip.dualWeaponSlotActive) {
					print ("dual weaponn " + currentWeaponSlotToUnequip.weaponSystem.getWeaponSystemName () + " removed");

					activatingDualWeaponSlot = true;

					weaponsManager.unequipWeapon (currentWeaponSlotToUnequip.secondaryWeaponSystem.getWeaponSystemName (), activatingDualWeaponSlot);
				} else {
					print ("dragged from Weapon List to unequip weapon");

					weaponsManager.unequipWeapon (currentWeaponSlotToUnequip.Name, false);
				}
					
				updateWeaponSlotInfo (-1, null, currentWeaponSlotToUnequip, null);
			}
		}

		resetDragAndDropWeaponSlotState ();
	}

	public void resetDragAndDropWeaponSlotState ()
	{
		activatingDualWeaponSlot = false;

		weaponSlotToMove.SetActive (false);

		weaponSlotFound = null;
		draggedFromInventoryList = false;
		draggedFromWeaponSlots = false;
		weaponSlotToMoveFound = false;
	}

	public void updateSingleWeaponSlotInfo (string currentRighWeaponName, string currentLeftWeaponName)
	{
		playerWeaponSystem currentRightWeaponSystem = weaponsManager.getWeaponSystemByName (currentRighWeaponName);
		playerWeaponSystem currentLeftWeaponSystem = weaponsManager.getWeaponSystemByName (currentLeftWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		secondaryWeaponSlotInfo currentSlotToDualWeapos = secondaryWeaponSlotInfoList [currentSlotToDualWeaponsIndex];

		activatingDualWeaponSlot = false;

		updateWeaponSlotInfo (currentSlotToDualWeaponsIndex, currentSlotToDualWeapos.weaponInventoryInfo, null, null);

		//get the amount of free slots
		int firstFreeWeaponSlotIndex = -1;

		int numberOfFreeSlots = 0;
		for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
			if (!secondaryWeaponSlotInfoList [i].slotActive) {
				numberOfFreeSlots++;
			}
		}

		if (numberOfFreeSlots > 0) {
			firstFreeWeaponSlotIndex = -1;
			for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
				if (!secondaryWeaponSlotInfoList [i].slotActive) {
					if (firstFreeWeaponSlotIndex == -1) {
						firstFreeWeaponSlotIndex = i;
					}
				}
			}
	
			for (int i = 0; i < inventoryList.Count; i++) {
				if (inventoryList [i].isWeapon) {
					if (inventoryList [i].Name == currentLeftWeaponSystem.getWeaponSystemName ()) {
						currentWeaponSlotToMoveInventoryObject = inventoryList [i];
					}
				}
			}

			updateWeaponSlotInfo (firstFreeWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, null);
		} else {
			weaponsManager.unequipWeapon (currentLeftWeaponSystem.getWeaponSystemName (), false);
		}
	}

	public void updateSingleWeaponSlotInfoWithoutAddingAnotherSlot (string currentRighWeaponName)
	{
		int rightWeaponSlotIndex = -1;

		playerWeaponSystem currentRightWeaponSystem = weaponsManager.getWeaponSystemByName (currentRighWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].isWeapon) {
				if (inventoryList [i].Name == currentRighWeaponName) {
					currentWeaponSlotToMoveInventoryObject = inventoryList [i];
				}
			}
		}

		activatingDualWeaponSlot = false;

		print ("set single slot for weapon " + currentRighWeaponName + " on slot " + rightWeaponSlotIndex);

		updateWeaponSlotInfo (currentSlotToDualWeaponsIndex, currentWeaponSlotToMoveInventoryObject, null, null);
	}

	public void updateDualWeaponSlotInfo (string currentRighWeaponName, string currentLeftWeaponName)
	{
		int rightWeaponSlotIndex = -1;
		int leftWeaponSlotIndex = -1;

		playerWeaponSystem currentRightWeaponSystem = weaponsManager.getWeaponSystemByName (currentRighWeaponName);
		playerWeaponSystem currentLeftWeaponSystem = weaponsManager.getWeaponSystemByName (currentLeftWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		secondaryWeaponSlotInfo currentSlotToDualWeapos = secondaryWeaponSlotInfoList [currentSlotToDualWeaponsIndex];

		playerWeaponSystem rightWeaponSystemToSetToSingle = currentSlotToDualWeapos.weaponSystem;
		playerWeaponSystem leftWeaponSystemToSetToSingle = currentSlotToDualWeapos.secondaryWeaponSystem;

		bool rightWeaponIsDualOnOtherSlot = false;
		bool leftWeaponIsDualOnOtherSlot = false;

		bool rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon = false;
		bool leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon = false;

		for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
			if (currentSlotToDualWeaponsIndex != i) {
				//search the right weapon slot of the new right weapon
				if (secondaryWeaponSlotInfoList [i].Name == currentRighWeaponName) {
					rightWeaponSlotIndex = i;
				}

				if (secondaryWeaponSlotInfoList [i].rightWeaponName == currentRighWeaponName) {
					rightWeaponSlotIndex = i;

					rightWeaponIsDualOnOtherSlot = true;
					rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon = true;
				}

				if (secondaryWeaponSlotInfoList [i].leftWeaponName == currentRighWeaponName) {
					rightWeaponSlotIndex = i;

					rightWeaponIsDualOnOtherSlot = true;
				}

				//search the left weapon slot of the new left weapon
				if (secondaryWeaponSlotInfoList [i].Name == currentLeftWeaponName) {
					leftWeaponSlotIndex = i;
				}

				if (secondaryWeaponSlotInfoList [i].rightWeaponName == currentLeftWeaponName) {
					leftWeaponSlotIndex = i;

					leftWeaponIsDualOnOtherSlot = true;
					leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon = true;
				}

				if (secondaryWeaponSlotInfoList [i].leftWeaponName == currentLeftWeaponName) {
					leftWeaponSlotIndex = i;

					leftWeaponIsDualOnOtherSlot = true;
				}
			}
		}

		//remove or add a new slot for the right weapon according to if it was configured as a single weapon in its own slot or dual weapon in a dual slot
		if (rightWeaponSlotIndex != -1) {
			if (rightWeaponIsDualOnOtherSlot) {
				print ("right weapon is dual on other slot");

				if (rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon) {

					print ("right weapon is main weapon on other slot");

					//in this case, leave the other weapon from this slot as the main weapon for this slot and remove the current right one from it
					string secondaryWeaponNameOnSlot = secondaryWeaponSlotInfoList [rightWeaponSlotIndex].leftWeaponName;

					for (int i = 0; i < inventoryList.Count; i++) {
						if (inventoryList [i].isWeapon) {
							if (inventoryList [i].Name == secondaryWeaponNameOnSlot) {
								currentWeaponSlotToMoveInventoryObject = inventoryList [i];
							}
						}
					}

					activatingDualWeaponSlot = false;

					updateWeaponSlotInfo (rightWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, null);
				} else {

					print ("right weapon is secondary weapon on other slot");

					activatingDualWeaponSlot = false;

					updateWeaponSlotInfo (rightWeaponSlotIndex, secondaryWeaponSlotInfoList [rightWeaponSlotIndex].weaponInventoryInfo, null, null);
				}
			} else {
				updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [rightWeaponSlotIndex], null);
			}
		}

		//remove or add a new slot for the left weapon according to if it was configured as a single weapon in its own slot or dual weapon in a dual slot
		if (leftWeaponSlotIndex != -1) {
			if (leftWeaponIsDualOnOtherSlot) {
				print ("left weapon is dual on other slot");

				if (leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon) {

					print ("left weapon is main weapon on other slot");

					//in this case, leave the other weapon from this slot as the main weapon for this slot and remove the current left one from it
					string secondaryWeaponNameOnSlot = secondaryWeaponSlotInfoList [leftWeaponSlotIndex].leftWeaponName;

					for (int i = 0; i < inventoryList.Count; i++) {
						if (inventoryList [i].isWeapon) {
							if (inventoryList [i].Name == secondaryWeaponNameOnSlot) {
								currentWeaponSlotToMoveInventoryObject = inventoryList [i];
							}
						}
					}

					activatingDualWeaponSlot = false;

					updateWeaponSlotInfo (leftWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, null);
				} else {

					//in this case, the left weapon is configured as a secondary weapon in another slot, so set that slot as single again, removing the current left weapon from that slot
					print ("left weapon is secondary weapon on other slot");

					activatingDualWeaponSlot = false;

					updateWeaponSlotInfo (leftWeaponSlotIndex, secondaryWeaponSlotInfoList [leftWeaponSlotIndex].weaponInventoryInfo, null, null);
				}
			} else {
				updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [leftWeaponSlotIndex], null);
			}
		}

		// configure the two selected weapons in the same slot
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].isWeapon) {
				if (inventoryList [i].Name == currentRighWeaponName) {
					currentWeaponSlotToMoveInventoryObject = inventoryList [i];
				}
			}
		}

		activatingDualWeaponSlot = true;

		print (currentRightWeaponSystem.getWeaponSystemName () + " configured as right weapon and " + currentLeftWeaponSystem.getWeaponSystemName () + " configured as left weapon");

		updateWeaponSlotInfo (currentSlotToDualWeaponsIndex, currentWeaponSlotToMoveInventoryObject, null, currentLeftWeaponSystem);

		activatingDualWeaponSlot = false;


		//get the amount of free slots
		int firstFreeWeaponSlotIndex = -1;

		int numberOfFreeSlots = 0;
		for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
			if (!secondaryWeaponSlotInfoList [i].slotActive) {
				numberOfFreeSlots++;
			}
		}

		//get the original weapons assigned in the current slot which is used for other couple of weapons
		//if the number of slots available is higher than 1, both weapons can be placed as new slots if there is right and left weapon to assign
		if (numberOfFreeSlots > 1 ||
		    (rightWeaponSystemToSetToSingle != null && leftWeaponSystemToSetToSingle == null) || (rightWeaponSystemToSetToSingle == null && leftWeaponSystemToSetToSingle != null)) {

			//assign the first free slot to the right weapon
			if (numberOfFreeSlots > 1 || (rightWeaponSystemToSetToSingle != null && leftWeaponSystemToSetToSingle == null)) {
				if (rightWeaponSystemToSetToSingle) {

					firstFreeWeaponSlotIndex = -1;
					for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
						if (!secondaryWeaponSlotInfoList [i].slotActive) {
							if (firstFreeWeaponSlotIndex == -1) {
								firstFreeWeaponSlotIndex = i;
							}
						}
					}

					if (rightWeaponSystemToSetToSingle.getWeaponSystemName () != currentRighWeaponName && rightWeaponSystemToSetToSingle.getWeaponSystemName () != currentLeftWeaponName) {
						for (int i = 0; i < inventoryList.Count; i++) {
							if (inventoryList [i].isWeapon) {
								if (inventoryList [i].Name == rightWeaponSystemToSetToSingle.getWeaponSystemName ()) {
									currentWeaponSlotToMoveInventoryObject = inventoryList [i];
								}
							}
						}

						updateWeaponSlotInfo (firstFreeWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, null);
					}
				}
			}

			//assign the first free slot to the left weapon
			if (numberOfFreeSlots > 1 || (rightWeaponSystemToSetToSingle == null && leftWeaponSystemToSetToSingle != null)) {
				if (leftWeaponSystemToSetToSingle) {

					firstFreeWeaponSlotIndex = -1;
					for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
						if (!secondaryWeaponSlotInfoList [i].slotActive) {
							if (firstFreeWeaponSlotIndex == -1) {
								firstFreeWeaponSlotIndex = i;
							}
						}
					}

					if (leftWeaponSystemToSetToSingle.getWeaponSystemName () != currentRighWeaponName && leftWeaponSystemToSetToSingle.getWeaponSystemName () != currentLeftWeaponName) {
						for (int i = 0; i < inventoryList.Count; i++) {
							if (inventoryList [i].isWeapon) {
								if (inventoryList [i].Name == leftWeaponSystemToSetToSingle.getWeaponSystemName ()) {
									currentWeaponSlotToMoveInventoryObject = inventoryList [i];
								}
							}
						}

						updateWeaponSlotInfo (firstFreeWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, null);
					}
				}
			}
		} else {
			//else, the number of free slot is only 1, so both weapons need to be combined
			for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
				if (!secondaryWeaponSlotInfoList [i].slotActive) {
					if (firstFreeWeaponSlotIndex == -1) {
						firstFreeWeaponSlotIndex = i;
					}
				}
			}

			if (firstFreeWeaponSlotIndex != -1) {
				activatingDualWeaponSlot = true;

				for (int i = 0; i < inventoryList.Count; i++) {
					if (inventoryList [i].isWeapon) {
						if (inventoryList [i].Name == rightWeaponSystemToSetToSingle.getWeaponSystemName ()) {
							currentWeaponSlotToMoveInventoryObject = inventoryList [i];
						}
					}
				}

				updateWeaponSlotInfo (firstFreeWeaponSlotIndex, currentWeaponSlotToMoveInventoryObject, null, leftWeaponSystemToSetToSingle);

				activatingDualWeaponSlot = false;
			}
		}
	}

	public void updateWeaponSlotInfo (int weaponSlotIndex, inventoryInfo currentWeaponSlotToMove, secondaryWeaponSlotInfo weaponSlotToUnEquip, playerWeaponSystem secondaryWeaponToEquip)
	{
		bool slotFound = false;

		if (weaponSlotIndex > -1 && weaponSlotIndex < secondaryWeaponSlotInfoList.Count && currentWeaponSlotToMove != null) {
			secondaryWeaponSlotInfo currentSecondaryWeaponSlotInfo = secondaryWeaponSlotInfoList [weaponSlotIndex];
			if (currentSecondaryWeaponSlotInfo != null) {
				currentSecondaryWeaponSlotInfo.Name = currentWeaponSlotToMove.Name;
				currentSecondaryWeaponSlotInfo.slotActive = true;

				playerWeaponSystem currentPlayerWeaponSystem = currentWeaponSlotToMove.IKWeaponManager.getWeaponSystemManager ();
				currentSecondaryWeaponSlotInfo.weaponSystem = currentPlayerWeaponSystem;

				currentSecondaryWeaponSlotInfo.dualWeaponSlotActive = activatingDualWeaponSlot;
				if (activatingDualWeaponSlot) {
					currentSecondaryWeaponSlotInfo.secondaryWeaponSystem = secondaryWeaponToEquip;
					currentSecondaryWeaponSlotInfo.weaponAmmoText.text = "";

					currentSecondaryWeaponSlotInfo.dualRightWeaponIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();
					currentSecondaryWeaponSlotInfo.dualLeftWeaponIcon.texture = secondaryWeaponToEquip.getWeaponInventorySlotIcon ();

					currentSecondaryWeaponSlotInfo.rightWeaponName = currentPlayerWeaponSystem.getWeaponSystemName ();
					currentSecondaryWeaponSlotInfo.leftWeaponName = secondaryWeaponToEquip.getWeaponSystemName ();
				} else {
					currentSecondaryWeaponSlotInfo.weaponAmmoText.text = currentPlayerWeaponSystem.getCurrentAmmoText ();

					currentSecondaryWeaponSlotInfo.weaponIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();

					currentSecondaryWeaponSlotInfo.rightWeaponName = "";
					currentSecondaryWeaponSlotInfo.leftWeaponName = "";

					currentSecondaryWeaponSlotInfo.secondaryWeaponSystem = null;
				}
					
				currentSecondaryWeaponSlotInfo.weaponAmmoTextContent.SetActive (currentPlayerWeaponSystem.weaponSettings.weaponUsesAmmo);
			
				currentSecondaryWeaponSlotInfo.slotMainSingleContent.SetActive (!activatingDualWeaponSlot);
				currentSecondaryWeaponSlotInfo.slotMainDualContent.SetActive (activatingDualWeaponSlot);
			
				currentSecondaryWeaponSlotInfo.weaponInventoryInfo = currentWeaponSlotToMove;

				int newNumberKey = weaponSlotIndex + 1;
				if (newNumberKey > 9) {
					newNumberKey = 0;
				}

				currentPlayerWeaponSystem.setNumberKey (newNumberKey);

				if (activatingDualWeaponSlot) {
					secondaryWeaponToEquip.setNumberKey (newNumberKey);
				}

				slotFound = true;
			}
		}

		if (!slotFound && weaponSlotToUnEquip != null) {

			if (activatingDualWeaponSlot) {
				playerWeaponSystem currentPlayerWeaponSystem = weaponSlotToUnEquip.weaponSystem;
				weaponSlotToUnEquip.weaponAmmoText.text = currentPlayerWeaponSystem.getCurrentAmmoText ();

				weaponSlotToUnEquip.weaponIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();

				weaponSlotToUnEquip.slotMainSingleContent.SetActive (true);
				weaponSlotToUnEquip.slotMainDualContent.SetActive (false);

				weaponSlotToUnEquip.secondaryWeaponSystem = null;

				weaponSlotToUnEquip.dualWeaponSlotActive = false;

				weaponSlotToUnEquip.rightWeaponName = "";
				weaponSlotToUnEquip.leftWeaponName = "";
			} else {
				weaponSlotToUnEquip.Name = "";
				weaponSlotToUnEquip.slotActive = false;
				weaponSlotToUnEquip.weaponSystem = null;
				weaponSlotToUnEquip.weaponAmmoText.text = "";
				weaponSlotToUnEquip.weaponIcon.texture = null;
				weaponSlotToUnEquip.weaponInventoryInfo = null;

				weaponSlotToUnEquip.slotMainSingleContent.SetActive (false);
				weaponSlotToUnEquip.slotMainDualContent.SetActive (false);
			}
		}
	}

	public void updateWeaponSlotAmmo (int weaponSlotIndex)
	{
		secondaryWeaponSlotInfoList [weaponSlotIndex].weaponAmmoText.text = secondaryWeaponSlotInfoList [weaponSlotIndex].weaponSystem.getCurrentAmmoText (); 
	}

	public string getFirstSingleWeaponSlot (string weaponNameToAvoid)
	{
		for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
			if (secondaryWeaponSlotInfoList [i].slotActive && !secondaryWeaponSlotInfoList [i].dualWeaponSlotActive &&
			    (weaponNameToAvoid == "" || weaponNameToAvoid != secondaryWeaponSlotInfoList [i].Name)) {
				return secondaryWeaponSlotInfoList [i].Name;
			}
		}

		return "";
	}

	public void setInventorySlotAmountValue (int newValue)
	{
		inventorySlotAmount = newValue;
	}

	public void setInventoryFromInventoryListManager ()
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			if (inventoryListManagerList [i].addInventoryObject) {
				inventoryInfo currentInventoryInfo = mainInventoryListManager.inventoryList [inventoryListManagerList [i].elementIndex];
				if (currentInventoryInfo != null) {
					inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
					newInventoryInfo.Name = inventoryListManagerList [i].name;
					newInventoryInfo.amount = inventoryListManagerList [i].amount;
					newInventoryInfo.isEquiped = inventoryListManagerList [i].isEquipped;
					inventoryList.Add (newInventoryInfo);
				}
			}
		}

		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			if (!inventoryListManagerList [i].addInventoryObject) {
				inventoryListManagerList.RemoveAt (i);
				i = 0;
			}
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

	public void setInventory (bool creatingInventoryIcons)
	{
		checkInventoryAmountPerSpace ();
		checkRemainigEmptyInventorySlots ();
		if (creatingInventoryIcons) {
			createInventoryIcons ();
		}
	}

	public void checkInventoryAmountPerSpace ()
	{
		if (infiniteAmountPerSlot) {
			return;
		}
		bucle = 0;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].amount > amountPerSlot) {
				while (inventoryList [i].amount > amountPerSlot) {
					bucle++;
					if (bucle > 100) {
						//print ("bucle loop");
						return;
					}
					int amount = 0;
					if (inventoryList [i].amount - amountPerSlot > 0) {
						amount = inventoryList [i].amount - amountPerSlot;
						inventoryList [i].amount = amountPerSlot;
					} else {
						amount = inventoryList [i].amount;
					}
					i = reOrderInventoryList (inventoryList [i], amount, i);
				}
			}
		}
	}

	public int reOrderInventoryList (inventoryInfo objectInfo, int amount, int index)
	{
		//print ("reordenate");
		int numberOfSlots = inventorySlotAmount;
		if (infiniteSlots) {
			numberOfSlots = inventoryList.Count * numberOfSlots;
		}
		if (inventoryList.Count >= numberOfSlots) {
			//print (amount);
			bool amountAdded = false;
			if (getNumberOfFreeSlots () > 0) {
				for (int i = 0; i < inventoryList.Count; i++) {
					if (inventoryList [i].amount == 0 && inventoryList [i].inventoryGameObject != objectInfo.inventoryGameObject && inventoryList [i] != objectInfo && !amountAdded) {
						addObjectToInventory (objectInfo, amount, i);
						inventoryAmountNotTaken -= amount;
						amountAdded = true;
					}
				}
			}
			if (currentPickUpObjectInfo != null) {
				if (currentPickUpObjectInfo.inventoryGameObject == objectInfo.inventoryGameObject) {
					inventoryAmountNotTaken += amount;
					//print (inventoryAmountNotTaken);
				}
			}
		} else {
			int newIndexPosition = 0;
			if (index == inventoryList.Count - 1) {
				newIndexPosition = inventoryList.Count;
			} else {
				newIndexPosition = index + 1;
			}
			index++;
			duplicateObject = new inventoryInfo (objectInfo);
			duplicateObject.amount = amount;
			inventoryList.Insert (newIndexPosition, duplicateObject);
		}
		return index;
	}

	public void checkRemainigEmptyInventorySlots ()
	{
		if (infiniteSlots) {
			return;
		}
		bucle = 0;
		if (inventoryList.Count < inventorySlotAmount) {
			while (inventoryList.Count < inventorySlotAmount) {
				bucle++;
				if (bucle > 100) {
					//print ("loop bucle");
					return;
				}
				addNewInventorySlot ();
			}
		}
	}

	public void addNewInventorySlot ()
	{
		inventoryInfo newEmptyInventoryObject = new inventoryInfo ();
		newEmptyInventoryObject.Name = "Empty Slot";
		newEmptyInventoryObject.objectInfo = "It is an empty slot";
		inventoryList.Add (newEmptyInventoryObject);
	}

	public void checkEmptySlots ()
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].amount == 0) {
				inventoryList.Add (inventoryList [i]);
				inventoryList.RemoveAt (i);
			}
		}
		updateFullInventorySlots ();
	}

	public void createInventoryIcons ()
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].button != null) {
				Destroy (inventoryList [i].button.gameObject);
			}
		}
		for (int i = 0; i < inventoryList.Count; i++) {
			createInventoryIcon (inventoryList [i], i);
		}
	}

	public void createInventoryIcon (inventoryInfo currentInventoryInfo, int index)
	{
		GameObject newIconButton = (GameObject)Instantiate (objectIcon, Vector3.zero, Quaternion.identity);
		newIconButton.SetActive (true);
		newIconButton.transform.SetParent (inventoryListContent.transform);
		newIconButton.transform.localScale = Vector3.one;
		newIconButton.transform.localPosition = Vector3.zero;

		inventoryMenuIconElement menuIconElement = newIconButton.GetComponent<inventoryMenuIconElement> ();
		menuIconElement.iconName.text = currentInventoryInfo.Name;
		if (currentInventoryInfo.inventoryGameObject != null) {
			menuIconElement.icon.texture = currentInventoryInfo.icon;
		} else {
			menuIconElement.icon.texture = null;
		}

		bool slotIsActive = currentInventoryInfo.amount > 0;
		menuIconElement.activeSlotContent.SetActive (slotIsActive);
		menuIconElement.emptySlotContent.SetActive (!slotIsActive);

		menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();
		menuIconElement.pressedIcon.SetActive (false);
		newIconButton.name = "Inventory Object-" + (index + 1).ToString ();
		Button button = menuIconElement.button;

		currentInventoryInfo.button = button;
		currentInventoryInfo.menuIconElement = menuIconElement;
		menuIconElementList.Add (menuIconElement);
	}

	int customAmountToUSe = 1;

	public void setCustomAmountToUse (float customAmount)
	{
		print ("HAO");
		customAmountToUSe = (int)customAmount;
	}

	public void tryToPickUpObjectByName (string objectName)
	{
		inventoryInfo inventoryObjectToPick = mainInventoryListManager.getInventoryInfoFromName (objectName);

		if (inventoryObjectToPick != null) {
			print (customAmountToUSe);
			inventoryObjectToPick.amount = customAmountToUSe;
			tryToPickUpObject (inventoryObjectToPick);
			customAmountToUSe = 1;
		}
	}

	public int tryToPickUpObject (inventoryInfo inventoryObjectToPickup)
	{
		inventoryAmountNotTaken = 0;
		currentPickUpObjectInfo = new inventoryInfo (inventoryObjectToPickup);

		if (infiniteAmountPerSlot) {
			int amountToTake = currentPickUpObjectInfo.amount;
		
			addAmountToInventorySlot (currentPickUpObjectInfo, -1, amountToTake);
		} else {
			int freeSpaceInInventorySlot = freeSpaceInSlot (currentPickUpObjectInfo.inventoryGameObject);

			if (freeSpaceInInventorySlot == 0 || freeSpaceInInventorySlot > 0) {
				print ("same slot type, full or with space");
				int amountToTake = currentPickUpObjectInfo.amount;

				int inventoryAmountInSlot = amountPerSlot - freeSpaceInInventorySlot;

				addAmountToInventorySlot (currentPickUpObjectInfo, inventoryAmountInSlot, amountToTake);
			} else {
				if (getNumberOfFreeSlots () >= 1) {
					print ("empty slots available");
					int amountToTake = currentPickUpObjectInfo.amount;

					addObjectToInventory (currentPickUpObjectInfo, amountToTake, -1);
				} else {
					print ("no slots available");
					inventoryAmountNotTaken = currentPickUpObjectInfo.amount;
				}
			}
		}
		setInventory (false);

		int inventoryAmountPicked = currentPickUpObjectInfo.amount - inventoryAmountNotTaken;

		//print (inventoryAmountPicked);
		currentPickUpObjectInfo = null;

		if (infiniteSlots) {

			int inventoryWithoutSlotAssigned = 0;
			for (int i = 0; i < inventoryList.Count; i++) {
				if (inventoryList [i].menuIconElement == null) {
					inventoryWithoutSlotAssigned++;
				}
			}
			inventoryWithoutSlotAssigned = inventoryList.Count - inventoryWithoutSlotAssigned;
			for (int i = inventoryWithoutSlotAssigned; i < inventoryList.Count; i++) {
				createInventoryIcon (inventoryList [i], i);
			}
			checkEmptySlots ();
		}

		updateAmountsInventoryPanel ();
		checkCurrentUseInventoryObject ();

		//manage weapons stored in the inventory
		if (equipWeaponsWhenPicked && weaponsManager.drawWeaponWhenPicked && inventoryAmountPicked > 0 && inventoryObjectToPickup.canBeEquiped) {
			currentInventoryObject = new inventoryInfo (inventoryObjectToPickup);
			equipCurrentObject ();
		}

		if (examiningObject) {
			examineCurrentObject (false);
		}

		return inventoryAmountPicked;
	}

	//add an object grabbed by the player to the current inventory
	public void addObjectToInventory (inventoryInfo objectToAdd, int amountToTake, int index)
	{
		if (infiniteSlots) {
			addNewInventorySlot ();
			for (int i = 0; i < inventoryList.Count; i++) {
				if (inventoryList [i].button == null) {
					createInventoryIcon (inventoryList [i], i);
					//print ("new slot added");
				}
			}
		}

		inventoryInfo currentInventoryInfo = new inventoryInfo ();
		if (index > -1) {
			currentInventoryInfo = inventoryList [index];
		} else {
			bool inventoryInfoFound = false;
			for (int i = 0; i < inventoryList.Count; i++) {
				if (!inventoryInfoFound) {
					if (inventoryList [i].amount == 0) {
						currentInventoryInfo = inventoryList [i];
						inventoryInfoFound = true;
					}
				}
			}
		}
			
		inventoryInfo inventoryObject = new inventoryInfo (mainInventoryListManager.getInventoryInfoFromInventoryGameObject (objectToAdd.inventoryGameObject));
		if (inventoryObject != null) {
			currentInventoryInfo.Name = inventoryObject.Name;
			currentInventoryInfo.inventoryGameObject = inventoryObject.inventoryGameObject;
			currentInventoryInfo.objectInfo = inventoryObject.objectInfo;
			currentInventoryInfo.icon = inventoryObject.icon;
			currentInventoryInfo.amount = amountToTake;
			currentInventoryInfo.amountPerUnit = objectToAdd.amountPerUnit;
			currentInventoryInfo.canBeUsed = inventoryObject.canBeUsed;
			currentInventoryInfo.canBeEquiped = inventoryObject.canBeEquiped;
			currentInventoryInfo.canBeDropped = inventoryObject.canBeDropped;
			currentInventoryInfo.canBeCombined = inventoryObject.canBeCombined;

			currentInventoryInfo.isEquiped = inventoryObject.isEquiped;

			if (currentInventoryInfo.isEquiped) {
				currentInventoryInfo.menuIconElement.equipedIcon.SetActive (true);
			} else {
				currentInventoryInfo.menuIconElement.equipedIcon.SetActive (false);
			}

			currentInventoryInfo.isWeapon = inventoryObject.isWeapon;
			if (currentInventoryInfo.isWeapon) {
				//print (currentInventoryInfo.Name);
				currentIKWeaponSystem = weaponsManager.getIKWeaponSystem (currentInventoryInfo.Name);
				//print (currentIKWeaponSystem.getWeaponSystemName ());
				currentInventoryInfo.IKWeaponManager = currentIKWeaponSystem;
			} else {
				currentInventoryInfo.IKWeaponManager = null;
			}

			currentInventoryInfo.objectToCombine = inventoryObject.objectToCombine;
			currentInventoryInfo.combinedObject = inventoryObject.combinedObject;
			currentInventoryInfo.combinedObjectMessage = inventoryObject.combinedObjectMessage;
			currentInventoryInfo.menuIconElement.iconName.text = currentInventoryInfo.Name;
			currentInventoryInfo.menuIconElement.icon.texture = inventoryObject.icon;
			currentInventoryInfo.menuIconElement.amount.text = amountToTake.ToString ();

			bool slotIsActive = amountToTake > 0;
			currentInventoryInfo.menuIconElement.activeSlotContent.SetActive (slotIsActive);
			currentInventoryInfo.menuIconElement.emptySlotContent.SetActive (!slotIsActive);
		}
	}

	public void updateFullInventorySlots ()
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];
			currentInventoryInfo.menuIconElement = menuIconElementList [i];
			currentInventoryInfo.button = menuIconElementList [i].button;
			currentInventoryInfo.menuIconElement.iconName.text = currentInventoryInfo.Name;
			currentInventoryInfo.menuIconElement.icon.texture = currentInventoryInfo.icon;
			currentInventoryInfo.menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();

			bool slotIsActive = currentInventoryInfo.amount > 0;
			currentInventoryInfo.menuIconElement.activeSlotContent.SetActive (slotIsActive);
			currentInventoryInfo.menuIconElement.emptySlotContent.SetActive (!slotIsActive);

			currentInventoryInfo.isEquiped = inventoryList [i].isEquiped;

			if (currentInventoryInfo.isEquiped) {
				currentInventoryInfo.menuIconElement.equipedIcon.SetActive (true);
			} else {
				currentInventoryInfo.menuIconElement.equipedIcon.SetActive (false);
			}

			currentInventoryInfo.isWeapon = inventoryList [i].isWeapon;
			if (currentInventoryInfo.isWeapon) {
				currentIKWeaponSystem = weaponsManager.getIKWeaponSystem (currentInventoryInfo.Name);
				currentInventoryInfo.IKWeaponManager = currentIKWeaponSystem;
			} else {
				currentInventoryInfo.IKWeaponManager = null;
			}
		}

		if (infiniteSlots) {
			for (int i = 0; i < inventoryList.Count; i++) {
				if (inventoryList [i].amount == 0) {
					//print (i);
					Destroy (inventoryList [i].button.gameObject);
					inventoryList.RemoveAt (i);
					menuIconElementList.RemoveAt (i);
					i--;
				}
			}
		}
	}

	public void addInventoryExtraSpace (int amount)
	{
		if (infiniteSlots) {
			return;
		}
		inventorySlotAmount += amount;
		checkRemainigEmptyInventorySlots ();
		for (int i = 0; i < inventorySlotAmount; i++) {
			if (inventoryList [i].button == null) {
				createInventoryIcon (inventoryList [i], i);
			}
		}
	}

	public void getPressedButton (Button buttonObj)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].button == buttonObj) {
				if (currentInventoryObject != null) {
					if (currentInventoryObject == inventoryList [i] && currentInventoryObject.menuIconElement.pressedIcon.activeSelf) {
						return;
					}
				}
				setObjectInfo (inventoryList [i]);

				float currentCameraFov = originalFov;
				if (currentInventoryObjectManager) {
					if (currentInventoryObjectManager.useZoomRange) {
						currentCameraFov = currentInventoryObjectManager.initialZoom;
					}
				}
				if (inventoryCamera.fieldOfView != currentCameraFov) {
					checkResetCameraFov (currentCameraFov);
				}
				return;
			}
		}
	}

	public void disableCurrentObjectInfo ()
	{
		currentObjectName.text = "";
		currentObjectInfo.text = "";
		useButtonImage.color = buttonNotUsable;

		equipButtonImage.color = buttonNotUsable;
		unequipButtonImage.color = buttonNotUsable;

		equipButton.SetActive (true);
		unequipButton.SetActive (false);

		discardButtonImage.color = buttonNotUsable;

		dropAllUnitsObjectButtonImage.color = buttonNotUsable;

		dropButtonImage.color = buttonNotUsable;
		combineButtonImage.color = buttonNotUsable;
		examineButtonImage.color = buttonNotUsable;
		objectImage.enabled = false;
	}

	public void setEquipButtonState (bool state)
	{
		if (state) {
			unequipButtonImage.color = buttonNotUsable;
			unequipButton.SetActive (false);
			equipButtonImage.color = buttonUsable;
			equipButton.SetActive (true);
		} else {
			unequipButtonImage.color = buttonUsable;
			unequipButton.SetActive (true);
			equipButtonImage.color = buttonNotUsable;
			equipButton.SetActive (false);
		}
	}

	public void setCurrenObjectByPrefab (GameObject obj)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == obj) {
				currentInventoryObject = inventoryList [i];
				return;
			}
		}
		currentInventoryObject = null;
	}

	public bool inventoryContainsObject (GameObject objectToCheck)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == objectToCheck) {
				if (inventoryList [i].amount > 0) {
					return true;
				}
			}
		}
		return false;
	}

	public void searchForObjectNeed (GameObject obj)
	{
		setCurrentUseInventoryGameObject (obj);
		useCurrentObject ();
	}

	public void setCurrentUseInventoryGameObject (GameObject obj)
	{
		currentUseInventoryGameObject = obj;
		if (currentUseInventoryGameObject) {
			currentUseInventoryObject = currentUseInventoryGameObject.GetComponent<useInventoryObject> ();
		} else {
			currentUseInventoryObject = null;
		}
	}

	public void removeCurrentInventoryObject ()
	{
		currentInventoryObject = null;
	}

	public void removeCurrentUseInventoryObject ()
	{
		currentUseInventoryObject = null;
	}

	public void setAllAmountObjectToUse ()
	{
		numberOfObjectsToUse = currentInventoryObject.amount;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void addObjectToUse ()
	{
		numberOfObjectsToUse++;
		if (numberOfObjectsToUse > currentInventoryObject.amount) {
			numberOfObjectsToUse = currentInventoryObject.amount;
		}
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void removeObjectToUse ()
	{
		numberOfObjectsToUse--;
		if (numberOfObjectsToUse < 1) {
			numberOfObjectsToUse = 1;
		}
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void startToAddObjectToUse ()
	{
		addingObjectToUse = true;
	}

	public void stopToAddObjectToUse ()
	{
		addingObjectToUse = false;
		lastTimeAddObjectToUse = 0;
	}

	public void startToRemoveObjectToUse ()
	{
		removinvObjectToUse = true;
	}

	public void stopToRemoveObjectToUse ()
	{
		removinvObjectToUse = false;
		lastTimeRemoveObjectToUse = 0;
	}

	public void setNumberOfObjectsToUseText (int amount)
	{
		numberOfObjectsToUseText.text = amount.ToString ();
	}

	public void enableNumberOfObjectsToUseMenu (RectTransform menuPosition)
	{
		if (activeNumberOfObjectsToUseMenu) {
			if (isCurrentObjectNotNull ()) {
				if ((currentInventoryObject.canBeUsed && menuPosition == numberOfObjectsToUseMenuPosition) ||
				    (currentInventoryObject.canBeDropped && menuPosition == numberOfObjectsToDropMenuPosition)) {

					numberOfObjectsToUseMenu.SetActive (true);
					numberOfObjectsToUseMenuRectTransform.anchoredPosition = menuPosition.anchoredPosition;
					numberOfObjectsToUse = 1;
					setNumberOfObjectsToUseText (numberOfObjectsToUse);
				}
			}
		}
	}

	public void disableNumberOfObjectsToUseMenu ()
	{
		numberOfObjectsToUse = 1;
		numberOfObjectsToUseMenu.SetActive (false);
	}

	public void resetNumberOfObjectsToUse ()
	{
		numberOfObjectsToUse = 1;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void resetAndDisableNumberOfObjectsToUseMenu ()
	{
		disableNumberOfObjectsToUseMenu ();
		resetNumberOfObjectsToUse ();
	}

	public void useCurrentObjectByButton (RectTransform menuPosition)
	{
		if (activeNumberOfObjectsToUseMenu) {
			if (!numberOfObjectsToUseMenu.activeSelf || numberOfObjectsToUseMenuRectTransform.anchoredPosition != menuPosition.anchoredPosition) {
				return;
			}
		}
		useCurrentObject ();
	}

	public void checkCurrentUseInventoryObject ()
	{
		if (currentUseInventoryObject) {
			currentUseInventoryObject.selectObjectOnInventory ();
		}
	}

	public void useCurrentObject ()
	{
		bool objectFound = false;
		if (isCurrentObjectNotNull ()) {
			if (currentInventoryObject.canBeUsed) {
				if (currentUseInventoryGameObject) {
					//currentUseInventoryObject = currentUseInventoryGameObject.GetComponent<useInventoryObject> ();
					if (currentUseInventoryObject.inventoryObjectNeededListContainsObject (currentInventoryObject.inventoryGameObject)) {
						bool amountNeededAvaliable = false;

						int amountToUse = numberOfObjectsToUse;
						int amountNeededForInventoryObject = currentUseInventoryObject.getInventoryObjectNeededAmound (currentInventoryObject.inventoryGameObject);
						if (useOnlyWhenNeededAmountToUseObject) {
							amountToUse = amountNeededForInventoryObject;
						} else {
							if (amountToUse > amountNeededForInventoryObject) {
								amountToUse = amountNeededForInventoryObject;
							}
						}

						if (amountToUse > 1) {
							if (currentInventoryObject.amount >= amountToUse) {
								amountNeededAvaliable = true;
							}
						} else {
							amountNeededAvaliable = true;
						}

						if (amountNeededAvaliable) {
							int amountUsed = amountToUse;
							if (currentInventoryObject.amountPerUnit > 0) {
								amountUsed *= currentInventoryObject.amountPerUnit;
							}
							currentUseInventoryObject.useObject (amountUsed);
							objectFound = true;
							currentInventoryObject.amount -= amountToUse;
							updateAmount (currentInventoryObject.menuIconElement.amount, currentInventoryObject.amount);
							if (currentInventoryObject.amount == 0) {
								removeButton (currentInventoryObject);
							}

							string objectUsedMessage = currentUseInventoryObject.getObjectUsedMessage ();

							if (objectUsedMessage != "") {
								showObjectMessage (objectUsedMessage, usedObjectMessageTime, usedObjectMessage);
							}

							if (inventoryOpened) {
								openOrCloseInventory (false);
							}

							setInventory (false);

							checkEmptySlots ();
						} else {
							showObjectMessage (nonNeededAmountAvaliable, usedObjectMessageTime, usedObjectMessage);
						}

						currentInventoryObject = null;

						//check the state of the use inventory object to load the new inventory elements to use if there are elements to use yet
						if (currentUseInventoryObject) {
							if (currentUseInventoryObject.objectUsed) {
								removeCurrentUseInventoryObject ();
							} else {
								currentUseInventoryObject.updateUseInventoryObjectState ();
							}
						}
					}
					if (!objectFound) {
						showObjectMessage (currentInventoryObject.Name + " " + unableToUseObjectMessage, usedObjectMessageTime, usedObjectMessage);
					}
				} else {
					showObjectMessage (cantUseThisObjectHereMessage, usedObjectMessageTime, usedObjectMessage);
				}
			}
		} else {
			showObjectMessage (objectNotFoundMessage, usedObjectMessageTime, usedObjectMessage);
		}
	}

	public bool isCurrentObjectNotNull ()
	{
		if (currentInventoryObject != null) {
			if (currentInventoryObject.inventoryGameObject == null) {
				return false;
			}
		} else {
			return false;
		}
		return true;
	}

	public void showObjectMessage (string message, float messageDuration, GameObject messagePanel)
	{
		if (objectMessageCoroutine != null) {
			StopCoroutine (objectMessageCoroutine);
		}
		objectMessageCoroutine = StartCoroutine (showObjectMessageCoroutine (message, messageDuration, messagePanel));
	}

	IEnumerator showObjectMessageCoroutine (string info, float messageDuration, GameObject messagePanel)
	{
		usingDevicesManager.checkDeviceName ();
		if (previousMessagePanel) {
			previousMessagePanel.SetActive (false);
		}
		messagePanel.SetActive (true);
		previousMessagePanel = messagePanel;
		messagePanel.GetComponentInChildren<Text> ().text = info;
		yield return new WaitForSeconds (messageDuration);
		messagePanel.SetActive (false);
	}

	public void equipCurrentObject ()
	{
		if (isCurrentObjectNotNull ()) {
			if (currentInventoryObject.canBeEquiped) {
				bool equippedCorrectly = false;

				if (currentInventoryObject.isWeapon) {
					currentIKWeaponSystem = weaponsManager.equipWeapon (currentInventoryObject.Name, initiatingInventory, activatingDualWeaponSlot, currentRighWeaponName, currentLeftWeaponName);

					if (currentIKWeaponSystem != null) {
						equippedCorrectly = true;
						print (currentIKWeaponSystem.getWeaponSystemName () + " equipped " + activatingDualWeaponSlot);
					} else {
						print ("WARNING: weapon " + currentInventoryObject.Name + " not found in the player weapons manager when player has tried to equip it from the inventory, " +
						"make sure the name of the weapon is the same on inventory as well");
					}
				}

				//here check for armor and any other kind of inventory object
				///////////////////////////
				///////////////////////////

				if (equippedCorrectly) {
					if (currentInventoryObject.menuIconElement == null) {
						bool inventoryObjectFound = false;
						for (int i = 0; i < inventoryList.Count; i++) {
							if (!inventoryObjectFound && inventoryList [i].Name == currentInventoryObject.Name) {
								currentInventoryObject = inventoryList [i];
								inventoryObjectFound = true;
							}
						}

						if (!inventoryObjectFound) {
							print ("WARNING: inventory object called " + currentInventoryObject.Name + " not found, please check the inventory manager" +
							" settings to assign a proper name to this inventory object");
						}
					} 

					currentInventoryObject.menuIconElement.equipedIcon.SetActive (true);

					currentInventoryObject.isEquiped = true;

					if (currentInventoryObject.isWeapon) {
						if (currentIKWeaponSystem.weaponGameObject) {
							currentInventoryObject.IKWeaponManager = currentIKWeaponSystem;

							//update the content in the weapon slot list, according to the weapons equipped
							if (storePickedWeaponsOnInventory) {
								bool slotAlreadyAdded = false;
								for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
									if (secondaryWeaponSlotInfoList [i].slotActive && secondaryWeaponSlotInfoList [i].Name == currentInventoryObject.Name) {
										slotAlreadyAdded = true;
									}
								}

								if (!slotAlreadyAdded) {
									bool secondaryWeaponSlotFound = false;
									for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
										if (!secondaryWeaponSlotInfoList [i].slotActive && !secondaryWeaponSlotFound && secondaryWeaponSlotInfoList [i].Name != currentInventoryObject.Name) {
											updateWeaponSlotInfo (i, currentInventoryObject, null, null);

											secondaryWeaponSlotFound = true;
										}
									}
								}
							}
						}
					}

					setEquipButtonState (false);
				}
			}
		}
	}

	public void unEquipCurrentObject ()
	{
		if (isCurrentObjectNotNull ()) {
			if (currentInventoryObject.canBeEquiped) {
				bool unequippedCorrectly = false;
				if (currentInventoryObject.isWeapon) {
					if (currentInventoryObject.isWeapon) {
						weaponsManager.unequipWeapon (currentInventoryObject.Name, false);
					}
					unequippedCorrectly = true;

					if (storePickedWeaponsOnInventory) {
						bool secondaryWeaponSlotFound = false;
						for (int i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
							if (secondaryWeaponSlotInfoList [i].slotActive && secondaryWeaponSlotInfoList [i].Name == currentInventoryObject.Name && !secondaryWeaponSlotFound) {
								updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [i], null);

								secondaryWeaponSlotFound = true;
							}
						}
					}
				}

				//here check for armor and any other kind of inventory object

				if (unequippedCorrectly) {
					currentInventoryObject.menuIconElement.equipedIcon.SetActive (false);

					currentInventoryObject.isEquiped = false;

					setEquipButtonState (true);
				}
			}
		}
	}

	public void unEquipObjectByName (string objectToUnequipName)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].canBeEquiped && inventoryList [i].isEquiped) {
				if (inventoryList [i].Name == objectToUnequipName) {
					print ("unequip " + objectToUnequipName);

					inventoryInfo inventoryObjectToUnequip = inventoryList [i];
					inventoryObjectToUnequip.menuIconElement.equipedIcon.SetActive (false);
					inventoryObjectToUnequip.isEquiped = false;

					setEquipButtonState (true);

					if (storePickedWeaponsOnInventory) {
						bool secondaryWeaponSlotFound = false;
						for (i = 0; i < secondaryWeaponSlotInfoList.Count; i++) {
							if (secondaryWeaponSlotInfoList [i].slotActive && secondaryWeaponSlotInfoList [i].Name == inventoryObjectToUnequip.Name && !secondaryWeaponSlotFound) {
								updateWeaponSlotInfo (-1, null, secondaryWeaponSlotInfoList [i], null);

								secondaryWeaponSlotFound = true;
							}
						}
					}

					return;
				}
			}
		}
	}

	public void dropAllUnitsObjectAtOnce ()
	{
		numberOfObjectsToUse = currentInventoryObject.amount;

		dropCurrentObject (true);
	}

	public void discardCurrentObject ()
	{
		numberOfObjectsToUse = currentInventoryObject.amount;

		dropCurrentObject (false);
	}

	public void dropCurrentObject (RectTransform menuPosition)
	{
		bool enabledPreviously = false;
		if (activeNumberOfObjectsToUseMenu && (currentInventoryObject == null || currentInventoryObject.amount > 1)) {
			if (numberOfObjectsToUseMenu.activeSelf && numberOfObjectsToUseMenuRectTransform.anchoredPosition == menuPosition.anchoredPosition) {
				enabledPreviously = true;
			}

			if (!enabledPreviously) {
				enableNumberOfObjectsToUseMenu (menuPosition);
				return;
			}
		}

		dropCurrentObject (true);

		if (enabledPreviously) {
			disableNumberOfObjectsToUseMenu ();
		}
	}

	public void dropCurrentObject (bool instantiateInventoryObjectPrefab)
	{
		//print ("drop " + currentInventoryObject.Name + " " + currentInventoryObject.amount);
		if (currentInventoryObject != null && currentInventoryObject.amount > 0) {

			currentInventoryObject.amount -= numberOfObjectsToUse;

			GameObject currentInventoryObjectPrefab = emptyInventoryPrefab;

			for (int i = 0; i < mainInventoryListManager.inventoryList.Count; i++) {
				if (mainInventoryListManager.inventoryList [i].inventoryGameObject == currentInventoryObject.inventoryGameObject) {
					currentInventoryObjectPrefab = mainInventoryListManager.inventoryList [i].inventoryObjectPrefab;
				}
			}

			Vector3 positionToInstantiate = transform.position + transform.forward + transform.up;

			if (instantiateInventoryObjectPrefab) {
				if (setTotalAmountWhenDropObject) {
					dropObject (currentInventoryObjectPrefab, positionToInstantiate, numberOfObjectsToUse);
				} else {
					for (int i = 0; i < numberOfObjectsToUse; i++) {
						dropObject (currentInventoryObjectPrefab, positionToInstantiate, 1);
					}
				}
			}

			if (currentInventoryObject.amount > 0) {
			
				updateAmount (currentInventoryObject.menuIconElement.amount, currentInventoryObject.amount);
				if (combineElementsAtDrop && !infiniteAmountPerSlot) {
					//combine same objects when the amount of an object is lower than amountPerSlot and there is another group equal to that object
					//for example if I have 10 cubes and 4 cubes with a amountPerSlot of 10, and you drop 1 cube of the first group, this combines the other cubes
					//so after that, you have 9 cubes and 4 cubes, and then, this changes that into 10 cubes and 3 cubes
					//this only checks the next objects after current object
					int currentIndex = inventoryList.IndexOf (currentInventoryObject) + 1;
					int index = -1;
					for (int i = currentIndex; i < inventoryList.Count; i++) {
						if (inventoryList [i].Name == currentInventoryObject.Name) {
							if (inventoryList [i].amount < amountPerSlot && index == -1) {
								index = i;
							}
						}
					}
					//if there are more objects equals to the current object dropped, then check their remaining amount to combine their values
					if (index != -1) {
						int amountToChange = amountPerSlot - currentInventoryObject.amount;
						if (amountToChange < inventoryList [index].amount) {
							inventoryList [index].amount -= amountToChange;
							currentInventoryObject.amount += amountToChange;
						} else if (amountToChange >= inventoryList [index].amount) {
							currentInventoryObject.amount += inventoryList [index].amount;
							inventoryList [index].amount -= inventoryList [index].amount;
							removeButton (inventoryList [index]);
						}
					} else {
						//if  all the objects equal to this has the max amount per space, search the last one of them to drop an object
						//from its amount
						currentIndex = inventoryList.IndexOf (currentInventoryObject);
						for (int i = inventoryList.Count - 1; i >= currentIndex; i--) {
							if (inventoryList [i].Name == currentInventoryObject.Name) {
								//|| i == inventoryList.Count - 1
								if ((inventoryList [i].amount == amountPerSlot) && index == -1) {
									index = i;
								}
							}
						}
						if (index != -1) {
							int amountToChange = amountPerSlot - currentInventoryObject.amount;
							if (amountToChange < inventoryList [index].amount) {
								inventoryList [index].amount -= amountToChange;
								currentInventoryObject.amount += amountToChange;
							} else if (amountToChange >= inventoryList [index].amount) {
								currentInventoryObject.amount += inventoryList [index].amount;
								inventoryList [index].amount -= inventoryList [index].amount;
								removeButton (inventoryList [index]);
							}
						}
					}
					updateAmountsInventoryPanel ();
				}
			} else {
				if (currentInventoryObject.canBeEquiped && currentInventoryObject.isEquiped) {
					if (currentInventoryObject.isWeapon) {
						print (currentInventoryObject.Name);

						weaponsManager.unequipWeapon (currentInventoryObject.Name, false);

						currentInventoryObject.isEquiped = false;
						currentInventoryObject.menuIconElement.equipedIcon.SetActive (false);
					}

				}
				removeButton (currentInventoryObject);
			}

			setInventory (false);

			checkEmptySlots ();

			resetAndDisableNumberOfObjectsToUseMenu ();

			checkCurrentUseInventoryObject ();
		}
	}

	public void showWeaponSlotsParentWhenWeaponSelected (int weaponSlotIndex)
	{
		if (showWeaponSlotsWhenChangingWepaons) {
			stopShowWeaponSlotsParentWhenWeaponSelectedCoroutuine ();

			if (!inventoryOpened) {
				weaponSlotsParentCouroutine = StartCoroutine (showWeaponSlotsParentWhenWeaponSelectedCoroutuine (weaponSlotIndex));
			}
		} 
	}

	public void stopShowWeaponSlotsParentWhenWeaponSelectedCoroutuine ()
	{
		if (weaponSlotsParentCouroutine != null) {
			StopCoroutine (weaponSlotsParentCouroutine);
		}
	}

	IEnumerator showWeaponSlotsParentWhenWeaponSelectedCoroutuine (int weaponSlotIndex)
	{
		moveWeaponSlotsOutOfInventory ();

		slotWeaponSelectedIcon.SetActive (true);

		slotWeaponSelectedIcon.transform.localScale = Vector3.one * weaponSlotsParentScale;

		updateWeaponCurrentlySelectedIcon (weaponSlotIndex, true);

		yield return new WaitForSeconds (0.001f);

		slotWeaponSelectedIcon.transform.position = secondaryWeaponSlotInfoList [weaponSlotIndex - 1].weaponSlotSelectedIconPosition.position;

		yield return new WaitForSeconds (showWepaonSlotsParentDuration);

		if (!showWeaponSlotsAlways) {
			moveWeaponSlotsToInventory ();
		}

		slotWeaponSelectedIcon.SetActive (false);
	}

	public void updateWeaponCurrentlySelectedIcon (int weaponSlotIndex, bool activeCurrentWeaponSlot)
	{
		bool weaponsAvailable = weaponsManager.checkIfWeaponsAvailable ();
		for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
			if (activeCurrentWeaponSlot && weaponsAvailable) {
				if (j == weaponSlotIndex - 1) {
					secondaryWeaponSlotInfoList [j].weaponCurrentlySelectedIcon.SetActive (true);
				} else {
					secondaryWeaponSlotInfoList [j].weaponCurrentlySelectedIcon.SetActive (false);
				}
			} else {
				secondaryWeaponSlotInfoList [j].weaponCurrentlySelectedIcon.SetActive (false);
			}
		}
	}

	public void disableCurrentlySelectedIcon ()
	{
		for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
			secondaryWeaponSlotInfoList [j].weaponCurrentlySelectedIcon.SetActive (false);
		}
	}

	public void selectWeaponByPressingSlotWeapon (GameObject buttonToCheck)
	{
		if (inventoryOpened) {
			return;
		}

		for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
			if (secondaryWeaponSlotInfoList [j].slot == buttonToCheck) {
				weaponsManager.changeCurrentWeaponByName (secondaryWeaponSlotInfoList [j].Name);
			}
		}
	}

	public void moveWeaponSlotsOutOfInventory ()
	{
		inventoryWeaponsSlots.transform.SetParent (weaponSlotsParentOutOfInventory);
		inventoryWeaponsSlots.transform.localScale = Vector3.one * weaponSlotsParentScale;
		inventoryWeaponsSlots.transform.localPosition = Vector3.zero;

		setWeaponSlotsBackgroundColorAlphaValue (weaponSlotsAlphaValueOutOfInventory);
	}

	public void moveWeaponSlotsToInventory ()
	{
		inventoryWeaponsSlots.transform.SetParent (weaponSlotsParentOnInventory);

		inventoryWeaponsSlots.transform.localScale = Vector3.one;
		inventoryWeaponsSlots.transform.localPosition = Vector3.zero;

		slotWeaponSelectedIcon.SetActive (false);

		setWeaponSlotsBackgroundColorAlphaValue (1);
	}

	public void setWeaponSlotsBackgroundColorAlphaValue (float newAlphaValue)
	{
		if (!setWeaponSlotsAlphaValueOutOfInventory) {
			return;
		}

		for (int j = 0; j < secondaryWeaponSlotInfoList.Count; j++) {
			Color currentColor = secondaryWeaponSlotInfoList [j].backgroundImage.color;
			currentColor.a = newAlphaValue;
			secondaryWeaponSlotInfoList [j].backgroundImage.color = currentColor;
		}
	}

	public void enableOrDisableWeaponSlotsParentOutOfInventory (bool state)
	{
		weaponSlotsParentOutOfInventory.gameObject.SetActive (state);
	}

	public void checkToEnableOrDisableWeaponSlotsParentOutOfInventory (bool state)
	{
		if (showWeaponSlotsAlways) {
			weaponSlotsParentOutOfInventory.gameObject.SetActive (state);
		}
	}

	public void dropObject (GameObject currentInventoryObjectPrefab, Vector3 positionToInstantiate, int amount)
	{
		if (numberOfObjectsToUse > 1) {
			positionToInstantiate = transform.position + transform.forward + transform.up + UnityEngine.Random.insideUnitSphere * maxRadiusToInstantiate;
		}

		GameObject inventoryObjectClone = (GameObject)Instantiate (currentInventoryObjectPrefab, positionToInstantiate, Quaternion.identity);

		inventoryObject inventoryObjectManager = inventoryObjectClone.GetComponentInChildren<inventoryObject> ();
		if (inventoryObjectManager) {
			inventoryObjectManager.inventoryObjectInfo = new inventoryInfo (currentInventoryObject);

			pickUpObject currentPickupObject = inventoryObjectClone.GetComponent<pickUpObject> ();
			currentPickupObject.amountPerUnit = currentInventoryObject.amountPerUnit;
			if (currentPickupObject.amountPerUnit > 0) {
				currentPickupObject.useAmountPerUnit = true;
			}
			currentPickupObject.setPickUpAmount (amount);
			inventoryObjectClone.name = inventoryObjectManager.inventoryObjectInfo.Name + " (inventory)";
			inventoryObjectClone.GetComponentInChildren<deviceStringAction> ().deviceName = inventoryObjectManager.inventoryObjectInfo.Name;
		}
	}

	public void dropEquipByName (string equipName, int amount)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].canBeEquiped && inventoryList [i].isEquiped) {
				if (inventoryList [i].Name == equipName) {
					currentInventoryObject = inventoryList [i];
				}
			}
		}
		numberOfObjectsToUse = amount;
		dropCurrentObject (false);
	}

	public void disableObjectsToCombineIcon ()
	{
		if (firstObjectToCombine != null && firstObjectToCombine.inventoryGameObject != null) {
			setCombineIcontPressedState (false, firstObjectToCombine);
		}
		if (secondObjectToCombine != null && secondObjectToCombine.inventoryGameObject != null) {
			setCombineIcontPressedState (false, secondObjectToCombine);
		}
	}

	public void setCombineIcontPressedState (bool state, inventoryInfo inventoryObjectToCombine)
	{
		if (inventoryObjectToCombine != null && (inventoryObjectToCombine.inventoryGameObject != null || !state)) {
			if (inventoryObjectToCombine.menuIconElement != null) {
				inventoryObjectToCombine.menuIconElement.combineIcon.SetActive (state);
			}
		}
	}

	public void setCombiningObjectsState (bool state)
	{
		if (isCurrentObjectNotNull ()) {
			if (currentInventoryObject.canBeCombined) {
				combiningObjects = state;
				setCombineIcontPressedState (state, secondObjectToCombine);
			} 
		}
	}

	public void combineCurrentObject ()
	{
		combiningObjects = false;
		if (firstObjectToCombine != null && secondObjectToCombine != null) {
			if (firstObjectToCombine.inventoryGameObject != null && secondObjectToCombine.inventoryGameObject != null) {
				bool canBeCombined = false;
				inventoryInfo combinedInventoryObject = new inventoryInfo ();

				if (firstObjectToCombine.canBeCombined && secondObjectToCombine.canBeCombined) {
					if (firstObjectToCombine.objectToCombine == secondObjectToCombine.inventoryGameObject ||
					    firstObjectToCombine.inventoryGameObject == secondObjectToCombine.objectToCombine) {
						canBeCombined = true;
						combinedInventoryObject = firstObjectToCombine.combinedObject.GetComponentInChildren<inventoryObject> ().inventoryObjectInfo;
					}
				}

				if (canBeCombined) {
					bool enoughSpace = false;
					if (infiniteSlots) {
						enoughSpace = true;
					} else {
						int amountFromFirstObject = firstObjectToCombine.amount;
						int amountFromSecondObject = secondObjectToCombine.amount;
						if (amountFromFirstObject == 1 || amountFromSecondObject == 1) {
							enoughSpace = true;
						} else {
							if (getNumberOfFreeSlots () >= 1) {
								enoughSpace = true;
							}
						}
					}

					if (enoughSpace) {
						showObjectMessage (secondObjectToCombine.combinedObjectMessage, combineObjectMessageTime, combinedObjectMessage);

						setCombineIcontPressedState (false, firstObjectToCombine);

						firstObjectToCombine.amount -= 1;
						updateAmount (firstObjectToCombine.menuIconElement.amount, firstObjectToCombine.amount);
						if (firstObjectToCombine.amount == 0) {
							removeButton (firstObjectToCombine);
						}

						secondObjectToCombine.amount -= 1;
						updateAmount (secondObjectToCombine.menuIconElement.amount, secondObjectToCombine.amount);
						if (secondObjectToCombine.amount == 0) {
							removeButton (secondObjectToCombine);
						}
							
						addObjectToInventory (combinedInventoryObject, 1, -1);

						firstObjectToCombine = null;

						secondObjectToCombine = null;

						bool iconPressed = false;
						//set as current inventory object the combined element
						for (int i = 0; i < inventoryList.Count; i++) {
							if (!iconPressed && inventoryList [i].inventoryGameObject == combinedInventoryObject.inventoryGameObject) {
								getPressedButton (inventoryList [i].button);
								iconPressed = true;
							}
						}
					} else {
						showObjectMessage (notEnoughSpaceToCombineMessage, combineObjectMessageTime, combinedObjectMessage);
					}
				} else {
					showObjectMessage (unableToCombineMessage, combineObjectMessageTime, combinedObjectMessage);
					disableObjectsToCombineIcon ();

					firstObjectToCombine = null;
					secondObjectToCombine = null;
				}
				checkCurrentUseInventoryObject ();
			}
		}
	}

	public void setCurrentPickupObject (pickUpObject newPickupObject)
	{
		currentPickupObject = newPickupObject;
	}

	public void examineCurrentPickupObject (inventoryInfo inventoryObjectToPickup)
	{
		openOrCloseInventory (true);
		currentInventoryObject = new inventoryInfo (inventoryObjectToPickup);

		setObjectInfo (currentInventoryObject);

		examineCurrentObject (true);
		takeObjectInExaminePanelButton.SetActive (true);
	}

	public void confirmToPickCurrentObjectInExaminingPanel ()
	{
		currentPickupObject.pickObjectByButton ();
		examineCurrentObject (false);
	}

	public void examineCurrentObject (bool state)
	{
		if (isCurrentObjectNotNull ()) {
			examiningObject = state;
			examineObjectPanel.SetActive (examiningObject);

			examineObjectName.text = currentInventoryObject.Name;
			examineObjectDescription.text = currentInventoryObject.objectInfo;

			if (examiningObject) {
				placeObjectInCameraPosition ();
				placeObjectInCameraRotation ();

				float currentCameraFov = originalFov;
				if (currentInventoryObjectManager) {
					if (currentInventoryObjectManager.useZoomRange) {
						currentCameraFov = currentInventoryObjectManager.initialZoom;
					}
				}
				currentCameraFov += extraCameraFovOnExamineObjects;
				inventoryCamera.fieldOfView = currentCameraFov;

				pauseManager.openOrClosePlayerMenu (inventoryOpened, examineObjectPanel.transform, useBlurUIPanel);

			} else {
				stopObjectInCameraPositionMovevement ();
				stopObjectInCameraRotationMovevement ();

				float currentCameraFov = originalFov;
				if (currentInventoryObjectManager) {
					if (currentInventoryObjectManager.useZoomRange) {
						currentCameraFov = currentInventoryObjectManager.initialZoom;
					}
				}
				inventoryCamera.fieldOfView = currentCameraFov;
				objectInCamera.transform.localPosition = Vector3.zero;
				objectInCamera.transform.localRotation = Quaternion.identity;

				pauseManager.openOrClosePlayerMenu (inventoryOpened, inventoryPanel.transform, useBlurUIPanel);

				if (currentPickupObject != null) {
					openOrCloseInventory (false);
					currentPickupObject = null;
					currentInventoryObject = null;
				}
			}

			takeObjectInExaminePanelButton.SetActive (false);
		}
	}

	public bool playerIsExaminingInventoryObject ()
	{
		return examiningObject;
	}

	public void updateAmountsInventoryPanel ()
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			updateAmount (inventoryList [i].menuIconElement.amount, inventoryList [i].amount);
		}
	}

	public void updateAmount (Text textAmount, int amount)
	{
		textAmount.text = amount.ToString ();
	}

	public void removeButton (inventoryInfo currentObj)
	{
		disableCurrentObjectInfo ();

		currentObj.resetInventoryInfo ();

		enableRotation = false;
		destroyObjectInCamera ();
		setMenuIconElementPressedState (false);
		currentInventoryObject = null;

		currentInventoryObjectManager = null;
	}

	public void setMenuIconElementPressedState (bool state)
	{
		if (currentInventoryObject != null) {
			if (currentInventoryObject.menuIconElement != null) {
				currentInventoryObject.menuIconElement.pressedIcon.SetActive (state);
			}
		}
	}

	public void setObjectInfo (inventoryInfo currentInventoryObjectInfo)
	{
		resetAndDisableNumberOfObjectsToUseMenu ();
		setMenuIconElementPressedState (false);

		if (isCurrentObjectNotNull ()) {
			firstObjectToCombine = currentInventoryObject;
		}

		currentInventoryObject = currentInventoryObjectInfo;

		GameObject currentInventoryObjectPrefab = emptyInventoryPrefab;

		for (int i = 0; i < mainInventoryListManager.inventoryList.Count; i++) {
			if (mainInventoryListManager.inventoryList [i].inventoryGameObject == currentInventoryObject.inventoryGameObject) {
				currentInventoryObjectPrefab = mainInventoryListManager.inventoryList [i].inventoryObjectPrefab;
			}
		}

		currentInventoryObjectManager = currentInventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();

		secondObjectToCombine = currentInventoryObject;

		setMenuIconElementPressedState (true);
		currentObjectName.text = currentInventoryObject.Name;
		currentObjectInfo.text = currentInventoryObject.objectInfo;
		inventoryObjectInforScrollbar.value = 1;

		if (currentInventoryObject.canBeUsed) {
			useButtonImage.color = buttonUsable;
		} else {
			useButtonImage.color = buttonNotUsable;
		}

		if (currentInventoryObject.canBeEquiped) {
			if (currentInventoryObject.isEquiped) {
				setEquipButtonState (false);
			} else {
				setEquipButtonState (true);
			}
		} else {
			equipButtonImage.color = buttonNotUsable;
			unequipButtonImage.color = buttonNotUsable;

			equipButton.SetActive (true);
			unequipButton.SetActive (false);
		}

		if (currentInventoryObject.canBeDropped) {
			dropButtonImage.color = buttonUsable;
			dropAllUnitsObjectButtonImage.color = buttonUsable;
		} else {
			dropButtonImage.color = buttonNotUsable;
			dropAllUnitsObjectButtonImage.color = buttonUsable;
		}

		if (currentInventoryObject.canBeDiscarded) {
			discardButtonImage.color = buttonUsable;
		} else {
			discardButtonImage.color = buttonNotUsable;
		}

		if (currentInventoryObject.canBeCombined) {
			combineButtonImage.color = buttonUsable;
		} else {
			combineButtonImage.color = buttonNotUsable;
		}

		examineButtonImage.color = buttonUsable;

		objectImage.enabled = true;
		destroyObjectInCamera ();
		if (currentInventoryObject.inventoryGameObject) {
			objectInCamera = (GameObject)Instantiate (currentInventoryObject.inventoryGameObject, lookObjectsPosition.transform.position, Quaternion.identity);
			objectInCamera.transform.SetParent (lookObjectsPosition);
		}

		if (combiningObjects) {
			combineCurrentObject ();
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

	public void placeObjectInCameraPosition ()
	{
		stopObjectInCameraPositionMovevement ();
		objectInCameraPositionCoroutine = StartCoroutine (placeObjectInCameraPositionCoroutine ());
	}

	public void stopObjectInCameraPositionMovevement ()
	{
		if (objectInCameraPositionCoroutine != null) {
			StopCoroutine (objectInCameraPositionCoroutine);
		}
	}

	IEnumerator placeObjectInCameraPositionCoroutine ()
	{
		if (objectInCamera) {
			objectInCamera.transform.localPosition = lookObjectsPosition.forward * distanceToPlaceObjectInCamera;
			Vector3 targetPosition = Vector3.zero;
			while (objectInCamera.transform.localPosition != targetPosition) {
				objectInCamera.transform.localPosition = Vector3.MoveTowards (objectInCamera.transform.localPosition, targetPosition, Time.deltaTime * placeObjectInCameraSpeed);
				yield return null;
			}
		}
	}

	public void placeObjectInCameraRotation ()
	{
		stopObjectInCameraRotationMovevement ();
		objectInCameraRotationCoroutine = StartCoroutine (placeObjectInCameraRotationCoroutine ());
	}

	public void stopObjectInCameraRotationMovevement ()
	{
		if (objectInCameraRotationCoroutine != null) {
			StopCoroutine (objectInCameraRotationCoroutine);
		}
	}

	IEnumerator placeObjectInCameraRotationCoroutine ()
	{
		if (objectInCamera) {
			objectInCamera.transform.localRotation = Quaternion.identity;
			Vector3 targetRotation = new Vector3 (0, 360 * numberOfRotationsObjectInCamera, 0);

			float currentLerpTime = 0;
			float lerpTime = placeObjectInCameraRotationSpeed;
			while (targetRotation != Vector3.zero) {
				currentLerpTime += Time.deltaTime;
				if (currentLerpTime > lerpTime) {
					currentLerpTime = lerpTime;
				}
					
				float perc = currentLerpTime / lerpTime;
				targetRotation = Vector3.Lerp (targetRotation, Vector3.zero, perc);

				objectInCamera.transform.localEulerAngles = targetRotation;
				yield return null;
			}
		}
	}

	public void openOrCloseInventory (bool state)
	{
		if ((!pauseManager.playerMenuActive || inventoryOpened) && !playerControllerManager.usingDevice && !mainGameManager.isGamePaused ()) {
			inventoryOpened = state;

			setMenuIconElementPressedState (false);

			disableObjectsToCombineIcon ();

			pauseManager.openOrClosePlayerMenu (inventoryOpened, inventoryPanel.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState ("Inventory Manager", inventoryOpened);

			inventoryPanel.SetActive (inventoryOpened);

			//set to visible the cursor
			pauseManager.showOrHideCursor (inventoryOpened);
			//disable the touch controls

			pauseManager.checkTouchControls (!inventoryOpened);

			//disable the camera rotation
			pauseManager.changeCameraState (!inventoryOpened);
			playerControllerManager.changeScriptState (!inventoryOpened);
			pauseManager.usingSubMenuState (inventoryOpened);

			pauseManager.enableOrDisableDynamicElementsOnScreen (!inventoryOpened);

			if (examiningObject) {
				stopObjectInCameraPositionMovevement ();
				stopObjectInCameraRotationMovevement ();
				examiningObject = false;
				examineObjectPanel.SetActive (false);
			}

			destroyObjectInCamera ();

			if (inventoryOpened) {
				inventorySlotsScrollbar.value = 1;
				inventoryObjectInforScrollbar.value = 1;

				if (showWeaponSlotsAlways || showWeaponSlotsWhenChangingWepaons) {

					stopShowWeaponSlotsParentWhenWeaponSelectedCoroutuine ();

					moveWeaponSlotsToInventory ();

					updateWeaponCurrentlySelectedIcon (-1, false);
				}
			} else {
				disableCurrentObjectInfo ();

				if (showWeaponSlotsAlways) {
					moveWeaponSlotsOutOfInventory ();

					weaponsManager.updateCurrentChoosedDualWeaponIndex ();

					updateWeaponCurrentlySelectedIcon (weaponsManager.chooseDualWeaponIndex, true);
				}
			}
				
			resetAndDisableNumberOfObjectsToUseMenu ();
			inventoryCamera.fieldOfView = originalFov;

			currentInventoryObject = null;

			currentInventoryObjectManager = null;

			if (currentUseInventoryObject) {
				currentUseInventoryObject.updateUseInventoryObjectState ();
			}
			inventoryCamera.enabled = inventoryOpened;

			unequipButton.SetActive (false);

			pauseManager.showOrHideMouseCursorController (inventoryOpened);
		}
	}

	public void openOrCLoseInventoryFromTouch ()
	{
		openOrCloseInventory (!inventoryOpened);
	}

	public void addNewInventoryObject ()
	{
		inventoryInfo newObject = new inventoryInfo ();
		inventoryList.Add (newObject);
		updateComponent ();
	}

	public bool isInventoryFull ()
	{
		if (infiniteSlots || infiniteAmountPerSlot) {
			return false;
		}
		bool isFull = true;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].amount < amountPerSlot) {
				isFull = false;
			}
		}
		return isFull;
	}

	public int getNumberOfFreeSlots ()
	{
		if (infiniteSlots) {
			return 1;
		}
		int numberOfSlots = 0;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].amount == 0) {
				numberOfSlots++;
			}
		}
		return numberOfSlots;
	}

	public int getRealAmountOfFreeSlots ()
	{
		int numberOfSlots = 0;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].amount == 0) {
				numberOfSlots++;
			}
		}
		return numberOfSlots;
	}

	public int freeSpaceInSlot (GameObject inventoryObjectMesh)
	{
		int freeSpaceAmount = -1;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == inventoryObjectMesh) {
				freeSpaceAmount = amountPerSlot - inventoryList [i].amount;
			}
		}
		return freeSpaceAmount;
	}

	public bool checkIfObjectCanBeStored (GameObject inventoryObjectMesh, int amountToStore)
	{
		if (infiniteSlots) {
			//if the inventory has infinite slots, it can be stored
			return true;
		}

		//get the number of free slots in the inventory
		int numberOfFreeSlots = getRealAmountOfFreeSlots ();

		//check if the object to store is already in the inventory, to obtain that amount
		int freeSpaceAmountOnSlot = freeSpaceInSlot (inventoryObjectMesh);

		bool hasSameObject = false;
		if (freeSpaceAmountOnSlot > 0) {
			hasSameObject = true;
		}

		//if the number of free slots is 0, then 
		if (numberOfFreeSlots == 0) {

			//if there is no at least an slot with the object to store, the object has not space to be stored
			if (!hasSameObject) {
				return false;
			} 

			//if there is at least an slot with the object to store, but amount to store is higher than the regular amount per slot, the object has not space to be stored
			else if (amountToStore > amountPerSlot) {
				return false;
			} 

			//if there is not enough free space in that slot, the object can't be stored
			else if (amountToStore > freeSpaceAmountOnSlot) {
				return false;
			}
		}

		//if there are free slots or the a same type of object already stored and infinite units can be stored in an slot, the object can be stored
		if (infiniteAmountPerSlot && (numberOfFreeSlots > 0 || hasSameObject)) {
			return true;
		}

		//get the total amount of units that can be stored in the inventory to compare it with the amount to store from the current object to get
		int freeUnits = numberOfFreeSlots * amountPerSlot + freeSpaceAmountOnSlot;

		if (freeUnits > amountToStore) {
			return true;
		}
			
		return false;
	}

	public void addAmountToInventorySlot (inventoryInfo objectInfo, int currentSlotAmount, int extraAmount)
	{
		bool amountAdded = false;
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == objectInfo.inventoryGameObject && (inventoryList [i].amount == currentSlotAmount || infiniteAmountPerSlot) && !amountAdded) {
				inventoryList [i].amount += extraAmount;
				inventoryList [i].amountPerUnit = objectInfo.amountPerUnit;
				updateAmount (inventoryList [i].menuIconElement.amount, inventoryList [i].amount);

				amountAdded = true;
			}
		}

		if (amountAdded) {
			//print ("object amount increased");
		} else {
			//print ("object not found on inventory, added new one");
			addObjectToInventory (objectInfo, extraAmount, -1);
		}
	}

	public void showInventoryFullMessage ()
	{
		if (inventoryFullCoroutine != null) {
			StopCoroutine (inventoryFullCoroutine);
		}
		inventoryFullCoroutine = StartCoroutine (showInventoryFullMessageCoroutine ());
	}

	IEnumerator showInventoryFullMessageCoroutine ()
	{
		fullInventoryMessage.SetActive (true);
		yield return new WaitForSeconds (fullInventoryMessageTime);
		fullInventoryMessage.SetActive (false);
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

	public void setCurrentInventoryBankSystem (GameObject inventoryBankGameObject)
	{
		bankManager.setCurrentInventoryBankSystem (inventoryBankGameObject);
		bankManager.setCurrentPlayer (gameObject);
	}

	public void openOrCloseInventoryBankMenu (bool state)
	{
		bankManager.openOrCloseInventoryBankMenu (state);
	}

	public void dropAllInventory ()
	{
		if (Application.isPlaying) {
	
			for (int i = 0; i < inventoryList.Count; i++) {
				bucle = 0;
				while (inventoryList [i].amount > 0) {
					currentInventoryObject = inventoryList [i];
					dropCurrentObject (true);
					bucle++;
					if (bucle > 100) {
						//print ("loop bucle");
						return;
					}
				}
			}
		}
	}

	public void getInventoryListManagerList ()
	{
		inventoryManagerListString = new string[mainInventoryListManager.inventoryList.Count];
		for (int i = 0; i < inventoryManagerListString.Length; i++) {
			inventoryManagerListString [i] = mainInventoryListManager.inventoryList [i].Name;
		}

		updateComponent ();
	}

	//functions used in the inventory bank manager
	public void setNewInventoryList (List<inventoryInfo> newInventoryList)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			inventoryList [i] = new inventoryInfo (newInventoryList [i]);
		}

		checkEmptySlots ();

		updateFullInventorySlots ();
	}

	public void moveObjectToBank (int objectToMoveIndex, int amountToMove)
	{
		bool amountNeededAvaliable = false;

		currentInventoryObject = inventoryList [objectToMoveIndex];

		print ("object moved to bank " + currentInventoryObject.Name);

		if (amountToMove > 1) {
			if (currentInventoryObject.amount >= amountToMove) {
				amountNeededAvaliable = true;
			}
		} else {
			amountNeededAvaliable = true;
		}

		if (amountNeededAvaliable) {
			print (amountToMove);

			if (currentInventoryObject.canBeEquiped) {
				if (currentInventoryObject.isEquiped) {
					if (currentInventoryObject.isWeapon) {
						unEquipCurrentObject ();
					}
				}
			}

			currentInventoryObject.amount -= amountToMove;

			print (currentInventoryObject.amount);
			updateAmount (currentInventoryObject.menuIconElement.amount, currentInventoryObject.amount);
			if (currentInventoryObject.amount == 0) {
				removeButton (currentInventoryObject);
			}
			setInventory (false);
			checkEmptySlots ();
		} else {
			showObjectMessage (nonNeededAmountAvaliable, usedObjectMessageTime, usedObjectMessage);
		}
		currentInventoryObject = null;
	}

	public persistanceInventoryListByPlayerInfo getPersistanceInventoryList ()
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();
		
		newPersistanceInventoryListByPlayerInfo.playerID = playerControllerManager.getPlayerID ();
		newPersistanceInventoryListByPlayerInfo.inventorySlotAmount = inventorySlotAmount;

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		for (int k = 0; k < inventoryList.Count; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();
			int inventoryListIndex = mainInventoryListManager.getInventoryIndexFromInventoryGameObject (inventoryList [k].inventoryGameObject);
			if (inventoryListIndex > -1) {
				newPersistanceInventoryObjectInfo.name = inventoryList [k].Name;
				newPersistanceInventoryObjectInfo.amount = inventoryList [k].amount;
				newPersistanceInventoryObjectInfo.infiniteAmount = inventoryList [k].infiniteAmount;
				newPersistanceInventoryObjectInfo.inventoryObjectName = mainInventoryListManager.inventoryList [k].Name;
				newPersistanceInventoryObjectInfo.elementIndex = inventoryListIndex;
				newPersistanceInventoryObjectInfo.isEquipped = inventoryList [k].isEquiped;

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
			
			gameSystemManager.savePlayerInventoryList (getPersistanceInventoryListElement (), gameSystemManager.saveNumberToLoad);
		} else {
			print ("Configure a valid save number in the Game Manager inspector, between 1 and infinite");
			return;
		}

		print ("Inventory list saved");
		updateComponent ();
	}

	public persistanceInventoryListByPlayerInfo getPersistanceInventoryListElement ()
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();

		newPersistanceInventoryListByPlayerInfo.playerID = playerControllerManager.getPlayerID ();
		newPersistanceInventoryListByPlayerInfo.inventorySlotAmount = inventorySlotAmount;

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		for (int k = 0; k < inventoryListManagerList.Count; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();
			newPersistanceInventoryObjectInfo.name = inventoryListManagerList [k].name;
			newPersistanceInventoryObjectInfo.amount = inventoryListManagerList [k].amount;
			newPersistanceInventoryObjectInfo.infiniteAmount = inventoryListManagerList [k].infiniteAmount;
			newPersistanceInventoryObjectInfo.inventoryObjectName = inventoryListManagerList [k].inventoryObjectName;
			newPersistanceInventoryObjectInfo.elementIndex = inventoryListManagerList [k].elementIndex;
			newPersistanceInventoryObjectInfo.isEquipped = inventoryListManagerList [k].isEquipped;

			print (newPersistanceInventoryObjectInfo.name + "" + newPersistanceInventoryObjectInfo.amount);
			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}

	public void inputOpenOrCloseInventory ()
	{
		if (inventoryEnabled) {
			openOrCloseInventory (!inventoryOpened);
		}
	}

	public void inputSetZoomInState (bool state)
	{
		if (inventoryEnabled && inventoryOpened) {
			if (state) {
				zoomInEnabled ();
			} else {
				zoomInDisabled ();
			}
		}
	}

	public void inputSetZoomOutState (bool state)
	{
		if (inventoryEnabled && inventoryOpened) {
			if (state) {
				zoomOutEnabled ();
			} else {
				zoomOutDisabled ();
			}
		}
	}

	public void setAddAllObjectEnabledState (bool state)
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			inventoryListManagerList [i].addInventoryObject = state;
		}

		updateComponent ();
	}

	public void setAddObjectEnabledState (int objectIndex)
	{
		inventoryListManagerList [objectIndex].addInventoryObject = !inventoryListManagerList [objectIndex].addInventoryObject;

		updateComponent ();
	}

	public void setEquippedObjectState (int objectIndex, bool state)
	{
		inventoryListManagerList [objectIndex].isEquipped = state;

		updateComponent ();
	}

	public void setEquippedObjectState (int objectIndex)
	{
		inventoryListManagerList [objectIndex].isEquipped = !inventoryListManagerList [objectIndex].isEquipped;

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<inventoryManager> ());
		#endif
	}

	[System.Serializable]
	public class secondaryWeaponSlotInfo
	{
		public string Name;
		public bool slotActive;
		public GameObject slot;
		public playerWeaponSystem weaponSystem;
		public Text weaponAmmoText;
		public RawImage weaponIcon;

		public playerWeaponSystem secondaryWeaponSystem;

		public RawImage dualRightWeaponIcon;
		public RawImage dualLeftWeaponIcon;

		public bool dualWeaponSlotActive;

		public string rightWeaponName;
		public string leftWeaponName;

		public Text iconNumberKeyText;
		public inventoryInfo weaponInventoryInfo;

		public Transform weaponSlotSelectedIconPosition;

		public GameObject weaponCurrentlySelectedIcon;

		public Image backgroundImage;

		public GameObject slotMainSingleContent;
		public GameObject slotMainDualContent;

		public GameObject weaponAmmoTextContent;
	}
}