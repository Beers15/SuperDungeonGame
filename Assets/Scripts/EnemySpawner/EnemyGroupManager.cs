using MapUtils;
using RegionUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

// Responsible for taking a List of enemy groups and spawn zones
// and figuring out where to spawn the enemies within the spawn zones
public class EnemyGroupManager
{
    [Header("Enemy Group Settings")]
    public int maxNumberOfEnemyGroups;
    public int minNumberOfEnemyGroups;

    private List<SpawnZone> spawnZones;
    private List<EnemyToSpawn> enemies;
    private MapGenerator mapGenerator;
    private System.Random rng;

    public EnemyGroupManager(List<SpawnZone> spawnZones) {
        this.spawnZones = spawnZones;
        enemies = new List<EnemyToSpawn>();

        mapGenerator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        rng = config.GetRNG();

        // Sort based on spawn zone size
        QuickSortSpawnZones(spawnZones, 0, spawnZones.Count - 1);
    }

    // Takes the group of enemies and creates EnemyToSpawn objects with the location
    // within the spawn zone of where to spawn them
    private void PopulateSpawnZone(EnemyGroup group, SpawnZone spawnZone) {
        List<GameAgentStats> enemyStats = group.GetEnemiesStatsForSpawn();
        List<Pos> zoneTiles = spawnZone.GetUnpopulatedZoneTiles();
        List<Pos> populatedZoneTiles = new List<Pos>();

        // Random distribution in zone
        List<int> exclusion = new List<int>();

        foreach(GameAgentStats stats in enemyStats) {
            int randomIndex = Utility.GetRandomIntWithExclusion(0, zoneTiles.Count - 1, rng, exclusion);
            Pos enemyPos = new Pos((int)zoneTiles[randomIndex].x, (int)zoneTiles[randomIndex].y);
            populatedZoneTiles.Add(zoneTiles[randomIndex]);
            EnemyToSpawn enemy = new EnemyToSpawn(enemyPos, stats);

            exclusion.Add(randomIndex);
            enemies.Add(enemy);
        }
        spawnZone.PopulateTiles(populatedZoneTiles);
    }

