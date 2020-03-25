using MapUtils;
using RegionUtils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
	
    private int width;
    private int height;
    private float cell_size;
    private Vector3 regionSize;
    private float radius;
    private MapManager mapManager;
    private MapGenerator mapGenerator;
    private System.Random rng;
    // A list of all accepted Spawn Zones
    private List<SpawnZone> spawnZones;
    List<Region> regions;
    public bool showEnemySpawnZones;
    private MapConfiguration mapConfiguration;
	
	public GameObject enemyPrefab;

    [Header("Enemy Spawn Zone Settings")]
    [Tooltip("Increase slightly to increase distance between zones.")]
    public float distanceBetweenZones = 1f;
    [Tooltip("Smallest size a zone can be.")]
    public float lowerRadius = 2f;
    [Tooltip("Largest size a zone can be.")]
    public float upperRadius = 6f;
    [Tooltip("Smallest number of spawnable tiles a zone can contain.")]
    public int minimumNumberOfTilesInSpawnZone = 3;
    [Tooltip("Largest number of spawnable tiles a zone can contain.")]
    public int maximumNumberOfTilesInSpawnZone = 30;

    public int maxNumberOfSpawnZones = 20;

    // Initializes map data
    public void Init(MapManager mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        mapGenerator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        width = config.width;
        height = config.height;
        regionSize = new Vector2(width, height);
        cell_size = config.cell_size;
        radius = cell_size * Mathf.Sqrt(2);
        this.mapManager = mapManager;
        mapConfiguration = config;
        rng = config.GetRNG();

        spawnZones = new List<SpawnZone>();
		SpawnEnemies();
    }

    // Call this to spawn enemies on the Map Manager
    public void SpawnEnemies() {
        GenerateSpawnZones();
        TrimSpawnZones();

        EnemyGroupManager enemyGroupManager = new EnemyGroupManager(spawnZones);
        List<EnemyToSpawn> enemies = enemyGroupManager.GetEnemiesToSpawn();

		foreach (EnemyToSpawn enemy in enemies) {
			mapManager.instantiate(enemyPrefab, enemy.gridPosition, enemy.stats);
		}
    }

    // Creates a list of Spawn Zones of varrying sizes in the map
    private void GenerateSpawnZones() {

        regions = mapGenerator.getRegions();
        List<Pos> regionTiles = new List<Pos>();
        int[,] grid = new int[width, height];
        int distanceBetweenZones = 0;
        int spawnZoneRadius = 0;
        int radiusAndDistance = 0;
        int numOfSpawnZonesInRegion = 0;

        // Iterates through each region
        foreach (Region region in regions) {
            numOfSpawnZonesInRegion = 0;
            regionTiles = region.GetRegionTiles();

            // Checks through each tile in the region
            foreach (Pos tile in regionTiles) {
                distanceBetweenZones = rng.Next(Mathf.Max(1, Mathf.RoundToInt(this.distanceBetweenZones / 2f)), Mathf.RoundToInt(this.distanceBetweenZones));
                spawnZoneRadius = rng.Next(Mathf.RoundToInt(lowerRadius), Mathf.RoundToInt(upperRadius + 1));
                radiusAndDistance = distanceBetweenZones + spawnZoneRadius;
                bool valid = true;

                while (!IsValid(tile, spawnZones, grid, spawnZoneRadius)) {
                    // If smallest radius doesn't work, then it isn't a valid spawn point
                    if (spawnZoneRadius == lowerRadius) {
                        valid = false;
                        break;
                    } else {
                        // Try a smaller radius
                        spawnZoneRadius--;
                    }
                }

                if (valid) {
                    SpawnZone spawnZone = CreateSpawnZone(tile, spawnZoneRadius);
                    // Checks if the number of zone tiles is acceptable
                    if (spawnZone.GetNumberOfUnpopulatedTilesInZone() >= minimumNumberOfTilesInSpawnZone
                        && spawnZone.GetNumberOfUnpopulatedTilesInZone() <= maximumNumberOfTilesInSpawnZone) {

                        // Spawn Zone is accepted and added to the list
                        spawnZones.Add(spawnZone);
                        grid[tile.x, tile.y] = spawnZones.Count;
                        numOfSpawnZonesInRegion++;
                    }
                }
            }

            region.numOfSpawnZones = numOfSpawnZonesInRegion;
        }
    }

    // Randomly removes spawn zones
    // Makes sure there is at least one spawn zone per region
    private void TrimSpawnZones() {
        if (spawnZones.Count > maxNumberOfSpawnZones && regions.Count > 0) {
            int numOfZonesToRemove = spawnZones.Count - maxNumberOfSpawnZones;
            int randomIndex = 0;
            bool oneSpawnZoneLeftInEveryRegion = false;
            bool zoneRemoved = false;
            List<int> exclude = new List<int>();

			int forceTermination = 10000;
			int iterationCount = 0;
            // Removes spawn zones until the max is in reach
            while (numOfZonesToRemove > 0) {
				if (iterationCount++ >= forceTermination) { Debug.Log("Forced loop to terminate"); break; }
				
                if (!oneSpawnZoneLeftInEveryRegion) {
                    zoneRemoved = false;

                    // Checks through all the regions
                    for (int i = 0; i < regions.Count; i++) {
                        // All the spawn zones needed have been removed
                        if (numOfZonesToRemove <= 0) {
                            break;
                        }

                        exclude.Clear();
                        for (int j = 0; j < spawnZones.Count; j++) {
                            // Get a random spawn zone
                            randomIndex = Utility.GetRandomIntWithExclusion(0, spawnZones.Count - 1, rng, exclude);
                            // Remove the spawn zone as long as the region has more than 1 spawn zone
                            if (regions[i].numOfSpawnZones > 1 && spawnZones[randomIndex].region == regions[i].ID) {
                                spawnZones.Remove(spawnZones[randomIndex]);
                                regions[i].numOfSpawnZones--;
                                numOfZonesToRemove--;
                                zoneRemoved = true;
                                break;
                            } else {
                                exclude.Add(randomIndex);
                            }
                        }
                    }

                    // Every region has 1 or less spawn zones
                    if (!zoneRemoved) {
                        oneSpawnZoneLeftInEveryRegion = true;
                    }
                } else {
                    // Begins removing spawn zones once every region only has 1 spawn zone
                    for (int i = 0; i < regions.Count; i++) {
                        // All the spawn zones needed have been removed
                        if (numOfZonesToRemove <= 0) {
                            break;
                        }

                        exclude.Clear();
                        for (int j = 0; j < spawnZones.Count; j++) {
                            // Get a random spawn zone
                            randomIndex = Utility.GetRandomIntWithExclusion(0, spawnZones.Count - 1, rng, exclude);
                            // Remove the spawn zone if able
                            if (regions[i].numOfSpawnZones > 0 && spawnZones[randomIndex].region == regions[i].ID) {
                                spawnZones.Remove(spawnZones[randomIndex]);
                                regions[i].numOfSpawnZones--;
                                numOfZonesToRemove--;
                                break;
                            } else {
                                exclude.Add(randomIndex);
                            }
                        }
                    }
                }
            }
        }
    }

    // Checks if the center of the Spawn Zone (candidate) will create a valid Spawn Zone
    // Checks if the center is traversable, and if other Spawn Zones fall within this Spawn Zone
    bool IsValid(Pos candidate, List<SpawnZone> points, int[,] grid, float spawnZoneRadius) {
        // Check if center is traversable
        if (!mapManager.IsTraversable(new Pos((int)candidate.x, (int)candidate.y))) {
            return false;
        }

        // Check if the surrounding cells are within the radius of another Spawn Zone already created
        if (candidate.x >=0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y) {
            int cellX = candidate.x;
            int cellY = candidate.y;
            int numOfCellsToScan = Mathf.CeilToInt(upperRadius);

            // Determines number of cells to search around the center
            int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
            int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
            int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
            int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x, y] - 1;
                    // non -1 means there is a Spawn Zone
                    if (pointIndex != -1) {
                        float dst = (new Vector3(candidate.x, candidate.y) - new Vector3(points[pointIndex].GetPosition().x, points[pointIndex].GetPosition().y)).magnitude;
                        if (dst <= (spawnZoneRadius + points[pointIndex].GetRadius())) {
                            // Candidate too close to another Spawn Zone
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

    // Creates the Spawn Zone and populates its traversable zone tile list
    // by checking all the tiles within its radius
    SpawnZone CreateSpawnZone(Pos candidate, float spawnZoneRadius) {
        SpawnZone spawnZone = new SpawnZone(candidate, spawnZoneRadius, mapManager.GetTileRegion(candidate));
        List<Pos> zoneTiles = new List<Pos>();

        int cellX = candidate.x;
        int cellY = candidate.y;
        int numOfCellsToScan = Mathf.CeilToInt(spawnZoneRadius);

        int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
        int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
        int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
        int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int y = searchStartY; y <= searchEndY; y++) {
                Pos tile = new Pos(x, y);
                if (mapManager.IsTraversable(tile) && !mapManager.IsOccupied(tile)) {
                    int a = cellX - x;
                    int b = cellY - y;
                    if (Mathf.Sqrt(a*a + b*b) <= spawnZoneRadius) {
                        zoneTiles.Add(tile);
                    }
                }
            }
        }
        spawnZone.SetZoneTiles(zoneTiles);

        return spawnZone;
    }

    public List<SpawnZone> GetSpawnZones() {
        return spawnZones;
    }

    void OnDrawGizmos() {
        if (showEnemySpawnZones) {
            List<Color> gizColors = new List<Color> { Color.red, Color.yellow, Color.blue, Color.cyan, Color.green, Color.white, Color.grey };

            if (spawnZones.Count > 0) {
                for (int i = 0; i < spawnZones.Count; i++) {

                    if (spawnZones[i].IsPopulated()) {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(mapManager.grid_to_world(new Pos((int)spawnZones[i].GetPosition().x, (int)spawnZones[i].GetPosition().y)), spawnZones[i].GetRadius());
                        List<Pos> zoneTiles = spawnZones[i].GetUnpopulatedZoneTiles();
                        foreach (Pos tile in zoneTiles) {
                            Gizmos.color = gizColors[i % gizColors.Count];
                            Gizmos.DrawWireCube(mapManager.grid_to_world(new Pos((int)tile.x, (int)tile.y)), new Vector3(mapConfiguration.cell_size, 0, mapConfiguration.cell_size));
                        }
                    }
                }
            }
        }
    }
}
