using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class menuPause : MonoBehaviour
{
	public bool menuPauseEnabled;
	public bool changeControlsEnabled;

	public bool fadeScreenActive = true;
	public float fadeScreenSpeed = 2;

	public GameObject hudAndMenus;
	public GameObject touchPanel;
	public GameObject pauseMenu;
	public Transform pauseMenuParent;
	public GameObject dieMenu;
	public Scrollbar accelerometerSwitchScrollbar;
	public Image blackBottomImage;

	public Text rightJoystickSensitivityText;
	public Text leftJoystickSensitivityText;
	public Text mouseSensitivityText;

	public List<submenuInfo> submeneInfoList = new List<submenuInfo> ();

	public List<playerMenuInfo> playerMenuInfoList = new List<playerMenuInfo> ();

	public List<ingameMenuInfo> ingameMenuInfoList = new List<ingameMenuInfo> ();

	public bool inputHelpMenuEnabled;
	public GameObject inputHelpMenuGameObject;
	bool inputHelpMenuEnabledState;

	public bool mechanicsHelpMenuEnabled;
	public GameObject mechanicsHelpMenuGameObject;
	bool mechanicsHelpMenuEnabledState;

	public bool closeOnlySubMenuIfOpenOnEscape;

	public bool useBlurUIPanelOnPlayerMenu;
	public bool useBlurUIPanelOnPause;
	public GameObject blurUIPanel;
	public Material blurUIPanelMaterial;
	public Image blurUIPanelImage;
	public float blurUIPanelValue = 4.4f;
	public float blurUIPanelSpeed = 10;
	public float blurUIPanelAlphaSpeed = 10;
	public float blurUIPanelAlphaValue = 120;
	public Transform blurUIPanelParent;

	public gameManager mainGameManager;
	public playerInputManager playerInput;
	public mapSystem mapManager;
	public vehicleHUDInfo HUDInfoManager;
	public showGameInfoHud gameInfoHudManager;
	public Canvas mainCanvas;
	public RectTransform mainCanvasRectTransform;
	public CanvasScaler mainCanvasScaler;
	public Camera mainCanvasCamera;
	public inputManager input;
	public playerTutorialSystem mainPlayerTutorialSystem;
	public timeBullet timeManager;
	public playerWeaponsManager mainWeaponsManager;
	public playerHealthBarManagementSystem mainPlayerHealthBarManager;
	public playerScreenObjectivesSystem playerScreenObjectivesManager;
	public playerPickupIconManager mainPlayerPickupIconManager;
	public mouseCursorController mouseCursorControllerManager;
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;
	public editControlPosition editControlPositionManager;
	public inventoryManager playerInventoryManager;

	public cursorStateInfo cursorState;

	public int currentPlayerMenuInfoIndex;
	public bool gamePaused = false;

	public bool subMenuActive;
	public bool usingSubMenu;

	public bool playerMenuActive;

	public bool useTouchControls = false;
	public bool usingTouchControlsPreviously;

	public bool usingDevice;
	bool usingPointAndClick;
	bool showGUI = false;
	bool dead;

	Color alpha;

	Coroutine blurUIPanelCoroutine;
	bool blurUIPanelActive;

	void Start ()
	{
		AudioListener.pause = false;

		blackBottomImage.enabled = true;

		if (!mainGameManager.isUsingTouchControls ()) {
			showOrHideCursor (false);
		} else {
			enableOrDisableTouchControls (true);
		}
			
		editControlPositionManager.getTouchButtons (useTouchControls);

		alpha.a = 1;
		Time.timeScale = 1;

		//if the fade of the screen is disabled, just set the alpha of the black panel to 0
		if (!fadeScreenActive) {
			alpha.a = 0;
			blackBottomImage.color = alpha;
			blackBottomImage.enabled = false;
		}

		pauseMenu.SetActive (false);

		disableMenuList ();

		setCurrentCameraState (true);

		//if the accelerometer is disabled, set the value in the menu to disabled
		if (!playerCameraManager.settings.useAcelerometer) {
			accelerometerSwitchScrollbar.value = 0;
		}
			
		blurUIPanelMaterial.SetFloat ("_Size", 0); 
	}

	void Update ()
	{
		//if the fade is enabled, decrease the value of alpha to get a nice fading effect at the beginning of the game
		if (fadeScreenActive) {
			alpha.a -= Time.deltaTime / fadeScreenSpeed;
			blackBottomImage.color = alpha;
			if (alpha.a <= 0) {
				blackBottomImage.enabled = false;
				fadeScreenActive = false;
			}
		}

		//if the mouse is showed, press in the screen to lock it again
		if (!gamePaused) {
			//check that the touch controls are disabled, the player is not dead, the powers is not being editing or selecting, the player is not using a device
			//or the cursor is visible
			if ((Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) && !useTouchControls && !dead && inGameWindowOpened ()
			    && (Cursor.lockState == CursorLockMode.None || Cursor.visible)) {
				showOrHideCursor (false);
			}
		}

		if (inputHelpMenuEnabled) {
			if (Input.GetKeyDown (KeyCode.F1)) {
				if (inputHelpMenuGameObject != null) {
					inputHelpMenuEnabledState = !inputHelpMenuEnabledState;
					inputHelpMenuGameObject.SetActive (inputHelpMenuEnabledState);
				}
			}
		}

		if (mechanicsHelpMenuEnabled) {
			if (Input.GetKeyDown (KeyCode.F2)) {
				if (mechanicsHelpMenuGameObject != null) {
					mechanicsHelpMenuEnabledState = !mechanicsHelpMenuEnabledState;
					mechanicsHelpMenuGameObject.SetActive (mechanicsHelpMenuEnabledState);
				}
			}
		}
	}

	void OnDisable ()
	{
		if (blurUIPanelActive) {
			if (blurUIPanelMaterial) {
				blurUIPanelMaterial.SetFloat ("_Size", 0); 
			}
		}
	}

	public void disableMenuList ()
	{
		for (int i = 0; i < submeneInfoList.Count; i++) {	
			submeneInfoList [i].menuGameObject.SetActive (false);
			if (submeneInfoList [i].useEventOnClose) {
				if (submeneInfoList [i].menuEvent.GetPersistentEventCount () > 0) {
					submeneInfoList [i].menuEvent.Invoke ();
				}
			}
		}
	}

	//save the previous and the current visibility of the cursor, to enable the mouse cursor correctly when the user enables the touch controls, or using a device
	//or editing the powers, or open the menu, or any action that enable and disable the mouse cursor
	void setCurrentCursorState (bool curVisible)
	{
		cursorState.currentVisible = curVisible;
	}

	void setPreviousCursorState (bool prevVisible)
	{
		cursorState.previousVisible = prevVisible;
	}

	//like the mouse, save the state of the camera, to prevent rotate it when a menu is enabled, or using a device, or the player is dead, etc...
	void setCurrentCameraState (bool curCamera)
	{
		cursorState.currentCameraEnabled = curCamera;
	}

	void setPreviousCameraState (bool prevCamera)
	{
		cursorState.previousCameraEnabled = prevCamera;
	}

	public bool inGameWindowOpened ()
	{
		if ((!usingDevice || (playerControllerManager.isPlayerDriving () && !playerMenuActive)) && !usingPointAndClick) {

			for (int i = 0; i < ingameMenuInfoList.Count; i++) {
				if (ingameMenuInfoList [i].menuOpened) {
					return false;
				}
			}

			return true;
		}

		return false;
	}

	public void setIngameMenuOpenedState (string ingameMenuName, bool state)
	{
		for (int i = 0; i < ingameMenuInfoList.Count; i++) {
			if (ingameMenuInfoList [i].Name == ingameMenuName) {
				ingameMenuInfoList [i].menuOpened = state;
			}
		}

		for (int i = 0; i < playerMenuInfoList.Count; i++) {
			if (playerMenuInfoList [i].Name == ingameMenuName) {
				currentPlayerMenuInfoIndex = i;
				return;
			}
		}
	}

	//get the scroller value in the touch options menu if the player enables or disables the accelerometer
	public void getAccelerometerScrollerValue (Scrollbar info)
	{
		if (info.value < 0.5f) {
			playerCameraManager.setUseAcelerometerState (false);
			if (info.value > 0) {
				info.value = 0;
			}
		} else {
			playerCameraManager.setUseAcelerometerState (true);
			if (info.value < 1) {
				info.value = 1;
			}
		}
	}

	public void getAimAssistScrollerValue (Scrollbar info)
	{
		if (info.value < 0.5f) {
			playerCameraManager.setLookAtTargetEnabledState (false);
			if (info.value > 0) {
				info.value = 0;
			}
		} else {
			playerCameraManager.setLookAtTargetEnabledState (true);
			if (info.value < 1) {
				info.value = 1;
			}
		}
	}

	//set the configuration of both joysticks in the touch options menu, so the joysticks can be configured ingame
	public void getToggleJoysticksValue (Toggle info)
	{
		string name = info.name;
		switch (name) {
		case "leftSnap":
			playerInput.touchMovementControl.snapsToFinger = info.isOn;
			break;
		case "leftHide":
			playerInput.touchMovementControl.hideOnRelease = info.isOn;
			break;
		case "leftPad":
			playerInput.touchMovementControl.touchPad = info.isOn;
			break;
		case "leftShow":
			playerInput.touchMovementControl.showJoystick = info.isOn;
			break;
		case "rightSnap":
			playerInput.touchCameraControl.snapsToFinger = info.isOn;
			break;
		case "rightHide":
			playerInput.touchCameraControl.hideOnRelease = info.isOn;
			break;
		case "rightPad":
			playerInput.touchCameraControl.touchPad = info.isOn;
			break;
		case "rightShow":
			playerInput.touchCameraControl.showJoystick = info.isOn;
			break;
		}
	}

	//get the values from the touch joystick sensitivity in the touch options menu when the player adjust the joysticks sensitivity
	public void getRightSensitivityValue (Slider info)
	{
		rightJoystickSensitivityText.text = info.value.ToString ("0.#");

		//set the values in the input manager
		playerInput.rightTouchSensitivity = info.value;
	}

	public void getLeftSensitivityValue (Slider info)
	{
		leftJoystickSensitivityText.text = info.value.ToString ("0.#");

		//set the values in the input manager
		playerInput.leftTouchSensitivity = info.value;
	}

	//get the mouse sensitivity value when the player adjusts it in the edit input menu
	public void getMouseSensitivityValue (Slider info)
	{
		mouseSensitivityText.text = info.value.ToString ("0.#");

		//set the values in the input manager
		playerInput.mouseSensitivity = info.value;
	}

	//set in the player is using a device like a computer or a text device
	public void usingDeviceState (bool state)
	{
		usingDevice = state;
	}

	public void usingPointAndClickState (bool state)
	{
		usingPointAndClick = state;
	}

	public void usingSubMenuState (bool state)
	{
		usingSubMenu = state;
	}

	public void pauseGame ()
	{
		if (!subMenuActive) {
			//if the main pause menu is the current place, resuem the game

			pause ();

			return;
		} else {
			//else, the current menu place is a submenu, so disable all the submenus and set the main menu window
			showGUI = true;
			subMenuActive = false;

			pauseMenu.SetActive (true);

			disableMenuList ();
		}
		//disable the edition of the touch button position if the player backs from that menu option
		editControlPositionManager.disableEdit ();
	}

	bool pauseCalledFromGameManager;

	public void setMenuPausedState (bool state)
	{
		if (transform.parent.gameObject.activeSelf) {
			gamePaused = !state;
		
			pauseCalledFromGameManager = true;

			pause ();
		}
	}

	public void pause ()
	{
		//check if the game is going to be paused or resumed
		if (!dead && menuPauseEnabled) {
			//if the player pauses the game and he is editing the powers or selecting them, disable the power manager menu

			bool playerMenuActivePreviously = playerMenuActive;

			for (int i = 0; i < ingameMenuInfoList.Count; i++) {
				if (ingameMenuInfoList [i].menuOpened) {
					ingameMenuInfoList [i].closeMenuEvent.Invoke ();
				}
			}

			if (playerMenuActivePreviously && closeOnlySubMenuIfOpenOnEscape) {
				return;
			}

			gamePaused = !gamePaused;

			mainGameManager.setGamePauseState (gamePaused);

			if (!pauseCalledFromGameManager) {
				mainGameManager.setCharactersManagerPauseState (gamePaused);
			}

			showGUI = !showGUI;
			AudioListener.pause = gamePaused;

			//enable or disable the main pause menu
			pauseMenu.SetActive (gamePaused);

			//change the camera state
			changeCameraState (!gamePaused);

			//check if the touch controls were enabled
			if (!useTouchControls) {
				showOrHideCursor (gamePaused);
			}

			input.setPauseState (gamePaused);

			//pause game
			if (gamePaused) {
				timeManager.disableTimeBullet ();
				Time.timeScale = 0;
				alpha.a = 0.5f;
				//fade a little to black an UI panel
				blackBottomImage.enabled = true;
				blackBottomImage.color = alpha;
				//disable the event triggers in the touch buttons
				editControlPositionManager.changeButtonsState (false);
			}

			//resume game
			if (!gamePaused) {
				Time.timeScale = 1;	 	
				alpha.a = 0;
				//fade to transparent the UI panel
				blackBottomImage.enabled = false;
				blackBottomImage.color = alpha;
				//enable the event triggers in the touch buttons
				editControlPositionManager.changeButtonsState (true);
				timeManager.reActivateTime ();
			}
			fadeScreenActive = false;

			if (useBlurUIPanelOnPause) {
				changeBlurUIPanelValue (gamePaused, pauseMenuParent, useBlurUIPanelOnPause);
			}

			playerControllerManager.setGamePausedState (gamePaused);

			mainWeaponsManager.setGamePausedState (gamePaused);

			mouseCursorControllerManager.showOrHideCursor (gamePaused);

			if (!gamePaused) {
				pauseCalledFromGameManager = false;
			}

			//print ("final " + transform.parent.name);
		}
	}

	public void showOrHideMouseCursorController (bool state)
	{
		mouseCursorControllerManager.showOrHideCursor (state);
	}

	//set the state of the cursor, according to if the touch controls are enabled, if the game is pause, if the powers manager menu is enabled, etc...
	//so the cursor is always locked and not visible correctly and vice versa
	public void showOrHideCursor (bool value)
	{
		if (cursorState.currentVisible && cursorState.previousVisible) {
			setPreviousCursorState (false);
			setCurrentCursorState (true);
			return;
		}

		if (cursorState.currentVisible && useTouchControls) {
			setPreviousCursorState (false);
			setCurrentCursorState (true);
			return;
		}

		if (value) {
			Cursor.lockState = CursorLockMode.None;
		} else {
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.lockState = CursorLockMode.Locked;
		}

		setPreviousCursorState (Cursor.visible);
		Cursor.visible = value;
		setCurrentCursorState (Cursor.visible);
	}

	//check if the touch controls have to be enable and disable and change the cursor visibility according to that
	public void checkTouchControls (bool state)
	{
		if (!state) {
			usingTouchControlsPreviously = useTouchControls;
		}

		if (usingTouchControlsPreviously) {
			enableOrDisableTouchControls (state);

			if (state && usingTouchControlsPreviously) {
				showOrHideCursor (true);
			}
		}
	}

	//the player dies, so enable the death menu to ask the player to play again
	public void death ()
	{
		dead = true;
		showOrHideCursor (true);
		dieMenu.SetActive (true);
		changeCameraState (false);
	}

	//the player chooses to play again
	public void getUp ()
	{
		dead = false;
		if (!useTouchControls) {
			showOrHideCursor (false);
		}
		changeCameraState (true);
	}

	//restart the scene
	public void restart ()
	{
		pause ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	//change the camera state according to if the player pauses the game or uses a device, etc... so the camera is enabled correctly according to every situation
	public void changeCameraState (bool state)
	{
		if (playerCameraManager) {
			//if the player paused the game using a device, then resume again with the camera disable to keep using that device
			if (!cursorState.currentCameraEnabled && !cursorState.previousCameraEnabled) {
				setPreviousCameraState (true);
			}
			//else save the current and previous state of the camera and set the state of the camera according to the current situation
			else {
				setPreviousCameraState (playerCameraManager.cameraCanBeUsed);
				playerCameraManager.pauseOrPlayCamera (state);
				setCurrentCameraState (playerCameraManager.cameraCanBeUsed);
			}
		}
	}

	public void changeCursorState (bool state)
	{
		if (playerCameraManager) {
			//if the player paused the game using a device, then resume again with the camera disable to keep using that device
			if (!cursorState.currentCameraEnabled && !cursorState.previousCameraEnabled) {
				setPreviousCameraState (true);
			}
			//else save the current and previous state of the camera and set the state of the camera according to the current situation
			else {
				setPreviousCameraState (state);
				setCurrentCameraState (!state);
			}
		}
	}

	public void openOrClosePlayerMenu (bool state, Transform blurUIParent, bool useBlurUIPanel)
	{
		playerMenuActive = state;
		playerControllerManager.setLastTimeMoved ();

		if (useBlurUIPanelOnPlayerMenu && useBlurUIPanel) {
			changeBlurUIPanelValue (playerMenuActive, blurUIParent, useBlurUIPanel);
		}
	}

	public void openPlayerMenuWithBlurPanel (Transform blurUIParent)
	{
		playerControllerManager.setLastTimeMoved ();

		if (useBlurUIPanelOnPlayerMenu) {
			changeBlurUIPanelValue (playerMenuActive, blurUIParent, true);
		}
	}


	public void setHeadBobPausedState (bool state)
	{
		playerControllerManager.setHeadBobPausedState (state);
	}

	//the player is in a submenu, so disable the main menu
	public void enterSubMenu ()
	{
		showGUI = false;
		subMenuActive = true;
	}

	//the player backs from a submenu, so enable the main menu
	public void exitSubMenu ()
	{
		showGUI = true;
		subMenuActive = false;
	}

	//switch between touch controls and the keyboard
	public void switchControls ()
	{
		useTouchControls = !useTouchControls;

		enableOrDisableTouchControls (useTouchControls);

		pause ();

		mainGameManager.setUseTouchControlsState (useTouchControls);

		input.setUseTouchControlsState (useTouchControls);
	}

	public void setUseTouchControlsState (bool state)
	{
		useTouchControls = state;
	}

	public bool isUsingTouchControls ()
	{
		return useTouchControls;
	}

	//exit from the game
	public void confirmExit ()
	{
		Application.Quit ();
	}

	public void confirmGoToHomeMenu ()
	{
		SceneManager.LoadScene (0);
	}

	//enable or disable the joysticks and the touch buttons in the HUD
	public void enableOrDisableTouchControls (bool state)
	{
		input.setKeyboardControls (!state);
	}

	public void reloadStart ()
	{
		Start ();
	}

	public void resetPauseMenuBlurPanel ()
	{
		changeBlurUIPanelValue (true, pauseMenuParent, true);
	}

	public void changeBlurUIPanelValue (bool state, Transform blurUIParent, bool useBlurUIPanel)
	{
		if (!useBlurUIPanel) {
			return;
		}

		if (state) {
			if (blurUIParent) {
				blurUIPanel.transform.SetParent (blurUIParent);
				blurUIPanel.transform.SetSiblingIndex (0);
			}
		} else {
			blurUIPanel.transform.SetParent (blurUIPanelParent);
		}

		if (blurUIPanelCoroutine != null) {
			StopCoroutine (blurUIPanelCoroutine);
		}
		blurUIPanelCoroutine = StartCoroutine (changeBlurUIPanelValueCoroutine (state));
	}

	IEnumerator changeBlurUIPanelValueCoroutine (bool state)
	{
		blurUIPanelActive = state;
		if (blurUIPanelActive) {
			blurUIPanel.SetActive (true);
		}

		float blurUIPanelValueTarget = 0;
		float blurUIPanelAlphaTarget = 0;

		if (blurUIPanelActive) {
			blurUIPanelValueTarget = blurUIPanelValue;
			blurUIPanelAlphaTarget = blurUIPanelAlphaValue;
		}

		float currentBlurUiPanelValue = blurUIPanelMaterial.GetFloat ("_Size");
		float currentBlurUIPanelAlphaValue = blurUIPanelImage.color.a * 255;
		Color currentColor = blurUIPanelImage.color;

		if (Time.timeScale > 0) {
			while (currentBlurUiPanelValue != blurUIPanelValueTarget || currentBlurUIPanelAlphaValue != blurUIPanelAlphaTarget) {
				currentBlurUiPanelValue = Mathf.MoveTowards (currentBlurUiPanelValue, blurUIPanelValueTarget, Time.deltaTime * blurUIPanelSpeed);
				blurUIPanelMaterial.SetFloat ("_Size", currentBlurUiPanelValue); 
				currentBlurUIPanelAlphaValue = Mathf.MoveTowards (currentBlurUIPanelAlphaValue, blurUIPanelAlphaTarget, Time.deltaTime * blurUIPanelAlphaSpeed);
				currentColor.a = currentBlurUIPanelAlphaValue / 255;
				blurUIPanelImage.color = currentColor;
				yield return null;
			}
		} else {
			blurUIPanelMaterial.SetFloat ("_Size", blurUIPanelValueTarget);
			currentColor.a = blurUIPanelAlphaTarget / 255;
			blurUIPanelImage.color = currentColor;
			yield return null;
		}

		if (!blurUIPanelActive) {
			blurUIPanelMaterial.SetFloat ("_Size", 0); 
			blurUIPanel.SetActive (false);
		}
	}

	public void enableOrDisableDynamicElementsOnScreen (bool state)
	{
		pauseOrResumeShowHealthSliders (!state);
		pauseOrResumeShowObjectives (!state);
		pauseOrResumeShowIcons (!state);
	}

	public void pauseOrResumeShowHealthSliders (bool state)
	{
		mainPlayerHealthBarManager.pauseOrResumeShowHealthSliders (state);
	}

	public void pauseOrResumeShowObjectives (bool state)
	{
		playerScreenObjectivesManager.pauseOrResumeShowObjectives (state);
	}

	public void pauseOrResumeShowIcons (bool state)
	{
		mainPlayerPickupIconManager.pauseOrResumeShowIcons (state);
	}

	public void inputPauseGame ()
	{
		pauseGame ();
	}

	public void openOrClosePlayerMenu ()
	{
		if (!playerMenuActive) {

			bool indexFound = false;

			for (int i = 0; i < playerMenuInfoList.Count; i++) {
				if (!indexFound && playerMenuInfoList [i].menuCanBeUsed) {
					currentPlayerMenuInfoIndex = i;

					indexFound = true;
				}
			}
		}

		playerMenuInfoList [currentPlayerMenuInfoIndex].currentMenuEvent.Invoke ();
	}

	public void openNextPlayerOpenMenu ()
	{
		if (!playerMenuActive) {
			return;
		}

		playerMenuInfoList [currentPlayerMenuInfoIndex].currentMenuEvent.Invoke ();

		bool exit = false;
		int max = 0;
		while (!exit) {
			max++;
			if (max > 100) {
				return;
			}

			currentPlayerMenuInfoIndex++;
			if (currentPlayerMenuInfoIndex > playerMenuInfoList.Count - 1) {
				currentPlayerMenuInfoIndex = 0;
			}

			if (playerMenuInfoList [currentPlayerMenuInfoIndex].menuCanBeUsed) {
				exit = true;
			}
		}

		print (currentPlayerMenuInfoIndex);

		playerMenuInfoList [currentPlayerMenuInfoIndex].currentMenuEvent.Invoke ();
	}

	public void openPreviousPlayerOpenMenu ()
	{
		if (!playerMenuActive) {
			return;
		}

		playerMenuInfoList [currentPlayerMenuInfoIndex].currentMenuEvent.Invoke ();

		bool exit = false;
		int max = 0;
		while (!exit) {
			max++;
			if (max > 100) {
				return;
			}

			currentPlayerMenuInfoIndex--;
			if (currentPlayerMenuInfoIndex < 0) {
				currentPlayerMenuInfoIndex = playerMenuInfoList.Count - 1;
			}

			if (playerMenuInfoList [currentPlayerMenuInfoIndex].menuCanBeUsed) {
				exit = true;
			}
		}

		print (currentPlayerMenuInfoIndex);

		playerMenuInfoList [currentPlayerMenuInfoIndex].currentMenuEvent.Invoke ();
	}

	public void enableOrDisablePlayerHUD (bool state)
	{
		HUDInfoManager.playerHUD.SetActive (state);

		if (mapManager.mapEnabled) {
			mapManager.enableOrDisableMiniMap (state);
		}

		if (mapManager.compassEnabled) {
			mapManager.enableOrDisableCompass (state);
		}

		if (mainWeaponsManager.isWeaponsHUDActive ()) {
			mainWeaponsManager.weaponsHUD.SetActive (state);
		}

		playerInventoryManager.enableOrDisableWeaponSlotsParentOutOfInventory (state);
	}

	public void enableOrDisableWeaponSlotsParentOutOfInventory (bool state)
	{
		playerInventoryManager.enableOrDisableWeaponSlotsParentOutOfInventory (state);
	}

	public mapSystem getMapSystem ()
	{
		return mapManager;
	}

	public playerTutorialSystem getPlayerTutorialSystem ()
	{
		return mainPlayerTutorialSystem;
	}

	public vehicleHUDInfo getHUDInfoManager ()
	{
		return HUDInfoManager;
	}

	public showGameInfoHud getGameInfoHudManager ()
	{
		return gameInfoHudManager;
	}

	public Vector2 getMainCanvasSizeDelta ()
	{
		return mainCanvasRectTransform.sizeDelta;
	}

	public Canvas getMainCanvas ()
	{
		return mainCanvas;
	}

	public CanvasScaler getMainCanvasScaler ()
	{
		return mainCanvasScaler;
	}

	public Camera getMainCanvasCamera ()
	{
		return mainCanvasCamera;
	}

	public bool isMenuPaused ()
	{
		return gamePaused;
	}

	public void setHorizontalMovementOrientationValue (Scrollbar scrollbarToCheck)
	{
		if (scrollbarToCheck.value < 0.5f) {
			playerInput.setHorizontalMovementOrientationValue (-1);
			if (scrollbarToCheck.value > 0) {
				scrollbarToCheck.value = 0;
			}
		} else {
			playerInput.setHorizontalMovementOrientationValue (1);
			if (scrollbarToCheck.value < 1) {
				scrollbarToCheck.value = 1;
			}
		}
	}

	public void setVerticalMovementOrientationValue (Scrollbar scrollbarToCheck)
	{
		if (scrollbarToCheck.value < 0.5f) {
			playerInput.setVerticalMovementOrientationValue (-1);
			if (scrollbarToCheck.value > 0) {
				scrollbarToCheck.value = 0;
			}
		} else {
			playerInput.setVerticalMovementOrientationValue (1);
			if (scrollbarToCheck.value < 1) {
				scrollbarToCheck.value = 1;
			}
		}
	}

	public void setHorizontalCameraOrientationValue (Scrollbar scrollbarToCheck)
	{
		if (scrollbarToCheck.value < 0.5f) {
			playerInput.setHorizontalCameraOrientationValue (-1);
			if (scrollbarToCheck.value > 0) {
				scrollbarToCheck.value = 0;
			}
		} else {
			playerInput.setHorizontalCameraOrientationValue (1);
			if (scrollbarToCheck.value < 1) {
				scrollbarToCheck.value = 1;
			}
		}
	}

	public void setVerticalCameratOrientationValue (Scrollbar scrollbarToCheck)
	{
		if (scrollbarToCheck.value < 0.5f) {
			playerInput.setVerticalCameratOrientationValue (-1);
			if (scrollbarToCheck.value > 0) {
				scrollbarToCheck.value = 0;
			}
		} else {
			playerInput.setVerticalCameratOrientationValue (1);
			if (scrollbarToCheck.value < 1) {
				scrollbarToCheck.value = 1;
			}
		}
	}
		
	//a class to save the current and previous state of the mouse visibility and the state of the camera, to enable and disable them correctly according to every
	//type of situation
	[System.Serializable]
	public class cursorStateInfo
	{
		public bool currentVisible;
		public bool previousVisible;
		public bool currentCameraEnabled;
		public bool previousCameraEnabled;
	}

	[System.Serializable]
	public class submenuInfo
	{
		public string Name;
		public GameObject menuGameObject;
		public bool useEventOnClose;
		public UnityEvent menuEvent;
	}

	[System.Serializable]
	public class playerMenuInfo
	{
		public string Name;
		public bool menuCanBeUsed = true;
		public UnityEvent currentMenuEvent;
	}

	[System.Serializable]
	public class ingameMenuInfo
	{
		public string Name;
		public bool menuOpened;
		public UnityEvent closeMenuEvent;
	}
}