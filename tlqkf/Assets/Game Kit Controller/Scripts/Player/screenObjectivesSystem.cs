using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class screenObjectivesSystem : MonoBehaviour
{
	public List<objectiveIconElement> objectiveIconList = new List<objectiveIconElement> ();
	public List<objectiveInfo> objectiveList = new List<objectiveInfo> ();

	public List<playerScreenObjectivesSystem> playerScreenObjectivesList = new List<playerScreenObjectivesSystem> ();

	mapCreator mapCreatorManager;

	int currentID = 0;

	void Start ()
	{
		mapCreatorManager = FindObjectOfType<mapCreator> ();
	}

	public void addNewPlayer (playerScreenObjectivesSystem newPlayer)
	{
		playerScreenObjectivesList.Add (newPlayer);
	}

	//get the renderer parts of the target to set its colors with the objective color, to see easily the target to reach
	public void addElementToScreenObjectiveList (GameObject obj, bool useCloseDistance, float radiusDistance, bool showOffScreen, bool showDistanceInfo, string objectiveIconName, 
	                                             bool useCustomObjectiveColor, Color objectiveColor, bool removeCustomObjectiveColor, float iconOffset)
	{
		objectiveInfo newObjective = new objectiveInfo ();
		newObjective.Name = obj.name;
		newObjective.mapObject = obj;
		newObjective.iconOffset = iconOffset;
		newObjective.ID = currentID;

		int currentObjectiveIconIndex = -1;
		for (int i = 0; i < objectiveIconList.Count; i++) {
			if (objectiveIconList [i].name == objectiveIconName) {
				currentObjectiveIconIndex = i;
			}
		}

		if (currentObjectiveIconIndex != -1) {

			objectiveIconElement currentObjectiveIconElement = objectiveIconList [currentObjectiveIconIndex];

			if (radiusDistance < 0) {
				radiusDistance = currentObjectiveIconElement.minDefaultDistance;
			}
			newObjective.useCloseDistance = useCloseDistance;
			newObjective.closeDistance = radiusDistance;
			newObjective.showOffScreenIcon = showOffScreen;
			newObjective.showDistance = showDistanceInfo;

			if (currentObjectiveIconElement.changeObjectiveColors && !removeCustomObjectiveColor) {
				Component[] components = obj.GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer child in components) {
					if (child.material.HasProperty ("_Color")) {
						for (int j = 0; j < child.materials.Length; j++) {
							newObjective.materials.Add (child.materials [j]);
							newObjective.originalColor.Add (child.materials [j].color);
							if (useCustomObjectiveColor) {
								child.materials [j].color = objectiveColor;
							} else {
								child.materials [j].color = currentObjectiveIconElement.objectiveColor;
							}
						}
					}
				}
			}
	
			objectiveList.Add (newObjective);

			for (int i = 0; i < playerScreenObjectivesList.Count; i++) {
				playerScreenObjectivesList [i].addElementToList (newObjective, currentObjectiveIconElement.iconInfoElement.gameObject, currentID);
			}
		} else {
			print ("Element not found in objective icon list");
		}

		currentID++;
	}

	public int getCurrentID ()
	{
		return currentID;
	}

	public void increaseCurrentID ()
	{
		currentID++;
	}

	public void addElementFromPlayerList (objectiveInfo newObjectiveInfo, objectiveIconElement currentObjectiveIconElement, Transform currentPlayer, bool addIconOnRestOfPlayers)
	{
		objectiveList.Add (newObjectiveInfo);

		if (addIconOnRestOfPlayers) {
			for (int i = 0; i < playerScreenObjectivesList.Count; i++) {
				if (playerScreenObjectivesList [i].player != currentPlayer) {
					playerScreenObjectivesList [i].addElementToList (newObjectiveInfo, currentObjectiveIconElement.iconInfoElement.gameObject, newObjectiveInfo.ID);
				}
			}
		}
	}

	//if the target is reached, disable all the parameters and clear the list, so a new objective can be added in any moment
	public void removeElementFromList (objectiveInfo objectiveListElement)
	{
		checkObjectiveInfoOnMapSystem (objectiveListElement);

		if (objectiveListElement.materials.Count > 0) {
			StartCoroutine (changeObjectColors (objectiveListElement, true));
		} else {
			removeElementFromObjectiveList (objectiveListElement);
		}
	}

	public void removeGameObjectFromList (GameObject objectToSearch)
	{
		for (int j = 0; j < objectiveList.Count; j++) {
			if (objectiveList [j].mapObject == objectToSearch) {
				removeElementFromList (objectiveList [j]);
			}
		}
	}

	public void removeGameObjectListFromList (List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++) {
			for (int j = 0; j < objectiveList.Count; j++) {
				if (objectiveList [j].mapObject == list [i]) {
					if (mapCreatorManager) {
						mapCreatorManager.removeMapObject (objectiveList [j].mapObject, true);
					}
					removeElementFromObjectiveList (objectiveList [j]);
				}
			}
		}
	}

	IEnumerator changeObjectColors (objectiveInfo objectiveListElement, bool removeElement)
	{
		if (objectiveListElement.materials.Count != 0) {
			for (float t = 0; t < 1;) {
				t += Time.deltaTime;
				for (int k = 0; k < objectiveListElement.materials.Count; k++) {
					objectiveListElement.materials [k].color = Color.Lerp (objectiveListElement.materials [k].color, objectiveListElement.originalColor [k], t / 3);
				}
				yield return null;
			}
		}
		if (removeElement) {
			removeElementFromObjectiveList (objectiveListElement);
		}
	}

	public void removeElementFromObjectiveList (objectiveInfo objectiveListElement)
	{
		for (int i = 0; i < playerScreenObjectivesList.Count; i++) {
			playerScreenObjectivesList [i].removeElementFromList (objectiveListElement.ID);
		}
		objectiveList.Remove (objectiveListElement);
	}

	public void removeElementFromObjectiveListCalledByPlayer (int objectId, GameObject currentPlayer)
	{
		for (int i = 0; i < objectiveList.Count; i++) {
			if (objectiveList [i].ID == objectId) {
				
				checkObjectiveInfoOnMapSystem (objectiveList [i]);

				if (i < objectiveList.Count && objectiveList [i].ID == objectId) {
					if (objectiveList [i].materials.Count > 0) {
						StartCoroutine (changeObjectColors (objectiveList [i], false));
					} else {
						objectiveList.Remove (objectiveList [i]);
					}
					print ("Screen Icon removed without the map system");
				} else {
					print ("Screen Icon removed with the map system");
				}

				for (int j = 0; j < playerScreenObjectivesList.Count; j++) {
					if (playerScreenObjectivesList [j].player != currentPlayer) {
						playerScreenObjectivesList [j].removeElementFromList (objectId);
					}
				}

				return;
			}
		}
	}

	public void checkObjectiveInfoOnMapSystem (objectiveInfo objectiveListElement)
	{
		if (objectiveListElement.mapObject) {
			bool isPathElement = false;
			mapObjectInformation currentMapObjectInformation = objectiveListElement.mapObject.GetComponent<mapObjectInformation> ();
			if (currentMapObjectInformation) {
				if (currentMapObjectInformation.typeName == "Path Element") {
					//objectiveListElement.mapObject.transform.parent.SendMessage ("pointReached", objectiveListElement.mapObject.transform, SendMessageOptions.DontRequireReceiver);
					isPathElement = true;
				}
				currentMapObjectInformation.checkPointReachedEvent ();
			}

			if (mapCreatorManager) {
				mapCreatorManager.removeMapObject (objectiveListElement.mapObject, isPathElement);
			}
		}
	}

	public void setScreenObjectiveVisibleState (bool state, GameObject mapObject)
	{
		for (int j = 0; j < objectiveList.Count; j++) {
			if (objectiveList [j].mapObject == mapObject) {
				objectiveList [j].showIconPaused = state;
			}
		}
	}

	[System.Serializable]
	public class objectiveInfo
	{
		public string Name;
		public GameObject mapObject;
		public RectTransform iconTransform;
		public GameObject onScreenIcon;
		public GameObject offScreenIcon;
		public Text iconText;

		public int ID;

		public bool useCloseDistance;
		public float closeDistance;
		public bool showOffScreenIcon;
		public bool showDistance;
		public float iconOffset;
		public bool showIconPaused;
		[HideInInspector] public List<Material> materials = new List<Material> ();
		[HideInInspector] public List<Color> originalColor = new List<Color> ();
	}

	[System.Serializable]
	public class objectiveIconElement
	{
		public string name;
		public objectiveIconInfo iconInfoElement;
		public bool changeObjectiveColors = true;
		public Color objectiveColor;
		public float minDefaultDistance = 10;
	}
}