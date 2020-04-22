using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerEntryHandler : MonoBehaviour
{
    public Text nameText;
    public string fullIP;

    void Start()
    {
        
    }

    public void JoinThis()
    {
        UI_MultiplayerJoin multijoin = FindObjectOfType<UI_MultiplayerJoin>();

        multijoin.entry.text = fullIP;
        FindObjectOfType<EntryBar>().gameObject.GetComponent<InputField>().text = fullIP;
        Debug.Log("Pressed on join: " + fullIP);
        multijoin.Connect();
    }

}
