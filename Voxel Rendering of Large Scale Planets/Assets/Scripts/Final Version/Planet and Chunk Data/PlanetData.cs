using System;
using UnityEngine;

[Serializable]
public class PlanetData : ScriptableObject
{
    public TerrainColour terrainColour;
    public RenderTexture texture;

    [HideInInspector] public int planetSize;
    [HideInInspector] public float noiseThreshold;
    [HideInInspector] public bool playable;

    [HideInInspector] public float scale;
    [HideInInspector] public float heightMultiplier;
    [HideInInspector] public Vector3 offset;

    [HideInInspector] public float caveScale;
    [HideInInspector] public float sizeMultiplier;
    [HideInInspector] public Vector3 caveOffset;

    [HideInInspector] public ComputeShader generateChunks;
    [HideInInspector] public ComputeShader noiseTexture;
    [HideInInspector] public ComputeShader terraformCompute;

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

    public void SetPlanetSettings(Version8Settings settings)
    {
        planetSize = settings.planetSize;
        noiseThreshold = settings.noiseThreshold;
        playable = settings.playable;
        scale = settings.scale;
        heightMultiplier = settings.heightMultiplier;
        offset = settings.offset;
        generateChunks = settings.generateChunks;
        noiseTexture= settings.noiseTexture;
        terraformCompute = settings.terraformCompute;
        chunkSize= settings.chunkSize;
        container= settings.container;
        containerSize= settings.containerSize;
        centre= settings.centre;
        numChunks= settings.numChunks;
        chunks = new Chunk[settings.chunks.Length];
        Array.Copy(settings.chunks, chunks, settings.chunks.Length);
        caveScale = settings.caveScale;
        sizeMultiplier = settings.sizeMultiplier;
        caveOffset = settings.caveOffset;
    }

    public void SetPlanetTerrainColour(TerrainColour terrainColour)
    {
        this.terrainColour = terrainColour;
    }

    public void UpdateColours(GameObject go)
    {
        terrainColour.UpdateColours();
        terrainColour.UpdateElevation(terrainColour.minimumPoint, terrainColour.maximumPoint, go.transform.position);
    }
     public void SetRenderTexture(RenderTexture renderTexture)
    {
        texture = renderTexture;
    }
}
