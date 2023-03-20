using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetData : ScriptableObject
{
    public TerrainColour terrainColour;

    private Chunk[] chunks;

    public void SetPlanetTerrainColour(TerrainColour terrainColour)
    {
        this.terrainColour = terrainColour;
    }

    public void SetPlanetChunks(Chunk[] chunks)
    {
        this.chunks = new Chunk[chunks.Length];

        int i = 0;
        foreach(Chunk chunk in chunks)
        {
            this.chunks[i] = chunk;
            i++;
        }
    }

    public void UpdateColours(GameObject go)
    {
        terrainColour.UpdateColours();
        terrainColour.UpdateElevation(terrainColour.minimumPoint, terrainColour.maximumPoint, go.transform.position);
    }
}
