using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunks : MonoBehaviour
{
	[SerializeField] GameObject chunk;

    // Start is called before the first frame update
    void Start()
    {
		Vector3 start = Vector3.zero;
		GameObject newChunk = Instantiate(chunk, start, Quaternion.identity);
		newChunk.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
