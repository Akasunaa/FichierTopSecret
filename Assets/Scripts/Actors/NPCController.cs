using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

/**
 *      Main component of the NPCs that will control their behaviors and overall actions
 */
public class NPCController : ModifiableController, Interactable
{
    [SerializeField] private bool canBeInteracted;
    bool Interactable.canBeInteracted { get; set; }

    private bool shouldEnd;
    [Header("Dialogue elements")]
    [SerializeField] private string portraitRef;            //reference to the portraits of the NPC -> should be rather moved to the states (each states contains their own refs to the portraits)
    private DialogueUIController ui;                        //reference to the UI used for dialogs
    private DialogSM dialogSM;                              //reference to the NPC's dialogSM

    [Header("DEBUG")] //DEBUG VARIABLES, SHOULD BE REMOVED 
    [HideInInspector, SerializeField] private bool changeState;
    [HideInInspector, SerializeField] private string stateName;

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
        if (changeState) { changeState = false; OnStateChange(stateName);}//DEBUG SHOULD BE REMOVED
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
    public void OnStateChange(string newStateName)
    {
        dialogSM.ChangeState(newStateName);
        ui.EndDisplay();
    }

    /**
     *  Function inherited from ModifiableController
     */
    public override void setDefaultProperties()
    {
        properties.Add("name", "Pouet Lord, Emperor of the Pouets");
    }

    /**
     *  Function that, FOR NOW, handle the modifications of the NPC files
     */
    public override void UpdateModification()
    {
        base.UpdateModification();
        if (properties.ContainsKey("name")) //TEST FOR CHANGED NAME ==> HARDCODED = SHIT
        {
            if (properties["name"] != "Pouet Lord, Emperor of the Pouets")
            {
                OnStateChange("StateChangedName"); //DEFINITELY SHOULD CHANGE STATE CHANGE SYSTEM (index is not the best)
            }
        }
    }

    /**
     *  Function called everytime the game gains or loses focus
     *  At these times, the Duplication Manager will check for gameObjects of a certain tag and trigger an event
     */
    private void OnApplicationFocus()
    {
        int numLamp = DuplicationCheckManager.Instance.Search("Lamp"); //NPC counts the number of lamp instances
        ReactSearchCount(numLamp);
    }

    /**
     *  Function that reacts to a search count of a tag variable
     *  RIGHT NOW, it's hardcoded, should be REWORKED
     */
    private void ReactSearchCount(int num)
    {
        if (num > 2)
        {
            OnStateChange("StateManyLights");
        }
        else if (num == 0)
        {
            OnStateChange("StateNoLights");
        }
    }


}
