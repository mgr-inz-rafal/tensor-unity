﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Player : MonoBehaviour
{
    const int MOVE_COUNT = 8;

    bool block_next_step = false;
    int player_movement_count = 0;
    Tuple<float, float> player_movement_modifier = new Tuple<float, float>(0.0f, 0.0f);

    public void StepRight()
    {
        if (false == block_next_step)
        {
            block_next_step = true;
            face_right();
            DoStep();
        }
    }

    public void StepLeft()
    {
        if (false == block_next_step)
        {
            block_next_step = true;
            face_left();
            DoStep();
        }
    }

    void face_right()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.flipX = false;
        Debug.Log(WorldState.current_angle);
        switch (WorldState.current_angle)
        {
            case 0:
                player_movement_modifier = new Tuple<float, float>(1.0f / MOVE_COUNT, 0.0f);
                break;
            case 90:
                player_movement_modifier = new Tuple<float, float>(0.0f, 1.0f / MOVE_COUNT);
                break;
            case 180:
                player_movement_modifier = new Tuple<float, float>(-(1.0f / MOVE_COUNT), 0.0f);
                break;
            case 270:
                player_movement_modifier = new Tuple<float, float>(0.0f, -(1.0f / MOVE_COUNT));
                break;
        }
    }

    void face_left()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.flipX = true;
        Debug.Log(WorldState.current_angle);
        switch (WorldState.current_angle)
        {
            case 0:
                r.flipX = true;
                player_movement_modifier = new Tuple<float, float>(-(1.0f / MOVE_COUNT), 0.0f);
                break;
            case 90:
                player_movement_modifier = new Tuple<float, float>(0.0f, -(1.0f / MOVE_COUNT));
                break;
            case 180:
                player_movement_modifier = new Tuple<float, float>(1.0f / MOVE_COUNT, 0.0f);
                r.flipX = true;
                break;
            case 270:
                player_movement_modifier = new Tuple<float, float>(0.0f, 1.0f / MOVE_COUNT);
                break;
        }
    }

    public void DoStep()
    {
        if (player_movement_count == 0)
        {
            player_movement_count = MOVE_COUNT;
            start_animation();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player_movement_count > 0)
        {
            perform_step();
            player_movement_count--;
            if (player_movement_count == 0)
            {
                stop_animation();
                block_next_step = false;
            }
        }

        /*
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            preform_step_left();
        } else if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            perform_step_right();
        }
        */
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
}
