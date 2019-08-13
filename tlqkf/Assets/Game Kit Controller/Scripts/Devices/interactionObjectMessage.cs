using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interactionObjectMessage : MonoBehaviour
{

	[TextArea (3, 10)] public string message;
	public bool usingDevice;
	public List<string> tagToDetect = new List<string> ();
	public bool pausePlayerWhileReading;
	public bool pressSecondTimeToStopReading;
	public float showMessageTime;
	public bool moveCameraToPosition;

	public bool callEventOnInteraction;
	public bool callEventOnEveryInteraction;
	public UnityEvent eventOnInteraction;
	public UnityEvent eventOnEndInteraction;

	public bool messageRemoved;
	public bool interactionUsed;

	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	menuPause pauseManager;
	usingDevicesSystem usingDevicesManager;
	float lastTimeUsed;
	moveCameraToDevice cameraMovementManager;

	void Start ()
	{
		cameraMovementManager = GetComponent<moveCameraToDevice> ();
	}

	void Update ()
	{
		if (usingDevice) {
			if ((!pausePlayerWhileReading || !pressSecondTimeToStopReading) && Time.time > lastTimeUsed + showMessageTime) {
				activateDevice ();
			}
		}
	}

	public void activateDevice ()
	{
		if (messageRemoved) {
			return;
		}

		if (!pressSecondTimeToStopReading && usingDevice && showMessageTime == 0) {
			return;
		}

		usingDevice = !usingDevice;

		if (pausePlayerWhileReading) {
			setDeviceState (usingDevice);
		}

		if (moveCameraToPosition && cameraMovementManager) {
			cameraMovementManager.moveCamera (usingDevice);
		}

		if (usingDevice) {
			usingDevicesManager.checkShowObjectMessage (message, showMessageTime);
			lastTimeUsed = Time.time;
		} else {
			if (showMessageTime == 0) {
				usingDevicesManager.stopShowObjectMessage ();
			}
		}

		if (callEventOnInteraction) {
			if (callEventOnEveryInteraction) {
				if (usingDevice) {
					eventOnInteraction.Invoke ();
				} else {
					eventOnEndInteraction.Invoke ();
				}
			} else {
				if (!interactionUsed) {
					if (usingDevice) {
						eventOnInteraction.Invoke ();
					} else {
						eventOnEndInteraction.Invoke ();
					}
				}

				if (!usingDevice) {
					interactionUsed = true;
				}
			}
		}
	}

	//check when the player enters or exits of the trigger in the device
	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		//if the player is entering in the trigger
		if (isEnter) {
			//if the device is already being used, return
			if (usingDevice) {
				return;
			}

			if (tagToDetect.Contains (col.tag)) {
				currentPlayer = col.gameObject;
				currentPlayerControllerManager = currentPlayer.GetComponent<playerController> ();
				usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
				pauseManager = currentPlayer.GetComponent<playerInputManager> ().getPauseManager ();

				if (cameraMovementManager) {
					cameraMovementManager.setCurrentPlayer (currentPlayer);
				}
			}
		} else {
			//if the player is leaving the trigger
			if (tagToDetect.Contains (col.tag)) {
				
				//if the player is the same that was using the device, the device can be used again
				if (col.gameObject == currentPlayer) {
					currentPlayer = null;
				}
			}
		}
	}

	public void setDeviceState (bool state)
	{
		currentPlayerControllerManager.setUsingDeviceState (state);

		pauseManager.usingDeviceState (state);

		currentPlayerControllerManager.changeScriptState (!state);
	}

	public void removeMessage ()
	{
		messageRemoved = true;

		if (usingDevicesManager) {
			usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
		}
	}
}
