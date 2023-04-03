using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.XR;

public class SphereWithChunks : MonoBehaviour
{
	[Header("Planet Settings")]
	public int planetSize = 10;
	public int chunkSize = 10;
	public Material meshTexture;
    public GameObject voxelPrefab;

    private int borderSize;

	[Header("Terrain")]
	[Range(0, 10)] public int amplitude;
    [Range(0, 1)] public float scale;
	public Vector3 offset;

    private int numOfChunks;

	//public int testValue;
	//public int returnVal;
	//public ComputeShader chunkVoxelCreater;
	//ComputeBuffer chunkBuffer;
	//ComputeBuffer valueBuffer;

	private GameObject container;
	private Vector3 centre;

    private string combinationName;
    private List<Chunk> chunks = new List<Chunk>();
    private List<CombineInstance> tempBlockData = new List<CombineInstance>();

    private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (GameObject.Find("Planet")) Destroy(GameObject.Find("Planet"));
            container = new GameObject("Planet");
            tempBlockData.Clear();
            chunks.Clear();
			CreatePoints();
			//bufferTest();
		}
	}

	struct Chunk
	{
		public Vector3Int startingPosition;
		public IDictionary<string, ArrayList> vertices;
        public List<Mesh> meshes;
        public List<CombineInstance> blockData;
    }

	private void CreatePoints()
	{
		centre = new Vector3(planetSize / 2, planetSize/ 2, planetSize / 2);
		numOfChunks = CalculateNumberOfChunks(planetSize, chunkSize);

		for (int x = 0; x < numOfChunks; x++)
		{
			for (int y = 0; y < numOfChunks; y++)
			{
				for (int z = 0; z < numOfChunks; z++)
				{
					//Position of chunk
					Vector3Int position = new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize);

					//Create chunk struct and add to list
					Chunk chunk = new Chunk();
					chunk.startingPosition = position;
					chunks.Add(chunk);
				}
			}
		}
		ChunkRender();
	}

	private int CalculateNumberOfChunks(int planetSize, int chunkSize)
	{
		float a = (float)planetSize / chunkSize;
		int b = (int)Mathf.Ceil(planetSize / chunkSize);
		if (a > b) b += 1;
		if (b < 1) b = 1;
		return b;
	}

	private void ChunkRender()
	{
		OpenSimplexNoise simplexNoise = new OpenSimplexNoise();
		List<Chunk> tempChunk = new List<Chunk>();

        foreach (Chunk chunk in chunks)
		{
            IDictionary<string, ArrayList> vertices = new Dictionary<string, ArrayList>();
            for (int x = chunk.startingPosition.x; x < chunk.startingPosition.x + chunkSize + 1; x++)
			{
				for (int y = chunk.startingPosition.y; y < chunk.startingPosition.y + chunkSize + 1; y++)
				{
					for (int z = chunk.startingPosition.z; z < chunk.startingPosition.z + chunkSize + 1; z++)
					{
						Vector3 voxelPosition = new Vector3(x, y, z);
                        double xPos = x * scale + offset.x;
                        double yPos = y * scale + offset.y;
                        double zPos = z * scale + offset.z;
                        float distance = Vector3.Distance(voxelPosition, centre) + (float) simplexNoise.Evaluate(xPos, yPos, zPos) * amplitude;

                        ArrayList voxelInfo;
						string key;

                        if (distance < (planetSize / 2) - 1)
                        {
                            key = x + "," + y + "," + z;
                            voxelInfo = new ArrayList();
                            voxelInfo.Add(voxelPosition);
                            voxelInfo.Add(true);
                            voxelInfo.Add((float)simplexNoise.Evaluate(xPos, yPos, zPos));
                            vertices.Add(key, voxelInfo);
                        }
                        else
                        {
                            key = x + "," + y + "," + z;
                            voxelInfo = new ArrayList();
                            voxelInfo.Add(voxelPosition);
                            voxelInfo.Add(false);
                            voxelInfo.Add((float)simplexNoise.Evaluate(xPos, yPos, zPos));
                            vertices.Add(key, voxelInfo);
                        }
                    }
				}
			}
			Chunk newChunk = new Chunk();
			newChunk.startingPosition = chunk.startingPosition;
			newChunk.vertices = vertices;
			tempChunk.Add(newChunk);
		}
		chunks = tempChunk;
        MarchCube();
	}

    private void MarchCube()
    {
        List<Chunk> tempChunk = new List<Chunk>();
        foreach (Chunk chunk in chunks)
		{    
            tempBlockData = new List<CombineInstance>();
            for (float x = chunk.startingPosition.x; x < chunk.startingPosition.x + chunkSize; x++)
			{
				for (float y = chunk.startingPosition.y + 1; y < chunk.startingPosition.y + chunkSize + 1; y++)
				{
					for (float z = chunk.startingPosition.z + 1; z < chunk.startingPosition.z + chunkSize + 1; z++)
					{
						string AKey = x + "," + y + "," + z;
						string BKey = x + "," + y + "," + (z - 1);
						string CKey = x + "," + (y - 1) + "," + z;
						string DKey = x + "," + (y - 1) + "," + (z - 1);
						string EKey = x + 1 + "," + y + "," + z;
						string FKey = x + 1 + "," + y + "," + (z - 1);
						string GKey = x + 1 + "," + (y - 1) + "," + z;
						string HKey = x + 1 + "," + (y - 1) + "," + (z - 1);

						ArrayList A = chunk.vertices[AKey];
						ArrayList B = chunk.vertices[BKey];
						ArrayList C = chunk.vertices[CKey];
						ArrayList D = chunk.vertices[DKey];
						ArrayList E = chunk.vertices[EKey];
						ArrayList F = chunk.vertices[FKey];
						ArrayList G = chunk.vertices[GKey];
						ArrayList H = chunk.vertices[HKey];

						string combination =
							ConvertToCode((bool)A[1]) + "," +
							ConvertToCode((bool)B[1]) + "," +
							ConvertToCode((bool)C[1]) + "," +
							ConvertToCode((bool)D[1]) + "," +
							ConvertToCode((bool)E[1]) + "," +
							ConvertToCode((bool)F[1]) + "," +
							ConvertToCode((bool)G[1]) + "," +
							ConvertToCode((bool)H[1]);

						MeshCombination(A, B, C, D, E, F, G, H, combination, chunk);
					}
				}
			}
            Chunk newChunk = new Chunk();
            newChunk.startingPosition = chunk.startingPosition;
            newChunk.vertices = chunk.vertices;
            newChunk.blockData = tempBlockData;
            tempChunk.Add(newChunk);
        }
        chunks = tempChunk;

        foreach (Chunk chunk in chunks)
        {
            CreatePlanet(chunk);
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

    private void MeshCombination(ArrayList A, ArrayList B, ArrayList C, ArrayList D, ArrayList E, ArrayList F, ArrayList G, ArrayList H, string combination, Chunk chunk)
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
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,0,0,0,0":
                //A,B,D
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,0,0,0,0":
                //A,C,D
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk); ;
                break;

            case "0,1,1,1,0,0,0,0":
                //B,C,D
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = C;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,0,0,1,0":
                //A,C,G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = G;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,0,0,1,0,1,0":
                //A,E,G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = E;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,1,0,0,0":
                //A,C,E
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,0,1,0,1,0":
                //C,E,G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = G;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,0,1,0,1,1":
                //E,G,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = H;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,0,1,1,0,1":
                //E,F,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = F;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,0,1,1,1,0":
                //E,F,G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,0,0,1,1,1":
                //F,G,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,0,1,0,0":
                //B,D,F
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = B;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,0,0,0,1":
                //B,D,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,0,0,1,0,1":
                //B,F,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,1,0,1,0,1":
                //D,F,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = D;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,1,0,0,0":
                //A,B,E
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = B;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,0,1,0,0":
                //A,B,F
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = B;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,0,0,1,1,0,0":
                //A,E,F
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = F;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,0,1,1,0,0":
                //B,E,F
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,0,0,1,0":
                //C,D,G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = D;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,0,0,0,1":
                //C,D,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = H;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,0,0,0,1,1":
                //C,G,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = H;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,1,0,0,1,1":
                //D,G,H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = H;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,1,1,1,1,1,1":
                //Everything except A
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,1,1,1,1":
                //Everything except B
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,1,1,1,1":
                //Everything except C
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,0,1,1,1,1":
                //Everything except D
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,1,1,1":
                //Everything except E
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,0,1,1":
                //Everything except F
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,1,0,1":
                //Everything except G
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,1,1,0":
                //Everything except H
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,1,0,1,1,1":
                //DFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,0,0,1,0":
                //ACDG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,0,1,0,1,1":
                //CEGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,1,1,0,0,0,1":
                //BCDH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,0,1,0,0":
                //ABDF
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,0,0,1,1,1,0":
                //AEFG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,0,1,0,0,0":
                //ABCE
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,0,1,1,0,1":
                //BEFH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
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
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,0,1,0,1":
                //B,F,D,H Left of the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,0,1,1,1,1":
                //E,F,G,H Back of the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,1,0,1,0":
                //A,C,E,G Left of the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,1,1,0,0":
                //ABEF Top of the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = B;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,0,0,1,1":
                //Bottom of the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = H;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = H;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,0,1,1":
                //A,B,C,D,G,H aka slant facing upwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = A;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,1,1,1,1":
                //A,B,E,F,G,H aka slant facing downwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = B;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,1,1,1,1":
                //C,D,E,F,G,H aka slant facing upwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = F;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,1,0,0":
                //A,B,C,D,E,F aka slant facing downwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = E;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,1,0,1,1":
                //A,C,D,E,G,H aka slant facing upwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = E;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,1,1,0,1":
                //A,B,D,E,F,H aka slant facing downwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = A;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,1,1,0,1,1,1":
                //B,C,D,F,G,H aka slant facing upwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = B;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,0,1,1,1,0":
                //ABCEFG aka slant facing downwards
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = F;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,1,0,1":
                //A,B,C,D,F,H face going across the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = A;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = A;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,1,1,1,1":
                //ACEFGH face going across the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = F;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = F;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,0,1,0":
                //A,B,C,D,E,G face going across the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = E;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = E;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,1,1,1,1":
                //BDEFGH face going across the cube
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = B;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,1,0,1,1":
                //CDEGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,1,0,1,1,1":
                //CDFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,1,1,0,0,1,1":
                //BCDGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,0,0,1,1":
                //ACDGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,0,1,1,1,1,1":
                //DEFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,0,1,1,1":
                //BDFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = F;
                combinationVertices[2] = B;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,1,1,0,1,0,1":
                //BCDFH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,0,0,1":
                //ABCDH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = B;
                combinationVertices[2] = A;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,0,1,0":
                //ABCDG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,1,1,0,1,0":
                //ACDEG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = A;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,1,0,1,1":
                //ACEGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,0,1,0,1,1,1,1":
                //CEFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = E;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,0,0,1,1,1,1":
                //AEFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = F;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = F;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,0,1,1,1,1":
                //BEFGH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = E;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = E;
                combinationVertices[2] = B;
                CreateMesh(combinationVertices, chunk);
                break;

            case "0,1,0,1,1,1,0,1":
                //BDEFH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = B;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = B;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,0,1,0,1":
                //ABDFH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = F;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = F;
                combinationVertices[2] = A;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,0,1,0,0":
                //ABCDF
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = A;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = A;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,1,1,0,0,0":
                //ABCDE
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = B;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = B;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,0,1,0,1,0":
                //ABCEG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = E;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = E;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,0,1,0,1,1,1,0":
                //ACEFG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = A;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = A;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,1,1,1,0":
                //ABEFG
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = A;
                combinationVertices[1] = B;
                combinationVertices[2] = G;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = G;
                combinationVertices[1] = B;
                combinationVertices[2] = F;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,0,1,1,0,1":
                //ABEFH
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = E;
                combinationVertices[1] = A;
                combinationVertices[2] = H;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = H;
                combinationVertices[1] = A;
                combinationVertices[2] = B;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,0,1,1,1,0,0":
                //ABDEF
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = F;
                combinationVertices[1] = E;
                combinationVertices[2] = D;
                CreateMesh(combinationVertices, chunk);
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = D;
                combinationVertices[1] = E;
                combinationVertices[2] = A;
                CreateMesh(combinationVertices, chunk);
                break;

            case "1,1,1,0,1,1,0,0":
                //ABCEF
                combinationVertices = new ArrayList[3];
                combinationVertices[0] = B;
                combinationVertices[1] = F;
                combinationVertices[2] = C;
                CreateMesh(combinationVertices, chunk);

                combinationVertices = new ArrayList[3];
                combinationVertices[0] = C;
                combinationVertices[1] = F;
                combinationVertices[2] = E;
                CreateMesh(combinationVertices, chunk);
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

    private void CreateMesh(ArrayList[] objects, Chunk chunk)
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

        tempBlockData.Add(ci);

        Destroy(customMesh);
    }

    private void CreatePlanet(Chunk chunk)
    {
        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());

        for (int i = 0; i < chunk.blockData.Count; i++)
        {
            vertexCount += chunk.blockData[i].mesh.vertexCount;
            if (vertexCount >= 60000)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(chunk.blockData[i]);
            }
        }
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject planetMesh = new GameObject("Chunk");
            planetMesh.transform.parent = container.transform;
            MeshFilter meshFilter = planetMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = planetMesh.AddComponent<MeshRenderer>();
            meshRenderer.material = meshTexture;
            meshFilter.mesh.CombineMeshes(data.ToArray());
            //chunk.meshes.Add(meshFilter.mesh);
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh; //setting colliders takes more time. disabled for testing.
        }
    }
}

