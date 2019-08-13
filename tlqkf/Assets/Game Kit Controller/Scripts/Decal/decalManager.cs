using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class decalManager : MonoBehaviour
{
	public bool fadeDecals;
	public float fadeSpeed;
	public static Transform decalParent;
	public static bool fadeDecalsValue;
	public static float fadeSpeedValue;
	public List<impactInfo> impactListInfo = new List<impactInfo> ();
	public static List<impactInfo> impactListInfoValue = new List<impactInfo> ();
	public GameObject projectileImpactSoundPrefab;
	public static GameObject projectileImpactSoundPrefabObject;

	void Start ()
	{
		if (!decalParent) {
			decalParent = new GameObject ().transform;
			decalParent.name = "Decal Parent";
		}
		fadeDecalsValue = fadeDecals;
		fadeSpeedValue = fadeSpeed;
		impactListInfoValue = impactListInfo;
		projectileImpactSoundPrefabObject = projectileImpactSoundPrefab;
	}

	public static void setScorch (Quaternion rotation, GameObject scorch, RaycastHit hit, GameObject collision)
	{
		//set the position of the scorch according to the hit point
		if (!collision.GetComponent<characterDamageReceiver> ()) {
			GameObject newScorch = Instantiate (scorch);
			newScorch.transform.rotation = rotation;
			newScorch.transform.position = hit.point + hit.normal * 0.03f;
			//get the surface normal to rotate the scorch to that angle
			Vector3 myForward = Vector3.Cross (newScorch.transform.right, hit.normal);
			Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
			newScorch.transform.rotation = dstRot;

			setScorchParent (collision, newScorch, hit.point);

			if (fadeDecalsValue) {
				newScorch.AddComponent<fadeObject> ().activeVanish (fadeSpeedValue);
			}
		}
	}

	public static bool placeProjectileInSurface (Quaternion rotation, GameObject projectile, RaycastHit hit, GameObject objectToDamage, bool attachToLimbs)
	{
		projectile.transform.rotation = rotation;
		projectile.transform.position = hit.point + hit.normal * 0.03f;
		//get the surface normal to rotate the scorch to that angle
		Vector3 myForward = Vector3.Cross (projectile.transform.right, hit.normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
		projectile.transform.rotation = dstRot;
	
		Rigidbody currentRigidbody = objectToDamage.GetComponent<Rigidbody> ();
		if (currentRigidbody) {
			ragdollActivator currentRagdollActivator = objectToDamage.GetComponent<ragdollActivator> ();
			if (currentRagdollActivator) {
				if (attachToLimbs) {
					List<ragdollActivator.bodyPart> bones = currentRagdollActivator.getBodyPartsList ();
					float distance = Mathf.Infinity;
					int index = -1;
					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, projectile.transform.position);
						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						projectile.transform.SetParent (bones [index].transform);
					}
					return true;
				}
			} 

			characterDamageReceiver currentCharacterDamageReceiver = objectToDamage.GetComponent<characterDamageReceiver> ();
			if (currentCharacterDamageReceiver) {
				projectile.transform.SetParent (currentCharacterDamageReceiver.character.transform);
				return true;
			} 

			projectile.transform.SetParent (objectToDamage.transform);

			return true;
		} 

		vehicleDamageReceiver currentVehicleDamageReceiver = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (currentVehicleDamageReceiver) {
			projectile.transform.SetParent (currentVehicleDamageReceiver.vehicle.transform);

			return true;
		} 

		doorSystem currentDoorSystem = objectToDamage.GetComponentInParent<doorSystem> ();
		if (currentDoorSystem) {
			Transform newParent = objectToDamage.transform;
			float maxDistance = Mathf.Infinity;
			for (int n = 0; n < currentDoorSystem.doorsInfo.Count; n++) {
				float currentDistance = GKC_Utils.distance (hit.point, currentDoorSystem.doorsInfo [n].doorMesh.transform.position);
				if (currentDistance < maxDistance) {
					maxDistance = currentDistance;
					newParent = currentDoorSystem.doorsInfo [n].doorMesh.transform;
				}
			}
			projectile.transform.SetParent (newParent);

			return true;
		} 

		projectile.transform.SetParent (decalParent);

		return true;
	}

	public static bool setImpactDecal (int impactIndex, GameObject projectile, GameObject objectToDamage, float projectileRayDistance, LayerMask projectilelayer)
	{
		bool hasImpactDecal = false;
		Vector3 projectilePosition = projectile.transform.position;
		if (impactIndex == -2) {
			bool terrainFound = false;
			//get the current texture index of the terrain under the player.
			int surfaceIndex = GetMainTexture (projectilePosition, objectToDamage.GetComponent<Terrain> ());
			for (int i = 0; i < impactListInfoValue.Count; i++) {
				if (!terrainFound && impactListInfoValue [i].checkTerrain) {
					if (impactListInfoValue [i].terrainTextureIndex == surfaceIndex) {
						impactIndex = i;
						terrainFound = true;
					}
				}
			}
		}

		if (impactIndex >= 0 && impactIndex < impactListInfoValue.Count) {
			RaycastHit hit;
			hasImpactDecal = true;
			impactInfo newImpactInfo = impactListInfoValue [impactIndex];
			if (newImpactInfo.impactSound) {
				GameObject newImpactSound = Instantiate (projectileImpactSoundPrefabObject);
				newImpactSound.transform.position = projectilePosition;
				AudioSource newImpactAudioSource = newImpactSound.GetComponent<AudioSource> ();
				GKC_Utils.checkAudioSourcePitch (newImpactAudioSource);
				newImpactAudioSource.clip = newImpactInfo.impactSound;
				newImpactAudioSource.Play ();
				newImpactSound.GetComponent<destroyGameObject> ().setTimer (newImpactInfo.impactSound.length);
				newImpactSound.name = "Impact Sound";
				newImpactSound.transform.SetParent (decalParent);
			}

			if (newImpactInfo.scorch) {
				//the bullet fired is a simple bullet or a greanade, check the hit point with a raycast to set in it a scorch
				if (Physics.Raycast (projectilePosition - projectile.transform.forward * 0.7f, projectile.transform.forward, 
					    out hit, projectileRayDistance, projectilelayer)) {
					setScorchFromList (projectile.transform.rotation, newImpactInfo.scorch, hit, objectToDamage,
						newImpactInfo.scorchScale, newImpactInfo.fadeScorch, newImpactInfo.timeToFade);
				}
			}

			if (newImpactInfo.impactParticles) {
				if (Physics.Raycast (projectilePosition - projectile.transform.forward * 0.7f, projectile.transform.forward, 
					    out hit, projectileRayDistance, projectilelayer)) {
					GameObject impactParticles = Instantiate (newImpactInfo.impactParticles);
					impactParticles.transform.position = hit.point + hit.normal * 0.03f;
					impactParticles.transform.rotation = projectile.transform.rotation;
					//get the surface normal to rotate the scorch to that angle
					Vector3 myForward = Vector3.Cross (impactParticles.transform.forward, hit.normal);
					Quaternion dstRot = Quaternion.LookRotation (hit.normal, myForward);
					impactParticles.transform.rotation = dstRot;

					Rigidbody currentRigidbody = objectToDamage.GetComponent<Rigidbody> ();
					if (currentRigidbody) {
						impactParticles.transform.SetParent (objectToDamage.transform);

						return hasImpactDecal;
					} 

					vehicleDamageReceiver currentVehicleDamageReceiver = objectToDamage.GetComponent<vehicleDamageReceiver> ();
					if (currentVehicleDamageReceiver) {
						impactParticles.transform.SetParent (currentVehicleDamageReceiver.vehicle.transform);

						return hasImpactDecal;
					} 

					impactParticles.transform.SetParent (decalParent);
				}
			}
		}

		return hasImpactDecal;
	}

	public static void setImpactSoundParent (GameObject newImpactSound)
	{
		newImpactSound.transform.SetParent (decalParent);
	}

	public static void setScorchFromList (Quaternion rotation, GameObject scorch, RaycastHit hit, GameObject collision, float scorchScale, bool fadeScorch, float timeToFade)
	{
		//set the position of the scorch according to the hit point
		GameObject newScorch = Instantiate (scorch);
		newScorch.transform.position = hit.point + hit.normal * 0.03f;
		newScorch.transform.rotation = rotation;
		//get the surface normal to rotate the scorch to that angle
		Vector3 myForward = Vector3.Cross (newScorch.transform.right, hit.normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
		newScorch.transform.rotation = dstRot;

		setScorchParent (collision, newScorch, hit.point);
	
		newScorch.transform.localScale *= scorchScale;
		if (fadeScorch) {
			newScorch.AddComponent<fadeObject> ().activeVanish (timeToFade);
		}
	}

	public static int checkIfHasDecalImpact (GameObject objectToCheck)
	{
		decalTypeInformation currentDecalTypeInformation = objectToCheck.GetComponent<decalTypeInformation> ();
		if (currentDecalTypeInformation) {
			return currentDecalTypeInformation.getDecalImpactIndex ();
		} 

		characterDamageReceiver currentCharacterDamageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (currentCharacterDamageReceiver) {
			return currentCharacterDamageReceiver.getDecalImpactIndex ();
		}

		vehicleDamageReceiver currentVehicleDamageReceiver = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (currentVehicleDamageReceiver) {
			return currentVehicleDamageReceiver.getDecalImpactIndex ();
		} 

		if (objectToCheck.GetComponent<Terrain> ()) {
			return -2;
		} 
		return -1;
	}

	public static void setScorchParent (GameObject objectDetected, GameObject newScorch, Vector3 impactPosition)
	{
		Rigidbody currentRigidbody = objectDetected.GetComponent<Rigidbody> ();
		if (currentRigidbody) {
			newScorch.transform.SetParent (objectDetected.transform);

			return;
		} 

		vehicleDamageReceiver currentVehicleDamageReceiver = objectDetected.GetComponent<vehicleDamageReceiver> ();
		if (currentVehicleDamageReceiver) {
			newScorch.transform.SetParent (currentVehicleDamageReceiver.vehicle.transform);

			return;
		} 

		doorSystem currentDoorSystem = objectDetected.GetComponentInParent<doorSystem> ();
		if (currentDoorSystem) {
			Transform newParent = objectDetected.transform;
			float maxDistance = Mathf.Infinity;
			for (int n = 0; n < currentDoorSystem.doorsInfo.Count; n++) {
				float currentDistance = GKC_Utils.distance (impactPosition, currentDoorSystem.doorsInfo [n].doorMesh.transform.position);
				if (currentDistance < maxDistance) {
					maxDistance = currentDistance;
					newParent = currentDoorSystem.doorsInfo [n].doorMesh.transform;
				}
			}
			newScorch.transform.SetParent (newParent);

			return;

		} 

		decalTypeInformation currentdecalTypeInformation = objectDetected.GetComponent<decalTypeInformation> ();
		if (currentdecalTypeInformation) {
			if (currentdecalTypeInformation.isParentScorchOnThisObjectEnabled ()) {
				newScorch.transform.SetParent (objectDetected.transform);

				return;
			} 
		}

		newScorch.transform.SetParent (decalParent);
	}

	public static int GetMainTexture (Vector3 playerPos, Terrain terrain)
	{
		//get the index of the current texture of the terrain where the player is walking
		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainPos = terrain.transform.position;

		//calculate which splat map cell the playerPos falls within
		int mapX = (int)(((playerPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
		int mapZ = (int)(((playerPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

		//get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
		float[,,] splatmapData = terrainData.GetAlphamaps (mapX, mapZ, 1, 1);

		//change the 3D array data to a 1D array:
		float[] cellMix = new float[splatmapData.GetUpperBound (2) + 1];
		for (int n = 0; n < cellMix.Length; n++) {
			cellMix [n] = splatmapData [0, 0, n];    
		}

		float maxMix = 0;
		int maxIndex = 0;
		//loop through each mix value and find the maximum
		for (int n = 0; n < cellMix.Length; n++) {
			if (cellMix [n] > maxMix) {
				maxIndex = n;
				maxMix = cellMix [n];
			}
		}
		return maxIndex;
	}

	public bool surfaceUseNoise (int surfaceIndex)
	{
		return impactListInfo [surfaceIndex].useNoise;
	}

	[System.Serializable]
	public class impactInfo
	{
		public string name;
		public string surfaceName;
		public AudioClip impactSound;
		public GameObject scorch;
		[Range (1, 5)] public float scorchScale;
		public bool fadeScorch;
		public float timeToFade;
		public GameObject impactParticles;
		public bool checkTerrain;
		public int terrainTextureIndex;
		public bool useNoise = true;
	}
}