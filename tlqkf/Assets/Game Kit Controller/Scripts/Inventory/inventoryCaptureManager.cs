using UnityEngine;
using System.Collections;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;

using UnityEditor.SceneManagement;

public class inventoryCaptureManager : EditorWindow
{
	public Vector2 captureResolution = new Vector2 (1024, 1024);
	public Vector3 rotationOffset;
	public Vector3 positionOffset;
	public string fileName = "New Capture";

	bool checkCapturePath;
	string currentSaveDataPath;
	inventoryListManager inventory;
	Camera inventoryCamera;
	static inventoryCaptureManager window;
	inventoryInfo currentInventoryInfo;
	GameObject currentInventoryObjectMesh;
	Rect renderRect;
	Transform inventoryLookObjectTransform;
	RenderTexture originalRenderTexture;
	Texture2D captureFile;
	GUISkin guiSkin;
	Vector2 rectSize = new Vector2 (510, 650);
	Color backGroundColor = Color.white;
	Color originalBackGroundColor;
	float cameraFov;
	float originalCameraFov;

	static void ShowWindow ()
	{
		window = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
		window.init ();
	}

	public void init ()
	{
		inventory = Selection.activeGameObject.GetComponent<inventoryListManager> ();
		inventoryCamera = inventory.inventoryCamera;
		inventoryLookObjectTransform = inventory.lookObjectsPosition;
		captureFile = null;
		checkCapturePath = false;
		originalBackGroundColor = inventoryCamera.backgroundColor;
		originalCameraFov = inventoryCamera.fieldOfView;
		cameraFov = originalCameraFov;
	}

	public void OnDisable ()
	{
		if (currentInventoryObjectMesh) {
			destroyAuxObjects ();
			inventoryCamera.backgroundColor = originalBackGroundColor;
			inventoryCamera.fieldOfView = originalCameraFov;
		}
	}

	void OnGUI ()
	{
		if (window == null) {
			window = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
		}

		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		if (inventoryCamera) {       
			inventoryCamera.Render ();
			renderRect = new Rect (position.width / 4, 360, position.width / 2, position.height / 2.2f);
			GUI.DrawTexture (renderRect, inventoryCamera.targetTexture);       
		}

		this.minSize = rectSize;
		this.titleContent = new GUIContent ("Inventory Object Capture Tool", null, "You can create inventory objects prefabs with this tool");
		GUILayout.BeginVertical ("Inventory Object Capture Tool", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("box");

		//GUILayout.Label ("Inventory Object Capture Tool", EditorStyles.boldLabel);
		captureResolution = EditorGUILayout.Vector2Field ("Capture Resolution", captureResolution);

		fileName = EditorGUILayout.TextField ("File Name", fileName);
		GUILayout.Label ("Rotation Offset", EditorStyles.boldLabel);
		rotationOffset.x = EditorGUILayout.Slider (rotationOffset.x, 0, 360);
		rotationOffset.y = EditorGUILayout.Slider (rotationOffset.y, 0, 360);
		rotationOffset.z = EditorGUILayout.Slider (rotationOffset.z, 0, 360);

		GUILayout.Label ("Position Offset", EditorStyles.boldLabel);
		positionOffset.x = EditorGUILayout.Slider (positionOffset.x, -5, 5);
		positionOffset.y = EditorGUILayout.Slider (positionOffset.y, -5, 5);
		positionOffset.z = EditorGUILayout.Slider (positionOffset.z, -5, 5);
		if (currentInventoryObjectMesh) {
			currentInventoryObjectMesh.transform.localRotation = Quaternion.Euler (rotationOffset);
			currentInventoryObjectMesh.transform.localPosition = positionOffset;
		}

		backGroundColor = EditorGUILayout.ColorField ("Background Color", backGroundColor);
		inventoryCamera.backgroundColor = backGroundColor;

		cameraFov = EditorGUILayout.FloatField ("Camera FOV", cameraFov);
		inventoryCamera.fieldOfView = cameraFov;

		if (GUILayout.Button ("Reset View")) {
			rotationOffset = Vector3.zero;
			positionOffset = Vector3.zero;
		}

		if (GUILayout.Button ("Get Capture")) {
			getCapture ();
		}
			
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	public void getCapture ()
	{
		if (currentInventoryObjectMesh == null) {
			Debug.Log ("Please, close this window, assign the Inventory Object Mesh and open this window again to take the capture");
			return;
		}

		if (fileName == "") {
			fileName = currentInventoryInfo.Name;
		}
			
		originalRenderTexture = inventoryCamera.targetTexture;
		inventoryCamera.targetTexture = new RenderTexture ((int)captureResolution.x, (int)captureResolution.y, 24);
		RenderTexture rendText = RenderTexture.active;
		RenderTexture.active = inventoryCamera.targetTexture;

		// render the texture
		inventoryCamera.Render ();
		// create a new Texture2D with the camera's texture, using its height and width
		Texture2D cameraImage = new Texture2D ((int)captureResolution.x, (int)captureResolution.y, TextureFormat.RGB24, false);
		cameraImage.ReadPixels (new Rect (0, 0, (int)captureResolution.x, (int)captureResolution.y), 0, 0);
		cameraImage.Apply ();
		RenderTexture.active = rendText;
		// store the texture into a .PNG file
		#if !UNITY_WEBPLAYER
		byte[] bytes = cameraImage.EncodeToPNG ();
		// save the encoded image to a file
		System.IO.File.WriteAllBytes (currentSaveDataPath + (fileName + " (Inventory Capture).png"), bytes);
		inventoryCamera.targetTexture = originalRenderTexture;
		#endif

		AssetDatabase.Refresh ();
		checkCapturePath = true;

		inventoryCamera.backgroundColor = originalBackGroundColor;
		inventoryCamera.fieldOfView = originalCameraFov;
	}
		
	void Update ()
	{
		if (checkCapturePath) {
			captureFile = (Texture2D)AssetDatabase.LoadAssetAtPath ((currentSaveDataPath + fileName + " (Inventory Capture).png"), typeof(Texture2D));
			if (captureFile) {
				inventory.setInventoryCaptureIcon (currentInventoryInfo, captureFile);
				checkCapturePath = false;
				closeWindow ();
			}
		}
	}

	public void closeWindow ()
	{
		if (currentInventoryObjectMesh) {
			destroyAuxObjects ();
		}
		window.Close ();
	}

	public void destroyAuxObjects ()
	{
		DestroyImmediate (currentInventoryObjectMesh);
	}

	public void setCurrentInventoryObjectInfo (inventoryInfo info, string savePath)
	{
		currentInventoryInfo = info;
		if (currentInventoryInfo.inventoryGameObject) {
			currentInventoryObjectMesh = (GameObject)Instantiate (info.inventoryGameObject, inventoryLookObjectTransform.position, Quaternion.identity);
			currentInventoryObjectMesh.transform.SetParent (inventoryLookObjectTransform);
			currentSaveDataPath = savePath;
		} else {
			Debug.Log ("Please, assign the Inventory Object Mesh to take the capture");
		}
	}
}
#endif