using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public enum GameState { SplashScreen, Menu, Game };
    public static GameState gameState = GameState.SplashScreen;

    public static byte[,] levelmap = new byte[BuildLevel.LEVEL_DIMENSION, BuildLevel.LEVEL_DIMENSION];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;
    public static (int, int) current_player_pos = (0, 0);
    public static Dictionary<int, (int, int)> amygdala_map_positions = new Dictionary<int, (int, int)>();
    public static Dictionary<int, (int, int)> obstacle_map_positions = new Dictionary<int, (int, int)>();

    public static void Reset()
    {
        rotation_direction = 0;
        current_angle = 0;
        lock_rotation = false;
        amygdala_map_positions.Clear();
        obstacle_map_positions.Clear();
        Physics2D.gravity = new Vector3(0.0f, -9.8f, 0.0f);
    }

    public static void recalculate_amygdala_positions()
    {
        for (int i = 0; i < BuildLevel.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < BuildLevel.LEVEL_DIMENSION; ++j)
            {
                if ((levelmap[j, i] == 2) || (levelmap[j, i] == 131) || (levelmap[j, i] == 132)) // Amygdala or Obstacle
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

        foreach (KeyValuePair<int, (int, int)> obstacle in obstacle_map_positions)
        {
            //Debug.Log("Respawning obstacle at (" + amygdala.Value.Item1 + "," + amygdala.Value.Item2 + ")");
            levelmap[obstacle.Value.Item1, obstacle.Value.Item2] = 131;
        }
    }
}
