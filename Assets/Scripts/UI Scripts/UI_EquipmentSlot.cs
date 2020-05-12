using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentSlot : MonoBehaviour
{
	public Image itemImage;
	public Item slotItem;
	public string itemName;
	UI_EquipmentDisplay manager;

	public Sprite bow, sword, staff, helmet, armor, gloves, boots;

	public int index = 0;

	void Awake()
	{
	}

	public void TriggerSlot()
	{
		manager.TriggerSlot(this.index);
	}

	public void SetItem(Item item)
	{
		if (item == null) return;

		slotItem = item;
		itemImage.color = new Color(1, 1, 1, 1);

		itemName = item.name;
		//	Debug.Log("IN INVENTORY SLOT THE SPRITE NAME IS " +item.name);
		switch (item.name)
		{
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
		}
	}

	public Sprite getImage()
	{
		return this.itemImage.sprite;
	}

	public Item getItem()
	{
		return this.slotItem;
	}
}
