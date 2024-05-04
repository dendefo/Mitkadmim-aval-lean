using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{

    [SerializeField] private Slider MasterVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;

    [SerializeField] private AudioMixer MasterMixer;



    private void OnEnable()
    {
        float output;
        MasterMixer.GetFloat("MasterVolume",out output);
        MasterVolumeSlider.value = output;


        MasterMixer.GetFloat("MusicVolume", out output);
        MusicVolumeSlider.value = output;

        MasterMixer.GetFloat("SFXVolume", out output);
        SFXVolumeSlider.value = output;
    }

    public void updateMasterVolume(float volume)
    {
        MasterMixer.SetFloat("MasterVolume", volume);
    }
    public void updateMusicVolume(float volume)
    {
        MasterMixer.SetFloat("MusicVolume", volume);
    }
    public void updateSFXVolume(float volume)
    {
        MasterMixer.SetFloat("SFXVolume", volume);
    }
}
