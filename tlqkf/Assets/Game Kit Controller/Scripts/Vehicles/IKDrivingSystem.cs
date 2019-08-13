using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class IKDrivingSystem : MonoBehaviour
{
	public GameObject vehicle;
	public GameObject vehicelCameraGameObject;

	public IKDrivingInformation IKDrivingInfo;

	public List<IKDrivingInformation> IKVehiclePassengersList = new List<IKDrivingInformation> ();

	public bool hidePlayerFromNPCs;
	public bool playerVisibleInVehicle = true;
	public bool ejectPlayerWhenDestroyed;
	public float ejectingPlayerForce;
	public bool useExplosionForceWhenDestroyed;
	public float explosionRadius;
	public float explosionForce;
	public float explosionDamage;

	public bool showGizmo;
	public Color gizmoLabelColor;
	public float gizmoRadius = 0.1f;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;

	public bool controlsMenuOpened;
	public bool hidePlayerWeaponsWhileDriving;
	public bool showSettings;
	public bool resetCameraRotationWhenGetOn = true;
	public bool resetCameraRotationWhenGetOff;

	public bool startGameInThisVehicle;
	public GameObject playerForVehicle;

	public bool playerIsAlwaysDriver = true;

	public bool pushCharactersOnExplosion = true;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;
	public bool killObjectsInRadius;
	public ForceMode forceMode;

	public bool useLayerMask;
	public LayerMask layer;

	public bool canBeDrivenRemotely;
	bool originalCanBeDrivenRemotelyValue;

	public bool isBeingDrivenRemotely;

	public bool activateFreeFloatingModeOnEject;
	public float activateFreeFloatingModeOnEjectDelay = 0.5f;

	bool activateFreeFloatingModeOnEjectEnabled;

	bool checkIfPlayerStartInVehicle;

	public bool drawWeaponIfCarryingPreviously;

	public bool addCollisionForceDirectionToPassengers;
	public Vector3 extraCollisionForceAmount = Vector3.one;

	public bool addAngularDirectionToPassengers;
	public float vehicleStabilitySpeed = 1;
	public Vector3 extraAngularDirectioAmount;

	public Vector3 debugCollisionForce;

	public bool activateActionScreen;
	public string actionScreenName;

	public bool useEventOnDriverGetOn;
	public eventParameters.eventToCallWithGameObject eventOnDriverGetOn;

	public inputActionManager actionManager;
	public vehicleCameraController vehicleCameraManager;
	public vehicleHUDManager HUDManager;
	public vehicleWeaponSystem currentVehicleWeaponSystem;
	public vehicleGravityControl vehicleGravityManager;

	List<GameObject> passengerGameObjectList = new List<GameObject> ();
	List<passengerComponents> passengerComponentsList = new List<passengerComponents> ();
	passengerComponents currentPassengerDriverComponents;

	bool isBeingDriven;
	bool passengersOnVehicle;

	Quaternion mainCameraTargetRotation;
	Vector3 mainCameraTargetPosition = Vector3.zero;

	Coroutine moveCamera;
	bool vehicleDestroyed;

	bool getDriverPosition;

	bool cursorUnlocked;

	List<usingDevicesSystem> usingDevicesManagerDetectedList = new List<usingDevicesSystem> ();

	Vector3 predictedUp;
	Vector3 newAngularDirection;

	void Start ()
	{
		//send the input manager component to the vehicle and its camera
		actionManager.getInputManager ();

		originalCanBeDrivenRemotelyValue = canBeDrivenRemotely;
	}

	void Update ()
	{
		if (!checkIfPlayerStartInVehicle) {
			if (startGameInThisVehicle) {
				if (playerForVehicle) {
					setDriverExternally (playerForVehicle);
				} else {
					print ("Warning: assign the player to drive this car in the field Player For Vehicle in IK Driving System inspector");
				}
			}
			checkIfPlayerStartInVehicle = true;
		}

		if (addAngularDirectionToPassengers && HUDManager.isVehicleBeingDriven ()) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (!IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
					if (!IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson) {
						newAngularDirection = Vector3.Lerp (newAngularDirection, Vector3.zero, Time.deltaTime * vehicleStabilitySpeed);
	
						IKVehiclePassengersList [i].currentAngularDirection = new Vector3 (newAngularDirection.z, newAngularDirection.y, newAngularDirection.x);
					} else {
						IKVehiclePassengersList [i].currentAngularDirection = Vector3.zero;
					}
				}
			}
		}
	}

	public void setNewAngularDirection (Vector3 newValue)
	{
		newAngularDirection = Vector3.Scale (newValue, extraAngularDirectioAmount);
	}

	public void setCollisionForceDirectionToPassengers (Vector3 forceDirection)
	{
		if (addCollisionForceDirectionToPassengers && HUDManager.isVehicleBeingDriven ()) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (!IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson && !IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
					Vector3 newForceDirection = forceDirection;

					newForceDirection = Vector3.Scale (newForceDirection, extraCollisionForceAmount);
					IKVehiclePassengersList [i].currentSeatShake = new Vector3 (newForceDirection.z, newForceDirection.y, newForceDirection.x);
				}
			}
		}
	}

	public void setPassengerFirstPersonState (bool state, GameObject passengerToCheck)
	{
		if (addCollisionForceDirectionToPassengers) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == passengerToCheck) {
					IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson = state;
				}
			}
		}
	}

	public void setDriverExternally (GameObject currentDriver)
	{
		usingDevicesSystem usingDevicesManager = currentDriver.GetComponent<usingDevicesSystem> ();
		if (usingDevicesManager) {
			usingDevicesManager.clearDeviceList ();
			usingDevicesManager.addDeviceToList (vehicle);
			usingDevicesManager.getclosestDevice ();
			usingDevicesManager.setCurrentVehicle (vehicle);
			getDriverPosition = true;
			usingDevicesManager.useDevice ();
			getDriverPosition = false;
		}
	}

	//if the vehicle is destroyed, remove it from the scene
	public void destroyVehicle ()
	{
		Destroy (vehicelCameraGameObject);
		Destroy (vehicle);
		Destroy (gameObject);
	}

	//if the vehicle is destroyed
	public void disableVehicle ()
	{
		vehicleDestroyed = true;
		//disable its components
		vehicle.GetComponent<Collider> ().enabled = false;

		//if the player was driving it
		if (passengersOnVehicle || isBeingDriven) {
			//stop the vehicle

			ejectVehiclePassengers (ejectPlayerWhenDestroyed, ejectingPlayerForce);

			//disable the weapon system if the vehicle has it
			if (currentVehicleWeaponSystem) {
				if (currentVehicleWeaponSystem.enabled) {
					currentVehicleWeaponSystem.changeWeaponState (false);
				}
			}
		}

		//disable the camera and the gravity control component
		vehicleCameraManager.enabled = false;
		vehicle.SendMessage ("disableVehicle");
		if (vehicleGravityManager) {
			vehicleGravityManager.enabled = false;
		}

		Vector3 vehiclePosition = vehicle.transform.position;

		if (useExplosionForceWhenDestroyed) {
			applyDamage.setExplosion (vehiclePosition, explosionRadius, useLayerMask, layer, vehicle, true, gameObject, killObjectsInRadius, true, false, 
				explosionDamage, pushCharactersOnExplosion, applyExplosionForceToVehicles, explosionForceToVehiclesMultiplier, explosionForce, forceMode, false, vehicle.transform);
		}

		//remove this vehicle from any character using device system
		for (int i = 0; i < usingDevicesManagerDetectedList.Count; i++) {
			if (usingDevicesManagerDetectedList [i]) {
				if (usingDevicesManagerDetectedList [i].existInDeviceList (vehicle)) {
					usingDevicesManagerDetectedList [i].removeVehicleFromList ();
					usingDevicesManagerDetectedList [i].removeCurrentVehicle (vehicle);
					usingDevicesManagerDetectedList [i].setIconButtonCanBeShownState (false);
				}
			}
		}
	}

	public void ejectVehiclePassengersOnSelfDestruct (float ejectPassengerFoce)
	{
		ejectVehiclePassengers (true, ejectPassengerFoce);
	}

	public void ejectVehiclePassengers (bool ejectPassenger, float ejectForce)
	{
		List<GameObject> auxPassengerGameObjectList = new List<GameObject> ();
		List<passengerComponents> auxPassengerComponentsList = new List<passengerComponents> ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			auxPassengerGameObjectList.Add (passengerGameObjectList [i]);
			passengerComponents auxCurrentPassengerDriverComponents = new passengerComponents (passengerComponentsList [i]);
			auxPassengerComponentsList.Add (auxCurrentPassengerDriverComponents);
		}

		for (int i = 0; i < auxPassengerGameObjectList.Count; i++) {
			//disable the option to get off from the vehicle if the player press that button
			passengerComponents auxCurrentPassengerDriverComponents = auxPassengerComponentsList [i];
			if (auxCurrentPassengerDriverComponents.usingDevicesManager) {
				auxCurrentPassengerDriverComponents.usingDevicesManager.removeVehicleFromList ();
				auxCurrentPassengerDriverComponents.usingDevicesManager.disableIcon ();
			}

			Vector3 nextPlayerPosition = Vector3.zero;
			if (auxPassengerGameObjectList [i]) {
				nextPlayerPosition = auxPassengerGameObjectList [i].transform.position;
			}
			startOrStopVehicle (auxPassengerGameObjectList [i], null, vehicelCameraGameObject.transform.up, nextPlayerPosition);

			Vector3 vehiclePosition = vehicle.transform.position;

			if (auxPassengerGameObjectList [i] && auxCurrentPassengerDriverComponents.passengerPhysicallyOnVehicle) {
				if (ejectPassenger) {
					//eject the player from the car
					if ((activateFreeFloatingModeOnEject || activateFreeFloatingModeOnEjectEnabled) && !auxCurrentPassengerDriverComponents.playerControllerManager.isCharacterUsedByAI ()) {
						auxCurrentPassengerDriverComponents.playerGravityManager.setfreeFloatingModeOnStateWithDelay (activateFreeFloatingModeOnEjectDelay, true);
						activateFreeFloatingModeOnEjectEnabled = false;
					} 

					auxCurrentPassengerDriverComponents.playerControllerManager.ejectPlayerFromVehicle (ejectForce);
				} else {
					//kill him
					Vector3 vehicleDirection = auxPassengerGameObjectList [i].transform.position - vehiclePosition;
					vehicleDirection = vehicleDirection / vehicleDirection.magnitude;
					applyDamage.killCharacter (vehicle, auxPassengerGameObjectList [i], vehicleDirection, vehiclePosition, vehicle, false);
				}

				if (!playerVisibleInVehicle) {
					auxCurrentPassengerDriverComponents.playerControllerManager.setCharacterMeshGameObjectState (true);
					enableOrDisablePlayerVisibleInVehicle (true, auxCurrentPassengerDriverComponents);
				}
			}
		}
	}

	public void setActivateFreeFloatingModeOnEjectEnabledState (bool state)
	{
		activateFreeFloatingModeOnEjectEnabled = state;
	}

	//the player is getting in or off from the vehicle
	public bool startOrStopVehicle (GameObject currentPassenger, Transform passengerParent, Vector3 normal, Vector3 nextPlayerPos)
	{
		if (passengerGameObjectList.Count == 0) {
			
			vehicleCameraManager.setCameraPosition (false);
			vehicleCameraManager.changeCameraDrivingState (false, false);
		
			return false;
		}

		IKDrivingInformation currentIKVehiclePassengerInfo = getIKVehiclePassengerInfo (currentPassenger);

		int currentPassengerIndex = passengerGameObjectList.IndexOf (currentPassenger);
		passengerComponents currentPassengerComponents = passengerComponentsList [currentPassengerIndex];

		bool passengerIsGettingOn = false;
		passengerIsGettingOn = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree;
		currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree = !passengerIsGettingOn;
		if (passengerIsGettingOn) {
			passengerIsGettingOn = true;
		}

		bool passengerOnDriverSeat = false;
		if (currentIKVehiclePassengerInfo.vehicleSeatInfo.isDriverSeat) {
			passengerOnDriverSeat = true;
			currentPassengerDriverComponents = currentPassengerComponents;
		}

		currentPassengerComponents.passengerOnDriverSeat = passengerOnDriverSeat;

		if (passengerOnDriverSeat) {
			isBeingDriven = passengerIsGettingOn;
		} else {
			isBeingDriven = false;
		}

		//print (passengerIsGettingOn + " " + passengerOnDriverSeat);

		bool isPlayer = false;
		if (!currentPassengerComponents.playerControllerManager.usedByAI) {
			isPlayer = true;
		}

		//set the state driving as the current state of the player
		currentPassengerComponents.playerControllerManager.setDrivingState (passengerIsGettingOn, vehicle);

		isBeingDrivenRemotely = !checkIfNotDrivenRemotely (isPlayer);

		if (!isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.enabled = !passengerIsGettingOn;
			currentPassengerComponents.passengerPhysicallyOnVehicle = true;

			currentPassengerComponents.playerControllerManager.changeScriptState (!passengerIsGettingOn);
		} else {
			currentPassengerComponents.playerControllerManager.changeScriptState (!passengerIsGettingOn);
			if (currentPassengerComponents.mapManager) {
				if (passengerIsGettingOn) {
					currentPassengerComponents.mapManager.setNewObjectToFollow (vehicle);
				} else {
					currentPassengerComponents.mapManager.setNewObjectToFollow (currentPassengerComponents.currentPassenger);
				}
			}
		}

		currentPassengerComponents.playerControllerManager.setUsingDeviceState (passengerIsGettingOn);

		if (isPlayer) {
			currentPassengerComponents.pauseManager.usingDeviceState (passengerIsGettingOn);

			currentPassengerComponents.pauseManager.enableOrDisableWeaponSlotsParentOutOfInventory (!passengerIsGettingOn);
		}

		//enable or disable the collider and the rigidbody of the player
		if (!isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.setMainColliderState (!passengerIsGettingOn);
	
			currentPassengerComponents.mainRigidbody.isKinematic = passengerIsGettingOn;

			//get the IK positions of the car to use them in the player
			currentPassengerComponents.IKManager.setDrivingState (passengerIsGettingOn, currentIKVehiclePassengerInfo);
		}

		//check if the camera in the player is in first or third view, to set the current view in the vehicle
		bool firstCameraEnabled = currentPassengerComponents.playerCameraManager.isFirstPersonActive ();
		if (isPlayer && passengerIsGettingOn && passengerOnDriverSeat) {
			vehicleCameraManager.setCameraPosition (firstCameraEnabled);
		}

		if (isPlayer) {
			//enable and disable the player's HUD and the vehicle's HUD
			currentPassengerComponents.HUDInfoManager.playerHUD.SetActive (!passengerIsGettingOn);

			if (passengerOnDriverSeat) {
				if (HUDManager.showVehicleHUDActive ()) {
					currentPassengerComponents.HUDInfoManager.vehicleHUD.SetActive (isBeingDriven);
				}

				//get the vehicle's HUD elements to show the current values of the vehicle, like health, energy, ammo....
				HUDManager.getHUDBars (currentPassengerComponents.HUDInfoManager, currentPassengerComponents.HUDInfoManager.getHudElements (), isBeingDriven);

				checkActivateActionScreen (passengerIsGettingOn, currentPassengerComponents.playerControllerManager);

				if (useEventOnDriverGetOn) {
					eventOnDriverGetOn.Invoke (currentPassengerComponents.currentPassenger);
				}
			}
		}

		currentPassengerComponents.mainFootStepManager.enableOrDisableFootSteps (!passengerIsGettingOn);

		currentPassengerComponents.healthManager.setSliderVisibleState (!passengerIsGettingOn);

		if (actionManager && isPlayer && passengerOnDriverSeat) {
			actionManager.enableOrDisableInput (isBeingDriven, currentPassengerComponents.currentPassenger);
		}

		currentPassengerComponents.playerCameraManager.playOrPauseHeadBob (!passengerIsGettingOn);

		if (isPlayer && passengerOnDriverSeat) {
			vehicleCameraManager.getPlayer (currentPassenger);
		}

		bool isCurrentPassengerCameraLocked = !currentPassengerComponents.playerCameraManager.isCameraTypeFree ();
	
		//if the first camera was enabled, set the current main camera position in the first camera position of the vehicle
		if (firstCameraEnabled) {
			//enable the player's body to see it
			if (passengerIsGettingOn) {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (true);
				//if the first person was actived, disable the player's body
			} else {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (false);
			}

			currentPassengerComponents.playerControllerManager.setAnimatorState (passengerIsGettingOn);
		}

		if (isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.setDrivingRemotelyState (passengerIsGettingOn);
		}

		//if the player is driving it
		if (passengerIsGettingOn) {
			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.HUDInfoManager.setControlList (actionManager);
				currentPassengerComponents.HUDInfoManager.setCurrentVehicleHUD (vehicle);
				//disable or enable the vehicle camera
				vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
			}

			//check the current state of the player, to check if he is carrying an object, aiming, etc... to disable that state
			currentPassengerComponents.statesManager.checkPlayerStates ();

			//disable the camera rotation of the player 
			if (!isCurrentPassengerCameraLocked) {
				currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
			}

			if (isPlayer) {
				//change the main camera from the player camera component to the vehicle's camera component
				currentPassengerComponents.originalCameraParentTransform = currentPassengerComponents.playerCameraManager.getCameraTransform ();
			
				if (!isCurrentPassengerCameraLocked) {

					vehicleCameraManager.adjustCameraToCurrentCollisionDistance ();

					//else the main camera position in the third camera position of the vehicle
					currentPassengerComponents.mainCameraTransform.SetParent (vehicleCameraManager.currentState.cameraTransform);
				}
			}

			//set the player's position and parent inside the car
			if (!isBeingDrivenRemotely) {
				currentPassenger.transform.SetParent (passengerParent.transform);
				currentPassenger.transform.localPosition = Vector3.zero;
				currentPassenger.transform.localRotation = Quaternion.identity;
				currentPassenger.transform.position = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform.position;
				currentPassenger.transform.rotation = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform.rotation;
			}
				
			//get the vehicle camera rotation
			mainCameraTargetRotation = vehicleCameraManager.currentState.cameraTransform.localRotation;

			if (passengerOnDriverSeat) {
				//reset the player's camera rotation input values
				currentPassengerComponents.playerCameraManager.setLookAngleValue (Vector2.zero);

				//set the player's camera rotation as the same in the vehicle
				currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicelCameraGameObject.transform.rotation;
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			currentPassengerComponents.usingDevicesManager.clearDeviceListButOne (vehicle);

			//set the same rotation in the camera pivot
			if (isPlayer) {
				if (passengerOnDriverSeat) {
					currentPassengerComponents.playerCameraManager.getPivotCameraTransform ().localRotation = vehicleCameraManager.currentState.pivotTransform.localRotation;

					currentPassengerComponents.playerCameraManager.resetMainCameraTransformLocalPosition ();
					currentPassengerComponents.playerCameraManager.resetPivotCameraTransformLocalPosition ();
					currentPassengerComponents.playerCameraManager.resetCurrentCameraStateAtOnce ();
				} else {
					if (resetCameraRotationWhenGetOn) {
						float angleY = Vector3.Angle (vehicle.transform.forward, currentPassengerComponents.playerCameraGameObject.transform.forward);
						angleY *= Mathf.Sign (currentPassengerComponents.playerCameraGameObject.transform.InverseTransformDirection (
							Vector3.Cross (vehicle.transform.forward, currentPassengerComponents.playerCameraGameObject.transform.forward)).y);
						currentPassengerComponents.playerCameraGameObject.transform.Rotate (0, -angleY, 0);
					}
				}
			}
		} 

		//the player gets off from the vehicle
		else {
			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.HUDInfoManager.setCurrentVehicleHUD (null);
			}
				
			if (!isBeingDrivenRemotely) {
				//set the parent of the player as null
				currentPassenger.transform.SetParent (null);

				if (isPlayer && !passengerOnDriverSeat) {
					if (!isCurrentPassengerCameraLocked) {
						//change the main camera parent to player's camera
						currentPassengerComponents.mainCameraTransform.SetParent (null);
					}
				}

				//set the player's position at the correct side of the car
				currentPassenger.transform.position = nextPlayerPos;

				bool setPlayerGravityOwnGravityDirection = false;
				//set the current gravity of the player's as the same in the vehicle
				if (vehicleGravityManager && vehicleGravityManager.changeDriverGravityWhenGetsOffActive ()) {
					//if the option to change player's gravity is on, change his gravity direction
					currentPassengerComponents.playerGravityManager.setNormal (normal);
				} else {
					//else, check if the player was using the gravity power before get on the vehicle
					if (!currentPassengerComponents.playerGravityManager.isUsingRegularGravity ()) {
						setPlayerGravityOwnGravityDirection = true;
					} else {
						//else, his rotation is (0,0,0)
						currentPassenger.transform.rotation = Quaternion.identity;
					}
				}

				//set the player's camera position in the correct place
				currentPassengerComponents.playerCameraGameObject.transform.position = nextPlayerPos;

				if (resetCameraRotationWhenGetOff) {
					float angleY = Vector3.Angle (vehicle.transform.forward, currentPassengerComponents.playerCameraGameObject.transform.forward);
					angleY *= Mathf.Sign (currentPassengerComponents.playerCameraGameObject.transform.InverseTransformDirection (
						Vector3.Cross (vehicle.transform.forward, currentPassengerComponents.playerCameraGameObject.transform.forward)).y);
					currentPassengerComponents.playerCameraGameObject.transform.Rotate (0, -angleY, 0);
				} else {

					if (passengerOnDriverSeat) {
						Quaternion vehiclePivotRotation = Quaternion.identity;
						if (vehicleCameraManager.currentState.firstPersonCamera) {
							currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicleCameraManager.currentState.pivotTransform.rotation;
							vehiclePivotRotation = vehicleCameraManager.getCurrentCameraTransform ().localRotation;
						} else {
							currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicelCameraGameObject.transform.rotation;
							vehiclePivotRotation = vehicleCameraManager.getCurrentCameraPivot ().localRotation;
						}

						currentPassengerComponents.playerCameraManager.getPivotCameraTransform ().localRotation = vehiclePivotRotation;
						float newLookAngleValue = vehiclePivotRotation.eulerAngles.x;
						if (newLookAngleValue > 180) {
							newLookAngleValue -= 360;
						}
						currentPassengerComponents.playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
					}
				}
				
				if (vehicleGravityManager && !vehicleGravityManager.changeDriverGravityWhenGetsOffActive ()) {
					if (!vehicleGravityManager.isUsingRegularGravity ()) {
						if (!currentPassengerComponents.playerGravityManager.isUsingRegularGravity ()) {
							setPlayerGravityOwnGravityDirection = true;
						} else {
							currentPassengerComponents.playerCameraGameObject.transform.rotation = Quaternion.identity;
						}
					}
				}

				if (setPlayerGravityOwnGravityDirection) {
					currentPassenger.transform.position = getGetOffPosition (currentIKVehiclePassengerInfo);
					currentPassengerComponents.playerCameraGameObject.transform.position = currentPassenger.transform.position;
					currentPassengerComponents.playerGravityManager.setNormal (currentPassengerComponents.playerGravityManager.getCurrentNormal ());
				}

				if (isPlayer) {
					mainCameraTargetRotation = Quaternion.identity;
				}

				if (drawWeaponIfCarryingPreviously && currentPassengerComponents.carryingWeaponsPrevioulsy) {
					currentPassengerComponents.playerWeaponManager.checkIfDrawSingleOrDualWeapon ();
				}
			}

			if (vehicleDestroyed && firstCameraEnabled) {
				if (!isCurrentPassengerCameraLocked) {
					currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
				}
				vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
				return passengerOnDriverSeat;
			}

			//add originalCameraParentTransform to passengerComponents
			if (isPlayer && passengerOnDriverSeat) {
				if (!isCurrentPassengerCameraLocked) {
					//change the main camera parent to player's camera
					currentPassengerComponents.mainCameraTransform.SetParent (currentPassengerComponents.originalCameraParentTransform);
				}
			}

			if (isPlayer) {
				//disable or enable the vehicle camera
				vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			if (isBeingDrivenRemotely) {
				if (!passengersInsideTrigger.Contains (currentPassengerComponents.currentPassenger)) {
					currentPassengerComponents.usingDevicesManager.removeVehicleFromList ();
					currentPassengerComponents.usingDevicesManager.removeCurrentVehicle (vehicle);
					currentPassengerComponents.usingDevicesManager.setIconButtonCanBeShownState (false);
				}
			}
		}

		if (!isBeingDrivenRemotely) {
			if (!playerVisibleInVehicle) {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (!passengerIsGettingOn);
				enableOrDisablePlayerVisibleInVehicle (passengerIsGettingOn, currentPassengerComponents);
			}

			if (hidePlayerWeaponsWhileDriving) {
				currentPassengerComponents.playerWeaponManager.enableOrDisableEnabledWeaponsMesh (!passengerIsGettingOn);
			}
		}

		if (isPlayer) {
			//stop the current transition of the main camera from the player to the vehicle and viceversa if the camera is moving from one position to another
			if (!isCurrentPassengerCameraLocked) {
				checkCameraTranslation (passengerIsGettingOn, currentPassengerComponents.mainCameraTransform,
					currentPassengerComponents.playerCameraManager, currentPassengerComponents.passengerOnDriverSeat);
			}
		} else {
			currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
		}

		if (!passengerIsGettingOn) {
			removePassengerInfo (currentIKVehiclePassengerInfo);

			setCanBeDrivenRemotelyState (originalCanBeDrivenRemotelyValue);
		}

		return passengerOnDriverSeat;
	}

	public bool checkIfNotDrivenRemotely (bool currentPassengerIsPlayer)
	{
		if (!canBeDrivenRemotely || !currentPassengerIsPlayer || (currentPassengerIsPlayer && !canBeDrivenRemotely)) {
			return true;
		}
		return false;
	}

	public void setCanBeDrivenRemotelyState (bool state)
	{
		canBeDrivenRemotely = state;
	}

	public bool getCanBeDrivenRemotelyValue ()
	{
		return canBeDrivenRemotely;
	}

	public void enableOrDisablePlayerVisibleInVehicle (bool state, passengerComponents currentPassengerComponents)
	{
		if (!currentPassengerComponents.playerCameraManager.isFirstPersonActive ()) {
			if (currentPassengerComponents.jetPackManager) {
				currentPassengerComponents.jetPackManager.enableOrDisableJetPackMesh (!state);
			}

			currentPassengerComponents.playerWeaponManager.enableOrDisableWeaponsMesh (!state);
			currentPassengerComponents.playerGravityManager.arrow.SetActive (!state);
		} else {
			currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (false);
		}
	}

	//stop the current coroutine and start it again
	public void checkCameraTranslation (bool state, Transform mainCameraTransform, playerCamera playerCameraManager, bool passengerOnDriverSeat)
	{
		if (moveCamera != null) {
			StopCoroutine (moveCamera);
		}
		moveCamera = StartCoroutine (adjustCamera (state, mainCameraTransform, playerCameraManager, passengerOnDriverSeat));
	}

	//move the camera position and rotation from the player's camera to vehicle's camera and viceversa
	IEnumerator adjustCamera (bool state, Transform mainCameraTransform, playerCamera playerCameraManager, bool passengerOnDriverSeat)
	{
		playerCameraManager.setVehicleCameraMovementCoroutine (moveCamera);

		if (!passengerOnDriverSeat) {
			if (state) {
				playerCameraManager.setCameraState ("Vehicle Passenger");
				playerCameraManager.configureCurrentLerpState (vehicleCameraManager.currentState.pivotTransform.localPosition, vehicleCameraManager.currentState.cameraTransform.localPosition);
				playerCameraManager.setTargetToFollow (vehicleCameraManager.transform);
				playerCameraManager.resetCurrentCameraStateAtOnce ();
				playerCameraManager.configureCameraAndPivotPositionAtOnce ();
				playerCameraManager.getMainCamera ().transform.SetParent (playerCameraManager.getCameraTransform ());
			} else {
				playerCameraManager.pauseOrPlayCamera (false);
				playerCameraManager.getMainCamera ().transform.SetParent (null);
		
				playerCameraManager.setCameraState ("Third Person");
				playerCameraManager.resetCurrentCameraStateAtOnce ();
				playerCameraManager.configureCameraAndPivotPositionAtOnce ();
				playerCameraManager.setOriginalTargetToFollow ();

				playerCameraManager.getMainCamera ().transform.SetParent (playerCameraManager.getCameraTransform ());
			}
		}


		float i = 0;
		//store the current rotation of the camera
		Quaternion currentRotation = mainCameraTransform.localRotation;
		//store the current position of the camera
		Vector3 currentPosition = mainCameraTransform.localPosition;

		//if the game starts with the player inside the vehicle, set his camera in the vehicle camera target transform directly
		if (!checkIfPlayerStartInVehicle && startGameInThisVehicle) {
			mainCameraTransform.localRotation = mainCameraTargetRotation;
			mainCameraTransform.localPosition = mainCameraTargetPosition;
		} else {
			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * 2;
				mainCameraTransform.localRotation = Quaternion.Lerp (currentRotation, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPosition, mainCameraTargetPosition, i);
				yield return null;
			}
		}

		if (!passengerOnDriverSeat) {
			if (state) {
				playerCameraManager.pauseOrPlayCamera (true);
			}
		}

		vehicleCameraManager.setCheckCameraShakeActiveState (state);

		//enable the camera rotation of the player if the vehicle is not being droven
		if (!state) {
			playerCameraManager.pauseOrPlayCamera (!state);
		}
	}

	//set the camera when the player is driving on locked camera
	public void setPlayerCameraParentAndPosition (Transform mainCameraTransform, playerCamera playerCameraManager)
	{
		vehicleCameraManager.changeCameraDrivingState (true, true);
		
		mainCameraTransform.SetParent (vehicleCameraManager.getCurrentCameraTransform ());

		bool passengerOnDriverSeat = false;
		for (int i = 0; i < passengerComponentsList.Count; i++) {
			if (passengerComponentsList [i].playerCameraManager == playerCameraManager) {
				passengerOnDriverSeat = passengerComponentsList [i].passengerOnDriverSeat;
			}
		}

		checkCameraTranslation (true, mainCameraTransform, playerCameraManager, passengerOnDriverSeat);
	}

	public void openOrCloseControlsMenu (bool state)
	{
		if ((!currentPassengerDriverComponents.pauseManager.playerMenuActive || controlsMenuOpened) && currentPassengerDriverComponents.playerControllerManager.usingDevice) {
			controlsMenuOpened = state;
			currentPassengerDriverComponents.pauseManager.openOrClosePlayerMenu (controlsMenuOpened, 
				currentPassengerDriverComponents.HUDInfoManager.vehicleControlsMenu.transform, currentPassengerDriverComponents.HUDInfoManager.useBlurUIPanel);
			currentPassengerDriverComponents.pauseManager.showOrHideCursor (controlsMenuOpened);

			//disable the touch controls
			currentPassengerDriverComponents.pauseManager.checkTouchControls (!controlsMenuOpened);

			//disable the camera rotation
			pauseOrPlayVehicleCamera (controlsMenuOpened);
			currentPassengerDriverComponents.pauseManager.usingSubMenuState (controlsMenuOpened);

			currentPassengerDriverComponents.pauseManager.enableOrDisableDynamicElementsOnScreen (!controlsMenuOpened);

			currentPassengerDriverComponents.HUDInfoManager.openOrCloseControlsMenu (controlsMenuOpened);

			if (currentVehicleWeaponSystem) {
				currentVehicleWeaponSystem.setWeaponsPausedState (controlsMenuOpened);
			}
		}
	}

	public bool setUnlockCursorState (bool state)
	{
		if (currentPassengerDriverComponents == null) {
			return false;
		}

		if (currentPassengerDriverComponents.pauseManager &&
		    (!currentPassengerDriverComponents.pauseManager.playerMenuActive || cursorUnlocked) && currentPassengerDriverComponents.playerControllerManager.usingDevice) {
			cursorUnlocked = state;
			currentPassengerDriverComponents.pauseManager.openOrClosePlayerMenu (cursorUnlocked, null, false);
			currentPassengerDriverComponents.pauseManager.showOrHideCursor (cursorUnlocked);

			//disable the camera rotation
			pauseOrPlayVehicleCamera (cursorUnlocked);
			currentPassengerDriverComponents.pauseManager.usingSubMenuState (cursorUnlocked);
			if (currentVehicleWeaponSystem) {
				currentVehicleWeaponSystem.setWeaponsPausedState (cursorUnlocked);
			}
			return true;
		}

		return false;
	}

	public void setCameraAndWeaponsPauseState (bool state)
	{
		pauseOrPlayVehicleCamera (state);
		if (currentVehicleWeaponSystem) {
			currentVehicleWeaponSystem.setWeaponsPausedState (state);
		}
	}

	public void pauseOrPlayVehicleCamera (bool state)
	{
		vehicleCameraManager.pauseOrPlayVehicleCamera (state);
	}

	public GameObject getcurrentDriver ()
	{
		if (hidePlayerFromNPCs) {
			return null;
		}
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat && !IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				return IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger;
			}
		}
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (!IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				return IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger;
			}
		}
		return null;
	}

	public vehicleHUDManager getHUDManager ()
	{
		return HUDManager;
	}

	public Vector3 getPassengerGetOffPosition (GameObject currentPassenger)
	{

		//search if the current passenger is in the list, to avoid get the closest seat
		int currentPassengerGameObjectIndex = -1;
		if (passengerGameObjectList.Contains (currentPassenger)) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == currentPassenger) {
					currentPassengerGameObjectIndex = i;
				}
			}	
		}

		IKDrivingInformation currentIKVehiclePassengerInfo;

		if (currentPassengerGameObjectIndex == -1) {

			int currentIKVehiclePassengerIndex = -1;

			//print (currentPassenger.name);

			bool setDriverPosition = false;
			playerController currentPlayerController = currentPassenger.GetComponent<playerController> ();
			if (currentPlayerController) {
				if (!currentPlayerController.usedByAI) {
					if (playerIsAlwaysDriver) {
						setDriverPosition = true;
					}
				}
			} else {
				print ("WARNING: player not found");
			}

			if (getDriverPosition || setDriverPosition) {
				currentIKVehiclePassengerIndex = getDriverSeatPassengerIndex ();
			} else {
				currentIKVehiclePassengerIndex = getClosestSeatToPassengerIndex (currentPassenger);
			}
				
			if (currentIKVehiclePassengerIndex > -1) {
				currentIKVehiclePassengerInfo = IKVehiclePassengersList [currentIKVehiclePassengerIndex];
			} else {
				return -Vector3.one;	
			}

			bool isGettingOn = setPassengerInfo (currentIKVehiclePassengerInfo, currentPassenger);

			if (isGettingOn) {
				return Vector3.zero;
			}
		} else {
			currentIKVehiclePassengerInfo = IKVehiclePassengersList [currentPassengerGameObjectIndex];
		}

		Transform seatTransform = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform;
		Vector3 getOffPosition = Vector3.zero;
		Vector3 rayDirection = Vector3.zero;
		Vector3 passengerPosition = Vector3.zero;
		Vector3 rayPosition = Vector3.zero;
		float rayDistance = 0;
		Ray ray = new Ray ();

		bool canGetOff = false;
		RaycastHit[] hits;

		if (currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
			getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
			rayDirection = seatTransform.right;
		} else {
			getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
			rayDirection = -seatTransform.right;
		}

		rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

		rayPosition = getOffPosition - rayDirection * rayDistance;

		//set the ray origin at the vehicle position with a little offset set in the inspector
		ray.origin = rayPosition;
		//set the ray direction to the left
		ray.direction = rayDirection;
		//get all the colliders in that direction where the yellow sphere is placed
		hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, HUDManager.layer);
		//get the position where the player will be placed
		passengerPosition = getOffPosition;
		if (hits.Length == 0) {
			//any obstacle detected, so the player can get off
			canGetOff = true;
		}

		//some obstacles found
		for (int i = 0; i < hits.Length; i++) {
			//check the distance to that obstacles, if they are lower that the rayDistance, the player can get off
			if (hits [i].distance > rayDistance) {
				canGetOff = true;
			} else {
				canGetOff = false;
			}
		}

		//if the left side is blocked, then check the right side in the same way that previously
		if (!canGetOff) {
			if (currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
				getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
				rayDirection = -seatTransform.right;
			} else {
				getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
				rayDirection = seatTransform.right;
			}

			rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

			rayPosition = getOffPosition - rayDirection * rayDistance;

			ray.origin = rayPosition;

			ray.direction = rayDirection;

			hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, HUDManager.layer);

			passengerPosition = getOffPosition;
			if (hits.Length == 0) {
				canGetOff = true;
			}
			for (int i = 0; i < hits.Length; i++) {
				if (hits [i].distance > rayDistance) {
					canGetOff = true;
				} else {
					canGetOff = false;
				}
			}
		}
	
		//if both sides are blocked, exit the function and the player can't get off
		if (!canGetOff) {
			return -Vector3.one;	
		}

		//if any side is avaliable then check a ray in down direction, to place the player above the ground
		RaycastHit hit;
		if (Physics.Raycast (getOffPosition, -vehicle.transform.up, out hit, currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffDistance, HUDManager.layer)) {
			Debug.DrawRay (getOffPosition, -vehicle.transform.up * hit.distance, Color.yellow);
			passengerPosition = hit.point;
		}

		return passengerPosition;
	}

	public Vector3 getGetOffPosition (IKDrivingInformation passengerInfo)
	{
		if (passengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
			return passengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
		} else {
			return passengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
		}
	}

	public int getClosestSeatToPassengerIndex (GameObject currentPassenger)
	{
		bool characterCanDrive = false;
		if (currentPassenger.GetComponent<playerController> ().canCharacterDrive ()) {
			characterCanDrive = true;
		}
		int currentIKVehiclePassengerIndex = -1;
		float maxDistance = Mathf.Infinity;
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				if (characterCanDrive || (!IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat && !characterCanDrive)) {
					float currentDistance = GKC_Utils.distance (currentPassenger.transform.position, IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform.position);
					if (currentDistance < maxDistance) {
						maxDistance = currentDistance;
						currentIKVehiclePassengerIndex = i;
					}
				}
			}
		}
		return currentIKVehiclePassengerIndex;
	}

	public int getDriverSeatPassengerIndex ()
	{
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat) {
				return i;
			}
		}
		return -1;
	}

	public IKDrivingInformation getIKVehiclePassengerInfo (GameObject passenger)
	{
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == passenger) {
				return IKVehiclePassengersList [i];
			}
		}
		return null;
	}

	public bool setPassengerInfo (IKDrivingInformation passengerInfo, GameObject passenger)
	{
		bool isGettingOn = false;
		if (!passengerGameObjectList.Contains (passenger)) {
			passengerGameObjectList.Add (passenger);
			passengerInfo.vehicleSeatInfo.currentPassenger = passenger;
			isGettingOn = true;
			setPassengerComponents (passenger);
			passengersOnVehicle = true;
		}
		return isGettingOn;
	}


	public void setPassengerComponents (GameObject currentPassenger)
	{
		passengerComponents newPassengerComponentsList = new passengerComponents ();
		newPassengerComponentsList.currentPassenger = currentPassenger;
		newPassengerComponentsList.playerControllerManager = currentPassenger.GetComponent<playerController> ();
		newPassengerComponentsList.usingDevicesManager = currentPassenger.GetComponent<usingDevicesSystem> ();
		newPassengerComponentsList.playerCameraGameObject = newPassengerComponentsList.playerControllerManager.playerCameraGameObject;
		newPassengerComponentsList.playerGravityManager = currentPassenger.GetComponent<gravitySystem> ();
		newPassengerComponentsList.playerCameraManager = newPassengerComponentsList.playerCameraGameObject.GetComponent<playerCamera> ();
		newPassengerComponentsList.playerWeaponManager = currentPassenger.GetComponent<playerWeaponsManager> ();
		newPassengerComponentsList.mainCamera = newPassengerComponentsList.playerCameraManager.getMainCamera ();
		newPassengerComponentsList.mainCameraTransform = newPassengerComponentsList.mainCamera.transform;
		newPassengerComponentsList.jetPackManager = currentPassenger.GetComponent<jetpackSystem> ();
		newPassengerComponentsList.mainRigidbody = currentPassenger.GetComponent<Rigidbody> ();
		newPassengerComponentsList.IKManager = currentPassenger.GetComponent<IKSystem> ();
		newPassengerComponentsList.mainFootStepManager = currentPassenger.GetComponent<footStepManager> ();
		newPassengerComponentsList.healthManager = currentPassenger.GetComponent<health> ();
		newPassengerComponentsList.statesManager = currentPassenger.GetComponent<playerStatesManager> ();
		if (!newPassengerComponentsList.playerControllerManager.usedByAI) {
			newPassengerComponentsList.pauseManager = currentPassenger.GetComponent<playerInputManager> ().getPauseManager ();
		}
		newPassengerComponentsList.carryingWeaponsPrevioulsy = newPassengerComponentsList.playerWeaponManager.isUsingWeapons ();

		if (newPassengerComponentsList.pauseManager) {
			newPassengerComponentsList.HUDInfoManager = newPassengerComponentsList.pauseManager.getHUDInfoManager ();
			newPassengerComponentsList.mapManager = newPassengerComponentsList.pauseManager.getMapSystem ();
		}

		passengerComponentsList.Add (newPassengerComponentsList);
	}

	public void removePassengerInfo (IKDrivingInformation passengerInfo)
	{
		if (passengerGameObjectList.Contains (passengerInfo.vehicleSeatInfo.currentPassenger)) {
			int passengerGameObjecToRemoveIndex = passengerGameObjectList.IndexOf (passengerInfo.vehicleSeatInfo.currentPassenger);
			passengerComponentsList.RemoveAt (passengerGameObjecToRemoveIndex);
			passengerGameObjectList.Remove (passengerInfo.vehicleSeatInfo.currentPassenger);

			if (passengerGameObjectList.Count == 0) {
				passengersOnVehicle = false;
			}
			passengerInfo.vehicleSeatInfo.currentPassenger = null;
		}
	}

	public void addPassenger ()
	{
		IKDrivingInformation newIKDrivingInformation = new IKDrivingInformation ();
		newIKDrivingInformation.Name = "New Seat";
		//newIKDrivingInformation = IKDrivingInfo;
		IKVehiclePassengersList.Add (newIKDrivingInformation);
		updateComponent ();
	}

	public List<GameObject> getPassengerGameObjectList ()
	{
		return passengerGameObjectList;
	}

	List<GameObject> passengersInsideTrigger = new List<GameObject> ();

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		playerController playerControllerToCheck = col.GetComponent<playerController> ();
		if (playerControllerToCheck) {
			if (isEnter) {
				if (!playerControllerToCheck.canCharacterGetOnVehicles ()) {
					return;
				}
				usingDevicesSystem currentUsingDevicesSystem = col.gameObject.GetComponent<usingDevicesSystem> ();
				currentUsingDevicesSystem.setCurrentVehicle (vehicle);

				if (!usingDevicesManagerDetectedList.Contains (currentUsingDevicesSystem)) {
					usingDevicesManagerDetectedList.Add (currentUsingDevicesSystem);
				}

				if (!passengersInsideTrigger.Contains (col.gameObject)) {
					passengersInsideTrigger.Add (col.gameObject);
				}
			} else {
				usingDevicesSystem currentUsingDevicesSystem = col.gameObject.GetComponent<usingDevicesSystem> ();
				currentUsingDevicesSystem.removeCurrentVehicle (vehicle);	

				if (usingDevicesManagerDetectedList.Contains (currentUsingDevicesSystem)) {
					usingDevicesManagerDetectedList.Remove (currentUsingDevicesSystem);
				}

				if (passengersInsideTrigger.Contains (col.gameObject)) {
					passengersInsideTrigger.Remove (col.gameObject);
				}
			}
		}
	}

	public bool isVehicleFull ()
	{
		if (passengerGameObjectList.Count == IKVehiclePassengersList.Count) {
			return true;
		}
		return false;
	}

	public void checkActivateActionScreen (bool state, playerController playerControllerManager)
	{
		if (activateActionScreen) {
			if (playerControllerManager) {
				playerControllerManager.getPlayerInput ().enableOrDisableActionScreen (actionScreenName, state);
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<IKDrivingSystem> ());
		#endif
	}

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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				showIKDrivingInformationGizmo (IKVehiclePassengersList [i]);
			}

			if (useExplosionForceWhenDestroyed) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (vehicle.transform.position, explosionRadius);
			}
		}
	}

	void showIKDrivingInformationGizmo (IKDrivingInformation currentIKDrivingInfo)
	{
		if (currentIKDrivingInfo.showIKPositionsGizmo) {
			for (int i = 0; i < currentIKDrivingInfo.IKDrivingPos.Count; i++) {
				if (currentIKDrivingInfo.IKDrivingPos [i].position) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.IKDrivingPos [i].position.position, gizmoRadius);
				}
			}

			for (int i = 0; i < currentIKDrivingInfo.IKDrivingKneePos.Count; i++) {
				if (currentIKDrivingInfo.IKDrivingKneePos [i].position) {
					Gizmos.color = Color.blue;
					Gizmos.DrawSphere (currentIKDrivingInfo.IKDrivingKneePos [i].position.position, gizmoRadius);
				}
			}

			if (currentIKDrivingInfo.steerDirecion) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (currentIKDrivingInfo.steerDirecion.position, gizmoRadius);
			}

			if (currentIKDrivingInfo.headLookDirection) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (currentIKDrivingInfo.headLookDirection.position, gizmoRadius);
			}

			if (currentIKDrivingInfo.headLookPosition) {
				Gizmos.color = Color.gray;
				Gizmos.DrawSphere (currentIKDrivingInfo.headLookPosition.position, gizmoRadius);
			}

			if (vehicle) {
				if (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position, 0.1f);
					Gizmos.DrawLine (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position, 
						currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position - vehicle.transform.up * currentIKDrivingInfo.vehicleSeatInfo.getOffDistance);
				}

				if (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position, 0.1f);
					Gizmos.DrawLine (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position, 
						currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position - vehicle.transform.up * currentIKDrivingInfo.vehicleSeatInfo.getOffDistance);
				}
			}
		}

		if (currentIKDrivingInfo.vehicleSeatInfo.seatTransform) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.seatTransform.position, 0.1f);
		}
	}

	[System.Serializable]
	public class IKDrivingInformation
	{
		public string Name;
		public bool showIKPositionsGizmo = true;
		public List<IKDrivingPositions> IKDrivingPos = new List<IKDrivingPositions> ();
		public List<IKDrivingKneePositions> IKDrivingKneePos = new List<IKDrivingKneePositions> ();
		public bool useSteerDirection;
		public Transform steerDirecion;
		public bool useHeadLookDirection;
		public Transform headLookDirection;
		public bool useHeadLookPosition;
		public Transform headLookPosition;

		public seatInfo vehicleSeatInfo;

		public bool shakePlayerBodyOnCollision;
		public Transform playerBodyParent;

		public Vector3 currentSeatUpRotation;
		public Vector3 currentSeatShake;

		public Vector3 currentAngularDirection;

		public float stabilitySpeed;
		public float shakeSpeed;
		public float shakeFadeSpeed;

		public Vector3 forceDirectionMinClamp = new Vector3 (-60, -60, -60);
		public Vector3 forceDirectionMaxClamp = new Vector3 (60, 60, 60);
		public Vector3 forceDirection = new Vector3 (1, 1, 1);
	}

	[System.Serializable]
	public class IKDrivingPositions
	{
		public string Name;
		public AvatarIKGoal limb;
		public Transform position;
	}

	[System.Serializable]
	public class IKDrivingKneePositions
	{
		public string Name;
		public AvatarIKHint knee;
		public Transform position;
	}

	[System.Serializable]
	public class seatInfo
	{
		public GameObject currentPassenger;
		public bool seatIsFree = true;
		public Transform seatTransform;
		public Transform rightGetOffPosition;
		public Transform leftGetOffPosition;
		public float getOffDistance;
		public getOffSide getOffPlace;
		public bool isDriverSeat;

		public enum getOffSide
		{
			left,
			right
		}

		public bool currentlyOnFirstPerson;
	}

	[System.Serializable]
	public class passengerComponents
	{
		public GameObject currentPassenger;
		public playerController playerControllerManager;
		public usingDevicesSystem usingDevicesManager;
		public GameObject playerCameraGameObject;
		public gravitySystem playerGravityManager;
		public playerCamera playerCameraManager;
		public playerWeaponsManager playerWeaponManager;
		public jetpackSystem jetPackManager;
		public Camera mainCamera;
		public Transform mainCameraTransform;
		public Rigidbody mainRigidbody;
		public IKSystem IKManager;
		public footStepManager mainFootStepManager;
		public health healthManager;
		public playerStatesManager statesManager;
		public menuPause pauseManager;
		public bool passengerPhysicallyOnVehicle;
		public bool carryingWeaponsPrevioulsy;
		public Transform originalCameraParentTransform;
		public bool passengerOnDriverSeat;
		public vehicleHUDInfo HUDInfoManager;
		public mapSystem mapManager;

		public passengerComponents (passengerComponents newComponent)
		{
			playerControllerManager = newComponent.playerControllerManager;
			usingDevicesManager = newComponent.usingDevicesManager;
			playerCameraGameObject = newComponent.playerCameraGameObject;
			playerGravityManager = newComponent.playerGravityManager;
			playerCameraManager = newComponent.playerCameraManager;
			playerWeaponManager = newComponent.playerWeaponManager;
			jetPackManager = newComponent.jetPackManager;
			mainCamera = newComponent.mainCamera;
			mainCameraTransform = newComponent.mainCameraTransform;
			mainRigidbody = newComponent.mainRigidbody;
			IKManager = newComponent.IKManager;
			mainFootStepManager = newComponent.mainFootStepManager;
			healthManager = newComponent.healthManager;
			statesManager = newComponent.statesManager;
			pauseManager = newComponent.pauseManager;
			passengerPhysicallyOnVehicle = newComponent.passengerPhysicallyOnVehicle;
			originalCameraParentTransform = newComponent.originalCameraParentTransform;
			passengerOnDriverSeat = newComponent.passengerOnDriverSeat;
			HUDInfoManager = newComponent.HUDInfoManager;
			mapManager = newComponent.mapManager;
		}

		public passengerComponents ()
		{

		}
	}
}