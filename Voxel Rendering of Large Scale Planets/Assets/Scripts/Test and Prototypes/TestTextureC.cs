using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class TestTextureC : MonoBehaviour
{
    public int planetSize;
    public int chunkSize;
    public float threshold;
    int numChunks;
    public Material mat;

    float containerSize;
    Vector3 centre;
    GameObject container;
    Chunk[] chunks;

    public float scale;
    public float height;

    [Header("Compute Shaders")]
    public RenderTexture texture;
    private int textureSize;
    public ComputeShader noiseTextureShader;
    public ComputeShader generateChunks;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;
    private VertexData[] vertexDataArray;

    [HideInInspector] public int vertexCount;

    public bool run = false;

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            run = false;

            CreateContainer();
            CreateChunks();
            CreateNoiseTexture();

            CalculateVertexCount();
            CreateComputeBuffers();

            foreach(Chunk chunk in chunks)
            {
                GenerateChunk(chunk);
            }
            ReleaseBuffers();
        }
    }

    private void CreateContainer()
    {
        containerSize = (int)Mathf.Ceil(planetSize * 2f);
        centre = new Vector3(planetSize, planetSize, planetSize);
        container = new GameObject("Planet");
        container.transform.position = centre;
    }

    private void CreateChunks()
    {
        numChunks = CalculateNumberOfChunks();
        chunks = new Chunk[numChunks * numChunks * numChunks];
        int i = 0;
        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                for (int z = 0; z < numChunks; z++)
                {
                    //Position of chunk
                    Vector3Int position = new(x * chunkSize, y * chunkSize, z * chunkSize);

                    //Create chunk struct and add to list
                    GameObject chunkContainer = new(position.ToString());
                    chunkContainer.transform.SetParent(container.transform);
                    Chunk chunk = new(position, chunkContainer, mat);
                    chunks[i] = chunk;
                    i++;
                }
            }
        }
    }

    private void CreateNoiseTexture()
    {
        textureSize = (numChunks * chunkSize);
        Debug.Log(textureSize);
        texture = new RenderTexture(textureSize, textureSize, 0);
        texture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
        texture.volumeDepth = textureSize;
        texture.enableRandomWrite = true;
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        texture.Create();

        noiseTextureShader.SetTexture(0, "Result", texture);
        noiseTextureShader.SetInt("textureSize", textureSize);
        noiseTextureShader.SetInt("planetSize", planetSize);
        noiseTextureShader.SetFloat("containerSize", containerSize);
        noiseTextureShader.SetFloat("scale", scale);
        noiseTextureShader.SetFloat("height", height);
        noiseTextureShader.SetFloats("centre", centre.x, centre.y, centre.z);
        noiseTextureShader.Dispatch(0, textureSize, textureSize, textureSize);
    }

    public void GenerateChunk(Chunk chunk)
    {
        triangleBuffer.SetCounterValue(0);
        generateChunks.SetBuffer(0, "triangles", triangleBuffer);
        generateChunks.SetTexture(0, "noiseTexture", texture);

        generateChunks.SetInt("textureSize", textureSize);
        generateChunks.SetFloat("threshold", threshold);
        generateChunks.SetFloat("planetSize", planetSize);
        generateChunks.SetFloat("chunkSize", chunkSize);
        generateChunks.SetFloats("centre", planetSize, planetSize, planetSize);
        generateChunks.SetInts("startingPosition", chunk.startingPosition.x, chunk.startingPosition.y, chunk.startingPosition.z);

        int threads = chunkSize;
        if (chunk.startingPosition.x + chunkSize >= containerSize || chunk.startingPosition.y + chunkSize >= containerSize || chunk.startingPosition.z + chunkSize >= containerSize)
        {
            threads -= 1;
        }

        generateChunks.Dispatch(0, threads, threads, threads);

        int[] vertexCountData = new int[1];
        triCountBuffer.SetData(vertexCountData);
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        triCountBuffer.GetData(vertexCountData);
        int numVertices = vertexCountData[0] * 3;
        triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);

        chunk.CreateMesh(vertexDataArray, numVertices, centre, planetSize, 10000);
    }

    public int CalculateNumberOfChunks()
    {
        float a = (float)containerSize / chunkSize;
        int b = (int)Mathf.Ceil(containerSize / chunkSize);
        if (a > b) b += 1;
        if (b < 1) b = 1;
        return b;
    }

    public void CalculateVertexCount()
    {
        int numVoxels = (chunkSize - 1) * (chunkSize - 1) * (chunkSize - 1);
        vertexCount = (numVoxels * 5) * 3;
    }

    public void CreateComputeBuffers()
    {
        vertexDataArray = new VertexData[vertexCount];
        triangleBuffer = new ComputeBuffer(vertexCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexData)), ComputeBufferType.Append);
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
    }
    public void ReleaseBuffers()
    {
        triangleBuffer.Dispose();
        triCountBuffer.Dispose();
    }
}
