using UnityEngine;
using System.Collections;

public class moveObject : MonoBehaviour
{
	public float speed;
	public float moveAmount;
	public Vector3 direction = Vector3.up;
	public Vector3 originalPosition;

	public bool movementEnabled = true;

	void Start ()
	{
		originalPosition = transform.position;
	}

	void Update ()
	{
		if (movementEnabled) {
			transform.position = originalPosition + ((Mathf.Cos (Time.time * speed)) / 2) * moveAmount *
			(transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z);
		}
	}

	public void enableMovement ()
	{
		setMovementEnabledState (true);
	}

	public void disableMovement ()
	{
		setMovementEnabledState (false);
	}

	public void setMovementEnabledState (bool state)
	{
		movementEnabled = state;
	}

	public void changeMovementEnabledState ()
	{
		setMovementEnabledState (!movementEnabled);
	}
}
