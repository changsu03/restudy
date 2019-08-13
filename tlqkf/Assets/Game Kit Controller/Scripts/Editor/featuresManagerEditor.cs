using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add a button in the features manager script inspector
[CustomEditor (typeof(featuresManager))]
public class featuresManagerEditor : Editor
{
	featuresManager manager;

	void OnEnable ()
	{
		manager = (featuresManager)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Configuration")) {
			manager.setConfiguration ();
		}
		if (GUILayout.Button ("Get Current Configuration")) {
			manager.getConfiguration ();
		}

		EditorGUILayout.Space ();
	}
}
#endif