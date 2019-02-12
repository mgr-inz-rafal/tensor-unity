using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public GameObject splashScreen;

    void Start()
    {
        Instantiate(splashScreen, new Vector3(5.5f, 5.5f, 0), Quaternion.identity);
    }

    void Update()
    {

    }
}
