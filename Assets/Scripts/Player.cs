using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Player : MonoBehaviour
{
    public AudioClip pickup1;
    public AudioClip pickup2;
    public AudioClip level_up;

    System.Random rnd = new System.Random();

    public enum Move_Direction { Left, Right };
    Dictionary<int, Tuple<int, int>> moves_right = new Dictionary<int, Tuple<int, int>>()
    {
        { 0, new Tuple<int, int>(1, 0) },
        { 90, new Tuple<int, int>(0, 1) },
        { 180, new Tuple<int, int>(-1, 0) },
        { 270, new Tuple<int, int>(0, -1) }
    };
    Dictionary<int, Tuple<int, int>> moves_left = new Dictionary<int, Tuple<int, int>>()
    {
        { 0, new Tuple<int, int>(-1, 0) },
        { 90, new Tuple<int, int>(0, -1) },
        { 180, new Tuple<int, int>(1, 0) },
        { 270, new Tuple<int, int>(0, 1) }
    };
    Dictionary<Move_Direction, Dictionary<int, Tuple<int, int>>> moves_definition = new Dictionary<Move_Direction, Dictionary<int, Tuple<int, int>>>();

    const int MOVE_COUNT = 8;

    bool block_next_step = false;
    int player_movement_count = 0;
    Tuple<float, float> player_movement_modifier = new Tuple<float, float>(0.0f, 0.0f);

    bool is_wall(byte b) {
        return b == 1;
    }

    bool is_obstacle(byte b) {
        return b == 131;
    }

    bool is_amygdala(byte b) {
        return b == 2;
    }

    Tuple<bool, int, int> amygdala_on_the_way(Move_Direction m)
    {
        if (player_movement_count != 0) {
            return new Tuple<bool, int, int>(false, -1, -1);
        }

        Tuple<int, int> modifier = get_player_position_modifiers(m);
        //Debug.Log(modifier);
        var px = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.y);
        px += modifier.Item1;
        py += modifier.Item2;

        //WorldState.virt[px, py] = 166;
        //WorldState.debug_print_virtual_level();
        byte at_location = WorldState.virt[px, py];
        // Debug.Log("at_location = " + at_location);
        if (is_amygdala(at_location)) {
            return new Tuple<bool, int, int>(true, px, py);
        }
        return new Tuple<bool, int, int>(false, -1, -1);
    }

    bool obstacles_on_the_way(Move_Direction m)
    {
        Tuple<int, int> modifier = get_player_position_modifiers(m);
        //Debug.Log(modifier);
        var px = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docent_instance.transform.position.y);
        px += modifier.Item1;
        py += modifier.Item2;

        //WorldState.virt[px, py] = 166;
        //WorldState.debug_print_virtual_level();
        byte at_location = WorldState.virt[px, py];
        // Debug.Log("at_location = " + at_location);
        return is_wall(at_location) || is_obstacle(at_location);
    }

    public void StepRight()
    {
        if (WorldState.lock_rotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }

        if (false == obstacles_on_the_way(Move_Direction.Right))
        {
            var amyg_in_way = amygdala_on_the_way(Move_Direction.Right);
            if (true == amyg_in_way.Item1) {
                Debug.Log("Amygdala found at " + amyg_in_way.Item2 + "/" + amyg_in_way.Item3 + "!");
                WorldState.destroy_amygdala_at(amyg_in_way.Item2, amyg_in_way.Item3);
            }
            if (false == block_next_step)
            {
                block_next_step = true;
                face_right();
                DoStep();
            }
        }
    }

    public void StepLeft()
    {
        if (WorldState.lock_rotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }

        if (false == obstacles_on_the_way(Move_Direction.Left))
        {
            var amyg_in_way = amygdala_on_the_way(Move_Direction.Left);
            if (true == amyg_in_way.Item1) {
                Debug.Log("Amygdala found at " + amyg_in_way.Item2 + "/" + amyg_in_way.Item3 + "!");
                WorldState.destroy_amygdala_at(amyg_in_way.Item2, amyg_in_way.Item3);
            }
            if (false == block_next_step)
            {
                block_next_step = true;
                face_left();
                DoStep();
            }
        }
    }

    Tuple<int, int> get_player_position_modifiers(Move_Direction m)
    {
        return moves_definition[m][WorldState.current_angle];
    }

    void initialize_player_movement(Tuple<int, int> modifiers)
    {
        switch (WorldState.current_angle)
        {
            case 0:
            case 180:
                player_movement_modifier = new Tuple<float, float>((float)modifiers.Item1 / MOVE_COUNT, (float)modifiers.Item2);
                break;
            case 90:
            default:
                player_movement_modifier = new Tuple<float, float>((float)modifiers.Item1, (float)modifiers.Item2 / MOVE_COUNT);
                break;
        }
    }

    void face_right()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.flipX = false;
        initialize_player_movement(get_player_position_modifiers(Move_Direction.Right));
    }

    void face_left()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.flipX = true;
        initialize_player_movement(get_player_position_modifiers(Move_Direction.Left));
    }

    public void DoStep()
    {
        if (player_movement_count == 0)
        {
            player_movement_count = MOVE_COUNT;
            start_animation();
        }
    }

    void Update()
    {
        if (BuildLevel.docent_instance == null)
        {
            return;
        }

        if (WorldState.gameState == WorldState.GameState.Elevator)
        {
            Vector3 docentpos = BuildLevel.docent_instance.transform.position;
            Vector3 elevpos = BuildLevel.elevator_instance.transform.position;

            if ((Math.Abs(elevpos.x - docentpos.x) < 0.3f) && (Math.Abs(elevpos.y - docentpos.y) < 0.3f))
            {
                //Debug.Log("Destroying docent");
                BuildLevel.docent_instance.transform.position = new Vector3(-1000, -1000, -1000);
                Destroy(BuildLevel.docent_instance, 2);
                BuildLevel.docent_instance = null;
            }
        }

        if (player_movement_count > 0)
        {
            perform_step();
            player_movement_count--;
            if (player_movement_count == 0)
            {
                stop_animation();
                block_next_step = false;
                WorldState.lock_rotation = true;
                WorldState.EnableGravitySelective();
                WorldState.skip_check_docent_moving = 2;
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
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
            buildLevel.PerformDestroy();
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.current_angle));

            BuildMenu buildMenu = cameraObj.GetComponent<BuildMenu>();
            if (buildMenu == null)
            {
                //Debug.Log("Unable to access BuildMenu");
                return;
            }
            buildMenu.PerformMenu();

            //Debug.Log("Going back to menu");
            WorldState.gameState = WorldState.GameState.Menu;
        }
    }

    void perform_step()
    {
        Vector3 position_change = new Vector3(player_movement_modifier.Item1, player_movement_modifier.Item2, 0.0f);
        gameObject.transform.position += position_change;
    }

    void RotateLeft()
    {
        this.transform.Rotate(new Vector3(0, 0, 1), 90);
    }

    void RotateRight()
    {
        this.transform.Rotate(new Vector3(0, 0, 1), -90);
    }

    void start_animation()
    {
        Animator anim = GetComponent<Animator>();
        anim.speed = 1.0f;
    }

    void stop_animation()
    {
        Animator anim = GetComponent<Animator>();
        anim.speed = 0.0f;
    }

    public Player()
    {
        moves_definition.Add(Move_Direction.Left, moves_left);
        moves_definition.Add(Move_Direction.Right, moves_right);
    }

    public void play_pickup_sound() {
        if (WorldState.total_amygdalas == 0)
        {
            GetComponent<AudioSource>().PlayOneShot(level_up, 1.0f);
        }
        else
        {
            if (rnd.Next(1, 3) == 1)
            {
                GetComponent<AudioSource>().PlayOneShot(pickup1, 1.0f);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(pickup2, 1.0f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // TODO: RC: Must be reworked when "map_spot" approach to physics is removed.
        //       Code responsible for removing amygdalas, spawning elevator, etc. must be retained.
        return;

        // Debug.Log("OnCollisionEnter2D");
        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
            if (amyg.CompareTag("Obstacle"))
            {
                // This is not really an amygdala
                continue;
            }
            if (amyg == col.gameObject)
            {
                float amygdala_pos = 0;
                float player_pos = 0;
                switch (WorldState.current_angle)
                {
                    case 0:
                    case 180:
                        amygdala_pos = amyg.transform.position.y;
                        player_pos = gameObject.transform.position.y;
                        break;
                    case 90:
                    case 270:
                        amygdala_pos = amyg.transform.position.x;
                        player_pos = gameObject.transform.position.x;
                        break;
                }

                // Debug.Log(amygdala_pos + " --- " + player_pos + " --- " + Math.Abs(amygdala_pos - player_pos));

                if (Math.Abs(amygdala_pos - player_pos) < 0.2f)
                {
                    WorldState.last_amygdala_position = amyg.transform.position;
                    --WorldState.total_amygdalas;
                    if (WorldState.total_amygdalas == 0)
                    {
                        GetComponent<AudioSource>().PlayOneShot(level_up, 1.0f);
                    }
                    else
                    {
                        if (rnd.Next(1, 3) == 1)
                        {
                            GetComponent<AudioSource>().PlayOneShot(pickup1, 1.0f);
                        }
                        else
                        {
                            GetComponent<AudioSource>().PlayOneShot(pickup2, 1.0f);
                        }
                    }
                    WorldState.amygdala_map_positions.Remove(amyg.GetInstanceID());
                    BuildLevel.amygdalas_instances.Remove(amyg);
                    Destroy(amyg, 0.0f);
                    //Debug.Log("Amygdalas left in this level:" + WorldState.total_amygdalas);
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
                    return;
                }
            }
        }
    }
}