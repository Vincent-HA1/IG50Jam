using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip audioFile;
    public AudioClip audioLoopFile;
    public float volume = 1;
    public bool loop;
}
