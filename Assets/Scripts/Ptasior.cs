using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Ptasior : MonoBehaviour
{
    public static double yacc = 0;
    public AudioClip crow;
    static bool first_voice = false;
    static bool second_voice = false;
    static bool flying = false;
    const int LICZBA_PAPIESKA = 2137;

    public void Fly()
    {
        if (!flying) {
            return;
        }
        var pos = BuildLevel.ptasiorInstance.transform.position;
        pos.x += 0.05f;
        pos.y += (float)yacc;
        BuildLevel.ptasiorInstance.transform.position = pos;
        if (!first_voice  && pos.x > -0.5f && pos.x < (-0.5f+1.5f)) {
            String x = String.Format("should crow {0} {1}", first_voice , pos.x);
            if (BuildLevel.rng.Next(4) >= 1) {
                GetComponent<AudioSource>().PlayOneShot(crow, 1.0f);
            }
            first_voice  = true;
        }
        if (!second_voice  && pos.x > 6.5f && pos.x < (6.5f+1.5f)) {
            String x = String.Format("should crow {0} {1}", first_voice , pos.x);
            if (BuildLevel.rng.Next(4) >= 1) {
                GetComponent<AudioSource>().PlayOneShot(crow, 1.0f);
            }
            second_voice  = true;
        }
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

        if(!flying) {
            var r = BuildLevel.rng.Next(1, 2700); 
            if (r == LICZBA_PAPIESKA) {
                flying = true;
            }
        }

        Fly();
    }

    public static void InitPos() {
        var posy = BuildLevel.rng.NextDouble() * 11;
        var pos = new Vector3((float)-11.5, (float)posy, 0);

        var randomYacc = BuildLevel.rng.NextDouble() * 0.017;
        if (posy > 5.5) {
            Ptasior.yacc = -randomYacc;
        }
        else {
            Ptasior.yacc = randomYacc;
        }
        BuildLevel.ptasiorInstance.transform.position = pos;
        Ptasior.first_voice  = false;
        Ptasior.second_voice  = false;
        Ptasior.flying = false;
    }
}