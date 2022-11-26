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

    //Elements for npc :
    private string npcName;                         //name of the NPC

    //Elements for DialogSM :
    [SerializeField] DialogState startingState;
    [SerializeField] List<DialogStateEditorItem> availableStatesList = new List<DialogStateEditorItem>();

    //Elements for NPCController :
    private string portraitRef;
    [SerializeField] List<FILE_PROPERTIES> npcProperties = new List<FILE_PROPERTIES>();

    Editor editor;

    private void OnGUI()
    {
        GUILayout.Label("NPC creator window", EditorStyles.boldLabel); //label of the window

        //we'll obtain the UI present in the scene, to test if the portrait is possible or not --------------------
        if (GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>() != null) { dialogueUIController = GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>(); }
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
            return;
        }
        //test if the inputted portrait name is available :
        if (dialogueUIController && !dialogueUIController.ContainsPortrait(portraitRef))
        {
            GUILayout.Label("WRONG PORTRAIT NAME INPUTTED, PLEASE REFER TO AVAILABLE PORTRAITS IN CURRENTLY UI PREFAB", EditorStyles.boldLabel); //label of the window
            return;
        }
        //test if npc has at least starting state :
        if (startingState == null)
        {
            GUILayout.Label("NO STARTING STATE INPUTTED", EditorStyles.boldLabel); //label of the window
            return;
        }
        //test if the properties are correct :
        if (CheckPropertiesCorrectness())
        {
            GUILayout.Label(errorProperties, EditorStyles.boldLabel); //label of the window
            return;
        }
        #endregion
        // ------------------------------------------------------------------------------------------------------

        //button that will instantiate the prefab in the scene with the corresponding NPC :
        if (GUILayout.Button("Press to create NPC"))
        {
            CreateNPC();
        }
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
     *  Function that will check that an inputted state name in the properties field is the correct one 
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
            //Check if the state is correctly linked :
            foreach (var state in availableStatesList)
            {
                if (state.state && state.state.name == npcProperties[index].propertyChangeState) //if the name is found in the list
                {
                    nameFound = true;
                }
            }
            if (!nameFound) { errorProperties = "WRONGLY INPUTTED STATE NAME IN PROPERTY " + index; return true; }
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
        var statesList = serializedObject.FindProperty("AvailableStatesList");
        EditorGUILayout.PropertyField(statesList, new GUIContent("Available States List"), true);

        var propertiesList = serializedObject.FindProperty("NPCProperties");
        EditorGUILayout.PropertyField(propertiesList, new GUIContent("NPC Properties"), true);
    }
}
