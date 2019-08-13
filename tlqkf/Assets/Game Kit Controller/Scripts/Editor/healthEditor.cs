using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(health))]
[CanEditMultipleObjects]
public class healthEditor : Editor
{
	SerializedObject list;
	health healthManager;
	GUIStyle style = new GUIStyle ();

	Color defBackgroundColor;
	string label;
	string isDead;
	string sedateActive;

	bool showSettings;
	bool showAdvancedSettings;
	bool showDamageDeadSettings;

	bool useHealthAmountOnSpotEnabled;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		healthManager = (health)target;
	}

	void OnSceneGUI ()
	{   
		if (healthManager.advancedSettings.showGizmo) {
			style.normal.textColor = healthManager.advancedSettings.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < healthManager.advancedSettings.weakSpots.Count; i++) {
				if (healthManager.advancedSettings.weakSpots [i].spotTransform) {
					label = healthManager.advancedSettings.weakSpots [i].name;
					if (healthManager.advancedSettings.weakSpots [i].killedWithOneShoot) {
						if (healthManager.advancedSettings.weakSpots [i].needMinValueToBeKilled) {
							label += "\nOne Shoot\n >=" + healthManager.advancedSettings.weakSpots [i].minValueToBeKilled;
						} else {
							label += "\nOne Shoot";	
						}
					} else {
						label += "\nx" + healthManager.advancedSettings.weakSpots [i].damageMultiplier;
					}

					if (healthManager.advancedSettings.weakSpots [i].useHealthAmountOnSpot) {
						label += "\n" + healthManager.advancedSettings.weakSpots [i].healhtAmountOnSpot.ToString ();
					}

					Handles.Label (healthManager.advancedSettings.weakSpots [i].spotTransform.position, label, style);	
				}
			}
			if (healthManager.settings.enemyHealthSlider) {
				style.normal.textColor = healthManager.advancedSettings.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				Handles.Label (healthManager.transform.position + healthManager.settings.sliderOffset, "Health Slider", style);	
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

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Health Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("invincible"));

		if (!list.FindProperty ("invincible").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("healthAmount"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regenerate Health Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("regenerateHealth"));
			if (list.FindProperty ("regenerateHealth").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("constantRegenerate"));
				EditorGUILayout.PropertyField (list.FindProperty ("regenerateTime"));
				if (list.FindProperty ("constantRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateSpeed"));
				}
				if (!list.FindProperty ("constantRegenerate").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("regenerateAmount"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health State", "window");
			GUILayout.Label ("Healht ID\t\t " + list.FindProperty ("currentID").intValue.ToString());
			isDead = "NO";
			if (Application.isPlaying) {
				if (list.FindProperty ("dead").boolValue) {
					isDead = "YES";
				} 
			} 
			GUILayout.Label ("Is Dead\t\t " + isDead);

			sedateActive = "NO";
			if (Application.isPlaying) {
				if (list.FindProperty ("sedateActive").boolValue) {
					sedateActive = "YES";
				} 
			} 
			GUILayout.Label ("Is Sedated\t\t " + sedateActive);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("placeToShootActive"));
		if (list.FindProperty ("placeToShootActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("placeToShoot"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("damagePrefab"));
		EditorGUILayout.PropertyField (list.FindProperty ("scorchMarkPrefab"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("ragdollManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("damageInScreenManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		showDamageDeadSettings = list.FindProperty ("showDamageDeadSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showDamageDeadSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Show Damage/Death Settings")) {
			showDamageDeadSettings = !showDamageDeadSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindProperty ("showDamageDeadSettings").boolValue = showDamageDeadSettings;

		if (showDamageDeadSettings) {

			GUILayout.BeginVertical ("Damage Function Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("damageFunctionCall"));
			EditorGUILayout.PropertyField (list.FindProperty ("damageFunction"));
					
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Extra Damage Function List", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useExtraDamageFunctions"));
			if (list.FindProperty ("useExtraDamageFunctions").boolValue) {

				EditorGUILayout.Space ();

				extraDamageFunctionList (list.FindProperty ("extraDamageFunctionList"));
			}
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Death Function Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("deadFuncionCall"));
			EditorGUILayout.PropertyField (list.FindProperty ("deadFuncion"));
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Extra Dead Function List", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useExtraDeadFunctions"));
			if (list.FindProperty ("useExtraDeadFunctions").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("delayInExtraDeadFunctions"));

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindProperty ("extraDeadFunctionCall"));
			}
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Resurrect Function Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("resurrectFunctionCall"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Surface Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useImpactSurface"));
		if (list.FindProperty ("useImpactSurface").boolValue) {
			if (list.FindProperty ("impactDecalList").arraySize > 0) {
				list.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Decal Impact Type", 
					list.FindProperty ("impactDecalIndex").intValue, healthManager.impactDecalList);
				list.FindProperty ("impactDecalName").stringValue = healthManager.impactDecalList [list.FindProperty ("impactDecalIndex").intValue];
			}

			EditorGUILayout.PropertyField (list.FindProperty ("getImpactListEveryFrame"));
			if (!list.FindProperty ("getImpactListEveryFrame").boolValue) {
				if (GUILayout.Button ("Update Decal Impact List")) {
					healthManager.getImpactListInfo ();					
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		showSettings = list.FindProperty ("showSettings").boolValue;
		showAdvancedSettings = list.FindProperty ("showAdvancedSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Settings")) {
			showSettings = !showSettings;
		}
		if (showAdvancedSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Advanced Settings")) {
			showAdvancedSettings = !showAdvancedSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindProperty ("showSettings").boolValue = showSettings;
		list.FindProperty ("showAdvancedSettings").boolValue = showAdvancedSettings;

		if (showSettings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Player Settings", MessageType.None);
			GUI.color = Color.white;
			EditorGUILayout.PropertyField (list.FindProperty ("healthSlider"));

			if (!list.FindProperty ("healthSlider").objectReferenceValue) {
				
				EditorGUILayout.Space ();

				GUI.color = Color.cyan;
				EditorGUILayout.HelpBox ("Enemy/Friend Settings", MessageType.None);
				GUI.color = Color.white;

				EditorGUILayout.Space ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Health Slider Settings", "window");
				EditorGUILayout.PropertyField (list.FindProperty ("settings.useHealthSlider"));
				if (list.FindProperty ("settings.useHealthSlider").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("settings.enemyHealthSlider"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.sliderOffset"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.layer"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.enemyName"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.enemySliderColor"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.allyName"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.allySliderColor"));
					EditorGUILayout.PropertyField (list.FindProperty ("settings.nameTextColor"));
				
					EditorGUILayout.PropertyField (list.FindProperty ("enemyTag"));
					EditorGUILayout.PropertyField (list.FindProperty ("friendTag"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showAdvancedSettings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Ragdoll and weak spots Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Advanced Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.notHuman"));
			EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.useWeakSpots"));
			EditorGUILayout.PropertyField (list.FindProperty ("showWeakSpotsInScannerMode"));

			EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.useHealthAmountOnSpotEnabled"));
			useHealthAmountOnSpotEnabled = list.FindProperty ("advancedSettings.useHealthAmountOnSpotEnabled").boolValue;

			if (list.FindProperty ("showWeakSpotsInScannerMode").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("weakSpotMesh"));
				EditorGUILayout.PropertyField (list.FindProperty ("weakSpotMeshAlphaValue"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ragdoll Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.haveRagdoll"));
			if (list.FindProperty ("advancedSettings.haveRagdoll").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.activateRagdollOnDamageReceived"));
				if (list.FindProperty ("advancedSettings.activateRagdollOnDamageReceived").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.minDamageToEnableRagdoll"));
					EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.ragdollEvent"));

					EditorGUILayout.Space ();

				}

				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.allowPushCharacterOnExplosions"));

				EditorGUILayout.PropertyField (list.FindProperty ("advancedSettings.ragdollCanReceiveDamageOnImpact"));

				EditorGUILayout.PropertyField (list.FindProperty ("receiveDamageEvenDead"));

				EditorGUILayout.PropertyField (list.FindProperty ("canBeSedated"));
				if (list.FindProperty ("canBeSedated").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("awakeOnDamageIfSedated"));

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Ragdoll Settings", "window");
					EditorGUILayout.PropertyField (list.FindProperty ("useEventOnSedate"));
					if (list.FindProperty ("useEventOnSedate").boolValue) {
						EditorGUILayout.PropertyField (list.FindProperty ("sedateStartEvent"));
						EditorGUILayout.PropertyField (list.FindProperty ("sedateEndEvent"));
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Damage Receivers Settings", "window");
				if (GUILayout.Button ("Add Damager Receivers To Ragdoll")) {
					healthManager.addDamageReceiversToRagdoll ();
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Remove Damage Receivers From Ragdoll")) {
					healthManager.removeDamageReceiversFromRagdoll ();
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();

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

			if (GUILayout.Button ("Set Humanoid Weak Spots")) {
				healthManager.setHumanoidWeaKSpots ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weak Spots List", "window");
			showWeakSpotsList (list.FindProperty ("advancedSettings.weakSpots"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Damage Receivers")) {
				healthManager.updateDamageReceivers ();
			}
		
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Kill Character (Only Ingame)")) {
			if (Application.isPlaying) {
				healthManager.killByButton ();
			}
		}

		if (list.FindProperty ("advancedSettings.haveRagdoll").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Push Character (Only Ingame)")) {
				if (Application.isPlaying) {
					healthManager.gameObject.SendMessage ("pushFullCharacter", 
						((healthManager.gameObject.transform.forward + healthManager.gameObject.transform.up) / 2), SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		//EditorGUILayout.PropertyField(list.FindProperty("auxHealthAmount"));
		EditorGUILayout.Space ();
		GUI.backgroundColor = defBackgroundColor;
		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list, int elementIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("spotTransform"));
		if (!list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplier"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("killedWithOneShoot"));
		if (list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("needMinValueToBeKilled"));
			if (list.FindPropertyRelative ("needMinValueToBeKilled").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minValueToBeKilled"));
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

		if (useHealthAmountOnSpotEnabled) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health Amount On Spot Settings", "window", GUILayout.Height (30));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHealthAmountOnSpot"));
			if (list.FindPropertyRelative ("useHealthAmountOnSpot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("healhtAmountOnSpot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEmtpyHealthAmountOnSpot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("killCharacterOnEmtpyHealthAmountOnSpot"));

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set Full Damage Spot")) {
					if (Application.isPlaying) {
						healthManager.damageSpot (elementIndex, list.FindPropertyRelative ("healhtAmountOnSpot").floatValue);
					}
				}
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeakSpotsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("Number Of Spots: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Spot")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

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
						showListElementInfo (list.GetArrayElementAtIndex (i), i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void extraDamageFunctionList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Functions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Function")) {
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
						showFunctionInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showFunctionInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageRecived"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageFunctionCall"));
		GUILayout.EndVertical ();
	}
}
#endif