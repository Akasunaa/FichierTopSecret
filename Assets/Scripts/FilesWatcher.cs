using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileUtils;
using UnityEngine;

public class FilesWatcher : MonoBehaviour
{
    // Associate each file path (which already exists in the game) to a FileParser
    private static Dictionary<string, FileParser> pathToScript;

    void Start()
    {
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test");

        if (!di.Exists)
        {
            di.Create();
        }

        Debug.Log("BasePath: " + di.FullName);
        
        FileSystemWatcher watcher = new FileSystemWatcher(di.FullName);
        
        // Open the file explorer of the client
        Application.OpenURL("file:///" + di.FullName); 

        // Watch for everything
        // TODO: maybe remove some filters ??
        watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;

        // Add callbacks to those events
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;

        // Watch only .txt files
        watcher.Filter = "*.txt";
        watcher.IncludeSubdirectories = true;
        // Start the watcher
        watcher.EnableRaisingEvents = true;
    }

    /*
     * Call when a file is modified
     */
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Changed: " + e.FullPath);

        FileInfo fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            if (pathToScript.TryGetValue(e.FullPath, out FileParser fileParser))
            {
                if (!fileParser.OnChange(fi))
                {
                    Debug.LogWarning(e.FullPath + " has made an impossible change !");
                }
            }
        }
    }

    /*
     * Call when a file is created
     */
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Created: " + e.FullPath);
        FileInfo fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            // Create a object from the file if possible
        }
    }

    /*
     * Call when a file is deleted
     */
    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Deleted: " + e.FullPath);
        FileInfo fi = new FileInfo(e.FullPath);
        if (!fi.Exists)
        {
            if (pathToScript.TryGetValue(e.FullPath, out FileParser fileParser))
            {
                // if (!fileParser.OnDelete(fi))
                // {
                //     Debug.LogWarning(e.FullPath + " has made an impossible delete !");
                // }
            }
        }
    }
}
