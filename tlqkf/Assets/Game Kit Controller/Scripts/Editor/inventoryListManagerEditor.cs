using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryListManager), true)]
public class inventoryListManagerEditor : Editor
{
	SerializedObject list;
	inventoryListManager inventory;
	inventoryCaptureManager inventoryWindow;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		inventory = (inventoryListManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Inventory Capture Tool Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("relativePathCaptures"));
		EditorGUILayout.PropertyField (list.FindProperty ("inventoryCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("lookObjectsPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefabs Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("emtpyInventoryPrefab"));	
		EditorGUILayout.PropertyField (list.FindProperty ("relativePathRegularInventory"));
		EditorGUILayout.PropertyField (list.FindProperty ("relativePathWeaponInventory"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Bank Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("mainInventoryBankManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List", "window");
		showUpperList (list.FindProperty ("inventoryList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list, bool expanded, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryGameObject"), new GUIContent ("Inventory Object Mesh"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectInfo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));

		EditorGUILayout.Space ();

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

		if (GUILayout.Button ("Open Inventory Capture Tool")) {
			inventoryWindow = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
			inventoryWindow.init ();
			inventoryWindow.setCurrentInventoryObjectInfo (inventory.inventoryList [index], inventory.getDataPath ());
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPerUnit"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
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

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isWeapon"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Create Inventory Prefab")) {
			inventory.createInventoryPrafab (index);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryObjectPrefab"));

		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Objects: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				inventory.addNewInventoryObject ();
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showListElementInfo (list.GetArrayElementAtIndex (i), expanded, i);
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