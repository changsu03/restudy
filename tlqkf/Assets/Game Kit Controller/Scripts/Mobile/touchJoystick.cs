using UnityEngine;
using System.Reflection;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class touchJoystick : MonoBehaviour
{
	public Aligns align;
	[Range (0, 3)] public float padSize;
	public bool useMargins;
	public Vector2 margins = new Vector2 (3, 3);
	public Vector2 touchZoneSize = new Vector2 (3, 3);
	public Vector2 touchZoneOffset;
	public float dragDistance = 1;
	public bool snapsToFinger = true;
	public bool hideOnRelease = false;
	public bool touchPad;

	public float touchPadHorizontalExtraValue = 1;
	public float touchPadVerticalExtraValue = 1;

	public bool showJoystick;
	public float changeColorSpeed;
	public Color regularBaseColor;
	public Color regularStickColor;
	public Color pressedBaseColor;
	public Color pressedStickColor;
	public bool touching;
	public GameObject stick;
	public GameObject stickBase;

	public bool setPositionAtStart;

	public Vector2 currentAxisValue;

	Rect touchZoneRect;

	int currentFingerId;
	bool touchPlatform;
	Touch currentTouch;
	RawImage baseRenderer;
	RawImage stickRenderer;
	Color currentBaseColor;
	Color currentStickColor;

	Vector3 previousPosition;
	Vector2 originalStickPosition;
	Vector2 originalBasePosition;
	Vector3 globalTouchPosition;
	Vector3 differenceVector;
	Vector3 difference;
	RectTransform rectBase;
	RectTransform rectStick;
	int touchCount;

	//an enum to set the side of the joystick, left or right
	public enum Aligns
	{
		Left = -1,
		Right = 1
	}

	void Start ()
	{
		touchPlatform = checkTouchPlatform ();

		if (setPositionAtStart) {
			setJoystickPosition ();
		}

		if (!showJoystick && Application.isPlaying) {
			setSticksState (false, false);
		}

		rectStick = stick.GetComponent<RectTransform> ();
		rectBase = stickBase.GetComponent<RectTransform> ();
		originalStickPosition = rectStick.anchoredPosition;
		originalBasePosition = rectBase.anchoredPosition;
	}

	void Update ()
	{
		setJoystickColors ();

		if (moveJoystick ()) {
			return;
		}

		//search for any touch created with a finger if the game is not in editor mode, or with the mouse in the editor
		int touchCount = Input.touchCount;
		if (!touchPlatform) {
			touchCount++;
		}

		for (int i = 0; i < touchCount; i++) {
			if (!touchPlatform) {
				currentTouch = convertMouseIntoFinger ();
			} else {
				currentTouch = Input.GetTouch (i);
			}

			//if the touch action has started, check if the finger is inside the touch zone rect, visible in the editor
			if (currentTouch.phase == TouchPhase.Began) {

				if (touchZoneRect.Contains (currentTouch.position)) {
					currentFingerId = currentTouch.fingerId;

					fingerTouching (true);

					if (snapsToFinger) {
						rectStick.position = currentTouch.position;
						rectBase.position = rectStick.position;
					}

					if (touchPad) {
						previousPosition = currentTouch.position;
					}
				}
			}
		}
	}

	//set the position of the joystick in the right or left side of the screen, according to the screen size
	public void setJoystickPosition ()
	{
		if (useMargins) {
			Vector2 newPosition = Vector2.zero;
			newPosition.x = (int)align * (Screen.currentResolution.width / 2 - margins.x);
			newPosition.y = -Screen.currentResolution.height / 2 + margins.y;
			transform.GetComponent<RectTransform> ().anchoredPosition = newPosition;
		}
		transform.localScale = new Vector3 (padSize, padSize, 1);
		touchZoneRect = new Rect (transform.position.x - touchZoneSize.x / 2f + touchZoneOffset.x, transform.position.y - touchZoneSize.y / 2f + touchZoneOffset.y, touchZoneSize.x, touchZoneSize.y);
	}

	// set the value of touching to activate of deactivate the icons of the joystick
	void fingerTouching (bool state)
	{
		touching = state;
		if (showJoystick) {
			if (hideOnRelease) {
				setSticksState (state, state);
			} else if ((!stickBase.gameObject.activeSelf || !stick.gameObject.activeSelf)) {
				setSticksState (true, true);
			}
		} else {
			setSticksState (false, false);
		}
	}

	void setSticksState (bool stickBaseState, bool stickState)
	{
		stickBase.gameObject.SetActive (stickBaseState);
		stick.gameObject.SetActive (stickState);
	}

	//if the joystick is released, the icons back to their default positions
	void resetJoystickPosition ()
	{
		rectStick.anchoredPosition = originalStickPosition;
		rectBase.anchoredPosition = originalBasePosition;
		currentAxisValue = Vector2.zero;
		fingerTouching (false);
	}

	//get touch id of a finger or the mouse
	Touch? getTouch (int fingerId)
	{
		if (!touchPlatform) {
			if (fingerId == 11) {
				return convertMouseIntoFinger ();
			}
		}
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++) {
			Touch touch = Input.GetTouch (i);
			if (touch.fingerId == fingerId) {
				return touch;
			}
		}
		return null;
	}

	//check if the joystick is being used and set the icons position according to the finger or mouse movement
	bool moveJoystick ()
	{
		if (touching) {
			Touch? touch = getTouch (currentFingerId);
			if (touch == null || touch.Value.phase == TouchPhase.Ended) {
				resetJoystickPosition ();
				return false;
			}

			globalTouchPosition = touch.Value.position;
			differenceVector = globalTouchPosition - stickBase.transform.position;
			if (differenceVector.sqrMagnitude > dragDistance * dragDistance) {
				differenceVector.Normalize ();
				differenceVector *= dragDistance;
				rectStick.position = rectBase.position + differenceVector;
			} else {
				rectStick.position = globalTouchPosition;
			}

			if (!touchPad) {
				currentAxisValue = differenceVector / dragDistance;
			} else {
				difference = globalTouchPosition - previousPosition;
				if (differenceVector.sqrMagnitude > dragDistance * dragDistance) {
					difference.Normalize ();
				}
				currentAxisValue = (difference / dragDistance);

				currentAxisValue.x *= touchPadHorizontalExtraValue;
				currentAxisValue.y *= touchPadVerticalExtraValue;

				previousPosition = globalTouchPosition;
			}

			return true;
		}
		return false;
	}

	//get the vertical and horizontal axis values
	public Vector2 GetAxis ()
	{
		return currentAxisValue;
	}

	public Vector2 getRawAxis ()
	{
		Vector2 axisValues = Vector2.zero;

		if (currentAxisValue.x > 0) {
			axisValues.x = 1;
		} else if (currentAxisValue.x < 0) {
			axisValues.x = -1;
		} else {
			axisValues.x = 0;
		}

		if (currentAxisValue.y > 0) {
			axisValues.y = 1;
		} else if (currentAxisValue.y < 0) {
			axisValues.y = -1;
		} else {
			axisValues.y = 0;
		}

		return axisValues;
	}

	void setJoystickColors ()
	{
		if (touching) {
			currentBaseColor = pressedBaseColor;
			currentStickColor = pressedStickColor;
		} else {
			currentBaseColor = regularBaseColor;
			currentStickColor = regularStickColor;
		}

		if (!baseRenderer) {
			baseRenderer = stickBase.GetComponent<RawImage> ();
		} else {
			baseRenderer.color = Color.Lerp (baseRenderer.color, currentBaseColor, Time.deltaTime * changeColorSpeed);
		}

		if (!stickRenderer) {
			stickRenderer = stick.GetComponent<RawImage> ();
		} else {
			stickRenderer.color = Color.Lerp (stickRenderer.color, currentStickColor, Time.deltaTime * changeColorSpeed);
		}
	}

	//it simulates touch control in the game with the mouse position, using left button as tap finger with press, hold and release actions
	public static Touch convertMouseIntoFinger ()
	{
		object mouseFinger = new Touch ();
		FieldInfo phase = mouseFinger.GetType ().GetField ("m_Phase", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo fingerId = mouseFinger.GetType ().GetField ("m_FingerId", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo position = mouseFinger.GetType ().GetField ("m_Position", BindingFlags.NonPublic | BindingFlags.Instance);
		if (Input.GetMouseButtonDown (0)) {
			phase.SetValue (mouseFinger, TouchPhase.Began);
		} else if (Input.GetMouseButtonUp (0)) {
			phase.SetValue (mouseFinger, TouchPhase.Ended);
		} else {
			phase.SetValue (mouseFinger, TouchPhase.Moved);
		}
		fingerId.SetValue (mouseFinger, 11);
		position.SetValue (mouseFinger, new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
		return (Touch)mouseFinger;
	}

	//check the if the current platform is a touch device
	public static bool checkTouchPlatform ()
	{
		bool value = false;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			value = true;
		}
		return value;
	}

	public void setTouchPadState (bool state)
	{
		touchPad = state;
	}

	public void setShowJoystickState (bool state)
	{
		showJoystick = state;
		setSticksState (showJoystick, showJoystick);
	}

	//in editor mode, draw a rect in the joystick position, so the player can adjust the touch zone size visually,
	//and set the position of the joystick every time the editor changes
	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (!Application.isPlaying) {
			setJoystickPosition ();
			setJoystickColors ();
		}
		Gizmos.color = Color.red;
		Vector3 touchZone = new Vector3 (touchZoneRect.x + touchZoneRect.width / 2f, touchZoneRect.y + touchZoneRect.height / 2f, transform.position.z);
		Gizmos.DrawWireCube (touchZone, new Vector3 (touchZoneSize.x, touchZoneSize.y, 0f));
	}
}