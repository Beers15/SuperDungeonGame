using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public interface Environment
{
    void init_environment(Pos grid_pos, int health=0);
}
