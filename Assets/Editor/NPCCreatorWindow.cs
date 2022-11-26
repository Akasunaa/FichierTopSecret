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
    private string npcName;                         //name of the NPC

    //Elements for DialogSM :
    [SerializeField] DialogStateEditorItem startingState;
    [SerializeField] List<DialogStateEditorItem> availableStatesList = new List<DialogStateEditorItem>();

    //Elements for NPCController :
    [SerializeField] private string portraitRef;
    [SerializeField] List<FILE_ELEMENTS> npcProperties = new List<FILE_ELEMENTS>();
    

    Editor editor;

    private void OnGUI()
    {
        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        GUILayout.Label("NPC creator window",EditorStyles.boldLabel); //label of the window

        npcName = EditorGUILayout.TextField("NPC name", npcName); //we get the NPC's name value


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

        //Adding the list of states to the dialogSM component ----------------------------------------------
        dialogSM.nextStates = new NEXT_STATE[availableStatesList.Count]; //we will simply add the items to the list of the dialogSM's inspector, dialogSM's script will handle the rest
        int i = 0;  
        foreach (var element in availableStatesList)
        {
            NEXT_STATE nextState;
            nextState.state = element.state;
            nextState.name = element.state.name;
            dialogSM.nextStates[i] = nextState;
            i++;
        }
        // ------------------------------------------------------------------------------------------------
        npcController = instantiatedNPC.GetComponent<NPCController>();
    }

    /**
     *  Function that will repaint the editor for the list
     */
    public void OnInspectorGUI()
    {
        Repaint();
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
