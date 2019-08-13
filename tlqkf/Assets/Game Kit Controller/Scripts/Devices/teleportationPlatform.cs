using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class teleportationPlatform : MonoBehaviour
{
	public bool teleportEnabled = true;
	public Transform platformToMove;
	public LayerMask layermask;
	public GameObject objectInside;
	public bool useButtonToActivate;

	public bool setGravityDirection;

	public setGravity setGravityManager;

	public bool callEventOnTeleport;
	public UnityEvent eventOnTeleport;
	public bool callEventOnEveryTeleport;

	bool eventCalled;

	teleportationPlatform platformToMoveManager;
	RaycastHit hit;
	grabbedObjectState currentGrabbedObject;

	void Start ()
	{
		if (platformToMove) {
			platformToMoveManager = platformToMove.GetComponent<teleportationPlatform> ();
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && !objectInside) {
			objectInside = col.gameObject;

			if (!teleportEnabled) {
				return;
			}

			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = objectInside.GetComponent<grabbedObjectState> ();
			if (currentGrabbedObject) {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectInside);
			}

			if (!useButtonToActivate) {
				activateTeleport ();
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (objectInside && col.gameObject == objectInside) {
			objectInside = null;
			currentGrabbedObject = null;
		}
	}

	void activateDevice ()
	{
		if (!teleportEnabled) {
			return;
		}

		if (useButtonToActivate && objectInside) {
			activateTeleport ();
		}
	}

	public void activateTeleport ()
	{
		platformToMoveManager.sendObject (objectInside);

		if (callEventOnTeleport) {
			if (!eventCalled || callEventOnEveryTeleport) {
				eventOnTeleport.Invoke ();
				eventCalled = true;
			}
		}
	}

	public void sendObject (GameObject objetToMove)
	{
		if (Physics.Raycast (transform.position + transform.up * 2, -transform.up, out hit, Mathf.Infinity, layermask)) {
			objetToMove.transform.position = hit.point;
			objectInside = objetToMove;
		}

		if (setGravityDirection && setGravityManager) {
			Collider currentCollider = objectInside.GetComponent<Collider> ();
			if (currentCollider) {
				setGravityManager.checkTriggerType (currentCollider, true);
			}
		}
	}

	public void setTeleportEnabledState (bool state)
	{
		teleportEnabled = state;
	}
}