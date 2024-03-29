using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class MainTitleController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float speed = 500;
    [SerializeField] private Canvas mainMenuCanvas;

    public UnityEvent onFadedIn;

    private float _currentAlpha;

    private bool _fadedIn;

    private void Awake()
    {
        Assert.IsNotNull(mainMenuCanvas);
        Assert.IsNotNull(canvasGroup);
    }

    private void Start()
    {
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        try {
            IntPtr unityWindow = FilesWatcher.GetActiveWindow();
            if (unityWindow != IntPtr.Zero)
            {
                if (FilesWatcher.GetWindowRect(unityWindow, out FilesWatcher.RECT r))
                {
                    FilesWatcher.MoveWindow(unityWindow, 0, 10,
                        r.Right - r.Left, r.Bottom - r.Top, true);
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(e);
        }
        #endif
        _currentAlpha = 0;
        canvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;
        
        if (_fadedIn)
        {
            mainMenuCanvas.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            StopCoroutine(FadeIn());
            _currentAlpha = 1;
            canvasGroup.alpha = 1;
            onFadedIn.Invoke();
        }
    }

    private IEnumerator FadeIn()
    {
        do
        {
            _currentAlpha = Mathf.Clamp(_currentAlpha + 1 / speed, 0, 1);
            canvasGroup.alpha = _currentAlpha;
            yield return null;
        } while (_currentAlpha is not 1);
        onFadedIn.Invoke();
        _fadedIn = true;
    }
}
