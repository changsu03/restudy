using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(moveDeviceToCamera))]
[CanEditMultipleObjects]
public class moveDeviceToCameraEditor : Editor
{
	SerializedObject objectToUse;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("deviceGameObject"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("smoothCameraMovement"));
		if (objectToUse.FindProperty ("smoothCameraMovement").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraMovementSpeed"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("distanceFromCamera"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerToExaminateDevices"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateExaminateObjectSystem"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectHasActiveRigidbody"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disablePlayerMeshGameObject"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxZoomDistance"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("minZoomDistance"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("zoomSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Collider List Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("deviceTrigger"));
		showSimpleList (objectToUse.FindProperty ("colliderListToDisable"), "Collider");
		showSimpleList (objectToUse.FindProperty ("colliderListButtons"), "Collider");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Disabled Object List Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useListOfDisabledObjects"));
		if (objectToUse.FindProperty ("useListOfDisabledObjects").boolValue) {
			showSimpleList (objectToUse.FindProperty ("disabledObjectList"), "Object");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepWeaponsIfCarrying"));
		if (objectToUse.FindProperty ("keepWeaponsIfCarrying").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("drawWeaponsIfPreviouslyCarrying"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("keepOnlyIfPlayerIsOnFirstPerson"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add "+ listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif