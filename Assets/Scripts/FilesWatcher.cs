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
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class FilesWatcher : MonoBehaviour
{

    [SerializeField] Material highlightMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material unhighlightMaterial;

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
    private static ConcurrentQueue<(string, bool)> selectedFilesQueue = new ConcurrentQueue<(string, bool)>();

    private bool isGettingCurrentObject;
    FileParser currentHighlightObject;

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

        print("BasePath: " + di.FullName);

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

        #if UNITY_STANDALONE_WIN
        Thread thread = new Thread(() => HighlightSelectedFiles(selectedFilesQueue));
        thread.Start();
        #endif
    }
    
    #if UNITY_STANDALONE_WIN
    private static void HighlightSelectedFiles(ConcurrentQueue<(string, bool)> selectedQueue)
    {
        HashSet<string> selectedFiles = new HashSet<string>();
        HashSet<string> viewFiles = new HashSet<string>();

        string command =
            "$shell = New-Object -ComObject shell.application;foreach($window in $shell.windows()) { foreach($item in $window.Document.SelectedItems()) { $item.Path } }";

        Process compiler = new Process();
        compiler.StartInfo.FileName = "powershell.exe";
        compiler.StartInfo.Arguments = command;
        compiler.StartInfo.UseShellExecute = false;
        compiler.StartInfo.RedirectStandardOutput = true;
        compiler.StartInfo.CreateNoWindow = true;
        while (true)
        {
            compiler.Start();

            string[] paths = compiler.StandardOutput.ReadToEnd().Split('\n');
            foreach (string pathOutput in paths)
            {
                string path = pathOutput.Trim().Replace('\\', '/');
                if (
                    pathOutput.Length >= Application.streamingAssetsPath.Length
                    && path.Substring(0, Application.streamingAssetsPath.Length) ==
                    Application.streamingAssetsPath)
                {
                    string relativePath = RelativePath(path);
                    if (pathToScript.TryGetValue(relativePath, out FileParser fp))
                    {
                        if (!selectedFiles.Contains(relativePath))
                        {
                            // Highlight the object
                            selectedQueue.Enqueue((relativePath, true));
                            selectedFiles.Add(relativePath);
                        }
                        viewFiles.Add(relativePath);
                    }
                }

            }

            foreach (string relativePath in selectedFiles.ToArray())
            {
                if (!viewFiles.Contains(relativePath))
                {
                    if (pathToScript.TryGetValue(relativePath, out FileParser fp))
                    {
                        selectedQueue.Enqueue((relativePath, false));
                    }
                    selectedFiles.Remove(relativePath);
                }
            }
            viewFiles.Clear();

            compiler.WaitForExit();
            compiler.Refresh();
        }
    }
    #endif

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
       print("Changed: " + e.FullPath);
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
        print("Created: " + e.FullPath);
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
        print("Deleted: " + e.FullPath);
        if (!fi.Exists)
        {
            dataQueue.Enqueue(new FileChange(fi, FileChangeType.Delete));
        }
    }


    void Update()
    {
        while (dataQueue.TryDequeue(out FileChange fc))
        {

            string relativePath = RelativePath(fc.fi.FullName);

            switch (fc.type)
            {
                case FileChangeType.New:
                    if (!pathToScript.ContainsKey(relativePath) && relativePath.Length > "/Test/".Length + fc.fi.Directory.Name.Length && relativePath.Substring("/Test/".Length, fc.fi.Directory.Name.Length) == LevelManager.Capitalize(SceneManager.GetActiveScene().name))
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
                            print(relativePath + " has made an impossible delete !");
                        }
                        else
                        {
                            pathToScript.Remove(relativePath);
                        }
                    }
                    break;
            }
        }

        while (selectedFilesQueue.TryDequeue(out (string, bool) a))
        {
            if (pathToScript.TryGetValue(a.Item1, out FileParser fp))
            {
                if (a.Item2)
                {
                    fp.gameObject.GetComponentInChildren<SpriteRenderer>().material = selectedMaterial;
                }
                else
                {
                    fp.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
                }
            }
        }

        #if UNITY_STANDALONE_WIN
        if (!isGettingCurrentObject)
        {
            StartCoroutine(FindForegroundWindow());
        }
        #endif
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
            print("FilesWatcher should not set a FileParser which already exists with the same path: " + relativePath);
        }
        pathToScript.Add(relativePath, fileParser);
    }

    public bool ContainsFile(FileInfo fi)
    {
        return pathToScript.ContainsKey(RelativePath(fi.FullName));
    }

    //Use to search for file open in first plan and highlight the object associated with this file
    IEnumerator FindForegroundWindow()
    {
        isGettingCurrentObject = true;
        IntPtr hWnd = GetForegroundWindow();
        StringBuilder windowName = new StringBuilder(100);
        GetWindowText(hWnd, windowName, 100);
        try
        {
            string objectFileName = Path.GetFileName(windowName.ToString()).Split()[0];
            objectFileName = objectFileName.Replace("*", "");
            Scene scene = SceneManager.GetActiveScene();
            string completObjectPath = "/Test/" + scene.name + "/" + objectFileName; //to be changed
            if (pathToScript[completObjectPath] != currentHighlightObject && currentHighlightObject)
            {
                currentHighlightObject.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
                pathToScript[completObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
                currentHighlightObject = pathToScript[completObjectPath];
            }
            pathToScript[completObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
            currentHighlightObject = pathToScript[completObjectPath];
        }
        catch
        {
            if(currentHighlightObject!=null)
                currentHighlightObject.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
            currentHighlightObject = null;
        }

        yield return new WaitForSeconds(0.5f);
        isGettingCurrentObject = false;
    }

#if UNITY_STANDALONE_WIN

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

#endif

}
