using UnityEngine;
using System.Collections;

public class destroyGameObject : MonoBehaviour
{
	public float timer = 0.6f;
	public bool destroyObjectAtStart = true;

	Coroutine destroyObjectCoroutine;

	void Start ()
	{
		if (destroyObjectAtStart) {
			destroyObjectInTime ();
		}
	}

	public void destroyObjectInTime ()
	{
		stopDestroyObjectCoroutine ();
		destroyObjectCoroutine = StartCoroutine (destroyObjectInTimeCoroutine ());
	}

	public void stopDestroyObjectCoroutine ()
	{
		if (destroyObjectCoroutine != null) {
			StopCoroutine (destroyObjectCoroutine);
		}
	}

	IEnumerator destroyObjectInTimeCoroutine ()
	{
		yield return new WaitForSeconds (timer);
		destroy ();
	}

	public void setTimer (float timeToDestroy)
	{
		timer = timeToDestroy;
	}

	void OnDisable ()
	{
		if (timer > 0) {
			destroy ();
		}
	}

	public void destroy ()
	{
		Destroy (gameObject);
	}
}