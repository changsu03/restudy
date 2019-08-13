using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class ragdollActivator : MonoBehaviour
{
	//Main variables
	public deathType typeOfDeath;
	public float timeToGetUp;
	public List<GameObject> objectsToIgnore = new List<GameObject> ();

	//Ragdoll physics variables
	public float ragdollToMecanimBlendTime = 0.5f;
	public float maxRagdollVelocity;
	public float maxVelocityToGetUp;
	public float extraForceOnRagdoll;
	public LayerMask layer;

	//Ragdoll state variables
	public ragdollState currentState = ragdollState.animated;
	public healthState playerState;
	public bool onGround;
	public bool canMove = true;

	//Player variables
	public float timeToShowMenu;

	//AI variables
	public bool usedByAI;

	public UnityEvent eventOnEnterRagdoll;
	public UnityEvent eventOnExitRagdoll;

	public string tagForColliders;

	public bool useDeathSound = true;
	public AudioClip deathSound;
	public AudioSource mainAudioSource;

	public GameObject characterBody;

	public float getUpDelay = 2;
	float lastTimeStateEnabled;

	//Damage on impact variables
	public bool ragdollCanReceiveDamageOnImpact;
	public float minTimeToReceiveDamageOnImpact;
	public float minVelocityToReceiveDamageOnImpact;
	public float receiveDamageOnImpactMultiplier;
	public float minTimToReceiveImpactDamageAgain;
	float lastTimeDamageOnImpact;

	public enum healthState
	{
		alive,
		dead,
		fallen
	}

	public enum ragdollState
	{
		animated,
		ragdolled,
		blendToAnim
	}

	public enum deathType
	{
		ragdoll,
		mecanim
	}

	List<bodyPart> bodyParts = new List<bodyPart> ();
	List<Collider> colliderBodyParts = new List<Collider> ();
	List<Rigidbody> rigidbodyBodyParts = new List<Rigidbody> ();
	List<Transform> objectsToIgnoreChildren = new List<Transform> ();
	float mecanimToGetUpTransitionTime = 0.05f;
	float ragdollingEndTime = -1;
	float deadMenuTimer;
	Vector3 ragdolledHipPosition;
	Vector3 ragdolledHeadPosition;
	Vector3 ragdolledFeetPosition;
	Vector3 playerVelocity;
	Vector3 damagePos;
	Vector3 damageDirection;
	Vector3 originalFirtPersonPivotPosition;

	public Transform playerCOM;

	public Transform rootMotion;

	public bool checkGetUpPaused;

	Transform headTransform;
	Transform leftFootTransform, rightFootTransform;
	GameObject skeleton;
	bool ragdollAdded;
	bool pushAllBodyParts;

	public playerWeaponsManager weaponsManager;
	public Transform mainCameraTransform;
	public playerStatesManager statesManager;
	public health healthManager;
	public Collider mainCollider;
	public playerInputManager playerInput;

	public Animator mainAnimator;
	public playerController playerControllerManager;
	public playerCamera cameraManager;
	public otherPowers powersManager;
	public gravitySystem gravityManager;
	public IKSystem IKSystemManager;
	public Rigidbody mainRigidbody;

	public menuPause pauseManager;
	public footStepManager stepsManager;
	public closeCombatSystem combatManager;

	public string getUpFromBellyName = "Get Up From Belly";
	public string getUpFromBackName = "Get Up From Back";

	public string deathName = "Dead";

	Rigidbody closestPart;

	Rigidbody hipsRigidbody;

	checkpointSystem checkpointManager;

	Rigidbody currentRigidbodyPart;

	RaycastHit hit;
	bool dropCamera;
	bool enableBehaviour;

	float currentSedateDuration;
	bool sedateUntilReceiveDamageState;
	float lastTimeSedated;
	bool sedateActive;

	float lastTimeRagdolled;

	bool carryinWeaponsPreviously;

	Coroutine dropOrPickCameraMovement;
	Vector3 damageVelocity;

	bool previouslyRagdolled;

	void Awake ()
	{
		for (int i = 0; i < objectsToIgnore.Count; i++) {
			if (objectsToIgnore [i] != null) {
				Component[] childrens = objectsToIgnore [i].GetComponentsInChildren (typeof(Transform));
				foreach (Component c in childrens) {
					objectsToIgnoreChildren.Add (c.GetComponent<Transform> ());
				}
			}
		}
	}

	void Start ()
	{
		if (playerCOM == null) {
			playerCOM = IKSystemManager.getIKBodyCOM ();
		}
			
		setBodyColliderList ();

		setKinematic (true);

		//store all the part inside the model of the player, in this case, his bones
		Component[] components = characterBody.GetComponentsInChildren (typeof(Transform));
		foreach (Component c in components) {
			//the objects with the ignore raycast layer belong to the head, and those are not neccessary for the ragdoll
			if (c.gameObject.layer != LayerMask.NameToLayer ("Ignore Raycast") && !checkChildsObjectsToIgnore (c.gameObject)) {
				bodyPart bodyPart = new bodyPart ();
				bodyPart.name = c.name;
				bodyPart.transform = c as Transform;
				bodyParts.Add (bodyPart);
			}
		}

		rootMotion = mainAnimator.GetBoneTransform (HumanBodyBones.Hips);
		headTransform = mainAnimator.GetBoneTransform (HumanBodyBones.Head);
		leftFootTransform = mainAnimator.GetBoneTransform (HumanBodyBones.LeftFoot);
		rightFootTransform = mainAnimator.GetBoneTransform (HumanBodyBones.RightFoot);
		if (rootMotion) {
			skeleton = rootMotion.parent.gameObject;
		}
			
		hipsRigidbody = rootMotion.GetComponent<Rigidbody> ();

		setBodyRigidbodyList ();

		if (rigidbodyBodyParts.Count > 0) {
			ragdollAdded = true;
		}

		checkpointManager = FindObjectOfType<checkpointSystem> ();
	}

	void Update ()
	{
		//when the ragdoll is enabled
		if (!usedByAI) {
			if (playerState == healthState.dead) {
				//check if the player is on the ground, so he can get up
				if (Physics.Raycast (rootMotion.position + Vector3.up, -Vector3.up, out hit, 2, layer) && hipsRigidbody.velocity.magnitude < maxVelocityToGetUp) {
					onGround = true;
					if (!dropCamera && cameraManager.isFirstPersonActive ()) {
						originalFirtPersonPivotPosition = mainCameraTransform.localPosition;

						dropOrPickCamera (true);

						dropCamera = true;
					}
				} else {
					onGround = false;
				}

				//enable the die menu after a few seconds and he is on the ground
				if (deadMenuTimer > 0 && onGround) {
					deadMenuTimer -= Time.deltaTime;
					if (deadMenuTimer < 0) {
						pauseManager.death ();
					}
				}
			}
		}

		if (playerState == healthState.fallen) {
			if (sedateActive) {
				if (Time.time > lastTimeSedated + currentSedateDuration && !sedateUntilReceiveDamageState) {
					stopSedateStateCoroutine ();
				}
			} else {
				//check if the player is on the ground, so he can get up
				if (hipsRigidbody.velocity.magnitude < maxVelocityToGetUp) {
					onGround = true;
				} else {
					onGround = false;
				}

				if (!checkGetUpPaused) {
					//enable the die menu after a few seconds and he is on the ground
					if (deadMenuTimer > 0 && onGround) {
						deadMenuTimer -= Time.deltaTime;
						if (deadMenuTimer <= 0) {
							getUp ();
						}
					}
				}
			}
		}

		if (currentState == ragdollState.ragdolled) {
			//set the empty player gameObject position with the hips of the character
			transform.position = rootMotion.position;
			//prevent the ragdoll reachs a high velocity
			if (hipsRigidbody.velocity.y <= -maxRagdollVelocity) {
				Vector3 newVelocity = new Vector3 (hipsRigidbody.velocity.x, -maxRagdollVelocity, hipsRigidbody.velocity.z);
				hipsRigidbody.velocity = newVelocity;
			}
		}

		if (currentState == ragdollState.animated && enableBehaviour) {
			if (Time.time > lastTimeStateEnabled + 0.5f) {
				mainAnimator.SetBool (getUpFromBackName, false);
				mainAnimator.SetBool (getUpFromBellyName, false);
			}

			if (Time.time > lastTimeStateEnabled + getUpDelay) {
				if (typeOfDeath == deathType.ragdoll || previouslyRagdolled) {
					//allow the scripts work again
					if (cameraManager) {
						gravityManager.death (false);
					}

					if (powersManager) {
						powersManager.death (false);
					}

					if (playerControllerManager) {
						playerControllerManager.changeScriptState (true);
						playerControllerManager.setPlayerDeadState (false);
					}

					enableBehaviour = false;
					canMove = true;

					eventOnExitRagdoll.Invoke ();

					if (playerControllerManager == null || !playerControllerManager.isCharacterControlOverrideActive ()) {
						healthManager.setSliderVisibleState (true);
					}

					if (cameraManager) {
						if (!cameraManager.isFirstPersonActive ()) {
							checkDrawWeaponsWhenResurrect ();
						}
					}

					previouslyRagdolled = false;
				} else {
					enableComponentsFromGettingUp (false);
					enableBehaviour = false;
					canMove = true;
				}
			}
		}
	}

	void LateUpdate ()
	{
		if (currentState == ragdollState.blendToAnim) {
			if (Time.time <= ragdollingEndTime + mecanimToGetUpTransitionTime) {
				//set the position of all the parts of the character to match them with the animation
				Vector3 animatedToRagdolled = ragdolledHipPosition - rootMotion.position;
				Vector3 newRootPosition = characterBody.transform.position + animatedToRagdolled;

				//use a raycast downwards and find the highest hit that does not belong to the character 
				RaycastHit[] hits = Physics.RaycastAll (new Ray (newRootPosition + transform.up, Vector3.down), 1, layer);
				float distance = Mathf.Infinity;
				foreach (RaycastHit hit in hits) {
					if (!hit.transform.IsChildOf (characterBody.transform)) {
						if (distance < Mathf.Max (newRootPosition.y, hit.point.y)) {
							distance = Mathf.Max (newRootPosition.y, hit.point.y);
						}
					}
				}
				if (distance != Mathf.Infinity) {
					newRootPosition.y = distance;
				}

				characterBody.transform.position = newRootPosition;
				//set the rotation of all the parts of the character to match them with the animation

				Vector3 ragdolledDirection = ragdolledHeadPosition - ragdolledFeetPosition;
				ragdolledDirection.y = 0;
				Vector3 meanFeetPosition = 0.5f * (leftFootTransform.position + rightFootTransform.position);
				Vector3 animatedDirection = headTransform.position - meanFeetPosition;
				animatedDirection.y = 0;
				characterBody.transform.rotation *= Quaternion.FromToRotation (animatedDirection.normalized, ragdolledDirection.normalized);
			}

			//compute the ragdoll blend amount in the range 0 to 1
			float ragdollBlendAmount = 1.0f - (Time.time - ragdollingEndTime - mecanimToGetUpTransitionTime) / ragdollToMecanimBlendTime;
			ragdollBlendAmount = Mathf.Clamp01 (ragdollBlendAmount);
			//to get a smooth transition from a ragdoll to animation, lerp the position of the hips 
			//and slerp all the rotations towards the ones stored when ending the ragdolling
			foreach (bodyPart b in bodyParts) {
				//this if is to avoid change the root of the character, only the actual body parts
				if (b.transform != characterBody.transform) { 
					//position is only interpolated for the hips
					if (b.transform == rootMotion) {
						b.transform.position = Vector3.Lerp (b.transform.position, b.storedPosition, ragdollBlendAmount);
					}
					//rotation is interpolated for all body parts
					b.transform.rotation = Quaternion.Slerp (b.transform.rotation, b.storedRotation, ragdollBlendAmount);
				}
			}

			//if the ragdoll blend amount has decreased to zero, change to animated state
			if (ragdollBlendAmount == 0) {
				setPlayerToRegularState ();
				currentState = ragdollState.animated;
				return;
			}
		}
	}

	//get the direction of the projectile that killed the player
	void deathDirection (Vector3 dir)
	{
		damageDirection = dir;
	}

	//the player has dead, get the last damage position, and the rigidbody velocity of the player
	void die (Vector3 pos)
	{
		if (statesManager) {
			statesManager.disableVehicleDrivenRemotely ();
		}

		canMove = false;

		eventOnEnterRagdoll.Invoke ();

		playerState = healthState.dead;
	
		if (weaponsManager) {
			//check if the player was using weapons before dying
			carryinWeaponsPreviously = weaponsManager.isPlayerCarringWeapon ();

			//set dead state in weapon to drop the current or currents weapons that the player has if he is in weapon mode and is carrying one of them
			weaponsManager.setDeadState (true);
		}

		if (statesManager) {
			statesManager.checkPlayerStates ();
		}

		damagePos = pos;
		playerVelocity = mainRigidbody.velocity;
		//check if the player has a ragdoll, if he hasn't it, then use the mecanim instead, to avoid issues

		bool canUseRagdoll = false;
		if (!ragdollAdded) {
			typeOfDeath = deathType.mecanim;
		} else {
			if (!Physics.Raycast (transform.position + Vector3.up, -Vector3.up, out hit, 2, layer)) {
				canUseRagdoll = true;
			}
		}

		if (combatManager) {
			combatManager.enableOrDisableTriggers (false);
		}

		if (stepsManager) {
			stepsManager.enableOrDisableFootSteps (false);
		}

		//check if the player use mecanim for the death, and if the first person mode is enabled, to use animations instead ragdoll
		if ((typeOfDeath == deathType.mecanim || (cameraManager && cameraManager.isFirstPersonActive ())) && !canUseRagdoll) {
			//disable the player and enable the gravity in the player's ridigdboby
			if (playerControllerManager) {
				playerControllerManager.changeScriptState (false);
				playerControllerManager.setPlayerDeadState (true);
			}

			if (gravityManager) {
				gravityManager.death (true);
			}

			if (cameraManager) {
				cameraManager.death (true, typeOfDeath == deathType.ragdoll);
			}

			if (powersManager) {
				powersManager.death (true);
			}

			//set the dead state in the mecanim
			mainAnimator.SetBool (deathName, true);
		}

		//else enable the ragdoll
		else {
			enableOrDisableRagdoll (true);
		}

		if (usedByAI) {
			tag = "Untagged";
		} else {
			deadMenuTimer = timeToShowMenu;
		}

		if (useDeathSound) {
			if (mainAudioSource) {
				mainAudioSource.PlayOneShot (deathSound);
			}
		}

		stopSedateStateCoroutine ();
	}
		
	public void checkLastCheckpoint ()
	{
		setcheckToGetUpState ();
	}

	public void setcheckToGetUpState ()
	{
		bool usingCheckPoinnt = false;
		if (checkpointManager && !usedByAI) {
			if (checkpointManager.thereIsLasCheckpoint ()) {
				usingCheckPoinnt = true;
				if (checkpointManager.deathLoackCheckpointType == checkpointSystem.deathLoadCheckpoint.respawn) {
					quickGetUp ();
					checkpointManager.respawnPlayer (gameObject);
				} else if (checkpointManager.deathLoackCheckpointType == checkpointSystem.deathLoadCheckpoint.reloadScene) {
					playerControllerManager.getPlayerManagersParentGameObject ().GetComponent<saveGameSystem> ().reloadLastCheckpoint ();
				} else if (checkpointManager.deathLoackCheckpointType == checkpointSystem.deathLoadCheckpoint.none) {
					getUp ();
				} 
			} 
		}

		if (!usingCheckPoinnt) {
			playerState = healthState.fallen;
			deadMenuTimer = 0.3f;

			healthManager.resurrect ();
		}
	}

	//play the game again
	public void getUp ()
	{
		if (playerState == healthState.dead) {
			healthManager.resurrect ();
		}

		playerState = healthState.alive;
		onGround = false;

		mainCollider.isTrigger = false;

		if (combatManager) {
			combatManager.enableOrDisableTriggers (true);
		}

		if (stepsManager) {
			stepsManager.enableOrDisableFootSteps (true);
		}

		bool isFirstPersonActive = false;

		if (cameraManager) {
			isFirstPersonActive = cameraManager.isFirstPersonActive ();
		}

		//check if the player use mecanim for the death, and if the first person mode is enabled, to use animations instead ragdoll
		if ((typeOfDeath == deathType.mecanim || (cameraManager && isFirstPersonActive)) && currentState == ragdollState.animated) {
			//set the get up animation in the mecanim
			mainAnimator.SetBool (deathName, false);
			mainAnimator.SetBool (getUpFromBackName, true);

			if (isFirstPersonActive) {
				enableComponentsFromGettingUp (isFirstPersonActive);
			} else {
				enableBehaviour = true;
				lastTimeStateEnabled = Time.time;
			}
		} else {
			//else disable the ragdoll
			enableOrDisableRagdoll (false);
		}

		damageDirection = Vector3.zero;
		resetLastTimeMoved ();

		if (weaponsManager) {
			weaponsManager.setDeadState (false);
		}
	}

	public void quickGetUp ()
	{
		if (playerState == healthState.dead) {
			healthManager.resurrect ();
		}

		setPlayerToRegularState ();
		currentState = ragdollState.animated;

		playerState = healthState.alive;
		currentState = ragdollState.animated;
		onGround = false;

		combatManager.enableOrDisableTriggers (true);

		stepsManager.enableOrDisableFootSteps (true);

		//enable again the player
		playerControllerManager.enabled = true;
		gravityManager.death (false);

		cameraManager.death (false, typeOfDeath == deathType.ragdoll);

		powersManager.death (false);

		//reset the rotation of the player
		if (cameraManager.isFirstPersonActive ()) {
			if (dropCamera) {
				mainCameraTransform.SetParent (cameraManager.pivotCameraTransform.transform);
				mainCameraTransform.localPosition = originalFirtPersonPivotPosition;
				mainCameraTransform.localRotation = Quaternion.identity;
				dropCamera = false;
			}
		}

		setKinematic (true);

		mainAnimator.enabled = true;

		rootMotion.localPosition = Vector3.zero;
		playerControllerManager.changeScriptState (true);
		playerControllerManager.setPlayerDeadState (false);
		canMove = true;

		damageDirection = Vector3.zero;
		resetLastTimeMoved ();
		weaponsManager.setDeadState (false);

		carryinWeaponsPreviously = false;
	}

	public void enableComponentsFromGettingUp (bool isFirstPersonActive)
	{
		//enable again the player
		if (playerControllerManager) {
			playerControllerManager.enabled = true;
		}

		if (gravityManager) {
			gravityManager.death (false);
		}

		if (cameraManager) {
			cameraManager.death (false, typeOfDeath == deathType.ragdoll);
		}

		if (powersManager) {
			powersManager.death (false);
		}

		//reset the rotation of the player
		if (cameraManager && isFirstPersonActive) {
			if (dropCamera) {
				dropOrPickCamera (false);

				dropCamera = false;
			}
		}

		if (playerControllerManager) {
			playerControllerManager.changeScriptState (true);
			playerControllerManager.setPlayerDeadState (false);
		}

		if (cameraManager && isFirstPersonActive) {
			mainCollider.enabled = false;
			mainCollider.enabled = true;
			canMove = true;
		}
	}

	public void damageToFall ()
	{
		if (playerState == healthState.fallen || playerState == healthState.dead) {
			return;
		}

		if ((cameraManager == null || !cameraManager.isFirstPersonActive ()) && ragdollAdded) {
			if (weaponsManager) {
				carryinWeaponsPreviously = weaponsManager.isUsingWeapons ();
			}

			eventOnEnterRagdoll.Invoke ();

			canMove = false;
			playerState = healthState.fallen;

			if (statesManager) {
				statesManager.checkPlayerStates ();
			}

			damagePos = hipsRigidbody.position;
			playerVelocity = mainRigidbody.velocity;
			//damageDirection = Vector3.zero;

			if (combatManager) {
				combatManager.enableOrDisableTriggers (false);
			}

			if (stepsManager) {
				stepsManager.enableOrDisableFootSteps (false);
			}

			enableOrDisableRagdoll (true);
			deadMenuTimer = timeToGetUp;
		}
	}

	public void dropOrPickCamera (bool state)
	{
		if (dropOrPickCameraMovement != null) {
			StopCoroutine (dropOrPickCameraMovement);
		}
		dropOrPickCameraMovement = StartCoroutine (dropOrPickCameraCoroutine (state));
	}

	IEnumerator dropOrPickCameraCoroutine (bool state)
	{
		Vector3 targetPosition = Vector3.zero;
		Vector3 currentPosition = Vector3.zero;

		if (state) {
			mainCameraTransform.SetParent (null);
			if (Physics.Raycast (mainCameraTransform.position, -Vector3.up, out hit, Mathf.Infinity, layer)) {
				targetPosition = hit.point + transform.up * 0.3f;
			}
			currentPosition = mainCameraTransform.position;
		} else {
			mainCameraTransform.SetParent (cameraManager.pivotCameraTransform.transform);
			targetPosition = originalFirtPersonPivotPosition;
			currentPosition = mainCameraTransform.localPosition;
		}

		float i = 0.0f;
		while (i < 1.0f) {
			i += Time.deltaTime * 3;
			if (state) {
				mainCameraTransform.position = Vector3.Lerp (currentPosition, targetPosition, i);
			} else {
				mainCameraTransform.localPosition = Vector3.Lerp (currentPosition, targetPosition, i);
				mainCameraTransform.localRotation = Quaternion.Slerp (mainCameraTransform.localRotation, Quaternion.identity, i);
			}
			yield return null;
		}

		print (state + " " + carryinWeaponsPreviously);
		if (!state) {
			checkDrawWeaponsWhenResurrect ();
		}
	}

	//public property that can be set to toggle between ragdolled and animated character
	public void enableOrDisableRagdoll (bool value)
	{
		if (value) {
			if (currentState == ragdollState.animated || currentState == ragdollState.blendToAnim) {
				setTransitionToRagdoll ();
			} else if (sedateActive || playerState == healthState.dead) {
				for (int i = 0; i < colliderBodyParts.Count; i++) {
					if (!colliderBodyParts [i].isTrigger) {
						colliderBodyParts [i].tag = tagForColliders;
					}
				}
			}
		} else {
			if (currentState == ragdollState.ragdolled) {
				setTransitionToAnimation ();
			}
		}	
	}

	public void setTransitionToRagdoll ()
	{
		//transition from animated to ragdolled
		characterBody.transform.SetParent (null);
		rootMotion.SetParent (null);
		characterBody.transform.rotation = new Quaternion (0, characterBody.transform.rotation.y, 0, characterBody.transform.rotation.w);

		rootMotion.SetParent (skeleton.transform);
		setKinematic (false);

		mainAnimator.enabled = false;

		currentState = ragdollState.ragdolled;

		previouslyRagdolled = true;

		//pause the scripts to stop any action of the player 
		if (playerControllerManager) {
			playerControllerManager.changeScriptState (false);
			playerControllerManager.setPlayerDeadState (true);
		}

		if (gravityManager) {
			gravityManager.death (true);
		}

		if (cameraManager) {
			cameraManager.death (true, true);
		}

		if (powersManager) {
			powersManager.death (true);
		}

		mainCollider.enabled = false;
		healthManager.setSliderVisibleState (false);

		lastTimeRagdolled = Time.time;
	}

	public void setTransitionToAnimation ()
	{
		//transition from ragdolled to animated through the blendToAnim state
		setKinematic (true);

		//store the state change time
		ragdollingEndTime = Time.time; 

		mainAnimator.enabled = true;

		currentState = ragdollState.blendToAnim;  

		//store the ragdolled position for blending
		foreach (bodyPart b in bodyParts) {
			b.storedRotation = b.transform.rotation;
			b.storedPosition = b.transform.position;
		}

		//save some key positions
		ragdolledFeetPosition = 0.5f * (leftFootTransform.position + rightFootTransform.position);
		ragdolledHeadPosition = headTransform.position;
		ragdolledHipPosition = rootMotion.position;

		//start the get up animation checking if the character is on his back or face down, to play the correct animation
		if (rootMotion.up.y > 0) { 
			mainAnimator.SetBool (getUpFromBackName, true);
		} else {
			mainAnimator.SetBool (getUpFromBellyName, true);
		}
	}

	//set the state of all the rigidbodies inside the character
	//kinematic is enabled or disabled according to the state
	void setKinematic (bool state)
	{
		if (colliderBodyParts.Count == 0) {
			setBodyColliderList ();
		}

		//if state== false, it means the player has dead, so get the position of the projectile that kills him,
		//and them the closest rigidbody of the character, to add velocity in the opposite direction to that part of the player
		if (!state) {
			closestPart = searchClosestBodyPart ();
		}

		if (!state) {
			damageVelocity = damageDirection * extraForceOnRagdoll;
			if (damageVelocity == Vector3.zero) {
				damageVelocity = playerVelocity / 1.75f;
			}
		}

		for (int i = 0; i < colliderBodyParts.Count; i++) {
			//if the collider is not trigger, set its state
			if (!colliderBodyParts [i].isTrigger) {
				//set the state of the colliders and rigidbodies inside the character to enable or disable them
				currentRigidbodyPart = colliderBodyParts [i].GetComponent<Rigidbody> ();
				if (currentRigidbodyPart != null) {
					currentRigidbodyPart.isKinematic = state;
					colliderBodyParts [i].enabled = !state;

					//change the layer of the colliders in the ragdoll, so the camera has not problems with it
					if (!usedByAI) {
						if (!state) {
							colliderBodyParts [i].gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
						} else {
							colliderBodyParts [i].gameObject.layer = LayerMask.NameToLayer ("Default");
						}
					} else {
						if (playerState != healthState.fallen) {
							if (!state) {
								colliderBodyParts [i].tag = tagForColliders;
							} else {
								colliderBodyParts [i].tag = "Untagged";
							}
						}
					}

					//if state== false, it means the player has dead, so get the position of the projectile that kills him,
					//and them the closest rigidbody of the character, to add velocity in the opposite direction to that part of the player
					if (!state) {
						if (playerState == healthState.fallen) {
							currentRigidbodyPart.velocity = playerVelocity / 1.75f;
						}

						if (pushAllBodyParts) {
							//currentRigidbodyPart.AddForce (currentRigidbodyPart.mass * damageDirection * (extraForceOnRagdoll/2), ForceMode.Impulse);
							currentRigidbodyPart.velocity = damageDirection * extraForceOnRagdoll / 2;
						} else {
							if (currentRigidbodyPart == closestPart) {
								
								print (damageVelocity + " " + currentRigidbodyPart.name + " " + damageVelocity.magnitude);
								currentRigidbodyPart.velocity = damageVelocity;
								//currentRigidbodyPart.AddForce (currentRigidbodyPart.mass * damageDirection * extraForceOnRagdoll, ForceMode.Impulse);
								//currentRigidbodyPart.AddForce (damageVelocity, ForceMode.Impulse);
							} else {
								if (playerState == healthState.dead) {
									currentRigidbodyPart.velocity = playerVelocity / 1.75f;
									//currentRigidbodyPart.AddForce (damageVelocity, ForceMode.Impulse);
								}
							}
						}
					}
				} else {
					Physics.IgnoreCollision (mainCollider, colliderBodyParts [i]);
				}
			}
			//if the collider is trigger, it is the foot trigger for the feetsteps sounds, so set the opposite state, to avoid play the sounds when
			//the player is dead
			else {
				colliderBodyParts [i].enabled = state;
			}
		}

		pushAllBodyParts = false;
	}

	public void setBodyColliderList ()
	{
		colliderBodyParts.Clear ();
		Component[] components = characterBody.GetComponentsInChildren (typeof(Collider));
		foreach (Component c in components) {
			Collider colliderPart = c.GetComponent<Collider> ();
			if (!colliderPart.isTrigger) {
				colliderBodyParts.Add (colliderPart);
			}
		}
	}

	public List<Collider> getBodyColliderList ()
	{
		return colliderBodyParts;
	}

	public void ignoreCollisionWithBodyColliderList (List<Collider> colliderList)
	{
		for (int i = 0; i < colliderBodyParts.Count; i++) {
			for (int j = 0; j < colliderList.Count; j++) {
				Physics.IgnoreCollision (colliderBodyParts [i], colliderList [j]);
			}
		}
	}

	public void setBodyRigidbodyList ()
	{
		rigidbodyBodyParts.Clear ();
		Component[] components = characterBody.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Component c in components) {
			Rigidbody rigidbodyPart = c.GetComponent<Rigidbody> ();
			rigidbodyBodyParts.Add (rigidbodyPart);
		}
	}

	public List<Rigidbody> getBodyRigidbodyList ()
	{
		return rigidbodyBodyParts;
	}

	//get the closest rigidbody to the projectile that killed the player, to add velocity with an opposite direction of the bullet
	Rigidbody searchClosestBodyPart ()
	{
		float distance = 100;
		Rigidbody part = new Rigidbody ();
		for (int i = 0; i < rigidbodyBodyParts.Count; i++) {
			float currentDistance = GKC_Utils.distance (rigidbodyBodyParts [i].transform.position, damagePos);
			if (currentDistance < distance) {
				distance = currentDistance;
				part = rigidbodyBodyParts [i];
			}
		}
		return part;
	}

	public bool checkChildsObjectsToIgnore (GameObject obj)
	{
		bool value = false;
		if (objectsToIgnore.Contains (obj)) {
			value = true;
			return value;
		}
		for (int i = 0; i < objectsToIgnoreChildren.Count; i++) {
			if (obj.transform.IsChildOf (objectsToIgnoreChildren [i].transform)) {
				value = true;
				return value;
			}
		}
		return value;
	}

	void setPlayerToRegularState ()
	{
		//set the parent of every object to back everything to the situation before the player died
		transform.position = characterBody.transform.position;
		transform.rotation = new Quaternion (0, characterBody.transform.rotation.y, 0, characterBody.transform.rotation.w);
		characterBody.transform.SetParent (playerCOM);

		mainCollider.enabled = false;
		mainCollider.enabled = true;

		enableBehaviour = true;

		if (cameraManager) {
			cameraManager.death (false, typeOfDeath == deathType.ragdoll);
		}
	
		lastTimeStateEnabled = Time.time;
	}

	public void setCharacterBody (GameObject newCharacterBody)
	{
		characterBody = newCharacterBody;
	}

	public void resetLastTimeMoved ()
	{
		if (playerControllerManager) {
			playerControllerManager.setLastTimeMoved ();
		}

		if (cameraManager) {
			cameraManager.setLastTimeMoved ();
		}

		if (powersManager) {
			powersManager.setLastTimeFired ();
		}

		if (weaponsManager) {
			weaponsManager.setLastTimeFired ();
		}
	}

	public List<bodyPart> getBodyPartsList ()
	{
		return bodyParts;
	}

	public void pushCharacter (Vector3 direction)
	{
		damagePos = hipsRigidbody.position;
		damageDirection = direction;
		damageToFall ();
	}

	public void pushFullCharacter (Vector3 direction)
	{
		pushAllBodyParts = true;
		pushCharacter (direction);
	}

	public Transform getRootMotion ()
	{
		return rootMotion;
	}

	Coroutine sedateCoroutine;

	public void sedateCharacter (float sedateDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		if (sedateCoroutine != null) {
			StopCoroutine (sedateCoroutine);
		}
		sedateCoroutine = StartCoroutine (setSedateStateCoroutine (sedateDelay, sedateUntilReceiveDamage, sedateDuration));
	}

	IEnumerator setSedateStateCoroutine (float sedateDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		if (!sedateActive) {
			yield return new WaitForSeconds (sedateDelay);
		}

		sedateUntilReceiveDamageState = sedateUntilReceiveDamage;
		currentSedateDuration = sedateDuration;
		lastTimeSedated = Time.time;
		sedateActive = true;
		damageToFall ();
		healthManager.setSedateState (true);
	}

	public void stopSedateStateCoroutine ()
	{
		if (sedateCoroutine != null) {
			StopCoroutine (sedateCoroutine);
		}
		sedateActive = false;
		sedateUntilReceiveDamageState = false;
		healthManager.setSedateState (false);
	}

	public void setImpactReceivedInfo (Vector3 impactVelocity, Collider impactCollider)
	{
		if (ragdollCanReceiveDamageOnImpact) {
			if (currentState == ragdollState.ragdolled) {
				if (!colliderBodyParts.Contains (impactCollider)) {
					if (Time.time > lastTimeRagdolled + minTimeToReceiveDamageOnImpact) {
						float currentImpactVelocity = impactVelocity.magnitude;
						//print (currentImpactVelocity);
						if (currentImpactVelocity > minVelocityToReceiveDamageOnImpact) {
							if (Time.time > lastTimeDamageOnImpact + minTimToReceiveImpactDamageAgain) {
								healthManager.takeConstantDamage (currentImpactVelocity * receiveDamageOnImpactMultiplier);
								//	print ("take " + currentImpactVelocity * receiveDamageOnImpactMultiplier);
								lastTimeDamageOnImpact = Time.time;
							}
						}
					}
				}
			}
		}
	}

	public void checkDrawWeaponsWhenResurrect ()
	{
		if (carryinWeaponsPreviously && weaponsManager.isDrawWeaponWhenResurrectActive () && weaponsManager.checkIfWeaponsAvailable ()) {
			weaponsManager.setWeaponsModeActive (true);
			carryinWeaponsPreviously = false;
		}
	}

	public void setCheckGetUpPausedState (bool state)
	{
		checkGetUpPaused = state;

		if (!checkGetUpPaused) {
			deadMenuTimer = timeToGetUp;
		}
	}

	[System.Serializable]
	public class bodyPart
	{
		public string name;
		public Transform transform;
		public Vector3 storedPosition;
		public Quaternion storedRotation;
	}
}