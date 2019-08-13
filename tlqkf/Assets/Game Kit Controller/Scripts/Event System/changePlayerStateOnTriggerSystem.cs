using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class changePlayerStateOnTriggerSystem : MonoBehaviour
{
	public bool disableTriggerAfterEnter;

	public bool setJumpEnabledState;
	public bool jumpEnabledState;

	public bool setDoubleJumpState;
	public bool doubleJumpState;

	public bool setCameraViewState;
	public bool setCameraToFirstPerson;

	public bool setActionToChangeBetweenViewsState;
	public bool actionToChangeBetweenViewsState;

	public bool setPlayerInputActionState;
	public bool playerInputActionState;
	public string multiAxesInputName;
	public string axesInputName;

	public bool drawWeapon;
	public bool drawCurrentWeapon;
	public bool drawCertainWeapon;
	public string weaponNameToDraw;
	public bool keepWeapon;

	public bool setGravityPowerState;
	public bool gravityPowerState;

	public bool setZeroGravityModeState;
	public bool zeroGravityModeState;

	public bool setFreeFloatingModeState;
	public bool freeFloatingModeState;

	public bool setGravityDirection;
	public setGravity setGravityManager;

	public bool setLookAtPointState;
	public bool lookAtPointState;
	public Transform pointToLook;
	public bool useDurationToLookAtPoint;
	public float durationToLookAtPoint;
	public bool enableLookAtPointStateAfterDuration;
	public bool setLookAtPointSpeed;
	public float lookAtPointSpeed;

	public bool setMaxDistanceToFindTarget;
	public float maxDistanceToFindTarget;

	public bool setCameraZoomState;
	public bool cameraZoomState;
	public bool useCameraZoomDuration;
	public float cameraZoomDuration;
	public bool enableCameraZoonInputAfterDuration;

	public bool setWalkSpeedValue;
	public float walkSpeedValue;

	public bool setStairsAdherenceValue;
	public float stairsMaxValue = 0.25f;
	public float stairsMinValue = 0.2f;
	public float stairsGroundAdherence = 8;

	public bool changeRootMotionActive;
	public bool useRootMotionActive = true;

	public bool useEventOnTrigger;
	public UnityEvent eventOnTrigger;
	public bool sendPlayerOnEvent;
	public eventParameters.eventToCallWithGameObject playerSendEvent;

	public bool setPlayerManually;
	public GameObject playerToConfigure;

	public GameObject currentPlayer;
	playerController playerControllerManager;
	playerCamera playerCameraManager;
	playerWeaponsManager weaponsManager;
	gravitySystem gravityManager;
	playerInputManager playerInput;

	void Start ()
	{
		if (setPlayerManually) {
			setCurrentPlayer (playerToConfigure);
		}

	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
	}

	public void changePlayerState ()
	{
		if (currentPlayer == null) {
			return;
		}

		//get all the components needed
		playerControllerManager = currentPlayer.GetComponent<playerController> ();
		if (playerControllerManager == null) {
			return;
		}

		playerCameraManager = playerControllerManager.getPlayerCameraManager ();
		weaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();
		gravityManager = currentPlayer.GetComponent<gravitySystem> ();
		playerInput = currentPlayer.GetComponent<playerInputManager> ();

		if (setJumpEnabledState) {
			playerControllerManager.enableOrDisableJump (jumpEnabledState);
		}

		if (setDoubleJumpState) {
			playerControllerManager.enableOrDisableDoubleJump (doubleJumpState);
		}

		if (setCameraViewState) {
			playerCameraManager.changeCameraToThirdOrFirstView (setCameraToFirstPerson);
		}

		if (setActionToChangeBetweenViewsState) {
			playerCameraManager.enableOrDisableChangeCameraView (actionToChangeBetweenViewsState);
		}

		if (drawWeapon) {
			if (drawCurrentWeapon) {
				weaponsManager.checkIfDrawSingleOrDualWeapon ();
			}

			if (drawCertainWeapon) {
				weaponsManager.selectWeaponByName (weaponNameToDraw, true);
			}
		}

		if (keepWeapon) {
			weaponsManager.checkIfKeepSingleOrDualWeapon ();
		}

		if (setGravityPowerState) {
			gravityManager.setGravityPowerEnabledState (gravityPowerState);
		}

		if (setZeroGravityModeState) {
			gravityManager.setZeroGravityModeOnState (zeroGravityModeState);
		}

		if (setFreeFloatingModeState) {
			gravityManager.setfreeFloatingModeOnState (freeFloatingModeState);
		}

		if (setGravityDirection) {
			Collider currentCollider = currentPlayer.GetComponent<Collider> ();
			setGravityManager.checkTriggerType (currentCollider, true);
		}

		if (setPlayerInputActionState) {
			playerInput.setPlayerInputActionState (playerInputActionState, multiAxesInputName, axesInputName);
		}
			
		//Player camera look at point settings
		if (setLookAtPointState) {
			if (lookAtPointState) {
				playerCameraManager.setLookAtTargetEnabledState (true);
			}

			if (setMaxDistanceToFindTarget) {
				playerCameraManager.setMaxDistanceToFindTargetValue (maxDistanceToFindTarget);
			}

			playerCameraManager.setLookAtTargetStateManual (lookAtPointState, pointToLook);
		
			if (lookAtPointState) {
				playerCameraManager.setLookAtTargetEnabledState (false);
			}
		
			if (setLookAtPointSpeed) {
				playerCameraManager.setLookAtTargetSpeedValue (lookAtPointSpeed);
			}

			if (useDurationToLookAtPoint) {
				playerCameraManager.setLookAtTargetEnabledStateDuration (false, durationToLookAtPoint, enableLookAtPointStateAfterDuration);
			}

			if (setMaxDistanceToFindTarget) {
				playerCameraManager.setOriginalmaxDistanceToFindTargetValue ();
			}
		}

		if (setCameraZoomState) {
			playerCameraManager.setZoom (cameraZoomState);

			if (useCameraZoomDuration) {
				playerCameraManager.setZoomStateDuration (false, cameraZoomDuration, enableCameraZoonInputAfterDuration);
			}
		}

		if (setWalkSpeedValue) {
			playerControllerManager.setWalkSpeedValue (walkSpeedValue);
		}

		if (setStairsAdherenceValue) {
			playerControllerManager.setStairsValues (stairsMaxValue, stairsMinValue, stairsGroundAdherence);
		}

		if (changeRootMotionActive) {
			playerControllerManager.setUseRootMotionActiveState (useRootMotionActive);
		}

		//Events to be called after any state is triggred
		if (sendPlayerOnEvent) {
			playerSendEvent.Invoke (currentPlayer);
		}

		if (useEventOnTrigger) {
			eventOnTrigger.Invoke ();
		}

		if (disableTriggerAfterEnter) {
			gameObject.SetActive (false);
		}
	}
}
