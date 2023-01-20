using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


public class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreenCanvas;
    [SerializeField] private GameOverScreenData[] gosDataList;
        
    [Space(10)]
    [SerializeField] private TextMeshProUGUI titleTMP;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI gameOverReasonTMP;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI clickAgainTMP;
    
    [Header("End game lore")]
    [SerializeField] private TextMeshProUGUI gameOverMessageLoreTMP;
    [SerializeField] private Canvas gameOverMessageLoreMaskCanvas;
    [SerializeField] private float gameOverMessageLoreScrollSpeed = 7f;
    
    [Header("To Disable on GameOver")] 
    [SerializeField] private List<GameObject> canvasToDisable;
    [SerializeField] private List<GameObject> toDisableSceneSpecific;

    
    private float _totalTravelDistance;
    private bool _buttonPressedOnce;
    
    private void Awake()
    {
        Assert.IsNotNull(gameOverMessageLoreTMP);
        Assert.IsNotNull(gameOverReasonTMP);
        Assert.IsNotNull(clickAgainTMP);
        Assert.IsNotNull(gameOverMessageLoreMaskCanvas);
        Assert.IsNotNull(gameOverScreenCanvas);
        
        clickAgainTMP.gameObject.SetActive(false);
        gameOverScreenCanvas.SetActive(false);
        
        var deathMessageLoreHeight = gameOverMessageLoreTMP.preferredHeight;
        var deathMessageLoreMaskCanvasHeight = gameOverMessageLoreMaskCanvas.GetComponent<RectTransform>().rect.height;
        _totalTravelDistance = deathMessageLoreHeight + deathMessageLoreMaskCanvasHeight;
    }

    /// <summary>
    /// Method you need to call when the game is over. Will disable all the canvas of the UI except of the Game Over canvas.
    /// </summary>
    /// <param name="gameOverType"></param>
    public void OnGameOver(GameOverType gameOverType)
    {
        Time.timeScale = 0;
        
        foreach (var canvas in canvasToDisable) canvas.SetActive(false);
        
        foreach (var localObject in toDisableSceneSpecific) localObject.SetActive(false);
        
        foreach (var localObject in GameObject.FindGameObjectsWithTag("DontDestroyOnLoad")) Destroy(localObject);
        

        UpdateGameOverScreen(gameOverType);
        ActivateGameOverScreen();
    }

    private void UpdateGameOverScreen(GameOverType gameOverType)
    {
        GameOverScreenData test = new();
        foreach (var screenData in gosDataList)
        {
            if (screenData.reason != gameOverType) continue;
            test = screenData;
            break;
        }

        if (test.reason == (GameOverType)(-1)) return;

        if (test.title != "") titleTMP.SetText(test.title);
        if(test.reasonText != "") gameOverReasonTMP.SetText(test.reasonText);
        if(test.lore != "") gameOverMessageLoreTMP.SetText(test.lore);
    }


    private void ActivateGameOverScreen()
    {
        gameOverScreenCanvas.SetActive(true);
        StartCoroutine(ScrollDeathMessageLore());
    }

    /// <summary>
    /// Start the scrolling effect of the message containing lore on what happens after the game is over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollDeathMessageLore()
    {
        var currentPos = gameOverMessageLoreTMP.rectTransform.localPosition;
        var currentScrollPosition = - gameOverMessageLoreMaskCanvas.GetComponent<RectTransform>().rect.height/2;

        while(_totalTravelDistance - currentScrollPosition > float.Epsilon)
        {
            currentScrollPosition += gameOverMessageLoreScrollSpeed * Time.unscaledDeltaTime;
            currentPos.y = currentScrollPosition;
            gameOverMessageLoreTMP.transform.localPosition = currentPos;
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

        StopCoroutine(ResetButtonPressedOnceToFalse());
        _buttonPressedOnce = false;
        clickAgainTMP.gameObject.SetActive(false);

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public enum GameOverType
    {
        Victory = 0,
        BombDetonated,
        PlayerIsDead
    }
}
