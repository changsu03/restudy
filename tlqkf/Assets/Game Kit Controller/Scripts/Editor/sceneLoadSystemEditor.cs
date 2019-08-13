using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(sceneLoadSystem))]
public class sceneLoadSystemEditor : Editor
{
	SerializedObject objectToUse;
	bool expanded;
	sceneLoadSystem manager;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		manager = (sceneLoadSystem)target;
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainMenuContent"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("sceneLoadMenu"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("sceneSlotPrefab"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("sceneSlotsParent"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainScrollbar"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainScrollRect"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("startingSceneIndex"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addSceneNumberToName"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scene List", "window", GUILayout.Height (30));
		showSceneList (objectToUse.FindProperty ("sceneInfoList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();

	}

	void showSceneList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Scenes: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
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

			if (GUILayout.Button ("Set Scene Number In Order")) {
				manager.setSceneNumberInOrder ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), 
						new GUIContent (list.GetArrayElementAtIndex (i).displayName + " - " + list.GetArrayElementAtIndex (i).FindPropertyRelative ("sceneNumber").intValue.ToString ()));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showSceneListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}
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
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showSceneListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sceneDescription"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sceneNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sceneImage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("addSceneToList"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fontSize"));
		GUILayout.EndVertical ();
	}
}
#endif