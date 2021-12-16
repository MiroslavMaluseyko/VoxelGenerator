
using System.Collections.Generic;
using UnityEngine;

public class TilePlacerSimple : AbstractTilePlacer
{
    public override void Generate()
    {
        Clear();
        
        for(int x = 1;x < CurrMapSize.x - 1;x++)
            for (int y = 1; y < CurrMapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }

    }
    

    private void PlaceTile(int x, int y)
    {
        List<VoxelTile> possibleHere = TilePrefabs.FindAll(t=>CanAppendTileHere(t,new Vector2Int(x,y)));

        VoxelTile tile = GetRandomTile(possibleHere);
        if (tile == null) return;
        Vector3 position = new Vector3(x, 0, y) * (tile.tileSize * tile.voxelSideSize);
        SpawnedTiles[x, y] = Instantiate(tile, position, tile.transform.rotation);
    }

    private VoxelTile GetRandomTile(List<VoxelTile> tiles)
    {
        if (tiles.Count == 0) return null;
        int sum = 0;
        foreach (VoxelTile tile in tiles)
        {
            sum += tile.weight;
        }

        int value = Random.Range(0, sum);
        sum = 0;

        foreach (VoxelTile tile in tiles)
        {
            sum += tile.weight;
            if (sum >= value) return tile;
        }
        return tiles[tiles.Count - 1];
    }

    private bool CanAppendTileHere(VoxelTile tile, Vector2Int pos)
    {
        if (!CanAppendTile(tile, SpawnedTiles[pos.x + 1, pos.y],  Direction.Right)) return false;
        if (!CanAppendTile(tile,SpawnedTiles[pos.x - 1, pos.y],  Direction.Left)) return false;
        if (!CanAppendTile(tile,SpawnedTiles[pos.x, pos.y + 1],  Direction.Forward)) return false;
        if (!CanAppendTile(tile,SpawnedTiles[pos.x, pos.y - 1],  Direction.Back)) return false;
        return true;
    }
}
