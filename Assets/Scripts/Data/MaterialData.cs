using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialData
{
    #region Data
    private string name;
    private string adjective;
    private int tier;
    private int modifier;

    private bool canBeUsedForWeapon;
    private bool canBeUsedForArmour;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public string Adjective { get => adjective; }
    public int Tier { get => tier; }
    public int Modifier { get => modifier; }

    public bool CanBeUsedForWeapon { get => canBeUsedForWeapon; }
    public bool CanBeUsedForArmour { get => canBeUsedForArmour; }
    #endregion Properties


    #region Methods
    public MaterialData(string name, string adjective, int tier, int modifier, bool canBeUsedForWeapon, bool canBeUsedForArmour)
    {
        this.name = name;
        this.adjective = adjective;
        this.tier = tier;
        this.modifier = modifier;

        this.canBeUsedForWeapon = canBeUsedForWeapon;
        this.canBeUsedForArmour = canBeUsedForArmour;
    }
    #endregion Methods
}

public enum MaterialType : byte
{
    Weapon,
    Armour
}