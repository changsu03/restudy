using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class timeBullet : MonoBehaviour
{
	public bool timeBulletEnabled;
	public float timeBulletTimeSpeed = 0.1f;
	public bool restoreAudioPitch;
	public bool timeBulletActivated;
	public bool previouslyActivated;
	public playerInputManager playerInput;

	float timeBulletTime = 1;

	AudioSource[] audios;
	Coroutine timeCoroutine;

	public void activateTime ()
	{
		//check that the player is not using a device, the game is not paused and that this feature is enabled
		if (Time.deltaTime != 0 && timeBulletEnabled) {
			timeBulletActivated = !timeBulletActivated;
			if (timeBulletActivated) {
				timeBulletTime = timeBulletTimeSpeed;
			} else {
				timeBulletTime = 1;
				changeAudioPitch ();
			}
			Time.timeScale = timeBulletTime;
			Time.fixedDeltaTime = timeBulletTime * 0.02f;
		}
	}

	public void disableTimeBullet ()
	{
		if (timeBulletActivated) {
			activateTime ();
			previouslyActivated = true;
		}
	}

	public void disableTimeBulletTotally ()
	{
		if (timeBulletActivated) {
			activateTime ();
		}
		previouslyActivated = false;
	}

	public void reActivateTime ()
	{
		if (previouslyActivated) {
			if (timeCoroutine != null) {
				StopCoroutine (timeCoroutine);
			}
			timeCoroutine = StartCoroutine (reActivateTimeCoroutine ());
			previouslyActivated = false;
		}
	}

	IEnumerator reActivateTimeCoroutine ()
	{
		yield return new WaitForSeconds (0.001f);
		activateTime ();
	}

	public void changeAudioPitch ()
	{
		if (restoreAudioPitch) {
			audios = FindObjectsOfType (typeof(AudioSource)) as AudioSource[];
			for (int i = 0; i < audios.Length; i++) {
				audios [i].pitch = 1;
			}
		}
	}

	Coroutine timeBulletXSecondsCoroutine;

	public void activateTimeBulletXSeconds (float timeBulletDuration, float timeScale)
	{
		if (timeBulletXSecondsCoroutine != null) {
			StopCoroutine (timeBulletXSecondsCoroutine);
		}
		timeBulletXSecondsCoroutine = StartCoroutine (activateTimeBulletXSecondsCoroutine (timeBulletDuration, timeScale));
	}

	IEnumerator activateTimeBulletXSecondsCoroutine (float timeBulletDuration, float timeScale)
	{
		setBulletTimeState (true, timeScale);
		yield return new WaitForSeconds (timeBulletDuration * timeScale);
		setBulletTimeState (false, 1);
	}

	public void setBulletTimeState (bool state, float timeScale)
	{
		//check that the player is not using a device, the game is not paused and that this feature is enabled
		if (Time.deltaTime != 0 && timeBulletEnabled) {
			timeBulletActivated = state;
			timeBulletTime = timeScale;
			Time.timeScale = timeBulletTime;
			Time.fixedDeltaTime = timeBulletTime * 0.02f;
			changeAudioPitch ();
		}
	}

	public void inputActivateBulletTime ()
	{
		if (timeBulletEnabled) {
			activateTime ();
		}
	}
}