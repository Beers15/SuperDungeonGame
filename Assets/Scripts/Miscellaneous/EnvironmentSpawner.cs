using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;
using RegionUtils;

public class EnvironmentSpawner : MonoBehaviour
{

    #region Variables
    // Referenced conponents.
    MapManager mapManager;
    MapConfiguration mapConfiguration;

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

    [Header("Portal Settings")]
    public GameObject startPortal;
    public GameObject exitPortal;
    public float portalMargin = 0.50f;
    public int spawnAreaRadius = 3;
    public int reserveAreaRadius = 3;

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
	
	[Header("Base Environment Prefab")]
	public GameObject basePrefab;

    [Header("Portrait Prefabs")]
    public GameObject[] folliageStructures;
    public GameObject[] folliageObjects;
    public GameObject[] folliageRubble;
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
    List<GameObject> allEnvironmentObject = new List<GameObject>();
    #endregion

    public void Init(MapManager mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        this.cell_size = config.cell_size;
        this.mapManager = mapManager;
        rng = new System.Random(Settings.MasterSeed);
        //spawnPortals();
		paintedList.Clear();
        spawnEnvironment();
    }

    #region Portraits

    void spawnPortrait(Pos position, portraitType type)
    {
        int radius = rng.Next(minRadius, maxRadius);

        Pos startPos = new Pos(position.x - radius, position.y - radius);
        Pos endPos = new Pos(position.x + radius, position.y + radius);
        List<Pos> validPosList = getListOfValidPositions(startPos, endPos);
        List<Pos> rubblePosList = validPosList;

        int targetRubbleQuota = (int)(validPosList.Count * rubbleDensity);
        int targetStructureQuota = (int)(validPosList.Count * structureDensity);
        int targetObjectQuota = (int)(validPosList.Count * objectDensity);

        Pos spawnPos;
        int i, randomIndex, portraitMargin;

        if (validPosList.Count < minArea) return;
        if (validPosList.Count < smallArea) type = portraitType.folliage;

        for (i = targetRubbleQuota; i > 0; i--)
        {
            if (rubblePosList.Count <= 0) break;
            randomIndex = rng.Next(0, rubblePosList.Count);
            spawnPos = rubblePosList[randomIndex];
            spawnRubble(spawnPos, type);
            rubblePosList.RemoveAt(randomIndex);
        }

        for (i = targetStructureQuota; i > 0; i--)
        {
            if (validPosList.Count <= 0) break;
            int attempt = validPosList.Count;
            do
            {
                randomIndex = rng.Next(0, validPosList.Count);
                spawnPos = validPosList[randomIndex];
                attempt--;
            } while (!isValidStructureSpawn(spawnPos, validPosList, structureRadius) && attempt > 0);
            if (isValidStructureSpawn(spawnPos, validPosList, structureRadius))
            {
                spawnStructure(spawnPos, type);
                validPosList.RemoveAt(randomIndex);
            }
        }

        for (i = targetObjectQuota; i > 0; i--)
        {
            if (validPosList.Count <= 0) break;
            randomIndex = rng.Next(0, validPosList.Count);
            spawnPos = validPosList[randomIndex];
            spawnObject(spawnPos, type);
            validPosList.RemoveAt(randomIndex);
        }

        portraitMargin = rng.Next(minPortraitMargin, maxPortraitMargin);
        updatePaintedList(position, radius + portraitMargin);
    }

    List<Pos> getListOfValidPositions(Pos startPos, Pos endPos)
    {
        List<Pos> tempPosList = new List<Pos>();
        Pos tempPos;
        int x, y;

        for (x = startPos.x; x < endPos.x; x++)
        {
            for (y = startPos.y; y < endPos.y; y++)
            {
                tempPos = new Pos(x, y);
                if (isValidPotraitSpawn(tempPos))
                {
                    tempPosList.Add(tempPos);
                }
            }
        }

        return tempPosList;
    }

    bool isValidPotraitSpawn(Pos position)
    {
        return mapManager.IsWalkable(position) && !mapManager.IsReserved(position) && !paintedList.Contains(position) && !mapManager.IsBridge(position);
    }

