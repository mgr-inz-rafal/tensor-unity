using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    const int TOTAL_CREDIT_LINES = 3;
    const float CREDITS_FADE_STEP = 0.02f;
    const int CREDITS_PRESENTATION_DELAY = 98;
    public int credits_presentation_counter = 0;
    public int current_credit_line = 1;

    public CanvasGroup menuNavigation;

    public GameObject splashScreen;
    public GameObject menu;
    public GameObject credits;

    public GameObject splashScreen_instance;
    public GameObject credits_instance;

    public void HideNavigationButtons()
    {
        menuNavigation.alpha = 0.0f;
        menuNavigation.blocksRaycasts = false;
    }

    public void ShowNavigationButtons()
    {
        menuNavigation.alpha = 1.0f;
        menuNavigation.blocksRaycasts = true;
    }

    public void Start()
    {
        splashScreen_instance = Instantiate(splashScreen, new Vector3(5.5f, 5.5f, 100), Quaternion.identity);
        HideNavigationButtons();
    }

    public void PerformMenu()
    {
        splashScreen_instance = Instantiate(menu, new Vector3(5.5f, 5.5f, 100), Quaternion.identity);
        credits_instance = Instantiate(credits, new Vector3(5.5f, 0.96f, 100), Quaternion.identity);
        credits_instance.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0);
        ShowNavigationButtons();

        GameObject world = GameObject.FindWithTag("WorldMarker");
        SoundManager sm = world.GetComponent<SoundManager>();
        sm.PlayMusic("Menu");
    }

    void SwitchToNextCredit()
    {
        ++current_credit_line;
        if (current_credit_line > TOTAL_CREDIT_LINES)
        {
            current_credit_line = 1;
        }

        SpriteRenderer credits_renderer = credits_instance.GetComponent<SpriteRenderer>();
        credits_renderer.sprite = Resources.Load<Sprite>("Credits/credits" + current_credit_line);
    }

    void FixedUpdate()
    {
        switch (WorldState.gameState)
        {
            case WorldState.GameState.Menu:
                {
                    switch (WorldState.creditState)
                    {
                        case WorldState.CreditState.FadeIn:
                            {
                                Color col = credits_instance.GetComponent<SpriteRenderer>().material.color;
                                col.a += CREDITS_FADE_STEP;
                                credits_instance.GetComponent<Renderer>().material.color = col;
                                if (col.a > 1.0f)
                                {
                                    WorldState.creditState = WorldState.CreditState.Presenting;
                                    credits_presentation_counter = CREDITS_PRESENTATION_DELAY;
                                }
                            }
                            break;
                        case WorldState.CreditState.FadeOut:
                            {
                                Color col = credits_instance.GetComponent<SpriteRenderer>().material.color;
                                col.a -= CREDITS_FADE_STEP;
                                credits_instance.GetComponent<Renderer>().material.color = col;
                                if (col.a < 0.0f)
                                {
                                    SwitchToNextCredit();
                                    WorldState.creditState = WorldState.CreditState.FadeIn;
                                    credits_presentation_counter = CREDITS_PRESENTATION_DELAY;
                                }
                            }
                            break;
                        case WorldState.CreditState.Presenting:
                            --credits_presentation_counter;
                            if (0 == credits_presentation_counter)
                            {
                                WorldState.creditState = WorldState.CreditState.FadeOut;
                            }
                            break;
                    }
                }
                break;
        }
    }
}
