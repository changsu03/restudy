using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerTutorialSystem))]
public class playerTutorialSystemEditor : Editor
{
	SerializedObject list;
	playerTutorialSystem manager;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (playerTutorialSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("tutorialsEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("tutorialOpened"));
		EditorGUILayout.PropertyField (list.FindProperty ("tutorialsPanel"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainAudioSource"));
		EditorGUILayout.PropertyField (list.FindProperty ("videoAudioSource"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial Info List", "window", GUILayout.Height (30));
		showTutorialInfoList (list.FindProperty ("tutorialInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showTutorialInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("unlockCursorOnTutorialActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionButtonToMoveThroughTutorial"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pressAnyButtonToNextTutorial"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeToEnableKeys"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openTutorialDelay"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setCustomTimeScale"));
		if (list.FindPropertyRelative ("setCustomTimeScale").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customTimeScale"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnTutorialOpen"));
		if (list.FindPropertyRelative ("useSoundOnTutorialOpen").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("soundOnTutorialOpen"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playTutorialOnlyOnce"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial Video Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsVideo"));

		if (list.FindPropertyRelative ("containsVideo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNextPanelWhenVideoEnds"));

			EditorGUILayout.Space ();

			showTutorialVideoList (list.FindPropertyRelative ("videoInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial Panel List", "window", GUILayout.Height (30));
		showTutorialPanelList (list.FindPropertyRelative ("tutorialPanelList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Activate Tutorial (Debug)")) {
			if (Application.isPlaying) {
				manager.activatingTutorialByNameFromEditor ();
				manager.activateTutorialByName (list.FindPropertyRelative ("Name").stringValue);
			}
		}
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showTutorialInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("Tutorial Info List"));
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Tutorials: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tutorial")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showTutorialPanelList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("Tutorial Panel List"));
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Panels: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Panel")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialPanelListElement (list.GetArrayElementAtIndex (i));
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

	void showTutorialPanelListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelGameObject"));
		GUILayout.EndVertical ();
	}

	void showTutorialVideoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("Tutorial Video List"));
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Videos: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Video")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialVideoListElement (list.GetArrayElementAtIndex (i));
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

	void showTutorialVideoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoRawImage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoFile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useVideoAudio"));
		if (list.FindPropertyRelative ("useVideoAudio").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoAudioVolume"));	
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("loopVideo"));
		GUILayout.EndVertical ();
	}
}
#endif