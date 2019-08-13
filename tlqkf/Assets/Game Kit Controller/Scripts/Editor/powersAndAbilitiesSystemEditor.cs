using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(powersAndAbilitiesSystem))]
public class powersAndAbilitiesSystemEditor : Editor
{
	SerializedObject systemToUse;
	powersAndAbilitiesSystem manager;

	bool expanded;
	bool applicationIsPlaying;

	void OnEnable ()
	{
		systemToUse = new SerializedObject (target);
		manager = (powersAndAbilitiesSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (systemToUse == null) {
			return;
		}
		systemToUse.Update ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
	
		GUILayout.BeginVertical ("Power Event List", "window");
		showPowerInfoList (systemToUse.FindProperty ("powerInfoList"));
		GUILayout.EndVertical ();

		applicationIsPlaying = Application.isPlaying;

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			systemToUse.ApplyModifiedProperties ();
		}
	}

	void showPowerInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Abilities: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Power")) {
				list.arraySize++;
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showPowerInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showPowerInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToEnable"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToDisable"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Enable Ability")) {
			if (applicationIsPlaying) {
				manager.enableGeneralPower (list.FindPropertyRelative ("Name").stringValue.ToString ());
			}
		}
		if (GUILayout.Button ("Disable Ability")) {
			if (applicationIsPlaying) {
				manager.disableGeneralPower (list.FindPropertyRelative ("Name").stringValue.ToString ());
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}
}
#endif
