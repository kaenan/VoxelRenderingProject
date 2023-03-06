using NoiseTest;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Terrain")]
    [Range(0, 10)]public float scale = 1;
    [Range(1, 10)] public float amplitude = 1;
    public Vector3 offset;
    public float minValue;

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
    }

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
                    chunks.Add(chunk);
                }
            }
        }
    }

    private void ChunkRender()
    {
        OpenSimplexNoise simplexNoise = new OpenSimplexNoise();
        List<Chunk> tempChunk = new List<Chunk>();

        foreach (Chunk chunk in chunks)
        {
            IDictionary<string, ArrayList> vertices = new Dictionary<string, ArrayList>();
            for (int x = chunk.startingPosition.x; x < chunk.startingPosition.x + chunkSize + 1; x++)
            {
                for (int y = chunk.startingPosition.y; y < chunk.startingPosition.y + chunkSize + 1; y++)
                {
                    for (int z = chunk.startingPosition.z; z < chunk.startingPosition.z + chunkSize + 1; z++)
                    {
                        Vector3 voxelPosition = new Vector3(x, y, z);
                        double xPos = x * scale + offset.x;
                        double yPos = y * scale + offset.y;
                        double zPos = z * scale + offset.z;
                        float distance = Vector3.Distance(voxelPosition, centre);
                        float noiseValue = distance + (float)simplexNoise.Evaluate(xPos, yPos, zPos) * amplitude;

                        noiseValue = Mathf.Max(planetSize / 2, noiseValue - minValue);

                        ArrayList voxelInfo;
                        string key;
                        key = x + "," + y + "," + z;

                        voxelInfo = new ArrayList
                            {
                                voxelPosition,
                                noiseValue
                            };
                        vertices.Add(key, voxelInfo);
                    }
                }
            }
            Chunk newChunk = new Chunk();
            newChunk.startingPosition = chunk.startingPosition;
            newChunk.vertices = vertices;
            tempChunk.Add(newChunk);
        }
        chunks = tempChunk;

        foreach(Chunk chunk in chunks)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            MarchingV6 marching = new MarchingCubesV6();
            marching.Surface = planetSize / 2;
            marching.Generate(chunk.vertices, verts, indices, chunk.startingPosition, chunkSize);

            if (verts.Count > 0 || indices.Count > 0)
            {
                var position = new Vector3(containerSize / 2, containerSize / 2, containerSize / 2);
                CreateMesh(verts, normals, indices, position, chunk);
            }
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

    private int CalculateNumberOfChunks(int planetSize, int chunkSize)
    {
        float a = (float)planetSize / chunkSize;
        int b = (int)Mathf.Ceil(planetSize / chunkSize);
        if (a > b) b += 1;
        if (b < 1) b = 1;
        return b;
    }
}
