using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MyTerrain
{
    #region Data
    private string name;
    private bool traversible;
    //private float movementRequiredToTraverse;

    private Tile tile;
    private Color colour;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public bool Traversible { get => traversible; }
    //public float MovementRequiredToTraverse { get => movementRequiredToTraverse; }

    public Tile Tile { get => tile; }
    public Color Colour { get => colour; }
    #endregion Properties


    #region Methods
    public MyTerrain(string name, bool traversible,/* float movementRequiredToTraverse,*/ Tile tile, Color colour)
    {
        this.name = name;
        this.traversible = traversible;
        //this.movementRequiredToTraverse = movementRequiredToTraverse;

        this.tile = tile;
        this.colour = colour;
    }
    #endregion Methods
}