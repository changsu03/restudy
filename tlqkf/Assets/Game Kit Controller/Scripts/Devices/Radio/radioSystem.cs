﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class radioSystem : MonoBehaviour
{
	public float radioVolume;

	public Scrollbar volumeScrollbar;

	public Text currentSongNameText;

	public Slider songLenghtSlider;

	public GameObject playButton;
	public GameObject pauseButton;

	public bool playingSong;
	public bool songPaused;
	public float currentSongLength;
	public bool playSongsRandomly;
	public bool repeatList;

	public bool radioActive;

	public bool playSongsOnActive;

	public bool movingSongLenghtSlider;

	public GameObject songListContent;
	public GameObject songListElementParent;
	public GameObject songListElement;

	public AudioSource source;
	public List<AudioClip> clips = new List<AudioClip> ();

	public int currentIndex = 0;
	songsManager mainSongsManager;

	bool songsLoaded;

	void Start ()
	{
		songListElement.SetActive (false);
		if (source == null) {
			source = gameObject.AddComponent<AudioSource> ();
		}

		volumeScrollbar.value = radioVolume;

		mainSongsManager = FindObjectOfType<songsManager> ();

		currentSongNameText.text = "...";
	}

	void Update ()
	{
		if (radioActive) {
			if (playingSong) {
				if (!movingSongLenghtSlider) {
					songLenghtSlider.value = source.time / currentSongLength;
				}

				if (!songPaused) {
					if ((source.time / currentSongLength) > 0.99f) {
						if (currentIndex == clips.Count - 1) {
							if (repeatList) {
								setNextSong ();
							} else {
								stopCurrentSong ();
								currentIndex = 0;
								setPlayPauseButtonState (true);
							}
						} else {
							if (playSongsRandomly) {
								setRandomSong ();
							} else {
								setNextSong ();
							}
						}
					}
				}
			}
		}

		if (!songsLoaded) {
			if (mainSongsManager.allSongsLoaded ()) {
				getSongsList ();
				songsLoaded = true;
			}
		}
	}

	public void playCurrentSong ()
	{
		if (playingSong) {
			source.Play ();
			currentSongLength = source.clip.length;
			songLenghtSlider.value = source.time / currentSongLength;
		} else {
			PlayCurrent ();
		}
		songPaused = false;
	}

	public void stopCurrentSong ()
	{
		source.Stop ();
		playingSong = false;
		songLenghtSlider.value = 0;
	}

	public void pauseCurrentSong ()
	{
		songPaused = true;
		source.Pause ();
	}

	public void setNextSong ()
	{
		if(clips.Count==0){
			return;
		}
		
		if (playSongsRandomly) {
			setRandomSongIndex ();
		} else {
			currentIndex = (currentIndex + 1) % clips.Count; 
		}
		setPlayPauseButtonState (false);
		PlayCurrent ();
	}

	public void setPreviousSong ()
	{
		if(clips.Count==0){
			return;
		}
		
		if (playSongsRandomly) {
			setRandomSongIndex ();
		} else {
			currentIndex--;
			if (currentIndex < 0) {
				currentIndex = clips.Count - 1;
			}
		}
		setPlayPauseButtonState (false);
		PlayCurrent ();
	}

	public void setRandomSong ()
	{
		setRandomSongIndex ();
		PlayCurrent ();
	}

	public void setRandomSongIndex ()
	{
		int nextIndex = 0;
		int loop = 0;
		while (nextIndex == currentIndex) {
			nextIndex = (int)UnityEngine.Random.Range (0, clips.Count);
			loop++;
			if (loop > 100) {
				print ("loop error");
				return;
			}
		}
		currentIndex = nextIndex;
	}

	public void setRadioVolume ()
	{
		radioVolume = volumeScrollbar.value;
		source.volume = radioVolume;
	}

	void PlayCurrent ()
	{
		if (clips.Count <= 0) {
			return;
		}
		source.clip = clips [currentIndex];
		source.time = 0;
		source.Play ();
		string songName = clips [currentIndex].name;
		int extensionIndex = songName.IndexOf (".");
		songName = songName.Substring (0, extensionIndex);
		currentSongNameText.text = songName;
		playingSong = true;
		currentSongLength = source.clip.length;
		songLenghtSlider.value = source.time / currentSongLength;
	}

	public void setPlaySongsRandomlyState (bool state)
	{
		playSongsRandomly = state;
	}

	public void setRepeatListState (bool state)
	{
		repeatList = state;
	}

	public void setMovingSongLenghtSliderState (bool state)
	{
		if (playingSong) {
			movingSongLenghtSlider = state;
		}
	}

	public void setSongPart ()
	{
		if (playingSong) {
			source.time = source.clip.length * songLenghtSlider.value;
		}
	}

	public void setRadioActiveState (bool state)
	{
		radioActive = state;

		if (radioActive) {
			if (playSongsOnActive) {
				PlayCurrent ();
				setPlayPauseButtonState (false);
			}
		} else {
			if (playingSong) {
				stopCurrentSong ();
				setPlayPauseButtonState (true);
			}
			songListContent.SetActive (false);
		}
	}

	public void setOnlyRadioActiveState (bool state)
	{
		radioActive = state;
	}

	public void setPlayPauseButtonState (bool state)
	{
		playButton.SetActive (state);
		pauseButton.SetActive (!state);
	}

	public void selectSongOnList (GameObject songButtonPressed)
	{
		for (int i = 0; i < clips.Count; i++) {
			if (clips [i].name.Contains (songButtonPressed.GetComponentInChildren<Text> ().text)) {
				currentIndex = i;
				PlayCurrent ();
				return;
			}
		}
	}

	public void getSongsList ()
	{
		//print (mainSongsManager.getSongsList ().Count);
		clips = mainSongsManager.getSongsList ();

		for (int i = 0; i < clips.Count; i++) {
			string songName = clips [i].name;
			int extensionIndex = songName.IndexOf (".");
			songName = songName.Substring (0, extensionIndex);

			GameObject newSongListElement = (GameObject)Instantiate (songListElement, Vector3.zero, songListElement.transform.rotation);
			newSongListElement.SetActive (true);
			newSongListElement.transform.SetParent (songListElementParent.transform);
			newSongListElement.transform.localScale = Vector3.one;
			newSongListElement.transform.localPosition = Vector3.zero;
			newSongListElement.name = "Song List Element (" + songName + ")";
			newSongListElement.GetComponentInChildren<Text> ().text = songName;
		}
	}
}