using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(objectiveEventSystem))]
public class objectiveEventSystemEditor : Editor
{
	SerializedObject objectToUse;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objective Info List", "window", GUILayout.Height (30));
		showObjectiveInfoList (objectToUse.FindProperty ("objectiveInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showObjectiveName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("generalObjectiveName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showObjectiveDescription"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("generalObjectiveDescription"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectiveFullDescription"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hideObjectivePanelsAfterXTime"));
		if (objectToUse.FindProperty ("hideObjectivePanelsAfterXTime").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToHideObjectivePanel"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectivesFollowsOrder"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addObjectiveToPlayerLogSystem"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("canCancelPreviousMissionToStartNewOne"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Time Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useTimeLimit"));
		if (objectToUse.FindProperty ("useTimeLimit").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timerSpeed"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minutesToComplete"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondsToComplete"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondSoundTimerLowerThan"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondTimerSound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToHideObjectivePanel"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window", GUILayout.Height (30));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventWhenObjectiveComplete"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("callEventWhenObjectiveNotComplete"));
		if (objectToUse.FindProperty ("callEventWhenObjectiveNotComplete").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventWhenObjectiveNotComplete"));

			EditorGUILayout.Space ();
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnObjectiveStart"));
		if (objectToUse.FindProperty ("useEventOnObjectiveStart").boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnObjectiveStart"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Map Object Information Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("enableAllMapObjectInformationAtOnce"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("enableAllMapObjectInformationOnTime"));
		if (objectToUse.FindProperty ("enableAllMapObjectInformationOnTime").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToEnableAllMapObjectInformation"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useExtraListMapObjectInformation"));
		if (objectToUse.FindProperty ("useExtraListMapObjectInformation").boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Objective Info List", "window", GUILayout.Height (30));
			showExtraListMapObjectInformation (objectToUse.FindProperty ("extraListMapObjectInformation"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundOnSubObjectiveComplete"));
		if (objectToUse.FindProperty ("useSoundOnSubObjectiveComplete").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundOnSubObjectiveComplete"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundOnObjectiveNotComplete"));
		if (objectToUse.FindProperty ("useSoundOnObjectiveNotComplete").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundOnObjectiveNotComplete"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setCurrentPlayerManually"));
		if (objectToUse.FindProperty ("setCurrentPlayerManually").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentPlayerToConfigure"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectiveInProcess"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectiveComplete"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("numberOfObjectives"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentSubObjectiveIndex"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showObjectiveInfoList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objectives: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Objective")) {
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
						showObjectiveInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showObjectiveInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveDescription"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMapObjectInformation"));
		if (list.FindPropertyRelative ("useMapObjectInformation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentMapObjectInformation"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("giveExtraTime"));
		if (list.FindPropertyRelative ("giveExtraTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTime"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectiveNameOnScreen"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectiveDescriptionOnScreen"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("subObjectiveComplete"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnSubObjectiveComplete"));
		if (list.FindPropertyRelative ("useEventOnSubObjectiveComplete").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnSubObjectiveComplete"));	
		}
		GUILayout.EndVertical ();
	}

	void showExtraListMapObjectInformation (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Map Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Map Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

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
	}
}
#endif