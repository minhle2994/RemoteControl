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
	}	
}