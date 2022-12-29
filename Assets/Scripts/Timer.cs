using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public static Timer instance { get; private set; }

    public float currentTime = 0f;
    
    private TimerUIController _timerUIController;
    private bool _uiExists = false;
    public bool isShown { get; private set; } = true;

    public bool isRunning = true;

    private void OnSceneUnload(Scene arg0)
    { 
        _timerUIController = null;
        _uiExists = false;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _timerUIController = GameObject.FindGameObjectWithTag("UI").GetComponent<TimerUIController>();
        if (_timerUIController == null) return;
        
        _uiExists = true;
        _timerUIController.timerCanvas.gameObject.SetActive(isShown);
    }

    
    public void PauseSwitchTimer()
    {
        isRunning = !isRunning;
    }
    public void SwitchTimerShowState()
    {
        isShown = !isShown;
        if(_uiExists) 
            _timerUIController.timerCanvas.gameObject.SetActive(isShown);
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
        if (_uiExists && isShown)
            _timerUIController.DisplayTime(currentTime);
        
        if(isRunning) 
            currentTime += Time.deltaTime;
    }
}