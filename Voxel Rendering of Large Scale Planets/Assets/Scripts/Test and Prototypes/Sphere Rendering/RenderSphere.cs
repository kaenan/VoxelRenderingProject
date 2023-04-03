using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSphere : MonoBehaviour
{
	[SerializeField] GameObject voxelPrefab;
	//[SerializeField] Material newMaterial;
	//[SerializeField] Material secondMat;
	[SerializeField] Material meshTexture;
	[SerializeField] int sizeOfPlanet;
	[SerializeField] private float noiseScale = 0.05f;
	[SerializeField, Range(0, 1)] private float threshold = 0.5f;

	private IDictionary<string, GameObject> vertices = new Dictionary<string, GameObject>();
	private GameObject TestSphere;

	string combinationName;

	void Start()
	{
		CreateSphere();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			Debug.Log("Deleting Sphere...");
			Destroy(GameObject.Find("Generated Sphere"));
			vertices.Clear();
			CreateSphere();
			Debug.Log("New Sphere Generated.");
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			MarchCube();
		}
	}

	private void CreateSphere()
	{
		Vector3 centre = new Vector3(sizeOfPlanet / 2, sizeOfPlanet / 2, sizeOfPlanet / 2);

		TestSphere = new GameObject("Generated Sphere");

		for (int x = 0; x < sizeOfPlanet; x++)
		{
			for (int y = 0; y < sizeOfPlanet; y++)
			{
				for (int z = 0; z < sizeOfPlanet; z++)
				{
					Vector3 position = new Vector3(x, y, z);
					float distance = Vector3.Distance(position, centre);
					float noiseValue = PerlinNoise3D(x * noiseScale, y * noiseScale, z * noiseScale);

					if (noiseValue >= threshold && distance < (sizeOfPlanet / 2) - 1)
					{
						GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
						render.transform.SetParent(TestSphere.transform);
						render.GetComponent<PointChanger>().SetTriggerPoint(true);
						//render.GetComponent<MeshRenderer>().material = newMaterial;

						string key = x + "," + y + "," + z;
						vertices.Add(key, render);
					}
					else
					{
						GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
						render.transform.SetParent(TestSphere.transform);
						render.GetComponent<PointChanger>().SetTriggerPoint(false);
						//render.GetComponent<MeshRenderer>().material = secondMat;
					
						string key = x + "," + y + "," + z;
						vertices.Add(key, render);

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
		for (int x = 0; x < sizeOfPlanet - 1; x++)
		{
			for (int y = 1; y < sizeOfPlanet; y++)
			{
				for (int z = 1; z < sizeOfPlanet; z++)
				{
					string AKey = x + "," + y + "," + z;
					string BKey = x + "," + y + "," + (z - 1);
					string CKey = x + "," + (y - 1) + "," + z;
					string DKey = x + "," + (y - 1) + "," + (z - 1);
					string EKey = x + 1 + "," + y + "," + z;
					string FKey = x + 1 + "," + y + "," + (z - 1) ;
					string GKey = x + 1 + "," + (y - 1) + "," + z;
					string HKey = x + 1 + "," + (y - 1) + "," + (z - 1);

					GameObject A = vertices[AKey];
					GameObject B = vertices[BKey];
					GameObject C = vertices[CKey];
					GameObject D = vertices[DKey];
					GameObject E = vertices[EKey];
					GameObject F = vertices[FKey];
					GameObject G = vertices[GKey];
					GameObject H = vertices[HKey];

					string combination =
						ConvertToCode(A.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(B.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(C.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(D.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(E.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(F.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(G.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(H.GetComponent<PointChanger>().GetTriggerPoint());

					MeshCombination(A, B, C, D, E, F, G, H, combination);
				}
			}
		}
	}

	private void MeshCombination(GameObject A, GameObject B, GameObject C, GameObject D, GameObject E, GameObject F, GameObject G, GameObject H, string combination)
	{
		GameObject[] combinationVertices;
		Vector3[] normalsOfMesh;
		combinationName = combination;
		switch (combination)
		{
			//
			//ONE TRIANGLE COMBINATIONS
			//

			case "1,1,1,0,0,0,0,0":
				//A,B,C
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,0,1,0,0,0,0":
				//A,B,D
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,1,1,0,0,0,0":
				//A,C,D
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,1,1,0,0,0,0":
				//B,C,D
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = B;
				combinationVertices[1] = C;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,1,0,0,0,1,0":
				//A,C,G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,0,0,1,0,1,0":
				//A,E,G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,1,0,1,0,0,0":
				//A,C,E
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,1,0,1,0,1,0":
				//C,E,G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,0,1,0,1,1":
				//E,G,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = G;
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,0,1,1,0,1":
				//E,F,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = H;
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,0,1,1,1,0":
				//E,F,G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,0,0,1,1,1":
				//F,G,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				combinationVertices[2] = H;
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,0,1,0,1,0,0":
				//B,D,F
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,0,1,0,0,0,1":
				//B,D,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,0,0,0,1,0,1":
				//B,F,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,1,0,1,0,1":
				//D,F,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = F;
				combinationVertices[1] = D;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,0,0,1,0,0,0":
				//A,B,E
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,0,0,0,1,0,0":
				//A,B,F
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,0,0,1,1,0,0":
				//A,E,F
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = E;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,0,0,1,1,0,0":
				//B,E,F
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,1,1,0,0,1,0":
				//C,D,G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = D;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,1,1,0,0,0,1":
				//C,D,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = C;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,1,0,0,0,1,1":
				//C,G,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,1,0,0,1,1":
				//D,G,H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,1,1,1,1,1,1":
				//Everything except A
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,1,1,1,1,1,1":
				//Everything except B
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,0,1,1,1,1,1":
				//Everything except C
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,0,1,1,1,1":
				//Everything except D
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = C;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,1,0,1,1,1":
				//Everything except E
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,1,1,0,1,1":
				//Everything except F
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,1,1,1,0,1":
				//Everything except G
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = H;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,1,1,1,1,0":
				//Everything except H
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = D;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,0,1,0,1,1,1":
				//DFGH
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,1,1,0,0,1,0":
				//ACDG
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,0,1,0,1,0,1,1":
				//CEGH
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,1,1,0,0,0,1":
				//BCDH
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,0,1,0,1,0,0":
				//ABDF
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,0,0,0,1,1,1,0":
				//AEFG
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "1,1,1,0,1,0,0,0":
				//ABCE
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			case "0,1,0,0,1,1,0,1":
				//BEFH
				combinationVertices = new GameObject[3];
				normalsOfMesh = new Vector3[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 3, 3, 2);
				break;

			 ///////////////////////////////
			//                           //
		   // TWO TRIANGLE COMBINATIONS //
		  //                           //
		 ///////////////////////////////

			case "1,1,1,1,0,0,0,0":
				//A,B,C,D Front of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 0, 0);
				normalsOfMesh[2] = new Vector3(1, 0, 0);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,0,1,0,1,0,1":
				//B,F,D,H Left of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(0, 0, 1);
				normalsOfMesh[2] = new Vector3(0, 0, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,0,0,1,1,1,1":
				//E,F,G,H Back of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 0, 0);
				normalsOfMesh[2] = new Vector3(-1, 0, 0);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,0,1,0,1,0":
				//A,C,E,G Left of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(0, 0, -1);
				normalsOfMesh[2] = new Vector3(0, 0, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,0,1,1,0,0":
				//Top of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(0, -1, 0);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(0, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,1,1,0,0,1,1":
				//Bottom of the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, 1, 0);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(0, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,0,0,1,1":
				//A,B,C,D,G,H aka slant facing upwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 1, 0);
				normalsOfMesh[1] = new Vector3(1, 1, 0);
				normalsOfMesh[2] = new Vector3(1, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,0,1,1,1,1":
				//A,B,E,F,G,H aka slant facing downwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, 0);
				normalsOfMesh[1] = new Vector3(-1, -1, 0);
				normalsOfMesh[2] = new Vector3(-1, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,1,1,1,1,1,1":
				//C,D,E,F,G,H aka slant facing upwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, 1, 0);
				normalsOfMesh[1] = new Vector3(-1, 1, 0);
				normalsOfMesh[2] = new Vector3(-1, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,1,1,0,0":
				//A,B,C,D,E,F aka slant facing downwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, -1, 0);
				normalsOfMesh[1] = new Vector3(1, -1, 0);
				normalsOfMesh[2] = new Vector3(1, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,1,1,0,1,1":
				//A,C,D,E,G,H aka slant facing upwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(0, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, -1);
				normalsOfMesh[2] = new Vector3(0, 1, -1);
				normalsOfMesh[3] = new Vector3(0, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,1,1,1,0,1":
				//A,B,D,E,F,H aka slant facing downwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(0, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 1);
				normalsOfMesh[2] = new Vector3(0, -1, 1);
				normalsOfMesh[3] = new Vector3(0, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,1,1,0,1,1,1":
				//B,C,D,F,G,H aka slant facing upwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(0, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 1);
				normalsOfMesh[2] = new Vector3(0, 1, 1);
				normalsOfMesh[3] = new Vector3(0, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,0,1,1,1,0":
				//ABCEFG aka slant facing downwards
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(0, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, -1);
				normalsOfMesh[2] = new Vector3(0, -1, -1);
				normalsOfMesh[3] = new Vector3(0, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,0,1,0,1":
				//A,B,C,D,F,H face going across the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, 0, 1);
				normalsOfMesh[1] = new Vector3(1, 0, 1);
				normalsOfMesh[2] = new Vector3(1, 0, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,0,1,1,1,1":
				//ACEFGH face going across the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 0, 1);
				normalsOfMesh[1] = new Vector3(-1, 0, 1);
				normalsOfMesh[2] = new Vector3(-1, 0, 1);
				normalsOfMesh[3] = new Vector3(-1, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,1,0,1,0":
				//A,B,C,D,E,G face going across the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 0, -1);
				normalsOfMesh[1] = new Vector3(1, 0, -1);
				normalsOfMesh[2] = new Vector3(1, 0, -1);
				normalsOfMesh[3] = new Vector3(1, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,0,1,1,1,1,1":
				//BDEFGH face going across the cube
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, 0, 1);
				normalsOfMesh[1] = new Vector3(-1, 0, 1);
				normalsOfMesh[2] = new Vector3(-1, 0, 1);
				normalsOfMesh[3] = new Vector3(-1, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,1,1,1,0,1,1":
				//CDEGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,1,1,0,1,1,1":
				//CDFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,1,1,0,0,1,1":
				//BCDGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(-1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,1,0,0,1,1":
				//ACDGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(0, 1, 0);
				normalsOfMesh[2] = new Vector3(0, 1, 0);
				normalsOfMesh[3] = new Vector3(1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,0,1,1,1,1,1":
				//DEFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				normalsOfMesh[3] = new Vector3(-1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,0,1,0,1,1,1":
				//BDFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(-1, 1, 1);
				normalsOfMesh[1] = new Vector3(-1, 1, 1);
				normalsOfMesh[2] = new Vector3(-1, 1, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,1,1,0,1,0,1":
				//BCDFH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				normalsOfMesh[3] = new Vector3(1, 1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,0,0,0,1":
				//ABCDH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(1, 1, 1);
				normalsOfMesh[1] = new Vector3(1, 1, 1);
				normalsOfMesh[2] = new Vector3(1, 1, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,0,0,1,0":
				//ABCDG
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				normalsOfMesh[3] = new Vector3(1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,1,1,0,1,0":
				//ACDEG
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(1, 1, -1);
				normalsOfMesh[1] = new Vector3(1, 1, -1);
				normalsOfMesh[2] = new Vector3(1, 1, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,0,1,0,1,1":
				//ACEGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				normalsOfMesh[3] = new Vector3(-1, 1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,0,1,0,1,1,1,1":
				//CEFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(-1, 1, -1);
				normalsOfMesh[1] = new Vector3(-1, 1, -1);
				normalsOfMesh[2] = new Vector3(-1, 1, -1);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,0,0,1,1,1,1":
				//AEFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				combinationVertices[3] = H;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				normalsOfMesh[3] = new Vector3(-1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,0,0,1,1,1,1":
				//BEFGH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = G;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(-1, 0, 0);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				normalsOfMesh[3] = new Vector3(-1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "0,1,0,1,1,1,0,1":
				//BDEFH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				combinationVertices[3] = D;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(-1, -1, 1);
				normalsOfMesh[2] = new Vector3(-1, -1, 1);
				normalsOfMesh[3] = new Vector3(0, 0, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,1,0,1,0,1":
				//ABDFH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = H;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(0, 0, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				normalsOfMesh[3] = new Vector3(1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,0,1,0,0":
				//ABCDF
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				combinationVertices[3] = C;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(1, -1, 1);
				normalsOfMesh[2] = new Vector3(1, -1, 1);
				normalsOfMesh[3] = new Vector3(1, 0, 0);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,1,1,0,0,0":
				//ABCDE
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = D;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(1, 0, 0);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				normalsOfMesh[3] = new Vector3(1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,0,1,0,1,0":
				//ABCEG
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				combinationVertices[3] = G;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(1, -1, -1);
				normalsOfMesh[2] = new Vector3(1, -1, -1);
				normalsOfMesh[3] = new Vector3(0, 0, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,0,1,0,1,1,1,0":
				//ACEFG
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = C;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(0, 0, -1);
				normalsOfMesh[1] = new Vector3(-1, -1, -1);
				normalsOfMesh[2] = new Vector3(-1, -1, -1);
				normalsOfMesh[3] = new Vector3(-1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,0,1,1,1,0":
				//ABEFG
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				combinationVertices[3] = F;
				normalsOfMesh[0] = new Vector3(-1, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,0,1,1,0,1":
				//ABEFH
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				combinationVertices[3] = B;
				normalsOfMesh[0] = new Vector3(1, -1, 1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, -1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,0,1,1,1,0,0":
				//ABDEF
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				combinationVertices[3] = A;
				normalsOfMesh[0] = new Vector3(1, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(-1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			case "1,1,1,0,1,1,0,0":
				//ABCEF
				combinationVertices = new GameObject[4];
				normalsOfMesh = new Vector3[4];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				combinationVertices[3] = E;
				normalsOfMesh[0] = new Vector3(-1, -1, -1);
				normalsOfMesh[1] = new Vector3(0, -1, 0);
				normalsOfMesh[2] = new Vector3(0, -1, 0);
				normalsOfMesh[3] = new Vector3(1, -1, 1);
				CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
				break;

			//case "0,1,1,1,0,0,1,0":
			//	//BCDG
			//	combinationVertices = new GameObject[4];
			//	normalsOfMesh = new Vector3[4];
			//	combinationVertices[0] = C;
			//	combinationVertices[1] = B;
			//	combinationVertices[2] = G;
			//	combinationVertices[3] = D;
			//	normalsOfMesh[0] = new Vector3(-1, 1, 1);
			//	normalsOfMesh[1] = new Vector3(0, 1, 0);
			//	normalsOfMesh[2] = new Vector3(0, 1, 0);
			//	normalsOfMesh[3] = new Vector3(1, 0, -1);
			//	CreateMesh(combinationVertices, normalsOfMesh, 4, 6, 1);
			//	break;

			default:
				Debug.Log("Some other combination: " + combination);
				break;

		}
	}

	private string ConvertToCode(bool trigger)
	{
		if (trigger)
		{
			return "1";
		} else
		{
			return "0";
		}
	}

	private void CreateMesh(GameObject[] objects, Vector3[] normalsOfMesh, int numOfVertices, int verticesPerTriangle, int combination)
	{
		Vector3[] vertices = new Vector3[numOfVertices];
		Vector2[] uv = new Vector2[numOfVertices];
		int[] triangles = new int[verticesPerTriangle];

		GameObject gameobject = new GameObject(combinationName, typeof(MeshFilter), typeof(MeshRenderer));
		gameobject.transform.SetParent(TestSphere.transform);

		for (int i = 0; i < objects.Length; i++)
		{
			vertices[i] = objects[i].transform.position;
		}

		switch (combination)
		{
			case 1:
				//Two triangles
				uv[0] = new Vector2(0, 1);
				uv[1] = new Vector2(1, 1);
				uv[2] = new Vector2(0, 0);
				uv[3] = new Vector2(1, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;
				break;

			case 2:
				//One triangle
				uv[0] = new Vector2(0, 1);
				uv[1] = new Vector2(1, 1);
				uv[2] = new Vector2(0, 0);
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				break;

		}

		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		if(normalsOfMesh.Length > 0)
		{
			mesh.normals = normalsOfMesh;
		}

		gameobject.GetComponent<MeshFilter>().mesh = mesh;
		gameobject.GetComponent<MeshRenderer>().material = meshTexture;

		Debug.Log("Created Custom Mesh");
	}
}
