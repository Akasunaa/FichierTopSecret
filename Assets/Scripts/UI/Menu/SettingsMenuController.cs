using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject backToThatCanvas;

    [Header("Resolution and Refresh rate")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown refreshRateDropdown;

    [Header("Sound")] 
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip[] soundClips;

    [Header("Music")] 
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip[] musicClips;
    
    
    private void Start()
    {

        AudioSourcesReferencesCheck();
        
        // RESOLUTION AND REFRESH RATE GUI INITIALIZATION

        SetRefreshRateDropdown();
        
        SetResolutionDropdown();
        
        
        // AUDIO GUI INITIALIZATION

        SetSfxUI();
    }

    #region Starting methods
    
    #region Resolution and refresh rate

    private void SetRefreshRateDropdown()
    {
        var options = ScreenValuesManager.instance.acceptedRefreshRates.Select(rate => rate + "Hz").ToList();
        
        refreshRateDropdown.ClearOptions();
        refreshRateDropdown.AddOptions(options);
        refreshRateDropdown.value = ScreenValuesManager.instance.currentRefreshRateIndex;
        refreshRateDropdown.RefreshShownValue();
    }
    
    private void SetResolutionDropdown()
    {
        List<string> options = new();
        for (var i = 0; i < ScreenValuesManager.instance.resolutions.Count; i++)
        {
            var resolution = ScreenValuesManager.instance.resolutions[i];
            var resolutionOption = resolution.width + "x" + resolution.height;
            options.Add(resolutionOption);
            if (resolution.width == Screen.width && resolution.height == Screen.height)
                ScreenValuesManager.instance.currentResolutionIndex = i;
        }
        
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = ScreenValuesManager.instance.currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    #endregion

    #region Audio

    private void AudioSourcesReferencesCheck()
    {
        var testSfx = sfxAudioSource == null;
        var testMusic = musicAudioSource == null;
        if (!testMusic && !testSfx) return;
        
        var sources = FindObjectsOfType<AudioSource>();
        foreach (var source in sources)
        {
            var sourceController = source.GetComponent<AudioSourceController>();
            if (sourceController == null) continue;
            
            var type = sourceController.audioType;
            if (testSfx && type == AudioSourceController.AudioType.Sfx) sfxAudioSource = source;
            else if (testMusic && type == AudioSourceController.AudioType.Music) musicAudioSource = source;
        }
    }
    
    private void SetSfxUI()
    {
        soundToggle.isOn = !AudioMixerManager.instance.sfxMuted;
        soundSlider.value = AudioMixerManager.instance.sfxLevel;
        
        musicToggle.isOn = !AudioMixerManager.instance.musicMuted;
        musicSlider.value = AudioMixerManager.instance.musicLevel;
    }

    #endregion
    
    #endregion

    #region Called by UI methods

    public void SetResolution(int resolutionIndex)
    {
        ScreenValuesManager.instance.SetResolution(resolutionIndex);
    }

    public void SetRefreshRateFromIndex(int refreshRateIndex)
    {
        ScreenValuesManager.instance.SetRefreshRateFromIndex(refreshRateIndex);
    }

    public void SoundOnTest()
    {
        StopAllCoroutines();
        if(musicAudioSource.isPlaying) musicAudioSource.Stop();
        if(sfxAudioSource.isPlaying) sfxAudioSource.Stop();
        StartCoroutine(PlayAudioClipForSec(GetRandomClipFrom(soundClips), sfxAudioSource));
    }
    
    public void SoundOnMute(bool enableSfx)
    {
        AudioMixerManager.instance.MuteSfx(!enableSfx);
    }

    public void SoundOnChangeLevel(float level)
    {
        AudioMixerManager.instance.SetSfxLevel(level);
    }
    
    
    
    public void MusicOnTest()
    {
        StopAllCoroutines();
        if(musicAudioSource.isPlaying) musicAudioSource.Stop();
        if(sfxAudioSource.isPlaying) sfxAudioSource.Stop();
        StartCoroutine(PlayAudioClipForSec(GetRandomClipFrom(musicClips), musicAudioSource));
    }

    public void MusicOnMute(bool enableMusic)
    {
        AudioMixerManager.instance.MuteMusic(!enableMusic);
    }

    public void MusicOnChangeLevel(float level)
    {
        AudioMixerManager.instance.SetMusicLevel(level);
    }

    

    public void BackButton()
    {
        AudioMixerManager.instance.SaveAudioSettings();
        ScreenValuesManager.instance.SaveScreenSettings();
        backToThatCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
    
    #endregion

    #region Private Methods

    private static IEnumerator PlayAudioClipForSec(AudioClip clip, AudioSource source, float time = 5f)
    {
        source.Stop();
        source.PlayOneShot(clip);
        yield return new WaitForSecondsRealtime(time);
        if(source.isPlaying) source.Stop();
    }

    private static AudioClip GetRandomClipFrom(IReadOnlyList<AudioClip> clipList)
    {
        var index = Random.Range(0, clipList.Count);
        return clipList[index];
    }

    #endregion
}
