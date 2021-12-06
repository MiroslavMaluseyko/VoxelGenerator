
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbstractTilePlacer : MonoBehaviour
{
    
    public Vector2Int MapSize;
    protected List<VoxelTile> tilePrefabs;
    protected VoxelTile[,] spawnedTiles;
    public abstract void Generate();
    public abstract void SetTilePrefabs(List<VoxelTile> tilePrefs);
    public void Clear()
    {
        if (spawnedTiles == null) return;
        for(int x = 1;x<MapSize.x - 1;x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                Destroy(spawnedTiles[x, y]?.gameObject);
                spawnedTiles[x, y] = null;
            }
        }
    }
    protected bool CanAppendTile(VoxelTile tileToAppend, VoxelTile existingTile, Direction dir)
    {

        if (existingTile == null) return true;

        int size = existingTile.TileSize;
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
    
}