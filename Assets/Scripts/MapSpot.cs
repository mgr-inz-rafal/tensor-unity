using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            WorldState.current_player_pos = BuildLevel.map_spots[gameObject.GetInstanceID()];
        }
        else if (col.gameObject.CompareTag("Amygdala"))
        {
            //Debug.Log("Crashed into Amygdala with ID = " + col.gameObject.GetInstanceID());
            WorldState.amygdala_map_positions[col.gameObject.GetInstanceID()] = BuildLevel.map_spots[gameObject.GetInstanceID()];
        }
        else if (col.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log("Crashed into Amygdala with ID = " + col.gameObject.GetInstanceID());
            WorldState.obstacle_map_positions[col.gameObject.GetInstanceID()] = BuildLevel.map_spots[gameObject.GetInstanceID()];
        }
    }

}
