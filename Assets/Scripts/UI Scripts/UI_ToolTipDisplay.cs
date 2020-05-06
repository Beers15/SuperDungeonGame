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

    public Sprite healthPot, manaPot, helmet, armor, gloves, boots, bow, sword, staff, nothing;


    private void Awake()
    {
        displayBackground = transform.Find("DisplayBackground").gameObject;
        manager = transform.parent.Find("InventoryMenu").GetComponent<UI_InventoryManager>();
        displayBackground.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        item = transform.parent.Find("Item").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		selectSlot = manager.getSlot();
        Debug.Log("the slot was " + selectSlot);
        Debug.Log("the item in the slot was " + inventorySlots[selectSlot]);
        this.setImage(selectSlot);
        if (itemImage == null)
        {
            Color temp = itemImage.color;
            temp.a = 0f;
            itemImage.color = temp;
        }
        text.text = inventorySlots[selectSlot].itemName; 

    }

    public void setImage(int selectSlot)
    {
        Debug.Log("the item in the slot was " + inventorySlots[selectSlot].itemName);
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
            default:
                itemImage.sprite = nothing; break;
        }
    }

	
}
