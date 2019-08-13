using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class getInventoryObjectSystem : MonoBehaviour
{
	public GameObject objectToGiveToplayer;
	public int objectAmount;

	public bool useEventIfObjectStored;
	public UnityEvent eventIfObjectStored;

	public bool placeObjectOnSceneIfCantBeStored;
	public Transform positionToPlaceInventoryObject;

	public GameObject currentPlayer;
	inventoryManager playerInventoryManager;

	public void getCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		playerInventoryManager = currentPlayer.GetComponent<inventoryManager> ();
	}

	public void giveObjectToPlayer ()
	{
		StartCoroutine (giveObjectToPlayerCoroutine ());
	}

	IEnumerator giveObjectToPlayerCoroutine ()
	{
		bool enoughFreeSpace = false;
		if (!playerInventoryManager.isInventoryFull ()) {
			GameObject inventoryObjectMesh = objectToGiveToplayer.GetComponentInChildren<inventoryObject> ().inventoryObjectInfo.inventoryGameObject;
			enoughFreeSpace = playerInventoryManager.checkIfObjectCanBeStored (inventoryObjectMesh, objectAmount);

			if (enoughFreeSpace) {
				GameObject newInventoryObject = (GameObject)Instantiate (objectToGiveToplayer, Vector3.one * 1000, Quaternion.identity);

				yield return new WaitForSeconds (0.1f);
				pickUpObject currentPickUpObject = newInventoryObject.GetComponent<pickUpObject> ();
				currentPickUpObject.getComponents ();
				currentPickUpObject.setNewAmount (objectAmount);
				currentPickUpObject.checkTriggerInfo (currentPlayer.GetComponent<Collider> ());
				newInventoryObject.GetComponentInChildren<simpleActionButton> ().activateDevice ();

				if (useEventIfObjectStored) {
					eventIfObjectStored.Invoke ();
				}
			} else {
				if(placeObjectOnSceneIfCantBeStored){
					GameObject newInventoryObject = (GameObject)Instantiate (objectToGiveToplayer, positionToPlaceInventoryObject.position, positionToPlaceInventoryObject.rotation);
					newInventoryObject.transform.position = positionToPlaceInventoryObject.position;
					objectToGiveToplayer.transform.position = positionToPlaceInventoryObject.position;
				}
			}
		} 

		if (!enoughFreeSpace) {
			playerInventoryManager.showInventoryFullMessage ();
		}
	}
}
