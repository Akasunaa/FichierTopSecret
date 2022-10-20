using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/**
 *      Script used to create the DialogStates Scriptable Objects
 *      Each state represents a state the NPC is in, with various possible speeches
 *      The change between states should be handled by the DialogSM
 */
[CreateAssetMenu(fileName ="Dialog State",menuName ="States")]
public class DialogState : BaseState
{
    [TextArea(3,10)]
    [SerializeField] private string[] speech;           //the different speech bubbles accsessible in a state (in descending order)
    private int interactionIndicator;                   //indicator of the current speech bubble
    [HideInInspector] public string currentSpeech;      //current selected speech bubble
    [SerializeField] public DialogState nextState;      //next state from the current state (called upon the change state method of the State machine)

    public DialogState(string name, DialogSM SM) : base(name, SM) { }

    public override void UpdateStateLogic()
    {
        base.UpdateStateLogic();
        Debug.Log("NPC : " + currentSpeech);
    }

    public override void Enter()
    {
        base.Enter();
        currentSpeech = speech[0];
        interactionIndicator = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    /**
     *  Function that will change the currently displayed speech of the state
     */
    public void ChangeSpeech()
    {
        interactionIndicator++;
        if (interactionIndicator < speech.Length)
        {
            currentSpeech = speech[interactionIndicator];
        }
        else //when reaching the end of the various speeches, the NPC will repeat the last inputted speech
        {
            currentSpeech = speech[speech.Length - 1];
        }
    }
}
