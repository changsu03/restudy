using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class moveCameraToDevice : MonoBehaviour
{
	public bool cameraMovementActive = true;
	public GameObject cameraPosition;
	public bool smoothCameraMovement = true;
	public float cameraMovementSpeed = 2;
	public bool secondMoveCameraToDevice;

	public bool unlockCursor = true;

	public bool disablePlayerMeshGameObject = true;

	public bool disableWeaponsCamera;
	public bool keepWeaponsIfCarrying;
	public bool drawWeaponsIfPreviouslyCarrying;
	public bool keepOnlyIfPlayerIsOnFirstPerson;
	bool carryingWeaponsPreviously;
	bool firstPersonActive;

	public bool carryWeaponOnLowerPositionActive;

	public bool setPlayerCameraRotationOnExit;
	public Transform playerPivotTransformThirdPerson;
	public Transform playerCameraTransformThirdPerson;
	public Transform playerPivotTransformFirstPerson;
	public Transform playerCameraTransformFirstPerson;

	public bool disablePlayerHUDWhileUsing;

	public bool alignPlayerWithCameraPositionOnStopUseDevice;

	public bool showGizmo;
	public float gizmoRadius = 0.1f;
	public Color gizmoLabelColor = Color.black;
	public float gizmoArrowLenght = 0.3f;
	public float gizmoArrowLineLenght = 0.5f;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;

	Transform cameraParentTransform;
	Vector3 mainCameraTargetPosition;
	Quaternion mainCameraTargetRotation;
	Coroutine cameraState;
	bool deviceEnabled;
	Camera mainCamera;
	menuPause pauseManager;
	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	playerWeaponsManager weaponsManager;
	usingDevicesSystem usingDevicesManager;
	headBob headBobManager;
	otherPowers otherPowersManager;
	gravitySystem gravitySystemManager;
	grabObjects grabObjectsManager;
	footStepManager stepManager;
	playerCamera playerCameraManager;
	headTrack headTrackManager;
	mapSystem mapSystemManager;
	vehicleHUDInfo HUDInfoManager;

	bool previouslyUsingGrabObjectsCursor;
	bool previouslyIconButtonActive;

	bool movingCamera;

	Coroutine headTrackTargetCoroutine;
	Transform headTrackTargeTransform;

	//this function was placed in computer device, but now it can be added to any type of device when the player is using it,
	//to move the camera position and rotation in front of the device and place it again in its regular place when the player stops using the device

	void Start ()
	{
		mainCameraTargetRotation = Quaternion.identity;
		mainCameraTargetPosition = Vector3.zero;
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
			if (!secondMoveCameraToDevice) {
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

				if (disableWeaponsCamera) {
					if (weaponsManager.carryingWeaponInFirstPerson) {
						weaponsManager.weaponsCamera.gameObject.SetActive (false);
					}
				}

				pauseManager.usingDeviceState (deviceEnabled);
				currentPlayerControllerManager.changeScriptState (!deviceEnabled);

				if (disablePlayerMeshGameObject) {
					gravitySystemManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
				}

				stepManager.enableOrDisableFootStepsComponents (!deviceEnabled);

				if (unlockCursor) {
					pauseManager.showOrHideCursor (deviceEnabled);
				}
				pauseManager.changeCameraState (!deviceEnabled);
			}

			previouslyIconButtonActive = usingDevicesManager.getCurrentIconButtonState ();
			usingDevicesManager.setIconButtonCanBeShownState (false);

			if (cameraMovementActive) {
				cameraParentTransform = mainCamera.transform.parent;
				mainCamera.transform.SetParent (cameraPosition.transform);
			}

			previouslyUsingGrabObjectsCursor = grabObjectsManager.grabObjectCursorState ();
			grabObjectsManager.enableOrDisableGrabObjectCursor (false);
		} else {

			//set player camera rotation when the player stops using the device
			if (setPlayerCameraRotationOnExit) {
				bool isFirstPersonActive = playerCameraManager.isFirstPersonActive ();

				Vector3 pivotCameraRotation = Vector3.zero;
				Vector3 cameraRotation = Vector3.zero;

				if (isFirstPersonActive) {
					cameraRotation = playerCameraTransformFirstPerson.eulerAngles;
					pivotCameraRotation = playerPivotTransformFirstPerson.localEulerAngles;
				} else {
					cameraRotation = playerCameraTransformThirdPerson.eulerAngles;
					pivotCameraRotation = playerPivotTransformThirdPerson.localEulerAngles;
				}
					
				playerCameraManager.transform.eulerAngles = cameraRotation;
				playerCameraManager.getPivotCameraTransform ().localEulerAngles = pivotCameraRotation;

				float newLookAngleValue = pivotCameraRotation.x;
				if (newLookAngleValue > 180) {
					newLookAngleValue -= 360;
				}
				playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
				playerCameraManager.setCurrentCameraUpRotationValue (0);
			}

			//if the player disconnect the computer, then enabled of its components and set the camera to its previous position inside the player
			if (!secondMoveCameraToDevice) {
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

				if (disableWeaponsCamera) {
					if (weaponsManager.carryingWeaponInFirstPerson) {
						weaponsManager.weaponsCamera.gameObject.SetActive (true);
					}
				}

				if (disablePlayerMeshGameObject) {
					gravitySystemManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
				}

				stepManager.enableOrDisableFootStepsWithDelay (!deviceEnabled, 0);

				if (unlockCursor) {
					pauseManager.showOrHideCursor (deviceEnabled);
				}
				pauseManager.changeCameraState (!deviceEnabled);
			}

			if (cameraMovementActive) {
				if (alignPlayerWithCameraPositionOnStopUseDevice) {
					float xPosition = currentPlayer.transform.InverseTransformPoint (mainCamera.transform.position).x;
					float zPosition = currentPlayer.transform.InverseTransformPoint (mainCamera.transform.position).z;

					currentPlayer.transform.position += currentPlayer.transform.right * xPosition + currentPlayer.transform.forward * zPosition;

					playerCameraManager.transform.position = currentPlayer.transform.position;
				}

				mainCamera.transform.SetParent (cameraParentTransform);
			}

			grabObjectsManager.enableOrDisableGrabObjectCursor (previouslyUsingGrabObjectsCursor);

			usingDevicesManager.setIconButtonCanBeShownState (previouslyIconButtonActive);

			usingDevicesManager.checkIfRemoveDeviceFromList ();
		}

		if (disablePlayerHUDWhileUsing) {
			mapSystemManager.enableOrDisableMiniMap (!state);
			HUDInfoManager.playerHUD.SetActive (!state);
		}

		pauseManager.enableOrDisableDynamicElementsOnScreen (!deviceEnabled);

		if (cameraMovementActive) {
			if (smoothCameraMovement) {
				
				//stop the coroutine to translate the camera and call it again
				if (cameraState != null) {
					StopCoroutine (cameraState);
				}
				cameraState = StartCoroutine (adjustCamera ());

				if (headTrackManager.useHeadTrackTarget) {
					headTrackTargeTransform = headTrackManager.getHeadTrackTargetTransform ();
					if (headTrackTargetCoroutine != null) {
						StopCoroutine (headTrackTargetCoroutine);
					}
					headTrackTargetCoroutine = StartCoroutine (adjustHeadTrackTarget ());
				}
			} else {
				mainCamera.transform.localRotation = mainCameraTargetRotation;
				mainCamera.transform.localPosition = mainCameraTargetPosition;

				if (headTrackManager.useHeadTrackTarget) {
					headTrackTargeTransform = headTrackManager.getHeadTrackTargetTransform ();

					if (deviceEnabled) {
						headTrackTargeTransform.SetParent (cameraPosition.transform);
						headTrackTargeTransform.localPosition = mainCameraTargetPosition;
					} else {
						headTrackTargeTransform.SetParent (headTrackManager.getHeadTrackTargetParent ());
						headTrackTargeTransform.localPosition = headTrackManager.getOriginalHeadTrackTargetPosition ();
					}
				}
			}
		}

		if (unlockCursor) {
			pauseManager.showOrHideMouseCursorController (deviceEnabled);
		}
	}

	//move the camera from its position in player camera to a fix position for a proper looking of the computer and vice versa
	IEnumerator adjustCamera ()
	{
		movingCamera = true;
		float i = 0;
		//store the current rotation of the camera
		Quaternion currentQ = mainCamera.transform.localRotation;
		//store the current position of the camera
		Vector3 currentPos = mainCamera.transform.localPosition;
		//translate position and rotation camera
		while (i < 1) {
			i += Time.deltaTime * cameraMovementSpeed;
			mainCamera.transform.localRotation = Quaternion.Lerp (currentQ, mainCameraTargetRotation, i);
			mainCamera.transform.localPosition = Vector3.Lerp (currentPos, mainCameraTargetPosition, i);
			yield return null;
		}

		movingCamera = false;
	}

	//move the camera from its position in player camera to a fix position for a proper looking of the computer and vice versa
	IEnumerator adjustHeadTrackTarget ()
	{
		Vector3 targetPosition = mainCameraTargetPosition;
		Quaternion targeRotation = Quaternion.identity;

		headTrackTargeTransform.SetParent (cameraPosition.transform);
		if (!deviceEnabled) {
			targetPosition = headTrackManager.getOriginalHeadTrackTargetPosition ();
			headTrackTargeTransform.SetParent (headTrackManager.getHeadTrackTargetParent ());
		}

		float i = 0;
		//store the current rotation of the camera
		Quaternion currentQ = headTrackTargeTransform.localRotation;
		//store the current position of the camera
		Vector3 currentPos = headTrackTargeTransform.localPosition;
		//translate position and rotation camera
		while (i < 1) {
			i += Time.deltaTime * cameraMovementSpeed;
			headTrackTargeTransform.localRotation = Quaternion.Lerp (currentQ, targeRotation, i);
			headTrackTargeTransform.localPosition = Vector3.Lerp (currentPos, targetPosition, i);
			yield return null;
		}
	}

	public void hasSecondMoveCameraToDevice ()
	{
		secondMoveCameraToDevice = true;
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		currentPlayerControllerManager = currentPlayer.GetComponent<playerController> ();
		playerCameraManager = currentPlayerControllerManager.getPlayerCameraManager ();
		mainCamera = playerCameraManager.getMainCamera ();
		usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
		headBobManager = mainCamera.GetComponent<headBob> ();
		otherPowersManager = currentPlayer.GetComponent<otherPowers> ();
		gravitySystemManager = currentPlayer.GetComponent<gravitySystem> ();
		grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();
		pauseManager = currentPlayer.GetComponent<playerInputManager> ().getPauseManager ();
		weaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();
		stepManager = currentPlayer.GetComponent<footStepManager> ();
		headTrackManager = currentPlayer.GetComponent<headTrack> ();
		HUDInfoManager = pauseManager.getHUDInfoManager ();

		mapSystemManager = pauseManager.getMapSystem ();
	}

	public void enableFreeInteractionState ()
	{
		if (carryWeaponOnLowerPositionActive) {
			weaponsManager.setCarryWeaponInLowerPositionActiveState (true);

			HUDInfoManager.setFreeInteractionDeviceCursorActiveState (true);

			grabObjectsManager.enableOrDisableGeneralCursor (false);
		}
	}

	public void disableFreeInteractionState ()
	{
		if (carryWeaponOnLowerPositionActive) {
			weaponsManager.setCarryWeaponInLowerPositionActiveState (false);

			HUDInfoManager.setFreeInteractionDeviceCursorActiveState (false);

			grabObjectsManager.enableOrDisableGeneralCursor (true);
		}
	}

	public void stopMovement ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}
		deviceEnabled = false;
	}

	public bool isCameraMoving ()
	{
		return movingCamera;
	}

	public void setCurrentPlayerUseDeviceButtonEnabledState (bool state)
	{
		if (usingDevicesManager) {
			usingDevicesManager.setUseDeviceButtonEnabledState (state);
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

	void DrawGizmos ()
	{
		if (showGizmo) {
			if (cameraPosition) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (cameraPosition.transform.position, gizmoRadius);
				Gizmos.color = Color.white;
				Gizmos.DrawLine (cameraPosition.transform.position, transform.position);

				Gizmos.color = Color.green;
				GKC_Utils.drawGizmoArrow (cameraPosition.transform.position, cameraPosition.transform.forward * gizmoArrowLineLenght, gizmoArrowColor, gizmoArrowLenght, gizmoArrowAngle);
			}
		}
	}
}