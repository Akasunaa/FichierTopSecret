using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector]public bool canBeInteracted;

    public abstract void Interact();

}
