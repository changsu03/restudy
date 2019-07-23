using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public float dropping = -0.03f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        if (tag == "gold")
        {
            transform.Translate(0, 0, this.dropping);
        }
        if (tag == "skull")
        {
            transform.Translate(0, this.dropping, 0);
        }
        if (transform.position.y < -1.0f)
        {
            Destroy(gameObject);

        }
    }
}
