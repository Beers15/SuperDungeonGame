using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
	private static System.Random seedRNG = new System.Random(MasterSeed);
	private static int _MasterSeed = 1111111;
	public static int MasterSeed {
		get {
			return _MasterSeed;
		}
		set {
			_MasterSeed = value;
			seedRNG = new System.Random(_MasterSeed);
		}
	}
	public static int MapSeed {
		get {
			return seedRNG.Next();
		}
	}
	public static System.Random globalRNG = new System.Random(MasterSeed);
	public static float Volume = 0;
	public static float AnimationSpeed = 10;
}
