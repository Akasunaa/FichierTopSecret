using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script used by special dialog states that will, when ariving in their Enter() function, stop an NPC's state.
/// </summary>
[CreateAssetMenu(fileName = "Movement Stop Dialog State", menuName = "States/Movement Stop Dialog State")]
public class MovementStopDialogState : DialogState
{
    [SerializeField] private bool shouldMoveBool;

    public MovementStopDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function Enter() called when state is loaded.
    /// Desactivate the movement of the NPC when entered.
    /// </summary>
    /// <param name="sm">State Machine operating the state</param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        sm.ConvertTo<DialogSM>().associatedNPCController.shouldMove = shouldMoveBool;
    }
}
