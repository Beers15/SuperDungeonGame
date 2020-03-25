using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{	
	Image itemImage;
	Text itemCount;
    Item equippedItem;
	UI_InventoryManager manager;
	
	public Sprite healthPot, manaPot, helmet, armor, glove, boots;
	
	public int index = 0;
	
	void Awake()
	{
		// finds the great great grandparent of this inventory slot!
		// man is this code getting sloppy
		manager = transform.parent.parent.parent.GetComponent<UI_InventoryManager>();
		itemImage = transform.Find("Item").GetComponent<Image>();
		itemCount = transform.Find("ItemCount").GetComponent<Text>();
	}
	
	public void TriggerSlot()
	{
		manager.TriggerSlot(this.index);
	}
	
	public void SetItem(Item item) 
	{
		if (item == null) return;
		equippedItem = item;
		itemCount.text = item.Amount.ToString();
		itemImage.color = new Color(1, 1, 1, 1);
		switch (item.Name) {
			case "Health Potion":
				itemImage.sprite = healthPot; break;
			case "Mana Potion":
				itemImage.sprite = manaPot; break;
			case "Helmet":
				itemImage.sprite = helmet; break;
			case "Armor":
				itemImage.sprite = armor; break;
			case "Boot":
				itemImage.sprite = boots; break;
		}
	}
}
