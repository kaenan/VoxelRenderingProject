using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    private PlayerMovement player;
    private Planet planet;
    private Transform child;
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            player.planet.TryGetComponent(out planet);

            if(planet != null && planet.planetData.playable) 
            {
                child = transform.GetChild(0);
                child.gameObject.SetActive(false);
                float size = (planet.planetData.chunkSize + (planet.planetData.chunkSize / 2)) * player.renderDistance;
                GetComponent<BoxCollider>().size = new Vector3(size, size, size);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ChangeChildActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ChangeChildActive(false);
        }
    }

    private void ChangeChildActive(bool active)
    {
        if (planet != null && planet.planetData.playable)
        {
            transform.GetChild(0).gameObject.SetActive(active);
        }
    }
}
