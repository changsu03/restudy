﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(AIWayPointPatrol))]
public class AIWayPointPatrolEditor : Editor
{
	AIWayPointPatrol patrol;
	SerializedObject objectToUse;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		patrol = (AIWayPointPatrol)target;
	}

	void OnSceneGUI ()
	{   
		if (patrol.showGizmo) {
			for (int i = 0; i < patrol.patrolList.Count; i++) {
				GUIStyle style = new GUIStyle ();
				style.normal.textColor = patrol.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				if (patrol.patrolList [i].patrolTransform) {
					Handles.Label (patrol.patrolList [i].patrolTransform.position, ("Patrol " + (i + 1).ToString ()), style);
				}
				for (int j = 0; j < patrol.patrolList [i].wayPoints.Count; j++) {
					if (patrol.patrolList [i].wayPoints [j]) {
						Handles.Label (patrol.patrolList [i].wayPoints [j].position, (j + 1).ToString (), style);	
					}
					if (patrol.useHandleForVertex) {
						Handles.color = patrol.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						Vector3 oldPoint = patrol.patrolList [i].wayPoints [j].position;
						Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, patrol.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (patrol.patrolList [i].wayPoints [j], "move Patrol Point Handle");
							patrol.patrolList [i].wayPoints [j].position = newPoint;
						}   
					}
				}
				if (patrol.useHandleForVertex) {
					Handles.color = patrol.handleGizmoColor;
					EditorGUI.BeginChangeCheck ();

					Vector3 oldPoint = patrol.patrolList [i].patrolTransform.position;
					Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, patrol.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (patrol.patrolList [i].patrolTransform, "move Patrol Parent Handle");
						patrol.patrolList [i].patrolTransform.position = newPoint;
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

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("waitTimeBetweenPoints"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("movingForward"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerMask"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("newWaypointOffset"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("surfaceAdjusmentOffset"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForVertex"));
			if (objectToUse.FindProperty ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleGizmoColor"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Patrol List", "window", GUILayout.Height (30));
		showPatrolList (objectToUse.FindProperty ("patrolList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Adjust To Surface")) {
			patrol.adjustWayPoints ();
		}
//		if (GUILayout.Button ("Invert Path")) {
//			patrol.invertPath ();
//		}
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showPatrolInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("patrolTransform"));
		GUILayout.BeginVertical ("WayPoint List", "window");
		showWayPointList (list.FindPropertyRelative ("wayPoints"), index);
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showWayPointList (SerializedProperty list, int index)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of WayPoints: " + list.arraySize.ToString ());
			EditorGUILayout.Space ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				patrol.addNewWayPoint (index);
			}
			if (GUILayout.Button ("Clear List")) {
				patrol.clearWayPoint (index);
			}
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						Transform point = list.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
						DestroyImmediate (point.gameObject);
					}
					list.DeleteArrayElementAtIndex (i);
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
		}       
	}

	void showPatrolList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Patrols: " + list.arraySize.ToString ());
			EditorGUILayout.Space ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Patrol")) {
				patrol.addNewPatrol ();
			}
			if (GUILayout.Button ("Clear List")) {
				patrol.clearPatrolList ();
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
						showPatrolInfo (list.GetArrayElementAtIndex (i), i);
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
					patrol.clearWayPoint (i);
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
}
#endif