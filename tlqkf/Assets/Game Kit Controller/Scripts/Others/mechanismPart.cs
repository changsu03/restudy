using UnityEngine;
using System.Collections;

public class mechanismPart : MonoBehaviour
{
	public bool enableRotation;
	public GameObject rotor;
	public int mechanimType;
	public Vector3 rotateDirection;
	public GameObject gear;

	float speed = 0;
	bool gearActivated;
	bool rotatoryGearEngaged;
	grabbedObjectState currentGrabbedObject;
	electronicDevice electronicDeviceManager;

	void Start ()
	{
		electronicDeviceManager = GetComponent<electronicDevice> ();
	}

	void Update ()
	{
		//the script checks if the object on rails has been engaged
		if (enableRotation && mechanimType == 0) {
			rotor.transform.Rotate (rotateDirection * (-speed * Time.deltaTime));
		}
		if (enableRotation && mechanimType == 1) {
			if (rotatoryGearEngaged) {
				gear.transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
				rotor.transform.Rotate (rotateDirection * (-speed * Time.deltaTime));
			}
			if (gear && gearActivated) {
				if (gear.transform.localEulerAngles.z > 350) {
					//if the object is being carried by the player, make him drop it
					currentGrabbedObject = gear.GetComponent<grabbedObjectState> ();
					if (currentGrabbedObject) {
						GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gear);
					}
					gear.tag = "Untagged";
					gearActivated = false;
					rotatoryGearEngaged = true;
					electronicDeviceManager.unlockObject ();
				} else if (gear.tag != "box") {
					gear.name = "rotatoryGear";
					gear.tag = "box";
				}
			}
		}
	}

	void setVelocity (float v)
	{
		speed = v;
	}

	void engaged ()
	{
		enableRotation = true;
	}

	void gearRotated (GameObject gearAsigned)
	{
		gearActivated = true;
		gear = gearAsigned;
	}
}