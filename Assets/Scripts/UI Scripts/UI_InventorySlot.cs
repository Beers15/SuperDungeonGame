﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour {	
	Image itemImage;
	Text itemCount;
    Item slotItem;
	UI_InventoryManager manager;
	
	public Sprite healthPot, manaPot, helmet, armor, gloves, boots, bow, sword, staff, nothing;
	
	public int index = 0;
	
	void Awake() {
		manager = transform.parent.parent.parent.GetComponent<UI_InventoryManager>();
		itemImage = transform.Find("Item").GetComponent<Image>();
		itemCount = transform.Find("ItemCount").GetComponent<Text>();
	}
	
	public void TriggerSlot() {
		manager.TriggerSlot(this.index);
	}
	
	public void SetItem(Item item) {
		if (item == null) return;
	
		slotItem = item;
		itemCount.text = item.Amount.ToString();
		itemImage.color = new Color(1, 1, 1, 1);
		Debug.Log("IN INVENTORY SLOT THE SPRITE NAME IS " +item.name);
		switch (item.name) {
			case "Health Potion":
				itemImage.sprite = healthPot; break;
			case "Mana Potion":
				itemImage.sprite = manaPot; break;
			case "Helmet":
				itemImage.sprite = helmet; break;
			case "Armor":
				itemImage.sprite = armor; break;
			case "Gloves":
				itemImage.sprite = gloves; break;
			case "Boots":
				itemImage.sprite = boots; break;
			case "Bow":
				itemImage.sprite = bow; break;
			case "Sword":
				itemImage.sprite = sword; break;
			case "Staff":
				itemImage.sprite = staff; break;
			default:
				itemImage.sprite = nothing; break;
		}
		//no amount displayed for empty item slots
		if(string.Compare(item.ID, "-1") == 0)
			itemCount.text = " ";
	}
}
