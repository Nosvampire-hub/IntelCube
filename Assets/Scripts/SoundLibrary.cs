using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class SoundLibrary{
    public AudioClip clip;

    public string name;

    public float volume;
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
