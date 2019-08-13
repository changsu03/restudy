using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class consoleMode : MonoBehaviour
{
	public bool consoleModeEnabled = true;

	public List<commandInfo> commandInfoList = new List<commandInfo> ();

	public GameObject consoleWindow;
	public Transform commandTextParent;
	public RectTransform commandTextParentRectTransform;
	public Text currentConsoleCommandText;

	public string incorrectCommandMessage;

	public float lineSpacingAmount;

	public inputManager input;
	public playerInputManager playerInput;
	public bool consoleOpened;
	public menuPause pauseManager;
	public playerController playerControllerManager;

	public ScrollRect commandListScrollRect;

	public Transform spawnPosition;
	public float maxRadiusToInstantiate;
	public float deletingTextRate = 0.3f;
	public float startDeletingTimeAmount = 0.5f;

	public List<string> allowedKeysList = new List<string> ();

	public gameManager mainGameManager;

	List<GameObject> commandTextGameObjectList = new List<GameObject> ();

	string currentKeyPressed;
	string originalKeyPressed;
	string currentTextWritten;
	string previousTextWritten;
	bool capsLockActivated;
	prefabsManager mainPrefabsManager;

	int numberOfLines;
	bool arrowKeyPressed;

	List<string> previousCommandList = new List<string> ();
	int previousCommandListIndex;
	bool canStartDeletingText;
	bool deletingText;
	float lastTimeDeletePressed;
	float lastTimeDeletingText;

	bool autocompletingCommand;
	int currentAutocompleteCommandIndex;
	string currentAutocompleteCommandFound = "";

	void Start ()
	{
		consoleWindow.SetActive (false);
	}

	void Update ()
	{
		if (consoleOpened) {
			currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyDown, true);
			if (currentKeyPressed != "") {
				checkKeyPressed (currentKeyPressed);

				if (currentKeyPressed.ToLower () == "delete" || currentKeyPressed.ToLower () == "backspace") {
					deletingText = true;
					lastTimeDeletePressed = Time.time;
				}
			}

			currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyUp, true);
			if (currentKeyPressed != "") {

				if (currentKeyPressed.ToLower () == "leftshift") {
					capsLockActivated = !capsLockActivated;
				}

				if (currentKeyPressed.ToLower () == "delete" || currentKeyPressed.ToLower () == "backspace") {
					stopDeletingText ();
				}
			}

			if (deletingText) {
				if (canStartDeletingText) {				
					if (Time.time > lastTimeDeletingText + deletingTextRate) {
						if (currentTextWritten.Length > 0) {
							currentTextWritten = currentTextWritten.Remove (currentTextWritten.Length - 1, 1);
						}

						currentConsoleCommandText.text = "> " + currentTextWritten;

						lastTimeDeletingText = Time.time;
					}
				} else {
					if (Time.time > lastTimeDeletePressed + startDeletingTimeAmount) {
						canStartDeletingText = true;
					}
				}
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

		if (keyPressed.Contains ("alpha")) {
			keyPressed = keyPressed.Substring (keyPressed.Length - 1);
			originalKeyPressed = keyPressed;
		}

		//check if the arrow keys have been pressed
		if (keyPressed == "uparrow" || keyPressed == "downarrow") {
			if (previousCommandList.Count > 0) {
				if (arrowKeyPressed) {
					if (keyPressed == "uparrow") {
						previousCommandListIndex--;
						if (previousCommandListIndex < 0) {
							previousCommandListIndex = 0;
						}
					} else {
						previousCommandListIndex++;
						if (previousCommandListIndex > previousCommandList.Count - 1) {
							previousCommandListIndex = previousCommandList.Count - 1;
						}
					}
				}

				arrowKeyPressed = true;

				print ("index " + previousCommandListIndex);

				currentTextWritten = "";
					
				originalKeyPressed = previousCommandList [previousCommandListIndex];
			} else {
				originalKeyPressed = "";
			}
		}

		if (keyPressed == "tab") {
			if (currentTextWritten.Length == 0) {
				return;
			}

			string commandToSearch = currentTextWritten;

			if (currentAutocompleteCommandFound != "") {
				commandToSearch = currentAutocompleteCommandFound;
			}

			bool commandFound = false;
			for (int i = 0; i < commandInfoList.Count; i++) {
				if (!commandFound && commandToSearch.Length <= commandInfoList [i].Name.Length &&
				    commandToSearch.ToLower () == commandInfoList [i].Name.ToLower ().Substring (0, commandToSearch.Length)) {
					if ((autocompletingCommand && i > currentAutocompleteCommandIndex) || !autocompletingCommand) {
						originalKeyPressed = commandInfoList [i].Name;
						if (commandInfoList [i].containsAmount || commandInfoList [i].containsBool || commandInfoList [i].containsName) {
							originalKeyPressed += " ";
						}
						commandFound = true;

						currentAutocompleteCommandIndex = i;
					}
				}
			}

			if (!commandFound) {
				return;
			}

			if (currentAutocompleteCommandFound == "") {
				currentAutocompleteCommandFound = currentTextWritten;
			}

			currentTextWritten = "";

			autocompletingCommand = true;

			print (currentAutocompleteCommandFound + " " + currentAutocompleteCommandIndex);
			if (currentAutocompleteCommandIndex < commandInfoList.Count - 1) {
				bool commandFoundAgain = false;
				for (int i = currentAutocompleteCommandIndex + 1; i < commandInfoList.Count; i++) {
					if (currentAutocompleteCommandFound.ToLower () == commandInfoList [i].Name.ToLower ().Substring (0, currentAutocompleteCommandFound.Length)) {
						commandFoundAgain = true;
					}
				}
				print (commandFoundAgain);
				if (!commandFoundAgain) {
					autocompletingCommand = false;
				}
			} else {
				autocompletingCommand = false;
			}
		} else {
			resetAutocompleteParameters ();
		}

		//add the an space
		if (keyPressed == "space") {
			currentTextWritten += " ";
		}

		//delete the last character
		else if (keyPressed == "delete" || keyPressed == "backspace") {
			if (currentTextWritten.Length > 0) {
				currentTextWritten = currentTextWritten.Remove (currentTextWritten.Length - 1);
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
			currentTextWritten += originalKeyPressed;
		}

		currentConsoleCommandText.text = "> " + currentTextWritten;

		//the enter key has been pressed, so check if the current text written is the correct password
		if (checkPass) {
			previousTextWritten = currentTextWritten.ToLower ();
			checkCurrentCommand (currentTextWritten);

			currentTextWritten = "";
		}

		setScrollRectPosition (Vector3.up * lineSpacingAmount * numberOfLines);
	}

	public void resetAutocompleteParameters ()
	{
		autocompletingCommand = false;
		currentAutocompleteCommandIndex = 0;
		currentAutocompleteCommandFound = "";
	}

	public bool checkCurrentCommand (string currentCommand)
	{
		if (currentCommand == "") {
			createNewCommandText ("> ");
			return false;
		}

		arrowKeyPressed = false;
		previousCommandList.Add (currentCommand);
		previousCommandListIndex = previousCommandList.Count - 1;

		resetAutocompleteParameters ();

		createNewCommandText ("> " + currentCommand);

		currentCommand = currentCommand.ToLower ();
		for (int i = 0; i < commandInfoList.Count; i++) {
			string commandName = commandInfoList [i].Name.ToLower ();
			bool commandFound = false;

			if (commandName == currentCommand) {
				commandFound = true;
			}

			if (!commandFound && (commandInfoList [i].containsAmount || commandInfoList [i].containsBool || commandInfoList [i].containsName)) {
				if (currentCommand.Contains (commandName)) {
					commandFound = true;
				}
			}

			if (commandFound) {

				//check the parameters of the command
				string nameParameter = "";
				bool incorrectCommand = false;
				if (commandInfoList [i].containsName) {
					nameParameter = currentCommand.Replace (commandInfoList [i].Name.ToLower () + " ", "");

					if (nameParameter.Length == 0) {
						incorrectCommand = true;
					} else {
						int amount = 0;

						string[] digits = Regex.Split (currentCommand, @"\D+");
						foreach (string value in digits) {
							if (int.TryParse (value, out amount)) {
								incorrectCommand = true;
							}
						}
					}
				}

				int amountValue = 0;
				if (commandInfoList [i].containsAmount) {
					bool numberFound = false;
					string[] digits = Regex.Split (currentCommand, @"\D+");
					foreach (string value in digits) {
						if (int.TryParse (value, out amountValue)) {
							Debug.Log (value);
							numberFound = true;
						}
					}

					if (commandInfoList [i].containsName) {
						nameParameter = nameParameter.Replace (amountValue.ToString (), "");
						if (nameParameter.Length > 0) {
							nameParameter = nameParameter.Remove (nameParameter.Length - 1, 1);
						}
					}

					incorrectCommand = !numberFound;
				}

				bool boolValue = false;
				if (commandInfoList [i].containsBool) {
					bool boolFound = false;
					if (currentCommand.Contains ("true")) {
						boolValue = true;
						boolFound = true;
					} else if (currentCommand.Contains ("false")) {
						boolFound = true;
					}
					print (boolValue);

					incorrectCommand = !boolFound;
				}

				//if the command is not correctly written, show the incorrect message and stop
				if (incorrectCommand) {
					createNewCommandText (commandInfoList [i].incorrectParametersText);
					return false;
				}
					
				//execute the event with the proper parameter
				if (commandInfoList [i].eventSendValues) {
					if (commandInfoList [i].containsAmount) {
						commandInfoList [i].eventToCallAmount.Invoke ((float)amountValue);
					}

					if (commandInfoList [i].containsBool) {
						commandInfoList [i].eventToCallBool.Invoke (boolValue);
					}

					print (nameParameter);

					if (commandInfoList [i].containsName) {
						commandInfoList [i].eventToCallName.Invoke (nameParameter);
					}
				} else {
					if (commandInfoList [i].eventToCall.GetPersistentEventCount () > 0) {
						commandInfoList [i].eventToCall.Invoke ();
					}
				}

				createNewCommandText (commandInfoList [i].commandExecutedText);
				return true;
			}
		}
		createNewCommandText (incorrectCommandMessage);
		return false;
	}

	public void createNewCommandText (string commandContent)
	{
		if (commandContent == "") {
			return;
		}
		GameObject newConsoleCommnadGameObject = (GameObject)Instantiate (currentConsoleCommandText.gameObject, Vector3.zero, Quaternion.identity);
		newConsoleCommnadGameObject.transform.SetParent (commandTextParent);
		newConsoleCommnadGameObject.transform.localScale = Vector3.one;
		newConsoleCommnadGameObject.GetComponent<Text> ().text = commandContent;
		commandTextGameObjectList.Add (newConsoleCommnadGameObject);
		currentConsoleCommandText.transform.SetSiblingIndex (commandTextParent.childCount - 1);
		currentConsoleCommandText.text = ">";

		numberOfLines++;

		setScrollRectPosition (Vector3.up * lineSpacingAmount * numberOfLines);
	}

	Coroutine scrollRectCoroutine;

	public void setScrollRectPosition (Vector3 position)
	{
		if (scrollRectCoroutine != null) {
			StopCoroutine (scrollRectCoroutine);
		}
		scrollRectCoroutine = StartCoroutine (setScrollRectPositionCoroutine (position));
	}

	IEnumerator setScrollRectPositionCoroutine (Vector3 position)
	{
		commandListScrollRect.vertical = false;

		//yield return new WaitForSeconds (0.01f);
		commandTextParentRectTransform.localPosition = position;

		yield return new WaitForSeconds (0.01f);
		commandListScrollRect.vertical = true;
	}

	public void showCommandList ()
	{
		for (int i = 0; i < commandInfoList.Count; i++) {
			createNewCommandText (commandInfoList [i].Name + " -> " + commandInfoList [i].description);
		}
	}

	public void clearCommandList ()
	{
		for (int i = 0; i < commandTextGameObjectList.Count; i++) {
			Destroy (commandTextGameObjectList [i]);
		}

		setScrollRectPosition (Vector3.zero);
		numberOfLines = 0;

		previousCommandList.Clear ();
		previousCommandListIndex = 0;
	}

	public void openOrCloseConsoleMode (bool state)
	{
		consoleOpened = state;
		consoleWindow.SetActive (consoleOpened);

		pauseManager.showOrHideCursor (consoleOpened);

		pauseManager.setHeadBobPausedState (consoleOpened);

		pauseManager.changeCursorState (!consoleOpened);

		playerControllerManager.changeScriptState (!consoleOpened);

		pauseManager.openOrClosePlayerMenu (consoleOpened, null, false);

		pauseManager.usingDeviceState (consoleOpened);

		mainGameManager.setGamePauseState (consoleOpened);

		playerInput.setPlayerInputEnabledState (!consoleOpened);

		stopDeletingText ();

		pauseManager.setIngameMenuOpenedState ("Console Mode", consoleOpened);
	}

	public void stopDeletingText ()
	{
		deletingText = false;
		canStartDeletingText = false;
	}

	public void spawnObject ()
	{
		bool objectToSpawnFound = false;

		if (!mainPrefabsManager) {
			mainPrefabsManager = FindObjectOfType<prefabsManager> ();
		}

		if (mainPrefabsManager) {
			string objectToSpawnName = previousTextWritten.Replace ("spawn", "");
			int amountToSpawn = 0;

			if (objectToSpawnName.Length == 0) {
				return;
			}

			string[] digits = Regex.Split (objectToSpawnName, @"\D+");
			foreach (string value in digits) {
				if (int.TryParse (value, out amountToSpawn)) {
					Debug.Log (value);
				}
			}

			objectToSpawnName = objectToSpawnName.Replace (amountToSpawn.ToString (), "");

			if (objectToSpawnName.Length == 0 || objectToSpawnName.Length < 3) {
				return;
			}

			objectToSpawnName = objectToSpawnName.Remove (0, 1);

			objectToSpawnName = objectToSpawnName.Remove (objectToSpawnName.Length - 1, 1);

			print (objectToSpawnName);

			int prefabTypeInfoIndex = -1;
			int prefabInfoIndex = -1;
			for (int i = 0; i < mainPrefabsManager.prefabTypeInfoList.Count; i++) {
				for (int j = 0; j < mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList.Count; j++) {
					if (mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList [j].Name.ToLower () == objectToSpawnName && !objectToSpawnFound) {
						prefabTypeInfoIndex = i;
						prefabInfoIndex = j;
						objectToSpawnFound = true;
					}
				}
			}

			if (objectToSpawnFound) { 

				GameObject objectToSpawn = mainPrefabsManager.prefabTypeInfoList [prefabTypeInfoIndex].prefabInfoList [prefabInfoIndex].prefabGameObject;

				if (objectToSpawn) {
					for (int i = 0; i < amountToSpawn; i++) {
						Vector3 positionToSpawn = spawnPosition.position;
						positionToSpawn += Random.insideUnitSphere * maxRadiusToInstantiate;
						spawnGameObject (objectToSpawn, positionToSpawn, spawnPosition.rotation);
					}
				}
			}
		}

		if (!objectToSpawnFound) {
			createNewCommandText ("That object doesn't exist");
		}
	}

	public void showSpawnObjectsList ()
	{
		if (!mainPrefabsManager) {
			mainPrefabsManager = FindObjectOfType<prefabsManager> ();
		}

		if (mainPrefabsManager) {
			for (int i = 0; i < mainPrefabsManager.prefabTypeInfoList.Count; i++) {
				for (int j = 0; j < mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList.Count; j++) {
					createNewCommandText (mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList [j].Name);
				}
			}
		}
	}

	public void spawnGameObject (GameObject objectToSpawn, Vector3 position, Quaternion rotation)
	{
		GameObject newConsoleCommnadGameObject = (GameObject)Instantiate (objectToSpawn, position, rotation);
		newConsoleCommnadGameObject.transform.position = position;
	}

	public void addCommand ()
	{
		commandInfo newCommandInfo = new commandInfo ();
		newCommandInfo.Name = "New Command";

		commandInfoList.Add (newCommandInfo);
		updateComponent ();
	}

	public void killAllEnemies ()
	{
		health[] healthList = FindObjectsOfType (typeof(health)) as health[];
		for (int i = 0; i < healthList.Length; i++) {
			if (healthList [i].gameObject.tag != "Player" && healthList [i].gameObject.tag != "friend") {
				healthList [i].killByButton ();
			}
		}
	}

	public void killAllCharacters ()
	{
		health[] healthList = FindObjectsOfType (typeof(health)) as health[];
		for (int i = 0; i < healthList.Length; i++) {
			healthList [i].killByButton ();
		}
	}

	public void destroyAllVehicles ()
	{
		vehicleHUDManager[] vehicleHUDManagerList = FindObjectsOfType (typeof(vehicleHUDManager)) as vehicleHUDManager[];
		for (int i = 0; i < vehicleHUDManagerList.Length; i++) {
			vehicleHUDManagerList [i].destroyVehicle ();
		}
	}

	public void inputActivateConsoleMode ()
	{
		if (!consoleModeEnabled) {
			return;
		}

		openOrCloseConsoleMode (!consoleOpened);
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<consoleMode> ());
		#endif
	}

	[System.Serializable]
	public class commandInfo
	{
		public string Name;
		[TextArea (1, 10)] public string description;
		[TextArea (1, 10)] public string commandExecutedText;
		[TextArea (1, 10)] public string incorrectParametersText;

		public UnityEvent eventToCall;

		public bool eventSendValues;
		public bool containsAmount;
		public bool containsBool;
		public bool containsName;

		[System.Serializable]
		public class eventToCallWithAmount : UnityEvent<float>
		{

		}

		[SerializeField] public eventToCallWithAmount eventToCallAmount;

		[System.Serializable]
		public class eventToCallWithBool : UnityEvent<bool>
		{

		}

		[SerializeField] public eventToCallWithBool eventToCallBool;

		[System.Serializable]
		public class eventToCallWithName : UnityEvent<string>
		{

		}

		[SerializeField] public eventToCallWithName eventToCallName;
	}
}
