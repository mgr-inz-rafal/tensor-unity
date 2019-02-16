using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intermission : MonoBehaviour
{
    public GameObject flora, flora_instance;
    public GameObject pieczara, pieczara_instance;
    public GameObject pieczarax, pieczarax_instance;

    public GameObject title01_top, title01_bottom;

    public GameObject no01;
    public GameObject cave_number_instance;
    public GameObject title_top_instance, title_bottom_instance;

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

        cave_number_instance = Instantiate(no01, new Vector3(2.0f, 6.7f, 1), Quaternion.identity);

        title_top_instance = Instantiate(title01_top, new Vector3(-18.0f, 3.44f, 1), Quaternion.identity);
        title_bottom_instance = Instantiate(title01_bottom, new Vector3(-18.0f, 2.23f, 1), Quaternion.identity);
        // title_top_instance = Instantiate(title01_top, new Vector3(1.64f, 3.44f, 1), Quaternion.identity);
        // title_bottom_instance = Instantiate(title01_bottom, new Vector3(1.64f, 2.23f, 1), Quaternion.identity);
    }

    public void PerformDestroyIntermission()
    {
        Destroy(flora_instance);
        Destroy(pieczara_instance);
        Destroy(pieczarax_instance);
        Destroy(cave_number_instance);
        Destroy(title_top_instance);
        Destroy(title_bottom_instance);
    }
}
