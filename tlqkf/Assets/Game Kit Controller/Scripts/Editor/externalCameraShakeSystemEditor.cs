using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(externalCameraShakeSystem))]
[CanEditMultipleObjects]
public class externalCameraShakeSystemEditor : Editor
{
	externalCameraShakeSystem shakeManager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		shakeManager = (externalCameraShakeSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (shakeManager.showGizmo) {
				
				style.normal.textColor = shakeManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				Handles.Label (shakeManager.transform.position + shakeManager.transform.up * shakeManager.minDistanceToShake, "External Shake: " + shakeManager.externalShakeName, style);
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
		if (objectToUse.FindProperty ("nameList").arraySize > 0) {
			objectToUse.FindProperty ("nameIndex").intValue = EditorGUILayout.Popup ("External Shake Type", objectToUse.FindProperty ("nameIndex").intValue, shakeManager.nameList);
			objectToUse.FindProperty ("externalShakeName").stringValue = shakeManager.nameList [objectToUse.FindProperty ("nameIndex").intValue];
		} 


		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useShakeListTriggeredByActions"));
		if (objectToUse.FindProperty ("useShakeListTriggeredByActions").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shake Triggered By Action List", "window", GUILayout.Height (30));
			showExternaShakeInfoList (objectToUse.FindProperty ("shakeTriggeredByActionList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		} else {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("shakeUsingDistance"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minDistanceToShake"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("layer"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useShakeEvent"));
			if (objectToUse.FindProperty ("useShakeEvent").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventAtStart"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventAtEnd"));
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
			if (objectToUse.FindProperty ("showGizmo").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update External Shake List")) {
			shakeManager.getExternalShakeList ();
		}
			
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Test Shake")) {
			if (Application.isPlaying) {
				shakeManager.setCameraShake ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showExternaShakeInfoList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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
						showExternalShakeElementInfo (list.GetArrayElementAtIndex (i));
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

	void showExternalShakeElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));
		if (shakeManager.nameList.Length > 0) {
			list.FindPropertyRelative ("nameIndex").intValue = EditorGUILayout.Popup ("External Shake Type", list.FindPropertyRelative ("nameIndex").intValue, shakeManager.nameList);
			list.FindPropertyRelative ("shakeName").stringValue = shakeManager.nameList [list.FindPropertyRelative ("nameIndex").intValue];

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeEvent"));
			if (list.FindPropertyRelative ("useShakeEvent").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventAtStart"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventAtEnd"));
			}
		} 
		GUILayout.EndVertical ();
	}
}
#endif