using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlantSpecies
{
    private static float[] lifeCycle = { 0f, 40f, 80f};

    #region Data
    private string name;
    private int health;
    private float growingRate;
    private ItemData resource;
    private GatherMethod gatherMethod;
    private Dictionary<PlantStage, int> yield;

    private Color colour;
    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public int Health { get => health; }
    public float GrowingRate { get => growingRate; }
    public ItemData Resource { get => resource; }
    public GatherMethod GatherMethod { get => gatherMethod; }
    public Dictionary<PlantStage, int> Yield { get => yield; }

    public Color Colour { get => colour; }
    public Tile Tile { get => tile; }

    public static float[] LifeCycle { get => lifeCycle; }
    #endregion Properties


    #region Methods
    public PlantSpecies(string name, int health, float growingRate, ItemData resource, GatherMethod gatherMethod, Dictionary<PlantStage, int> yield, Color colour, Tile tile)
    {
        this.name = name;
        this.health = health;
        this.growingRate = growingRate;
        this.resource = resource;
        this.gatherMethod = gatherMethod;
        this.yield = yield;
        this.colour = colour;
        this.tile = tile;
    }
    #endregion Methods
}



public enum GatherMethod : byte { None, Harvest, Cut }

public enum PlantStage : byte { Sprout, Young, Mature }