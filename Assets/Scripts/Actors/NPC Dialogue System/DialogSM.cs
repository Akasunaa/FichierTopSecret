using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 *  Dialog State Machine used by the NPCs inherited from StateMachine basis
 */
public class DialogSM : StateMachine
{
    [SerializeField] private DialogState startingState;
    [Header("Next possible states")]
    [SerializeField] public NextState[] nextStates;           //all the currently possible next states on the serialize part
    [HideInInspector] public Dictionary<string, DialogState> NextPossibleStates; //the dictionnary used for the possible next states
    [SerializeField, HideInInspector] public NPCController associatedNpcController;              //npcController that uses this DialogSM

    protected void Start()
    {
        //base.Start();
        // We begin the Enter function by creating the dictionary of the next possible states :
        NextPossibleStates = new Dictionary<string, DialogState> { { startingState.name, startingState } };
        foreach (var state in nextStates)
        {
            if (!NextPossibleStates.ContainsKey(state.name))
            {
                NextPossibleStates.Add(state.state.name, state.state);
            }
        }
        //-------------------------------------------------------------------------------------
    }

    public override BaseState GetStartingState()
    {
        return startingState;
    }

    /**
     *  Function only called by the Editor NPC script to setup the starting state
     */
    public void SetStartingState(BaseState state)
    {
        startingState = state.ConvertTo<DialogState>();
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
    public void ChangeState(string nextStateName)
    {
        if (nextStateName == currentState.name) //same state
        {
            return;
        }
        if (NextPossibleStates.TryGetValue(nextStateName, out DialogState dialog))
        {
            currentState.Exit(this);
            currentState = dialog;
            currentState.Enter(this);
        }
        else
        {
            Debug.LogError("DialogSM : ERROR : NOT ACCEPTABLE NEW STATE NAME. INPUTTED NAME : "+nextStateName);
        }
    }
}

/**
 *  Structure of elements used to create the list of new states
 *  We made it a different class to allow other scripts to use it (notably the custom editor window)
 */
[Serializable]
public struct NextState                               //struct used for the next states
{
    [HideInInspector] public string name;
    public DialogState state;
}
