using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSoundSystem : SoundSystem
{
    public AudioClip clickSound;

    protected new static FXSoundSystem _instance;
    public new static FXSoundSystem Instance { get { return _instance; } }



    // Start is called before the first frame update
    public override void Awake() {

        if (_instance == null) {
            _instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
            OnStart();
        } else {
            audioSource = _instance.gameObject.GetComponent<AudioSource>();
        }

    }

    public override void OnStart() {
        base.OnStart();
        SettingsData settings = SaveSystem.LoadSettings();
        SetSoundVolume(settings.sfxVol);
    }
    public void PlayClickSound() {
        audioSource.PlayOneShot(clickSound, 1f);
    }

}
