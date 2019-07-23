using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public float health = 30.0f;

    public void TargetDamage(float amount) {

        health -= amount;
        if (health <= 0f) {

            Die();


        }
    }
    void Die() {
        Destroy(gameObject);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
