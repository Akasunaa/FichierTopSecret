using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 *  Dialog State Machine used by the NPCs inherited from StateMachine basis
 */
public class DialogSM : StateMachine
{
    [SerializeField] private DialogState startingState;

    public override BaseState GetInitialState()
    {
        return startingState;
    }

    /**
     *  Function called by external scripts that will tell the SM that the current speech has been read
     */
    public void OnDialogInteraction()
    {
        currentState.ConvertTo<DialogState>().ChangeSpeech();
    }
}
