using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    [SerializeField]
    private Sound[] audios;
    [SerializeField]
    private AudioSource audioSource;
     

    public void PlaySound(string name) 
    {
        Sound s = Array.Find(audios, item => item.name == name);
        audioSource.clip = s.audioClip;
        audioSource.Play();
    }

}
[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip audioClip;
}
