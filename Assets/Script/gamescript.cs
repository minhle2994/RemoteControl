/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
 *  This project is available on the Unity Store. You are only allowed to use these
 *  resources if you've bought them from the Unity Assets Store.
 */
using UnityEngine;
using System.Collections;

public class gamescript : MonoBehaviour {
	private bool sent = false;
	private int numberOfClientReady = 0;
	private bool twoPlayerConnected = false;
	
	void  Awake (){
		//RE-enable the network messages now we've loaded the right level
		Network.isMessageQueueRunning = true;
		
		if(Network.isServer){
			Debug.Log("Server registered the game at the masterserver.");
			MultiplayerFunctions.SP.RegisterHost(GameSettings.serverTitle, GameSettings.description);
		}
	}
	
	
	void  OnGUI (){
		//Debug.Log("waiting scene");
		AutoResize(800, 480);
		if (Network.peerType == NetworkPeerType.Disconnected){
			//We are currently disconnected: Not a client or host
			GUILayout.Label("Connection status: We've (been) disconnected");
			if(GUILayout.Button("Back to main menu")){
				Application.LoadLevel(Application.loadedLevel-1);
			}
			
		}else{
			//We've got a connection(s)!
			
			
			if (Network.peerType == NetworkPeerType.Connecting){
				
				GUILayout.Label("Connection status: Connecting");
				
			} else if (Network.peerType == NetworkPeerType.Client){
				
				GUILayout.Label("Connection status: Client!");
				GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );	
				if (twoPlayerConnected == true && sent == false){
					if (GUILayout.Button("Ready", GUILayout.Width(150))){
						networkView.RPC("Ready", RPCMode.Server);
						sent = true;
					}
				}	
				
			} else if (Network.peerType == NetworkPeerType.Server){
				
				GUILayout.Label("Connection status: Server!");
				GUILayout.Label("Connections: "+Network.connections.Length);
				if(Network.connections.Length>=1){
					GUILayout.Label("Ping to first player: "+Network.GetAveragePing(  Network.connections[0] ) );
					if (Network.connections.Length == 1){
						GUILayout.Label("Waiting for other player");	
					}
					if (Network.connections.Length == 2 && sent == false){
						sent = true;
						GUILayout.Label("Press Ready to enter game");
						networkView.RPC("IsReady", RPCMode.Others);
					}
					if (numberOfClientReady == 2){
						numberOfClientReady = 0;
						networkView.RPC("StartGame", RPCMode.Others);
						Network.isMessageQueueRunning = false;
						Application.LoadLevel(Application.loadedLevel+1);
					}
				}
			}
			
			if (GUILayout.Button ("Disconnect"))
			{
				Network.Disconnect();
			}
		}
		
		
	}
	
	
	void  OnDisconnectedFromServer ( NetworkDisconnection info  ){
		if (Network.isServer) {
			Debug.Log("Local server connection disconnected");
		}else {
			if (info == NetworkDisconnection.LostConnection)
				Debug.Log("Lost connection to the server");
			else
				Debug.Log("Successfully diconnected from the server");
		}
	}
	
	
	//Server functions called by Unity
	void  OnPlayerConnected ( NetworkPlayer player  ){
		Debug.Log("Player connected from: " + player.ipAddress +":" + player.port);
	}
	
	void  OnPlayerDisconnected ( NetworkPlayer player  ){
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
		
	}
	
	public static void AutoResize(int screenWidth, int screenHeight)
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
	}
	
	[RPC]
	void IsReady()
	{
		twoPlayerConnected = true;
	}
	
	[RPC]
	void Ready()
	{
		numberOfClientReady = numberOfClientReady + 1;
	}
	
	[RPC]
	void StartGame()
	{
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel+1);
	}
}