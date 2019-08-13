using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(lockedCameraSystem))]
public class lockedCameraSystemEditor : Editor
{
	lockedCameraSystem lockedCameraManager;
	SerializedObject objectToUse;
	string labelText;
	GUIStyle style = new GUIStyle ();
	lockedCameraSystem.cameraAxis currentcameraAxis;
	Vector2 rangeAngleX;
	Vector2 rangeAngleY;

	Vector2 rotationLimitsX;
	Vector2 rotationLimitsY;

	Vector2 moveCameraLimitsX;
	Vector2 moveCameraLimitsY;

	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform triggerTransform;
	Transform waypoint;
	Transform currentCameraAxis;

	void OnEnable ()
	{
		objectToUse = new SerializedObject (target);
		lockedCameraManager = (lockedCameraSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying || lockedCameraManager.showGizmoOnPlaying) {
			if (lockedCameraManager.showGizmo) {
				style.normal.textColor = lockedCameraManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				for (int i = 0; i < lockedCameraManager.lockedCameraList.Count; i++) {
					if (lockedCameraManager.lockedCameraList [i].showGizmo) {
						for (int j = 0; j < lockedCameraManager.lockedCameraList [i].cameraTransformList.Count; j++) {
							currentCameraAxis = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].axis;
							if (currentCameraAxis) {
								labelText = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].name + "\n";

								if (lockedCameraManager.lockedCameraList [i].setCameraToFree) {
									labelText += "Free";
								} else {
									labelText += "Locked";
								}
								Handles.Label (currentCameraAxis.position, labelText, style);	

								if (lockedCameraManager.showDoPositionHandles) {
									currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentCameraAxis.rotation : Quaternion.identity;

									EditorGUI.BeginChangeCheck ();

									oldPoint = currentCameraAxis.position;
									oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
									if (EditorGUI.EndChangeCheck ()) {
										Undo.RecordObject (currentCameraAxis, "move Locked Camera Axis Do Position Handle" + j.ToString ());
										currentCameraAxis.position = oldPoint;
									}
								}
							}
								
							for (int k = 0; k < lockedCameraManager.lockedCameraList [i].cameraTransformList [j].triggerTransform.Count; k++) {
								if (lockedCameraManager.lockedCameraList [i].cameraTransformList [j].triggerTransform [k]) {
									triggerTransform = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].triggerTransform [k];
									labelText = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].name + "\n" + "Trigger" + (k + 1).ToString ();
					
									Handles.Label (triggerTransform.position, labelText, style);	
									if (lockedCameraManager.useFreeHandle) {
										Handles.color = lockedCameraManager.handleGizmoColor;
										EditorGUI.BeginChangeCheck ();

										oldPoint = triggerTransform.position;
										newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, lockedCameraManager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
										if (EditorGUI.EndChangeCheck ()) {
											Undo.RecordObject (triggerTransform, "move Locked Camera Trigger Free Handle" + k.ToString ());
											triggerTransform.position = newPoint;
										}   
									}

									if (lockedCameraManager.showDoPositionHandles) {
										currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? triggerTransform.rotation : Quaternion.identity;

										EditorGUI.BeginChangeCheck ();

										oldPoint = triggerTransform.position;
										oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
										if (EditorGUI.EndChangeCheck ()) {
											Undo.RecordObject (triggerTransform, "move Locked Camera Trigger Do Position Handle" + k.ToString ());
											triggerTransform.position = oldPoint;
										}
									}
								}
							}

							if (lockedCameraManager.showWaypointList) {
								for (int k = 0; k < lockedCameraManager.lockedCameraList [i].cameraTransformList [j].waypointList.Count; k++) {
									if (lockedCameraManager.lockedCameraList [i].cameraTransformList [j].waypointList [k]) {
										waypoint = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].waypointList [k];
										labelText = lockedCameraManager.lockedCameraList [i].cameraTransformList [j].name + "\n" + "Waypoint " + (k + 1).ToString ();

										Handles.Label (waypoint.position, labelText, style);	
										if (lockedCameraManager.useFreeHandle) {
											Handles.color = lockedCameraManager.handleGizmoColor;
											EditorGUI.BeginChangeCheck ();

											oldPoint = waypoint.position;
											newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, lockedCameraManager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
											if (EditorGUI.EndChangeCheck ()) {
												Undo.RecordObject (waypoint, "move Locked Camera Waypoint Free Handle" + k.ToString ());
												waypoint.position = newPoint;
											}   
										}

										if (lockedCameraManager.showDoPositionHandles) {
											currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? waypoint.rotation : Quaternion.identity;

											EditorGUI.BeginChangeCheck ();

											oldPoint = waypoint.position;
											oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
											if (EditorGUI.EndChangeCheck ()) {
												Undo.RecordObject (waypoint, "move Locked Camera Waypoint Do Position Handle" + k.ToString ());
												waypoint.position = oldPoint;
											}
										}
									}
								}
							}
						}
					}
				}    
			}
		}
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("layerToPlaceNewCamera"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useRaycastToPlaceCameras"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("eventTriggerScale"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefab Elements", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraTransformPrefab"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("cameraTriggerPrefab"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmoOnPlaying"));	
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLenght"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowLineLenght"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowAngle"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoArrowColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("boundColor"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Free Handles Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useFreeHandle"));
			if (objectToUse.FindProperty ("useFreeHandle").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleRadius"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("handleGizmoColor"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showDoPositionHandles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Floors Triggers Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showFloorTriggersCubes"));	
			if (objectToUse.FindProperty ("showFloorTriggersCubes").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("floorTriggersCubesColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Waypoint List Settings", "window", GUILayout.Height (50));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showWaypointList"));	
			if (objectToUse.FindProperty ("showWaypointList").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("waypointListColor"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Gizmos")) {
				lockedCameraManager.setGizmosState (true);
			}
			if (GUILayout.Button ("Disable All Gizmos")) {
				lockedCameraManager.setGizmosState (false);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera List", "window", GUILayout.Height (30));
		showLockedCameraList (objectToUse.FindProperty ("lockedCameraList"));


		if (GUILayout.Button ("Update Triggers")) {
			lockedCameraManager.updateTriggers ();
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showLockedCameraList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Camera Zone: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add New Zone")) {
				lockedCameraManager.addNewCameraZone ();
			}
			if (GUILayout.Button ("Clear")) {
				lockedCameraManager.removeAllCameraZones ();
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showLockedCameraListElement (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					lockedCameraManager.removeCameraZone (i, true);
					//list.DeleteArrayElementAtIndex (i);
					//list.DeleteArrayElementAtIndex (i);
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
				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();
		}       
	}

	void showLockedCameraListElement (SerializedProperty list, int lockedCameraListIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("parentList"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentCameraTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setCameraToFree"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Parent Name")) {
			lockedCameraManager.setCameraParentListName (list.FindPropertyRelative ("name").stringValue, lockedCameraListIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera List", "window", GUILayout.Height (30));
		showCameraTransformList (list.FindPropertyRelative ("cameraTransformList"), lockedCameraListIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showCameraTransformList (SerializedProperty list, int lockedCameraListIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Cameras: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add New Camera")) {
				lockedCameraManager.addNewCameraTransformElememnt (lockedCameraListIndex);
			}
			if (GUILayout.Button ("Clear")) {
				lockedCameraManager.removeAllCameraTransformElements (lockedCameraListIndex);
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCameraTransformListElement (list.GetArrayElementAtIndex (i), lockedCameraListIndex, i);
						expanded = true;
					}
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					lockedCameraManager.removeNewCameraTransformElememnt (lockedCameraListIndex, i, true);
					//list.DeleteArrayElementAtIndex (i);
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
				if (GUILayout.Button ("C")) {
					lockedCameraManager.setEditorCameraPositionToLockedCamera (lockedCameraListIndex, i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showCameraTransformListElement (SerializedProperty list, int lockedCameraListIndex, int cameraTransformListIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("axis"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Camera to Start Game", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("startGameWithThisView"));
		if (list.FindPropertyRelative ("startGameWithThisView").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMultiplePlayers"));
			if (list.FindPropertyRelative ("useMultiplePlayers").boolValue) {
				showplayerForViewList (list.FindPropertyRelative ("playerForViewList"));
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerForView"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothTransitionAtStart"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Point & Click Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePointAndClickSystem"));
		if (list.FindPropertyRelative ("usePointAndClickSystem").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Point & Click")) {
				lockedCameraManager.setAllCamerasAsPointAndClickOrFree (true);
			}
			
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Free")) {
				lockedCameraManager.setAllCamerasAsPointAndClickOrFree (false);
			}

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tank Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTankControls"));
		if (list.FindPropertyRelative ("useTankControls").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Tank Controls")) {
				lockedCameraManager.setAllCamerasAsTankControlsOrRegular (true);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Free")) {
				lockedCameraManager.setAllCamerasAsTankControlsOrRegular (false);
			}

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Movement Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRelativeMovementToLockedCamera"));
		if (list.FindPropertyRelative ("useRelativeMovementToLockedCamera").boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Relative Movement")) {
				lockedCameraManager.setAllCamerasAsRelativeMovementsOrRegular (true);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Free")) {
				lockedCameraManager.setAllCamerasAsRelativeMovementsOrRegular (false);
			}

			EditorGUILayout.Space ();
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeRootMotionActive"));
		if (list.FindPropertyRelative ("changeRootMotionActive").boolValue) {

			if (GUILayout.Button ("Set All Cameras As Change Root Motion Enabled")) {
				lockedCameraManager.setAllCamerasAsChangeRootMotionEnabledOrDisabled (true);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Cameras As Change Root Motion Disabled")) {
				lockedCameraManager.setAllCamerasAsChangeRootMotionEnabledOrDisabled (false);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRootMotionActive"));
			if (list.FindPropertyRelative ("useRootMotionActive").boolValue) {

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set All Cameras As Use Root Motion Enabled")) {
					lockedCameraManager.setAllCamerasAsUseRootMotionEnabledOrDisabled (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set All Cameras As Use Root Motion Disabled")) {
					lockedCameraManager.setAllCamerasAsUseRootMotionEnabledOrDisabled (false);
				}
			}

		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fov Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDifferentCameraFov"));
		if (list.FindPropertyRelative ("useDifferentCameraFov").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fovValue"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger Camera List", "window", GUILayout.Height (30));
		showCameraTriggerList (list.FindPropertyRelative ("triggerTransform"), lockedCameraListIndex, cameraTransformListIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look At Target Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAimAssist"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookOnlyIfTargetOnScreen"));
		if (list.FindPropertyRelative ("lookOnlyIfTargetOnScreen").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookOnlyIfTargetVisible"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraCursorSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Follow Player Position Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("followPlayerPosition"));
		if (list.FindPropertyRelative ("followPlayerPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lockedCameraPivot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("followPlayerPositionSmoothly"));
			if (list.FindPropertyRelative ("followPlayerPositionSmoothly").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSeparatedVerticalHorizontalSpeed"));
				if (list.FindPropertyRelative ("useSeparatedVerticalHorizontalSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalFollowPlayerPositionSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalFollowPlayerPositionSpeed"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("followPlayerPositionSpeed"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useLerpToFollowPlayerPosition"));
			}

			if (!list.FindPropertyRelative ("lockedCameraPivot").objectReferenceValue) {

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Locked Camera Pivot")) {
					lockedCameraManager.addLockedCameraPivot (lockedCameraListIndex, cameraTransformListIndex);
				}

				EditorGUILayout.Space ();
			} else {

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useZeroCameraTransition"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Bounds Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoundToFollowPlayer"));
				if (list.FindPropertyRelative ("useBoundToFollowPlayer").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("heightBoundTop"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("widthBoundRight"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("widthBoundLeft"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("depthBoundFront"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("depthBoundBackward"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("boundOffset"));
				}

				GUILayout.EndVertical ();

				if (list.FindPropertyRelative ("use2_5dView").boolValue || list.FindPropertyRelative ("useTopDownView").boolValue) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Offset Settings", "window", GUILayout.Height (30));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHorizontalOffsetOnFaceSide"));
					if (list.FindPropertyRelative ("useHorizontalOffsetOnFaceSide").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalOffsetOnFaceSide"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("inputToleranceOnFaceSide"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalOffsetOnFaceSideSpeed"));
					}

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHorizontalOffsetOnFaceSideOnMoving"));
					if (list.FindPropertyRelative ("useHorizontalOffsetOnFaceSideOnMoving").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalOffsetOnFaceSideOnMoving"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("inputToleranceOnFaceSideOnMoving"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalOffsetOnFaceSideOnMovingSpeed"));
					}

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("use2_5dVerticalOffsetOnMove"));
					if (list.FindPropertyRelative ("use2_5dVerticalOffsetOnMove").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalTopOffsetOnMove"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalBottomOffsetOnMove"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalOffsetOnMoveSpeed"));					
					}
					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("2.5d Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("use2_5dView"));
				if (list.FindPropertyRelative ("use2_5dView").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDefaultZValue2_5d"));
					if (!list.FindPropertyRelative ("pivot2_5d").objectReferenceValue) {

						EditorGUILayout.Space ();

						if (GUILayout.Button ("Add 2.5d Camera and Pivot")) {
							lockedCameraManager.addLockedCameraPivot2_5d (lockedCameraListIndex, cameraTransformListIndex);
						}

						EditorGUILayout.Space ();
					} else {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivot2_5d"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveInXAxisOn2_5d"));

						EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraLimitsX2_5d"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraLimitsY2_5d"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("clampAimDirections"));
					if (list.FindPropertyRelative ("clampAimDirections").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfAimDirections"));

						EditorGUILayout.Space ();

						if (GUILayout.Button ("Set All Cameras As Use Clamp Aim Direction Enabled")) {
							lockedCameraManager.setAllCamerasAsUseClampAimDirectionEnabledOrDisabled (true);
						}

						EditorGUILayout.Space ();

						if (GUILayout.Button ("Set All Cameras As Use Clamp Aim Direction Disabled")) {
							lockedCameraManager.setAllCamerasAsUseClampAimDirectionEnabledOrDisabled (false);
						}
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Top Down Settings", "window", GUILayout.Height (30));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTopDownView"));
				if (list.FindPropertyRelative ("useTopDownView").boolValue) {
					if (!list.FindPropertyRelative ("topDownPivot").objectReferenceValue) {

						EditorGUILayout.Space ();

						if (GUILayout.Button ("Add Top Down Camera and Pivot")) {
							lockedCameraManager.addLockedCameraPivotTopDown (lockedCameraListIndex, cameraTransformListIndex);
						}

						EditorGUILayout.Space ();
					} else {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("topDownPivot"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("topDownLookDirection"));
					}

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTopDownYRotation"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraLimitsXTopDown"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraLimitsYTopDown"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("use8DiagonalAim"));

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("canRotateCameraHorizontally"));
					if (list.FindPropertyRelative ("canRotateCameraHorizontally").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalCameraRotationSpeed"));
					}

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkPossibleTargetsBelowCursor"));
				
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look At Player Position Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookAtPlayerPosition"));
		if (list.FindPropertyRelative ("lookAtPlayerPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookAtPlayerPositionSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lockedCameraPivot"));


			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRotationLimits"));
			if (list.FindPropertyRelative ("useRotationLimits").boolValue) {

				currentcameraAxis = lockedCameraManager.lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];
				rotationLimitsX = currentcameraAxis.rotationLimitsX;
				rotationLimitsY = currentcameraAxis.rotationLimitsY;

				GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				rotationLimitsX.x = EditorGUILayout.FloatField (rotationLimitsX.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref rotationLimitsX.x, ref rotationLimitsX.y, -180, 180);
				rotationLimitsX.y = EditorGUILayout.FloatField (rotationLimitsX.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				rotationLimitsY.x = EditorGUILayout.FloatField (rotationLimitsY.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref rotationLimitsY.x, ref rotationLimitsY.y, -180, 180);
				rotationLimitsY.y = EditorGUILayout.FloatField (rotationLimitsY.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				currentcameraAxis.rotationLimitsX = rotationLimitsX;
				currentcameraAxis.rotationLimitsY = rotationLimitsY;
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionOffset"));
			if (list.FindPropertyRelative ("usePositionOffset").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionOffset"));
			}

			if (!list.FindPropertyRelative ("lockedCameraPivot").objectReferenceValue) {

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Locked Camera Pivot")) {
					lockedCameraManager.addLockedCameraPivot (lockedCameraListIndex, cameraTransformListIndex);
				}

				EditorGUILayout.Space ();

			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Smooth Camera Transition Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothCameraTransition"));
		if (list.FindPropertyRelative ("smoothCameraTransition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraTransitionSpeed"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraCanRotate"));
		if (list.FindPropertyRelative ("cameraCanRotate").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothRotation"));
			if (list.FindPropertyRelative ("smoothRotation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSpringRotation"));
				if (list.FindPropertyRelative ("useSpringRotation").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("springRotationDelay"));
				}
			}

			currentcameraAxis = lockedCameraManager.lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];
			rangeAngleX = currentcameraAxis.rangeAngleX;
			rangeAngleY = currentcameraAxis.rangeAngleY;

			GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();
			rangeAngleX.x = EditorGUILayout.FloatField (rangeAngleX.x, GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref rangeAngleX.x, ref rangeAngleX.y, -180, 180);
			rangeAngleX.y = EditorGUILayout.FloatField (rangeAngleX.y, GUILayout.MaxWidth (50));
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();
			rangeAngleY.x = EditorGUILayout.FloatField (rangeAngleY.x, GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref rangeAngleY.x, ref rangeAngleY.y, -180, 180);
			rangeAngleY.y = EditorGUILayout.FloatField (rangeAngleY.y, GUILayout.MaxWidth (50));
			GUILayout.EndHorizontal ();

			currentcameraAxis.rangeAngleX = rangeAngleX;
			currentcameraAxis.rangeAngleY = rangeAngleY;

			if (!list.FindPropertyRelative ("lockedCameraPivot").objectReferenceValue) {

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Locked Camera Pivot")) {
					lockedCameraManager.addLockedCameraPivot (lockedCameraListIndex, cameraTransformListIndex);
				}

				EditorGUILayout.Space ();

			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseZoom"));
		if (list.FindPropertyRelative ("canUseZoom").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("zoomValue"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Move Camera Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canMoveCamera"));
		if (list.FindPropertyRelative ("canMoveCamera").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveCameraSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothCameraMovement"));
			if (list.FindPropertyRelative ("smoothCameraMovement").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothCameraSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSpringMovement"));
				if (list.FindPropertyRelative ("useSpringMovement").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("springMovementDelay"));
				}
			}
				
			currentcameraAxis = lockedCameraManager.lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];
			moveCameraLimitsX = currentcameraAxis.moveCameraLimitsX;
			moveCameraLimitsY = currentcameraAxis.moveCameraLimitsY;

			GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();
			moveCameraLimitsY.x = EditorGUILayout.FloatField (moveCameraLimitsY.x, GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref moveCameraLimitsY.x, ref moveCameraLimitsY.y, -20, 20);
			moveCameraLimitsY.y = EditorGUILayout.FloatField (moveCameraLimitsY.y, GUILayout.MaxWidth (50));
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();
			moveCameraLimitsX.x = EditorGUILayout.FloatField (moveCameraLimitsX.x, GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref moveCameraLimitsX.x, ref moveCameraLimitsX.y, -20, 20);
			moveCameraLimitsX.y = EditorGUILayout.FloatField (moveCameraLimitsX.y, GUILayout.MaxWidth (50));
			GUILayout.EndHorizontal ();
		
			currentcameraAxis.moveCameraLimitsX = moveCameraLimitsX;
			currentcameraAxis.moveCameraLimitsY = moveCameraLimitsY;

			if (!list.FindPropertyRelative ("lockedCameraPivot").objectReferenceValue) {

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Locked Camera Pivot")) {
					lockedCameraManager.addLockedCameraPivot (lockedCameraListIndex, cameraTransformListIndex);
				}

				EditorGUILayout.Space ();

			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoint Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWaypoints"));
		if (list.FindPropertyRelative ("useWaypoints").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointCameraRotationSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNextWaypointDirection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastToPlaceWaypoints"));
			showWaypointList (list.FindPropertyRelative ("waypointList"), lockedCameraListIndex, cameraTransformListIndex);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Transparent Surface Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTransparetSurfaceSystem"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUnityEvent"));
		if (list.FindPropertyRelative ("useUnityEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUnityEventOnEnter"));
			if (list.FindPropertyRelative ("useUnityEventOnEnter").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("unityEventOnEnter"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUnityEventOnExit"));
			if (list.FindPropertyRelative ("useUnityEventOnExit").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("unityEventOnExit"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Locked Camera To Editor Camera Position")) {
			lockedCameraManager.setLockedCameraToEditorCameraPosition (lockedCameraListIndex, cameraTransformListIndex);
		}

		if (GUILayout.Button ("Set Editor Camera To Locked Camera Position")) {
			lockedCameraManager.setEditorCameraPositionToLockedCamera (lockedCameraListIndex, cameraTransformListIndex);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Duplicate Camera")) {
			lockedCameraManager.duplicateLockedCamera (lockedCameraListIndex, cameraTransformListIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showCameraTriggerList (SerializedProperty list, int lockedCameraListIndex, int cameraTransformListIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Triggers: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add New Trigger")) {
				lockedCameraManager.addCameraTrigger (lockedCameraListIndex, cameraTransformListIndex);
			}
			if (GUILayout.Button ("Clear")) {
				lockedCameraManager.removeAllCameraTriggers (lockedCameraListIndex, cameraTransformListIndex);
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
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					lockedCameraManager.removeCameraTrigger (lockedCameraListIndex, cameraTransformListIndex, i, true);
				}
				if (GUILayout.Button ("O")) {
					lockedCameraManager.selectCurrentTrigger (lockedCameraListIndex, cameraTransformListIndex, i);
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
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showWaypointList (SerializedProperty list, int lockedCameraListIndex, int cameraTransformListIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Waypoints: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Waypoint")) {
				lockedCameraManager.addWaypoint (lockedCameraListIndex, cameraTransformListIndex);
			}
			if (GUILayout.Button ("Clear")) {
				lockedCameraManager.removeAllWaypoints (lockedCameraListIndex, cameraTransformListIndex);
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
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					lockedCameraManager.removeWaypoint (lockedCameraListIndex, cameraTransformListIndex, i, true);
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
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showplayerForViewList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Players: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Player")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
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
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}
}
#endif