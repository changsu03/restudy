using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class laserPlayer : laser
{
	public float laserDamage = 0.3f;
	public LayerMask layer;
	public GameObject hitSmoke;
	public GameObject hitSparks;
	public laserDevice.laserType lasertype;
	public int reflactionLimit = 10;
	public GameObject player;
	Vector3 inDirection;
	Vector3 laserHitPosition;
	public GameObject currentLaser;
	RaycastHit hit;
	Ray ray;
	int nPoints;
	Transform mainCameraTransform;
	Renderer currentLaserRenderer;

	void Start ()
	{
		StartCoroutine (laserAnimation ());
		reflactionLimit++;
		mainCameraTransform = player.GetComponent<playerController> ().getPlayerCameraGameObject ().GetComponent<playerCamera> ().getCameraTransform ();
	}

	void Update ()
	{
		//the player's laser can be reflected, so the linerenderer has reflactionLimit vertex
		if (lasertype == laserDevice.laserType.refraction) {
			reflactionLimit = Mathf.Clamp (reflactionLimit, 1, reflactionLimit);
			ray = new Ray (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward));  
			nPoints = reflactionLimit;  			 
			//make the lineRenderer have nPoints  
			lRenderer.positionCount = reflactionLimit;  
			//set the first point of the line it its current positions
			lRenderer.SetPosition (0, transform.position);  
			for (int i = 0; i < reflactionLimit; i++) {  
				//if the ray has not be reflected yet  
				if (i == 0) {  
					//check if the ray has hit something  
					if (Physics.Raycast (ray.origin, ray.direction, out hit, Mathf.Infinity, layer)) {  
						//the reflection direction is the reflection of the current ray direction flipped at the hit normal  
						inDirection = Vector3.Reflect (ray.direction, hit.normal);  
						//cast the reflected ray, using the hit point as the origin and the reflected direction as the direction  
						ray = new Ray (hit.point, inDirection);  
						//if the number of reflections is set to 1  
						if (reflactionLimit == 1) {  
							//add a new vertex to the line renderer  
							lRenderer.positionCount = nPoints++;  
						}  
						//set the position of the next vertex at the line renderer to be the same as the hit point  
						lRenderer.SetPosition (i + 1, hit.point);  
						laserDistance = hit.distance;
					} else {
						//if the rays does not hit anything, set as a single straight line in the camera direction and disable the smoke
						laserDistance = 1000;
						transform.rotation = mainCameraTransform.rotation;
						hitSmoke.SetActive (false);
						hitSparks.SetActive (false);
						lRenderer.positionCount = 2;
						lRenderer.SetPosition (0, transform.position);
						lRenderer.SetPosition (1, (laserDistance * transform.forward));
					}
				} else if (i > 0) {  
					//check if the ray has hit something  
					if (Physics.Raycast (ray.origin, ray.direction, out hit, Mathf.Infinity, layer)) {  
						//the refletion direction is the reflection of the ray's direction at the hit normal  
						inDirection = Vector3.Reflect (inDirection, hit.normal);  
						//cast the reflected ray, using the hit point as the origin and the reflected direction as the direction  
						ray = new Ray (hit.point, inDirection);  
						lRenderer.positionCount = nPoints++;  
						//set the position of the next vertex at the line renderer to be the same as the hit point  
						lRenderer.SetPosition (i + 1, hit.point); 
						if (i + 1 == reflactionLimit) {
							//if this linerenderer vertex is the last, set the smoke in its position and check for a refraction cube or  a laser receiver
							hitSparks.SetActive (true);
							hitSmoke.SetActive (true);
							hitSmoke.transform.position = hit.point;
							hitSmoke.transform.rotation = Quaternion.identity;
							hitSparks.transform.rotation = Quaternion.LookRotation (hit.normal, transform.up);
							if (hit.collider.GetComponent<laserReceiver>() || hit.collider.GetComponent<refractionCube>()) {
								connectLasers (hit.collider.gameObject);
							}
							//check if the laser hits an object with a health component different from the player
							if (hit.collider.gameObject != player) {
								applyDamage.checkHealth (gameObject, hit.collider.gameObject, laserDamage, -transform.forward, hit.point, player, true, true);
							}
						}
						laserDistance = hit.distance;
					}  
				}
			}  
		}

		//the player's laser cannot be reflected, so the linerenderer only has 2 vertex
		if (lasertype == laserDevice.laserType.simple) {
			animateLaser ();
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
				//set the direction of the laser in the hit point direction
				transform.LookAt (hit.point);
				//check with a raycast if the laser hits a receiver, a refraction cube or a gameObject with a health component or a vehicle damage receiver
				//Debug.DrawRay (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward)*hit.distance, Color.yellow);
				if (hit.collider.GetComponent<laserReceiver>() || hit.collider.GetComponent<refractionCube>()) {
					connectLasers (hit.collider.gameObject);
				}
				if (hit.collider.gameObject != player) {
					applyDamage.checkHealth (gameObject, hit.collider.gameObject, laserDamage, -transform.forward, hit.point, player, true, true);
				}
				//get the hit position to set the particles of smoke and sparks
				laserDistance = hit.distance;
				hitSparks.SetActive (true);
				hitSmoke.SetActive (true);
				hitSmoke.transform.position = hit.point - transform.forward * 0.02f;
				hitSmoke.transform.rotation = Quaternion.identity;
				hitSparks.transform.rotation = Quaternion.LookRotation (hit.normal, transform.up);
				lRenderer.positionCount = 2;
				lRenderer.SetPosition (0, transform.position);
				lRenderer.SetPosition (1, hit.point);
				//set the laser size 
			} else {
				//set the direction of the laser in the camera forward
				Quaternion lookDir = Quaternion.LookRotation (mainCameraTransform.TransformDirection (Vector3.forward));
				transform.rotation = lookDir;
				hitSmoke.SetActive (false);
				hitSparks.SetActive (false);
				laserDistance = 1000;
				lRenderer.positionCount = 2;
				lRenderer.SetPosition (0, transform.position);
				lRenderer.SetPosition (1, (laserDistance * transform.forward));
			}
		}
	}

	void connectLasers (GameObject hitObject)
	{
		//check if the object touched with the laser is a laser receiver, to check if the current color of the laser is equal to the color needed
		//in the laser receiver
		laserReceiver currentLaserReceiver = hitObject.GetComponent<laserReceiver> ();
		refractionCube currentRefractionCube = hitObject.GetComponent<refractionCube> ();
		if (currentLaserReceiver) {
			if (currentLaserReceiver.colorNeeded == mainRenderer.material.GetColor ("_TintColor")) {
				currentLaserReceiver.laserConnected (mainRenderer.material.GetColor ("_TintColor"));
			} else {
				//else the laser is not reflected
				return;
			}
		}
		//if the object is not a laser receiver or a refraction cube, the laser is not refrated
		else if (!currentRefractionCube) {
			return;
		}else {
			//if the cube is already being used by another laser, cancel the action
			if (currentRefractionCube.isRefracting ()) {
				return;
			}
		}
		gameObject.SetActive (false);
		//deflect the laser and enable the laser connector 
		GameObject baseLaserConnector = currentLaser.GetComponent<laserDevice> ().laserConnector;
		baseLaserConnector.SetActive (true);
		baseLaserConnector.transform.position = laserHitPosition;
		baseLaserConnector.transform.LookAt (hitObject.transform.position);
		if (currentRefractionCube) {
			//if the hitted objects is a cube refraction, enable the laser inside it
			if (!currentRefractionCube.isRefracting ()) {
				baseLaserConnector.GetComponent<laserConnector> ().setCubeRefractionLaser (hitObject);
				currentRefractionCube.cubeLaserDeviceGameObject.transform.rotation = baseLaserConnector.transform.rotation;
				currentRefractionCube.setRefractingLaserState (true);
			} else {
				baseLaserConnector.SetActive (false);
				currentRefractionCube.setRefractingLaserState (false);
				return;
			}
		}
		//stop the laser that hits the player from detect any other collision, to deflect it
		currentLaser.SendMessage ("assignLaser");
		baseLaserConnector.GetComponent<laserConnector> ().setCurrentLaser (currentLaser);
		baseLaserConnector.GetComponent<laserConnector> ().setColor ();
		currentLaser = null;
	}

	//set the color of the laser according to the color of the laser device
	void setColor ()
	{
		if (currentLaserRenderer.material.HasProperty ("_TintColor")) {
			Color c = currentLaserRenderer.material.GetColor ("_TintColor");
			mainRenderer.material.SetColor ("_TintColor", c);
		}
	}

	public void setLaserInfo (laserDevice.laserType type, GameObject laser, Vector3 pos)
	{
		//get the position where the lasers hits the player, 
		laserHitPosition = pos;
		//get the laser that it is hitting the player
		currentLaser = laser;
		if (!currentLaserRenderer) {
			currentLaserRenderer = currentLaser.GetComponent<Renderer> ();
		}
		setColor ();
		lasertype = type;
		if (lasertype == laserDevice.laserType.refraction) {
			//lRenderer.useWorldSpace=true;
		} else {
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, Vector3.zero);
			//lRenderer.useWorldSpace=false;
		}
	}

	public void removeLaserInfo(){
		currentLaserRenderer = null;
	}
}