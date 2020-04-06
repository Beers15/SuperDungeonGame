using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerListStatic : MonoBehaviour
{
    public List<ServerListing> serversList = new List<ServerListing>();
    public GameObject listingPrefab;

    void Start()
    {
        //Destroy initial placeholders
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GenerateServerListing();
    }

    void Update()
    {
        
    }

    public void GenerateServerListing()
    {
        foreach(ServerListing sl in serversList)
        {
            GameObject newObj = Instantiate(listingPrefab);
            //Fix up object to make it visible and everything
            newObj.transform.SetParent(this.transform);
            newObj.transform.localScale = Vector3.one;
            newObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            newObj.GetComponent<ServerEntryHandler>().nameText.text = sl.name;
            newObj.GetComponent<ServerEntryHandler>().fullIP = sl.fullIP;
        }
    }
}

[System.Serializable]
public class ServerListing
{
    public string fullIP = "54.151.117.51:1337";
    public string name = "Official Server 01";
    public int currentPlayers = 0;
    public int maxPlayers = 4;
}
