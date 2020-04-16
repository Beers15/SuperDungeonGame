using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [Helmet._ID] = EquipEquipment,
        [Armor._ID] = EquipEquipment,
        [Glove._ID] = EquipEquipment,
        [Boot._ID] = EquipEquipment,
        [EquipWeapon._ID] = EquipEquipment,
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
        instance.itemFuncs[item.ID](item, agent);
    }

    private static void ApplyHealthPotion(Item item, GameAgent agent)
    {
        //decrement item in player inventory, call HP increase func (if applicable)
        //Debug.Log("Applying HP potion!");
		agent.stats.currentHealth += 20;
		agent.animator.PlayHealedAnimation();
		if (agent.stats.currentHealth > agent.stats.maxHealth)
			agent.stats.currentHealth = agent.stats.maxHealth;
    }

    private static void ApplyManaPotion(Item item, GameAgent agent)
    {
        //Debug.Log("Applying MP potion!");
		agent.stats.currentMagicPoints += 20;
		agent.animator.PlayHealedAnimation();
		if (agent.stats.currentMagicPoints > agent.stats.maxMagicPoints)
			agent.stats.currentMagicPoints = agent.stats.maxMagicPoints;
    }


    //FOLLOWING METHODS FOR EQUIPMENT SYSTEM
	
	private static void EquipEquipment(Item item, GameAgent agent)
	{
		Debug.Log("Equipping item!");
        EquipItem equip = (EquipItem)item;
        EquipItem oldItem;

        switch (equip.type)
        {
            case EquipType.HELMET:
                oldItem = agent.inventory.helmet;
                agent.inventory.helmet = equip;
                agent.stats.attack = (equip.atkbonus - oldItem.atkbonus);
                agent.stats.defense = (equip.defbonus - oldItem.defbonus);
                agent.inventory.AddItem(oldItem);
                break;
            case EquipType.BOOT:
                oldItem = agent.inventory.boots;
                agent.inventory.helmet = equip;
                agent.stats.attack = (equip.atkbonus - oldItem.atkbonus);
                agent.stats.defense = (equip.defbonus - oldItem.defbonus);
                agent.inventory.AddItem(oldItem);
                break;
            case EquipType.ARMOR:
                oldItem = agent.inventory.armor;
                agent.inventory.helmet = equip;
                agent.stats.attack = (equip.atkbonus - oldItem.atkbonus);
                agent.stats.defense = (equip.defbonus - oldItem.defbonus);
                agent.inventory.AddItem(oldItem);
                break;
            case EquipType.GLOVE:
                oldItem = agent.inventory.gloves;
                agent.inventory.helmet = equip;
                agent.stats.attack = (equip.atkbonus - oldItem.atkbonus);
                agent.stats.defense = (equip.defbonus - oldItem.defbonus);
                agent.inventory.AddItem(oldItem);
                break;
            case EquipType.WEAPON:
                oldItem = agent.inventory.weapon;
                agent.inventory.helmet = equip;
                agent.stats.attack = (equip.atkbonus - oldItem.atkbonus);
                agent.stats.defense = (equip.defbonus - oldItem.defbonus);
                agent.inventory.AddItem(oldItem);
                break;
        }
    }

    /*private void EquipHelmet(Item item, GameAgent agent)
    {
        Debug.Log("Equipping helmet!");
    }

    private void EquipArmor(Item item, GameAgent agent)
    {
        Debug.Log("Equipping armor!");
    }

    private void EquipWeapon(Item item, GameAgent agent)
    {
        Debug.Log("Equipping weapon!");
    }

    private void EquipGloves(Item item, GameAgent agent)
    {
        Debug.Log("Equipping gloves!");
    }

    private void EquipShoes(Item item, GameAgent agent)
    {
        Debug.Log("Equipping shoes!");
    }

    private void EquipOffhand(Item item, GameAgent agent)
    {
        Debug.Log("Equipping offhand item!");
    }*/
}
