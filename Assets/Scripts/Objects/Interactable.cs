using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public interface Interactable 
{
    public bool canBeInteracted { set; get;}

    public void Interact();

}
