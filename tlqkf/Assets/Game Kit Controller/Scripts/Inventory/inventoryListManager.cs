using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class inventoryListManager : MonoBehaviour
{
	public List<inventoryInfo> inventoryList = new List<inventoryInfo> ();

	public string relativePathCaptures = "Assets/Game Kit Controller/Prefabs/Inventory/Captures";

	public Camera inventoryCamera;
	public Transform lookObjectsPosition;

	public string relativePathRegularInventory = "Assets/Game Kit Controller/Prefabs/Inventory/Usable/Regular";
	public string relativePathWeaponInventory = "Assets/Game Kit Controller/Prefabs/Inventory/Usable/Weapons";

	public GameObject emtpyInventoryPrefab;

	public inventoryBankManager mainInventoryBankManager;

	public void setInventoryCaptureIcon (inventoryInfo info, Texture2D texture)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i] == info) {
				inventoryList [i].icon = texture;
			}
		}
		updateComponent ();
	}

	public string getDataPath ()
	{
		string dataPath = "";
		if (!Directory.Exists (relativePathCaptures)) {
			Directory.CreateDirectory (relativePathCaptures);
		}

		dataPath = relativePathCaptures + "/";

		return dataPath;
	}

	public void addNewInventoryObject ()
	{
		inventoryInfo newObject = new inventoryInfo ();
		inventoryList.Add (newObject);
		updateComponent ();
	}

	public inventoryInfo getInventoryInfoFromName (string objectName)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].Name.ToLower () == objectName.ToLower ()) {
				return inventoryList [i];
			}
		}
		return null;
	}

	public inventoryInfo getInventoryInfoFromInventoryGameObject (GameObject objectToFind)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == objectToFind) {
				return inventoryList [i];
			}
		}
		return null;
	}

	public int getInventoryIndexFromInventoryGameObject (GameObject objectToFind)
	{
		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryGameObject == objectToFind) {
				return i;
			}
		}
		return -1;
	}

	public void createInventoryPrafab (int index)
	{
		#if UNITY_EDITOR

		inventoryInfo currentInventoryInfo = inventoryList [index];

		if (currentInventoryInfo.inventoryGameObject == null) {
			print ("Please, the Inventory Object Mesh to create the prefab");
			return;
		}

		GameObject newEmtpyInventoryPrefab = Instantiate (emtpyInventoryPrefab);
		inventoryObject currentInventoryObject = newEmtpyInventoryPrefab.GetComponentInChildren<inventoryObject> ();

		currentInventoryObject.inventoryObjectInfo = new inventoryInfo (currentInventoryInfo);

		GameObject newInventoryObjectMesh = Instantiate (currentInventoryInfo.inventoryGameObject);
		newInventoryObjectMesh.transform.SetParent (newEmtpyInventoryPrefab.transform);
		newInventoryObjectMesh.transform.localPosition = Vector3.zero;
		newInventoryObjectMesh.transform.localRotation = Quaternion.identity;

		Component[] colliders = newInventoryObjectMesh.GetComponentsInChildren (typeof(Collider));
		foreach (Component c in colliders) {
			Type type = c.GetComponent<Collider> ().GetType ();
			Component copy = newEmtpyInventoryPrefab.AddComponent (type);
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			PropertyInfo[] pinfos = type.GetProperties (flags);
			for (int i = 0; i < pinfos.Length; i++) {
				pinfos [i].SetValue (copy, pinfos [i].GetValue (c.GetComponent<Collider> (), null), null);
			}
		}

		foreach (Component c in colliders) {
			DestroyImmediate (c.GetComponent<Collider> ());
		}

		deviceStringAction currentDeviceStringAction = newEmtpyInventoryPrefab.GetComponentInChildren<deviceStringAction> ();

		currentDeviceStringAction.deviceName = currentInventoryInfo.Name;

		string relativePath = relativePathRegularInventory;

		pickUpObject currentPickupObject = newEmtpyInventoryPrefab.GetComponent<pickUpObject> ();
		if (currentPickupObject) {
			currentPickupObject.amount = 1;
			if (currentInventoryInfo.isWeapon) {
				currentPickupObject.useSecondaryString = true;
				currentPickupObject.secondaryString = currentInventoryInfo.Name;
				currentPickupObject.pickType = pickUpObject.pickUpType.weapon;
				relativePath = relativePathWeaponInventory;
			}
		}

		string prefabFilePath = relativePath + "/" + currentInventoryInfo.Name + " (inventory) " + ".prefab";

		UnityEngine.Object prefab = EditorUtility.CreateEmptyPrefab (prefabFilePath);
		EditorUtility.ReplacePrefab (newEmtpyInventoryPrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);

		currentInventoryInfo.inventoryObjectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));

		DestroyImmediate (newEmtpyInventoryPrefab);

		updateComponent();
		#endif
	}

	public inventoryBankManager getMainInventoryBankManager ()
	{
		return mainInventoryBankManager;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<inventoryListManager> ());
		#endif
	}
}
