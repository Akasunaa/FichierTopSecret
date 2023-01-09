using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

/**
 *  This class will be used to create a custom window used to create more efficiently NPCs
 *  
 */
public class NPCCreatorWindow : EditorWindow
{
    #region SYSTEM_FUNCTIONS
    //------------------------------------------------
    //SYSTEM FUNCTIONS
    //function that actually creates access to the editor :
    [MenuItem("Custom Tools/NPC Creator")]
    public static void DisplayWindow()
    {
        GetWindow<NPCCreatorWindow>("NPC CREATOR WINDOW");
    }

    //function that keeps updating the editor window :
    private void OnInspectorUpdate()
    {
        Repaint();
    }
    //------------------------------------------------
    #endregion

    [SerializeField] private GameObject basicNPC;   //the ref towards a "blank" NPC prefab that will be setup by the window
    private GameObject instantiatedNPC;             //the ref towards the NPC that will be created
    private DialogSM dialogSM;
    private NPCController npcController;
    private DialogueUIController dialogueUIController;  //ref towards the dialogueUIController present in scene
    private string errorProperties = "ERROR IN THE PROPERTIES FIELDS";
    private string errorPlayerItems = "ERROR IN THE PLAYER ITEM FIELDS";
    private string errorQuestItems = "ERROR IN THE QUEST ITEM FIELDS";
    private string errorReactElements = "ERROR IN THE REACT ELEMENTS FIELDS";
    private string errorPlayerProperties = "ERROR IN THE PLAYER PROPERTIES FIELDS";

    //Elements for npc :
    private string npcName;                         //name of the NPC

    //Elements for DialogSM :
    [SerializeField] DialogState startingState;
    [SerializeField] List<DialogStateEditorItem> availableStatesList = new List<DialogStateEditorItem>();

    //Elements for NPCController :
    private string portraitRef;
    [SerializeField] List<FILE_PROPERTIES> npcProperties = new List<FILE_PROPERTIES>();
    [SerializeField] List<PLAYER_ITEMS> playerItems = new List<PLAYER_ITEMS>();
    [SerializeField] List<QUEST_ITEMS> questItems = new List<QUEST_ITEMS>();
    [SerializeField] List<REACT_ELEMENTS> reactElements = new List<REACT_ELEMENTS>();
    [SerializeField] List<PLAYER_PROPERTIES> playerProperties = new List<PLAYER_PROPERTIES>();

    Editor editor;
    private Vector2 scrollPos;

    private void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
        GUILayout.Label("NPC creator window", EditorStyles.boldLabel); //label of the window

        //we'll obtain the UI present in the scene, to test if the portrait is possible or not --------------------
        if (GameObject.FindGameObjectWithTag("UI") && GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>() != null) { dialogueUIController = GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>(); }
        else { dialogueUIController = null; }
        // -------------------------------------------------------------------------------------------------------

        npcName = EditorGUILayout.TextField("NPC name", npcName); //we get the NPC's name value

        //we get the various lists of serialized elements of the NPC
        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        //we get the portrait's string ref for the NPC
        portraitRef = EditorGUILayout.TextField("NPC portrait", portraitRef);


