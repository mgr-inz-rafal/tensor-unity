using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amygdala : MonoBehaviour
{
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
