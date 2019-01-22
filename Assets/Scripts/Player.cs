using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    public void StepRight()
    {
        Debug.Log("Stepping right!");
        perform_step_right();
    }

    public void StepLeft()
    {
        Debug.Log("Stepping left!");
        preform_step_left();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            preform_step_left();
        } else if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            perform_step_right();
        }
    }

    void perform_step_right()
    {
        Vector3 position_change = new Vector3(1, 0, 0);
        this.gameObject.transform.position += position_change;
    }

    void preform_step_left()
    {
        Vector3 position_change = new Vector3(-1, 0, 0);
        this.gameObject.transform.position += position_change;
    }

    void SetVerticalCollider() {
        Debug.Log("SetVerticalCollider");
        BoxCollider2D x = this.gameObject.GetComponent<BoxCollider2D>();
        x.size = new Vector2(0.6f, 1.0f);
    }

    void SetHorizontalCollider() {
        Debug.Log("SetHorizontalCollider");
        BoxCollider2D x = this.gameObject.GetComponent<BoxCollider2D>();
        x.size = new Vector2(1.0f, 0.6f);
    }
}
