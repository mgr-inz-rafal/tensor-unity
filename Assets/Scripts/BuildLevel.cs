using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuildLevel : MonoBehaviour
{
    public const int LEVEL_DIMENSION = 12;

    public GameObject brick05;
    public GameObject brick06;
    public GameObject brick08;
    public GameObject brick09;
    public GameObject brick10;
    public GameObject brick11;
    public GameObject docent;
    public GameObject amygdala_heart;
    public GameObject location_spot;
    public static GameObject docent_instance;
    public static List<GameObject> amygdalas_instances = new List<GameObject>();
    public static Dictionary<int, (int, int)> map_spots = new Dictionary<int, (int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        TextAsset bindata = Resources.Load("Maps/v1") as TextAsset;
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
                    case 5 + 64 + 64 + 64:
                        Instantiate(brick05, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 6:
                    case 7 + 64 + 64 + 64:
                        Instantiate(brick06, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 8:
                    case 8 + 64 + 64 + 64:
                        Instantiate(brick08, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 9:
                    case 9 + 64 + 64 + 64:
                        Instantiate(brick09, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 10:
                    case 10 + 64 + 64 + 64:
                        Instantiate(brick10, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    case 11:
                    case 11 + 64 + 64 + 64:
                        Instantiate(brick11, new Vector3(j, i, 0), Quaternion.identity);
                        break;
                    default:
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
