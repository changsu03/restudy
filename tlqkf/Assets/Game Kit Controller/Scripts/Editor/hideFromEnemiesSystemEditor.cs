//using UnityEngine;
//using System.Collections;
//
//#if UNITY_EDITOR
//using UnityEditor;
//
//[CustomEditor (typeof(hideFromEnemiesSystem))]
//[CanEditMultipleObjects]
//public class hideFromEnemiesSystemEditor : Editor
//{
//	hideFromEnemiesSystem manager;
//	SerializedObject objectToUse;
//	GUIStyle style = new GUIStyle ();
//	Vector3 center;
//
//	void OnEnable ()
//	{
//		objectToUse = new SerializedObject (targets);
//		manager = (hideFromEnemiesSystem)target;
//	}
//
//	void OnSceneGUI ()
//	{   
//		if (!Application.isPlaying) {
//			if (manager.showGizmo) {
//
//				if (manager.vertexPosition.Count > 0) {
//					style.normal.textColor = manager.gizmoLabelColor;
//					style.alignment = TextAnchor.MiddleCenter;
//					for (int i = 0; i < manager.vertexPosition.Count; i++) {
//						if (manager.vertexPosition [i]) {
//							Handles.Label (manager.vertexPosition [i].transform.position, manager.vertexPosition [i].gameObject.name.ToString (), style);
//
//							if (manager.useHandleForVertex) {
//								Handles.color = manager.gizmoLabelColor;
//								EditorGUI.BeginChangeCheck ();
//
//								Vector3 oldPoint = manager.vertexPosition [i].position;
//								Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
//								if (EditorGUI.EndChangeCheck ()) {
//									Undo.RecordObject (manager.vertexPosition [i], "move hide vertex");
//									manager.vertexPosition [i].position = newPoint;
//								}   
//							}
//						}
//					}
//				}
//			}
//			string name = manager.gameObject.name.ToString ();
//			name = name.Substring (0, 3);
//			Handles.Label (manager.center, "Part\n" + name, style);
//		}
//	}
//
//	public override void OnInspectorGUI ()
//	{
//		if (objectToUse == null) {
//			return;
//		}
//		objectToUse.Update ();
//		GUILayout.BeginVertical ("box");
//
//		EditorGUILayout.Space ();
//
//		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerToCharacters"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("characterNeedToCrouch"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("characterCantMove"));
//		if (objectToUse.FindProperty ("characterCantMove").boolValue) {
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxMoveAmount"));
//		}
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hiddenForATime"));
//		if (objectToUse.FindProperty ("hiddenForATime").boolValue) {
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("hiddenForATimeAmount"));
//		}
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeDelayToHidenAgainIfDiscovered"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCharacterStateIcon"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("visibleCharacterStateName"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("notVisibleCharacterStateName"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerLayer"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerMaterial"));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("newPositionOffset"));
//		GUILayout.EndVertical ();
//
//		EditorGUILayout.Space ();
//
//		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (50));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
//		if (objectToUse.FindProperty ("showGizmo").boolValue) {
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("linesColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("materialColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cubeGizmoScale"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
//
//			EditorGUILayout.Space ();
//
//			GUILayout.BeginVertical ("Handle Vertex Settings", "window", GUILayout.Height (50));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForVertex"));
//			if (objectToUse.FindProperty ("useHandleForVertex").boolValue) {
//				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
//			}
//			GUILayout.EndVertical ();
//		}
//		GUILayout.EndVertical ();
//
//		EditorGUILayout.Space ();
//
//		GUILayout.BeginVertical ("Vertex Transform List", "window", GUILayout.Height (50));
//		showVertextPositionList (objectToUse.FindProperty ("vertexPosition"), "Vertex position");
//		GUILayout.EndVertical ();
//
//		EditorGUILayout.Space ();
//
//		if (!Application.isPlaying) {
//			if (GUILayout.Button ("Rename All Vertex")) {
//				manager.renameAllVertex ();
//			}
//
//			if (GUILayout.Button ("Remove Up Offset")) {
//				manager.removeUpOffset ();
//			}
//
//			if (GUILayout.Button ("Create Tile Element")) {
//				manager.createTileElement ();
//			}
//
//			EditorGUILayout.Space ();
//		}
//		GUILayout.EndVertical ();
//
//		if (GUI.changed) {
//			objectToUse.ApplyModifiedProperties ();
//		}
//	}
//
//	void showVertextPositionList (SerializedProperty list, string listName)
//	{
//		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
//		if (list.isExpanded) {
//
//			EditorGUILayout.Space ();
//
//			GUILayout.Label ("Number Of Vertex: \t" + list.arraySize.ToString ());
//
//			EditorGUILayout.Space ();
//
//			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("Add Vertex")) {
//				manager.addNewVertex (-1);
//			}
//			if (GUILayout.Button ("Clear")) {
//				manager.removeAllVertex ();
//			}
//			GUILayout.EndHorizontal ();
//
//			EditorGUILayout.Space ();
//
//			for (int i = 0; i < list.arraySize; i++) {
//				GUILayout.BeginHorizontal ();
//				if (i < list.arraySize && i >= 0) {
//					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
//				}
//				if (GUILayout.Button ("x")) {
//					manager.removeVertex (i);
//				}
//				if (GUILayout.Button ("+")) {
//					manager.addNewVertex (i);
//				}
//				GUILayout.EndHorizontal ();
//			}
//		}       
//	}
//}
//#endif