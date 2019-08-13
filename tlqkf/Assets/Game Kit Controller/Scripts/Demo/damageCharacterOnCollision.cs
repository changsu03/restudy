using UnityEngine;
using System.Collections;

public class damageCharacterOnCollision : MonoBehaviour
{
	public bool addForceToRigidbodies = true;
	public float forceAmountToRigidbodies = 1000;
	public bool killCharacterOnCollision = true;
	public bool pushCharacterOnCollision;
	public bool applyDamageWhenPushCharacter;
	public float extraForceOnCollision;

	public bool damageEnabled = true;

	void OnCollisionEnter (Collision col)
	{
		if (!damageEnabled) {
			return;
		}

		if (addForceToRigidbodies) {
			if (applyDamage.canApplyForce (col.gameObject)) {
				col.rigidbody.AddExplosionForce (forceAmountToRigidbodies, col.transform.position, 100);
			}
		}
		if (killCharacterOnCollision) {
			//applyDamage.killCharacter (col.gameObject);
			float damage = applyDamage.getCurrentHealthAmount (col.gameObject);
			applyDamage.checkHealth (gameObject, col.gameObject, damage, transform.forward, col.contacts [0].point, gameObject, false, true);
		} else {
			if (pushCharacterOnCollision) {
				Vector3 pushDirection = (col.contacts [0].point - transform.position).normalized;
				if (extraForceOnCollision > 0) {
					pushDirection *= extraForceOnCollision;
				}
				applyDamage.pushCharacter (col.gameObject, pushDirection);
				if (applyDamageWhenPushCharacter) {
					float damage = col.relativeVelocity.magnitude;
					applyDamage.checkHealth (gameObject, col.gameObject, damage, transform.forward, col.contacts [0].point, gameObject, false, true);
				}
			}
		}
	}

	public void setDamageEnabledState (bool state)
	{
		damageEnabled = state;
	}

	public void disableDamage ()
	{
		setDamageEnabledState (false);
	}

	public void enableDamage ()
	{
		setDamageEnabledState (true);
	}
}
