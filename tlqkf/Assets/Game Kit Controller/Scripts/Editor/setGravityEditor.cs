using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(setGravity))]
public class setGravityEditor : Editor
{
	setGravity manager;
	SerializedObject objectToUse;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		manager = (setGravity)target;
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useWithPlayer"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useWithNPC"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useWithVehicles"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useWithAnyRigidbody"));
		if (objectToUse.FindProperty ("useWithAnyRigidbody").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("checkOnlyForArtificialGravitySystem"));
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("typeOfTrigger"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setGravityMode"));
		if (objectToUse.FindProperty ("setGravityMode").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("setRegularGravity"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCustomGravityDirection"));
		if (objectToUse.FindProperty ("useCustomGravityDirection").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("customGravityDirection"));
		} else {
			GUILayout.Label ("Default Gravity Direction is the UP axis of this transform");

			EditorGUILayout.Space ();

		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCenterPoint"));
		if (objectToUse.FindProperty ("useCenterPoint").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("centerPoint"));
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCenterPointForRigidbodies"));
		if (objectToUse.FindProperty ("useCenterPointForRigidbodies").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useInverseDirectionToCenterPoint"));
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeGravityDirectionActive"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("rotateToSurfaceSmoothly"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setCircumnavigateSurfaceState"));
		if (objectToUse.FindProperty ("setCircumnavigateSurfaceState").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("circumnavigateSurfaceState"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("preservePlayerVelocity"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("storeSetGravityManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Parent Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setTargetParent"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("setRigidbodiesParent"));
		if (objectToUse.FindProperty ("setTargetParent").boolValue || objectToUse.FindProperty ("setRigidbodiesParent").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("targetParent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useAnimation"));
		if (objectToUse.FindProperty ("useAnimation").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("animationName"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Center Point List Settings", "window", GUILayout.Height (30));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCenterPointList"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCenterIfPointListTooClose"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCenterPointListForRigidbodies"));

		EditorGUILayout.Space ();

		showRoomPointList (objectToUse.FindProperty ("centerPointList"), "Room Points List");
		GUILayout.EndVertical ();

		//EditorGUILayout.Space ();

//		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
//		if (objectToUse.FindProperty ("showGizmo").boolValue) {
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("centerGizmoScale"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("roomCenterColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("linesColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForWaypoints"));
//		}
//		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("In Game Options", "window", GUILayout.Height (30));

		if (GUILayout.Button ("Reverse Gravity Direction")) {
			if (Application.isPlaying) {
				manager.reverseGravityDirection ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showGameObjectList (SerializedProperty list, string listName, string objectsTypeName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + objectsTypeName + ": \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
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
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showRoomPointList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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