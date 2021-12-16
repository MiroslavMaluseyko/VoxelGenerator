using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTile : MonoBehaviour
{
    //size in voxels
    public int tileSize = 8;
    public float voxelSideSize = 0.1f;

    //Tile weight for generation
    [Range(0,100)]
    public int weight = 50;
    
    //how many different tiles can be obtained by rotation
    public RotationType rotationType;
    public enum RotationType
    {
        NoRotation,
        TwoRotations,
        FourRotations
    }

    //voxel color on each tile side
    [HideInInspector]
    public byte[] colorsLeft;
    [HideInInspector]
    public byte[] colorsForward;
    [HideInInspector]
    public byte[] colorsRight;
    [HideInInspector]
    public byte[] colorsBack;
    
    
    public void CalculateColors()
    {
        colorsLeft = new byte[tileSize*tileSize];
        colorsForward = new byte[tileSize*tileSize];
        colorsRight = new byte[tileSize*tileSize];
        colorsBack = new byte[tileSize*tileSize];

        for(int layer = 0;layer<tileSize;layer++)
        {
            for(int offset = 0;offset<tileSize;offset++)
            {
                colorsLeft[layer*tileSize + offset] = GetVoxelColor(layer,offset,Direction.Left);
                colorsForward[layer*tileSize + offset] = GetVoxelColor(layer,offset,Direction.Forward);
                colorsRight[layer*tileSize + offset] = GetVoxelColor(layer,offset,Direction.Right);
                colorsBack[layer*tileSize + offset] = GetVoxelColor(layer,offset,Direction.Back);
            }

        }
    }

    public void Rotate()
    {
        gameObject.transform.Rotate(0, 90, 0);
        int totalVoxels = tileSize * tileSize;

        for(int i = 0;i<totalVoxels;i++)
        {
            byte temp = colorsLeft[i];
            colorsLeft[i] = colorsBack[i];
            colorsBack[i] = colorsRight[i];
            colorsRight[i] = colorsForward[i];
            colorsForward[i] = temp;
        }

    }

    private byte GetVoxelColor(int layer, int offset, Direction direction)
    {
        var meshCollider = GetComponentInChildren<MeshCollider>();

        float vox = voxelSideSize;
        float half = vox / 2;


        Vector3 rayStart;
        Vector3 rayDirection;

        switch (direction)
        {
            case Direction.Left:
                rayStart = meshCollider.bounds.min +
                    new Vector3(-half, 0, half + offset * vox);
                rayDirection = Vector3.right;
                break;
            case Direction.Back:
                rayStart = meshCollider.bounds.min +
                    new Vector3(tileSize*vox - (half + offset * vox), 0, -half);
                rayDirection = Vector3.forward;
                break;
            case Direction.Right:
                rayStart = meshCollider.bounds.max +
                    new Vector3(half, 0, -half - offset * vox);
                rayDirection = Vector3.left;
                break;
            case Direction.Forward:
                rayStart = meshCollider.bounds.max +
                    new Vector3(-tileSize*vox + half + offset*vox,0,half);
                rayDirection = Vector3.back;
                break;
            default:
                rayStart = new Vector3();
                
                rayDirection = Vector3.forward;
                break;
        }


        rayStart.y = meshCollider.bounds.min.y + layer * vox + half;

        if(Physics.Raycast(new Ray(rayStart,rayDirection), out RaycastHit hit, vox))
        {
            byte colorIndex = (byte)(hit.textureCoord.x*256);
            if (colorIndex == 0) Debug.LogWarning("Collor is 0, shit!");

            return colorIndex;
        }

        return 0;
    }

}
