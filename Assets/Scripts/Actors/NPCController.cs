using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

/**
 *      Main component of the NPCs that will control their behaviors and overall actions
 */
public class NPCController : MonoBehaviour, Interactable
{
    //[SerializeField] private GameObject interactionPrompt; //Interaction prompt displayed when player is in interaction range with the NPC
    [SerializeField] private bool canBeInteracted;
    bool Interactable.canBeInteracted { get; set; }

    private bool inInteraction;                             //if the NPC is currently being interacted with
    [Header("Dialogue elements")]
    [SerializeField] private string portraitRef;            //reference to the portraits of the NPC -> should be rather moved to the states (each states contains their own refs to the portraits)
    private DialogueUIController ui;                        //reference to the UI used for dialogs
    private DialogSM dialogSM;                              //reference to the NPC's dialogSM

    private void Start()
    {
        inInteraction = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        dialogSM = GetComponent<DialogSM>();
        Assert.IsNotNull(dialogSM);
        Assert.IsNotNull(ui);
    }

    /**
     *  Inherited from the interface, interact method that will trigger the interactions with the player i.e. the dialogue
     *  For now, at each click on E button, Interact() will be called, updating the text 
     */
    public void Interact()
    {
        inInteraction = true;
        ui.DisplayDialogue(dialogSM.currentState.ConvertTo<DialogState>().currentSpeech, portraitRef); //visual display of the text
        dialogSM.OnDialogInteraction(); //the state machine's internal changes switching to the next dialogue line
    }

    /**
     *  Function triggered by external scripts yet to be defined that will change the NPC's state
     *  Param :
     *      newStateIndex : int : index that references the next state that should be chosen
     */
    public void OnStateChange(int newStateIndex)
    {
        dialogSM.ChangeState(newStateIndex);
        ui.EndDisplay();
    }
}
