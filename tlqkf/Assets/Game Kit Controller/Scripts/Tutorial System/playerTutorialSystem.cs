using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEngine.Video;
#endif

public class playerTutorialSystem : MonoBehaviour
{
	public bool tutorialsEnabled;
	public List<tutorialInfo> tutorialInfoList = new List<tutorialInfo> ();
	public int currentTutorialIndex;
	public int currentTutorialPanelIndex;
	public bool tutorialOpened;
	public Transform tutorialsPanel;

	public playerController playerControllerManager;
	public menuPause pauseManager;
	public AudioSource mainAudioSource;

	public AudioSource videoAudioSource;

	tutorialInfo currentTutorialInfo;
	KeyCode[] validKeyCodes;
	float lastTimeTutorialOpened;
	float previousTimeScale;

	VideoPlayer mainVideoPlayer;

	void Start ()
	{
		validKeyCodes = (KeyCode[])System.Enum.GetValues (typeof(KeyCode));
	}

	void Update ()
	{
		if (tutorialOpened && (currentTutorialInfo.pressAnyButtonToNextTutorial || pauseManager.gamePaused)) {
			if (Time.time > lastTimeTutorialOpened + currentTutorialInfo.timeToEnableKeys || pauseManager.gamePaused || currentTutorialInfo.setCustomTimeScale) {
				foreach (KeyCode vKey in validKeyCodes) {
					if (Input.GetKeyDown (vKey)) {
						setNextPanelOnTutorial ();
					}
				}
			}
		}
	}

	bool activatingTutorialByNameFromEditorState;

	public void activatingTutorialByNameFromEditor ()
	{
		activatingTutorialByNameFromEditorState = true;
	}

	public void activateTutorialByName (string tutorialName)
	{
		if (!tutorialsEnabled && !pauseManager.gamePaused) {
			return;
		}

		for (int i = 0; i < tutorialInfoList.Count; i++) {
			if (tutorialInfoList [i].Name == tutorialName) {
				currentTutorialInfo = tutorialInfoList [i];

				if (!activatingTutorialByNameFromEditorState) {
					if (currentTutorialInfo.playTutorialOnlyOnce && !pauseManager.gamePaused && currentTutorialInfo.tutorialPlayed) {
						return;
					}
				}

				currentTutorialIndex = i;
				currentTutorialPanelIndex = 0;

				checkOpenOrCloseTutorial (true);

				return;
			}
		}

		print ("WARNING: no tutorial was found with the name " + tutorialName + ", check if the name is properly configured in the event and in the player tutorial system inspector");
	}
		
	Coroutine openTutorialCoroutine;

	public void checkOpenOrCloseTutorial (bool state)
	{
		if (state) {
			if (pauseManager.gamePaused) {
				openOrCloseTutorial (true);
			} else {
				stopCheckOpenOrCloseTutorial ();

				openTutorialCoroutine = StartCoroutine (checkOpenOrCloseTutorialCoroutine ());
			}
		} else {
			stopCheckOpenOrCloseTutorial ();

			openOrCloseTutorial (false);
		}
	}

	public void stopCheckOpenOrCloseTutorial ()
	{
		if (openTutorialCoroutine != null) {
			StopCoroutine (openTutorialCoroutine);
		}
	}

	IEnumerator checkOpenOrCloseTutorialCoroutine ()
	{
		yield return new WaitForSeconds (currentTutorialInfo.openTutorialDelay);

		openOrCloseTutorial (true);
	}

