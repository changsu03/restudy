using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapCreator))]
public class mapCreatorEditor : Editor
{
	SerializedObject list;
	mapCreator map;
	bool shapeChangedSinceLastRepaint;
	bool generate3dMeshesActive;

	List<mapCreator.buildingInfo> buildingList = new List<mapCreator.buildingInfo> ();

	void OnEnable ()
	{
		list = new SerializedObject (target);
		map = (mapCreator)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (map.showGizmo) {
				buildingList = map.buildingList;
				for (int i = 0; i < buildingList.Count; i++) {
					for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
						if (map.buildingList [i].buildingFloorsList [j].floor) {
							Vector3 floorPosition = buildingList [i].buildingFloorsList [j].floor.transform.position;
							Handles.color = Color.red;
							Handles.Label (floorPosition, buildingList [i].buildingFloorsList [j].Name);					

							if (map.useHandleForVertex) {
								Handles.color = map.gizmoLabelColor;
								EditorGUI.BeginChangeCheck ();

								Vector3 oldPoint = floorPosition;
								Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, map.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (buildingList [i].buildingFloorsList [j].floor.transform, "move Floor");
									buildingList [i].buildingFloorsList [j].floor.transform.position = newPoint;
								}   
							}
						}

						if (map.showFloorTriggers) {
							for (int k = 0; k < buildingList [i].buildingFloorsList [j].triggerToChangeFloorList.Count; k++) {
								Gizmos.color = Color.green;
								Handles.Label (buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.position, 
									buildingList [i].buildingFloorsList [j].Name + " Trigger " + j.ToString ());					
							}
						}
					}

					if (map.showBuildingTriggers) {
						for (int k = 0; k < buildingList [i].triggerToChangeBuildingList.Count; k++) {
							Gizmos.color = Color.green;
							Handles.Label (buildingList [i].triggerToChangeBuildingList [k].transform.position, 
								buildingList [i].Name + " Trigger " + k.ToString ());					
						}
					}
				}
			}

			if (map.updateAllMapTilesEveryFrame) {
				DrawMap ();
			} else {
				Event guiEvent = Event.current;

				if (guiEvent.type == EventType.Repaint) {
					DrawMap ();
				}
			}
		}
	}

	public void DrawMap ()
	{
		if (shapeChangedSinceLastRepaint || map.updateAllMapTilesEveryFrame) {
			map.calculateAllMapTileMesh ();
		}

		shapeChangedSinceLastRepaint = false;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("floorMaterial"));
		EditorGUILayout.PropertyField (list.FindProperty ("mapLayer"));
		EditorGUILayout.PropertyField (list.FindProperty ("triggerToChangeBuildingPrefab"));
		EditorGUILayout.PropertyField (list.FindProperty ("triggerToChangeFloorPrefab"));	
		EditorGUILayout.PropertyField (list.FindProperty ("triggerToChangeDynamicObjectPrefab"));

		EditorGUILayout.PropertyField (list.FindProperty ("mapPart3dMeshMaterial"));
		EditorGUILayout.PropertyField (list.FindProperty ("mapPart3dMaterialColor"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("layerToPlaceElements"));
		EditorGUILayout.PropertyField (list.FindProperty ("useRaycastToPlaceElements"));
		EditorGUILayout.PropertyField (list.FindProperty ("mapPartEnabledTriggerScale"));
		EditorGUILayout.PropertyField (list.FindProperty ("enabledTriggerGizmoColor"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("updateAllMapTilesEveryFrame"));
			EditorGUILayout.PropertyField (list.FindProperty ("showMapPartsGizmo"));
			EditorGUILayout.PropertyField (list.FindProperty ("useSameLineColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapLinesColor"));	
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("showMapPartsTextGizmo"));	

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Handle Vertex Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindProperty ("useHandleForVertex"));
			if (list.FindProperty ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("handleRadius"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showVertexHandles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Building Triggers Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindProperty ("showBuildingTriggersLine"));
			if (list.FindProperty ("showBuildingTriggersLine").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("buildingTriggersLineColor"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showBuildingTriggers"));
			if (list.FindProperty ("showBuildingTriggers").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("buildingTriggersColor"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showBuildingTriggersCubes"));	
			if (list.FindProperty ("showBuildingTriggersCubes").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("buildingTriggersCubesColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Floors Triggers Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindProperty ("showFloorTriggersLine"));
			if (list.FindProperty ("showFloorTriggersLine").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("floorTriggersLineColor"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showFloorTriggers"));
			if (list.FindProperty ("showFloorTriggers").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("floorTriggersColor"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showFloorTriggersCubes"));	
			if (list.FindProperty ("showFloorTriggersCubes").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("floorTriggersCubesColor"));
			}

			EditorGUILayout.PropertyField (list.FindProperty ("showMapPartEnabledTrigger"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Part 3d Mesh Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindProperty ("generate3dMeshesActive"));
		generate3dMeshesActive = list.FindProperty ("generate3dMeshesActive").boolValue;
		if (generate3dMeshesActive) {
			EditorGUILayout.PropertyField (list.FindProperty ("generate3dMeshesShowGizmo"));
			EditorGUILayout.PropertyField (list.FindProperty ("generateFull3dMapMeshes"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapPart3dHeight"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapPart3dOffset"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Enable 3d Mesh")) {
				map.enableOrDisableMapPart3dMesh (true);
			}

			if (GUILayout.Button ("Disable 3d Mesh")) {
				map.enableOrDisableMapPart3dMesh (false);
			}

			if (GUILayout.Button ("Update Mesh Position")) {
				map.updateMapPart3dMeshPosition ();
			}

			if (GUILayout.Button ("Generate 3d Mesh")) {
				map.generateMapPart3dMesh ();
			}

			if (GUILayout.Button ("Remove 3d Mesh")) {
				map.removeMapPart3dMesh ();
			}

			if (GUILayout.Button ("Enable/Disable 3d Generation")) {
				map.setGenerate3dMapPartMeshState ();
			}
		}

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building List", "window", GUILayout.Height (50));
		showBuildingList (list.FindProperty ("buildingList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map System List Settings", "window", GUILayout.Height (50));
		showMapSystemInfoList (list.FindProperty ("mapSystemInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Icons List", "window");
		showMapIconsTypesList (list.FindProperty ("mapIconTypes"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Object Information Settings", "window");
		if (GUILayout.Button ("Set Map Object Information ID")) {
			map.setMapObjectInformationID ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map System On Map Objects")) {
			map.updateMapObjectInformationMapSystem ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Tiles Settings", "window");
		if (GUILayout.Button ("Update All Map Tiles")) {
			shapeChangedSinceLastRepaint = true;
			SceneView.RepaintAll();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Remove All Map Tiles")) {
			map.removeAllMapTileMesh ();
		}
		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showBuildingList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Buildings: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Building")) {
				map.addNewBuilding ();
			}
			if (GUILayout.Button ("Clear List")) {
				map.removeAllBuildings ();
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Buildings")) {
				map.enableOrDisableAllBuildings (false);
			}
			if (GUILayout.Button ("Enable All Buildings")) {
				map.enableOrDisableAllBuildings (true);
			}
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Buildings Info")) {
				map.getAllBuildings ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showBuildingListInfo (list.GetArrayElementAtIndex (i), i);
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
					map.removeBuilding (i);
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

	void showBuildingListInfo (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingMapParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingFloorsParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInterior"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentMap"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingMapEnabled"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraPositionOnMapMenu"));
		if (list.FindPropertyRelative ("useCameraPositionOnMapMenu").boolValue) {
			if (!list.FindPropertyRelative ("cameraPositionOnMapMenu").objectReferenceValue) {
				if (GUILayout.Button ("Add Camera Position On Map Menu")) {
					map.addCameraPositionOnMapMenu (buildingIndex);
				}
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraPositionOnMapMenu"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraOffset"));
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Building")) {
			map.renameBuilding (buildingIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger To Change Building", "window", GUILayout.Height (50));
		showChangeBuildingTriggerList (list.FindPropertyRelative ("triggerToChangeBuildingList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger For Dynamic Objects", "window", GUILayout.Height (50));
		showDynamicObjectsTriggerList (list.FindPropertyRelative ("triggerForDynamicObjectsList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building Floors List", "window", GUILayout.Height (50));
		showFloorList (list.FindPropertyRelative ("buildingFloorsList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}


	void showFloorListInfo (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floor"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorEnabled"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Floor")) {
			map.renameFloor (buildingIndex, floorIndex);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Trigger For Dynamic Object")) {
			map.addTriggerToDynamicObject (buildingIndex, floorIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger To Change Floor", "window", GUILayout.Height (50));
		showChangeFloorTriggerList (list.FindPropertyRelative ("triggerToChangeFloorList"), buildingIndex, floorIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Parts List", "window", GUILayout.Height (50));
		showMapPartsList (list.FindPropertyRelative ("mapPartsList"), buildingIndex, floorIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFloorList (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Floors: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
				map.addNewFloor (buildingIndex);
			}
			if (GUILayout.Button ("Clear List")) {
				map.removeAllFloors (buildingIndex);
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Floors")) {
				map.enableOrDisableBuilding (false, buildingIndex);
			}
			if (GUILayout.Button ("Enable All Floors")) {
				map.enableOrDisableBuilding (true, buildingIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFloorListInfo (list.GetArrayElementAtIndex (i), buildingIndex, i);
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
					map.removeFloor (buildingIndex, i);
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

	void showMapPartsList (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Parts: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor Part")) {
				map.addNewMapPartFromMapCreator (buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Clear")) {
				map.removeAllMapParts (buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get All Floor Parts")) {
				map.GetAllFloorParts (buildingIndex, floorIndex);
			}

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Floor Parts")) {
				map.enableOrDisableAllFloorParts (false, buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Enable All Floor Parts")) {
				map.enableOrDisableAllFloorParts (true, buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			if (generate3dMeshesActive) {
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable All Floor Parts Generate 3d Mesh")) {
					map.setGenerate3dMapPartMeshStateToFloor (false, buildingIndex, floorIndex);
				}
				if (GUILayout.Button ("Enable All Floor Generate 3d Mesh")) {
					map.setGenerate3dMapPartMeshStateToFloor (true, buildingIndex, floorIndex);
				}
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {
					GUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("x")) {
						map.removeMapPart (buildingIndex, floorIndex, i);
						return;
					}

					if (GUILayout.Button ("Enable Map Part")) {
						map.enableOrDisableFloorPart (true, buildingIndex, floorIndex, i);
					}
					if (GUILayout.Button ("Disable Map Part")) {
						map.enableOrDisableFloorPart (false, buildingIndex, floorIndex, i);
					}
					GUILayout.EndHorizontal ();

					GameObject floorPart = list.GetArrayElementAtIndex (i).objectReferenceValue as GameObject;
					if (floorPart) {
						mapTileBuilder currentMapTile = floorPart.GetComponent <mapTileBuilder> ();

						EditorGUILayout.Space ();

						GUILayout.BeginVertical ();
						currentMapTile.mapPartEnabled = EditorGUILayout.Toggle ("Enabled", currentMapTile.mapPartEnabled);
						if (generate3dMeshesActive) {
							currentMapTile.generate3dMapPartMesh = EditorGUILayout.Toggle ("Generate 3d mesh", currentMapTile.generate3dMapPartMesh);
						}
						EditorUtility.SetDirty (currentMapTile);
						GUILayout.EndVertical ();
					}

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showChangeFloorTriggerList (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Trigger")) {
				map.addTriggerToChangeFloor (buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Clear")) {
				map.removeTriggerToChangeFloorList (buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					map.removeTriggerToChangeFloor (buildingIndex, floorIndex, i);
				}
				if (GUILayout.Button ("O")) {
					map.selectCurrentFloorTrigger (buildingIndex, floorIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showChangeBuildingTriggerList (SerializedProperty list, int buildingIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Trigger")) {
				map.addTriggerToChangeBuilding (buildingIndex);
			}
			if (GUILayout.Button ("Clear")) {
				map.removeTriggerToChangeBuildingList (buildingIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					map.removeTriggerToChangeBuilding (buildingIndex, i);
				}
				if (GUILayout.Button ("O")) {
					map.selectChangeBuildingTrigger (buildingIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showDynamicObjectsTriggerList (SerializedProperty list, int buildingIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear")) {
				map.removeTriggerToDynamicObjectList (buildingIndex);
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					map.removeTriggerToDynamicObject (buildingIndex, i);
				}
				if (GUILayout.Button ("O")) {
					map.selectDynamicObjectsTrigger (buildingIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showMapSystemInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Map System: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Map")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.ClearArray ();
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMapSystemInfoListElement (list.GetArrayElementAtIndex (i), i);
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
		GUILayout.EndVertical ();
	}

	void showMapSystemInfoListElement (SerializedProperty list, int mapSystemIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainMapSystem"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Map Icon Types From This Map System")) {
			map.setMapIconTypes (mapSystemIndex);
		}
		GUILayout.EndVertical ();
	}

	void showMapIconsTypesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Map Icons: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Icon")) {
				list.arraySize++;
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMapIconsTypesListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showMapIconsTypesListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("typeName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showIconPreview"));
		bool showIconPreview = list.FindPropertyRelative ("showIconPreview").boolValue;
		if (showIconPreview) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Icon Preview \t");
			GUILayout.BeginHorizontal ("box", GUILayout.Height (50), GUILayout.Width (50));
			if (list.FindPropertyRelative ("icon").objectReferenceValue) {
				RectTransform icon = list.FindPropertyRelative ("icon").objectReferenceValue as RectTransform;
				Object texture = new Object ();
				if (icon.GetComponent<RawImage> ()) {
					texture = icon.GetComponent<RawImage> ().texture;
				} else if (icon.GetComponent<Image> ()) {
					texture = icon.GetComponent<Image> ().sprite;
				}
				Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
				GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCompassIcon"));
		if (list.FindPropertyRelative ("useCompassIcon").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("compassIconPrefab"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalOffset"));
		}

		GUILayout.EndVertical ();
	}

}
#endif