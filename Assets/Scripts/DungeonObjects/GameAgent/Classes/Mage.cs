using UnityEngine;

public class Mage : CharacterClass
{
    public Mage() {
        baseStats = new GameAgentStats(23, 70, 7, 6, true);
        rng = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().GetRNG();
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        int min = 2;
        int max = 4;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        int min = 1;
        int max = 2;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public override int GetRangeStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override int GetSpeedStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override Attack[] GetAvailableActs() {
        return new Attack[] { Attack.Get["WeakMelee"], Attack.Get["Fire"], Attack.Get["Fire Storm"] };
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
        weapon = CharacterClassOptions.Staff;
    }
}
