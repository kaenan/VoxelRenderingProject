using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class OceanWaves : MonoBehaviour
{
    MeshFilter mf;
    Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        vertices = mf.mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        if(vertices != null)
        {

        }
    }
}
