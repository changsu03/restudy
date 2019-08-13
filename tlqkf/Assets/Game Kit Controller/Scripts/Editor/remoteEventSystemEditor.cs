using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(remoteEventSystem))]
public class remoteEventSystemEditor : Editor
{
	SerializedObject list;

	void OnEnable ()
	{
		list = new SerializedObject (target);
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Event Info List", "window", GUILayout.Height (50));
		showEventInfoList (list.FindProperty ("eventInfoList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showEventInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Events: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showEventInfoListElement (list.GetArrayElementAtIndex (i));
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


	void showEventInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRegularEvent"));
		if (list.FindPropertyRelative ("useRegularEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActive"));
		}
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmountOnEvent"));
		if (list.FindPropertyRelative ("useAmountOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveAmount"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolOnEvent"));
		if (list.FindPropertyRelative ("useBoolOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveBool"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGameObjectOnEvent"));
		if (list.FindPropertyRelative ("useGameObjectOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveGameObject"));
		}
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTransformOnEvent"));
		if (list.FindPropertyRelative ("useTransformOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveTransform"));
		}

		GUILayout.EndVertical ();
	}

}
#endif