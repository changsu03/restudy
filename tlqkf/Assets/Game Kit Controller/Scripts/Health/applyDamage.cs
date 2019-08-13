using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class applyDamage : MonoBehaviour
{
	//check if the collided object has a health component, and apply damage to it
	//any object with health component will be damaged, so the friendly fire is allowed
	//also, check if the object is a vehicle to apply damage too
	public static void checkHealth (GameObject projectile, GameObject objectToDamage, float damageAmount, Vector3 direction, Vector3 position, GameObject projectileOwner, 
	                                bool damageConstant, bool searchClosestWeakSpot)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return;
		} 

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return;
		} 

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return;
		}

		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			healthToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return;
		}

//		Emerald_AI Emerald_AIToCheck = objectToDamage.GetComponent<Emerald_AI> ();
//		if (Emerald_AIToCheck) {
//			Emerald_AIToCheck.Damage ((int)damageAmount, Emerald_AI.TargetType.Player);
//		}
	}

	public static bool checkCanBeDamage (GameObject projectile, GameObject objectToDamage, float damageAmount, Vector3 direction, Vector3 position, GameObject projectileOwner,
	                                     bool damageConstant, bool searchClosestWeakSpot)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return true;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return true;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.setDamage (damageAmount, direction, position, projectileOwner, projectile, damageConstant, searchClosestWeakSpot);
			return true;
		}
		return false;
	}

	public static bool checkIfDead (GameObject objectToCheck)
	{
		health healthToCheck = objectToCheck.GetComponent<health> ();
		if (healthToCheck) {
			if (healthToCheck.getCurrentHealthAmount () <= 0) {
				return true;
			}
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			if (characterDamageReceiverToCheck.getCurrentHealthAmount () <= 0) {
				return true;
			}
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			if (vehicleDamageReceiverToCheck.hudManager.getAuxHealthAmount () <= 0) {
				return true;
			}
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			if (vehicleHUDManagerToCheck.getCurrentHealthAmount () <= 0) {
				return true;
			}
		}
		return false;
	}

	public static bool checkIfMaxHealth (GameObject objectToCheck)
	{
		vehicleHUDManager vehicleHUDManagerToChecK = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToChecK) {
			if (vehicleHUDManagerToChecK.getCurrentHealthAmount () >= vehicleHUDManagerToChecK.getMaxHealthAmount ()) {
				return true;
			}
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			if (vehicleDamageReceiverToCheck.getCurrentHealthAmount () >= vehicleDamageReceiverToCheck.getMaxHealthAmount ()) {
				return true;
			}
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			if (characterDamageReceiverToCheck.getCurrentHealthAmount () >= characterDamageReceiverToCheck.getMaxHealthAmount ()) {
				return true;
			} 
		}

		health healthToCheck = objectToCheck.GetComponent<health> ();
		if (healthToCheck) {
			if (healthToCheck.getCurrentHealthAmount () >= healthToCheck.getMaxHealthAmount ()) {
				return true;
			} 
		}

		return false;
	}

	public static void setDamageTargetOverTimeState (GameObject objectToDamage, float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                 float damageOverTimeRate, bool damageOverTimeToDeath)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
			return;
		} 

		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			healthToCheck.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
			return;
		} 

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
			return;
		}
	}

	public static void removeDamagetTargetOverTimeState (GameObject objectToDamage)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.removeDamagetTargetOverTimeState ();
			return;
		} 

		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			healthToCheck.stopDamageOverTime ();
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.removeDamagetTargetOverTimeState ();
			return;
		} 

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.stopDamageOverTime ();
			return;
		}
	}

	public static void sedateCharacter (GameObject objectToDamage, Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
			return;
		} 

		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			healthToCheck.sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
			return;
		}
	}

	//manage health, energy and fuel
	public static void setHeal (float healAmount, GameObject objectToHeal)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToHeal.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.setHeal (healAmount);
			return;
		}

		health healthToCheck = objectToHeal.GetComponent<health> ();
		if (healthToCheck) {
			healthToCheck.getHealth (healAmount);
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToHeal.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.setHeal (healAmount);
			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToHeal.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.getHealth (healAmount);
			return;
		}
	}

	public static void setEnergy (float energyAmount, GameObject objectToRecharge)
	{
		otherPowers otherPowersToCheck = objectToRecharge.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			otherPowersToCheck.getEnergy (energyAmount);
			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToRecharge.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.getEnergy (energyAmount);
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToRecharge.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.getEnergy (energyAmount);
			return;
		}
	}

	public static void removeEnergy (float energyAmount, GameObject objectToRecharge)
	{
		otherPowers otherPowersToCheck = objectToRecharge.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			otherPowersToCheck.removeEnergy (energyAmount);
			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToRecharge.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.removeEnergy (energyAmount);
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToRecharge.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.removeEnergy (energyAmount);
			return;
		}
	}

	public static void setFuel (float fuelAmount, GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.getFuel (fuelAmount);
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = vehicle.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.getFuel (fuelAmount);
			return;
		}
	}

	public static void removeFuel (float fuelAmount, GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.removeFuel (fuelAmount);
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = vehicle.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.removeFuel (fuelAmount);
			return;
		}
	}

	public static bool canApplyForce (GameObject objectToCheck)
	{
		bool canReceiveForce = false;
		Rigidbody RigidbodyToCheck = objectToCheck.GetComponent<Rigidbody> ();
		if (RigidbodyToCheck) {
			if (!RigidbodyToCheck.isKinematic) {
				canReceiveForce = true;
				characterDamageReceiver damageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();
				if (damageReceiver) {
					if (damageReceiver.character.tag == "Player" || damageReceiver.character.tag == "enemy" || damageReceiver.character.tag == "friend") {
						canReceiveForce = false;
					}
				}
			}
		}
		return canReceiveForce;
	}

	public static Rigidbody applyForce (GameObject objectToCheck)
	{
		Rigidbody RigidbodyToCheck = objectToCheck.GetComponent<Rigidbody> ();
		if (RigidbodyToCheck) {
			if (!RigidbodyToCheck.isKinematic) {
				characterDamageReceiver damageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();
				if (damageReceiver) {
					if (damageReceiver.isCharacterDead ()) {
						return RigidbodyToCheck;
					} else if (damageReceiver.character.tag != "Player" && damageReceiver.character.tag != "enemy" && damageReceiver.character.tag != "friend") {
						return damageReceiver.character.GetComponent<Rigidbody> ();
					}
				} else {
					vehicleDamageReceiver vehicleDamage = objectToCheck.GetComponent<vehicleDamageReceiver> ();
					if (vehicleDamage) {
						Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();
						if (vehicleRigidbody) {
							if (!vehicleRigidbody.isKinematic) {
								return vehicleRigidbody;
							}
						}
					} else {
						vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
						if (vehicleHUDManagerToCheck) {
							Rigidbody vehicleRigidbody = objectToCheck.GetComponent<Rigidbody> ();
							if (vehicleRigidbody) {
								if (!vehicleRigidbody.isKinematic) {
									return vehicleRigidbody;
								}
							}
						} else {
							return RigidbodyToCheck;
						}
					}
				}
			}
		} else {
			vehicleDamageReceiver vehicleDamage = objectToCheck.GetComponent<vehicleDamageReceiver> ();
			if (vehicleDamage) {
				Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();
				if (vehicleRigidbody) {
					if (!vehicleRigidbody.isKinematic) {
						return vehicleRigidbody;
					}
				}
			} else {
				vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
				if (vehicleHUDManagerToCheck) {
					Rigidbody vehicleRigidbody = vehicleHUDManagerToCheck.gameObject.GetComponent<Rigidbody> ();
					if (vehicleRigidbody) {
						if (!vehicleRigidbody.isKinematic) {
							return vehicleRigidbody;
						}
					}
				}
			}
		}
		return null;
	}

	//healt management
	public static float getCurrentHealthAmount (GameObject character)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.getCurrentHealthAmount ();
		}

		health healthToCheck = character.GetComponent<health> ();
		if (healthToCheck) {
			return healthToCheck.getCurrentHealthAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getCurrentHealthAmount ();
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = character.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.getCurrentHealthAmount ();
		}

		return 0;
	}

	public static float getMaxHealthAmount (GameObject character)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.getMaxHealthAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getMaxHealthAmount ();
		}

		return 0;
	}

	public static float getAuxHealthAmount (GameObject character)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.getAuxHealthAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getAuxHealthAmount ();
		}
		return 0;
	}

	public static void addAuxHealthAmount (GameObject character, float amount)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.addAuxHealthAmount (amount);
			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.addAuxHealthAmount (amount);
			return;
		}
	}

	public static float getHealthAmountToPick (GameObject character, float amount)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = characterDamageReceiverToCheck.getHealthAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);
				characterDamageReceiverToCheck.addAuxHealthAmount (totalAmmoAmountToAdd);
			}
			return totalAmmoAmountToAdd;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = vehicleHUDManagerToCheck.getHealthAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);
				vehicleHUDManagerToCheck.addAuxHealthAmount (totalAmmoAmountToAdd);
			}
			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static void killCharacter (GameObject projectile, GameObject objectToDamage, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			characterDamageReceiverToCheck.killCharacter (projectile, direction, position, attacker, damageConstant);
			return;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			vehicleDamageReceiverToCheck.destroyVehicle ();
			return;
		} 

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.destroyVehicle ();
			return;
		}
	}

	public static Transform getPlaceToShoot (GameObject objectToDamage)
	{
		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			return healthToCheck.getPlaceToShoot ();
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.character.GetComponent<health> ().getPlaceToShoot ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getPlaceToShoot ();
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.hudManager.getPlaceToShoot ();
		}
		return null;
	}

	public static GameObject getPlaceToShootGameObject (GameObject objectToDamage)
	{
		health healthToCheck = objectToDamage.GetComponent<health> ();
		if (healthToCheck) {
			return healthToCheck.getPlaceToShoot ().gameObject;
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToDamage.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.character.GetComponent<health> ().getPlaceToShoot ().gameObject;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToDamage.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getPlaceToShoot ().gameObject;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToDamage.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.hudManager.getPlaceToShoot ().gameObject;
		}
		return null;
	}

	public static bool isCharacter (GameObject objectToCheck)
	{
		health healthToCheck = objectToCheck.GetComponent<health> ();
		if (healthToCheck) {
			return true;
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return true;
		}

		return false;
	}

	public static bool isVehicle (GameObject objectToCheck)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return true;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return true;
		}
		return false;
	}

	public static float getCharacterHeight (GameObject objectToCheck)
	{
		playerController currentPlayerController = objectToCheck.GetComponent<playerController> ();
		if (currentPlayerController) {
			return currentPlayerController.getCharacterHeight ();
		}
		return -1;
	}

	public static playerController getPlayerControllerComponent (GameObject objectToCheck)
	{
		playerController currentPlayerController = objectToCheck.GetComponent<playerController> ();
		if (currentPlayerController) {
			return currentPlayerController;
		}
		return null;
	}

	public static List<health.weakSpot> getCharacterWeakSpotList (GameObject objectToCheck)
	{
		health healthToCheck = objectToCheck.GetComponent<health> ();
		if (healthToCheck) {
			return healthToCheck.advancedSettings.weakSpots;
		}

		characterDamageReceiver characterDamageReceiverToCheck = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.character.GetComponent<health> ().advancedSettings.weakSpots;
		}

		return null;
	}

	//energy management
	public static float getCurrentEnergyAmount (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			return otherPowersToCheck.getCurrentEnergyAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getCurrentEnergyAmount ();
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = character.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.getCurrentEnergyAmount ();
		}
		return 0;
	}

	public static float getMaxEnergyAmount (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			return otherPowersToCheck.getMaxEnergyAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getMaxEnergyAmount ();
		}
		return 0;
	}

	public static float getAuxEnergyAmount (GameObject character)
	{

		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			return otherPowersToCheck.getAuxEnergyAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getAuxEnergyAmount ();
		}
		return 0;
	}

	public static void addAuxEnergyAmount (GameObject character, float amount)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			otherPowersToCheck.addAuxEnergyAmount (amount);
			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.addAuxEnergyAmount (amount);
			return;
		}
	}

	public static float getEnergyAmountToPick (GameObject character, float amount)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = otherPowersToCheck.getEnergyAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				//print (totalAmmoAmountToAdd);

				otherPowersToCheck.addAuxEnergyAmount (totalAmmoAmountToAdd);
			}
			return totalAmmoAmountToAdd;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = vehicleHUDManagerToCheck.getEnergyAmountToLimit ();

			if (amountToRefill > 0) {
				//print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				//print (totalAmmoAmountToAdd);

				vehicleHUDManagerToCheck.addAuxEnergyAmount (totalAmmoAmountToAdd);
			}
			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static bool checkIfMaxEnergy (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();
		if (otherPowersToCheck) {
			if (otherPowersToCheck.getCurrentEnergyAmount () >= otherPowersToCheck.getMaxEnergyAmount ()) {
				return true;
			}
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			if (vehicleHUDManagerToCheck.getCurrentEnergyAmount () >= vehicleHUDManagerToCheck.getMaxEnergyAmount ()) {
				return true;
			}
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = character.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			if (vehicleDamageReceiverToCheck.getCurrentEnergyAmount () >= vehicleDamageReceiverToCheck.getMaxEnergyAmount ()) {
				return true;
			}
		}

		return false;
	}

	//vehicle fuel management
	public static float getCurrentFuelAmount (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getCurrentFuelAmount ();
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = vehicle.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.getCurrentFuelAmount ();
		}
		return 0;
	}

	public static float getMaxFuelAmount (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getMaxFuelAmount ();
		}
		return 0;
	}

	public static float getAuxFuelAmount (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getAuxFuelAmount ();
		}
		return 0;
	}

	public static void addAuxFuelAmount (GameObject vehicle, float amount)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			vehicleHUDManagerToCheck.addAuxFuelAmount (amount);
		}
	}

	public static float getFuelAmountToPick (GameObject vehicle, float amount)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = vehicleHUDManagerToCheck.getFuelAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);
				vehicleHUDManagerToCheck.addAuxFuelAmount (totalAmmoAmountToAdd);
			}
			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static bool checkIfMaxFuel (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			if (vehicleHUDManagerToCheck.getCurrentFuelAmount () >= vehicleHUDManagerToCheck.getMaxFuelAmount ()) {
				return true;
			}
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = vehicle.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			if (vehicleDamageReceiverToCheck.getCurrentFuelAmount () >= vehicleDamageReceiverToCheck.getMaxFuelAmount ()) {
				return true;
			}
		}

		return false;
	}

	//character ragdoll push management
	public static void pushCharacter (GameObject character, Vector3 direction)
	{
		health currentCharacter = character.GetComponent<health> ();
		if (currentCharacter) {
			if (currentCharacter.advancedSettings.haveRagdoll) {
				ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();
				if (currentRagdollActivator) {
					currentRagdollActivator.pushCharacter (direction);
				}
			}
		}
	}

	public static bool checkIfCharacterCanBePushedOnExplosions (GameObject character)
	{
		bool value = false;
		health currentCharacter = character.GetComponent<health> ();
		if (currentCharacter) {
			if (currentCharacter.advancedSettings.haveRagdoll && currentCharacter.advancedSettings.allowPushCharacterOnExplosions) {
				value = true;
			}
		}
		return value;
	}

	public static void pushRagdoll (GameObject character, Vector3 direction)
	{
		ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();
		if (currentRagdollActivator) {
			currentRagdollActivator.pushCharacter (direction);
		}
	}

	public static void pushAnyCharacter (GameObject character, float pushCharacterForce, float pushCharacterRagdollForce, Vector3 direction)
	{
		Rigidbody RigidbodyToCheck = character.GetComponent<Rigidbody> ();
		if (RigidbodyToCheck) {
			if (!RigidbodyToCheck.isKinematic) {
				characterDamageReceiver damageReceiver = character.GetComponent<characterDamageReceiver> ();
				if (damageReceiver) {
					if (damageReceiver.isCharacterDead ()) {
						RigidbodyToCheck.AddForce (direction * pushCharacterForce * RigidbodyToCheck.mass, ForceMode.Impulse);
					} else {
						health healthToCheck = damageReceiver.healthManager;
						if (healthToCheck) {
							if (healthToCheck.advancedSettings.haveRagdoll) {
								ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();
								if (currentRagdollActivator) {
									currentRagdollActivator.pushCharacter (direction * pushCharacterRagdollForce);
								}
							}
						}
					}
				} else {
					vehicleDamageReceiver vehicleDamage = character.GetComponent<vehicleDamageReceiver> ();
					if (vehicleDamage) {
						Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();
						if (vehicleRigidbody) {
							if (!vehicleRigidbody.isKinematic) {
								vehicleRigidbody.AddForce (direction * pushCharacterForce * vehicleRigidbody.mass, ForceMode.Impulse);
							}
						}
					} else {
						RigidbodyToCheck.AddForce (direction * pushCharacterForce * RigidbodyToCheck.mass, ForceMode.Impulse);
					}
				}
			}
		} else {
			vehicleDamageReceiver vehicleDamage = character.GetComponent<vehicleDamageReceiver> ();
			if (vehicleDamage) {
				Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();
				if (vehicleRigidbody) {
					if (!vehicleRigidbody.isKinematic) {
						vehicleRigidbody.AddForce (direction * pushCharacterForce * vehicleRigidbody.mass, ForceMode.Impulse);
					}
				}
			}
		}
	}

	public static void setExplosion (Vector3 explosionPosition, float explosionRadius, bool userLayerMask, LayerMask layerToSearch, GameObject explosionOwner, 
	                                 bool canDamageProjectileOwner, GameObject projectileGameObject, bool killObjectsInRadius, bool isExplosive, bool isImplosive, 
	                                 float explosionDamage, bool pushCharacters, bool applyExplosionForceToVehicles, float explosionForceToVehiclesMultiplier, 
	                                 float explosionForce, ForceMode forceMode, bool checkObjectsInsideExplosionOwner, Transform explosionOwnerTransform)
	{
		List<Collider> colliders = new List<Collider> ();

		List<Rigidbody> vehiclesRigidbodyFoundList = new List<Rigidbody> ();

		if (userLayerMask) {
			colliders.AddRange (Physics.OverlapSphere (explosionPosition, explosionRadius, layerToSearch));
		} else {
			colliders.AddRange (Physics.OverlapSphere (explosionPosition, explosionRadius));
		}

		Collider ownerCollider = explosionOwner.GetComponent<Collider> ();

		foreach (Collider currentCollider in colliders) {
			if (currentCollider != null &&
			    (canDamageProjectileOwner || ownerCollider != currentCollider) &&
			    (!checkObjectsInsideExplosionOwner || !currentCollider.gameObject.transform.IsChildOf (explosionOwnerTransform))) {

				GameObject objectDetected = currentCollider.gameObject;

				Vector3 objectPosition = objectDetected.transform.position;

				Vector3 explosionDirection = objectPosition - explosionPosition;
				if (isImplosive) {
					explosionDirection = explosionPosition - objectPosition;
				}
				explosionDirection = explosionDirection / explosionDirection.magnitude;

				if (killObjectsInRadius) {
					applyDamage.killCharacter (projectileGameObject, objectDetected, explosionDirection, objectPosition, explosionOwner, false);
				} else {
					applyDamage.checkHealth (projectileGameObject, objectDetected, explosionDamage, explosionDirection, objectPosition, explosionOwner, false, false);

					if (pushCharacters) {
						if (applyDamage.checkIfCharacterCanBePushedOnExplosions (objectDetected)) {
							applyDamage.pushRagdoll (objectDetected, explosionDirection);
						}
					}
				}

				if (applyExplosionForceToVehicles) {
					Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectDetected);
					if (objectToDamageMainRigidbody) {
						if (!vehiclesRigidbodyFoundList.Contains (objectToDamageMainRigidbody)) {
							bool isVehicle = applyDamage.isVehicle (objectDetected);

							float finalExplosionForce = explosionForce;
							if (isVehicle) {
								finalExplosionForce *= explosionForceToVehiclesMultiplier;
							}

							if (isExplosive) {
								objectToDamageMainRigidbody.AddExplosionForce (finalExplosionForce * objectToDamageMainRigidbody.mass, explosionPosition, explosionRadius, 3, forceMode);
							}

							if (isImplosive) {
								Vector3 Dir = explosionPosition - objectPosition;
								Vector3 Dirscale = Vector3.Scale (Dir.normalized, projectileGameObject.transform.localScale);
								objectToDamageMainRigidbody.AddForce (Dirscale * finalExplosionForce * objectToDamageMainRigidbody.mass, forceMode);
							}

							if (isVehicle) {
								vehiclesRigidbodyFoundList.Add (objectToDamageMainRigidbody);
							}
						}
					}
				} else {
					if (applyDamage.canApplyForce (objectDetected)) {
						Rigidbody currentHitRigidbody = currentCollider.GetComponent<Rigidbody> ();

						//explosion type
						if (isExplosive) {
							currentHitRigidbody.AddExplosionForce (explosionForce * currentHitRigidbody.mass, explosionPosition, explosionRadius, 3, forceMode);
						}

						//implosion type
						if (isImplosive) {
							Vector3 Dir = explosionPosition - objectPosition;
							Vector3 Dirscale = Vector3.Scale (Dir.normalized, projectileGameObject.transform.localScale);
							currentHitRigidbody.AddForce (Dirscale * explosionForce * currentHitRigidbody.mass, forceMode);
						}
					}
				}
			}
		}
	}

	//Check if an object is a vehicle or a character
	public static GameObject getCharacterOrVehicle (GameObject objectToCheck)
	{
		characterDamageReceiver characterDamageReceiverToCheck = objectToCheck.GetComponent<characterDamageReceiver> ();
		if (characterDamageReceiverToCheck) {
			return characterDamageReceiverToCheck.character;
		}

		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.vehicle;
		}
		return null;
	}

	public static GameObject getVehicle (GameObject objectToCheck)
	{
		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.vehicle;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.gameObject;
		}

		return null;
	}

	public static vehicleHUDManager getVehicleHUDManager (GameObject objectToCheck)
	{
		vehicleDamageReceiver vehicleDamageReceiverToCheck = objectToCheck.GetComponent<vehicleDamageReceiver> ();
		if (vehicleDamageReceiverToCheck) {
			return vehicleDamageReceiverToCheck.getHUDManager ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck;
		}

		return null;
	}

	//Audio Source Management
	public static AudioSource getAudioSource (GameObject objectToCheck, string audioSourceName)
	{
		playerStatesManager playerStatesManagerToCheck = objectToCheck.GetComponent<playerStatesManager> ();
		if (playerStatesManagerToCheck) {
			return playerStatesManagerToCheck.getAudioSourceElement (audioSourceName);
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();
		if (vehicleHUDManagerToCheck) {
			return vehicleHUDManagerToCheck.getAudioSourceElement (audioSourceName);
		}
		return null;
	}

	//Weapons management
	public static int getPlayerWeaponAmmoAmountToPick (playerWeaponsManager weaponsManager, string weaponName, int ammoAmuntToAdd)
	{
		int totalAmmoAmountToAdd = 0;
		int weaponIndex = -1;
		for (int i = 0; i < weaponsManager.weaponsList.Count; i++) {
			if (weaponsManager.weaponsList [i].getWeaponSystemName () == weaponName && weaponsManager.weaponsList [i].isWeaponEnabled ()) {
				weaponIndex = i;
			}
		}

		if (weaponIndex > -1) {
			playerWeaponSystem currentWeapon = weaponsManager.weaponsList [weaponIndex].getWeaponSystemManager ();
			if (currentWeapon.canIncreaseRemainAmmo ()) {
				
				int amountToRefill = currentWeapon.ammoAmountToMaximumLimit ();
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = ammoAmuntToAdd;
				if (amountToRefill < totalAmmoAmountToAdd) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);

				if (totalAmmoAmountToAdd > 0) {
					currentWeapon.addAuxRemainAmmo (totalAmmoAmountToAdd);
				}
			}
		}
		return totalAmmoAmountToAdd;
	}

	public static int getVehicleWeaponAmmoAmountToPick (vehicleWeaponSystem weaponsManager, string weaponName, int ammoAmuntToAdd)
	{
		int totalAmmoAmountToAdd = 0;
		int weaponIndex = -1;
		for (int i = 0; i < weaponsManager.weapons.Count; i++) {
			if (weaponsManager.weapons [i].Name == weaponName && weaponsManager.weapons [i].enabled) {
				weaponIndex = i;
			}
		}

		if (weaponIndex > -1) {
			vehicleWeaponSystem.vehicleWeapons currentWeapon = weaponsManager.weapons [weaponIndex];
			if (weaponsManager.canIncreaseRemainAmmo (currentWeapon)) {

				int amountToRefill = weaponsManager.ammoAmountToMaximumLimit (currentWeapon);
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = ammoAmuntToAdd;
				if (amountToRefill < totalAmmoAmountToAdd) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);

				if (totalAmmoAmountToAdd > 0) {
					weaponsManager.addAuxRemainAmmo (currentWeapon, totalAmmoAmountToAdd);
				}
			}
		}
		return totalAmmoAmountToAdd;
	}

	//Noises Management
	public static void sendNoiseSignal (float noiseRadius, Vector3 noisePosition, LayerMask noiseLyaer, bool showNoiseDetectionGizmo)
	{
		Collider[] colliders = Physics.OverlapSphere (noisePosition, noiseRadius, noiseLyaer);

		if (showNoiseDetectionGizmo) {
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.right * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.left * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.forward * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.back * noiseRadius + Vector3.up, Color.yellow, 2);
		}

		if (colliders.Length > 0) {
			for (int i = 0; i < colliders.Length; i++) {
				findObjectivesSystem currentFindObjectivesSystem = colliders [i].GetComponent<findObjectivesSystem> ();
				if (currentFindObjectivesSystem) {
					currentFindObjectivesSystem.checkNoisePosition (noisePosition);
				}
			}
		}
	}
}