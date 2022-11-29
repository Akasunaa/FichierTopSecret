using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil.Rocks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private Scene activeLevel;
    private bool isLoading = false;
    [SerializeField] private string levelToLoad;

    PlayerMovement player;
    
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
    }
    public void LoadScene(string levelName)
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadSceneCoroutine(Capitalize(levelName)));
        }
    }

    public static string Capitalize(string input)
    {
        switch (input)
        {
            case null: return input;
            case "": return input;
            default: return input[0].ToString().ToUpper() + input.ToLower().Substring(1);
        }
    }
    
    private IEnumerator LoadSceneCoroutine(string levelName)
    {
        isLoading = true;
        FilesWatcher.Instance.Clear();
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test" + "/" + levelName);

        bool directoryExists = di.Exists;
        if (!directoryExists)
        {
            Debug.Log("Create new directory: " + di.FullName + " | " + levelName);
            di.Create();
        }

        // if (activeLevel.isLoaded)
        // {
        //     SceneManager.UnloadSceneAsync(activeLevel);
        // }
        
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("Scenes/" + levelName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }

        activeLevel = SceneManager.GetSceneByName(levelName);
        UpdateFileGameObjects(directoryExists);
        CreateGameObjectFromFiles(di);
        try
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        catch (Exception error) { Debug.LogError("no player found"); }
        isLoading = false;
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
        GameObject newObj;
        FileParser fp;
        Vector3Int pos = Vector3Int.zero;
        if (Regex.IsMatch(fi.Name, ".*.txt$"))
        {
            string nameObject = Path.GetFileNameWithoutExtension(fi.Name);
            if (nameObject.Contains("Nouveau ") || nameObject.Contains("New "))
            {
                return;

            }
            foreach (RegToGoPair pair in instantiable)
            {
                //check all synonym
                string[] synonyms = SynonymController.SearchSynonym(nameObject);
                var synonym = synonyms.FirstOrDefault(x => Regex.IsMatch(x,pair.reg, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
                
                if (synonym!=null)
                {
                    Debug.Log("[LevelManager] Instantiate new file : " + fi.FullName);
                    newObj = Instantiate(pair.go);
                    fp = newObj.AddComponent<FileParser>();
                    fp.filePath = fi.FullName;
                    fp.ReadFromFile(fi.FullName);
                    FilesWatcher.Instance.Set(fp);
                    if (!fp.targetModifiable.ContainsKey<Vector2Int>("position"))
                    {
                        if (player != null)
                        {
                            pos = Utils.NearestTileEmpty(player.GetComponent<PlayerMovement>().GetTilemapPosition());
                        }
                        newObj.transform.position = SceneData.Instance.grid.GetCellCenterWorld(pos);
                        fp.targetModifiable.SetValue("position", new Vector2Int(pos.x, pos.y));
                    }
                    fp.WriteToFile();
                    // using (StreamWriter sw = new StreamWriter(fp.filePath))
                    // {
                    //     sw.Write(fp.targetModifiable.ToFileString());
                    // }

                    return;
                }
            }
            //nothing object : no object with the name of file 
            Debug.Log("[LevelManager] Instantiate a nothing : " + fi.FullName);
            newObj = Instantiate(instantiable.First(x => x.reg == "nothing").go);
            fp = newObj.AddComponent<FileParser>();
            fp.filePath = fi.FullName;
            fp.ReadFromFile(fi.FullName);
            FilesWatcher.Instance.Set(fp);
            if (fp.targetModifiable.ContainsKey<Vector2Int>("position"))
            {
                if (player != null)
                {
                    pos = Utils.NearestTileEmpty(player.GetComponent<PlayerMovement>().GetTilemapPosition());
                }
                newObj.transform.position = SceneData.Instance.grid.GetCellCenterWorld(pos);
                fp.targetModifiable.SetValue("position", new Vector2Int(pos.x, pos.y));
            }
            fp.WriteToFile();
            // using (StreamWriter sw = new StreamWriter(fp.filePath))
            // {
            //     sw.Write(fp.targetModifiable.ToFileString());
            // }
        }
    }
}
