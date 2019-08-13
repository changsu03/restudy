using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryManager), true)]
public class inventoryManagerEditor : Editor
{
	SerializedObject list;
	inventoryManager manager;
	bool showElementSettings;
	Color buttonColor;
	inventoryCaptureManager inventoryWindow;
	bool addInventoryObject;
	string isAdded;
	string menuOpened;
	string inputListOpenedText;
	string amountValue;
	bool isEquipped;
	string objectIsEquipped;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (inventoryManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Menu State", "window");
		menuOpened = "NO";
		if (Application.isPlaying) {
			if (list.FindProperty ("inventoryOpened").boolValue) {
				menuOpened = "YES";
			} 
		} 
		GUILayout.Label ("Inventory Menu Opened \t " + menuOpened);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("infiniteSlots"));
		if (!list.FindProperty ("infiniteSlots").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("inventorySlotAmount"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("infiniteAmountPerSlot"));
		if (!list.FindProperty ("infiniteAmountPerSlot").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("amountPerSlot"));
			EditorGUILayout.PropertyField (list.FindProperty ("combineElementsAtDrop"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("buttonUsable"), new GUIContent ("Button Usable Color"), false);
		EditorGUILayout.PropertyField (list.FindProperty ("buttonNotUsable"), new GUIContent ("Button Not Usable Color"), false);
		EditorGUILayout.PropertyField (list.FindProperty ("useOnlyWhenNeededAmountToUseObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("activeNumberOfObjectsToUseMenu"));
		EditorGUILayout.PropertyField (list.FindProperty ("setTotalAmountWhenDropObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("configureNumberObjectsToUseRate"));	
		EditorGUILayout.PropertyField (list.FindProperty ("bankManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("useBlurUIPanel"));
		EditorGUILayout.PropertyField (list.FindProperty ("examineObjectBeforeStoreEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Equip Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("storePickedWeaponsOnInventory"));
		if (list.FindProperty ("storePickedWeaponsOnInventory").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useDragDropWeaponSlots"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberWeaponsSlots"));
			EditorGUILayout.PropertyField (list.FindProperty ("equipWeaponsWhenPicked"));
			EditorGUILayout.PropertyField (list.FindProperty ("timeToDrag"));

			EditorGUILayout.PropertyField (list.FindProperty ("showWeaponSlotsWhenChangingWepaons"));
			if (list.FindProperty ("showWeaponSlotsWhenChangingWepaons").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotsParentOnInventory"));
				EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotsParentOutOfInventory"));
				EditorGUILayout.PropertyField (list.FindProperty ("showWepaonSlotsParentDuration"));
				EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotsParentScale"));
				EditorGUILayout.PropertyField (list.FindProperty ("slotWeaponSelectedIcon"));
				EditorGUILayout.PropertyField (list.FindProperty ("showWeaponSlotsAlways"));

				EditorGUILayout.PropertyField (list.FindProperty ("setWeaponSlotsAlphaValueOutOfInventory"));
				if (list.FindProperty ("setWeaponSlotsAlphaValueOutOfInventory").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotsAlphaValueOutOfInventory"));
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Messages Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedObjectMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("usedObjectMessageTime"));
		EditorGUILayout.PropertyField (list.FindProperty ("unableToUseObjectMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("nonNeededAmountAvaliable"));
		EditorGUILayout.PropertyField (list.FindProperty ("objectNotFoundMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("cantUseThisObjectHereMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("fullInventoryMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("fullInventoryMessageTime"));

		EditorGUILayout.PropertyField (list.FindProperty ("combinedObjectMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("combineObjectMessageTime"));
		EditorGUILayout.PropertyField (list.FindProperty ("unableToCombineMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("notEnoughSpaceToCombineMessage"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Show Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxZoomValue"));
		EditorGUILayout.PropertyField (list.FindProperty ("minZoomValue"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Examine Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("distanceToPlaceObjectInCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("placeObjectInCameraSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("numberOfRotationsObjectInCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("placeObjectInCameraRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("extraCameraFovOnExamineObjects"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		showElementSettings = list.FindProperty ("showElementSettings").boolValue;
		EditorGUILayout.BeginVertical ();

		inputListOpenedText = "";
		if (showElementSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showElementSettings = !showElementSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndHorizontal ();
		list.FindProperty ("showElementSettings").boolValue = showElementSettings;

		if (showElementSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set here every element used in the inventory", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("inventoryPanel"));
			EditorGUILayout.PropertyField (list.FindProperty ("inventoryListContent"));
			EditorGUILayout.PropertyField (list.FindProperty ("objectIcon"));
	
			EditorGUILayout.PropertyField (list.FindProperty ("equipButton"));
			EditorGUILayout.PropertyField (list.FindProperty ("unequipButton"));

			EditorGUILayout.PropertyField (list.FindProperty ("useButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("equipButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("unequipButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("dropButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("combineButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("examineButtonImage"));	
			EditorGUILayout.PropertyField (list.FindProperty ("discardButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("dropAllUnitsObjectButtonImage"));

			EditorGUILayout.PropertyField (list.FindProperty ("examineObjectPanel"));
			EditorGUILayout.PropertyField (list.FindProperty ("examineObjectName"));
			EditorGUILayout.PropertyField (list.FindProperty ("examineObjectDescription"));
			EditorGUILayout.PropertyField (list.FindProperty ("takeObjectInExaminePanelButton"));

			EditorGUILayout.PropertyField (list.FindProperty ("currentObjectName"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentObjectInfo"));
			EditorGUILayout.PropertyField (list.FindProperty ("objectImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("inventoryCamera"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookObjectsPosition"));
			EditorGUILayout.PropertyField (list.FindProperty ("emptyInventoryPrefab"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToUseMenu"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToUseMenuRectTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToUseText"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToUseMenuPosition"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToDropMenuPosition"));
			EditorGUILayout.PropertyField (list.FindProperty ("inventorySlotsScrollbar"));
			EditorGUILayout.PropertyField (list.FindProperty ("inventoryObjectInforScrollbar"));	

			EditorGUILayout.PropertyField (list.FindProperty ("inventoryWeaponsSlots"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotToMove"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponSlotPrefab"));

			EditorGUILayout.PropertyField (list.FindProperty ("gameSystemManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainGameManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainInventoryListManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("usingDevicesManager"));

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List Manager List", "window", GUILayout.Height (30));
		showInventoryListManagerList (list.FindProperty ("inventoryListManagerList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Inventory Manager List")) {
			manager.getInventoryListManagerList ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List (DEBUG ONLY)", "window");
		showInventoryList (list.FindProperty ("inventoryList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Drop All Inventory")) {
			manager.dropAllInventory ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Inventory Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("loadCurrentPlayerInventoryFromSaveFile"));
		EditorGUILayout.PropertyField (list.FindProperty ("saveCurrentPlayerInventoryToSaveFile"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Inventory List")) {
			manager.saveCurrentInventoryListToFile ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showInventoryListElementInfo (SerializedProperty list, bool expanded, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectInfo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Object Icon Preview \t");
		GUILayout.BeginHorizontal ("box", GUILayout.Height (50), GUILayout.Width (50));
		if (list.FindPropertyRelative ("icon").objectReferenceValue && expanded) {
			Object texture = list.FindPropertyRelative ("icon").objectReferenceValue as Texture2D;
			Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
			GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("");
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPerUnit"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeEquiped"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDropped"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeCombined"));
		if (list.FindPropertyRelative ("canBeCombined").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCombine"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObject"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObjectMessage"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDiscarded"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEquiped"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isWeapon"));
		if (list.FindPropertyRelative ("isWeapon").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("IKWeaponManager"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("button"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("menuIconElement"));
		GUILayout.EndVertical ();
	}

	void showInventoryList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addNewInventoryObject ();
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

					amountValue = " - " + list.GetArrayElementAtIndex (i).FindPropertyRelative ("amount").intValue.ToString ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + amountValue));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListElementInfo (list.GetArrayElementAtIndex (i), expanded, i);
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

	void showInventoryListManagerListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		if (manager.inventoryManagerListString.Length > 0) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
			if (!list.FindPropertyRelative ("infiniteAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEquipped"));
			list.FindPropertyRelative ("elementIndex").intValue = EditorGUILayout.Popup ("Object Name", list.FindPropertyRelative ("elementIndex").intValue, manager.inventoryManagerListString);
			list.FindPropertyRelative ("inventoryObjectName").stringValue = manager.inventoryManagerListString [list.FindPropertyRelative ("elementIndex").intValue];

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("addInventoryObject"));
		}
		GUILayout.EndVertical ();
	}

	void showInventoryListManagerList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("Inventory Manager List"));
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every inventory object of this player", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addNewInventoryObjectToInventoryListManagerList ();
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable Add All")) {
				for (int i = 0; i < list.arraySize; i++) {
					manager.setAddAllObjectEnabledState (true);
				}
			}
			if (GUILayout.Button ("Disable Add All")) {
				for (int i = 0; i < list.arraySize; i++) {
					manager.setAddAllObjectEnabledState (false);
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

					addInventoryObject = list.GetArrayElementAtIndex (i).FindPropertyRelative ("addInventoryObject").boolValue;
					isAdded = " + ";
					if (!addInventoryObject) {
						isAdded = " - ";
					}

					isEquipped = list.GetArrayElementAtIndex (i).FindPropertyRelative ("isEquipped").boolValue;
					objectIsEquipped = "";
					if (isEquipped) {
						objectIsEquipped = " E ";
					}

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + " (" + isAdded + objectIsEquipped + ")"));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListManagerListElement (list.GetArrayElementAtIndex (i));
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
				if (GUILayout.Button ("E")) {
					manager.setEquippedObjectState (i);
				}
				if (GUILayout.Button ("O")) {
					manager.setAddObjectEnabledState (i);
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
}
#endif