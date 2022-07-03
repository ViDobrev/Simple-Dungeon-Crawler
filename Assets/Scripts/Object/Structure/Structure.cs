using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Structure : Object
{
    #region Data
    private StructureTag tag;
    private Inventory inventory;

    private bool canBeOpened;
    private bool opened;
    #endregion Data

    #region Properties
    public bool Traversible { get => tag == StructureTag.Wall; }

    public bool HasInventory { get => inventory != null; }
    public Inventory Inventory { get => inventory; }

    public bool CanBeOpened { get => canBeOpened; }
    public bool Opened { get => opened; }
    #endregion Properties


    #region Methods
    public Structure(StructureData data)
        : base(data.Name, data.Colour, data.Tile)
    {
        tag = data.Tag;

        inventory = data.InventorySlots > 0 ? new Inventory(data.InventorySlots) : null;
        if (inventory != null) inventory.SetParentObject(this);

        canBeOpened = data.CanBeOpened;
        opened = false;
    }

    public bool Open()
    {
        if (canBeOpened && !opened)
        {
            opened = true;
            return true;
        }
        return false;
    }
    public bool Close()
    {
        if (canBeOpened && opened)
        {
            opened = false;
            return true;
        }
        return false;
    }
    #endregion Methods
}



public enum StructureTag : byte
{
    Floor,
    Workstation,
    Storage,
    Door,
    Wall
}