using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intermission : MonoBehaviour
{
    public GameObject flora, flora_instance;
    public GameObject pieczara, pieczara_instance;
    public GameObject pieczarax, pieczarax_instance;

    public void PerformBuildIntermission()
    {
        WorldState.Reset();
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.current_angle));
        flora_instance = Instantiate(
            flora,
            new Vector3(
                BuildLevel.LEVEL_DIMENSION + (BuildLevel.LEVEL_DIMENSION >> 1) + 1.0f,
                BuildLevel.LEVEL_DIMENSION - (BuildLevel.LEVEL_DIMENSION >> 1) - 0.5f, 1),
                Quaternion.identity);

        pieczara_instance = Instantiate(pieczara, new Vector3(2.5f, 8.8f, 1), Quaternion.identity);
        pieczarax_instance = Instantiate(pieczarax, new Vector3(2.5f, 8.8f, 1), Quaternion.identity);
    }
}
