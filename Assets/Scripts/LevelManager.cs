using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private Scene activeLevel;
    [SerializeField] private SceneAsset levelToLoad;

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
        while (!asyncLoadLevel.isDone){
            yield return null;
        }

        activeLevel = SceneManager.GetSceneByName(levelName);
        UpdateFileGameObjects(directoryExists);
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
}
