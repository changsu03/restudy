using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class saveGameSystem : MonoBehaviour
{
	public int sceneToLoadNewGame;
	public int saveStationId;
	public Transform saveStationPosition;

	public bool isPhysicalSaveStation = true;
	public bool useSavingIcon;
	public GameObject savingIcon;
	public string savingGameAnimationName;

	public bool usePlayerCameraOrientation;
	public GameObject playerCameraGameObject;
	public GameObject playerControllerGameObject;

	public GameObject saveMenu;
	public string animationName;
	public Image saveButton;
	public Image loadButton;
	public Image deleteButton;
	public Scrollbar scrollBar;
	public GameObject saveGameList;
	public Color disableButtonsColor;
	public Camera photoCapturer;
	public Vector2 captureResolution;

	public bool showDebugInfo;

	public saveStationInfo saveInfo;

	public bool usingSaveStation;
	public bool canSave;
	public bool canLoad;
	public bool canDelete;

	List<buttonInfo> saveGameListElements = new List<buttonInfo> ();

	Animation stationAnimation;

	Button currentButton;
	Color originalColor;
	gameManager gameManagerComponent;
	int currentButtonIndex;

	string currentSaveDataPath;
	string currentSaveDataName;
	GameObject slotPrefab;
	Animation savingGameAnimation;
	int checkpointSlotsAmount = 1;
	electronicDevice electronicDeviceManager;
	playerWeaponsManager weaponsManager;
	inventoryManager currentPlayerInventoryManager;

	void Start ()
	{
		startGameSystem ();
	}

	public void startGameSystem ()
	{
		gameManagerComponent = FindObjectOfType<gameManager> ();
		currentSaveDataPath = gameManagerComponent.getDataPath ();
		currentSaveDataName = gameManagerComponent.getDataName ();

		saveMenu.SetActive (true);

		Component component = saveGameList.GetComponentInChildren (typeof(saveGameSlot));
		slotPrefab = component.gameObject;

		saveGameListElements.Add (slotPrefab.GetComponent<saveGameSlot> ().buttonInfo);
		int slotsAmount = gameManagerComponent.slotBySaveStation - 1;
		for (int i = 0; i < slotsAmount; i++) {	
			addSaveSlot (i);
		}

		for (int i = 0; i < checkpointSlotsAmount; i++) {	
			addSaveSlot (saveGameListElements.Count - 1);
			saveGameListElements [saveGameListElements.Count - 1].isCheckpointSave = true;
			saveGameListElements [saveGameListElements.Count - 1].slotGameObject.SetActive (false);
		}

		scrollBar.value = 1;
		saveMenu.SetActive (false);
		stationAnimation = GetComponent<Animation> ();
		setSaveGameTransformInfo (saveStationPosition, true);

		saveInfo.saveStationScene = SceneManager.GetActiveScene ().buildIndex;

		originalColor = loadButton.color;
		changeButtonsColor (false, false, false, false);

		setPhooCapturerState (false);

		if (useSavingIcon) {
			savingGameAnimation = savingIcon.GetComponent<Animation> ();
		}

		electronicDeviceManager = GetComponent<electronicDevice> ();

		loadStates ();
	}

	public void addSaveSlot (int index)
	{
		GameObject newSlotPrefab = (GameObject)Instantiate (slotPrefab, slotPrefab.transform.position, slotPrefab.transform.rotation);
		newSlotPrefab.transform.SetParent (slotPrefab.transform.parent);
		newSlotPrefab.transform.localScale = Vector3.one;
		newSlotPrefab.name = "Save Game Slot " + (index + 2).ToString ();
		saveGameListElements.Add (newSlotPrefab.GetComponent<saveGameSlot> ().buttonInfo);
	}

	public void setSaveGameTransformInfo (Transform savePosition, bool canUsePlayerCameraOrientation)
	{

		if (savePosition == null) {
			return;
		}

		saveInfo.saveStationPositionX = savePosition.position.x;
		saveInfo.saveStationPositionY = savePosition.position.y;
		saveInfo.saveStationPositionZ = savePosition.position.z;
		saveInfo.saveStationRotationX = savePosition.eulerAngles.x;
		saveInfo.saveStationRotationY = savePosition.eulerAngles.y;
		saveInfo.saveStationRotationZ = savePosition.eulerAngles.z;

		if (usePlayerCameraOrientation && canUsePlayerCameraOrientation) {
			saveInfo.usePlayerCameraOrientation = true;
			saveInfo.playerCameraRotationX = playerCameraGameObject.transform.eulerAngles.x;
			saveInfo.playerCameraRotationY = playerCameraGameObject.transform.eulerAngles.y;
			saveInfo.playerCameraRotationZ = playerCameraGameObject.transform.eulerAngles.z;

			playerCamera currentPlayerCamera = playerCameraGameObject.GetComponent<playerCamera> ();
			Vector3 playerCameraPivotEulerRotation = currentPlayerCamera.getPivotCameraTransform ().localEulerAngles;

			saveInfo.playerCameraPivotRotationX = playerCameraPivotEulerRotation.x;
		} else {
			saveInfo.usePlayerCameraOrientation = false;
		}
	}

	public void activateSaveStation ()
	{
		usingSaveStation = !usingSaveStation;
		if (usingSaveStation) {
			loadStates ();
			saveMenu.SetActive (true);
			stationAnimation.Stop ();
			stationAnimation [animationName].speed = 1;
			stationAnimation.Play (animationName);
			scrollBar.value = 1;
		} else {
			saveMenu.SetActive (false);
			stationAnimation.Stop ();
			stationAnimation [animationName].speed = -1;
			stationAnimation [animationName].time = stationAnimation [animationName].length;
			stationAnimation.Play (animationName);
			changeButtonsColor (false, false, false, false);
		}
	}

	public void openSaveGameMenu ()
	{
		loadStates ();
		changeButtonsColor (false, false, false, false);
		setSaveGameTransformInfo (saveStationPosition, true);
		scrollBar.value = 1;
	}

	public void getSaveButtonSelected (Button button)
	{
		currentButtonIndex = -1;
		bool save = false;
		bool load = false;
		bool delete = false;
		for (int i = 0; i < saveGameListElements.Count; i++) {		
			if (saveGameListElements [i].button == button) {
				currentButtonIndex = i;	
				currentButton = button;
				if (saveGameListElements [i].infoAdded) {
					load = true;
				}
				if (!saveGameListElements [i].isCheckpointSave) {
					if (saveGameListElements [i].infoAdded) {
						delete = true;
					}
					save = true;
				}
			}
		}
		changeButtonsColor (true, save, load, delete);
	}

	public void newGame ()
	{
		PlayerPrefs.SetInt ("loadingGame", 0);
		SceneManager.LoadScene (sceneToLoadNewGame);
	}

	public void continueGame ()
	{
		saveStationInfo recentSave = new saveStationInfo ();
		List<saveStationInfo> saveList = loadFile ();
		long closestDate = 0;

		for (int j = 0; j < saveList.Count; j++) {
			//print (saveList [j].saveDate.Ticks);
			if (saveList [j].saveDate.Ticks > closestDate) {
				closestDate = saveList [j].saveDate.Ticks;
				recentSave = saveList [j];
			}
		}
		//print ("newer" + recentSave.saveDate+" "+recentSave.saveNumber);
		gameManagerComponent.getPlayerPrefsInfo (recentSave);
	}

	public void saveGame ()
	{
		if (currentButton && canSave) {
			int saveSlotIndex = -1;
			for (int i = 0; i < saveGameListElements.Count; i++) {
				if (saveGameListElements [i].button == currentButton) {
					saveGameListElements [i].infoAdded = true;
					saveSlotIndex = i;	
				}
			}

			saveCurrentGame (saveSlotIndex, false, false, -1, -1);
		}
	}

	public void playSavingGameAnimation ()
	{
		savingGameAnimation.Stop ();
		savingGameAnimation [savingGameAnimationName].speed = 1;
		savingGameAnimation.Play (savingGameAnimationName);
	}

	public void saveGameCheckpoint (Transform customTransform, int checkpointID, int checkpointSceneID, bool overwriteThisCheckpointActive)
	{
		int saveSlotIndex = -1;
		for (int i = 0; i < saveGameListElements.Count; i++) {
			if (saveGameListElements [i].isCheckpointSave) {
				saveSlotIndex = i;
			}
		}

		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		if (!overwriteThisCheckpointActive) {
			print (saveList.Count);
			for (int j = 0; j < saveList.Count; j++) {
				if (saveList [j].saveNumber == saveSlotIndex) {
					print (saveList [j].checkpointID + " " + checkpointID + " " + saveList [j].checkpointSceneID + " " + checkpointSceneID);
					if (saveList [j].checkpointID == checkpointID && saveList [j].checkpointSceneID == checkpointSceneID) {
						print ("trying to save in the same checkpoint where the player has started, save canceled");
						checkpointSystem checkpointManager = FindObjectOfType<checkpointSystem> ();
						checkpointManager.disableCheckPoint (checkpointID);
						return;
					}
				}
			}
		}

		bool useCustomSaveTransform = false;
		if (customTransform != null) {
			setSaveGameTransformInfo (customTransform, false);
			useCustomSaveTransform = true;
		} else {
			setSaveGameTransformInfo (saveStationPosition, true);
		}

		saveCurrentGame (saveSlotIndex, useCustomSaveTransform, true, checkpointID, checkpointSceneID);

		playSavingGameAnimation ();
	}

	public void saveCurrentGame (int saveSlotIndex, bool useCustomSaveTransform, bool isCheckpointSave, int checkpointID, int checkpointSceneID)
	{
		bool saveLocated = false;
		saveStationInfo newSave = new saveStationInfo (saveInfo);
		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		int currentSaveNumber = 0;

		if (saveList.Count == 0) {
			saveList = loadFile ();
			gameManagerComponent.setSaveList (saveList);
		}

		for (int j = 0; j < saveList.Count; j++) {
			if (saveList [j].saveNumber == saveSlotIndex && !saveLocated) {
				newSave = saveList [j];

				print ("Overwritting a previous slot " + saveSlotIndex + " " + j);
				saveLocated = true;
				currentSaveNumber = saveSlotIndex;
				//print ("save found");
			}
		}

		newSave.saveStationPositionX = saveInfo.saveStationPositionX;
		newSave.saveStationPositionY = saveInfo.saveStationPositionY;
		newSave.saveStationPositionZ = saveInfo.saveStationPositionZ;
		newSave.saveStationRotationX = saveInfo.saveStationRotationX;
		newSave.saveStationRotationY = saveInfo.saveStationRotationY;
		newSave.saveStationRotationZ = saveInfo.saveStationRotationZ;

		if (usePlayerCameraOrientation) {
			newSave.playerCameraRotationX = saveInfo.playerCameraRotationX;
			newSave.playerCameraRotationY = saveInfo.playerCameraRotationY;
			newSave.playerCameraRotationZ = saveInfo.playerCameraRotationZ;

			newSave.playerCameraPivotRotationX = saveInfo.playerCameraPivotRotationX;
		}

		if (!saveLocated) {
			//print ("new save");
			newSave.playTime = gameManagerComponent.playTime;
		} else {
			newSave.playTime += gameManagerComponent.playTime;
		}

		if (isPhysicalSaveStation || useCustomSaveTransform) {
			newSave.useRaycastToPlacePlayer = true;
		}

		newSave.usePlayerCameraOrientation = saveInfo.usePlayerCameraOrientation;

		newSave.isCheckpointSave = isCheckpointSave;
		newSave.checkpointID = checkpointID;
		newSave.checkpointSceneID = checkpointSceneID;

		gameManagerComponent.playTime = 0;
		newSave.chapterNumberAndName = gameManagerComponent.chapterInfo;
		newSave.saveNumber = saveSlotIndex;
		newSave.saveDate = System.DateTime.Now;

		newSave.saveStationScene = saveInfo.saveStationScene;

		if (!saveLocated) {
			saveList.Add (newSave);
			currentSaveNumber = saveSlotIndex;
		}

		showSaveList (saveList);

		if (gameManagerComponent.saveCameraCapture) {
			saveCameraView (newSave.saveNumber.ToString ());
		}

		if (!isCheckpointSave) {
			updateSaveSlotContent (saveSlotIndex, newSave);
		}

		gameManagerComponent.setSaveList (saveList);

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (currentSaveDataPath + currentSaveDataName + ".txt"); 
		bf.Serialize (file, saveList);
		file.Close ();
		changeButtonsColor (false, false, false, false);

		saveWeaponsInfo (currentSaveNumber);

		saveInventoryInfo (currentSaveNumber);

		saveBankInventoryInfo (currentSaveNumber);
	}

	public void updateSaveSlotContent (int saveSlotIndex, saveStationInfo newSave)
	{
		if (gameManagerComponent.saveCameraCapture) {
			
			saveGameListElements [saveSlotIndex].icon.enabled = true;

			#if !UNITY_WEBPLAYER

			if (File.Exists (currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber.ToString () + ".png"))) {
				byte[] bytes = File.ReadAllBytes (currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber.ToString () + ".png"));
				Texture2D texture = new Texture2D ((int)captureResolution.x, (int)captureResolution.y);
				texture.filterMode = FilterMode.Trilinear;
				texture.LoadImage (bytes);
				saveGameListElements [saveSlotIndex].icon.texture = texture;
			}
			#endif
		}

		saveGameListElements [saveSlotIndex].chapterName.text = newSave.chapterNumberAndName;
		saveGameListElements [saveSlotIndex].playTime.text = convertSecondsIntoHours (newSave.playTime);
		saveGameListElements [saveSlotIndex].saveNumber.text = "Save " + newSave.saveNumber.ToString ();
		saveGameListElements [saveSlotIndex].saveDate.text = String.Format ("{0:dd/MM/yy}", newSave.saveDate);
		saveGameListElements [saveSlotIndex].saveHour.text = newSave.saveDate.Hour.ToString ("00") + ":" + newSave.saveDate.Minute.ToString ("00");
	}

	public void reloadLastCheckpoint ()
	{
		bool checkpointSlotFound = false;
		currentButtonIndex = -1;
		for (int i = 0; i < saveGameListElements.Count; i++) {	
			if (!checkpointSlotFound) {	
				if (saveGameListElements [i].isCheckpointSave) {
					currentButtonIndex = i;	
					currentButton = saveGameListElements [i].button;
					canLoad = true;
					checkpointSlotFound = true;
					loadGame ();
				}
			}
		}
	}

	public void loadGame ()
	{
		if (currentButton && canLoad) {
			saveStationInfo newSave = new saveStationInfo (saveInfo);
			List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

			if (saveList.Count == 0) {
				saveList = loadFile ();
				gameManagerComponent.setSaveList (saveList);
			}

			for (int j = 0; j < saveList.Count; j++) {
				if (saveList [j].saveNumber == currentButtonIndex) {
					newSave = new saveStationInfo (saveList [j]);
					//print ("save loaded");

					showSaveSlotInfo (newSave, j);
				}
			}

			gameManagerComponent.getPlayerPrefsInfo (newSave);
		}
	}

	public void deleteGame ()
	{
		if (currentButton && canDelete) {
			bool saveLocated = false;
			saveStationInfo newSave = new saveStationInfo (saveInfo);
			List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

			if (saveList.Count == 0) {
				saveList = loadFile ();
				gameManagerComponent.setSaveList (saveList);
			}

			for (int j = 0; j < saveList.Count; j++) {
				if (saveList [j].saveNumber == currentButtonIndex) {
					newSave = saveList [j];
					saveLocated = true;
					//print ("save deleted");
				}
			}

			if (File.Exists (currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber.ToString () + ".png"))) {
				File.Delete (currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber.ToString () + ".png"));
			}

			if (saveLocated) {
				saveList.Remove (newSave);
			}

			showSaveList (saveList);

			saveGameListElements [currentButtonIndex].icon.enabled = false;
			saveGameListElements [currentButtonIndex].chapterName.text = "Chapter -";
			saveGameListElements [currentButtonIndex].saveNumber.text = "Save -";
			saveGameListElements [currentButtonIndex].playTime.text = "--:--:--";
			saveGameListElements [currentButtonIndex].saveDate.text = "--/--/--";
			saveGameListElements [currentButtonIndex].saveHour.text = "--:--";

			if (saveGameListElements [currentButtonIndex].isCheckpointSave) {
				saveGameListElements [currentButtonIndex].slotGameObject.SetActive (false);
				scrollBar.value = 0;
			}

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (currentSaveDataPath + currentSaveDataName + ".txt"); 
			bf.Serialize (file, saveList);
			file.Close ();
			changeButtonsColor (false, false, false, false);
		}
	}

	public void changeButtonsColor (bool state, bool save, bool load, bool delete)
	{
		if (saveButton) {
			if (save) {
				saveButton.color = originalColor;
			} else {
				saveButton.color = disableButtonsColor;
			}
		}

		if (load) {
			loadButton.color = originalColor;
		} else {
			loadButton.color = disableButtonsColor;
		}

		if (delete) {
			deleteButton.color = originalColor;
		} else {
			deleteButton.color = disableButtonsColor;
		}

		canSave = save;
		canLoad = load;
		canDelete = delete;

		if (!state) {
			currentButton = null;
		}
	}

	public void loadStates ()
	{
		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		if (saveList.Count == 0) {
			saveList = loadFile ();
			gameManagerComponent.setSaveList (saveList);
		}

		showSaveList (saveList);

		for (int i = 0; i < saveGameListElements.Count; i++) {
			for (int j = 0; j < saveList.Count; j++) {
				if (saveList [j].saveNumber == i) {
					
					updateSaveSlotContent (i, saveList [j]);

					saveGameListElements [i].infoAdded = true;

					if (saveList [j].isCheckpointSave) {
						saveGameListElements [i].slotGameObject.SetActive (true);
					}
				}
			}
		}

		for (int i = 0; i < saveGameListElements.Count; i++) {
			if (!saveGameListElements [i].infoAdded) {
				saveGameListElements [i].icon.enabled = false;
				saveGameListElements [i].chapterName.text = "Chapter -";
				saveGameListElements [i].saveNumber.text = "Save -";
				saveGameListElements [i].playTime.text = "--:--:--";
				saveGameListElements [i].saveDate.text = "--/--/--";
				saveGameListElements [i].saveHour.text = "--:--";
			}
		}
	}

	void saveCameraView (string saveNumber)
	{
		// get the camera's render texture
		setPhooCapturerState (true);

		photoCapturer.targetTexture = new RenderTexture ((int)captureResolution.x, (int)captureResolution.y, 24);
		RenderTexture rendText = RenderTexture.active;
		RenderTexture.active = photoCapturer.targetTexture;

		// render the texture
		photoCapturer.Render ();
		// create a new Texture2D with the camera's texture, using its height and width
		Texture2D cameraImage = new Texture2D ((int)captureResolution.x, (int)captureResolution.y, TextureFormat.RGB24, false);
		cameraImage.ReadPixels (new Rect (0, 0, (int)captureResolution.x, (int)captureResolution.y), 0, 0);
		cameraImage.Apply ();
		RenderTexture.active = rendText;
		// store the texture into a .PNG file
		#if !UNITY_WEBPLAYER
		byte[] bytes = cameraImage.EncodeToPNG ();
		// save the encoded image to a file
		System.IO.File.WriteAllBytes (currentSaveDataPath + (currentSaveDataName + "_" + saveNumber + ".png"), bytes);
		#endif

		photoCapturer.targetTexture = null;

		setPhooCapturerState (false);
	}

	public void setPhooCapturerState (bool state)
	{
		if (isPhysicalSaveStation) {
			photoCapturer.enabled = state;
		}
	}

	string convertSecondsIntoHours (float value)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds (value);
		string timeText = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		return timeText;
	}

	public List<saveStationInfo> loadFile ()
	{
		List<saveStationInfo> saveList = new List<saveStationInfo> ();
		if (File.Exists (currentSaveDataPath + currentSaveDataName + ".txt")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath + currentSaveDataName + ".txt", FileMode.Open);
			saveList = (List<saveStationInfo>)bf.Deserialize (file);
			file.Close ();	
		}
		return saveList;
	}

	public void setStationIde (int idValue)
	{
		saveInfo.id = idValue;
	}

	public void showSaveList (List<saveStationInfo> saveList)
	{
		if (!showDebugInfo) {
			return;
		}

		for (int i = 0; i < saveList.Count; i++) {
			showSaveSlotInfo (saveList [i], i);
		}
	}

	public void showSaveSlotInfo (saveStationInfo saveSlot, int saveIndex)
	{
		print ("SAVE " + saveIndex);
		print ("Chapter " + saveSlot.chapterNumberAndName);
		print ("Position " + saveSlot.saveStationPositionX + " " + saveSlot.saveStationPositionY + " " + saveSlot.saveStationPositionZ);
		print ("Scene Index " + saveSlot.saveStationScene);
		print ("Id " + saveSlot.id);
		print ("Save Number " + saveSlot.saveNumber);
		print ("PlayTime " + saveSlot.playTime);
		print ("Date " + saveSlot.saveDate);
		print ("Hour " + saveSlot.saveDate.Hour + ":" + saveSlot.saveDate.Minute);
		print ("\n");
	}

	public void saveWeaponsInfo (int currentSaveNumber)
	{
		//save weapons info from this player
		if (playerControllerGameObject == null) {
			playerControllerGameObject = electronicDeviceManager.getCurrentPlayer ();
		}

		weaponsManager = playerControllerGameObject.GetComponent<playerWeaponsManager> ();
		if (weaponsManager) {
			if (weaponsManager.saveCurrentPlayerWeaponsToSaveFile) {
				gameManagerComponent.savePlayerWeaponList (weaponsManager.getPersistanceWeaponInfoList (), currentSaveNumber);
			}
		}
	}

	public void saveInventoryInfo (int currentSaveNumber)
	{
		//save inventory info from this player
		if (playerControllerGameObject == null) {
			playerControllerGameObject = electronicDeviceManager.getCurrentPlayer ();
		}

		currentPlayerInventoryManager = playerControllerGameObject.GetComponent<inventoryManager> ();
		if (currentPlayerInventoryManager) {
			if (currentPlayerInventoryManager.saveCurrentPlayerInventoryToSaveFile) {
				gameManagerComponent.savePlayerInventoryList (currentPlayerInventoryManager.getPersistanceInventoryList (), currentSaveNumber);
			}
		}
	}

	public void saveBankInventoryInfo (int currentSaveNumber)
	{
		//save inventory info from the inventory bank
		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		inventoryBankManager inventoryBank = mainInventoryListManager.getMainInventoryBankManager ();
		if (inventoryBank) {
			if (inventoryBank.saveCurrentBankInventoryToSaveFile) {
				if (playerControllerGameObject == null) {
					playerControllerGameObject = electronicDeviceManager.getCurrentPlayer ();
				}

				if (playerControllerGameObject) {
					inventoryBank.setCurrentPlayer (playerControllerGameObject);
				}

				gameManagerComponent.saveBankInventoryList (inventoryBank.getPersistanceInventoryList (), currentSaveNumber);
			}
		}
	}

	[System.Serializable]
	public class saveStationInfo
	{
		public string chapterNumberAndName;
		public float saveStationPositionX;
		public float saveStationPositionY;
		public float saveStationPositionZ;
		public float saveStationRotationX;
		public float saveStationRotationY;
		public float saveStationRotationZ;

		public bool usePlayerCameraOrientation;
		public float playerCameraRotationX;
		public float playerCameraRotationY;
		public float playerCameraRotationZ;

		public float playerCameraPivotRotationX;

		public int saveStationScene;
		public int id;
		public int saveNumber;
		public float playTime;
		public DateTime saveDate;

		public bool useRaycastToPlacePlayer;

		public bool isCheckpointSave;

		public int checkpointID;

		public int checkpointSceneID;

		public saveStationInfo ()
		{

		}

		public saveStationInfo (saveStationInfo newSaveStationInfo)
		{
			chapterNumberAndName = newSaveStationInfo.chapterNumberAndName;
			saveStationPositionX = newSaveStationInfo.saveStationPositionX;
			saveStationPositionY = newSaveStationInfo.saveStationPositionY;
			saveStationPositionZ = newSaveStationInfo.saveStationPositionZ;
			saveStationRotationX = newSaveStationInfo.saveStationRotationX;
			saveStationRotationY = newSaveStationInfo.saveStationRotationY;
			saveStationRotationZ = newSaveStationInfo.saveStationRotationZ;

			usePlayerCameraOrientation = newSaveStationInfo.usePlayerCameraOrientation;
			playerCameraRotationX = newSaveStationInfo.playerCameraRotationX;
			playerCameraRotationY = newSaveStationInfo.playerCameraRotationY;
			playerCameraRotationZ = newSaveStationInfo.playerCameraRotationZ;

			playerCameraPivotRotationX = newSaveStationInfo.playerCameraPivotRotationX;

			saveStationScene = newSaveStationInfo.saveStationScene;
			id = newSaveStationInfo.id;
			saveNumber = newSaveStationInfo.saveNumber;
			playTime = newSaveStationInfo.playTime;
			saveDate = newSaveStationInfo.saveDate;

			useRaycastToPlacePlayer = newSaveStationInfo.useRaycastToPlacePlayer;

			isCheckpointSave = newSaveStationInfo.isCheckpointSave;

			checkpointID = newSaveStationInfo.checkpointID;

			checkpointSceneID = newSaveStationInfo.checkpointSceneID;
		}
	}

	[System.Serializable]
	public class buttonInfo
	{
		public GameObject slotGameObject;
		public Button button;
		public RawImage icon;
		public Text chapterName;
		public Text playTime;
		public Text saveNumber;
		public Text saveDate;
		public Text saveHour;
		public bool infoAdded;
		public bool isCheckpointSave;
	}
}