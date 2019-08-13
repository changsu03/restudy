using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class examineObjectSystemPlayerManagement : MonoBehaviour
{
	public bool examiningObject;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	examineObjectSystem currentExanimeObject;

	public void setExaminingObjectState (bool state)
	{
		examiningObject = state;

		checkEventsOnStateChange (examiningObject);
	}

	public void setcurrentExanimeObject (examineObjectSystem newExamineObject)
	{
		currentExanimeObject = newExamineObject;
	}

	//CALL INPUT FUNCTIONS TO EXAMINE OBJECTS
	public void examineObjectInputSetZoomValue (bool value)
	{
		if (currentExanimeObject) {
			currentExanimeObject.inputSetZoomValue (value);
		}
	}

	public void examineObjectInputResetRotation ()
	{
		if (currentExanimeObject) {
			currentExanimeObject.inputResetRotation ();
		}
	}

	public void examineObjectInputResetRotationAndPosition ()
	{
		if (currentExanimeObject) {
			currentExanimeObject.inputResetRotationAndPosition ();
		}
	}

	public void examineObjectInputCancelExamine ()
	{
		if (currentExanimeObject) {
			currentExanimeObject.inputCancelExamine ();
		}
	}

	public void examineObjectInputCheckIfMessage ()
	{
		if (currentExanimeObject) {
			currentExanimeObject.inputCheckIfMessage ();
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
