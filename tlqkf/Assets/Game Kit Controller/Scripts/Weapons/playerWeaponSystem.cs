using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class playerWeaponSystem : MonoBehaviour
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

	public bool useHumanBodyBonesEnum;
	public HumanBodyBones weaponParentBone;

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
	public playerWeaponsManager weaponsManager;
	public playerCamera playerCameraManager;
	public launchTrayectory parable;
	public gravitySystem gravitySystemManager;
	public Collider playerCollider;
	public Camera mainCamera;
	public Transform mainCameraTransform;
	public headBob headBobManager;
	public Animation weaponAnimation;
	public AudioSource weaponEffectsSource;

	public bool useEventOnSetDualWeapon;
	public UnityEvent eventOnSetRightWeapon;
	public UnityEvent eventOnSetLeftWeapon;

	public bool useEventOnSetSingleWeapon;
	public UnityEvent eventOnSetSingleWeapon;

	List<GameObject> shells = new List<GameObject> ();
	float destroyShellsTimer = 0;

	GameObject newShellClone;
	GameObject currentShellToRemove;
	AudioClip newClipToShell;

	RaycastHit hit;
	RaycastHit hitCamera;
	RaycastHit hitWeapon;
	float lastShoot;

	AudioSource currentWeaponEffectsSource;
	List<AudioSource> weaponEffectsSourceList = new List<AudioSource> ();

	bool animationForwardPlayed;
	bool animationBackPlayed;
	Transform originalParent;

	bool shellCreated;

	bool weaponHasAnimation;
	Vector3 forceDirection;
	GameObject closestEnemy;
	bool aimingHommingProjectile;

	List<GameObject> locatedEnemies = new List<GameObject> ();

	float weaponSpeed = 1;
	float originalWeaponSpeed;

	Vector3 aimedZone;
	bool objectiveFound;

	bool carryingWeaponPreviously;

	bool silencerActive;

	int originalClipSize;
	float originalFireRate;
	float originalProjectileDamage;
	bool originalAutomaticMode;
	bool orignalUseBurst;
	bool originalSpreadState;
	bool originalIsExplosiveState;
	bool originalDamageTargetOverTimeState;
	bool originalRemoveDamageTargetOverTimeState;
	bool originalSedateCharactersState;
	bool originalPushCharacterState;
	bool originalProjectileWithAbilityState;

	bool usingSight;

	Coroutine muzzleFlahsCoroutine;
	GameObject newMuzzleParticlesClone;

	Rigidbody currentProjectileRigidbody;
	GameObject newProjectileGameObject;
	projectileInfo newProjectileInfo;

	GameObject newClipToDrop;
	Collider newClipCollider;

	bool checkReloadWeaponState;

	bool usingScreenSpaceCamera;
	bool targetOnScreen;
	Vector3 screenPoint;

	playerScreenObjectivesSystem playerScreenObjectivesManager;

	GameObject originalWeaponProjectile;

	bool shootingBurst;
	int currentBurstAmount;

	bool surfaceFound;
	RaycastHit surfaceFoundHit;

	bool usingDualWeapon;
	bool usingRightHandDualWeapon;

	int originalNumberKey = -1;

	bool pauseDrawKeepWeaponSounds;

	void Start ()
	{
		gameObject.name += " (" + getWeaponSystemName () + ") ";
		currentWeaponEffectsSource = weaponEffectsSource;

		weaponSettings.ammoPerClip = weaponSettings.clipSize;
		originalClipSize = weaponSettings.ammoPerClip;
		if (weaponSettings.startWithEmptyClip) {
			weaponSettings.clipSize = 0;
		}
		originalParent = transform.parent;

		Transform weaponParent = weaponSettings.weaponParent;
		if (!playerCameraManager.isFirstPersonActive ()) {
			transform.SetParent (weaponParent);
		}

		GameObject keepPositionsParent = new GameObject ();

		Transform keepPositionsParentTransform = keepPositionsParent.transform;
		keepPositionsParentTransform.SetParent (weaponParent);
		keepPositionsParentTransform.name = getWeaponSystemName () + " Keep Positions Parent";
		keepPositionsParentTransform.localScale = Vector3.one;
		keepPositionsParentTransform.localPosition = Vector3.zero;
		keepPositionsParentTransform.localRotation = Quaternion.identity;

		IKWeaponManager.thirdPersonWeaponInfo.keepPosition.SetParent (keepPositionsParentTransform);

		IKWeaponManager.thirdPersonWeaponInfo.keepPosition.name = "Keep Position Single Weapon";

		if (IKWeaponManager.thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition) {
			IKWeaponManager.thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition.SetParent (keepPositionsParentTransform);

			IKWeaponManager.thirdPersonWeaponInfo.rightHandDualWeaopnInfo.keepPosition.name = "Keep Position Right Dual Weapon";
		}

		if (IKWeaponManager.thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition) {
			IKWeaponManager.thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition.SetParent (keepPositionsParentTransform);
			IKWeaponManager.thirdPersonWeaponInfo.leftHandDualWeaponInfo.keepPosition.name = "Keep Position Left Dual Weapon";
		}

		enableHUD (false);

		if (weaponSettings.animation != "") {
			weaponHasAnimation = true;
			weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed; 
		} else {
			shellCreated = true;
		}

		if (weaponSettings.useReloadAnimation) {
			weaponHasAnimation = true;
		}

		originalWeaponSpeed = weaponSpeed;

		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;

		//get original values from the weapon
		originalFireRate = weaponSettings.fireRate;
		originalProjectileDamage = weaponSettings.projectileDamage;
		originalAutomaticMode = weaponSettings.automatic;
		orignalUseBurst = weaponSettings.useBurst;
		originalSpreadState = weaponSettings.useProjectileSpread;
		originalIsExplosiveState = weaponSettings.isExplosive;
		originalDamageTargetOverTimeState = weaponSettings.damageTargetOverTime;
		originalRemoveDamageTargetOverTimeState = weaponSettings.removeDamageOverTimeState;
		originalSedateCharactersState = weaponSettings.sedateCharacters;
		originalPushCharacterState = weaponSettings.pushCharacter;
		originalProjectileWithAbilityState = weaponSettings.projectileWithAbility;

		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

		playerScreenObjectivesManager = weaponsManager.getPlayerScreenObjectivesManager ();

		originalWeaponProjectile = weaponProjectile;
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
			if (!shellCreated && !weaponSettings.createShellsOnReload &&
			    ((weaponHasAnimation && animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) || !weaponHasAnimation || !weaponSettings.useFireAnimation)) {
				createShells ();
			}

			if (!reloading) {
				if (weaponHasAnimation) {
					if (weaponSettings.useFireAnimation) {
						if (weaponSettings.clipSize > 0 || !weaponSettings.weaponUsesAmmo) {
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

							checkReloadWeaponState = false;
						} else {
							checkReloadWeaponState = true;
						}
					} else {
						checkReloadWeaponState = true;
					}
				} else {
					checkReloadWeaponState = true;
				}

				if (checkReloadWeaponState) {
					if (weaponSettings.clipSize == 0 && weaponSettings.weaponUsesAmmo) {
						if ((weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) && weaponSettings.autoReloadWhenClipEmpty) {
							reloadWeapon ();
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
					if ((!animationForwardPlayed && !animationBackPlayed && weaponHasAnimation) || !weaponHasAnimation || !weaponSettings.useFireAnimation) {
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

	public void setWeaponCarryStateAtOnce (bool state)
	{
		carryingWeaponInThirdPerson = state;
		carryingWeaponInFirstPerson = state;
	}

	public void setWeaponCarryState (bool thirdPersonCarry, bool firstPersonCarry)
	{
		carryingWeaponPreviously = carryingWeaponInThirdPerson || carryingWeaponInFirstPerson;

		carryingWeaponInThirdPerson = thirdPersonCarry;
		carryingWeaponInFirstPerson = firstPersonCarry;
		checkParableTrayectory (false, carryingWeaponInFirstPerson);
		checkWeaponAbility (false);

		//functions called when the player draws or keep the weapon in any view
		if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
			if (weaponSettings.useStartDrawAction) {
				weaponSettings.startDrawAction.Invoke ();
			}
		} else {
			if (weaponSettings.useStopDrawAction) {
				weaponSettings.stopDrawAction.Invoke ();
			}
		}

		if (playerCameraManager) {
			if (!playerCameraManager.isFirstPersonActive ()) {
				if (carryingWeaponInThirdPerson) {
					if (weaponSettings.useStartDrawActionThirdPerson) {
						weaponSettings.startDrawActionThirdPerson.Invoke ();
					}
				} else {
					if (weaponSettings.useStopDrawActionThirdPerson) {
						weaponSettings.stopDrawActionThirdPerson.Invoke ();
					}
				}
			}

			if (playerCameraManager.isFirstPersonActive ()) {
				if (carryingWeaponInFirstPerson) {
					if (weaponSettings.useStartDrawActionFirstPerson) {
						weaponSettings.startDrawActionFirstPerson.Invoke ();
					}
				} else {
					if (weaponSettings.useStopDrawActionFirstPerson) {
						weaponSettings.stopDrawActionFirstPerson.Invoke ();
					}
				}
			}
		}

		if (!pauseDrawKeepWeaponSounds && weaponSettings.useSoundOnDrawKeepWeapon) {
			if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
				playSound (weaponSettings.drawWeaponSound);
			} else {
				if (carryingWeaponPreviously) {
					playSound (weaponSettings.keepWeaponSound);
				}
			}
		}

		pauseDrawKeepWeaponSounds = false;
	}

	public void setPauseDrawKeepWeaponSound ()
	{
		pauseDrawKeepWeaponSounds = true;
	}

	public void setWeaponAimState (bool thirdPersonAim, bool firstPersonAim)
	{
		aimingInThirdPerson = thirdPersonAim;
		aimingInFirstPerson = firstPersonAim;

		if ((aimingInThirdPerson || aimingInFirstPerson) && weaponSettings.clipSize == 0 && weaponSettings.weaponUsesAmmo) {
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
					weaponSettings.startAimActionThirdPerson.Invoke ();
				}
			} else {
				if (weaponSettings.useStopAimActionThirdPerson) {
					weaponSettings.stopAimActionThirdPerson.Invoke ();
				}
			}
		}

		if (playerCameraManager.isFirstPersonActive ()) {
			if (aimingInFirstPerson) {
				if (weaponSettings.useStartAimActionFirstPerson) {
					weaponSettings.startAimActionFirstPerson.Invoke ();
				}
			} else {
				if (weaponSettings.useStopAimActionFirstPerson) {
					weaponSettings.stopAimActionFirstPerson.Invoke ();
				}
			}
		}
	}

	public IKWeaponSystem getIKWeaponSystem ()
	{
		return IKWeaponManager;
	}

	public bool weaponIsMoving ()
	{
		return IKWeaponManager.isWeaponMoving ();
	}

	public bool isWeaponOnRecoil ()
	{
		return IKWeaponManager.isWeaponOnRecoil ();
	}

	public bool isAimingWeapon ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool carryingWeapon ()
	{
		return carryingWeaponInFirstPerson || carryingWeaponInThirdPerson;
	}

	public bool isShootingBurst ()
	{
		return shootingBurst;
	}

	//fire the current weapon
	public void shootWeapon (bool isThirdPersonView, bool shootAtKeyDown)
	{
		if (!IKWeaponManager.weaponCanFire ()) {
			return;
		}
			
		if (IKWeaponManager.isWeaponMoving ()) {
			return;
		}

		if (reloading) {
			return;
		}

		checkWeaponAbility (shootAtKeyDown);

		//if the weapon system is active and the clip size higher than 0
		if (weaponSettings.clipSize > 0 || !weaponSettings.weaponUsesAmmo) {
			//else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + weaponSettings.fireRate) {

				//if the player fires a weapon, set the visible to AI state to true, this will change if the player is using a silencer
				if (weaponSettings.shootAProjectile || weaponSettings.launchProjectile) {
					weaponsManager.setVisibleToAIState (true);
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

				if ((animationForwardPlayed || animationBackPlayed) && weaponHasAnimation && weaponSettings.useFireAnimation) {
					return;
				}
					
				if (weaponSettings.automatic && weaponSettings.useBurst) {
					if (!shootingBurst && weaponSettings.burstAmount > 0) {
						shootingBurst = true;
						currentBurstAmount = weaponSettings.burstAmount;
					}
				}
					
				//camera shake
				checkWeaponCameraShake ();

				//recoil
				IKWeaponManager.startRecoil (isThirdPersonView);

				IKWeaponManager.setLastTimeMoved ();

				if (usingDualWeapon) {
					if (weaponSettings.shakeUpperBodyShootingDualWeapons) {
						weaponsManager.checkDualWeaponShakeUpperBodyRotationCoroutine (weaponSettings.dualWeaponShakeAmount, weaponSettings.dualWeaponShakeSpeed, usingRightHandDualWeapon);
					}
				} else {
					if (weaponSettings.shakeUpperBodyWhileShooting) {
						weaponsManager.checkShakeUpperBodyRotationCoroutine (weaponSettings.shakeAmount, weaponSettings.shakeSpeed);
					}
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
				if (weaponHasAnimation && weaponSettings.useFireAnimation) {
					weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed;
					weaponAnimation.Play (weaponSettings.animation);
					animationForwardPlayed = true;
					if (weaponSettings.cockSound) {
						playSound (weaponSettings.cockSound);
					}
				} 

				if (!weaponSettings.createShellsOnReload) {
					shellCreated = false;
				}

				Vector3 cameraDirection = mainCameraTransform.TransformDirection (Vector3.forward);
				//every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
					for (int l = 0; l < weaponSettings.projectilesPerShoot; l++) {

						surfaceFound = false;

						//create the projectile
						newProjectileGameObject = (GameObject)Instantiate (weaponProjectile, weaponSettings.projectilePosition [j].position, mainCameraTransform.rotation);

						if (!weaponSettings.launchProjectile) {
							//set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (!weaponCrossingSurface) {
								if (!weaponSettings.fireWeaponForward) {

									if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, Mathf.Infinity, layer)) {
										if (!hit.collider.isTrigger) {
											if (hit.collider != playerCollider) {
												//Debug.DrawLine (weaponSettings.projectilePosition [j].position, hit.point, Color.red, 2);
												newProjectileGameObject.transform.LookAt (hit.point);

												surfaceFound = true;
												surfaceFoundHit = hit;
											} else {
												if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layer)) {
													newProjectileGameObject.transform.LookAt (hit.point);

													surfaceFound = true;
													surfaceFoundHit = hit;
												}
											}
										}
									}
								}
							}
						}

						if (weaponSettings.launchProjectile) {
							currentProjectileRigidbody = newProjectileGameObject.GetComponent<Rigidbody> ();

							currentProjectileRigidbody.isKinematic = true;
							//if the vehicle has a gravity control component, and the current gravity is not the regular one, add an artifical gravity component to the projectile
							//like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle

							if (gravitySystemManager.getCurrentNormal () != Vector3.up) {
								newProjectileGameObject.AddComponent<artificialObjectGravity> ().setCurrentGravity (-gravitySystemManager.getCurrentNormal ());
							}
								
							if (weaponSettings.useParableSpeed) {
								//get the ray hit point where the projectile will fall
								if (surfaceFound) {
									aimedZone = surfaceFoundHit.point;
									objectiveFound = true;
								} else {
									if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, Mathf.Infinity, layer)) {
										if (hit.collider != playerCollider) {
											aimedZone = hit.point;
											objectiveFound = true;
										} else {
											if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layer)) {
												aimedZone = hit.point;
												objectiveFound = true;
											}
										}
									} else {
										objectiveFound = false;
									}
								}
							}

							launchCurrentProjectile (newProjectileGameObject, currentProjectileRigidbody);
						}

						//add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;
						if (weaponSettings.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							newProjectileGameObject.transform.Rotate (spreadAmount);
						}

						//set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						projectileSystem currentProjectileSystem = newProjectileGameObject.GetComponent<projectileSystem> ();
						currentProjectileSystem.setProjectileInfo (setProjectileInfo ());

						if (weaponSettings.isSeeker) {
							setSeekerProjectileInfo (newProjectileGameObject, weaponSettings.projectilePosition [j]);
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
							Vector3 forwardDirection = cameraDirection;
							Vector3 forwardPositon = mainCameraPosition;
							if (weaponSettings.fireWeaponForward && !weaponCrossingSurface) {
								forwardDirection = transform.forward;
								forwardPositon = weaponSettings.projectilePosition [j].position;
							}

							if (spreadAmount.magnitude != 0) {
								forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
							}

							if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layer)) {
								if (hit.collider != playerCollider) {
									if (weaponSettings.useFakeProjectileTrails) {
										currentProjectileSystem.creatFakeProjectileTrail (hit.point);
									}

									currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);
								} else {
									if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, Mathf.Infinity, layer)) {

										if (weaponSettings.useFakeProjectileTrails) {
											currentProjectileSystem.creatFakeProjectileTrail (hit.point);
										}

										currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);
									}
								}
								//print ("same object fired: " + hit.collider.name);
							} else {
								if (weaponSettings.useFakeProjectileTrails) {
									currentProjectileSystem.setFakeProjectileTrailSpeedMultiplier (0.3f);
									currentProjectileSystem.setDestroyTrailAfterTimeState (true);
									currentProjectileSystem.creatFakeProjectileTrail (forwardPositon + forwardDirection * 50);
								}

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
					weaponsManager.externalForce (forceDirection);
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

				//check if player changes automatically to next weapon with ammo
				if (weaponsManager.changeToNextWeaponIfAmmoEmpty) {
					weaponsManager.changeToNextWeaponWithAmmo ();
				}
			}
		}
	}

	public void checkWeaponCameraShake ()
	{
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
	}

	public void checkMeleeAttackShakeInfo ()
	{
		if (weaponsManager.carryingWeaponInFirstPerson && IKWeaponManager.firstPersonWeaponInfo.useMeleeAttackShakeInfo) {
			headBobManager.setShotShakeState (IKWeaponManager.firstPersonMeleeAttackShakeInfo);
		}

		if (weaponsManager.carryingWeaponInThirdPerson && IKWeaponManager.thirdPersonWeaponInfo.useMeleeAttackShakeInfo) {
			headBobManager.setShotShakeState (IKWeaponManager.thirdPersonMeleeAttackShakeInfo);
		}
	}

	public void checkWeaponAbility (bool keyDown)
	{
		if (weaponSettings.weaponWithAbility) {
			if (keyDown) {
				if (weaponSettings.useDownButton) {
					weaponSettings.downButtonAction.Invoke ();
				}
			} else {
				if (weaponSettings.useUpButton) {
					weaponSettings.upButtonAction.Invoke ();
				}
			}
		}
	}

	public void checkWeaponAbilityHoldButton ()
	{
		if (weaponSettings.weaponWithAbility) {
			if (weaponSettings.useHoldButton) {
				weaponSettings.holdButtonAction.Invoke ();
			}
		}
	}

	public void activateSecondaryAction ()
	{
		if (weaponSettings.useSecondaryAction) {
			weaponSettings.secondaryAction.Invoke ();
		}
	}

	public void activateSecondaryActionOnDownPress ()
	{
		if (weaponSettings.useSecondaryActionOnDownPress) {
			weaponSettings.secondaryActionOnDownPress.Invoke ();
		}

	}

	public void activateSecondaryActionOnUpPress ()
	{
		if (weaponSettings.useSecondaryActionOnUpPress) {
			weaponSettings.secondaryActionOnUpPress.Invoke ();
		}
	}

	public void activateSecondaryActionOnUpHold ()
	{
		if (weaponSettings.useSecondaryActionOnHoldPress) {
			weaponSettings.secondaryActionOnHoldPress.Invoke ();
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
		newProjectileInfo = new projectileInfo ();

		newProjectileInfo.isHommingProjectile = weaponSettings.isHommingProjectile;
		newProjectileInfo.isSeeker = weaponSettings.isSeeker;
		newProjectileInfo.waitTimeToSearchTarget = weaponSettings.waitTimeToSearchTarget;
		newProjectileInfo.useRayCastShoot = weaponSettings.useRayCastShoot;

		newProjectileInfo.useRaycastShootDelay = weaponSettings.useRaycastShootDelay;
		newProjectileInfo.raycastShootDelay = weaponSettings.raycastShootDelay;
		newProjectileInfo.getDelayWithDistance = weaponSettings.getDelayWithDistance;
		newProjectileInfo.delayWithDistanceSpeed = weaponSettings.delayWithDistanceSpeed;
		newProjectileInfo.maxDelayWithDistance = weaponSettings.maxDelayWithDistance;

		newProjectileInfo.useFakeProjectileTrails = weaponSettings.useFakeProjectileTrails;

		newProjectileInfo.useRaycastCheckingOnRigidbody = weaponSettings.useRaycastCheckingOnRigidbody;

		newProjectileInfo.projectileDamage = weaponSettings.projectileDamage;
		newProjectileInfo.projectileSpeed = weaponSettings.projectileSpeed;

		newProjectileInfo.impactForceApplied = weaponSettings.impactForceApplied;
		newProjectileInfo.forceMode = weaponSettings.forceMode;
		newProjectileInfo.applyImpactForceToVehicles = weaponSettings.applyImpactForceToVehicles;
		newProjectileInfo.impactForceToVehiclesMultiplier = weaponSettings.impactForceToVehiclesMultiplier;

		newProjectileInfo.projectileWithAbility = weaponSettings.projectileWithAbility;

		newProjectileInfo.impactSoundEffect = weaponSettings.impactSoundEffect;

		newProjectileInfo.scorch = weaponSettings.scorch;
		newProjectileInfo.scorchRayCastDistance = weaponSettings.scorchRayCastDistance;

		newProjectileInfo.owner = playerControllerGameObject;

		newProjectileInfo.projectileParticles = weaponSettings.projectileParticles;
		newProjectileInfo.impactParticles = weaponSettings.impactParticles;

		newProjectileInfo.isExplosive = weaponSettings.isExplosive;
		newProjectileInfo.isImplosive = weaponSettings.isImplosive;
		newProjectileInfo.useExplosionDelay = weaponSettings.useExplosionDelay;
		newProjectileInfo.explosionDelay = weaponSettings.explosionDelay;
		newProjectileInfo.explosionForce = weaponSettings.explosionForce;
		newProjectileInfo.explosionRadius = weaponSettings.explosionRadius;
		newProjectileInfo.explosionDamage = weaponSettings.explosionDamage;
		newProjectileInfo.pushCharacters = weaponSettings.pushCharacters;
		newProjectileInfo.canDamageProjectileOwner = weaponSettings.canDamageProjectileOwner;
		newProjectileInfo.applyExplosionForceToVehicles = weaponSettings.applyExplosionForceToVehicles;
		newProjectileInfo.explosionForceToVehiclesMultiplier = weaponSettings.explosionForceToVehiclesMultiplier;

		newProjectileInfo.killInOneShot = weaponSettings.killInOneShot;

		newProjectileInfo.useDisableTimer = weaponSettings.useDisableTimer;
		newProjectileInfo.noImpactDisableTimer = weaponSettings.noImpactDisableTimer;
		newProjectileInfo.impactDisableTimer = weaponSettings.impactDisableTimer;

		newProjectileInfo.targetToDamageLayer = weaponsManager.targetToDamageLayer;
		newProjectileInfo.targetForScorchLayer = weaponsManager.targetForScorchLayer;

		newProjectileInfo.impactDecalIndex = impactDecalIndex;

		newProjectileInfo.launchProjectile = weaponSettings.launchProjectile;

		newProjectileInfo.adhereToSurface = weaponSettings.adhereToSurface;
		newProjectileInfo.adhereToLimbs = weaponSettings.adhereToLimbs;

		newProjectileInfo.useGravityOnLaunch = weaponSettings.useGravityOnLaunch;
		newProjectileInfo.useGraivtyOnImpact = weaponSettings.useGraivtyOnImpact;

		newProjectileInfo.breakThroughObjects = weaponSettings.breakThroughObjects;
		newProjectileInfo.infiniteNumberOfImpacts = weaponSettings.infiniteNumberOfImpacts;
		newProjectileInfo.numberOfImpacts = weaponSettings.numberOfImpacts;
		newProjectileInfo.canDamageSameObjectMultipleTimes = weaponSettings.canDamageSameObjectMultipleTimes;
		newProjectileInfo.forwardDirection = mainCameraTransform.forward;

		newProjectileInfo.damageTargetOverTime = weaponSettings.damageTargetOverTime;
		newProjectileInfo.damageOverTimeDelay = weaponSettings.damageOverTimeDelay;
		newProjectileInfo.damageOverTimeDuration = weaponSettings.damageOverTimeDuration;
		newProjectileInfo.damageOverTimeAmount = weaponSettings.damageOverTimeAmount;
		newProjectileInfo.damageOverTimeRate = weaponSettings.damageOverTimeRate;
		newProjectileInfo.damageOverTimeToDeath = weaponSettings.damageOverTimeToDeath;
		newProjectileInfo.removeDamageOverTimeState = weaponSettings.removeDamageOverTimeState;

		newProjectileInfo.sedateCharacters = weaponSettings.sedateCharacters;
		newProjectileInfo.sedateDelay = weaponSettings.sedateDelay;
		newProjectileInfo.useWeakSpotToReduceDelay = weaponSettings.useWeakSpotToReduceDelay;
		newProjectileInfo.sedateDuration = weaponSettings.sedateDuration;
		newProjectileInfo.sedateUntilReceiveDamage = weaponSettings.sedateUntilReceiveDamage;

		newProjectileInfo.pushCharacter = weaponSettings.pushCharacter;
		newProjectileInfo.pushCharacterForce = weaponSettings.pushCharacterForce;
		newProjectileInfo.pushCharacterRagdollForce = weaponSettings.pushCharacterRagdollForce;

		return newProjectileInfo;
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

	public void createShells ()
	{
		shellCreated = true;

		if (weaponSettings.useShellDelay) {
			createShellsWithDelay ();
		} else {
			createShellsAtOnce ();
		}
	}

	Coroutine shellCoroutine;

	public void createShellsWithDelay ()
	{
		if (shellCoroutine != null) {
			StopCoroutine (shellCoroutine);
		}
		shellCoroutine = StartCoroutine (createShellsWithDelayCoroutine ());
	}

	//a delay for reload the weapon
	IEnumerator createShellsWithDelayCoroutine ()
	{
		bool firtPersonActive = weaponsManager.isFirstPersonActive ();

		if (firtPersonActive) {
			yield return new WaitForSeconds (weaponSettings.shellDelayFirsPerson);
		} else {
			yield return new WaitForSeconds (weaponSettings.shellDelayThirdPerson);
		}

		createShellsAtOnce ();
	}

	void createShellsAtOnce ()
	{
		for (int j = 0; j < weaponSettings.shellPosition.Count; j++) {
			//if the current weapon drops shells, create them
			if (weaponSettings.shell) {
				newShellClone = (GameObject)Instantiate (weaponSettings.shell, weaponSettings.shellPosition [j].position, weaponSettings.shellPosition [j].rotation);
				weaponShellSystem newWeaponShellSystem = newShellClone.GetComponent<weaponShellSystem> ();
				if (weaponSettings.shellDropSoundList.Count > 0) {
					newClipToShell = weaponSettings.shellDropSoundList [Random.Range (0, weaponSettings.shellDropSoundList.Count - 1)];
				}

				newWeaponShellSystem.setShellValues (weaponSettings.shellPosition [j].right * weaponSettings.shellEjectionForce * GKC_Utils.getCurrentScaleTime (), playerCollider, newClipToShell);
					
				shells.Add (newShellClone);

				if (shells.Count > 15) {
					currentShellToRemove = shells [0];
					shells.RemoveAt (0);
					Destroy (currentShellToRemove);
				}
			}
		}
	}

	//play the fire sound or the empty clip sound
	void playWeaponSoundEffect (bool hasAmmo)
	{
		if (weaponSettings.useSoundsPool) {
			currentWeaponEffectsSource = getAudioSourceFromPool ();
		} 

		if (hasAmmo) {
			if (weaponSettings.shootSoundEffect) {
				if (silencerActive) {
					currentWeaponEffectsSource.clip = weaponSettings.silencerShootEffect;
				} else {
					currentWeaponEffectsSource.clip = weaponSettings.shootSoundEffect;
				}
				currentWeaponEffectsSource.pitch = weaponSpeed;
				currentWeaponEffectsSource.Play ();
			}
		} else {
			if (Time.time > lastShoot + weaponSettings.fireRate) {
				currentWeaponEffectsSource.pitch = weaponSpeed;
				GKC_Utils.checkAudioSourcePitch (currentWeaponEffectsSource);
				currentWeaponEffectsSource.PlayOneShot (outOfAmmo);
				lastShoot = Time.time;
			}
		}
	}

	public void playSound (AudioClip clipSound)
	{
		if (weaponEffectsSource) {
			GKC_Utils.checkAudioSourcePitch (weaponEffectsSource);
			weaponEffectsSource.PlayOneShot (clipSound);
		}
	}

	AudioSource currentAudioSource;
	bool createNewWeaponEffectSource;

	public AudioSource getAudioSourceFromPool ()
	{
		createNewWeaponEffectSource = false;
		if (weaponEffectsSourceList.Count < weaponSettings.maxSoundsPoolAmount) {
			createNewWeaponEffectSource = true;

		} else {
			for (int j = 0; j < weaponEffectsSourceList.Count; j++) {
				if (!weaponEffectsSourceList [j].isPlaying) {
					return weaponEffectsSourceList [j];
				}
			}
				
			weaponSettings.maxSoundsPoolAmount++;
			createNewWeaponEffectSource = true;
		}

		if (createNewWeaponEffectSource) {
			GameObject newWeaponEffectSource = (GameObject)Instantiate (weaponSettings.weaponEffectSourcePrefab, transform.position, Quaternion.identity);
			newWeaponEffectSource.transform.SetParent (weaponSettings.weaponEffectSourceParent);
			currentAudioSource = newWeaponEffectSource.GetComponent<AudioSource> ();
			weaponEffectsSourceList.Add (currentAudioSource);
			return currentAudioSource;
		}

		return null;
	}

	//create the muzzle flash particles if the weapon has it
	void createMuzzleFlash ()
	{
		if (weaponSettings.shootParticles) {
			for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
				newMuzzleParticlesClone = (GameObject)Instantiate (weaponSettings.shootParticles, weaponSettings.projectilePosition [j].position, weaponSettings.projectilePosition [j].rotation);
				Destroy (newMuzzleParticlesClone, 1);	
				newMuzzleParticlesClone.transform.SetParent (weaponSettings.projectilePosition [j]);
				weaponSettings.shootParticles.GetComponent<ParticleSystem> ().Play ();
			}
		}
	}

	//	//decrease the amount of ammo in the clip
	public void useAmmo ()
	{
		weaponSettings.clipSize--;

		updateAmmoInfo ();

		//update hud ammo info
		weaponsManager.updateAmmo ();
	}

	public void useAmmo (int amount)
	{
		if (amount > weaponSettings.clipSize) {
			amount = weaponSettings.clipSize;
		}

		weaponSettings.clipSize -= amount;

		updateAmmoInfo ();

		//update hud ammo info
		weaponsManager.updateAmmo ();
	}

	void updateAmmoInfo ()
	{
		if (weaponSettings.HUD) {
			weaponSettings.clipSizeText.text = weaponSettings.clipSize.ToString ();
			if (!weaponSettings.infiniteAmmo) {
				weaponSettings.remainAmmoText.text = weaponSettings.remainAmmo.ToString ();
			} else {
				weaponSettings.remainAmmoText.text = "Inf";
			}
		}
	}

	//check the amount of ammo
	void checkRemainAmmo ()
	{
		if (!weaponSettings.weaponUsesAmmo) {
			return;
		}

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

		updateAmmoInfo ();
		weaponsManager.updateAmmo ();
		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;
	}

	Coroutine reloadCoroutine;

	public void reloadWeapon ()
	{
		stopReloadCoroutine ();

		reloadCoroutine = StartCoroutine (waitToReload ());
	}

	public void stopReloadCoroutine ()
	{
		if (reloadCoroutine != null) {
			StopCoroutine (reloadCoroutine);
		}
	}

	public void stopReloadAction ()
	{
		//check the ammo values
		checkRemainAmmo ();

		//stop reload
		reloading = false;

		if (weaponSettings.useReloadAnimation) {
			if (weaponHasAnimation) {
				weaponAnimation.Rewind (weaponSettings.reloadAnimationName);
			}
		}
	}

	//a delay for reload the weapon
	IEnumerator waitToReload ()
	{
		//print ("reload");
		//if the remmaining ammo is higher than 0 or infinite
		shootingBurst = false;

		if (weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) {
			//reload
			reloading = true;

			bool firtPersonActive = weaponsManager.isFirstPersonActive ();

			if (firtPersonActive) {
				if (weaponSettings.usePreReloadDelayFirstPerson) {
					yield return new WaitForSeconds (weaponSettings.preReloadDelayFirstPerson);
				}
			} else {
				if (weaponSettings.usePreReloadDelayThirdPerson) {
					yield return new WaitForSeconds (weaponSettings.preReloadDelayThirdPerson);
				}
			}

			if (weaponSettings.createShellsOnReload) {
				createShells ();
			}

			if (firtPersonActive) {
				IKWeaponManager.reloadWeaponFirstPerson ();
			}

			if (firtPersonActive) {
				if (weaponSettings.useReloadDelayFirstPerson) {
					yield return new WaitForSeconds (weaponSettings.reloadDelayFirstPerson);
				}
			} else {
				if (weaponSettings.useReloadDelayThirdPerson) {
					yield return new WaitForSeconds (weaponSettings.reloadDelayThirdPerson);
				}
			}

			if (weaponSettings.useReloadAnimation) {
				if (weaponHasAnimation) {
					weaponAnimation [weaponSettings.reloadAnimationName].speed = weaponSettings.reloadAnimationSpeed;
					weaponAnimation.Play (weaponSettings.reloadAnimationName);
				}
			}

			//play the reload sound
			if (weaponSettings.reloadSoundEffect) {
				playSound (weaponSettings.reloadSoundEffect);
			}

			if (weaponSettings.dropClipWhenReload) {
				newClipToDrop = (GameObject)Instantiate (weaponSettings.clipModel, weaponSettings.positionToDropClip.position, weaponSettings.positionToDropClip.rotation);

				newClipCollider = newClipToDrop.GetComponent<Collider> ();
				if (newClipCollider) {
					Physics.IgnoreCollision (playerCollider, newClipCollider);
				}
			}

			//wait an amount of time
			if (firtPersonActive) {
				yield return new WaitForSeconds (weaponSettings.reloadTimeFirstPerson);
			} else {
				yield return new WaitForSeconds (weaponSettings.reloadTimeThirdPerson);
			}

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
		if (!reloading && weaponSettings.weaponUsesAmmo) {
			if (weaponSettings.clipSize < weaponSettings.ammoPerClip) {
				reloadWeapon ();
			}
		}
	}

	public void enableHUD (bool state)
	{
		if (weaponSettings.HUD) {
			if (weaponSettings.useHUD) {
				if (usingDualWeapon) {
					if ((carryingWeaponInThirdPerson && weaponSettings.useHUDDualWeaponThirdPerson) || (carryingWeaponInFirstPerson && weaponSettings.useHUDDualWeaponFirstPerson)) {
						weaponSettings.HUD.SetActive (state);
					}
				} else {
					weaponSettings.HUD.SetActive (state);
				}

				updateAmmoInfo ();
			} else {
				weaponSettings.HUD.SetActive (false);
			}
		}
	}

	public void enableHUDTemporarily (bool state)
	{
		if (weaponSettings.HUD) {
			if (weaponSettings.useHUD) {
				weaponSettings.HUD.SetActive (state);
			}
		}
	}

	public void changeHUDPosition (bool thirdPerson)
	{
		if (weaponSettings.HUD && weaponSettings.useHUD) {
			if (usingDualWeapon) {
				if (weaponSettings.changeHUDPositionDualWeapon) {
					if (thirdPerson) {
						if (usingRightHandDualWeapon) {
							weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDRightHandTransformThirdPerson.position;
						} else {
							weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDLeftHandTransformThirdPerson.position;
						}
					} else {
						if (usingRightHandDualWeapon) {
							weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDRightHandTransformFirstPerson.position;
						} else {
							weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDLeftHandTransformFirstPerson.position;
						}
					}
				}
			} else {
				if (weaponSettings.changeHUDPosition) {
					if (thirdPerson) {
						weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDTransformInThirdPerson.position;
					} else {
						weaponSettings.ammoInfoHUD.transform.position = weaponSettings.HUDTransformInFirstPerson.position;
					}
				}
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

	//the player has used an ammo pickup, so increase the weapon ammo
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
		updateAmmoInfo ();
	}

	public void addAuxRemainAmmo (int amount)
	{
		weaponSettings.auxRemainAmmo += amount;
	}

	public bool hasAnyAmmo ()
	{
		if (weaponSettings.remainAmmo > 0 || weaponSettings.clipSize > 0 || weaponSettings.infiniteAmmo || !weaponSettings.weaponUsesAmmo) {
			return true;
		}
		return false;
	}

	public void getAndUpdateAmmo (int amount)
	{
		getAmmo (amount);
		weaponsManager.updateAmmo ();
	}

	public bool remainAmmoInClip ()
	{
		return weaponSettings.clipSize > 0;
	}

	public void launchCurrentProjectile (GameObject currentProjectile, Rigidbody projectileRigidbody)
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		projectileRigidbody.isKinematic = false;
		if (weaponSettings.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone);
			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}
			projectileRigidbody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			projectileRigidbody.AddForce (weaponSettings.parableDirectionTransform.forward * weaponSettings.projectileSpeed, ForceMode.Impulse);
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

	public Transform getWeaponParent ()
	{
		return originalParent;
	}

	public void setCharacter (GameObject pController, GameObject pCamera)
	{
		playerControllerGameObject = pController;
		playerCameraGameObject = pCamera;
		updateComponent ();
	}

	public void setWeaponParent (Transform parent, Animator animatorToUse)
	{
		weaponSettings.weaponParent = parent;
		if (useHumanBodyBonesEnum) {
			weaponSettings.weaponParent = animatorToUse.GetBoneTransform (weaponParentBone);
		}
		updateComponent ();
	}

	public Transform getWeaponsParent ()
	{
		if (weaponSettings.weaponParent) {
			return weaponSettings.weaponParent;
		}
		return null;
	}

	public Transform getMainCameraTransform ()
	{
		return mainCameraTransform;
	}

	public void getWeaponComponents ()
	{
		weaponsManager = playerControllerGameObject.GetComponent<playerWeaponsManager> ();
		playerCameraManager = playerCameraGameObject.GetComponent<playerCamera> ();
		mainCameraTransform = playerCameraManager.getCameraTransform ();
		mainCamera = playerCameraManager.getMainCamera ();
		headBobManager = mainCamera.GetComponent<headBob> ();
		weaponEffectsSource = GetComponent<AudioSource> ();
		IKWeaponManager = transform.parent.GetComponent<IKWeaponSystem> ();
		weaponAnimation = GetComponent<Animation> ();
		parable = GetComponentInChildren<launchTrayectory> ();

		gravitySystemManager = playerControllerGameObject.GetComponent<gravitySystem> ();

		playerCollider = playerControllerGameObject.GetComponent<Collider> ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<playerWeaponSystem> ());
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

	public int getWeaponNumberKey ()
	{
		return weaponSettings.numberKey;
	}

	public int getOriginalWeaponNumberKey ()
	{
		return originalNumberKey;
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

	public Texture getWeaponIcon ()
	{
		return weaponSettings.weaponIcon;
	}

	public Texture getWeaponInventorySlotIcon ()
	{
		return weaponSettings.weaponInventorySlotIcon;
	}

	public void setNumberKey (int newNumberKey)
	{
		if (originalNumberKey == -1) {
			originalNumberKey = weaponSettings.numberKey;
		}

		weaponSettings.numberKey = newNumberKey;
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
		if (silencerActive) {
			return;
		}

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
		
	//functions to change current weapon stats
	public void setMagazineSize (int magazineSize)
	{
		weaponSettings.ammoPerClip = magazineSize;
		weaponsManager.updateWeaponHUDInfo ();
		updateAmmoInfo ();
	}

	public playerWeaponsManager getPlayerWeaponsManger ()
	{
		return weaponsManager;
	}

	public void setOriginalMagazineSize ()
	{
		if (weaponSettings.clipSize > originalClipSize) {
			int extraBulletsOnMagazine = 0;
			extraBulletsOnMagazine = weaponSettings.clipSize - originalClipSize;
			if (extraBulletsOnMagazine > 0) {
				weaponSettings.remainAmmo += extraBulletsOnMagazine;
				weaponSettings.clipSize = originalClipSize;
				weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;
			}
		}

		setMagazineSize (originalClipSize);
	}

	public void setSilencerState (bool state)
	{
		silencerActive = state;
	}

	public void setAutomaticFireMode (bool state)
	{
		weaponSettings.automatic = state;
	}

	public void setOriginalAutomaticFireMode ()
	{
		setAutomaticFireMode (originalAutomaticMode);
	}

	public void setFireRate (float newFireRate)
	{
		weaponSettings.fireRate = newFireRate;
	}

	public void setOriginalFireRate ()
	{
		setFireRate (originalFireRate);
	}

	public void setProjectileDamage (float newDamage)
	{
		weaponSettings.projectileDamage = newDamage;
	}

	public void setOriginalProjectileDamage ()
	{
		setProjectileDamage (originalProjectileDamage);
	}

	public void setBurstModeState (bool state)
	{
		weaponSettings.useBurst = state;
		if (weaponSettings.useBurst) {
			weaponSettings.automatic = true;
		} else {
			weaponSettings.automatic = originalAutomaticMode;
		}
	}

	public void setOriginalBurstMode ()
	{
		setBurstModeState (orignalUseBurst);
	}

	public void setUsingSightState (bool state)
	{
		usingSight = state;
	}

	public void setProjectileDamageMultiplier (float multiplier)
	{
		weaponSettings.projectileDamage = originalProjectileDamage * multiplier;
	}

	public void setSpreadState (bool state)
	{
		weaponSettings.useProjectileSpread = state;
	}

	public void setOriginalSpreadState ()
	{
		setSpreadState (originalSpreadState);
	}

	public bool isUsingSight ()
	{
		return usingSight;
	}

	public void setKillOneShotState (bool state)
	{
		weaponSettings.killInOneShot = state;
	}

	public void setExplosiveAmmoState (bool state)
	{
		weaponSettings.isExplosive = state;
	}

	public void setOriginalExplosiveAmmoState ()
	{
		weaponSettings.isExplosive = originalIsExplosiveState;
	}

	public void setDamageOverTimeAmmoState (bool state)
	{
		weaponSettings.damageTargetOverTime = state;
	}

	public void setOriginalDamageOverTimeAmmoState ()
	{
		weaponSettings.damageTargetOverTime = originalDamageTargetOverTimeState;
	}

	public void setRemoveDamageOverTimeAmmoState (bool state)
	{
		weaponSettings.removeDamageOverTimeState = state;
		resetAmmoState (state);
	}

	public void setOriginalRemoveDamageOverTimeAmmoState ()
	{
		weaponSettings.removeDamageOverTimeState = originalRemoveDamageTargetOverTimeState;
		resetAmmoState (originalRemoveDamageTargetOverTimeState);
	}

	public void setSedateAmmoState (bool state)
	{
		weaponSettings.sedateCharacters = state;
		resetAmmoState (state);
	}

	public void setOriginalSedateAmmoState ()
	{
		weaponSettings.sedateCharacters = originalSedateCharactersState;
		resetAmmoState (originalSedateCharactersState);
	}

	public void resetAmmoState (bool state)
	{
		if (state) {
			setProjectileDamage (0);
			setExplosiveAmmoState (false);
			setDamageOverTimeAmmoState (false);
		} else {
			setOriginalProjectileDamage ();
			setOriginalExplosiveAmmoState ();
			setOriginalDamageOverTimeAmmoState ();
		}
	}

	public void setPushCharacterState (bool state)
	{
		weaponSettings.pushCharacter = state;
	}

	public void setOriginalPushCharacterState ()
	{
		weaponSettings.pushCharacter = originalPushCharacterState;
	}

	public bool useCustomReticleEnabled ()
	{
		return weaponSettings.useCustomReticle;
	}

	public Texture getRegularCustomReticle ()
	{
		return weaponSettings.regularCustomReticle;
	}

	public bool useAimCustomReticleEnabled ()
	{
		return weaponSettings.useAimCustomReticle;
	}

	public Texture getAimCustomReticle ()
	{
		return weaponSettings.aimCustomReticle;
	}

	public void setNewWeaponProjectile (GameObject newProjectile)
	{
		weaponProjectile = newProjectile;
	}

	public void setOriginalWeaponProjectile ()
	{
		weaponProjectile = originalWeaponProjectile;
	}

	public void setProjectileWithAbilityState (bool newValue)
	{
		weaponSettings.projectileWithAbility = newValue;
	}

	public void setOriginalProjectileWithAbilityValue ()
	{
		weaponSettings.projectileWithAbility = originalProjectileWithAbilityState;
	}

	public void setUsingDualWeaponState (bool state)
	{
		usingDualWeapon = state;

		if (!usingDualWeapon) {
			if (useEventOnSetSingleWeapon) {
				eventOnSetSingleWeapon.Invoke ();
			}
		}
	}

	public void setUsingRightHandDualWeaponState (bool state)
	{
		usingRightHandDualWeapon = state;

		if (useEventOnSetDualWeapon) {
			if (usingRightHandDualWeapon) {
				eventOnSetRightWeapon.Invoke ();
			} else {
				eventOnSetLeftWeapon.Invoke ();
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
		public int numberKey;

		public Texture weaponIcon;

		public Texture weaponInventorySlotIcon;

		public Texture weaponIConHUD;
		public bool showWeaponNameInHUD = true;
		public bool showWeaponIconInHUD = true;
		public bool showWeaponAmmoSliderInHUD = true;
		public bool showWeaponAmmoTextInHUD = true;

		public bool useCustomReticle;
		public Texture regularCustomReticle;
		public bool useAimCustomReticle;
		public Texture aimCustomReticle;

		public bool useRayCastShoot;

		public bool useRaycastShootDelay;
		public float raycastShootDelay;
		public bool getDelayWithDistance;
		public float delayWithDistanceSpeed;
		public float maxDelayWithDistance;

		public bool useFakeProjectileTrails;

		public bool fireWeaponForward;

		public bool useRaycastCheckingOnRigidbody;

		public bool weaponUsesAmmo = true;
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

		public bool useSecondaryAction;
		public UnityEvent secondaryAction;

		public bool useSecondaryActionOnDownPress;
		public UnityEvent secondaryActionOnDownPress;
		public bool useSecondaryActionOnUpPress;
		public UnityEvent secondaryActionOnUpPress;
		public bool useSecondaryActionOnHoldPress;
		public UnityEvent secondaryActionOnHoldPress;

		public bool automatic;

		public bool useBurst;
		public int burstAmount;

		public float fireRate;

		public float reloadTimeThirdPerson = 1;
		public float reloadTimeFirstPerson = 1;

		public bool useReloadDelayThirdPerson;
		public float reloadDelayThirdPerson;

		public bool useReloadDelayFirstPerson;
		public float reloadDelayFirstPerson;

		public bool usePreReloadDelayThirdPerson;
		public float preReloadDelayThirdPerson;

		public bool usePreReloadDelayFirstPerson;
		public float preReloadDelayFirstPerson;

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
		public bool applyExplosionForceToVehicles = true;
		public float explosionForceToVehiclesMultiplier = 0.2f;

		public List<Transform> projectilePosition = new List<Transform> ();

		public GameObject shell;
		public List<Transform> shellPosition = new List<Transform> ();
		public float shellEjectionForce = 100;
		public List<AudioClip> shellDropSoundList = new List<AudioClip> ();
		public bool useShellDelay;
		public float shellDelayThirdPerson;
		public float shellDelayFirsPerson;
		public bool createShellsOnReload;

		public GameObject weaponMesh;
		public Transform weaponParent;

		public bool useFireAnimation = true;
		public string animation;
		public float animationSpeed = 1;

		public bool useReloadAnimation;
		public string reloadAnimationName;
		public float reloadAnimationSpeed;

		public bool playAnimationBackward = true;
		public GameObject scorch;
		public float scorchRayCastDistance;
	
		public bool useCanvasHUD = true;
		public Text clipSizeText;
		public Text remainAmmoText;
		public GameObject HUD;
		public GameObject ammoInfoHUD;

		public bool useHUD;
		public bool changeHUDPosition;
		public bool disableHUDInFirstPersonAim;
		public Transform HUDTransformInThirdPerson;
		public Transform HUDTransformInFirstPerson;

		public bool useHUDDualWeaponThirdPerson;
		public bool useHUDDualWeaponFirstPerson;

		public bool changeHUDPositionDualWeapon;
		public Transform HUDRightHandTransformThirdPerson;
		public Transform HUDLeftHandTransformThirdPerson;
		public Transform HUDRightHandTransformFirstPerson;
		public Transform HUDLeftHandTransformFirstPerson;

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
		public float impactForceToVehiclesMultiplier = 1;
	
		public AudioClip reloadSoundEffect;
		public AudioClip cockSound;
		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;
		public AudioClip silencerShootEffect;

		public bool useSoundsPool;
		public int maxSoundsPoolAmount;
		public GameObject weaponEffectSourcePrefab;
		public Transform weaponEffectSourceParent;

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

		public bool shakeUpperBodyShootingDualWeapons;
		public float dualWeaponShakeAmount;
		public float dualWeaponShakeSpeed;

		public bool useSoundOnDrawKeepWeapon;
		public AudioClip drawWeaponSound;
		public AudioClip keepWeaponSound;

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