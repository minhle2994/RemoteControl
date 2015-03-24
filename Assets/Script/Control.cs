using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Control : MonoBehaviour {
	private float clientHInput = 0;
	private float clientVInput = 0;
	private bool action = false;
	public GameObject pauseButton;
	public GameObject playButton;
	public GameObject staminaBar;
	public bool vibrate = false;
	
	void  Awake (){
		Debug.Log("Awake");
		//RE-enable the network messages now we've loaded the right level
		Network.isMessageQueueRunning = true;
		
		if(Network.isServer){
			Debug.Log("Server registered the game at the masterserver.");
			MultiplayerFunctions.SP.RegisterHost(GameSettings.serverTitle, GameSettings.description);
		}
	}	
	
	void Start () {
		if (Network.isClient){
			pauseButton = GameObject.FindGameObjectWithTag("PauseButton");
			playButton = GameObject.FindGameObjectWithTag("PlayButton");
			staminaBar = GameObject.FindGameObjectWithTag("Stamina");
			playButton.SetActive(false);		
		}
	}
		
	void FixedUpdate() {
		if (Network.peerType == NetworkPeerType.Disconnected){
			Network.isMessageQueueRunning = false;
			Application.LoadLevel(0);				
		}
		
		staminaBar.GetComponent<Slider>().value -= 0.00025f;
		Mathf.Clamp(staminaBar.GetComponent<Slider>().value, 0, 1);
		if (staminaBar.GetComponent<Slider>().value < 0.1 && vibrate == false){
			Handheld.Vibrate();
			Handheld.Vibrate();
			vibrate = true;
		}
		
		if (staminaBar.GetComponent<Slider>().value > 0.1 && vibrate == true){
			vibrate = false;
		}
		
		VCAnalogJoystickBase moveJoystick = VCAnalogJoystickBase.GetInstance("MoveJoyStick");
		VCButtonBase actionButton = VCButtonBase.GetInstance("Action");
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
		                	actionButton.Pressed, staminaBar.GetComponent<Slider>().value);
	}
	
	public static void AutoResize(int screenWidth, int screenHeight)
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
	}	
	
	public void OnPause(){
		Time.timeScale = 0;
		pauseButton.SetActive(false);
		playButton.SetActive(true);
		networkView.RPC("PauseGame", RPCMode.Others);
	}

	public void OnResume(){
		Time.timeScale = 1;
		pauseButton.SetActive(true);
		playButton.SetActive(false);
		networkView.RPC("ResumeGame", RPCMode.Others);	
	}
	
	[RPC]
	void SendInput(float HInput, float VInput, bool actionButtonPressed, float stamina){
	}
	
	[RPC]
	void PauseGame(){
		Time.timeScale = 0;
		if (Network.isClient){
			pauseButton.SetActive(false);
			playButton.SetActive(true);			
		}	
	}
	
	[RPC]
	void ResumeGame(){
		Time.timeScale = 1;
		if (Network.isClient){
			pauseButton.SetActive(true);
			playButton.SetActive(false);			
		}
	}
	
	[RPC]
	void Eat(){
		staminaBar.GetComponent<Slider>().value += 0.01f;	
		Mathf.Clamp(staminaBar.GetComponent<Slider>().value, 0, 1);
	}
}