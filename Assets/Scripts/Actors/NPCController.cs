using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 *      Main component of the NPCs that will control their behaviors and overall actions
 */
public class NPCController : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt; //Interaction prompt displayed when player is in interaction range with the NPC
    [SerializeField] private bool canBeInteracted;

    private void Start()
    {
        Assert.IsNotNull(interactionPrompt);
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (canBeInteracted) { OnPlayerDetected(true); } //DEBUG ONLY
        else { OnPlayerDetected(false); }
    }

    /**
     *  Function called by ??other?? scripts upon player detection 
     *  @param : status (bool) indicates wether player detected on not
     */
    public void OnPlayerDetected(bool status)
    {
        DisplayInteractionPrompt(status);
    }

    /**
     *  Function displaying the interaction prompt
     */
    private void DisplayInteractionPrompt(bool status)
    {
        interactionPrompt.SetActive(status);
    }
}
