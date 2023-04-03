using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
	private float renderDistance = 10f;
	private GameObject mesh;
	private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if(mesh != null)
		{
			float distance = Vector3.Distance(mesh.transform.position, player.transform.position);
			if(distance < renderDistance && !mesh.activeSelf)
			{
				mesh.SetActive(true);
			} else if(distance > renderDistance && mesh.activeSelf)
			{
				mesh.SetActive(false);
			}
		}
    }

	public void SetMeshObject(GameObject meshObject)
	{
		mesh = meshObject;
	}

	public void SetRenderDistance(float distance)
	{
		renderDistance = distance;
	}
}
