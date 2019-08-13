using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class weaponSystem : MonoBehaviour
{
	public GameObject playerControllerGameObject;
	public GameObject playerCameraGameObject;
	public weaponInfo weaponSettings;
	public AudioClip outOfAmmo;
	public GameObject weaponProjectile;
	public LayerMask layer;
	public bool reloading;
	public bool carryingWeaponInThirdPerson;
	public bool carryingWeaponInFirstPerson;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool showSettings;

	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;
	decalManager impactDecalManager;

	public bool canMarkTargets;
	public List<string> tagListToMarkTargets = new List<string> ();
	public LayerMask markTargetsLayer;
	public string markTargetName;
	public float maxDistanceToMarkTargets;
	public bool canMarkTargetsOnFirstPerson;
	public bool canMarkTargetsOnThirdPerson;
	public bool aimOnFirstPersonToMarkTarget;
	public bool useMarkTargetSound;
	public AudioClip markTargetSound;

	public IKWeaponSystem IKWeaponManager;
	public Transform mainCameraTransform;

	public bool usingWeapon;

	List<GameObject> shells = new List<GameObject> ();
	float destroyShellsTimer = 0;
	RaycastHit hit;
	RaycastHit hitCamera;
	RaycastHit hitWeapon;
	float lastShoot;
	AudioSource weaponsEffectsSource;
	bool animationForwardPlayed;
	bool animationBackPlayed;

	playerWeaponsManager weaponsManager;
	bool shellCreated;
	Camera mainCamera;
	headBob headBobManager;
	Animation weaponAnimation;
	bool weaponHasAnimation;
	playerController playerControllerManager;
	Vector3 forceDirection;
	GameObject closestEnemy;
	bool aimingHommingProjectile;

	List<GameObject> locatedEnemies = new List<GameObject> ();

	float weaponSpeed = 1;
	float originalWeaponSpeed;
	playerCamera playerCameraManager;
	launchTrayectory parable;

	gravitySystem gravitySystemManager;
	Collider playerCollider;

	Vector3 aimedZone;
	bool objectiveFound;

	bool usingSight;

	Coroutine muzzleFlahsCoroutine;

	bool usingScreenSpaceCamera;
	bool targetOnScreen;
	Vector3 screenPoint;

	playerScreenObjectivesSystem playerScreenObjectivesManager;

	void Start ()
	{
		weaponsManager = playerControllerGameObject.GetComponent<playerWeaponsManager> ();
		playerControllerManager = playerControllerGameObject.GetComponent<playerController> ();
		playerCameraManager = playerCameraGameObject.GetComponent<playerCamera> ();
		mainCameraTransform = playerCameraManager.getCameraTransform ();
		mainCamera = playerCameraManager.getMainCamera ();
		headBobManager = mainCamera.GetComponent<headBob> ();
		weaponsEffectsSource = GetComponent<AudioSource> ();
		weaponSettings.ammoPerClip = weaponSettings.clipSize;

		if (weaponSettings.startWithEmptyClip) {
			weaponSettings.clipSize = 0;
		}

		weaponAnimation = GetComponent<Animation> ();
		if (weaponSettings.animation != "") {
			weaponHasAnimation = true;
			weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed; 
		} else {
			shellCreated = true;
		}
		originalWeaponSpeed = weaponSpeed;

		//get the parable launcher in case the weapons has it
		parable = GetComponentInChildren<launchTrayectory> ();

		gravitySystemManager = playerControllerGameObject.GetComponent<gravitySystem> ();

		playerCollider = playerControllerGameObject.GetComponent<Collider> ();
		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;

		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

		playerScreenObjectivesManager = weaponsManager.getPlayerScreenObjectivesManager ();
	}

	void Update ()
	{
		//if the amount of shells from the projectiles is higher than 0, check the time to remove then
		if (shells.Count > 0) {
			destroyShellsTimer += Time.deltaTime;
			if (destroyShellsTimer > 3) {
				for (int i = 0; i < shells.Count; i++) {
					Destroy (shells [i]);
				}
				shells.Clear ();
				destroyShellsTimer = 0;
			}
		}

		if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
			if (!shellCreated && ((weaponHasAnimation && animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) || !weaponHasAnimation)) {
				createShells ();
			}

			if (!reloading) {
				if (weaponHasAnimation) {
					if (weaponSettings.clipSize > 0) {
						if (weaponSettings.playAnimationBackward) {
							if (animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) {
								animationForwardPlayed = false;
								animationBackPlayed = true;
								weaponAnimation [weaponSettings.animation].speed = -weaponSettings.animationSpeed; 
								weaponAnimation [weaponSettings.animation].time = weaponAnimation [weaponSettings.animation].length;
								weaponAnimation.Play (weaponSettings.animation);
							}
							if (animationBackPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) {
								animationBackPlayed = false;
							}
						} else {
							animationForwardPlayed = false;
							animationBackPlayed = false;
						}
					} else if ((weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) && weaponSettings.autoReloadWhenClipEmpty) {
						reloadWeapon (weaponSettings.reloadTime);
					}
				} else {
					if (weaponSettings.clipSize == 0) {
						if ((weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) && weaponSettings.autoReloadWhenClipEmpty) {
							reloadWeapon (weaponSettings.reloadTime);
						}
					}
				}
			} 

			if (aimingHommingProjectile) {
				//while the number of located enemies is lowers that the max enemies amount, then
				if (locatedEnemies.Count < weaponSettings.projectilePosition.Count) {
					//uses a ray to detect enemies, to locked them
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, weaponsManager.targetToDamageLayer)) {
						GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);
						if (target != null && target != playerControllerGameObject) {
							if (weaponSettings.tagToLocate.Contains (target.tag)) {
								GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);
								if (!locatedEnemies.Contains (placeToShoot)) {
									//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
									locatedEnemies.Add (placeToShoot);

									playerScreenObjectivesManager.addElementToPlayerList (placeToShoot, false, false, 0, true, false, 
										false, weaponSettings.locatedEnemyIconName, false, Color.white, true, -1, 0, false);
								}
							}
						}
					}
				}
			}

			if (shootingBurst) {
				if (Time.time > lastShoot + weaponSettings.fireRate) {
					if ((!animationForwardPlayed && !animationBackPlayed && weaponHasAnimation) || !weaponHasAnimation) {
						currentBurstAmount--;
						if (currentBurstAmount == 0) {
							shootWeapon (!playerCameraManager.isFirstPersonActive (), false);
							shootingBurst = false;
						} else {
							shootWeapon (!playerCameraManager.isFirstPersonActive (), true);
						}
					}
				}
			}
		}
	}

	public void setWeaponCarryState (bool thirdPersonCarry, bool firstPersonCarry)
	{
		carryingWeaponInThirdPerson = thirdPersonCarry;
		carryingWeaponInFirstPerson = firstPersonCarry;
		checkParableTrayectory (false, carryingWeaponInFirstPerson);
		checkWeaponAbility (false);

		//functions called when the player draws or keep the weapon in any view
		if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
			if (weaponSettings.useStartDrawAction) {
				if (weaponSettings.startDrawAction.GetPersistentEventCount () > 0) {
					weaponSettings.startDrawAction.Invoke ();
				}
			}
		} else {
			if (weaponSettings.useStopDrawAction) {
				if (weaponSettings.stopDrawAction.GetPersistentEventCount () > 0) {
					weaponSettings.stopDrawAction.Invoke ();
				}
			}
		}

		if (playerCameraManager) {
			if (!playerCameraManager.isFirstPersonActive ()) {
				if (carryingWeaponInThirdPerson) {
					if (weaponSettings.useStartDrawActionThirdPerson) {
						if (weaponSettings.startDrawActionThirdPerson.GetPersistentEventCount () > 0) {
							weaponSettings.startDrawActionThirdPerson.Invoke ();
						}
					}
				} else {
					if (weaponSettings.useStopDrawActionThirdPerson) {
						if (weaponSettings.stopDrawActionThirdPerson.GetPersistentEventCount () > 0) {
							weaponSettings.stopDrawActionThirdPerson.Invoke ();
						}
					}
				}
			}

			if (playerCameraManager.isFirstPersonActive ()) {
				if (carryingWeaponInFirstPerson) {
					if (weaponSettings.useStartDrawActionFirstPerson) {
						if (weaponSettings.startDrawActionFirstPerson.GetPersistentEventCount () > 0) {
							weaponSettings.startDrawActionFirstPerson.Invoke ();
						}
					}
				} else {
					if (weaponSettings.useStopDrawActionFirstPerson) {
						if (weaponSettings.stopDrawActionFirstPerson.GetPersistentEventCount () > 0) {
							weaponSettings.stopDrawActionFirstPerson.Invoke ();
						}
					}
				}
			}
		}
	}

	public void setWeaponAimState (bool thirdPersonAim, bool firstPersonAim)
	{
		aimingInThirdPerson = thirdPersonAim;
		aimingInFirstPerson = firstPersonAim;
		if ((aimingInThirdPerson || aimingInFirstPerson) && weaponSettings.clipSize == 0) {
			if (weaponSettings.autoReloadWhenClipEmpty) {
				manualReload ();
			}
		}
		if (carryingWeaponInFirstPerson) {
			checkParableTrayectory (false, !aimingInFirstPerson);
		}
		if (carryingWeaponInThirdPerson) {
			checkParableTrayectory (aimingInThirdPerson, false);
			checkWeaponAbility (false);
		}
		if (!aimingInFirstPerson && !aimingInThirdPerson) {
			if (locatedEnemies.Count > 0) {
				aimingHommingProjectile = false;
				//remove the icons in the screen
				removeLocatedEnemiesIcons ();
			}
		}

		//functions called when the player aims or stop to aim the weapon in any view
		if (!playerCameraManager.isFirstPersonActive ()) {
			if (aimingInThirdPerson) {
				if (weaponSettings.useStartAimActionThirdPerson) {
					if (weaponSettings.startAimActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startAimActionThirdPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopAimActionThirdPerson) {
					if (weaponSettings.stopAimActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopAimActionThirdPerson.Invoke ();
					}
				}
			}
		}

		if (playerCameraManager.isFirstPersonActive ()) {
			if (aimingInFirstPerson) {
				if (weaponSettings.useStartAimActionFirstPerson) {
					if (weaponSettings.startAimActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startAimActionFirstPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopAimActionFirstPerson) {
					if (weaponSettings.stopAimActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopAimActionFirstPerson.Invoke ();
					}
				}
			}
		}
	}

	bool shootingBurst;
	int currentBurstAmount;

	public void setUsingWeaponState (bool state)
	{
		usingWeapon = state;
		weaponsManager.setAttachmentPanelState (usingWeapon);
		weaponsManager.setAttachmentIcon (weaponSettings.weaponIconHUD);
		weaponsManager.setAttachmentPanelAmmoText (weaponSettings.clipSize.ToString ());

		if (usingWeapon) {
			weaponSettings.clipSizeText = weaponsManager.attachmentAmmoText;
		}
	}

	public void checkCarryAndAimStates ()
	{
		if (weaponsManager.carryingWeaponInThirdPerson != carryingWeaponInThirdPerson || weaponsManager.carryingWeaponInFirstPerson != carryingWeaponInFirstPerson) {
			setWeaponCarryState (weaponsManager.carryingWeaponInThirdPerson, weaponsManager.carryingWeaponInFirstPerson);
		}

		if (weaponsManager.aimingInThirdPerson != aimingInThirdPerson || weaponsManager.aimingInFirstPerson != aimingInFirstPerson) {
			setWeaponAimState (weaponsManager.aimingInThirdPerson, weaponsManager.aimingInFirstPerson);
		}
	}

	public void inputShootWeaponOnPressDown ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {
			
			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					if (weaponSettings.automatic) {
						if (weaponSettings.useBurst) {
							shootWeapon (true);
						}
					} else {
						shootWeapon (true);
					}
				}
			}
		}
	}

	public void inputShootWeaponOnPressUp ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {

			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					shootWeapon (false);
				}
			}
		}
	}

	public void inputShootWeaponOnPress ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {

			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					if (weaponSettings.automatic) {
						if (!weaponSettings.useBurst) {
							shootWeapon (true);
						}
					}
				}
			}
		}
	}

	public void shootWeapon (bool state)
	{
		if (!weaponsManager.playerIsBusy ()) {
			if (state) {
				if (!shootingBurst) {

					if (IKWeaponManager.isCursorHidden ()) {
						weaponsManager.enableOrDisableGeneralWeaponCursor (true);
						IKWeaponManager.setCursorHiddenState (false);
					}

					weaponsManager.disablePlayerRunningState ();

					if (!reloading && getWeaponClipSize () > 0) {
						weaponsManager.setShootingState (true);
					} else {
						weaponsManager.setShootingState (false);
					}
					shootWeapon (!weaponsManager.isFirstPersonActive (), state);
					weaponsManager.setLastTimeFired ();
				}
			} else {
				weaponsManager.setShootingState (false);
				shootWeapon (!weaponsManager.isFirstPersonActive (), state);
			}
		}
	}


	//fire the current weapon
	public void shootWeapon (bool isThirdPersonView, bool shootAtKeyDown)
	{
		if (!IKWeaponManager.weaponCanFire ()) {
			return;
		}

		if (IKWeaponManager.moving) {
			return;
		}

		if (reloading) {
			return;
		}

		checkWeaponAbility (shootAtKeyDown);

		//if the weapon system is active and the clip size higher than 0
		if (weaponSettings.clipSize > 0) {
			//else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + weaponSettings.fireRate) {

				//if the player fires a weapon, set the visible to AI state to true, this will change if the player is using a silencer
				if (weaponSettings.shootAProjectile || weaponSettings.launchProjectile) {
					playerControllerManager.setVisibleToAIState (true);
				}

				//If the current projectile is homming type, check when the shoot button is pressed and release
				if ((weaponSettings.isHommingProjectile && shootAtKeyDown)) {
					aimingHommingProjectile = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if ((weaponSettings.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!weaponSettings.isHommingProjectile && !shootAtKeyDown)) {
					aimingHommingProjectile = false;
					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if ((animationForwardPlayed || animationBackPlayed) && weaponHasAnimation) {
					return;
				}


				if (weaponSettings.automatic && weaponSettings.useBurst) {
					if (!shootingBurst && weaponSettings.burstAmount > 0) {
						shootingBurst = true;
						currentBurstAmount = weaponSettings.burstAmount;
					}
				}

				//camera shake
				if (IKWeaponManager.sameValueBothViews) {
					headBobManager.setShotShakeState (IKWeaponManager.thirdPersonshotShakeInfo);
				} else {
					if (weaponsManager.carryingWeaponInFirstPerson && IKWeaponManager.useShotShakeInFirstPerson) {
						headBobManager.setShotShakeState (IKWeaponManager.firstPersonshotShakeInfo);
					}
					if (weaponsManager.carryingWeaponInThirdPerson && IKWeaponManager.useShotShakeInThirdPerson) {
						headBobManager.setShotShakeState (IKWeaponManager.thirdPersonshotShakeInfo);
					}
				}

				//recoil
				IKWeaponManager.startRecoil (isThirdPersonView);
				IKWeaponManager.setLastTimeMoved ();
				if (weaponSettings.shakeUpperBodyWhileShooting) {
					weaponsManager.checkShakeUpperBodyRotationCoroutine (weaponSettings.shakeAmount, weaponSettings.shakeSpeed);
				}

				//check shot camera noise
				IKWeaponManager.setShotCameraNoise ();

				checkWeaponShootNoise ();

				//play the fire sound
				playWeaponSoundEffect (true);

				//enable the muzzle flash light
				enableMuzzleFlashLight ();

				Vector3 mainCameraPosition = mainCameraTransform.position;

				//create the muzzle flash
				createMuzzleFlash ();
				bool weaponCrossingSurface = false;
				if (!isThirdPersonView) {
					if (weaponSettings.projectilePosition.Count > 0) {
						if (Physics.Raycast (mainCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hitCamera, Mathf.Infinity, layer) &&
						    Physics.Raycast (weaponSettings.projectilePosition [0].position, mainCameraTransform.TransformDirection (Vector3.forward), out hitWeapon, Mathf.Infinity, layer)) {
							if (hitCamera.collider != hitWeapon.collider) {
								//print ("too close surface");
								weaponCrossingSurface = true;
							} 
						}
					}
				}

				//play the fire animation
				if (weaponHasAnimation) {
					weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed;
					weaponAnimation.Play (weaponSettings.animation);
					animationForwardPlayed = true;
					if (weaponSettings.cockSound) {
						playSound (weaponSettings.cockSound);
					}
				} 
				shellCreated = false;

				//every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
					for (int l = 0; l < weaponSettings.projectilesPerShoot; l++) {
						//create the projectile
						GameObject newProjectile = (GameObject)Instantiate (weaponProjectile, weaponSettings.projectilePosition [j].position, mainCameraTransform.rotation);

						if (!weaponSettings.launchProjectile) {
							//set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (!weaponCrossingSurface) {
								if (Physics.Raycast (mainCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)
								    && !weaponSettings.fireWeaponForward) {
									if (!hit.collider.isTrigger) {
										//Debug.DrawLine (weaponSettings.projectilePosition [j].position, hit.point, Color.red, 2);
										newProjectile.transform.LookAt (hit.point);
									}
								}
							}
						}

						if (weaponSettings.launchProjectile) {
							newProjectile.GetComponent<Rigidbody> ().isKinematic = true;
							//if the vehicle has a gravity control component, and the current gravity is not the regular one, add an artifical gravity component to the projectile
							//like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle

							if (gravitySystemManager.getCurrentNormal () != Vector3.up) {
								newProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-gravitySystemManager.getCurrentNormal ());
							}

							if (weaponSettings.useParableSpeed) {
								//get the ray hit point where the projectile will fall
								if (Physics.Raycast (mainCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
									aimedZone = hit.point;
									objectiveFound = true;
								} else {
									objectiveFound = false;
								}
							}

							launchCurrentProjectile (newProjectile);
						}

						//add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;
						if (weaponSettings.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							newProjectile.transform.Rotate (spreadAmount);
						}

						//set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						projectileSystem currentProjectileSystem = newProjectile.GetComponent<projectileSystem> ();
						currentProjectileSystem.setProjectileInfo (setProjectileInfo ());

						if (weaponSettings.isSeeker) {
							setSeekerProjectileInfo (newProjectile, weaponSettings.projectilePosition [j]);
						}

						//if the homing projectiles are being using, then
						if (aimingHommingProjectile) {
							//if the button to shoot is released, shoot a homing projectile for every located enemy
							//check that the located enemies are higher that 0
							if (locatedEnemies.Count > 0) {
								//shoot the missiles
								if (j < locatedEnemies.Count) {
									currentProjectileSystem.setEnemy (locatedEnemies [j]);
								}
							}
						}

						//if the weapon shoots setting directly the projectile in the hit point, place the current projectile in the hit point position
						if (weaponSettings.useRayCastShoot || weaponCrossingSurface) {
							Vector3 forwardDirection = mainCameraTransform.TransformDirection (Vector3.forward);
							Vector3 forwardPositon = mainCameraPosition;
							if (weaponSettings.fireWeaponForward && !weaponCrossingSurface) {
								forwardDirection = transform.forward;
								forwardPositon = weaponSettings.projectilePosition [j].position;
							}
							if (spreadAmount.magnitude != 0) {
								forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
							}

							if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layer)) {
								currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);
								//print ("same object fired: " + hit.collider.name);
							} else {
								currentProjectileSystem.destroyProjectile ();
							}
						}
					}
					useAmmo ();
					lastShoot = Time.time;
					destroyShellsTimer = 0;
				}

				if (weaponSettings.weaponWithAbility) {
					lastShoot = Time.time;
					destroyShellsTimer = 0;
				}

				if (weaponSettings.applyForceAtShoot) {
					forceDirection = (mainCameraTransform.right * weaponSettings.forceDirection.x +
					mainCameraTransform.up * weaponSettings.forceDirection.y +
					mainCameraTransform.forward * weaponSettings.forceDirection.z) * weaponSettings.forceAmount;
					playerControllerManager.externalForce (forceDirection);
				}

				if (weaponSettings.isHommingProjectile && !shootAtKeyDown && aimingHommingProjectile) {
					//if the button to shoot is released, shoot a homing projectile for every located enemy
					//check that the located enemies are higher that 0
					if (locatedEnemies.Count > 0) {
						//remove the icons in the screen
						removeLocatedEnemiesIcons ();
					}
					aimingHommingProjectile = false;
				}
			}
		} 
		//else, the clip in the weapon is over, so check if there is remaining ammo
		else {
			if (weaponSettings.remainAmmo == 0 && !weaponSettings.infiniteAmmo) {
				playWeaponSoundEffect (false);
			}
		}
	}

	public void checkWeaponAbility (bool keyDown)
	{
		if (weaponSettings.weaponWithAbility) {
			if (keyDown) {
				if (weaponSettings.useDownButton) {
					if (weaponSettings.downButtonAction.GetPersistentEventCount () > 0) {
						weaponSettings.downButtonAction.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useUpButton) {
					if (weaponSettings.upButtonAction.GetPersistentEventCount () > 0) {
						weaponSettings.upButtonAction.Invoke ();
					}
				}
			}
		}
	}

	//remove the localte enemies icons
	public void removeLocatedEnemiesIcons ()
	{
		for (int i = 0; i < locatedEnemies.Count; i++) {
			playerScreenObjectivesManager.removeElementFromListByPlayer (locatedEnemies [i]);
		}
		locatedEnemies.Clear ();
	}

	public projectileInfo setProjectileInfo ()
	{
		projectileInfo newProjectile = new projectileInfo ();

		newProjectile.isHommingProjectile = weaponSettings.isHommingProjectile;
		newProjectile.isSeeker = weaponSettings.isSeeker;
		newProjectile.waitTimeToSearchTarget = weaponSettings.waitTimeToSearchTarget;
		newProjectile.useRayCastShoot = weaponSettings.useRayCastShoot;

		newProjectile.useRaycastCheckingOnRigidbody = weaponSettings.useRaycastCheckingOnRigidbody;

		newProjectile.projectileDamage = weaponSettings.projectileDamage;
		newProjectile.projectileSpeed = weaponSettings.projectileSpeed;

		newProjectile.impactForceApplied = weaponSettings.impactForceApplied;
		newProjectile.forceMode = weaponSettings.forceMode;
		newProjectile.applyImpactForceToVehicles = weaponSettings.applyImpactForceToVehicles;

		newProjectile.projectileWithAbility = weaponSettings.projectileWithAbility;

		newProjectile.impactSoundEffect = weaponSettings.impactSoundEffect;

		newProjectile.scorch = weaponSettings.scorch;
		newProjectile.scorchRayCastDistance = weaponSettings.scorchRayCastDistance;

		newProjectile.owner = playerControllerGameObject;

		newProjectile.projectileParticles = weaponSettings.projectileParticles;
		newProjectile.impactParticles = weaponSettings.impactParticles;

		newProjectile.isExplosive = weaponSettings.isExplosive;
		newProjectile.isImplosive = weaponSettings.isImplosive;
		newProjectile.useExplosionDelay = weaponSettings.useExplosionDelay;
		newProjectile.explosionDelay = weaponSettings.explosionDelay;
		newProjectile.explosionForce = weaponSettings.explosionForce;
		newProjectile.explosionRadius = weaponSettings.explosionRadius;
		newProjectile.explosionDamage = weaponSettings.explosionDamage;
		newProjectile.pushCharacters = weaponSettings.pushCharacters;
		newProjectile.canDamageProjectileOwner = weaponSettings.canDamageProjectileOwner;

		newProjectile.killInOneShot = weaponSettings.killInOneShot;

		newProjectile.useDisableTimer = weaponSettings.useDisableTimer;
		newProjectile.noImpactDisableTimer = weaponSettings.noImpactDisableTimer;
		newProjectile.impactDisableTimer = weaponSettings.impactDisableTimer;

		newProjectile.targetToDamageLayer = weaponsManager.targetToDamageLayer;
		newProjectile.targetForScorchLayer = weaponsManager.targetForScorchLayer;

		newProjectile.impactDecalIndex = impactDecalIndex;

		newProjectile.launchProjectile = weaponSettings.launchProjectile;

		newProjectile.adhereToSurface = weaponSettings.adhereToSurface;
		newProjectile.adhereToLimbs = weaponSettings.adhereToLimbs;

		newProjectile.useGravityOnLaunch = weaponSettings.useGravityOnLaunch;
		newProjectile.useGraivtyOnImpact = weaponSettings.useGraivtyOnImpact;

		newProjectile.breakThroughObjects = weaponSettings.breakThroughObjects;
		newProjectile.infiniteNumberOfImpacts = weaponSettings.infiniteNumberOfImpacts;
		newProjectile.numberOfImpacts = weaponSettings.numberOfImpacts;
		newProjectile.canDamageSameObjectMultipleTimes = weaponSettings.canDamageSameObjectMultipleTimes;
		newProjectile.forwardDirection = mainCameraTransform.forward;

		newProjectile.damageTargetOverTime = weaponSettings.damageTargetOverTime;
		newProjectile.damageOverTimeDelay = weaponSettings.damageOverTimeDelay;
		newProjectile.damageOverTimeDuration = weaponSettings.damageOverTimeDuration;
		newProjectile.damageOverTimeAmount = weaponSettings.damageOverTimeAmount;
		newProjectile.damageOverTimeRate = weaponSettings.damageOverTimeRate;
		newProjectile.damageOverTimeToDeath = weaponSettings.damageOverTimeToDeath;
		newProjectile.removeDamageOverTimeState = weaponSettings.removeDamageOverTimeState;

		newProjectile.sedateCharacters = weaponSettings.sedateCharacters;
		newProjectile.sedateDelay = weaponSettings.sedateDelay;
		newProjectile.useWeakSpotToReduceDelay = weaponSettings.useWeakSpotToReduceDelay;
		newProjectile.sedateDuration = weaponSettings.sedateDuration;
		newProjectile.sedateUntilReceiveDamage = weaponSettings.sedateUntilReceiveDamage;

		newProjectile.pushCharacter = weaponSettings.pushCharacter;
		newProjectile.pushCharacterForce = weaponSettings.pushCharacterForce;
		newProjectile.pushCharacterRagdollForce = weaponSettings.pushCharacterRagdollForce;

		return newProjectile;
	}

	public void setSeekerProjectileInfo (GameObject projectile, Transform shootZone)
	{
		//get all the enemies in the scene
		List<GameObject> enemiesInFront = new List<GameObject> ();
		List<GameObject> fullEnemyList = new List<GameObject> ();

		for (int i = 0; i < weaponSettings.tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (weaponSettings.tagToLocate [i]);
			fullEnemyList.AddRange (enemiesList);
		}

		for (int i = 0; i < fullEnemyList.Count; i++) {
			//get those enemies which are not dead and in front of the camera
			if (!applyDamage.checkIfDead (fullEnemyList [i])) {
				
				if (usingScreenSpaceCamera) {
					screenPoint = mainCamera.WorldToViewportPoint (fullEnemyList [i].transform.position);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
				} else {
					screenPoint = mainCamera.WorldToScreenPoint (fullEnemyList [i].transform.position);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
				}

				//the target is visible in the screen
				if (targetOnScreen) {
					enemiesInFront.Add (fullEnemyList [i]);
				}
			}
		}

		for (int i = 0; i < enemiesInFront.Count; i++) {
			//for every enemy in front of the camera, use a raycast, if it finds an obstacle between the enemy and the camera, the enemy is removed from the list
			Vector3 direction = enemiesInFront [i].transform.position - shootZone.position;
			direction = direction / direction.magnitude;
			float distance = GKC_Utils.distance (enemiesInFront [i].transform.position, shootZone.position);
			if (Physics.Raycast (shootZone.position, direction, out hit, distance, weaponsManager.targetToDamageLayer)) {
				if (!hit.transform.IsChildOf (enemiesInFront [i].transform) && hit.collider.gameObject != enemiesInFront [i]) {
					enemiesInFront.RemoveAt (i);
					i = i - 1;
				}
			}
		}

		//finally, get the enemy closest to the player
		float minDistance = Mathf.Infinity;
		for (int i = 0; i < enemiesInFront.Count; i++) {
			float currentDistance = GKC_Utils.distance (enemiesInFront [i].transform.position, transform.position);
			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				closestEnemy = enemiesInFront [i];
			}
		}

		if (closestEnemy) {
			Transform placeToShoot = applyDamage.getPlaceToShoot (closestEnemy);
			if (placeToShoot != null) {
				projectile.GetComponent<projectileSystem> ().setEnemy (placeToShoot.gameObject);
			} else {
				projectile.GetComponent<projectileSystem> ().setEnemy (closestEnemy);
			}
		}
		closestEnemy = null;
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = 0;
		if (carryingWeaponInFirstPerson) {
			spreadAmount = weaponSettings.spreadAmount;
		}
		if (carryingWeaponInThirdPerson) {
			if (weaponSettings.sameSpreadInThirdPerson) {
				spreadAmount = weaponSettings.spreadAmount;
			} else {
				spreadAmount = weaponSettings.thirdPersonSpreadAmount;
			}
		}
		if (aimingInFirstPerson) {
			//print ("aiming");
			if (weaponSettings.useSpreadAming) {
				if (weaponSettings.useLowerSpreadAiming) {
					spreadAmount = weaponSettings.lowerSpreadAmount;
					//print ("lower spread");
				} else {
					//print ("same spread");
				}
			} else {
				//print ("no spread");
				spreadAmount = 0;
			}
		} else {
			//print ("no aiming");
		}
		if (spreadAmount > 0) {
			Vector3 randomSpread = Vector3.zero;
			randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.z = Random.Range (-spreadAmount, spreadAmount);
			return randomSpread;
		}
		return Vector3.zero;
	}

	void createShells ()
	{
		for (int j = 0; j < weaponSettings.shellPosition.Count; j++) {
			//if the current weapon drops shells, create them
			if (weaponSettings.shell) {
				GameObject shellClone = (GameObject)Instantiate (weaponSettings.shell, weaponSettings.shellPosition [j].position, weaponSettings.shellPosition [j].rotation);
				shellClone.GetComponent<Rigidbody> ().AddForce (weaponSettings.shellPosition [j].right * weaponSettings.shellEjectionForce * GKC_Utils.getCurrentScaleTime ());
				Physics.IgnoreCollision (playerCollider, shellClone.transform.GetChild (0).GetComponent<Collider> ());
				if (weaponSettings.shellDropSoundList.Count > 0) {
					shellClone.GetComponent<AudioSource> ().clip = weaponSettings.shellDropSoundList [Random.Range (0, weaponSettings.shellDropSoundList.Count - 1)];
				}
				shells.Add (shellClone);
				if (shells.Count > 15) {
					GameObject shellToRemove = shells [0];
					shells.RemoveAt (0);
					Destroy (shellToRemove);
				}
				shellCreated = true;
			}
		}
	}

	//play the fire sound or the empty clip sound
	void playWeaponSoundEffect (bool hasAmmo)
	{
		if (weaponsEffectsSource) {
			if (hasAmmo) {
				if (weaponSettings.shootSoundEffect) {
					weaponsEffectsSource.clip = weaponSettings.shootSoundEffect;
			
					weaponsEffectsSource.pitch = weaponSpeed;
					weaponsEffectsSource.Play ();
				}
			} else {
				if (Time.time > lastShoot + weaponSettings.fireRate) {
					weaponsEffectsSource.pitch = weaponSpeed;
					GKC_Utils.checkAudioSourcePitch (weaponsEffectsSource);
					weaponsEffectsSource.PlayOneShot (outOfAmmo);
					lastShoot = Time.time;
				}
			}
		} else {
			print ("WARNING: no audio source attached on " + gameObject.name + " weapon");
		}
	}

	public void playSound (AudioClip clipSound)
	{
		if (weaponsEffectsSource) {
			GKC_Utils.checkAudioSourcePitch (weaponsEffectsSource);
			weaponsEffectsSource.PlayOneShot (clipSound);
		}
	}

	//create the muzzle flash particles if the weapon has it
	void createMuzzleFlash ()
	{
		if (weaponSettings.shootParticles) {
			for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
				GameObject muzzleParticlesClone = (GameObject)Instantiate (weaponSettings.shootParticles, weaponSettings.projectilePosition [j].position, weaponSettings.projectilePosition [j].rotation);
				Destroy (muzzleParticlesClone, 1);	
				muzzleParticlesClone.transform.SetParent (weaponSettings.projectilePosition [j]);
				weaponSettings.shootParticles.GetComponent<ParticleSystem> ().Play ();
			}
		}
	}

	//	//decrease the amount of ammo in the clip
	public void useAmmo ()
	{
		weaponSettings.clipSize--;
		//update hud ammo info
		weaponsManager.setAttachmentPanelAmmoText (weaponSettings.clipSize.ToString ());
	}

	public void useAmmo (int amount)
	{
		if (amount > weaponSettings.clipSize) {
			amount = weaponSettings.clipSize;
		}
		weaponSettings.clipSize -= amount;
		//update hud ammo info
		weaponsManager.setAttachmentPanelAmmoText (weaponSettings.clipSize.ToString ());
	}
		
	//check the amount of ammo
	void checkRemainAmmo ()
	{
		//if the weaopn has not infinite ammo
		if (!weaponSettings.infiniteAmmo) {
			//the clip is empty
			if (weaponSettings.clipSize == 0) {
				//if the remaining ammo is lower that the ammo per clip, set the final projectiles in the clip 
				if (weaponSettings.remainAmmo < weaponSettings.ammoPerClip) {
					weaponSettings.clipSize = weaponSettings.remainAmmo;
				} 
				//else, refill it
				else {
					weaponSettings.clipSize = weaponSettings.ammoPerClip;
				}
				//if the remaining ammo is higher than 0, remove the current projectiles added in the clip
				if (weaponSettings.remainAmmo > 0) {
					weaponSettings.remainAmmo -= weaponSettings.clipSize;
				} 
			} 
			//the clip has some bullets in it yet
			else {
				int usedAmmo = 0;
				if (weaponSettings.removePreviousAmmoOnClip) {
					weaponSettings.clipSize = 0;
					if (weaponSettings.remainAmmo < (weaponSettings.ammoPerClip)) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip;
					}
				} else {
					if (weaponSettings.remainAmmo < (weaponSettings.ammoPerClip - weaponSettings.clipSize)) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip - weaponSettings.clipSize;
					}
				}
				weaponSettings.remainAmmo -= usedAmmo;
				weaponSettings.clipSize += usedAmmo;
			}
		} else {
			//else, the weapon has infinite ammo, so refill it
			weaponSettings.clipSize = weaponSettings.ammoPerClip;
		}

		weaponsManager.setAttachmentPanelAmmoText (weaponSettings.clipSize.ToString ());
		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;
	}

	Coroutine reloadCoroutine;

	public void reloadWeapon (float waitTimeAmount)
	{
		if (reloadCoroutine != null) {
			StopCoroutine (reloadCoroutine);
		}
		reloadCoroutine = StartCoroutine (waitToReload (waitTimeAmount));
	}

	//a delay for reload the weapon
	IEnumerator waitToReload (float waitTimeAmount)
	{
		//print ("reload");
		//if the remmaining ammo is higher than 0 or infinite
		shootingBurst = false;
		if (weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) {
			//reload
			reloading = true;
			//play the reload sound
			if (weaponSettings.reloadSoundEffect) {
				playSound (weaponSettings.reloadSoundEffect);
			}

			if (weaponSettings.dropClipWhenReload) {
				GameObject newClipToDrop = (GameObject)Instantiate (weaponSettings.clipModel, weaponSettings.positionToDropClip.position, weaponSettings.positionToDropClip.rotation);
				Collider newClipCollider = newClipToDrop.GetComponent<Collider> ();
				if (newClipCollider) {
					Physics.IgnoreCollision (playerCollider, newClipCollider);
				}
			}

			//wait an amount of time
			yield return new WaitForSeconds (waitTimeAmount);
			//check the ammo values
			checkRemainAmmo ();
			//stop reload
			reloading = false;
		} else {
			//else, the ammo is over, play the empty weapon sound
			playWeaponSoundEffect (false);
		}
		yield return null;
	}

	public void manualReload ()
	{
		if (!reloading) {
			if (weaponSettings.clipSize < weaponSettings.ammoPerClip) {
				reloadWeapon (weaponSettings.reloadTime);
			}
		}
	}

	public bool canIncreaseRemainAmmo ()
	{
		if (weaponSettings.auxRemainAmmo < weaponSettings.ammoLimit) {
			return true;
		} else {
			return false;
		}
	}

	public bool hasMaximumAmmoAmount ()
	{
		if (weaponSettings.useAmmoLimit) {
			if (weaponSettings.remainAmmo >= weaponSettings.ammoLimit) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public bool hasAmmoLimit ()
	{
		return weaponSettings.useAmmoLimit;
	}

	public int ammoAmountToMaximumLimit ()
	{
		return weaponSettings.ammoLimit - weaponSettings.auxRemainAmmo;
	}

	//the vehicle has used an ammo pickup, so increase the correct weapon by name
	public void getAmmo (int amount)
	{
		bool empty = false;
		if (weaponSettings.remainAmmo == 0 && weaponSettings.clipSize == 0) {
			empty = true;
		}
		weaponSettings.remainAmmo += amount;
		if (empty && (carryingWeaponInFirstPerson || aimingInThirdPerson)) {
			if (weaponSettings.autoReloadWhenClipEmpty) {
				manualReload ();
			}
		}
		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;

	}

	public void addAuxRemainAmmo (int amount)
	{
		weaponSettings.auxRemainAmmo += amount;
	}

	public bool hasAnyAmmo ()
	{
		if (weaponSettings.remainAmmo > 0 || weaponSettings.clipSize > 0 || weaponSettings.infiniteAmmo) {
			return true;
		}
		return false;
	}

	public void getAndUpdateAmmo (int amount)
	{
		getAmmo (amount);
		weaponsManager.setAttachmentPanelAmmoText (weaponSettings.clipSize.ToString ());
	}

	public bool remainAmmoInClip ()
	{
		return weaponSettings.clipSize > 0;
	}

	public void launchCurrentProjectile (GameObject currentProjectile)
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		currentProjectile.GetComponent<Rigidbody> ().isKinematic = false;
		if (weaponSettings.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone);
			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}
			currentProjectile.GetComponent<Rigidbody> ().AddForce (newVel, ForceMode.VelocityChange);
		} else {
			currentProjectile.GetComponent<Rigidbody> ().AddForce (weaponSettings.parableDirectionTransform.forward * weaponSettings.projectileSpeed, ForceMode.Impulse);
		}
	}

	//calculate the speed applied to the launched projectile to make a parable according to a hit point
	Vector3 getParableSpeed (Vector3 origin, Vector3 target)
	{
		//if a hit point is not found, return
		if (!objectiveFound) {
			return -Vector3.one;
		}
		//get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;
		//remove the Y axis value
		toTargetXZ -= transform.InverseTransformDirection (toTargetXZ).y * transform.up;
		float y = transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;
		//get the velocity accoring to distance ang gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;
		//create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        
		//get direction of xz but with magnitude 1
		result *= v0xz;                                
		// set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= transform.InverseTransformDirection (result).y * transform.up;
		result += transform.up * v0y;
		return result;
	}

	public void checkParableTrayectory (bool usingThirdPerson, bool usingFirstPerson)
	{
		//enable or disable the parable linerenderer
		if (((usingThirdPerson && weaponSettings.activateLaunchParableThirdPerson) ||
		    (usingFirstPerson && weaponSettings.activateLaunchParableFirstPerson)) &&
		    weaponSettings.launchProjectile) {
			if (parable) {
				parable.changeParableState (true);
				if (weaponSettings.projectilePosition.Count > 0) {
					parable.shootPosition = weaponSettings.projectilePosition [0];
				}
			}
		} else {
			if (parable) {
				parable.changeParableState (false);
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<weaponSystem> ());
		#endif
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

	public void setReducedVelocity (float multiplierValue)
	{
		weaponSpeed *= multiplierValue;
	}

	public void setNormalVelocity ()
	{
		weaponSpeed = originalWeaponSpeed;
	}

	public string getWeaponSystemName ()
	{
		return weaponSettings.Name;
	}

	public int getWeaponClipSize ()
	{
		return weaponSettings.clipSize;
	}

	public string getCurrentAmmoText ()
	{
		if (!weaponSettings.infiniteAmmo) {
			return weaponSettings.clipSize.ToString () + "/" + weaponSettings.remainAmmo;
		} else {
			return weaponSettings.clipSize.ToString () + "/" + "Inf";
		}
	}

	public void enableMuzzleFlashLight ()
	{
		if (!weaponSettings.useMuzzleFlash) {
			return;
		}

		if (muzzleFlahsCoroutine != null) {
			StopCoroutine (muzzleFlahsCoroutine);
		}
		muzzleFlahsCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		weaponSettings.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (weaponSettings.muzzleFlahsDuration);

		weaponSettings.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	noiseMeshSystem noiseMeshManager;

	public void checkWeaponShootNoise ()
	{
		if (weaponSettings.useNoise) {
			if (weaponSettings.useNoiseDetection) {
				applyDamage.sendNoiseSignal (weaponSettings.noiseRadius, playerControllerGameObject.transform.position, weaponSettings.noiseDetectionLayer, weaponSettings.showNoiseDetectionGizmo);
			}

			if (!noiseMeshManager) {
				noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();
			}

			if (noiseMeshManager) {
				noiseMeshManager.addNoiseMesh (weaponSettings.noiseRadius, playerControllerGameObject.transform.position + Vector3.up, weaponSettings.noiseExpandSpeed);
			}
		}
	}

	void OnDrawGizmosSelected ()
	{
		if (!Application.isPlaying && getImpactListEveryFrame) {
			getImpactListInfo ();
		}
	}

	[System.Serializable]
	public class weaponInfo
	{
		public string Name;

		public Texture weaponIconHUD;
		public bool showWeaponNameInHUD = true;
		public bool showWeaponIconInHUD = true;
		public bool showWeaponAmmoSliderInHUD = true;
		public bool showWeaponAmmoTextInHUD = true;

		public Text clipSizeText;

		public bool useRayCastShoot;
		public bool fireWeaponForward;

		public bool useRaycastCheckingOnRigidbody;

		public int ammoPerClip;
		public bool removePreviousAmmoOnClip;
		public bool infiniteAmmo;
		public int remainAmmo;
		public int clipSize;
		public bool dropClipWhenReload;
		public Transform positionToDropClip;
		public GameObject clipModel;
		public bool startWithEmptyClip;
		public bool autoReloadWhenClipEmpty = true;
		public bool useAmmoLimit;
		public int ammoLimit;
		public int auxRemainAmmo;

		public bool shootAProjectile;
		public bool launchProjectile;

		public bool projectileWithAbility;

		public bool weaponWithAbility;
		public bool useDownButton;
		public UnityEvent downButtonAction;
		public bool useHoldButton;
		public UnityEvent holdButtonAction;
		public bool useUpButton;
		public UnityEvent upButtonAction;

		public bool useStartDrawAction;
		public UnityEvent startDrawAction;

		public bool useStopDrawAction;
		public UnityEvent stopDrawAction;

		public bool useStartDrawActionThirdPerson;
		public UnityEvent startDrawActionThirdPerson;

		public bool useStopDrawActionThirdPerson;
		public UnityEvent stopDrawActionThirdPerson;

		public bool useStartDrawActionFirstPerson;
		public UnityEvent startDrawActionFirstPerson;

		public bool useStopDrawActionFirstPerson;
		public UnityEvent stopDrawActionFirstPerson;

		public bool useStartAimActionThirdPerson;
		public UnityEvent startAimActionThirdPerson;

		public bool useStopAimActionThirdPerson;
		public UnityEvent stopAimActionThirdPerson;

		public bool useStartAimActionFirstPerson;
		public UnityEvent startAimActionFirstPerson;

		public bool useStopAimActionFirstPerson;
		public UnityEvent stopAimActionFirstPerson;

		public bool showDrawAimFunctionSettings;

		public bool automatic;

		public bool useBurst;
		public int burstAmount;

		public float fireRate;
		public float reloadTime;
		public float projectileDamage;
		public float projectileSpeed;
		public int projectilesPerShoot;

		public bool useProjectileSpread;
		public float spreadAmount;
		public bool sameSpreadInThirdPerson;
		public float thirdPersonSpreadAmount;
		public bool useSpreadAming;
		public bool useLowerSpreadAiming;
		public float lowerSpreadAmount;

		public bool isImplosive;
		public bool isExplosive;
		public float explosionForce;
		public float explosionRadius;
		public bool useExplosionDelay;
		public float explosionDelay;
		public float explosionDamage;
		public bool pushCharacters;
		public bool canDamageProjectileOwner = true;

		public List<Transform> projectilePosition = new List<Transform> ();

		public GameObject shell;
		public List<Transform> shellPosition = new List<Transform> ();
		public float shellEjectionForce = 100;
		public List<AudioClip> shellDropSoundList = new List<AudioClip> ();

		public GameObject weaponMesh;
		public string animation;
		public float animationSpeed = 1;
		public bool playAnimationBackward = true;
		public GameObject scorch;
		public float scorchRayCastDistance;

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public bool applyForceAtShoot;
		public Vector3 forceDirection;
		public float forceAmount;

		public bool isHommingProjectile;
		public bool isSeeker;
		public float waitTimeToSearchTarget;

		public float impactForceApplied;
		public ForceMode forceMode;
		public bool applyImpactForceToVehicles;

		public AudioClip reloadSoundEffect;
		public AudioClip cockSound;
		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;

		public GameObject shootParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		public bool killInOneShot;

		public bool useDisableTimer;
		public float noImpactDisableTimer;
		public float impactDisableTimer;

		public string locatedEnemyIconName = "Homing Located Enemy";
		public List<string> tagToLocate = new List<string> ();

		public bool activateLaunchParableThirdPerson;
		public bool activateLaunchParableFirstPerson;
		public bool useParableSpeed;
		public Transform parableDirectionTransform;

		public bool adhereToSurface;
		public bool adhereToLimbs;

		public bool useGravityOnLaunch;
		public bool useGraivtyOnImpact;

		public bool breakThroughObjects;
		public bool infiniteNumberOfImpacts;
		public int numberOfImpacts;
		public bool canDamageSameObjectMultipleTimes;

		public bool shakeUpperBodyWhileShooting;
		public float shakeAmount;
		public float shakeSpeed;

		public bool useMuzzleFlash;
		public Light muzzleFlahsLight;
		public float muzzleFlahsDuration;

		public bool damageTargetOverTime;
		public float damageOverTimeDelay;
		public float damageOverTimeDuration;
		public float damageOverTimeAmount;
		public float damageOverTimeRate;
		public bool damageOverTimeToDeath;
		public bool removeDamageOverTimeState;

		public bool sedateCharacters;
		public float sedateDelay;
		public bool useWeakSpotToReduceDelay;
		public bool sedateUntilReceiveDamage;
		public float sedateDuration;

		public bool pushCharacter;
		public float pushCharacterForce;
		public float pushCharacterRagdollForce;

		public bool useNoise;
		public float noiseRadius;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public LayerMask noiseDetectionLayer;
		public bool showNoiseDetectionGizmo;
	}
}