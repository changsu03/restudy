using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapSystem))]
public class mapSystemEditor : Editor
{
	SerializedObject list;
	bool mapComponents;
	bool mapSettings;
	bool compassComponents;
	bool compassSettings;
	bool mapFloorAndIcons;
	bool markSettings;
	Color defBackgroundColor;
	mapSystem mapSystemManager;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		mapSystemManager = (mapSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("mapEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("changeFloorWithTriggers"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Position In Map Settings", "window", GUILayout.Height (30));
		if (Application.isPlaying) {
			GUILayout.Label ("Current Building Name\t" + list.FindProperty ("currentBuildingName").stringValue.ToString ());
			GUILayout.Label ("Current Building Index\t" + list.FindProperty ("currentBuildingIndex").intValue.ToString ());
			GUILayout.Label ("Current Floor Index\t\t" + list.FindProperty ("currentFloorIndex").intValue.ToString ());
			GUILayout.Label ("Current Floor Number\t\t" + list.FindProperty ("currentFloorNumber").intValue.ToString ());
			GUILayout.Label ("Current Map Part Index\t" + list.FindProperty ("currentMapPartIndex").intValue.ToString ());

		} else {
			EditorGUILayout.PropertyField (list.FindProperty ("updateMapInfoEveryFrame"));
			if (mapSystemManager.buildingListString.Length > 0) {

				if (list.FindProperty ("updateMapInfoEveryFrame").boolValue) {
					mapSystemManager.updateEditorMapInfo ();
				}

				mapSystemManager.currentBuildingIndex = EditorGUILayout.Popup ("Building Number", mapSystemManager.currentBuildingIndex, mapSystemManager.buildingListString);
				if (mapSystemManager.currentBuildingIndex >= 0) {
					mapSystemManager.currentBuildingName = mapSystemManager.buildingListString [mapSystemManager.currentBuildingIndex];
				}

				if (mapSystemManager.floorListString.Length > 0) {
					mapSystemManager.currentFloorIndex = EditorGUILayout.Popup ("Floor Number", mapSystemManager.currentFloorIndex, mapSystemManager.floorListString);
					if (mapSystemManager.currentFloorIndex >= 0 && mapSystemManager.currentFloorIndex < mapSystemManager.floorListString.Length) {
						mapSystemManager.currentFloorName = mapSystemManager.floorListString [mapSystemManager.currentFloorIndex];
					}

					if (mapSystemManager.mapPartListString.Length > 0) {
						mapSystemManager.currentMapPartIndex = EditorGUILayout.Popup ("Map Part", mapSystemManager.currentMapPartIndex, mapSystemManager.mapPartListString);
						if (mapSystemManager.mapPartListString.Length > mapSystemManager.currentMapPartIndex) {
							mapSystemManager.currentMapPartName = mapSystemManager.mapPartListString [mapSystemManager.currentMapPartIndex];
						}
					}
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Map Info")) {
				mapSystemManager.updateEditorMapInfo ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Player Map System To Map Creator")) {
				mapSystemManager.addPlayerMapSystemToMapCreator ();	
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("mapCreatorManager"));

			if (!list.FindProperty ("mapCreatorManager").objectReferenceValue) {
				
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Map Creator")) {
					mapSystemManager.addNewMapCreator();	
				}
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		mapComponents = list.FindProperty ("showMapComponents").boolValue;
		mapSettings = list.FindProperty ("showMapSettings").boolValue;
		compassComponents = list.FindProperty ("showCompassComponents").boolValue;
		compassSettings = list.FindProperty ("showCompassSettings").boolValue;
		mapFloorAndIcons = list.FindProperty ("showMapFloorAndIcons").boolValue;
		markSettings = list.FindProperty ("showMarkSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (mapComponents) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Map Components")) {
			mapComponents = !mapComponents;
		}
		if (mapSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Map Settings")) {
			mapSettings = !mapSettings;
		}
		if (markSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Mark Settings")) {
			markSettings = !markSettings;
		}
		if (compassComponents) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Compass Components")) {
			compassComponents = !compassComponents;
		}
		if (compassSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Compass Settings")) {
			compassSettings = !compassSettings;
		}
		if (mapFloorAndIcons) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("World Map")) {
			mapFloorAndIcons = !mapFloorAndIcons;
		}
		GUI.backgroundColor = defBackgroundColor;

