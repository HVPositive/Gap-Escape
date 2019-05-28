using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPad : MonoBehaviour {

    public float springForce;


    private Rigidbody launchObject;

    private Vector3 direction;

	// Use this for initialization
	void Start () {
		direction = GetComponent<Transform>().up;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "PickUp") {
            launchObject = other.gameObject.GetComponent<Rigidbody>();

        }

        if (other.gameObject.tag == "Switch" ) {
           launchObject.velocity = new Vector3(0.0f,0.0f,0.0f);
            launchObject.AddForce(direction*springForce, ForceMode.Impulse);
            GetComponent<AudioSource>().Play();
        }
    }
}
