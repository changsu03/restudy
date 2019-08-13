using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(pickUpObject))]
public class pickUpObjectEditor : Editor
{
	SerializedObject list;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("pickType"));
		EditorGUILayout.PropertyField (list.FindProperty ("amount"));
		EditorGUILayout.PropertyField (list.FindProperty ("useAmountPerUnit"));
		if (list.FindProperty ("useAmountPerUnit").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("amountPerUnit"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("useSecondaryString"));
		if (list.FindProperty ("useSecondaryString").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("secondaryString"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("useTertiaryString"));
		if (list.FindProperty ("useTertiaryString").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("tertiaryString"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("pickUpSound"));
		EditorGUILayout.PropertyField (list.FindProperty ("staticPickUp"));
		EditorGUILayout.PropertyField (list.FindProperty ("moveToPlayerOnTrigger"));
		EditorGUILayout.PropertyField (list.FindProperty ("pickUpOption"));
		EditorGUILayout.PropertyField (list.FindProperty ("canBeExamined"));
		if (list.FindProperty ("canBeExamined").boolValue) {
			GUILayout.Label ("Examining Object\t" + list.FindProperty ("examiningObject").boolValue.ToString ());
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Icon Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showPickupInfoOnTaken"));

		EditorGUILayout.PropertyField (list.FindProperty ("usePickupIconOnScreen"));
		if (list.FindProperty ("usePickupIconOnScreen").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("pickupIconGeneralName"));
			EditorGUILayout.PropertyField (list.FindProperty ("pickupIconName"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("usePickupIconOnTaken"));
		if (list.FindProperty ("usePickupIconOnTaken").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("pickupIcon"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pick Up Used By", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("usableByAnything"));
		if (!list.FindProperty ("usableByAnything").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("usableByPlayer"));
			EditorGUILayout.PropertyField (list.FindProperty ("usableByVehicles"));
			EditorGUILayout.PropertyField (list.FindProperty ("usableByCharacters"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pick Up Taken Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useEventOnTaken"));
		if (list.FindProperty ("useEventOnTaken").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("eventOnTaken"));
			EditorGUILayout.PropertyField (list.FindProperty ("useEventOnRemainingAmount"));
			if (list.FindProperty ("useEventOnRemainingAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("eventOnRemainingAmount"));
			}
		}
		EditorGUILayout.PropertyField (list.FindProperty ("sendPickupFinder"));
		if (list.FindProperty ("sendPickupFinder").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("sendPickupFinderEvent"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}
}
#endif