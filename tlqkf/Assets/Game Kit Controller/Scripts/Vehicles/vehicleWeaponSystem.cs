#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[SerializeField]
public class vehicleWeaponSystem : MonoBehaviour
{
	public bool weaponsEnabled = true;

	public bool weaponsActivate;
	public AudioSource weaponsEffectsSource;
	public AudioClip outOfAmmo;

	public LayerMask layer;
	public LayerMask targetForScorchLayer;
	public float minimumX = 25;
	public float maximumX = 315;
	public float minimumY = 360;
	public float maximumY = 360;
	public bool hasBaseRotation = true;
	public GameObject baseX;
	public GameObject baseY;
	public Transform weaponLookDirection;
	public int weaponsSlotsAmount;
	public Vector2 touchZoneSize;
	public float minSwipeDist = 20;
	public bool showGizmo;
	public Color gizmoColor;
	public bool reloading;
	public bool aimingCorrectly;
	public List<vehicleWeapons> weapons = new List<vehicleWeapons> ();
	public GameObject vehicle;
	public vehicleWeapons currentWeapon;

	public Transform noInputWeaponDirection;
	public bool canRotateWeaponsWithNoCameraInput;
	public float noCameraInputWeaponRotationSpeed;
	public float weaponCursorMovementSpeed;
	public float weaponCursorHorizontalLimit;
	public float weaponCursorVerticalLimit;
	public LayerMask weaponCursorRaycastLayer;

	public bool useWeaponCursorScreenLimit;

	public Vector2 currentLockedCameraCursorSize;

	bool weaponCursorDisabled;

	float horizontaLimit;
	float verticalLimit;

	public float baseXRotationSpeed = 10;
	public float baseYRotationSpeed = 10;

	public string[] impactDecalList;
	decalManager impactDecalManager;

	List<GameObject> locatedEnemies = new List<GameObject> ();

	playerScreenObjectivesSystem playerScreenObjectivesManager;

	List<GameObject> shells = new List<GameObject> ();
	float rotationY = 0;
	float rotationX = 0;
	float lastShoot = 0;

	float lastTimeFired;

	float destroyShellsTimer = 0;
	public int choosedWeapon;
	public int weaponIndexToStart;

	public bool aimingHomingProjectile;
	bool usingLaser;
	bool launchingProjectile;
	bool objectiveFound;
	bool touchPlatform;
	bool touchEnabled;
	bool touching;
	public bool shootingPreviously;
	GameObject currentProjectile;
	Transform swipeCenterPosition;
	GameObject closestEnemy;

	Transform currentWeaponDirection;
	Transform vehicleCameraTransform;

	Vector3 baseXTargetRotation;
	Vector3 baseYTargetRotation;

	Quaternion noInputWeaponDirectionTargetRotation;
	Vector2 axisValues;
	float horizontalMouse;
	float verticalMouse;
	Vector2 lookAngle;

	public vehicleCameraController vehicleCameraManager;
	public Rigidbody mainRigidbody;
	public inputActionManager actionManager;
	public vehicleHUDManager hudManager;
	public launchTrayectory parable;
	public vehicleGravityControl gravityControlManager;

	RaycastHit hit;
	Ray ray;

	GameObject parableGameObject;
	Touch currentTouch;
	Rect touchZoneRect;
	Vector3 touchZone;

	Vector3 swipeStartPos;
	Vector3 aimedZone;

	GameObject currentDriver;
	Transform mainCameraTransform;
	Vector3 currentMainCameraPosition;

	Camera mainCamera;
	playerController playerControllerManager;
	playerCamera playerCameraManager;
	vehicleHUDInfo vehicleHUDInfoManager;

	bool weaponsPaused;

	Coroutine muzzleFlahsCoroutine;
	bool currentCameraUseRotationInput;

	RectTransform weaponCursor;
	Vector3 pointTargetPosition;
	Vector3 projectileDirectionToLook;
	RawImage weaponCursorImage;

	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;
	Transform currentTargetToAim;

	float posX, posY;
	Ray newRay;
	Vector2 newCameraPosition;
	Vector3 targetDirection;
	Quaternion targetRotation;

	bool playerCameraIsLocked;
	bool usingCameraWithNoInput;
	Vector3 homingProjectileRayPosition;
	Vector3 autoShootRayPosition;

	bool pointTargetPositionFound;
	bool pointTargetPositionSurfaceFound;
	GameObject currentPointTargetSurfaceFound;
	GameObject previousPointTargetSurfaceFound;

	vehicleHUDInfo.vehicleHUDElements currentHUDElements;
	bool usingCustomReticle;

	Vector3 screenPoint;
	bool targetOnScreen;

	bool usingScreenSpaceCamera;

	void Start ()
	{
		if (!weaponsEnabled) {
			return;
		}

		//get every the ammo per clip of every weapon according to their initial clip size
		for (int i = 0; i < weapons.Count; i++) {
			weapons [i].ammoPerClip = weapons [i].clipSize;
			if (weapons [i].weapon) {
				weapons [i].weaponAnimation = weapons [i].weapon.GetComponent<Animation> ();
			}
			if (weapons [i].startWithEmptyClip) {
				weapons [i].clipSize = 0;
			}
			weapons [i].auxRemainAmmo = weapons [i].remainAmmo;
		}
			
		if (parable) {
			parableGameObject = parable.gameObject;
		}

		//check the current type of platform
		touchPlatform = touchJoystick.checkTouchPlatform ();

		//set the touch zone in the right upper corner of the screen to swipe between weapons in vehicles when the platform is a touch device
		setHudZone ();

		if (weaponIndexToStart >= weapons.Count) {
			print ("WARNING: the weapon index configured is higher than the current amount of weapons in the vehicle, check that the index is lower than the current amount of weapons");
		}

		choosedWeapon = weaponIndexToStart;
		currentWeapon = weapons [choosedWeapon];
		if (vehicle == null) {
			vehicle = gameObject;
		}

		if (weaponsSlotsAmount < 10) {
			weaponsSlotsAmount++;
		}
	}

