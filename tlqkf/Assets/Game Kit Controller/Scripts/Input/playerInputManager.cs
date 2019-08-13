using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class playerInputManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();
	public List<screenActionInfo> screenActionInfoList = new List<screenActionInfo> ();

	public int playerID;
	public bool inputEnabled = true;

	public bool showDebugActions;

	public menuPause pauseManager;

	public bool inputCurrentlyActive = true;

	public inputManager input;
	public playerController playerControllerManager;

	public bool ignoreGamepad;
	public bool allowKeyboardAndGamepad;
	public bool allowGamepadInTouchDevice;

	public touchJoystick touchMovementControl;
	public touchJoystick touchCameraControl;
	[Range (0, 4)] public float leftTouchSensitivity;
	[Range (0, 4)] public float rightTouchSensitivity;
	[Range (0, 4)] public float mouseSensitivity;
	public GameObject touchPanel;

	public bool usingTouchMovementJoystick = true;

	public bool useHorizontaTouchMovementButtons;
	public bool useVerticalTouchMovementButtons;

	public GameObject horizontalTouchMovementButtons;
	public GameObject verticalTouchMovementButtons;
	public float inputLerpSpeedTouchMovementButtons = 5;

	public bool pressingTouchHorizontalRightInput;
	public bool pressingTouchHorizontalLeftInput;

	public bool pressingTouchVerticalUpInput;
	public bool pressingTouchVerticalDownInput;

	public bool useOnlyKeyboard;

	public bool usingGamepad;
	public bool usingKeyBoard;
	public bool usingTouchControls;
	public bool gameManagerPaused;

	public string horizontalString = "Horizontal";
	public string verticalString = "Vertical";
	public string mouseXString = "Mouse X";
	public string mouseYString = "Mouse Y";
	public string horizontalXString = "Horizontal X";
	public string verticalYString = "Vertical Y";

	multiAxes currenMultiAxes;
	Axes currentAxes;

	public Vector2 movementAxis;
	public Vector2 mouseAxis;
	public Vector2 rawMovementAxis;

	public bool useMovementOrientation;
	[Range (-1, 1)] public int horizontalMovementOrientation = 1;
	[Range (-1, 1)] public int verticalMovementOrientation = 1;
	public bool useCameraOrientation;
	[Range (-1, 1)] public int horizontalCameraOrientation = 1;
	[Range (-1, 1)] public int verticalCameraOrientation = 1;

	public bool manualControlActive;
	[Range (-1, 1)] public float manualMovementHorizontalInput;
	[Range (-1, 1)] public float manualMovementVerticalInput;
	[Range (-1, 1)] public float manualMouseHorizontalInput;
	[Range (-1, 1)] public float manualMouseVerticalInput;

	public bool inputPaused;

	string playerIDString;

	bool keyboardActive;

	bool checkKeyUp;

	public bool overrideInputValuesActive;
	Vector2 overrideInputAxisValue;

	Vector2 auxMovementAxis;
	Vector2 auxRawMovementAxis;

	bool playerIsDead;

	bool originalUsingTouchMovementJoystickValue;

	void Start ()
	{
		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}

		if (useOnlyKeyboard) {
			playerControllerManager.setPlayerID (0);
		}

		playerID = playerControllerManager.getPlayerID ();
		playerIDString = playerID.ToString ();

		//set the position of the camera joystick according to the screen size
		if (touchCameraControl) {
			touchCameraControl.setJoystickPosition ();
		}

		//set the joystick position at the beginning of the game
		if (touchMovementControl) {
			touchMovementControl.setJoystickPosition ();
		}

		originalUsingTouchMovementJoystickValue = usingTouchMovementJoystick;
	}

	void Update ()
	{
		playerIsDead = playerControllerManager.isPlayerDead ();

		if (inputEnabled) {

			gameManagerPaused = input.isGameManagerPaused ();

			if (Input.anyKey || checkKeyUp) {
				for (int i = 0; i < multiAxesList.Count; i++) {

					currenMultiAxes = multiAxesList [i];

					if (currenMultiAxes.currentlyActive) {
						for (int j = 0; j < currenMultiAxes.axes.Count; j++) {

							currentAxes = multiAxesList [i].axes [j];

							if (currentAxes.actionEnabled) {
								if (inputCurrentlyActive || currentAxes.canBeUsedOnPausedGame) {
									if (input.checkPlayerInputButtonFromMultiAxesList (currenMultiAxes.multiAxesStringIndex, currentAxes.axesStringIndex, 
										    currentAxes.buttonPressType, playerID, currentAxes.canBeUsedOnPausedGame, useOnlyKeyboard)) {

										if (showDebugActions) {
											print (currentAxes.Name);
										}

										currentAxes.buttonEvent.Invoke ();
									}
								}
							}
						}
					}
				}

				checkKeyUp = true;
			} else {
				if (!gameManagerPaused) {
					checkKeyUp = false;
				}
			}
				
			//convert the input from keyboard or a touch screen into values to move the player, given the camera direction
			//also, it checks in the player is using a device, like a vehicle
			//convert the mouse input in the tilt angle for the camera or the input from the touch screen depending of the settings
			//also, it checks in the player is using a device, like a vehicle

			usingTouchControls = input.isUsingTouchControls ();

			usingKeyBoard = input.isUsingKeyBoard ();

			allowKeyboardAndGamepad = input.isAllowKeyboardAndGamepad ();

			allowGamepadInTouchDevice = input.isAllowGamepadInTouchDevice ();

			usingGamepad = input.isUsingGamepad ();

			if (usingKeyBoard || allowKeyboardAndGamepad || useOnlyKeyboard) {
				if (!usingTouchControls || (allowGamepadInTouchDevice && usingGamepad)) {
					movementAxis.x = Input.GetAxis (horizontalString);
					movementAxis.y = Input.GetAxis (verticalString);

					mouseAxis.x = Input.GetAxis (mouseXString);
					mouseAxis.y = Input.GetAxis (mouseYString);
					mouseAxis *= mouseSensitivity;

					rawMovementAxis.x = Input.GetAxisRaw (horizontalString);
					rawMovementAxis.y = Input.GetAxisRaw (verticalString);

					keyboardActive = true;
				} else {
					if (usingTouchMovementJoystick) {
						movementAxis = touchMovementControl.GetAxis () * leftTouchSensitivity;
					} else {
						float movementHorizontalValue = 0;
						float movementVerticalValue = 0;

						if (pressingTouchHorizontalRightInput) {
							movementHorizontalValue = 1;
						}

						if (pressingTouchHorizontalLeftInput) {
							movementHorizontalValue = -1;
						}

						if ((pressingTouchHorizontalRightInput && pressingTouchHorizontalLeftInput) || (!pressingTouchHorizontalRightInput && !pressingTouchHorizontalLeftInput)) {
							movementHorizontalValue = 0;
						}

						if (pressingTouchVerticalUpInput) {
							movementVerticalValue = 1;
						}

						if (pressingTouchVerticalDownInput) {
							movementVerticalValue = -1;
						}

						if ((pressingTouchVerticalUpInput && pressingTouchVerticalDownInput) || (!pressingTouchVerticalUpInput && !pressingTouchVerticalDownInput)) {
							movementVerticalValue = 0;
						}

						movementAxis = Vector2.MoveTowards (movementAxis, new Vector2 (movementHorizontalValue, movementVerticalValue) * leftTouchSensitivity, Time.deltaTime * inputLerpSpeedTouchMovementButtons);
					}


					mouseAxis = touchCameraControl.GetAxis () * rightTouchSensitivity;					

					if (usingTouchMovementJoystick) {
						rawMovementAxis = touchMovementControl.getRawAxis ();
					} else {
						if (movementAxis.x > 0) {
							rawMovementAxis.x = 1;
						} else if (movementAxis.x < 0) {
							rawMovementAxis.x = -1;
						} else {
							rawMovementAxis.x = 0;
						}

						if (movementAxis.y > 0) {
							rawMovementAxis.y = 1;
						} else if (movementAxis.y < 0) {
							rawMovementAxis.y = -1;
						} else {
							rawMovementAxis.y = 0;
						}
					}

					keyboardActive = false;
				}
			} else {
				keyboardActive = false;
			}

			if ((!usingKeyBoard || allowKeyboardAndGamepad) && !useOnlyKeyboard) {
				if (usingGamepad) {
					if (!keyboardActive) {
						movementAxis = Vector2.zero;
						mouseAxis = Vector2.zero;
						rawMovementAxis = Vector2.zero;
					}

					movementAxis.x += Input.GetAxis (horizontalXString + playerIDString);
					movementAxis.y += Input.GetAxis (verticalYString + playerIDString);

					mouseAxis.x += Input.GetAxis (mouseXString + playerIDString);
					mouseAxis.y += Input.GetAxis (mouseYString + playerIDString);
					mouseAxis *= mouseSensitivity;

					rawMovementAxis.x += Input.GetAxis (horizontalXString + playerIDString);
					rawMovementAxis.y += Input.GetAxis (verticalYString + playerIDString);
				}
			}

			if (useMovementOrientation) {
				movementAxis.x *= horizontalMovementOrientation;
				movementAxis.y *= verticalMovementOrientation;
				rawMovementAxis.x *= horizontalMovementOrientation;
				rawMovementAxis.y *= verticalMovementOrientation;
			}
				
			if (useCameraOrientation) {
				mouseAxis.x *= horizontalCameraOrientation;
				mouseAxis.y *= verticalCameraOrientation;
			}
		}

		if (overrideInputValuesActive) {

			auxMovementAxis = movementAxis;
			auxRawMovementAxis = rawMovementAxis;

			movementAxis = overrideInputAxisValue;

			if (movementAxis.x > 0) {
				rawMovementAxis.x = 1;
			} else if (movementAxis.x < 0) {
				rawMovementAxis.x = -1;
			} else {
				rawMovementAxis.x = 0;
			}

			if (movementAxis.y > 0) {
				rawMovementAxis.y = 1;
			} else if (movementAxis.y < 0) {
				rawMovementAxis.y = -1;
			} else {
				rawMovementAxis.y = 0;
			}
		}

		if (manualControlActive) {
			auxMovementAxis = movementAxis;
			auxRawMovementAxis = rawMovementAxis;

			movementAxis = new Vector2 (manualMovementHorizontalInput, manualMovementVerticalInput);

			if (movementAxis.x > 0) {
				rawMovementAxis.x = 1;
			} else if (movementAxis.x < 0) {
				rawMovementAxis.x = -1;
			} else {
				rawMovementAxis.x = 0;
			}

			if (movementAxis.y > 0) {
				rawMovementAxis.y = 1;
			} else if (movementAxis.y < 0) {
				rawMovementAxis.y = -1;
			} else {
				rawMovementAxis.y = 0;
			}

			mouseAxis.x = manualMouseHorizontalInput;
			mouseAxis.y = manualMouseVerticalInput;
		}
	}

	public Vector2 getAuxMovementAxis ()
	{
		return auxMovementAxis;
	}

	public Vector2 getAuxRawMovementAxis ()
	{
		return auxRawMovementAxis;
	}

	//get if the touch controls are enabled, so any other component can check it
	public void changeControlsType (bool state)
	{
		touchPanel.SetActive (state);
		touchCameraControl.gameObject.SetActive (state);

		enableOrDisableTouchMovementJoystickForButtons (state);
	}

	public void enableOrDisableTouchMovementJoystickForButtons (bool state)
	{
		if (usingTouchMovementJoystick) {
			touchMovementControl.gameObject.SetActive (state);

			horizontalTouchMovementButtons.SetActive (false);
			verticalTouchMovementButtons.SetActive (false);
		} else {
			touchMovementControl.gameObject.SetActive (false);

			if (useHorizontaTouchMovementButtons) {
				horizontalTouchMovementButtons.SetActive (state);
			} else {
				horizontalTouchMovementButtons.SetActive (false);
			}

			if (useVerticalTouchMovementButtons) {
				verticalTouchMovementButtons.SetActive (state);
			} else {
				verticalTouchMovementButtons.SetActive (false);
			}
		}
	}

	public void setOriginalTouchMovementInputState ()
	{
		setUsingTouchMovementJoystickState (originalUsingTouchMovementJoystickValue);
		enableOrDisableTouchMovementJoystickForButtons (true);
	}

	public void setUsingTouchMovementJoystickState (bool state)
	{
		usingTouchMovementJoystick = state;
	}

	public Vector2 getPlayerRawMovementAxis ()
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		if (inputPaused) {
			return Vector2.zero;
		}

		if (playerIsDead) {
			return Vector2.zero;
		}
			
		return rawMovementAxis;
	}

	//get the current values of the axis keys or the mouse in the input manager
	public Vector2 getPlayerMovementAxis (string controlType)
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		if (inputPaused) {
			return Vector2.zero;
		}

		if (playerIsDead) {
			return Vector2.zero;
		}
			
		if (controlType == "keys") {
			return movementAxis;
		}

		if (controlType == "mouse") {
			//mouseAxis = Vector2.ClampMagnitude (mouseAxis, 1);
			return mouseAxis;
		}

		return Vector2.zero;
	}

	public bool checkPlayerInputButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, inputManager.buttonType type, bool canBeUsedOnPausedGame)
	{
		if (!inputEnabled) {
			return false;
		}

		if (inputPaused) {
			return false;
		}

		if (playerIsDead) {
			return false;
		}
			
		return input.checkPlayerInputButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, playerID, canBeUsedOnPausedGame, useOnlyKeyboard);
	}

	public bool checkPlayerInputButtonWithoutEvents (int multiAxesStringIndex, int axesStringIndex, inputManager.buttonType buttonType)
	{
		if (!inputEnabled) {
			return false;
		}

		if (inputPaused) {
			return false;
		}

		if (playerIsDead) {
			return false;
		}

		return input.checkPlayerInputButtonFromMultiAxesList (multiAxesStringIndex, axesStringIndex, buttonType, playerID, false, useOnlyKeyboard);
	}

	public menuPause getPauseManager ()
	{
		return pauseManager;
	}

	public void setPlayerInputEnabledState (bool state)
	{
		inputEnabled = state;
	}

	public void setInputCurrentlyActiveState (bool state)
	{
		inputCurrentlyActive = state;
	}

	public void setPlayerID (int newID)
	{
		playerID = newID;
	}

	public void enableOrDisableActionScreen (string actionScreenName, bool state)
	{
		if (usingTouchControls) {
			return;
		}

		for (int i = 0; i < screenActionInfoList.Count; i++) {
			if (screenActionInfoList [i].screenActionName == actionScreenName) {
				if (screenActionInfoList [i].screenActionsGameObject) {
					screenActionInfoList [i].screenActionsGameObject.SetActive (state);
				}
			}
		}
	}

	public void setPlayerInputActionState (bool playerInputActionState, string multiAxesInputName, string axesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName == multiAxesInputName) {
				for (int j = 0; j < multiAxesList [i].axes.Count; j++) {
					if (multiAxesList [i].axes [j].Name == axesInputName) {
						multiAxesList [i].axes [j].actionEnabled = playerInputActionState;
					}
				}
			}
		}
	}

	public void setPlayerInputMultiAxesState (bool playerInputMultiAxesState, string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName == multiAxesInputName) {
				multiAxesList [i].currentlyActive = playerInputMultiAxesState;
			}
		}
	}

	public void enablePlayerInputMultiAxes (string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName == multiAxesInputName) {
				multiAxesList [i].currentlyActive = true;
			}
		}
	}

	public void disablePlayerInputMultiAxes (string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName == multiAxesInputName) {
				multiAxesList [i].currentlyActive = false;
			}
		}
	}

	public void overrideInputValues (Vector2 newInputValues, bool overrideState)
	{
		overrideInputAxisValue = newInputValues;
		overrideInputValuesActive = overrideState;
	}

	public inputManager getInputManager ()
	{
		return input;
	}

	public bool isUsingTouchControls ()
	{
		return usingTouchControls;
	}

	public string getButtonKey (string buttonName)
	{
		return input.getButtonKey (buttonName);
	}

	public void addNewAxes ()
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input) {
			multiAxes newMultiAxes = new multiAxes ();

			newMultiAxes.multiAxesStringList = new string[input.multiAxesList.Count];
			for (int i = 0; i < input.multiAxesList.Count; i++) {
				string axesName = input.multiAxesList [i].axesName;
				newMultiAxes.multiAxesStringList [i] = axesName;
			}
			multiAxesList.Add (newMultiAxes);

			updateComponent ();
		}
	}

	public void addNewAction (int multiAxesIndex)
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesIndex];
			Axes newAxes = new Axes ();
			newAxes.axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

			for (int i = 0; i < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; i++) {
				string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [i].Name;
				newAxes.axesStringList [i] = actionName;
			}

			newAxes.multiAxesStringIndex = multiAxesIndex;
			currentMultiAxesList.axes.Add (newAxes);

			updateComponent ();
		}
	}

	public void updateMultiAxesList ()
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input) {
			for (int i = 0; i < multiAxesList.Count; i++) {
				
				multiAxesList [i].multiAxesStringList = new string[input.multiAxesList.Count];

				for (int j = 0; j < input.multiAxesList.Count; j++) {
					string axesName = input.multiAxesList [j].axesName;
					multiAxesList [i].multiAxesStringList [j] = axesName;
				}
				multiAxesList [i].axesName = input.multiAxesList [multiAxesList [i].multiAxesStringIndex].axesName;
			}

			updateComponent ();
		}
	}

	public void updateAxesList (int multiAxesListIndex)
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesListIndex];

			for (int i = 0; i < currentMultiAxesList.axes.Count; i++) {
				
				currentMultiAxesList.axes [i].axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

				for (int j = 0; j < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; j++) {
					string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [j].Name;
					currentMultiAxesList.axes [i].axesStringList [j] = actionName;
				}

				if (i >= input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count) {
					currentMultiAxesList.axes [i].axesStringIndex = 0;
				}
			}

			updateComponent ();
		}
	}

	public void setAllAxesList (int multiAxesListIndex)
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesListIndex];
			currentMultiAxesList.axes.Clear ();

			for (int i = 0; i < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; i++) {
				Axes newAxes = new Axes ();
				newAxes.axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

				for (int j = 0; j < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; j++) {
					string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [j].Name;
					newAxes.axesStringList [j] = actionName;
				}

				newAxes.multiAxesStringIndex = multiAxesListIndex;
				newAxes.axesStringIndex = i;
				newAxes.Name = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [i].Name;
				newAxes.actionName = newAxes.Name;
				currentMultiAxesList.axes.Add (newAxes);
			}

			updateComponent ();
		}
	}

	public void updateCurrentInputValues ()
	{
		updateComponent ();
	}

	public void setMultiAxesEnabledState (bool state)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			multiAxesList [i].currentlyActive = state;
		}

		updateComponent ();
	}

	public void setAllActionsEnabledState (int multiAxesListIndex, bool state)
	{
		for (int j = 0; j < multiAxesList [multiAxesListIndex].axes.Count; j++) {
			multiAxesList [multiAxesListIndex].axes [j].actionEnabled = state;
		}

		updateComponent ();
	}

	public void setHorizontalMovementOrientationValue (int newValue)
	{
		horizontalMovementOrientation = newValue;
	}

	public void setVerticalMovementOrientationValue (int newValue)
	{
		verticalMovementOrientation = newValue;
	}

	public void setHorizontalCameraOrientationValue (int newValue)
	{
		horizontalCameraOrientation = newValue;
	}

	public void setVerticalCameratOrientationValue (int newValue)
	{
		verticalCameraOrientation = newValue;
	}

	public void setPressingTouchHorizontalRightInputState (bool state)
	{
		pressingTouchHorizontalRightInput = state;
	}

	public void setPressingTouchHorizontalLeftInputState (bool state)
	{
		pressingTouchHorizontalLeftInput = state;
	}

	public void setPressingTouchVerticalUpInputState (bool state)
	{
		pressingTouchVerticalUpInput = state;
	}

	public void setPressingTouchVerticalDownInputState (bool state)
	{
		pressingTouchVerticalDownInput = state;
	}

	public void pauseOrResumeInput (bool state)
	{
		inputPaused = state;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<playerInputManager> ());
		#endif
	}

	[System.Serializable]
	public class multiAxes
	{
		public string axesName;
		public List<Axes> axes = new List<Axes> ();
		public GameObject screenActionsGameObject;
		public bool currentlyActive = true;
		public int multiAxesStringIndex;
		public string[] multiAxesStringList;

		public multiAxes (multiAxes newState)
		{
			axesName = newState.axesName;
		}

		public multiAxes ()
		{
		}
	}

	[System.Serializable]
	public class Axes
	{
		public string actionName = "New Action";
		public string Name;
		public bool actionEnabled = true;

		public bool canBeUsedOnPausedGame;

		public inputManager.buttonType buttonPressType;

		public UnityEvent buttonEvent;

		public int axesStringIndex;
		public string[] axesStringList;

		public int multiAxesStringIndex;
	}

	[System.Serializable]
	public class screenActionInfo
	{
		public string screenActionName;
		public GameObject screenActionsGameObject;
	}
}
