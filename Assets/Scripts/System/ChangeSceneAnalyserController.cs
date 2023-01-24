using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class that will conserve data as to how the player changed scene (door or not).
/// </summary>
public class ChangeSceneAnalyserController : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static ChangeSceneAnalyserController _instance;
    public static ChangeSceneAnalyserController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChangeSceneAnalyserController();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private bool loadedThroughDoor;

    /// <summary>
    /// Function that will store the loading state of the scene change.
    /// </summary>
    /// <param name="hasLoadedThroughDoor">true if the scene was loaded through a door.</param>
    public void SetLoadingState(bool hasLoadedThroughDoor)
    {
        loadedThroughDoor = hasLoadedThroughDoor;
    }

    /// <summary>
    /// Function that will obtain the scene's loading state
    /// </summary>
    /// <returns>True if scene was loaded by door.</returns>
    public bool GetLoadingState()
    {
        return loadedThroughDoor;
    }
}
