using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEventActivator : MonoBehaviour
{
	public string remoteEventToCall;

	public bool useAmount;
	public float amountValue;

	public bool useBool;
	public bool boolValue;

	public bool useGameObject;
	public GameObject gameObjectToUse;

	public bool useTransform;
	public Transform transformToUse;

	GameObject currentObjectToCall;

	public void callRemoteEvent (GameObject objectToCall)
	{
		currentObjectToCall = objectToCall;

		callEvent ();
	}

	public void setObjectToCall (GameObject objectToCall)
	{
		currentObjectToCall = objectToCall;
	}

	public void callEvent ()
	{
		remoteEventSystem currentRemoteEventSystem = currentObjectToCall.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem) {
			if (useAmount) {
				currentRemoteEventSystem.callRemoveEventWithAmount (remoteEventToCall, amountValue);
			} else if (useBool) {
				currentRemoteEventSystem.callRemoveEventWithBool (remoteEventToCall, boolValue);
			} else if (useGameObject) {
				currentRemoteEventSystem.callRemoveEventWithGameObject (remoteEventToCall, gameObjectToUse);
			} else if (useTransform) {
				currentRemoteEventSystem.callRemoveEventWithTransform (remoteEventToCall, transformToUse);
			} else {
				currentRemoteEventSystem.callRemoveEvent (remoteEventToCall);
			}
		}
	}
}
