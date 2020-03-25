using MapUtils;
using UnityEngine;

public class EnemyToSpawn
{
    public Pos gridPosition;
    public GameAgentStats stats;
    GameObject enemy;

    public EnemyToSpawn(Pos gridPosition, GameAgentStats stats) {
        this.gridPosition = gridPosition;
        this.stats = stats;
    }
}
