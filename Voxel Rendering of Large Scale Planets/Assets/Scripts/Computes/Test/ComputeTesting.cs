using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTesting : MonoBehaviour
{
    public ComputeShader RayTracingShader;

    private RenderTexture target;
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void SetShaderParameters()
    {
        RayTracingShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraToInverseProjection", camera.projectionMatrix.inverse);

    }
private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(target, destination);
    }

    private void InitRenderTexture()
    {
        if(target == null || target.width != Screen.width || target.height != Screen.height)
        {
            if(target!= null)
            {
                target.Release();
            }

            target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite= true;
            target.Create();

        }
    }
}
