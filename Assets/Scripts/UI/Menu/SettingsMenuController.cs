using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;

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
    
    private readonly struct ScreenRatio
    {
        private readonly int _widthFactor;
        private readonly int _heightFactor;

        public ScreenRatio(int wFactor, int hFactor)
        {
            _widthFactor = wFactor;
            _heightFactor = hFactor;
        }

        public int HeightFromWidth(int width)
        {
            return width * _heightFactor / _widthFactor;
        }
    }
    
    private readonly List<Resolution> _resolutions = new();
    private readonly ScreenRatio _screenRatio = new(4, 3);
    private readonly int[] _acceptedScreenWidths = { 800, 1000, 1600 };
    private readonly List<int> _acceptedRefreshRates = new() { 30, 60, 75, 120, 144, 240 };
    private int _currentRefreshRateIndex = -1;
    private int _currentResolutionIndex;

    private AudioMixerManager _mixerManager;
    
    private void Start()
    {
        // RESOLUTION AND REFRESH RATE GUI INITIALIZATION
        
        BuildResolutions();
        
        SetRefreshRateIndex();
        
        SetRefreshRateDropdown();
        
        SetResolutionDropdown();
        
        
        // AUDIO GUI INITIALIZATION
        
        SetMixerManager();
        
        SetSfxUI();
    }

    #region Starting methods
    
    #region Resolution and refresh rate
    
    private void BuildResolutions()
    {
        foreach (var width in _acceptedScreenWidths)
        {
            _resolutions.Add(new Resolution
            {
                width = width,
                height = _screenRatio.HeightFromWidth(width),
                refreshRate = 30 // dummy refresh rate
            });
        }
    }
    private void SetRefreshRateIndex()
    {        
        for (var i = 0; i < _acceptedRefreshRates.Count; i++) 
            if (_acceptedRefreshRates[i] == Screen.currentResolution.refreshRate) 
                _currentRefreshRateIndex = i;

        if (_currentRefreshRateIndex >= 0) return;
        
        _currentRefreshRateIndex = _acceptedRefreshRates.Count;
        _acceptedRefreshRates.Add(Screen.currentResolution.refreshRate);
    }

    private void SetRefreshRateDropdown()
    {
        var options = _acceptedRefreshRates.Select(rate => rate + "Hz").ToList();
        
        refreshRateDropdown.ClearOptions();
        refreshRateDropdown.AddOptions(options);
        refreshRateDropdown.value = _currentRefreshRateIndex;
        refreshRateDropdown.RefreshShownValue();
    }
    
    private void SetResolutionDropdown()
    {
        List<string> options = new();
        for (var i = 0; i < _resolutions.Count; i++)
        {
            var resolution = _resolutions[i];
            var resolutionOption = resolution.width + "x" + resolution.height;
            options.Add(resolutionOption);
            if (resolution.width == Screen.width && resolution.height == Screen.height)
                _currentResolutionIndex = i;
        }
        
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = _currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    #endregion

    #region Audio

    private void SetMixerManager()
    {
        _mixerManager = FindObjectOfType<AudioMixerManager>();
        if(_mixerManager == null) 
            Debug.LogError($"[{name}] No AudioMixerManager found in the scene, changing sound settings wont work.");
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
        var resolution = _resolutions[resolutionIndex];
        _currentResolutionIndex = resolutionIndex;
        Screen.SetResolution(resolution.width, resolution.height, false);
    }

    public void SetRefreshRate(int refreshRateIndex)
    {
        var rate = _acceptedRefreshRates[refreshRateIndex];
        _currentRefreshRateIndex = refreshRateIndex;
        Application.targetFrameRate = rate;
    }

    public void SoundOnTest()
    {
        StopAllCoroutines();
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


    public void ToMainMenu()
    {
        mainMenuCanvas.SetActive(true);
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
