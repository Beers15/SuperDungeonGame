using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    //derive item types from this class
    public int maxAmount; //max amount of item that can be held
    public string Name; //name of the item
    public int ID; //unique ID used to identity item in code (if necessary)
    public int Amount; //current amount of item held
	public Item() {}
	public Item(int maxAmount, string Name, int ID, int Amount) {
		this.maxAmount = maxAmount;
		this.Name = Name;
		this.ID = ID;
		this.Amount = Amount;
	}
}

public class ConsumableItem : Item
{
	public int effectiveness;
	public ConsumableItem(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(maxAmount, Name, ID, Amount) {
		this.effectiveness = effectiveness;
	}
}

public class HealthPot : ConsumableItem
{
	public static int _ID = 1;
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public HealthPot(int amount) : base(1, 64, "Health Potion", _ID, amount) {}
}
/*public class HealthPotSmall : HealthPot
{
	public HealthPotSmall() : base(1, 64, "Small Health Potion", _ID, 1) {}
}
public class HealthPotMedium : HealthPot
{
	public HealthPotMedium() : base(2, 64, "Medium Health Potion", _ID, 1) {}
}*/

public class ManaPot : ConsumableItem
{
	public static int _ID = 2;
	//public ManaPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public ManaPot(int amount) : base(1, 64, "Mana Potion", _ID, amount) {}
}
/*public class ManaPotSmall : ManaPot
{
	public ManaPotSmall() : base(1, 64, "Small Mana Potion", _ID, 1) {}
}
public class ManaPotMedium : ManaPot
{
	public ManaPotMedium() : base(2, 64, "Medium Mana Potion", _ID, 1) {}
}*/



