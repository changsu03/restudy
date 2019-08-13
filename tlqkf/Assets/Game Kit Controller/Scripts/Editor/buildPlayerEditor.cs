using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(buildPlayer))]
public class buildPlayerEditor : Editor
{
	SerializedObject list;
	buildPlayer manager;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		manager = (buildPlayer)target;
	}


	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("trail"));
		EditorGUILayout.PropertyField (list.FindProperty ("hitCombat"));
		EditorGUILayout.PropertyField (list.FindProperty ("shootZone"));
		EditorGUILayout.PropertyField (list.FindProperty ("arrow"));
		EditorGUILayout.PropertyField (list.FindProperty ("player"));
		EditorGUILayout.PropertyField (list.FindProperty ("jetPack"));	
		EditorGUILayout.PropertyField (list.FindProperty ("playerElementsTransform"));	
		EditorGUILayout.PropertyField (list.FindProperty ("currentCharacterModel"));	

		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Place New Character Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("layerToPlaceNPC"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Build Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("buildPlayerType"));
		EditorGUILayout.PropertyField (list.FindProperty ("hasWeaponsEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Build Player Manually", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("assignBonesManually"));
		if (list.FindProperty ("assignBonesManually").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("newCharacterModel"));

			if (list.FindProperty ("newCharacterModel").objectReferenceValue) {
				GUILayout.Label ("Top Part");
				EditorGUILayout.PropertyField (list.FindProperty ("head"));
				EditorGUILayout.PropertyField (list.FindProperty ("neck"));
				EditorGUILayout.PropertyField (list.FindProperty ("chest"));
				EditorGUILayout.PropertyField (list.FindProperty ("spine"));

				EditorGUILayout.Space ();

				GUILayout.Label ("Middle Part");
				EditorGUILayout.PropertyField (list.FindProperty ("rightLowerArm"));
				EditorGUILayout.PropertyField (list.FindProperty ("leftLowerArm"));
				EditorGUILayout.PropertyField (list.FindProperty ("rightHand"));
				EditorGUILayout.PropertyField (list.FindProperty ("leftHand"));

				EditorGUILayout.Space ();

				GUILayout.Label ("Lower Part");
				EditorGUILayout.PropertyField (list.FindProperty ("rightLowerLeg"));
				EditorGUILayout.PropertyField (list.FindProperty ("leftLowerLeg"));
				EditorGUILayout.PropertyField (list.FindProperty ("rightFoot"));
				EditorGUILayout.PropertyField (list.FindProperty ("leftFoot"));
				EditorGUILayout.PropertyField (list.FindProperty ("rightToes"));
				EditorGUILayout.PropertyField (list.FindProperty ("leftToes"));
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Bones On New Character")) {
				if (!Application.isPlaying) {
					manager.getCharacterBones ();
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Build Character")) {
			if (!Application.isPlaying) {
				manager.buildCharacterByButton ();
			}
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}
}
#endif