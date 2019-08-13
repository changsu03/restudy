using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class attrackObjectsSystem : MonoBehaviour
{
	public bool attractionEnabled;

	public bool attractionApplyForce = true;
	public bool attractionActive;
	public float attractionRadius = 20;

	public Collider playerCollider;
	public GameObject playerGameObject;

	public LayerMask layerToSearch;

	public Transform attractionPosition;

	public float attractionVelocity = 10;
	public float attractionForce = 1000;
	public ForceMode forceMode;

	public float maxThrowForce = 3500;
	public float increaseThrowForceSpeed = 1500;

	public List<string> tagToLocate = new List<string> ();
	public Slider powerSlider;

	public bool carryingObjects;

	public Transform mainCameraTransform;

	public LayerMask layerToDamage;

	public UnityEvent eventsOnActivateAttraction;
	public UnityEvent eventOnDeactivateAttraction;

	float currentForceToLaunchObjects;

	Rigidbody currentRigidbody;

	Vector3 attractDirection;

	Vector3 currentPosition;

	float holdTimer = 0;

	RaycastHit hit;

	List<grabbedObject> grabbedObjectList = new List<grabbedObject> ();

	float lastTimeActivateAttraction;

	Vector3 nextObjectHeldPosition;
	Vector3 currentObjectHeldPosition;

	void FixedUpdate ()
	{
		if (attractionActive) {

			if (!carryingObjects) {
				return;
			}

			currentPosition = attractionPosition.position;

			for (int i = 0; i < grabbedObjectList.Count; i++) {
				if (grabbedObjectList [i].objectToMove != null) {
					currentRigidbody = grabbedObjectList [i].mainRigidbody;
						
					if (currentRigidbody) {
//						Vector3 direction = currentPosition - currentRigidbody.position;
//
//						float gravityForce = attractionForce * ((mass * currentRigidbody.mass) / direction.sqrMagnitude);
//						gravityForce /= currentRigidbody.mass;
//
//						currentRigidbody.AddForce (direction.normalized * gravityForce * Time.fixedDeltaTime);

//						Vector3 gravityVector = currentPosition -  currentRigidbody.position;
//						float gravityDistance = Vector3.Distance(currentPosition, currentRigidbody.position);
//						Vector3 gravityStrength = Vector3.zero;                                                                                 
//						gravityStrength.x = attractionForce / Mathf.Pow(gravityDistance, 2);
//						gravityStrength.z = attractionForce / Mathf.Pow(gravityDistance, 2);
//						if (gravityDistance < attractionRadius)
//							currentRigidbody.velocity += (Vector3.Scale(gravityStrength, gravityVector));

						if (attractionApplyForce) {
							attractDirection = currentPosition - currentRigidbody.position;
							currentRigidbody.AddForce (attractDirection.normalized * attractionForce * currentRigidbody.mass * Time.fixedDeltaTime, forceMode);
						} else {
							nextObjectHeldPosition = currentPosition + attractionPosition.forward;
							
							currentObjectHeldPosition = currentRigidbody.position;

							currentRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * attractionVelocity;
						}
					}
				}
			}
		}
	}

	public void setAttractionEnabledState (bool state)
	{
		attractionEnabled = state;
	}

	public void inputGrabObjects ()
	{
		if (!attractionEnabled) {
			return;
		}

		if (carryingObjects) {
			return;
		}

		lastTimeActivateAttraction = Time.time;

		attractionActive = true;

		grabCloseObjects ();

		if (carryingObjects) {
			eventsOnActivateAttraction.Invoke ();
		}
	}

	public void inputHoldToLaunchObject ()
	{
		if (!attractionEnabled) {
			return;
		}

		if (carryingObjects) {
			if (Time.time > lastTimeActivateAttraction + 0.5f) {
				addForceToLaunchObjects ();
			}
		}
	}

	public void inputReleaseToLaunchObject ()
	{
		if (!attractionEnabled) {
			return;
		}

		if (carryingObjects) {
			if (Time.time > lastTimeActivateAttraction + 0.5f) {
				dropObjects ();

				attractionActive = false;

				eventOnDeactivateAttraction.Invoke ();
			}
		}
	}

	public void grabCloseObjects ()
	{
		//if the player has not grabbedObjects, store them
		if (grabbedObjectList.Count == 0) {
			//check in a radius, the close objects which can be grabbed
			currentPosition = attractionPosition.position;

			Collider[] objects = Physics.OverlapSphere (currentPosition, attractionRadius, layerToSearch);
			foreach (Collider hits in objects) {
				
				Collider currentCollider = hits.GetComponent<Collider> ();
				Rigidbody currentRigidbody = hits.GetComponent<Rigidbody> ();

				if (tagToLocate.Contains (currentCollider.tag.ToString ()) && currentRigidbody) {
					if (currentRigidbody.isKinematic) {
						currentRigidbody.isKinematic = false;
					}

					grabbedObject newGrabbedObject = new grabbedObject ();
					//removed tag and layer after store them, so the camera can still use raycast properly
					GameObject currentObject = hits.gameObject;
					newGrabbedObject.objectToMove = currentObject;
					newGrabbedObject.objectTag = currentObject.tag;
					newGrabbedObject.objectLayer = currentObject.layer;
					newGrabbedObject.mainRigidbody = currentRigidbody;

					currentObject.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);
					currentObject.tag = "Untagged";
					currentObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
					currentRigidbody.useGravity = false;

					//get the distance from every object to left and right side of the player, to set every side as parent of every object
					//disable collisions between the player and the objects, to avoid issues
					Physics.IgnoreCollision (currentCollider, playerCollider, true);

					//if any object grabbed has its own gravity, paused the script to move the object properly
					artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();
					if (currentArtificialObjectGravity) {
						currentArtificialObjectGravity.active = false;
					}

					explosiveBarrel currentExplosiveBarrel = currentObject.GetComponent<explosiveBarrel> ();
					if (currentExplosiveBarrel) {
						currentExplosiveBarrel.setExplosiveBarrelOwner (playerGameObject);
					}

					grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();
					if (!currentGrabbedObjectState) {
						currentGrabbedObjectState = currentObject.AddComponent<grabbedObjectState> ();
					}

					objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();
					if (currentObjectToPlaceSystem) {
						currentObjectToPlaceSystem.setObjectInGrabbedState (true);
					}

					//if any object is pickable and is inside an opened chest, activate its trigger or if it has been grabbed by the player, remove of the list
					pickUpObject currentPickUpObject = currentObject.GetComponent<pickUpObject> ();
					if (currentPickUpObject) {
						currentPickUpObject.activateObjectTrigger ();
					}

					deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();
					if (currentDeviceStringAction) {
						currentDeviceStringAction.setIconEnabledState (false);
					}

					if (currentGrabbedObjectState) {
						currentGrabbedObjectState.setCurrentHolder (gameObject);
						currentGrabbedObjectState.setGrabbedState (true);
					}

					grabbedObjectList.Add (newGrabbedObject);
				}
			}

			//if there are not any object close to the player, cancel 
			if (grabbedObjectList.Count > 0) {
				carryingObjects = true;
			} else {
				attractionActive = false;
			}

			powerSlider.maxValue = maxThrowForce;

			currentForceToLaunchObjects = 0;
		} 
	}

	//drop or throw the current grabbed objects
	public void dropObjects ()
	{
		//get the point at which the camera is looking, to throw the objects in that direction
		Vector3 hitDirection = Vector3.zero;
		bool surfaceFound = false;
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
			if (hit.collider != playerCollider) {
				surfaceFound = true;
			} else {
				if (Physics.Raycast (hit.point + mainCameraTransform.forward, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
					surfaceFound = true;
				}
			}
		}

		if (surfaceFound) {
			hitDirection = hit.point;
		}

		for (int j = 0; j < grabbedObjectList.Count; j++) {
			dropObject (grabbedObjectList [j], hitDirection);
		}

		carryingObjects = false;
		grabbedObjectList.Clear ();

		powerSlider.value = 0;
	}

	public void addForceToLaunchObjects ()
	{
		if (currentForceToLaunchObjects < maxThrowForce) {
			//enable the power slider in the center of the screen
			currentForceToLaunchObjects += Time.deltaTime * increaseThrowForceSpeed;
			if (currentForceToLaunchObjects > 300) {
				
				powerSlider.value = currentForceToLaunchObjects;
			}
		}
	}

	public void dropObject (grabbedObject currentGrabbedObject, Vector3 launchDirection)
	{
		GameObject currentObject = currentGrabbedObject.objectToMove;

		Rigidbody currentRigidbody = currentGrabbedObject.mainRigidbody;

		currentObject.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);
		currentObject.transform.SetParent (null);

		currentObject.tag = currentGrabbedObject.objectTag.ToString ();
		currentObject.layer = currentGrabbedObject.objectLayer;

		//drop the objects, because the grab objects button has been pressed quickly
		if (currentForceToLaunchObjects < 300) {
			Physics.IgnoreCollision (currentObject.GetComponent<Collider> (), playerCollider, false);
		}

		//launch the objects according to the amount of time that the player has held the buttton
		if (currentForceToLaunchObjects > 300) {
			//if the objects are launched, add the script launchedObject, to damage any enemy that the object would touch
			currentObject.AddComponent<launchedObjects> ().setCurrentPlayerAndCollider (playerGameObject, playerCollider);
			//if there are any collider in from of the camera, use the hit point, else, use the camera direciton
			if (launchDirection != Vector3.zero) {
				Vector3 throwDirection = launchDirection - currentObject.transform.position;
				throwDirection = throwDirection / throwDirection.magnitude;
				currentRigidbody.AddForce (throwDirection * currentForceToLaunchObjects * currentRigidbody.mass);
			} else {
				currentRigidbody.AddForce (mainCameraTransform.TransformDirection (Vector3.forward) * currentForceToLaunchObjects * currentRigidbody.mass);
			}
		}

		//set again the custom gravity of the object
		artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();
		if (currentArtificialObjectGravity) {
			currentArtificialObjectGravity.active = true;
		}

		explosiveBarrel currentExplosiveBarrel = currentObject.GetComponent<explosiveBarrel> ();
	
		deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();
		if (currentDeviceStringAction) {
			currentDeviceStringAction.setIconEnabledState (true);
		}

		objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();
		if (currentObjectToPlaceSystem) {
			currentObjectToPlaceSystem.setObjectInGrabbedState (false);
		}

		grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();
		if (currentGrabbedObjectState) {
			bool currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();
			currentGrabbedObjectState.checkGravityRoomState ();
			currentGrabbedObjectState.setGrabbedState (false);
			if (!currentObjectWasInsideGravityRoom) {
				Destroy (currentGrabbedObjectState);
				currentRigidbody.useGravity = true;
			}
		}
	}


	[System.Serializable]
	public class grabbedObject
	{
		public GameObject objectToMove;
		public string objectTag;
		public int objectLayer;
		public Rigidbody mainRigidbody;
	}
}
