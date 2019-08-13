using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class inputManager : MonoBehaviour
{
	public List<Axes> axes = new List<Axes> ();

	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public GameObject editInputPanelPrefab;
	public GameObject editInputMenu;
	public loadType loadOption;
	public bool useRelativePath;

	public GameObject touchPanel;

	public List<GameObject> buttonsDisabledAtStart = new List<GameObject> ();
	public List<gamepadInfo> gamepadList = new List<gamepadInfo> ();

	public List<playerInputManager> playerInputManagerList = new List<playerInputManager> ();

	public bool showActionKeysOnMenu = true;

	public bool touchPlatform;
	public bool gameCurrentlyPaused;

	public bool useTouchControls;

	menuPause pauseManager;
	gameManager mainGameManager;

	public bool ignoreGamepad;
	public bool allowKeyboardAndGamepad;
	public bool allowGamepadInTouchDevice;
	public float checkConnectedGamepadRate;
	float lastGamepadUpdateCheckTime;

	public List<multiAxesEditButtonInput> multiAxesEditButtonInputList = new List<multiAxesEditButtonInput> ();

	List<GameObject> editInputPanelList = new List<GameObject> ();

	public List<touchButtonListener> touchButtonList = new List<touchButtonListener> ();
	Scrollbar scroller;
	Touch currentTouch;

	public bool usingGamepad;
	public bool onlyOnePlayer;
	public bool usingKeyBoard;
	public int numberOfGamepads = -1;

	public bool showKeyboardPressed;
	public bool showKeyboardPressedAction;
	public bool showGamepadPressed;
	public bool showGamepadPressedAction;
	public bool showPressedKeyWhenEditingInput;

	bool editingInput;
	editButtonInput currentEditButtonInput;
	string currentEditButtonInputPreviouseValue;

	KeyCode[] validKeyCodes;

	string[] currentGamepadConnectedNameArray;
	List<string> currentGamepadConnectedNameList = new List<string> ();
	playerCharactersManager charactersManager;

	public int currentInputPanelListIndex = 0;
	public Text currentInputPanelListText;

	bool previouslyUsingGamepad;
	bool editInputMenuOpened;

	bool usingTouchControls;
	bool gameManagerPaused;

	public enum buttonType
	{
		//type of press of a key
		getKey,
		getKeyDown,
		getKeyUp,
		negMouseWheel,
		posMouseWheel,
	}

	//load the key input in the game from the current configuration in the inspector or load from a file
	public enum loadType
	{
		loadFile,
		loadEditorConfig,
	}

	float currentAxisValue;

	KeyCode currentKeyCodeToCheck;

	Dictionary<string, Dictionary<JoystickData.ButtonTypes, int>> register;
	bool registerInitialized;
	string[] joystickNames;
	string joystickName;
	Dictionary<JoystickData.ButtonTypes, int> buttonConfig;
	int keyIndexPressed;
	string retString;
	bool joystickNamesInitialized;

	bool currentJoystickButtonIsAxis;

	Axes currentJoystickButtonToCheck;

	int currentTouchCount;

	Axes currentTouchButtonToCheck;

	int currentTouchButtonIndexToCheck;

	int numberOfMultiAxes;
	List<int> multiAxesIndexList = new List<int> ();

	public string[] touchButtonListString;
	string currentJoystickButtonKeyString;
	string currentJoysticActionString;

	string saveFileFolderName = "Input";
	string saveFileName = "Input File";

	string defaultSaveFileName = "Default Input File";

	void Start ()
	{
		//get if the current platform is a touch device
		touchPlatform = touchJoystick.checkTouchPlatform ();

		getGameManagerSettings ();

		usingTouchControls = mainGameManager.isUsingTouchControls ();

		charactersManager = FindObjectOfType<playerCharactersManager> ();

		pauseManager = charactersManager.getPauseManagerFromPlayerByIndex (0);

		//if the current platform is a mobile, enable the touch controls in case they are not active
		if (touchPlatform && !usingTouchControls) {
			pauseManager.setUseTouchControlsState (true);
			mainGameManager.setUseTouchControlsState (true);
			pauseManager.reloadStart ();
			usingKeyBoard = true;
		}

		useRelativePath = mainGameManager.useRelativePath;

		//load the key input
		loadButtonsInput ();

		getValidKeyCodes ();

		if (ignoreGamepad) {
			currentGamepadConnectedNameArray = new String[0];
			setGamePadList ();
		}
	}

	void Update ()
	{
		usingTouchControls = mainGameManager.isUsingTouchControls ();

		gameManagerPaused = mainGameManager.isGamePaused ();

		if (!ignoreGamepad) {
			if (usingGamepad) {
				joystickNames = Input.GetJoystickNames ();
			}

			if ((Time.time > checkConnectedGamepadRate + lastGamepadUpdateCheckTime || gameManagerPaused)) {
				if (usingGamepad) {
					currentGamepadConnectedNameArray = joystickNames;
				} else {
					currentGamepadConnectedNameArray = Input.GetJoystickNames ();
				}

				lastGamepadUpdateCheckTime = Time.time;

				currentGamepadConnectedNameList.Clear ();
				for (int i = 0; i < currentGamepadConnectedNameArray.Length; ++i) {
					if (currentGamepadConnectedNameArray [i] != "") {
						currentGamepadConnectedNameList.Add (currentGamepadConnectedNameArray [i]);
					}
				}
				currentGamepadConnectedNameArray = new string[currentGamepadConnectedNameList.Count];
				for (int i = 0; i < currentGamepadConnectedNameList.Count; i++) {
					currentGamepadConnectedNameArray [i] = currentGamepadConnectedNameList [i];
				}
				
				if (numberOfGamepads != currentGamepadConnectedNameArray.Length) {
					setGamePadList ();
				}

				if (allowKeyboardAndGamepad) {
					usingKeyBoard = true;
				}

				if (usingGamepad) {
					if (!previouslyUsingGamepad) {
						print ("Gamepad added");
						previouslyUsingGamepad = true;
						if (editInputMenuOpened) {
							reloadInputMenu ();
						}
					}
				} else {
					if (previouslyUsingGamepad) {
						print ("Gamepad removed");
						previouslyUsingGamepad = false;
						if (editInputMenuOpened) {
							reloadInputMenu ();
						}
					}
				}
			}
		}

		if (usingGamepad && ignoreGamepad) {
			currentGamepadConnectedNameArray = new String[0];
			setGamePadList ();
		}

//		if (usingTouchControls) {
//			currentTouchCount = Input.touchCount;
//			if (!touchPlatform) {
//				currentTouchCount++;
//			}
//
//			for (int i = 0; i < currentTouchCount; i++) {
//				if (!touchPlatform) {
//					currentTouch = touchJoystick.convertMouseIntoFinger ();
//				} else {
//					currentTouch = Input.GetTouch (i);
//				}
//			}
//		}

		//if the player is changin this input field, search for any keyboard press
		if (editingInput) {
			if (usingGamepad) {
				bool checkPressedKey = false;
				string currentPressedKey = "";
				foreach (KeyCode vKey in validKeyCodes) {
					//set the value of the key pressed in the input field
					if (Input.GetKeyDown (vKey)) {
						currentPressedKey = vKey.ToString ();
						if (!currentPressedKey.Contains ("Alpha")) {
							checkPressedKey = true;
						}
					}
				}

				currentAxisValue = Input.GetAxis ("Left Trigger " + "1");
				if (currentAxisValue != 0) {
					currentPressedKey = "14";
					checkPressedKey = true;
				}

				currentAxisValue = Input.GetAxis ("Right Trigger " + "1");
				if (currentAxisValue != 0) {
					currentPressedKey = "15";
					checkPressedKey = true;
				}


				currentAxisValue = Input.GetAxis ("DPad X" + "1");
				if (currentAxisValue != 0) {
					if (currentAxisValue < 0) {
						currentPressedKey = "10";
					} else {
						currentPressedKey = "11";
					}
					checkPressedKey = true;
				}

				currentAxisValue = Input.GetAxis ("DPad Y" + "1");
				if (currentAxisValue != 0) {
					if (currentAxisValue < 0) {
						currentPressedKey = "12";
					} else {
						currentPressedKey = "13";
					}
					checkPressedKey = true;
				}

				if (checkPressedKey) {
					string keyName = "";
					int keyNumber = -1;

					string[] numbers = Regex.Split (currentPressedKey, @"\D+");
					foreach (string value in numbers) {
						int number = 0;
						if (int.TryParse (value, out number)) {
							print (value + " " + value.Length);
							keyNumber = number;
						}
					}

					if (keyNumber > -1) {
						keyName = Enum.GetName (typeof(joystickButtons), keyNumber);
						currentEditButtonInput.actionKeyText.text = keyName;

						if (showPressedKeyWhenEditingInput) {
							print (keyName);
						}
						//stop the checking of the keyboard
						editingInput = false;
					}
					return;
				}

			} else {
				foreach (KeyCode vKey in validKeyCodes) {
					//set the value of the key pressed in the input field
					if (Input.GetKeyDown (vKey)) {
						if (showPressedKeyWhenEditingInput) {
							print (vKey.ToString ());
						}

						currentEditButtonInput.actionKeyText.text = vKey.ToString ();

						//stop the checking of the keyboard
						editingInput = false;

						return;
					}
				}
			}
		}
	}

	public bool isUsingGamepad ()
	{
		return usingGamepad;
	}

	public bool isUsingKeyBoard ()
	{
		return usingKeyBoard;
	}

	public bool isUsingTouchControls ()
	{
		return usingTouchControls;
	}

	public bool isGameManagerPaused ()
	{
		return gameManagerPaused;
	}

	public bool isAllowKeyboardAndGamepad ()
	{
		return allowKeyboardAndGamepad;
	}

	public bool isAllowGamepadInTouchDevice ()
	{
		return allowGamepadInTouchDevice;
	}

	public void setEditInputMenuOpenedState (bool state)
	{
		editInputMenuOpened = state;
	}

	//Update current input panel to edit
	public void openInputMenu ()
	{
		if (!showActionKeysOnMenu) {
			return;
		}

		currentInputPanelListIndex = 0;
		updateCurrentInputPanel (currentInputPanelListIndex);

		setKeyNamesInActionsList ();
	}

	public void reloadInputMenu ()
	{
		updateCurrentInputPanel (currentInputPanelListIndex);

		setKeyNamesInActionsList ();
	}

	public void setKeyNamesInActionsList ()
	{
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
			for (int j = 0; j < multiAxesEditButtonInputList [i].buttonsList.Count; j++) {
				if (usingGamepad) {
					multiAxesEditButtonInputList [i].buttonsList [j].actionKeyText.text = multiAxesEditButtonInputList [i].buttonsList [j].gamepadActionKey;
				} else {
					multiAxesEditButtonInputList [i].buttonsList [j].actionKeyText.text = multiAxesEditButtonInputList [i].buttonsList [j].keyboardActionKey;
				}
			}
		}
	}

	public void updateCurrentInputPanel (int index)
	{
		for (int i = 0; i < editInputPanelList.Count; i++) {
			if (i == index) {
				editInputPanelList [i].SetActive (true);
			} else {
				editInputPanelList [i].SetActive (false);
			}
		}
		currentInputPanelListText.text = editInputPanelList [index].name;

		//get the scroller in the edit input menu
		scroller = editInputMenu.GetComponentInChildren<Scrollbar> ();
		//set the scroller in the top position
		scroller.value = 1;
	}

	public void setNextInputPanel ()
	{
		currentInputPanelListIndex++;
		if (currentInputPanelListIndex > editInputPanelList.Count - 1) {
			currentInputPanelListIndex = 0;
		}
		updateCurrentInputPanel (currentInputPanelListIndex);
	}

	public void setPreviousInputPanel ()
	{
		currentInputPanelListIndex--;
		if (currentInputPanelListIndex < 0) {
			currentInputPanelListIndex = editInputPanelList.Count - 1;
		}
		updateCurrentInputPanel (currentInputPanelListIndex);
	}

	void getCurrentAxesListFromInspector ()
	{
		editInputMenu.SetActive (true);
		editInputPanelPrefab.SetActive (false);

		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].currentlyActive) {
				GameObject editInputPanelPrefabClone = (GameObject)Instantiate (editInputPanelPrefab, editInputPanelPrefab.transform.position, Quaternion.identity);
				editInputPanelPrefabClone.transform.SetParent (editInputPanelPrefab.transform.parent);
				editInputPanelPrefabClone.SetActive (true);
				editInputPanelPrefabClone.name = multiAxesList [i].axesName;

				editInputPanelInfo currentEditInputPanelInfo = editInputPanelPrefabClone.GetComponent<editInputPanelInfo> ();

				editInputPanelList.Add (editInputPanelPrefabClone);

				List<Axes> currentAxesList = multiAxesList [i].axes;

				List<editButtonInput> buttonsList = new List<editButtonInput> ();

				GameObject bottom = currentEditInputPanelInfo.bottomGameObject;
				//get all the keys field inside the edit input menu
				for (int j = 0; j < currentAxesList.Count; j++) {
					currentAxesList [j].keyButton = currentAxesList [j].key.ToString ();

					if (currentAxesList [j].actionEnabled) {
						//every key field in the edit input button has a editButtonInput component, so create every of them
		
						GameObject buttonClone = (GameObject)Instantiate (currentEditInputPanelInfo.editButtonInputGameObject, 
							                         currentEditInputPanelInfo.editButtonInputGameObject.transform.position, Quaternion.identity);
						buttonClone.SetActive (true);
						buttonClone.name = currentAxesList [j].Name;
						buttonClone.transform.SetParent (currentEditInputPanelInfo.buttonsParent);
						buttonClone.name = currentAxesList [j].Name;
						editButtonInput currentEditButtonInput = buttonClone.GetComponent<editButtonInput> ();

						currentEditButtonInput.actionNameText.text = currentAxesList [j].Name;
						currentEditButtonInput.actionKeyText.text = currentAxesList [j].keyButton;

						currentEditButtonInput.keyboardActionKey = currentAxesList [j].keyButton;

						if (currentAxesList [j].joystickButton != joystickButtons.None) {
							currentEditButtonInput.gamepadActionKey = currentAxesList [j].joystickButton.ToString ();
						}

						currentEditButtonInput.multiAxesIndex = i;
						currentEditButtonInput.axesIndex = j;

						buttonClone.transform.localScale = Vector3.one;
						buttonsList.Add (currentEditButtonInput);
	
						currentEditInputPanelInfo.editButtonInputGameObject.SetActive (false);
					}
				}
				multiAxesEditButtonInput newMultiAxesEditButtonInput = new multiAxesEditButtonInput ();
				newMultiAxesEditButtonInput.buttonsList = buttonsList;

				multiAxesEditButtonInputList.Add (newMultiAxesEditButtonInput);

				//set the empty element of the list in the bottom of the list
				bottom.transform.SetAsLastSibling ();
				bottom.SetActive (false);
				editInputPanelPrefabClone.SetActive (false);
			}
		}
		editInputMenu.SetActive (false);
	}

	public void saveButtonsInput ()
	{
		//for every key field in the edit input menu, save its value and change them in the inputManager inspector aswell
		if (Application.isPlaying) {
			for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
				for (int j = 0; j < multiAxesEditButtonInputList [i].buttonsList.Count; j++) {
					string currentKeyAction = multiAxesEditButtonInputList [i].buttonsList [j].actionKeyText.text.ToString ();
					if (usingGamepad) {
						if (currentKeyAction != "") {
							multiAxesEditButtonInputList [i].buttonsList [j].gamepadActionKey = currentKeyAction;

							changeGamepadKeyValue (currentKeyAction,
								multiAxesEditButtonInputList [i].buttonsList [j].multiAxesIndex, multiAxesEditButtonInputList [i].buttonsList [j].axesIndex);
						}
					} else {
						multiAxesEditButtonInputList [i].buttonsList [j].keyboardActionKey = currentKeyAction;

						changeKeyValue (currentKeyAction, 
							multiAxesEditButtonInputList [i].buttonsList [j].multiAxesIndex, multiAxesEditButtonInputList [i].buttonsList [j].axesIndex);
					}
				}
			}
		}

		//create a list of axes to store it
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> temporalAxesList = new List<Axes> ();
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes axe = new Axes ();
				axe.Name = currentAxesList [j].Name;
				axe.actionEnabled = currentAxesList [j].actionEnabled;

				axe.key = currentAxesList [j].key;
				axe.keyButton = currentAxesList [j].key.ToString ();

				axe.joystickButton = currentAxesList [j].joystickButton;

				axe.touchButtonName = currentAxesList [j].touchButtonName;
				axe.touchButtonIndex = currentAxesList [j].touchButtonIndex;
			
				temporalAxesList.Add (axe);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = multiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;
			temporalMultiAxesList.Add (newMultiAxes);
		}

		//save the input list
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (getDataPath (saveFileName)); 
		bf.Serialize (file, temporalMultiAxesList);
		file.Close ();

		print ("Input Saved in " + saveFileName);
	}

	public string getDataPath (string fileNameToUse)
	{
		string dataPath = "";
		if (useRelativePath) {
			dataPath = saveFileFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + saveFileFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/" + fileNameToUse;

		return dataPath;
	}

	public void loadButtonsInput ()
	{
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		//if the configuration is loaded from a file, get a new axes list with the stored values
		if (loadOption == loadType.loadFile) {
			//if the file of buttons exists, get that list
			if (!touchPlatform && File.Exists (getDataPath (saveFileName))) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (getDataPath (saveFileName), FileMode.Open);
				temporalMultiAxesList = (List<multiAxes>)bf.Deserialize (file);
				file.Close ();

				multiAxesList.Clear ();

				for (int i = 0; i < temporalMultiAxesList.Count; i++) {
					List<Axes> temporalAxesList = new List<Axes> ();
					for (int j = 0; j < temporalMultiAxesList [i].axes.Count; j++) {
						temporalAxesList.Add (temporalMultiAxesList [i].axes [j]);
					}

					multiAxes newMultiAxes = new multiAxes ();
					newMultiAxes.axesName = temporalMultiAxesList [i].axesName;
					newMultiAxes.axes = temporalAxesList;
					multiAxesList.Add (newMultiAxes);
				}
			}

			//else, get the list created in the inspector
			else {
				for (int i = 0; i < multiAxesList.Count; i++) {
					List<Axes> temporalAxesList = new List<Axes> ();

					List<Axes> currentAxesList = multiAxesList [i].axes;

					for (int j = 0; j < currentAxesList.Count; j++) {
						temporalAxesList.Add (currentAxesList [j]);
					}

					multiAxes newMultiAxes = new multiAxes ();
					newMultiAxes.axesName = multiAxesList [i].axesName;
					newMultiAxes.axes = temporalAxesList;

					temporalMultiAxesList.Add (newMultiAxes);
				}
				
				saveButtonsInputFromInspector (saveFileName);
			}
		} 

		//else the new axes list is the axes in the input manager inspector
		else {
			for (int i = 0; i < multiAxesList.Count; i++) {
				List<Axes> currentAxesList = multiAxesList [i].axes;

				multiAxes newMultiAxes = new multiAxes ();
				newMultiAxes.axesName = multiAxesList [i].axesName;
				newMultiAxes.axes = currentAxesList;

				temporalMultiAxesList.Add (newMultiAxes);
			}
		}

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> currentAxesList = multiAxesList [i].axes;
			for (int j = 0; j < currentAxesList.Count; j++) {
				if (currentAxesList [j].touchButtonIndex > 0) {
					if (isInTouchButtonToDisableList (touchButtonList [currentAxesList [j].touchButtonIndex - 1].gameObject)) {
						touchButtonList [currentAxesList [j].touchButtonIndex - 1].gameObject.SetActive (false);
					}
				}
			}
		}

		if (showActionKeysOnMenu) {
			//get the current list of axes defined in the inspector
			getCurrentAxesListFromInspector ();
		}

		//set in every key field in the edit input menu with the stored key input for every field
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
			for (int j = 0; j < multiAxesEditButtonInputList [i].buttonsList.Count; j++) {
				if (i <= temporalMultiAxesList.Count - 1 && j < temporalMultiAxesList [i].axes.Count - 1) {
					multiAxesEditButtonInputList [i].buttonsList [j].actionKeyText.text = temporalMultiAxesList [i].axes [j].keyButton;
				}
			}
		}

		numberOfMultiAxes = multiAxesList.Count;

		for (int i = 0; i < multiAxesList.Count; i++) {
			multiAxesIndexList.Add (multiAxesList [i].axes.Count);
		}
	}

	//save the input list in the inspector to a file
	public void saveButtonsInputFromInspector (string fileNameToUse)
	{
		//create a list of axes to store it
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> temporalAxesList = new List<Axes> ();
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes axe = new Axes ();
				axe.Name = currentAxesList [j].Name;
				axe.actionEnabled = currentAxesList [j].actionEnabled;

				axe.key = currentAxesList [j].key;
				axe.keyButton = currentAxesList [j].key.ToString ();

				axe.joystickButton = currentAxesList [j].joystickButton;

				axe.touchButtonName = currentAxesList [j].touchButtonName;
				axe.touchButtonIndex = currentAxesList [j].touchButtonIndex;

				temporalAxesList.Add (axe);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = multiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;

			temporalMultiAxesList.Add (newMultiAxes);
		}

		//save the input list
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (getDataPath (fileNameToUse)); 
		bf.Serialize (file, temporalMultiAxesList);
		file.Close ();

		print ("Current Input Saved in " + fileNameToUse);
	}

	//load the input list from the file to the inspector
	public void loadButtonsInputFromInspector (string fileNameToUse)
	{
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		if (File.Exists (getDataPath (fileNameToUse))) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (getDataPath (fileNameToUse), FileMode.Open);
			temporalMultiAxesList = (List<multiAxes>)bf.Deserialize (file);
			file.Close ();
		} else {
			print ("File located in " + fileNameToUse + " doesn't exist, make sure the file is created previously to load it in the editor");

			return;
		}

		//print (axesList.Count);
		multiAxesList.Clear ();

		//set the touch button for every axes, if it had it
		for (int i = 0; i < temporalMultiAxesList.Count; i++) {
			
			List<Axes> temporalAxesList = new List<Axes> ();

			for (int j = 0; j < temporalMultiAxesList [i].axes.Count; j++) {
				temporalAxesList.Add (temporalMultiAxesList [i].axes [j]);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = temporalMultiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;

			multiAxesList.Add (newMultiAxes);
		}

		print ("Input File Loaded from " + fileNameToUse);
	}

	public void getGameManagerSettings ()
	{
		if (!mainGameManager) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}
			
		saveFileFolderName = mainGameManager.saveInputFileFolderName;

		saveFileName = mainGameManager.saveInputFileName + " " + mainGameManager.versionNumber + mainGameManager.fileExtension;

		defaultSaveFileName = mainGameManager.defaultInputSaveFileName + " " + mainGameManager.versionNumber + mainGameManager.fileExtension;

		useRelativePath = mainGameManager.useRelativePath;

		mainGameManager.setCurrentPersistentDataPath ();

		updateComponent ();
	}

	public void setCurrentInputToDefault ()
	{
		getGameManagerSettings ();

		loadButtonsInputFromInspector (defaultSaveFileName);

		updateComponent ();
	}

	public void loadButtonsInspectorFromSaveFile ()
	{
		getGameManagerSettings ();

		loadButtonsInputFromInspector (saveFileName);

		updateComponent ();
	}

	public void saveCurrentInputAsDefault ()
	{
		getGameManagerSettings ();

		saveButtonsInputFromInspector (defaultSaveFileName);

		updateComponent ();
	}

	public void saveButtonsInputToSaveFile ()
	{
		getGameManagerSettings ();

		saveButtonsInputFromInspector (saveFileName);

		updateComponent ();
	}

	public void changeKeyValue (string keyButton, int multiAxesIndex, int axesIndex)
	{
		Axes currentAxes = multiAxesList [multiAxesIndex].axes [axesIndex];

		print (currentAxes.Name + " " + keyButton);

		KeyCode newKeyCode = (KeyCode)System.Enum.Parse (typeof(KeyCode), keyButton);

		currentAxes.key = newKeyCode;
		currentAxes.keyButton = currentAxes.key.ToString ();
	}

	public void changeGamepadKeyValue (string keyButton, int multiAxesIndex, int axesIndex)
	{
		Axes currentAxes = multiAxesList [multiAxesIndex].axes [axesIndex];

		print (currentAxes.Name + " " + keyButton);

		currentAxes.joystickButton = (joystickButtons)Enum.Parse (typeof(joystickButtons), keyButton);
	}
		
	//get the key button value for an input field, using the action of the button
	public string getButtonKey (string actionName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {

				if (currentAxesList [j].Name == actionName) {
					if (usingGamepad && currentAxesList [j].joystickButton != joystickButtons.None) {
						return currentAxesList [j].joystickButton.ToString ();
					} else {
						return currentAxesList [j].keyButton;
					}
				}
			}
		}

		return "";
	}

	//if the input field has been pressed, call a coroutine, to avoid the input field get the mouse press as new value
	public void startEditingInput (GameObject button)
	{
		if (!editingInput) {
			StartCoroutine (startEditingInputCoroutine (button));
		}
	}

	//set the text of the input field to ... and start to check the keyboard press
	IEnumerator startEditingInputCoroutine (GameObject button)
	{
		yield return null;
		currentEditButtonInput = button.GetComponent<editButtonInput> ();
		currentEditButtonInputPreviouseValue = currentEditButtonInput.actionKeyText.text;
		currentEditButtonInput.actionKeyText.text = "...";
		editingInput = true;
	}

	//any change done in the input field is undone
	public void cancelEditingInput ()
	{
		editingInput = false;
		if (currentEditButtonInput) {
			print (currentEditButtonInputPreviouseValue);
			currentEditButtonInput.actionKeyText.text = currentEditButtonInputPreviouseValue;
			currentEditButtonInput = null;
		}
	}

	public void getValidKeyCodes ()
	{
		validKeyCodes = (KeyCode[])System.Enum.GetValues (typeof(KeyCode));
	}

	public string getKeyPressed (buttonType type, bool canBeUsedOnGamePaused)
	{
		if ((!gameManagerPaused || canBeUsedOnGamePaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {
			foreach (KeyCode vKey in validKeyCodes) {
				switch (type) {
				//this key is for holding
				case buttonType.getKey:
					if (Input.GetKey (vKey)) {
						return vKey.ToString ();
					}
					break;

				case buttonType.getKeyDown:
					//this key is for press once
					if (Input.GetKeyDown (vKey)) {
						return vKey.ToString ();
					}
					break;

				case buttonType.getKeyUp:
					//this key is for release 
					if (Input.GetKeyUp (vKey)) {
						return vKey.ToString ();
					}
					break;
				}
			}
		}

		return "";
	}

	public bool checkJoystickButton (int controllerNumber, int buttonIndex, buttonType type)
	{
		if (!usingTouchControls || allowGamepadInTouchDevice) {
			if ((!usingKeyBoard || allowKeyboardAndGamepad)) {
				if (usingGamepad) {
					string currentKey = "joystick " + controllerNumber.ToString () + " button " + buttonIndex.ToString ();

					switch (type) {
					//this key is for holding
					case buttonType.getKeyDown:
						if (Input.GetKeyDown (currentKey)) {
							if (showGamepadPressedAction) {
								print (controllerNumber + " Get Key Down: " + currentKey);
							}
							return true;
						}
						break;

					case buttonType.getKeyUp:
						if (Input.GetKeyUp (currentKey)) {
							if (showGamepadPressedAction) {
								print (controllerNumber + " Get Key Up: " + currentKey);
							}
							return true;
						}
						break;
					}
				}
			}
		}

		return false;
	}

	public bool checkPlayerInputButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, int controllerNumber, bool canBeUsedOnPausedGame, bool useOnlyKeyboard)
	{
		if (usingTouchControls) {
			if (getTouchButtonFromMultiAxesList (multiAxesIndex, axesIndex, type)) {
				return true;
			}
		} 

		if (!usingTouchControls || allowGamepadInTouchDevice) {
			if ((!usingKeyBoard || allowKeyboardAndGamepad) && !useOnlyKeyboard) {
				if (usingGamepad) {
					if (getJoystickButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, controllerNumber, canBeUsedOnPausedGame)) {
						return true;
					}
				}
			}

			if (usingKeyBoard || allowKeyboardAndGamepad || useOnlyKeyboard) {
				if (getKeyboardButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, canBeUsedOnPausedGame)) {
					return true;
				}
			}

		}

		return false;
	}

	//function called in the script where pressing that button will make an action in the game, for example jump, crouch, shoot, etc...
	//every button sends its action and the type of pressing
	public bool getKeyboardButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, bool canBeUsedOnPausedGame)
	{
		//if the game is not paused, and the current control is the keyboard
		if ((canBeUsedOnPausedGame || !gameManagerPaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {

			if (multiAxesIndex >= numberOfMultiAxes) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed");

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
				" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
				"settings in the main customm Input Manager of GKC");

				return false;
			}

			currentKeyCodeToCheck = multiAxesList [multiAxesIndex].axes [axesIndex].key;

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (Input.GetKey (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					//check that the key pressed has being defined as an action
					if (multiAxesList [multiAxesIndex].axes [axesIndex].actionEnabled) {
						return true;
					}
				}
				break;

			//this key is for press once
			case buttonType.getKeyDown:
				if (Input.GetKeyDown (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					//check that the key pressed has being defined as an action
					if (multiAxesList [multiAxesIndex].axes [axesIndex].actionEnabled) {
						return true;
					}
				}
				break;

			//this key is for release
			case buttonType.getKeyUp:
				if (Input.GetKeyUp (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					//check that the key pressed has being defined as an action
					if (multiAxesList [multiAxesIndex].axes [axesIndex].actionEnabled) {
						return true;
					}
				}
				break;
			//mouse wheel
			case buttonType.negMouseWheel:
				//check if the wheel of the mouse has been used, and in what direction
				if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					return true;
				}
				break;
			case buttonType.posMouseWheel:
				//check if the wheel of the mouse has been used, and in what direction
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					return true;
				}
				break;
			}
		}

		return false;
	}

	public bool getTouchButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type)
	{
		//if the game is not paused, and the current control is a touch device
		if (!gameManagerPaused && usingTouchControls) {

			if (multiAxesIndex >= numberOfMultiAxes) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed");

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
				" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
				"settings in the main customm Input Manager of GKC");
				
				return false;
			}

			currentTouchCount = Input.touchCount;
			if (!touchPlatform) {
				currentTouchCount++;
			}

			for (int i = 0; i < currentTouchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}
			
				//check for a began touch
				if (type == buttonType.getKeyDown) {
					
					if (currentTouch.phase == TouchPhase.Began) {
					
						currentTouchButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

						if (currentTouchButtonToCheck.touchButtonIndex > 0 && currentTouchButtonToCheck.actionEnabled) {
							currentTouchButtonIndexToCheck = currentTouchButtonToCheck.touchButtonIndex - 1;

							if (touchButtonList.Count > currentTouchButtonIndexToCheck && touchButtonList [currentTouchButtonIndexToCheck] != null) {
								if (touchButtonList [currentTouchButtonIndexToCheck].pressedDown) {
									//if the button is pressed (OnPointerDown), return true
									//print ("getKeyDown");
									return true;
								}
							}
						}
					}
				}

				//check for a hold touch
				if (type == buttonType.getKey) {
					
					if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) {
					
						currentTouchButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

						if (currentTouchButtonToCheck.touchButtonIndex > 0 && currentTouchButtonToCheck.actionEnabled) {
							currentTouchButtonIndexToCheck = currentTouchButtonToCheck.touchButtonIndex - 1;

							if (touchButtonList.Count > currentTouchButtonIndexToCheck && touchButtonList [currentTouchButtonIndexToCheck] != null) {
								if (touchButtonList [currentTouchButtonIndexToCheck].pressed) {
									//if the button is pressed OnPointerDown, and is not released yet (OnPointerUp), return true
									//print ("getKey");
									return true;					
								}
							}
						}
					}
				}

				//check for a release touch
				if (type == buttonType.getKeyUp) {
			
					if (currentTouch.phase == TouchPhase.Ended) {
			
						currentTouchButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

						if (currentTouchButtonToCheck.touchButtonIndex > 0 && currentTouchButtonToCheck.actionEnabled) {
							currentTouchButtonIndexToCheck = currentTouchButtonToCheck.touchButtonIndex - 1;

							if (touchButtonList.Count > currentTouchButtonIndexToCheck && touchButtonList [currentTouchButtonIndexToCheck] != null) {
								if (touchButtonList [currentTouchButtonIndexToCheck].pressedUp) {
									//if the button is released (OnPointerUp), return true
									//print ("getKeyUp");
									return true;
								}
							}
						}
					}
				}
			}
		}

		return false;
	}

	public bool getJoystickButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, int controllerNumber, bool canBeUsedOnPausedGame)
	{
		//if the game is not paused, and the current control is the keyboard
		if ((canBeUsedOnPausedGame || !gameManagerPaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {

			if (multiAxesIndex >= numberOfMultiAxes) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed");

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
				" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
				"settings in the main customm Input Manager of GKC");

				return false;
			}

			currentJoystickButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

			keyIndexPressed = (int)currentJoystickButtonToCheck.joystickButton;

			if (keyIndexPressed == -1) {
				return false;
			}

			currentJoystickButtonKeyString = "";

//			if (currentJoystickButtonToCheck.joystickButtonKeyAssigned) {
//				currentJoystickButtonKeyString = currentJoystickButtonToCheck.joystickButtonKeyString;
//			} else {
			currentJoystickButtonKeyString = getKeyString (controllerNumber, keyIndexPressed);
			currentJoystickButtonToCheck.joystickButtonKeyString = currentJoystickButtonKeyString;
			currentJoystickButtonToCheck.joystickButtonKeyAssigned = true;
//			}

			if (currentJoystickButtonKeyString == "") {
				return false;
			}

			currentJoystickButtonIsAxis = (keyIndexPressed >= 10 && keyIndexPressed <= 15);

			currentJoysticActionString = currentJoystickButtonToCheck.Name;

			if (currentJoystickButtonIsAxis) {
				return getAxisValue (controllerNumber, keyIndexPressed, multiAxesIndex, axesIndex, type);
			}

			//print (controllerNumber + " " +currentJoystickButtonKeyString);
				
			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (Input.GetKey (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key: " + currentJoysticActionString);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				//this key is for press once
				if (Input.GetKeyDown (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key Down: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key Down: " + currentJoysticActionString);
					}
					return true;
				}
				break;

			case buttonType.getKeyUp:
				//this key is for release 
				if (Input.GetKeyUp (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key Up: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key Up: " + currentJoysticActionString);
					}
					return true;
				}
				break;
			}
		}

		return false;
	}

	public bool getAxisValue (int numberPlayer, int keyPressed, int multiAxesIndex, int axesIndex, buttonType type)
	{
		if (keyPressed == 14) {
			currentAxisValue = Input.GetAxis ("Left Trigger " + numberPlayer.ToString ());

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadLeftTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadLeftTriggersInfo (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadLeftTriggersInfoDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}
						return true;
					}
				} else {
					if (getGamePadLeftTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadLeftTriggersInfoDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadLeftTriggersInfo (numberPlayer, multiAxesIndex, axesIndex)) {
						
						setGamepadLeftTriggersInfo (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}
						return true;
					}
				}
				break;
			}
		}

		if (keyPressed == 15) {
			currentAxisValue = Input.GetAxis ("Right Trigger " + numberPlayer.ToString ());

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadRightTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfo (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadRightTriggersInfoDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}
						return true;
					}
				} else {
					if (getGamePadRightTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfoDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadRightTriggersInfo (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfo (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}
						return true;
					}
				}
				break;
			}
		}

		if (keyPressed == 10) {
			currentAxisValue = Input.GetAxis ("DPad X" + numberPlayer.ToString ());

			if (currentAxisValue != 0) {
				if (currentAxisValue < 0 && !getGamePadDPadXInfo (numberPlayer, multiAxesIndex, axesIndex)) {
					setGamepadDPadXInfo (numberPlayer, true, multiAxesIndex, axesIndex);

					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
			} else {
				setGamepadDPadXInfo (numberPlayer, false, multiAxesIndex, axesIndex);
			}
		}

		if (keyPressed == 11) {
			currentAxisValue = Input.GetAxis ("DPad X" + numberPlayer.ToString ());

			if (currentAxisValue != 0) {
				if (currentAxisValue > 0 && !getGamePadDPadXInfo (numberPlayer, multiAxesIndex, axesIndex)) {
					setGamepadDPadXInfo (numberPlayer, true, multiAxesIndex, axesIndex);

					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
			} else {
				setGamepadDPadXInfo (numberPlayer, false, multiAxesIndex, axesIndex);
			}
		}

		if (keyPressed == 12) {
			currentAxisValue = Input.GetAxis ("DPad Y" + numberPlayer.ToString ());

			if (currentAxisValue != 0) {
				if (currentAxisValue < 0 && !getGamePadDPadYInfo (numberPlayer, multiAxesIndex, axesIndex)) {
					setGamepadDPadYInfo (numberPlayer, true, multiAxesIndex, axesIndex);

					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
			} else {
				setGamepadDPadYInfo (numberPlayer, false, multiAxesIndex, axesIndex);
			}
		}

		if (keyPressed == 13) {
			currentAxisValue = Input.GetAxis ("DPad Y" + numberPlayer.ToString ());

			if (currentAxisValue != 0) {
				if (currentAxisValue > 0 && !getGamePadDPadYInfo (numberPlayer, multiAxesIndex, axesIndex)) {
					setGamepadDPadYInfo (numberPlayer, true, multiAxesIndex, axesIndex);

					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}
					return true;
				}
			} else {
				setGamepadDPadYInfo (numberPlayer, false, multiAxesIndex, axesIndex);
			}
		}

		return false;
	}

	//set and get trigger state
	public void setGamepadLeftTriggersInfo (int numberPlayer, bool left, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingLeftTrigger = left;
	}

	public void setGamepadRightTriggersInfo (int numberPlayer, bool right, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingRightTrigger = right;
	}

	public void setGamepadLeftTriggersInfoDown (int numberPlayer, bool left, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].leftTriggerDown = left;
	}

	public void setGamepadRightTriggersInfoDown (int numberPlayer, bool right, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].rightTriggerDown = right;
	}

	public bool getGamePadLeftTriggersInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingLeftTrigger;
	}

	public bool getGamePadRightTriggersInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingRightTrigger;
	}

	public bool getGamePadLeftTriggersInfoDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].leftTriggerDown;
	}

	public bool getGamePadRightTriggersInfoDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].rightTriggerDown;
	}

	//set and get dpad state
	public bool getGamePadDPadXInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadX;
	}

	public void setGamepadDPadXInfo (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadX = DPad;
	}

	public bool getGamePadDPadYInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadY;
	}

	public void setGamepadDPadYInfo (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadY = DPad;
	}

	public void setRegister ()
	{
		// First use the platform to determine which register to search
		switch (Application.platform) {
		case RuntimePlatform.WindowsPlayer | RuntimePlatform.WindowsEditor:
			register = JoystickData.register_windows;
			break;

		case RuntimePlatform.OSXPlayer | RuntimePlatform.OSXEditor:
			register = JoystickData.register_osx;
			break;

		case RuntimePlatform.LinuxPlayer:
			register = JoystickData.register_linux;
			break;
		}

		register = JoystickData.register_default;
	}

	string getKeyString (int joystickNumber, int keyIndex)
	{
		if (!registerInitialized) {
			setRegister ();
			registerInitialized = true;
		}

		joystickName = "default";

		if (!joystickNamesInitialized) {
			joystickNames = Input.GetJoystickNames ();
			joystickNamesInitialized = true;
		}

		if (joystickNumber > joystickNames.Length) {
			return "";
		}

		joystickName = joystickNames [joystickNumber - 1];
	
		if (!register.ContainsKey (joystickName)) {
			joystickName = "default";
		
			// While we are here, make sure there the requested button is in the default joystick config of the default register
			if (!register [joystickName].ContainsKey ((JoystickData.ButtonTypes)keyIndex)) {
				// The requested button doesn't exist on the default joystick configuration! This is bad!
				return "";
			}
		}

		retString = "joystick " + joystickNumber + " button ";

		buttonConfig = register [joystickName];

		retString += buttonConfig [(JoystickData.ButtonTypes)keyIndex];

		return retString;
	}

	//change the current controls to keyboard or mobile
	public void setKeyboardControls (bool state)
	{
		if (!mainGameManager) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}

		if (!pauseManager) {
			charactersManager = FindObjectOfType<playerCharactersManager> ();

			pauseManager = charactersManager.getPauseManagerFromPlayerByIndex (0);
		}

		mainGameManager.setUseTouchControlsState (!state);
		pauseManager.setUseTouchControlsState (!state);

		useTouchControls = !state;

		for (int i = 0; i < playerInputManagerList.Count; i++) {
			playerInputManagerList [i].changeControlsType (!state);
		}

		updateComponent ();
	}

	public void setUseTouchControlsState (bool state)
	{
		useTouchControls = state;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR

		if (!mainGameManager) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}

		if (!pauseManager) {
			charactersManager = FindObjectOfType<playerCharactersManager> ();

			pauseManager = charactersManager.getPauseManagerFromPlayerByIndex (0);
		}

		EditorUtility.SetDirty (pauseManager);
		EditorUtility.SetDirty (mainGameManager);
		EditorUtility.SetDirty (this);
		#endif
	}

	public bool isInTouchButtonToDisableList (GameObject touchButtonGameObject)
	{
		if (buttonsDisabledAtStart.Contains (touchButtonGameObject)) {
			return true;
		}

		return false;
	}

	public void setGamePadList ()
	{
		gamepadList.Clear ();
		for (int i = 0; i < currentGamepadConnectedNameArray.Length; i++) {
			gamepadInfo newGamepad = new gamepadInfo ();
			newGamepad.gamepadNumber = (i + 1);
			newGamepad.gamepadName = currentGamepadConnectedNameArray [i];

			for (int j = 0; j < multiAxesList.Count; j++) {

				multiGamepadAxisInfo newMultiGamepadAxisInfo = new multiGamepadAxisInfo ();

				newMultiGamepadAxisInfo.Name = multiAxesList [j].axesName;

				for (int k = 0; k < multiAxesList [j].axes.Count; k++) {
					gamepadAxisInfo newGamepadAxisInfo = new gamepadAxisInfo ();

					newGamepadAxisInfo.Name = multiAxesList [j].axes [k].Name;
					newMultiGamepadAxisInfo.gamepadAxisInfoList.Add (newGamepadAxisInfo);
				}
				newGamepad.multiGamepadAxisInfoList.Add (newMultiGamepadAxisInfo);
			}


			gamepadList.Add (newGamepad);
			Debug.Log (currentGamepadConnectedNameArray [i]);
		}

		numberOfGamepads = gamepadList.Count;

		if (numberOfGamepads == 0) {
			usingKeyBoard = true;
			usingGamepad = false;
			//print ("keyboard");
		}
		if (numberOfGamepads >= 1) {
			usingKeyBoard = false;
			usingGamepad = true;
			//print ("gamepad");
		}
		if (numberOfGamepads <= 1) {
			onlyOnePlayer = true;
			//print ("one player");
		}	
	}

	//set the current pause state of the game
	public void setPauseState (bool state)
	{
		gameCurrentlyPaused = state;
	}

	//add a new axe to the list
	public void addNewAxe (int axeListIndex)
	{
		Axes newAxe = new Axes ();
		newAxe.Name = "New Action";
		newAxe.joystickButton = joystickButtons.None;
		multiAxesList [axeListIndex].axes.Add (newAxe);

		updateComponent ();
	}

	public void addNewAxesList ()
	{
		multiAxes newMultiAxes = new multiAxes ();
		newMultiAxes.axesName = "New Axes List";
		multiAxesList.Add (newMultiAxes);

		updateComponent ();
	}

	public int getAxesListIndexByName (string axesName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName == axesName) {
				return i;
			}
		}

		return -1;
	}

	public void removeAxesElement (int multiAxesIndex, int axesIndex)
	{
		if (multiAxesList.Count > multiAxesIndex && multiAxesList [multiAxesIndex].axes.Count > axesIndex) {
			multiAxesList [multiAxesIndex].axes.RemoveAt (axesIndex);

			print ("IMPORTANT: Axes elements in the group of actions called " + multiAxesList [multiAxesIndex].axesName + " has changed." +
			"\n Make sure to update the axes list in the Player Input Manager inspector of the main player");

			updateComponent ();
		}
	}

	public void enableTouchButtonByName (string touchButtonName)
	{
		enableOrDisableTouchButtonByName (true, touchButtonName);
	}

	public void disableTouchButtonByName (string touchButtonName)
	{
		enableOrDisableTouchButtonByName (false, touchButtonName);
	}

	public void enableOrDisableTouchButtonByName (bool state, string touchButtonName)
	{
		for (int i = 0; i < touchButtonList.Count; i++) {
			if (touchButtonList [i].gameObject.name == touchButtonName) {
				touchButtonList [i].gameObject.SetActive (state);
			}
		}
	}

	public void getTouchButtonList ()
	{
		touchButtonList.Clear ();

		touchPanel.SetActive (true);

		Component[] components = touchPanel.GetComponentsInChildren (typeof(touchButtonListener));
		foreach (Component c in components) {
			touchButtonList.Add (c.GetComponent<touchButtonListener> ());
		}

		if (!usingTouchControls) {
			touchPanel.SetActive (false);
		}

		updateComponent ();
	}

	public void getTouchButtonListString ()
	{
		getTouchButtonList ();

		touchButtonListString = new string[touchButtonList.Count + 1];
		touchButtonListString [0] = "None";
		for (int i = 0; i < touchButtonList.Count; i++) {
			string newName = touchButtonList [i].gameObject.name;
			touchButtonListString [i + 1] = newName;
		}
			
		updateComponent ();
	}

	[System.Serializable]
	public class Axes
	{
		public string Name;

		public bool actionEnabled = true;

		public string keyButton;

		public joystickButtons joystickButton;

		public int touchButtonIndex = 0;
		public string touchButtonName;

		public KeyCode key = KeyCode.A;

		public string joystickButtonKeyString;
		public bool joystickButtonKeyAssigned;

		//some constructors for a key input, incluing name, key button and touch button
		public Axes ()
		{
			Name = "";
			keyButton = "";
			actionEnabled = true;
		}

		public Axes (string n, string key)
		{
			Name = n;
			keyButton = key;
			actionEnabled = true;
		}
	}

	public enum joystickButtons
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		LeftBumper = 4,
		RightBumper = 5,
		Back = 6,
		Start = 7,
		LeftStickClick = 8,
		RightStickClick = 9,
		LeftDPadX = 10,
		RightDPadX = 11,
		TopDPadY = 12,
		BottomDPadY = 13,
		LeftTrigger = 14,
		RightTrigger = 15,
		None = -1
	}

	[System.Serializable]
	public class gamepadInfo
	{
		public int gamepadNumber;
		public string gamepadName;

		public List<multiGamepadAxisInfo> multiGamepadAxisInfoList = new List<multiGamepadAxisInfo> ();
	}

	[System.Serializable]
	public class multiGamepadAxisInfo
	{
		public string Name;
		public List<gamepadAxisInfo> gamepadAxisInfoList = new List<gamepadAxisInfo> ();
	}

	[System.Serializable]
	public class gamepadAxisInfo
	{
		public string Name;
		public bool usingRightTrigger;
		public bool usingLeftTrigger;
		public bool rightTriggerDown;
		public bool leftTriggerDown;
		public bool usingDPadX;
		public bool usingDPadY;
	}

	[System.Serializable]
	public class multiAxes
	{
		public string axesName;
		public bool currentlyActive = true;
		public List<Axes> axes = new List<Axes> ();
	}

	[System.Serializable]
	public class multiAxesEditButtonInput
	{
		public List<editButtonInput> buttonsList = new List<editButtonInput> ();
	}
}