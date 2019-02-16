using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intermission : MonoBehaviour
{
    const float HORIZONTAL_OFFSET = 1.65f;

    const int PIECZARA_REVEAL_DELAY = 6;
    const int PIECZARA_REVEAL_STEPS = 8;
    const int NUMBER_REVEAL_DELAY = 50;
    public int pieczara_reveal_counter = 0;
    public int pieczara_reveal_steps = 0;
    public int number_reveal_counter = 0;

    public GameObject flora, flora_instance;
    public GameObject pieczara, pieczara_instance;
    public GameObject pieczarax, pieczarax_instance;

    public GameObject title01_top, title01_bottom;

    public GameObject no01;
    public GameObject cave_number_instance;
    public GameObject title_top_instance, title_bottom_instance;

    public void PerformBuildIntermission()
    {
        WorldState.Reset();
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, WorldState.current_angle));
        flora_instance = Instantiate(
            flora,
            new Vector3(
                BuildLevel.LEVEL_DIMENSION + (BuildLevel.LEVEL_DIMENSION >> 1) + 1.0f + HORIZONTAL_OFFSET,
                BuildLevel.LEVEL_DIMENSION - (BuildLevel.LEVEL_DIMENSION >> 1) - 0.5f, 1),
                Quaternion.identity);

        pieczara_instance = Instantiate(pieczara, new Vector3(2.4f + HORIZONTAL_OFFSET, 8.8f, 1), Quaternion.identity);
        pieczarax_instance = Instantiate(pieczarax, new Vector3(2.4f + HORIZONTAL_OFFSET, 8.8f, 1), Quaternion.identity);

        cave_number_instance = Instantiate(no01, new Vector3(-18.0f, 6.7f, 1), Quaternion.identity);
        //        cave_number_instance = Instantiate(no01, new Vector3(2.0f, 6.7f, 1), Quaternion.identity);

        title_top_instance = Instantiate(title01_top, new Vector3(-29.0f, 3.44f, 1), Quaternion.identity);
        title_bottom_instance = Instantiate(title01_bottom, new Vector3(-18.0f, 2.23f, 1), Quaternion.identity);
        // title_top_instance = Instantiate(title01_top, new Vector3(1.64f, 3.44f, 1), Quaternion.identity);
        // title_bottom_instance = Instantiate(title01_bottom, new Vector3(1.64f, 2.23f, 1), Quaternion.identity);
    }

    public void PerformDestroyIntermission()
    {
        Destroy(flora_instance);
        Destroy(pieczara_instance);
        Destroy(pieczarax_instance);
        Destroy(cave_number_instance);
        Destroy(title_top_instance);
        Destroy(title_bottom_instance);
    }

    void FixedUpdate()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Intermission_Flora_In:
                {
                    Vector3 pos = flora_instance.transform.position;
                    pos.x -= 0.11f;
                    flora_instance.transform.position = pos;
                    if (pos.x < 12.5f + HORIZONTAL_OFFSET)
                    {
                        WorldState.gameState = WorldState.GameState.Intermission_Pieczara_Reveal;
                        pieczara_reveal_counter = PIECZARA_REVEAL_DELAY;
                        pieczara_reveal_steps = PIECZARA_REVEAL_STEPS;
                    }
                }
                break;
            case WorldState.GameState.Intermission_Pieczara_Reveal:
                {
                    if (pieczara_reveal_steps > 0)
                    {
                        if (pieczara_reveal_counter == 1)
                        {
                            Vector3 pos = pieczarax_instance.transform.position;
                            Vector3 scale = pieczarax_instance.transform.localScale;

                            pos.x += 0.5f;
                            scale.x -= 0.126f;

                            pieczarax_instance.transform.position = pos;
                            pieczarax_instance.transform.localScale = scale;

                            --pieczara_reveal_steps;
                            pieczara_reveal_counter = PIECZARA_REVEAL_DELAY;
                        }
                        --pieczara_reveal_counter;
                    }
                    else
                    {
                        number_reveal_counter = NUMBER_REVEAL_DELAY;
                        WorldState.gameState = WorldState.GameState.Intermission_WaitingForNumber;
                    }
                }
                break;
            case WorldState.GameState.Intermission_WaitingForNumber:
                {
                    if (number_reveal_counter > 0)
                    {
                        --number_reveal_counter;
                    }
                    else
                    {
                        Vector3 pos = cave_number_instance.transform.position;
                        pos.x = 2.0f + HORIZONTAL_OFFSET;
                        cave_number_instance.transform.position = pos;
                        WorldState.gameState = WorldState.GameState.Intermission_TopTitle;
                    }
                }
                break;
            case WorldState.GameState.Intermission_TopTitle:
                {
                    Vector3 pos = title_top_instance.transform.position;
                    pos.x += 0.32f;
                    title_top_instance.transform.position = pos;

                    if (pos.x > 1.54f + HORIZONTAL_OFFSET)
                    {
                        WorldState.gameState = WorldState.GameState.Intermission_BottomTitle;
                    }
                }
                break;
            case WorldState.GameState.Intermission_BottomTitle:
                {
                    Vector3 pos = title_bottom_instance.transform.position;
                    pos.x += 0.32f;
                    title_bottom_instance.transform.position = pos;

                    if (pos.x > 1.54f + HORIZONTAL_OFFSET)
                    {
                        WorldState.gameState = WorldState.GameState.Intermission_Done;
                    }
                }
                break;
        }
    }
}
