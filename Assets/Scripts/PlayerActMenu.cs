using UnityEngine;
using UnityEngine.UI;

public class ActMenuButtons { 
	// Button indexes
	public const int MOVE = 0;
	public const int ACT = 1;
	public const int POTION = 2;
	public const int WAIT = 3;
	public const int ACTION1 = 4;
	public const int ACTION2 = 5;
	public const int BLANK = 6;
	public const int BACK = 7;
}

public class PlayerActMenu
{
	
	
    private static GameObject actMenu;
    private static GameObject[] playerStats;
    private static Button[] buttons;
    

    private static bool isPlayerActMenuActive = false;

    public static void SetPlayerActMenuActive(bool active, GameAgentAction[] actions = null) {
        if (!active || actions == null) {
            SetButtonsToBattleMenu();
            isPlayerActMenuActive = false;
        } else if (active && actions != null) {
            SetButtons(actions);
            isPlayerActMenuActive = true;
        }
    }

    public static bool IsPlayerActMenuActive() {
        return isPlayerActMenuActive;
    }

    public static void MakeButtonNoninteractable(int buttonIndex) {
        buttons[buttonIndex].interactable = false;
    }

    public static void MakeAllButtonsInteractable(bool active) {
        foreach (Button button in buttons) {
            button.interactable = active;
        }
    }

    public static void init() {
		if (actMenu == null)
			actMenu = GameObject.FindGameObjectWithTag("PlayerActMenu");
		if (playerStats == null)
			playerStats = GameObject.FindGameObjectsWithTag("PlayerStats");
		
        buttons = actMenu.GetComponentsInChildren<Button>(true);
        SetPlayerActMenuActive(false);
    }

    public static void UpdatePlayerStatsMenu(int position, string name, GameAgentStats stats, bool deactivatePlayerSpot = false) {
        if (!deactivatePlayerSpot) {
            if (position < playerStats.Length) {
                if (!playerStats[position].activeSelf) {
                    playerStats[position].SetActive(true);
                }

                string hpString = stats.currentHealth.ToString() + "/" + stats.maxHealth.ToString();
                string mpString = stats.currentMagicPoints.ToString() + "/" + stats.maxMagicPoints.ToString();

                playerStats[position].GetComponentInChildren<Text>().text = name;
                FindObjectwithTag("Level", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = stats.level.ToString();
                FindObjectwithTag("HP", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = hpString;
                FindObjectwithTag("MP", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = mpString;
            }
        } else {
            playerStats[position].SetActive(false);
        }
    }

    public static void SetButtonsToBattleMenu() {
        buttons[ActMenuButtons.MOVE].gameObject.SetActive(true);
        buttons[ActMenuButtons.ACT].gameObject.SetActive(true);
        buttons[ActMenuButtons.POTION].gameObject.SetActive(true);
        buttons[ActMenuButtons.WAIT].gameObject.SetActive(true);

        buttons[ActMenuButtons.ACTION1].gameObject.SetActive(false);
        buttons[ActMenuButtons.ACTION2].gameObject.SetActive(false);
        buttons[ActMenuButtons.BLANK].gameObject.SetActive(false);
        buttons[ActMenuButtons.BACK].gameObject.SetActive(false);
    }

    public static void SetActButtons() {
        buttons[ActMenuButtons.MOVE].gameObject.SetActive(false);
        buttons[ActMenuButtons.ACT].gameObject.SetActive(false);
        buttons[ActMenuButtons.POTION].gameObject.SetActive(false);
        buttons[ActMenuButtons.WAIT].gameObject.SetActive(false);

        buttons[ActMenuButtons.ACTION1].gameObject.SetActive(true);
        buttons[ActMenuButtons.ACTION2].gameObject.SetActive(true);
        buttons[ActMenuButtons.BLANK].gameObject.SetActive(true);
        buttons[ActMenuButtons.BACK].gameObject.SetActive(true);
    }

    private static void SetButtons(GameAgentAction[] actions) {
        int buttonIndex = ActMenuButtons.ACTION1;
        foreach (GameAgentAction action in actions) {
            switch (action) {
                case GameAgentAction.MeleeAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "ATTACK";
                        buttonIndex++;
                            }
                    break;
                case GameAgentAction.Taunt:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "TAUNT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "SHOOT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttackMultiShot:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "MULTISHOT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackSingleTarget:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L BOLT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackAOE:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L STORM";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.Heal:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "HEAL";
                        buttonIndex++;
                    }
                    break;
            }
        }

        buttons[ActMenuButtons.BLANK].GetComponentInChildren<Text>().text = "";
        buttons[ActMenuButtons.BACK].GetComponentInChildren<Text>().text = "BACK";

        SetActButtons();
    }

    private static GameObject FindObjectwithTag(string _tag, GameObject parent) {
        return GetChildObject(parent, _tag);
    }

    private static GameObject GetChildObject(GameObject parent, string _tag) {
        for (int i = 0; i < parent.transform.childCount; i++) {
            Transform child = parent.transform.GetChild(i);
            if (child.tag == _tag) {
                return child.gameObject;
            }
            if (child.childCount > 0) {
                GetChildObject(child.gameObject, _tag);
            }
        }

        return null;
    }
}
