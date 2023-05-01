using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Version8Settings))]
public class Version8Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Planet"))
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var settings = serializedObject.targetObject as Version8Settings;
            Version8.Setup(settings);
            Version8.CreateChunks(settings, Version8.CalculateNumberOfChunks(settings));
            GenerateRenderTexture.CreateRenderTexture3D(settings);
            Version8.CalculateVertexCount(settings.chunkSize, out settings.vertexCount);
            Version8.CreateComputeBuffers(out settings.vertexDataArray, out settings.triangleBuffer, out settings.triCountBuffer, settings.vertexCount);

            foreach (Chunk chunk in settings.chunks)
            {
                Version8.GenerateChunk(settings, chunk);
            }

            CreateWater(settings);
            CreateAtmosphere(settings);
            GenerateTrees(settings);
            Version8.DisposeBuffers(settings.triangleBuffer, settings.triCountBuffer);
            settings.container.GetComponent<Planet>().planetData.SetPlanetSettings(settings);

            timer.Stop();
            Debug.Log("Execution Time = " + timer.ElapsedMilliseconds + "ms");
        }
    }

    private void CreateWater(Version8Settings settings)
    {
        if (settings.water != null)
        {
            GameObject w = Instantiate(settings.water, settings.centre, Quaternion.identity);
            Vector3 scale = new Vector3(settings.planetSize, settings.planetSize, settings.planetSize);
            w.transform.localScale = scale;
            w.transform.SetParent(settings.container.transform);
        }
    }

    private void CreateAtmosphere(Version8Settings settings)
    {
        if (settings.atmosphere != null)
        {
            GameObject w = Instantiate(settings.atmosphere, settings.centre, Quaternion.identity);
            Vector3 scale = new Vector3(settings.planetSize * 2, settings.planetSize * 2, settings.planetSize * 2);
            w.transform.localScale = scale;
            w.transform.SetParent(settings.container.transform);
        }
    }

    private void GenerateTrees(Version8Settings settings)
    {
        if (settings.treePrefab.Length > 0)
        {
            foreach (Chunk chunk in settings.chunks)
            {
                for (int i = 0; i < chunk.treePositions.Count; i++)
                {
                    var t = Instantiate(settings.treePrefab[Random.Range(0, settings.treePrefab.Length)], chunk.treePositions[i], Quaternion.identity);
                    t.transform.SetParent(chunk.container.transform);
                    t.transform.up = chunk.treeNormals[i];
                    t.transform.name = chunk.treeNormals[i].ToString();
                }
            }
        }
    }
}
