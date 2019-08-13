using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class mapTileBuilder : MonoBehaviour
{
	public Transform mapPartParent;

	public int mapPartBuildingIndex;
	public int mapPartFloorIndex;
	public int mapPartIndex;

	public List<Transform> verticesPosition = new List<Transform> ();
	public List<GameObject> eventTriggerList = new List<GameObject> ();
	public List<GameObject> extraMapPartsToActive = new List<GameObject> ();
	public mapCreator mapManager;
	public float mapPartRendererOffset;
	public Vector2 newPositionOffset;
	public bool mapPartEnabled = true;
	public bool useOtherColorIfMapPartDisabled;
	public Color colorIfMapPartDisabled;

	public bool showGizmo = true;
	public bool showEnabledTrigger = true;
	public bool showVerticesDistance;
	public Color mapPartMaterialColor = Color.white;
	public Vector3 cubeGizmoScale = Vector3.one;
	public Color gizmoLabelColor = Color.white;
	public Color mapLinesColor = Color.yellow;
	public bool useHandleForVertex;
	public float handleRadius = 0.1f;

	public bool showVertexHandles;

	public List<GameObject> textMeshList = new List<GameObject> ();
	public Vector3 center;

	public string mapPartName;

	public string internalName;

	public bool generate3dMapPartMesh;
	public bool onlyUse3dMapPartMesh;
	public bool generate3dMeshesShowGizmo;
	public GameObject mapPart3dGameObject;

	public Vector3 mapPart3dOffset;
	public float mapPart3dHeight = 1;

	public bool mapPart3dMeshCreated;

	public GameObject mapTileRenderer;
	public MeshFilter mapTileMeshFilter;

	public bool mapTileCreated;
	public MeshRenderer mainMeshRenderer;

	GameObject eventTriggerParent;
	GameObject textMeshParent;

	MeshRenderer mapPart3dMeshRenderer;
	Color originalMapPart3dMeshRendererColor;

	Vector3 current3dOffset;

	public void createMapTileElement ()
	{
		if (!mapTileCreated) {

			calculateMapTileMesh ();

			generateMapPart3dMesh (mapPart3dHeight);

			mapTileCreated = true;
		}

		if (!mapPartEnabled) {
			if (useOtherColorIfMapPartDisabled) {
				setWallRendererMaterialColor (colorIfMapPartDisabled);
				enableOrDisableTextMesh (false);
			} else {
				disableMapPart ();
			}

			enableOrDisableMapPart3dMesh (false);
		} else {
			if (onlyUse3dMapPartMesh) {
				if (mapTileRenderer) {
					mapTileRenderer.SetActive (false);
				}
			}
		}
	}

	public void calculateMapTileMesh ()
	{
		List<Vector2> vertices2D = new List<Vector2> ();
		for (int i = 0; i < verticesPosition.Count; i++) {
			vertices2D.Add (new Vector2 (verticesPosition [i].localPosition.x, verticesPosition [i].localPosition.y));
		}

		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator (vertices2D);
		int[] triangles = tr.Triangulate ();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertices2D.Count];
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = new Vector3 (vertices2D [i].x, vertices2D [i].y, 0);
		}

		// Create the mesh
		Mesh msh = new Mesh ();
		msh.vertices = vertices;
		msh.triangles = triangles;
		msh.RecalculateNormals ();
		msh.RecalculateBounds ();

		// Set up game object with mesh;
		if (!mapTileRenderer) {
			mapTileRenderer = new GameObject ();
			mapTileRenderer.transform.SetParent (transform);
			mapTileRenderer.layer = LayerMask.NameToLayer (mapManager.mapLayer);
			mapTileRenderer.transform.localPosition = Vector3.zero;
			mapTileRenderer.transform.localPosition -= mapTileRenderer.transform.forward * mapPartRendererOffset;
			mapTileRenderer.transform.localRotation = Quaternion.identity;
			mapTileRenderer.name = "Map Tile Renderer";
			mapTileRenderer.AddComponent (typeof(MeshRenderer));
			Material newMaterial = new Material (mapManager.floorMaterial);
			mainMeshRenderer = mapTileRenderer.GetComponent<MeshRenderer> ();
			mainMeshRenderer.material = newMaterial;
		}

		if (!mapTileMeshFilter) {
			mapTileMeshFilter = mapTileRenderer.AddComponent (typeof(MeshFilter)) as MeshFilter;
		}

		mapTileMeshFilter.mesh = msh;

		setWallRendererMaterialColor (mapPartMaterialColor);

		mapTileCreated = true;
	}

	public void removeMapTileRenderer ()
	{
		if (mapTileRenderer) {
			DestroyImmediate (mapTileRenderer);
		}

		if (mapTileMeshFilter) {
			DestroyImmediate (mapTileMeshFilter);
		}

		if (mainMeshRenderer) {
			DestroyImmediate (mainMeshRenderer);
		}

		mapTileCreated = false;

		updateComponent ();
	}

	public void setWallRendererMaterialColor (Color newColor)
	{
		if (mainMeshRenderer) {
			if (Application.isPlaying) {
				mainMeshRenderer.material.color = newColor;
			} else {
				mainMeshRenderer.sharedMaterial.color = newColor;
			}
		}
	}

	public void set3dMapPartMaterialColor (Color newColor)
	{
		if (mapPart3dMeshRenderer) {
			mapPart3dMeshRenderer.material.color = newColor;
		}
	}

	public void setOriginal3dMapPartMaterialColor ()
	{
		if (mapPart3dMeshRenderer) {
			mapPart3dMeshRenderer.material.color = originalMapPart3dMeshRendererColor;
		}
	}

	public void enableMapPart ()
	{
		if (!mapPartEnabled) {
			if (!onlyUse3dMapPartMesh) {
				mapTileRenderer.SetActive (true);
			}
			enableOrDisableTextMesh (true);
			mapPartEnabled = true;

			mapManager.enableOrDisableSingleMapIconByMapPartIndex (mapPartBuildingIndex, mapPartIndex, mapPartFloorIndex, true);

			enableOrDisableEventTriggerList (false);

			for (int i = 0; i < extraMapPartsToActive.Count; i++) {
				extraMapPartsToActive [i].GetComponent<mapTileBuilder> ().enableMapPart ();
			}

			enableOrDisableMapPart3dMesh (true);
		}

		mapManager.setCurrentMapPartIndex (mapPartIndex);
	}

	public void enableOrDisableEventTriggerList (bool state)
	{
		for (int i = 0; i < eventTriggerList.Count; i++) {
			eventTriggerList [i].SetActive (state);
		}
	}

	public void disableMapPart ()
	{
		mapTileRenderer.SetActive (false);
		enableOrDisableTextMesh (false);
		mapPartEnabled = false;
	}

	public void setMapPartEnabledState (bool state)
	{
		mapPartEnabled = state;
		updateComponent ();
	}

	public void enableOrDisableTextMesh (bool state)
	{
		if (textMeshList.Count > 0) {
			for (int i = 0; i < textMeshList.Count; i++) {
				textMeshList [i].SetActive (state);
			}
			if (useOtherColorIfMapPartDisabled) {
				if (state) {
					setWallRendererMaterialColor (mapPartMaterialColor);
				} else {
					setWallRendererMaterialColor (colorIfMapPartDisabled);
				}
			}
		}
	}

	public void addEventTriggerToActive ()
	{
		if (eventTriggerList.Count == 0 || eventTriggerParent == null) {
			eventTriggerParent = new GameObject ();
			eventTriggerParent.name = "Triggers Parent";
			eventTriggerParent.transform.SetParent (transform);
			eventTriggerParent.transform.localPosition = Vector3.zero;
			eventTriggerParent.transform.localRotation = Quaternion.identity;
		}
		mapPartEnabled = false;
		GameObject trigger = new GameObject ();
		trigger.AddComponent<BoxCollider> ().isTrigger = true;
		trigger.AddComponent<eventTriggerSystem> ().setSimpleFunctionByTag ("enableMapPart", gameObject, "Player");
		trigger.transform.SetParent (eventTriggerParent.transform);
		if (mapManager.useRaycastToPlaceElements) {
			#if UNITY_EDITOR
			if (SceneView.lastActiveSceneView) {
				if (SceneView.lastActiveSceneView.camera) {
					Camera currentCameraEditor = SceneView.lastActiveSceneView.camera;
					Vector3 editorCameraPosition = currentCameraEditor.transform.position;
					Vector3 editorCameraForward = currentCameraEditor.transform.forward;
					RaycastHit hit;
					if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, mapManager.layerToPlaceElements)) {
						trigger.transform.position = hit.point + Vector3.up * 0.1f;
					}
				}
			}
			#endif
		} else {
			trigger.transform.localPosition = Vector3.zero;
		}
		trigger.transform.rotation = Quaternion.identity;
		if (mapManager.mapPartEnabledTriggerScale != Vector3.zero) {
			trigger.transform.localScale = mapManager.mapPartEnabledTriggerScale;
		}
		trigger.layer = LayerMask.NameToLayer ("Ignore Raycast");
		trigger.name = "Map Part Enabled Trigger " + (eventTriggerList.Count + 1).ToString ();
		eventTriggerList.Add (trigger);
	}

	public void removeEventTrigger (int eventTriggerIndex)
	{
		GameObject currentEventTrigger = eventTriggerList [eventTriggerIndex];
		if (currentEventTrigger) {
			DestroyImmediate (currentEventTrigger);
		}
		eventTriggerList.RemoveAt (eventTriggerIndex);
		if (eventTriggerList.Count == 0) {
			if (eventTriggerParent) {
				DestroyImmediate (eventTriggerParent);
			}
		}
		updateComponent ();
	}

	public void removeAllEventTriggers ()
	{
		for (int i = 0; i < eventTriggerList.Count; i++) {
			if (eventTriggerList [i]) {
				DestroyImmediate (eventTriggerList [i]);
			}
		}
		eventTriggerList.Clear ();
		if (eventTriggerParent) {
			DestroyImmediate (eventTriggerParent);
		}
		updateComponent ();
	}

	public void addMapPartTextMesh ()
	{
		#if UNITY_EDITOR
		GameObject textMesh = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/Game Kit Controller/Prefabs/Map System/mapPartTextMesh.prefab", typeof(GameObject));

		if (textMesh) {

			if (textMeshList.Count == 0 || textMeshParent == null) {
				textMeshParent = new GameObject ();
				textMeshParent.name = "Text Mesh Parent";
				textMeshParent.transform.SetParent (transform);
				textMeshParent.transform.localPosition = Vector3.zero;
				textMeshParent.transform.localRotation = Quaternion.identity;
			}

			textMesh = (GameObject)Instantiate (textMesh, transform.position, Quaternion.identity);
			textMesh.transform.SetParent (textMeshParent.transform);
			Vector3 textMeshPosition = transform.position;
			if (verticesPosition.Count > 0) {
				textMeshPosition = center;
			}
			textMesh.transform.position = textMeshPosition + textMesh.transform.forward;
			textMesh.transform.localRotation = Quaternion.identity;
			textMesh.name = "Map Part Text Mesh " + (textMeshList.Count + 1).ToString ("000");
			textMeshList.Add (textMesh);
		} else {
			print ("Prefab not found");
		}
		#endif
	}

	public void removeTextMesh (int textMeshIndex)
	{
		GameObject currentTextMesh = textMeshList [textMeshIndex];
		if (currentTextMesh) {
			DestroyImmediate (currentTextMesh);
		}
		textMeshList.RemoveAt (textMeshIndex);
		if (textMeshList.Count == 0) {
			if (textMeshParent) {
				DestroyImmediate (textMeshParent);
			}
		}
		updateComponent ();
	}

	public void removeAllTextMesh ()
	{
		for (int i = 0; i < textMeshList.Count; i++) {
			if (textMeshList [i]) {
				DestroyImmediate (textMeshList [i]);
			}
		}
		textMeshList.Clear ();
		if (textMeshParent) {
			DestroyImmediate (textMeshParent);
		}
		updateComponent ();
	}

	public void addNewVertex (int insertAtIndex)
	{
		GameObject newTransform = new GameObject ();
		newTransform.transform.SetParent (transform);
		newTransform.transform.localRotation = Quaternion.identity;
		if (verticesPosition.Count > 0) {
			Vector3 lastPosition = verticesPosition [verticesPosition.Count - 1].localPosition;
			newTransform.transform.localPosition = new Vector3 (lastPosition.x + newPositionOffset.x + 1, lastPosition.y + newPositionOffset.y + 1, lastPosition.z);
		} else {
			newTransform.transform.localPosition = Vector3.zero;
		}
		newTransform.name = (verticesPosition.Count + 1).ToString ("000");
		if (insertAtIndex > -1) {
			if (verticesPosition.Count > 0) {
				Vector3 vertexPosition = verticesPosition [insertAtIndex].localPosition;
				newTransform.transform.localPosition = new Vector3 (vertexPosition.x + newPositionOffset.x + 1, vertexPosition.y + newPositionOffset.y + 1, vertexPosition.z);
			}
			verticesPosition.Insert (insertAtIndex + 1, newTransform.transform);
			newTransform.transform.SetSiblingIndex (insertAtIndex + 1);
			renameAllVertex ();
		} else {
			verticesPosition.Add (newTransform.transform);
		}
		updateComponent ();
	}

	public void removeVertex (int vertexIndex)
	{
		Transform currentVertex = verticesPosition [vertexIndex];
		if (currentVertex) {
			DestroyImmediate (currentVertex.gameObject);
		}
		verticesPosition.RemoveAt (vertexIndex);
		updateComponent ();
	}

	public void removeAllVertex ()
	{
		for (int i = 0; i < verticesPosition.Count; i++) {
			if (verticesPosition [i]) {
				DestroyImmediate (verticesPosition [i].gameObject);
			}
		}
		verticesPosition.Clear ();
		updateComponent ();
	}

	public void renameAllVertex ()
	{
		for (int i = 0; i < verticesPosition.Count; i++) {
			if (verticesPosition [i]) {
				verticesPosition [i].name = (i + 1).ToString ("000");
			}
		}
		updateComponent ();
	}

	public void  reverVertexOrder ()
	{
		verticesPosition.Reverse ();
		updateComponent ();
	}

	public void setInternalName (string nameToConfigure)
	{
		internalName = nameToConfigure;
		renameMapPart ();
	}

	public void renameMapPart ()
	{
		string newName = internalName;
		if (mapPartName != "") {
			newName += " (" + mapPartName + ")";
		}
		gameObject.name = newName;

		updateComponent ();
	}

	public void updateMapPart3dMeshPositionFromEditor ()
	{
		updateMapPart3dMeshPosition (mapPart3dOffset);
	}

	public void updateMapPart3dMeshPosition (Vector3 offset)
	{
		if (mapPart3dMeshCreated) {
			mapPart3dGameObject.transform.position = center + offset;
		}
	}

	public void enableOrDisableMapPart3dMesh (bool state)
	{
		if (mapPart3dMeshCreated) {
			mapPart3dGameObject.SetActive (state);
		}
	}

	public void removeMapPart3dMesh ()
	{
		if (mapPart3dMeshCreated) {
			DestroyImmediate (mapPart3dGameObject);
			mapPart3dMeshCreated = false;
		}
	}

	public void setGenerate3dMapPartMeshState (bool state)
	{
		generate3dMapPartMesh = state;

		updateComponent ();
	}

	public void generateMapPart3dMeshFromEditor ()
	{
		generateMapPart3dMesh (mapPart3dHeight);
	}

	public void generateMapPart3dMesh (float meshHeight)
	{
		if ((!generate3dMapPartMesh && (!generate3dMapPartMesh && !mapManager.generateFull3dMapMeshes)) || !mapManager.generate3dMeshesActive) {
			return;
		}
			
		removeMapPart3dMesh ();

		mapPart3dMeshCreated = true;

		mapPart3dGameObject = new GameObject ();

		mapPart3dGameObject.name = gameObject.name + " - 3d Mesh";
		mapPart3dGameObject.isStatic = true;
		mapPart3dGameObject.layer = LayerMask.NameToLayer (mapManager.mapLayer);
		mapPart3dGameObject.transform.SetParent (transform);
		mapPart3dGameObject.transform.position = center + mapPart3dOffset;

		MeshRenderer mapPart3dRenderer = mapPart3dGameObject.AddComponent<MeshRenderer> ();
		mapPart3dMeshRenderer = mapPart3dRenderer;
		MeshFilter mapPart3dMeshFilter = mapPart3dGameObject.AddComponent<MeshFilter> ();
		Mesh mapPart3dMesh = mapPart3dMeshFilter.mesh;

		mapPart3dRenderer.material = mapManager.mapPart3dMeshMaterial;
		originalMapPart3dMeshRendererColor = mapPart3dMeshRenderer.material.color;

		mapPart3dMesh.Clear ();
		mapPart3dMesh.ClearBlendShapes ();


		Vector3 position1 = verticesPosition [0].position;
		Vector3 position2 = verticesPosition [1].position;

		Vector3 direction1 = center - position1;
		direction1 = direction1 / direction1.magnitude;
		Vector3 direction2 = center - position2;
		direction2 = direction2 / direction2.magnitude;

		float angle1 = Vector3.Angle (direction1, Vector3.forward);
		float angle2 = Vector3.Angle (direction2, Vector3.forward);

//		print (gameObject.name + " " + angle1 + " " + angle2);

		if (angle1 < angle2) {
			verticesPosition.Reverse ();
		} 


		Vector3[] downCorners = new Vector3[verticesPosition.Count];

		for (int x = 0; x < downCorners.Length; x++) {
			downCorners [x] = mapPart3dGameObject.transform.InverseTransformPoint (verticesPosition [x].position);
		}

		Vector3[] topCorners = new Vector3[verticesPosition.Count];
		downCorners.CopyTo (topCorners, 0);
		for (int x = 0; x < topCorners.Length; x++) {
			topCorners [x] += new Vector3 (0, meshHeight, 0);
		}

		Vector3[] cornersCombined = new Vector3[downCorners.Length + topCorners.Length];
		downCorners.CopyTo (cornersCombined, 0);
		topCorners.CopyTo (cornersCombined, downCorners.Length);

		mapPart3dMesh.vertices = cornersCombined;

		List<Vector2> downVertices2D = new List<Vector2> ();
		for (int i = 0; i < downCorners.Length; i++) {
			downVertices2D.Add (new Vector2 (downCorners [i].x, downCorners [i].z));
		}

		Triangulator donwTr = new Triangulator (downVertices2D);
		int[] downIndices = donwTr.Triangulate ();


		int[] reverseDownIndices = new int[downIndices.Length];
		downIndices.CopyTo (reverseDownIndices, 0);
		for (int x = 0; x < downIndices.Length - 1; x++) {
			int leftValue = downIndices [x];
			int rightValue = downIndices [x + 2];
			reverseDownIndices [x] = rightValue;
			reverseDownIndices [x + 2] = leftValue;
			x += 2;
		}
		reverseDownIndices.CopyTo (downIndices, 0);

		int[] middleIndices = new int[verticesPosition.Count * 6];
		int middleIndicesIndex = 0;
		for (int x = 0; x < verticesPosition.Count; x++) {
			int leftIndex = x;
			int rightIndex = x + 1;

			if (x == verticesPosition.Count - 1) {
				rightIndex = 0;
			}

			middleIndices [middleIndicesIndex] = rightIndex;
			middleIndicesIndex++;
			middleIndices [middleIndicesIndex] = leftIndex;
			middleIndicesIndex++;
			if (x == verticesPosition.Count - 1) {
				middleIndices [middleIndicesIndex] = (verticesPosition.Count * 2) - 1;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex + verticesPosition.Count - 1;
			}
			middleIndicesIndex++;

			middleIndices [middleIndicesIndex] = leftIndex + verticesPosition.Count;
			middleIndicesIndex++;
			if (x == verticesPosition.Count - 1) {
				middleIndices [middleIndicesIndex] = verticesPosition.Count;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex + verticesPosition.Count;
			}
			middleIndicesIndex++;
			if (x == verticesPosition.Count - 1) {
				middleIndices [middleIndicesIndex] = 0;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex;
			}
			middleIndicesIndex++;
		}

		List<Vector2> topVertices2D = new List<Vector2> ();
		for (int i = 0; i < topCorners.Length; i++) {
			topVertices2D.Add (new Vector2 (topCorners [i].x, topCorners [i].z));
		}

		Triangulator topTr = new Triangulator (topVertices2D);
		int[] topdIndices = topTr.Triangulate ();

		for (int i = 0; i < topdIndices.Length; i++) {
			topdIndices [i] += verticesPosition.Count;
		}

		int[] combinedIndices = new int[downIndices.Length + topdIndices.Length + middleIndices.Length];
		downIndices.CopyTo (combinedIndices, 0);
		middleIndices.CopyTo (combinedIndices, downIndices.Length);
		topdIndices.CopyTo (combinedIndices, downIndices.Length + middleIndices.Length);

		mapPart3dMesh.triangles = combinedIndices;

		mapPart3dMesh.RecalculateNormals ();

		mapManager.addMapPart3dMeshToFloorParent (mapPart3dGameObject, gameObject);
	}

	public void setMapManager (mapCreator currentMapManager)
	{
		mapManager = currentMapManager;
		updateComponent ();
	}

	public void setMapPartParent (Transform currentMapPartParent)
	{
		mapPartParent = currentMapPartParent;
		updateComponent ();
	}

	public void setMapPartBuildingIndex (int newIndex)
	{
		mapPartBuildingIndex = newIndex;
		updateComponent ();
	}

	public void setMapPartFlooorIndex (int newIndex)
	{
		mapPartFloorIndex = newIndex;
		updateComponent ();
	}

	public void setMapPartIndex (int newIndex)
	{
		mapPartIndex = newIndex;
		updateComponent ();
	}

	public void setRandomMapPartColor ()
	{
		mapPartMaterialColor = new Vector4 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1);

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<mapTileBuilder> ());
		#endif
	}

	//draw every floor position and a line between floors
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo && mapManager.showMapPartsGizmo && !Application.isPlaying) {
			center = Vector3.zero;
			for (int i = 0; i < verticesPosition.Count; i++) {
				if (verticesPosition [i] != null) {
					if (i + 1 < verticesPosition.Count) {
						if (verticesPosition [i + 1] != null) {
							if (mapManager.useSameLineColor) {
								Gizmos.color = mapManager.mapLinesColor;
							} else {
								Gizmos.color = mapLinesColor;
							}
							Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [i + 1].position);
						}
					}
					if (i == verticesPosition.Count - 1) {
						if (verticesPosition [0] != null) {
							if (mapManager.useSameLineColor) {
								Gizmos.color = mapManager.mapLinesColor;
							} else {
								Gizmos.color = mapLinesColor;
							}
							Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [0].position);
						}
					}
					center += verticesPosition [i].position;
				} 
			}
			center /= verticesPosition.Count;

			if (showEnabledTrigger && mapManager.showMapPartEnabledTrigger) {
				for (int i = 0; i < eventTriggerList.Count; i++) {
					if (eventTriggerList [i]) {
						Gizmos.color = mapManager.enabledTriggerGizmoColor;
						Gizmos.DrawCube (eventTriggerList [i].transform.position, eventTriggerList [i].transform.localScale);

						Gizmos.color = Color.yellow;
						Gizmos.DrawLine (eventTriggerList [i].transform.position, center);
					}
				}
			}

			if (mapManager.showMapPartsTextGizmo && textMeshList.Count > 0) {
				for (int i = 0; i < textMeshList.Count; i++) {
					if (textMeshList [i]) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (textMeshList [i].transform.position, 0.1f);

						Gizmos.color = Color.blue;
						Gizmos.DrawLine (textMeshList [i].transform.position, center);
					}
				}
			}

			if (generate3dMapPartMesh && (mapManager.generate3dMeshesShowGizmo || generate3dMeshesShowGizmo)) {
				current3dOffset = Vector3.up * mapPart3dHeight;
				Gizmos.color = Color.white;
				Gizmos.DrawLine (center + mapPart3dOffset, center + mapPart3dOffset + current3dOffset);
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (center + mapPart3dOffset, 0.2f);
				Gizmos.DrawSphere (center + mapPart3dOffset + current3dOffset, 0.2f);

				for (int i = 0; i < verticesPosition.Count; i++) {
					if (verticesPosition [i] != null) {
						if (i + 1 < verticesPosition.Count) {
							if (verticesPosition [i + 1] != null) {
								if (mapManager.useSameLineColor) {
									Gizmos.color = mapManager.mapLinesColor;
								} else {
									Gizmos.color = mapLinesColor;
								}
								Gizmos.DrawLine (verticesPosition [i].position + current3dOffset, verticesPosition [i + 1].position + current3dOffset);
							}
						}
						if (i == verticesPosition.Count - 1) {
							if (verticesPosition [0] != null) {
								if (mapManager.useSameLineColor) {
									Gizmos.color = mapManager.mapLinesColor;
								} else {
									Gizmos.color = mapLinesColor;
								}
								Gizmos.DrawLine (verticesPosition [i].position + current3dOffset, verticesPosition [0].position + current3dOffset);
							}
						}
					} 
					Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [i].position + current3dOffset);
				}
			}

			Gizmos.color = mapPartMaterialColor;
			Gizmos.DrawCube (center, cubeGizmoScale);
		}
	}
}