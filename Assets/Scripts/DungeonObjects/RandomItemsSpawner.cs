using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;

public class RandomItemsSpawner : DungeonObject, Interactable, Environment, Renderable, Damageable {
    public AudioClip itemSFX;
    public string modelName;
    public int itemIndex;
    private AudioSource source;
	private GameObject itemObject;
    private int slainEnemyLvl;

	public void init_environment(Pos grid_pos, int health=1) {
		this.grid_pos = grid_pos;

        switch(itemIndex) {
            case 0:
                itemObject = GameObject.Find("SM_Prop_Chest_01"); break;
            case 1:
                itemObject = GameObject.Find("SM_Item_Potion_01"); break;
            case 2:
                itemObject = GameObject.Find("SM_Item_Potion_06"); break;
            case 3:
                itemObject = GameObject.Find("SM_Item_Coins_01"); break;
            default:
                itemObject = GameObject.Find("SM_Prop_Chest_01"); break;
        }
        source = GetComponent<AudioSource>();
	}

	void Update() {
		if (FogOfWar.IsSemiVisible(grid_pos))
			EnableRendering();
		else
			DisableRendering();
	}
	
    public void EnableRendering() {
		itemObject.SetActive(true);
	}

	public void DisableRendering() {
		itemObject.SetActive(false);
	}

	public void interact(GameAgent interactor) {
        source.PlayOneShot(itemSFX);

        int randomItemAmount;

		//set # of items recived, or set amount of gold based on slain enemy's lvl
        if(itemIndex != 3)
		    randomItemAmount = UnityEngine.Random.Range(1, 3);
        else {
            int goldRoll = UnityEngine.Random.Range(1, 10);
            if(goldRoll > 9) 
                randomItemAmount =  UnityEngine.Random.Range(1, 100 * slainEnemyLvl);
            else
                randomItemAmount =  UnityEngine.Random.Range(250, 500 * slainEnemyLvl);
        }

		Item toAdd;
		bool isConsumable = false;
		
        //itemIndex set in inspector should match number for that item here
		switch (itemIndex) {
			case 1:
				toAdd = new HealthPot(randomItemAmount); isConsumable = true; break;
			case 2:
				toAdd = new ManaPot(randomItemAmount); isConsumable = true; break;
			case 3:
				toAdd = new Gold(randomItemAmount); break;
			default:
				Debug.Log("NULL IN SPAWNER"); toAdd = null; break;
		}
		if (toAdd == null) return;
		
		//pluarlize text alert
		if(randomItemAmount > 1)
			UI_TextAlert.DisplayText("Received " + toAdd.Amount + " " + toAdd.Name + "s");
		else
			UI_TextAlert.DisplayText("Received " + toAdd.Name);

		//add to consumable/potions storage if consumable, otherwise add to normal inventory
		if(isConsumable) {
			interactor.potions.AddConsumable((ConsumableItem)toAdd);
			interactor.potions.display();
		} else {
			interactor.inventory.AddItem(toAdd);
			interactor.inventory.display();
		}

		GameManager.kill(this, 0.5f);
	}

    public int getLevel() {
        return slainEnemyLvl;
    }

    public void setLevel(int level) {
        slainEnemyLvl = level;
    }

	public void take_damage(int amount) {
		GameManager.kill(this, 1.0f);
	}
	public void playHitAnimation() {}
	public void playHitNoise(string noise) {}

	public void setLvlOfSlainMob(int level) {
		slainEnemyLvl = level;
		Debug.Log("LVL OF SLAIN MOB DROPPING ITEM IS : " + slainEnemyLvl);
	}
}
