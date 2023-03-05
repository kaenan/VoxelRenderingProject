using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalsReturn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		foreach(Vector3 n in GetComponent<MeshFilter>().mesh.normals)
		{
			Debug.Log("Normal of mesh: " + n);
		}

		foreach (Vector3 n in GetComponent<MeshFilter>().mesh.vertices)
		{
			Debug.Log("Vert of mesh: " + n);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
