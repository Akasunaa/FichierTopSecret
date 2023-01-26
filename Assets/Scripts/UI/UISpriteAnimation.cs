using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] spriteArray;
    [SerializeField] private float speed = 50;
    [SerializeField] private float pauseAtEnd;

    private int _indexSprite;

    public void PlayAnim()
    {
        StartCoroutine(PlayAnimCoroutine());
    }

    public void PauseAnim()
    {
        StopCoroutine(PlayAnimCoroutine());
    }

    public void ResetAnim()
    {
        _indexSprite = 0;
        image.sprite = spriteArray[_indexSprite];
    }

    private IEnumerator PlayAnimCoroutine()
    {
        do
        {
            image.sprite = spriteArray[_indexSprite];
            _indexSprite = (_indexSprite + 1) % spriteArray.Length;
            yield return new WaitForSecondsRealtime(_indexSprite == 0 && pauseAtEnd != 0 ? pauseAtEnd : 1/speed);
        } while (true);
        // ReSharper disable once IteratorNeverReturns
    }
}
