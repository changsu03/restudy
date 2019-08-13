using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(hideCharacterFixedPlaceSystem))]
public class hideCharacterFixedPlaceSystemEditor : Editor
{
	SerializedObject list;
	hideCharacterFixedPlaceSystem manager;

	Vector2 rangeAngleX;
	Vector2 rangeAngleY;
	Vector3 position;
	Transform cameraTransform;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (hideCharacterFixedPlaceSystem)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo) {
			Handles.color = Color.white;

			rangeAngleX = manager.rangeAngleX;
			rangeAngleY = manager.rangeAngleY;
			cameraTransform = manager.cameraPositionTransform;
			position = cameraTransform.position;

			Handles.DrawWireArc (position, -cameraTransform.up, cameraTransform.forward, -rangeAngleY.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (position, cameraTransform.up, cameraTransform.forward, rangeAngleY.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (position, cameraTransform.up, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleY.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (position, -cameraTransform.up, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleY.y)), manager.arcGizmoRadius);

			Handles.color = Color.white;
			Handles.DrawWireArc (position, -cameraTransform.right, cameraTransform.forward, -rangeAngleX.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (position, cameraTransform.right, cameraTransform.forward, rangeAngleX.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (position, cameraTransform.right, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleX.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (position, -cameraTransform.right, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleX.y)), manager.arcGizmoRadius);

			string text = "Camera Range\n" + "Y: " + (Mathf.Abs (rangeAngleX.x) + rangeAngleX.y) + "\n" + "X: " + (Mathf.Abs (rangeAngleY.x) + rangeAngleY.y);

			Handles.color = Color.red;
			Handles.Label (position + cameraTransform.up, text);	
		}
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("cameraTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("pivotTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("cameraPositionTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("canResetCameraRotation"));
		EditorGUILayout.PropertyField (list.FindProperty ("canResetCameraPosition"));
		EditorGUILayout.PropertyField (list.FindProperty ("useCharacterStateIcon"));
		EditorGUILayout.PropertyField (list.FindProperty ("visibleCharacterStateName"));
		EditorGUILayout.PropertyField (list.FindProperty ("notVisibleCharacterStateName"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hide System State", "window");
		GUILayout.Label ("Hidding Character\t " + list.FindProperty ("hidingCharacter").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("cameraCanRotate"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("smoothCameraRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("useSpringRotation"));
		if (list.FindProperty ("useSpringRotation").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("springRotationDelay"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Movement Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("cameraCanMove"));
		EditorGUILayout.PropertyField (list.FindProperty ("moveCameraSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("smoothMoveCameraSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("useSpringMovement"));
		if (list.FindProperty ("useSpringMovement").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("springMovementDelay"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hide/Visible Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("hidenEvent"));
		EditorGUILayout.PropertyField (list.FindProperty ("hideEventDelay"));
		EditorGUILayout.PropertyField (list.FindProperty ("visbleEvent"));
		EditorGUILayout.PropertyField (list.FindProperty ("visibleEventDelay"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rotation Range Settings", "window");

		GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.rangeAngleY.x = EditorGUILayout.FloatField (manager.rangeAngleY.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.rangeAngleY.x, ref manager.rangeAngleY.y, -180, 180);
		manager.rangeAngleY.y = EditorGUILayout.FloatField (manager.rangeAngleY.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.rangeAngleX.x = EditorGUILayout.FloatField (manager.rangeAngleX.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.rangeAngleX.x, ref manager.rangeAngleX.y, -180, 180);
		manager.rangeAngleX.y = EditorGUILayout.FloatField (manager.rangeAngleX.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Movement Range Settings", "window");

		GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.moveCameraLimitsY.x = EditorGUILayout.FloatField (manager.moveCameraLimitsY.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.moveCameraLimitsY.x, ref manager.moveCameraLimitsY.y, -3, 3);
		manager.moveCameraLimitsY.y = EditorGUILayout.FloatField (manager.moveCameraLimitsY.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.moveCameraLimitsX.x = EditorGUILayout.FloatField (manager.moveCameraLimitsX.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.moveCameraLimitsX.x, ref manager.moveCameraLimitsX.y, -3, 3);
		manager.moveCameraLimitsX.y = EditorGUILayout.FloatField (manager.moveCameraLimitsX.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera FOV Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("setHiddenFov"));
		if (list.FindProperty ("setHiddenFov").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("hiddenFov"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("zoomEnabled"));
		if (list.FindProperty ("zoomEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("zoomSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxZoom"));
			EditorGUILayout.PropertyField (list.FindProperty ("minZoom"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Unable To Hide Message Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("useMessageWhenUnableToHide"));
		if (list.FindProperty ("useMessageWhenUnableToHide").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("unableToHideMessage"));
			EditorGUILayout.PropertyField (list.FindProperty ("showMessageTime"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("activateActionScreen"));
		if (list.FindProperty ("activateActionScreen").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("actionScreenName"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("arcGizmoRadius"));
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowLenght"));
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowAngle"));
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoArrowColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}
}
#endif
