using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(decalTypeInformation))]
[CanEditMultipleObjects]
public class decalTypeInformationEditor : Editor
{
	SerializedObject list;
	decalTypeInformation decalType;
	GUIStyle style = new GUIStyle ();

	bool settings;
	bool advancedSettings;
	Color defBackgroundColor;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		decalType = (decalTypeInformation)target;
	}

	void OnSceneGUI ()
	{   
		if (decalType.showGizmo) {
			style.normal.textColor = decalType.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			Handles.Label (decalType.transform.position + decalType.transform.up, decalType.impactDecalName, style);	
		}
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Impact Surface Settings", "window", GUILayout.Height (30));
		if (list.FindProperty ("impactDecalList").arraySize > 0) {
			list.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Decal Impact Type", 
				list.FindProperty ("impactDecalIndex").intValue, decalType.impactDecalList);
			list.FindProperty ("impactDecalName").stringValue = decalType.impactDecalList [list.FindProperty ("impactDecalIndex").intValue];
		}

		EditorGUILayout.PropertyField (list.FindProperty ("parentScorchOnThisObject"));

		EditorGUILayout.PropertyField (list.FindProperty ("getImpactListEveryFrame"));
		if (!list.FindProperty ("getImpactListEveryFrame").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Decal Impact List")) {
				decalType.getImpactListInfo ();					
			}

			EditorGUILayout.Space ();

		}
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}
}
#endif