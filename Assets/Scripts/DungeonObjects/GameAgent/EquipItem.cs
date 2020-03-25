using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType {
    HELMET,
    ARMOR,
    GLOVE,
    BOOT,
    WEAPON,
}

public class EquipItem : Item
{
    public EquipType type;
    public int atkbonus;
    public int defbonus;

	public EquipItem() {}
    public EquipItem(string name, int id, EquipType etype, int atk, int def)
    {
        maxAmount = 1;
        Amount = 1;
        Name = name;
        ID = id;
        type = etype;
        atkbonus = atk;
        defbonus = def;
    }
}

public class Helmet : EquipItem
{
	public static int _ID = 3;
	public Helmet() : base("Helmet", _ID, EquipType.HELMET, 0, 10) {}
}

public class Armor : EquipItem
{
	public static int _ID = 4;
	public Armor() : base("Armor", _ID, EquipType.ARMOR, 0, 10) {}
}

public class Glove : EquipItem
{
	public static int _ID = 5;
	public Glove() : base("Glove", _ID, EquipType.GLOVE, 0, 10) {}
}

public class Boot : EquipItem
{
	public static int _ID = 6;
	public Boot() : base("Boot", _ID, EquipType.BOOT, 0, 10) {}
}

public class EquipWeapon : EquipItem
{
	public static int _ID = 7;
	public EquipWeapon() : base("Weapon", _ID, EquipType.WEAPON, 10, 0) {}
}
