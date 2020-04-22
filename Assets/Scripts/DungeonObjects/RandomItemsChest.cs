using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;

public class RandomItemsChest : DungeonObject, Interactable, Environment, Renderable, Damageable {
    public AudioClip chestOpeningSFX;
    private AudioSource source;
	private GameObject chestObject;
	private int slainEnemyLvl = 2; //default to 2 for spawned chest not dropped from slain enemies

	public void init_environment(Pos grid_pos, int health=1) {
		this.grid_pos = grid_pos;
		chestObject = transform.Find("SM_Prop_Chest_01").gameObject;
        source = GetComponent<AudioSource>();
	}

	void Update() {
		if (FogOfWar.IsSemiVisible(grid_pos))
			EnableRendering();
		else
			DisableRendering();
	}
	
    public void EnableRendering() {
		chestObject.SetActive(true);
	}

	public void DisableRendering() {
		chestObject.SetActive(false);
	}

	string[] itemOptions = { "health", "mana", "gold", "helmet", "armor", "gloves", "boot" };
	public void interact(GameAgent interactor)
	{
        source.PlayOneShot(chestOpeningSFX);

		int randomItemIndex = UnityEngine.Random.Range(0, itemOptions.Length);
		int randomItemAmount = UnityEngine.Random.Range(1, 7);
		string itemChoice = itemOptions[randomItemIndex];

		Item toAdd;
		bool isConsumable = false;
		bool notEquipItem = true;
		
		switch (itemChoice) {
			case "health":
				toAdd = new HealthPot(randomItemAmount); isConsumable = true; break;
			case "mana":
				toAdd = new ManaPot(randomItemAmount); isConsumable = true; break;
			case "gold":
				toAdd = new Gold(randomItemAmount);	break;
			case "helmet":
				toAdd = new Helmet(); 
				(toAdd as EquipItem).setLvl(slainEnemyLvl);
				notEquipItem = false;
				break;
			case "armor":
				toAdd = new Armor(); 
				(toAdd as EquipItem).setLvl(slainEnemyLvl);
				notEquipItem = false;
				break;
			case "gloves":
				toAdd = new Glove(); 
				(toAdd as EquipItem).setLvl(slainEnemyLvl);
				notEquipItem = false;
				break;
			case "boots":
				toAdd = new Boot(); 
				(toAdd as EquipItem).setLvl(slainEnemyLvl);
				notEquipItem = false;
				break;
			default:
				toAdd = null; break;
		}
		if (toAdd == null) return;
		
		//pluarlize text alert
		if(randomItemAmount >= 2 && notEquipItem)
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
	
	public void take_damage(int amount) {
		GameManager.kill(this, 1.0f);
	}
	public void playHitAnimation() {}
	public void playHitNoise(string noise) {}

	public void setLvlOfSlainMob(int level) {
		slainEnemyLvl = level;
	}
}

