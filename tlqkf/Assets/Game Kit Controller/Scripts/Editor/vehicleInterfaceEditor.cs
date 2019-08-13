using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vehicleInterface))]
public class vehicleInterfaceEditor : Editor
{
	SerializedObject list;
	vehicleInterface manager;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (vehicleInterface)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("interfaceCanBeEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("interfaceEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicle"));
		EditorGUILayout.PropertyField (list.FindProperty ("interfaceCanvas"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Interface Elements List", "window", GUILayout.Height (50));
		showInterfaceList (list.FindProperty ("interfaceElementList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Interface Panel Info List", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("useInterfacePanelInfoList"));
		if (list.FindProperty ("useInterfacePanelInfoList").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("HUDManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("interfacePanelParent"));
			EditorGUILayout.PropertyField (list.FindProperty ("movePanelSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("rotatePanelSpeed"));

			EditorGUILayout.Space ();

			interfacePanelInfoList (list.FindProperty ("interfacePanelInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showInterfaceListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("uiElement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableWhenVehicleOff"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventSendValues"));

		if (list.FindPropertyRelative ("eventSendValues").boolValue) {
			GUILayout.BeginVertical ("Send Amount Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsAmount"));
			if (list.FindPropertyRelative ("containsAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsRange"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("range"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentAmountValue"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallAmount"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Send Bool Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsBool"));
			if (list.FindPropertyRelative ("containsBool").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentBoolValue"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("setValueOnText"));
				if (list.FindPropertyRelative ("setValueOnText").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("valuetText"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomValueOnText"));
				if (list.FindPropertyRelative ("useCustomValueOnText").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolActiveCustomText"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolNoActiveCustomText"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallBool"));
			}
			GUILayout.EndVertical ();
	
			EditorGUILayout.Space ();
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		}

		GUILayout.EndVertical ();
	}

	void showInterfaceList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
				manager.addInterfaceElement ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize--;
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
						showInterfaceListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void interfacePanelInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize--;
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
						interfacePanelInfoListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void interfacePanelInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("uiRectTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelParent"));

		GUILayout.EndVertical ();
	}

}
#endif