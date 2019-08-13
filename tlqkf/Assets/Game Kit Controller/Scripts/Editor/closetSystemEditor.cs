using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(closetSystem))]
[CanEditMultipleObjects]
public class closetSystemeEditor : Editor
{
	closetSystem manager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	Vector3 center;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (closetSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				if (manager.closetDoorList.Count > 0) {
					style.normal.textColor = manager.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					for (int i = 0; i < manager.closetDoorList.Count; i++) {
						if (manager.closetDoorList [i].doorTransform) {
							Handles.Label (manager.closetDoorList [i].doorTransform.position, manager.closetDoorList [i].Name, style);
						}
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("openType"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSounds"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Door List", "window", GUILayout.Height (50));
		showClosetDoorList (objectToUse.FindProperty ("closetDoorList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLenght"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLineLenght"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowAngle"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));	
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showDoorInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("doorTransform"));
		if (manager.openType == closetSystem.doorOpenType.translate) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("openedPosition"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotatedPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCloseRotationTransform"));
			if (list.FindPropertyRelative ("useCloseRotationTransform").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("closeRotationTransform"));
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeOpened"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeClosed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("opened"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("closed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects On Door List", "window", GUILayout.Height (50));
		showSimpleList (list.FindPropertyRelative ("objectsInDoor"), "Number Of Objects: \t");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Doors List", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("onlyOneDoor"));
		if (!list.FindPropertyRelative ("onlyOneDoor").boolValue) {
			showSimpleList (list.FindPropertyRelative ("othersDoors"), "Number Of Other Doors: \t");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openSound"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("closeSound"));
		GUILayout.EndVertical ();


		GUILayout.EndVertical ();
	}

	void showClosetDoorList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Doors: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Door")) {
				manager.addNewDoor ();
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

	void showSimpleList (SerializedProperty list, string infoText)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label (infoText + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

}
#endif