using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class elevatorFloorsPanel : MonoBehaviour
{
	public GameObject elevator;
	public GameObject floorButton;
	public RectTransform floorListContent;
	elevatorSystem elevatorManager;
	public List<GameObject> floorButtonList = new List<GameObject> ();
	int i;
	bool usingPanel;
	electronicDevice deviceManager;

	void Start ()
	{
		elevatorManager = elevator.GetComponent<elevatorSystem> ();
		int floorsAmount = elevatorManager.floors.Count;
		for (i = 0; i < floorsAmount; i++) {
			GameObject newIconButton = (GameObject)Instantiate (floorButton, Vector3.zero, floorButton.transform.rotation);
			newIconButton.transform.SetParent (floorListContent.transform);
			newIconButton.transform.localScale = Vector3.one;
			newIconButton.transform.localPosition = Vector3.zero;
			newIconButton.transform.GetChild (0).GetComponent<Text> ().text = elevatorManager.floors [i].floorNumber.ToString();
			newIconButton.name = "floor - " + (i + 1).ToString ();
			Button button = newIconButton.GetComponent<Button> ();
			button.onClick.AddListener (() => {
				goToFloor (button);
			});
			floorButtonList.Add (newIconButton);
		}
		Destroy (floorButton);
		deviceManager = GetComponent<electronicDevice> ();
	}

	//activate the device
	public void activateElevatorFloorPanel ()
	{
		usingPanel = !usingPanel;
	}

	public void goToFloor (Button button)
	{
		int index = -1;
		for (i = 0; i < floorButtonList.Count; i++) {
			if (floorButtonList [i] == button.gameObject) {
				index = i;
				if (elevatorManager.goToNumberFloor (index)) {
					deviceManager.setDeviceState (false);
				}
				return;
			}
		}
	}
}