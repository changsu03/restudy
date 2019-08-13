using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(factionSystem))]
public class factionSystemEditor : Editor
{
	factionSystem manager;
	SerializedObject objectToUse;
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		manager = (factionSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Faction List", "window", GUILayout.Height (30));
		showFactionList (objectToUse.FindProperty ("factionList"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showFactionList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Factions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Faction")) {
				manager.addFaction ();
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
						showFactionElementInfo (list.GetArrayElementAtIndex (i));
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
					manager.removeFaction (i);
					//list.DeleteArrayElementAtIndex (i);
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

	void showFactionElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("turnToEnemyIfAttack"));
		if (list.FindPropertyRelative ("turnToEnemyIfAttack").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("turnFactionToEnemy"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("friendlyFireTurnIntoEnemies"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Relation With Other Factions", "window", GUILayout.Height (30));
		showRelationWithOtherFactions (list.FindPropertyRelative ("relationWithFactions"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

//		GUILayout.BeginVertical ("Detected Enemies", "window", GUILayout.Height (30));
//		showEnemiesDetected (list.FindPropertyRelative ("currentDetectedEnemyList"));
//		GUILayout.EndVertical ();
//
//		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showRelationWithOtherFactions (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Factions: \t" + list.arraySize.ToString ());

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
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showWeaponOnPocketListElement (list.GetArrayElementAtIndex (i), i);
					}
				}
				GUILayout.EndVertical ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

//	void showEnemiesDetected (SerializedProperty list)
//	{
//		GUILayout.BeginVertical ();
//		EditorGUILayout.PropertyField (list);
//		if (list.isExpanded) {
//
//			EditorGUILayout.Space ();
//
//			GUILayout.Label ("Number Of Enemies: \t" + list.arraySize.ToString ());
//
//			EditorGUILayout.Space ();
//
//			for (int i = 0; i < list.arraySize; i++) {
//				GUILayout.BeginHorizontal ();
//				GUILayout.BeginVertical ("box");
//				if (i < list.arraySize && i >= 0) {
//					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
//				}
//				GUILayout.EndVertical ();
//				GUILayout.EndHorizontal ();
//			}
//		}
//		GUILayout.EndVertical ();
//	}

	void showWeaponOnPocketListElement (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();
		if (!objectToUse.FindProperty ("removingFaction").boolValue) {
			if (objectToUse.FindProperty ("factionStringList").arraySize > 0) {
				list.FindPropertyRelative ("factionIndex").intValue = EditorGUILayout.Popup ("Faction ", list.FindPropertyRelative ("factionIndex").intValue, manager.getFactionStringList ());
				if (list.FindPropertyRelative ("factionIndex").intValue < manager.factionStringList.Length) {
					list.FindPropertyRelative ("factionName").stringValue = manager.factionStringList [list.FindPropertyRelative ("factionIndex").intValue];
				}

//				if (list.FindPropertyRelative ("factionIndex").intValue < manager.factionStringList.Length) {
//					list.FindPropertyRelative ("factionName").stringValue = manager.factionStringList [list.FindPropertyRelative ("factionIndex").intValue];
//					GUILayout.Label ("Faction Name\t\t" + list.FindPropertyRelative ("factionName").stringValue);
//				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("relation"));
			}
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

}
#endif