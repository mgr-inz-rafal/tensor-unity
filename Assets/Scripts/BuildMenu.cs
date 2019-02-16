using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
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
        ShowNavigationButtons();
    }

    void FixedUpdate()
    {
    }
}
