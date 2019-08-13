using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventObjectFoundOnCaptureSystem : MonoBehaviour
{
	public UnityEvent eventToCallOnCapture;

	public void callEventOnCapture ()
	{
		eventToCallOnCapture.Invoke ();
	}
}