    // Call this to get a list of EnemiesToSpawn ready to be initialized
    public List<EnemyToSpawn> GetEnemiesToSpawn() {
        // Trivial, Avergae, Difficult, Impossible
        int[] enemyGroupsByDifficulty = DistributeGroupsIntoRegions();
        int currentRegion = -1;
        int spawnZoneIndex = 0;

        if (spawnZones.Count > 0) {
            currentRegion = spawnZones[0].region;
        }

        for (int i = 0; i < enemyGroupsByDifficulty.Length; i++) {
            for (int j = 0; j < enemyGroupsByDifficulty[i]; j++) {
                // Stop if there are no more spawn zones
                if (spawnZoneIndex >= spawnZones.Count) {
                    break;
                }

                // Populate spawn zones in region by difficulty
                while (spawnZoneIndex < spawnZones.Count && currentRegion == spawnZones[spawnZoneIndex].region) {
                    switch (i) {
                        case EnemyGroupDifficulty.Trivial:
                            // Adjust the group size until it fits within the spawn zone
                            if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.LargeUpperBound) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Trivial, 1, rng), spawnZones[spawnZoneIndex]);
                            } else if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.MediumUpperBound) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Trivial, 1, rng, -1, EnemyGroupSize.Medium), spawnZones[spawnZoneIndex]);
                            } else if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.Small) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Trivial, 1, rng, -1, EnemyGroupSize.Small), spawnZones[spawnZoneIndex]);
                            }
                            break;
                        case EnemyGroupDifficulty.Average:
                            // Adjust the group size until it fits within the spawn zone
                            if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.MediumUpperBound) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Average, 1, rng), spawnZones[spawnZoneIndex]);
                            } else if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.Small) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Average, 1, rng, -1, EnemyGroupSize.Small), spawnZones[spawnZoneIndex]);
                            }
                            break;
                        case EnemyGroupDifficulty.Difficult:
                            // Adjust the group size until it fits within the spawn zone
                            if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.MediumUpperBound) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Difficult, 1, rng), spawnZones[spawnZoneIndex]);
                            } else if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.Small) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Difficult, 1, rng, -1, EnemyGroupSize.Small), spawnZones[spawnZoneIndex]);
                            }
                            break;
                        case EnemyGroupDifficulty.Impossible:
                            if (spawnZones[spawnZoneIndex].GetNumberOfUnpopulatedTilesInZone() >= EnemyGroupSize.Small) {
                                PopulateSpawnZone(EnemyGroupTemplate.GetEnemyGroupGivenDifficulty(EnemyGroupDifficulty.Impossible, 1, rng), spawnZones[spawnZoneIndex]);
                            }
                            break;
                    }
                    spawnZoneIndex++;
                }

                if (spawnZoneIndex >= spawnZones.Count) {
                    break;
                } else if (currentRegion != spawnZones[spawnZoneIndex].region) {
                    currentRegion = spawnZones[spawnZoneIndex].region;
                }
            }
        }

        return enemies;
    }

    /* 
     * This takes all the regions and distributes them into a bell curve over the different difficulties
     * This method decides how many regions of each difficulty to make
     * 
     *  ex. 12 regions into 4 different difficulties
     *  
     * 
     *                        1
     *              1         1           
     *              1         1            
     *              1         1           1
     *     1        1         1           1
     * [Trivial][Average][Difficult][Impossible]
     * 
     * DistributedGroupsIntoRegions[0] = 1
     * DistributedGroupsIntoRegions[1] = 4
     * DistributedGroupsIntoRegions[2] = 5
     * DistributedGroupsIntoRegions[3] = 2
    */
    private int[] DistributeGroupsIntoRegions() {
        int numOfRegions = mapGenerator.getRegions().Count;
        int[] countOfGroupsInRegions = new int[EnemyGroupDifficulty.count];
        int mean = (0 + numOfRegions) / 2;

        // Guassian distribution to place enemies
        for (int i = 0; i < numOfRegions; i++) {
            // Put one group of enemies in spawn zones
            if (countOfGroupsInRegions[i % countOfGroupsInRegions.Length] == 0) {
                countOfGroupsInRegions[i % countOfGroupsInRegions.Length]++;
            } else {
                // Guassian distribution
                int index = Utility.NextGaussian(mean, 1, 0, numOfRegions, rng);
                if (index < 0) {
                    index = 0;
                } else if (index > countOfGroupsInRegions.Length - 1) {
                    index = countOfGroupsInRegions.Length - 1;
                }
                countOfGroupsInRegions[index]++;
            }
        }

        return countOfGroupsInRegions;
    }

    // Sorts the Spawn Zones in descending order based on the size of the region size
    private void QuickSortSpawnZones(List<SpawnZone> spawnZones, int left, int right) {
        if (left < right) {
            int pivot = PartitionSpawnZones(spawnZones, left, right);

            if (pivot > 1) {
                QuickSortSpawnZones(spawnZones, left, pivot - 1);
            }
            if (pivot + 1 < right) {
                QuickSortSpawnZones(spawnZones, pivot + 1, right);
            }
        }
    }

    private int PartitionSpawnZones(List<SpawnZone> spawnZones, int left, int right) {
        SpawnZone pivot = spawnZones[left];
        while (true) {
            while (mapGenerator.GetRegionSize(spawnZones[left].region) > mapGenerator.GetRegionSize(pivot.region)) {
                left++;
            }

            while (mapGenerator.GetRegionSize(spawnZones[right].region) < mapGenerator.GetRegionSize(pivot.region)) {
                right--;
            }

            if (left < right) {
                SpawnZone temp = spawnZones[left];
                spawnZones[left] = spawnZones[right];
                spawnZones[right] = temp;

                if (mapGenerator.GetRegionSize(spawnZones[left].region) == mapGenerator.GetRegionSize(spawnZones[right].region)) {
                    left++;
                }
            } else {
                return right;
            }
        }
    }
}
