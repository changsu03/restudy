using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class closeCombatSystem : MonoBehaviour
{
	public bool combatSystemEnabled;
	public LayerMask layerMaskToDamage;
	//public float timeToDisableTriggers;
	public bool controlledByAI;
	public bool alwaysKicks;
	public bool alwaysPunchs;
	public bool makeFullCombos;
	public bool alternateKickAndPunchs;
	public bool randomAttack;
	public float minTimeBetweenAttacks;
	public float defaultArmHitDamage;
	public float defaultLegHitDamage;
	public float addForceMultiplier;
	public List<combatLimbInfo> combatLimbList = new List<combatLimbInfo> ();
	public bool currentPlayerMode;

	public enum colliderPlace
	{
		leg,
		arm,
		both
	}

	public int combatLayerIndex = 3;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public menuPause pauseManager;
	public gravitySystem gravityManager;
	public playerWeaponsManager weaponsManager;
	public playerController playerControllerManager;
	public otherPowers powersManager;
	public Animator animator;

	public string punchIDName = "Punch ID";
	public string kickIDName = "Kick ID";

	public GameObject hitCombatPrefab;
	public bool showGizmo;
	public Color gizmoColor;
	public float gizmoRadius;
	public Color gizmoLabelColor;

	float timerCombat = 0;
	int npunch = 0;
	int nkick = 0;
	float trailTimer;
	int i;
	bool fighting;
	float currentAnimLenght;

	bool lastAttackKick;
	bool lastAttackPunch;
	float lastTimeAttack;

	bool combatCanBeUsed;

	//this is a simple close combat system, I want to improve it in future updates, but for now, it worsk fine
	//just set your combat animatios in the combat layer of the animator
	//you can make combos of kick and punch, joining two kicks and a punch for example

	void Update ()
	{
		if (currentPlayerMode) {
			combatCanBeUsed = canUseCombat ();
		} else {
			combatCanBeUsed = false;
		}

		if (currentPlayerMode || fighting) {
			//get the current state of the animator in the combat layer
			AnimatorClipInfo[] ainfo = animator.GetCurrentAnimatorClipInfo (combatLayerIndex);
			if (ainfo.Length != 0) {
				//get the current animation info
				for (int idx = 0; idx < ainfo.Length; idx++) {
					currentAnimLenght = ainfo [idx].clip.length;
				}
				//enable the fight mode
				if (!fighting) {
					fighting = true;
				}
			} else {
				//disable the fight mode
				if (fighting) {
					fighting = false;
					disableCombat ();
				}
			}
		}

		//if the timer of the current animation is over, end the combat mode
		if (timerCombat > 0) {
			trailTimer = 0;
			timerCombat -= Time.deltaTime;
			if (timerCombat < 0) {
				disableCombat ();
			}
		}

		//configurate the trails renderer in the hands and foot of the player to disable smoothly
		if (trailTimer > 0 && timerCombat == 0) {
			trailTimer -= Time.deltaTime;
			if (trailTimer < 0) {
				trailTimer = 0;
				//set the state of the triggers in the hands and foot of the player
				changeCollidersState (false, colliderPlace.both, true);
			}
			for (i = 0; i < combatLimbList.Count; i++) {
				if (combatLimbList [i].trail) {
					if (combatLimbList [i].trail.time > 0) {
						combatLimbList [i].trail.time -= Time.deltaTime;
					}
				}
			}
		}
	}

	//the player has pressed the punch button
	public void punch ()
	{
		//check the state of the player
		if (combatCanBeUsed) {
			//the current combat has only a combo of three punchs
			if (npunch < 3) {
				nkick = 0;
				npunch++;
				//in the first punch, set the timer to a value, because the combat layer of the animator it cannot be check yet
				if (npunch == 1) {
					timerCombat = 0.33f;
					changeCollidersState (true, colliderPlace.arm, true);
				}
				//else, add to the timer the time of the animation
				else {
					timerCombat += currentAnimLenght;
				}

				//set the parameters of the animator
				animator.SetInteger (punchIDName, npunch);
				animator.SetInteger (kickIDName, nkick);
			}
		}
	}

	//the player has pressed the kick button
	public void kick ()
	{
		//check the state of the player
		if (combatCanBeUsed) {
			//the current combat has only a combo of four kicks
			if (nkick < 4) {
				npunch = 0;
				nkick++;
				//if the first kick, set the timer to a value, because the combat layer of the animator it cannot be check yet 
				if (nkick == 1) {
					timerCombat = 0.96f;
					changeCollidersState (true, colliderPlace.leg, true);
				}
				//else, add to the timer the time of the animation
				else {
					timerCombat += currentAnimLenght;
				}
				//set the parameters of the animator
				animator.SetInteger (punchIDName, npunch);
				animator.SetInteger (kickIDName, nkick);
			}
		}
	}

	bool canUseCombat ()
	{
		if (currentPlayerMode) { 
			if (!playerControllerManager.isGravityPowerActive () && playerControllerManager.isPlayerOnGround () && !powersManager.usingPowers && !weaponsManager.isUsingWeapons () &&
			    !playerControllerManager.usingDevice && combatSystemEnabled && !gravityManager.dead &&
			    (!pauseManager || !pauseManager.playerMenuActive)) {
				return true;
			} 
		}
		return false;
	}

	public void setCurrentPlayerMode (bool state)
	{
		currentPlayerMode = state;

		checkEventsOnStateChange (currentPlayerMode);
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}

	public bool isCurrentPlayerMode ()
	{
		return currentPlayerMode;
	}

	//the combo has finished, so disable the combat mode
	void disableCombat ()
	{
		npunch = 0;
		nkick = 0;
		timerCombat = 0;
		animator.SetInteger (punchIDName, npunch);
		animator.SetInteger (kickIDName, nkick);
		changeCollidersState (false, colliderPlace.both, false);
		trailTimer = 1;
	}

	//disable or enable the triggers in the hands and foot of the player, to damage the enemy when they touch it
	void changeCollidersState (bool state, colliderPlace place, bool trail)
	{
		//check what colliders have to be activated or deactivated, the hands or the foot, to damage the enemy with 
		//the correct triggers according to the type of combo, kicks or punchs
		for (i = 0; i < combatLimbList.Count; i++) {
			if (place == combatLimbList [i].limbType || place == colliderPlace.both) {
				combatLimbList [i].hitCombatManager.setCurrentState (state);
				if (trail) {
					if (combatLimbList [i].trail) {
						combatLimbList [i].trail.enabled = state;
						combatLimbList [i].trail.time = 1;
					}
				}
			}
		}
	}

	public void enableOrDisableTriggers (bool value)
	{
		for (i = 0; i < combatLimbList.Count; i++) {
			combatLimbList [i].trigger.enabled = value;
		}
	}

	public void addCombatPlace (string name, float hitDamage, GameObject limb, colliderPlace limbType, TrailRenderer trail)
	{
		combatLimbInfo newLimb = new combatLimbInfo ();
		newLimb.name = name;
		newLimb.hitDamage = hitDamage;
		newLimb.limb = limb;
		newLimb.limbType = limbType;
		newLimb.trail = trail;
		newLimb.hitCombatManager = limb.GetComponent<hitCombat> ();
		newLimb.trigger = limb.GetComponent<Collider> ();
		combatLimbList.Add (newLimb);
	}

	public void assignBasicCombatTriggers ()
	{
		if (hitCombatPrefab) {
			animator = transform.GetChild (0).GetComponentInChildren<Animator> ();
			if (animator) {
				combatLimbList.Clear ();
				//another list of bones, to the triggers in hands and feet for the combat
				Transform[] hitCombatPositions = new Transform[] {
					animator.GetBoneTransform (HumanBodyBones.LeftFoot),
					animator.GetBoneTransform (HumanBodyBones.RightFoot),
					animator.GetBoneTransform (HumanBodyBones.LeftHand),
					animator.GetBoneTransform (HumanBodyBones.RightHand)
				};
				for (int i = 0; i < hitCombatPositions.Length; i++) {
					GameObject hitCombatClone = (GameObject)Instantiate (hitCombatPrefab, Vector3.zero, Quaternion.identity);
					hitCombatClone.name = hitCombatClone.name.Replace ("(Clone)", "");
					hitCombatClone.transform.SetParent (hitCombatPositions [i]);
					hitCombatClone.transform.localPosition = Vector3.zero;
					hitCombatClone.transform.localRotation = Quaternion.identity;
					string name = hitCombatPositions [i].gameObject.name;
					if (hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.LeftFoot) || hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.RightFoot)) {
						addCombatPlace (name, defaultLegHitDamage, hitCombatClone, closeCombatSystem.colliderPlace.leg, hitCombatPositions [i].GetComponentInChildren<TrailRenderer> ());
						hitCombatClone.GetComponent<hitCombat> ().hitDamage = defaultLegHitDamage;
					} else {
						addCombatPlace (name, defaultArmHitDamage, hitCombatClone, closeCombatSystem.colliderPlace.arm, hitCombatPositions [i].GetComponentInChildren<TrailRenderer> ());
						hitCombatClone.GetComponent<hitCombat> ().hitDamage = defaultArmHitDamage;
					}
				}
			} else {
				print ("Animator not found in character, make sure it has one assigned");
			}

			animator = GetComponentInChildren<Animator> ();

			udpateHitCombatInfo ();

			updateInspector ();
		} else {
			print ("Assign the hit combat prefab to create the combat limbs");
		}
	}

	public void udpateHitCombatInfo ()
	{
		for (i = 0; i < combatLimbList.Count; i++) {
			//combatLimbList [i].hitCombatManager.timeToDisableTriggers = timeToDisableTriggers;
			combatLimbList [i].hitCombatManager.getOwner (gameObject);
			combatLimbList [i].hitCombatManager.layerMask = layerMaskToDamage;
			combatLimbList [i].hitCombatManager.hitDamage = combatLimbList [i].hitDamage;
			combatLimbList [i].hitCombatManager.addForceMultiplier = addForceMultiplier;
			combatLimbList [i].hitCombatManager.updateComponent ();
		}
	}

	public void removeLimbParts ()
	{
		for (i = 0; i < combatLimbList.Count; i++) {
			DestroyImmediate (combatLimbList [i].limb);
		}
		combatLimbList.Clear ();
		Component[] components = GetComponentsInChildren (typeof(hitCombat));
		foreach (Component c in components) {
			DestroyImmediate (c.gameObject);
		}
		updateInspector ();
	}

	public void AIMeleeAttack ()
	{
		if (Time.time > minTimeBetweenAttacks + lastTimeAttack || makeFullCombos) {
			bool doPunch = false;
			bool doKick = false;
			if (randomAttack) {
				int attackType = Random.Range (0, 2);
				if (attackType == 0) {
					punch ();
					doPunch = true;
				} else {
					kick ();
					doKick = true;
				}
			}
			if (makeFullCombos) {
				if (lastAttackKick) {
					kick ();
					doKick = true;
				}
				if (lastAttackPunch) {
					punch ();
					doPunch = true;
				}
			}
			if (alternateKickAndPunchs) {
				if (lastAttackKick) {
					punch ();
					doPunch = true;
				}
				if (lastAttackPunch) {
					kick ();
					doKick = true;
				}
			}
			lastTimeAttack = Time.time;
			if (doPunch) {
				lastAttackPunch = true;
				lastAttackKick = false;
			}
			if (doKick) {
				lastAttackPunch = false;
				lastAttackKick = true;
			}
		}
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		controlledByAI = !state;
	}

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
			for (int i = 0; i < combatLimbList.Count; i++) {
				if (combatLimbList [i].limb) {
					Gizmos.color = gizmoColor;
					Gizmos.DrawWireSphere (combatLimbList [i].limb.transform.position, gizmoRadius);
				}
			}
		}
	}

	public void getCombatPrefabs (GameObject combatPrefab)
	{
		hitCombatPrefab = combatPrefab;
		updateInspector ();
	}

	//CALL INPUT FUNCTIONS
	public void inputKick ()
	{
		if (combatCanBeUsed) {
			kick ();
		}
	}

	public void inputPunch ()
	{
		if (combatCanBeUsed) {
			punch ();
		}
	}

	public void updateInspector ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<closeCombatSystem> ());
		#endif
	}

	[System.Serializable]
	public class combatLimbInfo
	{
		public string name;
		public float hitDamage;
		public GameObject limb;
		public colliderPlace limbType;
		public TrailRenderer trail;
		public hitCombat hitCombatManager;
		public Collider trigger;
	}
}