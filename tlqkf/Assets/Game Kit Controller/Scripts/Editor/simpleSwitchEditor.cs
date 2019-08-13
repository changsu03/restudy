using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(simpleSwitch))]
public class simpleSwitchEditor : Editor
{
	SerializedObject objectToUse;
	simpleSwitch manager;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (simpleSwitch)target;
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("buttonEnabled"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("pressSound"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("sendCurrentUser"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("notUsableWhileAnimationIsPlaying"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSingleSwitch"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("switchAnimationName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("animationSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Function Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useUnityEvents"));
		if (objectToUse.FindProperty ("useUnityEvents").boolValue) {
			
			EditorGUILayout.Space ();
			if (objectToUse.FindProperty ("useSingleSwitch").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectToCallFunctions"));
			}else{
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("switchTurnedOn"));	

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (objectToUse.FindProperty ("turnOnEvent"));

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (objectToUse.FindProperty ("turnOffEvent"));
			}
		} else {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectToActive"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("activeFunctionName"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("sendThisButton"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Trigger Button Event")) {
			manager.triggerButtonEventFromEditor ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}
}
#endif