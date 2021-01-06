using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amygdala : MonoBehaviour
{
    void RotateLeft() {
        this.transform.Rotate(new Vector3(0, 0, 1), Consts.ROTATION_STEP);
    }

    void RotateRight() {
        this.transform.Rotate(new Vector3(0, 0, 1), -Consts.ROTATION_STEP);
    }

}
