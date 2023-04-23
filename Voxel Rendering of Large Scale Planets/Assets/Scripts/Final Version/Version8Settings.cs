using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class Version8Settings : ScriptableObject
{
    [Header("Planet Settings")]
    [Tooltip("The diameter of the planet.")]
    public int planetSize;
    [Tooltip("Each vertice has a noise level between 0 and 1. Vertices with a noise value under the threshold will be used to create the mesh.")]
    public float noiseThreshold;
    public GameObject water;
    [HideInInspector] public GameObject atmosphere;
    public TerrainColour terrainColour;
    public bool playable;

    [Header("Terrain")]
    [Range(0, 1)] public float scale;
    [Range(1, 100)] public float heightMultiplier;
    public Vector3 offset;

    [Header("Caves")]
    [Range(0, 10)] public float caveScale;
    [Range(0, 100)] public float sizeMultiplier;
    public Vector3 caveOffset;

    [Header("Trees")]
    public int treeDensity;
    public GameObject[] treePrefab;

    [Header("Compute Shaders")]
    public ComputeShader generateChunks;
    public ComputeShader noiseTexture;
    public ComputeShader terraformCompute;

    [HideInInspector] public RenderTexture texture;
    [HideInInspector] public ComputeBuffer triangleBuffer;
    [HideInInspector] public ComputeBuffer triCountBuffer;
    [HideInInspector] public VertexData[] vertexDataArray;
    [HideInInspector] public int vertexCount;

    [HideInInspector] public int chunkSize;
    [HideInInspector] public GameObject container;
    [HideInInspector] public int containerSize;
    [HideInInspector] public Vector3 centre;
    [HideInInspector] public int numChunks;
    [HideInInspector] public Chunk[] chunks;
}
