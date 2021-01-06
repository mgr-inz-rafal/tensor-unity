using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickHandler : MonoBehaviour
{
    public AudioClip rotate;

    const float GRAVITY_VALUE = 9.81f;
    const float NOT_MOVING_MAGNITUTE_THRESHOLD = 0.01f;
    const int ROTATION_LOCK_COUNT = 30;

    int target_angle;
    bool rotate_player_left = false;
    int rotation_unlock_frame_count = ROTATION_LOCK_COUNT;

    bool rotate_right = false;
    bool rotate_left = false;

    bool going_left = false;
    bool going_right = false;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

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
        Counters.movementWarmupCounter = Consts.MOVEMENT_WARMUP;

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

        if (WorldState.currentLevel.ReachedFinal())
        {
            WorldState.currentLevel.Set(1);
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
            WorldState.DisableGravity();
            WorldState.build_virtual_level_representation();
            WorldState.debug_print_virtual_level();
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
                    if (!WorldState.currentLevel.IsLast())
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
                BeginRotation(Player.Move_Direction.Left);
                break;
        }
    }

    // TODO: Do not mix with Player
    void BeginRotation(Player.Move_Direction direction) {
        if ((WorldState.rotationDirection != 0) || (WorldState.lockRotation) || (Counters.movementWarmupCounter > 0)) {
            return;
        }

        Camera.main.orthographic = false;
        rotation_unlock_frame_count = ROTATION_LOCK_COUNT;
        WorldState.lockRotation = true;
        WorldState.cameraDistanceIndex = 0;
        GetComponent<AudioSource>().PlayOneShot(rotate, 1.0f);

        switch (direction) {
            case Player.Move_Direction.Left:
                WorldState.rotationDirection = 1;
                target_angle = WorldState.currentAngle + 90;
                if (target_angle == 360) {
                    target_angle = 0;
                }
                rotate_player_left = true;
                break;
            case Player.Move_Direction.Right:
                WorldState.rotationDirection = -1;
                target_angle = WorldState.currentAngle - 90;
                if (target_angle < 0) {
                    target_angle += 360;
                }
                rotate_player_left = false;
                break;
        }
    }

    void FinishRotation()
    {
        WorldState.rotationDirection = 0;
        Camera.main.orthographic = true;

        switch (WorldState.currentAngle)
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

        WorldState.EnableGravity();
        WorldState.rotationDirection = 0;
    }


    public void OnClick_Rotate_Left()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (!WorldState.currentLevel.IsLast())
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
                BeginRotation(Player.Move_Direction.Right);
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
                    if (!WorldState.currentLevel.IsLast())
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
                if (Counters.movementWarmupCounter > 0)
                {
                    return;
                }

                Player playerScript = BuildLevel.docentInstance.GetComponent<Player>();
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

    public void OnPointerDown_LeftArrow()
    {
        if (WorldState.LeftBorderState != WorldState.BorderState.Movement)
        {
            WorldState.LeftBorderState = WorldState.BorderState.Movement;
            BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
            buildLevel.ShowBorders();
        }
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            going_left = true;
        }
    }

    public void OnPointerUp_LeftArrow()
    {
        WorldState.LeftBorderState = WorldState.BorderState.Neutral;
        BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
        buildLevel.ShowBorders();
        going_left = false;
    }

    public void OnPointerDown_RightArrow()
    {
        if (WorldState.RightBorderState != WorldState.BorderState.Movement)
        {
            WorldState.RightBorderState = WorldState.BorderState.Movement;
            BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
            buildLevel.ShowBorders();
        }
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            going_right = true;
        }
    }

    public void OnPointerUp_RightArrow()
    {
        WorldState.RightBorderState = WorldState.BorderState.Neutral;
        BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
        buildLevel.ShowBorders();
        going_right = false;
    }

    public void OnPointerDown_RotateLeft()
    {
        if (WorldState.LeftBorderState != WorldState.BorderState.Rotation)
        {
            WorldState.LeftBorderState = WorldState.BorderState.Rotation;
            BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
            buildLevel.ShowBorders();
        }
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            rotate_left = true;
        }
    }

    public void OnPointerUp_RotateLeft()
    {
        WorldState.LeftBorderState = WorldState.BorderState.Neutral;
        BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
        buildLevel.ShowBorders();
        rotate_left = false;
    }

    public void OnPointerDown_RotateRight()
    {
        if (WorldState.RightBorderState != WorldState.BorderState.Rotation)
        {
            WorldState.RightBorderState = WorldState.BorderState.Rotation;
            BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
            buildLevel.ShowBorders();
        }
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            rotate_right = true;
        }
    }

    public void OnPointerUp_RotateRight()
    {
        WorldState.RightBorderState = WorldState.BorderState.Neutral;
        BuildLevel buildLevel = Camera.main.GetComponent<BuildLevel>();
        buildLevel.ShowBorders();
        rotate_right = false;
    }

    public void OnClick_LeftArrow()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_IncomingDocent:
                break;
            case WorldState.GameState.Intermission_FloraIn:
                {
                    if (!WorldState.currentLevel.IsLast())
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
                if (Counters.movementWarmupCounter > 0)
                {
                    return;
                }
                Player playerScript = BuildLevel.docentInstance.GetComponent<Player>();
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
        PlayerPrefs.SetInt("SelectedLevel", WorldState.currentLevel.Get());
        SetDigit("DigitRight", WorldState.currentLevel.Get() % 10);
        SetDigit("DigitLeft", WorldState.currentLevel.Get() / 10);
    }

    public void OnClick_LevelDown()
    {
        int currentLevel = WorldState.DoLevelDown();
        UpdateLevelNumber();
    }

    public void OnClick_LevelUp()
    {
        int currentLevel = WorldState.DoLevelUp();
        UpdateLevelNumber();
    }

    void AdjustRotation()
    {
        switch (WorldState.rotationDirection)
        {
            case 0:
                break;
            case 1:
                WorldState.currentAngle += Consts.ROTATION_STEP;
                if (WorldState.currentAngle == 360)
                {
                    WorldState.currentAngle = 0;
                }
                DoSceneRotation();
                if (WorldState.currentAngle == target_angle)
                {
                    FinishRotation();
                }
                break;
            case -1:
                WorldState.currentAngle -= Consts.ROTATION_STEP;
                if (WorldState.currentAngle == -Consts.ROTATION_STEP)
                {
                    WorldState.currentAngle = 360 - Consts.ROTATION_STEP;
                }
                DoSceneRotation();
                if (WorldState.currentAngle == target_angle)
                {
                    FinishRotation();
                }
                break;
        }
    }

    bool DocentNotMoving()
    {
        Rigidbody2D rb = BuildLevel.docentInstance.GetComponent<Rigidbody2D>();
        return Math.Abs(rb.velocity.sqrMagnitude) <= NOT_MOVING_MAGNITUTE_THRESHOLD;
    }

    bool AmygdalasNotMoving()
    {
        foreach (GameObject o in BuildLevel.amygdalaInstances)
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
        if (WorldState.skip_check_docent_moving > 0) {
            --WorldState.skip_check_docent_moving;
            return;
        }

        if (!WorldState.lockRotation)
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
            WorldState.build_virtual_level_representation();
            WorldState.debug_print_virtual_level();
            WorldState.lockRotation = false;
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

        if (Counters.movementWarmupCounter == 0)
        {
            if (going_left)
            {
                OnClick_LeftArrow();
            }
            else if (going_right)
            {
                OnClick_RightArrow();
            }

            if (rotate_left)
            {
                OnClick_Rotate_Left();
            }
            else if (rotate_right)
            {
                OnClick_Rotate_Right();
            }
        }
    }

    void DoSceneRotation() {
        DoCameraRotation();
        DoObjectsRotation();
    }

    void DoObjectsRotation() {
        string message = rotate_player_left ? "RotateLeft" : "RotateRight";
        Player playerScript = BuildLevel.docentInstance.GetComponent<Player>();
        playerScript.SendMessage(message);
        foreach (GameObject o in BuildLevel.amygdalaInstances)
        {
            Amygdala amygdalaScript = o.GetComponent<Amygdala>();
            amygdalaScript.SendMessage(message);
        }
    }

    void DoCameraRotation()
    {
        var pos = Camera.main.transform.position;
        pos.z = Consts.CAMERA_DISTANCE_TABLE[WorldState.cameraDistanceIndex++];
        Camera.main.transform.position = pos;
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.currentAngle));
    }
}
