using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class crate : MonoBehaviour
{
	public GameObject brokenCrate;
	public AudioClip brokenSound;
	public float minVelocityToBreak;
	public float timeToRemove = 3;
	public float breakForce = 10;
	public ForceMode forceMode;

	public Shader transparentShader;

	Rigidbody mainRigidbody;

	bool broken;
	public bool canBeBroken = true;
	List<Material> rendererParts = new List<Material> ();
	int i, j;

	grabbedObjectState currentGrabbedObject;

	void Start ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
	}

	void Update ()
	{
		//if the crate has been broken, wait x seconds and then 
		if (broken) {
			if (timeToRemove > 0) {
				timeToRemove -= Time.deltaTime;
			} else {
				//change the alpha of the color in every renderer component in the fragments of the crate
				for (i = 0; i < rendererParts.Count; i++) {
					Color alpha = rendererParts [i].color;
					alpha.a -= Time.deltaTime / 5;
					rendererParts [i].color = alpha;
					//once the alpha is 0, remove the gameObject
					if (rendererParts [i].color.a <= 0) {
						Destroy (gameObject);
					}
				}
			}
		}		
	}

	//break this crate
	public void breakCrate ()
	{
		//disable the main mesh of the crate and create the copy with the fragments
		Vector3 originalRigidbodyVelocity = mainRigidbody.velocity;
		GetComponent<Collider> ().enabled = false;
		GetComponent<MeshRenderer> ().enabled = false;
		mainRigidbody.isKinematic = true;

		//if the option break in pieces is enabled, create the broken crate
		GameObject brokenCrateClone = (GameObject)Instantiate (brokenCrate, transform.position, transform.rotation);
		brokenCrateClone.transform.localScale = transform.localScale;
		brokenCrateClone.transform.SetParent (transform);
		brokenCrateClone.GetComponent<AudioSource> ().PlayOneShot (brokenSound);

		if (transparentShader == null) {
			transparentShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
		}
			
		Component[] components = brokenCrateClone.GetComponentsInChildren (typeof(MeshRenderer));
		foreach (Component c in components) {
			//add a box collider to every piece of the crate
			c.gameObject.AddComponent<Rigidbody> ();
			c.gameObject.AddComponent<MeshCollider> ().convex = true;
			//change the shader of the fragments to fade them
			MeshRenderer renderPart = c.gameObject.GetComponent<MeshRenderer> ();
			for (j = 0; j < renderPart.materials.Length; j++) {
				renderPart.materials [j].shader = transparentShader;
				rendererParts.Add (renderPart.materials [j]);
			}

			if (originalRigidbodyVelocity.magnitude > minVelocityToBreak) {
				c.GetComponent<Rigidbody> ().AddForce (originalRigidbodyVelocity, forceMode);
			} else {
				c.GetComponent<Rigidbody> ().AddForce ((c.transform.position - transform.position) * breakForce, forceMode);
			}
		}
			
		//kill the health component, to call the functions when the object health is 0
		if (!applyDamage.checkIfDead (gameObject)) {
			applyDamage.checkHealth (gameObject, gameObject, applyDamage.getCurrentHealthAmount (gameObject), 
				Vector3.zero, transform.position, gameObject, false, true);
		}

		//search the player in case he had grabbed the crate when it exploded
		broken = true;

		//if the object is being carried by the player, make him drop it
		currentGrabbedObject = GetComponent<grabbedObjectState> ();
		if (currentGrabbedObject) {
			GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gameObject);
		}
	}

	//if the player grabs this crate, disable its the option to break it
	public void crateCanBeBrokenState (bool state)
	{
		canBeBroken = state;
	}

	//if the crate collides at enough speed, break it
	void OnCollisionEnter (Collision col)
	{
		if (mainRigidbody && mainRigidbody.velocity.magnitude > minVelocityToBreak && canBeBroken && !broken) {
			breakCrate ();
		}
	}

	public void getCrateRigidbody ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
	}

	public void setBarrelRigidbody (Rigidbody rigidbodyToUse)
	{
		mainRigidbody = rigidbodyToUse;
	}
}