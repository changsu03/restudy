using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class setTransparentSurfaces : MonoBehaviour
{
	public bool checkSurfaceEnabled = true;
	public LayerMask layer;
	public Transform rayOriginPosition;
	public Transform rayTargetPosition;
	public float capsuleCastRadius = 0.3f;

	public playerController playerControllerManager;
	public int playerID;

	public bool showGizmo;
	public Color sphereColor = Color.green;
	public Color cubeColor = Color.blue;

	RaycastHit[] hits;
	float distanceToTarget;

	Vector3 rayDirection;
	List<GameObject> surfaceGameObjectList = new List<GameObject> ();
	Vector3 point1;
	Vector3 point2;

	GameObject currentSurfaceGameObjectFound;

	setTransparentSurfacesSystem setTransparentSurfacesManager;

	public List<GameObject> currentSurfaceGameObjectFoundList = new List<GameObject> ();

	setTransparentSurfacesSystem.surfaceInfo currentSurfaceInfo;

	void Start ()
	{
		setTransparentSurfacesManager = FindObjectOfType<setTransparentSurfacesSystem> ();

		if (playerControllerManager) {
			playerID = playerControllerManager.getPlayerID ();
		}
	}

	void Update ()
	{
		if (checkSurfaceEnabled) {
			if (!rayOriginPosition || !rayTargetPosition) {
				print ("WARNING: Ray Origin Position or Ray Target Position hasn't been assigned in the Set Transparent Surfaces inspector");
				return;
			}

			distanceToTarget = GKC_Utils.distance (rayOriginPosition.position, rayTargetPosition.position);
			rayDirection = rayOriginPosition.position - rayTargetPosition.position;
			rayDirection = rayDirection / rayDirection.magnitude;

			Debug.DrawLine (rayTargetPosition.position, (rayDirection * distanceToTarget) + rayTargetPosition.position, Color.red, 2);

			point1 = rayOriginPosition.position - rayDirection * capsuleCastRadius;
			point2 = rayTargetPosition.position + rayDirection * capsuleCastRadius;

			hits = Physics.CapsuleCastAll (point1, point2, capsuleCastRadius, rayDirection, 0, layer);

			surfaceGameObjectList.Clear ();

			for (int i = 0; i < hits.Length; i++) {
				currentSurfaceGameObjectFound = hits [i].collider.gameObject;

				if (!setTransparentSurfacesManager.listContainsSurface (currentSurfaceGameObjectFound)) {
					setTransparentSurfacesManager.addNewSurface (currentSurfaceGameObjectFound);
				}

				if (!currentSurfaceGameObjectFoundList.Contains (currentSurfaceGameObjectFound)) {
					currentSurfaceGameObjectFoundList.Add (currentSurfaceGameObjectFound);
					setTransparentSurfacesManager.addPlayerIDToSurface (playerID, currentSurfaceGameObjectFound);
				}

				surfaceGameObjectList.Add (currentSurfaceGameObjectFound);
			}
		
			for (int i = 0; i < setTransparentSurfacesManager.surfaceInfoList.Count; i++) {
				currentSurfaceInfo = setTransparentSurfacesManager.surfaceInfoList [i];

				if (!surfaceGameObjectList.Contains (currentSurfaceInfo.surfaceGameObject)) {
					if (currentSurfaceInfo.playerIDs.Contains (playerID)) {

						setTransparentSurfacesManager.removePlayerIDToSurface (playerID, i);
					}

					if (currentSurfaceGameObjectFoundList.Contains (currentSurfaceInfo.surfaceGameObject)) {
						currentSurfaceGameObjectFoundList.Remove (currentSurfaceInfo.surfaceGameObject);
					}

					if (currentSurfaceInfo.numberOfPlayersFound < 1) {
						setTransparentSurfacesManager.removeSurface (i);
						i = 0;
					}
				}
			}
		}
	}

	public void setCheckSurfaceEnabled (bool state)
	{
		checkSurfaceEnabled = state;

		if (!checkSurfaceEnabled) {

			checkSurfacesToRemove ();

			setTransparentSurfacesManager.checkSurfacesToRemove ();
		}
	}

	public void checkSurfacesToRemove ()
	{
		for (int i = 0; i < setTransparentSurfacesManager.surfaceInfoList.Count; i++) {
			currentSurfaceInfo = setTransparentSurfacesManager.surfaceInfoList [i];

			if (currentSurfaceGameObjectFoundList.Contains (currentSurfaceInfo.surfaceGameObject)) {
			
				if (currentSurfaceInfo.playerIDs.Contains (playerID)) {

					setTransparentSurfacesManager.removePlayerIDToSurface (playerID, i);
				}
			}
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo && Application.isPlaying && checkSurfaceEnabled) {
		
			Gizmos.color = sphereColor;
		
			Gizmos.DrawSphere (point1, capsuleCastRadius);
			Gizmos.DrawSphere (point2, capsuleCastRadius);

			Gizmos.color = cubeColor;
			Vector3 scale = new Vector3 (capsuleCastRadius * 2, capsuleCastRadius * 2, distanceToTarget - capsuleCastRadius * 2);
			Matrix4x4 cubeTransform = Matrix4x4.TRS ((rayDirection * (distanceToTarget / 2)) + rayTargetPosition.position, Quaternion.LookRotation (rayDirection, point1 - point2), scale);
			Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

			Gizmos.matrix *= cubeTransform;

			Gizmos.DrawCube (Vector3.zero, Vector3.one);

			Gizmos.matrix = oldGizmosMatrix;
		}
	}
	#endif
}
