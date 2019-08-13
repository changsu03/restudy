using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

public class computerDevice : MonoBehaviour
{
	public bool locked;
	public GameObject keyboard;
	public Text currentCode;
	public Text stateText;
	public string code;
	public GameObject computerLockedContent;
	public GameObject computerUnlockedContent;
	public Color unlockedColor;
	public Image wallPaper;
	public AudioClip wrongPassSound;
	public AudioClip corretPassSound;
	public AudioClip keyPressSound;
	public int numberLetterToStartToMovePasswordText;
	public float passwordTextMoveDistacePerLetter;
	public bool allowToUseKeyboard;
	public List<string> allowedKeysList = new List<string> ();
	public bool usingDevice;

	public List<Image> keysList = new List<Image> ();

	public bool disableUseDeviceActionButton;

	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();
	int totalKeysPressed = 0;
	bool changeScreen;
	bool unlocked;
	bool changedColor;
	bool touchPlatform;
	GameObject currentCaptured;
	Touch currentTouch;
	AudioSource audioSource;
	electronicDevice deviceManager;
	inputManager input;
	string currentKeyPressed;
	bool letterAdded;
	Vector3 originalCurrentCodePosition;
	string originalKeyPressed;
	bool capsLockActivated;

	GameObject currentPlayer;
	usingDevicesSystem usingDevicesManager;
	float lastTimeComputerActive;

	void Start ()
	{
		//get all the keys button in the keyboard and store them in a list
		if (keyboard) {
			Component[] components = keyboard.GetComponentsInChildren (typeof(Image));
			foreach (Component c in components) {
				keysList.Add (c.gameObject.GetComponent<Image> ());
			}
		}
		touchPlatform = touchJoystick.checkTouchPlatform ();
		if (!locked) {
			if (computerLockedContent) {
				computerLockedContent.SetActive (false);
			}
			if (computerUnlockedContent) {
				computerUnlockedContent.SetActive (true);
			}
		}
		audioSource = GetComponent<AudioSource> ();
		deviceManager = GetComponent<electronicDevice> ();
		input = FindObjectOfType<inputManager> ();
		if (currentCode) {
			originalCurrentCodePosition = currentCode.GetComponent<RectTransform> ().localPosition;
		}
	}

