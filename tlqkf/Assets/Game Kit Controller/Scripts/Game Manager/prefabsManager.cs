using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class prefabsManager : MonoBehaviour
{
	public string prefabsUrl = "Assets/Game Kit Controller/Prefabs/";
	public List<prefabTypeInfo> prefabTypeInfoList = new List<prefabTypeInfo> ();
	public LayerMask layerToPlaceObjects;

	//add function to get automatically the prefabs in an specific folder

	public void placePrefabInScene (int typeIndex, int prefabIndex)
	{
		#if UNITY_EDITOR
		Vector3 positionToInstantiate = Vector3.zero;
		if (SceneView.lastActiveSceneView) {
			if (SceneView.lastActiveSceneView.camera) {
				Camera currentCameraEditor = SceneView.lastActiveSceneView.camera;
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;
				RaycastHit hit;
				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceObjects)) {
					positionToInstantiate = hit.point + Vector3.up * 0.1f;
				}
			}
		}
		GameObject prefabToInstantiate = prefabTypeInfoList [typeIndex].prefabInfoList [prefabIndex].prefabGameObject;

		if (prefabToInstantiate) {
			GameObject newCameraTransformElement = (GameObject)Instantiate (prefabToInstantiate, positionToInstantiate, Quaternion.identity);
			newCameraTransformElement.transform.position = positionToInstantiate;
		} else {
			print("WARNING: prefab gameObject is empty, make sure it is assigned correctly");
		}
		#endif
	}

	public void addPrefab (int prefabTypeIndex)
	{
		prefabInfo newPrefabInfo = new prefabInfo ();
		newPrefabInfo.Name = "New Prefab";
		prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Add (newPrefabInfo);
		updateComponent ();
	}

	public void addPrefabType ()
	{
		prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();
		newPrefabTypeInfo.Name = "New Type";
		prefabTypeInfoList.Add (newPrefabTypeInfo);
		updateComponent ();
	}

	public string getPrefabPath (string prefabType, string prefabName)
	{
		string path = "";

		for (int i = 0; i < prefabTypeInfoList.Count; i++) {
			if (prefabTypeInfoList [i].Name == prefabType) {
				for (int j = 0; j < prefabTypeInfoList [i].prefabInfoList.Count; j++) {
					if (prefabTypeInfoList [i].prefabInfoList [j].Name == prefabName) {
						path = prefabsUrl + prefabTypeInfoList [i].url + "/" + prefabTypeInfoList [i].prefabInfoList [j].urlName;
					}
				}
			}
		}
		return path;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<prefabsManager> ());
		#endif
	}

	[System.Serializable]
	public class prefabTypeInfo
	{
		public string Name;
		public string url;
		public List<prefabInfo> prefabInfoList = new List<prefabInfo> ();
	}

	[System.Serializable]
	public class prefabInfo
	{
		public string Name;
		public string urlName;
		public GameObject prefabGameObject;
	}
}
