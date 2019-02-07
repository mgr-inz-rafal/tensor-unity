using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuildLevel : MonoBehaviour
{
    public const int LEVEL_DIMENSION = 12;

    public GameObject brick00;
    public GameObject docent;
    public GameObject amygdala_heart;
    public GameObject location_spot;
    public static GameObject docent_instance;
    public static List<GameObject> amygdalas_instances = new List<GameObject>();
    public static Dictionary<int, (int, int)> map_spots = new Dictionary<int, (int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        TextAsset bindata = Resources.Load("Maps/v17") as TextAsset;
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
                    default:
                        Instantiate(brick00, new Vector3(j, i, 0), Quaternion.identity);
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
