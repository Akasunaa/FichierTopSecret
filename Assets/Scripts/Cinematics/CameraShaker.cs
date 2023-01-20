using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that will be used to handle the shake of the camera.
/// </summary>
public class CameraShaker : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static CameraShaker _instance;
    public static CameraShaker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CameraShaker();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public void CameraShake(float duration, float magnitude)
    {
        StartCoroutine(CameraShakeCoroutine(duration, magnitude));
    }

    private IEnumerator CameraShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(x, y, originalPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
