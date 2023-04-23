using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderVoxelCube : MonoBehaviour
{
	[SerializeField] int size;
	[SerializeField] GameObject voxelPrefab;
	public bool sphere = false;

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

	//void Start()
	//{
	//	GameObject container = new GameObject();
	//	voxels = new GameObject[xAxis * yAxis * zAxis];
	//	int arrayStart = 0;
	//	int numPerChunk = 0;
	//	for (int i = 0; i < xAxis; i++)
	//	{
	//		for (int j = 0; j < yAxis; j++)
	//		{
	//			for (int k = 0; k < zAxis; k++)
	//			{
	//				//if (numPerChunk == 0 || numPerChunk == 5)
	//				//{
	//				//	latestChunk = Instantiate(chunk, transform.position, Quaternion.identity);
	//				//	latestChunk.transform.SetParent(transform);
	//				//	numPerChunk = 0;
	//				//}

	//				GameObject render = Instantiate(voxelPrefab, new Vector3(i, j, k), Quaternion.identity);
	//				render.transform.SetParent(container.transform);
	//				voxels[arrayStart] = render;
	//				arrayStart++;
	//				numPerChunk++;

	//				//if ((i == 0 || j == 0 || k == 0) || (i == xAxis - 1 || j == yAxis - 1 || k == zAxis - 1))
	//				//{
	//				//	render.SetActive(true);
	//				//}
	//				//else
	//				//{
	//				//	render.SetActive(false);
	//				//}						
	//			}
	//		}
	//	}
	//}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			CreateVoxels();
		}
	}

	private void CreateVoxels()
	{
		var timer = new System.Diagnostics.Stopwatch();
		timer.Start();

		Vector3 centre = new Vector3(size/2, size/2, size/2);

        GameObject container = new GameObject();
        voxels = new GameObject[size * size * size];
        int arrayStart = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
					if (sphere)
					{
						float distance = Vector3.Distance(centre, new Vector3(x, y, z));
						if(distance < size / 2)
						{
                            GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
                            render.transform.SetParent(container.transform);
                            voxels[arrayStart] = render;
                            arrayStart++;
                        }
					}
					else
					{
                        GameObject render = Instantiate(voxelPrefab, new Vector3(x, y, z), Quaternion.identity);
                        render.transform.SetParent(container.transform);
                        voxels[arrayStart] = render;
                        arrayStart++;
                    }		
                }
            }
        }
		timer.Stop();
		Debug.Log("Execution time: " + timer.ElapsedMilliseconds + "ms");
    }
}
