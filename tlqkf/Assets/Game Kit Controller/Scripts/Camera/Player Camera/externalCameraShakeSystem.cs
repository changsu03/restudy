using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class externalCameraShakeSystem : MonoBehaviour
{
	public string externalShakeName;
	public bool shakeUsingDistance;
	public float minDistanceToShake;
	public LayerMask layer;
	public bool useShakeListTriggeredByActions;
	public List<shakeTriggeredByActionInfo> shakeTriggeredByActionList = new List<shakeTriggeredByActionInfo> ();

	public bool useShakeEvent;
	public UnityEvent eventAtStart;
	public UnityEvent eventAtEnd;

	bool currentUseShakeEventValue;
	UnityEvent currentEventAtStart;
	UnityEvent currentEventAtEnd;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public int nameIndex;
	public string[] nameList;

	GameObject currentPlayer;
	externalShakeListManager externalShakeManager;
	float distancePercentage;
	headBob headBobManager;

	public void getExternalShakeList ()
	{
		if (!externalShakeManager) {
			externalShakeManager = FindObjectOfType<externalShakeListManager> ();
		}

		if (externalShakeManager) {
			nameList = new string[externalShakeManager.externalShakeInfoList.Count];
			for (int i = 0; i < externalShakeManager.externalShakeInfoList.Count; i++) {
				nameList [i] = externalShakeManager.externalShakeInfoList [i].name;
			}
			#if UNITY_EDITOR
			EditorUtility.SetDirty (GetComponent<externalCameraShakeSystem> ());
			#endif
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		playerController playerControllerManager = currentPlayer.GetComponent<playerController> ();
		if (playerControllerManager) {
			playerCamera playerCameraManager = playerControllerManager.getPlayerCameraManager ();
			headBobManager = playerCameraManager.getHeadBobManager ();
		}
	}

	public void setCameraShake ()
	{
		currentUseShakeEventValue = useShakeEvent;
		if (currentUseShakeEventValue) {
			currentEventAtStart = eventAtStart;
			currentEventAtEnd = eventAtEnd;
		}

		setCameraShakeByName (externalShakeName);

		currentUseShakeEventValue = false;
	}

	public void setCameraShakeByAction (string actionName)
	{
		for (int i = 0; i < shakeTriggeredByActionList.Count; i++) {
			if (shakeTriggeredByActionList [i].actionName == actionName) {

				currentUseShakeEventValue = shakeTriggeredByActionList [i].useShakeEvent;

				if (currentUseShakeEventValue) {
					currentEventAtStart = shakeTriggeredByActionList [i].eventAtStart;
					currentEventAtEnd = shakeTriggeredByActionList [i].eventAtEnd;
				}

				setCameraShakeByName (shakeTriggeredByActionList [i].shakeName);

				currentUseShakeEventValue = false;
				return;
			}
		}
	}

	public void setCameraShakeByName (string shakeName)
	{
		if (headBobManager && currentPlayer) {
			setExternalShakeSignal (currentPlayer, shakeName);
		} else {
			List<Collider> colliders = new List<Collider> ();
			colliders.AddRange (Physics.OverlapSphere (transform.position, minDistanceToShake, layer));
			foreach (Collider hit in colliders) {
				if (hit != null && !hit.isTrigger) {
					setExternalShakeSignal (hit.gameObject, shakeName);
				}
			}
		}
	}

	public void setExternalShakeSignal (GameObject objectToCall, string shakeName)
	{
		if (shakeUsingDistance) {
			float currentDistance = GKC_Utils.distance (objectToCall.transform.position, transform.position);
			if (currentDistance <= minDistanceToShake) {
				distancePercentage = currentDistance / minDistanceToShake;
				distancePercentage = 1 - distancePercentage;
			} else {
				return;
			}
		} else {
			distancePercentage = 1;
		}
			
		playerCamera playerCameraManager = objectToCall.GetComponent<playerController> ().getPlayerCameraManager ();
		headBobManager = playerCameraManager.getHeadBobManager ();
		if (headBobManager) {

			if (currentUseShakeEventValue) {
				headBobManager.setCurrentExternalCameraShakeSystemEvents (currentEventAtStart, currentEventAtEnd);
			} else {
				headBobManager.disableUseShakeEventState ();
			}

			headBobManager.setExternalShakeStateByName (shakeName, distancePercentage);
		}
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
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position, minDistanceToShake);
		}
	}

	[System.Serializable]
	public class shakeTriggeredByActionInfo
	{
		public string actionName;
		public string shakeName;
		public int nameIndex;

		public bool useShakeEvent;
		public UnityEvent eventAtStart;
		public UnityEvent eventAtEnd;
	}
}
