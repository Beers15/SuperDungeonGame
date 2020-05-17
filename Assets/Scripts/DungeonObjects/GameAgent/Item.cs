using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    //derive item types from this class
    public int maxAmount; //max amount of item that can be held
    public string name; //name of the item
    public string ID; //unique ID used to identity item in code (if necessary)
    public int Amount; //current amount of item held
	public Item() {}
	public Item(int maxAmount, string name, string ID, int Amount) {
		this.maxAmount = maxAmount;
		this.name = name;
		this.ID = ID;
		this.Amount = Amount;
	}
}

public class ConsumableItem : Item
{
	public int effectiveness;
	public ConsumableItem(int effectiveness, int maxAmount, string Name, string ID, int Amount) : base(maxAmount, Name, ID, Amount) {
		this.effectiveness = effectiveness;
	}
}

public class HealthPot : ConsumableItem
{
	public static string _ID = "1";
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public HealthPot(int amount) : base(1, 64, "Health Potion", _ID, amount) {}
}

public class ManaPot : ConsumableItem
{
	public static string _ID = "2";
	//public ManaPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public ManaPot(int amount) : base(1, 64, "Mana Potion", _ID, amount) {}
}

public class Tome : ConsumableItem
{
	public static string _ID = "3";
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public Tome(int amount) : base(1, 10, "Tome", _ID, amount) {}
}

public class Gem : ConsumableItem
{
	public static string _ID = "4";
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public Gem(int amount) : base(1, 10, "Gem", _ID, amount) {}
}

public class Food : ConsumableItem
{
	public static string _ID = "5";
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public Food(int amount) : base(1, 10, "Food", _ID, amount) {}
}

public class Scroll : ConsumableItem
{
	public static string _ID = "6";
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public Scroll(int amount) : base(1, 10, "Scroll", _ID, amount) {}
}

public class Gold : ConsumableItem {
    public static string _ID = "99";
	public Gold(int amount) : base(1, 64, "Gold Coin", _ID, amount) {}
}



