using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

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
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test/Cosmicbin");

        if (!di.Exists)
        {
            Debug.Log("Create new directory: " + di.FullName + " | " + cosmicBinFolderName);
            di.Create();
        }
    }

    public void OnCosmicBinLoad()
    {
        Debug.Log("LOADING BIN");
        cosmicBinIsloaded = true;
        Vector2 targetPos = Vector2.zero;
        foreach(var gameObject in objectsSuppressed)
        {
            gameObject.transform.position = targetPos;
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

    public void AddRestorationController(GameObject gameObject)
    {
        // the door out of the cosmic bin does not require a restoration controller
        if (!gameObject.TryGetComponent(out DoorObjectController _))
        {
            gameObject.AddComponent<BinRestorationController>();
        }
    }

    public void RestoreSuppressedObject(GameObject gameObject)
    {
        Debug.Log("Restore object " + gameObject.name);
        if (gameObject.TryGetComponent(out FileParser fileParser) && gameObject.TryGetComponent(out ModifiableController modifiableCtrlr))
        {
            string folderDestination;
            modifiableCtrlr.TryGet("scene target", out folderDestination);
            FileInfo fi = new FileInfo(fileParser.filePath);

            // remove read only and delete the file
            File.SetAttributes(fileParser.filePath, File.GetAttributes(fileParser.filePath) & ~FileAttributes.ReadOnly);
            File.Delete(fileParser.filePath);

            // change the path to the origin folder and rewrite the file
            fileParser.filePath = Application.streamingAssetsPath + "/Test/" + folderDestination + "/" + fi.Name;
            modifiableCtrlr.RemoveValue("scene target");
            fileParser.WriteToFile();
            
            // remove the object from the scene and from the file watcher
            Destroy(gameObject);
            FilesWatcher.Instance.GetPathToScript().Remove(folderDestination + "/" + fi.Name);
        }
    }

    public void AddSuppressedObject(GameObject gameObject)
    {
        objectsSuppressed.Add(gameObject);
    }
}
