using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(moveCameraToDevice))]
[CanEditMultipleObjects]
public class moveCameraToDeviceEditor : Editor
{
	moveCameraToDevice manager;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (moveCameraToDevice)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				if (manager.cameraPosition) {
					Handles.Label (manager.cameraPosition.transform.position, "Camera \n position", style);
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraMovementActive"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraPosition"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("smoothCameraMovement"));
		if (objectToUse.FindProperty ("smoothCameraMovement").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraMovementSpeed"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("secondMoveCameraToDevice"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("unlockCursor"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disablePlayerMeshGameObject"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disablePlayerHUDWhileUsing"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("alignPlayerWithCameraPositionOnStopUseDevice"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepWeaponsIfCarrying"));
		if (objectToUse.FindProperty ("keepWeaponsIfCarrying").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawWeaponsIfPreviouslyCarrying"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepOnlyIfPlayerIsOnFirstPerson"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableWeaponsCamera"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("carryWeaponOnLowerPositionActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		GUILayout.BeginVertical ("Camera Rotation On Exit Settings", "window");
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setPlayerCameraRotationOnExit"));
		if (objectToUse.FindProperty ("setPlayerCameraRotationOnExit").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerCameraTransformThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerPivotTransformThirdPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerCameraTransformFirstPerson"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerPivotTransformFirstPerson"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLenght"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLineLenght"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowAngle"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowColor"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}
}
#endif