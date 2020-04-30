using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoldDisplay : MonoBehaviour {	
	Image goldImage;
	Text goldCount;
    Item slotItem;
	UI_InventoryManager manager;
	

	void Awake() {
		manager = transform.parent.parent.GetComponent<UI_InventoryManager>();
		goldCount = transform.Find("GoldText").GetComponent<Text>();  
	}
	
	public void setItem(Item item) 	{
		if (item == null) return;
	
		goldCount.text = item.Amount.ToString();
	}
}
