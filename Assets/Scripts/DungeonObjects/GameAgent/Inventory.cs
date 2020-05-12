using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory {
    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 18;

    private int gearIdLowerBound = EquipItem.getIDLowerBound();
    private int gearIdUpperBound = EquipItem.getIDUpperBound();

    public EquipItem helmet;
    public EquipItem armor;
    public EquipItem gloves;
    public EquipItem boots;
    public EquipItem weapon;
    private Item gold;

	public Inventory() {
        for(int i = 0; i < numItemSlots; i++) 
            items[i] = new Item(20, "", "-1", 0);

        gold = new Item(9999999, "gold", "99", 0);
    }

    public int AddItem(Item item) { 
        int slotNum = -1;

        if(item == null) {
            return 0;
        }

        else if(string.Equals(item.ID, "99")) {
            int maxInsert = gold.maxAmount - item.Amount;

                if(item.Amount - maxInsert > 0) {
                    gold.Amount = gold.maxAmount;
                    item.Amount -= maxInsert;
                } else {
                    gold.Amount += item.Amount;
                }
                Debug.Log("TOTAL GOLD: " + gold.Amount);
            return 0;
        }

        bool itemFound = false;
        //check if item already exists in inventory
        for(int i = 0; i < numItemSlots; i++) {
            if(string.Equals(item.ID, items[i].ID)) {
                // if(string.Compare(items[i].ID, "-1") == 0) 
                //     items[i].ID = item.ID;

                int maxInsert = items[i].maxAmount - items[i].Amount;

                if (item.Amount - maxInsert > 0) {
                    items[i].Amount = items[i].maxAmount;
                    item.Amount -= maxInsert;
                } else {
                    items[i].Amount += item.Amount;
                }
                itemFound = true;
                slotNum = i;
                break;
            }
        }

        if(!itemFound) {
            bool emptySlotExists = false;
            //insert into inventory at an empty slot if not found
            for(int i = 0; i < numItemSlots; i++) {
                if(string.Equals(items[i].ID, "-1")) {
					items[i] = item;

                    //check to see if item that is being added is equipment
                    long id = long.Parse(item.ID);
                    Debug.Log("ID: " + id);
                    if(id >= gearIdLowerBound && id <= gearIdUpperBound) {
                        item.ID = getUniqueID();
                        
                        //todo: static function call from EquipItem that produces random stats for gear
                        //based on rarity variable (add rarity variable)

                        Debug.Log("Equipment ID generated: " + item.ID);
                    }
					/*
                    int maxInsert = items[i].maxAmount - items[i].Amount;

                    if(item.Amount - maxInsert > 0) {
                        items[i].Amount = items[i].maxAmount;
                        item.Amount -= maxInsert;
                    } else {
                        items[i].Amount += item.Amount;
                    }
					*/
                    emptySlotExists = true;
                    slotNum = i;
                    break;
                }
            }

            if(!emptySlotExists) {
                //TODO: handle how to deal with item when this occurs
        	    UI_TextAlert.DisplayText("Inventory full.");
            }
        }
        return slotNum;
    }

    //used to remove item completely from inventory
    //used for throwing away items (not using them)
    public void RemoveItemFromSlot(int slot) {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        items[slot].ID = "-1";
        RearrangeSlots();
    }

    //returns item in slot
    public Item GetItemFromSlot(int slot) {
        if (slot >= numItemSlots)
        {
            return null; //out of bounds
        }
        return items[slot];
    }

    public Item getGold() {
        return gold;
    }

    public void IncrementItemAtSlot(int slot) {
        if (slot >= numItemSlots)
        {
            return;
        }
        items[slot].Amount++;
    }

    //decreases amount of item by 1
    //used when item is being used, i.e. potion consumed
    public void DecrementItemAtSlot(int slot) {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
		if (items[slot].Amount <= 1)
		{
			items[slot] = new Item(20, "", "-1", 0); //remove item if amount is zero
			RearrangeSlots();
		}
		else
		{
			items[slot].Amount--;
		}
    }

    private void RearrangeSlots() {
        for (int i = 0; i < numItemSlots; i++)
        {
            if (items[i].ID == "-1")
            {
                for (int j = i + 1; j < numItemSlots; j++)
                {   
                        items[j - 1] = items[j];
                        items[j] = new Item(20, "", "-1", 0);
                }
            }
        }
    }

    //testing
    public void display() {
        for(int i = 0; i < numItemSlots; i++) {
            if(!string.Equals(items[i].ID, "-1"))
                Debug.Log("item slot #" + i + " Item: " + items[i].name + " Amount: " + items[i].Amount + " ID: " + items[i].ID);
        }
    }

    //referenced https://sushanta1991.blogspot.com/2016/02/how-to-create-unique-id-in-unity.html
    private string getUniqueID()
    {
        string [] split = System.DateTime.Now.TimeOfDay.ToString().Split(new char [] {':','.'});
        string id = "";
        for(int i = 0; i < split.Length; i++) {
            id+=split[i];
        }
        return id;
    }
}
