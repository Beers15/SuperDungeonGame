using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    HPPOTION,
    MPPOTION,
}

public class Consumable : Item
{
    private int restoreAmount;
    private ConsumableType ctype;

    public Consumable(string name, int id, int max, int amt, int res, ConsumableType type)
    {
        Name = name;
        ID = id;
        maxAmount = max;
        Amount = amt;
        restoreAmount = res;
        ctype = type;
    }
}
