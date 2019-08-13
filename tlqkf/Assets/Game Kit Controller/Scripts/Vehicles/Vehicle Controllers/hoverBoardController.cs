using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hoverBoardController : MonoBehaviour
{
	public List<hoverEngineSettings> hoverEngineList = new List<hoverEngineSettings> ();
	public OtherCarParts otherCarParts;
	public hoverCraftSettings settings;
	public playerMovementSettings playerSettings;

	public float stabilityForce = 1;
	public float stabilitySpeed = 2;

	public bool useHorizontalInputLerp = true;

	public float minSteerInputIdle = 0.4f;
	public float minSteerInputMoving = 0.4f;

	float currentMinSteerInput;

	public inputActionManager actionManager;
	public vehicleCameraController vCamera;
	public vehicleHUDManager hudManager;
	public Rigidbody mainRigidbody;
	public vehicleGravityControl vehicleGravityControlManager;

	Vector3 normal;
	float audioPower = 0;
	float maxEnginePower;
	float boostInput = 1;
	float resetTimer;
	float motorInput;
	float horizontalAxis;
	float verticalAxis;
	float currentSpeed;
	float gravityCenterAngleX;
	float gravityCenterAngleZ;
	float angleZ;
	float currentExtraBodyRotation;
	float currentExtraSpineRotation;
	float originalJumpPower;
	float headLookCenterCurrentRotation;
	int i;
	int collisionForceLimit = 5;
	bool driving;
	bool jump = false;
	bool moving;
	bool usingBoost;
	bool usingGravityControl;
	bool anyOnGround;
	bool rotating;
	bool usingHoverBoardWaypoint;

	Animator animator;
	Transform playerSpine;
	Transform rightArm;
	Transform leftArm;
	hoverBoardWayPoints wayPointsManager;
	bool isTurnedOn;

	Vector2 axisValues;

	bool braking;
	Vector3 gravityForce;

	bool firstPersonActive;

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
		currentExtraBodyRotation = playerSettings.extraBodyRotation;
		currentExtraSpineRotation = playerSettings.extraSpineRotation;
		otherCarParts.gravityCenterCollider.enabled = false;
		originalJumpPower = settings.jumpPower;

		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	public float getVerticalAxis ()
	{
		return verticalAxis;
	}

	void Update ()
	{
		firstPersonActive = vCamera.isFirstPerson;

		if (!firstPersonActive) {

			angleZ = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).z) * Mathf.Rad2Deg;
			float angleX = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).x) * Mathf.Rad2Deg;

			float gravityAngleZ = 0;
			if (Mathf.Abs (angleZ) > 1) {
				gravityAngleZ = -angleZ;
			} else {
				gravityAngleZ = 0;
			}

			float gravityAngleX = 0;
			if (Mathf.Abs (angleX) > 1) {
				gravityAngleX = -angleX;
			} else {
				gravityAngleX = 0;
			}

			gravityCenterAngleX = Mathf.Lerp (gravityCenterAngleX, gravityAngleX, Time.deltaTime * 5);
			gravityCenterAngleZ = Mathf.Lerp (gravityCenterAngleZ, gravityAngleZ, Time.deltaTime * 5);
			gravityCenterAngleX = Mathf.Clamp (gravityCenterAngleX, -playerSettings.limitBodyRotationX, playerSettings.limitBodyRotationX);
			gravityCenterAngleZ = Mathf.Clamp (gravityCenterAngleZ, -playerSettings.limitBodyRotationZ, playerSettings.limitBodyRotationZ);
			otherCarParts.playerGravityCenter.transform.localEulerAngles = new Vector3 (gravityCenterAngleX, currentExtraBodyRotation, gravityCenterAngleZ);
			float forwardSpeed = (mainRigidbody.transform.InverseTransformDirection (mainRigidbody.velocity).z) * 3f;
			float bodyRotation = playerSettings.extraBodyRotation;
			float spineRotation = playerSettings.extraSpineRotation;
			if (forwardSpeed < -2) {
				bodyRotation = -playerSettings.extraBodyRotation;
				spineRotation = -playerSettings.extraSpineRotation;
			} 
			currentExtraBodyRotation = Mathf.Lerp (currentExtraBodyRotation, bodyRotation, Time.deltaTime * 5);
			currentExtraSpineRotation = Mathf.Lerp (currentExtraSpineRotation, spineRotation, Time.deltaTime * 5);
		}

		mainRigidbody.centerOfMass = settings.centerOfMassOffset;

		//if the player is driving this vehicle and the gravity control is not being used, then
		if (driving && !usingGravityControl) {
			axisValues = actionManager.getPlayerMovementAxis ("keys");
			horizontalAxis = axisValues.x;

			if (!useHorizontalInputLerp && touchPlatform) {
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
			hudManager.getSpeed (currentSpeed, settings.maxSpeed);
		} else {
			horizontalAxis = 0;
			verticalAxis = 0;
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
			if (hoverEngineList [i].turbine) {
				hoverEngineList [i].turbine.transform.Rotate (0, rpm * Time.deltaTime * 6, 0);
			}
			if (hoverEngineList [i].ParticleSystem) {
				hoverEngineList [i].ParticleSystem.emissionRate = hoverEngineList [i].maxEmission * hoverEngineList [i].maxEnginePower;
				hoverEngineList [i].ParticleSystem.transform.position = hoverEngineList [i].hit.point + hoverEngineList [i].hit.normal * hoverEngineList [i].dustHeight;
				hoverEngineList [i].ParticleSystem.transform.LookAt (hoverEngineList [i].hit.point + hoverEngineList [i].hit.normal * 10);
			}
		}

		audioPower = Mathf.Lerp (maxEnginePower, motorInput, settings.audioEngineSpeed);
		otherCarParts.engineAudio.volume = Mathf.Lerp (settings.engineMinVolume, settings.engineMaxVolume, audioPower);
		otherCarParts.engineAudio.pitch = Mathf.Lerp (settings.minAudioPitch, settings.maxAudioPitch, audioPower);

		//reset the vehicle rotation if it is upside down 
		if (currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (normal, transform.up);
			if (angle > 60 && !rotating) {
				resetTimer += Time.deltaTime;
				if (resetTimer > settings.timeToFlip) {
					resetTimer = 0;
					StartCoroutine (rotateVehicle ());
				}
			} else {
				resetTimer = 0;
			}
		}
	}

	void FixedUpdate ()
	{
		currentSpeed = mainRigidbody.velocity.magnitude;
		//apply turn
		if (usingHoverBoardWaypoint) {
			return;
		}

		if (Mathf.Approximately (horizontalAxis, 0)) {
			float localR = Vector3.Dot (mainRigidbody.angularVelocity, transform.up);
			mainRigidbody.AddRelativeTorque (0, -localR * settings.brakingTorque, 0);
		} else {
			float targetRoll = -settings.rollOnTurns * horizontalAxis;
			float roll = Mathf.Asin (transform.right.y) * Mathf.Rad2Deg;
			// only apply additional roll if we're not "overrolled"
			if (Mathf.Abs (roll) > Mathf.Abs (targetRoll)) {
				roll = 0;
			} else {
				roll = Mathf.DeltaAngle (roll, targetRoll);
			}
			mainRigidbody.AddRelativeTorque (0, horizontalAxis * settings.steeringTorque, roll * settings.rollOnTurnsTorque);
		}

		if (!usingGravityControl && !jump) {
			Vector3 localVelocity = transform.InverseTransformDirection (mainRigidbody.velocity);
			Vector3 extraForce = Vector3.Scale (settings.extraRigidbodyForce, localVelocity);
			mainRigidbody.AddRelativeForce (-extraForce * mainRigidbody.mass);
			//use every engine to keep the vehicle in the air
			for (i = 0; i < hoverEngineList.Count; i++) {
				if (!hoverEngineList [i].mainEngine) {
					//find force direction by rotating local up vector towards world up
					Vector3 engineUp = hoverEngineList [i].engineTransform.up;
					gravityForce = (normal * 9.8f).normalized;
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
				}
			}
				
			Vector3 torqueVector = Vector3.Cross (transform.up, vCamera.transform.up);
			mainRigidbody.AddTorque (torqueVector * stabilitySpeed * stabilitySpeed);
		

			//if the handbrake is pressed, set the brake torque value in every wheel
			if (braking) {
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

						gravityForce = (normal * 9.8f).normalized;
						//current speed along forward axis
						float speed = Vector3.Dot (mainRigidbody.velocity, transform.forward);
						//if the vehicle doesn't move by input, apply automatic brake 
						bool isAutoBraking = Mathf.Approximately (motorInput, 0) && settings.autoBrakingDeceleration > 0;
						float thrust = motorInput;
						if (isAutoBraking) {
							thrust = -Mathf.Sign (speed) * settings.autoBrakingDeceleration / settings.maxBrakingDeceleration;
						}

						//check if it is braking, for example speed and thrust have opposing signs
						bool isBraking = motorInput * speed < 0;
						//don't apply force if speed is max already
						if (Mathf.Abs (speed) < settings.maxSpeed || isBraking) {
							//position on speed curve
							float normSpeed = Mathf.Sign (motorInput) * speed / settings.maxSpeed;
							//apply acceleration curve and select proper maximum value
							float acc = settings.accelerationCurve.Evaluate (normSpeed) * (isBraking ? settings.maxBrakingDeceleration : thrust > 0 ? settings.maxForwardAcceleration : settings.maxReverseAcceleration);
							//drag should be added to the acceleration
							float sdd = speed * settings.extraRigidbodyForce.z;
							float dragForce = sdd + mainRigidbody.drag * speed;
							float force = acc * thrust + dragForce;
							//reduce acceleration if the vehicle is close to vertical orientation and is trrying to go higher
							float y = Vector3.Dot (transform.forward, gravityForce);

							if (settings.maxSurfaceAngle < 90 && y * thrust > 0) {
								if (!isAutoBraking) {
									float pitch2 = Mathf.Asin (Mathf.Abs (y)) * Mathf.Rad2Deg;
									if (pitch2 > settings.maxSurfaceAngle) {
										float forceDecrease = (pitch2 - settings.maxSurfaceAngle) / (90 - settings.maxSurfaceAngle) * settings.maxSurfaceVerticalReduction;
										force /= 1 + forceDecrease;
									}
								}
							}
							mainRigidbody.AddForce (transform.forward * force * boostInput * movementMultiplier, ForceMode.Acceleration);
						}
					}
				}
			}
		}

		anyOnGround = true;
		int totalWheelsOnAir = 0;
		for (i = 0; i < hoverEngineList.Count; i++) {
			if (!Physics.Raycast (hoverEngineList [i].engineTransform.position, -hoverEngineList [i].engineTransform.up, out hoverEngineList [i].hit, hoverEngineList [i].maxHeight, settings.layer)) {
				totalWheelsOnAir++;
			}
		}
		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == hoverEngineList.Count && anyOnGround) {
			anyOnGround = false;
		}
	}

	void LateUpdate ()
	{
		if (driving && !firstPersonActive) {
			if (playerSpine) {
				Quaternion rotationX = Quaternion.FromToRotation (playerSpine.transform.InverseTransformDirection (transform.right), 
					                       playerSpine.transform.InverseTransformDirection (transform.forward));
				Vector3 directionX = rotationX.eulerAngles;
				Quaternion rotationZ = Quaternion.FromToRotation (playerSpine.transform.InverseTransformDirection (transform.forward), 
					                       playerSpine.transform.InverseTransformDirection (transform.forward));
				Vector3 directionZ = rotationZ.eulerAngles;
				float angleX = directionX.x;
				if (angleX > 180) {
					angleX = Mathf.Clamp (angleX, playerSettings.maxSpineRotationX, 360);
				} else {
					angleX = Mathf.Clamp (angleX, 0, playerSettings.minSpineRotationX);
				}
				playerSpine.transform.localEulerAngles = new Vector3 (angleX - angleZ, playerSpine.transform.localEulerAngles.y, directionZ.z - currentExtraSpineRotation);

				float armRotation = angleZ;
				armRotation = Mathf.Clamp (armRotation, -playerSettings.maxArmsRotation, playerSettings.maxArmsRotation);
				float rightArmRotationX = rightArm.transform.localEulerAngles.x - armRotation;
				rightArm.transform.localEulerAngles = new Vector3 (rightArmRotationX, rightArm.transform.localEulerAngles.y, rightArm.transform.localEulerAngles.z);
				float leftArmRotationX = leftArm.transform.localEulerAngles.x + armRotation;
				leftArm.transform.localEulerAngles = new Vector3 (leftArmRotationX, leftArm.transform.localEulerAngles.y, leftArm.transform.localEulerAngles.z);
			
				float headAngle = 0;
				if (motorInput < 0 && Mathf.Abs (currentSpeed) > playerSettings.minBackwardVelocityToHeadRotation) {
					headAngle = playerSettings.maxHeadRotationBackward;
				}
				headLookCenterCurrentRotation = Mathf.Lerp (headLookCenterCurrentRotation, headAngle, Time.deltaTime * playerSettings.headRotationSpeed);

				playerSettings.headLookCenter.transform.localEulerAngles = new Vector3 (0, headLookCenterCurrentRotation, 0);
			}
		}
	}

	IEnumerator jumpCoroutine ()
	{
		jump = true;
		yield return new WaitForSeconds (0.5f);
		jump = false;
	}

	public void enterOrExitFromWayPoint (bool state)
	{
		usingHoverBoardWaypoint = state;
		vehicleGravityControlManager.enabled = !state;
		mainRigidbody.isKinematic = state;
	}

	public void receiveWayPoints (hoverBoardWayPoints wayPoints)
	{
		wayPointsManager = wayPoints;
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

		otherCarParts.gravityCenterCollider.enabled = driving;
		//set the same state in the gravity control components
		animator = GetComponentInChildren<Animator> ();
		if (animator) {
			playerSpine = animator.GetBoneTransform (playerSettings.playerSpine);
			rightArm = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
			leftArm = animator.GetBoneTransform (HumanBodyBones.LeftUpperArm);
		}
		vehicleGravityControlManager.changeGravityControlState (driving);
	}

	public void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
	}

	public void setTurnOffState (bool previouslyTurnedOn)
	{
		if (previouslyTurnedOn) {
			setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, true);
		}
		motorInput = 0;
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
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);

		setTurnOffState (false);

		otherCarParts.gravityCenterCollider.enabled = false;
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
		settings.jumpPower = newJumpPower;
	}

	public void setOriginalJumpPower ()
	{
		settings.jumpPower = originalJumpPower;
	}

	//CALL INPUT FUNCTIONS
	public void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && settings.canJump) {
			if (anyOnGround) {
				if (!jump && anyOnGround) {
					StartCoroutine (jumpCoroutine ());
					mainRigidbody.AddForce (normal * mainRigidbody.mass * settings.jumpPower, ForceMode.Impulse);
				}
			}

			if (usingHoverBoardWaypoint) {
				StartCoroutine (jumpCoroutine ());
				wayPointsManager.pickOrReleaseVehicle (false, false);
				mainRigidbody.AddForce ((normal + transform.forward) * mainRigidbody.mass * settings.jumpPower, ForceMode.Impulse);
			}
		}
	}

	public void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn && !usingHoverBoardWaypoint) {
			//boost input
			if (holdingButton) {
				if (settings.canUseBoost) {
					usingBoost = true;
					//set the camera move away action
					vCamera.usingBoost (true, "Boost");
				}
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
		if (driving && !usingGravityControl) {
			if (hudManager.canSetTurnOnState) {
				turnOnOrOff (!isTurnedOn, isTurnedOn);
			}
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
		public float maxEmission = 100;
		public float dustHeight = 0.1f;
		public float maxHeight = 2;
		public float engineForce = 300;
		public float damping = 10;
		public float Exponent = 2;
		public float maxEngineAngle = 15;
		public bool mainEngine;
		public float minRPM = 100;
		public float maxRPM = 200;
		public Transform turbine;
		[HideInInspector] public RaycastHit hit;
		[HideInInspector] public float maxEnginePower;
	}

	[System.Serializable]
	public class OtherCarParts
	{
		public Transform COM;
		public Transform playerGravityCenter;
		public GameObject chassis;
		public AudioClip engineClip;
		public AudioClip[] crashClips;
		public AudioSource engineAudio;
		public AudioSource crashAudio;
		public List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();
		public Collider gravityCenterCollider;
	}

	[System.Serializable]
	public class hoverCraftSettings
	{
		public LayerMask layer;
		public float steeringTorque = 120;
		public float brakingTorque = 200;
		public float maxSpeed = 30;
		public float maxForwardAcceleration = 20;
		public float maxReverseAcceleration = 15;
		public float maxBrakingDeceleration = 30;
		public float autoBrakingDeceleration = 20;
		public float rollOnTurns = 10;
		public float rollOnTurnsTorque = 10;
		public float timeToFlip = 2;
		public float audioEngineSpeed = 0.5f;
		public float engineMinVolume = 0.5f;
		public float engineMaxVolume = 1;
		public float minAudioPitch = 0.4f;
		public float maxAudioPitch = 1;
		public AnimationCurve accelerationCurve;
		public float maxSurfaceVerticalReduction = 10;
		public float maxSurfaceAngle = 110;
		public Vector3 extraRigidbodyForce = new Vector3 (2, 0.1f, 0.2f);
		public Vector3 centerOfMassOffset;
		public float maxBoostMultiplier;
		[Range (0, 1)] public float inAirMovementMultiplier;
		public GameObject vehicleCamera;
		public float jumpPower;
		public bool canJump;
		public bool canUseBoost;
	}

	[System.Serializable]
	public class playerMovementSettings
	{
		public float extraBodyRotation = -20;
		public float extraSpineRotation = 30;
		public float limitBodyRotationX = 30;
		public float limitBodyRotationZ = 30;
		public float minSpineRotationX = 20;
		public float maxSpineRotationX = 330;
		public float maxArmsRotation = 25;

		public HumanBodyBones playerSpine;
		public Transform headLookCenter;
		public float maxHeadRotationBackward;
		public float headRotationSpeed;
		public float minBackwardVelocityToHeadRotation;
	}
}