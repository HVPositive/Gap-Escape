using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour {

	//The door this pad opens
	public SECTR_Door door;

    public bool singleActivate;

	public Material on;
	public Material off;



	private MeshRenderer mr;

	public PressurePad [] linkedPads;
	public PressurePad mainLink;
	private bool state;
	private bool doorState;
	void Start () {
		
		mr = GetComponent<MeshRenderer>();
		state = false;
		doorState = false;
	}
	
	// Update is called once per frame
	void Update () {
		mr = GetComponent<MeshRenderer>();

	
	}

      

    //These OnTrigger functions open and close the door depending on if the pad is touching the
    //trigger switch
	void OnTriggerExit(Collider other)
		{
            //checks if door should be closed if trigger but stay activated
            if (!singleActivate && linkedPads.Length>0) {
                if (other.gameObject.tag == "Switch") {
					
					if (door!=null){
						
						if (!singleActivate)
							state = false;
                    	CheckLinks();
					}
					else{
						if (!singleActivate)
							state=false;

						mainLink.CheckLinks();
					}

					
					mr.material = off;
                }
            }
		}

	void OnTriggerEnter(Collider other)
	{

        
		if (other.gameObject.tag == "Switch"){

			if (door!=null){
				GetComponents<AudioSource>()[1].Play();
				state = true;
				CheckLinks();
			} else{
				GetComponents<AudioSource>()[1].Play();
				state=true;
				mainLink.CheckLinks();
			}
			mr.material = on;


		}
	}


	void CheckLinks(){

		//no other linked pads, simply operate doors
		if (linkedPads.Length == 0){
			if (state){
				GetComponents<AudioSource>()[0].Play();
				door.OpenDoor();

			}else if (!doorState && !door.IsClosed()) {
				door.CloseDoor();
			}

		} else{
			//check the state of linked

			doorState = state;
			for (int i =0; i<linkedPads.Length; i++){
				if (linkedPads[i].GetState() ==false){
					doorState = false;
				}

			}

			if (doorState){
				GetComponents<AudioSource>()[0].Play();
				door.OpenDoor();
			} else if (!doorState && !door.IsClosed()) {
				door.CloseDoor();
			}

		}

	}


	bool GetState(){
		return state;

	}
}
