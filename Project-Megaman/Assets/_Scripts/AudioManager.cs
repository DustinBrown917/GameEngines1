using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField, Tooltip("Is this manager used to initialize the audio system?")] private bool isInitializer = false;
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
        if (!PlayerPrefs.HasKey(masterMuteKey)) {
            Debug.Log("Initializing");
            InitializePlayerPrefs();
        }
        LoadFromPlayerPrefs();        
    }

    public void Start()
    {
        if (isInitializer) {
            LoadFromPlayerPrefs();
            gameObject.SetActive(false);
        }
    }

    public void InitializePlayerPrefs()
    {
        PlayerPrefs.SetInt(masterMuteKey, 1);
        PlayerPrefs.SetInt(musicMuteKey, 1);
        PlayerPrefs.SetInt(sfxMuteKey, 1);
    }

    public void LoadFromPlayerPrefs()
    {
        SetMasterVol(PlayerPrefs.GetFloat(masterVolKey));
        masterSlider.value = PlayerPrefs.GetFloat(masterVolKey);
        masterMute.isOn = PlayerPrefs.GetInt(masterMuteKey) == 0 ? true : false;
        SetMasterMute(PlayerPrefs.GetInt(masterMuteKey) == 0 ? true : false);

        SetMusicVol(PlayerPrefs.GetFloat(musicVolKey));
        musicSlider.value = PlayerPrefs.GetFloat(musicVolKey);
        musicMute.isOn = PlayerPrefs.GetInt(musicMuteKey) == 0 ? true : false;
        SetMusicMute(PlayerPrefs.GetInt(musicMuteKey) == 0 ? true : false);

        SetSFXVol(PlayerPrefs.GetFloat(sfxVolKey));
        sfxSlider.value = PlayerPrefs.GetFloat(sfxVolKey);
        sfxMute.isOn = PlayerPrefs.GetInt(sfxMuteKey) == 0 ? true : false;
        SetSfxMute(PlayerPrefs.GetInt(sfxMuteKey) == 0 ? true : false);
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
        mute = !mute;
        if (mute) {
            masterMixer.SetFloat(masterVolKey, -80.0f);
            
        } else {
            masterMixer.SetFloat(masterVolKey, PlayerPrefs.GetFloat(masterVolKey));
        }

        PlayerPrefs.SetInt(masterMuteKey, mute? 1 : 0);
    }

    public void SetMusicMute(bool mute)
    {
        mute = !mute;
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
        mute = !mute;
        if (mute) {
            masterMixer.SetFloat(sfxVolKey, -80.0f);
        } else {
            masterMixer.SetFloat(sfxVolKey, PlayerPrefs.GetFloat(sfxVolKey));
        }

        PlayerPrefs.SetInt(sfxMuteKey, mute ? 1 : 0);
    }
}
