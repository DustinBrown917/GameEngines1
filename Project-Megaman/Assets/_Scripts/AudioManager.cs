﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private bool masterMuted = false;
    [SerializeField] private bool musicMuted = false;
    [SerializeField] private bool sfxMuted = false;

    [SerializeField] private AudioMixer masterMixer;

    [SerializeField] private string masterVolKey = "master_vol";
    [SerializeField] private string musicVolKey = "music_vol";
    [SerializeField] private string sfxVolKey = "sfx_vol";

    [SerializeField] private string masterMuteKey = "master_mute";
    [SerializeField] private string musicMuteKey = "music_mute";
    [SerializeField] private string sfxMuteKey = "sfx_mute";

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private Toggle masterMute;
    [SerializeField] private Toggle musicMute;
    [SerializeField] private Toggle sfxMute;


    public void OnEnable()
    {
        LoadFromPlayerPrefs();
    }

    public void LoadFromPlayerPrefs()
    {
        SetMasterVol(PlayerPrefs.GetFloat(masterVolKey));
        masterSlider.value = PlayerPrefs.GetFloat(masterVolKey);
        masterMute.isOn = PlayerPrefs.GetInt(masterMuteKey) == 0 ? false : true;
        SetMasterMute(PlayerPrefs.GetInt(masterMuteKey) == 0 ? false : true);

        SetMusicVol(PlayerPrefs.GetFloat(musicVolKey));
        musicSlider.value = PlayerPrefs.GetFloat(musicVolKey);
        musicMute.isOn = PlayerPrefs.GetInt(musicMuteKey) == 0 ? false : true;
        SetMusicMute(PlayerPrefs.GetInt(masterMuteKey) == 0 ? false : true);

        SetSFXVol(PlayerPrefs.GetFloat(sfxVolKey));
        sfxSlider.value = PlayerPrefs.GetFloat(sfxVolKey);
        sfxMute.isOn = PlayerPrefs.GetInt(sfxMuteKey) == 0 ? false : true;
        SetSfxMute(PlayerPrefs.GetInt(masterMuteKey) == 0 ? false : true);
    }

    public void SetMasterVol(float vol)
    {
        masterMixer.SetFloat(masterVolKey, vol);
        PlayerPrefs.SetFloat(masterVolKey, vol);
    }

    public void SetMusicVol(float vol)
    {
        masterMixer.SetFloat(musicVolKey, vol);
        PlayerPrefs.SetFloat(musicVolKey, vol);
    }

    public void SetSFXVol(float vol)
    {
        masterMixer.SetFloat(sfxVolKey, vol);
        PlayerPrefs.SetFloat(sfxVolKey, vol);
    }

    public void SetMasterMute(bool mute)
    {
        if (mute) {
            masterMixer.SetFloat(masterVolKey, -80.0f);
            
        } else {
            masterMixer.SetFloat(masterVolKey, PlayerPrefs.GetFloat(masterVolKey));
        }

        PlayerPrefs.SetInt(masterMuteKey, mute? 1 : 0);
    }

    public void SetMusicMute(bool mute)
    {
        if (mute) {
            masterMixer.SetFloat(musicVolKey, -80.0f);
        }
        else {
            masterMixer.SetFloat(musicVolKey, PlayerPrefs.GetFloat(musicVolKey));
        }

        PlayerPrefs.SetInt(musicMuteKey, mute ? 1 : 0);
    }

    public void SetSfxMute(bool mute)
    {
        if (mute) {
            masterMixer.SetFloat(sfxVolKey, -80.0f);
        } else {
            masterMixer.SetFloat(sfxVolKey, PlayerPrefs.GetFloat(sfxVolKey));
        }

        PlayerPrefs.SetInt(sfxMuteKey, mute ? 1 : 0);
    }
}