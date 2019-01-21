using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildLevel : MonoBehaviour
{
    public GameObject brick00;
    public GameObject docent;
    public static GameObject docent_instance;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset bindata = Resources.Load("Maps/v4") as TextAsset;
        if(bindata == null)
        {
            return;
        }
        for(int i = 0; i < 12; ++i)
        {
            for(int j = 0; j < 12; ++j) {
                WorldState.levelmap[j, 11-i] = bindata.bytes[12*i+j];
                Debug.Log(WorldState.levelmap[j, i]);
            }
        }

        for(int i = 0; i < 12; ++i)
        {
            for(int j = 0; j < 12; ++j) {
                switch(WorldState.levelmap[j, i]) {
                    case 0:
                        break;
                    case 1:
                        docent_instance = Instantiate(docent, new Vector3(j, i, 0), Quaternion.identity);
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
