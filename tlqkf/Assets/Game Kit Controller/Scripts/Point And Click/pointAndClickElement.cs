using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class pointAndClickElement : MonoBehaviour
{
	public bool elementEnabled = true;

	public GameObject objectToUse;

	public UnityEvent activateElementEvent;
	public UnityEvent deactivateElementEvent;

	public bool useElementTextInfo;
	[TextArea (1, 10)] public string elementTextInfo;

	public typeOfObject elementType;

	public bool useOnOffDeviceState;

	public bool isPickup;

	public bool activePanelAfterStopUse;
	public bool disablePanelAfterUse;

	public bool followObjectToUsePosition;

	public Transform positionForNavMesh;

	public bool useFrontRearPositionForNavMesh;
	public Transform frontPositionForNavMesh;
	public Transform rearPositionForNavMesh;

	public bool useCustomElementMinDistance;
	public float customElementMinDistance;

	public bool useCustomElementName;
	public string customElementName;

	public bool usingElement;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.green;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.3f;

	public enum typeOfObject
	{
		device,
		vehicle,
		friend,
		enemy
	}

	Transform objectToFollow;

	Collider mainCollider;

	playerNavMeshSystem currentPlayerNavMeshSystem;

	bool elementInstantiated;

	void Start ()
	{
		if (followObjectToUsePosition) {
			if (objectToUse) {
				objectToFollow = objectToUse.transform;
			}
		}
			
		mainCollider = GetComponent<Collider> ();
		elementInstantiated = true;
	}


	void Update ()
	{
		if (followObjectToUsePosition) {
			transform.position = objectToFollow.position;
		}
	}

	public void setElementEnabledState (bool state)
	{
		if (enabled && elementInstantiated) {
			elementEnabled = state;
			mainCollider.enabled = elementEnabled;
			if (currentPlayerNavMeshSystem) {
				currentPlayerNavMeshSystem.disablePanelInfo ();
			}
		}
	}

	public string getPointAndClickElementTextInfo ()
	{
		return elementTextInfo;
	}

	public void usePointAndClickElement ()
	{
		usingElement = !usingElement;
		if (elementType == typeOfObject.device) {

		} else if (elementType == typeOfObject.vehicle) {

		} else if (elementType == typeOfObject.friend) {

		} else if (elementType == typeOfObject.enemy) {

		}

		if (usingElement) {
			if (activateElementEvent.GetPersistentEventCount () > 0) {
				activateElementEvent.Invoke ();
			}
		} else {
			if (deactivateElementEvent.GetPersistentEventCount () > 0) {
				deactivateElementEvent.Invoke ();
			}
		}
	}

	public bool isDevice ()
	{
		return elementType == typeOfObject.device;
	}

	public bool isFriend ()
	{
		return elementType == typeOfObject.friend;
	}

	public bool isEnemy ()
	{
		return elementType == typeOfObject.enemy;
	}

	public GameObject getElementToUse ()
	{
		return objectToUse;
	}

	public Transform getPositionForNavMesh (Vector3 playerPosition)
	{
		if (useFrontRearPositionForNavMesh) {
			float dot = Vector3.Dot (transform.forward, (playerPosition - transform.position).normalized);
			if (dot > 0) {
				return frontPositionForNavMesh;
			}else{
				return rearPositionForNavMesh;
			}
		} else {
			return positionForNavMesh;
		}
	}

	public void removeElement ()
	{
		Destroy (gameObject);
	}

	public bool checkIfRemove ()
	{
		if (isPickup) {
			return true;
		}
		return false;
	}

	public void setCurrentPlayerNavMeshSystem (playerNavMeshSystem currentPlayer)
	{
		currentPlayerNavMeshSystem = currentPlayer;
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere (transform.position, gizmoRadius);
			if (useFrontRearPositionForNavMesh) {
				if (frontPositionForNavMesh) {
					Gizmos.DrawLine (transform.position, frontPositionForNavMesh.position);
					Gizmos.DrawSphere (frontPositionForNavMesh.position, gizmoRadius);
				}
				if (rearPositionForNavMesh) {
					Gizmos.DrawLine (transform.position, rearPositionForNavMesh.position);
					Gizmos.DrawSphere (rearPositionForNavMesh.position, gizmoRadius);
				}
			} else {
				if (positionForNavMesh) {
					Gizmos.DrawLine (transform.position, positionForNavMesh.position);
					Gizmos.DrawSphere (positionForNavMesh.position, gizmoRadius);
				}
			}
		}
	}
}
