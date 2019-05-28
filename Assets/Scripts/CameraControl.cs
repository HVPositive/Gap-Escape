using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour {

	public float speed;

	private float horizontal;
	private float vertical;


    public GameObject menu;



	public GameObject indicator;

	public GameObject gap;
	public GameObject gap2;

	public Material outline;

    //Messages to the headset
	public Text message;

	private Transform controllerLoc;
	private LineRenderer pointer;

    //Level to change to
	private string newLevel;

    //Door that is currently open - if not pointed at, will close and become null
	private GameObject openDoor;

    //Joint to pickup objects
	private ConfigurableJoint cj;

	//private Vector3 anchorOrg;

    private float holdSpeed;

    private GameObject highlighted;


	private float sTime1;
	private float sTime2;
	private Vector3 startSize;
	private Vector3 endSize;
	private float lerpLength;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		controllerLoc = GetComponent<Transform>();
		pointer = GetComponent<LineRenderer>();

		cj = GetComponent<ConfigurableJoint>();
        holdSpeed = 0.75f;
		
		sTime1 = -1.0f;
		sTime2 = -1.0f;

		startSize = new Vector3(0.8f, 0.1f, 0.8f);
		endSize = new Vector3(16.0f, 2.0f, 16.0f);
		lerpLength = Vector3.Distance (startSize, endSize);
		pointer.alignment = LineAlignment.Local;
		
	}

	void LateUpdate(){


	}

	// Update is called once per frame
	void Update () {


        controllerLoc = GetComponent<Transform>();
        pointer = GetComponent<LineRenderer>();

        cj = GetComponent<ConfigurableJoint>();


		 horizontal += speed * Input.GetAxis("Mouse X");
		 vertical -= speed * Input.GetAxis("Mouse Y");
		 
		 transform.eulerAngles = new Vector3(vertical, horizontal, 0.0f); 

		if (sTime1 != -1.0f){

			//interpolate balloon
			float dist = (Time.time - sTime1);
			float fractionDist = 40.0f * dist / lerpLength;

			gap.GetComponent<Transform>().localScale = Vector3.Lerp (startSize, endSize, fractionDist);

			if (gap.GetComponent<Transform>().localScale.x >= 16.0f) {
				sTime1 = -1.0f;
			}
		}
		if (sTime2 != -1.0f){

			//interpolate balloon
			float dist = (Time.time - sTime2);
			float fractionDist = 40.0f * dist / lerpLength;

			gap2.GetComponent<Transform>().localScale = Vector3.Lerp (startSize, endSize, fractionDist);

			if (gap2.GetComponent<Transform>().localScale.x >= 16.0f) {
				sTime2 = -1.0f;
			}
		}

        //if object is being held
        if  ( cj.connectedBody!=null){
			
            //If laser is not enabled, turn on indicator of where object will drop
            if (!pointer.enabled) {
				Transform indicatorLoc = indicator.GetComponent<Transform>(); 
				Vector3 heldLoc = cj.connectedBody.GetComponent<Transform>().position;
                indicator.SetActive(true);


				RaycastHit hit;
				//Ray landingRay = new Ray(heldLoc, new Vector3(heldLoc.x, 0.0f, heldLoc.z) );

				if (Physics.Raycast(heldLoc, new Vector3(0.0f, -1.0f, 0.0f), out hit))
				{
					//indicatorLoc.localPosition = new Vector3(heldLoc.x, hit.point.y, heldLoc.z);

					indicatorLoc.SetPositionAndRotation( new Vector3(heldLoc.x, hit.point.y, heldLoc.z), hit.collider.gameObject.GetComponent<Transform>().rotation );
				}





				//indicatorLoc.localPosition = new Vector3(heldLoc.x, 0.0f, heldLoc.z);
            } else{

				indicator.SetActive(false);
			}



			//Release/drop object


			if (Input.GetKeyDown("q") ) {
				cj.connectedBody = null;
				cj.autoConfigureConnectedAnchor = true;
				indicator.SetActive(false);
			 

            //Move held object forward along the laser
  			} else if (Input.GetKeyDown("w") ){
				//if (Vector3.Distance(controllerLoc.position,cj.connectedBody.position) < 42.0f)
					cj.anchor = cj.anchor + new Vector3(0.0f,0.0f,holdSpeed);

				

			 
            //Move held object backwards (towards user) along the laser
            } else if (Input.GetKeyDown("s")){
				//Debug.Log(Vector3.Distance(controllerLoc.position,cj.connectedBody.position));
				//if (Vector3.Distance(controllerLoc.position,cj.connectedBody.position) > 2.0f)
					cj.anchor = cj.anchor - new Vector3(0.0f,0.0f,holdSpeed);
				
			} 



			
		}


        //Enable/disable laser when 
		if (Input.GetKey("d"))
		{
			MakePointer();
		}
		else
		{
			pointer.enabled = false;

		}

		if (pointer.enabled)
		{
			RaycastHit hit;
			Ray landingRay = new Ray(pointer.GetPosition(0), pointer.GetPosition(1));

			if (Physics.Raycast(landingRay, out hit))
			{

				
				if (highlighted != null && hit.collider != highlighted ){
					StopHighlight(highlighted);

				}


				if (hit.collider.gameObject != openDoor && openDoor != null){
					openDoor.GetComponent<SECTR_Door>().CloseDoor();
					if (openDoor.GetComponent<SECTR_Door>().IsClosed()){
						GetComponents<AudioSource>()[2].Play();
						openDoor = null;
					}
				}

				if (hit.collider.tag == "Level" )
				{
					HighlightObject(hit);

					if (Input.GetKeyDown("e")  )
					{	
						SetNewLevel(hit.collider.name);


					}

				} else if (hit.collider.tag == "Menu"){
					HighlightMulti(hit);
					openDoor = hit.collider.gameObject ;
					
					openDoor.GetComponent<SECTR_Door>().OpenDoor();

					if (openDoor.GetComponent<SECTR_Door>().IsFullyOpen())
						GetComponents<AudioSource>()[1].Play();

					if (Input.GetKeyDown("e") && openDoor.GetComponent<SECTR_Door>().IsFullyOpen() ) 
					{	
						if (SceneManager.GetActiveScene().buildIndex == 0)
							SetNewLevel(hit.collider.name);
						else
							menu.SetActive(true);
					}

				} else if (hit.collider.tag == "Close"){
					HighlightObject(hit);

                    if (Input.GetKeyDown("e"))
					    menu.SetActive(false);

				} else if (hit.collider.tag == "PickUp"){
					HighlightObject(hit);
					if (Input.GetKeyDown("e") )
					{	
					cj.connectedBody =hit.collider.gameObject.GetComponent<Rigidbody> ();
					//anchorOrg = cj.anchor;
					cj.autoConfigureConnectedAnchor = false;
					}
				} else if (hit.collider.tag == "End"){
					HighlightMulti(hit);
					if (Input.GetKeyDown("e") )
					{	
						newLevel = hit.collider.gameObject.name;
						message.text = "Level Completed. Returning to lobby.";
						Invoke ("ChangeLevel", 5f);
					}


				} else if (hit.collider.tag == "Porthole"){

					if (Input.GetMouseButtonDown(0) ){
						SetupGap("Porthole", hit, gap);
					} else if (Input.GetMouseButtonDown(1)){
						SetupGap("Porthole", hit,gap2);
					
					}



				} else if (hit.collider.tag == "Port"){
					if (Input.GetMouseButtonDown(0) ){
						SetupGap("Port", hit, gap);
					} else if (Input.GetMouseButtonDown(1)){
						SetupGap("Port", hit,gap2);
					
					}



				}


			}


		} else{
            if (highlighted!= null)
			    StopHighlight(highlighted);

            if (openDoor != null){

				openDoor.GetComponent<SECTR_Door>().CloseDoor();
				if (openDoor.GetComponent<SECTR_Door>().IsClosed())
						openDoor = null;
			}

		} 
		


	}



	//Create laser
	void MakePointer()
	{
		pointer.enabled = true;
		Vector3 pos = controllerLoc.transform.position;
		Vector3 forward = controllerLoc.transform.forward;
		pointer.SetPosition(0, new Vector3(pos.x, pos.y-0.1f, pos.z) );
		pointer.SetPosition(1, new Vector3(forward.x, forward.y, forward.z) * 100 + pos);

        

	}

	void SetNewLevel(string name){
		newLevel = name;

		if (newLevel == "Restart")
			message.text = "Restarting level, please wait."; 
		else
			message.text = "Changing level, please wait.";
		Invoke ("ChangeLevel", 5f);
	}

	void ChangeLevel(){
		LevelChange.SelectLevel (newLevel);
		message.text = "";
	}

	//Recreate configurablejoint on break
	void OnJointBreak(float breakForce)
	{
		cj =gameObject.AddComponent<ConfigurableJoint>() as ConfigurableJoint;
        cj.breakForce = 10000;
		cj.anchor = new Vector3(0.0f, 0.0f, 0.0f);
		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;
		cj.angularXMotion = ConfigurableJointMotion.Locked;
		cj.angularYMotion = ConfigurableJointMotion.Locked;
		cj.angularZMotion = ConfigurableJointMotion.Locked;
		cj.autoConfigureConnectedAnchor = true;

	}


	//
	 //Highlight the hit objects
	//Only things with the tags: End, Level, Menu, PickUp should be highlightable
    void HighlightObject(RaycastHit hit)
    {
        MeshRenderer selectedMesh = hit.collider.GetComponent<MeshRenderer>();
        Material[] mats = selectedMesh.materials;

        //selectedMesh.materials.SetValue(outline, 0);
        mats.SetValue(outline, 1);
        selectedMesh.materials = mats;

        if (highlighted != null)
        	StopHighlight(highlighted);

        highlighted = hit.collider.gameObject;


    }
    //For hightlighting objects made up of more than one object (eg. doors)
    void HighlightMulti (RaycastHit hit) {

        MeshRenderer selectedMesh = hit.collider.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();
        Material[] mats = selectedMesh.materials;
        mats.SetValue(outline, 1);
        selectedMesh.materials = mats;

        //repeat the above for other child
        selectedMesh = hit.collider.GetComponent<Transform>().GetChild(1).GetComponent<MeshRenderer>();
        mats = selectedMesh.materials;
        mats.SetValue(outline, 1);
        selectedMesh.materials = mats;


        if (highlighted != null)
            StopHighlight(highlighted);

        highlighted = hit.collider.gameObject;
    }

    //Stop highlighting the given object
    void StopHighlight(GameObject hObject) {


        if (hObject.tag == "Menu" || hObject.tag == "End") {
            MeshRenderer selectedMesh = hObject.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();
            Material[] mats = selectedMesh.materials;

            selectedMesh.materials.SetValue(outline, 0);
            mats.SetValue(null, 1);
            selectedMesh.materials = mats;


            selectedMesh = hObject.GetComponent<Transform>().GetChild(1).GetComponent<MeshRenderer>();
            mats = selectedMesh.materials;

            selectedMesh.materials.SetValue(outline, 0);
            mats.SetValue(null, 1);
            selectedMesh.materials = mats;


        } else {
            MeshRenderer selectedMesh = hObject.GetComponent<MeshRenderer>();
            Material[] mats = selectedMesh.materials;

            selectedMesh.materials.SetValue(outline, 0);
            mats.SetValue(null, 1);
            selectedMesh.materials = mats;
        }

        highlighted = null;
               


    }


	void SetupGap(string type, RaycastHit hit, GameObject setGap){
		GetComponents<AudioSource>()[0].Play();

		if (setGap == gap)
			sTime1 = Time.time;
		else if (setGap == gap2)
			sTime2 = Time.time;
		
		setGap.SetActive(true);
		Transform gapPos = setGap.GetComponent<Transform>();
		 
		Transform hitTransform = hit.collider.gameObject.GetComponent<Transform> ();
		gapPos.GetComponentInChildren<SpringJoint>().spring = 0.0f;
		Quaternion rot = Quaternion.LookRotation(hitTransform.up, -hitTransform.forward);
		gapPos.GetComponentInChildren<SpringJoint>().autoConfigureConnectedAnchor = false;

		gapPos.localScale = startSize;

		if (type == "Porthole")

			if (gap.name == "Ceiling Porthole")
				gapPos.SetPositionAndRotation(hitTransform.position + hitTransform.up*0.01f , Quaternion.LookRotation(hitTransform.forward, hitTransform.up));
			else
				gapPos.SetPositionAndRotation(hitTransform.position + hitTransform.up*0.01f, rot);


		else if (type == "Port"){
				if (hit.collider.gameObject.name == "Plane")
					gapPos.SetPositionAndRotation(hit.point  + hitTransform.up * 0.01f, Quaternion.LookRotation(hitTransform.forward, hitTransform.up));
				else
					gapPos.SetPositionAndRotation(hit.point + hitTransform.up *0.01f, rot);
		}
		
		gapPos.GetComponentInChildren<SpringJoint>().autoConfigureConnectedAnchor = true;
		//gapPos.GetComponentInChildren<SpringJoint>().spring = 1000.0f;
	}

}
