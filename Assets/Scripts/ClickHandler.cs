using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickHandler : MonoBehaviour
{
    const float GRAVITY_VALUE = 9.81f;
    const float NOT_MOVING_MAGNITUTE_THRESHOLD = 0.01f;
    const int ROTATION_LOCK_COUNT = 30;

    int target_angle;
    bool rotate_player_left = false;
    int rotation_unlock_frame_count = ROTATION_LOCK_COUNT;

    bool buffer_rotation_right = false;
    bool buffer_rotation_left = false;

    void Handle_ClickOnSplashScreen()
    {
        Camera cameraObj = Camera.main;
        if (cameraObj == null)
        {
            Debug.Log("Unable to access main Camera");
            return;
        }
        BuildMenu buildMenu = cameraObj.GetComponent<BuildMenu>();
        if (buildMenu == null)
        {
            Debug.Log("Unable to access BuildMenu");
            return;
        }
        Destroy(buildMenu.splashScreen_instance);
        UpdateLevelNumber();
        buildMenu.PerformMenu();
        //buildMenu.ShowNavigationButtons();

        WorldState.gameState = WorldState.GameState.Menu;
    }

    void Handle_ClickOnMenuScreen()
    {
        Camera cameraObj = Camera.main;
        if (cameraObj == null)
        {
            Debug.Log("Unable to access main Camera");
            return;
        }
        BuildMenu buildMenu = cameraObj.GetComponent<BuildMenu>();
        if (buildMenu == null)
        {
            Debug.Log("Unable to access BuildMenu");
            return;
        }
        Destroy(buildMenu.splashScreen_instance);
        Destroy(buildMenu.credits_instance);
        buildMenu.HideNavigationButtons();

        Intermission intermission = cameraObj.GetComponent<Intermission>();
        if (intermission == null)
        {
            Debug.Log("Unable to access Intermission");
            return;
        }
        intermission.SendMessage("PerformBuildIntermission");
        WorldState.gameState = WorldState.GameState.Intermission_FloraIn;
    }

    void Handle_ClickOnIntermissionScreen()
    {
        Camera cameraObj = Camera.main;
        if (cameraObj == null)
        {
            Debug.Log("Unable to access main Camera");
            return;
        }
        Intermission intermission = cameraObj.GetComponent<Intermission>();
        if (intermission == null)
        {
            Debug.Log("Unable to access Intermission");
            return;
        }
        intermission.SendMessage("PerformDestroyIntermission");

        if (WorldState.current_level == WorldState.MAX_LEVEL_NUMBER + 1)
        {
            WorldState.current_level = 1;
            BuildMenu buildMenu = cameraObj.GetComponent<BuildMenu>();
            if (buildMenu == null)
            {
                Debug.Log("Unable to access BuildMenu");
                return;
            }
            buildMenu.Start();
            WorldState.gameState = WorldState.GameState.SplashScreen;
        }
        else
        {
            WorldState.gameState = WorldState.GameState.Game;
            BuildLevel buildLevel = cameraObj.GetComponent<BuildLevel>();
            if (buildLevel == null)
            {
                Debug.Log("Unable to access BuildLevel");
                return;
            }
            buildLevel.PerformBuild();
        }
    }

    public void OnClick_Rotate_Right()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (WorldState.current_level != WorldState.MAX_LEVEL_NUMBER + 1)
                    {
                        Handle_ClickOnIntermissionScreen();
                    }
                }
                break;
            case WorldState.GameState.Intermission_PieczaraReveal:
            case WorldState.GameState.Intermission_WaitingForNumber:
            case WorldState.GameState.Intermission_TopTitle:
            case WorldState.GameState.Intermission_BottomTitle:
            case WorldState.GameState.Intermission_Done:
                Handle_ClickOnIntermissionScreen();
                break;
            case WorldState.GameState.Menu:
                Handle_ClickOnMenuScreen();
                break;
            case WorldState.GameState.SplashScreen:
                Handle_ClickOnSplashScreen();
                break;
            case WorldState.GameState.Game:
                if (WorldState.rotation_direction != 0)
                {
                    return;
                }

                if (WorldState.lock_rotation)
                {
                    if (!buffer_rotation_left)
                    {
                        buffer_rotation_right = true;
                    }
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
                break;
        }
    }

    public void OnClick_Rotate_Left()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (WorldState.current_level != WorldState.MAX_LEVEL_NUMBER + 1)
                    {
                        Handle_ClickOnIntermissionScreen();
                    }
                }
                break;
            case WorldState.GameState.Intermission_PieczaraReveal:
            case WorldState.GameState.Intermission_WaitingForNumber:
            case WorldState.GameState.Intermission_TopTitle:
            case WorldState.GameState.Intermission_BottomTitle:
            case WorldState.GameState.Intermission_Done:
                Handle_ClickOnIntermissionScreen();
                break;
            case WorldState.GameState.Menu:
                Handle_ClickOnMenuScreen();
                break;
            case WorldState.GameState.SplashScreen:
                Handle_ClickOnSplashScreen();
                break;
            case WorldState.GameState.Game:
                if (WorldState.rotation_direction != 0)
                {
                    return;
                }

                if (WorldState.lock_rotation)
                {
                    if (!buffer_rotation_right)
                    {
                        buffer_rotation_left = true;
                    }
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
                break;
        }
    }

    public void OnClick_RightArrow()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (WorldState.current_level != WorldState.MAX_LEVEL_NUMBER + 1)
                    {
                        Handle_ClickOnIntermissionScreen();
                    }
                }
                break;
            case WorldState.GameState.Intermission_PieczaraReveal:
            case WorldState.GameState.Intermission_WaitingForNumber:
            case WorldState.GameState.Intermission_TopTitle:
            case WorldState.GameState.Intermission_BottomTitle:
            case WorldState.GameState.Intermission_Done:
                Handle_ClickOnIntermissionScreen();
                break;
            case WorldState.GameState.Menu:
                Handle_ClickOnMenuScreen();
                break;
            case WorldState.GameState.SplashScreen:
                Handle_ClickOnSplashScreen();
                break;
            case WorldState.GameState.Game:
                Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();
                if (playerScript == null)
                {
                    Debug.Log("Unable to find Player");
                }
                else
                {
                    playerScript.SendMessage("StepRight");
                }
                break;
        }
    }

    public void OnClick_LeftArrow()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (WorldState.current_level != WorldState.MAX_LEVEL_NUMBER + 1)
                    {
                        Handle_ClickOnIntermissionScreen();
                    }
                }
                break;
            case WorldState.GameState.Intermission_PieczaraReveal:
            case WorldState.GameState.Intermission_WaitingForNumber:
            case WorldState.GameState.Intermission_TopTitle:
            case WorldState.GameState.Intermission_BottomTitle:
            case WorldState.GameState.Intermission_Done:
                Handle_ClickOnIntermissionScreen();
                break;
            case WorldState.GameState.Menu:
                Handle_ClickOnMenuScreen();
                break;
            case WorldState.GameState.SplashScreen:
                Handle_ClickOnSplashScreen();
                break;
            case WorldState.GameState.Game:
                Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();
                if (playerScript == null)
                {
                    Debug.Log("Unable to find Player");
                }
                else
                {
                    playerScript.SendMessage("StepLeft");
                }
                break;
        }
    }

    public void SetDigit(string tag, int number)
    {
        GameObject digit = GameObject.FindWithTag(tag);
        if (digit == null)
        {
            Debug.Log("Unable to find digit");
            return;
        }
        RawImage ri = digit.GetComponent<RawImage>();
        if (ri == null)
        {
            Debug.Log("Unable to get raw image attached to digit");
            return;
        }

        Texture2D texture = (Texture2D)Resources.Load("Digits/" + number);
        if (texture == null)
        {
            Debug.Log("Unable to load texture for digit '" + number + "'");
            return;
        }
        ri.texture = texture;
    }

    public void UpdateLevelNumber()
    {
        SetDigit("DigitRight", WorldState.current_level % 10);
        SetDigit("DigitLeft", WorldState.current_level / 10);
    }

    public void OnClick_LevelDown()
    {
        int current_level = WorldState.DoLevelDown();
        UpdateLevelNumber();
        Debug.Log("Level down. Current level=" + current_level);
    }

    public void OnClick_LevelUp()
    {
        int current_level = WorldState.DoLevelUp();
        UpdateLevelNumber();
        Debug.Log("Level up. Current level=" + current_level);
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
        return Math.Abs(rb.velocity.sqrMagnitude) <= NOT_MOVING_MAGNITUTE_THRESHOLD;
    }

    bool AmygdalasNotMoving()
    {
        foreach (GameObject o in BuildLevel.amygdalas_instances)
        {
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
            if (Math.Abs(rb.velocity.sqrMagnitude) >= NOT_MOVING_MAGNITUTE_THRESHOLD)
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
            WorldState.recalculate_amygdala_positions();
            //Debug.Log("Unlocking rotation");
            WorldState.lock_rotation = false;
            if (buffer_rotation_right)
            {
                OnClick_Rotate_Right();
            }
            if (buffer_rotation_left)
            {
                OnClick_Rotate_Left();
            }
            buffer_rotation_right = false;
            buffer_rotation_left = false;
        }
    }

    void Update()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Game:
                AdjustRotation();
                UnlockRotation();
                break;
        }
    }

    void finish_rotation()
    {
        WorldState.rotation_direction = 0;

        BoxCollider2D collider = BuildLevel.docent_instance.GetComponent<BoxCollider2D>();
        switch (WorldState.current_angle)
        {
            case 90:
                Physics2D.gravity = new Vector3(GRAVITY_VALUE, 0.0f, 0.0f);
                break;
            case 180:
                Physics2D.gravity = new Vector3(0.0f, GRAVITY_VALUE, 0.0f);
                break;
            case 270:
                Physics2D.gravity = new Vector3(-GRAVITY_VALUE, 0.0f, 0.0f);
                break;
            case 0:
                Physics2D.gravity = new Vector3(0.0f, -GRAVITY_VALUE, 0.0f);
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
