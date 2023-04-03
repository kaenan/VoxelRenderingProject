using UnityEngine;

public static class Version8
{
    /// <summary>
    /// 
    /// This version (8) uses the chunks method and compute shaders
    /// 
    /// </summary>

    public static void Setup(Version8Settings settings)
    {
        if (settings.terrainColour == null)
        {
            settings.terrainColour = ScriptableObject.CreateInstance<TerrainColour>();
        }

        if(settings.planetSize <= 100)
        {
            settings.chunkSize = 10;
        } else if (settings.planetSize < 1000)
        {
            settings.chunkSize = 50;
        } else
        {
            settings.chunkSize = 100;
        }

        settings.containerSize = settings.planetSize * 2;
        settings.centre = new Vector3(settings.planetSize, settings.planetSize, settings.planetSize);
        settings.container = new GameObject("Planet", typeof(Planet));
        settings.container.transform.position = settings.centre;

        settings.terrainColour.PlanetHeightAddValue(0);
        settings.terrainColour.PlanetHeightAddValue((settings.planetSize / 2) + settings.heightMultiplier * (settings.scale + 1));

        settings.container.GetComponent<Planet>().planetData = ScriptableObject.CreateInstance<PlanetData>();
        settings.container.GetComponent<Planet>().planetData.SetPlanetTerrainColour(settings.terrainColour);
    }

    public static void CreateChunks(Version8Settings settings, int numChunks)
    {
        settings.chunks = new Chunk[numChunks * numChunks * numChunks];
        int i = 0;
        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                for (int z = 0; z < numChunks; z++)
                {
                    //Position of chunk
                    Vector3Int position = new(x * settings.chunkSize, y * settings.chunkSize, z * settings.chunkSize);

                    //Create chunk struct and add to list
                    GameObject chunkContainer = new(position.ToString());
                    chunkContainer.transform.SetParent(settings.container.transform);
                    Chunk chunk = new(position, chunkContainer, settings.terrainColour.planetMaterial);
                    settings.chunks[i] = chunk;
                    i++;
                }
            }
        }
    }

    public static void GenerateChunk(Version8Settings settings, Chunk chunk)
    {
        settings.triangleBuffer.SetCounterValue(0);
        settings.generateChunks.SetBuffer(0, "triangles", settings.triangleBuffer);
        settings.generateChunks.SetTexture(0, "noiseTexture", settings.texture);

        settings.generateChunks.SetInt("textureSize", settings.containerSize);
        settings.generateChunks.SetFloat("threshold", settings.noiseThreshold);
        settings.generateChunks.SetFloat("planetSize", settings.planetSize);
        settings.generateChunks.SetInts("startingPosition", chunk.startingPosition.x, chunk.startingPosition.y, chunk.startingPosition.z);

        int threads = settings.chunkSize;
        if (chunk.startingPosition.x + settings.chunkSize >= settings.containerSize || chunk.startingPosition.y + settings.chunkSize >= settings.containerSize || chunk.startingPosition.z + settings.chunkSize >= settings.containerSize)
        {
            threads -= 1;
        }

        settings.generateChunks.Dispatch(0, threads, threads, threads);

        int[] vertexCountData = new int[1];
        settings.triCountBuffer.SetData(vertexCountData);
        ComputeBuffer.CopyCount(settings.triangleBuffer, settings.triCountBuffer, 0);
        settings.triCountBuffer.GetData(vertexCountData);
        int numVertices = vertexCountData[0] * 3;
        settings.triangleBuffer.GetData(settings.vertexDataArray, 0, 0, numVertices);

        chunk.CreateMesh(settings.vertexDataArray, numVertices, settings.centre, settings.planetSize, settings.treeDensity);
    }

    public static int CalculateNumberOfChunks(Version8Settings settings)
    {
        float a = (float)settings.containerSize / settings.chunkSize;
        int b = (int)Mathf.Ceil(settings.containerSize / settings.chunkSize);
        if (a > b) b += 1;
        if (b < 1) b = 1;
        return b;
    }

    public static void CalculateVertexCount(int chunkSize, out int vertexCount)
    {
        int numVoxels = (chunkSize - 1) * (chunkSize - 1) * (chunkSize - 1);
        vertexCount= (numVoxels * 5) * 3;
    }

    public static void CreateComputeBuffers(out VertexData[] vertexDataArray, out ComputeBuffer triangleBuffer, out ComputeBuffer triCountBuffer, int vertexCount)
    {
        vertexDataArray = new VertexData[vertexCount];
        triangleBuffer = new ComputeBuffer(vertexCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexData)), ComputeBufferType.Append);
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
    }

    public static void DisposeBuffers(ComputeBuffer triangleBuffer, ComputeBuffer triCountBuffer)
    {
        triangleBuffer.Dispose();
        triCountBuffer.Dispose();
    }
}
