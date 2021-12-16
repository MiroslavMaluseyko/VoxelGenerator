
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public enum GenerationType
    {
        WCF,
        brutforce
    }
    
    public TilePlacer wcfPlacer;
    public TilePlacerSimple simplePlacer;
    
    [Header("Generation default settings")]
    public GenerationType genType;
    public List<VoxelTile> tilePrefabs;
    
    private Vector2Int _mapSize;
    private AbstractTilePlacer _placer;

    void Start()
    {
        switch (genType)
        {
            case GenerationType.WCF:
                wcfPlacer.gameObject.SetActive(true);
                simplePlacer.gameObject.SetActive(false);
                _placer = wcfPlacer;
                break;
            case GenerationType.brutforce:
                wcfPlacer.gameObject.SetActive(false);
                simplePlacer.gameObject.SetActive(true);
                _placer = simplePlacer;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        TilePreprocess();
        _placer.SetTilePrefabs(tilePrefabs);
        Generate();
    }

    public void Generate()
    {
        _mapSize = UIManager.MapSize;
        _placer.SetMapSize(_mapSize);
        _placer.Generate();
    }

    public void Test()
    {
        Debug.Log("Aboba");
    }
    //calculating tile`s voxels colors and rotating them
    private void TilePreprocess()
    {
        tilePrefabs = tilePrefabs.FindAll(t=>t.gameObject.activeInHierarchy);
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
                    tilePrefabs[i].weight /= 2;
                    clone = Instantiate(tilePrefabs[i], tilePrefabs[i].transform.position + Vector3.back, Quaternion.identity);
                    clone.Rotate();
                    tilePrefabs.Add(clone);
                    break;
                case VoxelTile.RotationType.FourRotations:
                    tilePrefabs[i].weight /= 4;
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
    }
}
