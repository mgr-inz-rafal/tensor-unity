using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public void OnClick_RightArrow()
    {
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();       
        if(playerScript == null)
        {
            Debug.Log("Unable to find Player");
        }
        else
        {
            playerScript.SendMessage("StepRight");
        }
    }

    public void OnClick_LeftArrow()
    {
        Player playerScript = BuildLevel.docent_instance.GetComponent<Player>();       
        if(playerScript == null)
        {
            Debug.Log("Unable to find Player");
        }
        else
        {
            playerScript.SendMessage("StepLeft");
        }
        
    }
}
