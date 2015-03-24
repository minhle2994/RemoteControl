using UnityEngine;
using System.Collections;

public class PlayAndPause : MonoBehaviour {
	public GameObject pad;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pad == null){
			pad = GameObject.FindGameObjectWithTag("GamePad");
			if (pad == null){
				GameObject pauseButton = GameObject.FindGameObjectWithTag("PauseButton");
				GameObject playButton = GameObject.FindGameObjectWithTag("PlayButton");
				//playButton.SetActive(false);						
			}
		}
			
	}
	
	public void Pause(){
		pad.GetComponent<Control>().OnPause();
	}
	
	public void Resume(){
		pad.GetComponent<Control>().OnResume();
	}
}
