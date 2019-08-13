using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class pauseOrResumePlayerControllerAndCameraSystem : MonoBehaviour
{
	public GameObject currentPlayer;
	public GameObject playerCameraGameObject;

	public bool assignPlayerManually;

	public bool playerComponentsPaused;

	public bool unlockCursor;

	public bool cameraIsMoved;
	public float resetCameraPositionSpeed;
	public bool setCameraDirectionAtEnd;
	public Transform cameraDirection;
	public Transform pivotDirection;

	public bool useEventOnPause;
	public UnityEvent eventOnPause;
	public eventParameters.eventToCallWithGameObject eventToSendCamera;
	public bool useEventOnResume;
	public UnityEvent eventOnResume;

	Transform mainCamera;
	playerController playerControllerManager;
	playerCamera playerCameraManager;
	headBob headBobManager;
	playerStatesManager statesManager;
	mapSystem mapSystemManager;
	vehicleHUDInfo HUDInfoManager;
	menuPause pauseManager;
	Transform previousCameraParent;
	Vector3 previousCameraPosition;

	void Start ()
	{
		if (assignPlayerManually) {
			getCurrentPlayer (currentPlayer);
		}
	}

	public void getCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		playerControllerManager = currentPlayer.GetComponent<playerController> ();
		playerCameraManager = playerControllerManager.getPlayerCameraManager ();
		playerCameraGameObject = playerCameraManager.gameObject;
		mainCamera = playerCameraManager.getMainCamera ().transform;
		headBobManager = mainCamera.GetComponent<headBob> ();

		statesManager = currentPlayer.GetComponent<playerStatesManager> ();

		pauseManager = currentPlayer.GetComponent<playerInputManager> ().getPauseManager();

		HUDInfoManager = pauseManager.getHUDInfoManager ();

		mapSystemManager = pauseManager.getMapSystem ();
	}

	public void pauseOrPlayPlayerComponents (bool state)
	{
		playerComponentsPaused = state;
		playerControllerManager.smoothChangeScriptState (!state);
		playerControllerManager.setUsingDeviceState (state);
		HUDInfoManager.playerHUD.SetActive (!state);

		if (playerComponentsPaused) {
			headBobManager.stopAllHeadbobMovements ();
		}

		headBobManager.playOrPauseHeadBob (!state);

		statesManager.checkPlayerStates(false, true, true, true, false, false, true, true);
		playerCameraManager.pauseOrPlayCamera (!state);

		mapSystemManager.enableOrDisableMiniMap (!state);
		checkCharacterMesh (state);

		if (unlockCursor) {
			pauseManager.showOrHideCursor (playerComponentsPaused);
			pauseManager.usingDeviceState (playerComponentsPaused);
			pauseManager.usingSubMenuState (playerComponentsPaused);
		}

		pauseManager.enableOrDisableDynamicElementsOnScreen (!playerComponentsPaused);

		if (playerComponentsPaused) {
			if (useEventOnPause) {
				eventToSendCamera.Invoke (mainCamera.gameObject);
				eventOnPause.Invoke ();
			}
			if (cameraIsMoved) {
				previousCameraParent = mainCamera.parent;
				previousCameraPosition = mainCamera.localPosition;
				mainCamera.SetParent (null);
			}
		} else {
			if (useEventOnResume) {
				eventOnResume.Invoke ();
			}
		}
	}

	public void checkCharacterMesh (bool state)
	{
		bool firstCameraEnabled = playerCameraManager.isFirstPersonActive ();
		if (firstCameraEnabled) {
			playerControllerManager.setCharacterMeshGameObjectState (state);
		}
	}

	Coroutine resetCameraPositionCoroutine;

	public void resetCameraPosition ()
	{
		if (!cameraIsMoved) {
			return;
		}

		if (resetCameraPositionCoroutine != null) {
			StopCoroutine (resetCameraPositionCoroutine);
		}
		resetCameraPositionCoroutine = StartCoroutine (resetCameraCoroutine ());
	}

	IEnumerator resetCameraCoroutine ()
	{
		setCameraDirection ();

		print ("new parent " + previousCameraParent);
		mainCamera.SetParent (previousCameraParent);

		Vector3	targetPosition = previousCameraPosition;

		Vector3	worldTargetPosition = previousCameraParent.position;
		float dist = GKC_Utils.distance (mainCamera.position, worldTargetPosition);
		float duration = dist / resetCameraPositionSpeed;
		float t = 0;

		while ((t < 1 && (mainCamera.localPosition != targetPosition || mainCamera.localRotation != Quaternion.identity))) {
			t += Time.deltaTime / duration;
			mainCamera.localPosition = Vector3.Lerp (mainCamera.localPosition, targetPosition, t);
			mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, Quaternion.identity, t);
			yield return null;
		}

		pauseOrPlayPlayerComponents (false);
	}

	public void setCameraDirection ()
	{
		if (setCameraDirectionAtEnd) {
			playerCameraGameObject.transform.rotation = cameraDirection.rotation;
			Quaternion newCameraRotation = pivotDirection.localRotation;
			playerCameraManager.getPivotCameraTransform ().localRotation = newCameraRotation;
			float newLookAngleValue = newCameraRotation.eulerAngles.x;
			if (newLookAngleValue > 180) {
				newLookAngleValue -= 360;
			}
			playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
		}
	}
}
