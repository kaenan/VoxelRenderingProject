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
                GetComponent<BoxCollider>().size = new Vector3(planet.planetData.chunkSize * player.renderDistance, planet.planetData.chunkSize * player.renderDistance, planet.planetData.chunkSize * player.renderDistance);
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
