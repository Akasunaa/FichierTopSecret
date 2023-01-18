using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class PlayerDeathScreenController : MonoBehaviour
{
    [Header("To Disable on GameOver")] 
    [SerializeField] private List<GameObject> canvasToDisable;
    [SerializeField] private List<GameObject> toDisableSceneSpecific;

    [Header("GameOver screen canvas stuff")] 
    [SerializeField] private GameObject gameOverScreenCanvas;
    [SerializeField] private TextMeshProUGUI deathMessageLoreTMP;
    [SerializeField] private Canvas deathMessageLoreMaskCanvas;
    [SerializeField] private TextMeshProUGUI deathReasonTMP;
    [SerializeField] private TextMeshProUGUI clickAgainTMP;
    [SerializeField] private float deathMessageLoreScrollSpeed = 20f;

    private float _totalTravelDistance;
    private bool _buttonPressedOnce;
    
    private void Awake()
    {
        Assert.IsNotNull(deathMessageLoreTMP);
        Assert.IsNotNull(deathReasonTMP);
        Assert.IsNotNull(clickAgainTMP);
        Assert.IsNotNull(deathMessageLoreMaskCanvas);
        Assert.IsNotNull(gameOverScreenCanvas);
        
        clickAgainTMP.gameObject.SetActive(false);
        gameOverScreenCanvas.SetActive(false);
        
        var deathMessageLoreHeight = deathMessageLoreTMP.preferredHeight;
        var deathMessageLoreMaskCanvasHeight = deathMessageLoreMaskCanvas.GetComponent<RectTransform>().rect.height;
        _totalTravelDistance = deathMessageLoreHeight + deathMessageLoreMaskCanvasHeight;
    }

    /// <summary>
    /// Method you need to call when the game is over. Will disable all the canvas of the UI except of the Game Over canvas.
    /// </summary>
    public void OnGameOver()
    {
        Time.timeScale = 0;
        
        foreach (var canvas in canvasToDisable)
        {
            canvas.SetActive(false);
        }

        foreach (var localObject in toDisableSceneSpecific)
        {
            localObject.SetActive(false);
        }

        foreach (var localObject in GameObject.FindGameObjectsWithTag("DontDestroyOnLoad"))
        {
            Destroy(localObject);
        }

        gameOverScreenCanvas.SetActive(true);
        StartCoroutine(ScrollDeathMessageLore());
    }

    /// <summary>
    /// Start the scrolling effect of the message containing lore on what happens after the game is over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollDeathMessageLore()
    {
        var currentPos = deathMessageLoreTMP.rectTransform.localPosition;
        var currentScrollPosition = - deathMessageLoreMaskCanvas.GetComponent<RectTransform>().rect.height/2;

        while(_totalTravelDistance - currentScrollPosition > float.Epsilon)
        {
            currentScrollPosition += deathMessageLoreScrollSpeed * Time.unscaledDeltaTime;
            currentPos.y = currentScrollPosition;
            deathMessageLoreTMP.transform.localPosition = currentPos;
            yield return null;
        }
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

        Debug.Log($"should do the thing to load main menu");
        StopCoroutine(ResetButtonPressedOnceToFalse());
        _buttonPressedOnce = false;
        clickAgainTMP.gameObject.SetActive(false);

        Time.timeScale = 1;
        Debug.Log($"Loading main menu scene");
        SceneManager.LoadScene("MainMenu");
        
        // TODO: Need to load the main menu scene and destroy all the DontDestroyOnLoad objects from SceneLauncher scene.
        //Debug.LogWarning($"[{name}] Need to load the main menu scene and destroy all the DontDestroyOnLoad objects from SceneLauncher scene.");
    }
}
