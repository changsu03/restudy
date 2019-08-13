using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerStatesManager))]
public class playerStatesManagerEditor : Editor
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

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("openPlayerModeMenuEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("changeModeEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("closeMenuWhenModeSelected"));
		EditorGUILayout.PropertyField (list.FindProperty ("canSetRegularModeActive"));
		EditorGUILayout.PropertyField (list.FindProperty ("useBlurUIPanel"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultControlStateName"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("States", "window");
		GUILayout.Label ("Menu Opened\t\t" + list.FindProperty ("menuOpened").boolValue.ToString ());
		GUILayout.Label ("Current Control State Name\t" + list.FindProperty ("currentControlStateName").stringValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Audio Source List", "window", GUILayout.Height (50));
		showAudioSourceList (list.FindProperty ("audioSourceList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Players Mode List", "window", GUILayout.Height (50));
		showPlayersMode (list.FindProperty ("playersMode"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Control List", "window", GUILayout.Height (50));
		showPlayerControlList (list.FindProperty ("playerControlList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("eventToEnableComponents"));
		EditorGUILayout.PropertyField (list.FindProperty ("eevntToDisableComponents"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Components", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("currentPlayerModeText"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControlModeMenu"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentPlayerControlModeImage"));
		EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("grabManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("scannerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("gravityManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("combatManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("IKSystemManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("usingDevicesManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("overrideElementManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("headBobManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("damageInScreenManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("jetpackSystemManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("overrideElementManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("overrideElementManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("overrideElementManager"));

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showAudioSourceList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Source: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Source")) {
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
						showAudioSourceListElement (list.GetArrayElementAtIndex (i));
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

	void showAudioSourceListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));
		GUILayout.EndVertical ();
	}

	void showPlayersMode (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Modes: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Mode")) {
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
						showPlayersModeElement (list.GetArrayElementAtIndex (i));
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

	void showPlayersModeElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("nameMode"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activatePlayerModeEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivatePlayerModeEvent"));
		GUILayout.EndVertical ();
	}

	void showPlayerControlList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Control: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Control")) {
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
						showPlayerControlListElement (list.GetArrayElementAtIndex (i));
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

	void showPlayerControlListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateControlModeEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateControlModeEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("modeTexture"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidToSetRegularModeWhenActive"));
		GUILayout.EndVertical ();
	}
}
#endif