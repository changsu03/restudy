using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class playerCharactersManager : MonoBehaviour
{
	public bool searchPlayersAtStart;

	public int currentNumberOfPlayers = 1;

	public GameObject mainCharacter;

	public List<playerInfo> playerList = new List<playerInfo> ();

	public string cameraStateNameToUse = "1 Player";

	public List<cameraStates> cameraStatesList = new List<cameraStates> ();

	public List<GameObject> extraCharacterList = new List<GameObject> ();

	public Vector3 newPlayerPositionOffset;

	public Vector2 regularReferenceResolution = new Vector2 (1280, 720);
	public Vector2 splitReferenceResolution = new Vector2 (2560, 1440);

	Vector3 currentCharacterPosition;

	public List<GameObject> auxPlayerList = new List<GameObject> ();

	public playerInfo currentPlayeraInfoActive;

	public string[] cameraStatesListString;
	public int currentCameraStateIndex;

	void Awake ()
	{
		if (searchPlayersAtStart) {
			searchPlayersOnTheLevel (false);
		}

		currentPlayeraInfoActive = playerList [0];
	}

	public void searchPlayersOnTheLevel (bool creatingNewPlayers)
	{
		playerList.Clear ();
		auxPlayerList.Clear ();

		currentCharacterPosition = Vector3.zero;

		Component[] childrens = GetComponentsInChildren (typeof(playerController));
		foreach (Component c in childrens) {
			auxPlayerList.Add (c.gameObject);
		}

		for (int i = 0; i < auxPlayerList.Count; i++) {
			playerInfo newPlayerInfo = new playerInfo ();

			newPlayerInfo.Name = "Player " + (i + 1).ToString ();
			newPlayerInfo.playerControllerGameObject = auxPlayerList [i];
			newPlayerInfo.playerControllerManager = newPlayerInfo.playerControllerGameObject.GetComponent<playerController> ();

			newPlayerInfo.playerControllerManager.setPlayerID (i + 1);

			newPlayerInfo.playerCameraGameObject = newPlayerInfo.playerControllerManager.getPlayerCameraGameObject ();
			newPlayerInfo.playerCameraManager = newPlayerInfo.playerCameraGameObject.GetComponent<playerCamera> ();
			newPlayerInfo.playerInput = newPlayerInfo.playerControllerGameObject.GetComponent<playerInputManager> ();
			newPlayerInfo.pauseManager = newPlayerInfo.playerInput.getPauseManager ();
			newPlayerInfo.playerParentGameObject = newPlayerInfo.pauseManager.transform.parent.gameObject;
			newPlayerInfo.playerParentGameObject.name = newPlayerInfo.Name;

			if (i == 0) {
				currentCharacterPosition = newPlayerInfo.playerControllerGameObject.transform.position;
			}

			if (creatingNewPlayers) {
				if (newPlayerInfo.playerParentGameObject != mainCharacter) {
					currentCharacterPosition += newPlayerPositionOffset;
					newPlayerInfo.playerControllerGameObject.transform.position = currentCharacterPosition;
					newPlayerInfo.playerCameraGameObject.transform.position = currentCharacterPosition;
				}
			}

			playerList.Add (newPlayerInfo);
		}

		updateComponent ();
	}

	public void setExtraCharacterList ()
	{
		extraCharacterList.Clear ();
		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerParentGameObject != mainCharacter) {

				extraCharacterList.Add (playerList [i].playerParentGameObject);
			}
		}
		updateComponent ();
	}

	public void setPlayerID ()
	{
		int numberOfPlayerID = 0;

		for (int i = 0; i < playerList.Count; i++) {
			playerInfo currentPlayerInfo = playerList [i];

			if (currentPlayerInfo.playerInput.useOnlyKeyboard) {
				numberOfPlayerID--;
			}
				
			currentPlayerInfo.playerControllerManager.setPlayerID (i + 1 + numberOfPlayerID);

			#if UNITY_EDITOR
			EditorUtility.SetDirty (currentPlayerInfo.playerControllerManager);
			#endif
		}

		updateComponent ();
	}

	public void setNumberOfPlayers ()
	{
		int newNumberOfPlayers = currentNumberOfPlayers - 1 - extraCharacterList.Count;

		if (newNumberOfPlayers > 0) {
			for (int i = 0; i < newNumberOfPlayers; i++) {
				GameObject newCharacter = (GameObject)Instantiate (mainCharacter, mainCharacter.transform.position, mainCharacter.transform.rotation);
				newCharacter.name = "Player " + (i + 2).ToString ();
				newCharacter.transform.SetParent (transform);

				extraCharacterList.Add (newCharacter);
			}
		} else {
			newNumberOfPlayers = Mathf.Abs (newNumberOfPlayers);

			print (newNumberOfPlayers);

			newNumberOfPlayers = extraCharacterList.Count - newNumberOfPlayers;

			for (int i = extraCharacterList.Count - 1; i >= newNumberOfPlayers; i--) {
				DestroyImmediate (extraCharacterList [i]);
			}

			for (int i = 0; i < extraCharacterList.Count; i++) {
				if (extraCharacterList [i] == null) {
					extraCharacterList.RemoveAt (i);
					i = 0;
				}
			}

			if (currentNumberOfPlayers == 1) {
				extraCharacterList.Clear ();
			}
		}

		searchPlayersOnTheLevel (true);

		updateComponent ();
	}

	public void setCameraConfiguration ()
	{
		for (int i = 0; i < playerList.Count; i++) {

			List<Camera> currentCameraList = playerList [i].playerCameraManager.getCameraList ();

			cameraStates currentCameraStates = getCameraStateByName (cameraStateNameToUse);
				
			if (currentCameraStates != null && currentCameraStates.cameraInfoList.Count >= currentNumberOfPlayers) {
				for (int j = 0; j < currentCameraList.Count; j++) {
			
					currentCameraList [j].rect = new Rect (currentCameraStates.cameraInfoList [i].newX, currentCameraStates.cameraInfoList [i].newY, 
						currentCameraStates.cameraInfoList [i].newW, currentCameraStates.cameraInfoList [i].newH);

					#if UNITY_EDITOR
					EditorUtility.SetDirty (currentCameraList [j]);
					#endif
				}

				if (currentNumberOfPlayers > 1) {
					playerList [i].pauseManager.getMainCanvasCamera ().gameObject.SetActive (true);
					playerList [i].pauseManager.getMainCanvasScaler ().referenceResolution = splitReferenceResolution;
					playerList [i].pauseManager.getMainCanvas ().renderMode = RenderMode.ScreenSpaceCamera;
				} else {
					playerList [i].pauseManager.getMainCanvasCamera ().gameObject.SetActive (false);
					playerList [i].pauseManager.getMainCanvasScaler ().referenceResolution = regularReferenceResolution;
					playerList [i].pauseManager.getMainCanvas ().renderMode = RenderMode.ScreenSpaceOverlay;
				}

				#if UNITY_EDITOR
				EditorUtility.SetDirty (playerList [i].pauseManager.getMainCanvas ());
				EditorUtility.SetDirty (playerList [i].pauseManager.getMainCanvasScaler ());
				#endif
			} else {
				print ("WARNING: Camera state called " + cameraStateNameToUse + " doesn't assign");
			}
		}
	}

	public void assignMapSystem ()
	{
		for (int i = 0; i < playerList.Count; i++) {

			mapCreator mapCreatorManager = FindObjectOfType<mapCreator> ();

			if (mapCreatorManager) {
				mapCreatorManager.addNewPlayer (playerList [i].pauseManager.gameObject.GetComponent<mapSystem> (), playerList [i].playerControllerGameObject);
			} else {
				print ("WARNING: There is no map creator in the scene, make sure to drag and drop the prefab and configure as you need if you want to use the map system");
			}
		}
	}

	public cameraStates getCameraStateByName (string cameraStateName)
	{
		for (int j = 0; j < cameraStatesList.Count; j++) {
			if (cameraStatesList [j].Name == cameraStateName) {
				return cameraStatesList [j];
			}
		}
		return null;
	}

	public void setAsCurrentPlayerActive (string playerName)
	{
		currentPlayeraInfoActive.playerCameraGameObject.transform.SetParent (currentPlayeraInfoActive.pauseManager.transform);
		currentPlayeraInfoActive.playerControllerGameObject.transform.SetParent (currentPlayeraInfoActive.pauseManager.transform);

		currentPlayeraInfoActive.playerParentGameObject.SetActive (false);

		Vector3 playerPosition = currentPlayeraInfoActive.playerControllerGameObject.transform.position;
		Quaternion playerRotation = currentPlayeraInfoActive.playerControllerGameObject.transform.rotation;

		Quaternion cameraRotation = currentPlayeraInfoActive.playerCameraGameObject.transform.rotation;

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].Name == playerName) {
				currentPlayeraInfoActive = playerList [i];
			}
		}
			
		currentPlayeraInfoActive.playerControllerGameObject.transform.position = playerPosition;
		currentPlayeraInfoActive.playerControllerGameObject.transform.rotation = playerRotation;

		currentPlayeraInfoActive.playerCameraGameObject.transform.rotation = cameraRotation;

		currentPlayeraInfoActive.playerParentGameObject.SetActive (true);
	}

	public menuPause getPauseManagerFromPlayerByIndex (int index)
	{
		return playerList [index].pauseManager;
	}

	public void setCharactersManagerPauseState (bool state)
	{
		for (int i = 0; i < playerList.Count; i++) {
			//print (playerList [i].pauseManager.isMenuPaused ());
			if (playerList [i].pauseManager.isMenuPaused () != state) {
				playerList [i].pauseManager.setMenuPausedState (state);
			}
		}
	}

	public void getCameraStateListString ()
	{
		cameraStatesListString = new string[cameraStatesList.Count];
		for (int i = 0; i < cameraStatesList.Count; i++) {
			string name = cameraStatesList [i].Name;
			cameraStatesListString [i] = name;
		}

		updateComponent ();
	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<playerCharactersManager> ());
		#endif
	}

	[System.Serializable]
	public class playerInfo
	{
		public string Name;
		public GameObject playerParentGameObject;
		public GameObject playerControllerGameObject;
		public GameObject playerCameraGameObject;
		public playerController playerControllerManager;
		public playerCamera playerCameraManager;
	
		public playerInputManager playerInput;
		public menuPause pauseManager;
	}

	[System.Serializable]
	public class cameraStates
	{
		public string Name;
		public int numberfOfPlayers;

		public List<cameraInfo> cameraInfoList = new List<cameraInfo> ();
	}

	[System.Serializable]
	public class cameraInfo
	{
		public float newX = 0;
		public float newY = 0;
		public float newW = 0;
		public float newH = 0;
	}
}
