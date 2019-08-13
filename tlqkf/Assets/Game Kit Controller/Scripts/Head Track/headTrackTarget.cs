using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class headTrackTarget : MonoBehaviour
{
	public Transform targetToLook;
	public Vector3 positionOffset;
	public float minDistanceToLook = 4;
	public targetVisibilityTypes visibilityTypes;
	public List<string> tagsToLocate = new List<string> ();

	public bool useCustomLayer;
	public LayerMask customLayer;

	public bool showGizmo;
	public Color gizmoColor = Color.yellow;

	public bool targetEnabled = true;

	public enum targetVisibilityTypes
	{
		None,
		Raycast
	}

	headTrack currentHeadTrack;
	headTrackTarget currentHeadTrackTarget;

	LayerMask currentLayerMask;
	Vector3 positionToLook;
	RaycastHit hit;

	void Start ()
	{
		currentHeadTrackTarget = GetComponent<headTrackTarget> ();
	}

	public Vector3 getLookPositon ()
	{
		if (targetToLook) {
			return targetToLook.position + positionOffset;
		} else {
			return transform.TransformPoint (positionOffset);
		}
	}

	public bool lookTargetVisible (Vector3 headPosition, LayerMask layer)
	{
		if (!targetEnabled) {
			return false;
		}

		positionToLook = getLookPositon ();

		if (visibilityTypes == targetVisibilityTypes.None) {
			if (GKC_Utils.distance (headPosition, positionToLook) > minDistanceToLook) {
				return false;
			} else {
				return true;
			}
		} else if (visibilityTypes == targetVisibilityTypes.Raycast) {
			if (GKC_Utils.distance (headPosition, positionToLook) > minDistanceToLook) {
				return false;
			} else {
				if (useCustomLayer) {
					currentLayerMask = customLayer;
				} else {
					currentLayerMask = layer;
				}

				if (Physics.Linecast (headPosition, positionToLook, out hit, currentLayerMask)) {
					if (hit.transform != transform) {
						drawLine (headPosition, hit.point, Color.red);
						return false;
					} else {
						drawLine (headPosition, hit.point, Color.green);
						return true;
					}
				} else {
					drawLine (headPosition, positionToLook, Color.green);
					return true;
				}
			}
		}
		return false;
	}

	public void drawLine (Vector3 startPosition, Vector3 endPosition, Color color)
	{
		if (showGizmo) {
			Debug.DrawLine (startPosition, endPosition, color);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (tagsToLocate.Contains (other.tag)) {
			currentHeadTrack = other.gameObject.GetComponent<headTrack> ();

			if (currentHeadTrack) {
				currentHeadTrack.checkHeadTrackTarget (currentHeadTrackTarget);
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (tagsToLocate.Contains (other.tag)) {
			currentHeadTrack = other.gameObject.GetComponent<headTrack> ();

			if (currentHeadTrack) {
				currentHeadTrack.removeHeadTrackTarget (currentHeadTrackTarget);
			}
		}
	}

	public void setEnableState (bool state)
	{
		targetEnabled = state;
	}

	public void disableState ()
	{
		setEnableState (false);
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
		if (showGizmo && targetEnabled) {
			Gizmos.color = gizmoColor;
			Vector3 position = getLookPositon ();
			Gizmos.DrawWireSphere (position, minDistanceToLook);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (position, 0.2f);
		}
	}
}