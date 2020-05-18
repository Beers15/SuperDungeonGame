using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class UI_ToolTipDisplay : MonoBehaviour
{
    GameObject displayBackground;
    public Image itemImage;
    public TextMeshProUGUI text;
    EquipItem temp;
    GameObject item;
    UI_InventoryManager manager;
	public List<UI_InventorySlot> inventorySlots;
	int selectSlot;
    bool active;

    public GameObject attackDisplay, defenseDisplay;
    public TextMeshProUGUI attackDisplayValue;
    public TextMeshProUGUI defenseDisplayValue;
    Player playerMain;

    public Sprite healthPot, manaPot, helmet, armor, gloves, boots, bow, sword, staff, tome, gem, food, scroll, nothing;


    private void Awake()
    {
        displayBackground = transform.Find("DisplayBackground").gameObject;
        manager = transform.parent.Find("InventoryMenu").GetComponent<UI_InventoryManager>();
        displayBackground.SetActive(false);
        active = false;
    }
    // Start is called before the first frame update
    void Start()
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
        selectSlot = manager.getSlot();
    //    Debug.Log("the slot was " + selectSlot);
    //    Debug.Log("the item in the slot was " + inventorySlots[selectSlot]);
        this.setImage(selectSlot);
        if (itemImage == null)
        {
            Color temp = itemImage.color;
            temp.a = 0f;
            itemImage.color = temp;
        }
        if (selectSlot > 17)
        {
            switch (selectSlot)
            {
                case 18:
                    text.text = playerMain.inventory.helmet.completeName;
                    attackDisplay.SetActive(true);
                    defenseDisplay.SetActive(true);
                    attackDisplayValue.SetText(playerMain.inventory.helmet.atkbonus.ToString());
                    defenseDisplayValue.SetText(playerMain.inventory.helmet.defbonus.ToString());
                    break;
                case 19:
                    text.text = playerMain.inventory.armor.completeName;
                    attackDisplay.SetActive(true);
                    defenseDisplay.SetActive(true);
                    attackDisplayValue.SetText(playerMain.inventory.armor.atkbonus.ToString());
                    defenseDisplayValue.SetText(playerMain.inventory.armor.defbonus.ToString());
                    break;
                case 20:
                    text.text = playerMain.inventory.gloves.completeName;
                    attackDisplay.SetActive(true);
                    defenseDisplay.SetActive(true);
                    attackDisplayValue.SetText(playerMain.inventory.gloves.atkbonus.ToString());
                    defenseDisplayValue.SetText(playerMain.inventory.gloves.defbonus.ToString());
                    break;
                case 21:
                    text.text = playerMain.inventory.boots.completeName;
                    attackDisplay.SetActive(true);
                    defenseDisplay.SetActive(true);
                    attackDisplayValue.SetText(playerMain.inventory.boots.atkbonus.ToString());
                    defenseDisplayValue.SetText(playerMain.inventory.boots.defbonus.ToString());
                    break;
                case 22:
                    text.text = playerMain.inventory.weapon.completeName;
                    attackDisplay.SetActive(true);
                    defenseDisplay.SetActive(true);
                    attackDisplayValue.SetText(playerMain.inventory.weapon.atkbonus.ToString());
                    defenseDisplayValue.SetText(playerMain.inventory.weapon.defbonus.ToString());
                    break;

            }
        }
        else if (selectSlot >= 0)
        {
            text.text = inventorySlots[selectSlot].slotItem.name;
            if (inventorySlots[selectSlot].slotItem is EquipItem)
            {
                attackDisplay.SetActive(true);
                defenseDisplay.SetActive(true);
                EquipItem equipment = inventorySlots[selectSlot].slotItem as EquipItem;
                attackDisplayValue.SetText(equipment.atkbonus.ToString());
                defenseDisplayValue.SetText(equipment.defbonus.ToString());
                string foo = equipment.completeName.ToString();
                text.text = foo;
            }
            else
            {
                text.text = inventorySlots[selectSlot].slotItem.name;
                attackDisplay.SetActive(false);
                defenseDisplay.SetActive(false);
            }
        }
    }

    public void setImage(int selectSlot)
    {
        if (selectSlot > 17)
        {
            switch (selectSlot)
            {
                case 18:
                    itemImage.sprite = helmet; break;
                case 19:
                    itemImage.sprite = armor; break;
                case 20:
                    itemImage.sprite = gloves; break;
                case 21:
                    itemImage.sprite = boots; break;
                case 22:
                    switch (playerMain.inventory.weapon.name)
                    {
                        case "Sword":
                            itemImage.sprite = sword; break;
                        case "Staff":
                            itemImage.sprite = staff; break;
                        case "Bow":
                            itemImage.sprite = bow; break;
                    }
                    break;
            }
        }
		else if (selectSlot >= 0)
        {
            switch (inventorySlots[selectSlot].itemName)
            {
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
                case "Tome":
                    itemImage.sprite = tome; break;
                case "Gem":
                    itemImage.sprite = gem; break;
                case "Food":
                    itemImage.sprite = food; break;
                case "Scroll":
                    itemImage.sprite = scroll; break;
                default:
                    itemImage.sprite = nothing; break;
            }
        }
    }

    public void closeDisplay()
    {
        displayBackground.SetActive(false);
    }

	
}
