using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add some buttons in the vehicle weapon script inspector
[CustomEditor (typeof(hoverBoardWayPoints))]
public class hoverBoardWayPointsEditor : Editor
{
	SerializedObject list;
	hoverBoardWayPoints points;

	GUIStyle style = new GUIStyle ();
	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		points = (hoverBoardWayPoints)target;
	}

	void OnSceneGUI ()
	{   
		if (points.showGizmo) {
			style.normal.textColor = points.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < points.wayPoints.Count; i++) {
				if (points.wayPoints [i].wayPoint) {
					currentWaypoint = points.wayPoints [i].wayPoint;

					Handles.Label (currentWaypoint.position + Vector3.up, (i + 1).ToString (), style);	
					if (points.useHandleForVertex) {
						Handles.color = points.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, points.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Hover Board Way Point Free Handle" + i.ToString ());
							currentWaypoint.position = newPoint;
						}   
					}

					if (points.showVertexHandles) {
						currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Hover Board Way Point Position Handle" + i.ToString ());
							currentWaypoint.position = oldPoint;
						}
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

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("wayPointElement"));
		EditorGUILayout.PropertyField (list.FindProperty ("movementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("moveInOneDirection"));
		EditorGUILayout.PropertyField (list.FindProperty ("triggerRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("extraRotation"));
		EditorGUILayout.PropertyField (list.FindProperty ("forceAtEnd"));
		EditorGUILayout.PropertyField (list.FindProperty ("railsOffset"));
		EditorGUILayout.PropertyField (list.FindProperty ("extraScale"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleTag"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Modify Speed Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("modifyMovementSpeedEnabled"));
		if (list.FindProperty ("modifyMovementSpeedEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("maxMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("minMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("modifyMovementSpeed"));
		}
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
		EditorGUILayout.PropertyField (list.FindProperty ("gizmoRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("useHandleForVertex"));
		EditorGUILayout.PropertyField (list.FindProperty ("handleRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("handleGizmoColor"));
		EditorGUILayout.PropertyField (list.FindProperty ("showVertexHandles"));	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("WayPoints List", "window", GUILayout.Height (30));
		showUpperList (list.FindProperty ("wayPoints"));
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
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("wayPoint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("direction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("trigger"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("railMesh"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				points.addNewWayPoint ();
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

			if (GUILayout.Button ("Rename Waypoints")) {
				points.renameAllWaypoints ();
			}

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
					points.removeWaypoint (i);
				}
				if (GUILayout.Button ("+")) {
					points.addNewWayPointAtIndex (i);
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