[System.Serializable]
public class SettingsData
{
    public float sfxVol = 1;
    public float bgMusicVol = 0.3f;


    public SettingsData(Settings settings) {
        sfxVol = settings.sfxVol;
        bgMusicVol = settings.bgMusicVol;
    }
    public SettingsData() {

    }
    public SettingsData(SettingsData settings) {
        sfxVol = settings.sfxVol;
        bgMusicVol = settings.bgMusicVol;
    }
}
