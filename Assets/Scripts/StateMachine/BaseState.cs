using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Basic State class that can be used for various state machines throughout the game
 *  States will be created as scriptable objects
 */
public class BaseState : ScriptableObject
{
    public string name;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    /**
     *  Function called when entering state
     */
    public virtual void Enter(StateMachine SM) { }

    /**
     *  Function called when exiting state
     */
    public virtual void Exit() { }

    /**
     *  Function called when updating the state's inner logic
     */
    public virtual void UpdateStateLogic() { }
}
