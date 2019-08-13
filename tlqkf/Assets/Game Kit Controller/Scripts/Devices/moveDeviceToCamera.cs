using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class moveDeviceToCamera : MonoBehaviour
{
	public GameObject deviceGameObject;
	public float distanceFromCamera;
	public bool smoothCameraMovement = true;
	public float cameraMovementSpeed = 2;

	public float maxZoomDistance;
	public float minZoomDistance;
	public float zoomSpeed;

	public string layerToExaminateDevices;

	public bool activateExaminateObjectSystem;

	public bool objectHasActiveRigidbody;

	public bool disablePlayerMeshGameObject = true;

	public bool keepWeaponsIfCarrying;
	public bool drawWeaponsIfPreviouslyCarrying;
	public bool keepOnlyIfPlayerIsOnFirstPerson;
	bool carryingWeaponsPreviously;
	bool firstPersonActive;

	public Collider deviceTrigger;

	public bool useListOfDisabledObjects;
	public List<GameObject> disabledObjectList = new List<GameObject> ();

	public List<Collider> colliderListToDisable = new List<Collider> ();
	public List<Collider> colliderListButtons = new List<Collider> ();

	public bool ignoreDeviceTriggerEnabled;

	float originalDistanceFromCamera;

	Vector3 devicePositionTarget;
	Quaternion deviceRotationTarget;

	Transform originalDeviceParentTransform;
	Coroutine cameraState;

	Transform deviceTransform;

	bool deviceEnabled;
	Camera mainCamera;
	menuPause pauseManager;
	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	playerWeaponsManager weaponsManager;
	usingDevicesSystem usingDevicesManager;
	bool previouslyUsingGrabObjectsCursor;
	bool previouslyIconButtonActive;
	examineObjectSystem examineObjectManager;

	Vector3 originalPosition;
	Quaternion originalRotation;
	List<layerInfo> layerList = new List<layerInfo> ();
	bool previouslyActivated;

	headBob headBobManager;
	otherPowers otherPowersManager;
	gravitySystem gravitySystemManager;
	footStepManager stepManager;
	grabObjects grabObjectsManager;
	Rigidbody mainRigidbody;
	bool originalKinematicValue;
	bool originalUseGravityValue;
	Collider playerCollider;

	void Start ()
	{
		if (deviceGameObject == null) {
			deviceGameObject = gameObject;
		}
		deviceTransform = deviceGameObject.transform;

		originalPosition = deviceTransform.localPosition;
		originalRotation = deviceTransform.localRotation;

		originalDeviceParentTransform = deviceTransform.parent;

		setLayerList ();
		originalDistanceFromCamera = distanceFromCamera;
		if (activateExaminateObjectSystem) {
			examineObjectManager = GetComponent<examineObjectSystem> ();
		}

		mainRigidbody = deviceGameObject.GetComponent<Rigidbody> ();
	}

	//activate the device
	public void moveCamera (bool state)
	{
		deviceEnabled = state;

		if (deviceEnabled) {
			headBobManager.stopAllHeadbobMovements ();
		}

		headBobManager.playOrPauseHeadBob (!state);


		//if the player is using the computer, disable the player controller, the camera, and set the parent of the camera inside the computer, 
		//to move to its view position

		if (deviceEnabled) {
			if (otherPowersManager.running) {
				otherPowersManager.stopRun ();
			}
			//make the mouse cursor visible according to the action of the player
			currentPlayerControllerManager.setUsingDeviceState (deviceEnabled);

			weaponsManager.setUsingDeviceState (deviceEnabled);

			if (keepWeaponsIfCarrying) {
				firstPersonActive = currentPlayerControllerManager.isPlayerOnFirstPerson ();
				if (!keepOnlyIfPlayerIsOnFirstPerson || firstPersonActive) {
					carryingWeaponsPreviously = weaponsManager.isUsingWeapons ();

					if (carryingWeaponsPreviously) {
						weaponsManager.checkIfKeepSingleOrDualWeapon ();
					}
				}
			}

			pauseManager.usingDeviceState (deviceEnabled);
			currentPlayerControllerManager.changeScriptState (!deviceEnabled);

			if (disablePlayerMeshGameObject) {
				gravitySystemManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
			}

			stepManager.enableOrDisableFootStepsComponents (!deviceEnabled);

			pauseManager.showOrHideCursor (deviceEnabled);
			pauseManager.changeCameraState (!deviceEnabled);

			previouslyUsingGrabObjectsCursor = grabObjectsManager.grabObjectCursorState ();
			grabObjectsManager.enableOrDisableGrabObjectCursor (false);

			previouslyIconButtonActive = usingDevicesManager.getCurrentIconButtonState ();
			usingDevicesManager.setIconButtonCanBeShownState (false);

			distanceFromCamera = originalDistanceFromCamera;

			deviceTransform.SetParent (mainCamera.transform);
			devicePositionTarget = Vector3.zero + Vector3.forward * distanceFromCamera;
			deviceRotationTarget = Quaternion.identity;

			setColliderListState (!deviceEnabled);
			setLayerListState (!deviceEnabled);
			usingDevicesManager.setExamineteDevicesCameraState (deviceEnabled);

			previouslyActivated = true;

			if (objectHasActiveRigidbody) {
				originalPosition = deviceTransform.position;
				originalRotation = deviceTransform.rotation;
			}

		} else {
			//if the player disconnect the computer, then enabled of its components and set the camera to its previous position inside the player
			//make the mouse cursor visible according to the action of the player
			currentPlayerControllerManager.setUsingDeviceState (deviceEnabled);

			pauseManager.usingDeviceState (deviceEnabled);
			currentPlayerControllerManager.changeScriptState (!deviceEnabled);

			weaponsManager.setUsingDeviceState (deviceEnabled);

			if (keepWeaponsIfCarrying) {
				if (!keepOnlyIfPlayerIsOnFirstPerson || firstPersonActive) {
					if (drawWeaponsIfPreviouslyCarrying && carryingWeaponsPreviously) {
						weaponsManager.checkIfDrawSingleOrDualWeapon ();
					}
				}
			}

			if (disablePlayerMeshGameObject) {
				gravitySystemManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
			}

			stepManager.enableOrDisableFootStepsWithDelay (!deviceEnabled, 0.5f);

			pauseManager.showOrHideCursor (deviceEnabled);
			pauseManager.changeCameraState (!deviceEnabled);

			grabObjectsManager.enableOrDisableGrabObjectCursor (previouslyUsingGrabObjectsCursor);

			if (previouslyActivated) {
				usingDevicesManager.setIconButtonCanBeShownState (previouslyIconButtonActive);
			}

			devicePositionTarget = originalPosition;
			deviceRotationTarget = originalRotation;
			deviceTransform.SetParent (originalDeviceParentTransform);

			usingDevicesManager.checkIfRemoveDeviceFromList ();
		}

		pauseManager.enableOrDisableDynamicElementsOnScreen (!deviceEnabled);

		if (smoothCameraMovement) {
			//stop the coroutine to translate the device and call it again
			checkCameraPosition ();
		} else {
			mainCamera.transform.localRotation = deviceRotationTarget;
			mainCamera.transform.localPosition = devicePositionTarget;
			if (!deviceEnabled) {
				setColliderListState (!deviceEnabled);
				setLayerListState (!deviceEnabled);
				usingDevicesManager.setExamineteDevicesCameraState (deviceEnabled);
			}
		}

		if (activateExaminateObjectSystem && examineObjectManager) {
			examineObjectManager.examineDevice ();
		}

		pauseManager.showOrHideMouseCursorController (deviceEnabled);
	}

	public void checkCameraPosition ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}
		cameraState = StartCoroutine (adjustCamera ());
	}

	//move the device from its position in the scene to a fix position in player camera for a proper looking
	IEnumerator adjustCamera ()
	{
		if (deviceEnabled) {
			setRigidbodyState (true);
		}

		float i = 0;
		Quaternion currentQ = deviceTransform.localRotation;
		Vector3 currentPos = deviceTransform.localPosition;
		//translate position and rotation of the device
		while (i < 1) {
			i += Time.deltaTime * cameraMovementSpeed;
			deviceTransform.localRotation = Quaternion.Lerp (currentQ, deviceRotationTarget, i);
			deviceTransform.localPosition = Vector3.Lerp (currentPos, devicePositionTarget, i);
			yield return null;
		}

		if (!deviceEnabled) {
			setColliderListState (true);
			setLayerListState (true);
			usingDevicesManager.setExamineteDevicesCameraState (false);
			setRigidbodyState (false);
		}
	}

	public void setRigidbodyState (bool state)
	{
		if (mainRigidbody && objectHasActiveRigidbody) {
			if (state) {
				originalKinematicValue = mainRigidbody.isKinematic;
				originalUseGravityValue = mainRigidbody.useGravity;
				mainRigidbody.useGravity = false;
				mainRigidbody.isKinematic = true;
			} else {
				mainRigidbody.useGravity = originalUseGravityValue;
				mainRigidbody.isKinematic = originalKinematicValue;
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		currentPlayerControllerManager = currentPlayer.GetComponent<playerController> ();
		mainCamera = currentPlayerControllerManager.getPlayerCameraManager ().getMainCamera ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
		headBobManager = mainCamera.GetComponent<headBob> ();
		otherPowersManager = currentPlayer.GetComponent<otherPowers> ();
		gravitySystemManager = currentPlayer.GetComponent<gravitySystem> ();
		grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();
		pauseManager = currentPlayer.GetComponent<playerInputManager> ().getPauseManager ();
		weaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();
		stepManager = currentPlayer.GetComponent<footStepManager> ();
		playerCollider = currentPlayer.GetComponent<Collider> ();
	}

	public void setColliderListState (bool state)
	{
		for (int i = 0; i < colliderListToDisable.Count; i++) {
			if (colliderListToDisable [i]) {
				colliderListToDisable [i].enabled = state;

				Physics.IgnoreCollision (playerCollider, colliderListToDisable [i], deviceEnabled);
			}
		}

		for (int i = 0; i < colliderListButtons.Count; i++) {
			Physics.IgnoreCollision (playerCollider, colliderListButtons [i], deviceEnabled);
		}

		if (ignoreDeviceTriggerEnabled && state) {
			deviceTrigger.enabled = false;
		} else {
			deviceTrigger.enabled = state;
		}

	}

	public void setIgnoreDeviceTriggerEnabledState (bool state)
	{
		ignoreDeviceTriggerEnabled = state;
	}

	public void setLayerList ()
	{
		Component[] components = deviceGameObject.GetComponentsInChildren (typeof(Transform));
		foreach (Component c in components) {
			layerInfo newLayerInfo = new layerInfo ();
			newLayerInfo.gameObject = c.gameObject;
			newLayerInfo.layerNumber = c.gameObject.layer;
			layerList.Add (newLayerInfo);
		}

		if (useListOfDisabledObjects) {
			for (int i = 0; i < disabledObjectList.Count; i++) {
				disabledObjectList [i].SetActive (false);
			}
		}
	}

	public void setLayerListState (bool state)
	{
		for (int i = 0; i < layerList.Count; i++) {
			if (layerList [i].gameObject) {
				if (state) {
					layerList [i].gameObject.layer = layerList [i].layerNumber;
				} else {
					layerList [i].gameObject.layer = LayerMask.NameToLayer (layerToExaminateDevices);
				}
			}
		}
	}

	public void changeDeviceZoom (bool zoomIn)
	{
		if (zoomIn) {
			distanceFromCamera += Time.deltaTime * zoomSpeed;
		} else {
			distanceFromCamera -= Time.deltaTime * zoomSpeed;
		}

		if (distanceFromCamera > maxZoomDistance) {
			distanceFromCamera = maxZoomDistance;
		}
		if (distanceFromCamera < minZoomDistance) {
			distanceFromCamera = minZoomDistance;
		}

		checkCameraPosition ();
		devicePositionTarget = Vector3.zero + Vector3.forward * distanceFromCamera;
		deviceRotationTarget = transform.localRotation;
	}

	public void resetRotation ()
	{
		devicePositionTarget = transform.localPosition;
		deviceRotationTarget = Quaternion.identity;
		checkCameraPosition ();
	}

	public void resetRotationAndPosition ()
	{
		devicePositionTarget = Vector3.zero + Vector3.forward * originalDistanceFromCamera;
		deviceRotationTarget = Quaternion.identity;
		checkCameraPosition ();
	}

	[System.Serializable]
	public class layerInfo
	{
		public GameObject gameObject;
		public int layerNumber;
	}
}