using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NoiseTest;

/// <summary>
/// This version is using the code from the visuliser and not phyiscal points for the vertices.
/// 
/// Changes to the marching cubes algorithm to use a triangulation table.
/// 
/// </summary>


public class MCVersion5 : MonoBehaviour
{
	[Header("Noise Parameters")]
	public float pointGap = 1;
	[Range(0, 100)] public int mapSize = 20;
	[Range(0, 1)] public float scale = 0.5f;
	[Range(1, 10)] public float amplitude = 1f;
	public Vector3 offset = new Vector3(0, 0, 0);

	[SerializeField] Material meshTexture;

	private int sizeOfPlanet;
	private Vector3 centre;
	private OpenSimplexNoise simplexNoise = new OpenSimplexNoise();

	private GameObject container;
	private List<CombineInstance> blockData = new List<CombineInstance>();
	private List<Mesh> meshes = new List<Mesh>();
	private IDictionary<string, ArrayList> vertices = new Dictionary<string, ArrayList>();

	private string combinationName;

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
	}

	private void CreateSphere()
	{
		centre = new Vector3(mapSize / 2, mapSize / 2, mapSize / 2);
		sizeOfPlanet = mapSize / 3;
		for (float x = 0; x < mapSize; x+= pointGap)
		{
			for (float y = 0; y < mapSize; y+= pointGap)
			{
				for (float z = 0; z < mapSize; z+= pointGap)
				{
					Vector3 voxelPosition = new Vector3(x, y, z);
					float distance = Vector3.Distance(centre, voxelPosition);
					double xPos = x * scale + offset.x;
					double yPos = y * scale + offset.y;
					double zPos = z * scale + offset.z;
					distance = distance + (float)simplexNoise.Evaluate(xPos, yPos, zPos) * amplitude;

					ArrayList voxelInfo;
					string key;
					if (distance < sizeOfPlanet)
					{
						key = x + "," + y + "," + z;
						voxelPosition = new Vector3(x, y, z);
						voxelInfo = new ArrayList();
						voxelInfo.Add(voxelPosition);
						voxelInfo.Add(true);
						voxelInfo.Add((float)simplexNoise.Evaluate(xPos, yPos, zPos));
						vertices.Add(key, voxelInfo);
					}
					else
					{
						key = x + "," + y + "," + z;
						voxelPosition = new Vector3(x, y, z);
						voxelInfo = new ArrayList();
						voxelInfo.Add(voxelPosition);
						voxelInfo.Add(false);
						voxelInfo.Add((float)simplexNoise.Evaluate(xPos, yPos, zPos));
						vertices.Add(key, voxelInfo);
					}
				}
			}
		}
		MarchCube();
	}

	private void MarchCube()
	{
		for (float x = 0; x < mapSize - pointGap; x+= pointGap)
		{
			for (float y = pointGap; y < mapSize; y+= pointGap)
			{
				for (float z = pointGap; z < mapSize; z+= pointGap)
				{
					string AKey = x + "," + y + "," + z;
					string BKey = x + "," + y + "," + (z - pointGap);
					string CKey = x + "," + (y - pointGap) + "," + z;
					string DKey = x + "," + (y - pointGap) + "," + (z - pointGap);
					string EKey = x + pointGap + "," + y + "," + z;
					string FKey = x + pointGap + "," + y + "," + (z - pointGap);
					string GKey = x + pointGap + "," + (y - pointGap) + "," + z;
					string HKey = x + pointGap + "," + (y - pointGap) + "," + (z - pointGap);

					ArrayList A = vertices[AKey];
					ArrayList B = vertices[BKey];
					ArrayList C = vertices[CKey];
					ArrayList D = vertices[DKey];
					ArrayList E = vertices[EKey];
					ArrayList F = vertices[FKey];
					ArrayList G = vertices[GKey];
					ArrayList H = vertices[HKey];

					string combination =
						ConvertToCode((bool)A[1]) + "," +
						ConvertToCode((bool)B[1]) + "," +
						ConvertToCode((bool)C[1]) + "," +
						ConvertToCode((bool)D[1]) + "," +
						ConvertToCode((bool)E[1]) + "," +
						ConvertToCode((bool)F[1]) + "," +
						ConvertToCode((bool)G[1]) + "," +
						ConvertToCode((bool)H[1]);

					MeshCombination(A, B, C, D, E, F, G, H, combination);
				}
			}
		}
		CreatePlanet();
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

	private void MeshCombination(ArrayList A, ArrayList B, ArrayList C, ArrayList D, ArrayList E, ArrayList F, ArrayList G, ArrayList H, string combination)
	{
		ArrayList[] combinationVertices;
		combinationName = combination;
		switch (combination)
		{
			///////////////////////////////////////
			//                                  //
			//    ONE TRIANGLE COMBINATIONS    //
			//                                //
			///////////////////////////////////

			case "1,1,1,0,0,0,0,0":
				//A,B,C
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,0,0,0,0":
				//A,B,D
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,0,0,0,0":
				//A,C,D
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices); ;
				break;

			case "0,1,1,1,0,0,0,0":
				//B,C,D
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = C;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,0,0,1,0":
				//A,C,G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,0,0,0,1,0,1,0":
				//A,E,G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,1,0,0,0":
				//A,C,E
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,0,1,0,1,0":
				//C,E,G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = G;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,0,1,0,1,1":
				//E,G,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = H;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,0,1,1,0,1":
				//E,F,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,0,1,1,1,0":
				//E,F,G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,0,0,1,1,1":
				//F,G,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,0,1,0,0":
				//B,D,F
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,0,0,0,1":
				//B,D,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,0,0,1,0,1":
				//B,F,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,1,0,1,0,1":
				//D,F,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = D;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,1,0,0,0":
				//A,B,E
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,0,1,0,0":
				//A,B,F
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "1,0,0,0,1,1,0,0":
				//A,E,F
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,0,1,1,0,0":
				//B,E,F
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,0,0,1,0":
				//C,D,G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = D;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,0,0,0,1":
				//C,D,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,0,0,0,1,1":
				//C,G,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,1,0,0,1,1":
				//D,G,H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,1,1,1,1,1,1":
				//Everything except A
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,1,1,1,1":
				//Everything except B
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,1,1,1,1":
				//Everything except C
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,0,1,1,1,1":
				//Everything except D
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,1,1,1":
				//Everything except E
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,0,1,1":
				//Everything except F
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,1,0,1":
				//Everything except G
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,1,1,0":
				//Everything except H
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,1,0,1,1,1":
				//DFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,0,0,1,0":
				//ACDG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,0,1,0,1,1":
				//CEGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,1,1,1,0,0,0,1":
				//BCDH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,0,1,0,0":
				//ABDF
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,0,0,0,1,1,1,0":
				//AEFG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,0,1,0,0,0":
				//ABCE
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,0,1,1,0,1":
				//BEFH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			///////////////////////////////
			//                           //
			// TWO TRIANGLE COMBINATIONS //
			//                           //
			///////////////////////////////

			case "1,1,1,1,0,0,0,0":
				//A,B,C,D Front of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,0,1,0,1":
				//B,F,D,H Left of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,0,1,1,1,1":
				//E,F,G,H Back of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,1,0,1,0":
				//A,C,E,G Left of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,1,1,0,0":
				//ABEF Top of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,0,0,1,1":
				//Bottom of the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = H;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = H;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,0,1,1":
				//A,B,C,D,G,H aka slant facing upwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,1,1,1,1":
				//A,B,E,F,G,H aka slant facing downwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,1,1,1,1":
				//C,D,E,F,G,H aka slant facing upwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,1,0,0":
				//A,B,C,D,E,F aka slant facing downwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,1,0,1,1":
				//A,C,D,E,G,H aka slant facing upwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,1,1,0,1":
				//A,B,D,E,F,H aka slant facing downwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,1,1,0,1,1,1":
				//B,C,D,F,G,H aka slant facing upwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,0,1,1,1,0":
				//ABCEFG aka slant facing downwards
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,1,0,1":
				//A,B,C,D,F,H face going across the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,1,1,1,1":
				//ACEFGH face going across the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = F;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,0,1,0":
				//A,B,C,D,E,G face going across the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,1,1,1,1":
				//BDEFGH face going across the cube
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,1,0,1,1":
				//CDEGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,1,0,1,1,1":
				//CDFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,1,1,0,0,1,1":
				//BCDGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,0,0,1,1":
				//ACDGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "0,0,0,1,1,1,1,1":
				//DEFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,0,1,1,1":
				//BDFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = F;
				combinationVertices[2] = B;
				CreateMesh(combinationVertices);
				break;

			case "0,1,1,1,0,1,0,1":
				//BCDFH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,0,0,1":
				//ABCDH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = B;
				combinationVertices[2] = A;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,0,1,0":
				//ABCDG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,1,1,0,1,0":
				//ACDEG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = A;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,1,0,1,1":
				//ACEGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,0,1,0,1,1,1,1":
				//CEFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = E;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "1,0,0,0,1,1,1,1":
				//AEFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = F;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = F;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,0,1,1,1,1":
				//BEFGH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = E;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = E;
				combinationVertices[2] = B;
				CreateMesh(combinationVertices);
				break;

			case "0,1,0,1,1,1,0,1":
				//BDEFH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = B;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = B;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,0,1,0,1":
				//ABDFH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = F;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = F;
				combinationVertices[2] = A;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,0,1,0,0":
				//ABCDF
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = A;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = A;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,1,1,0,0,0":
				//ABCDE
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = B;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = B;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,0,1,0,1,0":
				//ABCEG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = E;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = E;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				break;

			case "1,0,1,0,1,1,1,0":
				//ACEFG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = A;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = A;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,1,1,1,0":
				//ABEFG
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = A;
				combinationVertices[1] = B;
				combinationVertices[2] = G;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = G;
				combinationVertices[1] = B;
				combinationVertices[2] = F;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,0,1,1,0,1":
				//ABEFH
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = E;
				combinationVertices[1] = A;
				combinationVertices[2] = H;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = H;
				combinationVertices[1] = A;
				combinationVertices[2] = B;
				CreateMesh(combinationVertices);
				break;

			case "1,1,0,1,1,1,0,0":
				//ABDEF
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = F;
				combinationVertices[1] = E;
				combinationVertices[2] = D;
				CreateMesh(combinationVertices);
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = D;
				combinationVertices[1] = E;
				combinationVertices[2] = A;
				CreateMesh(combinationVertices);
				break;

			case "1,1,1,0,1,1,0,0":
				//ABCEF
				combinationVertices = new ArrayList[3];
				combinationVertices[0] = B;
				combinationVertices[1] = F;
				combinationVertices[2] = C;
				CreateMesh(combinationVertices);

				combinationVertices = new ArrayList[3];
				combinationVertices[0] = C;
				combinationVertices[1] = F;
				combinationVertices[2] = E;
				CreateMesh(combinationVertices);
				break;

			default:
				//if(combination != "1,1,1,1,1,1,1,1" && 
				//	combination != "0,0,0,0,0,0,0,0" &&
				//	combination != "1,0,0,0,0,0,0,0" &&
				//	combination != "0,1,0,0,0,0,0,0" &&
				//	combination != "0,0,1,0,0,0,0,0" &&
				//	combination != "0,0,0,1,0,0,0,0" &&
				//	combination != "0,0,0,0,1,0,0,0" &&
				//	combination != "0,0,0,0,0,1,0,0" &&
				//	combination != "0,0,0,0,0,0,1,0" &&
				//	combination != "0,0,0,0,0,0,0,1" &&

				//	combination != "1,1,0,0,0,0,0,0" &&
				//	combination != "1,0,1,0,0,0,0,0" &&
				//	combination != "1,0,0,1,0,0,0,0" &&
				//	combination != "1,0,0,0,1,0,0,0" &&
				//	combination != "1,0,0,0,0,1,0,0" &&
				//	combination != "1,0,0,0,0,0,1,0" &&
				//	combination != "1,0,0,0,0,0,0,1" &&

				//	combination != "0,1,1,0,0,0,0,0" &&
				//	combination != "0,1,0,1,0,0,0,0" &&
				//	combination != "0,1,0,0,1,0,0,0" &&
				//	combination != "0,1,0,0,0,1,0,0" &&
				//	combination != "0,1,0,0,0,0,1,0" &&
				//	combination != "0,1,0,0,0,0,0,1" &&

				//	combination != "0,0,1,1,0,0,0,0" &&
				//	combination != "0,0,1,0,1,0,0,0" &&
				//	combination != "0,0,1,0,0,1,0,0" &&
				//	combination != "0,0,1,0,0,0,1,0" &&
				//	combination != "0,0,1,0,0,0,0,1" &&

				//	combination != "0,0,0,1,1,0,0,0" &&
				//	combination != "0,0,0,1,0,1,0,0" &&
				//	combination != "0,0,0,1,0,0,1,0" &&
				//	combination != "0,0,0,1,0,0,0,1" &&

				//	combination != "0,0,0,0,1,1,0,0" &&
				//	combination != "0,0,0,0,1,0,1,0" &&
				//	combination != "0,0,0,0,1,0,0,1" &&

				//	combination != "0,0,0,0,0,1,1,0" &&
				//	combination != "0,0,0,0,0,1,0,1" &&

				//	combination != "0,0,0,0,0,0,1,1")
				//{
				//	Debug.Log("Some other combination: " + combination);
				//}
				break;

		}
	}

	private void CreateMesh(ArrayList[] objects)
	{
		Vector3[] vertices = new Vector3[3];
		Vector3[] normals = new Vector3[3];
		Vector2[] uv = new Vector2[3];
		int[] triangles = new int[3];

		GameObject customMesh = new GameObject(combinationName, typeof(MeshFilter));

		vertices[0] = new Vector3(1, 0, 0);
		vertices[1] = new Vector3(0, 0, 0);
		vertices[2] = new Vector3(0, 0, 1);
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		uv[0] = new Vector2(0, 1);
		uv[1] = new Vector2(1, 1);
		uv[2] = new Vector2(0, 0);
		normals[0] = new Vector3(0, 1, 0);
		normals[1] = new Vector3(0, 1, 0);
		normals[2] = new Vector3(0, 1, 0);


		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		vertices[0] = (Vector3)objects[0][0];
		vertices[1] = (Vector3)objects[1][0];
		vertices[2] = (Vector3)objects[2][0];
		mesh.vertices = vertices;
		mesh.RecalculateNormals();

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
			if (vertexCount >= 500)
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