using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedConfig : MonoBehaviour 
{

    public bool useRandomSeed = false;
    public static SeedConfig instance;
    // this code will create and set 
    // a random seed for single player and multiplayer
    public void Start()
	{
        instance = this;
    }

    public static void setSeed()
    {	
        if(instance.useRandomSeed)
			{
				var random = new System.Random();
				Settings.MasterSeed = random.Next();
			}
    }
}
