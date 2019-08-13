using UnityEngine;
using System.Collections;

public class vehicleLaser : laser
{
	public LayerMask layer;
	public GameObject hitParticles;
	public GameObject hitSparks;
	public GameObject vehicle;
	public GameObject vehicleCamera;
	[HideInInspector] public float laserDamage = 0.3f;
	RaycastHit hit;
	bool working;
	Transform mainCameraTransform;
	vehicleCameraController vehicleCameraManager;
	vehicleHUDManager vehicleHUD;
	GameObject currentDriver;
	Vector3 laserPosition;

	void Start ()
	{
		vehicleHUD = vehicle.GetComponent<vehicleHUDManager> ();
		vehicleCameraManager = vehicleCamera.GetComponent<vehicleCameraController> ();
		changeLaserState (false);
	}
		
	void Update ()
	{
		if (working) {
			//check the hit collider of the raycast
			mainCameraTransform = vehicleCameraManager.getCurrentCameraTransform ();
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layer)) {
				Debug.DrawRay (transform.position,  mainCameraTransform.forward*hit.distance, Color.red);
				transform.LookAt (hit.point);
				applyDamage.checkHealth (gameObject, hit.collider.gameObject, laserDamage, -transform.forward, (hit.point - (hit.normal / 4)), currentDriver, true, true);
				//set the sparks and .he smoke in the hit point
				laserDistance = hit.distance;
				hitSparks.SetActive (true);
				hitParticles.SetActive (true);
				hitParticles.transform.position = hit.point + (transform.position - hit.point) * 0.02f;
				hitParticles.transform.rotation = Quaternion.identity;
				hitSparks.transform.rotation = Quaternion.LookRotation (hit.normal, transform.up);
				laserPosition = hit.point;
			} else {
				//if the laser does not hit anything, disable the particles and set the hit point
				hitParticles.SetActive (false);
				hitParticles.SetActive (false);
				laserDistance = 1000;	
				Quaternion lookDir = Quaternion.LookRotation (mainCameraTransform.forward);
				transform.rotation = lookDir;
				laserPosition = (laserDistance * transform.forward);
			}
			//set the size of the laser, according to the hit position
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);
			lRenderer.SetPosition (1, laserPosition);
			animateLaser ();
		}
	}

	//enable or disable the vehicle laser
	public void changeLaserState (bool state)
	{
		lRenderer.enabled = state;
		working = state;
		if (state) {
			StartCoroutine (laserAnimation ());
		} else {
			hitSparks.SetActive (false);
			hitParticles.SetActive (false);
		}
		currentDriver = vehicleHUD.getCurrentDriver ();
	}
}