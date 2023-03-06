using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class MarchingV7
{
    public float Surface { get; set; }
    private float[] Cube { get; set; }
    protected int[] WindingOrder { get; private set; }

    public MarchingV7(float surface)
    {
        Surface = surface;
        Cube = new float[8];
        WindingOrder = new int[] { 0, 1, 2 };
    }

    public virtual void Generate(IDictionary<string, ArrayList> voxels, IList<Vector3> verts, IList<int> indices, Vector3 startingPosition, int chunkSize)
    {
        UpdateWindingOrder();

        for (int x = (int)startingPosition.x; x < startingPosition.x + chunkSize; x++)
        {
            for (int y = (int)startingPosition.y; y < startingPosition.y + chunkSize; y++)
            {
                for (int z = (int)startingPosition.z; z < startingPosition.z + chunkSize; z++)
                {
                    //Get the values in the 8 neighbours which make up a cube
                    for (int i = 0; i < 8; i++)
                    {

                        int ix = x + VertexOffset[i, 0];
                        int iy = y + VertexOffset[i, 1];
                        int iz = z + VertexOffset[i, 2];

                        string key = ix + "," + iy + "," + iz;

                        Cube[i] = (float) voxels[key][1];
                    }

                    March(x, y, z, Cube, verts, indices);
                }
            }
        }

    }

    protected virtual void UpdateWindingOrder()
    {
        if (Surface > 0.0f)
        {
            WindingOrder[0] = 2;
            WindingOrder[1] = 1;
            WindingOrder[2] = 0;
        }
        else
        {
            WindingOrder[0] = 0;
            WindingOrder[1] = 1;
            WindingOrder[2] = 2;
        }
    }

    protected abstract void March(float x, float y, float z, float[] cube, IList<Vector3> vertList, IList<int> indexList);
    protected virtual float GetOffset(float v1, float v2)
    {
        float delta = v2 - v1;
        return (delta == 0.0f) ? Surface : (Surface - v1) / delta;
    }

    protected static readonly int[,] VertexOffset = new int[,]
    {
        {0, 0, 0},{1, 0, 0},{1, 1, 0},{0, 1, 0},
        {0, 0, 1},{1, 0, 1},{1, 1, 1},{0, 1, 1}
    };

}