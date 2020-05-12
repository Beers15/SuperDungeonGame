﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoldDisplay : MonoBehaviour {	
	Image goldImage;
	Text goldCount;
    Item slotItem;
	UI_InventoryManager manager;
	

	void Awake() {
		manager = GameObject.Find("InventoryMenu").GetComponent<UI_InventoryManager>();
		goldCount = transform.Find("GoldText").GetComponent<Text>();  
	}
	
	public void setGold(Item gold) 	{
		if (gold == null) return;
	
		goldCount.text = gold.Amount.ToString();
	}
}
