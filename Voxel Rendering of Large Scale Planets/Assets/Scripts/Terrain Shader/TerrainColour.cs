using UnityEngine;

[CreateAssetMenu()]
public class TerrainColour : ScriptableObject
{
    public Gradient gradient;
    public Material planetMaterial;
    private Texture2D texture;
    const int textureResolution = 50;

    private bool pointsSet;
    public float minimumPoint;
    public float maximumPoint;

    public void PlanetHeightAddValue(float value)
    {
        if (!pointsSet)
        {
            minimumPoint = float.MaxValue;
            maximumPoint = float.MinValue;
            pointsSet = true;
        }

        if (value > maximumPoint)
        {
            maximumPoint = value;
        }
        if (value < minimumPoint)
        {
            minimumPoint = value;
        }
    }

    public void UpdateElevation(float minPoint, float maxPoint, Vector3 centre)
    {
        planetMaterial.SetVector("_elevation", new Vector4(minPoint, maxPoint));
        planetMaterial.SetVector("_centre", centre);
    }

    public void UpdateColours()
    { 
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }

        Color[] colours = new Color[textureResolution];

        for (int i = 0; i < textureResolution; i++)
        {
            colours[i] = gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colours);
        texture.Apply();
        planetMaterial.SetTexture("_PlanetTexture", texture);
    }
}
