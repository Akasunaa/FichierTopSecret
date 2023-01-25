using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenValuesManager : Singleton<ScreenValuesManager>
{
    public override bool useDontDestroyOnLoad => true;
    
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
    
    private readonly ScreenRatio _screenRatio = new(4, 3);
    private readonly int[] _acceptedScreenWidths = { 800, 1000, 1200, 1400, 1600, 1800 };
    
    public List<Resolution> resolutions { get; } = new();
    public int currentResolutionIndex { get; set; }

    public List<int> acceptedRefreshRates { get; } = new() { 30, 60, 75, 120, 144, 240 };
    public int currentRefreshRateIndex { get; private set; } = -1;
    
    
    protected override void OnAwake()
    {
        base.OnAwake();
        LoadScreenSettings();
    }

    private void Start()
    {
        BuildResolutions();
    }
    
    private void LoadScreenSettings()
    {
        var data = SaveSystem.LoadScreenSettingsData();
        
        SetRefreshRate(data?.refreshRate ?? 0);
        
        if (data == null) return;
        
        SetResolution(data.resolution[0], data.resolution[1]);
    }
    
    public void SaveScreenSettings()
    {
        Debug.Log(acceptedRefreshRates[currentRefreshRateIndex]);
        SaveSystem.SaveScreenSettingsData(Screen.width, Screen.height, acceptedRefreshRates[currentRefreshRateIndex]);
    }
    
    private void BuildResolutions()
    {
        foreach (var width in _acceptedScreenWidths)
        {
            resolutions.Add(new Resolution
            {
                width = width,
                height = _screenRatio.HeightFromWidth(width),
                refreshRate = 30 // dummy refresh rate
            });
        }
    }

    private void SetRefreshRateIndex(int rate = 0)
    {
        var testValue = rate == 0 ? Screen.currentResolution.refreshRate : rate; 
        for (var i = 0; i < acceptedRefreshRates.Count; i++) 
            if (acceptedRefreshRates[i] == testValue) 
                currentRefreshRateIndex = i;

        if (currentRefreshRateIndex >= 0) return;
        
        currentRefreshRateIndex = acceptedRefreshRates.Count;
        acceptedRefreshRates.Add(Screen.currentResolution.refreshRate);
    }
    
    private void SetRefreshRate(int rate)
    {
        SetRefreshRateIndex(rate);
        Application.targetFrameRate = rate;
        QualitySettings.vSyncCount = 0;
    }
    
    private static void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false);
    }
    
    public void SetRefreshRateFromIndex(int refreshRateIndex)
    {
        var rate = acceptedRefreshRates[refreshRateIndex];
        currentRefreshRateIndex = refreshRateIndex;
        Application.targetFrameRate = rate;
    }
    
    public void SetResolution(int resolutionIndex)
    {
        var resolution = resolutions[resolutionIndex];
        currentResolutionIndex = resolutionIndex;
        Screen.SetResolution(resolution.width, resolution.height, false);
        Invoke(nameof(ResetPosition), 0.05f);
    }

    private void ResetPosition()
    {
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        IntPtr unityWindow = FilesWatcher.GetActiveWindow();
        if (unityWindow != IntPtr.Zero)
        {
            if (FilesWatcher.GetWindowRect(unityWindow, out FilesWatcher.RECT r))
            {
                FilesWatcher.MoveWindow(unityWindow, 0, 50,
                    r.Right - r.Left, r.Bottom - r.Top, true);
            }
        }
        #endif
    }
}