using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static byte[,] levelmap = new byte[BuildLevel.LEVEL_DIMENSION, BuildLevel.LEVEL_DIMENSION];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;
    public static (int, int) current_player_pos = (0, 0);
    public static Dictionary<int, (int, int)> amygdala_map_positions = new Dictionary<int, (int, int)>();

    public static void recalculate_amygdala_positions()
    {
        for (int i = 0; i < BuildLevel.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < BuildLevel.LEVEL_DIMENSION; ++j)
            {
                if (levelmap[j, i] == 2)   // Amygdala
                {
                    //Debug.Log("Dropping amygdala at (" + j + "," + i + ")");
                    levelmap[j, i] = 0;
                }
            }
        }

        foreach (KeyValuePair<int, (int, int)> amygdala in amygdala_map_positions)
        {
            //Debug.Log("Respawning amygdala at (" + amygdala.Value.Item1 + "," + amygdala.Value.Item2 + ")");
            levelmap[amygdala.Value.Item1, amygdala.Value.Item2] = 2;
        }
    }
}
