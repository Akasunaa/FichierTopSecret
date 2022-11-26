using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CosmicBinManager : MonoBehaviour
{
    public static CosmicBinManager Instance { get; private set; }
    
    [SerializeField] private List<GameObject> gameObjectsInBin;
    public string cosmicBinSceneName;

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

        gameObjectsInBin = new List<GameObject>();
    }

    private void Start()
    {
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Test/CosmicBin");

        if (!di.Exists)
        {
            Debug.Log("Create new directory: " + di.FullName + " | " + cosmicBinSceneName);
            di.Create();
        }
    }

    public void MoveGameObjectInComsicBin(GameObject go)
    {
        gameObjectsInBin.Add(go);
        DontDestroyOnLoad(go);
    }

    public void OnCosmicBinLoad()
    {
        foreach(GameObject go in gameObjectsInBin)
        {

        }
    }
}
