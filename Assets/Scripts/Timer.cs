using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public static Timer instance { get; private set; }

    public float currentTime = 0f;
    
    public TimerUIController timerUIController;
    private bool _uiExists = false;
    private bool _isRunning = true;
    private bool _isShown = true;

    private void OnSceneUnload(Scene arg0)
    { 
        timerUIController = null;
        _uiExists = false;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        timerUIController = GameObject.FindGameObjectWithTag("UI").GetComponent<TimerUIController>();
        if (timerUIController == null) return;
        
        _uiExists = true;
        timerUIController.timerCanvas.gameObject.SetActive(_isShown);
    }

    
    public void PauseSwitchTimer()
    {
        _isRunning = !_isRunning;
    }
    public void SwitchTimerShowState()
    {
        _isShown = !_isShown;
        if(_uiExists) 
            timerUIController.timerCanvas.gameObject.SetActive(_isShown);
    }
    public void ResetTimer()
    {
        currentTime = 0f;
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
            timerUIController.DisplayTime(currentTime);
        }
        if(_isRunning) 
            currentTime += Time.deltaTime;
    }
}