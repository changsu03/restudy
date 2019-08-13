using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceInventoryListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceInventoryListByPlayerInfo> playerInventoryList = new List<persistanceInventoryListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceInventoryListByPlayerInfo
{
	public int playerID;
	public int inventorySlotAmount;
	public List<persistanceInventoryObjectInfo> inventoryObjectList = new List<persistanceInventoryObjectInfo> ();
}

[System.Serializable]
public class persistanceInventoryObjectInfo
{
	public string name;
	public int amount;
	public bool infiniteAmount;
	public string inventoryObjectName;
	public int elementIndex;
	public bool isEquipped;

	public persistanceInventoryObjectInfo (persistanceInventoryObjectInfo obj)
	{
		name = obj.name;
		amount = obj.amount;
		infiniteAmount = obj.infiniteAmount;
		inventoryObjectName = obj.inventoryObjectName;
		elementIndex = obj.elementIndex;
		isEquipped = obj.isEquipped;
	}

	public persistanceInventoryObjectInfo ()
	{
		name = "New Object";
	}
}