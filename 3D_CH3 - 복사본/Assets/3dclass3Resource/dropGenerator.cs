using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropGenerator : MonoBehaviour {

    public GameObject prefabglod;
    public GameObject prefabskull;
    float sp = 1.0f;
    float dt = 0;
    int interval = 2;
    float Speed = -0.03f;

    public void SetParameter(float sp, float Speed, int interval) {
        this.sp = sp;
        this.Speed = Speed;
        this.interval = interval;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.dt += Time.deltaTime;
        if(this.dt > this.sp) {
            this.dt = 0;
            GameObject item;
            int dice = Random.Range(1, 11);
            if (dice <= this.interval) {
                item = Instantiate(prefabskull) as GameObject;
            } else {

                item = Instantiate(prefabglod) as GameObject;
            }
            float x = Random.Range(-1, 2);
            float z = Random.Range(-1, 2);
            item.transform.position = new Vector3(x, 4, z);
            item.GetComponent<Controller> ().dropping = this.Speed;
        }
	}
}
