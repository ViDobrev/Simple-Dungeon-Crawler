using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FloorData
{
    #region Data
    private string name;

    private Color colour;
    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }

    public Color Colour { get => colour; }
    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public FloorData(string name, Color colour, Tile tile)
    {
        this.name = name;

        this.colour = colour;
        this.tile = tile;
    }
    #endregion Methods
}