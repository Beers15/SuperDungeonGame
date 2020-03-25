using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MultiplayerLobby : MonoBehaviour
{
	private GameObject Panel_Main;
	private Text[] clientNames = new Text[4];
	private ClassIcon[] clientIcons = new ClassIcon[4];
	//private Text client1Name, client2Name, client3Name, client4Name;
	private InputField nicknameEntry;
	//private ClassIcon client1Icon, client2Icon, client3Icon, client4Icon;
	
	private const string default_client_name = "Not Connected";
	
	private string classstring;
	public string characterClass 
	{ 
		get { return classstring; }
		set {
			Network.submitCommand(new ClassnameCommand(value, NetworkManager.clientID));
			classstring = value;
		}
	}
	
    public void Awake()
    {
        Panel_Main = transform.parent.Find("MainMenu").gameObject;
		Transform names = transform.Find("MiddleBar").Find("Names");
		for (int i = 0; i < 4; i++) {
			clientNames[i] = names.Find("Client (" + i + ")").GetComponent<Text>();
		}
		
		Transform icons = transform.Find("MiddleBar").Find("ClassIcons");
		for (int i = 0; i < 4; i++) {
			clientIcons[i] = icons.Find("ClassIcon (" + i + ")").GetComponent<ClassIcon>();
		}
		
		nicknameEntry = transform.Find("NicknameEntry").transform.Find("Entry").GetComponent<InputField>();
    }
	
	public void Start()
	{
		gameObject.SetActive(false);
	}
	
	private static Color readyTint = new Color(0, 255, 0);
	private static Color defaultTint = Color.white;

    // updates client information
    void Update()
    {
		for (int i = 0; i < 4; i++) {
			Client client = Network.getPeer(i + 1); // client numbering starts at 1
			if (client == null) {
				clientNames[i].text = default_client_name;
				clientIcons[i].SetActiveIcon("None");
				clientNames[i].color = defaultTint;
			}
			else {
				clientNames[i].text = client.nickname;
				clientIcons[i].SetActiveIcon(client.classname);
				clientNames[i].color = client.ready? readyTint : defaultTint;
			}
		}
    }
	
	public void SubmitNickname()
	{
		Network.submitCommand(new NicknameCommand(nicknameEntry.text, NetworkManager.clientID));
	}
	
	public void Ready()
	{
		Network.submitCommand(new ReadyCommand(NetworkManager.clientID));
	}
	
	public void StartGame()
	{
		if (NetworkManager.clientID == 1 && Network.allPlayersReady()) {
			Network.submitCommand(new StartCommand());
		}
	}
	
	public void Back()
	{
		gameObject.SetActive(false);
		Panel_Main.SetActive(true);
		Network.disconnectFromServer();
	}
	
	public void Settings()
	{
		
	}
}
