using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setMapOrientationSystem : MonoBehaviour
{
	public mapCameraMovement mapCameraMovementType;

	public Transform mapOrientationTransform;

	public enum mapCameraMovement
	{
		XY,
		XZ,
		YZ
	}

	GameObject currentPlayer;


	public void setMapOrientation ()
	{
		if (currentPlayer) {
			playerController playerControllermanager = currentPlayer.GetComponent<playerController> ();

			mapSystem currentMapSystem = playerControllermanager.getPlayerInput ().getPauseManager ().getMapSystem ();
			if (currentMapSystem) {
				currentMapSystem.setMapOrientation ((int)mapCameraMovementType, mapOrientationTransform);
			}
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}
}
