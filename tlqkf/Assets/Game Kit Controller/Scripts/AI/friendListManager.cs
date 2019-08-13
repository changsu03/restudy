using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class friendListManager : MonoBehaviour
{
	public bool friendManagerEnabled;

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public gameManager mainGameManager;
	public Collider mainCollider;

	public GameObject friendsMenuContent;
	public GameObject friendListContent;
	public GameObject friendListElement;
	public Button attackButton;
	public Button followButton;
	public Button waitButton;
	public Button hideButton;
	public bool menuOpened;

	public string attackTargetOrderName = "Attacking";
	public string followTargetOrderName = "Following";
	public string waitOrderName = "Waiting";
	public string hideOrderName = "Hiding";

	public List<friendInfo> friendsList = new List<friendInfo> ();
	public List<string> tagToLocate = new List<string> ();
	public bool useBlurUIPanel = true;
	public bool usedByAI;

	List<GameObject> closestEnemyList = new List<GameObject> ();

	int i, j;
	AIHidePositionsManager hidePositionsManager;

	friendInfo currentFriendInfo;

	void Start ()
	{
		if (usedByAI) {
			return;
		}

		friendListElement.SetActive (false);
		friendsMenuContent.SetActive (false);
		hidePositionsManager = FindObjectOfType<AIHidePositionsManager> ();
	}

	public void openOrCloseFriendMenu (bool state)
	{
		if ((!pauseManager.playerMenuActive || menuOpened) && (!playerControllerManager.usingDevice || playerControllerManager.isPlayerDriving ()) && !mainGameManager.isGamePaused ()) {
			menuOpened = state;

			pauseManager.openOrClosePlayerMenu (menuOpened, friendsMenuContent.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState ("Friend List Manager", menuOpened);

			friendsMenuContent.SetActive (menuOpened);
			//set to visible the cursor
			pauseManager.showOrHideCursor (menuOpened);
			//disable the touch controls
			pauseManager.checkTouchControls (!menuOpened);
			//disable the camera rotation
			pauseManager.changeCameraState (!menuOpened);
			playerControllerManager.changeScriptState (!menuOpened);
			pauseManager.usingSubMenuState (menuOpened);

			if (playerControllerManager.isPlayerDriving ()) {
				playerControllerManager.getCurrentVehicle ().GetComponent<vehicleHUDManager> ().IKDrivingManager.setCameraAndWeaponsPauseState (menuOpened);
			}

			pauseManager.showOrHideMouseCursorController (menuOpened);
		}
	}

	public void openOrCLoseFriendMenuFromTouch ()
	{
		openOrCloseFriendMenu (!menuOpened);
	}

	public void addFriend (GameObject friend)
	{
		if (!checkIfContains (friend.transform)) {
			GameObject newFriendListElement = (GameObject)Instantiate (friendListElement, friendListElement.transform.position, Quaternion.identity);
			newFriendListElement.name = "New Friend List Element " + (friendsList.Count + 1).ToString ();

			friendInfo newFriend = newFriendListElement.GetComponent<friendListElement> ().friendListElementInfo;
			newFriend.Name = friend.GetComponent<health> ().settings.allyName;
			newFriend.friendTransform = friend.transform;

			newFriendListElement.SetActive (true);

			newFriend.friendRemoteEventSystem = friend.GetComponent<remoteEventSystem> ();

			newFriendListElement.transform.SetParent (friendListElement.transform.parent);
			newFriendListElement.transform.localScale = Vector3.one;
			newFriend.friendListElement = newFriendListElement;
			if (canAIAttack (friend)) {
				newFriend.attackButton.onClick.AddListener (() => {
					setIndividualOrder (newFriend.attackButton);
				});
			} else {
				newFriend.attackButton.gameObject.SetActive (false);
			}
			newFriend.followButton.onClick.AddListener (() => {
				setIndividualOrder (newFriend.followButton);
			});
			newFriend.waitButton.onClick.AddListener (() => {
				setIndividualOrder (newFriend.waitButton);
			});
			newFriend.hideButton.onClick.AddListener (() => {
				setIndividualOrder (newFriend.hideButton);
			});
			friendsList.Add (newFriend);

			setCurrentStateText (friendsList.Count - 1, "Following");
			setFriendListName ();
		}
	}

	public void setFriendListName ()
	{
		for (i = 0; i < friendsList.Count; i++) {
			friendsList [i].nameText.text = (i + 1).ToString () + ".- " + friendsList [i].Name;
		}
	}

	public bool checkIfContains (Transform friend)
	{
		bool itContains = false;
		for (i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				itContains = true;
			}
		}
		return itContains;
	}

	public void setIndividualOrder (Button pressedButton)
	{
		for (i = 0; i < friendsList.Count; i++) {

			currentFriendInfo = friendsList [i];
			Transform target = transform;
			string action = "";
			if (currentFriendInfo.attackButton == pressedButton) {
				if (canAIAttack (currentFriendInfo.friendTransform.gameObject)) {
					//print ("attack");
					Transform closestEnemy = getClosestEnemy ();
					target = closestEnemy;
					action = attackTargetOrderName;
				}
			} else if (currentFriendInfo.followButton == pressedButton) {
				//print ("follow");
				target = transform;
				action = followTargetOrderName;

			} else if (currentFriendInfo.waitButton == pressedButton) {
				//print ("wait");
				target = transform;
				action = waitOrderName;

			} else if (currentFriendInfo.hideButton == pressedButton) {
				//print ("hide");
				target = getClosestHidePosition (currentFriendInfo.friendTransform);
				action = hideOrderName;

			}

			if (target && action != "") {

				currentFriendInfo.friendRemoteEventSystem.callRemoveEventWithTransform (action, target);
				setCurrentStateText (i, action);
			}
		}
	}

	public void setGeneralOrder (Button pressedButton)
	{
		Transform target = transform;
		string action = "";
		if (attackButton == pressedButton) {
			//print ("attack");
			action = attackTargetOrderName;
			target = getClosestEnemy ();
		} else if (followButton == pressedButton) {
			//print ("follow");
			action = followTargetOrderName;
		} else if (waitButton == pressedButton) {
			//print ("wait");
			action = waitOrderName;
		} else if (hideButton == pressedButton) {
			//print ("hide");
			action = hideOrderName;
		}


		for (i = 0; i < friendsList.Count; i++) {
			currentFriendInfo = friendsList [i];

			bool canDoAction = true;
			if (action == attackTargetOrderName) {
				if (!canAIAttack (currentFriendInfo.friendTransform.gameObject)) {
					canDoAction = false;
				}
			}

			if (action == hideOrderName) {
				target = getClosestHidePosition (currentFriendInfo.friendTransform);
			}

			if (canDoAction) {
				if (target) {
					currentFriendInfo.friendRemoteEventSystem.callRemoveEventWithTransform (action, target);

					setCurrentStateText (i, action);
				}
			}
		}
	}

	public void callToFriends ()
	{
		setGeneralOrder (followButton);
	}

	public void findFriendsInRadius (float radius)
	{
		Collider[] colliders = Physics.OverlapSphere (transform.position, radius);
		if (colliders.Length > 0) {
			
			for (int i = 0; i < colliders.Length; i++) {
				findObjectivesSystem currentFindObjectivesSystem = colliders [i].GetComponent<findObjectivesSystem> ();
				if (currentFindObjectivesSystem) {
					currentFindObjectivesSystem.checkTriggerInfo (mainCollider, true);
				}
			}
		}
	}

	public bool canAIAttack (GameObject AIFriend)
	{
		bool canAttack = false;
		if (AIFriend.GetComponent<findObjectivesSystem> ().attackType != findObjectivesSystem.AIAttackType.none) {
			canAttack = true;
		}
		return canAttack;
	}

	public void setCurrentStateText (int index, string state)
	{
		friendsList [index].currentState.text = "State: " + state;
	}

	public Transform getClosestEnemy ()
	{
		List<GameObject> fullEnemyList = new List<GameObject> ();

		GameObject closestEnemy;
		for (int i = 0; i < tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLocate [i]);
			fullEnemyList.AddRange (enemiesList);
		}

		closestEnemyList.Clear ();
		for (j = 0; j < fullEnemyList.Count; j++) {
			if (!applyDamage.checkIfDead (fullEnemyList [j])) {
				closestEnemyList.Add (fullEnemyList [j]);
			}
		}

		if (closestEnemyList.Count > 0) {
			float distance = Mathf.Infinity;
			int index = -1;
			for (j = 0; j < closestEnemyList.Count; j++) {
				float currentDistance = GKC_Utils.distance (closestEnemyList [j].transform.position, transform.position);
				if (currentDistance < distance) {
					distance = currentDistance;
					index = j;
				}
			}
			if (index != -1) {
				closestEnemy = closestEnemyList [index];
				return closestEnemy.transform;
			}
		}
		return null;
	}

	public Transform getClosestHidePosition (Transform AIFriend)
	{
		if (hidePositionsManager) {
			if (hidePositionsManager.hidePositionList.Count > 0) {
				float distance = Mathf.Infinity;
				int index = -1;
				for (j = 0; j < hidePositionsManager.hidePositionList.Count; j++) {
					float currentDistance = GKC_Utils.distance (AIFriend.position, hidePositionsManager.hidePositionList [j].position);
					if (currentDistance < distance) {
						distance = currentDistance;
						index = j;
					}
				}
				return hidePositionsManager.hidePositionList [index];
			}
		}
		return null;
	}

	public void removeFriend (Transform friend)
	{
		for (i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				Destroy (friendsList [i].friendListElement);
				friendsList.RemoveAt (i);
				return;
			}
		}
	}

	public void inputOpenOrCloseFriendListMenu ()
	{
		if (friendManagerEnabled) {
			openOrCloseFriendMenu (!menuOpened);
		}
	}

	[System.Serializable]
	public class friendInfo
	{
		public string Name;
		public Transform friendTransform;
		public Text nameText;
		public Text currentState;
		public GameObject friendListElement;
		public Button attackButton;
		public Button followButton;
		public Button waitButton;
		public Button hideButton;

		public remoteEventSystem friendRemoteEventSystem;
	}
}