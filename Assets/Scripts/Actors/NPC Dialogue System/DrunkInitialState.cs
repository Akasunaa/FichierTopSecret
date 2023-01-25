using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drunk Initial Dialog State", menuName = "States/Drunk Initial Dialog State")]
public class DrunkInitialState : DialogState
{
    public DrunkInitialState(string name, DialogSM SM) : base(name, SM)
    {
    }

    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        Debug.Log("NPC DRUNK : SWITCHING TO SLEEP");
        SM.gameObject.GetComponentInChildren<Animator>().Play("sleep");
    }

    public override void Exit(StateMachine sm)
    {
        base.Exit(sm);
        Debug.Log("NPC DRUNK : SWITCHING TO IDLE");
        SM.gameObject.GetComponentInChildren<Animator>().SetBool("IsSleeping", false);
        Debug.Log(SM.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }
}
