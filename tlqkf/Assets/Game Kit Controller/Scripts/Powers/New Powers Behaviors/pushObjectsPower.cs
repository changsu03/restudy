using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushObjectsPower : MonoBehaviour
{
	public bool powerEnabled = true;

	public bool pushObjectsFromCenterPosition;
	public Transform centerPosition;
	public otherPowers powersManager;
	public Transform pushObjectsCenter;
	public Transform mainCameraTransform;
	public LayerMask layer;

	public string messageNameToSend = "pushCharacter";
	public List<string> ignoreTagList = new List<string> ();

	public float forceToApply = 4000;
	public ForceMode forceMode;

	public bool canApplyForceToVehicles = true;
	public float applyForceToVehiclesMultiplier = 0.2f;

	public bool useCustomPushCenterDistance;
	public float pushCenterDistance;

	public GameObject playerGameObject;

	Rigidbody objectToDamageMainRigidbody;

	List<Rigidbody> vehiclesRigidbodyFoundList = new List<Rigidbody> ();

	GameObject objectToPush;
	Collider[] colliders;
	Vector3 currentForceToApply;
	float finalExplosionForce;
	bool isVehicle;

	void Start ()
	{
		if (!useCustomPushCenterDistance) {
			//get the distance from the empty object in the player to push objects, close to it
			pushCenterDistance = GKC_Utils.distance (transform.position, pushObjectsCenter.position);
		}

		if(pushObjectsFromCenterPosition){
			if (centerPosition == null) {
				centerPosition = transform;
			}
		}
	}

	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		vehiclesRigidbodyFoundList.Clear ();

		if (powersManager) {
			//the power number 2 is push objects, so any bullet is created
			powersManager.createShootParticles ();
		}

		//if the power selected is push objects, check the objects close to pushObjectsCenter and add force to them in camera forward direction
		colliders = Physics.OverlapSphere (pushObjectsCenter.position, pushCenterDistance, layer);

		for (int i = 0; i < colliders.Length; i++) {
			if (!colliders [i].isTrigger) {
				objectToPush = colliders [i].gameObject;

				if (!ignoreTagList.Contains (objectToPush.tag) && objectToPush != playerGameObject) {
				
					objectToPush.SendMessage (messageNameToSend, transform.forward, SendMessageOptions.DontRequireReceiver);

					if (canApplyForceToVehicles) {
						objectToDamageMainRigidbody = applyDamage.applyForce (objectToPush);
						if (objectToDamageMainRigidbody) {
							if (!vehiclesRigidbodyFoundList.Contains (objectToDamageMainRigidbody)) {
								isVehicle = applyDamage.isVehicle (objectToPush);

								finalExplosionForce = forceToApply;
								if (isVehicle) {
									finalExplosionForce *= applyForceToVehiclesMultiplier;
								}

								if (pushObjectsFromCenterPosition) {
									currentForceToApply = (objectToDamageMainRigidbody.position - centerPosition.position).normalized * finalExplosionForce;
								} else {
									currentForceToApply = mainCameraTransform.TransformDirection (Vector3.forward) * finalExplosionForce;
								}

								objectToDamageMainRigidbody.AddForce (currentForceToApply * objectToDamageMainRigidbody.mass, forceMode);

								if (isVehicle) {
									vehiclesRigidbodyFoundList.Add (objectToDamageMainRigidbody);
								}
							}
						}
					} else {
						if (applyDamage.canApplyForce (objectToPush)) {
							if (pushObjectsFromCenterPosition) {
								currentForceToApply = (objectToDamageMainRigidbody.position - centerPosition.position).normalized * forceToApply;
							} else {
								currentForceToApply = mainCameraTransform.TransformDirection (Vector3.forward) * forceToApply;
							}
								
							objectToDamageMainRigidbody = objectToPush.GetComponent<Rigidbody> ();
							objectToDamageMainRigidbody.AddForce (currentForceToApply * objectToDamageMainRigidbody.mass, forceMode);
						}
					}
				}
			}
		}
	}

	public void setPowerEnabledState (bool state)
	{
		powerEnabled = state;
	}
}
