using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public enum TYPE
    {
        INTEGER,
        STRING
    }

    [Serializable]
    public struct FILE_ELEMENTS                             //struct used for the dictionnary of the properties of the object 
    {
        public string propertyName;
        public string propertyValue;
        public TYPE propertyType;                           //MAYBE custom inspector as to avoid conditions not used depending on type ?
        public int propertyCondition;
        public string propertyChangeState;
    }
    [Header("File elements")]
    [SerializeField] private FILE_ELEMENTS[] fileElements;
    [HideInInspector] public Dictionary<string, FILE_ELEMENTS> propertyDict = new Dictionary<string, FILE_ELEMENTS>(); //Dictionnary that will contain all the properties inputted in the inspector of the NPC


    //[Header("DEBUG")] //DEBUG VARIABLES, SHOULD BE REMOVED 
    //[HideInInspector, SerializeField] private bool changeState;
    //[HideInInspector, SerializeField] private string stateName;

    private void Start()
    {
        shouldEnd = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        dialogSM = GetComponent<DialogSM>();
        Assert.IsNotNull(dialogSM);
        Assert.IsNotNull(ui);

        //Creating the dict of the values :
        foreach(var element in fileElements)
        {
            propertyDict.Add(element.propertyName, element);
        }
        //-----------------------------------
    }

    private void Update()
    {
        //if (changeState) { changeState = false; OnStateChange(stateName);}//DEBUG SHOULD BE REMOVED
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
     *      newStateName : string : name that references the next state that should be chosen
     */
    public void OnStateChange(string newStateName)
    {
        dialogSM.ChangeState(newStateName);
        ui.EndDisplay();
    }

    /**
     *  Function inherited from ModifiableController
     *  Should be reworked to use a list or something, rather than hard-coded properties.Add
     */
    public override void setDefaultProperties()
    {
        foreach(var element in propertyDict.Values)
        {
            properties.Add(element.propertyName, element.propertyValue);
        }
    }

    /**
     *  Function that, FOR NOW, handle the modifications of the NPC files
     */
    public override void UpdateModification()
    {
        base.UpdateModification();
        foreach (var propertyString in propertyDict.Keys) //for all properties in the NPC dico => SHOULD BE REWORKED, AS, FOR NOW, THE NPC REACTS TO THEFIRST VALUE IN THE DICO CHANGED, NOT THE LAST ONE UPDATED
        {
            if (properties.ContainsKey(propertyString) && propertyDict[propertyString].propertyType == TYPE.STRING) //we check if they exist in the file AND their the STRING type 
            {
                if (properties[propertyDict[propertyString].propertyName] != propertyDict[propertyString].propertyValue.ToString()) //we check if they changed
                {
                    OnStateChange(propertyDict[propertyString].propertyChangeState); //we change the state accordingly
                    return;
                }
            }
            else if(properties.ContainsKey(propertyString) && propertyDict[propertyString].propertyType == TYPE.INTEGER) // if type INTEGER
            {
                int integerValue;
                int.TryParse(properties[propertyString], out integerValue);
                if(integerValue < propertyDict[propertyString].propertyCondition) //AS OF RIGHT NOW, WE TEST FOR A PRESET CONDITION (should be reworked as either editor or something else)
                {
                    if (propertyDict[propertyString].propertyName == "health") //FOR NOW, IF HEALTH WE HAVE DIFFERENT OUTCOME
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                    else
                    {
                        OnStateChange(propertyDict[propertyString].propertyChangeState);
                        return;
                    }
                    
                }
            }
        }
        OnStateChange("StateIdle");
    }

    /**
     *  Function called everytime the game gains or loses focus
     *  At these times, the Duplication Manager will check for gameObjects of a certain tag and trigger an event
     */
    private void OnApplicationFocus()
    {
        int numLamp = DuplicationCheckManager.Instance.Search("Lamp"); //NPC counts the number of lamp instances
        int numNpc = DuplicationCheckManager.Instance.Search("NPC"); //NPC counts the number of lamp instances
        ReactSearchCount(numLamp,numNpc);
    }

    /**
     *  Function that reacts to a search count of a tag variable
     *  RIGHT NOW, it's hardcoded, should be REWORKED
     *      THE UPPER IFS TAKE PRECEDENCE OVER THE OTHER ONES
     */
    private void ReactSearchCount(int numLamp, int numNPC)
    {
        if (numNPC > 3)
        {
            OnStateChange("StateCloneArmy");
            return;
        }
        else if (numNPC > 1)
        {
            OnStateChange("StateClone");
            return;
        }
        else if (numLamp > 1)
        {
            OnStateChange("StateManyLights");
            return;
        }
        else if (numLamp == 0)
        {
            OnStateChange("StateNoLights");
            return;
        }
        else
        {
            UpdateModification();
            return;
        }
    }


}
