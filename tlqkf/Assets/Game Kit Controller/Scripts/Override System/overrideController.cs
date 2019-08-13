using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overrideController : MonoBehaviour
{
	public LayerMask layer;
	public float moveSpeedMultiplier;
	public float airControlAmount;
	public float jumpPower;
	public bool canJump;
	public float brakeForce = 5;
	public float collisionForceLimit;

	public bool damageObjectsEnabled;
	public float damageObjectsMultiplier;

	public float timeToJumpOnCollidingExit = 0.4f;

	public bool canImpulse;
	public float impulseForce;
	public float impulseCoolDown = 0.5f;

	//public float maxSpeed = 50;
	public float maxVelocityChange = 10;

	bool externalForceActivated;
	Vector3 externalForceValue;

	public LayerMask layerMask;
	public Transform raycastPosition;
	public float raycastDistance;

	public overrideInputManager overrideInput;
	public Transform overrideCameraTransform;
	public Transform overrideCameraParentTransform;

	public Transform controllerMeshParent;
	public GameObject controllerMesh;

	public bool controllerEnabled;
	public bool onGround;
	public float currentSpeed;

	float lastTimeImpulse;

	float horizontalAxis;
	float verticalAxis;
	Vector3 moveInput;

	Rigidbody mainRigidbody;

	Vector2 axisValues;
	bool braking;

	float lastTimeOnGround;

	RaycastHit hit;
	Vector3 forceToApply;

	void Start ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();	
	}

	void Update ()
	{
		if (controllerEnabled) {
			if (Physics.Raycast (raycastPosition.position, -raycastPosition.up, out hit, raycastDistance, layerMask)) {
				onGround = true;
				lastTimeOnGround = Time.time;
			} else {
				onGround = false;
			}

			axisValues = overrideInput.getCustomMovementAxis ("keys");
			horizontalAxis = axisValues.x;
			verticalAxis = axisValues.y;
		
			if (braking) {
				float verticalVelocity = overrideCameraTransform.InverseTransformDirection (mainRigidbody.velocity).y;
				Vector3 downVelocity = overrideCameraTransform.up * verticalVelocity;
				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero + downVelocity, Time.deltaTime * brakeForce);
			}

			moveInput = verticalAxis * overrideCameraTransform.forward + horizontalAxis * overrideCameraTransform.right;	
			//Vector3 force = moveInput * moveSpeedMultiplier;
		

//			if (currentSpeed > maxForwardSpeed) {
//				mainRigidbody.AddForce (Vector3.zero);
//				mainRigidbody.velocity = Vector3.ClampMagnitude (mainRigidbody.velocity, maxForwardSpeed);
//			} else {
//				if (colliding) {
//					mainRigidbody.AddForce (force);
//				} else {
//					mainRigidbody.AddForce (force * airControlAmount);
//				}
//			}
		}
	}

	void FixedUpdate ()
	{
		forceToApply = Vector3.zero;
		if (onGround) {
			forceToApply = moveInput * moveSpeedMultiplier;
		} else {
			forceToApply = moveInput * airControlAmount;
		}
	
		if (externalForceActivated) {
			mainRigidbody.AddForce (externalForceValue);
			onGround = false;
			externalForceActivated = false;
		}

		if (onGround) {
			currentSpeed = mainRigidbody.velocity.magnitude;
//			if (currentSpeed > maxSpeed) {
//				forceToApply = forceToApply - mainRigidbody.velocity;
//				
//				forceToApply = Vector3.ClampMagnitude (forceToApply, maxVelocityChange);
//			}

			mainRigidbody.AddForce (forceToApply, ForceMode.VelocityChange);

		} else {
			mainRigidbody.AddForce (forceToApply);
		}

	}

	public void changeControllerState (bool state)
	{
		controllerEnabled = state;

		braking = false;
		axisValues = Vector2.zero;
		moveInput = Vector3.zero;
		horizontalAxis = 0;
		verticalAxis = 0;
		if (mainRigidbody) {
			mainRigidbody.velocity = Vector3.zero;
			mainRigidbody.isKinematic = !state;
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if (damageObjectsEnabled) {
			//check that the collision is not with the player
			if (collision.contacts.Length > 0) {	
				//if the velocity of the collision is higher that the limit
				if (collision.relativeVelocity.magnitude > collisionForceLimit) {
					applyDamage.checkHealth (gameObject, collision.collider.gameObject, 
						collision.relativeVelocity.magnitude * damageObjectsMultiplier, 
						collision.contacts [0].normal, collision.contacts [0].point, gameObject, false, true);
				}
			}
		}
	}

	public void setControllerMesh (GameObject newControllerMesh)
	{
		controllerMesh = newControllerMesh;
		controllerMesh.transform.SetParent (controllerMeshParent);
		controllerMesh.AddComponent<parentAssignedSystem> ().assignParent (gameObject);
	}

	public GameObject getControllerMesh ()
	{
		return controllerMesh;
	}

	public void removeControllerMesh ()
	{
		controllerMesh.transform.SetParent (null);
		parentAssignedSystem currentParentAssignedSystem = controllerMesh.GetComponent<parentAssignedSystem> ();
		if (currentParentAssignedSystem) {
			Destroy (currentParentAssignedSystem);
		}
	}

	public void inputJump ()
	{
		if (canJump && onGround && Time.time < lastTimeOnGround + timeToJumpOnCollidingExit) {
			externalForceValue = overrideCameraTransform.up * mainRigidbody.mass * jumpPower;
			externalForceActivated = true;
		}
	}

	public void inputSetBrakeState (bool state)
	{
		if (state) {
			if (onGround) {
				braking = true;
			}
		} else {
			braking = false;
		}
	}

	public void inputImpulse ()
	{
		
		if (canImpulse) {
			if (Time.time > impulseCoolDown + lastTimeImpulse) {
				lastTimeImpulse = Time.time;
				Vector3 dashDirection = moveInput;
				dashDirection.Normalize ();
				if (dashDirection == Vector3.zero || dashDirection.magnitude < 0.1f) {
					dashDirection = overrideCameraParentTransform.forward;
				}
				externalForceValue = dashDirection * impulseForce * mainRigidbody.mass;
				externalForceActivated = true;
			}
		}
	}
}