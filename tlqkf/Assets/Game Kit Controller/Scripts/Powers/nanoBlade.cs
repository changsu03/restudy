using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class nanoBlade : projectileSystem
{
	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}
			projectileUsed = true;
			//set the bullet kinematic

			objectToDamage = col.GetComponent<Collider> ().gameObject;
			Vector3 previousVelocity = mainRigidbody.velocity;
			//print (objectToDamage.name);
			mainRigidbody.isKinematic = true;

			if (objectToDamage.GetComponent<Rigidbody> ()) {
				ragdollActivator currentRagdollActivator = objectToDamage.GetComponent<ragdollActivator> ();
				if (currentRagdollActivator) {
					List<ragdollActivator.bodyPart> bones = currentRagdollActivator.getBodyPartsList ();
					float distance = Mathf.Infinity;
					int index = -1;
					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, transform.position);
						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						transform.SetParent (bones [index].transform);
						//print (bones [index].transform.name);
						if (applyDamage.checkIfDead (objectToDamage)) {
							mainRigidbody.isKinematic = false;
							mainRigidbody.velocity = previousVelocity;
							projectileUsed = false;
						}
					}
				} else if (objectToDamage.GetComponent<characterDamageReceiver> ()) {
					transform.SetParent (objectToDamage.GetComponent<characterDamageReceiver> ().character.transform);
				} else {
					transform.SetParent (objectToDamage.transform);
				}
			} else if (objectToDamage.GetComponent<characterDamageReceiver> ()) {
				transform.SetParent (objectToDamage.transform);
			} else if (objectToDamage.GetComponent<vehicleDamageReceiver> ()) {
				transform.SetParent (objectToDamage.GetComponent<vehicleDamageReceiver> ().vehicle.transform);
			}

			checkProjectilesParent ();

			//add velocity if the touched object has rigidbody
			if (currentProjectileInfo.killInOneShot) {
				applyDamage.killCharacter (projectileGameObject, objectToDamage, -transform.forward, transform.position, currentProjectileInfo.owner, false);
			} else {
				applyDamage.checkHealth (projectileGameObject, objectToDamage, currentProjectileInfo.projectileDamage, -transform.forward, transform.position, 
					currentProjectileInfo.owner, false, true);
			}

			if (currentProjectileInfo.applyImpactForceToVehicles) {
				Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);
				if (objectToDamageMainRigidbody) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;
					objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, currentProjectileInfo.forceMode);
				}
			} else {
				if (applyDamage.canApplyForce (objectToDamage)) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;
					objectToDamage.GetComponent<Rigidbody> ().AddForce (force * objectToDamage.GetComponent<Rigidbody> ().mass, currentProjectileInfo.forceMode);
				}
			}
		}
	}
}