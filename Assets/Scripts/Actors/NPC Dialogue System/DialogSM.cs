using System;
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
    [Serializable]
    private struct NEXT_STATE                               //struct used for the next states
    {
        [HideInInspector] public string name;
        public DialogState state;
    }
    [Header("Next possible states")]
    [SerializeField] private NEXT_STATE[] nextStates;           //all the currently possible next states on the serialize part
    [HideInInspector] public Dictionary<string, DialogState> nextPossibleStates; //the dictionnary used for the possible next states

    protected void Start()
    {
        base.Start();
        // We begin the Enter function by creating the dictionary of the next possible states :
        nextPossibleStates = new Dictionary<string, DialogState>();
        nextPossibleStates.Add(startingState.name, startingState);
        foreach (NEXT_STATE state in nextStates)
        {
            if (!nextPossibleStates.ContainsKey(state.name))
            {
                nextPossibleStates.Add(state.state.name, state.state);
            }
        }
        //-------------------------------------------------------------------------------------
    }

    public override BaseState GetInitialState()
    {
        return startingState;
    }

    /**
     *  Function called by external scripts that will tell the SM that the current speech has been read
     */
    public int OnDialogInteraction()
    {
        return currentState.ConvertTo<DialogState>().ChangeSpeech();
    }

    /**
     *  Function called when changing state by other external factors
     *  Calls the currentState's exit method to solve any remaining state's actions if needed
     *  Then attributes and calls the new state's Enter method
     */
    public new void ChangeState(string nextStateName)
    {
        if(nextStateName == currentState.name) //same state
        {
            return;
        }
        if (nextPossibleStates.TryGetValue(nextStateName, out DialogState dialog))
        {
            currentState.Exit();
            currentState = dialog;
            currentState.Enter(this);
            return;
        }
        else
        {
            Debug.LogError("DialogSM : ERROR : NOT ACCEPTABLE NEW STATE NAME. INPUTTED NAME : "+nextStateName);
            return;
        }
    }
}
