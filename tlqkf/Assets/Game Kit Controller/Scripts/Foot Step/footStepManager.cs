using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class footStepManager : MonoBehaviour
{
	public bool soundsEnabled;
	public Vector2 feetVolumeRange = new Vector2 (0.8f, 1.2f);

	public bool useFootStepStateList;
	public string currentFootStepStateName;
	public LayerMask noiseDetectionLayer;
	public List<footStepState> footStepStateList = new List<footStepState> ();
	footStepState currentFootStepState;

	public string defaultSurfaceName = "Concrete";

	public footStepType typeOfFootStep;
	public LayerMask layer;

	public bool useFootPrints;
	public GameObject rightFootPrint;
	public GameObject leftFootPrint;
	public bool useFootPrintsFromStates;

	public bool useFootPrintMaxAmount;
	public int footPrintMaxAmount;
	public float timeToRemoveFootPrints;
	public float maxFootPrintDistance;
	public float distanceBetweenPrintsInFisrtPerson;
	public bool removeFootPrintsInTime;
	public bool vanishFootPrints;
	public float vanishSpeed;

	public bool useFootParticles;
	public GameObject footParticles;
	public bool useFootParticlesFromStates;

	public footStepsLayer[] footSteps;
	public GameObject leftFoot;
	public GameObject rightFoot;

	public bool showNoiseDetectionGizmo;

	public playerController playerManager;
	public footStep leftFootStep;
	public footStep rightFootStep;

	public Collider leftFootCollider;
	public Collider rightFootCollider;
	public AudioSource cameraAudioSource;

	int surfaceIndex;
	float lastFootstepTime = 0;
	GameObject currentSurface;
	RaycastHit hit;

	bool usingAnimator = true;

	Transform footPrintsParent;

	public enum footStepType
	{
		triggers,
		raycast
	}

	List<GameObject> footPrints = new List<GameObject> ();
	float destroyFootPrintsTimer;
	bool soundFound = false;
	int footStepIndex = -1;
	int poolSoundIndex = -1;

	noiseMeshSystem noiseMeshManager;

	Transform currentFootPrintParent;

	GameObject currentObjectBelowPlayer;
	GameObject previousObjectBelowPlayer;

	Terrain currentTerrain;
	AudioClip currentSoundEffect;

	footStepSurfaceSystem currentFootStepSurfaceSystem;
	string currentSurfaceName;


	void Start ()
	{
		if (useFootPrints || useFootParticles) {
			GameObject printsParent = GameObject.Find ("Foot Prints Parent");
			if (printsParent) {
				footPrintsParent = printsParent.transform;
			} else {
				printsParent = new GameObject ();
				printsParent.name = "Foot Prints Parent";
				footPrintsParent = printsParent.transform;
			}
		}

		if (useFootStepStateList) {
			setFootStepState (currentFootStepStateName);
		} else {
			setDefaultFeelVolumeRange ();
		}

		currentSurfaceName = defaultSurfaceName;
	}

	void Update ()
	{
		//if the player doesn't use the animator when the first person view is enabled, the footsteps in the feet of the player are disabled
		//so checkif the player is moving, and then play the steps sounds according to the stepInterval and the surface detected with a raycast under the player
		if (!usingAnimator && soundsEnabled &&
		    ((playerManager.isPlayerOnGround () && playerManager.isPlayerMoving (0)) || (currentFootStepState != null && currentFootStepState.playSoundOnce && !currentFootStepState.soundPlayed))) {
			if (Physics.Raycast (transform.position + transform.up * .1f, -transform.up, out hit, .5f, layer) || !currentFootStepState.checkPlayerOnGround) {
				//get the gameObject under the player's feet
				if (currentFootStepState.checkPlayerOnGround) {
					currentSurface = hit.collider.gameObject;
				}

				//check the footstep frequency
				if (Time.time > lastFootstepTime + currentFootStepState.stepInterval / playerManager.animSpeedMultiplier) {
					//get the audio clip according to the type of surface, mesh or terrain
					if (currentFootStepState.playCustomSound) {
						currentSoundEffect = currentFootStepState.customSound;
					} else {
						if (currentSurface) {
							currentSoundEffect = getSound (transform.position, currentSurface, footStep.footType.center);
						}
					}

					if (currentSoundEffect) {
						//play one shot of the audio
						cameraAudioSource.PlayOneShot (currentSoundEffect, Random.Range (0.8f, 1.2f));
						lastFootstepTime = Time.time;
					}
						
					if (currentFootStepState.playSoundOnce) {
						currentFootStepState.soundPlayed = true;
					}
				}
			}
		}

		if (useFootPrints && removeFootPrintsInTime) {
			if (footPrints.Count > 0) {
				destroyFootPrintsTimer += Time.deltaTime;
				if (destroyFootPrintsTimer > timeToRemoveFootPrints) {
					for (int i = 0; i < footPrints.Count; i++) {
						if (footPrints [i]) {
							Destroy (footPrints [i]);
						}
					}
					footPrints.Clear ();
					destroyFootPrintsTimer = 0;
				}
			}
		}
	}

	public void changeFootStespType (bool state)
	{
		if (typeOfFootStep == footStepType.raycast) {
			state = false;
		}
		leftFoot.SetActive (state);
		rightFoot.SetActive (state);
		usingAnimator = state;
	}

	public int GetMainTexture (Vector3 playerPos, Terrain terrain)
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

		//print (terrainData.splatPrototypes [maxIndex].texture.name + " " + maxIndex);
		return maxIndex;
	}

	//get the audio clip, according to the layer of the object under the player, the position of the player, and the ground itself
	public AudioClip getSound (Vector3 pos, GameObject ground, footStep.footType footSide)
	{
		soundFound = false;
		footStepIndex = -1;
		poolSoundIndex = -1;

		currentObjectBelowPlayer = ground;
		if (previousObjectBelowPlayer != currentObjectBelowPlayer) {
			previousObjectBelowPlayer = currentObjectBelowPlayer;
			currentTerrain = ground.GetComponent<Terrain> ();

			currentFootStepSurfaceSystem = currentObjectBelowPlayer.GetComponent<footStepSurfaceSystem> ();

			if (currentFootStepSurfaceSystem) {
				currentSurfaceName = currentFootStepSurfaceSystem.getSurfaceName ();
			} else {
				currentSurfaceName = defaultSurfaceName;
			}
		}

		//if the player is in a terrain
		if (currentTerrain) {
			//get the current texture index of the terrain under the player.
			surfaceIndex = GetMainTexture (pos, currentTerrain);
			for (int i = 0; i < footSteps.Length; i++) {
				//check if that terrain texture has a sound
				if (footSteps [i].checkTerrain && surfaceIndex == footSteps [i].terrainTextureIndex) {
					int index = -1;
					if (footSteps [i].randomPool) {
						//get a random sound
						index = randomStep (footSteps [i].poolSounds);
					} else {
						//get the next sound in the list
						footSteps [i].poolIndex++;
						if (footSteps [i].poolIndex > footSteps [i].poolSounds.Length - 1) {
							footSteps [i].poolIndex = 0;
						}
						index = footSteps [i].poolIndex;
					}

					soundFound = true;
					footStepIndex = i;
					poolSoundIndex = index;
				}
			}
		} 

		//else, the player is above a mesh
		else {
			surfaceIndex = -1;
			for (int i = 0; i < footSteps.Length; i++) {
				//check if the layer of the mesh has a sound 
				if (footSteps [i].checkSurfaceSystem && currentSurfaceName == footSteps [i].Name) {
					int index = -1;
					if (footSteps [i].randomPool) {
						//get a random sound
						index = randomStep (footSteps [i].poolSounds);
					} else {
						//get the next sound in the list
						footSteps [i].poolIndex++;
						if (footSteps [i].poolIndex > footSteps [i].poolSounds.Length - 1) {
							footSteps [i].poolIndex = 0;
						}
						index = footSteps [i].poolIndex;
					}
					soundFound = true;
					footStepIndex = i;
					poolSoundIndex = index;
				}
			}
		}

		if (soundFound) {
			placeFootPrint (footSide, footStepIndex);

			createParticles (footSide, footStepIndex);

			setNoiseOnStep (pos);
			//return the audio selected
			return footSteps [footStepIndex].poolSounds [poolSoundIndex];
		}

		return null;
	}

	//get a random index of the pool of sounds
	int randomStep (AudioClip[] pool)
	{
		int random = Random.Range (0, pool.Length);
		return random;
	}

	bool usingCenterSideToLeft;

	public void placeFootPrint (footStep.footType footSide, int footStepIndex)
	{
		if (useFootPrints) {
			Vector3 footPrintPosition = Vector3.zero;
			bool isLeftFoot = false;
			if (footSide == footStep.footType.left) {
				footPrintPosition = leftFoot.transform.position;
				isLeftFoot = true;
			} else if (footSide == footStep.footType.right) {
				footPrintPosition = rightFoot.transform.position;
			} else {
				usingCenterSideToLeft = !usingCenterSideToLeft;
				if (usingCenterSideToLeft) {
					footPrintPosition = transform.position - transform.right * distanceBetweenPrintsInFisrtPerson;
					isLeftFoot = true;
				} else {
					footPrintPosition = transform.position + transform.right * distanceBetweenPrintsInFisrtPerson;
				}
			}

			if (Physics.Raycast (footPrintPosition, -transform.up, out hit, 5, layer)) {
				if (hit.distance < maxFootPrintDistance) {
					Vector3 placePosition = hit.point + transform.up * 0.013f;

					if (isLeftFoot) {
						GameObject currentFootPrint = leftFootPrint;
						if (useFootPrintsFromStates) {
							if (footSteps [footStepIndex].useFootPrints) {
								currentFootPrint = footSteps [footStepIndex].leftFootPrint;
							}
						}

						if (currentFootPrint != null) {
							createFootPrint (currentFootPrint, placePosition, leftFoot.transform.rotation, hit.normal, hit.collider.gameObject);
						}
					} else {
						GameObject currentFootPrint = rightFootPrint;
						if (useFootPrintsFromStates) {
							if (footSteps [footStepIndex].useFootPrints) {
								currentFootPrint = footSteps [footStepIndex].rightFootPrint;
							}
						}

						if (currentFootPrint != null) {
							createFootPrint (currentFootPrint, placePosition, rightFoot.transform.rotation, hit.normal, hit.collider.gameObject);
						}
					}
				}
			}
		}
	}

	public void createFootPrint (GameObject foot, Vector3 position, Quaternion rotation, Vector3 normal, GameObject surfaceGameObject)
	{
		GameObject newFootPrint = (GameObject)Instantiate (foot, position, rotation);

		parentAssignedSystem currentParentAssgined = surfaceGameObject.GetComponent<parentAssignedSystem> ();

		currentFootPrintParent = footPrintsParent.transform;
		if (currentParentAssgined) {
			currentFootPrintParent = currentParentAssgined.getAssignedParent ().transform;
		} 

		if (currentFootPrintParent) {
			newFootPrint.transform.SetParent (currentFootPrintParent);
		}

		Vector3 myForward = Vector3.Cross (newFootPrint.transform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);
		newFootPrint.transform.rotation = dstRot;
		footPrints.Add (newFootPrint);
		if (vanishFootPrints) {
			newFootPrint.AddComponent<fadeObject> ().activeVanish (vanishSpeed);
		}

		if (useFootPrintMaxAmount && footPrintMaxAmount > 0 && footPrints.Count > footPrintMaxAmount) {
			GameObject footPrintToRemove = footPrints [0];
			footPrints.RemoveAt (0);
			Destroy (footPrintToRemove);
		}
	}

	public void createParticles (footStep.footType footSide, int footStepIndex)
	{
		if (useFootParticles) {
			Vector3 footPrintPosition = Vector3.zero;
			if (footSide == footStep.footType.left) {
				footPrintPosition = leftFoot.transform.position;
			} else {
				footPrintPosition = rightFoot.transform.position;
			}

			GameObject currentFootParticles = footParticles;
			if (useFootParticlesFromStates) {
				if (footSteps [footStepIndex].useFootParticles) {
					currentFootParticles = footSteps [footStepIndex].footParticles;
				}
			}

			if (currentFootParticles != null) {
				GameObject newFootParticle = (GameObject)Instantiate (currentFootParticles, footPrintPosition, transform.rotation);
				newFootParticle.transform.SetParent (footPrintsParent.transform);
			}
		}
	}

	public void enableOrDisableFootSteps (bool state)
	{
		leftFootCollider.enabled = state;
		rightFootCollider.enabled = state;
	}

	public void enableOrDisableFootStepsWithDelay (bool state, float delayAmount)
	{
		if (leftFootStep != null && leftFootStep.gameObject.activeSelf) {
			leftFootStep.setFooStepStateWithDelay (state, delayAmount);
		}
		if (rightFootStep != null && rightFootStep.gameObject.activeSelf) {
			rightFootStep.setFooStepStateWithDelay (state, delayAmount);
		}
	}

	public void enableOrDisableFootStepsComponents (bool state)
	{
		if (leftFootStep != null) {
			leftFootStep.enableOrDisableFootStep (state);
		}
		if (rightFootStep != null) {
			rightFootStep.enableOrDisableFootStep (state);
		}
	}

	public void setRightFootStep (GameObject rigth)
	{
		rightFoot = rigth;
	}

	public void setLeftFootStep (GameObject left)
	{
		leftFoot = left;
	}

	public void setFootStepState (string stateName)
	{
		if (!useFootStepStateList) {
			return;
		}

		if (currentFootStepStateName == stateName) {
			return;
		}

		bool stateFound = false;
		for (int i = 0; i < footStepStateList.Count; i++) {
			if (footStepStateList [i].Name == stateName && footStepStateList [i].stateEnabled) {
				currentFootStepState = footStepStateList [i];
				currentFootStepStateName = currentFootStepState.Name;
				leftFootStep.setStepVolumeRange (currentFootStepState.feetVolumeRange);
				rightFootStep.setStepVolumeRange (currentFootStepState.feetVolumeRange);
				footStepStateList [i].isCurrentState = true;

				footStepStateList [i].soundPlayed = false;

				stateFound = true;
				if (currentFootStepState.setNewStateAfterDelay) {
					setNewFootStepStateAfterDelay (currentFootStepState.newStateName, currentFootStepState.newStateDelay);
				} else {
					stopSetNewFootStepStateAfterDelayCoroutine ();
				}
			} else {
				footStepStateList [i].isCurrentState = false;
			}
		}

		if (!stateFound) {
			setDefaultFeelVolumeRange ();
		}
	}

	bool delayToNewStateActive;

	Coroutine setNewStateAfterDelayCoroutine;

	public void setNewFootStepStateAfterDelay (string newState, float delayAmount)
	{
		stopSetNewFootStepStateAfterDelayCoroutine ();
		setNewStateAfterDelayCoroutine = StartCoroutine (setNewFootStepStateAfterDelayCoroutine (newState, delayAmount));
	}

	public void stopSetNewFootStepStateAfterDelayCoroutine ()
	{
		if (setNewStateAfterDelayCoroutine != null) {
			StopCoroutine (setNewStateAfterDelayCoroutine);
		}
	}

	IEnumerator setNewFootStepStateAfterDelayCoroutine (string newState, float delayAmount)
	{
		delayToNewStateActive = true;
		yield return new WaitForSeconds (delayAmount);

		setFootStepState (newState);
		delayToNewStateActive = false;
	}

	public bool isDelayToNewStateActive ()
	{
		return delayToNewStateActive;
	}

	public void setNoiseOnStep (Vector3 footPosition)
	{
		if (currentFootStepState != null && useFootStepStateList && currentFootStepState.stateEnabled && currentFootStepState.useNoise) {
			if (currentFootStepState.useNoiseDetection) {
				applyDamage.sendNoiseSignal (currentFootStepState.noiseRadius, footPosition, noiseDetectionLayer, showNoiseDetectionGizmo);
			}

			if (!noiseMeshManager) {
				noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();
			}

			if (noiseMeshManager) {
				noiseMeshManager.addNoiseMesh (currentFootStepState.noiseRadius, footPosition + Vector3.up, currentFootStepState.noiseExpandSpeed);
			}
		}
	}

	public void setDefaultFeelVolumeRange ()
	{
		rightFootStep.setStepVolumeRange (feetVolumeRange);
		leftFootStep.setStepVolumeRange (feetVolumeRange);
	}

	public void removeSound (int footStepIndex, int soundIndex)
	{
		//footSteps [footStepIndex].poolSounds.remove
		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<footStepManager> ());
		#endif
	}

	//class to create every type of surface
	//selecting layerName and checkLayer to set that type of step in a mesh
	//if the current step is for a terrain, then set a terrainTextureName, checkTerrain and terrainTextureIndex according to the order in the terrain textures
	//set to true randomPool to play the sounds in a random order, else the sounds are played in the same order
	[System.Serializable]
	public class footStepsLayer
	{
		public string Name;
		public AudioClip[] poolSounds;
		public bool checkSurfaceSystem = true;
		public string terrainTextureName;
		public bool checkTerrain;
		public int terrainTextureIndex;
		public bool randomPool;
		[HideInInspector] public int poolIndex;

		public bool useFootPrints;
		public GameObject rightFootPrint;
		public GameObject leftFootPrint;

		public bool useFootParticles;
		public GameObject footParticles;
	}

	[System.Serializable]
	public class footStepState
	{
		public string Name;
		public bool stateEnabled = true;
		public float stepInterval = 0.3f;
		public Vector2 feetVolumeRange = new Vector2 (0.8f, 1.2f);

		public bool playSoundOnce;
		public bool soundPlayed;
		public bool checkPlayerOnGround = true;

		public bool isCurrentState;
		public bool useNoise;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public float noiseRadius;
		public bool setNewStateAfterDelay;
		public float newStateDelay;
		public string newStateName;

		public bool playCustomSound;
		public AudioClip customSound;
	}
}