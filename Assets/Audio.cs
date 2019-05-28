using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour {


	public SECTR_Door door;
	public AudioSource winSound;
	private bool playWin;
	// Use this for initialization
	void Start () {
		playWin = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (door.IsFullyOpen() && !playWin)
			winSound.Play();
			playWin = true;

	}
}
