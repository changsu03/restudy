    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bucket : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "gold") {
            Debug.Log("Tag==gold");
        } else {
            Debug.Log("Tag==skull");
        }
    }

    // Use this for initialization
    void Start () {
		
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
}
