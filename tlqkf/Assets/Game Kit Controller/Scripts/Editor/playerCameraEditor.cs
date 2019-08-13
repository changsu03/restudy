using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerCamera))]
public class playerCameraEditor : Editor
{
	SerializedObject list;
	playerCamera camera;
	public string currentCamera;
	bool checkCamera;
	bool settings;
	bool lookTargetSettings;
	Color defBackgroundColor;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		camera = (playerCamera)target;
	}

	void OnSceneGUI ()
	{   
		if (camera.settings.showCameraGizmo) {
			style.alignment = TextAnchor.MiddleCenter;
			if (camera.gameObject == Selection.activeGameObject) {
				for (int i = 0; i < camera.playerCameraStates.Count; i++) {
					if (camera.playerCameraStates [i].showGizmo) {
						style.normal.textColor = camera.settings.gizmoLabelColor;
						Handles.Label (camera.gameObject.transform.position + camera.playerCameraStates [i].pivotPositionOffset
						+ camera.playerCameraStates [i].camPositionOffset, camera.playerCameraStates [i].Name, style);						
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

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Components", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("mainCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("pivotCameraTransform"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		//EditorGUILayout.PropertyField (list.FindProperty ("cameraType"));
		EditorGUILayout.PropertyField (list.FindProperty ("thirdPersonVerticalRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("thirdPersonHorizontalRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("firstPersonVerticalRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("firstPersonHorizontalRotationSpeed"));

		EditorGUILayout.PropertyField (list.FindProperty ("smoothBetweenState"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxCheckDist"));
		EditorGUILayout.PropertyField (list.FindProperty ("movementLerpSpeed"));

		EditorGUILayout.PropertyField (list.FindProperty ("regularMovementOnBulletTime"));

		EditorGUILayout.PropertyField (list.FindProperty ("extraCameraCollisionDistance"));

		EditorGUILayout.PropertyField (list.FindProperty ("changeCameraViewEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Smooth Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("useSmoothCameraRotation"));
		if (list.FindProperty ("useSmoothCameraRotation").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useSmoothCameraRotationThirdPerson"));
			if (list.FindProperty ("useSmoothCameraRotationThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("smoothCameraRotationSpeedVerticalThirdPerson"));
				EditorGUILayout.PropertyField (list.FindProperty ("smoothCameraRotationSpeedHorizontalThirdPerson"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("useSmoothCameraRotationFirstPerson"));
			if (list.FindProperty ("useSmoothCameraRotationFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("smoothCameraRotationSpeedVerticalFirstPerson"));
				EditorGUILayout.PropertyField (list.FindProperty ("smoothCameraRotationSpeedHorizontalFirstPerson"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("fovChangeSpeed"));

		EditorGUILayout.PropertyField (list.FindProperty ("thirdPersonVerticalRotationSpeedZoomIn"));
		EditorGUILayout.PropertyField (list.FindProperty ("thirdPersonHorizontalRotationSpeedZoomIn"));
		EditorGUILayout.PropertyField (list.FindProperty ("firstPersonVerticalRotationSpeedZoomIn"));
		EditorGUILayout.PropertyField (list.FindProperty ("firstPersonHorizontalRotationSpeedZoomIn"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mouse Wheel Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("moveCameraPositionWithMouseWheelActive"));
		if (list.FindProperty ("moveCameraPositionWithMouseWheelActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("moveCameraPositionForwardWithMouseWheelSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("moveCameraPositionBackwardWithMouseWheelSpeed"));

			EditorGUILayout.PropertyField (list.FindProperty ("maxExtraDistanceOnThirdPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("useCameraMouseWheelStates"));
			if (list.FindProperty ("useCameraMouseWheelStates").boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Mouse Wheel Settings", "window");
				showCameraMouseWheelStatesList (list.FindProperty ("cameraMouseWheelStatesList"));
				GUILayout.EndVertical ();
			} else {
				EditorGUILayout.PropertyField (list.FindProperty ("minDistanceToChangeToFirstPerson"));
			}
		}
		GUILayout.EndVertical ();

		if (list.FindProperty ("using2_5ViewActive").boolValue || list.FindProperty ("cameraType").enumValueIndex == 1) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("2.5d Camera Settings", "window");

			EditorGUILayout.PropertyField (list.FindProperty ("useCameraLimit"));
			if (list.FindProperty ("useCameraLimit").boolValue) {

				EditorGUILayout.PropertyField (list.FindProperty ("useWidthLimit"));
				if (list.FindProperty ("useWidthLimit").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("widthLimitRight"));
					EditorGUILayout.PropertyField (list.FindProperty ("widthLimitLeft"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("useHeightLimit"));
				if (list.FindProperty ("useHeightLimit").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("heightLimitUpper"));
					EditorGUILayout.PropertyField (list.FindProperty ("heightLimitLower"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("useDepthLimit"));
				if (list.FindProperty ("useDepthLimit").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("depthLimitFront"));
					EditorGUILayout.PropertyField (list.FindProperty ("depthLimitBackward"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("currentCameraLimitPosition"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("clampAimDirections"));
			EditorGUILayout.PropertyField (list.FindProperty ("numberOfAimDirections"));
			EditorGUILayout.PropertyField (list.FindProperty ("minDistanceToCenterClampAim"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Camera Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("canRotateForwardOnZeroGravityModeOn"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotateForwardOnZeroGravitySpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Camera Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("resetCameraRotationAfterTime"));
		if (list.FindProperty ("resetCameraRotationAfterTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("timeToResetCameraRotation"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("resetCameraRotationSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Player Camera States", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("currentStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultThirdPersonStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultFirstPersonStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultThirdPersonCrouchStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultFirstPersonCrouchStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultThirdPersonAimRightStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultThirdPersonAimLeftStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultMoveCameraAwayStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultLockedCameraStateName"));

		EditorGUILayout.Space ();

		showUpperList (list.FindProperty ("playerCameraStates"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current Camera States", "window");
		GUILayout.Label ("Camera Mode\t" + camera.cameraType.ToString());
		GUILayout.Label ("On Ground\t\t" + list.FindProperty ("onGround").boolValue.ToString ());
		GUILayout.Label ("Aiming 3rd Person\t" + list.FindProperty ("aimingInThirdPerson").boolValue.ToString ());
		GUILayout.Label ("Move Away Active\t" + list.FindProperty ("moveAwayActive").boolValue.ToString ());
		GUILayout.Label ("Crouching\t\t" + list.FindProperty ("crouching").boolValue.ToString ());
		GUILayout.Label ("First Person Active\t" + list.FindProperty ("firstPersonActive").boolValue.ToString ());
		GUILayout.Label ("Using Zoom On\t" + list.FindProperty ("usingZoomOn").boolValue.ToString ());
		GUILayout.Label ("Using Zoom Off\t" + list.FindProperty ("usingZoomOff").boolValue.ToString ());
		GUILayout.Label ("Camera Can Rotate\t" + list.FindProperty ("cameraCanRotate").boolValue.ToString ());
		GUILayout.Label ("Camera Can Be Used\t" + list.FindProperty ("cameraCanBeUsed").boolValue.ToString ());
		GUILayout.Label ("Looking At Target\t" + list.FindProperty ("lookingAtTarget").boolValue.ToString ());
		GUILayout.Label ("Looking At Point\t" + list.FindProperty ("lookingAtPoint").boolValue.ToString ());
		GUILayout.Label ("Using 2.5d View\t" + list.FindProperty ("using2_5ViewActive").boolValue.ToString ());
		GUILayout.Label ("Using Top Down View\t" + list.FindProperty ("useTopDownView").boolValue.ToString ());
		GUILayout.Label ("Horizontal Input\t" + list.FindProperty ("horizontalInput").floatValue.ToString ());
		GUILayout.Label ("Vertical Input\t" + list.FindProperty ("verticalInput").floatValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraSystemPrefab"));
		EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraLimitSystemPrefab"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add New Locked Camera System")) {
			camera.addNewLockedCameraSystemToLevel ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add New Locked Camera Limit System")) {
			camera.addNewLockedCameraLimitSystemToLevel ();
		}

		EditorGUILayout.Space ();
		GUILayout.BeginVertical ("Locked Camera Prefabs List", "window");
		showLockedCameraPrefabsTypesList (list.FindProperty ("lockedCameraPrefabsTypesList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		lookTargetSettings = list.FindProperty ("showLookTargetSettings").boolValue;
		settings = list.FindProperty ("showSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (settings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Settings")) {
			settings = !settings;
		}
		if (lookTargetSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Look Target Settings")) {
			lookTargetSettings = !lookTargetSettings;
		}

		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndVertical ();

		list.FindProperty ("showSettings").boolValue = settings;
		list.FindProperty ("showLookTargetSettings").boolValue = lookTargetSettings;

		if (settings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Basic Camera Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settinngs", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("settings.layer"));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.useAcelerometer"));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.zoomEnabled"));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.moveAwayCameraEnabled"));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.enableMoveAwayInAir"));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.enableShakeCamera"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Locked Camera Elements", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraElementsParent"));
			EditorGUILayout.PropertyField (list.FindProperty ("lockedMainCameraTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraAxis"));
			EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraPosition"));
			EditorGUILayout.PropertyField (list.FindProperty ("lockedCameraPivot"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookCameraParent"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookCameraPivot"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookCameraDirection"));
			EditorGUILayout.PropertyField (list.FindProperty ("clampAimDirectionTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookDirectionTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("auxLockedCameraAxis"));
			EditorGUILayout.PropertyField (list.FindProperty ("setTransparentSurfacesManager"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Camera Elements", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("gravityManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("headBobManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("grabObjectsManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("scannerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerNavMeshManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("characterStateIconManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainGameManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainCollider"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainAnimator"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event On Moving Locked Camera Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useEventOnMovingLockedCamera"));
			EditorGUILayout.PropertyField (list.FindProperty ("useEventOnFreeCamereToo"));
			if (list.FindProperty ("useEventOnMovingLockedCamera").boolValue || list.FindProperty ("useEventOnFreeCamereToo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnStartMovingLockedCamera"));
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnKeepMovingLockedCamera"));
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnStopMovingLockedCamera"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event On Third/First Person View Change Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useEventOnThirdFirstPersonViewChange"));
			if (list.FindProperty ("useEventOnThirdFirstPersonViewChange").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("setFirstPersonEvent"));
				EditorGUILayout.PropertyField (list.FindProperty ("setThirdPersonEvent"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Change Third/First Person View In Editor Events Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("setThirdPersonInEditorEvent"));
			EditorGUILayout.PropertyField (list.FindProperty ("setFirstPersonInEditorEvent"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Camera List Settings", "window", GUILayout.Height (30));
			showSimpleList (list.FindProperty ("cameraList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
			GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("settings.showCameraGizmo"));
			if (list.FindProperty ("settings.showCameraGizmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("settings.gizmoRadius"));
				EditorGUILayout.PropertyField (list.FindProperty ("settings.gizmoLabelColor"));
				EditorGUILayout.PropertyField (list.FindProperty ("settings.linesColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		if (lookTargetSettings) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Look At Target Camera Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Look At Target Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("lookAtTargetEnabled"));
			if (list.FindProperty ("lookAtTargetEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("canActivateLookAtTargetEnabled"));
				EditorGUILayout.PropertyField (list.FindProperty ("lookAtTargetTransform"));
				EditorGUILayout.PropertyField (list.FindProperty ("targetToLook"));

				EditorGUILayout.PropertyField (list.FindProperty ("lookAtTargetSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("lookCloserAtTargetSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("lookAtTargetSpeed2_5dView"));

				EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToFindTarget"));
				EditorGUILayout.PropertyField (list.FindProperty ("useLookTargetIcon"));
				if (list.FindProperty ("useLookTargetIcon").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("lookAtTargetIcon"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("useLayerToSearchTargets"));	
				if (list.FindProperty ("useLayerToSearchTargets").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("layerToLook"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("lookOnlyIfTargetOnScreen"));
				EditorGUILayout.PropertyField (list.FindProperty ("checkObstaclesToTarget"));
				EditorGUILayout.PropertyField (list.FindProperty ("getClosestToCameraCenter"));
				if (list.FindProperty ("getClosestToCameraCenter").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("useMaxDistanceToCameraCenter"));
					if (list.FindProperty ("useMaxDistanceToCameraCenter").boolValue) {
						EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToCameraCenter"));
					}
				}
				EditorGUILayout.PropertyField (list.FindProperty ("useTimeToStopAimAssist"));
				if (list.FindProperty ("useTimeToStopAimAssist").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("timeToStopAimAssist"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("useTimeToStopAimAssistLockedCamera"));
				if (list.FindProperty ("useTimeToStopAimAssistLockedCamera").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("timeToStopAimAssistLockedCamera"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("searchPointToLookComponents"));
				if (list.FindProperty ("searchPointToLookComponents").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("pointToLookComponentsLayer"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("canActiveLookAtTargetOnLockedCamera"));
				EditorGUILayout.PropertyField (list.FindProperty ("canChangeTargetToLookWithCameraAxis"));
				if (list.FindProperty ("canChangeTargetToLookWithCameraAxis").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("minimumCameraDragToChangeTargetToLook"));
					EditorGUILayout.PropertyField (list.FindProperty ("waitTimeToNextChangeTargetToLook"));
					EditorGUILayout.PropertyField (list.FindProperty ("useOnlyHorizontalCameraDragValue"));
				}

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Tag To Look List", "window", GUILayout.Height (30));
				showSimpleList (list.FindProperty ("tagToLookList"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Look At Characters Body Parts Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindProperty ("lookAtBodyPartsOnCharacters"));

				if (list.FindProperty ("lookAtBodyPartsOnCharacters").boolValue) {
					
					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Body Parts To Look List", "window", GUILayout.Height (30));
					showSimpleList (list.FindProperty ("bodyPartsToLook"));
					GUILayout.EndVertical ();
				}

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Look At Characters Body Parts Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindProperty ("useObjectToGrabFoundShader"));

				if (list.FindProperty ("useObjectToGrabFoundShader").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("objectToGrabFoundShader"));
					EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineWidth"));
					EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineColor"));
				}

				GUILayout.EndVertical ();

			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		EditorGUILayout.Space ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();

		if (!Application.isPlaying) {
			playerCamera camera = (playerCamera)target;
			//set in the inspector the current camera type
			if (!checkCamera) {
				if (camera.firstPersonActive) {
					currentCamera = "FIRST PERSON";
				} else {
					currentCamera = "THIRD PERSON";
				}
				checkCamera = true;
			}
			GUILayout.Label ("Current Camera: " + currentCamera.ToString ());
			if (GUILayout.Button ("Set First Person")) {
				camera.setFirstPersonInEditor ();
				currentCamera = "FIRST PERSON";
			}
			if (GUILayout.Button ("Set Third Person")) {
				camera.setThirdPersonInEditor ();
				currentCamera = "THIRD PERSON";
			}
		}
	}

	void showCameraStateElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("camPositionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotPositionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotParentPossitionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("yLimits"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("initialFovValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fovTransitionSpeed"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxFovValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("minFovValue"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));
		if (list.FindPropertyRelative ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("gizmoColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
						showCameraStateElementInfo (list.GetArrayElementAtIndex (i));
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

	void showCameraMouseWheelStatesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
						showCameraMouseWheelStatesListElement (list.GetArrayElementAtIndex (i));
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

	void showCameraMouseWheelStatesListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraDistanceRange"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentCameraState"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraFromAboveStateWithName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraFromBelowStateWithName"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraIfDistanceChanged"));
		if (list.FindPropertyRelative ("changeCameraIfDistanceChanged").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minCameraDistanceToChange"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeToAboveCameraState"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFromAboveCameraState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFromBelowCameraState"));

		GUILayout.EndVertical ();
	}

	void showLockedCameraPrefabsTypesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Prefabs: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Prefab")) {
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
						showLockedCameraPrefabsTypesListElement (list.GetArrayElementAtIndex (i), i);
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
				if (GUILayout.Button ("Add")) {
					camera.addNewLockedCameraPrefabTypeLevel (i);
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
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

	void showLockedCameraPrefabsTypesListElement (SerializedProperty list, int lockedCameraIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("lockedCameraPrefab"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Camera To Scene")) {
			camera.addNewLockedCameraPrefabTypeLevel (lockedCameraIndex);
		}
		GUILayout.EndVertical ();
	}
}
#endif