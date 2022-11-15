using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Basic State Machine component that can be inherited into various State machines in the game
 */
public class StateMachine : MonoBehaviour
{
    public BaseState currentState;

    protected void Start()
    {
        currentState = GetInitialState();
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

    /**
     *  Function called when changing state by other external factors
     *  Calls the currentState's exit method to solve any remaining state's actions if needed
     *  Then attributes and calls the new state's Enter method
     */
    public void ChangeState(BaseState newState)
    {
        if(newState != null)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter(this);
        }
        else
        {
            Debug.LogError("ERROR : NO NEW ACCESSIBLE STATE");
        }
    }

    /**
     *  Function that will return the initial state of a certain state machine (must be updated in following scripts)
     */
    public virtual BaseState GetInitialState()
    {
        return null;
    }
}
