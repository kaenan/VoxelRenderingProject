using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class FlatPlainTerrain : MonoBehaviour
{
    public int size = 1;
    public MeshFilter plane;
    public float heighScale;
    public float scale;

    OpenSimplexNoise snoise;

    private bool ran = false;
    // Start is called before the first frame update
    void Start()
    {
        snoise = new OpenSimplexNoise();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ran)
        {
            //MeshFilter[] meshFilters = new MeshFilter[size*size];
            CombineInstance[] combine = new CombineInstance[size * size];
            int i = 0;
            for (int x = 0; x < size*10; x+=10) 
            {
                for (int z = 0; z < size*10; z += 10)
                {
                    var filter = Instantiate(plane, new Vector3(x, 0, z), Quaternion.identity);
                    combine[i].mesh = filter.sharedMesh;
                    combine[i].transform = filter.transform.localToWorldMatrix;
                    filter.gameObject.SetActive(false);
                    i++;
                }
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;

            Vector3[] vertex = new Vector3[mesh.vertexCount];
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                float noise = 0;
                float frequency = scale / 100;
                float amplitude = 1;

                for (int k = 0; k < 6; k++)
                {
                    float n = 1 - Mathf.Abs((float)snoise.Evaluate(mesh.vertices[j].x, mesh.vertices[j].y, mesh.vertices[j].z) * frequency * 2 - 1);
                    noise += n * amplitude;

                    amplitude *= 0.5f;
                    frequency *= 2;
                }
                vertex[j] = new Vector3(mesh.vertices[j].x, noise * heighScale, mesh.vertices[j].z);
            }
            mesh.vertices = vertex;
            mesh.RecalculateBounds();


            ran = true;
        }
    }
}

