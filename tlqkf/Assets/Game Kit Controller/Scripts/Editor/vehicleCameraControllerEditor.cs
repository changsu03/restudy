using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vehicleCameraController))]
[CanEditMultipleObjects]
public class vehicleCameraControllerEditor : Editor
{
	SerializedObject list;
	vehicleCameraController manager;

	bool showShakeSettings;
	Color defBackgroundColor;

	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (vehicleCameraController)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				if (manager.gameObject == Selection.activeGameObject) {
					GUIStyle style = new GUIStyle ();
					style.normal.textColor = list.FindProperty ("labelGizmoColor").colorValue;
					style.alignment = TextAnchor.MiddleCenter;
					for (int i = 0; i < manager.vehicleCameraStates.Count; i++) {
						if (manager.vehicleCameraStates [i].showGizmo) {
							Handles.color = manager.vehicleCameraStates [i].gizmoColor;
							Handles.Label (manager.vehicleCameraStates [i].cameraTransform.position + 
								(manager.transform.up * manager.vehicleCameraStates [i].labelGizmoOffset), manager.vehicleCameraStates [i].name, style);						
						}
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

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("clipCastRadius"));
		EditorGUILayout.PropertyField (list.FindProperty ("backClipSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("maximumBoostDistance"));
		EditorGUILayout.PropertyField (list.FindProperty ("cameraBoostSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotationDamping"));
		EditorGUILayout.PropertyField (list.FindProperty ("cameraChangeEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("smoothBetweenState"));
		EditorGUILayout.PropertyField (list.FindProperty ("defaultStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicle"));
		EditorGUILayout.PropertyField (list.FindProperty ("layer"));
		EditorGUILayout.PropertyField (list.FindProperty ("smoothTransitionsInNewCameraFov"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("zoomEnabled"));
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

		GUILayout.BeginVertical ("Vehicle Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("IKDrivingManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("weaponManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("hudManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("actionManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("gravityControl"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainRigidbody"));
		EditorGUILayout.PropertyField (list.FindProperty ("shakingManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (list.FindProperty ("labelGizmoColor"));

			EditorGUILayout.PropertyField (list.FindProperty ("showCameraDirectionGizmo"));
			if (list.FindProperty ("showCameraDirectionGizmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowLenght"));
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowLineLenght"));
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowAngle"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		GUILayout.BeginVertical ("Vehicle Camera States", "window", GUILayout.Height (30));
		showUpperList (list.FindProperty ("vehicleCameraStates"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Camera Current State ", "window");
		string cameraPaused = "-";
		if (Application.isPlaying) {
			if (list.FindProperty ("cameraPaused").boolValue) {
				cameraPaused = "YES";
			} else {
				cameraPaused = "NO";
			}
		} 
		string isFirstPerson = "-";
		if (Application.isPlaying) {
			if (list.FindProperty ("isFirstPerson").boolValue) {
				isFirstPerson = "YES";
			} else {
				isFirstPerson = "NO";
			}
		} 
		string usingZoomOn = "-";
		if (Application.isPlaying) {
			if (list.FindProperty ("usingZoomOn").boolValue) {
				usingZoomOn = "YES";
			} else {
				usingZoomOn = "NO";
			}
		}
		GUILayout.Label ("Camera Paused\t" + cameraPaused);
		GUILayout.Label ("First Person View\t" + isFirstPerson);
		GUILayout.Label ("Using Zoom\t"+ usingZoomOn);
		GUILayout.Label ("Driving Vehicle\t" + list.FindProperty ("drivingVehicle").boolValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		showShakeSettings = list.FindProperty ("showShakeSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showShakeSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Shake Settings")) {
			showShakeSettings = !showShakeSettings;
		}

		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndVertical ();

		list.FindProperty ("showShakeSettings").boolValue = showShakeSettings;

		if (showShakeSettings) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Camera Shake Settings", "window", GUILayout.Height (30));

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shake Settings when the vehicle receives Damage", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");
			EditorGUILayout.PropertyField (list.FindProperty ("shakeSettings.useDamageShake"));
			if (list.FindProperty ("shakeSettings.useDamageShake").boolValue) {
			
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindProperty ("shakeSettings.useDamageShakeInThirdPerson"));
				if (list.FindProperty ("shakeSettings.useDamageShakeInThirdPerson").boolValue) {
					showShakeInfo (list.FindProperty ("shakeSettings.thirdPersonDamageShake"));
				
					EditorGUILayout.Space ();
				}
				EditorGUILayout.PropertyField (list.FindProperty ("shakeSettings.useDamageShakeInFirstPerson"));
				if (list.FindProperty ("shakeSettings.useDamageShakeInFirstPerson").boolValue) {
					showShakeInfo (list.FindProperty ("shakeSettings.firstPersonDamageShake"));

					EditorGUILayout.Space ();

				}

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						manager.setDamageCameraShake ();
					}
				}
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndVertical ();
		}

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();

	}

	void showCameraStateElementInfo (SerializedProperty list)
	{
		Color listButtonBackgroundColor;
		bool listGizmoSettings = list.FindPropertyRelative ("gizmoSettings").boolValue;

		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraTransform"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRotationInput"));
		if (list.FindPropertyRelative ("useRotationInput").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("xLimits"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("yLimits"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCameraFov"));
		if (list.FindPropertyRelative ("useNewCameraFov").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCameraFov"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonCamera"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraFixed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothTransition"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useIdentityRotation"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUnlockCursor"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraSteer"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseZoom"));
		if (list.FindPropertyRelative ("canUseZoom").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("zoomSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("zoomFovValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationSpeedZoomIn"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (listGizmoSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = listButtonBackgroundColor;
		}
		if (GUILayout.Button ("Gizmo Settings")) {
			listGizmoSettings = !listGizmoSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();
		list.FindPropertyRelative ("gizmoSettings").boolValue = listGizmoSettings;
		if (listGizmoSettings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Camera State Gizmo Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("gizmoColor"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("labelGizmoOffset"));

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of States: \t" + list.arraySize.ToString ());

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

	void showShakeInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		GUILayout.EndVertical ();
	}
}
#endif