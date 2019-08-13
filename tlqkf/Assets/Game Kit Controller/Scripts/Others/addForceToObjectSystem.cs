using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addForceToObjectSystem : MonoBehaviour
{
	public Transform forceDirection;

	public float forceAmountCharacters;
	public float forceAmountRegularObjects;

	public ForceMode forceModeCharacters;
	public ForceMode forceModeRegularObjects;

	public bool addForceInUpdate;

	public List<rigidbodyInfo> rigidbodyInfoList = new List<rigidbodyInfo> ();

	public bool objectsDetected;

	void Update ()
	{
		if (objectsDetected) {
			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				if (rigidbodyInfoList [i].isPlayer) {
					rigidbodyInfoList [i].mainRigidbody.AddForce (forceDirection.forward * forceAmountCharacters, forceModeCharacters);
				} else {
					rigidbodyInfoList [i].mainRigidbody.AddForce (forceDirection.forward * forceAmountRegularObjects, forceModeRegularObjects);
				}
			}
		}
	}

	public void addNewObject (GameObject newObject)
	{
		Rigidbody mainRigidbody = newObject.GetComponent<Rigidbody> ();

		if (mainRigidbody) {
			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				if (rigidbodyInfoList [i].mainObject == newObject) {
					return;
				}
			}


			rigidbodyInfo newRigidbodyInfo = new rigidbodyInfo ();

			newRigidbodyInfo.mainObject = newObject;
			newRigidbodyInfo.mainRigidbody = mainRigidbody;

			if (newObject.tag == "Player") {
				newRigidbodyInfo.isPlayer = true;
			}

			rigidbodyInfoList.Add (newRigidbodyInfo);

			objectsDetected = true;
		}
	}

	public void removeObject (GameObject objectToRemove)
	{
		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			if (rigidbodyInfoList [i].mainObject == objectToRemove) {
				rigidbodyInfoList.RemoveAt (i);

				if (rigidbodyInfoList.Count == 0) {
					objectsDetected = false;
				}

				return;
			}
		}
	}

	[System.Serializable]
	public class rigidbodyInfo
	{
		public string Name;
		public bool isPlayer;
		public GameObject mainObject;
		public Rigidbody mainRigidbody;
	}
}
