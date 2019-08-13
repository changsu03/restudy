using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noiseSystem : MonoBehaviour
{
	public bool useNoise;
	public float noiseRadius;
	public float noiseExpandSpeed;
	public bool useNoiseDetection;
	public LayerMask noiseDetectionLayer;

	public bool usedOnRigidbody;
	public bool useRigidbodyCollisionSpeed;
	public float maxCollisionSpeedValue;
	public bool enableOnlyUsedByPlayer;

	public float collisionNoiseMultiplier = 1;

	public bool showNoiseDetectionGizmo;

	noiseMeshSystem noiseMeshManager;
	float extraNoiseRadiusValue;
	float extraNoiseExpandSpeedValue;
	float collisionSpeed;

	GameObject impactObject;
	decalManager impactDecalManager;

	public void activateNoise ()
	{
		if (useNoise) {

			bool canActivateNoise = false;

			bool decalInfoFound = false;

			if (impactObject) {
				impactDecalManager = FindObjectOfType<decalManager> ();
				decalTypeInformation currentDecalTypeInformation = impactObject.GetComponent<decalTypeInformation> ();
				if (currentDecalTypeInformation) {
					decalInfoFound = true;
					if (impactDecalManager.surfaceUseNoise (currentDecalTypeInformation.getDecalImpactIndex ())) {
						print ("can make noise");
						canActivateNoise = true;
					}
				} 

				if (!canActivateNoise) {
					health healthManager = impactObject.GetComponent<health> ();
					if (healthManager) {
						decalInfoFound = true;
						if (impactDecalManager.surfaceUseNoise (healthManager.getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					} 
				}

				if (!canActivateNoise) {
					characterDamageReceiver currentCharacterDamageReceiver = impactObject.GetComponent<characterDamageReceiver> ();
					if (currentCharacterDamageReceiver) {
						decalInfoFound = true;
						if (impactDecalManager.surfaceUseNoise (currentCharacterDamageReceiver.getHealthManager ().getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}

				if (!canActivateNoise) {
					vehicleHUDManager currentVehicleHUDManager = impactObject.GetComponent<vehicleHUDManager> ();
					if (currentVehicleHUDManager) {
						decalInfoFound = true;
						if (impactDecalManager.surfaceUseNoise (currentVehicleHUDManager.getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}

				if (!canActivateNoise) {
					vehicleDamageReceiver currentVehicleDamageReceiver = impactObject.GetComponent<vehicleDamageReceiver> ();
					if (currentVehicleDamageReceiver) {
						decalInfoFound = true;
						if (impactDecalManager.surfaceUseNoise (currentVehicleDamageReceiver.getHUDManager ().getDecalImpactIndex ())) {
							canActivateNoise = true;
						}
					}
				}
			} else {
				canActivateNoise = true;
			}

			if (canActivateNoise || !decalInfoFound) {
				if (useNoiseDetection) {
					applyDamage.sendNoiseSignal (noiseRadius + extraNoiseRadiusValue, transform.position, noiseDetectionLayer, showNoiseDetectionGizmo);
				}

				noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();

				if (noiseMeshManager) {
					noiseMeshManager.addNoiseMesh (noiseRadius + extraNoiseRadiusValue, transform.position, noiseExpandSpeed + extraNoiseExpandSpeedValue);
				}

				extraNoiseRadiusValue = 0;
				extraNoiseExpandSpeedValue = 0;

				if (enableOnlyUsedByPlayer) {
					useNoise = false;
				}
			}
		}
	}

	public void OnCollisionEnter (Collision col)
	{
		if (useNoise && usedOnRigidbody) {
			impactObject = col.gameObject;

			if (useRigidbodyCollisionSpeed) {

				collisionSpeed = Mathf.Abs (col.relativeVelocity.magnitude * collisionNoiseMultiplier);
				collisionSpeed = Mathf.Clamp (collisionSpeed, 0, maxCollisionSpeedValue);
				extraNoiseRadiusValue = collisionSpeed;
				extraNoiseExpandSpeedValue = extraNoiseRadiusValue * 2;
			}

			activateNoise ();
		}
	}

	public void setUseNoiseState (bool state)
	{
		useNoise = state;
	}

	public void setimpactObject (GameObject newObject)
	{
		impactObject = newObject;
	}
}
