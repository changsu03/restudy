using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarManagementSystem : MonoBehaviour
{
	public List<healthSliderInfo> healthSliderInfoList = new List<healthSliderInfo> ();

	public List<playerHealthBarManagementSystem> playerHealthBarManagementSystemList = new List<playerHealthBarManagementSystem> ();

	int currentID = 0;

	public void addNewPlayer (playerHealthBarManagementSystem newPlayer)
	{
		playerHealthBarManagementSystemList.Add (newPlayer);
	}

	public void disableHealhtBars ()
	{
		enableOrDisableHealhtBars (false);
	}

	public void enableHealhtBars ()
	{
		enableOrDisableHealhtBars (true);
	}

	public void enableOrDisableHealhtBars (bool state)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].enableOrDisableHealhtBars (state);
		}
	}

	public int addNewTargetSlider (GameObject sliderOwner, GameObject sliderPrefab, Vector3 sliderOffset, float healthAmount, string ownerName, 
	                               Color textColor, Color sliderColor)
	{
		healthSliderInfo newHealthSliderInfo = new healthSliderInfo ();
		newHealthSliderInfo.Name = ownerName;
		newHealthSliderInfo.sliderOwner = sliderOwner.transform;

		currentID++;

		newHealthSliderInfo.ID = currentID;

		healthSliderInfoList.Add (newHealthSliderInfo);

		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].addNewTargetSlider (sliderOwner, sliderPrefab, sliderOffset, healthAmount, ownerName, textColor, sliderColor, currentID);
		}

		return currentID;
	}

	public void removeTargetSlider (int objectID)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].removeTargetSlider (objectID);
		}

		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectID) {
				healthSliderInfoList.RemoveAt (i);
				return;
			}
		}
	}

	public void removeElementFromObjectiveListCalledByPlayer (int objectId, GameObject currentPlayer)
	{
		for (int i = 0; i < healthSliderInfoList.Count; i++) {
			if (healthSliderInfoList [i].ID == objectId) {

				healthSliderInfoList.Remove (healthSliderInfoList [i]);

				for (int j = 0; j < playerHealthBarManagementSystemList.Count; j++) {
					if (playerHealthBarManagementSystemList [j].playerGameObject != currentPlayer) {
						playerHealthBarManagementSystemList [j].removeTargetSlider (objectId);
					}
				}

				return;
			}
		}
	}

	public void udpateSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderInfo (objectID, newName, textColor, backgroundColor);
		}
	}

	public void updateSliderAmount (int objectID, float value)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderAmount (objectID, value);
		}
	}

	public void setSliderAmount (int objectID, float sliderValue)
	{
		updateSliderAmount (objectID, sliderValue);
	}

	public void setSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		udpateSliderInfo (objectID, newName, textColor, backgroundColor);
	}

	public void setSliderVisibleStateForPlayer (int objectID, GameObject player, bool state)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			if (playerHealthBarManagementSystemList [i].getPlayerGameObject () == player) {
				playerHealthBarManagementSystemList [i].setSliderVisibleState (objectID, state);
			}
		}
	}

	public void setSliderVisibleState (int objectID, bool state)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].setSliderVisibleState (objectID, state);
		}
	}

	public void pauseOrResumeShowHealthSliders (bool state)
	{
		for (int i = 0; i < playerHealthBarManagementSystemList.Count; i++) {
			playerHealthBarManagementSystemList [i].pauseOrResumeShowHealthSliders (state);
		}
	}
}
