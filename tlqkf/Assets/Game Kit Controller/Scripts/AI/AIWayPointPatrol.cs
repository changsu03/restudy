﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class AIWayPointPatrol : MonoBehaviour
{
	public List<patrolElementInfo> patrolList = new List<patrolElementInfo> ();
	public patrolElementInfo currentPatrol;
	public float waitTimeBetweenPoints;
	public bool movingForward = true;
	public bool showGizmo;
	public Color gizmoLabelColor;
	public float gizmoRadius;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;
	public LayerMask layerMask;
	public Vector3 newWaypointOffset;
	public float surfaceAdjusmentOffset = 0.1f;

	Coroutine movement;
	int currentPlatformIndex;
	int i, j;
	bool inside;
	bool moving;

	void Start ()
	{

	}

	public void addNewPatrol ()
	{
		Vector3 newPosition = transform.position;
		if (patrolList.Count > 0) {
			newPosition = patrolList [patrolList.Count - 1].patrolTransform.position +
			patrolList [patrolList.Count - 1].patrolTransform.right * newWaypointOffset.x +
			patrolList [patrolList.Count - 1].patrolTransform.up * newWaypointOffset.y +
			patrolList [patrolList.Count - 1].patrolTransform.forward * newWaypointOffset.z;
		}

		patrolElementInfo newPatrol = new patrolElementInfo ();
		GameObject newPatrolTransform = new GameObject ();
		newPatrolTransform.transform.SetParent (transform);
		newPatrolTransform.transform.position = newPosition;
		newPatrolTransform.transform.localRotation = Quaternion.identity;
		newPatrol.name = "Patrol " + (patrolList.Count + 1).ToString ();
		newPatrol.patrolTransform = newPatrolTransform.transform;
		newPatrolTransform.name = "Patrol_" + (patrolList.Count + 1).ToString ();
		patrolList.Add (newPatrol);
		updateWayPointPatrol ();
	}

	public void clearPatrolList ()
	{
		for (i = 0; i < patrolList.Count; i++) {
			clearWayPoint (i);
			Destroy (patrolList [i].patrolTransform.gameObject);
		}

		patrolList.Clear ();
		updateWayPointPatrol ();
	}

	//add a new waypoint
	public void addNewWayPoint (int index)
	{
		Vector3 newPosition = patrolList [index].patrolTransform.position;
		if (patrolList [index].wayPoints.Count > 0) {
			newPosition = patrolList [index].wayPoints [patrolList [index].wayPoints.Count - 1].position +
			patrolList [index].wayPoints [patrolList [index].wayPoints.Count - 1].right * newWaypointOffset.x +
			patrolList [index].wayPoints [patrolList [index].wayPoints.Count - 1].up * newWaypointOffset.y +
			patrolList [index].wayPoints [patrolList [index].wayPoints.Count - 1].forward * newWaypointOffset.z;
		}

		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (patrolList [index].patrolTransform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.transform.localRotation = Quaternion.identity;
		newWayPoint.name = (patrolList [index].wayPoints.Count + 1).ToString ();
		patrolList [index].wayPoints.Add (newWayPoint.transform);
		updateWayPointPatrol ();
	}

	public void clearWayPoint (int index)
	{
		for (i = 0; i < patrolList [index].wayPoints.Count; i++) {
			DestroyImmediate (patrolList [index].wayPoints [i].gameObject);
		}

		DestroyImmediate (patrolList [index].patrolTransform.gameObject);
		updateWayPointPatrol ();
		patrolList [index].wayPoints.Clear ();
		patrolList.RemoveAt (index);
	}

	public void updateWayPointPatrol ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<AIWayPointPatrol> ());
		#endif
	}

	public void adjustWayPoints ()
	{
		RaycastHit hit;
		for (i = 0; i < patrolList.Count; i++) {
			for (j = 0; j < patrolList [i].wayPoints.Count; j++) {
				if (Physics.Raycast (patrolList [i].wayPoints [j].position, -patrolList [i].wayPoints [j].up, out hit, Mathf.Infinity, layerMask)) {
					patrolList [i].wayPoints [j].position = hit.point + patrolList [i].wayPoints [j].up * surfaceAdjusmentOffset;
				}
			}
		}
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<AIWayPointPatrol> ());
		#endif
	}

	public void invertPath ()
	{

	}

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

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (i = 0; i < patrolList.Count; i++) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere (patrolList [i].patrolTransform.position, gizmoRadius);
				for (j = 0; j < patrolList [i].wayPoints.Count; j++) {
					if (patrolList [i].wayPoints [j]) {
						Gizmos.color = Color.yellow;
						Gizmos.DrawSphere (patrolList [i].wayPoints [j].position, gizmoRadius);
						if (j + 1 < patrolList [i].wayPoints.Count) {
							Gizmos.color = Color.white;

							Vector3 initialPosition = patrolList [i].wayPoints [j].position;
							Vector3 targetPosition = patrolList [i].wayPoints [j + 1].position;
							Gizmos.DrawLine (initialPosition, targetPosition);

							Vector3 arrowDirection = (targetPosition - initialPosition).normalized;
							GKC_Utils.drawGizmoArrow (initialPosition, arrowDirection, Color.green, 1, 20);
						}

						if (j == patrolList [i].wayPoints.Count - 1) {
							Gizmos.color = Color.white;
							Gizmos.DrawLine (patrolList [i].wayPoints [j].position, patrolList [i].wayPoints [0].position);
						}
					}
				}
				if (patrolList.Count > 1) {
					if (i + 1 < patrolList.Count) {
						if (patrolList [i].wayPoints.Count > 0) {
							if (patrolList [i + 1].wayPoints.Count > 0) {
								if (patrolList [i].wayPoints [patrolList [i].wayPoints.Count - 1] && patrolList [i + 1].wayPoints [0]) { 
									Gizmos.color = Color.blue;

									Vector3 initialPosition = patrolList [i].wayPoints [patrolList [i].wayPoints.Count - 1].position;
									Vector3 targetPosition = patrolList [i + 1].wayPoints [0].position;
									Gizmos.DrawLine (initialPosition, targetPosition);

									Vector3 arrowDirection = (targetPosition - initialPosition).normalized;
									GKC_Utils.drawGizmoArrow (initialPosition, arrowDirection, Color.red, 2, 20);
								}
							}
						}
					}

					if (i == patrolList.Count - 1) {
						if (patrolList [0].wayPoints.Count > 0 && patrolList [patrolList.Count - 1].wayPoints.Count > 0) {
							if (patrolList [patrolList.Count - 1].wayPoints [patrolList [patrolList.Count - 1].wayPoints.Count - 1]) {
								Gizmos.color = Color.blue;
								Gizmos.DrawLine (patrolList [0].wayPoints [0].position, 
									patrolList [patrolList.Count - 1].wayPoints [patrolList [patrolList.Count - 1].wayPoints.Count - 1].position);
							}
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class patrolElementInfo
	{
		public string name;
		public Transform patrolTransform;
		public List<Transform> wayPoints = new List<Transform> ();
	}
}