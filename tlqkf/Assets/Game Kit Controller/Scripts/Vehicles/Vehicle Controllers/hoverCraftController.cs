using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hoverCraftController : MonoBehaviour
{
	public float maxSteeringTorque = 1400;
	public float rollOnTurns = 10;
	public float rollOnTurnsTorque = 20;

	public float steerBrakingTorque = 1400;

	public float brakingTorque;
	public float pitchCompensationTorque;
	public float rollCompensationTorque = 10;
	public float timeToFlip = 2;
	public float audioEngineSpeed;
	public float engineMinVolume = 0.5f;
	public float engineMaxVolume = 1;
	public float minAudioPitch = 0.5f;
	public float maxAudioPitch = 1.2f;
	public float maxForwardAcceleration = 20;
	public float maxReverseAcceleration = 15;
	public float maxBrakingDeceleration = 30;
	public float autoBrakingDeceleration;
	public float maxSpeed = 30;
	public AnimationCurve accelerationCurve;
	public float verticalReduction = 10;
	public float maxPitchAngle = 45;
	public Vector3 extraRigidbodyForce;

	public bool useHorizontalInputLerp = true;

	public float minSteerInputIdle = 0.4f;
	public float minSteerInputMoving = 0.4f;

	float currentMinSteerInput;

	public List<hoverEngineSettings> hoverEngineList = new List<hoverEngineSettings> ();
	public OtherCarParts otherCarParts;
	public hoverCraftSettings settings;

	public Rigidbody mainRigidbody;
	public IKDrivingSystem IKDrivingManager;
	public inputActionManager actionManager;
	public vehicleCameraController vCamera;
	public vehicleHUDManager hudManager;
	public vehicleGravityControl vehicleGravityControlManager;

	Vector3 normal;
	float audioPower = 0;
	float maxEnginePower;
	float boostInput = 1;
	float resetTimer;
	float motorInput;
	float horizontalAxis;
	float verticalAxis;
	float steerInput;
	float forwardInput;
	float currentSpeed;
	float originalJumpPower;
	int i;
	int collisionForceLimit = 5;
	bool driving;
	bool jump = false;
	bool moving;
	bool usingBoost;
	bool usingGravityControl;
	public bool anyOnGround;
	bool rotating;

	float leftInput = 0;
	float rightInput = 0;
	bool isTurnedOn;

	Vector2 axisValues;

	bool braking;

	Vector2 rawAxisValues;

	bool touchPlatform;

	float steering;
	Vector3 localLook;

	void Start ()
	{
		//get the boost particles inside the vehicle
		for (i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			otherCarParts.boostingParticles [i].gameObject.SetActive (false);
		}
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);
		originalJumpPower = settings.jumpPower;

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

			if (vCamera.currentState.useCameraSteer && horizontalAxis == 0)
			{
				localLook = transform.InverseTransformDirection(settings.vehicleCamera.transform.forward);

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;

				steering = Mathf.Clamp(steering, -1f, 1f);

				if (axisValues.y != 0) {
					currentMinSteerInput = minSteerInputMoving;
				} else {
					currentMinSteerInput = minSteerInputIdle;
				}

				if (Mathf.Abs (steering) > currentMinSteerInput) {
					horizontalAxis = steering;
				} else {
					horizontalAxis = 0;
				}
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

			IKDrivingManager.setNewAngularDirection (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
		}

		moving = verticalAxis != 0;

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

		maxEnginePower = 0;

		for (i = 0; i < hoverEngineList.Count; i++) {
			if (hoverEngineList [i].maxEnginePower > maxEnginePower) {
				maxEnginePower = hoverEngineList [i].maxEnginePower;
			}
			//configure every particle system according to the engine state
			float rpm = Mathf.Lerp (hoverEngineList [i].minRPM, hoverEngineList [i].maxRPM, hoverEngineList [i].maxEnginePower);
			hoverEngineList [i].turbine.transform.Rotate (0, rpm * Time.deltaTime * 6, 0);
			if (hoverEngineList [i].ParticleSystem) {
				hoverEngineList [i].ParticleSystem.emissionRate = hoverEngineList [i].maxEmission * hoverEngineList [i].maxEnginePower;
				hoverEngineList [i].ParticleSystem.transform.position = hoverEngineList [i].hit.point + hoverEngineList [i].hit.normal * hoverEngineList [i].dustHeight;
				hoverEngineList [i].ParticleSystem.transform.LookAt (hoverEngineList [i].hit.point + hoverEngineList [i].hit.normal * 10);
			}
		}

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
		currentSpeed = mainRigidbody.velocity.magnitude;

		//apply turn
		if (Mathf.Approximately (horizontalAxis, 0)) {
			float localR = Vector3.Dot (mainRigidbody.angularVelocity, transform.up);
			mainRigidbody.AddRelativeTorque (0, -localR * steerBrakingTorque, 0);
		} else {
			float targetRoll = -rollOnTurns * horizontalAxis;
			float roll = Mathf.Asin (transform.right.y) * Mathf.Rad2Deg;
			if (Mathf.Abs (roll) > Mathf.Abs (targetRoll)) {
				roll = 0;
			} else {
				roll = Mathf.DeltaAngle (roll, targetRoll); 
			}
			mainRigidbody.AddRelativeTorque (0, horizontalAxis * maxSteeringTorque, roll * rollOnTurnsTorque);
		}

		if (!usingGravityControl && !jump) {
			Vector3 localVelocity = transform.InverseTransformDirection (mainRigidbody.velocity);
			Vector3 extraForce = Vector3.Scale (localVelocity, extraRigidbodyForce);
			mainRigidbody.AddRelativeForce (-extraForce * mainRigidbody.mass);

			//use every engine to keep the vehicle in the air
			for (i = 0; i < hoverEngineList.Count; i++) {
				//if the current engine is the main engine
				if (!hoverEngineList [i].mainEngine) {
					//find force direction by rotating local up vector towards world up
					Vector3 engineUp = hoverEngineList [i].engineTransform.up;
					Vector3 gravityForce = (normal * 9.8f).normalized;
					engineUp = Vector3.RotateTowards (engineUp, gravityForce, hoverEngineList [i].maxEngineAngle * Mathf.Deg2Rad, 1);
					//check if the vehicle is on ground
					hoverEngineList [i].maxEnginePower = 0;
					if (Physics.Raycast (hoverEngineList [i].engineTransform.position, -engineUp, out hoverEngineList [i].hit, hoverEngineList [i].maxHeight, settings.layer)) {
						//calculate down force
						hoverEngineList [i].maxEnginePower = Mathf.Pow ((hoverEngineList [i].maxHeight - hoverEngineList [i].hit.distance) / hoverEngineList [i].maxHeight, hoverEngineList [i].Exponent);
						float force = hoverEngineList [i].maxEnginePower * hoverEngineList [i].engineForce;
						float velocityUp = Vector3.Dot (mainRigidbody.GetPointVelocity (hoverEngineList [i].engineTransform.position), engineUp);
						float drag = -velocityUp * Mathf.Abs (velocityUp) * hoverEngineList [i].damping;
						mainRigidbody.AddForceAtPosition (engineUp * (force + drag), hoverEngineList [i].engineTransform.position);
					}
				} else {
					//find current local pitch and roll
					Vector3 gravityForce = (normal * 9.8f).normalized;
					float pitch = Mathf.Asin (Vector3.Dot (transform.forward, gravityForce)) * Mathf.Rad2Deg;
					float roll = Mathf.Asin (Vector3.Dot (transform.right, gravityForce)) * Mathf.Rad2Deg;
					pitch = Mathf.DeltaAngle (pitch, 0); 
					roll = Mathf.DeltaAngle (roll, 0);
					//apply compensation torque
					float auxPitch = -pitch * pitchCompensationTorque;
					float auxRoll = roll * rollCompensationTorque;
					if (!anyOnGround) {
						auxPitch *= 0.5f;
						auxRoll *= 0.5f;
					}
					mainRigidbody.AddRelativeTorque (pitch, 0, auxRoll);
				}
			}

			if (braking) {
				float localR = Vector3.Dot (mainRigidbody.angularVelocity, transform.up);
				mainRigidbody.AddRelativeTorque (0, -localR * brakingTorque, 0);
				for (i = 0; i < hoverEngineList.Count; i++) {
					if (hoverEngineList [i].mainEngine) {
						mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero, Time.deltaTime);
					}
				}
			} else {
				for (i = 0; i < hoverEngineList.Count; i++) {
					if (hoverEngineList [i].mainEngine) {
						float movementMultiplier = settings.inAirMovementMultiplier;
						if (Physics.Raycast (hoverEngineList [i].engineTransform.position, -transform.up, out hoverEngineList [i].hit, hoverEngineList [i].maxHeight, settings.layer)) {
							movementMultiplier = 1;
						} 

						Vector3 gravityForce = (normal * 9.8f).normalized;
						//current speed along forward axis
						float speed = Vector3.Dot (mainRigidbody.velocity, transform.forward);
						//if the vehicle doesn't move by input, apply automatic brake 
						bool isAutoBraking = Mathf.Approximately (motorInput, 0) && autoBrakingDeceleration > 0;
						float thrust = motorInput;
						if (isAutoBraking) {
							thrust = -Mathf.Sign (speed) * autoBrakingDeceleration / maxBrakingDeceleration;
						}

						//check if it is braking, for example speed and thrust have opposing signs
						bool isBraking = motorInput * speed < 0;
						//don't apply force if speed is max already
						if (Mathf.Abs (speed) < maxSpeed || isBraking) {
							//position on speed curve
							float normSpeed = Mathf.Sign (motorInput) * speed / maxSpeed;
							//apply acceleration curve and select proper maximum value
							float acc = accelerationCurve.Evaluate (normSpeed) * (isBraking ? maxBrakingDeceleration : thrust > 0 ? maxForwardAcceleration : maxReverseAcceleration);
							//drag should be added to the acceleration
							float sdd = speed * extraRigidbodyForce.z;
							float dragForce = sdd + mainRigidbody.drag * speed;
							float force = acc * thrust + dragForce;
							//reduce acceleration if the vehicle is close to vertical orientation and is trrying to go higher
							float y = Vector3.Dot (transform.forward, gravityForce);

							if (maxPitchAngle < 90 && y * thrust > 0) {
								if (!isAutoBraking) {
									float pitch2 = Mathf.Asin (Mathf.Abs (y)) * Mathf.Rad2Deg;
									if (pitch2 > maxPitchAngle) {
										float reduction = (pitch2 - maxPitchAngle) / (90 - maxPitchAngle) * verticalReduction;
										force /= 1 + reduction;
									}
								}
							}
							mainRigidbody.AddForce (transform.forward * force * boostInput * movementMultiplier, ForceMode.Acceleration);
						}
					}
				}
			}
		}

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

		anyOnGround = true;
		int totalWheelsOnAir = 0;
		for (i = 0; i < hoverEngineList.Count; i++) {
			if (!Physics.Raycast (hoverEngineList [i].engineTransform.position, -hoverEngineList [i].engineTransform.up, out hoverEngineList [i].hit, 
				    hoverEngineList [i].maxHeight, settings.layer)) {
				totalWheelsOnAir++;
			}
		}

		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == hoverEngineList.Count && anyOnGround) {
			anyOnGround = false;
		}
	}

	IEnumerator jumpCoroutine ()
	{
		jump = true;
		yield return new WaitForSeconds (0.5f);
		jump = false;
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
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		setTurnOffState (false);

		//disable the exhausts particles
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}

		//disable the controller
		this.enabled = false;
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

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		StartCoroutine (jumpCoroutine ());
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
		if (driving && !usingGravityControl && isTurnedOn && settings.canJump && anyOnGround && !jump) {
			StartCoroutine (jumpCoroutine ());
			mainRigidbody.AddForce (normal * mainRigidbody.mass * settings.jumpPower, ForceMode.Impulse);
		}
	}

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

	public void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl) {
			braking = holdingButton;
		}
	}

	[System.Serializable]
	public class hoverEngineSettings
	{
		public string Name;
		public Transform engineTransform;
		public ParticleSystem ParticleSystem;
		public float maxEmission = 250;
		public float dustHeight = 0.1f;
		public float maxHeight = 2;
		public float engineForce = 4000;
		public float damping = 150;
		public float Exponent = 2;
		public float maxEngineAngle = 10;
		public bool mainEngine;
		public float minRPM = 100;
		public float maxRPM = 150;
		public Transform turbine;
		[HideInInspector] public RaycastHit hit;
		[HideInInspector] public float maxEnginePower;
	}

	[System.Serializable]
	public class OtherCarParts
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
	public class hoverCraftSettings
	{
		public LayerMask layer;
		public float steerAngleLimit;
		public float maxBoostMultiplier;
		[Range (0, 1)] public float inAirMovementMultiplier;
		public GameObject vehicleCamera;
		public float jumpPower;
		public bool canJump;
		public bool canUseBoost;
	}
}