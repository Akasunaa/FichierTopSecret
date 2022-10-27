using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class FilesWatcher : MonoBehaviour
{

    DirectoryInfo dicri = new DirectoryInfo(Application.streamingAssetsPath + "/Test/ScenePOC1");
    public enum FileChangeType
    {
        New,
        Change,
        Delete,
    }

    public struct FileChange
    {
        public FileChangeType type;
        public FileInfo fi;

        public FileChange(FileInfo fi, FileChangeType type)
        {
            this.fi = fi;
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

        //Debug.Log("BasePath: " + di.FullName);

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

        //IsTooCool();
        IsCooler();
    }

    public static string RelativePath(string absolutePath)
    {
        return absolutePath[Application.streamingAssetsPath.Length..].Replace('\\', '/');
    }

    /*
     * Call when a file is modified
     */
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        FileInfo fi = new FileInfo(e.FullPath);
        //Debug.Log("Changed: " + e.FullPath);
        if (fi.Exists)
        {
            dataQueue.Enqueue(new FileChange(fi, FileChangeType.Change));
        }
    }

    /*
     * Call when a file is created
     */
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        FileInfo fi = new FileInfo(e.FullPath);
        //Debug.Log("Created: " + e.FullPath);
        if (fi.Exists)
        {
            // Create a object from the file if possible
            dataQueue.Enqueue(new FileChange(fi, FileChangeType.New));
        }
    }

    /*
     * Call when a file is deleted
     */
    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        FileInfo fi = new FileInfo(e.FullPath);
        //Debug.Log("Deleted: " + e.FullPath);
        if (!fi.Exists)
        {
            dataQueue.Enqueue(new FileChange(fi, FileChangeType.Delete));
        }
    }

    private void IsTooCool()
    {
        Process[] processes = Process.GetProcessesByName("notepad");

        for (int i = 0; i < processes.Length; i++)
        {
            //print(processes[i].Handle);
            IntPtr handleTest = FindWindow(processes[i].ProcessName, null);
            MoveWindow(handleTest, 0, 0,0,0,false);
            StringBuilder windowName = new StringBuilder();
            GetWindowText(handleTest, windowName, 10);
            print("the pute one " + windowName);
        }
    }

    private void IsCooler()
    {
        IntPtr hWnd = GetForegroundWindow(); // Get foreground window handle
        StringBuilder windowName = new StringBuilder(100);
        GetWindowText(hWnd, windowName, 50);
        print("the pute one " + windowName);
    }
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    private void IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                print("miaou");
                stream.Close();

            }
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            print("arg");
        }
    }


    void Update()
    {

        foreach (var fi in dicri.EnumerateFiles())
        {
            IsFileLocked(fi);
        }

        while (dataQueue.TryDequeue(out FileChange fc))
        {

            string relativePath = RelativePath(fc.fi.FullName);

                switch (fc.type)
            {
                case FileChangeType.New:
                    if (!pathToScript.ContainsKey(relativePath))
                    {
                        LevelManager.Instance.NewObject(fc.fi);
                    }
                    break;
                case FileChangeType.Change:
                    if (pathToScript.TryGetValue(relativePath, out FileParser fileParser1))
                    {
                        if (!fileParser1.OnChange(relativePath))
                        {
                            //Debug.LogWarning(relativePath + " has made an impossible change !");
                        }
                    }

                    break;

                case FileChangeType.Delete:
                    if (pathToScript.TryGetValue(relativePath, out FileParser fileParser))
                    {
                        if (!fileParser.OnDelete(relativePath))
                        {
                            //Debug.LogWarning(relativePath + " has made an impossible delete !");
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
        if (pathToScript.ContainsKey(relativePath))
        {
            //Debug.LogError("FilesWatcher should not set a FileParser which already exists with the same path: " + relativePath);
        }
        pathToScript.Add(relativePath, fileParser);
    }

    public bool ContainsFile(FileInfo fi)
    {
        return pathToScript.ContainsKey(RelativePath(fi.FullName));
    }
}
