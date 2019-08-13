using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(closeCombatSystem))]
public class closeCombatSystemEditor : Editor
{
	closeCombatSystem combatManager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		combatManager = (closeCombatSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (combatManager.showGizmo) {
			for (int i = 0; i < combatManager.combatLimbList.Count; i++) {
				if (combatManager.combatLimbList [i].limb) {
					style.normal.textColor = combatManager.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;
					string label = combatManager.combatLimbList [i].name + "\n" + combatManager.combatLimbList [i].hitDamage;
					Handles.Label (combatManager.combatLimbList [i].limb.transform.position, label, style);	
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		if (objectToUse == null) {
			return;
		}
		objectToUse.Update ();
		GUILayout.BeginVertical ("box");
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("combatSystemEnabled"));
		//EditorGUILayout.PropertyField (objectToUse.FindProperty ("timeToDisableTriggers"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerMaskToDamage"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("defaultArmHitDamage"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("defaultLegHitDamage"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addForceMultiplier"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hitCombatPrefab"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("combatLayerIndex"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("punchIDName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("kickIDName"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window", GUILayout.Height (30));
		GUILayout.Label ("Combat Mode Active\t" + objectToUse.FindProperty ("currentPlayerMode").boolValue.ToString ());
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventsOnStateChange"));
		if (objectToUse.FindProperty ("useEventsOnStateChange").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("evenOnStateEnabled"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnStateDisabled"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Combat Limb List", "window", GUILayout.Height (30));
		showUpperList (objectToUse.FindProperty ("combatLimbList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Elements Settings", "window");
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("powersManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gravityManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("weaponsManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("animator"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Combat Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("controlledByAI"));
		if (objectToUse.FindProperty ("controlledByAI").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("alwaysKicks"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("alwaysPunchs"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("makeFullCombos"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("alternateKickAndPunchs"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("randomAttack"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minTimeBetweenAttacks"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Make AI Attack (Only ingame)")) {
				if (Application.isPlaying) {
					combatManager.AIMeleeAttack ();
				}
			}

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hitDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limbType"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("trail"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hitCombatManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("trigger"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Limbs: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Hit Combat Triggers Info")) {
				combatManager.udpateHitCombatInfo ();
			}

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
						showListElementInfo (list.GetArrayElementAtIndex (i));
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

			if (GUILayout.Button ("Assign Basic Combat Triggers")) {
				combatManager.assignBasicCombatTriggers ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear Combat Triggers")) {
				combatManager.removeLimbParts ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}
}
#endif