using UnityEngine;
using System.Collections;

public class sphereController : MonoBehaviour
{
	public otherVehicleParts vehicleParts;
	public vehicleSettings settings;
	public float currentSpeed;
	public bool anyOnGround;
	public bool driving;
	public bool isTurnedOn;

	public Rigidbody mainRigidbody;
	public inputActionManager actionManager;
	public vehicleCameraController vCamera;
	public vehicleHUDManager hudManager;
	public vehicleGravityControl gravityManager;

	bool jump;
	bool moving;
	bool usingBoost;
	bool usingGravityControl;
	bool rotating;
	int i, j;
	int collisionForceLimit = 5;
	float boostInput = 1;
	float horizontalAxis;
	float verticalAxis;
	float originalJumpPower;
	Vector3 moveInput;

	Vector3 normal;

	Transform vehicleCameraTransform;

	Vector2 axisValues;

	void Start ()
	{
		originalJumpPower = settings.jumpPower;
		vehicleCameraTransform = vCamera.transform;
	}

	void Update ()
	{
		//if the player is driving this vehicle and the gravity control is not being used, then
		if (driving && !usingGravityControl) {
			if (isTurnedOn) {
				
				//get the current values from the input manager, keyboard and touch controls
				axisValues = actionManager.getPlayerMovementAxis ("keys");
				horizontalAxis = axisValues.x;
				verticalAxis = axisValues.y;
			}

			//if the boost input is enabled, check if there is energy enough to use it
			if (usingBoost) {
				//if there is enough energy, enable the boost
				if (hudManager.useBoost (moving)) {
					boostInput = settings.maxBoostMultiplier;
					usingBoosting ();
				} 
				//else, disable the boost
				else {
					usingBoost = false;
					//if the vehicle is not using the gravity control system, disable the camera move away action
					if (!gravityManager.isGravityPowerActive ()) {
						vCamera.usingBoost (false, "Boost");
					}
					usingBoosting ();
					boostInput = 1;
				}
			}

			//set the current speed in the HUD of the vehicle
			hudManager.getSpeed (currentSpeed, settings.maxForwardSpeed);
		} 

		//else, set the input values to 0
		else {
			horizontalAxis = 0;
			verticalAxis = 0;
		}

		moving = verticalAxis != 0;

		//if the vehicle has fuel, allow to move it
		if (moving) {
			if (!hudManager.useFuel ()) {
				if (isTurnedOn) {
					turnOnOrOff (false, isTurnedOn);
				}
			}
		}

		moveInput = verticalAxis * settings.vehicleCamera.transform.forward + horizontalAxis * settings.vehicleCamera.transform.right;	
		Vector3 force = moveInput * settings.moveSpeedMultiplier * boostInput;
		//substract the local Y axis velocity of the rigidbody
		//	force = force - settings.vehicleCamera.transform.up * transform.InverseTransformDirection (force).y;
		currentSpeed = mainRigidbody.velocity.magnitude;
		if (currentSpeed > settings.maxForwardSpeed) {
			mainRigidbody.AddForce (Vector3.zero);
		} else {
			mainRigidbody.AddForce (force);
		}
		anyOnGround = false;
		if (Physics.Raycast (vehicleParts.chassis.transform.position, -normal, gravityManager.settings.rayDistance, settings.layer)) {
			anyOnGround = true;
		}
	}

	//if the vehicle is using the gravity control, set the state in this component
	public void changeGravityControlUse (bool state)
	{
		usingGravityControl = state;
	}

	//the player is getting on or off from the vehicle, so
	//public void changeVehicleState (Vector3 nextPlayerPos)
	public void changeVehicleState ()
	{
		driving = !driving;
		//set the audio values if the player is getting on or off from the vehicle
		if (driving) {
			if (hudManager.autoTurnOnWhenGetOn) {
				turnOnOrOff (true, isTurnedOn);
			}
		} else {
			turnOnOrOff (false, isTurnedOn);
		}
		//set the same state in the gravity control components
		gravityManager.changeGravityControlState (driving);
	}

