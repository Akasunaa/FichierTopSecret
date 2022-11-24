using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Singleton component that will be called by other elements that want to count other elements using a tag
 */
public class DuplicationCheckManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static DuplicationCheckManager _instance;
    public static DuplicationCheckManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DuplicationCheckManager();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    /**
     *  Function that will check for the number of targetController-type elements found in the scene
     *  It will send the number as an event that can be suscribed to by the NPC
     */
    public int Search(string tag)
    {
        var targetsFound = GameObject.FindGameObjectsWithTag(tag);
        //print("Numbers of " + tag + " found in scene :" + targetsFound.Length);
        return targetsFound.Length;
    }
}
