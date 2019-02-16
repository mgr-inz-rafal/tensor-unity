using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static int current_level = 1;
    public const int MAX_LEVEL_NUMBER = 51;
    public static int elevator_frames = 0;
    public const int TOTAL_ELEVATOR_FRAMES = (int)(50 * 1.2f);

    public const float ELEVATOR_POSITION_CHANGE = 0.23f;

    public static int total_amygdalas = 0;

    public enum CreditState
    {
        FadeIn,
        Presenting,
        FadeOut
    }
    public static CreditState creditState = CreditState.FadeIn;

    public enum GameState
    {
        SplashScreen,
        Menu,
        Game,
        Elevator,
        Intermission_Flora_In,
        Intermission_Pieczara_Reveal,
        Intermission_WaitingForNumber,
        Intermission_TopTitle,
        Intermission_BottomTitle,
        Intermission_Done
    };
    public static GameState gameState = GameState.SplashScreen;

    public static byte[,] levelmap = new byte[BuildLevel.LEVEL_DIMENSION, BuildLevel.LEVEL_DIMENSION];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;
    public static (int, int) current_player_pos = (0, 0);
    public static Dictionary<int, (int, int)> amygdala_map_positions = new Dictionary<int, (int, int)>();
    public static Dictionary<int, (int, int)> obstacle_map_positions = new Dictionary<int, (int, int)>();

    public static Vector3 last_amygdala_position;

    public static void Reset()
    {
        rotation_direction = 0;
        current_angle = 0;
        lock_rotation = false;
        amygdala_map_positions.Clear();
        obstacle_map_positions.Clear();
        Physics2D.gravity = new Vector3(0.0f, -9.8f, 0.0f);
    }

    public static int DoLevelDown()
    {
        current_level--;
        if (0 == current_level)
        {
            current_level = 1;
        }
        return current_level;
    }

    public static int DoLevelUp()
    {
        current_level++;
        if (current_level == MAX_LEVEL_NUMBER + 1)
        {
            current_level = MAX_LEVEL_NUMBER;
        }
        return current_level;
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

    void Update()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Elevator:
                switch (WorldState.current_angle)
                {
                    case 0:
                        {
                            Vector3 position_change = new Vector3(0.0f, ELEVATOR_POSITION_CHANGE, 0.0f);
                            BuildLevel.elevator_instance.transform.position += position_change;
                        }
                        break;
                    case 180:
                        {
                            Vector3 position_change = new Vector3(0.0f, -ELEVATOR_POSITION_CHANGE, 0.0f);
                            BuildLevel.elevator_instance.transform.position += position_change;
                        }
                        break;
                    case 90:
                        {
                            Vector3 position_change = new Vector3(-ELEVATOR_POSITION_CHANGE, 0.0f, 0.0f);
                            BuildLevel.elevator_instance.transform.position += position_change;
                        }
                        break;
                    case 270:
                        {
                            Vector3 position_change = new Vector3(+ELEVATOR_POSITION_CHANGE, 0.0f, 0.0f);
                            BuildLevel.elevator_instance.transform.position += position_change;
                        }
                        break;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (elevator_frames > 0)
        {
            if (elevator_frames == 1)
            {
                Debug.Log("Destroying elevator and going into intermission");
                Destroy(BuildLevel.elevator_instance);
                elevator_frames = 0;
                WorldState.gameState = WorldState.GameState.Intermission_Flora_In;

                Camera cameraObj = Camera.main;
                if (cameraObj == null)
                {
                    Debug.Log("Unable to access main Camera");
                    return;
                }
                BuildLevel buildLevel = cameraObj.GetComponent<BuildLevel>();
                if (buildLevel == null)
                {
                    Debug.Log("Unable to access BuildLevel");
                    return;
                }
                Intermission intermission = cameraObj.GetComponent<Intermission>();
                if (intermission == null)
                {
                    Debug.Log("Unable to access Intermission");
                    return;
                }
                buildLevel.SendMessage("PerformDestroy", false);
                intermission.SendMessage("PerformBuildIntermission");
            }
        }
        --elevator_frames;
    }

}

