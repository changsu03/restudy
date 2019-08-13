using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(headTrack))]
public class headTrackEditor : Editor
{
	SerializedObject list;
	headTrack manager;

	Vector2 rangeAngleX;
	Vector2 rangeAngleY;
	Vector3 headPosition;

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (headTrack)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo && manager.useHeadRangeRotation) {
			Handles.color = Color.white;

			rangeAngleX = manager.rangeAngleX;
			rangeAngleY = manager.rangeAngleY;
			headPosition = manager.head.position;

			Handles.DrawWireArc (headPosition, -manager.transform.up, manager.transform.forward, -rangeAngleY.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, manager.transform.up, manager.transform.forward, rangeAngleY.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (headPosition, manager.transform.up, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleY.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, -manager.transform.up, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleY.y)), manager.arcGizmoRadius);

			Handles.color = Color.white;
			Handles.DrawWireArc (headPosition, -manager.transform.right, manager.transform.forward, -rangeAngleX.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, manager.transform.right, manager.transform.forward, rangeAngleX.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (headPosition, manager.transform.right, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleX.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, -manager.transform.right, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleX.y)), manager.arcGizmoRadius);

			string text = "Head Range\n"
			              + "Y: " + (Mathf.Abs (rangeAngleX.x) + rangeAngleX.y)
			              + "\n" + "X: " + (Mathf.Abs (rangeAngleY.x) + rangeAngleY.y);

			Handles.color = Color.red;
			Handles.Label (headPosition + manager.transform.up, text);	
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
		EditorGUILayout.PropertyField (list.FindProperty ("headTrackEnabled"));
		if (list.FindProperty ("headTrackEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("head"));
			EditorGUILayout.PropertyField (list.FindProperty ("headWeight"));
			EditorGUILayout.PropertyField (list.FindProperty ("bodyWeight"));
			EditorGUILayout.PropertyField (list.FindProperty ("rotationSpeed"));
			EditorGUILayout.PropertyField (list.FindProperty ("weightChangeSpeed"));	
			EditorGUILayout.PropertyField (list.FindProperty ("useTimeToChangeTarget"));
			if (list.FindProperty ("useTimeToChangeTarget").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("timeToChangeTarget"));
			}
			EditorGUILayout.PropertyField (list.FindProperty ("obstacleLayer"));

			EditorGUILayout.PropertyField (list.FindProperty ("useHeadTrackTarget"));
			if (list.FindProperty ("useHeadTrackTarget").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("headTrackTargeTransform"));
			}

			EditorGUILayout.Space ();

			if (!list.FindProperty ("head").objectReferenceValue) {
				if (GUILayout.Button ("Assign head")) {
					manager.searchHead ();
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look Camera Directon Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("lookInCameraDirection"));
		if (list.FindProperty ("lookInCameraDirection").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("cameraTargetToLook"));
			EditorGUILayout.PropertyField (list.FindProperty ("cameraHeadWeight"));
			EditorGUILayout.PropertyField (list.FindProperty ("cameraBodyWeight"));
			EditorGUILayout.PropertyField (list.FindProperty ("cameraRangeAngleX"));
			EditorGUILayout.PropertyField (list.FindProperty ("cameraRangeAngleY"));
			EditorGUILayout.PropertyField (list.FindProperty ("lookInOppositeDirectionOutOfRange"));	
			if (list.FindProperty ("lookInOppositeDirectionOutOfRange").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("oppositeCameraTargetToLook"));
				EditorGUILayout.PropertyField (list.FindProperty ("oppositeCameraTargetToLookParent"));
				EditorGUILayout.PropertyField (list.FindProperty ("oppositeCameraParentRotationSpeed"));
				EditorGUILayout.PropertyField (list.FindProperty ("lookBehindIfMoving"));
				if (list.FindProperty ("lookBehindIfMoving").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("lookBehindRotationSpeed"));
				}
				EditorGUILayout.PropertyField (list.FindProperty ("useDeadZone"));
				if (list.FindProperty ("useDeadZone").boolValue) {
					EditorGUILayout.PropertyField (list.FindProperty ("deadZoneLookBehind"));
				}
			}
		}
		GUILayout.EndVertical ();

		if (list.FindProperty ("headTrackEnabled").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Range Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("useHeadRangeRotation"));
			if (list.FindProperty ("useHeadRangeRotation").boolValue) {

				GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				manager.rangeAngleX.x = EditorGUILayout.FloatField (manager.rangeAngleX.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref manager.rangeAngleX.x, ref manager.rangeAngleX.y, -180, 180);
				manager.rangeAngleX.y = EditorGUILayout.FloatField (manager.rangeAngleX.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				manager.rangeAngleY.x = EditorGUILayout.FloatField (manager.rangeAngleY.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref manager.rangeAngleY.x, ref manager.rangeAngleY.y, -180, 180);
				manager.rangeAngleY.y = EditorGUILayout.FloatField (manager.rangeAngleY.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Targets To Ignore Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (list.FindProperty ("useTargetsToIgnoreList"));
			if (list.FindProperty ("useTargetsToIgnoreList").boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Targets To Ignore List", "window");
				showSimpleList (list.FindProperty ("targetsToIgnoreList"));
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Head Track State", "window");
			GUILayout.Label ("Can Look State\t" + list.FindProperty ("playerCanLookState").boolValue.ToString ());
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Element", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("animator"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("IKManager"));
			EditorGUILayout.PropertyField (list.FindProperty ("powersManager"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (list.FindProperty ("showGizmo"));
			if (list.FindProperty ("showGizmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("gizmoRadius"));
				EditorGUILayout.PropertyField (list.FindProperty ("arcGizmoRadius"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
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

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif
