using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Function that will clean the Player Prefs at the start of every session
 *  It is only to be inside the SceneLauncher scene
 */
public class CleanerManager : MonoBehaviour
{
    //Deleting all the playerPrefs
    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }
}
