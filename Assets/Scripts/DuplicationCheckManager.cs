using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component that will check wether or not a targetController script has been duplicated in the game
 *  Since all objects will only have ONE ModifiableController, we check for it
 *  It then sends an event with the number of objects found
 */
public class DuplicationCheckManager : MonoBehaviour
{
    public string targetTag;
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
        print("Numbers of " + tag + " found in scene :" + targetsFound.Length);
        return targetsFound.Length;
        //var targetsFound = GameObject.FindObjectsOfType(type);
        //print("Numbers of "+ targetController.name +" found in scene :"+targetsFound.Length);
        //return targetsFound.Length;
    }
}
