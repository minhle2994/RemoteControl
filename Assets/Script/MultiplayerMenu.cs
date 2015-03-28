using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MultiplayerMenu : Photon.MonoBehaviour {
	//private RoomInfo[] roomsList;
    void  Start (){
    	PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectUsingSettings("v1.0");
	}
    
    void Update(){
    	
    }
   	
	public void onClick(){
		GameObject input = GameObject.FindGameObjectWithTag("CodeInput");
		Text code = input.GetComponent<Text>();
		
		if (code != null){
			PhotonNetwork.JoinRoom(code.text);
				
		}
	}
		
	void OnReceivedRoomListUpdate(){
		//roomsList = PhotonNetwork.GetRoomList();
	}
	
	void OnJoinedLobby(){
		
	}
	
	void OnJoinedRoom(){
		GameObject input = GameObject.FindGameObjectWithTag("InputField");
		GameObject button = GameObject.FindGameObjectWithTag("EnterButton");	
		input.SetActive(false);
		button.SetActive(false);
	}
	
	void OnPhotonJoinRoomFailed(){
		GameObject input = GameObject.FindGameObjectWithTag("Feedback");
		Text code = input.GetComponent<Text>();
		code.text = "Invalid code";
		StartCoroutine(FadeOut());			
	}
	
	IEnumerator FadeOut() {
		yield return new WaitForSeconds(2);
		GameObject input = GameObject.FindGameObjectWithTag("Feedback");
		Text code = input.GetComponent<Text>();
		code.text = "";
	}	
		
	[RPC]
	void StartGame()
	{
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel+1);
	}
	
	[RPC]
	void Waiting()
	{	
		GameObject text = GameObject.FindGameObjectWithTag("Text");
		text.GetComponent<Text>().text = "Waiting for other player";		
	}	
}