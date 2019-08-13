using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObjectPositionSystem : MonoBehaviour
{
	public bool followObjectActive = true;
	public Transform objectToFollow;
	public bool followPosition = true;
	public bool followRotation = true;
	public Transform mainTransform;

	void Start ()
	{
		if (mainTransform == null) {
			mainTransform = transform;
		}
	}

	void Update ()
	{
		if (followObjectActive && objectToFollow) {
			mainTransform.position = objectToFollow.position;
			mainTransform.rotation = objectToFollow.rotation;
		}
	}
}
