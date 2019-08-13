using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class puzzleSystemPlayerManagement : MonoBehaviour
{
	public bool usingPuzzle;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	puzzleSystem currentPuzzleSystem;

	public void setcurrentPuzzleSystem (puzzleSystem newPuzzleObject)
	{
		currentPuzzleSystem = newPuzzleObject;
	}

	public void setUsingPuzzleState (bool state)
	{
		usingPuzzle = state;

		checkEventsOnStateChange (usingPuzzle);
	}

	//CALL INPUT FUNCTIONS TO CURRENT PUZZLE SYSTEM
	public void puzzleObjectInputSetRotateObjectState (bool state)
	{
		if (currentPuzzleSystem) {
			currentPuzzleSystem.inputSetRotateObjectState (state);
		}
	}

	public void puzzleObjectInputIncreaseObjectHolDistanceByButton (bool state)
	{
		if (currentPuzzleSystem) {
			currentPuzzleSystem.inputIncreaseObjectHolDistanceByButton (state);
		}
	}

	public void puzzleObjectInputDecreaseObjectHolDistanceByButton (bool state)
	{
		if (currentPuzzleSystem) {
			currentPuzzleSystem.inputDecreaseObjectHolDistanceByButton (state);
		}
	}

	public void puzzleObjectInputSetObjectHolDistanceByMouseWheel (bool state)
	{
		if (currentPuzzleSystem) {
			currentPuzzleSystem.inputSetObjectHolDistanceByMouseWheel (state);
		}
	}

	public void puzzleObjectInputResetPuzzle ()
	{
		if (currentPuzzleSystem) {
			currentPuzzleSystem.inputResetPuzzle ();
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}
}
