using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Atmosphere : MonoBehaviour
{
    private Mesh mesh;
    public bool done = false;

    //void Start()
    //{
    //    mesh = GetComponent<MeshFilter>().sharedMesh;
    //    mesh.triangles = mesh.triangles.Reverse().ToArray();
    //}

    private void Update()
    {
        if (!done)
        {
            done = true;
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
            mesh.RecalculateNormals();
        }
    }
}
