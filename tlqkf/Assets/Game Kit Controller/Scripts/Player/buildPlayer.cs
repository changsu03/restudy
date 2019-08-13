using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class buildPlayer : MonoBehaviour
{
	public GameObject trail;
	public GameObject hitCombat;
	public GameObject shootZone;
	public GameObject arrow;
	public GameObject player;
	public GameObject jetPack;
	public GameObject currentCharacterModel;

	public GameObject AIWeapons;
	public LayerMask layerToPlaceNPC;

	public bool buildPlayerType;
	public bool hasWeaponsEnabled;

	public Transform playerElementsTransform;

	public bool assignBonesManually;

	public Transform head;
	public Transform neck;
	public Transform chest;
	public Transform spine;

	public Transform hips;

	public Transform rightLowerArm;
	public Transform leftLowerArm;
	public Transform rightHand;
	public Transform leftHand;

	public Transform rightLowerLeg;
	public Transform leftLowerLeg;
	public Transform rightFoot;
	public Transform leftFoot;
	public Transform rightToes;
	public Transform leftToes;

	public GameObject newCharacterModel;

	Transform playerCOM;
	Animator anim;
	Vector3 IKHandPos;
	upperBodyRotationSystem upperBodyRotationManager;
	mapSystem mapSystemManager;
	GameObject playerCameraGameObject;
	playerController playerControllerManager;
	ragdollBuilder ragdollBuilderManager;
	gravitySystem gravityManager;
	headTrack headTrackManager;
	IKSystem IKManager;
	playerWeaponsManager weaponsManager;
	ragdollActivator ragdollActivatorManager;
	IKFootSystem IKFootManager;

	bool isInstantiated;

	public void buildCharacterByButton ()
	{
		buildCharacter ();
	}

	public void setCharacterVariables (bool characterIsPlayerType, bool hasWeapons, bool characterIsInstantiated, GameObject newModel)
	{
		buildPlayerType = characterIsPlayerType;
		hasWeaponsEnabled = hasWeapons;
		isInstantiated = characterIsInstantiated;

		newCharacterModel = newModel;
	}

	public void getCharacterBones ()
	{
		if (!newCharacterModel) {
			print ("WARNING: There is not new character model to use, assign it on the inspector or select a valid model on the character creator window");
			return;
		}

		anim = newCharacterModel.GetComponent<Animator> ();
		head = anim.GetBoneTransform (HumanBodyBones.Head);
		neck = anim.GetBoneTransform (HumanBodyBones.Neck);
		if (neck == null) {
			if (head != null) {
				neck = head.parent;
			} else {
				print ("WARNING: no head found, assign it manually to make sure all of them are configured correctly");
			}
		}	

		chest = anim.GetBoneTransform (HumanBodyBones.Chest);
		spine = anim.GetBoneTransform (HumanBodyBones.Spine);
	
		if (spine) {
			if (chest) {
				if (spine != chest.parent) {
					spine = chest.parent;
				}
			} else {
				print ("WARNING: no chest found, assign it manually to make sure all of them are configured correctly");
			}
		} else {
			print ("WARNING: no spine found, assign it manually to make sure all of them are configured correctly");
		}

		hips = anim.GetBoneTransform (HumanBodyBones.Hips);

		rightLowerArm = anim.GetBoneTransform (HumanBodyBones.RightLowerArm);
		leftLowerArm = anim.GetBoneTransform (HumanBodyBones.LeftLowerArm);
		rightHand = anim.GetBoneTransform (HumanBodyBones.RightHand);
		leftHand = anim.GetBoneTransform (HumanBodyBones.LeftHand);

		rightLowerLeg = anim.GetBoneTransform (HumanBodyBones.RightLowerLeg);
		leftLowerLeg = anim.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
		rightFoot = anim.GetBoneTransform (HumanBodyBones.RightFoot);
		leftFoot = anim.GetBoneTransform (HumanBodyBones.LeftFoot);
		rightToes = anim.GetBoneTransform (HumanBodyBones.RightToes);
		leftToes = anim.GetBoneTransform (HumanBodyBones.LeftToes);
	}

	public bool checkAllBonesFound ()
	{
		return 
			(head != null) &&
		(neck != null) &&
		(chest != null) &&
		(spine != null) &&
		(hips != null) &&
		(rightLowerArm != null) &&
		(leftLowerArm != null) &&
		(rightHand != null) &&
		(leftHand != null) &&
		(rightLowerLeg != null) &&
		(leftLowerLeg != null) &&
		(rightFoot != null) &&
		(leftFoot != null) &&
		(rightToes != null) &&
		(leftToes != null);
	}

	//set all the objects inside the character's body
	public void buildCharacter ()
	{
		//it only works in the editor mode, checking the game is not running
		if (!Application.isPlaying) {
			if (!assignBonesManually) {
				getCharacterBones ();
			}

			if (!checkAllBonesFound ()) {
				print ("WARNING: not all bones necessary for the new player has been found, assign them manually to make sure all of them are configured correctly");
				return;
			}

			if (assignBonesManually) {
				newCharacterModel.transform.position = currentCharacterModel.transform.position;
				newCharacterModel.transform.rotation = currentCharacterModel.transform.rotation;
			}

			upperBodyRotationManager = player.GetComponent<upperBodyRotationSystem> ();
			mapSystemManager = GetComponent<mapSystem> ();
			playerControllerManager = player.GetComponent<playerController> ();
			gravityManager = player.GetComponent<gravitySystem> ();
			headTrackManager = player.GetComponent<headTrack> ();
			playerCameraGameObject = playerControllerManager.getPlayerCameraGameObject ();
			weaponsManager = player.GetComponent<playerWeaponsManager> ();
			ragdollActivatorManager = player.GetComponent<ragdollActivator> ();
			IKFootManager = player.GetComponent<IKFootSystem> ();

			IKManager = player.GetComponent<IKSystem> ();
			playerCOM = IKManager.getIKBodyCOM ();

			currentCharacterModel.transform.SetParent (null);

			newCharacterModel.transform.SetParent (playerCOM);
				
			//get and set the animator and avatar of the model
			player.GetComponent<Animator> ().avatar = anim.avatar;

			playerElementsTransform.SetParent (chest);

			//create a list of the needed bones, to set every object inside of everyone, in this case for the trailes
			Transform[] trailsPositions = new Transform[] {
				leftFoot,
				rightFoot,
				leftLowerLeg,
				rightLowerLeg,
				leftHand,
				rightHand,
				leftLowerArm,
				rightLowerArm,
				spine,
				head
			};
					
			for (int i = 0; i < trailsPositions.Length; i++) {
				GameObject trailClone = (GameObject)Instantiate (trail, Vector3.zero, Quaternion.identity);
				//remove the clone string inside the instantiated object
				trailClone.name = trailClone.name.Replace ("(Clone)", "");
				trailClone.transform.SetParent (trailsPositions [i]);
				trailClone.transform.localPosition = Vector3.zero;
				//trailClone.transform.localRotation = Quaternion.identity;
				trailClone.GetComponent<TrailRenderer> ().enabled = false;
			}

			//create the shoot zone in the right hand of the player
			shootZone.transform.SetParent (rightHand);
			shootZone.transform.localPosition = Vector3.zero;
			shootZone.transform.localRotation = Quaternion.identity;

			closeCombatSystem combatManager = player.GetComponent<closeCombatSystem> ();
			combatManager.getCombatPrefabs (hitCombat);
			combatManager.assignBasicCombatTriggers ();

			setFootStep ();

			//set the arrow in the back of the player
			arrow.transform.SetParent (neck);
			arrow.transform.position = player.transform.position + player.transform.up * 1.6f - player.transform.forward * 0.3f;
			arrow.transform.rotation = player.transform.rotation;

			//set the part of every arm in the otherpowers script
			setPowerSettings ();

			//get every part in the head of the player, to set their layer in ignore raycast
			//this is for the ragdoll to mecanim system, to avoid that the face of the player deforms in the transition from ragdoll to mecnanim
			Component[] components = head.GetComponentsInChildren (typeof(Transform));
			foreach (Component c in components) {
				if (c != head) {
					c.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
				}
			}

			//set the animator in the ragdill builder component
			ragdollBuilderManager = GetComponent<ragdollBuilder> ();
			if (ragdollBuilderManager) {
				ragdollBuilderManager.getAnimator (anim);
				ragdollBuilderManager.createRagdoll ();
			}


			ragdollActivatorManager.setCharacterBody (newCharacterModel);

			if (IKFootManager) {
				IKFootManager.setLegsInfo (hips, rightLowerLeg, leftLowerLeg, rightFoot, leftFoot, rightToes, leftToes);
			}

			setWeapons ();

			setJetpack (neck);

			setIKUpperBodyComponents ();

			setMapSystemComponents ();

			setHealthWeakSpots ();

			setHeadTrackInfo ();

			setPlayerControllerComponents ();

			DestroyImmediate (currentCharacterModel);

			currentCharacterModel = newCharacterModel;

			if (isInstantiated) {
				placeCharacterInCameraPosition ();
			}

			updateComponent ();
		}
	}

	void setPlayerControllerComponents ()
	{
		SkinnedMeshRenderer currentSkinnedMeshRenderer = playerCOM.GetComponentInChildren<SkinnedMeshRenderer> ();
		if (currentSkinnedMeshRenderer) {
			playerControllerManager.setCharacterMeshGameObject (currentSkinnedMeshRenderer.gameObject);
		}
	}

	void setFootStep ()
	{
		footStepManager currentFootStepManager = player.GetComponent<footStepManager> ();
		Transform rightFoot = currentFootStepManager.rightFoot.transform;
		Transform leftFoot = currentFootStepManager.leftFoot.transform;
		Vector3 rightFootPosition = rightFoot.localPosition;
		Vector3 leftFootPosition = leftFoot.localPosition;

		rightFoot.SetParent (rightToes);
		leftFoot.SetParent (leftToes);

		rightFoot.localPosition = rightFootPosition;
		leftFoot.localPosition = leftFootPosition;
	}

	void setPowerSettings ()
	{
		otherPowers powers = player.GetComponent<otherPowers> ();
		powers.aimsettings.leftHand = leftHand.gameObject;
		powers.aimsettings.rightHand = rightHand.gameObject;

		SkinnedMeshRenderer currentSkinnedMeshRenderer = playerCOM.GetComponentInChildren<SkinnedMeshRenderer> ();

		int charactersMaterials = currentSkinnedMeshRenderer.sharedMaterials.Length;
		gravityManager.materialToChange = new List<bool> (charactersMaterials);
		for (int i = 0; i < gravityManager.materialToChange.Count; i++) {
			gravityManager.materialToChange [i] = true;
		}

		powers.setMeshCharacter (currentSkinnedMeshRenderer);

		gravityManager.setMeshCharacter (currentSkinnedMeshRenderer);
	}

	void setJetpack (Transform parent)
	{
		if (jetPack) {
			jetPack.transform.SetParent (parent);
		}
	}

	void setIKUpperBodyComponents ()
	{
		upperBodyRotationManager.spineTransform = spine;
		upperBodyRotationManager.chestTransform = chest;

		Vector3 newChestUpVector = chest.up.normalized;

		newChestUpVector = new Vector3 (Mathf.Round (newChestUpVector.x), Mathf.Round (newChestUpVector.y), Mathf.Round (newChestUpVector.z));

		upperBodyRotationManager.setNewChestUpVectorValue (newChestUpVector);
	}

	void setMapSystemComponents ()
	{
		if (mapSystemManager) {
			mapSystemManager.searchBuildingList ();
		}
	}

	void setHealthWeakSpots ()
	{
		if (!buildPlayerType) {
			player.GetComponent<health> ().setHumanoidWeaKSpots ();
		}
	}

	void setHeadTrackInfo ()
	{
		if (headTrackManager) {
			headTrackManager.setHeadTransform (head);
		}
	}

	void setWeapons ()
	{
		weaponsManager.setThirdPersonParent (playerElementsTransform);
		weaponsManager.setRightHandTransform (rightHand);
		weaponsManager.setLeftHandTransform (leftHand);
		weaponsManager.setWeaponList ();
		if (!hasWeaponsEnabled) {
			weaponsManager.enableOrDisableWeaponsList (false);
		}
	}

	void placeCharacterInCameraPosition ()
	{
		#if UNITY_EDITOR
		if (SceneView.lastActiveSceneView) {
			if (SceneView.lastActiveSceneView.camera) {
				Camera currentCameraEditor = SceneView.lastActiveSceneView.camera;
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;
				RaycastHit hit;
				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceNPC)) {
					if (!buildPlayerType) {
						transform.position = hit.point;
					} else {
						player.transform.position = hit.point;
						playerCameraGameObject.transform.position = player.transform.position;
					}
				}
			}
		}
		#endif
	}

	void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<buildPlayer> ());
		#endif
	}

}