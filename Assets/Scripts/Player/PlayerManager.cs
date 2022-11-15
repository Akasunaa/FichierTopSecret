using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 *  Class that will handle the creation of the folder of the player
 */
public class PlayerManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //class must be DontDestroyOnLoad to avoid repeating operations on switching scenes
    }

    private void Start()
    {
        Invoke("InitialisePlayerFolder", 0.1f);
    }

    /**
     *  Function that will create the player folder once
     */
    private void InitialisePlayerFolder()
    {
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test" + "/Player");
        bool directoryExists = di.Exists;
        if (!directoryExists)
        {
            Debug.Log("Create new directory: " + di.FullName);
            di.Create();
        }
    }

}
