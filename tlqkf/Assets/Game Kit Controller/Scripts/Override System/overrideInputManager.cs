using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class overrideInputManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public bool inputEnabled;
	public bool isPlayerController;
	public playerInputManager playerInput;

	public UnityEvent startOverrideFunction;
	public UnityEvent stopOverrideFunction;

	public bool usePreOverrideFunctions;
	public UnityEvent preStartOverrideFunction;
	public UnityEvent preStopOverrideFunction;

	public bool activateActionScreen = true;
	public string actionScreenName;

	public bool destroyObjectOnStopOverride;
	public UnityEvent eventToDestroy;

	public bool showDebugActions;

	public overrideCameraController overrideCameraControllerManager;

	GameObject currentPlayer;
	playerController playerControllerManager;

	inputManager input;

	overrideElementControlSystem currentOverrideElementControlSystem;

	void Start ()
	{
		input = FindObjectOfType<inputManager> ();
	}

	void Update ()
	{
		if (inputEnabled) {
			for (int i = 0; i < multiAxesList.Count; i++) {
				if (multiAxesList [i].currentlyActive) {
					for (int j = 0; j < multiAxesList [i].axes.Count; j++) {
						if (multiAxesList [i].axes [j].actionEnabled) {
							if (playerInput.checkPlayerInputButtonFromMultiAxesList (multiAxesList [i].multiAxesStringIndex, multiAxesList [i].axes [j].axesStringIndex, 
								    multiAxesList [i].axes [j].buttonPressType, multiAxesList [i].axes [j].canBeUsedOnPausedGame)) {
								if (showDebugActions) {
									print (multiAxesList [i].axes [j].Name);
								}
								multiAxesList [i].axes [j].buttonEvent.Invoke ();
							}
						}
					}
				}
			}
		}
	}

	public Vector2 getCustomRawMovementAxis ()
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		return playerInput.getPlayerRawMovementAxis ();
	}

	public Vector2 getCustomMovementAxis (string controlType)
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		return playerInput.getPlayerMovementAxis (controlType);
	}

	public void setCustomInputEnabledState (bool state)
	{
		inputEnabled = state;
	}

	public void setPlayerInfo (GameObject player)
	{
		currentPlayer = player;
		playerControllerManager = currentPlayer.GetComponent<playerController> ();
		currentOverrideElementControlSystem = currentPlayer.GetComponent<overrideElementControlSystem> ();

		if (!isPlayerController) {
			playerInput = player.GetComponent<playerInputManager> ();
		}
	}

	public void overrideControlState (bool state, GameObject currentPlayer)
	{
		if (state) {
			setPlayerInfo (currentPlayer);
			startOverrideFunction.Invoke ();
		} else {
			stopOverrideFunction.Invoke ();
		}

		if (playerInput) {
			if (isPlayerController) {
				playerInput.setPlayerID (playerControllerManager.getPlayerID ());
				playerInput.setPlayerInputEnabledState (state);
			} else {
				playerInput.setInputCurrentlyActiveState (!state);
			}
		}

		setCustomInputEnabledState (state);

		checkActivateActionScreen (state);
	}

	public void checkActivateActionScreen (bool state)
	{
		if (activateActionScreen) {
			if (playerControllerManager) {
				playerControllerManager.getPlayerInput ().enableOrDisableActionScreen (actionScreenName, state);
			}
		}
	}

	public void setPreOverrideControlState (bool state)
	{
		if (usePreOverrideFunctions) {
			if (state) {
				preStartOverrideFunction.Invoke ();
			} else {
				preStopOverrideFunction.Invoke ();
			}
		}
	}

	public void stopOverride ()
	{
		if (inputEnabled) {
			currentOverrideElementControlSystem.stopCurrentOverrideControl ();
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

	public overrideCameraController getOverrideCameraControllerManager ()
	{
		return overrideCameraControllerManager;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<overrideInputManager> ());
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
}
