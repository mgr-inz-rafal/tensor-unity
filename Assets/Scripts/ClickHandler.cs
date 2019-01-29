using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    const int ROTATION_LOCK_COUNT = 30;

    int target_angle;
    bool rotate_player_left = false;
    int rotation_unlock_frame_count = ROTATION_LOCK_COUNT;

    public void OnClick_Rotate_Right()
    {
        if (WorldState.rotation_direction != 0)
        {
            return;
        }

        if (WorldState.lock_rotation)
        {
            return;
        }
        rotation_unlock_frame_count = ROTATION_LOCK_COUNT;
        WorldState.lock_rotation = true;

        WorldState.rotation_direction = 1;
        target_angle = WorldState.current_angle + 90;
        if (target_angle == 360)
        {
            target_angle = 0;
        }

        rotate_player_left = true;
    }

    public void OnClick_Rotate_Left()
    {
        if (WorldState.rotation_direction != 0)
        {
            return;
        }

        if (WorldState.lock_rotation)
        {
            return;
        }
        rotation_unlock_frame_count = ROTATION_LOCK_COUNT;
        WorldState.lock_rotation = true;

        WorldState.rotation_direction = -1;
        target_angle = WorldState.current_angle - 90;
        if (target_angle < 0)
        {
            target_angle += 360;
        }

        rotate_player_left = false;
    }

    public void OnClick_RightArrow()
    {
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();
        if (playerScript == null)
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
        if (playerScript == null)
        {
            Debug.Log("Unable to find Player");
        }
        else
        {
            playerScript.SendMessage("StepLeft");
        }
    }

    void AdjustRotation()
    {
        switch (WorldState.rotation_direction)
        {
            case 0:
                break;
            case 1:
                WorldState.current_angle += 5;
                if (WorldState.current_angle == 360)
                {
                    WorldState.current_angle = 0;
                }
                do_camera_rotation();
                if (WorldState.current_angle == target_angle)
                {
                    finish_rotation();
                }
                break;
            case -1:
                WorldState.current_angle -= 5;
                if (WorldState.current_angle == -5)
                {
                    WorldState.current_angle = 360 - 5;
                }
                do_camera_rotation();
                if (WorldState.current_angle == target_angle)
                {
                    finish_rotation();
                }
                break;
        }
    }

    bool DocentNotMoving()
    {
        Rigidbody2D rb = BuildLevel.docent_instance.GetComponent<Rigidbody2D>();
        return rb.velocity.sqrMagnitude <= 0.10f;
    }

    bool AmygdalasNotMoving()
    {
        foreach (GameObject o in BuildLevel.amygdalas_instances)
        {
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
            if (rb.velocity.sqrMagnitude >= 0.10f)
            {
                return false;
            }
        }
        return true;
    }

    void UnlockRotation()
    {
        if (!WorldState.lock_rotation)
        {
            return;
        }
        rotation_unlock_frame_count--;
        if (rotation_unlock_frame_count > 0)
        {
            return;
        }

        if (DocentNotMoving() && AmygdalasNotMoving())
        {
            WorldState.lock_rotation = false;
        }
    }

    void Update()
    {
        AdjustRotation();
        UnlockRotation();
    }

    void finish_rotation()
    {
        WorldState.rotation_direction = 0;

        BoxCollider2D collider = BuildLevel.docent_instance.GetComponent<BoxCollider2D>();
        switch (WorldState.current_angle)
        {
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
        WorldState.rotation_direction = 0;
    }

    void DealWithActorRotations()
    {
        string message = rotate_player_left ? "RotateLeft" : "RotateRight";
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();
        playerScript.SendMessage(message);
        foreach (GameObject o in BuildLevel.amygdalas_instances)
        {
            Amygdala amygdalaScript = o.GetComponent<Amygdala>();
            amygdalaScript.SendMessage(message);
        }
    }

    void do_camera_rotation()
    {
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.current_angle));
    }
}
