using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    // These are different properties for adding audios dynamically in the inspector

    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 1;
    [Range(0.1f, 3f)]
    public float Pitch = 1;
    public bool Loop;

    [HideInInspector]
    public AudioSource Source;

    // Static function for avoiding code redundancy when initializing sounds
    public static void InitializeSounds(GameObject gameObject, Sound[] backgroundMusic)
    {
        foreach (Sound sound in backgroundMusic)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

}
