using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleMenu : MonoBehaviour
{
	private Button[] default_buttons = new Button[5];
	private Button[] action_buttons = new Button[5];
	private GameObject defaultMenu, actionMenu;
	
	private static UI_BattleMenu instance;
	
	void Awake()
	{
		instance = this;
		Transform bg = transform.Find("BG");
		defaultMenu = bg.transform.Find("DEFAULT_BUTTONS").gameObject;
		actionMenu  = bg.transform.Find("ACTION_BUTTONS").gameObject;
		
		default_buttons[0] = defaultMenu.transform.Find("MOVE").GetComponent<Button>();
		default_buttons[1] = defaultMenu.transform.Find("ACT").GetComponent<Button>();
		default_buttons[2] = defaultMenu.transform.Find("POTION").GetComponent<Button>();
		default_buttons[3] = defaultMenu.transform.Find("INTERACT").GetComponent<Button>();
		default_buttons[4] = defaultMenu.transform.Find("WAIT").GetComponent<Button>();
		
		action_buttons[0] = actionMenu.transform.Find("ACTION 1").GetComponent<Button>();
		action_buttons[1] = actionMenu.transform.Find("ACTION 2").GetComponent<Button>();
		action_buttons[2] = actionMenu.transform.Find("ACTION 3").GetComponent<Button>();
		action_buttons[3] = actionMenu.transform.Find("ACTION 4").GetComponent<Button>();
		action_buttons[4] = actionMenu.transform.Find("BACK").GetComponent<Button>();
	}
	
	void Start()
	{
		defaultMenu.SetActive(true);
		actionMenu.SetActive(false);
	}
	
	void Update()
	{
		if (defaultMenu.activeSelf) {
			if (Input.GetKeyDown("a")) Move();
			if (Input.GetKeyDown("s")) ActMenu();
			if (Input.GetKeyDown("d")) Potion();
			if (Input.GetKeyDown("f")) Interact();
			if (Input.GetKeyDown("w")) Wait();
		} else if (actionMenu.activeSelf) {
			if (Input.GetKeyDown("a")) Action(0);
			if (Input.GetKeyDown("s")) Action(1);
			if (Input.GetKeyDown("d")) Action(2);
			if (Input.GetKeyDown("f")) Action(3);
			if (Input.GetKeyDown("w")) ActMenuBack();
		}
	}
	
	public static void SetActButtons(string[] actionNames)
	{
		string[] actHotkeys = { "[A]", "[S]", "[D]", "[F]" };
		for (int i = 0; i < 4; i++) {
			if (i < actionNames.Length)
				instance.action_buttons[i].GetComponentInChildren<Text>().text = actionNames[i] + " " + actHotkeys[i];
			else
				instance.action_buttons[i].GetComponentInChildren<Text>().text = "";
		}
	}
	
	public void Move()
	{
		// gamemanager move
		GameManager.MovePlayer();
	}
	
	public void ActMenu()
	{
		// display action menu
		defaultMenu.SetActive(false);
		actionMenu.SetActive(true);
	}
	
	public void Potion()
	{
		// gamemanager potion
		GameManager.PotionPlayer();
	}
	
	public void Interact()
	{
		GameManager.InteractPlayer();
	}
	
	public void Wait()
	{
		// network wait
		GameManager.WaitPlayer();
	}
	
	public void Action(int action)
	{
		// gamemanager action
		GameManager.ActionPlayer(action);
	}
	
	public void ActMenuBack()
	{
		// go back to default menu
		GameManager.ClearPlayerAction();
		defaultMenu.SetActive(true);
		actionMenu.SetActive(false);
	}
}
