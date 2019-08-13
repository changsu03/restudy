using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add a button in the features manager script inspector
[CustomEditor (typeof(camera2_5dZoneLimitSystem))]
[CanEditMultipleObjects]
public class camera2_5dZoneLimitSystemEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		camera2_5dZoneLimitSystem manager = (camera2_5dZoneLimitSystem)target;
		DrawDefaultInspector ();
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Set Current Configuration")) {
			manager.setConfigurationToPlayer ();
		}
		EditorGUILayout.Space ();
	}
}
#endif