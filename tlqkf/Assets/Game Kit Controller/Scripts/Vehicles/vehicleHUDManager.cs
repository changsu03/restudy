using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class vehicleHUDManager : MonoBehaviour
{
	public string vehicleName;

	public float healthAmount;
	public float boostAmount;

	public bool vehicleUseFuel;
	public float fuelAmount;

	public bool destroyed;

	public bool regenerateHealth;
	public bool constantHealthRegenerate;
	public float regenerateHealthSpeed;
	public float regenerateHealthTime;
	public float regenerateHealthAmount;

	public bool regenerateBoost;
	public bool constantBoostRegenerate;
	public float regenerateBoostSpeed;
	public float regenerateBoostTime;
	public float regenerateBoostAmount;

	public bool infiniteBoost;
	public float boostUseRate;

	public bool infiniteFuel;
	public float fuelUseRate;

	public bool regenerateFuel;
	public bool constantFuelRegenerate;
	public float regenerateFuelSpeed;
	public float regenerateFuelTime;
	public float regenerateFuelAmount;

	public GameObject gasTankGameObject;

	public bool invincible;
	public AudioClip destroyedSound;
	public AudioSource destroyedSource;
	public LayerMask layer;
	public LayerMask layerForPassengers;
	public GameObject damageParticles;
	public GameObject destroyedParticles;
	public float healthPercentageDamageParticles;
	public float extraGrabDistance;
	public Transform placeToShoot;
	public float timeToFadePieces = 3;

	public Shader destroyedMeshShader;

	public advancedSettingsClass advancedSettings = new advancedSettingsClass ();

	public bool damageObjectsOnCollision = true;
	[Range (1, 100)] public float damageMultiplierOnCollision = 1;
	public float minVelocityToDamage = 20;

	public bool useWeakSpots;
	public Slider vehicleHealth;
	public Slider vehicleBoost;
	public Slider vehicleAmmo;
	public Slider vehicleFuel;

	public bool canSetTurnOnState;
	public bool autoTurnOnWhenGetOn;

	public bool launchDriverOnCollision;
	public float minCollisionVelocityToLaunch;
	public Vector3 launchDirectionOffset;
	public float extraCollisionForce = 1.2f;
	public bool ignoreRagdollCollider;
	public bool applyDamageToDriver;
	public bool useCollisionVelocityAsDamage;
	public float collisionDamageAmount;
	public float collisionDamageMultiplier;

	public bool receiveDamageFromCollision;
	public float minVelocityToDamageCollision;
	public bool useCurrentVelocityAsDamage;
	public float defaultDamageCollision;

	public float vehicleRadius;

	public List<audioSourceInfo> audioSourceList = new List<audioSourceInfo> ();

	public bool usedByAI;

	public Transform passengersParent;

	public bool isBeingDriven;
	public bool passengersOnVehicle;

	public bool showVehicleHUD = true;
	public bool showVehicleSpeed = true;

	public bool canUseSelfDestruct;
	public bool canStopSelfDestruction;
	public float selfDestructDelay;
	public bool ejectPassengerOnSelfDestruct;
	public float ejectPassengerFoce;
	public bool getOffPassengersOnSelfDestruct;

	public bool canEjectFromVehicle;

	public bool useHornToCallFriends;
	public bool callOnlyFoundFriends;
	public float radiusToCallFriends;
	public bool useHornEvent;
	public UnityEvent hornEvent;

	public bool canUnlockCursor;
	public bool cursorUnlocked;

	public bool useEventsOnStateChanged;
	public UnityEvent eventOnGetOn;
	public UnityEvent eventOnGetOff;
	public UnityEvent eventOnDestroyed;

	public bool useJumpPlatformEvents = true;
	public eventParameters.eventToCallWithVector3 jumpPlatformEvent;
	public eventParameters.eventToCallWithVector3 jumpPlatformParableEvent;
	public eventParameters.eventToCallWithAmount setNewJumpPowerEvent;
	public UnityEvent setOriginalJumpPowerEvent;

	public UnityEvent passengerGettingOnOffEvent;
	public UnityEvent changeVehicleStateEvent;

	public float vehicleExplosionForce = 500;
	public float vehicleExplosionRadius = 50;
	public ForceMode vehicleExplosionForceMode = ForceMode.Impulse;

	List<Material> rendererParts = new List<Material> ();
	List<GameObject> projectilesReceived = new List<GameObject> ();
	public List<ParticleSystem> fireParticles = new List<ParticleSystem> ();
	public List<Collider> colliderParts = new List<Collider> ();

	List<GameObject> previousPassengerGameObjectList = new List<GameObject> ();

	float lastDamageTime;
	float maxhealthAmount;
	float auxHealthAmount;

	float lastBoostTime;
	float maxBoostAmount;
	float auxPowerAmount;

	float lastFuelTime;
	float maxFuelAmount;
	float auxFuelAmount;

	bool vehicleDisabled;
	Text ammoAmountText;
	Text weaponNameText;
	Text currentSpeed;

	public IKDrivingSystem IKDrivingManager;
	public vehicleWeaponSystem weaponsManager;
	public Rigidbody mainRigidbody;
	public damageInScreen damageInScreenManager;

	public mapObjectInformation mapInformationManager;
	public vehicleCameraController vehicleCameraManager;
	public useInventoryObject gasTankManager;
	public vehicleGravityControl vehicleGravitymanager;
	public vehicleInterface vehicleInterfaceManager;

	Coroutine CollidersStateCoroutine;

	Coroutine damageOverTimeCoroutine;

	decalManager impactDecalManager;
	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;
	public bool useImpactSurface;

	bool selfDestructionActivated;
	Coroutine selfDestructionCoroutine;

	public vehicleHUDInfo.vehicleHUDElements currentHUDElements;
	vehicleHUDInfo currentVehicleHUDInfo;

	audioClipBip selfDestructAudioClipBipManager;

	int lastWeakSpotIndex = -1;

	//Gizmo variables
	public bool showAdvancedSettings;

	public List<vehicleDamageReceiver> vehicleDamageReceiverList = new List<vehicleDamageReceiver> ();

	public List<Renderer> vehicleRendererList = new List<Renderer> ();

	void Start ()
	{
		//get the max amount of health and boost
		maxhealthAmount = healthAmount;
		maxBoostAmount = boostAmount;
		maxFuelAmount = fuelAmount;

		//like in the player, store the max amount of health and boost in two auxiliars varaibles, used by the pick ups to check if the vehicle uses one or more of them
		auxPowerAmount = maxBoostAmount;
		auxHealthAmount = maxhealthAmount;
		auxFuelAmount = maxFuelAmount;

		//get the damage particles of the vehicle
		if (damageParticles) {
			for (int i = 0; i < fireParticles.Count; i++) {
				fireParticles [i].gameObject.SetActive (false);
			}
		}

		if (gasTankGameObject) {
			setGasTankState (false);
		}

		if (usedByAI) {
			activaAIVehicle ();
		}
	}

	void Update ()
	{
		if (isBeingDriven) {
			//get the current values of health and boost of the vehicle, checking if they are regenerative or not
			manageBarInfo ("health", regenerateHealth, constantHealthRegenerate, regenerateHealthSpeed, regenerateHealthTime, regenerateHealthAmount, 
				vehicleHealth, healthAmount, maxhealthAmount, lastDamageTime);

			manageBarInfo ("boost", regenerateBoost, constantBoostRegenerate, regenerateBoostSpeed, regenerateBoostTime, regenerateBoostAmount,
				vehicleBoost, boostAmount, maxBoostAmount, lastBoostTime);

			manageBarInfo ("fuel", regenerateFuel, constantFuelRegenerate, regenerateFuelSpeed, regenerateFuelTime, regenerateFuelAmount,
				vehicleFuel, fuelAmount, maxFuelAmount, lastFuelTime);
		}

		//clear the list which contains the projectiles received by the vehicle
		if (Time.time > lastDamageTime + 3) {
			projectilesReceived.Clear ();
		}

		//if the vehicle is destroyed, when destroyed time reachs 0, all the renderer parts of the vehicle are vanished, setting their alpha color value to 0
		if (destroyed && !vehicleDisabled) {
			if (timeToFadePieces > 0) {
				timeToFadePieces -= Time.deltaTime;
			}
			if (timeToFadePieces <= 0) {
				int piecesAmountFade = 0;
				for (int i = 0; i < rendererParts.Count; i++) {
					Color alpha = rendererParts [i].color;
					alpha.a -= Time.deltaTime / 5;
					rendererParts [i].color = alpha;
					if (alpha.a <= 0) {
						piecesAmountFade++;
					}
				}
				if (piecesAmountFade == rendererParts.Count) {
					IKDrivingManager.destroyVehicle ();
					vehicleDisabled = true;
					return;
				}
			}
		}
	}

	public void setUnlockCursorState (bool state)
	{
		if (vehicleCameraManager.currentState.canUnlockCursor) {
			cursorUnlocked = state;
			if (!IKDrivingManager.setUnlockCursorState (cursorUnlocked)) {
				cursorUnlocked = false;
			}
		}
	}

	public void disableUnlockedCursor ()
	{
		if (canUnlockCursor) {
			cursorUnlocked = false;
			if (IKDrivingManager) {
				IKDrivingManager.setUnlockCursorState (cursorUnlocked);
			}
		}
	}

	public void activateSelfDestruction ()
	{
		if (!selfDestructionActivated) {
			selfDestructionCoroutine = StartCoroutine (selfDestructVehicle ());
		} else {
			if (selfDestructionCoroutine != null) {
				StopCoroutine (selfDestructionCoroutine);
				selfDestructionActivated = false;
				if (selfDestructAudioClipBipManager) {
					selfDestructAudioClipBipManager.disableBip ();
				}
				if (ejectPassengerOnSelfDestruct) {

				}
			}
		}
	}

	IEnumerator selfDestructVehicle ()
	{
		selfDestructionActivated = true;
		if (selfDestructDelay > 0) {
			selfDestructAudioClipBipManager = GetComponentInChildren<audioClipBip> ();
			if (selfDestructAudioClipBipManager) {
				selfDestructAudioClipBipManager.increasePlayTime (selfDestructDelay);
			}
		}

		if (ejectPassengerOnSelfDestruct) {
			ejectFromVehicle ();

		} else {
			if (getOffPassengersOnSelfDestruct) {
				List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

				for (int i = 0; i < passengerGameObjectList.Count; i++) {
					usingDevicesSystem usingDevicesManager = passengerGameObjectList [i].GetComponent<usingDevicesSystem> ();
					if (usingDevicesManager) {
						usingDevicesManager.useDevice ();
					}
				}
			}
		}

		yield return new WaitForSeconds (selfDestructDelay);

		destroyVehicle ();
		yield return null;
	}

	public void callEventOnGetOff ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnGetOff.GetPersistentEventCount () > 0) {
				eventOnGetOff.Invoke ();
			}
		}
	}

	public void callEventOnGetOn ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnGetOn.GetPersistentEventCount () > 0) {
				eventOnGetOn.Invoke ();
			}
		}
	}

	public void callEventOnDestroyed ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnDestroyed.GetPersistentEventCount () > 0) {
				eventOnDestroyed.Invoke ();
			}
		}
	}

	public void ejectFromVehicle ()
	{
		callEventOnGetOff ();

		disableUnlockedCursor ();

		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			previousPassengerGameObjectList.Add (passengerGameObjectList [i]);
		}

		checkIgnorPassengersCollidersState ();

		IKDrivingManager.ejectVehiclePassengersOnSelfDestruct (ejectPassengerFoce);

		passengerGettingOnOffEvent.Invoke ();

		passengersOnVehicle = false;

		if (isBeingDriven) {
			isBeingDriven = false;
			changeVehicleStateEvent.Invoke ();
		}
	}

	public void ejectFromVehicleWithFreeFloatingMode ()
	{
		IKDrivingManager.setActivateFreeFloatingModeOnEjectEnabledState (true);
		ejectFromVehicle ();
	}

	public void changeIgnorePassengerCollidersState (bool state)
	{
		List<Collider> passengerColliderList = new List<Collider> ();

		for (int j = 0; j < previousPassengerGameObjectList.Count; j++) {
			passengerColliderList.Add (previousPassengerGameObjectList [j].GetComponent<Collider> ());
		}

		foreach (Collider currentCollider in colliderParts) {
			//check that the current renderer is not the player or any object inside him
			if (previousPassengerGameObjectList.Count == 0 || (!previousPassengerGameObjectList.Contains (currentCollider.gameObject) &&
			    checkNotChildOfPassenger (previousPassengerGameObjectList, currentCollider.gameObject.transform))) {
				//ignore collisions with the player
				for (int j = 0; j < passengerColliderList.Count; j++) {
					Physics.IgnoreCollision (passengerColliderList [j], currentCollider, state);
				}
			}
		}
	}


	public void checkIgnorPassengersCollidersState ()
	{
		if (CollidersStateCoroutine != null) {
			StopCoroutine (CollidersStateCoroutine);
		}
		CollidersStateCoroutine = StartCoroutine (changeIgnorePassengerCollidersStateCoroutine ());
	}

	IEnumerator changeIgnorePassengerCollidersStateCoroutine ()
	{
		changeIgnorePassengerCollidersState (true);
		yield return new WaitForSeconds (1);
		changeIgnorePassengerCollidersState (false);
		yield return null;
	}

	public void manualStartOrStopVehicle (bool state, GameObject passenger)
	{
		if (state) {
			callEventOnGetOn ();
		} else {
			callEventOnGetOff ();
		}

		disableUnlockedCursor ();

		//change the driving value
		Vector3 currentNormal = Vector3.up;
		if (vehicleGravitymanager) {
			currentNormal = vehicleGravitymanager.getCurrentNormal ();
		}
		IKDrivingManager.startOrStopVehicle (passenger, passengersParent, currentNormal, passenger.transform.position);
		//send the message to the vehicle movement component, to enable or disable the driving state
		isBeingDriven = state;
		passengersOnVehicle = false;

		changeVehicleStateEvent.Invoke ();
	}

	GameObject currentPassenger;

	public void setCurrentPassenger (GameObject passenger)
	{
		currentPassenger = passenger;
	}

	//function called when the player press the use device button
	public void activateDevice ()
	{
		if (IKDrivingManager.isVehicleFull () && !IKDrivingManager.getPassengerGameObjectList ().Contains (currentPassenger)) {
			return;
		}

		disableUnlockedCursor ();

		Vector3 nextPlayerPosition = Vector3.zero;
		//if the vehicle is being driven, check if the player can get off
		nextPlayerPosition = IKDrivingManager.getPassengerGetOffPosition (currentPassenger);
		//if the vehicle is not being driven (so the player is going to get on) or there is no obstacles to get off
		if (nextPlayerPosition != -Vector3.one) {
			//change the driving value

			//print (currentPassenger.name);
			Vector3 currentNormal = Vector3.up;
			if (vehicleGravitymanager) {
				currentNormal = vehicleGravitymanager.getCurrentNormal ();
			}
			bool canBeDriven = IKDrivingManager.startOrStopVehicle (currentPassenger, passengersParent, currentNormal, nextPlayerPosition);

			if (IKDrivingManager.getPassengerGameObjectList ().Count > 0) {
				passengersOnVehicle = true;
			} else {
				passengersOnVehicle = false;
			}

			passengerGettingOnOffEvent.Invoke ();

			if (canBeDriven) {
				isBeingDriven = !isBeingDriven;
				//send the message to the vehicle movement component, like car controller or motorbike controller
				changeVehicleStateEvent.Invoke ();
			}

			if (isBeingDriven) {
				callEventOnGetOn ();
			} else {
				callEventOnGetOff ();
			}
		}
	}

	public void activaAIVehicle ()
	{
		isBeingDriven = !isBeingDriven;

		//send the message to the vehicle movement component, like car controller or motorbike controller
		changeVehicleStateEvent.Invoke ();
	}

	//if any collider in the vehicle collides, then
	void OnCollisionEnter (Collision collision)
	{
		//check that the collision is not with the player
		if (collision.contacts.Length > 0) {
			if (passengersOnVehicle && launchDriverOnCollision) {
				if (collision.relativeVelocity.magnitude > minCollisionVelocityToLaunch) {

					Vector3 collisionDirectionOffset = Vector3.zero;
					if (launchDirectionOffset != Vector3.zero) {
						collisionDirectionOffset = transform.right * collisionDirectionOffset.x +
						transform.up * collisionDirectionOffset.y +
						transform.forward * collisionDirectionOffset.z;
					}
					Vector3 collisionDirection = (collision.contacts [0].point + collisionDirectionOffset - transform.position).normalized;
					if (extraCollisionForce > 1) {
						collisionDirection *= extraCollisionForce;
					}
					launchCharacterOnVehicleCollision (collisionDirection, collision.relativeVelocity.magnitude);
				}
			}

			if (receiveDamageFromCollision) {
				print ("collision" + collision.relativeVelocity.magnitude);
				if (collision.relativeVelocity.magnitude > minVelocityToDamageCollision) {
					Vector3 collisionDirection = (collision.contacts [0].point - transform.position).normalized;
					float collisionDamage = defaultDamageCollision;
					if (useCurrentVelocityAsDamage) {
						collisionDamage = collision.relativeVelocity.magnitude;
					} 

					if (collisionDamageMultiplier > 0) {
						collisionDamage *= collisionDamageMultiplier;
					}

					print (collisionDamage);
					applyDamage.checkHealth (gameObject, gameObject, collisionDamage, collisionDirection, collision.contacts [0].point, gameObject, false, true);
				}
			}

			if (damageObjectsOnCollision) {
				if (mainRigidbody.velocity.magnitude > minVelocityToDamage) {
					//if the vehicle hits another vehicle, apply damage to both of them according to the velocity at the impact
					applyDamage.checkHealth (gameObject, collision.collider.gameObject, 
						collision.relativeVelocity.magnitude * damageMultiplierOnCollision, 
						collision.contacts [0].normal, collision.contacts [0].point, gameObject, false, true);
				}
			}

			if (passengersOnVehicle) {
				if (IKDrivingManager.addCollisionForceDirectionToPassengers) {
					IKDrivingManager.setCollisionForceDirectionToPassengers ((collision.contacts [0].point - transform.position).normalized * collision.relativeVelocity.magnitude);
				}
			}
		}
	}

	public void launchCharacterOnVehicleCollision (Vector3 collisionDirection, float collisionVelocity)
	{
		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		GameObject currentDriver = IKDrivingManager.getcurrentDriver ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			GameObject currentPassengerToCheck = passengerGameObjectList [i];
			if (currentDriver != currentPassengerToCheck || (currentDriver == currentPassengerToCheck && !IKDrivingManager.getCanBeDrivenRemotelyValue ())) {
				manualStartOrStopVehicle (false, currentPassenger);
				if (currentPassenger) {
					currentPassenger.GetComponent<Collider> ().isTrigger = true;

					usingDevicesSystem currentUsingDevicesSystem = currentPassenger.GetComponent<usingDevicesSystem> ();
					currentUsingDevicesSystem.removeDeviceFromListExternalCall (gameObject);
					currentUsingDevicesSystem.removeCurrentVehicle (gameObject);
					currentUsingDevicesSystem.disableIcon ();

					if (ignoreRagdollCollider) {
						currentPassenger.SendMessage ("ignoreCollisionWithBodyColliderList", colliderParts, SendMessageOptions.DontRequireReceiver);
					}

					currentPassenger.SendMessage ("pushFullCharacter", collisionDirection, SendMessageOptions.DontRequireReceiver);

					if (applyDamageToDriver) {
						if (!useCollisionVelocityAsDamage) {
							collisionVelocity = collisionDamageAmount;
						}
					
						applyDamage.checkHealth (gameObject, currentPassenger, collisionVelocity, collisionDirection, currentPassenger.transform.position, gameObject, false, true);
					}
				}
			}
		}
	}

	public bool checkIfDetectSurfaceBelongToVehicle (Collider surfaceFound)
	{
		if (colliderParts.Contains (surfaceFound)) {
			return true;
		} else {
			return false;
		}
	}

	//the player has used a pickup while he is driving, so the health is added in the vehicle
	public void getHealth (float amount)
	{
		if (!destroyed) {
			//increase the health amount
			healthAmount += amount;
			//check that the current health is not higher than the max value
			if (healthAmount >= maxhealthAmount) {
				healthAmount = maxhealthAmount;
			}
			//set the value in the slider of the HUD
			updateHealthSlider (healthAmount);
			//check the current health amount to stop or reduce the damage particles
			changeDamageParticlesValue (false, amount);
			if (damageInScreenManager) {
				damageInScreenManager.showScreenInfo (amount, false, Vector3.zero, healthAmount);
			}
			auxHealthAmount = healthAmount;
		}
	}

	public void updateHealthSlider (float value)
	{
		if (vehicleHealth) {
			vehicleHealth.value = value;
		}
	}

	//the player has used a pickup while he is driving, so the boost is added in the vehicle
	public void getEnergy (float amount)
	{
		if (!destroyed) {
			//increase the boost amount
			boostAmount += amount;
			//check that the current boost is not higher than the max value
			if (boostAmount >= maxBoostAmount) {
				boostAmount = maxBoostAmount;
			}
			//set the value in the slider of the HUD
			updateEnergySlider (boostAmount);
			auxPowerAmount = boostAmount;
		}
	}

	public void removeEnergy (float amount)
	{
		//increase the boost amount
		boostAmount -= amount;
		//check that the current boost is not higher than the max value
		if (boostAmount < 0) {
			boostAmount = 0;
		}
		//set the value in the slider of the HUD
		updateEnergySlider (boostAmount);
		auxPowerAmount = boostAmount;
	}

	public void updateEnergySlider (float value)
	{
		if (vehicleBoost) {
			vehicleBoost.value = value;
		}
	}

	public void getFuel (float amount)
	{
		if (!destroyed) {
			fuelAmount += amount;
			if (fuelAmount > maxFuelAmount) {
				fuelAmount = maxFuelAmount;
				setGasTankState (false);
			}
			updateFuelSlider (fuelAmount);
			auxFuelAmount = fuelAmount;
		}
	}

	public void updateFuelSlider (float value)
	{
		if (vehicleFuel) {
			vehicleFuel.value = value;
		}
	}

	public void setGasTankState (bool state)
	{
		gasTankManager.enableOrDisableTrigger (state);
	}

	public void refillFuelTank ()
	{
		int fuelAmountToRefill = gasTankManager.getCurrentAmountUsed ();
		getFuel (fuelAmountToRefill);
	}

	//the player has used a pickup while he is driving, so the ammo is added in the vehicle
	public void getAmmo (string ammoName, int amount)
	{
		if (weaponsManager && weaponsManager.weaponsEnabled) {
			weaponsManager.getAmmo (ammoName, amount);
		}
	}

	//get the value of the current speed in the vehicle
	public void getSpeed (float speed, float maxSpeed)
	{
		if (currentSpeed) {
			currentSpeed.text = speed.ToString ("0") + " / " + maxSpeed.ToString ();
		}
	}

	//if the health or the boost are regenerative, increase the values according to the last time damaged or used
	public void manageBarInfo (string sliderType, bool regenerate, bool constantRegenerate, float regenerateSpeed, float regenerateTime, float regenerateAmount,
	                           Slider bar, float barAmount, float maxAmount, float lastTime)
	{
		if (regenerate && !destroyed) {
			if (constantRegenerate) {
				if (regenerateSpeed > 0 && barAmount < maxAmount) {
					if (Time.time > lastTime + regenerateTime) {
						if (sliderType == "health") {
							getHealth (regenerateSpeed * Time.deltaTime);
						} else if (sliderType == "boost") {
							getEnergy (regenerateSpeed * Time.deltaTime);
						} else if (sliderType == "fuel") {
							getFuel (regenerateSpeed * Time.deltaTime);
						}
					}
				}
			} else {
				if (barAmount < maxAmount) {
					if (Time.time > lastTime + regenerateTime) {
						if (sliderType == "health") {
							getHealth (regenerateAmount);
							lastDamageTime = Time.time;
						} else if (sliderType == "boost") {
							getEnergy (regenerateAmount);
							lastBoostTime = Time.time;
						} else if (sliderType == "fuel") {
							getFuel (regenerateAmount);
							lastFuelTime = Time.time;
						}
					}
				}
			}
		}
	}

	//use the boost in the vehicle, checking the current amount of energy in it
	public bool useBoost (bool moving)
	{
		bool boostAvaliable = false;
		//the vehicle is moving so 
		if (moving) {
			if (infiniteBoost) {
				boostAmount = maxBoostAmount;
				boostAvaliable = true;
			} else if (boostAmount > 0) {
				//reduce the boost amount and return a true value
				boostAmount -= Time.deltaTime * boostUseRate;
				auxPowerAmount = boostAmount;
				boostAvaliable = true;
			}
			if (boostAmount < 0) {
				boostAmount = 0;
			}
			if (boostAvaliable) {
				updateEnergySlider (boostAmount);
				lastBoostTime = Time.time;
			}
		}
		return boostAvaliable;
	}

	public bool useFuel ()
	{
		if (!vehicleUseFuel) {
			return true;
		}

		bool fuelAvaliable = false;
		if (infiniteFuel) {
			fuelAmount = maxFuelAmount;
			fuelAvaliable = true;
		} else if (fuelAmount > 0) {
			fuelAmount -= Time.deltaTime * fuelUseRate;
			auxFuelAmount = fuelAmount;
			fuelAvaliable = true;
			if (fuelAmount < 0) {
				fuelAmount = 0;
			}
			if (fuelAmount < maxFuelAmount) {
				setGasTankState (true);
			}
		}

		if (fuelAvaliable) {
			updateFuelSlider (fuelAmount);
			lastFuelTime = Time.time;
		}
		return fuelAvaliable;
	}

	public void removeFuel (float amount)
	{
		if (!vehicleUseFuel) {
			return;
		}

		if (infiniteFuel) {
			fuelAmount = maxFuelAmount;
		} else if (fuelAmount > 0) {
			fuelAmount -= amount;
			auxFuelAmount = fuelAmount;
			if (fuelAmount < 0) {
				fuelAmount = 0;
			}
			if (fuelAmount < maxFuelAmount) {
				setGasTankState (true);
			}
		}
		updateFuelSlider (fuelAmount);
	}

	public void setFuelInfo ()
	{
		if (vehicleFuel != null) {
			vehicleFuel.maxValue = maxFuelAmount;
			vehicleFuel.value = fuelAmount;
		}
	}

	//when the current weapon is changed for another, get the current name, ammo per clip and clip size of that weapon
	public void setWeaponName (string name, int ammoPerClip, int clipSize)
	{
		if (weaponNameText) {
			weaponNameText.text = name;
			vehicleAmmo.maxValue = ammoPerClip;
			vehicleAmmo.value = clipSize;
		}
	}

	//the player is shooting while he is driving, so use ammo of the vehicle weapon
	public void useAmmo (int clipSize, string remainAmmo)
	{
		if (ammoAmountText) {
			ammoAmountText.text = clipSize.ToString () + "/" + remainAmmo;
			vehicleAmmo.value = clipSize;
		}
	}

	//the vehicle is receiving damage, getting the current damage amount, the direction of the projectile, its hit position, the object that fired it and if the damage is applied only
	//one time, like a bullet, or constantly like a laser
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, bool damageConstant, bool searchClosestWeakSpot)
	{
		if (!damageConstant) {
			//if the projectile is not a laser, store it in a list
			//this is done like this because you can add as many colliders (box or mesh) as you want (according to the vehicle meshes), 
			//which are used to check the damage received by every vehicle, so like this the damage detection is really accurated. 
			//For example, if you shoot a grenade to a car, every collider will receive the explosion, but the vehicle will only be damaged once, with the correct amount.
			//in this case the projectile has not produced damage yet, so it is stored in the list and in the below code the damage is applied. 
			//This is used for bullets for example, which make damage only in one position
			if (!projectilesReceived.Contains (projectile)) {
				projectilesReceived.Add (projectile);
			} 
			//in this case the projectile has been added to the list previously, it means that the projectile has already applied damage to the vehicle, 
			//so it can't damaged the vehicle twice. This is used for grenades for example, which make a damage inside a radius
			else {
				return;
			}
		}

		//if any elememnt in the list of current projectiles received is not longer exits, remove it from the list
		for (int i = 0; i < projectilesReceived.Count; i++) {
			if (!projectilesReceived [i]) {
				projectilesReceived.RemoveAt (i);
			}
		}

		//if the object is not dead, invincible or its health is zero, exit
		if (invincible || destroyed || amount <= 0) {
			return;
		}

		if (vehicleCameraManager.shakeSettings.useDamageShake && isBeingDriven) {
			vehicleCameraManager.setDamageCameraShake ();
		}

		damageReceiverInfo currentWeakSpot = new damageReceiverInfo ();
		if (useWeakSpots && searchClosestWeakSpot) {
			int weakSpotIndex = getClosesWeakSpotIndex (damagePos);
			lastWeakSpotIndex = weakSpotIndex;
			if (amount < healthAmount) {
				currentWeakSpot = advancedSettings.damageReceiverList [weakSpotIndex];
				if (advancedSettings.damageReceiverList [weakSpotIndex].killedWithOneShoot) {
					if (advancedSettings.damageReceiverList [weakSpotIndex].needMinValueToBeKilled) {
						if (advancedSettings.damageReceiverList [weakSpotIndex].minValueToBeKilled < amount) {
							amount = healthAmount;
						}
					} else {
						amount = healthAmount;
					}
				}
			}

			if (currentWeakSpot.useHealthAmountOnSpot && !currentWeakSpot.healthAmountOnSpotEmtpy) {
				currentWeakSpot.healhtAmountOnSpot -= amount;
				if (currentWeakSpot.healhtAmountOnSpot <= 0) {
					currentWeakSpot.eventOnEmtpyHealthAmountOnSpot.Invoke ();
					currentWeakSpot.healthAmountOnSpotEmtpy = true;
					if (currentWeakSpot.killCharacterOnEmtpyHealthAmountOnSpot) {
						amount = healthAmount;
					}
				}
			}
		}

		if (amount > healthAmount) {
			amount = healthAmount;
		}
			
		//decrease the health amount
		healthAmount -= amount;
		auxHealthAmount = healthAmount;

		//if the player is driving this vehicle, set the value in the slider
		if (isBeingDriven) {
			updateHealthSlider (healthAmount);
		}

		if (damageInScreenManager) {
			damageInScreenManager.showScreenInfo (amount, true, fromDirection, healthAmount);
		}

		//increase the damage particles values
		changeDamageParticlesValue (true, amount);
		//set the last time damage
		lastDamageTime = Time.time;
		//if the health reachs 0, call the dead function
		if (healthAmount <= 0) {
			healthAmount = 0;
			destroyed = true;
			destroyVehicle (damagePos);
		}

		if (useWeakSpots && lastWeakSpotIndex > -1 && searchClosestWeakSpot) {
			bool callFunction = false;
			currentWeakSpot = advancedSettings.damageReceiverList [lastWeakSpotIndex];
			if (currentWeakSpot.sendFunctionWhenDamage) {
				callFunction = true;
			}
			if (currentWeakSpot.sendFunctionWhenDie && destroyed) {
				callFunction = true;
			}
			//print (advancedSettings.weakSpots [lastWeakSpotIndex].name +" " +callFunction +" "+ dead);
			if (callFunction) {
				if (currentWeakSpot.damageFunction.GetPersistentEventCount () > 0) {
					currentWeakSpot.damageFunction.Invoke ();
				}
			}
			lastWeakSpotIndex = -1;
		}
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath)
	{
		stopDamageOverTime ();
		damageOverTimeCoroutine = StartCoroutine (setDamageTargetOverTimeStateCoroutine (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath));
	}

	IEnumerator setDamageTargetOverTimeStateCoroutine (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath)
	{
		yield return new WaitForSeconds (damageOverTimeDelay);

		float lastTimeDamaged = Time.time;
		float lastTimeDuration = lastTimeDamaged;
		while (Time.time < lastTimeDuration + damageOverTimeDuration || (!destroyed && damageOverTimeToDeath)) {
			if (Time.time > lastTimeDamaged + damageOverTimeRate) {
				lastTimeDamaged = Time.time;
				setDamage (damageOverTimeAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, true, false);
			}
			yield return null;
		}
	}

	public void stopDamageOverTime ()
	{
		if (damageOverTimeCoroutine != null) {
			StopCoroutine (damageOverTimeCoroutine);
		}
	}

	public int getClosesWeakSpotIndex (Vector3 collisionPosition)
	{
		float distance = Mathf.Infinity;
		int index = -1;
		for (int i = 0; i < advancedSettings.damageReceiverList.Count; i++) {
			float currentDistance = GKC_Utils.distance (collisionPosition, advancedSettings.damageReceiverList [i].spotTransform.position);
			if (currentDistance < distance) {
				distance = currentDistance;
				index = i;
			}
		}
		if (index > -1) {
			if (advancedSettings.showGizmo) {
				//print (advancedSettings.damageReceiverList [index].name);
			}
		}
		return index;
	}

	//the vehicle health is 0, so the vehicle is destroyed
	public void destroyVehicle (Vector3 pos)
	{
		callEventOnDestroyed ();

		//instantiated an explosiotn particles
		GameObject destroyedParticlesClone = (GameObject)Instantiate (destroyedParticles, transform.position, transform.rotation);
		destroyedParticlesClone.transform.SetParent (transform);
		destroyedSource.PlayOneShot (destroyedSound);

		//set the velocity of the vehicle to zero
		mainRigidbody.velocity = Vector3.zero;
		mainRigidbody.isKinematic = true;

		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		List<Collider> passengerColliderList = new List<Collider> ();

		for (int j = 0; j < passengerGameObjectList.Count; j++) {
			passengerColliderList.Add (passengerGameObjectList [j].GetComponent<Collider> ());
		}

		Collider colliderPart;

		//any other object with a collider but with out renderer, is disabled
		for (int i = 0; i < colliderParts.Count; i++) {
			colliderPart = colliderParts [i];
			if (passengerGameObjectList.Count == 0 || (!passengerGameObjectList.Contains (colliderPart.gameObject) &&
			    checkNotChildOfPassenger (passengerGameObjectList, colliderPart.gameObject.transform))) {
				if (!colliderPart.GetComponent<Renderer> ()) {
					colliderPart.enabled = false;
				}
			}
		}

		if (destroyedMeshShader == null) {
			destroyedMeshShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
		}

		//get every renderer component if the car
		for (int i = 0; i < vehicleRendererList.Count; i++) {
			Renderer currentRenderer = vehicleRendererList [i];
			GameObject currentVehiclePiece = currentRenderer.gameObject;

			//check that the current renderer is not the player or any object inside him
			if (passengerGameObjectList.Count == 0 || (!passengerGameObjectList.Contains (currentVehiclePiece) && checkNotChildOfPassenger (passengerGameObjectList, currentVehiclePiece.transform))) {

				if (currentRenderer && currentVehiclePiece.layer != LayerMask.NameToLayer ("Scanner")) {
					if (currentRenderer.enabled) {
						
						//for every renderer object, change every shader in it for a transparent shader 
						for (int j = 0; j < currentRenderer.materials.Length; j++) {
							currentRenderer.materials [j].shader = destroyedMeshShader;
							rendererParts.Add (currentRenderer.materials [j]);
						}

						//set the layer ignore raycast to them
						currentVehiclePiece.layer = LayerMask.NameToLayer ("Ignore Raycast");

						Rigidbody currentRigidbody = currentVehiclePiece.GetComponent<Rigidbody> ();

						//add rigidbody and box collider to them
						if (!currentRigidbody) {
							currentRigidbody = currentVehiclePiece.AddComponent<Rigidbody> ();
						}

						Collider currentCollider = currentVehiclePiece.GetComponent<Collider> ();
						if (!currentCollider) {
							currentCollider = currentVehiclePiece.AddComponent<BoxCollider> ();
						}

						//apply explosion force
						currentRigidbody.AddExplosionForce (vehicleExplosionForce, pos, vehicleExplosionRadius, 3, vehicleExplosionForceMode);

						//ignore collisions with the player
						for (int j = 0; j < passengerColliderList.Count; j++) {
							Physics.IgnoreCollision (passengerColliderList [j], currentCollider);
						}
					}
				}
			}
		}
			
		if (vehicleInterfaceManager) {
			vehicleInterfaceManager.setInterfaceCanvasState (false);
		}

		//stop the IK system in the player
		IKDrivingManager.disableVehicle ();
		if (mapInformationManager) {
			mapInformationManager.removeMapObject ();
		}
	}

	public void destroyVehicleAtOnce ()
	{
		Destroy (IKDrivingManager.gameObject);
		Destroy (vehicleCameraManager.gameObject);
		Destroy (gameObject);
	}

	public bool checkNotChildOfPassenger (List<GameObject> passengerGameObjectList, Transform objectToCheck)
	{
		for (int j = 0; j < passengerGameObjectList.Count; j++) {
			if (objectToCheck.IsChildOf (passengerGameObjectList [j].transform)) {
				return false;
			}
		}

		return true;
	}

	//this function is called when the vehicle receives damage, to enable a fire and smoke particles system to show serious damage in the vehicle
	void changeDamageParticlesValue (bool damaging, float amount)
	{
		//if the vehicle has a damage particles object
		if (damageParticles) {
			bool activate = false;
			bool activeLoop;
			//if the health is 0, disable the damage particles
			if (healthAmount <= 0) {
				for (int i = 0; i < fireParticles.Count; i++) {
					fireParticles [i].gameObject.SetActive (false);
				}
				return;
			}
			//if the current vehicle health is lower than certain %, the damage particles are enabled 
			bool lowHealth = false;
			if (healthAmount <= maxhealthAmount / (100 / healthPercentageDamageParticles)) {
				activate = true;
				activeLoop = true;
				lowHealth = true;
			} else {
				activeLoop = false;
			}
			for (int i = 0; i < fireParticles.Count; i++) {
				//enable the particles
				if (activate) {
					if (!fireParticles [i].isPlaying) {
						fireParticles [i].Play ();
					}
					fireParticles [i].gameObject.SetActive (true);
				}
				//enable or disable their loop, if the particles are enabled, and the health is higher that the above %, then the particles loop is disabled, because the car 
				//has a better health
				if (activeLoop) {
					fireParticles [i].loop = true;
				} else {
					fireParticles [i].loop = false;
				}
				//if the health Percentage Damage Particles is reached, then increase or decrease its size according to if the vehicle is being damaged or receiving health
				if (lowHealth) {
					if (damaging) { 
						fireParticles [i].startSize += amount * 0.05f;
					} else {
						fireParticles [i].startSize -= amount * 0.05f;
					}
				}
			}
		}
	}

	//when the player gets on the vehicle, the IK driving system sends every slider and text component of the vehicles HUD, to update every value and show them to the player
	public void getHUDBars (vehicleHUDInfo newVehicleHUDInfo, vehicleHUDInfo.vehicleHUDElements hudElements, bool drivingState)
	{
		currentVehicleHUDInfo = newVehicleHUDInfo;

		//if (drivingState) {
		currentHUDElements = hudElements;
		vehicleHealth = currentHUDElements.vehicleHealth;
		vehicleBoost = currentHUDElements.vehicleBoost;
		vehicleFuel = currentHUDElements.vehicleFuel;
		vehicleAmmo = currentHUDElements.vehicleAmmo;
		vehicleHealth.value = healthAmount;
		vehicleBoost.value = boostAmount;
		ammoAmountText = currentHUDElements.ammoInfo;
		weaponNameText = currentHUDElements.weaponName;
		currentSpeed = currentHUDElements.currentSpeed;
		//}

		//check also if the vehicle has a weapon system attached, to enable or disable it
		//if the vehicle has not a weapon system, the weapon info of the HUD is disabled
		if (weaponsManager && weaponsManager.weaponsEnabled) {
			weaponsManager.changeWeaponState (drivingState);
			currentHUDElements.ammoContent.SetActive (true);
			currentHUDElements.vehicleCursor.SetActive (true);
		} else {
			currentHUDElements.ammoContent.SetActive (false);
			currentHUDElements.vehicleCursor.SetActive (false);
		}

		if (invincible) {
			currentHUDElements.healthContent.SetActive (false);
		} else {
			currentHUDElements.healthContent.SetActive (true);
		}

		if (infiniteFuel) {
			currentHUDElements.energyContent.SetActive (false);
		} else {
			currentHUDElements.energyContent.SetActive (true);
		}

		if (vehicleUseFuel) {
			currentHUDElements.fuelContent.SetActive (true);
			setFuelInfo ();
		} else {
			currentHUDElements.fuelContent.SetActive (false);
		}

		if (showVehicleSpeed) {
			currentHUDElements.speedContent.SetActive (drivingState);
		}
	}

	public vehicleHUDInfo.vehicleHUDElements getCurrentHUDElements ()
	{
		return currentHUDElements;
	}

	public vehicleHUDInfo getCurrentVehicleHUDInfo ()
	{
		return currentVehicleHUDInfo;
	}

	public void setWeaponState (bool state)
	{
		if (weaponsManager && weaponsManager.weaponsEnabled) {
			weaponsManager.changeWeaponState (state);

			if (currentHUDElements != null) {
				if (currentHUDElements.ammoContent) {
					currentHUDElements.ammoContent.SetActive (state);
					currentHUDElements.vehicleCursor.SetActive (state);
				}
			}
		}
	}

	public bool showVehicleHUDActive ()
	{
		return showVehicleHUD;
	}
		
	//use a jump platform
	public void useJumpPlatform (Vector3 direction)
	{
		if (useJumpPlatformEvents) {
			jumpPlatformEvent.Invoke (direction);
		}
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		if (useJumpPlatformEvents) {
			jumpPlatformParableEvent.Invoke (direction);
		}
	}

	public void useJumpPlatformWithKeyButton (bool state, float newJumpPower)
	{
		if (useJumpPlatformEvents) {
			if (state) {
				setNewJumpPowerEvent.Invoke (newJumpPower);
			} else {
				setOriginalJumpPowerEvent.Invoke ();
			}
		}
	}

	public void setVehicleParts ()
	{
		setVehicleDamageReceivers ();

		setColliderList ();

		setVehicleRendererParts ();

		setVehicleParticleSystemList ();

		updateComponent ();

		print ("Vehicle list for damage receivers, colliders, renderer and particles have been updated");
	}

	public void setVehicleDamageReceivers ()
	{
		vehicleDamageReceiverList.Clear ();

		Component[] damageReceivers = GetComponentsInChildren (typeof(vehicleDamageReceiver));
		foreach (Component c in damageReceivers) {
			vehicleDamageReceiver currentVehicleDamageReceiver = c.GetComponent<vehicleDamageReceiver> ();
			currentVehicleDamageReceiver.setVehicle (gameObject);

			vehicleDamageReceiverList.Add (currentVehicleDamageReceiver);
		}

		updateComponent ();
	}

	public void setColliderList ()
	{
		colliderParts.Clear ();
		Component[] colliderList = GetComponentsInChildren (typeof(Collider));
		Collider colliderPart;
		foreach (Component currentCollider in colliderList) {
			colliderPart = currentCollider as Collider;
			if (!colliderPart.isTrigger) {
				colliderParts.Add (colliderPart);
			}
		}

		updateComponent ();
	}

	public void setVehicleRendererParts ()
	{
		vehicleRendererList.Clear ();
		Component[] components = GetComponentsInChildren (typeof(Renderer));
		foreach (Component c in components) {
			vehicleRendererList.Add (c.GetComponent<Renderer> ());
		}

		updateComponent ();
	}

	public void setVehicleParticleSystemList ()
	{
		if (damageParticles) {
			fireParticles.Clear ();

			Component[] fireParticlesComponents = damageParticles.GetComponentsInChildren (typeof(ParticleSystem));
			foreach (Component c in fireParticlesComponents) {
				fireParticles.Add (c.GetComponent<ParticleSystem> ());
			}

			updateComponent ();
		}
	}

	public void getAllDamageReceivers ()
	{
		advancedSettings.damageReceiverList.Clear ();
		//get all the damage receivers in the vehicle
		Component[] damageReceivers = GetComponentsInChildren (typeof(vehicleDamageReceiver));
		foreach (Component c in damageReceivers) {
			damageReceiverInfo newInfo = new damageReceiverInfo ();
			newInfo.name = "Spot " + (advancedSettings.damageReceiverList.Count + 1).ToString ();
			newInfo.spotTransform = c.gameObject.transform;
			newInfo.vehicleDamageReceiverManager = c.GetComponent<vehicleDamageReceiver> ();
			newInfo.damageMultiplier = newInfo.vehicleDamageReceiverManager.damageMultiplier;
			advancedSettings.damageReceiverList.Add (newInfo);
		}

		updateComponent ();
	}

	//health management
	public float getCurrentHealthAmount ()
	{
		return healthAmount;
	}

	public float getMaxHealthAmount ()
	{
		return maxhealthAmount;
	}

	public float getAuxHealthAmount ()
	{
		return auxHealthAmount;
	}

	public void addAuxHealthAmount (float amount)
	{
		auxHealthAmount += amount;
	}

	public float getHealthAmountToLimit ()
	{
		return maxhealthAmount - auxHealthAmount;
	}

	//energy management
	public float getCurrentEnergyAmount ()
	{
		return boostAmount;
	}

	public float getMaxEnergyAmount ()
	{
		return maxBoostAmount;
	}

	public float getAuxEnergyAmount ()
	{
		return auxPowerAmount;
	}

	public void addAuxEnergyAmount (float amount)
	{
		auxPowerAmount += amount;
	}

	public float getEnergyAmountToLimit ()
	{
		return maxBoostAmount - auxPowerAmount;
	}

	//fuel management
	public float getCurrentFuelAmount ()
	{
		return fuelAmount;
	}

	public float getMaxFuelAmount ()
	{
		return maxFuelAmount;
	}

	public float getAuxFuelAmount ()
	{
		return auxFuelAmount;
	}

	public void addAuxFuelAmount (float amount)
	{
		auxFuelAmount += amount;
	}

	public float getFuelAmountToLimit ()
	{
		return maxFuelAmount - auxFuelAmount;
	}

	public bool hasFuel ()
	{
		return fuelAmount > 0;
	}

	public void killByButton ()
	{
		setDamage (healthAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false);
	}

	public void destroyVehicle ()
	{
		setDamage (healthAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false);
	}

	public void damageVehicle (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false);
	}

	public float getVehicleRadius ()
	{
		return vehicleRadius;
	}

	public void activateHorn ()
	{
		if (useHornEvent) {
			hornEvent.Invoke ();
		}

		callToFriends ();
	}

	public void callToFriends ()
	{
		if (useHornToCallFriends) {
			friendListManager currentFriendListManager = getCurrentDriver ().GetComponent<friendListManager> ();
			if (currentFriendListManager) {
				if (callOnlyFoundFriends) {
					currentFriendListManager.callToFriends ();
				} else {
					currentFriendListManager.findFriendsInRadius (radiusToCallFriends);
				}
			}
		}
	}

	public Transform getPlaceToShoot ()
	{
		return placeToShoot;
	}

	public GameObject getCurrentDriver ()
	{
		if (IKDrivingManager) {
			return IKDrivingManager.getcurrentDriver ();
		} else {
			return null;
		}
	}

	public string getVehicleName ()
	{
		return vehicleName;
	}

	public void getImpactListInfo ()
	{
		if (!impactDecalManager) {
			impactDecalManager = FindObjectOfType<decalManager> ();
		} 
		if (impactDecalManager) {
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];
			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}
			updateComponent ();
		}
	}

	public int getDecalImpactIndex ()
	{
		return impactDecalIndex;
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (destroyed) {
			return;
		}
		if ((1 << col.gameObject.layer & layerForPassengers.value) == 1 << col.gameObject.layer) {
			IKDrivingManager.checkTriggerInfo (col, isEnter);
		}
	}

	public vehicleCameraController getVehicleCameraController ()
	{
		return vehicleCameraManager;
	}

	public AudioSource getAudioSourceElement (string name)
	{
		for (int i = 0; i < audioSourceList.Count; i++) {
			if (audioSourceList [i].audioSourceName == name) {
				return audioSourceList [i].audioSource;
			}
		}
		return null;
	}

	public bool isVehicleBeingDriven ()
	{
		return isBeingDriven;
	}

	public bool isVehicleFull ()
	{
		return IKDrivingManager.isVehicleFull ();
	}

	public IKDrivingSystem getIKDrivingSystem ()
	{
		return IKDrivingManager;
	}

	//set the camera when the player is driving on locked camera
	public void setPlayerCameraParentAndPosition (Transform mainCameraTransform, playerCamera playerCameraManager)
	{
		IKDrivingManager.setPlayerCameraParentAndPosition (mainCameraTransform, playerCameraManager);

		if (weaponsManager && weaponsManager.weaponsEnabled) {
			weaponsManager.getCameraInfo (vehicleCameraManager.currentState.cameraTransform, vehicleCameraManager.currentState.useRotationInput);
		}
	}

	public bool vehicleIsDestroyed ()
	{
		return destroyed;
	}

	public void setInvencibleState (bool state)
	{
		invincible = state;
	}

	public void setInfiniteEnergyState (bool state)
	{
		infiniteBoost = state;
	}

	public void setInfiniteFuelState (bool state)
	{
		infiniteFuel = state;
	}

	bool vehicleSetAsChildOfParent;

	public void setVehicleAndCameraParent (Transform newParent)
	{
		vehicleCameraManager.setVehicleAndCameraParent (newParent);

		if (newParent != null) {
			setVehicletAsChildOfParentState (true);
		} else {
			setVehicletAsChildOfParentState (false);
		}
	}

	public bool isVehicletAsChildOfParent ()
	{
		return vehicleSetAsChildOfParent;
	}

	public void setVehicletAsChildOfParentState (bool state)
	{
		vehicleSetAsChildOfParent = state;
	}


	//CALL INPUT FUNCTIONS
	public void inputShowControlsMenu ()
	{
		if (isBeingDriven) {
			IKDrivingManager.openOrCloseControlsMenu (!IKDrivingManager.controlsMenuOpened);
		}
	}

	public void inputSelfDestruct ()
	{
		if (isBeingDriven && canUseSelfDestruct) {
			activateSelfDestruction ();
		}
	}

	public void inputEject ()
	{
		if (isBeingDriven && canEjectFromVehicle) {
			ejectFromVehicle ();
		}
	}

	public void inputEjectWithFreeFloatingMode ()
	{
		if (isBeingDriven && canEjectFromVehicle) {
			ejectFromVehicleWithFreeFloatingMode ();
		}
	}

	public void inputUnLockCursor ()
	{
		if (isBeingDriven && canUnlockCursor) {
			setUnlockCursorState (!cursorUnlocked);
		}
	}

	public void inputHoldOrReleaseSecondaryButton (bool holdingButton)
	{
		if (isBeingDriven && canUnlockCursor && cursorUnlocked) {
			if (holdingButton) {
				vehicleCameraManager.pauseOrPlayVehicleCamera (false);
			} else {
				vehicleCameraManager.pauseOrPlayVehicleCamera (true);
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<vehicleHUDManager> ());
		#endif
	}

	void OnDrawGizmos ()
	{
		if (!advancedSettings.showGizmo) {
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
		if (!Application.isPlaying && getImpactListEveryFrame && useImpactSurface) {
			getImpactListInfo ();
		}
	}

	void DrawGizmos ()
	{
		if (advancedSettings.showGizmo && !Application.isPlaying) {
			//draw two spheres at both sides of the vehicles, to see where are launched two raycast to  
			//check if that side is not blocking by an object, so the player will get off in the other side, 
			//checking in the same way, so if both sides are blocked, the player won't get off
			//if there is not any obstacle, another ray is used to check the distance to the ground, so the player is placed at the side of the vehicle
			for (int i = 0; i < advancedSettings.damageReceiverList.Count; i++) {
				if (advancedSettings.damageReceiverList [i].spotTransform) {
					float rValue = 0;
					float gValue = 0;
					float bValue = 0;
					if (!advancedSettings.damageReceiverList [i].killedWithOneShoot) {
						rValue = advancedSettings.damageReceiverList [i].damageMultiplier / 10;
					} else {
						rValue = 1;
						gValue = 1;
					}
					Color gizmoColor = new Vector4 (rValue, gValue, bValue, advancedSettings.alphaColor);
					Gizmos.color = gizmoColor;
					Gizmos.DrawSphere (advancedSettings.damageReceiverList [i].spotTransform.position, advancedSettings.gizmoRadius);

					if (advancedSettings.damageReceiverList [i].vehicleDamageReceiverManager) {
						advancedSettings.damageReceiverList [i].vehicleDamageReceiverManager.damageMultiplier = advancedSettings.damageReceiverList [i].damageMultiplier;
					}
				}
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (transform.position, vehicleRadius);
		}
	}

	[System.Serializable]
	public class advancedSettingsClass
	{
		public List<damageReceiverInfo> damageReceiverList = new List<damageReceiverInfo> ();
		public bool showGizmo;
		public Color gizmoLabelColor;
		[Range (0, 1)] public float alphaColor;
		[Range (0, 1)] public float gizmoRadius;
	}

	[System.Serializable]
	public class damageReceiverInfo
	{
		public string name;
		public Transform spotTransform;
		[Range (1, 10)] public float damageMultiplier;

		public bool killedWithOneShoot;
		public bool needMinValueToBeKilled;
		public float minValueToBeKilled;

		public bool sendFunctionWhenDamage;
		public bool sendFunctionWhenDie;
		public UnityEvent damageFunction;

		public bool useHealthAmountOnSpot;
		public float healhtAmountOnSpot;
		public bool killCharacterOnEmtpyHealthAmountOnSpot;
		public UnityEvent eventOnEmtpyHealthAmountOnSpot;
		public bool healthAmountOnSpotEmtpy;

		public vehicleDamageReceiver vehicleDamageReceiverManager;
	}
}