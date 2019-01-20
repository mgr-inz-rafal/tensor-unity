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
        for(int i = 0; i < 12; ++i)
        {
            Instantiate(brick00, new Vector3(i, 0, 0), Quaternion.identity);
            Instantiate(brick00, new Vector3(i, 11, 0), Quaternion.identity);
        }
        for(int i = 1; i < 11; ++i)
        {
            Instantiate(brick00, new Vector3(0, i, 0), Quaternion.identity);
            Instantiate(brick00, new Vector3(11, i, 0), Quaternion.identity);
        }

        docent_instance = Instantiate(docent, new Vector3(1, 1, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
