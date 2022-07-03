using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Species
{
    #region Data
    private string name;
    private bool intelligent;
    private Dictionary<StatName, int> baseStats;
    private Attack unarmedAttack;

    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public bool Intelligent { get => intelligent; }
    public int Health { get => 100 + (5 * baseStats[StatName.Vitality]); }
    public Dictionary<StatName, int> BaseStats { get => baseStats; }
    public Attack UnarmedAttack { get => unarmedAttack; }

    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public Species(string name, bool intelligent, Dictionary<StatName, int> baseStats, Attack unarmedAttack, Tile tile)
    {
        this.name = name;
        this.intelligent = intelligent;
        this.baseStats = baseStats;
        this.unarmedAttack = unarmedAttack;

        this.tile = tile;
    }
    #endregion Methods
}



public enum MovementType : byte { Land, Water, Air }