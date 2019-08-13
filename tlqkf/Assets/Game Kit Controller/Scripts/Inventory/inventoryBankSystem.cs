using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class inventoryBankSystem : MonoBehaviour
{
	public bool usingInventoryBank;

	public float openBankDelay;

	public string animationName;
	public AudioClip openSound;
	public AudioClip closeSound;

	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryInfo> bankInventoryList = new List<inventoryInfo> ();

	public string[] inventoryManagerListString;

	public bool useInventoryFromThisBank;

	public bool attachToTransformActive;
	public Transform transformToAttach;
	public Vector3 localOffset;

	public playerController playerControllerManager;
	GameObject currentPlayer;

	inventoryListManager mainInventoryManager;

	inventoryManager currentPlayerInventoryManager;
	Animation mainAnimation;
	Coroutine openCoroutine;
	bool firstAnimationPlay = true;
	AudioSource mainAudioSource;

	usingDevicesSystem usingDevicesManager;

	void Start ()
	{
		mainAnimation = GetComponent<Animation> ();
		mainAudioSource = GetComponent<AudioSource> ();

		mainInventoryManager = FindObjectOfType<inventoryListManager> ();
		setInventoryFromInventoryListManager ();

		if (attachToTransformActive && transformToAttach) {
			transform.SetParent (transformToAttach);
			transform.localPosition = Vector3.zero + localOffset;
		}
	}

	public void setUsingInventoryBankState (bool state)
	{
		usingInventoryBank = state;
	}

	public void activateInventoryBank ()
	{
		if (openCoroutine != null) {
			StopCoroutine (openCoroutine);
		}
		openCoroutine = StartCoroutine (openOrCloseInventoryBank ());
	}

	IEnumerator openOrCloseInventoryBank ()
	{
		usingInventoryBank = !usingInventoryBank;
		playAnimation (usingInventoryBank);
		playSound (usingInventoryBank);

		playerControllerManager.setUsingDeviceState (usingInventoryBank);

		if (usingInventoryBank) {
			yield return new WaitForSeconds (openBankDelay);
		}
			
		bool cancelOpenInventoryBank = false;
		if (usingInventoryBank) {
			currentPlayerInventoryManager.setCurrentInventoryBankSystem (gameObject);

			if (usingDevicesManager) {
				if (!usingDevicesManager.existInDeviceList (gameObject)) {
					activateInventoryBank ();
					cancelOpenInventoryBank = true;
				}
			}
		}

		if (!cancelOpenInventoryBank) {
			currentPlayerInventoryManager.openOrCloseInventoryBankMenu (usingInventoryBank);
		}
	}

	public void playAnimation (bool playForward)
	{
		if (mainAnimation) {
			if (playForward) {
				if (!mainAnimation.IsPlaying (animationName)) {
					mainAnimation [animationName].normalizedTime = 0;
				}
				mainAnimation [animationName].speed = 1;
			} else {
				if (!mainAnimation.IsPlaying (animationName)) {
					mainAnimation [animationName].normalizedTime = 1;
				}
				mainAnimation [animationName].speed = -1; 
			}
			if (firstAnimationPlay) {
				mainAnimation.Play (animationName);
				firstAnimationPlay = false;
			} else {
				mainAnimation.CrossFade (animationName);
			}
		}
	}

	public void playSound (bool state)
	{
		if (mainAudioSource) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);
			if (state) {
				mainAudioSource.PlayOneShot (openSound);
			} else {
				mainAudioSource.PlayOneShot (closeSound);
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		currentPlayerInventoryManager = currentPlayer.GetComponent<inventoryManager> ();
		playerControllerManager = currentPlayer.GetComponent<playerController> ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
	}

	public void updateFullInventorySlots (List<inventoryInfo> currentInventoryList, List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryList.Count; i++) {
			inventoryInfo currentInventoryInfo = currentInventoryList [i];
			currentInventoryInfo.menuIconElement = currentIconElementList [i];
			currentInventoryInfo.button = currentIconElementList [i].button;
			currentInventoryInfo.menuIconElement.iconName.text = currentInventoryInfo.Name;
			currentInventoryInfo.menuIconElement.icon.texture = currentInventoryInfo.icon;
			currentInventoryInfo.menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();
		}
	}

	public void createInventoryIcons (List<inventoryInfo> currentInventoryListToCreate, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                  List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			if (currentInventoryListToCreate [i].button != null) {
				Destroy (currentInventoryListToCreate [i].button.gameObject);
			}
		}
		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			createInventoryIcon (currentInventoryListToCreate [i], i, currentInventorySlot, currentInventorySlotsContent, currentIconElementList);
		}
	}

	public void createInventoryIcon (inventoryInfo currentInventoryInfo, int index, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                 List<inventoryMenuIconElement> currentIconElementList)
	{
		GameObject newIconButton = (GameObject)Instantiate (currentInventorySlot, Vector3.zero, Quaternion.identity);
		newIconButton.SetActive (true);
		newIconButton.transform.SetParent (currentInventorySlotsContent.transform);
		newIconButton.transform.localScale = Vector3.one;
		newIconButton.transform.localPosition = Vector3.zero;
		inventoryMenuIconElement menuIconElement = newIconButton.GetComponent<inventoryMenuIconElement> ();
		menuIconElement.iconName.text = currentInventoryInfo.Name;
		if (currentInventoryInfo.inventoryGameObject != null) {
			menuIconElement.icon.texture = currentInventoryInfo.icon;
		} else {
			menuIconElement.icon.texture = null;
		}
		menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();
		menuIconElement.pressedIcon.SetActive (false);
		newIconButton.name = "Inventory Object-" + (index + 1).ToString ();
		Button button = menuIconElement.button;
		currentInventoryInfo.button = button;
		currentInventoryInfo.menuIconElement = menuIconElement;
		currentIconElementList.Add (menuIconElement);
	}


	public void setInventoryFromInventoryListManager ()
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			inventoryInfo currentInventoryInfo = mainInventoryManager.inventoryList [inventoryListManagerList [i].elementIndex];
			if (currentInventoryInfo != null) {
				inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
				newInventoryInfo.Name = currentInventoryInfo.Name;
				newInventoryInfo.amount = inventoryListManagerList [i].amount;
				bankInventoryList.Add (newInventoryInfo);
			}
		}

		updateComponent ();
	}

	public void getInventoryListManagerList ()
	{
		if (!mainInventoryManager) {
			mainInventoryManager = FindObjectOfType<inventoryListManager> ();
		} 
		if (mainInventoryManager) {
			inventoryManagerListString = new string[mainInventoryManager.inventoryList.Count];
			for (int i = 0; i < inventoryManagerListString.Length; i++) {
				inventoryManagerListString [i] = mainInventoryManager.inventoryList [i].Name;
			}

			updateComponent ();
		}
	}

	public void addNewInventoryObjectToInventoryListManagerList ()
	{
		inventoryListElement newInventoryListElement = new inventoryListElement ();
		newInventoryListElement.name = "New Object";
		inventoryListManagerList.Add (newInventoryListElement);

		updateComponent ();
	}

	public List<inventoryInfo> getBankInventoryList ()
	{
		return bankInventoryList;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<inventoryBankSystem> ());
		#endif
	}
}