using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    /*
     * TODO: Create delegate for item functions
     * Make private functions following format of delegate in this list
     * Add private functions to dictionary
     * Decide ID ranges for types of items
     * Add inventory to game agents to be accessed by inventory manager   
     */

    //define delegate to encapsulate item functions to store in dictionary
    //item functions will take in game agent to apply item effects to that agent
    delegate void ItemFunc(Item item, GameAgent agent);
    IDictionary<string, ItemFunc> itemFuncs = new Dictionary<string, ItemFunc>() {
		[HealthPot._ID] = ApplyHealthPotion,
		[ManaPot._ID] = ApplyManaPotion,
        [Tome._ID] = ApplyTome,
        [Gem._ID] = ApplyGem,
	};

    public static InventoryManager instance = null;

    void Start()
    {
        //add all item functions here
        //item functions will be identified via item ID
        if (instance == null) instance = this;
    }

    public static void UseItem(Item item, GameAgent agent)
    {
		//uses item ID to know which item func to call
		//passes item and game agent (for use in equipment)
		if (item is EquipItem)
		{
			EquipEquipment(item as EquipItem, agent);
		}
		else
		{
			instance.itemFuncs[item.ID](item, agent);
		}
    }

    private static void ApplyHealthPotion(Item item, GameAgent agent)
    {
        //decrement item in player inventory, call HP increase func (if applicable)
        //Debug.Log("Applying HP potion!");
		agent.stats.currentHealth += 15;
		agent.animator.PlayHealedAnimation();
		if (agent.stats.currentHealth > agent.stats.maxHealth)
			agent.stats.currentHealth = agent.stats.maxHealth;
    }

    private static void ApplyManaPotion(Item item, GameAgent agent)
    {
        //Debug.Log("Applying MP potion!");
		agent.stats.currentMagicPoints += 15;
		agent.animator.PlayHealedAnimation();
		if (agent.stats.currentMagicPoints > agent.stats.maxMagicPoints)
			agent.stats.currentMagicPoints = agent.stats.maxMagicPoints;
    }

    private static void ApplyTome(Item item, GameAgent agent)
    {   
        int statBoostAmount = Settings.globalRNG.Next(1, 5);
        int statBoostChoice = Settings.globalRNG.Next(0, 2);
        if(statBoostChoice == 0) {
            Debug.Log("Player stats before Tome (atk, def): " + agent.stats.attack + " " + agent.stats.defense);
            agent.stats.attack += statBoostAmount;
            UI_TextAlert.DisplayColorText("An ancient tome provides " + agent.nickname + " with forbidden knowledge (Atk + "+ statBoostAmount + ")", 6);
            Debug.Log("Player stats after Tome (atk, def): " + agent.stats.attack + " " + agent.stats.defense);
        }
        else {
            Debug.Log("Player stats before Tome (atk, def): " + agent.stats.attack + " " + agent.stats.defense);
            agent.stats.defense += statBoostAmount;
            UI_TextAlert.DisplayColorText("An ancient tome provides " + agent.nickname + " with forbidden knowledge (Def + "+ statBoostAmount + ")", 6);
            Debug.Log("Player stats after Tome (atk, def): " + agent.stats.attack + " " + agent.stats.defense);
        }
		agent.animator.PlayHealedAnimation();
    }

    private static void ApplyGem(Item item, GameAgent agent)
    {   
        int statBoostAmount = Settings.globalRNG.Next(1, 20);
        int statBoostChoice = Settings.globalRNG.Next(0, 2);
        if(statBoostChoice == 0) {
            Debug.Log("Player attack stats before Tome (HP, MP): " + agent.stats.maxHealth + " " + agent.stats.maxMagicPoints);
            agent.stats.maxHealth += statBoostAmount;
            UI_TextAlert.DisplayColorText("An imbued gem gives " + agent.nickname + " a surge of power (Max Health + "+ statBoostAmount + ")", 6);
            Debug.Log("Player attack stats after Tome (HP, MP): " + agent.stats.maxHealth + " " + agent.stats.maxMagicPoints);
        }
        else {
            Debug.Log("Player stats before Tome (HP, MP): " + agent.stats.maxHealth + " " + agent.stats.maxMagicPoints);
            agent.stats.maxMagicPoints += statBoostAmount;
            UI_TextAlert.DisplayColorText("An imbued gem fills " + agent.nickname + " with energy (Max mana + "+ statBoostAmount + ")", 6);
            Debug.Log("Player stats after Tome (HP, MP): " + agent.stats.maxHealth + " " + agent.stats.maxMagicPoints);
        }
		agent.animator.PlayHealedAnimation();
    }
		
	private static void EquipEquipment(EquipItem item, GameAgent agent)
	{
		Debug.Log("Equipping item!");
		Debug.Log("Attack before: " + agent.stats.attack);
		Debug.Log("Defense before: " + agent.stats.defense);
		EquipItem equip = item;
        EquipItem oldItem = null;

        switch (equip.type)
        {
            case EquipType.HELMET:
                oldItem = agent.inventory.helmet;
                agent.inventory.helmet = equip;
                agent.stats.attack += equip.atkbonus;
                agent.stats.defense += equip.defbonus;
                break;
            case EquipType.BOOT:
                oldItem = agent.inventory.boots;
                agent.inventory.boots = equip;
                agent.stats.attack += equip.atkbonus;
                agent.stats.defense += equip.defbonus;
                break;
            case EquipType.ARMOR:
                oldItem = agent.inventory.armor;
                agent.inventory.armor = equip;
                agent.stats.attack += equip.atkbonus;
                agent.stats.defense += equip.defbonus;
                break;
            case EquipType.GLOVE:
                oldItem = agent.inventory.gloves;
                agent.inventory.gloves = equip;
                agent.stats.attack += equip.atkbonus;
                agent.stats.defense += equip.defbonus;
                break;
            case EquipType.WEAPON:
                oldItem = agent.inventory.weapon;
                agent.inventory.weapon = equip;
                agent.stats.attack += equip.atkbonus;
                agent.stats.defense += equip.defbonus;
                break;
		}
		if (oldItem != null)
		{
			agent.stats.attack -= oldItem.atkbonus;
			agent.stats.defense -= oldItem.defbonus;
			agent.inventory.AddItem(oldItem);
		}
		Debug.Log("Attack after: " + agent.stats.attack);
		Debug.Log("Defense after: " + agent.stats.defense);
	}

}
