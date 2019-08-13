using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleEventSystem : MonoBehaviour
{
	public UnityEvent eventToCallOnActivate = new UnityEvent ();
	public UnityEvent eventToCallOnDisable = new UnityEvent ();
	public bool activated;

	public void activateDevice ()
	{
		activated = !activated;
		if (activated) {
			if (eventToCallOnActivate.GetPersistentEventCount () > 0) {
				eventToCallOnActivate.Invoke ();
			}
		} else {
			if (eventToCallOnDisable.GetPersistentEventCount () > 0) {
				eventToCallOnDisable.Invoke ();
			}
		}
	}
}
