using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component used by objects that the player can interact with but not registered by files
 */
public class InteractableObjectController : MonoBehaviour, Interactable
{
    private ObjectInteractionController interactionController;

    public bool canBeInteracted { get; set; }

    private void Start()
    {
        if(gameObject.GetComponent<ObjectInteractionController>() != null)
        {
            interactionController = gameObject.GetComponent<ObjectInteractionController>();
        }
    }

    public void Interact()
    {
        if (Time.timeScale == 0f)
        {
            interactionController.EndDisplay();
            Time.timeScale = 1f;
            return;
        }
        else
        {
            Time.timeScale = 0f;    //if player in interaction, then stop time to prevent movement
        }
        if (interactionController)
        {
            interactionController.DisplayInteractionDialogue();
        }
    }
}