    bool isValidStructureSpawn(Pos position, List<Pos> referenceList, int radius)
    {
        Pos startPos = new Pos(position.x - radius, position.y - radius);
        Pos endPos = new Pos(position.x + radius, position.y + radius);
        Pos tempPos;
        int x, y;

        for (x = startPos.x; x < endPos.x; x++)
        {
            for (y = startPos.y; y < endPos.y; y++)
            {
                tempPos = new Pos(x, y);
                if (!referenceList.Contains(tempPos) && !paintedList.Contains(tempPos)) return false;
            }
        }
        return true;
    }

    portraitType getRandomPortraitType()
    {
        int randomIndex = rng.Next(1, sizeof(portraitType));
        return (portraitType)randomIndex;
    }

    void spawnStructure(Pos position, portraitType type)
    {
        int randomIndex;
        GameObject randomObject;

        switch (type)
        {
            case portraitType.orc:
                randomIndex = rng.Next(0, orcStructures.Length);
                randomObject = orcStructures[randomIndex];
                break;
            case portraitType.undead:
                randomIndex = rng.Next(0, undeadStructures.Length);
                randomObject = undeadStructures[randomIndex];
                break;
            default:
                randomIndex = rng.Next(0, folliageStructures.Length);
                randomObject = folliageStructures[randomIndex];
                break;
        }

        allEnvironmentObject.Add(mapManager.instantiate_environment(randomObject, position, true));

    }

    void spawnObject(Pos position, portraitType type)
    {
        int randomIndex;
        GameObject randomObject;

        switch (type)
        {
            case portraitType.orc:
                randomIndex = rng.Next(0, orcObjects.Length);
                randomObject = orcObjects[randomIndex];
                break;
            case portraitType.undead:
                randomIndex = rng.Next(0, undeadObjects.Length);
                randomObject = undeadObjects[randomIndex];
                break;
            default:
                randomIndex = rng.Next(0, folliageObjects.Length);
                randomObject = folliageObjects[randomIndex];
                break;
        }

        allEnvironmentObject.Add(mapManager.instantiate_environment(randomObject, position, true));

    }

    void spawnRubble(Pos position, portraitType type)
    {
        int randomIndex;
        GameObject randomObject;

        switch (type)
        {
            case portraitType.orc:
                randomIndex = rng.Next(0, orcRubble.Length);
                randomObject = orcRubble[randomIndex];
                break;
            case portraitType.undead:
                randomIndex = rng.Next(0, undeadRubble.Length);
                randomObject = undeadRubble[randomIndex];
                break;
            default:
                randomIndex = rng.Next(0, folliageRubble.Length);
                randomObject = folliageRubble[randomIndex];
                break;
        }

        allEnvironmentObject.Add(mapManager.instantiate_environment(randomObject, position, true));
    }

    void updatePaintedList(Pos position, int margin)
    {
        List<Pos> tempPosList = new List<Pos>();
        Pos startPos = new Pos(position.x - margin, position.y - margin);
        Pos endPos = new Pos(position.x + margin, position.y + margin);
        Pos tempPos;
        int x, y;

        for (x = startPos.x; x < endPos.x; x++)
        {
            for (y = startPos.y; y < endPos.y; y++)
            {
                tempPos = new Pos(x, y);
                paintedList.Add(tempPos);
            }
        }

    }

    #endregion

    #region Main methods

