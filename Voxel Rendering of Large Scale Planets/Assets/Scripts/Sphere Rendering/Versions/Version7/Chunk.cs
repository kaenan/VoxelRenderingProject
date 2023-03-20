using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public string chunkID;
    public Vector3Int startingPosition;

    public GameObject container;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public List<GameObject> meshes;
    public List<Vector3> processedVertices;
    public List<Vector3> processedNormals;
    public List<int> processedTriangles;

    Mesh mesh;

    public Chunk(Vector3Int startingPosition, GameObject container, Material material) 
    {
        this.startingPosition = startingPosition;
     
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        meshRenderer = container.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        meshFilter = container.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        this.container = container;

        processedVertices = new List<Vector3>();
        processedNormals = new List<Vector3>();
        processedTriangles = new List<int>();
    }

    public void CreateMesh(VertexData[] vertexData, int numVertices)
    {
        processedVertices.Clear();
        processedNormals.Clear();
        processedTriangles.Clear();

        int triangleIndex = 0;

        for (int i = 0; i < numVertices; i++)
        {
            VertexData data = vertexData[i];
            processedVertices.Add(data.position);
            processedNormals.Add(data.normal);
            processedTriangles.Add(triangleIndex);
            triangleIndex++;
        }

        mesh.SetVertices(processedVertices);
        mesh.SetTriangles(processedTriangles, 0, true);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }
}
