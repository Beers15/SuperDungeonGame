using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GetPlayerHS : MonoBehaviour
{
    public string getDataFromURL = "http://18.144.85.192/display.php";
    public string dataText;

    //"Reverse" order arrays due to find specifics.
    public GetPlayerTop[] GPT;
    public string[] playerStrings; 

    void Start()
    {
        StartCoroutine(GetTextFromWWW());

        GPT = this.GetComponentsInChildren<GetPlayerTop>();
    }

    IEnumerator GetTextFromWWW()
    {
        WWW www = new WWW(getDataFromURL);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Ooops, something went wrong...");
        }
        else
        {
            dataText = www.text;
            ProcessData();
        }
    }

   public void ProcessData()
   {
    playerStrings = dataText.Split('@');

        var i = 0;
        foreach(GetPlayerTop g in GPT)
            {
                if(i > playerStrings.Count())
                {

                }   
                else
                    {
                    g.gameObject.GetComponent<Text>().text = playerStrings[i];
                    i++;
                    }
                }
    }
}
