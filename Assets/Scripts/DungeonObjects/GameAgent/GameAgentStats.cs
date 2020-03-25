// Holds information regarding the GameAgents ingame stats
using System;
using UnityEngine;

public enum GameAgentState { Alive, Unconscious, Dead, Null };

public enum GameAgentStatusEffect { None, Taunted, Taunting };

public class GameAgentStats
{
    public int characterClassOption;
    public int characterRace;
    public GameAgentState currentState;
    public GameAgentStatusEffect currentStatus;
    public CharacterClass playerCharacterClass;
    // unit attack damage
    public float attack;
    // unit maxium health
    public float maxHealth;
    // unit current health
    public float currentHealth;
    // unit max mp
    public float maxMagicPoints = 40;
    // unit current mp
    public float currentMagicPoints = 40;
    // unit attack radius
    public float range;
    // unit move radius
    public float speed;
    // game agent level
    public int level = 1;
    // experience points
    public int xp;
    // stat granted through items
    public int defense = 0;
    // stat granted from taunting
    public int defenseFromTaunting = 0;
    // stat granted through items
    public int attackStatBoost = 0;

    private System.Random rng;

    // This is used to determine how much xp is awarded for death
    // the scale works by awarding the scaleFactor amount of xp for that level
    // ex. if it takes 1000xp to reach level, and the player kills a level 5 monster
    // the player will receive 1000xp * scaleFactor worth of xp
    private float scaleFactor = 0.25f;

    public GameAgentStats(int characterRace, int characterClass, float attack, float health, float range, float speed, int desiredLevel, int characterWeapon = CharacterClassOptions.RandomClassWeapon) {
        rng = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().GetRNG();

        this.characterRace = characterRace;
        characterClassOption = characterClass;
        SetGameAgentCharacterClass(characterWeapon);
        this.attack = attack;
        maxHealth = health;
        currentHealth = health;
        this.range = range;
        this.speed = speed;
        currentState = GameAgentState.Alive;
        currentStatus = GameAgentStatusEffect.None;

        LevelUpToDesiredLevel(desiredLevel);
    }

    // Only used for creating base stats
    public GameAgentStats(float attack, float health, float range, float speed, bool baseStats) {
        if (baseStats) {
            this.attack = attack;
            maxHealth = health;
            currentHealth = health;
            this.range = range;
            this.speed = speed;
        }
    }

    public GameAgentStats(int characterRace, int characterClass, int desiredLevel, int characterWeapon = CharacterClassOptions.RandomClassWeapon) {
        rng = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().GetRNG();

        this.characterRace = characterRace;
        characterClassOption = characterClass;
        SetGameAgentCharacterClass(characterWeapon);
        GetBaseCharacterClassStats();
        LevelUpToDesiredLevel(desiredLevel);
        currentState = GameAgentState.Alive;
        currentStatus = GameAgentStatusEffect.None;
    }

    private void GetBaseCharacterClassStats() {
        attack = playerCharacterClass.baseStats.attack;
        maxHealth = playerCharacterClass.baseStats.maxHealth;
        currentHealth = maxHealth;
        range = playerCharacterClass.baseStats.range;
        speed = playerCharacterClass.baseStats.speed;
    }

    public void LevelUp() {
        level++;
        attack += playerCharacterClass.GetAttackStatIncreaseFromLevelUp();
        range += playerCharacterClass.GetRangeStatIncreaseFromLevelUp();
        speed += playerCharacterClass.GetSpeedStatIncreaseFromLevelUp(level);
        int healthIncrease = playerCharacterClass.GetHealthStatIncreaseFromLevelUp();

        maxHealth += healthIncrease;
        if (currentHealth > 0) {
            currentHealth += healthIncrease;
        }
    }

    public void LevelUpToDesiredLevel(int desiredLevel) {
        while (level < desiredLevel) {
            LevelUp();
        }
    }

    public void GainXP(int xpGained) {
        xp += xpGained;
        CheckLevelProgression();
    }

    public void TakeDamage(int damage) {
        int damageDealt = damage - defense;
        if (damageDealt < 1) {
            damageDealt = 1;
        }
        currentHealth -= damageDealt;

        if (currentHealth <= 0) {
            currentHealth = 0;
            currentState = GameAgentState.Unconscious;
        }
    }

    public void SetTauntingStatus(bool active) {
        if (active) {
            if (currentStatus != GameAgentStatusEffect.Taunting) {
                defenseFromTaunting = Mathf.RoundToInt(attack * 0.33f);
            }
        } else {
            // remove defenseFromTaunting
        }
    }

    public void GetHealed(int amount) {
        currentHealth += amount;

        if (currentHealth >= maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public void GetMP(int amount) {
        currentMagicPoints += amount;

        if (currentMagicPoints >= maxMagicPoints) {
            currentMagicPoints = maxMagicPoints;
        }
    }

    public void UsePotion() {
        GetHealed(Mathf.RoundToInt(maxHealth * 0.33f));
        GetMP(Mathf.RoundToInt(maxMagicPoints * 0.33f));
    }

    public int DealDamage() {
        return Mathf.RoundToInt(attack + attackStatBoost);
    }

    public int GetHealAmount() {
        if (characterClassOption == CharacterClassOptions.Healer) {
            return Mathf.RoundToInt(UnityEngine.Random.Range(0.5f, 0.75f) * (attack + attackStatBoost));
        }
        return 0;
    }

    public int GetMultiHitCount() {
        int min = 2;
        int max = 4;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public int GetFireStormDamage() {
        return Mathf.RoundToInt(UnityEngine.Random.Range(0.55f, 0.7f) * (attack + attackStatBoost));
    }

    public int GetBerserkDamage() {
        return Mathf.RoundToInt(UnityEngine.Random.Range(0.45f, 0.75f) * (attack + attackStatBoost));
    }

    private void CheckLevelProgression() {
        // This formula is used for a linearly rising level gap
        float progressionTowardsLevel = (Mathf.Sqrt(100f * (2 * xp + 25f))+50f)/ 100f;

        // Level Up multiple times if needed
        if (progressionTowardsLevel >= level + 1) {
            while(progressionTowardsLevel > 1) {
                LevelUp();
                progressionTowardsLevel--;
            }
        }
    }
    
    public int RewardXPFromDeath() {
        return Mathf.RoundToInt((level * level + level) / 2 * 100 - (level * 100) * scaleFactor);
    }

    private void SetGameAgentCharacterClass(int characterWeapon) {
        switch (characterClassOption) {
            case CharacterClassOptions.Knight:
                playerCharacterClass = new Knight();
                break;
            case CharacterClassOptions.Hunter:
                playerCharacterClass = new Hunter();
                break;
            case CharacterClassOptions.Mage:
                playerCharacterClass = new Mage();
                break;
            case CharacterClassOptions.Healer:
                playerCharacterClass = new Healer();
                break;
        }

        playerCharacterClass.SetWeapon(characterWeapon);
    }
}
