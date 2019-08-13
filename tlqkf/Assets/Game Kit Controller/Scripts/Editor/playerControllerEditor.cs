using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerController))]
public class playerControllerEditor : Editor
{
	SerializedObject list;
	playerController manager;
	GUIStyle style = new GUIStyle ();
	bool showAdvancedSettings;
	Color buttonColor;
	string showAdvancedSettingsString;
	bool showPlayerStates;
	string showPlayerStatesString;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (playerController)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			if (manager.useAutoCrouch) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				Handles.Label (manager.autoCrouchRayPosition.position, "Auto Crouch \n raycast", style);	
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

		GUILayout.BeginVertical ("Movement Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("jumpPower"));
		EditorGUILayout.PropertyField (list.FindProperty ("airSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("airControl"));
		EditorGUILayout.PropertyField (list.FindProperty ("stationaryTurnSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("movingTurnSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("autoTurnSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("aimTurnSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("thresholdAngleDifference"));
		EditorGUILayout.PropertyField (list.FindProperty ("capsuleHeightOnCrouch"));
		EditorGUILayout.PropertyField (list.FindProperty ("regularMovementOnBulletTime"));	
		EditorGUILayout.PropertyField (list.FindProperty ("baseLayerIndex"));	
		EditorGUILayout.PropertyField (list.FindProperty ("inputLerpSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("animSpeedMultiplier"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Walk/Run/Sprint Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("walkSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("increaseWalkSpeedEnabled"));
		if (list.FindProperty ("increaseWalkSpeedEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("increaseWalkSpeedValue"));
			EditorGUILayout.PropertyField (list.FindProperty ("holdButtonToKeepIncreasedWalkSpeed"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("sprintEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("changeCameraFovOnSprint"));
		EditorGUILayout.PropertyField (list.FindProperty ("shakeCameraOnSprintThirdPerson"));
		EditorGUILayout.PropertyField (list.FindProperty ("sprintThirdPersonCameraShakeName"));

		EditorGUILayout.PropertyField (list.FindProperty ("useSecondarySprintValues"));
		if (list.FindProperty ("useSecondarySprintValues").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("sprintVelocity"));
			EditorGUILayout.PropertyField (list.FindProperty ("sprintJumpPower"));
			EditorGUILayout.PropertyField (list.FindProperty ("sprintAirSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("sprintAirControl"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ground Adherence Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("regularGroundAdherence"));	
		EditorGUILayout.PropertyField (list.FindProperty ("slopesGroundAdherence"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxRayDistanceRange"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxSlopeRayDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxStairsRayDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("useMaxWalkSurfaceAngle"));	
		EditorGUILayout.PropertyField (list.FindProperty ("maxWalkSurfaceAngle"));	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("First Person No Animator Settings", "window");
//		EditorGUILayout.PropertyField (list.FindProperty ("usingAnimatorInFirstMode"));	
//		if (!list.FindProperty ("usingAnimatorInFirstMode").boolValue) {
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorSpeed"));	
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorWalkMovementSpeed"));	
		if (list.FindProperty ("noAnimatorCanRun").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorRunMovementSpeed"));	
		}
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorCrouchMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorRunCrouchMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorStrafeMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorCanRun"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorWalkBackwardMovementSpeed"));	
		if (list.FindProperty ("noAnimatorCanRunBackwards").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorRunBackwardMovementSpeed"));	
		}
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorCrouchBackwardMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorRunCrouchBackwardMovementSpeed"));	
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorStrafeBackwardMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorCanRunBackwards"));	
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorAirSpeed"));	
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorSlopesGroundAdherence"));
		EditorGUILayout.PropertyField (list.FindProperty ("noAnimatorStairsGroundAdherence"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxVelocityChange"));

		EditorGUILayout.Space ();
		GUILayout.Label ("Current Speed\t" + list.FindProperty ("noAnimatorCurrentMovementSpeed").floatValue.ToString ());
		GUILayout.Label ("Current Float Speed\t" + list.FindProperty ("currentVelocityChangeMagnitude").floatValue.ToString ());
		GUILayout.Label ("Character Velocity\t" + list.FindProperty ("characterVelocity").vector3Value.ToString ());

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Root Motion Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("useRootMotionActive"));
		if (!list.FindProperty ("useRootMotionActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("noRootLerpSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootWalkMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootRunMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootSprintMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootCrouchMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootWalkStrafeMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("noRootRunStrafeMovementSpeed"));
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		showAdvancedSettings = list.FindProperty ("showAdvancedSettings").boolValue;

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showAdvancedSettings) {
			GUI.backgroundColor = Color.gray;
			showAdvancedSettingsString = "Hide Advanced Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			showAdvancedSettingsString = "Show Advanced Settings";
		}
		if (GUILayout.Button (showAdvancedSettingsString)) {
			showAdvancedSettings = !showAdvancedSettings;
		}

		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		list.FindProperty ("showAdvancedSettings").boolValue = showAdvancedSettings;

		if (showAdvancedSettings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Look In Camera Direction Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("lookAlwaysInCameraDirection"));	
			EditorGUILayout.PropertyField (list.FindProperty ("lookInCameraDirectionIfLookingAtTarget"));	
			if (list.FindProperty ("lookAlwaysInCameraDirection").boolValue || list.FindProperty ("lookInCameraDirectionIfLookingAtTarget").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("lookOnlyIfMoving"));
			}

			EditorGUILayout.PropertyField (list.FindProperty ("defaultStrafeWalkSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("defaultStrafeRunSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("rotateDirectlyTowardCameraOnStrafe"));
			EditorGUILayout.PropertyField (list.FindProperty ("strafeLerpSpeed"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Character Mesh Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("characterMeshGameObject"));	

			EditorGUILayout.Space ();

			showSimpleList (list.FindProperty ("extraCharacterMeshGameObject"));	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Move While Aim Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveWhileAimFirstPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveWhileAimThirdPerson"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Stairs Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("stairsMinValue"));
			EditorGUILayout.PropertyField (list.FindProperty ("stairsMaxValue"));
			EditorGUILayout.PropertyField (list.FindProperty ("stairsGroundAdherence"));	

			EditorGUILayout.PropertyField (list.FindProperty ("checkStairsWithInclination"));
			if (list.FindProperty ("checkStairsWithInclination").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("minStairInclination"));
				EditorGUILayout.PropertyField (list.FindProperty ("maxStairInclination"));	
			}

			EditorGUILayout.PropertyField (list.FindProperty ("checkForStairAdherenceSystem"));
			if (list.FindProperty ("checkForStairAdherenceSystem").boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Current Stairs Adherence (DEBUG)", "window");
				EditorGUILayout.PropertyField (list.FindProperty ("currentStairAdherenceSystemMinValue"));	
				EditorGUILayout.PropertyField (list.FindProperty ("currentStairAdherenceSystemMaxValue"));
				EditorGUILayout.PropertyField (list.FindProperty ("currentStairAdherenceSystemAdherenceValue"));
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Locked Camera Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("lockedPlayerMovement"));
			if (list.FindProperty ("lockedPlayerMovement").enumValueIndex == 0) {
				EditorGUILayout.PropertyField (list.FindProperty ("tankModeRotationSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("canMoveWhileAimLockedCamera"));
			}

			EditorGUILayout.PropertyField (list.FindProperty ("crouchVerticalInput2_5dEnabled"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gravity Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("gravityMultiplier"));
			EditorGUILayout.PropertyField (list.FindProperty ("gravityForce"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Physics Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("zeroFrictionMaterial"));
			EditorGUILayout.PropertyField (list.FindProperty ("highFrictionMaterial"));
			EditorGUILayout.PropertyField (list.FindProperty ("layer"));
			EditorGUILayout.PropertyField (list.FindProperty ("rayDistance"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Jump Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("enabledRegularJump"));
			EditorGUILayout.PropertyField (list.FindProperty ("enabledDoubleJump"));
			if (list.FindProperty ("enabledDoubleJump").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("maxNumberJumpsInAir"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("holdJumpSlowDownFall"));
			if (list.FindProperty ("holdJumpSlowDownFall").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("slowDownGravityMultiplier"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Fall Damage Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("damageFallEnabled"));
			if (list.FindProperty ("damageFallEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("maxTimeInAirDamage"));
				EditorGUILayout.PropertyField (list.FindProperty ("fallindDamageMultiplier"));
				EditorGUILayout.PropertyField (list.FindProperty ("callEventOnFallDamage"));
				if (list.FindProperty ("callEventOnFallDamage").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("minDamageToCallFunction"));

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (list.FindProperty ("eventOnFallDamage"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Land Mark Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useLandMark"));
			if (list.FindProperty ("useLandMark").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("maxLandDistance"));
				EditorGUILayout.PropertyField (list.FindProperty ("minDistanceShowLandMark"));
				EditorGUILayout.PropertyField (list.FindProperty ("landMark"));
				EditorGUILayout.PropertyField (list.FindProperty ("landMark1"));
				EditorGUILayout.PropertyField (list.FindProperty ("landMark2"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Modes Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("canUseSphereMode"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("canGetOnVehicles"));
			EditorGUILayout.PropertyField (list.FindProperty ("canDrive"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Crouch Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useAutoCrouch"));
			if (list.FindProperty ("useAutoCrouch").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("layerToCrouch"));
				EditorGUILayout.PropertyField (list.FindProperty ("raycastDistanceToAutoCrouch"));
				EditorGUILayout.PropertyField (list.FindProperty ("autoCrouchRayPosition"));
				EditorGUILayout.PropertyField (list.FindProperty ("secondRaycastOffset"));

			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dash Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("airDashEnabled"));
			if (list.FindProperty ("airDashEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("airDashForce"));
				EditorGUILayout.PropertyField (list.FindProperty ("airDashColdDown"));
				EditorGUILayout.PropertyField (list.FindProperty ("pauseGravityForce"));
				if (list.FindProperty ("pauseGravityForce").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("gravityForcePausedTime"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("resetGravityForceOnDash"));
				EditorGUILayout.PropertyField (list.FindProperty ("useDashLimit"));
				if (list.FindProperty ("useDashLimit").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("dashLimit"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("changeCameraFovOnDash"));
				if (list.FindProperty ("changeCameraFovOnDash").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("cameraFovOnDash"));
					EditorGUILayout.PropertyField (list.FindProperty ("cameraFovOnDashSpeed"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Zero Gravity Mode Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("zeroGravityMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("zeroGravityControlSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("zeroGravityLookCameraSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("useGravityDirectionLandMark"));
			if (list.FindProperty ("useGravityDirectionLandMark").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("forwardSurfaceRayPosition"));
				EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToAdjust"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("pauseCheckOnGroundStateZG"));
			EditorGUILayout.PropertyField (list.FindProperty ("pushPlayerWhenZeroGravityModeIsEnabled"));
			if (list.FindProperty ("pushPlayerWhenZeroGravityModeIsEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("pushZeroGravityEnabledAmount"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveVerticallyOnZeroGravity"));
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveVerticallyAndHorizontalZG"));
			EditorGUILayout.PropertyField (list.FindProperty ("zeroGravitySpeedMultiplier"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Free Floating Mode Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("freeFloatingMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("freeFloatingControlSpeed"));

			EditorGUILayout.PropertyField (list.FindProperty ("pauseCheckOnGroundStateFF"));
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveVerticallyOnFreeFloating"));
			EditorGUILayout.PropertyField (list.FindProperty ("canMoveVerticallyAndHorizontalFF"));
			EditorGUILayout.PropertyField (list.FindProperty ("freeFloatingSpeedMultiplier"));
			EditorGUILayout.PropertyField (list.FindProperty ("pushFreeFloatingModeEnabledAmount"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Zero Gravity And Free Floating Mode Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useMaxAngleToCheckOnGroundStateZGFF"));
			if (list.FindProperty ("useMaxAngleToCheckOnGroundStateZGFF").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("maxAngleToChekOnGroundStateZGFF"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));

			EditorGUILayout.PropertyField (list.FindProperty ("headBobManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("healthManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("IKSystemManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("animator"));
			EditorGUILayout.PropertyField (list.FindProperty ("stepManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainRigidbody"));
			EditorGUILayout.PropertyField (list.FindProperty ("gravityManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("characterStateIconManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("capsule"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainCollider"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
	
			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
			if (list.FindProperty ("showGizmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoColor"));
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoRadius"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		showPlayerStates = list.FindProperty ("showPlayerStates").boolValue;

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showPlayerStates) {
			GUI.backgroundColor = Color.gray;
			showPlayerStatesString = "Hide Player States";
		} else {
			GUI.backgroundColor = buttonColor;
			showPlayerStatesString = "Show Player States";
		}
		if (GUILayout.Button (showPlayerStatesString)) {
			showPlayerStates = !showPlayerStates;
		}

		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		list.FindProperty ("showPlayerStates").boolValue = showPlayerStates;

		if (showPlayerStates) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player State", "window");

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Controller State", "window");
			GUILayout.Label ("On Ground\t\t" + list.FindProperty ("playerOnGround").boolValue.ToString ());
			GUILayout.Label ("Is moving\t\t" + list.FindProperty ("isMoving").boolValue.ToString ());
			GUILayout.Label ("Jumping\t\t" + list.FindProperty ("jump").boolValue.ToString ());
			GUILayout.Label ("Running\t\t" + list.FindProperty ("running").boolValue.ToString ());
			GUILayout.Label ("Crouching\t\t" + list.FindProperty ("crouching").boolValue.ToString ());
			GUILayout.Label ("Can Move\t\t" + list.FindProperty ("canMove").boolValue.ToString ());
			GUILayout.Label ("Slowing Fall\t" + list.FindProperty ("slowingFall").boolValue.ToString ());
			GUILayout.Label ("Is Dead \t\t" + list.FindProperty ("isDead").boolValue.ToString ());
			GUILayout.Label ("Child Of Parent \t" + list.FindProperty ("playerSetAsChildOfParent").boolValue.ToString ());
			GUILayout.Label ("Relative Movement \t" + list.FindProperty ("useRelativeMovementToLockedCamera").boolValue.ToString ());
			GUILayout.Label ("Stairs Found \t" + list.FindProperty ("stairAdherenceSystemDetected").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons And Powers State", "window");
			GUILayout.Label ("Aiming In 3rd Person\t" + list.FindProperty ("aimingInThirdPerson").boolValue.ToString ());
			GUILayout.Label ("Aiming In 1st Person\t" + list.FindProperty ("aimingInFirstPerson").boolValue.ToString ());
			GUILayout.Label ("Using Free Fire Mode\t" + list.FindProperty ("usingFreeFireMode").boolValue.ToString ());
			GUILayout.Label ("Look Camera Direction Free Fire Active\t" + list.FindProperty ("lookInCameraDirectionOnFreeFireActive").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Mode State", "window");
			GUILayout.Label ("Jet Pack Equiped\t" + list.FindProperty ("jetPackEquiped").boolValue.ToString ());
			GUILayout.Label ("Using Jet Pack\t" + list.FindProperty ("usingJetpack").boolValue.ToString ());
			GUILayout.Label ("Sphere Mode Active\t" + list.FindProperty ("sphereModeActive").boolValue.ToString ());
			GUILayout.Label ("Fly Mode Active\t" + list.FindProperty ("flyModeActive").boolValue.ToString ());

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Camera State", "window");
			GUILayout.Label ("Camera Locked\t" + list.FindProperty ("lockedCameraActive").boolValue.ToString ());
			GUILayout.Label ("Look Camera Direction Active\t" + list.FindProperty ("lookInCameraDirectionActive").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Other Elements State", "window");
			GUILayout.Label ("Using Device\t" + list.FindProperty ("usingDevice").boolValue.ToString ());
			GUILayout.Label ("Visible To AI\t" + list.FindProperty ("visibleToAI").boolValue.ToString ());
			GUILayout.Label ("NavMesh Enabled\t" + list.FindProperty ("playerNavMeshEnabled").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle State", "window");
			GUILayout.Label ("Is Driving\t\t" + list.FindProperty ("driving").boolValue.ToString ());
			GUILayout.Label ("Driving Remotely\t" + list.FindProperty ("drivingRemotely").boolValue.ToString ());
			GUILayout.Label ("Overriding Element\t" + list.FindProperty ("overridingElement").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gravity State", "window");
			GUILayout.Label ("Gravity Force Paused\t" + list.FindProperty ("gravityForcePaused").boolValue.ToString ());
			GUILayout.Label ("Zero Gravity On\t" + list.FindProperty ("zeroGravityModeOn").boolValue.ToString ());
			GUILayout.Label ("Free Floating On\t" + list.FindProperty ("freeFloatingModeOn").boolValue.ToString ());
			GUILayout.Label ("Current Normal\t" + list.FindProperty ("currentNormal").vector3Value.ToString ());
			GUILayout.Label ("Check Ground Paused FF ZG\t" + list.FindProperty ("checkOnGroundStatePausedFFOrZG").boolValue.ToString ());
			GUILayout.Label ("Check Ground Paused\t\t" + list.FindProperty ("checkOnGroundStatePaused").boolValue.ToString ());
			;
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player ID Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("playerID"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
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
}
#endif