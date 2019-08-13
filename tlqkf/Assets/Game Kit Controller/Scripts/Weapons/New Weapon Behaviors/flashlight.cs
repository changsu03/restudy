using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlight : MonoBehaviour
{
	public GameObject mainLight;
	public Light mainFlashlight;
	public bool infiniteEnergy;
	public float useEnergyRate;
	public int amountEnergyUsed;
	public bool useSound;
	public AudioClip turnOnSound;
	public AudioClip turnOffSound;
	public bool isActivated;

	public bool reloading;

	public bool useHighIntentity;
	public float highIntensityAmount;

	public playerWeaponsManager weaponsManager;
	public playerWeaponSystem weaponManager;

	public float lightRotationSpeed = 10;

	bool highIntensityActivated;

	public AudioSource mainAudioSource;
	float lastTimeUsed;
	Transform mainCameraTransform;
	float originalIntensity;
	Quaternion targetRotation;

	void Start ()
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (weaponManager == null) {
			weaponManager = GetComponent<playerWeaponSystem> ();
		}

		if (mainFlashlight == null) {
			mainFlashlight = mainLight.GetComponent<Light> ();
		}

		originalIntensity = mainFlashlight.intensity;
	}

	void Update ()
	{
		if (isActivated) {
			if (mainCameraTransform) {
				if (!weaponManager.weaponIsMoving () && (weaponManager.aimingInThirdPerson || weaponManager.carryingWeaponInFirstPerson)
					&& !weaponsManager.isEditinWeaponAttachments ()) {
					targetRotation = Quaternion.LookRotation (mainCameraTransform.forward);
					mainLight.transform.rotation = Quaternion.Slerp (mainLight.transform.rotation, targetRotation, Time.deltaTime * lightRotationSpeed);

					//mainLight.transform.rotation = targetRotation;
				} else {
					targetRotation = Quaternion.identity;
					mainLight.transform.localRotation = Quaternion.Slerp (mainLight.transform.localRotation, targetRotation, Time.deltaTime * lightRotationSpeed);

					//mainLight.transform.localRotation = targetRotation;
				}
			} else {
				mainCameraTransform = weaponManager.getMainCameraTransform ();
			}

			if (infiniteEnergy) {
				return;
			}

			if (Time.time > lastTimeUsed + useEnergyRate) {
				if (weaponManager.remainAmmoInClip () && !weaponManager.reloading) {
					lastTimeUsed = Time.time;
					weaponManager.useAmmo (amountEnergyUsed);
				}
				if (!weaponManager.remainAmmoInClip () || weaponManager.reloading) {
					setFlashlightState (false);
					reloading = true;
				}
			}
		} else {
			if (reloading) {
				if (weaponManager.remainAmmoInClip () && weaponManager.carryingWeapon () && !weaponManager.reloading) {
					setFlashlightState (true);
					reloading = false;
				}
			}
		}
		
	}

	public void changeFlashLightState ()
	{
		setFlashlightState (!isActivated);
	}

	public void setFlashlightState (bool state)
	{
		isActivated = state;
		playSound (isActivated);
		mainLight.SetActive (isActivated);
	}

	public void turnOn ()
	{
		isActivated = true;
		playSound (isActivated);
	}

	public void turnOff ()
	{
		isActivated = false;
		playSound (isActivated);
	}

	public void playSound (bool state)
	{
		if (useSound) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);
			if (state) {
				mainAudioSource.PlayOneShot (turnOnSound);
			} else {
				mainAudioSource.PlayOneShot (turnOffSound);
			}
		}
	}

	public void changeLightIntensity (bool state)
	{
		if (useHighIntentity) {
			highIntensityActivated = state;
			if (highIntensityActivated) {
				mainFlashlight.intensity = highIntensityAmount;
			} else {
				mainFlashlight.intensity = originalIntensity;
			}
		}
	}

	public void setHighIntensity ()
	{
		changeLightIntensity (true);
	}

	public void setOriginalIntensity ()
	{
		changeLightIntensity (false);
	}
}
