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
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class FilesWatcher : MonoBehaviour
{

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unhighlightMaterial;

    private bool pathInExplorer = false;
    public IntPtr explorerHwnd = IntPtr.Zero;
    private bool isVibrating = false;

    private static string supportedExtensions = "(.txt|.bat)$";

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
    
    // Tells whether there is an explorer in the streaming asset directory already open
    private static ConcurrentQueue<(bool, IntPtr)> _explorerPathsQueue = new ConcurrentQueue<(bool, IntPtr)>();

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

    private void OnDestroy()
    {
        if (instance == this) instance = null;
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
        // Application.OpenURL("file:///" + di.FullName);

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

        // Watch all 
        watcher.Filter = "*.*"; 
        watcher.IncludeSubdirectories = true;
        // Start the watcher
        watcher.EnableRaisingEvents = true;

        #if UNITY_STANDALONE_WIN
        var thread = new Thread(() => HighlightSelectedFilesAndGetExplorer(_selectedFilesQueue, _explorerPathsQueue));
        thread.Start();
        #endif
    }
    
    #if UNITY_STANDALONE_WIN
    private static void HighlightSelectedFilesAndGetExplorer(ConcurrentQueue<(string, bool)> selectedQueue, ConcurrentQueue<(bool, IntPtr)> explorerPathQueue)
    {
        var selectedFiles = new HashSet<string>();
        var viewFiles = new HashSet<string>();
        
        bool inExplorer = false;

        var command =
            "$shell = New-Object -ComObject shell.application;foreach($window in $shell.windows()) { foreach($item in $window.Document.SelectedItems()) { $item.Path } }";

        var compiler = new Process();
        compiler.StartInfo.FileName = "powershell.exe";
        compiler.StartInfo.Arguments = command;
        compiler.StartInfo.UseShellExecute = false;
        compiler.StartInfo.RedirectStandardOutput = true;
        compiler.StartInfo.CreateNoWindow = true;
        
        var command2 =
            "$shell = New-Object -ComObject shell.application;foreach($window in $shell.windows()) { $window.LocationURL+ '|' + $window.HWND }";

        var compiler2 = new Process();
        compiler2.StartInfo.FileName = "powershell.exe";
        compiler2.StartInfo.Arguments = command2;
        compiler2.StartInfo.UseShellExecute = false;
        compiler2.StartInfo.RedirectStandardOutput = true;
        compiler2.StartInfo.CreateNoWindow = true;
        while (true)
        {
            compiler.Start();

            var paths = compiler.StandardOutput.ReadToEnd().Split('\n');
            foreach (string pathOutput in paths)
            {
                var path = pathOutput.Trim().Replace('\\', '/');
                if (
                    pathOutput.Length >= Application.streamingAssetsPath.Length
                    && path.Substring(0, Application.streamingAssetsPath.Length) ==
                    Application.streamingAssetsPath)
                {
                    var relativePath = Utils.RelativePath(path);
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

            compiler2.Start();
            bool checkExplorer = false;
            IntPtr hwnd = IntPtr.Zero;
            var explorerPaths = compiler2.StandardOutput.ReadToEnd().Split('\n');
            foreach (string pathOutput in explorerPaths)
            {
                var data = pathOutput.Trim().Replace('\\', '/').Split('|');
                var path = data[0];
                if (pathOutput != "")
                {
                    int minIndex = Mathf.Min(8, path.Length);
                    string dirPath = path.Substring(minIndex, path.Length - minIndex);
                    if (dirPath.Substring(0, Mathf.Min(dirPath.Length, Application.streamingAssetsPath.Length))
                        == Application.streamingAssetsPath)
                    {
                        checkExplorer = true;
                        hwnd = new IntPtr(Convert.ToInt32(data[1]));
                    }
                }
            }

            if (checkExplorer != inExplorer)
            {
                explorerPathQueue.Enqueue((checkExplorer, hwnd));
                inExplorer = checkExplorer;
            }

            compiler2.WaitForExit();
            compiler2.Refresh();
        }
    }
    #endif

    /*
     * Call when a file is modified
     */
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        string extension = Path.GetExtension(e.FullPath);
        if (!Regex.IsMatch(extension, supportedExtensions, RegexOptions.IgnoreCase))
            return;       
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
        string extension = Path.GetExtension(e.FullPath);
        if (!Regex.IsMatch(extension, supportedExtensions, RegexOptions.IgnoreCase))
            return;
        var fi = new FileInfo(e.FullPath);
        if (fi.Exists )
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
        string extension = Path.GetExtension(e.FullPath);
        if (!Regex.IsMatch(extension, supportedExtensions, RegexOptions.IgnoreCase))
            return;
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
            var relativePath = Utils.RelativePath(fc.Fi.FullName);

            switch (fc.Type)
            {
                case FileChangeType.New:
                    string levelName = LevelManager.Capitalize(SceneManager.GetActiveScene().name);
                    bool alreadyExists = _pathToScript.ContainsKey(relativePath);
                    string sceneName = Utils.SceneName(relativePath);
                    bool rightDirectory = levelName == sceneName;
                    if (!alreadyExists && relativePath.Length >= ("/" + Utils.RootFolderName + "/").Length + levelName.Length && rightDirectory)
                    {
                        Debug.Log("[FileWatcher] Trying to create new object from " + relativePath);
                        LevelManager.Instance.NewObject(fc.Fi);
                    }
                    else if (alreadyExists)
                    {
                        Debug.Log("[FileWatcher] Object " + relativePath + " already exists (it is normal if it was already in the scene)");
                    }
                    else if (Utils.SceneName(relativePath) == Utils.PlayerFolderName)
                    {
                        Debug.Log("[FileWatcher] Object " + relativePath + " is in player pocket");
                        LevelManager.Instance.NewObject(fc.Fi, isItem: true);
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
                            StartCoroutine(VibrateExplorer());
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

        #if UNITY_STANDALONE_WIN
        while (_selectedFilesQueue.TryDequeue(out (string, bool) a))
        {
            if (_pathToScript.TryGetValue(a.Item1, out FileParser fp))
            {
                print("FP: " + fp);
                print("FP GAMEOBJECT" + fp.gameObject);
                SpriteRenderer srenderer = fp.gameObject.GetComponentInChildren<SpriteRenderer>();
                if (srenderer == null) { break; }
                if (a.Item2)
                {
                    fp.gameObject.GetComponentInChildren<SpriteRenderer>().material = selectedMaterial;
                    foreach (SpriteRenderer sr in fp.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    {
                        sr.material.SetFloat("_numberOfSprite", sr.sprite.texture.width / sr.sprite.rect.width);
                    }
                }
                else
                {
                    fp.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
                }
            }
        }

        while (_explorerPathsQueue.TryDequeue(out (bool, IntPtr) data))
        {
            bool inExplorer = data.Item1;
            IntPtr hwnd = data.Item2;
            GameObject uiObject = GameObject.FindGameObjectWithTag("UI");
            if (uiObject != null && uiObject.TryGetComponent(out ExplorerUIController explorerUiController))
            {
                if (inExplorer)
                {
                    explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color =
                        new Color(1f, 1f, 1f, 0.4f);
                }
                else
                {
                    Color c = explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color;
                    explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color =
                        new Color(1f, 1f, 1f, 1f);
                }
            }
            else
            {
                Debug.LogWarning("Cannot find UI object");
            }

            if (inExplorer)
            {
                explorerHwnd = hwnd;
                GetWindowRect(hwnd, out RECT r);
                MoveWindow(hwnd, Display.main.systemWidth / 2, Display.main.renderingHeight / 4, Display.main.systemWidth / 2, Display.main.systemHeight / 2, true);
            }
            else
            {
                explorerHwnd = IntPtr.Zero;
            }

            pathInExplorer = inExplorer;
        }
        
        if (!_isGettingCurrentObject)
        {
            StartCoroutine(FindForegroundWindow());
        }
        #endif
    }

    public void EndLoadScene()
    {
        GameObject uiObject = GameObject.FindGameObjectWithTag("UI");
        if (uiObject != null && uiObject.TryGetComponent(out ExplorerUIController explorerUiController))
        {
            if (pathInExplorer)
            {
                explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color =
                    new Color(1f, 1f, 1f, 0.4f);
            }
            else
            {
                Color c = explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color;
                explorerUiController.explorerCanvas.GetComponentInChildren<Image>().color =
                    new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            Debug.LogWarning("Cannot find UI object");
        }
    }

    public void Clear()
    {
        _pathToScript.Clear();
    }

    public void Set(FileParser fileParser)
    {
        var relativePath = Utils.RelativePath(fileParser.filePath);
        if (_pathToScript.ContainsKey(relativePath))
        {
            Debug.LogError("FilesWatcher should not set a FileParser which already exists with the same path: " + relativePath);
        } else 
            _pathToScript.Add(relativePath, fileParser);
    }

    public bool? ContainsFile(FileInfo fi)
    {
        string extension = Path.GetExtension(fi.FullName);
        if (!Regex.IsMatch(extension, supportedExtensions, RegexOptions.IgnoreCase))
            return null;
        return _pathToScript.ContainsKey(Utils.RelativePath(fi.FullName));
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
            string sceneName = LevelManager.Capitalize(SceneManager.GetActiveScene().name);
            var completeObjectPath = "/" + Utils.RootFolderName + "/" + sceneName + "/" + objectFileName; //to be changed
            if (_pathToScript[completeObjectPath] != _currentHighlightObject && _currentHighlightObject)
            {
                _currentHighlightObject.gameObject.GetComponentInChildren<SpriteRenderer>().material = unhighlightMaterial;
                _pathToScript[completeObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
                _currentHighlightObject = _pathToScript[completeObjectPath];
                foreach (SpriteRenderer sr in _pathToScript[completeObjectPath].gameObject.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.material.SetFloat("_numberOfSprite", sr.sprite.texture.width / sr.sprite.rect.width);
                }
            }
            _pathToScript[completeObjectPath].gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMaterial;
            _currentHighlightObject = _pathToScript[completeObjectPath];
            foreach (SpriteRenderer sr in _pathToScript[completeObjectPath].gameObject.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.material.SetFloat("_numberOfSprite", sr.sprite.texture.width / sr.sprite.rect.width);
            }
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

    public IEnumerator VibrateExplorer()
    {
        if (explorerHwnd != IntPtr.Zero && !isVibrating)
        {
            isVibrating = true;
            SetForegroundWindow(explorerHwnd);
            // RECT r;
            if (GetWindowRect(explorerHwnd, out RECT r))
            {
                int width = r.Right - r.Left;
                int height = r.Bottom - r.Top;
                for (int i = 0; i < 10; i++)
                {
                    int dx = Random.Range(-8, 8);
                    int dy = Random.Range(-8, 8);
                    if (!MoveWindow(explorerHwnd, r.Left + dx, r.Top + dy, width, height, false))
                    {
                        Debug.LogWarning("Error to move explorer window");
                    }

                    yield return new WaitForSeconds(0.05f);
                }
            }
            else
            {
                Debug.LogWarning("Error to get explorer window rect");
            }
        }
        else
        {
            Debug.LogWarning("No eplorere HWND");
        }
        isVibrating = false;
    }


#if UNITY_STANDALONE_WIN

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left; // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom; // y position of lower-right corner
    }
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", SetLastError=true)]
    public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

#endif

}
