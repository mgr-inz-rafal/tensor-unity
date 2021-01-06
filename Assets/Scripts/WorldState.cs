using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static int currentLevel = 1;
    public const int MAX_LEVEL_NUMBER = 51;

    public const float ELEVATOR_positionChange = 0.20f;

    public static int totalAmygdalas = 0;
    public static int skip_check_docent_moving = 0;
    public static int skipCheckLevelFinished = 0;
    public static bool shouldCheckLevelFinished = false;
    public static int cameraDistanceIndex = 0;

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

    public static byte[,] levelmap = new byte[Consts.LEVEL_DIMENSION, Consts.LEVEL_DIMENSION];
    public static byte[,] virt = new byte[Consts.LEVEL_DIMENSION, Consts.LEVEL_DIMENSION];
    public static int rotationDirection = 0;
    public static int currentAngle = 0;
    public static bool lockRotation = false;
    public static Dictionary<int, (int, int)> amygdalaMapPositions = new Dictionary<int, (int, int)>();
    public static Dictionary<int, (int, int)> obstacleMapPositions = new Dictionary<int, (int, int)>();

    // TODO: Extract to separate component
    static byte[] amygdalasToEnable = new byte[Consts.MAXIMUM_AMYGDALAS_PER_LEVEL];
    static int amygdalasToEnableIndex;

    public static Vector3 lastAmygdalaPosition;

    public static void EnableGravity() {
        foreach (GameObject amyg in BuildLevel.amygdalaInstances)
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

        Rigidbody2D rigidDocent = BuildLevel.docentInstance.GetComponent<Rigidbody2D>();
        if (rigidDocent == null)
        {
            return;
        }
        else
        {
            rigidDocent.simulated = true;
        }

        //Debug.Log("Gravity ON");
    }

    // Enables gravity only for objects that have collision
    // probability after Docent made a step, i.e.:
    // - objects below current docent position (he could have stepped on an object and shouldn't fall through)
    // - objects above previous docent position (he could have stepped away and objects on his head should fall)
    // - objects below previous docent position (he could have stepped away and objects on his head should fall, but not through the objects below)
    public static void EnableGravitySelective() {
        // Debug.Log("EnableGravitySelective");
        Rigidbody2D rigidDocent = BuildLevel.docentInstance.GetComponent<Rigidbody2D>();
        if (rigidDocent == null) {
            return;
        }
        else
        {
            rigidDocent.simulated = true;
        }

        var px = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.y);

        amygdalasToEnableIndex = 0;
        switch (WorldState.currentAngle)
        {
            case 90:
                // Below new docent position
                {
                    var x = px+1;
                    var y = py;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++x < (Consts.LEVEL_DIMENSION - 1));
                }

                // Below old docent position
                {
                    var x = px+1;
                    var y = py;
                    if (Player.last_step_to_the_left) {
                        y += 1;
                    }
                    else
                    {
                        y -= 1;
                    }
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++x < (Consts.LEVEL_DIMENSION - 1));
                }

                // Below old docent position
                {
                    var x = px-1;
                    var y = py;
                    if (Player.last_step_to_the_left) {
                        y += 1;
                    }
                    else
                    {
                        y -= 1;
                    }
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(--x > 0);
                }
                break;
            case 270:
                // Below new docent position
                {
                    var x = px-1;
                    var y = py;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(--x > 0);
                }

                // Below old docent position
                {
                    var x = px-1;
                    var y = py;
                    if (Player.last_step_to_the_left) {
                        y -= 1;
                    }
                    else
                    {
                        y += 1;
                    }
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(--x > 0);
                }

                // Above old docent position
                {
                    var x = px+1;
                    var y = py;
                    if (Player.last_step_to_the_left) {
                        y -= 1;
                    }
                    else
                    {
                        y += 1;
                    }
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++x < (Consts.LEVEL_DIMENSION - 1));
                }
                break;
            case 0:
                // Below new docent position
                {
                    var x = px;
                    var y = py-1;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
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
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(--y > 0);
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
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++y < (Consts.LEVEL_DIMENSION - 1));
                }
                break;
            case 180:
                // Below new docent position
                {
                    var x = px;
                    var y = py+1;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++y < (Consts.LEVEL_DIMENSION - 1));
                }

                // Below old docent position
                {
                    var x = px;
                    if (Player.last_step_to_the_left) {
                        x -= 1;
                    }
                    else
                    {
                        x += 1;
                    }
                    var y = py+1;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(++y < (Consts.LEVEL_DIMENSION - 1));
                }

                // Above old docent position
                {
                    var x = px;
                    if (Player.last_step_to_the_left) {
                        x -= 1;
                    }
                    else
                    {
                        x += 1;
                    }
                    var y = py-1;
                    do 
                    {
                        if (IsWallAt(x, y)) {
                            break;
                        }
                        QueueAmygdalaToEnable(x, y);
                    } while(--y > 0);
                }
                break;
        }

        EnableQueuedAmygdalas();
    }

    static bool IsWallAt(int x, int y) {
        return WorldState.virt[x, y] == 1;
    }

    static void EnableAmygdalaAt(int x, int y) {
        foreach (GameObject amyg in BuildLevel.amygdalaInstances)
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

    public static void QueueAmygdalaToEnable(int x, int y) {
        amygdalasToEnable[amygdalasToEnableIndex++] = BitCoordinates.ToBits(x, y);
    }

    static void EnableQueuedAmygdalas() {
        while(--amygdalasToEnableIndex > -1) {
            (int x, int y) = BitCoordinates.FromBits(amygdalasToEnable[amygdalasToEnableIndex]);
            EnableAmygdalaAt(x, y);
        }
    }

    public static void DisableGravity() {
        //Debug.Log("Gravity OFF");
        foreach (GameObject amyg in BuildLevel.amygdalaInstances)
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

        if (BuildLevel.docentInstance != null) {
            Rigidbody2D rigidDocent = BuildLevel.docentInstance.GetComponent<Rigidbody2D>();
            if (rigidDocent == null)
            {
                return;
            }
            else
            {
                rigidDocent.simulated = false;
            }
        }
    }

    public static void Reset()
    {
        rotationDirection = 0;
        currentAngle = 0;
        lockRotation = false;
        amygdalaMapPositions.Clear();
        obstacleMapPositions.Clear();
        Physics2D.gravity = new Vector3(0.0f, -9.8f, 0.0f);
        //Debug.Log("Gravity On");
    }

    public static int DoLevelDown()
    {
        currentLevel--;
        if (0 == currentLevel)
        {
            currentLevel = 1;
        }
        return currentLevel;
    }

    public static int DoLevelUp()
    {
        currentLevel++;
        if (currentLevel == MAX_LEVEL_NUMBER + 1)
        {
            currentLevel = MAX_LEVEL_NUMBER;
        }
        return currentLevel;
    }

    public static void destroy_amygdala_at(int x, int y) {
        foreach (GameObject amyg in BuildLevel.amygdalaInstances) {
            float ax = amyg.transform.position.x;
            float ay = amyg.transform.position.y;
            float mx = x;
            float my = y;

            if ((System.Math.Abs(ax - mx) < 0.1f) && (System.Math.Abs(ay - my) < 0.1f)) {
                    WorldState.lastAmygdalaPosition = amyg.transform.position;
                    BuildLevel.amygdalaInstances.Remove(amyg);
                    Destroy(amyg, 0.0f);
                    --WorldState.totalAmygdalas;
                    var player = GameObject.FindWithTag("Player").GetComponent<Player>();
                    player.playPickupSound();
                    skipCheckLevelFinished = 8;
                    shouldCheckLevelFinished = true;
                    break;                
            }
        }
   }

    public static void checkLevelFinished() {
        if (skipCheckLevelFinished > 0) {
            --skipCheckLevelFinished;
            return;
        }
        if (WorldState.totalAmygdalas == 0)
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
            ++WorldState.currentLevel;

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
        for (int i = 0; i < Consts.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < Consts.LEVEL_DIMENSION; ++j)
            {
                virt[j, i] = 0;
            }
        }

        foreach (GameObject amyg in BuildLevel.amygdalaInstances)
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

        foreach (GameObject wall in BuildLevel.wallInstances)
        {
            var x = (int)System.Math.Round(wall.transform.position.x);
            var y = (int)System.Math.Round(wall.transform.position.y);
            virt[x, y] = 1;
            wall.transform.position = new Vector3(x, y, 0);
        }

        var px = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.x);
        var py = (int)System.Math.Round(BuildLevel.docentInstance.transform.position.y);
        virt[px, py] = 200;
        BuildLevel.docentInstance.transform.position = new Vector3(px, py, 0);
    }

    public static void debug_print_virtual_level() {
        return; // Inactive

        string lev = System.Environment.NewLine;
        Debug.Log("");
        Debug.Log("---");
        for (int i = 0; i < Consts.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < Consts.LEVEL_DIMENSION; ++j)
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

    void Update()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Game:
                if (shouldCheckLevelFinished) {
                    checkLevelFinished();
                }
                break;
            case WorldState.GameState.Elevator:
                switch (WorldState.currentAngle)
                {
                    case 0:
                        {
                            Vector3 positionChange = new Vector3(0.0f, ELEVATOR_positionChange, 0.0f);
                            BuildLevel.elevatorInstance.transform.position += positionChange;
                        }
                        break;
                    case 180:
                        {
                            Vector3 positionChange = new Vector3(0.0f, -ELEVATOR_positionChange, 0.0f);
                            BuildLevel.elevatorInstance.transform.position += positionChange;
                        }
                        break;
                    case 90:
                        {
                            Vector3 positionChange = new Vector3(-ELEVATOR_positionChange, 0.0f, 0.0f);
                            BuildLevel.elevatorInstance.transform.position += positionChange;
                        }
                        break;
                    case 270:
                        {
                            Vector3 positionChange = new Vector3(+ELEVATOR_positionChange, 0.0f, 0.0f);
                            BuildLevel.elevatorInstance.transform.position += positionChange;
                        }
                        break;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (Counters.elevatorFrames > 0)
        {
            if (Counters.elevatorFrames == 1)
            {
                //Debug.Log("Destroying elevator and going into intermission");
                Destroy(BuildLevel.elevatorInstance);
                Counters.elevatorFrames = 0;
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
        --Counters.elevatorFrames;
    }

    public static void GoBackToTitleScreen() {
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

            Camera.main.orthographic = true;
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.currentAngle));

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

