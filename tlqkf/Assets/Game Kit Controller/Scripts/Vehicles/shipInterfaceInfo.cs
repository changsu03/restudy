using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class shipInterfaceInfo : MonoBehaviour
{
	public bool interfaceEnabled;

	public GameObject vehicle;

	public bool compassEnabled;
	public GameObject interfaceCanvas;
	public RectTransform compassBase;
	public RectTransform north;
	public RectTransform south;
	public RectTransform east;
	public RectTransform west;
	public RectTransform altitudeMarks;
	public Text pitchValue;
	public Text yawValue;
	public Text rollValue;
	public Text altitudeValue;
	public Text velocityValue;
	public Text coordinateXValue;
	public Text coordinateYValue;
	public Text coordinateZValue;
	public RectTransform level;
	public float altitudeMarkSpeed;
	public Slider healthBar;
	public Slider energyBar;
	public Slider fuelBar;
	public Text weaponName;
	public Text weaponAmmo;
	public Text canLand;
	public Text enginesState;

	public GameObject healthContent;
	public GameObject energyContet;
	public GameObject fuelContent;
	public GameObject weaponContent;

	public vehicleHUDManager HUDManager;
	public vehicleGravityControl gravityManager;
	public vehicleWeaponSystem weaponManager;
	public Rigidbody mainRigidbody;

	int compassDirection;
	int compassDirectionAux;
	Vector3 normal;
	float currentSpeed;
	bool showHealth;
	bool showEnergy;
	bool showFuel;
	bool hasWeapons;
	Transform vehicleTransform;

	void Start ()
	{
		if (weaponManager) {
			if (!weaponManager.enabled) {
				weaponContent.SetActive (false);
			} else {
				hasWeapons = true;
			}
		} else {
			weaponContent.SetActive (false);
		}
		healthBar.maxValue = HUDManager.healthAmount;
		healthBar.value = healthBar.maxValue;
		energyBar.maxValue = HUDManager.boostAmount;
		energyBar.value = energyBar.maxValue;

		if (!HUDManager.invincible) {
			showHealth = true;
		} else {
			healthContent.SetActive (false);
		}

		if (!HUDManager.infiniteBoost) {
			showEnergy = true;
		} else {
			energyContet.SetActive (false);
		}

		if (HUDManager.vehicleUseFuel && !HUDManager.infiniteFuel) {
			showFuel = true;
		} else {
			fuelContent.SetActive (false);
		}

		vehicleTransform = vehicle.transform;
		enableOrDisableInterface (false);

	}

	void Update ()
	{
		if (interfaceEnabled) {
			currentSpeed = mainRigidbody.velocity.magnitude;
			if (compassEnabled) {
				compassDirection = (int)Mathf.Abs (vehicleTransform.eulerAngles.y);
				if (compassDirection > 360) {
					compassDirection = compassDirection % 360;
				}
				compassDirectionAux = compassDirection;
				if (compassDirectionAux > 180) {
					compassDirectionAux = compassDirectionAux - 360;
				}
				north.anchoredPosition = new Vector2 (-compassDirectionAux * 2, 0);
				south.anchoredPosition = new Vector2 (-compassDirection * 2 + 360, 0);
				east.anchoredPosition = new Vector2 (-compassDirectionAux * 2 + 180, 0);
				west.anchoredPosition = new Vector2 (-compassDirection * 2 + 540, 0);
				normal = gravityManager.currentNormal;
				if (altitudeMarks) {
					float angleX = Mathf.Asin (vehicleTransform.InverseTransformDirection (Vector3.Cross (normal.normalized, vehicleTransform.up)).x) * Mathf.Rad2Deg;
					altitudeMarks.anchoredPosition = Vector2.MoveTowards (altitudeMarks.anchoredPosition, new Vector2 (0, angleX), Time.deltaTime * altitudeMarkSpeed);
				}
			}

			if (pitchValue) {
				pitchValue.text = vehicleTransform.eulerAngles.x.ToString ("0") + " º";
			}

			if (yawValue) {
				yawValue.text = vehicleTransform.eulerAngles.y.ToString ("0") + " º";
			}

			if (rollValue) {
				rollValue.text = vehicleTransform.eulerAngles.z.ToString ("0") + " º";
			}

			if (altitudeValue) {
				altitudeValue.text = vehicleTransform.position.y.ToString ("0") + " m";
			}

			if (velocityValue) {
				velocityValue.text = currentSpeed.ToString ("0") + " km/h";
			}

			if (coordinateXValue) {
				coordinateXValue.text = vehicleTransform.position.x.ToString ("0"); 
			}

			if (coordinateYValue) {
				coordinateYValue.text = vehicleTransform.position.y.ToString ("0"); 
			}

			if (coordinateZValue) {
				coordinateZValue.text = vehicleTransform.position.z.ToString ("0"); 
			}

			if (level) {
				level.localEulerAngles = new Vector3 (0, 0, vehicleTransform.eulerAngles.z);
			}

			if (hasWeapons && weaponManager) {
				weaponName.text = weaponManager.currentWeapon.Name;
				weaponAmmo.text = weaponManager.currentWeapon.clipSize.ToString () + "/" + weaponManager.currentWeapon.remainAmmo.ToString ();
			}

			if (showHealth) {
				healthBar.value = HUDManager.getCurrentHealthAmount ();
			}

			if (showEnergy) {
				energyBar.value = HUDManager.getCurrentEnergyAmount ();
			}

			if (showFuel) {
				fuelBar.value = HUDManager.getCurrentFuelAmount ();
			}
		}
	}

	public void enableOrDisableInterface (bool state)
	{
		interfaceEnabled = state;
		interfaceCanvas.SetActive (interfaceEnabled);
	}

	public void shipCanLand (bool state)
	{
		if (state) {
			canLand.text = "YES";
		} else {
			canLand.text = "NO";
		}
	}

	public void shipEnginesState (bool state)
	{
		if (state) {
			enginesState.text = "ON";
		} else {
			enginesState.text = "OFF";
		}
	}
}