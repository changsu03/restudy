using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialActivatorSystem : MonoBehaviour
{
	public string tutorialName;

	public string extraString = "Controls";

	public bool tutorialActive = true;

	GameObject currentPlayer;

	public void activateTutorial (GameObject newPlayer)
	{
		if (!tutorialActive) {
			return;
		}

		currentPlayer = newPlayer;

		if (currentPlayer) {
			playerInputManager currentPlayerInputManager = currentPlayer.GetComponent<playerInputManager> ();

			if (currentPlayerInputManager) {
				playerTutorialSystem currentplayerTutorialSystem = currentPlayerInputManager.getPauseManager ().getPlayerTutorialSystem ();
				if (currentplayerTutorialSystem) {
					currentplayerTutorialSystem.activateTutorialByName (tutorialName + " " + extraString);
				}
			}
		} else {
			print ("WARNING: no player has been sent in the function call, check it is properly configured");
		}
	}
}
