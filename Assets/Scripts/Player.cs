using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    const int MOVE_COUNT = 8;

    int player_movement_count = 0;
    float player_movement_modifier;

    public void StepRight()
    {
        player_movement_modifier = 1.0f/MOVE_COUNT;
        DoStep();
    }

    public void StepLeft()
    {
        player_movement_modifier = -(1.0f/MOVE_COUNT);
        DoStep();
    }

    public void DoStep() {
        if(player_movement_count == 0) {
            player_movement_count = MOVE_COUNT;
            start_animation();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player_movement_count > 0) {
            perform_step();
            player_movement_count--;
            if(player_movement_count == 0) {
                stop_animation();
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
        Vector3 position_change = new Vector3(player_movement_modifier, 0.0f, 0.0f);
        this.gameObject.transform.position += position_change;
    }

    void RotateLeft() {
        this.transform.Rotate(new Vector3(0, 0, 1), 90);
    }

    void RotateRight() {
        this.transform.Rotate(new Vector3(0, 0, 1), -90);
    }

    void start_animation() {
        Animator anim = GetComponent<Animator>();
        anim.speed = 1.0f;
    }

    void stop_animation() {
        Animator anim = GetComponent<Animator>();
        anim.speed = 0.0f;
    }
}
