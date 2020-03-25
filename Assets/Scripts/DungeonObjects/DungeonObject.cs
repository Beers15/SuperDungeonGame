using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

// all objects on the map have a grid position
public abstract class DungeonObject : MonoBehaviour
{
    public Pos grid_pos;
}
