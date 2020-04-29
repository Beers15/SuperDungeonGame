using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProceduralDebugger : MonoBehaviour
{
     // this string is the character class
     // setup the string in the unity inspector 
     // set to either [Warrior Healer Hunter Mage]
    public enum CharacterClass 
    { 
        Warrior = 0 , 
        Healer = 1, 
        Hunter = 2, 
        Mage = 3
    };

    public bool debugFog;

    public string[] classes = {"Warrior", "Healer", "Hunter", "Mage"};

	public CharacterClass characterClass;

    private string classString;
    public GameObject loadingScreen;
    public GameObject networkPrefab;

    private MapManager mapManager;

	public void Start()
	{
        Instantiate(networkPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        mapManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        if(debugFog)
        {
            mapManager.debug = true;
        }
		StartGame();
        loadingScreen.SetActive(false);
	}
	
    public void StartGame()
	{
		Network.setPeer(0);
		Network.getPeer(0).classname = classes[(int)characterClass];
		switch (classes[(int) characterClass]) {
			case "Warrior": 
			Network.getPeer(0).nickname = "Sir Lancelot"; break;
			case "Healer": 
			Network.getPeer(0).nickname = "Sir Heals a Lot"; break;
			case "Hunter": 
			Network.getPeer(0).nickname = "Robin Hood"; break;
			case "Mage": 
			Network.getPeer(0).nickname = "Merlin the Great"; break;
		}
		//SceneManager.LoadScene("Procedural");
	}
	
}