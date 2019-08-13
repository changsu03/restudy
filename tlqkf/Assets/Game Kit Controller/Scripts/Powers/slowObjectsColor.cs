using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class slowObjectsColor : MonoBehaviour
{
	public Color slowColor;
	public bool changeToSlow = true;
	public bool changeToNormal;
	public float slowDownDuration;
	public float timer = 0;

	public string setNormalVelocityString = "setNormalVelocity";
	public string setReducedVelocityString = "setReducedVelocity";

	public List<Renderer> rendererParts = new List<Renderer> ();
	public List<Color> originalColor = new List<Color> ();
	public List<Color> transistionColor = new List<Color> ();

	bool renderPartsStored;
	float slowValue;
	int i = 0;
	int j = 0;
	slowObject slowObjectManager;
	GameObject objectToCallFunction;

	Renderer currenRenderer;

	void Start ()
	{	

	}

	void Update ()
	{
		//change the color smoothly from the original, to the other
		timer += Time.deltaTime;
		for (i = 0; i < rendererParts.Count; i++) {
			if (rendererParts [i]) {
				for (j = 0; j < rendererParts [i].materials.Length; j++) {
					rendererParts [i].materials [j].color = Color.Lerp (rendererParts [i].materials [j].color, transistionColor [i], timer);
				}
			}
		}

		//after the 80% of the time has passed, the color will change from the slowObjectsColor, to the original
		if (timer >= slowDownDuration * 0.8f && changeToSlow) {
			//set the transition color to the original
			changeToSlow = false;
			changeToNormal = true;
			transistionColor = originalColor;
			timer = 0;
		}

		//when the time is over, set the color and remove the script
		if (timer >= slowDownDuration * 0.2f && !changeToSlow && changeToNormal) {
			for (i = 0; i < rendererParts.Count; i++) {
				if (rendererParts [i]) {
					for (j = 0; j < rendererParts [i].materials.Length; j++) {
						rendererParts [i].materials [j].color = transistionColor [i];
					}
				}
			}
			if (objectToCallFunction) {
				objectToCallFunction.BroadcastMessage (setNormalVelocityString, SendMessageOptions.DontRequireReceiver);
			}
			Destroy (gameObject.GetComponent<slowObjectsColor> ());
		}
	}

	public void startSlowObject (Color newSlowColor, float newSlowValue, float newSlowDownDuration)
	{
		slowColor = newSlowColor;
		slowValue = newSlowValue;
		slowDownDuration = newSlowDownDuration;
		timer = 0;

		changeToSlow = true;
		changeToNormal = false;

		getComponents ();

		if (slowObjectManager.isUseCustomSlowSpeedEnabled ()) {
			slowValue = slowObjectManager.getCustomSlowSpeed ();
		}

		//send a message to slow down the object
		if (objectToCallFunction) {
			objectToCallFunction.BroadcastMessage (setReducedVelocityString, slowValue, SendMessageOptions.DontRequireReceiver);
		}
			
		if (!renderPartsStored) {
			bool useCustomSlowSpeed = slowObjectManager.useMeshesToIgnoreEnabled ();

			//get all the renderers inside of it, to change their color with the slowObjectsColor from otherpowers
			Component[] components = GetComponentsInChildren (typeof(Renderer));
			foreach (Renderer child in components) {
				currenRenderer = child as Renderer;
				if (!useCustomSlowSpeed || !slowObjectManager.checkChildsObjectsToIgnore (child.transform)) {
					if (currenRenderer.material.HasProperty ("_Color")) {
						for (j = 0; j < child.materials.Length; j++) {
							rendererParts.Add (child);
							originalColor.Add (currenRenderer.materials [j].color);
							transistionColor.Add (slowColor);
						}
					}
				}
			}
			renderPartsStored = true;
		}
	}

	public void getComponents ()
	{
		slowObjectManager = GetComponent<slowObject> ();
		objectToCallFunction = slowObjectManager.getObjectToCallFunction ();
	}
}