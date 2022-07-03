using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utilities
{
    #region Methods
    public static int RandomNumberByPropbability(int[] numbers, int[] probabilities)
    {
        int[] probRefArr = new int[probabilities.Length];
        probRefArr[0] = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            probRefArr[i] = probRefArr[i - 1] + probabilities[i];
        }

        if (probRefArr[probRefArr.Length-1] == 0)
        {
            Debug.LogError("Utilities.RandomNumberByPropbability(): All probabilities given are 0, returning 0.");
            return 0;
        }

        int randomNumber = Data.mathsRng.Next(0, probRefArr[probRefArr.Length - 1]);
        int ceiledValue = 0;
        for (int i = 0; i < probRefArr.Length; i++)
        {
            if (probRefArr[i] > randomNumber)
            {
                ceiledValue = i;
                break;
            }
        }
        return numbers[ceiledValue];
    }

    public static MapCell GetClosestCellByCondition(MapCell startCell, Func<MapCell, bool> condition)
    {
        if (condition(startCell)) return startCell;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        Vector2Int position = startCell.Position;

        int direction = 0;
        int steps = 1;
        int iter = 0;
        int cellsChecked = 0;
        //Spiral pattern
        while (cellsChecked < 50)
        {
            for (int i = 0; i < steps; i++)
            {
                position += directions[direction];
                MapCell cell = Data.Dungeon.CellAtPosition(position);
                if (condition(cell)) return cell;
            }

            direction++;
            if (direction > directions.Length - 1) direction = 0;

            iter++;
            if (iter == 2)
            {
                steps++;
                iter = 0;
            }
            cellsChecked++;
        }


        return null;
    }

    public static ItemSlot FindItemByCondition(List<List<ItemSlot>> allItems, Func<ItemSlot, bool> condition)
    {
        foreach(List<ItemSlot> list in allItems)
        {
            foreach(ItemSlot slot in list)
            {
                if (condition(slot)) return slot;
            }
        }

        return null;
    }
    #endregion Methods
}