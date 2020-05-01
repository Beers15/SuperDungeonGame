using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
	private int characterClass;
	private List<GameObject> portraits;
	private Text nameText;
	private Image image;
	private Color originalColor;

	void Awake()
	{
		portraits = new List<GameObject>();
		portraits.Add(transform.Find("WarriorTurn").gameObject);
		portraits.Add(transform.Find("MageTurn").gameObject);
		portraits.Add(transform.Find("HunterTurn").gameObject);
		portraits.Add(transform.Find("HealerTurn").gameObject);

		foreach (GameObject portrait in portraits)
		{
		//	portrait.SetActive(false);
		}

		nameText = transform.Find("Nickname").gameObject.GetComponent<Text>();
	}

	public void SetClass(int charClass)
	{
		Debug.Log("SetClass given: " + charClass);
		characterClass = charClass;
		if (charClass == CharacterClassOptions.Knight)
		{
			portraits[0].SetActive(true);
			portraits[1].SetActive(false);
			portraits[2].SetActive(false);
			portraits[3].SetActive(false);
			image = portraits[0].GetComponent<Image>();
			originalColor = image.color;
			Debug.Log("Warrior Portrait activate!");
		}
		else if (charClass == CharacterClassOptions.Mage)
		{
			portraits[0].SetActive(false);
			portraits[1].SetActive(true);
			portraits[2].SetActive(false);
			portraits[3].SetActive(false);
			image = portraits[1].GetComponent<Image>();
			originalColor = image.color;
			Debug.Log("Mage Portrait activate!");
		}
		else if (charClass == CharacterClassOptions.Hunter)
		{
			portraits[0].SetActive(false);
			portraits[1].SetActive(false);
			portraits[2].SetActive(true);
			portraits[3].SetActive(false);
			image = portraits[2].GetComponent<Image>();
			originalColor = image.color;
			Debug.Log("Hunter Portrait activate!");
		}
		else if (charClass == CharacterClassOptions.Healer)
		{
			portraits[0].SetActive(false);
			portraits[1].SetActive(false);
			portraits[2].SetActive(false);
			portraits[3].SetActive(true);
			image = portraits[3].GetComponent<Image>();
			originalColor = image.color;
			Debug.Log("Healer Portrait activate!");
		}
		else
		{
			Debug.Log("Unknown character class passed to TurnIndicator.SetClass: " + charClass);
		}
	}

	public void SetName(string name)
	{
		nameText.text = name;
	}

	public void SetActiveTurn(bool active)
	{
		if (active)
		{
			image.color = originalColor;
		}
		else
		{
			image.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
		}
	}
}
