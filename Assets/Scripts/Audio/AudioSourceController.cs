using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour
{
    [SerializeField] private AudioType type;

    private AudioSource _source;
    
    private void Start()
    {
        _source = gameObject.GetComponent<AudioSource>();
        UpdateSourceParameters();
    }

    public void UpdateSourceParameters()
    {
        switch (type)
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

    private enum AudioType
    {
        Music,
        Sfx
    }
}
