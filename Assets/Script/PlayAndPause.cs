using UnityEngine;
using System.Collections;

public class PlayAndPause : MonoBehaviour {
	public GameObject pad;
	void Start () {
		
	}
	
	void Update () {
		if (pad == null){
			pad = GameObject.FindGameObjectWithTag("GamePad");
//			if (pad == null){
//				GameObject pauseButton = GameObject.FindGameObjectWithTag("PauseButton");
//				GameObject playButton = GameObject.FindGameObjectWithTag("PlayButton");
//				playButton.SetActive("false");
//			}
		}	
	}
	
	public void Pause(){
		pad.GetComponent<Control>().OnPause();
	}
	
	public void Resume(){
		pad.GetComponent<Control>().OnResume();
	}
}
