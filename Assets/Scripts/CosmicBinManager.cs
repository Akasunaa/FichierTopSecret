using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CosmicBinManager : MonoBehaviour
{
    public static CosmicBinManager Instance { get; private set; }
    private List<GameObject> objectsSuppressed;
    private List<Vector2> usedPositions;

    [Header("Cosmic bin variables")]
    public string cosmicBinSceneName;
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

        objectsSuppressed = new List<GameObject>();
        usedPositions = new List<Vector2>();

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

    public void OnCosmicBinLoad()
    {
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

    public void AddSuppressedObject(GameObject gameObject)
    {
        objectsSuppressed.Add(gameObject);
    }
}
