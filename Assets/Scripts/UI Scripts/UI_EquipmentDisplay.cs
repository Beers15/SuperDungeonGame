using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentDisplay : MonoBehaviour
{
	public UI_EquipmentSlot HelmetSlot, ChestSlot, HandSlot, FootSlot, WeaponSlot;
	public UI_ToolTipDisplay tooltipDisplay;
	GameObject display;
	public GameObject tooltipScreen;
	Player playerMain;
	public int selectedSlotIndex = -1;

	// Start is called before the first frame update
	void Awake()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (playerMain == null)
		{
			playerMain = Network.getPlayer(NetworkManager.clientID);
			return;
		}
		if (playerMain.inventory.helmet != null)
		{
			HelmetSlot.SetItem(playerMain.inventory.helmet);

		}else if(playerMain.inventory.armor != null)
		{
			ChestSlot.SetItem(playerMain.inventory.armor);

		}else if(playerMain.inventory.gloves != null)
		{
			HandSlot.SetItem(playerMain.inventory.gloves);

		}else if(playerMain.inventory.boots != null){

			FootSlot.SetItem(playerMain.inventory.boots);

		}else if(playerMain.inventory.weapon != null)
		{
			WeaponSlot.SetItem(playerMain.inventory.weapon);
		}
	}

	public void TriggerSlot(int index)
	{
		selectedSlotIndex = index;
		Debug.Log("shit has been triggered, index: " + index);
		tooltipScreen.SetActive(true);

	}

	public void DropItem()
	{
		if (selectedSlotIndex == -1) return;
		playerMain.inventory.DecrementItemAtSlot(selectedSlotIndex);
	}

	public void UseItem()
	{
		if (selectedSlotIndex == -1) return;
		Network.submitCommand(new UseItemCommand(NetworkManager.clientID, selectedSlotIndex));
	}

	public void Back()
	{
		display.SetActive(false);
		tooltipScreen.SetActive(false);
	}

	public int getSlot()
	{
		return this.selectedSlotIndex;
	}
}
