using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NoiseTest;

public class SphereWithNoGameObjects : MonoBehaviour
{
	[SerializeField] GameObject voxelPrefab;
	//[SerializeField] Material newMaterial;
	//[SerializeField] Material secondMat;
	[SerializeField] Material meshTexture;
	[SerializeField] int sizeOfPlanet;
	[SerializeField] private float noiseScale = 0.05f;
	[SerializeField, Range(0, 1)] private float threshold = 0.5f;
	[SerializeField, Range(0, 100)] private int terrainRoughness = 0;

	private GameObject container;

	private List<CombineInstance> blockData = new List<CombineInstance>();
	private List<Mesh> meshes = new List<Mesh>();
	private IDictionary<string, ArrayList> vertices = new Dictionary<string, ArrayList>();

	private string combinationName;
	int sizeOfPlanetAndRoughness;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			Debug.Log("Deleting Sphere...");
			Destroy(GameObject.Find("Planet"));
			vertices.Clear();
			meshes.Clear();
			blockData.Clear();
			CreateSphere();
			Debug.Log("New Sphere Generated.");
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			Debug.Log("Marching the cubes...");
			MarchCube();
			Debug.Log("Cubes have marched...");
		}
	}

	private void CreateSphere()
	{
		sizeOfPlanetAndRoughness = sizeOfPlanet + terrainRoughness;
		Vector3 centre = new Vector3(sizeOfPlanetAndRoughness / 2, sizeOfPlanetAndRoughness / 2, sizeOfPlanetAndRoughness / 2);

		for (int x = 0; x < sizeOfPlanetAndRoughness; x++)
		{
			for (int y = 0; y < sizeOfPlanetAndRoughness; y++)
			{
				for (int z = 0; z < sizeOfPlanetAndRoughness; z++)
				{
					Vector3 position = new Vector3(x, y, z);
					float distance = Vector3.Distance(position, centre);
					float noiseValue = PerlinNoise3D(x * noiseScale, y * noiseScale, z * noiseScale);
					Vector3 voxelPosition;
					ArrayList voxelInfo;
					string key;

					if (distance < (sizeOfPlanet / 2) - 1)
					{
						key = x + "," + y + "," + z;
						voxelPosition = new Vector3(x, y, z);
						voxelInfo = new ArrayList();
						voxelInfo.Add(voxelPosition);
						voxelInfo.Add(true);
						vertices.Add(key, voxelInfo);
					}
					else
					{
						key = x + "," + y + "," + z;
						voxelPosition = new Vector3(x, y, z);
						voxelInfo = new ArrayList();
						voxelInfo.Add(voxelPosition);
						voxelInfo.Add(false);
						vertices.Add(key, voxelInfo);
					}
				}
			}
		}
	}

	public static float PerlinNoise3D(float x, float y, float z)
	{
		float ab = Mathf.PerlinNoise(x, y);
		float bc = Mathf.PerlinNoise(y, z);
		float ac = Mathf.PerlinNoise(x, z);

		float ba = Mathf.PerlinNoise(y, x);
		float cb = Mathf.PerlinNoise(z, y);
		float ca = Mathf.PerlinNoise(z, x);

		float abc = ab + bc + ac + ba + cb + ca;
		return abc / 6f;
	}

	private void MarchCube()
	{
		for (int x = 0; x < sizeOfPlanetAndRoughness - 1; x++)
		{
			for (int y = 1; y < sizeOfPlanetAndRoughness; y++)
			{
				for (int z = 1; z < sizeOfPlanetAndRoughness; z++)
				{
					string AKey = x + "," + y + "," + z;
					string BKey = x + "," + y + "," + (z - 1);
					string CKey = x + "," + (y - 1) + "," + z;
					string DKey = x + "," + (y - 1) + "," + (z - 1);
					string EKey = x + 1 + "," + y + "," + z;
					string FKey = x + 1 + "," + y + "," + (z - 1);
					string GKey = x + 1 + "," + (y - 1) + "," + z;
					string HKey = x + 1 + "," + (y - 1) + "," + (z - 1);

					ArrayList A = vertices[AKey];
					ArrayList B = vertices[BKey];
					ArrayList C = vertices[CKey];
					ArrayList D = vertices[DKey];
					ArrayList E = vertices[EKey];
					ArrayList F = vertices[FKey];
					ArrayList G = vertices[GKey];
					ArrayList H = vertices[HKey];

					string combination =
						ConvertToCode((bool) A[1]) + "," +
						ConvertToCode((bool) B[1]) + "," +
						ConvertToCode((bool) C[1]) + "," +
						ConvertToCode((bool) D[1]) + "," +
						ConvertToCode((bool) E[1]) + "," +
						ConvertToCode((bool) F[1]) + "," +
						ConvertToCode((bool) G[1]) + "," +
						ConvertToCode((bool) H[1]);

					MeshCombination((Vector3) A[0], (Vector3) B[0], (Vector3) C[0], (Vector3) D[0], (Vector3) E[0], (Vector3) F[0], (Vector3) G[0], (Vector3) H[0], combination);
				}
			}
		}
		CreatePlanet();
	}

	private void MeshCombination(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Vector3 E, Vector3 F, Vector3 G, Vector3 H, string combination)
	{
		Vector3[] combinationVertices;
		Vector3[] normalsOfMesh;
		int[] triangles;
		combinationName = combination;
		switch (combination)
		{
			//
			//ONE TRIANGLE COMBINATIONS
			//

			case "1,1,1,0,0,0,0,0":
				//A,B,C
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,0,1,0,0,0,0":
				//A,B,D
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,1,1,0,0,0,0":
				//A,C,D
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,1,1,0,0,0,0":
				//B,C,D
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = B;
				combinationVertices[1] = C;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,1,0,0,0,1,0":
				//A,C,G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,0,0,1,0,1,0":
				//A,E,G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,1,0,1,0,0,0":
				//A,C,E
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,1,0,1,0,1,0":
				//C,E,G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,0,1,0,1,1":
				//E,G,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = G;
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,0,1,1,0,1":
				//E,F,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = H;
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,0,1,1,1,0":
				//E,F,G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,0,0,1,1,1":
				//F,G,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = H;
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,0,1,0,1,0,0":
				//B,D,F
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,0,1,0,0,0,1":
				//B,D,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,0,0,0,1,0,1":
				//B,F,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,1,0,1,0,1":
				//D,F,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = F;
				combinationVertices[1] = D;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,0,0,1,0,0,0":
				//A,B,E
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,0,0,0,1,0,0":
				//A,B,F
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,0,0,1,1,0,0":
				//A,E,F
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = E;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,0,0,1,1,0,0":
				//B,E,F
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,1,1,0,0,1,0":
				//C,D,G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = D;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,1,1,0,0,0,1":
				//C,D,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = C;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,1,0,0,0,1,1":
				//C,G,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,1,0,0,1,1":
				//D,G,H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,1,1,1,1,1,1":
				//Everything except A
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,1,1,1,1,1,1":
				//Everything except B
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,0,1,1,1,1,1":
				//Everything except C
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,0,1,1,1,1":
				//Everything except D
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = C;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,1,0,1,1,1":
				//Everything except E
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,1,1,0,1,1":
				//Everything except F
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,1,1,1,0,1":
				//Everything except G
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = H;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,1,1,1,1,0":
				//Everything except H
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = D;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,0,1,0,1,1,1":
				//DFGH
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,1,1,0,0,1,0":
				//ACDG
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,0,1,0,1,0,1,1":
				//CEGH
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,1,1,0,0,0,1":
				//BCDH
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,0,1,0,1,0,0":
				//ABDF
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,0,0,0,1,1,1,0":
				//AEFG
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "1,1,1,0,1,0,0,0":
				//ABCE
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			case "0,1,0,0,1,1,0,1":
				//BEFH
				combinationVertices = new Vector3[3];
				normalsOfMesh = new Vector3[3];
				triangles = new int[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				CreateMesh(combinationVertices, normalsOfMesh, triangles, 3, 2);
				break;

			///////////////////////////////
			//                           //
			// TWO TRIANGLE COMBINATIONS //
			//                           //
			///////////////////////////////

			case "1,1,1,1,0,0,0,0":
				//A,B,C,D Front of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,0,1,0,1,0,1":
				//B,F,D,H Left of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,0,0,1,1,1,1":
				//E,F,G,H Back of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,0,1,0,1,0":
				//A,C,E,G Left of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,0,1,1,0,0":
				//Top of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(0, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,1,1,0,0,1,1":
				//Bottom of the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(0, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,0,0,1,1":
				//A,B,C,D,G,H aka slant facing upwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 1, 0);
				normalsOfMesh[1] = new Vector3(1, 1, 0);
				normalsOfMesh[2] = new Vector3(1, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,0,1,1,1,1":
				//A,B,E,F,G,H aka slant facing downwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, 0);
				normalsOfMesh[1] = new Vector3(-1, -1, 0);
				normalsOfMesh[2] = new Vector3(-1, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,1,1,1,1,1,1":
				//C,D,E,F,G,H aka slant facing upwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, 0);
				normalsOfMesh[1] = new Vector3(-1, 1, 0);
				normalsOfMesh[2] = new Vector3(-1, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,1,1,0,0":
				//A,B,C,D,E,F aka slant facing downwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, -1, 0);
				normalsOfMesh[1] = new Vector3(1, -1, 0);
				normalsOfMesh[2] = new Vector3(1, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,1,1,0,1,1":
				//A,C,D,E,G,H aka slant facing upwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(0, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, -1);
				normalsOfMesh[2] = new Vector3(0, 1, -1);
				normalsOfMesh[3] = new Vector3(0, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,1,1,1,0,1":
				//A,B,D,E,F,H aka slant facing downwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 1);
				normalsOfMesh[2] = new Vector3(0, -1, 1);
				normalsOfMesh[3] = new Vector3(0, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,1,1,0,1,1,1":
				//B,C,D,F,G,H aka slant facing upwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 1);
				normalsOfMesh[2] = new Vector3(0, 1, 1);
				normalsOfMesh[3] = new Vector3(0, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,0,1,1,1,0":
				//ABCEFG aka slant facing downwards
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(0, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, -1);
				normalsOfMesh[2] = new Vector3(0, -1, -1);
				normalsOfMesh[3] = new Vector3(0, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,0,1,0,1":
				//A,B,C,D,F,H face going across the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = C;
				triangles = new int[6];
				normalsOfMesh[0] = new Vector3(1, 0, 1);
				normalsOfMesh[1] = new Vector3(1, 0, 1);
				normalsOfMesh[2] = new Vector3(1, 0, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,0,1,1,1,1":
				//ACEFGH face going across the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 1);
				normalsOfMesh[1] = new Vector3(-1, 0, 1);
				normalsOfMesh[2] = new Vector3(-1, 0, 1);
				normalsOfMesh[3] = new Vector3(-1, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,1,0,1,0":
				//A,B,C,D,E,G face going across the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 0, -1);
				normalsOfMesh[1] = new Vector3(1, 0, -1);
				normalsOfMesh[2] = new Vector3(1, 0, -1);
				normalsOfMesh[3] = new Vector3(1, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,0,1,1,1,1,1":
				//BDEFGH face going across the cube
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, 0, 1);
				normalsOfMesh[1] = new Vector3(-1, 0, 1);
				normalsOfMesh[2] = new Vector3(-1, 0, 1);
				normalsOfMesh[3] = new Vector3(-1, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,1,1,1,0,1,1":
				//CDEGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,1,1,0,1,1,1":
				//CDFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,1,1,0,0,1,1":
				//BCDGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,1,0,0,1,1":
				//ACDGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,0,1,1,1,1,1":
				//DEFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				normalsOfMesh[3] = new Vector3(-1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,0,1,0,1,1,1":
				//BDFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,1,1,0,1,0,1":
				//BCDFH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				normalsOfMesh[3] = new Vector3(1, 1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,0,0,0,1":
				//ABCDH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,0,0,1,0":
				//ABCDG
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				normalsOfMesh[3] = new Vector3(1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,1,1,0,1,0":
				//ACDEG
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,0,1,0,1,1":
				//ACEGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				normalsOfMesh[3] = new Vector3(-1, 1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,0,1,0,1,1,1,1":
				//CEFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,0,0,1,1,1,1":
				//AEFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,0,0,1,1,1,1":
				//BEFGH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = G;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				normalsOfMesh[3] = new Vector3(-1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "0,1,0,1,1,1,0,1":
				//BDEFH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,1,0,1,0,1":
				//ABDFH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = H;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				normalsOfMesh[3] = new Vector3(1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,0,1,0,0":
				//ABCDF
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,1,1,0,0,0":
				//ABCDE
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = D;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				normalsOfMesh[3] = new Vector3(1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,0,1,0,1,0":
				//ABCEG
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,0,1,0,1,1,1,0":
				//ACEFG
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = C;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				normalsOfMesh[3] = new Vector3(-1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,0,1,1,1,0":
				//ABEFG
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,0,1,1,0,1":
				//ABEFH
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, -1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,0,1,1,1,0,0":
				//ABDEF
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			case "1,1,1,0,1,1,0,0":
				//ABCEF
				combinationVertices = new Vector3[4];
				normalsOfMesh = new Vector3[4];
				triangles = new int[6];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, 1);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;

				CreateMesh(combinationVertices, normalsOfMesh, triangles, 4, 1);
				break;

			default:
				//Debug.Log("Some other combination: " + combination);
				break;

		}
	}

	private string ConvertToCode(bool trigger)
	{
		if (trigger)
		{
			return "1";
		}
		else
		{
			return "0";
		}
	}

	private void CreateMesh(Vector3[] objects, Vector3[] normalsOfMesh, int[] triangles, int numOfVertices, int combination)
	{
		Vector3[] vertices = new Vector3[numOfVertices];
		Vector2[] uv = new Vector2[numOfVertices];


		GameObject customMesh = new GameObject(combinationName, typeof(MeshFilter));

		for (int i = 0; i < objects.Length; i++)
		{
			vertices[i] = objects[i];
		}

		switch (combination)
		{
			case 1:
				//Two triangles
				uv[0] = new Vector2(0, 1);
				uv[1] = new Vector2(1, 1);
				uv[2] = new Vector2(0, 0);
				uv[3] = new Vector2(1, 0);
				break;

			case 2:
				//One triangle
				uv[0] = new Vector2(0, 1);
				uv[1] = new Vector2(1, 1);
				uv[2] = new Vector2(0, 0);
				break;

		}

		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		if (normalsOfMesh.Length > 0)
		{
			mesh.normals = normalsOfMesh;
		}

		customMesh.GetComponent<MeshFilter>().mesh = mesh;

		CombineInstance ci = new CombineInstance
		{
			mesh = customMesh.GetComponent<MeshFilter>().sharedMesh,
			transform = customMesh.GetComponent<MeshFilter>().transform.localToWorldMatrix
		};
		blockData.Add(ci);

		Destroy(customMesh);
	}

	private void CreatePlanet()
	{
		List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
		int vertexCount = 0;
		blockDataLists.Add(new List<CombineInstance>());

		for (int i = 0; i < blockData.Count; i++)
		{
			vertexCount += blockData[i].mesh.vertexCount;
			if (vertexCount >= 65535)
			{
				vertexCount = 0;
				blockDataLists.Add(new List<CombineInstance>());
				i--;
			}
			else
			{
				blockDataLists.Last().Add(blockData[i]);
			}
		}

		container = new GameObject("Planet");
		foreach (List<CombineInstance> data in blockDataLists)
		{
			GameObject planetMesh = new GameObject("Mesh");
			planetMesh.transform.parent = container.transform;
			MeshFilter meshFilter = planetMesh.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = planetMesh.AddComponent<MeshRenderer>();
			meshRenderer.material = meshTexture;
			meshFilter.mesh.CombineMeshes(data.ToArray());
			meshes.Add(meshFilter.mesh);
			//g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh; //setting colliders takes more time. disabled for testing.
		}
	}
}
