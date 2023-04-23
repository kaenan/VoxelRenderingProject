using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMeshCreater : MonoBehaviour
{
	[HideInInspector]
	public GameObject voxelPrefab;
	public int startingPositionX;
	public int startingPositionY;
	public int startingPositionZ;
	public int chunkSize;

	public void CreatePoints()
	{
		for (int x = startingPositionX; x < startingPositionX + chunkSize; x++)
		{
			for (int y = startingPositionY; y < startingPositionY + chunkSize; y++)
			{
				for (int z = startingPositionZ; z < startingPositionZ + chunkSize; y++)
				{
					Debug.Log("test");
				}
			}
		}
	}
}
