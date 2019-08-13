using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerInputManager))]
public class playerInputManagerEditor : Editor
{
	SerializedObject list;
	playerInputManager manager;
	bool inputListOpened;
	Color defBackgroundColor;
	string[] currentStringList;
	int currentListIndex;

	bool actionEnabled;
	string isEnabled;
	bool currentlyActive;

	bool expandedMultiAxesList = false;
	bool expandedAxesList = false;
	bool expandedScreenActionsList = false;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (playerInputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("playerID"));
		EditorGUILayout.PropertyField (list.FindProperty ("showDebugActions"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mouseSensitivity"));
		EditorGUILayout.PropertyField (list.FindProperty ("useOnlyKeyboard"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Control Orientation Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useMovementOrientation"));
		if (list.FindProperty ("useMovementOrientation").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("horizontalMovementOrientation"));
			EditorGUILayout.PropertyField (list.FindProperty ("verticalMovementOrientation"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("useCameraOrientation"));
		if (list.FindProperty ("useCameraOrientation").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("horizontalCameraOrientation"));
			EditorGUILayout.PropertyField (list.FindProperty ("verticalCameraOrientation"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Controls Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("touchMovementControl"));
		EditorGUILayout.PropertyField (list.FindProperty ("touchCameraControl"));
		EditorGUILayout.PropertyField (list.FindProperty ("leftTouchSensitivity"));
		EditorGUILayout.PropertyField (list.FindProperty ("rightTouchSensitivity"));
		EditorGUILayout.PropertyField (list.FindProperty ("touchPanel"));

		EditorGUILayout.PropertyField (list.FindProperty ("usingTouchMovementJoystick"));
		EditorGUILayout.PropertyField (list.FindProperty ("inputLerpSpeedTouchMovementButtons"));

		EditorGUILayout.PropertyField (list.FindProperty ("useHorizontaTouchMovementButtons"));
		EditorGUILayout.PropertyField (list.FindProperty ("useVerticalTouchMovementButtons"));
		EditorGUILayout.PropertyField (list.FindProperty ("horizontalTouchMovementButtons"));
		EditorGUILayout.PropertyField (list.FindProperty ("verticalTouchMovementButtons"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input State", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("inputEnabled"));
		GUILayout.Label ("Input Active\t" + list.FindProperty ("inputCurrentlyActive").boolValue.ToString ());
		GUILayout.Label ("Override Input Active\t" + list.FindProperty ("overrideInputValuesActive").boolValue.ToString ());
		GUILayout.Label ("Input Paused\t" + list.FindProperty ("inputPaused").boolValue.ToString ());
		EditorGUILayout.PropertyField (list.FindProperty ("movementAxis"));
		EditorGUILayout.PropertyField (list.FindProperty ("mouseAxis"));
		EditorGUILayout.PropertyField (list.FindProperty ("rawMovementAxis"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Manual Control Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("manualControlActive"));
		if (list.FindProperty ("manualControlActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("manualMovementHorizontalInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("manualMovementVerticalInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("manualMouseHorizontalInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("manualMouseVerticalInput"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multi Axes List", "window", GUILayout.Height (30));

		EditorGUILayout.Space ();

		showMultiAxesList (list.FindProperty ("multiAxesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Screen Actions List", "window", GUILayout.Height (30));
		showScreenActionsList (list.FindProperty ("screenActionInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Element Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("input"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showMultiAxesListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		currentStringList = manager.multiAxesList [index].multiAxesStringList;
		if (currentStringList.Length > 0) {
			manager.multiAxesList [index].multiAxesStringIndex = EditorGUILayout.Popup ("Axe ", manager.multiAxesList [index].multiAxesStringIndex, currentStringList);
			currentListIndex = manager.multiAxesList [index].multiAxesStringIndex;
			if (currentListIndex >= 0) {
				manager.multiAxesList [index].axesName = currentStringList [currentListIndex];
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List", "window", GUILayout.Height (30));
		showAxesList (list.FindPropertyRelative ("axes"), index);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showMultiAxesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.Label ("Number Of Axes: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Axes")) {
				manager.addNewAxes ();
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Axes")) {
				manager.setMultiAxesEnabledState (true);
			}
			if (GUILayout.Button ("Disable All Axes")) {
				manager.setMultiAxesEnabledState (false);
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List")) {
				manager.updateMultiAxesList ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Current Values")) {
				manager.updateCurrentInputValues ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expandedMultiAxesList = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentlyActive = list.GetArrayElementAtIndex (i).FindPropertyRelative ("currentlyActive").boolValue;
					isEnabled = " +";
					if (!currentlyActive) {
						isEnabled = " -";
					}
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + isEnabled));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMultiAxesListElementInfo (list.GetArrayElementAtIndex (i), i);
						expandedMultiAxesList = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expandedMultiAxesList) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
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
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showAxesListElementInfo (SerializedProperty list, int multiAxesIndex, int axesIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));
		currentStringList = manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringList;
		if (currentStringList.Length > 0) {
			manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex = EditorGUILayout.Popup ("Axe ", 
				manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex, currentStringList);
			currentListIndex = manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex;
			if (currentListIndex >= 0 && axesIndex < manager.multiAxesList [multiAxesIndex].axes.Count && currentListIndex < currentStringList.Length) {
				manager.multiAxesList [multiAxesIndex].axes [axesIndex].Name = currentStringList [currentListIndex];
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonPressType"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedOnPausedGame"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonEvent"));
		GUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int multiAxesIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.Label ("Number Of Actions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				manager.addNewAction (multiAxesIndex);
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Actions")) {
				manager.setAllActionsEnabledState (multiAxesIndex, true);
			}
			if (GUILayout.Button ("Disable All Actions")) {
				manager.setAllActionsEnabledState (multiAxesIndex, false);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List")) {
				manager.updateAxesList (multiAxesIndex);
			}

			if (GUILayout.Button ("Set All Actions")) {
				manager.setAllAxesList (multiAxesIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expandedAxesList = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showAxesListElementInfo (list.GetArrayElementAtIndex (i), multiAxesIndex, i);
						expandedAxesList = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expandedAxesList) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
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
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showScreenActionsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.Label ("Number Of Actions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
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
				expandedScreenActionsList = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showScreenActionsListElementInfo (list.GetArrayElementAtIndex (i));
						expandedScreenActionsList = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expandedScreenActionsList) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
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
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showScreenActionsListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("screenActionName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("screenActionsGameObject"));
		GUILayout.EndVertical ();
	}
}
#endif