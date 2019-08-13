using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class gravitySystem : MonoBehaviour
{
	public Transform gravityCenter;
	public GameObject playerCameraGameObject;
	public bool gravityPowerEnabled;
	public bool liftToSearchEnabled;
	public bool randomRotationOnAirEnabled;
	public GameObject cursor;
	public LayerMask layer;
	public float searchSurfaceSpeed = 5;
	public float accelerateSpeed = 20;
	public float airControlSpeed = 5;

	public bool preserveVelocityWhenDisableGravityPower;

	float extraMultiplierPreserveVelocity;

	public float highGravityMultiplier;
	public List<bool> materialToChange = new List<bool> ();
	public bool changeModelColor;
	public Color powerColor;
	public float hoverSpeed;
	public float hoverAmount;
	public float hoverSmooth;
	public GameObject arrow;

	public bool powerActivated;
	public bool recalculatingSurface;
	public bool searchingSurface;
	public bool searchingNewSurfaceBelow;
	public bool searchAround;

	public Vector3 currentNormal = new Vector3 (0, 1, 0);
	public bool dead;
	public bool firstPersonView;
	public Renderer playerRenderer;
	public bool usedByAI;

	public string tagForCircumnavigate = "sphere";
	public string tagForMovingObjects = "moving";

	public float rotateToSurfaceSpeed = 2;
	public float rotateToRegularGravitySpeed = 2;

	public Vector3 regularGravity = new Vector3 (0, 1, 0);

	public bool gravityPowerActive;

	public bool startWithNewGravity;
	public bool usePlayerRotation;
	public bool adjustRotationToSurfaceFound;
	public Vector3 newGravityToStart;

	public bool zeroGravityModeOn;
	public bool startWithZeroGravityMode;
	public bool canResetRotationOnZeroGravityMode;
	public bool canAdjustToForwardSurface;
	public Transform forwardSurfaceRayPosition;
	public float maxDistanceToAdjust;
	public float resetRotationZeroGravitySpeed;
	public float adjustToForwardSurfaceSpeed;

	public bool canActivateFreeFloatingMode;
	public bool freeFloatingModeOn;

	public bool useEventsOnFreeFloatingModeStateChange;
	public UnityEvent evenOnFreeFloatingModeStateEnabled;
	public UnityEvent eventOnFreeFloatingModeStateDisabled;

	public float circumnavigateRotationSpeed = 10;
	public bool circumnavigateCurrentSurfaceActive;
	public bool useLerpRotation;

	public Transform gravityAdherenceRaycastParent;

	public bool checkSurfaceBelowLedge;
	public float surfaceBelowLedgeRaycastDistance = 3;
	public float belowLedgeRotationSpeed = 10;
	public Transform surfaceBelowRaycastTransform;

	public bool checkSurfaceInFront;
	public float surfaceInFrontRaycastDistance = 0.5f;
	public float surfaceInFrontRotationSpeed = 10;
	public Transform surfaceInFrontRaycastTransform;

	public bool checkCircumnavigateSurfaceOnZeroGravity = true;

	public bool searchNewSurfaceOnHighFallSpeed = true;
	public float minSpeedToSearchNewSurface = 15;

	public bool shakeCameraOnHighFallSpeed = true;
	public float minSpeedToShakeCamera = 15;

	public bool checkSurfaceBelowOnRegularState;
	public float timeToSetNullParentOnAir = 0.5f;
	float lastTimeParentFound;

	public Transform pivotCameraTransform;
	public SphereCollider gravityCenterCollider;
	public playerController playerControllerManager;
	public otherPowers powers;
	public CapsuleCollider playerCollider;
	public playerInputManager playerInput;

	public Camera mainCamera;
	public playerWeaponsManager weaponsManager;
	public Rigidbody mainRigidbody;
	public playerCamera playerCameraManager;
	public Transform mainCameraTransform;
	public grabObjects grabObjectsManager;

	zeroGravityRoomSystem currentZeroGravityRoom;

	Transform playerCameraTransform;

	Vector3 currentPlayerPosition;
	Quaternion arrowRotation;
	Color originalPowerColor;
	bool accelerating;
	bool rotating = false;
	Vector3 gravityDirection;
	Vector3 rightAxis;
	float timer = 0.75f;
	float rotateAmount = 40;
	float normalGravityMultiplier;
	Vector3 surfaceNormal;
	Vector3 turnDirection;

	GameObject fatherDetected;
	bool circumnavigableSurfaceFound;

	RaycastHit hit;

	public bool choosingDirection;
	bool onGround;
	bool lifting;
	bool hovering;
	bool turning;

	Coroutine rotateCharacterState;
	Coroutine rotateToSurfaceState;

	Transform externalGravityCenter;

	GameObject currentSurfaceBelowPlayer;
	GameObject previousSurfaceBelowPlayer;

	Vector3 currentPosition;

	bool preservingVelocity;
	Vector3 previousRigidbodyVelocity;

	bool playerInsideGravityRoom;

	Vector3 rayPosition;
	Vector3 rayDirection;

	Vector3 currentRotatingNormal;
	Coroutine colorCoroutine;

	public setGravity currentSetGravityManager;

	bool surfaceFound;
	RaycastHit currentSurfaceFound;

	float currentCircumnagivateRotationSpeed;

	Vector2 axisValues;
	float verticalInput;
	float horizontalInput;
	Vector3 moveInput;
	float gravityAdherenceRaycastParentAngleRotation;

	Coroutine zeroGravityModeRotateCharacterCoroutine;
	Coroutine teleportCoroutine;

	void Start ()
	{
		//get the pivot of the camera
		playerCameraTransform = playerCameraGameObject.transform;
		mainRigidbody.freezeRotation = true; 

		//the gravity center has a sphere collider, that surrounds completly the player to use it when the player searchs a new surface, 
		//detecting the collision with any object to rotate the player to that surface
		//this is done like this to avoid that the player cross a collider when he moves while he searchs a new surface
		//get all the neccessary components in the player

		//get the original value of some parameters
		normalGravityMultiplier = playerControllerManager.gravityMultiplier;
		originalPowerColor = powerColor;

		GameObject newGravityCenter = new GameObject ();
		externalGravityCenter = newGravityCenter.transform;
		externalGravityCenter.name = "External Gravity Center";
		externalGravityCenter.SetParent (transform);

		if (startWithNewGravity) {
			if (usePlayerRotation) {
				Vector3 normalDirection = -transform.up;
				if (adjustRotationToSurfaceFound) {
					if (Physics.Raycast (transform.position, -transform.up, out hit, Mathf.Infinity, layer)) {
						normalDirection = hit.normal;
					}
				}
					
				if (transform.up != currentNormal) {
					setNormal (normalDirection);
				}
			} else {
				setNormal (newGravityToStart);
			}
		}

		if (startWithZeroGravityMode) {
			setZeroGravityModeOnState (true);
		}

		currentRotatingNormal = currentNormal;
	}

	void Update ()
	{
		currentPosition = transform.position;

		//the arrow in the back of the player looks to the direction of the real gravity
		if (arrow) {
			currentPlayerPosition = new Vector3 (currentPosition.x, 0, currentPosition.z);
			if (currentPlayerPosition != Vector3.zero) {
				arrowRotation = Quaternion.LookRotation (currentPlayerPosition);
				if (arrowRotation.eulerAngles != Vector3.zero) {
					arrow.transform.rotation = arrowRotation;
				}
			}
		}

		//elevate the player above the ground when the gravity power is enabled and the player was in the ground before it
		if (lifting) {
			bool surfaceAbove = false;
			//check if there is any obstacle above the player while he is being elevated, to prevent he can cross any collider
			Ray crouchRay = new Ray (currentPosition + transform.up * 1.5f, transform.up);
			if (Physics.SphereCast (crouchRay, 0.4f, out hit, 0.5f, layer)) {
				surfaceAbove = true;
			}
			//if the ray doesn't found any surface, keep lifting the player until the timer reachs its target value
			else {
				timer -= Time.deltaTime;
				transform.Translate (Vector3.up * (Time.deltaTime * 4));
				playerCameraTransform.Translate (Vector3.up * (Time.deltaTime * 4));
			}

			//if the timer ends or a surface is found, stop the lifting and start rotate the player to float in the air
			if (surfaceAbove || timer < 0) {
				lifting = false;
				timer = 0.75f;
				searchingSurface = false;
				searchAround = false;
				rotateMeshPlayer ();
				setHoverState (true);
			}
		}	

		//moving in the air with the power gravity activated looking for a new surface
		if (searchingSurface) {
			//parameters to store the position and the direction of the raycast that checks any close surface to the player
			rayPosition = Vector3.zero;
			rayDirection = Vector3.zero;
			//set the size of the ray
			float rayDistance = 0;
			//if the player has set the direction of the air movement, the raycast starts in camera pivot position
			//else the player is falling and reach certain amount of velocity, so the next surface that he will touch becomes in his new ground
			//and the raycast starts in the player position
			if (!searchingNewSurfaceBelow) {
				rayDistance = 2;
				Vector3 newVelocity = Mathf.Abs (playerControllerManager.getGravityForce ()) * mainRigidbody.mass * gravityDirection * searchSurfaceSpeed;

				//when the player searchs a new surface using the gravity force, the player can moves like when he falls
				if (!powers.running) {
					//get the global input and convert it to the local direction, using the axis in the changegravity script
					Vector3 forwardAxis = Vector3.Cross (gravityDirection, rightAxis);
					Vector3 newMoveInput = playerControllerManager.getVerticalInput () * forwardAxis + playerControllerManager.getHorizontalInput () * rightAxis;
					if (newMoveInput.magnitude > 1) {
						newMoveInput.Normalize ();
					}
					if (newMoveInput.magnitude > 0) {
						newVelocity += newMoveInput * airControlSpeed;
					}
				}

				//apply and extra force if the player increase his movement
				if (accelerating) {
					newVelocity += gravityDirection * accelerateSpeed;
				}

				//make a lerp of the velocity applied to the player to move him smoothly
				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, newVelocity, Time.deltaTime * 2);
				//set the direction of the ray that checks any surface
				rayPosition = pivotCameraTransform.position;
				rayDirection = gravityDirection;
				if (currentNormal == regularGravity) {
					rayDistance = GKC_Utils.distance (rayPosition, currentPosition) + 0.5f;
				}
			} 

			//else, the player is falling in his ground direction, so the ray to check a new surface is below his feet
			else {    
				rayPosition = currentPosition;
				rayDirection = -transform.up;
				rayDistance = 0.6f;
			}

			//launch a raycast to check any surface
			Debug.DrawRay (rayPosition, rayDirection * rayDistance, Color.yellow);
			if (Physics.Raycast (rayPosition, rayDirection, out hit, rayDistance, layer)) {
				//if the object detected has not trigger and rigidbody, then
				if (!hit.collider.isTrigger && !hit.rigidbody) {
					//disable the search of the surface and rotate the player to that surface

					playerControllerManager.setGravityPowerActiveState (false);
					playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

					powerActivated = false;
					searchingNewSurfaceBelow = false;
					searchingSurface = false;
					searchAround = false;
					mainRigidbody.velocity = Vector3.zero;

					//disable the collider in the gravity center and enable the capsule collider in the player
					gravityCenterCollider.enabled = false;
					playerCollider.isTrigger = false;

					//check if the object detected can be circumnavigate
					if (hit.collider.gameObject.tag == tagForCircumnavigate) {
						circumnavigableSurfaceFound = true;
					}

					//check if the object is moving to parent the player inside it
					if (hit.collider.gameObject.tag == tagForMovingObjects) {
						addParent (hit.collider.gameObject);
					}	

					//set the camera in its regular position
					playerCameraManager.changeCameraFov (false);
					//if the new normal is different from the previous normal in the gravity power, then rotate the player
					if (hit.normal != currentNormal) {
						checkRotateToSurface (hit.normal, rotateToSurfaceSpeed); 
					}

					//if the player back to the regular gravity value, change its color to the regular state
					if (hit.normal == regularGravity) {
						changeColor (false);
					}	
				}
			}
		}

		//if the player falls and reachs certain velocity, the camera shakes and the mesh of the player rotates
		//also, if the gravity power is activated, look a new surface to change the gravity to the found surface
		if (!freeFloatingModeOn && !zeroGravityModeOn && !onGround && !searchingNewSurfaceBelow && !choosingDirection && !powerActivated && !playerControllerManager.usingJetpack &&
		    !playerControllerManager.jetPackEquiped && !playerControllerManager.flyModeActive) { 

			if (shakeCameraOnHighFallSpeed && transform.InverseTransformDirection (mainRigidbody.velocity).y < -minSpeedToShakeCamera) {
				playerCameraManager.shakeCamera ();

				if (!weaponsManager.carryingWeaponInThirdPerson) {
					rotateMeshPlayer ();
				}
			}

			if (searchNewSurfaceOnHighFallSpeed && transform.InverseTransformDirection (mainRigidbody.velocity).y < -minSpeedToSearchNewSurface) {
				//if the gravity of the player is different from the regular gravity, start searchin the new surface
				if (currentNormal != regularGravity) {
					searchingNewSurfaceBelow = true;
					searchingSurface = true;
					recalculatingSurface = false;
					circumnavigableSurfaceFound = false;
				}
			}
		}

		//walk in spheres and moving objects, recalculating his new normal and lerping the player to the new rotation
		if (!lifting && !searchingSurface && (circumnavigableSurfaceFound || fatherDetected) && recalculatingSurface && !rotating) {
			float rayDistance = 0.5f;
			if (!onGround) {
				if (!freeFloatingModeOn && !zeroGravityModeOn) {
					rayDistance = 10;
				}
			}

			surfaceFound = false;
			//get the normal direction of the object below the player, to recalculate the rotation of the player
			if (Physics.Raycast (currentPosition + transform.up * 0.1f, -transform.up, out hit, rayDistance, layer)) {
				currentSurfaceFound = hit;
				surfaceFound = true;
				currentCircumnagivateRotationSpeed = circumnavigateRotationSpeed;
			} 

			//Get the correct raycast orientation according to input, for example, it is no the same ray position and direction if the player is walking forward or backward on 
			//first person or if he is aiming
			if (checkSurfaceBelowLedge || checkSurfaceInFront) {
				rayPosition = currentPosition + transform.up * 0.1f;
				if (playerControllerManager.isLookingInCameraDirection ()) {

					axisValues = playerInput.getPlayerRawMovementAxis ();
					verticalInput = axisValues.y;
					horizontalInput = axisValues.x;

					rayPosition += verticalInput * (transform.forward * 0.2f);
					rayPosition += horizontalInput * (transform.right * 0.2f);

					moveInput = (verticalInput * transform.forward + horizontalInput * transform.right);	

					gravityAdherenceRaycastParentAngleRotation = Vector3.SignedAngle (transform.forward, moveInput, transform.up);

					if (verticalInput == 0 && horizontalInput == 0) {
						gravityAdherenceRaycastParentAngleRotation = 0;
					}

				} else {
					rayPosition += transform.forward * 0.2f;
					gravityAdherenceRaycastParentAngleRotation = 0;
				}

				gravityAdherenceRaycastParent.localRotation = Quaternion.Euler (new Vector3 (0, gravityAdherenceRaycastParentAngleRotation, 0));
			}

			if (checkSurfaceBelowLedge) {
				rayDirection = -transform.up;

				Debug.DrawRay (rayPosition, rayDirection, Color.white);
						
				if (!Physics.Raycast (rayPosition, rayDirection, out hit, 1, layer)) {
					Debug.DrawRay (rayPosition, rayDirection, Color.green);
					rayPosition = surfaceBelowRaycastTransform.position;
					rayDirection = surfaceBelowRaycastTransform.forward;
					if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceBelowLedgeRaycastDistance, layer)) {
						Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.yellow);
						currentSurfaceFound = hit;
						surfaceFound = true;
						currentCircumnagivateRotationSpeed = belowLedgeRotationSpeed;
					} else {
						Debug.DrawRay (rayPosition, rayDirection * surfaceBelowLedgeRaycastDistance, Color.red);
					}
				} else {
					Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.red);
				}
			}

			if (checkSurfaceInFront) {
				rayPosition = surfaceInFrontRaycastTransform.position;
				rayDirection = surfaceInFrontRaycastTransform.forward;
				if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceInFrontRaycastDistance, layer)) {
					currentSurfaceFound = hit;
					surfaceFound = true;
					currentCircumnagivateRotationSpeed = surfaceInFrontRotationSpeed;
				}
			}

			if (surfaceFound) {
				if (!currentSurfaceFound.collider.isTrigger && !currentSurfaceFound.rigidbody) {
					//the object detected can be circumnavigate, so get the normal direction
					if (currentSurfaceFound.collider.gameObject.tag == tagForCircumnavigate) {
						surfaceNormal = currentSurfaceFound.normal;
					}

					//the object is moving, so get the normal direction and set the player as a children of the moving obejct
					else if (currentSurfaceFound.collider.gameObject.tag == tagForMovingObjects) {
						surfaceNormal = currentSurfaceFound.normal;
						if (!fatherDetected) {
							addParent (currentSurfaceFound.collider.gameObject);
						}
					} 
				}
			} else {
				if (fatherDetected) {
					removeParent ();
				}
			}

			if ((!zeroGravityModeOn && !freeFloatingModeOn) || onGround) {
				if (useLerpRotation) {
					//recalculate the rotation of the player and the camera according to the normal of the surface under the player
					currentNormal = Vector3.Lerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForward = Vector3.Cross (transform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 
					transform.rotation = Quaternion.Lerp (transform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, currentNormal);
					Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);
					playerCameraTransform.rotation = Quaternion.Lerp (playerCameraTransform.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
				} else {
					currentNormal = Vector3.Slerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForward = Vector3.Cross (transform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 
					transform.rotation = Quaternion.Slerp (transform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, currentNormal);
					Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);
					playerCameraTransform.rotation = Quaternion.Slerp (playerCameraTransform.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
				}

				updateCurrentRotatingNormal (currentNormal);
			}

			//set the normal in the playerController component
			playerControllerManager.setCurrentNormalCharacter (currentNormal);
		}

		if (zeroGravityModeOn && !onGround && !rotating) {
			currentNormal = transform.up;
			playerControllerManager.setCurrentNormalCharacter (currentNormal);

			updateCurrentRotatingNormal (currentNormal);
		}

		//set a cursor in the screen when the character can choose a direction to change his gravity
		if (!usedByAI) {
			if (cursor) {
				if (choosingDirection && cursor) {
					if (!cursor.activeSelf) {
						cursor.SetActive (true);
					}
				}

				if (!choosingDirection && cursor) {
					if (cursor.activeSelf) {
						cursor.SetActive (false);
					}
				}
			}
		}

		//if the player can choosed a direction, lerp his velocity to zero
		if (choosingDirection) {
			mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero, Time.deltaTime * 2);
		}

		if (rotating && !powers.running) {
			mainRigidbody.velocity = Vector3.zero;
		}

		if ((gravityPowerActive || circumnavigateCurrentSurfaceActive || (zeroGravityModeOn && checkCircumnavigateSurfaceOnZeroGravity)) && !powerActivated) {
			currentSurfaceBelowPlayer = playerControllerManager.getCurrentSurfaceBelowPlayer ();
	
			if (currentSurfaceBelowPlayer) {
				//check if the object detected can be circumnavigate
				if (currentSurfaceBelowPlayer.tag == tagForCircumnavigate) {
					circumnavigableSurfaceFound = true;
				} else {
					circumnavigableSurfaceFound = false;
				}

				//check if the object is moving to parent the player inside it
				if (currentSurfaceBelowPlayer.tag == tagForMovingObjects) {
					if (previousSurfaceBelowPlayer != currentSurfaceBelowPlayer) {
						previousSurfaceBelowPlayer = currentSurfaceBelowPlayer;
						addParent (currentSurfaceBelowPlayer);
					}
				} else if (fatherDetected && !currentSurfaceBelowPlayer.transform.IsChildOf (fatherDetected.transform)) {
					removeParent ();
				}

				//if the surface where the player lands can be circumnavigated or an moving/rotating object, then keep recalculating the player throught the normal surface
				if (circumnavigableSurfaceFound || fatherDetected) {
					recalculatingSurface = true;
				} 
				//else disable this state
				else {
					recalculatingSurface = false;
				}
			} else {
				if (previousSurfaceBelowPlayer) {
					previousSurfaceBelowPlayer = null;
				}

				if (rotating) {
					circumnavigableSurfaceFound = false;
				}
			}
		} else {
			if ((circumnavigableSurfaceFound || fatherDetected) && rotating) {
				circumnavigableSurfaceFound = false;
			}

			if (checkSurfaceBelowOnRegularState && !gravityPowerActive && !zeroGravityModeOn && !playerControllerManager.isPlayerSetAsChildOfParent ()) {

				currentSurfaceBelowPlayer = playerControllerManager.getCurrentSurfaceBelowPlayer ();
				if (currentSurfaceBelowPlayer) {
					//check if the object is moving to parent the player inside it
					if (currentSurfaceBelowPlayer.tag == tagForMovingObjects) {
						if (previousSurfaceBelowPlayer != currentSurfaceBelowPlayer) {
							previousSurfaceBelowPlayer = currentSurfaceBelowPlayer;
							addParent (currentSurfaceBelowPlayer);

							lastTimeParentFound = Time.time;
						}
					} else if (fatherDetected && !currentSurfaceBelowPlayer.transform.IsChildOf (fatherDetected.transform)) {
						removeParent ();
					}
						
				} else {
					if (previousSurfaceBelowPlayer) {
						previousSurfaceBelowPlayer = null;
					}

					if (Time.time > timeToSetNullParentOnAir + lastTimeParentFound) {
						removeParent ();
					}
				}
			} else {
				if (currentSurfaceBelowPlayer) {
					currentSurfaceBelowPlayer = null;
				}

				if (previousSurfaceBelowPlayer) {
					previousSurfaceBelowPlayer = null;
				}
			}
		}
	}

	//rotate randomly the mesh of the player in the air, also make that mesh float while chooses a direction in the air
	void FixedUpdate ()
	{
		if (turning) {
			if (randomRotationOnAirEnabled || powerActivated) {
				gravityCenter.transform.Rotate (turnDirection * rotateAmount * Time.deltaTime);
			}

			if (hovering) {
				float posTargetY = Mathf.Sin (Time.time * hoverSpeed) * hoverAmount;
				mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, mainRigidbody.position + posTargetY * transform.up, Time.deltaTime * hoverSmooth);
			}
		}
	}

	//playerController set the values of ground in this script and in the camera code
	public void onGroundOrOnAir (bool state)
	{
		onGround = state;
		if (onGround) {
			//the player is on the ground
			//set the states in the camera, on ground, stop any shake of the camera, and back the camera to its regular position if it has been moved
			playerCameraManager.onGroundOrOnAir (true);
			playerCameraManager.stopShakeCamera ();
			playerCameraManager.changeCameraFov (false);

			//stop rotate the player
			turning = false;

			//if the surface where the player lands can be circumnavigated or an moving/rotating object, then keep recalculating the player throught the normal surface
			if ((circumnavigableSurfaceFound || fatherDetected) && (gravityPowerActive || circumnavigateCurrentSurfaceActive || (zeroGravityModeOn && checkCircumnavigateSurfaceOnZeroGravity))) {
				recalculatingSurface = true;
			} 

			//else disable this state
			else {
				recalculatingSurface = false;
			}

			//set the gravity force applied to the player to its regular state
			playerControllerManager.gravityMultiplier = normalGravityMultiplier;
			//set the model rotation to the regular state
			checkRotateCharacter (Vector3.zero);
			accelerating = false;

			if (currentNormal != regularGravity) {
				gravityPowerActive = true;
			}
		} else {
			//the player is on the air
			playerCameraManager.onGroundOrOnAir (false);
		}
	}

	//when the player searchs a new surface using the gravity power on button, check the collisions in the gravity center sphere collider, to change the
	//gravity of the player to the detected normal direction
	void OnCollisionEnter (Collision col)
	{
		//check that the player is searchin a surface, the player is not running, and that he is searching around
		if (searchingSurface && !powers.running && turning && searchAround) {
			//check that the detected object is not a trigger or the player himself
			if (col.gameObject.tag != "Player" && !col.rigidbody && !col.collider.isTrigger) {
				//get the collision contant point to change the direction of the ray that searchs a new direction, setting the direction from the player 
				//to the collision point as the new direction
				Vector3 hitDirection = col.contacts [0].point - pivotCameraTransform.position;
				hitDirection = hitDirection / hitDirection.magnitude;
				gravityDirection = hitDirection;
				searchAround = false;
			}
		}
	}

	//now the gravity power is in a function, so it can be called from keyboard and a touch button
	public void activateGravityPower ()
	{
		if (weaponsManager.isWeaponsModeActive ()) {
			return;
		}

		gravityPowerActive = true;
		//if the option to lift the player when he uses the gravity power is disable, then searchs an new surface in the camera direction 
		if (!liftToSearchEnabled) {
			changeOnTrigger (mainCameraTransform.TransformDirection (Vector3.forward), mainCameraTransform.TransformDirection (Vector3.right));
		} 

		//else lift the player, and once that he has been lifted, then press again the gravit power on button to search an new surface
		//or disable the gravity power
		else {
			//enable the sphere collider in the gravity center
			gravityCenterCollider.enabled = true;

			//disable the capsule collider in the player
			playerCollider.isTrigger = true;

			//get the last time that the player was in the air
			playerControllerManager.lastTimeFalling = Time.time;
			recalculatingSurface = false;
			accelerating = false;

			//change the color of the player's textures
			changeColor (true);
			searchingNewSurfaceBelow = false;
			removeParent ();
			circumnavigableSurfaceFound = false;	
			playerControllerManager.setGravityPowerActiveState (true);
			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (false);

			powerActivated = true;

			//calibrate the accelerometer to rotate the camera in this mode
			playerCameraManager.calibrateAccelerometer ();

			//drop any object that the player is holding and disable aim mode
			checkKeepPower ();

			checkKeepWeapons ();

			grabObjectsManager.checkIfDropObjectIfNotPhysical (false);

			//the player is in the ground, so he is elevated above it
			if (onGround) {
				lifting = true;
				choosingDirection = true;
			}

			//the player set the direction of the movement in the air to search a new surface
			if (!lifting && choosingDirection) {
				playerCameraManager.shakeCamera ();
				setHoverState (false);
				rotateMeshPlayer ();
				searchingSurface = true;
				circumnavigableSurfaceFound = false;	
				removeParent ();
				choosingDirection = false;
				searchAround = true;
				gravityDirection = mainCameraTransform.forward;
				//get direction and right axis of the camera, so when the player searchs a new surface, this is used to get the local movement, 
				//which allows to move the player in his local right, left, forward and back while he also displaces in the air
				rightAxis = mainCameraTransform.right;
				checkRotateCharacter (-gravityDirection);
				return;
			} 

			//the player is in the air, so he is stopped in it to choose a direction
			if (!onGround && !choosingDirection && !lifting) {
				playerCameraManager.stopShakeCamera (); 
				playerCameraManager.changeCameraFov (false);
				setHoverState (true); 
				rotateMeshPlayer ();
				choosingDirection = true; 	
				searchingSurface = false;	
				searchAround = false;
			}
		}
	}

	//now the gravity power is in a function, so it can be called from keyboard and a touch button
	public void deactivateGravityPower ()
	{
		//check that the power gravity is already enabled
		if ((choosingDirection || searchingSurface || currentNormal != regularGravity) && !playerControllerManager.usingDevice) {
			print ("deactivate gravity power");

			//disable the sphere collider in the gravity center
			gravityCenterCollider.enabled = false;

			//enable the capsule collider in the player
			playerCollider.isTrigger = false;

			//get the last time that the player was in the air
			playerControllerManager.lastTimeFalling = Time.time;

			accelerating = false;

			//set the force of the gravity in the player to its regular state
			playerControllerManager.gravityMultiplier = normalGravityMultiplier;

			//change the color of the player
			changeColor (false);
			choosingDirection = false;
			circumnavigableSurfaceFound = false;	
			removeParent ();
			setHoverState (false);
			turning = false;
			searchingSurface = false;
			searchAround = false;
			lifting = false;
			recalculatingSurface = false;
			timer = 0.75f;

			//stop to shake the camera and set its position to the regular state
			playerCameraManager.stopShakeCamera ();
			playerCameraManager.changeCameraFov (false);

			//if the normal of the player is different from the regular gravity, rotate the player
			if (currentNormal != regularGravity) {

				if (preserveVelocityWhenDisableGravityPower) {
					previousRigidbodyVelocity = mainRigidbody.velocity;
					preservingVelocity = true;

					extraMultiplierPreserveVelocity = 1;
				}

				checkRotateToSurface (regularGravity, rotateToRegularGravitySpeed);
			}

			//rotate the mesh of the player also
			checkRotateCharacter (regularGravity);

			playerControllerManager.setGravityPowerActiveState (false);
			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

			powerActivated = false;

			//set the value of the normal in the playerController component to its regular state
			playerControllerManager.setCurrentNormalCharacter (regularGravity);
			gravityPowerActive = false;

			currentSetGravityManager = null;
		}
	}

	public void checkKeepWeapons ()
	{
		if (weaponsManager.aimingInThirdPerson || weaponsManager.carryingWeaponInThirdPerson) {
			weaponsManager.checkIfKeepSingleOrDualWeapon ();
		}
	}

	public void checkKeepPower ()
	{
		if (powers.isAimingPowerInThirdPerson ()) {
			powers.deactivateAimMode ();
		}
	}

	//now the change of velocity is in a function, so it can be called from keyboard and a touch button
	public void changeMovementVelocity (bool value)
	{
		//if the player is not choosing a gravity direction and he is searching a surface or the player is not in the ground and with a changed normal, then
		if (!choosingDirection && (powerActivated || (!playerControllerManager.isPlayerOnGround () && currentNormal != regularGravity))) {
			accelerating = value;
			//move the camera to a further away position, and add extra force to the player's velocity
			if (accelerating) {
				playerCameraManager.changeCameraFov (true);
				playerControllerManager.gravityMultiplier = highGravityMultiplier;
				//when the player accelerates his movement in the air, the camera shakes
				//if the player accelerates his movement in the air and shake camera is enabled
				if (playerCameraManager.settings.enableShakeCamera) {
					playerCameraManager.accelerateShake (true);			
				}
			} 

			//else, set the camera to its regular position, reset the force applied to the player's velocity
			else {
				playerCameraManager.changeCameraFov (false);
				playerControllerManager.gravityMultiplier = normalGravityMultiplier;
				playerCameraManager.accelerateShake (false);
			}
		}
	}

	//convert the character in a child of the moving object
	public void addParent (GameObject obj)
	{
		fatherDetected = obj;

		parentAssignedSystem currentParentAssignedSystem = obj.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem) {
			fatherDetected = currentParentAssignedSystem.getAssignedParent ();
		}

		if (rotating) {
			externalGravityCenter.SetParent (fatherDetected.transform);
		} else {
			setFatherDetectedAsParent ();
		}
	}

	//remove the parent of the player, so he moves freely again
	public void removeParent ()
	{
		if (rotating) {
			setExternalGravityCenterAsParent ();
		} else {
			setNullParent ();
		}

		fatherDetected = null;
	}

	public void setExternalGravityCenterAsParent ()
	{
		transform.SetParent (externalGravityCenter);
		playerCameraTransform.SetParent (externalGravityCenter);
	}

	public void setFatherDetectedAsParent ()
	{
		transform.SetParent (fatherDetected.transform);
		playerCameraTransform.SetParent (fatherDetected.transform);
	}

	public void setNullParent ()
	{
		transform.SetParent (null);
		playerCameraTransform.SetParent (null);
	}

	public void setCorrectParent ()
	{
		if (fatherDetected) {
			setFatherDetectedAsParent ();
		} else {
			setNullParent ();
		}
	}

	//the funcion to change camera view, to be called from a key or a touch button
	public void changeCameraView ()
	{
		firstPersonView = !firstPersonView;

		//disable or enable the mesh of the player
		setGravityArrowState (!firstPersonView);

		//change to first person view
		if (firstPersonView) {
			playerCameraManager.activateFirstPersonCamera ();
			grabObjectsManager.setAimingState (true);
			powers.usingPowers = true;
		}

		//change to third person view
		else {
			playerCameraManager.deactivateFirstPersonCamera ();

			grabObjectsManager.checkIfDropObjectIfNotPhysical (true);

			powers.usingPowers = false;
			powers.keepPower ();
		}

		weaponsManager.setCurrentWeaponsParent (firstPersonView);

		playerCameraManager.setFirstOrThirdHeadBobView (firstPersonView);

		playerControllerManager.setLastTimeMoved ();
	}

	//set a random direction to rotate the character
	void rotateMeshPlayer ()
	{
		if (!turning) {
			turning = true;
			turnDirection = new Vector3 (Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1));
			if (turnDirection.magnitude == 0) {
				turnDirection.x = 1;
			}
		}
	}

	//set if the player is hovering or not
	void setHoverState (bool state)
	{
		hovering = state;
	}

	//change the gravity of the player when he touchs the arrow trigger
	public void changeOnTrigger (Vector3 dir, Vector3 right)
	{
		//set the parameters needed to change the player's gravity without using the gravity power buttons
		searchingNewSurfaceBelow = false;
		removeParent ();
		circumnavigableSurfaceFound = false;	
		searchingSurface = true;
		searchAround = true;
		changeColor (true);
		playerControllerManager.gravityMultiplier = normalGravityMultiplier;

		playerControllerManager.setGravityPowerActiveState (true);
		playerCameraManager.sethorizontalCameraLimitActiveOnAirState (false);

		powerActivated = true;
		playerCameraManager.calibrateAccelerometer ();
		rotateMeshPlayer ();
		playerCameraManager.shakeCamera ();
		gravityDirection = dir;
		rightAxis = right;
		checkRotateCharacter (-gravityDirection);
		gravityPowerActive = true;
	}

	//stop the gravity power when the player is going to drive a vehicle
	public void stopGravityPower ()
	{
		//disable the sphere collider in the gravity center
		gravityCenterCollider.enabled = false;
		//get the last time that the player was in the air
		playerControllerManager.lastTimeFalling = Time.time;
		accelerating = false;

		//set the force of the gravity in the player to its regular state
		playerControllerManager.gravityMultiplier = normalGravityMultiplier;
		choosingDirection = false;
		circumnavigableSurfaceFound = false;	
		removeParent ();
		setHoverState (false);
		turning = false;
		searchingSurface = false;
		searchAround = false;
		lifting = false;
		recalculatingSurface = false;
		timer = 0.75f;

		//stop to shake the camera and set its position to the regular state
		playerCameraManager.stopShakeCamera ();
		playerCameraManager.changeCameraFov (false);

		playerControllerManager.setGravityPowerActiveState (false);
		playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

		powerActivated = false;

		//reset the player's rotation
		transform.rotation = Quaternion.identity;
		gravityCenter.transform.localRotation = Quaternion.identity;

		//set to 0 the current velocity of the player
		mainRigidbody.velocity = Vector3.zero;
		gravityPowerActive = false;
	}

	//rotate the player, camera and mesh of the player to the new surface orientation
	//public
	public void checkRotateToSurface (Vector3 normal, float rotSpeed)
	{
		stopRotateToSurfaceCoroutine ();
		rotateCharacterState = StartCoroutine (rotateToSurface (normal, rotSpeed));
	}

	public void stopRotateToSurfaceCoroutine ()
	{
		//get the coroutine, stop it and play it again
		if (rotateCharacterState != null) {
			StopCoroutine (rotateCharacterState);
		}
	}

	public IEnumerator rotateToSurface (Vector3 normal, float rotSpeed)
	{
		updateCurrentRotatingNormal (normal);

		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = gravityCenter.position;
		externalGravityCenter.rotation = transform.rotation;

		setExternalGravityCenterAsParent ();

		rotating = true;

		Quaternion currentPlayerRotation = externalGravityCenter.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (externalGravityCenter.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = Quaternion.identity;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Slerp (currentPlayerRotation, playerTargetRotation, t);
			gravityCenter.transform.localRotation = Quaternion.Slerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);
			yield return null;
		}

		setCorrectParent ();

		externalGravityCenter.SetParent (transform);

		currentNormal = normal; 
		playerControllerManager.gravityMultiplier = normalGravityMultiplier;
		playerControllerManager.setCurrentNormalCharacter (normal);
		if (currentNormal == regularGravity) {
			gravityPowerActive = false;
		}
		rotating = false;
	
		//adjust gravity rotation for locked camera
		playerCameraManager.setLockedMainCameraTransformRotation (normal);

		if (zeroGravityModeOn || freeFloatingModeOn) {
			playerCameraManager.stopShakeCamera ();
		}

		if (preservingVelocity) {
			preservingVelocity = false;
			yield return new WaitForSeconds (0.02f);
			playerControllerManager.addExternalForce (previousRigidbodyVelocity * extraMultiplierPreserveVelocity);
		}
	}

	public bool isPlayerSearchingGravitySurface ()
	{
		return powerActivated;
	}

	public bool isCharacterRotatingToSurface ()
	{
		return rotating;
	}

	public void checkRotateToSurfaceWithoutParent (Vector3 normal, float rotSpeed)
	{
		stopRotateToSurfaceWithOutParentCoroutine ();

		rotateCharacterState = StartCoroutine (rotateToSurfaceWithOutParent (normal, rotSpeed));
	}

	public void stopRotateToSurfaceWithOutParentCoroutine ()
	{
		//get the coroutine, stop it and play it again
		if (rotateCharacterState != null) {
			StopCoroutine (rotateCharacterState);
		}
	}

	public IEnumerator rotateToSurfaceWithOutParent (Vector3 normal, float rotSpeed)
	{
		updateCurrentRotatingNormal (normal);

		rotating = true;

		Quaternion currentPlayerRotation = transform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (transform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentCameraRotation = playerCameraGameObject.transform.rotation;
		Vector3 currentCameraForward = Vector3.Cross (playerCameraGameObject.transform.right, normal);
		Quaternion cameraTargetRotation = Quaternion.LookRotation (currentCameraForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = Quaternion.identity;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			transform.rotation = Quaternion.Slerp (currentPlayerRotation, playerTargetRotation, t);
			playerCameraGameObject.transform.rotation = Quaternion.Slerp (currentCameraRotation, cameraTargetRotation, t);
			gravityCenter.transform.localRotation = Quaternion.Slerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);
			yield return null;
		}

		currentNormal = normal; 
		playerControllerManager.gravityMultiplier = normalGravityMultiplier;
		playerControllerManager.setCurrentNormalCharacter (normal);
		if (currentNormal == regularGravity) {
			gravityPowerActive = false;
		}
		rotating = false;

		//adjust gravity rotation for locked camera
		playerCameraManager.setLockedMainCameraTransformRotation (normal);

		if (zeroGravityModeOn || freeFloatingModeOn) {
			playerCameraManager.stopShakeCamera ();
		}

		if (preservingVelocity) {
			preservingVelocity = false;
			mainRigidbody.velocity = previousRigidbodyVelocity * 2;
		}
	}

	public void setNormal (Vector3 normal)
	{
		Vector3 myForwardPlayer = Vector3.Cross (transform.right, normal);
		Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion dstRotPlayer = Quaternion.LookRotation (myForwardPlayer, normal);		
		Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, normal);
		transform.rotation = dstRotPlayer;
		playerCameraTransform.rotation = dstRotCamera;
		currentNormal = normal;
		playerControllerManager.setCurrentNormalCharacter (normal);

		if (currentNormal != regularGravity) {
			gravityPowerActive = true;
			changeColor (true);
		} else {
			changeColor (false);
		}

		updateCurrentRotatingNormal (currentNormal);

		currentSetGravityManager = null;
	}

	public Vector3 getCurrentNormal ()
	{
		return currentNormal;
	}

	public bool isUsingRegularGravity ()
	{
		return currentNormal == regularGravity;
	}

	public Vector3 getRegularGravity ()
	{
		return regularGravity;
	}

	public void updateCurrentRotatingNormal (Vector3 newNormal)
	{
		currentRotatingNormal = newNormal;
	}

	public Vector3 getCurrentRotatingNormal ()
	{
		return currentRotatingNormal;
	}

	//rotate the mesh of the character in the direction of the camera when he selects a gravity direction in the air
	// and to the quaternion identity when he is on ground
	public void checkRotateCharacter (Vector3 normal)
	{
		float angle = Vector3.Angle (gravityCenter.up, transform.up);
		if (Mathf.Abs (angle) > 0 || currentNormal == regularGravity) {
			//get the coroutine, stop it and play it again
			if (rotateToSurfaceState != null) {
				StopCoroutine (rotateToSurfaceState);
			}
			rotateToSurfaceState = StartCoroutine (rotateCharacter (normal));
		}
	}

	public IEnumerator rotateCharacter (Vector3 normal)
	{
		Quaternion orgRotCenter = gravityCenter.transform.localRotation;
		Quaternion dstRotCenter = new Quaternion (0, 0, 0, 1);

		//check that the normal is different from zero, to rotate the player's mesh in the direction of the new gravity when he use the gravity power button
		//and select the camera direction to search a new surface
		//else, the player's mesh is rotated to its regular state
		if (normal != Vector3.zero) {
			orgRotCenter = gravityCenter.transform.rotation;
			Vector3 myForward = Vector3.Cross (gravityCenter.transform.right, normal);
			dstRotCenter = Quaternion.LookRotation (myForward, normal);
		}

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			if (normal == Vector3.zero) {
				gravityCenter.transform.localRotation = Quaternion.Slerp (orgRotCenter, dstRotCenter, t);
			} else {
				gravityCenter.transform.rotation = Quaternion.Slerp (orgRotCenter, dstRotCenter, t);
			}
			yield return null;
		}
	}

	public void changeGravityDirectionDirectlyInvertedValue (Vector3 gravityDirection, bool preserveVelocity)
	{
		changeGravityDirectionDirectly (-gravityDirection, preserveVelocity);
	}

	public void changeGravityDirectionDirectly (Vector3 gravityDirection, bool preserveVelocity)
	{
		if (!rotating) {
			//disable the search of the surface and rotate the player to that surface
			playerControllerManager.setGravityPowerActiveState (false);
			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

			powerActivated = false;
			searchingNewSurfaceBelow = false;
			searchingSurface = false;
			searchAround = false;

			if (preserveVelocity) {
				previousRigidbodyVelocity = mainRigidbody.velocity;
				preservingVelocity = true;

				extraMultiplierPreserveVelocity = 2;
			}

			mainRigidbody.velocity = Vector3.zero;

			//disable the collider in the gravity center and enable the capsule collider in the player
			gravityCenterCollider.enabled = false;
			playerCollider.isTrigger = false;
	
			//set the camera in its regular position
			playerCameraManager.changeCameraFov (false);

			//if the new normal is different from the previous normal in the gravity power, then rotate the player
			if (gravityDirection != currentNormal) {
				checkRotateToSurface (gravityDirection, rotateToSurfaceSpeed); 
			}

			//if the player back to the regular gravity value, change its color to the regular state
			if (gravityDirection == regularGravity) {
				changeColor (false);
			} else {
				changeColor (true);
			}
		}
	}

	public void setCurrentSetGravityManager (setGravity currentSetGravity)
	{
		currentSetGravityManager = currentSetGravity;
	}

	public setGravity getCurrentSetGravityManager ()
	{
		return currentSetGravityManager;
	}

	public void changeColor (bool state)
	{
		if (!changeModelColor) {
			return;
		}

		if (colorCoroutine != null) {
			StopCoroutine (colorCoroutine);
		}
		colorCoroutine = StartCoroutine (changeColorCoroutine (state));
	}

	//change the mesh color of the character according to the gravity power
	public IEnumerator changeColorCoroutine (bool value)
	{
		if (playerRenderer != null) {
			if (value) {
				powerColor = originalPowerColor;
			} else {
				powerColor = Color.white;
			}
			for (float t = 0; t < 1;) {
				t += Time.deltaTime;
				for (int i = 0; i < materialToChange.Count; i++) {
					if (playerRenderer.materials.Length >= materialToChange.Count) {
						if (materialToChange [i] == true && playerRenderer.materials [i].HasProperty ("_Color")) {
							playerRenderer.materials [i].color = Color.Lerp (playerRenderer.materials [i].color, powerColor, t);
						}
					}
				}
				yield return null;
			}
		}
	}

	public void setMeshCharacter (SkinnedMeshRenderer currentMeshCharacter)
	{
		playerRenderer = currentMeshCharacter.GetComponent<Renderer> ();

		updateComponent ();
	}
		
	//change the object which the camera follows and disable or enabled the powers according to the player state
	public void death (bool state)
	{
		dead = state;
		if (state) {
			deactivateGravityPower ();
			turning = false;
			hovering = false;
			checkRotateCharacter (Vector3.zero);
		} 
	}

	public void setGravityArrow (GameObject obj)
	{
		arrow = obj;
	}

	public void setGravityArrowState (bool state)
	{
		if (arrow) {
			arrow.SetActive (state);
		}
	}

	public void setFirstPersonView (bool state)
	{
		firstPersonView = state;
	}

	public Transform getGravityCenter ()
	{
		return gravityCenter;
	}

	public bool isCurcumnavigating ()
	{
		return circumnavigableSurfaceFound;
	}

	public bool isSearchingSurface ()
	{
		return searchingSurface;
	}

	public bool isGravityPowerActive ()
	{
		return gravityPowerActive;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		usedByAI = !state;
	}

	public void setZeroGravityModeOnState (bool state)
	{
		if (freeFloatingModeOn) {
			setfreeFloatingModeOnState (false);
		}

		zeroGravityModeOn = state;
		playerControllerManager.setZeroGravityModeOnState (zeroGravityModeOn);
		playerCameraManager.setZeroGravityModeOnState (zeroGravityModeOn);

		if (!zeroGravityModeOn) {
			if (currentNormal != regularGravity) {
				deactivateGravityPower ();
			}
		}
	}

	public void setZeroGravityModeOnStateWithOutRotation (bool state)
	{
		if (freeFloatingModeOn) {
			setfreeFloatingModeOnState (false);
		}

		zeroGravityModeOn = state;
		playerControllerManager.setZeroGravityModeOnState (zeroGravityModeOn);
		playerCameraManager.setZeroGravityModeOnState (zeroGravityModeOn);
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		if (zeroGravityModeOn) {
			return;
		}

		if (!canActivateFreeFloatingMode) {
			return;
		}

		freeFloatingModeOn = state;

		checkEventsOnFreeFloatingModeStateChange (freeFloatingModeOn);

		playerControllerManager.setfreeFloatingModeOnState (freeFloatingModeOn);
		playerCameraManager.setfreeFloatingModeOnState (freeFloatingModeOn);

		stopSetfreeFloatingModeOnStateWithDelayCoroutine ();
	}

	public void checkEventsOnFreeFloatingModeStateChange (bool state)
	{
		if (useEventsOnFreeFloatingModeStateChange) {
			if (state) {
				evenOnFreeFloatingModeStateEnabled.Invoke ();
			} else {
				eventOnFreeFloatingModeStateDisabled.Invoke ();
			}
		}
	}

	public void changeFreeFloatingModeOnState ()
	{
		setfreeFloatingModeOnState (!freeFloatingModeOn);
	}

	Coroutine freeFloatingModeCoroutine;

	public void setfreeFloatingModeOnStateWithDelay (float delayAmount, bool state)
	{
		stopSetfreeFloatingModeOnStateWithDelayCoroutine ();
		freeFloatingModeCoroutine = StartCoroutine (setfreeFloatingModeOnStateWithDelayCoroutine (delayAmount, state));
	}

	public void stopSetfreeFloatingModeOnStateWithDelayCoroutine ()
	{
		if (freeFloatingModeCoroutine != null) {
			StopCoroutine (freeFloatingModeCoroutine);
		}
	}

	public IEnumerator setfreeFloatingModeOnStateWithDelayCoroutine (float delayAmount, bool state)
	{
		yield return new WaitForSeconds (delayAmount);

		setfreeFloatingModeOnState (state);
	}

	public void setCanActivateFreeFloatingModeState (bool state)
	{
		canActivateFreeFloatingMode = state;
	}

	public void rotateCharacterInZeroGravityMode (Vector3 normal, float rotSpeed)
	{
		//get the coroutine, stop it and play it again
		if (zeroGravityModeRotateCharacterCoroutine != null) {
			StopCoroutine (zeroGravityModeRotateCharacterCoroutine);
		}
		zeroGravityModeRotateCharacterCoroutine = StartCoroutine (rotateCharacterInZeroGravityModeCoroutine (normal, rotSpeed));
	}

	public IEnumerator rotateCharacterInZeroGravityModeCoroutine (Vector3 normal, float rotSpeed)
	{
		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = transform.position + transform.up;
		externalGravityCenter.rotation = playerCameraTransform.rotation;

		setExternalGravityCenterAsParent ();

		rotating = true;

		Quaternion currentPlayerRotation = playerCameraTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = Quaternion.identity;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Lerp (currentPlayerRotation, playerTargetRotation, t);
			gravityCenter.transform.localRotation = Quaternion.Lerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);

			yield return null;
		}
			
		setCorrectParent ();

		externalGravityCenter.SetParent (transform);

		rotating = false;
	}

	public void teleportPlayer (Vector3 teleportPosition, float teleportSpeed, Vector3 normal, float rotSpeed)
	{
		stopTeleporting ();
		teleportCoroutine = StartCoroutine (teleportPlayerCoroutine (teleportPosition, teleportSpeed, normal, rotSpeed));
	}

	public void stopTeleporting ()
	{
		if (teleportCoroutine != null) {
			StopCoroutine (teleportCoroutine);
		}
		setPlayerControlState (true);
	}

	public void setPlayerControlState (bool state)
	{
		playerControllerManager.changeScriptState (state);
		playerControllerManager.setGravityForcePuase (!state);
		playerControllerManager.setRigidbodyVelocityToZero ();
		playerControllerManager.setPhysicMaterialAssigmentPausedState (!state);

		if (!state) {
			playerControllerManager.setZeroFrictionMaterial ();
			if (!playerControllerManager.isPauseCheckOnGroundStateZGActive ()) {
				playerControllerManager.setCheckOnGrundStatePausedFFOrZGState (false);
			}
		}
	}

	IEnumerator teleportPlayerCoroutine (Vector3 targetPosition, float currentTeleportSpeed, Vector3 normal, float rotSpeed)
	{
		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = transform.position + transform.up;
		externalGravityCenter.rotation = playerCameraTransform.rotation;

		setExternalGravityCenterAsParent ();

		setPlayerControlState (false);

		rotating = true;

		Quaternion currentPlayerRotation = playerCameraTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);


		float dist = GKC_Utils.distance (externalGravityCenter.position, targetPosition);
		float duration = dist / currentTeleportSpeed;
		float translateTimer = 0;
		float rotateTimer = 0;

		float teleportTimer = 0;

		float normalAngle = 0;

		Vector3 targetPositionDirection = targetPosition - externalGravityCenter.position;
		targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;

		bool targetReached = false;
		while (!targetReached) {
			translateTimer += Time.deltaTime / duration;
			externalGravityCenter.position = Vector3.Lerp (externalGravityCenter.position, targetPosition, translateTimer);

			rotateTimer += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Lerp (currentPlayerRotation, playerTargetRotation, rotateTimer);

			teleportTimer += Time.deltaTime;

			normalAngle = Vector3.Angle (transform.up, normal);   

			//print (normalAngle);
			if ((GKC_Utils.distance (externalGravityCenter.position, targetPosition) < 0.2f && normalAngle < 1) || teleportTimer > (duration + 5)) {
				targetReached = true;
			}
			yield return null;
		}

		setPlayerControlState (true);

		rotating = false;
			
		setCorrectParent ();

		externalGravityCenter.SetParent (transform);
	}

	public void setGravityPowerEnabledState (bool state)
	{
		gravityPowerEnabled = state;
	}

	//CALL INPUT FUNCTIONS
	public void inputSetGravityPowerState (bool enablePower)
	{
		//activate the power of change gravity
		//one press=the player elevates above the surface if he was in the ground or stops him in the air if he was not in the ground
		//two press=make the player moves in straight direction of the camera, looking a new surface
		//three press=stops the player again in the air
		if (!dead && !playerControllerManager.usingDevice && gravityPowerEnabled) {
			if (enablePower) {
				activateGravityPower ();
			} else {
				deactivateGravityPower ();
			}
		}
	}

	public void inputChangeGravitySpeed (bool increaseSpeed)
	{
		if (!dead && !playerControllerManager.usingDevice && gravityPowerEnabled) {
			if (increaseSpeed) {
				changeMovementVelocity (true);
			} else {
				changeMovementVelocity (false);
			}
		}
	}

	public void inputAdjustToSurfaceOnZeroGravity ()
	{
		if (!dead && !playerControllerManager.usingDevice && zeroGravityModeOn && !onGround && !rotating && canAdjustToForwardSurface) {
			if (Physics.Raycast (forwardSurfaceRayPosition.position, forwardSurfaceRayPosition.forward, out hit, maxDistanceToAdjust, layer)) {
				if (currentNormal != regularGravity) {
					teleportPlayer (hit.point + hit.normal * 0.6f, adjustToForwardSurfaceSpeed, hit.normal, resetRotationZeroGravitySpeed);
				}
			}
		}
	}

	public void inputResetRotationOnZeroGravity ()
	{
		if (!dead && !playerControllerManager.usingDevice && zeroGravityModeOn && !onGround && !rotating && canResetRotationOnZeroGravityMode) {
			if (currentNormal != regularGravity) {
				rotateCharacterInZeroGravityMode (regularGravity, resetRotationZeroGravitySpeed);
			}
		}
	}

	public void setCurrentZeroGravityRoom (zeroGravityRoomSystem gravityRoom)
	{
		currentZeroGravityRoom = gravityRoom;
		if (currentZeroGravityRoom) {
			playerInsideGravityRoom = true;
		} else {
			playerInsideGravityRoom = false;
		}
	}

	public bool isPlayerInsiderGravityRoom ()
	{
		return playerInsideGravityRoom;
	}

	public zeroGravityRoomSystem getCurrentZeroGravityRoom ()
	{
		return currentZeroGravityRoom;
	}

	public void setCircumnavigateSurfaceState (bool state)
	{
		circumnavigateCurrentSurfaceActive = state;

		if (!circumnavigateCurrentSurfaceActive) {
			if (circumnavigableSurfaceFound || fatherDetected) {
				circumnavigableSurfaceFound = false;
				if (fatherDetected) {
					removeParent ();	
				}
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
}