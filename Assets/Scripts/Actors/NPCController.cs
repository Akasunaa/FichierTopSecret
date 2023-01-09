using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

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

    [Header("File Properties")]
    [SerializeField] public FILE_PROPERTIES[] fileProperties;
    [HideInInspector] public Dictionary<string, FILE_PROPERTIES> propertyDict = new Dictionary<string, FILE_PROPERTIES>(); //Dictionnary that will contain all the properties inputted in the inspector of the NPC

    [Header("Element to check")]
    [SerializeField] public PLAYER_ITEMS[] objectsElements;
    [HideInInspector] public Dictionary<string, PLAYER_ITEMS> objectDict = new Dictionary<string, PLAYER_ITEMS>(); //Dictionnary that will contain all the properties inputted in the inspector of the NPC

    [Header("Quest item")]
    [SerializeField] public QUEST_ITEMS[] questItems;
    [HideInInspector] public Dictionary<string,QUEST_ITEMS> questItemsDict = new Dictionary<string, QUEST_ITEMS>();

    [Header("Tagged elements to react to")]
    [SerializeField] public REACT_ELEMENTS[] reactElements;
    [HideInInspector] public Dictionary<string, REACT_ELEMENTS> reactElementsDict = new Dictionary<string, REACT_ELEMENTS>();

    [Header("Deplacement")]
    [SerializeField] private Grid grid;
    [SerializeField] bool canMoving;
    bool isWaiting;

    private void Start()
    {
        shouldEnd = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        dialogSM = GetComponent<DialogSM>();
        Assert.IsNotNull(dialogSM);
        Assert.IsNotNull(ui);

        //Creating the dict of the values :
        foreach(var element in fileProperties)
        {
            if (!propertyDict.ContainsKey(element.propertyName))
            {
                propertyDict.Add(element.propertyName, element);
            }
        }
        //-----------------------------------

        //Creating the dict of the objects :
        foreach (var element in objectsElements)
        {
            if (!objectDict.ContainsKey(element.playerItemName))
            {
                objectDict.Add(element.playerItemName, element);
            }
        }

        //For the movement
        isWaiting = false;
        //-----------------------------------

        //Creating the dict of the quest items to give out :
        foreach (var element in questItems)
        {
            if (!questItemsDict.ContainsKey(element.questChangeState))
            {
                questItemsDict.Add(element.questChangeState, element);
            }
        }
        //-----------------------------------

        //Creating the dict of the react elements to check out for :
        foreach (var element in reactElements)
        {
            if (!reactElementsDict.ContainsKey(element.tagToReact))
            {
                reactElementsDict.Add(element.tagToReact, element);
            }
        }
        //-----------------------------------

        //UpdateModification();
    }

    private void Update()
    {
        //if (changeState) { changeState = false; OnStateChange(stateName);}//DEBUG SHOULD BE REMOVED
        if (canMoving && !isWaiting) { Movement(); }

    }

    private void Movement()
    {
        int randomTimer = Random.Range(5, 10); //to serialize
        int randomDistance = Random.Range(1, 4);
        int randomDirection = Random.Range(0, 4);
        StartCoroutine(WaitForMovement(randomTimer,randomDistance,randomDirection));
    }
    
    IEnumerator WaitForMovement(int timer, int distance, int direction)
    {
        isWaiting = true;
        Vector3Int actualGridPosition = grid.WorldToCell(transform.position);
        Vector3Int targetGridPosition = new Vector3Int(0, 0, 0);
        switch (direction)
        {
            case 0:
                targetGridPosition[0] = 1;
                break;
            case 1:
                targetGridPosition[0] = -1;
                break;
            case 2:
                targetGridPosition[1] = 1;
                break;
            case 3:
                targetGridPosition[1] = -1;
                break;
        }
       for (Vector3Int moved = targetGridPosition; moved.magnitude <= distance; moved += targetGridPosition)
        {
            if (!Utils.CheckPresenceOnTile(grid, actualGridPosition + moved)) { 
                transform.position += targetGridPosition; 
            }
            
        }
        yield return new WaitForSeconds(timer);
        isWaiting = false;
    }       


    /**
     *  Inherited from the interface, interact method that will trigger the interactions with the player i.e. the dialogue
     *  For now, at each click on E button, Interact() will be called, updating the text 
     */
    public void Interact()
    {
        foreach (var checkedObject in objectDict.Keys) //the NPC will check if the player has the items that he needs to check for when the interaction starts
        {
            bool playerHasObject = ScanPlayerInventory(checkedObject); //the NPC will scan the player's Inventory
            if (playerHasObject)
            {
                OnStateChange(objectDict[checkedObject].playerItemChangeState); //if player has the item, change the NPC's state accordingly

                if (questItemsDict.ContainsKey(objectDict[checkedObject].playerItemChangeState)) //if the NPC changes state by recognizing that the player has a certain item, and that the state correspondes to a quest item, the npc will give out said item
                {
                    GiveItem(questItemsDict[objectDict[checkedObject].playerItemChangeState].item);
                    questItemsDict.Remove(objectDict[checkedObject].playerItemChangeState); //we remove the item to avoid giving it twice
                }
            }
        }
        if (shouldEnd) //if in the previous interaction the player reached the end of the state's dialogue, rather than repeating the sentence, the NPC ends the dialogue (WITHOUT CHANGING STATE)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartMovement();
            EndDialogue();
            return;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
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
        shouldEnd = false;      //allows the player to interact again
    }

    /**
     *  Function triggered by external scripts yet to be defined that will change the NPC's state
     *  Param :
     *      newStateName : string : name that references the next state that should be chosen
     */
    public void OnStateChange(string newStateName)
    {
        dialogSM = GetComponent<DialogSM>();
        dialogSM.ChangeState(newStateName);
        if (ui!=null) 
        {
            ui.EndDisplay();
        } 
    }

    /**
     *  Function inherited from ModifiableController
     *  Should be reworked to use a list or something, rather than hard-coded properties.Add
     */
    public override void SetDefaultProperties()
    {
        foreach(var element in propertyDict.Values)
        {
            // as they are default properties, they are considered as important
            properties.Add(element.propertyName, new DicoValueProperty {IsImportant = true, Value = element.propertyValue});
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
                if (properties[propertyDict[propertyString].propertyName].Value.ToString() != propertyDict[propertyString].propertyValue.ToString()) //we check if they changed
                {
                    OnStateChange(propertyDict[propertyString].propertyChangeState[0]); //we change the state accordingly
                    return;
                }
                //else //maybe this else condition should be removed or changed -> otherwise, we might switch back to initial state when not needed
                //{
                //    OnStateChange(dialogSM.GetInitialState().name);
                //}
            }
            else if(properties.ContainsKey(propertyString) && propertyDict[propertyString].propertyType == TYPE.INTEGER) // if type INTEGER, hence for list of values
            {
                int integerValue;
                int.TryParse(properties[propertyString].Value.ToString(), out integerValue);
                for(int conditionListIndex = 0;conditionListIndex< propertyDict[propertyString].propertyCondition.Length;conditionListIndex++)
                {
                    if (propertyDict[propertyString].conditionIsSuperior[conditionListIndex]) //if the condition is a superior one
                    {
                        if (integerValue < propertyDict[propertyString].propertyCondition[conditionListIndex]) //AS OF RIGHT NOW, WE TEST FOR A PRESET CONDITION (should be reworked as either editor or something else)
                        {
                            if (propertyDict[propertyString].propertyName == "health") //FOR NOW, IF HEALTH WE HAVE DIFFERENT OUTCOME
                            {
                                gameObject.SetActive(false);
                                return;
                            }
                            else
                            {
                                OnStateChange(propertyDict[propertyString].propertyChangeState[conditionListIndex]);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (integerValue > propertyDict[propertyString].propertyCondition[conditionListIndex]) //AS OF RIGHT NOW, WE TEST FOR A PRESET CONDITION (should be reworked as either editor or something else)
                        {
                            OnStateChange(propertyDict[propertyString].propertyChangeState[conditionListIndex]);
                            return;
                        }
                    }
                }
            }
        }
        OnStateChange(dialogSM.GetInitialState().name); //if by that point nothing returned (triggered) the state changed, NPC should return to initial state
    }

    /**
     *  Function called everytime the game gains or loses focus
     *  At these times, the Duplication Manager will check for gameObjects of a certain tag and trigger an event
     */
    private void OnApplicationFocus()
    {
        foreach(var elementTag in reactElementsDict.Keys) //for each tag that the NPC must look out for, they will scan for it and then react
        {
            var elementTagCount = DuplicationCheckManager.Instance.Search(elementTag);
            //Debug.Log("NPC : FOUND " + elementTagCount + " ELEMENTS OF TAG " + elementTag);
            ReactSearchCount(elementTag, elementTagCount);
        }
    }

    /**
     *  Function that reacts to a search count of a tag variable
     *      THE UPPER IFS TAKE PRECEDENCE OVER THE OTHER ONES
     */
    private void ReactSearchCount(string searchedTag, int tagCount)
    {
        if (reactElementsDict.ContainsKey(searchedTag))
        {
            //the NPC can have various reactions depending on the number : as such, the list of conditions and possible states can be >1 in length to have different outcomes
            //However, that means that elements with low index take precedence over others
            for (int i = 0; i < reactElementsDict[searchedTag].condition.Length; i++)
            {
                if (reactElementsDict[searchedTag].isSuperior[i]) //if the tested condition is a superior one, we'll test for a superior condition
                {
                    if (tagCount > reactElementsDict[searchedTag].condition[i])
                    {
                        OnStateChange(reactElementsDict[searchedTag].stateChangeName[i]);
                        return;
                    }
                }
                else
                {
                    if (tagCount < reactElementsDict[searchedTag].condition[i])
                    {
                        OnStateChange(reactElementsDict[searchedTag].stateChangeName[i]);
                        return;
                    }
                }
               
            }
        }
        //UpdateModification();
        return;
    }

    /**
     *  Function that will recuperates a certain value from the properties, if such value exists
     */
    public string GetPropertyValue(string propertyName)
    {
        return properties.ContainsKey(propertyName) ? properties[propertyName].Value.ToString() : "DATA NOT FOUND";
    }

    /**
     *  Function that will scan the player's inventory for a specific object
     *  Param :
     *      @objectName : String : name of the object the npc will check for
     */
    private bool ScanPlayerInventory(String objectName)
    {
        var fileInfo = new FileInfo(Application.streamingAssetsPath + "/Test/Player/" + objectName + ".txt");
        return fileInfo.Exists;
    }

    /**
    *  Function called when the npc changes state by responding to a player bringing a correct item
    *  It will create an instance of the stored item, and call its internal ItemController.RecuperatingItem() function
    */
    private void GiveItem(GameObject item)
    { 
        var newItem = Instantiate(item, GameObject.FindGameObjectWithTag("Player").transform, true);
        newItem.GetComponent<ItemController>().RecuperatingItem();
    }
}

