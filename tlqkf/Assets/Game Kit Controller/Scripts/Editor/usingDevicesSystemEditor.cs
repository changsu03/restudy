using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(usingDevicesSystem))]
[CanEditMultipleObjects]
public class usingDevicesSystemEditor : Editor
{
	SerializedObject list;
	string currentVehicle;
	string usingDevices;
	string currentDeviceFound;

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

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("canUseDevices"));
		EditorGUILayout.PropertyField (list.FindProperty ("touchButton"));
		EditorGUILayout.PropertyField (list.FindProperty ("iconButton"));
		EditorGUILayout.PropertyField (list.FindProperty ("iconButtonRectTransform"));
		EditorGUILayout.PropertyField (list.FindProperty ("actionText"));
		EditorGUILayout.PropertyField (list.FindProperty ("keyText"));
		EditorGUILayout.PropertyField (list.FindProperty ("objectNameText"));
		EditorGUILayout.PropertyField (list.FindProperty ("useDeviceFunctionName"));
		EditorGUILayout.PropertyField (list.FindProperty ("setCurrentUserOnDeviceFunctionName"));

		EditorGUILayout.PropertyField (list.FindProperty ("examinateDevicesCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("usePickUpAmountIfEqualToOne"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Device Icon Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("showUseDeviceIconEnabled"));
		if (list.FindProperty ("showUseDeviceIconEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useFixedDeviceIconPosition"));
			if (list.FindProperty ("useFixedDeviceIconPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("deviceOnScreenIfUseFixedIconPosition"));
			}
		}

		EditorGUILayout.PropertyField (list.FindProperty ("useDeviceButtonEnabled"));
		EditorGUILayout.PropertyField (list.FindProperty ("getClosestDeviceToCameraCenter"));
		if (list.FindProperty ("getClosestDeviceToCameraCenter").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("useMaxDistanceToCameraCenter"));
			if (list.FindProperty ("useMaxDistanceToCameraCenter").boolValue) {
				EditorGUILayout.PropertyField (list.FindProperty ("maxDistanceToCameraCenter"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Raycast Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("layer"));
		EditorGUILayout.PropertyField (list.FindProperty ("raycastDistance"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tag For Devices List", "window");
		showTagsForDevicesList (list.FindProperty ("tagsForDevices"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Interaction Message Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("interactionMessageGameObject"));
		EditorGUILayout.PropertyField (list.FindProperty ("interactionMessageText"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Outline Shader Device Found Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("useDeviceFoundShader"));
		if (list.FindProperty ("useDeviceFoundShader").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("deviceFoundShader"));
			EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineWidth"));
			EditorGUILayout.PropertyField (list.FindProperty ("shaderOutlineColor"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hold Button To Pick Around Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("holdButtonToTakePickupsAround"));
		if (list.FindProperty ("holdButtonToTakePickupsAround").boolValue) {
			EditorGUILayout.PropertyField (list.FindProperty ("holdButtonTime"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Devices State", "window");
		GUILayout.Label ("Icon Can Be Shown\t" + list.FindProperty ("iconButtonCanBeShown").boolValue);

		currentVehicle = "NONE";

		if (list.FindProperty ("currentVehicle").objectReferenceValue) {
			currentVehicle = list.FindProperty ("currentVehicle").objectReferenceValue.name;
		}
		GUILayout.Label ("Current Vehicle\t" + currentVehicle);

		GUILayout.Label ("Driving\t\t" + list.FindProperty ("driving").boolValue);

		usingDevices = "NO";

		if (list.FindProperty ("objectToUse").objectReferenceValue) {
			usingDevices = "YES";
		}
		GUILayout.Label ("Device Detected\t" + usingDevices);

		currentDeviceFound = "None";

		if (list.FindProperty ("objectToUse").objectReferenceValue) {
			currentDeviceFound = list.FindProperty ("currenDeviceActionName").stringValue;
		}

		GUILayout.Label ("Device Name\t" + currentDeviceFound);

		GUILayout.Label ("Device Is Pickup \t" + list.FindProperty ("currentDeviceIsPickup").boolValue);

		GUILayout.Label ("Examining Object \t" + list.FindProperty ("examiningObject").boolValue);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Devices List", "window");
		showDeviceList (list.FindProperty ("deviceGameObjectList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Elements", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("grabObjectsManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("ragdollManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerInput"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCamera"));
		EditorGUILayout.PropertyField (list.FindProperty ("playerCameraManager"));
		EditorGUILayout.PropertyField (list.FindProperty ("mainCameraTransform"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindProperty ("usedByAI"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			list.ApplyModifiedProperties ();
		}
	}

	void showTagsForDevicesList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Number Of Tags: \t" + list.arraySize.ToString ());

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tag")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
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
	}

	void showDeviceList (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list);
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Devices: \t" + list.arraySize.ToString ());
		
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
	
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif