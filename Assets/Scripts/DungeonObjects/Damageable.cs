﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    void take_damage(int amount, int classOfAttacker);
	void playHitAnimation();
	void playHitNoise(string type);
}
