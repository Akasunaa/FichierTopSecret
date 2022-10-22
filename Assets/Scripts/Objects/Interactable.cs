using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public bool canBeInteracted;

    public void Interact()
    {
        print("miaou!!!");
    }

}
