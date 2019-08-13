using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headExplodeExample : MonoBehaviour
{
	public bool useTimeBullet = true;
	public float timeBulletDuration = 3;
	public float timeScale = 0.2f;

	public Transform headTransform;

	public void explode ()
	{
		print ("function to explode head here");

		if (useTimeBullet) {
			timeBullet timeBulletManager = FindObjectOfType<timeBullet> ();
			if (timeBulletManager) {
				timeBulletManager.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
			}
		}

		if (headTransform) {
			headTransform.localScale = Vector3.zero;
		}
	}

	public void explodeBodyPart (Transform objectToExplode)
	{
		if (objectToExplode) {
			objectToExplode.localScale = Vector3.zero;
		}
	}
}
