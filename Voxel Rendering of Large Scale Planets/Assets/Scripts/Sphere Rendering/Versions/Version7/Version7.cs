using UnityEngine;

[ExecuteInEditMode]
public class Version7 : MonoBehaviour
{
    /// <summary>
    /// 
    /// This version uses the chunks method and compute shaders
    /// 
    /// </summary>


    [Header("Planet Settings")]
    public int planetSize = 10;
    public int chunkSize = 10;
    public GameObject water;
    public GameObject atmosphere;
    public TerrainColour terrainColour;

    [Header("Terrain")]
    [Range(0, 1)]public float scale = 1;
    [Range(1, 100)] public float heightMultiplier = 1;
    public Vector3 offset;

    [Header("Compute Shaders")]
    public ComputeShader generateChunks;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;
    private VertexData[] vertexDataArray;

    [Header("Generate New Planet")]
    public bool generatePlanet = false;

    private GameObject container;
    private int containerSize;
    private Vector3 centre;
    private int numChunks;
    private Chunk[] chunks;

    int vertexCount;

    private void Update()
    {
        if (generatePlanet)
        {
            generatePlanet = false;

            if(terrainColour == null) 
            {
                terrainColour = ScriptableObject.CreateInstance<TerrainColour>();
            }

            if (GameObject.Find("Planet " + gameObject.name)) DestroyImmediate(GameObject.Find("Planet " + gameObject.name));
            containerSize = (int) Mathf.Ceil(planetSize * 2f);
            centre = new Vector3(containerSize / 2, containerSize / 2, containerSize / 2);
            container = new GameObject("Planet " + gameObject.name, typeof(Planet));
            container.transform.position = centre;
            container.transform.SetParent(transform);

            terrainColour.PlanetHeightAddValue(0);
            terrainColour.PlanetHeightAddValue((planetSize / 2) + heightMultiplier * (scale + 1));

            container.GetComponent<Planet>().planetData = ScriptableObject.CreateInstance<PlanetData>();
            container.GetComponent<Planet>().planetData.SetPlanetTerrainColour(terrainColour);

            CreateWater();
            CreateAtmosphere();
            CalculateVertexCount();
            CreateComputeBuffers();
            CreateChunks();
            GenerateAllChunks(chunks);
            container.GetComponent<Planet>().planetData.SetPlanetChunks(chunks);
        }
    }

    private void CreateChunks()
    {
        numChunks = CalculateNumberOfChunks(containerSize, chunkSize);
        chunks = new Chunk[numChunks * numChunks * numChunks];
        int i = 0;
        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                for (int z = 0; z < numChunks; z++)
                {
                    //Position of chunk
                    Vector3Int position = new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize);

                    //Create chunk struct and add to list
                    GameObject chunkContainer = new GameObject(position.ToString());
                    chunkContainer.transform.SetParent(container.transform);
                    Chunk chunk = new Chunk(position, chunkContainer, terrainColour.planetMaterial);
                    chunks[i] = chunk;
                    i++;
                }
            }
        }
    }

    private void GenerateAllChunks(Chunk[] chunks)
    {
        foreach(Chunk chunk in chunks)
        {
            GenerateChunk(chunk);
        }
    }

    private void GenerateChunk(Chunk chunk)
    {
        triangleBuffer.SetCounterValue(0);
        generateChunks.SetBuffer(0, "triangles", triangleBuffer);

        generateChunks.SetFloat("planetSize", planetSize);
        generateChunks.SetFloat("chunkSize", chunkSize);
        generateChunks.SetFloat("scale", scale);
        generateChunks.SetFloat("heightMultiplier", heightMultiplier);
        generateChunks.SetFloats("centre", containerSize / 2, containerSize / 2, containerSize / 2);
        generateChunks.SetFloats("noiseOffset", offset.x, offset.y, offset.z);
        generateChunks.SetInts("startingPosition", chunk.startingPosition.x, chunk.startingPosition.y, chunk.startingPosition.z);

        generateChunks.Dispatch(0, chunkSize, chunkSize, chunkSize);

        int[] vertexCountData = new int[1];
        triCountBuffer.SetData(vertexCountData);
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        triCountBuffer.GetData(vertexCountData);
        int numVertices = vertexCountData[0] * 3;
        triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);

        chunk.CreateMesh(vertexDataArray, numVertices);
    }

    private int CalculateNumberOfChunks(int planetSize, int chunkSize)
    {
        float a = (float)planetSize / chunkSize;
        int b = (int)Mathf.Ceil(planetSize / chunkSize);
        if (a > b) b += 1;
        if (b < 1) b = 1;
        return b;
    }

    private void CalculateVertexCount()
    {
        int numVoxels = (chunkSize - 1) * (chunkSize - 1) * (chunkSize - 1);
        vertexCount = (numVoxels * 5) * 3;
    }

    private void CreateComputeBuffers()
    {
        vertexDataArray = new VertexData[vertexCount];
        triangleBuffer = new ComputeBuffer(vertexCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexData)), ComputeBufferType.Append);
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
    }

    private void CreateWater()
    {
        GameObject w = Instantiate(water, centre, Quaternion.identity);
        Vector3 scale = new Vector3(planetSize, planetSize, planetSize);
        w.transform.localScale = scale;
        w.transform.SetParent(container.transform);
    }

    private void CreateAtmosphere()
    {
        if (atmosphere != null)
        {
            GameObject w = Instantiate(atmosphere, centre, Quaternion.identity);
            Vector3 scale = new Vector3(planetSize * 2, planetSize * 2, planetSize * 2);
            w.transform.localScale = scale;
            w.transform.SetParent(container.transform);
        }
    }
}
