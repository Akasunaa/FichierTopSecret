using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour
{
    
    [SerializeField] private AudioType type = AudioType.Music;
    
    public AudioType audioType
    {
        get => type;
        set
        {
            type = value;
            if(_source != null) UpdateSourceParameters();
        }
    }

    private AudioSource _source;

    private void Start()
    {
        _source = gameObject.GetComponent<AudioSource>();
        UpdateSourceParameters();
    }

    public void UpdateSourceParameters()
    {
        switch (audioType)
        {
            case AudioType.Music:
                _source.volume = AudioMixerManager.instance.musicLevel;
                _source.mute = AudioMixerManager.instance.musicMuted;
                break;
            case AudioType.Sfx:
                _source.volume = AudioMixerManager.instance.sfxLevel;
                _source.mute = AudioMixerManager.instance.sfxMuted;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public enum AudioType
    {
        Music,
        Sfx
    }
}
