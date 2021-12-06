using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTile : MonoBehaviour
{
    public int TileSize = 8;
    public float VoxelSideSize = 0.1f;

    [Range(0,100)]
    public int Weight = 50;

    public RotationType rotationType;
    public enum RotationType
    {
        NoRotation,
        TwoRotations,
        FourRotations
    }

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
        colorsLeft = new byte[TileSize*TileSize];
        colorsForward = new byte[TileSize*TileSize];
        colorsRight = new byte[TileSize*TileSize];
        colorsBack = new byte[TileSize*TileSize];

        for(int layer = 0;layer<TileSize;layer++)
        {
            for(int offset = 0;offset<TileSize;offset++)
            {
                colorsLeft[layer*TileSize + offset] = GetVoxelColor(layer,offset,Direction.Left);
                colorsForward[layer*TileSize + offset] = GetVoxelColor(layer,offset,Direction.Forward);
                colorsRight[layer*TileSize + offset] = GetVoxelColor(layer,offset,Direction.Right);
                colorsBack[layer*TileSize + offset] = GetVoxelColor(layer,offset,Direction.Back);
            }

        }
    }

    public void Rotate()
    {
        gameObject.transform.Rotate(0, 90, 0);
        int totalVoxels = TileSize * TileSize;

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

        float vox = VoxelSideSize;
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
                    new Vector3(TileSize*vox - (half + offset * vox), 0, -half);
                rayDirection = Vector3.forward;
                break;
            case Direction.Right:
                rayStart = meshCollider.bounds.max +
                    new Vector3(half, 0, -half - offset * vox);
                rayDirection = Vector3.left;
                break;
            case Direction.Forward:
                rayStart = meshCollider.bounds.max +
                    new Vector3(-TileSize*vox + half + offset*vox,0,half);
                rayDirection = Vector3.back;
                break;
            default:
                rayStart = new Vector3();
                
                rayDirection = Vector3.forward;
                break;
        }


        rayStart.y = meshCollider.bounds.min.y + layer * vox + half;

        //Debug.DrawRay(rayStart,rayDirection*vox,Color.green,2);

        if(Physics.Raycast(new Ray(rayStart,rayDirection), out RaycastHit hit, vox))
        {
            byte colorIndex = (byte)(hit.textureCoord.x*256);
            if (colorIndex == 0) Debug.LogWarning("Collor is 0, shit!");

            return colorIndex;
        }

        return 0;
    }

}
