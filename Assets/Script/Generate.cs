using UnityEngine;
using System.Collections;
using System.Net;

public class Generate : Photon.MonoBehaviour {
	public GameObject gamepadPrefab;
	public GameObject uiPrefab;
	public GameObject pad;
	public bool hasFinishTut = false;
	
	void  Awake (){
		PhotonNetwork.isMessageQueueRunning = true;
	}
	
	void Start () {
	}
	
	void Update () {
		if (pad == null)
			pad = GameObject.FindGameObjectWithTag("GamePad");
			
		if (hasFinishTut == true && pad != null){
			VCButtonBase actionButton = VCButtonBase.GetInstance("Action");
			if (actionButton.Pressed){
				photonView.RPC("Select", PhotonTargets.Others);						
				PhotonNetwork.isMessageQueueRunning = false;
				Application.LoadLevel(Application.loadedLevel+1);
			}
		}
		
		if (PhotonNetwork.playerList.Length < 3){
			PhotonNetwork.isMessageQueueRunning = false;
			Application.LoadLevel(0);				
		}
	}
		
	public void Skip(){
		photonView.RPC("SkipTut", PhotonTargets.Others);			
	}
				
	[RPC]
	void Spawn(int viewID)
	{
		GameObject clone = Instantiate(gamepadPrefab) as GameObject;
		PhotonView nView;
		nView = clone.GetComponent<PhotonView>();
		nView.viewID = viewID;	
		float scaleX = (float) Screen.width / 800;
		float scaleY = (float) Screen.height / 480;
		
		GameObject actionButton = clone.transform.Find("ActionButton").gameObject;
		GameObject pressed = actionButton.transform.Find("a_pressed").gameObject;
		GameObject up = actionButton.transform.Find("a_up").gameObject;
		Rect newSize = new Rect(pressed.guiTexture.pixelInset.x * scaleX, 
		                        pressed.guiTexture.pixelInset.y * scaleY,
		                        pressed.guiTexture.pixelInset.width * scaleX,
		                        pressed.guiTexture.pixelInset.height * scaleY);
		pressed.guiTexture.pixelInset = newSize;                      
		up.guiTexture.pixelInset = newSize;
		
		GameObject moveJoyStick = clone.transform.Find("MoveJoyStick").gameObject;
		GameObject back = moveJoyStick.transform.Find("analog_back").gameObject;
		GameObject front = moveJoyStick.transform.Find("analog_front").gameObject;
		newSize = new Rect(back.guiTexture.pixelInset.x * scaleX, 
		                   back.guiTexture.pixelInset.y * scaleY,
		                   back.guiTexture.pixelInset.width * scaleX,
		                   back.guiTexture.pixelInset.height * scaleY);
		back.guiTexture.pixelInset = newSize;                      
		front.guiTexture.pixelInset = newSize;		
		Instantiate(uiPrefab);			                                         						                                         			
	}
		
	[RPC]
	void FinishTut()
	{
		hasFinishTut = true;
	}
	
	[RPC]
	void Select()
	{
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel+1);
	}
	
	[RPC]
	void SkipTut()
	{
	}	
}