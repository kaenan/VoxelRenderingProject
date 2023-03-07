using NoiseTest;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;

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
    public Material meshTexture;
    public GameObject voxelPrefab;

    [Header("Terrain")]
    [Range(0, 10)]public float scale = 1;
    [Range(0, 100)] public float heightMultiplier = 1;
    //public Vector3 offset;
    //public float minValue;

    [Header("Compute Shaders")]
    public ComputeShader generateChunks;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

    private int numChunks;
    private GameObject container;
    private int containerSize;
    private Vector3 centre;
    private List<Chunk> chunks = new List<Chunk>();

    struct Chunk
    {
        public Vector3Int startingPosition;
        public IDictionary<string, ArrayList> vertices;
        public List<GameObject> meshes;
        public List<Vector3> processedVertices;
        public List<Vector3> processedNormals;
        public List<int> processedTriangles;
    }
    struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
    };


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.Find("Planet")) Destroy(GameObject.Find("Planet"));
            containerSize = (int) Mathf.Ceil(planetSize * 1.5f);
            centre = new Vector3(containerSize / 2, containerSize / 2, containerSize / 2);
            container = new GameObject("Planet");
            container.transform.position = centre;
            chunks.Clear();

            CreateChunks();
            GenerateChunk();
        }
    }

    private void CreateChunks()
    {
        numChunks = CalculateNumberOfChunks(containerSize, chunkSize);

        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                for (int z = 0; z < numChunks; z++)
                {
                    //Position of chunk
                    Vector3Int position = new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize);

                    //Create chunk struct and add to list
                    Chunk chunk = new Chunk();
                    chunk.startingPosition = position;
                    chunk.processedVertices = new List<Vector3>();
                    chunk.processedNormals = new List<Vector3>();
                    chunk.processedTriangles = new List<int>();
                    chunks.Add(chunk);
                }
            }
        }
    }

    private void GenerateChunk()
    {
        float[] centre = new float[3];
        centre[0] = containerSize / 2;
        centre[1] = containerSize / 2;
        centre[2] = containerSize / 2;
        foreach (Chunk chunk in chunks)
        {
            int[] startingPosition = new int[3];
            startingPosition[0] = chunk.startingPosition.x;
            startingPosition[1] = chunk.startingPosition.y;
            startingPosition[2] = chunk.startingPosition.z;

            int numVoxels = (chunkSize - 1) * (chunkSize - 1) * (chunkSize - 1);
            int vertexCount = (numVoxels * 5) * 3;

            VertexData[] vertexDataArray = new VertexData[vertexCount];
            triangleBuffer = new ComputeBuffer(vertexCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexData)), ComputeBufferType.Append);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            triangleBuffer.SetCounterValue(0);
            generateChunks.SetBuffer(0, "triangles", triangleBuffer);

            generateChunks.SetFloat("planetSize", planetSize);
            generateChunks.SetFloat("chunkSize", chunkSize);
            generateChunks.SetFloat("scale", scale);
            generateChunks.SetFloat("heightMultiplier", heightMultiplier);
            generateChunks.SetFloats("centre", centre);
            generateChunks.SetInts("startingPosition", startingPosition);
            generateChunks.Dispatch(0, chunkSize / 8, chunkSize / 8, chunkSize / 8);

            int[] vertexCountData = new int[1];
            triCountBuffer.SetData(vertexCountData);
            ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);

            triCountBuffer.GetData(vertexCountData);

            int numVertices = vertexCountData[0] * 3;

            triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);

            CreateMesh2(vertexDataArray, numVertices, chunk);
        }
    }

    private void CreateMesh(List<Vector3> verts, List<Vector3> normals, List<int> indices, Vector3 position, Chunk chunk)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);

        if (normals.Count > 0)
            mesh.SetNormals(normals);
        else
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        GameObject go = new GameObject("Mesh");
        go.transform.SetParent(container.transform);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = meshTexture;
        go.GetComponent<MeshFilter>().mesh = mesh;
        //go.transform.position = position;
        chunk.meshes = new List<GameObject>() { go };
    }

    private void CreateMesh2(VertexData[] vertexData, int numVertices, Chunk chunk)
    {
        Mesh mesh = new Mesh();
        chunk.processedVertices.Clear();
        chunk.processedNormals.Clear();
        chunk.processedTriangles.Clear();

        int triangleIndex = 0;

        for (int i = 0; i < numVertices; i++)
        {
            VertexData data = vertexData[i];
            chunk.processedVertices.Add(data.position);
            chunk.processedNormals.Add(data.normal);
            chunk.processedTriangles.Add(triangleIndex);
            triangleIndex++;
        }
 
        mesh.SetVertices(chunk.processedVertices);
        mesh.SetTriangles(chunk.processedTriangles, 0, true);
        mesh.RecalculateNormals();

        GameObject go = new GameObject("Mesh");
        go.transform.SetParent(container.transform);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = meshTexture;
        go.GetComponent<MeshFilter>().mesh = mesh;

    }

    private int CalculateNumberOfChunks(int planetSize, int chunkSize)
    {
        float a = (float)planetSize / chunkSize;
        int b = (int)Mathf.Ceil(planetSize / chunkSize);
        if (a > b) b += 1;
        if (b < 1) b = 1;
        return b;
    }
}