//	private float[] NoiseGenerator()
//	{
//		OpenSimplexNoise noise = new OpenSimplexNoise();
//		Vector3 centre = new Vector3(planetSize/2, planetSize/2, planetSize/2);
//        float[] pixels = new float[planetSize * planetSize * planetSize];
//        int i = 0;
//        for (float x = 0; x < planetSize; x++)
//        {
//            for (float y = 0; y < planetSize; y++)
//            {
//                for(float z = 0; z < planetSize; z++)
//				{
//					Vector3 pos = new Vector3(x, y, z);
//					float distance = Vector3.Distance(pos, centre);

//					float xPos = x * scale.x;
//					float yPos = y * scale.y;
//					float zPos = z * scale.z;

//					distance += ((float)noise.Evaluate(xPos, yPos, zPos) * amplitude);
//					pixels[i] = distance;
//					i++;
//				}
//            }
//        }
//		return pixels;
//    }

//	private void bufferTest()
//	{
//        chunkBuffer = new ComputeBuffer(planetSize * planetSize * planetSize, sizeof(float));
//        valueBuffer = new ComputeBuffer(returnVal, sizeof(float));

//        float[] pixels = NoiseGenerator();
//		for (int i = 0; i < returnVal; i++) {
//			Debug.Log("From array = " + (testValue + i) + " ---- " + pixels[testValue + i]);
//		}

//        chunkBuffer.SetData(pixels);
//        chunkVoxelCreater.SetBuffer(0, "pixels", chunkBuffer);
//        chunkVoxelCreater.SetBuffer(0, "randomValue", valueBuffer);
//		chunkVoxelCreater.SetInt("testValue", testValue);
//        chunkVoxelCreater.SetInt("returnVal", returnVal);
//        chunkVoxelCreater.Dispatch(0, 1, 1, 1);

//		Debug.Log("chunkBuffer.count = " + valueBuffer.count);
//        float[] test = new float[valueBuffer.count];
//        valueBuffer.GetData(test, 0, 0, valueBuffer.count);
//		int j = 0;
//        for (int i = 0; i < returnVal; i++)
//        {
//            Debug.Log("From HLSL = " + (testValue + i) + " ---- " + test[i]);
//        }

//		valueBuffer.Dispose();
//		chunkBuffer.Dispose();
//    }
//}
