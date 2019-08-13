using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class gameManager : MonoBehaviour
{
	public bool loadEnabled;

	public GameObject mainPlayerGameObject;
	public GameObject mainPlayerCameraGameObjec;
	public playerCamera currentPlayerCamera;

	public float playTime;
	public string chapterInfo;

	public bool useRelativePath;

	public string versionNumber = "3-01";

	public string saveGameFolderName = "Save";
	public string saveFileName = "Save State";

	public string saveWeaponsFileName = "Save Weapons";

	public string saveInventoryFileName = "Save Inventory";

	public string saveBankInventoryFileName = "Save Inventory Bank";

	public string saveCaptureFolder = "Captures";
	public string saveCaptureFileName = "Photo";

	public string touchControlsPositionFolderName = "Touch Buttons Position";
	public string touchControlsPositionFileName = "Touch Positions";

	public string saveInputFileFolderName = "Input";
	public string saveInputFileName = "Input File";

	public string defaultInputSaveFileName = "Default Input File";

	public string fileExtension = ".txt";

	public int slotBySaveStation;

	public bool saveCameraCapture = true;

	public LayerMask layer;

	public int saveNumberToLoad = -1;

	public bool showDebugLoadedWeaponInfo;
	public bool showDebugLoadedInventoryInfo;
	public bool showDebugLoadedBanKInventoryInfo;

	public playerCharactersManager charactersManager;

	public Camera mainCamera;

	public string currentPersistentDataPath;

	public bool limitFps;

	public int targetFps = 60;

	public bool gamePaused;

	public bool useTouchControls = false;

	public bool touchPlatform;

	public List<saveGameSystem.saveStationInfo> saveList = new List<saveGameSystem.saveStationInfo> ();

	public List<persistanceInventoryListBySaveSlotInfo> inventorySaveList = new List<persistanceInventoryListBySaveSlotInfo> ();

	saveGameSystem saveGameInfo;
	RaycastHit hit;

	string currentSaveDataPath;
	int lastSaveNumber = -1;

	void Awake ()
	{
		if (limitFps) {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFps;
		} else {
			Application.targetFrameRate = -1;
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (touchPlatform) {
			useRelativePath = false;
		}
	}

	void Start ()
	{
		currentPersistentDataPath = Application.persistentDataPath;

		if (loadEnabled) {
			if (PlayerPrefs.HasKey ("chapterInfo")) {
				chapterInfo = PlayerPrefs.GetString ("chapterInfo");
			}

			if (PlayerPrefs.HasKey ("loadingGame")) {
				if (PlayerPrefs.GetInt ("loadingGame") == 1) {

					Vector3 newPlayerPosition = 
						new Vector3 (PlayerPrefs.GetFloat ("saveStationPositionX"), PlayerPrefs.GetFloat ("saveStationPositionY"), PlayerPrefs.GetFloat ("saveStationPositionZ"));
					Quaternion newPlayerRotation = 
						Quaternion.Euler (PlayerPrefs.GetFloat ("saveStationRotationX"), PlayerPrefs.GetFloat ("saveStationRotationY"), PlayerPrefs.GetFloat ("saveStationRotationZ"));

					if (PlayerPrefs.GetInt ("useRaycastToPlacePlayer") == 1) {
						if (Physics.Raycast (newPlayerPosition + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, layer)) {
							newPlayerPosition = hit.point;
						}
					}

					//set player position and rotation
					mainPlayerGameObject.transform.position = newPlayerPosition;
					mainPlayerGameObject.transform.rotation = newPlayerRotation;

					Quaternion newCameraRotation = newPlayerRotation;

					if (PlayerPrefs.GetInt ("usePlayerCameraOrientation") == 1) {
						newCameraRotation =
							Quaternion.Euler (PlayerPrefs.GetFloat ("playerCameraRotationX"), PlayerPrefs.GetFloat ("playerCameraRotationY"), PlayerPrefs.GetFloat ("playerCameraRotationZ"));

						float playerCameraPivotRotationX = PlayerPrefs.GetFloat ("playerCameraPivotRotationX");
					
						float newLookAngle = playerCameraPivotRotationX;

						if (newLookAngle > 180) {
							newLookAngle -= 360;
						}
						Vector2 newPivotRotation = new Vector2 (0, newLookAngle);

						currentPlayerCamera.setLookAngleValue (newPivotRotation);

						currentPlayerCamera.getPivotCameraTransform ().localRotation = Quaternion.Euler (playerCameraPivotRotationX, 0, 0);
					}
						
					mainPlayerCameraGameObjec.transform.position = newPlayerPosition;
					mainPlayerCameraGameObjec.transform.rotation = newCameraRotation;

					PlayerPrefs.DeleteAll ();
				}
			}
		} else {
			PlayerPrefs.DeleteAll ();
		}
	}

	void Update ()
	{
		playTime += Time.deltaTime;
	
		if (limitFps) {
			if (Application.targetFrameRate != targetFps) {
				Application.targetFrameRate = targetFps;
			}
		}
	}

	public void getPlayerPrefsInfo (saveGameSystem.saveStationInfo save)
	{
		PlayerPrefs.SetInt ("loadingGame", 1);
		PlayerPrefs.SetInt ("saveNumber", save.saveNumber);
		PlayerPrefs.SetInt ("currentSaveStationId", save.id);
		PlayerPrefs.SetFloat ("saveStationPositionX", save.saveStationPositionX);
		PlayerPrefs.SetFloat ("saveStationPositionY", save.saveStationPositionY);
		PlayerPrefs.SetFloat ("saveStationPositionZ", save.saveStationPositionZ);
		PlayerPrefs.SetFloat ("saveStationRotationX", save.saveStationRotationX);
		PlayerPrefs.SetFloat ("saveStationRotationY", save.saveStationRotationY);
		PlayerPrefs.SetFloat ("saveStationRotationZ", save.saveStationRotationZ);

		if (save.usePlayerCameraOrientation) {
			PlayerPrefs.SetInt ("usePlayerCameraOrientation", 1);
		} else {
			PlayerPrefs.SetInt ("usePlayerCameraOrientation", 0);
		}

		PlayerPrefs.SetFloat ("playerCameraRotationX", save.playerCameraRotationX);
		PlayerPrefs.SetFloat ("playerCameraRotationY", save.playerCameraRotationY);
		PlayerPrefs.SetFloat ("playerCameraRotationZ", save.playerCameraRotationZ);
		PlayerPrefs.SetFloat ("playerCameraPivotRotationX", save.playerCameraPivotRotationX);

		if (save.useRaycastToPlacePlayer) {
			PlayerPrefs.SetInt ("useRaycastToPlacePlayer", 1);
		}

		SceneManager.LoadScene (save.saveStationScene);
	}

	public string getDataPath ()
	{
		string dataPath = "";
		if (useRelativePath) {
			dataPath = saveGameFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + saveGameFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/";

		return dataPath;
	}

	public string getDataName ()
	{
		return saveFileName + " " + versionNumber;
	}

	public List<saveGameSystem.saveStationInfo> getCurrentSaveList ()
	{
		return saveList;
	}

	public void setSaveList (List<saveGameSystem.saveStationInfo> currentList)
	{
		saveList = currentList;
	}

	//save weapons info
	public List<persistanceWeaponInfo> loadPlayerWeaponList (int playerID)
	{
		currentSaveDataPath = getDataPath ();

		List<persistanceWeaponInfo> playerWeaponList = new List<persistanceWeaponInfo> ();

		List<persistanceWeaponListBySaveSlotInfo> weaponSaveList = new List<persistanceWeaponListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath + saveWeaponsFileName + " " + versionNumber + fileExtension)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath + saveWeaponsFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			weaponSaveList = currentData as List<persistanceWeaponListBySaveSlotInfo>;

			file.Close ();	
		}

		lastSaveNumber = -1;
		if (PlayerPrefs.HasKey ("saveNumber")) {
			lastSaveNumber = PlayerPrefs.GetInt ("saveNumber");
		} else if (saveNumberToLoad >= 0) {
			lastSaveNumber = saveNumberToLoad;
		}

		if (lastSaveNumber > -1) {
			persistanceWeaponListBySaveSlotInfo newPersistanceWeaponListBySaveSlotInfo = new persistanceWeaponListBySaveSlotInfo ();
			for (int j = 0; j < weaponSaveList.Count; j++) {
				if (weaponSaveList [j].saveNumber == lastSaveNumber) {
					newPersistanceWeaponListBySaveSlotInfo = weaponSaveList [j];
				}
			}

			int playerWeaponIndex = -1;
			for (int j = 0; j < newPersistanceWeaponListBySaveSlotInfo.playerWeaponList.Count; j++) {
				if (newPersistanceWeaponListBySaveSlotInfo.playerWeaponList [j].playerID == playerID) {
					playerWeaponIndex = j;
				}
			}

			if (playerWeaponIndex > -1) {
				persistanceWeaponListByPlayerInfo playerWeaponListToLoad = newPersistanceWeaponListBySaveSlotInfo.playerWeaponList [playerWeaponIndex];
				playerWeaponList = playerWeaponListToLoad.weaponList;
			}
		}

		if (showDebugLoadedWeaponInfo) {
			print ("Weapons Loaded in Save Number " + lastSaveNumber);
			print ("Weapons amount: " + playerWeaponList.Count);
			for (int j = 0; j < playerWeaponList.Count; j++) {
				print ("Weapon Name: " + playerWeaponList [j].Name + " Is Enabled: " + playerWeaponList [j].isWeaponEnabled +
				"Is Current Weapon: " + playerWeaponList [j].isCurrentWeapon + " Remain Ammo: " + playerWeaponList [j].remainingAmmo);
			}
		}

		return playerWeaponList;
	}

	public void savePlayerWeaponList (persistanceWeaponListByPlayerInfo weaponListToSave, int currentSaveNumber)
	{
		currentSaveDataPath = getDataPath ();

		bool saveLocated = false;
		bool playerLoacated = false;

		int saveSlotIndex = -1;
		int weaponListPlayerIndex = -1;

		persistanceWeaponListBySaveSlotInfo newPersistanceWeaponListBySaveSlotInfo = new persistanceWeaponListBySaveSlotInfo ();
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		List<persistanceWeaponListBySaveSlotInfo> weaponSaveList = new List<persistanceWeaponListBySaveSlotInfo> ();
		if (File.Exists (currentSaveDataPath + saveWeaponsFileName + " " + versionNumber + fileExtension)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath + saveWeaponsFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			weaponSaveList = currentData as List<persistanceWeaponListBySaveSlotInfo>;

			file.Close ();	
		}

		for (int j = 0; j < weaponSaveList.Count; j++) {
			if (weaponSaveList [j].saveNumber == currentSaveNumber) {
				newPersistanceWeaponListBySaveSlotInfo = weaponSaveList [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			for (int j = 0; j < newPersistanceWeaponListBySaveSlotInfo.playerWeaponList.Count; j++) {
				if (newPersistanceWeaponListBySaveSlotInfo.playerWeaponList [j].playerID == weaponListToSave.playerID) {
					playerLoacated = true;
					weaponListPlayerIndex = j;
				}
			}
		}

		if (showDebugLoadedWeaponInfo) {
			print ("Number of weapons: " + weaponListToSave.weaponList.Count);

			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Loacated " + playerLoacated);
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLoacated) {
				weaponSaveList [saveSlotIndex].playerWeaponList [weaponListPlayerIndex].weaponList = weaponListToSave.weaponList;
			} else {
				weaponSaveList [saveSlotIndex].playerWeaponList.Add (weaponListToSave);
			}
		} else {
			newPersistanceWeaponListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceWeaponListBySaveSlotInfo.playerWeaponList.Add (weaponListToSave);
			weaponSaveList.Add (newPersistanceWeaponListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Create (currentSaveDataPath + saveWeaponsFileName + " " + versionNumber + fileExtension); 
		bf.Serialize (file, weaponSaveList);
		file.Close ();
	}

	//save player inventory info
	public List<inventoryListElement> loadPlayerInventoryList (int playerID, inventoryManager playerInventoryManager)
	{
		currentSaveDataPath = getDataPath ();

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		List<inventoryListElement> playerInventoryList = new List<inventoryListElement> ();

		inventorySaveList = new List<persistanceInventoryListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath + saveInventoryFileName + " " + versionNumber + fileExtension)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath + saveInventoryFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			inventorySaveList = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		lastSaveNumber = -1;
		if (PlayerPrefs.HasKey ("saveNumber")) {
			lastSaveNumber	= PlayerPrefs.GetInt ("saveNumber");
		} else if (saveNumberToLoad >= 0) {
			lastSaveNumber = saveNumberToLoad;
		}

		if (lastSaveNumber > -1) {
			persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();
			for (int j = 0; j < inventorySaveList.Count; j++) {
				//print (inventorySaveList [j].saveNumber + " " + lastSaveNumber);

				if (inventorySaveList [j].saveNumber == lastSaveNumber) {
					newPersistanceInventoryListBySaveSlotInfo = inventorySaveList [j];
				}

				for (int i = 0; i < inventorySaveList [j].playerInventoryList.Count; i++) {
					print ("info of " + inventorySaveList [j].saveNumber + " with " + inventorySaveList [j].playerInventoryList [i].inventoryObjectList.Count);
				}
			}

			//print ("number " + newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count);
		
			int playerInventoryIndex = -1;
			for (int j = 0; j < newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count; j++) {
				print ("CURRENT ID " + newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [j].playerID);
				if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [j].playerID == playerID) {
					playerInventoryIndex = j;
				}
			}

			//print ("index " + playerInventoryIndex);

			if (playerInventoryIndex > -1) {
				persistanceInventoryListByPlayerInfo playerInventoryListToLoad = newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [playerInventoryIndex];
				for (int j = 0; j < playerInventoryListToLoad.inventoryObjectList.Count; j++) {
					inventoryListElement newInventoryListElement = new inventoryListElement ();
					newInventoryListElement.name = playerInventoryListToLoad.inventoryObjectList [j].name;
					newInventoryListElement.amount = playerInventoryListToLoad.inventoryObjectList [j].amount;
					newInventoryListElement.infiniteAmount = playerInventoryListToLoad.inventoryObjectList [j].infiniteAmount;
					newInventoryListElement.inventoryObjectName = playerInventoryListToLoad.inventoryObjectList [j].inventoryObjectName;
					newInventoryListElement.elementIndex = playerInventoryListToLoad.inventoryObjectList [j].elementIndex;
					newInventoryListElement.isEquipped = playerInventoryListToLoad.inventoryObjectList [j].isEquipped;

					playerInventoryList.Add (newInventoryListElement);
				}

				playerInventoryManager.setInventorySlotAmountValue (playerInventoryListToLoad.inventorySlotAmount);
			}
		}

		if (showDebugLoadedInventoryInfo) {
			print ("Inventory Loaded in Save Number " + lastSaveNumber);
			print ("Number of objects: " + playerInventoryList.Count);

			for (int j = 0; j < playerInventoryList.Count; j++) {
				print ("Object Name: " + playerInventoryList [j].name + " Amount: " + playerInventoryList [j].amount);
			}
		}

		return playerInventoryList;
	}

	public void savePlayerInventoryList (persistanceInventoryListByPlayerInfo playerInventoryListToSave, int currentSaveNumber)
	{
		currentSaveDataPath = getDataPath ();
	
		bool saveLocated = false;
		bool playerLoacated = false;

		int saveSlotIndex = -1;
		int inventoryListPlayerIndex = -1;

		persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		inventorySaveList = new List<persistanceInventoryListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath + saveInventoryFileName + " " + versionNumber + fileExtension)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath + saveInventoryFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			inventorySaveList = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		for (int j = 0; j < inventorySaveList.Count; j++) {
			if (inventorySaveList [j].saveNumber == currentSaveNumber) {
				newPersistanceInventoryListBySaveSlotInfo = inventorySaveList [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			for (int j = 0; j < newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count; j++) {
				if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [j].playerID == playerInventoryListToSave.playerID) {
					playerLoacated = true;
					inventoryListPlayerIndex = j;
				}
			}
		}
			
		if (showDebugLoadedInventoryInfo) {
			print ("EXTRA INFO\n");
			print ("Number of objects: " + playerInventoryListToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Loacated " + playerLoacated);
			print ("Player ID " + playerInventoryListToSave.playerID);
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLoacated) {
				inventorySaveList [saveSlotIndex].playerInventoryList [inventoryListPlayerIndex].inventoryObjectList = playerInventoryListToSave.inventoryObjectList;
			} else {
				inventorySaveList [saveSlotIndex].playerInventoryList.Add (playerInventoryListToSave);
			}
		} else {
			newPersistanceInventoryListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Add (playerInventoryListToSave);
			inventorySaveList.Add (newPersistanceInventoryListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Create (currentSaveDataPath + saveInventoryFileName + " " + versionNumber + fileExtension); 
		bf.Serialize (file, inventorySaveList);
		file.Close ();
	}

	//save bank inventory info
	public List<inventoryListElement> loadBankInventoryList ()
	{
		currentSaveDataPath = getDataPath ();

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		List<inventoryListElement> bankInventoryList = new List<inventoryListElement> ();

		List<persistanceInventoryListBySaveSlotInfo> inventoryBankSaveList = new List<persistanceInventoryListBySaveSlotInfo> ();
		if (File.Exists (currentSaveDataPath + saveBankInventoryFileName + " " + versionNumber + fileExtension)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath + saveBankInventoryFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			inventoryBankSaveList = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		lastSaveNumber = -1;
		if (PlayerPrefs.HasKey ("saveNumber")) {
			lastSaveNumber	= PlayerPrefs.GetInt ("saveNumber");
		} else if (saveNumberToLoad >= 0) {
			lastSaveNumber = saveNumberToLoad;
		}

		if (lastSaveNumber > -1) {
			persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();
			for (int j = 0; j < inventoryBankSaveList.Count; j++) {
				if (inventoryBankSaveList [j].saveNumber == lastSaveNumber) {
					newPersistanceInventoryListBySaveSlotInfo = inventoryBankSaveList [j];
				}
			}

			int bankInventoryIndex = 0;

			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				persistanceInventoryListByPlayerInfo bankInventoryListToLoad = newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [bankInventoryIndex];
				for (int j = 0; j < bankInventoryListToLoad.inventoryObjectList.Count; j++) {
					inventoryListElement newInventoryListElement = new inventoryListElement ();
					newInventoryListElement.name = bankInventoryListToLoad.inventoryObjectList [j].name;
					newInventoryListElement.amount = bankInventoryListToLoad.inventoryObjectList [j].amount;
					newInventoryListElement.infiniteAmount = bankInventoryListToLoad.inventoryObjectList [j].infiniteAmount;
					newInventoryListElement.inventoryObjectName = bankInventoryListToLoad.inventoryObjectList [j].inventoryObjectName;
					newInventoryListElement.elementIndex = bankInventoryListToLoad.inventoryObjectList [j].elementIndex;

					bankInventoryList.Add (newInventoryListElement);
				}
			}
		}

		if (showDebugLoadedBanKInventoryInfo) {
			print ("Inventory Bank Loaded in Save Number " + lastSaveNumber);
			print ("Number of objects: " + bankInventoryList.Count);

			for (int j = 0; j < bankInventoryList.Count; j++) {
				print ("Object Name: " + bankInventoryList [j].name + " Amount: " + bankInventoryList [j].amount);
			}
		}

		return bankInventoryList;
	}

	public void saveBankInventoryList (persistanceInventoryListByPlayerInfo bankInventoryListToSave, int currentSaveNumber)
	{
		currentSaveDataPath = getDataPath ();

		bool saveLocated = false;
		bool bankLoacated = false;

		int saveSlotIndex = -1;
		int inventoryListBankIndex = -1;

		persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		List<persistanceInventoryListBySaveSlotInfo> inventoryBankSaveList = new List<persistanceInventoryListBySaveSlotInfo> ();
		if (File.Exists (currentSaveDataPath + saveBankInventoryFileName + " " + versionNumber + fileExtension)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath + saveBankInventoryFileName + " " + versionNumber + fileExtension, FileMode.Open);
			object currentData = bf.Deserialize (file);
			inventoryBankSaveList = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		for (int j = 0; j < inventoryBankSaveList.Count; j++) {
			if (inventoryBankSaveList [j].saveNumber == currentSaveNumber) {
				newPersistanceInventoryListBySaveSlotInfo = inventoryBankSaveList [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (showDebugLoadedBanKInventoryInfo) {
			print ("Number of objects: " + bankInventoryListToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
		}

		if (saveLocated) {
			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				bankLoacated = true;
				inventoryListBankIndex = 0;
			}
		}

		//if the save is located, check if the bank exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (bankLoacated) {
				inventoryBankSaveList [saveSlotIndex].playerInventoryList [inventoryListBankIndex].inventoryObjectList = bankInventoryListToSave.inventoryObjectList;
			} else {
				inventoryBankSaveList [saveSlotIndex].playerInventoryList.Add (bankInventoryListToSave);
			}
		} else {
			newPersistanceInventoryListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Add (bankInventoryListToSave);
			inventoryBankSaveList.Add (newPersistanceInventoryListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Create (currentSaveDataPath + saveBankInventoryFileName + " " + versionNumber + fileExtension); 
		bf.Serialize (file, inventoryBankSaveList);
		file.Close ();
	}

	public int getLastSaveNumber ()
	{
		return lastSaveNumber;
	}

	public Camera getMainCamera ()
	{
		return mainCamera;
	}

	public void setMainCamera (Camera cameraToConfigure)
	{
		mainCamera = cameraToConfigure;
	}

	public void setGamePauseState (bool state)
	{
		gamePaused = state;
	}

	public bool isGamePaused ()
	{
		return gamePaused;
	}

	public bool isUsingTouchControls ()
	{
		return useTouchControls;
	}

	public void setUseTouchControlsState (bool state)
	{
		useTouchControls = state;
	}

	public void setCharactersManagerPauseState (bool state)
	{
		charactersManager.setCharactersManagerPauseState (state);
	}

	public string getSaveCaptureFolder ()
	{
		return saveCaptureFolder;
	}

	public string getSaveCaptureFileName ()
	{
		return saveCaptureFileName;
	}

	public string getTouchControlsPositionFolderName ()
	{
		return touchControlsPositionFolderName;
	}

	public string getTouchControlsPositionFileName ()
	{
		return touchControlsPositionFileName + " " + versionNumber;
	}

	public void setCurrentPersistentDataPath ()
	{
		currentPersistentDataPath = Application.persistentDataPath;

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
}
