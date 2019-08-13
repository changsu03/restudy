using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(vehicleHUDManager))]
public class vehicleHUDManagerEditor : Editor
{
	SerializedObject list;
	vehicleHUDManager vehicleHUD;
	GUIStyle style = new GUIStyle ();
	bool useWeakSpots;
	bool advancedSettings;
	bool eventSettings;
	Color defBackgroundColor;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		vehicleHUD = (vehicleHUDManager)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (vehicleHUD.advancedSettings.showGizmo) {
				style.normal.textColor = vehicleHUD.advancedSettings.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				for (int i = 0; i < vehicleHUD.advancedSettings.damageReceiverList.Count; i++) {
					if (vehicleHUD.advancedSettings.damageReceiverList [i].spotTransform) {
						string label = vehicleHUD.advancedSettings.damageReceiverList [i].name;
						if (vehicleHUD.advancedSettings.damageReceiverList [i].killedWithOneShoot) {
							if (vehicleHUD.advancedSettings.damageReceiverList [i].needMinValueToBeKilled) {
								label += "\nOne Shoot\n >=" + vehicleHUD.advancedSettings.damageReceiverList [i].minValueToBeKilled;
							} else {
								label += "\nOne Shoot";	
							}
						} else {
							label += "\nx" + vehicleHUD.advancedSettings.damageReceiverList [i].damageMultiplier;
						}

						Handles.Label (vehicleHUD.advancedSettings.damageReceiverList [i].spotTransform.position, label, style);	
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Health Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("invincible"));

		if (!list.FindProperty ("invincible").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("healthAmount"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regenerate Health Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("regenerateHealth"));
			if (list.FindProperty ("regenerateHealth").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("constantHealthRegenerate"));
				EditorGUILayout.PropertyField (list.FindProperty ("regenerateHealthTime"));
				if (list.FindProperty ("constantHealthRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateHealthSpeed"));
				}
				if (!list.FindProperty ("constantHealthRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateHealthAmount"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("destroyed"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Energy Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("infiniteBoost"));

		if (!list.FindProperty ("infiniteBoost").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("boostAmount"));
			EditorGUILayout.PropertyField (list.FindProperty ("boostUseRate"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regenerate Boost Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("regenerateBoost"));
			if (list.FindProperty ("regenerateBoost").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("constantBoostRegenerate"));
				EditorGUILayout.PropertyField (list.FindProperty ("regenerateBoostTime"));
				if (list.FindProperty ("constantBoostRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateBoostSpeed"));
				}
				if (!list.FindProperty ("constantBoostRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateBoostAmount"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fuel Settings", "window");

		EditorGUILayout.PropertyField (list.FindProperty ("vehicleUseFuel"));

		if (list.FindProperty ("vehicleUseFuel").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("infiniteFuel"));

			if (!list.FindProperty ("infiniteFuel").boolValue) {

				EditorGUILayout.PropertyField (list.FindProperty ("fuelAmount"));
				EditorGUILayout.PropertyField (list.FindProperty ("fuelUseRate"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Regenerate Fuel Settings", "window");
				EditorGUILayout.PropertyField (list.FindProperty ("regenerateFuel"));
				if (list.FindProperty ("regenerateFuel").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("constantFuelRegenerate"));
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateFuelTime"));
					if (list.FindProperty ("constantFuelRegenerate").boolValue) {
						EditorGUILayout.PropertyField (list.FindProperty ("regenerateFuelSpeed"));
					}
					if (!list.FindProperty ("constantFuelRegenerate").boolValue) {
						EditorGUILayout.PropertyField (list.FindProperty ("regenerateFuelAmount"));
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindProperty ("gasTankGameObject"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleName"));
		EditorGUILayout.PropertyField (list.FindProperty ("destroyedSound"));
		EditorGUILayout.PropertyField (list.FindProperty ("destroyedSource"));
		EditorGUILayout.PropertyField (list.FindProperty ("damageParticles"));
		EditorGUILayout.PropertyField (list.FindProperty ("destroyedParticles"));
		EditorGUILayout.PropertyField (list.FindProperty ("healthPercentageDamageParticles"));
		EditorGUILayout.PropertyField (list.FindProperty ("extraGrabDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("placeToShoot"));
		EditorGUILayout.PropertyField (list.FindProperty ("timeToFadePieces"));
		EditorGUILayout.PropertyField (list.FindProperty ("destroyedMeshShader"));
		EditorGUILayout.PropertyField (list.FindProperty ("canSetTurnOnState"));
		EditorGUILayout.PropertyField (list.FindProperty ("autoTurnOnWhenGetOn"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("layer"));
		EditorGUILayout.PropertyField (list.FindProperty ("layerForPassengers"));
		EditorGUILayout.PropertyField (list.FindProperty ("passengersParent"));
		EditorGUILayout.PropertyField (list.FindProperty ("canUnlockCursor"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("IKDrivingManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainRigidbody"));
		EditorGUILayout.PropertyField (list.FindProperty ("damageInScreenManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mapInformationManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleCameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("gasTankManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleGravitymanager"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleInterfaceManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Horn/Friends Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useHornToCallFriends"));
		if (list.FindProperty ("useHornToCallFriends").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("callOnlyFoundFriends"));
			if (!list.FindProperty ("callOnlyFoundFriends").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("radiusToCallFriends"));
			}
		}
		EditorGUILayout.PropertyField (list.FindProperty ("useHornEvent"));
		if (list.FindProperty ("useHornEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("hornEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Self Destruction Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("canUseSelfDestruct"));
		if (list.FindProperty ("canUseSelfDestruct").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("canStopSelfDestruction"));
			EditorGUILayout.PropertyField (list.FindProperty ("selfDestructDelay"));
			EditorGUILayout.PropertyField (list.FindProperty ("ejectPassengerOnSelfDestruct"));	
			if (list.FindProperty ("ejectPassengerOnSelfDestruct").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("ejectPassengerFoce"));	
			}
			if (!list.FindProperty ("ejectPassengerOnSelfDestruct").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("getOffPassengersOnSelfDestruct"));	
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Eject Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("canEjectFromVehicle"));
		if (list.FindProperty ("canEjectFromVehicle").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("ejectPassengerFoce"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Collision Damage Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("damageObjectsOnCollision"));
		EditorGUILayout.PropertyField (list.FindProperty ("damageMultiplierOnCollision"));
		EditorGUILayout.PropertyField (list.FindProperty ("minVelocityToDamage"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("HUD Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showVehicleHUD"));	
		if (list.FindProperty ("showVehicleHUD").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("showVehicleSpeed"));		
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Launch Driver On Collision Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("launchDriverOnCollision"));
		if (list.FindProperty ("launchDriverOnCollision").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minCollisionVelocityToLaunch"));
			EditorGUILayout.PropertyField (list.FindProperty ("launchDirectionOffset"));
			EditorGUILayout.PropertyField (list.FindProperty ("extraCollisionForce"));
			EditorGUILayout.PropertyField (list.FindProperty ("ignoreRagdollCollider"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Collision Damage Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("applyDamageToDriver"));
			if (list.FindProperty ("applyDamageToDriver").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("useCollisionVelocityAsDamage"));
				if (!list.FindProperty ("useCollisionVelocityAsDamage").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("collisionDamageAmount"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Explosion Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleExplosionForce"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleExplosionRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleExplosionForceMode"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage On Collision Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("receiveDamageFromCollision"));
		if (list.FindProperty ("receiveDamageFromCollision").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minVelocityToDamageCollision"));
			EditorGUILayout.PropertyField (list.FindProperty ("useCurrentVelocityAsDamage"));
			if (!list.FindProperty ("useCurrentVelocityAsDamage").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("defaultDamageCollision"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("collisionDamageMultiplier"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Audio Source List", "window", GUILayout.Height (30));
		showAudioSourceList (list.FindProperty ("audioSourceList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (eventSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Show Event Settings")) {
			eventSettings = !eventSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (eventSettings) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useEventsOnStateChanged"));
			if (list.FindProperty ("useEventsOnStateChanged").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnGetOn"));
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnGetOff"));
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnDestroyed"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("useJumpPlatformEvents"));
			if (list.FindProperty ("useJumpPlatformEvents").boolValue) {
				GUILayout.BeginVertical ("Jump Platforms Events", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindProperty ("jumpPlatformEvent"));
				EditorGUILayout.PropertyField (list.FindProperty ("jumpPlatformParableEvent"));
				EditorGUILayout.PropertyField (list.FindProperty ("setNewJumpPowerEvent"));
				EditorGUILayout.PropertyField (list.FindProperty ("setOriginalJumpPowerEvent"));
				GUILayout.EndVertical ();
			}
				
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Driving State Events", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("passengerGettingOnOffEvent"));
			EditorGUILayout.PropertyField (list.FindProperty ("changeVehicleStateEvent"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("AI Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		advancedSettings = list.FindProperty ("showAdvancedSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (advancedSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Advanced Settings")) {
			advancedSettings = !advancedSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindProperty ("showAdvancedSettings").boolValue = advancedSettings;

		if (advancedSettings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Check all the damage receivers in this vehicle", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.showGizmo"));
			if (list.FindProperty ("advancedSettings.showGizmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.gizmoLabelColor"));
				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.gizmoRadius"));
				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.alphaColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Receiver List", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useWeakSpots"));
			useWeakSpots = list.FindProperty ("useWeakSpots").boolValue;

			EditorGUILayout.Space ();

			showDamageReceivers (list.FindProperty ("advancedSettings.damageReceiverList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Vehicle Parts")) {
				vehicleHUD.setVehicleParts ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Impact Surface Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useImpactSurface"));
			if (list.FindProperty ("useImpactSurface").boolValue) {
				if (list.FindProperty ("impactDecalList").arraySize > 0) {
					list.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Decal Impact Type", 
						list.FindProperty ("impactDecalIndex").intValue, vehicleHUD.impactDecalList);
					list.FindProperty ("impactDecalName").stringValue = vehicleHUD.impactDecalList [list.FindProperty ("impactDecalIndex").intValue];
				}

				EditorGUILayout.PropertyField (list.FindProperty ("getImpactListEveryFrame"));
				if (!list.FindProperty ("getImpactListEveryFrame").boolValue) {
					if (GUILayout.Button ("Update Decal Impact List")) {
						vehicleHUD.getImpactListInfo ();					
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Destroy Vehicle (Only Ingame)")) {
			if (Application.isPlaying) {
				vehicleHUD.killByButton ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle State", "window");
		GUILayout.Label ("Is Being Driven\t" + list.FindProperty ("isBeingDriven").boolValue.ToString ());
		GUILayout.Label ("Passengers Inside\t" + list.FindProperty ("passengersOnVehicle").boolValue.ToString ());
		//		EditorGUILayout.PropertyField (list.FindProperty ("driving"));
		//		EditorGUILayout.PropertyField (list.FindProperty ("passengersOnVehicle"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		GUI.backgroundColor = defBackgroundColor;
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("spotTransform"));
		if (!useWeakSpots || !list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplier"));
		}
		if (useWeakSpots) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killedWithOneShoot"));
			if (list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("needMinValueToBeKilled"));
				if (list.FindPropertyRelative ("needMinValueToBeKilled").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("minValueToBeKilled"));
				}
			}
		} 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Function When Damage Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDie"));
		if (list.FindPropertyRelative ("sendFunctionWhenDamage").boolValue || list.FindPropertyRelative ("sendFunctionWhenDie").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageFunction"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Health Amount On Spot Settings", "window", GUILayout.Height (30));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHealthAmountOnSpot"));
		if (list.FindPropertyRelative ("useHealthAmountOnSpot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("healhtAmountOnSpot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEmtpyHealthAmountOnSpot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killCharacterOnEmtpyHealthAmountOnSpot"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showDamageReceivers (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weak Spots: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Get List")) {
				vehicleHUD.getAllDamageReceivers ();
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

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showAudioSourceElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));
		GUILayout.EndVertical ();
	}

	void showAudioSourceList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Audios: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Audio")) {
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

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showAudioSourceElement (list.GetArrayElementAtIndex (i));
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
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}
}
#endif