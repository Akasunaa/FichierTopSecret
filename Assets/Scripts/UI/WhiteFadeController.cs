using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Controller of the White fade in case of bomb explosion
/// </summary>
public class WhiteFadeController : MonoBehaviour
{
    [SerializeField] private GameObject whiteFadeCanvas;
    [SerializeField] private Image whiteFadeImage;

    private void Awake()
    {
        Assert.IsNotNull(whiteFadeCanvas);
        whiteFadeCanvas.SetActive(false);
    }

    /// <summary>
    /// Function that triggers the white fade.
    /// </summary>
    /// <param name="duration">Duration of the white fade.</param>
    public void StartWhiteFade(float duration)
    {
        StartCoroutine(Fade(duration));
    }

    /// <summary>
    /// Coroutine handling the fade.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator Fade(float duration)
    {
        whiteFadeCanvas.SetActive(true);
        Color color = whiteFadeImage.color;
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime+= Time.deltaTime;
            color.a = elapsedTime / duration;
            whiteFadeImage.color = color;
            yield return null;
        }
        whiteFadeImage.color = Color.black;
    }
}
