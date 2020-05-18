using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Animator settingsAnimation;

    public Slider soundFXSlider;
    public Slider bgMusicSlider;

    public float sfxVol;
    public float bgMusicVol;

    private float initBgMusicVol;
    private float initSfxVol;

    public void Start() {
        initBgMusicVol = BGSoundSystem.Instance.GetCurrentVolume();
        initSfxVol = FXSoundSystem.Instance.GetCurrentVolume();
        LoadPreviousSettings();
    }

    private void LoadPreviousSettings() {
        SettingsData data = SaveSystem.LoadSettings();

        sfxVol = data.sfxVol;
        bgMusicVol = data.bgMusicVol;

        soundFXSlider.value = sfxVol;
        bgMusicSlider.value = bgMusicVol;

    }

    public void SaveSettings() {
        initBgMusicVol = BGSoundSystem.Instance.GetCurrentVolume();
        initSfxVol = FXSoundSystem.Instance.GetCurrentVolume();

        sfxVol = soundFXSlider.value;
        bgMusicVol = bgMusicSlider.value;

        SaveSystem.SaveSettings(this);
        HideSettings();
    }

    public void ShowSettings() {
        gameObject.SetActive(true);
        settingsAnimation.SetBool("isOpen", true);
        settingsAnimation.SetBool("isClose", false);
    }

    public void HideSettings() {
        LoadPreviousSettings();
        BGSoundSystem.Instance.SetSoundVolume(initBgMusicVol);
        FXSoundSystem.Instance.SetSoundVolume(initSfxVol);

        settingsAnimation.SetBool("isClose", true);
        settingsAnimation.SetBool("isOpen", false);
    }

    public void LoadControllerSettings() {
        BGSoundSystem.Instance.SetSoundVolume(initBgMusicVol);
        FXSoundSystem.Instance.SetSoundVolume(initSfxVol);
    }

    public void OnBGVolumeChange() {
        BGSoundSystem.Instance.SetSoundVolume(bgMusicSlider.value);
    }
    public void OnSFXVolChange() {
        FXSoundSystem.Instance.SetSoundVolume(soundFXSlider.value);
    }


}
