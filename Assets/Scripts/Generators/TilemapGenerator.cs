using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapGenerator
{
    #region Methods
    public Tilemap CreateTilemap(Grid tilemapHolder, string name)
    {
        GameObject gameObject = new GameObject(name);
        Tilemap tilemapChunk = gameObject.AddComponent<Tilemap>();
        gameObject.AddComponent<TilemapRenderer>();

        tilemapChunk.tileAnchor = new Vector3(0.5f, 0.5f, 0);
        gameObject.transform.SetParent(tilemapHolder.transform);

        return tilemapChunk;
    }

    public void FillTilemap(Dungeon dungeon, Tilemap tilemap)
    {
        int mapSizeX = dungeon.Map.GetLength(0);
        int mapSizeY = dungeon.Map.GetLength(1);

        tilemap.ClearAllTiles();

        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                if (dungeon.Map[x, y] == null) continue;
                Vector3Int position = new Vector3Int(x, y, 0);
                tilemap.SetTile(position, Data.GetTile(dungeon.Map[x, y]));
            }
        }
    }
    #endregion Methods
}