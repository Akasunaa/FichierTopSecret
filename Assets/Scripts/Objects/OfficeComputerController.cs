using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeComputerController : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject explorerButton;
    public bool canBeInteracted { get; set; }

    void Start()
    {
        explorerButton.SetActive(false);
    }
    
    public void Interact()
    {
        if (!explorerButton.activeInHierarchy)
        {
            explorerButton.SetActive(true);
        }
        Application.OpenURL("file:///" + Application.streamingAssetsPath + "/" + Utils.RootFolderName);
    }
}
