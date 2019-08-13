using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dialogSystem : MonoBehaviour
{
	public bool dialogEnabled = true;

	public GameObject playerGameObject;
	public usingDevicesSystem usingDevicesManager;

	public GameObject dialogPanel;

	public Text currentDialogOwnerNameText;

	public GameObject dialogContentWithoutOptions;
	public GameObject dialogContentWithOptions;

	public Text mainLineText;

	public Transform mainLineTextWithoutOptions;
	public Transform mainLineTextWithOptions;

	public GameObject dialogOptionsPanel;
	public RectTransform dialogOptionsPanelRectTransform;
	public Transform dialogOptionsParent;

	public GameObject nextLineButton;
	public GameObject closeDialogButton;

	public ScrollRect dialogOptionsScrollRect;
	public RectTransform dialogOptionsScrollRectRectTransform;

	public GameObject dialogOptionPrefab;

	public RectTransform dialogOptionsWithDialogLinePosition;
	public RectTransform dialogOptionsWithoutDialogLinePosition;

	public RectTransform dialogOptionsListSizeWithDialog;
	public RectTransform dialogOptionsListSizeWithoutDialog;

	public List<dialogPanelInfo> dialogPanelInfoList = new List<dialogPanelInfo> ();

	public dialogContentSystem currentDialogContentSystem;

	public int currentDialogContentIndex;

	public bool dialogActive;

	remoteEventSystem currentRemoteEventSystem;

	completeDialogInfo currentCompleteDialogInfo;

	List<dialogInfo> currentDialogInfoList = new List<dialogInfo> ();

	dialogLineInfo currentDialogLineInfo;

	dialogInfo currentDialogInfo;

	string previousDialogLine;

	void Start ()
	{
		dialogPanel.SetActive (false);
	}

	public void setNewDialogContent (dialogContentSystem newDialogContent)
	{
		usingDevicesManager.setUseDeviceButtonEnabledState (false);

		dialogActive = true;

		currentDialogContentSystem = newDialogContent;

		dialogPanel.SetActive (true);

		currentDialogContentIndex = 0;

		currentCompleteDialogInfo = currentDialogContentSystem.completeDialogInfoList [currentDialogContentSystem.currentDialogIndex];

		currentDialogInfoList = currentCompleteDialogInfo.dialogInfoList;

		currentDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		currentRemoteEventSystem = currentDialogContentSystem.dialogOwner.GetComponent<remoteEventSystem> ();

		updateDialogContent ();
	}

	public void setNextDialog ()
	{
		if (currentDialogInfo.disableDialogAfterSelect) {
			currentDialogInfoList [currentDialogContentIndex].dialogInfoDisabled = true;
		}

		currentDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		if (currentDialogInfo.changeToDialogInfoID) {
			
			if (currentDialogInfo.useRandomDialogInfoID) {
				int dialogContentIndex = 0;
				if (currentDialogInfo.useRandomDialogRange) {
					dialogContentIndex = (int)Random.Range ((float)currentDialogInfo.randomDialogRange.x, (float)currentDialogInfo.randomDialogRange.y);
				} else {
					bool valueFound = false;
					while (!valueFound) {
						dialogContentIndex = 
							(int)Random.Range ((float)currentDialogInfo.randomDialogIDList [0], (float)currentDialogInfo.randomDialogIDList [currentDialogInfo.randomDialogIDList.Count - 1] + 1);

						if (currentDialogInfo.randomDialogIDList.Contains (dialogContentIndex)) {
							valueFound = true;

							if (dialogContentIndex > currentDialogInfo.randomDialogIDList [currentDialogInfo.randomDialogIDList.Count - 1]) {
								dialogContentIndex--;
							}
						}
					}
				}

				currentDialogContentIndex = dialogContentIndex;

			} else {
				currentDialogContentIndex = currentDialogInfoList [currentDialogContentIndex].dialogInfoIDToActivate;
			}
				
			if (currentDialogInfoList [currentDialogContentIndex].dialogInfoDisabled) {
				currentDialogContentIndex = currentDialogInfoList [currentDialogContentIndex].dialogInfoIDToJump;
			}

			updateDialogContent ();

			return;
		} 

		currentDialogContentIndex++;

		if (currentDialogContentIndex < currentDialogInfoList.Count) {
			updateDialogContent ();
		} else {
			closeDialog ();
		}
	}

	public void closeDialog ()
	{
		usingDevicesManager.setUseDeviceButtonEnabledState (true);

		dialogPanel.SetActive (false);

		usingDevicesManager.useDevice ();

		if (currentDialogInfoList [currentDialogContentIndex].activateWhenDialogClosed) {
			checkDialogEvents ();
		}

		dialogActive = false;
	}

	public void setDialogContentAnswer (Button buttonToCheck)
	{
		for (int i = 0; i < currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList.Count; i++) {
			currentDialogLineInfo = currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i];
			if (currentDialogLineInfo.dialogLineButton == buttonToCheck) {

				if (currentDialogInfoList [currentDialogContentIndex].disableDialogAfterSelect) {
					currentDialogInfoList [currentDialogContentIndex].dialogInfoDisabled = true;
				}

				if (currentDialogLineInfo.useRandomDialogInfoID) {
					int dialogContentIndex = -1;
					if (currentDialogLineInfo.useRandomDialogRange) {
						dialogContentIndex = (int)Random.Range ((float)currentDialogLineInfo.randomDialogRange.x, (float)currentDialogLineInfo.randomDialogRange.y);
					} else {
						bool valueFound = false;

						while (!valueFound) {
							dialogContentIndex = 
							(int)Random.Range ((float)currentDialogLineInfo.randomDialogIDList [0], 
								(float)currentDialogLineInfo.randomDialogIDList [currentDialogLineInfo.randomDialogIDList.Count - 1] + 1);

							if (currentDialogLineInfo.randomDialogIDList.Contains (dialogContentIndex)) {
								valueFound = true;

								if (dialogContentIndex > currentDialogLineInfo.randomDialogIDList [currentDialogLineInfo.randomDialogIDList.Count - 1]) {
									dialogContentIndex--;
								}
							}
						}
					}
					currentDialogContentIndex = dialogContentIndex;
				
				} else {
					currentDialogContentIndex = currentDialogLineInfo.dialogInfoIDToActivate;
				}

				if (currentDialogLineInfo.disableLineAfterSelect) {
					currentDialogLineInfo.lineDisabled = true;
				}

				updateDialogContent ();
			}
		}
	}

	public void checkDialogEvents ()
	{
		currentDialogInfoList [currentDialogContentIndex].eventOnDialog.Invoke ();

		if (currentDialogInfoList [currentDialogContentIndex].activateRemoteTriggerSystem) {
			currentRemoteEventSystem.callRemoveEventWithGameObject (currentDialogInfoList [currentDialogContentIndex].remoteTriggerName, playerGameObject);
		}
	}

	public void updateDialogContent ()
	{
		int currentOptionsAmount = currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList.Count;
	
		bool currentLineHasOptions = currentOptionsAmount > 0;

		mainLineText.text = currentDialogInfoList [currentDialogContentIndex].dialogContent;

		if (currentLineHasOptions) {
			mainLineText.transform.position = mainLineTextWithOptions.position;
		} else {
			mainLineText.transform.position = mainLineTextWithoutOptions.position;
		}

		dialogContentWithoutOptions.SetActive (!currentLineHasOptions);
		dialogContentWithOptions.SetActive (currentLineHasOptions);

		if (currentLineHasOptions) {
			if (currentDialogInfoList [currentDialogContentIndex].showPreviousDialogLineOnOptions || currentDialogInfoList [currentDialogContentIndex].dialogContent != "") {
				if (currentDialogInfoList [currentDialogContentIndex].showPreviousDialogLineOnOptions) {
					mainLineText.text = previousDialogLine;
				}

				dialogOptionsPanelRectTransform.anchoredPosition = dialogOptionsWithDialogLinePosition.anchoredPosition;
				dialogOptionsPanelRectTransform.sizeDelta = dialogOptionsWithDialogLinePosition.sizeDelta;

				dialogOptionsScrollRectRectTransform.anchoredPosition = dialogOptionsListSizeWithDialog.anchoredPosition;
				dialogOptionsScrollRectRectTransform.sizeDelta = dialogOptionsListSizeWithDialog.sizeDelta;
			} else {
				dialogOptionsPanelRectTransform.anchoredPosition = dialogOptionsWithoutDialogLinePosition.anchoredPosition;
				dialogOptionsPanelRectTransform.sizeDelta = dialogOptionsWithoutDialogLinePosition.sizeDelta;

				dialogOptionsScrollRectRectTransform.anchoredPosition = dialogOptionsListSizeWithoutDialog.anchoredPosition;
				dialogOptionsScrollRectRectTransform.sizeDelta = dialogOptionsListSizeWithoutDialog.sizeDelta;
			}
		}

		if (currentDialogContentSystem.showDialogOnwerName) {
			currentDialogOwnerNameText.text = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
		}

		if (currentDialogInfoList [currentDialogContentIndex].isEndOfDialog) {
			closeDialogButton.SetActive (true);
			nextLineButton.SetActive (false);
		} else {
			if (currentDialogInfoList [currentDialogContentIndex].useNexLineButton) {
				nextLineButton.SetActive (true);
			} else {
				nextLineButton.SetActive (false);
			}

			closeDialogButton.SetActive (false);
		}

		if (currentLineHasOptions) {
			dialogOptionsPanel.SetActive (true);

			if (dialogPanelInfoList.Count < currentOptionsAmount) {
				int newAmountOfOptions = currentOptionsAmount - dialogPanelInfoList.Count;

				for (int i = 0; i < newAmountOfOptions; i++) {
					GameObject newDialogOption = (GameObject)Instantiate (dialogOptionPrefab, Vector3.zero, Quaternion.identity);
					newDialogOption.SetActive (true);
					newDialogOption.transform.SetParent (dialogOptionsParent);
					newDialogOption.transform.localScale = Vector3.one;
					newDialogOption.transform.localPosition = Vector3.zero;

					dialogPanelInfo newDialogPanelInfo = new dialogPanelInfo ();

					newDialogPanelInfo.dialogOptionGameObject = newDialogOption;
					newDialogPanelInfo.dialogOptionButton = newDialogOption.GetComponent<Button> ();
					newDialogPanelInfo.dialogOptionText = newDialogOption.GetComponentInChildren<Text> ();

					dialogPanelInfoList.Add (newDialogPanelInfo);
				}

				for (int i = 0; i < dialogPanelInfoList.Count; i++) {
					dialogPanelInfoList [i].dialogOptionGameObject.SetActive (true);

				}
			} else if (dialogPanelInfoList.Count > currentOptionsAmount) {
				for (int i = 0; i < dialogPanelInfoList.Count; i++) {
					if (i > currentOptionsAmount) {
						dialogPanelInfoList [i].dialogOptionGameObject.SetActive (false);
					}
				}
			}

			for (int i = 0; i < currentOptionsAmount; i++) {
				currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].dialogLineButton = dialogPanelInfoList [i].dialogOptionButton;
				dialogPanelInfoList [i].dialogOptionText.text = currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].dialogLineContent;
				dialogPanelInfoList [i].dialogOptionGameObject.SetActive (true);
			}

			for (int i = currentOptionsAmount; i < dialogPanelInfoList.Count; i++) {
				dialogPanelInfoList [i].dialogOptionGameObject.SetActive (false);
			}
				
		} else {
			dialogOptionsPanel.SetActive (false);
		}

		for (int i = 0; i < currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList.Count; i++) {
			if (currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].lineDisabled) {
				if (currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].dialogLineButton) {
					currentDialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].dialogLineButton.gameObject.SetActive (false);
				}
			}
		}

		dialogOptionsScrollRect.verticalNormalizedPosition = 0f;
		dialogOptionsScrollRect.horizontalNormalizedPosition = 0.5f;
		dialogOptionsScrollRect.horizontalNormalizedPosition = 0.5f;

		if (!currentDialogInfoList [currentDialogContentIndex].activateWhenDialogClosed) {
			checkDialogEvents ();
		}

		previousDialogLine = currentDialogInfoList [currentDialogContentIndex].dialogContent;
	}

	[System.Serializable]
	public class dialogPanelInfo
	{
		public string Name;

		public GameObject dialogOptionGameObject;
		public Button dialogOptionButton;
		public Text dialogOptionText;

	}
}
