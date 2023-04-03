using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

[System.Serializable]
public class TerrainGeneration
{
    public float strength = 1;
    [Range(1, 8)]
    public int numLayers = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistence = .5f;

}
