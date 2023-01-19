using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic State class that can be used for various state machines throughout the game
/// States will be created as scriptable objects
/// </summary>
public class BaseState : ScriptableObject
{
    //public string name;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// Function called when entering state
    /// </summary>
    /// <param name="SM">State machine operating said state</param>
    public virtual void Enter(StateMachine SM) { }

    /// <summary>
    /// Function called when exiting state
    /// </summary>
    /// <param name="SM">State Machine operating said state</param>
    public virtual void Exit(StateMachine SM) { }

    /// <summary>
    /// Function called when updating the state's inner logic
    /// </summary>
    public virtual void UpdateStateLogic() { }
}
