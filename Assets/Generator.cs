using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public enum GenerationType
    {
        WCF,
        brutforce
    }
    
    public TilePlacer wcfPlacer;
    public TilePlacerSimple simplePlacer;
    
    [Header("Generation settings")]
    public GenerationType genType;
    
    public List<VoxelTile> tilePrefabs;
    private AbstractTilePlacer placer;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            placer.Generate();
        }
    }

    void Start()
    {
        switch (genType)
        {
            case GenerationType.WCF:
                wcfPlacer.gameObject.SetActive(true);
                placer = wcfPlacer;
                break;
            case GenerationType.brutforce:
                simplePlacer.gameObject.SetActive(true);
                placer = simplePlacer;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        TilePreprocess();
        placer.SetTilePrefabs(tilePrefabs);
        placer.Generate();
    }
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
    }
}
