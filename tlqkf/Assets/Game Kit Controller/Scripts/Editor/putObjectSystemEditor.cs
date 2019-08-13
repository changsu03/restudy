using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(putObjectSystem))]
public class putObjectSystemEditor : Editor
{
	SerializedObject objectToUse;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCertainObjectToPlace"));
		if (objectToUse.FindProperty ("useCertainObjectToPlace").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("certainObjectToPlace"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectNameToPlace"));


		EditorGUILayout.PropertyField (objectToUse.FindProperty ("placeToPutObject"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("placeObjectPositionSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("placeObjectRotationSpeed"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableObjectOnceIsPlaced"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Limit Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useRotationLimit"));
		if (objectToUse.FindProperty ("useRotationLimit").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxUpRotationAngle"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxForwardRotationAngle"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("usePositionLimit"));
		if (objectToUse.FindProperty ("usePositionLimit").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("maxPositionDistance"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Place Before Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("needsOtherObjectPlacedBefore"));
		if (objectToUse.FindProperty ("needsOtherObjectPlacedBefore").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("numberOfObjectsToPlaceBefore"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();	

		GUILayout.BeginVertical ("Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("waitToObjectPlacedToCallEvent"));

		EditorGUILayout.Space ();	

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectPlacedEvent"));

		EditorGUILayout.Space ();	

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectRemovedEvent"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Remove Object Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useLimitToCheckIfObjectRemoved"));
		if (objectToUse.FindProperty ("useLimitToCheckIfObjectRemoved").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("minDistanceToRemoveObject"));
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundEffectOnObjectPlaced"));
		if (objectToUse.FindProperty ("useSoundEffectOnObjectPlaced").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundEffectOnObjectPlaced"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundEffectOnObjectRemoved"));
		if (objectToUse.FindProperty ("useSoundEffectOnObjectRemoved").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundEffectOnObjectRemoved"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainAudioSource"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectInsideTrigger"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectPlaced"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentObjectPlaced"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("currentObjectToPlaceSystem"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("checkingIfObjectIsRemoved"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}
}
#endif