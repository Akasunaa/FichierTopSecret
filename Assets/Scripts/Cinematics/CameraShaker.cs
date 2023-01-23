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

    private bool _isShaking=false;

    /// <summary>
    /// Function called by external scripts when a camera shake is required.
    /// </summary>
    /// <param name="duration">Duration of the camera shake.</param>
    /// <param name="magnitude">Magnitude of the camera shake.</param>
    /// <returns>True if the shake could be started, false otherwise.</returns>
    public bool CameraShake(float duration, float magnitude)
    {
        if(_isShaking)
        {
            return false;
        }
        else
        {
            StartCoroutine(CameraShakeCoroutine(duration, magnitude));
            return true;
        }
    }

    /// <summary>
    /// Coroutine that handles the camera shake.
    /// </summary>
    /// <param name="duration">Duration of the camera shake.</param>
    /// <param name="magnitude">Magnitude of the camera shake.</param>
    /// <returns></returns>
    private IEnumerator CameraShakeCoroutine(float duration, float magnitude)
    {
        _isShaking = true;

        //We save up the initial data :
        Vector3 originalPos = transform.position;
        float elapsedTime = 0.0f;

        //Main camera shake that consists of an ease-in :
        while (elapsedTime < duration/2)
        {
            float x = Random.Range(-1f, 1f) * magnitude*elapsedTime;
            float y = Random.Range(-1f, 1f) * magnitude*elapsedTime;
            transform.position = originalPos + new Vector3(x, y, originalPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //Main camera shake that consists of an ease-out :
        while (elapsedTime< duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude * (duration - elapsedTime);
            float y = Random.Range(-1f, 1f) * magnitude * (duration - elapsedTime);
            transform.position = originalPos + new Vector3(x, y, originalPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Reset the camera :
        transform.position = originalPos;
        _isShaking = false;
    }
}
