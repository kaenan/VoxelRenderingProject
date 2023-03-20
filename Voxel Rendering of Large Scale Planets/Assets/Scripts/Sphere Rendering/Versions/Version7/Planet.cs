using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetData planetData;

    void Update()
    {
        planetData.UpdateColours(gameObject);
    }
}
