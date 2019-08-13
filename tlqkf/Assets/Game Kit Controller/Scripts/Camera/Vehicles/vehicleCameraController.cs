using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class vehicleCameraController : MonoBehaviour
{
	public float rotationSpeed = 10;
	public float clipCastRadius = 0.16f;
	public float backClipSpeed;
	public float maximumBoostDistance;
	public float cameraBoostSpeed;
	public float smoothBetweenState;
	public string defaultStateName = "Third Person";
	public string currentStateName;
	public List<vehicleCameraStateInfo> vehicleCameraStates = new List<vehicleCameraStateInfo> ();
	public shakeSettingsInfo shakeSettings;

	public float gizmoRadius;
	public bool showGizmo;
	public Color labelGizmoColor;

	public GameObject vehicle;
	public LayerMask layer;
	public bool cameraChangeEnabled;
	public float rotationDamping = 3;
	public bool cameraPaused;

	public bool zoomEnabled;

	public bool isFirstPerson;
	public bool usingZoomOn;
	public vehicleCameraStateInfo currentState;
	public bool smoothTransitionsInNewCameraFov;

	public bool useSmoothCameraRotation;
	public bool useSmoothCameraRotationThirdPerson;
	public float smoothCameraRotationSpeedVerticalThirdPerson = 10;
	public float smoothCameraRotationSpeedHorizontalThirdPerson = 10;
	public bool useSmoothCameraRotationFirstPerson;
	public float smoothCameraRotationSpeedVerticalFirstPerson = 10;
	public float smoothCameraRotationSpeedHorizontalFirstPerson = 10;
	float currentCameraUpRotation;

	float currentSmoothCameraRotationSpeedVertical;
	float currentSmoothCameraRotationSpeedHorizontal;
	Quaternion currentPivotRotation;

	public bool drivingVehicle;

	public bool showCameraDirectionGizmo;
	public float gizmoArrowLenght = 1;
	public float gizmoArrowLineLenght = 2.5f;
	public float gizmoArrowAngle = 20;

	public IKDrivingSystem IKDrivingManager;
	public vehicleWeaponSystem weaponManager;
	public vehicleHUDManager hudManager;
	public inputActionManager actionManager;
	public vehicleGravityControl gravityControl;
	public Rigidbody mainRigidbody;
	public vehicleCameraShake shakingManager;

	float currentCameraDistance;
	float originalCameraDistance;
	float currentOriginalDistValue;
	float cameraSpeed;
	float originalRotationSpeed;
	float originalCameraFov;
	Vector2 mouseAxis;
	Vector2 lookAngle;
	Vector3 currentPivotPosition;
	Vector3 nextPivotPositon;
	bool boosting;
	bool releaseCamera;
	bool pickCamera;
	bool followVehiclePosition = true;
	bool firstCameraEnabled;
	Ray ray;
	RaycastHit[] hits;

	Coroutine moveCamera;

	int cameraStateIndex;

	GameObject player;
	playerController playerManager;
	playerCamera playerCameraManager;
	Camera mainCamera;
	Transform mainCameraTransform;

	Vector2 axisValues;

	float fixedRotationAmount;
	float vehicleSpeed;
	float forwadDirection;
	float upRotationAmount;

	public bool showShakeSettings;

	bool playerCameraIsLocked;

	bool isGamePaused;

	void Start ()
	{
		for (int i = 0; i < vehicleCameraStates.Count; i++) {
			vehicleCameraStates [i].originalDist = vehicleCameraStates [i].cameraTransform.localPosition.magnitude;
			vehicleCameraStates [i].originalPivotPosition = vehicleCameraStates [i].pivotTransform.localPosition;
		}

		//get the main components of the camera, like the pivot and the transform which contains the main camera when the player is driving this vehicle
		setCameraState (currentStateName);
		//get the current local position of the camera
		originalCameraDistance = currentState.cameraTransform.localPosition.magnitude;

		//if the vehicle has a weapon system, store it
		if (weaponManager) {
			//set the current camera used in the vehicle in the weapon component
			weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
		}

		//get the original local position of the pivot
		if (gravityControl) {
			gravityControl.getCurrentCameraPivot (currentState.pivotTransform);
		}
			
		originalRotationSpeed = rotationSpeed;
	}

	void Update ()
	{
		//set the camera position in the vehicle position to follow it
		if (followVehiclePosition) {
			transform.position = vehicle.transform.position;
		}

		//if the vehicle is being driving and the pause menu is not active, allow the camera to rotate
		if (drivingVehicle) {

			isGamePaused = actionManager.input.gameCurrentlyPaused;

			if (!isGamePaused && !cameraPaused) {
				
				if (!currentState.cameraFixed) {
					//get the current input axis values from the input manager
					axisValues = actionManager.getPlayerMovementAxis ("mouse");
					mouseAxis.x = axisValues.x;
					mouseAxis.y = axisValues.y;

					playerCameraIsLocked = !playerCameraManager.isCameraTypeFree ();

					//if the first camera view is enabled
					if (currentState.firstPersonCamera) {
					
						isFirstPerson = true;

						if (currentState.useRotationInput) {

							//get the look angle value
							lookAngle.x += mouseAxis.x * rotationSpeed;
							lookAngle.y -= mouseAxis.y * rotationSpeed;
						
							//clamp these values to limit the camera rotation
							lookAngle.y = Mathf.Clamp (lookAngle.y, -currentState.xLimits.x, currentState.xLimits.y);
							lookAngle.x = Mathf.Clamp (lookAngle.x, -currentState.yLimits.x, currentState.yLimits.y);

							//set every angle in the camera and the pivot
							if (useSmoothCameraRotation && useSmoothCameraRotationFirstPerson) {
								currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalFirstPerson;
								currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalFirstPerson;

								currentPivotRotation = Quaternion.Euler (0, lookAngle.x, 0);

								currentState.pivotTransform.localRotation = 
								Quaternion.Slerp (currentState.pivotTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);
							
								currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.y, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);

								currentState.cameraTransform.localRotation = Quaternion.Euler (currentCameraUpRotation, 0, 0);								
							} else {
								currentState.cameraTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
								currentState.pivotTransform.localRotation = Quaternion.Euler (0, lookAngle.x, 0);
							}
						}
					} else {
						//else, the camera is in third person view
						isFirstPerson = false;

						if (currentState.useRotationInput) {

							//get the look angle value
							lookAngle.x = mouseAxis.x * rotationSpeed;
							lookAngle.y -= mouseAxis.y * rotationSpeed;

							//clamp these values to limit the camera rotation
							lookAngle.y = Mathf.Clamp (lookAngle.y, -currentState.xLimits.x, currentState.xLimits.y);

							//set every angle in the camera and the pivot
							if (useSmoothCameraRotation && useSmoothCameraRotationThirdPerson) {
								currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalThirdPerson;
								currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalThirdPerson;

								currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

								currentState.pivotTransform.localRotation = 
							Quaternion.Slerp (currentState.pivotTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);

								currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, currentSmoothCameraRotationSpeedHorizontal * Time.deltaTime);

								transform.Rotate (0, currentCameraUpRotation, 0);					
							} else {
								transform.Rotate (0, lookAngle.x, 0);
								currentState.pivotTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
							}
						}
						
						//get the current camera position for the camera collision detection
						currentCameraDistance = checkCameraCollision ();
						//set the local camera position
						currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);
						currentState.cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
					}

					if (currentState.useIdentityRotation || playerCameraIsLocked) {
						transform.rotation = Quaternion.identity;
					}

				} else {
					vehicleSpeed = (mainRigidbody.transform.InverseTransformDirection (mainRigidbody.velocity).z) * 3f;
					forwadDirection = 1;
					upRotationAmount = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (transform.right, vehicle.transform.right)).y) * Mathf.Rad2Deg;
					if (vehicleSpeed < -2) {
						forwadDirection = -1;
					}

					fixedRotationAmount = upRotationAmount * Time.deltaTime * rotationDamping * forwadDirection;

					if (float.IsNaN (fixedRotationAmount)) {
						fixedRotationAmount = 0;
					}

					transform.Rotate (0, fixedRotationAmount, 0);

					//get the current camera position for the camera collision detection
					currentCameraDistance = checkCameraCollision ();
					//set the local camera position
					currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);
					currentState.cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
				}
			}
		}

		//if the boost is being used, move the camera in the backward direction
		if (boosting) {
			//the camera is moving in backward direction
			if (releaseCamera) {
				originalCameraDistance += Time.deltaTime * cameraBoostSpeed;
				if (originalCameraDistance >= maximumBoostDistance + currentState.originalDist) {
					originalCameraDistance = currentState.originalDist + maximumBoostDistance;
					releaseCamera = false;
				}
			}

			//the camera is moving to its regular position
			if (pickCamera) {
				originalCameraDistance -= Time.deltaTime * cameraBoostSpeed;
				if (originalCameraDistance <= currentState.originalDist) {
					originalCameraDistance = currentState.originalDist;
					pickCamera = false;
					boosting = false;
				}
			}
		}
	}

	public void setZoom (bool state)
	{
		if (zoomEnabled && currentState.canUseZoom) {
			//to the fieldofview of the camera, it is added of substracted the zoomvalue
			usingZoomOn = state;
			float targetFov = currentState.zoomFovValue;
			float rotationSpeedTarget = currentState.rotationSpeedZoomIn;
			if (!usingZoomOn) {
				rotationSpeedTarget = originalRotationSpeed;
				targetFov = originalCameraFov;
			}
			playerCameraManager.setMainCameraFov (targetFov, currentState.zoomSpeed);
			//also, change the sensibility of the camera when the zoom is on or off, to control the camera properly
			rotationSpeed = rotationSpeedTarget;
		}
	}

	public void disableZoom ()
	{
		if (usingZoomOn) {
			usingZoomOn = false;
			rotationSpeed = originalRotationSpeed;
		}
	}

	public void setCameraState (string stateName)
	{
		for (int i = 0; i < vehicleCameraStates.Count; i++) {
			if (vehicleCameraStates [i].name == stateName) {
				currentState = new vehicleCameraStateInfo (vehicleCameraStates [i]);
				currentStateName = stateName;
			}
		}

		hudManager.disableUnlockedCursor ();
	
		disableZoom ();

		IKDrivingManager.setPassengerFirstPersonState (currentState.firstPersonCamera, player);
	}

	bool checkCameraShakeActive;

	public void setCheckCameraShakeActiveState (bool state)
	{
		checkCameraShakeActive = state;
	}

	//function called when the player uses the boost in the vehicle
	public void usingBoost (bool state, string shakeName)
	{
		if (!checkCameraShakeActive) {
			return;
		}
			
		boosting = true;
		if (state) {
			releaseCamera = true;
			pickCamera = false;
			if (shakingManager) {
				shakingManager.startShake (shakeName);
			}
		} else {
			releaseCamera = false;
			pickCamera = true;
			if (shakingManager) {
				shakingManager.stopShake ();
			}
		}
	}

	//the player has changed the current camera view for the other option, firts or third
	public void changeCameraPosition ()
	{
		if (!playerCameraManager.isCameraTypeFree ()) {
			print ("camera is locked");
			return;
		}

		//if the camera can be changed
		if (cameraChangeEnabled) {
			cameraStateIndex++;
			if (cameraStateIndex > vehicleCameraStates.Count - 1) {
				cameraStateIndex = 0;
			}
			bool exit = false;
			int max = 0;
			while (!exit) {
				for (int k = 0; k < vehicleCameraStates.Count; k++) {
					if (vehicleCameraStates [k].enabled && k == cameraStateIndex) {
						cameraStateIndex = k;
						exit = true;
					}
				}
				if (!exit) {
					max++;
					if (max > 100) {
						print ("error");
						return;
					}
					//set the current power
					cameraStateIndex++;
					if (cameraStateIndex > vehicleCameraStates.Count - 1) {
						cameraStateIndex = 0;
					}
				}
			}

			nextPivotPositon = currentState.pivotTransform.position;
			setCameraState (vehicleCameraStates [cameraStateIndex].name);

			//reset the look angle
			lookAngle = Vector2.zero;

			//reset the pivot rotation
			if (currentState.xLimits != Vector2.zero || currentState.xLimits != Vector2.zero || currentState.cameraFixed) {
				currentState.pivotTransform.localRotation = Quaternion.identity;
				currentState.cameraTransform.localRotation = Quaternion.identity;
			}

			//set the new parent of the camera as the first person position
			mainCameraTransform.SetParent (currentState.cameraTransform);

			//reset camera rotation and position
			mainCameraTransform.localPosition = Vector3.zero;
			mainCameraTransform.localRotation = Quaternion.identity;
			currentOriginalDistValue = currentState.originalDist;

			if (currentState.smoothTransition) {
				checkCameraTranslation ();
			} else {
				originalCameraDistance = currentState.originalDist;
			}

			//change the current camera in the gravity controller component
			if (gravityControl) {
				gravityControl.getCurrentCameraPivot (currentState.pivotTransform);
			}

			if (shakingManager) {
				shakingManager.getCurrentCameraTransform (mainCameraTransform);
			}

			//do the same in the weapons system if the vehicle has it
			if (weaponManager) {
				weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
			}

			if (currentState.firstPersonCamera) {
				playerManager.changeHeadScale (true);
			} else {
				playerManager.changeHeadScale (false);
			}

			if (currentState.useNewCameraFov) {
				originalCameraFov = currentState.newCameraFov;
				if (smoothTransitionsInNewCameraFov) {
					playerCameraManager.setMainCameraFov (originalCameraFov, currentState.zoomSpeed);
				} else {
					playerCameraManager.quickChangeFovValue (originalCameraFov);
				}
			}
		}
	}

	public void resetCameraRotation ()
	{
		currentState.pivotTransform.localRotation = Quaternion.identity;
		currentState.cameraTransform.localRotation = Quaternion.identity;
	}

	public void resetAxisCameraRotationToVehicleForwardDirection ()
	{
		float angleY = Vector3.Angle (vehicle.transform.forward, transform.forward);
		angleY *= Mathf.Sign (transform.InverseTransformDirection (Vector3.Cross (vehicle.transform.forward, transform.forward)).y);
		transform.Rotate (0, -angleY, 0);
	}

	public void setDamageCameraShake ()
	{
		if (isFirstPerson) {
			if (shakeSettings.useDamageShakeInFirstPerson) {
				shakingManager.setExternalShakeState (shakeSettings.firstPersonDamageShake);
			}
		} else {
			if (shakeSettings.useDamageShakeInThirdPerson) {
				shakingManager.setExternalShakeState (shakeSettings.thirdPersonDamageShake);
			}
		}
	}

	public void setCameraExternalShake (externalShakeInfo externalShake)
	{
		shakingManager.setExternalShakeState (externalShake);
	}

	//move away or turn back the camera
	public void moveAwayCamera ()
	{
		
	}

	//stop the current coroutine and start it again
	void checkCameraTranslation ()
	{
		if (moveCamera != null) {
			StopCoroutine (moveCamera);
		}
		moveCamera = StartCoroutine (changeCamerCollisionDistanceCoroutine ());
	}

	IEnumerator changeCamerCollisionDistanceCoroutine ()
	{
		currentState.pivotTransform.position = nextPivotPositon;
		//move the pivot and the camera dist for the camera collision 
		float t = 0;
		//translate position of the pivot
		while (t < 1) {
			t += Time.deltaTime * smoothBetweenState;
			originalCameraDistance = Mathf.Lerp (originalCameraDistance, currentOriginalDistValue, t);
			currentState.pivotTransform.localPosition = Vector3.Lerp (currentState.pivotTransform.localPosition, currentState.originalPivotPosition, t);
			yield return null;
		}
	}

	public void getPlayer (GameObject playerElement)
	{
		player = playerElement;
		playerManager = player.GetComponent<playerController> ();
		playerCameraManager = playerManager.getPlayerCameraManager ();
		mainCamera = playerCameraManager.getMainCamera ();
		mainCameraTransform = mainCamera.transform;
		if (shakingManager) {
			shakingManager.getCurrentCameraTransform (mainCameraTransform);
		}
	}

	public Transform getVehicleMainCameraTransform ()
	{
		return currentState.cameraTransform;
	}

	//the vehicle is being driving or not
	public void changeCameraDrivingState (bool state, bool resetCameraRotationWhenGetOn)
	{
		drivingVehicle = state;
		//if the vehicle is not being driving, stop all its states
		if (!drivingVehicle) {
			releaseCamera = false;
			pickCamera = false;
			boosting = false;

			if (playerManager) {
				playerManager.changeHeadScale (false);
			}

			if (usingZoomOn) {
				setZoom (false);
			}

			if (currentState.useNewCameraFov) {
				if (playerCameraManager) {
					playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), currentState.zoomSpeed);
				}
			}
			cameraPaused = false;
		} 
		//else, reset the vehicle camera rotation
		else {
			if (firstCameraEnabled) {
				if (playerManager) {
					playerManager.changeHeadScale (true);
				}
			}

			if (resetCameraRotationWhenGetOn) {
				//reset the camera position in the vehicle, so always that the player gets on, the camera is set just behind the vehicle
				originalCameraDistance = currentState.originalDist;

				//reset the local angle x of the pivot camera
				currentState.pivotTransform.localRotation = Quaternion.identity;
				currentState.cameraTransform.localPosition = -Vector3.forward * originalCameraDistance;
				lookAngle = Vector2.zero;

				//reset the local angle y of the vehicle camera
				resetAxisCameraRotationToVehicleForwardDirection ();
			} else {
				resetAxisCameraRotationToVehicleForwardDirection ();

				resetCameraRotation ();

				drivingVehicle = false;
				//reset the camera position in the vehicle, so always that the player gets on, the camera is set just behind the vehicle
				lookAngle = Vector2.zero;
				originalCameraDistance = currentState.originalDist;

				//reset the local angle x of the pivot camera
				Quaternion playerPivotCameraRotation = playerCameraManager.getPivotCameraTransform ().localRotation;
				currentState.pivotTransform.localRotation = playerPivotCameraRotation;
				float newLookAngleValue = playerPivotCameraRotation.eulerAngles.x;
				if (newLookAngleValue > 180) {
					newLookAngleValue -= 360;
				}
				lookAngle.y = newLookAngleValue;

				currentState.cameraTransform.localPosition = -Vector3.forward * originalCameraDistance;

				float currentVehicleCameraAngle = transform.InverseTransformVector (transform.eulerAngles).y;
				float vehicleCameraAngleTarget = 0;
				if (currentVehicleCameraAngle > 0) {
					vehicleCameraAngleTarget = 360 - currentVehicleCameraAngle;
				} else {
					vehicleCameraAngleTarget = Mathf.Abs (currentVehicleCameraAngle);
				}
				transform.Rotate (0, vehicleCameraAngleTarget, 0);

				//reset the local angle y of the vehicle camera
				float angleY = playerCameraManager.transform.InverseTransformVector (playerCameraManager.transform.eulerAngles).y;
				transform.Rotate (0, angleY, 0);
				drivingVehicle = true;
			}

			//get values for the new fov if it is configured, or the original fov from the camera player
			if (currentState.useNewCameraFov) {
				originalCameraFov = currentState.newCameraFov;
			} else {
				if (playerCameraManager) {
					originalCameraFov = playerCameraManager.getOriginalCameraFov ();
				}
			}

			if (playerCameraManager) {
				playerCameraManager.setMainCameraFov (originalCameraFov, currentState.zoomSpeed);
				if (playerCameraManager.isUsingZoom ()) {
					playerCameraManager.setUsingZoomOnValue (false);
				}
			}
		}
	}

	//when the player gets on to the vehicle, it is checked if the first person was enabled or not, to set that camera view in the vehicle too
	public void setCameraPosition (bool state)
	{
		//get the current view of the camera, so when it is changed, it is done correctly
		firstCameraEnabled = state;

		if (firstCameraEnabled) {
			setFirstOrThirdPerson (true);
		} else {
			setFirstOrThirdPerson (false);
		}

		//set the current camera view in the weapons system
		if (weaponManager) {
			weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
		}
	}

	public void setFirstOrThirdPerson (bool state)
	{
		bool assigned = false;
		for (int k = 0; k < vehicleCameraStates.Count; k++) {
			if (!assigned) {
				if (state) {
					if (vehicleCameraStates [k].firstPersonCamera) {
						setCameraState (vehicleCameraStates [k].name);
						cameraStateIndex = k;
						assigned = true;
					}
				} else {
					if (!vehicleCameraStates [k].firstPersonCamera) {
						setCameraState (vehicleCameraStates [k].name);
						cameraStateIndex = k;
						assigned = true;
					}
				}
			}
		}
	}

	public void adjustCameraToCurrentCollisionDistance ()
	{
		if (currentState.firstPersonCamera) {
			//get the current camera position for the camera collision detection
			currentCameraDistance = checkCameraCollision ();
			//set the local camera position
			currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);
			currentState.cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
		}
	}

	//adjust the camera position to avoid cross any collider
	public float checkCameraCollision ()
	{
		//launch a ray from the pivot position to the camera direction
		ray.origin = currentState.pivotTransform.position;
		ray.direction = -currentState.pivotTransform.forward;

		//store the hits received
		hits = Physics.SphereCastAll (ray, clipCastRadius, originalCameraDistance + clipCastRadius, layer);
		float closest = Mathf.Infinity;
		float hitDist = originalCameraDistance;

		//find the closest
		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].distance < closest && !hits [i].collider.isTrigger) {
				//the camera will be moved that hitDist in its forward direction
				closest = hits [i].distance;
				hitDist = -currentState.pivotTransform.InverseTransformPoint (hits [i].point).z;
			}
		}
		//clamp the hidDist value
		if (hitDist < 0) {
			hitDist = 0;
		}
		if (hitDist > originalCameraDistance) {
			hitDist = originalCameraDistance;
		}
		//return the value of the collision in the camera
		return Mathf.SmoothDamp (currentCameraDistance, hitDist, ref cameraSpeed, currentCameraDistance > hitDist ? 0 : backClipSpeed);
	}

	public void startOrStopFollowVehiclePosition (bool state)
	{
		followVehiclePosition = state;
	}

	public void pauseOrPlayVehicleCamera (bool state)
	{
		cameraPaused = state;
	}

	public bool isVehicleCameraPaused ()
	{
		return cameraPaused;
	}

	public Transform getCurrentCameraPivot ()
	{
		return currentState.pivotTransform;
	}

	public Transform getCurrentCameraTransform ()
	{
		return currentState.cameraTransform;
	}

	public void setVehicleAndCameraParent (Transform newParent)
	{
		vehicle.transform.SetParent (newParent);
		transform.SetParent (newParent);
	}

	//CALL INPUT FUNCTIONS
	public void inputChangeCamera ()
	{
		if (drivingVehicle && !isGamePaused && !cameraPaused) {
			//check if the change camera input is used
			changeCameraPosition ();
		}
	}

	public void inputZoom ()
	{
		if (drivingVehicle && !isGamePaused && !cameraPaused) {
			setZoom (!usingZoomOn);
		}
	}

	//draw the move away position of the pivot and the camera in the inspector
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

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
		//&& !Application.isPlaying
		if (showGizmo) {
			for (int i = 0; i < vehicleCameraStates.Count; i++) {
				if (vehicleCameraStates [i].showGizmo) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (vehicleCameraStates [i].pivotTransform.position, vehicleCameraStates [i].cameraTransform.position);
					Gizmos.DrawLine (vehicleCameraStates [i].pivotTransform.position, transform.position);
					Gizmos.color = vehicleCameraStates [i].gizmoColor;
					Gizmos.DrawSphere (vehicleCameraStates [i].pivotTransform.position, gizmoRadius);
					Gizmos.DrawSphere (vehicleCameraStates [i].cameraTransform.position, gizmoRadius);

					if (showCameraDirectionGizmo) {
						GKC_Utils.drawGizmoArrow (vehicleCameraStates [i].cameraTransform.position, vehicleCameraStates [i].cameraTransform.forward * gizmoArrowLineLenght, 
							vehicleCameraStates [i].gizmoColor, gizmoArrowLenght, gizmoArrowAngle);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class vehicleCameraStateInfo
	{
		public string name;
		public Transform pivotTransform;
		public Transform cameraTransform;

		public bool useRotationInput;
		public Vector2 xLimits;
		public Vector2 yLimits;

		public bool useNewCameraFov;
		public float newCameraFov;
		public bool enabled;
		public bool firstPersonCamera;
		public bool cameraFixed;
		public bool smoothTransition;

		public bool useIdentityRotation;

		public bool canUnlockCursor;

		public bool useCameraSteer;

		public bool canUseZoom = true;
		public float zoomSpeed = 120;
		public float zoomFovValue = 17;
		public float rotationSpeedZoomIn = 2.5f;

		public bool showGizmo;
		public Color gizmoColor;
		public float labelGizmoOffset;
		public bool gizmoSettings;
		[HideInInspector] public float originalDist;
		[HideInInspector] public Vector3 originalPivotPosition;

		public vehicleCameraStateInfo (vehicleCameraStateInfo newState)
		{
			name = newState.name;
			pivotTransform = newState.pivotTransform;
			cameraTransform = newState.cameraTransform;
			useRotationInput = newState.useRotationInput;
			xLimits = newState.xLimits;
			yLimits = newState.yLimits;
			useNewCameraFov = newState.useNewCameraFov;
			newCameraFov = newState.newCameraFov;
			enabled = newState.enabled;
			firstPersonCamera = newState.firstPersonCamera;
			cameraFixed = newState.cameraFixed;
			smoothTransition = newState.smoothTransition;
			useIdentityRotation = newState.useIdentityRotation;
			originalDist = newState.originalDist;
			originalPivotPosition = newState.originalPivotPosition;
			canUnlockCursor = newState.canUnlockCursor;

			canUseZoom = newState.canUseZoom;
			zoomSpeed = newState.zoomSpeed;
			zoomFovValue = newState.zoomFovValue;
			rotationSpeedZoomIn = newState.rotationSpeedZoomIn;

			useCameraSteer = newState.useCameraSteer;
		}
	}

	[System.Serializable]
	public class shakeSettingsInfo
	{
		public bool useDamageShake;
		public bool sameValueBothViews;
		public bool useDamageShakeInThirdPerson;
		public externalShakeInfo thirdPersonDamageShake;
		public bool useDamageShakeInFirstPerson;
		public externalShakeInfo firstPersonDamageShake;
	}
}