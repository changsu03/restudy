using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inputManager))]
public class inputManagerEditor : Editor
{
	SerializedObject list;
	inputManager manager;
	string controlScheme;
	bool checkState;
	Color buttonColor;
	bool usingGamepad;
	string gamepadActive;
	bool actionEnabled;
	string isEnabled;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (inputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Input Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("editInputPanelPrefab"));
		EditorGUILayout.PropertyField (list.FindProperty ("editInputMenu"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentInputPanelListText"));
		EditorGUILayout.PropertyField (list.FindProperty ("showActionKeysOnMenu"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Controls Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("touchPanel"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Buttons Disabled At Start", "window");
		showButtonsToDisableAtStartList (list.FindProperty ("buttonsDisabledAtStart"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Touch Button List")) {
			manager.getTouchButtonListString ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Button List", "window");
		showTouchButtonList (list.FindProperty ("touchButtonList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Input Manager List", "window");
		showSimpleList (list.FindProperty ("playerInputManagerList"), "Player");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gamepad Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("ignoreGamepad"));
		EditorGUILayout.PropertyField (list.FindProperty ("allowKeyboardAndGamepad"));
		EditorGUILayout.PropertyField (list.FindProperty ("allowGamepadInTouchDevice"));
		EditorGUILayout.PropertyField (list.FindProperty ("checkConnectedGamepadRate"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("showKeyboardPressed"));
		EditorGUILayout.PropertyField (list.FindProperty ("showKeyboardPressedAction"));
		EditorGUILayout.PropertyField (list.FindProperty ("showGamepadPressed"));
		EditorGUILayout.PropertyField (list.FindProperty ("showGamepadPressedAction"));
		EditorGUILayout.PropertyField (list.FindProperty ("showPressedKeyWhenEditingInput"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input State", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usingGamepad"));
		EditorGUILayout.PropertyField (list.FindProperty ("onlyOnePlayer"));
		EditorGUILayout.PropertyField (list.FindProperty ("usingKeyBoard"));
		EditorGUILayout.PropertyField (list.FindProperty ("numberOfGamepads"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("loadOption"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multi Input List", "window");
		showMultiAxesList (list.FindProperty ("multiAxesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		//check the current controls enabled
		if (!checkState) {
			if (list.FindProperty ("useTouchControls").boolValue) {
				controlScheme = "Mobile";
			} else {
				controlScheme = "Keyboard";
			}
			checkState = true;
		}

		GUILayout.Label ("\nInput Scheme Options");
		//set the axes list in the inspector to the default value
		if (GUILayout.Button ("Save Current Input As Default")) {
			manager.saveCurrentInputAsDefault ();
		}

		if (GUILayout.Button ("Load Default Input")) {
			manager.setCurrentInputToDefault ();
		}

		//save the axes list in the inspector in a file
		if (GUILayout.Button ("Save Input To File")) {
			manager.saveButtonsInputToSaveFile ();
		}

		//set the axes list in the inspector to the values stored in a file
		if (GUILayout.Button ("Load Input From File")) {
			manager.loadButtonsInspectorFromSaveFile ();
		}

		//show the controls scheme
		GUILayout.Label ("\nCURRENT CONTROLS: " + controlScheme);
		//set the keyboard controls
		if (GUILayout.Button ("Set Keyboard Controls")) {
			manager.setKeyboardControls (true);
			controlScheme = "Keyboard";
		}

		//set the touch controls
		if (GUILayout.Button ("Set Touch Controls")) {
			manager.setKeyboardControls (false);
			controlScheme = "Mobile";
		}

		EditorGUILayout.Space ();

		usingGamepad = list.FindProperty ("usingGamepad").boolValue;
		gamepadActive = "NO";
		if (usingGamepad) {
			gamepadActive = "YES";
		}

		GUILayout.BeginVertical ("Gamepad Settings", "window");
		GUILayout.Label ("Using Gamepad: " + gamepadActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Keyboard Input", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("key"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Joystick Input", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("joystickButton"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Button Input", "window");
		if (manager != null && manager.touchButtonList.Count > 0 && manager.touchButtonListString.Length > 0) {
			list.FindPropertyRelative ("touchButtonIndex").intValue = EditorGUILayout.Popup ("Touch Buttonn", list.FindPropertyRelative ("touchButtonIndex").intValue, manager.touchButtonListString);
			if (list.FindPropertyRelative ("touchButtonIndex").intValue < manager.touchButtonListString.Length) {
				list.FindPropertyRelative ("touchButtonName").stringValue = manager.touchButtonListString [list.FindPropertyRelative ("touchButtonIndex").intValue];
			}
		}
		GUILayout.EndVertical ();
	
		GUILayout.EndVertical ();
	}

	void showMultiListElementInfo (SerializedProperty list, int axeListIndex)
	{
		GUILayout.BeginVertical ("Axes", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("axesName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), axeListIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int axeListIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Action")) {
				manager.addNewAxe (axeListIndex);
			}

			if (GUILayout.Button ("Clear")) {
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

					actionEnabled = list.GetArrayElementAtIndex (i).FindPropertyRelative ("actionEnabled").boolValue;
					isEnabled = " +";
					if (!actionEnabled) {
						isEnabled = " -";
					}
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + isEnabled));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showListElementInfo (list.GetArrayElementAtIndex (i));
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
					manager.removeAxesElement (axeListIndex, i);
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

	void showMultiAxesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Axes: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Axes List")) {
				manager.addNewAxesList ();
			}

			if (GUILayout.Button ("Clear")) {
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

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName));

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showMultiListElementInfo (list.GetArrayElementAtIndex (i), i);
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
		
	void showButtonsToDisableAtStartList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Button")) {
				list.arraySize++;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName));
						
					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label (listName + "s: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName));

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showTouchButtonList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Touch Button List: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName));

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}


}
#endif