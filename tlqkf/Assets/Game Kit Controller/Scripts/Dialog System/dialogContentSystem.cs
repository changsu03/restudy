using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class dialogContentSystem : MonoBehaviour
{
	public List<completeDialogInfo> completeDialogInfoList = new List<completeDialogInfo> ();

	public int currentDialogIndex;

	public GameObject dialogOwner;

	public bool showDialogOnwerName;

	public bool dialogActive;

	GameObject currentPlayer;

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void activateDialog ()
	{
		dialogActive = !dialogActive;

		if (dialogActive) {
			playerController currentPlayerControllerManager = currentPlayer.GetComponent<playerController> ();

			dialogSystem currentDialogSystem = currentPlayerControllerManager.getPlayerManagersParentGameObject ().GetComponent<dialogSystem> ();

			if (currentDialogSystem) {
				currentDialogSystem.setNewDialogContent (this);
			}
		}
	}

	public void setNextCompleteDialogIndex ()
	{
		currentDialogIndex++;
	}

	public void setCompleteDialogIndex (int newIndex)
	{
		currentDialogIndex = newIndex;
	}

	public void addNewDialog ()
	{
		completeDialogInfo newCompleteDialogInfo = new completeDialogInfo ();

		newCompleteDialogInfo.ID = completeDialogInfoList.Count;

		completeDialogInfoList.Add (newCompleteDialogInfo);

		updateComponent ();
	}

	public void addNewLine (int dialogIndex)
	{
		dialogInfo newDialogInfo = new dialogInfo ();
		newDialogInfo.ID = completeDialogInfoList [dialogIndex].dialogInfoList.Count;

		completeDialogInfoList [dialogIndex].dialogInfoList.Add (newDialogInfo);

		updateComponent ();
	}

	public void addnewAnswer (int dialogIndex, int lineIndex)
	{
		dialogLineInfo newDialogLineInfo = new dialogLineInfo ();
		newDialogLineInfo.ID = completeDialogInfoList [dialogIndex].dialogInfoList [lineIndex].dialogLineInfoList.Count;

		completeDialogInfoList [dialogIndex].dialogInfoList [lineIndex].dialogLineInfoList.Add (newDialogLineInfo);

		updateComponent ();
	}


	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
}
