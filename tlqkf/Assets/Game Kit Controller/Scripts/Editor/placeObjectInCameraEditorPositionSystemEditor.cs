using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(placeObjectInCameraEditorPositionSystem))]
public class placeObjectInCameraEditorPositionSystemEditor : Editor
{
	placeObjectInCameraEditorPositionSystem manager;

	void OnEnable ()
	{
		manager = (placeObjectInCameraEditorPositionSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Move Object To Camera Position")) {
			manager.moveObjects ();
		}

		if (GUILayout.Button ("Rotate Object To Left")) {
			manager.rotateObject (-1);
		}

		if (GUILayout.Button ("Rotate Object To Right")) {
			manager.rotateObject (1);
		}

		if (GUILayout.Button ("Reset Rotation")) {
			manager.resetObjectRotation ();
		}
	}
}
#endif