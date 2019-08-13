using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class grabPhysicalObjectSystem : MonoBehaviour
{
	public bool grabObjectPhysically = true;

	public bool objectIsCharacter;
	public GameObject character;
	public Transform characterBody;

	public bool IKSystemEnabled = true;

	[Range (0, 1)] public float animationSpeed = 1;

	public bool useRightHand = true;
	public bool useLeftHand = true;
	public Transform rightHandPosition;
	public Transform lefHandPosition;

	public bool useRightElbow;
	public bool useLeftElbow;
	public Transform rightElbowPosition;
	public Transform lefElbowPosition;

	public LayerMask layerForUsers;

	public GameObject objectToGrabParent;

	public Collider grabObjectTrigger;

	public Transform referencePosition;

	public bool useReferencePositionForEveryView;
	public Transform referencePositionThirdPerson;
	public Transform referencePositionFirstPerson;

	public Vector3 colliderScale;
	public Vector3 colliderOffset;

	public bool setRigidbodyMassValue;
	public float rigidbodyMassValue = 1;

	public bool setExtraListKinematic;
	public List<Rigidbody> extraListKinematic = new List<Rigidbody> ();

	public bool objectGrabed;

	List<Rigidbody> rigidbodyList = new List<Rigidbody> ();
	List<float> rigidbodyMassList = new List<float> ();

	List<int> layerMaskList = new List<int> ();

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	grabObjects grabObjectsManager;

	public List<grabObjects> grabObjectsList = new List<grabObjects> ();

	GameObject currentUser;
	grabObjects currentGrabObjectDetected;

	void Start ()
	{
		if (objectToGrabParent == null) {
			objectToGrabParent = gameObject;
		}
	}

	public void setRigidbodyList ()
	{
		if (rigidbodyList.Count == 0) {
			Component[] components = objectToGrabParent.GetComponentsInChildren (typeof(Rigidbody));
			foreach (Rigidbody child in components) {
				rigidbodyMassList.Add (child.mass);
				rigidbodyList.Add (child);
				layerMaskList.Add (child.gameObject.layer);
			}
		}
	}

	public void grabObject (GameObject currentPlayer)
	{
		if (objectGrabed) {
			return;
		}

		//print ("grab object");
		objectGrabed = true;
		grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();
		grabObjectTrigger.enabled = false;
		grabObjectsManager.grabPhysicalObject (IKSystemEnabled, objectToGrabParent, animationSpeed, rightHandPosition, lefHandPosition, referencePosition, useRightHand, useLeftHand,
			useRightElbow, useLeftElbow, rightElbowPosition, lefElbowPosition);

		for (int i = 0; i < grabObjectsList.Count; i++) {
			if (grabObjectsList [i] != grabObjectsManager) {
				grabObjectsList [i] .removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);
			}
		}

		if (setRigidbodyMassValue) {

			setRigidbodyList ();

			for (int i = 0; i < rigidbodyList.Count; i++) {
				rigidbodyList [i].mass = rigidbodyMassValue;
				rigidbodyList [i].gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
			}
		} 

		if (setExtraListKinematic) {
			for (int i = 0; i < extraListKinematic.Count; i++) {
				extraListKinematic [i].isKinematic = true;
			}
		}

		if (objectIsCharacter) {
			character.GetComponent<ragdollActivator> ().setCheckGetUpPausedState (true);
		}
	}

	public void dropObject ()
	{
		//print ("drop object");
		objectGrabed = false;
		grabObjectsManager.dropPhysicalObject ();
		grabObjectsManager = null;
		grabObjectTrigger.enabled = true;

		if (setExtraListKinematic) {
			for (int i = 0; i < extraListKinematic.Count; i++) {
				extraListKinematic [i].isKinematic = false;
			}
		}

		if (setRigidbodyMassValue) {
			for (int i = 0; i < rigidbodyList.Count; i++) {
				rigidbodyList [i].mass = rigidbodyMassList [i];
				rigidbodyList [i].gameObject.layer = layerMaskList [i];
			}
		} 

		if (objectIsCharacter) {
			character.GetComponent<ragdollActivator> ().setCheckGetUpPausedState (false);

			characterBody.transform.position = objectToGrabParent.transform.position;
			objectToGrabParent.transform.SetParent (characterBody);
		}
	}

	public bool isUseReferencePositionForEveryViewActive ()
	{
		return useReferencePositionForEveryView;
	}

	public Transform getReferencePositionThirdPerson ()
	{
		return referencePositionThirdPerson;
	}

	public Transform getReferencePositionFirstPerson ()
	{
		return referencePositionFirstPerson;
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (objectGrabed) {
			return;
		}
	
		if ((1 << col.gameObject.layer & layerForUsers.value) == 1 << col.gameObject.layer) {
			//if the player is entering in the trigger
			currentGrabObjectDetected = col.gameObject.GetComponent<grabObjects> ();

			if (isEnter) {
				currentUser = col.gameObject;
				if (grabObjectPhysically) {
					if (objectIsCharacter) {
						if (character == currentUser) {
							return;
						}
					}
			
					grabObjectsList.Add (currentGrabObjectDetected);

					currentGrabObjectDetected.addCurrentPhysicalObjectToGrabFound (objectToGrabParent);
				}
			} else {
				currentGrabObjectDetected.removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);

				grabObjectsList.Remove (currentGrabObjectDetected);
			}
		}
	}

	public bool isGrabObjectPhysicallyEnabled ()
	{
		return grabObjectPhysically;
	}

	public void disableGrabPhysicalObject ()
	{
		grabObjectPhysically = false;
		if (currentUser) {
			currentUser.GetComponent<grabObjects> ().removeCurrentPhysicalObjectToGrabFound (objectToGrabParent);
		}
	}

	//draw the lines of the pivot camera in the editor
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
			Gizmos.color = gizmoColor;

			Gizmos.DrawCube (transform.position + colliderOffset, colliderScale);

			if (rightHandPosition) {
				Gizmos.DrawSphere (rightHandPosition.position, 0.2f);
			}

			if (lefHandPosition) {
				Gizmos.DrawSphere (lefHandPosition.position, 0.2f);
			}
		}
	}
}
