using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(IKDrivingSystem))]
public class IKDrivingSystemEditor : Editor
{
	SerializedObject list;
	IKDrivingSystem manager;
	bool settings;
	Color defBackgroundColor;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentTransform;
	bool expanded;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (IKDrivingSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				GUIStyle style = new GUIStyle ();
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (manager.useExplosionForceWhenDestroyed) {
					Handles.Label (manager.gameObject.transform.position + manager.gameObject.transform.up * manager.explosionRadius, 
						"Explosion Radius " + manager.explosionRadius.ToString () + "\n" + "Explosion Force " + manager.explosionForce, style);
				}

				for (int i = 0; i < manager.IKVehiclePassengersList.Count; i++) {
					if (manager.IKVehiclePassengersList [i].showIKPositionsGizmo) {
						for (int j = 0; j < manager.IKVehiclePassengersList [i].IKDrivingPos.Count; j++) {
							if (manager.IKVehiclePassengersList [i].IKDrivingPos [j].position) {
								Handles.Label (manager.IKVehiclePassengersList [i].IKDrivingPos [j].position.position, manager.IKVehiclePassengersList [i].IKDrivingPos [j].Name, style);	
							}
						}

						for (int j = 0; j < manager.IKVehiclePassengersList [i].IKDrivingKneePos.Count; j++) {
							if (manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].position) {
								Handles.Label (manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].position.position, manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].Name, style);	
							}
						}
					}

					if (manager.IKVehiclePassengersList [i].steerDirecion) {
						Handles.Label (manager.IKVehiclePassengersList [i].steerDirecion.position, "Steer Position " + i.ToString (), style);
					}

					if (manager.IKVehiclePassengersList [i].headLookDirection) {
						Handles.Label (manager.IKVehiclePassengersList [i].headLookDirection.position, "Head Look\n Direction " + i.ToString (), style);
					}

					if (manager.IKVehiclePassengersList [i].headLookPosition) {
						Handles.Label (manager.IKVehiclePassengersList [i].headLookPosition.position, "Head Look\n Position " + i.ToString (), style);
					}

					Handles.color = manager.handleGizmoColor;

					if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition) {
						Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition.position, "Right Get \n Off Ray " + i.ToString (), style);

						EditorGUI.BeginChangeCheck ();

						currentTransform = manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition;
						oldPoint = currentTransform.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Right Get Off Position Handle " + i.ToString ());
							currentTransform.position = newPoint;
						}   
					}

					if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition) {
						Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition.position, "Left Get \n Off Ray " + i.ToString (), style);

						EditorGUI.BeginChangeCheck ();

						currentTransform = manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition;
						oldPoint = currentTransform.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Left Get Off Position Handle " + i.ToString ());
							currentTransform.position = newPoint;
						}   
					}

					if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform) {
						Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform.position, manager.IKVehiclePassengersList [i].Name, style);

						EditorGUI.BeginChangeCheck ();

						currentTransform = manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform;
						oldPoint = currentTransform.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Seat Transform Position Handle " + i.ToString ());
							currentTransform.position = newPoint;
						}   
					}
				}
			}
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

		GUILayout.BeginVertical ("Driver Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicle"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicelCameraGameObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerIsAlwaysDriver"));
		EditorGUILayout.PropertyField (list.FindProperty ("hidePlayerFromNPCs"), new GUIContent ("Hide Player From NPCs"), false);
		EditorGUILayout.PropertyField (list.FindProperty ("playerVisibleInVehicle"));

		EditorGUILayout.PropertyField (list.FindProperty ("hidePlayerWeaponsWhileDriving"));

		EditorGUILayout.PropertyField (list.FindProperty ("canBeDrivenRemotely"));
		EditorGUILayout.PropertyField (list.FindProperty ("drawWeaponIfCarryingPreviously"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("resetCameraRotationWhenGetOn"));
		EditorGUILayout.PropertyField (list.FindProperty ("resetCameraRotationWhenGetOff"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Eject Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("ejectPlayerWhenDestroyed"));
		if (list.FindProperty ("ejectPlayerWhenDestroyed").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("ejectingPlayerForce"));
		}
		EditorGUILayout.PropertyField (list.FindProperty ("activateFreeFloatingModeOnEject"));	
		EditorGUILayout.PropertyField (list.FindProperty ("activateFreeFloatingModeOnEjectDelay"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explosion Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useExplosionForceWhenDestroyed"));
		if (list.FindProperty ("useExplosionForceWhenDestroyed").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("explosionRadius"));
			EditorGUILayout.PropertyField (list.FindProperty ("explosionForce"));
			EditorGUILayout.PropertyField (list.FindProperty ("explosionDamage"));
			EditorGUILayout.PropertyField (list.FindProperty ("pushCharactersOnExplosion"));

			EditorGUILayout.PropertyField (list.FindProperty ("applyExplosionForceToVehicles"));
			if (list.FindProperty ("applyExplosionForceToVehicles").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("explosionForceToVehiclesMultiplier"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("killObjectsInRadius"));
			EditorGUILayout.PropertyField (list.FindProperty ("forceMode"));
			EditorGUILayout.PropertyField (list.FindProperty ("useLayerMask"));
			if (list.FindProperty ("useLayerMask").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("layer"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("actionManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleCameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("HUDManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("currentVehicleWeaponSystem"));
		EditorGUILayout.PropertyField (list.FindProperty ("vehicleGravityManager"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		GUILayout.BeginVertical ("Shake Passengers Body Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("addCollisionForceDirectionToPassengers"));
		if (list.FindProperty ("addCollisionForceDirectionToPassengers").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("extraCollisionForceAmount"));
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("debugCollisionForce"));
			if (GUILayout.Button ("Simulate Collision")) {
				manager.setCollisionForceDirectionToPassengers (list.FindProperty ("debugCollisionForce").vector3Value);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		EditorGUILayout.PropertyField (list.FindProperty ("addAngularDirectionToPassengers"));
		if (list.FindProperty ("addAngularDirectionToPassengers").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("vehicleStabilitySpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("extraAngularDirectioAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Start Game In Vehicle Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("startGameInThisVehicle"));
		if (list.FindProperty ("startGameInThisVehicle").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("playerForVehicle"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle State", "window");
		GUILayout.Label ("Driving remotely\t" + list.FindProperty ("isBeingDrivenRemotely").boolValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("activateActionScreen"));
		if (list.FindProperty ("activateActionScreen").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("actionScreenName"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("useEventOnDriverGetOn"));
		if (list.FindProperty ("useEventOnDriverGetOn").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("eventOnDriverGetOn"));	
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
		if (list.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (list.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (list.FindProperty ("useHandleForVertex"));
			if (list.FindProperty ("useHandleForVertex").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("handleRadius"));
				EditorGUILayout.PropertyField (list.FindProperty ("handleGizmoColor"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		settings = list.FindProperty ("showSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (settings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Settings")) {
			settings = !settings;
		}
		GUI.backgroundColor = defBackgroundColor;

		EditorGUILayout.EndHorizontal ();
		if (settings) {
			EditorGUILayout.Space ();
			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("IK positions in vehicle", MessageType.None);
			GUI.color = Color.white;
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("IK Passengers List", "window", GUILayout.Height (50));
			showIKVehiclePassengersList (list.FindProperty ("IKVehiclePassengersList"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUI.backgroundColor = defBackgroundColor;
		GUILayout.EndVertical ();

		list.FindProperty ("showSettings").boolValue = settings;

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showIKDrivingInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (50));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showIKPositionsGizmo"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("IK Hint List", "window", GUILayout.Height (50));
		showIkHintList (list.FindPropertyRelative ("IKDrivingPos"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("IK Goals List", "window", GUILayout.Height (50));
		showIKGoalList (list.FindPropertyRelative ("IKDrivingKneePos"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Seat Info", "window", GUILayout.Height (50));
		showSeatInfo (list.FindPropertyRelative ("vehicleSeatInfo"));
		GUILayout.EndVertical ();

		if (list.FindPropertyRelative ("vehicleSeatInfo.isDriverSeat").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Body Look Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSteerDirection"));
			if (list.FindPropertyRelative ("useSteerDirection").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("steerDirecion"), new GUIContent ("Steer Direction"), false);
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHeadLookDirection"));
			if (list.FindPropertyRelative ("useHeadLookDirection").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("headLookDirection"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHeadLookPosition"));
			if (list.FindPropertyRelative ("useHeadLookPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("headLookPosition"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePlayerBodyOnCollision"));
			if (list.FindPropertyRelative ("shakePlayerBodyOnCollision").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerBodyParent"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("stabilitySpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeFadeSpeed"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirectionMinClamp"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirectionMaxClamp"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
			}
			GUILayout.EndVertical ();
		}
	}

	void showSeatInfo (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("seatTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rightGetOffPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftGetOffPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getOffDistance"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getOffPlace"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isDriverSeat"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Seat State", "window", GUILayout.Height (50));
		GUILayout.Label ("Seat Is Free\t" + list.FindPropertyRelative ("seatIsFree").boolValue.ToString ());
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentPassenger"));
		//EditorGUILayout.PropertyField (list.FindPropertyRelative ("seatIsFree"));
		GUILayout.EndVertical ();
	}

	void showUpperListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("Position Transform"), false);
		GUILayout.EndVertical ();
	}

	void showIkHintList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("IK Hint List"), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of IK Positions: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add IK Pos")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
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
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showUpperListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showLowerListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"), new GUIContent ("Name"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("knee"), new GUIContent ("Limb"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("Position Transform"), false);
		GUILayout.EndVertical ();
	}

	void showIKGoalList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list, new GUIContent ("IK Goal List"), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of IK Goals: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add IK Pos")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
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
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showLowerListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showIKVehiclePassengersList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Passengers: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Passenger")) {
				manager.addPassenger ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showIKDrivingInfo (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
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
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif