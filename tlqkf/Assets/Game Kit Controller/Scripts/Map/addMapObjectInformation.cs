using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addMapObjectInformation : MonoBehaviour
{
	public bool addMapIcon;
	public string mapObjectName;
	public string mapObjectTypeName;
	[TextArea(3,10)] public string description;

	public bool visibleInAllBuildings;
	public bool visibleInAllFloors;
	public bool calculateFloorAtStart;

	public bool setFloorNumber;
	public int buildingIndex;
	public int floorNumber;

	public bool activateAtStart;

	public bool addIconOnScreen;
	public float triggerRadius;
	public bool showOffScreenIcon;
	public bool useCloseDistance;
	public bool showMapWindowIcon;
	public bool showDistance;
	public string objectiveIconName;
	public float objectiveOffset;

	public bool useCustomObjectiveColor;
	public Color objectiveColor;
	public bool removeCustomObjectiveColor;

	mapObjectInformation mapObjectInformationManager;
	mapCreator mapCreatorManager;

	void Start ()
	{
		if (activateAtStart) {
			activateMapObject ();
		}
	}

	public void activateMapObject ()
	{
		if (addMapIcon) {

			mapCreatorManager = FindObjectOfType<mapCreator> ();

			if (!mapCreatorManager) {
				print ("Warning: there is no map system configured, so the object " + gameObject.name + " won't use a new map object icon");
				return;
			}

			mapObjectInformationManager = gameObject.AddComponent<mapObjectInformation> ();
			mapObjectInformationManager.assignID (mapCreatorManager.getAndIncreaselastMapObjectInformationIDAssigned ());

			mapObjectInformationManager.name = mapObjectName;
			if (addIconOnScreen) {
				mapObjectInformationManager.setCustomValues (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, useCloseDistance, 
					triggerRadius, showOffScreenIcon, showMapWindowIcon, showDistance, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor);
			}

			if (setFloorNumber) {
				mapObjectInformationManager.floorIndex = floorNumber;
				mapObjectInformationManager.buildingIndex = buildingIndex;
			}

			mapObjectInformationManager.getMapObjectInformation ();
			mapObjectInformationManager.getIconTypeIndexByName (mapObjectTypeName);
			mapObjectInformationManager.description = description;
		} else {
			if (addIconOnScreen) {
				FindObjectOfType<screenObjectivesSystem>().addElementToScreenObjectiveList (gameObject, useCloseDistance, triggerRadius, showOffScreenIcon, 
					showDistance, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, objectiveOffset);
			}
		}
	}
}
