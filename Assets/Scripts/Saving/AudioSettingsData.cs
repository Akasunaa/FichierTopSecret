
[System.Serializable]
public class AudioSettingsData
{
    public float sfxLevel;
    public bool sfxMute;
    public float musicLevel;
    public bool musicMute;

    public AudioSettingsData(AudioMixerManager audioMixerManager)
    {
        sfxLevel = audioMixerManager.sfxLevel;
        sfxMute = audioMixerManager.sfxMuted;
        
        musicLevel = audioMixerManager.musicLevel;
        musicMute = audioMixerManager.musicMuted;
    }
}