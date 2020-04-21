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

public class EquipItem : Item {
    public EquipType type;
    public int atkbonus;
    public int defbonus;

    //All gear IDs are within this range by default until they are assigned stats and given unique IDs (Change as needed)
    private static int IDUpperBound = 7;
    private static int IDLowerBound = 3;
    private int lvlOfSlainMob = 1;  //used to calculate EquipItem's tier 
    private int tier;

	public EquipItem() {}
    public EquipItem(string name, string id, EquipType etype, int atk, int def) {
        maxAmount = 1;
        Amount = 1;
        Name = name;
        ID = id;
        type = etype;
        atkbonus = atk;
        defbonus = def;
    }

    public static int getIDUpperBound() {
        return IDUpperBound;
    }

    public static int getIDLowerBound() {
       return  IDLowerBound;
    }
        
    public void setLvl(int level) {
        lvlOfSlainMob = level;
        getTier();
    }

    public void getTier() {
        Debug.Log("This gear's slain mob was lvl " + lvlOfSlainMob);

        //TODO add logic to have enemies lvl/boss status affect this value
        int tierValue;
        
        int tierRoll = UnityEngine.Random.Range(1, 100);

        if(tierRoll > 98) {
            tierValue = 5; //legendary
        }
        else if(tierRoll > 91) {
            tierValue = 4; //epic
        }	
        else if(tierRoll > 80) {
            tierValue = 3; //rare
        }
        else if(tierRoll > 55) {
            tierValue = 2; //magic;
        }
        else
            tierValue = 1; //common

        tier = tierValue;
        
        //TODO
        //generateStats();
        //generateName();
        //displayMessage();
        Debug.Log("This gear's tier is " + tier);
    }

    // private void generateStats() {

    // }
    // private void generateName() {
        
    // }
    // public void displayMessage() {
        
    // }
}

public class Helmet : EquipItem {
	public static string _ID = "3";
	public Helmet() : base("Helmet", _ID, EquipType.HELMET, 0, 10) {}
}

public class Armor : EquipItem {
	public static string _ID = "4";
	public Armor() : base("Armor", _ID, EquipType.ARMOR, 0, 10) {}
}

public class Glove : EquipItem {
	public static string _ID = "5";
	public Glove() : base("Glove", _ID, EquipType.GLOVE, 0, 10) {}
}

public class Boot : EquipItem {
	public static string _ID = "6";
	public Boot() : base("Boot", _ID, EquipType.BOOT, 0, 10) {}
}

public class EquipWeapon : EquipItem {
	public static string _ID = "7";
	public EquipWeapon() : base("Weapon", _ID, EquipType.WEAPON, 10, 1) {}
}
