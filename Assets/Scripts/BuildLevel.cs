using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildLevel : MonoBehaviour
{
    public GameObject brick00;
    public GameObject docent;
    public GameObject leftButton;

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

        Instantiate(docent, new Vector3(1, 1, 0), Quaternion.identity);

        // Make the button 1/4 height of the screen
        int buttonHeight = Screen.height / 4;
        Debug.Log(buttonHeight);
        leftButton.transform.localScale = new Vector3(2, 2, 2);

        Instantiate(leftButton, new Vector3(3, 3, 3), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
