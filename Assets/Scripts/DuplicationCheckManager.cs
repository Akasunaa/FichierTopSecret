using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component that will check wether or not its associated gameObject has been duplicated in the game
 */
public class DuplicationCheckManager : MonoBehaviour
{
    [SerializeField] private ModifiableController targetController;
    private Type type;

    private void Awake()
    {
        type = targetController.GetType();
    }

    private void Start()
    {
        Search();
    }

    private void Search()
    {
        var targetsFound = GameObject.FindObjectsOfType(type);
        print("Numbers of "+ targetController.name +" found in scene :"+targetsFound.Length);
    }
}
