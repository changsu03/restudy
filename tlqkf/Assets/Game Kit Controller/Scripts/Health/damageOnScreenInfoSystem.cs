using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class damageOnScreenInfoSystem : MonoBehaviour
{
	public List<targetInfo> targetInfoList = new List<targetInfo> ();

	public List<playerDamageOnScreenInfoSystem> playerDamageOnScreenInfoSystemList = new List<playerDamageOnScreenInfoSystem> ();

	int currentID = 0;

	public void addNewPlayer (playerDamageOnScreenInfoSystem newPlayer)
	{
		newPlayer.damageOnScreenId = currentID;
		playerDamageOnScreenInfoSystemList.Add (newPlayer);

		currentID++;
	}

	public int addNewTarget (Transform newTarget)
	{
		targetInfo newTargetInfo = new targetInfo ();
		newTargetInfo.Name = newTarget.name;
		newTargetInfo.target = newTarget;
		newTargetInfo.ID = currentID;
		targetInfoList.Add (newTargetInfo);

		for (int i = 0; i < playerDamageOnScreenInfoSystemList.Count; i++) {
			playerDamageOnScreenInfoSystemList [i].addNewTarget (newTargetInfo);
		}

		currentID++;

		return currentID - 1;
	}

	public void setDamageInfo (int objectId, float amount, bool isDamage, Vector3 direction, float healhtAmount)
	{
		for (int j = 0; j < targetInfoList.Count; j++) {
			if (targetInfoList [j].ID == objectId) {
				for (int i = 0; i < playerDamageOnScreenInfoSystemList.Count; i++) {
					playerDamageOnScreenInfoSystemList [i].setDamageInfo (j, amount, isDamage, direction, healhtAmount);
				}
				return;
			}
		}
	}

	//if the target is reached, disable all the parameters and clear the list, so a new objective can be added in any moment
	public void removeElementFromList (int objectId)
	{
		for (int j = 0; j < targetInfoList.Count; j++) {
			if (targetInfoList [j].ID == objectId) {
				for (int i = 0; i < playerDamageOnScreenInfoSystemList.Count; i++) {
					playerDamageOnScreenInfoSystemList [i].removeElementFromList (objectId);
				}
				targetInfoList.RemoveAt (j);

				return;
			}
		}
	}

	public void removeElementFromObjectiveListCalledByPlayer (int objectId, GameObject currentPlayer)
	{
		for (int i = 0; i < targetInfoList.Count; i++) {
			if (targetInfoList [i].ID == objectId) {
				targetInfoList.Remove (targetInfoList [i]);

				for (int j = 0; j < playerDamageOnScreenInfoSystemList.Count; j++) {
					if (playerDamageOnScreenInfoSystemList [j].player != currentPlayer) {
						playerDamageOnScreenInfoSystemList [j].removeElementFromList (objectId);
					}
				}

				return;
			}
		}
	}


	[System.Serializable]
	public class targetInfo
	{
		public string Name;
		public Transform target;
		public int ID;
		public RectTransform targetRectTransform;
		public GameObject targetRectTransformGameObject;

		public bool isDead;
		public bool containsNumberToShow;
		public List<damageNumberInfo> damageNumberInfoList = new List<damageNumberInfo> ();
	}

	[System.Serializable]
	public class damageNumberInfo
	{
		public Text damageNumberText;
		public RectTransform damageNumberRectTransform;
		public Coroutine movementCoroutine;
	}
}
