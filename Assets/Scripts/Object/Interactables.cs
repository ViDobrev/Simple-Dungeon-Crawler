using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interactables
{
    #region Data
    private Corpse corpse;
    private Structure chest;
    private Structure door;
    #endregion Data

    #region Properties
    public Inventory this[string name]
    {
        get
        {
            if (name == "chest") return chest != null ? chest.Inventory : null;
            if (name == "corpse") return corpse != null ? corpse.Inventory : null;

            return null;
        }
    }
    #endregion Properties


    #region Methods
    public Interactables() { }

    public Structure GetOpenableStructure(string name)
    {
        switch (name)
        {
            case "chest": return chest;
            case "door": return door;
            default: return null;
        }
    }

    public void Update()
    {
        MapCell cellAtPlayer = Data.Player.Location;

        corpse = cellAtPlayer.Corpse;

        if (cellAtPlayer.HasStructure)
        {
            chest = (cellAtPlayer.Structure.Name.ToLower() == "chest" || cellAtPlayer.Structure.Name.ToLower() == "ornate chest") ? cellAtPlayer.Structure : null;
            door = (cellAtPlayer.Structure.Name.ToLower() == "door") ? cellAtPlayer.Structure : null;
        }
        else
        {
            chest = null;
            door = null;
        }
    }
    #endregion Methods
}