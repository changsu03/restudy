using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryBankManager), true)]
public class inventoryBankManagerEditor : Editor
{
	SerializedObject list;
	inventoryBankManager manager;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (inventoryBankManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();


		GUILayout.BeginVertical ("Player Inventory Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInventorySlots"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInventorySlotsContent"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInventorySlot"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInventoryScrollbar"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bank Inventory Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("bankInventorySlots"));
		EditorGUILayout.PropertyField (list.FindProperty ("bankInventorySlotsContent"));
		EditorGUILayout.PropertyField (list.FindProperty ("bankInventorySlot"));
		EditorGUILayout.PropertyField (list.FindProperty ("bankInventoryScrollbar"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Inventory Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryMenu"));
		EditorGUILayout.PropertyField (list.FindProperty ("numberOfElementsWindow"));
		EditorGUILayout.PropertyField (list.FindProperty ("numberOfObjectsToUseText"));
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryMenuRenderTexture"));
		EditorGUILayout.PropertyField (list.FindProperty ("lookObjectsPosition"));
		EditorGUILayout.PropertyField (list.FindProperty ("slotToMove"));
		EditorGUILayout.PropertyField (list.FindProperty ("menuBackground"));
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryObjectRenderPanel"));
		EditorGUILayout.PropertyField (list.FindProperty ("gameSystemManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainInventoryManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxZoomValue"));
		EditorGUILayout.PropertyField (list.FindProperty ("minZoomValue"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("dragAndDropIcons"));
		EditorGUILayout.PropertyField (list.FindProperty ("useNumberOfElementsOnPressUp"));
		EditorGUILayout.PropertyField (list.FindProperty ("dropFullObjectsAmount"));
		EditorGUILayout.PropertyField (list.FindProperty ("timeToDrag"));
		EditorGUILayout.PropertyField (list.FindProperty ("configureNumberObjectsToUseRate"));
		EditorGUILayout.PropertyField (list.FindProperty ("useBlurUIPanel"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List Manager List", "window", GUILayout.Height (30));
		showInventoryListManagerList (list.FindProperty ("inventoryListManagerList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Inventory Manager List")) {
			manager.getInventoryListManagerList ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bank Inventory List", "window");
		showInventoryList (list.FindProperty ("bankInventoryList"));
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Inventory List", "window");
		showInventoryList (list.FindProperty ("playerInventoryList"));
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Bank Inventory Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("loadCurrentBankInventoryFromSaveFile"));
		EditorGUILayout.PropertyField (list.FindProperty ("saveCurrentBankInventoryToSaveFile"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Inventory Bank List")) {
			manager.saveCurrentInventoryListToFile ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

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

					string amountValue = " - " + list.GetArrayElementAtIndex (i).FindPropertyRelative ("amount").intValue.ToString ();
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

	void showDropPickUpTypeInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		if (manager.inventoryManagerListString.Length > 0) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
			if (!list.FindPropertyRelative ("infiniteAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
			}
			list.FindPropertyRelative ("elementIndex").intValue = EditorGUILayout.Popup ("Object Name", list.FindPropertyRelative ("elementIndex").intValue, manager.inventoryManagerListString);
			list.FindPropertyRelative ("inventoryObjectName").stringValue = manager.inventoryManagerListString [list.FindPropertyRelative ("elementIndex").intValue];
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
			EditorGUILayout.HelpBox ("Configure every inventory object of this Bank", MessageType.None);
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
						showDropPickUpTypeInfo (list.GetArrayElementAtIndex (i));
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
}
#endif