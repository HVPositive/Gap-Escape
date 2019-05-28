using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	public GameObject leftGap;	
	public GameObject rightGap;
	public GameObject portholePrefab;

	public Transform hitIndicator;

	private Transform location;
	private Rigidbody rb;

	private Vector3 hitLoc;
	private Quaternion hitRot;

    public GameObject porthole;

	private float sTime;
	private Vector3 startSize;
	private Vector3 endSize;
	private float lerpLength;

	// Use this for initialization
	void Start () {
		location = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();

		sTime = -1.0f;

		startSize = new Vector3(0.4f, 0.4f, 0.05f);
		endSize = new Vector3(8.0f, 8.0f, 1.0f);
		lerpLength = Vector3.Distance (startSize, endSize);
	}
	
	// Update is called once per frame
	void Update () {
		location = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();


		if (sTime != -1.0f){

			//interpolate balloon
			float dist = (Time.time - sTime);
			float fractionDist = 40.0f * dist / lerpLength;

			porthole.GetComponent<Transform>().localScale = Vector3.Lerp (startSize, endSize, fractionDist);

			if (porthole.GetComponent<Transform>().localScale.x >= 8.0f) {
				sTime = -1.0f;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{

        if (other.gameObject.tag == "Port"){


			
			if (other.name != "Plane" && other.name != "Ceiling"){
				
				hitLoc = new Vector3(location.position.x, location.position.y, location.position.z);
				hitRot = other.gameObject.GetComponent<Transform>().rotation;
				Invoke ("MovePortHole", 2f);
				

			}
            //Quaternion.LookRotation(other.gameObject.GetComponent<Transform>().up, -other.gameObject.GetComponent<Transform>().forward));
			//hitIndicator.SetPositionAndRotation(location.position, Quaternion.LookRotation(other.gameObject.GetComponent<Transform>().up, -other.gameObject.GetComponent<Transform>().forward)); 
		}

		if (other.gameObject.tag == "Left Gap" && rightGap.activeSelf){

			GapTravel(rightGap);
		}
		else if (other.gameObject.tag == "Right Gap" && leftGap.activeSelf){
			
			GapTravel(leftGap);



		}
	}

	void GapTravel (GameObject destination){
				Transform newLoc = destination.GetComponent<Transform>();
				GetComponent<AudioSource>().Play();

				//Move it a bit from the button so it doesn't get stuck
				location.SetPositionAndRotation( newLoc.position + newLoc.up*4 , location.rotation * newLoc.rotation);
				float speed = rb.velocity.magnitude;
				
				rb.velocity = speed * newLoc.up;

	}


	void MovePortHole(){
		sTime = Time.time;
		porthole.GetComponent<Transform>().localScale = startSize;
		porthole.GetComponent<Transform>().SetPositionAndRotation(hitLoc, hitRot);
		if (!porthole.active)
			porthole.SetActive(true);
	}
}
