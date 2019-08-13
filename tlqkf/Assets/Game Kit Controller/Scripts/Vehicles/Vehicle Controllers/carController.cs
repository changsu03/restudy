using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class carController : MonoBehaviour
{
	public List<Wheels> wheelsList = new List<Wheels> ();
	public List<Gears> gearsList = new List<Gears> ();
	public OtherCarParts otherCarParts;
	public carSettings settings;

	public bool rotateVehicleUpward = true;

	public bool useHorizontalInputLerp = true;

	public int currentGear;
	public float currentSpeed;
	public float currentRearRPM = 0;
	public float currentRPM = 0;
	public bool anyOnGround;

	public IKDrivingSystem IKDrivingManager;
	public Rigidbody mainRigidbody;
	public vehicleCameraController vCamera;
	public vehicleHUDManager hudManager;
	public skidsManager skidMarksManager;
	public inputActionManager actionManager;
	public vehicleGravityControl vehicleGravityControlManager;
	public shipInterfaceInfo interfaceInfo;

	List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();

	public bool driving;
	public bool isTurnedOn;

	bool reversing;
	bool changingGear;
	bool jump;
	bool colliding;
	bool moving;
	bool usingBoost;
	bool vehicleDestroyed;
	bool usingGravityControl;
	bool rotating;
	bool braking;
	int i, j;
	int collisionForceLimit = 5;
	float horizontalLean = 0;
	float verticalLean = 0;
	float resetTimer = 0;
	float motorInput = 0;
	float steerInput = 0;
	float boostInput = 1;
	float acceleration;
	float lastVelocity = 0;
	float defSteerAngle;
	float driftAngle;
	float timeToStabilize = 0;
	float horizontalAxis;
	float verticalAxis;
	float originalJumpPower;
	RaycastHit hit;
	Vector3 normal;

	Vector2 axisValues;

	public Vector2 rawAxisValues;

	bool usingImpulse;
	float lastTimeImpulseUsed;
	float lastTimeJump;

	bool bouncingVehicle;
	Coroutine bounceCoroutine;

	bool touchPlatform;

	float steering;
	Vector3 localLook;

	void Start ()
	{
		//get every wheel component, like the mudguard, suspension and the slip smoke particles
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].mudGuard) {
				wheelsList [i].mudGuardOriginalRotation = transform.localRotation;
				wheelsList [i].mudGuardOffset = wheelsList [i].mudGuard.transform.localPosition - wheelsList [i].wheelMesh.transform.localPosition;
			}

			if (wheelsList [i].suspension) {
				wheelsList [i].suspensionOffset = wheelsList [i].suspension.transform.localPosition - wheelsList [i].wheelMesh.transform.localPosition;
			}
		}

		//set the sound components
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		//get the vehicle rigidbody
		mainRigidbody.maxAngularVelocity = 5;

		//store the max sterr angle
		defSteerAngle = settings.steerAngleLimit;

		//get the boost particles inside the vehicle
		if (otherCarParts.boostParticles) {
			Component[] boostParticlesComponents = otherCarParts.boostParticles.GetComponentsInChildren (typeof(ParticleSystem));
			foreach (Component c in boostParticlesComponents) {
				boostingParticles.Add (c.GetComponent<ParticleSystem> ());
				c.gameObject.SetActive (false);
			}
		}
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

			if (vCamera.currentState.useCameraSteer && horizontalAxis == 0) {
				localLook = transform.InverseTransformDirection (settings.vehicleCamera.transform.forward);

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;
				steering = Mathf.Clamp (steering, -1f, 1f);

				if (axisValues.y < 0) {
					steering *= (-1);
				}

				horizontalAxis = steering;
			}

			if (isTurnedOn) {
				
				//get the current values from the input manager, keyboard and touch controls
				verticalAxis = axisValues.y;
			
				if (settings.canImpulseHoldingJump) {
					if (usingImpulse) {
						if (settings.impulseUseEnergy) {
							if (Time.time > lastTimeImpulseUsed + settings.impulseUseEnergyRate) {
								hudManager.removeEnergy (settings.impulseUseEnergyAmount);
								lastTimeImpulseUsed = Time.time;
							}
						}
						mainRigidbody.AddForce (transform.up * mainRigidbody.mass * settings.impulseForce);
					}
				}
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
			hudManager.getSpeed (currentSpeed, settings.maxForwardSpeed);

			IKDrivingManager.setNewAngularDirection (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
		} else {
			//else, set the input values to 0
			horizontalAxis = 0;
			verticalAxis = 0;
		}

		if (hudManager.usedByAI) {
			horizontalAxis = vehicleAI.steering;
			verticalAxis = vehicleAI.accel;
			//print (verticalAxis);
			//braking = vehicleAI.footbrake == 1 ? true : false;
		}
			
		//set the current axis input in the motor input
		currentRearRPM = 0;
		if (!changingGear) {
			motorInput = verticalAxis;
		} else {
			motorInput = Mathf.Clamp (verticalAxis, -1, 0);
		}
		moving = verticalAxis != 0;

		//if the vehicle has fuel, allow to move it
		if (moving) {
			if (!hudManager.useFuel ()) {
				motorInput = 0;
				if (isTurnedOn) {
					turnOnOrOff (false, isTurnedOn);
				}
			}
		}

		//steering input
		steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * settings.steerInputSpeed);

		//set the steer angle in every steerable wheel and get the currentRearRPM from the wheel that power the vehicle
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].steerable) {
				if (wheelsList [i].reverseSteer) {
					wheelsList [i].wheelCollider.steerAngle = (settings.steerAngleLimit * -steerInput);
				} else {
					wheelsList [i].wheelCollider.steerAngle = (settings.steerAngleLimit * steerInput);
				}
			}

			if (wheelsList [i].powered) {
				currentRearRPM += wheelsList [i].wheelCollider.rpm;
			}
		}

		//change gears
		if (!changingGear && !usingGravityControl) {
			if (currentGear + 1 < gearsList.Count) {
				if (currentSpeed >= gearsList [currentGear].gearSpeed && currentRearRPM > 0) {
					//print ("mas"+gearsList [currentGear+1].Name + " " + gearsList [currentGear].gearSpeed);
					StartCoroutine (changeGear (currentGear + 1));
				}
			}
			if (currentGear - 1 >= 0) {
				if (currentSpeed < gearsList [currentGear - 1].gearSpeed) {
					//print ("menos"+gearsList [currentGear-1].Name + " " + gearsList [currentGear-1].gearSpeed);
					StartCoroutine (changeGear (currentGear - 1));
				}
			}
		}

		//reset the vehicle rotation if it is upside down 
		if (vehicleGravityControlManager.useGravity && currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (normal, transform.up);
			//&& !colliding
			if (angle > 60 && !rotating) {
				resetTimer += Time.deltaTime;
				if (resetTimer > 1.5f) {
					resetTimer = 0;

					if (rotateVehicleUpward) {
						rotateVehicle ();
					}
				}
			}
			//set the current gear to 0
			if (currentGear > 0) {
				StartCoroutine (changeGear (0));
			}
		}

		//check every wheel collider of the vehicle, to move it and apply rotation to it correctly using raycast
		WheelHit wheelGroundHit = new WheelHit ();
		for (i = 0; i < wheelsList.Count; i++) {
			//get the center position of the wheel
			Vector3 whellCenterPosition = wheelsList [i].wheelCollider.transform.TransformPoint (wheelsList [i].wheelCollider.center);

			//use a raycast in the ground direction
			wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHit);

			//if the wheel is close enough to the ground, then
			if (Physics.Raycast (whellCenterPosition, -wheelsList [i].wheelCollider.transform.up, out hit, 
				    (wheelsList [i].wheelCollider.suspensionDistance + wheelsList [i].wheelCollider.radius) * transform.localScale.y, settings.layer)) {
				//set the wheel mesh position according to the values of the wheel collider
				wheelsList [i].wheelMesh.transform.position = hit.point + (wheelsList [i].wheelCollider.transform.up * wheelsList [i].wheelCollider.radius) * transform.localScale.y;
				//set the suspension spring position of the wheel collider
				wheelsList [i].suspensionSpringPos = -(hit.distance - wheelsList [i].wheelCollider.radius);
			}

			//the wheel is in the air
			else {
				//set the suspension spring position of the wheel collider
				wheelsList [i].suspensionSpringPos = -(wheelsList [i].wheelCollider.suspensionDistance);
				//set the wheel mesh position according to the values of the wheel collider
				wheelsList [i].wheelMesh.transform.position = whellCenterPosition -
				(wheelsList [i].wheelCollider.transform.up * wheelsList [i].wheelCollider.suspensionDistance) * transform.localScale.y;
			}

			//set the rotation value in the wheel collider
			wheelsList [i].rotationValue += wheelsList [i].wheelCollider.rpm * (6) * Time.deltaTime;

			//if the wheel powers the vehicle
			if (wheelsList [i].powered) {
				//rotate the wheel mesh only according to the current speed 
				wheelsList [i].wheelMesh.transform.rotation = wheelsList [i].wheelCollider.transform.rotation *
				Quaternion.Euler (wheelsList [i].rotationValue, 0, wheelsList [i].wheelCollider.transform.rotation.z);
			}

			//if the wheel is used to change the vehicle direction
			if (wheelsList [i].steerable) {
				//rotate the wheel mesh according to the current speed and rotate in the local Y axis according to the rotation of the steering wheel
				wheelsList [i].wheelMesh.transform.rotation = wheelsList [i].wheelCollider.transform.rotation * Quaternion.Euler (wheelsList [i].rotationValue, 
					wheelsList [i].wheelCollider.steerAngle + (driftAngle / settings.steeringAssistanceDivider), wheelsList [i].wheelCollider.transform.rotation.z);
			}

			//if the wheel has a mudguard
			if (wheelsList [i].mudGuard) {
				if (wheelsList [i].steerable) {
					//if the wheel is steerable, rotate the mudguard according to that rotation
					wheelsList [i].mudGuard.transform.localRotation = wheelsList [i].mudGuardOriginalRotation * Quaternion.Euler (0, settings.steerAngleLimit * steerInput, 0);
				}
				//set its position according to the wheel position
				wheelsList [i].mudGuard.transform.localPosition = wheelsList [i].wheelMesh.transform.localPosition + wheelsList [i].mudGuardOffset;
			}

			//if the wheel has suspension, set its poition just like the mudguard
			if (wheelsList [i].suspension) {
				wheelsList [i].suspension.transform.localPosition = wheelsList [i].suspensionOffset + wheelsList [i].wheelMesh.transform.localPosition;	
			}

			//calculate the drift angle, using the right side of the vehicle
			if (wheelsList [i].powered && wheelsList [i].rightSide) {
				wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHit);
				driftAngle = Mathf.Lerp (driftAngle, (Mathf.Clamp (wheelGroundHit.sidewaysSlip, settings.driftAngleLimit.x, settings.driftAngleLimit.y)), Time.deltaTime * 2);
			}

			//rotate the wheel collider in its  forward local axis
			float handling = Mathf.Lerp (-1, 1, wheelGroundHit.force / settings.steerWheelRotationPercentage);
			int mult = 1;
			if (wheelsList [i].leftSide) {
				mult = -1;
			}
			wheelsList [i].wheelCollider.transform.localEulerAngles = 
				new Vector3 (wheelsList [i].wheelCollider.transform.localEulerAngles.x, wheelsList [i].wheelCollider.transform.localEulerAngles.y, (handling * mult));
		}

		//if the vehicle has a steering Wheel, rotate it according to the steer input
		if (otherCarParts.SteeringWheel) {
			otherCarParts.SteeringWheel.transform.localRotation = 
				Quaternion.Euler (otherCarParts.SteeringWheel.transform.localEulerAngles.x, otherCarParts.SteeringWheel.transform.localEulerAngles.y,
				-settings.steerAngleLimit * steerInput + (driftAngle / settings.steeringAssistanceDivider) * 2);
		}

		//check the right front and right rear wheel to play the skid audio according to their state
		WheelHit wheelGroundHitFront = new WheelHit ();
		WheelHit wheelGroundHitRear = new WheelHit ();
		for (i = 0; i < wheelsList.Count; i++) {
			//use a wheel hit to that
			if (wheelsList [i].powered && wheelsList [i].rightSide) {
				wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHitFront);
			}

			if (wheelsList [i].steerable && wheelsList [i].rightSide) {
				wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHitRear);
			}
		}

		//if the values in the wheel hit are higher that
		if (Mathf.Abs (wheelGroundHitFront.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHitRear.forwardSlip) > 0.5f || Mathf.Abs (wheelGroundHitFront.forwardSlip) > 0.5f) {
			//and the vehicle is moving, then 
			if (mainRigidbody.velocity.magnitude > 1) {
				//set the skid volume value according to the vehicle skid
				otherCarParts.skidAudio.volume = Mathf.Abs (wheelGroundHitFront.sidewaysSlip) + ((Mathf.Abs (wheelGroundHitFront.forwardSlip) +
				Mathf.Abs (wheelGroundHitRear.forwardSlip)) / 4f);
			} else {
				//set the skid volume value to 0
				otherCarParts.skidAudio.volume -= Time.deltaTime;
			}
		} else {
			//set the skid volume value to 0
			otherCarParts.skidAudio.volume -= Time.deltaTime;
		}

		//rotate the vehicle chassis when the gear is being changed
		//get the vertical lean value
		verticalLean = Mathf.Clamp (Mathf.Lerp (verticalLean, mainRigidbody.angularVelocity.x * settings.chassisLean.y, Time.deltaTime * 3), 
			-settings.chassisLeanLimit, settings.chassisLeanLimit);
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].powered && wheelsList [i].rightSide) {
				wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHit);
			}
		}

		float normalizedLeanAngle = Mathf.Clamp (wheelGroundHit.sidewaysSlip, -1, 1);
		if (normalizedLeanAngle > 0) {
			normalizedLeanAngle = 1;
		} else {
			normalizedLeanAngle = -1;
		}

		if (!bouncingVehicle) {
			//if (Time.timeScale == 1) {
			//get the horizontal lean value
			horizontalLean = Mathf.Clamp (Mathf.Lerp (horizontalLean, 
				(Mathf.Abs (transform.InverseTransformDirection (mainRigidbody.angularVelocity).y) * -normalizedLeanAngle) * settings.chassisLean.x, Time.deltaTime * 3), 
				-settings.chassisLeanLimit, settings.chassisLeanLimit);
			Quaternion chassisRotation = Quaternion.Euler (verticalLean, otherCarParts.chassis.transform.localRotation.y + (mainRigidbody.angularVelocity.z), horizontalLean);
			//set the lean rotation value in the chassis transform
			otherCarParts.chassis.transform.localRotation = chassisRotation;


			//}
			//set the vehicle mass center 
			mainRigidbody.centerOfMass = new Vector3 ((otherCarParts.COM.localPosition.x) * transform.localScale.x, 
				(otherCarParts.COM.localPosition.y) * transform.localScale.y, (otherCarParts.COM.localPosition.z) * transform.localScale.z);
		}
	}

	public void setEngineOnOrOffState ()
	{
		if (hudManager.hasFuel ()) {
			turnOnOrOff (!isTurnedOn, isTurnedOn);
		}
	}

	public void pressHorn ()
	{
		setAudioState (otherCarParts.hornAudio, 5, 1, otherCarParts.hornClip, false, true, false);
		hudManager.activateHorn ();
	}

	public void passengerGettingOnOff ()
	{
		vehicleBounceMovement (settings.horizontalLeanPassengerAmount, settings.verticalLeanPassengerAmount);
	}

	public void vehicleBounceMovement (float horizontalLeanAmount, float verticalLeanAmount)
	{
		if (bounceCoroutine != null) {
			StopCoroutine (bounceCoroutine);
		}
		bounceCoroutine = StartCoroutine (vehicleBounceMovementCoroutine (horizontalLeanAmount, verticalLeanAmount));
	}

	IEnumerator vehicleBounceMovementCoroutine (float horizontalLeanAmount, float verticalLeanAmount)
	{
		bouncingVehicle = true;
		horizontalLean = Mathf.Clamp (horizontalLeanAmount, -settings.chassisLeanLimit, settings.chassisLeanLimit);
		verticalLean = Mathf.Clamp (verticalLeanAmount, -settings.chassisLeanLimit, settings.chassisLeanLimit);
		Quaternion chassisRotation = Quaternion.Euler (otherCarParts.chassis.transform.localRotation.x + verticalLean, 
			                             otherCarParts.chassis.transform.localRotation.y, 
			                             otherCarParts.chassis.transform.localRotation.z + horizontalLean);
		Vector3 targetRotation = chassisRotation.eulerAngles;
		Vector3 originalEuler = otherCarParts.chassis.transform.eulerAngles;
		float t = 0;
		while (t < 1 && otherCarParts.chassis.transform.localEulerAngles != targetRotation) {
			t += Time.deltaTime * settings.leanPassengerSpeed;
			otherCarParts.chassis.transform.localRotation = Quaternion.Slerp (otherCarParts.chassis.transform.localRotation, Quaternion.Euler (targetRotation), t);	
		}

		bouncingVehicle = false;
		yield return null;	
	}

	void FixedUpdate ()
	{
		//allows vehicle to remain roughly pointing in the direction of travel
		//if the vehicle is not on the ground, not colliding, rotating and its speed is higher that 5
		if (!anyOnGround && settings.preserveDirectionWhileInAir && !colliding && !rotating && currentSpeed > 5) {
			//check the time to stabilize
			if (timeToStabilize < 0.6f) {
				timeToStabilize += Time.deltaTime;
			} else {
				//rotate every axis of the vehicle in the rigidbody velocity direction
				mainRigidbody.freezeRotation = true;
				float angleX = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).x) * Mathf.Rad2Deg;
				float angleZ = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).z) * Mathf.Rad2Deg;
				float angleY = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (mainRigidbody.velocity.normalized, transform.forward)).y) * Mathf.Rad2Deg;
				transform.Rotate (new Vector3 (angleX, angleY, angleZ) * Time.deltaTime * (-1));
			}
		}

		//if any of the vehicle is on the groud, free the rigidbody rotation
		if (anyOnGround) {
			mainRigidbody.freezeRotation = false;
			timeToStabilize = 0;
		}

		//if the handbrake is pressed, set the brake torque value in every wheel
		if (braking) {
			for (i = 0; i < wheelsList.Count; i++) {
				if (wheelsList [i].powered) {
					wheelsList [i].wheelCollider.brakeTorque = settings.brake * 15;
				}
				if (wheelsList [i].steerable) {
					wheelsList [i].wheelCollider.brakeTorque = settings.brake / 10;				
				}
			}
		} else {
			//else, check if the vehicle input is in forward or in backward direction
			for (i = 0; i < wheelsList.Count; i++) {
				//the vehicle is decelerating
				if (Mathf.Abs (motorInput) <= 0.05f && !changingGear) {
					wheelsList [i].wheelCollider.brakeTorque = settings.brake / 25;
				} 
				//the vehicle is braking
				else if (motorInput < 0 && !reversing) {
					if (wheelsList [i].powered) {
						wheelsList [i].wheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput / 2));
					}
					if (wheelsList [i].steerable) {
						wheelsList [i].wheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput));
					}
				} else {
					wheelsList [i].wheelCollider.brakeTorque = 0;
				}
			}
		}

		//adhere the vehicle to the ground
		//check the front part
		WheelHit FrontWheelHit;
		float travel = 1;
		float totalTravel = 0;
		float antiRollForceFront;
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].steerable) {
				//if the wheel is in the ground
				bool grounded = wheelsList [i].wheelCollider.GetGroundHit (out FrontWheelHit);
				if (grounded) {
					//get the value to the ground according to the wheel collider configuration
					travel = (-wheelsList [i].wheelCollider.transform.InverseTransformPoint (FrontWheelHit.point).y - wheelsList [i].wheelCollider.radius) /
					wheelsList [i].wheelCollider.suspensionDistance;
				}
				//if the wheel is the front wheel
				if (wheelsList [i].leftSide) {
					//add to the total travel
					totalTravel += travel;
				}
				//else
				if (wheelsList [i].rightSide) {
					//substract from the total travel
					totalTravel -= travel;
				}
			}
			travel = 1;
		}

		//now, with the force multiplier which has to be applied in the front wheels
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].steerable) {
				int mult = 1;
				if (wheelsList [i].leftSide) {
					mult = -1;
				}
				//get the total amount of force applied to every wheel
				antiRollForceFront = totalTravel * settings.antiRoll;
				//if the wheel is on the ground, apply the force
				bool grounded = wheelsList [i].wheelCollider.GetGroundHit (out FrontWheelHit);
				if (grounded) {
					mainRigidbody.AddForceAtPosition (wheelsList [i].wheelCollider.transform.up * mult * antiRollForceFront, wheelsList [i].wheelCollider.transform.position);  
				}
			}
		}

		//like the above code, but this time in the rear wheels
		WheelHit RearWheelHit;
		bool groundRear = true;
		travel = 1;
		totalTravel = 0;
		float antiRollForceRear = 0;
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].powered) {
				bool grounded = wheelsList [i].wheelCollider.GetGroundHit (out RearWheelHit);
				if (grounded) {
					travel = (-wheelsList [i].wheelCollider.transform.InverseTransformPoint (RearWheelHit.point).y - wheelsList [i].wheelCollider.radius) /
					wheelsList [i].wheelCollider.suspensionDistance;
				}
				if (wheelsList [i].leftSide) {
					totalTravel += travel;
				}
				if (wheelsList [i].rightSide) {
					totalTravel -= travel;
				}
			}
			travel = 1;
		}

		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].powered) {
				int mult = 1;
				if (wheelsList [i].leftSide) {
					mult = -1;
				}
				antiRollForceRear = totalTravel * settings.antiRoll;
				bool grounded = wheelsList [i].wheelCollider.GetGroundHit (out RearWheelHit);
				if (grounded) {
					mainRigidbody.AddForceAtPosition (wheelsList [i].wheelCollider.transform.up * mult * antiRollForceRear, wheelsList [i].wheelCollider.transform.position);
				} 
				//if both rear wheels are not in the ground, then 
				else {
					groundRear = false;
				}
			}
		}

		//if both rear wheels are in the ground, then 
		if (groundRear) {
			//add an extra force to the main rigidbody of the vehicle
			mainRigidbody.AddRelativeTorque ((Vector3.up * (steerInput)) * 5000);
			//check if the jump input has been presses
			if (jump) {
				//apply force in the up direction
				mainRigidbody.AddForce (transform.up * mainRigidbody.mass * settings.jumpPower);
				jump = false;
				lastTimeJump = Time.time;
			}
		}

		//get the current speed value
		currentSpeed = mainRigidbody.velocity.magnitude * 3;
		//calculate the current acceleration
		acceleration = 0;
		acceleration = (transform.InverseTransformDirection (mainRigidbody.velocity).z - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = transform.InverseTransformDirection (mainRigidbody.velocity).z;
		//set the drag according to vehicle acceleration
		mainRigidbody.drag = Mathf.Clamp ((acceleration / 50), 0, 1);
		//set the steer limit
		settings.steerAngleLimit = Mathf.Lerp (defSteerAngle, settings.highSpeedSteerAngle, (currentSpeed / settings.highSpeedSteerAngleAtSpeed));
		//set the current RPM
		currentRPM = Mathf.Clamp ((((Mathf.Abs (currentRearRPM) * settings.gearShiftRate) + settings.minRPM)) / (currentGear + 1), settings.minRPM, settings.maxRPM);
		//check if the vehicle is moving forwards or backwards
		if (motorInput <= 0 && currentRearRPM < 20) {
			reversing = true;
		} else {
			reversing = false;
		}

		//set the engine audio volume and pitch according to input and current RPM
		if (otherCarParts.engineAudio && !vehicleDestroyed) {
			if (!reversing) {
				otherCarParts.engineAudio.volume = Mathf.Lerp (otherCarParts.engineAudio.volume, Mathf.Clamp (motorInput, 0.35f, 0.85f), Time.deltaTime * 5);
			} else {
				otherCarParts.engineAudio.volume = Mathf.Lerp (otherCarParts.engineAudio.volume, Mathf.Clamp (Mathf.Abs (motorInput), 0.35f, 0.85f), Time.deltaTime * 5);
			}
			otherCarParts.engineAudio.pitch = Mathf.Lerp (otherCarParts.engineAudio.pitch, 
				Mathf.Lerp (1, 2, (currentRPM - settings.minRPM / 1.5f) / (settings.maxRPM + settings.minRPM)), Time.deltaTime * 5);
		}

		//if the current speed is higher that the max speed, stop apply motor torque to the powered wheels	
		if (currentSpeed > settings.maxForwardSpeed || Mathf.Abs (currentRearRPM / 2) > 3000 || usingGravityControl) {
			for (i = 0; i < wheelsList.Count; i++) {
				if (wheelsList [i].powered) {
					wheelsList [i].wheelCollider.motorTorque = 0;
				}
			}
		} else if (!reversing) {
			//else if the vehicle is moving in fowards direction, apply motor torque to every powered wheel using the gear animation curve
			for (i = 0; i < wheelsList.Count; i++) {
				if (wheelsList [i].powered) {
					float speedMultiplier = 1;
					if (settings.useCurves) {
						speedMultiplier = gearsList [currentGear].engineTorqueCurve.Evaluate (currentSpeed);
					}
					wheelsList [i].wheelCollider.motorTorque = settings.engineTorque * Mathf.Clamp (motorInput, 0, 1) * boostInput * speedMultiplier;
				}
			}
		}

		//if the vehicle is moving backwards, apply motor torque to every powered wheel
		if (reversing) {
			//if the current speed is lower than the maxBackWardSpeed, apply motor torque
			if (currentSpeed < settings.maxBackWardSpeed && Mathf.Abs (currentRearRPM / 2) < 3000) {
				for (i = 0; i < wheelsList.Count; i++) {
					if (wheelsList [i].powered) {
						wheelsList [i].wheelCollider.motorTorque = settings.engineTorque * motorInput;
					}
				}
			} 
			//else, stop adding motor torque
			else {
				for (i = 0; i < wheelsList.Count; i++) {
					if (wheelsList [i].powered) {
						wheelsList [i].wheelCollider.motorTorque = 0;
					}
				}
			}
		}

		//if the vehicle is not being driving
		if (!driving) {
			//stop the motor torque and apply brake torque to every wheel
			for (i = 0; i < wheelsList.Count; i++) {
				wheelsList [i].wheelCollider.motorTorque = 0;
				wheelsList [i].wheelCollider.brakeTorque = settings.brake / 10;
			}
		}

		//set the smoke skid particles in every wheel
		WheelHit wheelGroundHit = new WheelHit ();
		for (i = 0; i < wheelsList.Count; i++) {
			wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHit);

			//set the skid marks under every wheel
			wheelsList [i].wheelSlipAmountSideways = Mathf.Abs (wheelGroundHit.sidewaysSlip);
			wheelsList [i].wheelSlipAmountForward = Mathf.Abs (wheelGroundHit.forwardSlip);
			if (wheelsList [i].wheelSlipAmountSideways > 0.25f || wheelsList [i].wheelSlipAmountForward > 0.5f) {
				Vector3 skidPoint = wheelGroundHit.point + 2 * (mainRigidbody.velocity) * Time.deltaTime;
				if (mainRigidbody.velocity.magnitude > 1) {
					wheelsList [i].lastSkidmark = skidMarksManager.AddSkidMark (skidPoint, wheelGroundHit.normal, 
						(wheelsList [i].wheelSlipAmountSideways / 2) + (wheelsList [i].wheelSlipAmountForward / 2.5f), wheelsList [i].lastSkidmark);
				} else {
					wheelsList [i].lastSkidmark = -1;
				}
			} else {
				wheelsList [i].lastSkidmark = -1;
			}

			if (wheelsList [i].wheelParticleSystem) {
				wheelsList [i].wheelCollider.GetGroundHit (out wheelGroundHit);
				if (Mathf.Abs (wheelGroundHit.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHit.forwardSlip) > 0.5f) {
					wheelsList [i].wheelParticleSystem.Play ();
				} else { 
					wheelsList [i].wheelParticleSystem.Stop ();
				}
			}
		}

		//set the exhaust particles state
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			if (isTurnedOn) {
				if (currentSpeed < 10) {
					if (!otherCarParts.normalExhaust [i].isPlaying) {
						otherCarParts.normalExhaust [i].Play ();
					}
				} else {
					otherCarParts.normalExhaust [i].Stop ();
				}
			} else {
				otherCarParts.normalExhaust [i].Stop ();
			}
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			if (isTurnedOn) {
				if (currentSpeed > 10 && motorInput > 0.1f) {
					if (!otherCarParts.heavyExhaust [i].isPlaying) {
						otherCarParts.heavyExhaust [i].Play ();
					}
				} else {
					otherCarParts.heavyExhaust [i].Stop ();
				}
			} else {
				otherCarParts.heavyExhaust [i].Stop ();
			}
		}

		//check if the car is in the ground or not
		anyOnGround = true;
		int totalWheelsOnAir = 0;
		for (i = 0; i < wheelsList.Count; i++) {
			if (!wheelsList [i].wheelCollider.isGrounded) {
				//if the current wheel is in the air, increase the number of wheels in the air
				totalWheelsOnAir++;
			}
		}
		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == wheelsList.Count && anyOnGround) {
			anyOnGround = false;
		}

		if (interfaceInfo) {
			interfaceInfo.shipEnginesState (isTurnedOn);
		}

	}

	//if the vehicle is using the gravity control, set the state in this component
	public void changeGravityControlUse (bool state)
	{
		usingGravityControl = state;
		if (usingGravityControl) {
			StartCoroutine (changeGear (0));
		}
		usingImpulse = false;
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
		vehicleGravityControlManager.changeGravityControlState (driving);
	
		if (interfaceInfo) {
			interfaceInfo.enableOrDisableInterface (driving);
		}
	}

	public void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, true, true, false);
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
		}
		usingImpulse = false;
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
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, false, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);
		vehicleDestroyed = true;

		setTurnOffState (false);

		//disable the skid particles
		for (i = 0; i < wheelsList.Count; i++) {
			if (wheelsList [i].wheelParticleSystem) {
				wheelsList [i].wheelParticleSystem.Stop ();
			}
		}

		//disable the exhausts particles
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}

		//disable the controller
		this.enabled = false;

		if (interfaceInfo) {
			interfaceInfo.enableOrDisableInterface (false);
		}
	}

	//get the current normal in the gravity control component
	public void setNormal (Vector3 normalValue)
	{
		normal = normalValue;
	}

	Coroutine rotatingVehicleCoroutine;

	public void rotateVehicle ()
	{
		if (rotatingVehicleCoroutine != null) {
			StopCoroutine (rotatingVehicleCoroutine);
		}
		rotatingVehicleCoroutine = StartCoroutine (rotateVehicleCoroutine ());
	}

	//reset the vehicle rotation if it is upside down
	IEnumerator rotateVehicleCoroutine ()
	{
		rotating = true;
		timeToStabilize = 0;
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

	//change the gear in the vehicle
	IEnumerator changeGear (int gear)
	{
		changingGear = true;
		setAudioState (otherCarParts.gearShiftingSound, 5, 0.3f, gearsList [gear].gearShiftingClip, false, true, false);
		yield return new WaitForSeconds (0.5f);
		changingGear = false;
		currentGear = gear;
		currentGear = Mathf.Clamp (currentGear, 0, gearsList.Count - 1);
	}

	//play or stop every audio component in the vehicle, like engine, skid, etc.., configuring also volume and loop according to the movement of the vehicle
	public void setAudioState (AudioSource source, float distance, float volume, AudioClip audioClip, bool loop, bool play, bool stop)
	{
		source.minDistance = distance;
		source.volume = volume;
		source.clip = audioClip;
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
		//reset the collision values
		mainRigidbody.freezeRotation = false;
		colliding = true;
		timeToStabilize = 0;
	}

	//if the vehicle is colliding, then
	void OnCollisionStay (Collision collision)
	{
		//set the values to avoid stabilize the vehicle yet
		mainRigidbody.freezeRotation = false;
		colliding = true;
		timeToStabilize = 0;
	}

	//the vehicle is not colliding
	void OnCollisionExit (Collision collision)
	{
		colliding = false;
	}

	//if the vehicle is using the boost, set the boost particles
	public void usingBoosting ()
	{
		if (otherCarParts.boostParticles) {
			for (int i = 0; i < boostingParticles.Count; i++) {
				if (usingBoost) {
					if (!boostingParticles [i].isPlaying) {
						boostingParticles [i].gameObject.SetActive (true);
						boostingParticles [i].Play ();
						boostingParticles [i].loop = true;
					}
				} else {
					boostingParticles [i].loop = false;
				}
			}
		}
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

	public void setJumpPower (float newJumpPower)
	{
		settings.jumpPower = newJumpPower;
	}

	public void setNewJumpPower (float newJumpPower)
	{
		settings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		settings.jumpPower = originalJumpPower;
	}

	public void setMaxSpeed (float maxSpeedValue)
	{
		settings.maxForwardSpeed = maxSpeedValue;
	}

	public void setMaxAcceleration (float maxAccelerationValue)
	{
		settings.engineTorque = maxAccelerationValue;
	}

	public void setMaxBrakePower (float maxBrakePower)
	{
		settings.brake = maxBrakePower;
	}

	public void setMaxTurboPower (float maxTurboPower)
	{
		settings.maxBoostMultiplier = maxTurboPower;
	}

	vehicleAINavMesh.movementInfo vehicleAI;

	public void Move (vehicleAINavMesh.movementInfo AI)
	{
		vehicleAI = AI;
	}

	//CALL INPUT FUNCTIONS
	public void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//jump input
			if (settings.canJump) {
				jump = true;
			}
		}
	}

	public void inputHoldOrReleaseJump (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (settings.canImpulseHoldingJump) {
				if (holdingButton) {
					if (Time.time > lastTimeJump + 0.2f) {
						usingImpulse = true;
					}
				} else {
					usingImpulse = false;
				}
			}
		}
	}

	public void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (holdingButton) {
				//boost input
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

	public void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl) {
			if (hudManager.canSetTurnOnState) {
				setEngineOnOrOffState ();
			}
		}
	}

	public void inputHorn ()
	{
		if (driving && !usingGravityControl) {
			pressHorn ();
		}
	}

	public void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl) {
			braking = holdingButton;
		}
	}

	[System.Serializable]
	public class Gears
	{
		public string Name;
		public AnimationCurve engineTorqueCurve;
		public float gearSpeed;
		public AudioClip gearShiftingClip;
	}

	[System.Serializable]
	public class Wheels
	{
		public string Name;
		public WheelCollider wheelCollider;
		public GameObject wheelMesh;
		public GameObject mudGuard;
		public GameObject suspension;
		public bool steerable;
		public bool powered;
		public bool leftSide;
		public bool rightSide;
		public ParticleSystem wheelParticleSystem;

		public bool reverseSteer;

		[HideInInspector] public Quaternion mudGuardOriginalRotation;
		[HideInInspector] public Vector3 mudGuardOffset;
		[HideInInspector] public Vector3 suspensionOffset;
		[HideInInspector] public float suspensionSpringPos;
		[HideInInspector] public float rotationValue;
		[HideInInspector] public float wheelSlipAmountSideways;
		[HideInInspector] public float wheelSlipAmountForward;
		[HideInInspector] public int lastSkidmark = -1;
	}

	[System.Serializable]
	public class OtherCarParts
	{
		public Transform SteeringWheel;
		public Transform COM;
		public GameObject wheelSlipPrefab;
		public GameObject chassis;
		public AudioClip engineStartClip;
		public AudioClip engineClip;
		public AudioClip engineEndClip;
		public AudioClip skidClip;
		public AudioClip[] crashClips;
		public AudioClip hornClip;
		public List<ParticleSystem> normalExhaust = new List<ParticleSystem> ();
		public List<ParticleSystem> heavyExhaust = new List<ParticleSystem> ();
		public AudioSource engineStartAudio;
		public AudioSource engineAudio;
		public AudioSource skidAudio;
		public AudioSource crashAudio;
		public AudioSource gearShiftingSound;
		public AudioSource hornAudio;
		public GameObject boostParticles;
	}

	[System.Serializable]
	public class carSettings
	{
		public LayerMask layer;
		public float engineTorque = 2500;
		public float maxRPM = 6000;
		public float minRPM = 1000;
		public float steerAngleLimit;
		public float highSpeedSteerAngle = 10;
		public float highSpeedSteerAngleAtSpeed = 100;
		public Vector2 driftAngleLimit = new Vector2 (-35, 35);
		public float steerWheelRotationPercentage = 8000;
		public int steeringAssistanceDivider = 5;

		public float steerInputSpeed = 10;

		public float brake = 4000;
		public float maxForwardSpeed;
		public float maxBackWardSpeed;
		public float maxBoostMultiplier;
		public float gearShiftRate = 10;
		public Vector2 chassisLean;
		public float chassisLeanLimit;
		public float antiRoll = 10000;
		public GameObject vehicleCamera;
		public bool preserveDirectionWhileInAir;
		public float jumpPower;
		public bool canJump;
		public bool canImpulseHoldingJump;
		public float impulseForce;
		public bool impulseUseEnergy;
		public float impulseUseEnergyRate;
		public float impulseUseEnergyAmount;
		public bool canUseBoost;
		public bool useCurves;
		public float horizontalLeanPassengerAmount;
		public float verticalLeanPassengerAmount;
		public float leanPassengerSpeed;
	}
}