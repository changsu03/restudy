using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapZoneUnlocker))]
public class mapZoneUnlockerEditor : Editor
{
	SerializedObject list;
	mapZoneUnlocker manager;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		list = new SerializedObject (target);
		manager = (mapZoneUnlocker)target;
		style.alignment = TextAnchor.MiddleLeft;
	}

	public override void OnInspectorGUI ()
	{
		if (list == null) {
			return;
		}
		list.Update ();
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building List", "window", GUILayout.Height (50));
		showBuldingList (list.FindProperty ("buildingList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showBuldingList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Buildings: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Building List")) {
				manager.searchBuildingList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear Building List")) {
				manager.clearAllBuildingList ();
			}

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
						showBuildingListInfo (list.GetArrayElementAtIndex (i), i);
					}
					EditorGUILayout.Space ();
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showBuildingListInfo (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building Floor List", "window", GUILayout.Height (50));
		showFloorList (list.FindPropertyRelative ("buildingFloorsList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFloorListInfo (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorEnabled"));

		if (list.FindPropertyRelative ("floorEnabled").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Map Parts List", "window", GUILayout.Height (50));
			showMapPartsList (list.FindPropertyRelative ("mapPartsList"), buildingIndex, floorIndex);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFloorList (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Floors: " + list.arraySize.ToString ());

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
						showFloorListInfo (list.GetArrayElementAtIndex (i), buildingIndex, i);
					}
					EditorGUILayout.Space ();
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showMapPartsList (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Parts: " + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Floor Parts")) {
				manager.enableOrDisableAllFloorParts (false, buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Enable All Floor Parts")) {
				manager.enableOrDisableAllFloorParts (true, buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			manager.initialIndex = EditorGUILayout.FloatField (Mathf.Round (manager.initialIndex), GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref manager.initialIndex, ref manager.finalIndex, 0, list.arraySize);
			manager.finalIndex = EditorGUILayout.FloatField (Mathf.Round (manager.finalIndex), GUILayout.MaxWidth (50));
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable Range")) {
				manager.enableOrDisableMapPartsRange (true, buildingIndex, floorIndex);
			}

			if (GUILayout.Button ("Disable Range")) {
				manager.enableOrDisableMapPartsRange (false, buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {

					string name = (i + 1).ToString ("000") + " ";
					name += "(" + list.GetArrayElementAtIndex (i).FindPropertyRelative ("mapPartName").stringValue + ")";
				
					bool mapPartEnabled = list.GetArrayElementAtIndex (i).FindPropertyRelative ("mapPartEnabled").boolValue;

					list.GetArrayElementAtIndex (i).FindPropertyRelative ("mapPartEnabled").boolValue = EditorGUILayout.Toggle (name, mapPartEnabled);
				}
				GUILayout.EndHorizontal ();
			}
		}
	}
}
#endif