using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
	private float clientHInput = 0;
	private float clientVInput = 0;
	private bool action = false;
	private bool fire = false;
	
	void  Awake (){
		Debug.Log("Awake");
		//RE-enable the network messages now we've loaded the right level
		Network.isMessageQueueRunning = true;
		
		if(Network.isServer){
			Debug.Log("Server registered the game at the masterserver.");
			MultiplayerFunctions.SP.RegisterHost(GameSettings.serverTitle, GameSettings.description);
		}
	}	

	void  OnGUI (){
		if (Network.peerType == NetworkPeerType.Client){
			GUILayout.Label("Connection status: Client!");
			GUILayout.Label("Ping to server: "+Network.GetAveragePing(Network.connections[0]));
		}	

		if (Network.peerType == NetworkPeerType.Disconnected){
			//We are currently disconnected: Not a client or host
			GUILayout.Label("Connection status: We've (been) disconnected");
			if(GUILayout.Button("Back to main menu")){
				Application.LoadLevel(Application.loadedLevel-1);
			}
		}
		if (GUILayout.Button ("Disconnect"))
		{
			Network.Disconnect();
		}
	}		
	
	void FixedUpdate() {
		VCAnalogJoystickBase moveJoystick = VCAnalogJoystickBase.GetInstance("MoveJoyStick");
		VCButtonBase actionButton = VCButtonBase.GetInstance("Action");
		VCButtonBase fireButton = VCButtonBase.GetInstance("Fire");
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
		}
		networkView.RPC("SendInput", RPCMode.Server, directionVector.x, directionVector.y, 
													actionButton.Pressed, fireButton.Pressed);
	}
	
	[RPC]
	void SendInput(float HInput, float VInput, bool actionButtonPressed, bool fireButtonPressed){
		clientHInput = HInput;
		clientVInput = VInput;
		action = actionButtonPressed;
		fire = fireButtonPressed;
	}
}