using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSphereModeSystem : MonoBehaviour
{
	public bool sphereModeEnabled;
	public bool usingSphereMode;

	public GameObject newVehiclePrefab;
	public playerController playerManager;
	public gravitySystem gravityManager;
	public usingDevicesSystem usingDevicesManager;
	public Collider playerCollider;

	GameObject sphereVehicle;
	GameObject sphereVehicleParent;
	Collider sphereVehicleCollider;
	vehicleHUDManager vehicleManager;
	GameObject vehicleCamera;
	vehicleGravityControl vehicleGravityManager;

	public void setSphereModeActiveState (bool state)
	{
		if (!sphereModeEnabled) {
			return;
		}

		if (playerManager.canUseSphereMode) {
			StartCoroutine (setVehicleState (state));
		}
	}

	IEnumerator setVehicleState (bool state)
	{
		if (state) {
			if (!sphereVehicleParent) {
				sphereVehicleParent = (GameObject)Instantiate (newVehiclePrefab, Vector3.one * 1000, Quaternion.identity);

				yield return new WaitForSeconds (0.00001f);

				vehicleGravityManager = sphereVehicleParent.GetComponentInChildren<vehicleGravityControl> ();

				vehicleManager = sphereVehicleParent.GetComponent<IKDrivingSystem> ().getHUDManager ();

				sphereVehicle = vehicleManager.gameObject;

				vehicleGravityManager.pauseDownForce (true);
				sphereVehicle.SetActive (false);

				vehicleCamera = vehicleManager.getVehicleCameraController ().gameObject;

				sphereVehicleCollider = sphereVehicle.GetComponent<Collider> ();

				sphereVehicleCollider.enabled = false;
				yield return new WaitForSeconds (0.00001f);

				vehicleGravityManager.pauseDownForce (false);

				yield return null;
			}

			if (sphereVehicle) {
				if (!sphereVehicle.activeSelf) {
					vehicleGravityManager.setCustomNormal (gravityManager.getCurrentNormal ());
					Vector3 vehiclePosition = transform.position + transform.up;
					sphereVehicle.transform.position = vehiclePosition;
					vehicleCamera.transform.position = vehiclePosition;
					playerManager.enableOrDisableSphereMode (true);
					sphereVehicle.SetActive (true);
					vehicleManager.OnTriggerEnter (playerCollider);
					yield return null;
				}
			}
		} else {
			if (sphereVehicle) {
				if (sphereVehicle.activeSelf) {
					sphereVehicle.transform.rotation = vehicleCamera.transform.rotation;
					playerManager.enableOrDisableSphereMode (false);
				} 
			}
		}

		if (sphereVehicle) {
			if (state) {
				usingDevicesManager.addDeviceToList (sphereVehicle);
				usingDevicesManager.getclosestDevice ();
				usingDevicesManager.setCurrentVehicle (sphereVehicle);
				usingDevicesManager.useDevice ();

				usingDevicesManager.setUseDeviceButtonEnabledState (false);
			} else {
				if (sphereVehicleParent) {
					usingDevicesManager.useDevice ();
					usingDevicesManager.checkTriggerInfo (sphereVehicleCollider, false);
					usingDevicesManager.removeCurrentVehicle (sphereVehicle);
				}
				sphereVehicle.SetActive (false);

				usingDevicesManager.setUseDeviceButtonEnabledState (true);
			}
		}
	}
}
