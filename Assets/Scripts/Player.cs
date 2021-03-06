﻿using System.Collections;
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
    public static bool last_step_to_the_left = false;
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
        var px = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.y);
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
        var px = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.y);
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
        if (WorldState.lockRotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }

        if (false == obstacles_on_the_way(Move_Direction.Right))
        {
            last_step_to_the_left = false;
            var amyg_in_way = amygdala_on_the_way(Move_Direction.Right);
            if (true == amyg_in_way.Item1) {
                // Debug.Log("Amygdala found at " + amyg_in_way.Item2 + "/" + amyg_in_way.Item3 + "!");
                WorldState.DestroyAmygdalaAt(amyg_in_way.Item2, amyg_in_way.Item3);
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
        if (WorldState.lockRotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }

        if (false == obstacles_on_the_way(Move_Direction.Left))
        {
            last_step_to_the_left = true;
            var amyg_in_way = amygdala_on_the_way(Move_Direction.Left);
            if (true == amyg_in_way.Item1) {
                // Debug.Log("Amygdala found at " + amyg_in_way.Item2 + "/" + amyg_in_way.Item3 + "!");
                WorldState.DestroyAmygdalaAt(amyg_in_way.Item2, amyg_in_way.Item3);
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
        return moves_definition[m][WorldState.currentAngle];
    }

    void initialize_player_movement(Tuple<int, int> modifiers)
    {
        switch (WorldState.currentAngle)
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
        if (BuildLevel.docentInstance == null)
        {
            return;
        }

        if (WorldState.gameState == WorldState.GameState.Elevator)
        {
            Vector3 docentpos = BuildLevel.docentInstance.transform.position;
            Vector3 elevpos = BuildLevel.elevatorInstance.transform.position;

            if ((Math.Abs(elevpos.x - docentpos.x) < 0.3f) && (Math.Abs(elevpos.y - docentpos.y) < 0.3f))
            {
                //Debug.Log("Destroying docent");
                BuildLevel.docentInstance.transform.position = new Vector3(-1000, -1000, -1000);
                Destroy(BuildLevel.docentInstance, 2);
                BuildLevel.docentInstance = null;
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
                WorldState.lockRotation = true;
                WorldState.EnableGravitySelective();
                WorldState.skip_check_docent_moving = 1;
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            WorldState.GoBackToTitleScreen();
        }
    }

    void perform_step()
    {
        Vector3 positionChange = new Vector3(player_movement_modifier.Item1, player_movement_modifier.Item2, 0.0f);
        gameObject.transform.position += positionChange;
    }

    void RotateLeft()
    {
        this.transform.Rotate(new Vector3(0, 0, 1), Consts.ROTATION_STEP);
    }

    void RotateRight()
    {
        this.transform.Rotate(new Vector3(0, 0, 1), -Consts.ROTATION_STEP);
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

    public void playPickupSound() {
        if (WorldState.totalAmygdalas == 0)
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
}