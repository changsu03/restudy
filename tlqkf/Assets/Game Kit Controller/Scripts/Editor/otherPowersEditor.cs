using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(otherPowers))]
public class otherPowersEditor : Editor
{
	SerializedObject systemToUse;
	otherPowers powersManager;
	bool settings;
	bool aimSettings;
	bool shootSettings;
	Color buttonColor;
	bool powerEnabled;
	string powerEnabledString;
	bool expanded;

	void OnEnable ()
	{
		systemToUse = new SerializedObject (target);
		powersManager = (otherPowers)target;
	}

	public override void OnInspectorGUI ()
	{
		if (systemToUse == null) {
			return;
		}
		systemToUse.Update ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current Powers Action", "window");
		string powerName = "";
		if (powersManager.shootsettings.powersList.Count > 0) {
			powerName = powersManager.shootsettings.powersList [powersManager.choosedPower].Name;
		}
		GUILayout.Label ("Current Power Index\t " + systemToUse.FindProperty ("choosedPower").intValue.ToString () + "-" + powerName);
		GUILayout.Label ("Current Power         " + systemToUse.FindProperty ("currentPower.Name").stringValue);
		GUILayout.Label ("Amount Powers\t " + systemToUse.FindProperty ("amountPowersEnabled").intValue.ToString ());
		GUILayout.Label ("Carrying Objects\t " + systemToUse.FindProperty ("carryingObjects").boolValue);
		GUILayout.Label ("Wall Walking\t " + systemToUse.FindProperty ("wallWalk").boolValue);
		GUILayout.Label ("Running\t\t " + systemToUse.FindProperty ("running").boolValue);
		GUILayout.Label ("Using Shield\t " + systemToUse.FindProperty ("activatedShield").boolValue);
		GUILayout.Label ("Using Laser\t " + systemToUse.FindProperty ("laserActive").boolValue);
		GUILayout.Label ("Using Free Fire Mode\t " + systemToUse.FindProperty ("usingFreeFireMode").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Powers State", "window");

		string aimingInThirdPerson = "NO";
		if (Application.isPlaying) {
			if (systemToUse.FindProperty ("aimingInThirdPerson").boolValue) {
				aimingInThirdPerson = "YES";
			} 
		} 
		GUILayout.Label ("Aiming In Third Person\t " + aimingInThirdPerson);

		string aimingInFirstPerson = "NO";
		if (Application.isPlaying) {
			if (systemToUse.FindProperty ("aimingInFirstPerson").boolValue) {
				aimingInFirstPerson = "YES";
			}
		} 
		GUILayout.Label ("Aiming In First Person\t\t " + aimingInFirstPerson);

		string usingPowers = "NO";
		if (Application.isPlaying) {
			if (systemToUse.FindProperty ("usingPowers").boolValue) {
				usingPowers = "YES";
			}
		} 
		GUILayout.Label ("Using Powers\t\t " + usingPowers);

		string shootingWeapon = "NO";
		if (systemToUse.FindProperty ("shooting").boolValue) {
			shootingWeapon = "YES";
		}
		GUILayout.Label ("Shooting Power\t\t" + shootingWeapon);

		string powersModeActive = "NO";
		if (systemToUse.FindProperty ("powersModeActive").boolValue) {
			powersModeActive = "YES";
		}
		GUILayout.Label ("Powers Mode Active\t\t" + powersModeActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		settings = systemToUse.FindProperty ("showSettings").boolValue;
		aimSettings = systemToUse.FindProperty ("showAimSettings").boolValue;
		shootSettings = systemToUse.FindProperty ("showShootSettings").boolValue;

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (settings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Settings")) {
			settings = !settings;
		}
		if (aimSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Aim Settings")) {
			aimSettings = !aimSettings;
		}
		if (shootSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Shoot Settings")) {
			shootSettings = !shootSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		systemToUse.FindProperty ("showSettings").boolValue = settings;
		systemToUse.FindProperty ("showAimSettings").boolValue = aimSettings;
		systemToUse.FindProperty ("showShootSettings").boolValue = shootSettings;

		if (settings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Basic Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			GUILayout.BeginVertical ("Enabled abilities", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.runOnCrouchEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("wallWalkEnabled"));
			if (systemToUse.FindProperty ("wallWalkEnabled").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("stopGravityAdherenceWhenStopRun"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("wallWalkRotationSpeed"));
			}
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.aimModeEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.shieldEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.grabObjectsEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.shootEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.changePowersEnabled"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.trailsActive"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("grabbedObjectCanBeBroken"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("canGrabObjectsUsingWeapons"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("laserAbilityEnabled"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Settings", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.grabRadius"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.layer"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("layerToDamage"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("changePowersWithNumberKeysActive"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("changePowersWithMouseWheelActive"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("changePowersWithKeysActive"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Movements Settings", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("canFirePowersWithoutAiming"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useAimCameraOnFreeFireMode"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("timeToStopAimAfterStopFiring"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("changeCameraSideActive"));

			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useLowerRotationSpeedAimedThirdPerson"));
			if (systemToUse.FindProperty ("useLowerRotationSpeedAimedThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("verticalRotationSpeedAimedInThirdPerson"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("horizontalRotationSpeedAimedInThirdPerson"));
			}
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("runWhenAimingPowerInThirdPerson"));
			if (systemToUse.FindProperty ("runWhenAimingPowerInThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("stopRunIfPreviouslyNotRunning"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Teleport Ability Settings", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("teleportingEnabled"));
			if (systemToUse.FindProperty ("teleportingEnabled").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("teleportLayerMask"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("useBulletTimeOnTeleport"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("bulletTimeScaleOnTeleport"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("maxDistanceToTeleport"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("useTeleporIfSurfaceNotFound"));
				if (systemToUse.FindProperty ("useTeleporIfSurfaceNotFound").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("maxDistanceToTeleportAir"));
				}
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("holdButtonTimeToActivateTeleport"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("teleportSpeed"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("useTeleportMark"));
				if (systemToUse.FindProperty ("useTeleportMark").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("teleportMark"));
				}
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("stopTeleportIfMoving"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("changeCameraFovOnTeleport"));
				if (systemToUse.FindProperty ("changeCameraFovOnTeleport").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("cameraFovOnTeleport"));
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("cameraFovOnTeleportSpeed"));
				}
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("addForceIfTeleportStops"));
				if (systemToUse.FindProperty ("addForceIfTeleportStops").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("forceIfTeleportStops"));
				}
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("canTeleportOnZeroGravity"));	
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		
			GUILayout.BeginVertical ("Power Elements", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.cursor"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.runMat"), new GUIContent ("Run Material"), false);
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.slider"), new GUIContent ("Throw Objects Slider"), false);
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.powerBar"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.highFrictionMaterial"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("settings.shield"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("armorSurfaceManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerLaserGameObject"), new GUIContent ("Player Laser"), false);
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("pauseManager"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Grab Objects Settings", "window");
			showLowerList (systemToUse.FindProperty ("settings.ableToGrabTags"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("carryObjectsTransform"));

			EditorGUILayout.Space ();

			showLowerList (systemToUse.FindProperty ("carryObjectsTransformList"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Rotation Point Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("usePowerRotationPoint"));
			if (systemToUse.FindProperty ("usePowerRotationPoint").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("powerRotationPoint"));

				EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.rotationUpPointAmountMultiplier"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.rotationDownPointAmountMultiplier"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.rotationPointSpeed"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.useRotationUpClamp"));
				if (systemToUse.FindProperty ("rotationPointInfo.useRotationUpClamp").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.rotationUpClampAmount"));
				}
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.useRotationDownClamp"));
				if (systemToUse.FindProperty ("rotationPointInfo.useRotationDownClamp").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("rotationPointInfo.rotationDownClamp"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Mode Active Event Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useEventsOnStateChange"));
			if (systemToUse.FindProperty ("useEventsOnStateChange").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("evenOnStateEnabled"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("eventOnStateDisabled"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerCameraGameObject"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootZoneAudioSource"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("mainCameraTransform"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("pivotCameraTransform"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("mainCamera"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("upperBodyRotationManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("IKSystemManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerCameraManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("scannerSystemManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("headBobManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("weaponsManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("gravityManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("grabObjectsManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerScreenObjectivesManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("headTrackManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("impactDecalManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("laserPlayerManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("timeBulletManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("input"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("powersManager"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerCollider"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("cursorRectTransform"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("carryObjectsTransformAnimation"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("parable"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("playerRenderer"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (aimSettings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Aim Settings for bones using powers and weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Hands Settings", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("aimsettings.aimSide"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("aimsettings.leftHand"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("aimsettings.rightHand"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();
		}

		if (shootSettings) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shoot Settings for every power", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers List", "window");
			showUpperList (systemToUse.FindProperty ("shootsettings.powersList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Head Look Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("headLookWhenAiming"));
			if (systemToUse.FindProperty ("headLookWhenAiming").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("headLookSpeed"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("headLookTarget"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.autoShootOnTag"));
			if (systemToUse.FindProperty ("shootsettings.autoShootOnTag").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.layerToAutoShoot"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.maxDistanceToRaycast"));
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.shootAtLayerToo"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Auto Shoot Tag List", "window", GUILayout.Height (30));
				showLowerList (systemToUse.FindProperty ("shootsettings.autoShootTagList"));
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Aim Assist Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useAimAssistInThirdPerson"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useMaxDistanceToCameraCenterAimAssist"));
			if (systemToUse.FindProperty ("useMaxDistanceToCameraCenterAimAssist").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("maxDistanceToCameraCenterAimAssist"));
			}
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("useAimAssistInLockedCamera"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Amount Settings", "window");
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("infinitePower"));

			if (!systemToUse.FindProperty ("infinitePower").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.powerAmount"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Regenerate Power Settings", "window");
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("regeneratePower"));
				if (systemToUse.FindProperty ("regeneratePower").boolValue) {
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("constantRegenerate"));
					EditorGUILayout.PropertyField (systemToUse.FindProperty ("regenerateTime"));
					if (systemToUse.FindProperty ("constantRegenerate").boolValue) {
						EditorGUILayout.PropertyField (systemToUse.FindProperty ("regenerateSpeed"));
					}
					if (!systemToUse.FindProperty ("constantRegenerate").boolValue) {
						EditorGUILayout.PropertyField (systemToUse.FindProperty ("regenerateAmount"));
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.targetToDamageLayer"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("targetForScorchLayer"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.powersSlotsAmount"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.powerUsedByShield"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Elements", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.selectedPowerIcon"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.selectedPowerHud"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.shootZone"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.firstPersonShootPosition"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Touch Change Powers Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.touchZoneSize"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.minSwipeDist"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.touching"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.hudZone"));
			EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.showGizmo"));
			if (systemToUse.FindProperty ("shootsettings.showGizmo").boolValue) {
				EditorGUILayout.PropertyField (systemToUse.FindProperty ("shootsettings.gizmoColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (systemToUse.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUI.backgroundColor = buttonColor;
		if (GUI.changed) {
			systemToUse.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Power Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("texture"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("powerEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPowerNeeded"));
		GUILayout.EndVertical ();		

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Recoil Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRecoil"));
		if (list.FindPropertyRelative ("useRecoil").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilAmount"));
		}
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fire Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFireRate"));
		if (list.FindPropertyRelative ("useFireRate").boolValue || list.FindPropertyRelative ("automatic").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBurst"));
			if (list.FindPropertyRelative ("useBurst").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("burstAmount"));
			}
		}
		GUILayout.EndVertical ();		

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));
		if (list.FindPropertyRelative ("shootAProjectile").boolValue || list.FindPropertyRelative ("launchProjectile").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectile"));
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
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));
				if (!list.FindPropertyRelative ("useRayCastShoot").boolValue) {
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
				showLowerList (list.FindPropertyRelative ("tagToLocate"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("homingProjectilesMaxAmount"));
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
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
			GUILayout.EndVertical ();
	
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particles Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		} 
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToCall"));
		if (list.FindPropertyRelative ("useEventToCall").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ability Power Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("powerWithAbility"));
		if (list.FindPropertyRelative ("powerWithAbility").boolValue) {

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

		GUILayout.BeginVertical ("Secondary Action Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryAction"));
		if (list.FindPropertyRelative ("useSecondaryAction").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryAction"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Spread Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
		if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spreadAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch Settings", "window", GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch from Decal Manager", "window", GUILayout.Height (30));
		if (systemToUse.FindProperty ("impactDecalList").arraySize > 0) {
			list.FindPropertyRelative ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
				list.FindPropertyRelative ("impactDecalIndex").intValue, powersManager.impactDecalList);
			list.FindPropertyRelative ("impactDecalName").stringValue = powersManager.impactDecalList [list.FindPropertyRelative ("impactDecalIndex").intValue];
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Decal Impact List")) {
			powersManager.getImpactListInfo ();					
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
			EditorGUILayout.HelpBox ("Shake Settings when this power is used", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShake"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sameValueBothViews"));
			if (list.FindPropertyRelative ("useShake").boolValue) {

				EditorGUILayout.Space ();

				if (list.FindPropertyRelative ("sameValueBothViews").boolValue) {
					showShakeInfo (list.FindPropertyRelative ("thirdPersonShakeInfo"), false, "Shake In Third Person");
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeInThirdPerson"));
					if (list.FindPropertyRelative ("useShakeInThirdPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("thirdPersonShakeInfo"), false, "Shake In Third Person");

						EditorGUILayout.Space ();

					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeInFirstPerson"));
					if (list.FindPropertyRelative ("useShakeInFirstPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("firstPersonShakeInfo"), true, "Shake In First Person");

						EditorGUILayout.Space ();
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showShakeInfo (SerializedProperty list, bool isFirstPerson, string shakeName)
	{
		GUILayout.BeginVertical (shakeName, "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shotForce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		if (isFirstPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of powers: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Power")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Powers")) {
				for (int i = 0; i < list.arraySize; i++) {
					powersManager.enableOrDisableAllPowers (true);
				}
			}
			if (GUILayout.Button ("Disable All Powers")) {
				for (int i = 0; i < list.arraySize; i++) {
					powersManager.enableOrDisableAllPowers (false);
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

					powerEnabled = list.GetArrayElementAtIndex (i).FindPropertyRelative ("powerEnabled").boolValue;
					powerEnabledString = " +";
					if (!powerEnabled) {
						powerEnabledString = " -";
					}
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + powerEnabledString));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i), true);
						expanded = true;
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
			if (GUILayout.Button ("New")) {
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
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif
