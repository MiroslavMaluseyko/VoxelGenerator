
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbstractTilePlacer : MonoBehaviour
{
    
    private Vector2Int _mapSize;
    protected Vector2Int CurrMapSize;
    protected List<VoxelTile> TilePrefabs;
    protected VoxelTile[,] SpawnedTiles;
    public abstract void Generate();
    public void SetTilePrefabs(List<VoxelTile> tilePrefs)
    {
        TilePrefabs = tilePrefs;
    }
    public void Clear()
    {
        if (SpawnedTiles == null)
        {
            CurrMapSize = _mapSize;
            SpawnedTiles = new VoxelTile[CurrMapSize.x,CurrMapSize.y];
            return;
        }
        for(int x = 1;x<CurrMapSize.x - 1;x++)
        {
            for (int y = 1; y < CurrMapSize.y - 1; y++)
            {
                Destroy(SpawnedTiles[x, y]?.gameObject);
                SpawnedTiles[x, y] = null;
            }
        }
        if(CurrMapSize.x > _mapSize.x || CurrMapSize.y > _mapSize.y)
            SpawnedTiles = new VoxelTile[_mapSize.x,_mapSize.y];
        CurrMapSize = _mapSize;
    }
    protected bool CanAppendTile(VoxelTile tileToAppend, VoxelTile existingTile, Direction dir)
    {

        if (existingTile == null) return true;

        int size = existingTile.tileSize;
        for (int layer = 0; layer < size; layer++)
        {
            for (int offset = 0; offset < size; offset++)
            {
                switch (dir)
                {
                    case Direction.Right:
                        if (existingTile.colorsLeft[layer * size + offset] != tileToAppend.colorsRight[(layer + 1) * size - 1 - offset])
                            return false;
                        break;
                    case Direction.Left:
                        if (existingTile.colorsRight[layer * size + offset] != tileToAppend.colorsLeft[(layer + 1) * size - 1 - offset])
                            return false;
                        break;
                    case Direction.Back:
                        if (existingTile.colorsForward[layer * size + offset] != tileToAppend.colorsBack[(layer + 1) * size - 1 - offset])
                            return false;
                        break;
                    case Direction.Forward:
                        if (existingTile.colorsBack[layer * size + offset] != tileToAppend.colorsForward[(layer + 1) * size - 1 - offset])
                            return false;
                        break;
                    default:
                        break;
                }
            }
        }
        return true;
    }

    public void SetMapSize(Vector2Int ms)
    {
        _mapSize = ms;
    }
    
}