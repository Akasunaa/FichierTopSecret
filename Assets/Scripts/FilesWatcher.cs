using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class FilesWatcher : MonoBehaviour
{

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unhighlightMaterial;

    public enum FileChangeType
    {
        New,
        Change,
        Delete
    }

    public struct FileChange
    {
        public FileChangeType Type;
        public FileInfo Fi;

        public FileChange(FileInfo fi, FileChangeType type)
        {
            Fi = fi;
            Type = type;
        }
    }

    // Associate each file path (which already exists in the game) to a FileParser
    private static Dictionary<string, FileParser> _pathToScript = new Dictionary<string, FileParser>(10);
    public static FilesWatcher instance { get; private set; }

    private static ConcurrentQueue<FileChange> _dataQueue = new ConcurrentQueue<FileChange>();
    private static ConcurrentQueue<(string, bool)> _selectedFilesQueue = new ConcurrentQueue<(string, bool)>();

    private bool _isGettingCurrentObject;
    private FileParser _currentHighlightObject;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this;
        }
    }

    private void Start()
    {
        var di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName);

        if (!di.Exists)
        {
            di.Create();
        }

        Debug.Log("[FileWatcher] BasePath: " + di.FullName);

        var watcher = new FileSystemWatcher(di.FullName);

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
        var thread = new Thread(() => HighlightSelectedFiles(_selectedFilesQueue));
        thread.Start();
        #endif
    }
    
    #if UNITY_STANDALONE_WIN
    private static void HighlightSelectedFiles(ConcurrentQueue<(string, bool)> selectedQueue)
    {
        var selectedFiles = new HashSet<string>();
        var viewFiles = new HashSet<string>();

        var command =
            "$shell = New-Object -ComObject shell.application;foreach($window in $shell.windows()) { foreach($item in $window.Document.SelectedItems()) { $item.Path } }";

        var compiler = new Process();
        compiler.StartInfo.FileName = "powershell.exe";
        compiler.StartInfo.Arguments = command;
        compiler.StartInfo.UseShellExecute = false;
        compiler.StartInfo.RedirectStandardOutput = true;
        compiler.StartInfo.CreateNoWindow = true;
        while (true)
        {
            compiler.Start();

            var paths = compiler.StandardOutput.ReadToEnd().Split('\n');
            foreach (var pathOutput in paths)
            {
                var path = pathOutput.Trim().Replace('\\', '/');
                if (
                    pathOutput.Length >= Application.streamingAssetsPath.Length
                    && path.Substring(0, Application.streamingAssetsPath.Length) ==
                    Application.streamingAssetsPath)
                {
                    var relativePath = RelativePath(path);
                    if (_pathToScript.TryGetValue(relativePath, out var fp))
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

            foreach (var relativePath in selectedFiles.ToArray())
            {
                if (!viewFiles.Contains(relativePath))
                {
                    if (_pathToScript.TryGetValue(relativePath, out var fp))
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
        var fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            Debug.Log("[FileWatcher] File Changed: " + e.FullPath);
            _dataQueue.Enqueue(new FileChange(fi, FileChangeType.Change));
        }
        else // Looks weird, but the OnDelete is never triggered when the root folder is deleted so...
        {
            Debug.Log("[FileWatcher] OnChanged File Deleted: " + e.FullPath);
            _dataQueue.Enqueue(new FileChange(fi, FileChangeType.Delete));
        }
    }

    /*
     * Call when a file is created
     */
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        var fi = new FileInfo(e.FullPath);
        if (fi.Exists)
        {
            Debug.Log("[FileWatcher] File Created: " + e.FullPath);
            // Create a object from the file if possible
            _dataQueue.Enqueue(new FileChange(fi, FileChangeType.New));
        }
        else
        {
            Debug.LogError("[FileWatcher] Create a file that does not exists");
        }
    }

    /*
     * Call when a file is deleted
     */
    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        var fi = new FileInfo(e.FullPath);
        if (!fi.Exists)
        {
            Debug.Log("[FileWatcher] File Deleted: " + e.FullPath);
            _dataQueue.Enqueue(new FileChange(fi, FileChangeType.Delete));
        }
        else
        {
            Debug.LogError("[FileWatcher] Delete a file that still exists");
        }
    }

    private void Update()
    {
        while (_dataQueue.TryDequeue(out FileChange fc))
        {
            var relativePath = RelativePath(fc.Fi.FullName);

            switch (fc.Type)
            {
                case FileChangeType.New:
                    var levelName = LevelManager.Capitalize(SceneManager.GetActiveScene().name);
                    var alreadyExists = _pathToScript.ContainsKey(relativePath);
                    var rightDirectory =
                        LevelManager.Capitalize(relativePath.Substring(("/" + Utils.RootFolderName + "/").Length, levelName.Length)) == levelName;
                    if (!alreadyExists && relativePath.Length >= ("/" + Utils.RootFolderName + "/").Length + levelName.Length && rightDirectory)
                    {
                        Debug.Log("[FileWatcher] Trying to create new object from " + relativePath);
                        LevelManager.Instance.NewObject(fc.Fi);
                    }
                    else if (alreadyExists)
                    {
                        Debug.Log("[FileWatcher] Object " + relativePath + " already exists (it is normal if it was already in the scene)");
                    }
                    else {
                        Debug.LogWarning("[FileWatcher] File " + relativePath + " is in the wrong directory");
                    }
                    break;
                case FileChangeType.Change:
                    if (_pathToScript.TryGetValue(relativePath, out var fileParser1))
                    {
                        Debug.Log("[FileWatcher] Applying change to " + relativePath);
                        if (!fileParser1.OnChange(relativePath))
                        {
                            //Debug.LogWarning(relativePath + " has made an impossible change !");
                        }
                    }

                    break;

                case FileChangeType.Delete:
                    if (_pathToScript.TryGetValue(relativePath, out var fileParser))
                    {
                        if (!fileParser.OnDelete(relativePath))
                        {
                            // Debug.Log("[FileWatcher]" + relativePath + " should not be deleted !");
                        }
                        else
                        {
                            _pathToScript.Remove(relativePath);
                        }
                    }
                    break;
            }
        }

        while (_selectedFilesQueue.TryDequeue(out (string, bool) a))
        {
            if (_pathToScript.TryGetValue(a.Item1, out FileParser fp))
            {
                fp.gameObject.GetComponentInChildren<SpriteRenderer>().material = a.Item2 ? selectedMaterial : unhighlightMaterial;
            }
        }

        #if UNITY_STANDALONE_WIN
        if (!_isGettingCurrentObject)
        {
            StartCoroutine(FindForegroundWindow());
        }
        #endif
    }

    public void Clear()
    {
        _pathToScript.Clear();
    }

    public void Set(FileParser fileParser)
    {
        var relativePath = RelativePath(fileParser.filePath);
        if (_pathToScript.ContainsKey(relativePath))
        {
            Debug.LogError("FilesWatcher should not set a FileParser which already exists with the same path: " + relativePath);
        }
        _pathToScript.Add(relativePath, fileParser);
    }

    public bool ContainsFile(FileInfo fi)
    {
        return _pathToScript.ContainsKey(RelativePath(fi.FullName));
    }

    //Use to search for file open in first plan and highlight the object associated with this file
    IEnumerator FindForegroundWindow()
    {
        _isGettingCurrentObject = true;
        var hWnd = GetForegroundWindow();
        var windowName = new StringBuilder(100);
        GetWindowText(hWnd, windowName, 100);
        try
        {
            var objectFileName = Path.GetFileName(windowName.ToString()).Split()[0];
            objectFileName = objectFileName.Replace("*", "");
            var scene = SceneManager.GetActiveScene();
            var completeObjectPath = "/" + Utils.RootFolderName + "/" + scene.name + "/" + objectFileName; //to be changed
            if (_pathToScript[completeObjectPath] != _currentHighlightObject && _currentHighlightObject)
            {
                _currentHighlightObject.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
                _pathToScript[completeObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
                _currentHighlightObject = _pathToScript[completeObjectPath];
            }
            _pathToScript[completeObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
            _currentHighlightObject = _pathToScript[completeObjectPath];
        }
        catch
        {
            if(_currentHighlightObject!=null)
                _currentHighlightObject.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
            _currentHighlightObject = null;
        }

        yield return new WaitForSeconds(0.5f);
        _isGettingCurrentObject = false;
    }

    public Dictionary<string, FileParser> GetPathToScript()
    {
        return _pathToScript;
    }


#if UNITY_STANDALONE_WIN

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

#endif

}