		EditorGUILayout.EndVertical ();

		list.FindProperty ("showMapComponents").boolValue = mapComponents;
		list.FindProperty ("showMapSettings").boolValue = mapSettings;
		list.FindProperty ("showCompassComponents").boolValue = compassComponents;
		list.FindProperty ("showCompassSettings").boolValue = compassSettings;
		list.FindProperty ("showMapFloorAndIcons").boolValue = mapFloorAndIcons;
		list.FindProperty ("showMarkSettings").boolValue = markSettings;

		if (mapComponents) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set every Map Component here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("mapContent"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapCamera"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapSystemPivotTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapSystemCameraTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("player"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapMenu"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapWindowTargetPosition"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapRender"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapWindow"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerMapIcon"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerIconChild"));

			EditorGUILayout.PropertyField (list.FindProperty ("removeMarkButtonImage"));
			EditorGUILayout.PropertyField (list.FindProperty ("quickTravelButtonImage"));
	
			EditorGUILayout.PropertyField (list.FindProperty ("mapObjectNameField"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapObjectInfoField"));

			EditorGUILayout.PropertyField (list.FindProperty ("currentFloorNumberText"));
			EditorGUILayout.PropertyField (list.FindProperty ("useMapIndexWindow"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapIndexWindow"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapIndexWindowContent"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapIndexWindowScroller"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapObjectTextIcon"));
			EditorGUILayout.PropertyField (list.FindProperty ("useBlurUIPanel"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapWindowMask"));	
			EditorGUILayout.PropertyField (list.FindProperty ("zoomScrollbar"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentMapZoneText"));

			EditorGUILayout.PropertyField (list.FindProperty ("mapCursor"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapCursorRectTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("currenMapIconPressed"));

			EditorGUILayout.PropertyField (list.FindProperty ("mapCircleTransform"));

			EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainGameManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("screenObjectivesManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapCreatorManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerMapObjectInformation"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainMapCamera"));

			EditorGUILayout.PropertyField (list.FindProperty ("mapCreatorPrefab"));

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (mapSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Map Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("CONTROL", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("playerIconMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("openMapSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("mouseDragMapSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("keysDragMapSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("getClosestFloorToPlayerByDistance"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapCameraMovementType"));
			EditorGUILayout.PropertyField (list.FindProperty ("recenterCameraSpeed"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("ROTATION", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("rotateMap"));
			if (list.FindProperty ("rotateMap").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("smoothRotationMap"));
				EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("CIRCLE MAP", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("usingCircleMap"));
			if (list.FindProperty ("usingCircleMap").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("circleMapRadius"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("ICONS", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("showOffScreenIcons"));
			EditorGUILayout.PropertyField (list.FindProperty ("iconSize"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxIconSize"));
			EditorGUILayout.PropertyField (list.FindProperty ("offScreenIconSize"));
			EditorGUILayout.PropertyField (list.FindProperty ("openMapIconSizeMultiplier"));
			EditorGUILayout.PropertyField (list.FindProperty ("changeIconSizeSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("showIconsByFloor"));
			EditorGUILayout.PropertyField (list.FindProperty ("borderOffScreen"));
			EditorGUILayout.PropertyField (list.FindProperty ("useTextInIcons"));
			EditorGUILayout.PropertyField (list.FindProperty ("textIconsOffset"));	
			EditorGUILayout.PropertyField (list.FindProperty ("mapObjectTextIconColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("mapObjectTextSize"));
			EditorGUILayout.PropertyField (list.FindProperty ("miniMapWindowEnabledInGame"));
			EditorGUILayout.PropertyField (list.FindProperty ("miniMapWindowSmoothOpening"));
			EditorGUILayout.PropertyField (list.FindProperty ("miniMapWindowWithMask"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerUseMapObjectInformation"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerIconOffset"));
			EditorGUILayout.PropertyField (list.FindProperty ("useCurrentMapIconPressed"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("MAP CURSOR SETTINGS", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useMapCursor"));
			if (list.FindProperty ("useMapCursor").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("showInfoIconInsideCursor"));
				if (list.FindProperty ("showInfoIconInsideCursor").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToMapIcon"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("2D ZOOM SETTINGS", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomWhenOpen"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomWhenClose"));
			EditorGUILayout.PropertyField (list.FindProperty ("openCloseZoomSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxZoom"));
			EditorGUILayout.PropertyField (list.FindProperty ("minZoom"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomToActivateIcons"));	
			EditorGUILayout.PropertyField (list.FindProperty ("zoomToActivateTextIcons"));	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("3D ZOOM SETTINGS", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomWhenOpen3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomWhenClose3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("openCloseZoomSpeed3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxZoom3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("minZoom3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("zoomToActivateIcons3d"));	
			EditorGUILayout.PropertyField (list.FindProperty ("zoomToActivateTextIcons3d"));
			EditorGUILayout.PropertyField (list.FindProperty ("setColorOnCurrent3dMapPart"));
			if (list.FindProperty ("setColorOnCurrent3dMapPart").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("colorOnCurrent3dMapPart"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("MARKS", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("disabledRemoveMarkColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("disabledQuickTravelColor"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("3D Map Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("map3dEnabled"));
			if (list.FindProperty ("map3dEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("map3dPositionSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("map3dRotationSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("rangeAngleX"));
				EditorGUILayout.PropertyField (list.FindProperty ("rangeAngleY"));
				EditorGUILayout.PropertyField (list.FindProperty ("transtionTo3dSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("maxTimeBetweenTransition"));
				EditorGUILayout.PropertyField (list.FindProperty ("reset3dCameraSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("inital3dCameraRotation"));
				EditorGUILayout.PropertyField (list.FindProperty ("hideOffscreenIconsOn3dView"));
				EditorGUILayout.PropertyField (list.FindProperty ("showIconsOn3dView"));
				EditorGUILayout.PropertyField (list.FindProperty ("use3dMeshForPlayer"));
				EditorGUILayout.PropertyField (list.FindProperty ("player3dMesh"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (markSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Mark Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("markPrefab"));
			EditorGUILayout.PropertyField (list.FindProperty ("setMarkOnCurrenBuilding"));
			EditorGUILayout.PropertyField (list.FindProperty ("setMartOnCurrentFloor"));

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (compassComponents) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set every Compass Component here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("compassWindow"));
			EditorGUILayout.PropertyField (list.FindProperty ("compassElementsParent"));
			EditorGUILayout.PropertyField (list.FindProperty ("north"));
			EditorGUILayout.PropertyField (list.FindProperty ("south"));
			EditorGUILayout.PropertyField (list.FindProperty ("east"));
			EditorGUILayout.PropertyField (list.FindProperty ("west"));

			EditorGUILayout.PropertyField (list.FindProperty ("northEast"));
			EditorGUILayout.PropertyField (list.FindProperty ("southWest"));
			EditorGUILayout.PropertyField (list.FindProperty ("southEast"));
			EditorGUILayout.PropertyField (list.FindProperty ("northWest"));

			EditorGUILayout.PropertyField (list.FindProperty ("compassDirections"));

			EditorGUILayout.PropertyField (list.FindProperty ("northGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("southGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("eastGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("westGameObject"));

			EditorGUILayout.PropertyField (list.FindProperty ("northEastGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("southWestGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("southEastGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("northwestGameObject"));
	
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (compassSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Compass Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("compassEnabled"));
			EditorGUILayout.PropertyField (list.FindProperty ("compassOffset"));
			EditorGUILayout.PropertyField (list.FindProperty ("compassScale"));
			EditorGUILayout.PropertyField (list.FindProperty ("showIntermediateDirections"));
			EditorGUILayout.PropertyField (list.FindProperty ("maximumLeftDistance"));
			EditorGUILayout.PropertyField (list.FindProperty ("maximumRightDistance"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (mapFloorAndIcons) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every Floor and Icon element for the map", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Building List", "window");
			showBuildingList (list.FindProperty ("buildingList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
		GUI.backgroundColor = defBackgroundColor;
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showBuildingList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Buildings: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Building List")) {
				mapSystemManager.searchBuildingList ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Building")) {
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
						showBuildingElementInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showBuildingElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentMap"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInterior"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraPositionOnMapMenu"));
		if (list.FindPropertyRelative ("useCameraPositionOnMapMenu").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraPositionOnMapMenu"));
		}
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floors List", "window");
		showFloorList (list.FindPropertyRelative ("floors"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showFloorList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Floors: " + list.arraySize.ToString ());
	
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
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
						showFloorElementInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showFloorElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floor"));
		GUILayout.EndVertical ();
	}
}
#endif