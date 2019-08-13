using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class inventoryListElement
{
	public string name;
	public int amount;
	public bool infiniteAmount;
	public string inventoryObjectName;
	public int elementIndex;
	public bool isEquipped;
	public bool addInventoryObject = true;
}
