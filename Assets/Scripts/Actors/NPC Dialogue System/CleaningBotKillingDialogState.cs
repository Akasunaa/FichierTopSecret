using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script used to create a special state that will change the skin of the cleaning bot depending on its KILLING status.
/// </summary>
[CreateAssetMenu(fileName = "CleaningBot Killing Dialog State", menuName = "States/CleaningBot Killing Dialog State")]
public class CleaningBotKillingDialogState : DialogState
{
    [SerializeField] private bool shouldMoveBool;

    public CleaningBotKillingDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// When entering the idle state, cleaning bot will start killing
    /// </summary>
    /// <param name="sm"></param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        //Debug.Log("NPC CLEANING : SWITCHING TO KILLING");
        sm.ConvertTo<DialogSM>().associatedNPCController.shouldMove = shouldMoveBool;
        SM.gameObject.GetComponentInChildren<Animator>().SetBool("IsKilling", true);
        SM.gameObject.GetComponent<NPCController>().portraitRef = "Killing Drone";
    }

    /// <summary>
    /// When exiting the idle state, cleaning bot will start cleaning
    /// </summary>
    /// <param name="sm"></param>
    public override void Exit(StateMachine sm)
    {
        base.Exit(sm);
        //Debug.Log("NPC CLEANING : SWITCHING TO CLEANING");
        SM.gameObject.GetComponentInChildren<Animator>().SetBool("IsKilling", false);
        SM.gameObject.GetComponent<NPCController>().portraitRef = "Cleaning Drone";
        //Debug.Log(SM.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }
}
