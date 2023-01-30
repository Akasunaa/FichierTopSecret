using System;
using System.IO;
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
        
        CleanFilesAndFolders();
    }

    private static void CleanFilesAndFolders()
    {
        var di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName);
        if (!di.Exists) return;
        
        // remove readonly attributes on cosmicbin items to delete them
        var di2 = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.CosmicbinFolderName);
        if (di2.Exists)
        {
            foreach (var fileName in Directory.GetFiles(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.CosmicbinFolderName))
            {
                try
                {
                    File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        try
        {
            di.Delete(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