        // ----------------- WE TEST IF ALL VALUES INPUTTED CAN CREATE A VALID NPC ---------------------------
        #region TEST CORRECTNESS IN INPUTTED VALUES
        //test if the npc has a name :
        if (npcName == "")
        {
            GUILayout.Label("NO NPC NAME INPUTTED", EditorStyles.boldLabel);
            GUILayout.EndScrollView();
            return;
        }
        //test if the inputted portrait name is available :
        if (dialogueUIController && !dialogueUIController.ContainsPortrait(portraitRef))
        {
            GUILayout.Label("WRONG PORTRAIT NAME INPUTTED, PLEASE REFER TO AVAILABLE PORTRAITS IN CURRENTLY UI PREFAB", EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if npc has at least starting state :
        if (startingState == null)
        {
            GUILayout.Label("NO STARTING STATE INPUTTED", EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if the properties are correct :
        if (CheckPropertiesCorrectness())
        {
            GUILayout.Label(errorProperties, EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if the player items are correct :
        if (CheckPlayerItemsCorrectness())
        {
            GUILayout.Label(errorPlayerItems, EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if the quest items are correct :
        if (CheckQuestItemsCorrectness())
        {
            GUILayout.Label(errorQuestItems, EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if the react elements are correct :
        if (CheckReactElementsCorrectness())
        {
            GUILayout.Label(errorReactElements, EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        //test if the player properties are correct :
        if (CheckPlayerPropertiesCorrectness())
        {
            GUILayout.Label(errorPlayerProperties, EditorStyles.boldLabel); //label of the window
            GUILayout.EndScrollView();
            return;
        }
        #endregion
        // ------------------------------------------------------------------------------------------------------

        //button that will instantiate the prefab in the scene with the corresponding NPC :
        if (GUILayout.Button("Press to create NPC"))
        {
            CreateNPC();
        }

        GUILayout.EndScrollView();
    }

    /**
     *  Function that will handle the creation of the NPC in itself
     */
    private void CreateNPC()
    {
        instantiatedNPC = Instantiate(basicNPC);
        instantiatedNPC.name = npcName;
        dialogSM = instantiatedNPC.GetComponent<DialogSM>();
        npcController = instantiatedNPC.GetComponent<NPCController>();

        //Adding the list of states to the dialogSM component ----------------------------------------------
        dialogSM.nextStates = new NEXT_STATE[availableStatesList.Count]; //we will simply add the items to the list of the dialogSM's inspector, dialogSM's script will handle the rest
        int stateIndex = 0;  
        foreach (var element in availableStatesList)
        {
            NEXT_STATE nextState;
            nextState.state = element.state;
            nextState.name = element.state.name;
            dialogSM.nextStates[stateIndex] = nextState;
            stateIndex++;
        }
        // ------------------------------------------------------------------------------------------------

        //Adding the list of properties to NPCController --------------------------------------------------
        npcController.fileProperties = new FILE_PROPERTIES[npcProperties.Count];
        int propIndex = 0;
        foreach(var property in npcProperties)
        {
            FILE_PROPERTIES prop = new FILE_PROPERTIES();
            prop = property;
            npcController.fileProperties[propIndex] = prop;
            propIndex++;
        }
        //-------------------------------------------------------------------------------------------------

        //Adding the list of playerItems to NPCController --------------------------------------------------
        npcController.objectsElements = new PLAYER_ITEMS[playerItems.Count];
        int playerItemsIndex = 0;
        foreach (var playerItem in playerItems)
        {
            PLAYER_ITEMS pItem = new PLAYER_ITEMS();
            pItem = playerItem;
            npcController.objectsElements[playerItemsIndex] = pItem;
            playerItemsIndex++;
        }
        //-------------------------------------------------------------------------------------------------

        //Adding the list of questItems to NPCController --------------------------------------------------
        npcController.questItems = new QUEST_ITEMS[questItems.Count];
        int questItemsIndex = 0;
        foreach (var questItem in questItems)
        {
            QUEST_ITEMS qItem = new QUEST_ITEMS();
            qItem = questItem;
            npcController.questItems[questItemsIndex] = qItem;
            questItemsIndex++;
        }
        //-------------------------------------------------------------------------------------------------

        //Adding the list of reactElements to NPCController --------------------------------------------------
        npcController.reactElements = new REACT_ELEMENTS[reactElements.Count];
        int reactElementsIndex = 0;
        foreach (var reactElement in reactElements)
        {
            REACT_ELEMENTS rElement = new REACT_ELEMENTS();
            rElement = reactElement;
            npcController.reactElements[reactElementsIndex] = rElement;
            reactElementsIndex++;
        }
        //-------------------------------------------------------------------------------------------------

        //Adding the list of Player Properties to NPCController --------------------------------------------------
        npcController.reactElements = new REACT_ELEMENTS[reactElements.Count];
        int playerPropertiesIndex = 0;
        foreach (var playerProperty in playerProperties)
        {
            PLAYER_PROPERTIES pProperties = new PLAYER_PROPERTIES();
            pProperties = playerProperty;
            npcController.playerProperties[playerPropertiesIndex] = pProperties;
            playerPropertiesIndex++;
        }
        //-------------------------------------------------------------------------------------------------
        npcController = instantiatedNPC.GetComponent<NPCController>();
    }

    /**
     *  Function that will repaint the editor for the list
     */
    public void OnInspectorGUI()
    {
        Repaint();
    }

    /**
     *  Function that will check that the properties are correct
     */
    private bool CheckPropertiesCorrectness()
    {
        for(int index=0; index < npcProperties.Count; index++)
        {
            bool nameFound = false;
            //Check if the property has a name :
            if (npcProperties[index].propertyName == "")
            {
                errorProperties = "MISSING NAME IN PROPERTY "+index;
                return true;
            }
            //Check if the property has an initial value :
            if(npcProperties[index].propertyValue == "")
            {
                errorProperties = "MISSING VALUE IN PROPERTY " + index;
                return true;
            }
            //check if there is at least one linked state :
            if (npcProperties[index].propertyChangeState.Length == 0)
            {
                errorProperties = "THERE IS NO ACCESSIBLE CHANGE STATE IN PROPERTY "+index;
                return true;
            }
            //Check if the conditions list and booleans isSuperior list have correct lengths compared to one another:
            if (npcProperties[index].propertyType != TYPE.STRING && npcProperties[index].conditionIsSuperior.Length != npcProperties[index].propertyCondition.Length && npcProperties[index].propertyChangeState.Length != npcProperties[index].propertyCondition.Length)
            {
                errorProperties = "NOT ALL LISTS OF CONDITIONS HAVE COHERENT LENGTHS IN PROPERTY OF TYPE INTEGER "+index;
                return true;
            }
            //Check if the conditions list and changeState list have correct lengths compared to one another:
            if (npcProperties[index].propertyType == TYPE.STRING && npcProperties[index].propertyChangeState.Length != npcProperties[index].propertyCondition.Length)
            {
                errorProperties = "NOT ALL LISTS OF CONDITIONS HAVE COHERENT LENGTHS IN PROPERTY OF TYPE STRING " + index;
                return true;
            }
            //Check if the state is correctly linked :
            foreach (var state in availableStatesList)
            {
                for(int i =0; i< npcProperties[index].propertyChangeState.Length; i++)
                {
                    if (state.state && state.state.name == npcProperties[index].propertyChangeState[i]) //if the name is found in the list
                    {
                        nameFound = true;
                    }
                }
            }
            if (!nameFound) { errorProperties = "WRONGLY INPUTTED STATE NAME IN PROPERTY " + index; return true; }
        }
        return false;
    }

    /**
     *  Function that will check that the player items are correct
     */
    private bool CheckPlayerItemsCorrectness()
    {
        for (int index = 0; index < playerItems.Count; index++)
        {
            bool nameFound = false;
            //Check if the property has a name :
            if (playerItems[index].playerItemName == "")
            {
                errorPlayerItems = "MISSING NAME IN ITEM " + index;
                return true;
            }
            //Check if the state is correctly linked :
            foreach (var state in availableStatesList)
            {
                if (state.state && state.state.name == playerItems[index].playerItemChangeState) //if the name is found in the list
                {
                    nameFound = true;
                }
            }
            if (!nameFound) { errorPlayerItems = "WRONGLY INPUTTED OR NON-EXISTANT STATE NAME IN ITEM " + index; return true; }
        }
        return false;
    }

    /**
     *  Function that will check that the quest items are correct
     */
    private bool CheckQuestItemsCorrectness()
    {
        for (int index = 0; index < questItems.Count; index++)
        {
            bool nameFound = false;
            //Check if the quest item is not null :
            if (questItems[index].item == null)
            {
                errorPlayerItems = "MISSING ITEM PREFAB REF IN ITEM " + index;
                return true;
            }
            //Check if the state is correctly linked :
            foreach (var state in availableStatesList)
            {
                if (state.state && questItems[index].item!=null && state.state.name == questItems[index].questChangeState) //if the name is found in the list
                {
                    nameFound = true;
                }
            }
            if (!nameFound) { errorPlayerItems = "WRONGLY INPUTTED OR NON-EXISTANT STATE NAME IN ITEM " + index; return true; }
        }
        return false;
    }

    /**
     *  Function that will check that the react elements are correct
     */
    private bool CheckReactElementsCorrectness()
    {
        for (int index = 0; index < reactElements.Count; index++)
        {
            //Check if the tag is not void :
            if (reactElements[index].tagToReact == "")
            {
                errorReactElements = "MISSING TAG REF IN REACT ELEMENT " + index;
                return true;
            }
            //Check if the three lists have the same amount of elements in each of them :
            if(reactElements[index].condition.Length != reactElements[index].isSuperior.Length || reactElements[index].isSuperior.Length != reactElements[index].stateChangeName.Length || reactElements[index].condition.Length != reactElements[index].stateChangeName.Length)
            {
                errorReactElements = "SUBLISTS ARE NOT EQUAL IN REACT ELEMENT " + index;
                return true;
            }

            //Check if the state is correctly linked :
            for(int subIndex = 0; subIndex < reactElements[index].stateChangeName.Length; subIndex++)
            {
                bool nameFound = false;
                foreach (var state in availableStatesList)
                {
                    if (state.state && state.state.name == reactElements[index].stateChangeName[subIndex]) //if the name is found in the list
                    {
                        nameFound = true;
                    }
                }
                if (!nameFound) { errorReactElements = "WRONGLY INPUTTED OR NON-EXISTANT STATE NAME IN ITEM " + index + " FOR ELEMENT " + subIndex; return true; }
            }
        }
        return false;
    }

    /**
     *  Function that will check that the player properties are correct
     */
    private bool CheckPlayerPropertiesCorrectness()
    {
        for (int index = 0; index < playerProperties.Count; index++)
        {
            //Check that there's at least one state accessible and one condition :
            if (playerProperties[index].playerPropertyCondition.Length==0 || playerProperties[index].playerPropertyChangeState.Length==0)
            {
                errorPlayerProperties = "SUBLISTS OF STATES OR CONDITION ARE EMPTY IN PLAYER PROPERTIES " + index;
                return true;
            }
            //Check if the lists have the same amount of elements in each of them :
            if (playerProperties[index].playerPropertyCondition.Length != playerProperties[index].playerPropertyChangeState.Length)
            {
                errorPlayerProperties = "SUBLISTS ARE NOT EQUAL IN PLAYER PROPERTIES " + index;
                return true;
            }
            //Check if the state is correctly linked :
            for (int subIndex = 0; subIndex < playerProperties[index].playerPropertyChangeState.Length; subIndex++)
            {
                bool nameFound = false;
                foreach (var state in availableStatesList)
                {
                    if (state.state && state.state.name == playerProperties[index].playerPropertyChangeState[subIndex]) //if the name is found in the list
                    {
                        nameFound = true;
                    }
                }
                if (!nameFound) { errorPlayerProperties = "WRONGLY INPUTTED OR NON-EXISTANT STATE NAME IN PLAYER PROPERTY " + index + " FOR ELEMENT " + subIndex; return true; }
            }
        }
        return false;
    }
}

[System.Serializable]
public class DialogStateEditorItem
{
    public DialogState state;
}

public class NPCCreatorWindowDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        var statesList = serializedObject.FindProperty("AvailableStatesList");
        EditorGUILayout.PropertyField(statesList, new GUIContent("Available States List"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var propertiesList = serializedObject.FindProperty("NPCProperties");
        EditorGUILayout.PropertyField(propertiesList, new GUIContent("NPC Properties"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var playerItemsList = serializedObject.FindProperty("PlayerItems");
        EditorGUILayout.PropertyField(playerItemsList, new GUIContent("Player Items to React to"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var questItemsList = serializedObject.FindProperty("QuestItems");
        EditorGUILayout.PropertyField(questItemsList, new GUIContent("Quest Items to give out"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var reactElementsList = serializedObject.FindProperty("ReactElements");
        EditorGUILayout.PropertyField(reactElementsList, new GUIContent("Tagged Elements to react to"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var playerPropertyList = serializedObject.FindProperty("PlayerProperties");
        EditorGUILayout.PropertyField(playerPropertyList, new GUIContent("Player Properties to react to"), true);
        EditorGUILayout.EndHorizontal();

    }
}
