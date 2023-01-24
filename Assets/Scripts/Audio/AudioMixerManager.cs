using UnityEngine;

public class AudioMixerManager : Singleton<AudioMixerManager>
{
    public override bool useDontDestroyOnLoad => true;

    public float sfxLevel { get; private set; }
    public bool sfxMuted { get; private set; }
    public float musicLevel { get; private set; }
    public bool musicMuted { get; private set; }

    public void SetSfxLevel(float level)
    {
        sfxLevel = Mathf.Clamp(level, 0f, 1f);
        UpdateAllSourceParametersInScene();
    }

    public void MuteSfx(bool mute = true)
    {
        if (sfxMuted && mute) return;

        sfxMuted = mute;
        UpdateAllSourceParametersInScene();
    }

    public void SetMusicLevel(float level)
    {
        musicLevel = Mathf.Clamp(level, 0f, 1f);
        UpdateAllSourceParametersInScene();
    }

    public void MuteMusic(bool mute = true)
    {
        if (musicMuted && mute) return;
        
        musicMuted = mute;
        UpdateAllSourceParametersInScene();
    }

    private static void UpdateAllSourceParametersInScene()
    {
        var sourceList = FindObjectsOfType<AudioSourceController>();
        foreach (var audioSource in sourceList)
        {
            audioSource.UpdateSourceParameters();
        }
    }
}
