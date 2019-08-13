using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class pianoSystem : MonoBehaviour
{

	public List<keyInfo> keyInfoList = new List<keyInfo> ();

	public int keyRotationAmount = 30;
	public float keyRotationSpeed = 30;

	public bool usingPiano;

	[TextArea (1, 10)] public string songToPlay;
	public float playRate = 0.3f;
	public bool useEventWhenAutoPlaySongEnds;
	public UnityEvent eventWhenAutoPlaySongEnds;

	public int songLineLength;
	public float songLineDelay;

	//public bool allowToUseKeyboard;

	//string currentKeyPressed;
	AudioSource mainAudioSource;
	//inputManager input;

	Coroutine playSongCoroutine;

	void Start ()
	{
		mainAudioSource = GetComponent<AudioSource> ();
		//input = FindObjectOfType<inputManager> ();
		//playSong (songToPlay);
	}

	void Update ()
	{
//		if (usingPiano) {
//			if (allowToUseKeyboard) {
//				currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyDown);
//				if (currentKeyPressed != "") {
//					checkPressedKey (currentKeyPressed);
//				}
//			}
//		}
	}

	public void autoPlaySong ()
	{
		playSong (songToPlay);
	}

	public void playSong (string songNotes)
	{
		if (playSongCoroutine != null) {
			StopCoroutine (playSongCoroutine);
		}
		playSongCoroutine = StartCoroutine (autoPlaySongCoroutine (songNotes));
	}

	IEnumerator autoPlaySongCoroutine (string songNotes)
	{
		yield return new WaitForSeconds (1);
		int currentNumberOfNotes = 0;
		string[] notes = songNotes.Split(' ','\n');
		print (notes.Length);
		foreach (string letter in notes)
		{
			print (letter);
			checkPressedKey (letter);
			currentNumberOfNotes++;
			yield return new WaitForSeconds (playRate);

			if (currentNumberOfNotes % songLineLength == 0) {
				yield return new WaitForSeconds (songLineDelay);
			}
		}

		if (useEventWhenAutoPlaySongEnds) {
			if (eventWhenAutoPlaySongEnds.GetPersistentEventCount () > 0) {
				eventWhenAutoPlaySongEnds.Invoke ();
			}
		}

		yield return null;
	}

	public void checkPressedKey (string keyName)
	{
		if (!usingPiano) {
			return;
		}
		
		for (int i = 0; i < keyInfoList.Count; i++) {	
			if (keyInfoList [i].keyName == keyName) {
				playSound (keyInfoList [i].keySound);
				rotatePressedKey (keyInfoList [i]);
			}
		}
	}

	public void rotatePressedKey (keyInfo currentKeyInfo)
	{
		if (currentKeyInfo.keyPressCoroutine != null) {
			StopCoroutine (currentKeyInfo.keyPressCoroutine);
		}
		currentKeyInfo.keyPressCoroutine = StartCoroutine (rotatePressedKeyCoroutine (currentKeyInfo));
	}

	IEnumerator rotatePressedKeyCoroutine (keyInfo currentKeyInfo)
	{
		Quaternion targetRotation = Quaternion.Euler (new Vector3 (-keyRotationAmount, 0, 0));
		while (currentKeyInfo.keyTransform.localRotation != targetRotation) {
			currentKeyInfo.keyTransform.localRotation = Quaternion.Slerp (currentKeyInfo.keyTransform.localRotation, targetRotation, Time.deltaTime * keyRotationSpeed);
			yield return null;
		}
		targetRotation = Quaternion.identity;
		while (currentKeyInfo.keyTransform.localRotation != targetRotation) {
			currentKeyInfo.keyTransform.localRotation = Quaternion.Slerp (currentKeyInfo.keyTransform.localRotation, targetRotation, Time.deltaTime * keyRotationSpeed);
			yield return null;
		}
	}

	public void playSound (AudioClip clipSound)
	{
		if (mainAudioSource) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);
			mainAudioSource.PlayOneShot (clipSound);
		}
	}

	public void startOrStopUsingPiano ()
	{
		setUsingPianoState (!usingPiano);
	}

	public void setUsingPianoState (bool state)
	{
		usingPiano = state;
	}

	[System.Serializable]
	public class keyInfo
	{
		public string keyName;
		public AudioClip keySound;

		public Transform keyTransform;

		public Coroutine keyPressCoroutine;
	}
}
