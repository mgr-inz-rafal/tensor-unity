using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildLevel : MonoBehaviour
{
    System.Random rng = new System.Random();

    public CanvasGroup levelBorders;

    public const int AMYGDALA_SPECIES_COUNT = 8;

    public GameObject brick05;
    public GameObject brick06;
    public GameObject brick07;
    public GameObject brick08;
    public GameObject brick09;
    public GameObject brick10;
    public GameObject brick11;
    public GameObject brick12;
    public GameObject brick13;
    public GameObject brick14;
    public GameObject brick15;
    public GameObject brick16;
    public GameObject brick17;
    public GameObject brick18;
    public GameObject brick05p;
    public GameObject brick06p;
    public GameObject brick07p;
    public GameObject brick08p;
    public GameObject brick09p;
    public GameObject brick10p;
    public GameObject brick11p;
    public GameObject brick12p;
    public GameObject brick13p;
    public GameObject brick14p;
    public GameObject brick15p;
    public GameObject brick16p;
    public GameObject brick17p;
    public GameObject brick18p;
    public GameObject obstacle00;
    public GameObject obstacle01;
    public GameObject docent;
    public GameObject amygdala_heart;
    public GameObject location_spot;
    public GameObject elevator;

    public static bool mapSpotsInstantiated = false;
    public static GameObject elevatorInstance;
    public static GameObject docentInstance;
    public static List<GameObject> wallInstances = new List<GameObject>();
    public static List<GameObject> amygdalaInstances = new List<GameObject>();
    public static Dictionary<int, (int, int)> map_spots = new Dictionary<int, (int, int)>();
    public static Dictionary<WorldState.BorderState, string> border_textures = new Dictionary<WorldState.BorderState, string>()
    {
        {WorldState.BorderState.Neutral, "_neutral"},
        {WorldState.BorderState.Movement, "_movement"},
        {WorldState.BorderState.Rotation, "_rotation"}
    };

    void Start()
    {
        HideBorders();
    }

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void HideBorders()
    {
        levelBorders.alpha = 0.0f;
    }

    public void ShowBorders()
    {
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            {
                string prefix = "left";
                string suffix = border_textures[WorldState.LeftBorderState];
                GameObject border = GameObject.FindWithTag("LeftBorder");
                RawImage ri = border.GetComponent<RawImage>();
                Texture2D texture = (Texture2D)Resources.Load("Borders/" + prefix + suffix);
                ri.texture = texture;
            }

            {
                string prefix = "right";
                string suffix = border_textures[WorldState.RightBorderState];
                GameObject border = GameObject.FindWithTag("RightBorder");
                RawImage ri = border.GetComponent<RawImage>();
                Texture2D texture = (Texture2D)Resources.Load("Borders/" + prefix + suffix);
                ri.texture = texture;
            }
            levelBorders.alpha = 1.0f;
        }
    }

    public void PerformDestroy(bool destroyDocent = true)
    {
        if (destroyDocent)
        {
            Destroy(docentInstance);
        }
        foreach (GameObject g in amygdalaInstances) { Destroy(g); };
        amygdalaInstances.Clear();
        foreach (GameObject g in wallInstances) { Destroy(g); };
        wallInstances.Clear();
        HideBorders();

        WorldState.Reset();
    }

    public void SpawnElevator()
    {
        Vector3 pos = WorldState.lastAmygdalaPosition;
        switch (WorldState.currentAngle)
        {
            case 0:
                pos.y = -3;
                break;
            case 180:
                pos.y = Consts.LEVEL_DIMENSION - 1 + 3;
                break;
            case 90:
                pos.x = Consts.LEVEL_DIMENSION - 1 + 3;
                break;
            case 270:
                pos.x = -3;
                break;
        }
        elevatorInstance = Instantiate(elevator, pos, Quaternion.identity);
        Counters.elevatorFrames = Consts.TOTAL_ELEVATOR_FRAMES;
        switch (WorldState.currentAngle)
        {
            case 90:
            case 270:
                elevatorInstance.transform.Rotate(new Vector3(0, 0, 1), 90);
                break;
        }
    }

    public void PerformBuild()
    {
        GameObject world = GameObject.FindWithTag("WorldMarker");
        SoundManager sm = world.GetComponent<SoundManager>();
        sm.PlayRandomGameplayMusic();

        ShowBorders();

        WorldState.totalAmygdalas = 0;
        TextAsset bindata = Resources.Load("Maps/v" + WorldState.currentLevel) as TextAsset;
        if (bindata == null)
        {
            return;
        }
        for (int i = 0; i < Consts.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < Consts.LEVEL_DIMENSION; ++j)
            {
                WorldState.levelmap[j, (Consts.LEVEL_DIMENSION - 1) - i] = bindata.bytes[Consts.LEVEL_DIMENSION * i + j];
            }
        }

        int amygdala_number = rng.Next(1, AMYGDALA_SPECIES_COUNT + 1);
        for (int i = 0; i < Consts.LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < Consts.LEVEL_DIMENSION; ++j)
            {
                if (mapSpotsInstantiated == false)
                {
                    // TODO: RC: Abandon the "map_spot" approach to physics
                    //GameObject spot = Instantiate(location_spot, new Vector3(j, i, 0), Quaternion.identity);
                    //map_spots[spot.GetInstanceID()] = (j, i);
                }

                switch (WorldState.levelmap[j, i])
                {
                    case 0:
                        break;
                    case 2:
                        WorldState.totalAmygdalas++;
                        GameObject amygdala_instance = Instantiate(amygdala_heart, new Vector3(j, i, 0), Quaternion.identity);
                        amygdala_instance.tag = "Amygdala";

                        SpriteRenderer amygdala_renderer = amygdala_instance.GetComponent<SpriteRenderer>();
                        amygdala_renderer.sprite = Resources.Load<Sprite>("Amygdalas/amygdala" + amygdala_number);

                        //Debug.Log("Spawning Amygdala at " + j + "," + i);
                        amygdalaInstances.Add(amygdala_instance);
                        break;
                    case 1:
                        docentInstance = Instantiate(docent, new Vector3(j, i, 0), Quaternion.identity);
                        docentInstance.tag = "Player";
                        Player playerScript = docentInstance.GetComponent<Player>();
                        playerScript.SendMessage("stop_animation");
                        break;
                    case 5:
                        wallInstances.Add(Instantiate(brick05, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 5 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick05p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 6:
                        wallInstances.Add(Instantiate(brick06, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 6 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick06p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 7:
                        wallInstances.Add(Instantiate(brick07, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 7 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick07p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 8:
                        wallInstances.Add(Instantiate(brick08, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 8 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick08p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 9:
                        wallInstances.Add(Instantiate(brick09, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 9 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick09p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 10:
                        wallInstances.Add(Instantiate(brick10, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 10 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick10p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 11:
                        wallInstances.Add(Instantiate(brick11, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 11 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick11p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 12:
                        wallInstances.Add(Instantiate(brick12, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 12 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick12p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 13:
                        wallInstances.Add(Instantiate(brick13, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 13 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick13p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 14:
                        wallInstances.Add(Instantiate(brick14, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 14 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick14p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 15:
                        wallInstances.Add(Instantiate(brick15, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 15 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick15p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 16:
                        wallInstances.Add(Instantiate(brick16, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 16 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick16p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 17:
                        wallInstances.Add(Instantiate(brick17, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 17 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick17p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 18:
                        wallInstances.Add(Instantiate(brick18, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 18 + 64 + 64 + 64:
                        wallInstances.Add(Instantiate(brick18p, new Vector3(j, i, 0), Quaternion.identity));
                        break;
                    case 131:
                        GameObject obstacle_instance_01 = Instantiate(obstacle00, new Vector3(j, i, 0), Quaternion.identity);
                        obstacle_instance_01.tag = "Obstacle";
                        amygdalaInstances.Add(obstacle_instance_01);
                        break;
                    case 132:
                        GameObject obstacle_instance_02 = Instantiate(obstacle01, new Vector3(j, i, 0), Quaternion.identity);
                        obstacle_instance_02.tag = "Obstacle";
                        amygdalaInstances.Add(obstacle_instance_02);
                        break;
                    default:
                        //Debug.Log("Unsupported map element: " + WorldState.levelmap[j, i]);
                        break;
                }
            }
        }
        mapSpotsInstantiated = true;
        //Debug.Log("Total amygdalas in this level:" + WorldState.totalAmygdalas);
    }

    void FixedUpdate()
    {
        if (WorldState.gameState == WorldState.GameState.Game)
        {
            --Counters.movementWarmupCounter;
            if (Counters.movementWarmupCounter < 0)
            {
                Counters.movementWarmupCounter = 0;
            }
        }
    }
}
