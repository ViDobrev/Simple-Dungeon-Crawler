using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisplayNode : MonoBehaviour
{
    #region Data
    private LogicNode logicNode;
    private DataNode dataNode;
    private Dungeon dungeon;

    [SerializeField] private Grid grid;
    #endregion Data

    #region Properties
    public float GridCellSizeX { get => grid.cellSize.x; }
    #endregion Properties


    #region Methods
    public void Initialize(LogicNode logicNode, DataNode dataNode)
    {
        this.logicNode = logicNode;
        this.dataNode = dataNode;
    }
    public void SetDungeon(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }


    public void DisplayDungeon()
    {
        TilemapGenerator tilemapGenerator = new TilemapGenerator();
        Tilemap tilemap = tilemapGenerator.CreateTilemap(grid, "Tilemap");
        Data.SetTilemap(tilemap);

        tilemapGenerator.FillTilemap(dungeon, tilemap);
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
    }
    void Update()
    {
    }
    #endregion Behaviour
}