	void Update ()
	{
		//if the computer is locked and the player is inside its trigger 
		if (!unlocked && usingDevice && locked) {
			//get all the input touchs, including the mouse
			int touchCount = Input.touchCount;
			if (!touchPlatform) {
				touchCount++;
			}
			for (int i = 0; i < touchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}
				//get a list with all the objects under mouse or the finger tap
				captureRaycastResults.Clear ();
				PointerEventData p = new PointerEventData (EventSystem.current);
				p.position = currentTouch.position;
				p.clickCount = i;
				p.dragging = false;
				EventSystem.current.RaycastAll (p, captureRaycastResults);
				foreach (RaycastResult r in captureRaycastResults) {
					currentCaptured = r.gameObject;
					//check the current key pressed with the finger
					if (currentTouch.phase == TouchPhase.Began) {
						checkButton (currentCaptured);
					}
				}
			}

			if (Time.time > lastTimeComputerActive + 0.3f) {
				if (allowToUseKeyboard) {
					currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyDown, false);
					if (currentKeyPressed != "") {
						checkKeyPressed (currentKeyPressed);
					}

					currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyUp, false);
					if (currentKeyPressed.ToLower () == "leftshift") {
						capsLockActivated = !capsLockActivated;
					}
				}
			}
		}

		//if the device is unlocked, change the color of the interface for the unlocked color
		if (unlocked) {
			if (!changedColor) {
				wallPaper.color = Vector4.MoveTowards (wallPaper.color, unlockedColor, Time.deltaTime * 3);
				stateText.color = Vector4.MoveTowards (stateText.color, unlockedColor, Time.deltaTime * 3);
				if (wallPaper.color == unlockedColor && stateText.color == unlockedColor) {
					changedColor = true;
				}
			} else {
				//change the password screen for the unlocked screen
				if (changeScreen) {
					computerLockedContent.SetActive (false);
					computerUnlockedContent.SetActive (true);
					changeScreen = false;
				}
			}
		}
	}

	//activate the device
	public void activateComputer ()
	{
		usingDevice = !usingDevice;

		if (disableUseDeviceActionButton) {
			if (!currentPlayer) {
				currentPlayer = deviceManager.getCurrentPlayer ();
				usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
			}

			if (currentPlayer) {
				usingDevicesManager.setUseDeviceButtonEnabledState (!usingDevice);
			}
		}

		if (usingDevice) {
			lastTimeComputerActive = Time.time;
		}
	}

	//the currentCaptured is checked, to write the value of the key in the screen device
	void checkButton (GameObject button)
	{
		if (button.GetComponent<Image> ()) {
			//check if the currentCaptured is a key number
			if (keysList.Contains (button.GetComponent<Image> ())) {
				checkKeyPressed (button.name);
			}
		}
	}

	public void checkKeyPressed (string keyPressed)
	{
		originalKeyPressed = keyPressed;
		keyPressed = keyPressed.ToLower ();
		if (!allowedKeysList.Contains (keyPressed)) {
			return;
		}
		bool checkPass = false;
		letterAdded = true;

		print (keyPressed);

		//reset the password in the screen
		if (totalKeysPressed == 0) {
			currentCode.text = "";
		}	

		if (keyPressed.Contains ("alpha")) {
			print (keyPressed);
			keyPressed = keyPressed.Substring (keyPressed.Length - 1);
			originalKeyPressed = keyPressed;
			print (keyPressed);
		}

		//add the an space
		if (keyPressed == "space") {
			currentCode.text += " ";
		}

		//delete the last character
		else if (keyPressed == "delete" || keyPressed == "backspace") {
			if (currentCode.text.Length > 0) {
				currentCode.text = currentCode.text.Remove (currentCode.text.Length - 1);
				letterAdded = false;
			}
		}

		//check the current word added
		else if (keyPressed == "enter" || keyPressed == "return") {
			checkPass = true;
		} 

		//check if the caps are being using
		else if (keyPressed == "capslock" || keyPressed == "leftshift") {
			capsLockActivated = !capsLockActivated;
			return;
		}

		//add the current key pressed to the password
		else {
			if (capsLockActivated) {
				originalKeyPressed = originalKeyPressed.ToUpper ();
			} else {
				originalKeyPressed = originalKeyPressed.ToLower ();
			}
			currentCode.text += originalKeyPressed;
		}

		totalKeysPressed = currentCode.text.Length;
		//play the key press sound
		audioSource.PlayOneShot (keyPressSound);

		//the enter key has been pressed, so check if the current text written is the correct password
		if (checkPass) {
			if (currentCode.text == code) {
				enableAccessToCompturer ();
			} 
			//else, reset the terminal, and try again
			else {
				audioSource.PlayOneShot (wrongPassSound);
				currentCode.text = "Password";
				totalKeysPressed = 0;
				currentCode.GetComponent<RectTransform> ().localPosition = originalCurrentCodePosition;
			}
		} else if (totalKeysPressed > numberLetterToStartToMovePasswordText) {
			if (letterAdded) {
				currentCode.GetComponent<RectTransform> ().localPosition -= new Vector3 (passwordTextMoveDistacePerLetter, 0, 0);
			} else {
				currentCode.GetComponent<RectTransform> ().localPosition += new Vector3 (passwordTextMoveDistacePerLetter, 0, 0);
			}
		}
	}

	//if the object to unlock is this device, change the screen
	public void unlockComputer ()
	{
		changeScreen = true;
	}

	public void enableAccessToCompturer ()
	{
		//if it is equal, then call the object to unlock and play the corret pass sound
		audioSource.PlayOneShot (corretPassSound);
		stateText.text = "Unlocked";
		unlocked = true;
		//the object to unlock can be also this terminal, to see more content inside it
		deviceManager.unlockObject ();
	}

	public void unlockComputerWithUsb ()
	{
		unlockComputer ();
		enableAccessToCompturer ();
	}
}