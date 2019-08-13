using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(cameraWaypointSystem))]
public class cameraWaypointSystemEditor : Editor
{
	cameraWaypointSystem manager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();

	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (cameraWaypointSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < manager.waypointList.Count; i++) {
				if (manager.waypointList [i].waypointTransform) {

					currentWaypoint = manager.waypointList [i].waypointTransform;

					Handles.Label (currentWaypoint.position, (i + 1).ToString (), style);	
					if (manager.useHandleForWaypoints) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Camera Way Point Handle " + i.ToString());
							currentWaypoint.position = newPoint;
						}   
					}

					if (manager.showWaypointHandles) {
						currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Camera Way point Free Handle" + i.ToString ());
							currentWaypoint.position = oldPoint;
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

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentCameraTransform"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("waitTimeBetweenPoints"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("movementSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("rotationSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnEnd"));
		if (objectToUse.FindProperty ("useEventOnEnd").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnEnd"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bezier Path Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useBezierCurve"));
		if (objectToUse.FindProperty ("useBezierCurve").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("spline"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("bezierDuration"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForWaypoints"));
			if (objectToUse.FindProperty ("useHandleForWaypoints").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleGizmoColor"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showWaypointHandles"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoints List", "window", GUILayout.Height (30));
		showWaypointList (objectToUse.FindProperty ("waypointList"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();

	}

	void showWaypointList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				manager.addNewWayPoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllWaypoints  ();
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
						showWaypointListElementInfo (list.GetArrayElementAtIndex (i));
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
					manager.removeWaypoint (i);
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
				if (GUILayout.Button ("+")) {
					manager.addNewWayPoint (i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Rename Waypoints")) {
				manager.renameAllWaypoints ();
			}
		}       
	}

	void showWaypointListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotateCameraToNextWaypoint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePointToLook"));
		if (list.FindPropertyRelative ("usePointToLook").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pointToLook"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothTransitionToNextPoint"));
		if (list.FindPropertyRelative ("smoothTransitionToNextPoint").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomMovementSpeed"));
			if (list.FindPropertyRelative ("useCustomMovementSpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("movementSpeed"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomRotationSpeed"));
			if (list.FindPropertyRelative ("useCustomRotationSpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationSpeed"));
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeOnFixedPosition"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomWaitTimeBetweenPoint"));
		if (list.FindPropertyRelative ("useCustomWaitTimeBetweenPoint").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeBetweenPoints"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPointReached"));
		if (list.FindPropertyRelative ("useEventOnPointReached").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPointReached"));
		}

		GUILayout.EndVertical ();
	}
}
#endif