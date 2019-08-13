using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(weaponAttachmentSystem))]
public class weaponAttachmentSystemEditor : Editor
{
	SerializedObject list;
	weaponAttachmentSystem manager;
	bool showElementSettings;

	Color buttonColor;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (weaponAttachmentSystem)target;
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
		EditorGUILayout.PropertyField (list.FindProperty ("useOffsetPanels"));
		EditorGUILayout.PropertyField (list.FindProperty ("canChangeAttachmentWithNumberKeys"));
		EditorGUILayout.PropertyField (list.FindProperty ("thirdPersonCameraMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("canEditWeaponWithoutAttchments"));
		EditorGUILayout.PropertyField (list.FindProperty ("useSmoothTransitionFreeCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("useSmoothTransitionLockedCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("setPickedAttachments"));
		EditorGUILayout.PropertyField (list.FindProperty ("startEditWeaponSound"));
		EditorGUILayout.PropertyField (list.FindProperty ("stopEditWeaponSound"));
		EditorGUILayout.PropertyField (list.FindProperty ("UILinesScaleMultiplier"));
		EditorGUILayout.PropertyField (list.FindProperty ("dualWeaponOffsetScale"));
		EditorGUILayout.PropertyField (list.FindProperty ("disableHUDWhenEditingAttachments"));
		EditorGUILayout.PropertyField (list.FindProperty ("showCurrentAttachmentHoverInfo"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment State", "window", GUILayout.Height (50));
		GUILayout.Label ("Editing Attachments\t " + list.FindProperty ("editingAttachments").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		showElementSettings = list.FindProperty ("showElementSettings").boolValue;

		EditorGUILayout.BeginVertical ();
		string inputListOpenedText = "";
		if (showElementSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showElementSettings = !showElementSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		list.FindProperty ("showElementSettings").boolValue = showElementSettings;

		if (showElementSettings) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Element Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));	
			EditorGUILayout.PropertyField (list.FindProperty ("weaponSystem"));	
			EditorGUILayout.PropertyField (list.FindProperty ("IKWeaponManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("attachmentInfoGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("attachmentSlotGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponAttachmentsMenu"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsCamera"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainAudioSource"));
			EditorGUILayout.PropertyField (list.FindProperty ("removeAttachmentSound"));
			EditorGUILayout.PropertyField (list.FindProperty ("attachmentHoverInfoPanelGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("attachmentHoverInfoText"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment List", "window", GUILayout.Height (50));
		showAttachmentPlaceList (list.FindProperty ("attachmentInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		EditorGUILayout.PropertyField (list.FindProperty ("showDualWeaponsGizmo"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showAttachmentPlaceListElement (SerializedProperty list, int attachmentPlaceIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentPlaceEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("noAttachmentText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentPlaceTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("offsetAttachmentPlaceTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponOffsetAttachmentPlaceTransform"));	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("offsetAttachmentPointTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentAttachmentSelectedIndex"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Replace List", "window", GUILayout.Height (50));
		showObjectsToReplaceList (list.FindPropertyRelative ("objectToReplace"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments List", "window", GUILayout.Height (50));
		showAttachmentList (list.FindPropertyRelative ("attachmentPlaceInfoList"), attachmentPlaceIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showAttachmentPlaceList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Attachment Places: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Place")) {
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Places")) {
				manager.enableOrDisableAllAttachmentPlaces (true);
			}

			if (GUILayout.Button ("Disable All Places")) {
				manager.enableOrDisableAllAttachmentPlaces (false);
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
						showAttachmentPlaceListElement (list.GetArrayElementAtIndex (i), i);
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

	void showObjectsToReplaceList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Objects: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize--;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showAttachmentList (SerializedProperty list, int attachmentPlaceIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Attachment: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Attachment")) {
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Attachments")) {
				manager.enableOrDisableAllAttachment (true, attachmentPlaceIndex);
			}

			if (GUILayout.Button ("Disable All Attachments")) {
				manager.enableOrDisableAllAttachment (false, attachmentPlaceIndex);
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
						showAttachmentListElement (list.GetArrayElementAtIndex (i));
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

	void showAttachmentListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("onlyEnabledWhileCarrying"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentUseHUD"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("selectAttachmentSound"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments Event Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateEvent"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPress"));
		if (list.FindPropertyRelative ("useEventOnPress").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPress"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPressDown"));
		if (list.FindPropertyRelative ("useEventOnPressDown").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressDown"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPressUp"));
		if (list.FindPropertyRelative ("useEventOnPressUp").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressUp"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventHandPosition"));
		if (list.FindPropertyRelative ("useEventHandPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateEventHandPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateEventHandPosition"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments Hover Info Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAttachmentHoverInfo"));
		if (list.FindPropertyRelative ("useAttachmentHoverInfo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentHoverInfo"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}
}
#endif