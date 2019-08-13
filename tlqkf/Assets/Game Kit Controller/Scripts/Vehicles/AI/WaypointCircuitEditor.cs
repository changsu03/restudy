using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(WaypointCircuit))]
public class WaypointCircuitEditor : Editor{
	WaypointCircuit manager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle();

	void OnEnable(){
		objectToUse = new SerializedObject(targets);
		manager = (WaypointCircuit)target;
	}

	void OnSceneGUI(){   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < manager.waypointList.Count; i++) {
				if (manager.waypointList [i]) {
					Handles.Label (manager.waypointList [i].position, manager.waypointList [i].name, style);
					if (manager.useHandleForVertex) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						Vector3 oldPoint = manager.waypointList [i].position;
						Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (manager.waypointList [i], "move waypoint circuit Handle");
							manager.waypointList [i].position = newPoint;
						}   
					}
				}
			}
		}
	}

	public override void OnInspectorGUI(){
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window",GUILayout.Height(30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("smoothRoute"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("editorVisualisationSubsteps"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window",GUILayout.Height(30));
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

		GUILayout.BeginVertical ("Waypoints List", "window",GUILayout.Height(30));
		showUpperList (objectToUse.FindProperty ("waypointList"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showUpperList(SerializedProperty list){
		EditorGUILayout.PropertyField(list);
		if (list.isExpanded){

			EditorGUILayout.Space();

			GUILayout.Label ("Number Of Waypoints: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Point")){
				manager.addNewWayPoint ();
			}
			if (GUILayout.Button("Clear")){
				list.arraySize=0;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			for (int i = 0; i < list.arraySize; i++){
				GUILayout.BeginHorizontal();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button("x")){
					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						Transform point = list.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
						DestroyImmediate (point.gameObject);
					}
					list.DeleteArrayElementAtIndex(i);
					list.DeleteArrayElementAtIndex(i);
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
					manager.addNewWayPointAtIndex (i);
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Rename Waypoints")){
				manager.renameWaypoints ();
			}
		}       
	}
}
#endif