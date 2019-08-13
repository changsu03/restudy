using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(useInventoryObject))]
public class useInventoryObjectEditor : Editor
{
	useInventoryObject manager;
	SerializedObject objectToUse;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (useInventoryObject)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("canBeReUsed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useInventoryType"));
		if (objectToUse.FindProperty ("useInventoryType").enumValueIndex == 1) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useObjectsOneByOneUsingButton"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("inventoryObjectAction"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableObjectActionAfterUse"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("tagToConfigure"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Activated Settings", "window");
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectUsedMessage"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("enableObjectWhenActivate"));
		if (objectToUse.FindProperty ("enableObjectWhenActivate").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectToEnable"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAnimation"));
		if (objectToUse.FindProperty ("useAnimation").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectWithAnimation"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("animationName"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Object State", "window");
		GUILayout.Label ("Object Used\t\t\t " + objectToUse.FindProperty ("objectUsed").boolValue);
		GUILayout.Label ("Number Of Objects Used\t\t " + objectToUse.FindProperty ("numberOfObjectsUsed").intValue);
		GUILayout.Label ("Number Of Objects Needed\t\t " + objectToUse.FindProperty ("numberOfObjectsNeeded").intValue);
		GUILayout.Label ("Current Number Of Objects Needed\t " + objectToUse.FindProperty ("currentNumberOfObjectsNeeded").intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Object To Use List", "window");
		showInventoryObjectNeededList (objectToUse.FindProperty ("inventoryObjectNeededList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Call And Functions Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("unlockFunctionCall"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showInventoryObjectNeededList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addInventoryObjectNeededInfo ();
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
						showInventoryObjectNeededListElementInfo (list.GetArrayElementAtIndex (i), i);
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

	void showInventoryObjectNeededListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectNeeded"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useObjectAction"));
		if (list.FindPropertyRelative ("useObjectAction").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectAction"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountNeeded"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectUsedMessage"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useObjectSound"));
		if (list.FindPropertyRelative ("useObjectSound").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usedObjectSound"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Placed Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnObjectsPlaced"));
		if (list.FindPropertyRelative ("useEventOnObjectsPlaced").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnObjectsPlaced"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object State", "window");
		GUILayout.Label ("Object Used\t\t " + list.FindPropertyRelative ("objectUsed").boolValue);
		GUILayout.Label ("Amount Of Objects Used\t " + list.FindPropertyRelative ("amountOfObjectsUsed").intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Objects Info", "window", GUILayout.Height (30));
		showInventoryObjectList (list.FindPropertyRelative ("inventoryObjectNeededList"), index);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showInventoryObjectList (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addSubInventoryObjectNeededList (index);
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
						showInventoryObjectListElement (list.GetArrayElementAtIndex (i));
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

	void showInventoryObjectListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("instantiateObject"));
		if (list.FindPropertyRelative ("instantiateObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeForObject"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enableObject"));
		if (list.FindPropertyRelative ("enableObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToEnable"));
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAnimation"));
		if (list.FindPropertyRelative ("useAnimation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectWithAnimation"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationName"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Placed Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnObjectPlaced"));
		if (list.FindPropertyRelative ("useEventOnObjectPlaced").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnObjectPlaced"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object State", "window");
		GUILayout.Label ("Object Activated\t\t " + list.FindPropertyRelative ("objectActivated").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}
}
#endif