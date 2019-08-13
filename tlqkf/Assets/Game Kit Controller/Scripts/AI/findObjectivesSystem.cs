using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class findObjectivesSystem : MonoBehaviour
{
	public float timeToCheckSuspect;
	public List<GameObject> enemies = new List<GameObject> ();
	public List<GameObject> notEnemies = new List<GameObject> ();

	public List<GameObject> fullEnemyList = new List<GameObject> ();

	public LayerMask layerMask;
	public LayerMask layerToCheckTargets;
	public AIAttackType attackType;

	public float minDistanceToDraw;
	public float minDistanceToAim;
	public float minDistanceToShoot;

	public string functionToDrawWeapon;
	public string functionToKeepWeapon;
	public string functionToAimWeapon;
	public string functionToStartToShoot;

	public float minDistanceToMelee;
	public string functionToMelee;
	public float extraFieldOfViewRadiusOnSpot;
	public bool avoidEnemies;

	public float visionRange = 90;
	public float minDistanceToAdquireTarget = 2;
	public bool allowDetectionWhenTooCloseEvenNotVisible;
	public bool ignoreVisionRangeWhenGetUp;
	public float timeToIgnoreVisionRange = 1;
	public bool ignoreVisionRangeActive;
	float lastTimePaused;

	public bool canHearNoises = true;
	public LayerMask layerToCheckNoises;
	public float raycastDistanceToCheckNoises = 2;

	public bool checkRaycastToViewTarget = true;
	public LayerMask layerToCheckRaycastToViewTarget;

	public Transform rayCastPosition;
	public SphereCollider fovTrigger;

	public bool followPartnerOnTrigger = true;
	public string factionToFollowAsPartner = "Player";

	public enum AIAttackType
	{
		none,
		weapons,
		melee,
		both
	}

	public string surprisedCharacterStateName = "Surprised";
	public string wonderingCharacterStateName = "Wondering";

	public bool alertFactionOnSpotted;
	public float alertFactionRadius = 10;

	public bool useEventOnSpotted;
	public UnityEvent eventOnSpotted;
	public bool useEventOnNoEnemies;
	public UnityEvent eventOnNoEnemies;

	public bool onSpotted;
	public bool runningAway;
	public bool paused;

	public bool weaponEquiped;
	public bool aimingWeapon;

	public bool characterOnVehicle;

	public bool targetIsCharacter;
	public bool targetIsVehicle;
	public bool checkingThreat;
	public bool threatInfoStored;

	public bool searchingWeapon;
	public bool characterHasWeapons;

	public bool seeingCurrentTarget;

	public GameObject enemyToShoot;

	public GameObject posibleThreat;
	public GameObject partner;

	public bool showGizmo;
	public Color gizmoColor = Color.red;
	public float visionRangeGizmoRadius = 2;

	checkCollisionType viewTrigger;
	RaycastHit hit;
	float originalFOVRaduis;
	float timeToCheck = 0;
	float speedMultiplier = 1;
	float timeWithoutThreatFound;

	bool moveBack;
	float distanceToTarget;
	AINavMesh AINavMeshManager;
	Transform placeToShoot;
	float currentDistance;

	float originalMinDistanceToDraw;
	float originalMinDistanceToAim;
	float originalMinDistanceToShoot;
	float originalMinDistanceToMelee;

	AIPatrolSystem AIPatrolManager;

	characterFactionManager factionManager;
	playerController currentPlayerController;
	vehicleHUDManager currentVehicleHUDManager;
	vehicleHUDManager currentVehicleHUDManagerToGetOn;
	characterFactionManager characterFactionToCheck;
	playerController playerControllerPartner;
	usingDevicesSystem usingDevicesManager;
	playerController playerControllerManager;
	playerWeaponsManager weaponsManager;

	GameObject currentVehicle;
	GameObject previousEnemy;

	bool onGroundCheck;

	GameObject currentWeaponToGet;

	bool hasBeenAttacked;
	Vector3 targetDirection;
	bool enemyAlertActive;

	Vector3 currentPosition;
	Vector3 currentEnemyPosition;

	Transform emptyTargetPositionTransform;

	float lastTimeSpotted;

	bool targetIsDriving;

	void Start ()
	{
		originalFOVRaduis = fovTrigger.radius;
		viewTrigger = GetComponentInChildren<checkCollisionType> ();
	
		AINavMeshManager = GetComponent<AINavMesh> ();

		originalMinDistanceToDraw = minDistanceToDraw;
		originalMinDistanceToAim = minDistanceToAim;
		originalMinDistanceToShoot = minDistanceToShoot;
		originalMinDistanceToMelee = minDistanceToMelee;
		factionManager = GetComponent<characterFactionManager> ();
		AIPatrolManager = GetComponent<AIPatrolSystem> ();
		usingDevicesManager = GetComponent<usingDevicesSystem> ();
		playerControllerManager = GetComponent<playerController> ();
		weaponsManager = GetComponent<playerWeaponsManager> ();

		GameObject newEmptyTarget = new GameObject ();
		newEmptyTarget.name = "Empty Target Position Transform";
		emptyTargetPositionTransform = newEmptyTarget.transform;
		emptyTargetPositionTransform.SetParent (transform.parent);
	}

	public void getOffFromVehicle ()
	{
		if (characterOnVehicle) {
			if (playerControllerManager.canCharacterGetOnVehicles ()) {
				usingDevicesManager.useDevice ();
				AINavMeshManager.pauseAI (false);
			}
			currentVehicle = null;
			characterOnVehicle = false;
			setExtraMinDistanceState (false, 0);
			AINavMeshManager.setExtraMinDistanceState (false, 0);
		}
	}

	void Update ()
	{
		if (playerControllerPartner && !AINavMeshManager.isCharacterWaiting ()) {
			if (playerControllerPartner.isPlayerDriving ()) {
				if (!characterOnVehicle && !AINavMeshManager.isCharacterAttacking ()) {
					if (!currentVehicle) {
						currentVehicle = playerControllerPartner.getCurrentVehicle ();
						currentVehicleHUDManagerToGetOn = currentVehicle.GetComponent<vehicleHUDManager> ();
						setExtraMinDistanceState (true, currentVehicleHUDManagerToGetOn.getVehicleRadius ());
						AINavMeshManager.setExtraMinDistanceState (true, currentVehicleHUDManagerToGetOn.getVehicleRadius ());
						addNotEnemey (playerControllerPartner.gameObject);
					}
					if (!AINavMeshManager.isFollowingTarget () && !currentVehicleHUDManagerToGetOn.isVehicleFull ()) {
						if (playerControllerManager.canCharacterGetOnVehicles ()) {
							if (usingDevicesManager) {
								AINavMeshManager.pauseAI (true);
								usingDevicesManager.addDeviceToList (currentVehicle);
								usingDevicesManager.getclosestDevice ();
								usingDevicesManager.setCurrentVehicle (currentVehicle);
								usingDevicesManager.useDevice ();
								characterOnVehicle = true;
							}
						}
					}
				} else {
					if (AINavMeshManager.isCharacterAttacking ()) {
						getOffFromVehicle ();
					}
				}
			} else {
				getOffFromVehicle ();
			}
		}

		if (!paused) { 
			AINavMeshManager.setOnGroundState (playerControllerManager.isPlayerOnGround ());
			if (playerControllerManager.isPlayerOnGround ()) {
				if (!onGroundCheck) {
					onGroundCheck = true;
					//AINavMeshManager.setOnGroundState (true);
				}
			} else {
				if (onGroundCheck) {
					onGroundCheck = false;
					aimingWeapon = false;
					//AINavMeshManager.setOnGroundState (false);
				}
			}

			if (!searchingWeapon) {
				closestTarget ();
			} else {
				if (currentWeaponToGet) {
					return;
				}
				if (!currentWeaponToGet) {
					characterHasWeapons = weaponsManager.checkIfWeaponsAvailable ();
					searchingWeapon = false;
				}
			}

			if (ignoreVisionRangeActive) {
				if (Time.time > lastTimePaused + timeToIgnoreVisionRange) {
					ignoreVisionRangeActive = false;
				}
			}

			if (attackType != AIAttackType.none) {
				
				if (onSpotted) {
					lootAtTarget (placeToShoot);
					if (enemyToShoot) {
						if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, Mathf.Infinity, layerMask)) {
							if (hit.collider.gameObject == enemyToShoot || hit.collider.gameObject.transform.IsChildOf (enemyToShoot.transform)) {
								targetDirection = enemyToShoot.transform.position - transform.position;
								float angleWithTarget = Vector3.SignedAngle (transform.forward, targetDirection, enemyToShoot.transform.up);
								if (Mathf.Abs (angleWithTarget) < visionRange / 2 || ignoreVisionRangeActive) { 
									setAttackMode ();
								}
							}
						}
					}
				}

				//if the turret detects a target, it will check if it is an enemy, and this will take 2 seconds, while the enemy choose to leave or stay in the place
				else if (checkingThreat) {
					if (posibleThreat != null) {
						if (!placeToShoot) {
							//every object with a health component, has a place to be shoot, to avoid that a enemy shoots the player in his foot, so to center the shoot
							//it is used the gameObject placetoshoot in the health script
							if (applyDamage.checkIfDead (posibleThreat)) {
								cancelCheckSuspect (posibleThreat);
								return;
							} else {
								placeToShoot = applyDamage.getPlaceToShoot (posibleThreat);
							}
						}

						if (!threatInfoStored) {
							currentPlayerController = posibleThreat.GetComponent<playerController> ();
							if (currentPlayerController) {
								if (currentPlayerController.isCharacterVisibleToAI ()) {
									playerControllerManager.setCharacterStateIcon (wonderingCharacterStateName);
								}
							} else {
								playerControllerManager.setCharacterStateIcon (wonderingCharacterStateName);
							}

							//check if the current threat is hidden or not, to pause the patrol
							if (AIPatrolManager) {
								if (!AIPatrolManager.paused) {
									if (currentPlayerController.isCharacterVisibleToAI ()) {
										AINavMeshManager.setPatrolPauseState (true);
									}
								}
							} 
							threatInfoStored = true;
						} 

						if (placeToShoot) {
							//look at the target position
							lootAtTarget (placeToShoot);
						}

						//uses a raycast to check the posible threat
						if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, Mathf.Infinity, layerMask)) {
							if (hit.collider.gameObject == posibleThreat || hit.collider.gameObject.transform.IsChildOf (posibleThreat.transform)) {
								timeToCheck += Time.deltaTime * speedMultiplier;
							} else {
								timeWithoutThreatFound += Time.deltaTime * speedMultiplier;
							}

							//when the turret look at the target for a while, it will open fire 
							if (timeToCheck > timeToCheckSuspect) {
								timeToCheck = 0;
								checkingThreat = false;
								threatInfoStored = false;
								addEnemy (posibleThreat);
								posibleThreat = null;

								AINavMeshManager.setPatrolPauseState (false);

								playerControllerManager.disableCharacterStateIcon ();
							}

							if (timeWithoutThreatFound > timeToCheckSuspect) {
								resetCheckThreatValues ();
							}
						}
					}
				}
			}
		}
	}

	public void resetCheckThreatValues ()
	{
		placeToShoot = null;
		posibleThreat = null;
		checkingThreat = false;
		threatInfoStored = false;
		timeToCheck = 0;
		timeWithoutThreatFound = 0;

		AINavMeshManager.setPatrolPauseState (false);

		playerControllerManager.disableCharacterStateIcon ();
	}

	//follow the enemy position, to rotate torwards his direction
	void lootAtTarget (Transform objective)
	{
		Debug.DrawRay (rayCastPosition.position, rayCastPosition.forward, Color.red);
		if (objective != null) {
			Vector3 targetDir = objective.position - rayCastPosition.position;
			Quaternion targetRotation = Quaternion.LookRotation (targetDir, transform.up);
			rayCastPosition.rotation = Quaternion.Slerp (rayCastPosition.rotation, targetRotation, 10 * Time.deltaTime);
		}
	}

	public bool checkCharacterFaction (GameObject character, bool damageReceived)
	{
		//print (gameObject.name + " "+ character.name + " " + damageReceived);
		if (fullEnemyList.Contains (character)) {
			return true;
		}

		characterFactionToCheck = character.GetComponent<characterFactionManager> ();

		if (characterFactionToCheck) {
			bool isEnemy = false;
			if (damageReceived) {
				isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
			} else {
				isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
			}

			if (isEnemy) {
				return true;
			}
		} else {
			currentVehicleHUDManager = character.GetComponent<vehicleHUDManager> ();
			if (currentVehicleHUDManager) {
				if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
					GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();
					//print (currentDriver.name);
					if (!currentDriver) {
						return false;
					}

					characterFactionToCheck = currentDriver.GetComponent<characterFactionManager> ();

					if (characterFactionToCheck) {
						bool isEnemy = false;
						if (damageReceived) {
							isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
						} else {
							isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
						}
						if (isEnemy) {
							addEnemy (currentDriver);

							targetIsDriving = true;

							return true;
						}
					} 
				}
			}
		}

		return false;
	}

	//check if the object which has collided with the viewTrigger (the capsule collider in the head of the turret) is an enemy checking the tag of that object
	void checkSuspect (GameObject currentSuspect)
	{
		if (canCheckSuspect (currentSuspect)) {
			
			if (checkCharacterFaction (currentSuspect, false) && !onSpotted && !posibleThreat) {

				if (targetIsDriving || applyDamage.isVehicle (currentSuspect)) {
					targetIsDriving = false;
					targetIsVehicle = true;
				}

				if (!checkRaycastToViewTarget || checkIfTargetIsPhysicallyVisible (currentSuspect, true)) {
					posibleThreat = currentSuspect;
					checkingThreat = true;
				}
			}
		}
	}

	//in the object exits from the viewTrigger, the turret rotates again to search more enemies
	void cancelCheckSuspect (GameObject col)
	{
		if (checkCharacterFaction (col, false) && !onSpotted && posibleThreat) {
			resetCheckThreatValues ();
		}
	}

	//the sphere collider with the trigger of the turret has detected an enemy, so it is added to the list of enemies
	void enemyDetected (GameObject col)
	{
		if (checkCharacterFaction (col, false)) {
			addEnemy (col.gameObject);
		}
	}

	//one of the enemies has left, so it is removed from the enemies list
	void enemyLost (GameObject enemyToRemove)
	{
		//if (onSpotted) {
		removeEnemy (enemyToRemove);
		//}
	}

	void enemyAlert (GameObject target)
	{
		enemyDetected (target);
		enemyAlertActive = true;
	}

	//if anyone shoot this character, increase its field of view to search any enemy close to it
	public void checkShootOrigin (GameObject attacker)
	{
		if (!onSpotted) {
			if (checkCharacterFaction (attacker, true)) {
				addEnemy (attacker);
				factionManager.addDetectedEnemyFromFaction (attacker);
				hasBeenAttacked = true;
			}
		}
	}

	//add an enemy to the list, checking that that enemy is not already in the list
	void addEnemy (GameObject enemy)
	{
		if (!enemies.Contains (enemy)) {
			enemies.Add (enemy);
			if (!fullEnemyList.Contains (enemy)) {
				fullEnemyList.Add (enemy);
			}

			if (partner) {
				if (fullEnemyList.Contains (partner)) {
					AINavMeshManager.removePartner ();
					partner = null;
					playerControllerPartner = null;
					getOffFromVehicle ();
				}
			}
		}
	}

	//remove an enemy from the list
	void removeEnemy (GameObject enemy)
	{
		//remove this enemy from the faction system detected enemies for the faction of this character
		factionManager.removeDetectedEnemyFromFaction (enemy);
	
		enemies.Remove (enemy);
	}

	void addNotEnemey (GameObject notEnemy)
	{
		if (!notEnemies.Contains (notEnemy)) {
			characterFactionToCheck = notEnemy.GetComponent<characterFactionManager> ();
			if (characterFactionToCheck) {
				notEnemies.Add (notEnemy);
			}
		}
	}

	void removeNotEnemy (GameObject notEnemy)
	{
		if (notEnemies.Contains (notEnemy)) {
			notEnemies.Remove (notEnemy);
		}
	}

	//when there is one enemy or more, check which is the closest to shoot it.
	void closestTarget ()
	{
		if (enemies.Count > 0) {
			currentPosition = transform.position;
			float min = Mathf.Infinity;
			int index = -1;
			for (int i = 0; i < enemies.Count; i++) {
				if (enemies [i]) {
					currentDistance = GKC_Utils.distance (enemies [i].transform.position, currentPosition);
					if (currentDistance < min) {
						min = currentDistance;
						index = i;
						distanceToTarget = min;
					}
				} else {
					enemies.RemoveAt (i);
				}
			}

			if (index < 0) {
				return;
			}

			enemyToShoot = enemies [index];
			currentEnemyPosition = enemyToShoot.transform.position;

			placeToShoot = applyDamage.getPlaceToShoot (enemyToShoot);

			if (applyDamage.checkIfDead (enemyToShoot)) {
				removeEnemy (enemyToShoot);
				return;
			}

			currentPlayerController = enemyToShoot.GetComponent<playerController> ();
			if (currentPlayerController) {
				if (currentPlayerController.isPlayerDriving ()) {
					removeEnemy (enemyToShoot);
					targetIsCharacter = false;
					return;
				} else {
					targetIsCharacter = true;
					targetIsVehicle = false;
				}
			} else {
				currentVehicleHUDManager = enemyToShoot.GetComponent<vehicleHUDManager> ();
				if (currentVehicleHUDManager) {
					if (!currentVehicleHUDManager.isVehicleBeingDriven ()) {
						removeEnemy (enemyToShoot);
						return;
					} else {
						targetIsCharacter = false;
						targetIsVehicle = true;
					}
				} else {
					targetIsCharacter = false;
				}
			}

			if (previousEnemy != enemyToShoot) {
				previousEnemy = enemyToShoot;
				if (targetIsCharacter) {
					setExtraMinDistanceState (false, 0);
					AINavMeshManager.setExtraMinDistanceState (false, 0);
				} 
				if (targetIsVehicle) {
					setExtraMinDistanceState (true, currentVehicleHUDManager.getVehicleRadius ());
					AINavMeshManager.setExtraMinDistanceState (true, currentVehicleHUDManager.getVehicleRadius ());
				}
			}

			if (!onSpotted) {
				//the player can hack the turrets, but for that he has to crouch, so he can reach the back of the turret and activate the panel
				// if the player fails in the hacking or he gets up, the turret will detect the player and will start to fire him
				//check if the player fails or get up
				seeingCurrentTarget = false;
				if (checkRaycastToViewTarget) {

					seeingCurrentTarget = checkIfTargetIsPhysicallyVisible (enemyToShoot, false);
				} else {
					seeingCurrentTarget = true;
				}

				if (seeingCurrentTarget) {
					//if an enemy is inside the trigger, check its position with respect the AI, if the target is in the vision range, adquire it as target
					targetDirection = currentEnemyPosition - currentPosition;
					float angleWithTarget = Vector3.SignedAngle (transform.forward, targetDirection, enemyToShoot.transform.up);

					if (Mathf.Abs (angleWithTarget) < visionRange / 2 || hasBeenAttacked || enemyAlertActive || ignoreVisionRangeActive) { 
						if (currentPlayerController) {
							if ((!currentPlayerController.isCrouching () || checkingThreat || hasBeenAttacked || enemyAlertActive) && currentPlayerController.isCharacterVisibleToAI ()) {
								hasBeenAttacked = false;
								enemyAlertActive = false;
								targetAdquired ();
							}
						}
					//else, the target is a friend of the player, so shoot him
					else {
							targetAdquired ();
						}
					} else {
						//else check the distance, if the target is too close, adquire it as target too
						float distanceToTarget = GKC_Utils.distance (currentEnemyPosition, currentPosition);
						if (distanceToTarget < minDistanceToAdquireTarget && (currentPlayerController.isCharacterVisibleToAI () || allowDetectionWhenTooCloseEvenNotVisible)) {
							targetAdquired ();
						}
						//print ("out of range of vision");
					}
				}
			}

			if (onSpotted) {
				if (avoidEnemies) {
					AINavMeshManager.avoidTarget (enemyToShoot.transform);
					AINavMeshManager.setAvoidTargetState (true);
					runningAway = true;
					onSpotted = true;

					checkEventOnSpottedStateChange ();
				} else {
					AINavMeshManager.setTarget (enemyToShoot.transform);
					AINavMeshManager.setTargetType (false, false);
				}
				AINavMeshManager.setPatrolState (false);
			}
		} 

		//if there are no enemies
		else {
			if (onSpotted || runningAway) {
				placeToShoot = null;
				enemyToShoot = null;
				previousEnemy = null;
				onSpotted = false;

				lastTimeSpotted = Time.time;

				checkEventOnSpottedStateChange ();

				fovTrigger.radius = originalFOVRaduis;
				viewTrigger.gameObject.SetActive (true);
				AINavMeshManager.removeTarget ();

				if (avoidEnemies) {
					AINavMeshManager.setAvoidTargetState (false);
					runningAway = false;
				}

				if (attackType == AIAttackType.weapons) {
					keepWeapon ();
				}

				//stop the character from look towards his target
				AINavMeshManager.lookAtTaget (false);

				if (partner) {
					AINavMeshManager.setTarget (partner.transform);
					AINavMeshManager.setPatrolState (false);
					AINavMeshManager.setTargetType (true, false);
				} else {
					if (AIPatrolManager) {
						AIPatrolManager.setClosestWayPoint ();
					}
				}
			}
		}
	}

	Vector3 targetToCheckDirection;
	Transform temporalPlaceToShoot;
	vehicleHUDManager temporalVehicleHUDManagerToCheck;

	public bool checkIfTargetIsPhysicallyVisible (GameObject targetToCheck, bool checkingSuspect)
	{
		if (placeToShoot) {
			targetToCheckDirection = placeToShoot.position - rayCastPosition.position;
		} else {
			temporalPlaceToShoot = applyDamage.getPlaceToShoot (targetToCheck);
			if (temporalPlaceToShoot) {
				targetToCheckDirection = temporalPlaceToShoot.position - rayCastPosition.position;
			} else {
				targetToCheckDirection = (targetToCheck.transform.position + targetToCheck.transform.up) - rayCastPosition.position;
			}
		}

		if (targetIsVehicle) {
			temporalVehicleHUDManagerToCheck = applyDamage.getVehicleHUDManager (targetToCheck);
		}

		if (Physics.Raycast (rayCastPosition.position, targetToCheckDirection, out hit, Mathf.Infinity, layerToCheckRaycastToViewTarget)) {
			//print (hit.collider.gameObject.name + " " + targetIsVehicle);
			//print (hit.collider.name + " " + hit.transform.IsChildOf (targetToCheck.transform) + " " + targetToCheck.name);
		
			if ((!targetIsVehicle && (hit.collider.gameObject == targetToCheck || hit.transform.IsChildOf (targetToCheck.transform))) ||
			    (targetIsVehicle && temporalVehicleHUDManagerToCheck &&
			    ((temporalVehicleHUDManagerToCheck.gameObject == targetToCheck && checkingSuspect) || temporalVehicleHUDManagerToCheck.checkIfDetectSurfaceBelongToVehicle (hit.collider)))) {

				Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.green, 2);
				return true;
			} else {
				Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.red, 2);
				return false;
			}
		} else {
			Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.black, 2);
		}

		return false;
	}

	public void setAttackMode ()
	{
		if (attackType == AIAttackType.weapons) {

			if (characterHasWeapons) {

				if (!aimingWeapon && distanceToTarget <= minDistanceToAim) {
					if (weaponsManager.currentWeaponWithHandsInPosition () && weaponsManager.isPlayerCarringWeapon () && !weaponsManager.currentWeaponIsMoving ()) {
						aimWeapon ();
					}
				}

				if (!weaponEquiped && distanceToTarget <= minDistanceToDraw) {
					drawWeapon ();
				}

				if (weaponEquiped && aimingWeapon && distanceToTarget <= minDistanceToShoot) {
					if (!weaponsManager.currentWeaponIsMoving ()) {
						shootTarget ();
					}
				}
			} else {
				if (!searchingWeapon) {
					characterHasWeapons = weaponsManager.checkIfWeaponsAvailable ();
					//seach for the closest weapon
					if (!characterHasWeapons) {
						searchingWeapon = true;
						bool weaponFound = false;
						pickUpObject[] pickupList = FindObjectsOfType (typeof(pickUpObject)) as pickUpObject[];
						for (int i = 0; i < pickupList.Length; i++) {
							if (!weaponFound && pickupList [i].pickType == pickUpObject.pickUpType.weapon) {
								if (pickupList [i].secondaryString != "") {
									if (weaponsManager.checkIfWeaponCanBePicked (pickupList [i].secondaryString)) {
										currentWeaponToGet = pickupList [i].getPickupTrigger ().gameObject;
										AINavMeshManager.setTarget (pickupList [i].transform);
										AINavMeshManager.setTargetType (false, true);
										weaponFound = true;
										AINavMeshManager.lookAtTaget (false);
										//print (pickupList [i].secondaryString);
									}
								}
							}
						}

						//it will need to check if the weapon can be seen by the character and if it is can be reached by the navmesh
					}
				}
			}
		} else if (attackType == AIAttackType.melee) {
			if (distanceToTarget <= minDistanceToMelee) {
				meleeAttack ();
			}
		}
	}

	public void targetAdquired ()
	{
		onSpotted = true;

		checkEventOnSpottedStateChange ();

		fovTrigger.radius = GKC_Utils.distance (enemyToShoot.transform.position, transform.position) + extraFieldOfViewRadiusOnSpot;
		viewTrigger.gameObject.SetActive (false);

		//make the character to look towards his target
		AINavMeshManager.lookAtTaget (true);

		//send this enemy to faction system for the detected enemies list
		factionManager.addDetectedEnemyFromFaction (enemyToShoot);

		playerControllerManager.setCharacterStateIcon (surprisedCharacterStateName);

		if (alertFactionOnSpotted) {
			factionManager.alertFactionOnSpotted (alertFactionRadius, enemyToShoot, transform.position);
		}
	}

	public void keepWeapon ()
	{
		SendMessage (functionToKeepWeapon, false, SendMessageOptions.DontRequireReceiver);
		weaponEquiped = false;
		aimingWeapon = false;
	}

	public void drawWeapon ()
	{
		SendMessage (functionToDrawWeapon, true, SendMessageOptions.DontRequireReceiver);
		weaponEquiped = true;
	}

	public void aimWeapon ()
	{
		SendMessage (functionToAimWeapon, true, SendMessageOptions.DontRequireReceiver);
		aimingWeapon = true;
	}

	//active the fire mode
	public void shootTarget ()
	{
		SendMessage (functionToStartToShoot, true, SendMessageOptions.DontRequireReceiver);
	}

	public void meleeAttack ()
	{
		SendMessage (functionToMelee, SendMessageOptions.DontRequireReceiver);
	}

	public void pauseAction (bool state)
	{
		paused = state;
		resetAttackState ();

		if (ignoreVisionRangeWhenGetUp) {
			ignoreVisionRangeActive = true;
			lastTimePaused = Time.time;
		}
	}

	public void checkObject (GameObject objectToCheck)
	{
		Collider currentCollider = objectToCheck.GetComponent<Collider> ();

		if (currentCollider) {
			OnTriggerEnter (currentCollider);
		}
	}

	public void addPlayerAsPartner (GameObject objectToCheck)
	{
		followPartnerOnTrigger = true;
		checkObject (objectToCheck);
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public bool canCheckSuspect (GameObject currentSuspect)
	{
		if ((1 << currentSuspect.layer & layerToCheckTargets.value) == 1 << currentSuspect.layer) {
			return true;
		}
		return false;
	}

	GameObject objectToCheck;

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (canCheckSuspect (col.gameObject)) {
			if (isEnter) {
				if (!paused) {
					if (checkCharacterFaction (col.gameObject, false)) {
						enemyDetected (col.gameObject);
					} else {
						addNotEnemey (col.gameObject);

						objectToCheck = col.gameObject;

						if (currentVehicleHUDManager) {
							if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
								GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();
								if (currentDriver) {
									objectToCheck = currentDriver;
								}
							}
						}

						if (factionManager.checkIfCharacterBelongsToFaction (factionToFollowAsPartner, objectToCheck) && !partner) {
							if (!checkCharacterFaction (objectToCheck, false)) {
								if (followPartnerOnTrigger) {
									partner = objectToCheck;
									playerControllerPartner = partner.GetComponent<playerController> ();
									AINavMeshManager.partnerFound (partner.transform);
									AINavMeshManager.setTargetType (true, false);
								}
							} else {
								removeNotEnemy (objectToCheck);
								enemyDetected (objectToCheck);
							}
						}
					}
				}
			} else {
				if (checkCharacterFaction (col.gameObject, false)) {
					enemyLost (col.gameObject);
				} else {
					removeNotEnemy (col.gameObject);
				}
			}
		}
	}

	public void checkNoisePosition (Vector3 noisePosition)
	{
		if (!onSpotted && !partner && canHearNoises && Time.time > lastTimeSpotted + 2) {
			Ray newRay = new Ray ();
			newRay.direction = -Vector3.up;
			newRay.origin = noisePosition + Vector3.up;

			RaycastHit[] hits = Physics.RaycastAll (newRay, raycastDistanceToCheckNoises, layerToCheckNoises);
			System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

			bool surfaceFound = false;
			foreach (RaycastHit rh in hits) {
				print (rh.collider.name);
				if (!surfaceFound) {
					Rigidbody currenRigidbodyFound = applyDamage.applyForce (rh.collider.gameObject);
					if (!currenRigidbodyFound) {
						emptyTargetPositionTransform.position = rh.point + rh.normal * 0.3f;
						AINavMeshManager.setTarget (emptyTargetPositionTransform);
						AINavMeshManager.setTargetType (false, true);
						surfaceFound = true;
					}
				}
			}
		}
	}

	public void resetAttackState ()
	{
		weaponEquiped = false;
		aimingWeapon = false;
	}

	public void setExtraMinDistanceState (bool state, float extraDistance)
	{
		if (state) {
			minDistanceToDraw =	originalMinDistanceToDraw + extraDistance;
			minDistanceToAim = originalMinDistanceToAim + extraDistance;
			minDistanceToShoot = originalMinDistanceToShoot + extraDistance;
			minDistanceToMelee = originalMinDistanceToMelee + extraDistance;
		} else {
			minDistanceToDraw =	originalMinDistanceToDraw;
			minDistanceToAim = originalMinDistanceToAim;
			minDistanceToShoot = originalMinDistanceToShoot;
			minDistanceToMelee = originalMinDistanceToMelee;
		}
	}

	public void checkCharactersAroundAI ()
	{
		for (int i = 0; i < notEnemies.Count; i++) {
			enemyDetected (notEnemies [i]);
		}
	}

	public bool isSearchingWeapon ()
	{
		return searchingWeapon;
	}

	public void checkEventOnSpottedStateChange ()
	{
		if (onSpotted) {
			if (useEventOnSpotted) {
				eventOnSpotted.Invoke ();
			}
		} else {
			if (useEventOnNoEnemies) {
				eventOnNoEnemies.Invoke ();
			}
		}
	}

	public void searchEnemiesAround ()
	{
		enemies.Clear ();
		notEnemies.Clear ();
		closestTarget ();
		fovTrigger.enabled = false;
		fovTrigger.enabled = true;
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

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;

			Gizmos.DrawWireSphere (transform.position, alertFactionRadius);
		}
	}
}