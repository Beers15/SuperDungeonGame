using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{

    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 18;
	public Inventory() {}

    public EquipItem helmet;
    public EquipItem armor;
    public EquipItem gloves;
    public EquipItem boots;
    public EquipItem weapon;

    public int AddItem(Item item)
    {
        if (item == null)
        {
            return 0;
        }
        int count = 0;
        for (int i = 0; i < numItemSlots; i++)
        {
			if (items[i] == null)
            {
                count += item.Amount;
                items[i] = item;
                return count;
            }
            else if (items[i].ID == item.ID)
            {
                int maxInsert = items[i].maxAmount - items[i].Amount;
                if (item.Amount - maxInsert > 0)
                {
                    items[i].Amount = items[i].maxAmount;
                    item.Amount -= maxInsert;
                    count += maxInsert;
                }
                else
                {
                    items[i].Amount += item.Amount;
                    count += item.Amount;
                    return count;
                }
            }
        }

        return count;
    }

    //used to remove item completely from inventory
    //used for throwing away items (not using them)
    public void RemoveItemFromSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        items[slot] = null;
        RearrangeSlots();
    }

    //returns item in slot
    public Item GetItemFromSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return null; //out of bounds
        }
        return items[slot];
    }

    public void IncrementItemAtSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return;
        }
        items[slot].Amount++;
    }

    //decreases amount of item by 1
    //used when item is being used, i.e. potion consumed
    public void DecrementItemAtSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        if ((--items[slot].Amount) <= 0)
        {
            items[slot] = null; //remove item if amount is zero
            RearrangeSlots();
        }
    }

    private void RearrangeSlots()
    {
        for (int i = 0; i < numItemSlots; i++)
        {
            if (items[i] == null)
            {
                for (int j = i + 1; j < numItemSlots; j++)
                {
                    items[j - 1] = items[j];
                }
            }
        }
    }
}
