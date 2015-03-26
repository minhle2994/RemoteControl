using UnityEngine;
using System.Collections;

public class Generate : MonoBehaviour {
	public GameObject miloPrefab;
	public GameObject otisPrefab;
	public GameObject gamepadPrefab;
	public GameObject uiPrefab;
	
	private float startTime = 0.0f;
	public GameObject pad;
	public bool hasFinishTut = false;
	
	void  Awake (){
		startTime = Time.time;
		//RE-enable the network messages now we've loaded the right level
		Network.isMessageQueueRunning = true;
		
		if(Network.isServer){
			Debug.Log("Server registered the game at the masterserver.");
			MultiplayerFunctions.SP.RegisterHost(GameSettings.serverTitle, GameSettings.description);
		}
	}
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
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
		if (pad == null)
			pad = GameObject.Find("GamePad(Clone)");
		if (hasFinishTut == true && pad != null){
			VCButtonBase actionButton = VCButtonBase.GetInstance("Action");
			Debug.Log(actionButton == null);
			if (actionButton.Pressed){
				networkView.RPC("Select", RPCMode.Others);						
				Network.isMessageQueueRunning = false;
				Application.LoadLevel(Application.loadedLevel+1);
			}
		}
			
		if (Time.time - startTime > 1){
			if (hasFinishTut == true && pad != null){
				VCAnalogJoystickBase moveJoystick = VCAnalogJoystickBase.GetInstance("MoveJoyStick");
				Vector2 directionVector = new Vector2(moveJoystick.AxisX, moveJoystick.AxisY);
				if (directionVector != Vector2.zero){
					// Get the length of the directon vector and then normalize it
					// Dividing by the length is cheaper than normalizing when we already have the length anyway
					var directionLength = directionVector.magnitude;
					directionVector = directionVector / directionLength;
					
					// Make sure the length is no bigger than 1
					directionLength = Mathf.Min(1.0f, directionLength);
					
					// Make the input vector more sensitive towards the extremes and less sensitive in the middle
					// This makes it easier to control slow speeds when using analog sticks
					directionLength = directionLength * directionLength;
					
					// Multiply the normalized direction vector by the modified length
					directionVector = directionVector * directionLength;
					
					if (directionVector.x > 0)
						networkView.RPC("NextMap", RPCMode.Server);
					if (directionVector.x < 0)
						networkView.RPC("PreviousMap", RPCMode.Server);						 	
				}
			}
			startTime = Time.time;
		}
	}
		
	public void Skip(){
		networkView.RPC("SkipTut", RPCMode.Others);			
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
		Instantiate(uiPrefab);			                                         						                                         			
	}
		
	[RPC]
	void NextMap()
	{
	}
	
	[RPC]
	void PreviousMap()
	{
	}
	
	[RPC]
	void FinishTut()
	{
		hasFinishTut = true;
	}
	
	[RPC]
	void Select()
	{
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel+1);
	}
	
	[RPC]
	void SkipTut()
	{
	}	
}