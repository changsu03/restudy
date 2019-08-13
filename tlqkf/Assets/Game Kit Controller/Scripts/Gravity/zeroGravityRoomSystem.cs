using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class zeroGravityRoomSystem : MonoBehaviour
{
	public bool roomHasRegularGravity;
	public bool roomHasZeroGravity;
	public Transform gravityDirectionTransform;

	public bool objectsAffectedByGravity = true;
	public bool charactersAffectedByGravity = true;

	public bool changeGravityForceForObjects;
	public float newGravityForceForObjects;

	public bool changeGravityForceForCharacters;
	public float newGravityForceForCharacters;

	public bool addForceToObjectsOnZeroGravityState;
	public float forceAmountToObjectOnZeroGravity;
	public Transform forceDirectionToObjectsOnZeroGravity;
	public ForceMode forceModeToObjectsOnZeroGravity;

	public bool addInitialForceToObjectsOnZeroGravityState;
	public float initialForceToObjectsOnZeroGravity;
	public float initialForceRadiusToObjectsOnZeroGravity;
	public Transform initialForcePositionToObjectsOnZeroGravity;

	public List<string> nonAffectedObjectsTagList = new List<string> ();
	public List<string> charactersTagList = new List<string> ();
	public string playerTag;

	public List<Transform> roomPointsList = new List<Transform> ();
	public List<Vector2> roomPoints2DList = new List<Vector2> ();
	public List<GameObject> objectsInsideList = new List<GameObject> ();
	public List<GameObject> charactersInsideList = new List<GameObject> ();

	public bool addObjectsInsideParent;
	public Transform objectsInsideParent;

	public Transform highestPointPoisition;
	public Transform lowestPointPosition;

	public List<objectInfo> objectInfoList = new List<objectInfo> ();
	public bool debugModeActive;
	public bool debugModeListActive;

	public bool useSounds;
	public AudioSource mainAudioSource;
	public AudioClip regularGravitySound;
	public AudioClip zeroGravitySound;
	public AudioClip customGravitySound;

	public bool useSoundsOnCharacters;
	public AudioClip soundOnEntering;
	public AudioClip soundOnExiting;

	public bool showGizmo = true;
	public Vector3 centerGizmoScale = Vector3.one;
	public Color roomCenterColor = Color.gray;
	public Color gizmoLabelColor = Color.white;
	public Color linesColor = Color.yellow;
	public bool useHandleForWaypoints;

	public Vector3 roomCenter;

	zeroGravityRoomSystem currentGravityRoonSystem;
	Vector3 currentGravityDirection;

	void Start ()
	{
		for (int i = 0; i < roomPointsList.Count; i++) { 
			roomPoints2DList.Add (new Vector2 (roomPointsList [i].position.x, roomPointsList [i].position.z));
		}
		currentGravityRoonSystem = GetComponent<zeroGravityRoomSystem> ();

		currentGravityDirection = gravityDirectionTransform.up;

		if (addObjectsInsideParent) {
			addChildsFromParent (objectsInsideParent);
		}
			
		for (int i = 0; i < objectsInsideList.Count; i++) { 
			if (objectsInsideList [i]) {
				setObjectInsideState (objectsInsideList [i]);
			} else {
				print ("WARNING: some object in the object inside list has been removed or doesn't exists," +
				" make sure there are no empty elements in the Gravity Room " + gameObject.name);
			}
		}

		if (addInitialForceToObjectsOnZeroGravityState && roomHasZeroGravity) {
			addExplosionForceToObjectsOnZeroGravity ();
		}
	}

	void Update ()
	{
		if (debugModeActive) {
			for (int k = 0; k < objectInfoList.Count; k++) { 
				Vector2 currentPosition = new Vector2 (objectInfoList [k].objectTransform.position.x, objectInfoList [k].objectTransform.position.z);
				int j = roomPoints2DList.Count - 1; 
				bool inside = false; 
				for (int i = 0; i < roomPoints2DList.Count; j = i++) { 
					if (((roomPoints2DList [i].y <= currentPosition.y && currentPosition.y < roomPoints2DList [j].y) ||
					    (roomPoints2DList [j].y <= currentPosition.y && currentPosition.y < roomPoints2DList [i].y)) &&
					    (currentPosition.x < (roomPoints2DList [j].x - roomPoints2DList [i].x) * (currentPosition.y - roomPoints2DList [i].y) /
					    (roomPoints2DList [j].y - roomPoints2DList [i].y) + roomPoints2DList [i].x)) {
						inside = !inside;
					}
				} 
				objectInfoList [k].isInside = inside; 
			}
		}
	}

	public void checkObjectToAdd (GameObject newObject)
	{
		if (charactersTagList.Contains (newObject.tag)) {
			if (!charactersInsideList.Contains (newObject)) {
				charactersInsideList.Add (newObject);

				playerController currentPlayerController = newObject.GetComponent<playerController> ();
				gravitySystem currentGravitySystem = newObject.GetComponent<gravitySystem> ();

				if (!currentGravitySystem.isCharacterRotatingToSurface () && currentGravitySystem.getCurrentZeroGravityRoom () != currentGravityRoonSystem) {

					if (useSoundsOnCharacters) {
						playSound (soundOnEntering);
					}

					print ("inside " + gameObject.name);

					//the character is inside the room
					if (newObject.tag == playerTag) {
						if (currentGravitySystem) {
							currentGravitySystem.setCurrentZeroGravityRoom (currentGravityRoonSystem);
						}
					}

					if (charactersAffectedByGravity) { 
						if (roomHasRegularGravity) {
							currentGravitySystem.setZeroGravityModeOnState (false);
						} else if (roomHasZeroGravity) {
							currentGravitySystem.setZeroGravityModeOnState (true);
						} else {
							currentGravitySystem.setZeroGravityModeOnStateWithOutRotation (false);
							currentGravitySystem.changeGravityDirectionDirectlyInvertedValue (currentGravityDirection, true);
						}

						if (changeGravityForceForCharacters) {
							if (currentPlayerController) {
								currentPlayerController.setGravityForceValue (false, newGravityForceForCharacters);
							}
						}
					}
				}
			}
		}

		if (nonAffectedObjectsTagList.Contains (newObject.tag) || FindParentWithTag (newObject, nonAffectedObjectsTagList)) {
			return;
		}
			
		if (!objectsInsideList.Contains (newObject)) {
			objectsInsideList.Add (newObject);

			setObjectInsideState (newObject);
		}
	}

	public void checkObjectToRemove (GameObject newObject)
	{
		//check characters state
		if (charactersTagList.Contains (newObject.tag)) {
			if (charactersInsideList.Contains (newObject)) {

				playerController currentPlayerController = newObject.GetComponent<playerController> ();
				gravitySystem currentGravitySystem = newObject.GetComponent<gravitySystem> ();

				Vector3 characterPosition = currentGravitySystem.getGravityCenter ().position;
				Vector2 objectPosition = new Vector2 (characterPosition.x, characterPosition.z);

				//the character is inside the room
				if (!ContainsPoint (objectPosition) || !checkInsideFarthestPosition (newObject.transform.position)) {
					//the character is outside the room
					if (currentGravitySystem && currentGravitySystem.getCurrentZeroGravityRoom () == currentGravityRoonSystem && !currentGravitySystem.isCharacterRotatingToSurface ()) {

						print ("outisde " + gameObject.name);

						if (useSoundsOnCharacters) {
							playSound (soundOnExiting);
						}

						if (charactersAffectedByGravity) { 
							currentGravitySystem.setZeroGravityModeOnState (false);

							if (changeGravityForceForCharacters) {
								if (currentPlayerController) {
									currentPlayerController.setGravityForceValue (true, 0);
									//print ("original");
								}
							}
						}

						if (newObject.tag == playerTag) {
							currentGravitySystem.setCurrentZeroGravityRoom (null);
						}
					}
						
					charactersInsideList.Remove (newObject);
				}
			}

			return;
		}

		//check objects state
		if (objectsInsideList.Contains (newObject)) {
			Vector2 objectPosition = new Vector2 (newObject.transform.position.x, newObject.transform.position.z);
			if (!ContainsPoint (objectPosition) || !checkInsideFarthestPosition (newObject.transform.position)) {

				removeObjectFromRoom (newObject);
			}
		}
	}

	public void removeObjectFromRoom (GameObject newObject)
	{
		objectsInsideList.Remove (newObject);

		if (debugModeListActive) {
			for (int i = 0; i < objectInfoList.Count; i++) { 
				if (objectInfoList [i].objectTransform == newObject.transform) {
					objectInfoList.RemoveAt (i);
				}
			}
		}

		artificialObjectGravity currentArtificialObjectGravity = newObject.GetComponent<artificialObjectGravity> ();

		Rigidbody currentRigidbody = newObject.GetComponent<Rigidbody> ();

		grabbedObjectState currentGrabbedObjectState = newObject.GetComponent<grabbedObjectState> ();
		if (currentGrabbedObjectState) {
			if (currentGrabbedObjectState.getCurrentZeroGravityRoom () == currentGravityRoonSystem) {
				currentGrabbedObjectState.setInsideZeroGravityRoomState (false);
				currentGrabbedObjectState.setCurrentZeroGravityRoom (null);

				if (currentArtificialObjectGravity) {
					if (currentGrabbedObjectState.isGrabbed ()) {
						currentArtificialObjectGravity.removeJustGravityComponent ();
					} else {
						currentArtificialObjectGravity.removeGravityComponent ();
					}
				} 

				if (!currentGrabbedObjectState.isGrabbed ()) {
					currentRigidbody.useGravity = true;
				}
			}
		}
	}

	public void setObjectInsideState (GameObject newObject)
	{
		if (newObject == null) {
			return;
		}

		Rigidbody currentRigidbody = newObject.GetComponent<Rigidbody> ();

		if (currentRigidbody) {

			grabbedObjectState currentGrabbedObjectState = newObject.GetComponent<grabbedObjectState> ();

			if (!currentGrabbedObjectState) {
				currentGrabbedObjectState = newObject.AddComponent<grabbedObjectState> ();
			}
			if (currentGrabbedObjectState) {
				currentGrabbedObjectState.setInsideZeroGravityRoomState (true);
				currentGrabbedObjectState.setCurrentZeroGravityRoom (currentGravityRoonSystem);
			}

			artificialObjectGravity currentArtificialObjectGravity = newObject.GetComponent<artificialObjectGravity> ();

			if (roomHasRegularGravity || !objectsAffectedByGravity) {
				if (currentArtificialObjectGravity) {
					currentArtificialObjectGravity.removeGravity ();
				} else {
					currentRigidbody.useGravity = true;
				}
			} else if (roomHasZeroGravity) {
				if (currentArtificialObjectGravity) {
					currentArtificialObjectGravity.removeGravity ();
				}
				currentRigidbody.useGravity = false;
			} else {
				if (!currentArtificialObjectGravity) {
					currentArtificialObjectGravity = newObject.AddComponent<artificialObjectGravity> ();
				} 

				currentArtificialObjectGravity.setCurrentGravity (currentGravityDirection);
			}

			if (debugModeListActive) {
				objectInfo newObjectInfo = new objectInfo ();
				newObjectInfo.objectTransform = newObject.transform;
				objectInfoList.Add (newObjectInfo);
			}

			if (changeGravityForceForObjects && currentArtificialObjectGravity) {
				currentArtificialObjectGravity.setGravityForceValue (false, -newGravityForceForObjects);
			}
		}
	}

	public bool ContainsPoint (Vector2 currentPosition)
	{ 
		int j = roomPoints2DList.Count - 1; 
		bool inside = false; 
		for (int i = 0; i < roomPoints2DList.Count; j = i++) { 
			if (((roomPoints2DList [i].y <= currentPosition.y && currentPosition.y < roomPoints2DList [j].y) ||
			    (roomPoints2DList [j].y <= currentPosition.y && currentPosition.y < roomPoints2DList [i].y)) &&
			    (currentPosition.x < (roomPoints2DList [j].x - roomPoints2DList [i].x) * (currentPosition.y - roomPoints2DList [i].y) /
			    (roomPoints2DList [j].y - roomPoints2DList [i].y) + roomPoints2DList [i].x)) {
				inside = !inside;
			}
		} 
		return inside; 
	}

	public bool checkInsideFarthestPosition (Vector3 currentPosition)
	{
		bool inside = false;
		if (currentPosition.y < highestPointPoisition.position.y && currentPosition.y > lowestPointPosition.position.y) {
			inside = true;
		}

		return inside;
	}

	public bool objectInsideRoom (GameObject objectToCheck)
	{
		if (objectsInsideList.Contains (objectToCheck)) {
			return true;
		}
		return false;
	}

	public bool FindParentWithTag (GameObject childObject, List<string> tagList)
	{
		Transform t = childObject.transform;
		while (t.parent != null) {
			if (tagList.Contains (t.parent.tag)) {
				return true;
			}
			t = t.parent;
		}
		return false;
	}

	public void setNewGravityForceValueForObjectsInsideRoom (float newGravityForceValue)
	{
		setGravityForceValueForObjectsInsideRoom (false, newGravityForceValue);
	}

	public void setOriginalGravityForceValueForObjectsInsideRoom ()
	{
		setGravityForceValueForObjectsInsideRoom (true, 0);
	}

	public void setGravityForceValueForObjectsInsideRoom (bool setOriginal, float newValue)
	{
		for (int i = 0; i < objectsInsideList.Count; i++) { 
			if (changeGravityForceForObjects) {
				if (objectsInsideList [i]) {
					artificialObjectGravity currentArtificialObjectGravity = objectsInsideList [i].GetComponent<artificialObjectGravity> ();
					if (currentArtificialObjectGravity) {
						currentArtificialObjectGravity.setGravityForceValue (setOriginal, newValue);
					}
				}
			}
		}

		for (int i = 0; i < charactersInsideList.Count; i++) { 
			if (changeGravityForceForCharacters) {
				playerController currentPlayerController = charactersInsideList [i].GetComponent<playerController> ();
				if (currentPlayerController) {
					currentPlayerController.setGravityForceValue (setOriginal, newValue);
				}
			}
		}
	}

	public void setZeroGravityStateOnRoom ()
	{
		if (roomHasZeroGravity) {
			return;
		}

		playSound (zeroGravitySound);
	
		roomHasRegularGravity = false;
		roomHasZeroGravity = true;

		updateObjectsAndCharactersGravityInsideRoom ();

		addForceToObjectsOnZeroGravity ();
	}

	public void setRegularGravityStateOnRoom ()
	{
		if (roomHasRegularGravity) {
			return;
		}

		playSound (regularGravitySound);

		roomHasRegularGravity = true;
		roomHasZeroGravity = false;

		updateObjectsAndCharactersGravityInsideRoom ();
	}

	public void setCustomGravityStateOnRoom ()
	{
		playSound (customGravitySound);

		roomHasRegularGravity = false;
		roomHasZeroGravity = false;

		updateObjectsAndCharactersGravityInsideRoom ();
	}

	public void setCustomGravityStateOnRoom (Transform newGravityDirectionTransform)
	{
		currentGravityDirection = newGravityDirectionTransform.up;
		roomHasRegularGravity = false;
		roomHasZeroGravity = false;

		updateObjectsAndCharactersGravityInsideRoom ();
	}

	public void setOriginalGravityDirectionStateOnRoom ()
	{
		currentGravityDirection = gravityDirectionTransform.up;
		roomHasRegularGravity = false;
		roomHasZeroGravity = false;

		updateObjectsAndCharactersGravityInsideRoom ();
	}

	public void updateObjectsAndCharactersGravityInsideRoom ()
	{
		for (int i = 0; i < objectsInsideList.Count; i++) { 
			if (objectsInsideList [i]) {
				setObjectInsideState (objectsInsideList [i]);
			}
		}

		setCharactersGravityStateOnRoom (roomHasRegularGravity, roomHasZeroGravity);
	}

	public void setCharactersGravityStateOnRoom (bool hasRegularGravity, bool hasZeroGravity)
	{
		if (charactersAffectedByGravity) {
			for (int i = 0; i < charactersInsideList.Count; i++) { 
			
				playerController currentPlayerController = charactersInsideList [i].GetComponent<playerController> ();

				gravitySystem currentGravitySystem = charactersInsideList [i].GetComponent<gravitySystem> ();
				if (hasRegularGravity) {
					currentGravitySystem.setZeroGravityModeOnState (false);
				} else if (hasZeroGravity) {
					currentGravitySystem.setZeroGravityModeOnState (true);
				} else {
					currentGravitySystem.setZeroGravityModeOnStateWithOutRotation (false);
					currentGravitySystem.changeGravityDirectionDirectlyInvertedValue (currentGravityDirection, true);
				}

				if (changeGravityForceForCharacters) {
					if (currentPlayerController) {
						currentPlayerController.setGravityForceValue (false, newGravityForceForCharacters);
					}
				}
			}
		}
	}

	public void addForceToObjectsOnZeroGravity ()
	{
		if (addForceToObjectsOnZeroGravityState) {
			for (int i = 0; i < objectsInsideList.Count; i++) { 
				if (objectsInsideList [i]) {
					objectsInsideList [i].GetComponent<Rigidbody> ().AddForce (forceDirectionToObjectsOnZeroGravity.up * forceAmountToObjectOnZeroGravity, forceModeToObjectsOnZeroGravity);
				} else {
					print ("WARNING: some object in the object inside list has been removed or doesn't exists," +
					" make sure there are no empty elements in the Gravity Room " + gameObject.name);
				}
			}
		}
	}

	public void addExplosionForceToObjectsOnZeroGravity ()
	{
		for (int i = 0; i < objectsInsideList.Count; i++) { 
			if (objectsInsideList [i]) {
				objectsInsideList [i].GetComponent<Rigidbody> ().AddExplosionForce (initialForceToObjectsOnZeroGravity, initialForcePositionToObjectsOnZeroGravity.position,
					initialForceRadiusToObjectsOnZeroGravity);
			} else {
				print ("WARNING: some object in the object inside list has been removed or doesn't exists," +
				" make sure there are no empty elements in the Gravity Room " + gameObject.name);
			}
		}
	}

	public void playSound (AudioClip sound)
	{
		if (useSounds) {
			if (mainAudioSource) {
				mainAudioSource.PlayOneShot (sound);
			}
		}
	}

	public void addObjectToRoom (GameObject objectToAdd)
	{
		checkObjectToAdd (objectToAdd);

	}

	public void addObjectsToRoom (Transform parentToCheck)
	{
		Component[] childrens = parentToCheck.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Component c in childrens) {
			Rigidbody currentRigidbody = c as Rigidbody;
			if (currentRigidbody && !currentRigidbody.isKinematic && currentRigidbody.GetComponent<Collider> ()) {
				checkObjectToAdd (currentRigidbody.gameObject);
			}
		}
	}

	public void addChildsFromParent (Transform parentToCheck)
	{
		Component[] childrens = parentToCheck.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Component c in childrens) {
			Rigidbody currentRigidbody = c as Rigidbody;
			if (currentRigidbody && !currentRigidbody.isKinematic && currentRigidbody.GetComponent<Collider> ()) {
				objectsInsideList.Add (currentRigidbody.gameObject);
			}
		}
	}

	public float offsetOnNewRoomPoint = 2;

	public void renameRoomPoints ()
	{
		for (int i = 0; i < roomPointsList.Count; i++) { 
			roomPointsList [i].name = (i + 1).ToString ();

		}
		updateComponent ();
	}

	public void addRoomPoint ()
	{
		GameObject newRoomPoint = new GameObject ();
		newRoomPoint.transform.SetParent (transform);

		newRoomPoint.name = (roomPointsList.Count + 1).ToString ();

		Vector3 newPosition = Vector3.zero;

		if (roomPointsList.Count > 0) {
			newPosition = roomPointsList [roomPointsList.Count - 1].transform.localPosition + Vector3.right * offsetOnNewRoomPoint + Vector3.forward * offsetOnNewRoomPoint;
		}
		newRoomPoint.transform.localPosition = newPosition;

		roomPointsList.Add (newRoomPoint.transform);

		updateComponent ();
	}

	public void addRoomPoint (int index)
	{
		GameObject newRoomPoint = new GameObject ();
		newRoomPoint.transform.SetParent (transform);

		Vector3 newPosition = roomPointsList [index].transform.localPosition + Vector3.right * offsetOnNewRoomPoint + Vector3.forward * offsetOnNewRoomPoint;

		newRoomPoint.transform.localPosition = newPosition;

		roomPointsList.Insert (index + 1, newRoomPoint.transform);

		renameRoomPoints ();

		updateComponent ();
	}

	public void removeRoomPoint (int index)
	{
		if (roomPointsList [index]) {
			DestroyImmediate (roomPointsList [index].gameObject);
		}

		roomPointsList.RemoveAt (index);

		updateComponent ();
	}

	public void removeAllRoomPoints ()
	{
		for (int i = 0; i < roomPointsList.Count; i++) { 
			DestroyImmediate (roomPointsList [i].gameObject);

		}
		roomPointsList.Clear ();

		updateComponent ();
	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<zeroGravityRoomSystem> ());
		#endif
	}


	//draw every floor position and a line between floors
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			roomCenter = Vector3.zero;
			for (int i = 0; i < roomPointsList.Count; i++) {
				if (roomPointsList [i] != null) {
					if (i + 1 < roomPointsList.Count) {
						if (roomPointsList [i + 1] != null) {
							Gizmos.color = linesColor;
							Gizmos.DrawLine (roomPointsList [i].position, roomPointsList [i + 1].position);
						}
					}
					if (i == roomPointsList.Count - 1) {
						if (roomPointsList [0] != null) {
							Gizmos.color = linesColor;
							Gizmos.DrawLine (roomPointsList [i].position, roomPointsList [0].position);
						}
					}
					roomCenter += roomPointsList [i].position;
				} 
			}
			roomCenter /= roomPointsList.Count;

			Gizmos.color = Color.white;
			Gizmos.DrawLine (highestPointPoisition.position, roomCenter);
			Gizmos.DrawSphere (highestPointPoisition.position, 0.2f);
			Gizmos.DrawLine (lowestPointPosition.position, roomCenter);
			Gizmos.DrawSphere (lowestPointPosition.position, 0.2f);

			Gizmos.color = roomCenterColor;
			Gizmos.DrawCube (roomCenter, centerGizmoScale);
		}
	}

	[System.Serializable]
	public class objectInfo
	{
		public Transform objectTransform;
		public bool isInside;
	}

	[System.Serializable]
	public class characterInfo
	{
		public GameObject characterGameObject;
		public gravitySystem currentGravitySystem;
	}
}
