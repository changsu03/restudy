using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapTileBuilder))]
[CanEditMultipleObjects]
public class mapTileBuilderEditor : Editor
{
	mapTileBuilder builder;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	Vector3 center;
	Vector3 currentVertexPosition;
	Quaternion currentVertexRotation;
	Transform currentVertex;

	Vector3 oldPoint;
	Vector3 newPoint;
	float distance;
	Vector3 snapValue = new Vector3 (.25f, .25f, .25f);
	string currentName;

	string eventTriggerAdded;
	string textMeshAdded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		builder = (mapTileBuilder)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (builder.showGizmo && builder.mapManager.showMapPartsGizmo) {
				if (builder.eventTriggerList.Count > 0) {
					style.normal.textColor = builder.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					if (builder.showEnabledTrigger && builder.mapManager.showMapPartEnabledTrigger) {
						for (int i = 0; i < builder.eventTriggerList.Count; i++) {
							if (builder.eventTriggerList [i]) {
								Handles.Label (builder.eventTriggerList [i].transform.position, "Event\n Trigger " + (i + 1).ToString (), style);
								if (builder.useHandleForVertex) {
									Handles.color = builder.gizmoLabelColor;
									EditorGUI.BeginChangeCheck ();

									oldPoint = builder.eventTriggerList [i].transform.position;
									newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, builder.handleRadius, snapValue, Handles.CircleHandleCap);
									if (EditorGUI.EndChangeCheck ()) {
										Undo.RecordObject (builder.eventTriggerList [i].transform, "move Trigger" + i.ToString ());
										builder.eventTriggerList [i].transform.position = newPoint;
									}   
								}
							}
						}
					}
				}

