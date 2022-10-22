using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

    /**
     *  Function called when changing state by other external factors
     *  Calls the currentState's exit method to solve any remaining state's actions if needed
     *  Then attributes and calls the new state's Enter method
     */
    public new void ChangeState(int newStateIndex)
    {
        if (currentState.ConvertTo<DialogState>().nextStates.Length == 1) //in case the current state only has one follow up, we immediately change state regardless of said follow-up
        {
            currentState.Exit();
            currentState = currentState.ConvertTo<DialogState>().nextStates[0];
            currentState.Enter();
            return;
        }
        if(currentState.ConvertTo<DialogState>().nextStates.Length > newStateIndex)
        {
            currentState.Exit();
            currentState = currentState.ConvertTo<DialogState>().nextStates[newStateIndex];
            currentState.Enter();
            return;
        }
        else
        {
            Debug.LogError("DialogSM : ERROR : NOT ACCEPTABLE NEW STATE INDEX");
            return;
        }
        
    }
}
