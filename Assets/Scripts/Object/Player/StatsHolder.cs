using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatsHolder
{
    private const int MAX_EXP = 100;
    private const int STARTING_STAT_POINTS = 2;
    private const int STAT_POINTS_PER_LEVEL = 1;
    public const int HP_PER_VITALITY_POINT = 20;

    #region Data
    private int level;
    private int experience;
    private int statPoints;
    private Dictionary<StatName, int> stats;
    #endregion Data

    #region Properties
    public int Level { get => level; }
    public int Experience { get => experience; }
    public int StatPoints { get => statPoints; }

    public Dictionary<StatName, int> Stats { get => stats; }
    public int this[StatName key]
    {
        get => stats[key];
    }
    #endregion Properties


    #region Methods
    public StatsHolder(Species species)
    {
        level = 1;
        experience = 0;
        statPoints = STARTING_STAT_POINTS;
        stats = new Dictionary<StatName, int>(species.BaseStats);
    }
    public StatsHolder(Species species, int level)
    {
        this.level = level;
        experience = 0;
        statPoints = level == 1 ? 0 : level * STAT_POINTS_PER_LEVEL;
        stats = new Dictionary<StatName, int>(species.BaseStats);

        int nStats = stats.Count;
        while(statPoints > 0)
        {
            StatName statToIncrease = (StatName)Data.agentRng.Next(nStats);
            IncreaseStat(statToIncrease);
        }
    }

    public bool IncreaseStat(StatName statToIncrease)
    {
        if (statPoints > 0)
        {
            stats[statToIncrease]++;
            statPoints--;
            return true;
        }

        return false;
    }

    public void AddExperience(int addedExperience)
    {
        experience += addedExperience;

        if (experience >= MAX_EXP)
        {
            experience -= MAX_EXP;
            LevelUp();
        }
    }
    private void LevelUp()
    {
        level += 1;
        statPoints += STAT_POINTS_PER_LEVEL;

        Logger.SendMessageToUI($"You level up to level {level} and gain {STAT_POINTS_PER_LEVEL} point to increase your stats. Your current exp is {experience}.");
    }

    public string GetDescription()
    {
        string description = string.Empty;

        int nStats = stats.Count;
        int i = 1;
        foreach (StatName statName in stats.Keys)
        {
            description += $"{statName}: {stats[statName]}";
            if (i < nStats) description += ", ";
            i++;
        }

        return description;
    }
    #endregion Methods
}


public enum StatName : byte { Strength, Agility, Vitality }