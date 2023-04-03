using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderVoxelCube : MonoBehaviour
{
	[SerializeField] int xAxis;
	[SerializeField] int yAxis;
	[SerializeField] int zAxis;
	[SerializeField] GameObject voxelPrefab;
	[SerializeField] GameObject chunk;

	private GameObject latestChunk;
	private GameObject[] voxels;

  //  void Start()
  //  {
		//voxels = new GameObject[xAxis * yAxis * zAxis];
		//int arrayStart = 0;
  //      for(int i=0; i<xAxis; i++)
		//{
		//	for(int j=0; j<yAxis; j++)
		//	{
		//		for(int k=0; k<zAxis; k++)
		//		{
		//			if (Random.Range(0, 2) == 1)
		//			{
		//				GameObject render = Instantiate(voxelPrefab, new Vector3(i, j, k), Quaternion.identity);
		//				render.transform.SetParent(transform);
		//				voxels[arrayStart] = render;
		//				arrayStart++;
		//			}
		//		}
		//	}
		//}
  //  }

	void Start()
	{
		voxels = new GameObject[xAxis * yAxis * zAxis];
		int arrayStart = 0;
		int numPerChunk = 0;
		for (int i = 0; i < xAxis; i++)
		{
			for (int j = 0; j < yAxis; j++)
			{
				for (int k = 0; k < zAxis; k++)
				{
					if (numPerChunk == 0 || numPerChunk == 5)
					{
						latestChunk = Instantiate(chunk, transform.position, Quaternion.identity);
						latestChunk.transform.SetParent(transform);
						numPerChunk = 0;
					}

					GameObject render = Instantiate(voxelPrefab, new Vector3(i, j, k), Quaternion.identity);
					render.transform.SetParent(latestChunk.transform);
					voxels[arrayStart] = render;
					arrayStart++;
					numPerChunk++;

					if ((i == 0 || j == 0 || k == 0) || (i == xAxis - 1 || j == yAxis - 1 || k == zAxis - 1))
					{
						render.SetActive(true);
					}
					else
					{
						render.SetActive(false);
					}						
				}
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			for(int i = 0; i < xAxis * yAxis * zAxis; i++)
			{
				voxels[i].GetComponent<Rigidbody>().useGravity = true;
			}
		}
	}
}
