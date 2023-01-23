using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Dialog State Machine used by the NPCs inherited from StateMachine basis
/// </summary>
public class DialogSM : StateMachine
{
    [SerializeField] private DialogState startingState;
    [Header("Next possible states")]
    [SerializeField] public NEXT_STATE[] nextStates;           //all the currently possible next states on the serialize part
    [HideInInspector] public Dictionary<string, DialogState> nextPossibleStates; //the dictionnary used for the possible next states
    [SerializeField, HideInInspector] public NPCController associatedNPCController;              //npcController that uses this DialogSM

    protected void Start()
    {
        // We begin the Enter function by creating the dictionary of the next possible states :
        nextPossibleStates = new Dictionary<string, DialogState>();
        nextPossibleStates.Add(startingState.name, startingState);
        foreach (NEXT_STATE state in nextStates)
        {
            //Debug.Log("DIALOG SM : " + gameObject.name + " : STATE " + state.state.name + " CONSIDERED TO ADD IN DICT");
            if (!nextPossibleStates.ContainsKey(state.state.name))
            {
                //Debug.Log("DIALOG SM : " + gameObject.name + " : STATE " + state.state.name + " IN DICT");
                nextPossibleStates.Add(state.state.name, state.state);
            }
        }
        //-------------------------------------------------------------------------------------
    }

    public override BaseState GetStartingState()
    {
        return startingState;
    }

    /// <summary>
    /// Function that returns the name of the currently loaded state
    /// </summary>
    /// <returns>Name (string) of the currently playing state of the dialogSM</returns>
    public string GetCurrentStateName()
    {
        return currentState.name;
    }

    /// <summary>
    /// Function only called by the Editor NPC script to setup the starting state
    /// </summary>
    /// <param name="state">state that will be set as the starting state of the state machine</param>
    public void SetStartingState(BaseState state)
    {
        startingState = state.ConvertTo<DialogState>();
    }

    /// <summary>
    /// Function called by external scripts that will tell the SM that the current speech has been read
    /// </summary>
    /// <returns>0 if the currentSpeech is the last speech, 1 otherwise</returns>
    public int OnDialogInteraction()
    {
        return currentState.ConvertTo<DialogState>().ChangeSpeech();
    }

    /// <summary>
    /// Function called when changing state by other external factors
    /// Calls the currentState's exit method to solve any remaining state's actions if needed
    /// Then attributes and calls the new state's Enter method
    /// </summary>
    /// <param name="nextStateName">name of the next state to change to</param>
    public void ChangeState(string nextStateName)
    {
        if (nextStateName == currentState.name) //same state
        {
            return;
        }
        if (nextPossibleStates!=null && nextPossibleStates.TryGetValue(nextStateName, out DialogState dialog))
        {
            currentState.Exit(this);
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

/// <summary>
/// Structure of elements used to create the list of new states
/// We made it a different class to allow other scripts to use it (notably the custom editor window)
/// </summary>
[System.Serializable]
public struct NEXT_STATE                               
{
    [HideInInspector] public string name;
    public DialogState state;
}
