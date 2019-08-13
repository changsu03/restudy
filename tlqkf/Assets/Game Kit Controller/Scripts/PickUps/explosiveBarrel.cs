using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class explosiveBarrel : MonoBehaviour
{
	public GameObject brokenBarrel;
	public GameObject explosionParticles;
	public AudioClip explosionSound;
	public float explosionDamage;
	public float damageRadius;
	public float minVelocityToExplode;
	public float explosionDelay;
	public float explosionForce = 300;
	public bool breakInPieces;

	public float explosionForceToBarrelPieces = 5;
	public float explosionRadiusToBarrelPieces = 30;
	public ForceMode forceModeToBarrelPieces = ForceMode.Impulse;

	public bool pushCharacters = true;

	public bool killObjectsInRadius;

	public ForceMode explosionForceMode;

	public bool userLayerMask;
	public LayerMask layer;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	public Shader transparentShader;

//	public bool firstShootAddForce;
//	public float firstShootForce;
//	public ForceMode forceMode;
//	public bool useCustomForceDirection;
//	public Vector3 customDirection;

	public bool showGizmo;

	bool exploded;
	bool canExplode = true;
	List<Material> rendererParts = new List<Material> ();
	int i, j;
	float timeToRemove = 3;
	GameObject barrelOwner;
	Rigidbody mainRigidbody;
	bool isDamaged;

	Vector3 damageDirection;
	Vector3 damagePosition;

	grabbedObjectState currentGrabbedObject;

	void Start ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
	}

	void Update ()
	{
		//if the barrel has exploded, wait a seconds and then 
		if (exploded) {
			if (timeToRemove > 0) {
				timeToRemove -= Time.deltaTime;
			} else {
				//change the alpha of the color in every renderer component in the fragments of the barrel
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

//		if (isDamaged) {
//			if (useCustomForceDirection) {
//				mainRigidbody.AddForce (transform.TransformDirection (customDirection) * firstShootForce, forceMode);
//			} else {
//				mainRigidbody.AddForceAtPosition (-transform.InverseTransformDirection (damageDirection) * firstShootForce,
//					transform.position + transform.TransformDirection (damagePosition), forceMode);
//			}
//		}
	}

	//explode this barrel
	public void explodeBarrel ()
	{
		if (exploded) {
			return;
		}

		//if the barrel has not been throwing by the player, the barrel owner is the barrel itself
		if (barrelOwner == null) {
			barrelOwner = gameObject;
		}

		//disable the main mesh of the barrel and create the copy with the fragments of the barrel
		GetComponent<Collider> ().enabled = false;
		GetComponent<MeshRenderer> ().enabled = false;

		if (mainRigidbody) {
			mainRigidbody.isKinematic = true;
		}

		if (transparentShader == null) {
			transparentShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
		}

		Vector3 currentPosition = transform.position;

		//check all the colliders inside the damage radius
		applyDamage.setExplosion (currentPosition, damageRadius, userLayerMask, layer, barrelOwner, false, gameObject, killObjectsInRadius, true, false, 
			explosionDamage, pushCharacters, applyExplosionForceToVehicles, explosionForceToVehiclesMultiplier, explosionForce, explosionForceMode, true, barrelOwner.transform);

		//create the explosion particles
		GameObject explosionParticlesClone = (GameObject)Instantiate (explosionParticles, transform.position, transform.rotation);
		explosionParticlesClone.transform.SetParent (transform);

		//if the option break in pieces is enabled, create the barrel broken
		if (breakInPieces) {
			GameObject brokenBarrelClone = (GameObject)Instantiate (brokenBarrel, transform.position, transform.rotation);
			brokenBarrelClone.transform.localScale = transform.localScale;
			brokenBarrelClone.transform.SetParent (transform);
			brokenBarrelClone.GetComponent<AudioSource> ().PlayOneShot (explosionSound);
			Component[] components = brokenBarrelClone.GetComponentsInChildren (typeof(MeshRenderer));
			foreach (Component c in components) {
				//add force to every piece of the barrel and add a box collider
				c.gameObject.AddComponent<Rigidbody> ();
				c.gameObject.AddComponent<BoxCollider> ();
				c.GetComponent<Rigidbody> ().AddExplosionForce (explosionForceToBarrelPieces, c.transform.position, explosionRadiusToBarrelPieces, 1, forceModeToBarrelPieces);

				//change the shader of the fragments to fade them
				MeshRenderer renderPart = c.gameObject.GetComponent<MeshRenderer> ();
				for (j = 0; j < renderPart.materials.Length; j++) {
					renderPart.materials [j].shader = transparentShader;
					rendererParts.Add (renderPart.materials [j]);
				}
			}
		}

		//kill the health component, to call the functions when the object health is 0
		if (!applyDamage.checkIfDead (gameObject)) {
			applyDamage.checkHealth (gameObject, gameObject, applyDamage.getCurrentHealthAmount (gameObject), Vector3.zero, transform.position, gameObject, false, true);
		}
		//search the player in case he had grabbed the barrel when it exploded
		exploded = true;

		//if the object is being carried by the player, make him drop it
		currentGrabbedObject = GetComponent<grabbedObjectState> ();
		if (currentGrabbedObject) {
			GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gameObject);
		}
	}

	//if the player grabs this barrel, disable its explosion by collisions
	public void canExplodeState (bool state)
	{
		canExplode = state;
	}

	public void setExplosiveBarrelOwner (GameObject newBarrelOwner)
	{
		barrelOwner = newBarrelOwner;
	}

	//if the barrel collides at enough speed, explode it
	void OnCollisionEnter (Collision col)
	{
		if (mainRigidbody) {
			if (mainRigidbody.velocity.magnitude > minVelocityToExplode && canExplode && !exploded) {
				//if (!firstShootAddForce) {
					explodeBarrel ();
				//}
			}
		}
	}

	public void getBarrelRigidbody ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
	}

	public void setBarrelRigidbody (Rigidbody rigidbodyToUse)
	{
		mainRigidbody = rigidbodyToUse;
	}

	public void waitToExplode ()
	{
		if (explosionDelay > 0) {
			StartCoroutine (waitToExplodeCorutine ());
		} else {
			explodeBarrel ();
		}
	}

	//delay to explode the barrel
	IEnumerator waitToExplodeCorutine ()
	{
		yield return new WaitForSeconds (explosionDelay);
		explodeBarrel ();
	}

	//set the explosion values from other component
	public void setExplosionValues (float force, float radius)
	{
		explosionForce = force;
		damageRadius = radius;
	}

	public void damageDetected ()
	{
//		if (firstShootAddForce && !isDamaged) {
//			isDamaged = true;
//		}
	}

//	public void setDamagePosition (Vector3 position)
//	{
//		damagePosition = position;
//	}
//
//	public void setDamageDirection (Vector3 direcion)
//	{
//		damageDirection = direcion;
//	}

	//draw the lines of the pivot camera in the editor
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject) {
			DrawGizmos ();
		}
		#endif
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.position, damageRadius);
		}
	}
}