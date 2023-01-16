using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/**
 *  Script used by special dialog states triggered when an NPC dies, that will, when ariving in their Exit() function, unactivate the the parent NPC
 */
[CreateAssetMenu(fileName = "Death Dialog State", menuName = "States/Death Dialog State")]
public class DeathDialogState : DialogState
{
    public DeathDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /**
     *  Inherited and overrided function that will handle the killing of the NPC
     */
    public override int ChangeSpeech()
    {
        interactionIndex++;
        if (interactionIndex < speech.Length)
        {
            currentSpeech = speech[interactionIndex];
            GetSpeechVariables(SM);
            return 1;
        }
        else //when reaching the end of the various speeches, the NPC will die
        {
            currentSpeech = speech[^1];
            GetSpeechVariables(SM);
            TriggerNPCDeath();
            return 0;
        }
    }

    /**
     *  Actual trigger of the NPC's death
     */
    public void TriggerNPCDeath()
    {
        SM.ConvertTo<DialogSM>().associatedNPCController.gameObject.SetActive(false);
    }
}
