using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(elevatorSystem))]
public class elevatorSystemEditor : Editor
{
	elevatorSystem elevator;
	SerializedObject list;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		elevator = (elevatorSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (elevator.showGizmo) {
				style.normal.textColor = elevator.gizmoLabelColor;
				for (int i = 0; i < elevator.floors.Count; i++) {
					if (elevator.floors [i].floorPosition) {
						Handles.Label (elevator.floors [i].floorPosition.position, elevator.floors [i].name + " - " + elevator.floors [i].floorNumber, style);
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("currentFloor"));
		EditorGUILayout.PropertyField (list.FindProperty ("speed"));
		EditorGUILayout.PropertyField (list.FindProperty ("hasInsideElevatorDoor"));
		if (list.FindProperty ("hasInsideElevatorDoor").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("insideElevatorDoor"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("addSwitchInNewFloors"));
		if (list.FindProperty ("addSwitchInNewFloors").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("elevatorSwitchPrefab"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("addDoorInNewFloors"));
		if (list.FindProperty ("addDoorInNewFloors").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("elevatorDoorPrefab"));
		}
	
		EditorGUILayout.PropertyField (list.FindProperty ("doorsClosed"));
		EditorGUILayout.PropertyField (list.FindProperty ("changeIconFloorWhenMoving"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Elevator State", "window");		
		GUILayout.Label ("Moving\t " + list.FindProperty ("moving").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floors List", "window");
		showUpperList (list.FindProperty ("floors"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasFloorButton"));
		if (list.FindPropertyRelative ("hasFloorButton").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorButton"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasOutSideElevatorDoor"));
		if (list.FindPropertyRelative ("hasOutSideElevatorDoor").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("outsideElevatorDoor"));
		}
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Floors: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
				elevator.addNewFloor ();
			}
			if (GUILayout.Button ("Clear List")) {
				elevator.removeAllFloors ();
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
				expanded = false;
				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i));
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
					elevator.removeFloor (i);
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
}
#endif