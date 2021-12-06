using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePlacerSimple : MonoBehaviour
{
    public Vector2Int MapSize;
    public List<VoxelTile> tilePrefabs;

    private VoxelTile[,] spawnedTiles;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            for(int x = 1;x<MapSize.x - 1;x++)
            {
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (spawnedTiles[x, y] == null) continue;
                    Destroy(spawnedTiles[x, y].gameObject);
                    spawnedTiles[x, y] = null;
                }
            }
            Generate();
        }
    }

    void Start()
    {

        spawnedTiles = new VoxelTile[MapSize.x, MapSize.y];

        foreach (VoxelTile tile in tilePrefabs)
        {
            tile.CalculateColors();
        }

        int tilesBeforeAdding = tilePrefabs.Count;
        VoxelTile clone;
        for (int i = 0; i < tilesBeforeAdding; i++)
        {
            switch (tilePrefabs[i].rotationType)
            {
                case VoxelTile.RotationType.TwoRotations:
                    tilePrefabs[i].Weight /= 2;
                    clone = Instantiate(tilePrefabs[i], tilePrefabs[i].transform.position + Vector3.back, Quaternion.identity);
                    clone.Rotate();
                    tilePrefabs.Add(clone);
                    break;
                case VoxelTile.RotationType.FourRotations:
                    tilePrefabs[i].Weight /= 4;
                    clone = Instantiate(tilePrefabs[i], tilePrefabs[i].transform.position + Vector3.back, Quaternion.identity);
                    clone.Rotate();
                    tilePrefabs.Add(clone);
                    clone = Instantiate(tilePrefabs[i], tilePrefabs[i].transform.position + Vector3.back * 2, Quaternion.identity);
                    clone.Rotate();
                    clone.Rotate();
                    tilePrefabs.Add(clone);
                    clone = Instantiate(tilePrefabs[i], tilePrefabs[i].transform.position + Vector3.back * 3, Quaternion.identity);
                    clone.Rotate();
                    clone.Rotate();
                    clone.Rotate();
                    tilePrefabs.Add(clone);
                    break;
                default:
                    break;
            }
        }

        Generate();

    }

    private void Generate()
    {
        for(int x = 1;x < MapSize.x - 1;x++)
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }

    }

    private void PlaceTile(int x, int y)
    {
        List<VoxelTile> possibleHere = tilePrefabs.FindAll(t=>CanAppendTileHere(t,new Vector2Int(x,y)));

        VoxelTile tile = GetRandomTile(possibleHere);
        if (tile == null) return;
        Vector3 position = new Vector3(x, 0, y) * tile.TileSize * tile.VoxelSideSize;
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
}