				if (builder.verticesPosition.Count > 0) {
					style.normal.textColor = builder.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;
					for (int i = 0; i < builder.verticesPosition.Count; i++) {
						if (builder.verticesPosition [i]) {
							currentVertex = builder.verticesPosition [i].transform;
							currentVertexPosition = currentVertex.position;

							Handles.Label (currentVertexPosition, currentVertex.name.ToString (), style);

							if (builder.showVerticesDistance) {
								if (i + 1 < builder.verticesPosition.Count) {
									if (builder.verticesPosition [i + 1] != null) {
										center = Vector3.zero;
										center += currentVertexPosition;
										center += builder.verticesPosition [i + 1].position;
										center /= 2;
										distance = GKC_Utils.distance (currentVertexPosition, builder.verticesPosition [i + 1].position);
										Handles.Label (center, distance.ToString () + " m", style);
									}

								}
								if (i == builder.verticesPosition.Count - 1) {
									if (builder.verticesPosition [0] != null) {
										center = Vector3.zero;
										center += currentVertexPosition;
										center += builder.verticesPosition [0].position;
										center /= 2;
										distance = GKC_Utils.distance (currentVertexPosition, builder.verticesPosition [0].position);
										Handles.Label (center, distance.ToString () + " m", style);
									}
								}
							}

							if (builder.useHandleForVertex) {
								Handles.color = builder.gizmoLabelColor;
								EditorGUI.BeginChangeCheck ();

								oldPoint = currentVertexPosition;
								newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, builder.handleRadius, snapValue, Handles.CircleHandleCap);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (currentVertex, "move Handle" + i.ToString ());
									currentVertex.transform.position = newPoint;
								}   
							}

							if (builder.showVertexHandles || builder.mapManager.showVertexHandles) {
								currentVertexRotation = Tools.pivotRotation == PivotRotation.Local ? currentVertex.rotation : Quaternion.identity;
								//Handles.DoPositionHandle (currentVertexPosition, currentVertexRotation);

								EditorGUI.BeginChangeCheck ();

								oldPoint = currentVertex.position;
								oldPoint = Handles.DoPositionHandle (oldPoint, currentVertexRotation);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (currentVertex, "move Vertex" + i.ToString ());
									currentVertex.position = oldPoint;
								}
							}
						}
					}
				}
			}
			currentName = builder.gameObject.name.ToString ();
			currentName = currentName.Substring (0, 3);
			Handles.Label (builder.center, "Part\n" + currentName, style);

			if (builder.generate3dMapPartMesh) {
				Handles.Label (builder.center + builder.mapPart3dOffset + builder.mapPart3dHeight * Vector3.up, "3d height\n" + currentName, style);
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

		GUILayout.BeginVertical ("Map Part State", "window", GUILayout.Height (30));
		eventTriggerAdded = "NO";
		if (objectToUse.FindProperty ("eventTriggerList").arraySize > 0) {
			eventTriggerAdded = "YES";
		}
		GUILayout.Label ("Event Trigger Added\t\t" + eventTriggerAdded);
		textMeshAdded = "NO";
		if (objectToUse.FindProperty ("textMeshList").arraySize > 0) {
			textMeshAdded = "YES";
		}

		GUILayout.Label ("Text Mesh Added\t\t" + textMeshAdded);
		GUILayout.Label ("Map Tile Created\t\t" + objectToUse.FindProperty ("mapTileCreated").boolValue.ToString());

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartParent"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartBuildingIndex"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartFloorIndex"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartIndex"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartRendererOffset"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("newPositionOffset"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartEnabled"));
		if (!objectToUse.FindProperty ("mapPartEnabled").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useOtherColorIfMapPartDisabled"));
			if (objectToUse.FindProperty ("useOtherColorIfMapPartDisabled").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("colorIfMapPartDisabled"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showEnabledTrigger"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showVerticesDistance"), new GUIContent ("Show Vertex Distance"), false);
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapLinesColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartMaterialColor"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set Random Color")) {
				builder.setRandomMapPartColor ();
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cubeGizmoScale"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showVertexHandles"));
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Handle Vertex Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForVertex"));
			if (objectToUse.FindProperty ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rename Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPartName"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Map Part")) {
			builder.renameMapPart ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Transform List", "window", GUILayout.Height (50));
		showVertextPositionList (objectToUse.FindProperty ("verticesPosition"), "Vertex position");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Text Mesh List", "window", GUILayout.Height (50));
		showTextMeshList (objectToUse.FindProperty ("textMeshList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!Application.isPlaying) {
			if (GUILayout.Button ("Rename All Vertex")) {
				builder.renameAllVertex ();
			}

			if (GUILayout.Button ("Reverse Vertex Order")) {
				builder.reverVertexOrder ();
			}

			if (GUILayout.Button ("Add New Map Part")) {
				builder.mapManager.addNewMapPart (builder.mapPartParent);
			}

			if (GUILayout.Button ("Duplicate Map Part")) {
				builder.mapManager.duplicateMapPart (builder.mapPartParent, builder.gameObject);
			}

			if (GUILayout.Button ("Remove Map Part")) {
				builder.mapManager.removeMapPart (builder.mapPartParent, builder.gameObject);
				return;
			}

			if (GUILayout.Button ("Add Trigger Event to Enable Map Part")) {
				builder.addEventTriggerToActive ();
			}

			EditorGUILayout.Space ();

			if (objectToUse.FindProperty ("eventTriggerList").arraySize > 0) {
				GUILayout.BeginVertical ("Event Trigger List", "window", GUILayout.Height (50));
				showEventTriggerList (objectToUse.FindProperty ("eventTriggerList"), "Event Trigger List");
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();
		}
			
		GUILayout.BeginVertical ("Extra Map Parts To Active", "window", GUILayout.Height (50));
		showExtraMapPartsToActive (objectToUse.FindProperty ("extraMapPartsToActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Part 3d Mesh Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("generate3dMapPartMesh"));
		if (objectToUse.FindProperty ("generate3dMapPartMesh").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("onlyUse3dMapPartMesh"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("generate3dMeshesShowGizmo"));
			GUILayout.Label ("Map Part 3d Mesh Created: \t" + objectToUse.FindProperty ("mapPart3dMeshCreated").boolValue);
	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPart3dHeight"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPart3dOffset"));
			if (objectToUse.FindProperty ("mapPart3dMeshCreated").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("mapPart3dGameObject"));

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Enable 3d Mesh")) {
					builder.enableOrDisableMapPart3dMesh (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable 3d Mesh")) {
					builder.enableOrDisableMapPart3dMesh (false);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Update Mesh Position")) {
					builder.updateMapPart3dMeshPositionFromEditor ();
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Generate 3d Mesh")) {
				builder.generateMapPart3dMeshFromEditor ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Remove 3d Mesh")) {
				builder.removeMapPart3dMesh ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showVertextPositionList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Vertex: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Vertex")) {
				builder.addNewVertex (-1);
			}
			if (GUILayout.Button ("Clear")) {
				builder.removeAllVertex ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					builder.removeVertex (i);
				}
				if (GUILayout.Button ("+")) {
					builder.addNewVertex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}


	void showEventTriggerList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Triggers: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();
			
			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Event Trigger")) {
				builder.addEventTriggerToActive ();
			}
			if (GUILayout.Button ("Clear")) {
				builder.removeAllEventTriggers ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Enable Event Triggers")) {
				builder.enableOrDisableEventTriggerList (true);
			}
			if (GUILayout.Button ("Disable Event Triggers")) {
				builder.enableOrDisableEventTriggerList (false);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					builder.removeEventTrigger (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showTextMeshList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of TextMesh: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Text Mesh")) {
				builder.addMapPartTextMesh ();
			}
			if (GUILayout.Button ("Clear")) {
				builder.removeAllTextMesh ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					builder.removeTextMesh (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showExtraMapPartsToActive (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Extra Parts: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Extra Part")) {
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