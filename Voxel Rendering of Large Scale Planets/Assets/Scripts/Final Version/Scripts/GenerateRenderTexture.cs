using System.Threading;
using UnityEngine;

public static class GenerateRenderTexture
{
    public static void CreateRenderTexture3D(Version8Settings settings)
    {
        settings.texture = new RenderTexture(settings.containerSize, settings.containerSize, 0);
        settings.texture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
        settings.texture.volumeDepth = settings.containerSize;
        settings.texture.enableRandomWrite = true;
        settings.texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        settings.texture.Create();

        settings.noiseTexture.SetTexture(0, "Result", settings.texture);
        settings.noiseTexture.SetInt("planetSize", settings.planetSize);
        settings.noiseTexture.SetFloat("scale", settings.scale);
        settings.noiseTexture.SetFloat("height", settings.heightMultiplier);
        settings.noiseTexture.SetFloats("offset", settings.offset.x, settings.offset.y, settings.offset.z);
        settings.noiseTexture.SetFloat("caveScale", settings.caveScale);
        settings.noiseTexture.SetFloat("sizeMultiplier", settings.sizeMultiplier);
        settings.noiseTexture.SetFloats("caveOffset", settings.caveOffset.x, settings.caveOffset.y, settings.caveOffset.z);
        settings.noiseTexture.Dispatch(0, settings.containerSize, settings.containerSize, settings.containerSize);
        settings.container.GetComponent<Planet>().planetData.SetRenderTexture(settings.texture);
    }
    
    public static void CreateRenderTexture3D(PlanetData planetData)
    {
        planetData.texture = new RenderTexture(planetData.containerSize, planetData.containerSize, 0);
        planetData.texture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
        planetData.texture.volumeDepth = planetData.containerSize;
        planetData.texture.enableRandomWrite = true;
        planetData.texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        planetData.texture.Create();

        planetData.noiseTexture.SetTexture(0, "Result", planetData.texture);
        planetData.noiseTexture.SetInt("planetSize", planetData.planetSize);
        planetData.noiseTexture.SetFloat("scale", planetData.scale);
        planetData.noiseTexture.SetFloat("height", planetData.heightMultiplier);
        planetData.noiseTexture.SetFloats("offset", planetData.offset.x, planetData.offset.y, planetData.offset.z);
        planetData.noiseTexture.SetFloat("caveScale", planetData.caveScale);
        planetData.noiseTexture.SetFloat("sizeMultiplier", planetData.sizeMultiplier);
        planetData.noiseTexture.SetFloats("caveOffset", planetData.caveOffset.x, planetData.caveOffset.y, planetData.caveOffset.z);
        planetData.noiseTexture.Dispatch(0, planetData.containerSize, planetData.containerSize, planetData.containerSize);
    }
}
