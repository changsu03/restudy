using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster : MonoBehaviour
{
    int speed = 15;
    public GameObject 기는몬스터;
    void Start()
    {

    }
    
    
    void Update()
    {
        float fMove = Time.deltaTime * speed;
    transform.Translate(Vector3.forward * fMove);
        Destroy(gameObject, 100.1f);
    }
}
