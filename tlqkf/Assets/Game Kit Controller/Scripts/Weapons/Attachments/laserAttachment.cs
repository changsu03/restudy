using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class laserAttachment : laser
{
	public LayerMask layer;
	public playerWeaponsManager weaponsManager;
	public playerWeaponSystem weaponManager;
	public IKWeaponSystem IkWeaponManager;
	public bool sendMessageOnContact;
	public UnityEvent contantFunctions = new UnityEvent ();
	public bool laserEnabled = true;
	public float laserRotationSpeed = 20;
	public bool alwaysLookInCameraDirection;

	RaycastHit hit;
	Vector3 hitPointPosition;
	float rayDistance;
	public bool hittingSurface;
	GameObject lastObjectDetected;
	Transform mainCameraTransform;
	Quaternion targetRotation;
	Vector3 direction;

	GameObject objectDetectedByCamera;
	GameObject objectDetectedByLaser;

	bool sameObjectFound;

	Vector3 hitPointCameraDirection;

	void Start ()
	{
		rayDistance = Mathf.Infinity;
	}

	void Update ()
	{
		if (laserEnabled) {
			if (mainCameraTransform) {
				if (!weaponManager.weaponIsMoving () && (weaponManager.aimingInThirdPerson || weaponManager.carryingWeaponInFirstPerson)
				    && !weaponsManager.isEditinWeaponAttachments () && (!weaponManager.isWeaponOnRecoil () || alwaysLookInCameraDirection)
				    && !IkWeaponManager.isWeaponSurfaceDetected () && !IkWeaponManager.isWeaponInRunPosition () && !IkWeaponManager.isMeleeAtacking ()) {
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, rayDistance, layer)) {
						//Debug.DrawLine (mainCameraTransform.position, hit.point, Color.white, 2);
						direction = hit.point - transform.position;
						direction = direction / direction.magnitude;
						targetRotation = Quaternion.LookRotation (direction);

						objectDetectedByCamera = hit.collider.gameObject;

						if (IkWeaponManager.isWeaponSurfaceDetected()) {
							hitPointCameraDirection = direction;
						} else {
							hitPointCameraDirection = hit.point - mainCameraTransform.position;
							hitPointCameraDirection = hitPointCameraDirection / hitPointCameraDirection.magnitude;
						}
					} else {
						targetRotation = Quaternion.LookRotation (mainCameraTransform.forward);

						objectDetectedByCamera = null;

						hitPointCameraDirection = mainCameraTransform.forward;
					}

					if (sameObjectFound) {
						transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * laserRotationSpeed);
					}
				} else {
					if (sameObjectFound) {
						targetRotation = Quaternion.identity;
						transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, Time.deltaTime * laserRotationSpeed);
					}

					objectDetectedByCamera = null;
				}
			} else {
				mainCameraTransform = weaponManager.getMainCameraTransform ();
			}

			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);

			//check if the hitted object is the player, enabling or disabling his shield
			if (Physics.Raycast (transform.position, transform.forward, out hit, rayDistance, layer)) {
				//if the laser has been deflected, then check if any object collides with it, to disable all the other reflections of the laser
				hittingSurface = true;
				hitPointPosition = hit.point;
				if (hit.collider.gameObject != lastObjectDetected) {
					lastObjectDetected = hit.collider.gameObject;
					if (sendMessageOnContact) {
						if (contantFunctions.GetPersistentEventCount () > 0) {
							contantFunctions.Invoke ();
						}
					}
				}
			} else {
				//the laser does not hit anything, so disable the shield if it was enabled
				hittingSurface = false;
			}

			if (Physics.Raycast (transform.position, hitPointCameraDirection, out hit, rayDistance, layer)) {
				objectDetectedByLaser = hit.collider.gameObject;
				//Debug.DrawLine (transform.position, hit.point, Color.red, 2);
			} else {
				objectDetectedByLaser = null;
			}

			if (objectDetectedByCamera == objectDetectedByLaser || (objectDetectedByCamera == null && objectDetectedByLaser == null)) {
				sameObjectFound = true;
			} else {
				sameObjectFound = false;
			}

			if (!sameObjectFound) {
				hittingSurface = false;
				targetRotation = Quaternion.identity;
				transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, Time.deltaTime * laserRotationSpeed);
			}

			if (hittingSurface) {					
				lRenderer.SetPosition (1, hitPointPosition);
			} else {
				laserDistance = 1000;	
				lRenderer.SetPosition (1, (transform.position + laserDistance * transform.forward));
			}
		}
	}

	public void setLaserEnabledState (bool state)
	{
		laserEnabled = state;
		if (lRenderer) {
			lRenderer.enabled = state;
		}
		transform.localRotation = Quaternion.identity;
	}
}