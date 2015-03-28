using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Control : Photon.MonoBehaviour {
	public GameObject pauseButton;
	public GameObject playButton;
	public GameObject staminaBar;
	
	void  Awake (){
		PhotonNetwork.isMessageQueueRunning = true;
	}
	
	void Start () {
		pauseButton = GameObject.FindGameObjectWithTag("PauseButton");
		playButton = GameObject.FindGameObjectWithTag("PlayButton");
		staminaBar = GameObject.FindGameObjectWithTag("Stamina");
		playButton.SetActive(false);		
	}
		
	void FixedUpdate() {
		staminaBar.GetComponent<Slider>().value -= 0.00025f;
		Mathf.Clamp(staminaBar.GetComponent<Slider>().value, 0, 1);
		
		
		VCAnalogJoystickBase moveJoystick = VCAnalogJoystickBase.GetInstance("MoveJoyStick");
		VCButtonBase actionButton = VCButtonBase.GetInstance("Action");
		Vector2 directionVector = new Vector2(moveJoystick.AxisX, moveJoystick.AxisY);
		if (directionVector != Vector2.zero){
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			directionLength = Mathf.Min(1.0f, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		
		photonView.RPC("SendInput", PhotonTargets.MasterClient, directionVector.x, directionVector.y, 
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
		photonView.RPC("PauseGame", PhotonTargets.Others);
	}

	public void OnResume(){
		Time.timeScale = 1;
		pauseButton.SetActive(true);
		playButton.SetActive(false);
		photonView.RPC("ResumeGame", PhotonTargets.Others);	
	}
	
	[RPC]
	void SendInput(float HInput, float VInput, bool actionButtonPressed, float stamina){
	}
	
	[RPC]
	void PauseGame(){
		Time.timeScale = 0;
		pauseButton.SetActive(false);
		playButton.SetActive(true);			
	}
	
	[RPC]
	void ResumeGame(){
		Time.timeScale = 1;
		pauseButton.SetActive(true);
		playButton.SetActive(false);			
	}
	
	[RPC]
	void Eat(){
		staminaBar.GetComponent<Slider>().value += 0.01f;	
		Mathf.Clamp(staminaBar.GetComponent<Slider>().value, 0, 1);
	}
}