using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class jumpPlatform : MonoBehaviour
{
	public bool platformEnabled = true;
	public float jumpForce;
	public string platformAnimation;
	public bool useWithPlayer;
	public bool useWithNPC;
	public bool useWithVehicles;
	public bool useWithAnyRigidbody;
	public bool useKeyToJumpWithPlayer;
	public bool useKeyToJumpWithVehicles;

	public bool useParableLaunch;
	public Transform targetPosition;

	public GameObject objectToImpulse;
	public List<GameObject> objectToImpulseList = new List<GameObject> ();
	playerController playerControllerManager;
	grabbedObjectState currentGrabbedObject;
	Animation mainAnimation;
	vehicleHUDManager currentVehicleHUDManager;
	Rigidbody currentRigidbody;
	parentAssignedSystem currentParentAssignedSystem;

	void Start ()
	{
		mainAnimation = GetComponent<Animation> ();
	}

	void Update ()
	{
		if (platformEnabled) {
			//play the platform animation
			mainAnimation.Play (platformAnimation);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (!platformEnabled) {
			return;
		}

		//if the player is inside the trigger and the platform can be used with him, then
		if ((col.gameObject.tag == "Player" && useWithPlayer) || (col.gameObject.tag == "friend" && useWithNPC) || (col.gameObject.tag == "enemy" && useWithNPC)) {

			objectToImpulse = col.gameObject;
			if (objectToImpulseList.Contains (objectToImpulse)) {
				return;
			}
			objectToImpulseList.Add (objectToImpulse);

			playerControllerManager = objectToImpulse.GetComponent<playerController> ();
			//if the player is not driving
			if (!playerControllerManager.driving) {
				//the platform increase the jump force in the player, and only the jump button will make the player to jump
				if (useKeyToJumpWithPlayer) {
					playerControllerManager.useJumpPlatformWithKeyButton (true, jumpForce);
				} else {
					//else make the player to jump
					if (playerControllerManager.isPlayerOnFirstPerson ()) {
						objectToImpulse.GetComponent<playerStatesManager> ().checkPlayerStates (false, false, false, false, false, false, false, false);
					} else {
						objectToImpulse.GetComponent<playerStatesManager> ().checkPlayerStates ();
					}

					Vector3 jumpDirection = jumpForce * transform.up;
					if (useParableLaunch) {
						jumpDirection = getParableSpeed (objectToImpulse.transform.position, targetPosition.position) * jumpForce;
						playerControllerManager.useJumpPlatform (jumpDirection, ForceMode.VelocityChange);
					} else {
						playerControllerManager.useJumpPlatform (jumpDirection, ForceMode.Impulse);
					}
				}
			}
		} 

		//if any other rigidbody enters the trigger, then
		else { 
			currentRigidbody = col.gameObject.GetComponent<Rigidbody> ();
			if (currentRigidbody) {
				objectToImpulse = col.gameObject;
			} else {
				currentParentAssignedSystem = col.gameObject.GetComponent<parentAssignedSystem> ();
				if (currentParentAssignedSystem) {
					objectToImpulse = currentParentAssignedSystem.getAssignedParent ();
					currentRigidbody = objectToImpulse.GetComponent<Rigidbody> ();
				}
			}

			if (objectToImpulse == null) {
				objectToImpulse = applyDamage.getCharacterOrVehicle (col.gameObject);
			}
				
			if (objectToImpulse) {
				if (objectToImpulseList.Contains (objectToImpulse)) {
					return;
				}
				objectToImpulseList.Add (objectToImpulse);


				//if the object is being carried by the player, make him drop it
				currentGrabbedObject = objectToImpulse.GetComponent<grabbedObjectState> ();
				if (currentGrabbedObject) {
					GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectToImpulse);
				}

				currentVehicleHUDManager = objectToImpulse.GetComponent<vehicleHUDManager> ();
				//if a vehicle enters inside the trigger and the platform can be used with vehicles, then
				if (objectToImpulse.tag == "vehicle" && currentVehicleHUDManager && useWithVehicles) {
					//the platform increases the jump force in the vehicle, and only the jump button will make the vehicle to jump
					if (useKeyToJumpWithVehicles) {
						currentVehicleHUDManager.useJumpPlatformWithKeyButton (true, jumpForce);
					} else {
						//else make the vehicle to jump
						Vector3 jumpDirection = jumpForce * transform.up;
						if (useParableLaunch) {
							jumpDirection = getParableSpeed (objectToImpulse.transform.position, targetPosition.position) * jumpForce;
							print (jumpDirection);
							currentVehicleHUDManager.useJumpPlatformParable (jumpDirection);
						} else {
							currentVehicleHUDManager.useJumpPlatform (jumpDirection);
						}
					}
				} else {
					//if any other type of rigidbody enters the trigger, then
					if (useWithAnyRigidbody) {
						//add force to that rigidbody
						Vector3 jumpDirection = transform.up * (jumpForce / 2) * currentRigidbody.mass;
						if (useParableLaunch) {
							jumpDirection = getParableSpeed (objectToImpulse.transform.position, targetPosition.position) * jumpForce;
							currentRigidbody.velocity = Vector3.zero;
							currentRigidbody.AddForce (jumpDirection, ForceMode.VelocityChange);
						} else {
							currentRigidbody.AddForce (jumpDirection, ForceMode.Impulse);
						}
					}
				}
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		//restore the original jump force in the player of the vehicle is the jump button is needed
		if (objectToImpulseList.Contains (objectToImpulse)) {
			if (objectToImpulse.tag == "Player") {
				if (useKeyToJumpWithPlayer) {
					playerControllerManager.useJumpPlatformWithKeyButton (false, jumpForce);
				}
			} else if (objectToImpulse.tag == "vehicle") {
				if (useKeyToJumpWithVehicles) {
					objectToImpulse.GetComponent<vehicleHUDManager> ().useJumpPlatformWithKeyButton (false, jumpForce);
				}
			}
			objectToImpulseList.Remove (objectToImpulse);
		} else if (currentParentAssignedSystem != null && currentParentAssignedSystem.gameObject == col.gameObject) {
			objectToImpulseList.Remove (objectToImpulse);
		}
		objectToImpulse = null;
	}

	Vector3 getParableSpeed (Vector3 origin, Vector3 target)
	{
		//get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		//remove the Y axis value
		toTargetXZ -= transform.InverseTransformDirection (toTargetXZ).y * transform.up;
		float y = transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		//get the velocity accoring to distance ang gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		//create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        

		//get direction of xz but with magnitude 1
		result *= v0xz; 

		// set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= transform.InverseTransformDirection (result).y * transform.up;
		result += transform.up * v0y;
		return result;
	}

	public void setPlatformEnabledState (bool state)
	{
		platformEnabled = state;
	}
}