using UnityEngine;
using System.Collections;

public class BackControll : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void Back(){
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel-1);	
	}
}
