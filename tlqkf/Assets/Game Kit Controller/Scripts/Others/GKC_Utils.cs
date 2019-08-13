using UnityEngine;
using System.Collections;

public class GKC_Utils : MonoBehaviour
{
	public static float getCurrentDeltaTime ()
	{
		if (Time.timeScale > 0) {
			return 1 / Time.timeScale;
		} else {
			return 1;
		}
	}

	public static float getCurrentScaleTime ()
	{
		if (Time.timeScale != 1) {
			return ((1f / Time.fixedDeltaTime) * 0.02f);
		}
		return 1;
	}

	public static void checkAudioSourcePitch (AudioSource audioSourceToCheck)
	{
		if (audioSourceToCheck) {
			audioSourceToCheck.pitch = Time.timeScale;
		}
	}

	public static float distance (Vector3 positionA, Vector3 positionB)
	{
		return Mathf.Sqrt ((positionA - positionB).sqrMagnitude);
	}

	//the four directions of a swipe
	public class swipeDirections
	{
		public static Vector2 up = new Vector2 (0, 1);
		public static Vector2 down = new Vector2 (0, -1);
		public static Vector2 right = new Vector2 (1, 0);
		public static Vector2 left = new Vector2 (-1, 0);
	}

	public static void ForGizmo (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawRay (pos, direction);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void drawGizmoArrow (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength, float arrowHeadAngle)
	{
		Gizmos.color = color;
		Gizmos.DrawRay (pos, direction);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void ForDebug (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay (pos, direction);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Debug.DrawRay (pos + direction, right * arrowHeadLength);
		Debug.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void ForDebug (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay (pos, direction, color);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Debug.DrawRay (pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay (pos + direction, left * arrowHeadLength, color);
	}

	public static void dropObject (GameObject currentPlayer, GameObject objectToDrop)
	{
		if (!currentPlayer) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();
		if (grabObjectsManager) {
			if (grabObjectsManager.grabbed) {
				grabObjectsManager.checkIfDropObject (objectToDrop);
			} else {
				currentPlayer.GetComponent<otherPowers> ().checkIfDropObject (objectToDrop);
			}
			return;
		}

		gravityGun currentGravityGun = currentPlayer.GetComponent<gravityGun> ();
		if (currentGravityGun) {
			if (currentGravityGun.isCarryingObject ()) {
				currentGravityGun.checkIfDropObject (objectToDrop);
			}
		}
	}
}
