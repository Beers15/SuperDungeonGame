using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MapUtils;

public class UI_MultiplayerJoin : MonoBehaviour
{
    private GameObject Panel_Main, Panel_Lobby;
	private Text entry;
	
	public void Awake()
	{
		Panel_Main = transform.parent.Find("MainMenu").gameObject;
		Panel_Lobby = transform.parent.Find("MultiplayerLobby").gameObject;
		entry = transform.Find("IPEntry").GetComponent<Text>();
	}
	
	public void Start()
	{
		gameObject.SetActive(false);
	}
	
    public void Connect()
	{
		string[] IP_PORT = entry.text.Split(':');
		if (IP_PORT.Length != 2) Debug.Log("Invalid IP!");
		
		string IP = IP_PORT[0];
		int PORT = int.Parse(IP_PORT[1]);
		
		Network.connectToServer(IP, PORT);
		gameObject.SetActive(false);
		Panel_Lobby.SetActive(true);
	}
	
	public void Back()
	{
		gameObject.SetActive(false);
		Panel_Main.SetActive(true);
	}
}
