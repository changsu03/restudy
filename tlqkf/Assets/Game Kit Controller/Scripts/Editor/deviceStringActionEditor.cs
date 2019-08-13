using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(deviceStringAction))]
public class deviceStringActionEditor : Editor
{
	deviceStringAction device;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();
	Vector3 labelPosition;
	Transform currentTransform;
	Quaternion currentRotation;
	Vector3 oldPoint;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		device = (deviceStringAction)target;
	}

	void OnSceneGUI ()
	{   
		if (device.showGizmo) {
			style.normal.textColor = device.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			labelPosition = device.gameObject.transform.position + device.gameObject.transform.up * device.actionOffset;
			if (device.useTransformForStringAction) {
				if (device.useSeparatedTransformForEveryView) {
					currentTransform = device.transformForThirdPerson;
					if (currentTransform) {
						currentRotation = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentTransform.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentRotation);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Third Person position Handle");
							currentTransform.position = oldPoint;
						}
					
						labelPosition = currentTransform.position;
						Handles.Label (labelPosition, "Third Person \n position", style);
					}

					currentTransform = device.transformForFirstPerson;
					if (currentTransform) {
						currentRotation = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentTransform.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentRotation);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Firts Person position Handle");
							currentTransform.position = oldPoint;
						}

						labelPosition = currentTransform.position;
						Handles.Label (labelPosition, "First Person \n position", style);
					}
				} else {
					if (device.transformForStringAction) {
						labelPosition = device.transformForStringAction.position;
						Handles.Label (labelPosition, "Device String \n position", style);
					}
				}
			} else {
				Handles.Label (labelPosition, "Device String \n position", style);
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

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("deviceName"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("deviceAction"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondaryDeviceAction"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("reloadDeviceActionOnPress"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hideIconOnPress"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableIconOnPress"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("hideIconOnUseDevice"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showIconOnStopUseDevice")); 

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showIcon"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showTouchIconButton"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useRaycastToDetectDeviceParts"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setUsingDeviceState"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Transform Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useTransformForStringAction"));
		if (objectToUse.FindProperty ("useTransformForStringAction").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSeparatedTransformForEveryView"));
			if (objectToUse.FindProperty ("useSeparatedTransformForEveryView").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("transformForThirdPerson"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("transformForFirstPerson"));
			} else {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("transformForStringAction"));
			}
		}
		GUILayout.EndVertical ();

		if (!objectToUse.FindProperty ("useTransformForStringAction").boolValue) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("actionOffset"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useLocalOffset"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		GUILayout.Label ("Is Icon Enabled\t" + objectToUse.FindProperty ("iconEnabled").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
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