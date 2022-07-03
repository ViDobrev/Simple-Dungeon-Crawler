using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack
{
    #region Data
    // Example 2d8+1 Slashing
    private int dice;
    private int dieSides;
    private int bonus;
    private DamageType damageType;
    #endregion Data

    #region Properties
    //public string Damage { get => $"{dice}d{dieSides}+{bonus} {damageType}"; }
    //public int DamageMax { get => dice * dieSides + bonus; }
    //public int DamageMin { get => dice + bonus; }
    //public DamageType DamageType { get => damageType; }
    #endregion Properties


    #region Methods
    public Attack(string damageString, int bonus)
    {// String example: 2d8 Slashing
        int dIndex = damageString.IndexOf('d');
        int _Index = damageString.IndexOf(' ');

        int dice;
        int dieSides;

        if (!int.TryParse(damageString.Substring(0, dIndex), out dice)) dice = 0;
        if (!int.TryParse(damageString.Substring(dIndex + 1, _Index - dIndex - 1), out dieSides)) dieSides = 0;

        DamageType damageType;
        if (!Enum.TryParse(damageString.Substring(_Index + 1), out damageType)) damageType = DamageType.Unknown;

        this.dice = dice;
        this.dieSides = dieSides;
        this.bonus = bonus;
        this.damageType = damageType;
    }
    public Attack(Attack originalAttack)
    {// Used for item cloning
        dice = originalAttack.dice;
        dieSides = originalAttack.dieSides;
        bonus = originalAttack.bonus;
        damageType = originalAttack.damageType;
    }

    public Damage RollDamage(int strength)
    {
        int damage = Data.diceRng.Next(dice, (dice * dieSides) + 1);
        damage += bonus + strength;
        if (damage < 1) damage = 1;
        return new Damage(damage, damageType);
    }

    public override string ToString()
    {
        return $"{dice}d{dieSides}+{bonus} {damageType}";
    }
    #endregion Methods
}


public class Damage
{
    #region Data
    private int power;
    private DamageType damageType;
    #endregion Data

    #region Properties
    public int Power { get => power; }
    public DamageType Type { get => damageType; }
    #endregion Properties


    #region Methods
    public Damage(int power, DamageType damageType)
    {
        this.power = power;
        this.damageType = damageType;
    }
    #endregion Methods
}


public enum DamageType : byte { Slashing, Piercing, Bludgeoning, Unknown };