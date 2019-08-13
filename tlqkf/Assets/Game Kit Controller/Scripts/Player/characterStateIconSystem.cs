using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterStateIconSystem : MonoBehaviour
{

	public List<characterStateInfo> characterStateInfoList = new List<characterStateInfo> ();
	public AudioSource mainAudioSource;
	public bool hideAfterTime;
	public characterStateInfo currentCharacterState;
	public playerController playerControllerManager;
	public Transform mainCameraTransform;
	Vector3 directionToCamera;
	Transform currentIconGameObjectThirdPerson;

	void Start ()
	{
		if (mainCameraTransform == null) {
			mainCameraTransform = FindObjectOfType<gameManager> ().getMainCamera ().transform;
		}
	}

	public void enableOrDisableCharacterIcon (characterStateInfo characterState, bool state)
	{
		if (playerControllerManager != null) {
			if (playerControllerManager.isPlayerOnFirstPerson () || playerControllerManager.isUsingDevice ()) {
				characterState.iconGameObjectThirdPerson.SetActive (false);
				if (characterState.iconGameObjectFirstPerson != null) {
					characterState.iconGameObjectFirstPerson.SetActive (state);
				}
			} else {
				characterState.iconGameObjectThirdPerson.SetActive (state);
				if (characterState.iconGameObjectFirstPerson != null) {
					characterState.iconGameObjectFirstPerson.SetActive (false);
				}
			}
		} else {
			if (characterState.iconGameObjectFirstPerson != null) {
				characterState.iconGameObjectFirstPerson.SetActive (state);
			}
			characterState.iconGameObjectThirdPerson.SetActive (state);
		}
	}

	public void checkCharacterStateIconForViewChange ()
	{
		if (currentIconGameObjectThirdPerson != null) {
			enableOrDisableCharacterIcon (currentCharacterState, true);
		}
	}

	void Update ()
	{
		if (hideAfterTime) {
			if (Time.time > currentCharacterState.lastTimeHidden + currentCharacterState.hideAfterTimeAmount) {
				//print ("remove icon");
				enableOrDisableCharacterIcon (currentCharacterState, false);
				hideAfterTime = false;
				currentCharacterState = null;
				currentIconGameObjectThirdPerson = null;
			}
		}

		if (currentIconGameObjectThirdPerson != null) {
			if (playerControllerManager != null) {
				if (!playerControllerManager.isPlayerOnFirstPerson ()) {
					directionToCamera = currentIconGameObjectThirdPerson.position - mainCameraTransform.position;
					currentIconGameObjectThirdPerson.rotation = Quaternion.LookRotation (directionToCamera);
				}
			} else {
				directionToCamera = currentIconGameObjectThirdPerson.position - mainCameraTransform.position;
				currentIconGameObjectThirdPerson.rotation = Quaternion.LookRotation (directionToCamera);
			}
		}
	}

	public void setCharacterStateIcon (string stateName)
	{
		//print ("current state "+stateName);
		for (int i = 0; i < characterStateInfoList.Count; i++) {
			if (characterStateInfoList [i].Name == stateName) {
				currentCharacterState = characterStateInfoList [i];

				currentIconGameObjectThirdPerson = currentCharacterState.iconGameObjectThirdPerson.transform;

				enableOrDisableCharacterIcon (currentCharacterState, true);

				if (currentCharacterState.hideAfterTime) {
					currentCharacterState.lastTimeHidden = Time.time;
					hideAfterTime = true;
				} else {
					hideAfterTime = false;
				}

				if (currentCharacterState.useSound) {
					playSound (currentCharacterState.stateClip);
				}
			} else {
				enableOrDisableCharacterIcon (characterStateInfoList [i], false);
			}
		}
	}

	public void disableCharacterStateIcon ()
	{
		//print ("disable states icon");
		for (int i = 0; i < characterStateInfoList.Count; i++) {
			enableOrDisableCharacterIcon (characterStateInfoList [i], false);
		}
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource) {
			mainAudioSource.PlayOneShot (sound);
		}
	}

	[System.Serializable]
	public class characterStateInfo
	{
		public string Name;
		public GameObject iconGameObjectThirdPerson;
		public GameObject iconGameObjectFirstPerson;
		public bool hideAfterTime;
		public float hideAfterTimeAmount;
		public float lastTimeHidden;
		public bool useSound;
		public AudioClip stateClip;
	}
}
