using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class mapObjectInformation : MonoBehaviour
{
	public string name;
	[TextArea (3, 10)]
	public string description;
	public float offsetRadius;

	public bool showOffScreenIcon = true;
	public bool showMapWindowIcon = true;
	public bool showDistance = true;
	public bool isActivate = true;

	public bool visibleInAllBuildings;
	public bool visibleInAllFloors;
	public bool calculateFloorAtStart;

	public bool useCloseDistance = true;
	public float triggerRadius = 5;
	public Color triggerColor = Color.blue;
	public float gizmoLabelOffset;
	public Color gizmoLabelColor = Color.white;
	public int typeIndex;
	public string typeName;
	public string[] typeNameList;
	public int floorIndex;
	public string currentFloor;
	public string[] floorList;

	public float extraIconSizeOnMap;
	public bool followCameraRotation;
	public Vector3 offset;
	public mapSystem mapManager;

	public bool useCustomObjectiveColor;
	public Color objectiveColor;
	public bool removeCustomObjectiveColor;
	public float objectiveOffset;

	public bool removeComponentWhenObjectiveReached = true;

	public bool disableWhenPlayerHasReached;

	public int buildingIndex;
	public string buildingName;
	public string[] buildingList;
	public string currentBuilding;

	public bool belongToMapPart;
	public string mapPartName;
	public string[] mapPartList;
	public int mapPartIndex;
	public mapCreator mapCreatorManager;

	public bool useCustomValues;

	public int ID;

	public bool callEventWhenPointReached;
	public UnityEvent pointReachedEvent = new UnityEvent ();

	public bool useEventsOnChangeFloor;
	public bool useEventOnEnabledFloor;
	public UnityEvent evenOnEnabledFloor;
	public bool useEventOnDisabledFloor;
	public UnityEvent evenOnDisabledFloor;

	public bool canChangeBuildingAndFloor;

	public bool activateAtStart = true;

	public Color offsetGizmoColor;
	public bool offsetShowGizmo;
	public bool showGizmo;

	bool checkMapPart;
	string objectiveIconName;

	public mapSystem.mapObjectInfo currentMapObjectInfo;

	public bool setCustomCompassSettings;
	public bool useCompassIcon;
	public GameObject compassIconPrefab;
	public float verticalOffset;

	screenObjectivesSystem screenObjectivesManager;

	void Start ()
	{
		if (activateAtStart) {
			createMapIconInfo ();
		}
	}

	public void createMapIconInfo ()
	{
		if (!belongToMapPart) {
			mapPartIndex = -1;
		}

		if (typeName != "") {
			if (disableWhenPlayerHasReached) {
					
				if (showMapWindowIcon) {
					checkGetMapCreatorManager ();

					if (mapCreatorManager) {
						mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, false, gameObject, typeName, offset, -1, -1, buildingIndex, extraIconSizeOnMap, followCameraRotation,
							setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);
					}
				}

				getScreenObjectivesManager ();

				screenObjectivesManager.addElementToScreenObjectiveList (gameObject, useCloseDistance, triggerRadius, showOffScreenIcon, 
					showDistance, typeName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, objectiveOffset);
			} else {
				checkGetMapCreatorManager ();

				if (mapCreatorManager) {
					mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, gameObject, typeName, offset, ID, 
						mapPartIndex, buildingIndex, extraIconSizeOnMap, followCameraRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);
				}

				if (useCustomValues) {

					getScreenObjectivesManager ();

					screenObjectivesManager.addElementToScreenObjectiveList (gameObject, useCloseDistance, triggerRadius, showOffScreenIcon, 
						showDistance, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, 0);
				}
			}
		} else {
			print ("Object without map object information configurated " + gameObject.name);
		}
	}

	public void getScreenObjectivesManager ()
	{
		if (!screenObjectivesManager) {
			screenObjectivesManager = FindObjectOfType<screenObjectivesSystem> ();
		}
	}

	void Update ()
	{
		if (!checkMapPart) {
			if (belongToMapPart) {
				if (mapCreatorManager) {
					if (buildingIndex < mapCreatorManager.buildingList.Count && floorIndex < mapCreatorManager.buildingList [buildingIndex].buildingFloorsList.Count &&
						mapPartIndex < mapCreatorManager.buildingList [buildingIndex].buildingFloorsList [floorIndex].mapTileBuilderList.Count) {
						if (!mapCreatorManager.buildingList [buildingIndex].buildingFloorsList [floorIndex].mapTileBuilderList [mapPartIndex].mapPartEnabled) {
							//print ("inactive");
							mapCreatorManager.enableOrDisableSingleMapIconByID (ID, false);
						}
					} else {
						print ("WARNING: map object information not properly configured in object " + gameObject.name);
					}
				}
			}
			checkMapPart = true;
		}
	}

	public void addMapObject (string mapIconType)
	{
		if (mapCreatorManager) {
			mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, 
				gameObject, mapIconType, offset, ID, mapPartIndex, buildingIndex, extraIconSizeOnMap, followCameraRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);
		}
	}

	public void removeMapObject ()
	{
		getScreenObjectivesManager ();

		//remove the object from the screen objective system in case it was added as a mark
		screenObjectivesManager.removeGameObjectFromList (gameObject);

		//remove object of the map
		if (mapCreatorManager) {
			mapCreatorManager.removeMapObject (gameObject, false);
		}
	}

	public void setPathElementInfo (bool showOffScreenIconInfo, bool showMapWindowIconInfo, bool showDistanceInfo)
	{
		typeName = "Path Element";
		showGizmo = true;
		showOffScreenIcon = showOffScreenIconInfo;
		showMapWindowIcon = showMapWindowIconInfo;
		showDistance = showDistanceInfo;
	}

	public void getMapIconTypeList ()
	{
		checkGetMapManager ();

		if (mapManager) {
			typeNameList = new string[mapManager.mapIconTypes.Count];
			for (int i = 0; i < mapManager.mapIconTypes.Count; i++) {
				typeNameList [i] = mapManager.mapIconTypes [i].typeName;
			}
			updateComponent ();
		}
	}

	public void getBuildingList ()
	{
		checkGetMapManager ();

		checkGetMapCreatorManager ();

		if (mapManager) {
			if (mapManager.buildingList.Count > 0) {
				buildingList = new string[mapManager.buildingList.Count];
				for (int i = 0; i < mapManager.buildingList.Count; i++) {
					string name = mapManager.buildingList [i].Name;
					buildingList [i] = name;
				}
				updateComponent ();
			} else {
				print ("Not buildings were found. To use the map object information component, first add and configure different floors in the map creator component. Check" +
				"the documentation of the asset related to the Map System for a better explanation");
			}
		}
	}

	public void getFloorList ()
	{		
		if (mapManager && mapCreatorManager) {
			if (mapManager.buildingList.Count > 0) {
				if (buildingIndex >= 0 && buildingIndex < mapCreatorManager.buildingList.Count) {
					if (mapManager.buildingList [buildingIndex].floors.Count > 0) {
						floorList = new string[mapManager.buildingList [buildingIndex].floors.Count ];
						for (int i = 0; i < mapManager.buildingList [buildingIndex].floors.Count; i++) {
							if (mapManager.buildingList [buildingIndex].floors [i].floor) {
								string name = mapManager.buildingList [buildingIndex].floors [i].floor.gameObject.name;
								floorList [i] = name;
							}
						}

						updateComponent ();

						if (belongToMapPart) {
							getMapPartList ();
						}
					}
				}
			} else {
				print ("Not floors were found. To use the map object information component, first add and configure different floors in the map creator component. Check" +
				"the documentation of the asset related to the Map System for a better explanation");
			}
		}
	}

	public void checkGetMapManager ()
	{
		if (!mapManager) {
			mapManager = FindObjectOfType<mapSystem> ();
		}
	}

	public void checkGetMapCreatorManager ()
	{
		if (!mapCreatorManager) {
			mapCreatorManager = FindObjectOfType<mapCreator> ();
		}
	}

	public void getCurrentMapManager ()
	{
		mapManager = FindObjectOfType<mapSystem> ();
		mapCreatorManager = FindObjectOfType<mapCreator> ();

		updateComponent ();
	}

	public void getMapPartList ()
	{
		if (mapCreatorManager) {
			if (floorIndex >= 0 && buildingIndex >= 0) {
				if (buildingIndex < mapCreatorManager.buildingList.Count && floorIndex < mapCreatorManager.buildingList [buildingIndex].buildingFloorsList.Count) {
					mapPartList = new string[mapCreatorManager.buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count];
					for (int i = 0; i < mapCreatorManager.buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count; i++) {
						string name = mapCreatorManager.buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [i].name;
						mapPartList [i] = name;
					}
				} else {
					floorIndex = 0;
					mapPartList = new string[0];
				}

				updateComponent ();
			}
		}
	}

	public void getIconTypeIndexByName (string iconTypeName)
	{
		int index = mapManager.getIconTypeIndexByName (iconTypeName);
		if (index != -1) {
			typeIndex = index;
			typeName = iconTypeName;
		}
	}

	public void getMapObjectInformation ()
	{
		getBuildingList ();
		getFloorList ();
		getMapIconTypeList ();
	}

	public void setCustomValues (bool visibleInAllBuildingsValue, bool visibleInAllFloorsValue, bool calculateFloorAtStartValue, bool useCloseDistanceValue, 
	                             float triggerRadiusValue, bool showOffScreenIconValue, bool showMapWindowIconValue, bool showDistanceValue, string objectiveIconNameValue,
	                             bool useCustomObjectiveColorValue, Color objectiveColorValue, bool removeCustomObjectiveColorValue)
	{
		useCustomValues = true;

		visibleInAllBuildings = visibleInAllBuildingsValue;
		visibleInAllFloors = visibleInAllFloorsValue;
		calculateFloorAtStart = calculateFloorAtStartValue;

		useCloseDistance = useCloseDistanceValue;
		triggerRadius = triggerRadiusValue;
		showOffScreenIcon = showOffScreenIconValue;
		showMapWindowIcon = showMapWindowIconValue;
		showDistance = showDistanceValue;
		objectiveIconName = objectiveIconNameValue;
		useCustomObjectiveColor = useCustomObjectiveColorValue;
		objectiveColor = objectiveColorValue;
		removeCustomObjectiveColor = removeCustomObjectiveColorValue;
	}

	public void changeMapObjectIconFloor (int newFloorIndex)
	{
		mapCreatorManager.changeMapObjectIconFloor (gameObject, newFloorIndex);
	}

	public void changeMapObjectIconFloorByPosition ()
	{
		mapCreatorManager.changeMapObjectIconFloorByPosition (gameObject);
	}

	public void enableSingleMapIconByID ()
	{
		mapCreatorManager.enableOrDisableSingleMapIconByID (ID, true);
	}

	public void assignID (int newID)
	{
		ID = newID;

		updateComponent ();
	}

	public void checkPointReachedEvent ()
	{
		if (callEventWhenPointReached) {
			if (pointReachedEvent.GetPersistentEventCount () > 0) {
				pointReachedEvent.Invoke ();
			}
		}
	}

	//Functions called and used to link a map object information and a map object info in the map system, so if the state changes in one of them, the other can update its state too

	public void checkEventOnChangeFloor (int currentBuildingIndex, int currentFloorIndex)
	{
		if (useEventsOnChangeFloor) {
			if (currentBuildingIndex == buildingIndex && floorIndex == currentFloorIndex) {
				if (useEventOnEnabledFloor) {
					evenOnEnabledFloor.Invoke ();
				}
			} else {
				if (useEventOnDisabledFloor) {
					evenOnDisabledFloor.Invoke ();
				}
			}
		}
	}

	public void setCurrentMapObjectInfo (mapSystem.mapObjectInfo newMapObjectInfo)
	{
		if (canChangeBuildingAndFloor) {
			currentMapObjectInfo = newMapObjectInfo;
		}
	}

	public void setNewBuildingAndFloorIndex (int newBuildingIndex, int newFloorIndex)
	{
		if (canChangeBuildingAndFloor) {
			buildingIndex = newBuildingIndex;
			floorIndex = newFloorIndex;
			mapCreatorManager.setnewBuilingAndFloorIndexToMapObject (currentMapObjectInfo, newBuildingIndex, newFloorIndex);
		}
	}

	public void setNewBuildingAndFloorIndexByInspector ()
	{
		if (canChangeBuildingAndFloor) {
			mapCreatorManager.setnewBuilingAndFloorIndexToMapObject (currentMapObjectInfo, buildingIndex, floorIndex);
		}
	}

	public void setNewBuildingIndex (int newBuildingIndex)
	{
		buildingIndex = newBuildingIndex;
	}

	public void setNewFloorIndex (int newFloorIndex)
	{
		floorIndex = newFloorIndex;
	}

	public int getBuildingIndex ()
	{
		return buildingIndex;
	}

	public int getFloorIndex ()
	{
		return floorIndex;
	}

	public bool removeComponentWhenObjectiveReachedEnabled ()
	{
		return removeComponentWhenObjectiveReached;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<mapObjectInformation> ());
		#endif
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (transform.position, triggerRadius);

			if (offsetShowGizmo) {
				Gizmos.color = offsetGizmoColor;
				Gizmos.DrawSphere (transform.position + offset, offsetRadius);
			}
		}
	}
}