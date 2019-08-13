using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(overrideInputManager))]
public class overrideInputManagerEditor : Editor
{
	SerializedObject list;
	overrideInputManager manager;

	bool inputListOpened;
	Color defBackgroundColor;
	string[] currentStringList;
	int currentListIndex;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (overrideInputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("inputEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("isPlayerController"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
		EditorGUILayout.PropertyField (list.FindProperty ("showDebugActions"));
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("destroyObjectOnStopOverride"));
		if (list.FindProperty ("destroyObjectOnStopOverride").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("eventToDestroy"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("startOverrideFunction"));
		EditorGUILayout.PropertyField (list.FindProperty ("stopOverrideFunction"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pre Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("usePreOverrideFunctions"));
		EditorGUILayout.PropertyField (list.FindProperty ("preStartOverrideFunction"));
		EditorGUILayout.PropertyField (list.FindProperty ("preStopOverrideFunction"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("activateActionScreen"));
		if (list.FindProperty ("activateActionScreen").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("actionScreenName"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multi Axes List", "window", GUILayout.Height (30));
		showMultiAxesList (list.FindProperty ("multiAxesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Override Camera Controller", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("overrideCameraControllerManager"));
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

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMultiAxesListElementInfo (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showAxesListElementInfo (list.GetArrayElementAtIndex (i), multiAxesIndex, i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
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
}
#endif