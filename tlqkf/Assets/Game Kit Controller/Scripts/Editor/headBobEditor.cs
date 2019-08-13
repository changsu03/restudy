using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(headBob))]
public class headBobEditor : Editor
{
	SerializedObject objectToUse;
	headBob headBobManager;
	bool settings;
	bool elementSettings;
	bool showThirdPerson;
	bool showFirstPerson;
	Color buttonColor;

	bool sameValue;
	string shakeName;
	string useShakeInThird;
	bool shakeInThirdEnabled;
	bool shakeInFirstEnabled;
	bool applicationIsPlaying;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		headBobManager = (headBob)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();

		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("headBobEnabled"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("staticIdleName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentState"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("externalShakeEnabled"));
		if (objectToUse.FindProperty ("externalShakeEnabled").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("externalForceStateName"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("resetSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useDynamicIdle"));
		if (objectToUse.FindProperty ("useDynamicIdle").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("dynamicIdleName"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToActiveDynamicIdle"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("shakeCameraInLockedMode"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Jump Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("jumpStartMaxIncrease"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("jumpStartSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("jumpEndMaxDecrease"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("jumpEndSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("jumpResetSpeed"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bob States List", "window", GUILayout.Height (30));
		showBobList (objectToUse.FindProperty ("bobStatesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Bob State ", "window");

		applicationIsPlaying = Application.isPlaying;
		string isFirstPerson = "-";
		if (applicationIsPlaying) {
			if (objectToUse.FindProperty ("firstPersonMode").boolValue) {
				isFirstPerson = "YES";
			} else {
				isFirstPerson = "NO";
			}
		} 
		string isExternalShaking = "-";
		if (applicationIsPlaying) {
			if (objectToUse.FindProperty ("externalShakingActive").boolValue) {
				isExternalShaking = "YES";
			} else {
				isExternalShaking = "NO";
			}
		} 
		string canBeUsed = "-";
		if (applicationIsPlaying) {
			if (objectToUse.FindProperty ("headBobCanBeUsed").boolValue) {
				canBeUsed = "YES";

			} else {
				canBeUsed = "NO";
			}
		} 
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("First Person View ");
		GUILayout.Label (isFirstPerson);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("External Shake Active");
		GUILayout.Label (isExternalShaking);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Head Bob can be used");
		GUILayout.Label (canBeUsed);
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		if (objectToUse.FindProperty ("externalShakeEnabled").boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("External Shakes States List", "window", GUILayout.Height (30));
			showExternaShakeInfoList (objectToUse.FindProperty ("externalShakeInfoList"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements Settings", "window");
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerCameraManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobTransformStyle"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enableBobIn"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posAmount"), new GUIContent ("Position Amount"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posSpeed"), new GUIContent ("Position Speed"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posSmooth"), new GUIContent ("Position Smooth"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulAmount"), new GUIContent ("Rotation Amount"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulSpeed"), new GUIContent ("Rotation Speed"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulSmooth"), new GUIContent ("Rotation Smooth"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		GUILayout.EndVertical ();
	}

	void showBobList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Bob States: \t" + list.arraySize.ToString ());
			
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showElementInfo (list.GetArrayElementAtIndex (i));
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

			GUILayout.BeginVertical ("Current State Info", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerBobState"));
			if (objectToUse.FindProperty ("playerBobState").isExpanded) {
				showElementInfo (objectToUse.FindProperty ("playerBobState"));
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showExternaShakeInfoList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of External Shakes: \t" + list.arraySize.ToString ());
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Shake")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showExternalShakeElementInfo (list.GetArrayElementAtIndex (i), i);
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

	void showExternalShakeElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sameValueBothViews"));
		sameValue = list.FindPropertyRelative ("sameValueBothViews").boolValue;
		shakeName = "Third Person Damage Shake";
		useShakeInThird = "Shake In Third Person Enabled";
		if (sameValue) {
			shakeName = "Damage Shake";
			useShakeInThird = "Shake Enabled";
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInThirdPerson"), new GUIContent (useShakeInThird), false);
		if (!sameValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInFirstPerson"), new GUIContent ("Shake In First Person Enabled"), false);
		}

		shakeInThirdEnabled = list.FindPropertyRelative ("useDamageShakeInThirdPerson").boolValue;
		shakeInFirstEnabled = list.FindPropertyRelative ("useDamageShakeInFirstPerson").boolValue;
		if (shakeInThirdEnabled) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonDamageShake"), new GUIContent (shakeName), false);
			if (list.FindPropertyRelative ("thirdPersonDamageShake").isExpanded) {
				GUILayout.BeginVertical (shakeName, "window", GUILayout.Height (30));
				showExternalShakeElementInfoContent (list.FindPropertyRelative ("thirdPersonDamageShake"));
				GUILayout.EndVertical ();
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Test Shake")) {
					if (applicationIsPlaying) {
						headBobManager.setExternalShakeStateByIndex (index, false);
					}
				}

				EditorGUILayout.Space ();

				if (applicationIsPlaying) {
					if (GUILayout.Button ("Set Shake In Manager List")) {
						headBobManager.setShakeInManagerList (index);
					}

					EditorGUILayout.Space ();
				}
			}
		}
		if (shakeInFirstEnabled) {
			if (!list.FindPropertyRelative ("sameValueBothViews").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonDamageShake"));
				if (list.FindPropertyRelative ("firstPersonDamageShake").isExpanded) {
					GUILayout.BeginVertical ("First Person Damage Shake", "window", GUILayout.Height (30));
					showExternalShakeElementInfoContent (list.FindPropertyRelative ("firstPersonDamageShake"));
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Test Shake")) {
						if (applicationIsPlaying) {
							headBobManager.setExternalShakeStateByIndex (index, true);
						}
					}
				}
			}
		}
		GUILayout.EndVertical ();
	}

	void showExternalShakeElementInfoContent (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePositionSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePositionSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("decreaseShakeInTime"));
		if (list.FindPropertyRelative ("decreaseShakeInTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("decreaseShakeSpeed"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayBeforeStartDecrease"));
		if (list.FindPropertyRelative ("useDelayBeforeStartDecrease").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayBeforeStartDecrease"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("repeatShake"));
		if (list.FindPropertyRelative ("repeatShake").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfRepeats"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayBetweenRepeats"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("externalShakeDelay"));
	}
}
#endif