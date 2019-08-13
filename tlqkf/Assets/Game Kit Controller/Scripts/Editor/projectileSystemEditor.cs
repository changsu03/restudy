using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(projectileSystem))]
public class projectileSystemEditor : Editor
{
	SerializedObject list;

	void OnEnable ()
	{
		list = new SerializedObject (target);
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
		EditorGUILayout.PropertyField (list.FindProperty ("armorSurfaceLayer"));
		EditorGUILayout.PropertyField (list.FindProperty ("abilityFunctionNameAtStart"));
		EditorGUILayout.PropertyField (list.FindProperty ("abilityFunctionName"));
		EditorGUILayout.PropertyField (list.FindProperty ("useCustomValues"));
		EditorGUILayout.PropertyField (list.FindProperty ("fakeProjectileTrail"));
		GUILayout.EndVertical ();

		if (list.FindProperty ("useCustomValues").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Settings", "window", GUILayout.Height (30));

			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.projectileDamage"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.projectileSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.killInOneShot"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.projectileWithAbility"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.isHommingProjectile"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.isSeeker"));
			if (list.FindProperty ("currentProjectileInfo.isHommingProjectile").boolValue || list.FindProperty ("currentProjectileInfo.isSeeker").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.waitTimeToSearchTarget"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Force Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.impactForceApplied"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.forceMode"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.applyImpactForceToVehicles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Explosion Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.isExplosive"));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.isImplosive"));
			if (list.FindProperty ("currentProjectileInfo.isExplosive").boolValue || list.FindProperty ("currentProjectileInfo.isImplosive").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.explosionForce"));
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.explosionRadius"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Projectile Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.useDisableTimer"));
			if (list.FindProperty ("currentProjectileInfo.useDisableTimer").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.noImpactDisableTimer"));
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.impactDisableTimer"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sound Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.impactSoundEffect"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particle Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.impactParticles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Scorch Settings", "window", GUILayout.Height (30));
			//EditorGUILayout.Space ();
//		GUILayout.BeginVertical("Scorch from Decal Manager", "window",GUILayout.Height(30));
//		if (objectToUse.FindProperty ("impactDecalList").arraySize > 0) {
//			objectToUse.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
//				objectToUse.FindProperty ("impactDecalIndex").intValue, weapon.impactDecalList);
//			objectToUse.FindProperty ("impactDecalName").stringValue = weapon.impactDecalList [objectToUse.FindProperty ("impactDecalIndex").intValue];
//		}
//
//		EditorGUILayout.PropertyField (list.FindProperty ("getImpactListEveryFrame"));
//		if (!list.FindProperty ("getImpactListEveryFrame").boolValue) {
//
//			EditorGUILayout.Space ();
//
//			if (GUILayout.Button ("Update Decal Impact List")) {
//				weapon.getImpactListInfo ();					
//			}
//
//			EditorGUILayout.Space ();
//
//		}
			//GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regular Scorch", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.scorch"));
			if (list.FindProperty ("currentProjectileInfo.scorch").objectReferenceValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.targetForScorchLayer"));
				EditorGUILayout.PropertyField (list.FindProperty ("currentProjectileInfo.scorchRayCastDistance"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useEventOnImpact"));
		if (list.FindProperty ("useEventOnImpact").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("eventOnImpact"));
			EditorGUILayout.PropertyField (list.FindProperty ("useLayerMaskImpact"));
			if (list.FindProperty ("useLayerMaskImpact").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("layerMaskImpact"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("sendObjectDetectedOnImpactEvent"));
			if (list.FindProperty ("sendObjectDetectedOnImpactEvent").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("objectDetectedOnImpactEvent"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explosion Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useEventOnExplosion"));
		if (list.FindProperty ("useEventOnExplosion").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("evenOnExplosion"));
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