    public void spawnEnvironment()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Pos position = new Pos(x, y);
                if (isValidPotraitSpawn(position))
                {
                    portraitType type = getRandomPortraitType();
                    spawnPortrait(position, type);
                }
            }
        }
    }

    public void clearEnvironment()
    {
        for (int i = 0; i < allEnvironmentObject.Count; i++)
        {
            Destroy(allEnvironmentObject[i]);
        }
        allEnvironmentObject.Clear();
    }

    void spawnPortals()
    {
        Region[] furthestRegions = mapManager.findFurthestRegions(1);

        Region startRegion = furthestRegions[0];
        Pos startRegionPosition = startRegion.startpos;
        int startRegionMargin = (int)(System.Math.Sqrt(startRegion.count) / 2);

        Region endRegion = furthestRegions[1];
        Pos endRegionPosition = endRegion.startpos;
        int endRegionMargin = (int)(System.Math.Sqrt(endRegion.count) / 2);

        int randomX, randomY;
        Pos position;

        do
        {
            randomX = rng.Next(startRegionPosition.x - startRegionMargin, startRegionPosition.x + startRegionMargin);
            randomY = rng.Next(startRegionPosition.y - startRegionMargin, startRegionPosition.y + startRegionMargin);
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawnTraversableObject(startPortal, position);
        setSpawnAreaAroundPosition(position, spawnAreaRadius);
        reserveAreaAroundPoistion(position, reserveAreaRadius);

        do
        {
            randomX = rng.Next(endRegionPosition.x - endRegionMargin, endRegionPosition.x + endRegionMargin);
            randomY = rng.Next(endRegionPosition.y - endRegionMargin, endRegionPosition.y + endRegionMargin);
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawnTraversableObject(exitPortal, position);
    }

    void reserveAreaAroundPoistion(Pos position, int radius)
    {
        int diameter = radius * 2;
        for (int x = position.x - radius; x < position.x + diameter; x++)
        {
            for (int y = position.y - radius; y < position.y + diameter; y++)
            {
                Pos currentPos = new Pos(x, y);
                if (mapManager.IsTraversable(currentPos))
                {
                    mapManager.setTileReserved(currentPos);
                }
            }
        }
    }

    void setSpawnAreaAroundPosition(Pos position, int radius)
    {
        int diameter = radius * 2;
        for (int x = position.x - radius; x < position.x + diameter; x++)
        {
            for (int y = position.y - radius; y < position.y + diameter; y++)
            {
                Pos currentPos = new Pos(x, y);
                if (mapManager.IsTraversable(currentPos))
                {
                    mapManager.setTileSpawn(currentPos);
                }
            }
        }
    }

    #endregion

    #region Depreciated methods

    void spawnRandomEnvironmentObject(Pos position)
    {
        float random;

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * traversableFolliagDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableFolliage, position);
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * particleDensity)
        {
            spawnRandomTraversableObject(environmentType.particle, position);
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * traversableObjectDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableObject, position);
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * traversableStructureDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableStructure, position);
            return;
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * nonTraversableFolliageDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableFolliage, position);
            return;
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * nonTraversableObjectDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableObject, position);
            return;
        }

        random = rng.Next(0, environmentDensity);
        if (random < environmentDensity * nonTraversableStructureDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableStructure, position);
            return;
        }
    }

    void spawnRandomTraversableObject(environmentType type, Pos position)
    {
        mapManager.instantiate_environment(getRandomEnvironmentObject(type), position, true);
    }

    void spawnRandomNonTraversableObject(environmentType type, Pos position)
    {
        mapManager.instantiate_environment(getRandomEnvironmentObject(type), position, false);
    }

    void spawnTraversableObject(GameObject type, Pos position)
    {
        mapManager.instantiate_environment(type, position, true);
    }

    void spawnNonTraversableObject(GameObject type, Pos position)
    {
		mapManager.instantiate_environment(type, position, false);
    }

    GameObject getRandomEnvironmentObject(environmentType type)
    {
        int index;
        switch (type)
        {
            case environmentType.traversableFolliage:
                index = rng.Next(0, traversableFolliageObject.Length);
                return traversableFolliageObject[index];
            case environmentType.nonTraversableFolliage:
                index = rng.Next(0, nonTraversableFolliageObject.Length);
                return nonTraversableFolliageObject[index];
            case environmentType.traversableObject:
                index = rng.Next(0, traversableObjectObject.Length);
                return traversableObjectObject[index];
            case environmentType.nonTraversableObject:
                index = rng.Next(0, nonTraversableObjectObject.Length);
                return nonTraversableObjectObject[index];
            case environmentType.traversableStructure:
                index = rng.Next(0, traversableStructureObject.Length);
                return traversableStructureObject[index];
            case environmentType.nonTraversableStructure:
                index = rng.Next(0, nonTraversableStructureObject.Length);
                return nonTraversableStructureObject[index];
            case environmentType.particle:
                index = rng.Next(0, particleObject.Length);
                return particleObject[index];
            default:
                Debug.Log("getRandomEnvironmentObject() error.");
                return null;
        }
    }

    #endregion

}