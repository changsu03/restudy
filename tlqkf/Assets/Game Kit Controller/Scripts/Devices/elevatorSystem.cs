using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class elevatorSystem : MonoBehaviour
{
	public List<floorInfo> floors = new List<floorInfo> ();
	public int currentFloor;
	public float speed;
	public bool hasInsideElevatorDoor;
	public GameObject insideElevatorDoor;
	public GameObject elevatorSwitchPrefab;
	public bool addSwitchInNewFloors;
	public GameObject elevatorDoorPrefab;
	public bool addDoorInNewFloors;
	public bool moving;
	public bool doorsClosed = true;

	public bool changeIconFloorWhenMoving;

	public bool showGizmo;
	public Color gizmoLabelColor;

	bool inside;
	int i;
	bool lockedElevator;
	bool closingDoors;
	Coroutine elevatorMovement;
	mapObjectInformation mapObjectInformationManager;

	public List<passengersInfo> passengersInfoList = new List<passengersInfo> ();

	doorSystem insideElevatorDoorSystem;
	doorSystem outsideElevatorDoorSystem;

	void Start ()
	{
		mapObjectInformationManager = GetComponent<mapObjectInformation> ();

		if (insideElevatorDoor) {
			insideElevatorDoorSystem = insideElevatorDoor.GetComponent<doorSystem> ();
		}
	}

	void Update ()
	{
		//check if there is doors in the elevator to close them and start the elevator movement when they are closed
		if (closingDoors) {
			if (insideElevatorDoor) {
				if (insideElevatorDoorSystem.doorState == doorSystem.doorCurrentState.closed) {
					closingDoors = false;
					checkElevatorMovement ();
				}
			} else {
				closingDoors = false;
				checkElevatorMovement ();
			}
		}
	}

	//the player has press the button move up, so increase the current floor count
	public void nextFloor ()
	{
		getFloorNumberToMove (1);
	}

	//the player has press the button move down, so decrease the current floor count
	public void previousFloor ()
	{
		getFloorNumberToMove (-1);
	}

	int currentDirectionToMove = 1;

	public void moveBetweenTwoPositions ()
	{
		getFloorNumberToMove (currentDirectionToMove);
		currentDirectionToMove *= -1;
	}

	//move to the floor, according to the direction selected by the player
	void getFloorNumberToMove (int direction)
	{
		//if the player is inside the elevator and it is not moving, then 
		if (inside && !moving) {
			//change the current floor to the next or the previous
			int floorIndex = currentFloor + direction;
			//check that the floor exists, and start to move the elevator to that floor position
			if (floorIndex < floors.Count && floorIndex >= 0) {
				openOrCloseElevatorDoors ();
				currentFloor = floorIndex;
				closingDoors = true;
				setAllPlayersParent (transform);
			}
		}
	}

	//move to the floor, according to the direction selected by the player
	public bool goToNumberFloor (int floorNumber)
	{
		bool canMoveToFloor = false;
		//if the player is inside the elevator and it is not moving, then 
		if (inside && !moving) {
			//check that the floor exists, and start to move the elevator to that floor position
			if (floorNumber < floors.Count && floorNumber >= 0 && floorNumber != currentFloor) {
				openOrCloseElevatorDoors ();
				currentFloor = floorNumber;
				closingDoors = true;
				setAllPlayersParent (transform);
				canMoveToFloor = true;
			}
		}
		return canMoveToFloor;
	}

	//when a elevator button is pressed, move the elevator to that floor
	public void callElevator (GameObject button)
	{
		if (moving) {
			return;
		}

		for (i = 0; i < floors.Count; i++) {
			if (floors [i].floorButton == button) {
				lockedElevator = false;
				if (floors [currentFloor].outsideElevatorDoor) {
					if (floors [currentFloor].outsideElevatorDoor.GetComponent<doorSystem> ().locked) {
						lockedElevator = true;
					}
				}
				if (!lockedElevator) {
					if (currentFloor != i) {
						if (!doorsClosed) {
							openOrCloseElevatorDoors ();
						}
						currentFloor = i;
						closingDoors = true;
					} else {
						openOrCloseElevatorDoors ();
					}
				}
			}
		}
	}

	//open or close the inside and outside doors of the elevator if the elevator has every of this doors
	void openOrCloseElevatorDoors ()
	{
		if (insideElevatorDoor) {
			if (insideElevatorDoorSystem.doorState == doorSystem.doorCurrentState.closed) {
				doorsClosed = false;
			} else {
				doorsClosed = true;
			}
			insideElevatorDoorSystem.changeDoorsStateByButton ();
		}

		if (floors [currentFloor].outsideElevatorDoor) {
			hologramDoor currentHologramDoor = floors [currentFloor].outsideElevatorDoor.GetComponentInChildren<hologramDoor> ();
			if (currentHologramDoor) {
				currentHologramDoor.openHologramDoorByExternalInput ();
			} else {
				floors [currentFloor].outsideElevatorDoor.GetComponent<doorSystem> ().changeDoorsStateByButton ();
			}
		}
	}

	//stop the current elevator movement and start it again
	void checkElevatorMovement ()
	{
		if (elevatorMovement != null) {
			StopCoroutine (elevatorMovement);
		}
		elevatorMovement = StartCoroutine (moveElevator ());
	}

	IEnumerator moveElevator ()
	{
		moving = true;
		//move the elevator from its position to the currentfloor
		Vector3 currentElevatorPosition = transform.localPosition;

		Vector3 targetPosition = floors [currentFloor].floorPosition.localPosition;
		Quaternion targetRotation = floors [currentFloor].floorPosition.localRotation;

		bool rotateElevator = false;
		if (targetRotation != Quaternion.identity || transform.rotation != Quaternion.identity) {
			rotateElevator = true;
		}
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * speed;
			transform.localPosition = Vector3.Lerp (currentElevatorPosition, targetPosition, t);
			if (rotateElevator) {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRotation, t);
			}
			yield return null;
		}

		//if the elevator reachs the correct floor, stop its movement, and deattach the player of its childs
		moving = false;
		setAllPlayersParent (null);
		openOrCloseElevatorDoors ();

		if (changeIconFloorWhenMoving) {
			if (mapObjectInformationManager) {
				mapObjectInformationManager.changeMapObjectIconFloorByPosition ();
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		//the player has entered in the elevator trigger, stored it and set the evelator as his parent
		if (col.tag == "Player") {
			addPassenger (col.gameObject.transform);

			if (passengersInfoList.Count > 0) {
				inside = true;
			}

			setPlayerParent (transform, col.gameObject.transform);
		}
	}

	void OnTriggerExit (Collider col)
	{
		//the player has gone of the elevator trigger, remove the parent from the player
		if (col.tag == "Player") {

			setPlayerParent (null, col.gameObject.transform);

			removePassenger (col.gameObject.transform);

			if (passengersInfoList.Count == 0) {
				inside = false;
			}

			if (!doorsClosed) {
				openOrCloseElevatorDoors ();
			}
		}
	}

	//attach and disattch the player and the camera inside the elevator
	void setPlayerParent (Transform father, Transform newPassenger)
	{
		bool passengerFound = false;
		passengersInfo newPassengersInfo = new passengersInfo ();
		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger && !passengerFound) {
				newPassengersInfo = passengersInfoList [i];
				passengerFound = true;
			}
		}

		if (passengerFound) {
			newPassengersInfo.playerControllerManager.setPlayerAndCameraParent (father);
		}
	}

	void setAllPlayersParent (Transform father)
	{
		for (i = 0; i < passengersInfoList.Count; i++) {
			passengersInfoList [i].playerControllerManager.setPlayerAndCameraParent (father);
		}
	}

	public void addPassenger (Transform newPassenger)
	{
		bool passengerFound = false;
		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger && !passengerFound) {
				passengerFound = true;
			}
		}

		if (!passengerFound) {
			passengersInfo newPassengersInfo = new passengersInfo ();
			newPassengersInfo.playerTransform = newPassenger;
			newPassengersInfo.playerControllerManager = newPassenger.GetComponent<playerController> ();
			passengersInfoList.Add (newPassengersInfo);
		}
	}

	void removePassenger (Transform newPassenger)
	{
		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger) {
				passengersInfoList.RemoveAt (i);
			}
		}
	}

	//add a new floor, with a switch and a door, if they are enabled to add them
	public void addNewFloor ()
	{
		floorInfo newFloorInfo = new floorInfo ();
		GameObject newFloor = new GameObject ();
		newFloor.transform.SetParent (transform.parent);
		Vector3 newFloorLocalposition = Vector3.zero;
		if (floors.Count > 0) {
			newFloorLocalposition = floors [floors.Count - 1].floorPosition.position + floors [floors.Count - 1].floorPosition.up * 5;
		}
		newFloor.transform.position = newFloorLocalposition;
		newFloor.name = "New Floor";
		newFloorInfo.name = newFloor.name;
		newFloorInfo.floorNumber = floors.Count;
		newFloorInfo.floorPosition = newFloor.transform;
		newFloorInfo.hasFloorButton = true;
		newFloorInfo.hasOutSideElevatorDoor = true;

		//add a switch
		if (addSwitchInNewFloors) {
			GameObject newSwitch = (GameObject)Instantiate (elevatorSwitchPrefab, Vector3.zero, Quaternion.identity);
			newSwitch.transform.SetParent (transform.parent);
			newSwitch.transform.position = newFloorLocalposition + newFloorInfo.floorPosition.forward * 10;
			newSwitch.name = "Elevator Switch";
			newFloorInfo.floorButton = newSwitch;
			newSwitch.transform.SetParent (newFloor.transform);
			simpleSwitch currentSimpleSwitch = newSwitch.GetComponent<simpleSwitch> ();
			currentSimpleSwitch.objectToActive = gameObject;
			currentSimpleSwitch.activeFunctionName = "callElevator";
			#if UNITY_EDITOR
			EditorUtility.SetDirty (currentSimpleSwitch);
			#endif
		}

		//add a door
		if (addDoorInNewFloors) {
			GameObject newDoor = (GameObject)Instantiate (elevatorDoorPrefab, Vector3.zero, Quaternion.identity);
			newDoor.transform.SetParent (transform.parent);
			newDoor.transform.position = newFloorLocalposition + newFloorInfo.floorPosition.forward * 5;
			newDoor.name = "Elevator Door";
			newFloorInfo.outsideElevatorDoor = newDoor;
			newDoor.transform.SetParent (newFloor.transform);
		}

		floors.Add (newFloorInfo);
		updateComponent ();
	}

	public void removeFloor (int floorIndex)
	{
		if (floors [floorIndex].floorPosition) {
			DestroyImmediate (floors [floorIndex].floorPosition.gameObject);
		}

		floors.RemoveAt (floorIndex);

		updateComponent ();
	}

	public void removeAllFloors ()
	{
		for (i = 0; i < floors.Count; i++) {
			DestroyImmediate (floors [i].floorPosition.gameObject);
		}
		floors.Clear ();
		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<elevatorSystem> ());
		#endif
	}

	//draw every floor position and a line between floors
	void OnDrawGizmos ()
	{
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
			if (!Application.isPlaying) {
				for (i = 0; i < floors.Count; i++) {
					if (floors [i].floorPosition) {
						Gizmos.color = Color.yellow;
						if (floors [i].floorNumber == currentFloor) {
							Gizmos.color = Color.red;
						}

						Gizmos.DrawSphere (floors [i].floorPosition.position, 0.6f);
						if (i + 1 < floors.Count && floors [i + 1].floorPosition) {
							Gizmos.color = Color.yellow;
							Gizmos.DrawLine (floors [i].floorPosition.position, floors [i + 1].floorPosition.position);
						}

						if (floors [i].floorButton) {
							Gizmos.color = Color.blue;
							Gizmos.DrawLine (floors [i].floorButton.transform.position, floors [i].floorPosition.position);
							Gizmos.color = Color.green;
							Gizmos.DrawSphere (floors [i].floorButton.transform.position, 0.3f);
							if (floors [i].outsideElevatorDoor) {
								Gizmos.color = Color.white;
								Gizmos.DrawLine (floors [i].floorButton.transform.position, floors [i].outsideElevatorDoor.transform.position);
							}
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class floorInfo
	{
		public string name;
		public int floorNumber;
		public Transform floorPosition;
		public bool hasFloorButton;
		public GameObject floorButton;
		public bool hasOutSideElevatorDoor;
		public GameObject outsideElevatorDoor;
	}

	[System.Serializable]
	public class passengersInfo
	{
		public Transform playerTransform;
		public playerController playerControllerManager;
	}
}