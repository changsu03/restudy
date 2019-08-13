using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(grabObjects))]
public class grabObjectsEditor : Editor
{
	SerializedObject list;
	bool expanded;
	string currentPhysicalObjectToGrabFound;

	void OnEnable ()
	{
		list = new SerializedObject (target);
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Grab Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("grabObjectsEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("holdDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceHeld"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceGrab"));
		EditorGUILayout.PropertyField (list.FindProperty ("holdSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("alphaTransparency"));
		EditorGUILayout.PropertyField (list.FindProperty ("closestHoldDistanceInFixedPosition"));
		EditorGUILayout.PropertyField (list.FindProperty ("useCursor"));
		EditorGUILayout.PropertyField (list.FindProperty ("onlyEnableCursorIfLocatedObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("grabCursorScale"));	
		EditorGUILayout.PropertyField (list.FindProperty ("grabInFixedPosition"));	
		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectTag"));	
		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectLayer"));	
		EditorGUILayout.PropertyField (list.FindProperty ("currentGrabMode"));
		if (list.FindProperty ("currentGrabMode").enumValueIndex == 0) {
			EditorGUILayout.PropertyField (list.FindProperty ("changeGravityObjectsEnabled"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectCanBeBroken"));

		EditorGUILayout.PropertyField (list.FindProperty ("useForceWhenObjectDropped"));
		if (list.FindProperty ("useForceWhenObjectDropped").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useForceWhenObjectDroppedOnThirdPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("forceWhenObjectDroppedOnThirdPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("useForceWhenObjectDroppedOnFirstPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("forceWhenObjectDroppedOnFirstPerson"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("pauseCameraMouseWheelWhileObjectGrabbed"));
		EditorGUILayout.PropertyField (list.FindProperty ("grabObjectActionName"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Able To Grab List", "window");
		showAbleToGrabTagList (list.FindProperty ("ableToGrabTags"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grab Physical Objects Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("grabObjectsPhysicallyEnabled"));
		if (list.FindProperty ("grabObjectsPhysicallyEnabled").boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hands List", "window");
			showHandsList (list.FindProperty ("handInfoList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindProperty ("placeToCarryPhysicalObjectsThirdPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("placeToCarryPhysicalObjectsFirstPerson"));
			EditorGUILayout.PropertyField (list.FindProperty ("translatePhysicalObjectSpeed"));

			EditorGUILayout.PropertyField (list.FindProperty ("showGrabObjectIconEnabled"));
			if (list.FindProperty ("showGrabObjectIconEnabled").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("keyText"));
				EditorGUILayout.PropertyField (list.FindProperty ("grabObjectIcon"));
				EditorGUILayout.PropertyField (list.FindProperty ("iconRectTransform"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("getClosestDeviceToCameraCenter"));
			if (list.FindProperty ("getClosestDeviceToCameraCenter").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("useMaxDistanceToCameraCenter"));
				EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToCameraCenter"));	
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Outline Shader Device Found Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("useObjectToGrabFoundShader"));
		if (list.FindProperty ("useObjectToGrabFoundShader").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("objectToGrabFoundShader"));
			EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineWidth"));
			EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Collision Noises Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("launchedObjectsCanMakeNoise"));
		if (list.FindProperty ("launchedObjectsCanMakeNoise").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minObjectSpeedToActivateNoise"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rotation Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("objectCanBeRotated"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotateSpeed"));
		if (list.FindProperty ("grabInFixedPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("rotateToCameraInFixedPosition"));
		} else {
			EditorGUILayout.PropertyField (list.FindProperty ("rotateToCameraInFreePosition"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Throw Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("minTimeToIncreaseThrowForce"));
		EditorGUILayout.PropertyField (list.FindProperty ("increaseThrowForceSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("extraThorwForce"));	
		EditorGUILayout.PropertyField (list.FindProperty ("maxThrowForce"));		
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("canUseZoomWhileGrabbed"));
		EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("maxZoomDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("minZoomDistance"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Power Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("layer"));
		EditorGUILayout.PropertyField (list.FindProperty ("gravityObjectsLayer"));
		EditorGUILayout.PropertyField (list.FindProperty ("layerForCustomGravityObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("enableTransparency"));
		EditorGUILayout.PropertyField (list.FindProperty ("powerForceMode"));
		EditorGUILayout.PropertyField (list.FindProperty ("useThrowObjectsLayer"));	
		EditorGUILayout.PropertyField (list.FindProperty ("throwObjectsLayerToCheck"));	

		EditorGUILayout.PropertyField (list.FindProperty ("useGrabbedParticles"));
		EditorGUILayout.PropertyField (list.FindProperty ("useLoadThrowParticles"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Realistic Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("throwPower"));	
		EditorGUILayout.PropertyField (list.FindProperty ("realisticForceMode"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grab Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("grabZoneTransform"));	
		EditorGUILayout.PropertyField (list.FindProperty ("cursor"));	
		EditorGUILayout.PropertyField (list.FindProperty ("cursorRectTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("grabObjectCursor"));	
		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectCursor"));	
		EditorGUILayout.PropertyField (list.FindProperty ("pickableShader"));	
		EditorGUILayout.PropertyField (list.FindProperty ("powerSlider"));

		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectClonnedColliderTransform"));	
		EditorGUILayout.PropertyField (list.FindProperty ("grabbedObjectClonnedCollider"));
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Particles List", "window");
		showParticlesList (list.FindProperty ("particles"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player State", "window");
		if (list.FindProperty ("objectHeld").objectReferenceValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("objectHeld"), new GUIContent ("Current Object Held"), false);
		} else {
			GUILayout.Label ("Current Object Held\t" + "None");
		}

		GUILayout.Label ("Aiming Active\t" + list.FindProperty ("aiming").boolValue.ToString ());
		GUILayout.Label ("Object Grabbed\t" + list.FindProperty ("grabbed").boolValue.ToString ());
		GUILayout.Label ("Is Gear\t\t" + list.FindProperty ("gear").boolValue.ToString ());
		GUILayout.Label ("Is Rail\t\t" + list.FindProperty ("rail").boolValue.ToString ());
		GUILayout.Label ("Is Regular Object\t" + list.FindProperty ("regularObject").boolValue.ToString ());
		GUILayout.Label ("Is Physical Object\t" + list.FindProperty ("carryingPhysicalObject").boolValue.ToString ());

		if (list.FindProperty ("currentPhysicalObjectToGrabFound").objectReferenceValue != null) {
			currentPhysicalObjectToGrabFound = "YES";
		} else {
			currentPhysicalObjectToGrabFound = "NO";
		}
			
		GUILayout.Label ("Physic Object Found \t" + currentPhysicalObjectToGrabFound);
		GUILayout.Label ("Object To Grab Found\t" + list.FindProperty ("objectFocus").boolValue.ToString ());
		EditorGUILayout.PropertyField (list.FindProperty ("currentObjectToGrabFound"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("playerCameraTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("usingDevicesManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("gravityManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("IKManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCollider"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCamera"));
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

	void showParticlesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Particles: " + list.arraySize.ToString ());
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

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showAbleToGrabTagList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Tags: " + list.arraySize.ToString ());
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

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showHandsListElement (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("IKHint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("IKGoal"));
	}

	void showHandsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Hands: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Hand")) {
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
						showHandsListElement (list.GetArrayElementAtIndex (i));
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
}
#endif