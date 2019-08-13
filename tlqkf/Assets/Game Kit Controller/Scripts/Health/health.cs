using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class health : MonoBehaviour
{
	public float healthAmount = 100;
	public bool regenerateHealth;
	public bool constantRegenerate;
	public float regenerateSpeed = 0;
	public float regenerateTime;
	public float regenerateAmount;
	public bool invincible = false;
	public bool dead = false;
	public GameObject damagePrefab;
	public Transform placeToShoot;
	public bool placeToShootActive = true;
	public GameObject scorchMarkPrefab = null;

	public UnityEvent damageFunctionCall = new UnityEvent ();

	public string damageFunction;
	public bool useExtraDamageFunctions;
	public float delayInDamageFunctions;
	public List<damageFunctionInfo> extraDamageFunctionList = new List<damageFunctionInfo> ();

	public UnityEvent deadFuncionCall = new UnityEvent ();
	public UnityEvent extraDeadFunctionCall = new UnityEvent ();

	public UnityEvent resurrectFunctionCall;

	public string deadFuncion;
	public bool useExtraDeadFunctions;
	public float delayInExtraDeadFunctions;

	public enemySettings settings = new enemySettings ();
	public advancedSettingsClass advancedSettings = new advancedSettingsClass ();

	public bool showWeakSpotsInScannerMode;
	public GameObject weakSpotMesh;
	[Range (0, 1)] public float weakSpotMeshAlphaValue;

	public Slider healthSlider;

	public string enemyTag = "enemy";
	public string friendTag = "friend";

	public bool canBeSedated = true;
	public bool awakeOnDamageIfSedated;
	public bool sedateActive;
	public bool sedateUntilReceiveDamageState;

	public bool useEventOnSedate;
	public UnityEvent sedateStartEvent;
	public UnityEvent sedateEndEvent;

	public bool showSettings;
	public bool showAdvancedSettings;
	public bool showDamageDeadSettings;

	List<GameObject> projectilesReceived = new List<GameObject> ();
	List<characterDamageReceiver> damageReceiverList = new List<characterDamageReceiver> ();
	public List<characterDamageReceiver> customDamageReceiverList = new List<characterDamageReceiver> ();
	bool enemyLocated;
	GameObject scorchMark;
	ParticleSystem damageEffect;
	float lastDamageTime = 0;
	float maxhealthAmount;
	RaycastHit hit;
	Vector3 originalPlaceToShootPosition;

	float auxHealthAmount;
	string characterName;

	decalManager impactDecalManager;
	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;
	public bool useImpactSurface;

	public playerController playerControllerManager;
	public ragdollActivator ragdollManager;
	public damageInScreen damageInScreenManager;

	int lastWeakSpotIndex = -1;

	Coroutine damageOverTimeCoroutine;

	healthBarManagementSystem healthBarManager;

	bool hasHealthSlider;

	weakSpot currentWeakSpot;

	public int currentID;

	void Start ()
	{
		//get the initial health assigned
		maxhealthAmount = healthAmount;
		auxHealthAmount = maxhealthAmount;
		if (damagePrefab) {
			//if damage prefab has been assigned, instantiate the damage effect
			GameObject effect = (GameObject)Instantiate (damagePrefab, Vector3.zero, Quaternion.identity);
			effect.transform.SetParent (transform);
			effect.transform.localPosition = Vector3.zero;
			damageEffect = effect.GetComponent<ParticleSystem> ();
		}

		if (scorchMarkPrefab) {
			scorchMarkPrefab.SetActive (false);
		}

		//instantiate a health slider in the UI, used for the enemies and allies

		if (healthSlider) {
			hasHealthSlider = true;
		} else if (settings.useHealthSlider && settings.enemyHealthSlider) {

			healthBarManager = FindObjectOfType<healthBarManagementSystem> ();

			Color sliderColor = settings.allySliderColor;
			if (gameObject.tag == enemyTag) {
				characterName = settings.enemyName;
				sliderColor = settings.enemySliderColor;
			} else {
				characterName = settings.allyName;
			}

			currentID = healthBarManager.addNewTargetSlider (gameObject, settings.enemyHealthSlider, settings.sliderOffset, maxhealthAmount, characterName, settings.nameTextColor, sliderColor); 
		
			hasHealthSlider = true;
		}

		//get all the damage receivers in the character
		Component[] damageReceivers = GetComponentsInChildren (typeof(characterDamageReceiver));
		if (damageReceivers.Length > 0) {
			foreach (Component c in damageReceivers) {
				characterDamageReceiver newReceiver = c.GetComponent<characterDamageReceiver> ();
				newReceiver.character = gameObject;
				newReceiver.setCharacter (gameObject);
				damageReceiverList.Add (newReceiver);

				if (showWeakSpotsInScannerMode) {
					if (c.gameObject.GetComponent < Collider> ()) {
						Transform spot = c.transform;
						GameObject newWeakSpotMesh = (GameObject)Instantiate (weakSpotMesh, spot.position, spot.rotation);
						newWeakSpotMesh.transform.SetParent (spot);
						newWeakSpotMesh.transform.localScale = Vector3.one;
						for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
							if (advancedSettings.weakSpots [i].spotTransform == spot) {
								Renderer meshRenderer = newWeakSpotMesh.GetComponent<Renderer> ();
								for (int k = 0; k < meshRenderer.materials.Length; k++) {
									Color newColor = advancedSettings.weakSpots [i].weakSpotColor;
									newColor = new Vector4 (newColor.r, newColor.g, newColor.b, weakSpotMeshAlphaValue);
									meshRenderer.materials [k].color = newColor;
								}
							}
						}
					}
				}
			}
		} else {
			gameObject.AddComponent <characterDamageReceiver> ().setCharacter (gameObject);
		}

		if (placeToShootActive) {
			if (!placeToShoot) {
				GameObject newPlaceToShoot = new GameObject ();
				newPlaceToShoot.name = "Place To Shoot";
				placeToShoot = newPlaceToShoot.transform;
				placeToShoot.SetParent (transform);
				placeToShoot.localPosition = Vector3.zero;
			}

			if (placeToShoot) {
				originalPlaceToShootPosition = placeToShoot.transform.localPosition;
			}
		}
	}

	void Update ()
	{
		//clear the list which contains the projectiles received by the vehicle
		if (projectilesReceived.Count > 0 && Time.time > lastDamageTime + 0.3f) {
			projectilesReceived.Clear ();
		}

		//if the object can regenerate, add health after a while with no damage
		if (regenerateHealth && !dead) {
			if (constantRegenerate) {
				if (regenerateSpeed > 0 && healthAmount < maxhealthAmount) {
					if (Time.time > lastDamageTime + regenerateTime) {
						getHealth (regenerateSpeed * Time.deltaTime);
					}
				}
			} else {
				if (healthAmount < maxhealthAmount) {
					if (Time.time > lastDamageTime + regenerateTime) {
						getHealth (regenerateAmount);
						lastDamageTime = Time.time;
					}
				}
			}
		}
	}

	public void addDamageReceiversToRagdoll ()
	{
		if (advancedSettings.haveRagdoll) {
			Component[] components = GetComponentsInChildren (typeof(Collider));
			foreach (Component c in components) {
				Collider colliderPart = c.GetComponent<Collider> ();
				if (!colliderPart.isTrigger) {
					if (!colliderPart.gameObject.GetComponent <characterDamageReceiver> ()) {
						colliderPart.gameObject.AddComponent <characterDamageReceiver> ().setCharacter (gameObject);
						customDamageReceiverList.Add (colliderPart.gameObject.GetComponent <characterDamageReceiver> ());
					}
				}
			}

			customDamageReceiverList.Clear ();

			Component[] damageReceivers = GetComponentsInChildren (typeof(characterDamageReceiver));
			foreach (Component c in damageReceivers) {
				characterDamageReceiver newReceiver = c.GetComponent<characterDamageReceiver> ();
				customDamageReceiverList.Add (newReceiver);
			}
			print ("Damage recievers added to ragdoll");
			updateComponent ();
			print (customDamageReceiverList.Count);
		}
	}

	public void removeDamageReceiversFromRagdoll ()
	{
		if (advancedSettings.haveRagdoll) {
			for (int i = 0; i < customDamageReceiverList.Count; i++) {
				if (customDamageReceiverList [i] != null) {
					DestroyImmediate (customDamageReceiverList [i]);
				}
			}
			customDamageReceiverList.Clear ();
			print ("Damage recievers removed from ragdoll");
			updateComponent ();
		}
	}

	public bool receiveDamageEvenDead;

	bool recivingDamageAfterDeath;

	//receive a certain amount of damage
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker, GameObject projectile, bool damageConstant, bool searchClosestWeakSpot)
	{
		//if the objects is not dead, invincible or its health is zero, exit
		if (invincible || (dead && !receiveDamageEvenDead) || amount <= 0) {
			return;
		}

		if (dead && receiveDamageEvenDead) {
			recivingDamageAfterDeath = true;
		} else {
			recivingDamageAfterDeath = false;
		}

		if (!damageConstant) {
			//if the projectile is not a laser, store it in a list
			//this is done like this because you can add as many colliders (box or mesh) as you want (according to the vehicle meshes), 
			//which are used to check the damage received by every character, so like this the damage detection is really accurated. 
			//For example, if you shoot a grenade to a character, every collider will receive the explosion, but the character will only be damaged once, with the correct amount.
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

		if (playerControllerManager && playerControllerManager.isPlayerDriving () && !playerControllerManager.isDrivingRemotely ()) {
			return;
		}

		currentWeakSpot = new weakSpot ();
		if (advancedSettings.useWeakSpots && searchClosestWeakSpot) {
			if (advancedSettings.weakSpots.Count > 0) {
				int weakSpotIndex = getClosesWeakSpotIndex (damagePos);
				lastWeakSpotIndex = weakSpotIndex;
				if (amount < healthAmount || recivingDamageAfterDeath) {
					currentWeakSpot = advancedSettings.weakSpots [weakSpotIndex];
					if (currentWeakSpot.killedWithOneShoot) {
						if (currentWeakSpot.needMinValueToBeKilled) {
							if (currentWeakSpot.minValueToBeKilled < amount) {
								amount = healthAmount;
							}
						} else {
							amount = healthAmount;
						}
					} else {
						if (!advancedSettings.notHuman) {
							//print (advancedSettings.weakSpots [weakSpotIndex].damageMultiplier + " " + amount);
							amount *= currentWeakSpot.damageMultiplier;
						}
					}
				}

				if (advancedSettings.useHealthAmountOnSpotEnabled) {
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
			}
		}

		if (amount > healthAmount) {
			amount = healthAmount;
		}

		//print (amount);

		//active the damage prefab, substract the health amount, and set the value in the slider
		healthAmount -= amount;
		auxHealthAmount = healthAmount;

		updateSlider (healthAmount);

		if (!recivingDamageAfterDeath) {
			//call a function when the object receives damage
			if (damageFunctionCall.GetPersistentEventCount () > 0) {
				damageFunctionCall.Invoke ();
			}

			if (damageFunction != "") {
				SendMessage (damageFunction, attacker, SendMessageOptions.DontRequireReceiver);
			}

			lastDamageTime = Time.time;
			if (damageEffect) {
				//set the position of the damage in the position where the projectile hitted the object with the health component
				damageEffect.transform.position = damagePos;
				damageEffect.transform.rotation = Quaternion.LookRotation (fromDirection, Vector3.up);
				damageEffect.Play ();
			}
				
			if (damageInScreenManager) {
				damageInScreenManager.showScreenInfo (amount, true, fromDirection, healthAmount);
			}
		
			//if the health reachs 0, call the dead function
			if (healthAmount <= 0) {
				healthAmount = 0;

				removeHealthSlider ();

				dead = true;

				if (deadFuncionCall.GetPersistentEventCount () > 0) {
					deadFuncionCall.Invoke ();
				}

				if (deadFuncion != "") {
					//enable the ragdoll of the player
					SendMessage ("deathDirection", -fromDirection, SendMessageOptions.DontRequireReceiver);
					SendMessage (deadFuncion, damagePos, SendMessageOptions.DontRequireReceiver);
				}

				//check the map icon
				mapObjectInformation currentMapObjectInformation = GetComponent<mapObjectInformation> ();
				if (gameObject.tag != "Player" && currentMapObjectInformation) {
					currentMapObjectInformation.removeMapObject ();
				}

				if (useExtraDeadFunctions) {
					callExtraDeadFunctions ();
				}

				if (scorchMarkPrefab) {
					//if the object is an enemy, set an scorch below the enemy, using a raycast
					scorchMarkPrefab.SetActive (true);
					scorchMarkPrefab.transform.SetParent (null);
					RaycastHit hit;
					if (Physics.Raycast (transform.position, transform.up * (-1), out hit, 200, settings.layer)) {
						if ((1 << hit.collider.gameObject.layer & settings.layer.value) == 1 << hit.collider.gameObject.layer) {
							Vector3 scorchPosition = hit.point;
							scorchMarkPrefab.transform.position = scorchPosition + hit.normal * 0.03f;
						}
					}
				}
			} else {
				if (advancedSettings.haveRagdoll && advancedSettings.activateRagdollOnDamageReceived) {
					if (amount >= advancedSettings.minDamageToEnableRagdoll) {
						advancedSettings.ragdollEvent.Invoke ();
					}
				}

				if (useExtraDamageFunctions) {
					for (int i = 0; i < extraDamageFunctionList.Count; i++) {
						if (extraDamageFunctionList [i].damageRecived >= amount) {
							if (extraDamageFunctionList [i].damageFunctionCall.GetPersistentEventCount () > 0) {
								extraDamageFunctionList [i].damageFunctionCall.Invoke ();
							}
						}
					}
				}
			}
		}

		//call functions from weak spots when they are damaged
		if (advancedSettings.useWeakSpots && lastWeakSpotIndex > -1 && searchClosestWeakSpot) {
			bool callFunction = false;
			currentWeakSpot = advancedSettings.weakSpots [lastWeakSpotIndex];
			if (currentWeakSpot.sendFunctionWhenDamage) {
				callFunction = true;
			}

			if (currentWeakSpot.sendFunctionWhenDie && dead) {
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

		if (!recivingDamageAfterDeath) {
			if (sedateActive && (awakeOnDamageIfSedated || sedateUntilReceiveDamageState)) {
				if (ragdollManager) {
					ragdollManager.stopSedateStateCoroutine ();
				}
			}
		}
	}

	public void damageSpot (int elementIndex, float damageAmount)
	{
		setDamage (damageAmount, transform.forward, advancedSettings.weakSpots[elementIndex].spotTransform.position, gameObject, gameObject, false, true);
	}

	public void checkIfEnabledStateCanChange (bool state)
	{
		if (!receiveDamageEvenDead) {
			enabled = state;
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
		while (Time.time < lastTimeDuration + damageOverTimeDuration || (!dead && damageOverTimeToDeath)) {
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

	public void sedateCharacter (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		if (advancedSettings.haveRagdoll && ragdollManager && canBeSedated) {
			if (useWeakSpotToReduceDelay) {
				if (advancedSettings.weakSpots.Count > 0) {
					int weakSpotIndex = getClosesWeakSpotIndex (position);
					sedateDelay -= advancedSettings.weakSpots [weakSpotIndex].damageMultiplier;
					if (sedateDelay < 0) {
						sedateDelay = 0;
					}
				}
			}
			sedateUntilReceiveDamageState = sedateUntilReceiveDamage;
			if (sedateUntilReceiveDamage) {
				currentSedateDuration = 0;
			} else {
				currentSedateDuration = sedateDuration;
			}
			ragdollManager.sedateCharacter (sedateDelay, sedateUntilReceiveDamage, sedateDuration);
		}
	}

	float currentSedateDuration;

	public float getCurrentSedateDuration ()
	{
		return currentSedateDuration;
	}

	public void setSedateState (bool state)
	{
		sedateActive = state;

		if (dead) {
			return;
		}

		if (useEventOnSedate) {
			if (sedateActive) {
				sedateStartEvent.Invoke ();
			} else {
				sedateEndEvent.Invoke ();
			}
		}
	}

	public void setImpactReceivedInfo (Vector3 impactVelocity, Collider impactCollider)
	{
		if (advancedSettings.haveRagdoll && ragdollManager) {
			ragdollManager.setImpactReceivedInfo (impactVelocity, impactCollider);
		}
	}

	public bool canRagdollReceiveDamageOnImpact ()
	{
		return advancedSettings.ragdollCanReceiveDamageOnImpact;
	}

	public void updateSlider (float value)
	{
		if (hasHealthSlider) {
			if (healthSlider) {
				healthSlider.value = value;
			} else {
				healthBarManager.setSliderAmount (currentID, value); 
			}
		}
	}

	public void removeHealthSlider ()
	{
		if (hasHealthSlider) {
			if (healthBarManager) {
				healthBarManager.removeTargetSlider (currentID); 
			}
		}
	}

	public void getHealth (float amount)
	{
		if (damageInScreenManager) {
			damageInScreenManager.showScreenInfo (amount, false, Vector3.zero, healthAmount);
		}

		if (!dead) {
			healthAmount += amount;
			//check that the health amount is not higher that the health max value of the slider
			if (healthAmount >= maxhealthAmount) {
				healthAmount = maxhealthAmount;
			}

			updateSlider (healthAmount);
		}
		
		auxHealthAmount = healthAmount;
	}

	//if an enemy becomes an ally, set its name and its slider color
	public void setSliderInfo (string name, Color color)
	{
		if (hasHealthSlider) {
			healthBarManager.setSliderInfo (currentID, name, settings.nameTextColor, color); 
			characterName = name;
		}
	}

	public void hacked ()
	{
		setSliderInfo (settings.allyName, Color.green);
	}

	//restart the health component of the object
	public void resurrect ()
	{
		healthAmount = maxhealthAmount;
		dead = false;

		updateSlider (healthAmount);

		resurrectFunctionCall.Invoke ();
	}

	public void setSliderVisibleState (bool state)
	{
		if (hasHealthSlider) {
			if (healthBarManager) {
				healthBarManager.setSliderVisibleState (currentID, state); 
			}
		}
	}

	public void setSliderVisibleStateForPlayer (GameObject player, bool state)
	{
		if (hasHealthSlider) {
			if (healthBarManager) {
				healthBarManager.setSliderVisibleStateForPlayer (currentID, player, state); 
			}
		}
	}

	public int getClosesWeakSpotIndex (Vector3 collisionPosition)
	{
		float distance = Mathf.Infinity;
		int index = -1;
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			float currentDistance = GKC_Utils.distance (collisionPosition, advancedSettings.weakSpots [i].spotTransform.position);
			if (currentDistance < distance) {
				distance = currentDistance;
				index = i;
			}
		}

		if (index > -1) {
			if (advancedSettings.showGizmo) {
				print (advancedSettings.weakSpots [index].name);
			}
		}
		return index;
	}

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

	public void killCharacter ()
	{
		killByButton ();
	}

	public void killByButton ()
	{
		setDamage (healthAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false);
	}

	public void killCharacter (Vector3 fromDirection, Vector3 damagePos, GameObject attacker, GameObject projectile, bool damageConstant)
	{
		setDamage (healthAmount, fromDirection, damagePos, attacker, projectile, damageConstant, false);
	}

	public void takeHealth (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false);
	}

	public void takeConstantDamage (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, true, false);
	}

	public void callExtraDeadFunctions ()
	{
		StartCoroutine (callExtraDeadFunctionsnCoroutine ());
	}

	IEnumerator callExtraDeadFunctionsnCoroutine ()
	{
		yield return new WaitForSeconds (delayInExtraDeadFunctions);
		if (extraDeadFunctionCall.GetPersistentEventCount () > 0) {
			extraDeadFunctionCall.Invoke ();
		}
		yield return null;			
	}

	public void changePlaceToShootPosition (bool state)
	{
		if (state) {
			placeToShoot.transform.localPosition = originalPlaceToShootPosition - placeToShoot.transform.up;
		} else {
			placeToShoot.transform.localPosition = originalPlaceToShootPosition;
		}
	}

	public Transform getPlaceToShoot ()
	{
		return placeToShoot;
	}

	public void updateDamageReceivers ()
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			if (advancedSettings.weakSpots [i].spotTransform && advancedSettings.weakSpots [i].spotTransform.GetComponent<characterDamageReceiver> ()) {
				#if UNITY_EDITOR
				EditorUtility.SetDirty (advancedSettings.weakSpots [i].spotTransform.GetComponent<characterDamageReceiver> ());
				#endif
			}
		}
	}

	public bool isDead ()
	{
		return dead;
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

	public string getCharacterName ()
	{
		return characterName;
	}

	//Set weak spots on humanoid npcs
	public void setHumanoidWeaKSpots ()
	{
		Animator animator = transform.GetChild (0).GetComponentInChildren<Animator> ();
		if (animator) {		
			advancedSettings.weakSpots.Clear ();

			Transform head = animator.GetBoneTransform (HumanBodyBones.Head);
			Transform upperBody = animator.GetBoneTransform (HumanBodyBones.Chest);
			Transform lowerBody = animator.GetBoneTransform (HumanBodyBones.Spine);
			Transform upperLeftLeg = animator.GetBoneTransform (HumanBodyBones.LeftUpperLeg);
			Transform lowerLeftLeg = animator.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
			Transform upperRightLeg = animator.GetBoneTransform (HumanBodyBones.RightUpperLeg);
			Transform lowerRightLeg = animator.GetBoneTransform (HumanBodyBones.RightLowerLeg);
			Transform upperLeftArm = animator.GetBoneTransform (HumanBodyBones.LeftUpperArm);
			Transform lowerLeftArm = animator.GetBoneTransform (HumanBodyBones.LeftLowerArm);
			Transform upperRightArm = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
			Transform lowerRightArm = animator.GetBoneTransform (HumanBodyBones.RightLowerArm);

			addWeakSpot (head, "Head", 1, true);
			addWeakSpot (upperBody, "Upper Body", 2, false);
			addWeakSpot (lowerBody, "Lower Body", 2, false);
			addWeakSpot (upperLeftLeg, "Upper Left Leg", 2, false);
			addWeakSpot (lowerLeftLeg, "Lower Left Leg", 0.5f, false);
			addWeakSpot (upperRightLeg, "Upper Right Leg", 2, false);
			addWeakSpot (lowerRightLeg, "Lower Righ Leg", 0.5f, false);
			addWeakSpot (upperLeftArm, "Upper Left Arm", 2, false);
			addWeakSpot (lowerLeftArm, "Lower Left Arm", 0.5f, false);
			addWeakSpot (upperRightArm, "Upper Right Arm", 2, false);
			addWeakSpot (lowerRightArm, "Lower Right Arm", 0.5f, false);

			updateDamageReceivers ();
		}
	}

	public void addWeakSpot (Transform spotTransform, string spotName, float damageMultiplier, bool killedInOneShot)
	{
		weakSpot newWeakSpot = new weakSpot ();
		newWeakSpot.name = spotName;
		newWeakSpot.spotTransform = spotTransform;
		newWeakSpot.damageMultiplier = damageMultiplier;
		newWeakSpot.killedWithOneShoot = killedInOneShot;
		advancedSettings.weakSpots.Add (newWeakSpot);
	}

	public void setInvincibleState (bool state)
	{
		invincible = state;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<health> ());
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
		// && !Application.isPlaying
		if (advancedSettings.showGizmo) {
			for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
				if (advancedSettings.weakSpots [i].spotTransform) {
					float rValue = 0;
					float gValue = 0;
					float bValue = 0;
					if (!advancedSettings.weakSpots [i].killedWithOneShoot) {
						if (advancedSettings.weakSpots [i].damageMultiplier < 1) {
							bValue = advancedSettings.weakSpots [i].damageMultiplier / 0.1f;
						} else {
							rValue = advancedSettings.weakSpots [i].damageMultiplier / 20;
						}
					} else {
						rValue = 1;
						gValue = 1;
					}
					advancedSettings.weakSpots [i].weakSpotColor = new Vector4 (rValue, gValue, bValue, advancedSettings.alphaColor);
					Gizmos.color = advancedSettings.weakSpots [i].weakSpotColor;
					Gizmos.DrawSphere (advancedSettings.weakSpots [i].spotTransform.position, advancedSettings.gizmoRadius);
					if (advancedSettings.notHuman) {
						advancedSettings.weakSpots [i].spotTransform.GetComponent<characterDamageReceiver> ().damageMultiplier = advancedSettings.weakSpots [i].damageMultiplier;
					}
				}
			}

			if (settings.useHealthSlider && settings.enemyHealthSlider) {
				Gizmos.color = advancedSettings.gizmoLabelColor;
				Gizmos.DrawSphere (transform.position + settings.sliderOffset, advancedSettings.gizmoRadius);
			}
		}
	}

	[System.Serializable]
	public class enemySettings
	{
		public bool useHealthSlider = true;
		public GameObject enemyHealthSlider;
		public Vector3 sliderOffset = new Vector3 (0, 1, 0);
		public bool useRaycastToCheckVisible;
		public LayerMask layer;
		public string enemyName;
		public string allyName;
		public Color enemySliderColor = Color.red;
		public Color allySliderColor = Color.green;
		public Color nameTextColor = Color.black;
	}

	[System.Serializable]
	public class advancedSettingsClass
	{
		public bool notHuman;
		public bool useWeakSpots;
		public List<weakSpot> weakSpots = new List<weakSpot> ();

		public bool useHealthAmountOnSpotEnabled = true;

		public bool haveRagdoll;
		public bool allowPushCharacterOnExplosions = true;

		public bool activateRagdollOnDamageReceived = true;
		public float minDamageToEnableRagdoll;
		public bool ragdollCanReceiveDamageOnImpact;
		public UnityEvent ragdollEvent;

		public bool showGizmo;
		public Color gizmoLabelColor;
		[Range (0, 1)] public float alphaColor;
		[Range (0, 1)] public float gizmoRadius;
	}

	[System.Serializable]
	public class weakSpot
	{
		public string name;
		public Transform spotTransform;
		[Range (0.1f, 20)] public float damageMultiplier;

		public bool killedWithOneShoot;
		public bool needMinValueToBeKilled;
		public float minValueToBeKilled;

		public Color weakSpotColor;

		public bool sendFunctionWhenDamage;
		public bool sendFunctionWhenDie;
		public UnityEvent damageFunction;

		public bool useHealthAmountOnSpot;
		public float healhtAmountOnSpot;
		public bool killCharacterOnEmtpyHealthAmountOnSpot;
		public UnityEvent eventOnEmtpyHealthAmountOnSpot;
		public bool healthAmountOnSpotEmtpy;
	}

	[System.Serializable]
	public class damageFunctionInfo
	{
		public string Name;
		public UnityEvent damageFunctionCall;
		public float damageRecived;
	}
}