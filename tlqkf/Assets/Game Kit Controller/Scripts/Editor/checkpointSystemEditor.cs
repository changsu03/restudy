using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(checkpointSystem))]
public class checkpointSystemEditor : Editor
{
	checkpointSystem manager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		manager = (checkpointSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < manager.checkPointList.Count; i++) {
				if (manager.checkPointList [i]) {
					Handles.Label (manager.checkPointList [i].position, (i + 1).ToString (), style);	
					if (manager.useHandleForVertex) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						Vector3 oldPoint = manager.checkPointList [i].position;
						Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (manager.checkPointList [i], "move Checkpoint Handle " + i.ToString ());
							manager.checkPointList [i].position = newPoint;
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
		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("checkpointSceneID"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("checkPointPrefab"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerToPlaceNewCheckpoints"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("checkpointsPositionOffset"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerScale"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Respawn Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("deathLoackCheckpointType"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerToRespawn"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentCheckpointElement"));
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

		GUILayout.BeginVertical ("Checkpoints List", "window", GUILayout.Height (30));
		showCheckpointsList (objectToUse.FindProperty ("checkPointList"));
		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showCheckpointsList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Checkpoint")) {
				manager.addNewCheckpoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllCheckpoints ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					manager.removeCheckpoint (i);
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
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif
