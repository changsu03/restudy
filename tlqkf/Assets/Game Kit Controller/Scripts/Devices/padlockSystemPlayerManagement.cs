using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class padlockSystemPlayerManagement : MonoBehaviour
{
	public bool usingPadlock;

	padlockSystem currentPadlockSystem;

	public void setCurrentPadlockSystem (padlockSystem newPadlockSystem)
	{
		currentPadlockSystem = newPadlockSystem;
	}

	public void setUsingPadlockState (bool state)
	{
		usingPadlock = state;
	}

	//CALL INPUT FUNCTIONS TO CURRENT PUZZLE SYSTEM
	public void inputRotateWheel (bool directionUp)
	{
		if (usingPadlock && currentPadlockSystem) {
			currentPadlockSystem.inputRotateWheel (directionUp);
		}
	}
}
