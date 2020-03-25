using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Melee - group of knights
// BalancedAllUnits - group of knights, hunters, mages, and healers
// BalancedSingleRange - group of knights, (hunters or mages), and healers
// MeleeAndSingleRange - group of knights and (hunters or mages)
// MeleeAndRange - group of knights, hunters, and mages
// MeleeAndHealers - group of knights and healers
// RangeAndHealers - group of (hunters or mages) and healers
// LessKnightsAndMoreMages - group of few knights and more mages
// MoreKnightsAndLessMages - group of more knights and few mages
// LessKnightsAndMoreHunters - group of few knights and more hunters
// MoreKnightsAndLessHunters - group of more knights and few hunters
// MixedRange - group of hunters and mages
// MixedRangeAndHealers - group of hunters, mages, and healers
public static class EnemyGroupType {
    public const int Melee = 1;
    public const int BalancedAllUnits = 2;
    public const int BalancedSingleRange = 3;
    public const int MeleeAndSingleRange = 4;
    public const int MeleeAndRange = 5;
    public const int MeleeAndHealers = 6;
    public const int RangeAndHealers = 7;
    public const int LessKnightsAndMoreMages = 8;
    public const int MoreKnightsAndLessMages = 9;
    public const int LessKnightsAndMoreHunters = 10;
    public const int MoreKnightsAndLessHunters = 11;
    public const int MixedRange = 12;
    public const int MixedRangeAndHealers = 13;
};

// Determines the level of the enemies
public static class EnemyGroupDifficulty {
    public const int Trivial = 0;
    public const int Average = 1;
    public const int Difficult = 2;
    public const int Impossible = 3;

    public const int count = 4;
};

public static class EnemyGroupSize {
    public const int Small = 3;
    public const int MediumLowerBound = 4;
    public const int Medium = 5;
    public const int MediumUpperBound = 6;
    public const int Large = 7;
    public const int LargeUpperBound = 8;
    public const int XLarge = 9;
};

public static class EnemyGroupTemplate {
    public static EnemyGroup GetEnemyGroupGivenDifficulty(int difficulty, int level, System.Random rng, int race = -1, int enemyGroupSize = -1) {
        if (race == -1) {
            race = rng.Next(CharacterRaceOptions.Orc, CharacterRaceOptions.Skeleton + 1);
        }

        switch (difficulty) {
            case EnemyGroupDifficulty.Trivial:
                if (enemyGroupSize < 0) {
                    return GetEnemyGroup(rng.Next(EnemyGroupType.Melee, EnemyGroupType.MixedRangeAndHealers + 1), difficulty, EnemyGroupSize.Large, race, level);
                }
                break;
            case EnemyGroupDifficulty.Average:
                if (enemyGroupSize < 0) {
                    return GetEnemyGroup(rng.Next(EnemyGroupType.Melee, EnemyGroupType.MixedRangeAndHealers + 1), difficulty, EnemyGroupSize.Medium, race, level);
                }
                break;
            case EnemyGroupDifficulty.Difficult:
                if (enemyGroupSize < 0) {
                    return GetEnemyGroup(rng.Next(EnemyGroupType.Melee, EnemyGroupType.MixedRangeAndHealers + 1), difficulty, EnemyGroupSize.Medium, race, level);
                }
                break;
            case EnemyGroupDifficulty.Impossible:
                if (enemyGroupSize < 0) {
                    return GetEnemyGroup(rng.Next(EnemyGroupType.Melee, EnemyGroupType.MixedRangeAndHealers + 1), difficulty, EnemyGroupSize.Small, race, level);
                }
                break;
        }

        // else return group of specified size
        return GetEnemyGroup(rng.Next(EnemyGroupType.Melee, EnemyGroupType.MixedRangeAndHealers + 1), difficulty, enemyGroupSize, race, level);
    }

