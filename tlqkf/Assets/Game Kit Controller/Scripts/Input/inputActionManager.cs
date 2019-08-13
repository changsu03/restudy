using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class inputActionManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public bool inputActivated;
	public inputManager input;

	public bool showDebugActions;

	public bool manualControlActive;
	[Range (-1, 1)] public float manualHorizontalInput;
	[Range (-1, 1)] public float manualVerticalInput;

	public bool usingTouchMovementJoystick = true;

	Vector2 manualControlAxisValues;

	GameObject currentDriver;
	playerInputManager playerInput;

	multiAxes currentMultiAxes;
	Axes curentAxes;

	void Start ()
	{
		input = FindObjectOfType<inputManager> ();
	}

	void Update ()
	{
		if (inputActivated) {
			for (int i = 0; i < multiAxesList.Count; i++) {
				
				currentMultiAxes = multiAxesList [i];

				if (currentMultiAxes.currentlyActive) {
					for (int j = 0; j < currentMultiAxes.axes.Count; j++) {

						curentAxes = multiAxesList [i].axes [j];

						if (curentAxes.actionEnabled) {
							if (playerInput.checkPlayerInputButtonFromMultiAxesList (currentMultiAxes.multiAxesStringIndex, curentAxes.axesStringIndex, 
								curentAxes.buttonPressType, curentAxes.canBeUsedOnPausedGame)) {

								if (showDebugActions) {
									print (curentAxes.Name);
								}

								curentAxes.buttonEvent.Invoke ();
							}
						}
					}
				}
			}
		}
	}

	public Vector2 getPlayerMovementAxis (string controlType)
	{
		if (!inputActivated && !manualControlActive) {
			return Vector2.zero;
		}

		if (manualControlActive) {
			return new Vector2 (manualHorizontalInput, manualVerticalInput);
		} else {
			return playerInput.getPlayerMovementAxis (controlType);
		}
	}

	public Vector2 getPlayerRawMovementAxis ()
	{
		if (!inputActivated && !manualControlActive) {
			return Vector2.zero;
		}

		if (manualControlActive) {
			if (manualHorizontalInput > 0) {
				manualControlAxisValues.x = 1;
			} else if (manualHorizontalInput < 0) {
				manualControlAxisValues.x = -1;
			} else {
				manualControlAxisValues.x = 0;
			}

			if (manualVerticalInput > 0) {
				manualControlAxisValues.y = 1;
			} else if (manualVerticalInput < 0) {
				manualControlAxisValues.y = -1;
			} else {
				manualControlAxisValues.y = 0;
			}

			return manualControlAxisValues;
		} else {
			return playerInput.getPlayerRawMovementAxis ();
		}
	}

	public void enableOrDisableInput (bool state, GameObject driver)
	{
		inputActivated = state;
		currentDriver = driver;
		playerInput = currentDriver.GetComponent<playerInputManager> ();

		if (playerInput.isUsingTouchControls ()) {
			if (state) {
				playerInput.setUsingTouchMovementJoystickState (usingTouchMovementJoystick);
				playerInput.enableOrDisableTouchMovementJoystickForButtons (true);
			} else {
				playerInput.setOriginalTouchMovementInputState ();
			}
		}
	}

	//get the input manager component
	public void getInputManager ()
	{
		input = FindObjectOfType<inputManager> ();
	}

	public void setInputManager ()
	{
		if (!input) {
			input = FindObjectOfType<inputManager> ();
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

				currentMultiAxesList.axes [i].keyButton = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [currentMultiAxesList.axes [i].axesStringIndex].key.ToString ();
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

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<inputActionManager> ());
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
		public bool showInControlsMenu;

		public string keyButton;
	}
}