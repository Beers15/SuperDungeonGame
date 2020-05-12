﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryManager : MonoBehaviour {
	List<UI_InventorySlot> inventorySlots;
	List<UI_InventorySlot> equipSlots;
	UI_GoldDisplay goldDisplay;
	public UI_ToolTipDisplay tooltipDisplay;
	GameObject display;
	public GameObject tooltipScreen;
	Player playerMain;
	public int selectedSlotIndex = -1;
	
    // Start is called before the first frame update
    void Awake() {
		int index = 0;
		
		inventorySlots = new List<UI_InventorySlot>();
		display = transform.Find("MenuBackground").gameObject;
		Debug.Log(display.name);
		
        foreach (Transform child in display.transform.Find("InventorySlots")) {
			var slot = child.GetComponent<UI_InventorySlot>();
			slot.index = index++;
			inventorySlots.Add(slot);
		}

		goldDisplay =  transform.Find("GoldDisplay").gameObject.GetComponent<UI_GoldDisplay>();

		display.SetActive(true);
		
		/*equipSlots = new List<UI_InventorySlot>();
		foreach (Transform child in transform.Find("EquipSlots"))  {
			var equip = child.GetComponent<UI_InventorySlot>();
			equip.index = index++;
			equipSlots.Add(equip);
		}*/
			
		
    }

    // Update is called once per frame
    void Update() {
		if (playerMain == null) {
			playerMain = Network.getPlayer(NetworkManager.clientID);
			return;
		}
        for (int i = 0; i < inventorySlots.Count; i++) {
			UI_InventorySlot slot = inventorySlots[i];
			slot.SetItem(playerMain.inventory.GetItemFromSlot(i));
		}

		goldDisplay.setGold(playerMain.inventory.getGold());

		// if (Input.GetKeyDown("space")) {
		// 	display.SetActive(!display.activeSelf);
		// }
    }
	
	public void TriggerSlot(int index) {
		selectedSlotIndex = index;
		tooltipScreen.SetActive(true);

	}
	
	public void DropItem() {
		if (selectedSlotIndex == -1) return;
		playerMain.inventory.DecrementItemAtSlot(selectedSlotIndex);
	}
	
	public void UseItem() {
		if (selectedSlotIndex == -1) return;
		Network.submitCommand(new UseItemCommand(NetworkManager.clientID, selectedSlotIndex));
	}
	
	public void Back() {
		display.SetActive(false);
		tooltipScreen.SetActive(false);
	}

	public int getSlot()
	{
		return this.selectedSlotIndex;
	}
}
