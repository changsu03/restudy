using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class simpleSwitch : MonoBehaviour
{
	public bool buttonEnabled = true;

	public AudioClip pressSound;
	public bool sendCurrentUser;
	public bool notUsableWhileAnimationIsPlaying = true;
	public bool useSingleSwitch = true;
	public string switchAnimationName = "simpleSwitch";

	public float animationSpeed = 1;

	public bool useUnityEvents = true;
	public UnityEvent objectToCallFunctions = new UnityEvent ();

	public UnityEvent turnOnEvent = new UnityEvent ();
	public UnityEvent turnOffEvent = new UnityEvent ();

	public GameObject objectToActive;
	public string activeFunctionName;
	public bool sendThisButton;

	AudioSource audioSource;
	Animation buttonAnimation;
	GameObject currentPlayer;
	bool firstAnimationPlay = true;
	public bool switchTurnedOn;
	deviceStringAction deviceStringActionManager;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		buttonAnimation = GetComponent<Animation> ();
		deviceStringActionManager = GetComponent<deviceStringAction> ();
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void setCurrentUser (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void activateDevice ()
	{
		if (!buttonEnabled) {
			return;
		}

		//check if the player is inside the trigger, and if he press the button to activate the devide
		if ((!buttonAnimation.IsPlaying (switchAnimationName) && notUsableWhileAnimationIsPlaying) || !notUsableWhileAnimationIsPlaying) {

			if (useSingleSwitch) {
				playSingleAnimation ();
			} else {
				switchTurnedOn = !switchTurnedOn;
				playDualAnimation (switchTurnedOn);
				setDeviceStringActionState (switchTurnedOn);
			}

			if (sendCurrentUser && currentPlayer) {
				objectToActive.SendMessage ("setCurrentUser", currentPlayer, SendMessageOptions.DontRequireReceiver);
			}

			if (useUnityEvents) {
				if (useSingleSwitch) {
					if (objectToCallFunctions.GetPersistentEventCount () > 0) {
						objectToCallFunctions.Invoke ();
					}
				} else {
					if (switchTurnedOn) {
						if (turnOnEvent.GetPersistentEventCount () > 0) {
							turnOnEvent.Invoke ();
						}
					} else {
						if (turnOffEvent.GetPersistentEventCount () > 0) {
							turnOffEvent.Invoke ();
						}
					}
				}
			} else {
				if (objectToActive) {
					if (sendThisButton) {
						objectToActive.SendMessage (activeFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
					} else {
						objectToActive.SendMessage (activeFunctionName, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}

	public void setButtonEnabledState (bool state)
	{
		buttonEnabled = state;
	}

	public void triggerButtonEventFromEditor ()
	{
		activateDevice ();
	}

	public void playSingleAnimation ()
	{
		buttonAnimation [switchAnimationName].speed = animationSpeed;
		buttonAnimation.Play (switchAnimationName);
		audioSource.PlayOneShot (pressSound);
	}

	public void playDualAnimation (bool playForward)
	{
		if (playForward) {
			if (!buttonAnimation.IsPlaying (switchAnimationName)) {
				buttonAnimation [switchAnimationName].normalizedTime = 0;
			}
			buttonAnimation [switchAnimationName].speed = 1;
		} else {
			if (!buttonAnimation.IsPlaying (switchAnimationName)) {
				buttonAnimation [switchAnimationName].normalizedTime = 1;
			}
			buttonAnimation [switchAnimationName].speed = -1; 
		}
		if (firstAnimationPlay) {
			buttonAnimation.Play (switchAnimationName);
			firstAnimationPlay = false;
		} else {
			buttonAnimation.CrossFade (switchAnimationName);
		}
		audioSource.PlayOneShot (pressSound);
	}

	public void setDeviceStringActionState (bool state)
	{
		if (deviceStringActionManager) {
			deviceStringActionManager.changeActionName (state);
		}
	}
}