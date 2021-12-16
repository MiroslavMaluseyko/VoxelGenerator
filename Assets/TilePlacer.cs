
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : AbstractTilePlacer
{
    private List<VoxelTile> [,] _possibleTiles;
    private readonly Queue<Vector2Int> _tilesToRecalc = new Queue<Vector2Int>();

    public override void Generate()
    {
        Clear();
        
        _possibleTiles = new List<VoxelTile>[CurrMapSize.x, CurrMapSize.y];

        for (int x = 0; x < CurrMapSize.x; x++)
        {
            for (int y = 0; y < CurrMapSize.y; y++)
            {
                _possibleTiles[x, y] = new List<VoxelTile>(TilePrefabs);
            }
        }

        Vector2Int maxPos = new Vector2Int(CurrMapSize.x/2, CurrMapSize.y/2);

        int iterationsLimit = CurrMapSize.x*CurrMapSize.y;

        do
        {
            
            _possibleTiles[maxPos.x, maxPos.y] = new List<VoxelTile> { GetRandomTile(_possibleTiles[maxPos.x, maxPos.y]) };

            EnqueueNeighbours(new Vector2Int(maxPos.x, maxPos.y));

            int innerIterations = 500;

            while (_tilesToRecalc.Count > 0 && innerIterations-- > 0)
            {
                Vector2Int pos = _tilesToRecalc.Dequeue();
                if (pos.x == 0 || pos.y == 0 || pos.x == CurrMapSize.x - 1 || pos.y == CurrMapSize.y - 1) continue;

                if(_possibleTiles[pos.x,pos.y].Count == 0)
                {
                    _possibleTiles[pos.x, pos.y] = new List<VoxelTile>(TilePrefabs);
                    _possibleTiles[pos.x+1, pos.y] = new List<VoxelTile>(TilePrefabs);
                    _possibleTiles[pos.x-1, pos.y] = new List<VoxelTile>(TilePrefabs);
                    _possibleTiles[pos.x, pos.y+1] = new List<VoxelTile>(TilePrefabs);
                    _possibleTiles[pos.x, pos.y-1] = new List<VoxelTile>(TilePrefabs);
                    _tilesToRecalc.Enqueue(pos);
                    continue;
                }

                int countRemoved = _possibleTiles[pos.x, pos.y].RemoveAll(t => !CanAppendTileHere(t, pos));

                if (countRemoved == 0) continue;

                EnqueueNeighbours(pos);

            }

            maxPos = Vector2Int.one;
            for (int x = 1; x < CurrMapSize.x - 1; x++)
            {
                for (int y = 1; y < CurrMapSize.y - 1; y++)
                {
                    if (_possibleTiles[x, y].Count > _possibleTiles[maxPos.x, maxPos.y].Count) maxPos = new Vector2Int(x, y);
                }
            }
            if (_possibleTiles[maxPos.x, maxPos.y].Count <= 1) break;
        } while (iterationsLimit-- > 0);
        
        PlaceAllTiles();
    }

    private void EnqueueNeighbours(Vector2Int pos)
    {
        _tilesToRecalc.Enqueue(pos + Vector2Int.right);
        _tilesToRecalc.Enqueue(pos + Vector2Int.left );
        _tilesToRecalc.Enqueue(pos + Vector2Int.up   );
        _tilesToRecalc.Enqueue(pos + Vector2Int.down );
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
        if (_possibleTiles[pos.x + 1, pos.y].All(t => !CanAppendTile(tile, t, Direction.Right))) return false;
        if (_possibleTiles[pos.x - 1, pos.y].All(t => !CanAppendTile(tile, t, Direction.Left))) return false;
        if (_possibleTiles[pos.x, pos.y + 1].All(t => !CanAppendTile(tile, t, Direction.Forward))) return false;
        if (_possibleTiles[pos.x, pos.y - 1].All(t => !CanAppendTile(tile, t, Direction.Back))) return false;
        return true;
    }


    /*Placing tiles*/
    private void PlaceAllTiles()
    {
        for (int x = 1; x < CurrMapSize.x - 1; x++)
            for (int y = 1; y < CurrMapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }
    }


    private void PlaceTile(int x, int y)
    {
        VoxelTile tile = GetRandomTile(_possibleTiles[x,y]);

        //if (tile == null) return;

        Vector3 position = new Vector3(x, 0, y) * (tile.tileSize * tile.voxelSideSize);
        SpawnedTiles[x,y] = Instantiate(tile,position,tile.transform.rotation);
    }

}
