using UnityEngine;
using System.Collections;

public class Generate : MonoBehaviour {
	public GameObject miloPrefab;
	public GameObject otisPrefab;
	public GameObject gamepadPrefab;
	
	void  Awake (){
		//RE-enable the network messages now we've loaded the right level
		Network.isMessageQueueRunning = true;
		
		if(Network.isServer){
			Debug.Log("Server registered the game at the masterserver.");
			MultiplayerFunctions.SP.RegisterHost(GameSettings.serverTitle, GameSettings.description);
		}
	}
	
	// Use this for initialization
	void Start () {
		if (Network.isServer){
			NetworkViewID viewID = Network.AllocateViewID();
			GameObject clone = Instantiate(miloPrefab) as GameObject;
			NetworkView nView;
			nView = clone.GetComponent<NetworkView>();
			nView.viewID = viewID;			
			foreach (NetworkPlayer player in Network.connections){
				if (player.ipAddress == PlayerPrefs.GetString("Player 1 ip")){
					networkView.RPC("Spawn", player, viewID);
				}
			}	
			
			viewID = Network.AllocateViewID();
			clone = Instantiate(otisPrefab) as GameObject;
			nView = clone.GetComponent<NetworkView>();
			nView.viewID = viewID;			
			foreach (NetworkPlayer player in Network.connections){
				if (player.ipAddress == PlayerPrefs.GetString("Player 2 ip")){
					networkView.RPC("Spawn", player, viewID);
				}
			}					
		}
				
	}
	
	// Update is called once per frame
	void Update () {
	
	}
					
	[RPC]
	void Spawn(NetworkViewID viewID)
	{
		GameObject clone = Instantiate(gamepadPrefab) as GameObject;
		NetworkView nView;
		nView = clone.GetComponent<NetworkView>();
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
	}	
}