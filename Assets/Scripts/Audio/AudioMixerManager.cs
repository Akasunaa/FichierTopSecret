using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMixerManager : Singleton<AudioMixerManager>
{
    public override bool useDontDestroyOnLoad => true;

    public float sfxLevel { get; private set; }
    public bool sfxMuted { get; private set; }
    public float musicLevel { get; private set; }
    public bool musicMuted { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        
        SetSfxLevel(0.3f, false);
        MuteSfx(false, false);
        
        SetMusicLevel(0.77f, false);
        MuteMusic(true, false);
    }

    public void SetSfxLevel(float level, bool update = true)
    {
        sfxLevel = Mathf.Clamp(level, 0f, 1f);
        if(update) UpdateAllSourceParametersInScene();
    }

    public void MuteSfx(bool mute = true, bool update = true)
    {
        if (sfxMuted && mute) return;

        sfxMuted = mute;
        if(update) UpdateAllSourceParametersInScene();
    }

    public void SetMusicLevel(float level, bool update = true)
    {
        musicLevel = Mathf.Clamp(level, 0f, 1f);
        if(update) UpdateAllSourceParametersInScene();
    }

    public void MuteMusic(bool mute = true, bool update = true)
    {
        if (musicMuted && mute) return;
        
        musicMuted = mute;
        if(update) UpdateAllSourceParametersInScene();
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
