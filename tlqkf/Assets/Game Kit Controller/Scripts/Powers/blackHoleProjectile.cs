using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class blackHoleProjectile : projectileSystem
{
	public float stopBulletTimer = 1;
	public float blackHoleTimer = 10;
	public float getObjectsRadius = 40;
	public float blackHoleAttractionForce = 150;
	public GameObject openingBlackHoleParticles;
	public GameObject closingBlackHoleParticles;
	public List<string> tagToLocate = new List<string>();
	public List<GameObject> elementsToAbsorve = new List<GameObject>();
	public float minDistanceToStopAttraction = 4;

	public float timeToOpenBlackHoleAfterShoot = 0.5f;
	public float timeToActivateParticles = 4;
	public float timeToActivateOpenParticles = 3.5f;
	public float timeToCloseBlackHole = 0.5f;

	public float timeToDestroyParticles = 6;

	public float damageRate = 0.5f;

	bool abilityActivated;
	bool stopBullet;
	bool openingBlackHoleParticlesCreated;
	bool closingBlackHoleParticlesCreated;
	Vector3 blackHoleDirection;
	Vector3 blackHoleScale;
	float lastTimeDamage;

	bool canDamage;

	Vector3 currentPosition;

	List<Collider> colliders = new List<Collider>();
	Rigidbody currentRigidbody;

	void Update ()
	{
		if (abilityActivated) {

			currentPosition = transform.position;

			//black hole
			//when the bullet touchs a surface, or the timer reachs the limit, set the bullet kinematic, and activate the black hole
			if (blackHoleTimer > timeToOpenBlackHoleAfterShoot && mainRigidbody.isKinematic) {
				if (!openingBlackHoleParticlesCreated) {
					openingBlackHoleParticles = creatParticles (openingBlackHoleParticles, timeToDestroyParticles);
					openingBlackHoleParticlesCreated = true;
				}

				//get all the objects inside a radius
				if (colliders.Count == 0) {
					colliders.AddRange (Physics.OverlapSphere (currentPosition, getObjectsRadius, currentProjectileInfo.targetToDamageLayer));
					foreach (Collider hit in colliders) {
						if (applyDamage.checkIfCharacterCanBePushedOnExplosions (hit.gameObject)) {
							Vector3 explosionDirection = currentPosition - hit.gameObject.transform.position;
							explosionDirection = explosionDirection / explosionDirection.magnitude;
							applyDamage.pushRagdoll (hit.gameObject, explosionDirection);
						}
					}

					colliders.Clear ();
					colliders.AddRange (Physics.OverlapSphere (currentPosition, getObjectsRadius, currentProjectileInfo.targetToDamageLayer));

					foreach (Collider hit in colliders) {
						if (hit != null) {
							GameObject currentGameObject = hit.gameObject;
							bool isCharacterOrVehicle = false;
							bool isRigidbody = false;
							GameObject target = applyDamage.getCharacterOrVehicle (currentGameObject);
							if (target != null) {
								currentGameObject = target;
								isCharacterOrVehicle = true;

							} else {
								if (applyDamage.canApplyForce (currentGameObject)) {
									isRigidbody = true;
								}
							}

							if (!elementsToAbsorve.Contains(currentGameObject) && tagToLocate.Contains (currentGameObject.tag) && (isCharacterOrVehicle || isRigidbody)) {
								currentRigidbody = currentGameObject.GetComponent<Rigidbody> ();
								if (currentRigidbody) {
									currentRigidbody.isKinematic = false;
								}

								//set the kinematic rigigbody of the enemies to false, to attract them
								currentGameObject.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);
								elementsToAbsorve.Add (currentGameObject);
							}
						}
					}
				}

				for (int i = 0; i < elementsToAbsorve.Count; i++) {
					if (elementsToAbsorve[i] != null) {
						currentRigidbody = elementsToAbsorve[i].GetComponent<Rigidbody> ();
						if (currentRigidbody) {
							if (GKC_Utils.distance (currentPosition, elementsToAbsorve [i].transform.position) < minDistanceToStopAttraction) {
								currentRigidbody.velocity = Vector3.zero;
								currentRigidbody.useGravity = false;
							} else {
								blackHoleDirection = currentPosition - elementsToAbsorve [i].gameObject.transform.position;
								blackHoleScale = Vector3.Scale (blackHoleDirection.normalized, gameObject.transform.localScale);
								currentRigidbody.AddForce (blackHoleScale * blackHoleAttractionForce * currentRigidbody.mass, currentProjectileInfo.forceMode);
							}
						}

						if (canDamage) {
							applyDamage.checkHealth (projectileGameObject, elementsToAbsorve[i], currentProjectileInfo.projectileDamage, -transform.forward, 
								elementsToAbsorve[i].transform.position, currentProjectileInfo.owner, true, false);
						}
					}
				}

				if (canDamage) {
					canDamage = false;
				}

				if (Time.time > damageRate + lastTimeDamage) {
					lastTimeDamage = Time.time;
					canDamage = true;
				}
			}

			//activate the particles, they are activated and deactivated according to the timer value
			if (blackHoleTimer < timeToActivateParticles) {
				if (!closingBlackHoleParticlesCreated) {
					closingBlackHoleParticles = creatParticles (closingBlackHoleParticles, timeToDestroyParticles);
					closingBlackHoleParticlesCreated = true;
				}
			}

			if (blackHoleTimer < timeToActivateOpenParticles && openingBlackHoleParticles) {
				if (openingBlackHoleParticles.activeSelf) {
					openingBlackHoleParticles.SetActive (false);
				}
			}

			//when the time is finishing, apply an explosion force to all the objects inside the black hole, and make an extra damage to all of them
			if (blackHoleTimer < timeToCloseBlackHole && mainRigidbody.isKinematic) {
				foreach (GameObject current in elementsToAbsorve) {
					if (current != null) {
						currentRigidbody = current.GetComponent<Rigidbody> ();
						if (currentRigidbody) {
							currentRigidbody.useGravity = true;
							currentRigidbody.AddExplosionForce (currentProjectileInfo.explosionForce, currentPosition, currentProjectileInfo.explosionRadius, 3);	
						}

						current.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);
						applyDamage.checkHealth (projectileGameObject, current, currentProjectileInfo.projectileDamage * 10, -transform.forward, 
							current.transform.position, currentProjectileInfo.owner, false, false);
					}
				}
			}

			//when a black hole bullet is shooted, if it does not touch anything in a certain amount of time, set it kinematic and open the black hole
			if (stopBullet) {
				stopBulletTimer -= Time.deltaTime;
				if (stopBulletTimer < 0) {
					stopBulletTimer = 1;
					mainRigidbody.isKinematic = true;
					mainRigidbody.useGravity = false;
					stopBullet = false;
				}
			}

			//destroy the black hole bullet
			if (blackHoleTimer > 0) {
				blackHoleTimer -= Time.deltaTime;
				if (blackHoleTimer < 0) {
					Destroy (projectileGameObject);
				}
			}
		}
	}

	public void dropBlackHoleObjects ()
	{
		for (int i = 0; i < elementsToAbsorve.Count; i++) {
			if (elementsToAbsorve [i] != null) {
				currentRigidbody = elementsToAbsorve [i].GetComponent<Rigidbody> ();
				if (currentRigidbody) {
					currentRigidbody.useGravity = true;
					currentRigidbody.AddExplosionForce (currentProjectileInfo.explosionForce, transform.position, currentProjectileInfo.explosionRadius, 3);
				}
			}
		}

		if (openingBlackHoleParticles) {
			openingBlackHoleParticles.SetActive (false);
		}
	}

	GameObject creatParticles (GameObject particles, float timeToDestroy)
	{
		GameObject newParticles = (GameObject)Instantiate (particles, transform.position, transform.rotation);
		//newParticles.transform.SetParent (transform.parent);
		newParticles.AddComponent<destroyGameObject>().setTimer(timeToDestroy);
		return newParticles;
	}

	//when the bullet touchs a surface, then
	void OnTriggerEnter (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}
			projectileUsed = true;
			//set the bullet kinematic
			objectToDamage = col.GetComponent<Collider> ().gameObject;
			if (!objectToDamage.GetComponent<Rigidbody> () && stopBullet) {
				mainRigidbody.isKinematic = true;
			}
		}
	}

	//set these bools when the bullet is a black hole
	public void stopBlackHole ()
	{
		stopBullet = true;
	}

	public void activateProjectileAbility ()
	{
		abilityActivated = true;

		//if the bullet type is a black hole, remove any other black hole in the scene and set the parameters in the bullet script 
		//the bullet with the black hole has activated the option useGravity in its rigidbody
		blackHoleProjectile[] blackHoleList = FindObjectsOfType<blackHoleProjectile> ();
		foreach (blackHoleProjectile blackHole in blackHoleList){
			if (blackHole.gameObject != gameObject) {
				blackHole.dropBlackHoleObjects ();
				Destroy (blackHole.gameObject);
			}
		}
		mainRigidbody.useGravity = true;
		stopBlackHole ();
		blackHoleTimer = 10;
		GetComponent<SphereCollider> ().radius *= 5;

		TrailRenderer currentTrailRenderer = GetComponent<TrailRenderer> ();

		currentTrailRenderer.startWidth = 4;
		currentTrailRenderer.time = 2;
		currentTrailRenderer.endWidth = 3;
	}
}
