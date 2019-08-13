using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerCharactersManager))]
public class playerCharactersManagerEditor : Editor
{
	SerializedObject list;
	playerCharactersManager manager;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (playerCharactersManager)target;
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
		EditorGUILayout.PropertyField (list.FindProperty ("searchPlayersAtStart"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentNumberOfPlayers"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCharacter"));

		EditorGUILayout.PropertyField (list.FindProperty ("newPlayerPositionOffset"));
		EditorGUILayout.PropertyField (list.FindProperty ("regularReferenceResolution"));
		EditorGUILayout.PropertyField (list.FindProperty ("splitReferenceResolution"));

		EditorGUILayout.PropertyField (list.FindProperty ("cameraStateNameToUse"));
		if (manager.cameraStatesListString.Length > 0) {

			manager.currentCameraStateIndex = EditorGUILayout.Popup ("Camera State To Use", manager.currentCameraStateIndex, manager.cameraStatesListString);
			if (manager.currentCameraStateIndex >= 0) {
				manager.cameraStateNameToUse = manager.cameraStatesListString [manager.currentCameraStateIndex];
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Camera States")) {
			manager.getCameraStateListString ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player List", "window");
		showPlayerList (list.FindProperty ("playerList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera States List", "window");
		showCameraStatesList (list.FindProperty ("cameraStatesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Extra Character List", "window");
		showExtraCharacterListt (list.FindProperty ("extraCharacterList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Players On The Level")) {
			manager.searchPlayersOnTheLevel (false);
			manager.setExtraCharacterList ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Configure Players")) {
			manager.setNumberOfPlayers ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Camera Configuration")) {
			manager.setCameraConfiguration ();
		}
			
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Players ID")) {
			manager.setPlayerID ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Assign Map Systems On Map Creator")) {
			manager.assignMapSystem ();
		}

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showPlayerList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Players: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();
		
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Player")) {
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
						showPlayerListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showPlayerListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerParentGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerControllerGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerCameraGameObject"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerCameraManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseManager"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set As Current Player Active")) {
			if (Application.isPlaying) {
				manager.setAsCurrentPlayerActive (list.FindPropertyRelative ("Name").stringValue);
			}
		}

		GUILayout.EndVertical ();
	}

	void showCameraStatesList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Cameras: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Camera")) {
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
						showCameraStatesListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showCameraStatesListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberfOfPlayers"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Info List", "window");
		showCameraInfoList (list.FindPropertyRelative ("cameraInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showCameraInfoList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Cameras: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Camera")) {
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
						showCameraInfoListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showCameraInfoListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newX"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newY"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newW"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newH"));
		GUILayout.EndVertical ();
	}

	void showExtraCharacterListt (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Characters: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Character")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
			
				GUILayout.EndHorizontal ();
			}
		}
	}

}
#endif