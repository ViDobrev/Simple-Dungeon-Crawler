using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "TileManager", menuName = "Custom/Tile Manager")]
public class TileManager : ScriptableObject
{
    public TilePackArray[] tilePacks;
}


[System.Serializable]
public class TilePack
{
    public string name;
    public Tile tile;
}

[System.Serializable]
public class TilePackArray
{
    public string packName;
    public TilePack[] tilePacks;
}