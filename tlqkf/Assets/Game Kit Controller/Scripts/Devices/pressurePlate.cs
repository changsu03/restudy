﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class pressurePlate : MonoBehaviour
{
	public GameObject plate;
	public Transform finalPosition;
	public float minDistance;
	public bool usingPlate;
	public List<GameObject> objects = new List<GameObject> ();
	public bool activeFunctionCalled;
	public bool disableFunctionCalled;

	public UnityEvent unlockFunctionCall = new UnityEvent ();
	public UnityEvent lockFunctionCall = new UnityEvent ();

	List<GameObject> colliders = new List<GameObject> ();
	Coroutine setPlateState;

	void Start ()
	{
		Component[] components = GetComponentsInChildren (typeof(Collider));
		foreach (Component c in components) {
			colliders.Add (c.gameObject);
		}
		for (int i = 0; i < colliders.Count; i++) {
			if (colliders [i] != plate) {
				Physics.IgnoreCollision (plate.GetComponent<Collider> (), colliders [i].GetComponent<Collider> ());
			}
		}
	}

	void Update ()
	{
		if (usingPlate) {
			if ((Mathf.Abs (Mathf.Abs (plate.transform.position.y) - Mathf.Abs (finalPosition.position.y)) < minDistance) || plate.transform.position.y < finalPosition.position.y) {
				if (!activeFunctionCalled) {
					activeFunctionCalled = true;
					disableFunctionCalled = false;
					if (unlockFunctionCall.GetPersistentEventCount () > 0) {
						unlockFunctionCall.Invoke ();
					}
				}
			} else {
				if (activeFunctionCalled) {
					activeFunctionCalled = false;
					disableFunctionCalled = true;
					if (lockFunctionCall.GetPersistentEventCount () > 0) {
						lockFunctionCall.Invoke ();
					}
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && col.gameObject.tag != "Player") {
			checkCoroutine (true);
			if (!objects.Contains (col.gameObject) && col.gameObject != plate) {
				objects.Add (col.gameObject);
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && col.gameObject.tag != "Player") {
			if (objects.Contains (col.gameObject)) {
				objects.Remove (col.gameObject);
			}
			for (int i = 0; i < objects.Count; i++) {
				if (!objects [i]) {
					objects.RemoveAt (i);
				}
			}
			if (objects.Count == 0) {
				checkCoroutine (false);
			}
		}
	}

	void checkCoroutine (bool state)
	{
		if (setPlateState != null) {
			StopCoroutine (setPlateState);
		}
		setPlateState = StartCoroutine (enableOrDisablePlate (state));
	}

	IEnumerator enableOrDisablePlate (bool state)
	{
		if (state) {
			usingPlate = true;
			yield return null;
		} else {
			yield return new WaitForSeconds (1);
			usingPlate = false;
		}
	}

	void OnCollisionEnter (Collision col)
	{

	}
}