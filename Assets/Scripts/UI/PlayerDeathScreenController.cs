using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


public class PlayerDeathScreenController : MonoBehaviour
{
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
        
        clickAgainTMP.gameObject.SetActive(false);
        var deathMessageLoreHeight = deathMessageLoreTMP.preferredHeight;
        var deathMessageLoreMaskCanvasHeight = deathMessageLoreMaskCanvas.GetComponent<RectTransform>().rect.height;
        _totalTravelDistance = deathMessageLoreHeight + deathMessageLoreMaskCanvasHeight;
    }

    private void Start()
    {
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

        while(Math.Abs(_totalTravelDistance - currentScrollPosition) > float.Epsilon)
        {
            currentScrollPosition += deathMessageLoreScrollSpeed * Time.deltaTime;
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
        yield return new WaitForSeconds(5);
        
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
        
        // TODO: Need to load the main menu scene and destroy all the DontDestroyOnLoad objects from SceneLauncher scene.
        Debug.LogWarning($"[{name}] Need to load the main menu scene and destroy all the DontDestroyOnLoad objects from SceneLauncher scene.");
    }
}
