using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 *  Component used by objects that will allow the player to spew dialogue when interacting with them
 */
[RequireComponent(typeof(InteractableObjectController))]
public class ObjectInteractionController : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string dialogueStandard;         //standard dialogue
    [TextArea(3, 10)]
    [SerializeField] private string dialogueOnConditionChange; //dialogue when change condition applied
    private string curDialogue;
    private DialogueUIController ui;                        //reference to the UI used for dialogs

    private void Start()
    {
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(ui);
        if (dialogueStandard == null)
        {
            dialogueStandard = "ERROR : NO DIALOGUE WAS INPUTTED FOR THIS INTERACTION";
        }
        if (dialogueOnConditionChange == null)
        {
            dialogueOnConditionChange = "WARNING : NO INPUTTED DIALOGUE FOR CHANGED CONDITION OF PREFAB. CHECK EDITOR.";
        }
        curDialogue = dialogueStandard;
    }

    /**
     *  Function called by ObjectControllers to display the currently loaded dialogue when interacting with them
     */
    public void DisplayInteractionDialogue()
    {
        ui.DisplayDialogue(curDialogue, "player");
    }

    /**
     *  Function that will end the dialogue display
     */
    public void EndDisplay()
    {
        ui.EndDisplay();
    }

    /**
     *  Function called by exterior scripts to change the currently displayed dialogue
     */
    public void OnChangeDialogue()
    {
        if (curDialogue == dialogueOnConditionChange)
        {
            curDialogue = dialogueStandard;
        }
        else
        {
            curDialogue = dialogueOnConditionChange;
        }
    }
}
