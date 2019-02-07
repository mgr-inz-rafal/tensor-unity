using System.Collections;
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
        if (WorldState.lock_rotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }

        Tuple<int, int> modifier = get_player_position_modifiers_right();

        int targetx = WorldState.current_player_pos.Item1 + modifier.Item1;
        int targety = WorldState.current_player_pos.Item2 + modifier.Item2;

        Debug.Log("My pos: (" + WorldState.current_player_pos.Item1 + "," + WorldState.current_player_pos.Item2 + ")  --- Wanna go to: (" + targetx + "," + targety + ")");

        if (false == block_next_step)
        {
            block_next_step = true;
            face_right();
            DoStep();
        }
    }

    public void StepLeft()
    {
        if (WorldState.lock_rotation)
        {
            // Waiting until all rigid bodies settle on the ground
            return;
        }
        if (false == block_next_step)
        {
            block_next_step = true;
            face_left();
            DoStep();
        }
    }
    Tuple<int, int> get_player_position_modifiers_right()
    {
        switch (WorldState.current_angle)
        {
            case 0:
                return new Tuple<int, int>(1, 0);
            case 90:
                return new Tuple<int, int>(0, 1);
            case 180:
                return new Tuple<int, int>(-1, 0);
            default:
                return new Tuple<int, int>(0, -1);
        }
    }

    Tuple<int, int> get_player_position_modifiers_left()
    {
        switch (WorldState.current_angle)
        {
            case 0:
                return new Tuple<int, int>(-1, 0);
            case 90:
                return new Tuple<int, int>(0, -1);
            case 180:
                return new Tuple<int, int>(1, 0);
            default:
                return new Tuple<int, int>(0, 1);
        }
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
        initialize_player_movement(get_player_position_modifiers_right());
    }

    void face_left()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.flipX = true;
        initialize_player_movement(get_player_position_modifiers_left());
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
                WorldState.lock_rotation = true;
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

    void OnCollisionEnter2D(Collision2D col)
    {
        foreach (GameObject amyg in BuildLevel.amygdalas_instances)
        {
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

                Debug.Log(amygdala_pos + " --- " + player_pos + " --- " + Math.Abs(amygdala_pos - player_pos));

                if (Math.Abs(amygdala_pos - player_pos) < 0.2f)
                {

                    BuildLevel.amygdalas_instances.Remove(amyg);
                    Destroy(amyg, 0.0f);
                    return;
                }
            }
        }
    }
}