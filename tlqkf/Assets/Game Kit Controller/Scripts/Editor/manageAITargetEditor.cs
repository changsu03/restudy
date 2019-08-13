﻿using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add a button in the features manager script inspector
[CanEditMultipleObjects]
[CustomEditor (typeof(manageAITarget))]
public class manageAITargetEditor : Editor
{
	manageAITarget manager;

	float visionRange;
	Vector3 rangePosition;
	string text;

	Transform mainTransform;

	void OnEnable ()
	{
		manager = (manageAITarget)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo) {
			Handles.color = Color.white;

			mainTransform = manager.transform;

			rangePosition = mainTransform.position + mainTransform.up * 2;

			visionRange = manager.visionRange;

			//Handles.DrawWireArc (rangePosition, -manager.transform.up, manager.transform.right, visionRange, 2);
			Handles.DrawWireArc (rangePosition, mainTransform.up, mainTransform.forward, (visionRange / 2), 2);
			Handles.DrawWireArc (rangePosition, -mainTransform.up, mainTransform.forward, (visionRange / 2), 2);

			Vector3 viewAngleA = DirFromAngle (-visionRange / 2);
			Vector3 viewAngleB = DirFromAngle (visionRange / 2);

			Handles.DrawLine (rangePosition, rangePosition + viewAngleA * 2);
			Handles.DrawLine (rangePosition, rangePosition + viewAngleB * 2);

			string text = "Vision Range " + visionRange.ToString ();

			Handles.color = Color.red;
			Handles.Label (rangePosition, text);	
		}
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

	}

	public Vector3 DirFromAngle (float angleInDegrees)
	{
		angleInDegrees += manager.transform.eulerAngles.y;
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}
}
#endif