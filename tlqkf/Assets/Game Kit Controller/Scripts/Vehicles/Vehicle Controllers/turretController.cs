using UnityEngine;
using System.Collections;

public class turretController : MonoBehaviour
{
	public otherVehicleParts vehicleParts;
	public vehicleSettings settings;
	public inputActionManager actionManager;
	public vehicleCameraController vCamera;

	public bool useHorizontalInputLerp = true;

	bool driving;

	float horizontalAxis;
	float lookAngle;

	Vector2 rawAxisValues;

	bool touchPlatform;

	float steering;
	Vector3 localLook;

	Transform vehicleCameraTransform;

	void Start ()
	{
		touchPlatform = touchJoystick.checkTouchPlatform ();

		vehicleCameraTransform = vCamera.transform;
	}

	void Update ()
	{
		if (driving && settings.turretCanRotate) {
			horizontalAxis = actionManager.getPlayerMovementAxis ("keys").x;

			if (!useHorizontalInputLerp && !touchPlatform) {
				rawAxisValues = actionManager.getPlayerRawMovementAxis ();
				horizontalAxis = rawAxisValues.x;
			}

			if (vCamera.currentState.useCameraSteer && horizontalAxis == 0) {
				localLook = vehicleParts.chassis.transform.InverseTransformDirection (vehicleCameraTransform.forward);

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;
				steering = Mathf.Clamp (steering, -1f, 1f);

				horizontalAxis = steering;
			}

			lookAngle -= horizontalAxis * settings.rotationSpeed;
			if (settings.rotationLimited) {
				lookAngle = Mathf.Clamp (lookAngle, -settings.clampTiltXTurret.x, settings.clampTiltXTurret.y);
			} 
			vehicleParts.chassis.transform.localRotation = Quaternion.Euler (0, -lookAngle, 0);
		}
	}

	//the player is getting on or off from the vehicle, so
	//public void changeVehicleState(Vector3 nextPlayerPos){
	public void changeVehicleState ()
	{
		driving = !driving;
		if (!driving) {
			StartCoroutine (resetTurretRotation ());
			lookAngle = 0;
		}
	}

	//the vehicle has been destroyed, so disabled every component in it
	public void disableVehicle ()
	{
		//disable the controller
		this.enabled = false;
	}

	//reset the weapon rotation when the player gets off
	IEnumerator resetTurretRotation ()
	{
		Quaternion currentBaseYRotation = vehicleParts.chassis.transform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			vehicleParts.chassis.transform.localRotation = Quaternion.Slerp (currentBaseYRotation, Quaternion.identity, t);
			yield return null;
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public GameObject chassis;
	}

	[System.Serializable]
	public class vehicleSettings
	{
		public bool turretCanRotate;
		public bool rotationLimited;
		public float rotationSpeed;
		public Vector2 clampTiltXTurret;
		public GameObject vehicleCamera;
	}
}
