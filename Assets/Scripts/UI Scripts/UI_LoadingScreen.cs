using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadingScreen : MonoBehaviour
{	
	private GameObject waitingForPlayers;
	private GameObject waitingForLoad;
	void Awake()
	{
		waitingForPlayers = transform.Find("WaitingForPlayers").gameObject;
		waitingForLoad = transform.Find("LoadingLevel").gameObject;
	}
	void Update()
	{
		if (Network.localClient().ready) {
			waitingForLoad.SetActive(false);
		}
		else {
			waitingForLoad.SetActive(true);
		}
		if (Network.allPlayersReady()) {
			waitingForPlayers.SetActive(false);
		}
		else {
			waitingForPlayers.SetActive(true);
		}
	}
}
