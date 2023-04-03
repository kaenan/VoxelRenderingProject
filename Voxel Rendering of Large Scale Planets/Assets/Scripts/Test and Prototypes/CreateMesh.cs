using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{
	public Material material;
	public GameObject[] points = new GameObject[4];

	// Start is called before the first frame update
	void Start()
    {
		Vector3[] vertices = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		int[] triangles = new int[6];

		vertices[0] = points[0].transform.position;
		vertices[1] = points[1].transform.position;
		vertices[2] = points[2].transform.position;
		vertices[3] = points[3].transform.position;

		uv[0] = new Vector2(0,1);
		uv[1] = new Vector2(1,1);
		uv[2] = new Vector2(0,0);
		uv[3] = new Vector2(1,0);

		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 1;
		triangles[5] = 3;

		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer>().material = material;
    }
}
