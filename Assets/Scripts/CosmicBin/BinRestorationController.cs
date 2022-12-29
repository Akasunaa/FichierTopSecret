using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component used by objects that the player can interact with but not registered by files
 */
public class BinRestorationController : MonoBehaviour, Interactable
{

    public bool canBeInteracted { get; set; }

    public void Interact()
    {
        CosmicBinManager.Instance.RestoreSuppressedObject(gameObject);
    }
}
