using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Vector3 position_change = new Vector3(-1, 0, 0);
            this.gameObject.transform.position += position_change;
        } else if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            Vector3 position_change = new Vector3(1, 0, 0);
            this.gameObject.transform.position += position_change;
        }
    }
}
