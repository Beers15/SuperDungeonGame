using UnityEngine;

public class Knight : CharacterClass {

    public Knight() {
        baseStats = new GameAgentStats(20, 100, 2, 7, true);
        rng = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().GetRNG();
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        int min = 1;
        int max = 3;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public override Attack[] GetAvailableActs() {
        return new Attack[] { Attack.Get["Melee"], Attack.Get["StrongMelee"], Attack.Get["Berserk"] };
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        int min = 3;
        int max = 4;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public override int GetRangeStatIncreaseFromLevelUp(int level = -1) {
        return 0;
    }

    public override int GetSpeedStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override void HandleAct(GameAgentAction action) {
    }

    public override void LevelUp() {
    }

    public override void SetWeapon(int weapon) {
        if (weapon == CharacterClassOptions.RandomClassWeapon) {
            GenerateRandomClassWeapon();
        } else {
            this.weapon = weapon;
        }
    }

    protected override void GenerateRandomClassWeapon() {
        int[] knightWeapons = new int[] { CharacterClassOptions.Sword, CharacterClassOptions.Axe, CharacterClassOptions.Club, CharacterClassOptions.Unarmed };
        weapon = knightWeapons[Random.Range(0, knightWeapons.Length)];
    }
}
