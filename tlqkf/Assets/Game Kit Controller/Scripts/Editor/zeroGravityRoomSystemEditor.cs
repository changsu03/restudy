using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(zeroGravityRoomSystem))]
public class zeroGravityRoomSystemEditor : Editor
{
	zeroGravityRoomSystem manager;
	SerializedObject objectToUse;
	string labelText;
	GUIStyle style = new GUIStyle ();

	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform waypoint;
	string currentName; 

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		manager = (zeroGravityRoomSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				if (manager.roomPointsList.Count > 0) {
					style.normal.textColor = manager.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;
					for (int i = 0; i < manager.roomPointsList.Count; i++) {
						if (manager.roomPointsList [i]) {
							waypoint = manager.roomPointsList [i];

							Handles.Label (waypoint.position, waypoint.name.ToString (), style);

							if (manager.useHandleForWaypoints) {
								currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? waypoint.rotation : Quaternion.identity;

								EditorGUI.BeginChangeCheck ();

								oldPoint = waypoint.position;
								oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (waypoint, "move waypoint" + i.ToString ());
									waypoint.position = oldPoint;
								}
							}
						}
					}
				}

				currentName = manager.gameObject.name.ToString ();
				Handles.Label (manager.roomCenter, currentName, style);

				Handles.Label (manager.highestPointPoisition.position, "Highest \n position", style);
				Handles.Label (manager.lowestPointPosition.position, "Lowest \n position", style);
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
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("roomHasRegularGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("roomHasZeroGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("gravityDirectionTransform"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Affected Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectsAffectedByGravity"));
		if (objectToUse.FindProperty ("objectsAffectedByGravity").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeGravityForceForObjects"));
			if (objectToUse.FindProperty ("changeGravityForceForObjects").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("newGravityForceForObjects"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Characters Affected Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("charactersAffectedByGravity"));
		if (objectToUse.FindProperty ("charactersAffectedByGravity").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("changeGravityForceForCharacters"));
			if (objectToUse.FindProperty ("changeGravityForceForCharacters").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("newGravityForceForCharacters"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addForceToObjectsOnZeroGravityState"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("forceAmountToObjectOnZeroGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("forceDirectionToObjectsOnZeroGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("forceModeToObjectsOnZeroGravity"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addInitialForceToObjectsOnZeroGravityState"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("initialForceToObjectsOnZeroGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("initialForceRadiusToObjectsOnZeroGravity"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("initialForcePositionToObjectsOnZeroGravity"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tags To Affect", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("playerTag"));
		showGameObjectList (objectToUse.FindProperty ("nonAffectedObjectsTagList"), "No Affected Objects Tag List", "Tag");
		showGameObjectList (objectToUse.FindProperty ("charactersTagList"), "Characters Tag List", "Tag");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Room Points List", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("highestPointPoisition"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("lowestPointPosition"));

		EditorGUILayout.Space ();
		showRoomPointList (objectToUse.FindProperty ("roomPointsList"), "Room Points List");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Characters Inside Room List", "window", GUILayout.Height (30));
		showGameObjectList (objectToUse.FindProperty ("charactersInsideList"), "Characters Inside Room List", "Characters");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Inside Room List", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("addObjectsInsideParent"));
		if (objectToUse.FindProperty ("addObjectsInsideParent").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectsInsideParent"));
		}

		EditorGUILayout.Space ();

		showGameObjectList (objectToUse.FindProperty ("objectsInsideList"), "Objects Inside Room List", "Objects");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSounds"));
		if (objectToUse.FindProperty ("useSounds").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("mainAudioSource"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("regularGravitySound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("zeroGravitySound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("customGravitySound"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useSoundsOnCharacters"));
			if (objectToUse.FindProperty ("useSoundsOnCharacters").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundOnEntering"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("soundOnExiting"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("debugModeActive"));
		if (objectToUse.FindProperty ("debugModeActive").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("debugModeListActive"));

			showObjectsInfoList (objectToUse.FindProperty ("objectInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("centerGizmoScale"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("roomCenterColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("linesColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForWaypoints"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			objectToUse.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showGameObjectList (SerializedProperty list, string listName, string objectsTypeName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + objectsTypeName + ": \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showRoomPointList (SerializedProperty list, string listName)
	{
		EditorGUILayout.PropertyField (list, new GUIContent (listName), false);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				manager.addRoomPoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllRoomPoints ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Rename Rooom Points")) {
				manager.renameRoomPoints ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""));
				}
				if (GUILayout.Button ("x")) {
					manager.removeRoomPoint (i);
				}
				if (GUILayout.Button ("+")) {
					manager.addRoomPoint (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showObjectsInfoList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Objects: \t" + list.arraySize.ToString ());
	
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
						showObjectsInfoListElement (list.GetArrayElementAtIndex (i));
					}
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
					
				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showObjectsInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInside"));
		GUILayout.EndVertical ();
	}
}
#endif