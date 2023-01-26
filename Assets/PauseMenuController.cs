using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private Canvas settingsMenuCanvas;
    [SerializeField] private TMP_Text clickAgainTMP;
    [SerializeField] private Canvas[] canvasToManageOnPause;

    private GameObject _uiConstant;
    private bool _buttonPressedOnce;
    
    public bool gamePaused { get; private set; }

    private bool isInSettings => settingsMenuCanvas.gameObject.activeInHierarchy;
    
    private void Awake()
    {
        Assert.IsNotNull(pauseMenuCanvas);
        Assert.IsNotNull(clickAgainTMP);
        
        _uiConstant = GameObject.FindGameObjectWithTag("UI-CONSTANT");
        Assert.IsNotNull(_uiConstant);
        clickAgainTMP.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isInSettings)
        {
            if(gamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        PauseOrResume(false);
    }

    
    public void PauseGame()
    {
        PauseOrResume(true);
    }

    public void GoToSettings()
    {
        settingsMenuCanvas.gameObject.SetActive(true);
        pauseMenuCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Made to reset the state of the return to main menu button.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetButtonPressedOnceToFalse()
    {
        clickAgainTMP.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(5);
        
        clickAgainTMP.gameObject.SetActive(false);
        _buttonPressedOnce = false;
    }

    /// <summary>
    /// Method called whenever the back to main menu (B2MM) button is pressed.
    /// </summary>
    public void OnB2MMButtonPressed()
    {
        if (!_buttonPressedOnce)
        {
            _buttonPressedOnce = true;
            StartCoroutine(ResetButtonPressedOnceToFalse());
            return;
        }

        StopCoroutine(ResetButtonPressedOnceToFalse());
        _buttonPressedOnce = false;
        clickAgainTMP.gameObject.SetActive(false);

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    private void PauseOrResume(bool pause)
    {
        gamePaused = pause;
        
        Time.timeScale = pause ? 0 : 1;
        foreach (var canvas in canvasToManageOnPause)
        {
            canvas.gameObject.SetActive(!pause);
        }
        
        _uiConstant.gameObject.SetActive(!pause);
        
        pauseMenuCanvas.gameObject.SetActive(pause);
    }
}
