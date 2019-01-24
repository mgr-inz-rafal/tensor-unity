using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    int current_angle = 0;
    int target_angle;
    int rotation_direction = 0;
    bool rotate_player_left = false;

    public void OnClick_Rotate_Right() {
        if(rotation_direction != 0) {
            return;
        }

        rotation_direction = 1;
        target_angle = current_angle + 90;
        if(target_angle == 360) {
            target_angle = 0;
        }

        rotate_player_left = true;
    }

    public void OnClick_Rotate_Left() {
        if(rotation_direction != 0) {
            return;
        }

        rotation_direction = -1;
        target_angle = current_angle - 90;
        if(target_angle < 0) {
            target_angle += 360;
        }

        rotate_player_left = false;
    }

    public void OnClick_RightArrow()
    {
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();       
        if(playerScript == null)
        {
            Debug.Log("Unable to find Player");
        }
        else
        {
            playerScript.SendMessage("StepRight");
        }
    }

    public void OnClick_LeftArrow()
    {
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();       
        if(playerScript == null)
        {
            Debug.Log("Unable to find Player");
        }
        else
        {
            playerScript.SendMessage("StepLeft");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(rotation_direction) {
            case 0:
                break;
            case 1:
                current_angle += 5;
                if(current_angle == 360) {
                    current_angle = 0;
                }
                do_camera_rotation();
                if(current_angle == target_angle) {
                    finish_rotation();
                }
                break;
            case -1:
                current_angle -= 5;
                if(current_angle == -5) {
                    current_angle = 360-5;
                }
                do_camera_rotation();
                if(current_angle == target_angle) {
                    finish_rotation();
                }
                break;
        }
    }

    void finish_rotation() {
        rotation_direction = 0;

        BoxCollider2D collider = BuildLevel.docent_instance.GetComponent<BoxCollider2D>();
        switch(current_angle) {
            case 90:
                Physics2D.gravity = new Vector3(9.8f, 0.0f, 0.0f);
                break;
            case 180:
                Physics2D.gravity = new Vector3(0.0f, 9.8f, 0.0f);
                break;
            case 270:
                Physics2D.gravity = new Vector3(-9.8f, 0.0f, 0.0f);
                break;
            case 0:
                Physics2D.gravity = new Vector3(0.0f, -9.8f, 0.0f);
                break;
        }

        DealWithActorRotations();
        rotation_direction = 0;
    }

    void DealWithActorRotations() {
        string message = rotate_player_left ? "RotateLeft" : "RotateRight";
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();
        playerScript.SendMessage(message);
        foreach(GameObject o in BuildLevel.amygdalas_instances) {
            Amygdala amygdalaScript = o.GetComponent<Amygdala>();
            amygdalaScript.SendMessage(message);
        }
    }

    void do_camera_rotation() {
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, current_angle));
    }
}
