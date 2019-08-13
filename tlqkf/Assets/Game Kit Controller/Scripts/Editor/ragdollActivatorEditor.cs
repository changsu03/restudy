using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(ragdollActivator))]
[CanEditMultipleObjects]
public class ragdollActivatorEditor : Editor
{
	SerializedObject list;
	ragdollActivator manager;

	void OnEnable ()
	{
		list = new SerializedObject (targets);
		manager = (ragdollActivator)target;
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
		EditorGUILayout.PropertyField (list.FindProperty ("typeOfDeath"));
		EditorGUILayout.PropertyField (list.FindProperty ("timeToGetUp"));
	
		EditorGUILayout.PropertyField (list.FindProperty ("getUpDelay"));
		EditorGUILayout.PropertyField (list.FindProperty ("useDeathSound"));
		if (list.FindProperty ("useDeathSound").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("deathSound"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainAudioSource"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("getUpFromBellyName"));
		EditorGUILayout.PropertyField (list.FindProperty ("getUpFromBackName"));
		EditorGUILayout.PropertyField (list.FindProperty ("deathName"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		if (manager.typeOfDeath == ragdollActivator.deathType.ragdoll) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ragdoll Physics Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("ragdollToMecanimBlendTime"));
			EditorGUILayout.PropertyField (list.FindProperty ("layer"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxRagdollVelocity"));
			EditorGUILayout.PropertyField (list.FindProperty ("maxVelocityToGetUp"));
			EditorGUILayout.PropertyField (list.FindProperty ("extraForceOnRagdoll"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ragdoll Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("eventOnEnterRagdoll"));
		EditorGUILayout.PropertyField (list.FindProperty ("eventOnExitRagdoll"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ragdoll State", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("playerState"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentState"));
		GUILayout.Label ("On Ground\t\t" + list.FindProperty ("onGround").boolValue.ToString ());
		GUILayout.Label ("Can Move\t\t" + list.FindProperty ("canMove").boolValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Ignore List", "window");

		EditorGUILayout.Space ();

		showSimpleList (list.FindProperty ("objectsToIgnore"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Receive Damage On Impact Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("ragdollCanReceiveDamageOnImpact"));
		if (list.FindProperty ("ragdollCanReceiveDamageOnImpact").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minTimeToReceiveDamageOnImpact"));
			EditorGUILayout.PropertyField (list.FindProperty ("minVelocityToReceiveDamageOnImpact"));
			EditorGUILayout.PropertyField (list.FindProperty ("receiveDamageOnImpactMultiplier"));
			EditorGUILayout.PropertyField (list.FindProperty ("minTimToReceiveImpactDamageAgain"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("characterBody"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerCOM"));
		EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("statesManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("healthManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCollider"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainAnimator"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("cameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("gravityManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("IKSystemManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainRigidbody"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("stepsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("combatManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!list.FindProperty ("usedByAI").boolValue) {
			GUILayout.BeginVertical ("Player Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("timeToShowMenu"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}
			
		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		if (list.FindProperty ("usedByAI").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("tagForColliders"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Function")) {
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
				GUILayout.BeginHorizontal ();
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
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif