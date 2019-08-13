using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class launchedObjects : MonoBehaviour
{
	float timer = 0;
	GameObject currentPlayer;
	Collider currentPlayerCollider;
	Collider mainCollider;

	//this script is for the objects launched by the player, to check the object which collides with them

	void Update ()
	{
		//if the launched objects does not collide with other object, remove the script
		if (timer > 0) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				activateCollision ();
			}
		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (!col.collider.isTrigger) {
			//if the object has a health script, it reduces the amount of life according to the launched object velocity
			float damage = GetComponent<Rigidbody> ().velocity.magnitude;
			if (damage == 0) {
				damage = col.relativeVelocity.magnitude;
			}
			if (col.collider.GetComponent<characterDamageReceiver> () || col.collider.GetComponent<vehicleDamageReceiver> ()) {
				applyDamage.checkHealth (gameObject, col.collider.gameObject, damage, -transform.forward, transform.position, currentPlayer, false, true);
				if (col.collider.GetComponent<vehicleDamageReceiver> ()) {
					GameObject vehicle = col.collider.GetComponent<vehicleDamageReceiver> ().vehicle;
					Vector3 collisionDirection = (col.contacts [0].point - transform.position).normalized;
					vehicle.GetComponent<Rigidbody> ().AddForce (collisionDirection * damage * vehicle.GetComponent<Rigidbody> ().mass, ForceMode.Impulse);
				}
				activateCollision ();
			}
			//else, set the timer to disable the script
			else {
				timer = 1;
			}
		}
	}

	void activateCollision ()
	{
		if (currentPlayerCollider) {
			mainCollider = GetComponent<Collider> ();
			if (mainCollider) {
				Physics.IgnoreCollision (mainCollider, currentPlayerCollider, false);
			}
		}

		Destroy (GetComponent<launchedObjects> ());
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
	}

	public void setCurrentPlayerAndCollider (GameObject player, Collider playerCollider)
	{
		currentPlayer = player;
		currentPlayerCollider = playerCollider;
	}
}