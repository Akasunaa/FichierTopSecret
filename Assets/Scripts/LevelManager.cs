using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    private Scene _activeLevel;
    private bool _isLoading ; // = false;
    [SerializeField] private string levelToLoad;

    private PlayerMovement _player;
    
    [Serializable] private struct RegToGoPair
    {
        [SerializeField] public string reg;
        [SerializeField] public GameObject go;
    }
    
    [SerializeField] private List<RegToGoPair> instantiable;

    [SerializeField] private ParticleSystem popParticle;
    [SerializeField] public ParticleSystem depopParticle;

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

        
        var di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName);

        if (!di.Exists) return;
        
        // remove readonly attributes on cosmic bin items to delete them
        var di2 = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/Cosmicbin");
        if (di2.Exists)
        {
            foreach (var fileName in Directory.GetFiles(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/Cosmicbin"))
            {
                // var fileInfo = new FileInfo(fileName); // was not used
                File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
            }
        }
        di.Delete(true);

    }

    private void Start()
    {
        LoadScene(levelToLoad);
    }
    public void LoadScene(string levelName)
    {
        if (_isLoading) return;
        _isLoading = true;
        StartCoroutine(LoadSceneCoroutine(Capitalize(levelName)));
    }

    
    private IEnumerator LoadSceneCoroutine(string levelName)
    {
        _isLoading = true;
        FilesWatcher.instance.Clear();
        var di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + levelName);

        var directoryExists = di.Exists;
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

        _activeLevel = SceneManager.GetSceneByName(levelName);
        UpdateFileGameObjects(directoryExists);
        CreateGameObjectFromFiles(di);

        if (levelName == Capitalize(CosmicBinManager.Instance.cosmicBinFolderName))
        {
            Debug.Log("START LOADING");
            CosmicBinManager.Instance.OnCosmicBinLoad();
        } else
        {
            CosmicBinManager.Instance.cosmicBinIsloaded = false;
        }
        try
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        catch (Exception) { Debug.LogError("no player found"); }
        FilesWatcher.instance.EndLoadScene();
        _isLoading = false;
    }

    public static string Capitalize(string input)
    {
        return input switch
        {
            null => null,
            "" => input,
            _ => input[0].ToString().ToUpper() + input.ToLower().Substring(1)
        };
    }

    /*
     * Update game objects in the scene to load base on the files present
     */
    private void UpdateFileGameObjects(bool directoryExists)
    {
        var fileGameObjects = FindObjectsOfType<FileParser>();
        foreach (var fileParser in fileGameObjects)
        {
            var fileInfo = new FileInfo(fileParser.filePath);
            if (fileInfo.Exists)
            {
                Debug.Log("Updating file: " + fileInfo.FullName);
                fileParser.ReadFromFile(fileInfo.FullName);
                FilesWatcher.instance.Set(fileParser);
            }
            else if (!directoryExists || !fileParser.targetModifiable.canBeDeleted)
            {
                Debug.Log("Creating file: " + fileInfo.FullName);
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    if (fileInfo.DirectoryName != null) Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                fileParser.targetModifiable.SetDefaultProperties();
                using (var sw = new StreamWriter(fileInfo.FullName))  
                {  
                    sw.Write(fileParser.targetModifiable.ToFileString());
                }
                FilesWatcher.instance.Set(fileParser);
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
        foreach (var fi in di.EnumerateFiles())
        {
            if (!FilesWatcher.instance.ContainsFile(fi))
            {
                NewObject(fi, fi.FullName.Contains("Cosmicbin"));
            }
        }

        foreach (var diTmp in di.EnumerateDirectories())
        {
            CreateGameObjectFromFiles(diTmp);
        }
    }

    /*
     * Create a new game object from a file if it match a regex
     */
    public void NewObject(FileInfo fi, bool isInCosmicBin = false)
    {
        GameObject newObj;
        FileParser fp;
        var pos = Vector3Int.zero;
        if (true) //todo what is "to do" here tho ? 
        {
            var nameObject = Path.GetFileNameWithoutExtension(fi.Name);
            if (nameObject.Contains("Nouveau ") || nameObject.Contains("New "))
            {
                return;
            }
            foreach (var pair in instantiable)
            {
                //check all synonym
                var synonyms = SynonymController.SearchSynonym(nameObject);
                var synonym = synonyms.FirstOrDefault(x => Regex.IsMatch(x,pair.reg, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
                if (synonym == null) continue;
                
                Debug.Log("[LevelManager] Instantiate new file : " + fi.FullName);
                newObj = Instantiate(pair.go);

                    // setup file parser
                    fp = newObj.AddComponent<FileParser>();
                    fp.filePath = fi.FullName;
                    fp.ReadFromFile(fi.FullName);
                    FilesWatcher.instance.Set(fp);
                    Vector2? size = null;
                    if (newObj.TryGetComponent(out BoxCollider2D localCollider)){ size = localCollider.size * fp.transform.lossyScale;}
                    if (!fp.targetModifiable.ContainsKey<Vector2Int>("position"))
                    {
                        if (_player != null)
                        {
                            var target = Utils.NearestTileEmpty(_player.GetComponent<PlayerMovement>().GetTilemapPosition(), size);
                            if (target != null)
                                pos = (Vector3Int)target;
                            else
                                Destroy(newObj);
                        }
                        newObj.transform.position = SceneData.Instance.grid.GetCellCenterWorld(pos);
                        fp.targetModifiable.SetValue("position", new Vector2Int(pos.x, pos.y));
                        ParticleSystem particles = Instantiate(popParticle);
                        particles.gameObject.transform.position = pos;
                        particles.Play();
                        Destroy(particles.gameObject,1);

                }
                fp.targetModifiable.SetDefaultProperties();


                // Clean the prefab if it is instantiated in the Cosmic bin
                if (isInCosmicBin) { 
                    CosmicBinManager.Instance.AddRestorationController(newObj);
                } else
                {
                    fp.WriteToFile();
                }
                return;
            }
            //nothing object : no object with the name of file 
            Debug.Log("[LevelManager] Instantiate a nothing : " + fi.FullName);
            newObj = Instantiate(instantiable.First(x => x.reg == "nothing").go);

            // setup file parser
            fp = newObj.AddComponent<FileParser>();
            fp.filePath = fi.FullName;
            fp.ReadFromFile(fi.FullName);
            FilesWatcher.instance.Set(fp);
            if (!fp.targetModifiable.ContainsKey<Vector2Int>("position"))
            {
                if (_player != null)
                {
                    Vector3Int? target = Utils.NearestTileEmpty(_player.GetComponent<PlayerMovement>().GetTilemapPosition());
                    if (target != null)
                        pos = (Vector3Int)target;
                    else
                        Destroy(newObj);
                }
                newObj.transform.position = SceneData.Instance.grid.GetCellCenterWorld(pos);
                fp.targetModifiable.SetValue("position", new Vector2Int(pos.x, pos.y));
                ParticleSystem particles = Instantiate(popParticle);
                particles.gameObject.transform.position = pos;
                particles.Play();
                Destroy(particles.gameObject, 1);
            }
            fp.WriteToFile();
            // using (StreamWriter sw = new StreamWriter(fp.filePath))
            // {
            //     sw.Write(fp.targetModifiable.ToFileString());
            // }

            // Clean the prefab if it is instantiated in the Cosmic bin
            if (isInCosmicBin) CosmicBinManager.Instance.AddRestorationController(newObj);
        }
    }
}
