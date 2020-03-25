using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassIcon : MonoBehaviour
{
	GameObject warrior, mage, hunter, healer;
	GameObject active;
	
	public void Awake()
	{
		warrior = transform.Find("Warrior").gameObject;
		mage 	= transform.Find("Mage").gameObject;
		hunter 	= transform.Find("Hunter").gameObject;
		healer 	= transform.Find("Healer").gameObject;
		resetIcons();
	}
	
	private void resetIcons()
	{
		warrior.SetActive(false);
		mage.SetActive(false);
		hunter.SetActive(false);
		healer.SetActive(false);
	}
	
    public void SetActiveIcon(string classname)
	{
		resetIcons();
		switch (classname) {
			case "Warrior": warrior.SetActive(true); break;
			case "Mage": mage.SetActive(true); break;
			case "Hunter": hunter.SetActive(true); break;
			case "Healer": healer.SetActive(true); break;
		}
	}
	
	/*public void SetTint(Color color)
	{
		mage.GetComponent<Image>().color = color;
		warrior.GetComponent<Image>().color = color;
		hunter.GetComponent<Image>().color = color;
		healer.GetComponent<Image>().color = color;
	}*/
}
