using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(electronicDevice))]
public class electronicDeviceEditor : Editor
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useOnlyForTrigger"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("functionToSetPlayer"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useMoveCameraToDevice"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableDeviceWhenStopUsing"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("stopUsingDeviceWhenUnlock"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableAndRemoveDeviceWhenUnlock"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Free Interaction Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useFreeInteraction"));
		if (objectToUse.FindProperty ("useFreeInteraction").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useFreeInteractionEvent"));
			if (objectToUse.FindProperty ("useFreeInteractionEvent").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("freeInteractionEvent"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Device Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("functionToUseDevice"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Device State", "window", GUILayout.Height (30));
		GUILayout.Label ("Using Device\t\t" + objectToUse.FindProperty ("usingDevice").boolValue.ToString ());
		GUILayout.Label ("Device Can Be Used\t\t" + objectToUse.FindProperty ("deviceCanBeUsed").boolValue.ToString ());
		GUILayout.Label ("Player Inside\t\t" + objectToUse.FindProperty ("playerInside").boolValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event If Unable To Use Device Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateEventIfUnableToUseDevice"));
		if (objectToUse.FindProperty ("activateEventIfUnableToUseDevice").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("unableToUseDeviceEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Unlock Function Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("unlockFunctionCall"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Lock Function Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("lockFunctionCall"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Stay Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateEventOnTriggerStay"));
		if (objectToUse.FindProperty ("activateEventOnTriggerStay").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerStayEvent"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnTriggerStayRate"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Enter Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateEventOnTriggerEnter"));
		if (objectToUse.FindProperty ("activateEventOnTriggerEnter").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerEnterEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Exit Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateEventOnTriggerExit"));
		if (objectToUse.FindProperty ("activateEventOnTriggerExit").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerExitEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Send Current Player On Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("sendCurrentPlayerOnEvent"));
		if (objectToUse.FindProperty ("sendCurrentPlayerOnEvent").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("setCurrentPlayerEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Start/Stop Using Device Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnStartUsingDevice"));
		if (objectToUse.FindProperty ("useEventOnStartUsingDevice").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnStartUsingDevice"));
		}
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnStopUsingDevice"));
		if (objectToUse.FindProperty ("useEventOnStopUsingDevice").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventOnStopUsingDevice"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
		}
	}
}
#endif