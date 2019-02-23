using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void PlayMusic(string name)
    {
        GameObject asso = GameObject.FindWithTag("AudioMarker");
        AudioSource ass = asso.GetComponent<AudioSource>();
        ass.Stop();
        AudioClip ac = Resources.Load<AudioClip>("Sounds/" + name);
        ass.clip = ac;
        ass.Play();
    }
}
