using System;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Timer : MonoBehaviour
{
    public static Timer instance { get; private set; }
    
    private TimerUIController _timerUIController;
    private float _currentTime = 0f;
    private bool _uiExists = false;
    private bool _isRunning = false;
    private bool _isShown = false;

    private void OnSceneUnload(Scene arg0)
    { 
        _timerUIController = null;
        _uiExists = false;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _timerUIController = GameObject.FindGameObjectWithTag("UI").GetComponent<TimerUIController>();
        if (_timerUIController != null) _uiExists = true;
    }

    
    public void PauseSwitchTimer()
    {
        _isRunning = !_isRunning;
    }
    public void SwitchTimerShowState()
    {
        _isShown = !_isShown;
        if(_uiExists) 
            _timerUIController.timerCanvas.gameObject.SetActive(_isShown);
    }
    public void ResetTimer()
    {
        _currentTime = 0f;
    }
    
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void Update()
    {
        if (_uiExists && _isShown)
        {
            _timerUIController.DisplayTime(_currentTime);
        }
        if(_isRunning) 
            _currentTime += Time.deltaTime;
    }
}