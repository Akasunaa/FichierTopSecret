using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Texture2D manual;
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

    //particle effect
    [SerializeField] private ParticleSystem popParticle;
    [SerializeField] public ParticleSystem depopParticle;

    //fader
    private Image fadeImage;

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
        
        fadeImage = GetComponentInChildren<Image>();
        fadeImage.color = new Color(0F, 0F, 0F, 0F);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        //CREATE MANUAL :
        CreateManual();
        //LOAD SCENE :
        LoadScene(levelToLoad);
    }
    public void LoadScene(string levelName)
    {
        if (!isLoading)
        {
            isLoading = true;
            fadeImage.color = new Color(0F, 0F, 0F, 1F);
            StartCoroutine(LoadSceneCoroutine(Capitalize(levelName)));
        }
    }

    /// <summary>
    /// Function that will create the Manual PNG file in the root Game folder
    /// </summary>
    private void CreateManual()
    {
        if (manual != null)
        {
            byte[] bytes = manual.EncodeToPNG();
            File.WriteAllBytes(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/manual.png", bytes);
        }
    }

    private IEnumerator LoadSceneCoroutine(string levelName)
    {
        isLoading = true;
        FilesWatcher.instance.Clear();
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + levelName);

        bool directoryExists = di.Exists;
        if (!directoryExists)
        {
            Debug.Log("Create new directory: " + di.FullName + " | " + levelName);
            di.Create();
        }

       
        UnityEngine.AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("Scenes/" + levelName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {      
            yield return null;
        }

        activeLevel = SceneManager.GetSceneByName(levelName);
        bool completeSceneLoading = UpdateFileGameObjects(directoryExists);
        completeSceneLoading = CreateGameObjectFromFiles(di) && completeSceneLoading;

        if (levelName == Capitalize(CosmicBinManager.instance.cosmicBinFolderName))
        {
            Debug.Log("START LOADING");
            CosmicBinManager.instance.OnCosmicBinLoad();
        }
        else
        {
            CosmicBinManager.instance.cosmicBinIsLoaded = false;
        }
        try
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        catch { Debug.LogError("no player found"); }

        if (!completeSceneLoading)
        {
            if (SystemMessageController.Instance != null)
            {
                SystemMessageController.Instance.CallSystemMessage("Scene loading incomplete, please remove some files...");
            }
        }

        FilesWatcher.instance.EndLoadScene();
        isLoading = false;
        fadeImage.color = new Color(0F, 0F, 0F, 0F);
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

    /*
     * Update game objects in the scene to load base on the files present
     */
    private bool UpdateFileGameObjects(bool directoryExists)
    {
        // TODO : c'est moche
        ModifiableController[] modifiableGameObjects = FindObjectsOfType<ModifiableController>();
        foreach (ModifiableController modifiableController in modifiableGameObjects)
        {
            if (!modifiableController.TryGetComponent(out FileParser _))
            {
                modifiableController.SetDefaultProperties();
            }
        }

        uint nFileRead = 0;
        FileParser[] fileGameObjects = FindObjectsOfType<FileParser>();
        foreach (FileParser fileParser in fileGameObjects)
        {
            if (nFileRead > Utils.MAX_READ_FILE_SCENELOAD) { return false; }
            FileInfo fileInfo = new FileInfo(fileParser.filePath);
            if (fileInfo.Exists)
            {
                nFileRead++;
                Debug.Log("Updating file: " + fileInfo.FullName);
                fileParser.targetModifiable.SetDefaultProperties();
                fileParser.ReadFromFile(fileInfo.FullName, true);
                FilesWatcher.instance.Set(fileParser);
            }
            else if (!directoryExists || !fileParser.targetModifiable.canBeDeleted)
            {
                nFileRead++;
                Debug.Log("Creating file: " + fileInfo.FullName);
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    try
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                fileParser.targetModifiable.SetDefaultProperties();
                try
                {
                    using (StreamWriter sw = new StreamWriter(fileInfo.FullName))
                    {
                        sw.Write(fileParser.targetModifiable.ToFileString());
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                FilesWatcher.instance.Set(fileParser);
            }
            else
            {
                Debug.Log("Removing file: " + fileInfo.FullName);
                Destroy(fileParser.gameObject);
            }
        }

        return true;
    }

    /*
     * Recursively browse all files in the directory and create game objects from the files
     */
    private bool CreateGameObjectFromFiles(DirectoryInfo di, uint nFileRead = 0)
    {
        foreach (FileInfo fi in di.EnumerateFiles())
        {
            if (nFileRead > Utils.MAX_READ_FILE_SCENELOAD) { return false; }
            // If null it mean it is not a regular file (.txt and .bat)
            bool containFile = FilesWatcher.instance.ContainsFile(fi) ?? true;
            if (!containFile)
            {
                nFileRead++;
                // NewObject(fi, fi.FullName.Contains("Cosmicbin"));
                string relativePath = Utils.RelativePath(fi);
                NewObject(fi,  LevelManager.Capitalize(Utils.SceneName(relativePath)) == Utils.CosmicbinFolderName);
            }
        }

        foreach (DirectoryInfo diTmp in di.EnumerateDirectories())
        {
            CreateGameObjectFromFiles(diTmp, nFileRead);
        }

        return true;
    }

    /*
     * Create a new game object from a file if it match a regex
     * isItem : created in folder player
     */
    public void NewObject(FileInfo fi, bool isInComsicBin = false, bool isItem=false)
    {
        GameObject newObj;
        FileParser fp;
        Vector3Int pos = Vector3Int.zero;
        string nameObject = Path.GetFileNameWithoutExtension(fi.Name);
        if (nameObject.Contains("Nouveau ") || nameObject.Contains("New"))
            return;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerObjectController>().CreateSound();

        foreach (RegToGoPair pair in instantiable)
        {
            //check all synonym
            string[] synonyms = SynonymController.SearchSynonym(nameObject);
            var synonym = synonyms.FirstOrDefault(x => Regex.IsMatch(x,pair.reg, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));      
            if (synonym!=null)
            {
                if (pair.go == null) { return; }
                Debug.Log("[LevelManager] Instantiate new file : " + fi.FullName);
                if (isItem && !pair.go.TryGetComponent(out ItemController _)) { return; } //Non item object created in player folder
                newObj = Instantiate(pair.go);
                if (isItem)
                {
                    DontDestroyOnLoad(newObj);
                    newObj.transform.parent = PlayerItems.Instance.transform;
                    if (newObj.TryGetComponent(out ItemController ic) && ic.itemSprite != null)
                    {
                        ic.itemSprite.SetActive(false);
                    }
                }
                // setup file parser
                fp = newObj.AddComponent<FileParser>();
                fp.filePath = fi.FullName;
                fp.ReadFromFile(fi.FullName);
                FilesWatcher.instance.Set(fp);
                Vector2? size = null;
                if (newObj.TryGetComponent(out BoxCollider2D collider)){ size = collider.size * fp.transform.lossyScale;}
                if (!fp.targetModifiable.ContainsKey<Vector2Int>("position") && !isItem)
                {
                    if (player != null)
                    {
                        Vector2Int? target = Utils.NearestTileEmpty(player.GetComponent<PlayerMovement>().GetTilemapPosition(), size);
                        if (target != null)
                            pos = (Vector3Int)target;
                        else
                            pos = Vector3Int.one * 100_000;
                    }
                    newObj.transform.position = SceneData.Instance.grid.GetCellCenterWorld(pos);
                    fp.targetModifiable.SetValue("position", new Vector2Int(pos.x, pos.y));
                    if (!isItem) {
                        ParticleSystem particles = Instantiate(popParticle);
                        particles.gameObject.transform.position = pos;
                        particles.Play();
                        Destroy(particles.gameObject, 1);
                    }
                }
                fp.targetModifiable.SetDefaultProperties();


                // Clean the prefab if it is instantiated in the Cosmic bin
                if (isInComsicBin) { 
                    CosmicBinManager.AddRestorationController(newObj);
                } else
                {
                    fp.WriteToFile();
                }
                return;
            }
        }
        //nothing object : no object with the name of file 
        if (isItem)
            return;
        Debug.Log("[LevelManager] Instantiate a nothing : " + fi.FullName);
        newObj = Instantiate(instantiable.First(x => x.reg == "nothing").go);

        // setup file parser
        fp = newObj.AddComponent<FileParser>();
        fp.filePath = fi.FullName;
        fp.ReadFromFile(fi.FullName);
        FilesWatcher.instance.Set(fp);
        if (!fp.targetModifiable.ContainsKey<Vector2Int>("position"))
        {
            if (player != null)
            {
                Vector2Int? target = Utils.NearestTileEmpty(player.GetComponent<PlayerMovement>().GetTilemapPosition());
                if (target != null)
                    pos = (Vector3Int)target;
                else
                    pos = Vector3Int.one * 100_000;
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
        if (isInComsicBin) CosmicBinManager.AddRestorationController(newObj);
    }

    /**
    *  Function called when the npc changes state by responding to a player bringing a correct item
    *  It will create an instance of the stored item, and call its internal ItemController.RecuperatingItem() function
    */
    public static void GiveItem(GameObject item)
    {
        GiveItem(item.name);
    }

    public static void GiveItem(string itemName)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/" + Utils.RootFolderName +
                                                      "/" + Utils.PlayerFolderName + "/" + itemName + ".txt"))
            {
                sw.Write("");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public IEnumerator FadeIn(float time)
    {
        while (fadeImage.color.a < 1)
        {
            fadeImage.color += new Color(0F,0F,0F,Time.unscaledDeltaTime / time);
            fadeImage.color = new Color(0F, 0F, 0F, Mathf.Clamp01(fadeImage.color.a));
            yield return null;
        }
    }

    public IEnumerator FadeOut(float time)
    {
        while (fadeImage.color.a > 0)
        {
            fadeImage.color -= new Color(0F, 0F, 0F, Time.unscaledDeltaTime / time);
            fadeImage.color = new Color(0F, 0F, 0F, Mathf.Clamp01(fadeImage.color.a));
            yield return null;
        }
    }
}