[System.Serializable]
public enum TYPE                                        //enum used to indicate if a property is of type integer or string
{
    INTEGER,
    STRING
}

[System.Serializable]
public struct FILE_PROPERTIES                             //struct used for the dictionnary of the properties of the object 
{
    public string propertyName;                         //name of the property
    public string propertyValue;                        //value of the property
    public TYPE propertyType;                           //MAYBE custom inspector as to avoid conditions not used depending on type ?
    public bool[] conditionIsSuperior;                   //list of the booleans that indicate wether the associated properties are superior or inferior ones
    public int[] propertyCondition;                     //list of conditions of the property
    public string[] propertyChangeState;                //associated state of the property if changed
}

[System.Serializable]
public struct PLAYER_ITEMS                          //struct used for the dictionnary of the objects that the NPC will look out for in the player's inventory
{
    public string playerItemName;                           //name of the object (must be exact)
    public string playerItemChangeState;                    //state that will change if object detected in player's inventory
}

[System.Serializable]
public struct QUEST_ITEMS                               //struct used for the dictionnary of the quest items the NPC will give out upon trigger by a certain state's name
{
    public GameObject item;                             //item that the NPC will give the player
    public string questChangeState;                    //state that will trigger the NPC giving the item
}

[System.Serializable]
public struct REACT_ELEMENTS                            //struct used for elements that the NPC will react to by their tag 
{
    public string tagToReact;                           //tag that it will react to
    public string[] stateChangeName;                    //associated name of the state
    public bool[] isSuperior;                           //if the associated condition is a superior condition or not
    public int[] condition;                             //the value for the condition
}