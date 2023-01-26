using System.Collections;
using UnityEngine;

public class UIElementBlinking : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float speed = 200;

    private bool _increaseAlpha;
    private float _currentAlpha;

    private void OnEnable()
    {
        _currentAlpha = 0;
        canvasGroup.alpha = 0;
    }

    public void StartBlinking()
    {
        canvasGroup.alpha = 0;
        _increaseAlpha = true;
        StartCoroutine(Blinking());
    }

    private IEnumerator Blinking()
    {
        while (true)
        {
            _currentAlpha =  Mathf.Clamp(_currentAlpha + (_increaseAlpha ? 1 : -1) / speed, 0, 1);
            canvasGroup.alpha = _currentAlpha;
            if (_currentAlpha is 1 or 0) _increaseAlpha = !_increaseAlpha;
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
}
