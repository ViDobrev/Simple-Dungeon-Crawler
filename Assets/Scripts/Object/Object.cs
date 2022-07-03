using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Object
{
    #region Data
    protected string name;
    protected MapCell location;

    protected Color colour;
    protected Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public MapCell Location { get => location; }
    public virtual Vector2Int Position { get => location.Position; }

    public Color Colour { get => colour; }
    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public Object(string name, Color colour, Tile tile)
    {
        this.name = name;

        this.colour = colour;
        this.tile = tile;
    }

    public void SetLocation(MapCell newLocation)
    {
        location = newLocation;
    }

    public void ChangeColour(Color newColour)
    {
        colour = newColour;
    }
    #endregion Methods
}