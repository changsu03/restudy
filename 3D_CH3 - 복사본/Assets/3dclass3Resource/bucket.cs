    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bucket : MonoBehaviour {

    GameObject director;

    // Use this for initialization
    void Start () {
        this.director = GameObject.Find("Director");
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetMouseButtonDown (0)) {
            Ray light = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit spot;

             if(Physics.Raycast (light, out spot, Mathf.Infinity)) {
                float x = Mathf.RoundToInt(spot.point.x);
                float z = Mathf.RoundToInt(spot.point.z);
                transform.position = new Vector3(x, 0.0f, z);
            }

        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "gold") {
            this.director.GetComponent<Director>().GetScore();
        } else {
            this.director.GetComponent<Director>().loseScore();
        }
        Destroy(other.gameObject);
    }
}
