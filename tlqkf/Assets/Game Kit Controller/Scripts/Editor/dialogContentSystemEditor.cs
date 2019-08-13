using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(dialogContentSystem))]
public class dialogContentSystemEditor : Editor
{
	SerializedObject list;
	bool showElementSettings;

	Color buttonColor;
	dialogContentSystem manager;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (dialogContentSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("currentDialogIndex"));
		EditorGUILayout.PropertyField (list.FindProperty ("dialogOwner"));
		EditorGUILayout.PropertyField (list.FindProperty ("showDialogOnwerName"));
		EditorGUILayout.PropertyField (list.FindProperty ("dialogActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Complete Dialog List", "window", GUILayout.Height (50));
		showCompleteDialogInfoList (list.FindProperty ("completeDialogInfoList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showCompleteDialogInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Complete Dialogs: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Dialog")) {
				manager.addNewDialog ();
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
		
			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCompleteDialogInfoListElement (list.GetArrayElementAtIndex (i), i);
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


	void showCompleteDialogInfoListElement (SerializedProperty list, int dialogIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialogs List", "window", GUILayout.Height (50));
		showDialogInfoList (list.FindPropertyRelative ("dialogInfoList"), dialogIndex);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showDialogInfoList (SerializedProperty list, int dialogIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Dialogs: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Line")) {
				manager.addNewLine (dialogIndex);
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

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showDialogInfoListElement (list.GetArrayElementAtIndex (i), dialogIndex, i);
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

	void showDialogInfoListElement (SerializedProperty list, int dialogIndex, int lineIndex)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogOwnerName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogContent"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event System Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateRemoteTriggerSystem"));
		if (list.FindPropertyRelative ("activateRemoteTriggerSystem").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteTriggerName"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateWhenDialogClosed"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDialog"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Next UI Element Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNexLineButton"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEndOfDialog"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Next Line Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeToDialogInfoID"));

		if (list.FindPropertyRelative ("changeToDialogInfoID").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogInfoID"));
			if (list.FindPropertyRelative ("useRandomDialogInfoID").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogRange"));
				if (list.FindPropertyRelative ("useRandomDialogRange").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomDialogRange"));
				} else {
					GUILayout.BeginVertical ("Random Dialog Id List", "window", GUILayout.Height (50));
					showRandomDialogIDList (list.FindPropertyRelative ("randomDialogIDList"));
					GUILayout.EndVertical ();
				}
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivate"));
			}
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Disable Dialog Info Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableDialogAfterSelect"));
		if (list.FindPropertyRelative ("disableDialogAfterSelect").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToJump"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog Lines List", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showPreviousDialogLineOnOptions"));

		EditorGUILayout.Space ();

		showDialogLineInfoList (list.FindPropertyRelative ("dialogLineInfoList"), dialogIndex, lineIndex);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showDialogLineInfoList (SerializedProperty list, int dialogIndex, int lineIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Lines: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Answer")) {
				manager.addnewAnswer (dialogIndex, lineIndex);
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

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showDialogLineInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showDialogLineInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogLineContent"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Next Line Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogInfoID"));
		if (list.FindPropertyRelative ("useRandomDialogInfoID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogRange"));
			if (list.FindPropertyRelative ("useRandomDialogRange").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomDialogRange"));
			} else {
				GUILayout.BeginVertical ("Random Dialog Id List", "window", GUILayout.Height (50));
				showRandomDialogIDList (list.FindPropertyRelative ("randomDialogIDList"));
				GUILayout.EndVertical ();
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivate"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event System Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateRemoteTriggerSystem"));
		if (list.FindPropertyRelative ("activateRemoteTriggerSystem").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteTriggerName"));
		}
			
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Disable Line Info Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableLineAfterSelect"));
		if (list.FindPropertyRelative ("disableLineAfterSelect").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lineDisabled"));	
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showRandomDialogIDList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of ID: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add ID")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

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
		GUILayout.EndVertical ();
	}
}
#endif