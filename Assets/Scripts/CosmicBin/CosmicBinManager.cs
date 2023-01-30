using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public class CosmicBinManager : MonoBehaviour
{
    public bool cosmicBinIsLoaded { get; set; }
    public static CosmicBinManager instance { get; private set; }
    private List<GameObject> _objectsSuppressed;
    private List<Vector2> _usedPositions;

    [Header("Cosmic bin variables")]
    public string cosmicBinFolderName;
    [SerializeField] private int verticalGap;
    [SerializeField] private int horizontalGap;

    private readonly List<Vector2> _neighborPos = new() {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

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

        cosmicBinIsLoaded = false;
        _objectsSuppressed = new List<GameObject>();
        _usedPositions = new List<Vector2>();
    }

    private void Start()
    {
        GenerateCosmicBin();
    }

    public void GenerateCosmicBin()
    {
        try
        {
            var di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" +
                                       Utils.CosmicbinFolderName);

            if (di.Exists) return;
            Debug.Log("Create new directory: " + di.FullName + " | " + cosmicBinFolderName);
            di.Create();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnCosmicBinLoad()
    {
        Debug.Log("LOADING BIN");
        cosmicBinIsLoaded = true;
        var targetPos = Vector2.zero;
        foreach(var localGameObject in _objectsSuppressed)
        {
            localGameObject.transform.position = targetPos;
            _usedPositions.Add(targetPos);

            do
            {
                // generate random position
                var rd = Random.Range(0, _objectsSuppressed.Count);
                var rdDirection = Random.Range(0, 4);

                targetPos = (Vector2)_objectsSuppressed[rd].transform.position + _neighborPos[rdDirection] * new Vector2(horizontalGap, verticalGap);
            } while (_usedPositions.Contains(targetPos));
        }   
    }

    public static void AddRestorationController(GameObject localGameObject)
    {
        // the door out of the cosmic bin does not require a restoration controller
        if (!localGameObject.TryGetComponent(out DoorObjectController _))
        {
            localGameObject.AddComponent<BinRestorationController>();
        }
    }

    public static void RestoreSuppressedObject(GameObject localGameObject)
    {
        Debug.Log("Restore object " + localGameObject.name);
        
        if (!localGameObject.TryGetComponent(out FileParser fileParser) || 
            !localGameObject.TryGetComponent(out ModifiableController modifiableController))
            return;

        
        modifiableController.TryGet("scene target", out string folderDestination);
        
        var fi = new FileInfo(fileParser.filePath);

        // remove read only and delete the file
        try
        {
            File.SetAttributes(fileParser.filePath,
                File.GetAttributes(fileParser.filePath) & ~FileAttributes.ReadOnly);
            File.Delete(fileParser.filePath);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        // change the path to the origin folder and rewrite the file
        fileParser.filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + folderDestination + "/" + fi.Name;
        modifiableController.RemoveValue("scene target");
        fileParser.WriteToFile();
            
        // remove the object from the scene and from the file watcher
        Destroy(localGameObject);
        FilesWatcher.instance.GetPathToScript().Remove(folderDestination + "/" + fi.Name);
    }

    public void AddSuppressedObject(GameObject localGameObject)
    {
        _objectsSuppressed.Add(localGameObject);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        _objectsSuppressed.Clear();
    }
}
