using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_SinglePlayer : MonoBehaviour
{
    private GameObject Panel_Main;
	
	public string characterClass { get; set;} 
	
	public void Awake()
	{
		Panel_Main = transform.parent.Find("MainMenu").gameObject;
	}
	
	public void Start()
	{
		gameObject.SetActive(false);
	}
	
    public void StartGame()
	{
		Network.setPeer(0);
		Network.getPeer(0).classname = characterClass;
		switch (characterClass) {
			case "Warrior": 
			Network.getPeer(0).nickname = "Sir Lancelot"; break;
			case "Healer": 
			Network.getPeer(0).nickname = "Sir Heals a Lot"; break;
			case "Hunter": 
			Network.getPeer(0).nickname = "Robin Hood"; break;
			case "Mage": 
			Network.getPeer(0).nickname = "Merlin the Great"; break;
		}
		SceneManager.LoadScene("Procedural");
	}
	
	public void Back()
	{
		gameObject.SetActive(false);
		Panel_Main.SetActive(true);
	}
}
