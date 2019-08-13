using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapObjectInformation))]
[CanEditMultipleObjects]
public class mapObjectInformationEditor : Editor
{
	mapObjectInformation mapObject;
	SerializedObject objectToUse;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		objectToUse = new SerializedObject (targets);
		mapObject = (mapObjectInformation)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (mapObject.showGizmo) {
				if (mapObject.disableWhenPlayerHasReached) {
					style.normal.textColor = mapObject.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;
					Handles.Label (mapObject.transform.position + mapObject.transform.up * mapObject.triggerRadius + mapObject.transform.up * mapObject.gizmoLabelOffset, 
						"Objective: " + mapObject.gameObject.name, style);
				}

				if (mapObject.offsetShowGizmo) {
					style.normal.textColor = mapObject.offsetGizmoColor;
					style.alignment = TextAnchor.MiddleCenter;
					Handles.Label (mapObject.transform.position + mapObject.offset, "Offset \n Position", style);
				}
			}
			if (Selection.activeGameObject == mapObject.gameObject) {
				mapObject.getMapObjectInformation ();
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

		GUILayout.BeginVertical ("ID Settings", "window", GUILayout.Height (30));
		GUILayout.Label ("Map Object ID: " + objectToUse.FindProperty ("ID").intValue.ToString ());
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("name"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("description"));
		if (objectToUse.FindProperty ("typeNameList").arraySize > 0) {
			objectToUse.FindProperty ("typeIndex").intValue = EditorGUILayout.Popup ("Map Icon Type", objectToUse.FindProperty ("typeIndex").intValue, mapObject.typeNameList);
			objectToUse.FindProperty ("typeName").stringValue = mapObject.typeNameList [objectToUse.FindProperty ("typeIndex").intValue];
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("visibleInAllBuildings"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("visibleInAllFloors"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("calculateFloorAtStart"));

		if (objectToUse.FindProperty ("buildingList").arraySize > 0) {
			objectToUse.FindProperty ("buildingIndex").intValue = EditorGUILayout.Popup ("Building Number", objectToUse.FindProperty ("buildingIndex").intValue, mapObject.buildingList);
			if (objectToUse.FindProperty ("buildingIndex").intValue >= 0) {
				objectToUse.FindProperty ("currentBuilding").stringValue = mapObject.typeNameList [objectToUse.FindProperty ("buildingIndex").intValue];
			}
				
			if (objectToUse.FindProperty ("floorList").arraySize > 0) {
				objectToUse.FindProperty ("floorIndex").intValue = EditorGUILayout.Popup ("Floor Number", objectToUse.FindProperty ("floorIndex").intValue, mapObject.floorList);
				if (objectToUse.FindProperty ("floorIndex").intValue >= 0) {
					objectToUse.FindProperty ("currentFloor").stringValue = mapObject.typeNameList [objectToUse.FindProperty ("floorIndex").intValue];
				}
			}
		}

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCustomValues"));

		EditorGUILayout.PropertyField (objectToUse.FindProperty ("offset"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("extraIconSizeOnMap"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("followCameraRotation"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("canChangeBuildingAndFloor"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("activateAtStart"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map Values")) {
			mapObject.getMapObjectInformation ();
		}

		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Advanced Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("disableWhenPlayerHasReached"));
		if (objectToUse.FindProperty ("disableWhenPlayerHasReached").boolValue) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the Objective options", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCloseDistance"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showOffScreenIcon"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showMapWindowIcon"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("showDistance"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerRadius"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("triggerColor"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectiveOffset"));

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("removeComponentWhenObjectiveReached"));


			EditorGUILayout.PropertyField (objectToUse.FindProperty ("setCustomCompassSettings"));
			if (objectToUse.FindProperty ("setCustomCompassSettings").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCompassIcon"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("compassIconPrefab"));
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("verticalOffset"));
			}
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useCustomObjectiveColor"));
			if (objectToUse.FindProperty ("useCustomObjectiveColor").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("removeCustomObjectiveColor"));
				if (!objectToUse.FindProperty ("removeCustomObjectiveColor").boolValue) {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("objectiveColor"));
				}
			}
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Unity Event When Point Reached Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("callEventWhenPointReached"));
			if (objectToUse.FindProperty ("callEventWhenPointReached").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("pointReachedEvent"));
			}

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Part Owner Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("belongToMapPart"));
		if (objectToUse.FindProperty ("belongToMapPart").boolValue) {
			if (objectToUse.FindProperty ("mapPartList").arraySize > 0) {
				objectToUse.FindProperty ("mapPartIndex").intValue = EditorGUILayout.Popup ("Map Part Owner", objectToUse.FindProperty ("mapPartIndex").intValue, mapObject.mapPartList);
				if (objectToUse.FindProperty ("mapPartList").arraySize > objectToUse.FindProperty ("mapPartIndex").intValue) {
					objectToUse.FindProperty ("mapPartName").stringValue = mapObject.mapPartList [objectToUse.FindProperty ("mapPartIndex").intValue];
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floor Changed Events Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventsOnChangeFloor"));
		if (objectToUse.FindProperty ("useEventsOnChangeFloor").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnEnabledFloor"));
			if (objectToUse.FindProperty ("useEventOnEnabledFloor").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("evenOnEnabledFloor"));
			}
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useEventOnDisabledFloor"));
			if (objectToUse.FindProperty ("useEventOnDisabledFloor").boolValue) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("evenOnDisabledFloor"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("buildingIndex"));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("floorIndex"));
		if (GUILayout.Button ("Set Current Building/Floor Index")) {
			if (Application.isPlaying) {
				mapObject.setNewBuildingAndFloorIndexByInspector ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
		if (objectToUse.FindProperty ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelOffset"));
			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));

			if (objectToUse.FindProperty ("offset").vector3Value != Vector3.zero) {
				EditorGUILayout.PropertyField (objectToUse.FindProperty ("offsetShowGizmo"));
				if (objectToUse.FindProperty ("offsetShowGizmo").boolValue) {
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("offsetGizmoColor"));
					EditorGUILayout.PropertyField (objectToUse.FindProperty ("offsetRadius"));
				}
			}
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