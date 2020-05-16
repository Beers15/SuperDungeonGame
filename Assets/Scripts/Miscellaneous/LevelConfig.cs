using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;
using RegionUtils;

public class LevelConfig : MonoBehaviour
{

#region Variables
  
 
    // Map variables.
    float cell_size;
    private System.Random rng;
    int width;
    int height;
    enum environmentType { traversableFolliage, nonTraversableFolliage, traversableObject, nonTraversableObject, traversableStructure, nonTraversableStructure, particle, portal };
    enum portraitType { folliage, orc, undead };
    List<Pos> paintedList = new List<Pos>();

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableFolliagDensity = 0.10f;
    public float nonTraversableFolliageDensity = 0.10f;
    public float traversableObjectDensity = 0.10f;
    public float nonTraversableObjectDensity = 0.0f;
    public float traversableStructureDensity = 0.0f;
    public float nonTraversableStructureDensity = 0.0f;
    public float particleDensity = 0.0f;

 
    [Header("Portrait Settings")]
    public int maxRadius = 5;
    public int minRadius = 1;
    public int minArea = 10;
    public int smallArea = 30;
    public int minPortraitMargin = 2;
    public int maxPortraitMargin = 5;
    static int structureRadius = 1;
    public float structureDensity = 0.1f;
    public float objectDensity = 0.3f;
    public float rubbleDensity = 0.5f;
	
    [Header("Portrait Prefabs")]
    public GameObject[] folliageStructures;
    public GameObject[] folliageObjects;
    public GameObject[] folliageRubble;
    public GameObject[] rubble;
    public GameObject[] orcStructures;
    public GameObject[] orcObjects;
    public GameObject[] orcRubble;
    public GameObject[] undeadStructures;
    public GameObject[] undeadObjects;
    public GameObject[] undeadRubble;

    [Header("Environment Prefabs")]
    public GameObject[] traversableFolliageObject;
    public GameObject[] nonTraversableFolliageObject;
    public GameObject[] traversableObjectObject;
    public GameObject[] nonTraversableObjectObject;
    public GameObject[] traversableStructureObject;
    public GameObject[] nonTraversableStructureObject;
    public GameObject[] particleObject;
    #endregion


}