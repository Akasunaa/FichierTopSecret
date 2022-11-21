using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

}
