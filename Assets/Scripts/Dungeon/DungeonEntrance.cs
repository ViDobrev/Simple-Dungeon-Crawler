using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonEntrance : Object
{
    #region Data
    private EntranceType type;
    private bool unlocked;
    #endregion Data

    #region Properties
    public bool IsEntrance { get => type == EntranceType.Entrance; }
    public bool IsExit { get => type == EntranceType.Exit; }

    public bool Unlocked { get => unlocked; }
    #endregion Properties


    #region Methods
    public DungeonEntrance(EntranceType type)
        : base($"Dungeon {type}", Color.yellow, Data.Tiles[type.ToString()])
    {
        this.type = type;
        unlocked = false;
    }

    public void Unlock()
    {
        if (type == EntranceType.Exit) unlocked = true;
    }
    #endregion Methods
}


public enum EntranceType { Entrance, Exit }