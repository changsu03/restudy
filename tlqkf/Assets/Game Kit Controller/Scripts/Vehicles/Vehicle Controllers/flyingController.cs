using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class flyingController : MonoBehaviour
{
	public float timeToFlip = 2;
	public float audioEngineSpeed;
	public float engineMinVolume = 0.5f;
	public float engineMaxVolume = 1;
	public float minAudioPitch = 0.5f;
	public float maxAudioPitch = 1.2f;

	public float maxSpeed = 30;

	public float velocityChangeSpeed = 10;

	public float stabilityForce = 2;
	public float stabilitySpeed = 2;

	public float forwardSpeed = 15;
	public float rightSpeed = 15;
	public float upSpeed = 15;

	public float rollRotationSpeed = 5;

	public float engineArmRotationAmountForward;
	public float engineArmRotationAmountRight;

	public float engineArmExtraRotationAmount;

	public float engineArmRotationSpeed;

	public float engineRotationSpeed = 10;

	public float groundRaycastDistance;
	public float hoverForce = 5;
	public float hoverFoeceOnAir = 2;
	public float rotateForce = 5;
	public float extraHoverSpeed = 2;

	public bool useHorizontalInputLerp = true;

	public float motorInput;
	public float heightInput;
	public float rollDirection;
	public bool groundFound;

	public Transform leftEngineTransform;
	public Transform rightEngineTransform;

	public Transform leftEngineArmTransform;
	public Transform rightEngineArmTransform;

	public vehicleCameraController vCamera;
	public Transform groundRaycastPosition;

	public inputActionManager actionManager;
	public vehicleHUDManager hudManager;
	public vehicleGravityControl vehicleGravityControlManager;
	public IKDrivingSystem IKDrivingManager;
	public Rigidbody mainRigidbody;

	public otherVehicleParts otherCarParts;
	public vehicletSettings settings;

	Vector3 normal;
	float audioPower = 0;
	float maxEnginePower;
	float boostInput = 1;
	float resetTimer;
	float horizontalAxis;
	float verticalAxis;
	float steerInput;
	float forwardInput;
	float currentSpeed;
	int i;
	int collisionForceLimit = 5;
	bool driving;

	bool moving;
	bool usingBoost;
	bool usingGravityControl;

	bool rotating;
	Transform vehicleCameraTransform;

	float leftInput = 0;
	float rightInput = 0;
	bool isTurnedOn;

	bool checkVehicleTurnedOn;

	Vector3 currenLeftEngineArmRotation;
	Vector3 currenRightEngineArmRotation;

	Vector3 currenLeftEngineRotation;
	Vector3 currenRightEngineRotation;

	Vector2 axisValues;
	Vector3 appliedHoverForce;
	RaycastHit hit;

	Vector2 rawAxisValues;

	bool touchPlatform;

	float steering;
	Vector3 localLook;

	bool usingRollInput;

	void Start ()
	{
		//get the boost particles inside the vehicle
		for (i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			otherCarParts.boostingParticles [i].gameObject.SetActive (false);
		}

		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		vehicleCameraTransform = vCamera.transform;

		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	void Update ()
	{
		//if the player is driving this vehicle and the gravity control is not being used, then
		if (driving && !usingGravityControl) {
			axisValues = actionManager.getPlayerMovementAxis ("keys");
			horizontalAxis = axisValues.x;

			if (!useHorizontalInputLerp && !touchPlatform) {
				rawAxisValues = actionManager.getPlayerRawMovementAxis ();

				if (!hudManager.usedByAI) {
					horizontalAxis = rawAxisValues.x;
				}
			}

			if (vCamera.currentState.useCameraSteer && !usingRollInput)
			{
				localLook = transform.InverseTransformDirection(vehicleCameraTransform.forward);

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;
				steering = Mathf.Clamp(steering, -1f, 1f);

				steering = -steering;

				rollDirection = steering;
			}

			if (isTurnedOn) {
				//get the current values from the input manager, keyboard and touch controls
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
					if (!vehicleGravityControlManager.isGravityPowerActive ()) {
						vCamera.usingBoost (false, "Boost");
					}
					usingBoosting ();
					boostInput = 1;
				}
			}

			//set the current speed in the HUD of the vehicle
			hudManager.getSpeed (currentSpeed, maxSpeed);

			IKDrivingManager.setNewAngularDirection (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis + Vector3.forward * heightInput + Vector3.up * rollDirection);
		}

		moving = verticalAxis != 0 || horizontalAxis != 0 || heightInput != 0 || rollDirection != 0;

		motorInput = verticalAxis;

		//if the vehicle has fuel, allow to move it
		if (moving) {
			if (!hudManager.useFuel ()) {
				motorInput = 0;
				if (isTurnedOn) {
					turnOnOrOff (false, isTurnedOn);
				}
			}
		}

		maxEnginePower = currentSpeed;

		audioPower = Mathf.Lerp (maxEnginePower, motorInput, audioEngineSpeed);
		otherCarParts.engineAudio.volume = Mathf.Lerp (engineMinVolume, engineMaxVolume, audioPower);
		otherCarParts.engineAudio.pitch = Mathf.Lerp (minAudioPitch, maxAudioPitch, audioPower);

		if (Mathf.Abs (horizontalAxis) > 0.05f) {
			steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * 10);
		} else {
			steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * 10);
		}
		if (Mathf.Abs (motorInput) > 0.05f) {
			forwardInput = Mathf.Lerp (forwardInput, motorInput, Time.deltaTime * 10);
		} else {
			forwardInput = Mathf.Lerp (forwardInput, motorInput, Time.deltaTime * 10);
		}

		float left = 0;
		float right = 0;
		if ((forwardInput > 0.05f || forwardInput < -0.05f) && (steerInput < 0.05f && steerInput > -0.05f)) {
			left = right = forwardInput;
			//print("moving forward or backward");
		} else if (forwardInput > 0.05f && steerInput > 0) {
			left = forwardInput;
			right = -steerInput;
			//print("moving forward and to the right");
		} else if (forwardInput > 0.05f && steerInput < 0) {
			left = steerInput;
			right = forwardInput;
			//print("moving forward and to the left");
		} else if ((forwardInput < 0.05f && forwardInput > -0.05f) && steerInput > 0) {
			left = 0;
			right = steerInput;
			//print("moving to the right");
		} else if ((forwardInput < 0.05f && forwardInput > -0.05f) && steerInput < 0) {
			left = -steerInput;
			right = 0;
			//print("moving to the left");
		} else if (forwardInput < -0.05f && steerInput > 0) {
			left = 0;
			right = -steerInput;
			//print("moving backward and to the right");
		} else if (forwardInput < -0.05f && steerInput < 0) {
			left = steerInput;
			right = 0;
			//print("moving backward and to the left");
		}

		leftInput = Mathf.Lerp (leftInput, left, Time.deltaTime * 10);
		rightInput = Mathf.Lerp (rightInput, right, Time.deltaTime * 10);
		Vector3 rightHandLebarEuler = otherCarParts.rightHandLebar.transform.localEulerAngles;
		Vector3 lefttHandLebarEuler = otherCarParts.leftHandLebar.transform.localEulerAngles;
		otherCarParts.rightHandLebar.transform.localRotation = Quaternion.Euler (settings.steerAngleLimit * rightInput * 2, rightHandLebarEuler.y, rightHandLebarEuler.z);
		otherCarParts.leftHandLebar.transform.localRotation = Quaternion.Euler (settings.steerAngleLimit * leftInput * 2, lefttHandLebarEuler.y, lefttHandLebarEuler.z);
		//reset the vehicle rotation if it is upside down 
		if (currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (normal, transform.up);
			if (angle > 60 && !rotating) {
				resetTimer += Time.deltaTime;
				if (resetTimer > timeToFlip) {
					resetTimer = 0;
					StartCoroutine (rotateVehicle ());
				}
			}
		}
	}

	void FixedUpdate ()
	{
		if (!usingGravityControl && isTurnedOn) {
			if (Physics.Raycast (groundRaycastPosition.position, -transform.up, out hit, groundRaycastDistance, settings.layer)) {
				float proportionalHeight = (groundRaycastDistance - hit.distance) / groundRaycastDistance;
				appliedHoverForce = vehicleCameraTransform.up * proportionalHeight * hoverForce + ((Mathf.Cos (Time.time * extraHoverSpeed)) / 2) * transform.up;
				vehicleGravityControlManager.setGravityForcePausedState (false);

				groundFound = true;
			} else {

				if (isTurnedOn) {
					vehicleGravityControlManager.setGravityForcePausedState (true);
				} else {
					vehicleGravityControlManager.setGravityForcePausedState (false);
				}

				groundFound = false;
			}

			if (!groundFound) {
				if (!moving) {
					appliedHoverForce = ((Mathf.Cos (Time.time * extraHoverSpeed)) / 2) * hoverFoeceOnAir * transform.up;
				} else {
					appliedHoverForce = Vector3.zero;
				}
			}

			if (groundFound && heightInput < 0) {
				heightInput = 0;
			}

			Vector3 newVelocity = transform.forward * motorInput * forwardSpeed + transform.right * horizontalAxis * rightSpeed + transform.up * heightInput * upSpeed;

			newVelocity += appliedHoverForce;

			newVelocity *= boostInput;

			mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, newVelocity, Time.deltaTime * velocityChangeSpeed);

			if (rollDirection != 0) {
				transform.Rotate (0, -rollDirection * rollRotationSpeed, 0);
			}

			Vector3 predictedUp = Quaternion.AngleAxis (mainRigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityForce / stabilitySpeed, mainRigidbody.angularVelocity) * transform.up;
			Vector3 torqueVector = Vector3.Cross (predictedUp, vehicleCameraTransform.up);
			mainRigidbody.AddTorque (torqueVector * stabilitySpeed * stabilitySpeed);

			currentSpeed = mainRigidbody.velocity.magnitude;

			currenLeftEngineArmRotation = heightInput * Vector3.forward * engineArmRotationAmountForward +
			motorInput * Vector3.right * engineArmRotationAmountRight -
			horizontalAxis * Vector3.forward * engineArmRotationAmountForward -
			rollDirection * Vector3.right * engineArmRotationAmountRight;

			currenLeftEngineArmRotation.x = 
				Mathf.Clamp (currenLeftEngineArmRotation.x, -engineArmRotationAmountRight - engineArmExtraRotationAmount, engineArmRotationAmountRight + engineArmExtraRotationAmount);
			currenLeftEngineArmRotation.z = 
				Mathf.Clamp (currenLeftEngineArmRotation.z, -engineArmRotationAmountForward - engineArmExtraRotationAmount, engineArmRotationAmountForward + engineArmExtraRotationAmount);

			leftEngineArmTransform.localRotation = Quaternion.Lerp (leftEngineArmTransform.localRotation, Quaternion.Euler (currenLeftEngineArmRotation), Time.deltaTime * engineArmRotationSpeed);


			currenRightEngineRotation = -heightInput * Vector3.forward * engineArmRotationAmountForward +
			motorInput * Vector3.right * engineArmRotationAmountRight -
			horizontalAxis * Vector3.forward * engineArmRotationAmountForward +
			rollDirection * Vector3.right * engineArmRotationAmountRight;

			currenRightEngineRotation.x = 
				Mathf.Clamp (currenRightEngineRotation.x, -engineArmRotationAmountRight - engineArmExtraRotationAmount, engineArmRotationAmountRight + engineArmExtraRotationAmount);
			currenRightEngineRotation.z = 
				Mathf.Clamp (currenRightEngineRotation.z, -engineArmRotationAmountForward - engineArmExtraRotationAmount, engineArmRotationAmountForward + engineArmExtraRotationAmount);

			rightEngineArmTransform.localRotation = Quaternion.Lerp (rightEngineArmTransform.localRotation, Quaternion.Euler (currenRightEngineRotation), Time.deltaTime * engineArmRotationSpeed);

			rightEngineTransform.Rotate (0, engineRotationSpeed * Time.deltaTime, 0);

			leftEngineTransform.Rotate (0, engineRotationSpeed * Time.deltaTime, 0);
		}

		if (isTurnedOn) {
			checkVehicleTurnedOn = true;

			//set the exhaust particles state
			for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
				if (isTurnedOn && currentSpeed < 20) {
					if (!otherCarParts.normalExhaust [i].isPlaying) {
						otherCarParts.normalExhaust [i].Play ();
					}
				} else {
					otherCarParts.normalExhaust [i].Stop ();
				}
			}

			for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
				if (isTurnedOn && currentSpeed > 10 && motorInput > 0.1f) {
					if (!otherCarParts.heavyExhaust [i].isPlaying) {
						otherCarParts.heavyExhaust [i].Play ();
					}
				} else {
					otherCarParts.heavyExhaust [i].Stop ();
				}
			}
		} else {
			if (checkVehicleTurnedOn) {
				stopVehicleParticles ();
				checkVehicleTurnedOn = false;
			}
		}
	}

	//if the vehicle is using the gravity control, set the state in this component
	public void changeGravityControlUse (bool state)
	{
		usingGravityControl = state;
	}

	//the player is getting on or off from the vehicle, so
	//public void changeVehicleState(Vector3 nextPlayerPos){
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
		vehicleGravityControlManager.changeGravityControlState (driving);

		vehicleGravityControlManager.setGravityForcePausedState (false);
	}

	public void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, true, false);
	}

	public void setTurnOffState (bool previouslyTurnedOn)
	{
		if (previouslyTurnedOn) {
			setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, true);
			setAudioState (otherCarParts.engineAudio, 5, 1, otherCarParts.engineEndClip, false, true, false);
		}

		motorInput = 0;
		steerInput = 0;
		boostInput = 1;
		horizontalAxis = 0;
		verticalAxis = 0;

		//stop the boost
		if (usingBoost) {
			usingBoost = false;
			vCamera.usingBoost (false, "Boost");
			usingBoosting ();
			boostInput = 1;
		}

		vehicleGravityControlManager.setGravityForcePausedState (false);
	}

	public void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		isTurnedOn = state;
		if (isTurnedOn) {
			setTurnOnState ();
		} else {
			setTurnOffState (previouslyTurnedOn);
		}

		vehicleGravityControlManager.setCheckDownSpeedActiveState (!isTurnedOn);
	}

	//the vehicle has been destroyed, so disabled every component in it
	public void disableVehicle ()
	{
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		setTurnOffState (false);

		//disable the exhausts particles
		stopVehicleParticles ();

		//disable the controller
		this.enabled = false;
	}

	public void stopVehicleParticles ()
	{
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}

		for (int i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			otherCarParts.boostingParticles [i].loop = false;
		}
	}

	//get the current normal in the gravity control component
	public void setNormal (Vector3 normalValue)
	{
		normal = normalValue;
	}

	//reset the vehicle rotation if it is upside down
	IEnumerator rotateVehicle ()
	{
		rotating = true;
		Quaternion currentRotation = transform.rotation;
		//rotate in the forward direction of the vehicle
		Quaternion dstRotPlayer = Quaternion.LookRotation (transform.forward, normal);
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			transform.rotation = Quaternion.Slerp (currentRotation, dstRotPlayer, t);
			mainRigidbody.velocity = Vector3.zero;
			yield return null;
		}
		rotating = false;
	}

	//play or stop every audio component in the vehicle, like engine, skid, etc.., configuring also volume and loop according to the movement of the vehicle
	public void setAudioState (AudioSource source, float distance, float volume, AudioClip clip, bool loop, bool play, bool stop)
	{
		source.minDistance = distance;
		source.volume = volume;
		source.clip = clip;
		source.loop = loop;
		source.spatialBlend = 1;
		if (play) {
			source.Play ();
		}
		if (stop) {
			source.Stop ();
		}
	}

	//if any collider in the vehicle collides, then
	void OnCollisionEnter (Collision collision)
	{
		//check that the collision is not with the player
		if (collision.contacts.Length > 0 && collision.gameObject.tag != "Player") {
			//if the velocity of the collision is higher that the limit
			if (collision.relativeVelocity.magnitude > collisionForceLimit) {
				//set the collision audio with a random collision clip
				if (otherCarParts.crashClips.Length > 0) {
					setAudioState (otherCarParts.crashAudio, 5, 1, otherCarParts.crashClips [UnityEngine.Random.Range (0, otherCarParts.crashClips.Length)], false, true, false);
				}
			}
		}
	}

	//if the vehicle is using the boost, set the boost particles
	public void usingBoosting ()
	{
		for (int i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			if (usingBoost) {
				if (!otherCarParts.boostingParticles [i].isPlaying) {
					otherCarParts.boostingParticles [i].gameObject.SetActive (true);
					otherCarParts.boostingParticles [i].Play ();
					otherCarParts.boostingParticles [i].loop = true;
				}
			} else {
				otherCarParts.boostingParticles [i].loop = false;
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//boost input
			if (holdingButton) {
				usingBoost = true;
				//set the camera move away action
				vCamera.usingBoost (true, "Boost");
			} else {
				//stop boost
				usingBoost = false;
				//disable the camera move away action
				vCamera.usingBoost (false, "Boost");
				//disable the boost particles
				usingBoosting ();
				boostInput = 1;
			}
		}
	}

	public void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl && hudManager.canSetTurnOnState) {
			if (hudManager.hasFuel ()) {
				turnOnOrOff (!isTurnedOn, isTurnedOn);
			}
		}
	}

	public void inputHorn ()
	{
		if (driving) {
			setAudioState (otherCarParts.hornAudio, 5, 1, otherCarParts.hornClip, false, true, false);
		}
	}

	public void inputIncreaseHeightState (bool state)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (state) {
				heightInput = 1;
			} else {
				heightInput = 0;
			}
		}
	}

	public void inputDecreaseHeightState (bool state)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (state) {
				heightInput = -1;
			} else {
				heightInput = 0;
			}
		}
	}

	public void inputSetRotateToLeftState (bool state)
	{
		if (driving && isTurnedOn) {
			if (state) {
				rollDirection = 1;
			} else {
				rollDirection = 0;
			}

			usingRollInput = state;
		}
	}

	public void inputSetRotateToRightState (bool state)
	{
		if (driving && isTurnedOn) {
			if (state) {
				rollDirection = -1;
			} else {
				rollDirection = 0;
			}

			usingRollInput = state;
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public Transform rightHandLebar;
		public Transform leftHandLebar;
		public Transform COM;
		public GameObject chassis;
		public AudioClip engineStartClip;
		public AudioClip engineClip;
		public AudioClip engineEndClip;
		public AudioClip[] crashClips;
		public AudioClip hornClip;
		public List<ParticleSystem> normalExhaust = new List<ParticleSystem> ();
		public List<ParticleSystem> heavyExhaust = new List<ParticleSystem> ();
		public AudioSource engineStartAudio;
		public AudioSource engineAudio;
		public AudioSource crashAudio;
		public AudioSource hornAudio;
		public List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();
	}

	[System.Serializable]
	public class vehicletSettings
	{
		public LayerMask layer;
		public float steerAngleLimit;
		public float maxBoostMultiplier;
		public bool canUseBoost;
		public float increaseHeightSpeed = 2;
		public float decreaseHeightSpeed = 2;
	}
}