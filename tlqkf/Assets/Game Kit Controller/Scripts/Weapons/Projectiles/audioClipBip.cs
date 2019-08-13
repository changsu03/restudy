using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioClipBip : MonoBehaviour
{
	public float playTime;
	public float playRate;
	public float increasePitchSpeed;
	public float increasePlayRateSpeed;

	public AudioClip soundClip;

	public bool playBipAtStart = true;

	float lastTimePlayed;
	float totalTimePlayed;
	bool audioPlayed;
	AudioSource mainAudioSource;
	bool bipActivated;
	float originalPlayRate;

	void Start ()
	{
		mainAudioSource = GetComponent<AudioSource> ();
		totalTimePlayed = Time.time;
		originalPlayRate = playRate;
	}

	void Update ()
	{
		if (!audioPlayed && (bipActivated || playBipAtStart)) {
			if (Time.time > lastTimePlayed + playRate) {
				mainAudioSource.pitch += increasePitchSpeed;
				mainAudioSource.PlayOneShot (soundClip);
				lastTimePlayed = Time.time;
				playRate -= increasePlayRateSpeed;
				if (playRate <= 0) {
					playRate = 0.1f;
				}
				if (Time.time > totalTimePlayed + playTime) {
					audioPlayed = true;
				}
			}
		}
	}

	public void increasePlayTime (float extraTime)
	{
		totalTimePlayed = Time.time;
		playTime = extraTime;
		bipActivated = true;
	}

	public void disableBip(){
		bipActivated = false;
		mainAudioSource.pitch = 1;
		lastTimePlayed = 0;
		playRate = originalPlayRate;
	}
}
