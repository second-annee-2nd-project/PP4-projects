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
}
