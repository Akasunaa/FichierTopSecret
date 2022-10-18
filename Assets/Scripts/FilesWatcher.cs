using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileProperty;
using UnityEngine;

public class FilesWatcher : MonoBehaviour
{
    // private Dictionary<string, FileLink> pathToScript;

    void Start()
    {
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test");

        if (!di.Exists)
        {
            di.Create();
        }

        Debug.Log("BasePath: " + di.FullName);
        
        FileSystemWatcher watcher = new FileSystemWatcher(di.FullName);
        
        Application.OpenURL("file:///" + di.FullName); 

        watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;

        watcher.Filter = "*.txt"; // Maybe a need to change this
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Changed: " + e.FullPath);

        FileInfo fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            // Read the file and parse the values

            FileChange fc = new FileChange();
            fc.path = e.FullPath;
            fc.type = FileChangeType.Change;
            
            // Call a function to FileLink : pathToScript[e.FullPath]
            fs.Close();
        }
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Created: " + e.FullPath);
        FileInfo fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            // Read the file
            
            FileChange fc = new FileChange();
            fc.path = e.FullPath;
            fc.type = FileChangeType.New;
            
            // Call a function to FileLink : pathToScript[e.FullPath]
            fs.Close();
        }
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Debug.Log("Renamed: " + e.FullPath + " from " + e.OldFullPath);
        FileInfo fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            // Read the file

            // TODO: I don't know how to handle renamed event
            
            // Call a function to FileLink : pathToScript[e.FullPath]
            fs.Close();
        }
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Debug.Log("Deleted: " + e.FullPath);
        FileInfo fi = new FileInfo(e.FullPath);
        if (!fi.Exists)
        {
            FileChange fc = new FileChange();
            fc.path = e.FullPath;
            fc.type = FileChangeType.Delete;

            // Call a function to FileLink : pathToScript[e.FullPath]
        }
    }
}
