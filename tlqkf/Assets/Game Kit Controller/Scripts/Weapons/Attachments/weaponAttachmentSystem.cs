using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class weaponAttachmentSystem : MonoBehaviour
{
	public GameObject playerControllerGameObject;
	public playerCamera playerCameraManager;
	public playerWeaponsManager weaponsManager;
	public playerWeaponSystem weaponSystem;
	public IKWeaponSystem IKWeaponManager;
	public menuPause pauseManager;
	public bool editingAttachments;

	public GameObject attachmentInfoGameObject;
	public GameObject attachmentSlotGameObject;

	public GameObject weaponAttachmentsMenu;

	public Camera weaponsCamera;

	public float thirdPersonCameraMovementSpeed = 10;

	public bool canChangeAttachmentWithNumberKeys = true;

	public AudioSource mainAudioSource;

	public AudioClip removeAttachmentSound;

	public bool useSmoothTransitionFreeCamera = true;

	public bool useSmoothTransitionLockedCamera = true;

	public bool useOffsetPanels;

	public bool canEditWeaponWithoutAttchments = true;

	public bool setPickedAttachments = true;

	public AudioClip startEditWeaponSound;
	public AudioClip stopEditWeaponSound;

	public List<attachmentPlace> attachmentInfoList = new List<attachmentPlace> ();

	public float UILinesScaleMultiplier = 0.1f;

	public float dualWeaponOffsetScale = 0.5f;

	public bool showElementSettings;

	public bool showGizmo;

	public bool showDualWeaponsGizmo;

	public bool disableHUDWhenEditingAttachments;

	public bool showCurrentAttachmentHoverInfo;

	public Transform attachmentHoverInfoPanelGameObject;
	public Text attachmentHoverInfoText;

	bool attachmentChecked;
	List<GameObject> attachmentIconGameObjectList = new List<GameObject> ();

	bool chechAttachmentPlaces;
	int currentAmountAttachments;

	playerController playerControllerManager;
	otherPowers powersManager;
	grabObjects grabObjectsManager;
	bool previouslyOnFirstPerson;
	bool currentlyOnFirstPerson;

	bool movingToFirstPerson;
	GameObject currentWeaponAttachmentsMenu;
	attachmentInfo currentAttachmentInfo;
	attachmentInfo currentHoverAttachmentInfo;

	bool setFirstPersonForAttachmentEditor;

	Vector3 mainCameraTargetPosition;
	Quaternion mainCameraTargetRotation;
	Transform mainCameraTransform;
	Camera mainCamera;
	Camera currentCamera;
	Coroutine mainCameraMovement;
	bool movingCamera;

	attachmentPlace currentAttachmentPlace;

	bool attachmentsActiveInWeapon;

	Transform cameraParentTransform;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	Vector3 screenPoint;

	Transform currentOffsetAttachmentPlaceTransform;

	public bool usingDualWeapon;

	weaponAttachmentSystem secondaryWeaponAttachmentSystem;

	bool touchPlatform;
	Touch currentTouch;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	void Start ()
	{
		playerControllerManager = playerControllerGameObject.GetComponent<playerController> ();
		powersManager = playerControllerGameObject.GetComponent<otherPowers> ();
		grabObjectsManager = playerControllerGameObject.GetComponent<grabObjects> ();
	
		mainCamera = playerCameraManager.getMainCamera ();
		mainCameraTransform = mainCamera.transform;

		IKWeaponManager.setWeaponAttachmentSystem (GetComponent<weaponAttachmentSystem> ());

		mainCanvasSizeDelta = pauseManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	public void instantiateAttachmentIcons ()
	{
		int attachmentActiveIndex = 0;
		if (attachmentIconGameObjectList.Count == 0) {
			currentWeaponAttachmentsMenu = (GameObject)Instantiate (weaponAttachmentsMenu, Vector3.zero, Quaternion.identity);
			currentWeaponAttachmentsMenu.transform.SetParent (weaponAttachmentsMenu.transform.parent);
			currentWeaponAttachmentsMenu.transform.localScale = Vector3.one;
			currentWeaponAttachmentsMenu.transform.localPosition = Vector3.zero;
			currentWeaponAttachmentsMenu.name = "Weapon Attachments Menu (" + weaponSystem.weaponSettings.Name + ")";

			for (int i = 0; i < attachmentInfoList.Count; i++) {
				GameObject newAttachmentIconGameObject = (GameObject)Instantiate (attachmentInfoGameObject, Vector3.zero, Quaternion.identity);

				newAttachmentIconGameObject.SetActive (true);
				newAttachmentIconGameObject.name = "Attachment Info (" + attachmentInfoList [i].Name + ")";
				newAttachmentIconGameObject.transform.SetParent (currentWeaponAttachmentsMenu.transform);
				newAttachmentIconGameObject.transform.localScale = Vector3.one;
				newAttachmentIconGameObject.transform.localPosition = Vector3.zero;

				attachmentIcon curentAttachmentIcon = newAttachmentIconGameObject.GetComponent<attachmentIconInfo> ().attachmentIconManager;
				attachmentInfoList [i].attachmentIconManager = curentAttachmentIcon;
				curentAttachmentIcon.attachmentNameText.text = attachmentInfoList [i].Name;
				curentAttachmentIcon.attachmentNumberText.text = (attachmentActiveIndex + 1).ToString ();

				attachmentActiveIndex++;

				attachmentIconGameObjectList.Add (newAttachmentIconGameObject);

				bool anyAttachmentActive = false;

				bool anyAttachmentEnabled = false;

				curentAttachmentIcon.attachmentContent.GetComponent<GridLayoutGroup> ().constraintCount = attachmentInfoList [i].attachmentPlaceInfoList.Count + 1;
				for (int j = 0; j < attachmentInfoList [i].attachmentPlaceInfoList.Count; j++) {
					currentAttachmentInfo = attachmentInfoList [i].attachmentPlaceInfoList [j];

					GameObject newAttachmentSlotGameObject = (GameObject)Instantiate (attachmentSlotGameObject, Vector3.zero, Quaternion.identity);

					newAttachmentSlotGameObject.SetActive (true);
					newAttachmentSlotGameObject.name = "Attachment Slot (" + currentAttachmentInfo.Name + ")";
					newAttachmentSlotGameObject.transform.SetParent (curentAttachmentIcon.attachmentContent);
					newAttachmentSlotGameObject.transform.localScale = Vector3.one;
					newAttachmentSlotGameObject.transform.localPosition = Vector3.zero;

					attachmentSlot currentAttachmentSlot = newAttachmentSlotGameObject.GetComponent<attachmentSlotInfo> ().attachmentSlotManager;
					currentAttachmentInfo.attachmentSlotManager = currentAttachmentSlot;
					currentAttachmentSlot.attachmentNameText.text = currentAttachmentInfo.Name.ToUpper ();

					currentAttachmentInfo.attachmentSlotManager.attachmentSelectedIcon.SetActive (currentAttachmentInfo.attachmentActive);

					if (!anyAttachmentActive) {
						anyAttachmentActive = currentAttachmentInfo.attachmentActive;
					}

					if (!anyAttachmentEnabled) {
						anyAttachmentEnabled = currentAttachmentInfo.attachmentEnabled;
					}

					if (!currentAttachmentInfo.attachmentEnabled) {
						newAttachmentSlotGameObject.SetActive (false);
					}
				}

				curentAttachmentIcon.notAttachmentButton.attachmentSelectedIcon.SetActive (!anyAttachmentActive);

				if (attachmentInfoList [i].noAttachmentText != "") {
					curentAttachmentIcon.notAttachmentButton.attachmentNameText.text = attachmentInfoList [i].noAttachmentText;
				}

				if (!attachmentInfoList [i].attachmentPlaceEnabled || !anyAttachmentEnabled) {
					newAttachmentIconGameObject.SetActive (false);
				}
			}
			currentWeaponAttachmentsMenu.SetActive (false);
			currentAmountAttachments = attachmentActiveIndex;
		} else {
			for (int i = 0; i < attachmentInfoList.Count; i++) {
				currentAttachmentPlace = attachmentInfoList [i];
				if (currentAttachmentPlace.attachmentPlaceEnabled) {
					if (currentAttachmentPlace.noAttachmentText != "") {
						currentAttachmentPlace.attachmentIconManager.notAttachmentButton.attachmentNameText.text = currentAttachmentPlace.noAttachmentText;
					}

					currentAttachmentPlace.attachmentIconManager.iconRectTransform.gameObject.SetActive (true);

					bool anyAttachmentActive = false;
					for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
						currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
						if (currentAttachmentInfo.attachmentActive) {
							currentAttachmentInfo.attachmentSlotManager.slotButton.gameObject.SetActive (true);
							anyAttachmentActive = true;
						}
					
						currentAttachmentInfo.attachmentSlotManager.attachmentSelectedIcon.SetActive (currentAttachmentInfo.attachmentActive);
					}
				
					currentAttachmentPlace.attachmentIconManager.notAttachmentButton.attachmentSelectedIcon.SetActive (!anyAttachmentActive);

					if (!attachmentsActiveInWeapon) {
						attachmentsActiveInWeapon = anyAttachmentActive;
					}
				}
			}

		}
	}

	void Update ()
	{
		if (!attachmentChecked) {
			setAttachmentInitialState ();
			attachmentChecked = true;
		}

		if (chechAttachmentPlaces) {
			if (!IKWeaponManager.moving) {
				if (currentlyOnFirstPerson || setFirstPersonForAttachmentEditor) {
					if (playerCameraManager.isCameraPlacedInFirstPerson ()) {
						if (!currentlyOnFirstPerson && !previouslyOnFirstPerson && !movingToFirstPerson) {
							IKWeaponManager.enableOrDisableEditAttachment (editingAttachments);
							weaponSystem.enableHUD (!editingAttachments);
							movingToFirstPerson = true;
							return;
						}

						activateAttachmentUIPanels ();

						chechAttachmentPlaces = false;

						pauseManager.changeCameraState (!editingAttachments);
					}
				} else {
					if (!movingCamera) {
						activateAttachmentUIPanels ();
						chechAttachmentPlaces = false;
					}
				}
			}
		}

		if (editingAttachments) {
			if (canChangeAttachmentWithNumberKeys) {
				for (int i = 0; i < currentAmountAttachments; i++) {
					if (Input.GetKeyDown ("" + (i + 1))) {
						if (i < attachmentInfoList.Count) {
							if (attachmentInfoList [i].attachmentPlaceEnabled) {
								int currentIndex = attachmentInfoList [i].currentAttachmentSelectedIndex;
								currentIndex++;

								if (currentIndex > attachmentInfoList [i].attachmentPlaceInfoList.Count) {
									currentIndex = 0;
								}

								if (currentIndex > 0) {
									if (!attachmentInfoList [i].attachmentPlaceInfoList [currentIndex - 1].attachmentEnabled) {
										int nextIndex = currentIndex;
										int loop = 0;
										int nextIndexToConfigure = -1;
										while (nextIndex < attachmentInfoList [i].attachmentPlaceInfoList.Count) {
											if (attachmentInfoList [i].attachmentPlaceInfoList [nextIndex].attachmentEnabled) {
												//print (attachmentInfoList [i].attachmentPlaceInfoList [nextIndex].Name);
												if (nextIndexToConfigure == -1) {
													nextIndexToConfigure = nextIndex;
												}
											}
											nextIndex++;
											if (loop > 1000) {
												print ("loop error");
												break;
											}
										}

										if (nextIndexToConfigure != -1) {
											currentIndex = nextIndexToConfigure + 1;
											if (currentIndex > attachmentInfoList [i].attachmentPlaceInfoList.Count) {
												currentIndex = 0;
											}
										} else {
											currentIndex = 0;
										}
									}
								}
								
								//	print (currentIndex);
								if (currentIndex == 0) {
									checkPressedAttachmentButton (attachmentInfoList [i].attachmentIconManager.notAttachmentButton.slotButton);
								} else {
									checkPressedAttachmentButton (attachmentInfoList [i].attachmentPlaceInfoList [currentIndex - 1].attachmentSlotManager.slotButton);
								}
							}
						}
					}
				}
			}

			if (showCurrentAttachmentHoverInfo) {
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
						
					if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouch.position;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);

						bool touchButtonFound = false;
						foreach (RaycastResult r in captureRaycastResults) {
							for (int j = 0; j < attachmentInfoList.Count; j++) {
								currentAttachmentPlace = attachmentInfoList [j];

								if (currentAttachmentPlace.attachmentPlaceEnabled) {
									for (int k = 0; k < currentAttachmentPlace.attachmentPlaceInfoList.Count; k++) {
										currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [k];

										if (currentAttachmentInfo.attachmentSlotManager.slotButton.gameObject == r.gameObject) {
											touchButtonFound = true;

											if (currentHoverAttachmentInfo != currentAttachmentInfo) {
												currentHoverAttachmentInfo = currentAttachmentInfo;
														
												if (currentHoverAttachmentInfo.useAttachmentHoverInfo) {
													if (!attachmentHoverInfoPanelGameObject.gameObject.activeSelf) {
														attachmentHoverInfoPanelGameObject.SetSiblingIndex (attachmentHoverInfoPanelGameObject.parent.childCount - 1);
														attachmentHoverInfoPanelGameObject.gameObject.SetActive (true);
													}

													attachmentHoverInfoPanelGameObject.position = currentHoverAttachmentInfo.attachmentSlotManager.attachmentHoverInfoPanelPosition.position;
													attachmentHoverInfoText.text = currentHoverAttachmentInfo.attachmentHoverInfo;
												} else {
													if (attachmentHoverInfoPanelGameObject.gameObject.activeSelf) {
														attachmentHoverInfoPanelGameObject.gameObject.SetActive (false);
													}
												}
											}
										}
									}
								}
							}
						}

						if (!touchButtonFound) {
							if (attachmentHoverInfoPanelGameObject.gameObject.activeSelf) {
								attachmentHoverInfoPanelGameObject.gameObject.SetActive (false);

								currentHoverAttachmentInfo = null;
							}
						}
					}
				}
			}
		}
	}

	public void activateAttachmentUIPanels ()
	{
		currentWeaponAttachmentsMenu.SetActive (editingAttachments);

		currentCamera = weaponsCamera;
		if (!currentlyOnFirstPerson && !setFirstPersonForAttachmentEditor) {
			currentCamera = mainCamera;
		}

		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				attachmentIcon curentAttachmentIcon = currentAttachmentPlace.attachmentIconManager;

				if (usingDualWeapon) {
					curentAttachmentIcon.iconRectTransform.localScale = Vector3.one * dualWeaponOffsetScale;
				} else {
					curentAttachmentIcon.iconRectTransform.localScale = Vector3.one;
				}

				if (useOffsetPanels) {

					curentAttachmentIcon.iconRectTransform.gameObject.SetActive (true);
					curentAttachmentIcon.attachmentPointTransform.gameObject.SetActive (true);

					if (usingDualWeapon) {
						currentOffsetAttachmentPlaceTransform = currentAttachmentPlace.dualWeaponOffsetAttachmentPlaceTransform;
					} else {
						currentOffsetAttachmentPlaceTransform = currentAttachmentPlace.offsetAttachmentPlaceTransform;
					}

					if (usingScreenSpaceCamera) {
						screenPoint = currentCamera.WorldToViewportPoint (currentOffsetAttachmentPlaceTransform.position);
						iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
						curentAttachmentIcon.iconRectTransform.anchoredPosition = iconPosition2d;

						curentAttachmentIcon.attachmentPointTransform.SetParent (curentAttachmentIcon.iconRectTransform.parent);

						screenPoint = currentCamera.WorldToViewportPoint (currentAttachmentPlace.offsetAttachmentPointTransform.position);
						iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
						curentAttachmentIcon.attachmentPointTransform.GetComponent<RectTransform> ().anchoredPosition = iconPosition2d;

						curentAttachmentIcon.attachmentPointTransform.SetParent (curentAttachmentIcon.iconRectTransform);
					} else {
						screenPoint = currentCamera.WorldToScreenPoint (currentOffsetAttachmentPlaceTransform.position);
						curentAttachmentIcon.iconRectTransform.transform.position = screenPoint;

						screenPoint = currentCamera.WorldToScreenPoint (currentAttachmentPlace.offsetAttachmentPointTransform.position);
						curentAttachmentIcon.attachmentPointTransform.position = screenPoint;
					}

					curentAttachmentIcon.attachmentLineTransform.gameObject.SetActive (true);

					Vector3 direction = curentAttachmentIcon.attachmentPointTransform.position - curentAttachmentIcon.attachmentLineTransform.position;
					direction = direction / direction.magnitude;

					curentAttachmentIcon.attachmentLineTransform.rotation = Quaternion.identity;

					float angle = Vector3.SignedAngle (-curentAttachmentIcon.attachmentLineTransform.up, direction, Vector3.forward);

					curentAttachmentIcon.attachmentLineTransform.rotation = Quaternion.Euler (0, 0, angle);

					float distance = GKC_Utils.distance (curentAttachmentIcon.attachmentLineTransform.GetComponent<RectTransform> ().anchoredPosition, 
						                 curentAttachmentIcon.attachmentPointTransform.GetComponent<RectTransform> ().anchoredPosition);

					distance *= UILinesScaleMultiplier;
					curentAttachmentIcon.attachmentLineTransform.localScale = new Vector3 (1, distance, 1);

				} else {
					curentAttachmentIcon.attachmentPointTransform.gameObject.SetActive (false);
					curentAttachmentIcon.attachmentLineTransform.gameObject.SetActive (false);

					if (usingScreenSpaceCamera) {
						screenPoint = currentCamera.WorldToViewportPoint (currentAttachmentPlace.attachmentPlaceTransform.position);
						iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
						curentAttachmentIcon.iconRectTransform.anchoredPosition = iconPosition2d;

					} else {
						screenPoint = currentCamera.WorldToScreenPoint (currentAttachmentPlace.attachmentPlaceTransform.position);
						curentAttachmentIcon.iconRectTransform.transform.position = screenPoint;
					}
				}
			}
		}
	}

	public void openOrCloseWeaponAttachmentEditor (bool state)
	{
		editingAttachments = state;

		if (attachmentHoverInfoPanelGameObject && attachmentHoverInfoPanelGameObject.gameObject.activeSelf) {
			attachmentHoverInfoPanelGameObject.gameObject.SetActive (false);
		}

		if (editingAttachments) {
			currentlyOnFirstPerson = weaponsManager.isFirstPersonActive ();
			previouslyOnFirstPerson = currentlyOnFirstPerson;
		}

		setFirstPersonForAttachmentEditor = weaponsManager.setFirstPersonForAttachmentEditor;

		//if the player is not in first person mode, change it to that view
		if (editingAttachments) {
			if (previouslyOnFirstPerson) {
				IKWeaponManager.enableOrDisableEditAttachment (editingAttachments);
			} else {
				if (setFirstPersonForAttachmentEditor) {
					powersManager.changeTypeView ();
				} else {
					if (canUseSmoothTransition ()) {
						IKWeaponManager.enableOrDisableEditAttachment (editingAttachments);
					} else {
						IKWeaponManager.quickEnableOrDisableEditAttachment (editingAttachments);
					}

					pauseManager.changeCameraState (!editingAttachments);

					cameraParentTransform = mainCamera.transform.parent;

					adjustThirdPersonCamera ();
				}
			}

			playSound (startEditWeaponSound);
		} else {
			if (previouslyOnFirstPerson) {
				IKWeaponManager.enableOrDisableEditAttachment (editingAttachments);
			} else {
				if (setFirstPersonForAttachmentEditor) {
					
					IKWeaponManager.stopEditAttachment ();

					powersManager.changeTypeView ();
				} else {
					if (canUseSmoothTransition ()) {
						IKWeaponManager.enableOrDisableEditAttachment (editingAttachments);
					} else {
						IKWeaponManager.quickEnableOrDisableEditAttachment (editingAttachments);
					}

					adjustThirdPersonCamera ();
				}
			}

			pauseManager.changeCameraState (!editingAttachments);

			movingToFirstPerson = false;

			playSound (stopEditWeaponSound);
		}

		pauseManager.showOrHideCursor (editingAttachments);

		pauseManager.setHeadBobPausedState (editingAttachments);

		if (disableHUDWhenEditingAttachments) {
			pauseManager.enableOrDisablePlayerHUD (!editingAttachments);
		}

		weaponSystem.enableHUD (!editingAttachments);

		grabObjectsManager.enableOrDisableGeneralCursor (!editingAttachments);

		weaponsManager.enableOrDisableGeneralWeaponCursor (!editingAttachments);

		playerControllerManager.changeScriptState (!editingAttachments);

		if (editingAttachments) {
			instantiateAttachmentIcons ();
			chechAttachmentPlaces = true;
		} else {
			currentWeaponAttachmentsMenu.SetActive (editingAttachments);
		}

		pauseManager.openOrClosePlayerMenu (editingAttachments, null, false);

		pauseManager.enableOrDisableDynamicElementsOnScreen (!editingAttachments);
	}

	public bool canUseSmoothTransition ()
	{
		return (useSmoothTransitionFreeCamera && playerCameraManager.isCameraTypeFree ()) || (useSmoothTransitionLockedCamera && !playerCameraManager.isCameraTypeFree ());
	}

	public void cancelWeaponAttachmentEditor ()
	{
		if (editingAttachments) {

			editingAttachments = false;

			IKWeaponManager.stopEditAttachment ();

			pauseManager.showOrHideCursor (editingAttachments);
			pauseManager.changeCameraState (!editingAttachments);
			pauseManager.setHeadBobPausedState (editingAttachments);
			pauseManager.enableOrDisableDynamicElementsOnScreen (!editingAttachments);

			if (disableHUDWhenEditingAttachments) {
				pauseManager.enableOrDisablePlayerHUD (!editingAttachments);
			}

			currentWeaponAttachmentsMenu.SetActive (editingAttachments);

			if (mainCameraMovement != null) {
				StopCoroutine (mainCameraMovement);
			}
				
			mainCameraTransform.SetParent (playerCameraManager.getCameraTransform ());

			mainCameraTransform.localRotation = mainCameraTargetRotation;
			mainCameraTransform.localPosition = mainCameraTargetPosition;
		}
	}

	public void setAttachmentInitialState ()
	{
		attachmentInfo attchmentActiveInfo = new attachmentInfo ();

		for (int i = 0; i < attachmentInfoList.Count; i++) {
			bool attachmentPlaceEnabled = attachmentInfoList [i].attachmentPlaceEnabled;
			bool anyAttachmentEnabled = false;
			bool anyAttachmentActive = false;
			for (int j = 0; j < attachmentInfoList [i].attachmentPlaceInfoList.Count; j++) {
				currentAttachmentInfo = attachmentInfoList [i].attachmentPlaceInfoList [j];

				if (!attachmentPlaceEnabled) {
					currentAttachmentInfo.attachmentEnabled = attachmentPlaceEnabled;
				}

				if (currentAttachmentInfo.attachmentEnabled) {
					
					weaponsManager.setCurrentWeaponSystemWithAttachment (weaponSystem);

					if (currentAttachmentInfo.onlyEnabledWhileCarrying) {
						if (!weaponSystem.carryingWeapon ()) {
							currentAttachmentInfo.deactivateEvent.Invoke ();
						}

						if (currentAttachmentInfo.attachmentActive) {
							attachmentInfoList [i].currentAttachmentSelectedIndex = j + 1;
							anyAttachmentActive = true;
						}
					} else {
						if (currentAttachmentInfo.attachmentActive) {
							currentAttachmentInfo.currentlyActive = true;
							currentAttachmentInfo.activateEvent.Invoke ();
							attachmentInfoList [i].currentAttachmentSelectedIndex = j + 1;
							anyAttachmentActive = true;

							attchmentActiveInfo = currentAttachmentInfo;
						} else {
							currentAttachmentInfo.currentlyActive = false;
							currentAttachmentInfo.deactivateEvent.Invoke ();
						}
					}
				} else {
					currentAttachmentInfo.attachmentActive = false;
					currentAttachmentInfo.currentlyActive = false;
				}

				if (!anyAttachmentEnabled) {
					anyAttachmentEnabled = currentAttachmentInfo.attachmentEnabled;
				}

				if (currentAttachmentInfo.attachmentGameObject) {
					currentAttachmentInfo.attachmentGameObject.SetActive (currentAttachmentInfo.attachmentActive);

					if (currentAttachmentInfo.attachmentActive) {
						weaponsManager.setWeaponPartLayer (currentAttachmentInfo.attachmentGameObject);	
					}

				}
			}

			attachmentsActiveInWeapon = anyAttachmentActive;

			for (int k = 0; k < attachmentInfoList [i].objectToReplace.Count; k++) {
				if (attachmentInfoList [i].objectToReplace [k]) {
					attachmentInfoList [i].objectToReplace [k].SetActive (!anyAttachmentActive);

					if (!anyAttachmentActive) {
						weaponsManager.setWeaponPartLayer (attachmentInfoList [i].objectToReplace [k]);
					}
				}
			}

			if (anyAttachmentActive) {
				if (attchmentActiveInfo.useEventHandPosition) {
					attchmentActiveInfo.activateEventHandPosition.Invoke ();
				}
			} else {
				if (attchmentActiveInfo.useEventHandPosition) {
					attchmentActiveInfo.deactivateEventHandPosition.Invoke ();
				}
			}

			if (!anyAttachmentEnabled) {
				attachmentInfoList [i].attachmentPlaceEnabled = false;
			}
		}
	}

	public void setAttachmentState (bool state, int attachmentInfoListIndex, int attachmentPlaceListIndex, string attachmentName)
	{
		bool attachmentFound = false;
		attachmentPlace currentAttachmentPlace = attachmentInfoList [attachmentInfoListIndex];
		for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
			currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
			if (currentAttachmentInfo.attachmentEnabled) {
				bool stateToConfigure = false;
				if (attachmentName == currentAttachmentInfo.Name) {
					stateToConfigure = state;
					attachmentFound = true;
				}

				currentAttachmentInfo.attachmentActive = stateToConfigure;

				currentAttachmentInfo.currentlyActive = stateToConfigure;

				weaponsManager.setCurrentWeaponSystemWithAttachment (weaponSystem);

				if (currentAttachmentInfo.currentlyActive) {
					if ((currentAttachmentInfo.onlyEnabledWhileCarrying && weaponSystem.carryingWeapon ()) || !currentAttachmentInfo.onlyEnabledWhileCarrying) { 
						currentAttachmentInfo.activateEvent.Invoke ();

						if (currentAttachmentInfo.useEventHandPosition) {
							currentAttachmentInfo.activateEventHandPosition.Invoke ();
						}
					}
					if (attachmentName == currentAttachmentInfo.Name) {
						currentAttachmentPlace.currentAttachmentSelectedIndex = j + 1;
					}
				} else {
					currentAttachmentInfo.deactivateEvent.Invoke ();

					if (attachmentName == currentAttachmentInfo.Name) {
						currentAttachmentPlace.currentAttachmentSelectedIndex = 0;
					}

					if (currentAttachmentInfo.useEventHandPosition) {
						currentAttachmentInfo.deactivateEventHandPosition.Invoke ();
					}
				}

				if (currentAttachmentInfo.attachmentGameObject) {
					currentAttachmentInfo.attachmentGameObject.SetActive (stateToConfigure);

					weaponsManager.setWeaponPartLayer (currentAttachmentInfo.attachmentGameObject);	
				}

				if (attachmentName == "") {
					currentAttachmentPlace.currentAttachmentSelectedIndex = 0;
				}
			}
		}

		if (attachmentFound) {
			playSound (currentAttachmentPlace.attachmentPlaceInfoList [attachmentPlaceListIndex].selectAttachmentSound);
		} else {
			playSound (removeAttachmentSound);
		}

		for (int k = 0; k < currentAttachmentPlace.objectToReplace.Count; k++) {
			if (currentAttachmentPlace.objectToReplace [k]) {
				bool stateToConfigure = false;
				if (attachmentFound) {
					stateToConfigure = !state;
				} else {
					stateToConfigure = true;
				}

				currentAttachmentPlace.objectToReplace [k].SetActive (stateToConfigure);

				if (stateToConfigure) {
					weaponsManager.setWeaponPartLayer (currentAttachmentPlace.objectToReplace [k]);
				}
			}
		}

		weaponsManager.setCurrentWeaponSystemWithAttachment (weaponSystem);

		if (attachmentFound) {
			currentAttachmentInfo = attachmentInfoList [attachmentInfoListIndex].attachmentPlaceInfoList [attachmentPlaceListIndex];
			if (currentAttachmentInfo.currentlyActive) {
				if ((currentAttachmentInfo.onlyEnabledWhileCarrying && weaponSystem.carryingWeapon ()) || !currentAttachmentInfo.onlyEnabledWhileCarrying) { 
					currentAttachmentInfo.activateEvent.Invoke ();

					if (currentAttachmentInfo.useEventHandPosition) {
						currentAttachmentInfo.activateEventHandPosition.Invoke ();
					}
				}
			} else {
				currentAttachmentInfo.deactivateEvent.Invoke ();

				if (currentAttachmentInfo.useEventHandPosition) {
					currentAttachmentInfo.deactivateEventHandPosition.Invoke ();
				}
			}
		}
	}

	public void checkPressedAttachmentButton (Button pressedButton)
	{
		bool attachmentPreviouslyActive = false;
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
					if (currentAttachmentInfo.attachmentEnabled) {
						if (currentAttachmentInfo.attachmentSlotManager.slotButton == pressedButton && currentAttachmentInfo.currentlyActive) {
							attachmentPreviouslyActive = true;
						} else if (currentAttachmentPlace.attachmentIconManager.notAttachmentButton.slotButton == pressedButton && currentAttachmentPlace.currentAttachmentSelectedIndex == 0) {
							attachmentPreviouslyActive = true;
						}
					}
				}
			}
		}

		if (attachmentPreviouslyActive) {
			print ("attachment already active, cancel setup");
			return;
		}

		bool buttonFound = false;
		bool noAttachmentPressed = false;
		int attachmentInfoIndex = -1;
		int attachmentSlotIndex = -1;
			
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				if (!buttonFound) {
					for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
						if (!buttonFound) {
							currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
							if (currentAttachmentInfo.attachmentEnabled) {
								if (currentAttachmentInfo.attachmentSlotManager.slotButton == pressedButton) {
									setAttachmentState (true, i, j, currentAttachmentInfo.Name);
									buttonFound = true;
								} else if (currentAttachmentPlace.attachmentIconManager.notAttachmentButton.slotButton == pressedButton) {
									setAttachmentState (false, i, j, "");
									buttonFound = true;
									noAttachmentPressed = true;
								}
						
								if (buttonFound) {
									attachmentInfoIndex = i;
									attachmentSlotIndex = j;
								}
							}
						}
					}
				}
			}
		}

		if (buttonFound) {
			currentAttachmentPlace = attachmentInfoList [attachmentInfoIndex];
			currentAttachmentPlace.attachmentIconManager.notAttachmentButton.attachmentSelectedIcon.SetActive (noAttachmentPressed);

			for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
				if (currentAttachmentPlace.attachmentPlaceInfoList [j].attachmentEnabled) {
					currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
					currentAttachmentInfo.attachmentSlotManager.attachmentSelectedIcon.SetActive (false);
				}
			}

			if (!noAttachmentPressed) {
				currentAttachmentPlace.attachmentPlaceInfoList [attachmentSlotIndex].attachmentSlotManager.attachmentSelectedIcon.SetActive (true);
			}
		}
	}

	public void setAttachmentsState (bool state, bool changeCurrentState)
	{
		bool attachmentUseHUD = false;
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			if (attachmentInfoList [i].attachmentPlaceEnabled) {
				bool anyAttachmentEnabled = false;
				currentAttachmentPlace = attachmentInfoList [i];
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
					if (currentAttachmentInfo.attachmentEnabled) {
						if (currentAttachmentInfo.onlyEnabledWhileCarrying) {
							if (changeCurrentState) {
								currentAttachmentInfo.attachmentActive = state;

								if (currentAttachmentInfo.attachmentGameObject) {
									currentAttachmentInfo.attachmentGameObject.SetActive (state);
								}
							}

							weaponsManager.setCurrentWeaponSystemWithAttachment (weaponSystem);

							if (currentAttachmentInfo.attachmentActive) {
								currentAttachmentInfo.currentlyActive = state;
								if (currentAttachmentInfo.currentlyActive) {
									currentAttachmentInfo.activateEvent.Invoke ();
									anyAttachmentEnabled = true;

									if (currentAttachmentInfo.useEventHandPosition) {
										currentAttachmentInfo.activateEventHandPosition.Invoke ();
									}

									if (currentAttachmentInfo.attachmentGameObject) {
										weaponsManager.setWeaponPartLayer (currentAttachmentInfo.attachmentGameObject);
									}
								} else {
									currentAttachmentInfo.deactivateEvent.Invoke ();

									if (currentAttachmentInfo.useEventHandPosition) {
										currentAttachmentInfo.deactivateEventHandPosition.Invoke ();
									}
								}
							}
						}

						//check if the current weapon has an attachment which uses the hud, like the weapon system component, to show the ammo amount and attachmnent icon
						if (currentAttachmentInfo.currentlyActive) {
							if (currentAttachmentInfo.attachmentUseHUD) {
								currentAttachmentInfo.attachmentGameObject.GetComponent<weaponSystem> ().setUsingWeaponState (true);
								attachmentUseHUD = true;
							}
						}
					}
				}

				if (anyAttachmentEnabled && changeCurrentState) {
					for (int k = 0; k < currentAttachmentPlace.objectToReplace.Count; k++) {
						if (currentAttachmentPlace.objectToReplace [k]) {
							currentAttachmentPlace.objectToReplace [k].SetActive (!anyAttachmentEnabled);
							if (!anyAttachmentEnabled) {
								weaponsManager.setWeaponPartLayer (currentAttachmentPlace.objectToReplace [k]);
							}
						}
					}
				}
			}
		}

		if (!attachmentUseHUD) {
			weaponsManager.setAttachmentPanelState (false);
		}
	}

	public void checkAttachmentsHUD ()
	{
		bool attachmentUseHUD = false;
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			if (attachmentInfoList [i].attachmentPlaceEnabled) {

				currentAttachmentPlace = attachmentInfoList [i];
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
					if (currentAttachmentInfo.attachmentEnabled) {
						//check if the current weapon has an attachment which uses the hud, like the weapon system component, to show the ammo amount and attachmnent icon
						if (currentAttachmentInfo.currentlyActive) {
							if (currentAttachmentInfo.attachmentUseHUD) {
								weaponsManager.setAttachmentPanelState (true);
								attachmentUseHUD = true;
							}
						}
					}
				}
			}
		}

		if (!attachmentUseHUD) {
			weaponsManager.setAttachmentPanelState (false);
		}
	}

	public void checkAttachmentsOnDrawWeapon ()
	{
		setAttachmentsState (true, false);
	}

	public void checkAttachmentsOnKeepWeapon ()
	{
		setAttachmentsState (false, false);
	}

	public bool pickupAttachment (string attachmentName)
	{
		int attachmentPlaceIndex = -1;
		int attachmentInfoIndex = -1;
		bool attachmentFound = false;
		//print (attachmentName);

		for (int i = 0; i < attachmentInfoList.Count; i++) {
			if (!attachmentFound) {
				currentAttachmentPlace = attachmentInfoList [i];
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					if (!attachmentFound) {
						currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
						if (!currentAttachmentInfo.attachmentEnabled) {
							//print (currentAttachmentInfo.Name + " " + attachmentName);
							if (currentAttachmentInfo.Name == attachmentName) {
								attachmentPlaceIndex = i;
								attachmentInfoIndex = j;
								attachmentFound = true;
							}
						}
					}
				}
			}
		}

		if (!attachmentFound) {
			return false;
		}

		currentAttachmentPlace = attachmentInfoList [attachmentPlaceIndex];
		currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [attachmentInfoIndex];

		//print ("picked " + attachmentName);

		currentAttachmentInfo.attachmentEnabled = true;
		currentAttachmentInfo.attachmentActive = true;
		if (currentAttachmentInfo.attachmentSlotManager != null && currentAttachmentInfo.attachmentSlotManager.slotButton != null) {
			currentAttachmentInfo.attachmentSlotManager.slotButton.gameObject.SetActive (true);
			currentAttachmentInfo.attachmentSlotManager.attachmentSelectedIcon.SetActive (true);
		}

		if (!currentAttachmentPlace.attachmentPlaceEnabled) {
			currentAttachmentPlace.attachmentPlaceEnabled = true;
			if (currentAttachmentPlace.attachmentIconManager != null && currentAttachmentPlace.attachmentIconManager.iconRectTransform != null) {
				currentAttachmentPlace.attachmentIconManager.iconRectTransform.gameObject.SetActive (true);
				currentAttachmentPlace.attachmentIconManager.notAttachmentButton.attachmentSelectedIcon.SetActive (false);
			}
		}

		//add option to select if a picked attachment is selected or just activated			
		if (setPickedAttachments) {
			setAttachmentState (true, attachmentPlaceIndex, attachmentInfoIndex, currentAttachmentInfo.Name);
		}
		return true;
	}

	public void enableAllAttachments ()
	{
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
				currentAttachmentInfo = currentAttachmentPlace.attachmentPlaceInfoList [j];
				pickupAttachment (currentAttachmentInfo.Name);
			}
		}
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource && sound) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);
			mainAudioSource.PlayOneShot (sound);
		}
	}

	bool adjustThirdPersonCameraActive = true;

	public void setAdjustThirdPersonCameraActiveState (bool state)
	{
		adjustThirdPersonCameraActive = state;
	}

	//activate the device
	public void adjustThirdPersonCamera ()
	{
		if (!adjustThirdPersonCameraActive) {
			return;
		}

		if (mainCameraMovement != null) {
			StopCoroutine (mainCameraMovement);
		}

		mainCameraMovement = StartCoroutine (adjustThirdPersonCameraCoroutine ());
	}

	//move the camera from its position in player camera to a fix position for a proper looking of the computer and vice versa
	IEnumerator adjustThirdPersonCameraCoroutine ()
	{
		movingCamera = true;

		if (usingDualWeapon && secondaryWeaponAttachmentSystem) {
			secondaryWeaponAttachmentSystem.movingCamera = true;
		}

		mainCameraTargetRotation = Quaternion.identity;
		mainCameraTargetPosition = Vector3.zero;

		Transform targetTransform = IKWeaponManager.getThirdPersonAttachmentCameraPosition ();
	
		if (!editingAttachments) {
			targetTransform = cameraParentTransform;
		} 

		mainCameraTransform.SetParent (targetTransform);

		//store the current rotation of the camera
		Quaternion currentQ = mainCameraTransform.localRotation;
		//store the current position of the camera
		Vector3 currentPos = mainCameraTransform.localPosition;

		if (canUseSmoothTransition ()) {
			float i = 0;

			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * thirdPersonCameraMovementSpeed;
				mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPos, mainCameraTargetPosition, i);
				yield return null;
			}
		} else {
			mainCameraTransform.localRotation = mainCameraTargetRotation;
			mainCameraTransform.localPosition = mainCameraTargetPosition;
		}

		if (usingDualWeapon && secondaryWeaponAttachmentSystem) {
			secondaryWeaponAttachmentSystem.movingCamera = false;
		}

		movingCamera = false;
	}

	public bool canBeOpened ()
	{
		if (canEditWeaponWithoutAttchments || attachmentsActiveInWeapon) {
			return true;
		}
		return false;
	}

	public void enableOrDisableAllAttachmentPlaces (bool value)
	{
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			attachmentInfoList [i].attachmentPlaceEnabled = value;
		}

		updateComponent ();
	}


	public void enableOrDisableAllAttachment (bool value, int attachmentPlaceIndex)
	{
		currentAttachmentPlace = attachmentInfoList [attachmentPlaceIndex]; 
		for (int i = 0; i < currentAttachmentPlace.attachmentPlaceInfoList.Count; i++) {
			currentAttachmentPlace.attachmentPlaceInfoList [i].attachmentEnabled = value;
		}

		updateComponent ();
	}


	public void useEventOnPress ()
	{
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					if (currentAttachmentPlace.attachmentPlaceInfoList [j].attachmentEnabled && currentAttachmentPlace.attachmentPlaceInfoList [j].currentlyActive) {
						if (currentAttachmentPlace.attachmentPlaceInfoList [j].useEventOnPress) {
							currentAttachmentPlace.attachmentPlaceInfoList [j].eventOnPress.Invoke ();
						}
					}
				}
			}
		}
	}

	public void useEventOnPressDown ()
	{
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					if (currentAttachmentPlace.attachmentPlaceInfoList [j].attachmentEnabled && currentAttachmentPlace.attachmentPlaceInfoList [j].currentlyActive) {
						if (currentAttachmentPlace.attachmentPlaceInfoList [j].useEventOnPressDown) {
							currentAttachmentPlace.attachmentPlaceInfoList [j].eventOnPressDown.Invoke ();
						}
					}
				}
			}
		}
	}


	public void useEventOnPressUp ()
	{
		for (int i = 0; i < attachmentInfoList.Count; i++) {
			currentAttachmentPlace = attachmentInfoList [i];
			if (currentAttachmentPlace.attachmentPlaceEnabled) {
				for (int j = 0; j < currentAttachmentPlace.attachmentPlaceInfoList.Count; j++) {
					if (currentAttachmentPlace.attachmentPlaceInfoList [j].attachmentEnabled && currentAttachmentPlace.attachmentPlaceInfoList [j].currentlyActive) {
						if (currentAttachmentPlace.attachmentPlaceInfoList [j].useEventOnPressUp) {
							currentAttachmentPlace.attachmentPlaceInfoList [j].eventOnPressUp.Invoke ();
						}
					}
				}
			}
		}
	}

	public void setSecondaryWeaponAttachmentSystem (weaponAttachmentSystem newAttachment)
	{
		secondaryWeaponAttachmentSystem = newAttachment;
	}

	public void setUsingDualWeaponState (bool state)
	{
		usingDualWeapon = state;
	}

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
			for (int i = 0; i < attachmentInfoList.Count; i++) {
				Gizmos.color = Color.green;
				Gizmos.DrawSphere (attachmentInfoList [i].attachmentPlaceTransform.position, 0.01f);

				Gizmos.color = Color.yellow;
				if (showDualWeaponsGizmo) {
					Gizmos.DrawLine (attachmentInfoList [i].attachmentPlaceTransform.position, attachmentInfoList [i].dualWeaponOffsetAttachmentPlaceTransform.position);
					Gizmos.color = Color.grey;
					Gizmos.DrawSphere (attachmentInfoList [i].dualWeaponOffsetAttachmentPlaceTransform.position, 0.01f);
				} else {
					Gizmos.DrawLine (attachmentInfoList [i].attachmentPlaceTransform.position, attachmentInfoList [i].offsetAttachmentPlaceTransform.position);
					Gizmos.color = Color.red;
					Gizmos.DrawSphere (attachmentInfoList [i].offsetAttachmentPlaceTransform.position, 0.01f);
				}
			}
		}

	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<weaponAttachmentSystem> ());
		#endif
	}

	[System.Serializable]
	public class attachmentPlace
	{
		public string Name;
		public bool attachmentPlaceEnabled = true;
		public string noAttachmentText;
		public List<attachmentInfo> attachmentPlaceInfoList = new List<attachmentInfo> ();
		public Transform attachmentPlaceTransform;

		public Transform offsetAttachmentPlaceTransform;
		public Transform offsetAttachmentPointTransform;

		public Transform dualWeaponOffsetAttachmentPlaceTransform;

		public List<GameObject> objectToReplace = new List<GameObject> ();
		public attachmentIcon attachmentIconManager;
		public int currentAttachmentSelectedIndex;
	}
}
