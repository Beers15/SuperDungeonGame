using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionsStore 
{   
    public const int numPotionSlots = 2;
    public ConsumableItem[] potions = new ConsumableItem[numPotionSlots];
	public PotionsStore() {
        for(int i = 0; i < potionOptions.Length; i++) 
            potions[i] = new ConsumableItem(0, 20, potionOptions[i], "-1", 0);
    }

    string[] potionOptions = {"Health Potion", "Mana Potion"};

    public void AddConsumable(ConsumableItem potion)
    {
        if (potion == null) {
            return;
        }

        //using name instead of ID for now
        for(int i = 0; i < potionOptions.Length; i++) {
            if(string.Equals(potion.Name, potions[i].Name)) {
                //if(potions[i].effectivness == 0)
                    //potions[i].effectivness = potion.effectivness;

                if(string.Compare(potions[i].ID, "-1") == 0)
                    potions[i].ID = potion.ID;


                int maxInsert = potions[i].maxAmount - potions[i].Amount;

                if (potion.Amount - maxInsert > 0) {
                    potions[i].Amount = potions[i].maxAmount;
                    potion.Amount -= maxInsert;
                } else {
                    potions[i].Amount += potion.Amount;
                }
            }
        }
    }

    //returns potion in slot
    public Item GetPotionFromSlot(int slot)
    {
        if (slot >= numPotionSlots)
        {
            return null; //out of bounds
        }
        return potions[slot];
    }

    public void IncrementPotionAtSlot(int slot)
    {
        if (slot >= numPotionSlots)
        {
            return;
        }
        potions[slot].Amount++;
    }

    //decreases amount of item by 1
    //used when consumable is being used, i.e. potion consumed
    public void DecrementPotionAtSlot(int slot)
    {
        if (slot >= numPotionSlots)
        {
            return; //out of bounds
        }
        if ((--potions[slot].Amount) <= 0)
        {
            //change icon if none of a certain potion are possessed
        }
    }

    //testing
    public void display() {
        for(int i = 0; i < potionOptions.Length; i++) {
            if(string.Compare(potions[i].ID, "-1") != 0)
                Debug.Log("potions slot #" + i + " Potion: " + potions[i].Name + " Amount: " + potions[i].Amount + " ID: " + potions[i].ID);
        }
    }
}
