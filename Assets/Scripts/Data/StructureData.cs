using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class StructureData
{
    #region Data
    private string name;
    private StructureTag tag;
    private int inventorySlots;
    private bool canBeOpened;

    private Color colour;
    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public StructureTag Tag { get => tag; }
    public int InventorySlots { get => inventorySlots; }
    public bool CanBeOpened { get => canBeOpened; }

    public Color Colour { get => colour; }
    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public StructureData(string name, StructureTag tag, int inventorySlots, bool canBeOpened, Color colour, Tile tile)
    {
        this.name = name;
        this.tag = tag;
        this.inventorySlots = inventorySlots;
        this.canBeOpened = canBeOpened;

        this.colour = colour;
        this.tile = tile;
    }
    #endregion Methods
}