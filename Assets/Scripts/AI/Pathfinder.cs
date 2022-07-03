using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Pathfinder
{
    #region Data
    private static Dungeon dungeon;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private static Dictionary<Vector2Int, int> directions = new Dictionary<Vector2Int, int>()
        {
            { Vector2Int.up,            MOVE_STRAIGHT_COST },   // N
            { new Vector2Int(1, 1),     MOVE_DIAGONAL_COST },   // NE
            { Vector2Int.right,         MOVE_STRAIGHT_COST },   // E
            { new Vector2Int(1, -1),    MOVE_DIAGONAL_COST },   // SE
            { Vector2Int.down,          MOVE_STRAIGHT_COST },   // S
            { new Vector2Int(-1, -1),   MOVE_DIAGONAL_COST },   // SW
            { Vector2Int.left,          MOVE_STRAIGHT_COST },   // W
            { new Vector2Int(-1, 1),    MOVE_DIAGONAL_COST }    // NW
        };
    #endregion Data


    #region Methods
    public static void SetDungeon(Dungeon _dungeon)
    {
        dungeon = _dungeon;
    }


    public static Queue<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Dictionary<Vector2Int, PathCell> openCells = new Dictionary<Vector2Int, PathCell>();
        HashSet<Vector2Int> closedCells = new HashSet<Vector2Int>();

        PathCell cell = new PathCell(start, null);
        cell.gCost = 0;
        cell.hCost = Math.Abs(end.x - cell.position.x) * MOVE_STRAIGHT_COST + Math.Abs(end.y - cell.position.y) * MOVE_DIAGONAL_COST;
        cell.CalculateFCost();
        openCells.Add(cell.position, cell);

        //MapCell mapCell;
        while (openCells.Count > 0)
        {
            cell = FindLowestFCostCell(openCells);
            //mapCell = dungeon.CellAtPosition(cell.position);
            if (cell.position == end)
            //if (IsWithinRange(cell, end, maxDistance))
            {
                return CalculatePath(cell);
            }
            foreach (var direction in directions.Keys)
            {
                AddCell(cell, direction, directions[direction], end, openCells, closedCells);
            }
            openCells.Remove(cell.position);
            closedCells.Add(cell.position);
        }

        Debug.Log("Path could not be found");
        return null;
    }

    private static PathCell FindLowestFCostCell(Dictionary<Vector2Int, PathCell> openCells)
    {
        PathCell lowestCostCell = openCells.Values.First();

        foreach (PathCell cell in openCells.Values)
        {
            if (cell.FCost < lowestCostCell.FCost) lowestCostCell = cell;
        }

        return lowestCostCell;
    }

    private static void AddCell(PathCell cameFromCell, Vector2Int direction, int directionCost, Vector2Int end, Dictionary<Vector2Int, PathCell> openCells, HashSet<Vector2Int> closedCells)
    {
        Vector2Int newPosition = cameFromCell.position + direction;

        MapCell mapCell = dungeon.CellAtPosition(newPosition);
        if (mapCell == null || !mapCell.Traversible || closedCells.Contains(newPosition)) return;

        if (!openCells.ContainsKey(newPosition))
        {
            PathCell cell = new PathCell(newPosition, cameFromCell);
            cell.gCost = cameFromCell.gCost + directionCost;
            cell.hCost = Math.Abs(end.x - cell.position.x) * MOVE_STRAIGHT_COST + Math.Abs(end.y - cell.position.y) * MOVE_DIAGONAL_COST;
            cell.CalculateFCost();
            openCells.Add(cell.position, cell);
        }
        else
        {
            PathCell cell = openCells[newPosition];
            int newGCost = cameFromCell.gCost + directionCost;
            if (newGCost < cell.gCost) cell.gCost = newGCost;
            cell.hCost = Math.Abs(end.x - cell.position.x) * MOVE_STRAIGHT_COST + Math.Abs(end.y - cell.position.y) * MOVE_DIAGONAL_COST;
            cell.CalculateFCost();
        }
    }

    private static bool IsWithinRange(PathCell cell, Vector2Int target, int maxDistance)
    {// Ignore square rooting, irrelevant since actual distance is unnecessary
        /*int diffX = target.x - cell.position.x;
        int diffY = target.y - cell.position.y;

        int distance = (diffX * diffX) + (diffY * diffY);

        // Max distance value before sqrt is practically 2*maxDistance^2
        return distance <= (2 * maxDistance * maxDistance);*/

        Vector2Int difference = cell.position - target;

        // TODO wrong, use abs()
        return difference.x > maxDistance || difference.x < -maxDistance || difference.y > maxDistance || difference.y < -maxDistance;
    }

    private static Queue<Vector2Int> CalculatePath(PathCell endCell)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Queue<Vector2Int> directionsQueue = new Queue<Vector2Int>();

        PathCell cell = endCell;
        while (cell.previousCell != null)
        {
            path.Add(cell.position - cell.previousCell.position);
            cell = cell.previousCell;
        }

        path.Reverse();

        foreach (var direction in path)
        {
            directionsQueue.Enqueue(direction);
        }

        return directionsQueue;
    }
    #endregion Methods
}


public class PathCell
{
    #region Data
    public Vector2Int position;
    public PathCell previousCell;

    public int gCost;
    public int hCost;
    private int fCost;
    #endregion Data

    #region Properties
    public int FCost { get => fCost; }
    #endregion Properties


    #region Methods
    public PathCell(Vector2Int position, PathCell previousCell)
    {
        this.position = position;
        this.previousCell = previousCell;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    #endregion Methods
}