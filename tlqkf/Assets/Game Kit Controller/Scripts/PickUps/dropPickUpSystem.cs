using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class dropPickUpSystem : MonoBehaviour
{
	public bool dropPickupsEnabled = true;

	public List<dropPickUpElementInfo> dropPickUpList = new List<dropPickUpElementInfo> ();
	public List<pickUpElementInfo> managerPickUpList = new List<pickUpElementInfo> ();
	public float dropDelay;
	public bool destroyAfterDropping;
	public float pickUpScale;
	public bool setPickupScale;
	public bool randomContent;
	public float maxRadiusToInstantiate = 1;
	public Vector3 pickUpOffset;

	public float extraForceToPickup = 5;
	public float extraForceToPickupRadius = 5;
	public ForceMode forceMode = ForceMode.Impulse;

	public bool showGizmo;
	GameObject newObject;
	pickUpManager manager;

	//instantiate the objects in the enemy position, setting their configuration
	public void createDropPickUpObjects ()
	{
		if (!dropPickupsEnabled) {
			return;
		}

		StartCoroutine (createDropPickUpObjectsCoroutine ());
	}

	IEnumerator createDropPickUpObjectsCoroutine ()
	{
		yield return new WaitForSeconds (dropDelay);

		for (int i = 0; i < dropPickUpList.Count; i++) {
			for (int k = 0; k < dropPickUpList [i].dropPickUpTypeList.Count; k++) {
				//of every object, create the amount set in the inspector, the ammo and the inventory objects will be added in future updates
				int maxAmount = dropPickUpList [i].dropPickUpTypeList [k].amount;
				int quantity = dropPickUpList [i].dropPickUpTypeList [k].quantity;
				if (randomContent) {
					maxAmount = (int)Random.Range (dropPickUpList [i].dropPickUpTypeList [k].amountLimits.x, dropPickUpList [i].dropPickUpTypeList [k].amountLimits.y);
				}

				for (int j = 0; j < maxAmount; j++) {
					if (randomContent) {
						quantity = (int)Random.Range (dropPickUpList [i].dropPickUpTypeList [k].quantityLimits.x, dropPickUpList [i].dropPickUpTypeList [k].quantityLimits.y);
					}

					GameObject objectToInstantiate = managerPickUpList [dropPickUpList [i].typeIndex].pickUpTypeList [dropPickUpList [i].dropPickUpTypeList [k].nameIndex].pickUpObject;

					if (objectToInstantiate) {
						newObject = (GameObject)Instantiate (objectToInstantiate, transform.position + getOffset (), transform.rotation);

						pickUpObject currentPickUpObject = newObject.GetComponent<pickUpObject> ();
						if (currentPickUpObject) {
							currentPickUpObject.amount = quantity;
						}

						if (setPickupScale) {
							newObject.transform.localScale = Vector3.one * pickUpScale;
						}

						//set a random position  and rotation close to the enemy position
						newObject.transform.position += Random.insideUnitSphere * maxRadiusToInstantiate;

						//apply force to the objects
						Rigidbody currentRigidbody = newObject.GetComponent<Rigidbody> ();
						if (currentRigidbody) {
							currentRigidbody.AddExplosionForce (extraForceToPickup, transform.position, extraForceToPickupRadius, 1, forceMode);
						}
					} else {
						print ("Warning, the pickups haven't been configured correctly in the pickup manager inspector");
					}
				}
			}
		}
		if (destroyAfterDropping) {
			Destroy (gameObject);
		}
	}

	public void getManagerPickUpList ()
	{
		if (!manager) {
			manager = FindObjectOfType<pickUpManager> ();
		} 

		if (manager) {
			managerPickUpList.Clear ();

			for (int i = 0; i < manager.mainPickUpList.Count; i++) {	
				managerPickUpList.Add (manager.mainPickUpList [i]);
			}
			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<dropPickUpSystem> ());
		#endif
	}

	public Vector3 getOffset ()
	{
		return (pickUpOffset.x * transform.right + pickUpOffset.y * transform.up + pickUpOffset.z * transform.forward);
	}

	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (!Application.isPlaying && showGizmo) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (transform.position + getOffset (), maxRadiusToInstantiate);
		}
	}

	[System.Serializable]
	public class dropPickUpElementInfo
	{
		public string pickUpType;
		public int typeIndex;
		public List<dropPickUpTypeElementInfo> dropPickUpTypeList = new List<dropPickUpTypeElementInfo> ();
	}

	[System.Serializable]
	public class dropPickUpTypeElementInfo
	{
		public string name;
		public int amount;
		public int quantity;
		public Vector2 amountLimits;
		public Vector2 quantityLimits;
		public int nameIndex;
	}
}