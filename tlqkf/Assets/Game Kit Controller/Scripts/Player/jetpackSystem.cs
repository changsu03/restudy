using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class jetpackSystem : MonoBehaviour
{
	public bool jetpackEnabled;
	public bool jetPackEquiped;
	public bool usingJetpack;
	public IKJetpackInfo IKInfo;
	public List<ParticleSystem> thrustsParticles = new List<ParticleSystem> ();
	public GameObject jetpack;
	public string animationName;
	public GameObject jetpackHUDInfo;
	public Slider jetpackSlider;
	public Text fuelAmountText;
	public float jetpackForce;
	public float jetpackAirSpeed;
	public float jetpackAirControl;
	public float jetpackFuelAmount;
	public float jetpackFuelRate;
	public float regenerativeSpeed;
	public float timeToRegenerate;

	public UnityEvent eventOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public bool showGizmo;
	public bool usedByAI;

	public playerController playerManager;
	public Animation jetPackAnimation;
	public IKSystem IKManager;

	int i;

	bool hudEnabled;
	float lastTimeUsed;

	void Start ()
	{
		if (usedByAI) {
			return;
		}
			
		if (jetpack == null) {
			jetpackEnabled = false;
			return;
		}

		changeThrustsParticlesState (false);

		if (jetpackFuelAmount > 0) {
			jetpackSlider.maxValue = jetpackFuelAmount;
			jetpackSlider.value = jetpackFuelAmount;
			hudEnabled = true;
			fuelAmountText.text = jetpackSlider.maxValue.ToString ("0") + " / " + jetpackSlider.value.ToString ("0");
		}

		if (!jetpackEnabled) {
			jetpack.SetActive (false);
		}
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}

		if (jetPackEquiped) {

			if (usingJetpack) {
				playerManager.setPlayerOnGroundState (false);
				if (hudEnabled) {
					jetpackSlider.value -= Time.deltaTime * jetpackFuelRate;
					fuelAmountText.text = jetpackSlider.maxValue.ToString ("0") + " / " + jetpackSlider.value.ToString ("0");
					if (jetpackSlider.value <= 0) {
						startOrStopJetpack (false);
					}
				}
			} else if (regenerativeSpeed > 0 && lastTimeUsed != 0) {
				if (Time.time > lastTimeUsed + timeToRegenerate) {
					jetpackSlider.value += regenerativeSpeed * Time.deltaTime;
					if (jetpackSlider.value >= jetpackSlider.maxValue) {
						jetpackSlider.value = jetpackSlider.maxValue;
						lastTimeUsed = 0;
					}
					fuelAmountText.text = jetpackSlider.maxValue.ToString ("0") + " / " + jetpackSlider.value.ToString ("0");
				}
			} 
		}
	}

	public bool canUseJetpack ()
	{
		bool value = false;
		if (jetpackSlider.value > 0) {
			value = true;
		}
		return value;
	}

	public void startOrStopJetpack (bool state)
	{
		if (usingJetpack != state) {
			usingJetpack = state;
			if (usingJetpack) {
				jetPackAnimation [animationName].speed = 1; 
				jetPackAnimation.Play (animationName);
			} else {
				jetPackAnimation [animationName].speed = -1; 
				jetPackAnimation [animationName].time = jetPackAnimation [animationName].length;
				jetPackAnimation.Play (animationName);
				lastTimeUsed = Time.time;
			}
			playerManager.usingJetpack = state;
			changeThrustsParticlesState (state);
			IKManager.jetpackState (state, IKInfo);
		}
	}

	public void changeThrustsParticlesState (bool state)
	{
		for (i = 0; i < thrustsParticles.Count; i++) {
			if (state) {
				if (!thrustsParticles [i].isPlaying) {
					thrustsParticles [i].gameObject.SetActive (true);
					thrustsParticles [i].Play ();
					thrustsParticles [i].loop = true;
				}
			} else {
				thrustsParticles [i].loop = false;
			}
		}
	}

	public void enableOrDisableJetpack (bool state)
	{
		if (!jetpackEnabled && state) {
			return;
		}

		jetPackEquiped = state;
		playerManager.equipJetpack (state, jetpackForce, jetpackAirControl, jetpackAirSpeed);
		if (jetPackEquiped) {
			if (hudEnabled) {
				jetpackHUDInfo.SetActive (jetPackEquiped);
			}

			eventOnStateEnabled.Invoke ();
		} else {
			jetpackHUDInfo.SetActive (jetPackEquiped);

			eventOnStateDisabled.Invoke ();
		}
	}

	public void getJetpackFuel (float amount)
	{
		float newValue = amount + jetpackSlider.value;
		if (newValue > jetpackSlider.maxValue) {
			jetpackSlider.maxValue = newValue;
		}
		jetpackSlider.value = newValue;
		fuelAmountText.text = jetpackSlider.maxValue.ToString ("0") + " / " + jetpackSlider.value.ToString ("0");
	}

	public void enableOrDisableJetPackMesh (bool state)
	{
		if (!jetpackEnabled && state) {
			return;
		}
		jetpack.SetActive (state);
	}

	public void inputStartOrStopJetpack (bool state)
	{
		if (jetPackEquiped && !playerManager.isPlayerAimingInThirdPerson () && !playerManager.driving && canUseJetpack ()) {
			if (state) {
				startOrStopJetpack (true);
			} else {					
				startOrStopJetpack (false);
			}
		}
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			for (i = 0; i < IKInfo.IKGoals.Count; i++) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (IKInfo.IKGoals [i].position.position, 0.05f);
			}
			for (i = 0; i < IKInfo.IKHints.Count; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (IKInfo.IKHints [i].position.position, 0.05f);
			}
		}
	}

	[System.Serializable]
	public class IKJetpackInfo
	{
		public List<IKGoalsJetpackPositions> IKGoals = new List<IKGoalsJetpackPositions> ();
		public List<IKHintsJetpackPositions> IKHints = new List<IKHintsJetpackPositions> ();
	}

	[System.Serializable]
	public class IKGoalsJetpackPositions
	{
		public string Name;
		public AvatarIKGoal limb;
		public Transform position;
	}

	[System.Serializable]
	public class IKHintsJetpackPositions
	{
		public string Name;
		public AvatarIKHint limb;
		public Transform position;
	}
}