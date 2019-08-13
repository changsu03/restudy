using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class inventoryInfo
{
	public string Name;
	public GameObject inventoryGameObject;
	[TextArea (3, 10)] public string objectInfo;
	public Texture icon;
	public int amount;
	public int amountPerUnit;
	public bool infiniteAmount;
	public bool canBeUsed;
	public bool canBeEquiped;
	public bool canBeDropped;
	public bool canBeCombined;

	public bool isEquiped;

	public bool isWeapon;
	public IKWeaponSystem IKWeaponManager;

	public GameObject objectToCombine;
	public GameObject combinedObject;
	[TextArea (3, 10)] public string combinedObjectMessage;

	public bool canBeDiscarded;

	public Button button;
	public inventoryMenuIconElement menuIconElement;
	public GameObject inventoryObjectPrefab;

	public inventoryInfo (inventoryInfo obj)
	{
		Name = obj.Name;
		inventoryGameObject = obj.inventoryGameObject;
		objectInfo = obj.objectInfo;
		icon = obj.icon;
		amount = obj.amount;
		amountPerUnit = obj.amountPerUnit;
		infiniteAmount = obj.infiniteAmount;
		canBeUsed = obj.canBeUsed;
		canBeEquiped = obj.canBeEquiped;
		canBeDropped = obj.canBeDropped;
		canBeCombined = obj.canBeCombined;
		canBeDiscarded = obj.canBeDiscarded;

		isEquiped = obj.isEquiped;

		isWeapon = obj.isWeapon;
		IKWeaponManager = obj.IKWeaponManager;

		objectToCombine = obj.objectToCombine;
		combinedObject = obj.combinedObject;
		combinedObjectMessage = obj.combinedObjectMessage;
		button = obj.button;
	}

	public inventoryInfo ()
	{
		Name = "New Object";
		objectInfo = "New Description";
	}

	public void resetInventoryInfo ()
	{
		Name = "Emtpy Slot";
		inventoryGameObject = null;
		objectInfo = "It is an empty slot";
		icon = null;
		amount = 0;
		amountPerUnit = 0;
		infiniteAmount = false;
		canBeUsed = false;
		canBeDropped = false;
		canBeEquiped = false;
		canBeCombined = false;

		canBeDiscarded = false;

		isEquiped = false;

		isWeapon = false;

		IKWeaponManager = null;

		objectToCombine = null;
		combinedObject = null;
		combinedObjectMessage = "";
		menuIconElement.icon.texture = null;
		menuIconElement.iconName.text = "Emtpy Slot";
		menuIconElement.amount.text = "0";
	}
}
