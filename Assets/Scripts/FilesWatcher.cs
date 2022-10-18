using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Threading;

public class FilesWatcher : MonoBehaviour
{
    public enum FileChangeType
    {
        New,
        Change,
        Delete,
    }

    public struct FileChange
    {
        public FileChangeType type;
        public string path;

        public FileChange(string path, FileChangeType type)
        {
            this.path = path;
            this.type = type;
        }
    }

    // Associate each file path (which already exists in the game) to a FileParser
    private static Dictionary<string, FileParser> pathToScript = new Dictionary<string, FileParser>(10);
    public static FilesWatcher Instance { get; private set; }

    private static ConcurrentQueue<FileChange> dataQueue = new ConcurrentQueue<FileChange>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

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

    static string RelativePath(string absolutePath)
    {
        return absolutePath[Application.streamingAssetsPath.Length..].Replace('\\', '/');
    }

    /*
     * Call when a file is modified
     */
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        FileInfo fi = new FileInfo(e.FullPath);
        string relativePath = RelativePath(fi.FullName);
        Debug.Log("Changed: " + e.FullPath + " | " + relativePath);
        if (fi.Exists)
        {
            dataQueue.Enqueue(new FileChange(relativePath, FileChangeType.Change));
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
        FileInfo fi = new FileInfo(e.FullPath);
        string relativePath = RelativePath(fi.FullName);
        Debug.Log("Deleted: " + e.FullPath + " | " + relativePath);
        if (!fi.Exists)
        {
            dataQueue.Enqueue(new FileChange(relativePath, FileChangeType.Delete));
        }
    }

    void Update()
    {
        while (dataQueue.TryDequeue(out FileChange fc))
        {
            switch (fc.type)
            {
                case FileChangeType.New:
                    break;
                case FileChangeType.Change:
                    if (pathToScript.TryGetValue(fc.path, out FileParser fileParser1))
                    {
                        if (!fileParser1.OnChange(fc.path))
                        {
                            Debug.LogWarning(fc.path + " has made an impossible change !");
                        }
                    }
                    break;

                case FileChangeType.Delete:
                    if (pathToScript.TryGetValue(fc.path, out FileParser fileParser))
                    {
                        if (!fileParser.OnDelete(fc.path))
                        {
                            Debug.LogWarning(fc.path + " has made an impossible delete !");
                        }
                    }
                    break;
            }
        }
    }

    public void Clear()
    {
        pathToScript.Clear();
    }

    public void Set(FileParser fileParser)
    {
        string relativePath = RelativePath(fileParser.filePath);
        pathToScript.Add(relativePath, fileParser);
    }
}
