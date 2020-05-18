using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSoundSystem : SoundSystem
{
    public AudioClip BGGameplay;
    public AudioClip BGGameOver;
    protected new static BGSoundSystem _instance;
    public new static BGSoundSystem Instance { get { return _instance; } }
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
        audioSource.Play();
        SettingsData settings = SaveSystem.LoadSettings();
        SetSoundVolume(settings.bgMusicVol);
    }

    public void PlayBGGamePlay() {
        audioSource.clip = BGGameplay;
        audioSource.PlayDelayed(0.3f);
    }
    public void PlayBGGameOver() {
        audioSource.clip = BGGameOver;
        audioSource.PlayDelayed(1f);
    }
}
