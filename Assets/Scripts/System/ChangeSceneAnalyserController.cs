using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChangeSceneAnalyserController
{
    //#region SINGLETON PATTERN
    //private static ChangeSceneAnalyserController _instance;
    //public static ChangeSceneAnalyserController Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new ChangeSceneAnalyserController();
    //        }
    //        return _instance;
    //    }
    //}
    //private void Awake()
    //{
    //    _instance = this;
    //}
    //#endregion

    private static bool loadedThroughDoor;

    //private void Start()
    //{
    //    DontDestroyOnLoad(this);
    //}

    public static void SetLoadingState(bool hasLoadedThroughDoor)
    {
        loadedThroughDoor = hasLoadedThroughDoor;
    }

    public static bool GetLoadingState()
    {
        return loadedThroughDoor;
    }
}
