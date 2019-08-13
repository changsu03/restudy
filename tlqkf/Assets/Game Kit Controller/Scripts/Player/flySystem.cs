using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class flySystem : MonoBehaviour
{
	public bool flyModeEnabled = true;
	public bool flyModeActive;
	public IKFlyInfo IKInfo;

	public float flyForce;
	public float flyAirSpeed;
	public float flyAirControl;
	public float flyTurboSpeed;
	public float limbsMovementSpeed;
	public float limbsMovementSmooth;

	public playerController playerManager;
	public IKSystem IKManager;

	public UnityEvent eventOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public bool showGizmo;

	int i;
	bool turboEnabled;

	void Start ()
	{
		for (i = 0; i < IKInfo.IKGoals.Count; i++) {
			IKInfo.IKGoals [i].originalLocalPosition = IKInfo.IKGoals [i].position.localPosition;
		}
	}

	void FixedUpdate ()
	{
		if (flyModeActive) {
			for (i = 0; i < IKInfo.IKGoals.Count; i++) {
				float posTargetY = Mathf.Sin (Time.time * limbsMovementSpeed) * IKInfo.IKGoals [i].limbMovementAmount;
				IKInfo.IKGoals [i].position.position = Vector3.MoveTowards (IKInfo.IKGoals [i].position.position, IKInfo.IKGoals [i].position.position + posTargetY * transform.up, Time.deltaTime * limbsMovementSmooth);
			}
		}
	}

	public void resetLimbsPositons ()
	{
		for (i = 0; i < IKInfo.IKGoals.Count; i++) {
			IKInfo.IKGoals [i].position.localPosition = IKInfo.IKGoals [i].originalLocalPosition;
		}
	}

	public void enableOrDisableFlyingMode (bool state)
	{
		if (!flyModeEnabled) {
			return;
		}

		flyModeActive = state;
		playerManager.enableOrDisableFlyingMode (state, flyForce, flyAirSpeed, flyAirControl, flyTurboSpeed);
		IKManager.flyingModeState (state, IKInfo);

		if (flyModeActive) {
			eventOnStateEnabled.Invoke ();
		} else {
			eventOnStateDisabled.Invoke ();
		}
	}

	public void enableOrDisableTurbo (bool state)
	{
		turboEnabled = state;
		playerManager.enableOrDisableFlyModeTurbo (turboEnabled);
	}

	public void inputChangeTurboState (bool state)
	{
		if (flyModeActive) {
			if (state) {
				enableOrDisableTurbo (true);
			} else {
				enableOrDisableTurbo (false);
			}
		}
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
			for (i = 0; i < IKInfo.IKGoals.Count; i++) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (IKInfo.IKGoals [i].position.position, 0.05f);
			}
			for (i = 0; i < IKInfo.IKHints.Count; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (IKInfo.IKHints [i].position.position, 0.05f);
			}
		}
	}

	[System.Serializable]
	public class IKFlyInfo
	{
		public List<IKGoalsFlyPositions> IKGoals = new List<IKGoalsFlyPositions> ();
		public List<IKHintsFlyPositions> IKHints = new List<IKHintsFlyPositions> ();
	}

	[System.Serializable]
	public class IKGoalsFlyPositions
	{
		public string Name;
		public AvatarIKGoal limb;
		public Transform position;
		public float limbMovementAmount;
		[HideInInspector] public Vector3 originalLocalPosition;
	}

	[System.Serializable]
	public class IKHintsFlyPositions
	{
		public string Name;
		public AvatarIKHint limb;
		public Transform position;
	}
}