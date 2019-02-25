using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    System.Random rng = new System.Random();

    bool firstLaunch = true;

    static Dictionary<int, (string, int)> ingame_music_map = new Dictionary<int, (string, int)>()
    {
        {0, ("Ingame", 54)},
        {1, ("Ingame", 41)},
        {2, ("Ingame", 0)},
        {3, ("Title", 0)},
        {4, ("Title", 32)},
        {5, ("Title", 60+46)},
        {6, ("Menu", 0)},
        {7, ("Menu", 59)},
    };

    public void PlayMusic(string name, int offset = 0)
    {
        GameObject asso = GameObject.FindWithTag("AudioMarker");
        AudioSource ass = asso.GetComponent<AudioSource>();
        ass.Stop();
        AudioClip ac = Resources.Load<AudioClip>("Sounds/" + name);
        ass.clip = ac;
        ass.time = offset;
        ass.Play();
    }

    public void PlayRandomGameplayMusic()
    {
        int musindex = rng.Next(0, ingame_music_map.Count);
        if (firstLaunch)
        {
            firstLaunch = false;
            musindex = 0;
        }
        PlayMusic(ingame_music_map[musindex].Item1, ingame_music_map[musindex].Item2);
    }
}
