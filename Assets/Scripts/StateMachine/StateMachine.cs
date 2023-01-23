using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic State Machine component that can be inherited into various State machines in the game
/// </summary>
public class StateMachine : MonoBehaviour
{
    public BaseState currentState;

    protected virtual void Awake()
    {
        currentState = GetStartingState();
        if(currentState != null)
        {
            currentState.Enter(this);
        }
    }

    private void Update()
    {
        if(currentState != null)
        {
            currentState.UpdateStateLogic();
        }
    }

    /// <summary>
    /// Function called when changing state by other external factors.
    /// Calls the currentState's exit method to solve any remaining state's actions if needed.
    /// Then attributes and calls the new state's Enter method.
    /// </summary>
    /// <param name="newState">new state that the machine will switch to</param>
    public void ChangeState(BaseState newState)
    {
        if(newState != null)
        {
            currentState.Exit(this);
            currentState = newState;
            currentState.Enter(this);
        }
        else
        {
            Debug.LogError("ERROR : NO NEW ACCESSIBLE STATE");
        }
    }

    /// <summary>
    /// Function that will return the initial state of a certain state machine 
    /// </summary>
    /// <returns>the StateMachine's starting state</returns>
    public virtual BaseState GetStartingState()
    {
        return null;
    }
}
