using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dungeon
{
    #region Data
    private MapCell[,] map;
    private List<Room> rooms;
    private DungeonEntrance entrance;
    private DungeonEntrance exit;

    private Agent boss;
    #endregion Data

    #region Properties
    public MapCell[,] Map { get => map; }
    public MapCell EntranceCell { get => entrance.Location; }

    public Agent Boss { get => boss; }
    #endregion Properties


    #region Methods
    public Dungeon(MapCell[,] map)
    {
        this.map = map;
    }


    public MapCell CellAtPosition(int x, int y)
    {
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1)) return null;

        return map[x, y];
    }
    public MapCell CellAtPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= map.GetLength(0) || position.y < 0 || position.y >= map.GetLength(1)) return null;

        return map[position.x, position.y];
    }

    public MapCell GetRandomPosition()
    {// TODO SHOULD DO IT MORE EFFICIENTLY, ADD A SET OF ALL NON NULL CELLS
        MapCell randomCell = null;

        while (randomCell == null)
        {
            int randomX = Data.mapRng.Next(map.GetLength(0));
            int randomY = Data.mapRng.Next(map.GetLength(1));

            randomCell = CellAtPosition(randomX, randomY);
            if (randomCell == null) continue;
            if (!randomCell.Traversible) randomCell = null;
        }

        return randomCell;
    }

    public void SetEntrance(DungeonEntrance entrance)
    {
        if (entrance.IsEntrance) this.entrance = entrance;
        else exit = entrance;
    }
    public void SetBoss(Agent boss)
    {
        this.boss = boss;
    }

    public void UnlockExit()
    {
        exit.Unlock();
    }
    #endregion Methods
}