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

	public EquipItem() {}
    public EquipItem(string name, string id, EquipType etype, int atk, int def) {
        initWords();
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
        
    public void generateEquipmentValues(int level) {
        lvlOfSlainMob = level;
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
            tierValue = 2; //magic
        }
        else
            tierValue = 1; //common

        tier = tierValue;
        //TODO
        //generateStats();
        generateName();
        displayMessage();

        Debug.Log("This gear's tier is " + tier);
    }

    // private void generateStats() {

    // }
    private void generateName() {
    
        string adj = equipmentAdjectives.ToArray()[UnityEngine.Random.Range(0, equipmentAdjectives.Count - 1)];
        string noun = equipmentNouns.ToArray()[UnityEngine.Random.Range(0, equipmentNouns.Count - 1)];

        completeName = adj + " " + Name + " of " + noun;
        Debug.Log("Name of equipment is : " + completeName);
    }

    public void displayMessage() {
        UI_TextAlert.DisplayText("Received " + completeName);
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
	public Helmet() : base("helmet", _ID, EquipType.HELMET, 0, 10) {}
}

public class Armor : EquipItem {
	public static string _ID = "4";
	public Armor() : base("armor", _ID, EquipType.ARMOR, 0, 10) {}
}

public class Glove : EquipItem {
	public static string _ID = "5";
	public Glove() : base("gloves", _ID, EquipType.GLOVE, 0, 10) {}
}

public class Boot : EquipItem {
	public static string _ID = "6";
	public Boot() : base("boot", _ID, EquipType.BOOT, 0, 10) {}
}

public class EquipWeapon : EquipItem {
	public static string _ID = "7";
	public EquipWeapon() : base("weapon", _ID, EquipType.WEAPON, 10, 0) {}
}
