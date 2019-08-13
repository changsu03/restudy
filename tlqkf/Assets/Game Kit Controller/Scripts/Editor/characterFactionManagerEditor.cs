using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(characterFactionManager))]
public class characterFactionManagerEditor : Editor
{
	characterFactionManager manager;
	SerializedObject list;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (characterFactionManager)target;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindProperty ("factionManager"));

		EditorGUILayout.Space ();

		if (list.FindProperty ("factionStringList").arraySize > 0) {
			list.FindProperty ("factionIndex").intValue = EditorGUILayout.Popup ("Faction ", list.FindProperty ("factionIndex").intValue, manager.getFactionStringList ());
			list.FindProperty ("factionName").stringValue = manager.factionStringList [list.FindProperty ("factionIndex").intValue];
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Faction List")) {
			manager.getFactionList ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}
}
#endif