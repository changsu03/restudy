using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveLogSystem : MonoBehaviour
{
	public bool objectiveMenuActive = true;
	public bool objectiveMenuOpened;
	public GameObject objectiveMenuGameObject;
	public GameObject objectiveSlotPrefab;

	public Text objectiveNameText;
	public Text objectiveDescriptionText;
	public Text objectiveFullDescriptionText;

	public Image activeObjectiveButtonImage;
	public Image cancelObjectiveButtonImage;

	public Color buttonUsable;
	public Color buttonNotUsable;

	public Transform objectiveListContent;

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public gameManager mainGameManager;

	public List<objectiveSlotInfo> objectiveSlotInfoList = new List<objectiveSlotInfo> ();

	objectiveSlotInfo currentObjectiveSlot;
	objectiveEventSystem currentObjectiveEventManager;

	void Start ()
	{
		updateObjectiveTextContent ("", "", "");
	}

	public void setButtonsColor (bool activeObjectiveColor, bool cancelObjectiveColor)
	{
		if (activeObjectiveColor) {
			activeObjectiveButtonImage.color = buttonUsable;
		} else {
			activeObjectiveButtonImage.color = buttonNotUsable;
		}
		if (cancelObjectiveColor) {
			cancelObjectiveButtonImage.color = buttonUsable;
		} else {
			cancelObjectiveButtonImage.color = buttonNotUsable;
		}
	}

	public void activeObjective ()
	{
		if (currentObjectiveSlot != null) {
			if (!currentObjectiveSlot.objectiveEventManager.objectiveComplete) {
				currentObjectiveSlot.objectiveEventManager.startObjective ();
				currentObjectiveSlot.currentObjectiveIcon.SetActive (true);
				setButtonsColor (false, true);

				currentObjectiveEventManager = currentObjectiveSlot.objectiveEventManager;
			}
		}
	}

	public void cancelObjective ()
	{
		if (currentObjectiveSlot != null) {
			if (!currentObjectiveSlot.objectiveEventManager.objectiveComplete) {
				currentObjectiveSlot.objectiveEventManager.stopObjective ();
				currentObjectiveSlot.currentObjectiveIcon.SetActive (false);
				setButtonsColor (true, false);
			}
		}
	}

	public void objectiveComplete (objectiveEventSystem currentObjectiveEventSystem)
	{
		for (int i = 0; i < objectiveSlotInfoList.Count; i++) {
			if (objectiveSlotInfoList [i].objectiveEventManager == currentObjectiveEventSystem) {
				objectiveSlotInfoList [i].currentObjectiveIcon.SetActive (false);
				objectiveSlotInfoList [i].objectiveCompletePanel.SetActive (true);
				objectiveSlotInfoList [i].objectiveCompleteText.SetActive (true);
			}
		}
	}

	public void activeObjective (objectiveEventSystem currentObjectiveEventSystem)
	{
		for (int i = 0; i < objectiveSlotInfoList.Count; i++) {
			if (objectiveSlotInfoList [i].objectiveEventManager == currentObjectiveEventSystem) {
				objectiveSlotInfoList [i].currentObjectiveIcon.SetActive (true);

				currentObjectiveEventManager = objectiveSlotInfoList [i].objectiveEventManager;
			}
		}
	}

	public void cancelObjective (objectiveEventSystem currentObjectiveEventSystem)
	{
		for (int i = 0; i < objectiveSlotInfoList.Count; i++) {
			if (objectiveSlotInfoList [i].objectiveEventManager == currentObjectiveEventSystem) {
				objectiveSlotInfoList [i].currentObjectiveIcon.SetActive (false);
			}
		}
	}

	public void cancelPreviousObjective ()
	{
		if (currentObjectiveEventManager) {
			currentObjectiveEventManager.cancelPreviousObjective ();
		}
	}

	public void showObjectiveInformation (GameObject objectiveSlot)
	{
		if (currentObjectiveSlot != null) {
			currentObjectiveSlot.selectedObjectiveIcon.SetActive (false);
		}

		for (int i = 0; i < objectiveSlotInfoList.Count; i++) {
			if (objectiveSlotInfoList [i].objectiveSlotGameObject == objectiveSlot) {
				currentObjectiveSlot = objectiveSlotInfoList [i];

				updateObjectiveTextContent (currentObjectiveSlot.objectiveName, currentObjectiveSlot.objectiveDescription, currentObjectiveSlot.objectiveFullDescription);

				currentObjectiveSlot.selectedObjectiveIcon.SetActive (true);

				if (!currentObjectiveSlot.objectiveEventManager.objectiveComplete) {
					if (currentObjectiveSlot.objectiveEventManager.objectiveInProcess) {
						setButtonsColor (false, true);
					} else {
						setButtonsColor (true, false);
					}
				} else {
					setButtonsColor (false, false);
				}
				return;
			}
		}

		setButtonsColor (false, false);
	}

	public void updateObjectiveTextContent (string objectiveName, string objectiveDescription, string objectiveFullDescription)
	{
		objectiveNameText.text = objectiveName;
		objectiveDescriptionText.text = objectiveDescription;
		objectiveFullDescriptionText.text = objectiveFullDescription;
	}

	public void addObjective (string objectiveName, string objectiveDescription, string objectiveFullDescription, 
	                          string objectiveLocation, objectiveEventSystem currentObjectiveEventSystem)
	{
		for (int i = 0; i < objectiveSlotInfoList.Count; i++) {
			if (objectiveSlotInfoList [i].objectiveEventManager == currentObjectiveEventSystem) {
				return;
			}
		}

		GameObject newObjectiveSlotPrefab = (GameObject)Instantiate (objectiveSlotPrefab, objectiveSlotPrefab.transform.position, Quaternion.identity);
		newObjectiveSlotPrefab.SetActive (true);
		newObjectiveSlotPrefab.transform.SetParent (objectiveListContent);
		newObjectiveSlotPrefab.transform.localScale = Vector3.one;
		newObjectiveSlotPrefab.transform.localPosition = Vector3.zero;

		objectiveMenuIconElement currentobjectiveMenuIconElement = newObjectiveSlotPrefab.GetComponent<objectiveMenuIconElement> ();

		currentobjectiveMenuIconElement.objectiveNameText.text = objectiveName;
		currentobjectiveMenuIconElement.objectiveLocationText.text = objectiveLocation;
	

		objectiveSlotInfo newObjectiveSlotInfo = new objectiveSlotInfo ();
		newObjectiveSlotInfo.objectiveSlotGameObject = newObjectiveSlotPrefab;
		newObjectiveSlotInfo.objectiveName = objectiveName;
		newObjectiveSlotInfo.objectiveLocation = objectiveLocation;
		newObjectiveSlotInfo.objectiveDescription = objectiveDescription;
		newObjectiveSlotInfo.objectiveFullDescription = objectiveFullDescription;
		newObjectiveSlotInfo.currentObjectiveIcon = currentobjectiveMenuIconElement.currentObjectiveIcon;
		newObjectiveSlotInfo.objectiveCompletePanel = currentobjectiveMenuIconElement.objectiveCompletePanel;
		newObjectiveSlotInfo.selectedObjectiveIcon = currentobjectiveMenuIconElement.selectedObjectiveIcon;
		newObjectiveSlotInfo.objectiveCompleteText = currentobjectiveMenuIconElement.objectiveCompleteText;

		newObjectiveSlotInfo.currentObjectiveIcon.SetActive (true);


		newObjectiveSlotInfo.objectiveEventManager = currentObjectiveEventSystem;

		objectiveSlotInfoList.Add (newObjectiveSlotInfo);
	}

	public void openOrCloseObjectiveMenu (bool state)
	{
		if ((!pauseManager.playerMenuActive || objectiveMenuOpened) && (!playerControllerManager.usingDevice || playerControllerManager.isPlayerDriving ()) && !mainGameManager.isGamePaused ()) {
			objectiveMenuOpened = state;

			pauseManager.openOrClosePlayerMenu (objectiveMenuOpened, objectiveMenuGameObject.transform, true);

			pauseManager.setIngameMenuOpenedState ("Objective Log System", objectiveMenuOpened);

			objectiveMenuGameObject.SetActive (objectiveMenuOpened);

			//set to visible the cursor
			pauseManager.showOrHideCursor (objectiveMenuOpened);

			pauseManager.showOrHideMouseCursorController (objectiveMenuOpened);

			//disable the touch controls
			pauseManager.checkTouchControls (!objectiveMenuOpened);

			//disable the camera rotation
			pauseManager.changeCameraState (!objectiveMenuOpened);
			playerControllerManager.changeScriptState (!objectiveMenuOpened);
			pauseManager.usingSubMenuState (objectiveMenuOpened);

			if (currentObjectiveSlot != null) {
				if (currentObjectiveSlot.currentObjectiveIcon) {
					currentObjectiveSlot.currentObjectiveIcon.SetActive (false);
				}
			}

			currentObjectiveSlot = null;

			setButtonsColor (false, false);

			updateObjectiveTextContent ("", "", "");
		}
	}

	public void openOrCLoseObjectiveMenuFromTouch ()
	{
		openOrCloseObjectiveMenu (!objectiveMenuOpened);
	}

	public void inputOpenOrCloseObjectiveMenu ()
	{
		if (objectiveMenuActive) {
			openOrCloseObjectiveMenu (!objectiveMenuOpened);
		}
	}

	[System.Serializable]
	public class objectiveSlotInfo
	{
		public GameObject objectiveSlotGameObject;
		public string objectiveName;
		public string objectiveDescription;
		public string objectiveFullDescription;
		public string objectiveLocation;
		public GameObject currentObjectiveIcon;
		public GameObject objectiveCompletePanel;
		public GameObject selectedObjectiveIcon;
		public GameObject objectiveCompleteText;
		public objectiveEventSystem objectiveEventManager;
	}
}
