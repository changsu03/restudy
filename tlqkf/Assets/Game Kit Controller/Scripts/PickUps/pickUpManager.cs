using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class pickUpManager : MonoBehaviour
{
	public List<pickUpElementInfo> mainPickUpList = new List<pickUpElementInfo> ();

	public List<pickUpIconInfo> pickUpIconList = new List<pickUpIconInfo> ();

	public List<playerPickupIconManager> playerPickupIconManagerList = new List<playerPickupIconManager> ();

	int currentID = 0;

	public void addNewPlayer (playerPickupIconManager newPlayer)
	{
		playerPickupIconManagerList.Add (newPlayer);
	}

	//set what type of pick up is this object, and the object that the icon has to follow
	public void setPickUpIcon (GameObject target, string pickupIconGeneralName, string pickupIconName)
	{
		pickUpIconInfo newIcon = new pickUpIconInfo ();
		newIcon.ID = currentID;
		newIcon.target = target;

		Texture iconTexture = null;
		for (int i = 0; i < mainPickUpList.Count; i++) {
			if (mainPickUpList [i].pickUpType.ToLower () == pickupIconGeneralName.ToLower ()) {
				if (mainPickUpList [i].useGeneralIcon) {
					iconTexture = mainPickUpList [i].generalIcon;
				} else {
					for (int j = 0; j < mainPickUpList [i].pickUpTypeList.Count; j++) {
						if (mainPickUpList [i].pickUpTypeList [j].Name.ToLower () == pickupIconName.ToLower ()) {
							iconTexture = mainPickUpList [i].pickUpTypeList [j].pickupIcon;
						}
					}
				}
			}
		}

		pickUpIconList.Add (newIcon);

		for (int i = 0; i < playerPickupIconManagerList.Count; i++) {
			playerPickupIconManagerList [i].setPickUpIcon (target, iconTexture, currentID);
		}

		currentID++;
	}

	//destroy the icon
	public void removeTarget (GameObject target)
	{
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].target) {
				if (pickUpIconList [i].target == target) {
					removeAtTarget (pickUpIconList [i].ID, i);
					return;
				}
			}
		}
	}

	public void removeAtTarget (int objectID, int objectIndex)
	{
		for (int i = 0; i < playerPickupIconManagerList.Count; i++) {
			playerPickupIconManagerList [i].removeAtTargetByID (objectID);
		}
		pickUpIconList.RemoveAt (objectIndex);
	}

	public void removeElementFromPickupListCalledByPlayer (int objectID)
	{
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].ID == objectID) {
				pickUpIconList.RemoveAt (i);
				return;
			}
		}
	}

	public void setPauseState (bool state, GameObject iconObject)
	{
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].target == iconObject) {
				pickUpIconList [i].paused = state;
				for (int j = 0; j < playerPickupIconManagerList.Count; j++) {
					playerPickupIconManagerList [j].setPauseState (state, i);
				}
			}
		}
	}

	public void addNewPickup ()
	{
		pickUpElementInfo newPickupElementInfo = new pickUpElementInfo ();
		newPickupElementInfo.pickUpType = "New Pickup Type";
		mainPickUpList.Add (newPickupElementInfo);

		udpateComponent ();
	}

	public void addNewPickupToList (int index)
	{
		pickUpElementInfo.pickUpTypeElementInfo newPickupTypeElementInfo = new pickUpElementInfo.pickUpTypeElementInfo ();
		newPickupTypeElementInfo.Name = "New Pickup";
		mainPickUpList [index].pickUpTypeList.Add (newPickupTypeElementInfo);

		udpateComponent ();
	}

	public void udpateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<pickUpManager> ());
		#endif
	}
}