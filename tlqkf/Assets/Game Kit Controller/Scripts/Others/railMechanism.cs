using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class railMechanism : MonoBehaviour
{
	public Transform objectOnRail;
	public Transform stopPosition;
	public Transform finalPosition;

	public bool checkIfFinalPositionReached = true;
	public Transform positionToMoveOnStop;

	public bool usePositionToMoveOnFinal;
	public Transform positionToMoveOnFinal;

	public Transform initialPositionTransform;

	public bool resetToInitialPositionOndrop;
	public float resetToInitialPositionSpeed;

	public bool activated;
	public bool usingRail;

	public float displaceRailSpeed = 1;

	public float movementSpeed = 2;

	public UnityEvent eventOnMechanismConnected;
	public UnityEvent eventOnMechanismStopped;

	public bool useEventOnUsingRail;
	public UnityEvent eventOnUsingRail;
	public bool useEventOnDropRail;
	public UnityEvent eventOnDropRail;

	public bool moveObjectsWhileRailIsMoving;
	public List<Transform> objectsToMoveList = new List<Transform> ();

	public bool showGizmo;
	public float gizmoRadius = 0.2f;
	public float gizmoScale = 0.2f;

	public float multiplierValue = 1;
	float originalMultiplierValue = 1;
	public bool speedReduced;

	Vector3 initPos;
	grabbedObjectState currentGrabbedObject;

	Coroutine resetCoroutine;

	float distanceToPositionToMoveOnFinal;
	float distanceToStopPosition;

	float distanceToFinalPosition;
	float currentDistanceToFinalPosition;
	float currentDistanceToPositionToMoveOnFinal;
	float currentDistanceToStopPosition;

	void Start ()
	{
		initPos = initialPositionTransform.position;
		distanceToPositionToMoveOnFinal = GKC_Utils.distance (positionToMoveOnFinal.position, finalPosition.position);
		distanceToStopPosition = GKC_Utils.distance (stopPosition.position, initPos);
		distanceToFinalPosition = GKC_Utils.distance (initPos, finalPosition.position);
	}

	void Update ()
	{
		if (!activated) {
			if (usingRail) {
				//check the position of the object above the rail, if the object is too close of the extreme of the rail, set back to its original position
				currentDistanceToStopPosition = GKC_Utils.distance (stopPosition.position, objectOnRail.position);
				if (currentDistanceToStopPosition < 0.05f) {
					checkToDropObjectOnRail ();
					resetPosition (positionToMoveOnStop, movementSpeed, false);
					eventOnMechanismStopped.Invoke ();
				}
					
				//if the object reachs the final position, engaged the mechanim
				currentDistanceToFinalPosition = GKC_Utils.distance (finalPosition.position, objectOnRail.position);
				currentDistanceToPositionToMoveOnFinal = GKC_Utils.distance (objectOnRail.position, positionToMoveOnFinal.position);

				if (currentDistanceToFinalPosition < 0.05f || currentDistanceToPositionToMoveOnFinal < distanceToPositionToMoveOnFinal) {
					checkToDropObjectOnRail ();
					//the object only have to be moved from its original position to the final
					if (checkIfFinalPositionReached) {
						engaged ();
						if (usePositionToMoveOnFinal) {
							resetPosition (positionToMoveOnFinal, movementSpeed, false);
						}
					}
				}

				if (moveObjectsWhileRailIsMoving) {
					for (int j = 0; j < objectsToMoveList.Count; j++) {
						float newDistance = distanceToFinalPosition - currentDistanceToFinalPosition;
						newDistance = Mathf.Clamp (newDistance, 0, distanceToFinalPosition);
						objectsToMoveList [j].localPosition = new Vector3 (objectsToMoveList [j].localPosition.x, objectsToMoveList [j].localPosition.y, newDistance);
					}
				}
			}
		}
	}

	public void checkToDropObjectOnRail ()
	{
		currentGrabbedObject = objectOnRail.GetComponent<grabbedObjectState> ();
		if (currentGrabbedObject) {
			GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectOnRail.gameObject);
		}
	}

	//the mechanims has been engaged, so the player does not have to use it
	public void engaged ()
	{
		eventOnMechanismConnected.Invoke ();
		activated = true;
		tag = "Untagged";
		print (activated);
	}

	//set the object to its original position, to avoid the player can move further
	public void resetPosition (Transform targetTransform, float resetMovementSpeed, bool enganeAfterCoroutine)
	{
		stopResetCoroutine ();
		resetCoroutine = StartCoroutine (resetPositionCoroutine (targetTransform, resetMovementSpeed, enganeAfterCoroutine));
	}

	public void stopResetCoroutine ()
	{
		if (resetCoroutine != null) {
			StopCoroutine (resetCoroutine);
		}
	}

	float currentTimer;
	//move the object back
	IEnumerator resetPositionCoroutine (Transform targetTransform, float resetMovementSpeed, bool enganeAfterCoroutine)
	{
		Vector3 targetPosition = targetTransform.position;

		float dist = GKC_Utils.distance (targetPosition, objectOnRail.position);
		float duration = dist / resetMovementSpeed;
		currentTimer = 0;

		float currentDistance = 0;
		bool targetReached = false;
		while (!targetReached && objectOnRail.position != targetPosition) {
			
			currentTimer += (Time.deltaTime / duration) * multiplierValue;

			objectOnRail.position = Vector3.Lerp (objectOnRail.position, targetPosition, currentTimer);

			currentDistance = GKC_Utils.distance (objectOnRail.position, targetPosition);
			if (currentDistance < 0.01f) {
				targetReached = true;
			}
			yield return null; 
		}

		print ("fin");
		if (enganeAfterCoroutine) {
			engaged ();
		}
	}

	public void setUsingRailState (bool state)
	{
		usingRail = state;

		if (!usingRail && !activated && resetToInitialPositionOndrop) {
			resetPosition (initialPositionTransform, resetToInitialPositionSpeed, false);
		}

		if (usingRail) {
			if (useEventOnUsingRail) {
				eventOnUsingRail.Invoke ();
			}
			stopResetCoroutine ();
		} else {
			if (useEventOnDropRail) {
				eventOnDropRail.Invoke ();
			}
		}
	}

	public float getDisplaceRailSpeed ()
	{
		return displaceRailSpeed;
	}

	public void engageRailAutomatically ()
	{
		checkToDropObjectOnRail ();
		if (usePositionToMoveOnFinal) {
			resetPosition (positionToMoveOnFinal, movementSpeed, true);
		} else {
			resetPosition (finalPosition, movementSpeed, true);
		}
	}

	public void setReducedVelocity (float newMultiplierValue)
	{
		multiplierValue = newMultiplierValue;
		resetPosition (initialPositionTransform, resetToInitialPositionSpeed, false);
		//print ("reduce");
	}

	public void setNormalVelocity ()
	{
		//print ("normal");
		multiplierValue = originalMultiplierValue;
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
			if (finalPosition) {
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere (finalPosition.position, gizmoRadius);
			}

			if (stopPosition) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (stopPosition.position, gizmoRadius);
			}

			if (objectOnRail) {
				if (finalPosition) {
					Gizmos.color = Color.green;
					Gizmos.DrawLine (objectOnRail.position, finalPosition.position);
				}
				if (stopPosition) {
					Gizmos.color = Color.red;
					Gizmos.DrawLine (objectOnRail.position, stopPosition.position);
					Gizmos.color = Color.yellow;
					Gizmos.DrawCube (objectOnRail.position, Vector3.one * gizmoScale);
				}
			}

			if (usePositionToMoveOnFinal && positionToMoveOnFinal) {
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere (positionToMoveOnFinal.position, gizmoRadius);
				if (finalPosition) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (positionToMoveOnFinal.position, finalPosition.position);
				}
			}
		}
	}
}