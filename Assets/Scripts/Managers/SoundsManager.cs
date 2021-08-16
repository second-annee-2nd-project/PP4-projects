using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundsManager : MonoBehaviour
{ 
    [SerializeField] private AudioMixer master;
    [SerializeField] private AudioMixer sfx;
    
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;
    
   [SerializeField] private AudioSource sfxAudioSource;
    public AudioSource AudioSource
    {
        get => sfxAudioSource;
        set => sfxAudioSource = value;
    }
    void Start()
    {
        if (sfxAudioSource == null)
            return;
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 10);
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 10);
    }
    public void SetVolumeMaster(float volume)
    {
        master.SetFloat("masterVolume", volume);
        // PlayerPrefs.SetFloat("masterVolume",volume);
        // PlayerPrefs.Save();
    }
    
    public void SetVolumeSfx(float volume)
    {
        sfx.SetFloat("sfxVolume", volume);
        // PlayerPrefs.SetFloat("sfxVolume", volume);
        // PlayerPrefs.Save();
    }
    
    private void OnDisable()
    {
        float sfxVolume = 0;
        float masterVolume = 0;
    
        master.GetFloat("masterVolume", out masterVolume);
        sfx.GetFloat("sfxVolume", out sfxVolume);
    
        PlayerPrefs.SetFloat("masterVolume",masterVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
