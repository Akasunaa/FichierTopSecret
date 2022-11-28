using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Instance { get; private set; }

    [Header("World")]
    [SerializeField] public Grid grid;
    [Header("UI")]
    [SerializeField] public DialogueUIController dialogueUIController;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
