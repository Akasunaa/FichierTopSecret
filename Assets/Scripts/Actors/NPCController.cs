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

    private bool shouldEnd;
    [Header("Dialogue elements")]
    [SerializeField] private string portraitRef;            //reference to the portraits of the NPC -> should be rather moved to the states (each states contains their own refs to the portraits)
    private DialogueUIController ui;                        //reference to the UI used for dialogs
    private DialogSM dialogSM;                              //reference to the NPC's dialogSM

    [Header("DEBUG")] //DEBUG VARIABLES, SHOULD BE REMOVED 
    [SerializeField] private bool changeState;

    private void Start()
    {
        shouldEnd = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        dialogSM = GetComponent<DialogSM>();
        Assert.IsNotNull(dialogSM);
        Assert.IsNotNull(ui);
    }

    private void Update()
    {
        if (changeState) { changeState = false; OnStateChange(0);}//DEBUG SHOULD BE REMOVED
    }


    /**
     *  Inherited from the interface, interact method that will trigger the interactions with the player i.e. the dialogue
     *  For now, at each click on E button, Interact() will be called, updating the text 
     */
    public void Interact()
    {
        if (shouldEnd) //if in the previous interaction the player reached the end of the state's dialogue, rather than repeating the sentence, the NPC ends the dialogue (WITHOUT CHANGING STATE)
        {
            EndDialogue();
            return;
        }
        Time.timeScale = 0f;    //if player in interaction, then stop time to prevent movement
        ui.DisplayDialogue(dialogSM.currentState.ConvertTo<DialogState>().currentSpeech, portraitRef); //visual display of the text
        int ret = dialogSM.OnDialogInteraction(); //the state machine's internal changes switching to the next dialogue line
        shouldEnd = (ret == 0);
    }

    /**
     *  Function that ends the current dialogue being displayed, giving back control to player
     *  It does NOT change the state of the NPC : if interacted again, the NPC will repeat the current State's last sentence
     */
    private void EndDialogue()
    {
        ui.EndDisplay();
        Time.timeScale = 1f;    //if player in interaction, then stop time to prevent movement
        shouldEnd = false;      //allows the player to interact again
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
