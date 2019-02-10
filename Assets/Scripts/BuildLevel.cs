using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuildLevel : MonoBehaviour
{
    public const int LEVEL_DIMENSION = 12;

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
    public static GameObject docent_instance;
    public static List<GameObject> amygdalas_instances = new List<GameObject>();
    public static Dictionary<int, (int, int)> map_spots = new Dictionary<int, (int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        TextAsset bindata = Resources.Load("Maps/v43") as TextAsset;
        if (bindata == null)
        {
            return;
        }
        for (int i = 0; i < LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < LEVEL_DIMENSION; ++j)
            {
                WorldState.levelmap[j, (LEVEL_DIMENSION - 1) - i] = bindata.bytes[LEVEL_DIMENSION * i + j];
            }
        }

        for (int i = 0; i < LEVEL_DIMENSION; ++i)
        {
            for (int j = 0; j < LEVEL_DIMENSION; ++j)
            {
                GameObject spot = Instantiate(location_spot, new Vector3(j, i, 0), Quaternion.identity);
                map_spots[spot.GetInstanceID()] = (j, i);

                switch (WorldState.levelmap[j, i])
                {
                    case 0:
                        break;
                    case 2:
                        GameObject amygdala_instance = Instantiate(amygdala_heart, new Vector3(j, i, 0), Quaternion.identity);
                        amygdala_instance.tag = "Amygdala";
                        //Debug.Log("Spawning Amygdala with ID=" + amygdala_instance.GetInstanceID());
                        amygdalas_instances.Add(amygdala_instance);
                        break;
                    case 1:
                        docent_instance = Instantiate(docent, new Vector3(j, i, 0), Quaternion.identity);
                        docent_instance.tag = "Player";
                        Player playerScript = docent_instance.GetComponent<Player>();
                        playerScript.SendMessage("stop_animation");
                        break;
                    case 5:
                        Instantiate(brick05, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 5 + 64 + 64 + 64:
                        Instantiate(brick05p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 6:
                        Instantiate(brick06, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 6 + 64 + 64 + 64:
                        Instantiate(brick06p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 7:
                        Instantiate(brick07, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 7 + 64 + 64 + 64:
                        Instantiate(brick07p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 8:
                        Instantiate(brick08, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 8 + 64 + 64 + 64:
                        Instantiate(brick08p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 9:
                        Instantiate(brick09, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 9 + 64 + 64 + 64:
                        Instantiate(brick09p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 10:
                        Instantiate(brick10, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 10 + 64 + 64 + 64:
                        Instantiate(brick10p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 11:
                        Instantiate(brick11, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 11 + 64 + 64 + 64:
                        Instantiate(brick11p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 12:
                        Instantiate(brick12, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 12 + 64 + 64 + 64:
                        Instantiate(brick12p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 13:
                        Instantiate(brick13, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 13 + 64 + 64 + 64:
                        Instantiate(brick13p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 14:
                        Instantiate(brick14, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 14 + 64 + 64 + 64:
                        Instantiate(brick14p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 15:
                        Instantiate(brick15, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 15 + 64 + 64 + 64:
                        Instantiate(brick15p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 16:
                        Instantiate(brick16, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 16 + 64 + 64 + 64:
                        Instantiate(brick16p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 17:
                        Instantiate(brick17, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 17 + 64 + 64 + 64:
                        Instantiate(brick17p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 18:
                        Instantiate(brick18, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 18 + 64 + 64 + 64:
                        Instantiate(brick18p, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 131:
                        GameObject obstacle_instance_01 = Instantiate(obstacle00, new Vector3(j, i, 0), Quaternion.identity);
                        obstacle_instance_01.tag = "Obstacle";
                        amygdalas_instances.Add(obstacle_instance_01);
                        break;
                    case 132:
                        GameObject obstacle_instance_02 = Instantiate(obstacle01, new Vector3(j, i, 0), Quaternion.identity);
                        obstacle_instance_02.tag = "Obstacle";
                        amygdalas_instances.Add(obstacle_instance_02);
                        break;
                    default:
                        Debug.Log("Unsupported map element: " + WorldState.levelmap[j, i]);
                        break;
                }
            }
        }

        ;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
