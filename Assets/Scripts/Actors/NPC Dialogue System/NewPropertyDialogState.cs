using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// Script used by special dialog states that will add a new property to the.txt of associated NPCs
/// </summary>
[CreateAssetMenu(fileName = "New Property Dialog State", menuName = "States/New Property Dialog State")]
public class NewPropertyDialogState : DialogState
{
    [Header("Properties to be added")]
    [SerializeField] private FILE_PROPERTIES[] fileProperties;

    public NewPropertyDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function Enter() called when state is loaded.
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to add the inputted fileProperties to the NPC's.txt
    /// </summary>
    /// <param name="sm">State Machine operating the state</param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        //Debug.Log("NON ACCESSIBLE DIALOG STATE : ENTER()");
    }

    /// <summary>
    /// Function that will change the currently displayed speech of the state : at every call of the function by external scripts, will switch to next speech until the last one
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to modify a file property
    /// </summary>
    /// <returns>0 if the currentSpeech is the last speech, 1 otherwise</returns>
    public override int ChangeSpeech()
    {
        interactionIndex++;
        if (interactionIndex < speech.Length)
        {
            currentSpeech = speech[interactionIndex];
            GetSpeechVariables(SM);
            return 1;
        }
        else //when reaching the end of the various speeches, the NPC will repeat the last inputted speech
        {
            currentSpeech = speech[^1];
            GetSpeechVariables(SM);
            SM.ConvertTo<DialogSM>().associatedNPCController.AddProperty(fileProperties);
            return 0;
        }
    }
}
