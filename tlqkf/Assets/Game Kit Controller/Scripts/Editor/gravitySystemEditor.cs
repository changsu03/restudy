using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(gravitySystem))]
public class gravitySystemEditor : Editor
{
	SerializedObject list;
	bool settings;
	Color buttonColor;

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

		GUILayout.BeginVertical ("Gravity Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("gravityPowerEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("liftToSearchEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("randomRotationOnAirEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("layer"));
		EditorGUILayout.PropertyField (list.FindProperty ("searchSurfaceSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("airControlSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("accelerateSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("highGravityMultiplier"));
		EditorGUILayout.PropertyField (list.FindProperty ("hoverSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("hoverAmount"));
		EditorGUILayout.PropertyField (list.FindProperty ("hoverSmooth"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotateToSurfaceSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("rotateToRegularGravitySpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("preserveVelocityWhenDisableGravityPower"));

		EditorGUILayout.PropertyField (list.FindProperty ("searchNewSurfaceOnHighFallSpeed"));
		if (list.FindProperty ("searchNewSurfaceOnHighFallSpeed").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minSpeedToSearchNewSurface"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("shakeCameraOnHighFallSpeed"));
		if (list.FindProperty ("shakeCameraOnHighFallSpeed").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("minSpeedToShakeCamera"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("checkSurfaceBelowOnRegularState"));
		if (list.FindProperty ("checkSurfaceBelowOnRegularState").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("timeToSetNullParentOnAir"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("currentNormal"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Circumnavigation Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("circumnavigateRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindProperty ("useLerpRotation"));
		EditorGUILayout.PropertyField (list.FindProperty ("tagForCircumnavigate"));
		EditorGUILayout.PropertyField (list.FindProperty ("tagForMovingObjects"));
		EditorGUILayout.PropertyField (list.FindProperty ("checkCircumnavigateSurfaceOnZeroGravity"));

		EditorGUILayout.PropertyField (list.FindProperty ("checkSurfaceBelowLedge"));
		if (list.FindProperty ("checkSurfaceBelowLedge").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("surfaceBelowLedgeRaycastDistance"));
			EditorGUILayout.PropertyField (list.FindProperty ("belowLedgeRotationSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("surfaceBelowRaycastTransform"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("checkSurfaceInFront"));
		if (list.FindProperty ("checkSurfaceInFront").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("surfaceInFrontRaycastDistance"));
			EditorGUILayout.PropertyField (list.FindProperty ("surfaceInFrontRotationSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("surfaceInFrontRaycastTransform"));
		}

		EditorGUILayout.PropertyField (list.FindProperty ("gravityAdherenceRaycastParent"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Mode Settings", "window");	
		EditorGUILayout.PropertyField (list.FindProperty ("startWithZeroGravityMode"));
		EditorGUILayout.PropertyField (list.FindProperty ("canResetRotationOnZeroGravityMode"));	
		EditorGUILayout.PropertyField (list.FindProperty ("canAdjustToForwardSurface"));

		if (list.FindProperty ("canAdjustToForwardSurface").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("forwardSurfaceRayPosition"));	
			EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToAdjust"));

			EditorGUILayout.PropertyField (list.FindProperty ("resetRotationZeroGravitySpeed"));	
			EditorGUILayout.PropertyField (list.FindProperty ("adjustToForwardSurfaceSpeed"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Free Floating Mode Settings", "window");	
		EditorGUILayout.PropertyField (list.FindProperty ("canActivateFreeFloatingMode"));	
		EditorGUILayout.PropertyField (list.FindProperty ("useEventsOnFreeFloatingModeStateChange"));
		if (list.FindProperty ("useEventsOnFreeFloatingModeStateChange").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("evenOnFreeFloatingModeStateEnabled"));
			EditorGUILayout.PropertyField (list.FindProperty ("eventOnFreeFloatingModeStateDisabled"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("New Gravity At Start Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("startWithNewGravity"));
		if (list.FindProperty ("startWithNewGravity").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("usePlayerRotation"));
			if (list.FindProperty ("usePlayerRotation").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("adjustRotationToSurfaceFound"));
			} else {
				EditorGUILayout.PropertyField (list.FindProperty ("newGravityToStart"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity Components", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("gravityCenter"));
		EditorGUILayout.PropertyField (list.FindProperty ("cursor"));
		EditorGUILayout.PropertyField (list.FindProperty ("arrow"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerRenderer"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity Color Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("changeModelColor"));
		if (list.FindProperty ("changeModelColor").boolValue) {
			showSimpleList (list.FindProperty ("materialToChange"));
			EditorGUILayout.PropertyField (list.FindProperty ("powerColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (settings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Elements Settings")) {
			settings = !settings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		if (settings) {
			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraGameObject"));
			EditorGUILayout.PropertyField (list.FindProperty ("pivotCameraTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("gravityCenterCollider"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("powers"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCollider"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainCamera"));
			EditorGUILayout.PropertyField (list.FindProperty ("weaponsManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainRigidbody"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
			EditorGUILayout.PropertyField (list.FindProperty ("grabObjectsManager"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Gravity State", "window");
		GUILayout.Label ("Gravity Power Active\t\t" + list.FindProperty ("gravityPowerActive").boolValue.ToString ());
		GUILayout.Label ("Power Activated\t\t" + list.FindProperty ("powerActivated").boolValue.ToString ());
		GUILayout.Label ("Choosing Direction\t\t" + list.FindProperty ("choosingDirection").boolValue.ToString ());
		GUILayout.Label ("Recalculating Surface\t\t" + list.FindProperty ("recalculatingSurface").boolValue.ToString ());
		GUILayout.Label ("Searching Surface\t\t" + list.FindProperty ("searchingSurface").boolValue.ToString ());
		GUILayout.Label ("Seraching New Surface Below\t" + list.FindProperty ("searchingNewSurfaceBelow").boolValue.ToString ());
		GUILayout.Label ("Searching Around \t\t" + list.FindProperty ("searchAround").boolValue.ToString ());
		GUILayout.Label ("First Person \t\t" + list.FindProperty ("firstPersonView").boolValue.ToString ());
		GUILayout.Label ("Zero Gravity Mode On \t" + list.FindProperty ("zeroGravityModeOn").boolValue.ToString ());
		GUILayout.Label ("Circumnavigate Surface Active \t" + list.FindProperty ("circumnavigateCurrentSurfaceActive").boolValue.ToString ());
		GUILayout.Label ("Free Floating Mode On \t" + list.FindProperty ("freeFloatingModeOn").boolValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Colors: " + list.arraySize.ToString ());
			EditorGUILayout.Space ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Color")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					EditorGUILayout.Space ();
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
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
		GUILayout.EndVertical ();
	}
}
#endif