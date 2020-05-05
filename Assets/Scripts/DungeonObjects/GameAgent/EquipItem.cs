using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    public string owner;
    //public CharacterClassOptions weaponType;

    public string completeName;
    public List<string> equipmentAdjectives; 
    public List<string> equipmentNouns;
    public string[] adjectives;
    public string[] nouns;

    //All gear IDs are within this range by default until they are assigned stats and given unique IDs (Change as needed)
    private static int IDUpperBound = 7;
    private static int IDLowerBound = 3;
    private int lvlOfSlainMob = 1;  //used to calculate EquipItem's tier 
    private int tier;
    public int weaponClass;

	public EquipItem() {}
    public EquipItem(string name, string id, EquipType etype, int atk, int def) {
        initWords();
        maxAmount = 1;
        Amount = 1;
        this.name = name;
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
        
    public void generateEquipmentValues(int level, string owner) {
        lvlOfSlainMob = level;
        this.owner = owner;

        int tierValue;
        
        int tierRoll = Settings.globalRNG.Next(1, 100);

        if(tierRoll > 93) {
            tierValue = 5; //legendary
        }
        else if(tierRoll > 85) {
            tierValue = 4; //epic
        }	
        else if(tierRoll > 72) {
            tierValue = 3; //rare
        }
        else if(tierRoll > 60) {
            tierValue = 2; //magic
        }
        else
            tierValue = 1; //common

        tier = tierValue;
  
        generateStats(tierValue);
        generateName();
        displayMessage();

        Debug.Log("This gear's tier is " + tier);
    }

    private void generateStats(int tierValue) {
        int atkValue = tierValue * Settings.globalRNG.Next(tierValue, 10);
        int defValue = tierValue * Settings.globalRNG.Next(tierValue, 10);

        switch(type) {
            case EquipType.HELMET:
                atkbonus += (int)atkValue / 4;
                defbonus += (int)defValue;
                break;
            case EquipType.ARMOR:
                atkbonus += (int)atkValue / 2;
                defbonus += (int)defValue;
                break;
            case EquipType.GLOVE:
                atkbonus += (int)atkValue / 2;
                defbonus += (int)defValue / 3;
                break;
            case EquipType.BOOT:
                atkbonus += (int)atkValue / 3;
                defbonus += (int)defValue / 2;
                break;
            case EquipType.WEAPON:
                atkbonus += (int)atkValue;
                defbonus += (int)defValue / 5;
                break;

            default:
                break;
        }

        Debug.Log("ITEM ATTACK: " +  atkbonus);
        Debug.Log("ITEM DEFENCE: " + defbonus);
    }

    private void generateName() {
    
        string adj = equipmentAdjectives.ToArray()[Settings.globalRNG.Next(0, equipmentAdjectives.Count)];
        string noun = equipmentNouns.ToArray()[Settings.globalRNG.Next(0, equipmentNouns.Count)];
        if(type == EquipType.WEAPON)
            completeName = adj + " " + CharacterClassOptions.getWeaponType((this as EquipWeapon).weaponClass) + " of " + noun;
        else {
           completeName = adj + " " + name + " of " + noun; 
        }
        Debug.Log("Name of equipment is : " + completeName);
    }

    public void displayMessage() {
        UI_TextAlert.DisplayColorText(owner + " recieved " + completeName, tier);
    }

    public void initWords() {
       equipmentAdjectives = new List<string>();
       equipmentNouns = new List<string>();

        TextAsset EquipmentAdjectives = Resources.Load<TextAsset>("EquipmentAdjectives");
        TextAsset EquipmentNouns = Resources.Load<TextAsset>("EquipmentNouns");

        adjectives = EquipmentAdjectives.text.Split("\n"[0]);
        nouns = EquipmentNouns.text.Split("\n"[0]);

        for(int i = 0; i < adjectives.Length; i++) {  
            equipmentAdjectives.Add(adjectives[i]);
        }
        
        for(int i = 0; i < nouns.Length; i++) {  
            equipmentNouns.Add(nouns[i]);
        }

    }

}

public class Helmet : EquipItem {
	public static string _ID = "3";
	public Helmet() : base("Helmet", _ID, EquipType.HELMET, 0, 10) {}
}

public class Armor : EquipItem {
	public static string _ID = "4";
	public Armor() : base("Armor", _ID, EquipType.ARMOR, 0, 20) {}
}

public class Glove : EquipItem {
	public static string _ID = "5";
	public Glove() : base("Gloves", _ID, EquipType.GLOVE, 0, 8) {}
}

public class Boot : EquipItem {
	public static string _ID = "6";
	public Boot() : base("Boots", _ID, EquipType.BOOT, 0, 8) {}
}

public class EquipWeapon : EquipItem {
	public static string _ID = "7";
   
	public EquipWeapon(int weaponClass) : base(CharacterClassOptions.getWeaponType((weaponClass)), _ID, EquipType.WEAPON, 20, 0) {
        this.weaponClass = weaponClass;
    }

}
