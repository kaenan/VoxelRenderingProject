using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoiseTest;

[ExecuteInEditMode]
public class NoiseVisulisation : MonoBehaviour
{
	public RawImage debugImage;

	[Header("Terrain Parameters")]
	[Range(0, 100)] public int planetSize = 100;
	[Range(0, 200)] public int zPosition = 0;
	[Range(0, 1)] public float scale = 0.5f;
	[Range(1, 100)] public float amplitude = 1f;
	public Vector2 offset = new Vector2(0, 0);

	[Header("Cave Parameters")]
    [Range(0, 1)] public float caveScale = 0.5f;
    [Range(1, 100)] public float caveAmplitude = 1f;
	[Range(0, 100)] public float threshold = 1f;
    public Vector2 caveOffset = new Vector2(0, 0);


    private int mapsize;
	private Vector3 centre;
	private OpenSimplexNoise simplexNoise = new OpenSimplexNoise();

	void Update()
	{
        mapsize = planetSize * 2;
        centre = new Vector3(mapsize / 2, mapsize / 2, mapsize / 2);
		GenerateNoiseMapVisulisation(mapsize);
	}

	public void GenerateNoiseMapVisulisation(int mapSize)
	{
		Color[] pixels = new Color[mapSize * mapSize];
		int i = 0;
		for (float x = 0; x < mapSize; x++)
		{
			for (float y = 0; y < mapSize; y++)
			{
				Vector3 position = new Vector3(x, y, zPosition);
				float distance = Vector3.Distance(centre, position);

				double xPos = x * scale + offset.x;
				double yPos = y * scale + offset.y;
				float edge = distance + (float)simplexNoise.Evaluate(xPos, yPos, zPosition) * amplitude;
				
				if (edge < planetSize / 2)
				{
                    double xPos2 = x * caveScale + caveOffset.x;
                    double yPos2 = y * caveScale + caveOffset.y;
                    float cave = distance * (float)simplexNoise.Evaluate(xPos2, yPos2, zPosition) * caveAmplitude;

					if(cave > threshold * caveAmplitude)
					{
                        pixels[i] = Color.white;
                    } else
					{
                        pixels[i] = Color.black;
                    }
				}
				else if (edge <= (planetSize / 2) + 1)
				{
					pixels[i] = Color.white;
				}
				else
				{
					pixels[i] = Color.black;
				}
				i++;
			}
		}
		//GenerateCaveMapVisulisation(pixels, mapSize);
		Texture2D texture = new Texture2D(mapSize, mapSize);
		texture.SetPixels(pixels);
		texture.filterMode = FilterMode.Point;
		texture.Apply();
		debugImage.texture = texture;
	}

	public void GenerateCaveMapVisulisation(Color[] pixels, int mapSize)
	{
        int i = 0;
        for (float x = 0; x < mapSize; x++)
        {
            for (float y = 0; y < mapSize; y++)
            {
                Vector3 position = new Vector3(x, y, zPosition);
                float distance = Vector3.Distance(centre, position);

                double xPos = x * caveScale + offset.x;
                double yPos = y * caveScale + offset.y;

                distance = distance * (float)simplexNoise.Evaluate(xPos, yPos, zPosition) * caveAmplitude;

                if (distance < planetSize / 2 && pixels[i] == Color.red)
                {
                    pixels[i] = Color.white;
                } else if (pixels[i] == Color.red) 
				{
					pixels[i] = Color.black;
				}
                i++;
            }
        }
        Texture2D texture = new Texture2D(mapSize, mapSize);
		texture.SetPixels(pixels);
		texture.filterMode = FilterMode.Point;
		texture.Apply();
		debugImage.texture = texture;
	}
}
