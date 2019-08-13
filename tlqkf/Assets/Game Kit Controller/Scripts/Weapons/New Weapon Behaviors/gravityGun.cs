using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class gravityGun : MonoBehaviour
{
	public float holdDistance = 3;
	public float maxDistanceHeld = 4;
	public float maxDistanceGrab = 10;
	public float holdSpeed = 10;
	public float alphaTransparency = 0.5f;
	public float rotationSpeed;
	public float minTimeToIncreaseThrowForce = 300;
	public float increaseThrowForceSpeed = 1500;
	public float extraThorwForce = 10;
	public float maxThrowForce = 3500;
	public grabMode currentGrabMode;

	public bool grabInFixedPosition;
	public bool rotateToCameraInFixedPosition;
	public bool rotateToCameraInFreePosition;

	public float closestHoldDistanceInFixedPosition;
	public string grabbedObjectTag;
	public string grabbedObjectLayer;
	public Transform grabZoneTransform;

	public bool grabbedObjectCanBeBroken;

	public Shader pickableShader;
	public Slider powerSlider;

	public LayerMask layer;
	public bool enableTransparency = true;
	public ForceMode powerForceMode;

	public bool useThrowObjectsLayer = true;
	public LayerMask throwObjectsLayerToCheck;

	public float throwPower;
	public ForceMode realisticForceMode;

	public LayerMask gravityObjectsLayer;
	public string layerForCustomGravityObject;

	public bool changeGravityObjectsEnabled = true;

	public bool launchedObjectsCanMakeNoise;
	public float minObjectSpeedToActivateNoise;

	public enum grabMode
	{
		powers,
		realistic
	}

	public List<string> ableToGrabTags = new List<string> ();

	public playerController playerControllerManager;
	public otherPowers powersManager;
	public playerCamera playerCameraManager;

	public Transform mainCameraTransform;

	public UnityEvent secondaryFunctionEventPressDown;
	public UnityEvent secondaryFunctionEventPress;
	public UnityEvent secondaryFunctionEventPressUp;

	public simpleAnimationSystem animationSystem;

	public Transform rotor;
	public float rotorRotationSpeed = 20;

	public bool useForceWhenObjectDropped;
	public bool useForceWhenObjectDroppedOnFirstPerson;
	public bool useForceWhenObjectDroppedOnThirdPerson;
	public float forceWhenObjectDroppedOnFirstPerson;
	public float forceWhenObjectDroppedOnThirdPerson;

	public bool useEventOnObjectFound;
	public UnityEvent eventOnObjectFound;
	public UnityEvent eventOnObjectLost;

	public bool useEventOnObjectGrabbedDropped;
	public UnityEvent eventOnObjectGrabbed;
	public UnityEvent eventOnObjectDropped;

	public bool canUseWeapon = true;

	bool grabbed;

	GameObject objectHeld;

	bool objectFocus;
	GameObject currentObjectToGrabFound;
	Rigidbody objectHeldRigidbody;
	float holdTimer = 0;
	float timer = 0;
	RaycastHit hit;

	bool grabbedObjectTagLayerStored;
	string originalGrabbedObjectTag;
	int originalGrabbedObjectLayer;

	float orignalHoldDistance;

	Transform fixedGrabedTransform;
	RigidbodyConstraints objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
	Transform objectHeldFollowTransform;

	Vector3 nextObjectHeldPosition;
	Vector3 currentObjectHeldPosition;
	float currentMaxDistanceHeld;

	bool weaponActive;

	GameObject currentObjectToThrow;
	Transform currentHoldTransform;
	artificialObjectGravity currentArtificialObjectGravity;
	vehicleGravityControl currentVehicleGravityControl;

	Vector2 axisValues;
	float currentDistanceToGrabbedObject;

	bool currentObjectWasInsideGravityRoom;

	bool holdingCharacter;
	GameObject characterGrabbed;

	void Start ()
	{
		orignalHoldDistance = holdDistance;

		powerSlider.maxValue = maxThrowForce;
		powerSlider.value = maxThrowForce;
	}

	void Update ()
	{
		// if an object is grabbed, then move it from its original position, to the other in front of the camera
		if (objectHeld) {

			rotor.Rotate (0, 0, Time.deltaTime * rotorRotationSpeed);

			//get the transform for the grabbed object to follow
			currentHoldTransform = mainCameraTransform;
			if (playerCameraManager.is2_5ViewActive ()) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirection2_5d ();
				holdDistance = 0;
			}

			if (playerCameraManager.useTopDownView) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirectionTopDown ();
				holdDistance = 0;
			}

			currentDistanceToGrabbedObject = GKC_Utils.distance (objectHeld.transform.position, currentHoldTransform.position);
			if (!grabbed) {
				timer += Time.deltaTime;

				if ((currentDistanceToGrabbedObject <= currentMaxDistanceHeld || grabInFixedPosition) && timer > 0.5f) {
					grabbed = true;
					timer = 0;
				}
			}

			//if the object is not capable to move in front of the camera, because for example is being blocked for a wall, drop it
			if (currentDistanceToGrabbedObject > (currentMaxDistanceHeld + 2) && grabbed) {
				dropObject ();
			} else {
				//if the object is a cube, a turret, or anything that can move freely, set its position in front of the camera
				if (grabInFixedPosition) {
					nextObjectHeldPosition = fixedGrabedTransform.position + fixedGrabedTransform.forward * holdDistance;
					currentObjectHeldPosition = objectHeld.transform.position;
				} else {
					if (playerCameraManager.is2_5ViewActive ()) {
						nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * holdDistance;
					} else {
						nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * (holdDistance + objectHeld.transform.localScale.x);
					}
					currentObjectHeldPosition = objectHeld.transform.position;
				}

				objectHeldRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * holdSpeed;

				if ((rotateToCameraInFixedPosition && grabInFixedPosition) || (!grabInFixedPosition && rotateToCameraInFreePosition)) {
					objectHeld.transform.rotation = Quaternion.Slerp (objectHeld.transform.rotation, mainCameraTransform.rotation, Time.deltaTime * rotationSpeed);
				}
			}
		}
			
		if (weaponActive && !objectHeld && canUseWeapon) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer)) {

				if (currentObjectToGrabFound != hit.collider.gameObject) {
					currentObjectToGrabFound = hit.collider.gameObject;

					if (checkTypeObject (currentObjectToGrabFound)) {
						GameObject mainObjectFound = applyDamage.getCharacterOrVehicle (currentObjectToGrabFound);
						if (mainObjectFound == null) {
							grabObjectParent currentGrabObjectParent = currentObjectToGrabFound.GetComponent<grabObjectParent> ();
							if (currentGrabObjectParent) {
								mainObjectFound = currentGrabObjectParent.getObjectToGrab ();
							} else {
								mainObjectFound = currentObjectToGrabFound;
							}
						}
					}
				}

				if (checkTypeObject (hit.collider.gameObject) && !objectFocus) {

					animationSystem.playForwardAnimation ();

					objectFocus = true;

					checkObjectFoundOrLostState ();
				}

				if (!checkTypeObject (hit.collider.gameObject) && objectFocus) {

					animationSystem.playBackwardAnimation ();

					objectFocus = false;

					checkObjectFoundOrLostState ();
				}
			} else {
				if (objectFocus) {

					animationSystem.playBackwardAnimation ();
					
					objectFocus = false;

					checkObjectFoundOrLostState ();
				}
			}
		}

		if (grabbed && objectHeld == null) {
			dropObject ();
		}
	}

	public void checkObjectFoundOrLostState ()
	{
		if (useEventOnObjectFound) {
			if (objectFocus) {
				eventOnObjectFound.Invoke ();
			} else {
				eventOnObjectLost.Invoke ();
			}
		}
	}

	public void grabObject ()
	{
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer) && objectFocus) {
			grabCurrenObject (hit.collider.gameObject);
		} 
	}

	public void grabCurrenObject (GameObject objectToGrab)
	{
		if (checkTypeObject (objectToGrab)) {
			//reset the hold distance
			holdDistance = orignalHoldDistance;

			//if the located object is part of a vehicle, get the main vehicle object to grab it
			objectHeld = applyDamage.getVehicle (objectToGrab);
			characterDamageReceiver currentCharacterDamageReceiver = objectToGrab.GetComponent<characterDamageReceiver> ();
			if (objectHeld != null) {
				if (!objectHeld.GetComponent<Rigidbody> ().isKinematic) {
					//get the extra grab distance configurable for every vehicle
					holdDistance += objectHeld.GetComponent<vehicleHUDManager> ().extraGrabDistance;
					currentVehicleGravityControl = objectHeld.GetComponent<vehicleGravityControl> ();
					if (currentVehicleGravityControl) {
						currentVehicleGravityControl.pauseDownForce (true);
					}
				} else {
					objectHeld = null;
					return;
				}
			} 

			//if the located object is part of a character, get the main character object to grab it
			else if (currentCharacterDamageReceiver && !currentCharacterDamageReceiver.isCharacterDead ()) {
				objectHeld = currentCharacterDamageReceiver.character;

				if (applyDamage.isCharacter (objectHeld)) {
					objectHeld.SendMessage ("pushCharacter", Vector3.zero, SendMessageOptions.DontRequireReceiver);

					Animator currentObjectAnimator = objectHeld.GetComponent<Animator> ();
					if (currentObjectAnimator) {
						holdingCharacter = true;
						characterGrabbed = objectHeld;

						objectHeld.SendMessage ("setCheckGetUpPausedState", true, SendMessageOptions.DontRequireReceiver);
						objectHeld = currentObjectAnimator.GetBoneTransform (HumanBodyBones.Hips).gameObject;
					}
				}
			} 

			//else it is an object from the able to grab list
			else {
				grabObjectParent currentGrabObjectParent = objectToGrab.GetComponent<grabObjectParent> ();
				if (currentGrabObjectParent) {
					objectHeld = currentGrabObjectParent.getObjectToGrab ();
				} else {
					objectHeld = objectToGrab;
				}
			}

			//get its tag, to set it again to the object, when it is dropped
			if (!objectHeld.GetComponent<vehicleHUDManager> ()) {
				grabbedObjectTagLayerStored = true;
				originalGrabbedObjectTag = objectHeld.tag.ToString ();
				originalGrabbedObjectLayer = objectHeld.layer;
				objectHeld.tag = grabbedObjectTag;
				objectHeld.layer = LayerMask.NameToLayer (grabbedObjectLayer);
			}

			objectHeldRigidbody = objectHeld.GetComponent<Rigidbody> ();
			if (objectHeldRigidbody) {
				objectHeldRigidbody.isKinematic = false;
				objectHeldRigidbody.useGravity = false;
				objectHeldRigidbody.velocity = Vector3.zero;
			}

			//if the object has its gravity modified, pause that script
			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();
			if (currentArtificialObjectGravity) {
				currentArtificialObjectGravity.active = false;
			}

			explosiveBarrel currentExplosiveBarrel = objectHeld.GetComponent<explosiveBarrel> ();
			if (currentExplosiveBarrel) {
				currentExplosiveBarrel.setExplosiveBarrelOwner (gameObject);
			}

			if (!grabbedObjectCanBeBroken) {
				if (currentExplosiveBarrel) {
					currentExplosiveBarrel.canExplodeState (false);
				}
				crate currentCrate = objectHeld.GetComponent<crate> ();
				if (currentCrate) {
					currentCrate.crateCanBeBrokenState (false);
				}
			}

			pickUpObject currentPickUpObject = objectHeld.GetComponent<pickUpObject> ();
			if (currentPickUpObject) {
				currentPickUpObject.activateObjectTrigger ();
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent<grabObjectEventSystem> ();
			if (currentGrabObjectEventSystem) {
				currentGrabObjectEventSystem.callEventOnGrab ();
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();
			if (currentObjectToPlaceSystem) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (true);
			}

			if (currentGrabMode == grabMode.powers) {
				if (objectHeldRigidbody) {
					objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
					objectHeldRigidbody.freezeRotation = true;
				}

				grabbedObjectState currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();
				if (!currentGrabbedObjectState) {
					currentGrabbedObjectState = objectHeld.AddComponent<grabbedObjectState> ();
				}
				if (currentGrabbedObjectState) {
					currentGrabbedObjectState.setCurrentHolder (gameObject);
					currentGrabbedObjectState.setGrabbedState (true);
				}
			} else {
				if (objectHeldRigidbody) {
					if (!objectHeld.GetComponent<vehicleHUDManager> ()) {
						objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
						objectHeldRigidbody.freezeRotation = true;
					}
				}
			}

			//if the object grabbed is an AI, pause it
			objectHeld.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);

			if (objectHeld.GetComponent<pickUpObject> ()) {
				objectHeld.transform.SetParent (null);
			}
				
			//if the transparency is enabled, change all the color of all the materials of the object
			if (enableTransparency) {
				outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();
				if (currentOutlineObjectSystem) {
					currentOutlineObjectSystem.setTransparencyState (true, pickableShader, alphaTransparency);
				}
			}
				
			powerSlider.value = 0;
			powerSlider.maxValue = maxThrowForce;

			holdTimer = 0;
				
			if (!fixedGrabedTransform) {
				GameObject fixedPositionObject = new GameObject ();
				fixedGrabedTransform = fixedPositionObject.transform;
				fixedGrabedTransform.name = "Fixed Grabed Tansform";
			}

			if (grabInFixedPosition) {
				fixedGrabedTransform.SetParent (mainCameraTransform);
				fixedGrabedTransform.transform.position = objectHeld.transform.position;
				fixedGrabedTransform.transform.rotation = mainCameraTransform.rotation;
				currentMaxDistanceHeld = GKC_Utils.distance (objectHeld.transform.position, mainCameraTransform.position) + holdDistance;
				holdDistance = 0;
			} else {
				currentMaxDistanceHeld = maxDistanceHeld;
			}

			currentObjectWasInsideGravityRoom = false;
		}
	}

	//check if the object detected by the raycast is in the able to grab list or is a vehicle
	public bool checkTypeObject (GameObject objectToCheck)
	{
		if (ableToGrabTags.Contains (objectToCheck.tag.ToString ())) {
			return true;
		}

		if (objectToCheck.GetComponent<vehicleDamageReceiver> () || objectToCheck.GetComponent<vehicleHUDManager> ()) {
			return true;
		}

		characterDamageReceiver currentCharacterDamageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (currentCharacterDamageReceiver) {
			if (ableToGrabTags.Contains (currentCharacterDamageReceiver.character.tag.ToString ())) {
				return true;
			}
		}

		grabObjectParent currentGrabObjectParent = objectToCheck.GetComponent<grabObjectParent> ();
		if (currentGrabObjectParent) {
			return true;
		}

		return false;
	}

	Vector3 throwObjectDirection;

	//drop the object
	public void dropObject ()
	{
		if (useThrowObjectsLayer) {
			throwObjectDirection = Vector3.zero;

			if (objectHeld) {
				if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, throwObjectsLayerToCheck)) {
					Vector3 heading = hit.point - objectHeld.transform.position;
					float distance = heading.magnitude;
					throwObjectDirection = heading / distance;
					throwObjectDirection.Normalize ();
				}
			}
		}

		if (objectHeld) {
			//set the tag of the object that had before grab it, and if the object has its own gravity, enable again
			if (grabbedObjectTagLayerStored) {
				objectHeld.tag = originalGrabbedObjectTag;
				objectHeld.layer = originalGrabbedObjectLayer;
				grabbedObjectTagLayerStored = false;
			}

			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();
			if (currentArtificialObjectGravity) {
				currentArtificialObjectGravity.active = true;
			}

			explosiveBarrel currentExplosiveBarrel = objectHeld.GetComponent<explosiveBarrel> ();

			if (!grabbedObjectCanBeBroken) {
				if (currentExplosiveBarrel) {
					currentExplosiveBarrel.canExplodeState (true);
				}

				crate currentCrate = objectHeld.GetComponent<crate> ();
				if (currentCrate) {
					currentCrate.crateCanBeBrokenState (true);
				}
			}

			currentVehicleGravityControl = objectHeld.GetComponent<vehicleGravityControl> ();
			if (objectHeldRigidbody) {
				if (!currentVehicleGravityControl) {
					objectHeldRigidbody.useGravity = true;
				}
					
				objectHeldRigidbody.freezeRotation = false;
				if (objectHeldRigidbodyConstraints != RigidbodyConstraints.None) {
					objectHeldRigidbody.constraints = objectHeldRigidbodyConstraints;
					objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
				}
			}

			if (currentVehicleGravityControl) {
				currentVehicleGravityControl.pauseDownForce (false);
			}

			if (enableTransparency) {
				//set the normal shader of the object 
				outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();
				if (currentOutlineObjectSystem) {
					currentOutlineObjectSystem.setTransparencyState (false, null, 0);
				}
			}

			//if the grabbed object is a turret, call a function to make it kinematic again when it will touch a surface
			if (objectHeld.tag == "friend") {
				objectHeld.SendMessage ("dropCharacter", true, SendMessageOptions.DontRequireReceiver);
			}
			//if the object grabbed is an AI, enable it
			objectHeld.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);

			if (holdingCharacter) {
				characterGrabbed.SendMessage ("setCheckGetUpPausedState", false, SendMessageOptions.DontRequireReceiver);
			}

			grabbedObjectState currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();
			if (currentGrabbedObjectState) {
				currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();
				currentGrabbedObjectState.checkGravityRoomState ();
				currentGrabbedObjectState.setGrabbedState (false);
				if (!currentObjectWasInsideGravityRoom) {
					Destroy (currentGrabbedObjectState);
				}
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();
			if (currentObjectToPlaceSystem) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (false);
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent <grabObjectEventSystem> ();
			if (currentGrabObjectEventSystem) {
				currentGrabObjectEventSystem.callEventOnDrop ();
			}
		}

		grabbed = false;
		objectHeld = null;
		objectHeldRigidbody = null;

		animationSystem.playBackwardAnimation ();

		objectFocus = false;
	}

	public void checkJointsInObject (GameObject objectToThrow, float force)
	{
		CharacterJoint currentCharacterJoint = objectToThrow.GetComponent<CharacterJoint> ();
		if (currentCharacterJoint) {
			checkJointsInObject (currentCharacterJoint.connectedBody.gameObject, force);
		} else {
			addForceToThrownRigidbody (objectToThrow, force);
		}
	}

	public void addForceToThrownRigidbody (GameObject objectToThrow, float force)
	{
		Component[] components = objectToThrow.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Component c in components) {
			Rigidbody currentRigid = c as Rigidbody;
			if (!currentRigid.isKinematic && currentRigid.GetComponent<Collider> ()) {
				Vector3 forceDirection = mainCameraTransform.forward;

				if (useThrowObjectsLayer) {
					//print (throwObjectDirection + " " + forceDirection);
					if (throwObjectDirection != Vector3.zero) {
						forceDirection = throwObjectDirection;
					}
				}

				forceDirection *= force * currentRigid.mass;

				currentRigid.AddForce (forceDirection, powerForceMode);

				checkIfEnableNoiseOnCollision (objectToThrow, forceDirection.magnitude);
			}
		}
	}

	public void throwRealisticGrabbedObject ()
	{
		Rigidbody currentRigidbody = objectHeld.GetComponent<Rigidbody> ();

		dropObject ();

		if (currentRigidbody) {
			Vector3 forceDirection = mainCameraTransform.forward * throwPower * currentRigidbody.mass;
			currentRigidbody.AddForce (forceDirection, realisticForceMode);

			checkIfEnableNoiseOnCollision (objectHeld, forceDirection.magnitude);
		}
	}

	Coroutine setFixedGrabbedTransformCoroutine;

	public void resetFixedGrabedTransformPosition ()
	{
		stopResetFixedGrabedTransformPositionCoroutine ();
		setFixedGrabbedTransformCoroutine = StartCoroutine (resetFixedGrabedTransformPositionCoroutine ());
	}

	public void stopResetFixedGrabedTransformPositionCoroutine ()
	{
		if (setFixedGrabbedTransformCoroutine != null) {
			StopCoroutine (setFixedGrabbedTransformCoroutine);
		}
	}

	IEnumerator resetFixedGrabedTransformPositionCoroutine ()
	{
		Vector3 targetPosition = Vector3.forward * orignalHoldDistance;

		float dist = GKC_Utils.distance (fixedGrabedTransform.position, mainCameraTransform.position + targetPosition);
		float duration = dist / 10;
		float t = 0;
		bool targetReached = false;

		float timer = 0;

		while (!targetReached && t < 1 && fixedGrabedTransform.localPosition != targetPosition) {
			t += Time.deltaTime / duration;
			fixedGrabedTransform.localPosition = Vector3.Lerp (fixedGrabedTransform.localPosition, targetPosition, t);

			timer += Time.deltaTime;
			if (GKC_Utils.distance (fixedGrabedTransform.localPosition, targetPosition) < 0.01f || timer > duration) {
				targetReached = true;
			}

			yield return null;
		}
	}

	public void checkIfDropObject (GameObject objectToCheck)
	{
		if (objectHeld == objectToCheck) {
			dropObject ();
		}
	}

	public void setWeaponActiveState (bool state)
	{
		weaponActive = state;

		dropObject ();
	}

	public GameObject getGrabbedObject ()
	{
		return objectHeld;
	}

	public bool isGrabbedObject ()
	{
		return objectHeld != null;
	}

	public void checkIfEnableNoiseOnCollision (GameObject objectToCheck, float launchSpeed)
	{
		if (launchedObjectsCanMakeNoise && launchSpeed >= minObjectSpeedToActivateNoise) {
			noiseSystem currentNoiseSystem = objectToCheck.GetComponent<noiseSystem> ();
			if (currentNoiseSystem) {
				currentNoiseSystem.setUseNoiseState (true);
			}
		}
	}

	public void addForceToLaunchObject ()
	{
		if (currentGrabMode == grabMode.powers) {
			if (holdTimer < maxThrowForce) {
				holdTimer += Time.deltaTime * increaseThrowForceSpeed;

				powerSlider.value += Time.deltaTime * increaseThrowForceSpeed;
			}
		}
	}

	public void launchObject ()
	{
		powerSlider.value = 0;

		currentObjectToThrow = objectHeld;

		Rigidbody currentRigidbody = currentObjectToThrow.GetComponent<Rigidbody> ();

		dropObject ();

		bool canAddForceToObjectDropped = false;

		if (currentRigidbody) {
			//if the button has been pressed and released quickly, drop the object, else addforce to its rigidbody
			if (!currentObjectToThrow.GetComponent<vehicleGravityControl> () && !currentObjectWasInsideGravityRoom) {
				currentRigidbody.useGravity = true;
			}

			if (currentGrabMode == grabMode.powers) {
				if (holdTimer > minTimeToIncreaseThrowForce) {
					currentObjectToThrow.AddComponent<launchedObjects> ().setCurrentPlayer (gameObject);

					if (currentObjectToThrow.GetComponent<CharacterJoint> ()) {
						checkJointsInObject (currentObjectToThrow, holdTimer);
					} else {
						holdTimer += extraThorwForce;
						addForceToThrownRigidbody (currentObjectToThrow, holdTimer);
					}
				} else {
					canAddForceToObjectDropped = true;
				}
			} else {
				canAddForceToObjectDropped = true;
			}
		}

		if (canAddForceToObjectDropped) {
			if (useForceWhenObjectDropped) {
				if (useForceWhenObjectDroppedOnFirstPerson && playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnFirstPerson);
				} else if (useForceWhenObjectDroppedOnThirdPerson && !playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnThirdPerson);
				}
			}
		}
	}

	public bool isCurrentObjectToGrabFound (GameObject objectToCheck)
	{
		if (currentObjectToGrabFound == objectToCheck) {
			return true;
		}

		return false;
	}

	//CALL INPUT FUNCTIONS
	public void inputGrabObject ()
	{
		//if the player is in aim mode, grab an object
		if (!playerControllerManager.usingDevice && weaponActive && !objectHeld && canUseWeapon) {
			grabObject ();
		}
	}

	public void inputHoldToLaunchObject ()
	{
		//if the drop button is being holding, add force to the final velocity of the drooped object
		if (grabbed) {
			addForceToLaunchObject ();
		}
	}

	public void inputReleaseToLaunchObject ()
	{
		//when the button is released, check the amount of strength accumulated
		if (grabbed) {
			launchObject ();

			if (useEventOnObjectGrabbedDropped) {
				eventOnObjectDropped.Invoke ();
			}
		} else {
			if (objectHeld) {
				if (useEventOnObjectGrabbedDropped) {
					eventOnObjectGrabbed.Invoke ();
				}
			}
		}
	}

	public void inputResetFixedGrabedTransformPosition ()
	{
		if (objectHeld) {
			resetFixedGrabedTransformPosition ();
		}
	}

	public void activateSecondaryFunctionPressDown ()
	{
		secondaryFunctionEventPressDown.Invoke ();
	}

	public void activateSecondaryFunctionPress ()
	{
		secondaryFunctionEventPress.Invoke ();
	}

	public void activateSecondaryFunctionPressUp ()
	{
		secondaryFunctionEventPressUp.Invoke ();
	}

	public void setNewGravityToGrabbedObject ()
	{
		if (changeGravityObjectsEnabled) {
			GameObject grabbedObject = getGrabbedObject ();

			if (grabbedObject.GetComponent<Rigidbody> ()) {
				dropObject ();

				//if the current object grabbed is a vehicle, enable its own gravity control component
				currentVehicleGravityControl = grabbedObject.GetComponent<vehicleGravityControl> ();

				if (currentVehicleGravityControl) {
					currentVehicleGravityControl.activateGravityPower (mainCameraTransform.TransformDirection (Vector3.forward), 
						mainCameraTransform.TransformDirection (Vector3.right));
				} else {
					//else, it is a regular object
					//change the layer, because the object will use a raycast to check the new normal when a collision happens
					grabbedObject.layer = LayerMask.NameToLayer (layerForCustomGravityObject);

					//if the object has a regular gravity, attach the scrip and set its values
					currentArtificialObjectGravity = grabbedObject.GetComponent<artificialObjectGravity> ();
					if (!currentArtificialObjectGravity) {
						currentArtificialObjectGravity = grabbedObject.AddComponent<artificialObjectGravity> ();
					} 
					currentArtificialObjectGravity.enableGravity (gravityObjectsLayer, powersManager.settings.highFrictionMaterial, mainCameraTransform.forward);
				}
			}
		}
	}

	public bool isCarryingObject ()
	{
		return grabbed;
	}

	public void setCanUseWeaponState (bool state)
	{
		canUseWeapon = state;

		if (!canUseWeapon) {
			if (objectHeld != null) {
				dropObject ();
			}

			if (objectFocus) {
				animationSystem.playBackwardAnimation ();

				objectFocus = false;

				checkObjectFoundOrLostState ();
			}
		}
	}

	public void rotateRotor (float newSpeed)
	{
		rotor.Rotate (0, 0, Time.deltaTime * newSpeed);
	}
}
