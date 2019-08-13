using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(externalShakeListManager))]
public class externalShakeListManagerEditor : Editor
{
	SerializedObject objectToUse;
	externalShakeListManager manager;

	bool sameValue;
	string shakeName;
	string useShakeInThird;
	bool shakeInThirdEnabled;
	bool shakeInFirstEnabled;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		manager = (externalShakeListManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();

		GUILayout.BeginVertical ("box");
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("External Shakes States List", "window", GUILayout.Height (30));
		showExternaShakeInfoList (objectToUse.FindProperty ("externalShakeInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update All Head Bob")) {
			manager.udpateAllHeadbobShakeList ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
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

			EditorGUILayout.Space();

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
					manager.setExternalShakeStateByIndex (index, false);
				}

				EditorGUILayout.Space ();
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
						manager.setExternalShakeStateByIndex (index, true);
					}

					EditorGUILayout.Space ();
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