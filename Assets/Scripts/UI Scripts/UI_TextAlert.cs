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
	
	public static void DisplayText(string message, float waitTime = 2.0f )
	{
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
