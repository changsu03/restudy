using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerStatesManager : MonoBehaviour
{
	public Text currentPlayerModeText;
	public menuPause pauseManager;
	public List<playerMode> playersMode = new List<playerMode> ();

	public List<playerControl> playerControlList = new List<playerControl> ();
	public string defaultControlStateName = "Regular Mode";
	public string currentControlStateName;

	public GameObject playerControlModeMenu;
	public RawImage currentPlayerControlModeImage;

	public List<audioSourceInfo> audioSourceList = new List<audioSourceInfo> ();

	public bool openPlayerModeMenuEnabled = true;
	public bool changeModeEnabled = true;
	public bool closeMenuWhenModeSelected = true;

	public bool canSetRegularModeActive = true;

	public bool useBlurUIPanel = true;

	public bool menuOpened;

	public otherPowers powersManager;
	public grabObjects grabManager;
	public scannerSystem scannerManager;
	public playerController playerManager;
	public playerCamera playerCameraManager;
	public gravitySystem gravityManager;
	public playerWeaponsManager weaponsManager;
	public closeCombatSystem combatManager;
	public IKSystem IKSystemManager;
	public usingDevicesSystem usingDevicesManager;
	public overrideElementControlSystem overrideElementManager;
	public headBob headBobManager;
	public damageInScreen damageInScreenManager;
	public jetpackSystem jetpackSystemManager;

	public UnityEvent eventToEnableComponents;
	public UnityEvent eevntToDisableComponents;

	int currentStateIndex;

	void Start ()
	{
		for (int i = 0; i < playersMode.Count; i++) {
			if (playersMode [i].isCurrentState) {
				currentStateIndex = i;
			}
		}

		if (playerControlModeMenu) {
			playerControlModeMenu.SetActive (false);
		}

		setNextPlayerMode ();

		setCurrentControlMode (defaultControlStateName);

	}

	public void openOrCloseControlMode (bool state)
	{
		if ((!pauseManager.playerMenuActive || menuOpened) && ((!playerManager.driving && canSetRegularModeActive && !playerManager.usingDevice) || !canSetRegularModeActive) &&
		    playerManager.checkWeaponsState ()) {
			menuOpened = state;

			if (playerManager.driving) {
				GameObject currentVehicle = playerManager.getCurrentVehicle ();

				if (currentVehicle) {
					currentVehicle.GetComponent<vehicleHUDManager> ().getVehicleCameraController ().pauseOrPlayVehicleCamera (menuOpened);
				}
			}

			pauseManager.openOrClosePlayerMenu (menuOpened, playerControlModeMenu.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState ("Player States Manager", menuOpened);

			playerControlModeMenu.SetActive (state);
			pauseManager.showOrHideCursor (menuOpened);

			//disable the touch controls
			pauseManager.checkTouchControls (!menuOpened);

			//disable the camera rotation
			pauseManager.changeCameraState (!menuOpened);

			pauseManager.usingSubMenuState (menuOpened);

			pauseManager.showOrHideMouseCursorController (menuOpened);
		}
	}

	public void openOrCLoseControlModeMenuFromTouch ()
	{
		openOrCloseControlMode (!menuOpened);
	}

	public void setCurrentControlMode (string controlModeNameToCheck)
	{
		if (currentControlStateName != controlModeNameToCheck) {
			currentControlStateName = controlModeNameToCheck;

			for (int i = 0; i < playerControlList.Count; i++) {
				if (playerControlList [i].Name == controlModeNameToCheck) {
					currentPlayerControlModeImage.texture = playerControlList [i].modeTexture;

					canSetRegularModeActive = !playerControlList [i].avoidToSetRegularModeWhenActive;

					playerControlList [i].activateControlModeEvent.Invoke ();

					playerControlList [i].isCurrentState = true;
				} else {
					playerControlList [i].deactivateControlModeEvent.Invoke ();
					playerControlList [i].isCurrentState = false;
				}
			}

			if (closeMenuWhenModeSelected && pauseManager) {
				openOrCloseControlMode (false);
			}
		}
	}

	public void setNextPlayerMode ()
	{
		if (!powersManager.isAimingPowerInThirdPerson () &&
		    ((!weaponsManager.carryingWeaponInThirdPerson && !weaponsManager.aimingInThirdPerson &&
		    !weaponsManager.carryingWeaponInFirstPerson && !weaponsManager.aimingInFirstPerson) || weaponsManager.drawKeepWeaponWhenModeChanged)) {

			for (int i = 0; i < playersMode.Count; i++) {
				if (i == currentStateIndex) {
					if (currentPlayerModeText) {
						currentPlayerModeText.text = playersMode [i].nameMode;
					}

					playersMode [i].isCurrentState = true;
				} else {
					playersMode [i].isCurrentState = false;
					playersMode [i].deactivatePlayerModeEvent.Invoke ();
				}
			}

			playersMode [currentStateIndex].activatePlayerModeEvent.Invoke ();
		}
	}

	public void setModeIndex ()
	{
		if (changeModeEnabled) {
			currentStateIndex++;
			if (currentStateIndex > playersMode.Count - 1) {
				currentStateIndex = 0;
			}
			setNextPlayerMode ();
		}
	}

	//check every possible state that must not keep enabled when the player is going to make a certain action, like drive
	public void checkPlayerStates (bool disableWeaponsValue, bool disableAimModeValue, bool disableGrabModeValue, bool disableScannerModeValue,
	                               bool resetAnimatorStateValue, bool disableGrvityPowerValue, bool disablePowersValue, bool disablePlayerModesValue)
	{
		//print ("disable some states");
		//disable weapons
		if (disableWeaponsValue) {
			disableWeapons ();
		}

		//disable the aim mode
		if (disableAimModeValue) {
			disableAimMode ();
		}

		//disable the grab mode of one single object
		if (disableGrabModeValue) {
			disableGrabMode ();
		}

		//disable the grab mode when the player is carrying more than one object
		if (disableScannerModeValue) {
			disableScannerMode ();
		}

		//set the iddle state in the animator
		if (resetAnimatorStateValue) {
			resetAnimatorState ();
		}

		//disable the gravity power
		if (disableGrvityPowerValue) {
			disableGravityPower ();
		}

		//disable powers states
		if (disablePowersValue) {
			disablePowers ();
		}

		//set the normal mode for the player, to disable the jetpack and the sphere mode
		if (disablePlayerModesValue) {
			disablePlayerModes ();
		}
	}

	//check every possible state that must not keep enabled when the player is going to make a certain action, like drive
	public void checkPlayerStates ()
	{
		//print ("disable all states");
		//disable weapons
		disableWeapons ();

		//disable the aim mode
		disableAimMode ();

		//disable the grab mode of one single object
		disableGrabMode ();

		//disable the grab mode when the player is carrying more than one object
		disableScannerMode ();

		//set the iddle state in the animator
		resetAnimatorState ();

		//disable the gravity power
		disableGravityPower ();

		//disable powers states
		disablePowers ();

		//set the normal mode for the player, to disable the jetpack and the sphere mode
		disablePlayerModes ();
	}

	public void disableWeapons ()
	{
		weaponsManager.checkIfDisableCurrentWeapon ();
	}

	public void disableAimMode ()
	{
		if (powersManager) {
			if (powersManager.isAimingPowerInThirdPerson ()) {
				powersManager.deactivateAimMode ();
			}
		}
	}

	public void disableGrabMode ()
	{
		if (grabManager) {
			if (grabManager.isGrabbedObject ()) {
				grabManager.dropObject ();
			}
		}
		if (powersManager) {
			if (powersManager.carryingObjects) {
				powersManager.dropObjects ();
			}
		}
	}

	public void disableScannerMode ()
	{
		if (scannerManager) {
			if (scannerManager.isScannerActivated ()) {
				scannerManager.disableScanner ();
			}
		}
	}

	public void resetAnimatorState ()
	{
		if (playerManager.isCrouching ()) {
			playerManager.crouch ();
			playerManager.animator.SetBool ("Crouch", false);
		}
		playerManager.animator.SetFloat ("Jump", 0);
		playerManager.animator.SetFloat ("JumpLeg", 0);
		playerManager.animator.SetFloat ("Turn", 0);
		playerManager.animator.SetFloat ("Forward", 0);
		playerManager.animator.SetBool ("OnGround", true);
	}

	public void disableGravityPower ()
	{
		if (gravityManager.isGravityPowerActive ()) {
			gravityManager.stopGravityPower ();
		}
	}

	public void disablePowers ()
	{
		if (powersManager) {
			if (powersManager.running) {
				powersManager.stopRun ();
			}
			if (powersManager.activatedShield) {
				powersManager.activateShield ();
			}
		}
	}

	public void disablePlayerModes ()
	{
		if (canSetRegularModeActive) {
			setCurrentControlMode (defaultControlStateName);
		}
	}

	public void setAllControllerComponentsState (bool state)
	{
		GetComponent<Collider> ().enabled = state;
		playerManager.enabled = state;
		playerCameraManager.enabled = state;
		gravityManager.enabled = state;
		powersManager.enabled = state;
		GetComponent<health> ().checkIfEnabledStateCanChange (state);
		usingDevicesManager.enabled = state;
		combatManager.enabled = state;
		GetComponent<ragdollActivator> ().enabled = state;
		GetComponent<footStepManager> ().enabled = state;
		IKSystemManager.enabled = state;
		weaponsManager.enabled = state;
		GetComponent<damageInScreen> ().enabled = state;
		GetComponent<upperBodyRotationSystem> ().enabled = state;

		GetComponent<findObjectivesSystem> ().enabled = state;
		GetComponent<AINavMesh> ().enabled = state;

		if (state) {
			eventToEnableComponents.Invoke ();
		} else {
			eevntToDisableComponents.Invoke ();
		}
	}

	public AudioSource getAudioSourceElement (string name)
	{
		for (int i = 0; i < audioSourceList.Count; i++) {
			if (audioSourceList [i].audioSourceName == name) {
				return audioSourceList [i].audioSource;
			}
		}
		return null;
	}

	public void disableVehicleDrivenRemotely ()
	{
		if (playerManager.isDrivingRemotely ()) {
			usingDevicesManager.useDevice ();
		}

		if (playerManager.isOverridingElement ()) {
			if (overrideElementManager) {
				overrideElementManager.stopOverride ();
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputChangeMode ()
	{
		if (!playerManager.driving && ((pauseManager && !pauseManager.usingSubMenu && !pauseManager.playerMenuActive) || !pauseManager)) {
			setModeIndex ();
		}
	}

	public void inputChangePlayerControlMode ()
	{
		if (openPlayerModeMenuEnabled) {
			openOrCloseControlMode (!menuOpened);
		}
	}

	//set in editor mode, without game running, the camera position, starting the game in fisrt person view
	public void setFirstPersonEditor ()
	{
		//check that the player is not in this mode already and that the game is not being played
		if (!playerCameraManager.isFirstPersonActive () && !Application.isPlaying) {
			//set the parameters correctly, so there won't be issues
			gravityManager.setFirstPersonView (true);

			//disable the player's meshes
			gravityManager.setGravityArrowState (false);
			headBobManager.setFirstOrThirdMode (true);

			//put the camera in the correct position
			playerCameraManager.activateFirstPersonCamera ();

			//this is the first person view, so move the camera position directly to the first person view
			playerCameraManager.resetMainCameraTransformLocalPosition ();
			playerCameraManager.resetPivotCameraTransformLocalPosition ();

			//in this mode the player hasn't to aim, so enable the grab objects function
			grabManager.setAimingState (true);
			powersManager.usingPowers = true;

			//change the position where the projectiles are instantiated, in this case a little below the camera
			powersManager.shootsettings.shootZone.transform.SetParent(playerCameraManager.getCameraTransform ());
			powersManager.shootsettings.shootZone.transform.localPosition = -transform.up;
			powersManager.shootsettings.shootZone.transform.localRotation = Quaternion.identity;

			if (damageInScreenManager) {
				damageInScreenManager.pauseOrPlayDamageInScreen (true);
			}

			if (jetpackSystemManager) {
				jetpackSystemManager.enableOrDisableJetPackMesh (false);
			}

			weaponsManager.getPlayerWeaponsManagerComponents (true);
			weaponsManager.setWeaponsParent (true, true);
			headBobManager.setFirstOrThirdMode (true);

			updateCameraComponents ();

			print ("Player's view configured as First Person");
		}
	}

	//set in editor mode, without game running, the camera position, starting the game in third person view
	public void setThirdPersonEditor ()
	{
		//check that the player is not in this mode already and that the game is not being played
		if (playerCameraManager.isFirstPersonActive () && !Application.isPlaying) {
			//set the parameters correctly, so there won't be issues
			gravityManager.setFirstPersonView (false);

			//enable the player's meshes
			gravityManager.setGravityArrowState (true);
			headBobManager.setFirstOrThirdMode (false);

			//put the camera in the correct position
			playerCameraManager.deactivateFirstPersonCamera ();
			playerCameraManager.resetMainCameraTransformLocalPosition ();
			playerCameraManager.resetPivotCameraTransformLocalPosition ();

			//set the changes in grabObjects and other powers
			grabManager.setAimingState (false);
			powersManager.keepPower ();
			powersManager.usingPowers = false;

			//set the position where the projectiles are instantiated, in this case, in the right hand of the player
			powersManager.aimsettings.handActive = powersManager.aimsettings.rightHand;
			powersManager.shootsettings.shootZone.transform.SetParent (powersManager.aimsettings.handActive.transform);
			powersManager.shootsettings.shootZone.transform.localPosition = Vector3.zero;
			powersManager.shootsettings.shootZone.transform.localRotation = Quaternion.identity;

			if (damageInScreenManager) {
				damageInScreenManager.pauseOrPlayDamageInScreen (false);
			}

			if (jetpackSystemManager) {
				jetpackSystemManager.enableOrDisableJetPackMesh (true);
			}

			weaponsManager.getPlayerWeaponsManagerComponents (false);
			weaponsManager.setWeaponsParent (false, true);
			headBobManager.setFirstOrThirdMode (false);

			updateCameraComponents ();

			print ("Player's view configured as Third Person");
		}
	}

	void updateCameraComponents ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (playerCameraManager);
		EditorUtility.SetDirty (powersManager);
		EditorUtility.SetDirty (gravityManager);
		EditorUtility.SetDirty (headBobManager);
		EditorUtility.SetDirty (grabManager);
		EditorUtility.SetDirty (damageInScreenManager);
		EditorUtility.SetDirty (jetpackSystemManager);
		EditorUtility.SetDirty (weaponsManager);
		#endif
	}


	[System.Serializable]
	public class playerMode
	{
		public string nameMode;
		public bool isCurrentState;
		public UnityEvent activatePlayerModeEvent;
		public UnityEvent deactivatePlayerModeEvent;
	}

	[System.Serializable]
	public class playerControl
	{
		public string Name;
		public bool isCurrentState;
		public UnityEvent activateControlModeEvent;
		public UnityEvent deactivateControlModeEvent;
		public Texture modeTexture;
		public bool avoidToSetRegularModeWhenActive;
	}
}
