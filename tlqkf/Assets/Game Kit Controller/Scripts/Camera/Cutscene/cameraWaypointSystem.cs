using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class cameraWaypointSystem : MonoBehaviour
{
	public Transform currentCameraTransform;
	public List<cameraWaypointInfo> waypointList = new List<cameraWaypointInfo> ();
	public float waitTimeBetweenPoints;
	public float movementSpeed;
	public float rotationSpeed;

	public Transform pointToLook;

	public bool useEventOnEnd;
	public UnityEvent eventOnEnd;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandleForWaypoints;
	public float handleRadius;
	public Color handleGizmoColor;
	public bool showWaypointHandles;

	public float currentMovementSpeed;
	public float currentRotationSpeed;

	public bool useBezierCurve;
	public BezierSpline spline;
	public float bezierDuration = 10;

	float currentWaitTime;
	Vector3 targetDirection;

	Coroutine movement;
	Transform currentWaypoint;
	int currentWaypointIndex;
	int i;
	List<Transform> currentPath = new List<Transform> ();
	cameraWaypointInfo currentCameraWaypointInfo;

	int previousWaypointIndex;

	Vector3 targetPosition;
	Quaternion targetRotation;

	public void setCurrentCameraTransform (GameObject cameraGameObject)
	{
		currentCameraTransform = cameraGameObject.transform;
	}

	//stop the platform coroutine movement and play again
	public void checkMovementCoroutine (bool play)
	{
		if (movement != null) {
			StopCoroutine (movement);
		}
		if (play) {
			movement = StartCoroutine (moveThroughWayPoints ());
		}
	}

	IEnumerator moveThroughWayPoints ()
	{
		currentWaypointIndex = 0;

		previousWaypointIndex = -1;

		//if the current path to move has waypoints, then
		if (currentPath.Count == 0) {
			for (i = 0; i < waypointList.Count; i++) {
				currentPath.Add (waypointList [i].waypointTransform);
			}
		}

		if (currentPath.Count > 0) {
			if (useBezierCurve) {

				spline.setInitialSplinePoint (currentCameraTransform.position);

				float progress = 0;
				float progressTarget = 1;

				bool targetReached = false;

				while (!targetReached) {

					if (previousWaypointIndex != currentWaypointIndex) {

						if (previousWaypointIndex != -1) {
							if (currentCameraWaypointInfo.useEventOnPointReached) {
								currentCameraWaypointInfo.eventOnPointReached.Invoke ();
							}
						}

						previousWaypointIndex = currentWaypointIndex;

						currentCameraWaypointInfo = waypointList [currentWaypointIndex];

						currentWaypoint = currentCameraWaypointInfo.waypointTransform;

						//wait the amount of time configured
						if (currentCameraWaypointInfo.useCustomWaitTimeBetweenPoint) {
							currentWaitTime = currentCameraWaypointInfo.waitTimeBetweenPoints;
						} else {
							currentWaitTime = waitTimeBetweenPoints;
						}

						targetPosition = currentWaypoint.position;
						targetRotation = currentWaypoint.rotation;

						yield return new WaitForSeconds (currentWaitTime);			

						if (currentCameraWaypointInfo.useCustomMovementSpeed) {
							currentMovementSpeed = currentCameraWaypointInfo.movementSpeed;
						} else {
							currentMovementSpeed = movementSpeed;
						}

						if (currentCameraWaypointInfo.useCustomRotationSpeed) {
							currentRotationSpeed = currentCameraWaypointInfo.rotationSpeed;
						} else {
							currentRotationSpeed = rotationSpeed;
						}
					}

					currentWaypointIndex = spline.getPointIndex (progress);

					progress += Time.deltaTime / (bezierDuration * currentMovementSpeed);

					Vector3 position = spline.GetPoint (progress);
					currentCameraTransform.position = position;

					if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
						targetDirection = targetPosition - currentCameraTransform.position;
					} 

					if (currentCameraWaypointInfo.usePointToLook) {
						targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
					}

					if (targetDirection != Vector3.zero) {
						targetRotation = Quaternion.LookRotation (targetDirection);
						currentCameraTransform.rotation = Quaternion.Lerp (currentCameraTransform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
					}			

					if (progress > progressTarget) {
						targetReached = true;
					}

					yield return null;
				}
			} else {
				//move between every waypoint
				foreach (Transform waypoint in currentPath) {
					currentWaypoint = waypoint;
					currentCameraWaypointInfo = waypointList [currentWaypointIndex];

					//wait the amount of time configured
					if (currentCameraWaypointInfo.useCustomWaitTimeBetweenPoint) {
						currentWaitTime = currentCameraWaypointInfo.waitTimeBetweenPoints;
					} else {
						currentWaitTime = waitTimeBetweenPoints;
					}

					targetPosition = currentWaypoint.position;
					targetRotation = currentWaypoint.rotation;

					yield return new WaitForSeconds (currentWaitTime);			

					if (currentCameraWaypointInfo.useCustomMovementSpeed) {
						currentMovementSpeed = currentCameraWaypointInfo.movementSpeed;
					} else {
						currentMovementSpeed = movementSpeed;
					}

					if (currentCameraWaypointInfo.useCustomRotationSpeed) {
						currentRotationSpeed = currentCameraWaypointInfo.rotationSpeed;
					} else {
						currentRotationSpeed = rotationSpeed;
					}

					if (currentCameraWaypointInfo.smoothTransitionToNextPoint) {
						//while the platform moves from the previous waypoint to the next, then displace it
						while (GKC_Utils.distance (currentCameraTransform.position, targetPosition) > .01f) {
							currentCameraTransform.position = Vector3.MoveTowards (currentCameraTransform.position, targetPosition, Time.deltaTime * currentMovementSpeed);

							if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
								targetDirection = targetPosition - currentCameraTransform.position;
							} 

							if (currentCameraWaypointInfo.usePointToLook) {
								targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
							}
							if (targetDirection != Vector3.zero) {
								targetRotation = Quaternion.LookRotation (targetDirection);
								currentCameraTransform.rotation = Quaternion.Lerp (currentCameraTransform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
							}
							yield return null;
						}
					} else {
						currentCameraTransform.position = targetPosition;
						if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
							targetDirection = targetPosition - currentCameraTransform.position;
						} 

						if (currentCameraWaypointInfo.usePointToLook) {
							targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
						}

						if (!currentCameraWaypointInfo.rotateCameraToNextWaypoint && !currentCameraWaypointInfo.usePointToLook) {
							currentCameraTransform.rotation = currentCameraWaypointInfo.waypointTransform.rotation;
						} else {
							if (targetDirection != Vector3.zero) {
								currentCameraTransform.rotation = Quaternion.LookRotation (targetDirection);
							}
						}

						yield return new WaitForSeconds (currentCameraWaypointInfo.timeOnFixedPosition);
					}

					if (currentCameraWaypointInfo.useEventOnPointReached) {
						currentCameraWaypointInfo.eventOnPointReached.Invoke ();
					}

					//when the platform reaches the next waypoint
					currentWaypointIndex++;
				}
			}

			if (useEventOnEnd) {
				eventOnEnd.Invoke ();
			}
		} else {
			//else, stop the movement
			checkMovementCoroutine (false);
		}
	}

	//add a new waypoint
	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;
		if (waypointList.Count > 0) {
			newPosition = waypointList [waypointList.Count - 1].waypointTransform.position + waypointList [waypointList.Count - 1].waypointTransform.forward;
		}
		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (waypointList.Count + 1).ToString ();

		cameraWaypointInfo newCameraWaypointInfo = new cameraWaypointInfo ();
		newCameraWaypointInfo.Name = newWayPoint.name;
		newCameraWaypointInfo.waypointTransform = newWayPoint.transform;
		newCameraWaypointInfo.rotateCameraToNextWaypoint = true;
		waypointList.Add (newCameraWaypointInfo);

		updateComponent ();
	}

	public void addNewWayPoint (int insertAtIndex)
	{
		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.name = (waypointList.Count + 1).ToString ();

		cameraWaypointInfo newCameraWaypointInfo = new cameraWaypointInfo ();
		newCameraWaypointInfo.Name = newWayPoint.name;
		newCameraWaypointInfo.waypointTransform = newWayPoint.transform;
		newCameraWaypointInfo.rotateCameraToNextWaypoint = true;

		if (waypointList.Count > 0) {
			Vector3 lastPosition = waypointList [waypointList.Count - 1].waypointTransform.position + waypointList [waypointList.Count - 1].waypointTransform.forward;
			newWayPoint.transform.localPosition = lastPosition + waypointList [waypointList.Count - 1].waypointTransform.forward * 2;
		} else {
			newWayPoint.transform.localPosition = Vector3.zero;
		}
		if (insertAtIndex > -1) {
			if (waypointList.Count > 0) {
				newWayPoint.transform.localPosition = waypointList [insertAtIndex].waypointTransform.localPosition + waypointList [insertAtIndex].waypointTransform.forward * 2;
				;
			}
			waypointList.Insert (insertAtIndex + 1, newCameraWaypointInfo);
			newWayPoint.transform.SetSiblingIndex (insertAtIndex + 1);
			renameAllWaypoints ();
		} else {
			waypointList.Add (newCameraWaypointInfo);
		}

		updateComponent ();
	}

	public void renameAllWaypoints ()
	{
		for (int i = 0; i < waypointList.Count; i++) {
			if (waypointList [i].waypointTransform) {
				waypointList [i].waypointTransform.name = (i + 1).ToString ("000");
				waypointList [i].Name = (i + 1).ToString ("000");
			}
		}
		updateComponent ();
	}

	public void removeWaypoint (int index)
	{
		if (waypointList [index].waypointTransform) {
			DestroyImmediate (waypointList [index].waypointTransform.gameObject);
		}
//		if (waypointList [index].pointToLook) {
//			DestroyImmediate (waypointList [index].pointToLook.gameObject);
//		}
		waypointList.RemoveAt (index);

		updateComponent ();
	}

	public void removeAllWaypoints ()
	{
		for (int i = 0; i < waypointList.Count; i++) {
			if (waypointList [i].waypointTransform) {
				DestroyImmediate (waypointList [i].waypointTransform.gameObject);
			}
		}
		waypointList.Clear ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<cameraWaypointSystem> ());
		#endif
	}

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

	void DrawGizmos ()
	{
		if (showGizmo) {
			if (waypointList.Count > 0) {
				if (waypointList [0].waypointTransform) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (waypointList [0].waypointTransform.position, transform.position);
				}
			}

			for (i = 0; i < waypointList.Count; i++) {
				if (waypointList [i].waypointTransform) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (waypointList [i].waypointTransform.position, gizmoRadius);
					if (i + 1 < waypointList.Count) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (waypointList [i].waypointTransform.position, waypointList [i + 1].waypointTransform.position);
					}
					if (currentWaypoint) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (currentWaypoint.position, gizmoRadius);
					}

					if (waypointList [i].usePointToLook && waypointList [i].pointToLook) {
						Gizmos.color = Color.green;
						Gizmos.DrawLine (waypointList [i].waypointTransform.position, waypointList [i].pointToLook.position);
						Gizmos.color = Color.blue;
						Gizmos.DrawSphere (waypointList [i].pointToLook.position, gizmoRadius);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class cameraWaypointInfo
	{
		public string Name;
		public Transform waypointTransform;
	
		public bool rotateCameraToNextWaypoint;
		public bool usePointToLook;
		public Transform pointToLook;

		public bool smoothTransitionToNextPoint = true;
		public bool useCustomMovementSpeed;
		public float movementSpeed;
		public bool useCustomRotationSpeed;
		public float rotationSpeed;

		public float timeOnFixedPosition;

		public bool useCustomWaitTimeBetweenPoint;
		public float waitTimeBetweenPoints;

		public bool useEventOnPointReached;
		public UnityEvent eventOnPointReached;
	}
}
