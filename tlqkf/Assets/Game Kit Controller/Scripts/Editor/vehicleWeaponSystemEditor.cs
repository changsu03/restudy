using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(vehicleWeaponSystem))]
public class vehicleWeaponSystemEditor : Editor
{
	SerializedObject systemToUse;
	Color buttonColor;
	vehicleWeaponSystem manager;
	bool expanded;

	void OnEnable ()
	{
		systemToUse = new SerializedObject (targets);
		manager = (vehicleWeaponSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (systemToUse == null) {
			return;
		}

		systemToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Basic Settings", "window");
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponsEnabled"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("vehicle"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponsEffectsSource"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("outOfAmmo"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("layer"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("targetForScorchLayer"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponLookDirection"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponsSlotsAmount"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponIndexToStart"));	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Base Rotation Settings", "window");
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("hasBaseRotation"));
		if (systemToUse.FindProperty ("hasBaseRotation").boolValue) {
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("minimumX"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("maximumX"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("minimumY"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("maximumY"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("baseX"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("baseY"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("baseXRotationSpeed"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("baseYRotationSpeed"));

			EditorGUILayout.PropertyField (systemToUse.FindProperty ("noInputWeaponDirection"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("canRotateWeaponsWithNoCameraInput"));
			if (systemToUse.FindProperty ("canRotateWeaponsWithNoCameraInput").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("noCameraInputWeaponRotationSpeed"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponCursorMovementSpeed"));

				EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponCursorRaycastLayer"));

				EditorGUILayout.PropertyField (systemToUse.FindProperty ("useWeaponCursorScreenLimit"));
				if (systemToUse.FindProperty ("useWeaponCursorScreenLimit").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponCursorHorizontalLimit"));
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponCursorVerticalLimit"));
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Elements", "window");
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("vehicleCameraManager"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("mainRigidbody"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("actionManager"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("hudManager"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("parable"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("gravityControlManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("touchZoneSize"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("minSwipeDist"));
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("showGizmo"));
		if (systemToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("gizmoColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("State Info", "window");
		GUILayout.Label ("Weapons Activate\t\t" + systemToUse.FindProperty ("weaponsActivate").boolValue.ToString ());
		GUILayout.Label ("Reloading\t\t\t" + systemToUse.FindProperty ("reloading").boolValue.ToString ());
		GUILayout.Label ("Aiming Correctly\t\t" + systemToUse.FindProperty ("aimingCorrectly").boolValue.ToString ());
		GUILayout.Label ("Current Weapon Index\t" + systemToUse.FindProperty ("choosedWeapon").intValue.ToString ());
		GUILayout.Label ("Current Weapon Name\t" + systemToUse.FindProperty ("currentWeapon.Name").stringValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons List", "window");
		showUpperList (systemToUse.FindProperty ("weapons"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			systemToUse.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list, int weaponIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Info", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Custom Reticle Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticle"));
		if (list.FindPropertyRelative ("useCustomReticle").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticleColor"));
			if (list.FindPropertyRelative ("useCustomReticleColor").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticleColor"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticleSize"));
			if (list.FindPropertyRelative ("useCustomReticleSize").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticleSize"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Cursor Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableWeaponCursorWhileNotShooting"));
		if (list.FindPropertyRelative ("disableWeaponCursorWhileNotShooting").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToDisableWeaponCursor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fire Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTime"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));
		if (list.FindPropertyRelative ("shootAProjectile").boolValue || list.FindPropertyRelative ("launchProjectile").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileToShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectilesPerShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileDamage"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killInOneShot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileWithAbility"));

			if (list.FindPropertyRelative ("launchProjectile").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Launch Projectile Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParable"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParableSpeed"));
				if (!list.FindPropertyRelative ("useParableSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableDirectionTransform"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableTrayectoryPosition"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileModel"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireWeaponForward"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));
				if (!list.FindPropertyRelative ("useRayCastShoot").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastAllToCheckSurfaceFound"));
					if (list.FindPropertyRelative ("useRaycastAllToCheckSurfaceFound").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToRaycastAll"));
					}
				}
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isHommingProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isSeeker"));
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue || list.FindPropertyRelative ("isSeeker").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToSearchTarget"));
				showLowerList (list.FindPropertyRelative ("tagToLocate"));
			}
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("locatedEnemyIconName"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Force Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceApplied"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceMode"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyImpactForceToVehicles"));
			if (list.FindPropertyRelative ("applyImpactForceToVehicles").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceToVehiclesMultiplier"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Explosion Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isExplosive"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isImplosive"));
			if (list.FindPropertyRelative ("isExplosive").boolValue || list.FindPropertyRelative ("isImplosive").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForce"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExplosionDelay"));
				if (list.FindPropertyRelative ("useExplosionDelay").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDelay"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDamage"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacters"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyExplosionForceToVehicles"));
				if (list.FindPropertyRelative ("applyExplosionForceToVehicles").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForceToVehiclesMultiplier"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Projectile Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDisableTimer"));
			if (list.FindPropertyRelative ("useDisableTimer").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noImpactDisableTimer"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactDisableTimer"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particle Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("outOfAmmo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSoundEffect"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ammo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSize"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmmo"));
		if (!list.FindPropertyRelative ("infiniteAmmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmo"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmmoLimit"));
		if (list.FindPropertyRelative ("useAmmoLimit").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoLimit"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Laser Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isLaser"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Spread Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
		if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spreadAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Position Settings", "window", GUILayout.Height (30));
		showLowerList (list.FindPropertyRelative ("projectilePosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shell Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ejectShellOnShot"));
		if (list.FindPropertyRelative ("ejectShellOnShot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shell"));
			if (list.FindPropertyRelative ("shell").objectReferenceValue) {
				showLowerList (list.FindPropertyRelative ("shellPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellEjectionForce"));
				showLowerList (list.FindPropertyRelative ("shellDropSoundList"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Components", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animation"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch Settings", "window", GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch from Decal Manager", "window", GUILayout.Height (30));
		if (systemToUse.FindProperty ("impactDecalList").arraySize > 0) {
			list.FindPropertyRelative ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
				list.FindPropertyRelative ("impactDecalIndex").intValue, manager.impactDecalList);
			list.FindPropertyRelative ("impactDecalName").stringValue = manager.impactDecalList [list.FindPropertyRelative ("impactDecalIndex").intValue];
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Decal Impact List")) {
			manager.getImpactListInfo ();					
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Regular Scorch", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorch"));
		if (list.FindPropertyRelative ("scorch").objectReferenceValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorchRayCastDistance"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Auto Shoot Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("autoShootOnTag"));
		if (list.FindPropertyRelative ("autoShootOnTag").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerToAutoShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToRaycast"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAtLayerToo"));
			GUILayout.BeginVertical ("Auto Shoot Tag List", "window", GUILayout.Height (30));
			showLowerList (list.FindPropertyRelative ("autoShootTagList"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Force Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyForceAtShoot"));
		if (list.FindPropertyRelative ("applyForceAtShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Adherence Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGravityOnLaunch"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGraivtyOnImpact"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToSurface"));
		if (list.FindPropertyRelative ("adhereToSurface").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToLimbs"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Pierce Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("breakThroughObjects"));
		if (list.FindPropertyRelative ("breakThroughObjects").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteNumberOfImpacts"));
			if (!list.FindPropertyRelative ("infiniteNumberOfImpacts").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfImpacts"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageSameObjectMultipleTimes"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Muzzle Flash Light Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMuzzleFlash"));
		if (list.FindPropertyRelative ("useMuzzleFlash").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsLight"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsDuration"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Target Over Time Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTargetOverTime"));
		if (list.FindPropertyRelative ("damageTargetOverTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDuration"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeRate"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeToDeath"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeDamageOverTimeState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sedate Characters Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateCharacters"));
		if (list.FindPropertyRelative ("sedateCharacters").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWeakSpotToReduceDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateUntilReceiveDamage"));
			if (!list.FindPropertyRelative ("sedateUntilReceiveDamage").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDuration"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Push Characters Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacter"));
		if (list.FindPropertyRelative ("pushCharacter").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterForce"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterRagdollForce"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		bool shakeSettings = list.FindPropertyRelative ("showShakeSettings").boolValue;
		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		string buttonText = "";
		if (shakeSettings) {
			GUI.backgroundColor = Color.gray;
			buttonText = "Hide Shake Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonText = "Show Shake Settings";
		}
		if (GUILayout.Button (buttonText)) {
			shakeSettings = !shakeSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();
		list.FindPropertyRelative ("showShakeSettings").boolValue = shakeSettings;

		if (shakeSettings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shake Settings when this weapon fires", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shake Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShake"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.sameValueBothViews"));
			if (list.FindPropertyRelative ("shootShakeInfo.useDamageShake").boolValue) {

				EditorGUILayout.Space ();

				if (list.FindPropertyRelative ("shootShakeInfo.sameValueBothViews").boolValue) {
					showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.thirdPersonDamageShake"), "Shake In Third Person");
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInThirdPerson"));
					if (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInThirdPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.thirdPersonDamageShake"), "Shake In Third Person");

						EditorGUILayout.Space ();

					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInFirstPerson"));
					if (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInFirstPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.firstPersonDamageShake"), "Shake In First Person");

						EditorGUILayout.Space ();

					}
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						manager.testWeaponShake (weaponIndex);
					}
				}
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of weapons: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showListElementInfo (list.GetArrayElementAtIndex (i), i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showLowerList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
					return;
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showShakeInfo (SerializedProperty list, string shakeName)
	{
		GUILayout.BeginVertical (shakeName, "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		GUILayout.EndVertical ();
	}
}
#endif