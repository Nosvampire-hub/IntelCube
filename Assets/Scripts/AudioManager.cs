using UnityEngine.Audio;
using System;
using UnityEngine;



public class AudioManager : MonoBehaviour
{
    public SoundLibrary[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (SoundLibrary s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

        }
    }

    // Update is called once per frame
    public void  Play(string name)
    {
        SoundLibrary s = Array.Find(sounds, soundLibrary => soundLibrary.name == name);

        s.source.Play();
    }
}
