using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(playerWeaponsManager))]
public class playerWeaponsManagerEditor : Editor
{
	playerWeaponsManager manager;
	SerializedObject objectToUse;
	bool settings;
	bool elementSettings;
	bool weaponsList;
	bool debugSettings;
	Color buttonColor;

	string carryingWeaponMode;
	string aimingWeaponMode;
	string currentState;
	string currentWeaponName;
	string shootingWeapon;
	string weaponsModeActive;
	string inputListOpenedText;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (playerWeaponsManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Player Weapons State", "window");

		carryingWeaponMode = "Not Carrying";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("carryingWeaponInThirdPerson").boolValue) {
				carryingWeaponMode = "Third Person";
			} 
			if (objectToUse.FindProperty ("carryingWeaponInFirstPerson").boolValue) {
				carryingWeaponMode = "First Person";
			}
		} 
		GUILayout.Label ("Carrying Weapon\t " + carryingWeaponMode);

		aimingWeaponMode = "Not Aiming";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("aimingInThirdPerson").boolValue) {
				aimingWeaponMode = "Third Person";
			} 
			if (objectToUse.FindProperty ("aimingInFirstPerson").boolValue) {
				aimingWeaponMode = "First Person";
			}
		} 
		GUILayout.Label ("Aiming Weapon\t " + aimingWeaponMode);

		currentState = "Not Using";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("anyWeaponAvailable").boolValue) {
				if (objectToUse.FindProperty ("carryingWeaponInThirdPerson").boolValue) {
					currentState = "Carrying Weapon 3st";
				} else if (objectToUse.FindProperty ("carryingWeaponInFirstPerson").boolValue) {
					currentState = "Carrying Weapon 1st";
				} else {
					currentState = "Not Carrying";
				}
			} else {
				currentState = "Not Available";
			}
		} 
		GUILayout.Label ("Current State\t " + currentState);

		currentWeaponName = "None";
		if (Application.isPlaying) {
			if (objectToUse.FindProperty ("anyWeaponAvailable").boolValue) {
				currentWeaponName = objectToUse.FindProperty ("currentWeaponName").stringValue.ToString ();
			}
		} 

		GUILayout.Label ("Weapons Available\t " + objectToUse.FindProperty ("anyWeaponAvailable").boolValue);
		GUILayout.Label ("Changing Weapon\t " + objectToUse.FindProperty ("changingWeapon").boolValue);
		GUILayout.Label ("Keeping Weapon\t " + objectToUse.FindProperty ("keepingWeapon").boolValue);
		GUILayout.Label ("Editing Attachments\t " + objectToUse.FindProperty ("editingWeaponAttachments").boolValue);

		weaponsModeActive = "NO";
		if (objectToUse.FindProperty ("weaponsModeActive").boolValue) {
			weaponsModeActive = "YES";
		}
		GUILayout.Label ("Weapons Mode Active\t " + weaponsModeActive);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Single Weapon State", "window", GUILayout.Height (30));
		GUILayout.Label ("Current Weapon\t " + currentWeaponName);

		shootingWeapon = "NO";
		if (objectToUse.FindProperty ("shootingSingleWeapon").boolValue) {
			shootingWeapon = "YES";
		}
		GUILayout.Label ("Shooting Weapon\t " + shootingWeapon);
		GUILayout.Label ("Weapon Index\t " + objectToUse.FindProperty ("choosedWeapon").intValue.ToString ());
		GUILayout.Label ("Using Free Fire Mode\t " + objectToUse.FindProperty ("usingFreeFireMode").boolValue.ToString ());
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentIKWeapon"));
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon State", "window", GUILayout.Height (30));
		GUILayout.Label ("Using Dual Weapons\t " + objectToUse.FindProperty ("usingDualWeapon").boolValue);
		GUILayout.Label ("Current R Weapon\t " + objectToUse.FindProperty ("currentRighWeaponName").stringValue);
		GUILayout.Label ("Current L Weapon\t " + objectToUse.FindProperty ("currentLeftWeaponName").stringValue);
		GUILayout.Label ("Shootint R Weapon\t " + objectToUse.FindProperty ("shootingRightWeapon").boolValue);
		GUILayout.Label ("Shooting L Weapon\t " + objectToUse.FindProperty ("shootingLeftWeapon").boolValue);
		GUILayout.Label ("Dual Slot Index\t " + objectToUse.FindProperty ("chooseDualWeaponIndex").intValue.ToString());
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentRightIKWeapon"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentLeftIkWeapon"));
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		settings = objectToUse.FindProperty ("showSettings").boolValue;
		elementSettings = objectToUse.FindProperty ("showElementSettings").boolValue;
		weaponsList = objectToUse.FindProperty ("showWeaponsList").boolValue;
		debugSettings = objectToUse.FindProperty ("showDebugSettings").boolValue;
	
		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();

		inputListOpenedText = "";
		if (settings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			settings = !settings;
		}
		if (elementSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			elementSettings = !elementSettings;
		}

		if (weaponsList) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Weapon List";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Weapon List";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			weaponsList = !weaponsList;
		}
		if (debugSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Debug Options";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Debug Options";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			debugSettings = !debugSettings;
		}

		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		objectToUse.FindProperty ("showSettings").boolValue = settings;
		objectToUse.FindProperty ("showElementSettings").boolValue = elementSettings;
		objectToUse.FindProperty ("showWeaponsList").boolValue = weaponsList;
		objectToUse.FindProperty ("showDebugSettings").boolValue = debugSettings;

		if (settings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the max amount of weapons adn the layer used in weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsSlotsAmount"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsLayer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("targetToDamageLayer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("targetForScorchLayer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("setWeaponWhenPicked"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canGrabObjectsCarryingWeapons"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeToNextWeaponIfAmmoEmpty"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawKeepWeaponWhenModeChanged"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeWeaponsWithNumberKeysActive"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeWeaponsWithMouseWheelActive"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeWeaponsWithKeysActive"));	

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Management Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canFireWeaponsWithoutAiming"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawWeaponIfFireButtonPressed"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawAndAimWeaponIfFireButtonPressed"));	

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepWeaponAfterDelayThirdPerson"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepWeaponAfterDelayFirstPerson"));	
			if (objectToUse.FindProperty ("keepWeaponAfterDelayThirdPerson").boolValue || objectToUse.FindProperty ("keepWeaponAfterDelayFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepWeaponDelay"));	
			} 
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useQuickDrawWeapon"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToStopAimAfterStopFiring"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAimCameraOnFreeFireMode"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pivotPointRotationActive"));
			if (objectToUse.FindProperty ("pivotPointRotationActive").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("pivotPointRotationSpeed"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("runWhenAimingWeaponInThirdPerson"));
			if (objectToUse.FindProperty ("runWhenAimingWeaponInThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("stopRunIfPreviouslyNotRunning"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canJumpWhileAimingThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canAimOnAirThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("stopAimingOnAirThirdPerson"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("storePickedWeaponsOnInventory"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawWeaponWhenPicked"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeToNextWeaponWhenEquipped"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeToNextWeaponWhenUnequipped"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("notActivateWeaponsAtStart"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Attachment Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("openWeaponAttachmentsMenuEnabled"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("setFirstPersonForAttachmentEditor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useUniversalAttachments"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Initial Weapon Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("startGameWithCurrentWeapon"));
			if (objectToUse.FindProperty ("startGameWithCurrentWeapon").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawInitialWeaponSelected"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("startWithFirstWeaponAvailable"));
				if (!objectToUse.FindProperty ("startWithFirstWeaponAvailable").boolValue) {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("startGameWithDualWeapons"));
					if (objectToUse.FindProperty ("avaliableWeaponList").arraySize > 0) {
						if (objectToUse.FindProperty ("startGameWithDualWeapons").boolValue) {
							objectToUse.FindProperty ("rightWeaponToStartIndex").intValue = 
								EditorGUILayout.Popup ("Right Weapon To Start", objectToUse.FindProperty ("rightWeaponToStartIndex").intValue, manager.avaliableWeaponList);
							objectToUse.FindProperty ("rightWeaponToStartName").stringValue = manager.avaliableWeaponList [objectToUse.FindProperty ("rightWeaponToStartIndex").intValue];

							objectToUse.FindProperty ("leftWeaponToStartIndex").intValue = 
								EditorGUILayout.Popup ("Left Weapon To Start", objectToUse.FindProperty ("leftWeaponToStartIndex").intValue, manager.avaliableWeaponList);
							objectToUse.FindProperty ("leftWeaponToStartName").stringValue = manager.avaliableWeaponList [objectToUse.FindProperty ("leftWeaponToStartIndex").intValue];
						} else {
							objectToUse.FindProperty ("weaponToStartIndex").intValue = 
								EditorGUILayout.Popup ("Weapon To Start", objectToUse.FindProperty ("weaponToStartIndex").intValue, manager.avaliableWeaponList);
							objectToUse.FindProperty ("weaponToStartName").stringValue = manager.avaliableWeaponList [objectToUse.FindProperty ("weaponToStartIndex").intValue];
						}
					}

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Update Available Weapon List")) {
						if (!Application.isPlaying) {
							if (objectToUse.FindProperty ("storePickedWeaponsOnInventory").boolValue) {
								manager.getWeaponListString ();
							} else {
								manager.getAvailableWeaponListString ();
							}
						}
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Drop Weapon Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canDropWeapons"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeToNextWeaponWhenDrop"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropWeaponForceThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropWeaponForceFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("holdDropButtonToIncreaseForce"));
			if (objectToUse.FindProperty ("holdDropButtonToIncreaseForce").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropIncreaseForceSpeed"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxDropForce"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Death Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropCurrentWeaponWhenDie"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropAllWeaponsWhenDie"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dropWeaponsOnlyIfUsing"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawWeaponWhenResurrect"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Mark Targets Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canMarkTargets"));
			GUILayout.EndVertical ();
		
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Aim Assist Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAimAssistInThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAimAssistInFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useMaxDistanceToCameraCenterAimAssist"));
			if (objectToUse.FindProperty ("useMaxDistanceToCameraCenterAimAssist").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxDistanceToCameraCenterAimAssist"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAimAssistInLockedCamera"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("aimAssistLookAtTargetSpeed"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Message Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cantPickWeaponMessage"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cantDropCurrentWeaponMessage"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cantPickAttachmentMessage"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponMessageDuration"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Mode Active Event Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventsOnStateChange"));
			if (objectToUse.FindProperty ("useEventsOnStateChange").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("evenOnStateEnabled"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnStateDisabled"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Save/Load Weapons Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("loadCurrentPlayerWeaponsFromSaveFile"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("saveCurrentPlayerWeaponsToSaveFile"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Touch Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("swipeCenterPosition"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("touchZoneSize"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minSwipeDist"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("touching"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
			if (objectToUse.FindProperty ("showGizmo").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}

		if (elementSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every gameObject used for the weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD Elements", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsHUD"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("singleWeaponHUD"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentWeaponNameText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentWeaponAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("ammoSlider"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentWeaponIcon"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("attachmentPanel"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("attachmentAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentAttachmentIcon"));


			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dualWeaponHUD"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentRightWeaponAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("rightAttachmentPanel"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("rigthAttachmentAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentRightAttachmentIcon"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentLeftWeaponAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("leftAttachmentPanel"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("leftAttachmentAmmoText"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentLeftAttachmentIcon"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsParent"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsTransformInFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsTransformInThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("thirdPersonParent"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("firstPersonParent"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraController"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsCamera"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponCursor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cursorRectTransform"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponCursorRegular"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponCursorAimingInFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponCursorUnableToShoot"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponCustomReticle"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsMessageWindow"));


			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pauseManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerScreenObjectivesManager"));	

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerCameraManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("headBobManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("input"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("IKManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("powersManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("upperBodyRotationManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("headTrackManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("grabObjectsManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gameSystemManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerInventoryManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainInventoryListManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("usingDevicesManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainCollider"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("ragdollManager"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainCameraTransform"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainCamera"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainWeaponListManager"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("IK Hands Transform", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("rightHandTransform"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("leftHandTransform"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (weaponsList) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every weapon added to the player", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon List", "window", GUILayout.Height (30));
			showWeaponList (objectToUse.FindProperty ("weaponsList"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get Weapon List")) {
				manager.setWeaponList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear Weapon List")) {
				manager.clearWeaponList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Save Weapon List")) {
				manager.saveCurrentWeaponListToFile ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Pocket List", "window", GUILayout.Height (30));
			showWeaponPocketList (objectToUse.FindProperty ("weaponPocketList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (debugSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Debug options for weapon ingame", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Controls", "window", GUILayout.Height (30));
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Set Next Weapon")) {
				manager.chooseNextWeapon (false, true);
			}
			if (GUILayout.Button ("Set Previous Weapon")) {
				manager.choosePreviousWeapon (false, true);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Draw/Keep Weapon")) {
				manager.drawOrKeepWeaponInput ();
			}
			if (GUILayout.Button ("Drop Weapon")) {
				manager.dropWeaponByBebugButton ();
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Aim Weapon")) {
				manager.aimCurrentWeaponInput ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showWeaponListElementInfo (SerializedProperty list)
	{
		IKWeaponSystem IKWeapon = list.objectReferenceValue as IKWeaponSystem;
		if (IKWeapon) {
			playerWeaponSystem weaponSystem = IKWeapon.weaponGameObject.GetComponent<playerWeaponSystem> ();
			GUILayout.BeginVertical ();
			GUILayout.Label (weaponSystem.weaponSettings.Name);
			EditorGUILayout.ObjectField (IKWeapon.gameObject, typeof(GameObject));
			IKWeapon.weaponEnabled = EditorGUILayout.Toggle ("Enabled", IKWeapon.weaponEnabled);
			weaponSystem.weaponSettings.numberKey = EditorGUILayout.IntField ("Number Key:", weaponSystem.weaponSettings.numberKey);
			GUILayout.Label ("Is Current Weapon\t" + IKWeapon.currentWeapon);
			EditorUtility.SetDirty (IKWeapon);
			GUILayout.EndVertical ();
		}
	}

	void showWeaponList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weapons: \t" + list.arraySize.ToString ());
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					showWeaponListElementInfo (list.GetArrayElementAtIndex (i));
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginVertical ();
				if (GUILayout.Button ("x")) {
					manager.removeWeaponFromList (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			if (objectToUse.FindProperty ("weaponsList").arraySize > 0) {
				if (GUILayout.Button ("Enable All Weapons")) {
					manager.enableOrDisableWeaponsList (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable All Weapon")) {
					manager.enableOrDisableWeaponsList (false);

				}
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showWeaponPocketList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Pockets: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Pocket")) {
				manager.addPocket ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showWeaponListOnPocket (list.GetArrayElementAtIndex (i), i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showWeaponListOnPocket (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pocketTransform"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons On Pocket List", "window", GUILayout.Height (30));
		showWeaponOnPocketList (list.FindPropertyRelative ("weaponOnPocketList"), index);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketList (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Subpockets: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Subpocket")) {
				manager.addSubPocket (index);
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showWeaponOnPocketListElement (list.GetArrayElementAtIndex (i));
					}
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}

				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon List", "window", GUILayout.Height (30));
		showWeaponOnPocketListElementList (list.FindPropertyRelative ("weaponList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketListElementList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weapons: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Weapon")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					string weaponName = "Weapon " + i.ToString ();

					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						weaponName = list.GetArrayElementAtIndex (i).objectReferenceValue.name;
					}
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (weaponName));
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}

				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif