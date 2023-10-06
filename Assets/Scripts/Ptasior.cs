using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Ptasior : MonoBehaviour
{
    public static double yacc = 0;

    public void Fly()
    {
        var pos = BuildLevel.ptasiorInstance.transform.position;
        pos.x += 0.05f;
        pos.y += (float)yacc;
        BuildLevel.ptasiorInstance.transform.position = pos;
    }

    void Update()
    {
        if (BuildLevel.ptasiorInstance == null)
        {
            return;
        }

        Fly();
    }
}