using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(footStepManager))]
[CanEditMultipleObjects]
public class footStepsManagerEditor : Editor
{
	SerializedObject list;

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

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use FootSteps Setting", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("soundsEnabled"));
		GUILayout.EndVertical ();

		if (list.FindProperty ("soundsEnabled").boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("feetVolumeRange"));
			EditorGUILayout.PropertyField (list.FindProperty ("typeOfFootStep"));
			EditorGUILayout.PropertyField (list.FindProperty ("layer"));
			EditorGUILayout.PropertyField (list.FindProperty ("leftFoot"));
			EditorGUILayout.PropertyField (list.FindProperty ("rightFoot"));
			EditorGUILayout.PropertyField (list.FindProperty ("defaultSurfaceName"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Prints Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useFootPrints"));
			if (list.FindProperty ("useFootPrints").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("useFootPrintsFromStates"));

				if (!list.FindProperty ("useFootPrintsFromStates").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("rightFootPrint"));
					EditorGUILayout.PropertyField (list.FindProperty ("leftFootPrint"));
				}

				EditorGUILayout.PropertyField (list.FindProperty ("maxFootPrintDistance"));
				EditorGUILayout.PropertyField (list.FindProperty ("distanceBetweenPrintsInFisrtPerson"));
				EditorGUILayout.PropertyField (list.FindProperty ("useFootPrintMaxAmount"));
				if (list.FindProperty ("useFootPrintMaxAmount").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("footPrintMaxAmount"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("removeFootPrintsInTime"));
				if (list.FindProperty ("removeFootPrintsInTime").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("timeToRemoveFootPrints"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("vanishFootPrints"));
				if (list.FindProperty ("vanishFootPrints").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("vanishSpeed"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Particles Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useFootParticles"));
			if (list.FindProperty ("useFootParticles").boolValue) {
				
				EditorGUILayout.PropertyField (list.FindProperty ("useFootParticlesFromStates"));
				if (!list.FindProperty ("useFootParticlesFromStates").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("footParticles"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Steps List", "window");
			showFootStepsList (list.FindProperty ("footSteps"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Step State List", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useFootStepStateList"));
			if (list.FindProperty ("useFootStepStateList").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("currentFootStepStateName"));

				EditorGUILayout.PropertyField (list.FindProperty ("noiseDetectionLayer"));

				EditorGUILayout.Space ();

				showFootStepStateList (list.FindProperty ("footStepStateList"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("showNoiseDetectionGizmo"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("playerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("leftFootStep"));
			EditorGUILayout.PropertyField (list.FindProperty ("rightFootStep"));
			EditorGUILayout.PropertyField (list.FindProperty ("leftFootCollider"));
			EditorGUILayout.PropertyField (list.FindProperty ("rightFootCollider"));
			EditorGUILayout.PropertyField (list.FindProperty ("cameraAudioSource"));
			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showFootStepsListElementInfo (SerializedProperty list, bool showListNames, int footStepIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomPool"));
		EditorGUILayout.Space ();

		if (showListNames) {
			GUILayout.BeginVertical ("Pool Sounds", "window", GUILayout.Height (30));
			showLowerList (list.FindPropertyRelative ("poolSounds"), footStepIndex);
			GUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}

		GUILayout.BeginVertical ("Surface System Detection Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkSurfaceSystem"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Terrain Detection Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkTerrain"));
		if (list.FindPropertyRelative ("checkTerrain").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("terrainTextureName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("terrainTextureIndex"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Foot Print Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFootPrints"));
		if (list.FindPropertyRelative ("useFootPrints").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rightFootPrint"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftFootPrint"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Foot Particles Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFootParticles"));
		if (list.FindPropertyRelative ("useFootParticles").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("footParticles"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFootStepsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Surfaces: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Surface")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFootStepsListElementInfo (list.GetArrayElementAtIndex (i), true, i);
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
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showFootStepStateList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFootStepStateListElementInfo (list.GetArrayElementAtIndex (i), true);
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
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showFootStepStateListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("feetVolumeRange"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stepInterval"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playSoundOnce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkPlayerOnGround"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoise"));
		if (list.FindPropertyRelative ("useNoise").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseRadius"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseExpandSpeed"));	
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseDetection"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewStateAfterDelay"));
		if (list.FindPropertyRelative ("setNewStateAfterDelay").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newStateDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newStateName"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playCustomSound"));
		if (list.FindPropertyRelative ("playCustomSound").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customSound"));
		}
		GUILayout.EndVertical ();
	}

	void showLowerList (SerializedProperty list, int footStepIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Sound")) {
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