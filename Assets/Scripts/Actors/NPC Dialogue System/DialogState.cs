using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/**
 *      Script used to create the DialogStates Scriptable Objects
 *      Each state represents a state the NPC is in, with various possible speeches
 *      The change between states should be handled by the DialogSM
 */
[CreateAssetMenu(fileName ="Dialog State",menuName = "States/Dialog State")]
public class DialogState : BaseState
{
    [TextArea(3,10)]
    [SerializeField] protected string[] speech;               //the different speech bubbles accsessible in a state (in descending order)
    protected int interactionIndex;                           //indicator of the current speech bubble
    [HideInInspector] public string currentSpeech;          //current selected speech bubble

    public DialogState(string name, DialogSM SM) : base(name, SM) { }

    protected StateMachine SM;
    
    /// <summary>
    /// Upon entering the dialogState, the current Speech that will be used by the NPC will be the first one of the list
    /// </summary>
    /// <param name="sm">State machine operating said state</param>
    public override void Enter(StateMachine sm)
    {
        SM = sm;
        base.Enter(SM);
        currentSpeech = speech[0];
        interactionIndex = 0;
        GetSpeechVariables(SM);
    }


    /// <summary>
    /// Function that will change the currently displayed speech of the state : at every call of the function by external scripts, will switch to next speech until the last one
    /// </summary>
    /// <returns>0 if the currentSpeech is the last speech, 1 otherwise</returns>
    public virtual int ChangeSpeech()
    {
        interactionIndex++;
        if (interactionIndex < speech.Length)
        {
            currentSpeech = speech[interactionIndex];
            GetSpeechVariables(SM);
            return 1;
        }
        else //when reaching the end of the various speeches, the NPC will repeat the last inputted speech
        {
            currentSpeech = speech[^1];
            GetSpeechVariables(SM);
            return 0;
        }
    }

    /// <summary>
    /// Function that will recover the NPC's data for the current spoken speech, if need be
    /// Relevant data is seen with $dataname$ tag
    /// </summary>
    /// <param name="SM">State Machine operating state</param>
    protected void GetSpeechVariables(StateMachine SM)
    {
        if (currentSpeech.Contains("$"))
        {
            String newSpeech = "";
            newSpeech += currentSpeech.Split("$")[0];
            newSpeech += SM.gameObject.GetComponent<NPCController>().GetPropertyValue(currentSpeech.Split("$")[1]);
            newSpeech += currentSpeech.Split("$")[2];
            currentSpeech = newSpeech;
        }
    }
}
