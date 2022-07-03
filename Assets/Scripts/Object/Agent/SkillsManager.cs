using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsManager
{
    private const int MAX_NORMAL_SKILL_LEVEL = 5;
    private const int MAX_BEST_SKILL_LEVEL = 10;

    #region Data
    private Dictionary<SkillsEnum, Skill> skills;
    #endregion Data

    #region Properties
    public Dictionary<SkillsEnum, Skill> Skills { get => skills; }

    public Skill this[SkillsEnum key]
    {
        get => skills[key];
        set => skills[key] = value;
    }
    #endregion Properties


    #region Methods
    public SkillsManager(SkillsEnum bestSkill, System.Random rng)
    {
        skills = new Dictionary<SkillsEnum, Skill>();

        foreach(var element in System.Enum.GetValues(typeof(SkillsEnum)))
        {
            Skill skill;
            SkillsEnum skillName = (SkillsEnum)element;

            if (skillName == bestSkill) skill = new Skill(skillName.ToString(), rng.Next(0, MAX_BEST_SKILL_LEVEL + 1));
            else skill = new Skill(skillName.ToString(), rng.Next(0, MAX_NORMAL_SKILL_LEVEL + 1));

            skills.Add(skillName, skill);
        }
    }
    public SkillsManager(System.Random rng)
    {
        skills = new Dictionary<SkillsEnum, Skill>();
        int nSkills = System.Enum.GetValues(typeof(SkillsEnum)).Length;

        SkillsEnum bestSkill = (SkillsEnum)rng.Next(0, nSkills);

        for (int i = 0; i < nSkills; i++)
        {
            Skill skill;
            SkillsEnum skillName = (SkillsEnum)i;
            if (skillName == bestSkill) skill = new Skill(skillName.ToString(), rng.Next(0, MAX_BEST_SKILL_LEVEL + 1));
            else skill = new Skill(skillName.ToString(), rng.Next(0, MAX_NORMAL_SKILL_LEVEL + 1));

            skills.Add(skillName, skill);
        }
    }

    public Skill GetSkill(SkillsEnum key)
    {
        if (!skills.ContainsKey(key)) return null;
        return skills[key];
    }

    public void AddExperienceToSkill(SkillsEnum skill, float experience)
    {
        skills[skill].AddExperience(experience);
    }
    #endregion Methods
}



public class Skill
{
    private const int MAX_LEVEL = 20;
    private const int EXPERIENCE_MODIFIER = 1000;
    private static int[][] qualityDistribution =
    {
       new int[] { 90, 10,  0,  0,  0 }, // Level 1
       new int[] { 85, 15,  0,  0,  0 },
       new int[] { 70, 30,  0,  0,  0 },
       new int[] { 65, 35,  0,  0,  0 },
       new int[] { 45, 45, 10,  0,  0 }, // Level 5
       new int[] { 35, 50, 15,  0,  0 },
       new int[] { 25, 55, 20,  0,  0 },
       new int[] { 15, 60, 25,  0,  0 },
       new int[] { 10, 55, 35,  0,  0 },
       new int[] {  5, 50, 40,  5,  0 }, // Level 10
       new int[] {  0, 45, 45, 10,  0 },
       new int[] {  0, 35, 50, 15,  0 },
       new int[] {  0, 25, 55, 20,  0 },
       new int[] {  0, 15, 50, 35,  0 },
       new int[] {  0,  5, 50, 40,  5 }, // Level 15
       new int[] {  0,  0, 45, 45, 10 },
       new int[] {  0,  0, 30, 50, 15 },
       new int[] {  0,  0, 15, 55, 30 },
       new int[] {  0,  0, 10, 50, 40 },
       new int[] {  0,  0,  5, 45, 50 }  // Level 20
    };

    #region Data
    private string name;
    private int level;
    private float experience;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public int Level { get => level; }
    public float Experience { get => experience; }

    public static int[][] QualityDistribution { get => qualityDistribution; }
    #endregion Properties


    #region Methods
    public Skill(string name, int level)
    {
        this.name = name;
        this.level = level;
        this.experience = 0f;
    }

    public void AddExperience(float experience)
    {
        if (level != MAX_LEVEL)
        {
            this.experience += experience;
            CheckLevelup();
        }
    }

    public void CheckLevelup()
    {
        if (experience >= level * EXPERIENCE_MODIFIER)
        {
            experience -= level * EXPERIENCE_MODIFIER;
            level++;
        }
    }
    #endregion Methods
}


public enum SkillsEnum : byte { Crafting, Building, CombatMelee, CombatRanged }