    public static EnemyGroup GetEnemyGroup(int groupType, int difficulty, int enemyGroupSize, int race, int level) {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        System.Random rng = config.GetRNG();
        List<EnemyGroupDescription> enemyGroupDescriptions = new List<EnemyGroupDescription>();
        List<int> enemyQuantities = new List<int>();

        switch (groupType) {
            // Melee - group of knights
            case EnemyGroupType.Melee:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.Melee, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)), 
                                                                     enemyQuantities[0]));
                break;
            // BalancedAllUnits - group of knights, hunters, mages, and healers
            case EnemyGroupType.BalancedAllUnits:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.BalancedAllUnits, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                
                if (enemyGroupSize > EnemyGroupSize.Small) {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[2]));
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[3]));
                } else if (enemyGroupSize == EnemyGroupSize.Small) {
                    if (rng.Next(0, 1 + 1) == 1) {
                        enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                    } else {
                        enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                             enemyQuantities[1]));
                    }
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[2]));
                }
                break;
            // BalancedSingleRange - group of knights, (hunters or mages), and healers
            case EnemyGroupType.BalancedSingleRange:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.BalancedSingleRange, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                if (rng.Next(0, 1 + 1) == 1) {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                 enemyQuantities[1]));
                } else {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[1]));
                }
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[2]));
                break;
            // MeleeAndSingleRange - group of knights and (hunters or mages)
            case EnemyGroupType.MeleeAndSingleRange:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MeleeAndSingleRange, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                if (rng.Next(0, 1 + 1) == 1) {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                 enemyQuantities[1]));
                } else {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[1]));
                }
                break;
            // MeleeAndRange - group of knights, hunters, and mages
            case EnemyGroupType.MeleeAndRange:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MeleeAndRange, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[2]));
                break;
            // MeleeAndHealers - group of knights and healers
            case EnemyGroupType.MeleeAndHealers:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MeleeAndHealers, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[1]));
                break;
            // RangeAndHealers - group of (hunters or mages) and healers
            case EnemyGroupType.RangeAndHealers:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.RangeAndHealers, rng, enemyGroupSize);
                if (rng.Next(0, 1 + 1) == 1) {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                        enemyQuantities[0]));
                } else {
                    enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                         enemyQuantities[0]));
                }
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // LessKnightsAndMoreMages - group of few knights and more mages
            case EnemyGroupType.LessKnightsAndMoreMages:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.LessKnightsAndMoreMages, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // MoreKnightsAndLessMages - group of more knights and few mages
            case EnemyGroupType.MoreKnightsAndLessMages:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MoreKnightsAndLessMages, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // LessKnightsAndMoreHunters - group of few knights and more hunters
            case EnemyGroupType.LessKnightsAndMoreHunters:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.LessKnightsAndMoreHunters, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // MoreKnightsAndLessHunters - group of more knights and few hunters
            case EnemyGroupType.MoreKnightsAndLessHunters:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MoreKnightsAndLessHunters, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // MixedRange - group of hunters and mages
            case EnemyGroupType.MixedRange:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MixedRange, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                break;
            // MixedRangeAndHealers - group of hunters, mages, and healers
            case EnemyGroupType.MixedRangeAndHealers:
                enemyQuantities = GetEnemyQuantities(EnemyGroupType.MixedRangeAndHealers, rng, enemyGroupSize);
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Hunter, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[0]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Mage, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[1]));
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Healer, GetLevel(difficulty, rng, level)),
                                                                     enemyQuantities[2]));
                break;
        }

        return new EnemyGroup(enemyGroupDescriptions);
    }

    private static List<int> GetEnemyQuantities(int groupType, System.Random rng, int enemyGroupSize) {
        List<int> enemyQuantities = new List<int>();
        int unitsRemaining = enemyGroupSize;
        List<int> exclusion = new List<int>();

        switch (groupType) {
            // Melee - group of knights
            case EnemyGroupType.Melee:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        enemyQuantities.Add(EnemyGroupSize.Small);
                        break;
                    case EnemyGroupSize.Medium:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1));
                        break;
                    case EnemyGroupSize.Large:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1));
                        break;
                    case EnemyGroupSize.XLarge:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1));
                        break;
                }
                break;
            // BalancedAllUnits - group of knights, hunters, mages, and healers
            case EnemyGroupType.BalancedAllUnits:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, Hunter or Mage, then Healer
                        while (unitsRemaining > 0) {
                            enemyQuantities.Add(1);
                            unitsRemaining--;
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // BalancedSingleRange - group of knights, (hunters or mages), and healers
            case EnemyGroupType.BalancedSingleRange:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, Hunter or Mage, then Healer
                        while (unitsRemaining > 0) {
                            enemyQuantities.Add(1);
                            unitsRemaining--;
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MeleeAndSingleRange - group of knights and (hunters or mages)
            case EnemyGroupType.MeleeAndSingleRange:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Hunter or Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Hunter or Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Hunter or Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Hunter or Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MeleeAndRange - group of knights, hunters, and mages
            case EnemyGroupType.MeleeAndRange:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, Hunter, then Mage
                        while (unitsRemaining > 0) {
                            enemyQuantities.Add(1);
                            unitsRemaining--;
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MeleeAndHealers - group of knights and healers
            case EnemyGroupType.MeleeAndHealers:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // RangeAndHealers - group of (hunters or mages) and healers
            case EnemyGroupType.RangeAndHealers:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Hunter or Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // LessKnightsAndMoreMages - group of few knights and more mages
            case EnemyGroupType.LessKnightsAndMoreMages:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MoreKnightsAndLessMages - group of more knights and few mages
            case EnemyGroupType.MoreKnightsAndLessMages:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Mages
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Mages
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Mages
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Mages
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // LessKnightsAndMoreHunters - group of few knights and more hunters
            case EnemyGroupType.LessKnightsAndMoreHunters:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Knights
                            if (randomIndex == 0 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MoreKnightsAndLessHunters - group of more knights and few hunters
            case EnemyGroupType.MoreKnightsAndLessHunters:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Hunters
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Hunters
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Hunters
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Knight, then Hunter
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less Hunters
                            if (randomIndex == 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MixedRange - group of hunters and mages
            case EnemyGroupType.MixedRange:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Hunter, then Mage
                        enemyQuantities = new List<int> { 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            if (rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
            // MixedRangeAndHealers - group of hunters, mages, and healers
            case EnemyGroupType.MixedRangeAndHealers:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Hunter, Mage, then Healer
                        while (unitsRemaining > 0) {
                            enemyQuantities.Add(1);
                            unitsRemaining--;
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.XLarge:
                        // Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            unitsRemaining--;

                            // Less healers
                            if (randomIndex == enemyQuantities.Count - 1 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                }
                break;
        }

        return enemyQuantities;
    }

    private static int GetLevel(int difficulty, System.Random rng, int level) {
        switch (difficulty) {
            case EnemyGroupDifficulty.Trivial:
                if (level > 3) {
                    return rng.Next(level - 2, level + 1);
                } else if (level > 2) {
                    return rng.Next(level - 1, level + 1);
                }
                break;
            case EnemyGroupDifficulty.Average:
                if (level > 2) {
                    return rng.Next(level - 1, level + 1 + 1);
                } else {
                    return rng.Next(level, level + 1 + 1);
                }
            case EnemyGroupDifficulty.Difficult:
                return rng.Next(level + 1, level + 2 + 1);
            case EnemyGroupDifficulty.Impossible:
                return rng.Next(level + 2, level + 3 + 1);
        }
        return level;
    }
}