using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetData planetData;

    private void Start()
    {
        GenerateRenderTexture.CreateRenderTexture3D(planetData);
    }

    void Update()
    {
        planetData.UpdateColours(gameObject);
    }

    public void Terraform(Vector3Int startPoint, Vector3Int hitPoint, Vector3Int[] chunkID, int size, float weight)
    {
        Chunk[] chunks = new Chunk[7];
        int i = 0;
        foreach(Chunk c in planetData.chunks)
        {
            foreach(Vector3Int id in chunkID)
            if(c.chunkID == id.ToString()) 
            {
                chunks[i] = c;
                i++;
            }

            if(i== 7)
            {
                break;
            }
        }


        if (i > 1)
        {
            planetData.terraformCompute.SetTexture(0, "Result", planetData.texture);
            planetData.terraformCompute.SetFloat("weight", weight);
            planetData.terraformCompute.SetInt("size", size);
            planetData.terraformCompute.SetInts("startPoint", startPoint.x, startPoint.y, startPoint.z);
            planetData.terraformCompute.SetInts("hitPoint", hitPoint.x, hitPoint.y, hitPoint.z);
            planetData.terraformCompute.Dispatch(0, size * 2, size * 2, size * 2);

            Version8.CalculateVertexCount(planetData.chunkSize, out planetData.vertexCount);
            Version8.CreateComputeBuffers(out planetData.vertexDataArray, out planetData.triangleBuffer, out planetData.triCountBuffer, planetData.vertexCount);
            foreach (Chunk chunk in chunks)
            {
                GenerateChunk(chunk);
            }
            Version8.DisposeBuffers(planetData.triangleBuffer, planetData.triCountBuffer);
        } else
        {
            Debug.LogError("Chunk could not be found when trying to terraform.");
        }
    }

    public void GenerateChunk(Chunk chunk)
    {
        planetData.triangleBuffer.SetCounterValue(0);
        planetData.generateChunks.SetBuffer(0, "triangles", planetData.triangleBuffer);
        planetData.generateChunks.SetTexture(0, "noiseTexture", planetData.texture);

        planetData.generateChunks.SetInt("textureSize", planetData.containerSize);
        planetData.generateChunks.SetFloat("threshold", planetData.noiseThreshold);
        planetData.generateChunks.SetFloat("planetSize", planetData.planetSize);
        planetData.generateChunks.SetInts("startingPosition", chunk.startingPosition.x, chunk.startingPosition.y, chunk.startingPosition.z);

        int threads = planetData.chunkSize;
        if (chunk.startingPosition.x + planetData.chunkSize >= planetData.containerSize || chunk.startingPosition.y + planetData.chunkSize >= planetData.containerSize || chunk.startingPosition.z + planetData.chunkSize >= planetData.containerSize)
        {
            threads -= 1;
        }

        planetData.generateChunks.Dispatch(0, threads, threads, threads);

        int[] vertexCountData = new int[1];
        planetData.triCountBuffer.SetData(vertexCountData);
        ComputeBuffer.CopyCount(planetData.triangleBuffer, planetData.triCountBuffer, 0);
        planetData.triCountBuffer.GetData(vertexCountData);
        int numVertices = vertexCountData[0] * 3;
        planetData.triangleBuffer.GetData(planetData.vertexDataArray, 0, 0, numVertices);

        chunk.CreateMesh(planetData.vertexDataArray, numVertices, planetData.centre, planetData.planetSize, 1000);
    }
}
