using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class completeDialogInfo
{
	public string Name;

	public int ID;

	public List<dialogInfo> dialogInfoList = new List<dialogInfo> ();
}

[System.Serializable]
public class dialogInfo
{
	public string Name;

	public int ID;

	public string dialogOwnerName;

	[TextArea (3, 10)] public string dialogContent;

	public bool showPreviousDialogLineOnOptions;

	public List<dialogLineInfo> dialogLineInfoList = new List<dialogLineInfo> ();

	public UnityEvent eventOnDialog;

	public bool activateWhenDialogClosed;
	public bool activateRemoteTriggerSystem;
	public string remoteTriggerName;

	public bool useNexLineButton = true;

	public bool isEndOfDialog;

	public bool changeToDialogInfoID;
	public int dialogInfoIDToActivate;

	public bool useRandomDialogInfoID;
	public bool useRandomDialogRange;
	public Vector2 randomDialogRange;
	public List<int> randomDialogIDList = new List<int> ();

	public bool disableDialogAfterSelect;
	public int dialogInfoIDToJump;
	public bool dialogInfoDisabled;
}

[System.Serializable]
public class dialogLineInfo
{
	public string Name;

	public int ID;
	[TextArea (3, 10)] public string dialogLineContent;

	public int dialogInfoIDToActivate;

	public bool useRandomDialogInfoID;
	public bool useRandomDialogRange;
	public Vector2 randomDialogRange;
	public List<int> randomDialogIDList = new List<int> ();

	public bool activateRemoteTriggerSystem;
	public string remoteTriggerName;

	public Button dialogLineButton;

	public bool disableLineAfterSelect;

	public bool lineDisabled;
}