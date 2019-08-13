using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(consoleMode))]
public class consoleModeEditor : Editor
{
	SerializedObject list;
	consoleMode manager;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (consoleMode)target;
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
		EditorGUILayout.PropertyField (list.FindProperty ("consoleModeEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("incorrectCommandMessage"));
		EditorGUILayout.PropertyField (list.FindProperty ("lineSpacingAmount"));
		EditorGUILayout.PropertyField (list.FindProperty ("consoleOpened"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxRadiusToInstantiate"));
		EditorGUILayout.PropertyField (list.FindProperty ("deletingTextRate"));
		EditorGUILayout.PropertyField (list.FindProperty ("startDeletingTimeAmount"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Element Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("consoleWindow"));
		EditorGUILayout.PropertyField (list.FindProperty ("commandTextParent"));
		EditorGUILayout.PropertyField (list.FindProperty ("commandTextParentRectTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentConsoleCommandText"));
		EditorGUILayout.PropertyField (list.FindProperty ("input"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainGameManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("spawnPosition"));
		EditorGUILayout.PropertyField (list.FindProperty ("commandListScrollRect"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Allowed Key List", "window", GUILayout.Height (50));
		showAllowedKeyList (list.FindProperty ("allowedKeysList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Command List", "window", GUILayout.Height (50));
		showCommandInfoList (list.FindProperty ("commandInfoList"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showPrefabTypeListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("description"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("commandExecutedText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("incorrectParametersText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventSendValues"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsAmount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsBool"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsName"));
		if (!list.FindPropertyRelative ("eventSendValues").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		} else {
			if (list.FindPropertyRelative ("containsAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallAmount"));
			}

			if (list.FindPropertyRelative ("containsBool").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallBool"));
			}

			if (list.FindPropertyRelative ("containsName").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallName"));
			}
		}
	
		GUILayout.EndVertical ();
	}

	void showCommandInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Commnads: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Command")) {
				manager.addCommand ();
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
						showPrefabTypeListElement (list.GetArrayElementAtIndex (i));
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

	void showAllowedKeyList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Keys: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Key")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize--;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
				}
				GUILayout.EndHorizontal ();

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif