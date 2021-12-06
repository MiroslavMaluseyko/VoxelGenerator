
using System.Collections.Generic;

using UnityEngine;

public class TilePlacerSimple : AbstractTilePlacer
{
    public override void Generate()
    {
        spawnedTiles = new VoxelTile[MapSize.x,MapSize.y];
        for(int x = 1;x<MapSize.x - 1;x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                Destroy(spawnedTiles[x, y]?.gameObject);
                spawnedTiles[x, y] = null;
            }
        }
        for(int x = 1;x < MapSize.x - 1;x++)
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }

    }
    
    public override void SetTilePrefabs(List<VoxelTile> tilePrefs)
    {
        tilePrefabs = tilePrefs;
    }

    private void PlaceTile(int x, int y)
    {
        List<VoxelTile> possibleHere = tilePrefabs.FindAll(t=>CanAppendTileHere(t,new Vector2Int(x,y)));

        VoxelTile tile = GetRandomTile(possibleHere);
        if (tile == null) return;
        Vector3 position = new Vector3(x, 0, y) * (tile.TileSize * tile.VoxelSideSize);
        spawnedTiles[x, y] = Instantiate(tile, position, tile.transform.rotation);
    }

    private VoxelTile GetRandomTile(List<VoxelTile> tiles)
    {
        if (tiles.Count == 0) return null;
        int sum = 0;
        foreach (VoxelTile tile in tiles)
        {
            sum += tile.Weight;
        }

        int value = Random.Range(0, sum);
        sum = 0;

        foreach (VoxelTile tile in tiles)
        {
            sum += tile.Weight;
            if (sum >= value) return tile;
        }
        return tiles[tiles.Count - 1];
    }

    private bool CanAppendTileHere(VoxelTile tile, Vector2Int pos)
    {
        if (!CanAppendTile(tile, spawnedTiles[pos.x + 1, pos.y],  Direction.Right)) return false;
        if (!CanAppendTile(tile,spawnedTiles[pos.x - 1, pos.y],  Direction.Left)) return false;
        if (!CanAppendTile(tile,spawnedTiles[pos.x, pos.y + 1],  Direction.Forward)) return false;
        if (!CanAppendTile(tile,spawnedTiles[pos.x, pos.y - 1],  Direction.Back)) return false;
        return true;
    }
}
