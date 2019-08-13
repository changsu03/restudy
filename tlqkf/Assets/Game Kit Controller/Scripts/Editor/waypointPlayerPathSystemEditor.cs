using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(waypointPlayerPathSystem))]
public class waypointPlayerPathSystemEditor : Editor
{
	waypointPlayerPathSystem pathManager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	bool advancedSettings;
	Color buttonColor;
	mapObjectInformation currentMapObjectInformation;
	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		pathManager = (waypointPlayerPathSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (pathManager.showGizmo) {
				style.normal.textColor = pathManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				for (int i = 0; i < pathManager.wayPoints.Count; i++) {
					currentWaypoint = pathManager.wayPoints [i].point;

					if (currentWaypoint) {
						string label = "Point: " + pathManager.wayPoints [i].Name;

						currentMapObjectInformation = currentWaypoint.GetComponent<mapObjectInformation> ();

						if (pathManager.showInfoLabel) {
							label += "\n Radius: ";
							if (pathManager.useRegularGizmoRadius) {
								label += pathManager.triggerRadius;
							} else {
								label += pathManager.wayPoints [i].triggerRadius;
							}

							label += "\n Show OffScreen Icon: ";

							if (currentMapObjectInformation.showOffScreenIcon) {
								label += "On";
							} else {
								label += "Off";
							}

							label += "\n Show Map Icon: ";
							if (currentMapObjectInformation.showMapWindowIcon) {
								label += "On";
							} else {
								label += "Off";
							}

							label += "\n Show Distance: ";
							if (currentMapObjectInformation.showDistance) {
								label += "On";
							} else {
								label += "Off";
							}
						}

						Handles.Label (currentWaypoint.position + currentWaypoint.up * currentMapObjectInformation.triggerRadius, label, style);

						currentMapObjectInformation.showGizmo = pathManager.showGizmo;
						currentMapObjectInformation.showOffScreenIcon = pathManager.showOffScreenIcon;
						currentMapObjectInformation.showMapWindowIcon = pathManager.showMapWindowIcon;
						currentMapObjectInformation.showDistance = pathManager.showDistance;

						if (pathManager.useRegularGizmoRadius) {
							currentMapObjectInformation.triggerRadius = pathManager.triggerRadius;
						} else {
							currentMapObjectInformation.triggerRadius = pathManager.wayPoints [i].triggerRadius;
						}

						if (pathManager.useHandleForVertex) {
							Handles.color = pathManager.handleGizmoColor;
							EditorGUI.BeginChangeCheck ();

							oldPoint = currentWaypoint.position;
							newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, pathManager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
							if (EditorGUI.EndChangeCheck ()) {
								Undo.RecordObject (currentWaypoint, "move Player Path Way Point Handle " + i.ToString());
								currentWaypoint.position = newPoint;
							}   
						}

						if (pathManager.showDoPositionHandles) {
							currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

							EditorGUI.BeginChangeCheck ();

							oldPoint = currentWaypoint.position;
							oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
							if (EditorGUI.EndChangeCheck ()) {
								Undo.RecordObject (currentWaypoint, "move Player Path Way Point Do Position Handle" + i.ToString ());
								currentWaypoint.position = oldPoint;
							}
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

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("inOrder"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showOneByOne"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerRadius"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showOffScreenIcon"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showMapWindowIcon"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showDistance"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("pathActive"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events To Call Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnPathComplete"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnPathIncomplete"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		string inputListOpenedText = "";
		if (advancedSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Advanced Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Advanced Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			advancedSettings = !advancedSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndHorizontal ();

		if (advancedSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure timer options and functions to call once all the path is complete", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useTimer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timerSpeed"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minutesToComplete"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondsToComplete"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("extraTimePerPoint"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pathCompleteAudioSound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pathUncompleteAudioSound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondTimerSound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondSoundTimerLowerThan"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("pointReachedSound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useLineRenderer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("lineRendererColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("lineRendererWidth"));

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showInfoLabel"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useRegularGizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForVertex"));
			if (objectToUse.FindProperty ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleGizmoColor"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showDoPositionHandles"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoints List", "window", GUILayout.Height (30));
		showUpperList (objectToUse.FindProperty ("wayPoints"));
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Rename WayPoints")) {
			pathManager.renamePoints ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("point"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("triggerRadius"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reached"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.Label ("Number Of Points: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.Label ("Reached points: \t" + objectToUse.FindProperty ("pointsReached").intValue);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				pathManager.addNewWayPoint ();
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
					Transform point = list.GetArrayElementAtIndex (i).FindPropertyRelative ("point").objectReferenceValue as Transform;
					DestroyImmediate (point.gameObject);
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