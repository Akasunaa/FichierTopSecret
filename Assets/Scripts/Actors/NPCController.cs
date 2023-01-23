using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements.Experimental;
using static FilesWatcher;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
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
    public string portraitRef;            //reference to the portraits of the NPC -> should be rather moved to the states (each states contains their own refs to the portraits)
    private DialogueUIController ui;                        //reference to the UI used for dialogs
    private DialogSM dialogSM;                              //reference to the NPC's dialogSM

    [Header("File Properties")]
    [SerializeField] public FILE_PROPERTIES[] fileProperties;
    [HideInInspector] public Dictionary<string, FILE_PROPERTIES> propertyDict = new Dictionary<string, FILE_PROPERTIES>(); //Dictionnary that will contain all the properties inputted in the inspector of the NPC

    [Header("Player Items to check")]
    [SerializeField] public PLAYER_ITEMS[] objectsElements;
    [HideInInspector] public Dictionary<string, PLAYER_ITEMS> objectDict = new Dictionary<string, PLAYER_ITEMS>(); //Dictionnary that will contain all the properties inputted in the inspector of the NPC

    [Header("Quest item")]
    [SerializeField] public QUEST_ITEMS[] questItems;
    [HideInInspector] public Dictionary<string,QUEST_ITEMS> questItemsDict = new Dictionary<string, QUEST_ITEMS>();

    [Header("Tagged elements to react to")]
    [SerializeField] public REACT_ELEMENTS[] reactElements;
    [HideInInspector] public Dictionary<string, REACT_ELEMENTS> reactElementsDict = new Dictionary<string, REACT_ELEMENTS>();

    [Header("Player.txt elements to react to")]
    [SerializeField] public PLAYER_PROPERTIES[] playerProperties;
    [HideInInspector] public Dictionary<string, PLAYER_PROPERTIES> playerPropertiesDict = new Dictionary<string, PLAYER_PROPERTIES>();

    [Header("Player Prefs elements to react to")]
    [SerializeField] public PLAYER_PREFS_ELEMENTS[] playerPrefsElements;
    [HideInInspector] public Dictionary<string,PLAYER_PREFS_ELEMENTS> playerPrefsElementsDict = new Dictionary<string,PLAYER_PREFS_ELEMENTS>();

    [Header("Deplacement")]
    [SerializeField] private Grid grid;
    [SerializeField] public bool shouldMove; //if we want this NPC moving
    [SerializeField] private float speed=3f;
    private Animator animator;
    private bool isWaiting=false;
    static private bool canMove = true; //if the NPC can moving (not in dialogState)
    private IEnumerator lastSmoothMov;

    //player's informations
    private GameObject player;
    private PlayerObjectController playerObjectController;
    private bool newDialog = true;

    private void Awake()
    {
        dialogSM = GetComponent<DialogSM>();
        Assert.IsNotNull(dialogSM);
    }

    private void Start()
    {
        shouldEnd = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
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

        //Creating the dict of the player.txt elements to check out for :
        foreach (var element in playerProperties)
        {
            if (!playerPropertiesDict.ContainsKey(element.playerPropertyName))
            {
                playerPropertiesDict.Add(element.playerPropertyName, element);
            }
        }
        //-----------------------------------

        //Creating the dict of the Player Prefs Elements to check out for :
        foreach (var element in playerPrefsElements)
        {
            if (!playerPrefsElementsDict.ContainsKey(element.playerPrefsName))
            {
                playerPrefsElementsDict.Add(element.playerPrefsName, element);
            }
        }
        //-----------------------------------

        //we set up the different variables of the NPC controller that do not require external help :
        player = GameObject.FindGameObjectWithTag("Player");
        playerObjectController = player.GetComponent<PlayerObjectController>();
        animator = GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.speed = speed;
        }
        grid = SceneData.Instance.grid;

        //when NPC is initializing, we try to check if the player Prefs have been altered
        SearchPlayerPrefs();
    }

    /// <summary>
    /// Function purposely delayed to ensure that the NPC is unloaded when he was killed in the previous scene launched.
    /// TODO : SHOULD BE FIXED OTHERWISE (PIERRE)
    /// </summary>
    private void TestHealth()
    {
        //Debug.Log("NPC : " + gameObject.name + " TESTHEALTH CALLED, 0.3f seconds after start");
        int hp = 0;
        if (properties.ContainsKey("health"))
        {
            int.TryParse(properties["health"].Value.ToString(), out hp);
            //Debug.Log("NPC : " + gameObject.name + " PROPERTY HEALTH WITH VALUE " + properties["health"].Value);
        }

        if (properties.ContainsKey("health") && hp <= 0)
        {
            //TRIGGER DEATH
            //Debug.Log("NPC : " + gameObject.name + " PROPERTY HEALTH WITH VALUE " + properties["health"].Value + " INITIALLY LEADING TO DEATH");
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //if (changeState) { changeState = false; OnStateChange(stateName);}//DEBUG SHOULD BE REMOVED
        if (shouldMove && !isWaiting && canMove) { NewRandomMovement(); }
    }

    #region MOVEMENT_FUNCTIONS
    /**
    * Launch a new random movement of the NPC 
    */
    private void NewRandomMovement()
    {
        float movementCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length/speed;
        int randomDistance = Random.Range(1, 4);
        int randomTimer = Random.Range((int)movementCooldown*randomDistance*2+1, (int)movementCooldown * randomDistance*3+1);        
        StartCoroutine(Deplacement(randomTimer,randomDistance));
    }
    
    /**
    * Check if NPC can do the deplacement and launch it, then wait for the next movement 
    */
    IEnumerator Deplacement(int timer, int distance)
    { 
        isWaiting = true;
        Vector3Int actualGridPosition = grid.WorldToCell(transform.position);
        Vector3Int vectorDirection = new Vector3Int(0, 0, 0);
        List<Vector3Int> targetPositions = new List<Vector3Int>();
        switch (Random.Range(0, 4))
        {
            case 0:
                vectorDirection[0] = 1;
                break;
            case 1:
                vectorDirection[0] = -1;
                break;
            case 2:
                vectorDirection[1] = 1;
                break;
            case 3:
                vectorDirection[1] = -1;
                break;
        }
        for (Vector3Int moved = vectorDirection; moved.magnitude <= distance; moved += vectorDirection)
        {
            if (!Utils.CheckPresenceOnTile(grid, actualGridPosition + moved)) {
                targetPositions.Add(actualGridPosition + moved);
            }
            else
            {
                targetPositions.Clear();
                break;
            }
        }
        if (targetPositions.Any())
        {
            lastSmoothMov = SmoothMovement(targetPositions);
            StartCoroutine(lastSmoothMov);
        }
        yield return new WaitForSeconds(timer);
        isWaiting = false;
    }

    /**
     *  launch animation of the moving NPC
     */
    private IEnumerator SmoothMovement(List<Vector3Int> targetPositions)
    {
        // keep initial position
        Vector3 initialPosition = transform.position;
        //turn the sprite before animation
        UpdateSpriteOrientation(initialPosition.x - targetPositions[0].x, targetPositions[0].y - initialPosition.y);
        // get animation clip information 
        animator.SetTrigger("WalkTrigger");
        animator.Update(0.001f);
        float movementCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed;
        float timer = 0;
        while (timer < movementCooldown)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, grid.GetCellCenterWorld(targetPositions[0]), timer / movementCooldown);
            Utils.UpdateOrderInLayer(gameObject);
            yield return null;
        }
        player.GetComponent<PlayerMovement>().RefreshOrientationSprite(); //Permet de reset l'interaction avec le joueur 
        targetPositions.RemoveAt(0);
        if (targetPositions.Any() && !Utils.CheckPresenceOnTile(grid, targetPositions[0]) && canMove)
        {
            lastSmoothMov = SmoothMovement(targetPositions);
            StartCoroutine(lastSmoothMov);
        }
    }


    /**
    * Update the sprite when move or interact
    */
    private void UpdateSpriteOrientation(float dirX, float dirY)
    {
        if (animator)
        {
            animator.SetFloat("dirX", dirX);
            animator.SetFloat("dirY", dirY);
        }
    }
    #endregion

    /// <summary>
    /// Inherited from the interface, interact method that will trigger the interactions with the player i.e.the dialogue
    /// For now, at each click on E button, Interact() will be called, updating the text
    /// </summary>
    public void Interact()
    {
        //block and facing the player
        canMove = false;
        if(lastSmoothMov!=null)
            StopCoroutine(lastSmoothMov);
        transform.position = grid.GetCellCenterWorld(grid.WorldToCell(transform.position));
        UpdateSpriteOrientation(-GameObject.FindGameObjectWithTag("Player").transform.position.x+transform.position.x,
            GameObject.FindGameObjectWithTag("Player").transform.position.y - transform.position.y);


        bool prefHasChanged=false;
        if (SearchPlayerPrefs()) //if a state has changed due to player Prefs, it takes priority and sets a boolean that will avoid the NPC changing state
        {
            prefHasChanged = true;
        }

        foreach (var checkedObject in objectDict.Keys) //the NPC will check if the player has the items that he needs to check for when the interaction starts
        {
            bool playerHasObject = ScanPlayerInventory(checkedObject); //the NPC will scan the player's Inventory
            if (playerHasObject && !prefHasChanged && !objectDict[checkedObject].hasReacted) //if player has the item, has not changed due to player prefs AND is not already responding to the item, change the NPC's state accordingly
            {
                //Debug.Log("NPC : " + gameObject.name + " REACTING TO PLAYER ITEMS WITH CURRENT STATE : " + dialogSM.GetCurrentStateName() + " TO STATE " + objectDict[checkedObject].playerItemChangeState+ " WITH HASREACTED BOOL OF VALUE : "+ objectDict[checkedObject].hasReacted);
                OnStateChange(objectDict[checkedObject].playerItemChangeState);
                if (questItemsDict.ContainsKey(objectDict[checkedObject].playerItemChangeState)) //if the NPC changes state by recognizing that the player has a certain item, and that the state correspondes to a quest item, the npc will give out said item
                {
                    //Debug.Log("NPC : GIVING ITEM");
                    LevelManager.GiveItem(questItemsDict[objectDict[checkedObject].playerItemChangeState].item);
                    questItemsDict.Remove(objectDict[checkedObject].playerItemChangeState); //we remove the item to avoid giving it twice
                }
                PLAYER_ITEMS newItem = new PLAYER_ITEMS();
                newItem.hasReacted = true;
                newItem.playerItemChangeState = objectDict[checkedObject].playerItemChangeState;
                newItem.playerItemName = objectDict[checkedObject].playerItemName;
                objectDict[checkedObject] = newItem;
                //Debug.Log("NPC : SETTING HASREACTED BOOL TO VALUE : " + objectDict[checkedObject].hasReacted);
                break;
            }
        }
        if (shouldEnd) //if in the previous interaction the player reached the end of the state's dialogue, rather than repeating the sentence, the NPC ends the dialogue (WITHOUT CHANGING STATE)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartMovement();
            EndDialogue();
            return;
        }
        player.gameObject.GetComponent<InputController>().StopMovement();
        if (newDialog)
        {
            player.gameObject.GetComponent<PlayerObjectController>().InteractSound();
            newDialog = false;
        }
        ui.DisplayDialogue(dialogSM.currentState.ConvertTo<DialogState>().currentSpeech, portraitRef); //visual display of the text
        int ret = dialogSM.OnDialogInteraction(); //the state machine's internal changes switching to the next dialogue line
        shouldEnd = (ret == 0);
    }

    /// <summary>
    ///  Function that ends the current dialogue being displayed, giving back control to player
    ///     It does NOT change the state of the NPC : if interacted again, the NPC will repeat the current State's last sentence
    /// </summary>
    private void EndDialogue()
    {
        newDialog = true;
        canMove = true;
        ui.EndDisplay();
        shouldEnd = false;      //allows the player to interact again
    }

    /// <summary>
    /// Function that will change the NPC's state
    /// </summary>
    /// <param name="newStateName">name that references the next state that should be chosen</param>
    public void OnStateChange(string newStateName)
    {
        Debug.Log("NPC : " + gameObject.name + " CHANGING CURRENT STATE " + dialogSM.currentState.name + " TO STATE " + newStateName);
        dialogSM = GetComponent<DialogSM>();
        dialogSM.associatedNPCController = this;
        dialogSM.ChangeState(newStateName);
        if (ui!=null) 
        {
            ui.EndDisplay();
        }
    }

    /// <summary>
    /// Function inherited from ModifiableController
    /// Should be reworked to use a list or something, rather than hard-coded properties.Add
    /// </summary>
    public override void SetDefaultProperties()
    {
        foreach (var propertyKey in propertyDict.Keys)
        {
            // as they are default properties, they are considered as important
            //Debug.Log("NPC : " + gameObject.name + " SetDefaultProperties : value considered : " + propertyDict[propertyKey].propertyName);
            properties.TryAdd(propertyDict[propertyKey].propertyName, new DicoValueProperty { IsImportant = true, Value = propertyDict[propertyKey].propertyValue });
        }
        ////DEBUG -------------------------
        //foreach (var property in properties.Keys)
        //{
        //    Debug.Log("NPC " + gameObject.name + " : SetDefaultProperties : file properties value added : " + property);

        //}
        ////-------------------------------
    }

    /// <summary>
    /// Main Function of the NPC that will handle the modifications of its file.
    /// </summary>
    public override void UpdateModification(bool firstRead = false)
    {
        if (firstRead)
        {
            TestHealth();
        }

        base.UpdateModification();
        ////DEBUG -------------------------
        //foreach(var property in properties.Keys)
        //{
        //    Debug.Log("NPC " + gameObject.name + " : UpdateModification : file properties value considered : " + property);
        //}
        ////-------------------------------
        foreach (var propertyString in propertyDict.Keys) //for all properties in the NPC dico => SHOULD BE REWORKED, AS, FOR NOW, THE NPC REACTS TO THEFIRST VALUE IN THE DICO CHANGED, NOT THE LAST ONE UPDATED
        {
            //Debug.Log("NPC " + gameObject.name + " : UpdateModification : propertyDict value considered : " + propertyDict[propertyString].propertyName);
            if (properties.ContainsKey(propertyString) && propertyDict[propertyString].propertyType == TYPE.STRING) //we check if they exist in the file AND their the STRING type 
            {
                //Debug.Log("NPC " + gameObject.name + " : UpdateModification : propertyDict value : " + propertyDict[propertyString].propertyValue+" file value : "+ properties[propertyDict[propertyString].propertyName].Value.ToString());
                //Debug.Log("NPC " + gameObject.name + " : UpdateModification : conditions length : " + propertyDict[propertyString].propertyCondition.Length);
                if (propertyDict[propertyString].propertyCondition.Length > 0) //if there are various possible conditions to check for, we check for them
                {
                    for (int conditionListIndex = 0; conditionListIndex < propertyDict[propertyString].propertyCondition.Length; conditionListIndex++) //the NPC will check if the changed string corresponds to a certain value, if it does it will trigger specific state change
                    {
                        if (properties[propertyDict[propertyString].propertyName].Value.ToString().ToLower() == propertyDict[propertyString].propertyCondition[conditionListIndex].ToString().ToLower()) //we check if they changed
                        {
                            //Debug.Log("NPC " + gameObject.name + ": for STRING value " + propertyDict[propertyString].propertyName + " condition met, changing state to " + propertyDict[propertyString].propertyChangeState[conditionListIndex]);
                            OnStateChange(propertyDict[propertyString].propertyChangeState[conditionListIndex]); //we change the state accordingly
                            return;
                        }
                    }
                }
                if (properties[propertyDict[propertyString].propertyName].Value.ToString().ToLower() != propertyDict[propertyString].propertyValue.ToString().ToLower()) //if by default the change corresponds to nothing, the first state will be selected
                {
                    //Debug.Log("NPC " + gameObject.name + ": for STRING value " + propertyDict[propertyString].propertyName + " no conditions list found, and file value "+ properties[propertyDict[propertyString].propertyName].Value.ToString()+" different than saved value "+ propertyDict[propertyString].propertyValue.ToString() + ", leading to change state to "+ propertyDict[propertyString].propertyChangeState[0]);
                    OnStateChange(propertyDict[propertyString].propertyChangeState[0]); //we change the state accordingly
                    return;
                }
            }
            else if(properties.ContainsKey(propertyString) && propertyDict[propertyString].propertyType == TYPE.INTEGER) // if type INTEGER, hence for list of values
            {
                //Debug.Log("NPC "+gameObject.name+" : INTEGER value tested : " + propertyDict[propertyString].propertyName);
                int integerValue;
                int.TryParse(properties[propertyString].Value.ToString(), out integerValue);
                for(int conditionListIndex = 0;conditionListIndex < propertyDict[propertyString].propertyCondition.Length;conditionListIndex++)
                {
                    bool testConditionAcheived = Utils.NPCCompare(properties[propertyString].Value.ToString(), propertyDict[propertyString].propertyCondition[conditionListIndex], propertyDict[propertyString].conditionIsSuperior[conditionListIndex]);
                    if (testConditionAcheived)
                    {
                        OnStateChange(propertyDict[propertyString].propertyChangeState[conditionListIndex]);
                        return;
                    }
                    
                }
            }
        }
        OnStateChange(dialogSM.GetStartingState().name); //if by that point nothing returned (triggered) the state changed, NPC should return to initial state
    }

    /// <summary>
    /// Function called everytime the game gains or loses focus
    /// At these times, the Duplication Manager will check for gameObjects of a certain tag and trigger an event
    /// We will also use these times to check if the player.txt properties have changed
    /// </summary>
    /// <param name="hasFocus">Boolean indicating if application has focus or not</param>
    private void OnApplicationFocus(bool hasFocus)
    {
        //Check for Player.txt values :
        foreach(var playerElement in playerPropertiesDict.Keys) //for all the possible properties that the NPC will check in Player.txt, it will go through all the possibilities and change state accordingly
        {
            string value;
            if(playerObjectController.TryGet(playerElement,out value)) //we recuperate the value currently in Player.txt that we will test the possible conditions against
            {
                for(int playerPropertyConditionIndex=0; playerPropertyConditionIndex < playerPropertiesDict[playerElement].playerPropertyCondition.Length; playerPropertyConditionIndex++)
                {
                    //Debug.Log("NPC PLAYER TXT : Player.txt value found for key " + playerElement + " : " + value.ToString() + " | Value tested against  : " + playerPropertiesDict[playerElement].playerPropertyCondition[playerPropertyConditionIndex].ToString());
                    if(value.ToString() == playerPropertiesDict[playerElement].playerPropertyCondition[playerPropertyConditionIndex].ToString())
                    {
                        //Debug.Log("NPC PLAYER TXT : CHANGING STATE TO : " + playerPropertiesDict[playerElement].playerPropertyChangeState[playerPropertyConditionIndex]);
                        OnStateChange(playerPropertiesDict[playerElement].playerPropertyChangeState[playerPropertyConditionIndex]);
                        //Debug.Log("NPC PLAYER TXT : STATE CHANGED");
                        return;
                    }
                }
            }
            
        }
        //Check with Duplication Manager :
        foreach(var elementTag in reactElementsDict.Keys) //for each tag that the NPC must look out for, they will scan for it and then react
        {
            int elementTagCount = DuplicationCheckManager.Instance.Search(elementTag);
            //Debug.Log("NPC : FOUND " + elementTagCount + " ELEMENTS OF TAG " + elementTag);
            ReactSearchCount(elementTag, elementTagCount);
        }
    }

    /// <summary>
    /// Function that reacts to a search count of a tag variable
    /// It will linearily follow the order of tags inputted in the inspector's list
    /// </summary>
    /// <param name="searchedTag">tag searched for in the current scene</param>
    /// <param name="tagCount">number of elements that make a condition in the inspector, can be inferior or superior</param>
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
        return;
    }

    /// <summary>
    /// Function that will search the playerPrefs to see if modifications were added
    /// </summary>
    /// <returns>true if a state change was operated by reading the playerPrefs, false otherwise</returns>
    private bool SearchPlayerPrefs()
    {
        foreach(var playerPref in playerPrefsElementsDict.Keys)
        {
            if(PlayerPrefs.HasKey(playerPrefsElementsDict[playerPref].playerPrefsName) && PlayerPrefs.GetString(playerPrefsElementsDict[playerPref].playerPrefsName) == playerPrefsElementsDict[playerPref].playerPrefsCondition) //if the player pref value exists AND has been set to teh prerequisite condition does it change the NPC's state
            {
                OnStateChange(playerPrefsElementsDict[playerPref].playerPrefsChangeState);
                PlayerPrefs.SetString(playerPrefsElementsDict[playerPref].playerPrefsName,"READ");
                PlayerPrefs.Save();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Function that will recuperates a certain value from the properties, if such value exists
    /// </summary>
    /// <param name="propertyName">name of the property being looked for in the file</param>
    /// <returns>value of said property as a string if found, "DATA NOT FOUND" otherwise</returns>
    public string GetPropertyValue(String propertyName)
    {
        if (properties.ContainsKey(propertyName))
        {
            return properties[propertyName].Value.ToString();
        }
        return "DATA NOT FOUND";
    }

    /// <summary>
    /// Function that will scan the player's inventory for a specific object
    /// </summary>
    /// <param name="objectName">name of the object the npc will check for</param>
    /// <returns>true if file exists, false otherwise</returns>
    private bool ScanPlayerInventory(String objectName)
    {
        var fileInfo = new FileInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.PlayerFolderName + "/" + objectName + ".txt");
        return fileInfo.Exists;
    }

    /// <summary>
    /// Function called from special states, that will add specific and previously non-accessible properties to NPC's .txt files
    /// </summary>
    /// <param name="newFileProperties">List of FILE_PROPERTIES elements that will be added to NPC's file</param>
    public void AddProperty(FILE_PROPERTIES[] newFileProperties)
    {
        //we firstly add the values to the .txt file as to not suppress precedently written values :
        //then we add the new properties to the dict
        foreach (var element in newFileProperties)
        {
            if (!properties.ContainsKey(element.propertyName))
            {
                properties.Add(element.propertyName, new DicoValueProperty { IsImportant = true, Value = element.propertyValue });
            }
            if (!propertyDict.ContainsKey(element.propertyName))
            {
                propertyDict.Add(element.propertyName, element);
            }
        }
        UpdateFile();
    }
}

/// <summary>
/// enum used to indicate if a property is of type integer or string
/// </summary>
[System.Serializable]
public enum TYPE                                      
{
    INTEGER,
    STRING
}

/// <summary>
/// struct used for the dictionnary of the properties of the object
/// </summary>
[System.Serializable]
public struct FILE_PROPERTIES                             
{
    public string propertyName;                         //name of the property
    public string propertyValue;                        //value of the property
    public TYPE propertyType;                           //MAYBE custom inspector as to avoid conditions not used depending on type ?
    public bool[] conditionIsSuperior;                   //list of the booleans that indicate wether the associated properties are superior or inferior ones
    public string[] propertyCondition;                     //list of conditions of the property
    public string[] propertyChangeState;                //associated state of the property if changed
}

/// <summary>
/// struct used for the dictionnary of the objects that the NPC will look out for in the player's inventory
/// </summary>
[System.Serializable]
public struct PLAYER_ITEMS                          
{
    public string playerItemName;                           //name of the object (must be exact)
    public string playerItemChangeState;                    //state that will change if object detected in player's inventory
    public bool hasReacted;
}

/// <summary>
/// struct used for the dictionnary of the quest items the NPC will give out upon trigger by a certain state's name
/// </summary>
[System.Serializable]
public struct QUEST_ITEMS                               
{
    public GameObject item;                             //item that the NPC will give the player
    public string questChangeState;                    //state that will trigger the NPC giving the item
}

/// <summary>
/// struct used for elements that the NPC will react to by their tag 
/// </summary>
[System.Serializable]
public struct REACT_ELEMENTS                            
{
    public string tagToReact;                           //tag that it will react to
    public string[] stateChangeName;                    //associated name of the state
    public bool[] isSuperior;                           //if the associated condition is a superior condition or not
    public int[] condition;                             //the value for the condition
}

/// <summary>
/// struct used for values in the player.txt that the NPC will react to
/// </summary>
[System.Serializable]
public struct PLAYER_PROPERTIES                         
{
    public string playerPropertyName;
    public string[] playerPropertyCondition;                   //list of possibilities that property can take and the NPC will react to
    public string[] playerPropertyChangeState;                //associated state of the property if changed
}

/// <summary>
/// struct used for values in the PlayerPrefs that the NPC will react to
/// </summary>
[System.Serializable]
public struct PLAYER_PREFS_ELEMENTS                     
{
    public string playerPrefsName;
    public string playerPrefsCondition;                   //list of possibilities that property can take and the NPC will react to
    public string playerPrefsChangeState;                //associated state of the property if changed
}