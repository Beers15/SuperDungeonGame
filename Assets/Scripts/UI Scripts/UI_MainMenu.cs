using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
	private GameObject Panel_SinglePlayer, Panel_MultiPlayer, Panel_Settings;
	
	public void Awake()
	{
		Panel_SinglePlayer = transform.parent.Find("SinglePlayer").gameObject;
		Panel_MultiPlayer  = transform.parent.Find("MultiplayerJoin").gameObject;
		Panel_Settings	   = transform.parent.Find("Settings").gameObject;
		
		Application.targetFrameRate = 60;
	}
	
	public void Start()
	{
		gameObject.SetActive(true);
	}
	
    public void SinglePlayer()
	{
		gameObject.SetActive(false);
		Panel_SinglePlayer.SetActive(true);
	}
	
	public void MultiPlayer()
	{
		gameObject.SetActive(false);
		Panel_MultiPlayer.SetActive(true);
	}
	
	public void Settings()
	{
		gameObject.SetActive(false);
		Panel_Settings.SetActive(true);
	}
	
	public void Quit()
	{
		Debug.Log("Quitting Application...");
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
