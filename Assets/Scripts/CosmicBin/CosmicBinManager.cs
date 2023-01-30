using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public class CosmicBinManager : MonoBehaviour
{
    public bool cosmicBinIsloaded { get; set; }
    public static CosmicBinManager Instance { get; private set; }
    private List<GameObject> objectsSuppressed;
    private List<Vector2> usedPositions;

    [Header("Cosmic bin variables")]
    public string cosmicBinFolderName;
    [SerializeField] private int verticalGap;
    [SerializeField] private int horizontalGap;

    private List<Vector2> neighborPos = new List<Vector2>() {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

    private void Awake()
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

        cosmicBinIsloaded = false;
        objectsSuppressed = new List<GameObject>();
        usedPositions = new List<Vector2>();
    }

    private void Start()
    {
        GenerateCosmicBin();
    }

    public void GenerateCosmicBin()
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" +
                                                 Utils.CosmicbinFolderName);

            if (!di.Exists)
            {
                Debug.Log("Create new directory: " + di.FullName + " | " + cosmicBinFolderName);
                di.Create();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnCosmicBinLoad()
    {
        Debug.Log("LOADING BIN");
        cosmicBinIsloaded = true;
        Vector2 targetPos = Vector2.zero;
        foreach(var localGameObject in objectsSuppressed)
        {
            localGameObject.transform.position = targetPos;
            usedPositions.Add(targetPos);

            do
            {
                // generate random position
                int rd = Random.Range(0, objectsSuppressed.Count);
                int rdDirection = Random.Range(0, 4);

                targetPos = (Vector2)objectsSuppressed[rd].transform.position + neighborPos[rdDirection] * new Vector2(horizontalGap, verticalGap);
            } while (usedPositions.Contains(targetPos));
        }   
    }

    public void AddRestorationController(GameObject localGameObject)
    {
        // the door out of the cosmic bin does not require a restoration controller
        if (!localGameObject.TryGetComponent(out DoorObjectController _))
        {
            localGameObject.AddComponent<BinRestorationController>();
        }
    }

    public void RestoreSuppressedObject(GameObject localGameObject)
    {
        Debug.Log("Restore object " + localGameObject.name);
        if (localGameObject.TryGetComponent(out FileParser fileParser) && localGameObject.TryGetComponent(out ModifiableController modifiableCtrlr))
        {
            string folderDestination;
            modifiableCtrlr.TryGet("scene target", out folderDestination);
            FileInfo fi = new FileInfo(fileParser.filePath);

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
            modifiableCtrlr.RemoveValue("scene target");
            fileParser.WriteToFile();
            
            // remove the object from the scene and from the file watcher
            Destroy(localGameObject);
            FilesWatcher.instance.GetPathToScript().Remove(folderDestination + "/" + fi.Name);
        }
    }

    public void AddSuppressedObject(GameObject localGameObject)
    {
        objectsSuppressed.Add(localGameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        objectsSuppressed.Clear();
    }
}
