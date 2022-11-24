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
    [SerializeField] private string levelToLoad;
    
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
        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test/");
            di.Delete(true);
        }
        #endif
        LoadScene(levelToLoad);
        SynonymController.SetSynonym();
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

    /*
     * Update game objects in the scene to load base on the files present
     */
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
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                fileParser.targetModifiable.SetDefaultProperties();
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

    /*
     * Recursively browse all files in the directory and create game objects from the files
     */
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

    /*
     * Create a new game object from a file if it match a regex
     */
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

                        FileParser fp_tmp = newObj.AddComponent<FileParser>();
                        fp_tmp.filePath = fi.FullName;
                        print(fp_tmp.filePath);
                        fp_tmp.ReadFromFile(fi.FullName);
                        FilesWatcher.Instance.Set(fp_tmp);
                        break;
                    
                }
            }
        }
    }
}
