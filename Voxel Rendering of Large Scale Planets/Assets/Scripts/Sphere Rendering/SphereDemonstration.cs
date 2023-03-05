using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDemonstration : MonoBehaviour
{
	[SerializeField] GameObject voxelPrefab;
	[SerializeField] Material newMaterial;
	[SerializeField] Material secondMat;
	[SerializeField] Material meshTexture;
	[SerializeField] int sizeOfPlanet;
	[SerializeField] private float noiseScale = 0.05f;
	[SerializeField, Range(0, 1)] private float threshold = 0.5f;

	private IDictionary<string, GameObject> vertices = new Dictionary<string, GameObject>();

	GameObject A;
	GameObject B;
	GameObject C;
	GameObject D;
	GameObject E;
	GameObject F;
	GameObject G;
	GameObject H;

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
			StartCoroutine(MarchCube());
		}

		//Test: Draws lines between points on the marching cube.
		if (A != null && B != null && C != null && D != null && E != null && F != null && G != null && H != null)
		{
			Debug.DrawLine(A.transform.position, B.transform.position, Color.blue);
			Debug.DrawLine(A.transform.position, C.transform.position, Color.blue);
			Debug.DrawLine(A.transform.position, E.transform.position, Color.blue);
			Debug.DrawLine(F.transform.position, B.transform.position, Color.blue);
			Debug.DrawLine(F.transform.position, E.transform.position, Color.blue);
			Debug.DrawLine(F.transform.position, H.transform.position, Color.blue);
			Debug.DrawLine(D.transform.position, B.transform.position, Color.blue);
			Debug.DrawLine(D.transform.position, H.transform.position, Color.blue);
			Debug.DrawLine(D.transform.position, C.transform.position, Color.blue);
			Debug.DrawLine(G.transform.position, E.transform.position, Color.blue);
			Debug.DrawLine(G.transform.position, H.transform.position, Color.blue);
			Debug.DrawLine(G.transform.position, C.transform.position, Color.blue);
		}
	}

	private void CreateSphere()
	{
		Vector3 centre = new Vector3(sizeOfPlanet / 2, sizeOfPlanet / 2, sizeOfPlanet / 2);

		GameObject TestSphere = new GameObject("Generated Sphere");

		for (int x = 0; x < sizeOfPlanet; x++)
		{
			for (int y = 0; y < sizeOfPlanet; y++)
			{
				for (int z = 0; z < sizeOfPlanet; z++)
				{
					Vector3 position = new Vector3(x, y, z);
					float distance = Vector3.Distance(position, centre);
					float noiseValue = PerlinNoise3D(x * noiseScale, y * noiseScale, z * noiseScale);

					if (noiseValue >= threshold && distance < sizeOfPlanet / 2)
					{
						GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
						render.transform.SetParent(TestSphere.transform);
						render.GetComponent<PointChanger>().SetTriggerPoint(true);
						render.GetComponent<MeshRenderer>().material = newMaterial;

						string key = x + "," + y + "," + z;
						vertices.Add(key, render);
					}
					else
					{
						GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
						render.transform.SetParent(TestSphere.transform);
						render.GetComponent<PointChanger>().SetTriggerPoint(false);
						render.GetComponent<MeshRenderer>().material = secondMat;

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

	IEnumerator MarchCube()
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

					A = vertices[AKey];
					B = vertices[BKey];
					C = vertices[CKey];
					D = vertices[DKey];
					E = vertices[EKey];
					F = vertices[FKey];
					G = vertices[GKey];
					H = vertices[HKey];

					//Debug.DrawLine(A.transform.position, B.transform.position, Color.blue);
					//Debug.DrawLine(A.transform.position, C.transform.position, Color.blue);
					//Debug.DrawLine(A.transform.position, E.transform.position, Color.blue);
					//Debug.DrawLine(F.transform.position, B.transform.position, Color.blue);
					//Debug.DrawLine(F.transform.position, E.transform.position, Color.blue);
					//Debug.DrawLine(F.transform.position, H.transform.position, Color.blue);
					//Debug.DrawLine(D.transform.position, B.transform.position, Color.blue);
					//Debug.DrawLine(D.transform.position, H.transform.position, Color.blue);
					//Debug.DrawLine(D.transform.position, C.transform.position, Color.blue);
					//Debug.DrawLine(G.transform.position, E.transform.position, Color.blue);
					//Debug.DrawLine(G.transform.position, H.transform.position, Color.blue);
					//Debug.DrawLine(G.transform.position, C.transform.position, Color.blue);

					string combination =
						ConvertToCode(A.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(B.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(C.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(D.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(E.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(F.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(G.GetComponent<PointChanger>().GetTriggerPoint()) + "," +
						ConvertToCode(H.GetComponent<PointChanger>().GetTriggerPoint());

					MeshCombination(combination);

					yield return new WaitForSeconds(.1f);
				}
			}
		}
	}

	private void MeshCombination(string combination)
	{
		switch (combination)
		{
			case "1,1,1,1,0,0,0,0":
				Debug.Log("A,B,C,D");
				break;

			case "0,1,0,1,0,1,0,1":
				Debug.Log("B,F,D,H");
				break;

			case "0,0,0,0,1,1,1,1":
				Debug.Log("E,F,G,H");
				break;

			case "1,0,1,0,1,0,1,0":
				Debug.Log("A,C,E,G");
				break;

			case "1,1,0,0,0,0,1,1":
				Debug.Log("A,B,G,H aka slant with no way to tell direction");
				break;

			case "0,1,1,0,0,1,1,0":
				Debug.Log("B,C,F,G aka slant with no way to tell direction");
				break;

			case "0,0,1,1,1,1,0,0":
				Debug.Log("C,D,E,F aka slant with no way to tell direction");
				break;

			case "1,0,0,1,1,0,0,1":
				Debug.Log("A,D,E,H aka slant with no way to tell direction");
				break;

			case "1,1,1,1,0,0,1,1":
				Debug.Log("A,B,C,D,G,H aka slant facing upwards");
				break;

			case "1,1,0,0,1,1,1,1":
				Debug.Log("A,B,E,F,G,H aka slant facing downwards");
				break;

			default:
				Debug.Log("Some other combination");
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
}
