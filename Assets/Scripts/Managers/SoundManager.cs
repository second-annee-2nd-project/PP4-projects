using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   private AudioSource audioSource;
    public AudioSource AudioSource
    {
        get => audioSource;
        set => audioSource = value;
    }
    private float musicVolume = 1f;
    void Update () 
    {
        audioSource.volume = musicVolume;
    }
    
    public void SetVolume(float vol)
    {
        musicVolume = vol;
    }
}
