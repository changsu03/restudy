using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class smartphoneDevice : MonoBehaviour
{
	public GameObject smartphoneCamera;
	public GameObject smartphoneScreenCanvas;
	public bool canMakePhotos;
	public bool usePhotoSound;
	public AudioClip photoSound;
	public bool canUseFlash;
	public float flashDuration = 0.1f;
	public GameObject cameraFlash;

	public Transform mainScreenCenter;
	public Transform screenCenter;

	public bool canUseZoom;
	public float maxZoomAmout;
	public float minZoomAmount;
	public float zoomSpeed;

	public bool checkObjectFoundOnCapture;
	public LayerMask layerToCheckObjectFound;
	public float rayDistanceToCheckObjectFound;
	public Transform raycastTransform;
	RaycastHit hit;

	public bool storeCapturesEnabled = true;

	public bool useEventOnCapture;
	public UnityEvent eventOnCapture;

	public playerWeaponsManager mainPlayerWeaponsManager;
	public cameraCaptureSystem cameraCaptureManager;

	public AudioSource mainAudioSource;
	public playerWeaponSystem weaponManager;
	public Camera smartphoneMainCamera;
	public Transform playerCameraTransform;

	bool isActivated;

	bool changingZoom;
	float currentFov;
	float zoomDirection = -1;
	cameraPerspectiveSystem currentPerspectiveSystem;

	Coroutine flashCoroutine;

	void Update ()
	{
		if (canUseZoom && changingZoom) {
			currentFov = smartphoneMainCamera.fieldOfView + Time.deltaTime * zoomSpeed * zoomDirection;
			if (zoomDirection == -1) {
				if (currentFov < minZoomAmount) {
					zoomDirection = 1;
				}
			} else {
				if (currentFov > maxZoomAmout) {
					zoomDirection = -1;
				}
			}
			smartphoneMainCamera.fieldOfView = currentFov;
		}
	}

	public void takePhoto ()
	{
		if (canMakePhotos) {
			playSound ();

			checkFlash ();

			if (storeCapturesEnabled) {
				cameraCaptureManager.takeCapture (smartphoneMainCamera);
			}

			if (currentPerspectiveSystem) {
				currentPerspectiveSystem.checkCurrentPlayerPosition (playerCameraTransform, weaponManager.getMainCameraTransform (), smartphoneMainCamera);
			}

			if (checkObjectFoundOnCapture) {
				if (Physics.Raycast (raycastTransform.position, raycastTransform.forward, out hit, rayDistanceToCheckObjectFound, layerToCheckObjectFound)) {
					eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = hit.collider.gameObject.GetComponent<eventObjectFoundOnCaptureSystem> ();
					if (currentEventObjectFoundOnCaptureSystem) {
						currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();
					}
				}
			}

			changingZoom = false;

			if (useEventOnCapture) {
				eventOnCapture.Invoke ();
			}
		}
	}

	public void checkFlash ()
	{
		if (!canUseFlash) {
			return;
		}

		if (flashCoroutine != null) {
			StopCoroutine (flashCoroutine);
		}
		flashCoroutine = StartCoroutine (flashCoroutineCoroutine ());
	}

	IEnumerator flashCoroutineCoroutine ()
	{
		cameraFlash.SetActive (true);

		yield return new WaitForSeconds (flashDuration);

		cameraFlash.SetActive (false);

		yield return null;
	}


	public void changeZoom ()
	{
		changingZoom = !changingZoom;
	}

	public void turnOn ()
	{
		isActivated = true;
		setSmartphoneState (isActivated);
	}

	public void turnOff ()
	{
		isActivated = false;
		setSmartphoneState (isActivated);
	}

	public void changeSmartphoneState ()
	{
		setSmartphoneState (!isActivated);
	}

	public void setSmartphoneState (bool state)
	{
		isActivated = state;
		smartphoneScreenCanvas.SetActive (isActivated);
		smartphoneCamera.SetActive (isActivated);
		changingZoom = false;

		if (isActivated) {
			mainPlayerWeaponsManager.setWeaponPartLayer (smartphoneScreenCanvas);
		}
	}

	public void playSound ()
	{
		if (usePhotoSound) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);
			mainAudioSource.PlayOneShot (photoSound);
		}
	}

	public void setCurrentPerspectiveSystem (cameraPerspectiveSystem perspective)
	{
		currentPerspectiveSystem = perspective;
	}

	public void removeCurrentPerspectiveSystem ()
	{
		currentPerspectiveSystem = null;
	}

	public Camera getSmarthphoneMainCamera ()
	{
		return smartphoneMainCamera;
	}

	public void setUseEventOnCaptureState (bool state)
	{
		useEventOnCapture = state;
	}

	public void setStoreCapturesEnabledState (bool state)
	{
		storeCapturesEnabled = state;
	}

	public void rotateScreenToRight ()
	{
		mainScreenCenter.localEulerAngles = Vector3.zero;
		screenCenter.localEulerAngles = Vector3.zero;
	}

	public void rotateScreenToLeft ()
	{
		mainScreenCenter.localEulerAngles = new Vector3 (0, 0, 180);
		screenCenter.localEulerAngles = new Vector3 (0, 0, 180);
	}
}
