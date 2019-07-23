using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpsController : MonoBehaviour
{

    float moveSpeed = 5.0f;
    float rotSpeed = 5.0f;

    float range = 100.0f;
    float damage = 10.0f;

    public Camera fpsCam;
    public ParticleSystem MuzzleEffect;

    public GameObject impactParticle;

    

    void Shoot () {

        MuzzleEffect.Play ();
        RaycastHit hit;

        if(Physics.Raycast (fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();

            if (target != null) {
                target.TargetDamage(damage);
            }

            GameObject impactGO = Instantiate(impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
        }

    }
    // Update is called once per frame
    void Update()
    {
        moveCtrl();
        rotCtrl();
        if(Input.GetMouseButtonDown (0)) {
            Shoot();
        }
    }
    void moveCtrl() {

        if (Input.GetKey(KeyCode.W))
        {

            this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }   
        else if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
    void rotCtrl() {
        float rotX = Input.GetAxis("Mouse Y") * rotSpeed;
        float rotY = Input.GetAxis("Mouse X") * rotSpeed;

        rotX = Mathf.Clamp(rotX, -80, 80);

        this.transform.localRotation *= Quaternion.Euler(0, rotY, 0);
        fpsCam.transform.localRotation *= Quaternion.Euler(-rotX, 0, 0);
    }
}

