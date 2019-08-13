using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class objectiveEventSystem : MonoBehaviour
{
	public List<objectiveInfo> objectiveInfoList = new List<objectiveInfo> ();

	public bool showObjectiveName;
	public string generalObjectiveName;

	public bool showObjectiveDescription;
	[TextArea (1, 10)] public string generalObjectiveDescription;
	[TextArea (1, 10)] public string objectiveFullDescription;

	public bool hideObjectivePanelsAfterXTime;
	public float timeToHideObjectivePanel;

	public bool objectivesFollowsOrder;
	public int currentSubObjectiveIndex;

	public bool useTimeLimit;
	public float timerSpeed;
	[Range (0, 60)] public float minutesToComplete;
	[Range (0, 60)] public float secondsToComplete;
	public float secondSoundTimerLowerThan;
	public AudioClip secondTimerSound;

	public bool objectiveInProcess;
	public bool objectiveComplete;

	public bool addObjectiveToPlayerLogSystem = true;

	public UnityEvent eventWhenObjectiveComplete;
	public bool callEventWhenObjectiveNotComplete;
	public UnityEvent eventWhenObjectiveNotComplete;

	public bool useEventOnObjectiveStart;
	public UnityEvent eventOnObjectiveStart;

	public bool useExtraListMapObjectInformation;
	public List<mapObjectInformation> extraListMapObjectInformation = new List<mapObjectInformation> ();

	public bool useSoundOnSubObjectiveComplete;
	public AudioClip soundOnSubObjectiveComplete;

	public bool useSoundOnObjectiveNotComplete;
	public AudioClip soundOnObjectiveNotComplete;

	public bool enableAllMapObjectInformationAtOnce;

	public bool enableAllMapObjectInformationOnTime;
	public float timeToEnableAllMapObjectInformation;

	public int numberOfObjectives;

	public bool setCurrentPlayerManually;
	public GameObject currentPlayerToConfigure;

	public bool canCancelPreviousMissionToStartNewOne;

	float lastTimeObjectiveInProcess;

	float totalSecondsTimer;

	AudioSource mainAudioSource;

	Text currentObjectiveNameText;
	Text currentObjectiveDescriptionText;

	Text screenTimerText;
	showGameInfoHud gameInfoHudManager;
	GameObject currentPlayer;

	GameObject objectiveInfoPanel;

	objectiveLogSystem currentObjectiveLogSystem;
	menuPause currentPauseManager;

	bool cancellingMisionActive;

	void Start ()
	{
		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].objectiveEnabled) {
				numberOfObjectives++;
			}
		}

		if (setCurrentPlayerManually) {
			setCurrentPlayer (currentPlayerToConfigure);
		}
	}

	void Update ()
	{
		if (objectiveInProcess) {
			if (useTimeLimit) {
				totalSecondsTimer -= Time.deltaTime * timerSpeed;
				screenTimerText.text = convertSeconds ();
				if (secondTimerSound) {
					if (totalSecondsTimer - 1 <= secondSoundTimerLowerThan && totalSecondsTimer % 1 < 0.1f) {
						playAudioSourceShoot (secondTimerSound);
					}
				}

				if (totalSecondsTimer <= 0) {
					stopObjective ();
				}
			}

			if (enableAllMapObjectInformationOnTime) {
				if (lastTimeObjectiveInProcess > 0 && Time.time > lastTimeObjectiveInProcess + timeToEnableAllMapObjectInformation) {
					for (int i = 0; i < objectiveInfoList.Count; i++) {
						if (objectiveInfoList [i].useMapObjectInformation && objectiveInfoList [i].objectiveEnabled) {
							if (objectiveInfoList [i].currentMapObjectInformation) {
								objectiveInfoList [i].currentMapObjectInformation.createMapIconInfo ();
							}
						}
						lastTimeObjectiveInProcess = 0;
					}
				}
			}
		}
	}

	public string convertSeconds ()
	{
		int minutes = Mathf.FloorToInt (totalSecondsTimer / 60F);
		int seconds = Mathf.FloorToInt (totalSecondsTimer - minutes * 60);
		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}

	public void addSubObjectiveComplete (string subObjectiveName)
	{
		if (!objectiveInProcess) {
			return;
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].Name == subObjectiveName) {
				objectiveInfoList [i].subObjectiveComplete = true;

				if (objectivesFollowsOrder) {
					if (i != currentSubObjectiveIndex) {
						
						stopObjective ();
						return;
					}
				}

				currentSubObjectiveIndex++;

				if (objectiveInfoList [i].useEventOnSubObjectiveComplete) {
					objectiveInfoList [i].eventOnSubObjectiveComplete.Invoke ();
				}
					
				if (objectiveInfoList [i].setObjectiveNameOnScreen) {
					if (currentObjectiveNameText) {
						currentObjectiveNameText.text = objectiveInfoList [i].objectiveName;
					}
				}

				if (objectiveInfoList [i].setObjectiveDescriptionOnScreen) {
					if (currentObjectiveDescriptionText) {
						currentObjectiveDescriptionText.text = objectiveInfoList [i].objectiveDescription;
					}
				}

				print ("numberOfObjectives: " + currentSubObjectiveIndex + " " + numberOfObjectives + " " + i);

				if (currentSubObjectiveIndex == numberOfObjectives) {
					setObjectiveComplete ();
					print ("objective complete");
				} else {
					if (useTimeLimit && objectiveInfoList [i].giveExtraTime) {
						totalSecondsTimer += objectiveInfoList [i].extraTime;
					}

					if (i + 1 < objectiveInfoList.Count) {
						if (!enableAllMapObjectInformationAtOnce) {
							if (objectiveInfoList [i + 1].useMapObjectInformation && objectiveInfoList [i + 1].objectiveEnabled) {
								if (objectiveInfoList [i + 1].currentMapObjectInformation) {
									objectiveInfoList [i + 1].currentMapObjectInformation.createMapIconInfo ();
								}
							}
						}
					}
				}

				return;
			}
		}
	}

	public void removeSubObjectiveComplete (string subObjectiveName)
	{
		if (!objectiveInProcess) {
			return;
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].Name == subObjectiveName && objectiveInfoList [i].subObjectiveComplete) {
				objectiveInfoList [i].subObjectiveComplete = false;

				if (objectivesFollowsOrder) {
					if (i != currentSubObjectiveIndex) {

						stopObjective ();
						return;
					}
				} else {
					checkInCallEventWhenObjectiveNotComplete ();
				}

				currentSubObjectiveIndex--;

				return;
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer) {
			currentPauseManager = currentPlayer.GetComponent<playerInputManager> ().getPauseManager ();

			currentObjectiveLogSystem = currentPauseManager.gameObject.GetComponent<objectiveLogSystem> ();

			gameInfoHudManager = currentPauseManager.getGameInfoHudManager ();
		}
	}

	public void startObjective ()
	{
		if (objectiveInProcess && !canCancelPreviousMissionToStartNewOne) {
			return;
		}

		if (canCancelPreviousMissionToStartNewOne) {
			if (currentObjectiveLogSystem != null) {
				currentObjectiveLogSystem.cancelPreviousObjective ();
			}
		}

		if (hideObjectivePanelsAfterXTime) {
			disableObjectivePanelsAfterXTime ();
		}
			
		objectiveInProcess = true;

		if (useTimeLimit) {
			totalSecondsTimer = secondsToComplete + minutesToComplete * 60;

			if (!screenTimerText) {
				GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Timer", "Timer Text"); 
				if (currenObjectFound) {
					screenTimerText = currenObjectFound.GetComponent<Text> ();
				}
			} 
			if (screenTimerText) {
				screenTimerText.gameObject.SetActive (true);
			}
		}

		if (showObjectiveName) {
			if (!currentObjectiveNameText) {
				GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Objective Info", "Objective Name Text");
				if (currenObjectFound) {
					currentObjectiveNameText = currenObjectFound.GetComponent<Text> ();
				}
			} 
			if (currentObjectiveNameText) {
				currentObjectiveNameText.gameObject.SetActive (true);
				currentObjectiveNameText.text = generalObjectiveName;
			}
		}

		if (showObjectiveDescription) {
			if (!currentObjectiveDescriptionText) {
				GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Objective Info", "Objective Description Text");
				if (currenObjectFound) {
					currentObjectiveDescriptionText = currenObjectFound.GetComponent<Text> ();
				}
			} 
			if (currentObjectiveDescriptionText) {
				currentObjectiveDescriptionText.gameObject.SetActive (true);
				currentObjectiveDescriptionText.text = generalObjectiveDescription;
			}
		}

		if (showObjectiveName || showObjectiveDescription) {
			if (!objectiveInfoPanel) {
				objectiveInfoPanel = gameInfoHudManager.getHudElement ("Objective Info", "Objective Info Panel");
			} 
			if (objectiveInfoPanel) {
				objectiveInfoPanel.SetActive (true);
			}
		}
			
		if (currentPlayer) {
			mainAudioSource = currentPlayer.GetComponent<playerStatesManager> ().getAudioSourceElement ("Timer Audio Source");
		}

		if (useEventOnObjectiveStart) {
			eventOnObjectiveStart.Invoke ();
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].useMapObjectInformation && objectiveInfoList [i].objectiveEnabled) {
				if (objectiveInfoList [i].currentMapObjectInformation) {
					objectiveInfoList [i].currentMapObjectInformation.createMapIconInfo ();
				}
			}
		}

		lastTimeObjectiveInProcess = Time.time;
		if (addObjectiveToPlayerLogSystem && currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.addObjective (generalObjectiveName, generalObjectiveDescription, objectiveFullDescription, "", this);
			currentObjectiveLogSystem.activeObjective (this);
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				extraListMapObjectInformation [i].createMapIconInfo ();
			}
		}
	}

	public void resetAllSubObjectives ()
	{
		for (int i = 0; i < objectiveInfoList.Count; i++) {
			objectiveInfoList [i].subObjectiveComplete = false;
		}
	}

	public void cancelPreviousObjective ()
	{
		cancellingMisionActive = true;

		stopObjective ();

		cancellingMisionActive = false;
	}

	public void stopObjective ()
	{
		objectiveInProcess = false;
		if (useTimeLimit) {
			screenTimerText.gameObject.SetActive (false);
		}

		if (useSoundOnObjectiveNotComplete && !cancellingMisionActive) {
			playAudioSourceShoot (soundOnObjectiveNotComplete);
		}

		resetAllSubObjectives ();

		currentSubObjectiveIndex = 0;

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].currentMapObjectInformation) {
				objectiveInfoList [i].currentMapObjectInformation.removeMapObject ();
			}
		}

		disableHUDElements ();

		if (addObjectiveToPlayerLogSystem && currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.cancelObjective (this);
		}

		checkInCallEventWhenObjectiveNotComplete ();

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				extraListMapObjectInformation [i].removeMapObject ();
			}
		}
	}

	public void setObjectiveComplete ()
	{
		objectiveInProcess = false;

		objectiveComplete = true;

		eventWhenObjectiveComplete.Invoke ();

		if (useSoundOnSubObjectiveComplete) {
			playAudioSourceShoot (soundOnSubObjectiveComplete);
		}

		disableHUDElements ();

		if (addObjectiveToPlayerLogSystem && currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.objectiveComplete (this);
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				extraListMapObjectInformation [i].removeMapObject ();
			}
		}
	}

	public void checkInCallEventWhenObjectiveNotComplete ()
	{
		if (callEventWhenObjectiveNotComplete) {
			eventWhenObjectiveNotComplete.Invoke ();
		}
	}

	public void disableHUDElements ()
	{
		if (useTimeLimit) {
			screenTimerText.gameObject.SetActive (false);
		}

		if (currentObjectiveNameText) {
			currentObjectiveNameText.gameObject.SetActive (false);
		}

		if (currentObjectiveDescriptionText) {
			currentObjectiveDescriptionText.gameObject.SetActive (false);
		}

		if (objectiveInfoPanel) {
			objectiveInfoPanel.SetActive (false);
		}
	}

	public void playAudioSourceShoot (AudioClip clip)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (clip);
		}
	}


	Coroutine disableObjectivePanelCoroutine;

	public void disableObjectivePanelsAfterXTime ()
	{
		stopDisableObjectivePanelCoroutine ();
		disableObjectivePanelCoroutine = StartCoroutine (disableObjectivePanelsAfterXTimeCoroutine ());
	}

	public void stopDisableObjectivePanelCoroutine ()
	{
		if (disableObjectivePanelCoroutine != null) {
			StopCoroutine (disableObjectivePanelCoroutine);
		}
	}

	IEnumerator disableObjectivePanelsAfterXTimeCoroutine ()
	{
		yield return new WaitForSeconds (timeToHideObjectivePanel);

		if (currentObjectiveNameText) {
			currentObjectiveNameText.gameObject.SetActive (false);
		}
	
		if (showObjectiveDescription) {
			if (currentObjectiveDescriptionText) {
				currentObjectiveDescriptionText.gameObject.SetActive (false);
			}
		}

		if (showObjectiveName || showObjectiveDescription) {
			if (objectiveInfoPanel) {
				objectiveInfoPanel.SetActive (false);
			}
		}
	}

	[System.Serializable]
	public class objectiveInfo
	{
		public string Name;
		public string objectiveName;
		[TextArea (1, 10)] public string objectiveDescription;

		public bool objectiveEnabled = true;

		public bool useMapObjectInformation;
		public mapObjectInformation currentMapObjectInformation;

		public bool giveExtraTime;
		public float extraTime;

		public bool setObjectiveNameOnScreen;
		public bool setObjectiveDescriptionOnScreen;

		public bool subObjectiveComplete;

		public bool useEventOnSubObjectiveComplete;
		public UnityEvent eventOnSubObjectiveComplete;
	}
}
