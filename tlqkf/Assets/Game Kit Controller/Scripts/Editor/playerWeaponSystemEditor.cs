using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerWeaponSystem))]
[CanEditMultipleObjects]
public class playerWeaponSystemEditor : Editor
{
	SerializedObject objectToUse;
	playerWeaponSystem weapon;
	bool settings;
	bool showDrawAimFunctionSettings;
	Color buttonColor;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		weapon = (playerWeaponSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Weapon State", "window", GUILayout.Height (30));
		GUILayout.Label ("Reloading\t\t " + objectToUse.FindProperty ("reloading").boolValue);
		GUILayout.Label ("Carrying 3rd Person\t " + objectToUse.FindProperty ("carryingWeaponInThirdPerson").boolValue);
		GUILayout.Label ("Carrying 1st Person\t " + objectToUse.FindProperty ("carryingWeaponInFirstPerson").boolValue);
		GUILayout.Label ("Aiming 3rd Person\t " + objectToUse.FindProperty ("aimingInThirdPerson").boolValue);
		GUILayout.Label ("Aiming 1st Person\t " + objectToUse.FindProperty ("aimingInFirstPerson").boolValue);
		GUILayout.Label ("Key Number\t " + objectToUse.FindProperty ("weaponSettings.numberKey").intValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Components", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerControllerGameObject"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerCameraGameObject"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layer"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		settings = objectToUse.FindProperty ("showSettings").boolValue;

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		string inputListOpenedText = "";
		if (settings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Weapon Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Weapon Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			settings = !settings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		objectToUse.FindProperty ("showSettings").boolValue = settings;

		if (settings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the shot settings of this weapon", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			showWeaponSettings (objectToUse.FindProperty ("weaponSettings"));
		}

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showWeaponSettings (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Weapon Info", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponIcon"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponInventorySlotIcon"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("HUD Elements Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponIConHUD"));	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponNameInHUD"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponIconInHUD"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponAmmoSliderInHUD"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponAmmoTextInHUD"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Reticle Setting", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticle"));
		if (list.FindPropertyRelative ("useCustomReticle").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("regularCustomReticle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAimCustomReticle"));
			if (list.FindPropertyRelative ("useAimCustomReticle").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimCustomReticle"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fire Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
		if (list.FindPropertyRelative ("automatic").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBurst"));
			if (list.FindPropertyRelative ("useBurst").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("burstAmount"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Reload Settings", "window", GUILayout.Height (30));
		GUILayout.BeginVertical ("Third Person Reload Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTimeThirdPerson"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadDelayThirdPerson"));
		if (list.FindPropertyRelative ("useReloadDelayThirdPerson").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDelayThirdPerson"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePreReloadDelayThirdPerson"));
		if (list.FindPropertyRelative ("usePreReloadDelayThirdPerson").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("preReloadDelayThirdPerson"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("First Person Reload Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTimeFirstPerson"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadDelayFirstPerson"));
		if (list.FindPropertyRelative ("useReloadDelayFirstPerson").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDelayFirstPerson"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePreReloadDelayFirstPerson"));
		if (list.FindPropertyRelative ("usePreReloadDelayFirstPerson").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("preReloadDelayFirstPerson"));
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectilesPerShoot"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("killInOneShot"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileWithAbility"));

		if (list.FindPropertyRelative ("launchProjectile").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Launch Projectile Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableFirstPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParableSpeed"));
			if (!list.FindPropertyRelative ("useParableSpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableDirectionTransform"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireWeaponForward"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));
			if (list.FindPropertyRelative ("useRayCastShoot").boolValue) {
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Shoot Delay and Fake Trail Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastShootDelay"));
				if (list.FindPropertyRelative ("useRaycastShootDelay").boolValue) {

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("getDelayWithDistance"));
					if (list.FindPropertyRelative ("getDelayWithDistance").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayWithDistanceSpeed"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDelayWithDistance"));
					} else {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("raycastShootDelay"));
					}
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFakeProjectileTrails"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
			}
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Search Target Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isHommingProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isSeeker"));
		if (list.FindPropertyRelative ("isHommingProjectile").boolValue || list.FindPropertyRelative ("isSeeker").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToSearchTarget"));
			showTagToLocateList (list.FindPropertyRelative ("tagToLocate"));
		}
		if (list.FindPropertyRelative ("isHommingProjectile").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("locatedEnemyIconName"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ammo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponUsesAmmo"));
		if (list.FindPropertyRelative ("weaponUsesAmmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSize"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmmo"));
			if (!list.FindPropertyRelative ("infiniteAmmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmo"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("removePreviousAmmoOnClip"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropClipWhenReload"));
			if (list.FindPropertyRelative ("dropClipWhenReload").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionToDropClip"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipModel"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("startWithEmptyClip"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("autoReloadWhenClipEmpty"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmmoLimit"));
			if (list.FindPropertyRelative ("useAmmoLimit").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoLimit"));
			}
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
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageProjectileOwner"));
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

		GUILayout.BeginVertical ("Sound Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("silencerShootEffect"));	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("outOfAmmo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cockSound"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundsPool"));
		if (list.FindPropertyRelative ("useSoundsPool").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxSoundsPoolAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponEffectSourcePrefab"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponEffectSourceParent"));
		}
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Draw/Keep Weapon Sound Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnDrawKeepWeapon"));
		if (list.FindPropertyRelative ("useSoundOnDrawKeepWeapon").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponSound"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepWeaponSound"));
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Particle Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootParticles"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
			
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ability Weapon Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponWithAbility"));
		if (list.FindPropertyRelative ("weaponWithAbility").boolValue) {
				
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Button Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDownButton"));
			if (list.FindPropertyRelative ("useDownButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("downButtonAction"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHoldButton"));
			if (list.FindPropertyRelative ("useHoldButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("holdButtonAction"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUpButton"));
			if (list.FindPropertyRelative ("useUpButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("upButtonAction"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();


		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Draw/Aim Settings", "window", GUILayout.Height (30));
		buttonColor = GUI.backgroundColor;
		showDrawAimFunctionSettings = list.FindPropertyRelative ("showDrawAimFunctionSettings").boolValue;

		EditorGUILayout.BeginVertical ();
		string inputListOpenedText = "";
		if (showDrawAimFunctionSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Draw/Aim Function Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Draw/Aim Function Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showDrawAimFunctionSettings = !showDrawAimFunctionSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		list.FindPropertyRelative ("showDrawAimFunctionSettings").boolValue = showDrawAimFunctionSettings;

		if (showDrawAimFunctionSettings) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawAction"));
			if (list.FindPropertyRelative ("useStartDrawAction").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawAction"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawAction"));
			if (list.FindPropertyRelative ("useStopDrawAction").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawAction"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Third Person Draw Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawActionThirdPerson"));
			if (list.FindPropertyRelative ("useStartDrawActionThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawActionThirdPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawActionThirdPerson"));
			if (list.FindPropertyRelative ("useStopDrawActionThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawActionThirdPerson"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Draw Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawActionFirstPerson"));
			if (list.FindPropertyRelative ("useStartDrawActionFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawActionFirstPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawActionFirstPerson"));
			if (list.FindPropertyRelative ("useStopDrawActionFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawActionFirstPerson"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Third Person Aim Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartAimActionThirdPerson"));
			if (list.FindPropertyRelative ("useStartAimActionThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startAimActionThirdPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopAimActionThirdPerson"));
			if (list.FindPropertyRelative ("useStopAimActionThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAimActionThirdPerson"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Aim Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartAimActionFirstPerson"));
			if (list.FindPropertyRelative ("useStartAimActionFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startAimActionFirstPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopAimActionFirstPerson"));
			if (list.FindPropertyRelative ("useStopAimActionFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAimActionFirstPerson"));
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Secondary Action Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryAction"));
		if (list.FindPropertyRelative ("useSecondaryAction").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryAction"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnDownPress"));
		if (list.FindPropertyRelative ("useSecondaryActionOnDownPress").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnDownPress"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnHoldPress"));
		if (list.FindPropertyRelative ("useSecondaryActionOnHoldPress").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnHoldPress"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnUpPress"));
		if (list.FindPropertyRelative ("useSecondaryActionOnUpPress").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnUpPress"));
		}

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Spread Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
		if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spreadAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sameSpreadInThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonSpreadAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSpreadAming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useLowerSpreadAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lowerSpreadAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Position Settings", "window", GUILayout.Height (30));
		showSimpleList (list.FindPropertyRelative ("projectilePosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shell Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shell"));
		if (list.FindPropertyRelative ("shell").objectReferenceValue) {
			showSimpleList (list.FindPropertyRelative ("shellPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellEjectionForce"));
			showSimpleList (list.FindPropertyRelative ("shellDropSoundList"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShellDelay"));
			if (list.FindPropertyRelative ("useShellDelay").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellDelayThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellDelayFirsPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("createShellsOnReload"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Components", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponMesh"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHumanBodyBonesEnum"));	
		if (objectToUse.FindProperty ("useHumanBodyBonesEnum").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponParentBone"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponParent"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Animations", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFireAnimation"));
		if (list.FindPropertyRelative ("useFireAnimation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animation"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playAnimationBackward"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadAnimation"));
		if (list.FindPropertyRelative ("useReloadAnimation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadAnimationName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadAnimationSpeed"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch Settings", "window", GUILayout.Height (30));
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch from Decal Manager", "window", GUILayout.Height (30));
		if (objectToUse.FindProperty ("impactDecalList").arraySize > 0) {
			objectToUse.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
				objectToUse.FindProperty ("impactDecalIndex").intValue, weapon.impactDecalList);
			objectToUse.FindProperty ("impactDecalName").stringValue = weapon.impactDecalList [objectToUse.FindProperty ("impactDecalIndex").intValue];
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("getImpactListEveryFrame"));
		if (!objectToUse.FindProperty ("getImpactListEveryFrame").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Decal Impact List")) {
				weapon.getImpactListInfo ();					
			}

			EditorGUILayout.Space ();

		}

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
			showSimpleList (list.FindPropertyRelative ("autoShootTagList"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("HUD Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCanvasHUD"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUD"));
		if (list.FindPropertyRelative ("useHUD").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSizeText"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmoText"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoInfoHUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableHUDInFirstPersonAim"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeHUDPosition"));
			if (list.FindPropertyRelative ("changeHUDPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDTransformInThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDTransformInFirstPerson"));
			}
				
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUDDualWeaponThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUDDualWeaponFirstPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeHUDPositionDualWeapon"));
			if (list.FindPropertyRelative ("changeHUDPositionDualWeapon").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDRightHandTransformThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDLeftHandTransformThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDRightHandTransformFirstPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDLeftHandTransformFirstPerson"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Force Settings", "window", GUILayout.Height (30));
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

		GUILayout.BeginVertical ("Upper Body Shake Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeUpperBodyWhileShooting"));
		if (list.FindPropertyRelative ("shakeUpperBodyWhileShooting").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSpeed"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon Upper Body Shake Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeUpperBodyShootingDualWeapons"));
		if (list.FindPropertyRelative ("shakeUpperBodyShootingDualWeapons").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponShakeAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponShakeSpeed"));
		}
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnSetDualWeapon"));
		if (objectToUse.FindProperty ("useEventOnSetDualWeapon").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnSetRightWeapon"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnSetLeftWeapon"));
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Single Weapon Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnSetSingleWeapon"));
		if (objectToUse.FindProperty ("useEventOnSetSingleWeapon").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnSetSingleWeapon"));
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

		GUILayout.BeginVertical ("Shoot Noise Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoise"));
		if (list.FindPropertyRelative ("useNoise").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseRadius"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseExpandSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseDetection"));
			if (list.FindPropertyRelative ("useNoiseDetection").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseDetectionLayer"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("showNoiseDetectionGizmo"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mark Targets Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("canMarkTargets"));
		if (objectToUse.FindProperty ("canMarkTargets").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Tag List To Mark Targets", "window", GUILayout.Height (30));
			showSimpleList (objectToUse.FindProperty ("tagListToMarkTargets"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("markTargetName"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxDistanceToMarkTargets"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("markTargetsLayer"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canMarkTargetsOnFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("canMarkTargetsOnThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("aimOnFirstPersonToMarkTarget"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useMarkTargetSound"));
			if (objectToUse.FindProperty ("useMarkTargetSound").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("markTargetSound"));
			}
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize.ToString ());

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

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
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
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}

	void showTagToLocateList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Tags: \t" + list.arraySize.ToString ());

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

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif