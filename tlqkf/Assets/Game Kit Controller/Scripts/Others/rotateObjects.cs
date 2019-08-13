﻿using UnityEngine;
using System.Collections;

public class rotateObjects : MonoBehaviour
{
	public Vector3 direccion;
	public float speed = 1;
	public bool rotationEnabled = true;

	public float resetRotationSpeed = 5;

	void Update ()
	{
		if (rotationEnabled) {
			transform.Rotate (speed * direccion * Time.deltaTime);
		}
	}

	public void enableOrDisableRotation ()
	{
		rotationEnabled = !rotationEnabled;
	}

	public void increaseRotationSpeedTenPercentage ()
	{
		speed += speed * 0.1f;
	}

	public void setRotationEnabledState (bool state)
	{
		stopRotation ();
		rotationEnabled = state;
	}

	public void disableRotation ()
	{
		setRotationEnabledState (false);
	}

	public void enableRotation ()
	{
		setRotationEnabledState (true);
	}

	Coroutine rotationCoroutine;

	public void resetRotation ()
	{
		stopRotation ();
		rotationCoroutine = StartCoroutine (resetRotationCoroutine ());
	}

	public void stopRotation ()
	{
		if (rotationCoroutine != null) {
			StopCoroutine (rotationCoroutine);
		}
	}

	IEnumerator resetRotationCoroutine ()
	{
		Quaternion targetRotation = Quaternion.identity;
		float t = 0;
		while (transform.localRotation != targetRotation) {
			t += Time.deltaTime / resetRotationSpeed;
			transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRotation, t);
			yield return null;
		}
	}

	public void setRandomDirectionToCurrent ()
	{
		int newDirection = Random.Range (0, 2);
		if (newDirection == 0) {
			newDirection = -1;
		}
		direccion *= newDirection;
	}
}
