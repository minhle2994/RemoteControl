﻿using UnityEngine;
using System.Collections;

public class BackNextControll : MonoBehaviour {
	public GameObject startPoint;
	// Use this for initialization
	void Start () {
		startPoint = GameObject.Find("StartPoint");
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public void Back(){
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel-1);	
	}
	
	public void Next(){
		Generate genScript = startPoint.GetComponent<Generate>();	
		genScript.Skip();
	}
}