	void Update ()
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			if (hasBaseRotation) {

				playerCameraIsLocked = !playerCameraManager.isCameraTypeFree ();

				if (currentCameraUseRotationInput && !playerCameraIsLocked) {
					currentWeaponDirection = vehicleCameraTransform;
					currentMainCameraPosition = mainCameraTransform.position;
					usingCameraWithNoInput = false;
				} else {
					usingCameraWithNoInput = true;

					currentWeaponDirection = noInputWeaponDirection;

					if (playerCameraIsLocked) {
						vehicleCameraTransform = playerCameraManager.getCurrentLockedCameraTransform ();
						mainCameraTransform = vehicleCameraTransform;
						currentCameraUseRotationInput = false;
					}

					if (currentWeapon.fireWeaponForward) {
						currentWeaponDirection = vehicleCameraTransform;
						currentMainCameraPosition = mainCameraTransform.position;
						weaponCursor.anchoredPosition = Vector2.zero;
					} else if (canRotateWeaponsWithNoCameraInput && !actionManager.input.gameCurrentlyPaused) {
						axisValues = actionManager.getPlayerMovementAxis ("mouse");
						horizontalMouse = axisValues.x;
						verticalMouse = axisValues.y;

						weaponCursor.Translate (new Vector3 (horizontalMouse, verticalMouse, 0) * weaponCursorMovementSpeed);
						newCameraPosition = weaponCursor.anchoredPosition;

						if (useWeaponCursorScreenLimit) {
							posX = Mathf.Clamp (newCameraPosition.x, -weaponCursorHorizontalLimit, weaponCursorHorizontalLimit);
							posY = Mathf.Clamp (newCameraPosition.y, -weaponCursorVerticalLimit, weaponCursorVerticalLimit);
							weaponCursor.anchoredPosition = new Vector2 (posX, posY);
						} else {
							newCameraPosition = weaponCursor.position;
							newCameraPosition.x = Mathf.Clamp (newCameraPosition.x, currentLockedCameraCursorSize.x, horizontaLimit);
							newCameraPosition.y = Mathf.Clamp (newCameraPosition.y, currentLockedCameraCursorSize.y, verticalLimit);
							weaponCursor.position = new Vector3 (newCameraPosition.x, newCameraPosition.y, 0);
						}

						newRay = mainCamera.ScreenPointToRay (weaponCursor.position);

						if (Physics.Raycast (newRay, out hit, Mathf.Infinity, weaponCursorRaycastLayer)) {
							pointTargetPositionFound = false;

							//check if the object found is the current vehicle, to avoid to aim at it and aim in the proper direction even if this vehicle is found
							if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
								if (currentWeapon.useRaycastAllToCheckSurfaceFound && !currentWeapon.fireWeaponForward) {

									RaycastHit[] hits = Physics.RaycastAll (newRay, currentWeapon.maxDistanceToRaycastAll, layer);
									System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

									pointTargetPositionSurfaceFound = false;
									foreach (RaycastHit rh in hits) {
										if (!pointTargetPositionSurfaceFound && !hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider)) {
											pointTargetPosition = rh.point;
											currentDetectedSurface = rh.collider.gameObject;
											pointTargetPositionSurfaceFound = true;
										}
									}
								} else {
									pointTargetPositionFound = true;
								}
							} else {
								pointTargetPositionFound = true;

							}

							if (pointTargetPositionFound) {
								pointTargetPosition = hit.point;
								currentDetectedSurface = hit.collider.gameObject;
							}
						} else {
							pointTargetPosition = new Vector3 (newRay.origin.x, currentWeaponDirection.position.y, newRay.origin.z);
							currentDetectedSurface = null;
						}

						if (currentTargetDetected != currentDetectedSurface) {
							currentTargetDetected = currentDetectedSurface;
							if (currentTargetDetected) {
								currentTargetToAim = applyDamage.getPlaceToShoot (currentTargetDetected);
								if (currentTargetToAim) {
									print (currentTargetToAim.name);
								}
							} else {
								currentTargetToAim = null;
							}
						}

						if (currentTargetToAim) {
							pointTargetPosition = currentTargetToAim.position;
						}

						targetDirection = pointTargetPosition - currentWeaponDirection.position;
						targetRotation = Quaternion.LookRotation (targetDirection);

						lookAngle.x = targetRotation.eulerAngles.y;
						lookAngle.y = targetRotation.eulerAngles.x;

						noInputWeaponDirectionTargetRotation = Quaternion.Euler (lookAngle.y, lookAngle.x, 0);
					
						noInputWeaponDirection.rotation = Quaternion.Slerp (noInputWeaponDirection.rotation, noInputWeaponDirectionTargetRotation, 
							noCameraInputWeaponRotationSpeed * Time.deltaTime);							
					
						currentMainCameraPosition = pointTargetPosition;
					}
				}

				//rotate the weapon to look in the camera direction
				Quaternion cameraDirection = Quaternion.LookRotation (currentWeaponDirection.forward);
				weaponLookDirection.rotation = cameraDirection;
				float angleX = weaponLookDirection.localEulerAngles.x;
				//clamp the angle of the weapon, to avoid a rotation higher that the camera

				//in X axis
				if (angleX >= 0 && angleX <= minimumX) {
					rotationX = angleX;
					aimingCorrectly = true;
				} else if (angleX >= maximumX && angleX <= 360) {
					rotationX = angleX;
					aimingCorrectly = true;
				} else {
					aimingCorrectly = false;
				}

				//in Y axis
				float angleY = weaponLookDirection.localEulerAngles.y;
				if (angleY >= 0 && angleY <= minimumY) {
					rotationY = angleY;
				} else if (angleY >= maximumY && angleY <= 360) {
					rotationY = angleY;
				}

				baseXTargetRotation = new Vector3 (0, rotationY, 0);
				baseYTargetRotation = new Vector3 (rotationX, 0, 0);

				//rotate every transform of the weapon base
				baseY.transform.localRotation = Quaternion.Lerp (baseY.transform.localRotation, Quaternion.Euler (baseXTargetRotation), Time.deltaTime * baseYRotationSpeed);
				baseX.transform.localRotation = Quaternion.Lerp (baseX.transform.localRotation, Quaternion.Euler (baseYTargetRotation), Time.deltaTime * baseXRotationSpeed);
			}

			//check if a key number has been pressed, to change the current weapon for the key pressed, if there is a weapon using that key
			for (int i = 0; i < weaponsSlotsAmount; i++) {
				if (Input.GetKeyDown ("" + (i))) {
					for (int k = 0; k < weapons.Count; k++) {
						if (weapons [k].numberKey == (i) && choosedWeapon != k) {
							if (weapons [k].enabled) {
								choosedWeapon = k;
								weaponChanged ();
							}
						}
					}
				}
			}

			//if the touch controls are enabled, activate the swipe option
			if (touchEnabled) {
				//select the weapon by swiping the finger in the right corner of the screen, above the weapon info
				int touchCount = Input.touchCount;
				if (!touchPlatform) {
					touchCount++;
				}
				for (int i = 0; i < touchCount; i++) {
					if (!touchPlatform) {
						currentTouch = touchJoystick.convertMouseIntoFinger ();
					} else {
						currentTouch = Input.GetTouch (i);
					}
					//get the start position of the swipe
					if (currentTouch.phase == TouchPhase.Began) {
						if (touchZoneRect.Contains (currentTouch.position) && !touching) {
							swipeStartPos = currentTouch.position;
							touching = true;
						}
					}
					//and the final position, and get the direction, to change to the previous or the next power
					if (currentTouch.phase == TouchPhase.Ended && touching) {
						float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;
						if (swipeDistHorizontal > minSwipeDist) {
							float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);
							if (swipeValue > 0) {
								//right swipe
								choosePreviousWeapon ();
							} else if (swipeValue < 0) {
								//left swipe
								chooseNextWeapon ();
							}
						}
						touching = false;
					}
				}
			} 

			setVehicleWeaponCameraDirection ();

			checkAutoShootOnTag ();

			//if the current wepapon is the homing missiles
			if (aimingHomingProjectile) {
				//check if the amount of locoted enemies is equal or lower that the number of remaining projectiles
				if (locatedEnemies.Count < currentWeapon.projectilePosition.Count) {
					//uses a ray to detect enemies, to locked them

					homingProjectileRayPosition = mainCameraTransform.position;
					if (usingCameraWithNoInput) {
						homingProjectileRayPosition = weaponLookDirection.position;
					}

					if (Physics.Raycast (homingProjectileRayPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer)) {
						Debug.DrawLine (homingProjectileRayPosition, hit.point, Color.yellow, 2);

						GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

						if (target != null) {
							
							if (target != vehicle) {
								if (currentWeapon.tagToLocate.Contains (target.tag)) {
									GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);
									if (!locatedEnemies.Contains (placeToShoot)) {
										//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
										locatedEnemies.Add (placeToShoot);

										playerScreenObjectivesManager.addElementToPlayerList (placeToShoot, false, false, 0, true, false, 
											false, currentWeapon.locatedEnemyIconName, false, Color.white, true, -1, 0, false);
									}
								}
							}
						}
					}
				} 
			}

			//if the current weapon is the laser
			if (usingLaser) {
				if (currentWeapon.clipSize > 0) {
					//play the animation
					if (currentWeapon.weaponAnimation) {
						currentWeapon.weaponAnimation.Play (currentWeapon.animation);
					}

					//reduce the amount of ammo
					useAmmo ();

					//play the sound 
					if (Time.time > lastShoot + currentWeapon.fireRate) {
						playWeaponSoundEffect (true);
						lastShoot = Time.time;

						checkIfEnableWeaponCursor ();
					}
						
					checkShotShake ();
				} else {
					if (currentWeapon.autoReloadWhenClipEmpty) {
						autoReload ();
					}
				}
			}

			//if the current weapon launched the projectile
			if (launchingProjectile) {
				//if the launcher animation is not being played
				if (currentWeapon.weapon && currentWeapon.animation != "") {
					if (!currentWeapon.weaponAnimation.IsPlaying (currentWeapon.animation)) {
						//reverse it and play it again
						if (currentWeapon.weaponAnimation [currentWeapon.animation].speed == 1) {
							currentWeapon.weaponAnimation [currentWeapon.animation].speed = -1; 
							currentWeapon.weaponAnimation [currentWeapon.animation].time = currentWeapon.weaponAnimation [currentWeapon.animation].length;
							currentWeapon.weaponAnimation.Play (currentWeapon.animation);
							currentProjectile.transform.SetParent (null);

							launchCurrentProjectile ();
							return;
						}

						//the launcher has thrown a projectile and the animation is over
						if (currentWeapon.weaponAnimation [currentWeapon.animation].speed == -1) {
							launchingProjectile = false;
							if (currentWeapon.projectileModel) {
								currentWeapon.projectileModel.SetActive (true);
							}
						}
					}
				} else {
					launchCurrentProjectile ();
					launchingProjectile = false;
				}
			}

			checkIfDisableWeaponCursor ();
		}

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
	}

	public void checkIfEnableWeaponCursor ()
	{
		if (currentWeapon.disableWeaponCursorWhileNotShooting) {
			if (weaponCursor && weaponCursorDisabled) {
				weaponCursor.gameObject.SetActive (true);
				weaponCursorDisabled = false;
			}
			lastTimeFired = Time.time;
		}
	}

	public void checkIfDisableWeaponCursor ()
	{
		if (currentWeapon.disableWeaponCursorWhileNotShooting) {
			if (weaponCursor && !weaponCursorDisabled) {
				if (Time.time > currentWeapon.delayToDisableWeaponCursor + lastTimeFired) {
					weaponCursor.gameObject.SetActive (false);
					weaponCursorDisabled = true;
				}
			}
		} else {
			if (weaponCursor && weaponCursorDisabled) {
				weaponCursor.gameObject.SetActive (true);
				weaponCursorDisabled = false;
			}
		}
	}

	public void launchCurrentProjectile ()
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		Rigidbody currentProjectileRigidbody = currentProjectile.GetComponent<Rigidbody> ();
			
		currentProjectileRigidbody.isKinematic = false;
		if (currentWeapon.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone);
			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}
			currentProjectileRigidbody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			currentProjectileRigidbody.AddForce (currentWeapon.parableDirectionTransform.forward * currentWeapon.projectileSpeed, ForceMode.Impulse);
		}
	}

	//if the homing missile weapon has been fired or change when enemies were locked, remove the icons from the screen
	void removeLocatedEnemiesIcons ()
	{
		for (int i = 0; i < locatedEnemies.Count; i++) {
			playerScreenObjectivesManager.removeElementFromListByPlayer (locatedEnemies [i]);
		}
		locatedEnemies.Clear ();
	}

	//play the fire sound or the empty clip sound
	void playWeaponSoundEffect (bool hasAmmo)
	{
		if (hasAmmo) {
			if (currentWeapon.shootSoundEffect) {
				weaponsEffectsSource.clip = currentWeapon.shootSoundEffect;
				weaponsEffectsSource.Play ();
				//weaponsEffectsSource.PlayOneShot (weapons [choosedWeapon].soundEffect);
			}
		} else {
			if (Time.time > lastShoot + currentWeapon.fireRate) {
				weaponsEffectsSource.PlayOneShot (outOfAmmo);
				lastShoot = Time.time;

				checkIfEnableWeaponCursor ();
			}
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

	//fire the current weapon
	public void shootWeapon (bool shootAtKeyDown)
	{
		if (!shootAtKeyDown && !aimingHomingProjectile) {
			if (shootingPreviously) {
				shootingPreviously = false;
			}
			return;
		}

		//if the weapon system is active and the clip size higher than 0
		if (!weaponsActivate || weaponsPaused) {
			return;
		}

		if (currentWeapon.clipSize > 0) {

			//else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + currentWeapon.fireRate) {
				shootingPreviously = true;

				//if the current weapon is the homing missile, set to true and return
				//If the current projectile is homing type, check when the shoot button is pressed and release
				if ((currentWeapon.isHommingProjectile && shootAtKeyDown)) {
					aimingHomingProjectile = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHomingProjectile);
					return;
				}

				if ((currentWeapon.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!currentWeapon.isHommingProjectile && !shootAtKeyDown)) {
					aimingHomingProjectile = false;
					shootingPreviously = false;
					checkShotShake ();
					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHomingProjectile);
					return;
				}


				checkShotShake ();
				//if the current weapon is the laser, enable it and return
				if (currentWeapon.isLaser) {
					changeWeaponLaserState (true);
					return;
				}

				//play the fire sound
				playWeaponSoundEffect (true);

				//create the muzzle flash
				createMuzzleFlash ();

				//play the fire animation
				if (currentWeapon.weapon && currentWeapon.animation != "") {
					if (currentWeapon.launchProjectile) {
						currentWeapon.weaponAnimation [currentWeapon.animation].speed = 1; 
						//disable the projectile model in the weapon
						if (currentWeapon.projectileModel) {
							currentWeapon.projectileModel.SetActive (false);
						}
					}
					currentWeapon.weaponAnimation.Play (currentWeapon.animation);

				}
			
				//every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < currentWeapon.projectilePosition.Count; j++) {
					for (int l = 0; l < currentWeapon.projectilesPerShoot; l++) {
						//create the projectile
						currentProjectile = (GameObject)Instantiate (currentWeapon.projectileToShoot, currentWeapon.projectilePosition [j].position, 
							currentWeapon.projectilePosition [j].rotation);

						if (!currentWeapon.launchProjectile) {
							//set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (Physics.Raycast (currentMainCameraPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer) && aimingCorrectly &&
							    !currentWeapon.fireWeaponForward) {
								if (!hit.collider.isTrigger) {
									currentProjectile.transform.LookAt (hit.point);
								}
							}
						}

						//if the current weapon launches projectiles instead of shooting
						else {
							//if the projectile is not being launched, then 
							if (!launchingProjectile) {
								if (currentWeapon.projectileModel) {
									currentProjectile.transform.SetParent (currentWeapon.projectilePosition [j].transform);
									currentProjectile.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
								}
								currentProjectile.GetComponent<Rigidbody> ().isKinematic = true;

								//if the vehicle has a gravity control component, and the current gravity is not the regular one, add an artifical gravity component to the projectile
								//like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle
								if (gravityControlManager) {
									if (gravityControlManager.currentNormal != Vector3.up) {
										currentProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-gravityControlManager.currentNormal);
									}
								}

								explosiveBarrel currentExplosiveBarrel = currentProjectile.GetComponent<explosiveBarrel> ();
								if (currentExplosiveBarrel) {
									currentExplosiveBarrel.canExplodeState (true);
									currentExplosiveBarrel.setExplosiveBarrelOwner (vehicle);
									currentExplosiveBarrel.setExplosionValues (currentWeapon.explosionForce, currentWeapon.explosionRadius);
								}

								if (currentWeapon.useParableSpeed) {
									//get the ray hit point where the projectile will fall
									if (Physics.Raycast (currentMainCameraPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer)) {
										aimedZone = hit.point;
										objectiveFound = true;
									} else {
										objectiveFound = false;
									}
								}
								launchingProjectile = true;
							}
						}
							
						//add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;
						if (currentWeapon.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							currentProjectile.transform.Rotate (spreadAmount);
						}

						//set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						projectileSystem currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

						if (currentProjectileSystem) {
							currentProjectileSystem.setProjectileInfo (setProjectileInfo ());
						}

						if (currentWeapon.isSeeker) {
							setSeekerProjectileInfo (currentProjectile, currentWeapon.projectilePosition [j]);
						}

						//if the weapon shoots setting directly the projectile in the hit point, place the current projectile in the hit point position
						if (currentWeapon.useRayCastShoot) {
							Vector3 forwardDirection = projectileDirectionToLook;
							Vector3 forwardPositon = currentMainCameraPosition;
							if (!aimingCorrectly || currentWeapon.fireWeaponForward) {
								forwardDirection = currentWeapon.weapon.transform.forward;
								forwardPositon = currentWeapon.projectilePosition [j].position;
							}

							if (!currentCameraUseRotationInput && !currentWeapon.fireWeaponForward) {
								forwardPositon = weaponLookDirection.position;
								forwardDirection = projectileDirectionToLook;
							}

							if (!currentCameraUseRotationInput && !hasBaseRotation) {
								forwardDirection = currentWeapon.weapon.transform.forward;
							}

							if (spreadAmount.magnitude != 0) {
								forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
							}

							Debug.DrawLine (forwardPositon, currentMainCameraPosition, Color.white, 3);

							bool destroyProjectile = false;
							//check what element the projectile will impact, if the vehicle is found, destroy the projectile to avoid shooting at your own vehicle
							if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layer)) {
								Debug.DrawLine (forwardPositon, hit.point, Color.red, 3);
								//print (hit.collider.name + " " + hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider));

								if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
									if (currentWeapon.useRaycastAllToCheckSurfaceFound && !currentWeapon.fireWeaponForward) {
										if (currentCameraUseRotationInput) {
											if (aimingCorrectly) {
												ray.direction = projectileDirectionToLook;
												ray.origin = currentMainCameraPosition;
											} else {
												ray.origin = currentWeapon.projectilePosition [j].position;
												ray.direction = currentWeapon.weapon.transform.forward;
											}
										} else {
											ray.origin = weaponLookDirection.position;
											ray.direction = projectileDirectionToLook;
										}

										if (!hasBaseRotation && !currentCameraUseRotationInput) {
											ray.direction = mainCameraTransform.TransformDirection (Vector3.forward);
										}

										if (spreadAmount.magnitude != 0) {
											ray.direction = Quaternion.Euler (spreadAmount) * ray.direction;
										}

										RaycastHit[] hits = Physics.RaycastAll (ray, currentWeapon.maxDistanceToRaycastAll, layer);
										System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

										bool surfaceFound = false;
										foreach (RaycastHit rh in hits) {
											if (!surfaceFound && !hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider)) {
												Debug.DrawLine (ray.origin, rh.point, Color.yellow, 3);
												//print (rh.collider.name + " " + hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider));
												currentProjectileSystem.rayCastShoot (rh.collider, rh.point + rh.normal * 0.02f, projectileDirectionToLook);
												surfaceFound = true;
											}
										}

										if (!surfaceFound) {
											destroyProjectile = true;
										}
									} else {
										if (Physics.Raycast (currentWeapon.projectilePosition [j].position, forwardDirection, out hit, Mathf.Infinity, layer)) {
											if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
												destroyProjectile = true;
											} else {
												currentProjectileSystem.rayCastShoot (hit.collider, hit.point + hit.normal * 0.02f, forwardDirection);
											}
										} else {
											destroyProjectile = true;
										}
									}
								} else {
									currentProjectileSystem.rayCastShoot (hit.collider, hit.point + hit.normal * 0.02f, forwardDirection);
								}
							} else {
								destroyProjectile = true;
							}

							if (destroyProjectile) {
								currentProjectileSystem.destroyProjectile ();
							}
							//Debug.DrawLine (forwardPositon, hit.point, Color.red, 2);
						}

						if (aimingHomingProjectile) {
							//check that the located enemies are higher that 0
							if (locatedEnemies.Count > 0) {
								//shoot the missiles
								if (j < locatedEnemies.Count) {
									currentProjectileSystem.setProjectileInfo (setProjectileInfo ());
									currentProjectileSystem.setEnemy (locatedEnemies [j]);
								} else {
									currentProjectileSystem.destroyProjectile ();
								}
							}
						}
					}
					useAmmo ();
					lastShoot = Time.time;

					checkIfEnableWeaponCursor ();

					if (currentWeapon.applyForceAtShoot) {
						Transform currentVehicleCameraTransform = vehicleCameraManager.getCurrentCameraTransform ();
						Vector3 forceDirection = (currentVehicleCameraTransform.right * currentWeapon.forceDirection.x +
						                         currentVehicleCameraTransform.up * currentWeapon.forceDirection.y +
						                         currentVehicleCameraTransform.forward * currentWeapon.forceDirection.z) * currentWeapon.forceAmount;
						mainRigidbody.AddForce (mainRigidbody.mass * forceDirection, ForceMode.Impulse);
					}
				}

				//if the current weapon drops shells, create them
				createShells ();

				if (currentWeapon.isHommingProjectile && !shootAtKeyDown && aimingHomingProjectile) {
					//if the button to shoot is released, shoot a homing projectile for every located enemy
					//check that the located enemies are higher that 0
					if (locatedEnemies.Count > 0) {
						//remove the icons in the screen
						removeLocatedEnemiesIcons ();
					}
					aimingHomingProjectile = false;
				}
			}
		}
		//else, the clip in the weapon is over, so check if there is remaining ammo
		else {
			if (currentWeapon.autoReloadWhenClipEmpty) {
				autoReload ();
			}
		}
	}

	public void setVehicleWeaponCameraDirection ()
	{
		projectileDirectionToLook = mainCameraTransform.TransformDirection (Vector3.forward);
		if (!currentCameraUseRotationInput) {
			projectileDirectionToLook = currentMainCameraPosition - weaponLookDirection.position;
			//Debug.DrawLine (weaponLookDirection.position, currentMainCameraPosition, Color.yellow, 2);
		}
	}

	public void autoReload ()
	{
		//disable the laser
		changeWeaponLaserState (false);

		//if the weapon is not being reloaded, do it
		if (!reloading) {
			StartCoroutine (waitToReload (currentWeapon.reloadTime));
		}
		//checkRemainAmmo ();
	}

	public void changeWeaponLaserState (bool state)
	{
		if (currentWeapon.weapon) {
			vehicleLaser currentVehicleLaser = currentWeapon.weapon.GetComponentInChildren<vehicleLaser> ();
			if (currentVehicleLaser) {
				currentVehicleLaser.changeLaserState (state);
				usingLaser = state;
			}
		}
	}

	public void createShells ()
	{
		if (currentWeapon.ejectShellOnShot) {
			for (int j = 0; j < currentWeapon.shellPosition.Count; j++) {
				if (currentWeapon.shell) {
					GameObject shellClone = (GameObject)Instantiate (currentWeapon.shell, currentWeapon.shellPosition [j].position, currentWeapon.shellPosition [j].rotation);
					shellClone.GetComponent<Rigidbody> ().AddForce (currentWeapon.shellPosition [j].right * currentWeapon.shellEjectionForce);
					shells.Add (shellClone);
					if (currentWeapon.shellDropSoundList.Count > 0) {
						shellClone.GetComponent<AudioSource> ().clip = currentWeapon.shellDropSoundList [Random.Range (0, currentWeapon.shellDropSoundList.Count - 1)];
					}
					if (shells.Count > 15) {
						GameObject shellToRemove = shells [0];
						shells.RemoveAt (0);
						Destroy (shellToRemove);
					}
				}
			}
			destroyShellsTimer = 0;
		}
	}

	public void testWeaponShake (int weaponIndex)
	{
		vehicleWeapons weaponToTest = weapons [weaponIndex];
		if (weaponToTest.shootShakeInfo.useDamageShake) {
			if (weaponToTest.shootShakeInfo.sameValueBothViews) {
				vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.thirdPersonDamageShake);
			} else {
				if (vehicleCameraManager.currentState.firstPersonCamera) {
					if (weaponToTest.shootShakeInfo.useDamageShakeInFirstPerson) {
						vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.firstPersonDamageShake);
					}
				} else {
					if (weaponToTest.shootShakeInfo.useDamageShakeInThirdPerson) {
						vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.thirdPersonDamageShake);
					}
				}
			}
		}
	}

	public void checkShotShake ()
	{
		if (currentWeapon.shootShakeInfo.useDamageShake) {
			if (currentWeapon.shootShakeInfo.sameValueBothViews) {
				vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.thirdPersonDamageShake);
			} else {
				if (vehicleCameraManager.currentState.firstPersonCamera) {
					if (currentWeapon.shootShakeInfo.useDamageShakeInFirstPerson) {
						vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.firstPersonDamageShake);
					}
				} else {
					if (currentWeapon.shootShakeInfo.useDamageShakeInThirdPerson) {
						vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.thirdPersonDamageShake);
					}
				}
			}
		}
	}

	//create the muzzle flash particles if the weapon has it
	void createMuzzleFlash ()
	{
		if (currentWeapon.muzzleParticles) {
			for (int j = 0; j < currentWeapon.projectilePosition.Count; j++) {
				GameObject muzzleParticlesClone = (GameObject)Instantiate (currentWeapon.muzzleParticles, currentWeapon.projectilePosition [j].position,
					                                  currentWeapon.projectilePosition [j].rotation);
				Destroy (muzzleParticlesClone, 1);	
				muzzleParticlesClone.transform.SetParent (currentWeapon.projectilePosition [j]);
				currentWeapon.muzzleParticles.GetComponent<ParticleSystem> ().Play ();
			}
		}
	}

	//decrease the amount of ammo in the clip
	void useAmmo ()
	{
		currentWeapon.clipSize--;
		updateAmmo ();
	}

	void updateAmmo ()
	{
		if (!currentWeapon.infiniteAmmo) {
			hudManager.useAmmo (currentWeapon.clipSize, currentWeapon.remainAmmo.ToString ());
		} else {
			hudManager.useAmmo (currentWeapon.clipSize, "Inf");
		}
	}

	//check the amount of ammo
	void checkRemainAmmo ()
	{
		//if the weaopn has not infinite ammo
		if (!currentWeapon.infiniteAmmo) {
			if (currentWeapon.clipSize == 0) {
				//if the remaining ammo is lower that the ammo per clip, set the final projectiles in the clip 
				if (currentWeapon.remainAmmo < currentWeapon.ammoPerClip) {
					currentWeapon.clipSize = currentWeapon.remainAmmo;
				} 
			//else, refill it
			else {
					currentWeapon.clipSize = currentWeapon.ammoPerClip;
				}
				//if the remaining ammo is higher than 0, remove the current projectiles added in the clip
				if (currentWeapon.remainAmmo > 0) {
					currentWeapon.remainAmmo -= currentWeapon.clipSize;
				} 
			} else {
				int usedAmmo = 0;
				if (currentWeapon.removePreviousAmmoOnClip) {
					currentWeapon.clipSize = 0;
					if (currentWeapon.remainAmmo < (currentWeapon.ammoPerClip)) {
						usedAmmo = currentWeapon.remainAmmo;
					} else {
						usedAmmo = currentWeapon.ammoPerClip;
					}
				} else {
					if (currentWeapon.remainAmmo < (currentWeapon.ammoPerClip - currentWeapon.clipSize)) {
						usedAmmo = currentWeapon.remainAmmo;
					} else {
						usedAmmo = currentWeapon.ammoPerClip - currentWeapon.clipSize;
					}
				}
				currentWeapon.remainAmmo -= usedAmmo;
				currentWeapon.clipSize += usedAmmo;
			}
		} else {
			//else, the weapon has infinite ammo, so refill it
			currentWeapon.clipSize = currentWeapon.ammoPerClip;
		}

		currentWeapon.auxRemainAmmo = currentWeapon.remainAmmo;
	}

	//a delay for reload the weapon
	IEnumerator waitToReload (float amount)
	{
		//if the remmaining ammo is higher than 0 or infinite
		if (currentWeapon.remainAmmo > 0 || currentWeapon.infiniteAmmo) {
			//reload
			reloading = true;
			//play the reload sound
			if (currentWeapon.reloadSoundEffect) {
				weaponsEffectsSource.PlayOneShot (currentWeapon.reloadSoundEffect);
			}
			//wait an amount of time
			yield return new WaitForSeconds (amount);
			//check the ammo values
			checkRemainAmmo ();
			//stop reload
			reloading = false;
			updateAmmo ();
		} else {
			//else, the ammo is over, play the empty weapon sound
			playWeaponSoundEffect (false);
		}
		yield return null;
	}

	//the vehicle has used an ammo pickup, so increase the correct weapon by name
	public void getAmmo (string ammoName, int amount)
	{
		bool empty = false;
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].Name == ammoName) {
				if (weapons [i].remainAmmo == 0 && weapons [i].clipSize == 0) {
					empty = true;
				}
				weapons [i].remainAmmo += amount;
				weaponChanged ();
				if (empty && weapons [i].autoReloadWhenClipEmpty) {
					autoReload ();
				}
				weapons [i].auxRemainAmmo = weapons [i].remainAmmo;
				return;
			}
		}
	}

	public void addAuxRemainAmmo (vehicleWeapons weaponToCheck, int amount)
	{
		weaponToCheck.auxRemainAmmo += amount;
	}

	//select next or previous weapon
	public void chooseNextWeapon ()
	{
		if (!weaponsActivate) {
			return;
		}
			
		//check the index and get the correctly weapon 
		int max = 0;
		int numberKey = currentWeapon.numberKey;
		numberKey++;
		if (numberKey > weaponsSlotsAmount) {
			numberKey = 0;
		}
		bool exit = false;
		while (!exit) {
			for (int k = 0; k < weapons.Count; k++) {
				if (weapons [k].enabled && weapons [k].numberKey == numberKey) {
					choosedWeapon = k;
					exit = true;
				}
			}
			max++;
			if (max > 100) {
				return;
			}
			//get the current weapon index
			numberKey++;
			if (numberKey > weaponsSlotsAmount) {
				numberKey = 0;
			}
		}
		//set the current weapon 
		weaponChanged ();
	}

	public void choosePreviousWeapon ()
	{
		if (!weaponsActivate) {
			return;
		}

		int max = 0;
		int numberKey = currentWeapon.numberKey;
		numberKey--;
		if (numberKey < 0) {
			numberKey = weaponsSlotsAmount;
		}
		bool exit = false;
		while (!exit) {
			for (int k = weapons.Count - 1; k >= 0; k--) {
				if (weapons [k].enabled && weapons [k].numberKey == numberKey) {
					choosedWeapon = k;
					exit = true;
				}
			}
			max++;
			if (max > 100) {
				return;
			}
			numberKey--;
			if (numberKey < 0) {
				numberKey = weaponsSlotsAmount;
			}
		}
		weaponChanged ();
	}

	//set the info of the selected weapon in the hud
	void weaponChanged ()
	{
		currentWeapon = weapons [choosedWeapon];
		hudManager.setWeaponName (currentWeapon.Name, currentWeapon.ammoPerClip, currentWeapon.clipSize);
		if (!currentWeapon.infiniteAmmo) {
			hudManager.useAmmo (currentWeapon.clipSize, currentWeapon.remainAmmo.ToString ());
		} else {
			hudManager.useAmmo (currentWeapon.clipSize, "Inf");
		}

		checkParableTrayectory (true);

		//remove the located enemies icon
		removeLocatedEnemiesIcons ();

		checkCurrentWeaponCustomReticle ();

		checkIfEnableWeaponCursor ();
	}

	public void checkCurrentWeaponCustomReticle ()
	{
		if (weaponCursorImage) {
			if (currentWeapon.useCustomReticle) {
				weaponCursorImage.texture = currentWeapon.customReticle;

				if (currentWeapon.useCustomReticleColor) {
					weaponCursorImage.color = currentWeapon.customReticleColor;
				}

				if (currentWeapon.useCustomReticleSize) {
					weaponCursor.sizeDelta = currentWeapon.customReticleSize;
				}

				usingCustomReticle = true;
			} else {
				if (usingCustomReticle) {
					weaponCursorImage.texture = currentHUDElements.defaultVehicleCursor;
					weaponCursorImage.color = currentHUDElements.defaultVehicleCursorColor;
					weaponCursor.sizeDelta = currentHUDElements.defaultVehicleCursorSize;
				}
			}
		}
	}

	public void checkParableTrayectory (bool parableState)
	{
		//enable or disable the parable linerenderer
		if (currentWeapon.activateLaunchParable && parableState && currentWeapon.launchProjectile) {
			if (parable) {
				if (currentWeapon.parableTrayectoryPosition) {
					parableGameObject.transform.position = currentWeapon.parableTrayectoryPosition.position;
					parableGameObject.transform.rotation = currentWeapon.parableTrayectoryPosition.rotation;
					if (currentWeapon.projectilePosition.Count > 0) {
						parable.shootPosition = currentWeapon.projectilePosition [0];
					}
				}
				parable.changeParableState (true);
			}
		} else {
			if (parable) {
				parable.changeParableState (false);
			}
		}
	}

	//enable or disable the weapons in the vehicle according to if it is being droven or not
	public void changeWeaponState (bool state)
	{
		if (!weaponsEnabled) {
			return;
		}

		weaponsActivate = state;

		//if the player gets in, set the info in the hud
		if (weaponsActivate) {

			vehicleHUDInfoManager = hudManager.getCurrentVehicleHUDInfo ();

			touchEnabled = actionManager.input.isUsingTouchControls ();
			weaponChanged ();

			//get player info
			currentDriver = hudManager.getCurrentDriver ();
			if (currentDriver) {
				playerControllerManager = currentDriver.GetComponent<playerController> ();
				playerCameraManager = playerControllerManager.getPlayerCameraManager ();
				mainCamera = playerCameraManager.getMainCamera ();

				usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

				playerInputManager playerInput = currentDriver.GetComponent<playerInputManager> ();

				if (playerInput) {
					playerScreenObjectivesManager = playerInput.getPauseManager ().gameObject.GetComponent<playerScreenObjectivesSystem> ();
				}
			}

			checkCurrentWeaponCustomReticle ();

			checkIfEnableWeaponCursor ();
		} 

		//else, the player is getting off
		else {
			rotationX = 0;
			rotationY = 0;
			if (hasBaseRotation) {
				StartCoroutine (rotateWeapon ());
			}
			//if the laser is being used, disable it
			changeWeaponLaserState (false);
		}

		//disable the parable linerenderer
		checkParableTrayectory (weaponsActivate);

		if (!weaponsActivate) {
			if (locatedEnemies.Count > 0) {
				aimingHomingProjectile = false;
				//remove the icons in the screen
				removeLocatedEnemiesIcons ();
			}
		}
	}

	//get the camera info of the vehicle
	public void getCameraInfo (Transform camera, bool useRotationInput)
	{
		vehicleCameraTransform = camera;
		mainCameraTransform = vehicleCameraTransform;
		currentCameraUseRotationInput = useRotationInput;
		if (vehicleHUDInfoManager && vehicleHUDInfoManager.getVehicleCursor ()) {
			if (!weaponCursor) {
				weaponCursor = vehicleHUDInfoManager.getVehicleCursor ().GetComponent<RectTransform> ();
				 
				weaponCursorImage = weaponCursor.GetComponent<RawImage> ();

				currentHUDElements = vehicleHUDInfoManager.getHudElements ();
			}
			if (weaponCursor) {
				weaponCursor.anchoredPosition = Vector2.zero;
			}

			currentLockedCameraCursorSize = weaponCursor.sizeDelta;

			horizontaLimit = Screen.currentResolution.width - currentLockedCameraCursorSize.x;
			verticalLimit = Screen.currentResolution.height - currentLockedCameraCursorSize.y;
		}
	}

	//reset the weapon rotation when the player gets off
	IEnumerator rotateWeapon ()
	{
		Quaternion currentBaseXRotation = baseX.transform.localRotation;
		Quaternion currentBaseYRotation = baseY.transform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			baseX.transform.localRotation = Quaternion.Slerp (currentBaseXRotation, Quaternion.identity, t);
			baseY.transform.localRotation = Quaternion.Slerp (currentBaseYRotation, Quaternion.identity, t);
			yield return null;
		}
	}

	public void setSeekerProjectileInfo (GameObject projectile, Transform shootZone)
	{
		//get all the enemies in the scene
		List<GameObject> enemiesInFront = new List<GameObject> ();
		List<GameObject> fullEnemyList = new List<GameObject> ();

		for (int i = 0; i < currentWeapon.tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (currentWeapon.tagToLocate [i]);
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
			if (Physics.Raycast (shootZone.position, direction, out hit, distance, layer)) {
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
		float spreadAmount = currentWeapon.spreadAmount;
		if (spreadAmount > 0) {
			Vector3 randomSpread = Vector3.zero;
			randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.z = Random.Range (-spreadAmount, spreadAmount);
			return randomSpread;
		}
		return Vector3.zero;
	}

	//shoot to any object with the tag configured in the inspector, in case the option is enabled
	public void checkAutoShootOnTag ()
	{
		if (currentWeapon.autoShootOnTag) {
			autoShootRayPosition = mainCameraTransform.position;
			if (usingCameraWithNoInput) {
				autoShootRayPosition = weaponLookDirection.position;
			}

			if (Physics.Raycast (autoShootRayPosition, projectileDirectionToLook, out hit, currentWeapon.maxDistanceToRaycast, currentWeapon.layerToAutoShoot)) {
				GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);
				if (target != null && target != vehicle) {
					if (currentWeapon.autoShootTagList.Contains (target.tag)) {
						shootWeapon (true);
					}
				} else {
					if (currentWeapon.autoShootTagList.Contains (hit.collider.gameObject.tag) ||
					    (currentWeapon.shootAtLayerToo &&
					    (1 << hit.collider.gameObject.layer & currentWeapon.layerToAutoShoot.value) == 1 << hit.collider.gameObject.layer)) {
						shootWeapon (true);
					}
				}
			}
		}
	}

	public bool checkIfWeaponIsAvailable (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].Name == weaponName && weapons [i].enabled) {
				return true;
			}
		}
		return false;
	}

	public bool canIncreaseRemainAmmo (vehicleWeapons weaponToCheck)
	{
		if (weaponToCheck.auxRemainAmmo < weaponToCheck.ammoLimit) {
			return true;
		} else {
			return false;
		}
	}

	public bool hasMaximumAmmoAmount (vehicleWeapons weaponToCheck)
	{
		if (weaponToCheck.useAmmoLimit) {
			print (weaponToCheck.Name + " " + weaponToCheck.remainAmmo + " " + weaponToCheck.ammoLimit);
			if (weaponToCheck.remainAmmo >= weaponToCheck.ammoLimit) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public bool hasAmmoLimit (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].Name == weaponName && weapons [i].enabled && weapons [i].useAmmoLimit) {
				return true;
			}
		}
		return false;
	}

	public int ammoAmountToMaximumLimit (vehicleWeapons weaponToCheck)
	{
		return weaponToCheck.ammoLimit - weaponToCheck.auxRemainAmmo;
	}

	public bool hasMaximumAmmoAmount (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].Name == weaponName && weapons [i].enabled && hasMaximumAmmoAmount (weapons [i])) {
				return true;
			}
		}
		return false;
	}

	public projectileInfo setProjectileInfo ()
	{
		projectileInfo newProjectile = new projectileInfo ();
		newProjectile.isHommingProjectile = currentWeapon.isHommingProjectile;
		newProjectile.isSeeker = currentWeapon.isSeeker;
		newProjectile.waitTimeToSearchTarget = currentWeapon.waitTimeToSearchTarget;
		newProjectile.useRayCastShoot = currentWeapon.useRayCastShoot;

		newProjectile.useRaycastCheckingOnRigidbody = currentWeapon.useRaycastCheckingOnRigidbody;

		newProjectile.projectileDamage = currentWeapon.projectileDamage;
		newProjectile.projectileSpeed = currentWeapon.projectileSpeed;

		newProjectile.impactForceApplied = currentWeapon.impactForceApplied;
		newProjectile.forceMode = currentWeapon.forceMode;
		newProjectile.applyImpactForceToVehicles = currentWeapon.applyImpactForceToVehicles;
		newProjectile.impactForceToVehiclesMultiplier = currentWeapon.impactForceToVehiclesMultiplier;

		newProjectile.projectileWithAbility = currentWeapon.projectileWithAbility;

		newProjectile.impactSoundEffect = currentWeapon.impactSoundEffect;

		newProjectile.scorch = currentWeapon.scorch;
		newProjectile.scorchRayCastDistance = currentWeapon.scorchRayCastDistance;

		newProjectile.owner = vehicle;

		newProjectile.projectileParticles = currentWeapon.projectileParticles;
		newProjectile.impactParticles = currentWeapon.impactParticles;

		newProjectile.isExplosive = currentWeapon.isExplosive;
		newProjectile.isImplosive = currentWeapon.isImplosive;
		newProjectile.useExplosionDelay = currentWeapon.useExplosionDelay;
		newProjectile.explosionDelay = currentWeapon.explosionDelay;
		newProjectile.explosionForce = currentWeapon.explosionForce;
		newProjectile.explosionRadius = currentWeapon.explosionRadius;
		newProjectile.explosionDamage = currentWeapon.explosionDamage;
		newProjectile.pushCharacters = currentWeapon.pushCharacters;
		newProjectile.applyExplosionForceToVehicles = currentWeapon.applyExplosionForceToVehicles;
		newProjectile.explosionForceToVehiclesMultiplier = currentWeapon.explosionForceToVehiclesMultiplier;

		newProjectile.killInOneShot = currentWeapon.killInOneShot;

		newProjectile.useDisableTimer = currentWeapon.useDisableTimer;
		newProjectile.noImpactDisableTimer = currentWeapon.noImpactDisableTimer;
		newProjectile.impactDisableTimer = currentWeapon.impactDisableTimer;

		newProjectile.targetToDamageLayer = layer;
		newProjectile.targetForScorchLayer = targetForScorchLayer;

		newProjectile.impactDecalIndex = currentWeapon.impactDecalIndex;

		newProjectile.launchProjectile = currentWeapon.launchProjectile;

		newProjectile.adhereToSurface = currentWeapon.adhereToSurface;
		newProjectile.adhereToLimbs = currentWeapon.adhereToLimbs;

		newProjectile.useGravityOnLaunch = currentWeapon.useGravityOnLaunch;
		newProjectile.useGraivtyOnImpact = currentWeapon.useGraivtyOnImpact;

		newProjectile.breakThroughObjects = currentWeapon.breakThroughObjects;
		newProjectile.infiniteNumberOfImpacts = currentWeapon.infiniteNumberOfImpacts;
		newProjectile.numberOfImpacts = currentWeapon.numberOfImpacts;
		newProjectile.canDamageSameObjectMultipleTimes = currentWeapon.canDamageSameObjectMultipleTimes;
		newProjectile.forwardDirection = projectileDirectionToLook;

		newProjectile.damageTargetOverTime = currentWeapon.damageTargetOverTime;
		newProjectile.damageOverTimeDelay = currentWeapon.damageOverTimeDelay;
		newProjectile.damageOverTimeDuration = currentWeapon.damageOverTimeDuration;
		newProjectile.damageOverTimeAmount = currentWeapon.damageOverTimeAmount;
		newProjectile.damageOverTimeRate = currentWeapon.damageOverTimeRate;
		newProjectile.damageOverTimeToDeath = currentWeapon.damageOverTimeToDeath;
		newProjectile.removeDamageOverTimeState = currentWeapon.removeDamageOverTimeState;

		newProjectile.sedateCharacters = currentWeapon.sedateCharacters;
		newProjectile.sedateDelay = currentWeapon.sedateDelay;
		newProjectile.useWeakSpotToReduceDelay = currentWeapon.useWeakSpotToReduceDelay;
		newProjectile.sedateDuration = currentWeapon.sedateDuration;
		newProjectile.sedateUntilReceiveDamage = currentWeapon.sedateUntilReceiveDamage;

		newProjectile.pushCharacter = currentWeapon.pushCharacter;
		newProjectile.pushCharacterForce = currentWeapon.pushCharacterForce;
		newProjectile.pushCharacterRagdollForce = currentWeapon.pushCharacterRagdollForce;


		return newProjectile;
	}

	public void enableMuzzleFlashLight ()
	{
		if (!currentWeapon.useMuzzleFlash) {
			return;
		}

		if (muzzleFlahsCoroutine != null) {
			StopCoroutine (muzzleFlahsCoroutine);
		}
		muzzleFlahsCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		currentWeapon.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (currentWeapon.muzzleFlahsDuration);

		currentWeapon.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	public void setWeaponsPausedState (bool state)
	{
		weaponsPaused = state;
	}


	//CALL INPUT FUNCTIONS
	public void inputShootWeapon ()
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			//fire the current weapon
			if (currentWeapon.automatic) {
				shootWeapon (true);
			} 
		}
	}

	public void inputHoldOrReleaseShootWeapon (bool holdingButton)
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			if (holdingButton) {
				shootWeapon (true);
			} else {
				//if the shoot button is released, reset the last shoot timer
				shootWeapon (false);
				lastShoot = 0;

				if (usingLaser) {
					if (currentWeapon.clipSize > 0) {
						changeWeaponLaserState (false);
					}
				}
			}
		}
	}

	public void inputNextOrPreviousWeapon (bool setNextWeapon)
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			//select the power using the mouse wheel or the change power buttons
			if (setNextWeapon) {
				chooseNextWeapon ();
			} else {
				choosePreviousWeapon ();
			}
		}
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<vehicleWeaponSystem> ());
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

	#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			//set the change weapon touch zone in the right upper corner of the scren, visile as gizmo
			if (!EditorApplication.isPlaying) {
				setHudZone ();
			}

			if (swipeCenterPosition) {
				Gizmos.color = gizmoColor;
				touchZone = new Vector3 (touchZoneRect.x + touchZoneRect.width / 2f, touchZoneRect.y + touchZoneRect.height / 2f, swipeCenterPosition.position.z);
				Gizmos.DrawWireCube (touchZone, new Vector3 (touchZoneSize.x, touchZoneSize.y, 0f));
			}
		}
	}

	#endif
	//get the correct size of the rect
	void setHudZone ()
	{
		if (!swipeCenterPosition) {
			if (vehicleHUDInfoManager == null) {
				vehicleHUDInfoManager = FindObjectOfType<vehicleHUDInfo> ();
			}

			if (vehicleHUDInfoManager) {
				swipeCenterPosition = vehicleHUDInfoManager.getVehicleWeaponsSwipePosition ();
			}
		}

		if (swipeCenterPosition) {
			touchZoneRect = new Rect (swipeCenterPosition.position.x - touchZoneSize.x / 2f, swipeCenterPosition.position.y - touchZoneSize.y / 2f, touchZoneSize.x, touchZoneSize.y);
		}
	}

	[System.Serializable]
	public class vehicleWeapons
	{
		public string Name;
		public int numberKey;

		public bool useCustomReticle;
		public Texture customReticle;
		public bool useCustomReticleColor;
		public Color customReticleColor = Color.white;
		public bool useCustomReticleSize;
		public Vector2 customReticleSize = new Vector2 (60, 60);

		public bool disableWeaponCursorWhileNotShooting;
		public float delayToDisableWeaponCursor;

		public bool useRayCastShoot;
		public bool useRaycastAllToCheckSurfaceFound = true;
		public float maxDistanceToRaycastAll = 200;

		public bool useRaycastCheckingOnRigidbody;

		public bool fireWeaponForward;
		public bool enabled;

		public int ammoPerClip;
		public bool removePreviousAmmoOnClip;
		public bool infiniteAmmo;
		public int remainAmmo;
		public int clipSize;
		public bool startWithEmptyClip;
		public bool autoReloadWhenClipEmpty = true;
		public bool useAmmoLimit;
		public int ammoLimit;
		public int auxRemainAmmo;

		public bool shootAProjectile;
		public bool launchProjectile;

		public bool projectileWithAbility;

		public bool automatic;

		public float fireRate;
		public float reloadTime;
		public float projectileDamage;
		public float projectileSpeed;
		public int projectilesPerShoot;

		public bool useProjectileSpread;
		public float spreadAmount;

		public bool isImplosive;
		public bool isExplosive;
		public float explosionForce;
		public float explosionRadius;
		public bool useExplosionDelay;
		public float explosionDelay;
		public float explosionDamage;
		public bool pushCharacters;
		public bool applyExplosionForceToVehicles = true;
		public float explosionForceToVehiclesMultiplier = 0.2f;

		public List<Transform> projectilePosition = new List<Transform> ();

		public bool ejectShellOnShot;
		public GameObject shell;
		public List<Transform> shellPosition = new List<Transform> ();
		public float shellEjectionForce = 200;
		public List<AudioClip> shellDropSoundList = new List<AudioClip> ();

		public GameObject weapon;
		public string animation;
		public Animation weaponAnimation;
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
		public float impactForceToVehiclesMultiplier = 1;

		public AudioClip reloadSoundEffect;
		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;
		public AudioClip outOfAmmo;

		public GameObject muzzleParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		public bool killInOneShot;

		public bool useDisableTimer;
		public float noImpactDisableTimer;
		public float impactDisableTimer;

		public string locatedEnemyIconName = "Homing Located Enemy";
		public List<string> tagToLocate = new List<string> ();

		public bool activateLaunchParable;
		public bool useParableSpeed;
		public Transform parableTrayectoryPosition;
		public Transform parableDirectionTransform;

		public bool adhereToSurface;
		public bool adhereToLimbs;

		public bool useGravityOnLaunch;
		public bool useGraivtyOnImpact;

		public GameObject projectileModel;
	
		public GameObject projectileToShoot;
	
		public bool showShakeSettings;
		public vehicleCameraController.shakeSettingsInfo shootShakeInfo;

		public int impactDecalIndex;
		public string impactDecalName;

		public bool isLaser;

		public bool breakThroughObjects;
		public bool infiniteNumberOfImpacts;
		public int numberOfImpacts;
		public bool canDamageSameObjectMultipleTimes;

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
	}
}