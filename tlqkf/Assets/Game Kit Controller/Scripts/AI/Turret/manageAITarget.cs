using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class manageAITarget : MonoBehaviour
{
	public float timeToCheckSuspect;
	public List<GameObject> enemies = new List<GameObject> ();
	public List<GameObject> notEnemies = new List<GameObject> ();

	public List<GameObject> fullEnemyList = new List<GameObject> ();

	public LayerMask layerMask;
	public LayerMask layerToCheckTargets;

	public string functionToShoot;

	public float extraFieldOfViewRadiusOnSpot;

	public Transform rayCastPosition;
	public SphereCollider fovTrigger;

	public GameObject hackDevice;
		
	public bool checkRaycastToViewTarget = true;
	public LayerMask layerToCheckRaycastToViewTarget;

	public string factionToChangeName = "Friend Turrets";

	public string surprisedCharacterStateName = "Surprised";
	public string wonderingCharacterStateName = "Wondering";

	public bool alertFactionOnSpotted;
	public float alertFactionRadius = 10;

	public float visionRange = 90;
	public float minDistanceToAdquireTarget = 2;
	public bool allowDetectionWhenTooCloseEvenNotVisible;

	public bool onSpotted;
	public bool paused;
	public bool hacking;
	public bool checkingThreat;
	public bool targetIsCharacter;
	public bool targetIsVehicle;
	public bool threatInfoStored;

	public bool seeingCurrentTarget;

	public GameObject enemyToShoot;

	public GameObject posibleThreat;

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	float currentDistance;

	public Transform placeToShoot;

	checkCollisionType viewTrigger;
	RaycastHit hit;
	float originalFOVRaduis;
	float timeToCheck = 0;
	float speedMultiplier = 1;

	bool hackFailed;

	characterFactionManager factionManager;
	playerController currentPlayerController;
	vehicleHUDManager currentVehicleHUDManager;
	characterFactionManager characterFactionToCheck;
	characterStateIconSystem characterStateIconManager;

	bool hasBeenAttacked;
	Vector3 targetDirection;
	bool enemyAlertActive;

	float timeWithoutThreatFound;

	Vector3 currentPosition;

	bool targetIsDriving;

	void Start ()
	{
		originalFOVRaduis = fovTrigger.radius;
		viewTrigger = GetComponentInChildren<checkCollisionType> ();
		if (tag == "friend") {
			hackDevice.SetActive (false);
		}
		factionManager = GetComponent<characterFactionManager> ();
		characterStateIconManager = GetComponentInChildren<characterStateIconSystem> ();
	}

	void Update ()
	{
		if (!hacking && !paused) { 
			closestTarget ();
			if (onSpotted) {
				lootAtTarget (placeToShoot);
				if (enemyToShoot) {
					if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, Mathf.Infinity, layerMask)) {
						if (hit.collider.gameObject == enemyToShoot || hit.collider.gameObject.transform.IsChildOf (enemyToShoot.transform)) {
							targetDirection = enemyToShoot.transform.position - transform.position;
							float angleWithTarget = Vector3.SignedAngle (rayCastPosition.forward, targetDirection, enemyToShoot.transform.up);
							if (Mathf.Abs (angleWithTarget) < visionRange / 2) { 
								shootTarget ();
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
								setCharacterStateIcon (wonderingCharacterStateName);
							}
						} else {
							setCharacterStateIcon (wonderingCharacterStateName);
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
							addEnemy (posibleThreat);
							posibleThreat = null;

							disableCharacterStateIcon ();
						}

						if (timeWithoutThreatFound > timeToCheckSuspect) {
							resetCheckThreatValues ();
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
		timeToCheck = 0;
		timeWithoutThreatFound = 0;

		disableCharacterStateIcon ();
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
			return isEnemy;
		} else {
			currentVehicleHUDManager = character.GetComponent<vehicleHUDManager> ();
			if (currentVehicleHUDManager) {
				if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
					GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();
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

						targetIsDriving = true;

						return isEnemy;
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

				if (targetIsDriving || applyDamage.isVehicle(currentSuspect)) {
					targetIsDriving = false;
					targetIsVehicle = true;
				}

				if (!checkRaycastToViewTarget || checkIfTargetIsPhysicallyVisible (currentSuspect, true)) {
					posibleThreat = currentSuspect;
					checkingThreat = true;
					hacking = false;
				}
			}
		}
	}

	//in the object exits from the viewTrigger, the turret rotates again to search more enemies
	void cancelCheckSuspect (GameObject col)
	{
		if (checkCharacterFaction (col, false) && !onSpotted && posibleThreat) {
			placeToShoot = null;
			posibleThreat = null;
			checkingThreat = false;
			timeToCheck = 0;
			SendMessage ("cancelCheckSuspectTurret");

			disableCharacterStateIcon ();
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
	void enemyLost (GameObject col)
	{
		//if (onSpotted) {
		removeEnemy (col.gameObject);
		//}
	}

	void enemyAlert (GameObject target)
	{
		enemyDetected (target);
		enemyAlertActive = true;
	}

	//if anyone shoot the turret, increase its field of view to search any enemy close to it
	void checkShootOrigin (GameObject attacker)
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
					}
				} else {
					enemies.RemoveAt (i);
				}
			}

			if (index < 0) {
				return;
			}

			enemyToShoot = enemies [index];
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

			if (!onSpotted) {
				//the player can hack the turrets, but for that he has to crouch, so he can reach the back of the turret and activate the panel
				// if the player fails in the hacking or he gets up, the turret will detect the player and will start to fire him
				//check if the player fails or get up

				seeingCurrentTarget = false;
				if (checkRaycastToViewTarget) {
					
					seeingCurrentTarget = checkIfTargetIsPhysicallyVisible(enemyToShoot, false);
						
				} else {
					seeingCurrentTarget = true;
				}

				if (seeingCurrentTarget) {
					//if an enemy is inside the trigger, check its position with respect the AI, if the target is in the vision range, adquire it as target
					targetDirection = enemyToShoot.transform.position - currentPosition;
					float angleWithTarget = Vector3.SignedAngle (rayCastPosition.forward, targetDirection, enemyToShoot.transform.up);

					if (Mathf.Abs (angleWithTarget) < visionRange / 2 || hasBeenAttacked || enemyAlertActive) { 
						if (currentPlayerController) {
							if ((!currentPlayerController.isCrouching () || hackFailed || checkingThreat || enemyAlertActive || hasBeenAttacked) && currentPlayerController.isCharacterVisibleToAI ()) {
								hasBeenAttacked = false;
								enemyAlertActive = false;
								hackFailed = false;
								targetAdquired ();
							}
						} else {
							//else, the target is a friend of the player, so shoot him
							targetAdquired ();
						}
					} else {
						//else check the distance, if the target is too close, adquire it as target too
						float distanceToTarget = GKC_Utils.distance (enemyToShoot.transform.position, currentPosition);
						if (distanceToTarget < minDistanceToAdquireTarget && (currentPlayerController.isCharacterVisibleToAI () || allowDetectionWhenTooCloseEvenNotVisible)) {
							targetAdquired ();
						}
						//print ("out of range of vision");
					}
				}
			}
		} 

		//if there are no enemies
		else {
			if (onSpotted) {
				placeToShoot = null;
				enemyToShoot = null;
				//previousEnemyToShoot = null;
				onSpotted = false;
				fovTrigger.radius = originalFOVRaduis;
				viewTrigger.gameObject.SetActive (true);
				hackFailed = false;
				SendMessage ("setOnSpottedState", false);
			}
		}
	}
		
	Vector3 targetToCheckDirection;
	Transform temporalPlaceToShoot;
	Vector3 targetToCheckPosition;
	Vector3 targetToCheckRaycastPosition;
	vehicleHUDManager temporalVehicleHUDManagerToCheck;

	public float checkIfTargetIsPhysicallyVisibleRadius = 0.2f;

	public bool checkIfTargetIsPhysicallyVisible (GameObject targetToCheck, bool checkingSuspect)
	{
		targetToCheckRaycastPosition = transform.position + transform.up;

		if (placeToShoot) {
			targetToCheckDirection = placeToShoot.transform.position - targetToCheckRaycastPosition;
		} else {
			temporalPlaceToShoot = applyDamage.getPlaceToShoot (targetToCheck);
		
			if (temporalPlaceToShoot) {
				targetToCheckDirection = temporalPlaceToShoot.position - targetToCheckRaycastPosition;
			} else {
				targetToCheckDirection = (targetToCheck.transform.position + targetToCheck.transform.up) - targetToCheckRaycastPosition;
			}
		}

		targetToCheckPosition = targetToCheckRaycastPosition + targetToCheckDirection * checkIfTargetIsPhysicallyVisibleRadius;

		if (targetIsVehicle) {
			temporalVehicleHUDManagerToCheck = applyDamage.getVehicleHUDManager (targetToCheck);
		}

		//Debug.DrawRay (targetToCheckPosition, targetToCheckDirection * 0.2f, Color.green);
		if (Physics.Raycast (targetToCheckPosition, targetToCheckDirection, out hit, Mathf.Infinity, layerToCheckRaycastToViewTarget)) {
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

	public void targetAdquired ()
	{
		//print ("target adquired");

		onSpotted = true;
		fovTrigger.radius = GKC_Utils.distance (enemyToShoot.transform.position, transform.position) + extraFieldOfViewRadiusOnSpot;
		viewTrigger.gameObject.SetActive (false);
		SendMessage ("setOnSpottedState", true);

		//send this enemy to faction system for the detected enemies list
		factionManager.addDetectedEnemyFromFaction (enemyToShoot);

		setCharacterStateIcon (surprisedCharacterStateName);

		if (alertFactionOnSpotted) {
			factionManager.alertFactionOnSpotted (alertFactionRadius, enemyToShoot, transform.position);
		}
	}

	//active the fire mode
	public void shootTarget ()
	{
		//SendMessage (functionToShoot, true);
	}

	public void pauseAI (bool state)
	{
		paused = state;
		//SendMessage ("setPauseState", paused);
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkPossibleTarget (GameObject objectToCheck)
	{
		Collider currentCollider = objectToCheck.GetComponent<Collider> ();
		if (currentCollider) {
			checkTriggerInfo (currentCollider, true);
		}
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (canCheckSuspect (col.gameObject)) {
			if (isEnter) {
				if (!paused) {
					if (checkCharacterFaction (col.gameObject, false)) {
						enemyDetected (col.gameObject);
					} else {
						addNotEnemey (col.gameObject);
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

	public bool canCheckSuspect (GameObject currentSuspect)
	{
		if ((1 << currentSuspect.layer & layerToCheckTargets.value) == 1 << currentSuspect.layer) {
			return true;
		}
		return false;
	}

	public void setCorrectlyHackedState ()
	{
		setHackResult (true);
	}

	public void setIncorrectlyHackedState ()
	{
		setHackResult (false);
	}

	//check the result of the hacking, true the turret now is an ally, else, the turret detects the player
	public void setHackResult (bool state)
	{
		hacking = false;
		if (state) {
			hackDevice.GetComponent<enemyHackPanel> ().disablePanelHack ();
			tag = "friend";
			factionManager.changeCharacterToFaction (factionToChangeName);

			//if the turret becomes an ally, change its icon color in the radar
			mapObjectInformation currentMapObjectInformation = GetComponent<mapObjectInformation> ();
			if (currentMapObjectInformation) {
				currentMapObjectInformation.addMapObject ("Friend");
			}
			//set in the health slider the new name and slider color
			GetComponent<health> ().hacked ();

			enemies.Clear ();
			notEnemies.Clear ();
			fullEnemyList.Clear ();
		} else {
			hackFailed = true;
		}
	}

	//the turret is been hacked
	public void activateHack ()
	{
		hacking = true;
	}

	public void checkCharactersAroundAI ()
	{
		for (int i = 0; i < notEnemies.Count; i++) {
			enemyDetected (notEnemies [i]);
		}
	}

	public void setCharacterStateIcon (string stateName)
	{
		if (characterStateIconManager) {
			characterStateIconManager.setCharacterStateIcon (stateName);
		}
	}

	public void disableCharacterStateIcon ()
	{
		if (characterStateIconManager) {
			characterStateIconManager.disableCharacterStateIcon ();
		}
	}

	//disable ai when it dies
	public void setAIStateToDead ()
	{
		enabled = false;
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