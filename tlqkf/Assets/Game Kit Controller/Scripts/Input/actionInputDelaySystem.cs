using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class actionInputDelaySystem : MonoBehaviour
{
	public float actionDelay;

	public bool useLowerDelay;

	public bool pressedDown;

	public bool previousPressedDown;

	public pressType pressTypeCheck;

	public UnityEvent actionEvent;

	public bool eventTriggered;

	public enum pressType
	{
		hold,
		up
	}

	float lastTimePressedDown;

	public void inputSetPressedDownState ()
	{
		pressedDown = true;

		if (previousPressedDown != pressedDown) {
			previousPressedDown = pressedDown;

			lastTimePressedDown = Time.time;
			eventTriggered = false;
		}

		if (!eventTriggered && pressTypeCheck == pressType.hold && Time.time > lastTimePressedDown + actionDelay) {
			actionEvent.Invoke ();
			eventTriggered = true;
		}
	}

	public void inpuSetPressedUpState ()
	{
		pressedDown = false;

		if (previousPressedDown != pressedDown) {
			previousPressedDown = pressedDown;
			eventTriggered = false;
		}

		if (!eventTriggered && pressTypeCheck == pressType.up) {
			if (useLowerDelay) {
				if (Time.time < lastTimePressedDown + actionDelay) {
					actionEvent.Invoke ();
					eventTriggered = true;
				}
			} else {
				if (Time.time > lastTimePressedDown + actionDelay) {
					actionEvent.Invoke ();
					eventTriggered = true;
				}
			}
		}
	}
}