	public void setTurnOnState ()
	{

	}

	public void setTurnOffState (bool previouslyTurnedOn)
	{
		if (previouslyTurnedOn) {
			
		}
		boostInput = 1;
		//stop the boost
		if (usingBoost) {
			usingBoost = false;
			vCamera.usingBoost (false, "Boost");
			usingBoosting ();
			boostInput = 1;
		}
	}

	public void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		isTurnedOn = state;
		if (isTurnedOn) {
			setTurnOnState ();
		} else {
			setTurnOffState (previouslyTurnedOn);
		}
	}

	//the vehicle has been destroyed, so disabled every component in it
	public void disableVehicle ()
	{
		//stop the boost
		if (usingBoost) {
			usingBoost = false;
			vCamera.usingBoost (false, "Boost");
			usingBoosting ();
			boostInput = 1;
		}

		//disable the controller
		this.enabled = false;
	}

	//get the current normal in the gravity control component
	public void setNormal (Vector3 normalValue)
	{
		normal = normalValue;
	}

	//if any collider in the vehicle collides, then
	void OnCollisionEnter (Collision collision)
	{
		//check that the collision is not with the player
		if (collision.contacts.Length > 0 && collision.gameObject.tag != "Player") {	
			//if the velocity of the collision is higher that the limit
			if (collision.relativeVelocity.magnitude > collisionForceLimit) {
				//if the vehicle hits another vehicle, apply damage to both of them according to the velocity at the impact
				applyDamage.checkHealth (gameObject, collision.collider.gameObject, collision.relativeVelocity.magnitude * hudManager.damageMultiplierOnCollision, 
					collision.contacts [0].normal, collision.contacts [0].point, gameObject, false, true);
			}
		}
	}

	//if the vehicle is using the boost, set the boost particles
	public void usingBoosting ()
	{
		
	}

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		Vector3 jumpForce = direction;
		print (jumpForce);
		mainRigidbody.AddForce (jumpForce, ForceMode.VelocityChange);
	}

	public void setNewJumpPower (float newJumpPower)
	{
		settings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		settings.jumpPower = originalJumpPower;
	}

	//CALL INPUT FUNCTIONS
	public void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && settings.canJump && anyOnGround) {
			mainRigidbody.AddForce (normal * mainRigidbody.mass * settings.jumpPower);
		}
	}

	public void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//boost input
			if (holdingButton) {
				if (settings.canUseBoost) {
					usingBoost = true;
					//set the camera move away action
					vCamera.usingBoost (true, "Boost");
				}
			} else {
				//stop boost input
				usingBoost = false;
				//disable the camera move away action
				vCamera.usingBoost (false, "Boost");
				//disable the boost particles
				usingBoosting ();
				boostInput = 1;
			}
		}
	}

	public void inputBrake ()
	{
		if (driving && !usingGravityControl && isTurnedOn && anyOnGround) {
			float verticalVelocity = vehicleCameraTransform.InverseTransformDirection (mainRigidbody.velocity).y;
			//print (verticalVelocity);
			Vector3 downVelocity = vehicleCameraTransform.up * verticalVelocity;
			mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero + downVelocity, Time.deltaTime * settings.brakeForce);
		}
	}

	public void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl && hudManager.canSetTurnOnState) {
			turnOnOrOff (!isTurnedOn, isTurnedOn);
		} 
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public Transform COM;
		public GameObject chassis;
	}

	[System.Serializable]
	public class vehicleSettings
	{
		public LayerMask layer;
		public float maxForwardSpeed;
		public float maxBoostMultiplier;
		public float moveSpeedMultiplier;
		public GameObject vehicleCamera;
		public float jumpPower;
		public bool canJump;
		public bool canUseBoost;
		public float brakeForce = 5;
	}
}