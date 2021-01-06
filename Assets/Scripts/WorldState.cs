using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static int global_debug_frames = 0;

    public static int movement_warmup_counter = 0;
    public const int MOVEMENT_WARMUP = 50;

    public static int current_level = 1;
    public const int MAX_LEVEL_NUMBER = 51;
    public static int elevator_frames = 0;
    public const int TOTAL_ELEVATOR_FRAMES = (int)(50 * 1.5f);

    public const float ELEVATOR_POSITION_CHANGE = 0.20f;

    public static int total_amygdalas = 0;
    public static int skip_check_docent_moving = 0;
    public static int skip_check_level_finished = 0;
    public static bool should_check_level_finished = false;

    public enum BorderState
    {
        Neutral,
        Rotation,
        Movement
    }
    public static BorderState LeftBorderState = BorderState.Neutral;
    public static BorderState RightBorderState = BorderState.Neutral;

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
        Intermission_FloraIn,
        Intermission_PieczaraReveal,
        Intermission_WaitingForNumber,
        Intermission_TopTitle,
        Intermission_BottomTitle,
        Intermission_Done,
        Intermission_IncomingDocent
    };
    public static GameState gameState = GameState.SplashScreen;

    public static byte[,] levelmap = new byte[BuildLevel.LEVEL_DIMENSION, BuildLevel.LEVEL_DIMENSION];
    public static byte[,] virt = new byte[BuildLevel.LEVEL_DIMENSION, BuildLevel.LEVEL_DIMENSION];
    public static int rotation_direction = 0;
    public static int current_angle = 0;
    public static bool lock_rotation = false;
    public static (int, int) current_player_pos = (0, 0);
    public static Dictionary<int, (int, int)> amygdala_map_positions = new Dictionary<int, (int, int)>();
    public static Dictionary<int, (int, int)> obstacle_map_positions = new Dictionary<int, (int, int)>();

    public static Vector3 last_amygdala_position;

    public static void EnableGravity() {
        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
            Rigidbody2D rigid = amyg.GetComponent<Rigidbody2D>();
            if (rigid == null)
            {
                return;
            }
            else
            {
                rigid.simulated = true;
            }
        }

        Rigidbody2D rigid_docent = BuildLevel.docent_instance.GetComponent<Rigidbody2D>();
        if (rigid_docent == null)
        {
            return;
        }
        else
        {
            rigid_docent.simulated = true;
        }

        //Debug.Log("Gravity ON");
    }

    // Enables gravity only for objects that have collision
    // probability after Docent made a step, i.e.:
    // - objects below current docent position (he could have stepped on an object and shouldn't fall through)
    // - objects above previous docent position (he could have stepped away and objects on his head should fall) <-- TODO
    public static void EnableGravitySelective() {
        Debug.Log("EnableGravitySelective");
        Rigidbody2D rigid_docent = BuildLevel.docent_instance.GetComponent<Rigidbody2D>();
        if (rigid_docent == null) {
            return;
        }
        else
        {
            rigid_docent.simulated = true;
        }

        var px = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.y);

        // TODO: Each call to `EnableAmygdalaAt` traverses through all amygdalas.
        // Refactor this: precalc amygdalas to enable and
        // then go once through the list.
        switch (WorldState.current_angle)
        {
            case 90:
                {
                    var x = px+1;
                    var y = py;
                    do 
                    {
                        WorldState.virt[x, y] = 254;
                    } while(++x < (BuildLevel.LEVEL_DIMENSION - 1));
                }
                break;
            case 270:
                {
                    var x = px-1;
                    var y = py;
                    do 
                    {
                        EnableAmygdalaAt(x, y);
                    } while(--x > 0);
                }
                break;
            case 0:
                // Below new docent position
                {
                    var x = px;
                    var y = py-1;
                    do 
                    {
                        EnableAmygdalaAt(x, y);
                    } while(--y > 0);
                }

                // Below old docent position
                {
                    var x = px;
                    if (Player.last_step_to_the_left) {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                    }
                    var y = py-1;
                    do 
                    {
                        WorldState.virt[x, y] = 254;
                        EnableAmygdalaAt(x, y);
                    } while(--y > 0);
                    debug_print_virtual_level();
                }

                // Above old docent position
                {
                    var x = px;
                    if (Player.last_step_to_the_left) {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                    }
                    var y = py+1;
                    do 
                    {
                        EnableAmygdalaAt(x, y);
                    } while(++y < (BuildLevel.LEVEL_DIMENSION - 1));
                }
                break;
            case 180:
                {
                    var x = px;
                    var y = py+1;
                    do 
                    {
                        Debug.Log(x + "/" + y);
                        EnableAmygdalaAt(x, y);
                    } while(++y < (BuildLevel.LEVEL_DIMENSION - 1));
                }
                break;
        }
    }

    public static void EnableAmygdalaAt(int x, int y) {
        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
            float ax = amyg.transform.position.x;
            float ay = amyg.transform.position.y;

            if ((System.Math.Abs(ax - x) < 0.1f) && (System.Math.Abs(ay - y) < 0.1f)) {
                Rigidbody2D rigid = amyg.GetComponent<Rigidbody2D>();
                if (rigid == null)
                {
                    return;
                }
                else
                {
                    rigid.simulated = true;
                }
            }
        }
    }

    public static void DisableGravity() {
        //Debug.Log("Gravity OFF");
        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
            Rigidbody2D rigid = amyg.GetComponent<Rigidbody2D>();
            if (rigid == null)
            {
                return;
            }
            else
            {
                rigid.simulated = false;
            }
        }

        if (BuildLevel.docent_instance != null) {
            Rigidbody2D rigid_docent = BuildLevel.docent_instance.GetComponent<Rigidbody2D>();
            if (rigid_docent == null)
            {
                return;
            }
            else
            {
                rigid_docent.simulated = false;
            }
        }

        //Physics2D.gravity = Vector2.zero;
    }

    public static void Reset()
    {
        rotation_direction = 0;
        current_angle = 0;
        lock_rotation = false;
        amygdala_map_positions.Clear();
        obstacle_map_positions.Clear();
        Physics2D.gravity = new Vector3(0.0f, -9.8f, 0.0f);
        //Debug.Log("Gravity On");
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

    public static void destroy_amygdala_at(int x, int y) {
        foreach (GameObject amyg in BuildLevel.amygdalas_instances) {
            float ax = amyg.transform.position.x;
            float ay = amyg.transform.position.y;
            float mx = x;
            float my = y;

            if ((System.Math.Abs(ax - mx) < 0.1f) && (System.Math.Abs(ay - my) < 0.1f)) {
                    WorldState.last_amygdala_position = amyg.transform.position;
                    BuildLevel.amygdalas_instances.Remove(amyg);
                    Destroy(amyg, 0.0f);
                    --WorldState.total_amygdalas;
                    var player = GameObject.FindWithTag("Player").GetComponent<Player>();
                    player.play_pickup_sound();
                    skip_check_level_finished = 8;
                    should_check_level_finished = true;
                    break;                
            }
        }
   }

    public static void check_level_finished() {
        if (skip_check_level_finished > 0) {
            --skip_check_level_finished;
            return;
        }
        if (WorldState.total_amygdalas == 0)
        {
            Camera cameraObj = Camera.main;
            if (cameraObj == null)
            {
                //Debug.Log("Unable to access main Camera");
                return;
            }
            BuildLevel buildLevel = cameraObj.GetComponent<BuildLevel>();
            if (buildLevel == null)
            {
                //Debug.Log("Unable to access BuildLevel");
                return;
            }
            buildLevel.SendMessage("SpawnElevator");
            ++WorldState.current_level;

            GameObject world = GameObject.FindWithTag("WorldMarker");
            if (world == null)
            {
                //Debug.Log("Unable to access world");
            }
            ClickHandler ch = world.GetComponent<ClickHandler>();
            if (world == null)
            {
                //Debug.Log("Unable to access click handler");
            }
            ch.SendMessage("UpdateLevelNumber");

            WorldState.gameState = WorldState.GameState.Elevator;
        }
    }

    public static void build_virtual_level_representation() {
        WorldState.DisableGravity();
        for (int i = 0; i < BuildLevel.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < BuildLevel.LEVEL_DIMENSION; ++j)
            {
                virt[j, i] = 0;
            }
        }

        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
            var x = (int)System.Math.Round(amyg.transform.position.x);
            var y = (int)System.Math.Round(amyg.transform.position.y);
            if (amyg.CompareTag("Amygdala")) {
                virt[x, y] = 2;
            }
            else {
                virt[x, y] = 131; // TODO: RC: or 132
            }
            amyg.transform.position = new Vector3(x, y, 0);
        }

        foreach (GameObject wall in BuildLevel.wall_instances)
        {
            var x = (int)System.Math.Round(wall.transform.position.x);
            var y = (int)System.Math.Round(wall.transform.position.y);
            virt[x, y] = 1;
            wall.transform.position = new Vector3(x, y, 0);
        }

        var px = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.y);
        virt[px, py] = 200;
        BuildLevel.docent_instance.transform.position = new Vector3(px, py, 0);
    }

    public static void debug_print_virtual_level() {
        string lev = System.Environment.NewLine;
        Debug.Log("");
        Debug.Log("---");
        for (int i = 0; i < BuildLevel.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < BuildLevel.LEVEL_DIMENSION; ++j)
            {
                switch (virt[j,i]) {
                    case 2: // Amygdala
                        lev += "$";
                        break;
                    case 131:
                        lev += "O";
                        break;
                    case 166:
                        lev += "?";
                        break;
                    case 0:
                        lev += " ";
                        break;
                    case 200:
                        lev += "P";
                        break;
                    case 254:
                        lev += "@";
                        break;
                    default:
                        lev += "#";
                        break;
                }
            }
            lev += System.Environment.NewLine;
        }
        Debug.Log(lev);
        Debug.Log("---");
    }

    public static void recalculate_amygdala_positions()
    {
        // TODO: RC: Not needed with the new psychics approach?
        return;

        for (int i = 0; i < BuildLevel.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < BuildLevel.LEVEL_DIMENSION; ++j)
            {
                if ((levelmap[j, i] == 2) || (levelmap[j, i] == 131) || (levelmap[j, i] == 132)) // Amygdala or Obstacle
                {
                    if (levelmap[j, i] == 2)
                    {
                        // Debug.Log("Dropping amygdala at (" + j + "," + i + ")");
                    }
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
        /*
        WorldState.global_debug_frames++;
        if (WorldState.global_debug_frames == 15) {
            for (var i=0; i < BuildLevel.amygdalas_instances.Count; ++i)
            {
                GameObject amyg = BuildLevel.amygdalas_instances[i];
                if (amyg.CompareTag("Obstacle"))
                {
                    // This is not really an amygdala
                    continue;
                }
                var x = amyg.transform.position.x;
                var y = amyg.transform.position.y;
                Debug.Log(i + ": (" + x + " / " + y + ")");
            }
            WorldState.global_debug_frames = 0;
        }
        */

        switch (WorldState.gameState)
        {
            case WorldState.GameState.Game:
                if (should_check_level_finished) {
                    check_level_finished();
                }
                break;
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
                //Debug.Log("Destroying elevator and going into intermission");
                Destroy(BuildLevel.elevator_instance);
                elevator_frames = 0;
                WorldState.gameState = WorldState.GameState.Intermission_FloraIn;

                Camera cameraObj = Camera.main;
                if (cameraObj == null)
                {
                    //Debug.Log("Unable to access main Camera");
                    return;
                }
                BuildLevel buildLevel = cameraObj.GetComponent<BuildLevel>();
                if (buildLevel == null)
                {
                    //Debug.Log("Unable to access BuildLevel");
                    return;
                }
                Intermission intermission = cameraObj.GetComponent<Intermission>();
                if (intermission == null)
                {
                    //Debug.Log("Unable to access Intermission");
                    return;
                }
                buildLevel.SendMessage("PerformDestroy", false);
                intermission.SendMessage("PerformBuildIntermission");
            }
        }
        --elevator_frames;
    }

}

