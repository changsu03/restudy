using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class dragObjectSystem : MonoBehaviour
{
	public Rigidbody mainRigidbody;
	public float distanceToPlayer;

	public Transform playerDragPosition;
	public Transform rightIKHandPosition;
	public Transform leftIKHandPosition;

	public bool objectGrabbed;

	public float checkObstacleRaycastDistance;

	public Collider obstacleTrigger;

	public float dragObjectSpeed = 36;

	public bool obstacleFound;

	public float minDistanceToDropObject = 1;

	public float playerDistance;

	public float adjustObjectPositionSpeed = 5;

	public UnityEvent eventOnFarFromObject;

	Transform mainTransform;

	Transform currentPlayerTransform;

	Vector2 axisValues;

	Animator animator;
	playerInputManager playerInput;

	Ray ray;

	Vector3 rigidbodyPosition;
	Vector3 currentPoint;
	Vector3 objectPosition;
	Vector3 objectDirection;
	Vector3 objectRotation;

	float objectXPosition;
	float objectZPosition;

	Coroutine objectMovement;

	bool adjustingObjectPosition;

	float extraAngle;

	void Start ()
	{
		mainTransform = transform;
	}

	void FixedUpdate ()
	{
		if (objectGrabbed) {
			dragObject ();
		}
	}

	public void dragObject ()
	{
		if (adjustingObjectPosition) {
			return;
		}

		ray = new Ray (currentPlayerTransform.position, currentPlayerTransform.forward);

		axisValues = playerInput.getPlayerMovementAxis ("keys");

		currentPoint = ray.GetPoint (distanceToPlayer);
		objectPosition = new Vector3 (currentPoint.x, mainTransform.position.y, currentPoint.z);
		objectDirection = Quaternion.Euler (currentPlayerTransform.eulerAngles) * new Vector3 (axisValues.x, 0, axisValues.y);

		ray = new Ray (mainTransform.position, objectDirection);
		currentPoint = ray.GetPoint (checkObstacleRaycastDistance);
		if (objectDirection.magnitude > 0) {
			obstacleTrigger.transform.position = currentPoint;
		} else {
			obstacleTrigger.transform.localPosition = Vector3.zero;
		}

		rigidbodyPosition = mainRigidbody.position;
		objectXPosition = Mathf.Clamp (rigidbodyPosition.x - objectPosition.x, -0.1f, 0.1f);
		objectZPosition = Mathf.Clamp (rigidbodyPosition.z - objectPosition.z, -0.1f, 0.1f);

		objectPosition = new Vector3 (objectPosition.x + objectXPosition, mainTransform.position.y, objectPosition.z + objectZPosition);

		mainRigidbody.position = Vector3.Lerp (mainTransform.position, objectPosition, dragObjectSpeed * Time.fixedDeltaTime);

		objectRotation = mainTransform.eulerAngles;
		objectRotation.y = (currentPlayerTransform.eulerAngles).y + extraAngle;
		mainTransform.eulerAngles = objectRotation;

		playerDistance = Vector3.Distance (playerDragPosition.position, currentPlayerTransform.position);

		if (playerDistance >= minDistanceToDropObject) {
			eventOnFarFromObject.Invoke ();
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayerTransform = newPlayer.transform;
		animator = newPlayer.GetComponent<Animator> ();

		playerInput = currentPlayerTransform.GetComponent<playerInputManager> ();
	}

	public void grabObject ()
	{
		translateObjectToGrabPosition ();

		objectGrabbed = true;
	}

	public void dropObject ()
	{            
		objectGrabbed = false;
	}

	public void setObstacleFoundState (bool state)
	{
		obstacleFound = state;

		mainRigidbody.isKinematic = state;
	}

	public void translateObjectToGrabPosition ()
	{
		stopTranslateObjectToGrabPosition ();

		objectMovement = StartCoroutine (translateObjectToGrabPositionCoroutine ());
	}

	public void stopTranslateObjectToGrabPosition ()
	{
		if (objectMovement != null) {
			StopCoroutine (objectMovement);
		}
	}

	IEnumerator translateObjectToGrabPositionCoroutine ()
	{
		float dist = GKC_Utils.distance (mainTransform.position, currentPlayerTransform.position);
		float duration = dist / adjustObjectPositionSpeed;
		float t = 0;

		float movementTimer = 0;

		bool targetReached = false;

		adjustingObjectPosition = true;

		ray = new Ray (currentPlayerTransform.position, currentPlayerTransform.forward);

		currentPoint = ray.GetPoint (distanceToPlayer);
		objectPosition = new Vector3 (currentPoint.x, mainTransform.position.y, currentPoint.z);

		rigidbodyPosition = mainRigidbody.position;
		objectXPosition = Mathf.Clamp (rigidbodyPosition.x - objectPosition.x, -0.1f, 0.1f);
		objectZPosition = Mathf.Clamp (rigidbodyPosition.z - objectPosition.z, -0.1f, 0.1f);

		objectPosition = new Vector3 (objectPosition.x + objectXPosition, mainTransform.position.y, objectPosition.z + objectZPosition);

		Vector3 currentPlayerPosition = currentPlayerTransform.position;
		currentPlayerPosition.y = 0;

		Vector3 currentObjectPosition = mainTransform.position;
		currentObjectPosition.y = 0;

		Vector3 playerDirection = (currentObjectPosition - currentPlayerPosition).normalized;

		print (playerDirection);

		extraAngle = Vector3.SignedAngle (playerDirection, mainTransform.forward, Vector3.up);

		print (extraAngle);

		int orientation = (extraAngle > 0) ? 1 : -1;

		float extraAngleABS = Mathf.Abs (extraAngle);

		print (extraAngleABS);

		if (extraAngleABS < 45) {
			extraAngle = 0;
		} 

		if (extraAngleABS > 45 && extraAngleABS < 135) {
			extraAngle = 90;
		} 

		if (extraAngleABS > 135 && extraAngleABS < 235) {
			extraAngle = 180;
		}

		if (extraAngleABS > 235) {
			extraAngle = 270;
		}

		extraAngle *= orientation;

		print (extraAngle); 

		Vector3 targetEuler = currentPlayerTransform.eulerAngles + mainTransform.up * extraAngle;

		Quaternion targetRotation = Quaternion.Euler (targetEuler);

		while (!targetReached) {
			t += Time.deltaTime * duration;
			mainTransform.position = Vector3.Lerp (mainTransform.position, objectPosition, t);
			mainTransform.rotation = Quaternion.Slerp (mainTransform.rotation, targetRotation, t);

			movementTimer += Time.deltaTime;

			if (GKC_Utils.distance (mainTransform.position, objectPosition) < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}
			yield return null;
		}

		adjustingObjectPosition = false;

		playerDragPosition.position = currentPlayerTransform.position;
	}
}