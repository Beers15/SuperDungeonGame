using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TextAlert : MonoBehaviour
{
    Text alert;
	static UI_TextAlert instance;
	bool display = false;
	
	void Awake()
	{
		alert = GetComponent<Text>();
		instance = this;
	}
	
	void Update()
	{
		alert.enabled = display;
	}
	
	public static void DisplayText(string message, float waitTime = 5.0f )
	{   
		instance.alert.GetComponent<Text>().color = Color.white; 
		instance.alert.text = message;
		instance.display = true;
		instance.StartCoroutine(instance.waitForDisplayEnd(waitTime));
	}
	
	public static void DisplayColorText(string message, int tier, float waitTime = 5.0f)
	{
		switch(tier) {
			case 1:
				instance.alert.GetComponent<Text>().color = Color.white; break;
			case 2:
				instance.alert.GetComponent<Text>().color = Color.green; break;
			case 3:
				instance.alert.GetComponent<Text>().color = Color.blue; break;
			case 4:
				instance.alert.GetComponent<Text>().color = Color.magenta; break;
			case 5: 
				instance.alert.GetComponent<Text>().color = Color.red; break;
			default:
				instance.alert.GetComponent<Text>().color = Color.grey; break;
			break;
		}

		instance.alert.text = message;

		instance.display = true;
		instance.StartCoroutine(instance.waitForDisplayEnd(waitTime));
	}
	
	
	IEnumerator waitForDisplayEnd(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		display = false;
	}
}

//orange new Color32( 254 , 161 , 0, 1 ) },
//purpel new Color32( 143 , 0 , 254, 1 )
//blue new Color32( 0 , 122 , 254, 1 )
//green new Color32( 0 , 254 , 111, 1 ) 
//white