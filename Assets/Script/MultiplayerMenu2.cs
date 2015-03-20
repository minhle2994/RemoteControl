using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MultiplayerMenu2 : MonoBehaviour {
	private List<MyHostData> hostDataList = new List<MyHostData>();
	private  string failConnectMesage= "";
	
    void Awake (){
        GameSettings.Clear(); 
    }

    void  Start (){
        hostDataList = new List<MyHostData>();
        MultiplayerFunctions.SP.SetHostListDelegate(FullHostListReceived);

    }
    
    void Update(){

    }
   
	public void onClick(){
		GameObject input = GameObject.FindGameObjectWithTag("CodeInput");
		Text code = input.GetComponent<Text>();
		
		if (code != null){
			foreach(MyHostData hData  in hostDataList ){
				Debug.Log(hData.title);
				MultiplayerFunctions.SP.DirectConnect(hData.IP, hData.port, code.text, true, OnFinalFailedToConnect);
			}
		}
	}
	 
	private NetworkConnectionError lastConnectError;
	void OnFinalFailedToConnect (){
		GameObject input = GameObject.FindGameObjectWithTag("Feedback");
		Text code = input.GetComponent<Text>();
		code.text = "Invalid code";
		StartCoroutine(FadeOut());	
		lastConnectError = MultiplayerFunctions.SP.LastConnectionError();
		failConnectMesage = failConnectMesage + "Attempting to connect to [" + MultiplayerFunctions.SP.LastIP()[0] + ":" + MultiplayerFunctions.SP.LastPort() + "]: " + lastConnectError + "\n";
		Debug.Log("OnFinalFailedToConnect=" + failConnectMesage);
	}
	
	IEnumerator FadeOut() {
		yield return new WaitForSeconds(2);
		GameObject input = GameObject.FindGameObjectWithTag("Feedback");
		Text code = input.GetComponent<Text>();
		code.text = "";
	}	
	
	void OnConnectedToServer (){
		GameSettings.Clear();
		GameObject input = GameObject.FindGameObjectWithTag("InputField");
		GameObject button = GameObject.FindGameObjectWithTag("EnterButton");	
		input.SetActive(false);
		button.SetActive(false);
	}
	
	
    void FullHostListReceived (){
        StartCoroutine(ReloadHosts());
    }

     private bool  hasParsedHostListOnce= false;
     private bool  parsingHostList= false;

    IEnumerator ReloadHosts (){
        if (parsingHostList) yield break;
        parsingHostList = true;
         HostData[] newData= MultiplayerFunctions.SP.GetHostData();
         int hostLenght= -1;
        while (hostLenght != newData.Length)
        {
            yield return new WaitForSeconds(0.5f);
            newData = MultiplayerFunctions.SP.GetHostData();
            hostLenght = newData.Length;
        }

        hostDataList.Clear();
        foreach(HostData hData    in newData )
        {
             MyHostData cHost= new MyHostData();
            cHost.realHostData = hData;
            cHost.connectedPlayers = hData.connectedPlayers;
            cHost.IP = hData.ip;
            cHost.port = hData.port;
            cHost.maxPlayers = hData.playerLimit;

            cHost.passwordProtected = hData.passwordProtected;
            cHost.title = hData.gameName;
            cHost.useNAT = hData.useNat;
            
            /*//EXAMPLE CUSTOM FIELDS
            string[] comments= hData.comment.Split("#"[0]);
            cHost.gameVersion = int.Parse(comments[2]);

            //cHost.isDedicated = comments[1] == "1";         
            if (cHost.isDedicated)
            {
                cHost.connectedPlayers -= 1;
                cHost.maxPlayers -= 1;
            }*/

            hostDataList.Add(cHost);
            
            if (hostDataList.Count % 3 == 0)
            {
                yield return 0;
            }
        }
        parsingHostList = false;
        hasParsedHostListOnce = true;
    }

	[RPC]
	void Waiting()
	{	
		GameObject text = GameObject.FindGameObjectWithTag("Text");
		text.GetComponent<Text>().text = "Waiting for other player";	
	}	

	[RPC]
	void StartGame()
	{
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Application.loadedLevel+1);
	}
	
	public class MyHostData
	{
	     public HostData realHostData;
	     public string title;
	     public bool useNAT;
	     public int connectedPlayers;
	     public int maxPlayers;
	     public string[] IP;
	     public int port;
	     public bool passwordProtected;
	     
	     //Example custom fields
	     public bool isDedicated = false;
	     public int gameVersion;
	}
}