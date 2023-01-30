using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component used by objects that the player can interact with but not registered by files
 */
public class InteractableObjectController : MonoBehaviour, Interactable
{
    private ObjectInteractionController interactionController;
    protected bool isInInteraction; //boolean at true if the player is currently in interaction

    public bool canBeInteracted { get; set; }

    private void Start()
    {
        isInInteraction = false;
        if(gameObject.GetComponent<ObjectInteractionController>() != null)
        {
            interactionController = gameObject.GetComponent<ObjectInteractionController>();
        }
    }

    public void Interact()
    {
        if (isInInteraction)
        {
            interactionController.EndDisplay();
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartMovement();
            isInInteraction = false;
            return;
        }
        else
        {
            isInInteraction = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
        }
        if (interactionController)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerObjectController>().InteractSound();
            interactionController.DisplayInteractionDialogue();
        }
    }
}
