﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class headBob : MonoBehaviour
{
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;

	public bool headBobEnabled;
	public bool firstPersonMode;
	public bool externalShakeEnabled;
	public bool externalShakingActive;
	public bool headBobCanBeUsed = true;
	public string currentState;
	public string externalForceStateName;
	public bool useDynamicIdle;
	public string staticIdleName;
	public string dynamicIdleName;
	public float timeToActiveDynamicIdle;
	public bool shakeCameraInLockedMode;
	public List<bobStates> bobStatesList = new List<bobStates> ();
	public List<externalShakeInfoListElement> externalShakeInfoList = new List<externalShakeInfoListElement> ();

	public enum bobTransformType
	{
		onlyPosition,
		onlyRotation,
		both,
		none
	}

	public enum viewTypes
	{
		firstPerson,
		thirdPerson,
		both
	}

	public float resetSpeed;
	public Vector3 jumpStartMaxIncrease;
	public float jumpStartSpeed;
	public Vector3 jumpEndMaxDecrease;
	public float jumpEndSpeed;
	public float jumpResetSpeed;
	public bool headBobPaused = false;
	public bobStates playerBobState;
	public bool damageShakeActive;

	public externalShakeListManager externalShakeManager;

	bool stateChanged = false;
	bool checkResetCamera;
	Coroutine coroutineStartJump;
	Coroutine coroutineEndJump;
	Coroutine coroutineToStop;
	Coroutine externalForceCoroutine;
	Coroutine waitToActiveCoroutine;
	Vector3 initialTargetEul;
	Vector3 initialTargetPos;
	Transform mainCameraTransform;
	bool dead;

	float externalShakeDuration;
	float externalShakeDelay;

	bool useDelayBeforeStartDecrease;
	float delayBeforeStartDecrease;

	Coroutine decreaseShakeInTimeCoroutime;
	bool decreaseShakeInTime;
	float decreaseShakeSpeed;
	string defaultState;

	bool changePos;
	bool changeRot;
	float posTargetX;
	float posTargetY;
	float posTargetZ;
	Vector3 posTarget;
	float eulTargetX;
	float eulTargetY;
	float eulTargetZ;
	Vector3 eulTarget;

	bool stopHeadMovementsActive;

	int currentNumberOfRepeatShake;

	externalShakeInfo currentExternalShakeInfo;

	UnityEvent currentEventAtStart;
	UnityEvent currentEventAtEnd;
	bool useShakeEvent;

	void Start ()
	{
		//get the object to shake, in this case the main camera
		mainCameraTransform = transform;

		//set the position and rotation to reset the camera transform
		initialTargetEul = Vector3.zero;
		initialTargetPos = Vector3.zero;

		//set the initial state of the player
		playerBobState = new bobStates ();
		setState (currentState);
		defaultState = currentState;
		externalShakeManager = FindObjectOfType<externalShakeListManager> ();
		if (externalShakeManager) {
			//externalShakeInfoList = externalShakeManager.externalShakeInfoList;
		}
	}

	void Update ()
	{
		//if headbod enabled, check the current state
		if (headBobEnabled && headBobCanBeUsed) {
			if (playerBobState.bobTransformStyle != bobTransformType.none) {
				if (canBeUsed ()) {
					movementBob (playerBobState);
					if (stateChanged) {
						stateChanged = false;
					}

				}
			} else {
				if (!stateChanged) {
					if (!headBobPaused) {
						stopBobTransform ();
					}
					stateChanged = true;
				}
			}
		}
	}

	public bool canBeUsed ()
	{
		//if the camera is not being moved from the third to first move or viceversa,
		//or the camera is in first person mode and the current bobstate is only applied in first mode, 
		//or the camera is in third person mode and the current bobstate is only applied in third mode,
		//or the in the current bob state the camera is shake in both modes, then
		if (playerBobState.enableBobIn == viewTypes.both ||
		    ((playerBobState.enableBobIn == viewTypes.firstPerson && playerCameraManager.isFirstPersonActive () && firstPersonMode) ||
		    (playerBobState.enableBobIn == viewTypes.thirdPerson && !playerCameraManager.isFirstPersonActive ())) && !headBobPaused) {
			return true;
		}
		return false;
	}

	public bool cameraLockedAllowsShake ()
	{
		if ((shakeCameraInLockedMode && !playerCameraManager.isCameraTypeFree ()) || (!shakeCameraInLockedMode && playerCameraManager.isCameraTypeFree ()) ||
		    (shakeCameraInLockedMode && playerCameraManager.isCameraTypeFree ())) {
			return true;
		}
		return false;
	}

	public void stopBobTransform ()
	{
		if (coroutineToStop != null) {
			StopCoroutine (coroutineToStop);
		}
		coroutineToStop = StartCoroutine (resetCameraTransform ());
	}

	public void stopBobRotation ()
	{
		if (coroutineToStop != null) {
			StopCoroutine (coroutineToStop);
		}
		coroutineToStop = StartCoroutine (resetCameraRotation ());
	}
		
	//set a state in the current player state
	public void setState (string stateName)
	{
		//search the state recieved
		if ((stateName != playerBobState.Name && !externalShakingActive) ||
		    stateName == externalForceStateName ||
		    (!externalShakingActive && useDynamicIdle && stateName == dynamicIdleName) && !damageShakeActive) {

			for (int i = 0; i < bobStatesList.Count; i++) {
				if (bobStatesList [i].Name == stateName) {
					//if found, set the state values, and the enable this state as the current state
					playerBobState = bobStatesList [i];
					currentState = bobStatesList [i].Name;
					playerBobState.isCurrentState = true;
				} else {
					bobStatesList [i].isCurrentState = false;
				}
			}

			if (firstPersonMode) {
				if (stateName == "Jump Start") {
					jumpStarted ();
				}
				if (stateName == "Jump End") {
					jumpEnded ();
				}
			}
		}
	}

	public void setFirstOrThirdHeadBobView (bool state)
	{
		//if the camera is in first person view, then check the headbob
		firstPersonMode = state;
		//if the camera is set back to the third person mode, reset the camera rotation
		if (!firstPersonMode) {
			//stop the previous coroutine and play the reset camera rotation coroutine
			if (coroutineToStop != null) {
				StopCoroutine (coroutineToStop);
			}
			coroutineToStop = StartCoroutine (resetCameraRotation ());
		}
	}

	public void setFirstOrThirdMode (bool state)
	{
		firstPersonMode = state;
	}

	public void setShotShakeState (IKWeaponSystem.weaponShotShakeInfo shotShakeInfo)
	{
		if (externalShakeEnabled) {
			
			setState (externalForceStateName);

			if (firstPersonMode) {
				playerBobState.eulAmount = shotShakeInfo.shakeRotation;
				playerBobState.posAmount = shotShakeInfo.shakePosition;
				playerBobState.posSmooth = shotShakeInfo.shakeSmooth;
				playerBobState.eulSmooth = shotShakeInfo.shakeSmooth;
				playerBobState.bobTransformStyle = bobTransformType.both;
			} else {
				playerBobState.eulAmount = shotShakeInfo.shakeRotation;
				playerBobState.posAmount = Vector3.zero;
				playerBobState.posSmooth = 0;
				playerBobState.eulSmooth = shotShakeInfo.shakeSmooth;
				playerBobState.bobTransformStyle = bobTransformType.onlyRotation;
			}

			playerBobState.posSpeed = Vector3.one * shotShakeInfo.shotForce;
			playerBobState.eulSpeed = Vector3.one * shotShakeInfo.shotForce;
			externalShakeDuration = shotShakeInfo.shakeDuration;

			setExternalShakeDuration ();
		}
	}

	public void setExternalShakeState (externalShakeInfo shakeInfo)
	{
		if (externalShakeEnabled) {
			currentExternalShakeInfo = shakeInfo;
			
			setState (externalForceStateName);

			if (firstPersonMode) {
				playerBobState.eulAmount = shakeInfo.shakeRotation;
				playerBobState.posAmount = shakeInfo.shakePosition;
				playerBobState.posSmooth = shakeInfo.shakePositionSmooth;
				playerBobState.eulSmooth = shakeInfo.shakeRotationSmooth;
				playerBobState.bobTransformStyle = bobTransformType.both;
			} else {
				playerBobState.eulAmount = shakeInfo.shakeRotation;
				playerBobState.posAmount = Vector3.zero;
				playerBobState.posSmooth = 0;
				playerBobState.eulSmooth = shakeInfo.shakeRotationSmooth;
				playerBobState.bobTransformStyle = bobTransformType.onlyRotation;
			}

			playerBobState.posSpeed = shakeInfo.shakePositionSpeed;
			playerBobState.eulSpeed = shakeInfo.shakeRotationSpeed;

			externalShakeDuration = shakeInfo.shakeDuration;
			externalShakeDelay = shakeInfo.externalShakeDelay;

			decreaseShakeInTime = shakeInfo.decreaseShakeInTime;
			decreaseShakeSpeed = shakeInfo.decreaseShakeSpeed;

			useDelayBeforeStartDecrease = shakeInfo.useDelayBeforeStartDecrease;
			delayBeforeStartDecrease = shakeInfo.delayBeforeStartDecrease;

			playerBobState.repeatShake = shakeInfo.repeatShake;
			playerBobState.numberOfRepeats = shakeInfo.numberOfRepeats;
			playerBobState.delayBetweenRepeats = shakeInfo.delayBetweenRepeats;

			currentNumberOfRepeatShake = 0;

			setExternalShakeDuration ();

			checkDecreaseShake ();
		}
	}

	public void setExternalShakeStateByIndex (int index, bool isFirstPerson)
	{
		externalShakeInfoListElement newShake = externalShakeInfoList [index];
		if (isFirstPerson) {
			setExternalShakeState (newShake.firstPersonDamageShake);
		} else {
			setExternalShakeState (newShake.thirdPersonDamageShake);
		}
	}

	public void setExternalShakeStateByName (string shakeName, float multiplier)
	{
		if (!playerControllerManager.driving) {

			bool shakeNameFound = false;
			externalShakeInfo newShake = new externalShakeInfo ();
			for (int i = 0; i < externalShakeInfoList.Count; i++) {
				if (externalShakeInfoList [i].name == shakeName) {	
					if (externalShakeInfoList [i].sameValueBothViews) {
						newShake = new externalShakeInfo (externalShakeInfoList [i].thirdPersonDamageShake);
						if (!externalShakeInfoList [i].useDamageShakeInThirdPerson) {
							return;
						}
					} else {
						if (firstPersonMode) {
							if (!externalShakeInfoList [i].useDamageShakeInFirstPerson) {
								return;
							}
							newShake = new externalShakeInfo (externalShakeInfoList [i].firstPersonDamageShake);
						} else {
							if (!externalShakeInfoList [i].useDamageShakeInThirdPerson) {
								return;
							}
							newShake = new externalShakeInfo (externalShakeInfoList [i].thirdPersonDamageShake);
						}
					}
					shakeNameFound = true;
				}
			}

			if (shakeNameFound) {
				if (multiplier != 1) {
					newShake.shakePosition *= multiplier;
					newShake.shakePositionSpeed *= multiplier;
					newShake.shakeRotation *= multiplier;
					newShake.shakeRotationSpeed *= multiplier;
				}

				setExternalShakeState (newShake);
				damageShakeActive = true;
			} else {
				print ("WARNING: the shake type called " + shakeName + " wasn't found, make sure the name exist in the external " +
				"shake list manager and it is assigned in the head bod manager");
			}
		}
	}

	public void setExternalShakeStateToRepeat (externalShakeInfo shakeInfo)
	{
		if (externalShakeEnabled) {

			setState (externalForceStateName);

			if (firstPersonMode) {
				playerBobState.eulAmount = shakeInfo.shakeRotation;
				playerBobState.posAmount = shakeInfo.shakePosition;
				playerBobState.posSmooth = shakeInfo.shakePositionSmooth;
				playerBobState.eulSmooth = shakeInfo.shakeRotationSmooth;
				playerBobState.bobTransformStyle = bobTransformType.both;
			} else {
				playerBobState.eulAmount = shakeInfo.shakeRotation;
				playerBobState.posAmount = Vector3.zero;
				playerBobState.posSmooth = 0;
				playerBobState.eulSmooth = shakeInfo.shakeRotationSmooth;
				playerBobState.bobTransformStyle = bobTransformType.onlyRotation;
			}

			playerBobState.posSpeed = shakeInfo.shakePositionSpeed;
			playerBobState.eulSpeed = shakeInfo.shakeRotationSpeed;
			externalShakeDuration = shakeInfo.shakeDuration;
			externalShakeDelay = shakeInfo.externalShakeDelay;

			decreaseShakeInTime = shakeInfo.decreaseShakeInTime;
			decreaseShakeSpeed = shakeInfo.decreaseShakeSpeed;

			useDelayBeforeStartDecrease = shakeInfo.useDelayBeforeStartDecrease;
			delayBeforeStartDecrease = shakeInfo.delayBeforeStartDecrease;

			playerBobState.repeatShake = shakeInfo.repeatShake;
			playerBobState.numberOfRepeats = shakeInfo.numberOfRepeats;
			playerBobState.delayBetweenRepeats = shakeInfo.delayBetweenRepeats;

			setExternalShakeDuration ();

			checkDecreaseShake ();
		}
	}

	public void setExternalShakeDuration ()
	{
		externalShakingActive = true;
		if (externalForceCoroutine != null) {
			StopCoroutine (externalForceCoroutine);
		}

		externalForceCoroutine = StartCoroutine (setExternalShakeDurationCoroutine ());
	}

	IEnumerator setExternalShakeDurationCoroutine ()
	{
		bool repeatShake = false;

		if (useShakeEvent) {
			currentEventAtStart.Invoke ();
		}

		yield return new WaitForSeconds (externalShakeDelay);

		yield return new WaitForSeconds (externalShakeDuration);

		if (useShakeEvent) {
			currentEventAtEnd.Invoke ();
		}

		if (playerBobState.repeatShake) {

			if (currentNumberOfRepeatShake < playerBobState.numberOfRepeats) {
				currentNumberOfRepeatShake++;

				repeatShake = true;

				float delayBetweenRepeats = playerBobState.delayBetweenRepeats;

				stopExternalShake ();

				damageShakeActive = false;

				yield return new WaitForSeconds (delayBetweenRepeats);

				setExternalShakeStateToRepeat (currentExternalShakeInfo);
			} else {
				useShakeEvent = false;
			}
		} else {
			useShakeEvent = false;
		}

		if (!repeatShake) {
			stopExternalShake ();

			damageShakeActive = false;

			yield return null;	
		}
	}

	public void stopExternalShake ()
	{
		if (externalShakingActive) {
			playerControllerManager.setLastTimeMoved ();
		}

		externalShakingActive = false;

		if (!firstPersonMode) {
			stopBobRotation ();
			stateChanged = true;
		}
	}

	public void setCurrentExternalCameraShakeSystemEvents (UnityEvent eventAtStart, UnityEvent eventAtEnd)
	{
		currentEventAtStart = eventAtStart;
		currentEventAtEnd = eventAtEnd;
		useShakeEvent = true;
	}

	public void disableUseShakeEventState ()
	{
		useShakeEvent = false;
	}

	public void checkDecreaseShake ()
	{
		if (decreaseShakeInTime) {
			if (decreaseShakeInTimeCoroutime != null) {
				StopCoroutine (decreaseShakeInTimeCoroutime);
			}
			decreaseShakeInTimeCoroutime = StartCoroutine (checkDecreaseShakeInTimeCoroutime ());
		}
	}

	IEnumerator checkDecreaseShakeInTimeCoroutime ()
	{
		float t = 0;
		bool eulTargetIsZero = false;
		bool posTargetIsZero = false;

		float eulSmoothTarget = 0;
		float posSmoothTarget = 0;

		if (useDelayBeforeStartDecrease) {
			yield return new WaitForSeconds (delayBeforeStartDecrease);
		}

		while (t < 1) {
			t += Time.deltaTime / (1 / decreaseShakeSpeed);
			if (externalShakingActive) {
				if (playerBobState.eulSmooth == eulSmoothTarget) {
					eulTargetIsZero = true;
				}
				playerBobState.eulSmooth = Mathf.MoveTowards (playerBobState.eulSmooth, eulSmoothTarget, t);

				if (playerBobState.posSmooth == posSmoothTarget) {
					posTargetIsZero = true;
				}
				playerBobState.posSmooth = Mathf.MoveTowards (playerBobState.posSmooth, posSmoothTarget, t);
			
				if (posTargetIsZero && eulTargetIsZero) {
					t = 2;
				}
			}
			yield return null;
		}

		decreaseShakeInTime = false;

		if (!playerBobState.repeatShake || currentNumberOfRepeatShake >= playerBobState.numberOfRepeats) {
			stopExternalShake ();
		}

		yield return null;			
	}

	public void playerAliveOrDead (bool state)
	{
		dead = state;
		headBobCanBeUsed = !dead;
		if (dead) {
			stateChanged = true;
			headBobPaused = false;
			setState (staticIdleName);
		}
	}

	public void playOrPauseHeadBob (bool state)
	{
		headBobCanBeUsed = state;
	}

	public void stopAllHeadbobMovements ()
	{
		if (coroutineToStop != null) {
			StopCoroutine (coroutineToStop);
		}

		if (coroutineStartJump != null) {
			StopCoroutine (coroutineStartJump);
		}

		if (coroutineEndJump != null) {
			StopCoroutine (coroutineEndJump);
		}

		if (decreaseShakeInTimeCoroutime != null) {
			StopCoroutine (decreaseShakeInTimeCoroutime);
		}

		if (externalForceCoroutine != null) {
			StopCoroutine (externalForceCoroutine);
		}
	}

	//check the info of the current state, to apply rotation, translation, both or anything according to the parameters of the botState
	void movementBob (bobStates state)
	{
		if (!cameraLockedAllowsShake ()) {
			return;
		}

		if (stopHeadMovementsActive) {
			return;
		}

		changePos = false;
		changeRot = false;

		//check the type of shake
		if (playerBobState.bobTransformStyle == bobTransformType.onlyPosition) {
			changePos = true;
		} else if (playerBobState.bobTransformStyle == bobTransformType.onlyRotation) {
			changeRot = true;
		} else if (playerBobState.bobTransformStyle == bobTransformType.both) {
			changePos = true;
			changeRot = true;
		}

		//apply translation
		if (changePos) {
			posTargetX = Mathf.Sin (Time.time * state.posSpeed.x) * state.posAmount.x;
			posTargetY = Mathf.Sin (Time.time * state.posSpeed.y) * state.posAmount.y;
			posTargetZ = Mathf.Cos (Time.time * state.posSpeed.z) * state.posAmount.z;
			posTarget = new Vector3 (posTargetX, posTargetY, posTargetZ);
			mainCameraTransform.localPosition = Vector3.Lerp (mainCameraTransform.localPosition, posTarget, Time.deltaTime * state.posSmooth);
		}

		//apply rotation
		if (changeRot) {
			eulTargetX = Mathf.Sin (Time.time * state.eulSpeed.x) * state.eulAmount.x;
			eulTargetY = Mathf.Sin (Time.time * state.eulSpeed.y) * state.eulAmount.y;
			eulTargetZ = Mathf.Cos (Time.time * state.eulSpeed.z) * state.eulAmount.z;
			eulTarget = new Vector3 (eulTargetX, eulTargetY, eulTargetZ);
			mainCameraTransform.localRotation = Quaternion.Lerp (mainCameraTransform.localRotation, Quaternion.Euler (eulTarget), Time.deltaTime * state.eulSmooth);
		}
	}

	public void jumpStarted ()
	{
		if (stopHeadMovementsActive) {
			return;
		}

		//if the player jumps, stop the current coroutine, and play the jump coroutine
		if (coroutineEndJump != null) {
			StopCoroutine (coroutineEndJump);
		}
		coroutineStartJump = StartCoroutine (startJump ());
	}

	public void jumpEnded ()
	{
		if (stopHeadMovementsActive) {
			return;
		}

		//if the player is in firts person view and the camera is not moving from first to third mode, then
		if (playerCameraManager.isFirstPersonActive () && headBobEnabled && !dead) {
			//if the player reachs the ground after jump, stop the current coroutine, and play the landing coroutine
			if (coroutineStartJump != null) {
				StopCoroutine (coroutineStartJump);
			}
			coroutineEndJump = StartCoroutine (endJump ());
		}
	}

	IEnumerator startJump ()
	{
		//walk or run shakes are blocked
		headBobPaused = true;
		float i = 0.0f;
		float rate = jumpStartSpeed;
		//add to the current rotation the jumpStartMaxIncrease value, when the player jumps
		Vector3 targetEUL = new Vector3 (mainCameraTransform.localEulerAngles.x - jumpStartMaxIncrease.x, mainCameraTransform.localEulerAngles.y - jumpStartMaxIncrease.y, mainCameraTransform.localEulerAngles.z - jumpStartMaxIncrease.z);
		//store the current rotation
		Quaternion currentQ = mainCameraTransform.localRotation;
		//store the target rotation
		Quaternion targetQ = Quaternion.Euler (targetEUL);
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}
		headBobPaused = false;
	}

	IEnumerator endJump ()
	{
		headBobPaused = true;
		float i = 0.0f;
		float rate = jumpEndSpeed;
		//add to the current rotation the jumpMaxDrecrease value, when the player touch the ground again after jumping
		Vector3 targetEUL = new Vector3 (mainCameraTransform.localEulerAngles.x + jumpEndMaxDecrease.x, mainCameraTransform.localEulerAngles.y + jumpEndMaxDecrease.y, mainCameraTransform.localEulerAngles.z + jumpEndMaxDecrease.z);
		//store the current rotation
		Quaternion currentQ = mainCameraTransform.localRotation;
		//store the target rotation
		Quaternion targetQ = Quaternion.Euler (targetEUL);
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}
		//reset again the rotation of the camera
		i = 0;
		rate = jumpResetSpeed;
		currentQ = mainCameraTransform.localRotation;
		targetQ = Quaternion.Euler (initialTargetEul);
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}
		//the jump state has finished, so the camera can be shaked again
		headBobPaused = false;
	}

	IEnumerator resetCameraTransform ()
	{
		if (firstPersonMode || playerBobState.bobTransformStyle == bobTransformType.none) {
			float i = 0.0f;
			float rate = resetSpeed;
			//store the current rotation
			Quaternion currentQ = mainCameraTransform.localRotation;
			//store the current position
			Vector3 currentPos = mainCameraTransform.localPosition;
			while (i < 1.0f) {
				//reset the position and rotation of the camera to 0,0,0
				i += Time.deltaTime * rate;
				mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, Quaternion.Euler (initialTargetEul), i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPos, initialTargetPos, i);
				yield return null;
			}
			headBobPaused = false;
		}
	}

	IEnumerator resetCameraRotation ()
	{
		float i = 0.0f;
		float rate = resetSpeed;
		//store the current rotation
		Quaternion currentQ = mainCameraTransform.localRotation;
		while (i < 1.0f) {
			//reset the rotation of the camera to 0,0,0
			i += Time.deltaTime * rate;
			mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, Quaternion.Euler (initialTargetEul), i);
			yield return null;
		}
	}

	public void setDefaultState ()
	{
		setState (defaultState);
	}

	public void setShakeInManagerList (int index)
	{
		externalShakeManager.setShakeInManagerList (externalShakeInfoList [index], index);
	}

	Coroutine stopHeadBobCoroutine;

	public void pauseHeadBodWithDelay (float delayAmount)
	{
		stopAllHeadbobMovements ();

		if (stopHeadBobCoroutine != null) {
			StopCoroutine (stopHeadBobCoroutine);
		}
		stopHeadBobCoroutine = StartCoroutine (pauseHeadBodWithDelayCoroutine (delayAmount));
	}

	IEnumerator pauseHeadBodWithDelayCoroutine (float delayAmount)
	{
		stopHeadMovementsActive = true;
		yield return new WaitForSeconds (delayAmount);

		stopHeadMovementsActive = false;
		yield return null;		
	}

	public void updateExternalShakeInfoList (List<externalShakeInfoListElement> newExternalShakeInfoList)
	{
		externalShakeInfoList = newExternalShakeInfoList;

		updateComponent ();
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (GetComponent<headBob> ());
		#endif
	}

	[System.Serializable]
	public class bobStates
	{
		public string Name;
		public bobTransformType bobTransformStyle;
		public viewTypes enableBobIn;
		public Vector3 posAmount;
		public Vector3 posSpeed;
		public float posSmooth;
		public Vector3 eulAmount;
		public Vector3 eulSpeed;
		public float eulSmooth;
		public bool isCurrentState;

		public bool repeatShake;
		public int numberOfRepeats;
		public float delayBetweenRepeats;
	}
}