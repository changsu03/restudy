using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class mouseCursorController : MonoBehaviour
{
	public RectTransform cursor;
	public float cursorSpeedOnGame = 400;
	public float cursorSpeedOnPause = 10;

	public float cursorLerpSpeed = 10;

	public int buttonIndex = 0;
	public int joystickNumber = 1;

	public playerInputManager playerInput;

	public inputManager input;

	public bool cursorCanBeEnabled;

	public bool enableGamepadMouseOnPause;

	public bool useMouseLimits = true;
	public Vector2 mouseLimits = new Vector2 (1520, 840);
		
	public Vector2 cursorPosition;
	Vector2 axisInput;
	float newX, newY;

	float currentMouseSpeed;

	[DllImport ("user32.dll")]
	static extern bool SetCursorPos (int X, int Y);

	[DllImport ("user32.dll")]
	static extern bool GetCursorPos (out Point pos);

	Point cursorPos = new Point ();

	void Start ()
	{
		resetCursorPosition ();
	}

	void Update ()
	{
		if (cursorCanBeEnabled && !input.isUsingTouchControls () && input.isUsingGamepad ()) {
			if (input.checkJoystickButton (joystickNumber, buttonIndex, inputManager.buttonType.getKeyDown)) {
				MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftDown);
			}

			if (input.checkJoystickButton (joystickNumber, buttonIndex, inputManager.buttonType.getKeyUp)) {
				MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftUp);
			}

			axisInput = playerInput.getPlayerMovementAxis ("mouse");

			GetCursorPos (out cursorPos);

			if (Time.deltaTime > 0) {
				currentMouseSpeed = Time.deltaTime * cursorSpeedOnGame;
			} else {
				currentMouseSpeed = cursorSpeedOnPause;

				if (!enableGamepadMouseOnPause) {
					return;
				}
			}

			if (axisInput.x > 0) {
				cursorPosition.x += currentMouseSpeed;
			} else if (axisInput.x < 0) {
				cursorPosition.x -= currentMouseSpeed;
			}
			if (axisInput.y > 0) {
				cursorPosition.y -= currentMouseSpeed;
			} else if (axisInput.y < 0) {
				cursorPosition.y += currentMouseSpeed;
			}

			if (useMouseLimits) {
				cursorPosition.x = Mathf.Clamp (cursorPosition.x, 0, mouseLimits.x);
				cursorPosition.y = Mathf.Clamp (cursorPosition.y, 0, mouseLimits.y);
			}

			if (Time.deltaTime > 0) {
				newX = Mathf.Lerp (newX, cursorPosition.x, Time.deltaTime * cursorLerpSpeed);
				newY = Mathf.Lerp (newY, cursorPosition.y, Time.deltaTime * cursorLerpSpeed);
			} else {
				newX = cursorPosition.x;
				newY = cursorPosition.y;
			}

			SetCursorPos ((int)newX, (int)newY);
		}
	}

	public void showOrHideCursor (bool state)
	{
		if (!input.isUsingGamepad ()) {
			return;
		}

		cursorCanBeEnabled = state;
		if (cursorCanBeEnabled) {
			resetCursorPosition ();
		}
	}

	public void resetCursorPosition ()
	{
		cursorPosition.x = (int)(Screen.width / 2);
		cursorPosition.y = (int)(Screen.height / 2);
		mouseLimits = new Vector3 (Screen.width, Screen.height);
	}

	public struct Point
	{
		public int X;
		public int Y;

		public Point (int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}
