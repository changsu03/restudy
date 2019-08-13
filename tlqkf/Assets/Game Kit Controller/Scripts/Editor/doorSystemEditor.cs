using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(doorSystem))]
public class doorSystemEditor : Editor
{
	doorSystem manager;
	SerializedObject objectToUse;
	bool usesAnimation;

	bool expanded;
	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (doorSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Basic Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("movementType"));

		if (objectToUse.FindProperty ("movementType").enumValueIndex == 2) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("animationName"));
			usesAnimation = true;
		} else if (objectToUse.FindProperty ("movementType").enumValueIndex == 1) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("rotateInBothDirections"));
			usesAnimation = false;
		} else {
			usesAnimation = false;
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("openSound"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("closeSound"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("doorTypeInfo"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("doorState"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("openSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hologram"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("closeAfterTime"));
		if (objectToUse.FindProperty ("closeAfterTime").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToClose"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setMapIconsOnDoor"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("locked"));
		if (objectToUse.FindProperty ("locked").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("openDoorWhenUnlocked"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundOnUnlock"));
			if (objectToUse.FindProperty ("useSoundOnUnlock").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("unlockSound"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("unlockAudioSource"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Allowed Tag List", "window", GUILayout.Height (30));
		showAllowedTagList (objectToUse.FindProperty ("tagListToOpen"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Open/Close Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnOpenAndClose"));
		if (objectToUse.FindProperty ("useEventOnOpenAndClose").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("openEvent"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("closeEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Lock/Unlock Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnUnlockDoor"));
		if (objectToUse.FindProperty ("useEventOnUnlockDoor").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("evenOnUnlockDoor"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnLockDoor"));
		if (objectToUse.FindProperty ("useEventOnLockDoor").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnLockDoor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Door Found Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnDoorFound"));
		if (objectToUse.FindProperty ("useEventOnDoorFound").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnUnlockedDoorFound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnLockedDoorFound"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		if (!usesAnimation) {

			GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
			if (objectToUse.FindProperty ("showGizmo").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLenght"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLineLenght"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowAngle"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Doors List", "window", GUILayout.Height (30));
			showDoorList (objectToUse.FindProperty ("doorsInfo"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showDoorInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("doorMesh"));
		if (manager.movementType == doorSystem.doorMovementType.translate) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("openedPosition"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotatedPosition"));
		}
		GUILayout.EndVertical ();
	}

	void showDoorList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Doors: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Door")) {
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
						expanded = true;
						showDoorInfo (list.GetArrayElementAtIndex (i));
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

	void showAllowedTagList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tag")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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
}
#endif