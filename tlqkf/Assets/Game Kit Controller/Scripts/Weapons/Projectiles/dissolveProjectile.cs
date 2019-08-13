using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dissolveProjectile : projectileSystem
{
	public LayerMask layer;

	public Shader shaderToApply;
	public Texture dissolveTexture;
	public Color dissolveColor;
	public float dissolveColorAlpha;

	public float timeToDestroyObject = 0.9f;

	public string dissolveShaderFieldName = "_Amount";
	public string dissolveShaderTextureFieldName = "_DissolveTexture";
	public string dissolveShaderColorFieldName = "_DissolveColor";
	public string dissolveShaderAlphaColorFieldName = "_DissolveColorAlpha";
		
	public float dissolveSpeed = 4;
	public float currentFadeValue = 1;

	public bool objectToDissolveFound;

	public GameObject projectileMesh;

	GameObject objectToDissolve;

	public List<Renderer> rendererParts = new List<Renderer> ();

	public enum typeObjectFound
	{
		vehicle,
		regularObject,
		npc
	}

	public typeObjectFound currentTypeObjectFound;

	void Update ()
	{
		if (objectToDissolveFound) {
			currentFadeValue += Time.deltaTime * dissolveSpeed;
			for (int i = 0; i < rendererParts.Count; i++) {
				if (rendererParts [i]) {
					for (int j = 0; j < rendererParts [i].materials.Length; j++) {
						rendererParts [i].materials [j].SetFloat (dissolveShaderFieldName, currentFadeValue);
					}
				}
			}

			if (currentFadeValue >= 1 || currentFadeValue > timeToDestroyObject) {
				destroyObject ();
			} 
		}
	}

	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}
			projectileUsed = true;

			//set the bullet kinematic
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			mainRigidbody.isKinematic = true;

			if ((1 << col.gameObject.layer & layer.value) == 1 << col.gameObject.layer) {

				if (objectToDamage.GetComponent<Rigidbody> ()) {

					objectToDissolve = objectToDamage;

					objectToDissolveFound = true;

					currentTypeObjectFound = typeObjectFound.regularObject;
				} 
					
				characterDamageReceiver currentCharacterDamageReceiver = objectToDamage.GetComponent<characterDamageReceiver> ();
				if (currentCharacterDamageReceiver) {

					objectToDissolve = currentCharacterDamageReceiver.character;

					objectToDissolveFound = true;

					transform.SetParent (objectToDamage.transform);

					currentTypeObjectFound = typeObjectFound.npc;

				} else {
					vehicleDamageReceiver currentVehicleDamageReceiver = objectToDamage.GetComponent<vehicleDamageReceiver> ();
					if (currentVehicleDamageReceiver) {
						objectToDissolve = currentVehicleDamageReceiver.vehicle;

						objectToDissolveFound = true;

						transform.SetParent (currentVehicleDamageReceiver.vehicle.transform);

						currentTypeObjectFound = typeObjectFound.vehicle;
					}
				}

				if (objectToDissolveFound) {

					outlineObjectSystem currentOutlineObjectSystem = objectToDissolve.GetComponent<outlineObjectSystem> ();
					if (currentOutlineObjectSystem) {
						currentOutlineObjectSystem.disableOutlineAndRemoveUsers ();
					}

					storeRenderElements ();

					projectilePaused = true;

					projectileMesh.SetActive (false);
				}
			}

			checkProjectilesParent ();
		}
	}

	public void destroyObject ()
	{
		if (currentTypeObjectFound == typeObjectFound.vehicle) {
			objectToDissolve.GetComponent<vehicleHUDManager> ().destroyVehicleAtOnce ();
		}

		if (currentTypeObjectFound == typeObjectFound.npc) {
			playerController currentPlayerController = objectToDissolve.GetComponent<playerController> ();
			if (currentPlayerController) {
				currentPlayerController.destroyCharacterAtOnce ();
			} else {
				Destroy (objectToDissolve);
			}
		}

		if (currentTypeObjectFound == typeObjectFound.regularObject) {
			Destroy (objectToDissolve);
		}

		destroyProjectile ();
	}

	public void storeRenderElements ()
	{
		Component[] components = objectToDissolve.GetComponentsInChildren (typeof(Renderer));
		foreach (Renderer child in components) {
			if (child.GetComponent<Renderer> ().material.shader) {
				Renderer render = child.GetComponent<Renderer> ();
				for (int i = 0; i < render.materials.Length; i++) {
					rendererParts.Add (render);
				}
			}
		}

		for (int i = 0; i < rendererParts.Count; i++) {
			if (rendererParts [i]) {
				for (int j = 0; j < rendererParts [i].materials.Length; j++) {
					rendererParts [i].materials [j].shader = shaderToApply;
					rendererParts [i].materials [j].SetTexture (dissolveShaderTextureFieldName, dissolveTexture);
					rendererParts [i].materials [j].SetColor (dissolveShaderColorFieldName, dissolveColor);
					rendererParts [i].materials [j].SetFloat (dissolveShaderAlphaColorFieldName, dissolveColorAlpha);
				}
			}
		}
	}
}