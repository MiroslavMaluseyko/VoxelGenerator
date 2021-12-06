
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : AbstractTilePlacer
{
    private List<VoxelTile> [,] possibleTiles;
    private Queue<Vector2Int> tilesToRecalc = new Queue<Vector2Int>();


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            Generate();
            PlaceAllTiles();
        }
    }

    public override void Generate()
    {
        spawnedTiles = new VoxelTile[MapSize.x,MapSize.y];
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                Destroy(spawnedTiles[x, y]?.gameObject);
                spawnedTiles[x, y] = null;
            }
        }
        
        possibleTiles = new List<VoxelTile>[MapSize.x, MapSize.y];

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                possibleTiles[x, y] = new List<VoxelTile>(tilePrefabs);
            }
        }

        Vector2Int maxPos = new Vector2Int(MapSize.x/2, MapSize.y/2);

        int iterationsLimit = MapSize.x*MapSize.y;

        do
        {
            
            possibleTiles[maxPos.x, maxPos.y] = new List<VoxelTile> { GetRandomTile(possibleTiles[maxPos.x, maxPos.y]) };

            EnqueueNeighbours(new Vector2Int(maxPos.x, maxPos.y));

            int innerIterations = 500;

            while (tilesToRecalc.Count > 0 && innerIterations-- > 0)
            {
                Vector2Int pos = tilesToRecalc.Dequeue();
                if (pos.x == 0 || pos.y == 0 || pos.x == MapSize.x - 1 || pos.y == MapSize.y - 1) continue;

                if(possibleTiles[pos.x,pos.y].Count == 0)
                {
                    possibleTiles[pos.x, pos.y] = new List<VoxelTile>(tilePrefabs);
                    possibleTiles[pos.x+1, pos.y] = new List<VoxelTile>(tilePrefabs);
                    possibleTiles[pos.x-1, pos.y] = new List<VoxelTile>(tilePrefabs);
                    possibleTiles[pos.x, pos.y+1] = new List<VoxelTile>(tilePrefabs);
                    possibleTiles[pos.x, pos.y-1] = new List<VoxelTile>(tilePrefabs);
                    tilesToRecalc.Enqueue(pos);
                    continue;
                }

                int countRemoved = possibleTiles[pos.x, pos.y].RemoveAll(t => !CanAppendTileHere(t, pos));

                if (countRemoved == 0) continue;

                EnqueueNeighbours(pos);

            }

            maxPos = Vector2Int.one;
            for (int x = 1; x < MapSize.x - 1; x++)
            {
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (possibleTiles[x, y].Count > possibleTiles[maxPos.x, maxPos.y].Count) maxPos = new Vector2Int(x, y);
                }
            }
            if (possibleTiles[maxPos.x, maxPos.y].Count <= 1) break;
        } while (iterationsLimit-- > 0);
        
        PlaceAllTiles();
    }

    public override void SetTilePrefabs(List<VoxelTile> tilePrefs)
    {
        tilePrefabs = tilePrefs;
    }

    private void EnqueueNeighbours(Vector2Int pos)
    {
        tilesToRecalc.Enqueue(pos + Vector2Int.right);
        tilesToRecalc.Enqueue(pos + Vector2Int.left );
        tilesToRecalc.Enqueue(pos + Vector2Int.up   );
        tilesToRecalc.Enqueue(pos + Vector2Int.down );
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
        if (possibleTiles[pos.x + 1, pos.y].All(t => !CanAppendTile(tile, t, Direction.Right))) return false;
        if (possibleTiles[pos.x - 1, pos.y].All(t => !CanAppendTile(tile, t, Direction.Left))) return false;
        if (possibleTiles[pos.x, pos.y + 1].All(t => !CanAppendTile(tile, t, Direction.Forward))) return false;
        if (possibleTiles[pos.x, pos.y - 1].All(t => !CanAppendTile(tile, t, Direction.Back))) return false;
        return true;
    }
    private bool CanAppendTile(VoxelTile tileToAppend, VoxelTile existingTile, Direction dir)
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
    

    /*Placing tiles*/
    private void PlaceAllTiles()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }
    }


    private void PlaceTile(int x, int y)
    {
        VoxelTile tile = GetRandomTile(possibleTiles[x,y]);

        //if (tile == null) return;

        Vector3 position = new Vector3(x, 0, y) * (tile.TileSize * tile.VoxelSideSize);
        spawnedTiles[x,y] = Instantiate(tile,position,tile.transform.rotation);
    }

}
