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
        return;
        var pos = BuildLevel.ptasiorInstance.transform.position;
        pos.x += 0.05f;
        pos.y += (float)yacc;
        BuildLevel.ptasiorInstance.transform.position = pos;
        if (pos.x > 20.0f) {
            Ptasior.InitPos();
        }
    }

    void Update()
    {
        if (BuildLevel.ptasiorInstance == null)
        {
            return;
        }

        Fly();
    }

    public static void InitPos() {
        var posy = BuildLevel.rng.NextDouble() * 11;
//        var pos = new Vector3(-10, (float)posy, 0);

        var pos = new Vector3(5, 5, 0);


        var randomYacc = BuildLevel.rng.NextDouble() * 0.025;
        if (posy > 5.5) {
            Ptasior.yacc = -randomYacc;
        }
        else {
            Ptasior.yacc = randomYacc;
        }
        BuildLevel.ptasiorInstance.transform.position = pos;
    }
}