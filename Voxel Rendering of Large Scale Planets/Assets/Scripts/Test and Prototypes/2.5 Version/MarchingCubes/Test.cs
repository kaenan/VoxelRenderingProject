using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TreeEditor;
using NoiseTest;

public class Test : MonoBehaviour
{
    [Header("Planet Settings")]
    [Range(0, 1)] public float scale = 1;
    public Vector3 offset;
    public int planetSize = 30;
    public Material material;
    public bool smoothNormals = false;
    public int amplitude = 1;

    private List<GameObject> meshes = new List<GameObject>();
    private NormalRenderer normalRenderer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Destroy(GameObject.Find("Mesh"));

            Marching marching = new MarchingCubes();
            OpenSimplexNoise noise = new OpenSimplexNoise();
            marching.Surface = planetSize / 2;

            var voxels = new VoxelArray(planetSize * 2, planetSize * 2, planetSize * 2);
            Vector3 centre = new Vector3(planetSize / 2, planetSize / 2, planetSize / 2);

            for (int x = 0; x < planetSize * 2; x++)
            {
                for (int y = 0; y < planetSize * 2; y++)
                {
                    for (int z = 0; z < planetSize * 2; z++)
                    {
                        Vector3 voxelPosition = new Vector3(x, y, z);
                        float distance = Vector3.Distance(voxelPosition, centre);
                        double x2 = (x + offset.x) * scale;
                        double y2 = (y + offset.y) * scale;
                        double z2 = (z + offset.z) * scale;
                        float noiseValue = distance + (float)noise.Evaluate(x2, y2, z2) * amplitude;
                        voxels[x, y, z] = noiseValue;
                    }
                }
            }

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            marching.Generate(voxels.Voxels, verts, indices);

            #region normal smoothing
            //Create the normals from the voxel.

            //if (smoothNormals)
            //{
            //    for (int i = 0; i < verts.Count; i++)
            //    {
            //        Presumes the vertex is in local space where
            //        the min value is 0 and max is width/height/depth.
            //        Vector3 p = verts[i];

            //        float u = p.x / (planetSize - 1.0f);
            //        float v = p.y / (planetSize - 1.0f);
            //        float w = p.z / (planetSize - 1.0f);

            //        Vector3 n = voxels.GetNormal(u, v, w);

            //        normals.Add(n);
            //    }

            //    normalRenderer = new NormalRenderer();
            //    normalRenderer.DefaultColor = Color.red;
            //    normalRenderer.Length = 0.25f;
            //    normalRenderer.Load(verts, normals);
            //}
            #endregion

            var position = new Vector3(planetSize / 2, planetSize / 2, planetSize / 2);
            CreateMesh(verts, normals, indices, position);

        }

    }

    private void CreateMesh(List<Vector3> verts, List<Vector3> normals, List<int> indices, Vector3 position)
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
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.transform.localPosition = position;

        meshes.Add(go);

    }
}