using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointElement : MonoBehaviour
{
	public int checkpointID;

	public bool overwriteThisCheckpoint;

	public bool useCustomSaveTransform;
	public Transform customSaveTransform;

	public bool useCustomCameraTransform;
	public Transform customCameraTransform;
	public Transform customCameraPivotTransform;

	public List<string> tagToSave = new List<string> ();
	public bool saveInEveryTriggerEnter;

	public bool checkpointAlreadyFound;
	public checkpointSystem checkpointManager;
	public Collider mainCollider;

	void Awake ()
	{
		StartCoroutine (activateTriggers ());
	}

	IEnumerator activateTriggers ()
	{
		if (mainCollider) {
			mainCollider.enabled = false;
			yield return new WaitForSeconds (1);
			mainCollider.enabled = true;
		}
	}

	public void setCheckPointManager (checkpointSystem manager)
	{
		checkpointManager = manager;
	}

	public void OnTriggerEnter (Collider col)
	{
		if ((!checkpointAlreadyFound || saveInEveryTriggerEnter) && tagToSave.Contains (col.tag)) {
			checkpointAlreadyFound = true;
			playerController currentPlayerController = col.gameObject.GetComponent<playerController> ();
			GameObject playerManagersParentGameObject = currentPlayerController.getPlayerManagersParentGameObject ();
			if (useCustomSaveTransform) {
				playerManagersParentGameObject.GetComponent<saveGameSystem> ().saveGameCheckpoint (customSaveTransform, checkpointID, checkpointManager.checkpointSceneID, overwriteThisCheckpoint);
			} else {
				playerManagersParentGameObject.GetComponent<saveGameSystem> ().saveGameCheckpoint (null, checkpointID, checkpointManager.checkpointSceneID, overwriteThisCheckpoint);
			}

			checkpointManager.setCurrentCheckpointElement (transform);
		}
	}
}
