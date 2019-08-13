using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeProjectileTrailSystem : MonoBehaviour
{
	public bool trailActive;
	public Vector3 targetPosition;
	public TrailRenderer mainTrail;
	public float reduceTrailTimeSpeed = 4;
	public bool movementActive = true;

	public float speedMultiplier;

	float timerSpeed;

	Vector3 originalPosition;

	public float maxSpawnTime = 5;

	public bool destroyTrailAfterTime;

	float lastTimeInstantiated;

	void Update ()
	{
		if (trailActive) {
			if (movementActive) {

				timerSpeed += Time.deltaTime / speedMultiplier;
				transform.position = Vector3.Lerp (originalPosition, targetPosition, timerSpeed);

			} else {
				mainTrail.time -= Time.deltaTime * reduceTrailTimeSpeed;
				if (mainTrail.time <= 0) {
					Destroy (gameObject);
				}
			}

			if (destroyTrailAfterTime) {
				if (Time.time > lastTimeInstantiated + maxSpawnTime) {
					Destroy (gameObject);
				}
			}
		}
	}

	public void instantiateFakeProjectileTrail (Vector3 newTargetPosition)
	{
		originalPosition = transform.position;

		transform.SetParent (null);

		trailActive = true;

		Vector3 newTargetDirection = newTargetPosition - transform.position;

		transform.rotation = Quaternion.LookRotation (newTargetDirection);

		targetPosition = newTargetPosition;

		lastTimeInstantiated = Time.time;
	}

	public void setSpeedMultiplier (float newValue)
	{
		speedMultiplier = newValue;
	}

	public void stopTrailMovement ()
	{
		movementActive = false;
	}

	public void setDestroyTrailAfterTimeState (bool state)
	{
		destroyTrailAfterTime = state;
	}
}