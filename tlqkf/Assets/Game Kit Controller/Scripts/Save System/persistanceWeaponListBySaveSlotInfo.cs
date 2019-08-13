using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceWeaponListBySaveSlotInfo  {
	public int saveNumber;
	public List<persistanceWeaponListByPlayerInfo> playerWeaponList = new List<persistanceWeaponListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceWeaponListByPlayerInfo
{
	public int playerID;
	public List<persistanceWeaponInfo> weaponList = new List<persistanceWeaponInfo> ();
}

[System.Serializable]
public class persistanceWeaponInfo
{
	public string Name;
	public int index;
	public bool isWeaponEnabled;
	public bool isCurrentWeapon;
	public int remainingAmmo;
}
