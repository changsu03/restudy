using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class useInventoryObject : MonoBehaviour
{
	public bool canBeReUsed;
	public useInventoryObjectType useInventoryType;

	public bool useObjectsOneByOneUsingButton = true;

	public string inventoryObjectAction;

	[TextArea (3, 10)]
	public string objectUsedMessage;
	public bool enableObjectWhenActivate;
	public GameObject objectToEnable;
	public bool useAnimation;
	public GameObject objectWithAnimation;
	public string animationName;

	public List<inventoryObjectNeededInfo> inventoryObjectNeededList = new List<inventoryObjectNeededInfo> ();

	public UnityEvent unlockFunctionCall = new UnityEvent ();

	public bool disableObjectActionAfterUse;

	public bool objectUsed;

	public int numberOfObjectsUsed;
	public int numberOfObjectsNeeded;

	public int currentNumberOfObjectsNeeded;

	public enum useInventoryObjectType
	{
		menu,
		button,
		automatic
	}

	public string tagToConfigure = "device";

	GameObject currentPlayer;
	inventoryManager playerInventoryManager;
	usingDevicesSystem usingDevicesManager;

	string previousAction;
	string currentObjectUsedMessage;
	deviceStringAction deviceStringActionManager;
	GameObject currentObjectToUse;
	AudioSource mainAudioSource;
	Collider mainCollider;

	bool canBeUsed = true;

	int currentAmountUsed;

	void Start ()
	{
		deviceStringActionManager = GetComponent<deviceStringAction> ();
		if (deviceStringActionManager) {
			previousAction = deviceStringActionManager.deviceAction;
		}
		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			numberOfObjectsNeeded += inventoryObjectNeededList [i].amountNeeded;
		}	
		mainAudioSource = GetComponent<AudioSource> ();
		mainCollider = GetComponent<Collider> ();

		if (useInventoryType == useInventoryObjectType.button) {
			gameObject.tag = tagToConfigure;
		}
	}

	public void setCanBeUsedState (bool state)
	{
		canBeUsed = state;
	}

	public bool getInventoryObjectCanBeUsed ()
	{
		return canBeUsed;
	}

	public void activateDevice ()
	{
		if (useInventoryType == useInventoryObjectType.button) {
			if (!objectUsed) {
				playerInventoryManager.useCurrentObject ();
			}
		}
	}

	//When player uses an inventory object, this can happen:
	//-The option is menu
	//--Only one object can be used at the same time
	//-The option is button
	//--The player has all the objects needed
	//--The player has some objects needed
	//-The option is automatic
	//--The player has all the objects needed
	//---All inventory objects are used
	//--The player has some objects needed
	//---Those available objects are used

	public void useObject (int amountUsed)
	{
		if (!objectUsed) {

			for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
				if (!inventoryObjectNeededList [i].objectUsed && currentObjectToUse == inventoryObjectNeededList [i].objectNeeded) {
	
					inventoryObjectNeededList [i].amountOfObjectsUsed += amountUsed;
					if (inventoryObjectNeededList [i].amountOfObjectsUsed == inventoryObjectNeededList [i].amountNeeded) {
						inventoryObjectNeededList [i].objectUsed = true;

						if (inventoryObjectNeededList [i].useEventOnObjectsPlaced) {
							inventoryObjectNeededList [i].eventOnObjectsPlaced.Invoke ();
						}
					}

					currentAmountUsed = amountUsed;

					numberOfObjectsUsed += amountUsed;

					for (int j = 0; j < inventoryObjectNeededList [i].inventoryObjectNeededList.Count; j++) {
						if (j < inventoryObjectNeededList [i].amountOfObjectsUsed) {
							if (!inventoryObjectNeededList [i].inventoryObjectNeededList [j].objectActivated) {
								inventoryObjectNeededList [i].inventoryObjectNeededList [j].objectActivated = true;

								if (inventoryObjectNeededList [i].inventoryObjectNeededList [j].instantiateObject) {
									Instantiate (inventoryObjectNeededList [i].objectNeeded, 
										inventoryObjectNeededList [i].inventoryObjectNeededList [j].placeForObject.position, 
										inventoryObjectNeededList [i].inventoryObjectNeededList [j].placeForObject.rotation);
								} else if (inventoryObjectNeededList [i].inventoryObjectNeededList [j].enableObject) {
									inventoryObjectNeededList [i].inventoryObjectNeededList [j].objectToEnable.SetActive (true);
								}

								if (inventoryObjectNeededList [i].inventoryObjectNeededList [j].useEventOnObjectPlaced) {
									inventoryObjectNeededList [i].inventoryObjectNeededList [j].eventOnObjectPlaced.Invoke ();
								}

								if (inventoryObjectNeededList [i].inventoryObjectNeededList [j].useAnimation) {
									inventoryObjectNeededList [i].inventoryObjectNeededList [j].objectWithAnimation.GetComponent<Animation> ().Play (
										inventoryObjectNeededList [i].inventoryObjectNeededList [j].animationName);
								}
							}
						}
					}

					currentNumberOfObjectsNeeded = inventoryObjectNeededList [i].amountNeeded - inventoryObjectNeededList [i].amountOfObjectsUsed;
					currentObjectUsedMessage = inventoryObjectNeededList [i].objectUsedMessage;
					if (inventoryObjectNeededList [i].useObjectSound) {
						playObjectUsedSound (i);
					}
				}
			}
				
			if (numberOfObjectsUsed >= numberOfObjectsNeeded) {
				solveThisInventoryObject ();
			} 
		}
	}

	public void solveThisInventoryObject ()
	{
		currentObjectUsedMessage = currentObjectUsedMessage + "\n" + objectUsedMessage;

		if (unlockFunctionCall.GetPersistentEventCount () > 0) {
			unlockFunctionCall.Invoke ();
		}

		if (deviceStringActionManager) {
			if (disableObjectActionAfterUse) {
				deviceStringActionManager.showIcon = false;
				removeDeviceFromList ();
			} else {
				deviceStringActionManager.setDeviceAction (previousAction);
			}
		}

		if (useAnimation) {
			objectWithAnimation.GetComponent<Animation> ().Play (animationName);
		}

		if (!canBeReUsed) {
			objectUsed = true;
		}

		if (enableObjectWhenActivate) {
			objectToEnable.SetActive (true);
		}

		if (useAnimation) {
			objectWithAnimation.GetComponent<Animation> ().Play (animationName);
		}
	}

	public void updateUseInventoryObjectState ()
	{
		if (canBeReUsed) {
			if (canBeUsed) {
				selectObjectOnInventory ();
			} else {
				removePlayerInventoryInfo ();
			}
		} else {
			if (!objectUsed) {
				if (useInventoryType == useInventoryObjectType.button) {
					selectObjectOnInventory ();
					if (!useObjectsOneByOneUsingButton) {
						if (currentObjectToUse != null) {
							playerInventoryManager.useCurrentObject ();
						}
					}
				} else if (useInventoryType == useInventoryObjectType.menu) {
					setInfoCurrentInventoryObjectToUse ();
				} else if (useInventoryType == useInventoryObjectType.automatic) {
					selectObjectOnInventory ();
					if (currentObjectToUse != null) {
						playerInventoryManager.useCurrentObject ();
					}
				}
			}
		}
	}

	//set the object needed to be used from the current player inside the trigger of this use inventory object
	public void selectObjectOnInventory ()
	{
		if (deviceStringActionManager) {
			if (inventoryObjectAction != "") {
				deviceStringActionManager.setDeviceAction (inventoryObjectAction);
			}
		}

		setInfoCurrentInventoryObjectToUse ();

		//check the options button and automatic to use correctly the inventory objects
		if (useInventoryType == useInventoryObjectType.button) {
			useObjectByButton ();
		} else if (useInventoryType == useInventoryObjectType.menu) {
			useObjectByMenu ();
		} else if (useInventoryType == useInventoryObjectType.automatic) {
			useObjectAutomatically ();
		}
	}

	public void useObjectByButton ()
	{
		playerInventoryManager.setCurrenObjectByPrefab (currentObjectToUse);
		playerInventoryManager.setCurrentUseInventoryGameObject (gameObject);
	}

	public void useObjectByMenu ()
	{
		playerInventoryManager.setCurrentUseInventoryGameObject (gameObject);
	}

	public void useObjectAutomatically ()
	{
		playerInventoryManager.setCurrenObjectByPrefab (currentObjectToUse);
		playerInventoryManager.searchForObjectNeed (gameObject);
	}

	public void setInfoCurrentInventoryObjectToUse ()
	{
		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			if (!inventoryObjectNeededList [i].objectUsed) {
				if (playerInventoryManager.inventoryContainsObject (inventoryObjectNeededList [i].objectNeeded)) {
					currentObjectToUse = inventoryObjectNeededList [i].objectNeeded;
					currentNumberOfObjectsNeeded = inventoryObjectNeededList [i].amountNeeded - inventoryObjectNeededList [i].amountOfObjectsUsed;

					if (inventoryObjectNeededList[i].useObjectAction) {
						if (deviceStringActionManager) {
							deviceStringActionManager.setDeviceAction (inventoryObjectNeededList[i].objectAction + " x " + currentNumberOfObjectsNeeded.ToString());
							if (usingDevicesManager) {
								usingDevicesManager.checkDeviceName ();
							}
						}
					}
					return;
				} else {
					currentObjectToUse = null;
				}
			}
		}
	}

	public string getObjectUsedMessage ()
	{
		return currentObjectUsedMessage;
	}

	public bool inventoryObjectNeededListContainsObject (GameObject objectToCheck)
	{
		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			if (!inventoryObjectNeededList [i].objectUsed) {
				if (inventoryObjectNeededList [i].objectNeeded == objectToCheck) {
					currentObjectToUse = inventoryObjectNeededList [i].objectNeeded;
					return true;
				}
			}
		}
		return false;
	}

	public int getInventoryObjectNeededAmound (GameObject objectToCheck)
	{
		if (canBeReUsed) {
			return 1;
		}
		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			if (!inventoryObjectNeededList [i].objectUsed) {
				if (inventoryObjectNeededList [i].objectNeeded == objectToCheck) {
					currentNumberOfObjectsNeeded = inventoryObjectNeededList [i].amountNeeded - inventoryObjectNeededList [i].amountOfObjectsUsed;
					return currentNumberOfObjectsNeeded;
				}
			}
		}
		return -1;
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (!objectUsed && col.tag == "Player") {
				setCurrentPlayer (col.gameObject);
				selectObjectOnInventory ();
			}
		} else {
			if (!objectUsed && col.tag == "Player") {
				removePlayerInventoryInfo ();
				removeCurrentPlayer ();
			}
		}
	}

	public void removePlayerInventoryInfo ()
	{
		if (playerInventoryManager) {
			playerInventoryManager.setCurrentUseInventoryGameObject (null);
			playerInventoryManager.removeCurrentInventoryObject ();
		}
	}

	public void playObjectUsedSound (int index)
	{
		if (mainAudioSource) {
			AudioClip currentAudioClip = inventoryObjectNeededList [index].usedObjectSound;
			if (currentAudioClip) {
				mainAudioSource.PlayOneShot (currentAudioClip);
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		playerInventoryManager = currentPlayer.GetComponent<inventoryManager> ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
	}

	public void removeCurrentPlayer ()
	{
		currentPlayer = null;
		playerInventoryManager = null;
	}

	public void removeDeviceFromList ()
	{
		if (usingDevicesManager) {
			usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
		}
	}

	public int getCurrentAmountUsed ()
	{
		return currentAmountUsed;
	}

	public void enableOrDisableTrigger (bool state)
	{
		if (!mainCollider) {
			mainCollider = GetComponent<Collider> ();
		}
		if (mainCollider) {
			if (mainCollider.enabled != state) {
				canBeUsed = state;
				mainCollider.enabled = state;
				if (!state) {
					removeDeviceFromList ();
				}
			}
		}
	}

	public void addInventoryObjectNeededInfo ()
	{
		inventoryObjectNeededInfo newInventoryObjectNeededInfo = new inventoryObjectNeededInfo ();
		newInventoryObjectNeededInfo.Name = "New Object";
		inventoryObjectNeededList.Add (newInventoryObjectNeededInfo);
		updateComponent ();
	}

	public void addSubInventoryObjectNeededList (int index)
	{
		inventoryElementNeededInfo newInventoryElementNeededInfo = new inventoryElementNeededInfo ();
		inventoryObjectNeededList [index].inventoryObjectNeededList.Add (newInventoryElementNeededInfo);
		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<useInventoryObject> ());
		#endif
	}

	[System.Serializable]
	public class inventoryObjectNeededInfo
	{
		public string Name;
		public GameObject objectNeeded;
		public bool useObjectAction;
		public string objectAction;
		public int amountNeeded;
		public bool objectUsed;
		public int amountOfObjectsUsed;
		[TextArea (3, 10)]
		public string objectUsedMessage;

		public bool useEventOnObjectsPlaced;
		public UnityEvent eventOnObjectsPlaced;

		public bool useObjectSound;
		public AudioClip usedObjectSound;
		public List<inventoryElementNeededInfo> inventoryObjectNeededList = new List<inventoryElementNeededInfo> ();
	}

	[System.Serializable]
	public class inventoryElementNeededInfo
	{
		public bool instantiateObject;
		public Transform placeForObject;
		public bool enableObject;
		public GameObject objectToEnable;
		public bool objectActivated;

		public bool useEventOnObjectPlaced;
		public UnityEvent eventOnObjectPlaced;

		public bool useAnimation;
		public GameObject objectWithAnimation;
		public string animationName;
	}
}