	public void setNextPanelOnTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count == 0) {
			closeTutorial ();
			return;
		}

		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		currentTutorialPanelIndex++;
		if (currentTutorialPanelIndex <= currentTutorialInfo.tutorialPanelList.Count - 1) {
			currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (true);

			#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
			if (currentTutorialInfo.containsVideo) {
				currentTutorialInfo.currentVideoIndex++;

				playTutorialVideo (currentTutorialInfo);
			}
			#endif

		} else {
			currentTutorialPanelIndex = currentTutorialInfo.tutorialPanelList.Count - 1;
			closeTutorial ();
		}
	}

	public void setPreviousPanelOnTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count == 0) {
			return;
		}

		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		currentTutorialPanelIndex--;
		if (currentTutorialPanelIndex < 0) {
			currentTutorialPanelIndex = 0;
		}
		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
	}

	public void openOrCloseTutorial (bool opened)
	{
		tutorialOpened = opened;

		if (tutorialOpened) {
			currentTutorialInfo.panelGameObject.SetActive (true);

			if (currentTutorialInfo.tutorialPanelList.Count > 0) {
				currentTutorialInfo.tutorialPanelList [0].panelGameObject.SetActive (true);
			}

			lastTimeTutorialOpened = Time.time;

			#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
			if (currentTutorialInfo.containsVideo) {
				playTutorialVideo (currentTutorialInfo);
			}
			#endif
		}

		bool isPlayerDriving = playerControllerManager.isPlayerDriving ();

		if (!pauseManager.gamePaused) {
			if (!tutorialOpened || currentTutorialInfo.unlockCursorOnTutorialActive) {
				if (!isPlayerDriving) {
					pauseManager.showOrHideCursor (tutorialOpened);
				}
			}

			if (!isPlayerDriving) {
				pauseManager.changeCameraState (!tutorialOpened);
				pauseManager.setHeadBobPausedState (!tutorialOpened);
			}

			pauseManager.usingSubMenuState (tutorialOpened);
		}
			
		pauseManager.openOrClosePlayerMenu (tutorialOpened, tutorialsPanel, true);

		if (!isPlayerDriving) {
			pauseManager.usingDeviceState (tutorialOpened);

			playerControllerManager.setUsingDeviceState (tutorialOpened);
	
			playerControllerManager.changeScriptState (!tutorialOpened);
		} else {
			playerControllerManager.getPlayerInput ().setInputCurrentlyActiveState (!tutorialOpened);
		}

		playerControllerManager.getPlayerInput ().pauseOrResumeInput (tutorialOpened);

		if (pauseManager.gamePaused && !tutorialOpened) {
			pauseManager.resetPauseMenuBlurPanel ();
		}

		if (!pauseManager.gamePaused) {

			if (!activatingTutorialByNameFromEditorState) {
				currentTutorialInfo.tutorialPlayed = true;
			}

			activatingTutorialByNameFromEditorState = false;

			if (tutorialOpened && currentTutorialInfo.useSoundOnTutorialOpen) {
				if (mainAudioSource) {
					mainAudioSource.PlayOneShot (currentTutorialInfo.soundOnTutorialOpen);
				}
			}

			if (currentTutorialInfo.setCustomTimeScale) {
				if (tutorialOpened) {
					previousTimeScale = Time.timeScale;
					Time.timeScale = currentTutorialInfo.customTimeScale;
				} else {
					Time.timeScale = previousTimeScale;
				}
			}
		}
	}

	#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
	Coroutine currentTutorialVideoCoroutine;

	public void playTutorialVideo (tutorialInfo newTutorialInfo)
	{
		stopTutorialVideoCoroutine ();

		currentTutorialVideoCoroutine = StartCoroutine (playTutorialVideoCoroutine (newTutorialInfo));
	}

	public void stopTutorialVideoCoroutine ()
	{
		if (currentTutorialInfo != null) {
			if (currentTutorialInfo.containsVideo) {
				if (mainVideoPlayer) {
					if (mainVideoPlayer.isPlaying) {
						mainVideoPlayer.Stop ();

						if (videoAudioSource) {
							videoAudioSource.Stop ();
						}
					}
				}
			}
		}

		if (currentTutorialVideoCoroutine != null) {
			StopCoroutine (currentTutorialVideoCoroutine);
		}
	}

	IEnumerator playTutorialVideoCoroutine (tutorialInfo newTutorialInfo)
	{
		if (currentTutorialInfo.currentVideoIndex < currentTutorialInfo.videoInfoList.Count) {

			videoInfo currentVideoInfo = currentTutorialInfo.videoInfoList [currentTutorialInfo.currentVideoIndex];

			mainVideoPlayer = currentTutorialInfo.panelGameObject.GetComponent<VideoPlayer> ();

			if (mainVideoPlayer == null) {
				mainVideoPlayer = tutorialsPanel.gameObject.AddComponent<VideoPlayer> ();
			}

			mainVideoPlayer.source = VideoSource.VideoClip;

			mainVideoPlayer.clip = currentVideoInfo.videoFile;

			//Set Audio Output to AudioSource
			mainVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

			//Assign the Audio from Video to AudioSource to be played

			mainVideoPlayer.EnableAudioTrack (0, true);
			mainVideoPlayer.SetTargetAudioSource (0, videoAudioSource);

			if (currentVideoInfo.useVideoAudio) {
				videoAudioSource.volume = currentVideoInfo.videoAudioVolume;
			} else {
				videoAudioSource.volume = 0;
			}
				
			//Set video To Play then prepare Audio to prevent Buffering
			mainVideoPlayer.Prepare ();

			//Wait until video is prepared
			while (!mainVideoPlayer.isPrepared) {
				yield return null;
			}

			//Assign the Texture from Video to RawImage to be displayed
			currentVideoInfo.videoRawImage.texture = mainVideoPlayer.texture;

			//Play Video
			mainVideoPlayer.Play ();

			//Play Sound
			if (currentVideoInfo.useVideoAudio) {
				videoAudioSource.Play ();
			}
			
			while (mainVideoPlayer.isPlaying) {
				//Debug.LogWarning ("Video Time: " + Mathf.FloorToInt ((float)mainVideoPlayer.time));
				yield return null;
			}

			if (currentVideoInfo.loopVideo) {
				playTutorialVideo (currentTutorialInfo);
			}

			if (currentTutorialInfo.setNextPanelWhenVideoEnds) {
				
				setNextPanelOnTutorial ();
			}
		}
	}
	#endif

	public void closeTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count > 0) {
			currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		}
			
		checkOpenOrCloseTutorial (false);

		#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		if (currentTutorialInfo.containsVideo) {
			
			stopTutorialVideoCoroutine ();

			currentTutorialInfo.currentVideoIndex = 0;
		}
		#endif

		currentTutorialInfo.panelGameObject.SetActive (false);
	}

	public void callNextTutorialPanel ()
	{
		if (tutorialOpened) {
			if (currentTutorialInfo.useActionButtonToMoveThroughTutorial) {
				setNextPanelOnTutorial ();
			}
		}
	}

	public void resetAllTutorials ()
	{
		for (int i = 0; i < tutorialInfoList.Count; i++) {
			tutorialInfoList [i].panelGameObject.SetActive (false);
			for (int j = 0; j < tutorialInfoList [i].tutorialPanelList.Count; j++) {
				if (j == 0) {
					tutorialInfoList [i].tutorialPanelList [j].panelGameObject.SetActive (true);
				} else {
					tutorialInfoList [i].tutorialPanelList [j].panelGameObject.SetActive (false);
				}
			}
		}
	}

	public void setTutorialsEnabledState (bool state)
	{
		tutorialsEnabled = state;
	}

	[System.Serializable]
	public class tutorialInfo
	{
		public string Name;
		public GameObject panelGameObject;
		public List<panelInfo> tutorialPanelList = new List<panelInfo> ();
		public bool unlockCursorOnTutorialActive;
		public bool useActionButtonToMoveThroughTutorial;
		public bool pressAnyButtonToNextTutorial;
		public float timeToEnableKeys = 1;

		public float openTutorialDelay;

		public bool setCustomTimeScale;
		public float customTimeScale;
		public bool useSoundOnTutorialOpen;
		public AudioClip soundOnTutorialOpen;

		public bool playTutorialOnlyOnce;
		public bool tutorialPlayed;

		#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		public bool containsVideo;
		public List<videoInfo> videoInfoList = new List<videoInfo> ();
		public bool setNextPanelWhenVideoEnds;
		public int currentVideoIndex;
		#endif
	}

	#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
	[System.Serializable]
	public class videoInfo
	{
		public string Name = "New Video";
		public RawImage videoRawImage;
		public VideoClip videoFile;

		public bool useVideoAudio = true;
		public float videoAudioVolume = 1;

		public bool loopVideo = true;

		public Coroutine currentTutorialVideoCoroutine;
	}
	#endif

	[System.Serializable]
	public class panelInfo
	{
		public string Name;
		public GameObject panelGameObject;
	}
}
