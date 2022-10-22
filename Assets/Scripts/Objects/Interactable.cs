using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt; //Interaction prompt displayed when player is in interaction range with the NPC
    [SerializeField] public bool canBeInteracted;

    private void Start()
    {
        Assert.IsNotNull(interactionPrompt);
        interactionPrompt.SetActive(false);
    }


    private void Update()
    {
        //DisplayInteractionPrompt(canBeInteracted);
    }
    private void DisplayInteractionPrompt(bool status)
    {
        //interactionPrompt.SetActive(status);
    }

}
