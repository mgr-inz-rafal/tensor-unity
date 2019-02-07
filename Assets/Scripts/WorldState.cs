using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static byte[,] levelmap = new byte[12, 12];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;
    public static (int, int) current_player_pos = (0, 0);
    public static Dictionary<int, (int, int)> amygdala_map_positions = new Dictionary<int, (int, int)>();
}
