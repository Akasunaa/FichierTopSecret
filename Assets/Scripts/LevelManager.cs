using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private Scene activeLevel;
    [SerializeField] private SceneAsset levelToLoad;
    
    [Serializable] private struct RegToGoPair
    {
        [SerializeField] public string reg;
        [SerializeField] public GameObject go;
    }
    
    [SerializeField] private List<RegToGoPair> instantiable;

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
        LoadScene(levelToLoad.name);
    }

    public void LoadScene(string levelName)
    {
        StartCoroutine(LoadSceneCoroutine(levelName));
    }

    private IEnumerator LoadSceneCoroutine(string levelName)
    {
        FilesWatcher.Instance.Clear();
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test" + "/" + levelName);

        bool directoryExists = di.Exists;
        if (!directoryExists)
        {
            Debug.Log("Create new directory: " + di.FullName);
            di.Create();
        }

        if (activeLevel.isLoaded)
        {
            SceneManager.UnloadSceneAsync(activeLevel);
        }
        
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("Scenes/" + levelName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }

        activeLevel = SceneManager.GetSceneByName(levelName);
        UpdateFileGameObjects(directoryExists);
        CreateGameObjectFromFiles(di);
    }

    private void UpdateFileGameObjects(bool directoryExists)
    {
        FileParser[] fileGameObjects = FindObjectsOfType<FileParser>();
        foreach (FileParser fileParser in fileGameObjects)
        {
            FileInfo fileInfo = new FileInfo(fileParser.filePath);
            if (fileInfo.Exists)
            {
                Debug.Log("Updating file: " + fileInfo.FullName);
                fileParser.ReadFromFile(fileInfo.FullName);
                FilesWatcher.Instance.Set(fileParser);
            }
            else if (!directoryExists)
            {
                Debug.Log("Creating file: " + fileInfo.FullName);
                fileParser.targetModifiable.setDefaultProperties();
                using (StreamWriter sw = new StreamWriter(fileInfo.FullName))  
                {  
                    sw.Write(fileParser.targetModifiable.ToFileString());
                }
                FilesWatcher.Instance.Set(fileParser);
            }
            else
            {
                Debug.Log("Removing file: " + fileInfo.FullName);
                Destroy(fileParser.gameObject);
            }
        }
    }

    private void CreateGameObjectFromFiles(DirectoryInfo di)
    {
        foreach (FileInfo fi in di.EnumerateFiles())
        {
            if (!FilesWatcher.Instance.ContainsFile(fi))
            {
                NewObject(fi);
            }
        }

        foreach (DirectoryInfo diTmp in di.EnumerateDirectories())
        {
            CreateGameObjectFromFiles(diTmp);
        }
    }

    public void NewObject(FileInfo fi)
    {
        if (Regex.IsMatch(fi.Name, ".*.txt$"))
        {
            foreach (RegToGoPair pair in instantiable)
            {
                if (Regex.IsMatch(fi.Name, pair.reg, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                {
                    Debug.Log("New file to watch: " + fi.FullName);
                    GameObject newObj = Instantiate(pair.go);
                    FileParser fp = newObj.AddComponent<FileParser>();
                    fp.filePath = fi.FullName;
                    fp.ReadFromFile(fi.FullName);
                    FilesWatcher.Instance.Set(fp);
                    break;
                }
            }
        }
    }
}
