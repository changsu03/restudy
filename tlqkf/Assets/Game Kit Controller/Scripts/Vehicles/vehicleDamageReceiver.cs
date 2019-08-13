using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class vehicleDamageReceiver : MonoBehaviour
{
	[Range (1, 10)] public float damageMultiplier = 1;
	public GameObject vehicle;
	public vehicleHUDManager hudManager;

	//this script is added to every collider in a vehicle, so when a projectile hits the vehicle, its health component receives the damge
	//like this the damage detection is really accurated.
	//the function sends the amount of damage, the direction of the projectile, the position where hits, the object that fired the projectile,
	//and if the damaged is done just once, like a bullet, or the damaged is constant like a laser

	//health and damage management
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, bool damageConstant, bool searchClosestWeakSpot)
	{
		hudManager.setDamage (amount * damageMultiplier, fromDirection, damagePos, bulletOwner, projectile, damageConstant, searchClosestWeakSpot);
	}

	public void setHeal (float amount)
	{
		hudManager.getHealth (amount);
	}

	public float getCurrentHealthAmount ()
	{
		return hudManager.getCurrentHealthAmount ();
	}

	public float getMaxHealthAmount ()
	{
		return hudManager.getMaxHealthAmount ();
	}

	public float getAuxHealthAmount ()
	{
		return hudManager.getAuxHealthAmount ();
	}

	public void addAuxHealthAmount (float amount)
	{
		hudManager.addAuxHealthAmount (amount);
	}

	//fuel management
	public float getCurrentFuelAmount ()
	{
		return hudManager.getCurrentFuelAmount ();
	}

	public float getMaxFuelAmount ()
	{
		return hudManager.getMaxFuelAmount ();
	}

	public void getFuel (float amount)
	{
		hudManager.getFuel (amount);
	}

	public void removeFuel (float amount)
	{
		hudManager.removeFuel (amount);
	}

	//destroy vehicle
	public void destroyVehicle ()
	{
		hudManager.destroyVehicle ();
	}

	//impact decal management
	public int getDecalImpactIndex ()
	{
		return hudManager.getDecalImpactIndex ();
	}

	//energy management
	public void getEnergy (float amount)
	{
		hudManager.getEnergy (amount);
	}

	public void removeEnergy (float amount)
	{
		hudManager.removeEnergy (amount);
	}

	public float getCurrentEnergyAmount ()
	{
		return hudManager.getCurrentEnergyAmount ();
	}

	public float getMaxEnergyAmount ()
	{
		return hudManager.getMaxEnergyAmount ();
	}

	public float getAuxEnergyAmount ()
	{
		return hudManager.getAuxEnergyAmount ();
	}

	public void addAuxEnergyAmount (float amount)
	{
		hudManager.addAuxEnergyAmount (amount);
	}

	public bool isVehicleDestroyed ()
	{
		return hudManager.destroyed;
	}

	//set vehicle component
	public void setVehicle (GameObject vehicleGameObject)
	{
		vehicle = vehicleGameObject;
		hudManager = vehicle.GetComponent<vehicleHUDManager> ();

		updateComponent ();
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath)
	{
		hudManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
	}

	public void removeDamagetTargetOverTimeState ()
	{
		hudManager.stopDamageOverTime ();
	}

	public vehicleHUDManager getHUDManager ()
	{
		return hudManager;
	}

	public void updateComponent ()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty (this);
		#endif
	}
}