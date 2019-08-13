using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(pressObjectsInOrderSystem))]
public class pressObjectsInOrderSystemEditor : Editor
{
	SerializedObject objectToUse;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("allPositionsPressedEvent"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useIncorrectOrderSound"));
		if (objectToUse.FindProperty ("useIncorrectOrderSound").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("incorrectOrderSound"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("pressObjectsWhileMousePressed"));
		if (objectToUse.FindProperty ("pressObjectsWhileMousePressed").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pressObjectsLayer"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("usingPressedObjectSystem"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("allPositionsPressed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("correctPressedIndex"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Positions List", "window", GUILayout.Height (30));
		showPositionsList (objectToUse.FindProperty ("positionInfoList"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();

	}

	void showPositionInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionEvent"));
		if (list.FindPropertyRelative ("usePositionEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionEvent"));
		}
		GUILayout.EndVertical ();
	}

	void showPositionsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("Number Of Positions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Position")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + " - " + i.ToString ()));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showPositionInfo (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
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