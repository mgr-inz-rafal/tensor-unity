using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static byte[,] levelmap = new byte[12, 12];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
