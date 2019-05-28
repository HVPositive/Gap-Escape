using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour {



	void Start(){


	}
	void OnTriggerEnter (Collider sceneChanger){


	}
		
    //Function activated by the controller. Changes scene depending on what is selected.
    //Things with the tag "level" should have names that will load a certain scene.
	public static void SelectLevel(string level){


		if (level == "Exit" || level == "Finish") {
			SceneManager.LoadScene("lobby", LoadSceneMode.Single);

		} else if (level == "Basic") {
			SceneManager.LoadScene("basic level", LoadSceneMode.Single);
		} else if (level == "Spring") {
            SceneManager.LoadScene("spring level", LoadSceneMode.Single);
        } else if (level == "Gap") {
            SceneManager.LoadScene("gap level", LoadSceneMode.Single);
        } else if (level == "Final") {
            SceneManager.LoadScene("final level", LoadSceneMode.Single);
		} else if (level == "Restart"){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}


	}

}
