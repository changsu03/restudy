using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class hitCombat : MonoBehaviour
{
	public float hitDamage = 5;
	public float addForceMultiplier;

	public ForceMode forceMode = ForceMode.Impulse;
	public bool applyImpactForceToVehicles = true;
	public float impactForceToVehiclesMultiplier = 0.2f;

	public LayerMask layerMask;
	public bool currentlyEnabled;

	public GameObject currentPlayer;

	Rigidbody objectToDamageRigidbody;
		
	//check the collision in the sphere colliders in the hands and feet of the player when the close combat system is active
	//else the sphere collider are disabled to avoid damage enemies just with touch it without fight
	void OnTriggerEnter (Collider col)
	{
		if (currentlyEnabled) {
			if ((1 << col.gameObject.layer & layerMask.value) == 1 << col.gameObject.layer) {
				if (col.gameObject != currentPlayer && !col.isTrigger) {
					GameObject objectToDamage = col.gameObject;
					if (applyDamage.checkCanBeDamage (gameObject, objectToDamage, hitDamage, -transform.forward, transform.position, currentPlayer, false, true)) {
						currentlyEnabled = true;
					}

					objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

					if (applyImpactForceToVehicles) {
						Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);
						if (objectToDamageMainRigidbody) {
							Vector3 force = currentPlayer.transform.up + currentPlayer.transform.forward * addForceMultiplier;
							bool isVehicle = applyDamage.isVehicle (objectToDamage);
							if (isVehicle) {
								force *= impactForceToVehiclesMultiplier;
							}
							objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, forceMode);
						}
					} else {
						if (applyDamage.canApplyForce (objectToDamage)) {
							//print (objectToDamage.name);
							Vector3 force = currentPlayer.transform.up + currentPlayer.transform.forward * addForceMultiplier;
							if (objectToDamageRigidbody == null) {
								objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
							}
							objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, forceMode);
						}
					}

					explosiveBarrel currentExplosiveBarrel = objectToDamage.GetComponent<explosiveBarrel> ();
					if (currentExplosiveBarrel) {
						currentExplosiveBarrel.setExplosiveBarrelOwner (currentPlayer);
					}
				}
			}
		}
	}

	public void getOwner (GameObject ownerObject)
	{
		currentPlayer = ownerObject;
		//ignore collision between the owner and the sphere colliders, to avoid hurt him self
		Physics.IgnoreCollision (currentPlayer.GetComponent<Collider> (), GetComponent<Collider> ());
	}

	public void setCurrentState (bool value)
	{
		//print (value);
		currentlyEnabled = value;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<hitCombat> ());
		#endif
	}
}