using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class laserDevice : laser
{
	public LayerMask layer;
	public bool enablePlayerShield = true;
	public bool assigned;
	public GameObject laserConnector;
	public laserType lasertype;
	public float damageAmount;
	public bool canDamagePlayer;
	public bool canDamageCharacters;
	public bool canDamageVehicles;
	public bool canDamageEverything;
	public bool canKillWithOneHit;

	public bool sendMessageOnContact;

	public UnityEvent contantFunctions = new UnityEvent ();

	GameObject currentPlayer;
	otherPowers currentPowersManager;
	bool forceFieldEnabled;
	RaycastHit hit;
	Vector3 hitPointPosition;
	float rayDistance;
	float hitDistance;
	bool hittingSurface;
	bool damageCurrentSurface;
	bool laserEnabled = true;

	public enum laserType
	{
		simple,
		refraction
	}

	GameObject lastObjectDetected;

	void Start ()
	{
		StartCoroutine (laserAnimation ());
		//get the initial raycast distance
		rayDistance = Mathf.Infinity;

	}

	void Update ()
	{
		if (laserEnabled) {
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);

			//check if the hitted object is the player, enabling or disabling his shield
			if (Physics.Raycast (transform.position, transform.forward, out hit, rayDistance, layer)) {
				//if the laser has been deflected, then check if any object collides with it, to disable all the other reflections of the laser
				hittingSurface = true;
				laserDistance = hit.distance;
				hitPointPosition = hit.point;
				if (hit.collider.gameObject != lastObjectDetected) {
					lastObjectDetected = hit.collider.gameObject;
					if (sendMessageOnContact) {
						if (contantFunctions.GetPersistentEventCount () > 0) {
							contantFunctions.Invoke ();
						}
					}
				}

			} else {
				//the laser does not hit anything, so disable the shield if it was enabled
				hittingSurface = false;
			}

			if (hittingSurface) {
				if (hit.transform.tag == "Player" && currentPlayer == null) {
					currentPlayer = hit.collider.gameObject;
					currentPowersManager = currentPlayer.GetComponent<otherPowers> ();
				}

				if (assigned) {
					forceFieldEnabled = false;
					if (enablePlayerShield) {
						currentPowersManager.deactivateLaserForceField ();
					}
					rayDistance = Mathf.Infinity;
					laserConnector.GetComponent<laserConnector> ().disableRefractionState ();
				} else {
					///the laser touchs the player, active his shield and set the laser that is touching him
					if (hit.transform.tag == "Player" && !hit.collider.isTrigger && !forceFieldEnabled) {
						currentPowersManager.setLaser (gameObject, lasertype);
						forceFieldEnabled = true;
					}
					if (forceFieldEnabled) {
						hitDistance = hit.distance;
						//set the position where this laser is touching the player
						Vector3 position = hit.point;
						if (enablePlayerShield) {
							currentPowersManager.activateLaserForceField (position);
						}
						//the laser has stopped to touch the player, so deactivate the player's shield
						if (hit.transform.tag != "Player") {
							forceFieldEnabled = false;
							if (enablePlayerShield) {
								currentPowersManager.deactivateLaserForceField ();
							}
						}
					}
				}

				if (canDamagePlayer && hit.transform.tag == "Player") {
					damageCurrentSurface = true;
				} 

				if (canDamageCharacters) {
					if (hit.transform.GetComponent<characterDamageReceiver> () || hit.transform.GetComponent<health> ()) {
						damageCurrentSurface = true;
					}
				}

				if (canDamageVehicles) {
					if (hit.transform.GetComponent<vehicleHUDManager> () || hit.transform.GetComponent<characterDamageReceiver> ()) {
						damageCurrentSurface = true;
					}
				}

				if (canDamageEverything) {
					damageCurrentSurface = true;
				}

				if (damageCurrentSurface) {
					if (canKillWithOneHit) {
						float remainingHealth = applyDamage.getCurrentHealthAmount (hit.transform.gameObject);
						applyDamage.checkHealth (gameObject, hit.transform.gameObject, remainingHealth, -transform.forward, hit.point, gameObject, true, true);
					} else {
						applyDamage.checkHealth (gameObject, hit.transform.gameObject, damageAmount, -transform.forward, hit.point, gameObject, true, true);
					}
				}
				lRenderer.SetPosition (1, hitPointPosition);
			} else {
				if (!assigned) {
					if (forceFieldEnabled) {
						forceFieldEnabled = false;
						if (enablePlayerShield) {
							currentPowersManager.deactivateLaserForceField ();
						}
						//set to infinite the raycast distance again
						rayDistance = Mathf.Infinity;
					}		
					laserDistance = 1000;	
					lRenderer.SetPosition (1, (laserDistance * transform.forward));
				}
			}

			animateLaser ();
		}
	}

	void OnDisable ()
	{
		if (assigned) {
			forceFieldEnabled = false;
			if (currentPlayer) {
				if (enablePlayerShield) {
					currentPowersManager.deactivateLaserForceField ();
				}
				//set to infinite the raycast distance again
				rayDistance = Mathf.Infinity;
				//disable the laser connector
				laserConnector.GetComponent<laserConnector> ().disableRefractionState ();
			}
		}
	}

	//set the laser that it is touching the player, to assign it to the laser connector
	void assignLaser ()
	{
		assigned = true;
		rayDistance = hitDistance;
		if (enablePlayerShield) {
			currentPowersManager.deactivateLaserForceField ();
		}
	}

	public void setAssignLaserState (bool state)
	{
		assigned = state;
	}

	public void disableLaser ()
	{
		laserEnabled = false;
		lRenderer.enabled = false;
	}
}