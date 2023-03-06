using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTest2 : MonoBehaviour
{
    public ComputeShader shader;
    private ComputeBuffer buffer;

    public GameObject voxelPrefab;
    public int size;
    private float[] centre;

    private GameObject container;

    struct Voxel
    {
        public int x;
        public int y;
        public int z;
        public float distanceValue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.Find("Planet")) Destroy(GameObject.Find("Planet"));
            container = new GameObject("Planet");
            centre = new float[3];
            centre[0] = size / 2;
            centre[1] = size / 2;
            centre[2] = size / 2;
            DispatchShader();
        }
    }

    private void DispatchShader()
    {
        buffer = new ComputeBuffer(size * size * size, sizeof(float) + (sizeof(int) * 3), ComputeBufferType.Append);
        //buffer = new ComputeBuffer();
        //buffer.SetCounterValue(0);

        shader.SetBuffer(0, "buffer", buffer);

        shader.SetFloat("size", size);
        shader.SetFloats("centre", centre);

        shader.Dispatch(0, size / 8, size / 8, size / 8);

        Voxel[] voxels = new Voxel[size * size * size];
        buffer.GetData(voxels);

        int i = 0;
        foreach (Voxel voxel in voxels)
        {
            if (voxel.distanceValue > 0)
            {
                Vector3 pos = new Vector3(voxel.x, voxel.y, voxel.z);
                GameObject temp = Instantiate(voxelPrefab, pos, Quaternion.identity);
                temp.transform.name = i + " --- " + voxel.distanceValue.ToString();
                temp.transform.SetParent(container.transform);
                i++;
            }
        }
    }
}
