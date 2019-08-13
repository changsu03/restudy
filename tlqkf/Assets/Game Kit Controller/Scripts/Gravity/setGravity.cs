using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class setGravity : MonoBehaviour
{
	public bool useWithPlayer;
	public bool useWithNPC;
	public bool useWithVehicles;
	public bool useWithAnyRigidbody;

	public bool checkOnlyForArtificialGravitySystem;

	public triggerType typeOfTrigger;

	public enum triggerType
	{
		enter,
		exit,
		both
	}

	public bool setGravityMode = true;
	public bool setRegularGravity;
	public bool useCustomGravityDirection;
	public Vector3 customGravityDirection;

	public bool useCenterPoint;
	public Transform centerPoint;
	public bool useCenterPointForRigidbodies;
	public bool useInverseDirectionToCenterPoint;

	public bool useCenterPointList;
	public bool useCenterIfPointListTooClose = true;
	public List<Transform> centerPointList = new List<Transform> ();
	public bool useCenterPointListForRigidbodies;

	public bool changeGravityDirectionActive;

	public bool rotateToSurfaceSmoothly = true;

	public bool setCircumnavigateSurfaceState;
	public bool circumnavigateSurfaceState;

	public bool preservePlayerVelocity = true;

	public bool storeSetGravityManager = true;

	public bool setTargetParent;
	public GameObject targetParent;
	public bool setRigidbodiesParent;

	public bool useAnimation = true;
	public string animationName = "arrowAnim";

	public List<artificialObjectGravity> artificialObjectGravityList = new List<artificialObjectGravity> ();

	bool inside;
	GameObject objectToChangeGravity;
	GameObject character;
	grabObjects grabObjectsManager;
	grabbedObjectState currentGrabbedObject;
	Animation mainAnimation;

	void Start ()
	{
		mainAnimation = GetComponent<Animation> ();

		if (centerPoint == null) {
			centerPoint = transform;
		}
	}

	//set a custom gravity for the player and the vehicles, in the direction of the arrow
	void Update ()
	{
		if (useAnimation) {
			if (mainAnimation) {
				mainAnimation.Play (animationName);
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (typeOfTrigger == triggerType.enter || typeOfTrigger == triggerType.both) {
			checkTriggerType (col, true);
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (typeOfTrigger == triggerType.exit || typeOfTrigger == triggerType.both) {
			checkTriggerType (col, false);
		}
	}

	public void checkTriggerType (Collider col, bool isEnter)
	{
		objectToChangeGravity = col.gameObject;
		//if the player is not driving, stop the gravity power
		if ((objectToChangeGravity.tag == "Player" && useWithPlayer) ||
		    (objectToChangeGravity.tag == "friend" && useWithNPC) ||
		    (objectToChangeGravity.tag == "enemy" && useWithNPC)) {

			playerController currentPlayerController = objectToChangeGravity.GetComponent<playerController> ();
			if (!currentPlayerController.driving && !currentPlayerController.jetPackEquiped && !currentPlayerController.flyModeActive) {
				gravitySystem currentGravitySystem = objectToChangeGravity.GetComponent<gravitySystem> ();

				if (currentGravitySystem) {
					if (currentGravitySystem.getCurrentSetGravityManager () != this) {

						if (storeSetGravityManager) {
							if (isEnter) {
								currentGravitySystem.setCurrentSetGravityManager (this);
							} else {
								currentGravitySystem.setCurrentSetGravityManager (null);
							}
						}

						if (setCircumnavigateSurfaceState) {
							currentGravitySystem.setCircumnavigateSurfaceState (circumnavigateSurfaceState);
						} 

						if (setGravityMode) {
							if (setRegularGravity) {
								currentGravitySystem.deactivateGravityPower ();
							} else {
								if (changeGravityDirectionActive) {
									Vector3 newGravityDirection = -getGravityDirection (objectToChangeGravity.transform.position);

									if (currentGravitySystem.getCurrentNormal () != newGravityDirection) {
										currentGravitySystem.changeGravityDirectionDirectly (newGravityDirection, preservePlayerVelocity);
									}

									if (setTargetParent) {
										currentGravitySystem.addParent (targetParent);
									}
								} else {
									objectToChangeGravity.GetComponent<playerStatesManager> ().checkPlayerStates (true, true, true, true, true, false, true, true);

									if (rotateToSurfaceSmoothly) {
										currentGravitySystem.changeOnTrigger (getGravityDirection (objectToChangeGravity.transform.position), transform.right);
									} else {
										currentGravitySystem.setNormal (getGravityDirection (objectToChangeGravity.transform.position));
									}
								}

							}
						}
					}
				}
			}
		} else if (objectToChangeGravity.GetComponent<Rigidbody> ()) {

			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = objectToChangeGravity.GetComponent<grabbedObjectState> ();
			if (currentGrabbedObject) {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectToChangeGravity);
			}

			//if the player is driving, disable the gravity control in the vehicle
			vehicleGravityControl currentVehicleGravityControl = objectToChangeGravity.GetComponent<vehicleGravityControl> ();
			if (currentVehicleGravityControl && useWithVehicles) {
				if (currentVehicleGravityControl.gravityControlEnabled) {
					if (setRegularGravity) {
						currentVehicleGravityControl.deactivateGravityPower ();
					} else {
						currentVehicleGravityControl.activateGravityPower (getGravityDirection (currentVehicleGravityControl.transform.position), transform.right);
					}
				}
			} else {
				if (useWithAnyRigidbody || checkOnlyForArtificialGravitySystem) {

					artificialObjectGravity currentArtificialObjectGravity = objectToChangeGravity.GetComponent<artificialObjectGravity> ();

					if (checkOnlyForArtificialGravitySystem) {
						objectToUseArtificialGravitySystem currentObjectToUseArtificialGravitySystem = objectToChangeGravity.GetComponent<objectToUseArtificialGravitySystem> ();
						if (!currentObjectToUseArtificialGravitySystem && !currentArtificialObjectGravity) {
							return;
						}
					}

					if (currentArtificialObjectGravity == null) {
						currentArtificialObjectGravity = objectToChangeGravity.AddComponent<artificialObjectGravity> ();
					}
						

					if (setRegularGravity) {

						if (artificialObjectGravityList.Contains (currentArtificialObjectGravity)) {
							artificialObjectGravityList.Remove (currentArtificialObjectGravity);
						}

						currentArtificialObjectGravity.removeGravity ();

						if (setRigidbodiesParent) {
							objectToChangeGravity.transform.SetParent (null);
						}
					} else {
						if (!artificialObjectGravityList.Contains (currentArtificialObjectGravity)) {
							artificialObjectGravityList.Add (currentArtificialObjectGravity);
						}

						if (useCenterPointForRigidbodies && centerPoint) {
							currentArtificialObjectGravity.setUseCenterPointActiveState (useCenterPointForRigidbodies, centerPoint);
						}

						if (useCenterPointListForRigidbodies) { 
							currentArtificialObjectGravity.setUseCenterPointListForRigidbodiesState (useCenterPointListForRigidbodies, centerPointList);
						}
							
						currentArtificialObjectGravity.setUseInverseDirectionToCenterPointState (useInverseDirectionToCenterPoint);
					
						currentArtificialObjectGravity.setCurrentGravity (getGravityDirection (objectToChangeGravity.transform.position));

						if (setRigidbodiesParent) {
							objectToChangeGravity.transform.SetParent (targetParent.transform);
						} else {
							objectToChangeGravity.transform.SetParent (null);
						}
					}
				}
			}
		}
	}

	public Vector3 getGravityDirection (Vector3 objectPosition)
	{
		if (useCustomGravityDirection) {
			return customGravityDirection;
		} else {
			if (useCenterPoint) {
				Transform centerPointToUse = transform;
				if (useCenterPointList) {
					float minDistance = Mathf.Infinity;
					for (int i = 0; i < centerPointList.Count; i++) {
						float currentDistance = GKC_Utils.distance (objectPosition, centerPointList [i].position);
						if (currentDistance < minDistance) {
							minDistance = currentDistance;
							centerPointToUse = centerPointList [i];
						}
					}

					if (useCenterIfPointListTooClose) {
						float distanceToCenter = GKC_Utils.distance (objectPosition, centerPoint.position);
						if (minDistance < distanceToCenter) {
							centerPointToUse = centerPoint;
						}
					}
				} else {
					centerPointToUse = centerPoint;
				}

				Vector3 heading = centerPointToUse.position - objectToChangeGravity.transform.position;
				float distance = heading.magnitude;
				Vector3 direction = heading / distance;
				return direction;
			} else {
				return transform.up;
			}
		}
	}

	public void reverseGravityDirection ()
	{
		useInverseDirectionToCenterPoint = !useInverseDirectionToCenterPoint;

		for (int i = 0; i < artificialObjectGravityList.Count; i++) { 
			artificialObjectGravityList [i].setUseInverseDirectionToCenterPointState (useInverseDirectionToCenterPoint);
		}
	}
}
