using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class hideFromEnemiesSystem : MonoBehaviour
{

	public LayerMask layerToCharacters;
	public bool characterNeedToCrouch;
	public bool characterCantMove;
	public float maxMoveAmount;
	public bool hiddenForATime;
	public float hiddenForATimeAmount;

	public float timeDelayToHidenAgainIfDiscovered;

	public float checkIfCharacterCanBeHiddenAgainRate = 0.5f;

	public bool useCharacterStateIcon;
	public string visibleCharacterStateName;
	public string notVisibleCharacterStateName;

	public List<hiddenCharacterInfo> hiddenCharacterInfoList = new List<hiddenCharacterInfo> ();
	hiddenCharacterInfo currentCharacter;

	public string triggerLayer;
	public Material triggerMaterial;
	public Vector2 newPositionOffset;

	public bool showGizmo = true;
	public Vector3 cubeGizmoScale = Vector3.one;
	public Color gizmoLabelColor = Color.white;
	public Color linesColor = Color.yellow;
	public bool useHandleForVertex;
	public float handleRadius = 0.1f;
	public Color materialColor;

	GameObject wallRenderer;
	MeshRenderer mainMeshRenderer;
	public List<Transform> vertexPosition = new List<Transform> ();

	public Vector3 center;

	void Start ()
	{
		//createTileElement ();
	}

	void Update ()
	{
		if (hiddenCharacterInfoList.Count == 0) {
			return;
		}

		for (int i = 0; i < hiddenCharacterInfoList.Count; i++) {
			currentCharacter = hiddenCharacterInfoList [i];

			//if an external action changes the visibility of the character to AI, change its state here too
			if (currentCharacter.playerControllerManager.isCharacterVisibleToAI ()) {
				if (currentCharacter.hidden) {
					currentCharacter.hidden = false;
					if (currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
						currentCharacter.lastTimeDiscovered = Time.time;	
					}
				} 
			}

			if (!currentCharacter.hidden) {
				if (Time.time > currentCharacter.lastTimeCheckIfCharacterCanBeHiddenAgain + checkIfCharacterCanBeHiddenAgainRate) {
					if (currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
						if (currentCharacter.canBeHidden) {
							setCharacterState (visibleCharacterStateName, i);
							currentCharacter.playerControllerManager.setVisibleToAIState (true);
							currentCharacter.canBeHidden = false;
						}
					} else if (Time.time > currentCharacter.lastTimeDiscovered + timeDelayToHidenAgainIfDiscovered && currentCharacter.canBeHidden) {
						currentCharacter.playerControllerManager.setVisibleToAIState (false);
						currentCharacter.hidden = true;
						currentCharacter.lastTimeHidden = Time.time;
						setCharacterState (notVisibleCharacterStateName, i);
					}
					currentCharacter.lastTimeCheckIfCharacterCanBeHiddenAgain = Time.time;
				}
			}

			if (characterNeedToCrouch) {
				if (currentCharacter.playerControllerManager.isCrouching ()) {
					if (!currentCharacter.hidden) {
						if (!currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
							currentCharacter.playerControllerManager.setVisibleToAIState (false);
							currentCharacter.hidden = true;
							currentCharacter.canBeHidden = true;
							currentCharacter.lastTimeHidden = Time.time;
							setCharacterState (notVisibleCharacterStateName, i);
						}
					}
				} else {
					if (currentCharacter.hidden) {
						currentCharacter.playerControllerManager.setVisibleToAIState (true);
						currentCharacter.hidden = false;
						currentCharacter.lastTimeDiscovered = Time.time;
						setCharacterState (visibleCharacterStateName, i);
						currentCharacter.canBeHidden = false;
					}
				}
			} else {
				currentCharacter.playerControllerManager.setVisibleToAIState (false);
				if (!currentCharacter.hidden) {
					currentCharacter.hidden = true;
					currentCharacter.canBeHidden = true;
					currentCharacter.lastTimeHidden = Time.time;
					setCharacterState (notVisibleCharacterStateName, i);
				}
			}

//			if (hiddenForATime) {
//				if (Time.time > currentCharacter.lastTimeHidden + hiddenForATimeAmount) {
//					currentCharacter.playerControllerManager.setVisibleToAIState (true);
//					setCharacterState (visibleCharacterStateName, i);
//					currentCharacter.hidden = false;
//					currentCharacter.lastTimeDiscovered = Time.time;
//				}
//			}
//
//			if (characterCantMove) {
//				if (currentCharacter.playerControllerManager.isPlayerMoving (maxMoveAmount)) {
//					currentCharacter.playerControllerManager.setVisibleToAIState (true);
//					setCharacterState (visibleCharacterStateName, i);
//					currentCharacter.hidden = false;
//					currentCharacter.lastTimeDiscovered = Time.time;
//				}
//			}
		}
	}

	public void setCharacterState (string stateName, int characterIndex)
	{
		if (useCharacterStateIcon) {
			hiddenCharacterInfoList [characterIndex].playerControllerManager.setCharacterStateIcon (stateName);
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if ((1 << col.gameObject.layer & layerToCharacters.value) == 1 << col.gameObject.layer) {
			GameObject characterFound = col.gameObject;
			bool alreadyIncluded = false;
			int characterIndex = -1;
			for (int i = 0; i < hiddenCharacterInfoList.Count; i++) {
				if (hiddenCharacterInfoList [i].characterGameObject == characterFound && !alreadyIncluded) {
					alreadyIncluded = true;
					characterIndex = i;
				}

			}
			if (isEnter) {
				if (!alreadyIncluded) {
					hiddenCharacterInfo newHiddenCharacterInfo = new hiddenCharacterInfo ();
					newHiddenCharacterInfo.name = characterFound.name;
					newHiddenCharacterInfo.characterGameObject = characterFound;
					newHiddenCharacterInfo.playerControllerManager = characterFound.GetComponent<playerController> ();
					newHiddenCharacterInfo.playerControllerManager.setVisibleToAIState (false);
					newHiddenCharacterInfo.factionManager = characterFound.GetComponent<characterFactionManager> ();
					newHiddenCharacterInfo.hidden = true;
					newHiddenCharacterInfo.lastTimeHidden = Time.time;
					newHiddenCharacterInfo.canBeHidden = true;
					hiddenCharacterInfoList.Add (newHiddenCharacterInfo);

					setCharacterState (notVisibleCharacterStateName, hiddenCharacterInfoList.Count - 1);
				}
			} else {
				if (alreadyIncluded) {
					hiddenCharacterInfoList [characterIndex].playerControllerManager.setVisibleToAIState (true);
					setCharacterState (visibleCharacterStateName, characterIndex);
					hiddenCharacterInfoList.RemoveAt (characterIndex);
				}
			}
		}
	}

	public void addNewVertex (int insertAtIndex)
	{
		GameObject newTransform = new GameObject ();
		newTransform.transform.SetParent (transform);
		newTransform.transform.localRotation = Quaternion.identity;
		if (vertexPosition.Count > 0) {
			Vector3 lastPosition = vertexPosition [vertexPosition.Count - 1].localPosition;
			newTransform.transform.localPosition = new Vector3 (lastPosition.x + newPositionOffset.x, 0, lastPosition.z + newPositionOffset.y);
		} else {
			newTransform.transform.localPosition = Vector3.zero;
		}
		newTransform.name = (vertexPosition.Count + 1).ToString ("000");
		if (insertAtIndex > -1) {
			if (vertexPosition.Count > 0) {
				newTransform.transform.localPosition = vertexPosition [insertAtIndex].localPosition;
			}
			vertexPosition.Insert (insertAtIndex + 1, newTransform.transform);
			newTransform.transform.SetSiblingIndex (insertAtIndex + 1);
			renameAllVertex ();
		} else {
			vertexPosition.Add (newTransform.transform);
		}
		updateComponent ();
	}

	public void removeVertex (int vertexIndex)
	{
		Transform currentVertex = vertexPosition [vertexIndex];
		if (currentVertex) {
			DestroyImmediate (currentVertex.gameObject);
		}
		vertexPosition.RemoveAt (vertexIndex);
		updateComponent ();
	}

	public void removeAllVertex ()
	{
		for (int i = 0; i < vertexPosition.Count; i++) {
			if (vertexPosition [i]) {
				DestroyImmediate (vertexPosition [i].gameObject);
			}
		}
		vertexPosition.Clear ();
		updateComponent ();
	}

	public void renameAllVertex ()
	{
		for (int i = 0; i < vertexPosition.Count; i++) {
			if (vertexPosition [i]) {
				vertexPosition [i].name = (i + 1).ToString ("000");
			}
		}
		updateComponent ();
	}

	public void removeUpOffset ()
	{
		for (int i = 0; i < vertexPosition.Count; i++) {
			if (vertexPosition [i]) {
				vertexPosition [i].localPosition -= transform.forward * vertexPosition [i].localPosition.z;
			}
		}
		updateComponent ();
	}

	public void createTileElement ()
	{
		List<Vector2> vertex2D = new List<Vector2> ();
		for (int i = 0; i < vertexPosition.Count; i++) {
			vertex2D.Add (new Vector2 (vertexPosition [i].localPosition.x, vertexPosition [i].localPosition.y));
		}
		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator (vertex2D);
		int[] indices = tr.Triangulate ();

		// Create the Vector3 vertex
		Vector3[] vertex = new Vector3[vertex2D.Count];
		for (int i = 0; i < vertex.Length; i++) {
			vertex [i] = new Vector3 (vertex2D [i].x, vertex2D [i].y, 0);
		}

		// Create the mesh
		Mesh msh = new Mesh ();
		msh.vertices = vertex;
		msh.triangles = indices;
		msh.RecalculateNormals ();
		msh.RecalculateBounds ();

		// Set up game object with mesh;
		GameObject newRenderer = new GameObject ();
		wallRenderer = newRenderer;
		wallRenderer.transform.SetParent (transform);
		wallRenderer.layer = LayerMask.NameToLayer (triggerLayer);
		wallRenderer.transform.localPosition = Vector3.zero;
		wallRenderer.transform.localRotation = Quaternion.identity;
		wallRenderer.name = "Wall Renderer";
		wallRenderer.AddComponent (typeof(MeshRenderer));
		Material newMaterial = new Material (triggerMaterial);
		mainMeshRenderer = wallRenderer.GetComponent<MeshRenderer> ();
		mainMeshRenderer.material = newMaterial;

		MeshFilter filter = wallRenderer.AddComponent (typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;

		mainMeshRenderer.enabled = false;
		wallRenderer.AddComponent<MeshCollider> ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<hideFromEnemiesSystem> ());
		#endif
	}

	//draw every floor position and a line between floors
	void OnDrawGizmos ()
	{
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
		if (showGizmo && !Application.isPlaying) {
			center = Vector3.zero;
			for (int i = 0; i < vertexPosition.Count; i++) {
				if (vertexPosition [i] != null) {
					if (i + 1 < vertexPosition.Count) {
						if (vertexPosition [i + 1] != null) {
							Gizmos.color = linesColor;
							Gizmos.DrawLine (vertexPosition [i].position, vertexPosition [i + 1].position);
						}
					}
					if (i == vertexPosition.Count - 1) {
						if (vertexPosition [0] != null) {
							Gizmos.color = linesColor;
							Gizmos.DrawLine (vertexPosition [i].position, vertexPosition [0].position);
						}
					}
					center += vertexPosition [i].position;
				} 
			}
			center /= vertexPosition.Count;

			Gizmos.color = materialColor;
			Gizmos.DrawCube (center, cubeGizmoScale);
		}
	}

	[System.Serializable]
	public class hiddenCharacterInfo
	{
		public string name;
		public GameObject characterGameObject;
		public playerController playerControllerManager;
		public characterFactionManager factionManager;
		public bool hidden;
		public float hiddenTime;
		public float lastTimeHidden;
		public float lastTimeDiscovered;
		public bool canBeHidden;
		public float lastTimeCheckIfCharacterCanBeHiddenAgain;
	}
}
