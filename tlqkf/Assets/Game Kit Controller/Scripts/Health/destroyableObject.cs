using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class destroyableObject : MonoBehaviour
{
	public bool explosionEnabled = true;
	public GameObject destroyedParticles;
	public AudioClip destroyedSound;
	public bool useExplosionForceWhenDestroyed;
	public float explosionRadius;
	public float explosionForce;
	public float explosionDamage;
	public bool killObjectsInRadius;
	public ForceMode forceMode;

	public bool userLayerMask;
	public LayerMask layer;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	public bool pushCharactersOnExplosion = true;

	public float timeToFadePieces;

	public bool storeRendererParts = true;
	public bool disableColliders = true;

	public bool destroyObjectAfterFacePieces = true;

	public float meshesExplosionForce = 500;
	public float meshesExplosionRadius = 50;
	public ForceMode meshesExplosionForceMode;

	public Shader transparentShader;

	public bool destroyed;
	public bool showGizmo;

	List<Material> rendererParts = new List<Material> ();

	Rigidbody mainRigidbody;
	mapObjectInformation mapInformationManager;
	AudioSource destroyedSource;

	void Start ()
	{
		mainRigidbody = GetComponent<Rigidbody> ();
		mapInformationManager = GetComponent<mapObjectInformation> ();
		destroyedSource = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (storeRendererParts && destroyObjectAfterFacePieces) {
			if (destroyed) {
				if (timeToFadePieces > 0) {
					timeToFadePieces -= Time.deltaTime;
				}

				if (timeToFadePieces <= 0) {
					int piecesAmountFade = 0;
					for (int i = 0; i < rendererParts.Count; i++) {
						Color alpha = rendererParts [i].color;
						alpha.a -= Time.deltaTime / 5;
						rendererParts [i].color = alpha;
						if (alpha.a <= 0) {
							piecesAmountFade++;
						}
					}
					if (piecesAmountFade == rendererParts.Count) {
						Destroy (gameObject);
					}
				}
			}
		}
	}

	//Destroy the object
	public void destroyObject ()
	{
		if (!explosionEnabled) {
			return;
		}

		//instantiated an explosiotn particles
		if (destroyedParticles) {
			GameObject destroyedParticlesClone = (GameObject)Instantiate (destroyedParticles, transform.position, transform.rotation);
			destroyedParticlesClone.transform.SetParent (transform);
		}

		if (destroyedSource) {
			destroyedSource.PlayOneShot (destroyedSound);
		}

		//set the velocity of the object to zero
		if (mainRigidbody) {
			mainRigidbody.velocity = Vector3.zero;
			mainRigidbody.isKinematic = true;
		}

		//get every renderer component if the object
		if (storeRendererParts) {

			if (transparentShader == null) {
				transparentShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
			}

			Component[] components = GetComponentsInChildren (typeof(MeshRenderer));

			foreach (Component c in components) {
				Renderer currentRenderer = c.GetComponent<Renderer> ();

				if (currentRenderer && c.gameObject.layer != LayerMask.NameToLayer ("Scanner")) {
					if (currentRenderer.enabled) {
						//for every renderer object, change every shader in it for a transparent shader 
						for (int j = 0; j < currentRenderer.materials.Length; j++) {
							currentRenderer.materials [j].shader = transparentShader;
							rendererParts.Add (currentRenderer.materials [j]);
						}

						//set the layer ignore raycast to them
						c.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
						//add rigidbody and box collider to them
						Rigidbody currentRigidbody = c.gameObject.GetComponent<Rigidbody> ();

						if (!currentRigidbody) {
							currentRigidbody = c.gameObject.AddComponent<Rigidbody> ();
						} else {
							currentRigidbody.isKinematic = false;
							currentRigidbody.useGravity = true;
						}

						Collider currentCollider = c.gameObject.GetComponent<Collider> ();
						if (!currentCollider) {
							currentCollider = c.gameObject.AddComponent<BoxCollider> ();
						}

						//apply explosion force
						currentRigidbody.AddExplosionForce (meshesExplosionForce, transform.position, meshesExplosionRadius, 3, meshesExplosionForceMode);
					}
				} 
			}
		}

		if (disableColliders) {
			Collider colliderPart;
			//any other object with a collider but with out renderer, is disabled
			Component[] collidersInObject = GetComponentsInChildren (typeof(Collider));
			foreach (Component currentCollider in collidersInObject) {
				colliderPart = currentCollider as Collider;
				if (colliderPart && !currentCollider.GetComponent<Renderer> ()) {
					colliderPart.enabled = false;
				}
			}
		}

		if (mapInformationManager) {
			mapInformationManager.removeMapObject ();
		}

		if (useExplosionForceWhenDestroyed) {
			Vector3 currentPosition = transform.position;

			applyDamage.setExplosion (currentPosition, explosionRadius, userLayerMask, layer, gameObject, false, gameObject, killObjectsInRadius, true, false, 
				explosionDamage, pushCharactersOnExplosion, applyExplosionForceToVehicles, explosionForceToVehiclesMultiplier, explosionForce, forceMode, true, transform);

		}
		destroyed = true;
	}

	void OnDrawGizmos ()
	{
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
			if (useExplosionForceWhenDestroyed) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (transform.position, explosionRadius);
			}
		}
	}
}
