using UnityEngine;
using System.Collections;

public class characterDamageReceiver : MonoBehaviour
{
	[Range (1, 20)] public float damageMultiplier = 1;
	public GameObject character;
	public health healthManager;

	//this script is added to every collider in a vehicle, so when a projectile hits the vehicle, its health component receives the damge
	//like this the damage detection is really accurated.
	//the function sends the amount of damage, the direction of the projectile, the position where hits, the object that fired the projectile,
	//and if the damaged is done just once, like a bullet, or the damaged is constant like a laser

	//health and damage management
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, bool damageConstant, bool searchClosestWeakSpot)
	{
		healthManager.setDamage ((amount * damageMultiplier), fromDirection, damagePos, bulletOwner, projectile, damageConstant, searchClosestWeakSpot);
	}

	public void setHeal (float amount)
	{
		healthManager.getHealth (amount);
	}

	public float getCurrentHealthAmount ()
	{
		return healthManager.getCurrentHealthAmount ();
	}

	public float getMaxHealthAmount ()
	{
		return healthManager.getMaxHealthAmount ();
	}

	public float getAuxHealthAmount ()
	{
		return healthManager.getAuxHealthAmount ();
	}

	public void addAuxHealthAmount (float amount)
	{
		healthManager.addAuxHealthAmount (amount);
	}

	public float getHealthAmountToLimit ()
	{
		return healthManager.getHealthAmountToLimit ();
	}

	//kill character
	public void killCharacter (GameObject projectile, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		healthManager.killCharacter (direction, position, attacker, projectile, damageConstant);
	}

	//impact decal management
	public int getDecalImpactIndex ()
	{
		return healthManager.getDecalImpactIndex ();
	}

	public bool isCharacterDead ()
	{
		return healthManager.dead;
	}

	//set character component
	public void setCharacter (GameObject characterGameObject)
	{
		character = characterGameObject;
		healthManager = character.GetComponent<health> ();
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath)
	{
		healthManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath);
	}

	public void removeDamagetTargetOverTimeState ()
	{
		healthManager.stopDamageOverTime ();
	}

	public void sedateCharacter (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		healthManager.sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
	}

	public health getHealthManager ()
	{
		return healthManager;
	}

	void OnCollisionEnter (Collision col)
	{
		if (!healthManager.isDead () && healthManager.canRagdollReceiveDamageOnImpact ()) {
			healthManager.setImpactReceivedInfo (col.relativeVelocity, col.collider);
		}
	}
}