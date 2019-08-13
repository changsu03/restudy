using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setRigidbodyStateSystem : MonoBehaviour
{
	public List<GameObject> rigidbodyList = new List<GameObject> ();
	public ForceMode forceMode;
	public float forceAmount;

	public float explosionRadius;
	public float explosioUpwardAmount;

	public void setKinematicState (bool state)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();
			if (currentRigidbody) {
				currentRigidbody.isKinematic = state;
			}
		}
	}

	public void addForce (Transform forceDirection)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();
			if (currentRigidbody) {
				currentRigidbody.AddForce (forceDirection.up * forceAmount, forceMode);
			}
		}
	}

	public void addExplosiveForce (Transform explosionCenter)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();
			if (currentRigidbody) {
				currentRigidbody.AddExplosionForce (forceAmount, explosionCenter.position, explosionRadius, explosioUpwardAmount, forceMode);
			}
		}
	}

	public void addForceToThis (Transform forceDirection)
	{
		Rigidbody currentRigidbody = GetComponent<Rigidbody> ();
		if (currentRigidbody) {
			currentRigidbody.AddForce (forceDirection.up * forceAmount, forceMode);
		}
	}
}
