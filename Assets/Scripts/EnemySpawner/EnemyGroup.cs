using System.Collections.Generic;
using UnityEngine;

// Group of enemies to be spawned in a spawn zone
public class EnemyGroup
{
    [Header("Random Number of Enemies in Groups")]
    [Tooltip("If true, select minumum value and maximum value for number of enemies to be spawned in the group.")]
    public bool randomRangeNumberOfEnemies;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;

    [Header("Customize Group based on Power")]
    public bool powerBalance = false;

    private List<EnemyGroupDescription> typesOfEnemies;
    private List<GameAgentStats> enemies;

    public int count;

    public EnemyGroup(List<EnemyGroupDescription> typesOfEnemies, bool randomRangeNumberOfEnemies = false,
                        int minNumberOfEnemies = -1, int maxNumberOfEnemies = -1, bool powerBalance = false) {

        this.typesOfEnemies = typesOfEnemies;
        this.randomRangeNumberOfEnemies = randomRangeNumberOfEnemies;
        this.minNumberOfEnemies = minNumberOfEnemies;
        this.maxNumberOfEnemies = maxNumberOfEnemies;
        this.powerBalance = powerBalance;

        enemies = new List<GameAgentStats>();

        CreateEnemyStatsInGroup();
    }

    // Creates a list of enemy stats that will be used for placing in spawn zones
    void CreateEnemyStatsInGroup() {
        foreach (EnemyGroupDescription enemy in typesOfEnemies) {
            for (int i=0; i<enemy.quantityOfEnemyInGroup; i++) {
                GameAgentStats stats = new GameAgentStats(enemy.stats.characterRace, enemy.stats.characterClassOption, enemy.stats.level, CharacterClassOptions.RandomClassWeapon);
                enemies.Add(stats);
            }
        }

        count = enemies.Count;
    }

    public List<GameAgentStats> GetEnemiesStatsForSpawn() {
        return enemies;
    }
}
